using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController.Label
{
    public interface IObjectContent
    {
        string ContentName { get; set; }
        string ContentValue { get; set; }
        int OutputControl { get; set; }
        int DataField { get; set; }
    }
}
