using ElevatorChallenge.ElevatorChallenge.src.Models;
using System.Collections.Generic;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorManagementService
    {
        void AddElevator(Elevator elevator);
        List<Elevator> GetElevatorsStatus();
        string GetElevatorStatus();
        bool HasAvailableElevators();
    }
}
