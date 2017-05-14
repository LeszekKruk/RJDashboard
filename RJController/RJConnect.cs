using ReaPiSharp;
using RJController.IO;
using RJController.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController
{
    public class RJConnect
    {
        public Version ProtocolVersion { get; set; }
        public ReaPi.ConnectionIdentifier ConnectionID { get; set; }
        public ReaPi.LabelContentHandle LabelContentHandle { get; set; }
        public RJJob Job { get; set; }
        public string IOConfiguration { get; set; }
        public IDictionary<int, DigitalOutput> Outputs { get; set; }

        public RJConnect()
        {
            ConnectionID = ReaPi.ConnectionIdentifier.INVALID_CONNECTION;
            Outputs = new Dictionary<int, DigitalOutput>();
            Job = (RJJob)null;
        }

        public RJConnect(ReaPi.ConnectionIdentifier connectionID, Version protocolVersion)
        {
            ConnectionID = connectionID;
            ProtocolVersion = protocolVersion;
            //Job = (RJJob)null;
            Job = new RJJob();
            IOConfiguration = "";
            LabelContentHandle = ReaPi.CreateLabelContent();
        } 
        
        public void AddOutput(int number, DigitalOutput output)
        {
            if (Outputs == null)
                Outputs = new Dictionary<int, DigitalOutput>();

            Outputs.Add(number, output);
        } 

        public IDictionary<int, DigitalOutput> GetOutputs()
        {
            return Outputs;
        }
    }
}
