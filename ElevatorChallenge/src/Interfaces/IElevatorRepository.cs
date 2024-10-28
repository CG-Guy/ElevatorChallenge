using ElevatorChallenge.ElevatorChallenge.src.Models;
using System.Collections.Generic;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorRepository
    {
        IElevator FindBestElevator(int targetFloor, int passengerCount);
        IReadOnlyList<Elevator> GetAllElevators(); // Change to IReadOnlyList<Elevator>
        Elevator GetElevatorById(int id);
        bool TryAddElevator(Elevator elevator);
        // Other data access methods as needed
    }
}
