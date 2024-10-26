using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    public class PassengerElevator : Elevator
    {
        public int TargetFloor { get; private set; }
        private readonly ILogger<PassengerElevator> logger;

        // Constructor to initialize elevator with current floor and capacity
        public PassengerElevator(int id, int currentFloor, int maxPassengerCapacity, ILogger<PassengerElevator> logger, int maxFloor = 10)
            : base(id, maxFloor, maxPassengerCapacity, logger)
        {
            this.logger = logger;
            SetCurrentFloor(currentFloor);
        }

        // Method to validate floor range
        private int ValidateFloor(int floor)
        {
            if (floor < 0 || floor > MaxFloor)
                throw new ArgumentOutOfRangeException(nameof(floor), $"Floor must be within the valid range of 0 to {MaxFloor}.");
            return floor;
        }

        // Property to determine the elevator's direction
        public override string Direction => IsMoving ? (CurrentFloor < TargetFloor ? "Up" : "Down") : "Stationary";

        // Override to implement async movement to the target floor
        public override async Task MoveAsync(int targetFloor)
        {
            try
            {
                targetFloor = ValidateFloor(targetFloor);
            }
            catch (ArgumentOutOfRangeException)
            {
                logger.LogWarning($"Cannot move to floor {targetFloor}. It is out of range.");
                return;
            }

            if (!IsInService)
            {
                logger.LogWarning($"Elevator {Id} is out of service.");
                return;
            }

            TargetFloor = targetFloor;
            IsMoving = true;
            logger.LogInformation($"Passenger Elevator {Id} moving to floor {TargetFloor}");

            await MoveToTargetFloorAsync();

            Stop();
        }

        // Simulates moving to the target floor asynchronously
        private async Task MoveToTargetFloorAsync()
        {
            int timeTaken = Math.Abs(CurrentFloor - TargetFloor) * TimePerFloor * 1000;
            await Task.Delay(timeTaken);

            CurrentFloor = TargetFloor;
            logger.LogInformation($"Passenger Elevator {Id} arrived at floor {CurrentFloor}");
        }

        // Stops the elevator and logs the action
        public override void Stop()
        {
            if (IsMoving)
            {
                IsMoving = false;
                logger.LogInformation($"Passenger Elevator {Id} has stopped.");
            }
            else
            {
                logger.LogWarning($"Passenger Elevator {Id} is already stationary.");
            }
        }

        // Validates and sets the current floor
        public override void SetCurrentFloor(int floor)
        {
            CurrentFloor = ValidateFloor(floor);
        }

        // Provides synchronous method to move to a validated floor
        public override async Task MoveToFloor(int targetFloor)
        {
            if (targetFloor > MaxFloor)
            {
                throw new InvalidOperationException($"Cannot move above max floor {MaxFloor}.");
            }

            TargetFloor = ValidateFloor(targetFloor);
            CurrentFloor = TargetFloor;
            logger.LogInformation($"Passenger Elevator {Id} moved directly to floor {CurrentFloor}.");

            await Task.CompletedTask; // Return a completed task for asynchronous compatibility
        }
    }
}
