using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Logic
{
    public class ElevatorMovementLogic : IElevatorMovementLogic
    {
        private readonly IElevatorValidator _elevatorValidator; // Dependency injection for validation logic

        public ElevatorMovementLogic(IElevatorValidator elevatorValidator)
        {
            _elevatorValidator = elevatorValidator;
        }

        public async Task MoveElevatorToFloor(Elevator elevator, int targetFloor)
        {
            if (elevator == null)
            {
                Console.WriteLine("Elevator object is null.");
                return;
            }

            var validationResult = _elevatorValidator.ValidateElevatorMovement(elevator, targetFloor);
            if (!validationResult.IsValid)
            {
                Console.WriteLine(validationResult.ErrorMessage);
                return;
            }

            string direction = targetFloor > elevator.CurrentFloor ? "Up" : "Down";
            elevator.IsMoving = true;
            elevator.SetDirection(direction);
            Console.WriteLine($"Elevator is moving {direction}.");

            int floorsToMove = Math.Abs(targetFloor - elevator.CurrentFloor);
            var movementTime = CalculateMovementTime(floorsToMove, elevator.TimePerFloor);
            Console.WriteLine($"Simulating movement delay of {movementTime} milliseconds.");
            await Task.Delay(movementTime);

            elevator.SetCurrentFloor(targetFloor);
            elevator.IsMoving = false;
            elevator.SetDirection("Stationary");
            Console.WriteLine($"Elevator has arrived at floor {elevator.CurrentFloor}. Status: {elevator.Direction}, Is Moving: {elevator.IsMoving}");
        }

        private int CalculateMovementTime(int floorsToMove, int timePerFloor)
        {
            return floorsToMove * timePerFloor;
        }
    }
}
