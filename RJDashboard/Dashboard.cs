using MetroFramework.Forms;
using RJController;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RJController.Dashboard;
using RJController.Enums;
using System.Threading;
using RJDashboard.Files;
using System.IO;
using RJLogger;

namespace RJDashboard
{
    public partial class Dashboard : MetroForm, IDashboard
    {
        private string applicationPath;
        private string labelSettingsPath;
        private string LogEventsPath;
        private string databasePath;

        RJDevice rjDevice;

        public Dashboard()
        {
            InitializeComponent();

            rjDevice = new RJDevice(this);

            UpdateTrackEvents();
            UpdateTrackLogsAndErrors();
          
        }

        #region PUBLIC Methods

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

        public void UpdateDashboard(string rjEvent)
        {
            if (this.InvokeRequired)
            {
                Action<string> updateDashboard = new Action<string>(this.UpdateDashboard);
                if (!this.Created || this.IsDisposed)
                    return;
                this.Invoke(updateDashboard, rjEvent);
            }
            else
            {
                switch (rjEvent)
                {
                    case "CONNECT":
                        LoadAndSetJobFiles(rjDevice.GetJobFiles(), rjDevice.rjConnection.Job.JobFile);
                        LoadAndSetIOFiles(rjDevice.GetIOFiles(), rjDevice.rjConnection.IOConfiguration);

                        SetBackgroundColorForConnectionStatus(true);
                        break;
                    case "DISCONNECT":
                        SetBackgroundColorForConnectionStatus(false);
                        SetBackgroundColorForJobStatus(false);
                        break;
                    case "JOBSET":
                        LoadAndSetJobFiles(rjDevice.GetJobFiles(), rjDevice.rjConnection.Job.JobFile);
                        ShowVariableContentsForLabel();
                        break;
                    case "JOBSTOP":
                        SetBackgroundColorForJobStatus(false);
                        cmbJobFiles.Enabled = true;
                        cmbIOFiles.Enabled = true;
                        break;
                    case "JOBSTART":
                        SetBackgroundColorForJobStatus(true);
                        cmbJobFiles.Enabled = false;
                        cmbIOFiles.Enabled = false;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region PRIVATE Methods

        public void LoadAndSetJobFiles(string[] files, string actualJob)
        {
            if (this.InvokeRequired)
            {
                Action<string[], string> loadAndSetJobFiles = new Action<string[], string>(this.LoadAndSetJobFiles);
                if (!this.Created || this.IsDisposed)
                    return;
                this.Invoke(loadAndSetJobFiles, files, actualJob);
            }
            else
            {
                try
                {
                    cmbJobFiles.Items.Clear();

                    for (int i = 0; i < files.Length; i++)
                    {
                        cmbJobFiles.Items.Insert(i, (object)new FileInfo(files[i]).Name);
                    }
                    try
                    {
                        if (! string.IsNullOrEmpty(actualJob))
                            cmbJobFiles.Text = actualJob;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Brak aktywnego zadania na sterowniku!", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        if (! string.IsNullOrEmpty(cmbJobFiles.Items[0].ToString()))
                            cmbJobFiles.Text = cmbJobFiles.Items[0].ToString();

                        AppLogger.GetLogger().Error("Brak aktywnego zadania na sterowniku!", e);
                    }
                    tileClearJob.Enabled = true;
                    tileSetJob.Enabled = true;
                }
                catch (Exception e)
                {
                    tileSetJob.Enabled = false;
                    tileClearJob.Enabled = false;

                    AppLogger.GetLogger().Error("Problem podczas wczytywania listy zadań!", e);
                }
            }
        }

        public void LoadAndSetIOFiles(string[] files, string actualIO)
        {
            if (this.InvokeRequired)
            {
                Action<string[], string> loadAndSetIOFiles = new Action<string[], string>(this.LoadAndSetIOFiles);
                if (!this.Created || this.IsDisposed)
                    return;
                this.Invoke(loadAndSetIOFiles, files, actualIO);
            }
            else
            {
                try
                {
                    cmbIOFiles.Items.Clear();

                    for (int i = 0; i < files.Length; i++)
                    {
                        cmbIOFiles.Items.Insert(i, (object)new FileInfo(files[i]).Name);
                    }
                    try
                    {
                        if (!string.IsNullOrEmpty(actualIO))
                            cmbIOFiles.Text = actualIO;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Brak aktywnego zadania na sterowniku!", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (!string.IsNullOrEmpty(cmbIOFiles.Items[0].ToString()))
                        {
                            cmbIOFiles.Text = cmbIOFiles.Items[0].ToString();
                        }
                        AppLogger.GetLogger().Error("Brak aktywnego zadania na sterowniku!", e);
                    }
                    tileSetIOFiles.Enabled = true;
                }
                catch (Exception e)
                {
                    tileSetIOFiles.Enabled = false;

                    AppLogger.GetLogger().Error("Problem podczas wczytywania listy ustawień IO!", e);
                }
            }
        }

        private void BeforeCoseApplication()
        {
            string destinationErrors; 
            string destinationEvents;
            string fileName;

            DateTime thisTime = DateTime.Now;
            fileName = thisTime.ToShortDateString() + "_" + thisTime.Hour + "." + thisTime.Minute + "." + thisTime.Second;
            destinationErrors = LogEventsPath + "\\" + "Error_" + fileName + ".txt";
            destinationEvents = LogEventsPath + "\\" + "Event_" + fileName + ".txt";

            logErrors.Save(destinationErrors);
            logEvents.Save(destinationEvents);

            //reaJetControl.ReaStopJob();
        }

        private void ShowVariableContentsForLabel()
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

        private DialogResult MessageFormClose()
        {
            return MessageBox.Show("Czy chcesz zamknąć program?", "Koniec", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private void SetBackgroundColorForConnectionStatus(bool active)
        {
            if (active == true)
            {
                labelConnectionStatus.BackColor = Color.LimeGreen;
            }
            else
            {
                labelConnectionStatus.BackColor = Color.Red;
            }

        }

        private void SetBackgroundColorForJobStatus(bool active)
        {
            if (active == true)
            {
                labelJobStatus.BackColor = Color.LimeGreen;
            }
            else
            {
                labelJobStatus.BackColor = Color.Red;
            }
        }

        #endregion

        #region CONTROLS Methods

        private void tileConnect_Click(object sender, EventArgs e)
        {
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

        private void Dashboard_Load(object sender, EventArgs e)
        {
            AppFolders folder;
            string mainPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            applicationPath = folder.Create(mainPath, FolderType.Application);
            labelSettingsPath = folder.Create(applicationPath, FolderType.LabelSettings);
            databasePath = folder.Create(applicationPath, FolderType.Database);
            LogEventsPath = folder.Create(applicationPath, FolderType.LogEvents);
        }

        private void tileJobControl_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(0);
        }

        private void tileIOControl_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(1);
        }

        private void tileDataPrint_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(2);
        }

        private void tileLabelSettings_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(3);
        }

        private void tileAbout_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(4);
        }

        private void tileSettings_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(5);
        }

        private void tileClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageFormClose() == DialogResult.Yes)
            {
                BeforeCoseApplication();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void tileSetJob_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbJobFiles.Text))
            {
                ShowProblemSolution(ErrorType.nullJob);
                return;
            }
            rjDevice.RJSetJob(cmbJobFiles.Text);
        }

        private void tileClearJob_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbJobFiles.Text))
                return;

            DialogResult result = MessageBox.Show("Czy chcesz usunąć aktualne zadanie?", "Uwaga", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                rjDevice.RJClearJob();
            }
        }

        #endregion
    }
}
