using RJController.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController.Job
{
    public interface IVariableContent : IObjectContent, ILabelObject
    {
        string GroupName {get; set;}
    }
}
