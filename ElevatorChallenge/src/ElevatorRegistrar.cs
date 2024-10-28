using ElevatorChallenge.ElevatorChallenge.src.Models;
using global::ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ElevatorChallenge.ElevatorChallenge.src
{
    public class ElevatorRegistrar
    {
        private readonly IServiceCollection _services;
        private readonly ILogger<ElevatorRegistrar> _logger;
        private readonly ILoggerFactory _loggerFactory; // Added this line

        public ElevatorRegistrar(IServiceCollection services, ILogger<ElevatorRegistrar> logger, ILoggerFactory loggerFactory) // Updated constructor
        {
            _services = services;
            _logger = logger;
            _loggerFactory = loggerFactory; // Initialize the logger factory
        }

        public ILogger<ElevatorRegistrar> GetLogger()
        {
            return _logger;
        }

        public void RegisterElevators(int totalFloors, List<ElevatorConfig> elevators)
        {
            if (elevators == null || elevators.Count == 0)
            {
                _logger.LogWarning("No elevators configured in the loaded configuration.");
                return;
            }

            _logger.LogInformation($"Registering {elevators.Count} elevators.");
            var elevatorInstances = new List<IElevator>();

            foreach (var elevatorConfig in elevators)
            {
                var loggerForElevator = _loggerFactory.CreateLogger<ConcreteElevator>(); // Use logger factory to create logger
                var elevator = new ConcreteElevator(
                    elevatorConfig.Id,
                    elevatorConfig.MaxPassengerCapacity,
                    totalFloors,
                    loggerForElevator,
                    elevatorConfig.CurrentFloor,
                    elevatorConfig.CurrentPassengers
                );

                elevatorInstances.Add(elevator);
                _logger.LogInformation($"Registered elevator {elevatorConfig.Id} with capacity {elevatorConfig.MaxPassengerCapacity} at floor {elevator.CurrentFloor} with {elevator.CurrentPassengers} passengers.");
            }

            foreach (var elevator in elevatorInstances)
            {
                _services.AddSingleton(elevator);
            }

            // Additional logic to add new elevators can be added here.
        }
    }
}
