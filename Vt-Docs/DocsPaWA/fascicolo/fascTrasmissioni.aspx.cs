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
using System.Xml;
using System.IO;
using System.Globalization;
using System.Configuration;
using DocsPAWA.DocsPaWR;

//Andrea
using DocsPAWA.utils;
//End Andrea


namespace DocsPAWA.fascicolo
{
    /// <summary>
    /// Summary description for p_Trasmissioni.
    /// </summary>
    public class fasc_Trasmissioni : DocsPAWA.CssPage
    {
        //Andrea
        private ArrayList listaExceptionTrasmissioni = new ArrayList();
        private string messError = "";
        //End Andrea

        protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
        protected System.Web.UI.HtmlControls.HtmlForm f_Ricerca;
        protected DocsPAWA.DocsPaWR.Utente userHome;
        protected DocsPAWA.DocsPaWR.Trasmissione trasmissioneDoc;
        protected DocsPAWA.DocsPaWR.Trasmissione[] listaTrasmDocEff;
        protected DocsPAWA.DocsPaWR.Trasmissione[] listaTrasmDocRic;
        protected Hashtable HashTrasmEff;
        protected Hashtable HashTrasmRic;
        protected System.Web.UI.HtmlControls.HtmlForm f1;
        protected string idDoc;
        protected int DimTable;
        protected ListItemCollection listElemRadio;
        protected int numriga;
        protected int appoNumRigaSel;
        protected DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] listaFiltri;
        protected XmlElement xmlOggettoTrasm;
        protected XmlElement xmlListaFiltri;
        protected System.Web.UI.WebControls.Button ButtSel;
        protected System.Web.UI.WebControls.Button btn_ElDettDest;
        protected System.Web.UI.WebControls.Button btn_ElCancDest;
        protected DocsPAWA.DocsPaWR.InfoFascicolo infoFasc;
        protected DocsPAWA.DocsPaWR.FileDocumento fileDoc;
        protected DocsPAWA.DocsPaWR.FileRequest fileRequest;
        protected Hashtable HashInfoDoc;
        protected System.Web.UI.WebControls.Label lbl_ruolo;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        protected System.Web.UI.WebControls.Image img_statoReg;
        protected DocsPAWA.DocsPaWR.Registro RegScelto;
        protected DocsPAWA.DocsPaWR.InfoUtente Safe;
        protected Utilities.MessageBox msg_TrasmettiFasc;
        protected System.Web.UI.WebControls.DataGrid DataGrid1;

        protected System.Web.UI.WebControls.DataGrid Datagrid2;

        protected DocsPaWebCtrlLibrary.ImageButton btn_nuovaT;
        protected System.Web.UI.WebControls.RadioButtonList rbl_tipoTrasm;
        protected DocsPaWebCtrlLibrary.ImageButton btn_NuovaTrasm;
        protected DocsPaWebCtrlLibrary.ImageButton btn_ModifTrasm;
        protected DocsPaWebCtrlLibrary.ImageButton butt_Trasm;
        protected DocsPAWA.dataSet.DS_EFF dS_EFF1;
        protected DocsPAWA.dataSet.DS_RIC dS_RIC1;
        protected System.Web.UI.WebControls.Label lbl_message;
        protected System.Web.UI.WebControls.DropDownList ddl_tmpl;
        protected System.Web.UI.WebControls.Panel pnl_trasm_rapida;
        protected DocsPaWebCtrlLibrary.ImageButton btn_salva_trasm;
        protected System.Web.UI.WebControls.HyperLink stampa;
        protected DocsPaWebCtrlLibrary.ImageButton btn_stampa;
        protected System.Web.UI.HtmlControls.HtmlInputHidden flag_template;
        protected DocsPaWebCtrlLibrary.ImageButton btn_trasmettiDisabled;
        protected DocsPaWR.Fascicolo fasc;
        protected DocsPaWebService wws = new DocsPaWebService();
        protected static string separatore = "----------------";
        protected bool userAutorizedEditingACL;
        protected System.Web.UI.WebControls.HiddenField estendiVisibilita;
        protected System.Web.UI.WebControls.HiddenField abilitaModaleVis;


        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                //gestione finestra attendere prego
                //const string onClickBtn = "javascript:_buttonClicked=this.id;";
                //butt_Trasm.Attributes["onClick"] = onClickBtn;

                Utils.startUp(this);
                userRuolo = UserManager.getRuolo(this);
                userHome = UserManager.getUtente(this);
                
                fasc = FascicoliManager.getFascicoloSelezionato(this);
                // Inizializzazione controllo verifica acl
                if ((fasc != null) && (fasc.systemID != null))
                {

                    //Modifica per bug: creando un nuovo fascicolo da ricerca fascicoli si ha accessrights a null
                    if (fasc.accessRights == null)
                    {
                        DocsPAWA.DocsPaWR.InfoUtente infoUtenteTemp = UserManager.getInfoUtente();
                        fasc.accessRights = wws.getAccessRightFascBySystemID(fasc.systemID, infoUtenteTemp);
                    }

                    this.InitializeControlAclFascicolo();
                }

                if (fasc == null || fasc.systemID == null || fasc.systemID.Equals(""))
                {
                    this.btn_NuovaTrasm.Enabled = false;
                    this.btn_ModifTrasm.Enabled = false;
                    this.butt_Trasm.Enabled = false;
                    return;
                }

                // gestione cessione diritti            
                this.checkIsAutorizedEditingACL();

                if (!IsPostBack)
                {
                    Session.Remove("Modello");
                    caricaModelliTrasm();
                    this.caricaComboTemplate();
                    Session["Controllati"] = null;
                }

