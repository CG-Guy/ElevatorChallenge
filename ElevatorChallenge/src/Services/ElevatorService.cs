using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorChallenge.Services
{
    public class ElevatorService
    {
        private readonly List<Elevator> _elevators;
        private readonly ElevatorLogic _elevatorLogic;
        private readonly ElevatorMovementLogic _elevatorMovementLogic;

        public ElevatorService(List<Elevator> elevators)
        {
            _elevators = elevators ?? throw new ArgumentNullException(nameof(elevators));
            _elevatorLogic = new ElevatorLogic();
            _elevatorMovementLogic = new ElevatorMovementLogic();
        }

        // Method to assign the nearest elevator to a requested floor
        public Elevator AssignElevator(int requestFloor, int passengersWaiting)
        {
            Elevator nearestElevator = null;
            int minDistance = int.MaxValue;

            // Find the nearest elevator that can take the waiting passengers
            foreach (var elevator in _elevators)
            {
                int distance = Math.Abs(elevator.CurrentFloor - requestFloor);

                // Check if this elevator can take the additional passengers
                if (_elevatorLogic.CanTakePassengers(elevator, passengersWaiting) &&
                    distance < minDistance)
                {
                    minDistance = distance;
                    nearestElevator = elevator; // Set the nearest elevator
                }
            }

            // If we found a valid elevator, move it and assign passengers
            if (nearestElevator != null)
            {
                try
                {
                    nearestElevator.SetMovingStatus(true); // Indicate that the elevator is moving
                    _elevatorMovementLogic.MoveElevatorToFloor(nearestElevator, requestFloor); // Move to the requested floor
                    nearestElevator.AddPassengers(passengersWaiting); // Add waiting passengers
                    nearestElevator.SetMovingStatus(false); // Indicate that the elevator has stopped moving

                    Console.WriteLine($"Elevator {nearestElevator.Id} assigned to floor {requestFloor} with {passengersWaiting} passengers.");
                    return nearestElevator; // Return the assigned elevator
                }
                catch (InvalidOperationException ex) // Handle exceptions related to passenger addition
                {
                    Console.WriteLine($"Error assigning passengers to Elevator {nearestElevator.Id}: {ex.Message}");
                }
            }

            Console.WriteLine("No available elevators to assign.");
            return null; // Return null if no elevator can be assigned
        }

        // Method to request an elevator
        public void RequestElevator(int requestFloor, int passengersWaiting)
        {
            Console.WriteLine($"Requesting elevator to floor {requestFloor} for {passengersWaiting} passengers...");
            AssignElevator(requestFloor, passengersWaiting);
        }

        // Method to retrieve the current status of elevators
        public List<Elevator> GetElevatorsStatus()
        {
            return _elevators; // Return the list of elevators
        }

        // Method to show the current status of elevators
        public void ShowElevatorStatus()
        {
            foreach (var elevator in _elevators)
            {
                Console.WriteLine($"Elevator {elevator.Id} is on floor {elevator.CurrentFloor}, " +
                                  $"Moving: {elevator.IsMoving}, " +
                                  $"Direction: {elevator.Direction}, " + // Updated to use ElevatorDirection
                                  $"Passengers: {elevator.PassengerCount}/{elevator.MaxPassengerCapacity}");
            }
        }
    }
}
