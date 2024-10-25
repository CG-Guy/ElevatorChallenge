using Microsoft.Extensions.Logging; // Ensure this namespace is included
using System;
using System.Threading;
using System.Threading.Tasks; // Required for Task

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    public class StandardElevator : Elevator
    {
        private readonly ILogger<StandardElevator> logger; // Logger field
        private readonly object moveLock = new object(); // Lock for thread safety

        public StandardElevator(int id, int maxFloor, int maxPassengerCapacity, int currentFloor, ILogger<StandardElevator> logger)
            : base(id, maxFloor, maxPassengerCapacity, logger, currentFloor) // Pass logger to base constructor
        {
            this.logger = logger; // Initialize the logger field
        }

        // Override the MoveAsync method to implement movement logic for the standard elevator
        public override async Task MoveAsync(int targetFloor)
        {
            // Validate the target floor
            if (targetFloor < 1 || targetFloor > MaxFloor)
            {
                logger.LogWarning($"Target floor {targetFloor} is out of range."); // Log warning if out of range
                throw new ArgumentOutOfRangeException(nameof(targetFloor), "Floor must be within the valid range.");
            }

            logger.LogInformation($"Standard Elevator {Id} is moving from floor {CurrentFloor} to floor {targetFloor}.");

            string direction = CurrentFloor > targetFloor ? "Down" : "Up";
            SetDirection(direction); // Ensure this method is implemented in the Elevator class
            SetMovingStatus(true); // Ensure this method is implemented in the Elevator class

            // Move the elevator
            await MoveElevatorAsync(targetFloor);

            // Reset direction and moving status after reaching the target floor
            SetMovingStatus(false);
            SetDirection("Stationary"); // Reset direction when stopped
            logger.LogInformation($"Standard Elevator {Id} has reached floor {CurrentFloor} and stopped.");
        }

        private async Task MoveElevatorAsync(int targetFloor)
        {
            // Lock to ensure thread safety during movement
            lock (moveLock)
            {
                // Simulate the movement with a delay
                // In a real-world application, you'd include actual movement logic here
                Thread.Sleep(2000); // Simulate the time taken to move synchronously

                // Move to the target floor (Update the current floor)
                SetCurrentFloor(targetFloor); // Move to the target floor
            }

            // Await here to keep the method asynchronous
            await Task.CompletedTask; // Complete the task asynchronously
        }

        // Override the Stop method if needed (optional)
        public override void Stop()
        {
            base.Stop(); // Call the base class Stop method
            logger.LogInformation($"Standard Elevator {Id} has stopped."); // Log the stop event
        }
    }
}
