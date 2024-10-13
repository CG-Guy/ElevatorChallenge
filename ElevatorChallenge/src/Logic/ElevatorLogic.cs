using ElevatorChallenge.ElevatorChallenge.src.Models;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorChallenge.ElevatorChallenge.src.Logic
{
    public class ElevatorLogic
    {
        // Finds the nearest available elevator to the requested floor
        public Elevator FindNearestElevator(List<Elevator> elevators, int targetFloor)
        {
            // Validate input
            if (elevators == null || elevators.Count == 0)
                return null;

            Elevator nearestElevator = null;
            int minimumDistance = int.MaxValue;

            foreach (var elevator in elevators)
            {
                // Calculate the distance from the elevator's current floor to the target floor
                int distance = Math.Abs(elevator.CurrentFloor - targetFloor);

                // Check if this elevator is closer than the previously found nearest elevator
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    nearestElevator = elevator;
                }
            }

            return nearestElevator;
        }

        // Determine if the elevator has capacity to take more passengers
        public bool CanTakePassengers(Elevator elevator, int passengersWaiting)
        {
            // Check if adding waiting passengers exceeds the max passenger capacity
            return (elevator.PassengerCount + passengersWaiting) <= elevator.MaxPassengerCapacity; // Check if within capacity
        }
    }
}
