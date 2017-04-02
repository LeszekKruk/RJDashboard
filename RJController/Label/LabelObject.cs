using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController.Label
{
    public class LabelObject 
    {
        private string _name;
        private List<IContent> _contents;

        public LabelObject(string name)
        {
            _name = name;
            _contents = new List<IContent>();
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public void AddContent(IContent content)
        {
            _contents.Add(content);
        }

        public List<IContent> Contents {
            get
            {
                return _contents;
            }
        }

    }
}