                if (this.GetCurrentTipoTrasmissione() != rbl_tipoTrasm.SelectedValue)
                {
                    this.FillDataTrasmissioni();
                }
            }

            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
                
            }

            
            if (Session["doTrasm"] != null)
            {
                DocsPaWR.Trasmissione trasmissione = (DocsPAWA.DocsPaWR.Trasmissione)Session["doTrasm"];
                Session.Remove("doTrasm");

                string idTrasmissione = trasmissione.systemId;
                DocsPaWR.Trasmissione[] listaTrasmissioni = TrasmManager.getDocTrasmQueryEff(this);

                // Trova e trasmetti la trasmissione
                if (listaTrasmissioni != null && listaTrasmissioni.Length > 0)
                {
                    for (int i = 0; i < listaTrasmissioni.Length; i++)
                    {
                        if (listaTrasmissioni[i].systemId == idTrasmissione)
                        {
                            TrasmManager.setGestioneTrasmissione(this, listaTrasmissioni[i]);
                            break;
                        }
                    }

                    // Trasmetti il documento
                    this.butt_Trasm_Click(null, null);

                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Nessuna trasmissione in sessione!");
            }

            //Andrea
            if (Session["MessError"] != null)
            {
                    messError = Session["MessError"].ToString();
                    //Response.Write("<script language=\"javascript\">alert('Trasmissioni avvenute con successo per tutti i destinatari ad eccezione delle seguenti: \\n" + messError + "');</script>");
                    Response.Write("<script language=\"javascript\">alert('Trasmissioni con esito negativo: \\n" + messError + "\\n');</script>");
                    Session.Remove("MessError");
            }
            //End Andrea

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo">E=effettuate o R=ricevute</param>
        private void viewInfoFirstRow(string tipo, int index)
        {
            switch (tipo)
            {
                case "E":
                    if (DataGrid1.Items.Count > 0)
                    {
                        this.DataGrid1.SelectedIndex = index;
                        DataGrid1_SelectedIndexChanged(DataGrid1, new System.EventArgs());
                    }
                    break;
                case "R":
                    if (Datagrid2.Items.Count > 0)
                    {
                        this.Datagrid2.SelectedIndex = index;
                        DataGrid2_SelectedIndexChanged(Datagrid2, new System.EventArgs());
                    }
                    break;
            }
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {

            //abilitazione delle funzioni in base al ruolo
            UserManager.disabilitaFunzNonAutorizzate(this);

            //chiusura della finestra di attesa
            ClientScript.RegisterStartupScript(this.GetType(), "chiusuraProgrMsgFascTrasmissioni", "hideWorkingInProgress('fascTrasmissioni');", true);
            ClientScript.RegisterStartupScript(this.GetType(), "chiusuraProgrMsgTrasmFascDatiTrasm", "hideWorkingInProgress('trasmFascDatiTrasm');", true);
            
        }


        //TRASMISSIONI

        /// <summary>
        /// reperimento pagina correntemente visualizzata
        /// </summary>
        /// <returns></returns>
        private int GetCurrentPageIndex()
        {
            if (this.rbl_tipoTrasm.SelectedValue.Equals("E"))
                return this.DataGrid1.CurrentPageIndex + 1;
            else
                return this.Datagrid2.CurrentPageIndex + 1;
        }

        private void FillDataTrasmissioni()
        {
            this.SetCurrentTipoTrasmissione(this.rbl_tipoTrasm.SelectedValue);

            TrasmManager.removeDocTrasmSel(this);

            int totalPageNumber, totalRecordCount, requestedPageNumber;

            requestedPageNumber = this.GetCurrentPageIndex();

            DocsPaWR.Trasmissione[] trasmissioni = null;

            if (this.rbl_tipoTrasm.SelectedValue == "E")
            {
                trasmissioni = this.GetTrasmissioniEffettuate(requestedPageNumber, out totalPageNumber, out totalRecordCount);
                TrasmManager.setDocTrasmQueryEff(this, trasmissioni);
            }
            else
            {
                System.Collections.Generic.List<DocsPAWA.DocsPaWR.FiltroRicerca> filters = new System.Collections.Generic.List<DocsPAWA.DocsPaWR.FiltroRicerca>();
                DocsPAWA.DocsPaWR.FiltroRicerca item = new DocsPAWA.DocsPaWR.FiltroRicerca();
                item.argomento = "TAB_TRASMISSIONI";
                item.valore = "TRUE";
                filters.Add(item);
                oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();
                oggettoTrasm.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(fasc, this);
                trasmissioni = TrasmManager.getQueryRicevutePaging(this, oggettoTrasm, this.userHome, this.userRuolo, filters.ToArray(), requestedPageNumber, out totalPageNumber, out totalRecordCount);
               // trasmissioni = this.GetTrasmissioniRicevute(requestedPageNumber, out totalPageNumber, out totalRecordCount);
                TrasmManager.setDocTrasmQueryRic(this, trasmissioni);
            }

            // Mev Ospedale Maggiore Policlinico
            if (trasmissioni != null && trasmissioni.Length > 0)
                this.btn_stampa.Enabled = true;
            else
                this.btn_stampa.Enabled = false;
            // End Mev Opsedale Maggiore Policlinico

            int index = 0;
            foreach (DocsPAWA.DocsPaWR.Trasmissione trasm in trasmissioni)
            {
                CaricaDataGrid(trasm, index, this.rbl_tipoTrasm.SelectedValue);
                index++;
            }

            this.DataGrid1.Visible = false;
            this.Datagrid2.Visible = false;

            // disabilito i pulsanti
            this.btn_ModifTrasm.Enabled = false;
            this.butt_Trasm.Enabled = false;

            if (this.rbl_tipoTrasm.SelectedValue == "E")
            {
                this.DataGrid1.VirtualItemCount = totalRecordCount;
                this.DataGrid1.DataSource = this.dS_EFF1;
                this.DataGrid1.DataBind();

                this.pnl_trasm_rapida.Visible = true;

                if (DataGrid1.Items.Count > 0)
                    this.DataGrid1.Visible = true;
                else
                    Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='tabTrasmissioniEffFasc.aspx?chiudi=no';</script>");
            }
            else
            {
                this.Datagrid2.VirtualItemCount = totalRecordCount;
                this.Datagrid2.DataSource = this.dS_RIC1;
                this.Datagrid2.DataBind();

                this.pnl_trasm_rapida.Visible = false;

                if (Datagrid2.Items.Count > 0)
                    this.Datagrid2.Visible = true;
                else
                    Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='tabTrasmissioniRicFasc.aspx';</script>");
            }

            // visualizzazione numero elementi trovati
            this.SetMsgTrasmissioniTrovate(totalRecordCount);

            // visualizzazione dettagli prima trasmissione
            viewInfoFirstRow(this.rbl_tipoTrasm.SelectedValue, 0);
        }

        private void SetMsgTrasmissioniTrovate(int totalRecordCount)
        {
            if (totalRecordCount > 1)
                this.lbl_message.Text = "Elenco trasmissioni - Trovati " + totalRecordCount + " elementi.";
            else if (totalRecordCount == 1)
                this.lbl_message.Text = "Elenco trasmissioni - Trovato 1 elemento.";
            else
                this.lbl_message.Text = "Trasmissioni non trovate";
        }

        private DocsPAWA.DocsPaWR.Trasmissione[] GetTrasmissioniEffettuate(int pageNumber, out int totalPageNumber, out int recordCount)
        {
            oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();
            oggettoTrasm.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(fasc, this);
            return TrasmManager.getQueryEffettuateDocumentoPaging(this, oggettoTrasm, this.userHome, this.userRuolo, null, pageNumber, out totalPageNumber, out recordCount);
        }

        private DocsPAWA.DocsPaWR.Trasmissione[] GetTrasmissioniRicevute(int pageNumber, out int totalPageNumber, out int recordCount)
        {
            oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();
            oggettoTrasm.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(fasc, this);
            return TrasmManager.getQueryRicevutePaging(this, oggettoTrasm, this.userHome, this.userRuolo, null, pageNumber, out totalPageNumber, out recordCount);
        }

        protected void queryTrasmissioni(string tipoTrasm)
		{
			try
			{
				this.SetCurrentTipoTrasmissione(this.rbl_tipoTrasm.SelectedValue);
				oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();
				oggettoTrasm.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(fasc,this);
						
				//chiamo web services get trasm effettuate:
				if (tipoTrasm.Equals("E"))
				{
					listaTrasmDocEff =  TrasmManager.getQueryEffettuate(this,oggettoTrasm, this.userHome,this.userRuolo,null);
					TrasmManager.setDocTrasmQueryEff(this,listaTrasmDocEff);
					
				}
				else
					if (tipoTrasm.Equals("R"))
				{
					listaTrasmDocRic = TrasmManager.getQueryRicevute(this,oggettoTrasm, this.userHome,this.userRuolo,null);
					TrasmManager.setDocTrasmQueryRic(this,listaTrasmDocRic);
				}
			}
		
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

        private void PerformTrasmFolder()
        {
            int indexSel = ddl_tmpl.Items.IndexOf(ddl_tmpl.Items.FindByValue(separatore));

            if (this.ddl_tmpl.SelectedIndex > 0 && ddl_tmpl.SelectedIndex < indexSel)
            {                
                //MODELLI TRASMISSIONE NUOVI
                if (Session["Modello"] != null)
                {
                    DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

                    if (!this.notificheUtImpostate(modello))
                    {
                        this.ddl_tmpl.SelectedIndex = -1;
                        Response.Write("<script language='javascript'>window.alert('Attenzione,\\nil modello selezionato presenta la seguente anomalia:\\n\\nDESTINATARI PRIVI DI NOTIFICA IMPOSTATA.\\n\\nImpostare le notifiche utenti per questo modello nella sezione Gestione > Modelli tras.');</script>");
                        return;
                    }

                    Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();

                    //Parametri della trasmissione
                    trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
                    trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                    trasmissione.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(fasc, this);

                    trasmissione.utente = UserManager.getUtente(this);
                    trasmissione.ruolo = UserManager.getRuolo(this);

                    if (modello != null)
                        trasmissione.NO_NOTIFY = modello.NO_NOTIFY;

                    // gestione della cessione diritti
                    if (modello.CEDE_DIRITTI != null && modello.CEDE_DIRITTI.Equals("1"))
                    {
                        DocsPaWR.CessioneDocumento objCessione = new DocsPAWA.DocsPaWR.CessioneDocumento();

                        objCessione.docCeduto = true;
                        objCessione.idPeople = UserManager.getInfoUtente(this).idPeople;
                        objCessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;
                        objCessione.userId = UserManager.getInfoUtente(this).userId;
                        if (modello.ID_PEOPLE_NEW_OWNER != null && modello.ID_PEOPLE_NEW_OWNER != "")
                            objCessione.idPeopleNewPropr = modello.ID_PEOPLE_NEW_OWNER;
                        if (modello.ID_GROUP_NEW_OWNER != null && modello.ID_GROUP_NEW_OWNER != "")
                            objCessione.idRuoloNewPropr = modello.ID_GROUP_NEW_OWNER;

                        //
                        // Mev Cessione diritti - mantieni scrittura
                        trasmissione.mantieniScrittura = (modello.MANTIENI_SCRITTURA == "1" ? true : false);
                        // end mev
                        //

                        trasmissione.mantieniLettura = (modello.MANTIENI_LETTURA=="1"? true:false );

                        trasmissione.cessione = objCessione;
                    }

                    //gestione fascicolo controllato
                    if (this.fascicoloControllato(fasc))
                    {
                        if (Session["Controllati"] != null)
                        {
                            string[] listaDocControllati = new string[(Session["Controllati"] as string[]).Length];
                            listaDocControllati = Session["Controllati"] as string[];
                            trasmissione.listaDocControllati = listaDocControllati;
                            Session["Controllati"] = null;
                        }
                    }

                    //Parametri delle trasmissioni singole
                    for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                    {
                        DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                        ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                        for (int j = 0; j < destinatari.Count; j++)
                        {
                            DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                            DocsPaWR.Corrispondente corr = new Corrispondente();
                            if (mittDest.CHA_TIPO_MITT_DEST == "D")
                            {
                                corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                            }
                            else
                            {
                                corr = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, null, fasc, this);
                            }
                            if (corr != null)
                            {
                                DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                                
                                //Andrea - try - catch
                                try
                                {
                                    trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA);
                                }
                                catch (ExceptionTrasmissioni e)
                                {
                                    //Aggiungo l'errore alla lista
                                    listaExceptionTrasmissioni.Add(e.Messaggio);

                                    //foreach (string s in listaExceptionTrasmissioni)
                                    //{
                                    //    //messError = messError + s + "\r\n";
                                    //    messError = messError + s + "|";
                                    //}

                                    //if (messError != "")
                                    //{
                                    //    Session.Add("MessError", messError);
                                    //}
                                }
                                //End Andrea
                            }
                        }
                    }
                    //Andrea
                    foreach (string s in listaExceptionTrasmissioni)
                    {
                        //messError = messError + s + "\r\n";
                        messError = messError + s + "\\n";
                    }

                    if (messError != "")
                    {
                        Session.Add("MessError", messError);
                    }
                    //End Andrea

                    DocsPaWR.Trasmissione t_rs = null;
                    if (trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Length > 0)
                    {
                        trasmissione = this.impostaNotificheUtentiDaModello(trasmissione);

                        if (estendiVisibilita.Value == "false")
                        {
                            TrasmissioneSingola[] appoTrasmSingole = new TrasmissioneSingola[trasmissione.trasmissioniSingole.Length];
                            for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                            {
                                TrasmissioneSingola trasmSing = new TrasmissioneSingola();
                                trasmSing = trasmissione.trasmissioniSingole[i];
                                trasmSing.ragione.eredita = "0";
                                appoTrasmSingole[i] = trasmSing;
                            }
                            trasmissione.trasmissioniSingole = appoTrasmSingole;
                        }
                        DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                        if (infoUtente.delegato != null)
                            trasmissione.delegato = ((DocsPAWA.DocsPaWR.InfoUtente)(infoUtente.delegato)).idPeople;

                        //Nuovo metodo saveExecuteTrasm
                        //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
                        //t_rs = TrasmManager.executeTrasm(this, trasmissione);
                        t_rs = TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
                    }
                    if (t_rs != null && t_rs.ErrorSendingEmails)
                        Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");

                    //Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=trasmissioni'; </script>");
                    Response.Write("<script language='javascript'>parent.IframeTabs.document.location='fascTrasmissioni.aspx';</script>");

                    return;
                }
                //FINE MODELLI TRASMISSIONE NUOVI                
            }

            try
            {
                DocsPaWR.Trasmissione trasmEff;
                if (this.flag_template.Value.Equals("S") && this.ddl_tmpl.SelectedIndex > 0)
                {
                    trasmEff = creaTrasmissione();
                    if (trasmEff == null)
                        Response.Write("<script language='javascript'>alert('Si è verificato un errore nella creazione della trasmissione da template');</script>");
                }
                else
                {
                    trasmEff = TrasmManager.getGestioneTrasmissione(this);
                    if (trasmEff != null && trasmEff.utente != null && string.IsNullOrEmpty(trasmEff.utente.idAmministrazione))
                        trasmEff.utente.idAmministrazione = UserManager.getInfoUtente().idAmministrazione;
                }
                if (trasmEff != null)
                {

                    if (estendiVisibilita.Value == "false")
                    {
                        TrasmissioneSingola[] appoTrasmSingole = new TrasmissioneSingola[trasmEff.trasmissioniSingole.Length];
                        for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
                        {
                            TrasmissioneSingola trasmSing = new TrasmissioneSingola();
                            trasmSing = trasmEff.trasmissioniSingole[i];
                            trasmSing.ragione.eredita = "0";
                            appoTrasmSingole[i] = trasmSing;
                        }
                        trasmEff.trasmissioniSingole = appoTrasmSingole;
                    }

                    DocsPaWR.Trasmissione t_rs = TrasmManager.executeTrasm(this, trasmEff);

                    if (t_rs != null && t_rs.ErrorSendingEmails)
                        Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");
                }

                Response.Write("<script language='javascript'>parent.IframeTabs.document.location='fascTrasmissioni.aspx';</script>");

                //resetto il template della trasmissione
                flag_template.Value = "N";
                this.ddl_tmpl.SelectedIndex = -1;
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                Session.Remove("doTrasm");
                ErrorManager.redirect(this, es);
            }
        }

        private void butt_Trasm_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Controlliamo se il fascicolo è chiuso e se non esistono ragioni di trasmissione in sola
            // lettura
            DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato(this);
            DocsPaWR.RagioneTrasmissione[] listaRagioni = TrasmManager.getListaRagioniFasc(this, fasc);
            if (listaRagioni.Length == 0 && !fasc.stato.Equals("A"))
            {
                RegisterStartupScript("alert", @"<script>alert('Fascicolo chiuso. Non sono configurate ragioni in sola lettura utilizzabili per poterlo trasmettere.\nContattare l\'amministratore .');</script>");
                return;
            }

            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                bool continueTrasm = true;
                //caso del fascicolo controllato
                if (this.fascicoloControllato(fasc))
                {
                    if (Session["Controllati"] == null)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "scelta_documenti", "openChooseTransDoc();", true);
                        continueTrasm = false;
                    }
                }

                if (continueTrasm)
                {
                    this.PerformTrasmFolder();
                }
            }
        }


        private void msg_TrasmettiFasc_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                //riapre la finestra attendere prego
                //ClientScript.RegisterStartupScript(this.GetType(), "refreshProgressMsgFascTrasmissioni", "showWorkingInProgress(''); ", true);
                
                this.PerformTrasmFolder();
            }

            //if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Cancel)
            //{
            //    //chiude la finestra attendere prego
            //    ClientScript.RegisterStartupScript(this.GetType(), "chiusuraProgressMsgFascTrasmissioni", "hideWorkingInProgress('fascTrasmissioni');", true);
            
            //}
        }

        private void btn_ModifTrasm_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                Session["OggettoDellaTrasm"] = "FASC";
                Response.Write("<script language='javascript'>top.principale.document.location='../trasmissione/gestioneTrasm.aspx?azione=Modifica';</script>");
            }
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
        private void InitializeComponent()
        {
            this.dS_EFF1 = new DocsPAWA.dataSet.DS_EFF();
            this.dS_RIC1 = new DocsPAWA.dataSet.DS_RIC();
            ((System.ComponentModel.ISupportInitialize)(this.dS_EFF1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dS_RIC1)).BeginInit();
            this.rbl_tipoTrasm.SelectedIndexChanged += new System.EventHandler(this.rbl_tipoTrasm_SelectedIndexChanged);
            this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChanged);
            this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
            this.Datagrid2.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.Datagrid2_PageIndexChanged);
            this.Datagrid2.SelectedIndexChanged += new System.EventHandler(this.DataGrid2_SelectedIndexChanged);
            this.ddl_tmpl.SelectedIndexChanged += new System.EventHandler(this.ddl_tmpl_SelectedIndexChanged);
            this.btn_NuovaTrasm.Click += new System.Web.UI.ImageClickEventHandler(this.btn_NuovaTrasm_Click);
            this.btn_ModifTrasm.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ModifTrasm_Click);
            this.butt_Trasm.Click += new System.Web.UI.ImageClickEventHandler(this.butt_Trasm_Click);
            this.msg_TrasmettiFasc.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_TrasmettiFasc_GetMessageBoxResponse);
            // Mev Ospedale maggiore polkiclinico
            this.btn_stampa.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampa_Click);
            // End Mev
            this.flag_template.ServerChange += new System.EventHandler(this.flag_template_ServerChange);
            // 
            // dS_EFF1
            // 
            this.dS_EFF1.DataSetName = "DS_EFF";
            this.dS_EFF1.Locale = new System.Globalization.CultureInfo("en-US");
            // 
            // dS_RIC1
            // 
            this.dS_RIC1.DataSetName = "DS_RIC";
            this.dS_RIC1.Locale = new System.Globalization.CultureInfo("en-US");
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);
            ((System.ComponentModel.ISupportInitialize)(this.dS_EFF1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dS_RIC1)).EndInit();

        }
        #endregion

        protected void rbl_tipoTrasm_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //			TrasmManager.removeDocTrasmSel(this);

            //			if(this.rbl_tipoTrasm.SelectedItem.Value.Equals("E"))
            //			{	
            //				this.DataGrid1.Visible=true;
            //				this.Datagrid2.Visible=false;				
            //				pnl_trasm_rapida.Visible=true;
            //				viewInfoFirstRow("E",0);
            //			}
            //			else
            //			{
            //				this.DataGrid1.Visible=false;
            //				this.Datagrid2.Visible=true;				
            //				pnl_trasm_rapida.Visible=false;
            //				viewInfoFirstRow("R",0);
            //			}									
            //			this.butt_Trasm.Enabled = false;
            //			this.btn_ModifTrasm.Enabled = false;
        }

        /// <summary>
        /// Impostazione in viewstate del tipo di 
        /// trasmissione richiesta
        /// </summary>
        /// <param name="tipoTrasmissione"></param>
        private void SetCurrentTipoTrasmissione(string tipoTrasmissione)
        {
            this.ViewState["CURRENT_TIPO_TRASMISSIONE"] = tipoTrasmissione;
        }

        /// <summary>
        /// Reperimento da viewstate del tipo
        /// di trasmissione selezionata
        /// </summary>
        /// <returns></returns>
        private string GetCurrentTipoTrasmissione()
        {
            if (this.ViewState["CURRENT_TIPO_TRASMISSIONE"] == null)
                return "";
            else
                return (string)this.ViewState["CURRENT_TIPO_TRASMISSIONE"];
        }


        protected void CaricaDataGrid(DocsPAWA.DocsPaWR.Trasmissione trasm, int index, string tipoRic)
        {
            if (tipoRic.Equals("E"))
            {
                this.dS_EFF1.element1.Addelement1Row(trasm.dataInvio, trasm.utente.descrizione + "<br>" + "(" + trasm.utente.userId + ")", trasm.ruolo.descrizione + "<br>" + "(" + trasm.ruolo.codice + ")", index);
            }
            else
            {
                DocsPaWR.TrasmissioneSingola trasmSing = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
                DocsPaWR.TrasmissioneUtente trasmUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                DocsPaWR.TrasmissioneSingola[] listaTrasmSing;

                listaTrasmSing = trasm.trasmissioniSingole;
                string ragione = "";
                string dataScad = "";
                if (listaTrasmSing.Length > 0)
                {
                    trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)listaTrasmSing[0];
                    ragione = trasmSing.ragione.descrizione;
                    dataScad = trasmSing.dataScadenza;
                }

                this.dS_RIC1.element1.Addelement1Row(trasm.dataInvio, trasm.utente.descrizione + "<br>" + "(" + trasm.utente.userId + ")", trasm.ruolo.descrizione + "<br>" + "(" + trasm.ruolo.codice+ ")", ragione, dataScad, index);
            }
        }

        private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //controllo che la trasmissione possa essere modificata:
            //cioè se non è stata ancora trasmessa - datainvio = null
            DocsPaWR.Trasmissione[] listaEff = TrasmManager.getDocTrasmQueryEff(this);

            string key = ((Label)this.DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[3].Controls[1]).Text;
            DocsPaWR.Trasmissione trasmEff = (DocsPAWA.DocsPaWR.Trasmissione)listaEff[Int32.Parse(key)];
            if (trasmEff.dataInvio.Equals(""))
            {
                this.btn_ModifTrasm.Enabled = true;
                this.butt_Trasm.Enabled = true;
            }
            else
            {
                this.btn_ModifTrasm.Enabled = false;
                this.butt_Trasm.Enabled = false;
            }

            TrasmManager.setDocTrasmSel(this, (DocsPAWA.DocsPaWR.Trasmissione)listaEff[Int32.Parse(key)]);
            TrasmManager.setGestioneTrasmissione(this, (DocsPAWA.DocsPaWR.Trasmissione)listaEff[Int32.Parse(key)]);

            Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='tabTrasmissioniEffFasc.aspx?chiudi=no';</script>");
            //rimuovo la possibile trasmissione da template
            this.ddl_tmpl.SelectedIndex = -1;
            this.flag_template.Value = "N";
        }

        private void DataGrid2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            DocsPaWR.Trasmissione[] listaRic = TrasmManager.getDocTrasmQueryRic(this);
            btn_ModifTrasm.Enabled = false;
            this.butt_Trasm.Enabled = false;
            int index = this.Datagrid2.SelectedIndex;
            if (listaRic != null)
            {
                if (index >= 0)
                {
                    string key = ((Label)this.Datagrid2.Items[this.Datagrid2.SelectedIndex].Cells[5].Controls[1]).Text;
                    TrasmManager.setDocTrasmSel(this, (DocsPAWA.DocsPaWR.Trasmissione)listaRic[Int32.Parse(key)]);
                    Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='tabTrasmissioniRicFasc.aspx';</script>");
                }
            }
            //rimuovo la possibile trasmissione da template
            this.ddl_tmpl.SelectedIndex = -1;
            this.flag_template.Value = "N";
        }

        private void DataGrid1_OnItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {

        }

        private void Datagrid1_PreRender(object sender, System.EventArgs e)
        {
            DataGrid dg = ((DataGrid)sender);
            for (int i = 0; i < this.DataGrid1.Items.Count; i++)
            {
                if (this.DataGrid1.Items[i].ItemIndex >= 0)
                {
                    switch (this.DataGrid1.Items[i].ItemType.ToString().Trim())
                    {
                        case "Item":
                            {
                                this.DataGrid1.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                                this.DataGrid1.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioN'");
                                break;
                            }
                        case "AlternatingItem":
                            {
                                this.DataGrid1.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                                this.DataGrid1.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioA'");
                                break;
                            }

                    }
                }
            }




        }

        private void Datagrid2_PreRender(object sender, System.EventArgs e)
        {

            for (int i = 0; i < this.Datagrid2.Items.Count; i++)
            {
                if (this.Datagrid2.Items[i].ItemIndex >= 0)
                {
                    switch (this.Datagrid2.Items[i].ItemType.ToString().Trim())
                    {
                        case "Item":
                            {
                                this.Datagrid2.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                                this.Datagrid2.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioN'");
                                break;
                            }
                        case "AlternatingItem":
                            {
                                this.Datagrid2.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                                this.Datagrid2.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioA'");
                                break;
                            }

                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void DataGrid1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.DataGrid1.CurrentPageIndex = e.NewPageIndex;
            this.FillDataTrasmissioni();

            //			this.DataGrid1.DataSource=(DataSet)Session["dS_EFF1"];
            //			this.DataGrid1.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void Datagrid2_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.Datagrid2.CurrentPageIndex = e.NewPageIndex;
            this.FillDataTrasmissioni();

            //			this.Datagrid2.CurrentPageIndex=e.NewPageIndex;
            //			this.Datagrid2.DataSource=(DataSet)Session["dS_RIC1"];
            //			this.Datagrid2.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_NuovaTrasm_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Controlliamo se il fascicolo è chiuso e se non esistono ragioni di trasmissione in sola
            // lettura
            DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato(this);
            DocsPaWR.RagioneTrasmissione[] listaRagioni = TrasmManager.getListaRagioniFasc(this, fasc);
            if (listaRagioni.Length == 0 && !fasc.stato.Equals("A"))
            {
                RegisterStartupScript("alert", @"<script>alert('Fascicolo chiuso. Non sono configurate ragioni in sola lettura utilizzabili per poterlo trasmettere.\nContattare l\'amministratore .');</script>");
                return;
            }

            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                Session.Remove("Modello");
                TrasmManager.removeGestioneTrasmissione(this);
                Session["OggettoDellaTrasm"] = "FASC";
                Response.Write("<script language='javascript'>top.principale.document.location='../trasmissione/gestioneTrasm.aspx?azione=Nuova';</script>");
            }
        }

        private void btn_NuovoDaTempl_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        private void caricaComboTemplate()
        {
            Session.Remove("doc_protocollo.tx_tmpl");
            DocsPaWR.TemplateTrasmissione[] listaTmp;
            if (Session["doc_protocollo.tx_tmpl"] != null)
            {
                listaTmp = (DocsPAWA.DocsPaWR.TemplateTrasmissione[])Session["doc_protocollo.tx_tmpl"];
            }
            else
            {
                listaTmp = TrasmManager.getListaTemplate(this, UserManager.getUtente(this), UserManager.getRuolo(this), "F");
                Session["doc_protocollo.tx_tmpl"] = listaTmp;

            }

            if (listaTmp != null && listaTmp.Length > 0)
            {
                if (ddl_tmpl.Items.Count == 0)
                    ddl_tmpl.Items.Add(" "); // valore vuoto;

                for (int i = 0; i < listaTmp.Length; i++)
                {
                    System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                    li.Text = listaTmp[i].descrizione;
                    li.Value = listaTmp[i].systemId;
                    ddl_tmpl.Items.Add(li);
                }



            }
        }

        private void ddl_tmpl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!ddl_tmpl.SelectedIndex.Equals(0))
            {
                this.flag_template.Value = "S";
                this.butt_Trasm.Enabled = true;
                //elimino la selezione della riga del datagrid
                this.DataGrid1.SelectedIndex = -1;
                this.Datagrid2.SelectedIndex = -1;
                TrasmManager.removeDocTrasmSel(this);
                string newUrl = "";
                if (this.rbl_tipoTrasm.SelectedValue == "R")
                {
                    newUrl = "tabTrasmissioniRicFasc.aspx";
                }
                else
                {
                    newUrl = "tabTrasmissioniEffFasc.aspx?chiudi=no";
                }

                Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='" + newUrl + "';</script>");
            }
            else
            {
                this.flag_template.Value = "N";
                this.butt_Trasm.Enabled = false;
            }

            //MODELLI TRASMISSIONE NUOVI
            if (ddl_tmpl.SelectedItem.Text == separatore)
            {
                this.flag_template.Value = "N";
                this.butt_Trasm.Enabled = false;
                Session.Remove("Modello");
                return;
            }
            DocsPaWR.ModelloTrasmissione modello = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
            modello = wws.getModelloByID(UserManager.getInfoUtente(this).idAmministrazione, ddl_tmpl.SelectedValue);
            if (modello != null && modello.SYSTEM_ID != 0)
            {
                Session.Add("Modello", modello);
                
                if (fasc.privato == "1")
                {
                    if (string.IsNullOrEmpty(abilitaModaleVis.Value))
                    {
                        if (wws.ereditaVisibilita("null", modello.SYSTEM_ID.ToString()))
                        {
                            abilitaModaleVis.Value = "true";
                            //ClientScript.RegisterStartupScript(this.GetType(), "openAvvisoVisibilita", "AvvisoVisibilita();", true);
                        }
                    }
                }

            }
            else
            {
                Session.Remove("Modello");
            }
            //FINE MODELLI TRASMISSIONE NUOVI

        }

        private DocsPAWA.DocsPaWR.Trasmissione creaTrasmissione()
        {
            //crea trasmissione da template
            DocsPaWR.TemplateTrasmissione[] listaTmp;
            DocsPaWR.Trasmissione trasmissione = null;
            try
            {
                DocsPaWR.TemplateTrasmissione template = null;
                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);

                oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();
                oggettoTrasm.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(fasc, this);

                listaTmp = (DocsPAWA.DocsPaWR.TemplateTrasmissione[])(Session["doc_protocollo.tx_tmpl"]);

                //Adesso con i modelli di trasmissione nuovi il calcolo del template vecchio selezionato
                //é necessario farlo cosi'. In ogni caso funziona sia se ci sono sia se non ci sono modelli nuovi.
                int numberOldTemplate = ddl_tmpl.Items.Count - listaTmp.Length;
                if (listaTmp != null && listaTmp.Length > 0)
                    template = (DocsPAWA.DocsPaWR.TemplateTrasmissione)listaTmp[ddl_tmpl.SelectedIndex - numberOldTemplate];
                if (template != null)
                    trasmissione = TrasmManager.addTrasmFascicoloDaTemplate(this, oggettoTrasm.infoFascicolo, template, infoUtente);

            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }

            return trasmissione;
        }

        private void btn_salva_trasm_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                if (!ddl_tmpl.SelectedIndex.Equals(0))
                {
                    //crea trasmissione da template
                    DocsPaWR.Trasmissione trasmissione;
                    trasmissione = creaTrasmissione();
                    if (trasmissione == null)
                        Response.Write("<script language='javascript'>alert('Si è verificato un errore nella creazione della trasmissione da template');</script>");
                    else
                        Response.Write("<script language='javascript'>parent.IframeTabs.document.location='fascTrasmissioni.aspx';</script>");
                }
            }
        }

        private void btn_stampa_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                try
                {
                    DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();
                    oggettoTrasm.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(fasc, this);

                    DocsPaWR.FileDocumento fileRep = TrasmManager.getReportTrasm(this, oggettoTrasm);
                    if (fileRep == null)
                        return;
                    FileManager.setSelectedFileReport(this, fileRep, "../popup");
                }
                catch (Exception ex)
                {
                    ErrorManager.redirectToErrorPage(this, ex);
                }
            }
        }

        private void flag_template_ServerChange(object sender, System.EventArgs e)
        {

        }

        #region ModelliTrasmissioni

        private DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr, DocsPAWA.DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza)
        {
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                {
                    DocsPaWR.TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;
            //Imposto la data di scadenza
            if (scadenza > 0)
            {
                //string dataScadenza = "";
                System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                //dataScadenza = data.Day + "/" + data.Month + "/" + data.Year;
                trasmissioneSingola.dataScadenza = DocsPAWA.Utils.formatDataDocsPa(data);
            }

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPAWA.DocsPaWR.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);
                
                //Andrea
                if (listaUtenti.Length == 0)
                {
                    trasmissioneSingola = null;
                    throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                                                    + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + ".");
                }
                //End Andrea
                else
                {
                    //ciclo per utenti se dest è gruppo o ruolo
                    for (int i = 0; i < listaUtenti.Length; i++)
                    {
                        DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                        trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)listaUtenti[i];
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    }
                }
            }

            if (corr is DocsPAWA.DocsPaWR.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)corr;

                //Andrea
                if (trasmissioneUtente.utente == null)
                {
                    throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + " è inesistente.");
                }
                //End Andrea
                else
                    trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
            }

            if (corr is DocsPAWA.DocsPaWR.UnitaOrganizzativa)
            {
                DocsPaWR.UnitaOrganizzativa theUo = (DocsPAWA.DocsPaWR.UnitaOrganizzativa)corr;
                DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new AddressbookQueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = UserManager.getRuolo();
                qca.queryCorrispondente = new AddressbookQueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                //DocsPaWR.Ruolo[] ruoli = UserManager.getListaRuoliInUO (page, (DocsPAWA.DocsPaWR.UnitaOrganizzativa) corr, UserManager.getInfoUtente(page));
                DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(this, qca, theUo);
                
                //Andrea
                if (ruoli==null || ruoli.Length == 0)
                {
                    throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per l Ufficio: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                }
                //End Andrea
                else
                {
                    foreach (DocsPAWA.DocsPaWR.Ruolo r in ruoli)
                        trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza);
                }
                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);

            return trasmissione;
        }

        private DocsPAWA.DocsPaWR.Corrispondente[] queryUtenti(DocsPAWA.DocsPaWR.Corrispondente corr)
        {

            //costruzione oggetto queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;

            qco.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
            qco.fineValidita = true;

            //corrispondenti interni
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;

            return UserManager.getListaCorrispondenti(this, qco);
        }

        private void caricaModelliTrasm()
        {
            //Quando in sessione non c'è il registro selezionato, viene preso il primo registro utile
            //associato al ruolo che sta facendo l'operazione
            //DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getre(fasc.idClassificazione);  
            string RegTit = wws.getRegTitolarioById(fasc.idClassificazione);
            bool AllReg = false;
            if (RegTit == "") {
               AllReg = true;
            }
            string idRegistro = "";
            Registro[] registri = new Registro[1];
            if (UserManager.getRegistroSelezionato(this) != null)
            {
                idRegistro = UserManager.getRegistroSelezionato(this).systemId;
                registri[0] = UserManager.getRegistroSelezionato(this);
            }
            else
            {
                //idRegistro = ((DocsPAWA.DocsPaWR.Registro) (UserManager.GetRegistriByRuolo(this,UserManager.getRuolo().systemId))[0]).systemId;
                //In questo caso il fascicolo è visibile su tutti i registri
                //allora carico tutti i modelli di trasmissione utilizzabili dall'utente
                DocsPaWR.Utente utente = UserManager.getUtente(this);
                ArrayList modelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.getInfoUtente(this),null);
                if (ddl_tmpl.Items.Count == 0)
                {
                    System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                    li.Text = "";
                    ddl_tmpl.Items.Add(li);
                }
                for (int i = 0; i < modelliTrasmissione.Count; i++)
                {
                    DocsPaWR.ModelloTrasmissione mod = (DocsPAWA.DocsPaWR.ModelloTrasmissione)modelliTrasmissione[i];
                    System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                    if (mod.CHA_TIPO_OGGETTO == "F")
                    {
                        if (mod.CEDE_DIRITTI != null && mod.CEDE_DIRITTI.Equals("1") && !userAutorizedEditingACL)
                        {
                            continue;
                        }
                        else
                        {
                            li.Value = mod.SYSTEM_ID.ToString();
                            li.Text = mod.NOME;
                            ddl_tmpl.Items.Add(li);
                        }
                    }
                }
                if (modelliTrasmissione.Count > 0)
                {
                    ddl_tmpl.Items.Add(separatore);
                }
                return;
            }

            string idAmm = UserManager.getInfoUtente(this).idAmministrazione;
            string idPeople = UserManager.getInfoUtente(this).idPeople;
            string idCorrGlobali = UserManager.getInfoUtente(this).idCorrGlobali;
            string idRuoloUtente = UserManager.getInfoUtente(this).idGruppo;
            string idTipoDoc = "";
            string idDiagramma = "";
            string idStato = "";

            //ArrayList idModelli = new ArrayList(wws.getModelliPerTrasm(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, "F", fasc.systemID, idRuoloUtente, AllReg, fasc.accessRights));
            ArrayList idModelli = new ArrayList(wws.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, "F", fasc.systemID, idRuoloUtente, AllReg, fasc.accessRights));

            if (ddl_tmpl.Items.Count == 0)
            {
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                li.Text = "";
                ddl_tmpl.Items.Add(li);
            }
            for (int i = 0; i < idModelli.Count; i++)
            {
                DocsPaWR.ModelloTrasmissione mod = (DocsPAWA.DocsPaWR.ModelloTrasmissione)idModelli[i];
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();

                if (mod.CEDE_DIRITTI != null && mod.CEDE_DIRITTI.Equals("1") && !userAutorizedEditingACL)
                {
                    continue;
                }
                else
                {
                    li.Value = mod.SYSTEM_ID.ToString();
                    li.Text = mod.NOME;
                    ddl_tmpl.Items.Add(li);
                }
            }
            if (idModelli.Count > 0)
            {
                ddl_tmpl.Items.Add(separatore);
            }
        }

        #endregion

        protected void DataGrid1_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        protected void Datagrid2_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        #region Gestione controllo acl fascicolo

        /// <summary>
        /// Inizializzazione controllo verifica acl
        /// </summary>
        protected virtual void InitializeControlAclFascicolo()
        {
            AclFascicolo ctl = this.GetControlAclFascicolo();
            ctl.IdFascicolo = FascicoliManager.getFascicoloSelezionato().systemID;
            ctl.OnAclRevocata += new EventHandler(this.OnAclFascicoloRevocata);
        }

        /// <summary>
        /// Reperimento controllo acldocumento
        /// </summary>
        /// <returns></returns>
        protected AclFascicolo GetControlAclFascicolo()
        {
            return (AclFascicolo)this.FindControl("aclFascicolo");
        }

        /// <summary>
        /// Listener evento OnAclDocumentoRevocata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAclFascicoloRevocata(object sender, EventArgs e)
        {
            // Redirect alla homepage di docspa
            SiteNavigation.CallContextStack.Clear();
            SiteNavigation.NavigationContext.RefreshNavigation();
            string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
            Response.Write(script);
        }

        #endregion

        #region Gestione notifica utenti

        private DocsPaWR.Trasmissione impostaNotificheUtentiDaModello(DocsPaWR.Trasmissione objTrasm)
        {
            if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Length > 0)
            {
                for (int cts = 0; cts < objTrasm.trasmissioniSingole.Length; cts++)
                {
                    if (objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length > 0)
                    {
                        for (int ctu = 0; ctu < objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length; ctu++)
                        {
                            objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].daNotificare = this.daNotificareSuModello(objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].utente.idPeople, objTrasm.trasmissioniSingole[cts].corrispondenteInterno.systemId);
                        }
                    }
                }
            }

            return objTrasm;
        }

        private bool daNotificareSuModello(string currentIDPeople, string currentIDCorrGlobRuolo)
        {
            bool retValue = true;

            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                    if (mittDest.ID_CORR_GLOBALI.Equals(Convert.ToInt32(currentIDCorrGlobRuolo)))
                    {
                        if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                        {
                            for (int cut = 0; cut < mittDest.UTENTI_NOTIFICA.Length; cut++)
                            {
                                if (mittDest.UTENTI_NOTIFICA[cut].ID_PEOPLE.Equals(currentIDPeople))
                                {
                                    if (mittDest.UTENTI_NOTIFICA[cut].FLAG_NOTIFICA.Equals("1"))
                                        retValue = true;
                                    else
                                        retValue = false;

                                    return retValue;
                                }
                            }
                        }
                    }
                }
            }
            return retValue;
        }

        private bool notificheUtImpostate(DocsPaWR.ModelloTrasmissione modello)
        {
            bool retValue = true;
            bool flag = false;

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {
                foreach (DocsPaWR.MittDest mittDest in ragDest.DESTINATARI)
                {
                    if (!mittDest.CHA_TIPO_URP.Equals("U"))
                    {
                        // ritorna FALSE se anche un solo destinatario del modello non ha UTENTI_NOTIFICA
                        if (mittDest.UTENTI_NOTIFICA == null)
                            return false;

                        if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                        {
                            flag = false;

                            foreach (DocsPaWR.UtentiConNotificaTrasm utNot in mittDest.UTENTI_NOTIFICA)
                            {
                                if (utNot.FLAG_NOTIFICA.Equals("1"))
                                    flag = true;
                            }

                            // ritorna FALSE se anche un solo destinatario ha tutti gli utenti con le notifiche non impostate
                            if (!flag)
                                return false;
                        }
                    }
                }
            }

            return retValue;
        }
        #endregion

        /// <summary>
        /// GESTIONE CESSIONE DIRITTI:
        /// verifica se l'utente è abilitato alla funzione ABILITA_CEDI_DIRITTI_DOC
        /// </summary>
        private void checkIsAutorizedEditingACL()
        {
            userAutorizedEditingACL = UserManager.ruoloIsAutorized(this, "ABILITA_CEDI_DIRITTI_DOC");
        }

        public bool fascicoloControllato(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
        {
            bool result = false;

            if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.controllato) && fascicolo.controllato.Equals("1"))
            {
                result = true;
            }

            return result;
        }
    }
}
