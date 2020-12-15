using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using System.Xml;
using DocsPAWA.DocsPaWR;
using log4net;

namespace gestione.piani
{
    /// <summary>
    /// Summary description for PdfReport.
    /// </summary>
    public class XlsReport
    {
        private static ILog logger = LogManager.GetLogger(typeof(XlsReport));
        #region Crea xls --> file
        public static DocsPAWA.DocsPaWR.FileDocumento CreaReportPianiRientro(Page page, int id_registro, string registro, string dataInizio, string dataFine)
        {
            string temporaryXSLFilePath = string.Empty;
            StreamWriter writer = null;
            StringBuilder sb = new StringBuilder();
            int res = 0;
            DocsPAWA.DocsPaWR.FileDocumento file = new DocsPAWA.DocsPaWR.FileDocumento();
            
            try
            {
                //Creazione stringa XML
                sb = creaXML(id_registro, registro, dataInizio, dataFine, out res);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath(@"../../xls/PianiDiRientro-" + registro + ".xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Report");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "PianiDiRientro-" + registro + ".xls");

                writer = new StreamWriter(temporaryXSLFilePath, true);
                writer.AutoFlush = true;
                writer.WriteLine(sb.ToString());
                writer.Flush();
                writer.Close();

                //crea il file 
                FileStream stream = new FileStream(temporaryXSLFilePath, FileMode.Open, FileAccess.Read);
                if (stream != null)
                {
                    byte[] contentExcel = new byte[stream.Length];
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    file.content = contentExcel;
                    file.length = contentExcel.Length;
                    file.estensioneFile = "xls";
                    file.name = "PianiRientro";
                    file.contentType = "application/vnd.ms-excel";
                }
                File.Delete(temporaryXSLFilePath);

                //gestione errore
                if (res != 0)
                {
                    throw new Exception("Si è verificato un errore nella creazione del report");
                }
                else
                    if (res == 0 && file == null)
                    {
                        page.Response.Write("<script>alert('Non ci sono dati per il Rapporto selezionato')</script>");
                        return null;
                    }
                
            }
            catch (Exception ex)
            {
                file = null;
                logger.Error(ex);
                DocsPAWA.ErrorManager.redirect(page, ex);
            }
            return file;
        }

