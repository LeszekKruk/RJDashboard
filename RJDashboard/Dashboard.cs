using MetroFramework.Forms;
using RJController;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RJController.Dashboard;
using RJController.Enums;
using RJLogger;

namespace RJDashboard
{
    public partial class Dashboard : MetroForm, IDashboard
    {
        RJDevice rjControll;

        private int DISPLAY_MAX_EVENTS = 100;
        private int DISPLAY_MAX_LOG_ERRORS = 100;
        private int _maxEvents = 0;
        private int _maxLogErrors = 0;

        private List<string> listEvents;
        private List<string> listLogsAndErrors;

        //delegate void DisplayInformationErrorCallback(ErrorType type);

        public Dashboard()
        {
            InitializeComponent();
            listEvents = new List<string>();
            listLogsAndErrors = new List<string>();

            rjControll = new RJDevice(this);

            UpdateTrackEvents();
            UpdateTrackLogsAndErrors();

            ShowManagementableContents();

            //AppLogger.GetLogger().Info("Konstruktor");
            //AppLogger.GetLogger().Error("Error - konstruktor",  new Exception("XXX"));
            //AppLogger.GetLogger().Warn("Warn - konstruktor");
        }

        // PUBLIC
        public void DisplayEvents(string log)
        {
            if (this.InvokeRequired)
            {
                Action<string> eventsCallback = this.DisplayEvents;
                if (!this.Created || this.IsDisposed)
                    return;
                this.Invoke(eventsCallback, log);
            }
            else
            {
                txtEvents.AppendText(Environment.NewLine + log);
                _maxEvents += 1;

                listEvents.Add(log);

                if (_maxEvents > DISPLAY_MAX_EVENTS)
                {
                    try
                    {
                        _maxEvents = 0;
                        txtEvents.Clear();
                    }
                    catch (Exception e)
                    {
                        AppLogger.GetLogger().Warn("Błąd podczas czyszczenia zdarzeń");
                        AppLogger.GetLogger().Warn(e.ToString());
                    }
                }
            }
        }

