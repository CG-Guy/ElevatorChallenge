using System;

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    // Represents an Elevator object focusing only on its state.
    public abstract class Elevator
    {
        public int Id { get; set; }
        public int CurrentFloor { get; set; }
        public int PassengerCount { get; private set; }
        public int MaxPassengerCapacity { get; set; }
        public bool IsMoving { get; set; }
        public int MaxFloor { get; private set; }
        public int TimePerFloor { get; private set; } = 1; // Default time per floor (in seconds)
        public bool IsInService { get; set; }

        // Mark the Stop method as virtual to allow overriding
        public virtual void Stop()
        {
            IsMoving = false; // Logic to stop the elevator
            Console.WriteLine($"Elevator {Id} has stopped.");
        }

        // Add the TargetFloor property
        public int TargetFloor { get; set; } // New property to track the target floor

        // Mark the Direction property as virtual to allow overriding
        public virtual string Direction => IsMoving ? (CurrentFloor < TargetFloor ? "Up" : "Down") : "Stationary";

        public bool IsOriginal { get; internal set; }

        // Constructor to initialize the elevator with an ID, maximum floor, and initial passengers
        public Elevator(int id, int maxFloor, int maxPassengerCapacity, int currentFloor = 1, int currentPassengers = 0)
        {
            Id = id;
           MaxFloor = maxFloor;
            MaxPassengerCapacity = maxPassengerCapacity;
            CurrentFloor = currentFloor; // Ensure this is set correctly
            PassengerCount = currentPassengers; // Set initial passengers
            IsMoving = false;
        }

        // Abstract method for moving the elevator
        public abstract void Move(int targetFloor);

        private string _direction; // Private backing field for direction

        // Method to set the direction
        public void SetDirection(string direction)
        {
            // Allow only valid directions
            if (direction == "Up" || direction == "Down" || direction == "Stationary")
            {
                _direction = direction; // Set the current direction
            }
            else
            {
                throw new ArgumentException("Invalid direction. Use 'Up', 'Down', or 'Stationary'.");
            }
        }

        // Method to add passengers
        public bool AddPassengers(int count)
        {
            if (HasSpaceFor(count))
            {
                PassengerCount += count;
                return true; // Successfully added passengers
            }
            return false; // Capacity exceeded
        }

        // Method to remove passengers
        public void RemovePassengers(int count)
        {
            if (count > PassengerCount)
            {
                throw new InvalidOperationException("Cannot remove more passengers than currently on the elevator.");
            }
            PassengerCount -= count;
        }

        // Method to check if there is space for additional passengers
        public bool HasSpaceFor(int numberOfPassengers)
        {
            return (PassengerCount + numberOfPassengers) <= MaxPassengerCapacity;
        }

        // Method to set the current floor, ensuring it's within bounds
        public void SetCurrentFloor(int floor)
        {
            if (floor < 1 || floor > MaxFloor)
            {
                throw new ArgumentOutOfRangeException(nameof(floor), "Floor must be within the valid range.");
            }
            CurrentFloor = floor;
        }

        // Method to set the moving status
        public void SetMovingStatus(bool isMoving)
        {
            IsMoving = isMoving;
        }

        // Method to move the elevator to a specific floor
        public void MoveToFloor(int floor)
        {
            if (floor < 1 || floor > MaxFloor)
            {
                throw new ArgumentOutOfRangeException(nameof(floor), "Floor must be within the valid range.");
            }

            // Determine direction based on current floor and target floor
            SetDirection(CurrentFloor > floor ? "Down" : "Up");
            SetMovingStatus(true);

            // Simulate moving to the target floor
            SetCurrentFloor(floor); // Move to the requested floor

            SetMovingStatus(false);
            SetDirection("Stationary"); // Reset direction when stopped
        }

        public override string ToString()
        {
            return $"Elevator ID: {Id}, Current Floor: {CurrentFloor}, Moving: {IsMoving}, Direction: {Direction}, Passengers: {PassengerCount}/{MaxPassengerCapacity}";
        }
    }
}
