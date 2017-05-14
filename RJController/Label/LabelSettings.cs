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
        public void Test()
        {

        }

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
                        string labelName = xpathNodeIterator.Current.GetAttribute("labelName", "");
                        string objectName = xpathNodeIterator.Current.GetAttribute("objectName", "");
                        string contentName = xpathNodeIterator.Current.GetAttribute("contentName", "");
                        int.TryParse(xpathNodeIterator.Current.GetAttribute("outputControl", ""), out outputControl);
                        int.TryParse(xpathNodeIterator.Current.GetAttribute("dataField", ""), out dataField);

                        DTOLabelSettings dto = new DTOLabelSettings(groupName, labelName, objectName, contentName, outputControl, dataField);
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
    }
}
