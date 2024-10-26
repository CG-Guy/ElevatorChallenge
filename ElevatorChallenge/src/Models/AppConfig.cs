using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    public class AppConfig
    {
        // Initialize the Elevators list to ensure it's not null
        public List<ElevatorConfig> Elevators { get; set; } = new List<ElevatorConfig>();
        public BuildingConfig Building { get; set; }

        // Property to get the number of elevators
        public int NumberOfElevators => Elevators?.Count ?? 0;
    }

    public class ElevatorConfig
    {
        public int Id { get; set; }
        public int MaxPassengerCapacity { get; set; }
        private int _currentFloor;
        private int _currentPassengers;

        // Property to manage the current floor of the elevator
        public int CurrentFloor
        {
            get => _currentFloor;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(CurrentFloor), "Current floor must be greater than or equal to 1.");
                _currentFloor = value;
            }
        }

        // Property to manage the current number of passengers
        public int CurrentPassengers
        {
            get => _currentPassengers;
            set
            {
                if (value < 0 || value > MaxPassengerCapacity)
                    throw new ArgumentOutOfRangeException(nameof(CurrentPassengers), "Current passengers must be between 0 and MaxPassengerCapacity.");
                _currentPassengers = value;
            }
        }
    }

    public class BuildingConfig
    {
        public int TotalFloors { get; set; }
        public List<ElevatorConfig> Elevators { get; set; }
    }

    public interface IElevator
    {
        int Id { get; }
        int MaxPassengerCapacity { get; }
        int CurrentFloor { get; }
        int TotalFloors { get; }

        void MoveToFloor(int floor);
        void OpenDoors();
        void CloseDoors();
    }

    public class ConcreteElevator : IElevator
    {
        public int Id { get; }
        public int MaxPassengerCapacity { get; }
        public int CurrentFloor { get; private set; }
        public int TotalFloors { get; }

        private readonly ILogger<ConcreteElevator> _logger;

        public ConcreteElevator(int id, int maxPassengerCapacity, int totalFloors, ILogger<ConcreteElevator> logger)
        {
            Id = id;
            MaxPassengerCapacity = maxPassengerCapacity;
            TotalFloors = totalFloors;
            CurrentFloor = 1; // Default to the first floor
            _logger = logger;
        }

        public void MoveToFloor(int floor)
        {
            if (floor < 1 || floor > TotalFloors)
                throw new ArgumentOutOfRangeException(nameof(floor), "Floor must be within the range of building floors.");

            CurrentFloor = floor;
            _logger?.LogInformation($"Elevator {Id} moved to floor {CurrentFloor}.");
        }

        public void OpenDoors()
        {
            _logger?.LogInformation($"Elevator {Id} doors opened at floor {CurrentFloor}.");
        }

        public void CloseDoors()
        {
            _logger?.LogInformation($"Elevator {Id} doors closed.");
        }
    }
}
