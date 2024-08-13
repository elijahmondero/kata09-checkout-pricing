using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace CheckoutPricing.Api.Tests.Support;

public class TestOutputLoggerProvider(ITestOutputHelper output) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new TestOutputLogger(output, categoryName);
    }

    public void Dispose()
    {
    }

    private class TestOutputLogger(ITestOutputHelper output, string categoryName) : ILogger
    {
        IDisposable ILogger.BeginScope<TState>(TState state) => null!;

        bool ILogger.IsEnabled(LogLevel logLevel) => true;

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = $"{logLevel.ToString()}: {categoryName} - {formatter(state, exception)}";
            if (exception != null)
            {
                message += $"\nException: {exception.Message}\nStack Trace: {exception.StackTrace}";
            }
            output.WriteLine(message);
        }
    }
}