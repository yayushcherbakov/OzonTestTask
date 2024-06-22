using Confluent.Kafka;

namespace RequestProcessingService.Infrastructure.ServiceBus.Interfaces;

public interface IHandler<TKey, TValue>
{
    Task Handle(IReadOnlyCollection<ConsumeResult<TKey, TValue>> messages, CancellationToken token);
}