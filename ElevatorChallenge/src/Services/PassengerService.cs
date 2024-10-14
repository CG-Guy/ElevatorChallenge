using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.Services;
using System;

namespace ElevatorChallenge.ElevatorChallenge.src.Services
{
    public class PassengerService
    {
        private Elevator _elevator;

        public PassengerService(Elevator elevator)
        {
            _elevator = elevator;
        }

        // Adds a passenger to the elevator if there is space
        public void AddPassenger(Passenger passenger)
        {
            if (!_elevator.HasSpaceFor(1))
            {
                throw new InvalidOperationException("Elevator overloaded. Cannot add more passengers.");
            }
            _elevator.AddPassengers(1);
            Console.WriteLine($"Passenger added. Current count: {_elevator.PassengerCount}");
        }

        // Removes a passenger if there are any passengers present
        public void RemovePassenger()
        {
            if (_elevator.PassengerCount > 0)
            {
                _elevator.RemovePassengers(1);
                Console.WriteLine($"Passenger removed. Current count: {_elevator.PassengerCount}");
            }
            else
            {
                throw new InvalidOperationException("No passengers to remove.");
            }
        }

        // Prints the current status of the elevator
        public void UpdateElevatorStatus()
        {
            Console.WriteLine($"Elevator on floor {_elevator.CurrentFloor}, " +
                              $"Direction: {_elevator.Direction}, " +
                              $"Moving: {_elevator.IsMoving}, " +
                              $"Passengers: {_elevator.PassengerCount}/{_elevator.MaxPassengerCapacity}");
        }

        // Method to request an elevator to a floor (delegates to ElevatorService)
        public void RequestElevator(ElevatorService elevatorService, int floor, int passengersWaiting)
        {
            if (_elevator.IsInService) // Check if the elevator is in service before proceeding
            {
                Console.WriteLine($"Requesting elevator to floor {floor} for {passengersWaiting} passengers.");
                elevatorService.AssignElevator(floor, passengersWaiting); // Delegates to ElevatorService
            }
            else
            {
                Console.WriteLine($"Elevator {_elevator.Id} is not in service and cannot be dispatched.");
            }
        }
    }
}
