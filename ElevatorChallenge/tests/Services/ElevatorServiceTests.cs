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


        [Fact]
        public async Task AssignElevator_Should_Return_Nearest_Elevator()
        {
            // Arrange: Create mocks for other dependencies
            var applicationLoggerMock = new Mock<IApplicationLogger>();
            var elevatorLogicMock = new Mock<IElevatorLogic>();
            var elevatorMovementLogicMock = new Mock<IElevatorMovementLogic>();
            var elevatorValidatorMock = new Mock<IElevatorValidator>();
            var elevatorManagementServiceMock = new Mock<ElevatorManagementService>();

            // Setup mock behavior
            elevatorLogicMock.Setup(el => el.CanTakePassengers(It.IsAny<PassengerElevator>(), It.IsAny<int>()))
                             .Returns(true);
            elevatorMovementLogicMock.Setup(eml => eml.MoveElevatorToFloor(It.IsAny<Elevator>(), It.IsAny<int>()))
                                     .Returns(Task.CompletedTask);

            // Initialize the logger mock
            var _elevatorLoggerMock = new Mock<ILogger<PassengerElevator>>();

            // Create elevators with specific logger for PassengerElevator
            var elevators = new List<PassengerElevator>
            {
                new PassengerElevator(1, 1, 5, _elevatorLoggerMock.Object), // Elevator with ID 1, on floor 1
                new PassengerElevator(2, 5, 5, _elevatorLoggerMock.Object)  // Elevator with ID 2, on floor 5
            };

            // Ensure the elevators are in service for assignment
            foreach (var elevator in elevators)
            {
                elevator.IsInService = true;
            }

            // Initialize ElevatorService with elevators and mocked dependencies
            var elevatorService = new ElevatorService(
                elevators, // Pass the list of elevators
                applicationLoggerMock.Object, // Correctly pass logger (IApplicationLogger)
                (IApplicationLogger)elevatorLogicMock.Object, // Pass IElevatorLogic
                elevatorValidatorMock.Object, // Pass IElevatorValidator
                elevatorManagementServiceMock.Object); // Pass mock ElevatorManagementService

            // Act: Request for floor 3, assign based on proximity
            var assignedElevator = await elevatorService.AssignElevatorAsync(3, 0);

            // Assert: Ensure the closest elevator is chosen
            Assert.NotNull(assignedElevator); // Ensure an elevator was assigned
            Assert.Equal(1, assignedElevator.CurrentFloor); // Nearest elevator should be the one at floor 1
        }


        // Test to check if the statuses of all elevators are returned correctly
        [Fact]
        public void GetElevatorsStatus_Should_Return_All_Elevators_Status()
        {
            // Arrange: Create a list of elevators
            var elevators = new List<PassengerElevator>
            {
                new PassengerElevator(1, 1, 5, _loggerMock.Object), // Elevator with ID 1, on floor 1
                new PassengerElevator(2, 2, 5, _loggerMock.Object)  // Elevator with ID 2, on floor 2
            };
            elevators[0].AddPassengers(0); // No passengers in elevator 1
            elevators[1].AddPassengers(2); // 2 passengers in elevator 2

            var elevatorService = new ElevatorService(elevators);

            // Act: Get the status of all elevators
            var statuses = elevatorService.GetElevatorsStatus();

            // Assert: Verify that the statuses match expected values
            Assert.Equal(2, statuses.Count); // There should be two elevators
            Assert.Equal(1, statuses[0].CurrentFloor); // Elevator 1 is on floor 1
            Assert.Equal(2, statuses[1].PassengerCount); // Elevator 2 has 2 passengers
        }

        // Test to handle the scenario where no elevators are available
        [Fact]
        public async Task AssignElevator_Should_Return_Null_When_No_Elevators_Available()
        {
            // Arrange: Create mocks for other dependencies
            var applicationLoggerMock = new Mock<IApplicationLogger>();
            var elevatorLogicMock = new Mock<IElevatorLogic>();
            var elevatorValidatorMock = new Mock<IElevatorValidator>();
            var elevatorMovementLogicMock = new Mock<IElevatorMovementLogic>();

            // Create an empty list of elevators
            var elevators = new List<PassengerElevator>();

            // Initialize ElevatorService with the empty list and mocked dependencies
            var elevatorService = new ElevatorService(
                elevators,
                applicationLoggerMock.Object, // Pass mocked logger
                (IApplicationLogger)elevatorLogicMock.Object,      // Pass mocked elevator logic
                elevatorValidatorMock.Object,  // Pass mocked elevator validator
                (ElevatorManagementService)elevatorMovementLogicMock.Object); // Pass mocked elevator movement logic

            // Act: Try to assign an elevator
            var assignedElevator = await elevatorService.AssignElevatorAsync(3, 0); // Await here

            // Assert: Ensure no elevator can be assigned
            Assert.Null(assignedElevator);
        }

        [Fact]
        public void GetElevatorsStatus_Should_Return_Empty_When_No_Elevators()
        {
            // Arrange: Create an empty list of elevators
            var elevators = new List<PassengerElevator>();
            var elevatorService = new ElevatorService(elevators);

            // Act: Get the status of all elevators
            var statuses = elevatorService.GetElevatorsStatus();

            // Assert: Verify that the returned statuses are empty
            Assert.Empty(statuses); // There should be no elevator statuses
        }

        // Additional tests can be added based on specific behaviors and requirements of the ElevatorService class
    }
}
