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

namespace DocsPAWA.popup
{
    /// <summary>
    /// GIUGNO 2008  -  ADAMO
    /// Classe per la gestione delle notifiche trasmissione rispetto agli utenti del ruolo destinatario nei modelli di trasm.ne
    /// </summary>
    public partial class GestioneModelliTrasm_Notifiche : DocsPAWA.CssPage
    {
        protected DataSet dataSet;
        bool utenteAbilitatoACL = false;

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);
            try
            {
                this.impostaAbilitazioneACL();
                this.impostaTitolo();
                this.impostaIntestazione();

                if (!IsPostBack)
                {
                    this.generaLista();
                    this.impostaMode();
                }
            }
            catch
            {
                string jscript = "<script>alert('Errore nel reperimento dei dati!'); window.close();</script>";
                if (!ClientScript.IsStartupScriptRegistered("avvisaEchiude"))
                    ClientScript.RegisterStartupScript(this.GetType(), "avvisaEchiude", jscript);
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
            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            if (modello.CHA_TIPO_OGGETTO.Equals("D"))
                funzione = "ABILITA_CEDI_DIRITTI_DOC";
            if (modello.CHA_TIPO_OGGETTO.Equals("F"))
                funzione = "ABILITA_CEDI_DIRITTI_FASC";

            this.utenteAbilitatoACL = UserManager.ruoloIsAutorized(this, funzione);
        }

        /// <summary>
        /// Gestione del titolo della pagina web
        /// </summary>
        private void impostaTitolo()
        {
            if (this.utenteAbilitatoACL)
                this.titolo.Text += " e Cessione diritti";
        }

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

