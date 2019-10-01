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
using System.Collections.Generic;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using DocsPAWA.SiteNavigation;
using System.Linq;

namespace DocsPAWA.popup
{
    /// <summary>
    /// Summary description for notificheDocumento.
    /// </summary>
    public class notificheDocumento : DocsPAWA.CssPage
    {
        protected System.Web.UI.WebControls.Label lbl_titolo;
        protected System.Web.UI.WebControls.Label lbl_message;
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.TextBox TextBox1;
        protected System.Web.UI.WebControls.Label Label2;
        protected System.Web.UI.WebControls.TextBox Textbox2;
        protected System.Web.UI.WebControls.Label Label3;
        protected System.Web.UI.WebControls.TextBox Textbox3;
        protected System.Web.UI.WebControls.Label Label4;
        protected System.Web.UI.WebControls.TextBox Textbox4;
        protected System.Web.UI.WebControls.Label Label7;
        protected System.Web.UI.WebControls.DataGrid DataGrid1;
        protected System.Web.UI.WebControls.Button btn_chiudi;
        protected static int currentPage;
        protected DocsPaWR.ProtocolloDestinatario[] dest;
        protected DocsPaWR.Corrispondente corr;
        protected DocsPaWR.Corrispondente[] allCorr;
        protected DocsPaWR.SchedaDocumento schedaDocumento;
        string tipoCor;
        string indexCor;
        protected ArrayList Dg_elem;
        protected HtmlControl reportContent;
        protected int numTotPage;
        protected System.Web.UI.WebControls.Button btnApplyFilter;
        protected System.Web.UI.WebControls.CheckBoxList cboFilterType;
        protected System.Web.UI.WebControls.TextBox txtFilterCodiceIS;
        protected System.Web.UI.WebControls.CheckBoxList cbxMail;
        protected System.Web.UI.WebControls.CheckBox cbxSelDes;

        
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

        private void Page_Load(object sender, System.EventArgs e)
        {
            // Inizializzazione della pagina
            Utils.startUp(this);
            // Put user code to initialize the page here
            if (!Page.IsPostBack)
            {
                //seleziona tutti i filtri
                cbxSelDes.Checked = true;
                foreach (ListItem i in cboFilterType.Items)
                {
                    i.Selected = true;
                }
                ConfigureFilterInUI();
                DocumentManager.removeDataGridNotifiche(this.Page);
                getDatiNotifica();
            }
        }

