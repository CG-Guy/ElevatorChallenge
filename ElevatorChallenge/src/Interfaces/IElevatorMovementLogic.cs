using ElevatorChallenge.ElevatorChallenge.src.Models;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorMovementLogic
    {
        Task MoveElevatorToFloor(Elevator elevator, int targetFloor); // Change void to Task
    }
}
