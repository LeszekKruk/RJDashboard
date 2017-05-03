using ReaPiSharp;
using RJController.Dashboard;
using RJController.Enums;
using RJController.Job;
using RJLogger;
using System;
using System.IO;
using System.Threading;

namespace RJController
{
    public class RJDevice
    {
        static private ReaPi.connectionCallbackPtr _connectionCallback;
        static private ReaPi.responseCallbackPtr _responseCallback;
        static private ReaPi.eventCallbackPtr _eventCallback;

        IDashboard _view;

        public RJJob job;
        public RJConnection rjConnection;

        public RJDevice(IDashboard view)
        {
            _view = view;

            //test API
            job = new RJJob(1, "C:\\temp\\_ReaTest\\jobs\\test_LK.job");

            if (CheckReaPiLibrary() == true)
            {
                rjConnection = new RJConnection((ReaPi.ConnectionIdentifier) 1, "test",null);
                rjConnection.Job = job;
                rjConnection.IOConfiguration = "osb_hr_1.dio";

                _connectionCallback = new ReaPi.connectionCallbackPtr(OnConnectionCallback);

                ReaPi.EErrorCode tmpError = ReaPi.RegisterConnectionCallback(_connectionCallback, new IntPtr(0));

                if (tmpError == ReaPi.EErrorCode.OK)
                {
                    _view.DisplayLogsAndErrors("Status OK - ReaPi.RegisterConnectionCallback()", "", "");
                }
                else
                {
                    _view.DisplayLogsAndErrors("Błąd - ReaPi.RegisterConnectionCallback()", tmpError.ToString(), "");
                }
            }
            else
            {
                _view.ShowProblemSolution(ErrorType.libraryError);
            }        
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
                DisplayMessage(log, null, null);
            }

            return library;
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
                        _view.DisplayLogsAndErrors("OnReaPiConnectionCallbackPtr() for EConnState.CONNECT", errorCode.ToString(), "");
                        return;
                    }

                    _view.DisplayEvents("Połączenie " + connectionId + " nawiązane");

                    rjConnection = new RJConnection(connectionId, connectionId.ToString(), null);

                    _responseCallback = new ReaPi.responseCallbackPtr(OnResponseCallback);
                    ReaPi.RegisterResponseCallback(connectionId, _responseCallback, context);

                    _eventCallback = new ReaPi.eventCallbackPtr(OnEventCallback);
                    ReaPi.RegisterEventCallback(connectionId, _eventCallback, context);

