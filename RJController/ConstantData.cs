using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController
{
    public struct XMLNodes
    {
        public const string JOB_XML_PATH_NODE_INSTALLATIONS = "//REA-JET/Job/Installation";
        public const string JOB_XML_PATH_Label = "//REA-JET/Job/Labels/Label";
    }

    public struct NameFolders
    {
        public const string LABELS = "\\labels\\";
        public const string INSTALLATIONS = "\\installations\\";
        public const string JOBS = "\\jobs\\";
    }
}
