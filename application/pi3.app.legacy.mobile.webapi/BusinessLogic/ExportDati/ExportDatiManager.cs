// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;
using System.Data;
using System.Text;

namespace BusinessLogic.ExportDati
{
    public class ExportDatiManager
    {
        private static ILogger logger = Log.ForContext(typeof(ExportDatiManager));

        public DocsPaVO.documento.FileDocumento ExportTitolarioInExcel(string serverPath, DocsPaVO.amministrazione.OrgTitolario titolario, string idRegistro)
        {
            DocsPaVO.documento.FileDocumento file = new DocsPaVO.documento.FileDocumento();

            try
            {
                //Recupero tutti i nodi del titolario selezionato
                DataSet ds_nodiDiTitolario = new DataSet();
                ds_nodiDiTitolario = Amministrazione.TitolarioManager.GetNodiTitolario(titolario, idRegistro);

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXML(ds_nodiDiTitolario);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportTitolario.xls");
                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportTitolario.xls");

                writer = new StreamWriter(temporaryXSLFilePath, true);
                writer.AutoFlush = true;
                writer.WriteLine(sb.ToString());
                writer.Flush();
                writer.Close();

                //Crea il file
                FileStream stream = new FileStream(temporaryXSLFilePath, FileMode.Open, FileAccess.Read);
                if (stream != null)
                {
                    byte[] contentExcel = new byte[stream.Length];
                    var read =stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    file.content = contentExcel;
                    file.length = contentExcel.Length;
                    file.estensioneFile = "xls";
                    file.name = "ExportTitolario";
                    file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                file = null;
                logger.Debug("Errore esportazione titolario : " + ex.Message);
            }

            return file;
        }

        private StringBuilder creaXML(DataSet ds_nodiDiTitolario)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += sheetTitolario(ds_nodiDiTitolario);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());
            return sb;
        }

