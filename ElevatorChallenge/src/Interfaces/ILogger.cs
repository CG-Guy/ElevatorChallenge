using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Interfaces
{
    public interface ILogger
    {
        void Info(string message);
        void LogWarning(string invalidPassengerCountMessage);
    }
}