                    OnConnect(connectionId);
                }
                else
                {
                    _view.DisplayLogsAndErrors("Brak możliwości połączenia", null, null );
                    _view.ShowProblemSolution(ErrorType.errorConnection);
                }
            }
            else if (state == ReaPi.EConnState.DISCONNECT)
            {
                if (errorCode != ReaPi.EErrorCode.OK)
                {
                    _view.DisplayLogsAndErrors("OnReaPiConnectionCallbackPtr() for EConnState.DISCONNECT", errorCode.ToString(),null);
                }

                //_infoReaPiLib.Connections.Remove(connectionId);
                _view.DisplayLogsAndErrors("Połączenie <" + connectionId + "> zamknięte", null, null);

                OnDisconnect();
            }
            else if (state == ReaPi.EConnState.CONNECTIONERROR)
            {
                _view.DisplayLogsAndErrors("Połączenie <" + connectionId + "> zamknięte z błędem ", errorCode.ToString(), null);
                //_view.UpdateControls("DISCONNECTED");
                _view.ShowProblemSolution(ErrorType.errorConnection);
            }
        }

        private void OnEventCallback(ReaPi.ResponseHandle response, ReaPi.ConnectionIdentifier connection, ReaPi.EEventId eventid, IntPtr context)
        {
            switch (eventid)
            {
                case ReaPi.EEventId.JOBSET:
                    break;
                case ReaPi.EEventId.JOBSTARTED:
                    break;
                case ReaPi.EEventId.JOBSTOPPED:
                    break;
                case ReaPi.EEventId.PRINTTRIGGER:
                    break;
                case ReaPi.EEventId.PRINTSTART:
                    break;
                case ReaPi.EEventId.PRINTREJECTED:
                    break;
                case ReaPi.EEventId.PRINTEND:
                    break;
                case ReaPi.EEventId.PRINTABORTED:
                    break;
                case ReaPi.EEventId.PRINTSPEEDERROR:
                    break;
                case ReaPi.EEventId.INVALIDCONTENT:
                    break;
                case ReaPi.EEventId.CARTRIDGECHANGED:
                    break;
                case ReaPi.EEventId.SHAFTENCODERROTATION:
                    break;
                case ReaPi.EEventId.PRODUCTSENSORCHANGED:
                    break;
                case ReaPi.EEventId.FIRMWAREUPGRADEPROGRESS:
                    break;
                case ReaPi.EEventId.IOCONFIGURATIONSET:
                    break;
                case ReaPi.EEventId.LABELPROPERTYCHANGED:
                    break;
                case ReaPi.EEventId.LASERUNITINFO:
                    break;
                case ReaPi.EEventId.PRINTSTATUS:
                    break;
                case ReaPi.EEventId.LABELEVENT:
                    break;
                case ReaPi.EEventId.BUILDLOGPACKAGEPROGRESS:
                    break;
                case ReaPi.EEventId.PURGECOMPLETED:
                    break;
                case ReaPi.EEventId.MISSINGCONTENT:
                    break;
                case ReaPi.EEventId.PRINTREJECTCONFIRMED:
                    break;
                case ReaPi.EEventId.INSTALLATIONACTIVITY:
                    break;
                case ReaPi.EEventId.READYFORNEXTCONTENT:
                    break;
                case ReaPi.EEventId.INSTALLATIONSET:
                    break;
                case ReaPi.EEventId.LABELSET:
                    break;
                case ReaPi.EEventId.IMAGESTART:
                    break;
                case ReaPi.EEventId.IMAGEEND:
                    break;
                case ReaPi.EEventId.NETWORKCONFIGCHANGED:
                    break;
                case ReaPi.EEventId.NETWORKCONFIGCHANGING:
                    break;
                case ReaPi.EEventId.BUFFERUNDERRUN:
                    break;
                case ReaPi.EEventId.BUFFERFULL:
                    break;
                case ReaPi.EEventId.CARTRIDGEPARAMETERCHANGED:
                    break;
                default:
                    break;
            }
        }

        private void OnResponseCallback(ReaPi.ResponseHandle response, ReaPi.ConnectionIdentifier connection, ReaPi.ECommandId commandid, ReaPi.EErrorCode errorcode, IntPtr context)
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

        private void OnConnect(ReaPi.ConnectionIdentifier connectionId)
        {
            string lastError;
            rjConnection.ProtocolVersion = new Version(ReaPi.GetProtocolVersion(rjConnection.ConnectionID));

            lastError = ReaPi.SubscribeJobSet(rjConnection.ConnectionID, 1).ToString();
            lastError = ReaPi.SubscribeIOConfigurationSet(rjConnection.ConnectionID, 1).ToString();

            _view.DisplayLogsAndErrors("Połączenie: OK", " Wskaźnik połączenia: " + rjConnection.ConnectionID.ToString(), null);

            //_view.UpdateControls("CONNECTED");

            lastError = ReaPi.GetIOOutputLevel(rjConnection.ConnectionID).ToString();

            _view.DisplayLogsAndErrors(lastError, null, null);
        }

        private void DisplayMessage(string message, string a, string b)
        {
            _view.DisplayLogsAndErrors(message, a, b);
        }

        private void OnDisconnect()
        {

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
            if (!(rjConnection == null) && (rjConnection.ConnectionID <= 0))
            {
                try
                {
                    ReaPi.EErrorCode errorCode = (ReaPi.EErrorCode)ReaPi.Connect("TCP://" + ipAddress);
                    if (errorCode >= ReaPi.EErrorCode.OK)
                    {
                        return;
                    }
                    else
                    {
                        _view.DisplayLogsAndErrors("Połączenie <" + ipAddress + "> zamknięte z błędem ", errorCode.ToString(), null);
                        _view.ShowProblemSolution(ErrorType.errorConnection);
                    }                    
                }
                catch (Exception)
                {
                    //log = "Error on calling function ReaConnect()";

                }
            }
            else
            {
                _view.ShowProblemSolution(ErrorType.nullIPAddress);
            }
        }

        public void RJSetJob(string jobName)
        {

        }

        public void RJClearJob()
        {

        }

        public void RJDisconnect()
        {
            try
            {
                if (rjConnection.ConnectionID <= 0)
                    return;

                ReaPi.Disconnect(rjConnection.ConnectionID);
            }
            catch (Exception e)
            {
                _view.DisplayLogsAndErrors("Błąd wywołania metody Disconnect", e.Source.ToString(), e.Message.ToString());
            }
        }

    }
}
