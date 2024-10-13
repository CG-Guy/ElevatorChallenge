using ElevatorChallenge.ElevatorChallenge.src.Models;

namespace ElevatorChallenge.ElevatorChallenge.src.Services
{
    public class ElevatorControlSystem
    {
        private List<Elevator> _elevators;

        public ElevatorControlSystem()
        {
            _elevators = new List<Elevator>
            {
                new PassengerElevator(0, 1, 5),  // Provide id, currentFloor, and maxPassengerCapacity
                new HighSpeedElevator(1, 3, 10)  // Provide id, currentFloor, and maxPassengerCapacity
            };
        }

        // Function to request an elevator based on target floor and passenger count
        public void RequestElevator(int targetFloor, int passengerCount)
        {
            Elevator selectedElevator = FindBestElevator(targetFloor, passengerCount);

            if (selectedElevator != null)
            {
                Console.WriteLine($"Elevator {selectedElevator.Id} is assigned to go to floor {targetFloor}");
                selectedElevator.Move(targetFloor);  // This should now work
            }
            else
            {
                Console.WriteLine("No available elevator can accommodate the request.");
            }
        }

        // Function to initialize future elevator types
        public void InitializeElevators()
        {
            _elevators.Add(new GlassElevator(2, 1, 8));  // Provide id, currentFloor, and maxPassengerCapacity
            // Additional elevator types can be added dynamically here
        }

        // Logic to find the best elevator based on the target floor and passenger count
        private Elevator FindBestElevator(int targetFloor, int passengerCount)
        {
            var availableElevators = _elevators
                .Where(e => e.PassengerCount + passengerCount <= e.MaxPassengerCapacity && !e.IsMoving)
                .ToList();

            if (availableElevators.Count == 0) return null;  // No available elevators

            // Select the nearest elevator to the target floor
            Elevator bestElevator = availableElevators
                .OrderBy(e => Math.Abs(e.CurrentFloor - targetFloor))
                .FirstOrDefault();

            return bestElevator;
        }
    }
}
