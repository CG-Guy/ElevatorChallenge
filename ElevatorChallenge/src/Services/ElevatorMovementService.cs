using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System.Threading.Tasks;

namespace ElevatorChallenge.Services
{
    public class ElevatorMovementService
    {
        private readonly ElevatorMovementLogic _elevatorMovementLogic;
        private readonly IElevatorManagementService _elevatorManagementService; // Added field

        // Modified constructor to include IElevatorManagementService
        public ElevatorMovementService(ElevatorMovementLogic elevatorMovementLogic, IElevatorManagementService elevatorManagementService)
        {
            _elevatorMovementLogic = elevatorMovementLogic;
            _elevatorManagementService = elevatorManagementService; // Initialize the field
        }

        public async Task MoveElevator(Elevator elevator, int requestFloor)
        {
            elevator.SetMovingStatus(true);
            elevator.SetDirection(requestFloor > elevator.CurrentFloor ? "Up" : "Down");

            await _elevatorMovementLogic.MoveElevatorToFloor(elevator, requestFloor);
            elevator.SetMovingStatus(false);

            // Example usage of ElevatorManagementService
            // If there's a need to log the movement or update the elevator status, 
            // you can call methods from _elevatorManagementService here, e.g.:
            _elevatorManagementService.ManageElevators(); // Example call
        }
    }
}