        public void DisplayLogsAndErrors(string log, string errorCodeDomain, string errorMessage)
        {
            if (this.InvokeRequired)
            {
                //Action<string, string, string> logsAndErrorsCallback = new Action < string, string, string>(DisplayLogsAndErrors);
                Action<string, string, string> logsAndErrorsCallback = this.DisplayLogsAndErrors;
                if (!this.Created || this.IsDisposed)
                    return;
                this.Invoke(logsAndErrorsCallback, log, errorCodeDomain, errorMessage);
            }
            else
            {
                if (log != "" || errorMessage != "" || errorCodeDomain != "")
                {
                    try
                    {
                        _maxLogErrors += 1;

                        txtLastError.AppendText(Environment.NewLine);
                        txtLastError.AppendText("------------------------------");

                        listLogsAndErrors.Add("------------------------------");
                        if (log != "" && log != null)
                        {
                            txtLastError.AppendText(Environment.NewLine);
                            txtLastError.AppendText("Log: " + log);

                            listLogsAndErrors.Add(log);
                        }

                        if (errorMessage != "" && errorMessage != null)
                        {
                            txtLastError.AppendText(Environment.NewLine);
                            txtLastError.AppendText("LME: " + errorMessage);

                            listLogsAndErrors.Add("LME: " + errorMessage);
                        }

                        if (errorCodeDomain != "" && errorCodeDomain != null)
                        {
                            txtLastError.AppendText(Environment.NewLine);
                            txtLastError.AppendText("LCD: " + errorCodeDomain);

                            listLogsAndErrors.Add("LCD: " + errorCodeDomain);
                        }

                        if (_maxLogErrors > DISPLAY_MAX_LOG_ERRORS)
                        {
                            try
                            {
                                _maxLogErrors = 0;
                                txtLastError.Clear();
                            }
                            catch (Exception e)
                            {
                                AppLogger.GetLogger().Warn("Błąd podczas czyszczenia logów");
                                AppLogger.GetLogger().Warn(e.ToString());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        txtLastError.Clear();
                        _maxLogErrors = 0;

                        AppLogger.GetLogger().Error("Błąd podczas czyszczenia zdarzeń");
                        AppLogger.GetLogger().Error(e.ToString());
                    }
                }
            }
        }

        public void DisplayInformationError(ErrorType type)
        {
            if (this.InvokeRequired)
            {
                Action<ErrorType> showInformationErrorCallback = new Action<ErrorType>(this.DisplayInformationError);
                if (!this.Created || this.IsDisposed)
                    return;
                this.Invoke(showInformationErrorCallback, type);
            }
            else
            {               
                Information info = new Information(type);
                info.Show();
            }
        }

        // PRIVATE

        private void ShowManagementableContents()
        {
            listLabelContents.Items.Clear();
            listLabelContents.Clear();

            List<string> listHeaders = new List<string>();
            listHeaders.Add(" Grupa ");
            listHeaders.Add(" Nazwa obiektu ");
            listHeaders.Add(" Nazwa pola ");
            listHeaders.Add(" Zawartość pola ");
            listHeaders.Add(" Dane z bazy ");
            listHeaders.Add("Sterowane wyjście");

            foreach (var header in listHeaders)
            {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Name = header;
                columnHeader.Text = header;
                columnHeader.TextAlign = HorizontalAlignment.Center;
                
                SizeF sizeF = Graphics.FromHwnd(this.listLabelContents.Handle).MeasureString(columnHeader.Text, this.listLabelContents.Font);
                columnHeader.Width = Convert.ToInt32((double)sizeF.Width + 0.5) + 10;

                listLabelContents.Columns.Add(columnHeader);
            }

            for (int index = 0; index < rjControll.job.LabelManagement.Count; ++index)
            {
                ListViewItem listViewItem = new ListViewItem(rjControll.job.LabelManagement[index].GroupName);

                listViewItem.SubItems.Add(rjControll.job.LabelManagement[index].ObjectName);
                listViewItem.SubItems.Add(rjControll.job.LabelManagement[index].ContentName);
                listViewItem.SubItems.Add(rjControll.job.LabelManagement[index].ContentValue);
                listViewItem.SubItems.Add(rjControll.job.LabelManagement[index].DataField.ToString());
                listViewItem.SubItems.Add(rjControll.job.LabelManagement[index].OutputControl.ToString());

                listLabelContents.Items.Add(listViewItem);
            }
        }

        private void UpdateTrackEvents()
        {
            labelMaxEvents.Text = "Ilość wyświetlanych zdarzeń: " + trackEvents.Value.ToString();
            DISPLAY_MAX_EVENTS = trackEvents.Value;
        }

        private void UpdateTrackLogsAndErrors()
        {
            labelMaxErrors.Text = "Ilość wyświetlanych logów: " + trackErrors.Value.ToString();
            DISPLAY_MAX_LOG_ERRORS = trackErrors.Value;
        }

        private void trackEvents_ValueChanged(object sender, EventArgs e)
        {
            UpdateTrackEvents();
        }

        private void trackErrors_ValueChanged(object sender, EventArgs e)
        {
            UpdateTrackLogsAndErrors();
        }

        private void tileConnect_Click(object sender, EventArgs e)
        {
            //string ip = cmbIpAddress.SelectedItem.ToString();
            try
            {
                if (cmbIpAddress.SelectedItem.ToString() == null)
                {
                    DisplayInformationError(ErrorType.nullIPAddress);
                }
                else
                {
                    rjControll.RJConnect(cmbIpAddress.SelectedItem.ToString());
                    //UpdateControls("CONNECTED");
                }
            }
            catch (Exception) {}           
        }

        private void tileDisconnect_Click(object sender, EventArgs e)
        {
            rjControll.RJDisconnect();
            //UpdateControls("DISCONNECTED");
        }
    }
}