            this.titolo.Text = intestazioneSuperiore;
            this.lbl_avviso.Text = intestazione;
        }

        /// <summary>
        /// Reperisce la system_id della groups rispetto alla system_id della corr_globali 
        /// </summary>
        /// <param name="idCorrGlobRuolo">system_id della tabella DPA_CORR_GLOBALI</param>
        /// <returns>system_id della tabella GROUPS</returns>
        private string impostaIDGroup(string idCorrGlobRuolo)
        {
            string retValue = string.Empty;

            DocsPAWA.DocsPaWR.Ruolo ruolo = new DocsPAWA.DocsPaWR.Ruolo();
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

            ruolo = ws.getRuoloById(idCorrGlobRuolo);

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
                DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

                // prima elimina gli oggetti UTENTI_NOTIFICA...
                foreach (DocsPAWA.DocsPaWR.RagioneDest ragioneDest in modello.RAGIONI_DESTINATARI)
                    foreach (DocsPAWA.DocsPaWR.MittDest mittDest in ragioneDest.DESTINATARI)
                        mittDest.UTENTI_NOTIFICA = null;

                // quindi reperisce i dati sul db
                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                modello = ws.UtentiConNotificaTrasm(modello, null, null, "GET");

                // rimette in sessione l'oggetto MODELLO aggiornato con i dati presi da db
                Session.Remove("Modello");
                Session.Add("Modello", modello);

                // inizializza il dataset che popola il datagrid
                IniDataSet();

                // genera le righe del datagrid
                foreach (DocsPAWA.DocsPaWR.RagioneDest ragioneDest in modello.RAGIONI_DESTINATARI)
                    foreach (DocsPAWA.DocsPaWR.MittDest mittDest in ragioneDest.DESTINATARI)
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
        private void loadDataSet(DocsPaWR.ModelloTrasmissione modello, DocsPAWA.DocsPaWR.MittDest mittDest, string disabled)
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
                foreach (DocsPAWA.DocsPaWR.UtentiConNotificaTrasm ut in mittDest.UTENTI_NOTIFICA)
                {
                    row = dataSet.Tables[0].NewRow();
                    row["descrizione"] = "&nbsp;&nbsp;" + ut.CODICE_UTENTE + " - " + ut.NOME_COGNOME_UTENTE.ToUpper();
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
        private void chiudePopup(string mode)
        {
            string jscript = string.Empty;

            switch (mode)
            {
                case "INSERT":
                    jscript = "<script>window.returnValue = 'I'; self.close();</script>";
                    break;
                case "UPDATE":
                    jscript = "<script>window.returnValue = 'U'; self.close();</script>";
                    break;
                case "NONE":
                    jscript = "<script>self.close();</script>";
                    break;
            }

            if (!ClientScript.IsStartupScriptRegistered("chiude"))
                ClientScript.RegisterStartupScript(this.GetType(), "chiude", jscript);
        }

        /// <summary>
        /// Tasto Chiudi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_annulla_Click(object sender, EventArgs e)
        {
            this.chiudePopup("NONE");
        }

        /// <summary>
        /// Tasto Salva e Chiudi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_ok_Click(object sender, EventArgs e)
        {
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

            try
            {
                DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
                // verifica se è selezionato almeno un utente per ruolo e la corretta cessione dei diritti
                if (controllaInserimentiUtente())
                {
                    foreach (DocsPAWA.DocsPaWR.RagioneDest ragioneDest in modello.RAGIONI_DESTINATARI)
                    {
                        foreach (DocsPAWA.DocsPaWR.MittDest mittDest in ragioneDest.DESTINATARI)
                        {
                            if (!mittDest.CHA_TIPO_URP.Equals("U"))
                            {
                                if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                                {
                                    foreach (DocsPAWA.DocsPaWR.UtentiConNotificaTrasm utNot in mittDest.UTENTI_NOTIFICA)
                                    {
                                        if (utNot.FLAG_NOTIFICA != "1")
                                        {
                                            statoChk = this.utenteSelezionato(utNot.ID_PEOPLE, Convert.ToString(mittDest.ID_CORR_GLOBALI));
                                            utNot.FLAG_NOTIFICA = statoChk;
                                        }
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
                    this.chiudePopup(this.hd_mode.Value);
                }
            }
            catch
            {
                string jscript = "<script>alert('Errore durante il salvataggio dei dati!'); window.close();</script>";
                if (!ClientScript.IsStartupScriptRegistered("erroreEchiude"))
                    ClientScript.RegisterStartupScript(this.GetType(), "erroreEchiude", jscript);
            }
        }

        private void PerformUpdate()
        {
            bool modificaEffettuata = false;
            string statoChk;
            ArrayList utentiDaInserire = new ArrayList();
            ArrayList utentiDaCancellare = new ArrayList();

            try
            {
                DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

                // verifica se è selezionato almeno un utente per ruolo e la corretta cessione dei diritti
                if (controllaInserimentiUtente())
                {
                    foreach (DocsPAWA.DocsPaWR.RagioneDest ragioneDest in modello.RAGIONI_DESTINATARI)
                    {
                        foreach (DocsPAWA.DocsPaWR.MittDest mittDest in ragioneDest.DESTINATARI)
                        {
                            if (!mittDest.CHA_TIPO_URP.Equals("U"))
                            {
                                if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                                {
                                    foreach (DocsPAWA.DocsPaWR.UtentiConNotificaTrasm utNot in mittDest.UTENTI_NOTIFICA)
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
                        DocsPAWA.DocsPaWR.UtentiConNotificaTrasm[] utentiDaInserire_1 = new DocsPAWA.DocsPaWR.UtentiConNotificaTrasm[utentiDaInserire.Count];
                        utentiDaInserire.CopyTo(utentiDaInserire_1);
                        DocsPAWA.DocsPaWR.UtentiConNotificaTrasm[] utentiDaCancellare_1 = new DocsPAWA.DocsPaWR.UtentiConNotificaTrasm[utentiDaCancellare.Count];
                        utentiDaCancellare.CopyTo(utentiDaCancellare_1);
                        // effettua il salvataggio dei dati                        
                        modello = ws.UtentiConNotificaTrasm(modello, utentiDaInserire_1, utentiDaCancellare_1, "SET");

                        if (modello == null)
                        {
                            string jscript = "<script>alert('Errore durante il salvataggio dei dati!'); window.close();</script>";
                            if (!ClientScript.IsStartupScriptRegistered("erroreEchiude"))
                                ClientScript.RegisterStartupScript(this.GetType(), "erroreEchiude", jscript);
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

                            ws.SalvaCessioneDirittiSuModelliTrasm(modello);
                        }
                    }

                    Session.Remove("impostaNotifiche");

                    // chiude la popup
                    this.chiudePopup(this.hd_mode.Value);
                }
            }
            catch
            {
                string jscript = "<script>alert('Errore durante il salvataggio dei dati!'); window.close();</script>";
                if (!ClientScript.IsStartupScriptRegistered("erroreEchiude"))
                    ClientScript.RegisterStartupScript(this.GetType(), "erroreEchiude", jscript);
            }
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
            for (int i = 0; i < this.dg_Notifiche.Items.Count; i++)
            {
                spunta = (CheckBox)this.dg_Notifiche.Items[i].Cells[6].FindControl("Chk_C");
                if (spunta.Checked)
                {
                    idPeople = this.dg_Notifiche.Items[i].Cells[2].Text.Replace("&nbsp;", "");
                    idGroup = this.dg_Notifiche.Items[i].Cells[5].Text.Replace("&nbsp;", "");
                    trovataSelezione = true;
                    break;
                }
            }

            if (trovataSelezione)
            {
                modelloModificato.CEDE_DIRITTI = "1";
               
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.OrgRagioneTrasmissione ragione = null;
            DocsPaWR.MittDest mittDest = null;

            foreach (DocsPaWR.RagioneDest ragDest in modelloModificato.RAGIONI_DESTINATARI)
            {
                mittDest = ragDest.DESTINATARI[0];
                ragione = ws.AmmGetRagioneTrasmissione(Convert.ToString(mittDest.ID_RAGIONE));
                if (ragione != null)
                {
                    //
                    // Mev Cessione Diritti - Mantieni Scrittura
                    if (ragione.MantieniScrittura == true)
                    {
                        modelloModificato.MANTIENI_SCRITTURA = "1";
                        break;
                    }
                    else
                    {
                        modelloModificato.MANTIENI_SCRITTURA = "0";
                    }
                    // End Mev
                    //

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
            for (int i = 0; i < this.dg_Notifiche.Items.Count; i++)
            {
                spunta = (CheckBox)this.dg_Notifiche.Items[i].Cells[6].FindControl("Chk_C");
                if (spunta.Checked)
                {
                    // verifica se i dati sono gli stessi di quelli reperiti sul db
                    if (modello.ID_PEOPLE_NEW_OWNER.Equals(this.dg_Notifiche.Items[i].Cells[2].Text) &&
                        modello.ID_GROUP_NEW_OWNER.Equals(this.dg_Notifiche.Items[i].Cells[5].Text))
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
            for (int i = 0; i < this.dg_Notifiche.Items.Count; i++)
            {
                if (this.dg_Notifiche.Items[i].Cells[2].Text.Equals(id) && this.dg_Notifiche.Items[i].Cells[3].Text.Equals(idRuolo))
                {
                    spunta = (CheckBox)dg_Notifiche.Items[i].Cells[1].FindControl("Chk");
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

            for (int i = 0; i < this.dg_Notifiche.Items.Count; i++)
            {
                if (this.dg_Notifiche.Items[i].Cells[4].Text.ToUpper().Equals("R"))
                {
                    IDRuoloCorrente = this.dg_Notifiche.Items[i].Cells[3].Text;
                    if (this.contaChk(IDRuoloCorrente) == 0)
                        msg += "\\n\\n- inserire almeno un utente per il ruolo:\\n" + this.dg_Notifiche.Items[i].Cells[0].Text.Replace("<b>", "").Replace("</b>", "");
                }
            }

            // avvisa che non è stato selezionato almeno un utente per un dato ruolo
            if (!msg.Equals(string.Empty))
            {
                msg = "Attenzione!" + msg;
                string jscript = "<script>alert('" + msg + "');</script>";
                if (!ClientScript.IsStartupScriptRegistered("avvisaNoChk"))
                    ClientScript.RegisterStartupScript(this.GetType(), "avvisaNoChk", jscript);
                return false;
            }

            // cessione diritti
            if (this.utenteAbilitatoACL)
            {
                CheckBox spuntaCessione;
                CheckBox spuntaNotifica;

                // verifica se esistono SPUNTA Cessione abilitate (questo avviene quando la ragione trasm.ne prevede cessione)
                bool esistonoCheckCessione = false;
                for (int i = 0; i < this.dg_Notifiche.Items.Count; i++)
                {
                    spuntaCessione = (CheckBox)dg_Notifiche.Items[i].Cells[6].FindControl("Chk_C");
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
                    for (int i = 0; i < this.dg_Notifiche.Items.Count; i++)
                    {
                        spuntaCessione = (CheckBox)dg_Notifiche.Items[i].Cells[6].FindControl("Chk_C");
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
                        msg = "Attenzione!\\nimpostare almeno una spunta di cessione";
                        string jscript = "<script>alert('" + msg + "');</script>";
                        if (!ClientScript.IsStartupScriptRegistered("avvisaNoChk"))
                            ClientScript.RegisterStartupScript(this.GetType(), "avvisaNoChk", jscript);
                        return false;
                    }
                }

                // verifica della SPUNTA Notifica -> SPUNTA Cessione
                for (int i = 0; i < this.dg_Notifiche.Items.Count; i++)
                {
                    spuntaCessione = (CheckBox)dg_Notifiche.Items[i].Cells[6].FindControl("Chk_C");
                    if (spuntaCessione.Checked)
                    {
                        spuntaNotifica = (CheckBox)dg_Notifiche.Items[i].Cells[1].FindControl("Chk");
                        if (!spuntaNotifica.Checked)
                        {
                            msg = "Attenzione!\\nper l\\'utente con la spunta di cessione bisogna selezionare anche la notifica trasmissione";
                            string jscript = "<script>alert('" + msg + "');</script>";
                            if (!ClientScript.IsStartupScriptRegistered("avvisaNoChk"))
                                ClientScript.RegisterStartupScript(this.GetType(), "avvisaNoChk", jscript);
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
            for (int i = 0; i < this.dg_Notifiche.Items.Count; i++)
            {
                if (this.dg_Notifiche.Items[i].Cells[4].Text.ToUpper().Equals("P") && this.dg_Notifiche.Items[i].Cells[3].Text.Equals(id))
                {
                    spunta = (CheckBox)dg_Notifiche.Items[i].Cells[1].FindControl("Chk");
                    if (spunta.Checked)
                        contaChk++;
                }
            }
            return contaChk;
        }

        /// <summary>
        /// dg_Notifiche ItemCreated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dg_Notifiche_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            CheckBox spunta;

            if (!IsPostBack)
            {
                DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

                for (int i = 0; i < this.dg_Notifiche.Items.Count; i++)
                {
                    // per le righe di tipo RUOLO non viene visualizzata la checkbox
                    if (this.dg_Notifiche.Items[i].Cells[4].Text.ToUpper().Equals("R"))
                    {
                        this.dg_Notifiche.Items[i].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                        spunta = (CheckBox)dg_Notifiche.Items[i].Cells[1].FindControl("Chk");
                        spunta.Visible = false;

                        // cessione diritti
                        spunta = (CheckBox)dg_Notifiche.Items[i].Cells[6].FindControl("Chk_C");
                        spunta.Visible = false;
                    }

                    // cessione diritti
                    spunta = (CheckBox)dg_Notifiche.Items[i].Cells[6].FindControl("Chk_C");
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

        private void impostaMode()
        {
            if (Request.QueryString["mode"] != null)
            {
                this.btn_ok.Text = (Request.QueryString["mode"].ToString().Equals("INSERT") ? "Salva" : "Conferma") + " e Chiudi";
                this.hd_mode.Value = Request.QueryString["mode"].ToString();
                this.lbl_nota.Text = (Request.QueryString["mode"].ToString().Equals("INSERT") ? "" : "Il tasto Conferma consolida solo i dati impostati su questa pagina");
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
                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                DocsPaWR.OrgRagioneTrasmissione ragione = ws.AmmGetRagioneTrasmissione(Convert.ToString(idRagTrasm));
                if (ragione != null)
                    if (!ragione.PrevedeCessione.Equals(DocsPAWA.DocsPaWR.CedeDiritiEnum.No))
                        retValue = "true";
            }
            catch
            {
                retValue = "false";
            }
            return retValue;
        }
    }
}
