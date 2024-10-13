using ElevatorChallenge.ElevatorChallenge.src.Models;

namespace ElevatorChallenge.ElevatorChallenge.src.Logic
{
    public class ElevatorMovementLogic
    {
        public async Task MoveElevatorToFloor(Elevator elevator, int targetFloor)
        {
            // Ensure that the target floor is within the bounds
            if (targetFloor < 1 || targetFloor > elevator.MaxFloor)
            {
                return; // Invalid target floor; do not change the current floor, just return
            }

            if (targetFloor == elevator.CurrentFloor)
            {
                return; // No need to move if already on the target floor
            }

            // Set the direction based on the target floor
            string direction = targetFloor > elevator.CurrentFloor ? "Up" : "Down";
            elevator.SetDirection(direction);

            // Set IsMoving to true
            elevator.SetMovingStatus(true);

            // Simulate movement to the target floor
            await Task.Delay((int)CalculateMovementTime(elevator.CurrentFloor, targetFloor, elevator.TimePerFloor));

            // Update the current floor after movement
            elevator.SetCurrentFloor(targetFloor);
            elevator.SetMovingStatus(false);
            elevator.SetDirection("Stationary");
        }

        private int CalculateMovementTime(int currentFloor, int targetFloor, int timePerFloor)
        {
            int numberOfFloorsToMove = Math.Abs(targetFloor - currentFloor);
            return numberOfFloorsToMove * timePerFloor; // Total time = floors to move * time per floor
        }
    }
}
