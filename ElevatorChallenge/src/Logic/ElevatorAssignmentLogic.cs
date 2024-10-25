using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorChallenge.ElevatorChallenge.src.Logic
{
    public class ElevatorAssignmentLogic : IElevatorAssignmentLogic
    {
        private readonly IElevatorLogic _elevatorLogic;
        private readonly IElevatorRepository _elevatorRepository; // Reference to the repository
        private readonly ILogger _logger;

        public ElevatorAssignmentLogic(IElevatorLogic elevatorLogic, IElevatorRepository elevatorRepository, ILogger logger)
        {
            _elevatorLogic = elevatorLogic ?? throw new ArgumentNullException(nameof(elevatorLogic));
            _elevatorRepository = elevatorRepository ?? throw new ArgumentNullException(nameof(elevatorRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Elevator AssignElevator(int requestFloor, int passengersWaiting, IEnumerable<Elevator> elevators)
        {
            if (elevators == null)
            {
                throw new ArgumentNullException(nameof(elevators), "Elevators collection cannot be null.");
            }

            var availableElevators = elevators
                .Where(elevator => elevator != null && elevator.IsInService && !elevator.IsMoving &&
                                  _elevatorLogic.CanTakePassengers(elevator, passengersWaiting))
                .Select(elevator => new
                {
                    Elevator = elevator,
                    Distance = Math.Abs(elevator.CurrentFloor - requestFloor),
                    PassengerCount = elevator.PassengerCount
                });

            var nearestElevator = availableElevators
                .OrderBy(e => e.Distance)
                .ThenBy(e => e.PassengerCount)
                .FirstOrDefault()?.Elevator;

            if (nearestElevator == null)
            {
                _logger.Info("No available elevators found for the request.");
            }
            else
            {
                _logger.Info($"Assigning Elevator {nearestElevator.Id} to floor {requestFloor}");
            }

            return nearestElevator;
        }
    }
}
