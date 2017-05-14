using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController.Model.Database
{
    public class Database
    {
        private double _actualRecord;
        private double _previousRecord;
        private double _maxRecords;
        private List<Header> _headers;
        //private List<Output> _outputs;
        private Dictionary<double, List<string>> _records;

        public Database()
        {
            _previousRecord = 0;
            _actualRecord = 1;
            _maxRecords = 0;
            _headers = new List<Header>();
            //_outputs = new List<Output>();
            _records = new Dictionary<double, List<string>>();
        }

        public List<string> GetRecordWithKey(double keyCSVDataRecord)
        {
            List<string> record;
            _records.TryGetValue(keyCSVDataRecord, out record);

            return record;
        }

        public List<string> GetActualRecord()
        {
            List<string> record;
            _records.TryGetValue(_actualRecord, out record);

            return record;
        }

        public void AddRecord(List<string> data)
        {
            _maxRecords += 1;
            _records.Add(_maxRecords, data);
        }

        public void ClearHeaders()
        {
            try
            {
                _headers.Clear();
            }
            catch (Exception) { }
        }

        public bool ClearDatabase()
        {
            try
            {
                ClearHeaders();
                _records.Clear();
                _previousRecord = 0;
                _actualRecord = 1;
                _maxRecords = 0;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void AddHeaders(List<string> headerName)
        {
            for (int i = 0; i < headerName.Count; i++)
            {
                _headers.Add(new Header(i, headerName[i]));
            }
        }

        public List<Header> GetHeaders()
        {
            return _headers;
        }

        public string GetHeaderName(int number)
        {
            try
            {
                return _headers[number].Name;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public int GetHeadersCount()
        {
            return _headers.Count;
        }

        public double ActualRecord
        {
            get
            {
                return _actualRecord;
            }

            set
            {
                if (value <= _maxRecords)
                {
                    _actualRecord = value;
                } else
                {
                    throw new Exception("Aktualny rekord nie może być większy od ilości rekordów.");
                }
            }
        }

        public double PreviousRecord
        {
            get
            {
                return _previousRecord;
            }

            set
            {
                _previousRecord = value;
            }
        }

        public double SetNextRecord()
        {
            PreviousRecord = _actualRecord;
            _actualRecord += 1;

            return _actualRecord;
        }

        public bool IsNextRecord()
        {
            if (ActualRecord + 1 <= _maxRecords)
                return true;

            return false;
        }

        public bool RemoveData()
        {
            bool success = false;

            try
            {
                _maxRecords = 0;
                _actualRecord = 0;
                _headers.Clear();
                _records.Clear();

                success = true;
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        public double MaxRecords()
        {
            return _maxRecords;
        }
    }
}
