using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.ElevatorChallenge.src.Services.Logging
{
    public class ApplicationLogger : IApplicationLogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public ApplicationLogger(ILogger<ApplicationLogger> logger) // Generic ILogger
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Ensure logger is not null
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogError(string message)
        {
            _logger.LogError(message);
        }
    }
}
