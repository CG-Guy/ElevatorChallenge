// File: ElevatorChallenge/ElevatorChallenge/src/Interfaces/IElevatorService.cs
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System.Collections.Generic;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorService
    {
        Elevator AssignElevator(int requestFloor, int passengersWaiting);
        void RequestElevator(int requestFloor, int passengersWaiting);
        List<Elevator> GetElevatorsStatus();
        void ShowElevatorStatus();
        bool HasAvailableElevators();
    }
}
