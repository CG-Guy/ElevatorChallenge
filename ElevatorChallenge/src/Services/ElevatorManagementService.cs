using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorChallenge.Services
{
    public class ElevatorManagementService : IElevatorManagementService // Implement the interface
    {
        private readonly List<Elevator> _elevators;
        private readonly IApplicationLogger _logger; // Logger for elevator management

        public ElevatorManagementService(IEnumerable<Elevator> elevators, IApplicationLogger logger)
        {
            _elevators = elevators?.ToList() ?? throw new ArgumentNullException(nameof(elevators));
            _logger = logger;
        }

        public void AddElevator(Elevator elevator)
        {
            if (elevator == null)
            {
                throw new ArgumentNullException(nameof(elevator), "Elevator cannot be null.");
            }

            _elevators.Add(elevator);
            _logger.LogInformation($"Elevator {elevator.Id} added.");
        }

        public List<Elevator> GetElevatorsStatus() => _elevators;

        public string GetElevatorStatus()
        {
            if (_elevators.Count == 0)
            {
                return "No elevators available.";
            }

            var statusList = new List<string>();
            foreach (var elevator in _elevators)
            {
                statusList.Add($"Elevator {elevator.Id}: " +
                               $"Floor {elevator.CurrentFloor}, " +
                               $"Moving: {elevator.IsMoving}, " +
                               $"Direction: {elevator.Direction}, " +
                               $"Passengers: {elevator.PassengerCount}/{elevator.MaxPassengerCapacity}, " +
                               $"In Service: {elevator.IsInService}");
            }

            return string.Join(Environment.NewLine, statusList);
        }

        public bool HasAvailableElevators()
        {
            return _elevators.Any(elevator => elevator.IsInService && elevator.PassengerCount < elevator.MaxPassengerCapacity);
        }

        // Implementation of ManageElevators method
        public void ManageElevators()
        {
            foreach (var elevator in _elevators)
            {
                if (!elevator.IsInService)
                {
                    _logger.LogWarning($"Elevator {elevator.Id} is out of service.");
                    continue;
                }

                // Example logic to manage elevators
                // This could involve checking their status, moving them to waiting floors, etc.
                if (elevator.IsMoving)
                {
                    _logger.LogInformation($"Elevator {elevator.Id} is currently moving.");
                }
                else
                {
                    _logger.LogInformation($"Elevator {elevator.Id} is idle at floor {elevator.CurrentFloor}.");
                }
            }
        }
    }
}
