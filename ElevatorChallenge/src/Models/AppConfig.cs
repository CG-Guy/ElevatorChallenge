using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    // AppConfig class to hold the configurations for elevators and building
    public class AppConfig
    {
        public List<ElevatorConfig> Elevators { get; set; }
        public BuildingConfig Building { get; set; }

        public int NumberOfElevators
        {
            get => Elevators?.Count ?? 0; // Automatically calculate the number of elevators
        }
    }

    // Configuration for each elevator
    public class ElevatorConfig
    {
        public int Id { get; set; }
        public int MaxPassengerCapacity { get; set; }
        private int _currentFloor;

        public int CurrentFloor
        {
            get => _currentFloor;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(CurrentFloor), "Current floor must be greater than or equal to 1.");
                }
                _currentFloor = value;
            }
        }
    }

    // Configuration for the building
    public class BuildingConfig
    {
        public int TotalFloors { get; set; }
        public List<ElevatorConfig> Elevators { get; set; } // Ensure this property exists
    }

    // Interface for the elevator
    public interface IElevator
    {
        int Id { get; }
        int MaxPassengerCapacity { get; }
        int CurrentFloor { get; }
        int TotalFloors { get; }

        // Define elevator behavior methods
        void MoveToFloor(int floor);
        void OpenDoors();
        void CloseDoors();
    }

    // Concrete implementation of the elevator
    public class ConcreteElevator : IElevator
    {
        public int Id { get; }
        public int MaxPassengerCapacity { get; }
        public int CurrentFloor { get; private set; }
        public int TotalFloors { get; }

        private readonly ILogger<ConcreteElevator> _logger;
        private ILogger logger;

        // Constructor using ILogger<ConcreteElevator>
        public ConcreteElevator(int id, int maxPassengerCapacity, int totalFloors, ILogger<ConcreteElevator> logger)
        {
            Id = id;
            MaxPassengerCapacity = maxPassengerCapacity;
            TotalFloors = totalFloors;
            CurrentFloor = 1; // Start at the ground floor
            _logger = logger;
        }

        public ConcreteElevator(int id, int maxPassengerCapacity, int totalFloors, ILogger logger)
        {
            Id = id;
            MaxPassengerCapacity = maxPassengerCapacity;
            TotalFloors = totalFloors;
            this.logger = logger;
        }

        public ConcreteElevator(int id, int maxPassengerCapacity, int totalFloors, Castle.Core.Logging.ILogger logger1)
        {
            Id = id;
            MaxPassengerCapacity = maxPassengerCapacity;
            TotalFloors = totalFloors;
        }

        public void MoveToFloor(int floor)
        {
            if (floor < 1 || floor > TotalFloors)
            {
                throw new ArgumentOutOfRangeException(nameof(floor), "Floor must be within the range of building floors.");
            }

            // Simulate moving to the floor
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
