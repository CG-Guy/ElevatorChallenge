﻿using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ElevatorChallenge.src.Models
{
    public class ConcreteElevator : Elevator, IElevator
    {
        private readonly ILogger<Elevator> _logger; // Use a private field for the logger

        // Constructor
        public ConcreteElevator(int id, int maxFloor, int maxPassengerCapacity, ILogger<Elevator> logger)
            : base(id, maxFloor, maxPassengerCapacity, logger)
        {
            _logger = logger; // Initialize the private field
        }

        public int Capacity => MaxPassengerCapacity; // Implementing IElevator property

        public async Task AddPassengersAsync(int numberOfPassengers)
        {
            if (CurrentPassengers + numberOfPassengers <= MaxPassengerCapacity)
            {
                CurrentPassengers += numberOfPassengers;
                await Task.Delay(1000); // Simulate the time taken to move to the requested floor
            }
            else
            {
                throw new InvalidOperationException("Not enough capacity to add passengers.");
            }
        }

        public bool IsAvailable()
        {
            return CurrentPassengers < MaxPassengerCapacity; // Or whatever condition defines availability
        }

        public override async Task MoveToFloor(int targetFloor)
        {
            if (targetFloor < 1 || targetFloor > MaxFloor)
            {
                _logger.LogWarning($"Target floor {targetFloor} is out of range for Elevator {Id}.");
                return;
            }

            if (PassengerCount > MaxPassengerCapacity)
            {
                _logger.LogWarning($"Elevator {Id} is over capacity with {PassengerCount} passengers.");
                return;
            }

            _logger.LogInformation($"Elevator {Id} is moving from floor {CurrentFloor} to floor {targetFloor}.");

            // Simulate the time taken to move to the target floor
            await Task.Delay(2000); // Simulates the time to move between floors
            SetCurrentFloor(targetFloor); // Use SetCurrentFloor to update the current floor

            _logger.LogInformation($"Elevator {Id} has arrived at floor {CurrentFloor}.");
            Console.WriteLine($"Elevator {Id} moving to floor {CurrentFloor}.");
        }

        public override void AddPassengers(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("Passenger count cannot be negative.", nameof(count));
            }

            int newTotalPassengerCount = PassengerCount + count;

            if (newTotalPassengerCount <= MaxPassengerCapacity)
            {
                PassengerCount = newTotalPassengerCount;
                _logger.LogInformation($"{count} passengers added to Elevator {Id}. Current count: {PassengerCount}.");
            }
            else
            {
                _logger.LogWarning($"Cannot add {count} passengers. Capacity exceeded for Elevator {Id}.");
                throw new InvalidOperationException(
                    $"Cannot add {count} passengers to Elevator {Id}. Capacity exceeded. Current count: {PassengerCount}, Attempted to add: {count}, Max capacity: {MaxPassengerCapacity}."
                );
            }
        }

        public void GetStatus()
        {
            _logger.LogInformation($"Elevator {Id} Status - Current floor: {CurrentFloor}, Passengers on board: {PassengerCount}");
            Console.WriteLine($"Elevator {Id} Status - Current floor: {CurrentFloor}, Passengers on board: {PassengerCount}");
        }

        public void GoToFloor(int floor)
        {
            MoveToFloor(floor).GetAwaiter().GetResult(); // Wait for the task to complete safely
        }

        public async Task GoToFloorAsync(int floor)
        {
            await MoveToFloor(floor); // Await the task to avoid blocking
        }
    }
}