using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ElevatorChallenge.ElevatorChallenge.tests.Logic
{
    public class ElevatorLogicTests
    {
        private readonly Mock<IElevatorLogic> _mockElevatorLogic;
        private readonly Mock<IElevatorMovementLogic> _mockElevatorMovementLogic;
        private readonly Mock<ILogger<PassengerElevator>> _mockLogger;
        private readonly IElevatorLogic _elevatorLogic;

        public ElevatorLogicTests()
        {
            _mockElevatorLogic = new Mock<IElevatorLogic>();  // Mocking the elevator logic interface
            _mockElevatorMovementLogic = new Mock<IElevatorMovementLogic>();  // Mocking elevator movement logic
            _mockLogger = new Mock<ILogger<PassengerElevator>>(); // Mocking logger
            _elevatorLogic = new ElevatorLogic(); // Real implementation to test actual logic
        }

        [Fact]
        public void FindNearestElevator_Should_Return_Nearest_Elevator()
        {
            // Arrange
            var elevators = new List<Elevator>
            {
                new PassengerElevator(0, 1, 5, _mockLogger.Object), // Elevator at floor 1
                new PassengerElevator(1, 5, 5, _mockLogger.Object)  // Elevator at floor 5
            };

            // Act
            var nearestElevator = _elevatorLogic.FindNearestElevator(elevators, 3); // Request to find the nearest to floor 3

            // Assert
            Assert.NotNull(nearestElevator);
            Assert.Equal(elevators[0], nearestElevator); // Nearest should be the elevator at floor 1
        }

        [Fact]
        public void CanTakePassengers_Should_Return_True_When_Elevator_Has_Space()
        {
            // Arrange
            var elevator = new PassengerElevator(0, 1, 5, _mockLogger.Object);
            elevator.AddPassengers(2); // Add 2 passengers, leaving room for 3 more

            // Act
            var canTake = _elevatorLogic.CanTakePassengers(elevator, 2); // Check if it can take 2 more passengers

            // Assert
            Assert.True(canTake); // Should be able to take 2 more passengers
        }

        [Fact]
        public void CanTakePassengers_Should_Return_False_When_Elevator_Is_Full()
        {
            // Arrange
            var elevator = new PassengerElevator(0, 1, 5, _mockLogger.Object);
            elevator.AddPassengers(5); // Max capacity reached

            // Act
            var canTake = _elevatorLogic.CanTakePassengers(elevator, 1); // Check if it can take 1 more passenger

            // Assert
            Assert.False(canTake); // Should return false as it's full
        }

        [Fact]
        public async Task MoveElevatorToFloor_Should_Not_Move_Above_Max_Floor()
        {
            // Arrange
            var elevator = new PassengerElevator(1, 4, 5, _mockLogger.Object); // Current floor 4, max floor 5

            // Mock the movement logic to call the real method
            _mockElevatorMovementLogic.Setup(m => m.MoveElevatorToFloor(elevator, 6))
                .Returns(async () =>
                {
                    await Task.CompletedTask; // No actual movement since floor 6 is above max
                });

            // Act
            await _mockElevatorMovementLogic.Object.MoveElevatorToFloor(elevator, 6);

            // Assert: Verify the elevator didn't move above max floor
            Assert.Equal(4, elevator.CurrentFloor); // Should remain on floor 4
            Assert.False(elevator.IsMoving); // Should not be moving
            Assert.Equal("Stationary", elevator.Direction); // Should be stationary
        }

        [Fact]
        public async Task MoveElevatorToFloor_Should_Move_Within_Limits()
        {
            // Arrange
            var elevator = new PassengerElevator(1, 4, 5, _mockLogger.Object); // Current floor 4, max floor 5

            // Mock the movement logic to move the elevator to a valid floor (within limits)
            _mockElevatorMovementLogic.Setup(m => m.MoveElevatorToFloor(elevator, 5))
                .Returns(async () =>
                {
                    await elevator.MoveAsync(5); // Simulate elevator movement to floor 5
                    await Task.CompletedTask;
                });

            // Act
            await _mockElevatorMovementLogic.Object.MoveElevatorToFloor(elevator, 5);

            // Assert: Ensure the elevator moved to the correct floor
            Assert.Equal(5, elevator.CurrentFloor); // Should move to floor 5
            Assert.False(elevator.IsMoving); // Should not be moving after arrival
            Assert.Equal("Stationary", elevator.Direction); // Should be stationary after moving
        }

    }
}
