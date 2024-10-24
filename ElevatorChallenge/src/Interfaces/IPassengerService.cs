using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// File: ElevatorChallenge/ElevatorChallenge/src/Interfaces/IPassengerService.cs
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
