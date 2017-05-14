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
                    //UpdateChecklist()
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

                        SetBackgroundColorForConnectionStatus(true);
                        
                        break;

                    case EventType.DISCONNECT:
                        ClearAndSetControlAfterConnections(false);
                        SetBackgroundColorForConnectionStatus(false);
                        SetBackgroundColorForJobStatus(false);
                        tabControl.Enabled = false;

                        break;

                    case EventType.JOBSET:
                        LoadAndSetJobFiles(rjDevice.GetJobFiles(), rjDevice.GetActualJobFile());
                        LoadAndSetIOFiles(rjDevice.GetIOFiles(), rjDevice.GetActualIOFile());
                        LoadVariableContentsForLabel();
                        LoadOutputsSettings();
                        break;

                    case EventType.JOBSTOPPED:
                        SetBackgroundColorForJobStatus(false);
                        cmbJobFiles.Enabled = true;
                        cmbIOFiles.Enabled = true;
                        break;

                    case EventType.JOBSTARTED:
                        SetBackgroundColorForJobStatus(true);
                        cmbJobFiles.Enabled = false;
                        cmbIOFiles.Enabled = false;
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
                listViewItem.SubItems.Add(dto[i].OutputControl.ToString());

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

            IDictionary<int, DigitalOutput> outputs = rjDevice.GetDigitalOutputs();

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

        #endregion

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

        private void tileSendData_Click(object sender, EventArgs e)
        {
            if (listCSVContent.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Zaznacz rekord, który chcesz wydrukować!", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //UpdateCheckList(helper.FirstRecord, false);
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
                //UpdateCheckList(helper.FirstRecord, true);

                rjDevice.ManualSendRecordToPrint(actualItem);
            }
        }

        private void tileLoadLabelSettings_Click(object sender, EventArgs e)
        {
            if (rjDevice.GetVariableContentsCount() <= 0)
            {
                MessageBox.Show("Brak wczytanej etykiety!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //UpdateCheckList(helper.Label, false);
                return;
            }

            if (rjDevice.GetDatabaseMaxRecords() <= 0)
            {
                MessageBox.Show("Wpierw wczytaj plik z bazą danych!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //UpdateCheckList(helper.Label, false);
                //return;
            }

            string fileName = OpenXmlFile();
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                rjDevice.AssignSettingsToVariableContents(fileName);

                LoadVariableContentsForLabel();
                LoadOutputsSettings();

                MessageBox.Show("Ustawienia zostały wczytane.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string OpenXmlFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = labelSettingsPath;
            openFileDialog.Filter = "Label settings files (*.xml)|*.xml";
            //openFileDialog.Filter = "All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = false;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return null;

            return openFileDialog.FileName;            
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
        }

        private void LoadToComboDigitalOutputs()
        {
            comboOutputs.Items.Clear();

            IDictionary<int, DigitalOutput> outputs = rjDevice.GetDigitalOutputs();

            for (int i = 0; i < outputs.Count; i++)
            {
                comboOutputs.Items.Add(outputs[i]);
            }
        }

        private void LoadToComboCSVHeaders()
        {
            comboCSVHeaders.Items.Clear();
            List<Header> headers = rjDevice.GetHeaders();

            for (int i = 0; i < headers.Count; i++)
            {
                comboCSVHeaders.Items.Add(headers[i]);
            }
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

            if (rjDevice.LoadDatabase(openFileDialog.FileName) == true)
            {
                listCSVContent.Items.Clear();
                listCSVContent.Columns.Clear();

                comboCSVHeaders.Items.Clear();

                MessageBox.Show("Baza została poprawnie wczytana do pamięci.\n Naciśnij OK aby wyświetlić dane.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //LoadCSVDataToListView();

                //UpdateCheckList(helper.Database, true);

                LoadVariableContentsForLabel();
            }     
        }

        /*
        private void ParseCSVDataSource(string fileName)
        {

            if (reaJetControl._csvDatabase.ClearDatabase() == false)
            {
                MessageBox.Show("Błąd przygotowania danych!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateCheckList(helper.Database, false);
                return;
            }

            //reaJetControl._listLabelProperty[index].AssignedColumnToLoadCSVData



            //dołożone w wersji 1.07 - czyszcze przypisane kolumny po załadowaniu bazy
            reaJetControl.ClearAssignedColumnToLoadCSVDataInListLabelProperty();
            reaJetControl.ClearAssignedOutpuInListLabelProperty();

            //    ListView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
            progressBarCSVLoading.Visible = true;
            progressBarCSVLoading.Minimum = 0;
            progressBarCSVLoading.Maximum = 100;

            const char fieldSeparator = ';';
            bool createHeader = true;
            string logInfo = "";
            try
            {
                using (var sr = new StreamReader(fileName, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
                {
                    DisplayEvents("Baza: " + fileName);

                    FileInfo fileCSV = new FileInfo(fileName);
                    long fileSize = fileCSV.Length;
                    long currentSize = 0;

                    long row = 0;
                    int numberOfColumns = 0;

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        currentSize += line.Length;

                        UpdateProgressBar(currentSize, fileSize);

                        List<string> record = line.Split(fieldSeparator).ToList();

                        if (createHeader)
                        {
                            numberOfColumns = record.Count();
                            reaJetControl._csvDatabase.AddCSVDataHeaders(record);
                            createHeader = false;
                        }
                        else
                        {
                            row += 1;
                            if (numberOfColumns == record.Count)
                            {
                                reaJetControl._csvDatabase.AddRecord(record);
                            }
                            else
                            {
                                logInfo += " - wiersz nr: " + row.ToString() + "\n";
                                DisplayEvents("Niezgodność bazy - wiersz: " + row.ToString());

                                Logger logger = LogManager.GetCurrentClassLogger();
                                logger.Error("Brak zgodności rekordu: " + row.ToString());
                            }
                        }
                    }
                }

                UpdateProgressBar(1.0, 1.0);

                if (logInfo != "")
                {
                    MessageBox.Show("Podczas wczytywania bazy wystąpiła niezgodność w zawartości danych.\nPoniższe wiersze nie zostały wczytane: \n" + logInfo);
                }
                MessageBox.Show("Baza została poprawnie wczytana do pamięci.\n Naciśnij OK aby wyświetlić dane.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                progressBarCSVLoading.Visible = false;

                LoadCSVDataToListView();
                UpdateCheckList(helper.Database, true);
            }
            catch (Exception ex)
            {
                _actualCSVFile = "";
                txtCSVFileName.Text = (string)null;
                checkActivateTrigger.Enabled = false;
                checkSubitems.Enabled = false;
                checkPartialMatches.Enabled = false;
                progressBarCSVLoading.Visible = false;
                tileFindMaxData.Enabled = false;
                tileFindRecord.Enabled = false;
                tileSendData.Enabled = false;
                comboCSVHeaders.Items.Clear();

                UpdateCheckList(helper.Database, false);

                string info = "Błąd otwarcia pliku: " + ex.ToString();
                MessageBox.Show(info, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error(ex, "Błąd otwarcia pliku: " + ex.ToString());

                return;
            }
            //checkActivateCSVSendOnTrigger.Enabled = listCSVContent.Items.Count > 0;
        }
        */
    }
}
