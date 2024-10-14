using ElevatorChallenge.ElevatorChallenge.src.Models;

namespace ElevatorChallenge.ElevatorChallenge.src.Logic
{
    public class ElevatorMovementLogic
    {
        public async Task MoveElevatorToFloor(Elevator elevator, int targetFloor)
        {
            // Debug output for initial state
            Console.WriteLine($"Starting MoveElevatorToFloor. Current Floor: {elevator.CurrentFloor}, Target Floor: {targetFloor}, Max Floor: {elevator.MaxFloor}");

            // Check if the target floor is valid
            if (targetFloor < 1 || targetFloor > elevator.MaxFloor)
            {
                Console.WriteLine($"Invalid target floor: {targetFloor}. Current floor: {elevator.CurrentFloor} (Max: {elevator.MaxFloor})");
                return; // Early return if the target floor is invalid
            }

            // Debug output for current and target floor
            Console.WriteLine($"Current Floor: {elevator.CurrentFloor}, Target Floor: {targetFloor}");

            // If already at the target floor, do nothing
            if (targetFloor == elevator.CurrentFloor)
            {
                Console.WriteLine("Already at the target floor.");
                return; // No need to move
            }

            // Set direction and start moving
            string direction = targetFloor > elevator.CurrentFloor ? "Up" : "Down";
            elevator.IsMoving = true;
            elevator.SetDirection(direction);
            Console.WriteLine($"Elevator is moving {direction}.");

            // Simulate movement delay
            var movementTime = CalculateMovementTime(elevator.CurrentFloor, targetFloor, elevator.TimePerFloor);
            Console.WriteLine($"Simulating movement delay of {movementTime} milliseconds.");
            await Task.Delay(movementTime);

            // Update current floor after movement
            elevator.CurrentFloor = targetFloor; // Move to target floor
            elevator.IsMoving = false; // Not moving anymore
            elevator.SetDirection("Stationary");
            Console.WriteLine($"Elevator has arrived at floor {elevator.CurrentFloor}. Status: {elevator.Direction}, Is Moving: {elevator.IsMoving}");
        }

        private int CalculateMovementTime(int currentFloor, int targetFloor, int timePerFloor)
        {
            return Math.Abs(targetFloor - currentFloor) * timePerFloor; // Simplified calculation
        }
    }
}
