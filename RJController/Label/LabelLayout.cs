﻿using RJController.Job;
using RJPaths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RJController.Label
{
    public class LabelLayout
    {
        private List<IVariableContent> _viariableContents;

        public LabelLayout(string jobFile)
        {
            _viariableContents = new List<IVariableContent>();
            GetLabelsForJob(jobFile);
        }

        public List<IVariableContent> VariableContents
        {
            get
            {
                return _viariableContents;
            }
        }

        private void GetLabelsForJob(string jobFile)
        {
            try
            {
                using (XmlTextReader xmlTextReader = new XmlTextReader(jobFile))
                {
                    XPathNodeIterator xpathNodeIterator = new XPathDocument(xmlTextReader, XmlSpace.Preserve).CreateNavigator().Select(XMLNodes.JOB_XML_PATH_Label);
                    while (xpathNodeIterator.MoveNext())
                    {
                        string labelFileName = xpathNodeIterator.Current.GetAttribute("filename", "");
                        string groupName = xpathNodeIterator.Current.GetAttribute("group", "");

                        try
                        {
                            string pathLabelFileName = Path.GetPathRoot(jobFile) + NameFolders.LABELS + labelFileName;

                            GetObjectsFromLabel(groupName, pathLabelFileName);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
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
        }

        private void GetObjectsFromLabel(string groupName, string pathLabelFileName)
        {
            try
            {
                XElement label = XElement.Load(pathLabelFileName);
                IEnumerable<LabelObject> objects = 
                              from obj in label.Elements("Label").Elements("Layout").Elements("Object")
                              select new LabelObject()
                              {
                                  ObjectName = (string)obj.Attribute("name"),
                                  Contents = from con in obj.Elements("Content").Elements()
                                             where con.Parent.Name == "Content"
                                             select new ObjectContent()
                                             {
                                                 ContentType = con.Name.LocalName,
                                                 ContentName = (string)con.Attribute("name"),
                                                 ContentValue = (string)con.Attribute("value")
                                             }
                              };

                GetVariableContents(groupName, _viariableContents, objects);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void GetVariableContents(string groupName, List<IVariableContent> _viariableContent, IEnumerable<LabelObject> objects)
        {
            foreach (var obj in objects)
            {
                foreach (var content in obj.Contents)
                {
                    if (content.ContentType == "Variable")
                    {
                        IVariableContent vdl = new VariableContent();

                        vdl.GroupName = groupName;
                        vdl.ObjectName = obj.ObjectName;
                        vdl.ContentName = content.ContentName;
                        vdl.ContentValue = content.ContentValue;
                        vdl.OutputControl = -1;
                        vdl.DataField = -1;

                        _viariableContent.Add(vdl);
                    }
                }
            }
        } 
    }
}
