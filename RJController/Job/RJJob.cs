
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using RJPaths;
using RJController.Label;
using RJController.DTO;

namespace RJController.Job
{
    public class RJJob
    {
        public int JobId { get; set; }
        public string JobFile { get; set; }
        public string Installation { get; set; }
        public List<IVariableContent> VariableContents { get; set; }

        public RJJob() { }

        public RJJob(int jobId, string jobFile)
        {
            if (jobId <= 0)
            {
                throw new System.ArgumentException("Parameter JobId musi być większy od 0.");
            }

            JobId = jobId;
            JobFile = Path.GetFileName(jobFile);
            Installation = GetInstallation(jobFile);
            VariableContents = new LabelLayout(jobFile).VariableContents;
        }

        public ISet<string> GetGroups()
        {
            ISet<string> _groups = new HashSet<string>();
            foreach (var item in VariableContents)
            {
                _groups.Add(item.GroupName);
            }
            return _groups;
        }

        public void ClearDataFields()
        {
            foreach (var item in VariableContents)
            {
                item.DataField = -1;
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

        public void AssignSettings(List<DTOLabelSettings> dto, int maxFields)
        {
            for (int i = 0; i < VariableContents.Count; i++)
            {
                for (int j = 0; j < dto.Count; j++)
                {
                    if (VariableContents[i].GroupName == dto[i].GroupName && 
                        VariableContents[i].ObjectName == dto[i].ObjectName && 
                        VariableContents[i].ContentName == dto[i].ContentName)
                    {
                        //sprawdzam czy kolumna wykracza poza zakres bazy danych
                        if (dto[i].DataField <= maxFields)
                        {
                            VariableContents[i].DataField = dto[i].DataField;
                        }
                        else
                        {
                            VariableContents[i].DataField = -1;
                        }
                        //sprawdzam czy zakres wyjsc miesci sie pomiedzy 0 - 7
                        if (dto[i].OutputControl >= 0 && dto[i].OutputControl < 8)
                        {
                            VariableContents[i].OutputControl = dto[i].OutputControl;
                        }
                        else
                        {
                            VariableContents[i].OutputControl = -1;
                        }        
                    }
                }
            }
        }

        public List<IVariableContent> GetVariableContents()
        {
            return VariableContents;
        }
    }
}
