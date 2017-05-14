namespace RJController.Job
{
    public class VariableContent : IVariableContent
    {
        public string GroupName { get; set; }
        public string LabelName { get; set; }
        public string ObjectName { get; set; }
        public string ContentName { get; set; }
        public string ContentValue { get; set; }
        public string ContentType { get; set; }
        public int OutputControl { get; set; }
        public int DataField { get; set; }
    }
}
