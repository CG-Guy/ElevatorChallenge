// File: ElevatorChallenge/ElevatorChallenge/src/Logic/ElevatorAssignmentLogic.cs
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System;
using System.Collections.Generic;

public class ElevatorAssignmentLogic : IElevatorAssignmentLogic
{
    private readonly IElevatorLogic _elevatorLogic;
    private readonly ILogger _logger;

    public ElevatorAssignmentLogic(IElevatorLogic elevatorLogic, ILogger logger)
    {
        _elevatorLogic = elevatorLogic;
        _logger = logger;
    }

    public Elevator AssignElevator(int requestFloor, int passengersWaiting, IEnumerable<Elevator> elevators)
    {
        Elevator nearestElevator = null;
        int minDistance = int.MaxValue;
        int fewestPassengers = int.MaxValue;

        foreach (var elevator in elevators)
        {
            if (elevator == null)
            {
                _logger.Info("Elevator is null.");
                continue;
            }

            _logger.Info($"Checking Elevator {elevator.Id} on floor {elevator.CurrentFloor}");
            int distance = Math.Abs(elevator.CurrentFloor - requestFloor);

            if (elevator.IsInService && _elevatorLogic.CanTakePassengers(elevator as PassengerElevator, passengersWaiting))
            {
                if (distance < minDistance || (distance == minDistance && elevator.PassengerCount < fewestPassengers))
                {
                    minDistance = distance;
                    fewestPassengers = elevator.PassengerCount;
                    nearestElevator = elevator;
                }
            }
        }

        return nearestElevator;
    }
}
