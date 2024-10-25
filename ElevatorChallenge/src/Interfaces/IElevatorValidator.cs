using ElevatorChallenge.ElevatorChallenge.src.Models;
using System.ComponentModel.DataAnnotations;

using ElevatorChallenge.ElevatorChallenge.src.Models;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorValidator
    {
        ElevatorValidationResult ValidateElevatorMovement(Elevator elevator, int targetFloor);
    }
}