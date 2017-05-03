using RJController.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController.Label
{
    public class ObjectContent : IObjectContent
    {
        public string ContentName { get; set; }

        public string ContentValue { get; set; }

        public string ContentType { get; set; }

        public int OutputControl { get; set; }

        public int DataField { get; set; }


        public ObjectContent() { }

        public ObjectContent(string contentName, string contentValue, string contentType, int dataField, int outputControl)
        {
            if (string.IsNullOrEmpty(contentName))
            {
                throw new System.ArgumentException("Parameter can't be null");
            }
            else
            {
                ContentName = contentName;
            }

            ContentValue = contentValue;
            ContentType = contentType;
            DataField = dataField;
            OutputControl = outputControl;
        }
    }
}
