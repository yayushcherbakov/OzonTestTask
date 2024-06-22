using ReportRequestEventGenerator.Models;

namespace ReportRequestEventGenerator;

internal sealed class ReportRequestEventGenerator
{
    public static IEnumerable<ReportRequestEvent> GenerateEvents(int eventsCount)
    {
        var rnd = new Random();

        return Enumerable
            .Range(start: 0, count: eventsCount)
            .Select(_ => (long)rnd.Next(minValue: 1, maxValue: int.MaxValue))
            .Distinct()
            .Select(x => new ReportRequestEvent()
            {
                RequestId = x,
                ProductId = rnd.Next(minValue: 1, maxValue: int.MaxValue),
                EventConversionCheckPeriod = new EventConversionCheckPeriod()
                {
                    From = DateTimeOffset.Now.AddDays(-rnd.Next(minValue: 7, maxValue: 30)),
                    To = DateTimeOffset.Now.AddDays(-rnd.Next(minValue: 0, maxValue: 6))
                }
            });
    }
}