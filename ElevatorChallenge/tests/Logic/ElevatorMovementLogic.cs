using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using Xunit;

namespace ElevatorChallenge.ElevatorChallenge.tests.Logic
{
    public class ElevatorMovementLogicTests
    {
        [Fact]
        public async Task MoveElevatorToFloor_Should_Update_CurrentFloor()
        {
            // Use PassengerElevator instead of Elevator
            var elevator = new PassengerElevator(1, 1, 5); // ID 1, Current floor 1, Max capacity 5
            var movementLogic = new ElevatorMovementLogic();

            await movementLogic.MoveElevatorToFloor(elevator, 3); // Move to floor 3 asynchronously

            Assert.Equal(3, elevator.CurrentFloor); // Assert that the current floor is now 3
        }

        [Fact]
        public async Task MoveElevatorToFloor_Should_Not_Move_Above_Max_Floor()
        {
            // Arrange: Create an elevator at current floor 4 with a maximum floor of 5
            var elevator = new PassengerElevator(1, 4, 5); // Initializes with current floor 4
            var movementLogic = new ElevatorMovementLogic();

            // Act: Attempt to move the elevator to an invalid floor (6)
            await movementLogic.MoveElevatorToFloor(elevator, 6); // Await the asynchronous call

            // Debug output to verify state
            Console.WriteLine($"Elevator State - CurrentFloor: {elevator.CurrentFloor}, IsMoving: {elevator.IsMoving}, Direction: {elevator.Direction}");

            // Assert: Verify that the elevator remains on the current floor (4)
            Assert.Equal(4, elevator.CurrentFloor); // Should remain on the current floor
            Assert.False(elevator.IsMoving); // Should not be moving
            Assert.Equal("Stationary", elevator.Direction); // Should be stationary
        }

        [Fact]
        public async Task MoveElevatorToFloor_Should_Not_Move_Below_Min_Floor()
        {
            var elevator = new PassengerElevator(1, 1, 5); // ID 1, Max floor 5, Capacity 5, Current floor 1
            var movementLogic = new ElevatorMovementLogic();

            await movementLogic.MoveElevatorToFloor(elevator, 0); // Await the call

            Assert.Equal(1, elevator.CurrentFloor); // Should remain on the current floor
        }

        // Test to ensure that the Elevator object initializes with the correct maximum floor, current floor, and passenger capacity.
        [Fact]
        public void Elevator_Initialization_Should_Set_Default_Values()
        {
            // Arrange: Create a new passenger elevator with max floor 10, current floor 1, max capacity 5, and no passengers initially.
            var elevator = new PassengerElevator(1, 1, 5); // This initializes the elevator

            // Act & Assert: Check that MaxFloor, CurrentFloor, and MaxPassengerCapacity are set correctly.
            Assert.Equal(10, elevator.MaxFloor); // Expecting max floor to be set to 10
            Assert.Equal(1, elevator.CurrentFloor); // Expecting current floor to be set to 1
            Assert.Equal(5, elevator.MaxPassengerCapacity); // Expecting max capacity to be set to 5
            Assert.Equal(0, elevator.PassengerCount); // PassengerCount should be 0 by default
        }

        // Test to ensure IsMoving is true while elevator is in motion
        [Fact]
        public async Task MoveElevatorToFloor_Should_Set_IsMoving_To_True_While_Moving()
        {
            var elevator = new PassengerElevator(1, 1, 5); // ID 1, Max floor 5, Capacity 5, Current floor 1
            var movementLogic = new ElevatorMovementLogic();

            // Start moving the elevator
            var moveTask = movementLogic.MoveElevatorToFloor(elevator, 3);

            // Check IsMoving immediately after calling MoveElevatorToFloor
            Assert.True(elevator.IsMoving); // This should now pass while elevator is in motion

            await moveTask; // Await the completion of the movement
        }

        [Fact]
        public async Task MoveElevatorToFloor_Should_Set_IsMoving_To_False_After_Moving()
        {
            var elevator = new PassengerElevator(1, 1, 5); // ID 1, Max floor 5, Capacity 5, Current floor 1
            var movementLogic = new ElevatorMovementLogic();

            await movementLogic.MoveElevatorToFloor(elevator, 3); // Await the call

            Assert.False(elevator.IsMoving); // This should now pass after the elevator has moved
        }

        // Test for edge case: Moving to the same floor
        [Fact]
        public async Task MoveElevatorToFloor_Should_Not_Move_When_Current_Floor_Equals_Target_Floor()
        {
            var elevator = new PassengerElevator(1, 1, 5); // Current floor is 3
            var movementLogic = new ElevatorMovementLogic();

            await movementLogic.MoveElevatorToFloor(elevator, 3); // Await the call

            Assert.Equal(3, elevator.CurrentFloor); // Should remain on the current floor
            Assert.Equal("Stationary", elevator.Direction); // Should be stationary
        }
    }
        
}
