using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorChallenge.Services
{
    public class ElevatorService : IElevatorService
    {
        private readonly List<Elevator> _elevators; // Field to store elevators
        private readonly ElevatorLogic _elevatorLogic;
        private readonly ElevatorMovementLogic _elevatorMovementLogic;
        private readonly IApplicationLogger _logger; // Changed to use the interface
        private readonly IApplicationLogger _elevatorLogger; // Logger for elevator-specific operations

        public ElevatorService(
            IEnumerable<Elevator> elevators,
            IApplicationLogger logger,
            IApplicationLogger elevatorLogger,
            IElevatorValidator elevatorValidator,
            ElevatorManagementService elevatorManagementService)
        {
            _elevators = elevators?.Cast<Elevator>().ToList() ?? throw new ArgumentNullException(nameof(elevators));
            _logger = logger; // Store the logger for logging operations
            _elevatorLogger = elevatorLogger; // Initialize elevator logger
            _elevatorLogic = new ElevatorLogic();
            _elevatorMovementLogic = new ElevatorMovementLogic(elevatorValidator); // Pass the elevatorValidator to the ElevatorMovementLogic constructor
        }

        // Get status of all elevators
        public string GetElevatorStatus()
        {
            if (_elevators.Count == 0)
            {
                return "No elevators available.";
            }

            var statusList = new List<string>();
            foreach (var elevator in _elevators)
            {
                statusList.Add($"Elevator {elevator.Id}: " +
                               $"Floor {elevator.CurrentFloor}, " +
                               $"Moving: {elevator.IsMoving}, " +
                               $"Direction: {elevator.Direction}, " +
                               $"Passengers: {elevator.PassengerCount}/{elevator.MaxPassengerCapacity}, " +
                               $"In Service: {elevator.IsInService}");
            }

            return string.Join(Environment.NewLine, statusList);
        }

        public async Task<Elevator> AssignElevatorAsync(int requestFloor, int passengersWaiting)
        {
            Elevator nearestElevator = null;
            int minDistance = int.MaxValue;
            int fewestPassengers = int.MaxValue;

            // Find the best elevator based on criteria
            foreach (var elevator in _elevators)
            {
                if (elevator == null)
                {
                    _elevatorLogger.LogWarning("Elevator is null.");
                    continue;
                }

                _elevatorLogger.LogInformation($"Checking Elevator {elevator.Id} on floor {elevator.CurrentFloor}");
                int distance = Math.Abs(elevator.CurrentFloor - requestFloor);

                // Check if the elevator is in service and can take more passengers
                if (elevator.IsInService &&
                    _elevatorLogic.CanTakePassengers((PassengerElevator)elevator, passengersWaiting))
                {
                    // Choose the nearest elevator or the one with the fewest passengers
                    if (distance < minDistance ||
                        (distance == minDistance && elevator.PassengerCount < fewestPassengers))
                    {
                        minDistance = distance;
                        fewestPassengers = elevator.PassengerCount;
                        nearestElevator = elevator;
                    }
                }
            }

            // If no suitable elevator was found, select the first available one
            if (nearestElevator == null)
            {
                nearestElevator = _elevators.FirstOrDefault(); // Just take the first available elevator
                _elevatorLogger.LogWarning("No suitable elevators found; assigning the first available elevator.");
            }

            // Move the assigned elevator to the requested floor
            if (nearestElevator != null)
            {
                try
                {
                    nearestElevator.SetMovingStatus(true);
                    nearestElevator.SetDirection(requestFloor > nearestElevator.CurrentFloor ? "Up" : "Down");
                    await _elevatorMovementLogic.MoveElevatorToFloor(nearestElevator, requestFloor); // Assuming this method is async
                    nearestElevator.AddPassengers(passengersWaiting);
                    nearestElevator.SetMovingStatus(false);

                    _elevatorLogger.LogInformation($"Elevator {nearestElevator.Id} assigned to floor {requestFloor} with {passengersWaiting} passengers.");
                    return nearestElevator;
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError($"Error assigning passengers to Elevator {nearestElevator.Id}: {ex.Message}");
                }
            }

            // This point should not be reached, but we ensure something is returned
            _elevatorLogger.LogWarning("Returning an elevator despite errors.");
            return nearestElevator; // Return whatever elevator is available
        }

        // Request an elevator to a specified floor
        public async Task RequestElevatorAsync(int requestFloor, int passengersWaiting)
        {
            _elevatorLogger.LogInformation($"Requesting elevator to floor {requestFloor} for {passengersWaiting} passengers...");
            var assignedElevator = await AssignElevatorAsync(requestFloor, passengersWaiting);

            if (assignedElevator == null)
            {
                _elevatorLogger.LogWarning("Request failed: No available elevators.");
            }
        }

        // Get current status of elevators
        public List<Elevator> GetElevatorsStatus()
        {
            return _elevators;
        }

        // Show status of elevators in the log
        public void ShowElevatorStatus()
        {
            if (_elevators.Count == 0)
            {
                _elevatorLogger.LogInformation("No elevators available.");
                return;
            }

            foreach (var elevator in _elevators)
            {
                _elevatorLogger.LogInformation($"Elevator {elevator.Id} is on floor {elevator.CurrentFloor}, " +
                                               $"Moving: {elevator.IsMoving}, " +
                                               $"Direction: {elevator.Direction}, " +
                                               $"Passengers: {elevator.PassengerCount}/{elevator.MaxPassengerCapacity}, " +
                                               $"In Service: {elevator.IsInService}"); // Added In Service status for clarity
            }
        }

        // Add a new elevator to the service
        public void AddElevator(Elevator elevator)
        {
            if (elevator == null)
            {
                throw new ArgumentNullException(nameof(elevator), "Elevator cannot be null.");
            }

            _elevators.Add(elevator);
            _elevatorLogger.LogInformation($"Elevator {elevator.Id} added.");
        }

        // Check if there are available elevators
        public bool HasAvailableElevators()
        {
            foreach (var elevator in _elevators)
            {
                if (elevator.IsInService && elevator.PassengerCount < elevator.MaxPassengerCapacity)
                {
                    return true; // There is at least one available elevator
                }
            }

            return false; // No elevators are available
        }
    }
}
