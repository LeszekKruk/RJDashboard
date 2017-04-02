using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController.Label
{
    public interface IContent
    {
        string Name { get; }
        string Value { get; set; }
    }
}
