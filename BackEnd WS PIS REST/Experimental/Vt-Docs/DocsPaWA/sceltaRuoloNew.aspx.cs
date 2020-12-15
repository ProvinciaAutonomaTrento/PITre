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


namespace DocsPAWA
{
    /// <summary>
    /// versione 1.1
    /// </summary>
    public class sceltaRuoloNew : DocsPAWA.CssPage
    {
        //my objects
        DocsPaWR.Utente userHome;
        DocsPaWR.Ruolo userRuolo;
        protected System.Web.UI.WebControls.Image Image1;
        protected System.Web.UI.WebControls.Label lbl_cod;
        protected System.Web.UI.WebControls.Label DoveSonoLabel;
        protected System.Web.UI.WebControls.Label lbl_DoveSono;
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.Label Label2;
        protected System.Web.UI.WebControls.Label titolo;
        //protected System.Web.UI.WebControls.DropDownList DDLOggettoTab1;
        protected System.Web.UI.WebControls.Button btnCercaTrasmissioni;
        protected System.Web.UI.WebControls.Button btnShowFilters;
        protected System.Web.UI.WebControls.Button btnRemoveFilters;
        protected DocsPaWebCtrlLibrary.ImageButton btn_all_todolist;
        protected System.Web.UI.WebControls.DropDownList chklst_ruoli;
        protected System.Web.UI.WebControls.Label lbl_descr;
        protected System.Web.UI.WebControls.Label lbl_listaUO;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_AutoToDoListValue;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_performSearchToDoList;
        protected Utilities.MessageBox msg_delega;
        protected int indiceUO;
        protected DocsPAWA.DocsPaWR.Ruolo precRuolo;
        protected System.Web.UI.WebControls.ImageButton btn_pred_todolist;
        protected DocsPAWA.DocsPaWR.Ruolo userRole;
        public DocsPAWA.ricercaDoc.SchedaRicerca schedaRicerca = null;
        private const string KEY_SCHEDA_RICERCA = "RicercaDocEstesa";
        protected DocsPAWA.UserControls.NotificationCenterItemList ncItems;
        //end my obj

