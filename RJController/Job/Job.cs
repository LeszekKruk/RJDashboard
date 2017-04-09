using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace RJController.Job
{
    public class Job
    {
        private int _jobId;
        private string _jobFile;
        private string _installation;
        private Dictionary<string, string> _groups;

        public Job(int jobId, string jobFile)
        {
            if (jobId <= 0)
            {
                throw new System.ArgumentException("Parameter JobId must be > null");
            }
            _groups = new Dictionary<string, string>();

            JobId = jobId;
            JobFile = Path.GetFileName(jobFile);
            Installation = GetInstallation(jobFile);
            Groups = GetGroupLabels(jobFile);
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

        public Dictionary<string, string> Groups
        {
            get
            {
                return _groups;
            }
            set
            {
                _groups = value;
            }
        }       

        private static string GetInstallation(string jobFile)
        {
            string fileName = "";
            
            using (XmlTextReader xmlTextReader = new XmlTextReader(jobFile))
            {
                XPathNodeIterator xpathNodeIterator = new XPathDocument(xmlTextReader, XmlSpace.Preserve).CreateNavigator().Select(XMLNodes.JOB_XML_PATH_NODE_INSTALLATIONS);
                while (xpathNodeIterator.MoveNext())
                {
                    fileName = xpathNodeIterator.Current.GetAttribute("filename", "");
                }
            }

            return fileName;
        }

        private static Dictionary<string, string> GetGroupLabels(string jobFile)
        {
            Dictionary<string, string> groupLabel = new Dictionary<string, string>();

            using (XmlTextReader xmlTextReader = new XmlTextReader(jobFile))
            {
                XPathNodeIterator xpathNodeIterator = new XPathDocument(xmlTextReader, XmlSpace.Preserve).CreateNavigator().Select(XMLNodes.JOB_XML_PATH_Label);
                while (xpathNodeIterator.MoveNext())
                {
                    string labelFileName = xpathNodeIterator.Current.GetAttribute("filename", "");
                    string group = xpathNodeIterator.Current.GetAttribute("group", "");

                    try
                    {
                        string labelFile = Path.GetPathRoot(jobFile) + NameFolders.LABELS + labelFileName;
                        groupLabel.Add(group, labelFile);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }

            return groupLabel;
        }
    }
}
