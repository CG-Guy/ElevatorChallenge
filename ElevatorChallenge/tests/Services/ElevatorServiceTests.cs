using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.ElevatorChallenge.src.Services;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using ElevatorChallenge.Services;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;

namespace ElevatorChallenge.ElevatorChallenge.tests.Services
{
    public class ElevatorServiceTests
    {
        private readonly Mock<ILogger<PassengerElevator>> _loggerMock;

        public ElevatorServiceTests()
        {
            // Initialize the logger mock for tests
            _loggerMock = new Mock<ILogger<PassengerElevator>>();
        }


        private readonly Mock<ILogger<PassengerElevator>> _elevatorLoggerMock; // Specific ILogger for PassengerElevator

        public async Task AssignElevator_Should_Return_Nearest_Elevator()
        {
            // Arrange: Create mocks for other dependencies
            var applicationLoggerMock = new Mock<IApplicationLogger>();
            var elevatorValidatorMock = new Mock<IElevatorValidator>();
            var elevatorManagementServiceMock = new Mock<IElevatorManagementService>(); // Mocking interface

            // Setup mock behavior for the elevator validator
            elevatorValidatorMock
                .Setup(ev => ev.ValidateElevatorMovement(It.IsAny<Elevator>(), It.IsAny<int>()))
                .Returns(new ElevatorValidationResult { IsValid = true });

            // Mock logger for elevators, ensuring it implements IApplicationLogger
            var elevatorLoggerMock_ = new Mock<IApplicationLogger>(); // Use IApplicationLogger instead

            var elevatorLoggerMock = new Mock<ILogger<PassengerElevator>>();


            var elevators = new List<PassengerElevator>
            {
                new PassengerElevator(1, 1, 5, elevatorLoggerMock.Object),
                new PassengerElevator(2, 5, 5, elevatorLoggerMock.Object)
            };

            foreach (var elevator in elevators)
            {
                elevator.IsInService = true;
            }

            // Initialize ElevatorService with elevators and mocked dependencies
            var elevatorService = new ElevatorService(
                elevators,
                applicationLoggerMock.Object,
                elevatorLoggerMock_.Object, // Pass the elevator logger mock
                elevatorValidatorMock.Object,
                (ElevatorManagementService)elevatorManagementServiceMock.Object); // Ensure this is compatible with ElevatorManagementService

            // Act: Request for floor 3, assign based on proximity
            var assignedElevator = await elevatorService.AssignElevatorAsync(3, 0);

            // Assert: Ensure the closest elevator is chosen
            Assert.NotNull(assignedElevator);
            Assert.Equal(1, assignedElevator.CurrentFloor); // Nearest elevator should be the one at floor 1
        }

        // Test to handle the scenario where no elevators are available
        [Fact]
        public async Task AssignElevator_Should_Return_Null_When_No_Elevators_Available()
        {
            // Arrange: Create mocks for other dependencies
            var applicationLoggerMock = new Mock<IApplicationLogger>();
            var elevatorLoggerMock = new Mock<IApplicationLogger>(); // Separate logger for elevator-specific operations
            var elevatorValidatorMock = new Mock<IElevatorValidator>();
            var elevatorManagementServiceMock = new Mock<IElevatorManagementService>(); // Use the interface

            // Create an empty list of elevators
            var elevators = new List<PassengerElevator>();

            // Initialize ElevatorService with the empty list and mocked dependencies
            var elevatorService = new ElevatorService(
                elevators,
                applicationLoggerMock.Object,    // Pass mocked logger
                elevatorLoggerMock.Object,       // Pass mocked elevator logger
                elevatorValidatorMock.Object,    // Pass mocked elevator validator
                (ElevatorManagementService)elevatorManagementServiceMock.Object  // Pass mocked ElevatorManagementService
            );

            // Act: Request an elevator when none are available
            var assignedElevator = await elevatorService.AssignElevatorAsync(1, 0); // Example request for floor 1

            // Assert: Ensure the result is null as no elevators are available
            Assert.Null(assignedElevator);
        }

        [Fact]
        public void GetElevatorsStatus_Should_Return_Empty_When_No_Elevators()
        {
            // Arrange: Create an empty list of PassengerElevators
            var elevators = new List<PassengerElevator>(); // Initialize an empty list

            // Create mock objects for required parameters
            var loggerMock = new Mock<IApplicationLogger>();
            var elevatorLoggerMock = new Mock<IApplicationLogger>();
            var elevatorValidatorMock = new Mock<IElevatorValidator>();
            var elevatorManagementServiceMock = new Mock<ElevatorManagementService>();

            // Use the constructor that takes the required parameters
            var elevatorService = new ElevatorService(
                elevators,
                loggerMock.Object,
                elevatorLoggerMock.Object,
                elevatorValidatorMock.Object,
                elevatorManagementServiceMock.Object
            );

            // Act: Get the status of all elevators
            var statuses = elevatorService.GetElevatorsStatus();

            // Assert: Verify that the returned statuses are empty
            Assert.Empty(statuses); // There should be no elevator statuses
        }
        // Additional tests can be added based on specific behaviors and requirements of the ElevatorService class
    }
}
