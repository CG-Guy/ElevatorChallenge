using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.Services
{
    public class ElevatorLogger
    {
        private readonly ILogger<ElevatorLogger> _logger;

        public ElevatorLogger(ILogger<ElevatorLogger> logger)
        {
            _logger = logger;
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogError(Exception ex, string message)
        {
            _logger.LogError(ex, message);
        }
    }
}
