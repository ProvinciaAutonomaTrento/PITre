using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using System.Web.UI.HtmlControls;
using NttDataWA.UIManager;
using NttDataWA.Utils;


namespace NttDataWA.Popup
{
    public partial class ReceivedSending : System.Web.UI.Page
    {
        #region Fields
        protected DocsPaWR.Corrispondente corr;
        protected DocsPaWR.SchedaDocumento schedaDocumento;
        string tipoCor;
        string indexCor;
        protected DocsPaWR.ProtocolloDestinatario[] dest;
        protected ArrayList Dg_elem;
        protected DocsPaWR.Corrispondente[] allCorr;
        #endregion


        #region Properties
        protected FiltroRicerca FilterDestinatario
        {
            set
            {
                Session["filter.DestinatarioNotifiche"] = value;
            }
            get
            {
                if (Session["filter.DestinatarioNotifiche"] != null)
                    return (FiltroRicerca)Session["filter.DestinatarioNotifiche"];
                else
                    return new FiltroRicerca { argomento = "filterMailDest", valore = string.Empty };
            }
        }
        protected FiltroRicerca FilterTipo
        {
            set
            {
                Session["filter.TipoNotifica"] = value;
            }
            get
            {
                if (Session["filter.TipoNotifica"] != null)
                    return (FiltroRicerca)Session["filter.TipoNotifica"];
                else
                    return new FiltroRicerca { argomento = "filterTipo", valore = string.Empty };
            }
        }
        protected FiltroRicerca FilterIdDoc
        {
            set
            {
                Session["filter.IdDoc"] = value;
            }
            get
            {
                if (Session["filter.IdDoc"] != null)
                    return (FiltroRicerca)Session["filter.IdDoc"];
                else
                    return new FiltroRicerca { argomento = "filterIdDoc", valore = string.Empty };
            }
        }
        protected FiltroRicerca FilterCodiceIS
        {
            set
            {
                Session["filter.CodiceIS"] = value;
            }
            get
            {
                if (Session["filter.CodiceIS"] != null)
                    return (FiltroRicerca)Session["filter.CodiceIS"];
                else
                    return new FiltroRicerca { argomento = "filterCodiceIS", valore = string.Empty };
            }
        }
        protected FiltroRicerca FilterRicevute
        {
            set
            {
                Session["filter.Ricevuta"] = value;
            }
            get
            {
                if (Session["filter.Ricevuta"] != null)
                    return (FiltroRicerca)Session["filter.Ricevuta"];
                else
                    return new FiltroRicerca { argomento = "filterRicevuta", valore = string.Empty };
            }
        }
        protected FiltroRicerca FilterAnnulla
        {
            set
            {
                Session["filter.Annulla"] = value;
            }
            get
            {
                if (Session["filter.Annulla"] != null)
                    return (FiltroRicerca)Session["filter.Annulla"];
                else
                    return new FiltroRicerca { argomento = "filterAnnulla", valore = string.Empty };
            }
        }

        protected FiltroRicerca FilterExportRicevuteInterop
        {
            set
            {
                Session["filter.exportInterop"] = value;
            }
            get
            {
                if (Session["filter.exportInterop"] != null)
                    return (FiltroRicerca)Session["filter.exportInterop"];
                else
                    return new FiltroRicerca { argomento = "filterExportInterop", valore = string.Empty };
            }
        }

        private string CorrType
        {
            get
            {
                return HttpContext.Current.Session["corrType"] as String;
            }
            set
            {
                HttpContext.Current.Session["corrType"] = value;
            }
        }

        private string IndexCorr
        {
            get
            {
                return HttpContext.Current.Session["indexCorr"] as String;
            }
            set
            {
                HttpContext.Current.Session["indexCorr"] = value;
            }
        }

        private FileDocumento DocumentFile
        {
            get
            {
                return HttpContext.Current.Session["documentFile"] as FileDocumento;
            }
            set
            {
                HttpContext.Current.Session["documentFile"] = value;
            }
        }

        private ArrayList Notifiche
        {
            get 
            {
                return HttpContext.Current.Session["notifiche"] as ArrayList;
            }
            set
            {
                HttpContext.Current.Session["notifiche"] = value;
            }
        }

        #endregion

