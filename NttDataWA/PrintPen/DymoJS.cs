using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace printPen
{
    internal class DymoJS
    {

        static private string addDebugScript(bool debug, string debugMsg) { 
            string debugScript = "";

            if (debug)
            {
                debugScript = "alert('" + debugMsg + "');" + Environment.NewLine;
            }
            
            return debugScript;
        }

        static internal string getDymoBaseUrl()
        {
            return "http://labelwriter.com/software/dls/sdk/js/DYMO.Label.Framework.latest.js";
        }

        static internal string getDymoPrintScript(Dictionary<string,string> dymoMap, string dymoXml, short numeroStampe, bool debug)
        {
            string dymoScript = "";

            dymoScript += "function dymoPrint(){" + Environment.NewLine;
            dymoScript += "try{" + Environment.NewLine;
            
            dymoScript += addDebugScript(debug, "after try");

            dymoXml = dymoXml.Replace(Environment.NewLine, "\\" + Environment.NewLine);
            
            // se firefox uso il framework linkato da pagina principale, unico modo x farlo andare al momento
            // se usiamo sempre quello poi IE da un errore JS e non stampa
            dymoScript += "var dymoFr;";
            dymoScript += "if(navigator.userAgent.toLowerCase().indexOf('firefox') > -1)";
            dymoScript += "dymoFr = parent.dymo.label.framework" + Environment.NewLine;
            dymoScript += "else dymoFr = dymo.label.framework" + Environment.NewLine;

            dymoScript += "var dymoXML = '" + dymoXml + "';" + Environment.NewLine;
            dymoScript += addDebugScript(debug, "prima della chiamata al label framework");
            dymoScript += "var label = dymoFr.openLabelXml(dymoXML);" + Environment.NewLine;
            dymoScript += addDebugScript(debug, "dopo della chiamata al label framework");
            
            foreach (KeyValuePair<String,String> entry in dymoMap){
                dymoScript += "try{ ";
                dymoScript += "label.setObjectText('" + entry.Key + "', '" + entry.Value + "');" + Environment.NewLine;
                dymoScript += "}catch(e){" + (debug ? "alert(e.message || e);" : "") + "}" + Environment.NewLine;
            }
            
            dymoScript += "var printers = dymoFr.getPrinters(); " + Environment.NewLine;
            dymoScript += "if (printers.length == 0)" + Environment.NewLine;
            dymoScript += "throw 'Non è stata rilevata nessuna stampante DYMO';" + Environment.NewLine;

            dymoScript += "var printerName = '';" + Environment.NewLine;
            dymoScript += "for (var i = 0; i < printers.length; ++i){" + Environment.NewLine;
            dymoScript += "var printer = printers[i];" + Environment.NewLine;
            dymoScript += "if (printer.printerType == 'LabelWriterPrinter'){" + Environment.NewLine;
            dymoScript += "     printerName = printer.name;" + Environment.NewLine;
            dymoScript += "     if (printers.length > 1 && printer.name.toLowerCase().indexOf('dymo') > -1 ) { break; } //mod ctn: esce quando trova una stampante con dymo nel nome e ci sono + stampanti" + Environment.NewLine;
            dymoScript += "}" + Environment.NewLine;
            dymoScript += "}" + Environment.NewLine;
                
            dymoScript += "if (printerName == '')" + Environment.NewLine;
            dymoScript += "throw 'Non è stata rilevata nessuna stampante LabelWriter';" + Environment.NewLine;

            // stampaMultiplaDymo
            dymoScript += "for (var i=0; i<" + numeroStampe.ToString() + "; i++){" + Environment.NewLine;
            dymoScript += "label.print(printerName);" + Environment.NewLine;
            dymoScript += (debug ? "alert('stampa ' + (i+1).toString());" : "") + Environment.NewLine;
            dymoScript += "}" + Environment.NewLine;
            
            
            dymoScript += "}" + Environment.NewLine;
            dymoScript += "catch(e){" + Environment.NewLine;
            dymoScript += "alert(e.message || e);" + Environment.NewLine;
            dymoScript += "}" + Environment.NewLine;

            dymoScript += addDebugScript(debug, "before end");
         
            dymoScript += "}" + Environment.NewLine;

            
            return dymoScript;
        }
    }

}