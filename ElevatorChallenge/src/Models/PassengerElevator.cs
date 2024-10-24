using Microsoft.Extensions.Logging; // Ensure this namespace is included
using System;
using System.Threading.Tasks; // Ensure this is included for async methods

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    // File: PassengerElevator.cs
    public class PassengerElevator : Elevator
    {
        public int TargetFloor { get; private set; }

        public PassengerElevator(int id, int currentFloor, int maxPassengerCapacity, ILogger<PassengerElevator> logger, int maxFloor = 10)
    : base(id, maxFloor, maxPassengerCapacity, logger)
        {
            SetCurrentFloor(currentFloor); // Ensure this sets the current floor properly
        }

        // Method to validate the target floor
        public int ValidateFloor(int floor, int maxFloor)
        {
            if (floor < 0 || floor > maxFloor)
                throw new ArgumentOutOfRangeException(nameof(floor), $"Floor must be within the valid range of 0 to {maxFloor}.");

            return floor; // Return the valid floor
        }

        // Property to determine the direction of the elevator
        public override string Direction => IsMoving ? (CurrentFloor < TargetFloor ? "Up" : "Down") : "Stationary";

        // Implementing the inherited abstract method for async movement
        public override async Task MoveAsync(int targetFloor) // Change method name to MoveAsync
        {
            // Validate the target floor using the ValidateFloor method
            try
            {
                targetFloor = ValidateFloor(targetFloor, MaxFloor);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine($"Cannot move to floor {targetFloor}. It is out of range.");
                return; // Exit early if the floor is invalid
            }

            // If we reach here, the target floor is valid
            TargetFloor = targetFloor;
            IsMoving = true;
            Console.WriteLine($"Passenger Elevator {Id} moving to floor {TargetFloor}");

            // Simulate movement to the target floor asynchronously
            await MoveToTargetFloorAsync(); // Call the new method

            Stop(); // Stop after moving
        }

        // Asynchronous method to simulate the movement to the target floor
        private async Task MoveToTargetFloorAsync()
        {
            // Simulate the time taken to move to the target floor
            int timeTaken = Math.Abs(CurrentFloor - TargetFloor) * TimePerFloor * 1000; // Calculate delay in milliseconds
            await Task.Delay(timeTaken); // Simulate the delay for moving

            CurrentFloor = TargetFloor; // Move to the target floor
            Console.WriteLine($"Passenger Elevator {Id} arrived at floor {CurrentFloor}");
        }

        // Method to stop the elevator
        public override void Stop()
        {
            IsMoving = false;
            Console.WriteLine($"Passenger Elevator {Id} has stopped.");
        }

        // Override SetCurrentFloor to ensure validation occurs
        protected override void SetCurrentFloor(int floor)
        {
            CurrentFloor = ValidateFloor(floor, MaxFloor); // Use ValidateFloor for validation
        }

        // New method MoveToFloor for the testing scenario
        public async Task MoveToFloor(int targetFloor)
        {
            await MoveAsync(targetFloor); // Calls the MoveAsync method for the elevator
        }
    }
}
