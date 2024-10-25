using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using System;

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    public class Passenger : IPassenger
    {
        public int Id { get; }
        public int CurrentFloor { get; private set; } // Where the passenger is currently
        public int DestinationFloor { get; private set; }
        public bool IsBoarded { get; private set; } // Indicates if the passenger is in the elevator

        public Passenger(int id, int currentFloor, int destinationFloor)
        {
            if (destinationFloor < 0 || currentFloor < 0)
                throw new ArgumentException("Floor cannot be negative.");

            Id = id;
            CurrentFloor = currentFloor;
            DestinationFloor = destinationFloor;
            IsBoarded = false; // Initial state is not boarded
        }

        // Method to board the passenger
        public void Board()
        {
            ValidateBoarding();
            IsBoarded = true;
            Console.WriteLine($"Passenger {Id} has boarded the elevator.");
        }

        // Method to exit the elevator
        public void Exit()
        {
            ValidateExiting();
            IsBoarded = false;
            Console.WriteLine($"Passenger {Id} has exited the elevator at floor {DestinationFloor}.");
        }

        // Method to update the destination floor
        public void UpdateDestinationFloor(int newFloor)
        {
            if (newFloor < 0)
                throw new ArgumentException("Destination floor cannot be negative.");

            DestinationFloor = newFloor;
            Console.WriteLine($"Passenger {Id} updated destination to floor {DestinationFloor}.");
        }

        private void ValidateBoarding()
        {
            if (IsBoarded)
            {
                throw new InvalidOperationException("Cannot board again. Passenger is already on board."); // Throws exception if already boarded
            }
        }

        private void ValidateExiting()
        {
            if (!IsBoarded)
            {
                throw new InvalidOperationException("Passenger is not boarded and cannot exit."); // Throw exception if not boarded
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Passenger other)
            {
                return Id == other.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
