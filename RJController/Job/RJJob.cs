
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using RJPaths;
using RJController.Label;

namespace RJController.Job
{
    public class RJJob
    {
        private int _jobId;
        private string _jobFile;
        private string _installation;
        private List<IVariableDataLabel> _variableDataLabel;

        public RJJob(int jobId, string jobFile)
        {
            if (jobId <= 0)
            {
                throw new System.ArgumentException("Parameter JobId must be > null");
            }

            JobId = jobId;
            JobFile = Path.GetFileName(jobFile);
            Installation = GetInstallation(jobFile);

            LabelLayout ll = new LabelLayout(jobFile);
            LabelManagement = ll.VariableObjects;

        }

        public int JobId {
            get {
                return _jobId;
            }
            set {
                _jobId = value;
            }
        }

        public string JobFile {
            get
            {
                return _jobFile;
            }
            set
            {
                _jobFile = value;
            }
        }

        public string Installation
        {
            get
            {
                return _installation;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new System.ArgumentException("Parameter can't be null");
                }
                else
                {
                    _installation = value;
                }                
            }
        }

        public List<IVariableDataLabel> LabelManagement
        {
            get
            {
                return _variableDataLabel;
            }

            set
            {
                _variableDataLabel = value;
            }
        }

        private static string GetInstallation(string jobFile)
        {
            string fileName = "";
            try
            {
                using (XmlTextReader xmlTextReader = new XmlTextReader(jobFile))
                {
                    XPathNodeIterator xpathNodeIterator = new XPathDocument(xmlTextReader, XmlSpace.Preserve).CreateNavigator().Select(XMLNodes.JOB_XML_PATH_NODE_INSTALLATIONS);
                    while (xpathNodeIterator.MoveNext())
                    {
                        fileName = xpathNodeIterator.Current.GetAttribute("filename", "");
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("File not found");
            }
            catch (Exception)
            {
                throw;
            }

            return fileName;
        }
    }
}
