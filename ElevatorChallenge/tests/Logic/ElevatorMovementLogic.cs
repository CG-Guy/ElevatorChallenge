using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.ElevatorChallenge.src.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ElevatorChallenge.ElevatorChallenge.tests.Logic
{
    public class ElevatorMovementLogicTests
    {
        private readonly Mock<ILogger<PassengerElevator>> _mockLogger;

        public ElevatorMovementLogicTests()
        {
            // Initialize the mock logger before running the tests
            _mockLogger = new Mock<ILogger<PassengerElevator>>();
        }

        [Fact]
        public async Task MoveElevatorToFloor_Should_Update_CurrentFloor()
        {
            // Arrange: Create mocks for logger and validator
            var mockLogger = new Mock<ILogger<PassengerElevator>>(); // Create a mock logger
            var mockValidator = new Mock<IElevatorValidator>(); // Create a mock elevator validator

            // Setup the validator mock to return valid results for the test case
            mockValidator
                .Setup(v => v.ValidateElevatorMovement(It.IsAny<Elevator>(), It.IsAny<int>()))
                .Returns(new ElevatorValidationResult { IsValid = true }); // Assume validation is successful

            var elevator = new PassengerElevator(1, 1, 5, mockLogger.Object); // ID 1, Current floor 1, Max capacity 5

            // Create an instance of ElevatorMovementLogic with the mock validator
            var movementLogic = new ElevatorMovementLogic(mockValidator.Object);

            // Act: Move to floor 3 asynchronously
            await movementLogic.MoveElevatorToFloor(elevator, 3);

            // Assert: Current floor should now be 3
            Assert.Equal(3, elevator.CurrentFloor);
        }

        [Fact]
        public async Task MoveElevatorToFloor_Should_Not_Move_Below_Min_Floor()
        {
            // Arrange: Create mocks for logger and validator
            var mockLogger = new Mock<ILogger<PassengerElevator>>(); // Create a mock logger
            var mockValidator = new Mock<IElevatorValidator>(); // Create a mock elevator validator

            // Setup the validator mock to return an invalid result for moving to floor 0
            mockValidator
                .Setup(v => v.ValidateElevatorMovement(It.IsAny<Elevator>(), It.IsAny<int>()))
                .Returns(new ElevatorValidationResult { IsValid = false, ErrorMessage = "Invalid target floor." });

            var elevator = new PassengerElevator(1, 1, 5, mockLogger.Object); // ID 1, Current floor 1, Max capacity 5

            // Create an instance of ElevatorMovementLogic with the mock validator
            var movementLogic = new ElevatorMovementLogic(mockValidator.Object);

            // Act: Attempt to move to an invalid floor (0)
            await movementLogic.MoveElevatorToFloor(elevator, 0);

            // Assert: Should remain on the current floor
            Assert.Equal(1, elevator.CurrentFloor); // Should remain on the current floor
        }

        [Fact]
        public void Elevator_Initialization_Should_Set_Default_Values()
        {
            // Arrange: Ensure that the test is passing the correct values for initialization
            var elevator = new PassengerElevator(1, 1, 5, _mockLogger.Object); // Initializing with current floor set to 1

            // Act: Check initial state of the elevator (expected values)

            // Assert: Check that the default values are set correctly
            Assert.Equal(1, elevator.CurrentFloor); // Expect current floor to be 1
            Assert.Equal(5, elevator.MaxPassengerCapacity); // Ensure passenger capacity is correct
            Assert.Equal(10, elevator.MaxFloor); // Default max floor should be 10
            Assert.False(elevator.IsMoving); // Elevator should not be moving initially
            Assert.Equal("Stationary", elevator.Direction); // Direction should be stationary initially
        }

        // Test to ensure IsMoving is true while elevator is in motion
        [Fact]
        public async Task MoveElevatorToFloor_Should_Set_IsMoving_To_True_While_Moving()
        {
            // Arrange: Create mocks for logger and validator
            var mockLogger = new Mock<ILogger<PassengerElevator>>(); // Create a mock logger
            var mockValidator = new Mock<IElevatorValidator>(); // Create a mock elevator validator

            // Setup the validator mock to always return valid result
            mockValidator
                .Setup(v => v.ValidateElevatorMovement(It.IsAny<Elevator>(), It.IsAny<int>()))
                .Returns(new ElevatorValidationResult { IsValid = true });

            var elevator = new PassengerElevator(1, 1, 5, mockLogger.Object); // ID 1, Max floor 5, Capacity 5, Current floor 1

            // Create an instance of ElevatorMovementLogic with the mock validator
            var movementLogic = new ElevatorMovementLogic(mockValidator.Object);

            // Act: Start moving the elevator
            var moveTask = movementLogic.MoveElevatorToFloor(elevator, 3);

            // Assert: Check IsMoving immediately after calling MoveElevatorToFloor
            Assert.True(elevator.IsMoving); // This should now pass while elevator is in motion

            await moveTask; // Await the completion of the movement
        }

        // Test to ensure IsMoving is false after moving
        [Fact]
        public async Task MoveElevatorToFloor_Should_Set_IsMoving_To_False_After_Moving()
        {
            // Arrange: Create mocks for logger and validator
            var mockLogger = new Mock<ILogger<PassengerElevator>>(); // Create a mock logger
            var mockValidator = new Mock<IElevatorValidator>(); // Create a mock elevator validator

            // Setup the validator mock to always return valid result
            mockValidator
                .Setup(v => v.ValidateElevatorMovement(It.IsAny<Elevator>(), It.IsAny<int>()))
                .Returns(new ElevatorValidationResult { IsValid = true });

            var elevator = new PassengerElevator(1, 1, 5, mockLogger.Object); // ID 1, Max floor 5, Capacity 5, Current floor 1

            // Create an instance of ElevatorMovementLogic with the mock validator
            var movementLogic = new ElevatorMovementLogic(mockValidator.Object);

            // Act: Move to floor 3
            await movementLogic.MoveElevatorToFloor(elevator, 3);

            // Assert: Should now pass after the elevator has moved
            Assert.False(elevator.IsMoving); // This should now pass after the elevator has moved
        }

        // Test for edge case: Moving to the same floor
        [Fact]
        public async Task MoveElevatorToFloor_Should_Not_Move_When_Current_Floor_Equals_Target_Floor()
        {
            // Arrange: Create mocks for logger and validator
            var mockLogger = new Mock<ILogger<PassengerElevator>>(); // Create a mock logger
            var mockValidator = new Mock<IElevatorValidator>(); // Create a mock elevator validator

            // Setup the validator mock to return valid result for valid movements
            mockValidator
                .Setup(v => v.ValidateElevatorMovement(It.IsAny<Elevator>(), It.IsAny<int>()))
                .Returns(new ElevatorValidationResult { IsValid = true });

            // Setup the validator mock to return invalid result when moving to the same floor
            mockValidator
                .Setup(v => v.ValidateElevatorMovement(It.IsAny<Elevator>(), 1))
                .Returns(new ElevatorValidationResult { IsValid = false, ErrorMessage = "Already at the target floor." });

            var elevator = new PassengerElevator(1, 1, 5, mockLogger.Object); // Current floor is 1

            // Create an instance of ElevatorMovementLogic with the mock validator
            var movementLogic = new ElevatorMovementLogic(mockValidator.Object);

            // Act: Attempt to move to the same floor
            await movementLogic.MoveElevatorToFloor(elevator, 1);

            // Assert: Should remain on the current floor
            Assert.Equal(1, elevator.CurrentFloor); // Should remain on the current floor
            Assert.Equal("Stationary", elevator.Direction); // Should be stationary
        }

        [Fact]
        public void PassengerService_Should_Add_Passenger_When_Space_Exists()
        {
            // Arrange: Initialize an elevator and a passenger service
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            ILogger<PassengerElevator> logger = loggerFactory.CreateLogger<PassengerElevator>();
            var elevator = new PassengerElevator(10, 1, 5, logger); // Use a valid current floor of 1
            elevator.AddPassengers(4); // Add 4 passengers

            var passengerServiceLogger = loggerFactory.CreateLogger<PassengerService>();
            var passengerService = new PassengerService(elevator, passengerServiceLogger);

            // Assume the Passenger class has a constructor that requires three parameters
            var passenger = new Passenger(1, 70, 5); // Replace with actual parameter values

            // Act: Add a passenger
            passengerService.AddPassenger(passenger);

            // Assert: Verify that the passenger count is now 5
            Assert.Equal(5, elevator.PassengerCount);
        }

        [Fact]
        public void PassengerService_Should_Not_Add_Passenger_When_Full()
        {
            // Arrange: Initialize an elevator at max capacity
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            ILogger<PassengerElevator> logger = loggerFactory.CreateLogger<PassengerElevator>();
            var elevator = new PassengerElevator(10, 1, 5, logger); // Use a valid current floor of 1
            elevator.AddPassengers(5); // Add max capacity

            var passengerServiceLogger = loggerFactory.CreateLogger<PassengerService>();
            var passengerService = new PassengerService(elevator, passengerServiceLogger);

            // Provide valid arguments for the Passenger constructor
            var passenger = new Passenger(1, 70, 5); // Example values for id, weight, and destination floor

            // Act: Attempt to add another passenger
            passengerService.AddPassenger(passenger);

            // Assert: Verify that the passenger count remains at max capacity
            Assert.Equal(5, elevator.PassengerCount);
        }

    }
}
