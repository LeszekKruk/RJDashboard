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
        private string _contentType;
        private string _contentName;
        private string _contentValue;
        int _dataField;
        int _outputControl;

        public ObjectContent() { }

        public ObjectContent(string contentName, string contentValue, string contentType, int dataField, int outputControl)
        {
            if (string.IsNullOrEmpty(contentName))
            {
                throw new System.ArgumentException("Parameter can't be null");
            }
            else
            {
                _contentName = contentName;
            }

            _contentValue = contentValue;
            _contentType = contentType;
            _dataField = dataField;
            _outputControl = outputControl;
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

        public string ContentType {
            get {
                return _contentType;            
            }
            set {
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
