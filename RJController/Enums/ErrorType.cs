using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJController.Enums
{
    public enum ErrorType
    {
        stopJob,
        nullIPAddress,
        errorConnection,
        bufferFull,
        missingContent,
        invalidContent,
        printSpeedError,
        printAborted,
        printReject,
        noRecordToPrint, //brak rekordu do wydruku - prześlij jeszcze raz
        errorRecordToPrint,  //błąd rekordu - jeszcze raz
        printedLastRecord,   //wydrukowano ostatni rekord
        libraryError
    }
}
