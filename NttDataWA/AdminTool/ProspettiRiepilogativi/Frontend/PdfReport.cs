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
using System.Xml;

namespace ProspettiRiepilogativi.Frontend
{
    #region ENUM ReportDisponibili
    public enum ReportDisponibili
    {
        Annuale_By_Amministrazione,
        Annuale_By_Registro,
        Documenti_Classificati,
        Documenti_Trasmessi_Altre_AOO,
        Annuale_By_Fascicolo,
        Fascicoli_Per_VT,
        TempiMediLavFascicoli,
        ReportDocXSede,
        ReportDocXUo,
        ReportLogMassiveImport,
        ReportContatoriDocumento,
        ReportContatoriFascicolo,
        ReportProtocolloArma,
        ReportDettaglioPratica,
        ReportGiornaleRiscontri,
        ReportDocSpeditiInterop,
        CDC_reportControlloPreventivo,
        CDC_reportControlloPreventivoSRC,
        CDC_reportPensioniCivili,
        CDC_reportPensioniMilitari,
        //Giordano Iacozzilli:
        //Modifica CDC 15/6/2012
        //Aggiungo una nuova topologia:
        CDC_reportSuccessivoSCCLA
    }
    #endregion

    #region ENUM RegioniDocumento
    public enum RegioniDocumento
    {
        Generale,
        Classifica,
        Mensile
    }
    #endregion

    /// <summary>
    /// Summary description for PdfReport.
    /// </summary>
    public class PdfReport
    {
        public static SAAdminTool.DocsPaWR.FileDocumento do_MakePdfReport(ReportDisponibili tipoReport, string templateFilePath, DataSet ds, ArrayList parametriPDF)
        {
            return do_MakePdfReport(tipoReport, templateFilePath, null, ds, parametriPDF);
        }