        private string sheetTitolario(DataSet ds_nodiDiTitolario)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"TITOLARIO\">";
            strXML += "<Table>";
            strXML += creaTabellaTitolario();
            strXML += datiTitolarioXML(ds_nodiDiTitolario);
            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }

        private string topXML()
        {
            string strXML = string.Empty;

            strXML = "<?xml version=\"1.0\" encoding = \"UTF-16\" ?>";
            strXML += "<?mso-application progid=\"Excel.Sheet\"?>";
            strXML += "<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:html=\"http://www.w3.org/TR/REC-html40\">";
            strXML += "<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">";
            strXML += "<Author></Author>";
            strXML += "<LastAuthor></LastAuthor>";
            strXML += "<Created></Created>";
            strXML += "<Company>ETNOTEAM S.p.A.</Company>";
            strXML += "<Version></Version>";
            strXML += "</DocumentProperties>";
            strXML += "<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">";
            strXML += "<WindowHeight>1</WindowHeight>";
            strXML += "<WindowWidth>1</WindowWidth>";
            strXML += "<WindowTopX>1</WindowTopX>";
            strXML += "<WindowTopY>1</WindowTopY>";
            strXML += "<ProtectStructure>False</ProtectStructure>";
            strXML += "<ProtectWindows>False</ProtectWindows>";
            strXML += "</ExcelWorkbook>";
            return strXML;
        }

        private string stiliXML()
        {
            string strXML = string.Empty;

            strXML = "<Styles>";

            strXML += "<Style ss:ID=\"Default\" ss:Name=\"Normal\">";
            strXML += "<Alignment/>";
            strXML += "<Borders/>";
            strXML += "<Font/>";
            strXML += "<Interior/>";
            strXML += "<NumberFormat/>";
            //strXML += "<Protection/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s64\">";
            strXML += "<Alignment ss:Vertical=\"Top\" ss:WrapText=\"1\"/>";
            strXML += "<Font ss:FontName=\"Arial\" x:Family=\"Swiss\" ss:Size=\"8\"/>";
            //strXML += "<NumberFormat ss:Format=\"@\"/>";
            //strXML += "<Protection ss:Protected=\"0\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s63\">";
            strXML += "<Alignment ss:Vertical=\"Top\"/>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"8\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            //strXML += "<Protection ss:Protected=\"0\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s62\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"8\" ss:Color=\"#FFFFFF\" ss:Bold=\"1\"/>";
            strXML += "<Interior ss:Color=\"#993300\" ss:Pattern=\"Solid\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            //strXML += "<Protection ss:Protected=\"0\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s66\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"8\" ss:Color=\"#FFFFFF\" ss:Bold=\"1\"/>";
            strXML += "<Interior ss:Color=\"#FF0000\" ss:Pattern=\"Solid\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            //strXML += "<Protection ss:Protected=\"0\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s67\">";
            strXML += "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"8\" ss:Color=\"#FFFFFF\" ss:Bold=\"1\"/>";
            strXML += "<Interior ss:Color=\"#993300\" ss:Pattern=\"Solid\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            //strXML += "<Protection ss:Protected=\"0\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s68\">";
            strXML += "<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Bottom\"/>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"8\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            //strXML += "<Protection ss:Protected=\"0\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s69\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"10\" ss:Bold=\"1\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            //strXML += "<Protection ss:Protected=\"0\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s70\">";
            strXML += "<Borders>";
            strXML += "<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            strXML += "</Borders>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"10\" ss:Bold=\"1\"/>";
            strXML += "<Interior ss:Color=\"#D8D8D8\" ss:Pattern=\"Solid\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            //strXML += "<Protection ss:Protected=\"0\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s71\">";
            strXML += "<Alignment ss:Vertical=\"Top\"/>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"8\"/>";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            //strXML += "<Protection/>";

            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s21\">";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"20\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s23\">";
            strXML += "<Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Bottom\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s25\">";
            strXML += "<Alignment ss:Vertical=\"Top\"/>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"12\" ss:Bold=\"1\" />";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s27\">";
            strXML += "<Alignment ss:Vertical=\"Top\"/>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"11\" />";
            strXML += "<NumberFormat ss:Format=\"@\"/>";
            strXML += "</Style>";

            strXML += "<Style ss:ID=\"s30\">";
            strXML += "<Alignment ss:Vertical=\"Top\"/>";
            strXML += "<Font x:Family=\"Swiss\" ss:Size=\"8\"/>";
            strXML += "<NumberFormat/>";
            strXML += "</Style>";

            strXML += "</Styles>";

            return strXML;
        }

        private string creaTabellaTitolario()
        {
            string strXML = string.Empty;

            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"26\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"21\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"21\" ss:Span=\"5\"/>";
            strXML += "<Column ss:Index=\"9\" ss:StyleID=\"s63\" ss:AutoFitWidth=\"0\" ss:Width=\"274\"/>";
            strXML += "<Column ss:StyleID=\"s68\" ss:Width=\"60\"/>";
            strXML += "<Column ss:StyleID=\"s68\" ss:Width=\"70\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"80\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"100\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"100\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"100\"/>";
            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableContatoreTitolario()))
            {
                strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"100\"/>";
                strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"100\"/>";
            }

            return strXML;
        }

        private string datiTitolarioXML(DataSet ds_nodiDiTitolario)
        {
            string strXML = string.Empty;
            strXML = creaColonneTitolario();
            strXML += inserisciDatiTitolario(ds_nodiDiTitolario);
            return strXML;
        }

        private string inserisciDatiTitolario(DataSet ds)
        {
            string righe = string.Empty;
            if ((ds.Tables[0] != null) && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                    righe += inserisciRigaTitolario(row);
            }
            return righe;
        }

        private string workSheetOptionsXML()
        {
            string strXML = string.Empty;

            strXML = "<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">";
            strXML += "<Selected/>";
            strXML += "<ProtectObjects>False</ProtectObjects>";
            strXML += "<ProtectScenarios>False</ProtectScenarios>";
            strXML += "<PageSetup>";
            strXML += "<Layout x:Orientation=\"Landscape\"/>";
            strXML += "</PageSetup>";
            strXML += "<Print>";
            strXML += "<ValidPrinterInfo/>";
            strXML += "<HorizontalResolution>600</HorizontalResolution>";
            strXML += "<VerticalResolution>600</VerticalResolution>";
            strXML += "</Print>";
            strXML += "</WorksheetOptions>";

            return strXML;
        }

        private string creaColonneTitolario()
        {
            string strXML = string.Empty;
            strXML += "<Row>";

            //Colonna Amministrazione
            strXML += "<Cell ss:StyleID=\"s66\">";
            strXML += "<Data ss:Type=\"String\">AMM";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Registro
            strXML += "<Cell ss:StyleID=\"s62\">";
            strXML += "<Data ss:Type=\"String\">REG";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Livello 1
            strXML += "<Cell ss:StyleID=\"s66\">";
            strXML += "<Data ss:Type=\"String\">LIV1";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Livello 2
            strXML += "<Cell ss:StyleID=\"s62\">";
            strXML += "<Data ss:Type=\"String\">LIV2";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Livello 3
            strXML += "<Cell ss:StyleID=\"s62\">";
            strXML += "<Data ss:Type=\"String\">LIV3";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Livello 4
            strXML += "<Cell ss:StyleID=\"s62\">";
            strXML += "<Data ss:Type=\"String\">LIV4";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Livello 5
            strXML += "<Cell ss:StyleID=\"s62\">";
            strXML += "<Data ss:Type=\"String\">LIV5";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Livello 6
            strXML += "<Cell ss:StyleID=\"s62\">";
            strXML += "<Data ss:Type=\"String\">LIV6";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Descrizione
            strXML += "<Cell ss:StyleID=\"s66\">";
            strXML += "<Data ss:Type=\"String\">DESCRIZIONE";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Creazione Fascicoli
            strXML += "<Cell ss:StyleID=\"s67\">";
            strXML += "<Data ss:Type=\"String\">CREA_FASC";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Conservazione
            strXML += "<Cell ss:StyleID=\"s67\">";
            strXML += "<Data ss:Type=\"String\">CONSERVAZIONE";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Accesso
            strXML += "<Cell ss:StyleID=\"s62\">";
            strXML += "<Data ss:Type=\"String\">ACCESSO";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Blocco creazione figli
            strXML += "<Cell ss:StyleID=\"s62\">";
            strXML += "<Data ss:Type=\"String\">BLOCCA_CREAZIONE_NODI_FIGLI";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Consenti Classificazione
            strXML += "<Cell ss:StyleID=\"s62\">";
            strXML += "<Data ss:Type=\"String\">CONSENTI_CLASS";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Consenti Fascicolazione
            strXML += "<Cell ss:StyleID=\"s62\">";
            strXML += "<Data ss:Type=\"String\">CONSENTI_FASC";
            strXML += "</Data>";
            strXML += "</Cell>";
            string contatoreTitolario = DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableContatoreTitolario();
            if (!string.IsNullOrEmpty(contatoreTitolario))
            {
                //Colonna Attiva Contatore
                strXML += "<Cell ss:StyleID=\"s62\">";
                strXML += "<Data ss:Type=\"String\">ATTIVA_CONT_" + contatoreTitolario.ToUpper() + "";
                strXML += "</Data>";
                strXML += "</Cell>";

                //Colonna Protocollo Titolario
                strXML += "<Cell ss:StyleID=\"s62\">";
                strXML += "<Data ss:Type=\"String\">" + contatoreTitolario.ToUpper() + "";
                strXML += "</Data>";
                strXML += "</Cell>";
            }

            strXML += "</Row>";
            return strXML;

        }

        private static string inserisciRigaTitolario(DataRow row)
        {
            string riga = string.Empty;
            riga = "<Row>";

            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            string codiceAmm = string.Empty;
            string codiceReg = string.Empty;
            string[] codiceClassifica = null;

            if (row["ID_AMM"].ToString() != null && row["ID_AMM"].ToString() != "")
                codiceAmm = amm.GetVarCodiceAmm(row["ID_AMM"].ToString());
            if (row["ID_REGISTRO"].ToString() != null && row["ID_REGISTRO"].ToString() != "")
                codiceReg = amm.GetCodiceRegistro(row["ID_REGISTRO"].ToString());
            if (row["VAR_CODICE"].ToString() != null && row["VAR_CODICE"].ToString() != "")
                codiceClassifica = row["VAR_CODICE"].ToString().Split('.');

            //Colonna Amministrazione
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + codiceAmm;
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Registro
            if (codiceReg != null && codiceReg != "")
            {
                riga += "<Cell>";
                riga += "<Data ss:Type=\"String\">" + codiceReg;
                riga += "</Data>";
                riga += "</Cell>";
            }
            else
            {
                riga += "<Cell>";
                riga += "<Data ss:Type=\"String\">";
                riga += "</Data>";
                riga += "</Cell>";
            }

            //Imposto il codice classifica per livelli
            if (codiceClassifica != null)
            {
                for (int i = 0; i < codiceClassifica.Length; i++)
                {
                    riga += "<Cell>";
                    riga += "<Data ss:Type=\"String\">" + codiceClassifica[i].ToString();
                    riga += "</Data>";
                    riga += "</Cell>";
                }
                for (int j = 0; j < (6 - codiceClassifica.Length); j++)
                {
                    riga += "<Cell>";
                    riga += "<Data ss:Type=\"String\">";
                    riga += "</Data>";
                    riga += "</Cell>";
                }
            }

            //Colonna Descrizione
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\"><![CDATA[" + row["DESCRIPTION"].ToString() + "]]>";
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Creazione Fascicoli
            if (row["CHA_RW"].ToString() == "W")
            {
                riga += "<Cell>";
                riga += "<Data ss:Type=\"String\">SI";
                riga += "</Data>";
                riga += "</Cell>";
            }
            else
            {
                riga += "<Cell>";
                riga += "<Data ss:Type=\"String\">NO";
                riga += "</Data>";
                riga += "</Cell>";
            }

            //Colonna Conservazione
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + row["NUM_MESI_CONSERVAZIONE"].ToString();
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Accesso
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\"><![CDATA[";
            riga += "]]></Data>";
            riga += "</Cell>";

            //Colonna Blocco creazione figli
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + row["CHA_BLOCCA_FIGLI"].ToString();
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Consenti Classificazione
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + row["CHA_CONSENTI_CLASS"].ToString();
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Consenti fascicolazione
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + row["CHA_CONSENTI_FASC"].ToString();
            riga += "</Data>";
            riga += "</Cell>";

            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableContatoreTitolario()))
            {
                //Colonna Attiva Contatore
                riga += "<Cell>";
                riga += "<Data ss:Type=\"String\">" + row["CHA_CONTA_PROT_TIT"].ToString();
                riga += "</Data>";
                riga += "</Cell>";

                //Colonna Protocollo Titolario
                riga += "<Cell>";
                riga += "<Data ss:Type=\"String\">" + row["NUM_PROT_TIT"].ToString();
                riga += "</Data>";
                riga += "</Cell>";
            }

            riga += "</Row>";
            return riga;
        }

        public DocsPaVO.documento.FileDocumento ExportPianoCons_IntegrPIS(string idAmministrazione)
        {
            DocsPaVO.documento.FileDocumento file = new DocsPaVO.documento.FileDocumento();

            try
            {
                List<DocsPaVO.PianoCons_IntegrPIS> pianoConservazione = BusinessLogic.PianoConservazione.PianoConservazioneManager.SearchPianoCons_IntegrPIS("", "", "", "", "", "", "", "", "", "", "", idAmministrazione);

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLPianoCons_IntegrPIS(pianoConservazione);

                byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());
                using MemoryStream stream = new MemoryStream(byteArray);
                if (stream != null)
                {
                    byte[] contentExcel = new byte[stream.Length];
                    var read = stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    //stream = null;
                    file.content = contentExcel;
                    file.length = contentExcel.Length;
                    file.estensioneFile = "xls";
                    file.name = "ExportPianoCons_IntegrPIS.xls";
                    file.contentType = "application/vnd.ms-excel";
                }
            }
            catch (Exception ex)
            {
                file = null;
                logger.Debug("Errore esportazione PianoCons_IntegrPIS : " + ex.Message);
            }

            return file;
        }

        private StringBuilder creaXMLPianoCons_IntegrPIS(List<DocsPaVO.PianoCons_IntegrPIS> pianoConservazione)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += sheetPianoCons_IntegrPIS(pianoConservazione);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());
            return sb;
        }

        private string sheetPianoCons_IntegrPIS(List<DocsPaVO.PianoCons_IntegrPIS> pianoConservazione)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"Tabella di confine\">";
            strXML += "<Table>";
            strXML += creaTabellaPianoCons_IntegrPIS();
            strXML += datiPianoCons_IntegrPISXML(pianoConservazione);
            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }

        private string creaTabellaPianoCons_IntegrPIS()
        {
            string strXML = string.Empty;

            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"80\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"120\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"120\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"120\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"150\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"120\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"120\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"120\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"120\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"120\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"120\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"120\"/>";
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"120\"/>";

            return strXML;
        }


        private string datiPianoCons_IntegrPISXML(List<DocsPaVO.PianoCons_IntegrPIS> pianoConservazione)
        {
            string strXML = string.Empty;
            strXML = creaColonnePianoCons_IntegrPIS();
            strXML += inserisciDatiPianoCons_IntegrPIS(pianoConservazione);
            return strXML;
        }

        private string creaColonnePianoCons_IntegrPIS()
        {
            string strXML = string.Empty;
            strXML += "<Row>";

            //Colonna Ordinale
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Ordinale";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Azione
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Azione";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Ente
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Codice Amministrazione";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Codice registro
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Codice Registro";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Descrizione integrazione
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Descrizione integrazione";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna CODE APPLICATION
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Code Application";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Classificazione
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Codice classificazione";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna tipologia fascicolo
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Tipologia fascicolo";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Tempo conservazione
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Tempo conservazione";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Utente
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Codice utente";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Ruolo
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Codice ruolo";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Tipo documento
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Tipo documento";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Campo profilato fascicolo
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Campo profilato fascicolo";
            strXML += "</Data>";
            strXML += "</Cell>";

            strXML += "</Row>";
            return strXML;

        }

        private string inserisciDatiPianoCons_IntegrPIS(List<DocsPaVO.PianoCons_IntegrPIS> pianoConservazione)
        {
            StringBuilder righe = new ();
            var counter = 0;
            Dictionary<string, string> dict = new ();

            if (pianoConservazione != null && pianoConservazione.Count > 0)
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                foreach (DocsPaVO.PianoCons_IntegrPIS piano in pianoConservazione) {
                    if (!dict.ContainsKey(piano.IdAmministrazione))
                        dict.Add(piano.IdAmministrazione, amm.GetVarCodiceAmm(piano.IdAmministrazione));
                    righe.Append(inserisciRigaPianoCons_IntegrPIS(piano, dict[piano.IdAmministrazione]));
                    counter++;
                }
            }
            return righe.ToString();
        }

        private static string inserisciRigaPianoCons_IntegrPIS(DocsPaVO.PianoCons_IntegrPIS row, string varCodiceAmministrazione)
        {
            var riga = new StringBuilder();
            riga.Append("<Row>");

            //DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            //Colonna Ordinale
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">");
            riga.Append("</Data>");
            riga.Append("</Cell>");


            //Colonna Azione
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">");
            riga.Append("</Data>");
            riga.Append("</Cell>");

            //Colonna Amministrazione
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">" + varCodiceAmministrazione); // amm.GetVarCodiceAmm(row.IdAmministrazione);
            riga.Append("</Data>");
            riga.Append("</Cell>");

            //Colonna Codice Registro
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">" + row.CodRegistro);
            riga.Append("</Data>");
            riga.Append("</Cell>");

            //Descrizione integrazione
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">" + (!string.IsNullOrEmpty(row.DescIntegrazione) ? row.DescIntegrazione : ""));
            riga.Append("</Data>");
            riga.Append("</Cell>");

            //Colonna CODE APPLICATION
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">" + (!string.IsNullOrEmpty(row.CodeApplication) ? row.CodeApplication : ""));
            riga.Append("</Data>");
            riga.Append("</Cell>");

            //Colonna CLASSIFICAZIONE
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">" + (!string.IsNullOrEmpty(row.CodiceClassifica) ? row.CodiceClassifica : ""));
            riga.Append("</Data>");
            riga.Append("</Cell>");

            //Colonna Tipologia Fascicolo
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">" + (!string.IsNullOrEmpty(row.DescPianoConservazione) ? row.DescPianoConservazione : ""));
            riga.Append("</Data>");
            riga.Append("</Cell>");

            //Colonna Tempi di conservazione
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">" + (!string.IsNullOrEmpty(row.TempoConservazione) ? row.TempoConservazione : ""));
            riga.Append("</Data>");
            riga.Append("</Cell>");

            //Colonna Utente
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">" + (!string.IsNullOrEmpty(row.UserIdIntegrazione) ? row.UserIdIntegrazione : ""));
            riga.Append("</Data>");
            riga.Append("</Cell>");

            //Colonna Ruolo
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">" + (!string.IsNullOrEmpty(row.CodRuoloIntegrazione) ? row.CodRuoloIntegrazione : ""));
            riga.Append("</Data>");
            riga.Append("</Cell>");

            //Colonna Tipo documento
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">" + (!string.IsNullOrEmpty(row.DescTipoDoc) ? row.DescTipoDoc : ""));
            riga.Append("</Data>");
            riga.Append("</Cell>");

            //Colonna Campo profilato fascicolo
            riga.Append("<Cell>");
            riga.Append("<Data ss:Type=\"String\">" + (!string.IsNullOrEmpty(row.DescTipoFasc) ? row.DescTipoFasc : ""));
            riga.Append("</Data>");
            riga.Append("</Cell>");

            riga.Append("</Row>");
            return riga.ToString();
        }

    }
}
