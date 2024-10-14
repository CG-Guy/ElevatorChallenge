using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorChallenge.Services
{
    public class ElevatorService
    {
        private readonly List<Elevator> _elevators = new List<Elevator>();
        private readonly ElevatorLogic _elevatorLogic;
        private readonly ElevatorMovementLogic _elevatorMovementLogic;

        // Accept IEnumerable instead of List
        public ElevatorService(IEnumerable<Elevator> elevators)
        {
            _elevators = elevators?.ToList() ?? throw new ArgumentNullException(nameof(elevators)); // Ensure elevators are not null
            _elevatorLogic = new ElevatorLogic();
            _elevatorMovementLogic = new ElevatorMovementLogic();
        }

        // Method to assign the nearest elevator to a requested floor
        public Elevator AssignElevator(int requestFloor, int passengersWaiting)
        {
            Elevator nearestElevator = null;
            int minDistance = int.MaxValue;
            int fewestPassengers = int.MaxValue; // To track the minimum passenger count

            foreach (var elevator in _elevators)
            {
                if (elevator == null)
                {
                    Console.WriteLine("Elevator is null.");
                    continue; // Skip to the next iteration
                }

                Console.WriteLine($"Checking Elevator {elevator.Id} on floor {elevator.CurrentFloor}");
                int distance = Math.Abs(elevator.CurrentFloor - requestFloor);

                if (elevator.IsInService && // Ensure elevator is in service
                    _elevatorLogic.CanTakePassengers(elevator, passengersWaiting))
                {
                    // Check for nearest elevator or, if equidistant, with fewer passengers
                    if (distance < minDistance ||
                        (distance == minDistance && elevator.PassengerCount < fewestPassengers))
                    {
                        minDistance = distance;
                        fewestPassengers = elevator.PassengerCount; // Update the fewest passengers
                        nearestElevator = elevator; // Set the nearest elevator
                    }
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
            var assignedElevator = AssignElevator(requestFloor, passengersWaiting); // Use AssignElevator method

            if (assignedElevator == null)
            {
                Console.WriteLine("Request failed: No available elevators.");
            }
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
