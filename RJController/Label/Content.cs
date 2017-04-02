using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController.Label
{
    public class Content : IContent
    {
        private ContentType _type;
        private string _name;
        private string _value;
        private bool _directEdit;
        private bool _forceInput;
        private bool _protected;

        public Content(string name, string value)
        {
            _name = name;
            _value = value;
            _type = ContentType.NotImplemented;
        }

        public Content(string name, string value, ContentType type)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentException("Parameter can't be null");
            }
            else
            {
                _name = name;
            }
            
            _value = value;
            _type = type;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public ContentType ContentType {
            get {
                return _type;            
            }
            set {
                _type = value;
            }
        }
    }

    public enum ContentType
    {
        NotImplemented = -1,
        Static = 0,
        Variable = 1 
    }
}
