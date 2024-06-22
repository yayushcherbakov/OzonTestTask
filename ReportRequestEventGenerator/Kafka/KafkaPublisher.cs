﻿using Confluent.Kafka;

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
        var completionSource = new TaskCompletionSource<bool>();
        await using var registration = token.Register(() => completionSource.TrySetCanceled(token));

        int messagesInQueue = 1;

        foreach (var (key, value) in messages)
        {
            Interlocked.Increment(ref messagesInQueue);
            var awaiter = _producer
                .ProduceAsync(
                    _topic,
                    new Message<TKey, TValue> { Key = key, Value = value },
                    token)
                .ConfigureAwait(false)
                .GetAwaiter();

            awaiter.OnCompleted(
                () =>
                {
                    try
                    {
                        awaiter.GetResult();

                        // The counter reaches zero only if the loop has ended.
                        if (Interlocked.Decrement(ref messagesInQueue) == 0)
                            completionSource.SetResult(true);
                    }
                    catch (Exception exception)
                    {
                        completionSource.TrySetException(exception);
                    }
                });
        }

        if (Interlocked.Decrement(ref messagesInQueue) == 0)
            completionSource.TrySetResult(true);

        await completionSource.Task;
    }
}
