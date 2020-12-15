using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using System.Configuration;
using NttDataWA.Utils;
using log4net;
using NttDataWA.UIManager;
using System.Collections;

namespace NttDataWA.Popup
{
    public partial class ViewDetailTransmission : System.Web.UI.Page
    {
        #region Property

        private bool ShowsUserLocation
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["ShowsUserLocation"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["ShowsUserLocation"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ShowsUserLocation"] = value;
            }
        }

        private Notification Notification
        {
            get
            {
                if (HttpContext.Current.Session["Notification"] != null)
                    return (Notification)HttpContext.Current.Session["Notification"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["Notification"] = value;
            }
        }

        private Trasmissione Transmission
        {
            get
            {
                if (HttpContext.Current.Session["Transmission"] != null)
                    return (Trasmissione)HttpContext.Current.Session["Transmission"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["Transmission"] = value;
            }
        }

        private bool EnabledLibroFirma
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["EnabledLibroFirma"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["EnabledLibroFirma"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["EnabledLibroFirma"] = value;
            }
        }

        private TrasmissioneSingola TrasmToElaborate
        {
            get
            {
                if (HttpContext.Current.Session["TrasmToElaborate"] != null)
                    return (TrasmissioneSingola)HttpContext.Current.Session["TrasmToElaborate"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["TrasmToElaborate"] = value;
            }
        }
        #endregion

        #region Const

        private const string LINE = "<line>";
        private const string LINE_C = "</line>";

        #endregion

        #region Standard Method

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.ClearSession();
                this.LoadKeys();
                this.InitializeLanguage();
                this.BuildNotification();
                this.InitializePage();
            }
            else
            {
                if (!string.IsNullOrEmpty(this.final_state.Value))
                {
                    this.ChangeState();
                    this.final_state.Value = string.Empty;
                }
            }
            RefreshScript();
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            this.SetAjaxDescriptionProject();
        }

        private void ClearSession()
        {
            DocumentManager.setSelectedRecord(null);
            ProjectManager.setProjectInSession(null);
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_BLOCCA_CLASS.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_BLOCCA_CLASS.ToString()).Equals("1"))
            {
                this.EnableBlockClassification = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, DBKeys.FE_SEDE_TRASM.ToString())) && (Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, DBKeys.FE_SEDE_TRASM.ToString()).Equals("1")))
            {
                this.ShowsUserLocation = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("1"))
            {
                this.EnabledLibroFirma = true;
            }
        }

        private void InitializePage()
        {
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.TransmissionLitDetailsRecipient.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDetailsRecipient", language);
            this.TransmissionNoteAccRej.Text = Utils.Languages.GetLabelFromCode("TransmissionNoteAccRej", language);
            this.BtnAccept.Text = Utils.Languages.GetLabelFromCode("ViewDetailNotifyBtnAccept", language);
            this.BtnAcceptAdLU.Text = Utils.Languages.GetLabelFromCode("ViewDetailNotifyBtnAcceptAdLU", language);
            this.BtnAcceptAdLR.Text = Utils.Languages.GetLabelFromCode("ViewDetailNotifyBtnAcceptAdLR", language);
            this.BtnReject.Text = Utils.Languages.GetLabelFromCode("ViewDetailNotifyBtnReject", language);
            this.BtnView.Text = Utils.Languages.GetLabelFromCode("ViewDetailNotifyBtnView", language);
            this.BtnViewAdLU.Text = Utils.Languages.GetLabelFromCode("ViewDetailNotifyBtnViewAdLU", language);
            this.BtnViewAdLR.Text = Utils.Languages.GetLabelFromCode("ViewDetailNotifyBtnViewAdLR", language);
            this.BtnCancel.Text = Utils.Languages.GetLabelFromCode("ViewDetailNotifyBtnCancel", language);
            this.BtnAcceptLF.Text = Languages.GetLabelFromCode("ViewDetailNotifyBtnAcceptLF", language);
            this.BtnAcceptLF.ToolTip = Languages.GetLabelFromCode("ViewDetailNotifyBtnAcceptLFTooltip", language);
            this.DocumentLitClassificationRapidTrasm.Text = Languages.GetLabelFromCode("DocumentLitClassificationRapidTrasm", language);
            this.DocumentImgSearchProjects.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgSearchProjects", language);
            this.DocumentImgSearchProjects.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgSearchProjects", language);
            this.btnclassificationschema.AlternateText = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.btnclassificationschema.ToolTip = Utils.Languages.GetLabelFromCode("btnclassificationschema", language);
            this.ImgAddProjects.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgAddProjects", language);
        }

        #endregion

        #region Details notification

        /// <summary>
        /// 
        /// </summary>
        private void BuildNotification()
        {
            string tipoOggetto = TrasmManager.getSelectedTransmission().tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO ? "F" : "D";
            this.Transmission = TrasmManager.GetTransmission(this, TrasmManager.getSelectedTransmission().systemId, tipoOggetto);
            TrasmManager.setSelectedTransmission(this.Transmission);

            TrasmissioneSingola[] listTrasmSing = Transmission.trasmissioniSingole;

            if (listTrasmSing != null && listTrasmSing.Length > 0 && !string.IsNullOrEmpty(Transmission.dataInvio))
            {
                TrasmissioneUtente trasmUtente;
                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                //Vedo se presente la trasmissione a ruolo
                List<DocsPaWR.TrasmissioneSingola> list = new List<TrasmissioneSingola>(Transmission.trasmissioniSingole);

                //Emanuela: modifica per accettazione di trasmissioni a ruolo ed utente, seleziono per defaul prima quella di tipo ruolo per far si
                //che funzioni la gestione di accettazioni di entrambi le trasm
                List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.RUOLO).ToList();
                TrasmToElaborate = TrasmManager.RoleTransmissionWithHistoricized(trasmSingoleUtente, infoUtente.idCorrGlobali);
                bool trasmRoleWorked = true;

                if (TrasmToElaborate != null)
                {
                    trasmUtente = (DocsPaWR.TrasmissioneUtente)TrasmToElaborate.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();
                    if (trasmUtente != null && TrasmToElaborate.ragione.tipo == "W" && string.IsNullOrEmpty(trasmUtente.dataRifiutata) && string.IsNullOrEmpty(trasmUtente.dataAccettata))
                    {
                        trasmRoleWorked = !TrasmManager.checkTrasm_UNO_TUTTI_AccettataRifiutata(TrasmToElaborate);
                    }
                    if (trasmUtente != null && TrasmToElaborate.ragione.tipo != "W" && TrasmManager.getIfDocOrFascIsInToDoList(infoUtente, trasmUtente.systemId))
                    {
                        trasmRoleWorked = false;
                    }
                }

                if (TrasmToElaborate == null || trasmRoleWorked)
                {
                    // Se non è stata trovata la trasmissione come destinatario a ruolo, 
                    // cerca quella con destinatario l'utente corrente

                    trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.UTENTE).ToList();
                    if (trasmSingoleUtente != null)
                    {
                        DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);
                        TrasmissioneSingola s = trasmSingoleUtente.Where(e => ((DocsPaWR.Utente)e.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();
                        if (s != null)
                            TrasmToElaborate = s;
                    }
                }
            }

            this.ShowTransmission();

            this.ShowDetailsNotification();

            this.EnableButtonsNotification();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ShowTransmission()
        {
            if (Transmission != null)
            {
                TrasmissioneSingola[] listTrasmSing = Transmission.trasmissioniSingole;
                if (listTrasmSing != null)
                {
                    for (int i = 0; i < listTrasmSing.Length; i++)
                    {
                        DocsPaWR.TrasmissioneSingola trasmSing = (DocsPaWR.TrasmissioneSingola)listTrasmSing[i];
                        this.BuildSingleTransmissionsTables(Transmission, trasmSing);
                    }
                }
            }

            this.containerDetailTrasmission.Visible = true;
            //this.UpPnlTransmission.Update();
        }

        private void ShowDetailsNotification()
        {
            if (this.Transmission.infoDocumento != null && !string.IsNullOrEmpty(this.Transmission.infoDocumento.docNumber))
            {
                if (!string.IsNullOrEmpty(this.Transmission.infoDocumento.segnatura))
                {
                    this.TableCellTxtSignatureDoc.Text = this.Transmission.infoDocumento.segnatura;
                }
                else
                {
                    this.TableCellTxtSignatureDoc.Text = this.Transmission.infoDocumento.docNumber;
                }
                this.TableCellTxtObjectDoc.Text = this.Transmission.infoDocumento.oggetto;
                this.TableCellTxtNoteGenerali.Text = this.Transmission.noteGenerali;
            }
            else
            {
                if (!string.IsNullOrEmpty(this.Transmission.infoFascicolo.codice))
                {
                    this.TableCellTxtSignatureDoc.Text = this.Transmission.infoFascicolo.codice;
                    this.TableCellTxtObjectDoc.Text = this.Transmission.infoFascicolo.descrizione;
                }
            }

            TrasmissioneUtente trasmU = (this.Transmission != null && this.Transmission.trasmissioniSingole != null && this.Transmission.trasmissioniSingole.Length > 0) ? (from t in Transmission.trasmissioniSingole
                                                                                                                                                                            where t.corrispondenteInterno.descrizione.Equals(UserManager.GetUserInSession().descrizione)
                                                                                                                                                                            select t.trasmissioneUtente[0]).FirstOrDefault() :
                                            (from t in Transmission.trasmissioniSingole
                                             where t.corrispondenteInterno.descrizione.Equals(RoleManager.GetRoleInSession().descrizione)
                                             select
                                                 (from trasmUser in t.trasmissioneUtente
                                                  where trasmUser.utente.idPeople.Equals(UserManager.GetInfoUser().idPeople)
                                                  select trasmUser).FirstOrDefault()).FirstOrDefault();

            if (trasmU != null)
            {
                this.TableCellDispayed.Text = trasmU.dataVista;
                this.TableCellReplied.Text = trasmU.dataRisposta;
                this.TableCellAccepted.Text = trasmU.dataAccettata;
                this.TableCellRejected.Text = trasmU.dataRifiutata;
                this.TableCellNoteAccRej.Text = trasmU.noteAccettazione + trasmU.noteRifiuto;
                this.rowRoleUserNotification.Visible = false;
                this.rowTypeEvent.Visible = false;
            }

        }

        //protected string GetLabelField()
        //{
        //    Items item = Notification.ITEMS;
        //    string result = string.Empty;
        //    string label = string.Empty;
        //    if (!string.IsNullOrEmpty(item.ITEM1))
        //    {
        //        result = NotificationManager.getLabel(item.ITEM1);
        //    }
        //    if (!string.IsNullOrEmpty(item.ITEM2))
        //    {
        //        label = NotificationManager.getLabel(item.ITEM2);
        //        if (!string.IsNullOrEmpty(label))
        //            result = string.IsNullOrEmpty(result) ? label : result + " - " + label;
        //    }
        //    if (!string.IsNullOrEmpty(item.ITEM4))
        //    {
        //        label = NotificationManager.getLabel(item.ITEM4);
        //        if (!string.IsNullOrEmpty(label))
        //            result = string.IsNullOrEmpty(result) ? label : result + " - " + label;
        //    }
        //    return result;
        //}

        //protected string GetLabelSpecializedField()
        //{
        //    string result = string.Empty;
        //    if (!string.IsNullOrEmpty(Notification.ITEM_SPECIALIZED))
        //    {
        //        string[] splitL = Notification.ITEM_SPECIALIZED.Split(new string[] { LINE }, StringSplitOptions.None);
        //        foreach (string s in splitL)
        //        {
        //            result += NotificationManager.getLabel(s);
        //        }
        //        result = result.Replace(LINE_C, "<br />").Replace(LINE, "");
        //    }
        //    string dtaNotify = Utils.Languages.GetLabelFromCode("lblDta_notify", UIManager.UserManager.GetUserLanguage()) + " " + Notification.DTA_NOTIFY + "</br>";
        //    return dtaNotify + result;
        //}

        #endregion

        #region Transmission

        private void BuildSingleTransmissionsTables(Trasmissione trasm, TrasmissioneSingola trasmSing)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            Table table = this.GetTransmissionTable(trasmSing);

            // DRAW TABLE
            ((TableCell)GetControlById(table, "trasmDetailsRecipient" + trasmSing.systemId)).Text = "<strong>" + formatBlankValue(trasmSing.corrispondenteInterno.descrizione) + "</strong>";
            ((TableCell)GetControlById(table, "trasmDetailsReason" + trasmSing.systemId)).Text = formatBlankValue(trasmSing.ragione.descrizione);

            switch (trasmSing.tipoTrasm)
            {
                case "T":
                    ((TableCell)GetControlById(table, "trasmDetailsType" + trasmSing.systemId)).Text = Utils.Languages.GetLabelFromCode("TransmissionDdlTypeMulti", language).ToUpper();
                    break;
                case "S":
                    ((TableCell)GetControlById(table, "trasmDetailsType" + trasmSing.systemId)).Text = Utils.Languages.GetLabelFromCode("TransmissionDdlTypeSingle", language).ToUpper();
                    break;
                default:
                    ((TableCell)GetControlById(table, "trasmDetailsType" + trasmSing.systemId)).Text = formatBlankValue(null);
                    break;
            }

            if (this.IsSingleNotesVisible(trasm, trasmSing))
            {
                ((TableCell)GetControlById(table, "trasmDetailsNote" + trasmSing.systemId)).Text = this.formatBlankValue(trasmSing.noteSingole);
            }
            else
            {
                ((TableCell)GetControlById(table, "trasmDetailsNote" + trasmSing.systemId)).Text = new string('-', 15);
            }

            ((TableCell)GetControlById(table, "trasmDetailsExpire" + trasmSing.systemId)).Text = this.formatBlankValue(trasmSing.dataScadenza);


            // DRAW USERS
            if (trasmSing.trasmissioneUtente != null)
            {
                for (int i = 0; i < trasmSing.trasmissioneUtente.Length; i++)
                {
                    DocsPaWR.TrasmissioneUtente TrasmUt = (DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente[i];
                    TableRow row = this.GetDetailsRow(TrasmUt.systemId);

                    string userDetails = TrasmUt.utente.descrizione;
                    if (this.ShowsUserLocation && !string.IsNullOrEmpty(TrasmUt.utente.sede)) userDetails += " (" + TrasmUt.utente.sede + ")";
                    ((TableCell)GetControlById(row, "trasmDetailsUser" + TrasmUt.systemId)).Text = this.formatBlankValue(userDetails);

                    if (!string.IsNullOrEmpty(TrasmUt.dataVista) && (string.IsNullOrEmpty(TrasmUt.cha_vista_delegato) || TrasmUt.cha_vista_delegato.Equals("0")))
                    {
                        ((TableCell)GetControlById(row, "trasmDetailsViewed" + TrasmUt.systemId)).Text = this.formatBlankValue(TrasmUt.dataVista);
                    }
                    else
                    {
                        ((TableCell)GetControlById(row, "trasmDetailsViewed" + TrasmUt.systemId)).Text = this.formatBlankValue(null);
                    }

                    if (!string.IsNullOrEmpty(TrasmUt.dataAccettata) && (string.IsNullOrEmpty(TrasmUt.cha_accettata_delegato) || TrasmUt.cha_accettata_delegato.Equals("0")))
                    {
                        ((TableCell)GetControlById(row, "trasmDetailsAccepted" + TrasmUt.systemId)).Text = this.formatBlankValue(TrasmUt.dataAccettata);
                    }
                    else
                    {
                        ((TableCell)GetControlById(row, "trasmDetailsAccepted" + TrasmUt.systemId)).Text = this.formatBlankValue(null);
                    }

                    if (!string.IsNullOrEmpty(TrasmUt.dataRifiutata) && (string.IsNullOrEmpty(TrasmUt.cha_rifiutata_delegato) || TrasmUt.cha_rifiutata_delegato.Equals("0")))
                    {
                        ((TableCell)GetControlById(row, "trasmDetailsRif" + TrasmUt.systemId)).Text = this.formatBlankValue(TrasmUt.dataRifiutata);
                    }
                    else
                    {
                        ((TableCell)GetControlById(row, "trasmDetailsRif" + TrasmUt.systemId)).Text = this.formatBlankValue(null);
                    }

                    if (!string.IsNullOrEmpty(TrasmUt.dataRimossaTDL) && (string.IsNullOrEmpty(TrasmUt.cha_rimossa_delegato) || TrasmUt.cha_rimossa_delegato.Equals("0")))
                    {
                        ((TableCell)GetControlById(row, "trasmDetailsRemoved" + TrasmUt.systemId)).Text = this.formatBlankValue(TrasmUt.dataRimossaTDL);
                    }
                    else
                    {
                        ((TableCell)GetControlById(row, "trasmDetailsRemoved" + TrasmUt.systemId)).Text = this.formatBlankValue(null);
                    }

                    // viene verificato se l'utente corrente
                    // può visualizzare i dettagli (note rifiuto e note accettazione)
                    // della trasmissione singola:
                    // - ha pieni diritti di visualizzazione
                    //   se è l'utente che ha creato la trasmissione;
                    // - altrimenti viene verificato se l'utente corrente è lo stesso
                    //   che ha ricevuto la trasmissione (e quindi l'ha accettata)
                    if (CheckTrasmEffettuataDaUtenteCorrente(trasm) || CheckTrasmUtenteCorrente(TrasmUt))
                    {
                        if (!string.IsNullOrEmpty(TrasmUt.dataAccettata) && (string.IsNullOrEmpty(TrasmUt.cha_accettata_delegato) || TrasmUt.cha_accettata_delegato.Equals("0")))
                        {
                            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString())) &&
                                Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString()).Equals("1")
                                && TrasmUt.noteAccettazione.Contains(TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA))
                            {
                                ((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Controls.Add(BuildNoteTask(TrasmUt.noteAccettazione));
                            }
                            else
                            {
                                ((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Text = formatBlankValue(TrasmUt.noteAccettazione);
                            }
                        }
                        else
                            if (!string.IsNullOrEmpty(TrasmUt.dataRifiutata) && (string.IsNullOrEmpty(TrasmUt.cha_rifiutata_delegato) || TrasmUt.cha_rifiutata_delegato.Equals("0")))
                            {
                                ((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Text = formatBlankValue(TrasmUt.noteRifiuto);
                            }
                            else
                            {
                                ((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Text = formatBlankValue(null);
                            }
                    }
                    else
                    {
                        ((TableCell)GetControlById(row, "trasmDetailsInfo" + TrasmUt.systemId)).Text = new string('-', 15);
                    }

                    table.Controls.Add(row);


                    // RIGA IN CASO DI DELEGA
                    if (!string.IsNullOrEmpty(TrasmUt.idPeopleDelegato) && (TrasmUt.cha_accettata_delegato == "1" || TrasmUt.cha_rifiutata_delegato == "1" || TrasmUt.cha_vista_delegato == "1"  || TrasmUt.cha_rimossa_delegato == "1") && (!string.IsNullOrEmpty(TrasmUt.dataVista) || !string.IsNullOrEmpty(TrasmUt.dataAccettata) || !string.IsNullOrEmpty(TrasmUt.dataRifiutata) || !string.IsNullOrEmpty(TrasmUt.dataRimossaTDL)))
                    {
                        string del = Utils.Languages.GetLabelFromCode("TransmissionDelegatedBy", language);
                        TableRow row2 = GetDetailsRow(TrasmUt.idPeopleDelegato);

                        ((TableCell)GetControlById(row2, "trasmDetailsUser" + TrasmUt.idPeopleDelegato)).Text = formatBlankValue(TrasmUt.idPeopleDelegato + "<br>(" + del + " " + TrasmUt.utente.descrizione + ")");

                        if (!string.IsNullOrEmpty(TrasmUt.dataVista) && TrasmUt.cha_vista_delegato.Equals("1"))
                        {
                            ((TableCell)GetControlById(row2, "trasmDetailsViewed" + TrasmUt.idPeopleDelegato)).Text = formatBlankValue(TrasmUt.dataVista);
                        }
                        else
                        {
                            ((TableCell)GetControlById(row2, "trasmDetailsViewed" + TrasmUt.idPeopleDelegato)).Text = formatBlankValue(null);
                        }

                        if (TrasmUt.cha_accettata_delegato.Equals("1"))
                        {
                            ((TableCell)GetControlById(row2, "trasmDetailsAccepted" + TrasmUt.idPeopleDelegato)).Text = formatBlankValue(TrasmUt.dataAccettata);
                        }
                        else
                        {
                            ((TableCell)GetControlById(row2, "trasmDetailsAccepted" + TrasmUt.idPeopleDelegato)).Text = formatBlankValue(null);
                        }

                        if (TrasmUt.cha_rifiutata_delegato.Equals("1"))
                        {
                            ((TableCell)GetControlById(row2, "trasmDetailsRif" + TrasmUt.idPeopleDelegato)).Text = formatBlankValue(TrasmUt.dataRifiutata);
                        }
                        else
                        {
                            ((TableCell)GetControlById(row2, "trasmDetailsRif" + TrasmUt.idPeopleDelegato)).Text = formatBlankValue(null);
                        }

                        if (TrasmUt.cha_rimossa_delegato.Equals("1"))
                        {
                            ((TableCell)GetControlById(row2, "trasmDetailsRemoved" + TrasmUt.idPeopleDelegato)).Text = this.formatBlankValue(TrasmUt.dataRimossaTDL);
                        }
                        else
                        {
                            ((TableCell)GetControlById(row2, "trasmDetailsRemoved" + TrasmUt.idPeopleDelegato)).Text = this.formatBlankValue(null);
                        }

                        if (CheckTrasmEffettuataDaUtenteCorrente(trasm) || CheckTrasmUtenteCorrente(TrasmUt))
                        {
                            if (!string.IsNullOrEmpty(TrasmUt.dataAccettata) && TrasmUt.cha_accettata_delegato.Equals("1"))
                            {

                                    if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString())) &&
                                        Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.ENABLE_TASK.ToString()).Equals("1")
                                        && TrasmUt.noteAccettazione.Contains(TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA))
                                    {
                                        ((TableCell)GetControlById(row2, "trasmDetailsInfo" + TrasmUt.idPeopleDelegato)).Controls.Add(BuildNoteTask(TrasmUt.noteAccettazione));
                                    }
                                    else
                                    {
                                        ((TableCell)GetControlById(row2, "trasmDetailsInfo" + TrasmUt.idPeopleDelegato)).Text = formatBlankValue(TrasmUt.noteAccettazione);
                                    }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(TrasmUt.dataRifiutata) && TrasmUt.cha_rifiutata_delegato.Equals("1"))
                                {
                                    ((TableCell)GetControlById(row2, "trasmDetailsInfo" + TrasmUt.idPeopleDelegato)).Text = formatBlankValue(TrasmUt.noteRifiuto);
                                }
                                else
                                {
                                    ((TableCell)GetControlById(row2, "trasmDetailsInfo" + TrasmUt.idPeopleDelegato)).Text = formatBlankValue(null);
                                }
                            }
                        }
                        else
                        {
                            ((TableCell)GetControlById(row2, "trasmDetailsInfo" + TrasmUt.idPeopleDelegato)).Text = new string('-', 15);
                        }

                        table.Controls.Add(row2);
                    }
                }
            }
            else
            {
                table.Controls.Remove(((TableRow)GetControlById(table, "row_users" + trasmSing.systemId)));
            }


            // ADD TABLE
            this.plcTransmissions.Controls.Add(table);
        }


        private System.Web.UI.HtmlControls.HtmlGenericControl BuildNoteTask(string noteAccettazione)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            noteAccettazione = noteAccettazione.Replace(TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA, Utils.Languages.GetLabelFromCode("TaskNote", language));
            noteAccettazione = noteAccettazione.Replace(TaskManager.TagIdContributo.LABEL_ATTIVITA_CONCLUSA_C, "");
            noteAccettazione = noteAccettazione.Replace(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO, Utils.Languages.GetLabelFromCode("TaskIdContributo", language)).Replace(TaskManager.TagIdContributo.LABEL_ID_CONTRIBUTO_C, "");
            noteAccettazione = noteAccettazione.Replace(TaskManager.TagIdContributo.LABEL_TEXT_WRAP, "<br /><br />");

            Label note = new Label();
            note.Text = noteAccettazione;

            Panel pnlContentNote = new Panel();
            pnlContentNote.ID = "pnlContentNote";
            pnlContentNote.Attributes.Add("class", "fieldNotesAcc");
            pnlContentNote.Controls.Add(note);

            System.Web.UI.HtmlControls.HtmlGenericControl divContent = new System.Web.UI.HtmlControls.HtmlGenericControl();
            divContent.Attributes.Add("class", "contentNoteTask");

            System.Web.UI.HtmlControls.HtmlGenericControl fieldset = new System.Web.UI.HtmlControls.HtmlGenericControl("fieldset");

            System.Web.UI.HtmlControls.HtmlGenericControl h2 = new System.Web.UI.HtmlControls.HtmlGenericControl("h2");
            h2.Attributes.Add("class", "expand");

            System.Web.UI.HtmlControls.HtmlGenericControl div2 = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            div2.ID = "divContentNoteAcc";
            div2.Attributes.Add("class", "collapse");

            h2.InnerHtml = Utils.Languages.GetLabelFromCode("TaskExtendNotes", language);
            //h2.Controls.Add(pnlContentNote1);
            div2.Controls.Add(pnlContentNote);
            fieldset.Controls.Add(h2);
            fieldset.Controls.Add(div2);
            divContent.Controls.Add(fieldset);
            return divContent;
        }

        private Control GetControlById(Control owner, string controlID)
        {
            Control myControl = null;
            // cycle all controls
            if (owner.Controls.Count > 0)
            {
                foreach (Control c in owner.Controls)
                {
                    myControl = GetControlById(c, controlID);
                    if (myControl != null) return myControl;
                }
            }
            if (controlID.Equals(owner.ID)) return owner;
            return null;
        }

        private TableRow GetDetailsRow(string id)
        {
            TableRow row = new TableRow();
            row.CssClass = "users";

            TableCell cellUser = new TableCell();
            cellUser.ID = "trasmDetailsUser" + id;
            cellUser.CssClass = "first";
            row.Cells.Add(cellUser);

            TableCell cellViewed = new TableCell();
            cellViewed.ID = "trasmDetailsViewed" + id;
            cellViewed.CssClass = "center";
            row.Cells.Add(cellViewed);

            TableCell cellAccepted = new TableCell();
            cellAccepted.ID = "trasmDetailsAccepted" + id;
            cellAccepted.CssClass = "center";
            row.Cells.Add(cellAccepted);

            TableCell cellRif = new TableCell();
            cellRif.ID = "trasmDetailsRif" + id;
            cellRif.CssClass = "center";
            row.Cells.Add(cellRif);

            TableCell cellRemoved = new TableCell();
            cellRemoved.ID = "trasmDetailsRemoved" + id;
            cellRemoved.CssClass = "center";
            row.Cells.Add(cellRemoved);

            TableCell cellInfo = new TableCell();
            cellInfo.ID = "trasmDetailsInfo" + id;
            cellInfo.ColumnSpan = 2;
            row.Cells.Add(cellInfo);

            return row;
        }

        private Table GetTransmissionTable(TrasmissioneSingola trasmSing)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            Table tbl = new Table();

            tbl.CssClass = "tbl_rounded2";
            {// header
                TableRow row = new TableRow();

                TableCell cell1 = new TableCell();
                cell1.ColumnSpan = 7;
                cell1.CssClass = (this.TrasmToElaborate != null && trasmSing.systemId.Equals(this.TrasmToElaborate.systemId)) ? "th header2" : "th header1";
                cell1.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDetailsRecipient", language);
                row.Controls.Add(cell1);

                tbl.Controls.Add(row);
            }

            {// recipient header
                TableRow row = new TableRow();
                row.CssClass = "header";

                TableCell cell1 = new TableCell();
                cell1.ColumnSpan = 2;
                cell1.CssClass = "first";
                cell1.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDescription", language);
                row.Controls.Add(cell1);

                TableCell cell2 = new TableCell();
                cell2.CssClass = "center trasmDetailReason";
                cell2.Text = Utils.Languages.GetLabelFromCode("TransmissionLitReason", language);
                row.Controls.Add(cell2);

                TableCell cell3 = new TableCell();
                cell3.CssClass = "center trasmDetailType";
                cell3.Text = Utils.Languages.GetLabelFromCode("TransmissionLitType", language);
                cell3.ColumnSpan = 2;
                row.Controls.Add(cell3);

                TableCell cell4 = new TableCell();
                cell4.CssClass = "trasmDetailNote";
                cell4.Text = Utils.Languages.GetLabelFromCode("TransmissionLitNoteShort", language);
                row.Controls.Add(cell4);

                TableCell cell5 = new TableCell();
                cell5.CssClass = "center trasmDetailDate";
                cell5.Text = Utils.Languages.GetLabelFromCode("TransmissionLitExpire", language);
                row.Controls.Add(cell5);

                tbl.Controls.Add(row);
            }

            {// recipient data
                TableRow row = new TableRow();

                TableCell cell1 = new TableCell();
                cell1.ID = "trasmDetailsRecipient" + trasmSing.systemId;
                cell1.ColumnSpan = 2;
                cell1.CssClass = "first";
                cell1.Text = Utils.Languages.GetLabelFromCode("TransmissionLitDetailsRecipient", language);
                row.Controls.Add(cell1);

                TableCell cell2 = new TableCell();
                cell2.ID = "trasmDetailsReason" + trasmSing.systemId;
                cell2.CssClass = "center";
                cell2.Text = Utils.Languages.GetLabelFromCode("TransmissionLitReason", language);
                row.Controls.Add(cell2);

                TableCell cell3 = new TableCell();
                cell3.ID = "trasmDetailsType" + trasmSing.systemId;
                cell3.CssClass = "center";
                cell3.ColumnSpan = 2;
                cell3.Text = Utils.Languages.GetLabelFromCode("TransmissionLitType", language);
                row.Controls.Add(cell3);

                TableCell cell4 = new TableCell();
                cell4.ID = "trasmDetailsNote" + trasmSing.systemId;
                cell4.Text = Utils.Languages.GetLabelFromCode("TransmissionLitNoteShort", language);
                row.Controls.Add(cell4);

                TableCell cell5 = new TableCell();
                cell5.ID = "trasmDetailsExpire" + trasmSing.systemId;
                cell5.CssClass = "center";
                cell5.Text = Utils.Languages.GetLabelFromCode("TransmissionLitExpire", language);
                row.Controls.Add(cell5);

                tbl.Controls.Add(row);
            }

            {// users header
                TableRow row = new TableRow();
                row.ID = "row_users" + trasmSing.systemId;
                row.CssClass = "header2";

                TableCell cell1 = new TableCell();
                cell1.CssClass = "first";
                cell1.Text = Utils.Languages.GetLabelFromCode("TransmissionLitUser", language);
                row.Controls.Add(cell1);

                TableCell cell2 = new TableCell();
                cell2.CssClass = "center trasmDetailDate";
                cell2.Text = Utils.Languages.GetLabelFromCode("TransmissionLitViewedOn", language);
                row.Controls.Add(cell2);

                TableCell cell3 = new TableCell();
                cell3.CssClass = "center trasmDetailDate";
                cell3.Text = Utils.Languages.GetLabelFromCode("TransmissionLitAcceptedOn", language);
                row.Controls.Add(cell3);

                TableCell cell4 = new TableCell();
                cell4.CssClass = "center trasmDetailDate";
                cell4.Text = Utils.Languages.GetLabelFromCode("TransmissionLitRejectedOn", language);
                row.Controls.Add(cell4);

                TableCell cell5 = new TableCell();
                cell5.CssClass = "center trasmDetailDate";
                cell5.Text = Utils.Languages.GetLabelFromCode("TransmissionLitRemoved", language);
                row.Controls.Add(cell5);

                TableCell cell6 = new TableCell();
                cell6.ColumnSpan = 2;
                cell6.CssClass = "center";
                cell6.Text = Utils.Languages.GetLabelFromCode("TransmissionLitInfoAccRej", language);
                row.Controls.Add(cell6);

                tbl.Controls.Add(row);
            }

            return tbl;
        }

        private string formatBlankValue(string valore)
        {
            string retValue = "&nbsp;";

            if (valore != null && valore != "")
            {
                retValue = valore;
            }

            return retValue;
        }

        /// <summary>
        /// verifica se la trasmissione è stata effettuata 
        /// dall'utente correntemente connesso
        /// </summary>
        /// <returns></returns>
        private bool CheckTrasmEffettuataDaUtenteCorrente(Trasmissione trasm)
        {
            bool retValue = false;

            if (trasm != null && trasm.utente != null) retValue = (trasm.utente.idPeople.Equals(UserManager.GetInfoUser().idPeople));

            return retValue;
        }

        /// <summary>
        ///Verifica se le note singole devono essere visualizzate.
        ///Rules:
        ///Le note singole sono visibili se:
        /// -   Il ruolo corrente è il mittente della trasmissione;
        /// -   Il ruolo corrente è il destinatario della tramissione;
        /// -   La trasmissione è ad utente e
        ///
        /// </summary>
        /// <returns></returns>
        private bool IsSingleNotesVisible(Trasmissione trasm, TrasmissioneSingola trasmSing)
        {
            bool visible = false;
            if (trasm != null &&
                trasm.ruolo != null &&
                !string.IsNullOrEmpty(trasm.ruolo.systemId) &&
                trasm.ruolo.systemId.Equals(RoleManager.GetRoleInSession().systemId))
                visible = true;
            else if (trasmSing != null &&
                trasmSing.tipoDest != null &&
                trasmSing.tipoDest == TrasmissioneTipoDestinatario.RUOLO &&
                !string.IsNullOrEmpty(trasmSing.corrispondenteInterno.systemId) &&
                trasmSing.corrispondenteInterno.systemId.Equals(RoleManager.GetRoleInSession().systemId))
                visible = true;
            else if (trasmSing != null &&
                trasmSing.tipoDest != null &&
                trasmSing.tipoDest == TrasmissioneTipoDestinatario.UTENTE &&
                (trasmSing.corrispondenteInterno as Utente) != null &&
                (trasmSing.corrispondenteInterno as Utente).idPeople.Equals(UserManager.GetInfoUser().idPeople))
                visible = true;
            return visible;
        }

        /// <summary>
        /// verifica se l'utente relativo ad una trasmissione utente sia lo
        /// stesso soggetto correntemente connesso all'applicazione
        /// </summary>
        /// <param name="trasmUtente"></param>
        private bool CheckTrasmUtenteCorrente(DocsPaWR.TrasmissioneUtente trasmUtente)
        {
            bool retValue = false;

            if (trasmUtente.utente != null)
            {
                retValue = (trasmUtente.utente.idPeople.Equals(UserManager.GetInfoUser().idPeople));
            }

            return retValue;
        }

        #endregion

        #region Management buttom

        private void EnableButtonsNotification()
        {

            bool isInToDoList = false;
            bool isToUserOrRole = true;
            TrasmissioneSingola trasmSing = null;
            TrasmissioneUtente trasmUtente = null;
            TrasmissioneSingola[] listTrasmSing;

            this.BtnAccept.Visible = false;
            this.BtnAcceptAdLU.Visible = false;
            this.BtnAcceptAdLR.Visible = false;
            this.BtnAcceptLF.Visible = false;
            this.BtnReject.Visible = false;

            this.BtnView.Visible = false;
            this.BtnViewAdLU.Visible = false;
            this.BtnViewAdLR.Visible = false;

            this.upPnlNoteAccRif.Visible = false;
            this.upPnlNoteAccRif.Update();

            this.pnlFascRequired.Visible = false;
            this.upPnlFascRequired.Update();

            listTrasmSing = Transmission.trasmissioniSingole;
            if (listTrasmSing != null && listTrasmSing.Length > 0 && !string.IsNullOrEmpty(Transmission.dataInvio))
            {
                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                //Vedo se presente la trasmissione a ruolo
                List<DocsPaWR.TrasmissioneSingola> list = new List<TrasmissioneSingola>(Transmission.trasmissioniSingole);

                //Emanuela: modifica per accettazione di trasmissioni a ruolo ed utente, seleziono per defaul prima quella di tipo ruolo per far si
                //che funzioni la gestione di accettazioni di entrambi le trasm
                List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.RUOLO).ToList();
                TrasmToElaborate = TrasmManager.RoleTransmissionWithHistoricized(trasmSingoleUtente, infoUtente.idCorrGlobali);
                bool trasmRoleWorked = true;

                if (TrasmToElaborate != null)
                {
                    trasmUtente = (DocsPaWR.TrasmissioneUtente)TrasmToElaborate.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();
                    if (trasmUtente != null && TrasmToElaborate.ragione.tipo == "W" && string.IsNullOrEmpty(trasmUtente.dataRifiutata) && string.IsNullOrEmpty(trasmUtente.dataAccettata))
                    {
                        trasmRoleWorked = !TrasmManager.checkTrasm_UNO_TUTTI_AccettataRifiutata(TrasmToElaborate);
                    }
                    if (trasmUtente != null && TrasmToElaborate.ragione.tipo != "W" && TrasmManager.getIfDocOrFascIsInToDoList(infoUtente, trasmUtente.systemId))
                    {
                        trasmRoleWorked = false;
                    }
                }

                if (TrasmToElaborate == null || trasmRoleWorked)
                {
                    // Se non è stata trovata la trasmissione come destinatario a ruolo, 
                    // cerca quella con destinatario l'utente corrente

                    trasmSingoleUtente = list.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.UTENTE).ToList();
                    if (trasmSingoleUtente != null)
                    {
                        DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);
                        TrasmissioneSingola s = trasmSingoleUtente.Where(e => ((DocsPaWR.Utente)e.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();
                        if (s != null)
                            TrasmToElaborate = s;
                    }
                }
                trasmSing = this.TrasmToElaborate;
                if (trasmSing != null)
                {
                    trasmUtente = (DocsPaWR.TrasmissioneUtente)trasmSing.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();
                    if (trasmUtente != null && trasmSing.ragione.tipo == "W" && string.IsNullOrEmpty(trasmUtente.dataRifiutata) && string.IsNullOrEmpty(trasmUtente.dataAccettata))
                    {
                        bool value = TrasmManager.checkTrasm_UNO_TUTTI_AccettataRifiutata(trasmSing);
                        value = value && isToUserOrRole;
                        this.BtnAccept.Visible = value;
                        this.BtnAcceptAdLU.Visible = value;
                        this.BtnAcceptAdLR.Visible = value;

                        if (Transmission.infoDocumento != null && this.EnabledLibroFirma && LibroFirmaManager.CanInsertInLibroFirma(trasmSing, Transmission.infoDocumento.docNumber))
                        {
                            this.BtnAcceptLF.Visible = value;
                        }

                        this.BtnReject.Visible = value;

                        this.upPnlNoteAccRif.Visible = value;
                        this.upPnlNoteAccRif.Update();

                        if (Transmission.tipoOggetto.ToString().Equals(NotificationManager.ListDomainObject.DOCUMENT) && trasmSing.ragione.fascicolazioneObbligatoria)
                        {
                            this.pnlFascRequired.Visible = true;
                        }
                    }
                    else
                    {
                        if (trasmSing.ragione.tipo != "W")
                        {
                            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()])
                                && ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()] == "2"
                                )
                            {
                                this.BtnView.Visible = true;
                                this.BtnViewAdLU.Visible = true;
                                this.BtnViewAdLR.Visible = true;
                            }

                            // PALUMBO: inserita variabile isInToDoList per eliminare visualizzazione tasto Visto in caso 
                            // di trasmissione per IS e già vista incident: INC000000103503  

                            if (trasmUtente != null && TrasmManager.getIfDocOrFascIsInToDoList(infoUtente, trasmUtente.systemId) && isToUserOrRole)
                            {
                                isInToDoList = true;
                                this.BtnView.Visible = true;
                                this.BtnViewAdLU.Visible = true;
                                this.BtnViewAdLR.Visible = true;
                            }
                            else
                            {
                                this.BtnView.Visible = false;
                                this.BtnViewAdLU.Visible = false;
                                this.BtnViewAdLR.Visible = false;
                            }

                            if (this.Transmission.infoDocumento != null && !string.IsNullOrEmpty(this.Transmission.infoDocumento.docNumber))
                            {
                                // PALUMBO: condizione valida per APSS
                                if (SimplifiedInteroperabilityManager.IsDocumentReceivedWithIS(Transmission.infoDocumento.idProfile) &&
                                    ConfigurationManager.AppSettings[WebConfigKeys.SET_DATA_VISTA_GRD.ToString()].Equals("1"))
                                {
                                    if (TrasmManager.getIfDocOrFascIsInToDoList(infoUtente, trasmUtente.systemId))
                                        isInToDoList = true;
                                }

                                // Se il documento è stato ricevuto per interoperabilità semplificata, il tasto visto deve essere
                                // visualizzato solo nel caso in cui il documento è già stato protocollato
                                // Il pulsante deve essere visualizzato indipendentemente dal fatto che sia attivo o meno il
                                // set_grd_datavista
                                // PALUMBO: inserita variabile isInToDoList per eliminare visualizzazione tasto Visto in caso 
                                // di trasmissione per IS e già vista incident: INC000000103503 
                                if (
                                    Transmission.tipoOggetto == TrasmissioneTipoOggetto.DOCUMENTO
                                    && SimplifiedInteroperabilityManager.IsDocumentReceivedWithIS(Transmission.infoDocumento.idProfile)
                                    && isInToDoList)
                                {
                                    bool value = !String.IsNullOrEmpty(Transmission.infoDocumento.segnatura) ||
                                        (String.IsNullOrEmpty(Transmission.infoDocumento.segnatura) && Transmission.infoDocumento.tipoProto.Equals("G"));
                                    this.BtnView.Visible = value;
                                    this.BtnViewAdLU.Visible = value;
                                    this.BtnViewAdLR.Visible = value;
                                }
                            }
                        }
                        else
                        {
                            // hide note acc/rif
                            this.upPnlNoteAccRif.Visible = false;
                            this.upPnlNoteAccRif.Update();
                        }
                    }
                }
                else
                {
                    // hide note acc/rif
                    this.upPnlNoteAccRif.Visible = false;
                    this.upPnlNoteAccRif.Update();
                }
            }

        }

        /// <summary>
        /// Disable all buttons
        /// </summary>
        private void DisabledButtons()
        {
            this.BtnAccept.Visible = false;
            this.BtnAcceptAdLU.Visible = false;
            this.BtnAcceptAdLR.Visible = false;
            this.BtnAcceptLF.Visible = false;
            this.BtnReject.Visible = false;

            this.BtnView.Visible = false;
            this.BtnViewAdLU.Visible = false;
            this.BtnViewAdLR.Visible = false;
        }

        #endregion

        #region Event hendler


        protected void BtnView_Click(object sender, System.EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
               
                if (TrasmManager.getSelectedTransmission().infoDocumento != null)
                {
                    DocsPaWR.InfoDocumento infoDocumento = TrasmManager.getSelectedTransmission().infoDocumento;
                    TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), infoDocumento.docNumber, "D", infoDocumento.idRegistro, TrasmManager.getSelectedTransmission().systemId);
                }
                else
                {
                    InfoFascicolo infofascicolo = TrasmManager.getSelectedTransmission().infoFascicolo;
                    TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), infofascicolo.idFascicolo, "F", infofascicolo.idRegistro, TrasmManager.getSelectedTransmission().systemId);
                }

                DisabledButtons();
                this.EnableButtonsNotification();
                this.ShowTransmission();
                this.UpPnlButtons.Update();
                this.UpdatePanelTransmission.Update();
                this.UpdatePanelContainerInfoNotify.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnViewAdLU_Click(object sender, System.EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
               
                if (TrasmManager.getSelectedTransmission().infoDocumento != null)
                {
                    DocsPaWR.InfoDocumento infoDocumento = TrasmManager.getSelectedTransmission().infoDocumento;
                    TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), infoDocumento.docNumber, "D", infoDocumento.idRegistro, TrasmManager.getSelectedTransmission().systemId);
                    DocumentManager.addWorkArea(this.Page, Transmission.infoDocumento);
                }
                else
                {
                    InfoFascicolo infofascicolo = TrasmManager.getSelectedTransmission().infoFascicolo;
                    Fascicolo prj = ProjectManager.getFascicoloById(infofascicolo.idFascicolo);
                    TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), infofascicolo.idFascicolo, "F", infofascicolo.idRegistro, TrasmManager.getSelectedTransmission().systemId);
                    ProjectManager.addFascicoloInAreaDiLavoro(prj,UIManager.UserManager.GetInfoUser());
                }
              
                DisabledButtons();
                this.EnableButtonsNotification();
                this.ShowTransmission();
                this.UpPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnViewAdLR_Click(object sender, System.EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
               
                if (TrasmManager.getSelectedTransmission().infoDocumento != null)
                {
                    DocsPaWR.InfoDocumento infoDocumento = TrasmManager.getSelectedTransmission().infoDocumento;
                    TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), infoDocumento.docNumber, "D", infoDocumento.idRegistro, TrasmManager.getSelectedTransmission().systemId);
                    DocumentManager.addWorkAreaRole(this.Page, Transmission.infoDocumento);
                }
                else
                {
                    InfoFascicolo infofascicolo = TrasmManager.getSelectedTransmission().infoFascicolo;
                    Fascicolo prj = ProjectManager.getFascicoloById(infofascicolo.idFascicolo);
                    TrasmManager.setdatavistaSP_TV(UserManager.GetInfoUser(), infofascicolo.idFascicolo, "F", infofascicolo.idRegistro, TrasmManager.getSelectedTransmission().systemId);
                    ProjectManager.addFascicoloInAreaDiLavoroRole(prj, UIManager.UserManager.GetInfoUser());
                }

                DisabledButtons();
                this.EnableButtonsNotification();
                this.ShowTransmission();
                this.UpPnlButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnAccept_Click(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            this.AcceptTransmission();
        }

        protected void BtnAcceptLF_Click(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            InfoUtente infoUtente = UserManager.GetInfoUser();
            this.AcceptTransmission();
            if (Transmission.infoDocumento != null)
            {
                DocumentManager.addWorkArea(this, Transmission.infoDocumento);

                System.Collections.Generic.List<DocsPaWR.TrasmissioneSingola> list = new System.Collections.Generic.List<TrasmissioneSingola>(Transmission.trasmissioniSingole);
                List<DocsPaWR.TrasmissioneSingola> trasmSingoleUtente = list.Where(i => i.tipoDest == TrasmissioneTipoDestinatario.UTENTE).ToList();
                TrasmissioneSingola trasmSing = trasmSingoleUtente.Where(i => ((DocsPaWR.Utente)i.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();

                if (trasmSing == null)
                {
                    // Se non è stata trovata la trasmissione come destinatario ad utente, 
                    // cerca quella con destinatario ruolo corrente dell'utente
                    trasmSingoleUtente = list.Where(i => i.tipoDest == TrasmissioneTipoDestinatario.RUOLO).ToList();
                    trasmSing = TrasmManager.RoleTransmissionWithHistoricized(trasmSingoleUtente, infoUtente.idCorrGlobali);
                }

                LibroFirmaManager.InserimentoInLibroFirma(this.Page, Transmission, trasmSing.systemId);
            }
        }



        protected void BtnAcceptAdLU_Click(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            this.AcceptTransmission();

            if (TrasmManager.getSelectedTransmission().infoDocumento != null)
            {
                DocumentManager.addWorkArea(this, Transmission.infoDocumento);
                //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
                SchedaDocumento sch = DocumentManager.getDocumentDetailsNoSecurity(this.Page, TrasmManager.getSelectedTransmission().infoDocumento.idProfile, TrasmManager.getSelectedTransmission().infoDocumento.idProfile);
                DocumentManager.setSelectedRecord(sch);
            }
            else
            {
                Fascicolo prj = ProjectManager.getFascicoloById(TrasmManager.getSelectedTransmission().infoFascicolo.idFascicolo);
                ProjectManager.addFascicoloInAreaDiLavoro(prj, UIManager.UserManager.GetInfoUser());
                ProjectManager.setProjectInSession(prj);
            }
        }

        protected void BtnAcceptAdLR_Click(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            this.AcceptTransmission();

            if (TrasmManager.getSelectedTransmission().infoDocumento != null)
            {
                DocumentManager.addWorkAreaRole(this, Transmission.infoDocumento);
                //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
                SchedaDocumento sch = DocumentManager.getDocumentDetailsNoSecurity(this.Page, TrasmManager.getSelectedTransmission().infoDocumento.idProfile, TrasmManager.getSelectedTransmission().infoDocumento.idProfile);
                DocumentManager.setSelectedRecord(sch);
            }
            else
            {
                Fascicolo prj = ProjectManager.getFascicoloById(TrasmManager.getSelectedTransmission().infoFascicolo.idFascicolo);
                ProjectManager.addFascicoloInAreaDiLavoroRole(prj, UIManager.UserManager.GetInfoUser());
                ProjectManager.setProjectInSession(prj);
            }
        }

        protected void BtnCancel_Click(object sender, System.EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('ViewDetailTransmission','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnReject_Click(object sender, System.EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                bool result = AcceptReject(DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO);
                if (result)
                {
                    if (Transmission.infoDocumento != null)
                    {
                        //Cancello i riferimenti alle tramissioni da controllare per quanto riguarda
                        //il passaggio di stato automatico
                        if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                        {
                            DocsPaWR.InfoDocumento infoDocumento = Transmission.infoDocumento;
                            string docNumber = infoDocumento.docNumber;
                            DocsPaWR.Stato stato = DiagrammiManager.GetStateDocument(docNumber);
                            if (stato != null)
                            {
                                string idStato = Convert.ToString(stato.SYSTEM_ID);
                                DiagrammiManager.deleteStoricoTrasmDiagrammi(docNumber, idStato);
                            }
                        }
                    }
                    else
                    {
                        //Cancello i riferimenti alle tramissioni da controllare per quanto riguarda
                        //il passaggio di stato automatico
                        if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                        {
                            DocsPaWR.InfoFascicolo infoFascicolo = Transmission.infoFascicolo;
                            string idFascicolo = infoFascicolo.idFascicolo;
                            DocsPaWR.Stato stato = DiagrammiManager.getStatoFasc(idFascicolo);
                            if (stato != null)
                            {
                                string idStato = Convert.ToString(stato.SYSTEM_ID);
                                DiagrammiManager.deleteStoricoTrasmDiagrammi(idFascicolo, idStato);
                            }
                        }
                    }
                    this.DisabledButtons();
                    this.EnableButtonsNotification();
                    ShowTransmission();
                    ShowDetailsNotification();
                    this.upPnlNoteAccRif.Visible = false;
                    this.UpdatePanelTransmission.Update();
                    this.UpdatePanelContainerInfoNotify.Update();
                    this.UpPnlButtons.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion


        #region

        private void AcceptTransmission()
        {

            //Effettuo l'accettazione della trasmissione
            bool result = AcceptReject(DocsPaWR.TrasmissioneTipoRisposta.ACCETTAZIONE);


            if (result)
            {
                string errorMessage;
                if (TrasmManager.getSelectedTransmission().infoDocumento != null)
                {
                    SchedaDocumento doc = DocumentManager.getDocumentDetailsNoDataCheck(this.Page, TrasmManager.getSelectedTransmission().infoDocumento.idProfile, TrasmManager.getSelectedTransmission().infoDocumento.idProfile, out errorMessage);
                    DocumentManager.setSelectedRecord(doc);
                }
                else
                {
                    Fascicolo prj = ProjectManager.getFascicoloById(TrasmManager.getSelectedTransmission().infoFascicolo.idFascicolo);
                    ProjectManager.setProjectInSession(prj);
                }
            }

            //Verifico l'abilitazione dei diagrammi di stato
            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"].Equals("1"))
            {
                if (TrasmManager.getSelectedTransmission().infoDocumento != null)
                {
                    ChangeStateDocument();
                }
                else if (TrasmManager.getSelectedTransmission().infoFascicolo != null)
                {
                    ChangeStateFolder();
                }
            }
            DisabledButtons();
            EnableButtonsNotification();
            ShowTransmission();
            ShowDetailsNotification();
            this.upPnlNoteAccRif.Visible = false;
            this.UpdatePanelTransmission.Update();
            this.UpdatePanelContainerInfoNotify.Update();
            this.UpPnlButtons.Update();
        }

        private bool AcceptReject(DocsPaWR.TrasmissioneTipoRisposta tipoRisp)
        {
            bool rtn = true;
            string errore = string.Empty;
            DocsPaWR.Trasmissione trasmissione = TrasmManager.getSelectedTransmission();
            DocsPaWR.TrasmissioneSingola trasmSing = null;
            DocsPaWR.TrasmissioneUtente trasmUtente = null;
            string msg = string.Empty;
            if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
            {
                if (string.IsNullOrEmpty(this.txt_noteAccRif.Text.Trim()))
                {
                    msg = "ErrorTransmissionNoteReject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', 'Accettazione');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', 'Accettazione');}", true);

                    return false;
                }
            }

            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
            trasmSing = this.TrasmToElaborate;
            if (trasmSing != null)
            {

                Fascicolo project = null;
                //Nel caso di ragione con classificazione obbligatoria verifico che sia stato inserito il fascicolo
                if (trasmissione.tipoOggetto.ToString().Equals(NotificationManager.ListDomainObject.DOCUMENT) && tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.ACCETTAZIONE && trasmSing.ragione.fascicolazioneObbligatoria)
                {
                    if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
                    {
                        //RegisterStartupScript("alert", "<script>alert('La fascicolazione rapida è obbligatoria !');</script>");
                        msg = "WarningDocumentRequestProject";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return false;
                    }
                    DocsPaWR.Fascicolo fasc = this.Project;
                    if (fasc != null && fasc.systemID != null && this.EnableBlockClassification)
                    {
                        if (fasc.tipo.Equals("G") && fasc.isFascConsentita != null && fasc.isFascConsentita == "0")
                        {
                            string msgDesc = fasc.isFascicolazioneConsentita ? "WarningDocumentNoDocumentInsert" : "WarningDocumentNoDocumentInsertClassification";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return false;
                        }
                        if (fasc.tipo.Equals("P") && !fasc.isFascicolazioneConsentita)
                        {
                            string msgDesc = "WarningDocumentNoDocumentInsertFolder";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            return false;
                        }
                    }
                    project = this.Project;
                }

                //MODIFICA PRE PROBLEMA ACCETTAZIONE TRASMISSIONE INVIATA SIA A RUOLO CHE UTENTE
                DocsPaWR.TrasmissioneSingola[] listaTrasmSing;
                //DocsPaWR.TrasmissioneUtente[] listaTrasmUtente;

                listaTrasmSing = trasmissione.trasmissioniSingole;

                //foreach (TrasmissioneSingola sing in trasmissione.trasmissioniSingole)
                //{

                trasmUtente = trasmSing.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();

                //note acc/rif
                if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                    trasmUtente.noteRifiuto = this.txt_noteAccRif.Text;
                else
                    trasmUtente.noteAccettazione = this.txt_noteAccRif.Text;

                //data Accettazione /Rifiuto
                if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                    trasmUtente.dataRifiutata = dateformat.getDataOdiernaDocspa(); //getDataCorrente();
                else
                    trasmUtente.dataAccettata = dateformat.getDataOdiernaDocspa(); //getDataCorrente();

                //tipoRisposta
                trasmUtente.tipoRisposta = tipoRisp;

                if (!TrasmManager.executeAccRif(this, trasmUtente, trasmissione.systemId, project, out errore))
                {
                    rtn = false;
                    trasmUtente.dataRifiutata = null;
                    trasmUtente.dataAccettata = null;

                    msg = "ErrorCustom";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '', '" + errore.Replace("'", @"\'") + "');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '', '" + errore.Replace("'", @"\'") + "');}", true);
                }
                else if (trasmSing.tipoDest == TrasmissioneTipoDestinatario.RUOLO) //Se ho accettato una trasm a ruolo vado ad aggiornare eventuali trasm per competenza ad utente
                {
                    List<TrasmissioneSingola> trasmSingoleUtente = listaTrasmSing.Where(e => e.tipoDest == TrasmissioneTipoDestinatario.UTENTE && e.ragione.systemId.Equals(trasmSing.ragione.systemId)).ToList();
                    if (trasmSingoleUtente != null)
                    {
                        DocsPaWR.Utente utenteCorrente = (DocsPaWR.Utente)UserManager.getCorrispondenteByIdPeople(this, infoUtente.idPeople, AddressbookTipoUtente.INTERNO);
                        TrasmissioneSingola s = trasmSingoleUtente.Where(e => ((DocsPaWR.Utente)e.corrispondenteInterno).idPeople == infoUtente.idPeople).FirstOrDefault();
                        if (s != null)
                        {
                            TrasmissioneUtente trasmUtente2 = s.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();
                            trasmUtente2.dataRifiutata = trasmUtente.dataRifiutata;
                            trasmUtente2.dataAccettata = trasmUtente.dataAccettata;
                        }
                    }
                }
                //}

                ////NEL CASO DI TRASMISSIONE A RUOLO ACCETTO TUTTE LE TRASMISSIONE AD UTENTE
                //if (trasmSing.tipoDest == TrasmissioneTipoDestinatario.RUOLO && listaTrasmSing.Length > 1)
                //{
                //    DocsPaWR.TrasmissioneSingola trasmSingTemp;
                //    for (int i = 1; i < listaTrasmSing.Length; i++)
                //    {
                //        trasmSingTemp = (DocsPaWR.TrasmissioneSingola)listaTrasmSing[i];
                //        listaTrasmUtente = trasmSingTemp.trasmissioneUtente;
                //        if (listaTrasmUtente.Length > 0 && trasmSingTemp.tipoDest == TrasmissioneTipoDestinatario.UTENTE)
                //        {
                //            trasmUtente = trasmSingTemp.trasmissioneUtente.Where(e => ((DocsPaWR.Utente)e.utente).idPeople == infoUtente.idPeople).FirstOrDefault();

                //            //note acc/rif
                //            if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                //                trasmUtente.noteRifiuto = this.txt_noteAccRif.Text;
                //            else
                //                trasmUtente.noteAccettazione = this.txt_noteAccRif.Text;

                //            //data Accettazione /Rifiuto
                //            if (tipoRisp == DocsPaWR.TrasmissioneTipoRisposta.RIFIUTO)
                //                trasmUtente.dataRifiutata = dateformat.getDataOdiernaDocspa(); //getDataCorrente();
                //            else
                //                trasmUtente.dataAccettata = dateformat.getDataOdiernaDocspa(); //getDataCorrente();

                //            //tipoRisposta
                //            trasmUtente.tipoRisposta = tipoRisp;

                //            TrasmManager.executeAccRif(this, trasmUtente, trasmissione.systemId, out errore);
                //        }
                //    }
                //}
            }
            return rtn;

        }

        private void ChangeStateDocument()
        {
            DocsPaWR.InfoDocumento infoDocumento = Transmission.infoDocumento;

            //E' importante che l'accettazione della trasmiossione corrente sia fatta prima di questo tipo di verifica
            if (DiagrammiManager.isUltimaDaAccettare(Transmission.systemId, this))
            {

                DocsPaWR.Stato statoSucc = DiagrammiManager.getStatoSuccessivoAutomatico(infoDocumento.docNumber);
                DocsPaWR.Stato statoCorr = DiagrammiManager.GetStateDocument(infoDocumento.docNumber);

                //Se il documento è di una tipologia sospesa non viene fatta nessuna considerazione su un eventuale passaggio di stato automatico
                if (!string.IsNullOrEmpty(infoDocumento.idTipoAtto))
                {
                    Templates tipoDocumento = ProfilerDocManager.getTemplate(infoDocumento.docNumber);
                    if (tipoDocumento != null && tipoDocumento.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        string msg = "ErrorTransmissionTypeSuspended";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');", true);
                        return;
                    }
                }

                if (statoSucc != null)
                {
                    if (statoSucc.STATO_FINALE)
                    {
                        //Controllo se non è bloccato il documento principale o un suo allegato
                        /* APPLET_G */
                        if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(infoDocumento.idProfile, infoDocumento.docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord())
                            || CheckInOutApplet.CheckInOutServices.IsCheckedOutDocument(infoDocumento.idProfile, infoDocumento.docNumber, UserManager.GetInfoUser(), true))
                        {
                            string msg = "ErrorTransmissionDocumentOrAttachmentsBlocked";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');", true);
                            return;
                        }

                        string msg2 = "ConfirmTransmissionFinalState";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "parent.ajaxConfirmModal('" + msg2.Replace("'", @"\'") + "', 'final_state', '');", true);
                        return;
                    }
                    else
                    {
                        //Cambio stato
                        DocsPaWR.DiagrammaStato dg = DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA));
                        DiagrammiManager.salvaModificaStato(infoDocumento.docNumber, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), "", this.Page);

                        //Cancellazione storico trasmissioni
                        DiagrammiManager.deleteStoricoTrasmDiagrammi(infoDocumento.docNumber, Convert.ToString(statoCorr.SYSTEM_ID));

                        //Verifico se il nuovo stato ha delle trasmissioni automatiche
                        DocsPaWR.Stato stato = DiagrammiManager.GetStateDocument(infoDocumento.docNumber);
                        string idTipoDoc = ProfilerDocManager.getIdTemplate(infoDocumento.docNumber);
                        if (idTipoDoc != "")
                        {
                            ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAuto(UserManager.GetInfoUser().idAmministrazione, Convert.ToString(stato.SYSTEM_ID), idTipoDoc));
                            for (int i = 0; i < modelli.Count; i++)
                            {
                                DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                if (mod.SINGLE.Equals("1"))
                                {
                                    TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), Transmission.infoDocumento, this);
                                }
                                else
                                {
                                    for (int k = 0; k < mod.MITTENTE.Length; k++)
                                    {
                                        if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.GetSelectedRole().systemId)
                                        {
                                            TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), Transmission.infoDocumento, this);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ChangeStateFolder()
        {
            InfoFascicolo infoFascicolo = Transmission.infoFascicolo;
            Fascicolo prj = UIManager.ProjectManager.getFascicoloById(Notification.ID_OBJECT);
            prj.template = ProfilerProjectManager.getTemplateFascDettagli(prj.systemID);
            ProjectManager.setProjectInSession(prj);
            //E' importante che l'accettazione della trasmiossione corrente sia fatta prima di questo tipo di verifica
            if (DiagrammiManager.isUltimaDaAccettare(Transmission.systemId, this.Page))
            {

                DocsPaWR.Stato statoSucc = DiagrammiManager.getStatoSuccessivoAutomaticoFasc(infoFascicolo.idFascicolo);
                DocsPaWR.Stato statoCorr = DiagrammiManager.getStatoFasc(infoFascicolo.idFascicolo);
                string idTemplate = string.Empty;
                if (prj.template != null)
                {
                    idTemplate = prj.template.SYSTEM_ID.ToString();
                }
                //Se il fascicolo è di una tipologia sospesa non viene fatta nessuna considerazione su un eventuale passaggio di stato automatico
                if (!string.IsNullOrEmpty(idTemplate))
                {
                    Templates tipoFascicolo = ProfilerDocManager.getTemplate(infoFascicolo.idFascicolo);
                    if (tipoFascicolo != null && tipoFascicolo.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        string msg = "ErrorTransmissionTypeSuspended";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                }

                if (statoSucc != null)
                {
                    if (statoSucc.STATO_FINALE)
                    {
                        string msg2 = "ConfirmTransmissionFinalStateFasc";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msg2.Replace("'", @"\'") + "', 'final_state', '');", true);
                        return;
                    }
                    //Cambio stato
                    DocsPaWR.DiagrammaStato dg = DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA));
                    DiagrammiManager.salvaModificaStatoFasc(prj.systemID, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), string.Empty);

                    //Cancellazione storico trasmissioni
                    DiagrammiManager.deleteStoricoTrasmDiagrammi(prj.systemID, Convert.ToString(statoCorr.SYSTEM_ID));

                    //Verifico se il nuovo stato ha delle trasmissioni automatiche
                    DocsPaWR.Stato stato = ProjectManager.getStatoFasc(prj);
                    string idTipoFasc = string.Empty;
                    if (prj.template != null)
                    {
                        idTipoFasc = prj.template.SYSTEM_ID.ToString();
                    }
                    if (idTipoFasc != "")
                    {
                        ArrayList modelli = new ArrayList(DiagrammiManager.isStatoTrasmAuto(UserManager.GetInfoUser().idAmministrazione, Convert.ToString(stato.SYSTEM_ID), idTipoFasc));
                        for (int i = 0; i < modelli.Count; i++)
                        {
                            DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];

                            if (mod.SINGLE == "1")
                            {
                                //DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, Fasc, this);
                                TrasmManager.effettuaTrasmissioneFascDaModello(mod, Convert.ToString(stato.SYSTEM_ID), prj, this);
                                //TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), trasmissione.infoDocumento, this);
                            }
                            else
                            {
                                for (int k = 0; k < mod.MITTENTE.Length; k++)
                                {
                                    if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.GetSelectedRole().systemId)
                                    {
                                        //TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), trasmissione.infoDocumento, this);
                                        TrasmManager.effettuaTrasmissioneFascDaModello(mod, Convert.ToString(stato.SYSTEM_ID), prj, this);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private void ChangeState()
        {
            Fascicolo prj = ProjectManager.getProjectInSession();
            DocsPaWR.Trasmissione trasmissione = TrasmManager.getSelectedTransmission();
            // da verifiarlo solo per i documenti
            DocsPaWR.InfoFascicolo infoFascicolo = trasmissione.infoFascicolo;
            DocsPaWR.Stato statoSucc = DiagrammiManager.getStatoSuccessivoAutomaticoFasc(infoFascicolo.idFascicolo);
            DocsPaWR.DiagrammaStato dg = DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA));
            DiagrammiManager.salvaModificaStatoFasc(prj.systemID, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.GetInfoUser().userId, UserManager.GetInfoUser(), string.Empty);
            this.chiudiFascicolo(prj, UIManager.UserManager.GetInfoUser(), UIManager.RoleManager.GetRoleInSession());
        }

        /// <summary>
        /// consente la chiusura di un fascicolo
        /// </summary>
        private void chiudiFascicolo(Fascicolo fascicolo, InfoUtente infoutente, Ruolo ruolo)
        {
            string msg = string.Empty;
            fascicolo.chiusura = DateTime.Now.ToShortDateString();
            fascicolo.stato = "C";
            if (fascicolo.chiudeFascicolo == null)
            {
                fascicolo.chiudeFascicolo = new DocsPaWR.ChiudeFascicolo();
            }
            fascicolo.chiudeFascicolo.idPeople = infoutente.idPeople;
            fascicolo.chiudeFascicolo.idCorrGlob_Ruolo = ruolo.systemId;
            fascicolo.chiudeFascicolo.idCorrGlob_UO = ruolo.uo.systemId;
            fascicolo = UIManager.ProjectManager.setFascicolo(fascicolo, infoutente);
            if (fascicolo == null)
            {
                msg = "ErroreProjectChiusuraFascicolo";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error');}", true);
            }
        }
        #endregion

        #region FASCICOLAZIONE OBBLIGATORIA

        protected void btnSearchProject_Click(object sender, EventArgs e)
        {
            if (this.ReturnValue.Split('#').Length > 1)
            {
                this.TxtCodeProject.Text = this.ReturnValue.Split('#').First();
                this.TxtDescriptionProject.Text = this.ReturnValue.Split('#').Last();
                this.UpPnlProject.Update();
                TxtCodeProject_OnTextChanged(new object(), new EventArgs());
            }
            else
                //Laura 19 Marzo
                if (this.ReturnValue.Contains("//"))
                {
                    this.TxtCodeProject.Text = this.ReturnValue;
                    this.TxtDescriptionProject.Text = "";
                    this.UpPnlProject.Update();
                    TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProject','');", true);
        }

        protected void btnTitolarioPostback_Click(object sender, EventArgs e)
        {
            if (this.ReturnValue.Split('#').Length > 1)
            {
                this.TxtCodeProject.Text = this.ReturnValue.Split('#').First();
                this.TxtDescriptionProject.Text = this.ReturnValue.Split('#').Last();
                this.UpPnlProject.Update();
                TxtCodeProject_OnTextChanged(new object(), new EventArgs());
            }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','');", true);
        }

        protected void DocumentImgSearchProjects_Click(object sender, EventArgs e)
        {
            RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "pPnlBUttonsProject", "parent.ajaxModalPopupSearchProject();", true);
        }

        protected void DocumentImgOpenTitolario_Click(object sender, EventArgs e)
        {
            RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "pPnlBUttonsProject", "parent.ajaxModalPopupOpenTitolario();", true);
        }

        protected void ImgAddProjects_Click(object sender, EventArgs e)
        {
            try
            {
                SchedaDocumento schedaDoc = UIManager.DocumentManager.getDocumentDetailsNoSecurity(this.Page, this.Transmission.infoDocumento.docNumber, this.Transmission.infoDocumento.docNumber);
                DocumentManager.setSelectedRecord(schedaDoc);
                TrasmManager.setSelectedTransmission(this.Transmission);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('ViewDetailTransmission','OPEN_PROJECT');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCodeProject_OnTextChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                Registro registro = RoleManager.GetRoleInSession().registri[0];
                ProjectManager.removeFascicoloSelezionatoFascRapida(this);

                if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
                {
                    SchedaDocumento documentoSelezionato = UIManager.DocumentManager.getSelectedRecord();

                    if (registro == null)
                    {
                        this.SearchProjectNoRegistro();
                    }
                    else
                    {
                        this.SearchProjectRegistro();
                    }
                    this.cercaClassificazioneDaCodice(this.TxtCodeProject.Text);
                }
                else
                {
                    this.TxtCodeProject.Text = string.Empty;
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.IdProject.Value = string.Empty;
                    this.Project = null;
                    //Laura 25 Marzo
                    ProjectManager.setProjectInSessionForRicFasc(null);
                    ProjectManager.setProjectInSessionForRicFasc(String.Empty);
                }

                this.UpPnlProject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchProjectNoRegistro()
        {
            Registro registro = RoleManager.GetRoleInSession().registri[0];
            this.TxtDescriptionProject.Text = string.Empty;
            if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                this.TxtDescriptionProject.Text = string.Empty;
                return;
            }
            //su DocProfilo devo cercare senza condizione sul registro.
            //Basta che il fascicolo sia visibile al ruolo loggato

            if (this.TxtCodeProject.Text.IndexOf("//") > -1)
            {

                string codice = string.Empty;
                string descrizione = string.Empty;

                DocsPaWR.Fascicolo SottoFascicolo = getFolder(null, ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {
                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        TxtDescriptionProject.Text = descrizione;
                        this.TxtDescriptionProject.Focus();
                        TxtCodeProject.Text = codice;
                        this.Project = SottoFascicolo;
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, SottoFascicolo.idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                        // DocsPaWR.Fascicolo fascForRicFasc = ProjectManager.getFascicoloById(gerClassifica[gerClassifica.Length - 1].systemId);
                        ProjectManager.setProjectInSessionForRicFasc(gerClassifica[gerClassifica.Length - 1].codice);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);

                    }
                    else
                    {
                        //string msg = @"Attenzione, sottofascicolo non presente.";
                        string msg = "WarningDocumentSubFileNoFound";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                        this.TxtDescriptionProject.Text = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;

                    }
                }
                else
                {
                    //string msg = @"Attenzione, sottofascicolo non presente.";
                    string msg = "WarningDocumentSubFileNoFound";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.TxtCodeProject.Text = string.Empty;
                }
            }
            else
            {

                DocsPaWR.Fascicolo[] listaFasc = getFascicolo(registro);
                string codClassifica = string.Empty;
                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            this.TxtDescriptionProject.Text = listaFasc[0].descrizione;
                            this.TxtDescriptionProject.Focus();
                            //metto il fascicolo in sessione
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                codClassifica = listaFasc[0].codice;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);

                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            this.Project = listaFasc[0];
                            ProjectManager.setProjectInSessionForRicFasc(codClassifica);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                            //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'N');");
                        }
                        else
                        {
                            codClassifica = this.TxtCodeProject.Text;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                //codClassifica = codClassifica;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            //Page.RegisterStartupScript("openListaFasc","<SCRIPT>ApriSceltaFascicolo();</SCRIPT>");
                            //Session.Add("hasRegistriNodi",hasRegistriNodi);

                            //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'Y');");

                            //Da Fare
                            //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli2('" + codClassifica + "', 'Y')</script>");                            

                            return;
                        }
                    }
                    else
                    {
                        //caso 0: al codice digitato non corrisponde alcun fascicolo
                        if (listaFasc.Length == 0)
                        {
                            //Provo il caso in cui il fascicolo è chiuso
                            Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.TxtCodeProject.Text);
                            if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                            {
                                //string msg = @"Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato.";
                                string msg = "WarningDocumentFileNoOpen";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                //string msg = @"Attenzione, codice fascicolo non presente.";
                                string msg = "WarningDocumentCodFileNoFound";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                            }

                            this.TxtDescriptionProject.Text = string.Empty;
                            this.TxtCodeProject.Text = string.Empty;
                        }
                        //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                        //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', '');");
                    }
                }
            }
        }

        protected void SearchProjectRegistro()
        {
            Registro registro = RoleManager.GetRoleInSession().registri[0];
            this.TxtDescriptionProject.Text = string.Empty;
            string codClassifica = string.Empty;

            if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                this.TxtDescriptionProject.Text = string.Empty;
                this.Project = null;
                //Laura 25 Marzo
                ProjectManager.setProjectInSessionForRicFasc(null);
                ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                return;
            }

            //FASCICOLAZIONE IN SOTTOFASCICOLI

            if (this.TxtCodeProject.Text.IndexOf("//") > -1)
            {
                #region FASCICOLAZIONE IN SOTTOFASCICOLI
                string codice = string.Empty;
                string descrizione = string.Empty;
                DocsPaWR.Fascicolo SottoFascicolo = getFolder(registro, ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {

                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        TxtDescriptionProject.Text = descrizione;
                        this.TxtDescriptionProject.Focus();
                        TxtCodeProject.Text = codice;
                        this.Project = SottoFascicolo;
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, SottoFascicolo.idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                        ProjectManager.setProjectInSessionForRicFasc(gerClassifica[gerClassifica.Length - 1].codice);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);
                    }
                    else
                    {

                        //string msg = @"Attenzione, sottofascicolo non presente.";
                        string msg = "WarningDocumentSubFileNoFound";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        this.TxtDescriptionProject.Text = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;
                        this.Project = null;
                        ProjectManager.setProjectInSessionForRicFasc(null);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                    }
                }
                else
                {
                    Session["validCodeFasc"] = "false";

                    //string msg = @"Attenzione, sottofascicolo non presente.";
                    string msg = "WarningDocumentSubFileNoFound";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.TxtCodeProject.Text = string.Empty;
                    this.Project = null;
                    ProjectManager.setProjectInSessionForRicFasc(null);
                    ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                }

                #endregion
            }
            else
            {
                DocsPaWR.Fascicolo[] listaFasc = getFascicoli(registro);

                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            this.Project = listaFasc[0];
                            this.TxtDescriptionProject.Text = listaFasc[0].descrizione;
                            this.TxtDescriptionProject.Focus();
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                codClassifica = listaFasc[0].codice;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            ProjectManager.setProjectInSessionForRicFasc(codClassifica);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                        }
                        else
                        {
                            //caso 2: al codice digitato corrispondono piu fascicoli
                            codClassifica = this.TxtCodeProject.Text;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                //codClassifica = codClassifica;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }

                            ////Da Fare
                            //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codClassifica + "', 'Y')</script>");
                            return;
                        }
                    }
                    else
                    {
                        //caso 0: al codice digitato non corrisponde alcun fascicolo
                        if (listaFasc.Length == 0)
                        {
                            //Provo il caso in cui il fascicolo è chiuso
                            Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.TxtCodeProject.Text);
                            if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                            {
                                //string msg = @"Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato.";
                                string msg = "WarningDocumentFileNoOpen";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                //string msg = @"Attenzione, codice fascicolo non presente.";
                                string msg = "WarningDocumentCodFileNoFound";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            this.TxtDescriptionProject.Text = string.Empty;
                            this.TxtCodeProject.Text = string.Empty;
                            this.Project = null;
                            ProjectManager.setProjectInSessionForRicFasc(null);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                        }
                    }
                }
            }
        }

        private DocsPaWR.Fascicolo[] getFascicolo(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                string codiceFascicolo = TxtCodeProject.Text;
                listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        private DocsPaWR.Fascicolo[] getFascicoli(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!this.TxtCodeProject.Text.Equals(""))
            {
                string codiceFascicolo = TxtCodeProject.Text;
                listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        private DocsPaWR.Fascicolo getFolder(DocsPaWR.Registro registro, ref string codice, ref string descrizione)
        {
            DocsPaWR.Folder[] listaFolder = null;
            DocsPaWR.Fascicolo fasc = null;
            string separatore = "//";
            int posSep = this.TxtCodeProject.Text.IndexOf("//");
            if (this.TxtCodeProject.Text != string.Empty && posSep > -1)
            {

                string codiceFascicolo = TxtCodeProject.Text.Substring(0, posSep);
                string descrFolder = TxtCodeProject.Text.Substring(posSep + separatore.Length);

                listaFolder = ProjectManager.getListaFolderDaCodiceFascicolo(this, codiceFascicolo, descrFolder, registro);
                if (listaFolder != null && listaFolder.Length > 0)
                {
                    //calcolo fascicolazionerapida
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    fasc = ProjectManager.getFascicoloById(listaFolder[0].idFascicolo, infoUtente);

                    if (fasc != null)
                    {
                        //folder selezionato è l'ultimo
                        fasc.folderSelezionato = listaFolder[listaFolder.Length - 1];
                    }
                    codice = fasc.codice + separatore;
                    descrizione = fasc.descrizione + separatore;
                    for (int i = 0; i < listaFolder.Length; i++)
                    {
                        codice += listaFolder[i].descrizione + "/";
                        descrizione += listaFolder[i].descrizione + "/";
                    }
                    codice = codice.Substring(0, codice.Length - 1);
                    descrizione = descrizione.Substring(0, descrizione.Length - 1);

                }
            }
            if (fasc != null)
            {

                return fasc;

            }
            else
            {
                return null;
            }
        }

        private void FascicolazioneRapida(string idProfile)
        {
            DocsPaWR.Fascicolo fasc;
            string returnMsg = string.Empty;
            string retMes2 = string.Empty;
            fasc = this.Project;
            if (fasc != null)
            {
                if (fasc.stato == "C")
                {
                    returnMsg += "WarningDocumentProjectClosed";
                }
                else
                {
                    if (fasc != null && fasc.systemID != null && this.EnableBlockClassification)
                    {

                        if (fasc.tipo.Equals("G") && fasc.isFascConsentita != null && fasc.isFascConsentita == "0")
                        {
                            returnMsg = fasc.isFascicolazioneConsentita ? "WarningDocumentNoDocumentInsert" : "WarningDocumentNoDocumentInsertClassification";
                        }
                        if (fasc.tipo.Equals("P") && !fasc.isFascicolazioneConsentita)
                        {
                            returnMsg = "WarningDocumentNoDocumentInsertFolder";
                        }
                    }
                    if (!string.IsNullOrEmpty(returnMsg))
                    {
                        int risultato = DocumentManager.fascicolaRapida(idProfile, idProfile, string.Empty, fasc, true);
                        switch (risultato)
                        {
                            case 1:
                                //returnMsg += "WarningDocumentNoClassificated";
                                returnMsg += "WarningDocumentDocumentFound";
                                break;
                            case 2:
                                returnMsg += "WarningDocumentNoClassificatedSelect";
                                retMes2 = fasc.descrizione;
                                break;
                            case 3:
                                returnMsg += "WarningBloccoModificheDocumentoInLf";
                                break;
                        }

                    }
                    if (!string.IsNullOrEmpty(returnMsg))
                    {
                        if (!string.IsNullOrEmpty(retMes2))
                        {
                            string language = UIManager.UserManager.GetUserLanguage();
                            returnMsg = Utils.Languages.GetMessageFromCode("WarningDocumentNoClassificatedSelect", language);
                            returnMsg = returnMsg + " " + retMes2;
                            string msgDesc = "WarningDocumentCustom";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(returnMsg) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(returnMsg) + "');};", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + returnMsg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + returnMsg.Replace("'", @"\'") + "', 'warning', '');};", true);
                        }
                    }
                }

            }
        }

        private bool cercaClassificazioneDaCodice(string codClassificazione)
        {
            bool res = false;
            DocsPaWR.Fascicolo[] listaFasc;
            if (!string.IsNullOrEmpty(codClassificazione))
            {
                listaFasc = this.getFascicolo(RoleManager.GetRoleInSession().registri[0], codClassificazione);

                DocsPaWR.FascicolazioneClassificazione[] FascClass = ProjectManager.fascicolazioneGetTitolario2(this, codClassificazione, false, this.getIdTitolario(codClassificazione, listaFasc));
                if (FascClass != null && FascClass.Length != 0)
                {
                    HttpContext.Current.Session["classification"] = FascClass[0];
                }
                else
                {
                    HttpContext.Current.Session["classification"] = null;
                }
            }

            return res;
        }

        private DocsPaWR.Fascicolo[] getFascicolo(DocsPaWR.Registro registro, string codClassificazione)
        {
            DocsPaWR.Fascicolo[] listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codClassificazione, registro, "I");
            return listaFasc;
        }

        private string getIdTitolario(string codClassificazione, DocsPaWR.Fascicolo[] listaFasc)
        {
            if (listaFasc != null && listaFasc.Length > 0)
            {
                DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)listaFasc[0];
                return fasc.idTitolario;
            }
            else
            {
                return null;
            }
        }
           

        protected void SetAjaxDescriptionProject()
        {
            Ruolo ruolo = RoleManager.GetRoleInSession();
            string dataUser = ruolo.idGruppo;
            dataUser = dataUser + "-" + ruolo.registri[0].systemId;
            if (UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione) != null)
            {
                RapidSenderDescriptionProject.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione).ID + "-" + UIManager.UserManager.GetUserInSession().idPeople + "-" + UIManager.UserManager.GetUserInSession().systemId;
            }
        }

        private DocsPaWR.Fascicolo Project
        {
            get
            {
                Fascicolo result = null;
                if (HttpContext.Current.Session["project"] != null)
                {
                    result = HttpContext.Current.Session["project"] as Fascicolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["project"] = value;
            }
        }

        private string ReturnValue
        {
            get
            {
                //Laura 19 Marzo
                if ((HttpContext.Current.Session["ReturnValuePopup"]) != null)
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        private bool EnableBlockClassification
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableBlockClassification"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableBlockClassification"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableBlockClassification"] = value;
            }
        }

        #endregion
    }
}
