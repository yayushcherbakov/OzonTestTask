using Confluent.Kafka;

namespace ReportRequestEventGenerator.Kafka;

internal sealed class KafkaPublisher<TKey, TValue> : IDisposable
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly string _topic;

    public KafkaPublisher(
        string bootstrapServers,
        string topic,
        ISerializer<TKey>? keySerializer,
        ISerializer<TValue>? valueSerializer)
    {
        _topic = topic;

        var builder = new ProducerBuilder<TKey, TValue>(
            new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                BatchSize = 50000,
                LingerMs = 5
            });

        if (keySerializer is not null)
        {
            builder.SetKeySerializer(keySerializer);
        }

        if (valueSerializer is not null)
        {
            builder.SetValueSerializer(valueSerializer);
        }

        _producer = builder.Build();
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }

    public async Task Publish(IEnumerable<(TKey key, TValue value)> messages, CancellationToken token)
    {
        // Индикатор завершения задачи публикации сообщений.
        var completionSource = new TaskCompletionSource<bool>();

        // Регистрация отмены задачи, если токен сработает.
        await using var registration = token.Register(() => completionSource.TrySetCanceled(token));

        // Количество сообщений в очереди публикации сообщений.
        int messagesInQueue = 1;

        foreach (var (key, value) in messages)
        {
            // Потокобезопасно увеличивает счетчик на единицу.
            Interlocked.Increment(ref messagesInQueue);

            // Асинхронная публикация сообщения без ожидания выполнения.
            var awaiter = _producer
                .ProduceAsync(
                    _topic,
                    new Message<TKey, TValue> { Key = key, Value = value },
                    token)
                .ConfigureAwait(false)
                .GetAwaiter();

            // Здесь мы подписываемся на событие завершения задачи.
            awaiter.OnCompleted(
                () =>
                {
                    try
                    {
                        awaiter.GetResult();

                        // Если счетчик достигает нуля, вызывается completionSource.SetResult(true),
                        // сигнализируя об успешном завершении всех операций.
                        if (Interlocked.Decrement(ref messagesInQueue) == 0)
                        {
                            completionSource.SetResult(true);
                        }
                    }
                    catch (Exception exception)
                    {
                        // В случае исключения задача завершается с ошибкой.
                        completionSource.TrySetException(exception);
                    }
                });
        }

        // Проверка вызывается еще раз для покрытия случая,
        // когда все сообщения были отправлены до попаданию в эту строчку кода.
        if (Interlocked.Decrement(ref messagesInQueue) == 0)
        {
            completionSource.TrySetResult(true);
        }


        // Задача будет выполнена после того, как все сообщения будут опубликованы, и messagesInQueue станет 
        // равным нулю, что означает завершение всех асинхронных операций по публикации сообщений, или когда возникнет
        // исключения при отправке сообщения. (вызван метод SetResult, TrySetResult, TrySetException).
        await completionSource.Task;
    }
}