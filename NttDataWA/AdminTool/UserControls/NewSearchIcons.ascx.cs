using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Configuration;
using SAAdminTool.DocsPaWR;
using System.IO;
using System.Text;

namespace SAAdminTool.UserControls
{
    public partial class NewSearchIcons : System.Web.UI.UserControl
    {
        /// <summary>
        /// Evento scatenato al completamento di una operazione di inserimento / rimozione 
        /// effettuata sull'area di lavoro. Consultare l'event args per ottenere informazioni
        /// sull'esito e sull'id dell'oggetto su cui è stata compiuta l'operazione.
        /// </summary>
        public EventHandler OnWorkingAreaOperationCompleted;

        /// <summary>
        /// Evento scatenato al completamento di una operazione di inserimento / rimozione
        /// effettuata sull'area di conservazione. Consultare l'event args per ottenere 
        /// informazioni sull'esito e sull'id dell'oggetto cu cui è stata compiuta l'operazione.
        /// </summary>
        public EventHandler OnStorageAreaOperationCompleted;

        /// <summary>
        /// Evento lanciato quando viene cliccato il pulsante per la visualizzione dell'immagine
        /// associata al documento
        /// </summary>
        public EventHandler OnViewImageCompleted;

        /// <summary>
        /// Evento scatenato quando viene cliccato il pulsante per la visualizzazione dei dettagli
        /// della firma del documento
        /// </summary>
        public EventHandler OnViewSignDetailsCompleted;
       
        /// <summary>
        /// Enumerazione delle possibili pagine in cui può essere inserita questa
        /// pagina
        /// </summary>
        public enum ParentPageEnum
        {
            SearchDocument,             // Ricerca documenti
            SearchToDoList,             // Ricerca to do list
            SearchProject,              // Ricerca fascicoli
            SearchDocumentInProject     // Ricerca documenti in fascicolo

        }

        /// <summary>
        /// Tipo di oggetto cui fa riferimento il controllo
        /// </summary>
        public enum ObjectTypeEnum
        {
            Document,
            Project
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Inizializzazione del controllo se è valorizzata la proprietà
            // ObjectId
            if (!String.IsNullOrEmpty(this.ObjectId))
                this.Initialize();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Inizializzazione del controllo se è valorizzata la proprietà
            // ObjectId
            if (!String.IsNullOrEmpty(this.ObjectId))
                this.Initialize();
        }

        /// <summary>
        /// Al click su questo pulsante, si procede con l'inserimento o la rimozione
        /// dell'oggetto dall'area di lavoro
        /// </summary>
        protected void btnWorkingArea_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton castedSender = (ImageButton)sender;

            switch (castedSender.CommandName)
            {
                case "InsertDocumentInWA":
                    // Inserimento del documento nell'ADL
                    this.InsertDocumentInWA(castedSender.CommandArgument);
                    break;
                case "RemoveDocumentFromWA":
                    // Rimozione del documento dall'ADL
                    this.RemoveDocumentFromWA(castedSender.CommandArgument);
                    break;
                case "InsertProjectInWA":
                    // Inserimento del fascicolo nell'ADL
                    this.InsertProjectInWA(castedSender.CommandArgument);
                    break;
                case "RemoveProjectFromWA":
                    // Rimozione del fascicolo dall'ADL
                    this.RemoveProjectFromWA(castedSender.CommandArgument);
                    break;

            }

            // Aggiornamento delle informazioni sul bottone Inserisci / Rimuovi dalla
            // To do list
            this.InitializeOrUpdateWAButton(castedSender.CommandArgument, this.ProtoType);

            // Viene scatento l'evento se registrato
            if (this.OnWorkingAreaOperationCompleted != null)
                this.OnWorkingAreaOperationCompleted(
                    this,
                    new NewSearchIconsEventArgs()
                    {
                        ObjectId = this.ObjectId,
                        Result = true
                    });
            
        }

