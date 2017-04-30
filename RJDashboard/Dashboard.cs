using MetroFramework.Forms;
using RJController;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RJController.Dashboard;
using NLog;
using RJController.Enums;

namespace RJDashboard
{
    public partial class Dashboard : MetroForm, IDashboard
    {
        RJControll rjControll;

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

            rjControll = new RJControll(this);

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
                        Logger logger = LogManager.GetCurrentClassLogger();
                        logger.Error(e, "Błąd podczas czyszczenia zdarzeń");
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
                                Logger logger = LogManager.GetCurrentClassLogger();
                                logger.Error(e, "Błąd podczas czyszczenia logów");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        txtLastError.Clear();
                        _maxLogErrors = 0;
                        Logger logger = LogManager.GetCurrentClassLogger();
                        logger.Error(e, "Błąd zapisu zdarzeń LogsAndErrors");
                    }
                }
            }
        }

        public void DisplayInformationError(ErrorType type)
        {
            if (this.InvokeRequired)
            {
                //Dashboard.DisplayInformationErrorCallback showInformationErrorCallback = new Dashboard.DisplayInformationErrorCallback(this.DisplayInformationError);
                Action<ErrorType> showInformationErrorCallback = new Action<ErrorType>(this.DisplayInformationError);
                if (!this.Created || this.IsDisposed)
                    return;
                this.Invoke(showInformationErrorCallback, type);
                //showInformationErrorCallback.Invoke(type);
            }
            else
            {
                string errorDescription = "";
                string errorSolution = "";

                switch (type)
                {
                    case ErrorType.stopJob:
                        errorDescription = "STOP Job";
                        errorSolution = "";
                        break;
                    case ErrorType.nullIPAddress:
                        errorDescription = "Brak wybranego adresu IP drukarki.";
                        errorSolution = "Wybierz IP drukarki odpowiadający podłączonemu urządzeniu.\n";
                        errorSolution += "Aby sprawdzić IP drukarki odczytaj go ze sterownika - okno informacyjne.";
                        break;
                    case ErrorType.errorConnection:
                        errorDescription = "Brak komunikacji pomiędzy komputerem a sterownikiem drukarki";
                        errorSolution = "1. Sprawdź czy sterownik drukarki jest włączony.\n";
                        errorSolution += "2. Sprawdź czy wybrałeś właściwe IP drukarki.\n";
                        errorSolution += "3. Sprawdź czy przewód sieciowy jest prawidłowo podłączony do komputera i sterownika drukarki.\n";
                        break;
                    case ErrorType.bufferFull:
                        errorDescription = "BUFFER FULL - przepełniony bufor";
                        errorSolution = "1. Wyłącz / włącz druk JOB'a.\n";
                        errorSolution += "2. Jeżeli to nie pomogło to zrestartuj sterownik drukarki.\n";
                        errorSolution += "3. Utórz nowy zestaw logów poprzez menu Create Log.\n";
                        break;
                    case ErrorType.missingContent:
                        errorDescription = "MISSING CONTENT - utracone dane do wydruku";
                        errorSolution = "1. Sprawdź czy nie nastąpiło rozłączenie komputera ze sterownikiem drukarki.\n";
                        errorSolution += "2. Prześlij jeszcze raz rekord, od którego chcesz zacząć druk.\n";
                        break;
                    case ErrorType.invalidContent:
                        errorDescription = "INVALID CONTENT - nieprawidłowe/brak danych do wydruku ";
                        errorDescription += "\nBłąd najczęściej objawia się tym, że steronwik nie może wygenerować nowej etykiety.\n";
                        errorSolution = "1. Jeżeli na etykiecie występuje kod kreskowy, kod 2D - sprawdź czy dla\ntych danych nie ma pustej komórki z danymi.\n";
                        errorSolution += "2. Ponów próbę przesłania jeszcze raz rekordu do wydruku. \nJeżeli ponownie nastąpi zatrzymanie zadania - pomiń ten rekord, a jego nr, id, itp. prześlij do EPD, KO.\n";
                        break;
                    case ErrorType.printSpeedError:
                        errorDescription = "PRINT SPEED ERROR - zbyt duża prędkość wydruku ";
                        errorDescription += "\nBłąd najczęściej objawia się tym, że sterowik otrzymał do wydruku kolejny rekord,\na wcześniejszy nie został jeszcze wydrukowany\n";
                        errorSolution = "1. Sprawdź czy parametry etykiety są zgodne (np. nie jest za długa ws do egzemplarza).\n";
                        errorSolution += "2. Sprawdź ustawienia konfiguracyjne drukarki (blanking, label offset, odległość czujnika).\n";
                        errorSolution += "3. Sprawdź czy fotokomórka nie jest wyzwalana np. przez ścinki.\n";
                        errorSolution += "4. Zmień w ustawieniach konfiguracyjnych parametr, PrintSpeed na DOUBLE.\n";
                        break;
                    case ErrorType.printAborted:
                        errorDescription = "PRINT ABORTED - przerwany druk ";
                        errorSolution = "1. Jeżeli druk został przerwany przez Operatora to prześlij jeszcze raz rekord do wydruku\n";
                        errorSolution += "2. Z okna 'Zadanie' odczytaj szczegóły błędu PRINT ABORTED i prześlij do mnie.\n";
                        errorSolution += "3. Wyślij jeszcze raz rekord do wydruku.\n";
                        break;
                    case ErrorType.printReject:
                        errorDescription = "PRINT REJECT - odrzucony wydruk, brak danych ";
                        errorSolution = "1. Jeżeli druk został przerwany przez Operatora to prześlij jeszcze raz rekord do wydruku\n";
                        errorSolution += "2. Z okna 'Zadanie' odczytaj szczegóły błędu PRINT REJECT i prześlij do mnie.\n";
                        errorSolution += "3. Wyślij jeszcze raz rekord do wydruku.\n";
                        break;
                    case ErrorType.noRecordToPrint:
                        errorDescription = "RECORD TO PRINT - brak przesłanych przez program danych do wydruku ";
                        errorSolution = "1. Wyślij jeszcze raz rekord który należy wydrukować.\n";
                        break;
                    case ErrorType.errorRecordToPrint:
                        errorDescription = "ERROR RECORD TO PRINT - błąd danych w rekordzie ";
                        errorSolution = "1. Wyślij jeszcze raz rekord który należy wydrukować.\n";
                        errorSolution += "2. Jeżeli błąd wystąpi ponownie pomiń ten rekord, zapisz jego nr i prześlij do KO, EPD.\n";
                        break;
                    case ErrorType.printedLastRecord:
                        errorDescription = "PRINTED LAST RECORD - wydrukowano całą bazę ";
                        errorSolution = "1. Po zakończeniu druku całej bazy usuń plik z danymi z komputera (dla tej bazy).\n";
                        break;
                    default:
                        errorDescription = "STOP Job";
                        errorSolution = "";
                        break;
                }

                Information info = new Information(errorDescription, errorSolution);
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
