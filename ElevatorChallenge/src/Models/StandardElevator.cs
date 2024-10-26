using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    public class StandardElevator : Elevator
    {
        private readonly ILogger<StandardElevator> logger;
        private readonly object moveLock = new object();

        public StandardElevator(int id, int maxFloor, int maxPassengerCapacity, int currentFloor, ILogger<StandardElevator> logger)
            : base(id, maxFloor, maxPassengerCapacity, logger, currentFloor)
        {
            this.logger = logger;
        }

        // Implement the MoveToFloor method to satisfy the abstract class requirement
        public override async Task MoveToFloor(int targetFloor)
        {
            await MoveAsync(targetFloor);
        }

        // Override the MoveAsync method to implement movement logic for the standard elevator
        public override async Task MoveAsync(int targetFloor)
        {
            if (targetFloor < 1 || targetFloor > MaxFloor)
            {
                logger.LogWarning($"Target floor {targetFloor} is out of range.");
                throw new ArgumentOutOfRangeException(nameof(targetFloor), "Floor must be within the valid range.");
            }

            logger.LogInformation($"Standard Elevator {Id} is moving from floor {CurrentFloor} to floor {targetFloor}.");

            string direction = CurrentFloor > targetFloor ? "Down" : "Up";
            SetDirection(direction);
            SetMovingStatus(true);

            await MoveElevatorAsync(targetFloor);

            SetMovingStatus(false);
            SetDirection("Stationary");
            logger.LogInformation($"Standard Elevator {Id} has reached floor {CurrentFloor} and stopped.");
        }

        private async Task MoveElevatorAsync(int targetFloor)
        {
            lock (moveLock)
            {
                int timeTaken = Math.Abs(CurrentFloor - targetFloor) * 1000;
                SetCurrentFloor(targetFloor);
            }

            await Task.Delay(2000);
        }

        public override void Stop()
        {
            base.Stop();
            logger.LogInformation($"Standard Elevator {Id} has stopped.");
        }
    }
}