        private void Page_Load(object sender, System.EventArgs e)
        {
            //Utils.startUp(this);
            Response.Expires = -1;
            userHome = UserManager.getUtente(this);

            // cancello tutte le variabili di sessione risettando solo la userHome 
            //e userRuol(miglioramento per non dover selezionare sempre il ruolo dalla combo)
            Utils.RemoveDataSession(this);

            UserManager.setUtente(this, userHome);

            if (!IsPostBack && userHome != null)
            {
                // Impostazione tipo trasmissione
                this.SetTipoTrasmissione();

                this.btnShowFilters.Attributes.Add("onClick", "return ShowFiltersDialog();");

                setInfoRuolo();

                chklst_ruoli.SelectedIndex = 0;
                if (Session["userRuolo"] != null)
                {
                    string valore = ((DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"]).systemId;
                    this.chklst_ruoli.SelectedIndex = chklst_ruoli.Items.IndexOf(chklst_ruoli.Items.FindByValue(valore));
                }
                //Session["Tipo_obj"] = DDLOggettoTab1.SelectedItem.Value;
                aggiornaDatiRuolo(chklst_ruoli.SelectedIndex);
                if (this.AutoToDoList())
                {
                    aggiornaToDoList();
                    this.hd_AutoToDoListValue.Value = "true";
                }
                else
                {
                    this.hd_AutoToDoListValue.Value = "false";
                }

                // Visualizzazione degli item del centro notifiche
                this.ncItems.Visible = true;

                /*
                 * MA STA COSA QUA SOTTO PERCHE'?
                
                btnRemoveFilters.Visible = false;
                DocumentManager.removeFiltroRicTrasm(this);

                // Rimozione filtri su ricerca trasmissioni in todolist
                ricercaTrasm.DialogFiltriRicercaTrasmissioni.RemoveCurrentFilters();

                //if (DelegheManager.VerificaDelega(this) > 0)
                //{
                //    string messaggio = InitMessageXml.getInstance().getMessage("ESERCITA_DELEGA");
                //    msg_delega.Confirm(messaggio);
                //}
                 *
                 * */
            }
            else
            {
                if ((System.Configuration.ConfigurationManager.AppSettings["AUTO_TO_DO_LIST"] != null) && (System.Configuration.ConfigurationManager.AppSettings["AUTO_TO_DO_LIST"] == "1"))
                {
                    if (Session["newRuolo"] != null)
                    {
                        userRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["newRuolo"];
                        string valore = ((DocsPAWA.DocsPaWR.Ruolo)Session["newRuolo"]).systemId;
                        this.chklst_ruoli.SelectedIndex = chklst_ruoli.Items.IndexOf(chklst_ruoli.Items.FindByValue(valore));
                        Session.Remove("newRuolo");
                        btnCercaTrasmissioni_Click(null, null);
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_PreRender(object sender, System.EventArgs e)
        {
            this.EnableFilterButtons();
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chklst_ruoli.SelectedIndexChanged += new System.EventHandler(this.chklst_ruoli_SelectedIndexChanged);
            this.btnCercaTrasmissioni.Click += new System.EventHandler(this.btnCercaTrasmissioni_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(this.Page_PreRender);
            this.btn_all_todolist.Click += new System.Web.UI.ImageClickEventHandler(this.btn_all_todolist_Click);
            //this.msg_delega.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_delega_GetMessageBoxResponse);
            this.btn_pred_todolist.Click += new ImageClickEventHandler(btn_pred_todolist_Click);
        }
        #endregion

        private void setInfoRuolo()
        {
            if (userHome != null)
            {
                if (userHome.ruoli != null)
                {
                    for (int i = 0; i < userHome.ruoli.Length; i++)
                    {
                        // caricamento della combobox dei ruoli dell'utente loggato
                        ListItem item = new ListItem(((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[i]).descrizione.ToString(), ((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[i]).systemId.ToString());
                        //chklst_ruoli.Items.Add(((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[i]).descrizione);
                        this.chklst_ruoli.Items.Add(item);
                    }

                    if (userHome.ruoli.Length > 0)
                    {
                        userRuolo = ((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[0]);
                    }
                    if (userHome.ruoli.Length > 1)
                    {
                        //Ricerca degli elementi presenti e non letti per ogni ruolo ricoperto dall'utente
                        //Richiamo webmetodo che mi restituisce un oggettino con per ogni ruolo
                        //lista documenti visti, non letti; lista fascicoli visti, non letti; tutti i doc
                        //if (TrasmManager.getAllTodoList(userHome, userRuolo) && (System.Configuration.ConfigurationManager.AppSettings["AUTO_TO_DO_LIST"] != null) && (System.Configuration.ConfigurationManager.AppSettings["AUTO_TO_DO_LIST"] == "1"))
                        if ((System.Configuration.ConfigurationManager.AppSettings["AUTO_TO_DO_LIST"] != null) && (System.Configuration.ConfigurationManager.AppSettings["AUTO_TO_DO_LIST"] == "1"))
                            btn_all_todolist.Visible = true;
                        else
                            btn_all_todolist.Visible = false;
                    }  
                    if (userRuolo != null && TrasmManager.getPredInTodoList(userHome, userRuolo) && (System.Configuration.ConfigurationManager.AppSettings["PRED_IN_TO_DO_LIST"] != null) && (System.Configuration.ConfigurationManager.AppSettings["PRED_IN_TO_DO_LIST"] == "1"))
                            btn_pred_todolist.Visible = true;
                        else
                            btn_pred_todolist.Visible = false;
                    //}
                }

            }
        }

        void btn_pred_todolist_Click(object sender, ImageClickEventArgs e)
        {
            //nel caso in cui si selezioni il pulsante, si vuole arrivare alla home page
            //già impostata con il filtro sui documenti predisposti
            // 1) impostare i filtri di ricerca
            DocsPaWR.FiltroRicerca[][] qV;
            DocsPaWR.FiltroRicerca fV1;
            DocsPaWR.FiltroRicerca[] fVList;
            qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
            qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
            fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
            fV1.valore = "false";
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
            fV1.valore = "false";
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
            fV1.valore = "false";
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
            fV1.valore = "false";
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
            fV1.valore = "true";
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

            //fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            //fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
            //fV1.valore = userHome.descrizione;
            //fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

            // 2) inserire in sessione i filtri impostati
            qV[0] = fVList;
            DocumentManager.setFiltroRicDoc(this, qV);
            if (Session["userRuolo"] != null)
                userRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
            else
            {
                if (userHome != null)
                {
                    if (userHome.ruoli != null)
                    {
                        if (userHome.ruoli.Length > 0)
                        {
                            userRuolo = ((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[0]);
                        }
                    }
                }
            }
            schedaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca(KEY_SCHEDA_RICERCA, userHome, userRuolo, this);
            schedaRicerca.FiltriRicerca = qV;
            Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY] = schedaRicerca;

            // 3) rimandare alla home page
            Session["PredispostiInToDoList"] = true;
            Response.Write("<SCRIPT>try { top.principale.document.location='RicercaDoc/gestioneRicDoc.aspx?tab=estesa'; } catch(e) {try { top.principale.iFrame_dx.document.location='RicercaDoc/gestioneRicDoc.aspx?tab=estesa'; } catch(e) {}}</SCRIPT>");
        }

        private void chklst_ruoli_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            aggiornaDatiRuolo(chklst_ruoli.SelectedIndex);

            // Visualizzazione della lista di item del centro notifiche
            this.ncItems.Visible = true;

            // Rimozione filtri su ricerca trasmissioni in todolist
            ricercaTrasm.DialogFiltriRicercaTrasmissioni.RemoveCurrentFilters();

            if (this.AutoToDoList())
                this.aggiornaToDoList();
            else
                Response.Write("<SCRIPT>top.principale.iFrame_dx.document.location='blank_page.htm';</SCRIPT>");

        }

        private void aggiornaDatiRuolo(int index)
        {
            /*
            * GESTIONE DELLA SESSIONE:
            * -----------------------------------------------------------------------------
            * sia il tool di amministrazione sia Docspa si trovano sotto lo stesso progetto 
            * quindi hanno in comune il presente Global.asax .
            * 
            * Esiste una sessione denominata "AppWA" che all'accesso del tool di amm.ne 
            * viene impostata a "ADMIN"; all'accesso di Docspa viene impostata a "DOCSPA".
            * 
            * Vedi >>>>>>>     Global.asax.cs > Session_End(Object sender, EventArgs e)
            */
            Session["AppWA"] = "DOCSPA";

            if (index != -1)
            {

                if (userHome.ruoli.Length == 0)
                {
                    Session["noruolo"] = "L'utente non ha ruoli associati.<br> Contattare l'amministratore del sistema";
                }

                //costruisco la lista delle Unità Ogranizzativa	
                drawListaUO(createListHierarchy(index));
                Session["userRuolo"] = ((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[index]);
            }

            //Ricarica il frame dei menù
            Response.Write("<script>top.superiore.location.href = 'testata320.aspx';</script>");
            if (Session["reloadScelta"] == null)
            {
                Session.Add("reloadScelta", "ok");
                //Response.Write("<script>top.principale.location.href = 'sceltaRuoloNew.aspx';</script>");
                Response.Write("<script>top.principale.iFrame_sx.document.location.href = 'sceltaRuoloNew.aspx';</script>");
            }
        }

        private string getCodRuolo(string systemID)
        {
            int cont;

            string Codice = "";
            cont = 0;

            while ((cont < userHome.ruoli.Length) && !((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[cont]).systemId.Equals(systemID))
            {
                cont = cont + 1;
            }
            if (cont < userHome.ruoli.Length)
            {
                Codice = ((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[cont]).codice;
            }
            return Codice;
        }



        private int getIndexRuolo(string systemID)
        {
            int cont;
            int indice = -1;
            cont = 0;

            while ((cont < userHome.ruoli.Length) && !((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[cont]).systemId.Equals(systemID))
            {
                cont = cont + 1;
            }
            if (cont < userHome.ruoli.Length)
            {
                indice = cont;
            }
            return indice;
        }



        private ArrayList createListHierarchy(int indexUO)
        {
            ArrayList Hlist = new ArrayList();

            Hlist.Add(((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[indexUO]).uo.descrizione);
            DocsPaWR.UnitaOrganizzativa CurrUO = ((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[indexUO]).uo.parent;


            while (CurrUO != null)
            {
                Hlist.Add(CurrUO.descrizione);
                CurrUO = CurrUO.parent;
            }

            return Hlist;
        }

        private void drawListaUO(ArrayList HList)
        {
            string lista = string.Empty;
            this.lbl_listaUO.Text = "";

            if (HList.Count > 1)
            {
                for (int i = 0; i < HList.Count; i++)
                {
                    lista += (HList[HList.Count - i - 1].ToString()) + "&nbsp;&gt;&nbsp;";
                }

                lista = lista.Remove((lista.Length - 16)); // elimina il ">" finale

                // mette in bold l'ultima UO (di apparetenenza)
                int ultimo = lista.LastIndexOf(";");
                string primaParte = lista;
                if (ultimo != -1)
                    primaParte = lista.Substring(0, ultimo);
                lista = primaParte + "<b>" + lista.Substring(ultimo + 1) + "<b />";
            }

            if (HList.Count == 1)
            {
                // mette in bold la UO di apparetenenza
                lista = "<b>" + HList[0].ToString() + "<b />";
            }

            this.lbl_listaUO.Text = lista;

        }

        private void Button1_Click(object sender, System.EventArgs e)
        {
            Response.Write("<script>alert('ppp');</script>");
            Response.Write("<script>window.open('testata320.aspx','superiore','');</script>");
            Response.Redirect("testata320.aspx");
        }

        private void aggiornaToDoList()
        {
            try
            {
                    // Aggiornamento contesto corrente
                    this.RefreshCurrentContext();

                    //array contenitore degli array filtro di ricerca
                    DocsPaWR.FiltroRicerca[][] qV;
                    DocsPaWR.FiltroRicerca fV1;
                    DocsPaWR.FiltroRicerca[] fVList;

                    qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                    qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];

                    if (ricercaTrasm.DialogFiltriRicercaTrasmissioni.CurrentFilters != null)
                        fVList = ricercaTrasm.DialogFiltriRicercaTrasmissioni.CurrentFilters;
                    else
                        fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                    #region filtro "oggetto trasmesso"
                    //TODO VERONICA: filtro per oggetto trasmesso quando si aggiungono i checkbox documenti fascicoli
                    //if (this.DDLOggettoTab1.SelectedIndex >= 0)
                    //{
                    //    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    //    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString();
                    //    fV1.valore = this.DDLOggettoTab1.SelectedItem.Value.ToString();
                    //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    //}
                    #endregion
                    #region filtro "TO DO LIST"
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TODO_LIST.ToString();

                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    #endregion
                    #region filtro "NO SOTTOPOSTI"
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.NO_CERCA_INFERIORI.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    #endregion

                    #region filtro "ELEMENTI NON LETTI"
                    if (Session["TrasmNonViste"] != null && Session["TrasmNonViste"].ToString() != "")
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString();
                        fV1.valore = Session["TrasmNonViste"].ToString();
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.ELEMENTI_NON_VISTI.ToString();
                        fV1.valore = "1";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    #endregion

                    if (Session["TrasmDocPredisposti"] != null && Convert.ToBoolean(Session["TrasmDocPredisposti"]))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                        fV1.valore = "PR";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                
                    if (Session["TrasmNonAccettate"] != null && Session["TrasmNonAccettate"] != "")
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONI_ACCETTATE.ToString();
                        fV1.valore = "1";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                   

                    qV[0] = fVList;
                    DocumentManager.setFiltroRicTrasm(this, qV[0]);
                    
                    //paging
                    TrasmManager.removeMemoriaNumPag(this);
                    Session.Remove("data");

                    //Response.Write("<SCRIPT>try { top.principale.iFrame_dx.document.location='TodoList/toDoList.aspx?type=" + DDLOggettoTab1.SelectedValue.ToString() + "&tiporic=R&home=Y'; } catch(e) {try { top.principale.iFrame_dx.document.location='TodoList/toDoList.aspx?type=" + DDLOggettoTab1.SelectedValue.ToString() + "&tiporic=R&home=Y'; } catch(e) {}}</SCRIPT>");
                    Response.Write("<SCRIPT>try { top.principale.iFrame_dx.document.location='TodoList/toDoList.aspx?tiporic=R&home=Y'; } catch(e) {try { top.principale.iFrame_dx.document.location='TodoList/toDoList.aspx?tiporic=R&home=Y'; } catch(e) {}}</SCRIPT>");
                //}
            }
            catch (System.Exception es)
            {
                //System.Diagnostics.Debug.WriteLine("error Login"+es.Message.ToString());
                ErrorManager.redirect(this, es);
            }
        }

        protected void btnCercaTrasmissioni_Click(object sender, EventArgs e)
        {
            this.PerformActionCercaTrasmissioni();
        }

        /// <summary>
        /// Restituzione valore configurazione relativamente al caricamento
        /// automatico della todolist
        /// </summary>
        /// <returns></returns>
        private bool AutoToDoList()
        {
            bool update = false;

            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null && 
                currentContext.ContextName == SiteNavigation.NavigationKeys.PAGINA_INIZIALE &&
                currentContext.IsBack)
            {
                // Verifica se è stata effettuata già una ricerca in todolist
                // e, se selezionato già una trasmissione, notifica che 
                // la ricerca deve essere effettuata automaticamente,
                // anche se autotodolist=false
                update = (currentContext.QueryStringParameters.ContainsKey("idTrasm"));
            }

            if (!update)
                update = (ConfigSettings.getKey(ConfigSettings.KeysENUM.AUTO_TO_DO_LIST) == "1");

            return update;
        }

        /// <summary>
        /// Azione di ricerca trasmissioni in todolist
        /// </summary>
        private void PerformActionCercaTrasmissioni()
        {
            //Spostano nel metodo aggiornaToDoList
            //// Aggiornamento contesto corrente
            //this.RefreshCurrentContext();

            //Session["Tipo_obj"] = DDLOggettoTab1.SelectedItem.Value;
            aggiornaDatiRuolo(chklst_ruoli.SelectedIndex);
            aggiornaToDoList();
        }

        /// <summary>
        /// Azione di visualizzazione filtri ricerca trasmissioni
        /// </summary>
        private void PerformActionShowFiltersTrasmissioni()
        {
            bool searchToDoList;

            if (bool.TryParse(this.hd_performSearchToDoList.Value, out searchToDoList))
            {
                if (searchToDoList)
                {
                    // Azione di ricerca trasmissioni
                    this.PerformActionCercaTrasmissioni();

                    this.hd_performSearchToDoList.Value = "false";
                }
            }
        }

        /// <summary>
        /// Azione di rimozione filtri ricerca trasmissioni
        /// </summary>
        private void PerformActionRemoveFiltersTrasmissioni()
        {
            ricercaTrasm.DialogFiltriRicercaTrasmissioni.CurrentFilters = null;
            this.hd_performSearchToDoList.Value = "false";

            // Azione di ricerca trasmissioni
            this.PerformActionCercaTrasmissioni();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnShowFilters_Click(object sender, EventArgs e)
        {
            // Azione di visualizzazione filtri ricerca trasmissioni
            this.PerformActionShowFiltersTrasmissioni();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRemoveFilters_Click(object sender, EventArgs e)
        {
            // Azione di rimozione filtri ricerca trasmissioni
            Session.Remove("TrasmNonViste");
            Session.Remove("TrasmNonAccettate");
            Session.Remove("TrasmDocPredisposti");
            this.PerformActionRemoveFiltersTrasmissioni();
        }

        /// <summary>
        /// Abilitazione / disabilitazione pulsanti di filtro
        /// </summary>
        private void EnableFilterButtons()
        {
            if ( (Session["TrasmNonViste"] != null && Session["TrasmNonViste"].ToString()!="") ||
                 (Session["TrasmDocPredisposti"] != null && Convert.ToBoolean(Session["TrasmDocPredisposti"]) ||
                 (Session["TrasmNonAccettate"] != null && Session["TrasmNonAccettate"].ToString() != "")
                 )
                )
            {
                this.btnRemoveFilters.Visible = true;
                aggiornaToDoList();
            }
            else
            {
                bool isVisible = (ricercaTrasm.DialogFiltriRicercaTrasmissioni.CurrentFilters != null);
                this.btnRemoveFilters.Visible = isVisible;
            }
        }

        /// <summary>
        /// Impostazione valore combo per la tipologia trasmissione
        /// (documento o fascicolo)
        /// </summary>
        private void SetTipoTrasmissione()
        {
            string tipoOggettoTrasmissione = string.Empty;

            SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;
            if (context != null && context.ContextName == SiteNavigation.NavigationKeys.PAGINA_INIZIALE && context.IsBack)
            {
                // Reperimento del tipo di ricerca effettuata dal contest
                if (context.QueryStringParameters.ContainsKey("tipoRic"))
                    tipoOggettoTrasmissione = context.QueryStringParameters["tipoRic"].ToString();
            }
            else if (ricercaTrasm.DialogFiltriRicercaTrasmissioni.CurrentFilters != null)
            {
                // nel caso in cui ci siano dei filtri correntemente in sessione
                foreach (DocsPaWR.FiltroRicerca item in ricercaTrasm.DialogFiltriRicercaTrasmissioni.CurrentFilters)
                {
                    if (item.argomento.Equals(DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString()))
                    {
                        tipoOggettoTrasmissione = item.valore;
                        break;
                    }
                    
                }
            }
            
            //Commentato vecchia gestione todolist non unificata
            //if (!string.IsNullOrEmpty(tipoOggettoTrasmissione))
            //    this.DDLOggettoTab1.SelectedValue = tipoOggettoTrasmissione;
            //else
            //    // Impostazione di default
            //    DDLOggettoTab1.SelectedIndex = 0;
        }

        protected void Button1_Click1(object sender, EventArgs e)
        {

        }

        private void btn_all_todolist_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            
            string scriptString = "<SCRIPT>ApriListaToDoList();</SCRIPT>";
            this.RegisterStartupScript("apriListaToDoList", scriptString);
        }

        #region Gestione CallContext

        /// <summary>
        /// Aggiornamento contesto corrente
        /// </summary>
        private void RefreshCurrentContext()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            
            if (currentContext != null && 
                currentContext.ContextName == SiteNavigation.NavigationKeys.PAGINA_INIZIALE &&
                !currentContext.IsBack)
            {
                currentContext.PageNumber = 1;
                //currentContext.QueryStringParameters["tipoRic"] = this.DDLOggettoTab1.SelectedValue;
            }
        }

        #endregion

        #region MessageBox
        //private void msg_delega_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        //{
        //    if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
        //    {
        //        //Session.Add("ESERCITADELEGA", true);
        //        string scriptString = "<SCRIPT>OpenDeleghe();</SCRIPT>";
        //        this.RegisterStartupScript("OpenDeleghe", scriptString);
        //        Session.Add("ESERCITADELEGA", true);
        //    }
        //}
        #endregion
    }
}