using System.Text.Json;
using System.Text.Json.Serialization;
using ReportRequestEventGenerator.Kafka;
using ReportRequestEventGenerator.Models;

const string bootstrapServers = "kafka:9092";
const string topicName = "report_request_events";
const int eventsCount = 100;
const int timeoutMs = 5 * 60 * 1000;

using var cts = new CancellationTokenSource(timeoutMs);
var publisher = new KafkaPublisher<long, ReportRequestEvent>(
    bootstrapServers,
    topicName,
    keySerializer: null,
    new SystemTextJsonSerializer<ReportRequestEvent>(new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } }));

var generator = new ReportRequestEventGenerator.ReportRequestEventGenerator();

var messages = ReportRequestEventGenerator.ReportRequestEventGenerator.GenerateEvents(eventsCount)
    .Select(e => (e.RequestId, e));

await publisher.Publish(messages, cts.Token);

Console.WriteLine("Done!");