using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.ElevatorChallenge.src.Repositories
{
    public class ElevatorRepository : IElevatorRepository // Implementing the repository interface
    {
        private readonly ConcurrentDictionary<int, Elevator> _elevators; // Thread-safe in-memory storage with dictionary for quicker lookup
        private readonly ILogger<ElevatorRepository> _logger;
        private readonly ElevatorSelector _elevatorSelector;
        private List<Elevator> elevators;

        public ElevatorRepository(IEnumerable<Elevator> elevators, ILogger<ElevatorRepository> logger) // Accept IEnumerable for flexibility
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
            _elevatorSelector = new ElevatorSelector((ILogger<ElevatorSelector>)logger);

            if (elevators == null)
            {
                throw new ArgumentNullException(nameof(elevators), "Elevators collection cannot be null.");
            }

            _elevators = new ConcurrentDictionary<int, Elevator>(elevators.ToDictionary(e => e.Id));
        }

        public ElevatorRepository(List<Elevator> elevators)
        {
            this.elevators = elevators;
        }

        public IElevator FindBestElevator(int targetFloor, int passengerCount)
        {
            ValidateInput(targetFloor, passengerCount);

            var availableElevators = _elevators.Values
                .Where(e => e.PassengerCount + passengerCount <= e.MaxPassengerCapacity && !e.IsMoving)
                .Cast<IElevator>() // Ensure we are returning IElevator
                .ToList();

            if (!availableElevators.Any())
            {
                _logger.LogWarning("No available elevators found.");
                return null;
            }

            return _elevatorSelector.SelectBestElevator(availableElevators, targetFloor);
        }

        public bool TryAddElevator(Elevator elevator)
        {
            if (elevator == null)
            {
                throw new ArgumentNullException(nameof(elevator), "Elevator cannot be null.");
            }

            // Thread-safe addition of elevator
            if (!_elevators.TryAdd(elevator.Id, elevator))
            {
                _logger.LogError($"Elevator with ID {elevator.Id} already exists.");
                return false;
            }

            return true;
        }

        public IReadOnlyList<Elevator> GetAllElevators()
        {
            return _elevators.Values.ToList().AsReadOnly();
        }

        public Elevator GetElevatorById(int id)
        {
            _elevators.TryGetValue(id, out Elevator elevator);
            return elevator;
        }

        private void ValidateInput(int targetFloor, int passengerCount)
        {
            if (targetFloor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetFloor), "Target floor cannot be negative.");
            }
            if (passengerCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(passengerCount), "Passenger count cannot be negative.");
            }
        }
    }

    public class ElevatorSelector // Separating the selection logic into its own class for SRP
    {
        private readonly ILogger<ElevatorSelector> _logger;

        public ElevatorSelector(ILogger<ElevatorSelector> logger)
        {
            _logger = logger;
        }

        public IElevator SelectBestElevator(IEnumerable<IElevator> availableElevators, int targetFloor)
        {
            var bestElevator = availableElevators
                .OrderBy(e => Math.Abs(e.CurrentFloor - targetFloor))
                .FirstOrDefault();

            _logger.LogInformation($"Best elevator selected: ID {bestElevator?.Id} for target floor {targetFloor}");
            return bestElevator;
        }
    }
}
