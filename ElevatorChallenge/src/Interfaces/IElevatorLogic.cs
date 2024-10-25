public interface IElevatorLogic
{
    Elevator FindNearestElevator(List<Elevator> elevators, int targetFloor);
    bool CanTakePassengers(IElevator elevator, int passengersWaiting); // Changed to use IElevator
    void UpdateElevatorStatus(Elevator elevator);
    IElevator FindNearestElevator(List<IElevator> elevators, int targetFloor);
}
