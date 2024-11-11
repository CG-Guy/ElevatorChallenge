using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace ElevatorChallenge.ElevatorChallenge.src.Repositories
{
    public class ElevatorRepository : IElevatorRepository // Implementing the repository interface
    {
        private readonly ConcurrentDictionary<int, Elevator> _elevators;
        private readonly ILogger<ElevatorRepository> _logger;
        private readonly ElevatorSelector _elevatorSelector;
        private readonly object _lockObject = new(); // Ensuring thread-safe access for non-ConcurrentDictionary operations
        private List<Elevator> elevators;

        public ElevatorRepository(IEnumerable<Elevator> elevators, ILogger<ElevatorRepository> logger)
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
            try
            {
                ValidateInput(targetFloor, passengerCount);

                var availableElevators = _elevators.Values
                    .Where(e => e.PassengerCount + passengerCount <= e.MaxPassengerCapacity && !e.IsMoving)
                    .Cast<IElevator>()
                    .ToList();

                if (!availableElevators.Any())
                {
                    _logger.LogWarning("No available elevators found.");
                    return null;
                }

                return _elevatorSelector.SelectBestElevator(availableElevators, targetFloor);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError($"Input validation failed: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while selecting the best elevator.");
                throw new InvalidOperationException("An error occurred while processing your request.", ex);
            }
        }

        public bool TryAddElevator(Elevator elevator)
        {
            try
            {
                if (elevator == null)
                {
                    throw new ArgumentNullException(nameof(elevator), "Elevator cannot be null.");
                }

                if (!_elevators.TryAdd(elevator.Id, elevator))
                {
                    _logger.LogError("Elevator ID conflict occurred.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while adding an elevator.");
                throw new InvalidOperationException("An error occurred while adding the elevator.", ex);
            }
        }

        public IReadOnlyList<Elevator> GetAllElevators()
        {
            try
            {
                return _elevators.Values.ToList().AsReadOnly();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving elevators.");
                throw new InvalidOperationException("An error occurred while retrieving the list of elevators.", ex);
            }
        }

        public Elevator GetElevatorById(int id)
        {
            try
            {
                if (!_elevators.TryGetValue(id, out var elevator))
                {
                    _logger.LogWarning("Elevator not found.");
                }
                return elevator;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving an elevator.");
                throw new InvalidOperationException("An error occurred while retrieving the elevator.", ex);
            }
        }

        private void ValidateInput(int targetFloor, int passengerCount)
        {
            if (targetFloor < 0 || targetFloor > 100) // Adjust max floor as per building constraints
            {
                throw new ArgumentOutOfRangeException(nameof(targetFloor), "Target floor out of range.");
            }
            if (passengerCount < 0 || passengerCount > 20) // Adjust max capacity based on actual limits
            {
                throw new ArgumentOutOfRangeException(nameof(passengerCount), "Passenger count out of range.");
            }
        }

        // Hashing example for security (for unique elevator IDs if needed)
        private string HashElevatorId(int elevatorId)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(elevatorId.ToString()));
                return BitConverter.ToString(data).Replace("-", "").ToLower();
            }
        }
    }

    public class ElevatorSelector
    {
        private readonly ILogger<ElevatorSelector> _logger;

        public ElevatorSelector(ILogger<ElevatorSelector> logger)
        {
            _logger = logger;
        }

        public IElevator SelectBestElevator(IEnumerable<IElevator> availableElevators, int targetFloor)
        {
            try
            {
                var bestElevator = availableElevators
                    .OrderBy(e => Math.Abs(e.CurrentFloor - targetFloor))
                    .FirstOrDefault();

                _logger.LogInformation("Best elevator selected.");
                return bestElevator;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during elevator selection.");
                throw new InvalidOperationException("An error occurred while selecting the best elevator.", ex);
            }
        }
    }
}
