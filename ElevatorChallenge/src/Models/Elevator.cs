using ElevatorChallenge;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

// Abstract Elevator class
public abstract class Elevator : IElevator
{
    public readonly ILogger<Elevator> logger;
    public readonly SemaphoreSlim semaphore = new(1, 1); // Semaphore for thread-safety

    public int Id { get; set; }
    public int CurrentFloor { get; set; }
    public int CurrentPassengers { get; set; }
    public int PassengerCount { get; set; }
    public int MaxPassengerCapacity { get; set; }
    public bool IsMoving { get; set; }
    public int MaxFloor { get; set; }
    public int TimePerFloor { get; set; } = 1; // Time per floor in seconds
    public int TargetFloor { get; set; }
    public bool IsInService { get; set; } = true;


    public virtual string Direction => IsMoving ? (CurrentFloor < TargetFloor ? "Up" : "Down") : "Stationary";

    public Elevator(int id, int maxFloor, int maxPassengerCapacity, ILogger<Elevator> logger, int currentFloor = 1, int currentPassengers = 0)
    {
        Id = id;
        MaxFloor = maxFloor;
        MaxPassengerCapacity = maxPassengerCapacity;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Only assign in the constructor
        SetCurrentFloor(currentFloor);

        if (currentPassengers < 0)
        {
            throw new ArgumentException("Current passenger count cannot be negative.", nameof(currentPassengers));
        }

        if (currentPassengers > 0)
        {
            AddPassengers(currentPassengers);
        }

        IsMoving = false;
        IsInService = true;
    }

    // Abstract method to enforce derived classes to implement the MoveToFloor method
    public abstract Task MoveToFloor(int targetFloor);

    public virtual async Task MoveAsync(int floor)
    {
        ValidateFloor(floor); // Validate the target floor

        TargetFloor = floor;
        SetMovingStatus(true);

        await semaphore.WaitAsync(); // Ensure only one movement at a time
        try
        {
            logger.LogInformation($"Elevator {Id} moving from Floor {CurrentFloor} to Floor {TargetFloor}.");
            await Task.Delay(TimePerFloor * Math.Abs(CurrentFloor - TargetFloor) * 1000); // Simulate time taken to move

            SetCurrentFloor(floor); // Retain using SetCurrentFloor if it performs validation
            Stop();
        }
        finally
        {
            semaphore.Release();
        }
    }

    public virtual void Stop()
    {
        SetMovingStatus(false);
        logger.LogInformation($"Elevator {Id} has stopped at Floor {CurrentFloor}.");
    }

    // Mark this method as virtual to allow overriding
    public virtual void AddPassengers(int count)
    {
        if (count < 0)
        {
            throw new ArgumentException("Passenger count cannot be negative.", nameof(count));
        }

        int newTotalPassengerCount = PassengerCount + count;

        if (newTotalPassengerCount <= MaxPassengerCapacity)
        {
            PassengerCount = newTotalPassengerCount;
            logger.LogInformation($"{count} passengers added to Elevator {Id}. Current count: {PassengerCount}.");
        }
        else
        {
            logger.LogWarning($"Cannot add {count} passengers. Capacity exceeded for Elevator {Id}.");
            throw new InvalidOperationException(
                $"Cannot add {count} passengers to Elevator {Id}. Capacity exceeded. Current count: {PassengerCount}, Attempted to add: {count}, Max capacity: {MaxPassengerCapacity}."
            );
        }
    }

    public async Task AddPassengersAsync(int count, Elevator[] elevators)
    {
        if (count < 0)
        {
            throw new ArgumentException("Passenger count cannot be negative.", nameof(count));
        }

        // Check for available elevators
        if (!AreElevatorsAvailable(elevators, count))
        {
            logger.LogWarning($"No elevators available for the requested number of passengers: {count}.");
            throw new InvalidOperationException($"No elevators available for the requested number of passengers: {count}.");
        }

        int newTotalPassengerCount = CurrentPassengers + count;

        // Check if the new total passenger count exceeds capacity
        if (newTotalPassengerCount > Capacity)
        {
            logger.LogWarning($"Cannot add {count} passengers. Capacity exceeded for Elevator {Id}.");
            throw new InvalidOperationException(
                $"Cannot add {count} passengers to Elevator {Id}. Capacity exceeded. Current count: {CurrentPassengers}, Attempted to add: {count}, Max capacity: {Capacity}."
            );
        }

        // Simulate asynchronous work
        await Task.Delay(500); // Simulate time taken to add passengers

        // If validation passed, call the AddPassengers method
        AddPassengers(count); // Delegate to the synchronous AddPassengers method

        logger.LogInformation($"{count} passengers added to Elevator {Id}. Current count: {CurrentPassengers}.");
    }

