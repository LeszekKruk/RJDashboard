//namespace RJController
namespace RJPaths
{
    public struct XMLNodes
    {
        static public string JOB_XML_PATH_NODE_INSTALLATIONS = "//REA-JET/Job/Installation";
        static public string JOB_XML_PATH_Label = "//REA-JET/Job/Labels/Label";
        static public string LABEL_XML_PATH_Layout = "//REA-JET/Label/Layout";
        static public string LABEL_XML_PATH_Object = "//REA-JET/Label/Layout/Object";
        static public string LABEL_SETTINGS = "//LAYOUT/OBJECT";
    }

    public struct NameFolders
    {
        public const string LABELS = "\\labels\\";
        public const string INSTALLATIONS = "\\installations\\";
        public const string JOBS = "\\jobs\\";
    }
}
