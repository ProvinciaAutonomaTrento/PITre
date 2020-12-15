using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class Sender : System.Web.UI.Page
    {
        private DocsPaWR.SchedaDocumento schedaDocumento;
        private string messError = "";

        protected Boolean closeAll = false;

        //Contiene i destinatari a cui non è stato possibile fare la spedizione
        private string DestinatariNonRaggiunti
        {
            get
            {
                if ((HttpContext.Current.Session["DestinatariNonRaggiunti"]) != null)
                    return HttpContext.Current.Session["DestinatariNonRaggiunti"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["DestinatariNonRaggiunti"] = value;
            }
        }

        private int MaxFileSizeSend
        {
            get
            {
                if (HttpContext.Current.Session["MaxFileSizeSend"] != null)
                    return (int)HttpContext.Current.Session["MaxFileSizeSend"];
                else
                    return 0;
            }
            set
            {
                HttpContext.Current.Session["MaxFileSizeSend"] = value;
            }
        }

        private List<DocsPaWR.Messaggio> NextMessages
        {
            get
            {
                if ((HttpContext.Current.Session["NextMessages"]) != null)
                    return HttpContext.Current.Session["NextMessages"] as List<DocsPaWR.Messaggio>;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["NextMessages"] = value;
            }
        }

        private DocsPaWR.SpedizioneDocumento InfoSpedizioneSelectMessage
        {
            get
            {
                if ((HttpContext.Current.Session["InfoSpedizioneSelectMessage"]) != null)
                    return HttpContext.Current.Session["InfoSpedizioneSelectMessage"] as DocsPaWR.SpedizioneDocumento;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["InfoSpedizioneSelectMessage"] = value;
            }
        }

        private const string SELECT_NEXT_MESSAGE_CLOSE_POPUP = "SelectNextMessageClosePopup";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //this.Response.Expires = -1;
                if (!this.IsPostBack)
                {
                    DocsPaWR.SpedizioneDocumento infoSpedizione = this.GetSpedizioneDocumento();

                    schedaDocumento = DocumentManager.getSelectedRecord();

                    // Impostazione abilitazione controlli
                    this.SetControlsEnabled(infoSpedizione);

                    bool fileAcquisito = (this.schedaDocumento.documenti != null && this.schedaDocumento.documenti[0].fileSize != "0");
                    if (!fileAcquisito)
                    {
                        string language = UIManager.UserManager.GetUserLanguage();
                        this.messager.Text = Utils.Languages.GetMessageFromCode("SenderMessageNoDocument", language);
                    }

                    //Controllo dimensione massima dei file
                    if (CheckSendInterop(infoSpedizione.DestinatariEsterni))
                    {

                        if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, Utils.DBKeys.FE_DO_SEND_BIG_FILE.ToString())) &&
                            !Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, Utils.DBKeys.FE_DO_SEND_BIG_FILE.ToString()).Equals("0"))
                            this.MaxFileSizeSend = Convert.ToInt32(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, Utils.DBKeys.FE_DO_SEND_BIG_FILE.ToString()));
                        if (this.MaxFileSizeSend > 0 && FileManager.TolatFileSizeDocument(schedaDocumento.docNumber) > this.MaxFileSizeSend)
                        {
                            string maxSize = Convert.ToString(Math.Round((double)this.MaxFileSizeSend / 1048576, 3));
                            string language = UIManager.UserManager.GetUserLanguage();
                            this.messager.Text = Utils.Languages.GetMessageFromCode("WarningSenderMessageMaxFileSizeSend", language).Replace("@@", maxSize.ToString());
                        }
                    }
                    //    this.Session["showConfirmSpedizioneAutomatica"] = true;
                    //else
                    //    this.Session.Remove("showConfirmSpedizioneAutomatica");

                    // La sezione relativa agli interoperanti per IS non deve essere visibile se l'IS è disabilitata
                    this.listaDestinatatiInteropSempl.Visible = SimplifiedInteroperabilityManager.IsEnabledSimpInterop;

                    //this.SenderBtnSend.OnClientClick = string.Format("return btnSpedisci_onClientClick();");

                    CheckMezzoDiSpedizione(infoSpedizione.DestinatariEsterni);

                    this.FetchRegistri(infoSpedizione);

                    // Impostazione visibilità controlli
                    this.SetControlsVisibility(infoSpedizione);
                    //this.FetchRegistriIs();

                    this.FetchData(infoSpedizione);

                    InitializesForm(infoSpedizione, schedaDocumento.tipoProto);

                }
                else
                {
                    this.ReadRetValueFromPopup();
                }
                this.ReApplyChosenScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ReadRetValueFromPopup()
        {
            if (this.Request.Form["__EVENTARGUMENT"] != null && this.Request.Form["__EVENTARGUMENT"].Equals(SELECT_NEXT_MESSAGE_CLOSE_POPUP))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                if (!string.IsNullOrEmpty(this.SelectNextMessage.ReturnValue))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SelectNextMessage','');", true);
                    this.Spedisci(this.InfoSpedizioneSelectMessage);

                    if (Session["MessError"] != null)
                    {
                        messError = Session["MessError"].ToString();

                        string msgDesc = "WarningDocumentCustom";
                        string errFormt = Server.UrlEncode(messError);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);

                        Session.Remove("MessError");
                        messError = string.Empty;
                    }

                    if (listaDestinatariInterni.Items > 0) UpdateDestinatariInterni.Update();
                    if (listaDestinatariInteroperanti.Items > 0) UpdateDestinatariInteroperanti.Update();
                    if (listaDestinatatiInteropSempl.Items > 0) UpdateDestinatatiInteropSempl.Update();
                    if (listaDestinatariNonInteroperanti.Items > 0) UpdateDestinatariNonInteroperanti.Update();

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                    NttDataWA.DocsPaWR.SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
                    //aggiorno l'elenco allegati in sessione(per ricevute PITRE)
                    if (documentTab != null && !string.IsNullOrEmpty(documentTab.docNumber))
                    {
                        DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this.Page, documentTab.docNumber, documentTab.docNumber));
                    }

                    if (!string.IsNullOrEmpty(this.DestinatariNonRaggiunti))
                    {
                        string msgDesc = "WarningSendingRecipients";
                        string errFormt = this.DestinatariNonRaggiunti;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);
                        HttpContext.Current.Session.Remove("DestinatariNonRaggiunti");
                    }

                    return;
                }
            }
        }

        private void ReApplyChosenScript()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: 'Nessun risultato trovato' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: 'Nessun risultato trovato' });", true);
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
                            (!dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("PORTALE")) &&
                            (!dest.DatiDestinatari[0].canalePref.typeId.Equals(SimplifiedInteroperabilityManager.SimplifiedInteroperabilityId)))
                        //((!string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.typeId) && !dest.DatiDestinatari[0].canalePref.typeId.ToUpper().Equals("S")) ||
                        //(!string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.tipoCanale) && !dest.DatiDestinatari[0].canalePref.tipoCanale.ToUpper().Equals(InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId))))
                        {
                            // destinatario non interoperante
                            dest.Interoperante = false;
                            dest.StatoSpedizione.Descrizione = "mezzo di spedizione: " + dest.DatiDestinatari[0].canalePref.descrizione;
                        }
                    }
                    //DocsPaWR.Canale canalePref = UserManager.getCorrispondentBySystemID(dest.DatiDestinatari[0].systemId).canalePref;
                    DocsPaWR.Canale canalePref = SenderManager.getDatiCanPref(dest.DatiDestinatari[0]);
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
                        (!canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")) &&
                        (!canalePref.descrizione.ToUpper().Equals("PORTALE"))) &&
                        (dest.DatiDestinatari[0].canalePref != null && !dest.DatiDestinatari[0].canalePref.typeId.Equals(SimplifiedInteroperabilityManager.SimplifiedInteroperabilityId)))
                    //((!string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.typeId) && !dest.DatiDestinatari[0].canalePref.typeId.ToUpper().Equals("S")) ||
                    //(!string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.tipoCanale) && !dest.DatiDestinatari[0].canalePref.tipoCanale.ToUpper().Equals(InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId))))
                    {
                        if ((!string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.descrizione)) &&
                            ((!dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("MAIL")) &&
                            (!dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")) &&
                            (!dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("PORTALE")) &&
                            (!dest.DatiDestinatari[0].canalePref.typeId.Equals(SimplifiedInteroperabilityManager.SimplifiedInteroperabilityId))))
                        {
                            dest.Interoperante = false;
                            dest.StatoSpedizione.Descrizione = "Canale preferenziale non interoperante.\nmezzo di spedizione: " + dest.DatiDestinatari[0].canalePref.descrizione;
                        }
                    }
                }
            }
        }

        private bool CheckSendInterop(DocsPaWR.DestinatarioEsterno[] destinatari)
        {
            bool result = false;

            foreach (DocsPaWR.DestinatarioEsterno dest in destinatari)
            {
                if (dest.DatiDestinatari[0].canalePref != null && (!string.IsNullOrEmpty(dest.DatiDestinatari[0].canalePref.descrizione)) &&
                            ((dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("MAIL")) ||
                            (dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")) ||
                            (dest.DatiDestinatari[0].canalePref.descrizione.ToUpper().Equals("PORTALE"))))
                {
                    return true;
                }
            }
            return result;
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
             */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSpedizione"></param>
        protected void SetControlsVisibility(DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            this.SenderDivRegistri.Visible = (this.cboRegistriRF.Items.Count > 0);
            // se ci sono solo destinatari interop interna, abilito il pulsante di spedizione
            if (infoSpedizione.DestinatariEsterni.Count() == infoSpedizione.DestinatariEsterni.Count(d => d.DatiDestinatari[0].tipoIE != null && d.DatiDestinatari[0].tipoIE.Equals("I")))
                SenderBtnSend.Enabled = true;
        }

        /// <summary>
        /// Verifica se la versione corrente del documento è stata acquista o meno
        /// </summary>
        protected bool IsDocumentoAcquisito
        {
            get
            {
                bool retValue = false;

                DocsPaWR.SchedaDocumento documento = DocumentManager.getSelectedRecord();

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
        protected void FetchData()
        {
            this.FetchData(this.GetSpedizioneDocumento());
        }

        /// <summary>
        /// Caricamento dati delle spedizioni effettuate per il documento
        /// </summary>
        /// <param name="infoSpedizione"></param>
        protected void FetchData(DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            // Carico la lista dei registri e degli RF per non caricarla per ogni destinatario esterno
            NttDataWA.DocsPaWR.Registro[] rf = RegistryManager.getListaRegistriWithRF("1", RegistryManager.GetRegistryInSession().systemId);
            NttDataWA.DocsPaWR.Registro[] registri = RegistryManager.getListaRegistriWithRF("0", RegistryManager.GetRegistryInSession().systemId);
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            NttDataWA.DocsPaWR.StatoInvio[] listaSped_opt= ws.GetListaSpedizioni(DocumentManager.getSelectedRecord().systemId);
               
            // Caricamento dati nei singoli usercontrol
            this.listaDestinatariInterni.FetchData(infoSpedizione, CorrespondentTypeEnum.Interno, rf, registri, "INTERNO", listaSped_opt);
            this.listaDestinatariInteroperanti.FetchData(infoSpedizione, CorrespondentTypeEnum.Esterno, rf, registri, "INTEROP", listaSped_opt);
            this.listaDestinatariNonInteroperanti.FetchData(infoSpedizione, CorrespondentTypeEnum.EsternoNonInteroperante, rf, registri, "NONINTEROP", listaSped_opt);
            this.listaDestinatatiInteropSempl.FetchData(infoSpedizione, CorrespondentTypeEnum.SimplifiedInteroperability, rf, registri, "IS", listaSped_opt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSpedizione"></param>
        protected void FetchRegistri(DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            // Se è presente almeno un destinatario interoperante, 
            // vengono caricati i registri e gli RF visibili dall'utente corrente
            if (infoSpedizione.DestinatariEsterni.Where(e => e.Interoperante).Count() > 0)
            {
                // Inserimento elemento vuoto
                this.cboRegistriRF.Items.Add(new ListItem(string.Empty, string.Empty));

                DocsPaWR.Registro[] registriRF = MultiBoxManager.GetRegisterEnabledSend().ToArray();

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

                if (!string.IsNullOrEmpty(this.cboRegistriRF.SelectedValue))
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
            string ricevutaPecType = SenderManager.getSetRicevutaPec(this.cboRegistriRF.SelectedValue, null, null, true, ddl_caselle.SelectedValue);

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
                    SenderLblTipoRicevuta.Visible = true;
                    cboTipoRicevutaPec.Visible = true;
                    this.cboTipoRicevutaPec.SelectedValue = ricevutaPecType;
                    break;
                default:
                    SenderLblTipoRicevuta.Visible = false;
                    cboTipoRicevutaPec.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSpedizione"></param>
        protected void Spedisci(DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            // Spedizione del documento ai destinatari selezionati
            DocsPaWR.SchedaDocumento scheda = DocumentManager.getSelectedRecord();


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
            if (scheda.spedizioneDocumento != null)
                scheda.spedizioneDocumento.tipoMessaggio = infoSpedizione.tipoMessaggio;
            else
                scheda.spedizioneDocumento = infoSpedizione;
            infoSpedizione = SenderManager.SpedisciDocumento(scheda, infoSpedizione);

            //Andrea
            foreach (string s in infoSpedizione.listaDestinatariNonRaggiungibili)
            {
                messError = messError + s + Environment.NewLine;
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

            this.SetReturnValue(true);

            InitializesForm(infoSpedizione, scheda.tipoProto);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cboRegistriRF_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Spedisci()
        {
            DocsPaWR.SpedizioneDocumento infoSpedizione = this.GetSpedizioneDocumento();

            this.Spedisci(infoSpedizione);

            this.SetReturnValue(true);

            //listaDestinatariInterni.reloadContentText();

        }

        /// <summary>
        /// Impostazione messaggio di errore
        /// </summary>
        /// <param name="errorMessage"></param>
        protected void ShowErrorMessage(string errorMessage)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + errorMessage.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + errorMessage.Replace("'", @"\'") + "', 'warning', '');}", true);
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
            DocumentManager.getSelectedRecord().spedizioneDocumento = infoSpedizione;
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
        /// <returns></returns>
        public DocsPaWR.SpedizioneDocumento GetSpedizioneDocumento()
        {
            DocsPaWR.SpedizioneDocumento instance = this.ViewState["SpedizioniDocumento"] as DocsPaWR.SpedizioneDocumento;

            if (instance == null || !instance.IdDocumento.Equals(DocumentManager.getSelectedRecord().docNumber))
            {
                if (DocumentManager.getSelectedRecord().spedizioneDocumento == null)
                    instance = SenderManager.GetSpedizioneDocumento(DocumentManager.getSelectedRecord());
                else
                    instance = DocumentManager.getSelectedRecord().spedizioneDocumento;

                this.ViewState["SpedizioniDocumento"] = instance;
            }

            return instance;
        }

        /// <summary>
        /// Aggiorna la drop down list delle caselle abilitate alla spedizione associate al registro selezionato
        /// </summary>
        private void SetCaselleRegistro(string selectMain)
        {
            //se è stato selezionato un registro/rf nella ddl dei registri mittente, allora setto la ddl delle caselle associate al registro
            if (!string.IsNullOrEmpty(cboRegistriRF.SelectedValue))
            {
                List<DocsPaWR.CasellaRegistro> listCaselle = new List<DocsPaWR.CasellaRegistro>();
                listCaselle = MultiBoxManager.GetComboRegisterSend(cboRegistriRF.SelectedValue);
                foreach (DocsPaWR.CasellaRegistro c in listCaselle)
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
                if (selectMain.Equals("1"))
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

        protected void ddl_caselle_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                SetRicevutaPecCombo();
                DocsPaWR.SpedizioneDocumento infoSpedizione = this.GetSpedizioneDocumento();
                infoSpedizione.mailAddress = ddl_caselle.SelectedValue;
                this.SetSpedizioneDocumento(infoSpedizione);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

         /// <summary>
        /// Initializes application labels
        /// </summary>
        private void InitializesForm(DocsPaWR.SpedizioneDocumento infoSpedizione,string tipoProto)
        {

            Utils.Languages.InitializesLanguages();
            string currentLanguage = (string.IsNullOrEmpty(UIManager.UserManager.GetUserLanguage())?"Italian":UIManager.UserManager.GetUserLanguage());

            this.listaDestinatariInterni.setLanguage(currentLanguage);
            this.listaDestinatariInteroperanti.setLanguage(currentLanguage);
            this.listaDestinatatiInteropSempl.setLanguage(currentLanguage);
            this.listaDestinatariNonInteroperanti.setLanguage(currentLanguage);

            cboTipoRicevutaPec.Items[0].Text = UIManager.LoginManager.GetLabelFromCode("SendercboTipoRicevutaPec_0", currentLanguage);
            cboTipoRicevutaPec.Items[1].Text = UIManager.LoginManager.GetLabelFromCode("SendercboTipoRicevutaPec_1", currentLanguage);
            cboTipoRicevutaPec.Items[2].Text = UIManager.LoginManager.GetLabelFromCode("SendercboTipoRicevutaPec_2", currentLanguage);

            this.SenderLblInternalRecipients.Text = UIManager.LoginManager.GetLabelFromCode("SenderLblInternalRecipients", currentLanguage) + " " + (this.listaDestinatariInterni.Items.ToString());
            this.SenderLblInteroperableRecipientsPEC.Text = UIManager.LoginManager.GetLabelFromCode("SenderLblInteroperableRecipientsPEC", currentLanguage) + " " + (this.listaDestinatariInteroperanti.Items.ToString());
            this.SenderLblInteroperableRecipientsPITRE.Text = UIManager.LoginManager.GetLabelFromCode("SenderLblInteroperableRecipientsPITRE", currentLanguage) + " " + SimplifiedInteroperabilityManager.SearchItemDescriprion + ": " + (this.listaDestinatatiInteropSempl.Items.ToString());
            this.SenderLblExternalNotInteroperableRecipients.Text = UIManager.LoginManager.GetLabelFromCode("SenderLblExternalNotInteroperableRecipients", currentLanguage) + " " + (this.listaDestinatariNonInteroperanti.Items.ToString());
            this.SelectNextMessage.Title = UIManager.LoginManager.GetLabelFromCode("SelectNextMessageTitle", currentLanguage);
            this.SenderLblRegistriRF.Text = UIManager.LoginManager.GetLabelFromCode("SenderLblRegistriRF", currentLanguage);
            this.SenderLblCaselle.Text = UIManager.LoginManager.GetLabelFromCode("SenderLblCaselle", currentLanguage);
            this.SenderLblTipoRicevuta.Text = UIManager.LoginManager.GetLabelFromCode("SenderLblTipoRicevuta", currentLanguage);

            this.SenderImgObjectHistory.ToolTip = UIManager.LoginManager.GetLabelFromCode("SenderImgObjectHistoryToolTip", currentLanguage);

            string sendText = (tipoProto.ToUpper().Equals("I")?UIManager.LoginManager.GetLabelFromCode("SenderBtnTrasmit", currentLanguage):UIManager.LoginManager.GetLabelFromCode("SenderBtnSend", currentLanguage));
            string resendText = (tipoProto.ToUpper().Equals("I")?UIManager.LoginManager.GetLabelFromCode("SenderBtnRetrasmit", currentLanguage):UIManager.LoginManager.GetLabelFromCode("SenderBtnResend", currentLanguage));

            if (tipoProto.ToUpper().Equals("I")) SenderImgObjectHistory.Visible = false;
            
            if (infoSpedizione.Spedito)
            {
                this.SenderBtnSend.Text = resendText;
            }
            else
            {
                this.SenderBtnSend.Text = sendText;
            }

            // Impostazione dello stato della spedizione nei dettagli
            this.listaDestinatariInterni.SetStatoSpedizione(infoSpedizione.Spedito);
            if (this.MaxFileSizeSend > 0 && FileManager.TolatFileSizeDocument(DocumentManager.getSelectedRecord().docNumber) > this.MaxFileSizeSend)
            {
                this.listaDestinatariInteroperanti.DisableStatoSpedizione();
            }
            else
            {
                this.listaDestinatariInteroperanti.SetStatoSpedizione(infoSpedizione.Spedito);
            }
            this.listaDestinatatiInteropSempl.SetStatoSpedizione(infoSpedizione.Spedito);
            this.listaDestinatariNonInteroperanti.SetStatoSpedizione(infoSpedizione.Spedito);

            this.SenderBtnClose.Text = UIManager.LoginManager.GetLabelFromCode("SenderBtnClose", currentLanguage);

            /*
            Control clientControl = Page.FindControl("ContentPlaceHolderContent_pnelDestinatariNonInteroperanti");
            string strValue = Page.Request.Form["ContentPlaceHolderContent_pnelDestinatariNonInteroperanti"].ToString();
            */
            this.SenderInterni.Attributes["class"] = (listaDestinatariInterni.Items > 0 ? "expand" : "noexpand");
            this.SenderInteroperablePEC.Attributes["class"] = (listaDestinatariInteroperanti.Items > 0 ? "expand" : "noexpand");
            this.SenderPITRE.Attributes["class"] = (listaDestinatatiInteropSempl.Items > 0 ? "expand" : "noexpand");
            this.SenderNoInterop.Attributes["class"] = (listaDestinatariNonInteroperanti.Items > 0 ? "expand" : "noexpand");

            this.pnelDestinatariInterni.Attributes["class"] = (listaDestinatariInterni.Items > 0 ? "collapse collapsed" : "locked");
            this.pnelDestinatariInteroperanti.Attributes["class"] = (listaDestinatariInteroperanti.Items > 0 ? "collapse collapsed" : "locked");
            this.pnelDestinatatiInteropSempl.Attributes["class"] = (listaDestinatatiInteropSempl.Items > 0 ? "collapse collapsed" : "locked");
            this.pnelDestinatariNonInteroperanti.Attributes["class"] = (listaDestinatariNonInteroperanti.Items > 0 ? "collapse collapsed" : "locked");

            this.listaDestinatariInterni.Visible = (listaDestinatariInterni.Items > 0);
            this.listaDestinatariInteroperanti.Visible = (listaDestinatariInteroperanti.Items > 0);
            this.listaDestinatatiInteropSempl.Visible = (listaDestinatatiInteropSempl.Items > 0);
            this.listaDestinatariNonInteroperanti.Visible = (listaDestinatariNonInteroperanti.Items > 0);

            if (tipoProto.ToUpper().Equals("I"))
            {
                this.SenderNoInterop.Visible = false;
            }

            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.INTEROP_SERVICE_ACTIVE.ToString())) ||
               Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.INTEROP_SERVICE_ACTIVE.ToString()).Equals("0"))
            {
                pnelDestinatatiInteropSempl.Visible = false;
                SenderLblInteroperableRecipientsPITRE.Visible = false;
            }


            this.SenderObjHistory.Title = Utils.Languages.GetLabelFromCode("SenderImgObjectHistoryToolTip", currentLanguage);
        }

        #region Page properties
        /// <summary>
        /// Selected language
        /// </summary>
        protected string SelectedLanguage
        {
            get;
            set;
        }

        #endregion

        protected void SenderBtnSend_Click(object sender, EventArgs e)
        {
            try {
                HttpContext.Current.Session.Remove("DestinatariNonRaggiunti");
                if (cboTipoRicevutaPec.Visible == true)
                    SenderManager.getSetRicevutaPec(cboRegistriRF.SelectedValue, null, cboTipoRicevutaPec.SelectedValue, false, ddl_caselle.SelectedValue);
                try
                {

                    DocsPaWR.ConfigSpedizioneDocumento config = SenderManager.GetConfigSpedizioneDocumento();
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
                        this.ShowErrorMessage("WarningSenderRecipients");
                    }
                    /*else if (!this.IsDocumentoAcquisito && config.AvvisaSuSpedizioneDocumento)
                    {
                        this.msgSpedisci.Confirm("E' stata richiesta la spedizione senza aver associato alcun documento elettronico.\\nSi vuole eseguire le operazioni di trasmissione e spedizione automatiche?");
                    }*/ //Gestito con nuovo messaggio
                    else
                    {
                        bool destInteropRGSSelected = (from d in infoSpedizione.DestinatariEsterni where d.InteroperanteRGS && d.IncludiInSpedizione select d).FirstOrDefault() != null;

                        DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getSelectedRecord();
                        if (destInteropRGSSelected && schedaDoc.template != null && !string.IsNullOrEmpty(schedaDoc.template.ID_CONTESTO_PROCEDURALE))
                        {
                            List<DocsPaWR.Messaggio> messaggiSuccessivi = SenderManager.GetMessaggiSuccessiviFlussoProcedurale(schedaDoc);

                            //se sono presenti più di uno messaggio lascio scegliere all'utente.
                            if (messaggiSuccessivi != null && messaggiSuccessivi.Count > 1)
                            {
                                this.NextMessages = messaggiSuccessivi;
                                this.InfoSpedizioneSelectMessage = infoSpedizione;
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "SelectNextMessage", "ajaxModalPopupSelectNextMessage();", true);
                                return;
                            }
                            if (messaggiSuccessivi != null && messaggiSuccessivi.Count == 1)
                            {
                                infoSpedizione.tipoMessaggio = messaggiSuccessivi[0];
                            }
                        }
                        else
                        {
                            infoSpedizione.tipoMessaggio = null;
                        }
                        this.Spedisci(infoSpedizione);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (Session["MessError"] != null)
                {
                    messError = Session["MessError"].ToString();
                
                    string msgDesc = "WarningDocumentCustom";
                    string errFormt = Server.UrlEncode(messError);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);

                    Session.Remove("MessError");
                    messError = string.Empty;
                }

                if (listaDestinatariInterni.Items > 0) UpdateDestinatariInterni.Update();
                if (listaDestinatariInteroperanti.Items > 0) UpdateDestinatariInteroperanti.Update();
                if (listaDestinatatiInteropSempl.Items > 0) UpdateDestinatatiInteropSempl.Update();
                if (listaDestinatariNonInteroperanti.Items > 0) UpdateDestinatariNonInteroperanti.Update();

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                NttDataWA.DocsPaWR.SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
                //aggiorno l'elenco allegati in sessione(per ricevute PITRE)
                if (documentTab != null && !string.IsNullOrEmpty(documentTab.docNumber))
                {
                    DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this.Page, documentTab.docNumber, documentTab.docNumber));
                }

                if (!string.IsNullOrEmpty(this.DestinatariNonRaggiunti))
                {
                    string msgDesc = "WarningSendingRecipients";
                    string errFormt = this.DestinatariNonRaggiunti;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);
                    HttpContext.Current.Session.Remove("DestinatariNonRaggiunti");
                }

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SenderBtnClose_Click(object sender, EventArgs e)
        {
            this.closeAll = true;
            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('senderpopup','up');</script></body></html>");
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "closePopup", "parent.closeAjaxModal('senderpopup');", true);
            Response.End();
        }

        protected void SenderImgObjectHistory_Click(object sender, ImageClickEventArgs e)
        {
            // PEC 4 - requisito 5 - storico spedizioni
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "SenderObjHistory", "ajaxModalPopupSenderObjHistory();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        //protected void Page_PreRender(object sender, EventArgs e)
        //{
        //    if (this.Session["showConfirmSpedizioneAutomatica"] != null)
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "messError",
        //            "<script language=\"javascript\">alert('E\\' stata richiesta la spedizione senza aver associato il documento principale. \\nSi vuole procedere comunque (SCELTA SCONSIGLIATA)?');</script>", false);
        //}
    }
}