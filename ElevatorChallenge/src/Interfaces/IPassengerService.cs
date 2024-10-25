using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.Services;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IPassengerService
    {
        void AddPassenger(Passenger passenger);
        void RemovePassenger();
        void UpdateElevatorStatus();
        void RequestElevator(ElevatorService elevatorService, int floor, int passengersWaiting);
    }
}
