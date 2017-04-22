using RJController.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RJController.Label
{
    public class PrintManagement : IManagementable
    {
        string _groupName;
        string _labelName;

        private string _objectName;

        private string _contentType;
        private string _contentName;
        private string _contentValue;
        int _dataField;
        int _outputControl;

        public string GroupName
        {
            get
            {
                return _groupName;
            }

            set
            {
                _groupName = value;
            }
        }

        public string LabelName
        {
            get
            {
                return _labelName;
            }

            set
            {
                _labelName = value;
            }
        }

        public string ObjectName
        {
            get
            {
                return _objectName;
            }
            set
            {
                _objectName = value;
            }
        }

        public string ContentName
        {
            get
            {
                return _contentName;
            }
            set
            {
                _contentName = value;
            }
        }

        public string ContentValue
        {
            get
            {
                return _contentValue;
            }
            set
            {
                _contentValue = value;
            }
        }

        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }

        public int OutputControl
        {
            get
            {
                return _outputControl;
            }

            set
            {
                _outputControl = value;
            }
        }

        public int DataField
        {
            get
            {
                return _dataField;
            }

            set
            {
                _dataField = value;
            }
        }
    }
}