        private void getDatiNotifica()
        {
            //carica_Info
            tipoCor = Request.QueryString["tipoCor"];
            

            indexCor = Request.QueryString["indexCor"];
            schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
            bool visualizzaNotifichePec = false;

            if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_NOTIFICHE_PEC)) && bool.Parse(ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_NOTIFICHE_PEC)))
                visualizzaNotifichePec = true;


            //modifica
            DocsPaWR.Notifica[] notifica = null;
            if (visualizzaNotifichePec)
            {
                if (schedaDocumento != null &&
                    !string.IsNullOrEmpty(schedaDocumento.docNumber))
                {
                    notifica = InteroperabilitaManager.getNotifica(schedaDocumento.docNumber);
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

            // PALUMBO: se clicco sull'icona delle ricevute del 1° pannello voglio le ricevute sia dei dest principali che di quelli in conoscenza
            if (tipoCor.Equals("T"))
            {
                //prende tutti i corrispondenti e per ognuno cerca i dati di interop
                Dg_elem = new ArrayList();
                for (int i = 0; i < ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Length; i++)
                {
                    corr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[i];
                    if (corr != null && corr.systemId != null)
                        dest = DocumentManager.getDestinatariInteropAggConferma(this, schedaDocumento.systemId, corr);
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
                            dest = DocumentManager.getDestinatariInteropAggConferma(this, schedaDocumento.systemId, corr);
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
                    Dg_elem = DocumentManager.getDataGridNotifiche(Page);
                }

                if (Dg_elem.Count > 0)
                {
                    this.DataGrid1.DataSource = Dg_elem;
                    this.DataGrid1.DataBind();
                }
                else
                {
                    this.lbl_message.Text = "Ricevute non trovate";
                    this.DataGrid1.Visible = false;
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
                        dest = DocumentManager.getDestinatariInteropAggConferma(this, schedaDocumento.systemId, corr);


                    if ((dest == null || dest.Length == 0)
                        && (notifica == null || notifica.Length == 0))
                    {
                        this.lbl_message.Text = "Ricevute non trovate";
                        this.DataGrid1.Visible = false;
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
                                dest = DocumentManager.getDestinatariInteropAggConferma(this, schedaDocumento.systemId, c);
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
                    Dg_elem = DocumentManager.getDataGridNotifiche(Page);
                }
                

                if (Dg_elem.Count > 0)
                {
                    this.DataGrid1.DataSource = Dg_elem;
                    this.DataGrid1.DataBind();
                }
                else
                {
                    this.lbl_message.Text = "Ricevute non trovate";
                    this.DataGrid1.Visible = false;
                    this.lbl_message.Visible = true;
                    return;
                }
                
            }

        }

        private void visualizzaDati()
        {
            this.DataGrid1.VirtualItemCount = 2;
            this.DataGrid1.CurrentPageIndex = currentPage - 1;
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
                DocumentManager.setDataGridNotifiche(this, (ArrayList)Dg_elem);
            }

        }

        public void BindGridNotifichePecUpd(DocsPaWR.Notifica[] notifica)
        {
            // Recupero dei destinatari del protocollo in uscita
            //Corrispondente[] receivers = ((ProtocolloUscita)DocumentManager.getDocumentoSelezionato().protocollo).destinatari;

            //popolo il datagrid con le ricevute e le notifiche di annullamento
            BindGridRicevuteUpd();
            for (int i = 0; i < notifica.Length; i++)
            {
                bool agg_notifica_is = false;
                string infoNotifica = string.Empty;
                string nonCertificata = "MAIL NON CERTIFICATA";

                // Recupero del destinario con codice uguale a quello del destinatario della notifica
                //Corrispondente receiver = receivers.Where(r => r.codiceRubrica.Contains(notifica[i].destinatario.Replace("'", ""))).FirstOrDefault();

                DocsPaWR.TipoNotifica tiponotifica = InteroperabilitaManager.getTipoNotifica(notifica[i].idTipoNotifica);
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

                string descrizioneNotifica = String.Empty;
                //if (receiver != null && receiver.canalePref.tipoCanale == InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId)
                //    descrizioneNotifica = tiponotifica.descrizioneNotifica.Replace("email di ", String.Empty) + (!notifica[i].erroreRicevuta.ToUpper().Equals("NESSUNO") || !string.IsNullOrEmpty(notifica[i].errore_esteso) ? " - " + notifica[i].erroreRicevuta : string.Empty);
                if (Uri.IsWellFormedUriString(notifica[i].mittente, UriKind.Absolute))
                    descrizioneNotifica = tiponotifica.codiceNotifica.Equals("errore-consegna") ? "mancata-consegna" : tiponotifica.codiceNotifica;
                // PALUMBO: introdotto questo controllo su richiesta di Solda post collaudo 2.17 per visualizzare nella prima
                // colonna della popup ricevute di spedizione la dicitura mancata-consegna nel caso di mancata consegna o di preavviso di m.c.PEC  
                else if (tiponotifica.codiceNotifica.Contains("errore-consegna"))
                    descrizioneNotifica = "mancata-consegna";
                else
                    descrizioneNotifica = tiponotifica.descrizioneNotifica + (!notifica[i].erroreRicevuta.ToUpper().Equals("NESSUNO") || !string.IsNullOrEmpty(notifica[i].errore_esteso) ? " - " + notifica[i].erroreRicevuta : string.Empty);
                Cols col = new Cols(notifica[i].data_ora, descrizioneNotifica, notifica[i].destinatario, infoNotifica, 1, notifica[i].errore_esteso, true);
                //se ho selezionato da tabProtocollo tutti i destinatari devo eseguire i controlli per il monocasella
                if (string.IsNullOrEmpty(Request.QueryString["indexCor"]) || Request.QueryString["indexCor"].Equals("-1") &&
                    ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari != null)
                {
                    bool aggiunto = false;

                    foreach (DocsPaWR.Corrispondente c in ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari)
                    {
                        if ((from m in c.Emails where m.Email.ToUpper().Equals(notifica[i].destinatario.ToUpper()) select m).Count() > 0)
                        {
                            Dg_elem.Add(col);
                            aggiunto = true;
                            break;
                        }
                    }
                    if (!aggiunto)
                    {
                        string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                        if (!System.Text.RegularExpressions.Regex.Match(notifica[i].destinatario, pattern).Success)
                        {
                            Dg_elem.Add(col);
                            agg_notifica_is = true;
                        }
                    }
                }
                if (string.IsNullOrEmpty(Request.QueryString["indexCor"]) || Request.QueryString["indexCor"].Equals("-1") &&
                    ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza != null)
                {
                    bool aggiunto = false;

                    foreach (DocsPaWR.Corrispondente c in ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza)
                    {
                        if ((from m in c.Emails where m.Email.ToUpper().Equals(notifica[i].destinatario.ToUpper()) select m).Count() > 0)
                        {
                            Dg_elem.Add(col);
                            aggiunto = true;
                            break;
                        }
                    }
                    if ((!aggiunto) && (!agg_notifica_is))
                    {
                        string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                        if (!System.Text.RegularExpressions.Regex.Match(notifica[i].destinatario, pattern).Success)
                        {
                            Dg_elem.Add(col);
                        }
                    }
                }
                string appo = Request.QueryString["indexCor"];
                if ((appo != null) && (appo != "-1"))
                {
                    Dg_elem.Add(col);
                }
            }
            this.DataGrid1.DataSource = Dg_elem;
            this.DataGrid1.CurrentPageIndex = 0;
            DocumentManager.setDataGridNotifiche(this, Dg_elem);

            if (Dg_elem.Count > 0 && !this.DataGrid1.Visible)
                this.DataGrid1.Visible = true;
            this.DataGrid1.DataBind();
        }

        //Aggiorna la lista delle ricevute
        public void BindGridRicevuteUpd()
        {
            tipoCor = Request.QueryString["tipoCor"];
            indexCor = Request.QueryString["indexCor"];
            schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
            Dg_elem = new ArrayList();
            if (!string.IsNullOrEmpty(indexCor) && (!indexCor.Equals("-1")))
            {
                if (tipoCor.Equals("D"))
                {
                    if (!string.IsNullOrEmpty(indexCor) && Int32.Parse(indexCor) >= 0)
                        corr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[Int32.Parse(indexCor)];
                }

                // PALUMBO: aggiunto controllo per verifica su selezione destinatario in CC
                if (tipoCor.Equals("C"))
                {
                    if (!string.IsNullOrEmpty(indexCor) && Int32.Parse(indexCor) >= 0)
                        corr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[Int32.Parse(indexCor)];
                }


                if (corr != null && corr.systemId != null)
                {
                    dest = DocumentManager.getDestinatariInteropAggConferma(this, schedaDocumento.systemId, corr);
                    string filterDest = string.Empty;
                    if (dest != null && dest.Length > 0)
                    {
                        //if (corr.email.IndexOf(FilterDestinatario.valore) > -1 || corr.codiceRubrica.ToLower().IndexOf(FilterDestinatario.valore.ToLower()) > -1)
                        BindGrid(dest[0]);
                    }
                }
            }

            else
            {
                //if (tipoCor.Equals("D"))
                //{
                //    allCorr = ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
                //}
                //if (tipoCor.Equals("C"))
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
                    foreach (DocsPaWR.Corrispondente corr in allCorr)
                    {
                        if (corr.systemId != null)
                        {
                            dest = DocumentManager.getDestinatariInteropAggConferma(this, schedaDocumento.systemId, corr);

                            if (dest != null && dest.Length > 0)
                            {
                                BindGrid(dest[0]);
                            }
                        }
                    }
                }
            }
        }

        public void BindGridNotifichePec(DocsPaWR.Notifica[] notifica)
        {
            //Costruisco il datagrid
            ArrayList listaElementiPrecedenti = DocumentManager.getDataGridNotifiche(Page);

            if (notifica != null)
            {
                for (int i = 0; i < notifica.Length; i++)
                {
                    bool agg_notifica_is = false;
                    string infoNotifica = string.Empty;
                    string nonCertificata = "MAIL NON CERTIFICATA";

                    DocsPaWR.TipoNotifica tiponotifica = InteroperabilitaManager.getTipoNotifica(notifica[i].idTipoNotifica);


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

                    string descrizioneNotifica = String.Empty;
                    
                    if (Uri.IsWellFormedUriString(notifica[i].mittente, UriKind.Absolute))
                        descrizioneNotifica = tiponotifica.codiceNotifica.Equals("errore-consegna") ? "mancata-consegna" : tiponotifica.codiceNotifica;
                    // PALUMBO: introdotto questo controllo su richiesta di Solda post collaudo 2.17 per visualizzare nella prima
                    // colonna della popup ricevute di spedizione la dicitura mancata-consegna nel caso di mancata consegna o di preavviso di m.c.PEC  
                    else if (tiponotifica.codiceNotifica.Contains("errore-consegna"))
                        descrizioneNotifica = "mancata-consegna";
                    else
                        descrizioneNotifica = tiponotifica.descrizioneNotifica + (!notifica[i].erroreRicevuta.ToUpper().Equals("NESSUNO") || !string.IsNullOrEmpty(notifica[i].errore_esteso) ? " - " + notifica[i].erroreRicevuta : string.Empty);


                    if (listaElementiPrecedenti != null && listaElementiPrecedenti.Count > 0)
                    {
                        Cols col = new Cols(notifica[i].data_ora, descrizioneNotifica, notifica[i].destinatario, infoNotifica, 1, notifica[i].errore_esteso, true);
                        bool aggiungi = true;
                        //bool daRicercareInCC = false;

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
                                agg_notifica_is = true;
                            }


                            if (aggiungi)
                                listaElementiPrecedenti.Add(new Cols(notifica[i].data_ora, descrizioneNotifica, notifica[i].destinatario, infoNotifica, 1, notifica[i].errore_esteso, true));
                        }
                        // PALUMBO: se ho selezionato da tabProtocollo tutti i destinatari devo eseguire i controlli per il monocasella anche per destinatari in conoscenza
                        if ((aggiungi && string.IsNullOrEmpty(Request.QueryString["indexCor"])) || Request.QueryString["indexCor"].Equals("-1") &&
                            ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza != null )
                        {
                            aggiungi = false;
                            foreach (DocsPaWR.Corrispondente c in ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza)
                            {
                                if ((from m in c.Emails where m.Email.ToUpper().Equals(notifica[i].destinatario.ToUpper()) select m).Count() > 0)
                                {
                                    aggiungi = true;
                                    break;
                                }
                            }
                            string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                            if ((!System.Text.RegularExpressions.Regex.Match(notifica[i].destinatario, pattern).Success) && agg_notifica_is == false)
                            {
                                aggiungi = true;
                            }

                            if (aggiungi)
                                listaElementiPrecedenti.Add(new Cols(notifica[i].data_ora, descrizioneNotifica, notifica[i].destinatario, infoNotifica, 1, notifica[i].errore_esteso, true));

                        }
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
                                agg_notifica_is = true;
                            }
                            if (aggiungi)
                                Dg_elem.Add(col);
                        }

                        //se ho selezionato da tabProtocollo tutti i destinatari devo eseguire i controlli per il monocasella -- PALUMBO: aggiunto controllo per destinatari in conoscenza
                        if (aggiungi && string.IsNullOrEmpty(Request.QueryString["indexCor"]) || Request.QueryString["indexCor"].Equals("-1") &&
                            ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza != null)
                        {
                            aggiungi = false;
                            foreach (DocsPaWR.Corrispondente c in ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza)
                            {
                                if ((from m in c.Emails where m.Email.ToUpper().Equals(notifica[i].destinatario.ToUpper()) select m).Count() > 0)
                                {
                                    aggiungi = true;
                                    break;
                                }
                            }
                            string pattern = "^(([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+([;](([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5}){1,25})+)*$";
                            if ((!System.Text.RegularExpressions.Regex.Match(notifica[i].destinatario, pattern).Success) && agg_notifica_is == false)
                            {
                                aggiungi = true;
                            }
                            if (aggiungi)
                                Dg_elem.Add(col);
                        }
                        //if (aggiungi)
                        //    Dg_elem.Add(col);
                    }
                }
            }

            if (listaElementiPrecedenti != null &&
                listaElementiPrecedenti.Count > 0)
                DocumentManager.setDataGridNotifiche(this, (ArrayList)listaElementiPrecedenti);
            else
                DocumentManager.setDataGridNotifiche(this, (ArrayList)Dg_elem);
        }

        public void BindGridNotifichePec(DocsPaWR.Notifica[] notifica, DocsPaWR.Corrispondente corr, string tutti)
        {
            //Costruisco il datagrid
            ArrayList listaElementiPrecedenti = DocumentManager.getDataGridNotifiche(Page);

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

                    DocsPaWR.TipoNotifica tiponotifica = InteroperabilitaManager.getTipoNotifica(notifica[i].idTipoNotifica);
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
                     
                    // PALUMBO: introdotto questo controllo su richiesta di Solda post collaudo 2.17 per visualizzare nella prima
                    // colonna della popup ricevute di spedizione la dicitura mancata-consegna nel caso di mancata consegna o di preavviso di m.c.PEC  
                    if (tiponotifica.codiceNotifica.Contains("errore-consegna"))
                        descrizioneNotifica = "mancata-consegna";
                    else
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
                DocumentManager.setDataGridNotifiche(this, (ArrayList)listaElementiPrecedenti);
            else
                DocumentManager.setDataGridNotifiche(this, (ArrayList)Dg_elem);
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

                        DocsPaWR.TipoNotifica tiponotifica = InteroperabilitaManager.getTipoNotifica(notifica[i].idTipoNotifica);
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

                        string descrizioneNotifica = String.Empty;

                        if (Uri.IsWellFormedUriString(notifica[i].mittente, UriKind.Absolute))
                            descrizioneNotifica = tiponotifica.codiceNotifica.Equals("errore-consegna") ? "mancata-consegna" : tiponotifica.codiceNotifica;
                        // PALUMBO: introdotto questo controllo su richiesta di Solda post collaudo 2.17 per visualizzare nella prima
                        // colonna della popup ricevute di spedizione la dicitura mancata-consegna nel caso di mancata consegna o di preavviso di m.c.PEC  
                        else if (tiponotifica.codiceNotifica.Contains("errore-consegna"))
                            descrizioneNotifica = "mancata-consegna";
                        else
                            descrizioneNotifica = tiponotifica.descrizioneNotifica + (!notifica[i].erroreRicevuta.ToUpper().Equals("NESSUNO") || !string.IsNullOrEmpty(notifica[i].errore_esteso) ? " - " + notifica[i].erroreRicevuta : string.Empty);
                        
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
            DocumentManager.setDataGridNotifiche(this, (ArrayList)Dg_elem);
        }

        private void Datagrid1_pager(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {

            this.DataGrid1.CurrentPageIndex = e.NewPageIndex;
            ArrayList ColNew = new ArrayList();
            ColNew = (ArrayList)DocumentManager.getDataGridNotifiche(this);

            DataGrid1.DataSource = ColNew;
            DataGrid1.DataBind();
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
            this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
            this.btnApplyFilter.Click += new System.EventHandler(this.btnApplyFilter_Click);
            this.cbxSelDes.CheckedChanged += new EventHandler(cbxSelDes_CheckedChanged);
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion

        private void btn_chiudi_Click(object sender, System.EventArgs e)
        {
            //svuoto i filtri in sessione
            FilterIdDoc = null;
            FilterTipo = null;
            FilterDestinatario = null;
            Response.Write("<script>window.close();</script>");
        }

        protected void Datagrid1_SelectedPageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {

            this.DataGrid1.CurrentPageIndex = e.NewPageIndex;
            ArrayList ColNew = new ArrayList();
            ColNew = (ArrayList)DocumentManager.getDataGridNotifiche(this);

            DataGrid1.DataSource = ColNew;
            DataGrid1.DataBind();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            tipoCor = Request.QueryString["tipoCor"];
            indexCor = Request.QueryString["indexCor"];
            schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
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
                FilterIdDoc = new FiltroRicerca { argomento = "filterIdDoc", valore = DocumentManager.getDocumentoInLavorazione(this).docNumber };
            
            if (string.IsNullOrEmpty(indexCor) || indexCor.Equals("-1"))            
            {
                // nel caso di seleziona tutti devo verificare se è attivo il monocasella ed eventualmente considerare solo la mail principale
                if (!utils.MultiCasellaManager.IsEnabledMultiMail(UserManager.getRuolo().idAmministrazione))
                {
                    System.Text.StringBuilder strBuild = new System.Text.StringBuilder();
                    foreach (DocsPaWR.Corrispondente c in ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari)
                    {
                        if (c.Emails != null && c.Emails.Length > 0)
                            strBuild.Append(c.Emails[0].Email + "#");
                    }
                    if (strBuild.Length > 0)
                        FilterDestinatario = new FiltroRicerca { argomento = "filterDest", valore = strBuild.ToString().ToLower() };
                }
            }
            else if (cbxMail.Items != null && cbxMail.Items.Count > 0)
            {
                System.Text.StringBuilder strBuild = new System.Text.StringBuilder();
                foreach (ListItem m in cbxMail.Items)
                {
                    if (m.Selected)
                        strBuild.Append(m.Value + "#");
                }
                if (strBuild.Length < 1)
                {
                    foreach (ListItem m in cbxMail.Items)
                    {
                        m.Selected = true;
                        strBuild.Append(m.Value + "#");
                    }
                }
                FilterDestinatario = new FiltroRicerca { argomento = "filterDest", valore = strBuild.ToString().ToLower() };
            }

            FilterCodiceIS = new FiltroRicerca { argomento = "filterCodiceIS", valore = txtFilterCodiceIS.Text };

            FileDocumento file = GenerateReport("NotifichePec", "NotificheSpedizione", "Ricevute di spedizione", "", ReportTypeEnum.Excel,
                new FiltroRicerca[] { FilterIdDoc, FilterTipo, FilterDestinatario, FilterCodiceIS, FilterExportRicevuteInterop });           
            if (file != null)
            {
                CallContextStack.CurrentContext.ContextState["documentFile"] = file;
                this.reportContent.Attributes["src"] = "Reporting/ReportContent.aspx";
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
                UserInfo = UserManager.getInfoUtente(),
                AdditionalInformation = (DocumentManager.getDocumentoSelezionato() != null &&
                                        DocumentManager.getInfoDocumento(DocumentManager.getDocumentoSelezionato()) != null &&
                                        DocumentManager.getInfoDocumento(DocumentManager.getDocumentoSelezionato()).segnatura != null)
                                        ? DocumentManager.getInfoDocumento(DocumentManager.getDocumentoSelezionato()).segnatura : string.Empty

            };
            //generazione report da codice
            FileDocumento report = ReportingUtils.GenerateReport(request);
            return report;
        }

        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            //se non ho applicato alcun filtro, allora li seleziono tutti
            bool checkedAll = true;
            foreach (ListItem i in cboFilterType.Items)
            {
                if (i.Selected)
                    checkedAll = false;
            }
            if (checkedAll)
            {
                foreach (ListItem i in cboFilterType.Items)
                    i.Selected = true;
                cbxSelDes.Checked = true;
            }
            

            FilterIdDoc = new FiltroRicerca { argomento = "filterIdDoc", valore = DocumentManager.getDocumentoInLavorazione(this).docNumber };
            //filtro codice corrispondente(per destinatari IS)
            FilterCodiceIS = new FiltroRicerca { argomento = "filterCodiceIS", valore = txtFilterCodiceIS.Text };
            System.Text.StringBuilder strBuild = new System.Text.StringBuilder();

            //Filtro per elenco mail (per destinatai interoperabilità/mail)
            if (cbxMail.Items != null && cbxMail.Items.Count > 0)
            {
                foreach (ListItem m in cbxMail.Items)
                {
                    if (m.Selected)
                        strBuild.Append(m.Value + "#");
                }
                if (strBuild.Length < 1)
                {
                    foreach (ListItem m in cbxMail.Items)
                    {
                        m.Selected = true;
                        strBuild.Append(m.Value + "#");
                    }
                }
                FilterDestinatario = new FiltroRicerca { argomento = "filterDest", valore = strBuild.ToString().ToLower() };
            }
            else
            {
                // nel caso di seleziona tutti devo verificare se è attivo il monocasella ed eventualmente considerare solo la mail principale
                if (!utils.MultiCasellaManager.IsEnabledMultiMail(UserManager.getRuolo().idAmministrazione))
                {
                    schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
                    foreach (DocsPaWR.Corrispondente c in ((DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari)
                    {
                        if (c.Emails != null && c.Emails.Length > 0)
                            strBuild.Append(c.Emails[0] + "#");
                    }
                    if (strBuild.Length > 0)
                        FilterDestinatario = new FiltroRicerca { argomento = "filterDest", valore = strBuild.ToString().ToLower() };
                    else
                        FilterDestinatario = new FiltroRicerca { argomento = "filterDest", valore = string.Empty };
                }
                else
                    FilterDestinatario = new FiltroRicerca { argomento = "filterDest", valore = string.Empty };
            }

            string selected = string.Empty;
            foreach (ListItem item in cboFilterType.Items)
            {
                if (item.Selected)
                {
                    selected += item.Value + "#";
                }
            }
            FilterTipo = new FiltroRicerca { argomento = "filterTipo", valore = selected };
            //filtro conferma ricezione
            if (string.IsNullOrEmpty(FilterTipo.valore) || FilterTipo.valore.IndexOf("ricevuta") > -1)
                FilterRicevute = new FiltroRicerca { argomento = "filterRicevuta", valore = "1" };
            else
                FilterRicevute = new FiltroRicerca { argomento = "filterRicevuta", valore = "0" };
            //filtro annullamento protocollo
            if (string.IsNullOrEmpty(FilterTipo.valore) || FilterTipo.valore.IndexOf("annulla") > -1)
                FilterAnnulla = new FiltroRicerca { argomento = "filterAnnulla", valore = "1" };
            else
                FilterAnnulla = new FiltroRicerca { argomento = "filterAnnulla", valore = "0" };

            //aggiorna il datagrid tenendo conto dei filtri inseriti
            Notifica[] notifica = InteroperabilitaManager.getNotificaFiltra(new FiltroRicerca[] { FilterIdDoc, FilterTipo, FilterDestinatario, FilterCodiceIS });            
            BindGridNotifichePecUpd(notifica);
        }

        /// <summary>
        /// gestione selezione/deselezione dei filtri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbxSelDes_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = (sender as CheckBox);
            if (check.Checked)
            {
                foreach (ListItem item in cboFilterType.Items)
                    item.Selected = true;
            }
            else
            {
                foreach (ListItem item in cboFilterType.Items)
                    item.Selected = false;
            }
        }

        /// <summary>
        /// Setta il filtro per le Email ed il codice corr(dest IS) e li rende disponibili nella UI
        /// </summary>
        private void ConfigureFilterInUI()
        {
            FilterCodiceIS = new FiltroRicerca { argomento = "filterCodiceIS", valore = string.Empty };
            FilterDestinatario = new FiltroRicerca { argomento = "filterDest", valore = string.Empty };
            string index = Request.QueryString["indexCor"];
            string tipo = Request.QueryString["tipoCor"];
            SchedaDocumento schDoc = DocumentManager.getDocumentoInLavorazione(this);
            //se è stato selezionato un destinatario allora imposto i filtri per il codice corr(IS) e per l'elenco mail
            if (!string.IsNullOrEmpty(index) && (!index.Equals("-1")))
            {
                if (tipo.Equals("D"))
                {
                    if (!string.IsNullOrEmpty(index) && Int32.Parse(index) >= 0)
                        corr = ((DocsPaWR.ProtocolloUscita)schDoc.protocollo).destinatari[Int32.Parse(index)];
                }
                else if (tipo.Equals("C"))
                {
                    if (!string.IsNullOrEmpty(index) && Int32.Parse(index) >= 0)
                        corr = ((DocsPaWR.ProtocolloUscita)schDoc.protocollo).destinatariConoscenza[Int32.Parse(index)];
                }
            }
            if (corr != null) // se si è scelto un destinatario imposto il filtro su quest'ultimo.
            {
                //PALUMBO: workaround per consentire la visualizzazione delle notifiche quando erroneamente arriva un typeid==S
                if ((corr.canalePref.typeId == InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId) || (corr.canalePref.typeId == "S"))
                {
                    this.txtFilterCodiceIS.Text = corr.codiceRubrica.Replace(String.Format("_{0}", corr.systemId), String.Empty);
                    FilterCodiceIS = new FiltroRicerca { argomento = "filterCodiceIS", valore = this.txtFilterCodiceIS.Text };
                    (FindControl("divCodiceCorr") as HtmlGenericControl).Style.Remove("display");
                    (FindControl("divCodiceCorr") as HtmlGenericControl).Style.Add("display", "block");
                }
                if (!string.IsNullOrEmpty(corr.email))
                {
                    System.Text.StringBuilder filterDest = new System.Text.StringBuilder();
                    //recupero tutte le mail associate al corrispondente e le visualizzo nella UI
                    foreach (MailCorrispondente m in (from m in corr.Emails orderby m.Principale descending select m))
                    {
                        cbxMail.Items.Add(new ListItem { Text = m.Principale.Equals("1") ? "* " + m.Email : m.Email, Value = m.Email, Selected = true });
                        filterDest.Append(m.Email.ToLower().Trim() + "#");
                    }
                    //Imposto il filtro per le email(le seleziono tutte)
                    /* FilterDestinatario = new FiltroRicerca
                     {
                         argomento = "filterDest",
                         valore = filterDest.ToString()
                     };*/
                    (FindControl("divMail") as HtmlGenericControl).Style.Remove("display");
                    (FindControl("divMail") as HtmlGenericControl).Style.Add("display", "inline");
                }
            }
        }
    }
}
