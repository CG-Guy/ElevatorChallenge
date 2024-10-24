// File: ElevatorChallenge/ElevatorChallenge/src/Logic/ElevatorMovementLogic.cs
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Logic
{
    public class ElevatorMovementLogic : IElevatorMovementLogic
    {
        public async Task MoveElevatorToFloor(Elevator elevator, int targetFloor)
        {
            Console.WriteLine($"Starting MoveElevatorToFloor. Current Floor: {elevator.CurrentFloor}, Target Floor: {targetFloor}, Max Floor: {elevator.MaxFloor}");

            // Validate the elevator object
            if (elevator == null)
            {
                Console.WriteLine("Elevator object is null.");
                return; // Avoid null reference issues
            }

            // Check if the target floor is valid
            if (targetFloor < 1 || targetFloor > elevator.MaxFloor)
            {
                Console.WriteLine($"Invalid target floor: {targetFloor}. Current floor: {elevator.CurrentFloor} (Max: {elevator.MaxFloor})");
                // Return without changing any state
                return; // Ensure we exit early
            }

            // Check if the elevator is already at the target floor
            if (targetFloor == elevator.CurrentFloor)
            {
                Console.WriteLine("Already at the target floor.");
                return; // Exit the method if no movement is needed
            }

            // Determine the direction of movement
            string direction = targetFloor > elevator.CurrentFloor ? "Up" : "Down";
            elevator.IsMoving = true; // Set the moving state to true
            elevator.SetDirection(direction); // Set the direction
            Console.WriteLine($"Elevator is moving {direction}.");

            // Calculate the time it takes to move to the target floor
            int floorsToMove = Math.Abs(targetFloor - elevator.CurrentFloor);
            var movementTime = CalculateMovementTime(floorsToMove, elevator.TimePerFloor); // Ensure this method handles params correctly
            Console.WriteLine($"Simulating movement delay of {movementTime} milliseconds.");
            await Task.Delay(movementTime); // Simulate the movement delay

            // Move the elevator to the target floor
            elevator.CurrentFloor = targetFloor; // Only set this if the target floor was valid
            elevator.IsMoving = false; // Reset moving state
            elevator.SetDirection("Stationary"); // Set direction to stationary
            Console.WriteLine($"Elevator has arrived at floor {elevator.CurrentFloor}. Status: {elevator.Direction}, Is Moving: {elevator.IsMoving}");
        }


        // Helper method to calculate movement time
        private int CalculateMovementTime(int floorsToMove, int timePerFloor)
        {
            // Calculate total time based on the number of floors and time per floor
            return floorsToMove * timePerFloor;
        }


        private int CalculateMovementTime(int currentFloor, int targetFloor, int timePerFloor)
        {
            return Math.Abs(targetFloor - currentFloor) * timePerFloor;
        }
    }
}
