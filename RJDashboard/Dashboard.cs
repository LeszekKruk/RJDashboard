using MetroFramework.Forms;
using RJController;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RJController.Dashboard;
using RJController.Enums;
using System.Threading;

namespace RJDashboard
{
    public partial class Dashboard : MetroForm, IDashboard
    {
        RJDevice rjDevice;

        public Dashboard()
        {
            InitializeComponent();

            rjDevice = new RJDevice(this);

            UpdateTrackEvents();
            UpdateTrackLogsAndErrors();

            ShowManagementableContents();

            
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
                logEvents.AddLog(log, null, null);
            }
        }

        public void DisplayLogsAndErrors(string log, string errorCodeDomain, string errorMessage)
        {
            if (this.InvokeRequired)
            {
                Action<string, string, string> logsAndErrorsCallback = this.DisplayLogsAndErrors;
                if (!this.Created || this.IsDisposed)
                    return;
                this.Invoke(logsAndErrorsCallback, log, errorCodeDomain, errorMessage);
            }
            else
            {
                logErrors.AddLog(log, errorCodeDomain, errorMessage);
            }
        }

        public void ShowProblemSolution(ErrorType type)
        {
            if (this.InvokeRequired)
            {
                Action<ErrorType> showProblemSolution = new Action<ErrorType>(this.ShowProblemSolution);
                if (!this.Created || this.IsDisposed)
                    return;
                this.Invoke(showProblemSolution, type);
            }
            else
            {               
                ProblemSolution info = new ProblemSolution(type);
                info.Show();
            }
        }

        // PRIVATE Methods

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

            for (int index = 0; index < rjDevice.job.LabelManagement.Count; ++index)
            {
                ListViewItem listViewItem = new ListViewItem(rjDevice.job.LabelManagement[index].GroupName);

                listViewItem.SubItems.Add(rjDevice.job.LabelManagement[index].ObjectName);
                listViewItem.SubItems.Add(rjDevice.job.LabelManagement[index].ContentName);
                listViewItem.SubItems.Add(rjDevice.job.LabelManagement[index].ContentValue);
                listViewItem.SubItems.Add(rjDevice.job.LabelManagement[index].DataField.ToString());
                listViewItem.SubItems.Add(rjDevice.job.LabelManagement[index].OutputControl.ToString());

                listLabelContents.Items.Add(listViewItem);
            }
        }

        private void UpdateTrackEvents()
        {
            labelMaxEvents.Text = "Ilość wyświetlanych zdarzeń: " + trackEvents.Value.ToString();
            logEvents.MaxLogs = trackEvents.Value;
        }

        private void UpdateTrackLogsAndErrors()
        {
            labelMaxErrors.Text = "Ilość wyświetlanych logów: " + trackErrors.Value.ToString();
            logErrors.MaxLogs = trackErrors.Value;
        }



        // End Private



        // CONTROL AKCTIONS
        private void tileClearJob_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                logErrors.AddLog($"Error {i}", null, null);
                logEvents.AddLog($"Event {i}", null, null);
                Thread.Sleep(500);
            }
        }

        private void tileConnect_Click(object sender, EventArgs e)
        {
            //string ip = cmbIpAddress.SelectedItem.ToString();
            try
            {
                if (cmbIpAddress.SelectedItem.ToString() == null)
                {
                    ShowProblemSolution(ErrorType.nullIPAddress);
                }
                else
                {
                    rjDevice.RJConnect(cmbIpAddress.SelectedItem.ToString());
                    //UpdateControls("CONNECTED");
                }
            }
            catch (Exception) { }
        }

        private void tileDisconnect_Click(object sender, EventArgs e)
        {
            rjDevice.RJDisconnect();
            //UpdateControls("DISCONNECTED");
        }

        private void trackEvents_ValueChanged(object sender, EventArgs e)
        {
            UpdateTrackEvents();
        }

        private void trackErrors_ValueChanged(object sender, EventArgs e)
        {
            UpdateTrackLogsAndErrors();
        }
        //
    }
}
