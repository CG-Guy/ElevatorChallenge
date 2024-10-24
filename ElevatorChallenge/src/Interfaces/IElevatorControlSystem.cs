// File: ElevatorChallenge/ElevatorChallenge/src/Interfaces/IElevatorControlSystem.cs
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorControlSystem
    {
        Task RequestElevatorAsync(int targetFloor, int passengerCount); // Change this line
        void InitializeElevators();
    }
}
