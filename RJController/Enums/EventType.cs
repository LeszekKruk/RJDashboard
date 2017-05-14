using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController.Enums
{
    public enum EventType
    {
        CONNECT = 1,
        DISCONNECT,
        JOBSET,
        JOBSTOPPED,
        JOBSTARTED,
        IOSET,
        PRINTSTARTED
    }
}
