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

            // No need to move if already on the target floor
            if (targetFloor == elevator.CurrentFloor)
            {
                return; // No need to move
            }

            // Set the direction based on the target floor
            string direction = targetFloor > elevator.CurrentFloor ? "Up" : "Down";
            elevator.IsMoving = true; // Set IsMoving to true

            // Use the method to set the direction
            elevator.SetDirection(direction); // Call the method to set direction

            // Simulate movement to the target floor
            await Task.Delay((int)CalculateMovementTime(elevator.CurrentFloor, targetFloor, elevator.TimePerFloor));

            // Update the current floor after movement
            elevator.CurrentFloor = targetFloor; // Move to target floor
            elevator.IsMoving = false; // Set IsMoving to false
            elevator.SetDirection("Stationary"); // Reset direction to stationary
        }

        private double CalculateMovementTime(int currentFloor, int targetFloor, int timePerFloor)
        {
            return Math.Abs(targetFloor - currentFloor) * timePerFloor; // Calculate movement time based on floors
        }
    }
}
