using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorController
    {
        void RequestElevator(int floorNumber, int passengerCount);
        void ShowElevatorStatus();
        bool HasAvailableElevators();
    }
}
