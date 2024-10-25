using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface IElevatorController
    {
        void Start();
        Task RequestElevator(int floor, int passengers);
        void ShowElevatorStatus();
        bool HasAvailableElevators();
    }
}
