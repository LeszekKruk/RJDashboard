using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController.Label
{
    public class Content : IContent
    {
        private string _type;
        private string _name;
        private string _value;
        private bool _directEdit;
        private bool _forceInput;
        private bool _protected;

        public Content(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public Content(string name, string value, string type)
        {
            _name = name;
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
    }
}
