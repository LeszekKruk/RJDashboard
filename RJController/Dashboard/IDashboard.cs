using RJController.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController.Dashboard
{
    public interface IDashboard
    {
        void UpdateDashboard(EventType rjEvent);
        void DisplayEvents(string log);
        void DisplayLogsAndErrors(string log, string errorCodeDomain, string messageError);
        void ShowProblemSolution(ErrorType type);
    }
}
