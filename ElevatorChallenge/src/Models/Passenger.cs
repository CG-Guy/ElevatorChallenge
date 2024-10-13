using System;

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    public class Passenger
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
            IsBoarded = true;
            Console.WriteLine($"Passenger {Id} has boarded the elevator.");
        }

        // Method to exit the elevator
        public void Exit()
        {
            IsBoarded = false;
            Console.WriteLine($"Passenger {Id} has exited the elevator at floor {DestinationFloor}.");
        }

        // Add this method to update the destination floor
        public void UpdateDestinationFloor(int newFloor)
        {
            if (newFloor < 0)
                throw new ArgumentException("Destination floor cannot be negative.");

            DestinationFloor = newFloor;
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
