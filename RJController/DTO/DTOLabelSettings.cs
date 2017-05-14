using RJController.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RJController.DTO
{
    public class DTOLabelSettings
    {
        public string GroupName { get; set; }
        public string LabelName { get; set; }
        public string ObjectName { get; set; }
        public string ContentName { get; set; }
        public int OutputControl { get; set; }
        public int DataField { get; set; }


        public DTOLabelSettings() { }

        public DTOLabelSettings(string group, string labelName, string objectName, string contentName, int output, int data)
        {
            GroupName = group;
            LabelName = labelName;
            ObjectName = objectName;
            ContentName = contentName;
            OutputControl = output;
            DataField = data;
        }
    }
}
