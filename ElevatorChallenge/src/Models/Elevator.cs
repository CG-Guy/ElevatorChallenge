using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public abstract class Elevator : IElevator
{
    public readonly ILogger<Elevator> logger;
    public readonly SemaphoreSlim semaphore = new(1, 1); // Semaphore for thread-safety

    public int Id { get; set; }
    public int CurrentFloor { get; set; }
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
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            SetCurrentFloor(floor);
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

    public void AddPassengers(int count)
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
}
