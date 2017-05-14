using RJController.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RJController.DTO
{
    public class DTOVariableContent
    {
        public string GroupName { get; set; }

        public string ObjectName { get; set; }

        public string ContentName { get; set; }

        public string ContentValue { get; set; }

        public DTOVariableContent() { }

        public DTOVariableContent(string groupName, string objectName, string contentName, string contentValue)
        {
            GroupName = groupName;
            ObjectName = objectName;
            ContentName = contentName;
            ContentValue = contentValue;
        }
    }
}
