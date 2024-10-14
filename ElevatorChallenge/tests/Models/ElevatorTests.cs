using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using Xunit;

namespace ElevatorChallenge.Tests
{
    public class ElevatorTests
    {
        [Fact]
        public void Elevator_Initialization_Should_Set_Default_Values()
        {
            // Arrange: Create a new passenger elevator with max floor 10, current floor 1, max capacity 5, and no passengers initially.
            var elevator = new PassengerElevator(1, 1, 5); // Set current floor to 1

            // Act & Assert: Check that MaxFloor, CurrentFloor, and MaxPassengerCapacity are set correctly.
            Assert.Equal(10, elevator.MaxFloor); // This should remain the same
            Assert.Equal(1, elevator.CurrentFloor); // Expecting current floor to be set to 1
            Assert.Equal(5, elevator.MaxPassengerCapacity);
            Assert.Equal(0, elevator.PassengerCount); // This should now pass
        }

        // Test to check adding passengers without exceeding capacity.
        [Fact]
        public void CanTakePassengers_Should_Return_True_When_Elevator_Has_Space()
        {
            // Arrange: Create elevator with max capacity 5 and current passengers 2
            var elevator = new PassengerElevator(0, 0, 5); // ID 0, Max floor 5, Current floor 0
            elevator.AddPassengers(2); // Add 2 passengers

            var elevatorLogic = new ElevatorLogic();

            // Act: Check if the elevator can take 3 more passengers (Total should be 5)
            var canTake = elevatorLogic.CanTakePassengers(elevator, 3);

            // Assert: Validate that it can take more passengers
            Assert.True(canTake); // Should return true since capacity is sufficient
            Assert.Equal(2, elevator.PassengerCount); // Verify PassengerCount is still 2 after the check
        }

        // Test to check that adding passengers exceeding capacity is rejected.
        [Fact]
        public void Elevator_Should_Reject_Passengers_Exceeding_Capacity()
        {
            // Arrange: Initialize an elevator with a capacity of 5 and 4 passengers initially.
            var elevator = new PassengerElevator(10, 0, 5); // ID 10, Max floor 10, Current floor 0
            elevator.AddPassengers(4); // Add 4 passengers

            // Act: Try to add 3 more passengers, which would exceed the capacity.
            var result = elevator.AddPassengers(3);

            // Assert: Verify that adding passengers exceeds capacity.
            Assert.False(result); // Should be false as adding exceeds capacity
            Assert.Equal(4, elevator.PassengerCount); // Assert on PassengerCount instead of CurrentPassengers
        }

        // Test to check the nearest elevator finding logic.
        // Test to check the nearest elevator finding logic.
        [Fact]
        public void ElevatorLogic_Should_Find_Nearest_Elevator()
        {
            // Arrange: Create a list of elevators with correct parameters as Elevator types.
            var elevators = new List<Elevator> // Change this to List<Elevator>
            {
                new PassengerElevator(1, 0, 5), // Elevator with ID 1 at floor 0
                new PassengerElevator(2, 3, 5),  // Elevator with ID 2 at floor 3
                new PassengerElevator(3, 2, 5)   // Elevator with ID 3 at floor 2
            };

            var elevatorLogic = new ElevatorLogic();

            // Act: Find the nearest elevator to floor 2.
            var nearestElevator = elevatorLogic.FindNearestElevator(elevators, 2);

            // Assert: Verify that the nearest elevator is not null.
            Assert.NotNull(nearestElevator);

            // Confirm that the nearest elevator is indeed the one at floor 2.
            Assert.Equal(2, nearestElevator.CurrentFloor);

            // Additional Assertion: Verify that the nearest elevator is the one with ID 3
            Assert.Equal(3, nearestElevator.Id); // Ensures the correct elevator is found
        }

        // Test to ensure the elevator doesn't exceed the max passenger capacity.
        [Fact]
        public void ElevatorLogic_Should_Capacity_Check()
        {
            // Arrange: Initialize an elevator.
            var elevator = new PassengerElevator(10, 0, 5); // ID 10, Current floor 0, Max capacity 5
            elevator.AddPassengers(4); // Add 4 passengers

            var elevatorLogic = new ElevatorLogic();

            // Act: Check if the elevator can take 1 more passenger.
            var canTakeMore = elevatorLogic.CanTakePassengers(elevator, 1);

            // Assert: Verify that it can take more passengers.
            Assert.True(canTakeMore);
        }

        // Test to check if the elevator can't take more than the capacity.
        [Fact]
        public void ElevatorLogic_Should_Reject_Passengers_When_Full()
        {
            // Arrange: Initialize an elevator with 5 max capacity and 5 current passengers.
            var elevator = new PassengerElevator(10, 0, 5); // ID 10, Current floor 0, Max capacity 5
            elevator.AddPassengers(5); // Fill the elevator to max capacity

            var elevatorLogic = new ElevatorLogic();

            // Act: Check if the elevator can take 1 more passenger.
            var canTakeMore = elevatorLogic.CanTakePassengers(elevator, 1);

            // Assert: Verify that it cannot take more passengers.
            Assert.False(canTakeMore); // Expecting it to return false since the elevator is full
        }
    }
}
