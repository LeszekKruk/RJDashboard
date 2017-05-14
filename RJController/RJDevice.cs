using ReaPiSharp;
using RJController.Dashboard;
using RJController.DTO;
using RJController.Enums;
using RJController.IO;
using RJController.Job;
using RJController.Label;
using RJController.Model.Database;
using RJLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace RJController
{
    public class RJDevice
    {
        static private ReaPi.connectionCallbackPtr _connectionCallback;
        static private ReaPi.responseCallbackPtr _responseCallback;
        static private ReaPi.eventCallbackPtr _eventCallback;

        private Queue<Record> _queueRecords = new Queue<Record>();
        private int MAX_DIGITAL_OUTPUTS = 8;

        IDashboard _view;
        private bool isPrintTrigger = true;
        private bool invalidContentStopJob = true;
        private bool printRejectStopJob = true;
        private bool printAbortedStopJob = true;
        private bool printSpeedErrorStopJob = true;
        private bool missingContentStopJob = true;
        private bool bufferFullStopJob = true;
        private bool showResponseWithStatusOK = false;

        private bool _stopLine;

        private RJConnect rjConnection;
        private Database database;

        public RJDevice(IDashboard view)
        {
            _view = view;
            database = new Database();        

            if (CheckReaPiLibrary() == true)
            {
                
                //rjConnection = new RJConnect((ReaPi.ConnectionIdentifier) 1, null);
                rjConnection = new RJConnect();

                //test API
                rjConnection.IOConfiguration = "osb_hr_1.dio";

                RegisterConnection();
                SetDigitalOutputsProperty();
            }
            else
            {
                _view.ShowProblemSolution(ErrorType.libraryError);
            }        
        }

        private void OnConnectionCallback(
            ReaPi.ConnectionIdentifier connectionId,
            ReaPi.EConnState state,
            ReaPi.EErrorCode errorCode,
            IntPtr context)
        {
            if (state == ReaPi.EConnState.CONNECT)
            {
                if (connectionId > 0)
                {
                    if (errorCode != ReaPi.EErrorCode.OK)
                    {
                        SendToDashboard(MessageType.ERROR, "Błąd podczas połączenia ze sterownikiem!", errorCode.ToString(), "OnConnectionCallback");
                        return;
                    }

                    rjConnection = new RJConnect(connectionId, null);

                    _responseCallback = new ReaPi.responseCallbackPtr(OnResponseCallback);
                    ReaPi.RegisterResponseCallback(connectionId, _responseCallback, context);

                    _eventCallback = new ReaPi.eventCallbackPtr(OnEventCallback);
                    ReaPi.RegisterEventCallback(connectionId, _eventCallback, context);

                    OnConnect(connectionId);
                }
                else
                {
                    SendToDashboard(MessageType.ERROR, $"Nieprawidłowy IdConnection: <{connectionId}>.", null, "OnConnectionCallback");
                    _view.ShowProblemSolution(ErrorType.errorConnection);
                }
            }
            else if (state == ReaPi.EConnState.DISCONNECT)
            {
                if (errorCode != ReaPi.EErrorCode.OK)
                    SendToDashboard(MessageType.ERROR, "Błąd podczas rozłączenia ze sterownikiem!", errorCode.ToString(), "OnConnectionCallback");

                OnDisconnect(connectionId);
            }
            else if (state == ReaPi.EConnState.CONNECTIONERROR)
            {
                SendToDashboard(MessageType.ERROR, "Błąd podczas połączenia <" + connectionId + ">.", errorCode.ToString(), "OnConnectionCallback");
                UpdateDashboard(EventType.DISCONNECT);

                _view.ShowProblemSolution(ErrorType.errorConnection);
            }
        }

        private void OnEventCallback(
            ReaPi.ResponseHandle response, 
            ReaPi.ConnectionIdentifier connection, 
            ReaPi.EEventId eventId, 
            IntPtr context)
        {
            int error = 0;

            SendToDashboard(MessageType.EVENT, $"{eventId.ToString()}", null, null);

            switch (eventId)
            {
                case ReaPi.EEventId.JOBSET:
                    OnJobSetEvent(connection, ReaPi.GetJobId(response, out error), ReaPi.GetJobFilename(response, out error));
                    break;
                case ReaPi.EEventId.JOBSTARTED:
                    OnJobStartedEvent();
                    break;
                case ReaPi.EEventId.JOBSTOPPED:
                    OnJobStoppedEvent();
                    break;
                case ReaPi.EEventId.PRINTTRIGGER:
                    OnJobPrintTriggerEvent(eventId);
                    break;
                case ReaPi.EEventId.PRINTSTART:
                    OnJobPrintStartEvent(eventId);
                    break;
                case ReaPi.EEventId.PRINTREJECTED:
                    OnJobPrintRejectEvent();
                    break;
                case ReaPi.EEventId.PRINTEND:
                    OnJobPrintEndEvent();
                    break;
                case ReaPi.EEventId.PRINTABORTED:
                    OnJobPrintAbortedEvent();
                    break;
                case ReaPi.EEventId.PRINTSPEEDERROR:
                    OnJobPrintSpeedErrorEvent();
                    break;
                case ReaPi.EEventId.INVALIDCONTENT:
                    OnInvalidContentEvent(ReaPi.GetGroupname(response, out error));
                    break;
                case ReaPi.EEventId.IOCONFIGURATIONSET:
                    OnIOConfigurationSetEvent(ReaPi.GetIOConfigurationFilename(response, out error));
                    break;
                case ReaPi.EEventId.MISSINGCONTENT:
                    OnMissingContentEvent(ReaPi.GetGroupname(response, out error));
                    break;
                case ReaPi.EEventId.BUFFERFULL:
                    OnJobBufferFullEvent(ReaPi.GetJobErrorStatus(response, out error));
                    break;
                default:
                    break;
            }
        }

        private void OnResponseCallback(
            ReaPi.ResponseHandle response, 
            ReaPi.ConnectionIdentifier connection, 
            ReaPi.ECommandId commandid, 
            ReaPi.EErrorCode errorCode, 
            IntPtr context)
        {
            int error = 0;
            ErrorStatus errorStatus = ReaPi.GetErrorStatus(response, out error);
            switch (commandid)
            {
                case ReaPi.ECommandId.CMD_SUBSCRIBEJOBSET:
                    CmdSubscribeJobSetResponse(true);
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEJOBSET:
                    CmdSubscribeJobSetResponse(false);
                    break;
                case ReaPi.ECommandId.CMD_GETIOCONFIGURATION:
                    CmdGetIOConfigurationResponse(ReaPi.GetIOConfigurationFilename(response, out error));
                    break;
                case ReaPi.ECommandId.CMD_GETIOOUTPUTLEVEL:
                    CmdGetOutputLevelResponse(response, new GetIOOutputLevelResponseEventArgs(connection, response, commandid, ReaPi.GetErrorStatus(response, out error)));
                    break;
                case ReaPi.ECommandId.CMD_SETIOOUTPUTLEVEL:
                    CmdSetOutputLevelResponse();
                    break;
                default:
                    break;
            }
            ShowResponseError(response, connection, commandid.ToString(), errorCode);
        }

        #region PRIVATE Methods

        private void RegisterConnection()
        {
            _connectionCallback = new ReaPi.connectionCallbackPtr(OnConnectionCallback);
            ReaPi.EErrorCode tmpError = ReaPi.RegisterConnectionCallback(_connectionCallback, new IntPtr(0));

            if (tmpError == ReaPi.EErrorCode.OK)
            {
                SendToDashboard(MessageType.LOG, "RegisterConnectionCallback() - Status.OK", "", "");
            }
            else
            {
                SendToDashboard(MessageType.LOG, "RegisterConnectionCallback() - Status.Error", tmpError.ToString(), "");
            }
        }

        private void OnConnect(ReaPi.ConnectionIdentifier connectionId)
        {
            string lastError = "";
            rjConnection.ProtocolVersion = new Version(ReaPi.GetProtocolVersion(rjConnection.ConnectionID));

            lastError = ReaPi.SubscribeJobSet(rjConnection.ConnectionID, 1).ToString();
            lastError = ReaPi.SubscribeIOConfigurationSet(rjConnection.ConnectionID, 1).ToString();
            lastError = ReaPi.GetIOOutputLevel(rjConnection.ConnectionID).ToString();

            //SetOutputLevel(OutputTypeReaction.ControlDivert, OutputLevel.Low);
            //SetOutputLevel(OutputTypeReaction.ControlStacker, OutputLevel.Low);

            SendToDashboard(MessageType.LOG, "Połączenie: OK", $" Wskaźnik połączenia: {rjConnection.ConnectionID.ToString()}.", null);
            SendToDashboard(MessageType.ERROR, lastError, null, null);
            UpdateDashboard(EventType.CONNECT);
        }

        private void OnDisconnect(ReaPi.ConnectionIdentifier connectionId)
        {
            rjConnection.ConnectionID = ReaPi.ConnectionIdentifier.INVALID_CONNECTION;

            SendToDashboard(MessageType.LOG, $"Połączenie <{connectionId}> zamknięte.", null, null);
            UpdateDashboard(EventType.DISCONNECT);
        }

        private void SendToDashboard(MessageType type, string log, string errorCode, string errorMessage)
        {
            switch (type)
            {
                case MessageType.EVENT:
                    _view.DisplayEvents(log);
                    break;
                case MessageType.LOG:
                    _view.DisplayLogsAndErrors("Log: " + log, errorCode, errorMessage);
                    break;
                case MessageType.ERROR:
                    _view.DisplayLogsAndErrors("Error: " + log, errorCode, errorMessage);
                    break;
                default:
                    break;
            }
        }

        private void UpdateDashboard(EventType rjEvent)
        {
            _view.UpdateDashboard(rjEvent);
        }

        private void ShowProblemSolution(ErrorType type)
        {
            _view.ShowProblemSolution(type);
        }

        private bool CheckReaPiLibrary()
        {
            string log = "";
            bool library = true;
            try
            {

                log = "Informacja o bibliotece ReaPi.dll" + Environment.NewLine;
                log += "ReaPi Info: " + ReaPi.GetLibInfo() + Environment.NewLine;
                log += "Revision: " + ReaPi.GetRevision();
            }
            catch (Exception e)
            {
                log = "Brak bibliotek ReaPi.dll";
                AppLogger.GetLogger().Fatal(log, e);

                library = false;
            }
            finally
            {
                SendToDashboard(MessageType.LOG, log, null, null);
            }

            return library;
        }

        private void PrepareNextRecord(ReaPi.EEventId eventId)
        {
            if ((isPrintTrigger == true && eventId == ReaPi.EEventId.PRINTTRIGGER) || (isPrintTrigger == false && eventId == ReaPi.EEventId.PRINTSTART))
            {
                SendRecordToPrint(database.ActualRecord + 1);
            }
        }

        private void SendRecordToPrint(double actualRecord)
        {
            _stopLine = false;

            database.PreviousRecord = database.ActualRecord;
            database.ActualRecord = actualRecord;

            if (actualRecord > database.MaxRecords())
            {
                _stopLine = true;
                return;
            }

            List<string> valueContent = GetContentToPrint();

            List <DTOVariableContent> labelContents = new List<DTOVariableContent>();

            for (int i = 0; i < rjConnection.Job.VariableContents.Count; i++)
            {
                DTOVariableContent dtoVC = new DTOVariableContent(
                    rjConnection.Job.VariableContents[i].GroupName,
                    rjConnection.Job.VariableContents[i].ObjectName,
                    rjConnection.Job.VariableContents[i].ContentName,
                    valueContent[i]
                    );
                labelContents.Add(dtoVC);
            }
            SetLabelContent(labelContents);
        }

        private List<string> GetContentToPrint()
        {
            List<string> csvContent = new List<string>();
            List<string> record = database.GetActualRecord();

            for (int i = 0; i < rjConnection.Job.VariableContents.Count(); i++)
            {
                int columnNumber = rjConnection.Job.VariableContents[i].DataField;
                if (columnNumber == -1)
                {
                    csvContent.Add("");
                }
                else
                {
                    csvContent.Add(record[columnNumber]);
                }

            }

            return csvContent;
        }

        private void SetLabelContent(List<DTOVariableContent> dtoVC)
        {
            try
            {
                if (rjConnection.ConnectionID <= 0)
                    return;

                ReaPi.EErrorCode error = ReaPi.RemoveLabelContent(rjConnection.LabelContentHandle);
                ReaPi.LabelContentHandle labelContentHandle = ReaPi.CreateLabelContent();

                rjConnection.LabelContentHandle = labelContentHandle;

                if (labelContentHandle >= (ReaPi.LabelContentHandle)0)
                {
                    foreach (var lp in dtoVC)
                    {
                        error = ReaPi.PrepareLabelContent(labelContentHandle, 1, lp.GroupName, lp.ObjectName, lp.ContentName, lp.ContentValue);

                        if (error != ReaPi.EErrorCode.OK)
                            SendToDashboard(MessageType.LOG, "Error: Nie można przygotować danych dla pola: {lp.ContentName}", error.ToString(), null);
                    }

                    ReaPi.ResponseHandle response = ReaPi.SetLabelContent(rjConnection.ConnectionID, labelContentHandle);

                    if (response < 0)
                    {
                        SendToDashboard(MessageType.LOG, "Error: Nie można wysłać danych dla etykiety", null, null);
                    }
                    else
                    {
                        SendToDashboard(MessageType.LOG, $"Wysłano zawartość rekordu: {database.ActualRecord.ToString()}", null, null);

                        Record record = new Record();
                        record.Id = database.ActualRecord;
                        record.IsError = false;

                        _queueRecords.Enqueue(record);
                    }
                }
                else
                {
                    SendToDashboard(MessageType.LOG, "Error: Nie można przygotować danych dla etykiety", null, null);
                }
            }
            catch (FormatException ex)
            {
                SendToDashboard(MessageType.ERROR, "Function SetLabelContent()", ex.ToString(), ex.Message.ToString());
            }
        }

        private void PrepareVariablesForStartPtintingNewRecord()
        {
            _stopLine = false;
            _queueRecords.Clear();
            database.PreviousRecord = 0;
            database.ActualRecord = 1;
        }

        private void SetDigitalOutputsProperty()
        {
            string description = "";
            for (int i = 0; i < MAX_DIGITAL_OUTPUTS; i++)
            {
                switch (i)
                {
                    case 0:
                        description = "Status zadania";
                        break;
                    case 1:
                        description = "Błąd drukarki";
                        break;
                    case 2:
                        description = "Sterowanie sztaplarką";
                        break;
                    case 3:
                        description = "Sterowanie odrzutnikiem";
                        break;
                    default:
                        description = "";
                        break;
                }
                DigitalOutput output = new DigitalOutput();
                output.Id = i + 1;
                //output.OutputName = "Out " + i.ToString();
                output.Name = ((DOutputControl)i).ToString();
                output.Description = description;
                if (i < 4)
                {
                    output.IsActive = true;
                }
                else
                {
                    output.IsActive = false;
                }

                output.DataField = -1;

                rjConnection.AddOutput(output);
            }
        }

        private void SetDigitalOutputLevel(DOutputControl outputType, DigitalOutputLevel level)
        {
            int outputNumber = (int)outputType;
            int outputMask = (int)Math.Pow(2, outputNumber);

            RJSetOutputLevel(outputMask, (int)level);
        }

        private DigitalOutputLevel GetOutputLevelToBeSetForDataField(DOutputControl outputType, double printedRecord)
        {
            DigitalOutputLevel level = DigitalOutputLevel.Low;
            int outputNumber = (int)outputType;

            try
            {
                int columnNumber = rjConnection.Outputs[outputNumber].DataField;

                if (columnNumber != -1)
                {
                    List<string> record = database.GetRecordWithKey(printedRecord);
                    string columnValue = record[columnNumber];

                    if (columnValue != "" && columnValue != null)
                        level = DigitalOutputLevel.High;
                }
            }
            catch (Exception) { }

            return level;
        }

        #endregion

        #region OnEvent Methods

        private void OnJobSetEvent(ReaPi.ConnectionIdentifier connection, int job, string jobFileName)
        {
            SendToDashboard(MessageType.EVENT, $"Job: <{jobFileName}>", null, null);

            if (!string.IsNullOrEmpty(jobFileName))
            {
                try
                {
                    rjConnection.Job = new RJJob(job, jobFileName);

                    if (rjConnection.Job.VariableContents.Count > 0)
                    {
                        ISet<string> groups = rjConnection.Job.GetGroups();
                        //subskrypcja dla każdej grupy
                        foreach (var group in groups)
                        {
                            ReaPi.SubscribeInvalidContent(connection, job, group).ToString();
                            ReaPi.SubscribeBufferFull(connection, job, group).ToString();
                        }
                    }
                }
                catch (Exception) { }

                string logError;

                logError = ReaPi.SubscribeJobStarted(connection, job).ToString();
                logError += ReaPi.SubscribeJobStopped(connection, job).ToString();
                logError += ReaPi.SubscribeIOConfigurationSet(connection, job).ToString();
                logError += ReaPi.SubscribePrintTrigger(connection, job).ToString();
                logError += ReaPi.SubscribePrintStart(connection, job).ToString();
                logError += ReaPi.SubscribePrintRejected(connection, job).ToString();
                logError += ReaPi.SubscribePrintEnd(connection, job).ToString();
                logError += ReaPi.SubscribePrintAborted(connection, job).ToString();
                logError += ReaPi.SubscribePrintSpeedError(connection, job).ToString();
                logError += ReaPi.SubscribeMissingContent(connection, job).ToString();

                SendToDashboard(MessageType.ERROR, logError, "", "");
            }
            else
            {
                ShowProblemSolution(ErrorType.nullJob);                
            }
        }

        private void OnJobStartedEvent()
        {
            UpdateDashboard(EventType.JOBSTARTED);
            SetDigitalOutputLevel(DOutputControl.JOB_STATUS, DigitalOutputLevel.High);
        }

        private void OnJobStoppedEvent()
        {
            UpdateDashboard(EventType.JOBSTOPPED);
            SetDigitalOutputLevel(DOutputControl.JOB_STATUS, DigitalOutputLevel.Low);
        }

        private void OnJobBufferFullEvent(ErrorStatus errorStatus)
        {
            try
            {
                Record record = _queueRecords.Last();
                SendToDashboard(MessageType.ERROR, "Przepełniony bufor", errorStatus.Domain.ToString() + " / " + errorStatus.Code.ToString(), errorStatus.Message);
            }
            catch (Exception) { }
            finally
            {
                if (bufferFullStopJob == true)
                {
                    RJStopJob();
                    ShowProblemSolution(ErrorType.bufferFull);
                }
            }
        }

        private void OnIOConfigurationSetEvent(string ioFileName)
        {
            rjConnection.IOConfiguration = ioFileName;

            SendToDashboard(MessageType.EVENT, $"I/O: <{ioFileName}>", null, null);
            UpdateDashboard(EventType.IOSET);
        }

        private void OnInvalidContentEvent(string groupName)
        {
            SendToDashboard(MessageType.EVENT, $" Grupa: <{groupName}>", null, null);
            if (invalidContentStopJob == true)
            {
                RJStopJob();
                ShowProblemSolution(ErrorType.invalidContent);
            }
        }

        private void OnMissingContentEvent(string groupName)
        {
            SendToDashboard(MessageType.EVENT, $" Grupa: <{groupName}>", null, null);
            if (missingContentStopJob == true)
            {
                RJStopJob();
                ShowProblemSolution(ErrorType.missingContent);
            }
        }

        private void OnJobPrintTriggerEvent(ReaPi.EEventId eventId)
        {
            PrepareNextRecord(eventId);
        }

        private void OnJobPrintStartEvent(ReaPi.EEventId eventId)
        {
            UpdateDashboard(EventType.PRINTSTARTED);

            if (_queueRecords.Count > 0)
            {
                Record record = _queueRecords.First();
                SendToDashboard(MessageType.EVENT, $" Wiersz: <{record.Id.ToString()}>", null, null);

                if (record.IsError == true)
                {
                    SendToDashboard(MessageType.EVENT, " Zatrzymany wydruk", null, null);
                    RJStopJob();
                    ShowProblemSolution(ErrorType.errorRecordToPrint);
                }
                //nowy kod do sprawdzenia
                PrepareNextRecord(eventId);
            }
            else
            {
                SendToDashboard(MessageType.ERROR, "Brak danych do wydruku", null, null);
                RJStopJob();
                ShowProblemSolution(ErrorType.noRecordToPrint);
            }
        }

        private void OnJobPrintRejectEvent()
        {
            if (printRejectStopJob == true)
            {
                RJStopJob();
                ShowProblemSolution(ErrorType.printReject);
            }
        }

        private void OnJobPrintEndEvent()
        {
            try
            {
                Record record = _queueRecords.First();

                DigitalOutputLevel level = GetOutputLevelToBeSetForDataField(DOutputControl.SET_STACKER, record.Id);
                SetDigitalOutputLevel(DOutputControl.SET_STACKER, level);

                _queueRecords.Dequeue();
            }
            catch (Exception) { }

            if (_stopLine == true)
            {
                SendToDashboard(MessageType.LOG, "Koniec bazy", $"Wydrukowano ostatni rekord: {DateTime.Now.ToString()}", null);
                RJStopJob();
                ShowProblemSolution(ErrorType.printedLastRecord);
            }
        }

        private void OnJobPrintAbortedEvent()
        {
            try
            {
                Record record = _queueRecords.Last();
                SendToDashboard(MessageType.EVENT, $" Wiersz: <{record.Id.ToString()}>", null, null);
            }
            catch (Exception) { }
            finally
            {
                if (printAbortedStopJob == true)
                {
                    RJStopJob();
                    ShowProblemSolution(ErrorType.printAborted);
                }
            }
        }

        private void OnJobPrintSpeedErrorEvent()
        {
            try
            {
                Record record = _queueRecords.Last();
                SendToDashboard(MessageType.EVENT, $" Wiersz: <{record.Id.ToString()}>", null, null);
            }
            catch (Exception) { }
            finally
            {
                if (printSpeedErrorStopJob == true)
                {
                    RJStopJob();
                    ShowProblemSolution(ErrorType.printSpeedError);
                }
            }
        }

        #endregion

        #region OnResponse Methods

        private void ShowResponseError(
            ReaPi.ResponseHandle response,
            ReaPi.ConnectionIdentifier connectionId,
            string command,
            ReaPi.EErrorCode error)
        {
            if (error == ReaPi.EErrorCode.OK)
            {
                if (showResponseWithStatusOK == true)
                    SendToDashboard(MessageType.EVENT,$"{command}", null, null);
            }
            else
            {
                int err = 0;
                string lastError = command + ", Błąd: " + error;
                string errorMessage = ReaPi.GetErrorMessage(response, out err);
                string errorDomain = ReaPi.GetErrorDomain(response, out err).ToString();
                string errorCode = ReaPi.GetErrorCode(response, out err).ToString();

                SendToDashboard(MessageType.ERROR, lastError, errorCode + " / " + errorDomain, errorMessage);
            }
        }


        private void CmdGetIOConfigurationResponse(string ioFileName)
        {
            SendToDashboard(MessageType.LOG, "IO: " + ioFileName, null, null);
        }

        private void CmdSubscribeJobSetResponse(bool subscribed)
        {
            SendToDashboard(MessageType.LOG, "Subscribtion JobSet", "Event successful " + (subscribed ? "subscribed" : "unsubscribed"), null);
        }

        private void CmdSetOutputLevelResponse()
        {
            //aktualizuje stan wyjsc
            RJGetOutputLevel();
        }

        private void CmdGetOutputLevelResponse(ReaPi.ResponseHandle response, GetIOOutputLevelResponseEventArgs getIOOutputLevelResponseEventArgs)
        {
            int error = 0;
            List<GetIOOutputLevelResponseEventArgs.IOOutput> listIOOutputs = new List<GetIOOutputLevelResponseEventArgs.IOOutput>();
            int count = ReaPi.GetNumberOfOutputs(response, out error);
            for (int i = 1; i <= count; i++)
            {
                GetIOOutputLevelResponseEventArgs.IOOutput ioOutput = new GetIOOutputLevelResponseEventArgs.IOOutput();
                ioOutput.Index = i;
                ioOutput.Level = ReaPi.GetIOOutputLevelValue(response, i, out error);

                listIOOutputs.Add(ioOutput);
            }

            List<bool> outputsState = new List<bool>();
            for (int i = 0; i < getIOOutputLevelResponseEventArgs.IOOutputs.Count; i++)
            {
                outputsState.Add((bool)getIOOutputLevelResponseEventArgs.IOOutputs[i].Level);
            }
            try
            {
                _view.UpdateOutputsState(outputsState);
            }
            catch (Exception) { }
        }
        #endregion

        #region PUBLIC Methods
        public void SetPrinterControlParameters(bool printTrigger, bool invalidContent, bool printReject, bool printAborted, bool printSpeedError, bool missingContent, bool bufferFull)
        {
            isPrintTrigger = printTrigger;
            invalidContentStopJob = invalidContent;
            printRejectStopJob = printReject;
            printAbortedStopJob = printAborted;
            printSpeedErrorStopJob = printSpeedError;
            missingContentStopJob = missingContent;
            bufferFullStopJob = bufferFull;
        }

        public void ManualSendRecordToPrint(double actualRecord)
        {
            PrepareVariablesForStartPtintingNewRecord();

            SendRecordToPrint(actualRecord);
        }

        public string[] GetJobFiles()
        {
            try
            {
                //string[] files = Directory.GetFiles("\\\\" + _ipAddress + "\\rea-jet\\jobs");
                string[] files = Directory.GetFiles("C:\\temp\\_ReaTest\\jobs");
                return files;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string[] GetIOFiles()
        {
            try
            {
                //string[] files = Directory.GetFiles("\\\\" + _ipAddress + "\\rea-jet\\device");
                string[] files = Directory.GetFiles("C:\\temp\\_ReaTest\\device");
                return files;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void RJConnect(string ipAddress)
        {
            if (rjConnection == null || rjConnection.ConnectionID <= 0)
            {
                try
                {
                    ReaPi.Connect("TCP://" + ipAddress);
                }
                catch (Exception e) {
                    AppLogger.GetLogger().Error($"Błąd połączenia z IP: {ipAddress}", e);
                }
            }            
        }

        public void RJSetJob(string jobName)
        {
            OnJobSetEvent((ReaPi.ConnectionIdentifier)1, 1, $"C:\\temp\\_ReaTest\\jobs\\{jobName}");
            UpdateDashboard(EventType.JOBSET);

            if (rjConnection.ConnectionID <= 0)
                return;

            //ReaPi.SetJob(rjConnection.ConnectionID, 1, jobName);
        }

        public void RJClearJob()
        {
            if (rjConnection.ConnectionID <= 0)
                return;

            ReaPi.ClearJob(rjConnection.ConnectionID, 1);
        }

        public void RJStopJob()
        {
            UpdateDashboard(EventType.JOBSTOPPED);

            if (rjConnection.ConnectionID <= 0)
                return;

            //ReaPi.StopJob(rjConnection.ConnectionID, 1);
        }

        public void RJStartJob()
        {
            UpdateDashboard(EventType.JOBSTARTED);
            if (rjConnection.ConnectionID <= 0)
                return;

            //ReaPi.StartJob(rjConnection.ConnectionID, 1);
        }

        public void RJDisconnect()
        {
            try
            {
                if (rjConnection.ConnectionID <= 0)
                    return;

                ReaPi.Disconnect(rjConnection.ConnectionID);
            }
            catch (Exception e) {
                AppLogger.GetLogger().Error($"Błąd rozłączenia połączenia i ID: {rjConnection.ConnectionID}", e);
            }
        }

        public void RJSetOutputLevel(int output, int state)
        {
            if (rjConnection.ConnectionID <= 0)
                return;

            //ReaPi.SetIOOutputLevel(rjConnection.ConnectionID, output, state);
        }

        public void RJGetOutputLevel()
        {
            if (rjConnection.ConnectionID <= 0)
                return;

            //ReaPi.GetIOOutputLevel(rjConnection.ConnectionID);
        }

        public void RJSetIOConfiguration(string fileName)
        {
            if (rjConnection.ConnectionID <= 0)
                return;

            ReaPi.SetIOConfiguration(rjConnection.ConnectionID, rjConnection.Job.JobId, fileName);
        }

        public List<DTOLabelSettings> GetDTOLabelSettings()
        {
            List<DTOLabelSettings> listDTO = new List<DTOLabelSettings>();

            foreach (var item in rjConnection.Job.VariableContents)
            {
                DTOLabelSettings dto = new DTOLabelSettings(item.GroupName, "", item.ObjectName, item.ContentName, item.OutputControl, item.DataField);
                listDTO.Add(dto);
            }

            return listDTO;
        }

        public void AssignSettingsToVariableContents(string fileName)
        {
            List<DTOLabelSettings> listDTO = new List<DTOLabelSettings>();

            LabelSettings labelSettings = new LabelSettings();

            listDTO = labelSettings.Load(fileName);

            //if (countVariableContents != listDTO.Count)
            //{
            //    string message = "Brak zgodności pomiędzy etykietą a zapisanymi danymi!\n sterownik: " + countVariableContents.ToString() + " wczytane dane: " + listDTO.Count.ToString();
            //    MessageBox.Show(message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    //UpdateCheckList(helper.Label, false);
            //    return;
            //}

            //Przypisuje ustawienia etykiety
            rjConnection.Job.AssignSettings(listDTO, database.GetHeadersCount());
        }

        public List<Header> GetHeaders()
        {
            return database.GetHeaders();
        }

        public int GetHeadersCount()
        {
            return database.GetHeadersCount();
        }

        public int GetVariableContentsCount()
        {
            return rjConnection.Job.VariableContents.Count;
        }

        public double GetDatabaseMaxRecords()
        {
            return database.MaxRecords();
        }

        public double GetActualRecord()
        {
            return database.ActualRecord;
        }

        public List<string> GetRecordWithKey(double item)
        {
            return database.GetRecordWithKey(item);
        }

        public List<DigitalOutput> GetDigitalOutputs()
        {
            return rjConnection.GetOutputs();
        }

        public List<IVariableContent> GetVariableContents()
        {
            return rjConnection.Job.GetVariableContents();
        }

        public string GetActualJobFile()
        {
            return rjConnection.Job.JobFile;
        }

        public string GetActualIOFile()
        {
            return rjConnection.IOConfiguration;
        }

        public bool JobIsNull()
        {
            if (rjConnection.Job == null)
            {
                return true;
            } else
            {
                return false;
            }
            
        }

        public string GetDataFieldName(int columnNumber)
        {
            int headresCount = database.GetHeadersCount();
            if (columnNumber < 0 || columnNumber > headresCount || headresCount <= 0)
                return "";

            return database.GetHeaderName(columnNumber);
        }

        public string GetOutputName(int outputNumber)
        {
            int headresCount = database.GetHeadersCount();
            if (outputNumber < 0 || outputNumber > MAX_DIGITAL_OUTPUTS)
                return "";

            return rjConnection.GetOuputName(outputNumber);
        }

        public void AssignOutputToDataField(int outputNumber, int labelRow)
        {
            //czyszcze wczesniejsze ustawienia wyjscia - jedno wyjscie do jednej kolumny
            foreach (var item in rjConnection.Job.VariableContents)
            {
                if (item.OutputControl == outputNumber)
                    item.OutputControl = -1;
            }                       
            
            int dataField = rjConnection.Job.VariableContents[labelRow].DataField;

            //nie można przypisac wyjscia bez kolumny danych
            if (dataField > 0)
            {
                rjConnection.Job.VariableContents[labelRow].OutputControl = outputNumber;
                rjConnection.Outputs[outputNumber].DataField = dataField;
            }
        }

        public void AssignDataFieldToContent(int dataField,  int labelRow)
        {
            rjConnection.Job.VariableContents[labelRow].DataField = dataField;

            //usuwam wyjscie jak usuwana jest kolumna z danymi
            if (rjConnection.Job.VariableContents[labelRow].OutputControl > 0)
                rjConnection.Job.VariableContents[labelRow].OutputControl = -1;
        }

        public bool LoadDatabase(string fileName)
        {
            if (database.ClearDatabase() == false)
                return false;

            const char fieldSeparator = ';';
            bool createHeader = true;

            try
            {
                using (var sr = new StreamReader(fileName, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
                {
                    SendToDashboard(MessageType.EVENT, "Baza: " + fileName, null, null);

                    FileInfo fileCSV = new FileInfo(fileName);
                    long fileSize = fileCSV.Length;
                    long currentSize = 0;

                    long row = 0;
                    int numberOfColumns = 0;

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        currentSize += line.Length;

                        List<string> record = line.Split(fieldSeparator).ToList();

                        if (createHeader)
                        {
                            numberOfColumns = record.Count();
                            database.AddHeaders(record);
                            createHeader = false;
                        }
                        else
                        {
                            row += 1;
                            if (numberOfColumns == record.Count)
                            {
                                database.AddRecord(record);
                            }
                            else
                            {
                                //logInfo += " - wiersz nr: " + row.ToString() + "\n";
                                SendToDashboard(MessageType.EVENT, "Niezgodność bazy - wiersz: " + row.ToString(), null, null);

                                AppLogger.GetLogger().Error("Brak zgodności rekordu: " + row.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

                return true;
        }

        #endregion


    }
}
