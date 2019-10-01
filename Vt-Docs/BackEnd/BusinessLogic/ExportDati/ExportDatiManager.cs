using System;
using System.Web;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Reflection;
using System.Diagnostics;
using System.Data;
using System.Text;
using DocsPaVO.fascicolazione;
using System.Linq;
using DocsPaVO.documento;
using DocsPaVO.Grid;
using DocsPaVO.ExportData;
using DocsPaVO.filtri;
using DocsPaVO.utente;
using DocsPaVO.ProfilazioneDinamica;
using BusinessLogic.ProfilazioneDinamica;
using log4net;
using System.Collections.Generic;
using DocsPaVO.ricerche;
using DocsPaVO.Grids;
using AODL;
using DocsPaDB.Query_DocsPAWS;

namespace BusinessLogic.ExportDati
{
    public class ExportDatiManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ExportDatiManager));
        #region Variabili
        private DocsPaVO.documento.FileDocumento _file = new DocsPaVO.documento.FileDocumento();
        private ArrayList _objList = new ArrayList();
        private DocsPaVO.filtri.FiltroRicerca[][] _filtri = null;
        private DocsPaVO.filtri.FiltroRicerca[] _filtriTrasm = null;
        private DocsPaVO.fascicolazione.Folder _folder = null;
        private string _codFasc = string.Empty;
        private string _exportType = string.Empty;
        private string _title = string.Empty;
        private string _docXSLUrl = string.Empty;
        private string _fascXSLUrl = string.Empty;
        private string _docCestXSLUrl = string.Empty;
        private string _trasmXSLUrl = string.Empty;
        private string _todolistXSLUrl = string.Empty;
        private string _conservazioneXSLUrl = string.Empty;
        private string _conservazionePolicyXSLUrl = string.Empty;
        private string _conservazioneRigeneraIstanzaXSLUrl = string.Empty;
        private string _scartoXSLUrl = string.Empty;
        private XmlDocument _xmlDoc = new XmlDocument();
        private string _temporaryXSLFilePath = string.Empty;
        private string _descAmm = string.Empty;
        private string _rowsList = string.Empty;
        private bool _protoIntEnabled = false;
        private string _idIstanza = string.Empty;
        private string _pathModelloExc = string.Empty;
        private string _notificationCenterXSLUrl = string.Empty;
        protected DocsPaVO.documento.EtichettaInfo[] etichette;
        DocsPaVO.utente.InfoUtente infoUser = new DocsPaVO.utente.InfoUtente();

        private DocsPaVO.filtri.FiltroRicerca[][] _filtriOrder = null;

        #endregion Variabili

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetXmlTemplatePath(string relativeFilePath)
        {
            string retValue = string.Empty;

            if (HttpContext.Current != null)
            {
                retValue = HttpContext.Current.Server.MapPath(relativeFilePath);
            }
            else
            {
                string appPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

                retValue = Path.Combine(appPath, relativeFilePath);
            }

            return retValue;
        }


        public ExportDatiManager()
        {
            this._docXSLUrl = this.GetXmlTemplatePath(@"xml/xslfo_export_doc.xsl");
            _fascXSLUrl = this.GetXmlTemplatePath(@"xml/xslfo_export_fasc.xsl");
            _docCestXSLUrl = this.GetXmlTemplatePath(@"xml/xslfo_export_docInCest.xsl");
            _trasmXSLUrl = this.GetXmlTemplatePath(@"xml/xslfo_export_trasm.xsl");
            _todolistXSLUrl = this.GetXmlTemplatePath(@"xml/xslfo_export_todolist.xsl");
            _conservazioneXSLUrl = this.GetXmlTemplatePath(@"xml/xslfo_export_conservazione.xsl");
            _conservazionePolicyXSLUrl = this.GetXmlTemplatePath(@"xml/xslfo_export_conservazione_policy.xsl");
            _conservazioneRigeneraIstanzaXSLUrl = this.GetXmlTemplatePath(@"xml/xslfo_export_conservazione_rigIst.xsl");
            _scartoXSLUrl = this.GetXmlTemplatePath(@"xml/xslfo_export_fasc_scarto.xsl");
            _notificationCenterXSLUrl = this.GetXmlTemplatePath(@"xml/xslfo_export_notification_center.xsl");
        }

        #region Chiamate per l'esportazione
        //OK
        public DocsPaVO.documento.FileDocumento ExportDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] filtri, string exportType, string title, DocsPaVO.ricerche.FullTextSearchContext context, ArrayList campiSelezionati, bool mittDest_indirizzo, String[] documentsSystemId, Grid grid, bool useTraditionaExport)
        {

            this._filtri = filtri;
            this._exportType = exportType;
            this._title = title;
            string idGruppo = infoUtente.idGruppo;
            string idPeople = infoUtente.idPeople;
            this._descAmm = this.getNomeAmministrazione(idPeople);
            this.infoUser = infoUtente;
            //VERONICA: gestione ricerca veloce full text
            if ((context != null) && (context.TextToSearch != ""))
            {
                ArrayList documenti = null;
                try
                {
                    documenti = Documenti.InfoDocManager.FullTextSearch(infoUtente, ref context);

                    // Se ci sono system id di documenti selezionati, viene effettuato un filtraggio 
                    // dei risultati
                    if (documentsSystemId != null &&
                        documentsSystemId.Length > 0)
                    {
                        ArrayList tmp = new ArrayList();

                        foreach (InfoDocumento doc in documenti)
                            if (documentsSystemId.Where(e => e.Equals(doc.idProfile)).Count() > 0)
                                tmp.Add(doc);

                        documenti = tmp;
                    }


                    switch (exportType)
                    {
                        case "PDF":
                            this.exportDocRicFullTextPDF(documenti);
                            break;
                        case "XLS":
                            //this.exportDocRicFullTextXLS(documenti, campiSelezionati);
                            this.exportDocRicFullTextXLS(documenti, campiSelezionati);
                            break;
                        //case "Model":
                        //    this.exportDocRicFromModel(documenti, campiSelezionati);
                        //    break;
                    }
                }
                catch (Exception ex)
                {
                    this._file = null;
                    logger.Debug(ex);
                }
            }
            else //ricerca normale
            {
                switch (exportType)
                {
                    case "PDF":
                        //                        this.exportDocPDF(idGruppo, idPeople, mittDest_indirizzo);
                        this.exportDocPDF(infoUtente, mittDest_indirizzo, documentsSystemId);
                        break;
                    case "XLS":
                        //this.exportDocXLS(idGruppo, idPeople, campiSelezionati);
                        //                        this.exportDocXLS(idGruppo, idPeople, campiSelezionati, mittDest_indirizzo);
                        this.exportDocXLS(infoUtente, campiSelezionati, mittDest_indirizzo, documentsSystemId, grid, useTraditionaExport);
                        break;
                    case "Model":
                        this.exportDocRicFromModel(infoUtente, campiSelezionati, mittDest_indirizzo, documentsSystemId, grid);
                        break;
                }
            }
            return this._file;
        }

        //OK
        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="folder"></param>
        /// <param name="codFascicolo"></param>
        /// <param name="exportType"></param>
        /// <param name="title"></param>
        /// <param name="filtriRicerca"></param>
        /// <param name="campiSelezionati"></param>
        /// <param name="mitt_dest_Indirizzo"></param>
        /// <param name="grid">Griglia da utilizzare per l'export</param>
        /// <param name="selectedDocumentsId"></param>
        /// <returns></returns>
        public DocsPaVO.documento.FileDocumento ExportDocInFasc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Folder folder, string codFascicolo, string exportType, string title, DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca, ArrayList campiSelezionati, bool mitt_dest_Indirizzo, Grid grid, String[] selectedDocumentsId, bool useTraditionaExport)
        {
            this._folder = folder;
            this._codFasc = codFascicolo;
            //this._exportType = exportType;
            this._title = title;
            this._descAmm = this.getNomeAmministrazione(infoUtente.idPeople);
            this.infoUser = infoUtente;
            switch (exportType)
            {
                case "PDF":
                    this.exportDocInFascPDF(infoUtente.idGruppo, infoUtente.idPeople, filtriRicerca, mitt_dest_Indirizzo, selectedDocumentsId);
                    break;
                case "XLS":
                    //this.exportDocInFascXLS(idGruppo, idPeople, filtriRicerca, campiSelezionati);
                    this.exportDocInFascXLS(infoUtente, filtriRicerca, campiSelezionati, mitt_dest_Indirizzo, grid, selectedDocumentsId, useTraditionaExport);
                    break;
            }

            return this._file;
        }

        //OK
        public DocsPaVO.documento.FileDocumento ExportDocInCest(DocsPaVO.utente.InfoUtente infoUtente, string exportType, string title, DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca, ArrayList campiSelezionati)
        {

            this.infoUser = infoUtente;
            //this._exportType = exportType;
            this._title = title;
            this._descAmm = this.getNomeAmministrazione(infoUtente.idPeople);

            switch (exportType)
            {
                case "PDF":
                    this.exportDocInCestPDF(infoUtente, filtriRicerca);
                    break;
                case "XLS":
                    //this.exportDocInCestXLS(infoUtente, filtriRicerca, campiSelezionati);
                    this.exportDocInCestXLS(infoUtente, filtriRicerca, campiSelezionati);
                    break;
            }

            return this._file;
        }

        //OK
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="registro"></param>
        /// <param name="enableUfficioRef"></param>
        /// <param name="enableProfilazione"></param>
        /// <param name="enableChilds"></param>
        /// <param name="classificazione"></param>
        /// <param name="filtri"></param>
        /// <param name="exportType"></param>
        /// <param name="title"></param>
        /// <param name="campiSelezionati"></param>
        /// <param name="idProjectsList"></param>
        /// <param name="grid">Griglia da cui prelevare le informazioni sui campi da esportare</param>
        /// <returns></returns>
        public DocsPaVO.documento.FileDocumento ExportFasc(DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, bool enableChilds, DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.filtri.FiltroRicerca[][] filtri, string exportType, string title, ArrayList campiSelezionati, String[] idProjectsList, Grid grid, bool useTraditionaExport)
        {
            this._filtri = filtri;
            this._exportType = exportType;
            this._title = title;
            this._descAmm = this.getNomeAmministrazione(userInfo.idPeople);

            switch (exportType)
            {
                case "PDF":
                    this.exportFascPDF(userInfo, registro, enableUfficioRef, enableProfilazione, enableChilds, classificazione, idProjectsList);
                    break;
                case "XLS":
                    //this.exportFascXLS(userInfo, registro, enableUfficioRef, enableProfilazione, enableChilds, classificazione, campiSelezionati);
                    this.exportFascXLS(userInfo, registro, enableUfficioRef, enableProfilazione, enableChilds, classificazione, campiSelezionati, idProjectsList, grid, useTraditionaExport);
                    break;
            }

            return this._file;
        }

        //OK
        public DocsPaVO.documento.FileDocumento ExportTrasm(DocsPaVO.trasmissione.OggettoTrasm oggettoTrasmesso, string tipoRicerca, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.filtri.FiltroRicerca[] filtri, string exportType, string title, ArrayList campiSelezionati)
        {
            this._filtriTrasm = filtri;
            this._exportType = exportType;
            this._title = title;
            this._descAmm = this.getNomeAmministrazione(utente.idPeople);

            switch (exportType)
            {
                case "PDF":
                    this.exportTrasmPDF(oggettoTrasmesso, tipoRicerca, utente, ruolo);
                    break;
                case "XLS":
                    this.exportTrasmXLS(oggettoTrasmesso, tipoRicerca, utente, ruolo, campiSelezionati);
                    break;
                case "ODS":
                    this.exportTrasmODS(oggettoTrasmesso, tipoRicerca, utente, ruolo, campiSelezionati);
                    break;
            }

            return _file;
        }

        //OK
        public DocsPaVO.documento.FileDocumento ExportToDoList(DocsPaVO.utente.InfoUtente infoUtente, string docOrFasc, DocsPaVO.filtri.FiltroRicerca[] listaFiltri, string registri, string exportType, string title, ArrayList campiSelezionati, String[] objectId)
        {
            this._filtriTrasm = listaFiltri;
            this._exportType = exportType;
            this._title = title;
            this._descAmm = this.getNomeAmministrazione(infoUtente.idPeople);

            switch (exportType)
            {
                case "PDF":
                    this.exportToDoListPDF(infoUtente, docOrFasc, registri, objectId);
                    break;
                case "XLS":
                    this.exportToDoListXLS(infoUtente, docOrFasc, registri, campiSelezionati, objectId);
                    break;
            }

            return _file;
        }


        public DocsPaVO.documento.FileDocumento ExportScarto(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.AreaScarto.InfoScarto infoScarto, string exportType, string title, DocsPaVO.ricerche.FullTextSearchContext context, ArrayList campiSelezionati)
        {
            this._exportType = exportType;
            this._title = title;
            string idGruppo = infoUtente.idGruppo;
            string idPeople = infoUtente.idPeople;
            this._descAmm = this.getNomeAmministrazione(idPeople);

            switch (exportType)
            {
                case "PDF":
                    this.exportScartoPDF(infoUtente, infoScarto);
                    break;
                case "XLS":
                    this.exportScartoXLS(infoUtente, campiSelezionati, infoScarto);
                    break;

            }

            return this._file;
        }



        #endregion Chiamate per l'esportazione


        //Export in excel tramite HTML
        #region Export dati Archivio cartaceo

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="filtri"></param>
        /// <param name="exportTitle"></param>
        /// <returns></returns>
        public DocsPaVO.documento.FileDocumento ExportArchivioCartaceo(DocsPaVO.ExportData.ExportDataFormatEnum format, DocsPaVO.FascicolazioneCartacea.DocumentoFascicolazione[] documenti, string exportTitle)
        {
            if (format.Equals(DocsPaVO.ExportData.ExportDataFormatEnum.Excel))
            {
                this.exportArchivioCartaceoXLS(documenti, exportTitle);
            }
            else
            {

            }

            return this._file;
        }

        /// <summary>
        ///  creazione del file XLS in formato HTM per l'archivio cartaceo
        /// </summary>
        /// <returns></returns>
        private string createXLS_HTM_ARCHIVIO_CARTACEO()
        {
            string theHTM = string.Empty;

            theHTM = this.commonTopXLS();

            theHTM += ".xl24";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;}";
            theHTM += ".xl25";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "vertical-align:middle;}";
            theHTM += ".xl26";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl27";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:.5pt solid windowtext;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl28";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:.5pt solid windowtext;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl29";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:.5pt solid windowtext;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl30";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl31";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl32";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "font-weight:700;";
            theHTM += "font-family:Arial, sans-serif;";
            theHTM += "mso-font-charset:0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "background:silver;";
            theHTM += "mso-pattern:auto none;}";
            theHTM += ".xl33";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:.5pt solid windowtext;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl34";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-weight:700;";
            theHTM += "font-family:Arial, sans-serif;";
            theHTM += "mso-font-charset:0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "background:silver;";
            theHTM += "mso-pattern:auto none;}";
            theHTM += "-->";
            theHTM += "</style>";
            theHTM += "<!--[if gte mso 9]><xml>";
            theHTM += "<x:ExcelWorkbook>";
            theHTM += "<x:ExcelWorksheets>";
            theHTM += "<x:ExcelWorksheet>";
            theHTM += "<x:Name>Sheet1</x:Name>";
            theHTM += "<x:WorksheetOptions>";
            theHTM += "<x:Print>";
            theHTM += "<x:ValidPrinterInfo/>";
            theHTM += "<x:HorizontalResolution>600</x:HorizontalResolution>";
            theHTM += "<x:VerticalResolution>600</x:VerticalResolution>";
            theHTM += "</x:Print>";
            theHTM += "<x:Selected/>";
            theHTM += "<x:Panes/>";
            theHTM += "<x:ProtectContents>False</x:ProtectContents>";
            theHTM += "<x:ProtectObjects>False</x:ProtectObjects>";
            theHTM += "<x:ProtectScenarios>False</x:ProtectScenarios>";
            theHTM += "</x:WorksheetOptions>";
            theHTM += "</x:ExcelWorksheet>";
            theHTM += "</x:ExcelWorksheets>";
            theHTM += "<x:WindowHeight>10230</x:WindowHeight>";
            theHTM += "<x:WindowWidth>15195</x:WindowWidth>";
            theHTM += "<x:WindowTopX>0</x:WindowTopX>";
            theHTM += "<x:WindowTopY>30</x:WindowTopY>";
            theHTM += "<x:ProtectStructure>False</x:ProtectStructure>";
            theHTM += "<x:ProtectWindows>False</x:ProtectWindows>";
            theHTM += "</x:ExcelWorkbook>";
            theHTM += "<x:ExcelName>";
            theHTM += "<x:Name>Print_Titles</x:Name>";
            theHTM += "<x:SheetIndex>1</x:SheetIndex>";
            theHTM += "<x:Formula>=Sheet1!$1:$1</x:Formula>";
            theHTM += "</x:ExcelName>";
            theHTM += "</xml><![endif]-->";
            theHTM += "</head>";
            theHTM += "<body link=blue vlink=purple class=xl30>";
            theHTM += "<table x:str border=0 cellpadding=0 cellspacing=0 width=972 style='border-collapse:collapse;table-layout:fixed;width:910pt'>";
            theHTM += "<col class=xl24 width=50 style='mso-width-source:userset;mso-width-alt:5000;width:50pt'>";
            theHTM += "<col class=xl24 width=60 style='mso-width-source:userset;mso-width-alt:1500;width:60pt'>";
            theHTM += "<col class=xl24 width=100 style='mso-width-source:userset;mso-width-alt:2000;width:100pt'>";
            theHTM += "<col class=xl24 width=100 style='mso-width-source:userset;mso-width-alt:6000;width:100pt'>";
            theHTM += "<col class=xl24 width=400 style='mso-width-source:userset;mso-width-alt:20000;width:400pt'>";
            theHTM += "<col class=xl24 width=200 style='mso-width-source:userset;mso-width-alt:2000;width:200pt'>";
            theHTM += "<tr class=xl28 height=17 style='height:12.75pt'>";
            theHTM += "<td height=17 class=xl27 width=50 style='height:12.75pt;width:50pt'>Documento</td>";
            theHTM += "<td class=xl27 width=60 style='border-left:none;width:60pt'>Tipo</td>";
            theHTM += "<td class=xl27 width=100 style='border-left:none;width:100pt'>Versione</td>";
            theHTM += "<td class=xl27 width=100 style='border-left:none;width:100pt'>Registro</td>";
            theHTM += "<td class=xl27 width=400 style='border-left:none;width:400pt'>Fascicolo</td>";
            theHTM += "<td class=xl27 width=200 style='border-left:none;width:200pt'>Inserisci in cartaceo</td>";
            theHTM += "</tr>";
            theHTM += "@RIGHE@";
            theHTM += "<![if supportMisalignedColumns]>";
            theHTM += "<tr height=32 style='display:none'>";
            theHTM += "<td width=62 style='width:62pt'></td>";
            theHTM += "<td width=60 style='width:60pt'></td>";
            theHTM += "<td width=100 style='width:100pt'></td>";
            theHTM += "<td width=200 style='width:200pt'></td>";
            theHTM += "<td width=400 style='width:400pt'></td>";
            theHTM += "<td width=200 style='width:200pt'></td>";
            theHTM += "</tr>";
            theHTM += "<![endif]>";
            theHTM += "</table>";
            theHTM += "</body>";
            theHTM += "</html>";

            return theHTM;
        }

        private void exportArchivioCartaceoXLS(DocsPaVO.FascicolazioneCartacea.DocumentoFascicolazione[] documenti, string exportTitle)
        {
            try
            {
                string theHTM = string.Empty;
                string titolo = string.Empty;
                string righe = string.Empty;
                StringBuilder sb = new StringBuilder();

                if (documenti.Length > 0)
                {
                    theHTM = this.createXLS_HTM_ARCHIVIO_CARTACEO();
                    titolo = this._descAmm + " - Stampa del " + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + " - Righe stampate: " + Convert.ToString(this._objList.Count) + "\\000A" + this._title;

                    foreach (DocsPaVO.FascicolazioneCartacea.DocumentoFascicolazione documento in documenti)
                    {
                        righe += "<tr height=32 style='height:24.0pt'>";

                        string cellDocumento = string.Empty;
                        if (documento.NumeroProtocollo > 0)
                            cellDocumento = string.Format("{0} {1}", documento.NumeroProtocollo.ToString(), documento.DataProtocollo.ToString("dd/MM/yyyy"));
                        else
                            cellDocumento = string.Format("{0} {1}", documento.DocNumber.ToString(), documento.DataCreazione.ToString("dd/MM/yyyy"));

                        righe += " <td height=32 class=xl29 width=82 style='height:24.0pt;width:62pt'>" + cellDocumento + "</td>";
                        righe += " <td class=xl29  style='width:60pt'>" + documento.TipoDocumento + "</td>";
                        righe += " <td class=xl29 style='width:100pt'>" + documento.VersionLabel + "</td>";
                        righe += "  <td class=xl29 style='width:200pt'>" + documento.CodiceRegistro + "</td>";
                        righe += "  <td class=xl29 style='width:400pt'>" + documento.Fascicolo.CodiceFascicolo + @"\" + documento.Fascicolo.DescrizioneFascicolo + "</td>";
                        righe += "  <td class=xl29 style='width:200pt'> Si / No </td>";
                        righe += "</tr>";
                    }

                    theHTM = theHTM.Replace("@TITOLO@", exportTitle);

                    theHTM = theHTM.Replace("@RIGHE@", righe);

                    sb.Append(theHTM.ToString());
                    this.saveAndClose("archivioCartaceo", sb);

                    this.createExportFile();
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        #endregion


        //Export in excel tramite XML    
        #region Esportazioni in Excel tramite XML

        #region Export Titolario
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
                    stream.Read(contentExcel, 0, contentExcel.Length);
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

        #endregion Export Titolario


        #region Export Rubrica

        /// <summary>
        /// Export rubrica con motore ReportGeneratorCommand
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="store"></param>
        /// <param name="registri"></param>
        /// <param name="title"></param>
        /// <param name="tipologia"></param>
        /// <returns></returns>
        public DocsPaVO.documento.FileDocumento ExportRubricaNew(DocsPaVO.utente.InfoUtente infoUtente, bool store, string registri, string title, string tipologia)
        {
            try
            {
                this._title = title;

                // Registri
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

                ArrayList regs = utenti.GetListaRegistriRfRuolo(infoUtente.idCorrGlobali, "", "");
                for (int i = 0; regs != null && i < regs.Count; i++)
                {
                    DocsPaVO.utente.Registro reg = (DocsPaVO.utente.Registro)regs[i];
                    if (i == 0)
                    {
                        registri = reg.systemId;
                    }
                    else
                        registri += "," + reg.systemId;
                }

                // Creazione filtri
                List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "registri", valore = registri });
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "store", valore = store ? "1" : "0" });

                // Creazione e personalizzazione request
                DocsPaVO.Report.PrintReportRequest request = new DocsPaVO.Report.PrintReportRequest();
                request.Title = title;
                request.SubTitle = string.Empty;
                request.AdditionalInformation = string.Empty;
                request.ContextName = "ExportRubrica";
                request.ReportKey = "ExportRubrica";
                request.SearchFilters = filters;
                request.UserInfo = infoUtente;
                switch (tipologia)
                {
                    case "XLS":
                        request.ReportType = DocsPaVO.Report.ReportTypeEnum.Excel;
                        break;

                    case "ODS":
                        request.ReportType = DocsPaVO.Report.ReportTypeEnum.ODS;
                        break;
                }

                // Chiamata metodo generazione report
                this._file = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request).Document;


            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione rubrica : " + ex.Message);
            }

            return this._file;
        }

        public DocsPaVO.documento.FileDocumento ExportRubrica(DocsPaVO.utente.InfoUtente infoUtente, bool store, string registri, string title)
        {
            try
            {
                this._title = title;

                //Recupero i dati
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

                ArrayList regs = utenti.GetListaRegistriRfRuolo(infoUtente.idCorrGlobali, "", "");

                for (int i = 0; regs != null && i < regs.Count; i++)
                {
                    DocsPaVO.utente.Registro reg = (DocsPaVO.utente.Registro)regs[i];
                    if (i == 0)
                    {
                        registri = reg.systemId;
                    }
                    else
                        registri += "," + reg.systemId;

                }


                ArrayList corr = Rubrica.DPA3_RubricaSearchAgent.GetListaCorrispondenti(infoUtente, registri);

                if (corr == null || corr.Count == 0)
                {
                    this._file = null;
                    return this._file;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;

                //Creazione stringa XML
                //StringBuilder sb = new StringBuilder();
                //sb = creaXMLRubrica(corr, store);
                string sb = creaXMLStringRubrica(corr, store);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportCorrispondenti.xls");
                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportCorrispondenti.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportRubrica";
                    this._file.contentType = "application/vnd.ms-excel";
                    //this._file.contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione rubrica : " + ex.Message);
            }
            return this._file;
        }

        private StringBuilder creaXMLRubrica(ArrayList corr, bool store)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += sheetRubrica(corr, store);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());
            return sb;
        }

        private string creaXMLStringRubrica(ArrayList corr, bool store)
        {
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += sheetRubrica(corr, store);

            strXML += "</Workbook>";

            return strXML;
        }

        private string sheetRubrica(ArrayList corr, bool store)
        {
            string sb = string.Empty;

            sb += "<Worksheet ss:Name=\"RUBRICA\">";
            sb += "<Table>";
            sb += creaTabellaRubrica(store);
            sb += datiRubricaXML(corr, store);
            sb += "</Table>";
            sb += workSheetOptionsXML();
            sb += "</Worksheet>";

            return sb;
        }

        private string creaTabellaRubrica(bool store)
        {
            string strXML = string.Empty;
            if (store)
                strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Storicizza
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Codice registro
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Codice rubrica
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Codice amministrazione
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Codice AOO
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"25\"/>"; //Tipo
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Descrizione
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Cognome
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Nome
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Indirizzo
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"30\"/>"; //Cap
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Città
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Provincia
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Nazione
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Codice fiscale            
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Partita Iva   
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"50\"/>"; //Telefono 1            
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"50\"/>"; //Telefono 2   
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"50\"/>"; //Fax
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Email
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Localita
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Note
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"60\"/>"; //Nuovo registro
            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"70\"/>"; //Canale Preferenziale
            return strXML;
        }

        private string datiRubricaXML(ArrayList corr, bool store)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append(creaColonneRubrica(store));
            //sb.Append(inserisciDatiRubrica(corr, store));
            string sb = string.Empty;
            sb = creaColonneRubrica(store);
            sb += inserisciDatiRubrica(corr, store);
            return sb;
        }

        private string creaColonneRubrica(bool store)
        {
            string strXML = string.Empty;
            // Inserimento del sottotitolo se specificato
            if (!String.IsNullOrEmpty(_title))
                strXML += "<Row ss:Height=\"23.25\"><Cell ss:StyleID=\"s27\"><Data ss:Type=\"String\">" + _title + "</Data></Cell></Row>";
            strXML += "<Row>";
            if (store)
            {
                //Colonna Storicizza
                strXML += "<Cell ss:StyleID=\"s70\">";
                strXML += "<Data ss:Type=\"String\">Storicizza";
                strXML += "</Data>";
                strXML += "</Cell>";
            }
            //Colonna Codice rubrica
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Cod. registro";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Codice rubrica
            strXML += "<Cell ss:StyleID=\"s66\">";
            strXML += "<Data ss:Type=\"String\">Cod. rubrica";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Codice amministrazione
            strXML += "<Cell ss:StyleID=\"s66\">";
            strXML += "<Data ss:Type=\"String\">Cod. amm.";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Codice AOO
            strXML += "<Cell ss:StyleID=\"s66\">";
            strXML += "<Data ss:Type=\"String\">Cod. AOO";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Tipo
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Tipo";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Descrizione
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Descrizione";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Colonna Cognome
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Cognome";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Colonna Nome
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Nome";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Indirizzo
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Indirizzo";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Cap
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Cap";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Città
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Città";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Provincia
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Provincia";
            strXML += "</Data>";
            strXML += "</Cell>";

            //Colonna Nazione
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Nazione";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Codice Fiscale
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Cod. Fiscale";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Partita Iva
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">P.Iva";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Telefono 1
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Tel1";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Telefono 2
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Tel2";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Fax
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Fax";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna email
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Email";
            strXML += "</Data>";
            strXML += "</Cell>";
            //Colonna Localita
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Località";
            strXML += "</Data>";
            strXML += "</Cell>";

            //Colonna Note
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Note";
            strXML += "</Data>";
            strXML += "</Cell>";

            //Colonna Nuovo registro
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Nuovo registro";
            strXML += "</Data>";
            strXML += "</Cell>";

            //Colonna Canale preferenziale
            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Canale preferenziale";
            strXML += "</Data>";
            strXML += "</Cell>";
            strXML += "</Row>";
            return strXML;

        }

        private string inserisciDatiRubrica(ArrayList corr, bool store)
        {
            string sb = string.Empty;

            foreach (DocsPaVO.utente.DatiModificaCorr corrispondente in corr)
                sb += inserisciRigaRubrica(corrispondente, store);

            return sb.ToString();
        }

        private string inserisciRigaRubrica(DocsPaVO.utente.DatiModificaCorr corr, bool store)
        {
            string sb = string.Empty;

            sb += "<Row>";

            if (store)
            {
                //Colonna Storicizza
                sb += "<Cell>";
                sb += "<Data ss:Type=\"String\">";
                sb += "</Data>";
                sb += "</Cell>";
            }
            //Colonna Codice Registro
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\">" + corr.codice;
            sb += "</Data>";
            sb += "</Cell>";

            //Colonna Codice Rubrica
            sb += "<Cell ss:StyleID=\"s71\">";
            sb += "<Data ss:Type=\"String\">" + corr.codRubrica;
            sb += "</Data>";
            sb += "</Cell>";

            //Colonna Codice Amministrazione
            sb += "<Cell ss:StyleID=\"s71\">";
            sb += "<Data ss:Type=\"String\">" + corr.codiceAmm;
            sb += "</Data>";
            sb += "</Cell>";

            //Colonna Codice AOO
            sb += "<Cell ss:StyleID=\"s71\">";
            sb += "<Data ss:Type=\"String\">" + corr.codiceAoo;
            sb += "</Data>";
            sb += "</Cell>";

            //Colonna Tipo
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\">" + corr.tipoCorrispondente;
            sb += "</Data>";
            sb += "</Cell>";

            //Colonna Descrizione
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\"><![CDATA[" + corr.descCorr + "]]>";
            sb += "</Data>";
            sb += "</Cell>";

            //Colonna Cognome
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\"><![CDATA[" + corr.cognome + "]]>";
            sb += "</Data>";
            sb += "</Cell>";

            //Colonna Nome
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\"><![CDATA[" + corr.nome + "]]>";
            sb += "</Data>";
            sb += "</Cell>";

            //Colonna Indirizzo
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\"><![CDATA[" + corr.indirizzo + "]]>";
            sb += "</Data>";
            sb += "</Cell>";
            //Colonna CAP
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\">" + corr.cap;
            sb += "</Data>";
            sb += "</Cell>";
            //Colonna Citta
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\"><![CDATA[" + corr.citta + "]]>";
            sb += "</Data>";
            sb += "</Cell>";
            //Colonna Provincia
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\"><![CDATA[" + corr.provincia + "]]>";
            sb += "</Data>";
            sb += "</Cell>";

            //Colonna Nazione
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\"><![CDATA[" + corr.nazione + "]]>";
            sb += "</Data>";
            sb += "</Cell>";
            //Colonna Codice fiscale
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\">" + corr.codFiscale;
            sb += "</Data>";
            sb += "</Cell>";
            //Partita Iva
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\">" + corr.partitaIva;
            sb += "</Data>";
            sb += "</Cell>";
            //Colonna Telefono1
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\">" + corr.telefono;
            sb += "</Data>";
            sb += "</Cell>";
            //Colonna Telefono2
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\">" + corr.telefono2;
            sb += "</Data>";
            sb += "</Cell>";
            //Colonna Fax
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\">" + corr.fax;
            sb += "</Data>";
            sb += "</Cell>";
            //Colonna Email
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\"><![CDATA[" + corr.email + "]]>";
            sb += "</Data>";
            sb += "</Cell>";
            //Colonna Localita
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\"><![CDATA[" + corr.localita + "]]>";
            sb += "</Data>";
            sb += "</Cell>";

            //Colonna Note
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\"><![CDATA[" + corr.note + "]]>";
            sb += "</Data>";
            sb += "</Cell>";

            //Colonna Nuovo registro
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\"><![CDATA[]]>";
            sb += "</Data>";
            sb += "</Cell>";

            //Colonna Canale preferenziale
            sb += "<Cell>";
            sb += "<Data ss:Type=\"String\"><![CDATA[" + corr.descrizioneCanalePreferenziale + "]]>";
            sb += "</Data>";
            sb += "</Cell>";

            sb += "</Row>";

            return sb;
        }

        #endregion

        #region Export Search Address Book

        public DocsPaVO.documento.FileDocumento ExportSearchAddressBook(DocsPaVO.utente.InfoUtente infoUtente, bool store, DocsPaVO.rubrica.ParametriRicercaRubrica qr, string title)
        {
            try
            {
                this._title = title;

                //Recupero i dati
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

                ArrayList corr = Rubrica.DPA3_RubricaSearchAgent.GetCorrespondentsByFilter(qr, infoUtente);

                if (corr == null || corr.Count == 0)
                {
                    this._file = null;
                    return this._file;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;

                string sb = creaXMLStringRubrica(corr, store);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportCorrispondenti.xls");
                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportCorrispondenti.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportRubrica";
                    this._file.contentType = "application/vnd.ms-excel";
                    //this._file.contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione rubrica : " + ex.Message);
            }
            return this._file;
        }

        /// <summary>
        /// Export ricerche in rubrica con nuovo motore
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="store"></param>
        /// <param name="qr"></param>
        /// <param name="title"></param>
        /// <param name="tipologia"></param>
        /// <returns></returns>
        public DocsPaVO.documento.FileDocumento ExportSearchAddressBookNew(DocsPaVO.utente.InfoUtente infoUtente, bool store, DocsPaVO.rubrica.ParametriRicercaRubrica qr, string title, string tipologia)
        {
            try
            {
                this._title = title;

                //Recupero i dati
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

                ArrayList corr = Rubrica.DPA3_RubricaSearchAgent.GetCorrespondentsByFilter(qr, infoUtente);

                if (corr == null || corr.Count == 0)
                {
                    this._file = null;
                    return this._file;
                }

                // Costruisco il DataSet da inviare nella request
                DataSet dataSet = new DataSet();
                dataSet = creaDataSetRubrica(corr, store);

                // Creazione request
                DocsPaVO.Report.PrintReportRequestDataset request = new DocsPaVO.Report.PrintReportRequestDataset();
                request.Title = title;
                request.SubTitle = string.Empty;
                request.AdditionalInformation = string.Empty;
                request.ContextName = "ExportRubricaSearch";
                request.ReportKey = "ExportRubricaSearch";
                request.InputDataset = dataSet;
                request.UserInfo = infoUtente;
                switch (tipologia)
                {
                    case "XLS":
                        request.ReportType = DocsPaVO.Report.ReportTypeEnum.Excel;
                        break;

                    case "ODS":
                        request.ReportType = DocsPaVO.Report.ReportTypeEnum.ODS;
                        break;
                }

                this._file = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request).Document;

            }

            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione ricerca rubrica: " + ex.Message);
            }

            return this._file;
        }

        private DataSet creaDataSetRubrica(ArrayList corr, bool store)
        {
            DataSet dataSet = new DataSet();
            DataTable table = new DataTable();

            // Creo le colonne
            if (store)
                table.Columns.Add(new DataColumn("Storicizza", typeof(string)));
            table.Columns.Add(new DataColumn("Cod. Registro", typeof(string)));
            table.Columns.Add(new DataColumn("Cod. Rubrica", typeof(string)));
            table.Columns.Add(new DataColumn("Cod. Amm.", typeof(string)));
            table.Columns.Add(new DataColumn("Cod. AOO", typeof(string)));
            table.Columns.Add(new DataColumn("Tipo", typeof(string)));
            table.Columns.Add(new DataColumn("Descrizione", typeof(string)));
            table.Columns.Add(new DataColumn("Cognome", typeof(string)));
            table.Columns.Add(new DataColumn("Nome", typeof(string)));
            table.Columns.Add(new DataColumn("Indirizzo", typeof(string)));
            table.Columns.Add(new DataColumn("CAP", typeof(string)));
            table.Columns.Add(new DataColumn("Città", typeof(string)));
            table.Columns.Add(new DataColumn("Provincia", typeof(string)));
            table.Columns.Add(new DataColumn("Nazione", typeof(string)));
            table.Columns.Add(new DataColumn("Cod. Fiscale", typeof(string)));
            table.Columns.Add(new DataColumn("P. IVA", typeof(string)));
            table.Columns.Add(new DataColumn("Tel 1", typeof(string)));
            table.Columns.Add(new DataColumn("Tel 2", typeof(string)));
            table.Columns.Add(new DataColumn("Fax", typeof(string)));
            table.Columns.Add(new DataColumn("Email", typeof(string)));
            table.Columns.Add(new DataColumn("Località", typeof(string)));
            table.Columns.Add(new DataColumn("Note", typeof(string)));
            table.Columns.Add(new DataColumn("Nuovo registro", typeof(string)));
            table.Columns.Add(new DataColumn("Canale preferenziale", typeof(string)));

            // inserisco le righe
            foreach (DocsPaVO.utente.DatiModificaCorr item in corr)
            {
                table.Rows.Add(createDataRowRubrica(item, table, store));
            }

            dataSet.Tables.Add(table);
            return dataSet;
        }

        private DataRow createDataRowRubrica(DocsPaVO.utente.DatiModificaCorr corr, DataTable table, bool store)
        {
            DataRow row = table.NewRow();

            if (store)
                row["Storicizza"] = string.Empty;

            row["Cod. Registro"] = corr.codice;
            row["Cod. Rubrica"] = corr.codRubrica;
            row["Cod. Amm."] = corr.codiceAmm;
            row["Cod. AOO"] = corr.codiceAoo;
            row["Tipo"] = corr.tipoCorrispondente;
            row["Descrizione"] = corr.descCorr;
            row["Cognome"] = corr.cognome;
            row["Nome"] = corr.nome;
            row["Indirizzo"] = corr.indirizzo;
            row["CAP"] = corr.cap;
            row["Città"] = corr.citta;
            row["Provincia"] = corr.provincia;
            row["Nazione"] = corr.nazione;
            row["Cod. Fiscale"] = corr.codFiscale;
            row["P. IVA"] = corr.partitaIva;
            row["Tel 1"] = corr.telefono;
            row["Tel 2"] = corr.telefono2;
            row["Fax"] = corr.fax;
            row["Email"] = corr.email;
            row["Località"] = corr.localita;
            row["Note"] = corr.note;
            row["Nuovo Registro"] = string.Empty;
            row["Canale preferenziale"] = corr.descrizioneCanalePreferenziale;

            return row;
        }

        #endregion

        #region Export Documenti Custom

        private void exportDocXLS(DocsPaVO.utente.InfoUtente infoUtente, ArrayList campiSelezionati, bool mittDest_indirizzo, String[] documentsSystemId, Grid grid, bool useTraditionaExport)
        {
            try
            {
                //Recupero i dati
                //ArrayList documenti = Documenti.InfoDocManager.getQueryExport(idGruppo, idPeople, this._filtri, mittDest_indirizzo);
                ArrayList documenti = Documenti.InfoDocManager.getQueryExport(infoUtente, this._filtri, mittDest_indirizzo, documentsSystemId);

                if (documenti == null || documenti.Count == 0)
                {
                    this._file = null;
                    return;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLDocGrids(documenti, campiSelezionati, grid, infoUtente, useTraditionaExport);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportDocumenti.xls");
                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportDocumenti.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportDocumenti";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione documenti : " + ex.Message);
            }
        }

        private void exportDocInCestXLS(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca, ArrayList campiSelezionati)
        {
            this.infoUser = infoUtente;

            try
            {
                //Recupero i dati
                //In questo caso mi devo ricostruire un ArrayList di InfoDocumentoExport
                //perchè mi viene restituito un ArrayLIst di InfoDocumento
                ArrayList documentiAppoggio = Documenti.InfoDocManager.getQueryExportDocInCest(infoUtente, filtriRicerca);
                ArrayList documenti = new ArrayList();
                foreach (DocsPaVO.documento.InfoDocumento docAppoggio in documentiAppoggio)
                {
                    DocsPaVO.documento.InfoDocumentoExport docExport = new DocsPaVO.documento.InfoDocumentoExport();
                    docExport.codiceRegistro = docAppoggio.codRegistro;
                    docExport.tipologiaDocumento = docAppoggio.tipoProto;
                    if (docAppoggio.numProt != null && docAppoggio.numProt != "")
                        docExport.idOrNumProt = docAppoggio.numProt;
                    else
                        docExport.idOrNumProt = docAppoggio.docNumber;
                    docExport.data = docAppoggio.dataApertura;
                    docExport.oggetto = docAppoggio.oggetto;
                    string str_mittDest = "";
                    if (docAppoggio.mittDest.Count > 0)
                    {
                        for (int g = 0; g < docAppoggio.mittDest.Count; g++)
                            str_mittDest += docAppoggio.mittDest[g] + " - ";
                        str_mittDest = str_mittDest.Substring(0, str_mittDest.Length - 3);
                    }
                    docExport.mittentiDestinatari = str_mittDest;
                    docExport.noteCestino = docAppoggio.noteCestino;

                    documenti.Add(docExport);
                }


                if (documenti == null || documenti.Count == 0)
                {
                    this._file = null;
                    return;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLDoc(documenti, campiSelezionati);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportDocumenti.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportDocumenti.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportDocumenti";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione documenti : " + ex.Message);
            }
        }

        private void exportDocRicFromModel(DocsPaVO.utente.InfoUtente infoUtente, ArrayList campiSelezionati, bool mittDest_indirizzo, String[] documentsSystemId, Grid grid)
        {
            DocsPaVO.utente.Utente utente = Utenti.UserManager.getUtenteById(infoUtente.idPeople);
            //C:\\Documents and Settings\\ricciutife\\Desktop\\provaExport.xls

            try
            {
                //Recupero i dati
                //ArrayList documenti = Documenti.InfoDocManager.getQueryExport(idGruppo, idPeople, this._filtri, mittDest_indirizzo);
                ArrayList documenti = Documenti.InfoDocManager.getQueryExport(infoUtente, this._filtri, mittDest_indirizzo, documentsSystemId);
                DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateDettagli(((DocsPaVO.documento.InfoDocumentoExport)documenti[0]).docNumber);
                _pathModelloExc = template.PATH_MODELLO_EXCEL;

                if (documenti == null || documenti.Count == 0)
                {
                    this._file = null;
                    return;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLMod(documenti, campiSelezionati, utente, grid, infoUtente);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportDocumenti.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportDocumenti.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportDocumenti";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione documenti : " + ex.Message);
            }

        }

        private StringBuilder creaXMLMod(ArrayList documenti, ArrayList campiSelezionati, DocsPaVO.utente.Utente utente, Grid grid, InfoUtente userInfo)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXMLModel();

            //Tabella campi scelti
            strXML += sheetModello(documenti, campiSelezionati, utente, grid, userInfo);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());

            return sb;
        }

        private string sheetModello(ArrayList documenti, ArrayList campiSelezionati, DocsPaVO.utente.Utente utente, Grid grid, InfoUtente userInfo)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"DOCUMENTI\">";
            strXML += "<Table>";
            //strXML += creaTabellaDocumenti(campiSelezionati);
            //Aggiungo intestazione del modello
            strXML += addHeaderFromModel(documenti, utente);

            strXML += "<Row></Row>";

            // Se sono attive le griglie, i nomi dei campi potrebbero essere
            // differenti rispetto a quelli standard, quindi in questo caso viene
            // richiamata una funzione apposita
            if (Grids.GridManager.ExistGridPersonalizationFunction() && grid != null)
                strXML += PopulateDocumentTable(documenti, campiSelezionati, grid, userInfo);
            else
                strXML += datiDocumentiXML_OLD(documenti, campiSelezionati);

            strXML += "</Table>";
            strXML += workSheetOptionsXMLModel();
            strXML += "</Worksheet>";
            return strXML;
        }

        private string workSheetOptionsXMLModel()
        {
            string strXML = string.Empty;
            strXML = "<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">";
            strXML += "<PageSetup>";
            //strXML += "<Layout x:Orientation=\"Landscape\"/>";
            XmlTextReader textReader = new XmlTextReader(_pathModelloExc);
            if (textReader.ReadToFollowing("Header"))
                strXML += copyNode(textReader);
            if (textReader.ReadToFollowing("Footer"))
                strXML += copyNode(textReader);


            //strXML += "<PageMargins x:Bottom=\"0.984251969\" x:Left=\"0.78740157499999996\" x:Right=\"0.78740157499999996\" x:Top=\"0.984251969\"/>";
            strXML += "</PageSetup>";
            strXML += "<Print>";
            strXML += "<ValidPrinterInfo/>";
            strXML += "<PaperSizeIndex>9</PaperSizeIndex>";
            strXML += "<HorizontalResolution>600</HorizontalResolution>";
            strXML += "<VerticalResolution>600</VerticalResolution>";
            strXML += "</Print>";
            strXML += "<Selected/>";
            strXML += "<ProtectObjects>False</ProtectObjects>";
            strXML += "<ProtectScenarios>False</ProtectScenarios>";
            strXML += "</WorksheetOptions>";
            textReader.Close();
            return strXML;
        }

        private string addHeaderFromModel(ArrayList documenti, DocsPaVO.utente.Utente utente)
        {
            ModelDataProcessor dataProcessor = new ModelDataProcessor();
            string strXML = string.Empty;
            XmlTextReader textReader = new XmlTextReader(_pathModelloExc);
            XmlNodeType nType = textReader.NodeType;
            textReader.ReadToFollowing("Table");
            while (textReader.Read() && !textReader.Name.Equals("Table"))
            {
                if (textReader.Name.Equals("Data"))
                {
                    strXML += "<Data ss:Type=\"String\">";
                    string a = textReader.ReadElementContentAsString();
                    if (a.Contains("#"))
                    {
                        string[] arrayValori = a.Split(new char[] { '#' });
                        for (int y = 0; y < arrayValori.Length; y++)
                        {
                            if (!string.IsNullOrEmpty(arrayValori[y]))
                                strXML += dataProcessor.fillData(arrayValori[y], documenti, utente, infoUser, _filtri);
                        }
                        //strXML += dataProcessor.fillData(a, documenti,utente ,infoUser, _filtri);
                        strXML += "</Data>";
                    }
                    else
                        strXML += a + "</Data>";
                    strXML += copyNode(textReader);
                }
                else
                    strXML += copyNode(textReader);

            }
            textReader.Close();
            return strXML;
        }

        private string copyNode(XmlTextReader textReader)
        {
            string sb = string.Empty;
            switch (textReader.NodeType)
            {
                case XmlNodeType.Element: // The node is an element.
                    //Nei tag dell'header e del Footer ci sono dei caratteri speciali 
                    if (textReader.Name.Equals("Header") || textReader.Name.Equals("Footer"))
                    {
                        string endTag;
                        sb += "<" + textReader.Name;
                        if (textReader.IsEmptyElement)
                            endTag = "/>";
                        else
                            endTag = ">";
                        while (textReader.MoveToNextAttribute())// Read the attributes.
                        {
                            string valore = textReader.Value;
                            //valore = valore.Substring(1, valore.Length - 2);
                            valore = valore.Replace("&", "&amp;");
                            valore = valore.Replace("\"", "&quot;");
                            //valore = "\"" + valore + "\"";
                            //valore.Split(new char[] { '#' });
                            sb += " " + textReader.Name + "=\"" + valore + "\"";
                        }
                        sb += endTag;
                    }
                    else
                    {
                        string endTag;
                        sb += "<" + textReader.Name;
                        if (textReader.IsEmptyElement)
                            endTag = "/>";
                        else
                            endTag = ">";
                        while (textReader.MoveToNextAttribute()) // Read the attributes.
                            sb += " " + textReader.Name + "=\"" + textReader.Value + "\"";
                        sb += endTag;
                    }
                    break;
                case XmlNodeType.Text: //Display the text in each element.
                    sb += textReader.Value;
                    break;
                case XmlNodeType.EndElement: //Display the end of the element.
                    sb += "</" + textReader.Name;
                    sb += ">";
                    break;
            }
            return sb;
        }

        private string copyStyleNode(XmlTextReader textReader)
        {
            string sb = string.Empty;
            switch (textReader.NodeType)
            {
                case XmlNodeType.Element: // The node is an element.
                    sb += "<" + textReader.Name;

                    while (textReader.MoveToNextAttribute()) // Read the attributes.
                        sb += " " + textReader.Name + "=\"" + textReader.Value + "\"";
                    sb += "/>";
                    break;
                case XmlNodeType.Text: //Display the text in each element.
                    sb += textReader.Value;
                    break;
                case XmlNodeType.EndElement: //Display the end of the element.
                    sb += "</" + textReader.Name;
                    sb += ">";
                    break;
            }
            return sb;
        }


        private void exportDocRicFullTextXLS(ArrayList listaDocumenti, ArrayList campiSelezionati)
        {
            try
            {
                //Recupero i dati
                //In questo caso mi devo ricostruire un ArrayList di InfoDocumentoExport
                //perchè mi viene restituito un ArrayLIst di InfoDocumento
                ArrayList documentiAppoggio = listaDocumenti;
                ArrayList documenti = new ArrayList();
                foreach (DocsPaVO.documento.InfoDocumento docAppoggio in documentiAppoggio)
                {
                    DocsPaVO.documento.InfoDocumentoExport docExport = new DocsPaVO.documento.InfoDocumentoExport();
                    docExport.codiceRegistro = docAppoggio.codRegistro;
                    if (docAppoggio.numProt != null && docAppoggio.numProt != "")
                        docExport.idOrNumProt = docAppoggio.numProt;
                    else
                        docExport.idOrNumProt = docAppoggio.docNumber;
                    docExport.data = docAppoggio.dataApertura;
                    docExport.oggetto = docAppoggio.oggetto;
                    docExport.tipologiaDocumento = docAppoggio.tipoProto;
                    string str_mittDest = "";
                    if (docAppoggio.mittDest.Count > 0)
                    {
                        for (int g = 0; g < docAppoggio.mittDest.Count; g++)
                            str_mittDest += docAppoggio.mittDest[g] + " - ";
                        str_mittDest = str_mittDest.Substring(0, str_mittDest.Length - 3);
                    }
                    docExport.mittentiDestinatari = str_mittDest;
                    docExport.dataAnnullamento = docAppoggio.dataAnnullamento;
                    if (Documenti.CacheFileManager.isFileInCache(docAppoggio.docNumber))
                        docExport.acquisitaImmagine = "SI";
                    else
                        docExport.acquisitaImmagine = docAppoggio.acquisitaImmagine;

                    documenti.Add(docExport);
                }

                if (documenti == null || documenti.Count == 0)
                {
                    this._file = null;
                    return;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLDoc(documenti, campiSelezionati);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportDocumenti.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportDocumenti.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportDocumenti";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione documenti : " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtriRicerca"></param>
        /// <param name="campiSelezionati"></param>
        /// <param name="mittDest_indirizzo"></param>
        /// <param name="grid">Griglia da utilizzare per la creazione del file Excel</param>
        /// <param name="selectedDocumentsId">Id dei documenti dei documenti da esportare</param>
        private void exportDocInFascXLS(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca, ArrayList campiSelezionati, bool mittDest_indirizzo, Grid grid, String[] selectedDocumentsId, bool useTraditionaExport)
        {
            try
            {
                //Recupero i dati
                //In questo caso mi devo ricostruire un ArrayList di InfoDocumentoExport
                //perchè mi viene restituito un ArrayLIst di InfoDocumento
                ArrayList documentiAppoggio = Documenti.InfoDocManager.getQueryExportDocInFasc(infoUtente.idGruppo, infoUtente.idPeople, this._folder, filtriRicerca, mittDest_indirizzo, selectedDocumentsId);
                ArrayList documenti = new ArrayList();
                foreach (DocsPaVO.documento.InfoDocumento docAppoggio in documentiAppoggio)
                {
                    DocsPaVO.documento.InfoDocumentoExport docExport = new DocsPaVO.documento.InfoDocumentoExport();
                    docExport.codiceRegistro = docAppoggio.codRegistro;
                    if (docAppoggio.numProt != null && docAppoggio.numProt != "")
                        docExport.idOrNumProt = docAppoggio.numProt;
                    else
                        docExport.idOrNumProt = docAppoggio.docNumber;
                    docExport.data = docAppoggio.dataApertura;
                    docExport.oggetto = docAppoggio.oggetto;
                    docExport.tipologiaDocumento = docAppoggio.tipoProto;
                    if (!mittDest_indirizzo)
                    {
                        string str_mittDest = "";
                        if (docAppoggio.mittDest.Count > 0)
                        {
                            for (int g = 0; g < docAppoggio.mittDest.Count; g++)
                                str_mittDest += docAppoggio.mittDest[g] + " - ";
                            str_mittDest = str_mittDest.Substring(0, str_mittDest.Length - 3);
                        }
                        docExport.mittentiDestinatari = str_mittDest;
                    }
                    else
                        docExport.mittentiDestinatari = docAppoggio.mittDoc;

                    docExport.codiceFascicolo = this._codFasc;
                    docExport.dataAnnullamento = docAppoggio.dataAnnullamento;

                    if (Documenti.CacheFileManager.isFileInCache(docAppoggio.docNumber))
                        docExport.acquisitaImmagine = "SI";
                    else
                        docExport.acquisitaImmagine = docAppoggio.acquisitaImmagine;

                    docExport.idTipoAtto = docAppoggio.idTipoAtto;
                    docExport.docNumber = docAppoggio.docNumber;

                    // Impostazione del system id del documento
                    docExport.systemId = docAppoggio.idProfile;

                    // Reprimento dell'ultima nota inserita
                    DocsPaDB.Query_DocsPAWS.Documenti.GetUltimaNotaDocumento(docExport, infoUtente);

                    documenti.Add(docExport);
                }

                if (documenti == null || documenti.Count == 0)
                {
                    this._file = null;
                    return;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLDocGrids(documenti, campiSelezionati, grid, infoUtente, useTraditionaExport);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportDocumenti.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportDocumenti.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportDocumenti";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione documenti : " + ex.Message);
            }
        }

        /// <summary>
        /// Funzione vecchia lasciata perché le griglie non sono presenti ovunque
        /// </summary>
        private StringBuilder creaXMLDoc(ArrayList documenti, ArrayList campiSelezionati)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += sheetDocumenti(documenti, campiSelezionati);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());

            return sb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documenti"></param>
        /// <param name="campiSelezionati"></param>
        /// <param name="grid">da utilizzare per individuare i campi da esportare</param>
        /// <param name="userInfo">Informazioni sull'utente che ha richiesto l'export</param>
        /// <returns></returns>
        private StringBuilder creaXMLDocGrids(ArrayList documenti, ArrayList campiSelezionati, Grid grid, InfoUtente userInfo, bool useTraditionaExport)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += this.CreateDocumentSheet(documenti, campiSelezionati, grid, userInfo, useTraditionaExport);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());

            return sb;
        }

        private string sheetDocumenti(ArrayList documenti, ArrayList campiSelezionati)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"DOCUMENTI\">";
            strXML += "<Table>";
            strXML += this.creaTabellaDocumenti_old(campiSelezionati);
            strXML += this.datiDocumentiXML_OLD(documenti, campiSelezionati);
            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documenti"></param>
        /// <param name="campiSelezionati"></param>
        /// <param name="grid">Griglia da utilizzare per individuare i campi da esportare</param>
        /// <param name="userInfo">Informazioni sull'utente che ha richiesto l'esportazione</param>
        /// <param name="useTraditionaExport">True se bisogna utilizzare l'export pre griglia</param>
        /// <returns></returns>
        private string CreateDocumentSheet(ArrayList documenti, ArrayList campiSelezionati, Grid grid, InfoUtente userInfo, bool useTraditionaExport)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"DOCUMENTI\">";
            strXML += "<Table>";
            if (Grids.GridManager.ExistGridPersonalizationFunction() && grid != null && !useTraditionaExport)
            {
                strXML += this.CreateExcelTable(campiSelezionati, grid);
                // Inserimento del titolo, se presente, e del sommario di export
                strXML += this.InsertTitleAndSummaryExportDoc(documenti.Count);
                strXML += PopulateDocumentTable(documenti, campiSelezionati, grid, userInfo);
            }
            else
            {
                strXML += creaTabellaDocumenti_old(campiSelezionati);
                strXML += this.InsertTitleAndSummaryExportDoc(documenti.Count);
                strXML += datiDocumentiXML_OLD(documenti, campiSelezionati);
            }

            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }

        /// <summary>
        /// Funzione per l'inserimento della riga di intestazione del file Excel.
        /// La riga sarà costituita da un titolo, un sottotitolo, se specificato, dal numero di fascicolo,
        /// dalla data di creaizone report e dal numero di documenti presenti nel report
        /// </summary>
        /// <param name="documentNumber">Numero di documenti presenti nel report</param>
        /// <returns>Stringa XML con le informazioni da inserire nel file Excel</returns>
        private string InsertTitleAndSummaryExportDoc(int documentNumber)
        {
            // String a da restituire
            StringBuilder toReturn = new StringBuilder();

            // Inserimento del titolo del report
            toReturn.AppendFormat("<Row><Cell ss:StyleID=\"s25\"><Data ss:Type=\"String\">Export dei documenti {0}</Data></Cell></Row>",
                !String.IsNullOrEmpty(this._codFasc) ? "contenuti nel fascicolo " + this._codFasc : String.Empty);

            // Inserimento del sottotitolo se specificato
            if (!String.IsNullOrEmpty(_title))
                toReturn.AppendFormat("<Row ss:Height=\"23.25\"><Cell ss:StyleID=\"s27\"><Data ss:Type=\"String\">{0}</Data></Cell></Row>",
                    _title);

            // Inserimento del summary
            toReturn.AppendFormat("<Row ss:Height=\"23.25\"><Cell ss:StyleID=\"s27\"><Data ss:Type=\"String\">Data report: {0} - Numero documenti esportati: {1}</Data></Cell></Row>",
                DateTime.Now.ToString("dd/MM/yyyy"),
                documentNumber);

            // Restituzione della string aXML
            return toReturn.ToString();

        }

        /// <summary>
        /// Funzione per l'inserimento della riga di intestazione del file Excel di export fascicoli.
        /// La riga sarà costituita da un titolo, un sottotitolo, se specificato,
        /// dalla data di creaizone report e dal numero di fascicoli
        /// </summary>
        /// <param name="folderNumber">Numero di fascicoli presenti nel report</param>
        /// <returns>Stringa XML con le informazioni da inserire nel file Excel</returns>
        private string InsertTitleAndSummaryExportFld(int folderNumber)
        {
            // String a da restituire
            StringBuilder toReturn = new StringBuilder();

            // Inserimento del titolo del report
            toReturn.AppendFormat("<Row><Cell ss:StyleID=\"s25\"><Data ss:Type=\"String\">Export dei fascicoli.</Data></Cell></Row>");

            // Inserimento del sottotitolo se specificato
            if (!String.IsNullOrEmpty(_title))
                toReturn.AppendFormat("<Row ss:Height=\"23.25\"><Cell ss:StyleID=\"s27\"><Data ss:Type=\"String\">{0}</Data></Cell></Row>",
                    _title);

            // Inserimento del summary
            toReturn.AppendFormat("<Row ss:Height=\"23.25\"><Cell ss:StyleID=\"s27\"><Data ss:Type=\"String\">Data report: {0} - Numero fascicoli esportati: {1}</Data></Cell></Row>",
                DateTime.Now.ToString("dd/MM/yyyy"),
                folderNumber);

            // Restituzione della string aXML
            return toReturn.ToString();

        }

        /// <summary>
        /// Funzione per la creazione della tabella Excel con i dati sui campi da mostrare in un export Excel
        /// di documenti o fascicoli
        /// </summary>
        /// <param name="selectedFields">Campi da visualizzare</param>
        /// <param name="grid">Griglia da utilizzare per prelevare le informazioni sui campi</param>
        /// <returns>XML generato</returns>
        private string CreateExcelTable(ArrayList selectedFields, Grid grid)
        {
            string xmlStr = String.Empty;

            // Per ogni campo da visualizzare
            foreach (CampoSelezionato selectedField in selectedFields)
                xmlStr += "<Column ss:StyleID=\"s63\" ss:Width=\"" + grid.Fields.Where(e => e.Label == selectedField.nomeCampo).First().Width + "\"/>";

            // Restituzione dell'XML generato
            return xmlStr;

        }

        /// <summary>
        /// Funzione per la popolazione della tabella dei documenti
        /// </summary>
        /// <param name="documents">Documenti da inserire</param>
        /// <param name="selectedFields">Campi da mostrare</param>
        /// <param name="grid">Griglia da utilizzare per reperire le informazioni sui campi da visualizzare</param>
        /// <returns>Xml con i dati sui documenti</returns>
        private string PopulateDocumentTable(ArrayList documents, ArrayList selectedFields, Grid grid, InfoUtente userInfo)
        {
            string xmlStr = string.Empty;

            // Aggiunta della colonna
            xmlStr = this.creaColonneDocumenti(selectedFields);

            // Inserimento dei dati sui documenti
            xmlStr += this.InsertDocumentsData(documents, selectedFields, grid, userInfo);

            // Restituzione dell'xml generato
            return xmlStr;
        }

        /// <summary>
        /// Funzione per la creazione delle righe con le informazioni sui documenti
        /// </summary>
        /// <param name="documents">Lista dei documenti da esportare</param>
        /// <param name="selectedFields">Lista dei campi da mostrare</param>
        /// <param name="grid">Griglia da utilizzare per reperire informazioni sui campi</param>
        /// <param name="userInfo">Informazioni sull'utente proprietario dell'export</param>
        /// <returns>Xml con le informazioni sui documenti</returns>
        private string InsertDocumentsData(ArrayList documents, ArrayList selectedFields, Grid grid, InfoUtente userInfo)
        {
            // XML da restituire
            string toReturn = string.Empty;

            // Per ogni documento...
            foreach (InfoDocumentoExport document in documents)
                toReturn += this.InsertDocumentData(document, selectedFields, grid, userInfo);

            // Restituzione dell'XML generato
            return toReturn;
        }

        /// <summary>
        /// Funzione per l'export dei dati di un documento
        /// </summary>
        /// <param name="document">Documenti di cui esportare i dati</param>
        /// <param name="selectedFields">Campi da visualizzare</param>
        /// <param name="grid">Griglia da utilizzare per recuperare i dati sui campi da mostrare</param>
        /// <param name="userInfo">Informazioni sull'utente</param>
        /// <returns>XML Excel</returns>
        private string InsertDocumentData(InfoDocumentoExport document, ArrayList selectedFields, Grid grid, InfoUtente userInfo)
        {
            // Riga da restituire
            string toReturn = String.Empty;

            // Proprietà del campo da visualizzare
            Field field;

            // Informazioni sul documento
            InfoDocumento documentInfo = new DocsPaDB.Query_DocsPAWS.Documenti().GetInfoDocumento(userInfo.idGruppo, userInfo.idPeople, document.systemId, true);

            // Valore da scrivere nell'XML
            String value;

            // Apertura riga
            toReturn = "<Row>";

            // Template del documento (caricato solo se c'è almeno un campo profilato)
            Templates template = null;

            string[] visibleFields;
            List<string> idObjectsCustom = new List<string>();
            if (!string.IsNullOrEmpty(documentInfo.tipoAtto))
            {
                foreach (Field fiedlTempl in grid.Fields)
                {
                    if (!string.IsNullOrEmpty(fiedlTempl.AssociatedTemplateName) && fiedlTempl.AssociatedTemplateName.Equals(documentInfo.tipoAtto))
                    {
                        idObjectsCustom.Add((fiedlTempl.CustomObjectId).ToString());
                    }
                }
            }

            // Inserimento dei valori per i campi
            foreach (CampoSelezionato selectedField in selectedFields)
            {

                if (selectedField.SystemId > 0)
                    field = grid.Fields.Where(e => e.CustomObjectId.Equals(selectedField.SystemId)).First();
                else
                    // Recupero delle informazioni sul campo
                    field = grid.Fields.Where(e => e.Label.Equals(selectedField.nomeCampo)).First();

                // Costruzione del nome del campo
                String fieldName = field.CustomObjectId == 0 ? field.OriginalLabel : field.OriginalLabel + field.CustomObjectId;

                StringBuilder temp;

                // Selezione del valore da mostrare
                switch (fieldName.ToUpper())
                {
                    case "SEGNATURA":
                        value = documentInfo.segnatura;
                        break;
                    case "REGISTRO":
                        value = documentInfo.codRegistro;
                        break;
                    case "TIPO":
                        value = documentInfo.tipoProto == "G" ? "NP" : documentInfo.tipoProto;
                        break;
                    case "OGGETTO":
                        value = documentInfo.oggetto;
                        break;
                    case "MITTENTE / DESTINATARIO":
                        temp = new StringBuilder();

                        if (documentInfo.mittDest != null && documentInfo.mittDest.Count > 0)
                        {
                            foreach (string sendRec in documentInfo.mittDest)
                                temp.Append(sendRec + " - ");

                            temp = temp.Remove(temp.Length - 3, 3);
                        }

                        value = temp.ToString();

                        break;
                    case "MITTENTE":
                        temp = new StringBuilder();
                        if (documentInfo.Mittenti != null && documentInfo.Mittenti.Count > 0)
                        {
                            foreach (string sender in documentInfo.Mittenti)
                                temp.Append(sender + " - ");

                            temp = temp.Remove(temp.Length - 3, 3);
                        }

                        value = String.Empty;
                        break;
                    case "DESTINATARI":
                        temp = new StringBuilder();
                        if (documentInfo.Destinatari != null && documentInfo.Destinatari.Count > 0)
                        {
                            foreach (string recivier in documentInfo.Destinatari)
                                temp.Append(recivier + " - ");

                            temp = temp.Remove(temp.Length - 3, 3);
                        }

                        value = String.Empty;
                        break;
                    case "DATA":
                        value = documentInfo.dataApertura;
                        break;
                    case "ESITO PUBBLICAZIONE":
                        value = String.Empty;
                        break;
                    case "DATA ANNULLAMENTO":
                        value = documentInfo.dataAnnullamento;
                        break;
                    case "DOCUMENTO":
                        temp = new StringBuilder();
                        // Creazione dell'informazione sul documento
                        if (!String.IsNullOrEmpty(documentInfo.numProt))
                            temp.Append(documentInfo.numProt);
                        else
                            temp.Append(documentInfo.docNumber);
                        temp.Append(" - ");
                        temp.Append(documentInfo.dataApertura);

                        if (!String.IsNullOrEmpty(documentInfo.protocolloTitolario))
                            temp.Append(" - " + documentInfo.protocolloTitolario);

                        value = temp.ToString();
                        break;

                    case "NUMERO PROTOCOLLO":
                        value = documentInfo.numProt;
                        break;

                    case "AUTORE":
                        value = documentInfo.autore;
                        break;

                    case "DATA ARCHIVIAZIONE":
                        value = documentInfo.dataArchiviazione;
                        break;

                    case "PERSONALE":
                        value = documentInfo.personale;
                        break;

                    case "PRIVATO":
                        value = !String.IsNullOrEmpty(documentInfo.privato) && documentInfo.privato != "0" ? "Si" : "No";
                        break;

                    case "TIPOLOGIA":
                        value = !String.IsNullOrEmpty(documentInfo.idTipoAtto) ? ProfilazioneDocumenti.GetModelNameById(documentInfo.idTipoAtto) : String.Empty;
                        break;

                    case "NOTE":
                        value = !String.IsNullOrEmpty(document.ultimaNota) ? document.ultimaNota : String.Empty;
                        break;

                    case "COD. FASCICOLI":
                        value = document.codiceFascicolo;
                        break;

                    default:



                        if (!string.IsNullOrEmpty(documentInfo.tipoAtto))
                        {
                            visibleFields = idObjectsCustom.ToArray();
                            template = ProfilazioneDocumenti.getTemplateDettagliFilterObjects(documentInfo.docNumber, documentInfo.idTipoAtto, visibleFields);
                            value = this.GetValueForCustomObject(fieldName, template);
                        }
                        else
                        {
                            value = "";
                        }
                        break;
                }

                // Se il campo è standard vengono aggiunti determinati tag altrimenti ne vengono aggiunti altri
                if (field.CustomObjectId == 0)
                {
                    toReturn += "<Cell ss:StyleID=\"s30\">";
                    toReturn += "<Data ss:Type=\"String\">" + value;
                    toReturn += "</Data>";
                    toReturn += "</Cell>";
                }
                else
                {
                    toReturn += "<Cell ss:StyleID=\"s30\">";
                    toReturn += "<Data ss:Type=\"String\"><![CDATA[";
                    toReturn += value;
                    toReturn += "]]></Data>";
                    toReturn += "</Cell>";
                }

            }

            // Chiusura della riga
            toReturn += "</Row>";

            // Restituzione della riga creata
            return toReturn;
        }

        /// <summary>
        /// Funzione per la ricerca del valore da assegnare al campo di profilazione dinamica specificato
        /// </summary>
        /// <param name="fieldName">Il nome del campo custom da valorizzare</param>
        /// <param name="template">Il template associato da cui prelevare il valore</param>
        /// <returns>Il valore da assegnare al campo</returns>
        private String GetValueForCustomObject(string fieldName, Templates template)
        {
            // L'oggetto custom da cui prelevare i risultati
            OggettoCustom customObject;

            // Il testo da restituire
            StringBuilder toReturn;

            // Inizializzazione del valore da restituire
            toReturn = new StringBuilder();

            // Prelevamento dell'oggetto custom con l'etichetta specificata
            // Viene anche effettuata una verifica sul flag "DA_VISUALIZZARE_RICERCA"; non è necessario farlo perchè ora con le nuove griglie tutti i campi sono visualizzabili.
            customObject = ((OggettoCustom[])template.ELENCO_OGGETTI.ToArray(typeof(OggettoCustom))).Where(e => (e.DESCRIZIONE.ToUpper() + e.SYSTEM_ID).Equals(fieldName.ToUpper())).FirstOrDefault();

            // Se l'oggetto custom è una casella di selezione, vengono mergiati i valori
            // selezionati altrimenti il valore da restituire è il valore associato al campo
            if (customObject != null)
            {
                switch (customObject.TIPO.DESCRIZIONE_TIPO.ToUpper())
                {
                    case "CASELLADISELEZIONE":
                        foreach (String value in customObject.VALORI_SELEZIONATI)
                            if (!string.IsNullOrEmpty(value))
                                toReturn.Append(value + "; ");

                        toReturn.Remove(toReturn.Length - 1, 1);
                        break;

                    case "CORRISPONDENTE":
                        Corrispondente corr = (new DocsPaDB.Query_DocsPAWS.Utenti()).GetCorrispondenteBySystemIDDisabled(customObject.VALORE_DATABASE);
                        toReturn.Append(corr.codiceRubrica + " " + corr.descrizione);
                        break;

                    case "LINK":
                        if (customObject.VALORE_DATABASE.Contains("||||"))
                        {
                            int stop = customObject.VALORE_DATABASE.IndexOf("||||");
                            toReturn.Append(customObject.VALORE_DATABASE.Substring(0, stop));
                        }
                        else
                            toReturn.Append(customObject.VALORE_DATABASE);
                        break;
                    case "CONTATORESOTTOCONTATORE":
                        if (customObject.VALORE_DATABASE != null && !customObject.VALORE_DATABASE.Equals(""))
                            toReturn.Append(customObject.VALORE_DATABASE + "-" + customObject.VALORE_SOTTOCONTATORE);
                        break;
                    default:
                        toReturn.Append(customObject.VALORE_DATABASE);
                        break;
                }
            }

            // Restituzione del valore
            return toReturn.ToString();
        }

        private string creaTabellaDocumenti_old(ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                //strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"90\"/>";
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                if (campoSelezionato != null)
                {
                    switch (campoSelezionato.nomeCampo)
                    {
                        case "Registro":
                            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"45\"/>";
                            break;
                        case "Prot. / Id. Doc.":
                            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"70\"/>";
                            break;

                        case "Data":
                            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"45\"/>";
                            break;

                        case "Oggetto":
                            strXML += "<Column ss:StyleID=\"s64\" ss:Width=\"195\"/>";
                            break;

                        case "Tipo":
                            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"25\"/>";
                            break;

                        case "Mitt. / Dest.":
                            strXML += "<Column ss:StyleID=\"s64\" ss:Width=\"120\"/>";
                            break;

                        case "Cod. Fascicoli":
                            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"70\"/>";
                            break;

                        case "Annullato":
                            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"50\"/>";
                            break;

                        case "File":
                            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"20\"/>";
                            break;

                        case "Motivo Rimozione":
                            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"80\"/>";
                            break;

                        default:
                            strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"90\"/>";
                            break;
                    }
                }
            }
            return strXML;
        }


        private string datiDocumentiXML_OLD(ArrayList documenti, ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            strXML = creaColonneDocumenti(campiSelezionati);
            strXML += inserisciDatiDocumenti(documenti, campiSelezionati);
            return strXML;
        }

        private string creaColonneDocumenti(ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            strXML += "<Row>";

            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                strXML += "<Cell ss:StyleID=\"s70\">";
                strXML += "<Data ss:Type=\"String\">" + campoSelezionato.nomeCampo.ToString();
                strXML += "</Data>";
                strXML += "</Cell>";
            }

            strXML += "</Row>";
            return strXML;
        }

        private string inserisciDatiDocumenti(ArrayList documenti, ArrayList campiSelezionati)
        {
            string righe = string.Empty;
            foreach (DocsPaVO.documento.InfoDocumentoExport documento in documenti)
            {
                righe += inserisciRigaDocumenti(documento, campiSelezionati);
            }
            return righe;
        }

        private string inserisciRigaDocumenti(DocsPaVO.documento.InfoDocumentoExport documento, ArrayList campiSelezionati)
        {
            string riga = string.Empty;

            riga = "<Row>";

            //Inserimento Campi Standard
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                if (campoSelezionato.campoStandard == "1")
                {
                    switch (campoSelezionato.nomeCampo)
                    {
                        case "Registro":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + documento.codiceRegistro;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;
                        case "Prot. / Id. Doc.":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + documento.idOrNumProt;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Data":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + documento.data;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Oggetto":
                            riga += "<Cell ss:StyleID=\"s30\">";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + documento.oggetto + "]]>";
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Tipo":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + getLettereProtocolli(documento.tipologiaDocumento);
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Mitt. / Dest.":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + documento.mittentiDestinatari + "]]>";
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Cod. Fascicoli":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + documento.codiceFascicolo;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Annullato":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + documento.dataAnnullamento;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "File":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + documento.acquisitaImmagine;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Motivo Rimozione":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + documento.noteCestino + "]]>";
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Note":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + documento.ultimaNota + "]]>";
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;
                    }
                }
            }

            //Inserimento Campi della profilazione dinamica
            if (documento.idTipoAtto != null && documento.idTipoAtto != "")
            {
                DocsPaVO.ProfilazioneDinamica.Templates templateVuoto = ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(documento.idTipoAtto);
                if (templateVuoto != null && templateVuoto.ID_AMMINISTRAZIONE != null && templateVuoto.ID_AMMINISTRAZIONE != "")
                {
                    DocsPaVO.ProfilazioneDinamica.Templates template = ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateDettagli(documento.docNumber);
                    if (template != null && template.ELENCO_OGGETTI.Count != 0)
                    {
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                        {
                            foreach (DocsPaVO.ExportData.CampoSelezionato campoSelezionato in campiSelezionati)
                            {
                                if (oggettoCustom != null && campoSelezionato.nomeCampo == oggettoCustom.DESCRIZIONE && campoSelezionato.campoStandard != "1")
                                {
                                    riga += "<Cell>";
                                    riga += "<Data ss:Type=\"String\"><![CDATA[";
                                    riga += getValoreOggettoCustom(oggettoCustom);
                                    riga += "]]></Data>";
                                    riga += "</Cell>";
                                }
                            }
                        }
                    }
                }
            }

            riga += "</Row>";
            return riga;
        }

        #endregion export Documenti Custom



        #region Export Fascicoli Custom

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="registro"></param>
        /// <param name="enableUfficioRef"></param>
        /// <param name="enableProfilazione"></param>
        /// <param name="enableChilds"></param>
        /// <param name="classificazione"></param>
        /// <param name="campiSelezionati"></param>
        /// <param name="idProjectsList"></param>
        /// <param name="grid">Griglia da utilizzare per l'esportazione</param>
        private void exportFascXLS(DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, bool enableChilds, DocsPaVO.fascicolazione.Classificazione classificazione, ArrayList campiSelezionati, String[] idProjectsList, Grid grid, bool useTraditionaExport)
        {
            try
            {
                //Recupero i dati
                DocsPaDB.Query_DocsPAWS.Fascicoli objFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                ArrayList fascicoli = objFasc.getQueryExportFasc(userInfo, classificazione, this._filtri[0], enableUfficioRef, enableProfilazione, enableChilds, registro, null, "", idProjectsList);

                //Non serve più perchè il controllo lo fa prima
                /*      if (idProjectsList != null &&
                          idProjectsList.Length > 0)
                      {
                          ArrayList tmp = new ArrayList();
                          foreach (Fascicolo prj in fascicoli)
                              if (idProjectsList.Where(e => e.Equals(prj.systemID)).Count() > 0)
                                  tmp.Add(prj);

                          fascicoli = tmp;

                      }*/


                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLFasc(fascicoli, campiSelezionati, registro, grid, infoUser, useTraditionaExport);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportFascicoli.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportFascicoli.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportFascicoli";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione fascicoli : " + ex.Message);
            }
        }

        private StringBuilder creaXMLFasc(ArrayList fascicoli, ArrayList campiSelezionati, DocsPaVO.utente.Registro registro, Grid grid, InfoUtente userInfo, bool useTraditionaExport)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            if (grid != null && Grids.GridManager.ExistGridPersonalizationFunction() && !useTraditionaExport)
                strXML += this.CreateProjectExcel(fascicoli, campiSelezionati, registro, grid, userInfo);
            else
                strXML += sheetFascicoli(fascicoli, campiSelezionati, registro);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());

            return sb;
        }

        /// <summary>
        /// Funzione per la creazione dell'Excel per l'export dei fascicoli
        /// </summary>
        /// <param name="projects">Lista dei fascicoli da esportare</param>
        /// <param name="selectFields">Campi da mostrare nell'Excel</param>
        /// <param name="registry">Registro</param>
        /// <param name="grid">Griglia da cui ricavare le informazioni sui campi da mostrare</param>
        /// <returns>Excel da restituire all'utente</returns>
        private string CreateProjectExcel(ArrayList projects, ArrayList selectFields, Registro registry, Grid grid, InfoUtente userInfo)
        {
            // Excel da restituire
            string toReturn = String.Empty;

            toReturn = "<Worksheet ss:Name=\"Fascicoli\">";
            toReturn += "<Table>";
            toReturn += this.CreateExcelTable(selectFields, grid);
            toReturn += this.InsertTitleAndSummaryExportFld(projects.Count);
            toReturn += this.creaColonneFascicoli(selectFields);
            toReturn += this.CreateProjectsData(projects, selectFields, registry, grid, userInfo);
            toReturn += "</Table>";
            toReturn += workSheetOptionsXML();
            toReturn += "</Worksheet>";

            // Restituzione dell'Excel
            return toReturn;
        }

        /// <summary>
        /// Funzione per la creazione delle righe del foglio Excel con le informaizoni
        /// sui fascicoli
        /// </summary>
        /// <param name="projects">Fascicoli da esportare</param>
        /// <param name="selectedFields">Campi da esportare</param>
        /// <param name="registry">Registro</param>
        /// <param name="grid">Griglia da cui reperire le informazioni sui campi</param>
        /// <returns>Excel da mostrare</returns>
        private string CreateProjectsData(ArrayList projects, ArrayList selectedFields, Registro registry, Grid grid, InfoUtente userInfo)
        {
            // Valore da restituire
            String toReturn = String.Empty;

            // Per ogni fascicolo, esportazione dei dati sul fascicolo
            foreach (Fascicolo project in projects)
                toReturn += this.InsertProjectData(project, selectedFields, registry, grid, userInfo);

            // Restituzione dell'Excel
            return toReturn;
        }

        /// <summary>
        /// Funzione per l'export dei dati di un documento
        /// </summary>
        /// <param name="document">Documenti di cui esportare i dati</param>
        /// <param name="selectedFields">Campi da visualizzare</param>
        /// <param name="grid">Griglia da utilizzare per recuperare i dati sui campi da mostrare</param>
        /// <param name="userInfo">Informazioni sull'utente</param>
        /// <returns>XML Excel</returns>
        private string InsertProjectData(Fascicolo project, ArrayList selectedFields, Registro registry, Grid grid, InfoUtente userInfo)
        {
            // Riga da restituire
            string toReturn = String.Empty;

            // Proprietà del campo da visualizzare
            Field field;

            // Valore da scrivere nell'XML
            String value;

            // Apertura riga
            toReturn = "<Row>";

            // Template del documento (caricato solo se c'è almeno un campo profilato)
            Templates template = null;

            // Inserimento dei valori per i campi
            foreach (CampoSelezionato selectedField in selectedFields)
            {

                if (selectedField.SystemId > 0)
                    field = grid.Fields.Where(e => e.CustomObjectId.Equals(selectedField.SystemId)).First();
                else
                    // Recupero delle informazioni sul campo
                    field = grid.Fields.Where(e => e.Label.Equals(selectedField.nomeCampo)).First();

                // Costruzione del nome del campo
                String fieldName = field.CustomObjectId == 0 ? field.OriginalLabel : field.OriginalLabel + field.CustomObjectId;

                StringBuilder temp;

                // Selezione del valore da mostrare
                switch (fieldName.ToUpper())
                {
                    case "APERTURA":
                        value = project.apertura;
                        break;
                    case "CARTACEO":
                        value = project.cartaceo ? "Si" : "No";
                        break;
                    case "CHIUSURA":
                        value = project.chiusura;
                        break;
                    case "CODICE":
                        value = project.codice;
                        break;
                    case "CODICE CLASSIFICA":
                        value = project.codiceGerarchia;
                        break;
                    case "AOO":
                        value = project.codiceRegistroNodoTit;
                        break;
                    case "DESCRIZIONE":
                        value = project.descrizione;
                        break;
                    case "IN ARCHIVIO":
                        value = !String.IsNullOrEmpty(project.inArchivio) && project.inArchivio != "0" ? "Si" : "No";
                        break;
                    case "IN CONSERVAZIONE":
                        value = !String.IsNullOrEmpty(project.inConservazione) && project.inConservazione != "0" ? "Si" : "No";
                        break;
                    case "IN SCARTO":
                        value = !String.IsNullOrEmpty(project.inScarto) && project.inScarto != "0" ? "Si" : "No";
                        break;
                    case "NOTE":
                        value = !String.IsNullOrEmpty(project.ultimaNota) ? project.ultimaNota : String.Empty;
                        break;
                    case "NUMERO":
                        value = project.numFascicolo;
                        break;
                    case "NUMERO MESI IN CONSERVAZIONE":
                        value = !String.IsNullOrEmpty(project.numMesiConservazione) ? project.numMesiConservazione : String.Empty;
                        break;
                    case "PRIVATO":
                        value = project.privato != "0" ? "Si" : "No";
                        break;
                    case "STATO":
                        value = project.stato == "A" ? "Aperto" : "Chiuso";
                        break;
                    case "TIPO":
                        value = project.tipo == "G" ? "Generale" : "Procedimentale";
                        break;
                    case "TIPOLOGIA":
                        value = !String.IsNullOrEmpty(project.template.DESCRIZIONE) ? project.template.DESCRIZIONE : String.Empty;
                        break;

                    default:
                        if (template == null)
                            template = ProfilazioneFascicoli.getTemplateFascDettagli(project.systemID);

                        value = this.GetValueForCustomObject(fieldName, template);
                        break;
                }

                // Se il campo è standard vengono aggiunti determinati tag altrimenti ne vengono aggiunti altri
                if (field.CustomObjectId == 0)
                {
                    toReturn += "<Cell>";
                    toReturn += "<Data ss:Type=\"String\">" + value;
                    toReturn += "</Data>";
                    toReturn += "</Cell>";
                }
                else
                {
                    toReturn += "<Cell>";
                    toReturn += "<Data ss:Type=\"String\"><![CDATA[";
                    toReturn += value;
                    toReturn += "]]></Data>";
                    toReturn += "</Cell>";
                }

            }

            // Chiusura della riga
            toReturn += "</Row>";

            // Restituzione della riga creata
            return toReturn;
        }

        private string sheetFascicoli(ArrayList fascicoli, ArrayList campiSelezionati, DocsPaVO.utente.Registro registro)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"Fascicoli\">";
            strXML += "<Table>";
            strXML += creaTabellaFascicoli(campiSelezionati);
            strXML += datiFascicoliXML(fascicoli, campiSelezionati, registro);
            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }

        private string creaTabellaFascicoli(ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                //strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"90\"/>";
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                switch (campoSelezionato.nomeCampo)
                {
                    case "Registro":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"45\"/>";
                        break;
                    case "Tipo":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"25\"/>";
                        break;

                    case "Codice":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"90\"/>";
                        break;

                    case "Descrizione":
                        strXML += "<Column ss:StyleID=\"s64\" ss:Width=\"250\"/>";
                        break;

                    case "Data Apertura":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"70\"/>";
                        break;

                    case "Data Chiusura":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"70\"/>";
                        break;

                    case "Collocazione Fisica":
                        strXML += "<Column ss:StyleID=\"s64\" ss:Width=\"100\"/>";
                        break;

                    default:
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"90\"/>";
                        break;
                }
            }
            return strXML;
        }

        private string datiFascicoliXML(ArrayList fascicoli, ArrayList campiSelezionati, DocsPaVO.utente.Registro registro)
        {
            string strXML = string.Empty;
            strXML = creaColonneFascicoli(campiSelezionati);
            strXML += inserisciDatiFascicoli(fascicoli, campiSelezionati, registro);
            return strXML;
        }

        private string creaColonneFascicoli(ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            strXML += "<Row>";

            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                strXML += "<Cell ss:StyleID=\"s70\">";
                strXML += "<Data ss:Type=\"String\">" + ((DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i]).nomeCampo.ToString();
                strXML += "</Data>";
                strXML += "</Cell>";
            }

            strXML += "</Row>";
            return strXML;
        }

        private string inserisciDatiFascicoli(ArrayList fascicoli, ArrayList campiSelezionati, DocsPaVO.utente.Registro registro)
        {
            string righe = string.Empty;
            foreach (DocsPaVO.fascicolazione.Fascicolo fascicolo in fascicoli)
            {
                righe += inserisciRigaFascicoli(fascicolo, campiSelezionati, registro);
            }
            return righe;
        }

        private string inserisciRigaFascicoli(DocsPaVO.fascicolazione.Fascicolo fascicolo, ArrayList campiSelezionati, DocsPaVO.utente.Registro registro)
        {
            string riga = string.Empty;

            riga = "<Row>";

            //Inserimento Campi Standard
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                switch (campoSelezionato.nomeCampo)
                {
                    case "Registro":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\">#" + fascicolo.codiceRegistroNodoTit;
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;
                    case "Tipo":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\">#" + fascicolo.tipo;
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;

                    case "Codice":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\">#" + fascicolo.codice;
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;

                    case "Descrizione":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\"><![CDATA[#" + fascicolo.descrizione + "]]>";
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;

                    case "Data Apertura":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\">#" + fascicolo.apertura;
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;

                    case "Data Chiusura":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\">#" + fascicolo.chiusura;
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;

                    case "Collocazione Fisica":
                        BusinessLogic.ExportDati.ExportDatiManager exportManager = new ExportDatiManager();
                        if (fascicolo.idUoLF != null && fascicolo.idUoLF != string.Empty)
                        {
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">#" + exportManager.getLocazioneFisica(fascicolo.idUoLF);
                            riga += "</Data>";
                            riga += "</Cell>";
                        }
                        else
                        {
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">#";
                            riga += "</Data>";
                            riga += "</Cell>";
                        }
                        break;

                    case "Note":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\"><![CDATA[#" + fascicolo.ultimaNota + "]]>"; ;
                        riga += "</Data>";
                        riga += "</Cell>";

                        break;

                    default:
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\"><![CDATA[#";
                        riga += "]]></Data>";
                        riga += "</Cell>";
                        break;
                }
            }

            string[] splitRiga = riga.Split('#');

            //Inserimanto Campi della profilazione dinamica
            if (fascicolo.template != null && fascicolo.systemID != null)
            {
                DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascDettagli(fascicolo.systemID);
                if (template != null && template.ELENCO_OGGETTI.Count != 0)
                {
                    //I Campi Selezionati sono solo campi comuni
                    if (isAllCampiComuni(campiSelezionati))
                    {
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                        {
                            for (int i = 0; i < campiSelezionati.Count; i++)
                            {
                                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                                if (oggettoCustom != null && oggettoCustom.CAMPO_COMUNE == "1" && campoSelezionato.nomeCampo == oggettoCustom.DESCRIZIONE)
                                {
                                    splitRiga[i + 1] = splitRiga[i + 1].Replace("]]></Data>", getValoreOggettoCustom(oggettoCustom) + "]]></Data>");
                                }
                            }
                        }
                    }
                    //I Campi selezionati sono di una specifica tipologia
                    else
                    {
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                        {
                            for (int i = 0; i < campiSelezionati.Count; i++)
                            {
                                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                                if (oggettoCustom != null && campoSelezionato.nomeCampo == oggettoCustom.DESCRIZIONE && campoSelezionato.campoStandard != "1")
                                {
                                    splitRiga[i + 1] = splitRiga[i + 1].Replace("]]></Data>", getValoreOggettoCustom(oggettoCustom) + "]]></Data>");
                                }
                            }
                        }
                    }
                }
            }

            //Ricompongo la string riga 
            riga = string.Empty;
            foreach (string stringaRiga in splitRiga)
            {
                riga += stringaRiga;
            }

            riga += "</Row>";
            return riga;
        }

        #endregion



        #region Trasmissioni Custom

        private void exportTrasmODS(DocsPaVO.trasmissione.OggettoTrasm oggettoTrasmesso, string tipoRicerca, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo, ArrayList campiSelezionati)
        {
            try
            {
                //Recupero i dati
                ArrayList trasmissioni = new ArrayList();
                int totalPageNumber;
                int recordCount;

                switch (tipoRicerca)
                {
                    case "R":
                        trasmissioni = BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryRicevuteMethodPagingLite(oggettoTrasmesso, utente, ruolo, this._filtriTrasm, 1, true, 1, out totalPageNumber, out recordCount);
                        break;

                    case ("E"):
                        // INC000000528056
                        // Export trasmissioni effettuate con filtro destinatario valorizzato
                        //trasmissioni = BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryEffettuateMethodPagingLite(oggettoTrasmesso, utente, ruolo, this._filtriTrasm, 1, true, 1, out totalPageNumber, out recordCount);
                        trasmissioni = BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryEffettuateMethodPagingLiteWithoutTrasmUtente(oggettoTrasmesso, utente, ruolo, this._filtriTrasm, 1, true, 1, out totalPageNumber, out recordCount);
                        break;
                }

                //Cerco tutti i destinatari delle trasmissioni che mi serviranno inseguito
                Dictionary<string, string> destTrasm = new Dictionary<string, string>();
                destTrasm = Trasmissioni.TrasmManager.getDestinatariTrasmByListaTrasm(trasmissioni);

                DataSet docExp = new DataSet();

                string tipoOgetto = (from i in this._filtriTrasm where i.argomento.Equals(DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO.ToString()) select i.valore).FirstOrDefault();
                if (tipoOgetto == "F")
                    docExp = createOpenOfficeTrasmFasc(trasmissioni, campiSelezionati, utente.idPeople, ruolo.idGruppo, destTrasm);
                else
                    docExp = createOpenOfficeTrasm(trasmissioni, campiSelezionati, utente.idPeople, ruolo.idGruppo, destTrasm);

                string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                serverPath = serverPath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                string userDoc = "\\export_trasmissioni" + infoUser.idPeople + ".ods";
                OpenDocument.OpenDocumentServices open = new OpenDocument.OpenDocumentServices(serverPath + userDoc);
                open.SetTitle("Export trasmissioni - Trovate " + trasmissioni.Count + " trasmissioni");
                open.SetSubtitle(this._title);
                open.SetData(docExp);
                this._file = open.SaveAndExportData();
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione trasmissioni : " + ex.Message);
            }
        }

        private DataSet createOpenOfficeTrasm(ArrayList trasmissioni, ArrayList campiSelezionati, string idPeople, string idGruppo, Dictionary<string, string> destTrasm)
        {
            DataSet result = new DataSet();
            DataTable dt = new DataTable();
            DataRow column = dt.NewRow();
            String value;
            int i = 0;
            foreach (DocsPaVO.trasmissione.Trasmissione trasmissione in trasmissioni)
            {
                // Riga da restituire
                DataRow toReturn = dt.NewRow();


                // Valore da scrivere nell'XML


                // Inserimento dei valori per i campi
                foreach (CampoSelezionato selectedField in campiSelezionati)
                {
                    // field = grid.Fields.Where(e => e.FieldId.Equals(selectedField.fieldID)).First();

                    // Selezione del valore da mostrare
                    switch (selectedField.nomeCampo)
                    {
                        //Data Trasm.
                        case "Data Trasm.":
                            value = trasmissione.dataInvio;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                        //Documento Trasmesso"
                        case "Documento Trasmesso":
                            value = getInfoDocTrasmXSL(trasmissione, idPeople, idGruppo).Replace("<br>", " ; ");
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                        //Mittenti
                        case "Mittenti":
                            value = "(" + trasmissione.ruolo.descrizione + ") " + trasmissione.utente.descrizione;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                        //Destinatari
                        case "Destinatari":
                            value = getDestinatariTrasmLite(trasmissione, destTrasm);
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                    }

                }
                if (i == 0)
                {
                    dt.Rows.Add(column);
                }

                i++;
                dt.Rows.Add(toReturn);
            }
            result.Tables.Add(dt);
            return result;
        }

        private DataSet createOpenOfficeTrasmFasc(ArrayList trasmissioni, ArrayList campiSelezionati, string idPeople, string idGruppo, Dictionary<string, string> destTrasm)
        {
            DataSet result = new DataSet();
            DataTable dt = new DataTable();
            DataRow column = dt.NewRow();
            String value;
            int i = 0;
            foreach (DocsPaVO.trasmissione.Trasmissione trasmissione in trasmissioni)
            {
                // Riga da restituire
                DataRow toReturn = dt.NewRow();


                // Valore da scrivere nell'XML


                // Inserimento dei valori per i campi
                foreach (CampoSelezionato selectedField in campiSelezionati)
                {
                    // field = grid.Fields.Where(e => e.FieldId.Equals(selectedField.fieldID)).First();

                    // Selezione del valore da mostrare
                    switch (selectedField.fieldID)
                    {
                        //Data Trasm.
                        case "DATA_INVIO":
                            value = trasmissione.dataInvio;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                        //Documento Trasmesso"
                        case "COD_FASCICOLO":
                            value = trasmissione.infoFascicolo.codice;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                        //Mittenti
                        case "DESC_FASCICOLO":
                            value = trasmissione.infoFascicolo.descrizione;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                        //Destinatari
                        case "DATA_APERTURA":
                            value = trasmissione.infoFascicolo.apertura;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                    }

                }
                if (i == 0)
                {
                    dt.Rows.Add(column);
                }

                i++;
                dt.Rows.Add(toReturn);
            }
            result.Tables.Add(dt);
            return result;
        }

        private void exportTrasmXLS(DocsPaVO.trasmissione.OggettoTrasm oggettoTrasmesso, string tipoRicerca, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo, ArrayList campiSelezionati)
        {
            try
            {
                //Recupero i dati
                ArrayList trasmissioni = new ArrayList();
                int totalPageNumber;
                int recordCount;

                switch (tipoRicerca)
                {
                    case "R":
                        trasmissioni = BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryRicevuteMethodPagingLite(oggettoTrasmesso, utente, ruolo, this._filtriTrasm, 1, true, 1, out totalPageNumber, out recordCount);
                        break;

                    case ("E"):
                        // INC000000528056
                        // Export trasmissioni effettuate con filtro destinatario valorizzato
                        //trasmissioni = BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryEffettuateMethodPagingLite(oggettoTrasmesso, utente, ruolo, this._filtriTrasm, 1, true, 1, out totalPageNumber, out recordCount);
                        trasmissioni = BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryEffettuateMethodPagingLiteWithoutTrasmUtente(oggettoTrasmesso, utente, ruolo, this._filtriTrasm, 1, true, 1, out totalPageNumber, out recordCount);
                        break;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLTrasm(trasmissioni, campiSelezionati, utente.idPeople, ruolo.idGruppo, null);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportTrasmissioni.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportTrasmissioni.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportTrasmissioni";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione trasmissioni : " + ex.Message);
            }
        }

        private void exportToDoListXLS(DocsPaVO.utente.InfoUtente infoUtente, string docOrFasc, string registri, ArrayList campiSelezionati, String[] objectId)
        {
            try
            {
                //Recupero i dati
                ArrayList trasmissioni = new ArrayList();
                trasmissioni = BusinessLogic.Trasmissioni.QueryTrasmManager.getToDoList(infoUtente, docOrFasc, registri, this._filtriTrasm, objectId);

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLTrasm(trasmissioni, campiSelezionati, infoUtente.idPeople, infoUtente.idGruppo, docOrFasc);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportTrasmissioniToDoList.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportTrasmissioniToDoList.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportTrasmissioniToDoList";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione trasmissioni toDoList : " + ex.Message);
            }
        }

        private StringBuilder creaXMLTrasm(ArrayList trasmissioni, ArrayList campiSelezionati, string idPeople, string idGruppo, string docOrFasc)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += sheetTrasmissioni(trasmissioni, campiSelezionati, docOrFasc, idPeople, idGruppo);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());

            return sb;
        }

        private string sheetTrasmissioni(ArrayList trasmissioni, ArrayList campiSelezionati, string docOrFasc, string idPeople, string idGruppo)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"Trasmissioni\">";
            strXML += "<Table>";
            string tipoOgetto = (from i in this._filtriTrasm where i.argomento.Equals(DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO.ToString()) select i.valore).FirstOrDefault();
            if (tipoOgetto == "F")
                strXML += creaTabellaTrasmissioniFasc(campiSelezionati);
            else
                strXML += creaTabellaTrasmissioni(campiSelezionati);
            strXML += datiTrasmissioniXML(trasmissioni, campiSelezionati, docOrFasc, idPeople, idGruppo);
            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }

        private string creaTabellaTrasmissioni(ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                //strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"90\"/>";
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                switch (campoSelezionato.nomeCampo)
                {
                    case "Data Trasm.":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"80\"/>";
                        break;

                    case "Documento Trasmesso":
                        strXML += "<Column ss:StyleID=\"s64\" ss:Width=\"220\"/>";
                        break;

                    case "Mittenti":
                        strXML += "<Column ss:StyleID=\"s64\" ss:Width=\"170\"/>";
                        break;

                    case "Destinatari":
                        strXML += "<Column ss:StyleID=\"s64\" ss:Width=\"170\"/>";
                        break;

                    default:
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"90\"/>";
                        break;
                }
            }
            return strXML;
        }

        private string creaTabellaTrasmissioniFasc(ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                //strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"90\"/>";
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                switch (campoSelezionato.fieldID)
                {
                    case "DATA_INVIO":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"80\"/>";
                        break;

                    case "COD_FASCICOLO":
                        strXML += "<Column ss:StyleID=\"s64\" ss:Width=\"100\"/>";
                        break;

                    case "DESC_FASCICOLO":
                        strXML += "<Column ss:StyleID=\"s64\" ss:Width=\"220\"/>";
                        break;

                    case "DATA_APERTURA":
                        strXML += "<Column ss:StyleID=\"s64\" ss:Width=\"80\"/>";
                        break;

                    default:
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"90\"/>";
                        break;
                }
            }
            return strXML;
        }


        private string datiTrasmissioniXML(ArrayList trasmissioni, ArrayList campiSelezionati, string docOrFasc, string idPeople, string idGruppo)
        {
            string strXML = string.Empty;
            strXML = creaColonneTrasmissioni(campiSelezionati);
            strXML += inserisciDatiTrasmissioni(trasmissioni, campiSelezionati, docOrFasc, idPeople, idGruppo);
            return strXML;
        }

        private string inserisciDatiTrasmissioni(ArrayList trasmissioni, ArrayList campiSelezionati, string docOrFasc, string idPeople, string idGruppo)
        {
            string righe = string.Empty;

            if (trasmissioni.Count != 0)
            {
                if (trasmissioni[0].GetType().IsInstanceOfType(new DocsPaVO.trasmissione.Trasmissione()))
                {
                    //Cerco tutti i destinatari delle trasmissioni che mi serviranno inseguito
                    Dictionary<string, string> destTrasm = new Dictionary<string, string>();
                    destTrasm = Trasmissioni.TrasmManager.getDestinatariTrasmByListaTrasm(trasmissioni);
                    string tipoOgetto = (from i in this._filtriTrasm where i.argomento.Equals(DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO.ToString()) select i.valore).FirstOrDefault();
                    if (tipoOgetto == "F")
                    {
                        foreach (DocsPaVO.trasmissione.Trasmissione trasmissione in trasmissioni)
                        {
                            righe += inserisciRigaTrasmissioniFasc(trasmissione, campiSelezionati, docOrFasc, destTrasm, idPeople, idGruppo);
                        }
                    }
                    else
                    {
                        foreach (DocsPaVO.trasmissione.Trasmissione trasmissione in trasmissioni)
                        {
                            righe += inserisciRigaTrasmissioni(trasmissione, campiSelezionati, docOrFasc, destTrasm, idPeople, idGruppo);
                        }
                    }
                }

                if (trasmissioni[0].GetType().IsInstanceOfType(new DocsPaVO.trasmissione.infoToDoList()))
                {
                    foreach (DocsPaVO.trasmissione.infoToDoList trasmissione in trasmissioni)
                    {
                        righe += inserisciRigaTrasmissioniToDoList(trasmissione, campiSelezionati, docOrFasc, idPeople, idGruppo);
                    }
                }
            }

            return righe;
        }

        private string inserisciRigaTrasmissioni(DocsPaVO.trasmissione.Trasmissione trasmissione, ArrayList campiSelezionati, string docOrFasc, Dictionary<string, string> destTrasm, string idPeople, string idGruppo)
        {
            string riga = string.Empty;

            riga = "<Row>";

            //Inserimento Campi Standard
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                if (campoSelezionato.campoStandard == "1")
                {
                    switch (campoSelezionato.nomeCampo)
                    {
                        case "Data Trasm.":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + trasmissione.dataInvio;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Documento Trasmesso":
                            riga += "<Cell ss:StyleID=\"s64\">";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + getInfoDocTrasmXSL(trasmissione, idPeople, idGruppo).Replace("<br>", " ; ");
                            riga += "]]></Data>";
                            riga += "</Cell>";
                            break;

                        case "Mittenti":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\"><![CDATA[(" + trasmissione.ruolo.descrizione + ") " + trasmissione.utente.descrizione;
                            riga += "]]></Data>";
                            riga += "</Cell>";
                            break;

                        case "Destinatari":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + getDestinatariTrasmLite(trasmissione, destTrasm);
                            riga += "]]></Data>";
                            riga += "</Cell>";
                            break;
                    }
                }

            }

            riga += "</Row>";
            return riga;
        }

        private string inserisciRigaTrasmissioniFasc(DocsPaVO.trasmissione.Trasmissione trasmissione, ArrayList campiSelezionati, string docOrFasc, Dictionary<string, string> destTrasm, string idPeople, string idGruppo)
        {
            string riga = string.Empty;

            riga = "<Row>";

            //Inserimento Campi Standard
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                if (campoSelezionato.campoStandard == "1")
                {
                    switch (campoSelezionato.fieldID)
                    {
                        case "DATA_INVIO":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + trasmissione.dataInvio;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "COD_FASCICOLO":
                            riga += "<Cell ss:StyleID=\"s64\">";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + trasmissione.infoFascicolo.codice;
                            riga += "]]></Data>";
                            riga += "</Cell>";
                            break;

                        case "DESC_FASCICOLO":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + trasmissione.infoFascicolo.descrizione;
                            riga += "]]></Data>";
                            riga += "</Cell>";
                            break;

                        case "DATA_APERTURA":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + trasmissione.infoFascicolo.apertura;
                            riga += "]]></Data>";
                            riga += "</Cell>";
                            break;
                    }
                }

            }

            riga += "</Row>";
            return riga;
        }

        private string inserisciRigaTrasmissioniToDoList(DocsPaVO.trasmissione.infoToDoList trasmissione, ArrayList campiSelezionati, string docOrFasc, string idPeople, string idGruppo)
        {
            string riga = string.Empty;

            riga = "<Row>";

            //Inserimento Campi Standard
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                if (campoSelezionato.campoStandard == "1")
                {
                    switch (campoSelezionato.nomeCampo)
                    {
                        case "Data Trasm.":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + trasmissione.dataInvio;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Documento Trasmesso":
                            riga += "<Cell ss:StyleID=\"s64\">";
                            riga += "<Data ss:Type=\"String\"> " + getInfoDocToDoListXSL(docOrFasc, trasmissione, idPeople, idGruppo).Replace("<br>", " &#10; ");
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Mittenti":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + trasmissione.utenteMittente + " (" + trasmissione.ruoloMittente + ")";
                            riga += "]]></Data>";
                            riga += "</Cell>";
                            break;

                        case "Destinatari":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + trasmissione.utenteDestinatario;
                            riga += "]]></Data>";
                            riga += "</Cell>";
                            break;
                    }
                }
            }

            riga += "</Row>";
            return riga;
        }

        private string creaColonneTrasmissioni(ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            strXML += "<Row>";

            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                strXML += "<Cell ss:StyleID=\"s70\">";
                strXML += "<Data ss:Type=\"String\">" + ((DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i]).nomeCampo.ToString();
                strXML += "</Data>";
                strXML += "</Cell>";
            }

            strXML += "</Row>";
            return strXML;
        }

        #endregion Trasmissioni Custom



        #region Export Indice Sistematico
        public DocsPaVO.documento.FileDocumento ExportIndiceSistematico(string serverPath, DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            DocsPaVO.documento.FileDocumento file = new DocsPaVO.documento.FileDocumento();

            try
            {
                //Recupero l'indice sistematico in base al titolario selezionato
                //per la creazione del foglio excel NODO --> VOCI
                DataSet ds_nodoVociIndice = new DataSet();
                ds_nodoVociIndice = Amministrazione.TitolarioManager.GetIndiceSistematico(titolario, " ORDER BY VAR_COD_LIV1");

                //Recupero l'indice sistematico in base al titolario selezionato
                //per la creazione del foglio excel VOCE --> NODI
                DataSet ds_voceNodiDiTitolario = new DataSet();
                ds_voceNodiDiTitolario = Amministrazione.TitolarioManager.GetIndiceSistematico(titolario, " ORDER BY VOCE_INDICE");

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                string strXML = string.Empty;

                //Intestazione XML
                strXML += topXML();

                //Aggiungo una serie di stili utili alla grafica del foglio
                strXML += stiliXML();

                //Fogli Excel - Nodo --> Voci
                strXML += sheetIndice(ds_nodoVociIndice, ds_voceNodiDiTitolario);

                strXML += "</Workbook>";

                sb.Append(strXML.ToString());

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportIndice.xls");
                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportIndice.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    file.content = contentExcel;
                    file.length = contentExcel.Length;
                    file.estensioneFile = "xls";
                    file.name = "ExportIndice";
                    file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                file = null;
                logger.Debug("Errore esportazione indice titolario : " + ex.Message);
            }

            return file;
        }

        private string sheetIndice(DataSet ds_nodoVociIndice, DataSet ds_voceNodiDiTitolario)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"NODO-VOCI\">";
            strXML += "<Table>";
            strXML += creaTabellaIndice("NODO-VOCI");
            strXML += datiIndiceNodoVociXML(ds_nodoVociIndice);
            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";

            strXML += "<Worksheet ss:Name=\"VOCE-NODI\">";
            strXML += "<Table>";
            strXML += creaTabellaIndice("VOCE-NODI");
            strXML += datiIndiceVoceNodiXML(ds_voceNodiDiTitolario);
            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";

            return strXML;
        }

        private string creaTabellaIndice(string ordineColonne)
        {
            string strXML = string.Empty;
            switch (ordineColonne)
            {
                case "NODO-VOCI":
                    strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"80\"/>";
                    strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"200\"/>";
                    strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"80\"/>";
                    strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"320\"/>";
                    break;

                case "VOCE-NODI":
                    strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"200\"/>";
                    strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"400\"/>";
                    break;
            }

            return strXML;
        }

        private string datiIndiceNodoVociXML(DataSet ds_nodoVociIndice)
        {
            string strXML = string.Empty;
            strXML = creaColonneIndice("NODO-VOCI");
            strXML += inserisciDatiIndiceNodoVoci(ds_nodoVociIndice);
            return strXML;
        }

        private string datiIndiceVoceNodiXML(DataSet ds_voceNodiDiTitolario)
        {
            string strXML = string.Empty;
            strXML = creaColonneIndice("VOCE-NODI");
            strXML += inserisciDatiIndiceVoceNodi(ds_voceNodiDiTitolario);
            return strXML;
        }

        private string creaColonneIndice(string ordineColonne)
        {
            string strXML = string.Empty;
            strXML += "<Row>";

            switch (ordineColonne)
            {
                case "NODO-VOCI":
                    //Colonna CODICE NODO
                    strXML += "<Cell ss:StyleID=\"s62\">";
                    strXML += "<Data ss:Type=\"String\">CODICE_NODO";
                    strXML += "</Data>";
                    strXML += "</Cell>";
                    //Colonna DESCRIZIONE NODO
                    strXML += "<Cell ss:StyleID=\"s62\">";
                    strXML += "<Data ss:Type=\"String\">DESCRIZIONE_NODO";
                    strXML += "</Data>";
                    strXML += "</Cell>";
                    //Colonna CODICE REGISTRO
                    strXML += "<Cell ss:StyleID=\"s62\">";
                    strXML += "<Data ss:Type=\"String\">CODICE_REGISTRO";
                    strXML += "</Data>";
                    strXML += "</Cell>";
                    //Colonna VOCI
                    strXML += "<Cell ss:StyleID=\"s62\">";
                    strXML += "<Data ss:Type=\"String\">VOCI_DI_INDICE";
                    strXML += "</Data>";
                    strXML += "</Cell>";
                    break;

                case "VOCE-NODI":
                    //Colonna VOCE
                    strXML += "<Cell ss:StyleID=\"s62\">";
                    strXML += "<Data ss:Type=\"String\">VOCE_DI_INDICE";
                    strXML += "</Data>";
                    strXML += "</Cell>";
                    //Colonna NODI
                    strXML += "<Cell ss:StyleID=\"s62\">";
                    strXML += "<Data ss:Type=\"String\">NODI_DI_TITOLARIO";
                    strXML += "</Data>";
                    strXML += "</Cell>";
                    break;
            }

            strXML += "</Row>";
            return strXML;

        }

        private string inserisciDatiIndiceNodoVoci(DataSet ds_nodoVociIndice)
        {
            string righe = string.Empty;
            if ((ds_nodoVociIndice.Tables[0] != null) && ds_nodoVociIndice.Tables[0].Rows.Count > 0)
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                int start = 0;

                string vociIndice = string.Empty;
                string codiceNodo = ds_nodoVociIndice.Tables[0].Rows[0]["VAR_CODICE"].ToString();
                string descrizioneNodo = ds_nodoVociIndice.Tables[0].Rows[0]["DESCRIPTION"].ToString();
                string codiceRegistro = string.Empty;
                if (!string.IsNullOrEmpty(ds_nodoVociIndice.Tables[0].Rows[0]["ID_REGISTRO"].ToString()))
                    codiceRegistro = amm.GetCodiceRegistro(ds_nodoVociIndice.Tables[0].Rows[0]["ID_REGISTRO"].ToString());

                foreach (DataRow row in ds_nodoVociIndice.Tables[0].Rows)
                {
                    if (start == 0)
                    {
                        vociIndice = row["VOCE_INDICE"].ToString();
                    }
                    else
                    {
                        //Codici nodo uguali e registri nulli
                        if (codiceNodo == (row["VAR_CODICE"].ToString()) &&
                            string.IsNullOrEmpty(row["ID_REGISTRO"].ToString()) &&
                            !string.IsNullOrEmpty(row["VOCE_INDICE"].ToString()))
                        {
                            vociIndice += row["VOCE_INDICE"].ToString() + ";";
                        }

                        //Codici nodo uguali e registri uguali
                        if (codiceNodo == (row["VAR_CODICE"].ToString()) &&
                            !string.IsNullOrEmpty(row["ID_REGISTRO"].ToString()) &&
                            codiceRegistro == amm.GetCodiceRegistro(row["ID_REGISTRO"].ToString()) &&
                            !string.IsNullOrEmpty(row["VOCE_INDICE"].ToString()))
                        {
                            vociIndice += row["VOCE_INDICE"].ToString() + ";";
                        }

                        //Inserimento riga
                        if (codiceNodo != (row["VAR_CODICE"].ToString()))
                        {
                            righe += inserisciRigaIndiceNodoVoci(codiceNodo, descrizioneNodo, codiceRegistro, vociIndice);
                            vociIndice = row["VOCE_INDICE"].ToString();
                            codiceNodo = row["VAR_CODICE"].ToString();
                            descrizioneNodo = row["DESCRIPTION"].ToString();
                            if (!string.IsNullOrEmpty(row["ID_REGISTRO"].ToString()))
                                codiceRegistro = amm.GetCodiceRegistro(row["ID_REGISTRO"].ToString());
                        }
                    }
                    start++;
                }

                righe += inserisciRigaIndiceNodoVoci(codiceNodo, descrizioneNodo, codiceRegistro, vociIndice);
            }

            return righe;
        }

        private string inserisciDatiIndiceVoceNodi(DataSet ds_voceNodiDiTitolario)
        {
            string righe = string.Empty;
            if ((ds_voceNodiDiTitolario.Tables[0] != null) && ds_voceNodiDiTitolario.Tables[0].Rows.Count > 0)
            {
                int start = 0;
                string voceIndice = ds_voceNodiDiTitolario.Tables[0].Rows[0]["VOCE_INDICE"].ToString();
                string nodiDiTitolario = string.Empty;

                foreach (DataRow row in ds_voceNodiDiTitolario.Tables[0].Rows)
                {
                    if (start == 0)
                    {
                        nodiDiTitolario = row["VAR_CODICE"].ToString() + " - " + row["DESCRIPTION"].ToString();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(voceIndice))
                        {
                            if (voceIndice == (row["VOCE_INDICE"].ToString()))
                            {
                                nodiDiTitolario += row["VAR_CODICE"].ToString() + " - " + row["DESCRIPTION"].ToString() + ";";
                            }
                            else
                            {
                                righe += inserisciRigaIndiceVoceNodi(voceIndice, nodiDiTitolario);
                                voceIndice = row["VOCE_INDICE"].ToString();
                                nodiDiTitolario = row["VAR_CODICE"].ToString() + " - " + row["DESCRIPTION"].ToString();
                            }
                        }
                    }
                    start++;
                }
            }

            return righe;
        }

        private static string inserisciRigaIndiceNodoVoci(string codiceNodo, string descrizioneNodo, string codiceRegistro, string vociIndice)
        {
            string riga = string.Empty;
            riga = "<Row>";

            //Colonna Codice Nodo
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + codiceNodo;
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Descrizione Nodo
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\"><![CDATA[" + descrizioneNodo + "]]>";
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Codice Registro
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\">" + codiceRegistro;
            riga += "</Data>";
            riga += "</Cell>";

            //Colonna Voci Indice
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\"><![CDATA[" + vociIndice + "]]>";
            riga += "</Data>";
            riga += "</Cell>";

            riga += "</Row>";
            return riga;
        }

        private static string inserisciRigaIndiceVoceNodi(string voce, string nodi)
        {
            string riga = string.Empty;
            riga = "<Row>";

            //Colonna Voce
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\"><![CDATA[" + voce + "]]>";
            riga += "</Data>";

            riga += "</Cell>";
            //Colonna Nodi
            riga += "<Cell>";
            riga += "<Data ss:Type=\"String\"><![CDATA[" + nodi + "]]>";
            riga += "</Data>";
            riga += "</Cell>";

            riga += "</Row>";
            return riga;
        }
        #endregion Export Indice Sistematico

        #endregion Esportazioni in Excel tramite XML


        #region Esportazione in PDF

        #region Documenti
        /// <summary>
        /// Generazione del file (PDF) di export dei documenti
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>		
        /// <returns></returns>
        private void exportDocPDF(DocsPaVO.utente.InfoUtente infoUtente, bool mittDest_indirizzo, String[] documentsSystemId)
        {
            try
            {
                this._objList = Documenti.InfoDocManager.getQueryExport(infoUtente, this._filtri, mittDest_indirizzo, documentsSystemId);

                if (this._objList.Count > 0)
                {
                    this._rowsList = Convert.ToString(this._objList.Count);

                    // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                    // iTextSharp per la stampa dei risultati della ricerca
                    // ...creazione del report PDF con iTextSharp
                    this._file = this.CreaPDFRisRic("D", "");
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        /// <summary>
        /// Conversione dei dati dei documenti in formato XML
        /// </summary>		
        private void exportDocToXML()
        {
            this.addAttToRootNode();

            /* per i doc usiamo questi dati
            Numero_protocollo,
            Data_di_protocollo,
            Tipologia_di_documento,
            Data_annullamento,
            Oggetto,
            Mittenti_o_destinatari,
            Codice_del_fascicolo_che_contiene_il_documento	
            Codice_registro
            Immagine
            */

            foreach (DocsPaVO.documento.InfoDocumentoExport doc in this._objList)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                XmlElement numProt = this._xmlDoc.CreateElement("NUM_PROTOCOLLO");
                numProt.InnerText = doc.idOrNumProt;
                record.AppendChild(numProt);

                XmlElement dataProt = this._xmlDoc.CreateElement("DATA_PROTOCOLLO");
                dataProt.InnerText = doc.data;
                record.AppendChild(dataProt);

                XmlElement tipoDoc = this._xmlDoc.CreateElement("TIPO_DOC");
                //MODIFICATO DA FABIO PER CAMBIO ETICHETTE
                // Aggiunta del tipo di documento
                tipoDoc.InnerText = getLettereProtocolli(doc.tipologiaDocumento);
                record.AppendChild(tipoDoc);

                XmlElement dataAnn = this._xmlDoc.CreateElement("DATA_ANNULLA");
                dataAnn.InnerText = doc.dataAnnullamento;
                record.AppendChild(dataAnn);

                XmlElement oggetto = this._xmlDoc.CreateElement("OGGETTO");
                oggetto.InnerText = doc.oggetto;
                record.AppendChild(oggetto);

                XmlElement mittDest = this._xmlDoc.CreateElement("MITT_DEST");
                mittDest.InnerText = doc.mittentiDestinatari;
                record.AppendChild(mittDest);

                XmlElement codFasc = this._xmlDoc.CreateElement("COD_FASC");
                codFasc.InnerText = doc.codiceFascicolo;
                record.AppendChild(codFasc);



                XmlElement codReg = this._xmlDoc.CreateElement("COD_REG");
                codReg.InnerText = doc.codiceRegistro;
                record.AppendChild(codReg);

                XmlElement immagine = this._xmlDoc.CreateElement("IMG");
                if (Documenti.CacheFileManager.isFileInCache(doc.docNumber))
                    immagine.InnerText = "SI";
                else
                    immagine.InnerText = doc.acquisitaImmagine;
                record.AppendChild(immagine);


                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }

        /// <summary>
        /// Generazione del file pdf di export dei documenti con ricerca fulltext 
        ///</summary>
        private void exportDocRicFullTextPDF(ArrayList documenti)
        {
            try
            {
                if (documenti.Count > 0)
                {
                    this._rowsList = Convert.ToString(documenti.Count);

                    // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                    // iTextSharp per la stampa dei risultati della ricerca
                    // ...impostazione della lista degli oggetti
                    this._objList = documenti;

                    // ...creazione del report PDF con iTextSharp
                    this._file = this.CreaPDFRisRic("D", "");
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DocsPaVO.documento.FileDocumento CreaPDFRisRic(string tipoObj, string param)
        {
            // Il nome del file di template
            string templateFileName;
            // Il datatable utilizzato per creare la tabella con i dati da  visualizzare
            DataTable infoObjs;

            // 1. Determinazione della tipologia di oggetto di cui stampare il report
            switch (tipoObj)
            {
                case "D":   // Documenti
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisRicDoc.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicDoc();
                    break;
                case "F":   // Fascicoli
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisRicFasc.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicFasc(param);
                    break;

                case "T":   // Trasmissioni
                    // Caricamento dei dati da inserire nel report
                    string tipoOgetto = (from i in this._filtriTrasm where i.argomento.Equals(DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO.ToString()) select i.valore).FirstOrDefault();
                    if (tipoOgetto == "F")
                    {
                        // Impostazione del nome del file
                        templateFileName = "XMLRepStampaRisRicTrasmFasc.xml";
                        infoObjs = this.GetDataTableRicTrasmFasc();
                    }
                    else
                    {
                        // Impostazione del nome del file
                        templateFileName = "XMLRepStampaRisRicTrasm.xml";
                        infoObjs = this.GetDataTableRicTrasm();
                    }
                    break;

                case "DF":  // Documenti in fascicolo
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisRicDoc.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicDocInFasc(param);
                    break;

                case "TDL": // To do list di qualsiasi tipo tranne che D
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisRicToDoList.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicToDoList();
                    break;

                case "DC":  // Documenti in cestino
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisDocInCestino.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicDocInCestino();
                    break;

                case "S":   // Scarto
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisScarto.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicScarto();
                    break;

                case "C":   // Conservazione
                    // Impostazione del nome del file
                    templateFileName = "XMLRepExportConservazione.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableConservazione(param);
                    break;

                default:
                    throw new Exception("Tipo oggetto non valido: " + tipoObj);

            }

            // 2. Creazionedell'oggetto di generazione PDF
            StampaPDF.StampaRisRicerca report = new StampaPDF.StampaRisRicerca();

            // 3. Restituzione del file documento PDF con le informazioni sui
            //    risultati della ricerca
            return report.GetFileDocumento(templateFileName, this._title, this._descAmm,
     this._objList.Count.ToString(), infoObjs);

        }

        private DataTable GetDataTableConservazione(string rifiuto)
        {
            // Creazione del dataset con i dati sulla conservazione
            DataTable conservazione = new DataTable();
            DataRow item;

            // Creazione della struttura per conservazione
            conservazione.Columns.Add("TIPO_DOC");
            conservazione.Columns.Add("OGGETTO");
            conservazione.Columns.Add("CODICE_FASC");
            conservazione.Columns.Add("DATA_INSERIMENTO");
            conservazione.Columns.Add("ID_SEGNATURA_DATA");
            conservazione.Columns.Add("SIZE_BYTE");
            conservazione.Columns.Add("ESITO");

            // Aggiunta delle informazioni sulle voci di conservazione
            foreach (DocsPaVO.areaConservazione.ItemsConservazione itemCons in this._objList)
            {
                // Creazione di una nuova riga
                item = conservazione.NewRow();

                // Aggiunta delle informazioni sul tipo documento
                //Richiesta del 05/12/2012, nell report sostiuire G con NP
                if (itemCons.TipoDoc.Equals("G"))
                    item["TIPO_DOC"] = "NP";
                else
                    item["TIPO_DOC"] = itemCons.TipoDoc;

                // Aggiunta delle informazioni sull'oggetto
                item["OGGETTO"] = itemCons.desc_oggetto;

                // Aggiunta delle informazioni sul codice fascicolo
                item["CODICE_FASC"] = itemCons.CodFasc;

                // Aggiunta delle informazioni sulla data di inserimento
                item["DATA_INSERIMENTO"] = itemCons.Data_Ins;

                // Aggiunta delle informazioni sull'id della segnatura
                string segnatura = itemCons.numProt_or_id;
                string data = itemCons.data_prot_or_create;
                string data_doc = segnatura + "\n" + data;

                item["ID_SEGNATURA_DATA"] = data_doc;

                // Aggiunta delle informazioni sulla dimensione
                item["SIZE_BYTE"] = itemCons.SizeItem;

                // Aggiunta delle informazioni sull'esito

                //se sto rifiutando l'istanza senza neppure aver provato a eseguire la lavorazione
                if (itemCons.esitoLavorazione == string.Empty)
                {
                    item["ESITO"] = "-";
                }
                if (itemCons.esitoLavorazione == "1" && Boolean.Parse(rifiuto))
                {
                    item["ESITO"] = "-";
                }
                if (itemCons.esitoLavorazione == "0")
                {
                    item["ESITO"] = "File non trovato";
                }
                if (itemCons.esitoLavorazione == "1" && !Boolean.Parse(rifiuto))
                {
                    item["ESITO"] = "SI";
                }
                if (itemCons.esitoLavorazione == "2")
                {
                    item["ESITO"] = "Documento non trovato";
                }

                // Aggiunta della riga compilata al data table
                conservazione.Rows.Add(item);

            }

            // Restituzione del data table
            return conservazione;

        }

        private DataTable GetDataTableRicScarto()
        {
            // Creazione del dataset con i dati sui fascicoli da sottoporre a scarto
            DataTable scarto = new DataTable();
            DataRow item;

            // Creazione della struttura per scarto
            scarto.Columns.Add("TIPO");
            scarto.Columns.Add("CODCLASS");
            scarto.Columns.Add("CODICE");
            scarto.Columns.Add("DESCRIZIONE");
            scarto.Columns.Add("CHIUSURA");
            scarto.Columns.Add("MEDICONS");
            scarto.Columns.Add("MESICHIUSURA");

            // Aggiunta delle righe con le informaizoni sui fascicoli da aggiungere al report
            foreach (DocsPaVO.fascicolazione.Fascicolo fasc in this._objList)
            {
                // Creazione di una nuova riga
                item = scarto.NewRow();

                // Aggiunta delle informazioni sul tipo
                item["TIPO"] = fasc.tipo;

                // Aggiunta del codice di classifica
                item["CODCLASS"] = fasc.codiceGerarchia;

                // Aggiunta del codice
                item["CODICE"] = fasc.codice;

                // Aggiunta della descrizione
                item["DESCRIZIONE"] = fasc.descrizione;


                // Aggiunta della data di chiusura
                item["CHIUSURA"] = fasc.chiusura;

                // Aggiunta del numero di mesi di conserrvazione
                item["MESICONS"] = fasc.numMesiConservazione;

                // Aggiunta del numero di mesi di chiusura
                //calcolo mesi: oggi - chiusura = mesi dalla chiusura
                int numMesi = 0;
                DateTime today = DateTime.Today;
                DateTime chiusura = Convert.ToDateTime(fasc.chiusura);

                if (today.Year == chiusura.Year)
                    numMesi = today.Month - chiusura.Month;
                if (today.Year > chiusura.Year)
                {
                    int intervallo = today.Year - chiusura.Year;
                    numMesi = today.Month - chiusura.Month + (12 * intervallo);
                }
                string numMesiChiusura = numMesi.ToString();

                item["MESICHIUSURA"] = numMesiChiusura;

                // Aggiunta della riga compilata
                scarto.Rows.Add(item);
            }

            // Restituzione del data table creato
            return scarto;

        }

        private DataTable GetDataTableRicDocInCestino()
        {
            // Creazione del dataset con i dati sui documenti in cestino da esportare
            DataTable docsInCestino = new DataTable();
            DataRow docInCestino;

            // Creazione della struttura per docsInCestino
            docsInCestino.Columns.Add("COD_REG");
            docsInCestino.Columns.Add("NUM_DOCUMENTO");
            docsInCestino.Columns.Add("DATA_CREAZIONE");
            docsInCestino.Columns.Add("OGGETTO");
            docsInCestino.Columns.Add("TIPO_DOC");
            docsInCestino.Columns.Add("MITT_DEST");
            docsInCestino.Columns.Add("NOTE_CEST");

            // Aggiunta delle righe con le informazioni sui singoli documenti
            foreach (DocsPaVO.documento.InfoDocumento doc in this._objList)
            {
                // Creazione di una nuova riga
                docInCestino = docsInCestino.NewRow();

                // Aggiunta del codice registro
                docInCestino["COD_REG"] = doc.codRegistro;

                // Aggiunta del numero di documento
                docInCestino["NUM_DOCUMENTO"] = doc.docNumber;

                // Aggiunta della data di creazione
                docInCestino["DATA_CREAZIONE"] = doc.dataApertura;

                // Aggiunta dell'oggetto
                docInCestino["OGGETTO"] = doc.oggetto;

                //MODIFICATO DA FABIO PER CAMBIO ETICHETTE
                // Aggiunta del tipo di documento
                string nuovaEtichetta = getLettereProtocolli(doc.tipoProto);
                docInCestino["TIPO_DOC"] = nuovaEtichetta;

                // Aggiunta delle informazioni su mittenti/destinatari
                string str_mittDest = "";
                if (doc.mittDest.Count > 0)
                {
                    for (int g = 0; g < doc.mittDest.Count; g++)
                        str_mittDest += doc.mittDest[g] + " - ";
                    str_mittDest = str_mittDest.Substring(0, str_mittDest.Length - 3);
                }

                docInCestino["MITT_DEST"] = str_mittDest;

                // Aggiunta delle informazioni sulle note di rimozione
                docInCestino["NOTE_CEST"] = doc.noteCestino;

                // Aggiunta della riga compilata
                docsInCestino.Rows.Add(docInCestino);

            }

            // Restituzione del data table creato
            return docsInCestino;

        }

        private DataTable GetDataTableRicToDoList()
        {
            // Creazione del dataset con i dati sulla todolist da inserire nel report
            DataTable toDoList = new DataTable();
            DataRow toDoListItem;

            // Creazione della struttura per toDoList
            toDoList.Columns.Add("DATA_INVIO");
            toDoList.Columns.Add("INFO_OBJ");
            toDoList.Columns.Add("MITT");
            toDoList.Columns.Add("DEST");

            // Aggiunta delle sugli oggetti trasmessi
            foreach (DocsPaVO.trasmissione.infoToDoList trasm in this._objList)
            {
                // Creazione di una nuova riga
                toDoListItem = toDoList.NewRow();

                // Aggiunta della data di invio
                toDoListItem["DATA_INVIO"] = trasm.dataInvio;

                // Aggiunta delle informazioni sul mittente
                toDoListItem["MITT"] = trasm.utenteMittente + " (" + trasm.ruoloMittente + ")";

                // Aggiunta delle informazioni sui destinatari
                toDoListItem["DEST"] = trasm.utenteDestinatario;

                // Se le informaizoni sul documento sono accessibili...
                if (trasm.infoDoc != null)
                    // ...aggiunta del codice
                    toDoListItem["INFO_OBJ"] = "Codice: " + trasm.infoDoc +
                        "\nDescrizione: " + trasm.oggetto;

                // Aggiunta della riga compilata
                toDoList.Rows.Add(toDoListItem);

            }

            // Restituzione del data set
            return toDoList;

        }

        private DataTable GetDataTableRicDocInFasc(string param)
        {
            // Creazione del dataset con i dati sui documenti da inserire nel report
            DataTable infoDocs = new DataTable();
            DataRow infoDoc;

            // Creazione della struttura per infoDocs
            infoDocs.Columns.Add("COD_REG");
            infoDocs.Columns.Add("NUM_PROTOCOLLO");
            infoDocs.Columns.Add("DATA_PROTOCOLLO");
            infoDocs.Columns.Add("OGGETTO");
            infoDocs.Columns.Add("TIPO_DOC");
            infoDocs.Columns.Add("MITT_DEST");
            infoDocs.Columns.Add("COD_FASC");
            infoDocs.Columns.Add("DATA_ANNULLA");
            infoDocs.Columns.Add("IMG");

            // Aggiunta dlle righe con le informazioni sui documenti
            foreach (DocsPaVO.documento.InfoDocumento doc in this._objList)
            {
                // Creazione di una nuova riga nel dataset
                infoDoc = infoDocs.NewRow();

                // Aggiunta del codice registro
                infoDoc["COD_REG"] = doc.codRegistro;

                // Aggiunta del numero di protocollo
                string descDoc = string.Empty;
                int numProtocollo = new Int32();
                if (doc.numProt != null && !doc.numProt.Equals(""))
                {
                    numProtocollo = Int32.Parse(doc.numProt);
                    descDoc = numProtocollo + "\n" + doc.dataApertura;
                }
                else
                {
                    // se documento grigio
                    descDoc = doc.docNumber + "\n" + doc.dataApertura;
                }

                infoDoc["NUM_PROTOCOLLO"] = descDoc;

                // Aggiunta della data di protocollazione
                infoDoc["DATA_PROTOCOLLO"] = doc.dataApertura;

                //MODIFICATO DA FABIO PER CAMBIO ETICHETTE
                // Aggiunta del tipo di documento
                string nuovaEtichetta = getLettereProtocolli(doc.tipoProto);
                infoDoc["TIPO_DOC"] = nuovaEtichetta;

                // Aggiunta dell'oggetto
                infoDoc["OGGETTO"] = doc.oggetto;

                // Agigunta della data di annullamento
                infoDoc["DATA_ANNULLA"] = doc.dataAnnullamento;

                // Aggiunta del codice fascicolo
                infoDoc["COD_FASC"] = this._codFasc;

                // Aggiunta delle informazioni sul mittente/destinatario
                if (!Boolean.Parse(param))
                {
                    string str_mittDest = "";
                    if (doc.mittDest.Count > 0)
                    {
                        for (int g = 0; g < doc.mittDest.Count; g++)
                            str_mittDest += doc.mittDest[g] + " - ";
                        str_mittDest = str_mittDest.Substring(0, str_mittDest.Length - 3);
                    }

                    infoDoc["MITT_DEST"] = str_mittDest;

                }
                else
                    infoDoc["MITT_DEST"] = doc.mittDoc;

                // Aggiunta delle informazioni sull'acquisizione
                // if (doc.acquisitaImmagine.Equals("1"))
                //    infoDoc["IMG"] = "Si";
                // else
                //   infoDoc["IMG"] = "No";
                infoDoc["IMG"] = doc.acquisitaImmagine;
                // Aggiunta della riga compilata al dataset
                infoDocs.Rows.Add(infoDoc);
            }

            // Restituzione del dataset creato
            return infoDocs;

        }

        private DataTable GetDataTableRicTrasm()
        {
            // Creazione del table con i dati sulle trasmissioni da inserire nel report
            DataTable infoTrasms = new DataTable();
            DataRow infoTrasm;

            // Creazione della struttura per infoTrasms
            infoTrasms.Columns.Add("DATA_INVIO");
            infoTrasms.Columns.Add("INFO_DOC");
            infoTrasms.Columns.Add("MITT");
            infoTrasms.Columns.Add("DEST");

            Dictionary<string, string> destTrasmXML = new Dictionary<string, string>();
            destTrasmXML = Trasmissioni.TrasmManager.getDestinatariTrasmByListaTrasm(this._objList);

            // Aggiunta delle righe con le informazioni sui fascicoli
            foreach (DocsPaVO.trasmissione.Trasmissione trasm in this._objList)
            {
                // Creazione della nuova riga
                infoTrasm = infoTrasms.NewRow();

                // Aggiunta della data di invio
                infoTrasm["DATA_INVIO"] = trasm.dataInvio;

                // Aggiunta delle informazioni sul documento
                if (trasm.infoDocumento != null)
                    infoTrasm["INFO_DOC"] = "ID: " + trasm.infoDocumento.docNumber +
                        "\nProtocollo: " + trasm.infoDocumento.numProt +
                        "\nData: " + trasm.infoDocumento.dataApertura +
                        "\nOggetto: " + trasm.infoDocumento.oggetto;

                // Aggiunta delle informazioni sui mittenti
                infoTrasm["MITT"] = trasm.utente.descrizione + " (" + trasm.ruolo.descrizione + ")";

                // Aggiunta delle informazioni sui destinatari
                infoTrasm["DEST"] = getDestinatariTrasmLite(trasm, destTrasmXML);

                // Aggiunta della riga alla tabella
                infoTrasms.Rows.Add(infoTrasm);

            }

            // Restituisce le informazioni sui fascicoli
            return infoTrasms;

        }


        private DataTable GetDataTableRicTrasmFasc()
        {
            // Creazione del table con i dati sulle trasmissioni da inserire nel report
            DataTable infoTrasms = new DataTable();
            DataRow infoTrasm;

            // Creazione della struttura per infoTrasms
            infoTrasms.Columns.Add("DATA_INVIO");
            infoTrasms.Columns.Add("COD_FASCICOLO");
            infoTrasms.Columns.Add("DESC_FASCICOLO");
            infoTrasms.Columns.Add("DATA_APERTURA");

            // Aggiunta delle righe con le informazioni sui fascicoli
            foreach (DocsPaVO.trasmissione.Trasmissione trasm in this._objList)
            {
                // Creazione della nuova riga
                infoTrasm = infoTrasms.NewRow();

                // Aggiunta della data di invio
                infoTrasm["DATA_INVIO"] = trasm.dataInvio;

                // Aggiunta delle informazioni sul documento
                if (trasm.infoFascicolo != null)
                {
                    infoTrasm["COD_FASCICOLO"] = trasm.infoFascicolo.codice;
                    infoTrasm["DESC_FASCICOLO"] = trasm.infoFascicolo.descrizione;
                    infoTrasm["DATA_APERTURA"] = trasm.infoFascicolo.apertura;
                }

                // Aggiunta della riga alla tabella
                infoTrasms.Rows.Add(infoTrasm);

            }

            // Restituisce le informazioni sui fascicoli
            return infoTrasms;

        }

        private DataTable GetDataTableRicFasc(string codRegistro)
        {
            // Creazione del dataset con i dati sui fascicoli da inserire nel report
            DataTable infoFascs = new DataTable();
            DataRow infoFasc;

            // Creazione della struttura per infoFascs
            infoFascs.Columns.Add("COD_REG");
            infoFascs.Columns.Add("TIPO_FASC");
            infoFascs.Columns.Add("COD_FASC");
            infoFascs.Columns.Add("DESC_FASC");
            infoFascs.Columns.Add("DATA_A");
            infoFascs.Columns.Add("DATA_C");
            infoFascs.Columns.Add("COLL_FIS");

            // Aggiunta delle righe con le informazioni sui fascicoli
            foreach (DocsPaVO.Grids.SearchObject project in this._objList)
            {
                // Creazione della nuova riga
                infoFasc = infoFascs.NewRow();

                // Aggiunta del codice registro
                //infoFasc["COD_REG"] = codRegistro;
                infoFasc["COD_REG"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P7")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta del tipo fascicolo
                //infoFasc["TIPO_FASC"] = fasc.tipo;
                infoFasc["TIPO_FASC"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P1")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta del codice fascicolo
                //infoFasc["COD_FASC"] = fasc.codice;
                infoFasc["COD_FASC"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P3")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta della descrizione del fascicolo
                //infoFasc["DESC_FASC"] = fasc.descrizione;
                infoFasc["DESC_FASC"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P4")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta della data di apertura
                infoFasc["DATA_A"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P5")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta della data di chiusura
                infoFasc["DATA_C"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P6")).FirstOrDefault().SearchObjectFieldValue;


                infoFasc["COLL_FIS"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P22")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta della riga alla tabella
                infoFascs.Rows.Add(infoFasc);

            }

            // Restituisce le informazioni sui fascicoli
            return infoFascs;

        }

        private DataTable GetDataTableRicDoc()
        {
            // Creazione del dataset con i dati sui documenti da inserire nel report
            DataTable infoDocs = new DataTable();
            DataRow infoDoc;

            // Creazione della struttura per infoDocs
            infoDocs.Columns.Add("COD_REG");
            infoDocs.Columns.Add("NUM_PROTOCOLLO");
            infoDocs.Columns.Add("DATA_PROTOCOLLO");
            infoDocs.Columns.Add("OGGETTO");
            infoDocs.Columns.Add("TIPO_DOC");
            infoDocs.Columns.Add("MITT_DEST");
            infoDocs.Columns.Add("COD_FASC");
            infoDocs.Columns.Add("DATA_ANNULLA");
            infoDocs.Columns.Add("IMG");

            // Aggiunta delle righe con le informazioni sui documenti
            //foreach (DocsPaVO.documento.InfoDocumentoExport doc in this._objList)
            foreach (DocsPaVO.documento.InfoDocumento doc in this._objList)
            {
                // Creazione della nuova riga
                infoDoc = infoDocs.NewRow();

                // Aggiunta del codice registro
                //infoDoc["COD_REG"] = doc.codiceRegistro;
                infoDoc["COD_REG"] = doc.codRegistro;

                // Aggiunta del numero di protocollo
                //infoDoc["NUM_PROTOCOLLO"] = doc.idOrNumProt;
                if (!string.IsNullOrEmpty(doc.numProt))
                    infoDoc["NUM_PROTOCOLLO"] = doc.numProt;
                else
                    infoDoc["NUM_PROTOCOLLO"] = doc.docNumber;

                // Aggiunta della data di apertura
                //infoDoc["DATA_PROTOCOLLO"] = doc.data;
                infoDoc["DATA_PROTOCOLLO"] = doc.dataApertura;

                // Aggiunta dell'oggetto
                infoDoc["OGGETTO"] = doc.oggetto;

                //MODIFICATO DA FABIO PER CAMBIO ETICHETTE
                // Aggiunta del tipo di documento
                //string nuovaEtichetta = getLettereProtocolli(doc.tipologiaDocumento);
                string nuovaEtichetta = getLettereProtocolli(doc.tipoProto);
                infoDoc["TIPO_DOC"] = nuovaEtichetta;

                // Aggiunta delle informazioni su mittente / destinatario
                string str_mittDest = "";
                if (doc.mittDest.Count > 0)
                {
                    for (int g = 0; g < doc.mittDest.Count; g++)
                        str_mittDest += doc.mittDest[g] + " - ";
                    str_mittDest = str_mittDest.Substring(0, str_mittDest.Length - 3);
                }

                //infoDoc["MITT_DEST"] = doc.mittentiDestinatari;
                infoDoc["MITT_DEST"] = str_mittDest;

                // Aggiunta delle informazioni sul codice fascicolo
                //infoDoc["COD_FASC"] = doc.codiceFascicolo;
                infoDoc["COD_FASC"] = string.Empty;

                // Aggiunta delle informazioni sulla data di annullamento
                infoDoc["DATA_ANNULLA"] = doc.dataAnnullamento;

                // Aggiunta dell'informazione indicante se il documento è stato acquisito
                infoDoc["IMG"] = doc.acquisitaImmagine;

                // Aggiunta della riga alla tabella
                infoDocs.Rows.Add(infoDoc);

            }

            return infoDocs;

        }

        /// <summary>
        /// Conversione dei dati dei documenti in formato XML
        /// </summary>
        private void exportDocRicFullTextToXML(ArrayList documenti)
        {
            string descrDoc = string.Empty;
            string MittDest = "";
            int numeroProt = new Int32();
            DocsPaVO.documento.InfoDocumento doc = new DocsPaVO.documento.InfoDocumento();
            this.addAttToRootNode();

            /* per i doc usiamo questi dati
            Numero_protocollo,
            Data_di_protocollo,
            Tipologia_di_documento,
            Data_annullamento,
            Oggetto,
            Mittenti_o_destinatari,
            Codice_del_fascicolo_che_contiene_il_documento	
            Codice_registro
            Immagine
            */

            for (int i = 0; i < documenti.Count; i++)
            {
                doc = (DocsPaVO.documento.InfoDocumento)documenti[i];

                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                string data = doc.dataApertura;
                if (doc.numProt != null && !doc.numProt.Equals(""))
                {
                    numeroProt = Int32.Parse(doc.numProt);
                    descrDoc = numeroProt.ToString();
                }
                else
                {
                    descrDoc = doc.docNumber;
                }
                descrDoc = descrDoc + "\n" + data;
                XmlElement numProt = this._xmlDoc.CreateElement("NUM_PROTOCOLLO");
                numProt.InnerText = descrDoc;
                record.AppendChild(numProt);

                XmlElement dataProt = this._xmlDoc.CreateElement("DATA_PROTOCOLLO");
                dataProt.InnerText = doc.dataApertura;
                record.AppendChild(dataProt);

                XmlElement tipoDoc = this._xmlDoc.CreateElement("TIPO_DOC");
                tipoDoc.InnerText = doc.tipoProto;
                record.AppendChild(tipoDoc);

                XmlElement dataAnn = this._xmlDoc.CreateElement("DATA_ANNULLA");
                dataAnn.InnerText = doc.dataAnnullamento;
                record.AppendChild(dataAnn);

                XmlElement oggetto = this._xmlDoc.CreateElement("OGGETTO");
                oggetto.InnerText = doc.oggetto;
                record.AppendChild(oggetto);

                //il campo mittDest è un array list di possibili mitt/dest lo scorro tutto e concat in una singola string con separatore ="[spazio]-[spazio]"
                if (doc.mittDest != null && doc.mittDest.Count > 0)
                {
                    for (int g = 0; g < doc.mittDest.Count; g++)
                        MittDest += doc.mittDest[g] + " - ";
                    if (doc.mittDest.Count > 0)
                        MittDest = MittDest.Substring(0, MittDest.Length - 3);
                }
                else
                    MittDest += "";
                XmlElement mittDest = this._xmlDoc.CreateElement("MITT_DEST");
                mittDest.InnerText = MittDest;
                record.AppendChild(mittDest);

                XmlElement codFasc = this._xmlDoc.CreateElement("COD_FASC");
                //codFasc.InnerText = doc.codiceFascicolo;
                codFasc.InnerText = "";
                record.AppendChild(codFasc);

                XmlElement codReg = this._xmlDoc.CreateElement("COD_REG");
                codReg.InnerText = doc.codRegistro;
                record.AppendChild(codReg);

                XmlElement immagine = this._xmlDoc.CreateElement("IMG");
                if (Documenti.CacheFileManager.isFileInCache(doc.docNumber))
                    immagine.InnerText = "SI";
                else
                    immagine.InnerText = doc.acquisitaImmagine;
                record.AppendChild(immagine);

                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }

        /// <summary>
        /// Generazione del file (PDF) di export dei documenti di un dato fascicolo
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>		
        /// <param name="selectedDocumentsId">Id dei documenti da esportare</param>
        /// <returns></returns>
        private void exportDocInFascPDF(string idGruppo, string idPeople, DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca, bool mittDest_indirizzo, String[] selectedDocumentsId)
        {
            try
            {
                this._objList = Documenti.InfoDocManager.getQueryExportDocInFasc(idGruppo, idPeople, this._folder, filtriRicerca, mittDest_indirizzo, selectedDocumentsId);

                this._rowsList = Convert.ToString(this._objList.Count);

                // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                // iTextSharp per la stampa dei risultati della ricerca
                // ...creazione del report PDF con iTextSharp
                this._file = this.CreaPDFRisRic("DF", mittDest_indirizzo.ToString());
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        /// <summary>
        /// Conversione dei dati dei documenti in formato XML
        /// </summary>		
        private void exportDocInFascToXML(bool mittDest_indirizzo)
        {
            this.addAttToRootNode();

            /* per i doc usiamo questi dati
            Numero_protocollo o numero documento,
            Data_di_protocollo o di creazione documento,
            Registro di protocollo
            Tipologia_di_documento (arrivo, partenza, interno, grigio),
            Oggetto,
            Mittenti_o_destinatari,
            Data annullamento,
            File,
            Codice Fascicolo
            */
            foreach (DocsPaVO.documento.InfoDocumento doc in this._objList)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                string descDoc = string.Empty;
                int numProtocollo = new Int32();
                if (doc.numProt != null && !doc.numProt.Equals(""))
                {
                    numProtocollo = Int32.Parse(doc.numProt);
                    descDoc = numProtocollo + "\n" + doc.dataApertura;
                }
                else
                {
                    // se documento grigio
                    descDoc = doc.docNumber + "\n" + doc.dataApertura;
                }
                XmlElement numProt = this._xmlDoc.CreateElement("NUM_PROTOCOLLO");
                numProt.InnerText = descDoc;
                record.AppendChild(numProt);

                XmlElement dataProt = this._xmlDoc.CreateElement("DATA_PROTOCOLLO");
                dataProt.InnerText = doc.dataApertura;
                record.AppendChild(dataProt);

                XmlElement codReg = this._xmlDoc.CreateElement("COD_REG");
                codReg.InnerText = doc.codRegistro;
                record.AppendChild(codReg);

                XmlElement tipoDoc = this._xmlDoc.CreateElement("TIPO_DOC");
                //MODIFICATO DA FABIO PER CAMBIO ETICHETTE
                // Aggiunta del tipo di documento
                tipoDoc.InnerText = getLettereProtocolli(doc.tipoProto);
                record.AppendChild(tipoDoc);

                XmlElement oggetto = this._xmlDoc.CreateElement("OGGETTO");
                oggetto.InnerText = doc.oggetto;
                record.AppendChild(oggetto);

                XmlElement dataAnn = this._xmlDoc.CreateElement("DATA_ANNULLA");
                dataAnn.InnerText = doc.dataAnnullamento;
                record.AppendChild(dataAnn);

                XmlElement codFasc = this._xmlDoc.CreateElement("COD_FASC");
                codFasc.InnerText = this._codFasc;
                record.AppendChild(codFasc);

                if (!mittDest_indirizzo)
                {
                    string str_mittDest = "";
                    if (doc.mittDest.Count > 0)
                    {
                        for (int g = 0; g < doc.mittDest.Count; g++)
                            str_mittDest += doc.mittDest[g] + " - ";
                        str_mittDest = str_mittDest.Substring(0, str_mittDest.Length - 3);
                    }

                    XmlElement mittDest = this._xmlDoc.CreateElement("MITT_DEST");
                    mittDest.InnerText = str_mittDest;
                    record.AppendChild(mittDest);
                }
                else
                {
                    XmlElement mittDest = this._xmlDoc.CreateElement("MITT_DEST");
                    mittDest.InnerText = doc.mittDoc;
                    record.AppendChild(mittDest);
                }

                XmlElement immagine = this._xmlDoc.CreateElement("IMG");
                if (Documenti.CacheFileManager.isFileInCache(doc.docNumber))
                    immagine.InnerText = "Si";
                else
                    if (doc.acquisitaImmagine.Equals("1"))
                        immagine.InnerText = "Si";
                    else
                        immagine.InnerText = "No";
                record.AppendChild(immagine);

                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }

        /// <summary>
        /// Generazione del file (PDF) di export dei documenti in cestino
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="filtriRicerca"></param>
        private void exportDocInCestPDF(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca)
        {
            try
            {
                this._objList = Documenti.InfoDocManager.getQueryExportDocInCest(infoUtente, filtriRicerca);
                if (this._objList.Count > 0)
                {
                    this._rowsList = Convert.ToString(this._objList.Count);

                    // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                    // ...creazione del report PDF con iTextSharp
                    this._file = this.CreaPDFRisRic("DC", "");
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        /// <summary>
        /// Conversione dei dati dei documenti in formato XML
        /// </summary>		
        private void exportDocInCestToXML()
        {
            this.addAttToRootNode();

            /* per i doc usiamo questi dati
            Identificativo / data di creazione,
            Registro (per i documenti predisposti alla protocollazione)
            Tipologia_di_documento (arrivo, partenza, interno, grigio),
            Oggetto,
            Mittenti_o_destinatari,
            Motivo della cancellazione
            */
            foreach (DocsPaVO.documento.InfoDocumento doc in this._objList)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                string descDoc = string.Empty;
                //int numProtocollo = new Int32();
                /*if (doc.numProt != null && !doc.numProt.Equals(""))
                {
                    numProtocollo = Int32.Parse(doc.numProt);
                    descDoc = numProtocollo + "\n" + doc.dataApertura;
                }
                else
                {*/
                descDoc = doc.docNumber + "\n" + doc.dataApertura;
                //}
                XmlElement numProt = this._xmlDoc.CreateElement("NUM_DOCUMENTO");
                numProt.InnerText = doc.docNumber;
                record.AppendChild(numProt);

                XmlElement dataProt = this._xmlDoc.CreateElement("DATA_CREAZIONE");
                dataProt.InnerText = doc.dataApertura;
                record.AppendChild(dataProt);

                XmlElement codReg = this._xmlDoc.CreateElement("COD_REG");
                codReg.InnerText = doc.codRegistro;
                record.AppendChild(codReg);

                XmlElement tipoDoc = this._xmlDoc.CreateElement("TIPO_DOC");
                //MODIFICATO DA FABIO PER CAMBIO ETICHETTE
                // Aggiunta del tipo di documento
                tipoDoc.InnerText = getLettereProtocolli(doc.tipoProto);
                record.AppendChild(tipoDoc);

                XmlElement oggetto = this._xmlDoc.CreateElement("OGGETTO");
                oggetto.InnerText = doc.oggetto;
                record.AppendChild(oggetto);

                /*XmlElement dataAnn = this._xmlDoc.CreateElement("DATA_ANNULLA");
                dataAnn.InnerText = doc.dataAnnullamento;
                record.AppendChild(dataAnn);*/

                string str_mittDest = "";
                if (doc.mittDest.Count > 0)
                {
                    for (int g = 0; g < doc.mittDest.Count; g++)
                        str_mittDest += doc.mittDest[g] + " - ";
                    str_mittDest = str_mittDest.Substring(0, str_mittDest.Length - 3);
                }
                XmlElement mittDest = this._xmlDoc.CreateElement("MITT_DEST");
                mittDest.InnerText = str_mittDest;
                record.AppendChild(mittDest);

                XmlElement motivoRimozione = this._xmlDoc.CreateElement("NOTE_CEST");
                motivoRimozione.InnerText = doc.noteCestino;
                record.AppendChild(motivoRimozione);


                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }

        private string getLettereProtocolli(string etichetta)
        {
            this.etichette = BusinessLogic.Documenti.EtichetteManager.GetInstance(this.infoUser, this.infoUser.idAmministrazione);
            if (etichetta.Equals("A"))
            {
                return this.etichette[0].Descrizione;
            }
            else
            {
                if (etichetta.Equals("P"))
                {
                    return this.etichette[1].Descrizione;
                }
                else
                {
                    if (etichetta.Equals("I"))
                    {
                        return this.etichette[2].Descrizione;
                    }
                    else
                    {
                        if (etichetta.Equals("ALL"))
                        {
                            return this.etichette[4].Descrizione;
                        }
                        else
                        {
                            return this.etichette[3].Descrizione;
                        }
                    }
                }
            }
        }

        #endregion

        #region Fascicoli
        private void exportFascPDF(DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, bool enableChilds, DocsPaVO.fascicolazione.Classificazione classificazione, String[] idProjectsList)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Fascicoli objFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                this._objList = objFasc.GetListaFascicoli(userInfo,
                                                        classificazione,
                                                        this._filtri[0],
                                                        enableUfficioRef,
                                                        enableProfilazione,
                                                        enableChilds,
                                                        registro, null, "");

                if (idProjectsList != null &&
                    idProjectsList.Length > 0)
                {
                    ArrayList tmp = new ArrayList();
                    foreach (Fascicolo prj in this._objList)
                        if (idProjectsList.Where(e => e.Equals(prj.systemID)).Count() > 0)
                            tmp.Add(prj);

                    this._objList = tmp;

                }

                if (this._objList.Count > 0)
                {
                    this._rowsList = Convert.ToString(this._objList.Count);

                    // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                    // iTextSharp per la stampa dei risultati della ricerca
                    this._file = this.CreaPDFRisRic("F", registro.codRegistro);
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="codRegistro"></param>
        private void exportFascToXML(string codRegistro)
        {
            this.addAttToRootNode();

            /* per i fasc usiamo questi dati
            Tipo_fascicolo,
            Codice_fascicolo,
            Descrizione_fascicolo,
            Data_apertura,
            Data_chiusura,
            Collocazione_fisica
            */

            foreach (DocsPaVO.fascicolazione.Fascicolo fasc in this._objList)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                XmlElement codReg = this._xmlDoc.CreateElement("COD_REG");
                codReg.InnerText = codRegistro;
                record.AppendChild(codReg);

                XmlElement tipoFasc = this._xmlDoc.CreateElement("TIPO_FASC");
                tipoFasc.InnerText = fasc.tipo;
                record.AppendChild(tipoFasc);

                XmlElement codFasc = this._xmlDoc.CreateElement("COD_FASC");
                codFasc.InnerText = fasc.codice;
                record.AppendChild(codFasc);

                XmlElement descFasc = this._xmlDoc.CreateElement("DESC_FASC");
                descFasc.InnerText = fasc.descrizione;
                record.AppendChild(descFasc);

                XmlElement dataA = this._xmlDoc.CreateElement("DATA_A");
                dataA.InnerText = fasc.apertura;
                record.AppendChild(dataA);

                XmlElement dataC = this._xmlDoc.CreateElement("DATA_C");
                dataC.InnerText = fasc.chiusura;
                record.AppendChild(dataC);

                XmlElement collFis = this._xmlDoc.CreateElement("COLL_FIS");
                if (fasc.idUoLF != null && fasc.idUoLF != string.Empty)
                    collFis.InnerText = getLocazioneFisica(fasc.idUoLF);
                else
                    collFis.InnerText = "";
                record.AppendChild(collFis);

                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }

        #endregion

        #region Trasmissioni
        /// <summary>
        /// Generazione del file (PDF) di export delle trasmissioni
        /// </summary>
        /// <param name="oggettoTrasmesso"></param>
        /// <param name="tipoRicerca"></param>
        /// <param name="utente"></param>
        /// <param name="ruolo"></param>		
        private void exportTrasmPDF(DocsPaVO.trasmissione.OggettoTrasm oggettoTrasmesso, string tipoRicerca, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
        {
            try
            {
                int recordCount;
                int totalPageNumber;
                switch (tipoRicerca)
                {
                    case "R":
                        this._objList = BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryRicevuteMethodPagingLite(oggettoTrasmesso, utente, ruolo, this._filtriTrasm, 1, true, 1, out totalPageNumber, out recordCount);

                        break;

                    case ("E"):
                        // INC000000528056
                        // Export trasmissioni effettuate con filtro destinatario valorizzato
                        //this._objList = BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryEffettuateMethodPagingLite(oggettoTrasmesso, utente, ruolo, this._filtriTrasm, 1, true, 1, out totalPageNumber, out recordCount);
                        this._objList = BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryEffettuateMethodPagingLiteWithoutTrasmUtente(oggettoTrasmesso, utente, ruolo, this._filtriTrasm, 1, true, 1, out totalPageNumber, out recordCount);
                        break;
                }

                if (this._objList.Count > 0)
                {
                    this._rowsList = Convert.ToString(this._objList.Count);

                    // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                    // ...creazione del report PDF con iTextSharp
                    this._file = this.CreaPDFRisRic("T", "");
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void exportTrasmToXML(string idPeople, string idGruppo)
        {
            this.addAttToRootNode();

            /* per le trasm usiamo questi dati
            Data trasmissione,
            Mittente (utente e ruolo),
            Destinatari,
            Documento trasmesso (id, protocollo, data),
            Oggetto del documento
            */

            Dictionary<string, string> destTrasmXML = new Dictionary<string, string>();
            destTrasmXML = Trasmissioni.TrasmManager.getDestinatariTrasmByListaTrasm(this._objList);

            foreach (DocsPaVO.trasmissione.Trasmissione trasm in this._objList)
            {
                if (trasm.trasmissioniSingole != null && trasm.trasmissioniSingole.Count > 0)
                {
                    for (int a = 0; a < trasm.trasmissioniSingole.Count; a++)
                    {
                        DocsPaVO.trasmissione.TrasmissioneSingola trasmS = new DocsPaVO.trasmissione.TrasmissioneSingola();
                        trasmS = (DocsPaVO.trasmissione.TrasmissioneSingola)trasm.trasmissioniSingole[a];

                        XmlElement record = this._xmlDoc.CreateElement("RECORD");

                        XmlElement dataTrasm = this._xmlDoc.CreateElement("DATA_INVIO");
                        dataTrasm.InnerText = trasm.dataInvio;
                        record.AppendChild(dataTrasm);

                        XmlElement mittTrasm = this._xmlDoc.CreateElement("MITT");
                        mittTrasm.InnerText = trasm.utente.descrizione + " (" + trasm.ruolo.descrizione + ")";
                        record.AppendChild(mittTrasm);

                        XmlElement destTrasm = this._xmlDoc.CreateElement("DEST");
                        destTrasm.InnerText = getDestinatariTrasmSingolaLite(trasmS, destTrasmXML);
                        record.AppendChild(destTrasm);

                        if (trasm.infoDocumento != null)
                        {
                            XmlElement docIDTrasm = this._xmlDoc.CreateElement("DOC_ID");
                            docIDTrasm.InnerText = trasm.infoDocumento.docNumber;
                            record.AppendChild(docIDTrasm);

                            XmlElement docProtTrasm = this._xmlDoc.CreateElement("DOC_PROT");
                            docProtTrasm.InnerText = trasm.infoDocumento.numProt;
                            record.AppendChild(docProtTrasm);

                            XmlElement docDataTrasm = this._xmlDoc.CreateElement("DOC_DATA");
                            docDataTrasm.InnerText = trasm.infoDocumento.dataApertura;
                            record.AppendChild(docDataTrasm);

                            XmlElement oggDocTrasm = this._xmlDoc.CreateElement("DOC_OGG");
                            string msg = string.Empty;
                            int diritti = BusinessLogic.Documenti.DocManager.VerificaACL("D", trasm.infoDocumento.idProfile, idPeople, idGruppo, out msg);

                            if (diritti == 2)
                                oggDocTrasm.InnerText = trasm.infoDocumento.oggetto;

                            if (diritti == 0)
                                oggDocTrasm.InnerText = "Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower();

                            //oggDocTrasm.InnerText = trasm.infoDocumento.oggetto;
                            record.AppendChild(oggDocTrasm);
                        }
                        this._xmlDoc.DocumentElement.AppendChild(record);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docOrFasc"></param>
        /// <param name="registri"></param>
        /// <param name="objSystemId">Lista dei system id degli elementi selezionati</param>
        private void exportToDoListPDF(DocsPaVO.utente.InfoUtente infoUtente, string docOrFasc, string registri, String[] objectId)
        {
            try
            {
                this._objList = BusinessLogic.Trasmissioni.QueryTrasmManager.getToDoList(infoUtente, docOrFasc, registri, this._filtriTrasm, objectId);

                if (this._objList.Count > 0)
                {
                    this._rowsList = Convert.ToString(this._objList.Count);

                    // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                    // iTextSharp per la stampa dei risultati della ricerca
                    // ...creazione del report PDF con iTextSharp
                    if (docOrFasc.Equals("D"))
                        this._file = this.CreaPDFRisRic("T", "");
                    else
                        this._file = this.CreaPDFRisRic("TDL", "");

                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        private void exportToDoListToXML(string docOrFasc)
        {
            this.addAttToRootNode();

            /* per le trasm usiamo questi dati
            Data trasmissione,
            Mittente (utente e ruolo),
            Destinatari,
            Documento trasmesso (id, protocollo, data),
            Oggetto del documento
            */

            foreach (DocsPaVO.trasmissione.infoToDoList trasm in this._objList)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                XmlElement dataTrasm = this._xmlDoc.CreateElement("DATA_INVIO");
                dataTrasm.InnerText = trasm.dataInvio;
                record.AppendChild(dataTrasm);

                XmlElement mittTrasm = this._xmlDoc.CreateElement("MITT");
                mittTrasm.InnerText = trasm.utenteMittente + " (" + trasm.ruoloMittente + ")";
                record.AppendChild(mittTrasm);

                XmlElement destTrasm = this._xmlDoc.CreateElement("DEST");
                destTrasm.InnerText = trasm.utenteDestinatario;
                //destTrasm.InnerText = getDestinatariTrasm(trasm);
                record.AppendChild(destTrasm);


                if (trasm.infoDoc != null)
                {
                    if (docOrFasc.Equals("D"))
                    {
                        XmlElement docIDTrasm = this._xmlDoc.CreateElement("DOC_ID");
                        docIDTrasm.InnerText = trasm.infoDoc;
                        record.AppendChild(docIDTrasm);

                        XmlElement docProtTrasm = this._xmlDoc.CreateElement("DOC_PROT");
                        docProtTrasm.InnerText = trasm.numProto;
                        record.AppendChild(docProtTrasm);

                        XmlElement docDataTrasm = this._xmlDoc.CreateElement("DOC_DATA");
                        docDataTrasm.InnerText = trasm.dataDoc;
                        record.AppendChild(docDataTrasm);

                        XmlElement oggDocTrasm = this._xmlDoc.CreateElement("DOC_OGG");
                        oggDocTrasm.InnerText = trasm.oggetto;
                        record.AppendChild(oggDocTrasm);
                    }
                    else
                    {
                        XmlElement fascCodTrasm = this._xmlDoc.CreateElement("CODICE");
                        fascCodTrasm.InnerText = trasm.infoDoc;
                        record.AppendChild(fascCodTrasm);

                        XmlElement fascDescTrasm = this._xmlDoc.CreateElement("DESCRIZIONE");
                        fascDescTrasm.InnerText = trasm.oggetto;
                        record.AppendChild(fascDescTrasm);
                    }
                }

                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }
        #endregion

        #region Export Scarto
        private void exportScartoPDF(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.AreaScarto.InfoScarto infoScarto)
        {
            try
            {
                this._objList = BusinessLogic.fascicoli.AreaScartoManager.getListaFascicoliInScartoNoPaging(infoUtente, infoScarto);
                if (this._objList.Count > 0)
                {
                    this._rowsList = Convert.ToString(this._objList.Count);

                    this._rowsList = Convert.ToString(this._objList.Count);

                    // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                    // iTextSharp per la stampa dei risultati della ricerca
                    // ...creazione del report PDF con iTextSharp
                    this._file = this.CreaPDFRisRic("S", "");
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        private void exportScartoToXML()
        {
            this.addAttToRootNode();

            /* per lo scarto usiamo questi dati
            Tipo,
            Codice classificazione,
            Codice,
            Descrizione,
            Data chiusura,
            Mesi conservazione,
            Mesi dalla chiusura
            */

            foreach (DocsPaVO.fascicolazione.Fascicolo fasc in this._objList)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                XmlElement tipoFasc = this._xmlDoc.CreateElement("TIPO");
                tipoFasc.InnerText = fasc.tipo;
                record.AppendChild(tipoFasc);

                XmlElement codClassFasc = this._xmlDoc.CreateElement("CODCLASS");
                codClassFasc.InnerText = fasc.codiceGerarchia;
                record.AppendChild(codClassFasc);

                XmlElement codiceFasc = this._xmlDoc.CreateElement("CODICE");
                codiceFasc.InnerText = fasc.codice;
                record.AppendChild(codiceFasc);

                XmlElement descFasc = this._xmlDoc.CreateElement("DESCRIZIONE");
                descFasc.InnerText = fasc.descrizione;
                record.AppendChild(descFasc);


                XmlElement dataChiusuraFasc = this._xmlDoc.CreateElement("CHIUSURA");
                dataChiusuraFasc.InnerText = fasc.chiusura;
                record.AppendChild(dataChiusuraFasc);

                XmlElement mesiConservazioneFasc = this._xmlDoc.CreateElement("MESICONS");
                mesiConservazioneFasc.InnerText = fasc.numMesiConservazione;
                record.AppendChild(mesiConservazioneFasc);

                //calcolo mesi: oggi - chiusura = mesi dalla chiusura
                int numMesi = 0;
                DateTime today = DateTime.Today;
                DateTime chiusura = Convert.ToDateTime(fasc.chiusura);

                if (today.Year == chiusura.Year)
                    numMesi = today.Month - chiusura.Month;
                if (today.Year > chiusura.Year)
                {
                    int intervallo = today.Year - chiusura.Year;
                    numMesi = today.Month - chiusura.Month + (12 * intervallo);
                }
                string numMesiChiusura = numMesi.ToString();

                XmlElement mesiChiusuraFasc = this._xmlDoc.CreateElement("MESICHIUSURA");
                mesiChiusuraFasc.InnerText = numMesiChiusura;
                record.AppendChild(mesiChiusuraFasc);


                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }

        private void exportScartoXLS(DocsPaVO.utente.InfoUtente infoUtente, ArrayList campiSelezionati, DocsPaVO.AreaScarto.InfoScarto infoScarto)
        {
            try
            {
                //Recupero i dati
                ArrayList fascicoli = BusinessLogic.fascicoli.AreaScartoManager.getListaFascicoliInScartoNoPaging(infoUtente, infoScarto);



                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLScarto(fascicoli, campiSelezionati);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportFascicoli.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportFascicoli.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportFascicoli";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione fascicoli : " + ex.Message);
            }
        }

        private StringBuilder creaXMLScarto(ArrayList fascicoli, ArrayList campiSelezionati)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += sheetScarto(fascicoli, campiSelezionati);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());

            return sb;
        }

        private string sheetScarto(ArrayList fascicoli, ArrayList campiSelezionati)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"Fascicoli\">";
            strXML += "<Table>";
            strXML += creaTabellaScarto(campiSelezionati);
            strXML += datiScartoXML(fascicoli, campiSelezionati);
            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }

        private string creaTabellaScarto(ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                switch (campoSelezionato.nomeCampo)
                {
                    case "Tipo":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"25\"/>";
                        break;

                    case "Cod. Classificazione":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"90\"/>";
                        break;

                    case "Codice":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"90\"/>";
                        break;

                    case "Descrizione":
                        strXML += "<Column ss:StyleID=\"s64\" ss:Width=\"250\"/>";
                        break;

                    case "Data Chiusura":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"70\"/>";
                        break;

                    case "Mesi conservazione":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"70\"/>";
                        break;

                    case "Mesi Chiusura":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"70\"/>";
                        break;

                    default:
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"90\"/>";
                        break;
                }
            }
            return strXML;
        }

        private string datiScartoXML(ArrayList fascicoli, ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            strXML = creaColonneScarto(campiSelezionati);
            strXML += inserisciDatiScarto(fascicoli, campiSelezionati);
            return strXML;
        }

        private string creaColonneScarto(ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            strXML += "<Row>";

            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                strXML += "<Cell ss:StyleID=\"s70\">";
                strXML += "<Data ss:Type=\"String\">" + ((DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i]).nomeCampo.ToString();
                strXML += "</Data>";
                strXML += "</Cell>";
            }

            strXML += "</Row>";
            return strXML;
        }

        private string inserisciDatiScarto(ArrayList fascicoli, ArrayList campiSelezionati)
        {
            string righe = string.Empty;
            foreach (DocsPaVO.fascicolazione.Fascicolo fascicolo in fascicoli)
            {
                righe += inserisciRigaScarto(fascicolo, campiSelezionati);
            }
            return righe;
        }

        private string inserisciRigaScarto(DocsPaVO.fascicolazione.Fascicolo fascicolo, ArrayList campiSelezionati)
        {
            //calcolo mesi: oggi - chiusura = mesi dalla chiusura
            int numMesi = 0;
            DateTime today = DateTime.Today;
            DateTime chiusura = Convert.ToDateTime(fascicolo.chiusura);

            if (today.Year == chiusura.Year)
                numMesi = today.Month - chiusura.Month;
            if (today.Year > chiusura.Year)
            {
                int intervallo = today.Year - chiusura.Year;
                numMesi = today.Month - chiusura.Month + (12 * intervallo);
            }
            string numMesiChiusura = numMesi.ToString();

            string riga = string.Empty;

            riga = "<Row>";

            //Inserimento Campi Standard
            for (int i = 0; i < campiSelezionati.Count; i++)
            {

                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                switch (campoSelezionato.nomeCampo)
                {
                    case "Tipo":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\">#" + fascicolo.tipo;
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;

                    case "Codice Classificazione":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\">#" + fascicolo.codiceGerarchia;
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;

                    case "Codice":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\">#" + fascicolo.codice;
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;

                    case "Descrizione":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\"><![CDATA[#" + fascicolo.descrizione + "]]>";
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;

                    case "Data Chiusura":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\">#" + fascicolo.chiusura;
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;

                    case "Mesi conservazione":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\">#" + fascicolo.numMesiConservazione;
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;


                    case "Mesi di chiusura":
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\">#" + numMesiChiusura;
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;

                    default:
                        riga += "<Cell>";
                        riga += "<Data ss:Type=\"String\">#";
                        riga += "</Data>";
                        riga += "</Cell>";
                        break;
                }
            }

            string[] splitRiga = riga.Split('#');

            //Ricompongo la string riga 
            riga = string.Empty;
            foreach (string stringaRiga in splitRiga)
            {
                riga += stringaRiga;
            }

            riga += "</Row>";
            return riga;
        }




        #endregion

        #region Export Conservazione
        public DocsPaVO.documento.FileDocumento exportConservazione(ArrayList itemsConservazione, string exportType, string idPeople, InfoUtente infoUtente,
            string noteRifiuto)
        {
            switch (exportType)
            {
                case "PDF":
                    this.exportConservazione(itemsConservazione, idPeople, infoUtente, noteRifiuto);
                    break;
                case "XLS":

                    break;
            }

            return this._file;
        }

        private void exportConservazione(ArrayList itemsConservazione, string idPeople, InfoUtente infoUtente, string noteRifiuto)
        {
            try
            {
                this._rowsList = Convert.ToString(itemsConservazione.Count);
                this._descAmm = this.getNomeAmministrazione(idPeople);
                bool rifiuto = false;

                string nomePolicy = this.getNomePolicy(((DocsPaVO.areaConservazione.ItemsConservazione)itemsConservazione[0]).ID_Conservazione);
                //se ho una trasmissione di rifiuto istanza
                if (!string.IsNullOrEmpty(noteRifiuto))
                {
                    //Gabriele Melini 11-09-2013 modifica per inserire nel report i dettagli della policy


                    this._title = "Note rifiuto: " + noteRifiuto + " - Policy utilizzata: " + nomePolicy;
                    this._idIstanza = ((DocsPaVO.areaConservazione.ItemsConservazione)itemsConservazione[0]).ID_Conservazione + " RIFIUTATA";
                    rifiuto = true;
                }
                else
                {
                    this._idIstanza = ((DocsPaVO.areaConservazione.ItemsConservazione)itemsConservazione[0]).ID_Conservazione;
                }
                if (itemsConservazione.Count > 0)
                {
                    // Valore booleano utilizzato per verificare se è richiesta conversione con
                    // iTextSharp


                    // Se è richiesta conversione con iTextSharp...
                    //OLD CODE
                    /*
                    // ...impostazione degli oggetti da inserire nel report
                    this._objList = itemsConservazione;
                    // ...creazione del data table con i dati sulle voci da inserire nel report
                    this._file = this.CreaPDFRisRic("C", rifiuto.ToString());
                    */

                    //NEW CODE
                    //Gabriele Melini 19-09-2013
                    //genero il report con itextsharp
                    this._idIstanza = ((DocsPaVO.areaConservazione.ItemsConservazione)itemsConservazione[0]).ID_Conservazione;

                    List<FiltroRicerca> filters = new List<FiltroRicerca>() 
                    {
                        new FiltroRicerca() {argomento = "idIstanza", valore = this._idIstanza}

                    };

                    //RIFIUTO
                    if (rifiuto)
                    {

                        this._file = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(
                            new DocsPaVO.Report.PrintReportRequest()
                            {
                                UserInfo = infoUtente,
                                SearchFilters = filters,
                                ReportType = DocsPaVO.Report.ReportTypeEnum.PDF,
                                ReportKey = "NotificheRifiuto",
                                ContextName = "NotificheRifiuto",
                                SubTitle = nomePolicy,
                                AdditionalInformation = noteRifiuto
                            }
                            ).Document;
                    }
                    //CHIUSURA
                    else
                    {
                        this._file = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(
                            new DocsPaVO.Report.PrintReportRequest()
                            {
                                UserInfo = infoUtente,
                                SearchFilters = filters,
                                ReportType = DocsPaVO.Report.ReportTypeEnum.PDF,
                                ReportKey = "NotificheChiusura",
                                ContextName = "NotificheChiusura",
                                SubTitle = nomePolicy,
                                AdditionalInformation = string.Empty
                            }
                            ).Document;

                    }

                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        /// <summary>
        /// Funzione per recuperare il nome della policy utilizzata nell'istanza
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        private string getNomePolicy(string idIstanza)
        {

            try
            {
                PolicyConservazione policy = new PolicyConservazione();
                string nomePolicy = policy.GetPolicyValidazioneByIdIstanzaConservazione(idIstanza).nome;

                return nomePolicy;

            }
            catch (Exception exc)
            {

                return string.Empty;
            }

        }


        //MEV Cons. 1.3. Aggiunti i campi per la verifica Dimensione,Firma,Marcatura,Formato
        private void exportConservazioneToXML(ArrayList itemsConservazione, bool rifiuto)
        {

            this.addAttToRootNodeConservazione();

            foreach (DocsPaVO.areaConservazione.ItemsConservazione itemCons in itemsConservazione)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                XmlElement tipoDoc = this._xmlDoc.CreateElement("TIPO_DOC");
                //Richiesta del 05/12/2012, nell report sostiuire G con NP
                if (itemCons.TipoDoc.Equals("G"))
                    tipoDoc.InnerText = "NP";
                else
                    tipoDoc.InnerText = itemCons.TipoDoc;
                record.AppendChild(tipoDoc);

                XmlElement oggetto = this._xmlDoc.CreateElement("OGGETTO");
                oggetto.InnerText = itemCons.desc_oggetto;
                record.AppendChild(oggetto);

                XmlElement codiceFasc = this._xmlDoc.CreateElement("CODICE_FASC");
                codiceFasc.InnerText = itemCons.CodFasc;
                record.AppendChild(codiceFasc);

                XmlElement dataInserimento = this._xmlDoc.CreateElement("DATA_INSERIMENTO");
                dataInserimento.InnerText = itemCons.Data_Ins;
                record.AppendChild(dataInserimento);

                XmlElement idSegnaturaData = this._xmlDoc.CreateElement("ID_SEGNATURA_DATA");
                string segnatura = itemCons.numProt_or_id;
                string data = itemCons.data_prot_or_create;
                string data_doc = segnatura + "\n" + data;
                idSegnaturaData.InnerText = data_doc;
                record.AppendChild(idSegnaturaData);

                XmlElement sizeByte = this._xmlDoc.CreateElement("SIZE_BYTE");
                sizeByte.InnerText = itemCons.SizeItem;
                record.AppendChild(sizeByte);

                XmlElement esito = this._xmlDoc.CreateElement("ESITO");

                //se sto rifiutando l'istanza senza neppure aver provato a eseguire la lavorazione
                if (itemCons.esitoLavorazione == string.Empty)
                {
                    esito.InnerText = "-";
                }
                if (itemCons.esitoLavorazione == "1" && rifiuto)
                {
                    esito.InnerText = "-";
                }
                if (itemCons.esitoLavorazione == "0")
                {
                    esito.InnerText = "File non trovato";
                }
                if (itemCons.esitoLavorazione == "1" && !rifiuto)
                {
                    esito.InnerText = "SI";
                }
                if (itemCons.esitoLavorazione == "2")
                {
                    esito.InnerText = "Documento non trovato";
                }
                record.AppendChild(esito);

                //Check Firma
                XmlElement checkFirma = this._xmlDoc.CreateElement("CHECK_FIRMA");
                //if (itemCons.Check_Firma.Equals("1")) checkFirma.InnerText = "Valido";  
                //    else if (itemCons.Check_Firma.Equals("0")) checkFirma.InnerText = "Non Valido"; 
                //        else checkFirma.InnerText = string.Empty;
                if (itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.Valida)
                    || itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_Valida))
                    checkFirma.InnerText = "Valido";
                else if (itemCons.Check_Firma.Equals("0") ||
                        itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FirmaNonValida) ||
                        itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_FirmaNonValida))
                    checkFirma.InnerText = "Non Valido";
                else checkFirma.InnerText = string.Empty;
                record.AppendChild(checkFirma);

                //Check Marcatura
                XmlElement checkMarcatura = this._xmlDoc.CreateElement("CHECK_MARCATURA");
                //if (itemCons.Check_Marcatura.Equals("1")) checkMarcatura.InnerText = "Valido"; 
                //    else if (itemCons.Check_Marcatura.Equals("0")) checkMarcatura.InnerText = "Non Valido"; 
                //        else checkMarcatura.InnerText = string.Empty;
                if (itemCons.Check_Marcatura.Equals("1") &&
                    !(itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.MarcaNonValida)
                    || itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_MarcaNonValida)
                    )
                   ) checkMarcatura.InnerText = "Valido";
                else if (itemCons.Check_Marcatura.Equals("0") ||
                            itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.MarcaNonValida) ||
                            itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_MarcaNonValida)
                    ) checkMarcatura.InnerText = "Non Valido";
                else checkMarcatura.InnerText = string.Empty;
                record.AppendChild(checkMarcatura);

                //Check Formati
                XmlElement checkFormati = this._xmlDoc.CreateElement("CHECK_FORMATO");

                int validazFirma = 0;
                validazFirma = getItemConservazioneValidazioneFirma(itemCons.SystemID);
                if (validazFirma >= 4)
                {
                    itemCons.Check_Formato = "0";
                }

                //if (itemCons.Check_Formato.Equals("1")) checkFormati.InnerText = "Valido"; 
                //    else if (itemCons.Check_Formato.Equals("0")) checkFormati.InnerText = "Non Valido";
                //        else checkFormati.InnerText = string.Empty;
                if (itemCons.Check_Formato.Equals("1") &&
                    //(itemCons.Check_Marcatura.Equals("1") || string.IsNullOrEmpty(itemCons.Check_Marcatura)) &&
                    //(itemCons.Check_Firma.Equals("1") || string.IsNullOrEmpty(itemCons.Check_Firma)) &&
                            !(itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_MarcaNonValida)) &&
                            !(itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_FirmaNonValida)) &&
                            !(itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_Valida)) &&
                            !(itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido)) &&
                            !itemCons.invalidFileFormat
                            ) checkFormati.InnerText = "Valido";
                else if (itemCons.Check_Formato.Equals("0") ||
                            (itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_MarcaNonValida)) ||
                            (itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_FirmaNonValida)) ||
                            (itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_Valida)) ||
                            (itemCons.esitoValidazioneFirma.Equals(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido)) ||
                            itemCons.invalidFileFormat
                        ) checkFormati.InnerText = "Non Valido";
                else checkFormati.InnerText = string.Empty;
                record.AppendChild(checkFormati);

                //Check Policy
                XmlElement checkPolicy = this._xmlDoc.CreateElement("CHECK_POLICY");
                checkPolicy.InnerText = FormatDatiVerificaPolicy(itemCons.Check_Mask_Policy);
                record.AppendChild(checkPolicy);

                this._xmlDoc.DocumentElement.AppendChild(record);

                // Log Applicativo
                if (itemCons != null && itemCons.esitoValidazioneFirma != null)
                    logger.DebugFormat("LOG items.validazioneFirma: " + itemCons.esitoValidazioneFirma.ToString() + ";");
            }
        }

        /// <summary>
        /// Metodo per il prelievo del valore del campo VALIDAZIONE_FIRMA.
        /// </summary>
        /// <param name="idConservazione">Id dell'item di conservazione</param>
        /// <returns>Il valore del campo VALIDAZIONE_FIRMA</returns>
        public static int getItemConservazioneValidazioneFirma(string idItemConservazione)
        {
            int validazioneFirma = 0;
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            string fields_itemsCons = "VALIDAZIONE_FIRMA ";
            queryDef1.setParam("param1", fields_itemsCons);
            fields_itemsCons = "FROM DPA_ITEMS_CONSERVAZIONE ";
            queryDef1.setParam("param2", fields_itemsCons);
            fields_itemsCons = "WHERE SYSTEM_ID=" + idItemConservazione + "";
            queryDef1.setParam("param3", fields_itemsCons);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            string validazFirmaS = reader.GetValue(reader.GetOrdinal("VALIDAZIONE_FIRMA")).ToString();
                            Int32.TryParse(validazFirmaS, out validazioneFirma);
                        }
                    }
                }

            }

            return validazioneFirma;
        }

        /// <summary>
        /// Formatta i dati relativi alla verifica della policy
        /// </summary>
        /// <param name="maskPolicyDoc">stringa mask della policy</param>
        /// <returns></returns>
        public string FormatDatiVerificaPolicy(string maskPolicyDoc)
        {
            string result_validazione_policy = string.Empty;
            if (!string.IsNullOrEmpty(maskPolicyDoc))
            {
                // decodifica il mask di validità della policy
                DocsPaVO.areaConservazione.ItemPolicyValidator policyValidator = new DocsPaVO.areaConservazione.ItemPolicyValidator();
                policyValidator = DocsPaVO.areaConservazione.ItemPolicyValidator.getItemPolicyValidator(maskPolicyDoc);

                // recupera la validità di ogni filtro
                // Filtro TIPOLOGIA DOCUMENTO
                if (policyValidator.TipologiaDocumento != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Tipologia del Documento: {0}", policyValidator.TipologiaDocumento == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro STATO DOCUMENTO
                if (policyValidator.StatoDocumento != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Stato del Documento: {0}", policyValidator.StatoDocumento == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro AOO CREATORE
                if (policyValidator.AooCreator != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - AOO Creatore: {0}", policyValidator.AooCreator == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro RF Creatore
                if (policyValidator.Rf_Creator != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - RF Creatore: {0}", policyValidator.Rf_Creator == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro UO creatore
                if (policyValidator.Uo_Creator != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Uo Creatore: {0}", policyValidator.Uo_Creator == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro Includi anche sottoposti
                if (policyValidator.Uo_Creator != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Uo Creatore: {0}", policyValidator.Uo_Creator == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro Titoloario
                if (policyValidator.Titolario != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Titolario: {0}", policyValidator.Titolario == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro Codice Classificazioni
                if (policyValidator.Classificazione != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Classificazione: {0}", policyValidator.Classificazione == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro tipo documento: arrivo
                if (policyValidator.DocArrivo != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Tipo Doc. Arrivo: {0}", policyValidator.DocArrivo == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro tipo documento: partenza
                if (policyValidator.DocPartenza != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Tipo Doc. Partenza: {0}", policyValidator.DocPartenza == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro tipo documento: Interno
                if (policyValidator.DocInterno != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Tipo Doc. Interno: {0}", policyValidator.DocInterno == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro tipo documento: NP
                if (policyValidator.DocNP != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Tipo Doc. NP: {0}", policyValidator.DocNP == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro includi solo i documenti digitali
                if (policyValidator.DocDigitale != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Doc. Digitale: {0}", policyValidator.DocDigitale == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro DOCUMENTI FIRMATI: Includi solo documenti firmati
                if (policyValidator.DocFirmato != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Doc. Firmato: {0}", policyValidator.DocFirmato == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro DATA CREAZIONE + DATA DA | DATA A
                if (policyValidator.DocDataCreazione != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Data Creazione: {0}", policyValidator.DocDataCreazione == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro DATA DI PROTOCOLLAZIONE
                if (policyValidator.DocDataProtocollazione != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Data Protocollazione: {0}", policyValidator.DocDataProtocollazione == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                // Filtro FORMATI DOCUMENTI 
                if (policyValidator.DocFormato != DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Unsetting)
                    result_validazione_policy += string.Format(" - Formato: {0}", policyValidator.DocFormato == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.Valid ? "Valido" : "Non Valido");
                return (!string.IsNullOrEmpty(result_validazione_policy)) ? result_validazione_policy.Substring(3) : string.Empty;
            }
            else return string.Empty;
        }

        /// <summary>
        /// Imposta il nodo root del documento XML per la conversione in PDF
        /// </summary>		
        private void addAttToRootNodeConservazione()
        {
            XmlNode rootNode = this._xmlDoc.AppendChild(this._xmlDoc.CreateElement("EXPORT"));

            XmlAttribute attrRoot = this._xmlDoc.CreateAttribute("admin");
            attrRoot.InnerText = this._descAmm;
            rootNode.Attributes.Append(attrRoot);

            attrRoot = this._xmlDoc.CreateAttribute("date");
            attrRoot.InnerText = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
            rootNode.Attributes.Append(attrRoot);

            attrRoot = this._xmlDoc.CreateAttribute("rows");
            attrRoot.InnerText = this._rowsList;
            rootNode.Attributes.Append(attrRoot);

            attrRoot = this._xmlDoc.CreateAttribute("idIstanza");
            attrRoot.InnerText = this._idIstanza;
            rootNode.Attributes.Append(attrRoot);

            attrRoot = this._xmlDoc.CreateAttribute("title");
            attrRoot.InnerText = this._title;
            rootNode.Attributes.Append(attrRoot);


        }


        #region export rigenerazione istanza

        public DocsPaVO.documento.FileDocumento reportRigenerazioneIstanza(ArrayList itemsConservazione, string exportType, string idPeople, InfoUtente infoUtente,
            string idSupporto)
        {
            switch (exportType)
            {
                case "PDF":
                    this.reportRigenerazioneIstanza(itemsConservazione, idPeople, infoUtente, idSupporto);
                    break;
                case "XLS":

                    break;
            }

            return this._file;
        }


        /// <summary>
        /// MEV CS 1.5
        /// Metodo per la generazione del report di rigenerazione di un'istanza
        /// </summary>
        /// <param name="itemsConservazione"></param>
        /// <param name="idPeople"></param>
        /// <param name="infoUtente"></param>
        /// <param name="note"></param>
        private void reportRigenerazioneIstanza(ArrayList itemsConservazione, string idPeople, InfoUtente infoUtente, string idSupporto)
        {
            try
            {
                this._rowsList = Convert.ToString(itemsConservazione.Count);
                this._descAmm = this.getNomeAmministrazione(idPeople);

                this._title = "Si richiede la rigenerazione dell'istanza perché risulta danneggiato il supporto numero " + idSupporto;
                this._idIstanza = ((DocsPaVO.areaConservazione.ItemsConservazione)itemsConservazione[0]).ID_Conservazione;

                if (itemsConservazione.Count > 0)
                {
                    // Valore booleano utilizzato per verificare se è richiesta conversione con
                    // iTextSharp

                    this._idIstanza = ((DocsPaVO.areaConservazione.ItemsConservazione)itemsConservazione[0]).ID_Conservazione;

                    List<FiltroRicerca> filters = new List<FiltroRicerca>() 
                    {
                        new FiltroRicerca() {argomento = "idIstanza", valore = this._idIstanza},
                        new FiltroRicerca() {argomento = "idSupporto", valore = idSupporto}
                    };


                    this._file = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(
                        new DocsPaVO.Report.PrintReportRequest()
                        {
                            UserInfo = infoUtente,
                            SearchFilters = filters,
                            ReportType = DocsPaVO.Report.ReportTypeEnum.PDF,
                            ReportKey = "RigenerazioneIstanza",
                            ContextName = "RigenerazioneIstanza",
                            SubTitle = string.Format("Si richiede la rigenerazione dell'istanza numero {0}.", this._idIstanza),
                            AdditionalInformation = ""
                        }
                        ).Document;
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        private void exportRigeneraIstanzaToXML(ArrayList itemsConservazione)
        {

            this.addAttToRootNodeConservazione();
            foreach (DocsPaVO.areaConservazione.ItemsConservazione itemCons in itemsConservazione)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                XmlElement tipoDoc = this._xmlDoc.CreateElement("TIPO_DOC");
                //Richiesta del 05/12/2012, nell report sostiuire G con NP
                if (itemCons.TipoDoc.Equals("G"))
                    tipoDoc.InnerText = "NP";
                else
                    tipoDoc.InnerText = itemCons.TipoDoc;
                record.AppendChild(tipoDoc);

                XmlElement oggetto = this._xmlDoc.CreateElement("OGGETTO");
                oggetto.InnerText = itemCons.desc_oggetto;
                record.AppendChild(oggetto);

                XmlElement codiceFasc = this._xmlDoc.CreateElement("CODICE_FASC");
                codiceFasc.InnerText = itemCons.CodFasc;
                record.AppendChild(codiceFasc);

                XmlElement dataInserimento = this._xmlDoc.CreateElement("DATA_INSERIMENTO");
                dataInserimento.InnerText = itemCons.Data_Ins;
                record.AppendChild(dataInserimento);

                XmlElement idSegnaturaData = this._xmlDoc.CreateElement("ID_SEGNATURA_DATA");
                string segnatura = itemCons.numProt_or_id;
                string data = itemCons.data_prot_or_create;
                string data_doc = segnatura + "\n" + data;
                idSegnaturaData.InnerText = data_doc;
                record.AppendChild(idSegnaturaData);

                XmlElement sizeByte = this._xmlDoc.CreateElement("SIZE_BYTE");
                sizeByte.InnerText = itemCons.SizeItem;
                record.AppendChild(sizeByte);

                XmlElement esito = this._xmlDoc.CreateElement("ESITO");  //??? da verificare il senso
                esito.InnerText = "-";
                record.AppendChild(esito);

                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }
        #endregion rigenerazione istanza



        #endregion Export Conservazione

        #endregion Esportazione in PDF

        #region Utility
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

        private string topXMLODS()
        {
            string strXML = string.Empty;

            strXML = "<office:document-meta xmlns:office=\"urn:oasis:names:tc:opendocument:xmlns:office:1.0\"";
            strXML += "xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\"";
            strXML += "xmlns:meta=\"urn:oasis:names:tc:opendocument:xmlns:meta:1.0\"";
            strXML += "xmlns:ooo=\"http://openoffice.org/2004/office\"";
            strXML += "xmlns:grddl=\"http://www.w3.org/2003/g/data-view#\" office:version=\"1.2\"";
            strXML += "grddl:transformation=\"http://docs.oasis-open.org/office/1.2/xslt/odf2rdf.xsl\">";
            strXML += "<office:meta><meta:generator>OpenOffice.org/3.3$Win32 OpenOffice.org_project/330m20$Build-9567</meta:generator>";
            strXML += "<dc:date>2011-07-14T16:54:50.87</dc:date>";
            strXML += "<meta:document-statistic meta:table-count=\"1\" meta:cell-count=\"7\" meta:object-count=\"0\"/>";
            strXML += "</office:meta></office:document-meta>";
            return strXML;
        }

        private string stiliXMLModel()
        {
            XmlTextReader textReader = new XmlTextReader(_pathModelloExc);
            string strXML = string.Empty;
            strXML = "<Styles>";
            textReader.ReadToFollowing("Styles");
            //copio gli stili presenti nel modello
            while (textReader.Read() && !textReader.Name.Equals("Styles"))
            {
                strXML += copyNode(textReader);
                //if (textReader.Name.Equals("Style"))
                //    strXML += copyNode(textReader);
                //else
                //    strXML += copyStyleNode(textReader);
            }

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

            strXML += "</Styles>";

            textReader.Close();
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

        /// <summary>
        /// Genera il file per lo stream
        /// </summary>
        private void createExportFile()
        {
            FileStream stream = new FileStream(this._temporaryXSLFilePath, FileMode.Open, FileAccess.Read);
            if (stream != null)
            {
                byte[] contentExcel = new byte[stream.Length];
                stream.Read(contentExcel, 0, contentExcel.Length);
                stream.Flush();
                stream.Close();
                stream = null;

                this._file.content = contentExcel;
                this._file.length = contentExcel.Length;
                this._file.estensioneFile = "xls";
                this._file.contentType = "application/vnd.ms-excel";
                //this._file.contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }

            File.Delete(this._temporaryXSLFilePath);
        }

        /// <summary>
        /// Salva e chiude
        /// </summary>
        /// <param name="tipoRicerca">possibili valori: doc, fasc, trasm</param>
        /// <param name="theHTM">file</param>
        private void saveAndClose(string tipoRicerca, StringBuilder sb)
        {
            //cambiato per gestire lettere accentate
            string toAppend = System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + "_" + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString();
            this._temporaryXSLFilePath = HttpContext.Current.Server.MapPath(@"xml/export_" + tipoRicerca + "_" + toAppend + ".xls");
            StreamWriter writer = null;

            writer = new StreamWriter(this._temporaryXSLFilePath, true);
            writer.AutoFlush = true;
            writer.WriteLine(sb.ToString());
            writer.Flush();
            writer.Close();


            /*System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            Byte[] buffer = encoding.GetBytes(theHTM);

            string toAppend = System.DateTime.Now.Year.ToString()+System.DateTime.Now.Month.ToString()+System.DateTime.Now.Day.ToString()+"_"+System.DateTime.Now.Hour.ToString()+System.DateTime.Now.Minute.ToString()+System.DateTime.Now.Second.ToString();
            this._temporaryXSLFilePath = HttpContext.Current.Server.MapPath(@"xml/export_"+tipoRicerca+"_"+toAppend+".xls");

            FileStream file = File.Create(this._temporaryXSLFilePath);
            file.Write(buffer,0,theHTM.Length);
            file.Flush();
            file.Close();*/
        }

        /// <summary>
        /// Imposta il nodo root del documento XML per la conversione in PDF
        /// </summary>		
        private void addAttToRootNode()
        {
            XmlNode rootNode = this._xmlDoc.AppendChild(this._xmlDoc.CreateElement("EXPORT"));

            XmlAttribute attrRoot = this._xmlDoc.CreateAttribute("admin");
            attrRoot.InnerText = this._descAmm;
            rootNode.Attributes.Append(attrRoot);

            attrRoot = this._xmlDoc.CreateAttribute("title");
            attrRoot.InnerText = this._title;
            rootNode.Attributes.Append(attrRoot);

            attrRoot = this._xmlDoc.CreateAttribute("date");
            attrRoot.InnerText = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
            rootNode.Attributes.Append(attrRoot);

            attrRoot = this._xmlDoc.CreateAttribute("rows");
            attrRoot.InnerText = this._rowsList;
            rootNode.Attributes.Append(attrRoot);

            attrRoot = this._xmlDoc.CreateAttribute("idIstanza");
            attrRoot.InnerText = this._idIstanza;
            rootNode.Attributes.Append(attrRoot);
        }

        /// <summary>
        /// Reperisce il nome dell'amministrazione
        /// </summary>
        /// <param name="idPeople">utente di appartenenza</param>
        /// <returns>descrizione nome amm.ne</returns>
        private string getNomeAmministrazione(string idPeople)
        {
            string nome = string.Empty;
            DocsPaDB.Query_DocsPAWS.Report rep = new DocsPaDB.Query_DocsPAWS.Report();
            nome = rep.getNomeAmmFromPeople(idPeople);
            return nome;
        }

        /// <summary>
        /// Reperisce la collocazione fisica di un fascicolo
        /// </summary>
        /// <param name="id">ID del fascicolo</param>
        /// <returns>descrizione collocazione fisica</returns>
        private string getLocazioneFisica(string id)
        {
            string descLocFis = string.Empty;
            DocsPaDB.Query_DocsPAWS.Fascicoli obj = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            descLocFis = obj.GetDescLF(id);
            return descLocFis;
        }

        /// <summary>
        /// Reperisce i destinatari di una trasmissione
        /// </summary>
        /// <param name="trasm">oggetto trasmissione</param>
        /// <returns>lista dei destinatari concatenati tramite virgola</returns>
        private string getDestinatariTrasm(DocsPaVO.trasmissione.Trasmissione trasm)
        {
            string destinatari = string.Empty;

            if (trasm.trasmissioniSingole != null)
            {
                foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in trasm.trasmissioniSingole)
                {
                    destinatari = Trasmissioni.TrasmManager.getDestinatariTrasmissioniUtenteDaSingola(ts.systemId);




                }
            }

            return destinatari;
        }

        /// <summary>
        /// Reperisce le informazione del documento nell'export delle trasmissioni
        /// </summary>
        /// <param name="trasm">oggetto trasmissione</param>
        /// <returns>info documento</returns>
        private static string getInfoDocTrasmXSL(DocsPaVO.trasmissione.Trasmissione trasm, string idPeople, string idGruppo)
        {
            string infoDoc = string.Empty;

            if (trasm.infoDocumento != null)
            {
                infoDoc = "ID: " + trasm.infoDocumento.docNumber + "<br>";
                infoDoc += "Protocollo: " + trasm.infoDocumento.numProt + "<br>";
                infoDoc += "Data: " + trasm.infoDocumento.dataApertura + "<br>";
                string msg = string.Empty;
                int diritti = BusinessLogic.Documenti.DocManager.VerificaACL("D", trasm.infoDocumento.idProfile, idPeople, idGruppo, out msg);

                if (diritti == 0)
                    infoDoc += "Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "<br>";
                else
                    infoDoc += "Oggetto: " + trasm.infoDocumento.oggetto + "<br>";

                //infoDoc += "Oggetto: " + trasm.infoDocumento.oggetto + "<br>";
                infoDoc += "Mittente: " + trasm.infoDocumento.mittDoc;
                if (trasm.infoDocumento.Destinatari != null && trasm.infoDocumento.Destinatari.Count > 0)
                {
                    int i = 0;
                    infoDoc += "<br>Destinatari: ";
                    foreach (string dest in trasm.infoDocumento.Destinatari)
                    {
                        infoDoc += dest;
                    }
                }
            }

            return infoDoc;
        }

        private static string getInfoDocToDoListXSL(string docOrFasc, DocsPaVO.trasmissione.infoToDoList trasm, string idPeople, string idGruppo)
        {
            string infoDoc = string.Empty;
            string msg = string.Empty;
            if (trasm.infoDoc != null)
            {
                if (string.IsNullOrEmpty(docOrFasc))
                {
                    if (!string.IsNullOrEmpty(trasm.sysIdDoc))
                    {
                        infoDoc = "ID: " + trasm.infoDoc + "<br>";
                        infoDoc += "Protocollo: " + trasm.numProto + "<br>";
                        infoDoc += "Data: " + trasm.dataDoc + "<br>";
                        int diritti = BusinessLogic.Documenti.DocManager.VerificaACL("D", trasm.sysIdDoc, idPeople, idGruppo, out msg);

                        if (diritti == 0)
                            infoDoc += "Non si possiedono i diritti per la visualizzazione delle informazioni sul documento";
                        else
                            infoDoc += "Oggetto: " + trasm.oggetto;
                    }

                    if (!string.IsNullOrEmpty(trasm.sysIdFasc))
                    {
                        infoDoc += "Codice: " + trasm.infoDoc + "<br>";
                        int diritti = BusinessLogic.Documenti.DocManager.VerificaACL("F", trasm.sysIdFasc, idPeople, idGruppo, out msg);

                        if (diritti == 0)
                            infoDoc += "Non si possiedono i diritti per la visualizzazione delle informazioni sul fascicolo";
                        else
                            infoDoc += "Descrizione: " + trasm.oggetto;
                    }
                }
                else
                {
                    if (docOrFasc.Equals("D"))
                    {
                        infoDoc = "ID: " + trasm.infoDoc + "<br>";
                        infoDoc += "Protocollo: " + trasm.numProto + "<br>";
                        infoDoc += "Data: " + trasm.dataDoc + "<br>";
                        int diritti = BusinessLogic.Documenti.DocManager.VerificaACL("D", trasm.sysIdDoc, idPeople, idGruppo, out msg);

                        if (diritti == 0)
                            infoDoc += "Non si possiedono i diritti per la visualizzazione delle informazioni sul documento";
                        else
                            infoDoc += "Oggetto: " + trasm.oggetto;
                    }
                    else
                    {
                        infoDoc += "Codice: " + trasm.infoDoc + "<br>";
                        int diritti = BusinessLogic.Documenti.DocManager.VerificaACL("F", trasm.sysIdFasc, idPeople, idGruppo, out msg);

                        if (diritti == 0)
                            infoDoc += "Non si possiedono i diritti per la visualizzazione delle informazioni sul fascicolo";
                        else
                            infoDoc += "Descrizione: " + trasm.oggetto;
                    }
                }
            }
            return infoDoc;
        }

        /// <summary>
        /// creazione del file XLS in formato HTM per i documenti
        /// </summary>
        /// <returns></returns>
        private string createXLS_HTM_DOC()
        {
            string theHTM = string.Empty;

            theHTM = this.commonTopXLS();

            #region OLD
            //theHTM += ".xl26";
            //theHTM += "	{mso-style-parent:style0;";
            //theHTM += "	text-align:center;";
            //theHTM += "	vertical-align:middle;}";
            //theHTM += ".xl27";
            //theHTM += "	{mso-style-parent:style0;";
            //theHTM += "	font-size:9.0pt;";
            //theHTM += "	font-weight:700;";
            //theHTM += "	font-family:Arial, sans-serif;";
            //theHTM += "	mso-font-charset:0;";
            //theHTM += "	text-align:center;";
            //theHTM += "	vertical-align:top;";
            //theHTM += "	border:1.0pt solid windowtext;}";
            //theHTM += ".xl28";
            //theHTM += "	{mso-style-parent:style0;";
            //theHTM += "	vertical-align:top;}";
            //theHTM += ".xl29";
            //theHTM += "	{mso-style-parent:style0;";
            //theHTM += "	font-size:9.0pt;";
            //theHTM += "	text-align:center;";
            //theHTM += "	vertical-align:middle;";
            //theHTM += "	border:.5pt solid windowtext;";
            //theHTM += "	white-space:normal;}";
            //theHTM += ".xl30";
            //theHTM += "	{mso-style-parent:style0;";
            //theHTM += "	font-size:9.0pt;";
            //theHTM += "	mso-number-format:\"Short Date\";";
            //theHTM += "	text-align:center;";
            //theHTM += "	vertical-align:middle;";
            //theHTM += "	border:.5pt solid windowtext;";
            //theHTM += "	white-space:normal;}";
            //theHTM += ".xl31";
            //theHTM += "	{mso-style-parent:style0;";
            //theHTM += "	font-size:9.0pt;";
            //theHTM += "	vertical-align:top;";
            //theHTM += "	border:.5pt solid windowtext;";
            //theHTM += "	white-space:normal;}";
            //theHTM += ".xl32";
            //theHTM += "	{mso-style-parent:style0;";
            //theHTM += "	font-size:9.0pt;";
            //theHTM += "	white-space:normal;}";
            //theHTM += "-->";
            //theHTM += "</style>";
            //theHTM += "<!--[if gte mso 9]><xml>";
            //theHTM += "<x:ExcelWorkbook>";
            //theHTM += "<x:ExcelWorksheets>";
            //theHTM += "<x:ExcelWorksheet>";
            //theHTM += "	<x:Name>Sheet1</x:Name>";
            //theHTM += "	<x:WorksheetOptions>";
            //theHTM += "	<x:Print>";
            //theHTM += "	<x:ValidPrinterInfo/>";
            //theHTM += "	<x:HorizontalResolution>600</x:HorizontalResolution>";
            //theHTM += "	<x:VerticalResolution>600</x:VerticalResolution>";
            //theHTM += "	</x:Print>";
            //theHTM += "	<x:Selected/>";
            //theHTM += "	<x:Panes>";
            //theHTM += "	<x:Pane>";
            //theHTM += "	<x:Number>3</x:Number>";
            //theHTM += "	<x:RangeSelection>$A$1:$H$1</x:RangeSelection>";
            //theHTM += "	</x:Pane>";
            //theHTM += "	</x:Panes>";
            //theHTM += "	<x:ProtectContents>False</x:ProtectContents>";
            //theHTM += "	<x:ProtectObjects>False</x:ProtectObjects>";
            //theHTM += "	<x:ProtectScenarios>False</x:ProtectScenarios>";
            //theHTM += "	</x:WorksheetOptions>";
            //theHTM += "</x:ExcelWorksheet>";
            //theHTM += "</x:ExcelWorksheets>";
            //theHTM += "<x:WindowHeight>10230</x:WindowHeight>";
            //theHTM += "<x:WindowWidth>15195</x:WindowWidth>";
            //theHTM += "<x:WindowTopX>0</x:WindowTopX>";
            //theHTM += "<x:WindowTopY>30</x:WindowTopY>";
            //theHTM += "<x:ProtectStructure>False</x:ProtectStructure>";
            //theHTM += "<x:ProtectWindows>False</x:ProtectWindows>";
            //theHTM += "</x:ExcelWorkbook>";
            //theHTM += "</xml><![endif]-->";
            //theHTM += "</head>";
            //theHTM += "<body link=blue vlink=purple>";
            //theHTM += "<table x:str border=0 cellpadding=0 cellspacing=0 width=957 style='border-collapse:collapse;table-layout:fixed;width:718pt'>";
            //theHTM += "<col class=xl26 width=75 span=2 style='mso-width-source:userset;mso-width-alt:2742;width:56pt'>";
            //theHTM += "<col class=xl26 width=68 style='mso-width-source:userset;mso-width-alt:2486;width:51pt'>";
            //theHTM += "<col class=xl28 width=320 style='mso-width-source:userset;mso-width-alt:11702;width:240pt'>";
            //theHTM += "<col class=xl26 width=26 style='mso-width-source:userset;mso-width-alt:950;width:20pt'>";
            //theHTM += "<col class=xl28 width=180 style='mso-width-source:userset;mso-width-alt:6582;width:135pt'>";
            //theHTM += "<col class=xl28 width=145 style='mso-width-source:userset;mso-width-alt:5302;width:109pt'>";
            //theHTM += "<col class=xl26 width=68 style='mso-width-source:userset;mso-width-alt:2486;width:51pt'>";
            //theHTM += "<tr height=53 style='mso-height-source:userset;height:39.95pt'>";			
            //theHTM += "<td colspan=8 height=53 class=xl32 width=957 style='height:39.95pt;width:718pt'>@TITOLO@</td>";
            //theHTM += "</tr>";
            //theHTM += "<tr class=xl24 height=18 style='height:13.5pt'>";
            //theHTM += "<td height=18 class=xl25 style='height:13.5pt'>Registro</td>";
            //theHTM += "<td class=xl25 style='border-left:none'>Protocollo</td>";
            //theHTM += "<td class=xl25 style='border-left:none'>Data</td>";
            //theHTM += "<td class=xl27 style='border-left:none'>Oggetto</td>";
            //theHTM += "<td class=xl25 style='border-left:none'>Tipo</td>";
            //theHTM += "<td class=xl27 style='border-left:none'>Mitt / Dest</td>";
            //theHTM += "<td class=xl27 style='border-left:none'>Cod. fascicoli</td>";
            //theHTM += "<td class=xl25 style='border-left:none'>Annullato</td>";
            //theHTM += "</tr>";
            //theHTM += "@RIGHE@";
            //theHTM += "<![if supportMisalignedColumns]>";
            //theHTM += "<tr height=0 style='display:none'>";
            //theHTM += "<td width=75 style='width:56pt'></td>";
            //theHTM += "<td width=75 style='width:56pt'></td>";
            //theHTM += "<td width=68 style='width:51pt'></td>";
            //theHTM += "<td width=320 style='width:240pt'></td>";
            //theHTM += "<td width=26 style='width:20pt'></td>";
            //theHTM += "<td width=180 style='width:135pt'></td>";
            //theHTM += "<td width=145 style='width:109pt'></td>";
            //theHTM += "<td width=68 style='width:51pt'></td>";
            //theHTM += "</tr>";
            //theHTM += "<![endif]>";
            //theHTM += "</table>";
            //theHTM += "</body>";
            //theHTM += "</html>";
            #endregion

            theHTM += ".xl24";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "vertical-align:middle;}";
            theHTM += ".xl25";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;}";
            theHTM += ".xl26";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-weight:700;";
            theHTM += "font-family:Arial, sans-serif;";
            theHTM += "mso-font-charset:0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "background:silver;";
            theHTM += "mso-pattern:auto none;}";
            theHTM += ".xl27";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "font-weight:700;";
            theHTM += "font-family:Arial, sans-serif;";
            theHTM += "mso-font-charset:0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "background:silver;";
            theHTM += "mso-pattern:auto none;}";
            theHTM += ".xl28";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "font-weight:700;";
            theHTM += "font-family:Arial, sans-serif;";
            theHTM += "mso-font-charset:0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:.5pt solid windowtext;";
            theHTM += "border-right:.5pt solid windowtext;;";
            theHTM += "border-bottom:.5pt solid windowtext;;";
            theHTM += "border-left:.5pt solid windowtext;;";
            theHTM += "background:silver;";
            theHTM += "mso-pattern:auto none;}";
            theHTM += ".xl29";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:.5pt solid windowtext;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl30";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl31";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "mso-number-format:\"Short Date\";";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl32";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += "-->";
            theHTM += "</style>";
            theHTM += "<!--[if gte mso 9]><xml>";
            theHTM += "<x:ExcelWorkbook>";
            theHTM += "<x:ExcelWorksheets>";
            theHTM += "<x:ExcelWorksheet>";
            theHTM += "<x:Name>Sheet1</x:Name>";
            theHTM += "<x:WorksheetOptions>";
            theHTM += "<x:Print>";
            theHTM += "<x:ValidPrinterInfo/>";
            theHTM += "<x:HorizontalResolution>600</x:HorizontalResolution>";
            theHTM += "<x:VerticalResolution>600</x:VerticalResolution>";
            theHTM += "</x:Print>";
            theHTM += "<x:Selected/>";
            theHTM += "<x:Panes/>";
            theHTM += "<x:ProtectContents>False</x:ProtectContents>";
            theHTM += "<x:ProtectObjects>False</x:ProtectObjects>";
            theHTM += "<x:ProtectScenarios>False</x:ProtectScenarios>";
            theHTM += "</x:WorksheetOptions>";
            theHTM += "</x:ExcelWorksheet>";
            theHTM += "</x:ExcelWorksheets>";
            theHTM += "<x:WindowHeight>10230</x:WindowHeight>";
            theHTM += "<x:WindowWidth>15195</x:WindowWidth>";
            theHTM += "<x:WindowTopX>0</x:WindowTopX>";
            theHTM += "<x:WindowTopY>30</x:WindowTopY>";
            theHTM += "<x:ProtectStructure>False</x:ProtectStructure>";
            theHTM += "<x:ProtectWindows>False</x:ProtectWindows>";
            theHTM += "</x:ExcelWorkbook>";
            theHTM += "<x:ExcelName>";
            theHTM += "<x:Name>Print_Titles</x:Name>";
            theHTM += "<x:SheetIndex>1</x:SheetIndex>";
            theHTM += "<x:Formula>=Sheet1!$1:$1</x:Formula>";
            theHTM += "</x:ExcelName>";
            theHTM += "</xml><![endif]-->";
            theHTM += "</head>";

            theHTM += "<body link=blue vlink=purple class=xl24>";
            theHTM += "<table x:str border=0 cellpadding=0 cellspacing=0 width=957 style='border-collapse:collapse;table-layout:fixed;width:718pt'>";
            theHTM += "<col class=xl25 width=75 span=2 style='mso-width-source:userset;mso-width-alt:2742;width:56pt'>";
            theHTM += "<col class=xl25 width=68 style='mso-width-source:userset;mso-width-alt:2486;width:51pt'>";
            theHTM += "<col class=xl24 width=320 style='mso-width-source:userset;mso-width-alt:11702;width:240pt'>";
            theHTM += "<col class=xl25 width=26 style='mso-width-source:userset;mso-width-alt:950;width:20pt'>";
            theHTM += "<col class=xl24 width=180 style='mso-width-source:userset;mso-width-alt:6582;width:135pt'>";
            theHTM += "<col class=xl24 width=145 style='mso-width-source:userset;mso-width-alt:5302;width:109pt'>";
            theHTM += "<col class=xl25 width=68 style='mso-width-source:userset;mso-width-alt:2486;width:51pt'>";
            theHTM += "<tr class=xl26 height=17 style='height:12.75pt'>";
            theHTM += "<td height=17 class=xl27 width=70 style='height:12.75pt;width:52pt'>Registro</td>";
            theHTM += "<td class=xl28 width=80 style='width:60pt'>Prot. / Id Doc.</td>";
            theHTM += "<td class=xl28 width=68 style='width:51pt'>Data</td>";
            theHTM += "<td class=xl28 width=294 style='width:220pt'>Oggetto</td>";
            theHTM += "<td class=xl28 width=26 style='width:20pt'>Tipo</td>";
            theHTM += "<td class=xl28 width=180 style='width:135pt'>Mitt / Dest</td>";
            theHTM += "<td class=xl28 width=145 style='width:109pt'>Cod. fascicoli</td>";
            theHTM += "<td class=xl28 width=68 style='width:51pt'>Annullato</td>";
            theHTM += "<td class=xl28 width=26 style='width:20pt'>File</td>";
            theHTM += "</tr>";
            theHTM += "@RIGHE@";
            theHTM += "<![if supportMisalignedColumns]>";
            theHTM += "<tr height=0 style='display:none'>";
            theHTM += "<td width=70 style='width:52pt'></td>";
            theHTM += "<td width=80 style='width:60pt'></td>";
            theHTM += "<td width=68 style='width:51pt'></td>";
            theHTM += "<td width=294 style='width:220pt'></td>";
            theHTM += "<td width=26 style='width:20pt'></td>";
            theHTM += "<td width=180 style='width:135pt'></td>";
            theHTM += "<td width=145 style='width:109pt'></td>";
            theHTM += "<td width=68 style='width:51pt'></td>";
            theHTM += "<td width=26 style='width:20pt'></td>";
            theHTM += "</tr>";
            theHTM += "<![endif]>";
            theHTM += "</table>";
            theHTM += "</body>";
            theHTM += "</html>";

            return theHTM;
        }

        /// <summary>
        /// creazione del file XLS in formato HTM per i documenti in cestino
        /// </summary>
        /// <returns></returns>
        private string createXLS_HTM_DOC_IN_CEST()
        {
            string theHTM = string.Empty;

            theHTM = this.commonTopXLS();

            theHTM += ".xl24";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "vertical-align:middle;}";
            theHTM += ".xl25";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;}";
            theHTM += ".xl26";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-weight:700;";
            theHTM += "font-family:Arial, sans-serif;";
            theHTM += "mso-font-charset:0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "background:silver;";
            theHTM += "mso-pattern:auto none;}";
            theHTM += ".xl27";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "font-weight:700;";
            theHTM += "font-family:Arial, sans-serif;";
            theHTM += "mso-font-charset:0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "background:silver;";
            theHTM += "mso-pattern:auto none;}";
            theHTM += ".xl28";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "font-weight:700;";
            theHTM += "font-family:Arial, sans-serif;";
            theHTM += "mso-font-charset:0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:.5pt solid windowtext;";
            theHTM += "border-right:.5pt solid windowtext;;";
            theHTM += "border-bottom:.5pt solid windowtext;;";
            theHTM += "border-left:.5pt solid windowtext;;";
            theHTM += "background:silver;";
            theHTM += "mso-pattern:auto none;}";
            theHTM += ".xl29";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:.5pt solid windowtext;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl30";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl31";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "mso-number-format:\"Short Date\";";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl32";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += "-->";
            theHTM += "</style>";
            theHTM += "<!--[if gte mso 9]><xml>";
            theHTM += "<x:ExcelWorkbook>";
            theHTM += "<x:ExcelWorksheets>";
            theHTM += "<x:ExcelWorksheet>";
            theHTM += "<x:Name>Sheet1</x:Name>";
            theHTM += "<x:WorksheetOptions>";
            theHTM += "<x:Print>";
            theHTM += "<x:ValidPrinterInfo/>";
            theHTM += "<x:HorizontalResolution>600</x:HorizontalResolution>";
            theHTM += "<x:VerticalResolution>600</x:VerticalResolution>";
            theHTM += "</x:Print>";
            theHTM += "<x:Selected/>";
            theHTM += "<x:Panes/>";
            theHTM += "<x:ProtectContents>False</x:ProtectContents>";
            theHTM += "<x:ProtectObjects>False</x:ProtectObjects>";
            theHTM += "<x:ProtectScenarios>False</x:ProtectScenarios>";
            theHTM += "</x:WorksheetOptions>";
            theHTM += "</x:ExcelWorksheet>";
            theHTM += "</x:ExcelWorksheets>";
            theHTM += "<x:WindowHeight>10230</x:WindowHeight>";
            theHTM += "<x:WindowWidth>15195</x:WindowWidth>";
            theHTM += "<x:WindowTopX>0</x:WindowTopX>";
            theHTM += "<x:WindowTopY>30</x:WindowTopY>";
            theHTM += "<x:ProtectStructure>False</x:ProtectStructure>";
            theHTM += "<x:ProtectWindows>False</x:ProtectWindows>";
            theHTM += "</x:ExcelWorkbook>";
            theHTM += "<x:ExcelName>";
            theHTM += "<x:Name>Print_Titles</x:Name>";
            theHTM += "<x:SheetIndex>1</x:SheetIndex>";
            theHTM += "<x:Formula>=Sheet1!$1:$1</x:Formula>";
            theHTM += "</x:ExcelName>";
            theHTM += "</xml><![endif]-->";
            theHTM += "</head>";

            theHTM += "<body link=blue vlink=purple class=xl24>";
            theHTM += "<table x:str border=0 cellpadding=0 cellspacing=0 width=957 style='border-collapse:collapse;table-layout:fixed;width:718pt'>";
            theHTM += "<col class=xl25 width=75 span=2 style='mso-width-source:userset;mso-width-alt:2742;width:56pt'>";
            theHTM += "<col class=xl25 width=68 style='mso-width-source:userset;mso-width-alt:2486;width:51pt'>";
            theHTM += "<col class=xl24 width=320 style='mso-width-source:userset;mso-width-alt:11702;width:240pt'>";
            theHTM += "<col class=xl25 width=26 style='mso-width-source:userset;mso-width-alt:950;width:20pt'>";
            theHTM += "<col class=xl24 width=180 style='mso-width-source:userset;mso-width-alt:6582;width:135pt'>";
            theHTM += "<tr class=xl26 height=17 style='height:12.75pt'>";
            theHTM += "<td height=17 class=xl27 width=75 style='height:12.75pt;width:56pt'>Registro</td>";
            theHTM += "<td class=xl28 width=75 style='width:56pt'>Num. doc.</td>";
            theHTM += "<td class=xl28 width=68 style='width:51pt'>Data</td>";
            theHTM += "<td class=xl28 width=294 style='width:220pt'>Oggetto</td>";
            theHTM += "<td class=xl28 width=26 style='width:20pt'>Tipo</td>";
            theHTM += "<td class=xl28 width=180 style='width:135pt'>Mitt / Dest</td>";
            theHTM += "<td class=xl28 width=145 style='width:109pt'>Note</td>";
            theHTM += "</tr>";
            theHTM += "@RIGHE@";
            theHTM += "<![if supportMisalignedColumns]>";
            theHTM += "<tr height=0 style='display:none'>";
            theHTM += "<td width=75 style='width:56pt'></td>";
            theHTM += "<td width=75 style='width:56pt'></td>";
            theHTM += "<td width=68 style='width:51pt'></td>";
            theHTM += "<td width=294 style='width:220pt'></td>";
            theHTM += "<td width=26 style='width:20pt'></td>";
            theHTM += "<td width=180 style='width:135pt'></td>";
            theHTM += "<td width=145 style='width:109pt'></td>";
            theHTM += "</tr>";
            theHTM += "<![endif]>";
            theHTM += "</table>";
            theHTM += "</body>";
            theHTM += "</html>";

            return theHTM;
        }

        /// <summary>
        ///  creazione del file XLS in formato HTM per i fascicoli
        /// </summary>
        /// <returns></returns>
        private string createXLS_HTM_FASC()
        {
            string theHTM = string.Empty;

            theHTM = this.commonTopXLS();

            theHTM += ".xl24";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;}";
            theHTM += ".xl25";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "vertical-align:middle;}";
            theHTM += ".xl26";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl27";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:.5pt solid windowtext;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl28";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:.5pt solid windowtext;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl29";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:.5pt solid windowtext;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl30";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl31";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl32";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "font-weight:700;";
            theHTM += "font-family:Arial, sans-serif;";
            theHTM += "mso-font-charset:0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "background:silver;";
            theHTM += "mso-pattern:auto none;}";
            theHTM += ".xl33";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:.5pt solid windowtext;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl34";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-weight:700;";
            theHTM += "font-family:Arial, sans-serif;";
            theHTM += "mso-font-charset:0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "background:silver;";
            theHTM += "mso-pattern:auto none;}";
            theHTM += "-->";
            theHTM += "</style>";
            theHTM += "<!--[if gte mso 9]><xml>";
            theHTM += "<x:ExcelWorkbook>";
            theHTM += "<x:ExcelWorksheets>";
            theHTM += "<x:ExcelWorksheet>";
            theHTM += "<x:Name>Sheet1</x:Name>";
            theHTM += "<x:WorksheetOptions>";
            theHTM += "<x:Print>";
            theHTM += "<x:ValidPrinterInfo/>";
            theHTM += "<x:HorizontalResolution>600</x:HorizontalResolution>";
            theHTM += "<x:VerticalResolution>600</x:VerticalResolution>";
            theHTM += "</x:Print>";
            theHTM += "<x:Selected/>";
            theHTM += "<x:Panes/>";
            theHTM += "<x:ProtectContents>False</x:ProtectContents>";
            theHTM += "<x:ProtectObjects>False</x:ProtectObjects>";
            theHTM += "<x:ProtectScenarios>False</x:ProtectScenarios>";
            theHTM += "</x:WorksheetOptions>";
            theHTM += "</x:ExcelWorksheet>";
            theHTM += "</x:ExcelWorksheets>";
            theHTM += "<x:WindowHeight>10230</x:WindowHeight>";
            theHTM += "<x:WindowWidth>15195</x:WindowWidth>";
            theHTM += "<x:WindowTopX>0</x:WindowTopX>";
            theHTM += "<x:WindowTopY>30</x:WindowTopY>";
            theHTM += "<x:ProtectStructure>False</x:ProtectStructure>";
            theHTM += "<x:ProtectWindows>False</x:ProtectWindows>";
            theHTM += "</x:ExcelWorkbook>";
            theHTM += "<x:ExcelName>";
            theHTM += "<x:Name>Print_Titles</x:Name>";
            theHTM += "<x:SheetIndex>1</x:SheetIndex>";
            theHTM += "<x:Formula>=Sheet1!$1:$1</x:Formula>";
            theHTM += "</x:ExcelName>";
            theHTM += "</xml><![endif]-->";
            theHTM += "</head>";
            theHTM += "<body link=blue vlink=purple class=xl25>";
            theHTM += "<table x:str border=0 cellpadding=0 cellspacing=0 width=938 style='border-collapse:collapse;table-layout:fixed;width:704pt'>";
            theHTM += "<col class=xl24 width=82 style='mso-width-source:userset;mso-width-alt:2998;width:62pt'>";
            theHTM += "<col class=xl24 width=33 style='mso-width-source:userset;mso-width-alt:1206;width:25pt'>";
            theHTM += "<col class=xl24 width=117 style='mso-width-source:userset;mso-width-alt:4278;width:88pt'>";
            theHTM += "<col class=xl25 width=355 style='mso-width-source:userset;mso-width-alt:12982;width:266pt'>";
            theHTM += "<col class=xl24 width=68 span=2 style='mso-width-source:userset;mso-width-alt:2486;width:51pt'>";
            theHTM += "<col class=xl25 width=215 style='mso-width-source:userset;mso-width-alt:7862;width:161pt'>";
            theHTM += "<tr class=xl34 height=17 style='height:12.75pt'>";
            theHTM += "<td height=17 class=xl32 width=82 style='height:12.75pt;width:62pt'>Registro</td>";
            theHTM += "<td class=xl32 width=33 style='border-left:none;width:25pt'>Tipo</td>";
            theHTM += "<td class=xl32 width=117 style='border-left:none;width:88pt'>Codice</td>";
            theHTM += "<td class=xl32 width=355 style='border-left:none;width:266pt'>Descrizione</td>";
            theHTM += "<td class=xl32 width=68 style='border-left:none;width:51pt'>Apertura</td>";
            theHTM += "<td class=xl32 width=68 style='border-left:none;width:51pt'>Chiusura</td>";
            theHTM += "<td class=xl32 width=215 style='border-left:none;width:161pt'>Collocazione fisica</td>";
            theHTM += "</tr>";
            theHTM += "@RIGHE@";
            theHTM += "<![if supportMisalignedColumns]>";
            theHTM += "<tr height=0 style='display:none'>";
            theHTM += "<td width=82 style='width:62pt'></td>";
            theHTM += "<td width=33 style='width:25pt'></td>";
            theHTM += "<td width=117 style='width:88pt'></td>";
            theHTM += "<td width=355 style='width:266pt'></td>";
            theHTM += "<td width=68 style='width:51pt'></td>";
            theHTM += "<td width=68 style='width:51pt'></td>";
            theHTM += "<td width=215 style='width:161pt'></td>";
            theHTM += "</tr>";
            theHTM += "<![endif]>";
            theHTM += "</table>";
            theHTM += "</body>";
            theHTM += "</html>";

            return theHTM;
        }

        /// <summary>
        ///  creazione del file XLS in formato HTM per le trasmissioni
        /// </summary>
        /// <returns></returns>
        private string createXLS_HTM_TRASM()
        {
            string theHTM = string.Empty;

            theHTM = this.commonTopXLS();

            theHTM += ".xl24";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;}";
            theHTM += ".xl25";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl26";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:.5pt solid windowtext;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl27";
            theHTM += " {mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "font-weight:700;";
            theHTM += "font-family:Arial, sans-serif;";
            theHTM += "mso-font-charset:0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "background:silver;";
            theHTM += "mso-pattern:auto none;}";
            theHTM += ".xl28";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-weight:700;";
            theHTM += "font-family:Arial, sans-serif;";
            theHTM += "mso-font-charset:0;";
            theHTM += "text-align:center;";
            theHTM += "vertical-align:middle;";
            theHTM += "border:.5pt solid windowtext;";
            theHTM += "background:silver;";
            theHTM += "mso-pattern:auto none;}";
            theHTM += ".xl29";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:none;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += ".xl30";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "vertical-align:middle;}";
            theHTM += ".xl31";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "font-size:9.0pt;";
            theHTM += "vertical-align:middle;";
            theHTM += "border-top:.5pt solid windowtext;";
            theHTM += "border-right:.5pt solid windowtext;";
            theHTM += "border-bottom:.5pt solid windowtext;";
            theHTM += "border-left:none;";
            theHTM += "white-space:normal;}";
            theHTM += "-->";
            theHTM += "</style>";
            theHTM += "<!--[if gte mso 9]><xml>";
            theHTM += "<x:ExcelWorkbook>";
            theHTM += "<x:ExcelWorksheets>";
            theHTM += "<x:ExcelWorksheet>";
            theHTM += "<x:Name>Sheet1</x:Name>";
            theHTM += "<x:WorksheetOptions>";
            theHTM += "<x:Print>";
            theHTM += "<x:ValidPrinterInfo/>";
            theHTM += "<x:HorizontalResolution>600</x:HorizontalResolution>";
            theHTM += "<x:VerticalResolution>600</x:VerticalResolution>";
            theHTM += "</x:Print>";
            theHTM += "<x:Selected/>";
            theHTM += "<x:Panes/>";
            theHTM += "<x:ProtectContents>False</x:ProtectContents>";
            theHTM += "<x:ProtectObjects>False</x:ProtectObjects>";
            theHTM += "<x:ProtectScenarios>False</x:ProtectScenarios>";
            theHTM += "</x:WorksheetOptions>";
            theHTM += "</x:ExcelWorksheet>";
            theHTM += "</x:ExcelWorksheets>";
            theHTM += "<x:WindowHeight>10230</x:WindowHeight>";
            theHTM += "<x:WindowWidth>15195</x:WindowWidth>";
            theHTM += "<x:WindowTopX>0</x:WindowTopX>";
            theHTM += "<x:WindowTopY>30</x:WindowTopY>";
            theHTM += "<x:ProtectStructure>False</x:ProtectStructure>";
            theHTM += "<x:ProtectWindows>False</x:ProtectWindows>";
            theHTM += "</x:ExcelWorkbook>";
            theHTM += "<x:ExcelName>";
            theHTM += "<x:Name>Print_Titles</x:Name>";
            theHTM += "<x:SheetIndex>1</x:SheetIndex>";
            theHTM += "<x:Formula>=Sheet1!$1:$1</x:Formula>";
            theHTM += "</x:ExcelName>";
            theHTM += "</xml><![endif]-->";
            theHTM += "</head>";
            theHTM += "<body link=blue vlink=purple class=xl30>";
            theHTM += "<table x:str border=0 cellpadding=0 cellspacing=0 width=972 style='border-collapse:collapse;table-layout:fixed;width:729pt'>";
            theHTM += "<col class=xl24 width=82 style='mso-width-source:userset;mso-width-alt:2998;width:62pt'>";
            theHTM += "<col class=xl30 width=460 style='mso-width-source:userset;mso-width-alt:16822;width:345pt'>";
            theHTM += "<col class=xl30 width=215 span=2 style='mso-width-source:userset;mso-width-alt:7862;width:161pt'>";
            theHTM += "<tr class=xl28 height=17 style='height:12.75pt'>";
            theHTM += "<td height=17 class=xl27 width=82 style='height:12.75pt;width:62pt'>Data trasm.</td>";
            theHTM += "<td class=xl27 width=460 style='border-left:none;width:345pt'>@TIPO@</td>";
            theHTM += "<td class=xl27 width=215 style='border-left:none;width:161pt'>Mittenti</td>";
            theHTM += "<td class=xl27 width=215 style='border-left:none;width:161pt'>Destinatari</td>";
            theHTM += "</tr>";
            theHTM += "@RIGHE@";
            theHTM += "<![if supportMisalignedColumns]>";
            theHTM += "<tr height=0 style='display:none'>";
            theHTM += "<td width=82 style='width:62pt'></td>";
            theHTM += "<td width=460 style='width:345pt'></td>";
            theHTM += "<td width=215 style='width:161pt'></td>";
            theHTM += "<td width=215 style='width:161pt'></td>";
            theHTM += "</tr>";
            theHTM += "<![endif]>";
            theHTM += "</table>";
            theHTM += "</body>";
            theHTM += "</html>";

            return theHTM;
        }

        /// <summary>
        /// Parte in comune per la creazione del file XLS in formato HTM
        /// </summary>
        /// <returns></returns>
        private string commonTopXLS()
        {
            string theHTM = string.Empty;

            theHTM = "<html xmlns:o=\"urn:schemas-microsoft-com:office:office\"";
            theHTM += "xmlns:x=\"urn:schemas-microsoft-com:office:excel\"";
            theHTM += "xmlns=\"http://www.w3.org/TR/REC-html40\">";
            theHTM += "<head>";
            theHTM += "<meta http-equiv=Content-Type content=\"text/html; charset=utf-8\">";
            theHTM += "<meta name=ProgId content=Excel.Sheet>";
            theHTM += "<meta name=Generator content=\"Microsoft Excel 11\">";
            theHTM += "<link rel=File-List href=\"docspa_files/filelist.xml\">";
            theHTM += "<link rel=Edit-Time-Data href=\"docspa_files/editdata.mso\">";
            theHTM += "<link rel=OLE-Object-Data href=\"docspa_files/oledata.mso\">";
            theHTM += "<!--[if gte mso 9]><xml>";
            theHTM += "<o:DocumentProperties>";
            theHTM += "<o:Author>Docspa</o:Author>";
            theHTM += "<o:LastAuthor></o:LastAuthor>";
            theHTM += "<o:Created></o:Created>";
            theHTM += "<o:LastSaved></o:LastSaved>";
            theHTM += "<o:Company></o:Company>";
            theHTM += "<o:Version></o:Version>";
            theHTM += "</o:DocumentProperties>";
            theHTM += "</xml><![endif]-->";
            theHTM += "<style>";
            theHTM += "<!--table";
            theHTM += "{mso-displayed-decimal-separator:\"\\,\";";
            theHTM += "mso-displayed-thousand-separator:\"\\.\";}";
            theHTM += "@page";
            theHTM += "{mso-header-data:\"&L@TITOLO@\";";//                     <<<  TITOLO
            theHTM += "mso-footer-data:\"&R&P\\/&N\";";
            theHTM += "margin:.98in .08in .59in .08in;";
            theHTM += "mso-header-margin:.39in;";
            theHTM += "mso-footer-margin:.39in;";
            theHTM += "mso-page-orientation:landscape;}";
            theHTM += "tr";
            theHTM += "{mso-height-source:auto;}";
            theHTM += "col";
            theHTM += "{mso-width-source:auto;}";
            theHTM += "br";
            theHTM += "{mso-data-placement:same-cell;}";
            theHTM += ".style0";
            theHTM += "{mso-number-format:General;";
            theHTM += "text-align:general;";
            theHTM += "vertical-align:bottom;";
            theHTM += "white-space:nowrap;";
            theHTM += "mso-rotate:0;";
            theHTM += "mso-background-source:auto;";
            theHTM += "mso-pattern:auto;";
            theHTM += "color:windowtext;";
            theHTM += "font-size:10.0pt;";
            theHTM += "font-weight:400;";
            theHTM += "font-style:normal;";
            theHTM += "text-decoration:none;";
            theHTM += "font-family:Arial;";
            theHTM += "mso-generic-font-family:auto;";
            theHTM += "mso-font-charset:0;";
            theHTM += "border:none;";
            theHTM += "mso-protection:locked visible;";
            theHTM += "mso-style-name:Normal;";
            theHTM += "mso-style-id:0;}";
            theHTM += "td";
            theHTM += "{mso-style-parent:style0;";
            theHTM += "padding-top:1px;";
            theHTM += "padding-right:1px;";
            theHTM += "padding-left:1px;";
            theHTM += "mso-ignore:padding;";
            theHTM += "color:windowtext;";
            theHTM += "font-size:10.0pt;";
            theHTM += "font-weight:400;";
            theHTM += "font-style:normal;";
            theHTM += "text-decoration:none;";
            theHTM += "font-family:Arial;";
            theHTM += "mso-generic-font-family:auto;";
            theHTM += "mso-font-charset:0;";
            theHTM += "mso-number-format:General;";
            theHTM += "text-align:general;";
            theHTM += "vertical-align:bottom;";
            theHTM += "border:none;";
            theHTM += "mso-background-source:auto;";
            theHTM += "mso-pattern:auto;";
            theHTM += "mso-protection:locked visible;";
            theHTM += "white-space:nowrap;";
            theHTM += "mso-rotate:0;}";

            return theHTM;
        }

        public string getValoreOggettoCustom(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom)
        {
            string riga = string.Empty;
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "Corrispondente":
                    DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(oggettoCustom.VALORE_DATABASE);
                    if (corr != null && corr.descrizione != null)
                        riga += corr.descrizione;
                    break;

                case "Contatore":
                    string contatore = string.Empty;
                    if (oggettoCustom.FORMATO_CONTATORE != "")
                    {
                        contatore = oggettoCustom.FORMATO_CONTATORE;
                        if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                        {
                            contatore = contatore.Replace("ANNO", oggettoCustom.ANNO);
                            contatore = contatore.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);
                            if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "0")
                            {
                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                if (reg != null)
                                {
                                    contatore = contatore.Replace("RF", reg.codRegistro);
                                    contatore = contatore.Replace("AOO", reg.codRegistro);
                                }
                            }
                        }
                        else
                        {
                            contatore = string.Empty;
                        }
                    }
                    else
                    {
                        contatore = oggettoCustom.VALORE_DATABASE;
                    }

                    riga += contatore;
                    break;

                case "CasellaDiSelezione":
                    string selezione = string.Empty;
                    foreach (string sel in oggettoCustom.VALORI_SELEZIONATI)
                    {
                        if (sel != null && sel != "")
                            selezione += sel + " - ";
                    }
                    if (selezione != null && selezione != string.Empty)
                        riga += selezione.Substring(0, selezione.Length - 2);
                    break;
                //controllare bene
                case "ContatoreSottocontatore":
                    string s_contatore = string.Empty;
                    if (oggettoCustom.FORMATO_CONTATORE != "")
                    {
                        s_contatore = oggettoCustom.FORMATO_CONTATORE;
                        if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                        {
                            s_contatore = s_contatore.Replace("ANNO", oggettoCustom.ANNO);
                            s_contatore = s_contatore.Replace("CONTATORE", (oggettoCustom.VALORE_DATABASE + "-" + oggettoCustom.VALORE_SOTTOCONTATORE));
                            if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "0")
                            {
                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                if (reg != null)
                                {
                                    s_contatore = s_contatore.Replace("RF", reg.codRegistro);
                                    s_contatore = s_contatore.Replace("AOO", reg.codRegistro);
                                }
                            }
                        }
                        else
                        {
                            s_contatore = string.Empty;
                        }
                    }
                    else
                    {
                        s_contatore = oggettoCustom.VALORE_DATABASE;
                    }

                    riga += s_contatore;
                    break;


                default:
                    riga += oggettoCustom.VALORE_DATABASE;
                    break;
            }
            return riga;
        }

        public bool isAllCampiComuni(ArrayList campiSelezionati)
        {
            foreach (DocsPaVO.ExportData.CampoSelezionato campoSelezionato in campiSelezionati)
            {
                if (campoSelezionato.campoStandard != "1" && campoSelezionato.campoComune != "1")
                    return false;
            }

            return true;
        }
        #endregion Utility

        #region REPORT AVANZATI
        private string _generatedFile = string.Empty;
        public DocsPaVO.documento.FileDocumento ExportReportAvanzati(DocsPaVO.ExportData.ExportExcelClass objReport)
        {
            StringBuilder sb = new StringBuilder();
            DocsPaDB.Query_DocsPAWS.Report qr = new DocsPaDB.Query_DocsPAWS.Report();

            // reperimento dati
            this._objList = qr.GetQueryReportAvanzatiXLS(objReport);

            if (this._objList.Count > 0)
            {
                // gestione del protocollo interno
                this.checkProtoIntEnabled(objReport.filtro.idAmministrazione);

                // generazione del file Excel
                this.generaFileReportAvanzati(objReport);

                sb.Append(this._generatedFile);
                this.saveAndClose("exportAV", sb);

                this.createExportFile();
            }
            return this._file;
        }
        /// <summary>
        /// Richiama tutte le funzioni per poter generare il file XLS in formato XML
        /// </summary>
        /// <param name="objReport">oggetto ExportExcelClass</param>
        private void generaFileReportAvanzati(DocsPaVO.ExportData.ExportExcelClass objReport)
        {
            this.topXLS_RepAv();
            this.styleXLS_RepAv();
            this.apreTabellaXLS_RepAv(objReport.filtro.tipologiaReport);
            this.intestazioneColonneXLS_RepAv(objReport.filtro.tipologiaReport);
            this.aggiungeRigheXLS_RepAv(objReport);
            this.chiudeTabellaXLS_RepAv();
        }

        /// <summary>
        /// Genera la parte comune di TOP del file XML
        /// </summary>
        private void topXLS_RepAv()
        {
            this._generatedFile = topXML();
        }

        /// <summary>
        /// Genera la parte comune di STILE del file XML
        /// </summary>
        private void styleXLS_RepAv()
        {
            this._generatedFile += "<Styles>";
            this._generatedFile += "<Style ss:ID=\"Default\" ss:Name=\"Normal\">";
            this._generatedFile += " <Alignment ss:Vertical=\"Bottom\"/>";
            this._generatedFile += " <Borders/>";
            this._generatedFile += " <Font/>";
            this._generatedFile += " <Interior/>";
            this._generatedFile += " <NumberFormat/>";
            this._generatedFile += " <Protection/>";
            this._generatedFile += "</Style>";
            this._generatedFile += "<Style ss:ID=\"s_intestazione\">";
            this._generatedFile += " <Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/>";
            this._generatedFile += " <Borders>";
            this._generatedFile += " <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            this._generatedFile += " <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            this._generatedFile += "  <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            this._generatedFile += "  <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"2\"/>";
            this._generatedFile += " </Borders>";
            this._generatedFile += " <Font x:Family=\"Swiss\" ss:Bold=\"1\"/>";
            this._generatedFile += " <Interior ss:Color=\"#969696\" ss:Pattern=\"Solid\"/>";
            this._generatedFile += "</Style>";
            this._generatedFile += "<Style ss:ID=\"s_default\">";
            this._generatedFile += " <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Top\" ss:WrapText=\"1\"/>";
            this._generatedFile += " <Borders>";
            this._generatedFile += "  <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            this._generatedFile += "  <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            this._generatedFile += "  <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            this._generatedFile += " <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            this._generatedFile += " </Borders>";
            this._generatedFile += " <NumberFormat ss:Format=\"@\"/>";
            this._generatedFile += "</Style>";
            this._generatedFile += "<Style ss:ID=\"s_tot\">";
            this._generatedFile += " <Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Top\"/>";
            this._generatedFile += "<Borders>";
            this._generatedFile += "  <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            this._generatedFile += "  <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            this._generatedFile += "  <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            this._generatedFile += "  <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>";
            this._generatedFile += " </Borders>";
            this._generatedFile += " <NumberFormat ss:Format=\"@\"/>";
            this._generatedFile += "</Style>";
            this._generatedFile += "</Styles>";
        }

        /// <summary>
        /// Genera la parte di apertura TABELLA del file XML
        /// </summary>
        /// <param name="tipo">tipo di report da generare</param>
        private void apreTabellaXLS_RepAv(string tipo)
        {
            switch (tipo)
            {
                case "TX_R":
                    this._generatedFile += "<Worksheet ss:Name=\"Nr.trasm. da ruolo+utente\">";
                    this._generatedFile += " <Table ss:ExpandedColumnCount=\"5\" x:FullColumns=\"1\" x:FullRows=\"1\">";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"108.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"187.5\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"161.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"82.5\"/>";
                    break;
                case "TX_P":
                    this._generatedFile += "<Worksheet ss:Name=\"Nr.trasm. a ruolo pendenti\">";
                    this._generatedFile += " <Table ss:ExpandedColumnCount=\"4\" x:FullColumns=\"1\" x:FullRows=\"1\">";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"108.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"213.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"161.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"108.75\"/>";
                    break;
                case "PR_REG":
                    this._generatedFile += "<Worksheet ss:Name=\"Nr.doc. per registro\">";
                    if (this._protoIntEnabled)
                        this._generatedFile += " <Table ss:ExpandedColumnCount=\"8\" x:FullColumns=\"1\" x:FullRows=\"1\">";
                    else
                        this._generatedFile += " <Table ss:ExpandedColumnCount=\"7\" x:FullColumns=\"1\" x:FullRows=\"1\">";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"108.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"82.5\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"42.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"56.25\"/>";
                    if (this._protoIntEnabled)
                        this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"56.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"71.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"60\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"54.75\"/>";
                    break;
                case "PR_REG_R":
                    this._generatedFile += "<Worksheet ss:Name=\"Nr.doc. per registro+ruolo\">";
                    if (this._protoIntEnabled)
                        this._generatedFile += " <Table ss:ExpandedColumnCount=\"9\" x:FullColumns=\"1\" x:FullRows=\"1\">";
                    else
                        this._generatedFile += " <Table ss:ExpandedColumnCount=\"8\" x:FullColumns=\"1\" x:FullRows=\"1\">";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"108.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"161.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"82.5\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"42.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"56.25\"/>";
                    if (this._protoIntEnabled)
                        this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"56.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"71.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"60\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"54.75\"/>";
                    break;
                case "PFCNC_REG_R":
                    this._generatedFile += "<Worksheet ss:Name=\"Doc da decidere il nome\">";
                    this._generatedFile += " <Table ss:ExpandedColumnCount=\"7\" x:FullColumns=\"1\" x:FullRows=\"1\">";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"70.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.5\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.25\"/>";
                    break;
                case "INTEROP_REG":
                    this._generatedFile += "<Worksheet ss:Name=\"Doc interoperabilità\">";
                    this._generatedFile += " <Table ss:ExpandedColumnCount=\"7\" x:FullColumns=\"1\" x:FullRows=\"1\">";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"70.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.5\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.25\"/>";
                    break;
                case "PEC_REG":
                    this._generatedFile += "<Worksheet ss:Name=\"Ricevuti e protocollati \">";
                    this._generatedFile += " <Table ss:ExpandedColumnCount=\"4\" x:FullColumns=\"1\" x:FullRows=\"1\">";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"70.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.5\"/>";
                    break;
                case "TRASM_EVI_RIF_REG":
                    this._generatedFile += "<Worksheet ss:Name=\"Trasmissioni con evidenza Rifiuti \">";
                    this._generatedFile += " <Table ss:ExpandedColumnCount=\"5\" x:FullColumns=\"1\" x:FullRows=\"1\">";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"70.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.25\"/>";
                    break;
                case "TX_R_RIC":
                    this._generatedFile += "<Worksheet ss:Name=\"Nr.trasm. ric da ruolo+utente\">";
                    this._generatedFile += " <Table ss:ExpandedColumnCount=\"6\" x:FullColumns=\"1\" x:FullRows=\"1\">";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"70.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.25\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.75\"/>";
                    this._generatedFile += "  <Column ss:AutoFitWidth=\"1\" ss:Width=\"135.25\"/>";
                    break;
            }
        }

        /// <summary>
        /// Genera la parte comune di chiusura della TABELLA del file XML
        /// </summary>
        private void chiudeTabellaXLS_RepAv()
        {
            this._generatedFile += "</Table>";
            this._generatedFile += "<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">";
            this._generatedFile += " <PageSetup>";
            this._generatedFile += "  <Layout x:Orientation=\"Landscape\"/>";
            this._generatedFile += " </PageSetup>";
            this._generatedFile += " <Print>";
            this._generatedFile += "  <ValidPrinterInfo/>";
            this._generatedFile += "  <PaperSizeIndex>9</PaperSizeIndex>";
            this._generatedFile += "  <HorizontalResolution>-2</HorizontalResolution>";
            this._generatedFile += "  <VerticalResolution>0</VerticalResolution>";
            this._generatedFile += " </Print>";
            this._generatedFile += " <Selected/>";
            this._generatedFile += " <Panes />";
            this._generatedFile += " <ProtectObjects>False</ProtectObjects>";
            this._generatedFile += " <ProtectScenarios>False</ProtectScenarios>";
            this._generatedFile += "</WorksheetOptions>";
            this._generatedFile += "</Worksheet>";
            this._generatedFile += "</Workbook>";
        }

        /// <summary>
        /// Genera la parte di INTESTAZIONE dei campi del file XML
        /// </summary>
        /// <param name="tipo">tipo di report da generare</param>
        private void intestazioneColonneXLS_RepAv(string tipo)
        {
            switch (tipo)
            {
                case "TX_R":

                    this._generatedFile += "  <Row ss:AutoFitHeight=\"0\" ss:Height=\"13.5\">";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">REGISTRO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">RUOLO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">UTENTE</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">RAGIONE TRASM.</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">TRASM. EFFETT.</Data></Cell>";
                    this._generatedFile += "  </Row>";
                    break;
                case "TX_P":

                    this._generatedFile += "  <Row ss:AutoFitHeight=\"0\" ss:Height=\"13.5\">";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">REGISTRO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">RUOLO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">RAGIONE TRASM.</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">TRASM. PENDENTI</Data></Cell>";
                    this._generatedFile += "  </Row>";
                    break;
                case "PR_REG":

                    this._generatedFile += "  <Row ss:AutoFitHeight=\"0\" ss:Height=\"13.5\">";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">REGISTRO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">PROTOCOLLATI</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">ARRIVO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">PARTENZA</Data></Cell>";
                    if (this._protoIntEnabled)
                        this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">INTERNI</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">FASCICOLATI</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">ANNULLATI</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">IMMAGINE</Data></Cell>";
                    this._generatedFile += "  </Row>";
                    break;
                case "PR_REG_R":

                    this._generatedFile += "  <Row ss:AutoFitHeight=\"0\" ss:Height=\"13.5\">";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">REGISTRO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">RUOLO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">PROTOCOLLATI</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">ARRIVO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">PARTENZA</Data></Cell>";
                    if (this._protoIntEnabled)
                        this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">INTERNI</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">FASCICOLATI</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">ANNULLATI</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">IMMAGINE</Data></Cell>";
                    this._generatedFile += "  </Row>";
                    break;
                case "PFCNC_REG_R":
                    this._generatedFile += "  <Row ss:AutoFitHeight=\"0\" ss:Height=\"13.5\">";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">REGISTRO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">PROTOCOLLATI IN ARRIVO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">FASCICOLATI IN ARRIVO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">CLASSIFICATI IN ARRIVO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">PROTOCOLLATI IN PARTENZA</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">FASCICOLATI IN PARTENZA</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">CLASSIFICATI IN PARTENZA</Data></Cell>";
                    this._generatedFile += "  </Row>";
                    break;
                case "INTEROP_REG":
                    this._generatedFile += "  <Row ss:AutoFitHeight=\"0\" ss:Height=\"13.5\">";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">REGISTRO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">PROTOCOLLI IN INGRESSO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">PROTOCOLLI IN USCITA</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">SPEDITI INT. ESTERNA</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">SPEDITI INT. INTERNA</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">RICEVUTI INT. INTERNA</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">RICEVUTI INT. ESTERNA</Data></Cell>";
                    this._generatedFile += "  </Row>";
                    break;
                case "PEC_REG":
                    this._generatedFile += "  <Row ss:AutoFitHeight=\"0\" ss:Height=\"13.5\">";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">REGISTRO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">RICEVUTI MAIL ESTERNA</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">PROTO MAIL ESTERNA</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">PROTO IN TEMPO RICHIESTO</Data></Cell>";
                    this._generatedFile += "  </Row>";
                    break;
                case "TRASM_EVI_RIF_REG":
                    this._generatedFile += "  <Row ss:AutoFitHeight=\"0\" ss:Height=\"13.5\">";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">REGISTRO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">RUOLO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">UTENTE</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">NUMERO TRASM</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">NUMERO TRASM RIF</Data></Cell>";
                    this._generatedFile += "  </Row>";
                    break;
                case "TX_R_RIC":
                    this._generatedFile += "  <Row ss:AutoFitHeight=\"0\" ss:Height=\"13.5\">";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">REGISTRO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">RUOLO</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">UTENTE</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">NUMERO TRASM</Data></Cell>";
                    this._generatedFile += "   <Cell ss:StyleID=\"s_intestazione\"><Data ss:Type=\"String\">NUMERO TRASM ACC</Data></Cell>";
                    this._generatedFile += "  </Row>";
                    break;
            }
        }

        /// <summary>
        /// Genera la parte delle RIGHE che compongono il file XML
        /// </summary>
        /// <param name="objReport">oggetto ExportExcelClass</param>
        private void aggiungeRigheXLS_RepAv(DocsPaVO.ExportData.ExportExcelClass objReport)
        {
            foreach (DocsPaVO.ExportData.ExportDataExcel dato in this._objList)
            {
                switch (objReport.filtro.tipologiaReport)
                {
                    case "TX_R":

                        this._generatedFile += "<Row ss:AutoFitHeight=\"1\">";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato1 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato2 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato3 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato4 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato5 + "</Data></Cell>";
                        this._generatedFile += "</Row>";

                        break;
                    case "TX_P":

                        this._generatedFile += "<Row ss:AutoFitHeight=\"1\">";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato1 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato2 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato3 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato4 + "</Data></Cell>";
                        this._generatedFile += "</Row>";

                        break;
                    case "PR_REG":

                        this._generatedFile += "<Row ss:AutoFitHeight=\"1\">";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato1 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato2 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato3 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato4 + "</Data></Cell>";
                        if (this._protoIntEnabled)
                            this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato10 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato5 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato6 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato7 + "</Data></Cell>";
                        this._generatedFile += "</Row>";

                        break;
                    case "PR_REG_R":

                        this._generatedFile += "<Row ss:AutoFitHeight=\"1\">";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato1 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato2 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato3 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato4 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato5 + "</Data></Cell>";
                        if (this._protoIntEnabled)
                            this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato10 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato6 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato7 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato8 + "</Data></Cell>";
                        this._generatedFile += "</Row>";

                        break;
                    case "PFCNC_REG_R":
                        this._generatedFile += "<Row ss:AutoFitHeight=\"1\">";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato1.ToString() + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato2 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato3 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato4 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato5 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato6 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato7 + "</Data></Cell>";
                        this._generatedFile += "</Row>";
                        break;
                    case "INTEROP_REG":
                        this._generatedFile += "<Row ss:AutoFitHeight=\"1\">";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato1.ToString() + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato2 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato3 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato4 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato5 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato6 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato7 + "</Data></Cell>";
                        this._generatedFile += "</Row>";
                        break;
                    case "PEC_REG":
                        this._generatedFile += "<Row ss:AutoFitHeight=\"1\">";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato1.ToString() + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato2 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato3 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato4 + "</Data></Cell>";
                        this._generatedFile += "</Row>";
                        break;
                    case "TRASM_EVI_RIF_REG":
                        this._generatedFile += "<Row ss:AutoFitHeight=\"1\">";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato1.ToString() + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"String\">" + dato.dato2 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"String\">" + dato.dato3 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato4 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato5 + "</Data></Cell>";
                        this._generatedFile += "</Row>";
                        break;
                    case "TX_R_RIC":
                        this._generatedFile += "<Row ss:AutoFitHeight=\"1\">";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_default\"><Data ss:Type=\"String\">" + dato.dato1 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"String\">" + dato.dato2 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"String\">" + dato.dato3 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato4 + "</Data></Cell>";
                        this._generatedFile += "    <Cell ss:StyleID=\"s_tot\"><Data ss:Type=\"Number\">" + dato.dato5 + "</Data></Cell>";
                        this._generatedFile += "</Row>";
                        break;
                }
            }
        }

        /// <summary>
        /// Verifica se l'amm.ne ha il protocollo interno e imposta la variabile _protoIntEnabled
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        private void checkProtoIntEnabled(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();
            this._protoIntEnabled = obj.IsEnabledProtoInt(idAmm);
        }

        #endregion

        public DocsPaVO.documento.FileDocumento ExportDocRicOrder(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] filtri, string exportType, string title, DocsPaVO.ricerche.FullTextSearchContext context, ArrayList campiSelezionati, bool mittDest_indirizzo, String[] documentsSystemId, Grid grid, bool useTraditionaExport)
        {

            this._filtri = filtri;
            this._exportType = exportType;
            this._title = title;
            string idGruppo = infoUtente.idGruppo;
            string idPeople = infoUtente.idPeople;
            this._descAmm = this.getNomeAmministrazione(idPeople);
            this.infoUser = infoUtente;
            //VERONICA: gestione ricerca veloce full text
            if ((context != null) && (context.TextToSearch != ""))
            {
                ArrayList documenti = null;
                try
                {
                    documenti = Documenti.InfoDocManager.FullTextSearch(infoUtente, ref context);

                    // Se ci sono system id di documenti selezionati, viene effettuato un filtraggio 
                    // dei risultati
                    if (documentsSystemId != null &&
                        documentsSystemId.Length > 0)
                    {
                        ArrayList tmp = new ArrayList();

                        foreach (InfoDocumento doc in documenti)
                            if (documentsSystemId.Where(e => e.Equals(doc.idProfile)).Count() > 0)
                                tmp.Add(doc);

                        documenti = tmp;
                    }


                    switch (exportType)
                    {
                        case "PDF":
                            this.exportDocRicFullTextPDF(documenti);
                            break;
                        case "XLS":
                            //this.exportDocRicFullTextXLS(documenti, campiSelezionati);
                            this.exportDocRicFullTextXLS(documenti, campiSelezionati);
                            break;
                        //case "Model":
                        //    this.exportDocRicFromModel(documenti, campiSelezionati);
                        //    break;
                    }
                }
                catch (Exception ex)
                {
                    this._file = null;
                    logger.Debug(ex);
                }
            }
            else //ricerca normale
            {
                switch (exportType)
                {
                    case "PDF":
                        //                        this.exportDocPDF(idGruppo, idPeople, mittDest_indirizzo);
                        // this.exportDocPDF(infoUtente, mittDest_indirizzo, documentsSystemId);
                        if (!useTraditionaExport)
                        {
                            this.exportDocPDF(infoUtente, mittDest_indirizzo, documentsSystemId);
                        }
                        else
                        {
                            this.exportDocPDFRicOrder(infoUtente, mittDest_indirizzo, documentsSystemId);
                        }
                        break;
                    case "XLS":
                        //this.exportDocXLS(idGruppo, idPeople, campiSelezionati);
                        //                        this.exportDocXLS(idGruppo, idPeople, campiSelezionati, mittDest_indirizzo);
                        if (!useTraditionaExport)
                        {
                            this.exportDocXLS(infoUtente, campiSelezionati, mittDest_indirizzo, documentsSystemId, grid, useTraditionaExport);
                        }
                        else
                        {
                            this.exportDocXLSRicOrder(infoUtente, campiSelezionati, mittDest_indirizzo, documentsSystemId, grid, useTraditionaExport);
                        }
                        break;
                    case "Model":
                        this.exportDocRicFromModel(infoUtente, campiSelezionati, mittDest_indirizzo, documentsSystemId, grid);
                        break;
                }
            }
            return this._file;
        }

        private void exportDocXLSRicOrder(DocsPaVO.utente.InfoUtente infoUtente, ArrayList campiSelezionati, bool mittDest_indirizzo, String[] documentsSystemId, Grid grid, bool useTraditionaExport)
        {
            try
            {
                //Recupero i dati
                //ArrayList documenti = Documenti.InfoDocManager.getQueryExport(idGruppo, idPeople, this._filtri, mittDest_indirizzo);
                //   ArrayList documenti = Documenti.InfoDocManager.getQueryExport(infoUtente, this._filtri, mittDest_indirizzo, documentsSystemId);
                ArrayList documenti = Documenti.InfoDocManager.getQueryExportAsRic(infoUtente.idGruppo, infoUtente.idPeople, this._filtri, true, documentsSystemId);


                if (documenti == null || documenti.Count == 0)
                {
                    this._file = null;
                    return;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLDocGridsAsRic(documenti, campiSelezionati, grid, infoUtente, useTraditionaExport);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportDocumenti.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportDocumenti.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportDocumenti";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione documenti : " + ex.Message);
            }
        }

        /// <summary>
        /// Generazione del file (PDF) di export dei documenti
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>		
        /// <returns></returns>
        private void exportDocPDFRicOrder(DocsPaVO.utente.InfoUtente infoUtente, bool mittDest_indirizzo, String[] documentsSystemId)
        {
            try
            {
                //  this._objList = Documenti.InfoDocManager.getQueryExport(infoUtente, this._filtri, mittDest_indirizzo, documentsSystemId);
                this._objList = Documenti.InfoDocManager.getQueryExportAsRic(infoUtente.idGruppo, infoUtente.idPeople, this._filtri, true, documentsSystemId);

                if (this._objList.Count > 0)
                {
                    this._rowsList = Convert.ToString(this._objList.Count);

                    // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                    // iTextSharp per la stampa dei risultati della ricerca
                    // ...creazione del report PDF con iTextSharp
                    this._file = this.CreaPDFRisRic("D", "");
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documenti"></param>
        /// <param name="campiSelezionati"></param>
        /// <param name="grid">da utilizzare per individuare i campi da esportare</param>
        /// <param name="userInfo">Informazioni sull'utente che ha richiesto l'export</param>
        /// <returns></returns>
        private StringBuilder creaXMLDocGridsAsRic(ArrayList documenti, ArrayList campiSelezionati, Grid grid, InfoUtente userInfo, bool useTraditionaExport)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += this.CreateDocumentSheetAsRic(documenti, campiSelezionati, grid, userInfo, useTraditionaExport);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());

            return sb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documenti"></param>
        /// <param name="campiSelezionati"></param>
        /// <param name="grid">Griglia da utilizzare per individuare i campi da esportare</param>
        /// <param name="userInfo">Informazioni sull'utente che ha richiesto l'esportazione</param>
        /// <param name="useTraditionaExport">True se bisogna utilizzare l'export pre griglia</param>
        /// <returns></returns>
        private string CreateDocumentSheetAsRic(ArrayList documenti, ArrayList campiSelezionati, Grid grid, InfoUtente userInfo, bool useTraditionaExport)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"DOCUMENTI\">";
            strXML += "<Table>";
            if (Grids.GridManager.ExistGridPersonalizationFunction() && grid != null && !useTraditionaExport)
            {
                strXML += this.CreateExcelTable(campiSelezionati, grid);
                // Inserimento del titolo, se presente, e del sommario di export
                strXML += this.InsertTitleAndSummaryExportDoc(documenti.Count);
                strXML += PopulateDocumentTable(documenti, campiSelezionati, grid, userInfo);
            }
            else
            {
                strXML += creaTabellaDocumenti_old(campiSelezionati);
                strXML += this.InsertTitleAndSummaryExportDoc(documenti.Count);
                strXML += datiDocumentiXML_OLDAsRic(documenti, campiSelezionati);
            }

            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }


        private string datiDocumentiXML_OLDAsRic(ArrayList documenti, ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            strXML = creaColonneDocumenti(campiSelezionati);
            strXML += inserisciDatiDocumentiRicOld(documenti, campiSelezionati);
            return strXML;
        }

        private string inserisciDatiDocumentiRicOld(ArrayList documenti, ArrayList campiSelezionati)
        {
            string righe = string.Empty;
            foreach (DocsPaVO.documento.InfoDocumento documento in documenti)
            {
                righe += inserisciRigaDocumentiRicOrder(documento, campiSelezionati);
            }
            return righe;
        }

        private string inserisciRigaDocumentiRicOrder(DocsPaVO.documento.InfoDocumento documento, ArrayList campiSelezionati)
        {
            string riga = string.Empty;

            riga = "<Row>";

            //Inserimento Campi Standard
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                if (campoSelezionato.campoStandard == "1")
                {
                    switch (campoSelezionato.nomeCampo)
                    {
                        case "Registro":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + documento.codRegistro;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;
                        case "Prot. / Id. Doc.":
                            string idOrNumProto = string.Empty;
                            if (string.IsNullOrEmpty(documento.segnatura))
                            {
                                idOrNumProto = documento.idProfile;
                            }
                            else
                            {
                                idOrNumProto = documento.numProt;
                            }
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + idOrNumProto;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Data":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + documento.dataApertura;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Oggetto":
                            riga += "<Cell ss:StyleID=\"s30\">";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + documento.oggetto + "]]>";
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Tipo":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + getLettereProtocolli(documento.tipoProto);
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Mitt. / Dest.":
                            string str_mittDest = string.Empty;
                            if (documento.mittDest.Count > 0)
                            {
                                for (int g = 0; g < documento.mittDest.Count; g++)
                                    str_mittDest += documento.mittDest[g] + " - ";
                                str_mittDest = str_mittDest.Substring(0, str_mittDest.Length - 3);
                            }
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + str_mittDest + "]]>";
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Cod. Fascicoli":
                            string codFasc = string.Empty;
                            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                            codFasc = doc.DocumentoGetCodiciClassifica(documento.idProfile, this.infoUser);
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + codFasc;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Annullato":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + documento.dataAnnullamento;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "File":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\">" + documento.acquisitaImmagine;
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Motivo Rimozione":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + documento.noteCestino + "]]>";
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;

                        case "Note":
                            riga += "<Cell>";
                            riga += "<Data ss:Type=\"String\"><![CDATA[" + documento.ultimaNota + "]]>";
                            riga += "</Data>";
                            riga += "</Cell>";
                            break;
                    }
                }
            }

            //Inserimento Campi della profilazione dinamica
            if (documento.idTipoAtto != null && documento.idTipoAtto != "")
            {
                DocsPaVO.ProfilazioneDinamica.Templates templateVuoto = ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(documento.idTipoAtto);
                if (templateVuoto != null && templateVuoto.ID_AMMINISTRAZIONE != null && templateVuoto.ID_AMMINISTRAZIONE != "")
                {
                    DocsPaVO.ProfilazioneDinamica.Templates template = ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateDettagli(documento.docNumber);
                    if (template != null && template.ELENCO_OGGETTI.Count != 0)
                    {
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                        {
                            foreach (DocsPaVO.ExportData.CampoSelezionato campoSelezionato in campiSelezionati)
                            {
                                if (oggettoCustom != null && campoSelezionato.nomeCampo == oggettoCustom.DESCRIZIONE && campoSelezionato.campoStandard != "1")
                                {
                                    riga += "<Cell>";
                                    riga += "<Data ss:Type=\"String\"><![CDATA[";
                                    riga += getValoreOggettoCustom(oggettoCustom);
                                    riga += "]]></Data>";
                                    riga += "</Cell>";
                                }
                            }
                        }
                    }
                }
            }

            riga += "</Row>";
            return riga;
        }

        /// <summary>
        /// Conversione dei dati dei documenti in formato XML
        /// </summary>		
        private void exportDocToXMLRicOrder()
        {
            this.addAttToRootNode();

            /* per i doc usiamo questi dati
            Numero_protocollo,
            Data_di_protocollo,
            Tipologia_di_documento,
            Data_annullamento,
            Oggetto,
            Mittenti_o_destinatari,
            Codice_del_fascicolo_che_contiene_il_documento	
            Codice_registro
            Immagine
            */

            foreach (DocsPaVO.documento.InfoDocumento documento in this._objList)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                XmlElement numProt = this._xmlDoc.CreateElement("NUM_PROTOCOLLO");
                string idOrNumProto = string.Empty;
                if (string.IsNullOrEmpty(documento.segnatura))
                {
                    idOrNumProto = documento.idProfile;
                }
                else
                {
                    idOrNumProto = documento.numProt;
                }
                numProt.InnerText = idOrNumProto;
                record.AppendChild(numProt);

                XmlElement dataProt = this._xmlDoc.CreateElement("DATA_PROTOCOLLO");
                dataProt.InnerText = documento.dataApertura;
                record.AppendChild(dataProt);

                XmlElement tipoDoc = this._xmlDoc.CreateElement("TIPO_DOC");
                //MODIFICATO DA FABIO PER CAMBIO ETICHETTE
                // Aggiunta del tipo di documento
                tipoDoc.InnerText = getLettereProtocolli(documento.tipoProto);
                record.AppendChild(tipoDoc);

                XmlElement dataAnn = this._xmlDoc.CreateElement("DATA_ANNULLA");
                dataAnn.InnerText = documento.dataAnnullamento;
                record.AppendChild(dataAnn);

                XmlElement oggetto = this._xmlDoc.CreateElement("OGGETTO");
                oggetto.InnerText = documento.oggetto;
                record.AppendChild(oggetto);

                XmlElement mittDest = this._xmlDoc.CreateElement("MITT_DEST");
                string str_mittDest = string.Empty;
                if (documento.mittDest.Count > 0)
                {
                    for (int g = 0; g < documento.mittDest.Count; g++)
                        str_mittDest += documento.mittDest[g] + " - ";
                    str_mittDest = str_mittDest.Substring(0, str_mittDest.Length - 3);
                }
                mittDest.InnerText = str_mittDest;
                record.AppendChild(mittDest);

                XmlElement codFasc = this._xmlDoc.CreateElement("COD_FASC");
                string codFasc2 = string.Empty;
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                codFasc2 = doc.DocumentoGetCodiciClassifica(documento.idProfile, this.infoUser);
                codFasc.InnerText = codFasc2;
                record.AppendChild(codFasc);



                XmlElement codReg = this._xmlDoc.CreateElement("COD_REG");
                codReg.InnerText = documento.codRegistro;
                record.AppendChild(codReg);

                XmlElement immagine = this._xmlDoc.CreateElement("IMG");
                immagine.InnerText = documento.acquisitaImmagine;
                record.AppendChild(immagine);

                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="registro"></param>
        /// <param name="enableUfficioRef"></param>
        /// <param name="enableProfilazione"></param>
        /// <param name="enableChilds"></param>
        /// <param name="classificazione"></param>
        /// <param name="filtri"></param>
        /// <param name="exportType"></param>
        /// <param name="title"></param>
        /// <param name="campiSelezionati"></param>
        /// <param name="idProjectsList"></param>
        /// <param name="grid">Griglia da cui prelevare le informazioni sui campi da esportare</param>
        /// <returns></returns>
        public DocsPaVO.documento.FileDocumento ExportFascRicOrder(DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, bool enableChilds, DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.filtri.FiltroRicerca[][] filtri, string exportType, string title, ArrayList campiSelezionati, String[] idProjectsList, Grid grid, bool useTraditionaExport)
        {
            this._filtri = filtri;
            this._exportType = exportType;
            this._title = title;
            this._descAmm = this.getNomeAmministrazione(userInfo.idPeople);

            switch (exportType)
            {
                case "PDF":
                    this.exportFascPDF(userInfo, registro, enableUfficioRef, enableProfilazione, enableChilds, classificazione, idProjectsList);
                    break;
                case "XLS":
                    //this.exportFascXLS(userInfo, registro, enableUfficioRef, enableProfilazione, enableChilds, classificazione, campiSelezionati);
                    this.exportFascXLSRicOrder(userInfo, registro, enableUfficioRef, enableProfilazione, enableChilds, classificazione, campiSelezionati, idProjectsList, grid, useTraditionaExport);
                    break;
            }

            return this._file;
        }

        private void exportFascXLSRicOrder(DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, bool enableChilds, DocsPaVO.fascicolazione.Classificazione classificazione, ArrayList campiSelezionati, String[] idProjectsList, Grid grid, bool useTraditionaExport)
        {
            try
            {
                //Recupero i dati
                DocsPaDB.Query_DocsPAWS.Fascicoli objFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                //using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
                /*    listaFascicoli = fascicoli.GetListaFascicoliPaging(infoUtente, objClassificazione, registro, filtriFascicoli, filtriDocumentiInFascicoli, enableUfficioRef, enableProfilazione, childs, out numTotPage, out nRec, numPage, pageSize, getSystemIdList, out idProjectList);*/
                /*      ArrayList fascicoli = objFasc.GetListaFascicoliPaging(userInfo, classificazione, this._filtri[0], enableUfficioRef, enableProfilazione, enableChilds, registro, null, "", idProjectsList);*/

                ArrayList fascicoli = objFasc.GetListaFascicoliPagingExportOrder(userInfo, classificazione, registro, this._filtri[0], enableUfficioRef, enableProfilazione, enableChilds, idProjectsList);

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLFasc(fascicoli, campiSelezionati, registro, grid, infoUser, useTraditionaExport);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportFascicoli.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportFascicoli.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportFascicoli";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione fascicoli : " + ex.Message);
            }
        }


        public DocsPaVO.documento.FileDocumento ExportDocCustom(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] filtri, string exportType, string title, DocsPaVO.ricerche.FullTextSearchContext context, ArrayList campiSelezionati, String[] documentsSystemId, Grid grid, bool gridPersonalization, Field[] visibleFieldsTemplate)
        {

            this._filtri = filtri;
            this._exportType = exportType;
            this._title = title;
            string idGruppo = infoUtente.idGruppo;
            string idPeople = infoUtente.idPeople;
            this._descAmm = this.getNomeAmministrazione(idPeople);
            this.infoUser = infoUtente;

            switch (exportType)
            {
                case "PDF":
                    this.exportDocPDFCustom(infoUtente, documentsSystemId, gridPersonalization, visibleFieldsTemplate);
                    break;
                case "XLS":
                    this.exportDocXLSCustom(infoUtente, documentsSystemId, gridPersonalization, visibleFieldsTemplate, campiSelezionati, grid, true);
                    break;
                case "Model":
                    this.exportDocRicFromModelCustom(infoUtente, documentsSystemId, gridPersonalization, visibleFieldsTemplate, campiSelezionati, grid);
                    break;
                case "ODS":
                    this.exportDocODSCustom(infoUtente, documentsSystemId, gridPersonalization, visibleFieldsTemplate, campiSelezionati, grid, false);
                    break;
            }
            return this._file;
        }

        /// <summary>
        /// Generazione del file (PDF) di export dei documenti
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>		
        /// <returns></returns>
        private void exportDocPDFCustom(DocsPaVO.utente.InfoUtente infoUtente, String[] documentsSystemId, bool gridPersonalization, Field[] visibleFieldsTemplate)
        {
            try
            {
                int numTotPage = 0;
                int nRec = 0;
                List<SearchResultInfo> toSet = new List<SearchResultInfo>();
                this._objList = BusinessLogic.Documenti.InfoDocManager.getQueryPagingCustom(infoUtente, this._filtri, 1, 20, true, true, gridPersonalization, visibleFieldsTemplate, documentsSystemId, out numTotPage, out nRec, false, out toSet);

                if (this._objList.Count > 0)
                {
                    this._rowsList = Convert.ToString(this._objList.Count);

                    // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                    // ...creazione del report PDF con iTextSharp
                    this._file = this.CreaPDFRisRicCustom("D", "");
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        /// <summary>
        /// Conversione dei dati dei documenti in formato XML
        /// </summary>		
        private void exportDocToXMLCustom()
        {
            this.addAttToRootNode();

            foreach (DocsPaVO.Grids.SearchObject documento in this._objList)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                XmlElement numProt = this._xmlDoc.CreateElement("NUM_PROTOCOLLO");
                string idOrNumProto = string.Empty;
                if (string.IsNullOrEmpty(documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D8")).FirstOrDefault().SearchObjectFieldValue))
                {
                    idOrNumProto = documento.SearchObjectID;
                }
                else
                {
                    idOrNumProto = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                }
                numProt.InnerText = idOrNumProto;
                record.AppendChild(numProt);

                XmlElement dataProt = this._xmlDoc.CreateElement("DATA_PROTOCOLLO");
                dataProt.InnerText = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(dataProt);

                XmlElement tipoDoc = this._xmlDoc.CreateElement("TIPO_DOC");
                //MODIFICATO DA FABIO PER CAMBIO ETICHETTE
                // Aggiunta del tipo di documento
                string tipo = string.Empty;
                tipo = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE")).FirstOrDefault().SearchObjectFieldValue;
                string tempVal = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;
                if (!string.IsNullOrEmpty(tipo))
                {
                    tipo = getLettereProtocolli("ALL");
                }
                else
                {
                    tipo = getLettereProtocolli(tempVal);
                }
                tipoDoc.InnerText = getLettereProtocolli(tipo);
                record.AppendChild(tipoDoc);

                XmlElement dataAnn = this._xmlDoc.CreateElement("DATA_ANNULLA");
                dataAnn.InnerText = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D11")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(dataAnn);

                XmlElement oggetto = this._xmlDoc.CreateElement("OGGETTO");
                oggetto.InnerText = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D4")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(oggetto);

                XmlElement mittDest = this._xmlDoc.CreateElement("MITT_DEST");
                mittDest.InnerText = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D5")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(mittDest);

                XmlElement codFasc = this._xmlDoc.CreateElement("COD_FASC");
                codFasc.InnerText = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D18")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(codFasc);

                XmlElement codReg = this._xmlDoc.CreateElement("COD_REG");
                codReg.InnerText = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D2")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(codReg);

                XmlElement immagine = this._xmlDoc.CreateElement("IMG");
                immagine.InnerText = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D23")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(immagine);

                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DocsPaVO.documento.FileDocumento CreaPDFRisRicCustom(string tipoObj, string param)
        {
            // Il nome del file di template
            string templateFileName;
            // Il datatable utilizzato per creare la tabella con i dati da  visualizzare
            DataTable infoObjs;

            // 1. Determinazione della tipologia di oggetto di cui stampare il report
            switch (tipoObj)
            {
                case "D":   // Documenti
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisRicDoc.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicDocCustom();
                    break;
                case "F":   // Fascicoli
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisRicFasc.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicFasc(param);
                    break;

                case "T":   // Trasmissioni
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisRicTrasm.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicTrasm();
                    break;

                case "DF":  // Documenti in fascicolo
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisRicDoc.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicDocInFascCustom(param);
                    break;

                case "TDL": // To do list di qualsiasi tipo tranne che D
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisRicToDoList.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicToDoList();
                    break;

                case "DC":  // Documenti in cestino
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisDocInCestino.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicDocInCestino();
                    break;

                case "S":   // Scarto
                    // Impostazione del nome del file
                    templateFileName = "XMLRepStampaRisScarto.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableRicScarto();
                    break;

                case "C":   // Conservazione
                    // Impostazione del nome del file
                    templateFileName = "XMLRepExportConservazione.xml";
                    // Caricamento dei dati da inserire nel report
                    infoObjs = this.GetDataTableConservazione(param);
                    break;

                default:
                    throw new Exception("Tipo oggetto non valido: " + tipoObj);

            }

            // 2. Creazionedell'oggetto di generazione PDF
            StampaPDF.StampaRisRicerca report = new StampaPDF.StampaRisRicerca();

            // 3. Restituzione del file documento PDF con le informazioni sui
            //    risultati della ricerca
            return report.GetFileDocumento(templateFileName, this._title, this._descAmm,
     this._objList.Count.ToString(), infoObjs);

        }

        private DataTable GetDataTableRicDocCustom()
        {
            // Creazione del dataset con i dati sui documenti da inserire nel report
            DataTable infoDocs = new DataTable();
            DataRow infoDoc;

            // Creazione della struttura per infoDocs
            infoDocs.Columns.Add("COD_REG");
            infoDocs.Columns.Add("NUM_PROTOCOLLO");
            infoDocs.Columns.Add("DATA_PROTOCOLLO");
            infoDocs.Columns.Add("OGGETTO");
            infoDocs.Columns.Add("TIPO_DOC");
            infoDocs.Columns.Add("MITT_DEST");
            infoDocs.Columns.Add("COD_FASC");
            infoDocs.Columns.Add("DATA_ANNULLA");
            infoDocs.Columns.Add("IMG");

            // Aggiunta delle righe con le informazioni sui documenti
            foreach (DocsPaVO.Grids.SearchObject documento in this._objList)
            {
                // Creazione della nuova riga
                infoDoc = infoDocs.NewRow();

                // Aggiunta del codice registro
                infoDoc["COD_REG"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D2")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta del numero di protocollo
                string idOrNumProto = string.Empty;
                if (string.IsNullOrEmpty(documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D8")).FirstOrDefault().SearchObjectFieldValue))
                {
                    idOrNumProto = documento.SearchObjectID;
                }
                else
                {
                    idOrNumProto = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                }
                infoDoc["NUM_PROTOCOLLO"] = idOrNumProto;

                // Aggiunta della data di apertura
                infoDoc["DATA_PROTOCOLLO"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta dell'oggetto
                infoDoc["OGGETTO"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D4")).FirstOrDefault().SearchObjectFieldValue;

                //MODIFICATO DA FABIO PER CAMBIO ETICHETTE
                // Aggiunta del tipo di documento
                string tipo = string.Empty;
                tipo = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE")).FirstOrDefault().SearchObjectFieldValue;
                string tempVal = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;
                if (!string.IsNullOrEmpty(tipo))
                {
                    tipo = getLettereProtocolli("ALL");
                }
                else
                {
                    tipo = getLettereProtocolli(tempVal);
                }
                infoDoc["TIPO_DOC"] = tipo;

                // Aggiunta delle informazioni su mittente / destinatario
                infoDoc["MITT_DEST"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D5")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta delle informazioni sul codice fascicolo
                infoDoc["COD_FASC"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D18")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta delle informazioni sulla data di annullamento
                infoDoc["DATA_ANNULLA"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D11")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta dell'informazione indicante se il documento è stato acquisito
                infoDoc["IMG"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D23")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta della riga alla tabella
                infoDocs.Rows.Add(infoDoc);

            }

            return infoDocs;

        }

        private void exportDocXLSCustom(DocsPaVO.utente.InfoUtente infoUtente, String[] documentsSystemId, bool gridPersonalization, Field[] visibleFieldsTemplate, ArrayList campiSelezionati, Grid grid, bool excel)
        {
            try
            {
                int numTotPage = 0;
                int nRec = 0;
                List<SearchResultInfo> toSet = new List<SearchResultInfo>();
                ArrayList documenti = BusinessLogic.Documenti.InfoDocManager.getQueryPagingCustom(infoUtente, this._filtri, 1, 20, true, true, gridPersonalization, visibleFieldsTemplate, documentsSystemId, out numTotPage, out nRec, false, out toSet);


                if (documenti == null || documenti.Count == 0)
                {
                    this._file = null;
                    return;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLDocGridsCustom(documenti, campiSelezionati, infoUtente, gridPersonalization, grid, excel);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportDocumenti.xls");
                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportDocumenti.xls");
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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.contentType = "application/vnd.ms-excel";
                    this._file.name = "ExportDocumenti";

                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione documenti : " + ex.Message);
            }
        }

        private StringBuilder creaXMLDocGridsCustom(ArrayList documenti, ArrayList campiSelezionati, InfoUtente userInfo, bool gridPersonalization, Grid grid, bool excel)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML

            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += this.CreateDocumentSheetCustom(documenti, campiSelezionati, userInfo, gridPersonalization, grid);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());

            return sb;
        }

        private string CreateDocumentSheetCustom(ArrayList documenti, ArrayList campiSelezionati, InfoUtente userInfo, bool gridPersonalization, Grid grid)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"DOCUMENTI\">";
            strXML += "<Table>";
            strXML += this.CreateExcelTableCustom(campiSelezionati, grid);
            // Inserimento del titolo, se presente, e del sommario di export
            strXML += this.InsertTitleAndSummaryExportDoc(documenti.Count);
            strXML += PopulateDocumentTableCustom(documenti, campiSelezionati, grid, userInfo);

            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }

        private string CreateExcelTableCustom(ArrayList selectedFields, Grid grid)
        {
            string xmlStr = String.Empty;
            Field d = new Field();

            // Per ogni campo da visualizzare
            foreach (CampoSelezionato selectedField in selectedFields)
            {
                d = (Field)grid.Fields.Where(e => e.FieldId == selectedField.fieldID).FirstOrDefault();
                if (d != null)
                {
                    xmlStr += "<Column ss:StyleID=\"s63\" ss:Width=\"" + d.Width + "\"/>";
                }
                else
                {
                    xmlStr += "<Column ss:StyleID=\"s63\" ss:Width=\"100\"/>";
                }
            }
            // Restituzione dell'XML generato
            return xmlStr;

        }

        private string PopulateDocumentTableCustom(ArrayList documents, ArrayList selectedFields, Grid grid, InfoUtente userInfo)
        {
            string xmlStr = string.Empty;

            // Aggiunta della colonna
            xmlStr = this.creaColonneDocumenti(selectedFields);

            // Inserimento dei dati sui documenti
            xmlStr += this.InsertDocumentsDataCustom(documents, selectedFields, grid, userInfo);

            // Restituzione dell'xml generato
            return xmlStr;
        }

        private string InsertDocumentsDataCustom(ArrayList documents, ArrayList selectedFields, Grid grid, InfoUtente userInfo)
        {
            // XML da restituire
            string toReturn = string.Empty;

            // Per ogni documento...
            foreach (DocsPaVO.Grids.SearchObject document in documents)
                toReturn += this.InsertDocumentDataCustomIn(document, selectedFields, grid, userInfo);

            // Restituzione dell'XML generato
            return toReturn;
        }

        private string InsertDocumentDataCustomIn(DocsPaVO.Grids.SearchObject doc, ArrayList selectedFields, Grid grid, InfoUtente userInfo)
        {
            // Riga da restituire
            string toReturn = String.Empty;

            // Valore da scrivere nell'XML
            String value;



            // Apertura riga
            toReturn = "<Row>";

            // Inserimento dei valori per i campi
            foreach (CampoSelezionato selectedField in selectedFields)
            {
                // field = grid.Fields.Where(e => e.FieldId.Equals(selectedField.fieldID)).First();

                // Selezione del valore da mostrare
                switch (selectedField.fieldID)
                {
                    //SEGNATURA
                    case "D8":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //REGISTRO
                    case "D2":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //TIPO
                    case "D3":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE")).FirstOrDefault().SearchObjectFieldValue;
                        string tempVal = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        if (!string.IsNullOrEmpty(value))
                        {
                            value = getLettereProtocolli("ALL");
                        }
                        else
                        {
                            value = getLettereProtocolli(tempVal);
                        }
                        break;
                    //OGGETTO
                    case "D4":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //MITTENTE / DESTINATARIO
                    case "D5":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //MITTENTE
                    case "D6":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //DESTINATARI
                    case "D7":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //DATA
                    case "D9":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //ESITO PUBBLICAZIONE
                    case "D10":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //DATA ANNULLAMENTO
                    case "D11":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //DOCUMENTO
                    case "D1":
                        string numeroDocumento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        string numeroProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                        if (!String.IsNullOrEmpty(numeroProtocollo))
                            value = numeroProtocollo;
                        else
                            value = numeroDocumento;
                        break;
                    //NUMERO PROTOCOLLO
                    case "D12":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //AUTORE
                    case "D13":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //DATA ARCHIVIAZIONE
                    case "D14":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //PERSONALE
                    case "D15":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        {
                            value = "Si";
                        }
                        else
                        {
                            value = "No";
                        }
                        break;
                    //PRIVATO
                    case "D16":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        {
                            value = "Si";
                        }
                        else
                        {
                            value = "No";
                        }
                        break;
                    //TIPOLOGIA
                    case "U1":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //NOTE
                    case "D17":
                        //    if (IsPresentsNote())
                        //     {
                        //        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ESISTE_NOTA")).FirstOrDefault().SearchObjectFieldValue;
                        //     }
                        //     else
                        //     {
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        //    }
                        //value = (!string.IsNullOrEmpty(doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue) ? "SI" : "NO");
                        break;
                    //COD FASCICOLI
                    case "D18":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //Nome e cognome autore
                    case "D19":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //Ruolo autore
                    case "D20":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //Data arrivo
                    case "D21":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //Stato del documento
                    case "D22":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    case "D23":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    case "CONTATORE":
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //OGGETTI CUSTOM
                    default:
                        SearchObjectField serachObjectFiled = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault();
                        if (serachObjectFiled != null && !string.IsNullOrEmpty(serachObjectFiled.SearchObjectFieldValue))
                        {
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (value.Equals("#CONTATORE_DI_REPERTORIO#"))
                            {
                                string dataAnnullamento = string.Empty;
                                string idDoc = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                                value = Documenti.DocManager.GetSegnaturaRepertorio(idDoc, infoUser.idAmministrazione, false, out dataAnnullamento);
                                if (!string.IsNullOrEmpty(dataAnnullamento))
                                    dataAnnullamento = " - " + dataAnnullamento;
                                value += dataAnnullamento;
                            }
                        }
                        else
                        {
                            value = "";
                        }
                        break;
                }

                toReturn += "<Cell ss:StyleID=\"s30\">";
                toReturn += "<Data ss:Type=\"String\"><![CDATA[";
                toReturn += value;
                toReturn += "]]></Data>";
                toReturn += "</Cell>";

            }

            // Chiusura della riga
            toReturn += "</Row>";

            // Restituzione della riga creata
            return toReturn;
        }

        public DocsPaVO.documento.FileDocumento ExportFascCustom(DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, bool enableChilds, DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.filtri.FiltroRicerca[][] filtri, string exportType, string title, ArrayList campiSelezionati, String[] idProjectsListGrid, Grid grid, bool gridPersonalization, Field[] visibleFieldsTemplate, bool security)
        {
            this._filtri = filtri;
            this._exportType = exportType;
            this._title = title;
            this._descAmm = this.getNomeAmministrazione(userInfo.idPeople);

            switch (exportType)
            {
                case "PDF":
                    this.exportFascPDFCustom(userInfo, registro, enableUfficioRef, enableProfilazione, enableChilds, classificazione, idProjectsListGrid, visibleFieldsTemplate, gridPersonalization, security);
                    break;
                case "XLS":
                    this.exportFascXLSCustom(userInfo, registro, enableUfficioRef, enableProfilazione, enableChilds, classificazione, campiSelezionati, idProjectsListGrid, grid, gridPersonalization, visibleFieldsTemplate, true, security);
                    break;
                case "ODS":
                    this.exportFascODSCustom(userInfo, registro, enableUfficioRef, enableProfilazione, enableChilds, classificazione, campiSelezionati, idProjectsListGrid, grid, gridPersonalization, visibleFieldsTemplate, false, security);
                    break;
            }

            return this._file;
        }

        private void exportFascPDFCustom(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, bool enableChilds, DocsPaVO.fascicolazione.Classificazione classificazione, String[] idProjectsList, Field[] visibleFieldsTemplate, bool gridPersonalization, bool security)
        {
            try
            {
                int nRec = 0;
                int numTotPage = 0;

                List<SearchResultInfo> idProjects = new List<SearchResultInfo>();
                DocsPaDB.Query_DocsPAWS.Fascicoli objFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                this._objList = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPagingCustom(infoUtente, classificazione, registro, this._filtri[0], enableUfficioRef, enableProfilazione, enableChilds, out numTotPage, out  nRec, 1, 20, false, out idProjects, null, string.Empty, gridPersonalization, true, visibleFieldsTemplate, idProjectsList, security);

                if (this._objList.Count > 0)
                {
                    this._rowsList = Convert.ToString(this._objList.Count);

                    // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                    // ...creazione del report PDF con iTextSharp
                    this._file = this.CreaPDFRisRicCustom("F", "");
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        private DataTable GetDataTableRicFascCustom(string codRegistro)
        {
            // Creazione del dataset con i dati sui fascicoli da inserire nel report
            DataTable infoFascs = new DataTable();
            DataRow infoFasc;

            // Creazione della struttura per infoFascs
            infoFascs.Columns.Add("COD_REG");
            infoFascs.Columns.Add("TIPO_FASC");
            infoFascs.Columns.Add("COD_FASC");
            infoFascs.Columns.Add("DESC_FASC");
            infoFascs.Columns.Add("DATA_A");
            infoFascs.Columns.Add("DATA_C");
            infoFascs.Columns.Add("COLL_FIS");

            // Aggiunta delle righe con le informazioni sui fascicoli
            foreach (DocsPaVO.Grids.SearchObject project in this._objList)
            {
                // Creazione della nuova riga
                infoFasc = infoFascs.NewRow();

                // Aggiunta del codice registro
                infoFasc["COD_REG"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P7")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta del tipo fascicolo
                infoFasc["TIPO_FASC"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P1")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta del codice fascicolo
                infoFasc["COD_FASC"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P3")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta della descrizione del fascicolo
                infoFasc["DESC_FASC"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P4")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta della data di apertura
                infoFasc["DATA_A"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P5")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta della data di chiusura
                infoFasc["DATA_C"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P6")).FirstOrDefault().SearchObjectFieldValue;

                infoFasc["COLL_FIS"] = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P22")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta della riga alla tabella
                infoFascs.Rows.Add(infoFasc);

            }

            // Restituisce le informazioni sui fascicoli
            return infoFascs;

        }

        private void exportFascToXMLCustom()
        {
            this.addAttToRootNode();

            /* per i fasc usiamo questi dati
            Tipo_fascicolo,
            Codice_fascicolo,
            Descrizione_fascicolo,
            Data_apertura,
            Data_chiusura,
            Collocazione_fisica
            */

            foreach (DocsPaVO.Grids.SearchObject project in this._objList)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                XmlElement codReg = this._xmlDoc.CreateElement("COD_REG");
                codReg.InnerText = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P7")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(codReg);

                XmlElement tipoFasc = this._xmlDoc.CreateElement("TIPO_FASC");
                tipoFasc.InnerText = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P1")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(tipoFasc);

                XmlElement codFasc = this._xmlDoc.CreateElement("COD_FASC");
                codFasc.InnerText = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P3")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(codFasc);

                XmlElement descFasc = this._xmlDoc.CreateElement("DESC_FASC");
                descFasc.InnerText = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P4")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(descFasc);

                XmlElement dataA = this._xmlDoc.CreateElement("DATA_A");
                dataA.InnerText = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P5")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(dataA);

                XmlElement dataC = this._xmlDoc.CreateElement("DATA_C");
                dataC.InnerText = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P6")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(dataC);

                XmlElement collFis = this._xmlDoc.CreateElement("COLL_FIS");
                collFis.InnerText = project.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P22")).FirstOrDefault().SearchObjectFieldValue;
                record.AppendChild(collFis);

                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }

        private void exportFascXLSCustom(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, bool enableChilds, DocsPaVO.fascicolazione.Classificazione classificazione, ArrayList campiSelezionati, String[] idProjectsList, Grid grid, bool gridPersonalization, Field[] visibleFieldsTemplate, bool excel, bool security)
        {
            try
            {
                //Recupero i dati
                DocsPaDB.Query_DocsPAWS.Fascicoli objFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                int nRec = 0;
                int numTotPage = 0;

                List<SearchResultInfo> idProjects = new List<SearchResultInfo>();

                ArrayList fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPagingCustom(infoUtente, classificazione, registro, this._filtri[0], enableUfficioRef, enableProfilazione, enableChilds, out numTotPage, out  nRec, 1, 20, false, out idProjects, null, string.Empty, gridPersonalization, true, visibleFieldsTemplate, idProjectsList, security);


                if (fascicoli == null || fascicoli.Count == 0)
                {
                    this._file = null;
                    return;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Salva e chiudi il file
                if (excel)
                {
                    temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportFascicoli.xls");
                }
                else
                {
                    temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportFascicoli.ods");
                }

                //Creazione stringa XML
                sb = creaXMLFascCustom(fascicoli, campiSelezionati, infoUser, gridPersonalization, grid, excel);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportFascicoli.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Report");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportFascicoli.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    if (excel)
                    {
                        this._file.estensioneFile = "xls";
                        this._file.contentType = "application/vnd.ms-excel";
                    }
                    else
                    {
                        this._file.estensioneFile = "ods";
                        this._file.contentType = "application/vnd.ms-excel";
                    }
                    this._file.name = "ExportFascicoli";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione fascicoli : " + ex.Message);
            }
        }

        private StringBuilder creaXMLFascCustom(ArrayList fascicoli, ArrayList campiSelezionati, InfoUtente userInfo, bool gridPersonalization, Grid grid, bool excel)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += this.CreateFascSheetCustom(fascicoli, campiSelezionati, userInfo, gridPersonalization, grid);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());

            return sb;
        }

        private string CreateFascSheetCustom(ArrayList fascicoli, ArrayList campiSelezionati, InfoUtente userInfo, bool gridPersonalization, Grid grid)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"FASCICOLI\">";
            strXML += "<Table>";
            strXML += this.CreateExcelTableCustom(campiSelezionati, grid);
            // Inserimento del titolo, se presente, e del sommario di export
            strXML += this.InsertTitleAndSummaryExportFld(fascicoli.Count);
            strXML += PopulateFascicoliTableCustom(fascicoli, campiSelezionati, grid, userInfo);

            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }

        private string PopulateFascicoliTableCustom(ArrayList fascicoli, ArrayList selectedFields, Grid grid, InfoUtente userInfo)
        {
            string xmlStr = string.Empty;

            // Aggiunta della colonna
            xmlStr = this.creaColonneDocumenti(selectedFields);

            // Inserimento dei dati sui fascicoli
            xmlStr += this.InsertFascicoliDataCustom(fascicoli, selectedFields, grid, userInfo);

            // Restituzione dell'xml generato
            return xmlStr;
        }

        private string InsertFascicoliDataCustom(ArrayList fascicoli, ArrayList selectedFields, Grid grid, InfoUtente userInfo)
        {
            // XML da restituire
            string toReturn = string.Empty;

            // Per ogni documento...
            foreach (DocsPaVO.Grids.SearchObject fascicolo in fascicoli)
                toReturn += this.InsertFascDataCustomIn(fascicolo, selectedFields, grid, userInfo);

            // Restituzione dell'XML generato
            return toReturn;
        }

        private string InsertFascDataCustomIn(DocsPaVO.Grids.SearchObject prj, ArrayList selectedFields, Grid grid, InfoUtente userInfo)
        {
            // Riga da restituire
            string toReturn = String.Empty;

            // Valore da scrivere nell'XML
            String value;



            // Apertura riga
            toReturn = "<Row>";

            // Inserimento dei valori per i campi
            foreach (CampoSelezionato selectedField in selectedFields)
            {
                // field = grid.Fields.Where(e => e.FieldId.Equals(selectedField.fieldID)).First();

                // Selezione del valore da mostrare
                switch (selectedField.fieldID)
                {
                    //APERTURA
                    case "P5":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //CARTACEO
                    case "P11":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        {
                            value = "Si";
                        }
                        else
                        {
                            value = "No";
                        }
                        break;
                    //CHIUSURA
                    case "P6":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //CODICE
                    case "P3":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //CODICE CLASSIFICA
                    case "P2":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //AOO
                    case "P7":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //DESCRIZIONE
                    case "P4":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    // IN ARCHIVIO
                    case "P12":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        {
                            value = "Si";
                        }
                        else
                        {
                            value = "No";
                        }
                        break;
                    //IN CONSERVAZIONE
                    case "P13":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        {
                            value = "Si";
                        }
                        else
                        {
                            value = "No";
                        }
                        break;
                    //NOTE
                    case "P8":
                        if (IsPresentsNote())
                        {
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ESISTE_NOTA")).FirstOrDefault().SearchObjectFieldValue;
                        }
                        else
                        {
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        }
                        break;
                    // NUMERO
                    case "P14":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //NUMERO MESI IN CONSERVAZIONE
                    case "P15":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    // PRIVATO
                    case "P9":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        {
                            value = "Si";
                        }
                        else
                        {
                            value = "No";
                        }
                        break;
                    // STATO
                    case "P16":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    // TIPO
                    case "P1":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    // TIPOLOGIA
                    case "U1":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //TITOLARIO
                    case "P10":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //Nome e cognome autore
                    case "P17":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //desc ruolo autore
                    case "P18":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //desc uo autore
                    case "P19":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //Data creazione
                    case "P20":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //Collocazione fisica
                    case "P22":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;
                    //CONTATORE
                    case "CONTATORE":
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        break;

                    //OGGETTI CUSTOM
                    default:
                        if (!string.IsNullOrEmpty(prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue))
                        {
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        }
                        else
                        {
                            value = "";
                        }

                        break;
                }

                toReturn += "<Cell ss:StyleID=\"s30\">";
                toReturn += "<Data ss:Type=\"String\"><![CDATA[";
                toReturn += value;
                toReturn += "]]></Data>";
                toReturn += "</Cell>";

            }

            // Chiusura della riga
            toReturn += "</Row>";

            // Restituzione della riga creata
            return toReturn;
        }

        public DocsPaVO.documento.FileDocumento ExportDocInFascCustom(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Folder folder, string codFascicolo, string exportType, string title, DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca, ArrayList campiSelezionati, String[] documentsSystemId, Grid grid, bool gridPersonalization, Field[] visibleFieldsTemplate, DocsPaVO.filtri.FiltroRicerca[][] filtriRIcercaOrdinamento)
        {
            this._folder = folder;
            this._codFasc = codFascicolo;
            //this._exportType = exportType;
            this._title = title;
            this._descAmm = this.getNomeAmministrazione(infoUtente.idPeople);
            this.infoUser = infoUtente;
            this._filtri = filtriRicerca;
            this._filtriOrder = filtriRIcercaOrdinamento;
            switch (exportType)
            {
                case "PDF":
                    this.exportDocInFascPDFCustom(infoUtente, documentsSystemId, gridPersonalization, visibleFieldsTemplate);
                    break;
                case "XLS":
                    this.exportDocInFascXLSCustom(infoUtente, documentsSystemId, gridPersonalization, visibleFieldsTemplate, campiSelezionati, grid, true);
                    break;
                case "ODS":
                    this.exportDocInFascODSCustom(infoUtente, documentsSystemId, gridPersonalization, visibleFieldsTemplate, campiSelezionati, grid, true, codFascicolo);
                    break;
            }

            return this._file;
        }

        private void exportDocInFascPDFCustom(DocsPaVO.utente.InfoUtente infoUtente, String[] selectedDocumentsId, bool gridPersonalization, Field[] visibleFieldsTemplate)
        {
            try
            {
                int numTotPage = 0;
                int nRec = 0;
                List<SearchResultInfo> toSet = new List<SearchResultInfo>();

                this._objList = BusinessLogic.Fascicoli.FolderManager.getDocumentiPagingCustom(infoUtente, this._folder, this._filtri, 1, out numTotPage, out nRec, false, out toSet, gridPersonalization, true, visibleFieldsTemplate, selectedDocumentsId, 20, this._filtriOrder);

                this._rowsList = Convert.ToString(this._objList.Count);

                // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                // iTextSharp per la stampa dei risultati della ricerca
                // ...creazione del report PDF con iTextSharp
                this._file = this.CreaPDFRisRicCustom("DF", "");
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        private DataTable GetDataTableRicDocInFascCustom(string param)
        {
            // Creazione del dataset con i dati sui documenti da inserire nel report
            DataTable infoDocs = new DataTable();
            DataRow infoDoc;

            // Creazione della struttura per infoDocs
            infoDocs.Columns.Add("COD_REG");
            infoDocs.Columns.Add("NUM_PROTOCOLLO");
            infoDocs.Columns.Add("DATA_PROTOCOLLO");
            infoDocs.Columns.Add("OGGETTO");
            infoDocs.Columns.Add("TIPO_DOC");
            infoDocs.Columns.Add("MITT_DEST");
            infoDocs.Columns.Add("COD_FASC");
            infoDocs.Columns.Add("DATA_ANNULLA");
            infoDocs.Columns.Add("IMG");

            // Aggiunta delle righe con le informazioni sui documenti
            foreach (DocsPaVO.Grids.SearchObject documento in this._objList)
            {
                // Creazione della nuova riga
                infoDoc = infoDocs.NewRow();

                // Aggiunta del codice registro
                infoDoc["COD_REG"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D2")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta del numero di protocollo
                string idOrNumProto = string.Empty;
                if (string.IsNullOrEmpty(documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D8")).FirstOrDefault().SearchObjectFieldValue))
                {
                    idOrNumProto = documento.SearchObjectID;
                }
                else
                {
                    idOrNumProto = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                }
                infoDoc["NUM_PROTOCOLLO"] = idOrNumProto;

                // Aggiunta della data di apertura
                infoDoc["DATA_PROTOCOLLO"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta dell'oggetto
                infoDoc["OGGETTO"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D4")).FirstOrDefault().SearchObjectFieldValue;

                //MODIFICATO DA FABIO PER CAMBIO ETICHETTE
                // Aggiunta del tipo di documento
                string tipo = string.Empty;
                tipo = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE")).FirstOrDefault().SearchObjectFieldValue;
                string tempVal = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;
                if (!string.IsNullOrEmpty(tipo))
                {
                    tipo = getLettereProtocolli("ALL");
                }
                else
                {
                    tipo = getLettereProtocolli(tempVal);
                }
                infoDoc["TIPO_DOC"] = tipo;

                // Aggiunta delle informazioni su mittente / destinatario
                infoDoc["MITT_DEST"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D5")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta delle informazioni sul codice fascicolo
                infoDoc["COD_FASC"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D18")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta delle informazioni sulla data di annullamento
                infoDoc["DATA_ANNULLA"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D11")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta dell'informazione indicante se il documento è stato acquisito
                infoDoc["IMG"] = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D23")).FirstOrDefault().SearchObjectFieldValue;

                // Aggiunta della riga alla tabella
                infoDocs.Rows.Add(infoDoc);

            }

            return infoDocs;
        }

        private void exportDocInFascXLSCustom(DocsPaVO.utente.InfoUtente infoUtente, String[] selectedDocumentsId, bool gridPersonalization, Field[] visibleFieldsTemplate, ArrayList campiSelezionati, Grid grid, bool useTraditionaExport)
        {
            try
            {
                int numTotPage = 0;
                int nRec = 0;
                List<SearchResultInfo> toSet = new List<SearchResultInfo>();

                ArrayList documenti = BusinessLogic.Fascicoli.FolderManager.getDocumentiPagingCustom(infoUtente, this._folder, this._filtri, 1, out numTotPage, out nRec, false, out toSet, gridPersonalization, true, visibleFieldsTemplate, selectedDocumentsId, 20, this._filtriOrder);

                if (documenti == null || documenti.Count == 0)
                {
                    this._file = null;
                    return;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLDocGridsCustom(documenti, campiSelezionati, infoUtente, gridPersonalization, grid, true);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportDocumenti.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportDocumenti.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportDocumenti";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione documenti : " + ex.Message);
            }
        }

        protected void exportDocODSCustom(DocsPaVO.utente.InfoUtente infoUtente, String[] documentsSystemId, bool gridPersonalization, Field[] visibleFieldsTemplate, ArrayList campiSelezionati, Grid grid, bool excel)
        {
            try
            {
                int numTotPage = 0;
                int nRec = 0;
                List<SearchResultInfo> toSet = new List<SearchResultInfo>();
                ArrayList documenti = BusinessLogic.Documenti.InfoDocManager.getQueryPagingCustom(infoUtente, this._filtri, 1, 20, true, true, gridPersonalization, visibleFieldsTemplate, documentsSystemId, out numTotPage, out nRec, false, out toSet);

                if (documenti == null || documenti.Count == 0)
                {
                    this._file = null;
                    return;
                }

                DataSet docExp = new DataSet();
                docExp = createOpenOfficeDoc(documenti, campiSelezionati, infoUtente, gridPersonalization, grid, excel);

                string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                serverPath = serverPath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                string userDoc = "\\export_documenti" + infoUser.idPeople + ".ods";
                OpenDocument.OpenDocumentServices open = new OpenDocument.OpenDocumentServices(serverPath + userDoc);
                open.SetTitle("Export documenti - Trovati " + documenti.Count + " documenti");
                open.SetSubtitle(this._title);
                open.SetData(docExp);
                this._file = open.SaveAndExportData();

            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione documenti : " + ex.Message);
            }
        }

        protected DataSet createOpenOfficeDoc(ArrayList documenti, ArrayList campiSelezionati, InfoUtente userInfo, bool gridPersonalization, Grid grid, bool excel)
        {
            DataSet result = new DataSet();
            DataTable dt = new DataTable();
            DataRow column = dt.NewRow();
            String value;
            int i = 0;
            foreach (SearchObject doc in documenti)
            {
                // Riga da restituire
                DataRow toReturn = dt.NewRow();


                // Valore da scrivere nell'XML


                // Inserimento dei valori per i campi
                foreach (CampoSelezionato selectedField in campiSelezionati)
                {
                    // field = grid.Fields.Where(e => e.FieldId.Equals(selectedField.fieldID)).First();

                    // Selezione del valore da mostrare
                    switch (selectedField.fieldID)
                    {
                        //SEGNATURA
                        case "D8":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //REGISTRO
                        case "D2":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //TIPO
                        case "D3":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE")).FirstOrDefault().SearchObjectFieldValue;
                            string tempVal = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value))
                            {
                                value = getLettereProtocolli("ALL");
                            }
                            else
                            {
                                value = getLettereProtocolli(tempVal);
                            }
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //OGGETTO
                        case "D4":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //MITTENTE / DESTINATARIO
                        case "D5":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //MITTENTE
                        case "D6":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //DESTINATARI
                        case "D7":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //DATA
                        case "D9":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //ESITO PUBBLICAZIONE
                        case "D10":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //DATA ANNULLAMENTO
                        case "D11":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //DOCUMENTO
                        case "D1":
                            string numeroDocumento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            string numeroProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                            if (!String.IsNullOrEmpty(numeroProtocollo))
                                value = numeroProtocollo;
                            else
                                value = numeroDocumento;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //NUMERO PROTOCOLLO
                        case "D12":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //AUTORE
                        case "D13":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //DATA ARCHIVIAZIONE
                        case "D14":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //PERSONALE
                        case "D15":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                value = "Si";
                            }
                            else
                            {
                                value = "No";
                            }
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //PRIVATO
                        case "D16":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                value = "Si";
                            }
                            else
                            {
                                value = "No";
                            }
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //TIPOLOGIA
                        case "U1":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //NOTE
                        case "D17":
                            //   if (IsPresentsNote())
                            //   {
                            //       value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ESISTE_NOTA")).FirstOrDefault().SearchObjectFieldValue;
                            //   }
                            //   else
                            //   {
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            //    }
                            //value = (!string.IsNullOrEmpty(doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue) ? "SI" : "NO");
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //COD FASCICOLI
                        case "D18":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //Nome e cognome autore
                        case "D19":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //Ruolo autore
                        case "D20":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //Data arrivo
                        case "D21":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //Stato del documento
                        case "D22":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        case "D23":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        case "CONTATORE":
                            value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //OGGETTI CUSTOM
                        default:
                            if (!string.IsNullOrEmpty(doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue))
                            {
                                value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                                if (value.Equals("#CONTATORE_DI_REPERTORIO#"))
                                {
                                    string dataAnnullamento = string.Empty;
                                    string idDoc = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                                    value = Documenti.DocManager.GetSegnaturaRepertorio(idDoc, infoUser.idAmministrazione, false, out dataAnnullamento);
                                    if (!string.IsNullOrEmpty(dataAnnullamento))
                                        dataAnnullamento = " - " + dataAnnullamento;
                                    value += dataAnnullamento;
                                }
                            }
                            else
                            {
                                value = "";
                            }
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;


                    }

                }
                if (i == 0)
                {
                    dt.Rows.Add(column);
                }

                i++;
                dt.Rows.Add(toReturn);
            }
            result.Tables.Add(dt);
            return result;
        }

        protected void exportFascODSCustom(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, bool enableChilds, DocsPaVO.fascicolazione.Classificazione classificazione, ArrayList campiSelezionati, String[] idProjectsList, Grid grid, bool gridPersonalization, Field[] visibleFieldsTemplate, bool excel, bool security)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Fascicoli objFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                int nRec = 0;
                int numTotPage = 0;

                List<SearchResultInfo> idProjects = new List<SearchResultInfo>();

                ArrayList fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPagingCustom(infoUtente, classificazione, registro, this._filtri[0], enableUfficioRef, enableProfilazione, enableChilds, out numTotPage, out  nRec, 1, 20, false, out idProjects, null, string.Empty, gridPersonalization, true, visibleFieldsTemplate, idProjectsList, security);

                if (fascicoli == null || fascicoli.Count == 0)
                {
                    this._file = null;
                    return;
                }

                DataSet docFasc = new DataSet();
                docFasc = createOpenOfficeFasc(fascicoli, campiSelezionati, infoUtente, gridPersonalization, grid, excel);

                string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                serverPath = serverPath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                string userDoc = "\\export_fascicoli" + infoUser.idPeople + ".ods";
                OpenDocument.OpenDocumentServices open = new OpenDocument.OpenDocumentServices(serverPath + userDoc);
                open.SetTitle("Export fascicoli - Trovati " + fascicoli.Count + " fascicoli");
                open.SetSubtitle(this._title);
                open.SetData(docFasc);
                this._file = open.SaveAndExportData();

            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione fascicoli open office : " + ex.Message);
            }
        }

        protected DataSet createOpenOfficeFasc(ArrayList fascicoli, ArrayList campiSelezionati, InfoUtente userInfo, bool gridPersonalization, Grid grid, bool excel)
        {
            DataSet result = new DataSet();
            DataTable dt = new DataTable();
            DataRow column = dt.NewRow();
            String value;
            int i = 0;

            foreach (SearchObject prj in fascicoli)
            {
                // Riga da restituire
                DataRow toReturn = dt.NewRow();

                // Inserimento dei valori per i campi
                foreach (CampoSelezionato selectedField in campiSelezionati)
                {

                    // Selezione del valore da mostrare
                    switch (selectedField.fieldID)
                    {
                        //APERTURA
                        case "P5":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //CARTACEO
                        case "P11":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                value = "Si";
                            }
                            else
                            {
                                value = "No";
                            }
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //CHIUSURA
                        case "P6":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //CODICE
                        case "P3":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //CODICE CLASSIFICA
                        case "P2":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //AOO
                        case "P7":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //DESCRIZIONE
                        case "P4":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        // IN ARCHIVIO
                        case "P12":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                value = "Si";
                            }
                            else
                            {
                                value = "No";
                            }
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //IN CONSERVAZIONE
                        case "P13":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                value = "Si";
                            }
                            else
                            {
                                value = "No";
                            }
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //NOTE
                        case "P8":
                            if (IsPresentsNote())
                            {
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ESISTE_NOTA")).FirstOrDefault().SearchObjectFieldValue;
                            }
                            else
                            {
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            }
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        // NUMERO
                        case "P14":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //NUMERO MESI IN CONSERVAZIONE
                        case "P15":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        // PRIVATO
                        case "P9":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                value = "Si";
                            }
                            else
                            {
                                value = "No";
                            }
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        // STATO
                        case "P16":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        // TIPO
                        case "P1":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        // TIPOLOGIA
                        case "U1":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //TITOLARIO
                        case "P10":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //Nome e cognome autore
                        case "P17":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //desc ruolo autore
                        case "P18":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //desc uo autore
                        case "P19":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //Data creazione
                        case "P20":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //Collocazione fisica
                        case "P22":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                        //CONTATORE
                        case "CONTATORE":
                            value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;

                        //OGGETTI CUSTOM
                        default:
                            if (!string.IsNullOrEmpty(prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue))
                            {
                                value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                            }
                            else
                            {
                                value = "";
                            }
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.fieldID);
                                column.SetField(selectedField.fieldID, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.fieldID, value);
                            break;
                    }

                }
                if (i == 0)
                {
                    dt.Rows.Add(column);
                }

                i++;
                dt.Rows.Add(toReturn);
            }
            result.Tables.Add(dt);
            return result;
        }

        private void exportDocInFascODSCustom(DocsPaVO.utente.InfoUtente infoUtente, String[] selectedDocumentsId, bool gridPersonalization, Field[] visibleFieldsTemplate, ArrayList campiSelezionati, Grid grid, bool useTraditionaExport, string codFascicolo)
        {
            try
            {
                int numTotPage = 0;
                int nRec = 0;
                List<SearchResultInfo> toSet = new List<SearchResultInfo>();

                ArrayList documenti = BusinessLogic.Fascicoli.FolderManager.getDocumentiPagingCustom(infoUtente, this._folder, this._filtri, 1, out numTotPage, out nRec, false, out toSet, gridPersonalization, true, visibleFieldsTemplate, selectedDocumentsId, 20, this._filtriOrder);

                if (documenti == null || documenti.Count == 0)
                {
                    this._file = null;
                    return;
                }

                DataSet docExp = new DataSet();
                docExp = createOpenOfficeDoc(documenti, campiSelezionati, infoUtente, gridPersonalization, grid, gridPersonalization);

                string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                serverPath = serverPath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                string userDoc = "\\export_documenti" + infoUser.idPeople + ".ods";
                OpenDocument.OpenDocumentServices open = new OpenDocument.OpenDocumentServices(serverPath + userDoc);
                open.SetTitle("Export documenti nel fascicolo " + codFascicolo + " - Trovati " + documenti.Count + " documenti");
                open.SetSubtitle(this._title);
                open.SetData(docExp);
                this._file = open.SaveAndExportData();
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione documenti in fascicolo: " + ex.Message);
            }
        }

        private void exportDocRicFromModelCustom(DocsPaVO.utente.InfoUtente infoUtente, String[] documentsSystemId, bool gridPersonalization, Field[] visibleFieldsTemplate, ArrayList campiSelezionati, Grid grid)
        {
            DocsPaVO.utente.Utente utente = Utenti.UserManager.getUtenteById(infoUtente.idPeople);
            //C:\\Documents and Settings\\ricciutife\\Desktop\\provaExport.xls

            try
            {
                int numTotPage = 0;
                int nRec = 0;
                List<SearchResultInfo> toSet = new List<SearchResultInfo>();
                ArrayList documenti = BusinessLogic.Documenti.InfoDocManager.getQueryPagingCustom(infoUtente, this._filtri, 1, 20, true, true, gridPersonalization, visibleFieldsTemplate, documentsSystemId, out numTotPage, out nRec, false, out toSet);

                string idTemplate = ((DocsPaVO.Grids.SearchObject)documenti[0]).SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_TIPO_ATTO")).FirstOrDefault().SearchObjectFieldValue;

                DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateDettagli(idTemplate);
                _pathModelloExc = template.PATH_MODELLO_EXCEL;

                if (documenti == null || documenti.Count == 0)
                {
                    this._file = null;
                    return;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLModCustom(documenti, campiSelezionati, infoUtente, gridPersonalization, grid);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportDocumenti.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportDocumenti.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportDocumenti";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione documenti : " + ex.Message);
            }

        }

        private StringBuilder creaXMLModCustom(ArrayList documenti, ArrayList campiSelezionati, InfoUtente userInfo, bool gridPersonalization, Grid grid)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXMLModel();

            //  strXML += this.CreateDocumentSheetCustom(documenti, campiSelezionati, userInfo, gridPersonalization, grid);
            //Tabella campi scelti
            strXML += sheetModelloCustom(documenti, campiSelezionati, userInfo, gridPersonalization, grid);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());

            return sb;
        }

        private string sheetModelloCustom(ArrayList documenti, ArrayList campiSelezionati, InfoUtente userInfo, bool gridPersonalization, Grid grid)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"DOCUMENTI\">";
            strXML += "<Table>";
            //strXML += creaTabellaDocumenti(campiSelezionati);
            //Aggiungo intestazione del modello
            DocsPaVO.utente.Utente utente = Utenti.UserManager.getUtenteById(userInfo.idPeople);
            strXML += addHeaderFromModel(documenti, utente);

            strXML += "<Row></Row>";

            strXML += PopulateDocumentTableCustom(documenti, campiSelezionati, grid, userInfo);

            strXML += "</Table>";
            strXML += workSheetOptionsXMLModel();
            strXML += "</Worksheet>";
            return strXML;
        }

        /// <summary>
        /// Ultima nota oppure si/no
        /// </summary>
        /// <returns></returns>
        public bool IsPresentsNote()
        {
            string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_IS_PRESENT_NOTE");

            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
            {
                return value == "1";
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reperisce i destinatari di una trasmissione
        /// </summary>
        /// <param name="trasm">oggetto trasmissione</param>
        /// <returns>lista dei destinatari concatenati tramite virgola</returns>
        private string getDestinatariTrasmLite(DocsPaVO.trasmissione.Trasmissione trasm, Dictionary<string, string> destTrasm)
        {
            string destinatari = string.Empty;

            if (trasm.trasmissioniSingole != null)
            {
                foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in trasm.trasmissioniSingole)
                {
                    if (destTrasm.ContainsKey(ts.systemId))
                    {
                        destinatari = string.Empty;
                        destinatari = destTrasm[ts.systemId];
                    }
                }
            }

            return destinatari;
        }

        /// <summary>
        /// Reperisce i destinatari di una trasmissione singola
        /// </summary>
        /// <param name="trasm">oggetto trasmissione</param>
        /// <returns>lista dei destinatari concatenati tramite virgola</returns>
        private string getDestinatariTrasmSingolaLite(DocsPaVO.trasmissione.TrasmissioneSingola trasm, Dictionary<string, string> destTrasm)
        {
            string destinatari = string.Empty;

            if (destTrasm.ContainsKey(trasm.systemId))
            {
                destinatari = string.Empty;
                destinatari = destTrasm[trasm.systemId];
            }

            return destinatari;
        }

        public DocsPaVO.documento.FileDocumento ExportReportPregresso(DocsPaVO.Import.Pregressi.ReportPregressi report, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {

                if (report == null || report.itemPregressi == null || report.itemPregressi.Count == 0)
                {
                    _file = null;
                    return null;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLPregressi(report, infoUtente);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportPregressi.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportPregressi.xls");


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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportDocumenti";
                    this._file.contentType = "application/vnd.ms-excel";

                }

                File.Delete(temporaryXSLFilePath);

                return this._file;
            }
            catch (Exception ex)
            {
                this._file = null;
                return this._file;
                logger.Debug("Errore esportazione pregressi : " + ex.Message);
            }
        }

        private StringBuilder creaXMLPregressi(DocsPaVO.Import.Pregressi.ReportPregressi report, DocsPaVO.utente.InfoUtente infoUtente)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML

            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += this.CreateDocumentSheetPregresso(report, infoUtente);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());

            return sb;
        }

        private string CreateDocumentSheetPregresso(DocsPaVO.Import.Pregressi.ReportPregressi report, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"PREGRESSI\">";
            strXML += "<Table>";
            strXML += this.CreateExcelTableExportPregressi();
            strXML += PopulateDocumentTableExportPregressi(report, infoUtente);

            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }

        private string CreateExcelTableExportPregressi()
        {
            string xmlStr = String.Empty;
            for (int i = 0; i < 12; i++)
            {
                xmlStr += "<Column ss:StyleID=\"s63\" ss:Width=\"100\"/>";
            }
            // Restituzione dell'XML generato
            return xmlStr;
        }

        private string PopulateDocumentTableExportPregressi(DocsPaVO.Import.Pregressi.ReportPregressi report, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string xmlStr = string.Empty;

            // Aggiunta della colonna
            xmlStr = this.creaColonneExportPregressi();

            // Inserimento dei dati sui documenti
            xmlStr += this.InsertDocumentsExportPregressi(report, infoUtente);

            // Restituzione dell'xml generato
            return xmlStr;
        }

        private string creaColonneExportPregressi()
        {
            string strXML = string.Empty;
            strXML += "<Row>";

            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Data";
            strXML += "</Data>";
            strXML += "</Cell>";

            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Esito";
            strXML += "</Data>";
            strXML += "</Cell>";

            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Id Documento";
            strXML += "</Data>";
            strXML += "</Cell>";

            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Numero Protocollo/Id Vecchio Documento";
            strXML += "</Data>";
            strXML += "</Cell>";

            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Registro";
            strXML += "</Data>";
            strXML += "</Cell>";

            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Proprietario";
            strXML += "</Data>";
            strXML += "</Cell>";

            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Tipo Operazione";
            strXML += "</Data>";
            strXML += "</Cell>";

            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">N° Allegati";
            strXML += "</Data>";
            strXML += "</Cell>";

            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Errore";
            strXML += "</Data>";
            strXML += "</Cell>";

            strXML += "</Row>";
            return strXML;
        }

        private string InsertDocumentsExportPregressi(DocsPaVO.Import.Pregressi.ReportPregressi report, DocsPaVO.utente.InfoUtente infoUtente)
        {
            // XML da restituire
            string toReturn = string.Empty;

            Dictionary<string, DocsPaVO.utente.Utente> utenti = new Dictionary<string, DocsPaVO.utente.Utente>();
            Dictionary<string, DocsPaVO.utente.Ruolo> ruoli = new Dictionary<string, DocsPaVO.utente.Ruolo>();
            Dictionary<string, DocsPaVO.utente.Registro> registri = new Dictionary<string, Registro>();

            // Per ogni documento...
            foreach (DocsPaVO.Import.Pregressi.ItemReportPregressi item in report.itemPregressi)
            {
                toReturn += this.InsertDocumentDataExport(item, ref utenti, ref ruoli, ref registri);
            }

            // Restituzione dell'XML generato
            return toReturn;
        }

        private string InsertDocumentDataExport(DocsPaVO.Import.Pregressi.ItemReportPregressi item, ref Dictionary<string, DocsPaVO.utente.Utente> utenti, ref Dictionary<string, DocsPaVO.utente.Ruolo> ruoli, ref Dictionary<string, DocsPaVO.utente.Registro> registri)
        {
            // Riga da restituire
            string toReturn = String.Empty;

            // Apertura riga
            toReturn = "<Row>";

            toReturn += "<Cell ss:StyleID=\"s30\">";
            toReturn += "<Data ss:Type=\"String\"><![CDATA[";
            toReturn += item.data;
            toReturn += "]]></Data>";
            toReturn += "</Cell>";

            toReturn += "<Cell ss:StyleID=\"s30\">";
            toReturn += "<Data ss:Type=\"String\"><![CDATA[";
            toReturn += item.esito;
            toReturn += "]]></Data>";
            toReturn += "</Cell>";

            toReturn += "<Cell ss:StyleID=\"s30\">";
            toReturn += "<Data ss:Type=\"String\"><![CDATA[";
            toReturn += item.idDocumento;
            toReturn += "]]></Data>";
            toReturn += "</Cell>";

            toReturn += "<Cell ss:StyleID=\"s30\">";
            toReturn += "<Data ss:Type=\"String\"><![CDATA[";
            toReturn += item.idNumProtocolloExcel;
            toReturn += "]]></Data>";
            toReturn += "</Cell>";

            toReturn += "<Cell ss:StyleID=\"s30\">";
            toReturn += "<Data ss:Type=\"String\"><![CDATA[";
            if (!string.IsNullOrEmpty(item.idRegistro))
            {
                DocsPaVO.utente.Registro registro = new Registro();
                if (registri.ContainsKey(item.idRegistro))
                {
                    registro = registri[item.idRegistro];
                }
                else
                {
                    registro = BusinessLogic.Utenti.RegistriManager.getRegistro(item.idRegistro);
                    registri.Add(registro.systemId, registro);
                }
                toReturn += registro.codRegistro;

            }
            else
            {
                toReturn += "";
            }
            toReturn += "]]></Data>";
            toReturn += "</Cell>";

            toReturn += "<Cell ss:StyleID=\"s30\">";
            toReturn += "<Data ss:Type=\"String\"><![CDATA[";
            DocsPaVO.utente.Utente utente = new Utente();
            DocsPaVO.utente.Ruolo ruolo = new Ruolo();
            string nome = "";
            string descrizioneRuolo = "";
            if (!string.IsNullOrEmpty(item.idUtente))
            {
                if (utenti.ContainsKey(item.idUtente))
                {
                    utente = utenti[item.idUtente];
                }
                else
                {
                    utente = BusinessLogic.Utenti.UserManager.getUtenteById(item.idUtente);
                    utenti.Add(utente.idPeople, utente);
                }
                nome = utente.nome + " " + utente.cognome;
            }

            if (!string.IsNullOrEmpty(item.idRuolo))
            {
                if (ruoli.ContainsKey(item.idRuolo))
                {
                    ruolo = ruoli[item.idRuolo];
                }
                else
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(item.idRuolo);
                    ruoli.Add(ruolo.idGruppo, ruolo);
                }
                descrizioneRuolo = " (" + ruolo.descrizione + ")";
            }

            toReturn += nome + descrizioneRuolo;
            toReturn += "]]></Data>";
            toReturn += "</Cell>";

            toReturn += "<Cell ss:StyleID=\"s30\">";
            toReturn += "<Data ss:Type=\"String\"><![CDATA[";
            toReturn += item.tipoOperazione;
            toReturn += "]]></Data>";
            toReturn += "</Cell>";

            toReturn += "<Cell ss:StyleID=\"s30\">";
            toReturn += "<Data ss:Type=\"String\"><![CDATA[";
            if (item.Allegati != null)
            {
                toReturn += item.Allegati.Count;
            }
            else
            {
                toReturn += "0";
            }

            toReturn += "]]></Data>";
            toReturn += "</Cell>";

            toReturn += "<Cell ss:StyleID=\"s30\">";
            toReturn += "<Data ss:Type=\"String\"><![CDATA[";
            toReturn += item.errore;
            toReturn += "]]></Data>";
            toReturn += "</Cell>";



            // Chiusura della riga
            toReturn += "</Row>";

            // Restituzione della riga creata
            return toReturn;
        }

        //Andrea - Export Excel Report in Errore
        public DocsPaVO.documento.FileDocumento ExportReportInError(List<DocsPaVO.Import.Pregressi.ItemReportPregressi> repInErr)
        {
            try
            {

                if (repInErr == null || repInErr.Count == 0)
                {
                    _file = null;
                    return null;
                }

                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = creaXMLReportInError(repInErr);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportReportInError.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportReportInError.xls");


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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    //Andrea De Marco - Modificato per richiesta Cliente: Nome del file = EsitoControllo_NomeFileImportato
                    //this._file.name = "ExportReportInError";
                    this._file.name = "EsitoControllo_";
                    this._file.contentType = "application/vnd.ms-excel";

                }

                File.Delete(temporaryXSLFilePath);

                return this._file;
            }
            catch (Exception ex)
            {
                this._file = null;
                return this._file;
                logger.Debug("Errore esportazione Report pregressi in errore : " + ex.Message);
            }
        }

        private StringBuilder creaXMLReportInError(List<DocsPaVO.Import.Pregressi.ItemReportPregressi> repInErr)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML

            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            //Fogli Excel
            strXML += this.CreateDocumentSheetReportInErrore(repInErr);

            strXML += "</Workbook>";

            sb.Append(strXML.ToString());

            return sb;
        }

        private string CreateDocumentSheetReportInErrore(List<DocsPaVO.Import.Pregressi.ItemReportPregressi> repInErr)
        {
            string strXML = string.Empty;

            strXML = "<Worksheet ss:Name=\"REPORTINERRORE\">";
            strXML += "<Table>";
            strXML += this.CreateExcelTableExportReportInErrore();
            strXML += PopulateDocumentTableExportReportInErrore(repInErr);

            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            return strXML;
        }

        private string CreateExcelTableExportReportInErrore()
        {
            string xmlStr = String.Empty;
            for (int i = 0; i < 2; i++)
            {
                xmlStr += "<Column ss:StyleID=\"s63\" ss:Width=\"100\"/>";
            }
            // Restituzione dell'XML generato
            return xmlStr;
        }

        private string PopulateDocumentTableExportReportInErrore(List<DocsPaVO.Import.Pregressi.ItemReportPregressi> repInErr)
        {
            string xmlStr = string.Empty;

            // Aggiunta della colonna
            xmlStr = this.creaColonneExportReportInErrore();

            // Inserimento dei dati sui documenti
            xmlStr += this.InsertDocumentsExportReportInErrore(repInErr);

            // Restituzione dell'xml generato
            return xmlStr;
        }

        private string creaColonneExportReportInErrore()
        {
            string strXML = string.Empty;
            strXML += "<Row>";

            strXML += "<Cell ss:StyleID=\"s70\">";
            strXML += "<Data ss:Type=\"String\">Errore";
            strXML += "</Data>";
            strXML += "</Cell>";

            //Richiesta Cliente - Al posto della parola Riga, Inserire la parola Ordinale.
            strXML += "<Cell ss:StyleID=\"s70\">";
            //strXML += "<Data ss:Type=\"String\">Riga";
            strXML += "<Data ss:Type=\"String\">Ordinale";
            strXML += "</Data>";
            strXML += "</Cell>";

            strXML += "</Row>";
            return strXML;
        }

        private string InsertDocumentsExportReportInErrore(List<DocsPaVO.Import.Pregressi.ItemReportPregressi> repInErr)
        {
            // XML da restituire
            string toReturn = string.Empty;

            // Per ogni Errore...
            foreach (DocsPaVO.Import.Pregressi.ItemReportPregressi item in repInErr)
            {
                toReturn += this.InsertDocumentDataExportReportInErrore(item);
            }

            // Restituzione dell'XML generato
            return toReturn;
        }

        private string InsertDocumentDataExportReportInErrore(DocsPaVO.Import.Pregressi.ItemReportPregressi item)
        {
            // Riga da restituire
            string toReturn = String.Empty;

            // Apertura riga
            toReturn = "<Row>";

            toReturn += "<Cell ss:StyleID=\"s30\">";
            toReturn += "<Data ss:Type=\"String\"><![CDATA[";
            toReturn += item.errore.Replace("|", " ");
            toReturn += "]]></Data>";
            toReturn += "</Cell>";

            toReturn += "<Cell ss:StyleID=\"s30\">";
            toReturn += "<Data ss:Type=\"String\"><![CDATA[";
            toReturn += item.ordinale;
            toReturn += "]]></Data>";
            toReturn += "</Cell>";

            // Chiusura della riga
            toReturn += "</Row>";

            // Restituzione della riga creata
            return toReturn;
        }

        #region Notification Center

        public DocsPaVO.documento.FileDocumento ExportNotificationCenter(DocsPaVO.utente.InfoUtente infoUtente, string exportType, string title, ArrayList campiSelezionati, List<DocsPaVO.Notification.Notification> ListNotify)
        {
            this._exportType = exportType;
            this._title = title;
            this._descAmm = this.getNomeAmministrazione(infoUtente.idPeople);

            switch (exportType)
            {
                case "PDF":
                    this.ExportNotificationCenterPDF(infoUtente, ListNotify);
                    break;
                case "XLS":
                    this.ExportNotificationCenterXLS(infoUtente, campiSelezionati, ListNotify);
                    break;
                case "ODS":
                    this.ExportNotificationCenterODS(infoUtente, campiSelezionati, ListNotify);
                    break;
            }

            return _file;
        }

        private void ExportNotificationCenterPDF(DocsPaVO.utente.InfoUtente infoUtente, List<DocsPaVO.Notification.Notification> ListNotify)
        {
            try
            {
                if (ListNotify.Count > 0)
                {
                    this._rowsList = Convert.ToString(ListNotify.Count);

                    // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                    // iTextSharp per la stampa dei risultati della ricerca
                    // Il nome del file di template
                    string templateFileName = "XMLStampaRisRicNotificationCenter.xml";
                    // Il datatable utilizzato per creare la tabella con i dati da  visualizzare
                    DataTable infoObjs = this.DataTableNotificationCenter(ListNotify);
                    // 2. Creazione dell'oggetto di generazione PDF
                    StampaPDF.StampaRisRicerca report = new StampaPDF.StampaRisRicerca();
                    // 3. Restituzione del file documento PDF con i dati dell'export
                    this._file = report.GetFileDocumento(templateFileName, this._title, this._descAmm, ListNotify.Count.ToString(), infoObjs);
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        private void ExportNotificationCenterODS(DocsPaVO.utente.InfoUtente infoUtente, ArrayList campiSelezionati, List<DocsPaVO.Notification.Notification> ListNotify)
        {
            DataSet docExp = new DataSet();
            docExp = createOpenOfficeNotificatioCenter(ListNotify, campiSelezionati, infoUtente);

            string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
            serverPath = serverPath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
            string userDoc = "\\export_notifiche" + infoUser.idPeople + ".ods";
            OpenDocument.OpenDocumentServices open = new OpenDocument.OpenDocumentServices(serverPath + userDoc);
            open.SetTitle("Export notifiche - Trovate " + ListNotify.Count + " notifiche");
            open.SetSubtitle(this._title);
            open.SetData(docExp);
            this._file = open.SaveAndExportData();
        }

        protected DataSet createOpenOfficeNotificatioCenter(List<DocsPaVO.Notification.Notification> ListNotify, ArrayList campiSelezionati, InfoUtente userInfo)
        {
            DataSet result = new DataSet();
            DataTable dt = new DataTable();
            DataRow column = dt.NewRow();
            String value;
            int i = 0;
            foreach (DocsPaVO.Notification.Notification notify in ListNotify)
            {
                // Riga da restituire
                DataRow toReturn = dt.NewRow();


                // Valore da scrivere nell'XML


                // Inserimento dei valori per i campi
                foreach (CampoSelezionato selectedField in campiSelezionati)
                {
                    // field = grid.Fields.Where(e => e.FieldId.Equals(selectedField.fieldID)).First();

                    // Selezione del valore da mostrare
                    switch (selectedField.nomeCampo)
                    {
                        //Evento
                        case "Evento":
                            value = MappingLabelNotify(notify.TYPE_EVENT);
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                        //Autore
                        case "Autore":
                            value = notify.PRODUCER;
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                        //Data Evento
                        case "Data Evento":
                            value = notify.DTA_EVENT.ToString("dd/MM/yyyy HH:mm:ss");
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                        //Doc/Fasc
                        case "Doc/Fasc":
                            value = DomainObjectNotify(notify);
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                        //Oggetto/Descrizione
                        case "Oggetto/Descrizione":
                            value = (notify.ITEMS.ITEM3.Length > 50 ?
                                    FWithoutHtml(notify.ITEMS.ITEM3).Substring(0, 50) + "..." : FWithoutHtml(notify.ITEMS.ITEM3));
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                        //Dettaglio
                        case "Dettaglio":
                            value = DetailsNotify(notify, " ");
                            if (i == 0)
                            {
                                dt.Columns.Add(selectedField.nomeCampo);
                                column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                            }
                            toReturn.SetField(selectedField.nomeCampo, value);
                            break;
                    }

                }
                if (i == 0)
                {
                    dt.Rows.Add(column);
                }

                i++;
                dt.Rows.Add(toReturn);
            }
            result.Tables.Add(dt);
            return result;
        }

        private void ExportNotificationCenterXLS(DocsPaVO.utente.InfoUtente infoUtente, ArrayList campiSelezionati, List<DocsPaVO.Notification.Notification> ListNotify)
        {
            try
            {
                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = this.CreateXLSNotificationCenter(ListNotify, campiSelezionati, infoUtente.idPeople, infoUtente.idGruppo);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportNotificationCenter.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportNotificationCenter.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportNotificationCenter";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione centro notifiche: " + ex.Message);
            }
        }

        #region Export Pdf

        private DataTable DataTableNotificationCenter(List<DocsPaVO.Notification.Notification> ListNotify)
        {
            const string EVENT = "EVENTO";
            const string AUTHOR = "AUTORE";
            const string DTA_EVENT = "DATA EVENTO";
            const string OBJECT = "DOC/FASC";
            const string DESCRIPTION = "OGGETTO/DESCRIZIONE";
            const string DETAILS = "DETTAGLIO";

            // Creazione del dataset con i dati sul centro notifiche da inserire nel report
            DataTable notificationCenter = new DataTable();
            DataRow notificationCenterItem;

            // Creazione della struttura per il centro notifiche
            notificationCenter.Columns.Add(EVENT);
            notificationCenter.Columns.Add(AUTHOR);
            notificationCenter.Columns.Add(DTA_EVENT);
            notificationCenter.Columns.Add(OBJECT);
            notificationCenter.Columns.Add(DESCRIPTION);
            notificationCenter.Columns.Add(DETAILS);


            // Build delle singole notifiche da aggiungere al report
            foreach (DocsPaVO.Notification.Notification notify in ListNotify)
            {
                // Creazione di una nuova riga
                notificationCenterItem = notificationCenter.NewRow();

                // Evento
                notificationCenterItem[EVENT] = MappingLabelNotify(notify.TYPE_EVENT);
                //Autore evento
                notificationCenterItem[AUTHOR] = notify.PRODUCER;
                //Data evento
                notificationCenterItem[DTA_EVENT] = notify.DTA_EVENT.ToString("dd/MM/yyyy HH:mm:ss");
                //oggetto di dominio
                notificationCenterItem[OBJECT] = DomainObjectNotify(notify);
                //descrizione dell'oggetto di dominio al quale fa riferimento la notifica
                if (notify.ITEMS.ITEM3.Length > 50)
                {
                    notificationCenterItem[DESCRIPTION] = FWithoutHtml(notify.ITEMS.ITEM3).Substring(0, 50) + "...";
                }
                else
                {
                    notificationCenterItem[DESCRIPTION] = FWithoutHtml(notify.ITEMS.ITEM3);
                }
                //dettagli sulla notifica
                notificationCenterItem[DETAILS] = DetailsNotify(notify, "\n");

                // Aggiunta della riga compilata
                notificationCenter.Rows.Add(notificationCenterItem);
            }

            // Return data set
            return notificationCenter;

        }

        private void ExportNotificationCenterToXML(List<DocsPaVO.Notification.Notification> ListNotify)
        {
            this.addAttToRootNode();

            /* per il centro notifiche le colonne  di interesse sono:
             * EVENTO, AUTORE, DATA EVENTO, DOC/FASC, OGGETTO/DESCRIZIONE, DETTAGLIO.
            */

            foreach (DocsPaVO.Notification.Notification notify in ListNotify)
            {
                XmlElement record = this._xmlDoc.CreateElement("RECORD");

                XmlElement typeEvent = this._xmlDoc.CreateElement("EVENTO");
                typeEvent.InnerText = MappingLabelNotify(notify.TYPE_EVENT);
                record.AppendChild(typeEvent);

                XmlElement author = this._xmlDoc.CreateElement("AUTORE");
                author.InnerText = notify.PRODUCER;
                record.AppendChild(author);

                XmlElement dtaEvent = this._xmlDoc.CreateElement("DATA_EVENTO");
                dtaEvent.InnerText = notify.DTA_EVENT.ToString("dd/MM/yyyy HH:mm:ss");
                record.AppendChild(dtaEvent);

                XmlElement domainObject = this._xmlDoc.CreateElement("DOC_FASC");
                domainObject.InnerText = DomainObjectNotify(notify);
                record.AppendChild(domainObject);

                XmlElement description = this._xmlDoc.CreateElement("OGG_DESC");
                description.InnerText = FWithoutHtml(notify.ITEMS.ITEM3);
                record.AppendChild(description);

                //dettagli sulla notifica
                XmlElement details = this._xmlDoc.CreateElement("DETTAGLIO");
                details.InnerText = DetailsNotify(notify, "\n");
                record.AppendChild(details);

                this._xmlDoc.DocumentElement.AppendChild(record);
            }
        }

        #endregion

        #region Export XLS

        private StringBuilder CreateXLSNotificationCenter(List<DocsPaVO.Notification.Notification> ListNotify, ArrayList campiSelezionati, string idPeople, string idGruppo)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            strXML += "<Worksheet ss:Name=\"Notifiche\">";
            strXML += "<Table>";
            //creo la tabella per le notifiche
            foreach (DocsPaVO.ExportData.CampoSelezionato field in campiSelezionati)
            {
                switch (field.nomeCampo)
                {
                    case "Evento":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"130\"/>";
                        break;
                    case "Autore":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"130\"/>";
                        break;
                    case "Data Evento":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"130\"/>";
                        break;
                    case "Doc/Fasc":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"200\"/>";
                        break;
                    case "Oggetto/Descrizione:":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"130\"/>";
                        break;
                    case "Dettaglio":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"200\"/>";
                        break;
                    default:
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"100\"/>";
                        break;
                }
            }

            strXML += InsertDataNotificationCenterXML(ListNotify, campiSelezionati, idPeople, idGruppo);
            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            strXML += "</Workbook>";

            sb.Append(strXML.ToString());
            return sb;
        }

        private string InsertDataNotificationCenterXML(List<DocsPaVO.Notification.Notification> ListNotify, ArrayList campiSelezionati, string idPeople, string idGruppo)
        {
            string strXML = string.Empty;
            strXML = CreateColumnsNotify(campiSelezionati);
            strXML += InsertDataNotificationCenter(ListNotify, campiSelezionati, idPeople, idGruppo);
            return strXML;
        }

        private string CreateColumnsNotify(ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            strXML += "<Row>";
            foreach (DocsPaVO.ExportData.CampoSelezionato field in campiSelezionati)
            {
                strXML += "<Cell ss:StyleID=\"s70\">";
                strXML += "<Data ss:Type=\"String\">" + field.nomeCampo.ToString().Replace(":", string.Empty);
                strXML += "</Data>";
                strXML += "</Cell>";
            }

            strXML += "</Row>";
            return strXML;
        }

        private string InsertDataNotificationCenter(List<DocsPaVO.Notification.Notification> ListNotify, ArrayList campiSelezionati, string idPeople, string idGruppo)
        {
            string righe = string.Empty;

            if (ListNotify.Count != 0)
            {
                foreach (DocsPaVO.Notification.Notification notify in ListNotify)
                {
                    string riga = string.Empty;
                    riga = "<Row>";

                    //Inserimento Campi Standard
                    foreach (DocsPaVO.ExportData.CampoSelezionato field in campiSelezionati)
                    {
                        switch (field.nomeCampo)
                        {
                            case "Evento":
                                riga += "<Cell>";
                                riga += "<Data ss:Type=\"String\">" + MappingLabelNotify(notify.TYPE_EVENT);
                                riga += "</Data>";
                                riga += "</Cell>";
                                break;

                            case "Autore":
                                riga += "<Cell>";
                                riga += "<Data ss:Type=\"String\">" + notify.PRODUCER;
                                riga += "</Data>";
                                riga += "</Cell>";
                                break;

                            case "Data Evento":
                                riga += "<Cell>";
                                riga += "<Data ss:Type=\"String\">" + notify.DTA_EVENT.ToString("dd/MM/yyyy HH:mm:ss");
                                riga += "</Data>";
                                riga += "</Cell>";
                                break;

                            case "Doc/Fasc":
                                riga += "<Cell>";
                                riga += "<Data ss:Type=\"String\">" + DomainObjectNotify(notify);
                                riga += "</Data>";
                                riga += "</Cell>";
                                break;

                            case "Oggetto/Descrizione:":
                                riga += "<Cell>";
                                riga += "<Data ss:Type=\"String\">" + (notify.ITEMS.ITEM3.Length > 50 ?
                                    FWithoutHtml(notify.ITEMS.ITEM3).Substring(0, 50) + "..." : FWithoutHtml(notify.ITEMS.ITEM3));
                                riga += "</Data>";
                                riga += "</Cell>";
                                break;

                            case "Dettaglio":

                                riga += "<Cell>";
                                riga += "<Data ss:Type=\"String\">" + DetailsNotify(notify, "&#10");
                                riga += "</Data>";
                                riga += "</Cell>";
                                break;
                        }
                    }

                    riga += "</Row>";
                    righe += riga;
                }
            }
            return righe;
        }

        #endregion

        /// <summary>
        /// Data una label restituisce la stringa corrispondente
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        private string MappingLabelNotify(string label)
        {
            Hashtable hash = new Hashtable();
            hash.Add("DOCUMENTOCONVERSIONEPDF", "Conversione PDF");
            hash.Add("ACCEPT_TRASM_FOLDER", "Accettazione Trasmissione F.");
            hash.Add("ACCEPT_TRASM_DOCUMENT", "Accettazione Trasmissione D.");
            hash.Add("REJECT_TRASM_FOLDER", "Rifiuto Trasmissione F.");
            hash.Add("REJECT_TRASM_DOCUMENT", "Rifiuto Trasmissione D.");
            hash.Add("CHECK_TRASM_FOLDER", "Visto Trasmissione F.");
            hash.Add("CHECK_TRASM_DOCUMENT", "Visto Trasmissione D.");
            hash.Add("MODIFIED_OBJECT_PROTO", "Mod. oggetto protocollo");
            hash.Add("MODIFIED_OBJECT_DOC", "Modificato oggetto del Documento");
            hash.Add("DOC_CAMBIO_STATO", "Cambio stato documento");
            hash.Add("NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY", "Ricevuta di mancata consegna IS");
            hash.Add("ANNULLA_PROTO", "Annullamento protocollo");
            hash.Add("RECORD_PREDISPOSED", "Protocollazione predisposto");
            hash.Add("NO_DELIVERY_SEND_PEC", "Ricezione mancata consegna/con errori.");
            hash.Add("EXCEPTION_INTEROPERABILITY_PEC", "Eccezione interoperabilità PEC");
            hash.Add("lblSender", "Mittente: ");
            hash.Add("lblObjectDescription", "Oggetto/Descrizione: ");
            hash.Add("lblDta_notify", "Data Notifica: ");
            hash.Add("lblInterno", "I");
            hash.Add("lblArrivo", "A");
            hash.Add("lblPartenza", "P");
            hash.Add("lblStampaReg", "Stampa Registro");
            hash.Add("lblGrigio", "NP");
            hash.Add("lblRepertorio", "Rep: ");
            hash.Add("lblGeneralNote", "Nota generale: ");
            hash.Add("lblIndividualNote", "Nota individuale: ");
            hash.Add("lblDocType", "Tipologia Documento: ");
            hash.Add("lblRejectNote", "Nota di Rifiuto: ");
            hash.Add("lblAcceptNote", "Nota di Accettazione: ");
            hash.Add("lblChangeStateDoc", "Stato documento modificato in: ");
            hash.Add("lblFascType", "Tipologia Fascicolo: ");
            hash.Add("lblDetailReceivedIS", "Dettaglio:  ");
            hash.Add("lblSendRecipientIS", "Destinatario spedizione: ");
            hash.Add("lblExpiration", "Data scadenza: ");
            hash.Add("lblIdNotification", "Id notifica: ");
            hash.Add("lblDtaAbortRecord", "Data Annullamento: ");
            hash.Add("lblDescAbortRecord", "Descrizione Annullamento: ");
            hash.Add("CREATED_FILE_ZIP_INSTANCE_ACCESS", "Preparazione download istanza - Con successo ");
            hash.Add("FAILED_CREATING_FILE_ZIP_INSTANCE_ACCESS", "Preparazione download istanza - Con errori ");
            hash.Add("lblResultCreationFileInstance", "Esito creazione File Zip: ");
            hash.Add("INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE", "Interrotto processo dal titolare Doc ");
            hash.Add("INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE", "Interrotto processo dal titolare	All ");
            hash.Add("INTERROTTO_PROCESSO_ALLEGATO_DAL_PROPONENTE", "Interrotto processo dal proponente All ");
            hash.Add("INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE", "Interrotto processo dal proponente	Doc ");
            hash.Add("CONCLUSIONE_PROCESSO_LF_ALLEGATO", "Conclusione processo All ");
            hash.Add("CONCLUSIONE_PROCESSO_LF_DOCUMENTO", "Conclusione processo Doc ");
            hash.Add("lblDescriptionProcess", "Nome processo: ");
            hash.Add("lblNotesStartup", "Note di avvio: ");
            hash.Add("INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN", "Interrotto processo da Amministratore Doc ");
            hash.Add("INTERROTTO_PROCESSO_ALLEGATO_DA_ADMIN", "Interrotto processo da Amministratore All ");
            hash.Add("lblReasonRejection", "Motivo interruzione: ");
            hash.Add("lblDescriptionAction", "Descrizione evento: ");
            hash.Add("lblSuccessfulConversionDoc", "Esito conversione: ");
            const string TRASM_DOC = "TRASM_DOC_";
            const string TRASM_FOLDER = "TRASM_FOLDER_";

            if (hash.ContainsKey(label))
            {
                return (string)hash[label];
            }
            else if (label.Contains(TRASM_DOC) || label.Contains(TRASM_FOLDER))
            {
                return label.Replace(TRASM_DOC, "T: ").Replace(TRASM_FOLDER, "T: ").Replace("_", " ");
            }
            return string.Empty;
        }

        /// <summary>
        /// Data una stringa restituisce quest'ultima mappata con le opportune corrispondenze
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        private string MappingStringNotify(string str)
        {
            Hashtable hash = new Hashtable();
            hash.Add("DOCUMENTOCONVERSIONEPDF", "Conversione PDF");
            hash.Add("ACCEPT_TRASM_FOLDER", "Accettazione Trasmissione F.");
            hash.Add("ACCEPT_TRASM_DOCUMENT", "Accettazione Trasmissione D.");
            hash.Add("REJECT_TRASM_FOLDER", "Rifiuto Trasmissione F.");
            hash.Add("REJECT_TRASM_DOCUMENT", "Rifiuto Trasmissione D.");
            hash.Add("CHECK_TRASM_FOLDER", "Visto Trasmissione F.");
            hash.Add("CHECK_TRASM_DOCUMENT", "Visto Trasmissione D.");
            hash.Add("MODIFIED_OBJECT_PROTO", "Mod. oggetto protocollo");
            hash.Add("MODIFIED_OBJECT_DOC", "Modificato oggetto del Documento");
            hash.Add("DOC_CAMBIO_STATO", "Cambio stato documento");
            hash.Add("NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY", "Ricevuta di mancata consegna IS");
            hash.Add("ANNULLA_PROTO", "Annullamento protocollo");
            hash.Add("RECORD_PREDISPOSED", "Protocollazione predisposto");
            hash.Add("NO_DELIVERY_SEND_PEC", "Ricezione mancata consegna/con errori.");
            hash.Add("EXCEPTION_INTEROPERABILITY_PEC", "Eccezione interoperabilità PEC");
            hash.Add("lblSender", "Mittente: ");
            hash.Add("lblObjectDescription", "Oggetto/Descrizione: ");
            hash.Add("lblDta_notify", "Data Notifica: ");
            hash.Add("lblInterno", "I");
            hash.Add("lblArrivo", "A");
            hash.Add("lblPartenza", "P");
            hash.Add("lblStampaReg", "Stampa Registro");
            hash.Add("lblGrigio", "NP");
            hash.Add("lblRepertorio", "Rep: ");
            hash.Add("lblGeneralNote", "Nota generale: ");
            hash.Add("lblIndividualNote", "Nota individuale: ");
            hash.Add("lblDocType", "Tipologia Documento: ");
            hash.Add("lblRejectNote", "Nota di Rifiuto: ");
            hash.Add("lblAcceptNote", "Nota di Accettazione: ");
            hash.Add("lblChangeStateDoc", "Stato documento modificato in: ");
            hash.Add("lblFascType", "Tipologia Fascicolo: ");
            hash.Add("lblDetailReceivedIS", "Dettaglio:  ");
            hash.Add("lblSendRecipientIS", "Destinatario spedizione: ");
            hash.Add("lblExpiration", "Data scadenza: ");
            hash.Add("lblIdNotification", "Id notifica: ");
            hash.Add("lblDtaAbortRecord", "Data Annullamento: ");
            hash.Add("lblDescAbortRecord", "Descrizione Annullamento: ");
            hash.Add("CREATED_FILE_ZIP_INSTANCE_ACCESS", "Preparazione download istanza - Con successo ");
            hash.Add("FAILED_CREATING_FILE_ZIP_INSTANCE_ACCESS", "Preparazione download istanza - Con errori ");
            hash.Add("lblResultCreationFileInstance", "Esito creazione File Zip: ");
            hash.Add("INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE", "Interrotto processo dal titolare Doc ");
            hash.Add("INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE", "Interrotto processo dal titolare	All ");
            hash.Add("INTERROTTO_PROCESSO_ALLEGATO_DAL_PROPONENTE", "Interrotto processo dal proponente All ");
            hash.Add("INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE", "Interrotto processo dal proponente	Doc ");
            hash.Add("CONCLUSIONE_PROCESSO_LF_ALLEGATO", "Conclusione processo All ");
            hash.Add("CONCLUSIONE_PROCESSO_LF_DOCUMENTO", "Conclusione processo Doc ");
            hash.Add("lblDescriptionProcess", "Nome processo: ");
            hash.Add("lblNotesStartup", "Note di avvio: ");
            hash.Add("INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN", "Interrotto processo da Amministratore Doc ");
            hash.Add("INTERROTTO_PROCESSO_ALLEGATO_DA_ADMIN", "Interrotto processo da Amministratore All ");
            hash.Add("lblReasonRejection", "Motivo interruzione: ");
            hash.Add("lblDescriptionAction", "Descrizione evento: ");
            hash.Add("lblSuccessfulConversionDoc", "Esito conversione: ");

            foreach (string key in hash.Keys)
            {
                str = str.Replace(key, (string)hash[key]);
            }

            return str;
        }

        private string DomainObjectNotify(DocsPaVO.Notification.Notification notification)
        {
            try
            {
                string result = string.Empty;
                result = this.FWithoutHtml(notification.ITEMS.ITEM1) + " " + this.FWithoutHtml(notification.ITEMS.ITEM2);
                return MappingStringNotify(result);
            }
            catch (Exception exc)
            {
                return string.Empty;
            }
        }

        private string DetailsNotify(DocsPaVO.Notification.Notification notification, string codEndLine)
        {
            string result = string.Empty;
            try
            {
                result = this.FWithoutHtml(notification.ITEM_SPECIALIZED.Replace("<line>", codEndLine));
                result = MappingStringNotify(result);
                result = MappingLabelNotify("lblIdNotification") + notification.ID_NOTIFY + (result.Substring(0, codEndLine.Length).Equals(codEndLine) ? result : codEndLine + result);
                if (!string.IsNullOrEmpty(notification.ITEMS.ITEM4))
                {
                    result = result + ((char)10).ToString() + this.FWithoutHtml(MappingStringNotify(notification.ITEMS.ITEM4));
                }
            }
            catch (Exception exc)
            {
                return string.Empty;
            }

            return result;
        }

        public string FWithoutHtml(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, @"<[^>]*>", "");
        }

        #endregion

        #region Visibilita Processi Firma

        public DocsPaVO.documento.FileDocumento ExportVisibilitaProcessiFirma(DocsPaVO.utente.InfoUtente infoUtente, string exportType, string title, ArrayList campiSelezionati, List<DocsPaVO.LibroFirma.ProcessoFirma> listaProssiFirma)
        {
            this._exportType = exportType;
            this._title = title;
            this._descAmm = this.getNomeAmministrazione(infoUtente.idPeople);

            switch (exportType)
            {
                case "PDF":
                    ExportVisibilitaProcessiFirmaPDF(infoUtente, listaProssiFirma);
                    break;
                case "XLS":
                    this.ExportVisibilitaProcessiFirmaXLS(infoUtente, campiSelezionati, listaProssiFirma);
                    break;
                case "ODS":
                    this.ExportVisibilitaProcessiODS(infoUtente, campiSelezionati, listaProssiFirma);
                    break;
            }

            return _file;
        }

        #region Export Pdf

        private void ExportVisibilitaProcessiFirmaPDF(DocsPaVO.utente.InfoUtente infoUtente, List<DocsPaVO.LibroFirma.ProcessoFirma> listaProssiFirma)
        {
            try
            {
                if (listaProssiFirma.Count > 0)
                {
                    this._rowsList = Convert.ToString(listaProssiFirma.Count);

                    // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                    // iTextSharp per la stampa dei risultati della ricerca
                    // Il nome del file di template
                    string templateFileName = "XMLStampaRisRicVisibilitaProcessi.xml";
                    // Il datatable utilizzato per creare la tabella con i dati da  visualizzare
                    DataTable infoObjs = this.DataTableVisibilitaProcessi(listaProssiFirma, infoUtente);
                    // 2. Creazione dell'oggetto di generazione PDF
                    StampaPDF.StampaRisRicerca report = new StampaPDF.StampaRisRicerca();
                    // 3. Restituzione del file documento PDF con i dati dell'export
                    this._file = report.GetFileDocumento(templateFileName, this._title, this._descAmm, listaProssiFirma.Count.ToString(), infoObjs);
                }
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug(ex);
            }
        }

        private DataTable DataTableVisibilitaProcessi(List<DocsPaVO.LibroFirma.ProcessoFirma> listaProssiFirma, DocsPaVO.utente.InfoUtente infoUtente)
        {
            const string NOME_PROCESSO = "NOME PROCESSO";
            const string CODICE_RUOLO = "CODICE RUOLO";
            const string DESCRIZIONE_RUOLO = "DESCRIZIONE RUOLO";
            const string TIPO_VISIBILITA = "TIPO VISIBILITA";


            DataTable processoFirma = new DataTable(); 
            DataRow processoFirmaItem;

            processoFirma.Columns.Add(NOME_PROCESSO);
            processoFirma.Columns.Add(CODICE_RUOLO);
            processoFirma.Columns.Add(DESCRIZIONE_RUOLO);
            processoFirma.Columns.Add(TIPO_VISIBILITA);

            foreach (DocsPaVO.LibroFirma.ProcessoFirma processo in listaProssiFirma)
            {
                List<DocsPaVO.LibroFirma.VisibilitaProcessoRuolo> visibilita = GetVisibilitaProcessoFirma(processo.idProcesso, infoUtente);
                if (visibilita != null && visibilita.Count > 0)
                {
                    foreach(DocsPaVO.LibroFirma.VisibilitaProcessoRuolo vis in visibilita)
                    {
                        // Creazione di una nuova riga
                        processoFirmaItem = processoFirma.NewRow();
                        processoFirmaItem[NOME_PROCESSO] = processo.nome;
                        processoFirmaItem[CODICE_RUOLO] = vis.ruolo.codiceRubrica;
                        processoFirmaItem[DESCRIZIONE_RUOLO] = vis.ruolo.descrizione;

                        if(vis.tipoVisibilita.Equals(DocsPaVO.LibroFirma.TipoVisibilita.MONITORATORE))
                            processoFirmaItem[TIPO_VISIBILITA] = "Monitoraggio";
                        else
                            processoFirmaItem[TIPO_VISIBILITA] = "Attivatore";
                        processoFirma.Rows.Add(processoFirmaItem);
                    }
                }
                else
                {
                    // Creazione di una nuova riga
                    processoFirmaItem = processoFirma.NewRow();
                    processoFirmaItem[NOME_PROCESSO] = processo.nome;
                    processoFirmaItem[CODICE_RUOLO] = string.Empty;
                    processoFirmaItem[DESCRIZIONE_RUOLO] = string.Empty;
                    processoFirmaItem[TIPO_VISIBILITA] = string.Empty;
                    processoFirma.Rows.Add(processoFirmaItem);
                }

                
            }

            // Return data set
            return processoFirma;

        }

        #endregion

        #region Export XLS

        private void ExportVisibilitaProcessiFirmaXLS(DocsPaVO.utente.InfoUtente infoUtente, ArrayList campiSelezionati, List<DocsPaVO.LibroFirma.ProcessoFirma> listaProssiFirma)
        {
            try
            {
                string temporaryXSLFilePath = string.Empty;
                StreamWriter writer = null;
                StringBuilder sb = new StringBuilder();

                //Creazione stringa XML
                sb = this.CreateXLSVisibilitaProcessi(listaProssiFirma, campiSelezionati, infoUtente);

                //Salva e chiudi il file
                //temporaryXSLFilePath = HttpContext.Current.Server.MapPath("ExportNotificationCenter.xls");

                temporaryXSLFilePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "Export");
                if (!Directory.Exists(temporaryXSLFilePath))
                    Directory.CreateDirectory(temporaryXSLFilePath);

                temporaryXSLFilePath = Path.Combine(temporaryXSLFilePath, "ExportVisibilitaProcessi.xls");

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
                    stream.Read(contentExcel, 0, contentExcel.Length);
                    stream.Flush();
                    stream.Close();
                    stream = null;
                    this._file.content = contentExcel;
                    this._file.length = contentExcel.Length;
                    this._file.estensioneFile = "xls";
                    this._file.name = "ExportVisibilitaProcessi";
                    this._file.contentType = "application/vnd.ms-excel";
                }

                File.Delete(temporaryXSLFilePath);
            }
            catch (Exception ex)
            {
                this._file = null;
                logger.Debug("Errore esportazione centro notifiche: " + ex.Message);
            }
        }

        private StringBuilder CreateXLSVisibilitaProcessi(List<DocsPaVO.LibroFirma.ProcessoFirma> listaProssiFirma, ArrayList campiSelezionati, DocsPaVO.utente.InfoUtente infoUtente)
        {
            StringBuilder sb = new StringBuilder();
            string strXML = string.Empty;
            //Intestazione XML
            strXML += topXML();

            //Aggiungo una serie di stili utili alla grafica del foglio
            strXML += stiliXML();

            strXML += "<Worksheet ss:Name=\"Visibilita Processi di Firma\">";
            strXML += "<Table>";
            //creo la tabella per le notifiche
            foreach (DocsPaVO.ExportData.CampoSelezionato field in campiSelezionati)
            {
                switch (field.nomeCampo)
                {
                    case "Nome processo":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"130\"/>";
                        break;
                    case "Codice ruolo":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"130\"/>";
                        break;
                    case "Descrizione ruolo":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"130\"/>";
                        break;
                    case "Tipo visibilità":
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"200\"/>";
                        break;
                    default:
                        strXML += "<Column ss:StyleID=\"s63\" ss:Width=\"100\"/>";
                        break;
                }
            }

            strXML += InsertDataVisibilitaProcessiXML(listaProssiFirma, campiSelezionati, infoUtente);
            strXML += "</Table>";
            strXML += workSheetOptionsXML();
            strXML += "</Worksheet>";
            strXML += "</Workbook>";

            sb.Append(strXML.ToString());
            return sb;
        }

        private string InsertDataVisibilitaProcessiXML(List<DocsPaVO.LibroFirma.ProcessoFirma> listaProssiFirma, ArrayList campiSelezionati, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string strXML = string.Empty;
            strXML = CreateColumnsVisibilitaProcessi(campiSelezionati);
            strXML += InsertDataVisibilitaProcessi(listaProssiFirma, campiSelezionati, infoUtente);
            return strXML;
        }

        private string CreateColumnsVisibilitaProcessi(ArrayList campiSelezionati)
        {
            string strXML = string.Empty;
            strXML += "<Row>";
            foreach (DocsPaVO.ExportData.CampoSelezionato field in campiSelezionati)
            {
                strXML += "<Cell ss:StyleID=\"s70\">";
                strXML += "<Data ss:Type=\"String\">" + field.nomeCampo.ToString().Replace(":", string.Empty);
                strXML += "</Data>";
                strXML += "</Cell>";
            }

            strXML += "</Row>";
            return strXML;
        }

        private string InsertDataVisibilitaProcessi(List<DocsPaVO.LibroFirma.ProcessoFirma> listaProssiFirma, ArrayList campiSelezionati, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string righe = string.Empty;

            if (listaProssiFirma.Count != 0)
            {
                foreach (DocsPaVO.LibroFirma.ProcessoFirma processo in listaProssiFirma)
                {
                    List<DocsPaVO.LibroFirma.VisibilitaProcessoRuolo> visibilita = GetVisibilitaProcessoFirma(processo.idProcesso, infoUtente);
                    if (visibilita != null && visibilita.Count > 0)
                    {
                        foreach(DocsPaVO.LibroFirma.VisibilitaProcessoRuolo vis in visibilita)
                        {
                            string riga = string.Empty;
                            riga = "<Row>";

                            foreach (DocsPaVO.ExportData.CampoSelezionato field in campiSelezionati)
                            {
                                switch (field.nomeCampo)
                                {
                                    case "Nome processo":
                                        riga += "<Cell>";
                                        riga += "<Data ss:Type=\"String\">" + processo.nome;
                                        riga += "</Data>";
                                        riga += "</Cell>";
                                        break;

                                    case "Codice ruolo":
                                        riga += "<Cell>";
                                        riga += "<Data ss:Type=\"String\">" + vis.ruolo.codiceRubrica;
                                        riga += "</Data>";
                                        riga += "</Cell>";
                                        break;

                                    case "Descrizione ruolo":
                                        riga += "<Cell>";
                                        riga += "<Data ss:Type=\"String\">" + vis.ruolo.descrizione;
                                        riga += "</Data>";
                                        riga += "</Cell>";
                                        break;

                                    case "Tipo visibilità":

                                        riga += "<Cell>";
                                        riga += "<Data ss:Type=\"String\">" + ((vis.tipoVisibilita.Equals(DocsPaVO.LibroFirma.TipoVisibilita.MONITORATORE)) ? "Monitoraggio" : "Attivatore");
                                        riga += "</Data>";
                                        riga += "</Cell>";
                                        break;
                                }
                            }

                            riga += "</Row>";
                            righe += riga;
                        }
                    }
                    else
                    {
                        string riga = string.Empty;
                        riga = "<Row>";

                        foreach (DocsPaVO.ExportData.CampoSelezionato field in campiSelezionati)
                        {
                            switch (field.nomeCampo)
                            {
                                case "Nome processo":
                                    riga += "<Cell>";
                                    riga += "<Data ss:Type=\"String\">" + processo.nome;
                                    riga += "</Data>";
                                    riga += "</Cell>";
                                    break;

                                case "Codice ruolo":
                                    riga += "<Cell>";
                                    riga += "<Data ss:Type=\"String\">";
                                    riga += "</Data>";
                                    riga += "</Cell>";
                                    break;

                                case "Descrizione ruolo":
                                    riga += "<Cell>";
                                    riga += "<Data ss:Type=\"String\">";
                                    riga += "</Data>";
                                    riga += "</Cell>";
                                    break;

                                case "Tipo visibilità":

                                    riga += "<Cell>";
                                    riga += "<Data ss:Type=\"String\">";
                                    riga += "</Data>";
                                    riga += "</Cell>";
                                    break;
                            }
                        }
                        riga += "</Row>";
                        righe += riga;
                    }
                }
            }
            return righe;
        }

        #endregion

        #region Export ODS

        private void ExportVisibilitaProcessiODS(DocsPaVO.utente.InfoUtente infoUtente, ArrayList campiSelezionati, List<DocsPaVO.LibroFirma.ProcessoFirma> listaProssiFirma)
        {
            DataSet docExp = new DataSet();
            docExp = createOpenOfficeVisibilitaProcessi(listaProssiFirma, campiSelezionati, infoUtente);

            string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
            serverPath = serverPath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
            string userDoc = "\\export_visibilita_processi" + infoUser.idPeople + ".ods";
            OpenDocument.OpenDocumentServices open = new OpenDocument.OpenDocumentServices(serverPath + userDoc);
            open.SetTitle("Export visibilità processi - Trovati " + listaProssiFirma.Count + " processi di firma");
            open.SetSubtitle(this._title);
            open.SetData(docExp);
            this._file = open.SaveAndExportData();
        }

        protected DataSet createOpenOfficeVisibilitaProcessi(List<DocsPaVO.LibroFirma.ProcessoFirma> listaProssiFirma, ArrayList campiSelezionati, InfoUtente userInfo)
        {
            DataSet result = new DataSet();
            DataTable dt = new DataTable();
            DataRow column = dt.NewRow();
            String value;
            int i = 0;
            foreach (DocsPaVO.LibroFirma.ProcessoFirma processo in listaProssiFirma)
            {
                List<DocsPaVO.LibroFirma.VisibilitaProcessoRuolo> visibilita = GetVisibilitaProcessoFirma(processo.idProcesso, userInfo);
                if (visibilita != null && visibilita.Count > 0)
                {
                    foreach (DocsPaVO.LibroFirma.VisibilitaProcessoRuolo vis in visibilita)
                    {
                        // Riga da restituire
                        DataRow toReturn = dt.NewRow();

                        // Inserimento dei valori per i campi
                        foreach (CampoSelezionato selectedField in campiSelezionati)
                        {
                            switch (selectedField.nomeCampo)
                            {
                                case "Nome processo":
                                    value = processo.nome;
                                    if (i == 0)
                                    {
                                        dt.Columns.Add(selectedField.nomeCampo);
                                        column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                                    }
                                    toReturn.SetField(selectedField.nomeCampo, value);
                                    break;
                                case "Codice ruolo":
                                    value = vis.ruolo.codiceRubrica;
                                    if (i == 0)
                                    {
                                        dt.Columns.Add(selectedField.nomeCampo);
                                        column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                                    }
                                    toReturn.SetField(selectedField.nomeCampo, value);
                                    break;
                                case "Descrizione ruolo":
                                    value = vis.ruolo.descrizione;
                                    if (i == 0)
                                    {
                                        dt.Columns.Add(selectedField.nomeCampo);
                                        column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                                    }
                                    toReturn.SetField(selectedField.nomeCampo, value);
                                    break;
                                case "Tipo visibilità":
                                    value = ((vis.tipoVisibilita.Equals(DocsPaVO.LibroFirma.TipoVisibilita.MONITORATORE)) ? "Monitoraggio" : "Attivatore");
                                    if (i == 0)
                                    {
                                        dt.Columns.Add(selectedField.nomeCampo);
                                        column.SetField(selectedField.nomeCampo, selectedField.nomeCampo);
                                    }
                                    toReturn.SetField(selectedField.nomeCampo, value);
                                    break;
                            }

                        }
                        if (i == 0)
                        {
                            dt.Rows.Add(column);
                        }

                        i++;
                        dt.Rows.Add(toReturn);
                    }
                }
            }
            result.Tables.Add(dt);
            return result;
        }

        #endregion

        private List<DocsPaVO.LibroFirma.VisibilitaProcessoRuolo> GetVisibilitaProcessoFirma(string idProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<DocsPaVO.LibroFirma.VisibilitaProcessoRuolo> visibilita = new List<DocsPaVO.LibroFirma.VisibilitaProcessoRuolo>();

            DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            visibilita = libro.GetVisibilitaProcesso(idProcesso, null, infoUtente);

            return visibilita;
        }

        #endregion

    }
}
