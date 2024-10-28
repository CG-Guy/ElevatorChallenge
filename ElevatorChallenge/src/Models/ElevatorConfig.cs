using System;

namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
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
}
