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
using RJController.Label;
using RJController.DTO;
using RJController.Job;
using RJController.IO;
using RJController.Model.Database;
using RJDashboard.Classes;
using System.Xml;

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
                this.BeginInvoke(eventsCallback, log);
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
                this.BeginInvoke(logsAndErrorsCallback, log, errorCodeDomain, errorMessage);
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

        public void UpdateDashboard(EventType rjEvent)
        {
            if (this.InvokeRequired)
            {
                Action<EventType> updateDashboard = new Action<EventType>(this.UpdateDashboard);
                if (!this.Created || this.IsDisposed)
                    return;
                this.Invoke(updateDashboard, rjEvent);
            }
            else
            {
                switch (rjEvent)
                {   
                    //UpdateOutputsState()
                    case EventType.CONNECT:
                        ClearAndSetControlAfterConnections(true);
                        if (rjDevice.JobIsNull() == true )
                        {
                            LoadAndSetJobFiles(rjDevice.GetJobFiles(), "");
                            LoadAndSetIOFiles(rjDevice.GetIOFiles(), "");
                        }
                        else
                        {
                            LoadAndSetJobFiles(rjDevice.GetJobFiles(), rjDevice.GetActualJobFile());
                            LoadAndSetIOFiles(rjDevice.GetIOFiles(), rjDevice.GetActualIOFile());

                            LoadVariableContentsForLabel();
                            LoadOutputsSettings();
                        }
                        UpdateCheckList(EventType.CONNECT, true);

                        SetBackgroundColorForConnectionStatus(true);                        
                        break;
                    case EventType.DISCONNECT:
                        ClearAndSetControlAfterConnections(false);
                        SetBackgroundColorForConnectionStatus(false);
                        SetBackgroundColorForJobStatus(false);
                        tabControl.Enabled = false;
                        UpdateCheckList(EventType.CONNECT, false);
                        break;
                    case EventType.JOBSET:
                        LoadAndSetJobFiles(rjDevice.GetJobFiles(), rjDevice.GetActualJobFile());
                        LoadAndSetIOFiles(rjDevice.GetIOFiles(), rjDevice.GetActualIOFile());
                        LoadVariableContentsForLabel();
                        LoadOutputsSettings();
                        UpdateCheckList(EventType.JOBSET, true);
                        break;
                    case EventType.JOBSTOPPED:
                        SetBackgroundColorForJobStatus(false);
                        cmbJobFiles.Enabled = true;
                        cmbIOFiles.Enabled = true;
                        UpdateCheckList(EventType.JOBSTARTED, false);
                        break;
                    case EventType.JOBSTARTED:
                        SetBackgroundColorForJobStatus(true);
                        cmbJobFiles.Enabled = false;
                        cmbIOFiles.Enabled = false;
                        UpdateCheckList(EventType.JOBSTARTED, true);
                        break;
                    case EventType.IOSET:
                        LoadAndSetIOFiles(rjDevice.GetIOFiles(), rjDevice.GetActualIOFile());
                        break;
                    case EventType.PRINTSTARTED:
                        SelectNextRecordInListView((int)rjDevice.GetActualRecord());
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region PRIVATE Methods

            #region JOB Title
            private void LoadAndSetJobFiles(string[] files, string actualJob)
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
                            if (!string.IsNullOrEmpty(actualJob))
                                cmbJobFiles.Text = actualJob;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Brak aktywnego zadania na sterowniku!", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            if (!string.IsNullOrEmpty(cmbJobFiles.Items[0].ToString()))
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
            #endregion



        private void LoadAndSetIOFiles(string[] files, string actualIO)
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

        private void SelectNextRecordInListView(int index)
        {
            try
            {
                listCSVContent.Items[index].Selected = true;
                listCSVContent.TopItem = listCSVContent.Items[index];
            }
            catch (Exception e)
            {
                AppLogger.GetLogger().Error("Błąd podczas zaznaczenia rekordu.", e);
            }
        }

        private void LoadVariableContentsForLabel()
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

            List<IVariableContent> dto = rjDevice.GetVariableContents();

            for (int i = 0; i < dto.Count; i++)
            {
                ListViewItem listViewItem = new ListViewItem(dto[i].GroupName);

                listViewItem.SubItems.Add(dto[i].ObjectName);
                listViewItem.SubItems.Add(dto[i].ContentName);
                listViewItem.SubItems.Add(dto[i].ContentValue);
                listViewItem.SubItems.Add(GetDataFieldName(dto[i].DataField));
                listViewItem.SubItems.Add(GetOutputName(dto[i].OutputControl));
                //listViewItem.SubItems.Add(dto[i].OutputControl.ToString());

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

        private void ClearAndSetControlAfterConnections(bool _isConnected)
        {
            tabControl.Enabled = _isConnected;

            cmbJobFiles.Items.Clear();
            cmbJobFiles.Text = "";

            cmbIOFiles.Items.Clear();
            cmbIOFiles.Text = "";

            tileConnect.Enabled = !_isConnected;
            cmbIpAddress.Enabled = !_isConnected;
            tileDisconnect.Enabled = _isConnected;

            txtLabelSettings.Text = "";
        }

        private void LoadOutputsSettings()
        {
            listOutputs.Items.Clear();
            listOutputs.Clear();

            List<string> listHeaders = new List<string>();
            listHeaders.Add(" Numer ");
            listHeaders.Add(" Nazwa ");
            listHeaders.Add("  Opis  ");
            listHeaders.Add(" Czy aktywne ");
            listHeaders.Add(" Kolumna z danymi ");
            listHeaders.Add(" Szukany znak ");

            foreach (var header in listHeaders)
            {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Name = header;
                columnHeader.Text = header;
                SizeF sizeF = Graphics.FromHwnd(this.listOutputs.Handle).MeasureString(columnHeader.Text, this.listOutputs.Font);
                columnHeader.Width = Convert.ToInt32((double)sizeF.Width + 0.5) + 8;
                listOutputs.Columns.Add(columnHeader);
            }

            List<DigitalOutput> outputs = rjDevice.GetDigitalOutputs();

            for (int i = 0; i < outputs.Count; i++)
            {
                ListViewItem listViewItem = new ListViewItem( (i+1).ToString());

                listViewItem.SubItems.Add(outputs[i].Name);
                listViewItem.SubItems.Add(outputs[i].Description);
                listViewItem.SubItems.Add(outputs[i].IsActive.ToString());
                listViewItem.SubItems.Add(GetDataFieldName(outputs[i].DataField));
                listViewItem.SubItems.Add("");
                listOutputs.Items.Add(listViewItem);
            }
        }

        private string GetDataFieldName(int columnNumber)
        {
            return rjDevice.GetDataFieldName(columnNumber);
        }

        private string GetOutputName(int outputNumber)
        {
            return rjDevice.GetOutputName(outputNumber);
        }

        private string OpenXmlFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = labelSettingsPath;
            openFileDialog.Filter = "Label settings files (*.xml)|*.xml";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = false;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return null;

            return openFileDialog.FileName;
        }

        private void LoadToComboDigitalOutputs()
        {
            comboOutputs.Items.Clear();

            List<DigitalOutput> outputs = rjDevice.GetDigitalOutputs();

            for (int i = 0; i < outputs.Count; i++)
            {
                comboOutputs.Items.Add(outputs[i]);
            }
            //comboOutputs.SelectedIndex = 0;
        }

        private void LoadToComboDatabaseHeaders()
        {
            comboCSVHeaders.Items.Clear();
            List<Header> headers = rjDevice.GetHeaders();

            for (int i = 0; i < headers.Count; i++)
            {
                comboCSVHeaders.Items.Add(headers[i]);
            }
        }

        private void ShowDatabaseInListView()
        {
            listCSVContent.Items.Clear();
            listCSVContent.Columns.Clear();

            comboCSVHeaders.Items.Clear();

            progressBarCSVLoading.Visible = true;
            progressBarCSVLoading.Minimum = 0;
            progressBarCSVLoading.Maximum = 100;

            LoadHeader();
            LoadRecords();

            UpdateProgressBar(1.0, 1.0);
            MessageBox.Show("Dane zostały załadowane...", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadToComboDatabaseHeaders();
            LoadToComboDigitalOutputs();

            progressBarCSVLoading.Visible = false;

            tileFindRecord.Enabled = listCSVContent.Items.Count > 0;
            tileSendData.Enabled = listCSVContent.Items.Count > 0;

            checkSubitems.Enabled = listCSVContent.Items.Count > 0;
            checkPartialMatches.Enabled = listCSVContent.Items.Count > 0;
        }

        private void LoadRecords()
        {
            double maxRecords = rjDevice.GetDatabaseMaxRecords();
            int headersCount = rjDevice.GetHeadersCount();

            for (int i = 1; i <= maxRecords; i++)
            {
                UpdateProgressBar(i, maxRecords);
                List<string> record = rjDevice.GetRecordWithKey(i);

                ListViewItem aa = new ListViewItem(i.ToString());

                for (int j = 1; j < headersCount; j++)
                {
                    aa.SubItems.Add(record[j]);
                }
                try
                {
                    listCSVContent.Items.Add(aa);
                }
                catch (Exception ex)
                {
                    string info = "Błąd podczas wczytywania bazy danych do kontrolki ListView: " + ex.ToString();
                    MessageBox.Show(info, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    AppLogger.GetLogger().Error("Błąd podczas wczytywania bazy danych do kontrolki ListView", ex);
                }
            }
        }

        private void LoadHeader()
        {
            List<Header> headers = rjDevice.GetHeaders();

            foreach (var item in headers)
            {
                ColumnHeader columnHeader = new ColumnHeader();
                columnHeader.Name = "column " + item.Id;
                columnHeader.Text = item.Name;
                SizeF sizeF = Graphics.FromHwnd(listCSVContent.Handle).MeasureString(columnHeader.Text, listCSVContent.Font);
                columnHeader.Width = Convert.ToInt32((double)sizeF.Width + 0.5) + 10;

                listCSVContent.Columns.Add(columnHeader);
            }
        }

        private void UpdateProgressBar(double currentSize, double fileSize)
        {
            progressBarCSVLoading.Value = (int)(currentSize / fileSize * 100);
        }

        private void UpdateCheckList(EventType eType, bool state)
        {
            switch (eType)
            {
                case EventType.CONNECT:
                    chbConnect.Checked = state;
                    break;
                case EventType.DISCONNECT:
                    break;
                case EventType.JOBSET:
                    chbJob.Checked = state;
                    chbLabel.Checked = false;
                    chbDatabase.Checked = false;
                    chbFirstRecord.Checked = false;
                    break;
                case EventType.JOBSTOPPED:
                    break;
                case EventType.JOBSTARTED:
                    chbStart.Checked = state;
                    break;
                case EventType.IOSET:
                    break;
                case EventType.PRINTSTARTED:
                    break;
                case EventType.LOADEDDATABASE:
                    chbDatabase.Checked = state;
                    chbLabel.Checked = false;
                    chbFirstRecord.Checked = false;
                    break;
                case EventType.SETLABEL:
                    chbLabel.Checked = state;
                    break;
                case EventType.SENDFIRSTRECORD:
                    chbFirstRecord.Checked = state;
                    break;
                default:
                    chbConnect.Checked = state;
                    chbJob.Checked = state;
                    chbDatabase.Checked = state;
                    chbLabel.Checked = state;
                    chbFirstRecord.Checked = state;
                    chbStart.Checked = state;
                    chbHelp_7.Checked = state;
                    break;
            }
        }
        #endregion

        #region CONTROLS Methods

        private void tileConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = cmbIpAddress.SelectedItem.ToString();
                if (string.IsNullOrEmpty(ip))
                {
                    ShowProblemSolution(ErrorType.nullIPAddress);
                }
                else
                {
                    rjDevice.RJConnect(ip);
                }
            }
            catch (Exception) {
                ShowProblemSolution(ErrorType.nullIPAddress);
            }
        }

        private void tileDisconnect_Click(object sender, EventArgs e)
        {
            rjDevice.RJDisconnect();
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
            try
            {
                string job = cmbJobFiles.SelectedItem.ToString();
                if (string.IsNullOrEmpty(job))
                {
                    ShowProblemSolution(ErrorType.nullJob);
                    return;
                }
                rjDevice.RJSetJob(job);
            }
            catch (Exception)
            {
                ShowProblemSolution(ErrorType.nullJob);
            }

        }

        private void tileSetIOFiles_Click(object sender, EventArgs e)
        {
            try
            {
                string io = cmbIOFiles.SelectedItem.ToString();
                if (string.IsNullOrEmpty(io))
                {
                    ShowProblemSolution(ErrorType.nullIO);
                    return;
                }
                rjDevice.RJSetIOConfiguration(io);
            }
            catch (Exception)
            {
                ShowProblemSolution(ErrorType.nullIO);
            }
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

        private void tileSendData_Click(object sender, EventArgs e)
        {
            if (listCSVContent.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Zaznacz rekord, który chcesz wydrukować!", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateCheckList(EventType.SENDFIRSTRECORD, false);
            }
            else
            {
                double actualItem = listCSVContent.Items.IndexOf(listCSVContent.SelectedItems[0]) + 1;

                rjDevice.SetPrinterControlParameters(
                    radioPrintTrigger.Checked,
                    chbInvalidContent.Checked,
                    chbPrintReject.Checked,
                    chbPrintAborted.Checked,
                    chbPrintSpeedError.Checked,
                    chbMissingContent.Checked,
                    chbBufferFull.Checked);

                //rjDevice.showEventResponseWithStatusOk = chbShowEventResponseWithStatusOk.Checked;

                UpdateCheckList(EventType.SENDFIRSTRECORD, true);

                rjDevice.ManualSendRecordToPrint(actualItem);
            }
        }

        private void tileLoadLabelSettings_Click(object sender, EventArgs e)
        {
            if (rjDevice.GetVariableContentsCount() <= 0)
            {
                MessageBox.Show("Brak wczytanej etykiety!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateCheckList(EventType.SETLABEL, false);
                return;
            }

            if (rjDevice.GetDatabaseMaxRecords() <= 0)
            {
                MessageBox.Show("Wpierw wczytaj plik z bazą danych!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateCheckList(EventType.SETLABEL, false);
                return;
            }

            string fileName = OpenXmlFile();
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                rjDevice.AssignSettingsToVariableContents(fileName);

                LoadVariableContentsForLabel();
                LoadOutputsSettings();
                UpdateCheckList(EventType.SETLABEL, true);

                MessageBox.Show("Ustawienia zostały wczytane.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tileSetOutputToColumn_Click(object sender, EventArgs e)
        {
            int labelContentNumber;
            int outputNumber;

            if (string.IsNullOrWhiteSpace(comboOutputs.Text))
            {
                MessageBox.Show("Wybierz wyjście, którą chcesz przypisać do pola etykiety", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (listLabelContents.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Zaznacz pole etykiety, dla którego chcesz przypisać ustawienia.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            labelContentNumber = listLabelContents.Items.IndexOf(listLabelContents.SelectedItems[0]);
            outputNumber = (comboOutputs.SelectedItem as DigitalOutput).Id - 1;

            rjDevice.AssignOutputToDataField(outputNumber, labelContentNumber);
            LoadOutputsSettings();
            LoadVariableContentsForLabel();
        }

        private void tileClearOutputToColumn_Click(object sender, EventArgs e)
        {
            int labelContentNumber;
            if (listLabelContents.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Zaznacz pole etykiety, dla którego chcesz zmienić ustawienia.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            labelContentNumber = listLabelContents.Items.IndexOf(listLabelContents.SelectedItems[0]);

            rjDevice.AssignOutputToDataField(-1, labelContentNumber);
            LoadOutputsSettings();
            LoadVariableContentsForLabel();
        }

        private void tileSetDataCSVToLabel_Click(object sender, EventArgs e)
        {
            int labelContentNumber;
            int dataField;

            if (string.IsNullOrWhiteSpace(comboCSVHeaders.Text))
            {
                MessageBox.Show("Wybierz kolumnę z danymi, którą chcesz przypisać do pola etykiety", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (listLabelContents.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Zaznacz pole etykiety, dla którego chcesz przypisać dane.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            labelContentNumber = listLabelContents.Items.IndexOf(listLabelContents.SelectedItems[0]);
            dataField = (comboCSVHeaders.SelectedItem as Header).Id;

            rjDevice.AssignDataFieldToContent(dataField, labelContentNumber);

            LoadVariableContentsForLabel();
        }

        private void tileClearDataCSVToLabel_Click(object sender, EventArgs e)
        {
            int labelContentNumber;

            if (listLabelContents.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Zaznacz pole etykiety, dla którego chcesz przypisać dane.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            labelContentNumber = listLabelContents.Items.IndexOf(listLabelContents.SelectedItems[0]);

            rjDevice.AssignDataFieldToContent(-1, labelContentNumber);

            LoadVariableContentsForLabel();
        }

        private void tileLoadCSVFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = databasePath;
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = false;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                if (rjDevice.LoadDatabase(openFileDialog.FileName) == true)
                {
                    txtFileName.Text = openFileDialog.FileName;

                    listCSVContent.Items.Clear();
                    listCSVContent.Columns.Clear();
                    comboCSVHeaders.Items.Clear();

                    MessageBox.Show("Baza została poprawnie wczytana do pamięci.\n Naciśnij OK aby wyświetlić dane.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ShowDatabaseInListView();
                    UpdateCheckList(EventType.LOADEDDATABASE, true);

                    LoadVariableContentsForLabel();
                }
            }
            catch (Exception ex)
            {
                checkSubitems.Enabled = false;
                checkPartialMatches.Enabled = false;
                progressBarCSVLoading.Visible = false;

                tileFindRecord.Enabled = false;
                tileSendData.Enabled = false;
                comboCSVHeaders.Items.Clear();

                UpdateCheckList(EventType.LOADEDDATABASE, false);

                MessageBox.Show("Błąd podczas wczytywania bazy danych.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppLogger.GetLogger().Error("Błąd wczytywania bazy danych", ex);
                return;
            }
        }

        private void tileSaveLabelSettings_Click(object sender, EventArgs e)
        {
            int variableContentsCount = rjDevice.GetVariableContentsCount();
            if (variableContentsCount <= 0)
            {
                MessageBox.Show("Brak wczytanej etykiety!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateCheckList(EventType.SETLABEL, false);
                return;
            }

            if (rjDevice.GetDatabaseMaxRecords() <= 0)
            {
                MessageBox.Show("Nie można zapisać danych jeżeli nie ma wczytanej bazy danych!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateCheckList(EventType.LOADEDDATABASE, false);
                return;
            }

            Stream file;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = labelSettingsPath;
            saveFileDialog.Filter = "Label settings files (*.xml)|*.xml";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            if ((file = saveFileDialog.OpenFile()) != null)
            {
                try
                {
                    LabelSettings labelSettings = new LabelSettings();
                    labelSettings.Save(file, txtFileName.Text, cmbJobFiles.Text, rjDevice.GetDTOLabelSettings());

                    txtLabelSettings.Text = saveFileDialog.FileName;
                    UpdateCheckList(EventType.SETLABEL, true);

                    DisplayLogsAndErrors("Ustawienia etykiety - zapis: " + txtLabelSettings.Text, null, null);
                }
                catch (Exception ex)
                {
                    string info = "Błąd podczas zapisu ustawień etykiety. Spróbuj ponownie! ";
                    MessageBox.Show(info, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    DisplayLogsAndErrors("Zapis ustawień etykiety", null, ex.ToString());
                    UpdateCheckList(EventType.SETLABEL, false);

                    AppLogger.GetLogger().Error("Błąd zapisu ustawień etykiety.", ex);
                }
            }
        }

        #endregion

        #region TEST
        private void button1_Click(object sender, EventArgs e)
        {
            rjDevice.RJConnect("10.10.2.1");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rjDevice.RJDisconnect();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            rjDevice.RJSetJob("test_LK.job");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            rjDevice.RJStartJob();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            rjDevice.RJStopJob();
        }
        #endregion


    }
}
