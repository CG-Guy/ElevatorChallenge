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
        // Use a thread-safe collection to prevent race conditions
        private readonly ConcurrentBag<Elevator> _elevators;

        public ElevatorRepository(IEnumerable<Elevator> elevators) // Accept IEnumerable for better flexibility
        {
            if (elevators == null)
            {
                throw new ArgumentNullException(nameof(elevators), "Elevators collection cannot be null.");
            }

            _elevators = new ConcurrentBag<Elevator>(elevators); // Initialize thread-safe collection
        }

        public Elevator FindBestElevator(int targetFloor, int passengerCount)
        {
            // Validate input parameters
            ValidateInput(targetFloor, passengerCount);

            // Use LINQ to find available elevators
            var availableElevators = _elevators
                .Where(e => e.PassengerCount + passengerCount <= e.MaxPassengerCapacity && !e.IsMoving)
                .ToList();

            if (!availableElevators.Any())
            {
                LogWarning("No available elevators found.");
                return null; // No available elevators
            }

            // Select the nearest elevator to the target floor
            Elevator bestElevator = availableElevators
                .OrderBy(e => Math.Abs(e.CurrentFloor - targetFloor))
                .FirstOrDefault();

            return bestElevator;
        }

        public bool TryAddElevator(Elevator elevator)
        {
            // Validate the elevator before adding
            if (elevator == null)
            {
                throw new ArgumentNullException(nameof(elevator), "Elevator cannot be null.");
            }

            // Optionally, check for duplicate elevators based on ID
            if (_elevators.Any(e => e.Id == elevator.Id))
            {
                throw new InvalidOperationException($"Elevator with ID {elevator.Id} already exists.");
            }

            // Add the elevator in a thread-safe manner
            _elevators.Add(elevator);
            return true; // Indicate success
        }

        public IReadOnlyList<Elevator> GetAllElevators() // Return a read-only list to limit external modification
        {
            return _elevators.ToList(); // Return a snapshot of the current state
        }

        // Additional method for the repository interface
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
            // Implement a logging mechanism (could be a logger interface or a logging framework)
            Console.WriteLine($"WARNING: {message}"); // Replace with proper logging
        }
    }
}
