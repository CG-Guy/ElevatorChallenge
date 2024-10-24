using Microsoft.Extensions.Logging; // Ensure this namespace is included
using System;
using System.Threading.Tasks; // Required for Task

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    public class StandardElevator : Elevator
    {
        private readonly ILogger<StandardElevator> logger; // Logger field

        public StandardElevator(int id, int maxFloor, int maxPassengerCapacity, int currentFloor, ILogger<StandardElevator> logger)
            : base(id, maxFloor, maxPassengerCapacity, logger, currentFloor) // Pass logger to base constructor
        {
            this.logger = logger; // Initialize the logger field
        }

        // Override the MoveAsync method to implement movement logic for the standard elevator
        public override async Task MoveAsync(int targetFloor)
        {
            logger.LogInformation($"Standard Elevator {Id} is moving from floor {CurrentFloor} to floor {targetFloor}.");

            // Ensure the target floor is valid
            if (targetFloor < 1 || targetFloor > MaxFloor)
            {
                throw new ArgumentOutOfRangeException(nameof(targetFloor), "Floor must be within the valid range.");
            }

            // Determine direction based on current floor and target floor
            SetDirection(CurrentFloor > targetFloor ? "Down" : "Up");
            SetMovingStatus(true);

            // Call the base MoveAsync method to handle actual movement
            await base.MoveAsync(targetFloor);

            // Reset direction and moving status after reaching the target floor
            SetMovingStatus(false);
            SetDirection("Stationary"); // Reset direction when stopped
        }

        // Override the Stop method if needed (optional)
        public override void Stop()
        {
            base.Stop(); // Call the base class Stop method
            logger.LogInformation($"Standard Elevator {Id} has stopped."); // Log the stop event
        }
    }
}