    // Helper method to check for available elevators
    private bool AreElevatorsAvailable(Elevator[] elevators, int passengerCount)
    {
        return elevators.Any(elevator => elevator.IsAvailable() && (elevator.CurrentPassengers + passengerCount <= elevator.Capacity));
    }

    public bool HasSpaceFor(int count)
    {
        return (PassengerCount + count) <= MaxPassengerCapacity;
    }

    public void RemovePassengers(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Passenger count must be greater than zero.", nameof(count));
        }

        if (count > PassengerCount)
        {
            throw new InvalidOperationException("Cannot remove more passengers than currently on the elevator.");
        }

        PassengerCount -= count;
        logger.LogInformation($"{count} passengers removed from Elevator {Id}. Current count: {PassengerCount}");
    }

    public virtual void SetCurrentFloor(int floor)
    {
        ValidateFloor(floor); // Validate the floor before setting
        CurrentFloor = floor;
    }

    private void ValidateFloor(int floor)
    {
        if (floor < 1 || floor > MaxFloor)
        {
            throw new ArgumentOutOfRangeException(nameof(floor), $"Floor must be within the valid range of 1 to {MaxFloor}.");
        }
    }

    public void SetMovingStatus(bool isMoving)
    {
        IsMoving = isMoving;
        logger.LogInformation($"Elevator {Id} moving status changed to: {isMoving}");
    }

    public void CloseDoors()
    {
        logger.LogInformation($"Elevator {Id} doors closed.");
    }

    public bool IsAvailable() => IsInService && HasSpaceFor(1);

    public override string ToString()
    {
        return $"Elevator ID: {Id}, Current Floor: {CurrentFloor}, Moving: {IsMoving}, Direction: {Direction}, " +
               $"Passengers: {PassengerCount}/{MaxPassengerCapacity}, In Service: {IsInService}";
    }

    // Implement SetDirection to comply with IElevator
    public void SetDirection(string direction)
    {
        if (direction != "Up" && direction != "Down" && direction != "Stationary")
        {
            throw new ArgumentException("Invalid direction. Use 'Up', 'Down', or 'Stationary'.", nameof(direction));
        }

        logger.LogInformation($"Elevator {Id} direction set to: {direction}");
    }

    // Implement Capacity property from the IElevator interface
    public int Capacity => MaxPassengerCapacity; // Implementing Capacity property

    // Implement GoToFloor method from the IElevator interface
    public void GoToFloor(int floor)
    {
        ValidateFloor(floor); // Validate the target floor
        logger.LogInformation($"Elevator {Id} is going to floor {floor}.");
        SetCurrentFloor(floor);
    }
}

// ConcreteElevator class
public class ConcreteElevator : Elevator, IElevator
{
    private int totalFloors;
    private ILogger<ConcreteElevator> loggerForElevator;

    public ConcreteElevator(int id, int maxFloor, int maxPassengerCapacity, ILogger<Elevator> logger, int currentFloor, int currentPassengers)
     : base(id, maxFloor, maxPassengerCapacity, logger)
    {
    }

    public int Capacity => MaxPassengerCapacity; // Implementing Capacity property

    public void GoToFloor(int floor)
    {
        MoveToFloor(floor).Wait(); // Ensure this is appropriately managed
    }

    public override async Task MoveToFloor(int targetFloor)
    {
        if (targetFloor < 1 || targetFloor > MaxFloor)
        {
            logger.LogWarning($"Target floor {targetFloor} is out of range for Elevator {Id}.");
            return;
        }

        if (PassengerCount > MaxPassengerCapacity)
        {
            logger.LogWarning($"Elevator {Id} is over capacity with {PassengerCount} passengers.");
            return;
        }

        logger.LogInformation($"Elevator {Id} is moving from floor {CurrentFloor} to floor {targetFloor}.");

        // Simulate the time taken to move to the target floor
        await Task.Delay(2000); // Simulates the time to move between floors
        SetCurrentFloor(targetFloor); // Use SetCurrentFloor to update the current floor

        logger.LogInformation($"Elevator {Id} has arrived at floor {CurrentFloor}.");
        Console.WriteLine($"Elevator {Id} moving to floor {CurrentFloor}.");
    }

    // Additional methods and properties...
}
