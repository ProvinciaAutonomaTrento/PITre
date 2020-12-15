using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using DocsPAWA.utils;

namespace DocsPAWA.Spedizione
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SpedizioneDocumento : DocsPAWA.CssPage
    {
		private DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;
        protected Utilities.MessageBox msg_SpedizioneAutomatica;
        //Andrea
        private string messError = "";
        //End Andrea

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            this.msg_SpedizioneAutomatica.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_SpedizioneAutomatica_GetMessageBoxResponse);
            base.OnInit(e);
        }
        #endregion
		
        #region Public members


        #endregion

        #region Protected members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.Response.Expires = -1;

                if (!this.IsPostBack)
                {
					this.schedaDocumento = DocumentManager.getDocumentoSelezionato();
                    bool fileAcquisito = (this.schedaDocumento.documenti != null && this.schedaDocumento.documenti[0].fileSize != "0");
                    if (!fileAcquisito)
                        this.Session["showConfirmSpedizioneAutomatica"] = true;
                    else
                        this.Session.Remove("showConfirmSpedizioneAutomatica");
                    // La sezione relativa agli interoperanti per IS non deve essere visibile se l'IS è disabilitata
                    this.listaDestinatatiInteropSempl.Visible = InteroperabilitaSemplificataManager.IsEnabledSimpInterop;

                    this.btnSpedisci.OnClientClick = string.Format("return btnSpedisci_onClientClick();");

                    // Caricamento dati delle spedizioni del documento
                    DocsPaWR.SpedizioneDocumento infoSpedizione = this.GetSpedizioneDocumento();
                    
                    CheckMezzoDiSpedizione(infoSpedizione.DestinatariEsterni);

                    this.FetchRegistri(infoSpedizione);

                    this.FetchRegistriIs();

                    this.FetchData(infoSpedizione);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.OpenErrorPage(this, ex, "Page_Load");
            }
        }

        /// <summary>
        /// Handler per l'evento di risposta alla messagebox di spedizione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void msg_SpedizioneAutomatica_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                this.Session.Remove("showConfirmSpedizioneAutomatica");
            }
            else
                this.btnSpedisci.Enabled = false;
        }

        /// <summary>		
        /// Metodo per il caricamento dei registri e degli RF abilitati all'interoperabilità
        /// semplificata. 
        /// </summary>
        private void FetchRegistriIs()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    DocsPaWR.SpedizioneDocumento infoSpedizione = this.GetSpedizioneDocumento();

                    DocsPaWR.SchedaDocumento scheda = DocumentManager.getDocumentoSelezionato();
                    if (scheda.tipoProto.ToUpper().Equals("I"))
                    {
                        this.lblTitle.Text = "Trasmissione documento";
                        //  listaDestinatariInteroperanti.Visible = false;
                        //  listaDestinatariNonInteroperanti.Visible = false;
                        this.Title = "Trasmissione documento";
                        SetStatoSpedizioneTrasmissione(infoSpedizione);
                    }
                    else
                    {
                        // Impostazione dello stato della spedizione del documento
                        this.lblTitle.Text = "Spedizione documento";
                        //listaDestinatariInteroperanti.Visible = true;
                        //listaDestinatariNonInteroperanti.Visible = true;
                        this.Title = "Spedizione documento";
                        this.SetStatoSpedizione(infoSpedizione);

                    }
                    // Impostazione abilitazione controlli
                    this.SetControlsEnabled(infoSpedizione);

                    // Impostazione visibilità controlli
                    this.SetControlsVisibility(infoSpedizione);
                    if (this.Session["showConfirmSpedizioneAutomatica"] != null)
                    {
                        this.msg_SpedizioneAutomatica.Confirm("E' stata richiesta la spedizione senza aver associato il documento principale. \\nSi vuole procedere comunque (SCELTA SCONSIGLIATA)?");
                        Session.Remove("showConfirmSpedizioneAutomatica");
                    }
                }
                
            }
            catch (Exception ex)
            {
                ErrorManager.OpenErrorPage(this, ex, "Page_PreRender");
            }
        }

        /// <summary>
        /// Effettua dei controlli legati al tipo di mezzo di spedizione scelto per ogni singolo destinatario 
        /// </summary>
        /// <param name="destinatari"></param>
        protected void CheckMezzoDiSpedizione(DocsPaWR.DestinatarioEsterno[] destinatari)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            foreach (DocsPaWR.DestinatarioEsterno dest in destinatari)
            {
                //se destinatario diverso da occasionale
                if (!dest.DatiDestinatari[0].tipoCorrispondente.ToUpper().Equals("O"))
                { 
                    //se il destinatario di default è interoperante ma per la trasmissione corrente, ha scelto un mezzo di trasmissione non interoperante, allora
                    // aggiorno le informazioni sullo stato.
                    if (dest.DatiDestinatari[0].canalePref != null)
                    {
                        if ((!string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.descrizione)) &&
                            (!dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("MAIL")) &&
                            (!dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")) &&
                            (!dest.DatiDestinatari[0].canalePref.descrizione.Equals("Interoperabilità PITRE")))
                            //((!string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.typeId) && !dest.DatiDestinatari[0].canalePref.typeId.ToUpper().Equals("S")) ||
                            //(!string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.tipoCanale) && !dest.DatiDestinatari[0].canalePref.tipoCanale.ToUpper().Equals(InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId))))
                        
                        { 
                            // destinatario non interoperante
                            dest.Interoperante = false;
                            dest.StatoSpedizione.Descrizione = "mezzo di spedizione: " + dest.DatiDestinatari[0].canalePref.descrizione;
                        }
                    }
                    DocsPaWR.Canale canalePref = UserManager.getCorrispondenteBySystemID(this.Page,dest.DatiDestinatari[0].systemId).canalePref;
                    //se il canale preferenziale non è definito ed il mezzo di spedizione per la trasmissione corrente non è interoperante,
                    //allora aggiorno le informazioni sullo stato ed imposto il corrispondente come non interoperante  
                    if (canalePref == null && !dest.DatiDestinatari[0].tipoIE.Equals("I"))
                    {
                        // Ticket Unitn-Apss dopo rilascio reperortori 3.19.x-brach.. impossibile spedire i documenti se un destinatario non ha il mezzo 
                        //if (
                        //    !string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.descrizione) &&
                        //    ((dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("MAIL")) ||
                        //    (dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA"))))
                        //{
                            dest.Interoperante = false;
                            dest.StatoSpedizione.Descrizione = "Canale preferenziale non definito";
                        //}
                    }

                    if (canalePref != null && 
                        ((!string.IsNullOrEmpty(canalePref.descrizione)) && 
                        (!canalePref.descrizione.ToUpper().Equals("MAIL")) && 
                        (!canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA"))) &&
                        (!dest.DatiDestinatari[0].canalePref.descrizione.Equals("Interoperabilità PITRE")))
                        //((!string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.typeId) && !dest.DatiDestinatari[0].canalePref.typeId.ToUpper().Equals("S")) ||
                        //(!string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.tipoCanale) && !dest.DatiDestinatari[0].canalePref.tipoCanale.ToUpper().Equals(InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId))))
                    {
                        if ((!string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.descrizione)) &&
                            ((!dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("MAIL")) &&
                            (!dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")) &&
                            (!dest.DatiDestinatari[0].canalePref.descrizione.Equals("Interoperabilità PITRE"))))
                        {
                            dest.Interoperante = false;
                            dest.StatoSpedizione.Descrizione = "Canale preferenziale non interoperante.\nmezzo di spedizione: " + dest.DatiDestinatari[0].canalePref.descrizione;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSpedizione"></param>
        protected void SetControlsEnabled(DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            /*
            if ((MultiCasellaManager.RoleIsAuthorizedSend(this.Page, "E") && !ddl_caselle.SelectedValue.Equals(string.Empty)) ||
                !MultiCasellaManager.RoleIsAuthorizedSend(this.Page, "E"))
                this.btnSpedisci.Enabled = true;
            else
                this.btnSpedisci.Enabled = false;
             * */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSpedizione"></param>
        protected void SetControlsVisibility(DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            this.trRegistriRF.Visible = (this.cboRegistriRF.Items.Count > 0);
            // se ci sono solo destinatari interop interna, abilito il pulsante di spedizione
            if (infoSpedizione.DestinatariEsterni.Count() == infoSpedizione.DestinatariEsterni.Count(d => d.DatiDestinatari[0].tipoIE != null && d.DatiDestinatari[0].tipoIE.Equals("I")))
                btnSpedisci.Enabled = true;
        }

        /// <summary>
        /// Impostazione dello stato della spedizione del documento
        /// </summary>
        /// <param name="infoSpedizione"></param>
        protected void SetStatoSpedizione(DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            if (infoSpedizione.Spedito)
            {
                // Se il documento è già stato spedito almeno una volta,
                // il testo del pulsante "Spedisci" diventa "Rispedisci"
                this.btnSpedisci.Text = "Rispedisci";
            }
            else
            {
                this.btnSpedisci.Text = "Spedisci";
            }

            // Impostazione dello stato della spedizione nei dettagli
            this.listaDestinatariInterni.SetStatoSpedizione(infoSpedizione.Spedito);
            this.listaDestinatariInteroperanti.SetStatoSpedizione(infoSpedizione.Spedito);
        }


        /// <summary>
        /// Impostazione dello stato della spedizione del documento
        /// </summary>
        /// <param name="infoSpedizione"></param>
        protected void SetStatoSpedizioneTrasmissione(DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            if (infoSpedizione.Spedito)
            {
                // Se il documento è già stato spedito almeno una volta,
                // il testo del pulsante "Spedisci" diventa "Rispedisci"
                this.btnSpedisci.Text = "Ritrasmetti";
            }
            else
            {
                this.btnSpedisci.Text = "Trasmetti";
            }

            // Impostazione dello stato della spedizione nei dettagli
            this.listaDestinatariInterni.SetStatoSpedizione(infoSpedizione.Spedito);
            this.listaDestinatariInteroperanti.SetStatoSpedizione(infoSpedizione.Spedito);
        }


        /// <summary>
        /// Verifica se la versione corrente del documento è stata acquista o meno
        /// </summary>
        protected bool IsDocumentoAcquisito
        {
            get
            {
                bool retValue = false;

                DocsPaWR.SchedaDocumento documento = DocumentManager.getDocumentoSelezionato();

                if (documento.documenti != null)
                {
                    int fileSize;
                    if (Int32.TryParse(documento.documenti[0].fileSize, out fileSize))
                        retValue = (fileSize > 0);
                }

                return retValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void FetchData()
        {
            this.FetchData(this.GetSpedizioneDocumento());
        }

        /// <summary>
        /// Caricamento dati delle spedizioni effettuate per il documento
        /// </summary>
        /// <param name="infoSpedizione"></param>
        protected virtual void FetchData(DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            // Caricamento dati nei singoli usercontrol
            this.listaDestinatariInterni.FetchData(infoSpedizione, TipoDestinatarioEnum.Interno);
            this.listaDestinatariInteroperanti.FetchData(infoSpedizione, TipoDestinatarioEnum.Esterno);
            this.listaDestinatariNonInteroperanti.FetchData(infoSpedizione, TipoDestinatarioEnum.EsternoNonInteroperante);
            this.listaDestinatatiInteropSempl.FetchData(infoSpedizione, TipoDestinatarioEnum.SimplifiedInteroperability);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSpedizione"></param>
        protected virtual void FetchRegistri(DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            // Se è presente almeno un destinatario interoperante, 
            // vengono caricati i registri e gli RF visibili dall'utente corrente
            if (infoSpedizione.DestinatariEsterni.Where(e => e.Interoperante).Count() > 0)
            {
                // Inserimento elemento vuoto
                this.cboRegistriRF.Items.Add(new ListItem(string.Empty, string.Empty));

                DocsPaWR.Registro[] registriRF = DocsPAWA.utils.MultiCasellaManager.GetRegisterEnabledSend(this.Page).ToArray();

                this.cboRegistriRF.Items.AddRange(
                    (from reg in registriRF
                     select new ListItem(
                     string.Format("{0} - {1}", reg.codRegistro, reg.descrizione), reg.systemId)).ToArray());

                int countRF = registriRF.Count(e => e.chaRF == "1");

                if (this.SelezionaRFPredefinito)
                {
                    if (countRF == 0)
                    {
                        // Nessun RF presente, selezione sul registro di protocollazione
                        //if (registriRF.Where(reg => reg.systemId == infoSpedizione.IdRegistroRfMittente) != null)
						if (registriRF.Count(reg => reg.systemId == infoSpedizione.IdRegistroRfMittente) > 0) //Riallineamento codice da versione 3.22
                            this.cboRegistriRF.SelectedValue = registriRF.Where(reg => reg.systemId == infoSpedizione.IdRegistroRfMittente).First().systemId;
                        else
                            this.cboRegistriRF.SelectedValue = string.Empty;
                        //this.cboRegistriRF.SelectedValue = registriRF.Contains(UserManager.getRegistroBySistemId(this.Page, infoSpedizione.IdRegistroRfMittente)) ? infoSpedizione.IdRegistroRfMittente : string.Empty;
                    }
                    else if (countRF == 1)
                    {
                        // Se è presente un solo RF, viene selezionato per impostazione predefinita nella combo

                        DocsPaWR.Registro rf = registriRF.Where(e => e.chaRF == "1").First();
                        this.cboRegistriRF.SelectedValue = rf.systemId;
                    }
                    else if (countRF > 1)
                    {
                        // In presenza di più di un RF, la selezione viene effettuata sull'elemento vuoto
                        this.cboRegistriRF.SelectedValue = string.Empty;
                    }
                }
                else
                {
                    //this.cboRegistriRF.SelectedValue = registriRF.Contains(UserManager.getRegistroBySistemId(this.Page,infoSpedizione.IdRegistroRfMittente)) ? infoSpedizione.IdRegistroRfMittente : string.Empty;
                    if (countRF == 0)
                    {
                        //if (registriRF.Where(reg => reg.systemId == infoSpedizione.IdRegistroRfMittente) != null)
						if (registriRF.Count(reg => reg.systemId == infoSpedizione.IdRegistroRfMittente) > 0) //Riallineamento codice da versione 3.22
                            this.cboRegistriRF.SelectedValue = registriRF.Where(reg => reg.systemId == infoSpedizione.IdRegistroRfMittente).First().systemId;
                        else
                            this.cboRegistriRF.SelectedValue = string.Empty;
                    }
                    else
                        // In presenza di più di un RF, seleziona elemento vuoto
                        this.cboRegistriRF.SelectedValue = string.Empty;
                }

                if(!string.IsNullOrEmpty(this.cboRegistriRF.SelectedValue))
                {
                    SetCaselleRegistro("1");
                    SetRicevutaPecCombo();
                    ddl_caselle.Enabled = true;
                    cboTipoRicevutaPec.Enabled = true;
                    // Refresh id registro selezionato e mail address
                    infoSpedizione.IdRegistroRfMittente = this.cboRegistriRF.SelectedValue;
                    infoSpedizione.mailAddress = this.ddl_caselle.SelectedValue;
                    this.SetSpedizioneDocumento(infoSpedizione);
                }
            }
        }

        private void SetRicevutaPecCombo()
        {
            string ricevutaPecType = SpedizioneManager.getSetRicevutaPec(this.cboRegistriRF.SelectedValue, null, null, true, ddl_caselle.SelectedValue);

            if (ricevutaPecType == null)
                ricevutaPecType = string.Empty;
            else
                if (ricevutaPecType.Length > 0)
                    ricevutaPecType = ricevutaPecType.Substring(0, 1);
                else
                    ricevutaPecType = string.Empty;

            switch (ricevutaPecType)
            {
                case "B":
                case "S":
                case "C":
                    lblRicevutaPec.Visible = true;
                    cboTipoRicevutaPec.Visible = true;
                    this.cboTipoRicevutaPec.SelectedValue = ricevutaPecType;
                    break;
                default:
                    lblRicevutaPec.Visible = false;
                    cboTipoRicevutaPec.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSpedizione"></param>
        protected virtual void Spedisci(DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            // Spedizione del documento ai destinatari selezionati
            DocsPaWR.SchedaDocumento scheda = DocumentManager.getDocumentoSelezionato();


            //filtro dagli allegati da spedire quelli associati a notifiche di tipo PEC
            List<DocsPaWR.Allegato> listAllegati = new List<DocsPaWR.Allegato>();
            if (scheda.allegati != null && scheda.allegati.Length > 0)
            {
                foreach (DocsPaWR.Allegato a in scheda.allegati)
                {
                    if (a.versionId != null)
                    {
                        if (!DocumentManager.AllegatoIsPEC(a.versionId).Equals("1"))
                            listAllegati.Add(a);
                    }
                }
                scheda.allegati = listAllegati.ToArray();
            }

            infoSpedizione = SpedizioneManager.SpedisciDocumento(scheda, infoSpedizione);

            //Andrea
            foreach (string s in infoSpedizione.listaDestinatariNonRaggiungibili) 
            {
                messError = messError + s + "\\n";
            }
            if (infoSpedizione != null && infoSpedizione.listaDestinatariNonRaggiungibili != null)
            {
                infoSpedizione.listaDestinatariNonRaggiungibili = null;
            }

            if (messError != "")
            {
                Session.Add("MessError", messError);
            }
            //End Andrea

            // Impostazione dei dati di spedizione
            this.SetSpedizioneDocumento(infoSpedizione);

            this.FetchData(infoSpedizione);

            if (scheda.tipoProto.ToUpper().Equals("I"))
                this.SetStatoSpedizioneTrasmissione(infoSpedizione);
            else
                 // Impostazione dello stato della spedizione
                this.SetStatoSpedizione(infoSpedizione);

            this.SetReturnValue(true);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cboRegistriRF_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_caselle.Items.Clear();
            if (!string.IsNullOrEmpty(this.cboRegistriRF.SelectedValue))
            {
                DocsPaWR.SpedizioneDocumento infoSpedizione = this.GetSpedizioneDocumento();
                //aggiorno le caselle associate al registro/rf appena selezionato
                SetCaselleRegistro("1");
                SetRicevutaPecCombo();
                ddl_caselle.Enabled = true;
                cboTipoRicevutaPec.Enabled = true;
                infoSpedizione.IdRegistroRfMittente = this.cboRegistriRF.SelectedValue;
                infoSpedizione.mailAddress = this.ddl_caselle.SelectedValue;
                this.SetSpedizioneDocumento(infoSpedizione);
            }
            else
            {
                ddl_caselle.Enabled = false;
                cboTipoRicevutaPec.Enabled = false;
                DocsPaWR.SpedizioneDocumento infoSpedizione = this.GetSpedizioneDocumento();
                infoSpedizione.IdRegistroRfMittente = string.Empty;
                this.SetSpedizioneDocumento(infoSpedizione);
            }
            // Impostazione abilitazione controlli
            this.SetControlsEnabled(this.GetSpedizioneDocumento());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSpedisci_Click(object sender, EventArgs e)
        {
			//ABBATANGELI GIANLUIGI AsyncPostBackTimeOut="3600" (1ora) spostato in <asp:ScriptManager /> della pagina aspx
            //setto il timeout della richiesta di aggiornamento asincrona a 5 minuti
            //scrManager.AsyncPostBackTimeout = 900;
            //Non cambio il default, imposto un temporaneo.
            if (cboTipoRicevutaPec.Visible == true)
                SpedizioneManager.getSetRicevutaPec(cboRegistriRF.SelectedValue, null, cboTipoRicevutaPec.SelectedValue, false, ddl_caselle.SelectedValue);
            try
            {

                DocsPaWR.ConfigSpedizioneDocumento config = SpedizioneManager.GetConfigSpedizioneDocumento();
                DocsPaWR.SpedizioneDocumento infoSpedizione = this.GetSpedizioneDocumento();

                // Aggiornamento dati destinatari selezionati per la spedizione
                this.listaDestinatariInterni.SaveData(infoSpedizione);
                this.listaDestinatariInteroperanti.SaveData(infoSpedizione);
                this.listaDestinatatiInteropSempl.SaveData(infoSpedizione);

                bool almostOne = (this.listaDestinatariInterni.Items > 0 && this.listaDestinatariInterni.AlmostOneChecked);
                
                if (!almostOne)
                    almostOne = (this.listaDestinatariInteroperanti.Items > 0 && this.listaDestinatariInteroperanti.AlmostOneChecked) || (this.listaDestinatatiInteropSempl.Items > 0 && this.listaDestinatatiInteropSempl.AlmostOneChecked);

                if (!almostOne)
                {
                    this.ShowErrorMessage("Nessun destinatario selezionato per la spedizione del documento");
                }
                /*else if (!this.IsDocumentoAcquisito && config.AvvisaSuSpedizioneDocumento)
                {
                    this.msgSpedisci.Confirm("E' stata richiesta la spedizione senza aver associato alcun documento elettronico.\\nSi vuole eseguire le operazioni di trasmissione e spedizione automatiche?");
                }*/ //Gestito con nuovo messaggio
                else
                {
                    this.Spedisci(infoSpedizione);
                }

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseErroreMsg",
                    "<script>CloseWaitPanel();</script>", false);
            }
            catch (Exception ex)
            {
                ErrorManager.OpenErrorPage(this, ex, "btnSpedisci_Click");
            }

            //Andrea
            if (Session["MessError"] != null)
            {
                messError = Session["MessError"].ToString();
                //messError.Replace("'", "\'");
                //Response.Write("<script language=\"javascript\">alert('Spedizioni con esito negativo: \\n" + messError + "');</script>");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messError",
                    "<script language=\"javascript\">alert('Spedizioni con esito negativo: \\n" + messError + "');</script>", false);
                Session.Remove("MessError");
                messError = string.Empty;
            }
            //End Andrea

        }

        /// <summary>
        /// 
        /// </summary>
        protected void Spedisci()
        {
            DocsPaWR.SpedizioneDocumento infoSpedizione = this.GetSpedizioneDocumento();

            this.Spedisci(infoSpedizione);

            this.SetReturnValue(true);
        }

        /// <summary>
        /// Impostazione messaggio di errore
        /// </summary>
        /// <param name="errorMessage"></param>
        protected void ShowErrorMessage(string errorMessage)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ErrorMessage", 
                "<script>alert('" + errorMessage.Replace("'", "\\'") + "');</script>", false);
        }

        /// <summary>
        /// Impostazione valore di ritorno della dialog
        /// </summary>
        /// <param name="value"></param>
        protected void SetReturnValue(bool value)
        {
            this.txtReturnValue.Value = value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSpedizione"></param>
        protected void SetSpedizioneDocumento(DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            this.ViewState["SpedizioniDocumento"] = infoSpedizione;
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool SelezionaRFPredefinito
        {
            get
            {
                bool retValue = false;
                bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["SPEDIZIONE_SELEZIONA_RF_PREDEFINITO"], out retValue);
                return retValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void msgSpedisci_messageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            try
            {
                if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
                    this.Spedisci();
                else
                    this.FetchData();
            }
            catch (Exception ex)
            {
                ErrorManager.OpenErrorPage(this, ex, "msgSpedisci_messageBoxResponse");
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DocsPaWR.SpedizioneDocumento GetSpedizioneDocumento()
        {
            DocsPaWR.SpedizioneDocumento instance = this.ViewState["SpedizioniDocumento"] as DocsPaWR.SpedizioneDocumento;

            if (instance == null)
            {
                instance = SpedizioneManager.GetSpedizioneDocumento(DocumentManager.getDocumentoSelezionato());

                this.ViewState["SpedizioniDocumento"] = instance;
            }

            return instance;
        }

        #region Multi Casella
        /// <summary>
        /// Aggiorna la drop down list delle caselle abilitate alla spedizione associate al registro selezionato
        /// </summary>
        private void SetCaselleRegistro(string selectMain)
        {
            //se è stato selezionato un registro/rf nella ddl dei registri mittente, allora setto la ddl delle caselle associate al registro
            if (!string.IsNullOrEmpty(cboRegistriRF.SelectedValue))
            {
                List<DocsPAWA.DocsPaWR.CasellaRegistro> listCaselle = new List<DocsPaWR.CasellaRegistro>();
                listCaselle = DocsPAWA.utils.MultiCasellaManager.GetComboRegisterSend(cboRegistriRF.SelectedValue);
                foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in listCaselle)
                {
                    System.Text.StringBuilder formatMail = new System.Text.StringBuilder();
                    if (c.Principale.Equals("1"))
                        formatMail.Append("* ");
                    formatMail.Append(c.EmailRegistro);
                    if (!string.IsNullOrEmpty(c.Note))
                    {
                        formatMail.Append(" - ");
                        formatMail.Append(c.Note);
                    }
                    ddl_caselle.Items.Add(new ListItem(formatMail.ToString(), c.EmailRegistro));
                }

                if (listCaselle.Count == 0)
                {
                    ddl_caselle.Enabled = false;
                    ddl_caselle.Width = new Unit(200);
                    return;
                }
                //se ho appena settato un nuovo registro/rf allora seleziono la casella principale
                if(selectMain.Equals("1"))
                {
                    //imposto la casella principale come selezionata
                    foreach (ListItem i in ddl_caselle.Items)
                    {
                        if (i.Text.Split(new string[] { "*" }, 2, System.StringSplitOptions.None).Length > 1)
                        {
                            ddl_caselle.SelectedValue = i.Value;
                            break;
                        }
                    }
                }

            }
        }
        #endregion

        protected void ddl_caselle_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetRicevutaPecCombo();
            DocsPaWR.SpedizioneDocumento infoSpedizione = this.GetSpedizioneDocumento();
            infoSpedizione.mailAddress = ddl_caselle.SelectedValue;
            this.SetSpedizioneDocumento(infoSpedizione);
        }
    }
}
