using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;
using System;
using System.Xml.Serialization;
using System.Web.Services.Protocols;
using Microsoft.Web.UI.WebControls;
using System.Globalization;
using System.Configuration;
using log4net.Repository.Hierarchy;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.fascicolo
{
    /// <summary>
    /// Summary description for fascDocumenti.
    /// </summary>
    public class fascDocumenti : DocsPAWA.CssPage
    {
        protected Note.DettaglioNota dettaglioNota;
        protected System.Web.UI.WebControls.TextBox txt_tipoFascicolo;
        protected System.Web.UI.WebControls.ImageButton img_btnDettagliProf;
        protected System.Web.UI.WebControls.ImageButton img_cercaSottoFasc;
        protected System.Web.UI.WebControls.Panel pnl_profilazione;
        protected System.Web.UI.WebControls.DropDownList ddl_tipologiaFasc;
        protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
        protected DocsPAWA.DocsPaWR.Registro userReg;
        protected DocsPAWA.DocsPaWR.Utente userHome;
        protected DocsPAWA.DocsPaWR.Registro fascRegistro;
        protected DocsPAWA.DocsPaWR.Ruolo fascRuolo;
        protected System.Web.UI.WebControls.Panel Panel_DiagrammiStato;
        //protected fascicoloWR.fascicoloWS Fasc;
        protected DocsPaWR.Templates template;
        public DocsPAWA.DocsPaWR.FascicolazioneClassifica[] FascClass;

        public DocsPAWA.DocsPaWR.Fascicolo Fasc;
        public string paramClass;
        protected DocsPAWA.DocsPaWR.Utente utente;
        protected DocsPAWA.DocsPaWR.Ruolo ruolo;
        //protected Utilities.MessageBox  msg_StatoAutomatico;
        //protected Utilities.MessageBox msg_StatoFinale;

        protected System.Web.UI.WebControls.ImageButton btn_rubrica_ref;
        protected DocsPAWA.DocsPaWR.Trasmissione trasmissione;
        protected Utilities.MessageBox msg_StatoAutomatico;
        protected Utilities.MessageBox msg_StatoFinale;
        protected DocsPAWA.DocsPaWR.Corrispondente corrRef;
        protected DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
        protected Hashtable HashFolder;
        protected Microsoft.Web.UI.WebControls.TreeView Folders;
        protected System.Web.UI.WebControls.Label Label7;
        protected string ger = "";
        public int indFasc;
        // protected DocsPAWA.DocsPaWR.Fascicolo Fasc;
        protected DocsPAWA.DocsPaWR.InfoUtente infoUt;
        protected System.Web.UI.WebControls.Label Label5;
        protected System.Web.UI.WebControls.Label Label6;
        protected System.Web.UI.WebControls.Label Label8;
        protected System.Web.UI.WebControls.TextBox txt_Fasctipo;
        protected System.Web.UI.WebControls.TextBox txt_fascStato;
        protected System.Web.UI.WebControls.TextBox txt_fascnote;
        protected System.Web.UI.WebControls.Label Label9;
        protected System.Web.UI.WebControls.TextBox Textbox2;
        private int indexH;
        protected System.Web.UI.WebControls.TextBox txt_fascApertura;
        protected System.Web.UI.WebControls.TextBox txt_FascChiusura;
        protected DocsPaWebCtrlLibrary.ImageButton btn_modifica;
        protected System.Web.UI.WebControls.DropDownList ddl_statiSuccessivi;
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.Label lblTitolario;
        protected System.Web.UI.WebControls.Label lblClassifica;
        protected System.Web.UI.WebControls.TextBox txt_ClassFasc;
        protected System.Web.UI.WebControls.Label Label2;
        protected System.Web.UI.WebControls.Label lbl_tipoFasc;
        protected System.Web.UI.WebControls.TextBox txt_fascdesc;
        protected DocsPaWebCtrlLibrary.ImageButton btn_rimuovi;
        protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungi;
        protected DocsPaWebCtrlLibrary.ImageButton btn_chiudiRiapri;
        protected DocsPaWebCtrlLibrary.ImageButton btn_AnnullaModifiche;
        protected DocsPaWebCtrlLibrary.ImageButton btn_log;
        // protected Microsoft.Web.UI.WebControls.TreeView Gerarchia;
        protected DocsPaWebCtrlLibrary.ImageButton btn_addToAreaLavoro;
        protected DocsPaWebCtrlLibrary.ImageButton btn_Salva;
        protected DocsPaWebCtrlLibrary.ImageButton btn_modFolder;
        protected System.Web.UI.WebControls.Label lbl_dataAp;
        protected System.Web.UI.WebControls.Label lbl_dataC;
        protected DocsPaWebCtrlLibrary.ImageButton btn_visibilita;
        //   protected DocsPaWebCtrlLibrary.ImageButton btn_storia_visibilita;
        protected System.Web.UI.WebControls.Label Label3;
        protected System.Web.UI.WebControls.TextBox txt_descrizione;
        protected DocsPAWA.DocsPaWR.InfoFascicolo infoFasc;
        protected System.Web.UI.WebControls.Label Label12;
        protected DocsPAWA.UserControls.Corrispondente Corr;
        protected DocsPAWA.UserControls.Calendar txt_LFDTA;
        protected System.Web.UI.WebControls.TextBox txt_desc_uff_ref;
        protected System.Web.UI.WebControls.TextBox txt_cod_uff_ref;
        protected System.Web.UI.WebControls.CheckBox chkFascicoloCartaceo;
        protected System.Web.UI.WebControls.Label lblFascicoloCartaceo;
        protected System.Web.UI.WebControls.CheckBox chkFascicoloPrivato;
        protected System.Web.UI.WebControls.CheckBox chkFascicoloControllato;
        protected System.Web.UI.WebControls.Label lblFascicoloControllato;
        protected System.Web.UI.WebControls.ImageButton btn_rubrica;
        private DocsPAWA.DocsPaWR.Folder[] _foldersDocument = null;
        protected System.Web.UI.WebControls.Panel pnl_uff_ref;
        protected System.Web.UI.WebControls.TextBox txt_LFCod;
        protected System.Web.UI.WebControls.TextBox txt_LFDesc;
        protected System.Web.UI.WebControls.Label lblFascicoloPrivato;
        protected System.Web.UI.WebControls.Label ldb_locFis;
        protected System.Web.UI.WebControls.Label lbl_uffRef;
        protected System.Web.UI.WebControls.Panel pnl_locFis;
        protected DocsPaWebCtrlLibrary.ImageButton btn_stampaFascette;
        //protected DocsPaWebCtrlLibrary.ImageButton btn_exp_fasc;
        private bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
            && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;

        //  protected System.Web.UI.WebControls.Panel Panel_DiagrammiStato;
        protected System.Web.UI.WebControls.Panel Panel_DataScadenza;
        protected System.Web.UI.WebControls.Label lbl_statoFasc;
        protected System.Web.UI.WebControls.Label lbl_statoAttuale;
        protected DocsPaWebCtrlLibrary.ImageButton img_btnStoriaDiagrammi;
        protected System.Web.UI.WebControls.Label lbl_dataScadenza;
        protected System.Web.UI.WebControls.TextBox txt_dataScadenza;
        //protected System.Web.UI.WebControls.ImageButton btn_imp_doc;
        protected System.Web.UI.WebControls.ImageButton btn_storiaFasc;
        protected System.Web.UI.HtmlControls.HtmlInputControl clTesto;
        protected int caratteriDisponibili;

        protected DocsPaWebCtrlLibrary.ImageButton btnPrintSignature;
        protected System.Web.UI.WebControls.PlaceHolder ph_stampa;
        //protected StampaEtichetta StampaEtichetta;

        //
        // Mev Ospedale Maggiore Policlinico
        protected DocsPaWebCtrlLibrary.ImageButton btn_Riclassifica;
        private DocsPAWA.DocsPaWR.FascicolazioneClassificazione RiclassificazioneSelezionata;
        // End Mev
        //

        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                TrasmManager.removeGestioneTrasmissione(this);

                Utils.startUp(this);
                //this.Gerarchia.Attributes.Remove("oncheck");
                //this.Gerarchia.Attributes.Remove("onselectedindexchange");
                // Modifico i campi profilati solo se è selezionata una tipologia
                if (ddl_tipologiaFasc.Items.Count > 0 && ddl_tipologiaFasc.SelectedItem.Text != "")
                {
                    this.btn_Salva.Attributes.Add("onclick", "UpdateCampiProfilati();");

                }
                userRuolo = UserManager.getRuolo(this);
                userReg = UserManager.getRegistroSelezionato(this);
                userHome = UserManager.getUtente(this);
                infoUt = UserManager.getInfoUtente(this);


                if (!Page.IsPostBack)
                {
                    //Inserimento Christian
                    if (this.Page.Session["fascDocumenti.nodoSelezionato"] != null)
                    {
                        //Session.Remove("fascDocumenti.nodoSelezionato");
                        //Session.Remove("fascDocumenti.HashFolder");
                        DocumentManager.removeDocumentoInLavorazione();
                        DocumentManager.removeDocumentoSelezionato();
                    }

                    Page.MaintainScrollPositionOnPostBack = true;
                    DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
                    info = UserManager.getInfoUtente(this.Page);


                    string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_DESC_FASC");
                    if (valoreChiave != string.Empty)
                        caratteriDisponibili = int.Parse(valoreChiave);

                    txt_descrizione.MaxLength = caratteriDisponibili;
                    clTesto.Value = caratteriDisponibili.ToString();
                    txt_descrizione.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','DESCRIZIONE'," + clTesto.ClientID + ")");
                    txt_descrizione.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','DESCRIZIONE'," + clTesto.ClientID + ")");
                    // Creazione di una copia locale del fascicolo, utilizzata solo
                    // per apportare modifiche nell'ambito del dettaglio
                    //if (this.FascicoloInLavorazione == null)
                    //{
                    this.FascicoloInLavorazione = this.CloneFascicolo(FascicoliManager.getFascicoloSelezionato(this));

                    //Causa modifiche per velocizzare ricerca fascicoli sono costretto a fare il get completo dei dati
                    if (this.FascicoloInLavorazione != null)
                    {
                        this.FascicoloInLavorazione = FascicoliManager.getFascicoloById(this, this.FascicoloInLavorazione.systemID);
                        FascicoliManager.setFascicoloSelezionato(this.FascicoloInLavorazione);
                    }
                    //}
                    // Al dettaglio delle note viene impostata la chiave di sessione del fascicolo in lavorazione
                    this.dettaglioNota.ContainerSessionKey = FASCICOLO_IN_LAVORAZIONE_SESSION_KEY;
                    this.dettaglioNota.Fetch();
                    HashFolder = new Hashtable();






                }
                //Fasc=new DocsPAWA.DocsPaWR.Fascicolo();
                if (Fasc == null)
                {
                    Fasc = this.FascicoloInLavorazione;
                    Fasc.template = (DocsPaWR.Templates)Session["template"];
                }

                // Inizializzazione controllo verifica acl
                if ((Fasc != null) && (Fasc.systemID != null))
                    this.InitializeControlAclFascicolo();

                //se arrivo dalla ricerca trasm.
                if (Fasc == null)
                {
                    infoFasc = FascicoliManager.getInfoFascicolo(this);
                    Fasc = FascicoliManager.getFascicolo(this, infoUt.idGruppo, infoUt.idPeople, infoFasc);
                }

                //Profilazione dinamica fascicoli
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1" && Fasc.tipo.Equals("P"))
                {
                    pnl_profilazione.Visible = true;
                    Fasc.template = ProfilazioneFascManager.getTemplateFascDettagli(Fasc.systemID, this);

                    if (Fasc.template != null)
                    {


                        FascicoliManager.setFascicoloSelezionato(Fasc);
                        if (!IsPostBack)
                        {
                            CaricaComboTipologiaFasc();
                            if (ddl_tipologiaFasc.Items.Count > 0 && Fasc.template.ID_TIPO_FASC.ToString() != "")
                            {
                                ddl_tipologiaFasc.SelectedValue = Fasc.template.ID_TIPO_FASC.ToString();
                                if (ddl_tipologiaFasc.SelectedItem.Text != "")
                                    Session["ListaDocs-CampiProf"] = "CampiProf";
                            }
                        }
                    }

                    if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                    {
                        //Diagrammi di stato - Stato del fascicolo
                        DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoFasc(Fasc.systemID, this);
                        if (stato != null)
                        {
                            lbl_statoAttuale.Text = stato.DESCRIZIONE;
                            Panel_DiagrammiStato.Visible = true;
                        }
                        // Data Scadenza
                        if (Fasc.dtaScadenza != null && Fasc.dtaScadenza != "")
                        {
                            txt_dataScadenza.Text = Fasc.dtaScadenza;
                            Panel_DataScadenza.Visible = true;
                        }

                    }
                }
                if (!this.IsPostBack)
                {
                    //if (Session["ListaDocs-CampiProf"] == null || Session["ListaDocs-CampiProf"].ToString() == "")
                    //{
                    if (ddl_tipologiaFasc.Items.Count > 0 && ddl_tipologiaFasc.SelectedItem.Text != "")
                    {
                        Session["ListaDocs-CampiProf"] = "CampiProf";
                    }
                    else
                    {
                        Session["ListaDocs-CampiProf"] = "ListaDocs";
                    }
                    // }
                }

                //Fine Profilazione dinamica fascicoli


                if (Fasc.tipo == "G")
                {
                    lblFascicoloPrivato.Visible = false;
                    chkFascicoloPrivato.Visible = false;
                }
                else
                {
                    lblFascicoloPrivato.Visible = true;
                    chkFascicoloPrivato.Visible = true;
                }

                if (Fasc != null && Fasc.systemID != null)
                {
                    int result = DocumentManager.verificaDeletedACL(this.Fasc.systemID, UserManager.getInfoUtente(this));
                    //0-->nessun diritto rimosso: icona visibilità normale, no icona storia
                    //1-->diritti rimossi: icona visibilità rossa, si icona storia
                    //2-->diritti rimossi ma ripristinati: icona visibilità normale, si icona storia
                    if (result > 0)
                    {
                        // btn_storia_visibilita.Visible = true;
                        if (result == 1)
                        {
                            btn_visibilita.BorderWidth = 1;
                            btn_visibilita.BorderColor = Color.Red;
                            //btn_visibilita.ImageUrl = "../images/proto/ico_visibilita_rimosso.gif";
                        }
                    }
                    else
                        btn_visibilita.BorderWidth = 0;
                    // else
                    //  btn_storia_visibilita.Visible = false;
                }
                if (!Page.IsPostBack)
                {


                    // Creazione di una copia locale del fascicolo, utilizzata solo
                    // per apportare modifiche nell'ambito del dettaglio
                    this.FascicoloInLavorazione = this.CloneFascicolo(FascicoliManager.getFascicoloSelezionato(this));

                    // Al dettaglio delle note viene impostata la chiave di sessione del fascicolo in lavorazione
                    this.dettaglioNota.ContainerSessionKey = FASCICOLO_IN_LAVORAZIONE_SESSION_KEY;
                    HashFolder = new Hashtable();

                    //ricaviamo la gerarchia in base alla classificazione selezionata dall'utente
                    FascClass = FascicoliManager.getGerarchia(this, Fasc.idClassificazione, UserManager.getUtente(this).idAmministrazione);

                    //carico la classificazione del fascicolo
                    caricaGerarchiaFascicolazioneClassifica(FascClass);

                    if (this.Page.Session["fascDocumenti.nodoSelezionato"] == null)
                    {
                        this.Page.Session["fascDocumenti.nodoSelezionato"] = getSelectedNodeFolder();
                    }

                    string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
                    if (use_new_rubrica != "1")
                        this.btn_rubrica.Attributes["onClick"] = "ApriRubrica('fascLF','U');";
                    else
                        this.btn_rubrica.Attributes["onClick"] = "_ApriRubrica('ef_locfisica');";



                    if (enableUfficioRef && !Fasc.tipo.Equals("G"))
                    {
                        this.pnl_uff_ref.Visible = true;
                        if (UserManager.ruoloIsAutorized(this, "DO_PROT_MODIFICA_UFF_REF"))
                        {
                          

                            // Inizializzazione condizionale link rubrica
                            if (use_new_rubrica != "1")
                                this.btn_rubrica_ref.Attributes["onClick"] = "ApriRubrica('fascUffRefMod','U');";
                            else
                                this.btn_rubrica_ref.Attributes["onClick"] = "_ApriRubrica('ef_uffref');";

                        }
                    }


                    if (!Fasc.tipo.Equals("G"))//se il fascicolo è generale non deve essere gestita la locazione fisica
                    {
                        this.pnl_locFis.Visible = true;
                    }
                    else
                    {
                        this.pnl_locFis.Visible = false;
                    }
                    //luluciani: per il fascicolo generale non deve poter essere modificabile la descrizione.
                    if (Fasc != null && Fasc.tipo.Equals("G"))
                    {
                        this.txt_descrizione.ReadOnly = true;
                    }
                    //bottone per l'inserimento di un oggetto
                    //string page = "'../popup/modificaFolder.aspx'";
                    //string titolo = "'Modifica_Folder'";
                    //string param = "'width=420,height=200, scrollbar=no'";
                    //this.btn_modFolder.Attributes.Add("onclick","ApriFinestraGen("+ page + ',' + titolo + ',' + param + ");");
                    // this.btn_modifica.Attributes.Add("onclick", "editFascFields('true')");
                    //controllo per i pulsanti
                    if (Fasc.tipo.Equals("P"))
                    {
                        this.btn_chiudiRiapri.Visible = true;
                        this.btn_visibilita.Visible = true;
                        // this.btn_storia_visibilita.Attributes.Add("onclick", "ApriFinestraStoriaVisibilita('F');return false;");
                        this.btn_visibilita.Attributes.Add("onclick", "ApriFinestraVisibilitaFasc();");

                        // l'import dei documenti massivi è attivo solo se selzionato un fascicolo di tipo P
                        //if ((System.Configuration.ConfigurationManager.AppSettings["IMPORT_MASSIVO"] != null) && (System.Configuration.ConfigurationManager.AppSettings["IMPORT_MASSIVO"] != ""))
                        //{
                        //    if (System.Configuration.ConfigurationManager.AppSettings["IMPORT_MASSIVO"] == "1")
                        //    {
                        //        if (UserManager.ruoloIsAutorized(this, "IMP_DOC_MASSIVA"))
                        //        {
                        //            this.btn_imp_doc.Visible = true;
                        //        }
                        //    }
                        //}


                        //luluciani   if (UserManager.ruoloIsAutorized(this, "ACL_RIMUOVI"))
                        //  {
                        //utente è autorizzato a rimuovere le acl per il fascicolo 
                        //verifica che ci siano ACL rimosse 
                        
                        //}
                        if (enableUfficioRef)
                        {
                            this.pnl_uff_ref.Visible = true;
                        }
                        this.pnl_locFis.Visible = true;
                    }
                    else
                    {
                        this.btn_chiudiRiapri.Visible = false;
                        this.btn_visibilita.Visible = false;
                        //   this.btn_storia_visibilita.Visible = false;
                        if (enableUfficioRef)
                        {
                            this.pnl_uff_ref.Visible = false;
                        }
                        this.pnl_locFis.Visible = false;
                    }

                }
                else
                {

                    #region modifica Fascicolo
                    // region aggiunta da Dimitri De Filippo per abilitare la modifica del fascicolo direttamente sulla pagina di visualizzazione
                    if (FascicoliManager.DO_VerifyFlagLF())
                    {
                        //FascicoliManager.DO_RemoveFlagLF();

                        DocsPaVO.LocazioneFisica.LocazioneFisica LF = FascicoliManager.DO_GetLocazioneFisica();
                        //						FascicoliManager.DO_RemoveLocazioneFisica();
                        if (LF != null)
                        {
                            txt_LFDesc.Text = LF.Descrizione;

                            if (LF.CodiceRubrica != null)
                            {
                                this.txt_LFCod.Text = LF.CodiceRubrica;

                                if (this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString().Trim() != "")
                                {
                                    LF.Data = this.GetCalendarControl("txt_LFDTA").txt_Data.Text;
                                }
                                else
                                {
                                    LF.Data = "";
                                }

                            }
                        }
                        FascicoliManager.DO_RemoveFlagLF();
                    }

                    if (FascicoliManager.DO_VerifyFlagUR() && enableUfficioRef)
                    {
                        DocsPaWR.Corrispondente uff_ref = FascicoliManager.getUoReferenteSelezionato(this);
                        if (uff_ref != null)
                        {
                            txt_desc_uff_ref.Text = uff_ref.descrizione;
                            if (uff_ref.codiceRubrica != null)
                                txt_cod_uff_ref.Text = uff_ref.codiceRubrica;
                        }
                        FascicoliManager.DO_RemoveFlagUR();
                    }



                    #endregion

                    this.Page.Session.Remove("fascDocumenti.nodoSelezionato");
                    HashFolder = FascicoliManager.getHashFolder(this);
                }

                //Seleziono il folder selezionato da ricerca sottofascicolo

                if (hd_returnValueModal.Value == "Y") //ritorno dalla modale di ricerca dei sottofascicoli
                    SelezionaSottofascicolo();
                else
                    selezionaUltimoNodoSelezionato();



                //controllo se devo creare una nuova cartella
                if (Session["descNewFolder"] != null)
                {
                    DocsPAWA.DocsPaWR.ResultCreazioneFolder result;
                    if (!this.CreateNewFolder(out result))
                    {
                        // Visualizzazione messaggio di errore
                        string errorMessage = string.Empty;
                        if (result == DocsPAWA.DocsPaWR.ResultCreazioneFolder.FOLDER_EXIST)
                            errorMessage = "Il sottofascicolo richiesto è già presente e non può essere duplicato";
                        else
                            errorMessage = "Errore nella creazione del sottofascicolo";

                        Response.Write(string.Format("<script>alert('{0}');</script>", errorMessage));
                    }

                    Session.Remove("descNewFolder");
                }

                //si ripulisce la sessione da eventuali dati necessari alla
                //popUp per la ricerca dei documenti da classificare

                this.ClearResourcesRicercaDocumentiPerFascicolazione();
                // personalizzazzione label data collocazione fisica da web.config.
                if (Utils.label_data_Loc_fisica.Trim() != "")
                    this.Label12.Text = Utils.label_data_Loc_fisica;
                else
                    this.Label12.Text = "&nbsp;Data collocaz.";

                //Verifico che è stato selezionato il calcolo di un contatore,
                //in caso affermativo, riapro la popup di profilazione, per far verificare il numero generato
                if (Session["contaDopoChecked"] != null && !IsPostBack)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "apriPopUpProfilazioneFasc", "window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamicaFasc.aspx','','dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;');", true);
                    Session.Remove("contaDopoChecked");
                }

                // Se il ruolo non ha attiva la funzione EXP_DOC_MASSIVA viene nascosto il bottone export
                // fascicolo
                //if (!UserManager.ruoloIsAutorized(this, "EXP_DOC_MASSIVA"))
                //    this.btn_exp_fasc.Visible = false;
                if (Session["ListaDocs-CampiProf"] != null && Session["ListaDocs-CampiProf"].ToString().Equals("CampiProf"))
                {
                    if (!IsPostBack)
                    {
                        if (ddl_tipologiaFasc.Items.Count > 0 && ddl_tipologiaFasc.SelectedItem.Text != "")
                        {
                            bool edit = !txt_descrizione.ReadOnly;
                            apriCampiProfilati(edit);
                        }
                    }
                }

                this.UpdateWorkingAreaIcon();

            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(this, es);
            }

            //ABBATANGELI GIANLUIGI - Utilizzato per impostare la cartella selezionata in caso di importazione cartelle e documenti
            if (Session["Old_SelectedNodeIndex"] != null)
            {
                this.SelectedFolder = (String)Session["Old_SelectedNodeIndex"];
                Session["Old_SelectedNodeIndex"] = null;
            }

            if (!String.IsNullOrEmpty(this.SelectedFolder))
            {
                this.SelectFolder(this.SelectedFolder);
                this.SelectedFolder = String.Empty;
            }
            else
            {
                Session["SelectedNodeIndex"] = this.Folders.SelectedNodeIndex;
            }

            //
            // Mev Ospedale Maggiore Policlinico
            // Il bottone riclassifica è visibile solo se il ruolo è abilitato e il fascicolo è procedimentale
            if (UserManager.ruoloIsAutorized(this, btn_Riclassifica.Tipologia) && (Fasc != null && Fasc.tipo.Equals("P")))
            {
                btn_Riclassifica.Visible = true;
                gestioneParametriRiclassificazione();
            }
            // End Mev
            //
        }

        /// <summary>
        /// Mev Ospedale Maggiore Policlinico
        /// Metodo invocato dopo il postback del campo "txt_ClassFasc"
        /// gestisce i parametri per il rendering della pagina.
        /// </summary>
        private void gestioneParametriRiclassificazione()
        {
            //
            // Recupero dalla sessione il valore del codice classificazione selezionato
            RiclassificazioneSelezionata = FascicoliManager.getClassificazioneSelezionata(this);

            try
            {
                if (IsPostBack)
                {
                    if (RiclassificazioneSelezionata != null && Session["Titolario"] != null && Session["Titolario"].ToString() == "Y")
                    {
                        //
                        // Siamo in una sessione di modifica:
                        // Per capire ciò il Codice Classifica è evidenziato in Rosso
                        this.txt_ClassFasc.Text = RiclassificazioneSelezionata.codice;
                        this.txt_ClassFasc.BackColor = Color.Yellow;
                        Session["Riclassifica"] = true;
                        //lblClassifica.Text = "Classifica " + RiclassificazioneSelezionata.codice + "-" + RiclassificazioneSelezionata.descrizione;
                    }
                }
            }
            catch (System.Exception es)
            {

            }
        }

        /// <summary>
        /// Metodo per l'evidenziazione di un folder
        /// </summary>
        /// <param name="folderIndex">Indice della folder da selezionare</param>
        private void SelectFolder(string folderIndex)
        {
            this.Folders.SelectedNodeIndex = String.Empty;
            // La folder è identificata attraverso una stringa composta di numeri
            // separati da punto in cui un numero indica il nodo selezionato in un dato
            // livello.
            String[] splittedIndexes = folderIndex.Split('.');
            foreach (var index in splittedIndexes)
            {
                if (String.IsNullOrEmpty(Folders.SelectedNodeIndex))
                    this.Folders.SelectedNodeIndex = index;
                else
                    try
                    {
                        this.Folders.SelectedNodeIndex += String.Format(".{0}", index);

                    }
                    catch (Exception)
                    {
                        // Se fallisce, significa che il nodo non esiste (quindi questa funzione è stata richiamata
                        // a seguito dell'eliminazione di una folder. In questo caso viene cancellato l'ultimo nodo
                        // selezionato in modo che venga caricato il contenuto della folder selezionata
                        Session["fascDocumenti.nodoSelezionato"] = null;

                    }
                    
                
                // Selezione del folder identificato dall'indice
                Microsoft.Web.UI.WebControls.TreeNode node = Folders.GetNodeFromIndex(this.Folders.SelectedNodeIndex);

                // Espansione del nodo
                node.Expanded = true;
                
            }
            
        }


        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }
        private bool IsAbilitataRicercaSottoFascicoli()
        {
            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.CERCA_SOTTOFASCICOLI) == null || ConfigSettings.getKey(ConfigSettings.KeysENUM.CERCA_SOTTOFASCICOLI).Equals("0"))
                return false;
            else
                return true;
        }


        /// <summary>
        /// Gestione deallocazione risorse utilizzata dalla dialog per la ricerca di documenti
        /// da fascicolare (31/03/2006)
        /// </summary>
        private void ClearResourcesRicercaDocumentiPerFascicolazione()
        {
            if (DocsPAWA.popup.RicercaDocumentiPerClassifica.RicercaDocumentiClassificaSessionMng.IsLoaded(this))
            {
                DocsPAWA.popup.RicercaDocumentiPerClassifica.RicercaDocumentiClassificaSessionMng.SetAsNotLoaded(this);
                DocsPAWA.popup.RicercaDocumentiPerClassifica.RicercaDocumentiClassificaSessionMng.ClearSessionData(this);
            }
        }

        private void selezionaUltimoNodoSelezionato()
        {
            Microsoft.Web.UI.WebControls.TreeNode nodeToSelect = getSelectedNodeFolder();

            selectNodeFolder(nodeToSelect);
        }

        private void SelezionaSottofascicolo()
        {

            hd_returnValueModal.Value = "";
            Microsoft.Web.UI.WebControls.TreeNode trnParent;
            string idFolder = "";
            if (Session["NodeIndexRicercaSottoFascicoli"] != null)
            {
                string indx = Session["NodeIndexRicercaSottoFascicoli"].ToString();
                Session.Remove("NodeIndexRicercaSottoFascicoli");

                Microsoft.Web.UI.WebControls.TreeNode node = Folders.GetNodeFromIndex(indx);
                ExpandAllParentNodes(node);

                //if (!indx.Equals("0"))
                //{
                //    trnParent = (Microsoft.Web.UI.WebControls.TreeNode)Folders.GetNodeFromIndex(indx).Parent;
                //    trnParent.Expanded = true;
                //}
                Folders.SelectedNodeIndex = indx;




                DocsPaWR.Folder folder = (DocsPAWA.DocsPaWR.Folder)HashFolder[Int32.Parse(Folders.GetNodeFromIndex(indx).ID)];
                FascicoliManager.setFolderSelezionato(this, folder);
                if (folder != null)
                    idFolder = Folders.GetNodeFromIndex(indx).ID;
                else
                    idFolder = "";


                //if (Session["ListaDocs-CampiProf"] != null && Session["ListaDocs-CampiProf"].ToString().Equals("ListaDocs"))
                //{
                Session["ListaDocs-CampiProf"] = "ListaDocs";
                string newUrl = "tabPulsantiDoc.aspx?idFolder=" + idFolder;
                this.GetControlAclFascicolo().VerificaRevocaAcl();
                newUrl += "&AclRevocata=" + this.GetControlAclFascicolo().AclRevocata.ToString();
                newUrl += "&codFasc=" + txt_fascdesc.Text;
                if (this.OnBackContext && this.Request.QueryString["docIndex"] != null)
                    newUrl += "&back=true&docIndex=" + this.Request.QueryString["docIndex"];

                Response.Write("<script>parent.parent.iFrame_dx.location='" + newUrl + "'</script>");
                //}
            }

        }

        private void ddl_tipologiaFasc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string idTemplate = ddl_tipologiaFasc.SelectedValue;
            Panel_DiagrammiStato.Visible = false;
            Panel_DataScadenza.Visible = false;

            if (idTemplate != "")
            {
                Session.Remove("template");
                template = ProfilazioneFascManager.getTemplateFascById(idTemplate, this);
                Session.Add("template", template);
                //panel_Contenuto.Controls.Clear();
                if (template != null && template.PRIVATO != null && template.PRIVATO == "1")
                {

                    chkFascicoloPrivato.Checked = true;
                }
                else
                    if (chkFascicoloPrivato.Enabled == false)
                    {
                        chkFascicoloPrivato.Checked = false;
                    }


                //if (Session["ListaDocs-CampiProf"] != null && Session["ListaDocs-CampiProf"].ToString().Equals("CampiProf"))
                //{
                if (ddl_tipologiaFasc.Items.Count > 0 && ddl_tipologiaFasc.SelectedItem.Text != "")
                {
                    bool edit = !txt_descrizione.ReadOnly;
                    //apriCampiProfilati(edit);
                    ProfilazioneFascManager.verificaCampiPersonalizzati(this, Fasc, Folders, true);
                }
                //}
            }
            else
            {
                Session.Remove("template");
            }

            //DIAGRAMMI DI STATO 
            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                {
                    if (ddl_tipologiaFasc.SelectedValue != null && ddl_tipologiaFasc.SelectedValue != "")
                    {
                        //Verifico se esiste un diagramma di stato associato al tipo di documento
                        DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoFasc(ddl_tipologiaFasc.SelectedValue, (UserManager.getInfoUtente(this)).idAmministrazione, this);
                        //Session.Add("DiagrammaSelezionato", dg);

                        //Popolo la comboBox degli stati
                        if (dg != null)
                        {
                            popolaComboBoxStatiSuccessivi(null, dg);
                            Panel_DiagrammiStato.Visible = true;
                            Session.Add("DiagrammaSelezionato", dg);
                        }

                        //Imposto la data di scadenza
                        if (Session["template"] != null)
                        {
                            DocsPaWR.Templates template = (DocsPaWR.Templates)Session["template"];
                            if (template.SCADENZA != null && template.SCADENZA != "" && template.SCADENZA != "0")
                            {
                                try
                                {
                                    DateTime dataOdierna = System.DateTime.Now;
                                    int scadenza = Convert.ToInt32(template.SCADENZA);
                                    DateTime dataCalcolata = dataOdierna.AddDays(scadenza);
                                    txt_dataScadenza.Text = Utils.formatDataDocsPa(dataCalcolata);
                                    if (FascicoliManager.getFascicoloSelezionato(this) != null)
                                        FascicoliManager.getFascicoloSelezionato(this).dtaScadenza = Utils.formatDataDocsPa(dataCalcolata);
                                    Panel_DataScadenza.Visible = true;
                                }
                                catch (Exception ex) { }
                            }
                            else
                            {
                                txt_dataScadenza.Text = "";
                                Panel_DataScadenza.Visible = false;
                                if (FascicoliManager.getFascicoloSelezionato(this) != null)
                                    FascicoliManager.getFascicoloSelezionato(this).dtaScadenza = "";
                            }
                        }
                    }
                }
            }
            //FINE DIAGRAMMI DI STATO 



        }

        private void apriCampiProfilati(bool editMode)
        {
            ////string newUrl = "tabFascCampiProf.aspx?tipoFascicolo=" + Fasc.tipo + "&codTipologiaFasc=" + ddl_tipologiaFasc.SelectedValue.ToString() + "&editMode=" + editMode.ToString(); 

            //string newUrl = "tabPulsantiDoc.aspx?tipoFascicolo=" + Fasc.tipo + "&codTipologiaFasc=" + ddl_tipologiaFasc.SelectedValue.ToString() + "&editMode=" + editMode.ToString();
            //this.GetControlAclFascicolo().VerificaRevocaAcl();
            //newUrl += "&AclRevocata=" + this.GetControlAclFascicolo().AclRevocata.ToString();
            //newUrl += "&codFasc=" + txt_fascdesc.Text;
            //Session["ListaDocs-CampiProf"] = "CampiProf";
            //Response.Write("<script>parent.parent.iFrame_dx.location='" + newUrl + "'</script>");
        }

        private void apriListaDocs()
        {
            try
            {
                string newUrl = "tabPulsantiDoc.aspx?idFolder=" + this.getSelectedNodeFolder().ID.ToString() + "&AclRevocata=" + this.GetControlAclFascicolo().AclRevocata.ToString() + "&codFasc=" + Server.UrlEncode(txt_fascdesc.Text);
                this.GetControlAclFascicolo().VerificaRevocaAcl();

                Session["ListaDocs-CampiProf"] = "ListaDocs";
                Response.Write("<script>parent.parent.iFrame_dx.location='" + newUrl + "'</script>");
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }
        private void caricaValoriFascicoloSelezionato(DocsPAWA.DocsPaWR.Fascicolo Fasc)
        {
            this.txt_fascApertura.Text = Fasc.apertura;
            this.txt_FascChiusura.Text = Fasc.chiusura;
            this.txt_descrizione.Text = Fasc.descrizione;

            // Reperimento ultima nota del fascicolo
            //this.txt_fascnote.Text = this.GetUltimaNota(Fasc);

            this.txt_fascStato.Text = FascicoliManager.decodeStatoFasc(this, Fasc.stato);
            this.chkFascicoloCartaceo.Checked = Fasc.cartaceo;
            if ((Fasc.privato == null) || (Fasc.privato == "0"))
                this.chkFascicoloPrivato.Checked = false;
            else
                this.chkFascicoloPrivato.Checked = true;

            if ((Fasc.controllato == null) || (Fasc.controllato == "0"))
                this.chkFascicoloControllato.Checked = true;
            else
                this.chkFascicoloControllato.Checked = false;

            if (!Fasc.tipo.Equals("G"))//locazione fisica nn ammessa per fascicoli generali
            {
                DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemID(this, Fasc.idUoLF);
                if (corr != null && corr.descrizione != null && corr.codiceRubrica != null)
                {
                    this.txt_LFCod.Text = corr.codiceRubrica;



                    this.txt_LFDesc.Text = corr.descrizione;
                    Fasc.varCodiceRubricaLF = txt_LFCod.Text;
                    Fasc.descrizioneUOLF = txt_LFDesc.Text;
                }
                if (Fasc.dtaLF != null && Fasc.dtaLF != "")
                {
                    this.txt_LFDTA.Text = Fasc.dtaLF.Substring(0, 10);
                    Fasc.dtaLF = txt_LFDTA.Text;
                }
            }

            if (!Fasc.tipo.Equals("G"))//ufficio referente non ammesso per fascicoli generali
            {
                if (enableUfficioRef)
                {
                    DocsPaWR.Fascicolo f = wws.FascicolazioneGetFascicoloDaCodice(infoUt, Fasc.codice, UserManager.getRegistroSelezionato(this), true, false);
                    DocsPaWR.Corrispondente uff_ref = f.ufficioReferente;
                    Fasc.ufficioReferente = uff_ref;
                    if (Fasc.ufficioReferente != null)
                    {
                        if (Fasc.ufficioReferente.codiceRubrica != null && Fasc.ufficioReferente.descrizione != null)
                        {
                            this.txt_cod_uff_ref.Text = Fasc.ufficioReferente.codiceRubrica;
                            this.txt_desc_uff_ref.Text = Fasc.ufficioReferente.descrizione;
                        }
                        else
                        {
                            DocsPaWR.Corrispondente corrRef = UserManager.getCorrispondenteBySystemID(this, Fasc.ufficioReferente.systemId);
                            if (corrRef != null && corrRef.descrizione != null && corrRef.codiceRubrica != null)
                            {
                                this.txt_cod_uff_ref.Text = corrRef.codiceRubrica;
                                this.txt_desc_uff_ref.Text = corrRef.descrizione;
                            }
                            Fasc.ufficioReferente = corrRef;
                        }
                    }
                }
            }

            FascicoliManager.setFascicoloSelezionato(this, Fasc);
            FascicoliManager.DO_RemoveLocazioneFisica();
            FascicoliManager.removeUoReferenteSelezionato(this);


            if (Fasc.stato.Equals("C"))
            {
                ViewState["Chiuso"] = true;

            }
            else
            {
                ViewState["Chiuso"] = false;

            }


            this.txt_Fasctipo.Text = FascicoliManager.decodeTipoFasc(this, Fasc.tipo);

            //controllo sulla tipologia di fascicolo
            if (Fasc.tipo.Equals("G"))
            {
                this.btn_chiudiRiapri.Enabled = false;
                this.txt_fascApertura.Visible = false;
                this.txt_FascChiusura.Visible = false;
                this.lbl_dataAp.Visible = false;
                this.lbl_dataC.Visible = false;
            }
            else
            {
                this.btn_chiudiRiapri.Enabled = true;
                this.txt_fascApertura.Visible = true;
                this.txt_FascChiusura.Visible = true;
                this.lbl_dataAp.Visible = true;
                this.lbl_dataC.Visible = true;
            }

            this.txt_ClassFasc.Text = getCodiceGerarchia(Fasc);
            this.txt_fascdesc.Text = Fasc.codice;
        }

        private void CaricaComboTipologiaFasc()
        {
            ArrayList listaTipiFasc = new ArrayList(ProfilazioneFascManager.getTipoFascFromRuolo(UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "2", this));

            ListItem item = new ListItem();
            item.Value = "";
            item.Text = "";
            ddl_tipologiaFasc.Items.Add(item);
            for (int i = 0; i < listaTipiFasc.Count; i++)
            {
                DocsPaWR.Templates templates = (DocsPaWR.Templates)listaTipiFasc[i];
                if (templates.IPER_FASC_DOC != "1")
                {
                    ListItem item_1 = new ListItem();
                    item_1.Value = templates.SYSTEM_ID.ToString();
                    item_1.Text = templates.DESCRIZIONE;
                    ddl_tipologiaFasc.Items.Add(item_1);
                }
            }

            DocsPaWR.FascicolazioneClassificazione classificazione = FascicoliManager.getClassificazioneSelezionata(this);
            if (classificazione != null && classificazione.idTipoFascicolo != null && classificazione.idTipoFascicolo != "")
            {
                ddl_tipologiaFasc.SelectedValue = classificazione.idTipoFascicolo;
                string idTemplate = ddl_tipologiaFasc.SelectedValue;
                Session.Remove("template");
                template = ProfilazioneFascManager.getTemplateFascById(idTemplate, this);
                Session.Add("template", template);
                //panel_Contenuto.Controls.Clear();
            }
            //Blocco eventualmente la tipologia di fascicolo
            //if (classificazione != null && classificazione.bloccaTipoFascicolo != null && classificazione.bloccaTipoFascicolo.Equals("SI"))
            //    ddl_tipologiaFasc.Enabled = false;
            //else
            //    ddl_tipologiaFasc.Enabled = true;
        }


        private void popolaComboBoxStatiSuccessivi(DocsPAWA.DocsPaWR.Stato stato, DocsPAWA.DocsPaWR.DiagrammaStato diagramma)
        {
            //Inizializzazione
            ddl_statiSuccessivi.Items.Clear();
            ListItem itemEmpty = new ListItem();
            ddl_statiSuccessivi.Items.Add(itemEmpty);

            //Popola la combo con gli stati iniziali del diagramma
            if (stato == null)
            {
                for (int i = 0; i < diagramma.STATI.Length; i++)
                {
                    DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)diagramma.STATI[i];
                    if (st.STATO_INIZIALE)
                    {
                        ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                        ddl_statiSuccessivi.Items.Add(item);
                    }
                }
                if (ddl_statiSuccessivi.Items.Count == 2)
                    ddl_statiSuccessivi.SelectedIndex = 1;
            }
            //Popola la combo con i possibili stati, successivi a quello passato
            else
            {
                for (int i = 0; i < diagramma.PASSI.Length; i++)
                {
                    DocsPaWR.Passo step = (DocsPAWA.DocsPaWR.Passo)diagramma.PASSI[i];
                    if (step.STATO_PADRE.SYSTEM_ID == stato.SYSTEM_ID)
                    {
                        for (int j = 0; j < step.SUCCESSIVI.Length; j++)
                        {
                            DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)step.SUCCESSIVI[j];
                            ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                            ddl_statiSuccessivi.Items.Add(item);
                        }
                    }
                }
                //Controllo che non sia uno stato finale
                if (stato.STATO_FINALE)
                {
                    ddl_statiSuccessivi.Items.Clear();
                    ListItem item = new ListItem(stato.DESCRIZIONE, Convert.ToString(stato.SYSTEM_ID));
                    ddl_statiSuccessivi.Items.Add(item);
                    ddl_statiSuccessivi.Enabled = false;
                }
            }
        }
        protected void msg_StatoAutomatico_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            DocsPAWA.DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"];
            Fasc = FascicoliManager.getFascicoloSelezionato();
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                DocsPAWA.DiagrammiManager.salvaModificaStatoFasc(Fasc.systemID, ddl_statiSuccessivi.SelectedValue, dg, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);

                //Verifico se effettuare una tramsissione automatica assegnata allo stato
                if (Fasc.template != null && Fasc.template.SYSTEM_ID != null && Fasc.template.SYSTEM_ID != 0 && Panel_DiagrammiStato.Visible)
                {
                    ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAutoFasc(UserManager.getInfoUtente(this).idAmministrazione, ddl_statiSuccessivi.SelectedItem.Value, Fasc.template.SYSTEM_ID.ToString(), this));
                    for (int i = 0; i < modelli.Count; i++)
                    {
                        DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                        //Nel caso vengo da toDoList il registro non è impostato quindi l'id lo recupero dal ruolo
                        if (mod.SINGLE == "1")
                        {
                            DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, Fasc, this);
                        }
                        else
                        {
                            for (int k = 0; k < mod.MITTENTE.Length; k++)
                            {
                                if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                {
                                    DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, Fasc, this);
                                    break;
                                }
                            }
                        }
                    }
                }

                ClientScript.RegisterStartupScript(this.GetType(), "closePage", "window.close();", true);
            }
        }

        protected void msg_StatoFinale_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            DocsPAWA.DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"];
            Fasc = FascicoliManager.getFascicoloSelezionato();
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                DocsPAWA.DiagrammiManager.salvaModificaStatoFasc(Fasc.systemID, ddl_statiSuccessivi.SelectedValue, dg, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);
                DateTime ora = DateTime.Now;
                Fasc.chiusura = ora.ToString("dd/MM/yyyy");
                Fasc.stato = "C";
                Fasc.chiudeFascicolo = new DocsPaWR.ChiudeFascicolo();
                Fasc.chiudeFascicolo.idPeople = infoUt.idPeople;
                Fasc.chiudeFascicolo.idCorrGlob_Ruolo = userRuolo.systemId;
                Fasc.chiudeFascicolo.idCorrGlob_UO = userRuolo.uo.systemId;
                FascicoliManager.setFascicolo(this, ref Fasc);
                FascicoliManager.setFascicoloSelezionato(this, Fasc);

                //Verifico se effettuare una tramsissione automatica assegnata allo stato
                if (Fasc.template != null && Fasc.template.SYSTEM_ID != null && Fasc.template.SYSTEM_ID != 0 && Panel_DiagrammiStato.Visible)
                {
                    ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAutoFasc(UserManager.getInfoUtente(this).idAmministrazione, ddl_statiSuccessivi.SelectedItem.Value, Fasc.template.SYSTEM_ID.ToString(), this));
                    for (int i = 0; i < modelli.Count; i++)
                    {
                        DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                        //Nel caso vengo da toDoList il registro non è impostato quindi l'id lo recupero dal ruolo
                        if (mod.SINGLE == "1")
                        {
                            DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, Fasc, this);
                        }
                        else
                        {
                            for (int k = 0; k < mod.MITTENTE.Length; k++)
                            {
                                if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                {
                                    DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, Fasc, this);
                                    break;
                                }
                            }
                        }
                    }
                }

                //ClientScript.RegisterStartupScript(this.GetType(), "closePage", "window.close();", true);
                ClientScript.RegisterStartupScript(this.GetType(), "ricaricaSinistra", "top.principale.iFrame_sx.document.location = 'tabGestioneFasc.aspx?tab=documenti';", true);
            }
        }

        private string getCodiceGerarchia(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
        {
            string retValue = "";
            DocsPaWR.FascicolazioneClassifica[] classifica = FascicoliManager.getGerarchia(this, fascicolo.idClassificazione, UserManager.getUtente(this).idAmministrazione);
            if (classifica != null)
            {
                retValue = ((DocsPAWA.DocsPaWR.FascicolazioneClassifica)classifica[classifica.Length - 1]).codice;
            }

            return retValue;
        }


        private void caricaFoldersFascicolo(DocsPAWA.DocsPaWR.Folder folder)
        {
            FascicoliManager.removeHashFolder(this);
            Folders.Nodes.Clear();
            HashFolder.Clear();

            indexH = 0;
            Microsoft.Web.UI.WebControls.TreeNode rootFolder = new Microsoft.Web.UI.WebControls.TreeNode();
            if (folder != null)
            {
                //Creo la root folder dell'albero
                //Modifica per sostituire la dicitura "Root Folder" con codice del fascicolo
                rootFolder.Text = Fasc.codice; //folder.descrizione;
                this.GetControlAclFascicolo().VerificaRevocaAcl();
                //rootFolder.NavigateUrl = "tabPulsantiDoc.aspx?idFolder=" + indexH.ToString() + "&AclRevocata=" + this.GetControlAclFascicolo().AclRevocata.ToString() + "&codFasc=" + txt_fascdesc.Text;
                //rootFolder.Target = "iFrame_dx";
                rootFolder.ID = indexH.ToString();

                //aggiungo la root folder alla collezione dei nodi dell'albero
                Folders.Nodes.Add(rootFolder);

                //aggiungo la root folder alla tabella di hash associata
                HashFolder.Add(indexH, folder);

                indexH = indexH + 1;
            }


            // Se è attualmente visualizzato un documento,
            // vengono reperiti tutti i folders del fascicolo
            // che contengono il documento stesso.
            // Successivamente, in fase di caricamento dell'albero,
            // verrà espanso il primo nodo che rappresenta
            // il folder.

            //cambiato getDocumentoSelezionato in getDocumentoInLavorazione per bug 2275 su schianto fascicolo proc
            //DocsPaWR.SchedaDocumento schedaDocCorrente=DocumentManager.getDocumentoSelezionato(this);
            DocsPaWR.SchedaDocumento schedaDocCorrente = DocumentManager.getDocumentoInLavorazione(this);
            if (schedaDocCorrente != null && schedaDocCorrente.systemId != null)
            {
                _foldersDocument = FascicoliManager.GetFoldersDocument(this,
                    schedaDocCorrente.systemId,
                    this.Fasc.systemID);

                schedaDocCorrente = null;
            }

            if (Session["isRicercaSottofascicoli"] != null && _foldersDocument == null)
            {
                //DocsPaWR.Folder[] folders = FascicoliManager.getFolderByDescrizione(this, fascicoloSelezionato.systemID, sottofascicolo);
                string sottofascicolo = (string)Session["isRicercaSottofascicoli"];
                _foldersDocument = FascicoliManager.getFolderByDescrizione(this, folder.idFascicolo, sottofascicolo);
                Session.Remove("isRicercaSottofascicoli");
            }

            //Costruzione Albero Folder del fascicolo.	
            if (folder.childs.Length > 0)
            {
                bool mustExpandNodeFolderDocument = true;

                for (int k = 0; k < folder.childs.Length; k++)
                {
                    this.CreateTree(rootFolder, folder.childs[k], ref mustExpandNodeFolderDocument);
                }
            }

            FascicoliManager.setHashFolder(this, this.HashFolder);
        }

        private void caricaGerarchiaFascicolazioneClassifica(DocsPAWA.DocsPaWR.FascicolazioneClassifica[] fascClass)
        {



            //Recupero il titolario di appartenenza
            if (fascClass.Length != 0)
            {
                if (fascClass[0].idTitolario != null && fascClass[0].idTitolario != "")
                {
                    int i = fascClass.Length - 1;
                    DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(fascClass[0].idTitolario);
                    lblTitolario.Text = titolario.Descrizione;
                    lblClassifica.Text = "Classifica " + fascClass[i].codice + "-" + fascClass[i].descrizione;

                    //Microsoft.Web.UI.WebControls.TreeNode nodoTit = new Microsoft.Web.UI.WebControls.TreeNode();
                    //nodoTit.Text = "<strong>" + titolario.Descrizione + "</strong>";
                    //nodoTit.ID = titolario.ID;
                    //this.Gerarchia.Nodes.Add(nodoTit);
                }
            }

            //for (int i = 0; i < fascClass.Length; i++)
            //{
            //    Microsoft.Web.UI.WebControls.TreeNode Root2 = new Microsoft.Web.UI.WebControls.TreeNode();
            //    Root2.Text = fascClass[i].codice + "-" + fascClass[i].descrizione;
            //    Root2.ID = i.ToString();
            //    this.Gerarchia.Nodes.Add(Root2);
            //}

            //visualizzo dati fascicolo
            caricaValoriFascicoloSelezionato(Fasc);

            //get folder dal fascicolo Fasc:					
            DocsPaWR.Folder folder = FascicoliManager.getFolder(this, Fasc);

            caricaFoldersFascicolo(folder);
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            if (!this.DesignMode)
            {
                if (Context != null && Context.Session != null && Session != null)
                {
                    InitializeComponent();

                    base.OnInit(e);
                }
            }
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///	the contents of this method with the code editor.
        ///	</summary>
        private void InitializeComponent()
        {

            this.btn_Salva.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Salva_Click);
            this.btn_chiudiRiapri.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiRiapri_Click);
            this.btn_addToAreaLavoro.Click += new System.Web.UI.ImageClickEventHandler(this.btn_addToAreaLavoro_Click);
            this.btn_stampaFascette.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampaFascette_Click);
            this.btn_modFolder.Click += new System.Web.UI.ImageClickEventHandler(this.btn_modFolder_Click);
            this.btn_modifica.Click += new System.Web.UI.ImageClickEventHandler(this.btn_modifica_Click);
            this.btn_AnnullaModifiche.Click += new System.Web.UI.ImageClickEventHandler(this.btn_AnnullaModifiche_Click);
            this.btn_rimuovi.Click += new System.Web.UI.ImageClickEventHandler(this.btn_rimuovi_Click);
            this.img_btnDettagliProf.Click += new System.Web.UI.ImageClickEventHandler(this.img_btnDettagliProf_Click);
            this.img_btnStoriaDiagrammi.Click += new System.Web.UI.ImageClickEventHandler(this.img_btnStoriaDiagrammi_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.txt_LFCod.TextChanged += new System.EventHandler(this.txt_LFCod_TextChanged);
            this.txt_cod_uff_ref.TextChanged += new System.EventHandler(this.txt_cod_uff_ref_TextChanged);
            this.PreRender += new System.EventHandler(this.fascDocumenti_PreRender);
            this.btn_log.Click += new System.Web.UI.ImageClickEventHandler(this.btn_log_Click);

            this.btn_storiaFasc.Click += new System.Web.UI.ImageClickEventHandler(this.btn_storiaFasc_Click);
            this.ddl_tipologiaFasc.SelectedIndexChanged += new EventHandler(this.ddl_tipologiaFasc_SelectedIndexChanged);
            this.Folders.SelectedIndexChange += new SelectEventHandler(this.Folders_SelectedIndexChange);
            this.btnPrintSignature.Click += new System.Web.UI.ImageClickEventHandler(this.btnPrintSignature_Click);
            this.btn_aggiungi.Click += new ImageClickEventHandler(btn_aggiungi_Click);

            //
            // Mev Ospedale Maggiore Policlinico
            this.btn_Riclassifica.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Riclassifica_Click);
            // End Mev
            //
        }
        #endregion

        private void setDescCorrispondente(string codiceRubrica, bool fineValidita)
        {
            string msg = "Codice rubrica non esistente";
            DocsPaWR.Corrispondente corr = null;
            try
            {
                if (!codiceRubrica.Equals(""))
                    corr = UserManager.GetCorrispondenteInterno(this, codiceRubrica, fineValidita);
                if ((corr != null && corr.descrizione != "") && corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)))
                {
                    txt_LFDesc.Text = corr.descrizione;
                    DocsPaVO.LocazioneFisica.LocazioneFisica LF = new DocsPaVO.LocazioneFisica.LocazioneFisica();
                    LF.CodiceRubrica = corr.codiceRubrica;
                    LF.Descrizione = corr.descrizione;
                    LF.UO_ID = corr.systemId;
                    if (this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString().Trim() != "")
                    {
                        LF.Data = this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString();
                    }
                    else
                    {
                        LF.Data = System.DateTime.Now.ToShortDateString();
                    }

                    //metto la LF in session
                    FascicoliManager.DO_SetLocazioneFisica(LF);
                    //					FascicoliManager.DO_SetFlagLF();
                }
                else
                {
                    if (!codiceRubrica.Equals(""))
                    {
                        RegisterStartupScript("alert", "<script language='javascript'>alert('" + msg + "');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_LFCod.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                    }
                    this.txt_LFDesc.Text = "";

                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        protected void switchToDocumentslist()
        {

            Session["ListaDocs-CampiProf"] = "ListaDocs";
        }

        private Microsoft.Web.UI.WebControls.TreeNode addFolderNode(Microsoft.Web.UI.WebControls.TreeNode parentNode, DocsPaWR.Folder folder)
        {
            //   Session["ListaDocs-CampiProf"] = "ListaDocs";
            Microsoft.Web.UI.WebControls.TreeNode node = new Microsoft.Web.UI.WebControls.TreeNode();

            node.Text = folder.descrizione;
            node.ID = indexH.ToString();
            this.GetControlAclFascicolo().VerificaRevocaAcl();
            //node.NavigateUrl = "tabPulsantiDoc.aspx?idFolder=" + indexH.ToString() + "&AclRevocata=" + this.GetControlAclFascicolo().AclRevocata.ToString() +  "&codFasc=" + txt_fascdesc.Text; 
            //node.Target = "iFrame_dx";


            //aggiunge il nodo creato al nodo genitore
            parentNode.Nodes.Add(node);

            //aggiunge nella hashtable, la folder corrispondente al nodo creato 
            HashFolder.Add(indexH, folder);

            indexH = indexH + 1;

            return node;
        }

        private void ExpandAllParentNodes(Microsoft.Web.UI.WebControls.TreeNode node)
        {
            if (node.Parent != null)
            {
                Microsoft.Web.UI.WebControls.TreeNode parentNode = node.Parent as Microsoft.Web.UI.WebControls.TreeNode;

                if (parentNode != null)
                {
                    parentNode.Expanded = true;
                    this.ExpandAllParentNodes(parentNode);
                    //parentNode=null;
                }
            }
        }

        //se cambia il registro ri-creo tutto albero.
        public void CreateTree(Microsoft.Web.UI.WebControls.TreeNode parentNode, DocsPaWR.Folder obj, ref bool mustExpandNodeFolderDocument)
        {
            Microsoft.Web.UI.WebControls.TreeNode newAddedNode = addFolderNode(parentNode, obj);

            int g = obj.childs.Length;

            if (mustExpandNodeFolderDocument &&
                this.ContainCurrentDocumentInFolder(obj.systemID))
            {
                // espansione di tutti i nodi padre del nodo corrente
                this.ExpandAllParentNodes(newAddedNode);

                // NB: in fase di creazione dei nodi (oggetti folder),
                // viene verificato se il documento corrente sia
                // contenuto nel folder stesso; in tal caso 
                // è richiesta la visualizzazione del contenuto di quest'ultimo.
                // Causa un probabile baco del treeview riguardante
                // il passaggio dei parametri sulla querystring, 
                // è necessario impostare una variabile di sessione contenente
                // l'ID del folder selezionato.
                Session["tabFascListaDoc.idFolder"] = newAddedNode.ID;

                // impostazione del nodo come selezionato
                Folders.SelectedNodeIndex = newAddedNode.GetNodeIndex();

                mustExpandNodeFolderDocument = false;
            }

            for (int j = 0; j < g; j++)
            {
                DocsPaWR.Folder newFolder = obj.childs[j];

                //richiama la funzione ricorsivamente
                CreateTree(newAddedNode, newFolder, ref mustExpandNodeFolderDocument);
            }
        }

        /// <summary>
        /// Verifica se è presente, nel folder fornito in ingresso,
        /// del documento correntemente visualizzato
        /// (utilizzata in fase di caricamento dell'albero)
        /// </summary>
        /// <param name="systemIdFolder"></param>
        /// <returns></returns>
        private bool ContainCurrentDocumentInFolder(string systemIdFolder)
        {
            bool retValue = false;

            if (this._foldersDocument != null)
            {
                foreach (DocsPAWA.DocsPaWR.Folder folder in this._foldersDocument)
                {
                    retValue = (systemIdFolder.Equals(folder.systemID));
                    if (retValue)
                        break;
                }
            }

            return retValue;
        }


        #region OLD getSelectedFolder
        //		private DocsPAWA.DocsPaWR.Folder getSelectedFolder()
        //		{
        //			
        //			DocsPaWR.Folder folderSelected ;
        //			TreeNode node=this.Folders.GetNodeFromIndex(Folders.SelectedNodeIndex);
        //			int id=Int32.Parse(node.ID);
        //			//inserito questo ocntrollo per verificare se la cartella
        //			//sia una root folder (a indice 0),pertanto ,non cancellabile.
        //			//suggerimento : individuare il metodo che dia l'indice del nodo 
        //			//selezionato in modo da disabilitare al click sul nodo ,il  bottone
        //			//rimuovi
        //
        //			//elimino il controllo per consentire l'inserimento di un doc anche nel root folder
        //////			if(id != 0)
        //			{
        //				FascicoliManager.setFolderSelezionato(this,(DocsPAWA.DocsPaWR.Folder)this.HashFolder[id]);
        //				folderSelected=FascicoliManager.getFolderSelezionato(this);
        //			}
        //////			else
        //////			{
        //////				folderSelected = null;
        //////			}
        //		  
        //			return folderSelected;		
        //		}
        #endregion

        private DocsPAWA.DocsPaWR.Folder getSelectedFolder(out bool rootFolder)
        {
            DocsPaWR.Folder folderSelected = null;
            rootFolder = false;
            Microsoft.Web.UI.WebControls.TreeNode node = this.Folders.GetNodeFromIndex(Folders.SelectedNodeIndex);
            int id = Int32.Parse(node.ID);
            if (id >= 0)
            {
                folderSelected = (DocsPAWA.DocsPaWR.Folder)this.HashFolder[id];
                FascicoliManager.setFolderSelezionato(this, folderSelected);
                if (id == 0)
                    rootFolder = true;
            }
            return folderSelected;
        }

        private Microsoft.Web.UI.WebControls.TreeNode getSelectedNodeFolder()
        {
            Microsoft.Web.UI.WebControls.TreeNode nodeToSelect;
            if (this.Page.Session["fascDocumenti.nodoSelezionato"] != null)
            {
                nodeToSelect = (Microsoft.Web.UI.WebControls.TreeNode)this.Page.Session["fascDocumenti.nodoSelezionato"];
            }
            else
            {
                if (Folders.Nodes.Count > 0)
                {
                    nodeToSelect = Folders.GetNodeFromIndex(Folders.SelectedNodeIndex);
                }
                else
                {
                    nodeToSelect = null;
                }
                this.Page.Session["fascDocumenti.nodoSelezionato"] = nodeToSelect;
            }

            return nodeToSelect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool CreateNewFolder(out DocsPaWR.ResultCreazioneFolder result)
        {
            bool retValue = false;
            result = DocsPAWA.DocsPaWR.ResultCreazioneFolder.GENERIC_ERROR;

            try
            {
                DocsPaWR.Folder folderSelected = FascicoliManager.getFolderSelezionato(this);

                Microsoft.Web.UI.WebControls.TreeNode nodeSelected = getSelectedNodeFolder();

                if (folderSelected != null)
                {
                    DocsPaWR.Folder newFolder = new DocsPAWA.DocsPaWR.Folder();

                    newFolder.idFascicolo = Fasc.systemID;
                    newFolder.idParent = folderSelected.systemID;

                    newFolder.descrizione = Session["descNewFolder"].ToString();

                    if (FascicoliManager.newFolder(this, ref newFolder, infoUt, userRuolo, out result))
                    {
                        DocsPaWR.Folder folder = FascicoliManager.getFolder(this, Fasc);
                        caricaFoldersFascicolo(folder);

                        selectNodeFolder(nodeSelected);
                    }

                    retValue = (result == DocsPAWA.DocsPaWR.ResultCreazioneFolder.OK);
                }
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(this, es);
            }

            return retValue;
        }


        private void expandNode(Microsoft.Web.UI.WebControls.TreeNode nodeFolderToSelect)
        {
            Hashtable nodi = new Hashtable();
            Microsoft.Web.UI.WebControls.TreeNode selectedNode = nodeFolderToSelect;
            selectedNode.Expanded = true;
            int index = 0;
            while (selectedNode != null)
            {
                nodi.Add(index, selectedNode);
                if (selectedNode.Parent.GetType().ToString() != Folders.GetType().ToString())
                {
                    selectedNode = (Microsoft.Web.UI.WebControls.TreeNode)selectedNode.Parent;
                }
                else
                {
                    selectedNode = null;
                }

                index++;
            }

            for (int i = nodi.Count - 1; i >= 0; i--)
            {
                selectedNode = (Microsoft.Web.UI.WebControls.TreeNode)nodi[i];
                selectedNode.Expanded = true;
            }

            nodi.Clear();
        }

        private void selectNodeFolder(Microsoft.Web.UI.WebControls.TreeNode nodeFolderToSelect)
        {
            //Microsoft.Web.UI.WebControls.TreeNode nodo=HashFolder[folderToSelect];
            //Folders.Nodes[folderToSelect]
            string idFolder = "";
            if (nodeFolderToSelect != null)
            {
                expandNode(nodeFolderToSelect);

                DocsPaWR.Folder folder = (DocsPAWA.DocsPaWR.Folder)HashFolder[Int32.Parse(nodeFolderToSelect.ID)];
                FascicoliManager.setFolderSelezionato(this, folder);
                if (folder != null)
                {
                    idFolder = nodeFolderToSelect.ID;
                }
                else
                {
                    idFolder = "";
                }
            }
            else
            {
                idFolder = "";
            }
            Session["IdFolderselezionato"] = idFolder;

            //if (Session["ListaDocs-CampiProf"] != null && Session["ListaDocs-CampiProf"].ToString() == "ListaDocs")
            //{

            //    string newUrl = "tabPulsantiDoc.aspx?idFolder=" + idFolder;

            //    if (this.OnBackContext && this.Request.QueryString["docIndex"] != null)
            //    {
            //        newUrl += "&back=true&docIndex=" + this.Request.QueryString["docIndex"];
            //    }
            //    newUrl += "&codFasc=" + txt_fascdesc.Text;
            //    this.GetControlAclFascicolo().VerificaRevocaAcl();
            //    newUrl += "&AclRevocata=" + this.GetControlAclFascicolo().AclRevocata.ToString();
            //    Response.Write("<script>parent.parent.iFrame_dx.location='" + newUrl + "'</script>");


            //}            
        }

        private void btn_stampa_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        private void btn_log_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string scriptString = "<SCRIPT>ApriFinestraLog('F');</SCRIPT>";
            this.Page.RegisterStartupScript("apriModalDialogLog", scriptString);
        }

        private void btn_rimuovi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                try
                {
                    this.SelectedFolder = Folders.SelectedNodeIndex;

                    #region OLD btn_rimuovi_Click
                    //				DocsPaWR.Folder selectedFolder=getSelectedFolder();
                    //				if (selectedFolder!=null)
                    //				{
                    //					if (selectedFolder.idParent.Equals(selectedFolder.idFascicolo))
                    //					{
                    //						this.Response.Write("<script>alert('Non è possibile rimuovere un Root Folder');</script>");
                    //						return;
                    //					}
                    //					TreeNode parentNode=(TreeNode)getSelectedNodeFolder().Parent;
                    //					FascicoliManager.delFolder(this,selectedFolder);
                    //					DocsPaWR.Folder folder=FascicoliManager.getFolder(this,Fasc);
                    //					caricaFoldersFascicolo(folder);
                    //					DocsPaWR.Folder folderToSelect=(DocsPAWA.DocsPaWR.Folder)HashFolder[parentNode.ID];
                    //					selectNodeFolder(parentNode);
                    //				}
                    #endregion

                    bool rootFolder = false;
                    string nFasc = "";
                    DocsPaWR.Folder selectedFolder = getSelectedFolder(out rootFolder);
                    if (rootFolder)
                    {
                        if (Fasc.tipo.Equals("P"))
                        {
                            Response.Write("<script>alert('Non è possibile rimuovere il fascicolo procedimentale: " + Fasc.codice + "') ;</script>");
                        }
                        if (Fasc.tipo.Equals("G"))
                        {
                            Response.Write("<script>alert('Non è possibile rimuovere il fascicolo generale: " + Fasc.codice + "') ;</script>");
                        } return;
                    }
                    if (selectedFolder != null)
                    {
                        /* Se il folder selezionato ha figli (doc o sottocartelle) su cui HO visibilità 
                         * non deve essere rimosso. Dopo l'avviso all'utente, la procedura termina */
                        if (selectedFolder.childs.Length > 0)
                        {
                            Response.Write("<script>alert('Non è possibile rimuovere il sottofascicolo selezionato:\\n\\ncontiene DOCUMENTI o SOTTOFASCICOLI');</script>");
                        }
                        else
                        {
                            /* Se il folder selezionato ha figli (doc o sottocartelle) su cui NON HO 
                             * la visibilità non deve essere rimosso */
                            //CanRemoveFascicolo ritornerà un bool: true = posso rimuovere il folder, false altrimenti
                            if (!FascicoliManager.CanRemoveFascicolo(this, selectedFolder.systemID, out nFasc))
                            {
                                if (nFasc.Equals("0") || nFasc.Equals(""))
                                {
                                    Response.Write("<script>alert('Non è possibile rimuovere il sottofascicolo selezionato:\\n\\ncontiene DOCUMENTI');</script>");
                                }
                                else
                                {
                                    Response.Write("<script>alert('Non è possibile rimuovere il sottofascicolo selezionato:\\n\\ncontiene DOCUMENTI o SOTTOFASCICOLI');</script>");
                                }
                            }
                            else
                            {
                                //Microsoft.Web.UI.WebControls.TreeNode parentNode = (Microsoft.Web.UI.WebControls.TreeNode)getSelectedNodeFolder().Parent;
                                FascicoliManager.delFolder(this, selectedFolder);
                                DocsPaWR.Folder folder = FascicoliManager.getFolder(this, Fasc);
                                caricaFoldersFascicolo(folder);
                                //DocsPaWR.Folder folderToSelect = (DocsPAWA.DocsPaWR.Folder)HashFolder[parentNode.ID];
                                //selectNodeFolder(parentNode);
                                
                                this.SelectFolder(this.SelectedFolder);
                                this.SelectedFolder = String.Empty;
                                this.VisualizzaContenutoFolder();
                            }
                        }
                    }

                }
                catch (System.Web.Services.Protocols.SoapException es)
                {
                    ErrorManager.redirect(this, es);
                }
            }
        }


        private void fascDocumenti_PreRender(object sender, System.EventArgs e)
        {
            DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
            info = UserManager.getInfoUtente(this.Page);
            string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_DESC_FASC");
            if (valoreChiave != string.Empty)
                caratteriDisponibili = int.Parse(valoreChiave);

            this.clTesto.Value = (caratteriDisponibili - txt_descrizione.Text.Length).ToString();

            if (IsAbilitataRicercaSottoFascicoli())
                this.img_cercaSottoFasc.Visible = true;
            else
                this.img_cercaSottoFasc.Visible = false;


            string profilazione = System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"];
            //if (Fasc.tipo.Equals("P"))
            //    this.btn_modifica.Attributes.Add("onclick", "ApriFinestraModificaFasc('" + profilazione + "');");
            //else
            //    this.btn_modifica.Attributes.Add("onclick", "ApriFinestraModificaFasc('0');");


            this.btn_aggiungi.Attributes.Add("onclick", "ApriFinestraNewFolder();");
            

            //abilitazione delle funzioni in base al ruolo
            UserManager.disabilitaFunzNonAutorizzate(this);
            //Elisa 11/08/2005 gestione nodo titolario ReadOnly

            if (FascClass != null)
            {
                if (FascClass.Length > 0)
                {
                    if (!UserManager.ruoloIsAutorized(this, this.btn_aggiungi.Tipologia.ToString()) || FascClass[0].cha_ReadOnly == true)
                    {
                        this.btn_aggiungi.Enabled = false;
                    }
                }
            }

            if (UserManager.ruoloIsAutorized(this, "DO_FASC_SE_STAMPA"))
                btnPrintSignature.Visible = true;
            else
                btnPrintSignature.Visible = false;
            

            verificaHMdiritti();
            verificaChiusuraFascicolo();
            verificaNodoTitolarioAbilitataCreaFasc();

            //Controllo se il ruolo utente è autorizzato a creare documenti privati
            if (!UserManager.ruoloIsAutorized(this, "DO_FASC_PRIVATO"))
            {
                this.chkFascicoloPrivato.Visible = false;
                this.lblFascicoloPrivato.Visible = false;
            }
            else
            {
                this.chkFascicoloPrivato.Visible = true;
                this.lblFascicoloPrivato.Visible = true;
            }
            if (Fasc.tipo.Equals("P")
                && Fasc.privato != null
                 )
            {
                //in questo caso in cui privato=1
                //forzo la visibilità anche se 
                //il ruolo non è autorizzato, 
                // altrimenti non è visibile 
                //l'informazione che il documento è privato
                this.chkFascicoloPrivato.Visible = (Fasc.privato == "1");
                this.lblFascicoloPrivato.Visible = (Fasc.privato == "1");

            }

            //Controllo che sia possibile la gestione di fascicoli controllati
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["GEST_FASC_CONTROLLATO"]) && System.Configuration.ConfigurationManager.AppSettings["GEST_FASC_CONTROLLATO"].Equals("1"))
            {
                this.chkFascicoloControllato.Visible = true;
                lblFascicoloControllato.Visible = true;
            }
            else
            {
                this.chkFascicoloControllato.Visible = false;
                this.lblFascicoloControllato.Visible = false;
            }

            //string conservazione = ConfigurationManager.AppSettings["CONSERVAZIONE"];
            //if (!string.IsNullOrEmpty(conservazione) && conservazione == "1")
            //{
            //    this.btn_storiaFasc.Visible = true;
            //}

            if (UserManager.ruoloIsAutorized(this, "DO_CONS"))
            {
                this.btn_storiaFasc.Visible = true;
            }

            //if (Fasc.chiusura != null && !Fasc.chiusura.Equals(""))
            //    this.btn_imp_doc.Visible = false;
            //Logger.log("fascdocumneti.verificaCampiPersonalizzati");
            if (!IsPostBack)
                ProfilazioneFascManager.verificaCampiPersonalizzati(this, Fasc, Folders, false);

            //Quando un Fascicolo è di una tipologia non in esercizio, non deve essere possibile cambiare lo stato
            if ((DocsPaWR.Templates)Session["template"] != null && ((DocsPaWR.Templates)Session["template"]).IN_ESERCIZIO.ToUpper().Equals("NO"))
            {
                ddl_statiSuccessivi.SelectedIndex = 0;
                ddl_statiSuccessivi.Enabled = false;
            }

            //Verifica atipicita documento
            Utils.verificaAtipicita(Fasc, DocsPaWR.TipoOggettoAtipico.FASCICOLO, ref btn_visibilita);

            // Verifica del vero creatore del documento
            Utils.CheckCreatorRole(Fasc, ref this.btn_log);

        }

        void btn_aggiungi_Click(object sender, ImageClickEventArgs e)
        {
            this.SelectedFolder = this.Folders.SelectedNodeIndex;
        }

        private void settaStampaFascicolo() {

           
                StampaEtichetta se = (StampaEtichetta)LoadControl("StampaEtichetta.ascx");

                se.descrizione = this.txt_descrizione.Text;

                se.classifica = userReg.codRegistro; //this.txt_ClassFasc.Text;
                se.codice = this.txt_fascdesc.Text;

                ph_stampa.Controls.Add(se);
           
        }

        private void verificaChiusuraFascicolo()
        {
            if (Fasc != null && Fasc.stato == "C")
            {
                this.btn_AnnullaModifiche.Enabled = false;
                this.btn_aggiungi.Enabled = false;
                this.btn_modifica.Enabled = false;
                this.btn_modFolder.Enabled = false;
                this.btn_rimuovi.Enabled = false;
            }

        }
        private void verificaNodoTitolarioAbilitataCreaFasc()
        {

            if (!FascicoliManager.verificaNodoAbilitatoCreaFasc(Fasc.idClassificazione))
            {

                this.btn_aggiungi.Enabled = false;


            }


        }

        private void verificaHMdiritti()
        {
            //disabilitazione dei bottoni in base all'autorizzazione di HM 
            //sul documento

            if (Fasc != null && (Fasc.accessRights != null && Fasc.accessRights != ""))
            {
                //if( UserManager.disabilitaButtHMDiritti(Fasc.accessRights) || (Fasc.inArchivio!= null && Fasc.inArchivio=="1") )
                if (UserManager.disabilitaButtHMDiritti(Fasc.accessRights))
                {
                    //bottoni che devono essere disabilitati in caso
                    //di diritti di sola lettura
                    this.btn_chiudiRiapri.Enabled = false;
                    this.btn_modifica.Enabled = false;
                    this.btn_modFolder.Enabled = false;
                    this.btn_aggiungi.Enabled = false;
                    this.btn_AnnullaModifiche.Enabled = false;
                    this.btn_rimuovi.Enabled = false;
                }
            }
        }

        private void verificaReadOnly(DocsPAWA.DocsPaWR.FascicolazioneClassifica[] FascClass)
        {


        }

        private void btn_chiudiRiapri_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                DateTime ora = DateTime.Now;
                try
                {

                    if (((bool)ViewState["Chiuso"]) == false)
                    {
                        Fasc.chiusura = ora.ToString("dd/MM/yyyy");
                        Fasc.stato = "C";
                        Fasc.chiudeFascicolo = new DocsPaWR.ChiudeFascicolo();
                        Fasc.chiudeFascicolo.idPeople = infoUt.idPeople;
                        Fasc.chiudeFascicolo.idCorrGlob_Ruolo = userRuolo.systemId;
                        Fasc.chiudeFascicolo.idCorrGlob_UO = userRuolo.uo.systemId;
                        FascicoliManager.setFascicolo(this, ref Fasc);
                    }
                    else
                    {
                        Fasc.chiusura = "";
                        Fasc.apertura = ora.ToString("dd/MM/yyyy");
                        Fasc.stato = "A";
                        FascicoliManager.setFascicolo(this, ref Fasc);

                    }
                    FascicoliManager.setFascicoloSelezionato(this, Fasc);
                    string funct = " window.open('../fascicolo/fascDocumenti.aspx','IframeTabs'); ";

                    Response.Write("<script> " + funct + "</script>");

                }
                catch (System.Web.Services.Protocols.SoapException es)
                {
                    ErrorManager.redirect(this, es);
                }
            }
        }

        private void btn_addToAreaLavoro_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                try
                {
                    if (Fasc.InAreaLavoro == "0" || String.IsNullOrEmpty(Fasc.InAreaLavoro))
                    {
                        FascicoliManager.addFascicoloInAreaDiLavoro(this, Fasc);
                        this.Fasc.InAreaLavoro = "1";
                    }
                    else
                    {
                        FascicoliManager.eliminaFascicoloDaAreaDiLavoro(this, Fasc);
                        Fasc.InAreaLavoro = "0";
                    }

                    this.UpdateWorkingAreaIcon();
                }
                catch (System.Web.Services.Protocols.SoapException es)
                {
                    ErrorManager.redirect(this, es);
                }
            }
        }



        private void btn_insDaAreaLavoro_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                //DocsPaWR.Folder selectedFolder=getSelectedFolder();
                bool rootFolder = false;
                DocsPaWR.Folder selectedFolder = getSelectedFolder(out rootFolder);
                string idFolder = "";
                string scriptName = "";
                string script = "";

                if (selectedFolder != null && selectedFolder.systemID != "")
                {
                    idFolder = selectedFolder.systemID;

                    string tipoDoc = "tipoDoc=T";
                    string action = "action=addDocToFolder";
                    string parameter = "";
                    parameter = "folderId=" + idFolder;

                    /* massimo digregorio 
                    descrizione: visualizzazione dei DOCUMENTI in ADL filtratri per Registro del FASCICOLO. 
                    new:*/
                    string paramIdReg = String.Empty;
                    if (Fasc.idRegistroNodoTit != null && !Fasc.idRegistroNodoTit.Equals(String.Empty))
                        paramIdReg = "&idReg=" + Fasc.idRegistroNodoTit;

                    string queryString = tipoDoc + "&" + action + "&" + parameter + paramIdReg;


                    script = "<script>ApriFinestraRicercaDocPerClassifica('" + queryString + "');</script>";
                    //script="<script>ApriFinestraADL('../popup/areaDiLavoro.aspx?"+queryString+"');</script>";
                    //scriptName="addFromADL";
                    scriptName = "addRicPerClass";
                }
                else
                {
                    script = "<script>alert('Selezionare un Folder');</script>";
                    scriptName = "SelectFolderAlert";
                }

                this.RegisterStartupScript(scriptName, script);
                Page.RegisterClientScriptBlock(scriptName, script);
            }
        }


        private void btn_modFolder_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                bool rootFolder = false;
                DocsPaWR.Folder selectedFolder = getSelectedFolder(out rootFolder);
                if (rootFolder)
                {
                    Response.Write("<script>alert('Non è possibile modificare la descrizione della cartella principale');</script>");
                }
                else
                {
                    //this.EnableModify();
                    string page = "'../popup/modificaFolder.aspx'";
                    string titolo = "'Modifica_Folder'";
                    string param = "'width=420,height=200, scrollbar=no'";
                    Page.RegisterStartupScript("modificaFolder", "<SCRIPT>ApriFinestraGen(" + page + ',' + titolo + ',' + param + ");</SCRIPT>");

                    this.btn_modFolder.Attributes.Add("onclick", "ApriFinestraGen(" + page + ',' + titolo + ',' + param + ");");

                    // Aggiunta al call context del nodo selezionato in modo da forzarne 
                    // la riselezione quando termina la modifica
                    this.SelectedFolder = Folders.SelectedNodeIndex;
                }
            }
        }

        private void DisableModify()
        {
            txt_descrizione.ReadOnly = true;
            chkFascicoloCartaceo.Enabled = false;
            chkFascicoloControllato.Enabled = false;
            chkFascicoloPrivato.Enabled = false;
            btn_rubrica_ref.Visible = false;
            txt_LFCod.ReadOnly = true;
            txt_cod_uff_ref.ReadOnly = true;
            // Corr.CODICE_READ_ONLY = true;
            txt_LFDTA.EnableBtnCal = false;
            dettaglioNota.Enabled = false;
            btn_modifica.Enabled = true;

            btn_Salva.Enabled = false;
            btn_AnnullaModifiche.Enabled = false;
            ddl_statiSuccessivi.Enabled = false;
            //ddl_statiSuccessivi.Items.Clear();
            ddl_tipologiaFasc.Enabled = false;
            btn_rubrica.Visible = false;
            // Apro i campi profilati solo se è selezionata una tipologia
            if (ddl_tipologiaFasc.Items.Count > 0 && ddl_tipologiaFasc.SelectedItem.Text != "" && Session["ListaDocs-CampiProf"].ToString() == "CampiProf")
            {
                apriCampiProfilati(false);
            }

            //
            // Mev Ospedale Maggiore Policlinico
            // Il pulsante "Riclassifica sarà visibile solo se il ruolo è abilitato alla funzione"
            // Se non clicco la matita il pulsante è disabilitato
            if (btn_Riclassifica.Visible)
            {
                //
                // Mev Ospedale Maggiore Policlinico
                // La sessione del Titolario è Y solo se si proviene dalla popup del titolario
                Session["Titolario"] = "";
                // End Mev
                //
                btn_Riclassifica.Enabled = false;
            }
            // End Mev
            //

        }
        private void EnableModify()
        {
            txt_descrizione.ReadOnly = false;
            chkFascicoloCartaceo.Enabled = true;
            chkFascicoloControllato.Enabled = true;
            chkFascicoloPrivato.Enabled = true;
            // Corr.CODICE_READ_ONLY = false;
            btn_rubrica.Visible = true;
            btn_rubrica_ref.Visible = true;
            txt_LFCod.ReadOnly = false;
            txt_cod_uff_ref.ReadOnly = false;
            txt_LFDTA.EnableBtnCal = true;
            dettaglioNota.Enabled = true;
            btn_modifica.Enabled = false;
            ddl_statiSuccessivi.Enabled = true;
            btn_Salva.Enabled = true;
            btn_AnnullaModifiche.Enabled = true;

            //
            // Mev Ospedale Maggiore Policlinico
            // Il pulsante "Riclassifica sarà visibile solo se il ruolo è abilitato alla funzione"
            // Quando è visibile e clicco la matita, allora il pulsante (ImageButton) è abilitato
            if (btn_Riclassifica.Visible)
            {
                //
                // Mev Ospedale Maggiore Policlinico
                // La sessione del Titolario è Y solo se si proviene dalla popup del titolario
                Session["Titolario"] = "";
                // End Mev
                //
                btn_Riclassifica.Enabled = true;
            }
            // End Mev
            //

            ProfilazioneFascManager.verificaCampiPersonalizzati(this, Fasc, Folders, true);

            //DIAGRAMMI DI STATO 
            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                {
                    if (ddl_tipologiaFasc.SelectedValue != null && ddl_tipologiaFasc.SelectedValue != "")
                    {
                        //Verifico se esiste un diagramma di stato associato al tipo di documento
                        DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoFasc(ddl_tipologiaFasc.SelectedValue, (UserManager.getInfoUtente(this)).idAmministrazione, this);
                        //Session.Add("DiagrammaSelezionato", dg);

                        //Popolo la comboBox degli stati
                        if (dg != null)
                        {
                            DocsPaWR.Stato statoAttuale = DocsPAWA.DiagrammiManager.getStatoFasc(FascicoliManager.getFascicoloSelezionato(this).systemID, this);
                            if (statoAttuale != null)
                                popolaComboBoxStatiSuccessivi(statoAttuale, dg);
                            else
                                popolaComboBoxStatiSuccessivi(null, dg);
                            Panel_DiagrammiStato.Visible = true;
                            Session.Add("DiagrammaSelezionato", dg);
                        }
                    }
                }
            }
            //FINE DIAGRAMMI DI STATO 

            ////Apro i campi profilati solo se è selezionata una tipologia
            //if (ddl_tipologiaFasc.Items.Count > 0 && ddl_tipologiaFasc.SelectedItem.Text != "" && Session["ListaDocs-CampiProf"].ToString() == "CampiProf")
            //{
            //    ddl_tipologiaFasc.Enabled = false;
            //    apriCampiProfilati(true);
            //}
            //else
            //{
            //    if (ddl_tipologiaFasc.Items.Count > 0 && ddl_tipologiaFasc.SelectedItem.Text == "")
            //       ddl_tipologiaFasc.Enabled = true;
            //    else 
            //        ddl_tipologiaFasc.Enabled = false;


            //}
        }




        private void img_btnDettagliProf_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                // Apro i campi profilati solo se è selezionata una tipologia
                if (ddl_tipologiaFasc.Items.Count > 0 && ddl_tipologiaFasc.SelectedItem.Text != "")
                {
                    bool edit = !txt_descrizione.ReadOnly;
                    //apriCampiProfilati(edit);
                    ProfilazioneFascManager.verificaCampiPersonalizzati(this, Fasc, Folders, false);
                }

            }
        }

        private void btn_stampaFascette_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclFascicolo().AclRevocata)
            {
                DocsPaWR.FileDocumento fileRep = FascicoliManager.reportFascette(this, this.txt_fascdesc.Text, userReg);
                if (fileRep != null)
                {
                    //				FileManager.setSelectedFileReport(this,fileRep,string.Empty);
                    this.Session["FileManager.selectedReport"] = fileRep;
                    //string sval = @"../popup/ModalVisualReport.aspx?id=" + this.Session.SessionID;
                    //RegisterStartupScript("ApriModale", "<script>OpenMyDialog('" + sval + "');</script>");

                    // Registro lo script per l'esportaizone del file
                    ClientScript.RegisterStartupScript(this.GetType(), "openFile", "OpenFile();", true);

                }
                else
                {
                    Response.Write("<script>alert('I dati immessi non hanno prodotto alcun report.')</script>");
                }
            }

        }

        #region Gestione callcontext

        /// <summary>
        /// Verifica se si è in un contesto di back
        /// </summary>
        private bool OnBackContext
        {
            get
            {
                return (this.Request.QueryString["back"] != null &&
                    this.Request.QueryString["back"] != string.Empty);
            }
        }

        #endregion

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


        /// <summary>
        /// Reperimento ultima nota visibile per il fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        private string GetUltimaNota(DocsPaWR.Fascicolo fascicolo)
        {
            // Reperimento ultima nota del fascicolo
            Note.INoteManager noteManager = Note.NoteManagerFactory.CreateInstance(DocsPAWA.DocsPaWR.OggettiAssociazioniNotaEnum.Fascicolo);

            DocsPaWR.InfoNota nota = noteManager.GetUltimaNota();
            if (nota != null)
                return nota.Testo;
            else
                return string.Empty;
        }

        protected void img_cercaSottoFasc_Click(object sender, ImageClickEventArgs e)
        {
            string idFascicolo = Fasc.systemID;
            RegisterStartupScript("openModale", "<script>ApriRicercaSottoFascicoli('" + idFascicolo + "','" + "" + "')</script>");
            //RegisterStartupScript("openModale", "<script>ApriRicercaSottoFascicoli('" + idFascicolo + "')</script>");
        }

        private void img_btnStoriaDiagrammi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (FascicoliManager.getFascicoloSelezionato(this) != null && FascicoliManager.getFascicoloSelezionato(this).systemID != "")
            {
                DataSet storico = DocsPAWA.DiagrammiManager.getDiagrammaStoricoFasc(FascicoliManager.getFascicoloSelezionato(this).systemID, this);
                if (storico == null || storico.Tables[0].Rows.Count == 0)
                {
                    RegisterStartupScript("chiudiFinestra", "<script>alert('Non esiste uno storico degli stati per il fascicolo corrente !');</script>");
                }
                else
                {
                    RegisterStartupScript("apriStoricoStati", "<script>apriStoricoStati();</script>");
                }
            }
        }

        #region Area Conservazione

        private void btn_storiaFasc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string idProject = Fasc.systemID;
            RegisterStartupScript("openStoriaConsFasc", "<SCRIPT>ApriStoriaConservazione('" + idProject + "');</SCRIPT>");
        }

        #endregion

        //protected void btn_exp_fasc_Click(object sender, ImageClickEventArgs e)
        //{
        //    // Ricavo il system id del fascicolo correntemente aperto
        //    string idFascicolo = this.Fasc.systemID;
        //    // Richiamo la funzione javascript per l'apertura del popup modale per
        //    // l'esportazione del fascicolo
        //    RegisterStartupScript("openModale", "<script>OpenPopUpExportFasc('" + idFascicolo + "'" + ")</script>");

        //}





        protected void btn_modifica_Click(object sender, ImageClickEventArgs e)
        {

            this.EnableModify();
            //  apriCampiProfilati(true);

        }

        #region modifica fascicolo

        #region Gestione sessione fascicolo in lavorazione

        /// <summary>
        /// Il fascicolo correntemente selezionato viene clonato
        /// per creare una nuova copia in memoria "lavorabile"
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        protected DocsPaWR.Fascicolo CloneFascicolo(DocsPaWR.Fascicolo fascicolo)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(DocsPaWR.Fascicolo));

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                serializer.Serialize(stream, fascicolo);
                stream.Position = 0;
                return (DocsPaWR.Fascicolo)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Chiave di sessione utilizzata per mantenere una copia in memoria
        /// del fascicolo correntemente in modifica
        /// </summary>
        private const string FASCICOLO_IN_LAVORAZIONE_SESSION_KEY = "modificaFascicolo.fascicoloInLavorazione";

        /// <summary>
        /// Impostazione / reperimento del fascicolo in lavorazione per il dettaglio corrente
        /// </summary>
        /// <param name="fascicolo"></param>
        protected DocsPaWR.Fascicolo FascicoloInLavorazione
        {
            get
            {
                if (this.Session[FASCICOLO_IN_LAVORAZIONE_SESSION_KEY] != null)
                    return (DocsPaWR.Fascicolo)this.Session[FASCICOLO_IN_LAVORAZIONE_SESSION_KEY];
                else
                    return null;
            }
            set
            {
                this.Session[FASCICOLO_IN_LAVORAZIONE_SESSION_KEY] = value;
            }
        }

        #endregion

        #region Gestione del fascicolo in modalità readonly

        /// <summary>
        /// Disabilitazione di tutti i controlli grafici
        /// del dettaglio se il fascicolo è in sola lettura
        /// </summary>
        private void DisableControlsIfReadOnlyMode()
        {
            // Verifica se il fascicolo in lavorazione è in modalità readonly
            bool readOnly = (this.FascicoloInLavorazione.accessRights == "45");

            foreach (Control control in this.Form.Controls)
            {
                if (control is WebControl)
                {
                    ((WebControl)control).Enabled = !readOnly;
                }
            }
        }

        /// <summary>
        /// Gestione abilitazione / disabilitazione pulsante salva
        /// </summary>
        private void EnableButtonSalva()
        {
            // Il pulsante è abilitato solo se il controllo del dettaglio della nota 
            // risulta abilitato
            this.btn_Salva.Enabled = this.dettaglioNota.Enabled;
        }


        /// <summary>
        /// Aggiornamento in batch delle sole note
        /// </summary>
        protected virtual void UpdateNote()
        {
            DocsPaWR.Fascicolo fascicolo = this.FascicoloInLavorazione;

            DocsPaWR.AssociazioneNota oggettoAssociato = new DocsPaWR.AssociazioneNota();
            oggettoAssociato.TipoOggetto = DocsPaWR.OggettiAssociazioniNotaEnum.Fascicolo;
            oggettoAssociato.Id = fascicolo.systemID;

            // Inserimento della nota creata
            this.dettaglioNota.Save();

            // Aggiornamento delle note sul backend
            fascicolo.noteFascicolo = Note.NoteManager.Update(oggettoAssociato, fascicolo.noteFascicolo);

            // Disabilitazione del dettaglio nota e del pulsante salva
            this.dettaglioNota.Enabled = false;
            this.btn_Salva.Enabled = false;
        }

        #endregion


        private bool controllaStatoFinale()
        {
            DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"];
            for (int i = 0; i < dg.STATI.Length; i++)
            {
                DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)dg.STATI[i];
                if (st.SYSTEM_ID.ToString() == ddl_statiSuccessivi.SelectedValue && st.STATO_FINALE)
                    return true;
            }
            return false;
        }
        private string ChechInput()
        {
            string msg = "";
            if ((this.txt_cod_uff_ref.Text.Equals("") || this.txt_cod_uff_ref.Text == null)
                || (this.txt_desc_uff_ref.Text.Equals("") || this.txt_desc_uff_ref.Text == null))
            {
                msg = "Inserire il valore: Ufficio Referente";
            }
            return msg;
        }
        private bool CheckUoReferenteFascicoli()
        {
            bool result = true;
            try
            {
                DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
                DocsPaWR.Corrispondente corrRef = FascicoliManager.getUoReferenteSelezionato(this);
                if (corrRef != null)
                {
                    if (!docsPaWS.UOHasReferenceRole(corrRef.systemId))
                    {
                        result = false;
                    }
                }
                else
                {
                    //necessario per controllare il caso in cui si salvi la modifica di un fascicolo
                    //specificando la stessa UO di prima(non è detto che in questo momento possieda Ruoli di Riferimento)

                    //devo leggere l'ufficio referente attraverso la seguente chiamata a webservices a causa dell'anomalia che non salva in fase di caricamento della pagina 
                    //l'ufficio referente nell'oggetto Fasc
                    DocsPaWR.Fascicolo f = wws.FascicolazioneGetFascicoloDaCodice(infoUt, Fasc.codice, UserManager.getRegistroSelezionato(this), true, false);

                    if (!docsPaWS.UOHasReferenceRole(f.ufficioReferente.systemId))
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
            return result;
            return true;
        }




        private bool getRagTrasmissioneUfficioReferente()
        {
            bool retValue = true;
            bool verificaRagioni;
            trasmissione = TrasmManager.getGestioneTrasmissione(this);
            utente = UserManager.getUtente(this);
            ruolo = UserManager.getRuolo(this);

            //se è null la trasmissione è nuova altrimenti è in modifica
            if (trasmissione == null)
            {
                trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
                trasmissione.systemId = null;
                trasmissione.ruolo = ruolo;
                trasmissione.utente = utente;
                trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                trasmissione.infoDocumento = null;
                trasmissione.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(FascicoliManager.getFascicoloSelezionato(this), this);
                TrasmManager.setGestioneTrasmissione(this, trasmissione);
            }

            DocsPaWR.RagioneTrasmissione ragTrasm = null;

            ragTrasm = FascicoliManager.TrasmettiFascicoloToUoReferente(ruolo, out verificaRagioni);

            if (ragTrasm == null && !verificaRagioni)
            {
                retValue = false;
            }
            else
            {
                TrasmManager.setRagioneSel(this, ragTrasm);
            }
            return retValue;
        }
        private string setCorrispondentiTrasmissione()
        {
            string esito = "";
            try
            {

                DocsPaWR.Trasmissione trasmissione = TrasmManager.getGestioneTrasmissione(this);
                //creo l'oggetto qca in caso di trasmissioni a UO
                DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
                qco.fineValidita = true;
                DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = setQCA(qco);
                DocsPaWR.Corrispondente corrRef = FascicoliManager.getUoReferenteSelezionato(this);
                if (corrRef != null)
                {
                    // se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
                    DocsPaWR.Ruolo[] listaRuoli = UserManager.getRuoliRiferimentoAutorizzati(this, qca, (DocsPAWA.DocsPaWR.UnitaOrganizzativa)corrRef);
                    if (listaRuoli != null && listaRuoli.Length > 0)
                    {
                        for (int index = 0; index < listaRuoli.Length; index++)
                            trasmissione = addTrasmissioneSingola(trasmissione, (DocsPAWA.DocsPaWR.Ruolo)listaRuoli[index]);
                    }
                    else
                    {
                        if (esito.Equals(""))
                            esito += "Ruoli di riferimento non trovati o non autorizzati nella: ";
                        esito += "\\nUO: " + corrRef.descrizione;
                    }
                }

                TrasmManager.setGestioneTrasmissione(this, trasmissione);

            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
            return esito;
        }
        public DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr)
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
            trasmissioneSingola.tipoTrasm = "S";
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = TrasmManager.getRagioneSel(this);

            // Aggiungo la lista di trasmissioniUtente
            if (corr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr.codiceRubrica);
                if (listaUtenti == null || listaUtenti.Length == 0)
                    return trasmissione;
                //ciclo per utenti se dest è gruppo o ruolo
                for (int i = 0; i < listaUtenti.Length; i++)
                {
                    DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)listaUtenti[i];
                    if (TrasmManager.getRagioneSel(this).descrizione.Equals("RISPOSTA"))
                        trasmissioneUtente.idTrasmRispSing = trasmissioneSingola.systemId;
                    trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                }
            }
            else
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)corr;
                trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
            }
            trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);

            return trasmissione;

        }
        private DocsPAWA.DocsPaWR.Corrispondente[] queryUtenti(string codiceRubrica)
        {

            //costruzione oggetto queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();

            qco.codiceRubrica = codiceRubrica;
            qco.getChildren = true;
            qco.fineValidita = true;

            qco.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;// ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];

            //corrispondenti interni
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;

            DocsPaWR.Corrispondente[] l_corrispondenti = UserManager.getListaCorrispondenti(this.Page, qco);

            return pf_getCorrispondentiFiltrati(l_corrispondenti);

        }
        private DocsPAWA.DocsPaWR.Corrispondente[] pf_getCorrispondentiFiltrati(DocsPAWA.DocsPaWR.Corrispondente[] corrispondenti)
        {
            string l_oldSystemId = "";
            System.Object[] l_objects = new System.Object[0];
            System.Object[] l_objects_ruoli = new System.Object[0];
            DocsPaWR.Ruolo[] lruolo = new DocsPAWA.DocsPaWR.Ruolo[0];
            int i = 0;
            foreach (DocsPAWA.DocsPaWR.Corrispondente t_corrispondente in corrispondenti)
            {
                string t_systemId = t_corrispondente.systemId;
                if (t_systemId != l_oldSystemId)
                {
                    l_objects = Utils.addToArray(l_objects, t_corrispondente);
                    l_oldSystemId = t_systemId;
                    i = i + 1;
                    continue;
                }
                else
                {
                    /* il corrispondente non viene aggiunto, in quanto sarebbe un duplicato 
                     * ma viene aggiunto solamente il ruolo */

                    if (t_corrispondente.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
                    {
                        if ((l_objects[i - 1]).GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
                        {
                            l_objects_ruoli = ((Utils.addToArray(((DocsPAWA.DocsPaWR.Utente)(l_objects[i - 1])).ruoli, ((DocsPAWA.DocsPaWR.Utente)t_corrispondente).ruoli[0])));
                            DocsPaWR.Ruolo[] l_ruolo = new DocsPAWA.DocsPaWR.Ruolo[l_objects_ruoli.Length];
                            ((DocsPAWA.DocsPaWR.Utente)(l_objects[i - 1])).ruoli = l_ruolo;
                            l_objects_ruoli.CopyTo(((DocsPAWA.DocsPaWR.Utente)(l_objects[i - 1])).ruoli, 0);
                        }

                    }
                }

            }

            DocsPaWR.Corrispondente[] l_corrSearch = new DocsPAWA.DocsPaWR.Corrispondente[l_objects.Length];
            l_objects.CopyTo(l_corrSearch, 0);

            return l_corrSearch;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qco"></param>
        /// <returns>DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato</returns>
        private DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato setQCA(DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente qco)
        {
            DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qcAut = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
            qcAut.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
            qcAut.idNodoTitolario = FascicoliManager.getFascicoloSelezionato(this).idClassificazione;
            qcAut.idRegistro = FascicoliManager.getFascicoloSelezionato(this).idRegistroNodoTit;
            if (qcAut.idRegistro != null && qcAut.idRegistro.Equals(""))
                qcAut.idRegistro = null;
            //cerco la ragione in base all'id che ho nella querystring
            qcAut.ragione = TrasmManager.getRagioneSel(this);
            if (TrasmManager.getGestioneTrasmissione(this) != null)
            {
                qcAut.ruolo = TrasmManager.getGestioneTrasmissione(this).ruolo;
            }
            qcAut.queryCorrispondente = qco;
            return qcAut;
        }
        #endregion

        protected void btn_Salva_Click(object sender, ImageClickEventArgs e)
        {

            // Mev Ospedale Maggiore Policlinico
            string RiclassificaRedirect = string.Empty;
            //End Mev Ospedale Maggiore Policlinico

            try
            {
                if (Session["template"] != null
                    && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null
                    && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1"
                    && Fasc.tipo.Equals("P")
                    )
                {
                    if (ProfilazioneFascManager.verificaCampiObbligatori((DocsPAWA.DocsPaWR.Templates)Session["template"]))
                    {
                        string msg = "Ci sono dei campi obligatori non valorizzati per il tipo fasciolo";
                        Response.Write("<script>alert('" + msg + "');</script>");
                        return;
                    }

                    string errorMessage = ProfilazioneFascManager.verificaOkContatoreFasc((DocsPAWA.DocsPaWR.Templates)Session["template"]);
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Response.Write("<script>alert('" + errorMessage + "');</script>");
                        return;
                    }

                    Fasc.template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                    ProfilazioneFascManager.verificaCampiPersonalizzati(this, Fasc, Folders, false);
                }

                if (this.Fasc.tipo == "G" ||
                    (this.Fasc.tipo == "P" &&
                        this.dettaglioNota.ReadOnly && this.dettaglioNota.IsDirty))
                {
                    // Aggiornamento in modalità batch delle sole note solo se:
                    // - il fascicolo è generale
                    // - il fascicolo è procedimentale ma in sola lettura
                    this.UpdateNote();
                    FascicoliManager.setFascicoloSelezionato(this.Fasc);
                }
                else
                {
                    //codice incriminato
                    if (enableUfficioRef && !Fasc.tipo.Equals("G"))
                    {
                        string msg = ChechInput();
                        if (!msg.Equals(""))
                        {
                            msg = msg.Replace("'", "\\'");
                            Response.Write("<script>alert('" + msg + "');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_uff_ref.ID + "').focus() </SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return;
                        }
                        else
                        {
                            if (!CheckUoReferenteFascicoli())
                            {
                                msg = "Il salvataggio dei dati non può essere effettuato.";
                                msg = msg + "\\nL\\'Ufficio Referente non possiede ruoli di riferimento.";
                                Response.Write("<script>alert('" + msg + "');</script>");
                                string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_uff_ref.ID + "').focus() </SCRIPT>";
                                RegisterStartupScript("focus", s);
                                return;
                            }
                            corrRef = FascicoliManager.getUoReferenteSelezionato(this);
                            if (corrRef != null)
                            {
                                if ((Fasc.ufficioReferente == null || Fasc.ufficioReferente.systemId == null) || (corrRef.systemId != Fasc.ufficioReferente.systemId))
                                {
                                    Fasc.ufficioReferente = corrRef;
                                    Fasc.daAggiornareUfficioReferente = true;
                                }
                                else
                                {
                                    Fasc.daAggiornareUfficioReferente = false;
                                }

                            }
                            else
                            {
                                Fasc.daAggiornareUfficioReferente = false;
                            }
                        }
                    }
                    //////// END CODICE INCRIMINATO
                    if (this.GetCalendarControl("txt_LFDTA").txt_Data.Text != "" && !Utils.isDate(this.GetCalendarControl("txt_LFDTA").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Il formato della data non è valido.');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_LFDTA").txt_Data.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return;
                    }

                    if (this.txt_descrizione.Text.Trim() == "")
                    {
                        string errore = "La modifica del fascicolo non può essere effettuata.\\nCi sono dei campi obbligatori non valorizzati.";
                        Response.Write("<script>alert(" + "'" + errore + "'" + ");</script>");
                        return;
                    }


                    //    //Diagrammi di stato
                    if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                    {
                        DocsPaWR.Stato statoAttuale = DocsPAWA.DiagrammiManager.getStatoFasc(FascicoliManager.getFascicoloSelezionato(this).systemID, this);
                        DocsPAWA.DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"];
                        //Stato iniziale
                        if (statoAttuale == null && dg != null)
                        {
                            DocsPAWA.DiagrammiManager.salvaModificaStatoFasc(Fasc.systemID, ddl_statiSuccessivi.SelectedValue, dg, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);
                        }

                        //Stato qualsiasi
                        if (statoAttuale != null && dg != null)
                        {
                            if (ddl_statiSuccessivi.SelectedValue != null && ddl_statiSuccessivi.SelectedValue != "" && dg != null)
                            {
                                bool statoQualsiasi = true;
                                //Controllo se lo stato selezionato è sia automatico che finale
                                if (DocsPAWA.DiagrammiManager.isStatoAuto(ddl_statiSuccessivi.SelectedValue, dg.SYSTEM_ID.ToString(), this) && controllaStatoFinale())
                                {
                                    msg_StatoFinale.Confirm("Si sta portando il fascicolo in uno stato finale.\\nIl fascicolo verrà chiuso.\\nConfermi il salvataggio ?");
                                    statoQualsiasi = false;
                                }

                                //Controllo se lo stato selezioanto è uno stato automatico
                                if (DocsPAWA.DiagrammiManager.isStatoAuto(ddl_statiSuccessivi.SelectedValue, dg.SYSTEM_ID.ToString(), this))
                                {
                                    msg_StatoAutomatico.Confirm("Lo stato selezionato è uno stato automatico.\\nConfermi il salvataggio ?");
                                    statoQualsiasi = false;
                                }

                                //Controllo se lo stato selezionato è uno stato finale
                                if (controllaStatoFinale())
                                {
                                    msg_StatoFinale.Confirm("Si sta portando il fascicolo in uno stato finale.\\nIl fascicolo verrà chiuso.\\nConfermi il salvataggio ?");
                                    statoQualsiasi = false;
                                }

                                if (statoQualsiasi)
                                {
                                    DocsPAWA.DiagrammiManager.salvaModificaStatoFasc(Fasc.systemID, ddl_statiSuccessivi.SelectedValue, dg, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);

                                    //Verifico se effettuare una tramsissione automatica assegnata allo stato
                                    if (Fasc.template != null && Fasc.template.SYSTEM_ID != 0 && Panel_DiagrammiStato.Visible)
                                    {
                                        ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAutoFasc(UserManager.getInfoUtente(this).idAmministrazione, ddl_statiSuccessivi.SelectedItem.Value, Fasc.template.SYSTEM_ID.ToString(), this));
                                        for (int i = 0; i < modelli.Count; i++)
                                        {
                                            DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                            if (mod.SINGLE == "1")
                                            {
                                                DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, Fasc, this);
                                            }
                                            else
                                            {
                                                for (int j = 0; j < mod.MITTENTE.Length; j++)
                                                {
                                                    if (mod.MITTENTE[j].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                                    {
                                                        DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, Fasc, this);
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
                    //}
                    if (ddl_statiSuccessivi.SelectedItem != null)
                        lbl_statoAttuale.Text = ddl_statiSuccessivi.SelectedItem.Text;


                    Fasc.descrizione = this.txt_descrizione.Text;
                    // Salvataggio dettagli nota
                    this.dettaglioNota.Save();
                    Fasc.cartaceo = this.chkFascicoloCartaceo.Checked;
                    if (this.chkFascicoloControllato.Checked)
                        Fasc.controllato = "0";
                    else
                        Fasc.controllato = "1";
                    DocsPaVO.LocazioneFisica.LocazioneFisica lf = FascicoliManager.DO_GetLocazioneFisica();
                    if (lf != null)
                    {
                        Fasc.idUoLF = lf.UO_ID;
                        Fasc.descrizioneUOLF = lf.Descrizione;
                        Fasc.varCodiceRubricaLF = lf.CodiceRubrica;

                        if (this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString().Trim() != "")
                        {
                            Fasc.dtaLF = this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString();
                        }
                        else
                        {
                            Fasc.dtaLF = null;
                        }
                    }

                    FascicoliManager.DO_RemoveLocazioneFisica();

                    //
                    // Mev Ospedale Maggiore Policlinico
                    // La scelta è stata quella di popolare nei casi dove previsto un parametro aggiuntivo riclassificazione dell'oggetto della VO Fascicolo.
                    // Tale parametro viene gestito nel BackEnd per effettuare l'update della tabella Project
                    
                    //Se il valore in sessione del campo Riclassifica è a true, significa che ho scelto il nuovo nodo di titolario
                    if (Session["Riclassifica"]!=null && (bool)Session["Riclassifica"])
                    {
                        //Nodo di titolario selezionato
                        RiclassificazioneSelezionata = FascicoliManager.getClassificazioneSelezionata(this);
                        
                        // Imposto il nuovo Nodo di Riclassificazione selezionato all'oggetto Fascicolo:
                        
                        Fasc.NodoRiclassificazione_Codice = RiclassificazioneSelezionata.codice;
                        Fasc.NodoRiclassificazione_SystemID = RiclassificazioneSelezionata.systemID;

                        RiclassificaRedirect = "<script language='javascript'> top.principale.iFrame_sx.document.location = 'tabGestioneFasc.aspx?tab=documenti'; </script>";
                    }
                    // End Mev Ospedale Maggiore Policlinico
                    //

                    FascicoliManager.setFascicolo(this, ref Fasc);
                    
                    FascicoliManager.setFascicoloSelezionato(this, Fasc);

                    //ufficio referente
                    if (Page.IsPostBack && enableUfficioRef)
                    {

                        if (Fasc.daAggiornareUfficioReferente == true)
                        {
                            //l'ufficio referente è stato cambiato quindi viene creata la trasmissione
                            if (!getRagTrasmissioneUfficioReferente())//si ricava la riagione di trasmissione
                            {
                                string theAlert = "<script>alert('Attenzione! Ragione di trasmissione assente per l\\'ufficio referente.";
                                theAlert = theAlert + "\\nLa trasmissione non è stata effettuata.');</script>";
                                Response.Write(theAlert);
                            }
                            else
                            {
                                //Si invia la trasmissione ai ruoli di riferimento autorizzati dell'Ufficio Referente
                                string esito = setCorrispondentiTrasmissione();
                                if (!esito.Equals(""))
                                {
                                    esito = esito.Replace("'", "\\'");
                                    Page.RegisterStartupScript("chiudi", "<script>alert('" + esito + "')</script>");
                                    esito = "";
                                }
                                else
                                {
                                    //richiamo il metodo che salva la trasmissione
                                    DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                                    if (infoUtente.delegato != null)
                                        trasmissione.delegato = ((DocsPAWA.DocsPaWR.InfoUtente)(infoUtente.delegato)).idPeople;

                                    //Nuovo metodo salvaExecuteTrasm
                                    trasmissione.daAggiornare = false;
                                    DocsPaWR.Trasmissione trasm_res = TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
                                    //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
                                    //trasmissione.daAggiornare = false;
                                    //DocsPaWR.Trasmissione trasm_res = TrasmManager.executeTrasm(this, trasmissione);
                                    if (trasm_res != null && trasm_res.ErrorSendingEmails)
                                        Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");
                                }
                            }

                            //rimozione variabili di sessione
                            TrasmManager.removeGestioneTrasmissione(this);
                            TrasmManager.removeRagioneSel(this);
                            Fasc.daAggiornareUfficioReferente = false;
                        }

                    }

                    FascicoliManager.removeUoReferenteSelezionato(this);
                }

                this.DisableModify();
                // this.apriCampiProfilati(false);

                //
                // Mev Ospedale Maggiore Policlinico
                if (!string.IsNullOrEmpty(RiclassificaRedirect))
                    Response.Write(RiclassificaRedirect);
                // End Mev Ospedale Maggiore Policlinico
                //

            }
            catch (System.Exception ex)
            {

                ErrorManager.redirect(this, ex);
            }
        }



        protected void btn_AnnullaModifiche_Click(object sender, ImageClickEventArgs e)
        {
            //this.apriCampiProfilati(false);

            this.DisableModify();
            string funct = " window.open('../fascicolo/fascDocumenti.aspx','IframeTabs'); ";

            Response.Write("<script> " + funct + "</script>");




        }

        /// <summary>
        /// Mev Ospedale Maggiore Policlinico
        /// Metodo associato al click del pulsante Riclassifica
        /// Il metodo apre la popup per scegliere il nuovo nodo di titolario in cui riclassificare il fascicolo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Riclassifica_Click(object sender, ImageClickEventArgs e)
        {
            Session["Titolario"] = "Y";
            if (!this.IsStartupScriptRegistered("apriModalDialog"))
            {
                if (FascicoloInLavorazione != null)
                {
                    //string scriptString = "<SCRIPT>ApriTitolario('codClass=" + txt_ClassFasc.Text + "&idTit=" + FascicoloInLavorazione.idTitolario + "','gestFasc')</SCRIPT>";
                    string scriptString = "<SCRIPT>ApriTitolario('codClass=" + getCodiceGerarchia(FascicoloInLavorazione) + "&idTit=" + FascicoloInLavorazione.idTitolario + "','gestRiclassFasc')</SCRIPT>";
                    this.RegisterStartupScript("apriModalDialog", scriptString);
                }
            }
        }




        protected void txt_LFCod_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txt_LFDesc.Text = "";
                if (txt_LFCod.Text != "")
                {
                    setDescCorrispondente(this.txt_LFCod.Text, true);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }
        /// <summary>
        /// setDescUffRefInModificaFascicolo:
        /// Setta la descrizione dell'Ufficio Referente nel relativo campo della form,
        /// qualora il corrispondente selezionato sia una UO, altrimenti verrà lanciato un 
        /// messaggio d'errore
        /// </summary>
        /// string codiceRubrica: codice rubrica del corrispondente
        /// <returns></returns>
        private void setDescUffRefInModificaFascicolo(string codiceRubrica)
        {
            DocsPaWR.Corrispondente corr = null;
            string msg = "Codice rubrica non valido per l\\'Ufficio referente!";
            if (!codiceRubrica.Equals(""))
                corr = UserManager.getCorrispondenteReferente(this, codiceRubrica, false);
            if (corr != null && (corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))))
            {
                this.txt_desc_uff_ref.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
                FascicoliManager.setUoReferenteSelezionato(this.Page, corr);
            }
            else
            {
                this.txt_desc_uff_ref.Text = "";
                if (!codiceRubrica.Equals(""))
                {
                    RegisterStartupScript("alert", "<script language='javascript'>alert('" + msg + "');</script>");
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_uff_ref.ID + "').focus() </SCRIPT>";
                    RegisterStartupScript("focus", s);
                }
            }

        }
        protected void txt_cod_uff_ref_TextChanged(object sender, EventArgs e)
        {
            try
            {
                setDescUffRefInModificaFascicolo(this.txt_cod_uff_ref.Text);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        protected void Folders_SelectedIndexChange(object sender, TreeViewSelectEventArgs e)
        {
            VisualizzaContenutoFolder();
        }

        private void VisualizzaContenutoFolder()
        {
            bool root = false;

            DocsPaWR.Folder selFold = this.getSelectedFolder(out root);

            if (!root)
            {

                apriListaDocs();
            }
            else
            {
                if (Session["ListaDocs-CampiProf"].ToString() == "ListaDocs")
                {
                    apriListaDocs();
                }
                else
                {
                    apriCampiProfilati(!this.btn_modifica.Enabled);
                }
            }
        }
        private void btnPrintSignature_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            settaStampaFascicolo();
                PrintSignature();
        }
        private void PrintSignature()
        {
            try { 
            
                ((StampaEtichetta)ph_stampa.Controls[0]).Stampa(false);
            }
            catch{}
          //fascicolo.StampaEtichetta stampaEtichetta = this.GetControlStampaEtichetta();
          //stampaEtichetta.Stampa(false);
        }
        //private fascicolo.StampaEtichetta GetControlStampaEtichetta()
        //{
        //    return (fascicolo.StampaEtichetta)this.FindControl("StampaEtichetta");
        //}

        /// <summary>
        /// Metodo per l'impostazione dell'icona del bottone per l'inserimento / rimozione del fascicolo 
        /// dall'area di lavoro
        /// </summary>
        protected void UpdateWorkingAreaIcon()
        {
            if (String.IsNullOrEmpty(Fasc.InAreaLavoro) || Fasc.InAreaLavoro == "0")
            {
                this.btn_addToAreaLavoro.ImageUrl = "../images/proto/ins_area.gif";
                this.btn_addToAreaLavoro.ToolTip = "Inserisci fascicolo in area di lavoro";
            }
            else
            {
                this.btn_addToAreaLavoro.ImageUrl = "../images/proto/canc_area.gif";
                this.btn_addToAreaLavoro.ToolTip = "Rimuovi fascicolo da area di lavoro";
            }
        }

        /// <summary>
        /// Indice dell'ultimo nodo selezionato
        /// </summary>
        public string SelectedFolder 
        {
            get
            {
                String retVal = String.Empty;
                if (CallContextStack.CurrentContext.ContextState["Folder.SelectedIndex"] != null)
                    retVal = CallContextStack.CurrentContext.ContextState["Folder.SelectedIndex"].ToString();
                return retVal;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState["Folder.SelectedIndex"] = value;
            }
        }
    }
}
