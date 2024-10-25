using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorController
    {
        Task RequestElevator(int floorNumber, int passengerCount); // Change void to Task
        void ShowElevatorStatus();
        bool HasAvailableElevators();
    }
}
