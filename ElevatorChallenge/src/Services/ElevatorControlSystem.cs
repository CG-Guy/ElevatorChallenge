using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.ElevatorChallenge.src.Services
{
    public class ElevatorControlSystem : IElevatorControlSystem
    {
        private readonly IElevatorRepository _elevatorRepository;
        private readonly ILogger<ElevatorControlSystem> _logger;
        private readonly object _lock = new object();

        public ElevatorControlSystem(IElevatorRepository elevatorRepository, ILogger<ElevatorControlSystem> logger)
        {
            _elevatorRepository = elevatorRepository ?? throw new ArgumentNullException(nameof(elevatorRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RequestElevatorAsync(int targetFloor, int passengerCount)
        {
            IElevator selectedElevator;

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
                    return;
                }
            }

            try
            {
                await selectedElevator.MoveAsync(targetFloor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to move Elevator {selectedElevator.Id} to floor {targetFloor}.");
            }
        }

        public void InitializeElevators(IElevatorFactory elevatorFactory, List<ElevatorConfig> elevatorConfigs)
        {
            lock (_lock)
            {
                var elevators = elevatorFactory.CreateElevators(elevatorConfigs);
                foreach (var elevator in elevators)
                {
                    if (!_elevatorRepository.TryAddElevator(elevator))
                    {
                        _logger.LogWarning($"Elevator with ID {elevator.Id} could not be added to the repository.");
                    }
                }
            }
        }
    }
}
