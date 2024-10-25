using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;

namespace ElevatorChallenge.ElevatorChallenge.src.Repositories
{
    public class ElevatorRepository : IElevatorRepository // Implementing the repository interface
    {
        private readonly ConcurrentBag<Elevator> _elevators; // Thread-safe in-memory storage

        public ElevatorRepository(IEnumerable<Elevator> elevators) // Accept IEnumerable for flexibility
        {
            if (elevators == null)
            {
                throw new ArgumentNullException(nameof(elevators), "Elevators collection cannot be null.");
            }

            _elevators = new ConcurrentBag<Elevator>(elevators); // Initialize thread-safe collection
        }

        public Elevator FindBestElevator(int targetFloor, int passengerCount)
        {
            ValidateInput(targetFloor, passengerCount);

            // Use LINQ to find available elevators
            var availableElevators = _elevators
                .Where(e => e.PassengerCount + passengerCount <= e.MaxPassengerCapacity && !e.IsMoving)
                .ToList();

            if (!availableElevators.Any())
            {
                LogWarning("No available elevators found.");
                return null;
            }

            // Select the nearest elevator to the target floor
            return availableElevators
                .OrderBy(e => Math.Abs(e.CurrentFloor - targetFloor))
                .FirstOrDefault();
        }

        public bool TryAddElevator(Elevator elevator)
        {
            if (elevator == null)
            {
                throw new ArgumentNullException(nameof(elevator), "Elevator cannot be null.");
            }

            if (_elevators.Any(e => e.Id == elevator.Id))
            {
                throw new InvalidOperationException($"Elevator with ID {elevator.Id} already exists.");
            }

            _elevators.Add(elevator);
            return true;
        }

        public IReadOnlyList<Elevator> GetAllElevators()
        {
            return _elevators.ToList().AsReadOnly();
        }

        public Elevator GetElevatorById(int id)
        {
            return _elevators.FirstOrDefault(e => e.Id == id);
        }

        private void ValidateInput(int targetFloor, int passengerCount)
        {
            if (targetFloor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetFloor), "Target floor cannot be negative.");
            }
            if (passengerCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(passengerCount), "Passenger count cannot be negative.");
            }
        }

        private void LogWarning(string message)
        {
            Console.WriteLine($"WARNING: {message}");
        }
    }
}
