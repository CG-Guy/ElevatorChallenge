// File: ElevatorChallenge/ElevatorChallenge/src/Interfaces/IElevatorLogic.cs
using ElevatorChallenge.ElevatorChallenge.src.Models;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorLogic
    {
        Elevator FindNearestElevator(List<Elevator> elevators, int targetFloor);
        bool CanTakePassengers(PassengerElevator elevator, int passengersWaiting); // Change Elevator to PassengerElevator
        void UpdateElevatorStatus(Elevator elevator);
    }
}
