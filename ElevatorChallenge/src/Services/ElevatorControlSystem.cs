using System.Collections.Generic;
using System.Threading.Tasks;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.ElevatorChallenge.src.Repositories;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.ElevatorChallenge.src.Services
{
    public class ElevatorControlSystem : IElevatorControlSystem
    {
        private readonly ElevatorRepository _elevatorRepository; // Use repository
        private readonly ILogger<ElevatorControlSystem> _logger;
        private readonly object _lock = new object();

        // Constructor to accept elevator configurations
        public ElevatorControlSystem(IElevatorFactory elevatorFactory, ILogger<ElevatorControlSystem> logger, List<ElevatorConfig> elevatorConfigs)
        {
            // Create elevators using the factory and pass to repository
            var elevators = elevatorFactory.CreateElevators(elevatorConfigs);
            _elevatorRepository = new ElevatorRepository(elevators); // Initialize repository
            _logger = logger; // Store the logger for logging operations
        }

        // Change the signature to async
        public async Task RequestElevatorAsync(int targetFloor, int passengerCount)
        {
            Elevator selectedElevator = null;

            lock (_lock)
            {
                selectedElevator = _elevatorRepository.FindBestElevator(targetFloor, passengerCount);
                if (selectedElevator != null)
                {
                    _logger.LogInformation($"Elevator {selectedElevator.Id} is assigned to go to floor {targetFloor}");
                }
                else
                {
                    _logger.LogWarning("No available elevator can accommodate the request.");
                    return; // Exit early if no elevator is available
                }
            }

            // Await MoveAsync outside of the lock to prevent blocking other threads
            try
            {
                await selectedElevator.MoveAsync(targetFloor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to move Elevator {selectedElevator.Id} to floor {targetFloor}."); // Log the error
            }
        }

        public void InitializeElevators()
        {
            lock (_lock)
            {
                // Initialize with various elevator types based on configuration if needed
                var glassElevator = new GlassElevator(2, 1, 8, (ILogger<GlassElevator>)_logger); // Provide id, currentFloor, maxPassengerCapacity, and logger

                // Use the TryAddElevator method instead of AddElevator
                _elevatorRepository.TryAddElevator(glassElevator);

                // Uncomment and implement other elevator types as necessary
                // var standardElevator = new StandardElevator(...); 
                // _elevatorRepository.TryAddElevator(standardElevator);
            }
        }
    }
}