        /// <summary>
        /// Al click su questo pulsante, viene rimosso il documento dal fasciolo aperto
        /// </summary>
        protected void btnRemoveDocumentFromProject_Click(object sender, ImageClickEventArgs e)
        {
            // Il folder selezionato
            Folder folder;

            // Messaggio restituito dalla procedura di rimozione
            String message;

            // Risultato della rimozione
            ValidationResultInfo result;

            // Recupero del folder selezionato
            folder = FascicoliManager.getFolderSelezionato(Page);

            // Rimozione del documento
            string valoreChiaveFasc = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
            if (string.IsNullOrEmpty(valoreChiaveFasc))
                valoreChiaveFasc = "false";
            result = FascicoliManager.deleteDocFromFolder(
                Page,
                folder,
                ((ImageButton)sender).CommandArgument,
                valoreChiaveFasc,
                out message);

            // Se result è valorizzato ed esistono delle BrokenRules, viene
            // visualizzato un messaggio
            if (result != null &&
                result.BrokenRules.Length > 0)
            {
                this.ShowMessageToUser(result.BrokenRules[0].Description);
            }
            else
            {
                // Altrimenti viene visualizzato il messaggio restituito dall'operazione
                // di rimozione
                if (!string.IsNullOrEmpty(message))
                {
                    this.ShowMessageToUser(message);
                }
                else
                {
                    if (Request["idFolder"] != null)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "LanciaRic", "top.principale.document.iFrame_dx.location='tabPulsantiDoc.aspx?IdFolder=" + Request["idFolder"].ToString() + "';", true);
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "LanciaRic", "top.principale.document.iFrame_dx.location='tabPulsantiDoc.aspx';", true);
                }

            }
    
        }

        /// <summary>
        /// Al click su questo pulsante, viene inserito / rimosso il documento / fascicolo dall'area di conservazione
        /// </summary>
        protected void btnStorageArea_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton castedSender = (ImageButton)sender;

            // Risultato dell'operazione
            String result = String.Empty;

            switch (castedSender.CommandName)
            {
                case "InsertDocumentInSA":
                    // Si procede con l'inserimento
                    result = this.InsertDocumentInStorageArea(castedSender.CommandArgument, null);
                    break;

                case "RemoveDocumentFromSA":
                    // Si procede con la rimozione dall'area di conservazione
                    result = this.RemoveDocumentFromStorageArea(castedSender.CommandArgument);
                    break;

                case "InsertProjectInSA":
                    // Inserimento del fascicolo nell'area di conservazione
                    result = this.InsertProjectInStorageArea(castedSender.CommandArgument);
                    break;

                case "RemoveProjectFromSA":
                    // Rimozione del fascicolo dall'area di conservazione
                    result = this.RemoveProjectFromStorageArea(castedSender.CommandArgument);
                    break;
            }

            if (!String.IsNullOrEmpty(result))
                this.ShowMessageToUser(result);
            
            // Aggiornamento informazioni sul bottone Inserisci / Rimuovi da
            // Area di conservazione
            this.InitializeOrUpdateSAButton(castedSender.CommandArgument);

            // Viene scatenato l'evento se registrato
            if (this.OnStorageAreaOperationCompleted != null)
                this.OnStorageAreaOperationCompleted(
                    this,
                    new NewSearchIconsEventArgs()
                        {
                            ObjectId = this.ObjectId,
                            Result = String.IsNullOrEmpty(result)
                        });

        }

        /// <summary>
        /// Al click su questo pulsante, da javascript viene avviato lo script per la visualizzazione dell'immagine
        /// del documento mentre da codice viene scatenato un evento per segnalare che è stato cliccato questo
        /// pulsante
        /// </summary>
        protected void btnShowDocumentFile_Click(object sender, ImageClickEventArgs e)
        {
            // Se registrato, viene scatenato l'evento di completamento visualizzazione immagine
            if (this.OnViewImageCompleted != null)
                this.OnViewImageCompleted(this, new NewSearchIconsEventArgs() { ObjectId = this.ObjectId });

        }

        /// <summary>
        /// Al click su questo pulsante, da javascript viene avviato lo script per la visualizzazione del
        /// dettaglio della firma del documento mentre da codice viene scatenato un evento per segnalare che è 
        /// stato cliccato questo pulsante
        /// </summary>
        protected void btnSignInformation_Click(object sender, ImageClickEventArgs e)
        {
            // Se registrato, viene scatenato l'evento di completamento visualizzazione dettaglio firma
            if (this.OnViewSignDetailsCompleted != null)
                this.OnViewSignDetailsCompleted(this, new NewSearchIconsEventArgs() { ObjectId = this.ObjectId });

        }

        #region Inizializzazione e aggiornamento pagina

        /// <summary>
        /// Funzione per l'inizializzazione del controllo
        /// </summary>
        public void Initialize()
        {
            
            // Diritti di visibilità sul documento
            int rights;

            // Messaggio di errore ricevuto dalla richiesta dei diritti sul documento
            String errorMessage;

            // Bottone Dettaglio documento / fascicolo / trasmissione (Gestione di btnShowDetails e btnShowDocumentDetails)
            this.InitializeDetailsButton(this.ObjectId, this.NavigateUrl);

            // Bottone ADL
            this.InitializeOrUpdateWAButton(this.ObjectId, this.ProtoType);

            // Bottone Visualizza File Documento
            this.InitializeFileButton(this.FileExtension, this.ObjectId);

            // Bottone Visualizza Informazioni Firma
            this.InitializeSignButton(this.ObjectId);

            // Bottone Elimina documento da fascicolo
            this.InitializeRemoveDocumentButton(this.ObjectId);
            
            // Bottone conservazione
            this.InitializeOrUpdateSAButton(this.ObjectId);

        }

        /// <summary>
        /// Funzione per l'inizializzazione del bottone per 
        /// mostrare i dettagli del documento
        /// </summary>
        private void InitializeDetailsButton(String objectId, String navigationUrl)
        {
            // Se il controllo è legato ad un fascicolo viene visualizzata l'icona
            // appropriata, viene modificato il tool tip "Vai al dettaglio del fascicolo"
            // ed impostato il command name
            if (this.ObjectType == ObjectTypeEnum.Project)
            {
                this.btnShowDetails.ImageUrl = "~/images/proto/folder2.gif";
                this.btnShowDetails.ToolTip = "Vai al dettaglio del fascicolo ";// +objectId;
                this.btnShowDetails.CommandName = "ProjectDetails";
            }
            else
            {
                this.btnShowDetails.ToolTip = "Vai al dettaglio del documento " + objectId;
                this.btnShowDetails.CommandName = "DocumentDetails";
            }

            // Url dell'immagine, tool tip e impostazione dell'URL da seguire per l'apertura del dettaglio
            this.btnShowDetails.PostBackUrl = navigationUrl;
            this.btnShowDocumentDetails.PostBackUrl = navigationUrl;
            this.btnShowDocumentDetails.CommandName = this.btnShowDetails.CommandName;
            this.btnShowDocumentDetails.ImageUrl = this.btnShowDetails.ImageUrl;
            this.btnShowDocumentDetails.ToolTip = this.btnShowDetails.ToolTip;

            // Se si è in area di lavoro, questo bottone deve essere utilizzato per visualizzare
            // il dettaglio della tramissione
            if (this.ParentPage == ParentPageEnum.SearchToDoList)
            {
                this.btnShowDetails.CommandName = "TransmissionDetails";
                this.btnShowDetails.ImageUrl = "~/images/proto/fulmine.gif";
                this.btnShowDetails.ToolTip = "Vai al dettaglio della trasmissione";
            }

        }

        /// <summary>
        /// Funzione per l'inizializzazione o l'update del bottone per
        /// inserimento / rimozione oggetto dall'area di lavoro
        /// </summary>
        private void InitializeOrUpdateWAButton(String objectId, string protoType)
        {
            // Se il documento / fascicolo è in ADL, viene mostrato il bottone di cancellazione
            // del documento / fascicolo dall'area di lavoro, altrimenti viene mostrata l'icona per 
            // l'inserimento
            //if (!string.IsNullOrEmpty(protoType) && protoType.Equals("C"))
            if (Request.QueryString.ToString().Contains("StampaRep") || Request.QueryString.ToString().Contains("StampaReg"))
                this.btnWorkingArea.Visible = false;
            else
            {
                this.btnWorkingArea.Visible = true;
                if (this.IsInWorkingArea)
                {
                    this.btnWorkingArea.ImageUrl = "~/images/proto/canc_area.gif";
                    if (this.ObjectType == ObjectTypeEnum.Document)
                    {
                        this.btnWorkingArea.ToolTip = "Elimina questo documento da Area di lavoro";
                        this.btnWorkingArea.CommandName = "RemoveDocumentFromWA";
                        this.btnWorkingArea.OnClientClick = String.Format("return removeFromWA('{0}', '{1}');", this.btnWorkingArea.ClientID, "documento");
                    }
                    else
                    {
                        this.btnWorkingArea.ToolTip = "Elimina questo fascicolo da Area di lavoro";
                        this.btnWorkingArea.CommandName = "RemoveProjectFromWA";
                        this.btnWorkingArea.OnClientClick = String.Format("return removeFromWA('{0}', '{1}');", this.btnWorkingArea.ClientID, "fascicolo");
                    }

                    this.btnWorkingArea.CommandArgument = objectId;
                }
                else
                {
                    this.btnWorkingArea.ImageUrl = "~/images/proto/ins_area.gif";
                    if (this.ObjectType == ObjectTypeEnum.Document)
                    {
                        this.btnWorkingArea.ToolTip = "Inserisci questo documento in Area di lavoro";
                        this.btnWorkingArea.CommandName = "InsertDocumentInWA";
                        this.btnWorkingArea.OnClientClick = String.Format("return insertInWA('{0}', '{1}');", this.btnWorkingArea.ClientID, "documento");
                    }
                    else
                    {
                        this.btnWorkingArea.ToolTip = "Inserisci questo fascicolo in Area di lavoro";
                        this.btnWorkingArea.CommandName = "InsertProjectInWA";
                        this.btnWorkingArea.OnClientClick = String.Format("return insertInWA('{0}', '{1}');", this.btnWorkingArea.ClientID, "fascicolo");
                    }

                    this.btnWorkingArea.CommandArgument = objectId;
                }
            }
        }

        /// <summary>
        /// Funzione per l'inizializzazione 
        /// del bottone per mostrare il file allegato al documento
        /// </summary>
        private void InitializeFileButton(String fileExtension, String objectId)
        {

            // Impostazione dell'icona del pulsante con l'immagine del'eventale documento
            // acquisito
            if (!String.IsNullOrEmpty(fileExtension))
            {
                this.btnShowDocumentFile.ImageUrl = FileManager.getFileIcon(Page, fileExtension.Trim());

                // Il link da visualizzare
                String link;

                // Creazione del link
                link = String.Format(
                    "../documento/Visualizzatore/VisualizzaFrame.aspx?docNumber={0}&idProfile={1}&numVersion={2}&versioneStampabile=false",
                    String.Empty,
                    objectId,
                    String.Empty);

                // Impostazione dell'evento OnClientClick del pulsante per la
                // visualizzazione del file associato al documento
                this.btnShowDocumentFile.OnClientClick = String.Format("openViewer('{0}');", link);
            }

        }

        /// <summary>
        /// Funzione per l'inizializzazione del bottone per mostrare
        /// i dettagli sulla fiema del documento
        /// </summary>
        private void InitializeSignButton(String objectId)
        {
            // Se il documento è di tipo R, viene visualizzato il messaggio
            // Tipologia documento non visualizzabile
            if (this.ProtoType != null && (this.ProtoType.Equals("R") || this.ProtoType.Equals("C")))
                this.btnSignInformation.OnClientClick = "alert('Tipologia documento non visualizzabile');";
            else
            {
                if (this.IsSigned)
                    this.btnSignInformation.OnClientClick = String.Format(
                        "showDialogSignInformation('{0}');",
                        objectId);
            }
        }

        /// <summary>
        /// Funzione per l'inizializzazione del bottone per la rimozione del
        /// documento da un fascicolo
        /// </summary>
        private void InitializeRemoveDocumentButton(String objectId)
        {
            // Impostazione delle icone e dei tool tip appropriati in base alla pagina
            // in cui è inserito il controllo
            if (this.ShowDocumentRemove)
            {
                this.btnRemoveDocumentFromProject.ToolTip = "Rimuovi il documento dal fascicolo";
                // this.btnRemoveDocumentFromProject.OnClientClick = String.Format(
                //     "return showDialogRemoveDocumentFromProject('{0}');", this.btnRemoveDocumentFromProject.ClientID);
                this.btnRemoveDocumentFromProject.ImageUrl = "~/images/proto/cancella.gif";
                this.btnRemoveDocumentFromProject.CommandName = "RemoveDocumentFromProject";
                this.btnRemoveDocumentFromProject.CommandArgument = objectId;
            }
        }

        /// <summary>
        /// Funzione per l'inizializzazione o l'aggiornamento del bottone per inserimento
        /// o rimozione del documento dall'area di conservazione
        /// </summary>
        private void InitializeOrUpdateSAButton(String objectId)
        {
            if (this.ShowStorageAreaButton)
            {
                if (this.Request.QueryString["newFasc"] == null || !this.Request.QueryString["newFasc"].Equals("1"))
                {
                    this.btnStorageArea.Visible = true;
                    // Se il documento è nell'area di conservazione, viene
                    // predisposta la maschera in modo che visualizzi il pulsante di
                    // cancellazione dell'oggetto dall'area di conservazione
                    if (this.IsInStorageArea)
                    {
                        this.btnStorageArea.ImageUrl = "~/images/proto/cancella.gif";
                        if (this.ObjectType == ObjectTypeEnum.Document)
                        {
                            this.btnStorageArea.ToolTip = "Elimina questo documento da Area di conservazione";
                            this.btnStorageArea.CommandName = "RemoveDocumentFromSA";
                            this.btnStorageArea.OnClientClick = String.Format("return showDialogRemoveFromSA('{0}','{1}');", this.btnStorageArea.ClientID, "documento");
                        }
                        else
                        {
                            this.btnStorageArea.ToolTip = "Elimina questo fascicolo da Area di conservazione";
                            this.btnStorageArea.CommandName = "RemoveProjectFromSA";
                            this.btnStorageArea.OnClientClick = String.Format("return showDialogRemoveFromSA('{0}','{1}');", this.btnStorageArea.ClientID, "fascicolo");
                        }

                        this.btnStorageArea.CommandArgument = objectId;

                    }
                    else
                    {
                        this.btnStorageArea.ImageUrl = "~/images/proto/conservazione_d.gif";
                        if (this.ObjectType == ObjectTypeEnum.Document)
                        {
                            this.btnStorageArea.ToolTip = "Inserisci questo documento in Area conservazione";
                            this.btnStorageArea.CommandName = "InsertDocumentInSA";
                            this.btnStorageArea.OnClientClick = String.Format("return showDialogInsertInSA('{0}','{1}');", this.btnStorageArea.ClientID, "documento");
                        }
                        else
                        {
                            this.btnStorageArea.ToolTip = "Inserisci questo fascicolo in Area conservazione";
                            this.btnStorageArea.CommandName = "InsertProjectInSA";
                            this.btnStorageArea.OnClientClick = String.Format("return showDialogInsertInSA('{0}','{1}');", this.btnStorageArea.ClientID, "fascicolo");
                        }

                        this.btnStorageArea.CommandArgument = objectId;
                    }



                }
                else
                {
                    this.btnStorageArea.Visible = false;
                }
            }
        }


        #endregion

        #region WA per documenti

        private void InsertDocumentInWA(String objectId)
        {
            // Scheda documento selezionata
            SchedaDocumento selectedDocument;

            // Recupero del dettaglio del documento
            selectedDocument = DocumentManager.getDettaglioDocumento(
                this.Page,
                objectId,
                String.Empty);

            // Aggiunta del documento all'area di lavoro
            DocumentManager.addAreaLavoro(this.Page, selectedDocument);

            // Il documento è nell'ADL
            this.IsInWorkingArea = true;

        }

        /// <summary>
        /// Rimozione del documento dall'area di lavoro
        /// </summary>
        private void RemoveDocumentFromWA(String objectId)
        {
            // Scheda del documento selezionato
            SchedaDocumento selectedDocument;

            // Recupero del documento
            selectedDocument = DocumentManager.getDettaglioDocumento(
                this.Page,
                objectId,
                String.Empty);

            // Eliminazione dall'ADL
            DocumentManager.eliminaDaAreaLavoro(this.Page, selectedDocument.systemId, null);

            // Il documento non è nell'area di lavoro
            this.IsInWorkingArea = false;

        }

        #endregion

        #region WA per i fascicoli

        /// <summary>
        /// Funzione per l'inserimento di un fascicolo nell'area di lavoro
        /// </summary>
        /// <param name="objectId">Id del fascicolo da spostare nell'area di lavoro</param>
        private void InsertProjectInWA(String objectId)
        {
            // Scheda del fascicolo selezionato
            Fascicolo project;

            // Recupero del dettaglio del fascicolo
            project = FascicoliManager.getFascicoloById(this.Page, objectId);

            // Aggiunta del fascicolo all'area di lavoro
            FascicoliManager.addFascicoloInAreaDiLavoro(this.Page, project);

            // Il fascicolo è nell'ADL
            this.IsInWorkingArea = true;
        }

        /// <summary>
        /// Funzione per la rimozione di un fascicolo dall'area di lavoro
        /// </summary>
        /// <param name="objectId">Id del fascicolo da rimuovere</param>
        private void RemoveProjectFromWA(String objectId)
        {
            // Scheda del fascicolo selezionato
            Fascicolo project;

            // Recupero del dettaglio del fascicolo
            project = FascicoliManager.getFascicoloById(this.Page, objectId);

            // Eliminazione dall'ADL
            DocumentManager.eliminaDaAreaLavoro(this.Page, null, project);

            // Il fascicolo non è nell'area di lavoro
            this.IsInWorkingArea = false;
        }

        #endregion

        #region SA per i documenti

        /// <summary>
        /// Funzione per la procedura di inserimento del documento in area di conservazione
        /// </summary>
        private String InsertDocumentInStorageArea(String documentID, String projectId)
        {
            // Eventuale messaggio da restituire
            String toReturn = String.Empty;

            // Dettaglio del documento selezionato
            SchedaDocumento selectedDocument = DocumentManager.getDettaglioDocumento(
                this.Page,
                documentID,
                String.Empty);

            // Il fascicolo selezionato
            if (String.IsNullOrEmpty(projectId))
            {
                Fascicolo f = FascicoliManager.getFascicoloSelezionato();
                if (f != null && String.IsNullOrEmpty(f.systemID))
                    projectId = string.Empty;
            }

            // Identificativo dell'istanza di certificazione
            String conservationAreaId;

            // Se il documento ha un file acquisito...
            if (Convert.ToInt32(selectedDocument.documenti[0].fileSize) > 0)
            {
                // Se si sta creando la prima istanza di conservazione...
                if (DocumentManager.isPrimaConservazione(
                    this.Page,
                    UserManager.getInfoUtente(Page).idPeople,
                    UserManager.getInfoUtente(Page).idGruppo) == 1)
                    // Viene visualizzato un messaggio all'utente
                    toReturn = "E' stata creata una nuova istanza di conservazione.";
                /*
                // Inserimento del documento nella conservazione e recupero dell'id dell'istanza
                if (projectId != null && this.ParentPage == ParentPageEnum.SearchDocumentInProject)
                    // Da ricerca documenti in fascicolo
                    conservationAreaId = DocumentManager.addAreaConservazione(
                        this.Page,
                        documentID,
                        projectId,
                        selectedDocument.docNumber,
                        UserManager.getInfoUtente(Page),
                        "D");
                else
                    conservationAreaId = DocumentManager.addAreaConservazione(
                        this.Page,
                        documentID,
                        "",
                        selectedDocument.docNumber,
                        UserManager.getInfoUtente(Page),
                        "D");
                */
                string tipo = string.Empty;
                if (!string.IsNullOrEmpty(projectId))
                {
                    tipo = "F";
                }
                else
                {
                    tipo = "D";
                }
                conservationAreaId = DocumentManager.addAreaConservazione(
                    this.Page,
                    documentID,
                    projectId,
                    selectedDocument.docNumber,
                    UserManager.getInfoUtente(Page),
                    tipo);

                // Se l'identificativo non è -1 vengono aggiornati i dati sulla conservazione
                if (conservationAreaId != "-1")
                {
                    int size_xml = DocumentManager.getItemSize(
                        this.Page,
                        selectedDocument,
                        conservationAreaId);

                    int doc_size = Convert.ToInt32(selectedDocument.documenti[0].fileSize);

                    int numeroAllegati = selectedDocument.allegati.Length;
                    string fileName = selectedDocument.documenti[0].fileName;
                    string tipoFile = Path.GetExtension(fileName);
                    int size_allegati = 0;
                    for (int i = 0; i < selectedDocument.allegati.Length; i++)
                    {
                        size_allegati = size_allegati + Convert.ToInt32(selectedDocument.allegati[i].fileSize);
                    }
                    int total_size = size_allegati + doc_size + size_xml;

                    DocumentManager.insertSizeInItemCons(Page, conservationAreaId, total_size);

                    DocumentManager.updateItemsConservazione(
                        this.Page,
                        tipoFile,
                        Convert.ToString(numeroAllegati),
                        conservationAreaId);

                    this.IsInStorageArea = true;
                }

            }
            else
                // Altrimenti viene mostrato un messaggio all'utente
                toReturn = "Il documento principale non ha alcun file associato, impossibile inserirlo in area conservazione";

            return toReturn;
        }

        /// <summary>
        /// Funzione per la rimozione del documento dall'area di conservazione
        /// </summary>
        private String RemoveDocumentFromStorageArea(String objectId)
        {
            // Eventuale messaggio da mostrare
            String toReturn = String.Empty;

            SchedaDocumento selectedDocument = DocumentManager.getDettaglioDocumento(
                this.Page,
                objectId,
                String.Empty);

            Fascicolo selectedProject;

            // Se il documento può essere rimosso dall'area di conservazione
            if ((DocumentManager.canDeleteAreaConservazione(
                this.Page,
                selectedDocument.systemId,
                UserManager.getInfoUtente(Page).idPeople,
                UserManager.getInfoUtente(Page).idGruppo) == 0))
                // Viene mostrato un messaggio all'utente
                toReturn = "Impossibile eliminare il documento da Area di conservazione";
            else
            {
                // Reperimento del fascicolo selezionato
                selectedProject = FascicoliManager.getFascicoloSelezionato();
                // Eliminazione del documento
                if (this.ParentPage == ParentPageEnum.SearchDocument || this.ParentPage == ParentPageEnum.SearchProject)
                    DocumentManager.eliminaDaAreaConservazione(
                        this.Page,
                        selectedDocument.systemId,
                        selectedProject,
                        null,
                        false,
                        String.Empty);

                // Il documento non è più in area di conservazione
                this.IsInStorageArea = false;
            }

            return toReturn;

        }

        #endregion

        #region SA per fascicoli

        /// <summary>
        /// Funzione per l'inserimento di documenti presenti in un fascicolo nell'area di
        /// conservazione
        /// </summary>
        /// <param name="objectId">Id del fascicolo di cui inserire i documenti nell'area di conservazione</param>
        /// <rereturns>Eventuale messaggio da mostrare all'utente</rereturns>
        private String InsertProjectInStorageArea(String objectId)
        {
            // Lista dei system id dei documenti contenuti nel fasciolo
            string[] idProjects = FascicoliManager.getIdDocumentiFromFascicolo(objectId);

            // Risultato da restituire
            StringBuilder toReturn = new StringBuilder();

            // Risultato di un inserimento
            String temp;

            // Se il fascicolo non contiene documenti, viene preparato un messaggio
            // apposito
            if (idProjects.Length == 0)
                toReturn = new StringBuilder("Il fascicolo non contiene nessun documento.");

            // Inserimento di tutti i documenti nell'area di conservazione
            foreach (String id in idProjects)
            {
                temp = this.InsertDocumentInStorageArea(id, objectId);

                if (!String.IsNullOrEmpty(temp))
                    toReturn.AppendLine(temp);
            }

            // Restituzione del risultato
            return toReturn.ToString();
 
        }

        /// <summary>
        /// Funzione per la rimozione dall'area di conservazione
        /// di documenti contenuti in un fascicolo
        /// </summary>
        /// <param name="objectId">Id del fascicolo di cui rimuovere i documenti</param>
        /// <returns>Eventuale messaggio da mostrare all'utente</returns>
        private String RemoveProjectFromStorageArea(String objectId)
        {
            // Scheda del fascicolo selezionato
            Fascicolo project;

            // Recupero del dettaglio del fascicolo
            project = FascicoliManager.getFascicoloById(this.Page, objectId);

            FascicoliManager.setFascicoloSelezionato(project);

            // Lista dei system id dei documenti contenuti nel fasciolo
            string[] idProjects = FascicoliManager.getIdDocumentiFromFascicolo(objectId);

            // Risultato da restituire
            StringBuilder toReturn = new StringBuilder();

            // Risultato di un inserimento
            String temp;

            // Inserimento di tutti i documenti nell'area di conservazione
            foreach (String id in idProjects)
            {
                temp = this.RemoveDocumentFromStorageArea(id);

                if (!String.IsNullOrEmpty(temp))
                    toReturn.AppendLine(temp);
            }

            // Restituzione del risultato
            return toReturn.ToString();

        }

        #endregion

        /// <summary>
        /// Questa funzione si occupa di visualizzare un messaggio all'utente
        /// </summary>
        /// <param name="message">Messaggio da visualizzare</param>
        private void ShowMessageToUser(String message)
        {
            // Riferimento all'alert ajax
            AjaxMessageBox messageBox;
            messageBox = this.Page.FindControl(this.AjaxAlertBoxID) as AjaxMessageBox;

            // Se l'alert box è stata reperita con successo, viene visualizzato il messaggio
            if(messageBox != null)
                messageBox.ShowMessage(message);
 
        }

        #region Proprietà di pagina

        /// <summary>
        /// Booleano che indica se il bottone dell'area di lavoro deve essere visualizzato.
        /// Il bottone sarà visibile solo se non si è in to do list
        /// </summary>
        protected bool ShowWAButton
        {
            get
            {
                return this.ParentPage != ParentPageEnum.SearchToDoList;
            }
        }

        /// <summary>
        /// Proprietà che indica se bisogna visualizzare il bottone i prima riga
        /// seconda cella per l'apertura del dettaglio dell'oggetto.
        /// Questo bottone sarà visualizzato solo se ci si trova nella To do List
        /// </summary>
        protected bool ShowDocumentDetailsFromToDoList
        {
            get 
            {
                return this.ParentPage == ParentPageEnum.SearchToDoList;
            }
        }

        /// <summary>
        /// Proprietà che indica se il pulsante con l'immagine del tipo documento acquisito
        /// deve essere visualizzato.
        /// Il bottone deve essere visibile solo se è presente e ad 1 l'impostazione GRD_VIS_UNIFICATA,
        /// se il documento ha una immagine acquisita e se l'oggetto legato a questo controllo è un 
        /// documento
        /// </summary>
        protected bool ShowFileImageButton
        {
            get
            {
                return !String.IsNullOrEmpty(ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"]) &&
                    ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] != "0" &&
                    !String.IsNullOrEmpty(this.FileExtension) &&
                    this.ObjectType == ObjectTypeEnum.Document;
            }
        }

        /// <summary>
        /// Booleano che indica se il bottone cancella documento da fascicolo
        /// deve essere visualizzato.
        /// Il bottone deve essere visualizzo solo se è vera almeno una delle seguenti due condizioni:
        /// - Ci si trova nella ricerca "Documenti in fascicolo", il ruolo è abilitato
        ///   alla funzione DO_DEL_DOC_FASC, !UserManager.disabilitaButtHMDiritti(access right del fascicolo
        ///   selezionato) il fascicolo selezionato non è Chiuso
        /// - Ci si trova in todolist ed il ruolo è abilitato a DO_PRO_RIMUOVI, l'oggetto è un documento
        ///   ed il documento è contenuto in almeno un fascicolo
        /// </summary>
        protected bool ShowRemoveFromProjectsButton
        {
            get
            {
                // Il fascicolo selezionato
                Fascicolo selectedProject = FascicoliManager.getFascicoloSelezionato();

                // Lista dei fascicoli in cui è contenuto il documento

                Fascicolo[] projects = null;

                if (this.ParentPage == ParentPageEnum.SearchToDoList)
                    projects = DocumentManager.GetFascicoliDaDoc(this.Page, this.ObjectId);

                return (this.ShowDocumentRemove &&
                    UserManager.ruoloIsAutorized(this.Page, "DO_DEL_DOC_FASC") &&
                    selectedProject != null &&
                    !UserManager.disabilitaButtHMDiritti(selectedProject.accessRights) &&
                    selectedProject.stato != "C")
                    || (this.ParentPage == ParentPageEnum.SearchToDoList &&
                    UserManager.ruoloIsAutorized(this.Page, "DO_PRO_RIMUOVI") &&
                    this.ObjectType == ObjectTypeEnum.Document) && projects != null &&
                    projects.Length > 0;
            }
        }

        /// <summary>
        /// Questa proprietà restituisce un booleano che indica se il bottone della
        /// conservazione deve essere visualizzato. Il bottone sarà visualizzato solo se 
        /// il ruolo è abilitato alla conservazione
        /// </summary>
        protected bool ShowStorageAreaButton
        {
            get
            {
                return UserManager.ruoloIsAutorized(this.Page, "DO_CONS");
            }

        }

        /// <summary>
        /// Pagina in cui sono inserite queste icone
        /// </summary>
        [DefaultValue(ParentPageEnum.SearchDocument)]
        public ParentPageEnum ParentPage { get; set; }

        /// <summary>
        /// Booleano che indica se il documento si trova nell'area di conservazione
        /// </summary>
        [DefaultValue(false)]
        public bool IsInStorageArea { get; set;}

        /// <summary>
        /// Estensione del file dell'immagine acquisita
        /// </summary>
        public String FileExtension { get; set; }

        /// <summary>
        /// Booleano che indica se il documento è firmato.
        /// </summary>
        [DefaultValue(false)]
        public bool IsSigned { get; set; }

        /// <summary>
        /// Booleano che indica se il documento è in Area di lavoro
        /// </summary>
        [DefaultValue(false)]
        public bool IsInWorkingArea { get; set; }

        /// <summary>
        /// Identificativo dell'oggetto legato a queste icone
        /// </summary>
        public String ObjectId { get; set; }

        /// <summary>
        /// Tipo di oggetto legato a questo controllo
        /// </summary>
        public ObjectTypeEnum ObjectType { get; set; }

        /// <summary>
        /// Url da seguire per la visualizzazione del dettaglio del documento
        /// </summary>
        public String NavigateUrl { get; set; }

        /// <summary>
        /// Tipo di protocollo.
        /// </summary>
        public String ProtoType { get; set; }

        /// <summary>
        /// Identificativo dell'alert ajax da utilizzare per visualizzare messaggi
        /// </summary>
        public String AjaxAlertBoxID { get; set; }

        /// <summary>
        /// True se bisogna mostrare il pulsante rimozione
        /// documenti in fascicolo
        /// </summary>
        [DefaultValue(false)]
        public bool ShowDocumentRemove { get; set; }

        public String GetInsertInWAClientID
        {
            get
            {
                return this.btnWorkingArea.ClientID;
            }
        }

        #endregion

    }

    /// <summary>
    /// Argomento da associare all'evento scatenato ogni qualvolta si effettua 
    /// una azione su di un elemento
    /// </summary>
    public class NewSearchIconsEventArgs : EventArgs
    {
        /// <summary>
        /// Id dell'oggetto che ha subito la modifica
        /// </summary>
        public String ObjectId { get; set; }

        /// <summary>
        /// True se l'operazione ha avuto esito positivo
        /// </summary>
        public bool Result { get; set; }
    }

}
