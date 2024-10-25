public interface IElevator
{
    int CurrentFloor { get; }
    string Direction { get; }
    bool IsMoving { get; }
    int PassengerCount { get; }
    int MaxPassengerCapacity { get; }
    bool IsInService { get; }
    void AddPassengers(int count);
    void RemovePassengers(int count);
    bool HasSpaceFor(int count); // Ensure this is public
    void SetDirection(string direction);
    Task MoveAsync(int targetFloor); // Keep this asynchronous
    int Id { get; }
    // Removed MaxFloor if not needed
}
