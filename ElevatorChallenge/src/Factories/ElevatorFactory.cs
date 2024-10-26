using System;
using System.Collections.Generic;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.ElevatorChallenge.src.Factories
{
    public class ElevatorFactory : IElevatorFactory
    {
        private readonly int _maxFloor; // Maximum floor for all elevators
        private readonly ILoggerFactory _loggerFactory; // Logger factory for creating loggers

        // Constructor to set the max floor and logger factory for the elevators
        public ElevatorFactory(int maxFloor, ILoggerFactory loggerFactory)
        {
            if (maxFloor < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxFloor), "Maximum floor must be greater than or equal to 1.");
            }
            _maxFloor = maxFloor;
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory)); // Ensure loggerFactory is not null
        }

        // Create a single elevator instance with a logger parameter
        public Elevator CreateElevator(int id, int maxPassengerCapacity, int currentFloor, ILogger<Elevator> logger)
        {
            // Debug output
            Console.WriteLine($"Creating elevator with ID: {id}, Max Floor: {_maxFloor}, Current Floor: {currentFloor}");

            // Validate currentFloor before creating the elevator
            if (currentFloor < 1 || currentFloor > _maxFloor)
            {
                throw new ArgumentOutOfRangeException(nameof(currentFloor), $"Current floor {currentFloor} must be within the valid range [1, {_maxFloor}].");
            }

            // Return a new elevator instance, ensuring the logger is passed
            return new StandardElevator(id, _maxFloor, maxPassengerCapacity, currentFloor, (ILogger<StandardElevator>)logger);
        }

        // Implement the method to create multiple elevators based on configurations
        public List<Elevator> CreateElevators(List<ElevatorConfig> elevatorConfigs)
        {
            var elevators = new List<Elevator>();

            foreach (var config in elevatorConfigs)
            {
                int elevatorId = config.Id; // Directly use Id as it is already an int

                // Validate currentFloor before creating the elevator
                if (config.CurrentFloor < 1 || config.CurrentFloor > _maxFloor)
                {
                    throw new ArgumentOutOfRangeException(nameof(config.CurrentFloor),
                        $"Current floor {config.CurrentFloor} for elevator ID {elevatorId} must be within the valid range [1, {_maxFloor}].");
                }

                // Validate max passenger capacity
                if (config.MaxPassengerCapacity <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(config.MaxPassengerCapacity),
                        $"Max passenger capacity must be greater than 0 for elevator ID {elevatorId}.");
                }

                // Debug output
                Console.WriteLine($"Creating elevator with ID: {elevatorId}, Max Passenger Capacity: {config.MaxPassengerCapacity}, Current Floor: {config.CurrentFloor}");

                // Create a logger specifically for StandardElevator
                var logger = _loggerFactory.CreateLogger<Elevator>();

                // Create the elevator using its configuration
                elevators.Add(CreateElevator(elevatorId, config.MaxPassengerCapacity, config.CurrentFloor, logger));
            }

            return elevators;
        }
    }
}