        private static StringBuilder creaXML(int id_registro, string registro, string dataInizio, string dataFine, out int res)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            strXML += topXML();
            strXML += stiliXML();
            strXML += sheetProvvConScadenzaXML(id_registro, registro, dataInizio, dataFine, out res);
            strXML += sheetProvvSenzaScadenzaXML(id_registro, registro, dataInizio, dataFine, out res);
            strXML += sheetProvvFuoriPianoXML(id_registro, registro, dataInizio, dataFine, out res);
            strXML += "</Workbook>";
            sb.Append(strXML.ToString());
            return sb;
        }

      
        private static string sheetProvvConScadenzaXML(int id_registro, string registro, string dataInizio, string dataFine, out int res)
        {
            string strXML = string.Empty;
            
            //Recupero dati e creazione dataset
            DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DataSet ds = new DataSet();
            ds = docsPaWS.ReportPianiRientro(id_registro, dataInizio, dataFine, out res);
            
            strXML = "<Worksheet ss:Name=\"Provv. con Scad.\">";
            strXML += creaTabella();
            strXML += creaTotali(ds, true);
            strXML += datiXML(ds);
            strXML += "</Table>";
            strXML += workSheetOptionsXML(registro, dataInizio, dataFine, "Provvedimenti previsti da piano con scadenza");
            return strXML;
        }

        private static string sheetProvvSenzaScadenzaXML(int id_registro, string registro, string dataInizio, string dataFine, out int res)
        {
            string strXML = string.Empty;

            //Recupero dati e creazione dataset
            DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DataSet ds = new DataSet();
            ds = docsPaWS.ReportPianiRientroSenzaScadenza(id_registro, dataInizio, dataFine, out res);

            strXML = "<Worksheet ss:Name=\"Provv. senza Scad.\">";
            strXML += creaTabella();
            strXML += creaTotali(ds, false);
            strXML += datiXML(ds);
            strXML += "</Table>";
            strXML += workSheetOptionsXML(registro, dataInizio, dataFine, "Provvedimenti previsti da piano senza scadenza");
            return strXML;
        }

        private static string sheetProvvFuoriPianoXML(int id_registro, string registro, string dataInizio, string dataFine, out int res)
        {
            string strXML = string.Empty;

            //Recupero dati e creazione dataset
            DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DataSet ds = new DataSet();
            ds = docsPaWS.ReportPianiRientroFuoriPiano(id_registro, dataInizio, dataFine, out res);

            strXML = "<Worksheet ss:Name=\"Provv. fuori piano\">";
            strXML += creaTabella();
            strXML += creaTotali(ds, false);
            strXML += datiXML(ds);
            strXML += "</Table>";
            strXML += workSheetOptionsXML(registro, dataInizio, dataFine, "Provvedimenti non previsti da piano");
            return strXML;
        }

        private static string creaTabella()
        {
            string strXML = string.Empty;
            strXML = "<Table  x:FullColumns=\"1\" x:FullRows=\"1\">";
            strXML += "<Column ss:Width=\"280\"/>";
            strXML += "<Column ss:Width=\"40\"/>";
            strXML += "<Column ss:Width=\"60\"/>";
            strXML += "<Column ss:Width=\"30\"/>";
            strXML += "<Column ss:Width=\"100\"/>";
            strXML += "<Column ss:Width=\"110\"/>";
            strXML += "<Column ss:Width=\"40\"/>";
            strXML += "<Column ss:Width=\"60\"/>";
            strXML += "<Row ss:Height=\"13.5\"/>";
            return strXML;
        }

        private static string creaTotali(DataSet ds, bool scadenza)
        {
            string strXML = string.Empty;
            int totIstruttoria = 0;
            int totTrasmessiMef = 0;
            int totRicevutiMef = 0;
            int totTrasmessiReg = 0;
            int tot = 0;
            if (ds.Tables["ISTRUTTORIA"] != null)
                totIstruttoria = ds.Tables["ISTRUTTORIA"].Rows.Count;
            if (ds.Tables["TRASMESSI_MEF"] != null)
                totTrasmessiMef = ds.Tables["TRASMESSI_MEF"].Rows.Count;
            if (ds.Tables["RICEVUTI_MEF"] != null)
                totRicevutiMef = ds.Tables["RICEVUTI_MEF"].Rows.Count;
            if (ds.Tables["TRASMESSI_REGIONE"] != null)
                totTrasmessiReg = ds.Tables["TRASMESSI_REGIONE"].Rows.Count;
            if (scadenza)
            {
                strXML += "<Row>";
                strXML += "<Cell ss:StyleID=\"s22\">";
                strXML += "<Data ss:Type=\"String\">Totale Provvedimenti attesi nel periodo di riferimento";
                strXML += "</Data>";
                strXML += "</Cell>";
                strXML += "<Cell ss:StyleID=\"s24\">";
                tot = totIstruttoria + totTrasmessiMef + totRicevutiMef + totTrasmessiReg;
                strXML += "<Data ss:Type=\"Number\">" + tot;
                strXML += "</Data>";
                strXML += "</Cell>";
                strXML += "<Cell ss:StyleID=\"s23\"/>"; 
                strXML += "<Cell ss:StyleID=\"s23\"/>"; 
                strXML += "<Cell ss:StyleID=\"s23\"/>";
                strXML += "<Cell ss:StyleID=\"s23\"/>";
                strXML += "<Cell ss:StyleID=\"s23\"/>";
                strXML += "<Cell ss:StyleID=\"s25\"/>";
                strXML += "</Row>";

                strXML += "<Row>";
                strXML += "<Cell ss:StyleID=\"s26\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s28\"/>";
                strXML += "</Row>";

                strXML += "<Row>";
                strXML += "<Cell ss:StyleID=\"s29\">";
                strXML += "<Data ss:Type=\"String\">... Di cui";
                strXML += "</Data>";
                strXML += "</Cell>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s28\"/>";
                strXML += "</Row>";

                strXML += "<Row>";
                strXML += "<Cell ss:StyleID=\"s26\">";
                strXML += "<Data ss:Type=\"String\">Totale Provvedimenti in istruttoria MdS: ";
                strXML += "</Data>";
                strXML += "</Cell>";
                strXML += "<Cell ss:StyleID=\"s21\">";
                strXML += "<Data ss:Type=\"Number\">" + totIstruttoria;
                strXML += "</Data>";
                strXML += "</Cell>";
                //strXML += "TOTALE_ISTRUTTORIA";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s27\"/>";
                strXML += "<Cell ss:StyleID=\"s28\"/>";
                strXML += "</Row>";
            }
            else
            {
                strXML += "<Row>";
                strXML += "<Cell ss:StyleID=\"s62\">";
                strXML += "<Data ss:Type=\"String\">Totale Provvedimenti in istruttoria MdS: ";
                strXML += "</Data>";
                strXML += "</Cell>";
                strXML += "<Cell ss:StyleID=\"s24\">";
                strXML += "<Data ss:Type=\"Number\">" + totIstruttoria;
                strXML += "</Data>";
                strXML += "</Cell>";
                //strXML += "TOTALE_ISTRUTTORIA";
                strXML += "<Cell ss:StyleID=\"s23\"/>";
                strXML += "<Cell ss:StyleID=\"s23\"/>";
                strXML += "<Cell ss:StyleID=\"s23\"/>";
                strXML += "<Cell ss:StyleID=\"s23\"/>";
                strXML += "<Cell ss:StyleID=\"s23\"/>";
                strXML += "<Cell ss:StyleID=\"s25\"/>";
                strXML += "</Row>";
            }
            strXML += "<Row>";
            strXML += "<Cell ss:StyleID=\"s26\">";
            strXML += "<Data ss:Type=\"String\">Totale Provvedimenti trasmessi al MEF per il concerto tecnico: ";
            strXML += "</Data>";
            strXML += "</Cell>";
            strXML += "<Cell ss:StyleID=\"s21\">";
            strXML += "<Data ss:Type=\"Number\">" + totTrasmessiMef; 
            strXML += "</Data>";
            strXML += "</Cell>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s28\"/>";
            strXML += "</Row>";

            strXML += "<Row>";
            strXML += "<Cell ss:StyleID=\"s26\">";
            strXML += "<Data ss:Type=\"String\">Totale Provvedimenti ricevuti dal MEF: ";
            strXML += "</Data>";
            strXML += "</Cell>";
            strXML += "<Cell ss:StyleID=\"s49\">";
            strXML += "<Data ss:Type=\"Number\">" + totRicevutiMef; 
            strXML += "</Data>";
            strXML += "</Cell>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s28\"/>";
            strXML += "</Row>";

            strXML += "<Row ss:Height=\"13.5\">";
            strXML += "<Cell ss:StyleID=\"s30\">";
            strXML += "<Data ss:Type=\"String\">Totale Provvedimenti trasmessi alla Regione: ";
            strXML += "</Data>";
            strXML += "</Cell>";
            strXML += "<Cell ss:StyleID=\"s32\">";
            strXML += "<Data ss:Type=\"Number\">" + totTrasmessiReg;
            strXML += "</Data>";
            strXML += "</Cell>";
            //strXML += "TOTALE_TRASMESSI_REGIONE";
            strXML += "<Cell ss:StyleID=\"s31\"/>";
            strXML += "<Cell ss:StyleID=\"s31\"/>";
            strXML += "<Cell ss:StyleID=\"s31\"/>";
            strXML += "<Cell ss:StyleID=\"s31\"/>";
            strXML += "<Cell ss:StyleID=\"s31\"/>";
            strXML += "<Cell ss:StyleID=\"s33\"/>";
            strXML += "</Row>";

            strXML += "<Row ss:Height=\"13.5\">";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "<Cell ss:StyleID=\"s27\"/>";
            strXML += "</Row>";

            return strXML;
        }

        private static string workSheetOptionsXML(string registro, string dataInizio, string dataFine, string titolo)
        {
            string strXML = string.Empty;
            strXML = "<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">";
            strXML += "<PageSetup>";
            strXML += "<Layout x:Orientation=\"Landscape\"/>";
            strXML += "<Header x:Margin=\"0.30\" x:Data=\"&amp;C&amp;&quot;Arial,Grassetto&quot;&amp;18" + registro + "&#10;&amp;&quot;Arial,Normale&quot;&amp;10" + titolo + "&#10;Periodo di riferimento: " + dataInizio + " - " + dataFine + "\"/>";
            strXML += "<Footer x:Margin=\"0.30\" x:Data=\"Page &amp;P of &amp;N\"/>";
            strXML += "<PageMargins x:Bottom=\"0.19685039370078741\" x:Left=\"0.19685039370078741\" x:Right=\"0.19685039370078741\" x:Top=\"0.78740157480314965\"/>";
            strXML += "</PageSetup>";
            strXML += "<Print>";
            strXML += "<ValidPrinterInfo/>";
            strXML += "<HorizontalResolution>600</HorizontalResolution>";
            strXML += "<VerticalResolution>600</VerticalResolution>";
            strXML += "</Print>";
            strXML += "<Selected/>";
            strXML += "<ProtectObjects>False</ProtectObjects>";
            strXML += "<ProtectScenarios>False</ProtectScenarios>";
            strXML += "</WorksheetOptions>";
            strXML += "</Worksheet>";

            return strXML;
        }

        private static string datiXML(DataSet ds)
        {
            string strXML = string.Empty;
            strXML = creaColonne();

            strXML += inserisciDati(ds);
            return strXML;
        }

        private static string creaColonne()
        {
            string strXML = string.Empty;
            strXML = "<Row ss:Height=\"13.5\">";
            strXML += "<Cell ss:MergeAcross=\"7\" ss:StyleID=\"s60\">";
            strXML += "<Data ss:Type=\"String\">Lista dei provvedimenti nel periodo di riferimento";
            strXML += "</Data>";
            strXML += "</Cell>";
            strXML += "</Row>";

            strXML += "<Row ss:Height=\"13.5\">";
            //colonna oggetto
            strXML += "<Cell ss:StyleID=\"s50\">";
            strXML += "<Data ss:Type=\"String\">Voce oggettario - scadenza";
            strXML += "</Data>";
            strXML += "</Cell>";
            //colonna numero protocollo
            strXML += "<Cell  ss:StyleID=\"s60\">";
            strXML += "<Data ss:Type=\"String\">Proto ";
            strXML += "</Data>";
            strXML += "</Cell>";
            //colonna data protocollo
            strXML += "<Cell  ss:StyleID=\"s60\">";
            strXML += "<Data ss:Type=\"String\">Data";
            strXML += "</Data>";
            strXML += "</Cell>";
            //colonna tipologia protocollo
            strXML += "<Cell  ss:StyleID=\"s60\">";
            strXML += "<Data ss:Type=\"String\">Tipo";
            strXML += "</Data>";
            strXML += "</Cell>";
            //colonna tipo provvedimento
            strXML += "<Cell ss:StyleID=\"s60\">";
            strXML += "<Data ss:Type=\"String\">Tipo Provvedimento";
            strXML += "</Data>";
            strXML += "</Cell>";
            //colonna stato
            strXML += "<Cell ss:StyleID=\"s60\">";
            strXML += "<Data ss:Type=\"String\">Stato";
            strXML += "</Data>";
            strXML += "</Cell>";
            //colonna num_proto associato
            strXML += "<Cell ss:StyleID=\"s60\">";
            strXML += "<Data ss:Type=\"String\">Rif.";
            strXML += "</Data>";
            strXML += "</Cell>";
            //colonna data protocollo associato
            strXML += "<Cell  ss:StyleID=\"s60\">";
            strXML += "<Data ss:Type=\"String\">Data Rif.";
            strXML += "</Data>";
            strXML += "</Cell>";
            strXML += "</Row>";

            return strXML;

        }

        private static string inserisciDati(DataSet ds)
        {
            string righe = string.Empty;
            if ( (ds.Tables["ISTRUTTORIA"]!=null) && (ds.Tables["ISTRUTTORIA"].Rows.Count > 0))
            {
                foreach (DataRow row in ds.Tables["ISTRUTTORIA"].Rows)
                {
                    righe += inserisciRiga(row, "In istruttoria MDS");
                }
            }
           
            if ((ds.Tables["TRASMESSI_MEF"]!=null) && (ds.Tables["TRASMESSI_MEF"].Rows.Count > 0))
            {
                foreach (DataRow row in ds.Tables["TRASMESSI_MEF"].Rows)
                {
                    righe += inserisciRiga(row, "Trasmessi al MEF");
                }
            }

            if ((ds.Tables["RICEVUTI_MEF"]!=null) && (ds.Tables["RICEVUTI_MEF"].Rows.Count > 0))
            {
                foreach (DataRow row in ds.Tables["RICEVUTI_MEF"].Rows)
                {
                    righe += inserisciRiga(row, "Ricevuti dal MEF");
                }
            }

            if ((ds.Tables["TRASMESSI_REGIONE"]!=null) && (ds.Tables["TRASMESSI_REGIONE"].Rows.Count > 0))
            {
                foreach (DataRow row in ds.Tables["TRASMESSI_REGIONE"].Rows)
                {
                    righe += inserisciRiga(row, "Trasmessi alla regione");
                }
            }
           
            return righe;
        }

        private static string inserisciRiga(DataRow row, string statoProv)
        {
            string righe = string.Empty;
            string tipo_atto = row["atto"].ToString();
           
            righe = "<Row>";
            //cella oggetto
            righe += "<Cell ss:StyleID=\"s55\" >";
            righe += "<Data ss:Type=\"String\" >" + row["oggetto"];
            righe += "</Data>";
            righe += "</Cell>";
            //cella numero di protocollo
            righe += "<Cell ss:StyleID=\"s21\">";
            righe += "<Data ss:Type=\"Number\">" + row["num_proto"];
            righe += "</Data>";
            righe += "</Cell>";
            //cella data protocollo
            righe += "<Cell ss:StyleID=\"s21\">";
            righe += "<Data ss:Type=\"String\">" + row["dta_proto"].ToString().Remove(10); ;
            righe += "</Data>";
            righe += "</Cell>";
            //cella tipologia protocollo
            righe += "<Cell ss:StyleID=\"s21\">";
            righe += "<Data ss:Type=\"String\"> " + row["tipo_proto"];
            righe += "</Data>";
            righe += "</Cell>";
            //cella descrizione atto
            righe += "<Cell ss:StyleID=\"s56\">";
            if (statoProv.Equals("Trasmessi alla regione"))
                if (tipo_atto.Equals("PARERE"))
                    tipo_atto += " - " + row["stato"];
            righe += "<Data ss:Type=\"String\">" + tipo_atto;
            righe += "</Data>";
            righe += "</Cell>";
            //cella tipo provvedimento
            righe += "<Cell ss:StyleID=\"s21\" >";
            righe += "<Data ss:Type=\"String\">" + statoProv;
            righe += "</Data>";
            righe += "</Cell>";
            //cella numero di protocollo collegato
            righe += "<Cell ss:StyleID=\"s21\">";
            //if ( Convert.ToInt32(row["parent"]) == 0 )
              //  righe += "<Data ss:Type=\"Number\"> " + "";
            //else
            if (statoProv.Equals("Trasmessi alla regione"))
            {
                righe += "<Data ss:Type=\"Number\">" + row["parent"];
            }
            else righe += "<Data ss:Type=\"Number\">";
            righe += "</Data>";
            righe += "</Cell>";
           
            //cella data di protocollo collegato
            righe += "<Cell ss:StyleID=\"s61\">";
            if (statoProv.Equals("Trasmessi alla regione"))
            {
                righe += "<Data ss:Type=\"String\">" + row["parent_dta_proto"].ToString().Remove(10);
            }
            else
            {
                righe += "<Data ss:Type=\"String\">";
            }
            righe += "</Data>";
            righe += "</Cell>";
            righe += "</Row>";
            return righe;
        }

        private static string stiliXML()
        {
            string strXML = string.Empty;
            strXML = "<Styles>";
            strXML += "<Style ss:ID=\"Default\" ss:Name=\"Normal\">";
            strXML += "<Alignment />";
            strXML += "<Borders/>";
            strXML += "<Font/>";
            strXML += "<Interior/>";
            strXML += "<NumberFormat/>";
            strXML += "<Protection/>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s21\">";
            strXML += "<Alignment />";
            strXML += "<Borders>"; 
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            //strXML += "<Style ss:ID=\"s22\" ss:Vertical=\"Top\">";
            strXML += "<Style ss:ID=\"s22\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Bold=\"1\"/>";
            strXML += "<ss:WrapText=\"1\"/>";

            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s23\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s24\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s25\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s26\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s27\">";
            strXML += "<Borders/>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s28\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s29\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Bold=\"1\"/>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s30\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s31\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s32\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s33\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s49\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s50\">";
            strXML += "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Bold=\"1\"/>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s51\">";
            strXML += "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Bold=\"1\"/>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s53\">";
            strXML += "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s54\">";
            strXML += "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s55\" >";
            strXML += "<Alignment  ss:WrapText=\"1\" />";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            //strXML += "<Style ss:ID=\"s56\" ss:Vertical=\"Top\">";
            strXML += "<Style ss:ID=\"s56\" >";
            strXML += "<Alignment ss:WrapText=\"1\"/>";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s58\">";
            strXML += "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s59\">";
            strXML += "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s60\">";
            strXML += "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Bold=\"1\"/>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s61\">";
            strXML += "<Alignment  />";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "<Style ss:ID=\"s62\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            strXML += "</Borders>";
            strXML += "</Style>";
            strXML += "</Styles>";
            return strXML;
        }

        private static string topXML()
        {
            string strXML = string.Empty;
            strXML = "<?xml version=\"1.0\" encoding = \"UTF-16\" ?>";
            strXML += "<?mso-application progid=\"Excel.Sheet\"?>";
            strXML += "<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:html=\"http://www.w3.org/TR/REC-html40\">";
            strXML += "<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">";
            strXML += "<Author></Author>";
            strXML += "<LastAuthor></LastAuthor>";
            strXML += "<LastPrinted></LastPrinted>";
            strXML += "<Created>2007-08-09T07:47:01Z</Created>";
            strXML += "<LastSaved>2007-08-15T07:58:29Z</LastSaved>";
            strXML += "<Company>ETNOTEAM S.p.A.</Company>";
            strXML += "<Version>11.6568</Version>";
            strXML += "</DocumentProperties>";
            strXML += "<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">";
            strXML += "<WindowHeight>8835</WindowHeight>";
            strXML += "<WindowWidth>11340</WindowWidth>";
            strXML += "<WindowTopX>480</WindowTopX>";
            strXML += "<WindowTopY>135</WindowTopY>";
            strXML += "<ProtectStructure>False</ProtectStructure>";
            strXML += "<ProtectWindows>False</ProtectWindows>";
            strXML += "</ExcelWorkbook>";
            return strXML;
        }
        #endregion Fine crea xls --> file




    }
}
