using Microsoft.Extensions.Logging;

namespace Shared.Extensions;

public static class ExceptionExtentions
{
    public static void LogError(this Exception ex, ILogger logger, string logContext)
    {
        logger.LogError("{logContext:l} Has Error. Message: {message:l}", logContext, ex.Message);
    }
}