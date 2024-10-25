using Microsoft.Extensions.Logging; // Ensure this namespace is included
using System;
using System.Threading.Tasks; // Ensure this is included for async methods

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    // File: PassengerElevator.cs
    public class PassengerElevator : Elevator
    {
        public int TargetFloor { get; private set; }
        private readonly ILogger<PassengerElevator> logger; // Logger field

        // Constructor to initialize the elevator with current floor and capacity
        public PassengerElevator(int id, int currentFloor, int maxPassengerCapacity, ILogger<PassengerElevator> logger, int maxFloor = 10)
            : base(id, maxFloor, maxPassengerCapacity, logger)
        {
            this.logger = logger; // Initialize the logger field
            SetCurrentFloor(currentFloor); // Ensure this sets the current floor properly
        }

        // Method to validate the target floor
        private int ValidateFloor(int floor)
        {
            if (floor < 0 || floor > MaxFloor)
                throw new ArgumentOutOfRangeException(nameof(floor), $"Floor must be within the valid range of 0 to {MaxFloor}.");

            return floor; // Return the valid floor
        }

        // Property to determine the direction of the elevator
        public override string Direction => IsMoving ? (CurrentFloor < TargetFloor ? "Up" : "Down") : "Stationary";

        // Implementing the inherited abstract method for async movement
        public override async Task MoveAsync(int targetFloor)
        {
            // Validate the target floor using the ValidateFloor method
            try
            {
                targetFloor = ValidateFloor(targetFloor); // Validates target floor
            }
            catch (ArgumentOutOfRangeException)
            {
                logger.LogWarning($"Cannot move to floor {targetFloor}. It is out of range."); // Use logger for warnings
                return; // Exit early if the floor is invalid
            }

            // If we reach here, the target floor is valid
            if (!IsInService)
            {
                logger.LogWarning($"Elevator {Id} is out of service."); // Log warning if not in service
                return; // Exit if the elevator is not in service
            }

            TargetFloor = targetFloor;
            IsMoving = true;
            logger.LogInformation($"Passenger Elevator {Id} moving to floor {TargetFloor}"); // Use logger for movement

            // Simulate movement to the target floor asynchronously
            await MoveToTargetFloorAsync();

            Stop(); // Stop after moving
        }

        // Asynchronous method to simulate the movement to the target floor
        private async Task MoveToTargetFloorAsync()
        {
            // Simulate the time taken to move to the target floor
            int timeTaken = Math.Abs(CurrentFloor - TargetFloor) * TimePerFloor * 1000; // Calculate delay in milliseconds
            await Task.Delay(timeTaken); // Simulate the delay for moving

            CurrentFloor = TargetFloor; // Move to the target floor
            logger.LogInformation($"Passenger Elevator {Id} arrived at floor {CurrentFloor}"); // Use logger for arrival
        }

        // Method to stop the elevator
        public override void Stop()
        {
            IsMoving = false;
            logger.LogInformation($"Passenger Elevator {Id} has stopped."); // Use logger for stop
        }

        // Override SetCurrentFloor to ensure validation occurs
        public override void SetCurrentFloor(int floor)
        {
            CurrentFloor = ValidateFloor(floor); // Use ValidateFloor for validation
        }

        // Updated MoveToFloor method to validate max floor limit
        public async Task MoveToFloor(int targetFloor)
        {
            if (targetFloor > MaxFloor)
            {
                throw new InvalidOperationException($"Cannot move above max floor {MaxFloor}.");
            }

            // Remaining movement logic, if any
        }
    }
}
