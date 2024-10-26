// File: ElevatorChallenge/ElevatorChallenge/src/Interfaces/IElevatorService.cs
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorService
    {
        Task<Elevator> AssignElevatorAsync(int requestFloor, int passengersWaiting);
        Task RequestElevatorAsync(int requestFloor, int passengersWaiting);
        List<Elevator> GetElevatorsStatus();
        void ShowElevatorStatus();
        bool HasAvailableElevators(int passengers); // Keep this only once
        string GetElevatorStatus();
    }
}
