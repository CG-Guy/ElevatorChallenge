// File: ElevatorChallenge/ElevatorChallenge/src/Interfaces/IElevatorControlSystem.cs
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorControlSystem
    {
        Task RequestElevatorAsync(int targetFloor, int passengerCount); // Change this line
        void InitializeElevators(IElevatorFactory elevatorFactory, List<ElevatorConfig> elevatorConfigs);
    }
}
