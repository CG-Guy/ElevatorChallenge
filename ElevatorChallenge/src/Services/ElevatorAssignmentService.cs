using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorChallenge.Services
{
    public class ElevatorAssignmentService
    {
        private readonly List<Elevator> _elevators;
        private readonly ElevatorLogic _elevatorLogic;
        private readonly IApplicationLogger _elevatorLogger;
        private readonly ElevatorMovementLogic _elevatorMovementLogic;

        public ElevatorAssignmentService(
            IEnumerable<Elevator> elevators,
            IApplicationLogger elevatorLogger,
            ElevatorMovementLogic elevatorMovementLogic)
        {
            _elevators = elevators?.ToList() ?? throw new ArgumentNullException(nameof(elevators));
            _elevatorLogger = elevatorLogger;
            _elevatorLogic = new ElevatorLogic();
            _elevatorMovementLogic = elevatorMovementLogic;
        }

        public async Task<Elevator> AssignElevatorAsync(int requestFloor, int passengersWaiting)
        {
            Elevator nearestElevator = null;
            int minDistance = int.MaxValue;
            int fewestPassengers = int.MaxValue;

            foreach (var elevator in _elevators)
            {
                if (elevator == null)
                {
                    _elevatorLogger.LogWarning("Elevator is null.");
                    continue;
                }

                _elevatorLogger.LogInformation($"Checking Elevator {elevator.Id} on floor {elevator.CurrentFloor}");
                int distance = Math.Abs(elevator.CurrentFloor - requestFloor);

                if (elevator.IsInService && _elevatorLogic.CanTakePassengers((PassengerElevator)elevator, passengersWaiting))
                {
                    if (distance < minDistance || (distance == minDistance && elevator.PassengerCount < fewestPassengers))
                    {
                        minDistance = distance;
                        fewestPassengers = elevator.PassengerCount;
                        nearestElevator = elevator;
                    }
                }
            }

            if (nearestElevator != null)
            {
                try
                {
                    nearestElevator.SetMovingStatus(true);
                    nearestElevator.SetDirection(requestFloor > nearestElevator.CurrentFloor ? "Up" : "Down");
                    await _elevatorMovementLogic.MoveElevatorToFloor(nearestElevator, requestFloor);
                    nearestElevator.AddPassengers(passengersWaiting);
                    nearestElevator.SetMovingStatus(false);

                    _elevatorLogger.LogInformation($"Elevator {nearestElevator.Id} assigned to floor {requestFloor} with {passengersWaiting} passengers.");
                    return nearestElevator;
                }
                catch (InvalidOperationException ex)
                {
                    _elevatorLogger.LogError($"Error assigning passengers to Elevator {nearestElevator.Id}: {ex.Message}");
                }
            }

            _elevatorLogger.LogWarning("No available elevators to assign.");
            return null;
        }

        public async Task RequestElevatorAsync(int requestFloor, int passengersWaiting)
        {
            _elevatorLogger.LogInformation($"Requesting elevator to floor {requestFloor} for {passengersWaiting} passengers...");
            var assignedElevator = await AssignElevatorAsync(requestFloor, passengersWaiting);

            if (assignedElevator == null)
            {
                _elevatorLogger.LogWarning("Request failed: No available elevators.");
            }
        }
    }
}
