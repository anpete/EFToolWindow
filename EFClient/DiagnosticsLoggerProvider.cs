using System;
using Microsoft.Extensions.Logging;
using NetMQ;
using NetMQ.Sockets;

namespace Microsoft.EFClient
{
    public class DiagnosticsLoggerProvider : ILoggerProvider
    {
        private readonly PublisherSocket _publisherSocket = new PublisherSocket();

        public DiagnosticsLoggerProvider()
        {
            _publisherSocket.Options.SendHighWatermark = 1000;
            _publisherSocket.Bind(address: "tcp://localhost:12345");
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DiagnosticsLogger(categoryName, this);
        }

        private void Publish(string categoryName, string message)
        {
            lock (_publisherSocket)
            {
                _publisherSocket.SendMoreFrame(categoryName).SendFrame(message);
            }
        }

        public void Dispose()
        {
            _publisherSocket.Dispose();
        }

        private class DiagnosticsLogger : ILogger
        {
            private readonly string _categoryName;
            private readonly DiagnosticsLoggerProvider _diagnosticsLoggerProvider;

            public DiagnosticsLogger(string categoryName, DiagnosticsLoggerProvider diagnosticsLoggerProvider)
            {
                _categoryName = categoryName;
                _diagnosticsLoggerProvider = diagnosticsLoggerProvider;
            }

            public void Log<TState>(
                LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _diagnosticsLoggerProvider.Publish(_categoryName, formatter(state, exception));
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return new NullDisposable();
            }

            private class NullDisposable : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }
}