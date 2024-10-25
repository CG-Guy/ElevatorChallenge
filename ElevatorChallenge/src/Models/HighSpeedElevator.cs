using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging; // Ensure this namespace is included
using System;
using System.Threading.Tasks; // Required for Task

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    // File: HighSpeedElevator.cs
    public class HighSpeedElevator : Elevator
    {
        private readonly ILogger<HighSpeedElevator> logger; // Logger field
        public int TargetFloor { get; set; }

        public HighSpeedElevator(int id, int currentFloor, int maxPassengerCapacity, ILogger<HighSpeedElevator> logger)
            : base(id, 10, maxPassengerCapacity, logger, currentFloor) // Set maxFloor to a default value, e.g., 10
        {
            this.logger = logger; // Initialize the logger field
        }

        public override string Direction => IsMoving ? (CurrentFloor < TargetFloor ? "Up" : "Down") : "Stationary";

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
            logger.LogInformation($"High-Speed Elevator {Id} moving to floor {TargetFloor} at high speed.");

            try
            {
                // Call the base MoveAsync method to handle actual movement
                await base.MoveAsync(targetFloor);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while moving High-Speed Elevator {Id} to floor {TargetFloor}: {ex.Message}");
                // Optionally reset IsMoving to false if there's an error
                IsMoving = false;
            }
            finally
            {
                // Ensure the elevator stops moving in case of error or after completion
                if (IsMoving)
                {
                    IsMoving = false;
                    logger.LogInformation($"High-Speed Elevator {Id} has arrived at floor {CurrentFloor}. Status: Stationary.");
                }
            }
        }

        // Override the Stop method
        public override void Stop()
        {
            if (!IsMoving)
            {
                logger.LogWarning($"High-Speed Elevator {Id} is already stationary."); // Log if the elevator is already stopped
                return;
            }

            IsMoving = false; // This will work if IsMoving is protected
            logger.LogInformation($"High-Speed Elevator {Id} has stopped.");
            // Call the base Stop method to ensure proper behavior
            base.Stop();
        }
    }
}
