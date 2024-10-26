public interface IElevator
{
    int CurrentFloor { get; set; }
    string Direction { get; }
    bool IsMoving { get; }
    int PassengerCount { get; }
    int MaxPassengerCapacity { get; }
    bool IsInService { get; }
    void AddPassengers(int count);
    void RemovePassengers(int count);
    bool HasSpaceFor(int count);
    void SetDirection(string direction);
    Task MoveAsync(int targetFloor);
    int Id { get; }
    int Capacity { get; }
    void GoToFloor(int floor);
}
