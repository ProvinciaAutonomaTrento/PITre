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
using System.Configuration;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Management
{
    /// <summary>   
    /// Classe per la gestione delle notifiche trasmissione rispetto agli utenti del ruolo destinatario nei modelli di trasm.ne
    /// </summary>
    public partial class GestioneModelliTrasm_Notifiche : System.Web.UI.Page
    {
        protected DataSet dataSet;
        bool utenteAbilitatoACL = false;


        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            //this.btn_ok.Text = Utils.Languages.GetLabelFromCode("NoticheModelliBtnOk", language);
            //this.btn_annulla.Text = Utils.Languages.GetLabelFromCode("NoticheModelliBtnAnnulla", language); 

        }

        public string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }


        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.impostaAbilitazioneACL();
            if (!IsPostBack)
            {
                
                //this.impostaTitolo();
                this.impostaIntestazione();
                this.InitializeLanguage();
                this.generaLista();
                this.impostaMode();
            }
            //EMILIO
            if (dg_Notifiche.Rows.Count != 0)
            {
                btn_ok.Enabled = true;
            }
            else
                btn_ok.Enabled = false;


        }

        protected bool getCheckBox(object abilita)
        {
            string abil = abilita.ToString();
            if (abil == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }     

        /// <summary>
        /// GESTIONE CESSIONE DIRITTI:
        /// verifica se l'utente è abilitato alla funzione ABILITA_CEDI_DIRITTI_DOC / ABILITA_CEDI_DIRITTI_FASC
        /// 
        /// NOTA:
        /// Poichè la stessa popup viene utilizzata anche in amministrazione, in questo caso l'utente amm.re è sempre abilitato alla cessione
        /// </summary>
        private void impostaAbilitazioneACL()
        {
            // gestione chiamante: Amm.ne o docspa 
            string appKey = string.Empty;

            if (Session["AppWA"] != null)
            {
                appKey = Session["AppWA"].ToString();
                if (appKey != null && appKey.ToString().Equals("ADMIN"))
                {
                    this.utenteAbilitatoACL = this.isCessioneAbilitata();
                    return;
                }
            }

            string funzione = string.Empty;
            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            if (modello.CHA_TIPO_OGGETTO.Equals("D"))
                funzione = "ABILITA_CEDI_DIRITTI_DOC";
            if (modello.CHA_TIPO_OGGETTO.Equals("F"))
                funzione = "ABILITA_CEDI_DIRITTI_FASC";

            this.utenteAbilitatoACL = UserManager.IsAuthorizedFunctions(funzione);
        }

        /// <summary>
        /// Gestione del titolo della pagina web
        /// </summary>
        //private void impostaTitolo()
        //{
        //    if (this.utenteAbilitatoACL)
        //        this.titolo.Text += " e Cessione diritti";
        //}

        /// <summary>
        /// Genera l'intestazione della pagina
        /// </summary>
        private void impostaIntestazione()
        {
            string intestazioneSuperiore = string.Empty;
            string intestazione = string.Empty;

            intestazioneSuperiore = "Gestione notifiche trasmissione";
            intestazione = "<br />Selezionare gli utenti ai quali sarà notificata la trasmissione generata da questo modello<br />";

            if (this.utenteAbilitatoACL)
            {
                intestazioneSuperiore += " e Cessione diritti";
                intestazione += "e l'utente al quale cedere i diritti di proprietà o visibilità sull'oggetto trasmesso.<br />";
            }

            //this.titolo.Text = GetLabel("ModelliNotificheTitolo").Replace("@@",intestazioneSuperiore);
            this.lbl_avviso.Text = GetLabel("ModelliNotificeLblAvviso").Replace("@@", intestazione);
        }

        /// <summary>
        /// Reperisce la system_id della groups rispetto alla system_id della corr_globali 
        /// </summary>
        /// <param name="idCorrGlobRuolo">system_id della tabella DPA_CORR_GLOBALI</param>
        /// <returns>system_id della tabella GROUPS</returns>
        private string impostaIDGroup(string idCorrGlobRuolo)
        {
            string retValue = string.Empty;

            NttDataWA.DocsPaWR.Ruolo ruolo = new NttDataWA.DocsPaWR.Ruolo();
            //DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();

            ruolo = RoleManager.getRuoloById(idCorrGlobRuolo);

            if (ruolo != null)
                retValue = ruolo.idGruppo;

            return retValue;
        }

        /// <summary>
        /// Genera la lista con ruoli e utenti da visualizzare a video 
        /// </summary>
        private void generaLista()
        {
            try
            {
                DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

                // prima elimina gli oggetti UTENTI_NOTIFICA...
                foreach (NttDataWA.DocsPaWR.RagioneDest ragioneDest in modello.RAGIONI_DESTINATARI)
                    foreach (NttDataWA.DocsPaWR.MittDest mittDest in ragioneDest.DESTINATARI)
                        mittDest.UTENTI_NOTIFICA = null;

                // quindi reperisce i dati sul db
                //DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();
                //modello = ws.UtentiConNotificaTrasm(modello, null, null, "GET");
                modello = ModelliTrasmManager.UtentiConNotificaTrasm(modello, null, null, "GET", this.Page);

                // rimette in sessione l'oggetto MODELLO aggiornato con i dati presi da db
                Session.Remove("Modello");
                Session.Add("Modello", modello);

                // inizializza il dataset che popola il datagrid
                IniDataSet();

                // genera le righe del datagrid
                foreach (NttDataWA.DocsPaWR.RagioneDest ragioneDest in modello.RAGIONI_DESTINATARI)
                    foreach (NttDataWA.DocsPaWR.MittDest mittDest in ragioneDest.DESTINATARI)
                    {
                        if (mittDest.CHA_TIPO_URP.Equals("R"))
                        {
                            if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 1)
                            {
                                this.loadDataSet(modello, mittDest, "true");
                            }
                            else
                            {
                                if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length == 1)
                                {
                                    mittDest.UTENTI_NOTIFICA[0].FLAG_NOTIFICA = "1";
                                    this.loadDataSet(modello, mittDest, "false");
                                }
                            }
                        }
                        else
                        {
                            if (mittDest.CHA_TIPO_URP == "P" && mittDest.UTENTI_NOTIFICA.Length == 1)
                            {
                                if (mittDest.UTENTI_NOTIFICA != null)
                                {
                                    mittDest.UTENTI_NOTIFICA[0].FLAG_NOTIFICA = "1";
                                    this.loadDataSet(modello, mittDest, "false");
                                }
                            }
                        }
                    }
                Session.Add("Modello", modello);

                // bind del datagrid
                this.dg_Notifiche.DataSource = dataSet;
                this.dg_Notifiche.DataBind();
            }
            catch
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Genera le righe del datagrid
        /// </summary>
        /// <param name="mittDest"></param>
        private void loadDataSet(DocsPaWR.ModelloTrasmissione modello, NttDataWA.DocsPaWR.MittDest mittDest, string disabled)
        {
            DataRow row;
            string idGroup = "";
            if (mittDest.CHA_TIPO_MITT_DEST == "D")
            {
                if (mittDest.CHA_TIPO_URP.Equals("R"))
                {
                    row = dataSet.Tables[0].NewRow();

                    // riga del ruolo
                    row["descrizione"] = "<b>" + mittDest.DESCRIZIONE.ToUpper() + "</b>";
                    row["attivo"] = "false";
                    row["idPeople"] = "";
                    row["idRuolo"] = mittDest.ID_CORR_GLOBALI;
                    row["tipo"] = mittDest.CHA_TIPO_URP;

                    idGroup = impostaIDGroup(mittDest.ID_CORR_GLOBALI.ToString());
                    row["idGroup"] = idGroup;

                    row["attivoC"] = "false";
                    row["enabledC"] = "false";

                    dataSet.Tables["NOTIFICHE"].Rows.Add(row);
                }

                //righe degli utenti
                foreach (NttDataWA.DocsPaWR.UtentiConNotificaTrasm ut in mittDest.UTENTI_NOTIFICA)
                {
                    row = dataSet.Tables[0].NewRow();
                    row["descrizione"] = ut.CODICE_UTENTE + " - " + ut.NOME_COGNOME_UTENTE.ToUpper();
                    if (ut.FLAG_NOTIFICA.Equals("1"))
                        row["attivo"] = "true";
                    else
                        row["attivo"] = "false";
                    row["idPeople"] = ut.ID_PEOPLE;
                    row["idRuolo"] = mittDest.ID_CORR_GLOBALI;
                    row["tipo"] = "P";
                    row["idGroup"] = idGroup;
                    row["disabled"] = disabled;

                    // cessione diritti
                    if (ut.ID_PEOPLE.Equals(modello.ID_PEOPLE_NEW_OWNER) && idGroup.Equals(modello.ID_GROUP_NEW_OWNER))
                        row["attivoC"] = "true";
                    else
                        row["attivoC"] = "false";

                    row["enabledC"] = this.isRagioneConCessione(mittDest.ID_RAGIONE);

                    dataSet.Tables["NOTIFICHE"].Rows.Add(row);
                }
            }
        }

        /// <summary>
        /// Inizializza il dataset
        /// </summary>
        private void IniDataSet()
        {
            dataSet = new DataSet();

            dataSet.Tables.Add("NOTIFICHE");

            DataColumn dc = new DataColumn("descrizione");
            dataSet.Tables["NOTIFICHE"].Columns.Add(dc);

            dc = new DataColumn("attivo");
            dataSet.Tables["NOTIFICHE"].Columns.Add(dc);

            dc = new DataColumn("idPeople");
            dataSet.Tables["NOTIFICHE"].Columns.Add(dc);

            dc = new DataColumn("idRuolo");
            dataSet.Tables["NOTIFICHE"].Columns.Add(dc);

            dc = new DataColumn("tipo");
            dataSet.Tables["NOTIFICHE"].Columns.Add(dc);

            dc = new DataColumn("idGroup");
            dataSet.Tables["NOTIFICHE"].Columns.Add(dc);

            dc = new DataColumn("attivoC");
            dataSet.Tables["NOTIFICHE"].Columns.Add(dc);

            dc = new DataColumn("enabledC");
            dataSet.Tables["NOTIFICHE"].Columns.Add(dc);

            dc = new DataColumn("disabled");
            dataSet.Tables["NOTIFICHE"].Columns.Add(dc);

        }

        /// <summary>
        /// Effettua la chiusura della Popup
        /// </summary>
        //private void chiudePopup(string mode)
        //{
        //    string jscript = string.Empty;

        //    switch (mode)
        //    {
        //        case "INSERT":
        //            jscript = "<script>window.returnValue = 'I'; self.close();</script>";
        //            break;
        //        case "UPDATE":
        //            jscript = "<script>window.returnValue = 'U'; self.close();</script>";
        //            break;
        //        case "NONE":
        //            jscript = "<script>self.close();</script>";
        //            break;
        //    }

        //    if (!ClientScript.IsStartupScriptRegistered("chiude"))
        //        ClientScript.RegisterStartupScript(this.GetType(), "chiude", jscript);
        //}

        /// <summary>
        /// Tasto Chiudi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_annulla_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            ScriptManager.RegisterClientScriptBlock(this.UpdatePanelButtons, this.UpdatePanelButtons.GetType(), "closeAJM", "parent.closeAjaxModal('GestioneNotifiche','');", true);
            //this.chiudePopup("NONE");
        }

        /// <summary>
        /// Tasto Salva e Chiudi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_ok_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            switch (this.hd_mode.Value)
            {
                case "INSERT":
                    this.PerformInsert();
                    break;

                case "UPDATE":
                    this.PerformUpdate();
                    break;
            }
        }

        private void PerformInsert()
        {
            string statoChk;

            //try
            //{
                DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
                // verifica se è selezionato almeno un utente per ruolo e la corretta cessione dei diritti
                if (controllaInserimentiUtente())
                {
                    foreach (NttDataWA.DocsPaWR.RagioneDest ragioneDest in modello.RAGIONI_DESTINATARI)
                    {
                        foreach (NttDataWA.DocsPaWR.MittDest mittDest in ragioneDest.DESTINATARI)
                        {
                            if (!mittDest.CHA_TIPO_URP.Equals("U"))
                            {
                                if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                                {
                                    foreach (NttDataWA.DocsPaWR.UtentiConNotificaTrasm utNot in mittDest.UTENTI_NOTIFICA)
                                    {
                                        statoChk = this.utenteSelezionato(utNot.ID_PEOPLE, Convert.ToString(mittDest.ID_CORR_GLOBALI));
                                        utNot.FLAG_NOTIFICA = statoChk;
                                        //if (utNot.FLAG_NOTIFICA != "1")
                                        //{
                                        //    statoChk = this.utenteSelezionato(utNot.ID_PEOPLE, Convert.ToString(mittDest.ID_CORR_GLOBALI));
                                        //    utNot.FLAG_NOTIFICA = statoChk;
                                        //}
                                    }
                                }
                            }
                        }
                    }

                    // cessione diritti
                    if (this.utenteAbilitatoACL)
                        modello = this.impostaCessioneSuModello(modello);

                    Session.Remove("impostaNotifiche");

                    // chiude la popup
                    //this.chiudePopup(this.hd_mode.Value);
                    switch (this.hd_mode.Value)
                    {
                        case "INSERT":
                            Session["retValueNotifiche"] = "I";
                            break;
                        case "UPDATE":
                            Session["retValueNotifiche"] = "U";
                            break;
                        case "NONE":
                            break;
                    }

                    this.PerformSaveModel();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "closeAJM", "parent.closeAjaxModal('GestioneNotifiche','up');", true);
                    //ScriptManager.RegisterClientScriptBlock(this.UpdatePanelButtons, this.UpdatePanelButtons.GetType(), "closeAJM", "parent.closeAjaxModal('GestioneNotifiche','up');", true);
                }
            //}
            //catch
            //{
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectModelli(){$(location).attr('href','" + this.ResolveUrl("~/Management/GestioneModelliTrasm.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErroreSalvataggioNotifiche', 'warning', '','',null,null,'RedirectModelli()')} else {parent.parent.ajaxDialogModal('ErroreSalvataggioNotifiche', 'warning', '','',null,null,'RedirectModelli()');}", true);
            //    //string jscript = "<script>alert('Errore durante il salvataggio dei dati!'); window.close();</script>";
            //    //if (!ClientScript.IsStartupScriptRegistered("erroreEchiude"))
            //    //    ClientScript.RegisterStartupScript(this.GetType(), "erroreEchiude", jscript);
            //}
        }

        private void PerformSaveModel()
        {

            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

            TransmissionModelsManager.SaveTemplate(modello, infoUtente);
         
            Session.Remove("modelloToSave");
        }

        private void PerformUpdate()
        {
            bool modificaEffettuata = false;
            string statoChk;
            ArrayList utentiDaInserire = new ArrayList();
            ArrayList utentiDaCancellare = new ArrayList();

            //try
            //{
                DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
                //DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();

                // verifica se è selezionato almeno un utente per ruolo e la corretta cessione dei diritti
                if (controllaInserimentiUtente())
                {
                    foreach (NttDataWA.DocsPaWR.RagioneDest ragioneDest in modello.RAGIONI_DESTINATARI)
                    {
                        foreach (NttDataWA.DocsPaWR.MittDest mittDest in ragioneDest.DESTINATARI)
                        {
                            if (!mittDest.CHA_TIPO_URP.Equals("U"))
                            {
                                if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                                {
                                    foreach (NttDataWA.DocsPaWR.UtentiConNotificaTrasm utNot in mittDest.UTENTI_NOTIFICA)
                                    {
                                        // verifica se le selezioni sono uguali ai dati reperiti sul db
                                        statoChk = this.utenteSelezionato(utNot.ID_PEOPLE, Convert.ToString(mittDest.ID_CORR_GLOBALI));
                                        if (!statoChk.Equals(utNot.FLAG_NOTIFICA))
                                        {
                                            //if (utNot.FLAG_NOTIFICA != "1")
                                            //{
                                            utNot.FLAG_NOTIFICA = statoChk;
                                            modificaEffettuata = true;      // è stata effettuata almeno una modifica! si procederà al salvataggio dei dati...
                                            //}
                                            if (statoChk.Equals("0"))
                                                utentiDaCancellare.Add(utNot);
                                            else
                                                utentiDaInserire.Add(utNot);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (modificaEffettuata)
                    {
                        NttDataWA.DocsPaWR.UtentiConNotificaTrasm[] utentiDaInserire_1 = new NttDataWA.DocsPaWR.UtentiConNotificaTrasm[utentiDaInserire.Count];
                        utentiDaInserire.CopyTo(utentiDaInserire_1);
                        NttDataWA.DocsPaWR.UtentiConNotificaTrasm[] utentiDaCancellare_1 = new NttDataWA.DocsPaWR.UtentiConNotificaTrasm[utentiDaCancellare.Count];
                        utentiDaCancellare.CopyTo(utentiDaCancellare_1);
                        // effettua il salvataggio dei dati                        
                        //modello = ws.UtentiConNotificaTrasm(modello, utentiDaInserire_1, utentiDaCancellare_1, "SET");
                        modello = ModelliTrasmManager.UtentiConNotificaTrasm(modello, utentiDaInserire_1, utentiDaCancellare_1, "SET", this.Page);
                        if (modello == null)
                        {
                            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectModelli(){$(location).attr('href','" + this.ResolveUrl("~/Management/GestioneModelliTrasm.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErroreSalvataggioNotifiche', 'warning', '','',null,null,'RedirectModelli()')} else {parent.parent.ajaxDialogModal('ErroreSalvataggioNotifiche', 'warning', '','',null,null,'RedirectModelli()');}", true);

                            //string jscript = "<script>alert('Errore durante il salvataggio dei dati!'); window.close();</script>";
                            //if (!ClientScript.IsStartupScriptRegistered("erroreEchiude"))
                            //    ClientScript.RegisterStartupScript(this.GetType(), "erroreEchiude", jscript);
                            return;
                        }
                    }

                    // cessione diritti
                    if (this.utenteAbilitatoACL)
                    {
                        modificaEffettuata = this.cessioneModificata(modello);
                        if (modificaEffettuata)
                        {
                            modello = this.impostaCessioneSuModello(modello);

                            ModelliTrasmManager.SalvaCessioneDirittiSuModelliTrasm(modello, this.Page);

                            //ws.SalvaCessioneDirittiSuModelliTrasm(modello);
                        }
                    }

                    Session.Remove("impostaNotifiche");

                    // chiude la popup
                    //this.chiudePopup(this.hd_mode.Value);
                    switch (this.hd_mode.Value)
                    {
                        case "INSERT":
                            Session["retValueNotifiche"] = "I";
                            break;
                        case "UPDATE":
                            Session["retValueNotifiche"] = "U";
                            break;
                        case "NONE":
                            break;
                    }
                    ScriptManager.RegisterClientScriptBlock(this.UpdatePanelButtons, this.UpdatePanelButtons.GetType(), "closeAJM", "parent.closeAjaxModal('GestioneNotifiche','up');", true);
                }
            //}
            //catch
            //{
            // //   ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectModelli(){$(location).attr('href','" + this.ResolveUrl("~/Management/GestioneModelliTrasm.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErroreSalvataggioNotifiche', 'warning', '','',null,null,'RedirectModelli()')} else {parent.parent.ajaxDialogModal('ErroreSalvataggioNotifiche', 'warning', '','',null,null,'RedirectModelli()');}", true);

            //    //string jscript = "<script>alert('Errore durante il salvataggio dei dati!'); window.close();</script>";
            //    //if (!ClientScript.IsStartupScriptRegistered("erroreEchiude"))
            //    //    ClientScript.RegisterStartupScript(this.GetType(), "erroreEchiude", jscript);
            //}
        }

        /// <summary>
        /// Imposta l'oggetto modello con i dati inseriti dall'utente
        /// </summary>
        /// <param name="modello"></param>
        /// <returns></returns>
        private DocsPaWR.ModelloTrasmissione impostaCessioneSuModello(DocsPaWR.ModelloTrasmissione modello)
        {
            bool trovataSelezione = false;
            string idPeople = string.Empty;
            string idGroup = string.Empty;
            DocsPaWR.ModelloTrasmissione modelloModificato = modello;

            CheckBox spunta;
            for (int i = 0; i < this.dg_Notifiche.Rows.Count; i++)
            {
                spunta = (CheckBox)this.dg_Notifiche.Rows[i].Cells[6].FindControl("Chk_C");
                if (spunta.Checked)
                {
                    string idPeopleSel = dg_Notifiche.DataKeys[i].Values["idPeople"].ToString();
                    string idGroupSel = dg_Notifiche.DataKeys[i].Values["idGroup"].ToString();
                    idPeople = idPeopleSel.Replace("&nbsp;", "");
                    idGroup = idGroupSel.Replace("&nbsp;", "");
                    trovataSelezione = true;
                    break;
                }
            }

            if (trovataSelezione)
            {
                modelloModificato.CEDE_DIRITTI = "1";

                //DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();
                DocsPaWR.OrgRagioneTrasmissione ragione = null;
                DocsPaWR.MittDest mittDest = null;

                foreach (DocsPaWR.RagioneDest ragDest in modelloModificato.RAGIONI_DESTINATARI)
                {
                    mittDest = ragDest.DESTINATARI[0];

                    //ragione = ws.AmmGetRagioneTrasmissione(Convert.ToString(mittDest.ID_RAGIONE));
                    ragione = TrasmManager.GetRagioneTrasmissione(Convert.ToString(mittDest.ID_RAGIONE));
                    if (ragione != null)
                    {
                        if (ragione.MantieniLettura == true)
                        {
                            modelloModificato.MANTIENI_LETTURA = "1";
                            break;
                        }
                        else
                        {
                            modelloModificato.MANTIENI_LETTURA = "0";
                        }

                    }
                }


            }
            else
                modelloModificato.CEDE_DIRITTI = "0";

            modelloModificato.ID_PEOPLE_NEW_OWNER = idPeople;
            modelloModificato.ID_GROUP_NEW_OWNER = idGroup;

            Session.Remove("Modello");
            Session.Add("Modello", modelloModificato);

            return modelloModificato;
        }

        /// <summary>
        /// Verifica se la cessione dei diritti è stata modificata dall'utente
        /// </summary>
        /// <param name="modello"></param>
        /// <returns></returns>
        private bool cessioneModificata(DocsPaWR.ModelloTrasmissione modello)
        {
            bool retValue = false;
            string idPeople = string.Empty;
            string idGroup = string.Empty;

            CheckBox spunta;
            for (int i = 0; i < this.dg_Notifiche.Rows.Count; i++)
            {
                spunta = (CheckBox)this.dg_Notifiche.Rows[i].Cells[6].FindControl("Chk_C");
                if (spunta.Checked)
                {
                    string idPeopleSel = dg_Notifiche.DataKeys[i].Values["idPeople"].ToString();
                    string idGroupSel = dg_Notifiche.DataKeys[i].Values["idGroup"].ToString();
                    // verifica se i dati sono gli stessi di quelli reperiti sul db
                    if (modello.ID_PEOPLE_NEW_OWNER.Equals(idPeopleSel) &&
                        modello.ID_GROUP_NEW_OWNER.Equals(idGroupSel))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Stato della checkbox per l'utente passato come parametro
        /// </summary>
        /// <param name="id">id Utente</param>
        /// <returns></returns>
        private string utenteSelezionato(string id, string idRuolo)
        {
            CheckBox spunta;
            for (int i = 0; i < this.dg_Notifiche.Rows.Count; i++)
            {
                string idPeople = dg_Notifiche.DataKeys[i].Values["idPeople"].ToString();
                string ruolo = dg_Notifiche.DataKeys[i].Values["idRuolo"].ToString();

                if (idPeople.Equals(id) && ruolo.Equals(idRuolo))
                {
                    spunta = (CheckBox)dg_Notifiche.Rows[i].Cells[1].FindControl("Chk");
                    if (spunta.Checked)
                        return "1";
                }
            }
            return "0";
        }

        /// <summary>
        /// Verifica se è selezionato almeno un utente per ruolo
        /// </summary>
        /// <returns></returns>
        private bool controllaInserimentiUtente()
        {
            string msg = string.Empty;
            string IDRuoloCorrente;

            for (int i = 0; i < this.dg_Notifiche.Rows.Count; i++)
            {
                string tipo = dg_Notifiche.DataKeys[i].Values["tipo"].ToString();
                if (tipo.ToUpper().Equals("R"))
                {
                    string ruolo = dg_Notifiche.DataKeys[i].Values["idRuolo"].ToString();
                    IDRuoloCorrente = ruolo;
                    if (this.contaChk(IDRuoloCorrente) == 0)
                        msg += "<br/>- inserire almeno un utente per il ruolo:<br/>" + this.dg_Notifiche.Rows[i].Cells[0].Text.Replace("<b>", "").Replace("</b>", "");
                }
            }

            // avvisa che non è stato selezionato almeno un utente per un dato ruolo
            if (!msg.Equals(string.Empty))
            {
                msg = "Attenzione!" + msg;

                string msgDesc = "WarningModelliCustom";
                string errFormt = Server.UrlEncode(msg);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);

                //string jscript = "<script>alert('" + msg + "');</script>";
                //if (!ClientScript.IsStartupScriptRegistered("avvisaNoChk"))
                //    ClientScript.RegisterStartupScript(this.GetType(), "avvisaNoChk", jscript);
                return false;
            }

            // cessione diritti
            if (this.utenteAbilitatoACL)
            {
                CheckBox spuntaCessione;
                CheckBox spuntaNotifica;

                // verifica se esistono SPUNTA Cessione abilitate (questo avviene quando la ragione trasm.ne prevede cessione)
                bool esistonoCheckCessione = false;
                for (int i = 0; i < this.dg_Notifiche.Rows.Count; i++)
                {
                    spuntaCessione = (CheckBox)dg_Notifiche.Rows[i].Cells[6].FindControl("Chk_C");
                    if (spuntaCessione.Enabled)
                    {
                        esistonoCheckCessione = true;
                        break;
                    }
                }

                if (esistonoCheckCessione)
                {
                    // verifica se è stata impostata la cessione
                    bool esisteCessioneImpostata = false;
                    for (int i = 0; i < this.dg_Notifiche.Rows.Count; i++)
                    {
                        spuntaCessione = (CheckBox)dg_Notifiche.Rows[i].Cells[6].FindControl("Chk_C");
                        if (spuntaCessione.Enabled)
                        {
                            if (spuntaCessione.Checked)
                            {
                                esisteCessioneImpostata = true;
                                break;
                            }
                        }
                    }

                    if (!esisteCessioneImpostata)
                    {
                        msg = "Attenzione!<br/>impostare almeno una spunta di cessione";

                        string msgDesc = "WarningModelliCustom";
                        string errFormt = Server.UrlEncode(msg);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);

                        //string jscript = "<script>alert('" + msg + "');</script>";
                        //if (!ClientScript.IsStartupScriptRegistered("avvisaNoChk"))
                        //    ClientScript.RegisterStartupScript(this.GetType(), "avvisaNoChk", jscript);
                        return false;
                    }
                }

                // verifica della SPUNTA Notifica -> SPUNTA Cessione
                for (int i = 0; i < this.dg_Notifiche.Rows.Count; i++)
                {
                    spuntaCessione = (CheckBox)dg_Notifiche.Rows[i].Cells[6].FindControl("Chk_C");
                    if (spuntaCessione.Checked)
                    {
                        spuntaNotifica = (CheckBox)dg_Notifiche.Rows[i].Cells[1].FindControl("Chk");
                        if (!spuntaNotifica.Checked)
                        {
                            msg = "Attenzione!<br/>Per l'utente con la spunta di cessione bisogna selezionare anche la notifica trasmissione";

                            string msgDesc = "WarningModelliCustom";
                            string errFormt = Server.UrlEncode(msg);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);

                            //string jscript = "<script>alert('" + msg + "');</script>";
                            //if (!ClientScript.IsStartupScriptRegistered("avvisaNoChk"))
                            //    ClientScript.RegisterStartupScript(this.GetType(), "avvisaNoChk", jscript);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Conta quante selezioni sono presenti per un dato ruolo
        /// </summary>
        /// <param name="id">id Ruolo</param>
        /// <returns></returns>
        private int contaChk(string id)
        {
            CheckBox spunta;
            int contaChk = 0;
            for (int i = 0; i < this.dg_Notifiche.Rows.Count; i++)
            {
                string idRuolo = dg_Notifiche.DataKeys[i].Values["idRuolo"].ToString();
                string tipo = dg_Notifiche.DataKeys[i].Values["tipo"].ToString();
                if (tipo.ToUpper().Equals("P") && idRuolo.Equals(id))
                {
                    spunta = (CheckBox)dg_Notifiche.Rows[i].Cells[1].FindControl("Chk");
                    if (spunta.Checked)
                        contaChk++;
                }
            }
            return contaChk;
        }


        private void impostaMode()
        {
            if (Session["mode"] != null)
            {
                this.btn_ok.Text = (Session["mode"].ToString().Equals("INSERT") ? "Salva" : "Conferma");
                this.hd_mode.Value = Session["mode"].ToString();
                this.lbl_nota.Text = GetLabel("ModelliTrasmissioneLblNota").Replace("@@", (Session["mode"].ToString().Equals("INSERT") ? "" : "Il tasto Conferma consolida solo i dati impostati su questa pagina"));
            }
        }

        private bool isCessioneAbilitata()
        {
            return (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["GEST_RAG_TRASM_CESSIONE"]) && System.Configuration.ConfigurationManager.AppSettings["GEST_RAG_TRASM_CESSIONE"].Equals("1"));
        }

        private string isRagioneConCessione(int idRagTrasm)
        {
            string retValue = "false";
            try
            {
                //DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();
                DocsPaWR.OrgRagioneTrasmissione ragione = TrasmManager.GetRagioneTrasmissione(Convert.ToString(idRagTrasm));
                if (ragione != null)
                    if (!ragione.PrevedeCessione.Equals(NttDataWA.DocsPaWR.CedeDiritiEnum.No))
                        retValue = "true";
            }
            catch
            {
                retValue = "false";
            }
            return retValue;
        }

        protected void dg_Notifiche_DataBound(object sender, EventArgs e)
        {
            CheckBox spunta;

            if (!IsPostBack)
            {
                DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

                for (int i = 0; i < this.dg_Notifiche.Rows.Count; i++)
                {
                    string tipo = dg_Notifiche.DataKeys[i].Values["tipo"].ToString();
                    // per le righe di tipo RUOLO non viene visualizzata la checkbox
                    if (tipo.ToUpper().Equals("R"))
                    {
                        this.dg_Notifiche.Rows[i].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                        spunta = (CheckBox)dg_Notifiche.Rows[i].Cells[1].FindControl("Chk");
                        spunta.Visible = false;

                        // cessione diritti
                        spunta = (CheckBox)dg_Notifiche.Rows[i].Cells[6].FindControl("Chk_C");
                        spunta.Visible = false;
                    }

                    // cessione diritti
                    spunta = (CheckBox)dg_Notifiche.Rows[i].Cells[6].FindControl("Chk_C");
                    if (utenteAbilitatoACL)
                        spunta.Attributes.Add("onclick", "SingleSelect('Chk_C',this);");
                    else
                    {
                        if (modello.CEDE_DIRITTI != null && modello.CEDE_DIRITTI.Equals("1"))
                            spunta.Enabled = false;
                    }
                }

                // cessione diritti
                if (!utenteAbilitatoACL && modello.CEDE_DIRITTI != null && modello.CEDE_DIRITTI.Equals("1"))
                    this.dg_Notifiche.Columns[6].Visible = true;
                if (!utenteAbilitatoACL && (modello.CEDE_DIRITTI != null && (modello.CEDE_DIRITTI.Trim().Equals("") || modello.CEDE_DIRITTI.Equals("0"))))
                    this.dg_Notifiche.Columns[6].Visible = false;
            }
        }
    }
}
