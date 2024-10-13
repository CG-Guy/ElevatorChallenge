using ElevatorChallenge.ElevatorChallenge.src.Services;
using ElevatorChallenge.Services;

namespace ElevatorChallenge.Controllers
{
    public class ElevatorController
    {
        private readonly ElevatorService _elevatorService;

        public ElevatorController(ElevatorService elevatorService)
        {
            _elevatorService = elevatorService;
        }

        // Entry point for the elevator control system interaction
        public void Start()
        {
            Console.WriteLine("Welcome to the Elevator Control System!");
            while (true)
            {
                string floorInput = PromptForFloor();
                if (string.Equals(floorInput, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break; // Exit the application
                }

                if (int.TryParse(floorInput, out int floor) && floor >= 0)
                {
                    int passengers = PromptForPassengers();
                    if (passengers > 0)
                    {
                        RequestElevator(floor, passengers);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid floor number. Please try again.");
                }

                ShowElevatorStatus();
            }
        }

        // Method to prompt user for a floor number
        private string PromptForFloor()
        {
            Console.WriteLine("\nEnter the floor number to call the elevator (or type 'exit' to quit):");
            return Console.ReadLine();
        }

        // Method to prompt user for the number of passengers
        private int PromptForPassengers()
        {
            Console.WriteLine("Enter the number of passengers waiting:");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int passengers) && passengers > 0)
                {
                    return passengers; // Return valid passenger count
                }
                Console.WriteLine("Invalid number of passengers. Please enter a positive number.");
            }
        }

        // Method to request an elevator
        public void RequestElevator(int floor, int passengers)
        {
            var elevator = _elevatorService.AssignElevator(floor, passengers);
            if (elevator != null)
            {
                Console.WriteLine($"Elevator dispatched to floor {floor} with {passengers} passengers.");
            }
            else
            {
                Console.WriteLine("No available elevators to dispatch.");
            }
        }

        // Method to show the current status of elevators
        public void ShowElevatorStatus()
        {
            var elevators = _elevatorService.GetElevatorsStatus();
            Console.WriteLine("\nCurrent Elevator Status:");

            foreach (var elevator in elevators)
            {
                Console.WriteLine($"Elevator ID: {elevator.Id}, " +
                                  $"Current Floor: {elevator.CurrentFloor}, " +
                                  $"Moving: {elevator.IsMoving}, " +
                                  $"Direction: {elevator.Direction}, " + // Updated to use ElevatorDirection
                                  $"Passengers: {elevator.PassengerCount}/{elevator.MaxPassengerCapacity}"); // Assuming Capacity is the correct property
            }
        }
    }
}
