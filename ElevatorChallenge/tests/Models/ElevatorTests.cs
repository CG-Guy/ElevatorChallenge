using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;

namespace ElevatorChallenge.Tests
{
    public class ElevatorTests
    {
        private readonly Mock<ILogger<PassengerElevator>> _mockLogger;
        private readonly IElevatorLogic _elevatorLogic;

        public ElevatorTests()
        {
            _mockLogger = new Mock<ILogger<PassengerElevator>>();
            _elevatorLogic = new ElevatorLogic();
        }

        [Fact]
        public void Elevator_Initialization_Should_Set_Default_Values()
        {
            // Arrange
            var elevator = new PassengerElevator(1, 1, 5, _mockLogger.Object);

            // Act & Assert
            Assert.Equal(10, elevator.MaxFloor);
            Assert.Equal(1, elevator.CurrentFloor);
            Assert.Equal(5, elevator.MaxPassengerCapacity);  // No need for cast
            Assert.Equal(0, elevator.PassengerCount);        // No need for cast
        }

        [Theory]
        [InlineData(5, 0, 1, true)] // Can add 1 passenger when none are in
        [InlineData(5, 4, 1, true)] // Can add 1 passenger when 4 are in (total becomes 5)
        [InlineData(5, 4, 2, false)] // Cannot add 2 passengers when 4 are in (would exceed capacity)
        public void CanTakePassengers_Should_ReturnExpectedResult_WhenGivenCurrentLoad(int maxCapacity, int currentPassengers, int passengersToAdd, bool expectedResult)
        {
            // Arrange
            PassengerElevator elevator = new PassengerElevator(1, 0, maxCapacity, _mockLogger.Object);
            elevator.AddPassengers(currentPassengers);

            // Act
            var canTakeMore = _elevatorLogic.CanTakePassengers(elevator, passengersToAdd);

            // Assert
            Assert.Equal(expectedResult, canTakeMore);
        }

        [Fact]
        public void Elevator_Should_Reject_Passengers_Exceeding_Capacity()
        {
            // Arrange
            IElevator elevator = new PassengerElevator(10, 0, 5, _mockLogger.Object);
            ((PassengerElevator)elevator).AddPassengers(4);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => ((PassengerElevator)elevator).AddPassengers(3));
            Assert.Equal("Cannot add 3 passengers to Elevator 10. Capacity exceeded. Current count: 4, Attempted to add: 3, Max capacity: 5.", exception.Message);
            Assert.Equal(4, ((PassengerElevator)elevator).PassengerCount);
        }

        [Fact]
        public void ElevatorLogic_Should_Find_Nearest_Elevator()
        {
            // Arrange
            var elevators = new List<IElevator>
            {
                new PassengerElevator(1, 1, 5, _mockLogger.Object),
                new PassengerElevator(2, 3, 5, _mockLogger.Object),
                new PassengerElevator(3, 2, 5, _mockLogger.Object)
            };

            // Act
            var nearestElevator = _elevatorLogic.FindNearestElevator(elevators, 2) as PassengerElevator;

            // Assert
            Assert.NotNull(nearestElevator);
            Assert.Equal(2, nearestElevator.CurrentFloor);
            Assert.Equal(3, nearestElevator.Id);
        }

        [Theory]
        [InlineData(4, true)]   // Should accept one more passenger
        [InlineData(5, false)]  // Should reject more passengers when at capacity
        public void ElevatorLogic_Should_Check_Capacity(int currentPassengers, bool expectedResult)
        {
            // Arrange
            PassengerElevator elevator = new PassengerElevator(10, 0, 5, _mockLogger.Object);
            elevator.AddPassengers(currentPassengers);

            // Act
            var canTakeMore = _elevatorLogic.CanTakePassengers(elevator, 1);

            // Assert
            Assert.Equal(expectedResult, canTakeMore);
        }

        [Fact]
        public async Task Elevator_Should_Reject_Movement_Above_Max_Floor()
        {
            // Arrange
            IElevator elevator = new PassengerElevator(10, 10, 5, _mockLogger.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => ((PassengerElevator)elevator).MoveToFloor(11));
            Assert.Equal("Cannot move above max floor 10.", exception.Message);
        }

        [Fact]
        public void Elevator_Should_Remove_Passengers_Correctly()
        {
            // Arrange
            IElevator elevator = new PassengerElevator(10, 0, 5, _mockLogger.Object);
            ((PassengerElevator)elevator).AddPassengers(3);

            // Act
            ((PassengerElevator)elevator).RemovePassengers(2);

            // Assert
            Assert.Equal(1, ((PassengerElevator)elevator).PassengerCount);
        }

        [Fact]
        public void Elevator_Should_Not_Remove_Passengers_Exceeding_Current_Count()
        {
            // Arrange
            IElevator elevator = new PassengerElevator(10, 0, 5, _mockLogger.Object);
            ((PassengerElevator)elevator).AddPassengers(2);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => ((PassengerElevator)elevator).RemovePassengers(3));

            // Adjusted to match the actual exception message thrown
            Assert.Equal("Cannot remove more passengers than currently on the elevator.", exception.Message);

            // Ensure the passenger count remains the same after attempting to remove more passengers than available
            Assert.Equal(2, ((PassengerElevator)elevator).PassengerCount);
        }

    }
}
