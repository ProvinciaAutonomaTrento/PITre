using System;
using System.Text.RegularExpressions;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA
{
    public partial class visualizzaOggetto : System.Web.UI.Page
    {
        #region Dichiarazione Variabili globali

        // L'id dell'amministrazione
        string administrationId;

        // Tipo oggetto da visualizzare (F per fascicolo, D per documento)
        string objectType;

        // Id oggetto da visualizzare (DocNumber per i documenti, SystemID per i fascicoli)
        string objectId;

        // Tipo di oggetto da visualizzare (A, P e G per documento)
        string protoType;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Dichiarazione Veriabili

            // Valore utilizzato per indicare l'esito della validazione
            // dei parametri da query string
            bool validParameters;

            // Valore utilizzato per indicare l'esito della verifica di
            // visibilità dell'oggetto da parte dell'utente
            bool canView;

            #endregion

            Utils.startUp(this);

            // Lettura e validazione parametri
            validParameters = this.ReadAndValidateParameters();

            // Se la validazione ha avuto esito positivo...
            if (validParameters)
            {
                // ...si verifica se l'utente è autenticato. In caso negativo si procede
                // alla sua autenticazione.
                this.AuthenticateUser();

                // Verifica dei diritti di visibilità
                canView = this.CheckRights(
                    this.objectId,
                    this.objectType,
                    UserManager.getUtente());

                // Se l'utente possiede visibilità sull'oggetto si provvede 
                // alla sua visualizzazione
                if (canView)
                    // Se l'oggetto da ricercare è un documento...
                    if (this.objectType == "D")
                        // ...si procede alla sua visualizzione
                        this.ShowDocument();
                    else
                        // ...altrimenti si procede alla visualizzazione del documento
                        this.ShowProject();

            }

        }

        /// <summary>
        /// Funzione per la lettura e la validazione dei parametri da query string
        /// </summary>
        /// <returns>True se lettura e validazione dati vanno a buon fine</returns>
        private bool ReadAndValidateParameters()
        {
            // Il risultato dell'elaborazione
            bool toReturn = true;

            // Espressione regolare per verificare se una stringa è un numero
            Regex isNumeric = new Regex("[0-9]*");

            #region Id amministrazione

            // Lettura e validazione id amministrazione
            if (String.IsNullOrEmpty(Request["idAmministrazione"].Trim()) ||
                !(isNumeric.Match(Request["idAmministrazione"].Trim()).Success))
            {
                this.blMessage.Items.Add("Id amministrazione non valido.");
                toReturn = false;
            }
            else
                this.administrationId = Request["idAmministrazione"].Trim();

            #endregion

            #region Tipo Oggetto

            // Lettura e validazione tipo oggetto
            if (String.IsNullOrEmpty(Request["tipoOggetto"].Trim()) ||
                (Request["tipoOggetto"].Trim() != "F" &&
                Request["tipoOggetto"].Trim() != "D"))
            {
                this.blMessage.Items.Add("Tipo Oggetto non valido.");
                toReturn = false;
            }
            else
                this.objectType = Request["tipoOggetto"].Trim();

            #endregion

            #region Id Oggetto

            // Lettura e validazione id oggetto
            if (String.IsNullOrEmpty(Request["idObj"].Trim()) ||
                !(isNumeric.Match(Request["idObj"].Trim()).Success))
            {
                this.blMessage.Items.Add("Id Oggetto non valido.");
                toReturn = false;
            }
            else
                this.objectId = Request["idObj"].Trim();

            #endregion

            #region Tipo proto

            // Lettura e validazione tipo oggetto
            if (!String.IsNullOrEmpty(Request["tipoProto"]) &&
                Request["tipoProto"].Trim() != "A" &&
                Request["tipoProto"].Trim() != "P" &&
                Request["tipoProto"].Trim() != "G" &&
                Request["tipoProto"].Trim() != "I")
            {
                this.blMessage.Items.Add("Tipo Proto non valido.");
                toReturn = false;
            }
            
            if(!String.IsNullOrEmpty(Request["tipoProto"]))
                this.protoType = Request["tipoProto"].Trim();

            #endregion

            // Restituzione esito di validazione
            return toReturn;

        }

        /// <summary>
        /// Funzione per la verifica di autenticazione dell'utente
        /// </summary>
        private void AuthenticateUser()
        {
            // L'utente autenticato
            Utente userHome;

            // L'indirizzo della pagina di login
            string lgnPage;

            // Si prova a prelevare l'utente attualmente loggato
            try
            {
                userHome = UserManager.getUtente();
            }
            catch (Exception ex)
            {
                // ...se si sono verificati errori si considera 
                // l'utente come non loggato...
                userHome = null;
            }

            if (userHome == null)
            {
                lgnPage = Utils.getHttpFullPath(this) + "/login.aspx?" +
                    Request.QueryString.ToString();

                //string scriptString = String.Format("redirectToLogin('{0}');", lgnPage);
                //ClientScript.RegisterStartupScript(this.GetType(), "redLogin", scriptString, true);
                Response.Redirect(lgnPage);

            }

        }

        /// <summary>
        /// Funzione per la ricerca e la visualizzazione del documento 
        /// specificato
        /// </summary>
        private void ShowDocument()
        {
            #region Dichiarazione Variabili

            // Matrice dei filtri di ricerca
            FiltroRicerca[][] qV;

            // Lista dei filtri di ricerca
            FiltroRicerca[] fVList;

            // Filtro di ricerca
            FiltroRicerca fV1;

            // Informazioni sull'utente loggato
            InfoUtente userInfo;

            // Scheda del documento da visualizzare
            SchedaDocumento schedaDoc;

            // Informazioni sul documento da visualizzare
            InfoDocumento infoDoc;

            // Tab della pagina di profilo da visualizzare e script da immergere nella
            // pagina
            string tab = String.Empty, scriptToSubmit;

            #endregion

            // Creazione matrice dei filtri di ricerca e dell'array dei filtri
            qV = new FiltroRicerca[1][];
            qV[0] = new FiltroRicerca[1];
            fVList = new FiltroRicerca[0];

            // filtro DOCNUMBER
            fV1 = new FiltroRicerca();
            fV1.argomento = FiltriDocumento.DOCNUMBER.ToString();
            fV1.valore = objectId;
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

            // filtro TIPODOC
            fV1 = new FiltroRicerca();
            fV1.argomento = FiltriDocumento.TIPO.ToString();
            fV1.valore = this.protoType;
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

            qV[0] = fVList;

            // Recupero delle informazioni sull'utente
            userInfo = UserManager.getInfoUtente(this);

            // Recupero della scheda del documento
            schedaDoc = DocumentManager.getDettaglioDocumento(this, "", this.objectId);

            // Se la scheda è stata recuperata con successo...
            if (schedaDoc != null)
            {
                // ...recupero delle informazioni sul documento
                infoDoc = DocumentManager.getInfoDocumento(schedaDoc);

                // Salvataggio dei risultati di ricerca
                DocumentManager.setRisultatoRicerca(this, infoDoc);

                // A seconda del tipo proto viene individuato il tab da visualizzare
                switch (infoDoc.tipoProto.ToUpper())
                {
                    case "G":   // Grigio
                        tab = "profilo";
                        break;
                    case "A":   // Arrivo e partenza
                    case "P":
                        tab = "protocollo";
                        break;
                }

                /*
                // Se si proviene dalla pagina di login, si redireziona l'utente alla pagina index
                // altirmenti lo si redireziona alla pagina di dettaglio del documento
                if (Request.UrlReferrer != null &&
                    Request.UrlReferrer.PathAndQuery.Contains("login.aspx"))
                    scriptToSubmit = String.Format(
                        "redirectToPage('{0}/index.aspx?tab={1}&objType={2}', 'true');",
                        Utils.getHttpFullPath(this),
                        tab,
                        this.objectType);
                else
                    scriptToSubmit = String.Format(
                        "redirectToPage('{0}/documento/gestioneDoc.aspx?tab={1}', 'false');",
                        Utils.getHttpFullPath(this),
                        tab);
                */

                scriptToSubmit = String.Format(
                        "redirectToPage('{0}/index.aspx?tab={1}&objType={2}', 'true');",
                        Utils.getHttpFullPath(this),
                        tab,
                        this.objectType);

                // Immersione dello script nella pagina
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "Index",
                    scriptToSubmit,
                    true);


            }
            else
                // ...altrimenti viene segnalato all'utente che il recupero dei dati è fallito
                this.blMessage.Items.Add("Oggetto non trovato");
        }

        /// <summary>
        /// Funzione per la ricerca e la visualizzazione del fascicolo
        /// specificato
        /// </summary>
        private void ShowProject()
        {
            #region Dichiarazione variabili

            // Informazioni sull'utente
            InfoUtente userInfo;

            // Fascicolo da visualizzare
            Fascicolo fascicoloSelezionato;

            // Script da immergere nella pagina
            string scriptToSubmit;

            #endregion

            // Reperimento delle informazioni sull'utente loggato
            userInfo = UserManager.getInfoUtente(this);

            // Selezione del fascicolo da visualizzare
            fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice(this, this.objectId, userInfo);
            
            // Se il fascicolo è stato recuperato con successo...
            if (fascicoloSelezionato != null)
            {
                // ...salvataggio del risultato di ricerca
                FascicoliManager.setFascicoloSelezionato(this, fascicoloSelezionato);
                
                // ...impostazione del registro
                UserManager.setRegistroSelezionato(this, ((DocsPAWA.DocsPaWR.Ruolo)this.Session["userRuolo"]).registri[0]);

                // Se si proviene dalla pagina di login, si redireziona l'utente alla pagina index
                // altrimenti lo si redireziona alla pagina di dettaglio del fascicolo
                if (Request.UrlReferrer != null &&
                    Request.UrlReferrer.PathAndQuery.Contains("login.aspx"))
                    scriptToSubmit = String.Format(
                        "redirectToPage('{0}/index.aspx?tab=documenti&objType={1}', 'true');",
                        Utils.getHttpFullPath(this),
                        this.objectType);
                else
                    scriptToSubmit = String.Format(
                          "redirectToPage('{0}//index.aspx?tab=fascicoli&objType={1}', 'true');",
                        Utils.getHttpFullPath(this),
                        this.objectType);

                // Immersione dello script nella pagina
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "Index",
                    scriptToSubmit,
                    true);

            }
        }

        /// <summary>
        /// Funzione per la verifica dei diritti di visibilità da parte di un
        /// utente nei confronti di un oggetto
        /// </summary>
        /// <param name="systemId">System id dell'oggetto di cui verificare i diritti</param>
        /// <param name="objType">Tipo di oggetto di cui verificare i diritti</param>
        /// <param name="userHome">L'utente di cui verificare i diritti</param>
        /// <returns>True se l'utente possiede i diritti di visibilità per l'oggetto specificato</returns>
        private bool CheckRights(string systemId, string objType, Utente userHome)
        {
            // L'eventuale messaggio di errore resituito dalla verifica della visibilità
            string errorMessage = String.Empty;

            // Il fascicolo da mostrare
            Fascicolo project = null;

            if (Session["userRuolo"] != null)
                return true;

            // Variabile utilizzata per indicare che un ruolo valido per
            // visalizzare il documento è stato trovato
            bool trovato = false;

            foreach (Ruolo ruolo in userHome.ruoli)
            {
                // ...viene immesso il ruolo corrente in sessione...
                Session["userRuolo"] = ruolo;

                // ...se con il ruolo attuale l'utente ha i diritti 
                // necessari alla visualizzazione...
                // (nel caso di fascicolo è prima necessario ricavarne l'id)
                if (objType == "F")
                    project = FascicoliManager.getFascicoloDaCodice(this, this.objectId);

                if (project != null)
                    systemId = project.systemID;
                         
                if (DocumentManager.verificaACL(
                    objType,
                    systemId ,
                    UserManager.getInfoUtente(),
                    out errorMessage) == 2)
                {
                    trovato = true;
                    break;
                }
            }

            // Se non è stato trovato un ruolo adatto
            if (!trovato)
                // viene segnalato all'utente che non ha i diritti
                // necessari
                this.blMessage.Items.Add(errorMessage);

            return trovato;

        }

    }

}
