using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging; // Ensure this namespace is included
using System.Threading.Tasks; // Required for Task

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    public class GlassElevator : Elevator
    {
        private readonly ILogger<GlassElevator> logger; // Logger field
        public int TargetFloor { get; set; }

        // Constructor to initialize GlassElevator with id, currentFloor, and maxPassengerCapacity
        public GlassElevator(int id, int currentFloor, int maxPassengerCapacity, ILogger<GlassElevator> logger, int maxFloor = 10)
            : base(id, maxFloor, maxPassengerCapacity, logger, currentFloor, 0) // Pass logger to base constructor
        {
            this.logger = logger; // Initialize the logger field
        }

        public override async Task MoveAsync(int targetFloor)
        {
            TargetFloor = targetFloor;
            IsMoving = true;
            logger.LogInformation($"Glass Elevator {Id} moving to floor {TargetFloor}"); // Use logger to log the movement

            // Call the base MoveAsync method to handle actual movement
            await base.MoveAsync(targetFloor);
        }

        public override void Stop() // Override the Stop method to use logging
        {
            IsMoving = false; // Assuming IsMoving is accessible
            logger.LogInformation($"Glass Elevator {Id} has stopped."); // Use logger to log the stop
            base.Stop(); // Call the base Stop method to ensure proper behavior
        }
    }
}
