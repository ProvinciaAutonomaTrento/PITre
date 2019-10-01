using System;
using System.Configuration;
using System.Web;
using System.IO;

namespace DocsPAWA.documento.VisualizzatoreLink
{
    public partial class VisualizzaDocumento : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            if(UserManager.getUtente(HttpContext.Current.Session) != null)
                this.ShowDocument();
        }

        private void ShowDocument()
        {
            // L'oggetto con le informazioni sul file da visualizzare
            DocsPaWR.FileDocumento theDoc = null;

            // La label con le informazioni sulla segnatura
            DocsPaWR.labelPdf label = new DocsPAWA.DocsPaWR.labelPdf();

            // I Parametri passati da query string
            string plusEtic = string.Empty;
            string PosLabelPdf = null;
            bool tipo = false;
            string rotazione = null;
            string carattere = string.Empty;
            string colore = string.Empty;
            string orientamento = null;

            // Paramentri presenti in query string utilizzati per un rapido reprimento
            // del file del documento
            string docNumber = String.Empty;
            string verisonId = String.Empty;
            string versionNumber = String.Empty;
            string path = String.Empty;
            string fileName = String.Empty;

            // Valore booleano per indicare se il documento va visualizzato
            // nel suo formato originale
            bool visInOrig = false;

            // Se il query string contiene visInOrig impostato a 1
            // ed il ruolo è abilitato alla visualizzazione del file in
            // originale
            if (!string.IsNullOrEmpty(Request.QueryString["visInOrig"])
                && Request.QueryString["visInOrig"].ToString().Equals("1")
                && UserManager.ruoloIsAutorized(this, "DO_VIS_ORIG"))
                visInOrig = true;

            // Lettura dei parametri per un rapido reprimenti del file
            docNumber = Request["docNumber"];
            verisonId = Request["versionId"];
            versionNumber = Request["versionNumber"];
            path = Server.UrlDecode(Request["path"]);
            fileName = Server.UrlDecode(Request["fileName"]);
            
           

            // Se in sessione non è presente un valore per docToSign...
            if (Session["docToSign"] == null)
            {
                // ...si procede con il caricamento normale del documento
                plusEtic = Request["plusEtic"];
                PosLabelPdf = Request["pos"];
                tipo = System.Convert.ToBoolean(Request["tipo"]);
                rotazione = Request["rotazione"];
                carattere = Request["carattere"];
                colore = Request["colore"];
                orientamento = Request["orientamento"];
                //carico i dati dentro l'oggetto Label
                label.position = PosLabelPdf;
                label.tipoLabel = tipo;
                label.label_rotation = rotazione;
                label.orientamento = orientamento;
                label.sel_font = carattere;
                label.sel_color = colore;

                // Se bisogna visualizzare l'etichetta...
                if (!String.IsNullOrEmpty(plusEtic))
                    // ...reperimento del contenuto del documento
                    theDoc = FileManager.getInstance(Session.SessionID).
                        GetFileConSegnatura(this, verisonId, versionNumber,
                            docNumber, path, fileName, label);

                else
                {
                    bool showAsPdfFormat = false;

                    // Se visInOrig è false...
                    if (!visInOrig)
                    {
                        // ...si prova a verficare se si è abilitati a visualizzare il documento
                        // direttamente convertito in PDF
                        bool showDocumentAsPdf = false;

                        Boolean.TryParse(
                            ConfigurationManager.AppSettings["SHOW_DOCUMENT_AS_PDF_FORMAT"],
                            out showAsPdfFormat);

                        // Si preleva il file
                        if (fileName.ToUpper().EndsWith(".EML"))
                            theDoc = FileManager.getInstance(Session.SessionID).GetFileAsEML(this, verisonId, versionNumber, docNumber, path, fileName, showAsPdfFormat);
                        else
                            theDoc = FileManager.getInstance(Session.SessionID).
                            GetFile(this, verisonId, versionNumber,
                                docNumber, path, fileName, showAsPdfFormat);
                    }
                }
            }
            else
                // ...altrimenti viene prelevato dalla sessione il documento da firmare
                theDoc = (DocsPAWA.DocsPaWR.FileDocumento)Session["docToSign"];

            // Se il documento è stato reperito con successo... set datavista se richiesto.

            if (theDoc != null)
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SET_DATA_VISTA_GRD"]))
                {
                    if (ConfigurationManager.AppSettings["SET_DATA_VISTA_GRD"] == "1" || ConfigurationManager.AppSettings["SET_DATA_VISTA_GRD"] == "2")
                        // nella configurazione SET_DATA_VISTA_GRD=2, la visualizzazione del documento dalle griglie non deve farlo, ma il controllo sta sul BE, quindi dal FE 
                        // la lascio eseguire.
                        FileManager.setdatavistaSP(UserManager.getInfoUtente(), docNumber, "D");
               }

            // Se il documento è stato reperito con successo...  lo visualizzo
            if (theDoc != null)
            {
                if (theDoc.fullName.Contains("."))
                {
                    string extension = theDoc.fullName.Substring(theDoc.fullName.LastIndexOf(".") + 1);
                    if (!string.IsNullOrEmpty(extension) && extension.ToUpper().Equals("XML")) { theDoc.contentType = "image/jpeg"; }
                }
                // Si verifica se il documento deve essere scaricato o visualizzato
                // internamente
                bool downloadAsAttatchment = false;
                bool.TryParse(this.Request.QueryString["downloadAsAttatchment"], out downloadAsAttatchment);
                // verifica se l'esetnsione del file è di tipo eml
                //downloadAsAttatchment |= Path.GetExtension(theDoc.fullName).ToLower().Equals(".eml");

                // verifico se l'estensione del file è .pps
                if (Path.GetExtension(theDoc.fullName).ToLower().Equals(".pps"))
                    downloadAsAttatchment = true;

                // Se bisogna visualizzare esternamente...
                if (downloadAsAttatchment)
                    // ...si visualizza il file come attachment
                    this.WriteResponse("attachment", theDoc);
                else
                    // ...altrimenti si visualizza internamente
                    this.WriteResponse("inline", theDoc);

            }
            else
                // ...altrimenti si redireziona l'utente alla pagina di errore
                ErrorManager.redirectToErrorPage(this, new Exception("Non è stato possibile recuperare il file"));

        }

        /// <summary>
        /// Funzione che si occupa di visualizzare il documento
        /// (scrittura del content del documento nel content del body)
        /// </summary>
        /// <param name="contentDisposition">La tipologia di visualizzazione</param>
        /// <param name="theDoc">Le informazioni sul file da visualizzare</param>
        private void WriteResponse(string contentDisposition, DocsPaWR.FileDocumento theDoc)
        {
            // Pulizia e creazione degli header
            Response.Buffer = true;
            Response.ClearHeaders();
            Response.ContentType = theDoc.contentType;
            Response.AppendHeader("Content-Disposition", contentDisposition + ";filename=" + theDoc.name);
            Response.AddHeader("Content-Length", theDoc.content.Length.ToString());

            // Scrittura del contenuto dl file
            Response.BinaryWrite(theDoc.content);

            // Flushcing ed invio della response
            Response.Flush();
            Response.End();
    
        }

    }

}
