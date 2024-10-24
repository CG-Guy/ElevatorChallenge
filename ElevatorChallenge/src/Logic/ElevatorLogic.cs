// File: ElevatorChallenge/ElevatorChallenge/src/Logic/ElevatorLogic.cs
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System;
using System.Collections.Generic;

namespace ElevatorChallenge.ElevatorChallenge.src.Logic
{
    public class ElevatorLogic : IElevatorLogic
    {
        public Elevator FindNearestElevator(List<Elevator> elevators, int targetFloor)
        {
            if (elevators == null || elevators.Count == 0)
                return null;

            Elevator nearestElevator = null;
            int minimumDistance = int.MaxValue;

            foreach (var elevator in elevators)
            {
                int distance = Math.Abs(elevator.CurrentFloor - targetFloor);

                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    nearestElevator = elevator;
                }
            }

            return nearestElevator;
        }

        public bool CanTakePassengers(PassengerElevator elevator, int additionalPassengers) // Keep using PassengerElevator
        {
            return (elevator.PassengerCount + additionalPassengers) <= elevator.MaxPassengerCapacity;
        }

        public void UpdateElevatorStatus(Elevator elevator)
        {
            Console.WriteLine($"Elevator {elevator.Id} status updated.");
            // Implement additional logic as needed
        }
    }
}
