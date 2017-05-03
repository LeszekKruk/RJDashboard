using System;
using System.Collections.Generic;

namespace RJController.Label
{
    public class LabelObject : ILabelObject
    {
        private List<IObjectContent> _contents;

        public string ObjectName { get; set; }

        public LabelObject() { }

        public LabelObject(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
            {
                throw new System.ArgumentException("Parameter can't be null");
            }
            else
            {
                ObjectName = objectName;
            }
            _contents = new List<IObjectContent>();
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
