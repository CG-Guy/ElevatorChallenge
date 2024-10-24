using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorChallenge.Services
{
    public class ElevatorService : IElevatorService
    {
        private readonly List<Elevator> _elevators;
        private readonly ElevatorLogic _elevatorLogic;
        private readonly ElevatorMovementLogic _elevatorMovementLogic;

        // Injecting IEnumerable<Elevator> through DI container
        public ElevatorService(IEnumerable<Elevator> elevators)
        {
            _elevators = elevators?.ToList() ?? throw new ArgumentNullException(nameof(elevators));
            _elevatorLogic = new ElevatorLogic();
            _elevatorMovementLogic = new ElevatorMovementLogic();
        }

        // Method to assign the nearest elevator to a requested floor
        public Elevator AssignElevator(int requestFloor, int passengersWaiting)
        {
            Elevator nearestElevator = null;
            int minDistance = int.MaxValue;
            int fewestPassengers = int.MaxValue;

            foreach (var elevator in _elevators)
            {
                if (elevator == null)
                {
                    Console.WriteLine("Elevator is null.");
                    continue;
                }

                Console.WriteLine($"Checking Elevator {elevator.Id} on floor {elevator.CurrentFloor}");
                int distance = Math.Abs(elevator.CurrentFloor - requestFloor);

                if (elevator.IsInService &&
                    _elevatorLogic.CanTakePassengers((PassengerElevator)elevator, passengersWaiting))
                {
                    if (distance < minDistance ||
                        (distance == minDistance && elevator.PassengerCount < fewestPassengers))
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
                    nearestElevator.SetDirection(requestFloor > nearestElevator.CurrentFloor ? "Up" : "Down"); // Use SetDirection method
                    _elevatorMovementLogic.MoveElevatorToFloor(nearestElevator, requestFloor);
                    nearestElevator.AddPassengers(passengersWaiting);
                    nearestElevator.SetMovingStatus(false);

                    Console.WriteLine($"Elevator {nearestElevator.Id} assigned to floor {requestFloor} with {passengersWaiting} passengers.");
                    return nearestElevator;
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Error assigning passengers to Elevator {nearestElevator.Id}: {ex.Message}");
                }
            }

            Console.WriteLine("No available elevators to assign.");
            return null;
        }

        public void RequestElevator(int requestFloor, int passengersWaiting)
        {
            Console.WriteLine($"Requesting elevator to floor {requestFloor} for {passengersWaiting} passengers...");
            var assignedElevator = AssignElevator(requestFloor, passengersWaiting);

            if (assignedElevator == null)
            {
                Console.WriteLine("Request failed: No available elevators.");
            }
        }

        public List<Elevator> GetElevatorsStatus()
        {
            return _elevators;
        }

        public void ShowElevatorStatus()
        {
            if (_elevators.Count == 0)
            {
                Console.WriteLine("No elevators available.");
                return;
            }

            foreach (var elevator in _elevators)
            {
                Console.WriteLine($"Elevator {elevator.Id} is on floor {elevator.CurrentFloor}, " +
                                  $"Moving: {elevator.IsMoving}, " +
                                  $"Direction: {elevator.Direction}, " +
                                  $"Passengers: {elevator.PassengerCount}/{elevator.MaxPassengerCapacity}, " +
                                  $"In Service: {elevator.IsInService}"); // Added In Service status for clarity
            }
        }

        // Method to add a new elevator to the service
        public void AddElevator(Elevator elevator)
        {
            if (elevator == null)
            {
                throw new ArgumentNullException(nameof(elevator), "Elevator cannot be null.");
            }

            _elevators.Add(elevator);
            Console.WriteLine($"Elevator {elevator.Id} added.");
        }

        // Implementation of the HasAvailableElevators method
        public bool HasAvailableElevators()
        {
            // Check if any elevator is in service and has capacity for more passengers
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
