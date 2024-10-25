using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;

namespace ElevatorChallenge.ElevatorChallenge.src.Logic
{
    public class ElevatorValidator : IElevatorValidator
    {
        public ElevatorValidationResult ValidateElevatorMovement(Elevator elevator, int targetFloor)
        {
            if (targetFloor < 1 || targetFloor > elevator.MaxFloor)
            {
                return new ElevatorValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Invalid target floor: {targetFloor}. Current floor: {elevator.CurrentFloor} (Max: {elevator.MaxFloor})"
                };
            }

            if (targetFloor == elevator.CurrentFloor)
            {
                return new ElevatorValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Already at the target floor."
                };
            }

            if (elevator.IsMoving)
            {
                return new ElevatorValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Elevator {elevator.Id} is currently moving and cannot be redirected."
                };
            }

            return new ElevatorValidationResult { IsValid = true }; // No validation errors
        }
    }
}