        /// <summary>
        /// Delete Property in session
        /// </summary>
        private void DeleteProperty()
        {

            HttpContext.Current.Session.Remove("corrType");
            HttpContext.Current.Session.Remove("indexCorr");
            HttpContext.Current.Session.Remove("documentFile");
            HttpContext.Current.Session.Remove("notifiche");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                // Put user code to initialize the page here
                if (!Page.IsPostBack)
                {
                    this.InitializePage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
    
        public void InitializePage()
        {
            this.InitializeLanguage();
            
            //seleziona tutti i filtri
            ReceivedSendingCbxEnDisAll.Checked = true;
            foreach (ListItem i in ReceivedSendingCbxFilterType.Items)
            {
                i.Selected = true;
            }
            getDatiNotifica();
            //ConfigureFilterInUI();
            //DocumentManager.removeDataGridNotifiche(this.Page);
            //getDatiNotifica();

        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            //this.ReceivedSendingLblReceivedDocument.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingLblReceivedDocument", language);
            this.ReceivedSendingBtnClose.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingBtnClose", language);
            this.ReceivedSendingBtnApplyFilter.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingBtnApplyFilter", language);
            this.ReceivedSendingCbxEnDisAll.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingCbxEnDisAll", language);
            this.ReceivedSendingExportReceipts.AlternateText = Utils.Languages.GetLabelFromCode("ReceivedSendingExportReceipts", language);
            this.ReceivedSendingExportReceipts.ToolTip = Utils.Languages.GetLabelFromCode("ReceivedSendingExportReceipts", language);
            this.ReceivedSendingCbxAccettazione.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingCbxAccettazione", language);
            this.ReceivedSendingCbxAvvenutaConsegna.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingCbxAvvenutaConsegna", language);
            this.ReceivedSendingCbxNonAccettazione.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingCbxNonAccettazione", language);
            this.ReceivedSendingCbxErroreConsegna.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingCbxErroreConsegna", language);
            this.ReceivedSendingCbxRicevuta.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingCbxRicevuta", language);
            this.ReceivedSendingCbxAnnulla.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingCbxAnnulla", language);
            this.ReceivedSendingCbxEccezione.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingCbxEccezione", language);
            this.ReceivedSendingCbxErrore.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingCbxErrore", language);
            this.ReceivedSendingLblTypeReceived.Text = Utils.Languages.GetLabelFromCode("ReceivedSendingLblTypeReceived", language);
            this.ReceivedSendinglblMailDest.Text = Utils.Languages.GetLabelFromCode("ReceivedSendinglblMailDest", language);
            this.ReceivedSendinglblCodiceCorr.Text = Utils.Languages.GetLabelFromCode("ReceivedSendinglblCodiceCorr", language);
        }

        private void getDatiNotifica()
        {
            //carica_Info
            tipoCor = CorrType;
            indexCor = IndexCorr;
            schedaDocumento = DocumentManager.getSelectedRecord();;
            bool visualizzaNotifichePec = true;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_NOTIFICHE_PEC.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VISUALIZZA_NOTIFICHE_PEC.ToString()]))
                visualizzaNotifichePec = true;


            //modifica
            DocsPaWR.Notifica[] notifica = null;
            if (visualizzaNotifichePec)
            {
                if (schedaDocumento != null &&
                    !string.IsNullOrEmpty(schedaDocumento.docNumber))
                {
                    notifica = SimplifiedInteroperabilityManager.getNotifica(schedaDocumento.docNumber);
                }
            }

            //fine modifica