        public static SAAdminTool.DocsPaWR.FileDocumento do_MakePdfReport(ReportDisponibili tipoReport, string templateFilePath, string titolo, DataSet ds, ArrayList parametriPDF)
        {
            //inizializzazione dei parametri di input
            //templateFilePath = templateFilePath + "XMLProspettoRiepilogativo.xml";
            StampaPDF.Report stampaReport;
            string RootPathTemplate = AppDomain.CurrentDomain.BaseDirectory + "ProspettiRiepilogativi/Frontend/TemplateXML/";
            string schemaFilePath = RootPathTemplate + "XMLReport.xsd";
            string outputFilename = DateTime.Now.ToLongDateString() + ".pdf";


            DataTable dt = new DataTable();
            Byte[] dati;

            //variabili ausiliarie
            FileStream fs = null;
            MemoryStream memStream = null;

            //inizializzazione parametro di output (caso integrazione Docspa)
            SAAdminTool.DocsPaWR.FileDocumento doc = new SAAdminTool.DocsPaWR.FileDocumento();
            doc.name = outputFilename;
            doc.path = "";
            doc.fullName = "StampaReport_" + DateTime.Now.ToString();
            doc.contentType = "application/pdf";
            try
            {
                try
                {
                    //lettura template XML
                    fs = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                try
                {
                    //inizializzazione oggetto StampaPDF
                    stampaReport = new StampaPDF.Report(fs, schemaFilePath, titolo);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                try
                {
                    //Agginta dei Paragrafi e della tabella
                    //DataTable da inserire nel Report
                    dt = ds.Tables[0];
                    //Non funziona se i template sono contenuti in IIS
                    //DO_AppendTextInHeader(templateFilePath,"aaa","PIPPO");
                    DO_AppendObject(stampaReport, tipoReport, parametriPDF, dt);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                try
                {
                    //Chiusura template xml
                    fs.Close();

                    //recupero dati per memory stream
                    memStream = stampaReport.getStream();

                    //chiusura stream stampa
                    stampaReport.close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                try
                {
                    //vettore di byte
                    dati = memStream.GetBuffer();

                    #region Salviamo su FileSystem
                    //Salviamo su FileSystem
                    //I file si trovano al percorso: RootPathTemplate + nome file
                    #region Determinazione del FileName
                    //string fileName = "";
                    //					switch(tipoReport)
                    //					{
                    //						case ReportDisponibili.Annuale_By_Registro:
                    //							fileName = RootPathTemplate +"\\"+"PDF\\annualeByReg.pdf";
                    //							break;
                    //						case ReportDisponibili.Annuale_By_Fascicolo:
                    //							fileName = RootPathTemplate +"\\"+"PDF\\annualeByFasc.pdf";
                    //							break;
                    //						case ReportDisponibili.Documenti_Classificati:
                    //							fileName = RootPathTemplate +"\\"+"PDF\\docClass.pdf";
                    //							break;
                    //						case ReportDisponibili.Documenti_Trasmessi_Altre_AOO:
                    //							fileName = RootPathTemplate +"\\"+"PDF\\docTrasmToAOO.pdf";
                    //							break;
                    //						case ReportDisponibili.Fascicoli_Per_VT:
                    //							fileName = RootPathTemplate +"\\"+"PDF\\fascPerVT.pdf";
                    //							break;
                    //						case ReportDisponibili.TempiMediLavFascicoli:
                    //							fileName = RootPathTemplate +"\\"+"PDF\\TempiMediLavFasc.pdf";
                    //							break;
                    //					}
                    //					#endregion
                    //
                    //					if(fileName != "")
                    //					{
                    //						Stream outputStream = new FileStream(fileName,FileMode.Create,FileAccess.Write);
                    //						outputStream.Write(dati,0,dati.Length);
                    //					
                    //						outputStream.Flush();
                    //						outputStream.Close();
                    //						outputStream = null;
                    //					}
                    #endregion
                    #endregion

                    //chiudo mem stream
                    memStream.Close();

                    //aggiorno informazioni del doc : utilizzo un tipo infodocumento come output
                    doc.length = (int)dati.Length;
                    doc.content = dati;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //rilascio oggetti utilizzati
                if (fs != null)
                {
                    fs.Close();
                }
                if (memStream != null)
                {
                    memStream.Close();
                }
                //ritorno oggetto doc (caso integrazione Docspa)
            }
            return doc;
        }

        #region METODI PRIVATI

        #region DO_AddParagraphToReport
        /// <summary>
        /// DO_AddParagraphToReport: appende a ciascun report
        /// i paragrafi di interesse
        /// </summary>
        /// <param name="objReport">oggetto Report</param>
        /// <param name="tipoReport">tipo Report</param>
        /// <param name="parametri">parametri da stampare nei paragrafi</param>
        /// <param name="region">regione di interesse del paragrafo</param>
        private static void DO_AddParagraphToReport(StampaPDF.Report objReport, ReportDisponibili tipoReport, ArrayList parametri, RegioniDocumento region)
        {
            #region Recupero dei parametri
            Hashtable parametriPDF = new Hashtable();

            if (parametri != null)
            {
                foreach (Parametro p in parametri)
                {
                    parametriPDF.Add(p.Descrizione, p.Valore);
                }
            }
            #endregion

            switch (tipoReport)
            {
                #region ReportDisponibili.Annuale_By_Amministrazione
                case ReportDisponibili.Annuale_By_Amministrazione:
                    //TODO: Aggiungere Codice di gestione
                    break;
                #endregion

                #region ReportDisponibili.ReportLogMassiveImport
                case ReportDisponibili.ReportLogMassiveImport:
                    //nulla da settare
                    break;
                #endregion

                #region ReportDisponibili.Annuale_By_Registro
                case ReportDisponibili.Annuale_By_Registro:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            //objReport.appendParagraph("PAR_TITOLO",parametriPDF,false);
                            objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                            //objReport.appendParagraph("PAR_ANNO",parametriPDF,false);
                            break;
                        case RegioniDocumento.Classifica:
                            //objReport.appendParagraph("PAR_CLASS","",false);
                            break;
                        case RegioniDocumento.Mensile:
                            //objReport.appendParagraph("PAR_MESI","",false);
                            break;
                    }
                    break;
                #endregion

                #region ReportDisponibili.Documenti_Classificati
                case ReportDisponibili.Documenti_Classificati:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            objReport.appendParagraph("PAR_TITOLO", parametriPDF, false);
                            objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                            objReport.appendParagraph("PAR_TOT_DOC", parametriPDF, false);
                            if (parametriPDF.Count > 2)
                            {
                                objReport.appendParagraph("SEDE", parametriPDF, false);
                            }

                            break;
                    }
                    break;
                #endregion

                #region ReportDisponibili.Documenti_Trasmessi_Altre_AOO
                case ReportDisponibili.Documenti_Trasmessi_Altre_AOO:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            objReport.appendParagraph("PAR_TITOLO", "", false);
                            objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                            objReport.appendParagraph("PAR_ANNO", parametriPDF, false);
                            objReport.appendParagraph("TOT_SPED", parametriPDF, false);
                            break;
                    }
                    break;
                #endregion

                #region ReportDisponibili.Fascicoli_Per_VT
                case ReportDisponibili.Fascicoli_Per_VT:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            objReport.appendParagraph("PAR_TITOLO", "", false);
                            objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                            objReport.appendParagraph("PAR_ANNO", parametriPDF, false);
                            objReport.appendParagraph("TOT_FASC_CREATI", parametriPDF, false);
                            objReport.appendParagraph("TOT_FASC_CHIUSI", parametriPDF, false);
                            break;
                    }
                    break;
                #endregion

                #region ReportDisponibili.Annuale_By_Fascicolo
                case ReportDisponibili.Annuale_By_Fascicolo:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            objReport.appendParagraph("PAR_TITOLO", "", false);
                            objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                            objReport.appendParagraph("PAR_ANNO", parametriPDF, false);
                            objReport.appendParagraph("TOT_FASCICOLI", parametriPDF, false);
                            objReport.appendParagraph("TOT_FASCICOLI_CREATI", parametriPDF, false);
                            objReport.appendParagraph("TOT_FASCICOLI_CHIUSI", parametriPDF, false);
                            break;
                    }
                    break;
                #endregion

                #region ReportDisponibili.TempiMediLavFascicoli
                case ReportDisponibili.TempiMediLavFascicoli:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            objReport.appendParagraph("PAR_TITOLO", "", false);
                            objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                            objReport.appendParagraph("PAR_ANNO", parametriPDF, false);
                            break;
                    }
                    break;

                #endregion

                #region ReportDisponibili.ReportDocXSede
                case ReportDisponibili.ReportDocXSede:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            //objReport.appendParagraph("PAR_TITOLO",parametriPDF,false);
                            objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                            objReport.appendParagraph("PAR_ANNO", parametriPDF, false);
                            break;
                    }
                    break;

