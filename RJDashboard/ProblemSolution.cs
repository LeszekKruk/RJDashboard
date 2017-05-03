using MetroFramework.Forms;
using RJController.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJDashboard
{
    public partial class ProblemSolution : MetroForm
    {
        public ProblemSolution(ErrorType type)
        {
            InitializeComponent();
            ShowInfo(type);
        }

        private void tileClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ShowInfo(ErrorType type)
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
                case ErrorType.libraryError:
                    errorDescription = "BRAK BIBLIOTEK - ReaPI ";
                    errorSolution = "1. Sprawdź czy uruchomiłeś aplikację z właściwej lokalizacji.\n";
                    errorSolution += "2. Przeinstaluj oprogramowanie.\n";
                    break;
                case ErrorType.nullJob:
                    errorDescription = "Brak wybranego zadania.";
                    errorSolution = "Wybierz zadanie, które chcesz załadować na sterownik.\n";
                    errorSolution += "Wybierz przycisk 'Ustaw'";
                    break;
                default:
                    errorDescription = "STOP Job";
                    errorSolution = "";
                    break;
            }

            lblErrorType.Text = errorDescription;
            lblErrorSolution.Text = errorSolution;

        }
    }
}