            if (!schedaDocumento.tipoProto.Equals("P"))
            {
                this.lbl_message.Text = "Non ci sono ricevute/ricevute di ritorno";
                this.lbl_message.Visible = true;
                return;
            }
            if (schedaDocumento.systemId.Equals(""))
            {
                this.lbl_message.Text = "Non ci sono ricevute/ricevute di ritorno";
                this.lbl_message.Visible = true;
                return;
            }
            if ((tipoCor == null) || (tipoCor == ""))
                return;
            if (schedaDocumento == null)
                return;
            if (tipoCor.Equals("T"))
            {
                //prende tutti i corrispondenti e per ognuno cerca i dati di interop
                Dg_elem = new ArrayList();
                for (int i = 0; i < ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Length; i++)
                {
                    corr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[i];
                    if (corr != null && corr.systemId != null)
                        dest = DocumentManager.getDestinatariInteropAggConferma(schedaDocumento.systemId, corr);
                    if (dest != null && dest.Length > 0)
                    {
                        BindGrid(dest[0]);
                    }
                    if (corr != null)
                        BindGridNotifichePec(notifica, corr, "T");

                }

                if (((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza != null)
                {
                    for (int i = 0; i < ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza.Length; i++)
                    {
                        corr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[i];
                        if (corr != null && corr.systemId != null)
                            dest = DocumentManager.getDestinatariInteropAggConferma(schedaDocumento.systemId, corr);
                        if (dest != null && dest.Length > 0 && notifica == null)
                        {
                            BindGrid(dest[0]);
                        }

                        if (corr != null)
                            BindGridNotifichePec(notifica, corr, "T");
                    }
                }


                if (visualizzaNotifichePec && notifica != null)
                {
                    BindGridNotifichePec(notifica);
                    Dg_elem = Notifiche;
                }

                if (Dg_elem.Count > 0)
                {
                    this.grdList.DataSource = Dg_elem;
                    this.grdList.DataBind();
                }
                else
                {
                    this.lbl_message.Text = "Ricevute non trovate";
                    this.grdList.Visible = false;
                    this.lbl_message.Visible = true;
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(indexCor) && (!indexCor.Equals("-1")))
                {
                    if (tipoCor.Equals("D"))
                    {
                        if (!string.IsNullOrEmpty(indexCor) && Int32.Parse(indexCor) >= 0)
                            corr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[Int32.Parse(indexCor)];
                    }
                    else if (tipoCor.Equals("C"))
                    {
                        if (!string.IsNullOrEmpty(indexCor) && Int32.Parse(indexCor) >= 0)
                            corr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[Int32.Parse(indexCor)];
                    }

                    if (corr != null && corr.systemId != null)
                        dest = DocumentManager.getDestinatariInteropAggConferma(schedaDocumento.systemId, corr);


                    if ((dest == null || dest.Length == 0)
                        && (notifica == null || notifica.Length == 0))
                    {
                        this.lbl_message.Text = "Ricevute non trovate";
                        this.grdList.Visible = false;
                        this.lbl_message.Visible = true;
                        return;
                    }


                    Dg_elem = new ArrayList();
                    if (dest != null && dest.Length > 0)
                    {
                        BindGrid(dest[0]);
                    }
                }

                else
                {
                    Dg_elem = new ArrayList();
                    //if (tipoCor.Equals("D"))
                    //{
                    //    allCorr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
                    //}
                    //else if (tipoCor.Equals("C"))
                    //{
                    //    allCorr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;
                    //}

                    //PALUMBO: modifica per permettere la visualizzazione delle conferme ricezione dia dei destinatari in To che in CC 
                    //quando si clicca sulla relativa icona
                    List<Corrispondente> temp = new List<Corrispondente>();
                    temp.AddRange(((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari);
                    if (((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza != null)
                    {
                        temp.AddRange(((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza);
                    }
                    allCorr = temp.ToArray();

                    if (allCorr != null && allCorr.Length > 0)
                    {
                        foreach (DocsPaWR.Corrispondente c in allCorr)
                        {
                            if (c.systemId != null)
                            {
                                dest = DocumentManager.getDestinatariInteropAggConferma(schedaDocumento.systemId, c);
                                if (dest != null && dest.Length > 0)
                                {
                                    BindGrid(dest[0]);
                                }
                            }
                        }
                    }
                }

                if (visualizzaNotifichePec && notifica != null)
                {
                    if (corr != null)
                        BindGridNotifichePec(notifica, corr);
                    else
                        BindGridNotifichePec(notifica);
                    Dg_elem = Notifiche;
                }

                if (Dg_elem.Count > 0)
                {
                    this.grdList.DataSource = Dg_elem;
                    this.grdList.DataBind();
                    this.grdList.Visible = true;
                }
                else
                {
                    this.lbl_message.Text = "Ricevute non trovate";
                    this.grdList.Visible = false;
                    this.lbl_message.Visible = true;
                    return;
                }
            }
        }

        public void BindGridNotifichePec(DocsPaWR.Notifica[] notifica, DocsPaWR.Corrispondente corr, string tutti)
        {
            //Costruisco il datagrid
            ArrayList listaElementiPrecedenti = Notifiche;

            // Recupero dei destinatari del protocollo in uscita
            //Corrispondente[] receivers = ((ProtocolloUscita)DocumentManager.getDocumentoSelezionato().protocollo).destinatari;

            if (notifica != null)
            {
                for (int i = 0; i < notifica.Length; i++)
                {
                    string infoNotifica = string.Empty;
                    string nonCertificata = "MAIL NON CERTIFICATA";

                    bool corrok = false;
                    for (int j = 0; j < notifica.Length; j++)
                    {
                        if (corr.email.ToLower().Equals(notifica[j].destinatario.ToLower()))
                        {
                            corrok = true;
                            break;
                        }
                    }

                    if (!corrok)
                        break;

                    // Recupero del destinario con codice uguale a quello del destinatario della notifica
                    //Corrispondente receiver = receivers.Where(r => r.codiceRubrica.Contains(notifica[i].destinatario.Replace("'", ""))).FirstOrDefault();

                    DocsPaWR.TipoNotifica tiponotifica = SimplifiedInteroperabilityManager.getTipoNotifica(notifica[i].idTipoNotifica);
                    if (tiponotifica != null)
                    {
                        // per i DSN e le Eccezioni diamo piu dettagli e lo mettiamo prima
                        if ((tiponotifica.codiceNotifica == "eccezione") || (tiponotifica.codiceNotifica == "errore"))
                        {
                            //if (receiver != null && receiver.canalePref.tipoCanale == InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId)
                            //    infoNotifica += notifica[i].Reason + "\r\n\r\n";
                            //else
                            infoNotifica += notifica[i].oggetto + "\r\n\r\n";
                        }
                        else
                        {
                            // Se il destinatario ha come canale impostato, IS, viene visualizzato un testo differente rispetto
                            // a quello mostrato nel caso PEC
                            //if (receiver != null && receiver.canalePref.tipoCanale == InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId)
                            //{
                            //    infoNotifica = tiponotifica.descrizioneNotifica.Replace("email di ", String.Empty);
                            //    infoNotifica += " inviata il '" + notifica[i].data_ora + "'";
                            //}
                            //else
                            infoNotifica += tiponotifica.descrizioneNotifica + " è stata inviata il '" + notifica[i].data_ora + "'"
                                + "'. Il destinatario '" + notifica[i].destinatario + "' ha una mail di tipo: '" + (notifica[i].tipoDestinatario.ToUpper().Equals("ESTERNO") ? nonCertificata : notifica[i].tipoDestinatario) + "'."
                                + "Il codice identificatore del messaggio è: '" + notifica[i].identificativo + "'.";
                        }
                    }

                    string descrizioneNotifica = String.Empty;
                    //if (receiver != null && receiver.canalePref.tipoCanale == InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId)
                    //    descrizioneNotifica = tiponotifica.descrizioneNotifica.Replace("email di ", String.Empty) + (!notifica[i].erroreRicevuta.ToUpper().Equals("NESSUNO") || !string.IsNullOrEmpty(notifica[i].errore_esteso) ? " - " + notifica[i].erroreRicevuta : string.Empty);
                    //else
                    descrizioneNotifica = tiponotifica.descrizioneNotifica + (!notifica[i].erroreRicevuta.ToUpper().Equals("NESSUNO") || !string.IsNullOrEmpty(notifica[i].errore_esteso) ? " - " + notifica[i].erroreRicevuta : string.Empty);

                    if (listaElementiPrecedenti != null &&
                        listaElementiPrecedenti.Count > 0)
                    {
                        Cols col = new Cols(notifica[i].data_ora, descrizioneNotifica, notifica[i].mittente, infoNotifica, 1, notifica[i].errore_esteso, true);
                        bool aggiungi = true;
                        for (int j = 0; j < listaElementiPrecedenti.Count; j++)
                        {
                            Cols col1 = (Cols)listaElementiPrecedenti[j];
                            if ((col1.Dettaglio == col.Dettaglio) &&
                                (col1.Descrizione == col.Descrizione) &&
                                (col1.Data == col.Data) &&
                                (col1.Tipo == col.Tipo))
                            {
                                aggiungi = false;
                                break;
                            }

                        }
                        if (aggiungi)
                            listaElementiPrecedenti.Add(new Cols(notifica[i].data_ora, descrizioneNotifica, notifica[i].destinatario, infoNotifica, 1, notifica[i].errore_esteso, true));

                    }
                    else
                    {
                        Cols col = new Cols(notifica[i].data_ora, descrizioneNotifica, notifica[i].destinatario, infoNotifica, 1, notifica[i].errore_esteso, true);
                        bool aggiungi = true;
                        for (int j = 0; j < Dg_elem.Count; j++)
                        {
                            Cols col1 = (Cols)Dg_elem[j];
                            if ((col1.Dettaglio == col.Dettaglio) &&
                                (col1.Descrizione == col.Descrizione) &&
                                (col1.Data == col.Data) &&
                                (col1.Tipo == col.Tipo))
                            {
                                aggiungi = false;
                                break;
                            }

                        }
                        if (aggiungi)
                            Dg_elem.Add(col);
                    }
                }
            }

            if (listaElementiPrecedenti != null &&
                listaElementiPrecedenti.Count > 0)
                Notifiche = listaElementiPrecedenti;
            else
                Notifiche = Dg_elem; ;
        }

        public void BindGrid(DocsPaWR.ProtocolloDestinatario dest)
        {
            //Costruisco il datagrid

            string infoRicevuta = "";
            if (dest != null)
            {
                if (dest.protocolloDestinatario != null && !dest.protocolloDestinatario.Equals(""))
                {
                    infoRicevuta = "AMM: " + dest.codiceAmm + "<br>";
                    infoRicevuta += "AOO: " + dest.codiceAOO + "<br>";
                    infoRicevuta += "DATA PROT.: " + dest.dataProtocolloDestinatario + "<br>";
                    infoRicevuta += "PROTOC.: " + dest.protocolloDestinatario;
                }
                //notifica annullamento
                if (dest.annullato != null && dest.annullato.Equals("1") && (string.IsNullOrEmpty(FilterAnnulla.valore) || FilterAnnulla.valore.Equals("1")))
                {
                    string datiAnnullamento;
                    string infoDoc = infoRicevuta + "<br><br>";
                    datiAnnullamento = "MOTIVO ANNULLAMENTO: " + dest.motivo;
                    //datiAnnullamento += "Provvedimento: " + dest.provvedimento;
                    Dg_elem.Add(new Cols("--", "Annullamento protocollazione", dest.descrizioneCorr, infoDoc + datiAnnullamento, 0, string.Empty, false));
                }
                //ricevuta di ritorno
                if (dest.protocolloDestinatario != null && !dest.protocolloDestinatario.Equals("") && (string.IsNullOrEmpty(FilterRicevute.valore) || FilterRicevute.valore.Equals("1")))
                {
                    Dg_elem.Add(new Cols("--", "Conferma di ricezione", dest.descrizioneCorr, infoRicevuta, 1, string.Empty, false));
                }
                Notifiche = Dg_elem;
            }

        }

        public void BindGridNotifichePec(DocsPaWR.Notifica[] notifica)
        {
            //Costruisco il datagrid
            ArrayList listaElementiPrecedenti = Notifiche;

            if (notifica != null)
            {
                for (int i = 0; i < notifica.Length; i++)
                {
                    string infoNotifica = string.Empty;
                    string nonCertificata = "MAIL NON CERTIFICATA";

                    DocsPaWR.TipoNotifica tiponotifica = SimplifiedInteroperabilityManager.getTipoNotifica(notifica[i].idTipoNotifica);


                    if (tiponotifica != null)
                    {
                        // per i DSN e le Eccezioni diamo piu dettagli e lo mettiamo prima
                        if ((tiponotifica.codiceNotifica == "eccezione") || (tiponotifica.codiceNotifica == "errore"))
                        {
                            infoNotifica += notifica[i].oggetto + "\r\n\r\n";
                        }
                        else
                        {
                            // Se il mittente della ricevuta è un URL, significa che è stato utilizzato come canale
                            // l'interoperabilità semplificata, quindi viene creata una descrizione diversa
                            if (Uri.IsWellFormedUriString(notifica[i].mittente, UriKind.Absolute))
                                infoNotifica = String.Format("{0} inviata il {1}", tiponotifica.codiceNotifica.Equals("errore-consegna") ? "mancata-consegna" : tiponotifica.codiceNotifica, notifica[i].data_ora);
                            else
                                infoNotifica += tiponotifica.descrizioneNotifica + " è stata inviata il '" + notifica[i].data_ora + "'"
                                    + "'. Il destinatario '" + notifica[i].destinatario + "' ha una mail di tipo: '" + (notifica[i].tipoDestinatario.ToUpper().Equals("ESTERNO") ? nonCertificata : notifica[i].tipoDestinatario) + "'."
                                    + "Il codice identificatore del messaggio è: '" + notifica[i].identificativo + "'.";
                        }
                    }

                    // Se la ricevuta in esame è relativa ad una spedizione per interoperabilità semplificata,
                    // non deve comparire "email di"
                    string descrizioneNotifica = Uri.IsWellFormedUriString(notifica[i].mittente, UriKind.Absolute) ? (tiponotifica.codiceNotifica.Equals("errore-consegna") ? "mancata-consegna" : tiponotifica.codiceNotifica) : tiponotifica.descrizioneNotifica +
                        (!notifica[i].erroreRicevuta.ToUpper().Equals("NESSUNO") || !string.IsNullOrEmpty(notifica[i].errore_esteso) ? " - " + notifica[i].erroreRicevuta : string.Empty);

                    if (listaElementiPrecedenti != null && listaElementiPrecedenti.Count > 0)
                    {
                        Cols col = new Cols(notifica[i].data_ora, descrizioneNotifica, notifica[i].destinatario, infoNotifica, 1, notifica[i].errore_esteso, true);
                        bool aggiungi = true;
                        for (int j = 0; j < listaElementiPrecedenti.Count; j++)
                        {
                            Cols col1 = (Cols)listaElementiPrecedenti[j];
                            if ((col1.Dettaglio == col.Dettaglio) &&
                                (col1.Descrizione == col.Descrizione) &&
                                (col1.Data == col.Data) &&
                                (col1.Tipo == col.Tipo))
                            {
                                aggiungi = false;
                                break;
                            }

                        }

                        //se ho selezionato da tabProtocollo tutti i destinatari devo eseguire i controlli per il monocasella
                        if (aggiungi && string.IsNullOrEmpty(Request.QueryString["indexCor"]) || Request.QueryString["indexCor"].Equals("-1") &&
                            ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari != null)
                        {
                            aggiungi = false;
                            foreach (DocsPaWR.Corrispondente c in ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari)
                            {
                                if ((from m in c.Emails where m.Email.ToUpper().Equals(notifica[i].destinatario.ToUpper()) select m).Count() > 0)
                                {
                                    aggiungi = true;
                                    break;
                                }
                            }
                            string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                            if (!System.Text.RegularExpressions.Regex.Match(notifica[i].destinatario, pattern).Success)
                            {
                                aggiungi = true;
                            }
                        }

                        if (aggiungi)
                            listaElementiPrecedenti.Add(new Cols(notifica[i].data_ora, descrizioneNotifica, notifica[i].destinatario, infoNotifica, 1, notifica[i].errore_esteso, true));

                    }
                    else
                    {
                        Cols col = new Cols(notifica[i].data_ora, descrizioneNotifica, notifica[i].destinatario, infoNotifica, 1, notifica[i].errore_esteso, true);
                        bool aggiungi = true;
                        for (int j = 0; j < Dg_elem.Count; j++)
                        {
                            Cols col1 = (Cols)Dg_elem[j];
                            if ((col1.Dettaglio == col.Dettaglio) &&
                                (col1.Descrizione == col.Descrizione) &&
                                (col1.Data == col.Data) &&
                                (col1.Tipo == col.Tipo))
                            {
                                aggiungi = false;
                                break;
                            }

                        }
                        //se ho selezionato da tabProtocollo tutti i destinatari devo eseguire i controlli per il monocasella
                        if (aggiungi && string.IsNullOrEmpty(Request.QueryString["indexCor"]) || Request.QueryString["indexCor"].Equals("-1") &&
                            ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari != null)
                        {
                            aggiungi = false;
                            foreach (DocsPaWR.Corrispondente c in ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari)
                            {
                                if ((from m in c.Emails where m.Email.ToUpper().Equals(notifica[i].destinatario.ToUpper()) select m).Count() > 0)
                                {
                                    aggiungi = true;
                                    break;
                                }
                            }
                            string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                            if (!System.Text.RegularExpressions.Regex.Match(notifica[i].destinatario, pattern).Success)
                            {
                                aggiungi = true;
                            }
                        }

                        if (aggiungi)
                            Dg_elem.Add(col);
                    }
                }
            }

            if (listaElementiPrecedenti != null &&
                listaElementiPrecedenti.Count > 0)
                Notifiche = listaElementiPrecedenti;
            else
                Notifiche = Dg_elem;
        }

        public void BindGridNotifichePec(DocsPaWR.Notifica[] notifica, DocsPaWR.Corrispondente corr)
        {
            indexCor = Request.QueryString["indexCor"].ToString();
            if (notifica != null)
            {
                for (int i = 0; i < notifica.Length; i++)
                {
                    string infoNotifica = string.Empty;
                    string nonCertificata = "MAIL NON CERTIFICATA";

                    bool corrok = false;
                    //se il campo destinatario è diverso da mail allora non si tratta di una ricevuta pec
                    string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                    if (!System.Text.RegularExpressions.Regex.Match(notifica[i].destinatario, pattern).Success)
                    {
                        //filtro per codice corrispondente(IS)
                        if ((!string.IsNullOrEmpty(FilterCodiceIS.valore) && notifica[i].destinatario.Contains(FilterCodiceIS.valore)) ||
                            (string.IsNullOrEmpty(indexCor) || indexCor.Equals("-1")))
                            corrok = true;
                        else corrok = false;
                    }
                    else
                    {
                        // si tratta di una ricevuta pec(la prima volta non filtro per mail, quindi prendo tutte le ricevute pec)
                        if ((from m in corr.Emails where m.Email.ToUpper().Equals(notifica[i].destinatario.ToUpper()) select m).Count() > 0)
                        {
                            corrok = true;
                        }
                        else
                            corrok = false;
                    }
                    if (corrok)
                    {

                        DocsPaWR.TipoNotifica tiponotifica = SimplifiedInteroperabilityManager.getTipoNotifica(notifica[i].idTipoNotifica);
                        if (tiponotifica != null)
                        {
                            // per i DSN e le Eccezioni diamo piu dettagli e lo mettiamo prima
                            if ((tiponotifica.codiceNotifica == "eccezione") || (tiponotifica.codiceNotifica == "errore"))
                            {
                                infoNotifica += notifica[i].oggetto + "\r\n\r\n";
                            }
                            else
                            {
                                // Se il destinatario ha come canale impostato, IS, viene visualizzato un testo differente rispetto
                                // a quello mostrato nel caso PEC
                                //if (receiver != null && receiver.canalePref.tipoCanale == InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId)
                                if (Uri.IsWellFormedUriString(notifica[i].mittente, UriKind.Absolute))
                                {
                                    //infoNotifica = tiponotifica.codiceNotifica.Replace("email di ", String.Empty);
                                    //infoNotifica += " inviata il '" + notifica[i].data_ora + "'";
                                    infoNotifica = String.Format("{0} inviata il {1}", tiponotifica.codiceNotifica.Equals("errore-consegna") ? "mancata-consegna" : tiponotifica.codiceNotifica, notifica[i].data_ora);
                                }
                                else
                                    infoNotifica += tiponotifica.descrizioneNotifica + " è stata inviata il '" + notifica[i].data_ora + "'"
                                        + "'. Il destinatario '" + notifica[i].destinatario + "' ha una mail di tipo: '" + (notifica[i].tipoDestinatario.ToUpper().Equals("ESTERNO") ? nonCertificata : notifica[i].tipoDestinatario) + "'."
                                        + "Il codice identificatore del messaggio è: '" + notifica[i].identificativo + "'.";
                            }
                        }

                        //string descrizioneNotifica = tiponotifica.descrizioneNotifica + (notifica[i].erroreRicevuta.ToUpper().Equals("NESSUNO") || !string.IsNullOrEmpty(notifica[i].errore_esteso) ? string.Empty : " - " + notifica[i].erroreRicevuta);
                        string descrizioneNotifica = Uri.IsWellFormedUriString(notifica[i].mittente, UriKind.Absolute) ? (tiponotifica.codiceNotifica.Equals("errore-consegna") ? "mancata-consegna" : tiponotifica.codiceNotifica) :
                            tiponotifica.descrizioneNotifica + (!notifica[i].erroreRicevuta.ToUpper().Equals("NESSUNO") || !string.IsNullOrEmpty(notifica[i].errore_esteso) ? " - " + notifica[i].erroreRicevuta : string.Empty);
                        Cols col = new Cols(notifica[i].data_ora, descrizioneNotifica, notifica[i].destinatario, infoNotifica, 1, notifica[i].errore_esteso, true);
                        bool aggiungi = true;
                        for (int j = 0; j < Dg_elem.Count; j++)
                        {
                            Cols col1 = (Cols)Dg_elem[j];
                            if ((col1.Dettaglio == col.Dettaglio) &&
                                (col1.Descrizione == col.Descrizione) &&
                                (col1.Data == col.Data) &&
                                (col1.Tipo == col.Tipo))
                            {
                                aggiungi = false;
                                break;
                            }

                        }
                        if (aggiungi)
                            Dg_elem.Add(col);
                    }
                }

            }
            Notifiche = Dg_elem;
        }

        protected void ReceivedSendingCbxEnDisAll_CheckedChanged(object sender, EventArgs e)
        {
            try {
                CheckBox check = (sender as CheckBox);
                if (check.Checked)
                {
                    foreach (ListItem item in ReceivedSendingCbxFilterType.Items)
                        item.Selected = true;
                }
                else
                {
                    foreach (ListItem item in ReceivedSendingCbxFilterType.Items)
                        item.Selected = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ReceivedSendingBtnClose_Click(object sender, EventArgs e)
        {
            DeleteProperty();
            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('ReceivedSending', '');</script></body></html>");
            Response.End();
        }

        protected void ReceivedSendingBtnApplyFilter_Click(object sender, EventArgs e)
        { 
        }

        protected void ReceivedSendingExportReceipts_Click(object sender, EventArgs e)
        {
            tipoCor = CorrType;
            indexCor = IndexCorr;
            schedaDocumento = DocumentManager.getSelectedRecord();
            //configuro il filtro FilterExportRicevuteInterop
            System.Text.StringBuilder listCorr = new System.Text.StringBuilder();
            if (tipoCor.Equals("T"))
            {
                FilterExportRicevuteInterop = new FiltroRicerca { argomento = "filterExportInterop", valore = "T" };
            }
            else if (tipoCor.Equals("D"))
            {
                if (!string.IsNullOrEmpty(indexCor) && Int32.Parse(indexCor) >= 0)
                    corr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[Int32.Parse(indexCor)];
            }
            else if (tipoCor.Equals("C"))
            {
                if (!string.IsNullOrEmpty(indexCor) && Int32.Parse(indexCor) >= 0)
                    corr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[Int32.Parse(indexCor)];
            }
            if (corr != null && !string.IsNullOrEmpty(corr.systemId))
                FilterExportRicevuteInterop = new FiltroRicerca { argomento = "filterExportInterop", valore = corr.systemId };

            if (string.IsNullOrEmpty(FilterIdDoc.valore))
                FilterIdDoc = new FiltroRicerca { argomento = "filterIdDoc", valore = DocumentManager.getSelectedRecord().docNumber };
            if (string.IsNullOrEmpty(indexCor) || indexCor.Equals("-1"))
            {
                // nel caso di seleziona tutti devo verificare se è attivo il monocasella ed eventualmente considerare solo la mail principale
                //if (MultiBoxManager.IsEnabledMultiMail(UserManager.getRuolo().idAmministrazione))
                //{
                //    System.Text.StringBuilder strBuild = new System.Text.StringBuilder();
                //    foreach (DocsPaWR.Corrispondente c in ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari)
                //    {
                //        if (c.Emails != null && c.Emails.Length > 0)
                //            strBuild.Append(c.Emails[0].Email + "#");
                //    }
                //    if (strBuild.Length > 0)
                //        FilterDestinatario = new FiltroRicerca { argomento = "filterDest", valore = strBuild.ToString().ToLower() };
                //}
            }

            FileDocumento file = GenerateReport("NotifichePec", "NotificheSpedizione", "Ricevute di spedizione", "", ReportTypeEnum.Excel,
                new FiltroRicerca[] { FilterIdDoc, FilterTipo, FilterDestinatario, FilterCodiceIS, FilterExportRicevuteInterop });
            if (file != null)
            {
                DocumentFile = file; 
                //this.reportContent.Attributes["src"] = "Reporting/ReportContent.aspx";
                Response.Clear();
                Response.ContentType = file.contentType;
                Response.AddHeader("content-disposition", "attachment;filename=" + file.name);
                Response.BinaryWrite(file.content);
                Response.Flush();
                Response.End();
            }
        }

        private FileDocumento GenerateReport(String contextName, String reportKey, String title, String subtitle, ReportTypeEnum reportType, FiltroRicerca[] filters)
        {
            PrintReportRequest request = new PrintReportRequest()
            {
                ContextName = contextName,
                ReportKey = reportKey,
                ReportType = reportType,
                SubTitle = subtitle,
                Title = title,
                SearchFilters = filters,
                UserInfo = UserManager.GetInfoUser(),
                AdditionalInformation = (DocumentManager.getSelectedRecord() != null &&
                                        DocumentManager.getInfoDocumento(DocumentManager.getSelectedRecord()) != null &&
                                        DocumentManager.getInfoDocumento(DocumentManager.getSelectedRecord()).segnatura != null)
                                        ? DocumentManager.getInfoDocumento(DocumentManager.getSelectedRecord()).segnatura : string.Empty

            };
            //generazione report da codice
            FileDocumento report = ReportingManager.GenerateReport(request);
            return report;
        }

        public class Cols
        {
            private string data;
            private string tipo;
            private string descrizione;
            private string dettaglio;
            private int chiave;
            private string erroreEsteso;

            public Cols(string data, string tipo, string descrizione, string dettaglio, int chiave, string erroreEsteso, bool visualizzaErroreEsteso)
            {
                this.data = data;
                this.tipo = tipo;
                this.descrizione = descrizione;
                this.dettaglio = dettaglio;
                this.chiave = chiave;
            }
            public string Data { get { return data; } }
            public string Tipo { get { return tipo; } }
            public string Descrizione { get { return descrizione; } }
            public string Dettaglio { get { return dettaglio; } }
            public int Chiave { get { return chiave; } }
            public string ErroreEsteso { get { return erroreEsteso; } }
        }

        
    }
}