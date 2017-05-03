using RJController.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RJController.Label
{
    public class VariableDataLabel : IVariableDataLabel
    {
        public string GroupName { get; set; }

        public string LabelName { get; set; }

        public string ObjectName { get; set; }

        public string ContentName { get; set; }

        public string ContentValue { get; set; }

        public string ContentType { get; set; }

        public int OutputControl { get; set; }

        public int DataField { get; set; }
    }
}
