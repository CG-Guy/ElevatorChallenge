using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IPassenger
    {
        int Id { get; }
        int CurrentFloor { get; }
        int DestinationFloor { get; }
        bool IsBoarded { get; }

        void Board();
        void Exit();
        void UpdateDestinationFloor(int newFloor);
    }
}
