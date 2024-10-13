namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    public class GlassElevator : Elevator
    {
        public int TargetFloor { get; set; }

        // Constructor to initialize GlassElevator with id, currentFloor, and maxPassengerCapacity
        public GlassElevator(int id, int currentFloor, int maxPassengerCapacity, int maxFloor = 10) : base(id, maxFloor, maxPassengerCapacity, currentFloor, 0)
        {
            Id = id;
            CurrentFloor = currentFloor;
            MaxPassengerCapacity = maxPassengerCapacity;
        }

        public override void Move(int targetFloor)
        {
            TargetFloor = targetFloor;
            IsMoving = true;
            Console.WriteLine($"Glass Elevator {Id} moving to floor {TargetFloor}");
            // Add actual movement logic here
        }

        public virtual void Stop() // Mark it as virtual to allow overriding
        {
            IsMoving = false; // Assuming IsMoving is accessible
            Console.WriteLine($"Elevator {Id} has stopped.");
        }
    }
}
