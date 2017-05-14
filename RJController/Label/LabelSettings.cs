using RJController.DTO;
using RJPaths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace RJController.Label
{
    public class LabelSettings
    {
        public List<DTOLabelSettings> Load(string filePath)
        {
            try
            {
                List<DTOLabelSettings>  _listDTO = new List<DTOLabelSettings>();
                using (XmlTextReader xmlTextReader = new XmlTextReader(filePath))
                {
                    
                    XPathNodeIterator xpathNodeIterator = new XPathDocument(xmlTextReader, XmlSpace.Preserve).CreateNavigator().Select(XMLNodes.LABEL_SETTINGS);
                    while (xpathNodeIterator.MoveNext())
                    {
                        int outputControl;
                        int dataField;
                        string groupName = xpathNodeIterator.Current.GetAttribute("groupName", "");
                        //string labelName = xpathNodeIterator.Current.GetAttribute("labelName", "");
                        string objectName = xpathNodeIterator.Current.GetAttribute("objectName", "");
                        string contentName = xpathNodeIterator.Current.GetAttribute("contentName", "");
                        int.TryParse(xpathNodeIterator.Current.GetAttribute("outputControl", ""), out outputControl);
                        int.TryParse(xpathNodeIterator.Current.GetAttribute("dataField", ""), out dataField);

                        DTOLabelSettings dto = new DTOLabelSettings(groupName, "", objectName, contentName, outputControl, dataField);
                        _listDTO.Add(dto);
                    }
                }
                return _listDTO;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("File not found");
            }
            catch (Exception)
            {
                throw new Exception("Nieprawidłowa struktura pliku z ustawieniami etykiety.");
            }
        }

        public void Save(Stream labelFile, string databaseFile, string jobFile, List<DTOLabelSettings> labelSettings)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement xmlLayout = doc.CreateElement(string.Empty, "LAYOUT", string.Empty);
            doc.AppendChild(xmlLayout);

            XmlElement metaCSVFileName = doc.CreateElement("meta");
            metaCSVFileName.SetAttribute("databaseFile", databaseFile);
            doc.DocumentElement.AppendChild(metaCSVFileName);

            XmlElement metaJobFile = doc.CreateElement("meta");
            metaJobFile.SetAttribute("jobFile", jobFile);
            doc.DocumentElement.AppendChild(metaJobFile);

            for (int i = 0; i < labelSettings.Count; i++)
            {
                XmlElement xmlObject = doc.CreateElement("OBJECT");

                xmlObject.SetAttribute("groupName", labelSettings[i].GroupName);
                xmlObject.SetAttribute("objectName", labelSettings[i].ObjectName);
                xmlObject.SetAttribute("contentName", labelSettings[i].ContentName);
                xmlObject.SetAttribute("dataField", labelSettings[i].DataField.ToString());
                xmlObject.SetAttribute("outputControl", labelSettings[i].OutputControl.ToString());

                doc.DocumentElement.AppendChild(xmlObject);
            }

            doc.Save(labelFile);
        }
    }
}
