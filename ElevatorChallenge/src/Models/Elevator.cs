using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public abstract class Elevator : IElevator
{
    private readonly ILogger<Elevator> logger; // Logger field

    public int Id { get; set; }
    public int CurrentFloor { get; set; }
    public int PassengerCount { get; set; }
    public int MaxPassengerCapacity { get; set; }
    public bool IsMoving { get; set; }
    public int MaxFloor { get; set; }
    public int TimePerFloor { get; set; } = 1; // Default time per floor (in seconds)
    public int TargetFloor { get; private set; } // Property to track the target floor
    public bool IsInService { get; set; } = true; // Default value

    // Backing field for Direction
    private string _direction;

    // Direction property with a public getter and private setter
    public virtual string Direction
    {
        get => IsMoving ? _direction : "Stationary";
        private set => _direction = value;
    }

    // Constructor to initialize the elevator and logger
    public Elevator(int id, int maxFloor, int maxPassengerCapacity, ILogger<Elevator> logger, int currentFloor = 1, int currentPassengers = 0)
    {
        Id = id;
        MaxFloor = maxFloor;
        MaxPassengerCapacity = maxPassengerCapacity;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Ensure logger is not null
        SetCurrentFloor(currentFloor);

        // Only add passengers if the count is greater than zero
        if (currentPassengers > 0)
        {
            AddPassengers(currentPassengers);
        }

        IsMoving = false;
        IsInService = true;
    }

    // Ensure this is the only MoveAsync method in this class
    public virtual async Task MoveAsync(int floor)
    {
        if (floor < 1 || floor > MaxFloor)
        {
            throw new ArgumentOutOfRangeException(nameof(floor), "Floor must be within the valid range.");
        }

        TargetFloor = floor;

        // Determine direction based on current and target floor
        SetDirection(CurrentFloor > floor ? "Down" : "Up");
        SetMovingStatus(true);

        // Simulate movement (you may replace this with actual delay logic)
        await Task.Delay(TimePerFloor * Math.Abs(CurrentFloor - TargetFloor) * 1000); // Simulated delay in milliseconds

        CurrentFloor = floor; // Only set after "moving"
        Stop(); // Stop the elevator after reaching the target floor
    }

    public virtual void Stop()
    {
        SetMovingStatus(false); // Logic to stop the elevator
        logger.LogInformation($"Elevator {Id} has stopped.");
    }

    public void SetDirection(string direction)
    {
        if (direction != "Up" && direction != "Down" && direction != "Stationary")
            throw new ArgumentException("Invalid direction. Use 'Up', 'Down', or 'Stationary'.");

        Direction = direction; // Set the direction using the private setter
    }

    public void AddPassengers(int count)
    {
        // Ensure valid passenger count (allow zero but disallow negative values)
        if (count < 0)
            throw new ArgumentException("Passenger count cannot be negative.");

        // Check if there is enough space for the passengers
        if (HasSpaceFor(count))
        {
            PassengerCount += count; // Add passengers
            logger.LogInformation($"{count} passengers added to Elevator {Id}. Current count: {PassengerCount}");
        }
        else
        {
            logger.LogWarning($"Cannot add {count} passengers. Capacity exceeded for Elevator {Id}.");
            throw new InvalidOperationException($"Cannot add {count} passengers to Elevator {Id}. Capacity exceeded.");
        }
    }

    public void RemovePassengers(int count)
    {
        if (count <= 0) throw new ArgumentException("Passenger count must be greater than zero.");
        if (count > PassengerCount) throw new InvalidOperationException("Cannot remove more passengers than currently on the elevator.");
        PassengerCount -= count;
        logger.LogInformation($"{count} passengers removed from Elevator {Id}. Current count: {PassengerCount}");
    }

    public bool HasSpaceFor(int numberOfPassengers) => (PassengerCount + numberOfPassengers) <= MaxPassengerCapacity;

    // Marked as virtual to allow overriding in derived classes
    protected virtual void SetCurrentFloor(int floor)
    {
        // Validate the floor value to ensure it is within the allowable range
        if (floor < 1 || floor > MaxFloor)
        {
            throw new ArgumentOutOfRangeException(nameof(floor), $"Floor must be within the valid range of 1 to {MaxFloor}.");
        }

        // Assign the validated floor to the CurrentFloor property
        CurrentFloor = floor;
    }

    public void SetMovingStatus(bool isMoving)
    {
        IsMoving = isMoving;
        logger.LogInformation($"Elevator {Id} moving status changed to: {isMoving}");
    }

    // Implementing CloseDoors method
    public void CloseDoors()
    {
        logger.LogInformation($"Elevator {Id} doors closed.");
    }

    // New method to check if the elevator is available
    public bool IsAvailable() => IsInService && HasSpaceFor(1); // Check if it's in service and has space for at least one passenger

    public override string ToString()
    {
        return $"Elevator ID: {Id}, Current Floor: {CurrentFloor}, Moving: {IsMoving}, Direction: {Direction}, " +
               $"Passengers: {PassengerCount}/{MaxPassengerCapacity}, In Service: {IsInService}";
    }
}
