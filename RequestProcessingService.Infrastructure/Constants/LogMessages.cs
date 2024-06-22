namespace RequestProcessingService.Infrastructure.Constants;

internal static class LogMessages
{
    public const string HandledCountMessages = "Handled {Count} messages";

    public const string RetryErrorMessage = "Retry strategy logging: {ExceptionMessage}";

    public const string UnhandledErrorMessage = "Unhandled exception occured";
}