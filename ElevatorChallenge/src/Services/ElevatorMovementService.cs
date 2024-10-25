using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System.Threading.Tasks;

namespace ElevatorChallenge.Services
{
    public class ElevatorMovementService
    {
        private readonly ElevatorMovementLogic _elevatorMovementLogic;

        public ElevatorMovementService(ElevatorMovementLogic elevatorMovementLogic)
        {
            _elevatorMovementLogic = elevatorMovementLogic;
        }

        public async Task MoveElevator(Elevator elevator, int requestFloor)
        {
            elevator.SetMovingStatus(true);
            elevator.SetDirection(requestFloor > elevator.CurrentFloor ? "Up" : "Down");
            await _elevatorMovementLogic.MoveElevatorToFloor(elevator, requestFloor);
            elevator.SetMovingStatus(false);
        }
    }
}
