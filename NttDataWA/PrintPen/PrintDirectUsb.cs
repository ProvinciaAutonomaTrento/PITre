using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace printPen
{
    class PrintDirectUsb
    {

        private struct DOCINFO
        {
            public string pDocName;
            public string pOutputFile;
            public string pDatatype;
        }

        //UPGRADE_WARNING: Structure DOCINFO may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
        //UPGRADE_ISSUE: Declaring a parameter 'As Any' is not supported. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'

        // ERROR: Not supported in C#: DeclareDeclaration
        // ERROR: Not supported in C#: DeclareDeclaration
        // ERROR: Not supported in C#: DeclareDeclaration
        // ERROR: Not supported in C#: DeclareDeclaration
        // ERROR: Not supported in C#: DeclareDeclaration
        // ERROR: Not supported in C#: DeclareDeclaration
        // ERROR: Not supported in C#: DeclareDeclaration
        public void PrintDirect(string printerDeviceName, string dataToPrint)
        {
            int lhPrinter = 0;
            int lReturn = 0;
            int lpcWritten = 0;
            int lDoc = 0;
            DOCINFO MyDocInfo = default(DOCINFO);

            // Accodamento di un carattere di carriage return, altrimenti non stampa
            dataToPrint = dataToPrint + Environment.NewLine;

            //lReturn = OpenPrinter(printerDeviceName, ref lhPrinter, 0);

            if (lReturn == 0)
            {
                //Interaction.MsgBox("The Printer Name you typed wasn't recognized.");
                return;
            }

            MyDocInfo.pDocName = "PrintUSB";
            MyDocInfo.pOutputFile = "";
            MyDocInfo.pDatatype = "";

            //lDoc = StartDocPrinter(lhPrinter, 1, ref MyDocInfo);
            //StartPagePrinter(lhPrinter);

            //lReturn = WritePrinter(lhPrinter, ref dataToPrint, Strings.Len(dataToPrint), ref lpcWritten);

            //lReturn = EndPagePrinter(lhPrinter);
            //lReturn = EndDocPrinter(lhPrinter);

            //lReturn = ClosePrinter(lhPrinter);
        }
    }
}
