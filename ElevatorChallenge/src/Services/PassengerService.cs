using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.Services;
using Microsoft.Extensions.Logging;
using System;

namespace ElevatorChallenge.ElevatorChallenge.src.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IElevator _elevator;
        private readonly ILogger<PassengerService> _logger;
        private readonly object _lock = new object(); // Lock for thread safety

        public PassengerService(IElevator elevator, ILogger<PassengerService> logger)
        {
            _elevator = elevator ?? throw new ArgumentNullException(nameof(elevator), "Elevator cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        // Adds a passenger to the elevator
        public void AddPassenger(Passenger passenger)
        {
            if (passenger == null)
            {
                throw new ArgumentNullException(nameof(passenger), "Passenger cannot be null.");
            }

            lock (_lock) // Ensure thread safety
            {
                if (!_elevator.HasSpaceFor(1))
                {
                    _logger.LogWarning($"Elevator {_elevator.Id} is full. Cannot add more passengers. Current count: {_elevator.PassengerCount}/{_elevator.MaxPassengerCapacity}");
                    return;
                }

                _elevator.AddPassengers(1);
                _logger.LogInformation($"Passenger added successfully to elevator {_elevator.Id}. Current count: {_elevator.PassengerCount}/{_elevator.MaxPassengerCapacity}");
            }
        }

        // Removes a passenger from the elevator
        public void RemovePassenger()
        {
            lock (_lock)
            {
                if (_elevator.PassengerCount > 0)
                {
                    _elevator.RemovePassengers(1);
                    _logger.LogInformation($"Passenger removed from elevator {_elevator.Id}. Current count: {_elevator.PassengerCount}/{_elevator.MaxPassengerCapacity}");
                }
                else
                {
                    _logger.LogWarning($"Elevator {_elevator.Id} has no passengers to remove.");
                }
            }
        }

        // Logs the current status of the elevator
        public void UpdateElevatorStatus()
        {
            _logger.LogInformation($"Elevator {_elevator.Id} status update: " +
                                   $"Floor: {_elevator.CurrentFloor}, " +
                                   $"Direction: {_elevator.Direction}, " +
                                   $"Moving: {_elevator.IsMoving}, " +
                                   $"Passengers: {_elevator.PassengerCount}/{_elevator.MaxPassengerCapacity}");
        }

        // Request an elevator to a specific floor with a specified number of passengers
        public void RequestElevator(ElevatorService elevatorService, int floor, int passengersWaiting)
        {
            if (elevatorService == null)
            {
                throw new ArgumentNullException(nameof(elevatorService), "Elevator service cannot be null.");
            }

            if (_elevator.IsInService)
            {
                if (!_elevator.HasSpaceFor(passengersWaiting))
                {
                    _logger.LogWarning($"Elevator {_elevator.Id} is full. Cannot accommodate {passengersWaiting} passengers on floor {floor}.");
                    return;
                }

                _logger.LogInformation($"Elevator {_elevator.Id} requested to floor {floor} for {passengersWaiting} passengers.");
                elevatorService.AssignElevator(floor, passengersWaiting); // Delegates to ElevatorService
                _logger.LogInformation($"Elevator {_elevator.Id} dispatched to floor {floor}.");
            }
            else
            {
                _logger.LogWarning($"Elevator {_elevator.Id} is not in service and cannot be dispatched.");
            }
        }
    }
}
