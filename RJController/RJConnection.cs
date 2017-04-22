using ReaPiSharp;
using RJController.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController
{
    public class RJConnection
    {
        private ReaPi.ConnectionIdentifier _connectionID;
        private string _name;
        private Version _protocolVersion;
        private RJJob _actualJob;
        private string _IOConfiguration;
        private ReaPi.LabelContentHandle _labelContentHandle;

        public RJConnection()
        {
            _connectionID = ReaPi.ConnectionIdentifier.INVALID_CONNECTION;
        }

        public RJConnection(ReaPi.ConnectionIdentifier connectionID, string name, Version protocolVersion)
        {
            _connectionID = connectionID;
            _name = name;
            _protocolVersion = protocolVersion;
            _actualJob = (RJJob)null;
            _IOConfiguration = "";
            _labelContentHandle = ReaPi.CreateLabelContent();
        }

        public ReaPi.ConnectionIdentifier ConnectionID
        {
            get
            {
                return _connectionID;
            }
            set
            {
                _connectionID = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public Version ProtocolVersion
        {
            get
            {
                return _protocolVersion;
            }
            set
            {
                _protocolVersion = value;
            }
        }

        public RJJob Job
        {
            get
            {
                return _actualJob;
            }
            set
            {
                _actualJob = value;
            }
        }

        public string IOConfiguration
        {
            get
            {
                return _IOConfiguration;
            }
            set
            {
                _IOConfiguration = value;
            }
        }

        public ReaPi.LabelContentHandle LabelContentHandle
        {
            get
            {
                return _labelContentHandle;
            }
            set
            {
                _labelContentHandle = value;
            }
        }
      
    }
}
