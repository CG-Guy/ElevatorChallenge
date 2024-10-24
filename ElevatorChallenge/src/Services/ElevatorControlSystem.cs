using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.ElevatorChallenge.src.Services
{
    public class ElevatorControlSystem : IElevatorControlSystem // Ensure this line is correct
    {
        private readonly List<Elevator> _elevators;
        private readonly ILogger<ElevatorControlSystem> _logger;
        private readonly object _lock = new object();

        // Constructor to accept elevator configurations
        public ElevatorControlSystem(IElevatorFactory elevatorFactory, ILogger<ElevatorControlSystem> logger, List<ElevatorConfig> elevatorConfigs)
        {
            // Pass the configurations to create elevators
            _elevators = elevatorFactory.CreateElevators(elevatorConfigs); // No need to pass the logger here
            _logger = logger; // Store the logger for logging operations
        }

        // Change the signature to async
        public async Task RequestElevatorAsync(int targetFloor, int passengerCount)
        {
            lock (_lock)
            {
                Elevator selectedElevator = FindBestElevator(targetFloor, passengerCount);
                if (selectedElevator != null)
                {
                    _logger.LogInformation($"Elevator {selectedElevator.Id} is assigned to go to floor {targetFloor}");
                    // Call MoveAsync instead of Move
                     selectedElevator.MoveAsync(targetFloor);
                }
                else
                {
                    _logger.LogWarning("No available elevator can accommodate the request.");
                }
            }
        }

        public void InitializeElevators() // Updated method signature
        {
            lock (_lock)
            {
                // Example: Initialize with a default GlassElevator; add more as needed
                var glassElevator = new GlassElevator(2, 1, 8, (ILogger<GlassElevator>)_logger); // Provide id, currentFloor, maxPassengerCapacity, and logger
                _elevators.Add(glassElevator);

                // Add other elevator types if necessary
                // _elevators.Add(new StandardElevator(...)); // Uncomment and implement if needed
            }
        }

        private Elevator FindBestElevator(int targetFloor, int passengerCount)
        {
            var availableElevators = _elevators
                .Where(e => e.PassengerCount + passengerCount <= e.MaxPassengerCapacity && !e.IsMoving)
                .ToList();

            if (!availableElevators.Any()) return null; // No available elevators

            // Select the nearest elevator to the target floor
            Elevator bestElevator = availableElevators
                .OrderBy(e => Math.Abs(e.CurrentFloor - targetFloor))
                .FirstOrDefault();

            return bestElevator;
        }
    }
}