                #endregion

                #region ReportDisponibili.ReportDocXUo
                case ReportDisponibili.ReportDocXUo:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            //objReport.appendParagraph("PAR_TITOLO",parametriPDF,false);
                            objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                            objReport.appendParagraph("PAR_ANNO", parametriPDF, false);
                            break;
                    }
                    break;


                #endregion

                #region ReportDisponibili.ReportContatoriDocumento
                case ReportDisponibili.ReportContatoriDocumento:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                            objReport.appendParagraph("PAR_ANNO", parametriPDF, false);
                            break;
                    }
                    break;

                #endregion

                #region ReportDisponibili.ReportContatoriFascicolo
                case ReportDisponibili.ReportContatoriFascicolo:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                            objReport.appendParagraph("PAR_ANNO", parametriPDF, false);
                            break;
                    }
                    break;

                #endregion

                #region ReportDisponibili.ReportProtocolloArma
                case ReportDisponibili.ReportProtocolloArma:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                            objReport.appendParagraph("PAR_ANNO", parametriPDF, false);
                            break;
                    }
                    break;
                #endregion

                #region ReportDisponibili.ReportDettaglioPratica
                case ReportDisponibili.ReportDettaglioPratica:
                    objReport.appendParagraph("PAR_ANNO", parametriPDF, false);
                    objReport.appendParagraph("PAR_CLASSIFICA", parametriPDF, false);
                    objReport.appendParagraph("PAR_PRATICA", parametriPDF, false);
                    objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                    break;
                #endregion

                #region ReportDisponibili.ReportGiornaleRiscontri
                case ReportDisponibili.ReportGiornaleRiscontri:
                    objReport.appendParagraph("PAR_REGISTRO", parametriPDF, false);
                    break;
                #endregion

                #region ReportDisponibili.ReportDocSpeditiInterop
                case ReportDisponibili.ReportDocSpeditiInterop:
                    objReport.appendParagraph("PAR_DOC_SPEDITI", parametriPDF, false);
                    objReport.appendParagraph("PAR_RICEVUTE", parametriPDF, false);
                    objReport.appendParagraph("PAR_MANCANTI", parametriPDF, false);
                    break;
                #endregion

                #region ReportDisponibili.CDC_reportControlloPreventivo
                case ReportDisponibili.CDC_reportControlloPreventivo:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            objReport.appendParagraph("SOTTOTITOLO", parametriPDF, false);
                            objReport.appendParagraph("UFFICIO", parametriPDF, false);
                            objReport.appendParagraph("MAGISTRATO", parametriPDF, false);
                            objReport.appendParagraph("REVISORE", parametriPDF, false);
                            objReport.appendParagraph("INTERVALLO_DI_STAMPA", parametriPDF, false);
                            break;
                    }
                    break;
                #endregion

                #region ReportDisponibili.CDC_reportSuccessivoSCCLA
                //***************************************************************
                //
                //Modifica CDC GIORDANO IACOZZILLI 18/06/2012
                //
                //Aggiungo il report nuovo CDC_reportControlloSuccessivoSCCLA
                case ReportDisponibili.CDC_reportSuccessivoSCCLA:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            objReport.appendParagraph("SOTTOTITOLO", parametriPDF, false);
                            objReport.appendParagraph("UFFICIO", parametriPDF, false);
                            objReport.appendParagraph("MAGISTRATO", parametriPDF, false);
                            objReport.appendParagraph("REVISORE", parametriPDF, false);
                            objReport.appendParagraph("INTERVALLO_DI_STAMPA", parametriPDF, false);
                            break;
                    }
                    break;
                //***************************************************************
                //FINE
                //***************************************************************
                #endregion

                #region ReportDisponibili.CDC_reportPensioniCivili
                case ReportDisponibili.CDC_reportPensioniCivili:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            objReport.appendParagraph("SOTTOTITOLO_UNO", parametriPDF, false);
                            objReport.appendParagraph("SOTTOTITOLO_DUE", parametriPDF, false);
                            objReport.appendParagraph("UFFICIO", parametriPDF, false);
                            objReport.appendParagraph("MAGISTRATO", parametriPDF, false);
                            objReport.appendParagraph("REVISORE", parametriPDF, false);
                            objReport.appendParagraph("INTERVALLO_DI_STAMPA", parametriPDF, false);
                            break;
                    }
                    break;
                #endregion

                #region ReportDisponibili.CDC_reportPensioniMilitari
                case ReportDisponibili.CDC_reportPensioniMilitari:
                    switch (region)
                    {
                        case RegioniDocumento.Generale:
                            objReport.appendParagraph("SOTTOTITOLO_UNO", parametriPDF, false);
                            objReport.appendParagraph("SOTTOTITOLO_DUE", parametriPDF, false);
                            objReport.appendParagraph("UFFICIO", parametriPDF, false);
                            objReport.appendParagraph("MAGISTRATO", parametriPDF, false);
                            objReport.appendParagraph("REVISORE", parametriPDF, false);
                            objReport.appendParagraph("INTERVALLO_DI_STAMPA", parametriPDF, false);
                            break;
                    }
                    break;
                #endregion


            }
        }
        #endregion

        #region DO_AddTableToReport
        /// <summary>
        /// DO_AddTableToReport: appende a ciascun report
        /// le tabelle di interesse
        /// </summary>
        /// <param name="objReport">oggetto Report</param>
        /// <param name="tipoReport">tipo Report</param>
        /// <param name="dt">DataTable contenente i dati della tabella</param>
        /// <param name="TableName">Nome della tabella nel template</param>
        private static void DO_AddTableToReport(StampaPDF.Report objReport, ReportDisponibili tipoReport, DataTable dt, string TableName)
        {
            switch (tipoReport)
            {
                #region ReportDisponibili.Annuale_By_Amministrazione
                case ReportDisponibili.Annuale_By_Amministrazione:
                    //TODO: Aggiungere Codice di gestione
                    break;
                #endregion

                #region ReportDisponibili.Annuale_By_Registro
                case ReportDisponibili.Annuale_By_Registro:
                    /* Creiamo 2 diversi DT a partire dall'unico che abbiamo
                     * Ne recuperiamo la struttura, e ripuliamo il contenuto
                     */
                    switch (TableName)
                    {
                        case "TABLE_ANNO":
                            DataTable dtAnni = new DataTable();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                DataColumn _dc = new DataColumn(dc.ColumnName, dc.DataType);
                                DataColumn _dc1 = new DataColumn(dc.ColumnName, dc.DataType);
                                dtAnni.Columns.Add(_dc1);
                            }
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["ANNO"].ToString() == dr["MESE"].ToString())
                                {
                                    dtAnni.ImportRow(dr);
                                }
                            }
                            objReport.appendTable("TABLE_ANNO", dtAnni, false);
                            break;
                        case "TABLE_MESI":
                            DataTable dtMesi = new DataTable();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                DataColumn _dc = new DataColumn(dc.ColumnName, dc.DataType);
                                DataColumn _dc1 = new DataColumn(dc.ColumnName, dc.DataType);
                                dtMesi.Columns.Add(_dc1);
                            }
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["ANNO"].ToString() != dr["MESE"].ToString())
                                {
                                    dtMesi.ImportRow(dr);
                                }
                            }
                            objReport.appendTable("TABLE_MESI", dtMesi, false);
                            break;
                        case "TABLE_CLASS":
                            DataTable dtAnniClass = new DataTable();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                DataColumn _dc = new DataColumn(dc.ColumnName, dc.DataType);
                                DataColumn _dc1 = new DataColumn(dc.ColumnName, dc.DataType);
                                dtAnniClass.Columns.Add(_dc1);
                            }
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["ANNO"].ToString() == dr["MESE"].ToString())
                                {
                                    dtAnniClass.ImportRow(dr);
                                }
                            }
                            objReport.appendTable("TABLE_CLASS", dtAnniClass, false);
                            objReport.appendParagraph("PAR_NOTE", "", false);
                            break;


                    }
                    break;
                #endregion

                #region ReportDisponibili.Documenti_Classificati
                case ReportDisponibili.Documenti_Classificati:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    objReport.appendParagraph("PAR_NOTE", "", false);
                    break;
                #endregion

                #region ReportDisponibili.Documenti_Trasmessi_Altre_AOO
                case ReportDisponibili.Documenti_Trasmessi_Altre_AOO:
                    #region eliminiamo le righe relative all'anno
                    //				for(int i = dt.Rows.Count-1;i>=0;i--)
                    //				{
                    //					DataRow dr = dt.Rows[i];
                    //					if(dr["mese"].ToString().Length > 2)
                    //					{
                    //						dt.Rows.RemoveAt(i);
                    //					}
                    //				}
                    #endregion
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.Fascicoli_Per_VT
                case ReportDisponibili.Fascicoli_Per_VT:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    objReport.appendParagraph("PAR_NOTE", "", false);
                    break;
                #endregion

                #region ReportDisponibili.Annuale_By_Fascicolo
                case ReportDisponibili.Annuale_By_Fascicolo:
                    objReport.appendTable("TABLE_MESE", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.TempiMediLavFascicoli
                case ReportDisponibili.TempiMediLavFascicoli:
                    objReport.appendTable("TABLE_MESE", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.ReportDocXSede
                case ReportDisponibili.ReportDocXSede:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.ReportDocXUo
                case ReportDisponibili.ReportDocXUo:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    objReport.appendParagraph("PAR_NOTE", "", false);
                    break;
                #endregion

                #region ReportDisponibili.ReportLogMassiveImport:
                case ReportDisponibili.ReportLogMassiveImport:
                    objReport.appendTable("TABLE_DATA", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.ReportContatoriDocumento
                case ReportDisponibili.ReportContatoriDocumento:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.ReportContatoriFascicolo
                case ReportDisponibili.ReportContatoriFascicolo:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.ReportProtocolloArma
                case ReportDisponibili.ReportProtocolloArma:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.ReportDettaglioPratica
                case ReportDisponibili.ReportDettaglioPratica:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.ReportGiornaleRiscontri
                case ReportDisponibili.ReportGiornaleRiscontri:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.ReportDocSpeditiInterop
                case ReportDisponibili.ReportDocSpeditiInterop:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.CDC_reportControlloPreventivo
                case ReportDisponibili.CDC_reportControlloPreventivo:
                    objReport.appendTable("TABLE_DOCUMENTI", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.CDC_reportControlloSuccessivoSCCLA
                //***************************************************************
                //
                //Modifica CDC GIORDANO IACOZZILLI 18/06/2012
                //
                //Aggiungo il report nuovo CDC_reportControlloSuccessivoSCCLA
                case ReportDisponibili.CDC_reportSuccessivoSCCLA:
                    objReport.appendTable("TABLE_DOCUMENTI", dt, false);
                    break;
                //***************************************************************
                //FINE
                //***************************************************************
                #endregion

                #region ReportDisponibili.CDC_reportPensioniCivili
                case ReportDisponibili.CDC_reportPensioniCivili:
                    objReport.appendTable("TABLE_DOCUMENTI", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.CDC_reportPensioniMilitari
                case ReportDisponibili.CDC_reportPensioniMilitari:
                    objReport.appendTable("TABLE_DOCUMENTI", dt, false);
                    break;
                #endregion
            }
        }

        private static void DO_AddTableToReport(StampaPDF.Report objReport, ReportDisponibili tipoReport, DataTable dt, string TableName, ArrayList parametriPDF)
        {
            switch (tipoReport)
            {
                #region ReportDisponibili.Annuale_By_Amministrazione
                case ReportDisponibili.Annuale_By_Amministrazione:
                    //TODO: Aggiungere Codice di gestione
                    break;
                #endregion

                #region ReportDisponibili.Annuale_By_Registro
                case ReportDisponibili.Annuale_By_Registro:
                    /* Creiamo 2 diversi DT a partire dall'unico che abbiamo
                     * Ne recuperiamo la struttura, e ripuliamo il contenuto
                     */
                    int anno = 0;
                    foreach (Parametro p in parametriPDF)
                    {
                        if (p.Descrizione == "@param2")
                        {
                            anno = Convert.ToInt32(p.Valore);
                        }
                    }

                    switch (TableName)
                    {
                        case "TABLE_ANNO":
                            DataTable dtAnni = new DataTable();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                DataColumn _dc = new DataColumn(dc.ColumnName, dc.DataType);
                                DataColumn _dc1 = new DataColumn(dc.ColumnName, dc.DataType);
                                dtAnni.Columns.Add(_dc1);
                            }
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["THING"].ToString() == anno.ToString())
                                {
                                    dtAnni.ImportRow(dr);
                                }
                            }

                            objReport.appendTable("TABLE_ANNO", dtAnni, false);
                            break;
                        case "TABLE_MESI":
                            DataTable dtMesi = new DataTable();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                DataColumn _dc = new DataColumn(dc.ColumnName, dc.DataType);
                                DataColumn _dc1 = new DataColumn(dc.ColumnName, dc.DataType);
                                dtMesi.Columns.Add(_dc1);
                            }
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["THING"].ToString() != anno.ToString() && dr["THING"].ToString() != "Classificati*" && dr["THING"].ToString() != "Senza Img.")
                                {
                                    dtMesi.ImportRow(dr);
                                }
                            }
                            objReport.appendTable("TABLE_MESI", dtMesi, false);

                            break;
                        case "TABLE_CLASS":
                            DataTable dtAnniClass = new DataTable();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                DataColumn _dc = new DataColumn(dc.ColumnName, dc.DataType);
                                DataColumn _dc1 = new DataColumn(dc.ColumnName, dc.DataType);
                                dtAnniClass.Columns.Add(_dc1);
                            }
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["THING"].ToString() == "Classificati*" || dr["THING"].ToString() == "Senza Img.")
                                {
                                    dtAnniClass.ImportRow(dr);
                                }
                            }
                            objReport.appendTable("TABLE_CLASS", dtAnniClass, false);
                            objReport.appendParagraph("PAR_NOTE", "", false);
                            break;
                    }
                    break;
                #endregion

                #region ReportDisponibili.Documenti_Classificati
                case ReportDisponibili.Documenti_Classificati:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.Documenti_Trasmessi_Altre_AOO
                case ReportDisponibili.Documenti_Trasmessi_Altre_AOO:
                    #region eliminiamo le righe relative all'anno
                    //				for(int i = dt.Rows.Count-1;i>=0;i--)
                    //				{
                    //					DataRow dr = dt.Rows[i];
                    //					if(dr["mese"].ToString().Length > 2)
                    //					{
                    //						dt.Rows.RemoveAt(i);
                    //					}
                    //				}
                    #endregion
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.Fascicoli_Per_VT
                case ReportDisponibili.Fascicoli_Per_VT:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.Annuale_By_Fascicolo
                case ReportDisponibili.Annuale_By_Fascicolo:
                    objReport.appendTable("TABLE_MESE", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.TempiMediLavFascicoli
                case ReportDisponibili.TempiMediLavFascicoli:
                    objReport.appendTable("TABLE_MESE", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.ReportDocXSede
                case ReportDisponibili.ReportDocXSede:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    break;
                #endregion

                #region ReportDisponibili.ReportDocXUo
                case ReportDisponibili.ReportDocXUo:
                    objReport.appendTable("TABLE_ANNO", dt, false);
                    objReport.appendParagraph("PAR_NOTE", "", false);
                    break;
                #endregion
            }
        }
        #endregion

        #region DO_AppendObject
        /// <summary>
        /// DO_AppendObject: aggiunge gli oggetti opportuni
        /// a ciascu template
        /// </summary>
        /// <param name="objReport">oggetto Report</param>
        /// <param name="tipoReport">tipo di Report</param>
        /// <param name="parametri">valori da stampare</param>
        private static void DO_AppendObject(StampaPDF.Report objReport, ReportDisponibili tipoReport, ArrayList parametriPDF, DataTable dt)
        {
            switch (tipoReport)
            {
                #region ReportDisponibili.Annuale_By_Registro
                case ReportDisponibili.Annuale_By_Registro:

                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    bool isMensile = false;
                    foreach (Parametro p in parametriPDF)
                    {
                        if (p.Descrizione == "mese" && p.Valore != "")
                        {
                            isMensile = true;
                        }
                    }
                    if (!isMensile)
                    {
                        DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_ANNO", parametriPDF);
                    }
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Mensile);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_MESI", parametriPDF);
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Classifica);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_CLASS", parametriPDF);
                    break;
                #endregion

                #region ReportDisponibili.Documenti_Classificati
                case ReportDisponibili.Documenti_Classificati:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_ANNO");
                    break;
                #endregion

                #region ReportDisponibili.Documenti_Trasmessi_Altre_AOO
                case ReportDisponibili.Documenti_Trasmessi_Altre_AOO:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_ANNO");
                    break;
                #endregion

                #region ReportDisponibili.Fascicoli_Per_VT
                case ReportDisponibili.Fascicoli_Per_VT:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_ANNO");
                    break;
                #endregion

                #region ReportDisponibili.Annuale_By_Fascicolo
                case ReportDisponibili.Annuale_By_Fascicolo:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_MESE");
                    break;
                #endregion

                #region ReportiDisponibili.TempiMediLavFascicoli
                case ReportDisponibili.TempiMediLavFascicoli:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_MESE");
                    break;

                #endregion

                #region ReportDisponibili.ReportDocXSede
                case ReportDisponibili.ReportDocXSede:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_ANNO");
                    break;

                #endregion

                #region ReportDisponibili.ReportDocXUo
                case ReportDisponibili.ReportDocXUo:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_ANNO");
                    break;

                #region ReportDisponibili.ReportLogMassiveImport
                case ReportDisponibili.ReportLogMassiveImport:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_DATA");
                    break;

                #endregion


                #endregion

                #region ReportDisponibili.ReportContatoriDocumento
                case ReportDisponibili.ReportContatoriDocumento:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_ANNO");
                    break;
                #endregion

                #region ReportDisponibili.ReportContatoriFascicolo
                case ReportDisponibili.ReportContatoriFascicolo:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_ANNO");
                    break;
                #endregion

                #region ReportDisponibili.ReportProtocolloArma
                case ReportDisponibili.ReportProtocolloArma:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_ANNO");
                    break;
                #endregion

                #region ReportDisponibili.ReportDettaglioPratica
                case ReportDisponibili.ReportDettaglioPratica:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_ANNO");
                    break;
                #endregion

                #region ReportDisponibili.ReportGiornaleRiscontri
                case ReportDisponibili.ReportGiornaleRiscontri:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_ANNO");
                    break;
                #endregion

                #region ReportDisponibili.ReportDocSpeditiInterop
                case ReportDisponibili.ReportDocSpeditiInterop:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_ANNO");
                    break;
                #endregion

                #region ReportDisponibili.CDC_reportControlloPreventivo
                case ReportDisponibili.CDC_reportControlloPreventivo:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_DOCUMENTI");
                    break;
                #endregion

                #region ReportDisponibili.CDC_reportControlloSuccessivoSCCLA
                //***************************************************************
                //
                //Modifica CDC GIORDANO IACOZZILLI 18/06/2012
                //
                //Aggiungo il report nuovo CDC_reportControlloSuccessivoSCCLA
                case ReportDisponibili.CDC_reportSuccessivoSCCLA:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_DOCUMENTI");
                    break;
                //***************************************************************
                //Modifica FINE CDC GIORDANO IACOZZILLI 18/06/2012
                //***************************************************************
                #endregion

                #region ReportDisponibili.CDC_reportPensioniCivili
                case ReportDisponibili.CDC_reportPensioniCivili:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_DOCUMENTI");
                    break;
                #endregion

                #region ReportDisponibili.CDC_reportPensioniMilitari
                case ReportDisponibili.CDC_reportPensioniMilitari:
                    DO_AddParagraphToReport(objReport, tipoReport, parametriPDF, RegioniDocumento.Generale);
                    DO_AddTableToReport(objReport, tipoReport, dt, "TABLE_DOCUMENTI");
                    break;
                #endregion
            }
        }
        #endregion

        #region DO_AppendTextInHeader
        /// <summary>
        /// DO_AppendTextInHeader
        /// </summary>
        /// <param name="path">percorso del file da modificare</param>
        /// <param name="param">parametro da sostituire</param>
        /// <param name="text">testo da inserire</param>
        private static void DO_AppendTextInHeader(string path, string param, string text)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                XmlElement xn = (XmlElement)doc.SelectSingleNode("report/header");
                string titolo = xn.InnerText;
                titolo = titolo.Replace(param, text);
                xn.InnerText = titolo;

                XmlTextWriter xtw = new XmlTextWriter(path, System.Text.Encoding.ASCII);
                doc.WriteContentTo(xtw);
                xtw.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #endregion

    }
}