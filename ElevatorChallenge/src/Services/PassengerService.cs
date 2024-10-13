using ElevatorChallenge.ElevatorChallenge.src.Models;
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

        public void AddPassenger(Passenger passenger)
        {
            if (!_elevator.HasSpaceFor(1))
            {
                throw new InvalidOperationException("Elevator overloaded. Cannot add more passengers.");
            }
            _elevator.AddPassengers(1);
            Console.WriteLine($"Passenger added. Current count: {_elevator.PassengerCount}");
        }

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

        public void UpdateElevatorStatus()
        {
            Console.WriteLine($"Elevator on floor {_elevator.CurrentFloor}, " +
                $"Direction: {_elevator.Direction}, " +
                $"Moving: {_elevator.IsMoving}, " +
                $"Passengers: {_elevator.PassengerCount}/{_elevator.MaxPassengerCapacity}");
        }

        // New method to request an elevator to a floor
        public void RequestElevator(int floor, int passengersWaiting)
        {
            Console.WriteLine($"Requesting elevator to floor {floor} for {passengersWaiting} passengers.");
            // Here you would link to the ElevatorService to handle the request
        }
    }
}
