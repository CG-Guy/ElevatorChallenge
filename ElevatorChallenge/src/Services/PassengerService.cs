using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.Services;
using System;

namespace ElevatorChallenge.ElevatorChallenge.src.Services
{
    public class PassengerService
    {
        private readonly Elevator _elevator; // Marked as readonly for better practice

        public PassengerService(Elevator elevator)
        {
            _elevator = elevator ?? throw new ArgumentNullException(nameof(elevator), "Elevator cannot be null."); // Added null check
        }

        /// <summary>
        /// Adds a passenger to the elevator if there is space.
        /// </summary>
        /// <param name="passenger">The passenger to be added.</param>
        public void AddPassenger(Passenger passenger)
        {
            if (passenger == null)
            {
                throw new ArgumentNullException(nameof(passenger), "Passenger cannot be null."); // Ensure passenger is not null
            }

            if (!_elevator.HasSpaceFor(1))
            {
                Console.WriteLine($"Elevator is full. Cannot add more passengers. Current count: {_elevator.PassengerCount}/{_elevator.MaxPassengerCapacity}");
                return; // Exit early if elevator is full
            }

            _elevator.AddPassengers(1);
            Console.WriteLine($"Passenger added successfully. Current count: {_elevator.PassengerCount}/{_elevator.MaxPassengerCapacity}");
        }

        /// <summary>
        /// Removes a passenger if there are any passengers present.
        /// </summary>
        public void RemovePassenger()
        {
            if (_elevator.PassengerCount > 0)
            {
                _elevator.RemovePassengers(1);
                Console.WriteLine($"Passenger removed successfully. Current count: {_elevator.PassengerCount}/{_elevator.MaxPassengerCapacity}");
            }
            else
            {
                Console.WriteLine("No passengers to remove.");
            }
        }

        /// <summary>
        /// Prints the current status of the elevator.
        /// </summary>
        public void UpdateElevatorStatus()
        {
            Console.WriteLine($"Elevator on floor {_elevator.CurrentFloor}, " +
                              $"Direction: {_elevator.Direction}, " +
                              $"Moving: {_elevator.IsMoving}, " +
                              $"Passengers: {_elevator.PassengerCount}/{_elevator.MaxPassengerCapacity}");
        }

        /// <summary>
        /// Method to request an elevator to a floor (delegates to ElevatorService).
        /// </summary>
        /// <param name="elevatorService">The elevator service to handle requests.</param>
        /// <param name="floor">The target floor for the elevator.</param>
        /// <param name="passengersWaiting">The number of passengers waiting on the requested floor.</param>
        public void RequestElevator(ElevatorService elevatorService, int floor, int passengersWaiting)
        {
            if (elevatorService == null)
            {
                throw new ArgumentNullException(nameof(elevatorService), "Elevator service cannot be null."); // Added null check for elevatorService
            }

            if (_elevator.IsInService) // Check if the elevator is in service before proceeding
            {
                if (!_elevator.HasSpaceFor(passengersWaiting))
                {
                    Console.WriteLine($"Cannot dispatch elevator to floor {floor}. Elevator is full.");
                    return; // Exit early if elevator is full
                }

                Console.WriteLine($"Requesting elevator to floor {floor} for {passengersWaiting} passengers.");
                elevatorService.AssignElevator(floor, passengersWaiting); // Delegates to ElevatorService
                Console.WriteLine($"Elevator dispatched to floor {floor} with {passengersWaiting} passengers.");
            }
            else
            {
                Console.WriteLine($"Elevator {_elevator.Id} is not in service and cannot be dispatched.");
            }
        }
    }
}
