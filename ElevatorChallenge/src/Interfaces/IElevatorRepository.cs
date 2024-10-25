using ElevatorChallenge.ElevatorChallenge.src.Models;
using System.Collections.Generic;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorRepository
    {
        IReadOnlyList<Elevator> GetAllElevators(); // Change to IReadOnlyList<Elevator>
        Elevator GetElevatorById(int id);
        // Other data access methods as needed
    }
}
