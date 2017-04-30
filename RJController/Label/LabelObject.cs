using System;
using System.Collections.Generic;

namespace RJController.Label
{
    public class LabelObject : ILabelObject
    {
        private string _objectName;
        private List<IObjectContent> _contents;

        public LabelObject() { }

        public LabelObject(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentException("Parameter can't be null");
            }
            else
            {
                _objectName = name;
            }
            _contents = new List<IObjectContent>();
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

        public void AddContent(IObjectContent content)
        {
            try
            {
                _contents.Add(content);
            }
            catch (Exception)
            {
                throw new System.Exception("No List<Content>");
            }            
        }

        public IEnumerable<ObjectContent> Contents { get; set; }

    }
}
