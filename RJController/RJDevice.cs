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

        private bool _stopLine;

        private RJConnect rjConnection;
        private Database database;

        public RJDevice(IDashboard view)
        {
            _view = view;
            database = new Database();        

            if (CheckReaPiLibrary() == true)
            {
                //test API
                //rjConnection = new RJConnect((ReaPi.ConnectionIdentifier) 1, null);
                rjConnection = new RJConnect();

                rjConnection.IOConfiguration = "osb_hr_1.dio";

                /*
                for (int i = 0; i < 8; i++)
                {
                    DigitalOutput output = new DigitalOutput();
                    output.Name = $"Out {i + 1}";
                    output.Description = " - ";
                    output.IsActive = true;
                    output.DataField = i;

                    rjConnection.AddOutput(i, output);
                }*/
                //test
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
            ReaPi.EErrorCode errorcode, 
            IntPtr context)
        {
            switch (commandid)
            {
                case ReaPi.ECommandId.CMD_CONNECT:
                    break;
                case ReaPi.ECommandId.CMD_DISCONNECT:
                    break;
                case ReaPi.ECommandId.CMD_SETJOB:
                    break;
                case ReaPi.ECommandId.CMD_STARTJOB:
                    break;
                case ReaPi.ECommandId.CMD_STOPJOB:
                    break;
                case ReaPi.ECommandId.CMD_SETLABELCONTENT:
                    break;
                case ReaPi.ECommandId.CMD_SETDATETIME:
                    break;
                case ReaPi.ECommandId.CMD_GETCARTRIDGES:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEJOBSET:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEJOBSET:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEJOBSTARTED:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEJOBSTARTED:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEJOBSTOPPED:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEJOBSTOPPED:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEPRINTTRIGGER:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEPRINTTRIGGER:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEPRINTSTART:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEPRINTSTART:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEPRINTEND:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEPRINTEND:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEPRINTREJECTED:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEPRINTREJECTED:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEPRINTABORTED:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEPRINTABORTED:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEPRINTSPEEDERROR:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEPRINTSPEEDERROR:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEINVALIDCONTENT:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEINVALIDCONTENT:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBESHAFTENCODERROTATION:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBESHAFTENCODERROTATION:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEPRODUCTSENSORCHANGED:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEPRODUCTSENSORCHANGED:
                    break;
                case ReaPi.ECommandId.CMD_CLEARJOB:
                    break;
                case ReaPi.ECommandId.CMD_STARTPURGE:
                    break;
                case ReaPi.ECommandId.CMD_LABELPREVIEW:
                    break;
                case ReaPi.ECommandId.CMD_GETTIMEZONE:
                    break;
                case ReaPi.ECommandId.CMD_SETTIMEZONE:
                    break;
                case ReaPi.ECommandId.CMD_GETDATETIME:
                    break;
                case ReaPi.ECommandId.CMD_GETIOCONFIGURATION:
                    break;
                case ReaPi.ECommandId.CMD_SETIOCONFIGURATION:
                    break;
                case ReaPi.ECommandId.CMD_GETNETWORKCONFIG:
                    break;
                case ReaPi.ECommandId.CMD_SETNETWORKCONFIG:
                    break;
                case ReaPi.ECommandId.CMD_GETDEVICEINFO:
                    break;
                case ReaPi.ECommandId.CMD_GETLABELCONTENT:
                    break;
                case ReaPi.ECommandId.CMD_GETLABELOBJECT:
                    break;
                case ReaPi.ECommandId.CMD_SETLABELOBJECT:
                    break;
                case ReaPi.ECommandId.CMD_GETCARTRIDGEPARAMETER:
                    break;
                case ReaPi.ECommandId.CMD_SETCARTRIDGEPARAMETER:
                    break;
                case ReaPi.ECommandId.CMD_BUILDLOGPACKAGE:
                    break;
                case ReaPi.ECommandId.CMD_FACTORYDEFAULTS:
                    break;
                case ReaPi.ECommandId.CMD_FIRMWAREUPDATE:
                    break;
                case ReaPi.ECommandId.CMD_LISTDIRECTORY:
                    break;
                case ReaPi.ECommandId.CMD_PUTFILE:
                    break;
                case ReaPi.ECommandId.CMD_GETFILE:
                    break;
                case ReaPi.ECommandId.CMD_DELETEFILE:
                    break;
                case ReaPi.ECommandId.CMD_COPYFILE:
                    break;
                case ReaPi.ECommandId.CMD_MOVEFILE:
                    break;
                case ReaPi.ECommandId.CMD_LOCKFILE:
                    break;
                case ReaPi.ECommandId.CMD_UNLOCKFILE:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEIOCONFIGURATIONSET:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEIOCONFIGURATIONSET:
                    break;
                case ReaPi.ECommandId.CMD_GETSYSTEMTIMEZONES:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBELABELPROPERTYCHANGED:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBELABELPROPERTYCHANGED:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBELASERUNITINFO:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBELASERUNITINFO:
                    break;
                case ReaPi.ECommandId.CMD_UPDATEFONTS:
                    break;
                case ReaPi.ECommandId.CMD_SETDEVICEPROPERTY:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBELABELEVENT:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBELABELEVENT:
                    break;
                case ReaPi.ECommandId.CMD_SYSTEMCOMMAND:
                    break;
                case ReaPi.ECommandId.CMD_GETLOGLEVEL:
                    break;
                case ReaPi.ECommandId.CMD_SETLOGLEVEL:
                    break;
                case ReaPi.ECommandId.CMD_GETPILOTLASERDISPLACEMENT:
                    break;
                case ReaPi.ECommandId.CMD_SETPILOTLASERDISPLACEMENT:
                    break;
                case ReaPi.ECommandId.CMD_GETIOINPUTLEVEL:
                    break;
                case ReaPi.ECommandId.CMD_GETIOOUTPUTLEVEL:
                    break;
                case ReaPi.ECommandId.CMD_SETIOOUTPUTLEVEL:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEMISSINGCONTENT:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEMISSINGCONTENT:
                    break;
                case ReaPi.ECommandId.CMD_GETNOPRINTBUFFERMODE:
                    break;
                case ReaPi.ECommandId.CMD_SETNOPRINTBUFFERMODE:
                    break;
                case ReaPi.ECommandId.CMD_CONFIRMPRINTREJECT:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEPRINTREJECTCONFIRMED:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEPRINTREJECTCONFIRMED:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEINSTALLATIONACTIVITY:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEINSTALLATIONACTIVITY:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEREADYFORNEXTCONTENT:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEREADYFORNEXTCONTENT:
                    break;
                case ReaPi.ECommandId.CMD_SETINSTALLATION:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEINSTALLATIONSET:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEINSTALLATIONSET:
                    break;
                case ReaPi.ECommandId.CMD_SETLABEL:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBELABELSET:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBELABELSET:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEIMAGESTART:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEIMAGESTART:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEIMAGEEND:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEIMAGEEND:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBENETWORKCONFIGCHANGED:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBENETWORKCONFIGCHANGED:
                    break;
                case ReaPi.ECommandId.CMD_GETNTPCONFIG:
                    break;
                case ReaPi.ECommandId.CMD_SETNTPCONFIG:
                    break;
                case ReaPi.ECommandId.CMD_ADDLABELCONTENT:
                    break;
                case ReaPi.ECommandId.CMD_DEINSTALLADDON:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBENETWORKCONFIGCHANGING:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBENETWORKCONFIGCHANGING:
                    break;
                case ReaPi.ECommandId.CMD_SETNETWORKCONFIGWITHNTPCONFIG:
                    break;
                case ReaPi.ECommandId.CMD_GETPRODUCTSENSORLEVEL:
                    break;
                case ReaPi.ECommandId.CMD_SETCONTENTBUFFERCONTROL:
                    break;
                case ReaPi.ECommandId.CMD_GETCONTENTBUFFERCONTROL:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEBUFFERUNDERRUN:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEBUFFERUNDERRUN:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBEBUFFERFULL:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBEBUFFERFULL:
                    break;
                case ReaPi.ECommandId.CMD_DEVICEDEVELOPFEATURES:
                    break;
                case ReaPi.ECommandId.CMD_SUBSCRIBECARTRIDGEPARAMETERCHANGED:
                    break;
                case ReaPi.ECommandId.CMD_UNSUBSCRIBECARTRIDGEPARAMETERCHANGED:
                    break;
                case ReaPi.ECommandId.CMD_GETPRODUCTIONDATA:
                    break;
                case ReaPi.ECommandId.CMD_REA:
                    break;
                case ReaPi.ECommandId.CMD_NONE:
                    break;
                default:
                    break;
            }
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
            //SetOutputLevel(OutputTypeReaction.JobStatus, OutputLevel.High);
        }

        private void OnJobStoppedEvent()
        {
            UpdateDashboard(EventType.JOBSTOPPED);
            //SetOutputLevel(OutputTypeReaction.JobStatus, OutputLevel.Low);
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

                //OutputLevel level = GetOutputLevelToBeSetForAssignedColumn(OutputTypeReaction.ControlStacker, record.Id);
                //SetOutputLevel(OutputTypeReaction.ControlStacker, level);

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
        }

        public void RJClearJob()
        {

        }

        public void RJStopJob()
        {
            UpdateDashboard(EventType.JOBSTOPPED);
        }

        public void RJStartJob()
        {
            UpdateDashboard(EventType.JOBSTARTED);
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

        public void RJSetIOConfiguration(string fileName)
        {

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
            //DODA AUTOMATYCZNY ODCZYT ilosci kolumn
            rjConnection.Job.AssignSettings(listDTO, 10);
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

        public void AssignOutputToDataField(int outputNumber, int dataField)
        {
            if (outputNumber > 0)
            {
                rjConnection.Outputs[outputNumber].DataField = dataField;
            }            
            rjConnection.Job.VariableContents[dataField].OutputControl = outputNumber;
        }

        public void AssignDataFieldToContent(int dataField,  int labelContentNumber)
        {
            rjConnection.Job.VariableContents[labelContentNumber].DataField = dataField;
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
