using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using System.Collections.Generic;
using Xunit;

namespace ElevatorChallenge.ElevatorChallenge.tests.Logic
{
    public class ElevatorLogicTests
    {
        [Fact]
        public void FindNearestElevator_Should_Return_Nearest_Elevator()
        {
            var elevators = new List<Elevator>
            {
                new PassengerElevator(0, 1, 5), // ID 0, Current floor 1, Max capacity 5
                new PassengerElevator(1, 5, 5)  // ID 1, Current floor 5, Max capacity 5
            };

            var elevatorLogic = new ElevatorLogic();
            var nearestElevator = elevatorLogic.FindNearestElevator(elevators, 3); // Request to find the nearest to floor 3

            Assert.NotNull(nearestElevator);
            Assert.Equal(1, nearestElevator.CurrentFloor); // Expected: the nearest elevator is at floor 1
        }

        [Fact]
        public void CanTakePassengers_Should_Return_True_When_Elevator_Has_Space()
        {
            var elevator = new PassengerElevator(0, 0, 5); // Use PassengerElevator, ID 0, Current floor 0, Max capacity 5
            elevator.AddPassengers(2); // Add 2 passengers

            var elevatorLogic = new ElevatorLogic();

            var canTake = elevatorLogic.CanTakePassengers(elevator, 2); // Check if the elevator can take 2 more passengers

            Assert.True(canTake); // Should return true since capacity is sufficient
        }

        [Fact]
        public void CanTakePassengers_Should_Return_False_When_Elevator_Is_Full()
        {
            // Arrange: Create a passenger elevator with a maximum capacity of 5 and current passengers as 5
            var elevator = new PassengerElevator(0, 0, 5); // ID 0, Current floor 0, Max capacity 5
            elevator.AddPassengers(5); // Add maximum passengers to reach capacity (5)

            var elevatorLogic = new ElevatorLogic();

            // Act: Check if it can take 1 more passenger
            var canTake = elevatorLogic.CanTakePassengers(elevator, 1); // Check if the elevator can take 1 more passenger

            // Assert: Expecting false since the elevator is full
            Assert.False(canTake);
        }
    }
}
