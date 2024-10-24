using ElevatorChallenge.ElevatorChallenge.src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorAssignmentLogic
    {
        Elevator AssignElevator(int requestFloor, int passengersWaiting, IEnumerable<Elevator> elevators);
    }
}
