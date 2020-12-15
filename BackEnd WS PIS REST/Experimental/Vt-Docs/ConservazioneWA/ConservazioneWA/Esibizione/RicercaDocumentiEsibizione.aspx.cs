using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ConservazioneWA.Utils;
using ConservazioneWA.DocsPaWR;
using System.Collections;
using System.Globalization;
using System.Text;
using System.IO;

namespace ConservazioneWA.Esibizione
{
    public partial class RicercaDocumentiEsibizione : System.Web.UI.Page
    {
        protected WSConservazioneLocale.InfoUtente infoUtente;
        protected DocsPaWR.InfoUtente infoUser;
        protected WSConservazioneLocale.InfoAmministrazione amm;


        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);

            // MEV CS 1.4 - Esibizione
            menuTop.ProfiloUtente = "ESIBIZIONE";

            if (!IsPostBack)
            {
                Session.Remove("SelectedElementDictionary");

                GetLettereProtocolli();
                GestioneGrafica();
                GetAOO();
                GetTypeDocument();
                caricaComboTipoFileAcquisiti();
                this.RecordCount = 0;
                this.SelectedPage = 1;
                this.SearchFilters = null;
                this.Result = null;
                this.TemplateProf = null;
            }
        }

        public void Page_Prerender(object sender, EventArgs e)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session["fascicoli"] != null && !string.IsNullOrEmpty(is_fasc.Value))
            {
                WSConservazioneLocale.Fascicolo[] projectList = HttpContext.Current.Session["fascicoli"] as WSConservazioneLocale.Fascicolo[];
                txtCodFascicolo.Text = projectList[0].codice;
                txtDescFascicolo.Text = projectList[0].descrizione;
                this.id_Fasc.Value = this.is_fasc.Value;
                this.is_fasc.Value = string.Empty;
                projectList = null;
            }

            //
            // Mev CS 1.4 Esibizione
            if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
            {
                // Recupero l'idGruppo selezionato
                //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                string idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
                DocsPaWR.Utente user = (DocsPaWR.Utente)Session["userData"];

                if (user != null)
                {
                    //Recupero il ruolo selezionato a partire dall'idGruppo
                    Ruolo ruoloSelected = user.ruoli.FirstOrDefault(item => item.idGruppo == idGruppo);
                    //Ruolo ruoloSelected = (Ruolo)Session["RuoloSelezionato"];

                    if (ruoloSelected != null)
                    {
                        lbl_selectedRole.Text = ruoloSelected.descrizione;
                    }
                }

                // Rimetto in sessione il valore dell'idGruppo utilizzato dall'utente
                HttpContext.Current.Session["RuoloSelezionato_idGruppo"] = idGruppo;
            }
            // End Mev CS 1.4 Esibizione
            //
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkRicercaNotifiche_CheckedChanged(object sender, EventArgs e)
        {
            this.tabFiltriDocumentiInConservazione.Visible = !this.chkRicercaNotifiche.Checked;
            this.tabFiltriNotificheInConservazione.Visible = this.chkRicercaNotifiche.Checked;
            this.upFiltriRicerca.Update();

            this.SelectedPage = 1;
            this.dgResult.VirtualItemCount = 0;
            this.dgResult.CurrentPageIndex = 0;
            this.dgResult.DataSource = null;
            this.dgResult.DataBind();
            this.upRisultati.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dgResult_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            //Prova
            #region Gestione_Colore_Riga_Selezionata

            for (int i = 0; i < dgResult.Items.Count; i+=2)
                dgResult.Items[i].BackColor = System.Drawing.ColorTranslator.FromHtml("#f2f2f2");
            for (int i = 1; i < dgResult.Items.Count; i += 2)
                dgResult.Items[i].BackColor = System.Drawing.ColorTranslator.FromHtml("#fafafa");
            e.Item.BackColor = System.Drawing.ColorTranslator.FromHtml("#666666");

            #endregion
            //End Prova

            Label lblSystemId = (Label)e.Item.FindControl("SYSTEM_ID");

            if (e.CommandName == "VISUALIZZA_REPORT_PDF")
            {
                ScriptManager.RegisterStartupScript(this,
                            this.GetType(),
                            "VISUALIZZA_REPORT_PDF",
                            string.Format("showFile('{0}', '0');", lblSystemId.Text),
                            true);
            }

            //
            // Mev Cs 1.4 - Esibizione
            // Questa region mette in sessione un dictionary con la scelta dell'istanza di conservazione effettuata dall'utente
            #region SCELTA_ISTANZE_CONSERVAZIONE
            // Scelta Istanze di Conservazione
            if (e.CommandName == "SCELTA_ISTANZE_CONSERVAZIONE")
            {
                ImageButton btnSceltaIstanze = (ImageButton)e.Item.FindControl("btnSceltaIstanze");
                
                string listaIstanze = e.CommandArgument.ToString().Split('$')[0];
                string idObject = e.CommandArgument.ToString().Split('$')[1];

                if (btnSceltaIstanze != null)
                {
                    try
                    {
                        //
                        // Invoco la Funzione Javascript per aprire la popup di scelta istanze
                        ScriptManager.RegisterStartupScript(this,
                            this.GetType(),
                            "SCELTA_ISTANZE_CONSERVAZIONE",
                            string.Format("SceltaIstanze('{0}','{1}');", listaIstanze, idObject),
                            true);
                    }
                    catch (Exception ee)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Scelta_istanze_KO", "<script>alert('Scelta Istanze Conservazione KO. Eccezione = " + ee.Message.ToString() + "');</script>", false);
                    }
                }
            }
            #endregion

            #region INVIA_AREA_ESIBIZIONE
            // Invio in Esibizione
            if (e.CommandName == "INVIA_AREA_ESIBIZIONE")
            {
                
                ImageButton btnInviaAreaEsibizione = (ImageButton)e.Item.FindControl("btnInviaAreaEsibizione");

                string param_idIstanza = string.Empty;
                string param_idOggetto = string.Empty;

                if (btnInviaAreaEsibizione != null)
                {
                    #region RECUPERO_PARAMETRI_DICTIONARY
                    // Se non apro la Popup di scelta istanze, viene presa la prima istanza di default
                    // Recupero Lista_Istanze e IdObject a partire dal CommandArgument
                    
                    //
                    // La lista Istanze è caratterizzata da:
                    // idIstanza1-idIstanza2-idIstanza3
                    string listaIstanze = e.CommandArgument.ToString().Split('$')[0];
                    string idObject = e.CommandArgument.ToString().Split('$')[1];

                    // Imposto la prima istanza cme default
                    string idIstanzaDefault = listaIstanze.ToString().Split('-')[0];

                    // imposto Object come parametro da passare al metodo per l'invio in esibizione 
                    param_idOggetto = idObject;
                    
                    // Gestione dictionary
                    Dictionary<string, string> dictIstSelected = null;

                    // Dictionary presente in sessione (Scelta istanze di conservazione)
                    if (HttpContext.Current.Session["dictIstSelected"] != null)
                    {
                        //Controllo che il dictionary per quell'oggetto contiene già l'item selected
                        dictIstSelected = HttpContext.Current.Session["dictIstSelected"] as Dictionary<string, string>;

                        //Dictionary già popolato
                        if (dictIstSelected.ContainsKey(idObject))
                        {
                            // Recupero il valore dell'istanza selezionata per ultimo
                            param_idIstanza = dictIstSelected[idObject];
                        }
                        else
                        { 
                            // Valore Dictionary non presente
                            //Aggiungo valore di default al dictionary
                            dictIstSelected[idObject] = idIstanzaDefault;
                            param_idIstanza = idIstanzaDefault;
                        }
                    }
                    else
                    {
                        // Creo il dictionary <objectSelected, idIstanza>
                        dictIstSelected = new Dictionary<string, string>();
                        //Aggiungo valore di default al dictionary
                        dictIstSelected[idObject] = idIstanzaDefault;
                        param_idIstanza = idIstanzaDefault;
                    }
                    // Metto il dictionary in sessione
                    HttpContext.Current.Session["dictIstSelected"] = dictIstSelected;
                    #endregion

                    #region CODE_EXECUTE_INVIO_ESIBIZIONE
                    // Metodo che fa l'invio
                    string IdItemsEsibizione = "-1";
                    string eccezione = string.Empty;
                    try
                    {
                        //
                        // Get info utili per fare la chiamata al BE

                        string idGruppo = string.Empty;
                        
                        // Recupero dalla sessione l'idGruppo selezionato
                        if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
                            // Recupero l'idGruppo selezionato
                            //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                            idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
                        
                        WSConservazioneLocale.ItemsConservazione[] itemsCons = ConservazioneManager.getItemsConservazioneWithSecurity(param_idIstanza, this.infoUtente, idGruppo);
                        // End Mev Cs 1.4 - Esibizione
                        //

                        //
                        // Occorre recuperare i valori dell'elemento (istanza di conservazione) selezionato

                        if (itemsCons != null && itemsCons.Length > 0)
                        {
                            // Mi serve per recuperare l'elemento selezionato
                            string idProfile = param_idOggetto;
                            
                            if (!string.IsNullOrEmpty(idProfile))
                            {
                                // Occorre reperire i parametri da passare alla chiamata di Backend
                                // Costruzione dei Parametri
                                var query = from p in itemsCons.ToList()
                                            where p.ID_Profile == idProfile
                                            select new { p.ID_Profile, p.ID_Project, p.DocNumber, p.tipo_oggetto };
                                
                                string id_profile = query.FirstOrDefault().ID_Profile;
                                string id_project = string.Empty;
                                string doc_number = query.FirstOrDefault().DocNumber;
                                //string tipo_oggetto = query.FirstOrDefault().tipo_oggetto;
                                string tipo_oggetto = "D";            
                                
                                // Old Code
                                //string id_profile = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).ID_Profile;
                                //string id_project = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).ID_Project;
                                //string doc_number = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).DocNumber;
                                //string tipo_oggetto = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).tipo_oggetto;
                                // End Old Code

                                string idIstanzaConservazione = param_idIstanza;

                                // Verifico la presenza del documento con idProfile nell'istanza di esibizione con idIstanzaConservazione da cui è stato prelevato il file 
                                // e per lo stesso idPeople e idGruppo
                                bool docPresente = ConservazioneManager.checkItemEsibizionePresenteInIstanzaEsibizione(id_profile, id_project, tipo_oggetto, infoUtente, idIstanzaConservazione);

                                if (!docPresente)
                                {
                                    WSConservazioneLocale.SchedaDocumento documentDetails = null;

                                    // Il metodo deve invocare la Stored Procedure SP_INSERT_AREA_ESIB
                                    IdItemsEsibizione = ConservazioneManager.CreateAndAddDocInAreaEsibizione(id_profile, id_project, doc_number, infoUtente, tipo_oggetto, idIstanzaConservazione, out documentDetails);

                                    try
                                    {
                                        int size_xml = ConservazioneManager.getItemSizeEsib(
                                            this.Page,
                                            documentDetails,
                                            IdItemsEsibizione);

                                        int doc_size = Convert.ToInt32(documentDetails.documenti[0].fileSize);

                                        int numeroAllegati = documentDetails.allegati.Length;
                                        string fileName = documentDetails.documenti[0].fileName;
                                        string tipoFile = Path.GetExtension(fileName);
                                        int size_allegati = 0;
                                        for (int i = 0; i < documentDetails.allegati.Length; i++)
                                        {
                                            size_allegati = size_allegati + Convert.ToInt32(documentDetails.allegati[i].fileSize);
                                        }
                                        int total_size = size_allegati + doc_size + size_xml;

                                        ConservazioneManager.insertSizeInItemEsib(Page, IdItemsEsibizione, total_size);

                                        ConservazioneManager.updateItemsEsibizione(
                                            this.Page,
                                            tipoFile,
                                            Convert.ToString(numeroAllegati),
                                            IdItemsEsibizione);
                                    }
                                    catch (Exception exc1)
                                    {
                                    }
                                }
                                else
                                {
                                    // Documento già presente
                                    IdItemsEsibizione = "-2";
                                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Invio_Esibizione_OK", "<script>alert('Attenzione, Documento già presente.');</script>", false);
                                    //ClientScript.RegisterStartupScript(this.GetType(), "Invio_Esibizione_Doc_Presente", "<script>alert('Attenzione, Documento presente.');</script>");
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        IdItemsEsibizione = "-1";
                        eccezione = exc.Message;
                    }

                    // esito -1 -> KO
                    if (!string.IsNullOrEmpty(IdItemsEsibizione) && IdItemsEsibizione.Equals("-1"))
                        // Alert Javascript
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Invio_Esibizione_KO", "<script>alert('Attenzione, Invio istanza di esibizione non riuscita');</script>", false);
                    else
                    {
                        if (!string.IsNullOrEmpty(IdItemsEsibizione) && IdItemsEsibizione.Equals("-2"))
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Invio_Esibizione_OK", "<script>alert('Attenzione, Documento già presente.');</script>", false);
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Invio_Esibizione_OK", "<script>alert('Invio istanza di esibizione avvenuto correttamente');</script>", false);
                    }
                    #endregion
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Invio_Esibizione_OK", "<script>alert('Invio Esibizione. param_idIstanza = " + param_idIstanza.ToString() + "');</script>", false);
                }
            }
            #endregion

            #region VISUALIZZA_DOCUMENTO
            // Visualizzazione documento
            if (e.CommandName == "VISUALIZZA_DOCUMENTO") 
            {
                //Button btnInviaAreaEsibizione = (Button)e.Item.FindControl("btnInviaAreaEsibizione");
                ImageButton btnVisualizza = (ImageButton)e.Item.FindControl("btnVisualizza");

                string param_idIstanza = string.Empty;
                string param_idDoc = string.Empty;

                #region oldCode
                //string listaIstanze = e.CommandArgument.ToString().Split('$')[0];
                //param_idDoc = e.CommandArgument.ToString().Split('$')[1];

                //// Imposto la prima istanza cme default
                //param_idIstanza = listaIstanze.ToString().Split('-')[0];
                #endregion

                #region RECUPERO_PARAMETRI_DICTIONARY
                // Se non apro la Popup di scelta istanze, viene presa la prima istanza di default
                // Recupero Lista_Istanze e IdObject a partire dal CommandArgument

                //
                // La lista Istanze è caratterizzata da:
                // idIstanza1-idIstanza2-idIstanza3
                string listaIstanze = e.CommandArgument.ToString().Split('$')[0];
                string idObject = e.CommandArgument.ToString().Split('$')[1];

                // Imposto la prima istanza cme default
                string idIstanzaDefault = listaIstanze.ToString().Split('-')[0];

                // imposto Object come parametro da passare al metodo per l'invio in esibizione 
                param_idDoc = idObject;

                // Gestione dictionary
                Dictionary<string, string> dictIstSelected = null;

                // Dictionary presente in sessione (Scelta istanze di conservazione)
                if (HttpContext.Current.Session["dictIstSelected"] != null)
                {
                    //Controllo che il dictionary per quell'oggetto contiene già l'item selected
                    dictIstSelected = HttpContext.Current.Session["dictIstSelected"] as Dictionary<string, string>;

                    //Dictionary già popolato
                    if (dictIstSelected.ContainsKey(idObject))
                    {
                        // Recupero il valore dell'istanza selezionata per ultimo
                        param_idIstanza = dictIstSelected[idObject];
                    }
                    else
                    {
                        // Valore Dictionary non presente
                        //Aggiungo valore di default al dictionary
                        dictIstSelected[idObject] = idIstanzaDefault;
                        param_idIstanza = idIstanzaDefault;
                    }
                }
                else
                {
                    // Creo il dictionary <objectSelected, idIstanza>
                    dictIstSelected = new Dictionary<string, string>();
                    //Aggiungo valore di default al dictionary
                    dictIstSelected[idObject] = idIstanzaDefault;
                    param_idIstanza = idIstanzaDefault;
                }
                // Metto il dictionary in sessione
                HttpContext.Current.Session["dictIstSelected"] = dictIstSelected;
                #endregion

                if (btnVisualizza != null)
                {
                    #region oldCode
                    //ScriptManager.RegisterStartupScript(this,
                    //        this.GetType(),
                    //        "Visualizza_Documento",
                    //        string.Format("showFile('{0}', '0');", e.CommandArgument.ToString()),
                    //        true);
                    #endregion

                    ScriptManager.RegisterStartupScript(this,
                            this.GetType(),
                            "Visualizza_Documento",
                            string.Format("showFile('{0}', '{1}');", param_idIstanza, param_idDoc),
                            true);
                }
            }
            #endregion

            #region VISUALIZZA_METADATI_DOCUMENTO
            // Visualizzazione metadati
            if (e.CommandName == "VISUALIZZA_METADATI_DOCUMENTO")
            {
                //Button btnInviaAreaEsibizione = (Button)e.Item.FindControl("btnInviaAreaEsibizione");
                ImageButton btnVisualizza = (ImageButton)e.Item.FindControl("btnVisualizza");

                string param_idIstanza = string.Empty;
                string param_idDoc = string.Empty;

                #region RECUPERO_PARAMETRI_DICTIONARY
                // Se non apro la Popup di scelta istanze, viene presa la prima istanza di default
                // Recupero Lista_Istanze e IdObject a partire dal CommandArgument

                //
                // La lista Istanze è caratterizzata da:
                // idIstanza1-idIstanza2-idIstanza3
                string listaIstanze = e.CommandArgument.ToString().Split('$')[0];
                string idObject = e.CommandArgument.ToString().Split('$')[1];

                // Imposto la prima istanza cme default
                string idIstanzaDefault = listaIstanze.ToString().Split('-')[0];

                // imposto Object come parametro da passare al metodo per l'invio in esibizione 
                param_idDoc = idObject;

                // Gestione dictionary
                Dictionary<string, string> dictIstSelected = null;

                // Dictionary presente in sessione (Scelta istanze di conservazione)
                if (HttpContext.Current.Session["dictIstSelected"] != null)
                {
                    //Controllo che il dictionary per quell'oggetto contiene già l'item selected
                    dictIstSelected = HttpContext.Current.Session["dictIstSelected"] as Dictionary<string, string>;

                    //Dictionary già popolato
                    if (dictIstSelected.ContainsKey(idObject))
                    {
                        // Recupero il valore dell'istanza selezionata per ultimo
                        param_idIstanza = dictIstSelected[idObject];
                    }
                    else
                    {
                        // Valore Dictionary non presente
                        //Aggiungo valore di default al dictionary
                        dictIstSelected[idObject] = idIstanzaDefault;
                        param_idIstanza = idIstanzaDefault;
                    }
                }
                else
                {
                    // Creo il dictionary <objectSelected, idIstanza>
                    dictIstSelected = new Dictionary<string, string>();
                    //Aggiungo valore di default al dictionary
                    dictIstSelected[idObject] = idIstanzaDefault;
                    param_idIstanza = idIstanzaDefault;
                }
                // Metto il dictionary in sessione
                HttpContext.Current.Session["dictIstSelected"] = dictIstSelected;
                #endregion

                if (btnVisualizza != null)
                {
                    ScriptManager.RegisterStartupScript(this,
                            this.GetType(),
                            "Visualizza_Documento",
                            string.Format("showXMLFile('{0}', '{1}', 'D');", param_idIstanza, param_idDoc),
                            true);
                }
            }
            #endregion
            // End Mev Cs 1.4 - Esibizione
            //
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool IsRicercaNotifiche()
        {
            return this.chkRicercaNotifiche.Checked;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dgResult_PreRender(object sender, EventArgs e)
        {
            
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            Session.Remove("SelectedElementDictionary");

            this.cbSelectAll.Checked = false;

            int number = 0;
            ConservazioneWA.WSConservazioneLocale.SearchObject[] documents = null;

            if (!this.chkRicercaNotifiche.Checked && !this.chk_Arr.Checked && !this.chk_Part.Checked && !this.chk_Int.Checked && !this.chk_Grigio.Checked)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Devi selezionare almeno un tipo di documento');", true);
            }
            else
            {
                if (GetFiltriRicerca())
                {
                    if (this.SearchFilters != null)
                    {
                        this.Result = SearchDocument(this.SearchFilters, this.SelectedPage, out number);
                        this.dgResult.VirtualItemCount = number;
                        this.dgResult.CurrentPageIndex = 0;
                        this.dgResult.DataSource = this.Result;
                        this.dgResult.DataBind();


                        this.pnl_result.Visible = true;
                        this.upRisultati.Update();
                    }
                }
            }
        }

        private void GetLettereProtocolli()
        {

        }

        protected void ddl_dataCreazione_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataCreazione_E.SelectedIndex)
            {
                case 0:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;
                    this.lblDa.Text = "Il";

                    this.lblDa.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;

                case 1:
                    this.lblA.Visible = true;
                    this.lbl_dataCreazioneA.Visible = true;
                    this.lblDa.Text = "Da";

                    this.lblDa.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;

                case 2:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;

                case 3:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;

                case 4:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;

            }

            this.upFiltriRicerca.Update();
        }

        protected void ddl_dataCreazione_E_notifiche_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataCreazione_E_notifiche.SelectedIndex)
            {
                case 0:
                    this.lblANotifiche.Visible = false;
                    this.lbl_dataCreazioneANotifiche.Visible = false;
                    this.lbl_dataCreazioneANotifiche.Text = string.Empty;
                    this.lblDaNotifiche.Text = "Il";

                    this.lblDaNotifiche.Visible = true;
                    this.lbl_dataCreazioneDaNotifiche.Visible = true;
                    break;

                case 1:
                    this.lblANotifiche.Visible = true;
                    this.lbl_dataCreazioneANotifiche.Visible = true;
                    this.lblDaNotifiche.Text = "Da";

                    this.lblDaNotifiche.Visible = true;
                    this.lbl_dataCreazioneDaNotifiche.Visible = true;
                    break;

                case 2:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDaNotifiche.Visible = false;
                    this.lbl_dataCreazioneDaNotifiche.Visible = false;
                    this.lbl_dataCreazioneDaNotifiche.Text = string.Empty;
                    break;

                case 3:
                    this.lblANotifiche.Visible = false;
                    this.lbl_dataCreazioneANotifiche.Visible = false;
                    this.lbl_dataCreazioneANotifiche.Text = string.Empty;

                    this.lblDaNotifiche.Visible = false;
                    this.lbl_dataCreazioneDaNotifiche.Visible = false;
                    this.lbl_dataCreazioneDaNotifiche.Text = string.Empty;
                    break;

                case 4:
                    this.lblANotifiche.Visible = false;
                    this.lbl_dataCreazioneANotifiche.Visible = false;
                    this.lbl_dataCreazioneANotifiche.Text = string.Empty;

                    this.lblDaNotifiche.Visible = false;
                    this.lbl_dataCreazioneDaNotifiche.Visible = false;
                    this.lbl_dataCreazioneDaNotifiche.Text = string.Empty;
                    break;

            }

            this.upFiltriRicerca.Update();
        }

        protected void ddl_dataProt_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataProt_E.SelectedIndex)
            {
                case 0:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.txt_Data.Text = string.Empty;
                    this.lblDaP.Text = "Il";

                    this.lblDaP.Visible = true;
                    this.lbl_dataCreazioneDaP.Visible = true;
                    break;

                case 1:
                    this.lblAP.Visible = true;
                    this.lbl_dataCreazioneAP.Visible = true;
                    this.lblDaP.Text = "Da";

                    this.lblDaP.Visible = true;
                    this.lbl_dataCreazioneDaP.Visible = true;
                    break;

                case 2:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.Text = string.Empty;

                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Text = string.Empty;
                    break;

                case 3:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.Text = string.Empty;

                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Text = string.Empty;
                    break;

                case 4:
                    this.lblAP.Visible = false;
                    this.lbl_dataCreazioneAP.Visible = false;
                    this.lbl_dataCreazioneAP.Text = string.Empty;

                    this.lblDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Visible = false;
                    this.lbl_dataCreazioneDaP.Text = string.Empty;
                    break;

            }

            this.upFiltriRicerca.Update();
        }

        protected void GestioneGrafica()
        {
            this.lblA.Visible = false;
            this.lbl_dataCreazioneA.Visible = false;
            this.lblAP.Visible = false;
            this.lbl_dataCreazioneAP.Visible = false;
            this.lblAnumprot_E.Visible = false;
            this.txt_fineNumProt_E.Visible = false;
            this.btnFind.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            this.btnFind.Attributes.Add("onmouseout", "this.className='cbtn';");
            this.tbAnnoProtocollo.Text = DateTime.Now.Year.ToString();
            amm = Utils.ConservazioneManager.GetInfoAmmCorrente(infoUtente.idAmministrazione);
            this.lbl_amm.Text = this.amm.Codice + " - " + this.amm.Descrizione;
            
            tabFiltriNotificheInConservazione.Visible = false;
            this.lblANotifiche.Visible = false;
            this.lbl_dataCreazioneANotifiche.Visible = false;
            this.chk_Arr.Checked = true;
            this.chk_Part.Checked = true;
        }

        protected void ddl_numProt_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txt_fineNumProt_E.Text = "";

            if (this.ddl_numProt_E.SelectedIndex == 0)
            {
                this.txt_fineNumProt_E.Visible = false;
                this.lblAnumprot_E.Visible = false;
                this.lblDAnumprot_E.Text = "Il";
            }
            else
            {
                this.txt_fineNumProt_E.Visible = true;
                this.lblAnumprot_E.Visible = true;
                this.lblDAnumprot_E.Text = "Da";
            }
            this.upFiltriRicerca.Update();
        }

        protected void ddl_idDocumento_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txt_fineIdDoc_C.Text = "";
            if (this.ddl_idDocumento_C.SelectedIndex == 0)
            {
                this.txt_fineIdDoc_C.Visible = false;
                this.lblAidDoc_C.Visible = false;
                this.lblDAidDoc_C.Text = "Il";
            }
            else
            {
                this.txt_fineIdDoc_C.Visible = true;
                this.lblAidDoc_C.Visible = true;
                this.lblDAidDoc_C.Text = "Da";
            }
        }

        protected void GetAOO()
        {
            WSConservazioneLocale.Registro[] reg = ConservazioneManager.GetRfByIdAmm(Convert.ToInt32(infoUtente.idAmministrazione), "0");
            if (reg != null && reg.Length > 0)
            {
                ddl_aoo.Items.Clear();
                ddl_aoo.Items.Add("");
                for (int i = 0; i < reg.Length; i++)
                {
                    string descrizione = "[" + reg[i].codice + "]" + " - " + reg[i].descrizione;
                    ddl_aoo.Items.Add(descrizione);
                    ddl_aoo.Items[i + 1].Value = reg[i].systemId;
                }
            }
        }

        /// <summary>
        /// Evento generato al cambio del testo nella casella del codice rubrica
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCodRuolo_TextChanged(object sender, EventArgs e)
        {
            if (this.infoUtente != null)
            {
                this.infoUser = new InfoUtente();
                infoUser.idCorrGlobali = this.infoUtente.idCorrGlobali;
                infoUser.idPeople = this.infoUtente.idPeople;
                infoUser.idGruppo = this.infoUtente.idGruppo;
                infoUser.dst = this.infoUtente.dst;
                infoUser.idAmministrazione = this.infoUtente.idAmministrazione;
                infoUser.userId = this.infoUtente.userId;
                infoUser.sede = this.infoUtente.sede;
            }

            if (!string.IsNullOrEmpty(txtCodRuolo.Text))
            {
                setDescCorr(txtCodRuolo.Text);
            }
            else
            {
                txtCodRuolo.Text = string.Empty;
                txtDescRuolo.Text = string.Empty;
                id_corr.Value = string.Empty;
                tipo_corr.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Inserire un codice da cercare in rubrica');", true);
            }
            this.upFiltriRicerca.Update();
        }

        protected void setDescCorr(string codRubrica)
        {

            Corrispondente corr = Utils.UserManager.GetCorrispondenteByCodRubricaIE(codRubrica, AddressbookTipoUtente.INTERNO, this.infoUser);
            if (corr == null)
            {
                txtCodRuolo.Text = string.Empty;
                txtDescRuolo.Text = string.Empty;
                id_corr.Value = string.Empty;
                tipo_corr.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Corrispondente non trovato');", true);
            }
            else
            {
                if (corr != null && corr.tipoCorrispondente.Equals("U"))
                {
                    txtCodRuolo.Text = string.Empty;
                    txtDescRuolo.Text = string.Empty;
                    id_corr.Value = string.Empty;
                    tipo_corr.Value = string.Empty;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Inserire soltanto Ruoli o Persone');", true);
                }
                else
                {
                    txtCodRuolo.Text = corr.codiceRubrica;
                    txtDescRuolo.Text = corr.descrizione;
                    id_corr.Value = corr.systemId;
                    tipo_corr.Value = corr.tipoCorrispondente;
                }
            }
        }

        /// <summary>
        /// Evento generato al cambio del testo nella casella del codice rubrica
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCodMitt_TextChanged(object sender, EventArgs e)
        {
            if (this.infoUtente != null)
            {
                this.infoUser = new InfoUtente();
                infoUser.idCorrGlobali = this.infoUtente.idCorrGlobali;
                infoUser.idPeople = this.infoUtente.idPeople;
                infoUser.idGruppo = this.infoUtente.idGruppo;
                infoUser.dst = this.infoUtente.dst;
                infoUser.idAmministrazione = this.infoUtente.idAmministrazione;
                infoUser.userId = this.infoUtente.userId;
                infoUser.sede = this.infoUtente.sede;
            }

            if (!string.IsNullOrEmpty(txtCodMitt.Text))
            {
                setDescCorrMittDest(txtCodMitt.Text);
            }
            else
            {
                txtCodMitt.Text = string.Empty;
                txtDescMitt.Text = string.Empty;
                id_corr_mitt.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Nessun corrispondente trovato');", true);
            }
            this.upFiltriRicerca.Update();
        }

        protected void setDescCorrMittDest(string codRubrica)
        {

            Corrispondente corr = Utils.UserManager.GetCorrispondenteByCodRubricaIE(codRubrica, AddressbookTipoUtente.GLOBALE, this.infoUser);
            if (corr == null)
            {
                txtCodMitt.Text = string.Empty;
                txtDescMitt.Text = string.Empty;
                id_corr_mitt.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Corrispondente non trovato');", true);
            }
            else
            {
                txtCodMitt.Text = corr.codiceRubrica;
                txtDescMitt.Text = corr.descrizione;
                id_corr_mitt.Value = corr.systemId;
            }
        }

        /// <summary>
        /// Evento generato al cambio del testo nella casella del codice fascicolo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCodFascicolo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtCodFascicolo.Text))
            {
                this.SearchProject();
            }
            else
            {
                txtCodFascicolo.Text = string.Empty;
                txtDescFascicolo.Text = string.Empty;
                this.id_Fasc.Value = string.Empty;
                this.is_fasc.Value = string.Empty;
            }
            this.upFiltriRicerca.Update();
        }

        protected void SearchProject()
        {
            Session.Remove("fascicoli");
            WSConservazioneLocale.Fascicolo[] projectList = ConservazioneManager.GetFascicoloDaCodiceNoSecurity(this.txtCodFascicolo.Text, this.infoUtente.idAmministrazione, false);

            if (projectList == null || projectList.Length == 0)
            {
                txtCodFascicolo.Text = string.Empty;
                txtDescFascicolo.Text = string.Empty;
                this.id_Fasc.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "alert('Nessun fascicolo trovato con questo codice');", true);
            }
            else
            {
                if (projectList.Length == 1)
                {
                    txtCodFascicolo.Text = projectList[0].codice;
                    txtDescFascicolo.Text = projectList[0].descrizione;
                    this.id_Fasc.Value = projectList[0].systemID;
                }
                else
                {
                    HttpContext.Current.Session["fascicoli"] = projectList as WSConservazioneLocale.Fascicolo[];

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "OpenSceltaFascicoli();", true);
                    txtCodFascicolo.Text = string.Empty;
                    txtDescFascicolo.Text = string.Empty;
                    this.id_Fasc.Value = string.Empty;
                }
            }
        }

        protected void GetTypeDocument()
        {

            WSConservazioneLocale.TemplateLite[] templateList;
            templateList = ConservazioneManager.GetTypeDocumentsWithDiagramByIdAmm(Convert.ToInt32(infoUtente.idAmministrazione), "D");
            if (templateList != null && templateList.Length > 0)
            {
                ddl_type_documents.Items.Clear();
                ddl_type_documents.Items.Add("");
                for (int i = 0; i < templateList.Length; i++)
                {
                    ddl_type_documents.Items.Add(templateList[i].name);
                    ddl_type_documents.Items[i + 1].Value = templateList[i].system_id;
                }
            }

        }

        protected void caricaComboTipoFileAcquisiti()
        {
            if (this.infoUtente != null)
            {
                this.infoUser = new InfoUtente();
                infoUser.idCorrGlobali = this.infoUtente.idCorrGlobali;
                infoUser.idPeople = this.infoUtente.idPeople;
                infoUser.idGruppo = this.infoUtente.idGruppo;
                infoUser.dst = this.infoUtente.dst;
                infoUser.idAmministrazione = this.infoUtente.idAmministrazione;
                infoUser.userId = this.infoUtente.userId;
                infoUser.sede = this.infoUtente.sede;
            }
            ArrayList tipoFile = new ArrayList();
            tipoFile = ConservazioneManager.getExtFileAcquisiti(this.infoUser);
            for (int i = 0; i < tipoFile.Count; i++)
            {
                if (!tipoFile[i].ToString().Contains("P7M"))
                {
                    ListItem item = new ListItem(tipoFile[i].ToString());
                    this.ddl_tipoFileAcquisiti.Items.Add(item);
                }
            }
        }

        protected bool GetFiltriRicerca()
        {
            //array contenitore degli array filtro di ricerca
            WSConservazioneLocale.FiltroRicerca[][] qV = new WSConservazioneLocale.FiltroRicerca[1][];
            qV[0] = new WSConservazioneLocale.FiltroRicerca[1];
            WSConservazioneLocale.FiltroRicerca[] fVList = new WSConservazioneLocale.FiltroRicerca[0];
            WSConservazioneLocale.FiltroRicerca fV1 = null;

            if (this.chkRicercaNotifiche.Checked)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriDocumento.GRIGIO.ToString();
                fV1.valore = "true";
                fVList = addToArrayFiltroRicerca(fVList, fV1);

                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriDocumento.OGGETTO.ToString();

                string OGGETTO_NOTIFICA = string.Empty;

                if (!string.IsNullOrEmpty(this.txt_num_istanza.Text))
                {
                    OGGETTO_NOTIFICA = "ISTANZA DI CONSERVAZIONE NUMERO " + this.txt_num_istanza.Text.Trim();
                }
                else
                {
                    OGGETTO_NOTIFICA = "ISTANZA DI CONSERVAZIONE NUMERO";
                }

                fV1.valore = DO_AdattaString(OGGETTO_NOTIFICA);
                fVList = addToArrayFiltroRicerca(fVList, fV1);

                if (this.ddl_dataCreazione_E_notifiche.SelectedIndex == 2)
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_CREAZ_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreazione_E_notifiche.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_CREAZ_SC.ToString();
                    fV1.valore = "1";
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreazione_E_notifiche.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_CREAZ_MC.ToString();
                    fV1.valore = "1";
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreazione_E_notifiche.SelectedIndex == 0)
                {//valore singolo carico DATA_CREAZIONE
                    if (!this.GetCalendarControl("lbl_dataCreazioneDaNotifiche").txt_Data.Text.Equals(""))
                    {
                        if (!isDate(this.GetCalendarControl("lbl_dataCreazioneDaNotifiche").txt_Data.Text))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                            return false;
                        }
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("lbl_dataCreazioneDaNotifiche").txt_Data.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataCreazione_E_notifiche.SelectedIndex == 1)
                {//valore singolo carico DATA_CREAZIONE_DAL - DATA_CREAZIONE_AL
                    if (!this.GetCalendarControl("lbl_dataCreazioneDaNotifiche").txt_Data.Text.Equals(""))
                    {
                        if (!isDate(this.GetCalendarControl("lbl_dataCreazioneDaNotifiche").txt_Data.Text))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                            return false;
                        }
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("lbl_dataCreazioneDaNotifiche").txt_Data.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("lbl_dataCreazioneANotifiche").txt_Data.Text.Equals(""))
                    {
                        if (!isDate(this.GetCalendarControl("lbl_dataCreazioneANotifiche").txt_Data.Text))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);

                            return false;
                        }
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("lbl_dataCreazioneANotifiche").txt_Data.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                fV1 = new WSConservazioneLocale.FiltroRicerca
                {
                    argomento = FiltriDocumento.ORDER_DIRECTION.ToString(),
                    nomeCampo = "CREATION_DATE",
                    valore = "DESC"
                };
                fVList = addToArrayFiltroRicerca(fVList, fV1);

                qV[0] = fVList;
            }
            else
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriDocumento.PROT_ARRIVO.ToString();
                if (this.chk_Arr.Checked)
                {
                    fV1.valore = "true";
                }
                else
                {
                    fV1.valore = "false";
                }
                fVList = addToArrayFiltroRicerca(fVList, fV1);

                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriDocumento.PROT_PARTENZA.ToString();
                if (chk_Part.Checked)
                {
                    fV1.valore = "true";
                }
                else
                {
                    fV1.valore = "false";
                }

                fVList = addToArrayFiltroRicerca(fVList, fV1);

                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriDocumento.PROT_INTERNO.ToString();
                if (chk_Int.Checked)
                {
                    fV1.valore = "true";
                }
                else
                {
                    fV1.valore = "false";
                }
                fVList = addToArrayFiltroRicerca(fVList, fV1);

                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriDocumento.GRIGIO.ToString();
                fV1.valore = Convert.ToString(this.chk_Grigio.Checked);
                fVList = addToArrayFiltroRicerca(fVList, fV1);

                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriDocumento.TIPO.ToString();
                fV1.valore = "tipo";
                fVList = addToArrayFiltroRicerca(fVList, fV1);

                if (this.ddl_idDocumento_C.SelectedIndex == 0)
                {
                    if (this.txt_initIdDoc_C.Text != null && !this.txt_initIdDoc_C.Text.Equals(""))
                    {
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.DOCNUMBER.ToString();
                        fV1.valore = this.txt_initIdDoc_C.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {
                    if (this.txt_initIdDoc_C.Text != null && !this.txt_initIdDoc_C.Text.Equals(""))
                    {
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.DOCNUMBER_DAL.ToString();
                        fV1.valore = this.txt_initIdDoc_C.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (this.txt_fineIdDoc_C.Text != null && !this.txt_fineIdDoc_C.Text.Equals(""))
                    {
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.DOCNUMBER_AL.ToString();
                        fV1.valore = this.txt_fineIdDoc_C.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                if (!string.IsNullOrEmpty(ddl_aoo.SelectedValue))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.REGISTRO.ToString();
                    fV1.valore = ddl_aoo.SelectedValue;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (this.ddl_numProt_E.SelectedIndex == 0)
                {//valore singolo carico NUM_PROTOCOLLO

                    if (this.txt_initNumProt_E.Text != null && !this.txt_initNumProt_E.Text.Equals(""))
                    {
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                        fV1.valore = this.txt_initNumProt_E.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                    if (!this.txt_initNumProt_E.Text.Equals(""))
                    {
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                        fV1.valore = this.txt_initNumProt_E.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txt_fineNumProt_E.Text.Equals(""))
                    {
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                        fV1.valore = this.txt_fineNumProt_E.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                if (this.ddl_dataProt_E.SelectedIndex == 2)
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_PROT_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProt_E.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_PROT_SC.ToString();
                    fV1.valore = "1";
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProt_E.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_PROT_MC.ToString();
                    fV1.valore = "1";
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProt_E.SelectedIndex == 0)
                {//valore singolo carico DATA_PROTOCOLLO
                    if (!this.GetCalendarControl("lbl_dataCreazioneDaP").txt_Data.Text.Equals(""))
                    {
                        if (!isDate(this.GetCalendarControl("lbl_dataCreazioneDaP").txt_Data.Text))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                            return false;
                        }
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_PROT_IL.ToString();
                        fV1.valore = this.GetCalendarControl("lbl_dataCreazioneDaP").txt_Data.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataProt_E.SelectedIndex == 1)
                {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                    if (!this.GetCalendarControl("lbl_dataCreazioneDaP").txt_Data.Text.Equals(""))
                    {
                        if (!isDate(this.GetCalendarControl("lbl_dataCreazioneDaP").txt_Data.Text))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                            return false;
                        }
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("lbl_dataCreazioneDaP").txt_Data.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("lbl_dataCreazioneAP").txt_Data.Text.Equals(""))
                    {
                        if (!isDate(this.GetCalendarControl("lbl_dataCreazioneAP").txt_Data.Text))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                            return false;
                        }
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("lbl_dataCreazioneAP").txt_Data.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                if (this.ddl_dataCreazione_E.SelectedIndex == 2)
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_CREAZ_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreazione_E.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_CREAZ_SC.ToString();
                    fV1.valore = "1";
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreazione_E.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_CREAZ_MC.ToString();
                    fV1.valore = "1";
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreazione_E.SelectedIndex == 0)
                {//valore singolo carico DATA_CREAZIONE
                    if (!this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text.Equals(""))
                    {
                        if (!isDate(this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                            return false;
                        }
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataCreazione_E.SelectedIndex == 1)
                {//valore singolo carico DATA_CREAZIONE_DAL - DATA_CREAZIONE_AL
                    if (!this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text.Equals(""))
                    {
                        if (!isDate(this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                            return false;
                        }
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("lbl_dataCreazioneA").txt_Data.Text.Equals(""))
                    {
                        if (!isDate(this.GetCalendarControl("lbl_dataCreazioneA").txt_Data.Text))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);

                            return false;
                        }
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("lbl_dataCreazioneA").txt_Data.Text;
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                if (!this.txt_oggetto.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.OGGETTO.ToString();
                    fV1.valore = DO_AdattaString(this.txt_oggetto.Text);
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!string.IsNullOrEmpty(this.id_corr_mitt.Value))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                    fV1.valore = this.id_corr_mitt.Value;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!string.IsNullOrEmpty(this.id_corr.Value))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = "ID_OWNER";
                    fV1.valore = this.id_corr.Value;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!string.IsNullOrEmpty(this.tipo_corr.Value))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = "CORR_TYPE_OWNER";
                    fV1.valore = this.tipo_corr.Value;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!string.IsNullOrEmpty(this.id_Fasc.Value))
                {
                    string idFascicolo = this.id_Fasc.Value;

                    WSConservazioneLocale.Fascicolo fasc = ConservazioneManager.GetFascicoloByID(idFascicolo);
                    WSConservazioneLocale.Folder folder = null;
                    string paramRicerca = string.Empty;

                    if (fasc != null)
                    {
                        this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                        folder = ConservazioneManager.FascicolazioneGetFolder(this.infoUtente, fasc);
                    }

                    if (folder != null && !string.IsNullOrEmpty(folder.systemID))
                    {
                        paramRicerca = folder.systemID;
                    }
                    else
                    {
                        paramRicerca = idFascicolo;
                    }

                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.IN_CHILD_RIC_ESTESA.ToString();
                    //fV1.valore = " IN (" + this.id_Fasc.Value + ") ";
                    fV1.valore = " IN (" + paramRicerca + ") ";
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                fV1.valore = this.tbAnnoProtocollo.Text;
                fVList = addToArrayFiltroRicerca(fVList, fV1);

                if (this.ddl_type_documents.SelectedIndex > 0)
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.TIPO_ATTO.ToString();
                    fV1.valore = this.ddl_type_documents.SelectedItem.Value;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);

                }

                if (this.txt_protoEme.Text != null && !this.txt_protoEme.Text.Equals(""))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.NUM_PROTO_EMERGENZA.ToString();
                    fV1.valore = this.txt_protoEme.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (!this.txt_segnatura.Text.Equals(string.Empty))
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.SEGNATURA.ToString();
                    fV1.valore = this.txt_segnatura.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                if (this.ckFirmato.Checked)
                {
                    //cerco documenti firmati
                    if (!ckNonFirmato.Checked)
                    {
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.FIRMATO.ToString();
                        fV1.valore = "1";
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }//se sono entrambi selezionati cerco i documenti che abbiano un file acquisito, siano essi firmati o meno.
                    else
                    {
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = WSConservazioneLocale.FiltriDocumento.FIRMATO.ToString();
                        fV1.valore = "2";
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {
                    //cerco i documenti non firmati
                    if (ckNonFirmato.Checked)
                    {
                        fV1 = new WSConservazioneLocale.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                        fV1.valore = "0";
                        fVList = addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                if (this.ddl_tipoFileAcquisiti.SelectedIndex > 0)
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.TIPO_FILE_ACQUISITO.ToString();
                    fV1.valore = this.ddl_tipoFileAcquisiti.SelectedItem.Value;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                //Senza timestamp
                if (ctTimestamp.Checked)
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.CON_TIMESTAMP.ToString();
                    fV1.valore = "1";
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                //Con timestamp
                if (ctNoTimestamp.Checked)
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.SENZA_TIMESTAMP.ToString();
                    fV1.valore = "0";
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }

                // 
                //Mev CS 1.4 - Filtro per esibizione:
                // Solo per la conservazione le istanze devono essere Conservate o chiuse
                #region oldcode
                //fV1 = new WSConservazioneLocale.FiltroRicerca();
                //fV1.argomento = WSConservazioneLocale.FiltriDocumento.IN_CONSERVAZIONE.ToString();
                //fV1.valore = "D";
                //fVList = addToArrayFiltroRicerca(fVList, fV1);
                #endregion
                #region newFilter
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriDocumento.IN_CONSERVAZIONE_ESIB.ToString();
                fV1.valore = "D";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
                #endregion



                if (this.TemplateProf != null)
                {
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString();
                    fV1.valore = "Profilazione Dinamica";
                    fV1.template = this.TemplateProf;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }



                qV[0] = fVList;
            }

            this.SearchFilters = qV;

            return true;
        }

        public static WSConservazioneLocale.FiltroRicerca[] addToArrayFiltroRicerca(WSConservazioneLocale.FiltroRicerca[] array, WSConservazioneLocale.FiltroRicerca nuovoElemento)
        {
            WSConservazioneLocale.FiltroRicerca[] nuovaLista;
            if (array != null)
            {
                int len = array.Length;
                nuovaLista = new WSConservazioneLocale.FiltroRicerca[len + 1];
                array.CopyTo(nuovaLista, 0);
                nuovaLista[len] = nuovoElemento;
                return nuovaLista;
            }
            else
            {
                nuovaLista = new WSConservazioneLocale.FiltroRicerca[1];
                nuovaLista[0] = nuovoElemento;
                return nuovaLista;
            }
        }

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        protected ConservazioneWA.UserControl.Calendar GetCalendarControl(string controlId)
        {
            return (ConservazioneWA.UserControl.Calendar)this.FindControl(controlId);
        }

        public static bool isDate(string data)
        {
            try
            {
                data = data.Trim();
                CultureInfo ci = new CultureInfo("it-IT");
                string[] formati = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
                DateTime d_ap = DateTime.ParseExact(data, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string DO_AdattaString(string valore)
        {
            valore = valore.Trim();
            valore = valore.Replace("\r", "");
            valore = valore.Replace("\n", "");
            return valore;
        }

        /// <summary>
        /// Funzione per la ricerca dei documenti
        /// </summary>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        protected WSConservazioneLocale.SearchObject[] SearchDocument(WSConservazioneLocale.FiltroRicerca[][] searchFilters, int selectedPage, out int recordNumber)
        {
            //
            // MEV CS 1.4
            if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
            {
                // Recupero l'idGruppo selezionato
                //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                string idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
                if (!string.IsNullOrEmpty(idGruppo)) 
                {
                    // Imposto l'idGruppo selezionato per le ricerche dei documenti
                    infoUtente.idGruppo = idGruppo;
                }
            }

            // Documenti individuati dalla ricerca
            WSConservazioneLocale.SearchObject[] documents;

            // Numero totale di pagine
            int pageNumbers;

            // Lista dei system id dei documenti restituiti dalla ricerca
            WSConservazioneLocale.SearchResultInfo[] idProfiles = null;
            //
            // La ricerca dei documenti deve essere effettuata in base alla security
            // Il parametro security viene impostato a true.
            //documents = ConservazioneManager.getQueryInfoDocumentoPagingCustom(infoUtente, searchFilters, this.SelectedPage, out pageNumbers, out recordNumber, false, !IsPostBack, false, 10, false, null, null, out idProfiles);
            documents = ConservazioneManager.getQueryInfoDocumentoPagingCustom(infoUtente, searchFilters, this.SelectedPage, out pageNumbers, out recordNumber, true, !IsPostBack, false, 10, false, null, null, out idProfiles);

            try
            {
                int pgNum = 0;
                int recNum = 0;
                WSConservazioneLocale.SearchResultInfo[] idProf = null;
                // Metto in sessione tutti i documenti
                this.ResultAllDoc = ConservazioneManager.getQueryInfoDocumentoPagingCustom(infoUtente, searchFilters, this.SelectedPage, out pgNum, out recNum, true, !IsPostBack, false, 10, true, null, null, out idProf);
            }
            catch (Exception excep) 
            { 
                
            }

            // Memorizzazione del numero di risultati restituiti dalla ricerca, del numero di pagine e dei risultati
            this.RecordCount = recordNumber;
            this.PageCount = pageNumbers;
            this.Result = documents;

            return documents;
        }

        /// <summary>
        /// Al cambio di pagina, vengono caricati i documenti per la pagina selezionata
        /// e vengono visualizzati
        /// </summary>
        protected void dgResult_SelectedPageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            // Aggiornamento del numero di pagina memorizzato nel call context
            this.dgResult.CurrentPageIndex = e.NewPageIndex;
            this.SelectedPage = e.NewPageIndex + 1;

            // Ricerca dei documenti e visualizzazione dei risultati
            int number = 0;
            this.Result = SearchDocument(this.SearchFilters, this.SelectedPage, out number);

            this.dgResult.VirtualItemCount = number;
            this.dgResult.DataSource = this.Result;
            this.dgResult.DataBind();
            this.upRisultati.Update();
        }

        protected void dgResult_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        protected String GetSystemID(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectID;
        }

        protected String GetDataCreazione(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetTipo(WSConservazioneLocale.SearchObject temp)
        {
            string value = temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;

            if (value == "G")
                value = "NP";

            return value;
        }

        protected String GetIdSegnatura(WSConservazioneLocale.SearchObject doc)
        {
            string result = string.Empty;
            StringBuilder temp;
            string numeroDocumento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
            string numeroProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
            string dataProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
            string dataApertura = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
            string protTit = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("PROT_TIT")).FirstOrDefault().SearchObjectFieldValue;

            temp = new StringBuilder("<span style=\"color:");
            // Se il documento è un protocollo viene colorato in rosso altrimenti
            // viene colorato in nero
            temp.Append(String.IsNullOrEmpty(numeroProtocollo) ? "#333333" : "Red");
            // Il testo deve essere grassetto
            temp.Append("; font-weight:bold;\">");

            // Creazione dell'informazione sul documento
            if (!String.IsNullOrEmpty(numeroProtocollo))
            {
                temp.Append(numeroProtocollo);
                temp.Append("<br />");
                temp.Append(dataProtocollo);
            }
            else
            {
                temp.Append(numeroDocumento);
                temp.Append("<br />");
                temp.Append(dataApertura);
            }


            if (!String.IsNullOrEmpty(protTit))
                temp.Append("<br />" + protTit);

            // Chiusura del tag span
            temp.Append("</span>");

            result = temp.ToString();

            return result;
        }

        protected String GetOggetto(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D4")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetRegistro(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D2")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetTipoFile(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D23")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetTipologia(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("U1")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetIstanze(WSConservazioneLocale.SearchObject temp)
        {
            string istanze = string.Empty;

            WSConservazioneLocale.SearchObjectField field = temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ISTANZECONSERVAZIONE")).FirstOrDefault();
            //Gabriele Melini 26-09-2013
            //MEV CONS 1.4
            //se accedo al dettaglio dell'istanza dalla ricerca documenti
            //devo avere evidenza del documento di partenza
            WSConservazioneLocale.SearchObjectField docField = temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault();

            if (field != null)
            {
                string totaleIstanze = field.SearchObjectFieldValue;
                string idDoc = string.Empty;
                if (docField != null)
                    idDoc = docField.SearchObjectFieldValue;

                if (!string.IsNullOrEmpty(totaleIstanze))
                {
                    string[] singoleIstanze = totaleIstanze.Split('-');
                    if (singoleIstanze != null)
                    {
                        istanze = "<ul class=\"link_istanze\">";
                        for (int i = 0; i < singoleIstanze.Length; i++)
                        {
                            if (i != singoleIstanze.Length - 1)
                            {
                                if (!string.IsNullOrEmpty(idDoc))
                                    istanze += "<li class=\"spazioLi\"><a href=\"RicercaIstanzeConsEsibizione.aspx?id=" + singoleIstanze[i] + "&docNumber=" + idDoc + "\" title=\"Istanza numero " + singoleIstanze[i] + "\">" + singoleIstanze[i] + "</a></li>";
                                else
                                    istanze += "<li class=\"spazioLi\"><a href=\"RicercaIstanzeConsEsibizione.aspx?id=" + singoleIstanze[i] + "\" title=\"Istanza numero " + singoleIstanze[i] + "\">" + singoleIstanze[i] + "</a></li>";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(idDoc))
                                    istanze += "<li><a href=\"RicercaIstanzeConsEsibizione.aspx?id=" + singoleIstanze[i] + "&docNumber=" + idDoc + "\" title=\"Istanza numero " + singoleIstanze[i] + "\">" + singoleIstanze[i] + "</a></li>";
                                else
                                    istanze += "<li><a href=\"RicercaIstanzeConsEsibizione.aspx?id=" + singoleIstanze[i] + "\" title=\"Istanza numero " + singoleIstanze[i] + "\">" + singoleIstanze[i] + "</a></li>";
                            }
                        }
                        istanze += "</ul>";
                    }
                }
            }

            return istanze;
        }

        protected String GetListaIstanze(WSConservazioneLocale.SearchObject temp)
        {
            string istanze = string.Empty;

            WSConservazioneLocale.SearchObjectField field = temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ISTANZECONSERVAZIONE")).FirstOrDefault();
            
            // Lista Istanze per quel documento
            if (field != null)
            {
                istanze = field.SearchObjectFieldValue;
            }

            return istanze;
        }

        protected void ChangeTypeDocument(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddl_type_documents.SelectedValue))
            {
                this.btnCampiProfilati.OnClientClick = String.Format("OpenCampiProfilati('" + ddl_type_documents.SelectedValue + "');");
                this.btnCampiProfilati.Enabled = true;
                this.TemplateProf = null;
            }
            else
            {
                this.btnCampiProfilati.Enabled = false;
            }
            this.upFiltriRicerca.Update();
        }

        protected void ViewCampiProlilati(object sender, EventArgs e)
        {

        }

        protected string UrlCampiProfilati
        {
            get
            {
                return "../PopUp/CampiProfilati.aspx?type=D";
            }

        }

        protected string UrlChooseProject
        {
            get
            {
                return "../ChooseProject.aspx";
            }

        }

        protected int SelectedPage
        {
            get
            {
                int toReturn = 20;
                if (HttpContext.Current.Session["selectedPage"] != null)
                    toReturn = Convert.ToInt32(HttpContext.Current.Session["selectedPage"].ToString());

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["selectedPage"] = value;
            }
        }

        protected int RecordCount
        {
            get
            {
                int toReturn = 20;
                if (HttpContext.Current.Session["recordCount"] != null)
                    toReturn = Convert.ToInt32(HttpContext.Current.Session["recordCount"].ToString());

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["recordCount"] = value;
            }
        }

        protected WSConservazioneLocale.FiltroRicerca[][] SearchFilters
        {
            get
            {
                return HttpContext.Current.Session["searchFilters"] as WSConservazioneLocale.FiltroRicerca[][];

            }

            set
            {
                HttpContext.Current.Session["searchFilters"] = value;
            }
        }

        /// <summary>
        /// Numero di pagine restituiti dalla ricerca
        /// </summary>
        protected int PageCount
        {
            get
            {
                int toReturn = 1;

                if (HttpContext.Current.Session["PageCount"] != null &&
                    Int32.TryParse(
                        HttpContext.Current.Session["PageCount"].ToString(),
                        out toReturn)) ;

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["PageCount"] = value;
            }
        }

        /// <summary>
        /// Risultati restituiti dalla ricerca.
        /// </summary>
        public WSConservazioneLocale.SearchObject[] Result
        {
            get
            {
                return HttpContext.Current.Session["Result"] as WSConservazioneLocale.SearchObject[];
            }
            set
            {
                HttpContext.Current.Session["Result"] = value;
            }
        }

        /// <summary>
        /// Tutti i Risultati restituiti dalla ricerca.
        /// </summary>
        public WSConservazioneLocale.SearchObject[] ResultAllDoc
        {
            get
            {
                return HttpContext.Current.Session["ResultAllDoc"] as WSConservazioneLocale.SearchObject[];
            }
            set
            {
                HttpContext.Current.Session["ResultAllDoc"] = value;
            }
        }

        /// <summary>
        /// Template selezionato
        /// </summary>
        protected WSConservazioneLocale.Templates TemplateProf
        {
            get
            {
                return HttpContext.Current.Session["TemplateProf"] as WSConservazioneLocale.Templates;
            }
            set
            {
                HttpContext.Current.Session["TemplateProf"] = value;
            }
        }

        /// <summary>
        /// Metodo seleziona-deseleziona tutti
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Seleziona_DeselezionaTutti(Object sender, EventArgs e)
        {
            CheckBox cbSelectAll = sender as CheckBox;
            #region SELECTED_ELEMENT_DICTIONARY

            // Gestione dictionary
            Dictionary<string, string> SelectedElementDictionary = null;

            // Dictionary presente in sessione (Già ho selezionato un elemento)
            //if (HttpContext.Current.Session["SelectedElementDictionary"] != null)
            if (Session["SelectedElementDictionary"] != null)
            {
                //Recupero dictionary dalla sessione
                //SelectedElementDictionary = HttpContext.Current.Session["SelectedElementDictionary"] as Dictionary<string, string>;
                SelectedElementDictionary = Session["SelectedElementDictionary"] as Dictionary<string, string>;
            }
            else
            {
                // Creo il dictionary <objectSelected, idIstanza>
                SelectedElementDictionary = new Dictionary<string, string>();
            }

            #endregion

            // Recupero gli oggetti risultato della ricerca
            //foreach (ConservazioneWA.WSConservazioneLocale.SearchObject item in this.Result)
            foreach (ConservazioneWA.WSConservazioneLocale.SearchObject item in this.ResultAllDoc)
            {
                string systemId = item.SearchObjectID;
                string istanze = string.Empty;

                #region RECUPERO LISTA ISTANZE OGGETTO
                ConservazioneWA.WSConservazioneLocale.SearchObjectField field = item.SearchObjectField.Where(x => x.SearchObjectFieldID.Equals("ISTANZECONSERVAZIONE")).FirstOrDefault();
                // Lista Istanze per quel documento
                if (field != null)
                {
                    istanze = field.SearchObjectFieldValue;
                }
                #endregion

                string istanzaDefault = istanze.Split('-')[0];

                if (!cbSelectAll.Checked)
                {
                    // Elementi erano tutti selezionati e devono essere deselezionati
                    if (SelectedElementDictionary.ContainsKey(systemId))
                        SelectedElementDictionary.Remove(systemId);
                }
                else
                {
                    // Elementi devono essere tutti selezionati

                    // Inserimento in dictionary
                    if (!SelectedElementDictionary.ContainsKey(systemId))
                        SelectedElementDictionary.Add(systemId, istanzaDefault);
                }
            }

            // Metto il dictionary in sessione
            //HttpContext.Current.Session["SelectedElementDictionary"] = SelectedElementDictionary;
            Session["SelectedElementDictionary"] = SelectedElementDictionary;

            // Aggiorno il datagrid
            int VirtualItemCount = dgResult.VirtualItemCount;

            this.dgResult.VirtualItemCount = VirtualItemCount;
            this.dgResult.CurrentPageIndex = 0;
            this.dgResult.DataSource = this.Result;
            this.dgResult.DataBind();


            this.pnl_result.Visible = true;
            this.upRisultati.Update();

        }

        /// <summary>
        /// Invio massivo in esibizione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void InvioEsibMass(Object sender, EventArgs args)
        {
            Dictionary<string, string> SelectedElementDictionary = null;

            // Recupero dalla sessione il dictionary con gli elementi selezionati
            //if (HttpContext.Current.Session["SelectedElementDictionary"] != null)
            if (Session["SelectedElementDictionary"] != null)
                //Recupero dictionary dalla sessione
                //SelectedElementDictionary = HttpContext.Current.Session["SelectedElementDictionary"] as Dictionary<string, string>;
                SelectedElementDictionary = Session["SelectedElementDictionary"] as Dictionary<string, string>;

            // Per ogni elemento del dictionary eseguo l'operazione puntuale
            if (SelectedElementDictionary != null && SelectedElementDictionary.Count > 0)
            {
                // Dictionary Aggiornato
                Dictionary<string, string> SelectedElementDictionaryUpdated = new Dictionary<string, string>();

                // Ricontrollo che l'utente non abbia cambiato la selezione
                foreach (KeyValuePair<string, string> KeyValue in SelectedElementDictionary)
                {
                    // Recupero il dictionary che contiene la scelta istanze
                    #region RECUPERO_PARAMETRI_DICTIONARY_SCELTA_ISTANZE

                    Dictionary<string, string> dictIstSelected = null;
                    // Dictionary presente in sessione (Scelta istanze di conservazione)
                    if (HttpContext.Current.Session["dictIstSelected"] != null)
                    {
                        //Controllo che il dictionary per quell'oggetto contiene già l'item selected
                        dictIstSelected = HttpContext.Current.Session["dictIstSelected"] as Dictionary<string, string>;

                        //Dictionary già popolato
                        if (dictIstSelected.ContainsKey(KeyValue.Key))
                        {
                            // Aggiorno il valore dell'istanza selezionata per ultimo
                            //SelectedElementDictionary[KeyValue.Key] = dictIstSelected[KeyValue.Key];
                            string value = dictIstSelected[KeyValue.Key];
                            SelectedElementDictionaryUpdated.Add(KeyValue.Key, value);
                        }
                        else
                            SelectedElementDictionaryUpdated.Add(KeyValue.Key, KeyValue.Value);
                    }
                    else
                        SelectedElementDictionaryUpdated.Add(KeyValue.Key, KeyValue.Value);

                    #endregion
                }

                #region variabili Riepilogo Massivo

                List<string> ListIdObject_OK = new List<string>();
                List<string> ListIdObject_KO = new List<string>();
                List<string> ListIdObject_warning = new List<string>();
                // Dictionary da passare in sessione
                Dictionary<string, WSConservazioneLocale.ItemsConservazione> dictRiepilogoMass = new Dictionary<string, WSConservazioneLocale.ItemsConservazione>();

                #endregion

                // Ora tutti i valori del dictionary degli elemmenti selezionati sono aggiornati
                // Procedo con l'invio in esibizione:
                //foreach (KeyValuePair<string, string> KeyValue in SelectedElementDictionary)
                foreach (KeyValuePair<string, string> KeyValue in SelectedElementDictionaryUpdated)
                {
                    #region CODE_EXECUTE_INVIO_ESIBIZIONE

                    string param_idIstanza = string.Empty;
                    string param_idOggetto = string.Empty;

                    param_idIstanza = KeyValue.Value;
                    param_idOggetto = KeyValue.Key;

                    // Metodo che fa l'invio
                    string IdItemsEsibizione = "-1";
                    string eccezione = string.Empty;
                    try
                    {
                        //
                        // Get info utili per fare la chiamata al BE

                        string idGruppo = string.Empty;

                        // Recupero dalla sessione l'idGruppo selezionato
                        if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
                            // Recupero l'idGruppo selezionato
                            //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                            idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();

                        WSConservazioneLocale.ItemsConservazione[] itemsCons = ConservazioneManager.getItemsConservazioneWithSecurity(param_idIstanza, this.infoUtente, idGruppo);

                        #region Elemento_Selezionato
                        WSConservazioneLocale.ItemsConservazione item;
                        try
                        {
                            if (itemsCons != null && itemsCons.Length > 0)
                                // Recupero elemneto
                                item = itemsCons.FirstOrDefault(x => x.ID_Profile == param_idOggetto);
                            else
                                // Elemnto non trovato; Caso idItemCons = -1
                                item = new WSConservazioneLocale.ItemsConservazione();
                        }
                        catch (Exception exc)
                        {
                            item = new WSConservazioneLocale.ItemsConservazione();
                        }

                        // Aggiungo elemento al dictionary
                        if (!dictRiepilogoMass.ContainsKey(param_idOggetto))
                            dictRiepilogoMass.Add(param_idOggetto, item);
                        #endregion

                        //
                        // Occorre recuperare i valori dell'elemento (istanza di conservazione) selezionato
                        if (itemsCons != null && itemsCons.Length > 0)
                        {
                            // Mi serve per recuperare l'elemento selezionato
                            string idProfile = param_idOggetto;

                            if (!string.IsNullOrEmpty(idProfile))
                            {
                                // Occorre reperire i parametri da passare alla chiamata di Backend
                                // Costruzione dei Parametri
                                var query = from p in itemsCons.ToList()
                                            where p.ID_Profile == idProfile
                                            select new { p.ID_Profile, p.ID_Project, p.DocNumber, p.tipo_oggetto };

                                string id_profile = query.FirstOrDefault().ID_Profile;
                                string id_project = string.Empty;
                                string doc_number = query.FirstOrDefault().DocNumber;
                                //string tipo_oggetto = query.FirstOrDefault().tipo_oggetto;
                                string tipo_oggetto = "D";

                                //string id_profile = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).ID_Profile;
                                //string id_project = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).ID_Project;
                                //string doc_number = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).DocNumber;
                                //string tipo_oggetto = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).tipo_oggetto;

                                string idIstanzaConservazione = param_idIstanza;

                                // Verifico la presenza del documento con idProfile nell'istanza di esibizione con idIstanzaConservazione da cui è stato prelevato il file 
                                // e per lo stesso idPeople e idGruppo
                                bool docPresente = ConservazioneManager.checkItemEsibizionePresenteInIstanzaEsibizione(id_profile, id_project, tipo_oggetto, infoUtente, idIstanzaConservazione);

                                if (!docPresente)
                                {
                                    WSConservazioneLocale.SchedaDocumento documentDetails = null;

                                    // Il metodo deve invocare la Stored Procedure SP_INSERT_AREA_ESIB
                                    IdItemsEsibizione = ConservazioneManager.CreateAndAddDocInAreaEsibizione(id_profile, id_project, doc_number, infoUtente, tipo_oggetto, idIstanzaConservazione, out documentDetails);

                                    try
                                    {
                                        int size_xml = ConservazioneManager.getItemSizeEsib(
                                            this.Page,
                                            documentDetails,
                                            IdItemsEsibizione);

                                        int doc_size = Convert.ToInt32(documentDetails.documenti[0].fileSize);

                                        int numeroAllegati = documentDetails.allegati.Length;
                                        string fileName = documentDetails.documenti[0].fileName;
                                        string tipoFile = Path.GetExtension(fileName);
                                        int size_allegati = 0;
                                        for (int i = 0; i < documentDetails.allegati.Length; i++)
                                        {
                                            size_allegati = size_allegati + Convert.ToInt32(documentDetails.allegati[i].fileSize);
                                        }
                                        int total_size = size_allegati + doc_size + size_xml;

                                        ConservazioneManager.insertSizeInItemEsib(Page, IdItemsEsibizione, total_size);

                                        ConservazioneManager.updateItemsEsibizione(
                                            this.Page,
                                            tipoFile,
                                            Convert.ToString(numeroAllegati),
                                            IdItemsEsibizione);
                                    }
                                    catch (Exception exc1)
                                    {
                                    }
                                }
                                else
                                {
                                    // Documento già presente
                                    IdItemsEsibizione = "-2";
                                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Invio_Esibizione_OK", "<script>alert('Attenzione, Documento già presente.');</script>", false);
                                    //
                                    // Popolo la lista degli elementi che non possono essere inseriti in esibizione
                                    //ListIdObject_warning.Add(KeyValue.Key);
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        IdItemsEsibizione = "-1";
                        eccezione = exc.Message;
                    }

                    // esito -1 -> KO
                    if (!string.IsNullOrEmpty(IdItemsEsibizione) && IdItemsEsibizione.Equals("-1"))
                    {
                        //
                        // Popolo la lista di elementi con esito negativo
                        ListIdObject_KO.Add(KeyValue.Key);

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(IdItemsEsibizione) && IdItemsEsibizione.Equals("-2"))
                            //
                            // Lista elementi già presenti
                            ListIdObject_warning.Add(KeyValue.Key);
                        else
                            //
                            // Lista elementi ok.
                            ListIdObject_OK.Add(KeyValue.Key);
                    }
                    #endregion

                }// End foreach

                // Tutte le liste sono popolate
                // Devo lanciare la popup con il riepilogo degli esiti
                // TO DO
                #region TEMPORANEA_OLDCODE

                //string idOK = string.Empty;
                //string idKO = string.Empty;
                //string warning = string.Empty;

                //foreach (string itm in ListIdObject_OK) 
                //{
                //    idOK = idOK + itm + "; ";
                //}

                //foreach (string itm in ListIdObject_KO)
                //{
                //    idKO = idKO + itm + "; ";
                //}

                //foreach (string itm in ListIdObject_warning)
                //{
                //    warning = warning + itm + "; ";
                //}

                //ScriptManager.RegisterStartupScript(
                //    this.Page,
                //    this.GetType(),
                //    "InvioEsibMass_Esito", "<script>alert('Elementi OK: " + idOK +
                //    //Environment.NewLine +
                //    "\\r\\n" +
                //    "Elementi precedentemente inseriti: " + warning +
                //    //Environment.NewLine +
                //    "\\r\\n" +
                //    "Elementi KO: " + idKO + "');</script>", false);

                #endregion

                #region Sessione Riepilogo Massivo
                // Metto in sessione:

                // le liste 
                Session["ListIdObject_OK"] = ListIdObject_OK;
                Session["ListIdObject_warning"] = ListIdObject_warning;
                Session["ListIdObject_KO"] = ListIdObject_KO;

                // Il dictionary
                Session["dictRiepilogoMass"] = dictRiepilogoMass;
                #endregion

                ScriptManager.RegisterStartupScript(this,
                            this.GetType(),
                            "Apri_Riepilogo",
                            "showDialogMassiveOperation();",
                            true);

            }
            else
            {
                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.GetType(),
                    "InvioEsibMass_KO", "<script>alert('Selezionare almeno un elemento.');</script>", false);
            }
        }

        /// <summary>
        /// Metodo per verificare se elemento è nel dictionary SelectedElementDictionary
        /// </summary>
        /// <param name="idObject"></param>
        /// <returns></returns>
        protected bool Is_SystemIDDoc_InDictionary(string idObject)
        {
            bool result;
            Dictionary<string, string> SelectedElementDictionary = Session["SelectedElementDictionary"] as Dictionary<string, string>;

            if (SelectedElementDictionary != null)
            {
                // Esiste il dictionary
                if (SelectedElementDictionary.ContainsKey(idObject))
                {
                    // Elemento selezionato
                    result = true;
                }
                else
                {
                    // Elemento non selezionato
                    result = false;
                }
            }
            else
                result = false;

            return result;
        }

        /// <summary>
        /// Metodo per inserire elementi nel dictionary SelectedElementDictionary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void Insert_SystemIDDoc_InDictionary(Object sender, EventArgs args)
        {
            this.cbSelectAll.Checked = false;

            string param_idOggetto = string.Empty;
            string param_idIstanza = string.Empty;

            CheckBox cbMassivo = sender as CheckBox;

            #region RECUPERO_PARAMETRI_DICTIONARY_SCELTA_ISTANZE
            // Se non apro la Popup di scelta istanze, viene presa la prima istanza di default

            //
            // La lista Istanze è caratterizzata da:
            // idIstanza1-idIstanza2-idIstanza3
            string listaIstanze = cbMassivo.CssClass.ToString().Split('$')[1];
            string idObject = cbMassivo.CssClass.ToString().Split('$')[0];

            // Imposto la prima istanza cme default
            string idIstanzaDefault = listaIstanze.ToString().Split('-')[0];

            // imposto Object come parametro da passare al metodo per l'invio in esibizione 
            param_idOggetto = idObject;

            // Gestione dictionary
            Dictionary<string, string> dictIstSelected = null;

            // Dictionary presente in sessione (Scelta istanze di conservazione)
            if (HttpContext.Current.Session["dictIstSelected"] != null)
            {
                //Controllo che il dictionary per quell'oggetto contiene già l'item selected
                dictIstSelected = HttpContext.Current.Session["dictIstSelected"] as Dictionary<string, string>;

                //Dictionary già popolato
                if (dictIstSelected.ContainsKey(idObject))
                {
                    // Recupero il valore dell'istanza selezionata per ultimo
                    param_idIstanza = dictIstSelected[idObject];
                }
                else
                {
                    // Valore Dictionary non presente
                    //Aggiungo valore di default al dictionary
                    dictIstSelected[idObject] = idIstanzaDefault;
                    param_idIstanza = idIstanzaDefault;
                }
            }
            else
            {
                // Creo il dictionary <objectSelected, idIstanza>
                dictIstSelected = new Dictionary<string, string>();
                //Aggiungo valore di default al dictionary
                dictIstSelected[idObject] = idIstanzaDefault;
                param_idIstanza = idIstanzaDefault;
            }
            // Metto il dictionary in sessione
            HttpContext.Current.Session["dictIstSelected"] = dictIstSelected;
            #endregion

            // Dopo aver recuperato l'istanza per quell'oggetto (param_idIstanza), gestisco il dictionary di selezione elemento
            #region SELECTED_ELEMENT_DICTIONARY

            // Gestione dictionary
            Dictionary<string, string> SelectedElementDictionary = null;

            // Dictionary presente in sessione (Già ho selezionato un elemento)
            //if (HttpContext.Current.Session["SelectedElementDictionary"] != null)
            if (Session["SelectedElementDictionary"] != null)
            {
                //Recupero dictionary dalla sessione
                //SelectedElementDictionary = HttpContext.Current.Session["SelectedElementDictionary"] as Dictionary<string, string>;
                SelectedElementDictionary = Session["SelectedElementDictionary"] as Dictionary<string, string>;
            }
            else
            {
                // Creo il dictionary <objectSelected, idIstanza>
                SelectedElementDictionary = new Dictionary<string, string>();
            }

            #endregion

            // Inserimento elementi in dictionary
            if (!cbMassivo.Checked)
            {
                // Ho deselezionato l'elemento
                // Elemento già inserito nel dictionary
                if (SelectedElementDictionary.ContainsKey(param_idOggetto))
                    SelectedElementDictionary.Remove(param_idOggetto);
            }
            else
            {
                // Ho selezionato l'elemento
                // Elemento deve essere inserito
                if (!SelectedElementDictionary.ContainsKey(param_idOggetto))
                    SelectedElementDictionary.Add(param_idOggetto, param_idIstanza);
            }

            // Metto il dictionary in sessione
            //HttpContext.Current.Session["SelectedElementDictionary"] = SelectedElementDictionary;
            Session["SelectedElementDictionary"] = SelectedElementDictionary;
        }
    }
}