using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace SAAdminTool.UserControls
{
    public partial class IconeRicerca : System.Web.UI.UserControl
    {

        private bool elimina = false;


        protected void Page_Load(object sender, EventArgs e)
        {

            this.Response.Expires = -1;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            this.Inizialize();
        }

        protected void Inizialize()
        {
            string docNumber = this.DOC_NUMBER;

            #region contolli visualizzazione area conservazione
            if (!this.PAGINA_CHIAMANTE.Equals("toDoList"))
            {
                if (!UserManager.ruoloIsAutorized(this.Page, "DO_CONS"))
                {
                    this.btn_conservazione.Visible = false;
                }
            }
            else
            {
                this.btn_conservazione.Visible = false;
            }

            if (this.IN_CONSERVAZIONE != string.Empty && this.IN_CONSERVAZIONE == "1")
            {
                this.btn_conservazione.ImageUrl = "../images/proto/cancella.gif";
                this.btn_conservazione.ToolTip = "Elimina questo documento da Area di conservazione";
                this.btn_conservazione.CommandName = "EliminaAreaCons";
                this.btn_conservazione.OnClientClick = "return ApriModalDialogEliminaCons();";
            }
            else
            {
                if (this.IN_CONSERVAZIONE == "0")
                {
                    this.btn_conservazione.ImageUrl = "../images/proto/conservazione_d.gif";
                    this.btn_conservazione.ToolTip = "Inserisci questo documento in Area conservazione";
                    this.btn_conservazione.CommandName = "AreaConservazione";
                }
            }
            #endregion

            #region controlli per visualizzazione documento
            if ((System.Configuration.ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] != null) && (System.Configuration.ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] == "0"))
            {
                this.btn_visualizza.Visible = false;
            }
            if ((this.ACQUISITA_IMG != string.Empty && this.ACQUISITA_IMG == "0") || (!string.IsNullOrEmpty(this.IS_DOC_OR_FASC) && this.IS_DOC_OR_FASC.StartsWith("Fasc")))
            {
                this.btn_visualizza.Visible = false;
            }
            else
            {
                if (this.ACQUISITA_IMG != string.Empty)
                {
                    string image = FileManager.getFileIcon(Page, this.ACQUISITA_IMG.Trim().ToLower());
                    this.btn_visualizza.ImageUrl = image;
                }
            }
            //modifica
            //SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            //string new_doc_number = docNumber.Split('.')[docNumber.Split('.').Length - 1];
            //SAAdminTool.DocsPaWR.InfoFileCaching info = ws.massimaVersioneDelDocumento(new_doc_number);
            //if (info != null)
            //{
            //    string image = FileManager.getFileIcon(Page, info.ext.Trim().ToLower());
            //    this.btn_visualizza.Visible = true;
            //    this.btn_visualizza.ImageUrl = image;
            //}
            //fine modifica
            #endregion


            #region controlli per bottone firma
            if (string.IsNullOrEmpty(this.FIRMATO) || this.FIRMATO == "0" || (!string.IsNullOrEmpty(this.IS_DOC_OR_FASC) && this.IS_DOC_OR_FASC.StartsWith("Fasc")))
            {
                this.btn_firmato.Visible = false;
            }
            #endregion

            #region controlli per visualizzazione area di lavoro
            if (!this.PAGINA_CHIAMANTE.Equals("toDoList"))
            {
                if (this.IN_ADL != string.Empty && this.IN_ADL == "1")
                {
                    this.btn_adl.ImageUrl = "../images/proto/cancella.gif";
                    this.btn_adl.ToolTip = "Elimina questo documento da Area di lavoro";
                    this.btn_adl.CommandName = "EliminaADL";
                    this.btn_adl.OnClientClick = "return ApriModalDialogNewADL();";
                }
                else
                {
                    if (this.IN_ADL == "0")
                    {
                        this.btn_adl.ImageUrl = "../images/proto/ins_area.gif";
                        this.btn_adl.ToolTip = "Inserisci questo documento in Area di lavoro";
                        this.btn_adl.CommandName = "inserisciAdl";
                        this.btn_adl.OnClientClick = "return ApriModalDialog();";
                    }
                }
            }
            else
            {
                this.btn_adl.Visible = false;
            }
            #endregion


            #region controlli visualizzazione bottone elimina
            if (this.PAGINA_CHIAMANTE == "NewDocListInProject")
            {
                if (UserManager.ruoloIsAutorized(this, "DO_DEL_DOC_FASC"))
                {
                    this.btn_eliminaDocInFasc.Visible = true;
                }
                else
                {
                    this.btn_eliminaDocInFasc.Visible = false;
                }

                DocsPaWR.Fascicolo Fasc = FascicoliManager.getFascicoloSelezionato(Page);
                if (Fasc != null && Fasc.accessRights != null && Fasc.accessRights != "")
                {
                    if (UserManager.disabilitaButtHMDiritti(Fasc.accessRights))
                    {
                        this.btn_eliminaDocInFasc.Visible = false;
                    }
                }
                if (Fasc != null && Fasc.stato == "C")
                {
                    this.btn_eliminaDocInFasc.Visible = false;
                }
                this.btn_eliminaDocInFasc.ToolTip = "Rimuovi il documento dal fascicolo";
                this.btn_eliminaDocInFasc.OnClientClick = "return showModalDialogEliminaDocInFasc();";



                //Hashtable hashDoc = FascicoliManager.getHashDocProtENonProt(Page);

                //if (hashDoc != null)
                //{
                //    if (!string.IsNullOrEmpty(this.ID_PROFILE))
                //    { 
                //        int numPage = 1;
                //        int numTotPage = 0;
                //        int nRec = 0;

                //        DocsPaWR.Folder fold = FascicoliManager.getFolderSelezionato(Page);
                //       // DocsPaWR.InfoDocumento[] listaDoc = FascicoliManager.getListaDocumentiPaging(Page, fold, numPage, out numTotPage, out nRec);
                //        //if (listaDoc != null)
                //        //{
                //            if ((hashDoc.Count < 2) &&
                //                (ConfigSettings.getKey(ConfigSettings.KeysENUM.FASC_RAPIDA_REQUIRED).ToUpper().Equals("TRUE")))
                //            {
                //                elimina = false;
                //                this.btn_eliminaDocInFasc.OnClientClick = "return showModalDialogNonEliminareFascicolo();";
                //            }
                //            else
                //            {
                //                elimina = true;
                //                this.btn_eliminaDocInFasc.OnClientClick = "return showModalDialogEliminaDocInFasc();";
                //            }
                //        //}
                //        //else
                //        //{
                //        //    elimina = true;
                //        //    this.btn_eliminaDocInFasc.OnClientClick = "return showModalDialogEliminaDocInFasc();";
                //        //}
                //    }
                //    else
                //    {
                //        elimina = true;
                //        this.btn_eliminaDocInFasc.OnClientClick = "return showModalDialogEliminaDocInFasc();";
                //    }
                //}
                //else
                //{
                //    elimina = true;
                //    this.btn_eliminaDocInFasc.OnClientClick = "return showModalDialogEliminaDocInFasc();";
                //}
                this.btn_eliminaDocInFasc.ImageUrl = "../images/proto/cancella.gif";
                this.btn_eliminaDocInFasc.CommandName = "eliminaDocInFasc";
            }
            else
            {
                if (this.PAGINA_CHIAMANTE == "toDoList")
                {
                    if (UserManager.ruoloIsAutorized(Page, "DO_DOC_TDL_RIMUOVI"))
                    {
                        this.btn_eliminaDocInFasc.Visible = true;
                        if (!this.IS_DOC_OR_FASC.StartsWith("Id"))
                        {
                            this.btn_eliminaDocInFasc.Visible = false;
                        }
                        else
                            this.btn_eliminaDocInFasc.Visible = true;
                    }
                    else
                    {
                        this.btn_eliminaDocInFasc.Visible = false;
                    }
                    this.btn_eliminaDocInFasc.ImageUrl = "../images/proto/b_elimina.gif";
                    this.btn_eliminaDocInFasc.CommandName = "eliminaDaTodoList";
                }
                else
                {
                    this.btn_eliminaDocInFasc.Visible = false;
                }
            }
            #endregion

            #region contolli per dettaglio documento
            if (this.PAGINA_CHIAMANTE.Equals("toDoList"))
            {
                this.btn_dettaglio.CommandName = "dettaglio";
                //this.btn_dettaglio.ToolTip = this.IS_DOC_OR_FASC;
                this.btn_dettaglio.ImageUrl = "../images/proto/fulmine.gif";
                this.btn_dettaglio.ToolTip = "Vai al dettaglio della trasmissione";
            }
            #endregion

            #region controlli per bottone scheda
            if (this.PAGINA_CHIAMANTE.Equals("toDoList"))
            {
                this.btn_scheda.Visible = true;
                //this.btn_scheda.ToolTip = this.IS_DOC_OR_FASC;
                this.btn_scheda.ToolTip = "Vai al dettaglio del documento " + this.IS_DOC_OR_FASC;
                if (this.IS_DOC_OR_FASC.StartsWith("Fasc"))
                {
                    this.btn_scheda.ImageUrl = "../images/proto/folder2.gif";
                    this.btn_scheda.ToolTip = "Vai al dettaglio del fascicolo " + this.IS_DOC_OR_FASC;
                }
            }
            #endregion
        }

        protected void btn_conservazione_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (btn_conservazione.CommandName.Equals("AreaConservazione"))
                {

                    SAAdminTool.DocsPaWR.SchedaDocumento schedaSel = DocumentManager.getDettaglioDocumento(Page, this.ID_PROFILE, this.DOC_NUMBER);

                    if (Convert.ToInt32(schedaSel.documenti[0].fileSize) > 0)
                    {
                        int isPrimaIstanza = DocumentManager.isPrimaConservazione(Page, UserManager.getInfoUtente(Page).idPeople, UserManager.getInfoUtente(Page).idGruppo);
                        if (isPrimaIstanza == 1)
                        {
                            string popup = "<script> alert('Si sta per creare una nuova istanza di conservazione')</script>";
                            Page.RegisterClientScriptBlock("popUp", popup);
                        }
                        DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato();
                        string sysId = string.Empty;
                        //se sto inserendo un documento dall'interno di un fascicolo (da tabFascListaDoc.aspx)
                        if (fasc != null && this.PAGINA_CHIAMANTE == "NewDocListInProject")
                        {
                            sysId = DocumentManager.addAreaConservazione(Page, this.ID_PROFILE, fasc.systemID, this.DOC_NUMBER, UserManager.getInfoUtente(Page), "D");
                        }
                        else
                        {
                            sysId = DocumentManager.addAreaConservazione(Page, this.ID_PROFILE, "", this.DOC_NUMBER, UserManager.getInfoUtente(Page), "D");
                        }

                        if (sysId != "-1")
                        {
                            int size_xml = DocumentManager.getItemSize(Page, schedaSel, sysId);
                            int doc_size = Convert.ToInt32(schedaSel.documenti[0].fileSize);

                            int numeroAllegati = schedaSel.allegati.Length;
                            string fileName = schedaSel.documenti[0].fileName;
                            string tipoFile = System.IO.Path.GetExtension(fileName);

                            int size_allegati = 0;
                            for (int i = 0; i < schedaSel.allegati.Length; i++)
                            {
                                size_allegati = size_allegati + Convert.ToInt32(schedaSel.allegati[i].fileSize);
                            }
                            int total_size = size_allegati + doc_size + size_xml;

                            DocumentManager.insertSizeInItemCons(Page, sysId, total_size);

                            DocumentManager.updateItemsConservazione(Page, tipoFile, Convert.ToString(numeroAllegati), sysId);


                            this.IN_CONSERVAZIONE = "1";
                        }

                    }
                    else
                    {
                        string popup = "<script> alert('Il documento principale non ha alcun file associato, impossibile inserirlo in area conservazione')</script>";
                        Page.RegisterClientScriptBlock("popUp", popup);
                    }

                }//se elimino dall'area di conservazione
                else
                {
                    SAAdminTool.DocsPaWR.SchedaDocumento schedaSel = DocumentManager.getDettaglioDocumento(Page, this.ID_PROFILE, this.DOC_NUMBER);

                    if (DocumentManager.canDeleteAreaConservazione(Page, schedaSel.systemId, UserManager.getInfoUtente(Page).idPeople, UserManager.getInfoUtente(Page).idGruppo) == 0)
                    {
                        string popup = "<script> alert('Impossibile eliminare il documento da Area di conservazione')</script>";
                        Page.RegisterClientScriptBlock("alert", popup);
                    }
                    else
                    {
                        //DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato();
                        if (this.PAGINA_CHIAMANTE == "tabRisultatiRicDoc")
                        {
                            DocumentManager.eliminaDaAreaConservazione(Page, schedaSel.systemId, null, null, false, "");
                        }
                        else
                        {
                            DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato();
                            DocumentManager.eliminaDaAreaConservazione(Page, schedaSel.systemId, fasc, null, false, "");
                        }
                        this.IN_CONSERVAZIONE = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(Page, ex);
            }
        }


        protected void btn_adl_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (btn_adl.CommandName.Equals("inserisciAdl"))
                {
                    SAAdminTool.DocsPaWR.SchedaDocumento schedaSel = DocumentManager.getDettaglioDocumento(Page, this.ID_PROFILE, this.DOC_NUMBER);

                    //se ho attiva la nuova ADL devo invertire la funzionalità
                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    {
                        DocumentManager.eliminaDaAreaLavoro(Page, schedaSel.systemId, null);
                        //riavvio la ricerca
                        string fromPage = Request.QueryString["from"].ToString();
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "LanciaRic", "top.principale.document.location = 'gestioneRicDoc.aspx?tab=" + fromPage + "&ricADL=1&from=" + fromPage + "';", true);

                    }
                    else //normale comportamento
                    {
                        DocumentManager.addAreaLavoro(Page, schedaSel);

                        this.IN_ADL = "1";

                        if (Session["listInArea"] != null)
                        {
                            SAAdminTool.DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaSel);
                            System.Collections.Hashtable listInArea = (Hashtable)Session["listInArea"];
                            if (listInArea != null && listInArea.ContainsKey(this.DOC_NUMBER) == false)
                                listInArea.Add(this.DOC_NUMBER, infoDoc);

                            Session["listInArea"] = listInArea;
                        }
                        else
                        {
                            SAAdminTool.DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaSel);
                            System.Collections.Hashtable listInArea = new Hashtable();
                            listInArea.Add(infoDoc.numProt + infoDoc.dataApertura, infoDoc);
                            Session["listInArea"] = listInArea;
                        }
                    }
                }//se elimino dall'area di lavoro
                else
                {
                    SAAdminTool.DocsPaWR.SchedaDocumento schedaSel = DocumentManager.getDettaglioDocumento(Page, this.ID_PROFILE, this.DOC_NUMBER);
                    DocumentManager.eliminaDaAreaLavoro(Page, schedaSel.systemId, null);

                    //se ho attiva la nuova ADL devo invertire la funzionalità
                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    {
                        //string fromPage = Request.QueryString["from"].ToString();
                        SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
                        string fromPage = (string)currentContext.QueryStringParameters["from"];
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "LanciaRic", "top.principale.document.location = 'gestioneRicDoc.aspx?tab=" + fromPage + "&ricADL=1&from=" + fromPage + "';", true);

                    }
                    this.IN_ADL = "0";
                    if (Session["listInArea"] != null)
                    {
                        SAAdminTool.DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaSel);
                        System.Collections.Hashtable listInArea = (Hashtable)Session["listInArea"];
                        if (listInArea.ContainsKey(this.DOC_NUMBER) == false)
                            listInArea.Add(this.DOC_NUMBER, infoDoc);

                        Session["listInArea"] = listInArea;
                    }
                    else
                    {
                        SAAdminTool.DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaSel);
                        System.Collections.Hashtable listInArea = new Hashtable();
                        listInArea.Add(infoDoc.numProt + infoDoc.dataApertura, infoDoc);
                        Session["listInArea"] = listInArea;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(Page, ex);
            }
        }


        protected void btn_visualizza_Click(object sender, ImageClickEventArgs e)
        {
            //vis unificata
            try
            {
                if (!this.PAGINA_CHIAMANTE.Equals("toDoList"))
                {
                    #region Vecchio visualizzatore
                    //SAAdminTool.DocsPaWR.SchedaDocumento schedaSel = DocumentManager.getDettaglioDocumento(Page, this.ID_PROFILE, this.DOC_NUMBER);
                    //if (schedaSel.documenti[0].fileSize != "" && schedaSel.documenti[0].fileSize != "0")
                    //{
                    //    DocumentManager.setDocumentoSelezionato(Page, schedaSel);
                    //    FileManager.setSelectedFile(Page, schedaSel.documenti[0], false);
                    //    Page.ClientScript.RegisterStartupScript(this.GetType(), "lanciaVIs", "loadvisualizzaDoc('" + Session.SessionID + "','" + this.DOC_NUMBER + "','" + this.ID_PROFILE + "');", true);
                    //}
                    #endregion
                    #region Nuovo visualizzatore

                    string src = String.Format(
                            "{0}/documento/Visualizzatore/VisualizzaFrame.aspx?docNumber={1}&idProfile={2}&numVersion={3}",
                            Utils.getHttpFullPath(Page),
                            this.DOC_NUMBER,
                            this.ID_PROFILE,
                            String.Empty);

                    // Si visualizza il documento
                    Page.ClientScript.RegisterStartupScript(this.GetType(),
                        "vis", "OpenNewViewer('" + src + "');", true);

                    #endregion
                }
                else
                {
                    #region Vecchio visualizzatore
                    //string docNumberAppo = this.DOC_NUMBER.Substring(this.DOC_NUMBER.IndexOf(":") + 1);
                    //string docNumber = docNumberAppo.Trim();
                    //SAAdminTool.DocsPaWR.SchedaDocumento schedaSel = DocumentManager.getDettaglioDocumento(Page, this.ID_PROFILE, docNumber);
                    //if (schedaSel.documenti[0].fileSize != "" && schedaSel.documenti[0].fileSize != "0")
                    //{
                    //    DocumentManager.setDocumentoSelezionato(Page, schedaSel);
                    //    FileManager.setSelectedFile(Page, schedaSel.documenti[0], false);
                    //    Page.ClientScript.RegisterStartupScript(this.GetType(), "lanciaVIs", "loadvisualizzaDoc('" + Session.SessionID + "','" + docNumber + "','" + this.ID_PROFILE + "');", true);
                    //}
                    #endregion
                    #region Nuovo visualizzatore

                    string src = String.Format(
                                "{0}/documento/Visualizzatore/VisualizzaFrame.aspx?docNumber={1}&idProfile={2}&numVersion={3}",
                                Utils.getHttpFullPath(Page),
                                String.Empty,
                                this.ID_PROFILE,
                                String.Empty);

                    // Si visualizza il documento
                    Page.ClientScript.RegisterStartupScript(this.GetType(),
                        "vis", "OpenNewViewer('" + src + "');", true);


                    #endregion
                }
            }
            catch (Exception ex)
            {
                Exception eApp = null;
                var err = "";
                string PagChiamante = "RicercaDoc";
                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    PagChiamante = "RicercaADLDoc";
                err = string.Format("{0} \r\n {1} \r\n {2}", "Pagina chiamante: " + PagChiamante, "Errore: " + ex.Message.ToString(), "StackTrace: " + ex.StackTrace.ToString());
                eApp = new Exception(err);

                //ErrorManager.OpenErrorPage(this, err,PagChiamante);
                ErrorManager.redirectToErrorPage(this.Page, eApp);
            }
        }

        protected void btn_firmato_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string errorMessage = string.Empty;
                SAAdminTool.DocsPaWR.SchedaDocumento schedaSel = new SAAdminTool.DocsPaWR.SchedaDocumento();
                if (!this.PAGINA_CHIAMANTE.Equals("toDoList"))
                    schedaSel = DocumentManager.getDettaglioDocumento(Page, this.ID_PROFILE, this.DOC_NUMBER);
                else
                {
                    string docNumberAppo = this.DOC_NUMBER.Substring(this.DOC_NUMBER.IndexOf(":") + 1);
                    string docNumber = docNumberAppo.Trim();
                    schedaSel = DocumentManager.getDettaglioDocumento(Page, this.ID_PROFILE, docNumber);
                }
                SAAdminTool.DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaSel);
                if (infoDoc.tipoProto.Equals("R") || infoDoc.tipoProto.Equals("C"))
                {
                    this.RegisterClientScript("DocumentoTipoStampaRegistro",
                        "alert('Tipologia documento non visualizzabile');");
                }
                else
                {
                    int retValue = DocumentManager.verificaACL("D", this.ID_PROFILE, UserManager.getInfoUtente(), out errorMessage);
                    if (retValue == 0 || retValue == 1)
                    {
                        string script = ("<script>alert('" + errorMessage + "');</script>");
                        Response.Write(script);
                    }
                    else
                    {
                        DocumentManager.setRisultatoRicerca(Page, infoDoc);
                        DocumentManager.setDocumentoSelezionato(Page, schedaSel);
                        //FileManager.setSelectedFile(Page, schedaSel.documenti[0]);
                        Page.Session["FileManager.selectedFile"] = schedaSel.documenti[0];
                        if (this.FIRMATO == "1")
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "firma", "ShowMaskDettagliFirma();", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(Page, ex);
            }
        }

        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        protected void btn_eliminaDocInFasc_Click(object sender, ImageClickEventArgs e)
        {
            switch (this.PAGINA_CHIAMANTE)
            {
                case "NewDocListInProject":


                    Hashtable hashDoc = FascicoliManager.getHashDocProtENonProt(Page);

                    DocsPaWR.Folder fold = FascicoliManager.getFolderSelezionato(Page);
                    string msg = string.Empty;
                    string valoreChiaveFasc = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
                    if (string.IsNullOrEmpty(valoreChiaveFasc))
                        valoreChiaveFasc = "false";
                    SAAdminTool.DocsPaWR.ValidationResultInfo result = FascicoliManager.deleteDocFromFolder(Page, fold, this.ID_PROFILE, valoreChiaveFasc, out msg);
                    if (result != null && result.BrokenRules.Length > 0)
                    {
                        SAAdminTool.DocsPaWR.BrokenRule br = (SAAdminTool.DocsPaWR.BrokenRule)result.BrokenRules[0];
                        Response.Write("<script>alert('" + br.Description + "')</script>");
                        return;
                    }
                    if (msg != string.Empty)
                    {
                        Response.Write("<script>alert('" + msg + "')</script>");
                        return;
                    }
                    // rimuove la sessione della chiave che specifica il record da eliminare
                    Page.Session.Remove("key");
                    
                    break;
            }
        }





        public string IN_ADL
        {
            get
            {
                return this.GetStateValue("IN_ADL");
            }
            set
            {
                this.SetStateValue("IN_ADL", value);
            }
        }

        public string IN_CONSERVAZIONE
        {
            get
            {
                return this.GetStateValue("IN_CONSERVAZIONE");
            }
            set
            {
                this.SetStateValue("IN_CONSERVAZIONE", value);
            }
        }

        public string FIRMATO
        {
            get
            {
                return this.GetStateValue("FIRMATO");
            }
            set
            {
                this.SetStateValue("FIRMATO", value);
            }
        }

        public string ACQUISITA_IMG
        {
            get
            {
                return this.GetStateValue("ACQUISITA_IMG");
            }
            set
            {
                this.SetStateValue("ACQUISITA_IMG", value);
            }
        }

        public string ID_PROFILE
        {
            get
            {
                return this.GetStateValue("ID_PROFILE");
            }
            set
            {
                this.SetStateValue("ID_PROFILE", value);
            }
        }

        public string DOC_NUMBER
        {
            get
            {
                return this.GetStateValue("DOC_NUMBER");
            }
            set
            {
                this.SetStateValue("DOC_NUMBER", value);
            }
        }

        public string PAGINA_CHIAMANTE
        {
            get
            {
                return this.GetStateValue("PAGINA_CHIAMANTE");
            }
            set
            {
                this.SetStateValue("PAGINA_CHIAMANTE", value);
            }
        }

        public string IS_DOC_OR_FASC
        {
            get
            {
                return this.GetStateValue("IS_DOC_OR_FASC");
            }
            set
            {
                this.SetStateValue("IS_DOC_OR_FASC", value);
            }
        }

        protected string GetStateValue(string key)
        {
            if (this.ViewState[key] != null)
                return this.ViewState[key].ToString();
            else
                return string.Empty;
        }

        protected void SetStateValue(string key, string obj)
        {
            this.ViewState[key] = obj;
        }

        /// <summary>
        /// Proprietà per indicare se l'utente ha i diritti di visibilità
        /// sull'oggetto della trasmissione
        /// </summary>
        public bool HaveVisibilityRights
        {
            set
            {
                this.btn_visualizza.Visible = value;
            }

        }

    }
}
