using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging; // Ensure this namespace is included
using System;
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
            // Validate the target floor
            if (targetFloor < 1 || targetFloor > MaxFloor)
            {
                logger.LogWarning($"Invalid target floor: {targetFloor}. Current floor: {CurrentFloor} (Max: {MaxFloor})");
                return;
            }

            TargetFloor = targetFloor;
            IsMoving = true;
            logger.LogInformation($"Glass Elevator {Id} moving to floor {TargetFloor}"); // Use logger to log the movement

            try
            {
                // Call the base MoveAsync method to handle actual movement
                await base.MoveAsync(targetFloor);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while moving Glass Elevator {Id} to floor {TargetFloor}: {ex.Message}");
                // Optionally, reset IsMoving to false if there's an error
                IsMoving = false;
            }
            finally
            {
                // Ensure the elevator stops moving in case of error or after completion
                if (IsMoving)
                {
                    IsMoving = false;
                    logger.LogInformation($"Glass Elevator {Id} has arrived at floor {CurrentFloor}. Status: Stationary.");
                }
            }
        }

        public override void Stop() // Override the Stop method to use logging
        {
            if (!IsMoving)
            {
                logger.LogWarning($"Glass Elevator {Id} is already stationary."); // Log if the elevator is already stopped
                return;
            }

            IsMoving = false; // Assuming IsMoving is accessible
            logger.LogInformation($"Glass Elevator {Id} has stopped."); // Use logger to log the stop
            base.Stop(); // Call the base Stop method to ensure proper behavior
        }
    }
}
