using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    // File: HighSpeedElevator.cs
    public class HighSpeedElevator : Elevator
    {
        private readonly ILogger<HighSpeedElevator> logger;
        public int TargetFloor { get; set; }

        // Constructor with default maxFloor set to 10
        public HighSpeedElevator(int id, int currentFloor, int maxPassengerCapacity, ILogger<HighSpeedElevator> logger)
            : base(id, 10, maxPassengerCapacity, logger, currentFloor)
        {
            this.logger = logger;
        }

        public override string Direction => IsMoving ? (CurrentFloor < TargetFloor ? "Up" : "Down") : "Stationary";

        // Implement abstract MoveToFloor method if required by base class
        public override async Task MoveToFloor(int targetFloor)
        {
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
                // Simulate movement delay based on floors and elevator speed
                await Task.Delay((int)(0.5 * TimePerFloor * Math.Abs(CurrentFloor - TargetFloor) * 1000));
                CurrentFloor = targetFloor;
                logger.LogInformation($"High-Speed Elevator {Id} has arrived at floor {CurrentFloor}.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while moving High-Speed Elevator {Id} to floor {TargetFloor}: {ex.Message}");
                IsMoving = false;
            }
            finally
            {
                IsMoving = false;
                logger.LogInformation($"High-Speed Elevator {Id} is now stationary.");
            }
        }

        // Override the Stop method
        public override void Stop()
        {
            if (!IsMoving)
            {
                logger.LogWarning($"High-Speed Elevator {Id} is already stationary.");
                return;
            }

            IsMoving = false;
            logger.LogInformation($"High-Speed Elevator {Id} has stopped.");
            base.Stop();
        }
    }
}
