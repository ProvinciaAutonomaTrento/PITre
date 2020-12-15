using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DocsPaWA.UserControls;
using DocsPAWA.DocsPaWR;
using log4net;
using System.Collections.Generic;

namespace DocsPAWA.fascicolo
{
    public partial class tabFascCampiProf : DocsPAWA.CssPage
    {
        private ILog logger = LogManager.GetLogger(typeof(tabFascCampiProf));
        #region variabili
        protected DocsPaWR.Templates template;
        protected Table table;
        protected ArrayList dirittiCampiRuolo;
        protected DocsPAWA.DocsPaWR.Fascicolo fasc;
        protected Utilities.MessageBox msg_StatoAutomatico;
        protected Utilities.MessageBox msg_StatoFinale;
        protected Dictionary<string, Corrispondente> dic_Corr;
        
        /// <summary>
        /// Chiave di sessione utilizzata per mantenere una copia in memoria
        /// del fascicolo correntemente in modifica
        /// </summary>
        private const string FASCICOLO_IN_LAVORAZIONE_SESSION_KEY = "modificaFascicolo.fascicoloInLavorazione";

        #endregion
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

        private void InitializeComponent()
        {

            //this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.tabFascCampiProf_PreRender);
            this.btn_HistoryField.Attributes.Add("onclick", "ApriFinestraStoriaMod('campiProfilatiFasc');return false");
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            //Christian Palumbo 6/09/2012 INC000000096595 
            if (this.FascicoloInLavorazione != null)
                this.fasc = this.FascicoloInLavorazione;
            else
                this.fasc = FascicoliManager.getFascicoloSelezionato();

            // vecchio codice
            //// this.fasc = this.FascicoloInLavorazione;
            //this.fasc = FascicoliManager.getFascicoloSelezionato();

            if (Session["template"] == null)
                Session["template"] = fasc.template;
            if (fasc.template != null && fasc.template.SYSTEM_ID != 0)
                Session["template"] = fasc.template;


            //calcolo campi di tipo corrispondente per attivazione rubrica ajax SAB
            DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
            string count = ws.CountTipoFromTipologiaFasc("CORRISPONDENTE", fasc.template.SYSTEM_ID.ToString());
            Session.Add("CountCorr", count);

            //if (!IsPostBack)
            //{
            //Profilazione dinamica fascicoli
            string tipofasc = "";


            tipofasc = fasc.tipo;


            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1" && tipofasc.Equals("P"))
            {
                // Session.Remove("template");
                panel_profDinamica.Visible = true;
                // CaricaComboTipologiaFasc();

                bool editmode = false;

                if (Request.QueryString["editMode"] != null && Request.QueryString["editMode"].ToString() != string.Empty)
                    editmode = Boolean.Parse(Request.QueryString["editMode"].ToString());

                if (panel_Contenuto.Controls.Count == 0)
                    popolaCampiProfilati(editmode);



            }


            //if (hd_Update.Value != "")
            //{
            //    this.UpdateCampiprofilati();

            //}


        }

        private void popolaCampiProfilati(bool editMode)
        {
            template = (DocsPaWR.Templates)Session["template"];

            if (Request.QueryString["codTipologiaFasc"] != null)
            {

                if (template != null)//&& Request.QueryString["codTipologiaFasc"].ToString() == template.SYSTEM_ID.ToString())
                {
                    inserisciComponenti(!editMode, template);
                    // ddl_tipologiaFasc.SelectedValue = Request.QueryString["codTipologiaFasc"].ToString();
                }
            }
            else
            {
                logger.Debug(this.Title + "- valore codTipologiaFasc non presente nella querystring");
            }
        }

        #region InserisciCampoDiTesto
        public void inserisciCampoDiTesto(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            DocsPAWA.DocsPaWR.StoricoProfilatiOldValue oldObjText = new StoricoProfilatiOldValue();
            Label etichettaCampoDiTesto = new Label();
            TextBox txt_CampoDiTesto = new TextBox();
            if (oggettoCustom.MULTILINEA.Equals("SI"))
            {
                if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE + " *";
                }
                else
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;
                }

                etichettaCampoDiTesto.CssClass = "titolo_scheda";

                //txt_CampoDiTesto.Width = 450;
                txt_CampoDiTesto.Width = Unit.Percentage(100);
                txt_CampoDiTesto.TextMode = TextBoxMode.MultiLine;

                if (oggettoCustom.NUMERO_DI_LINEE.Equals(""))
                {
                    txt_CampoDiTesto.Height = 55;
                }
                else
                {
                    txt_CampoDiTesto.Rows = Convert.ToInt32(oggettoCustom.NUMERO_DI_LINEE);
                }

                if (oggettoCustom.NUMERO_DI_CARATTERI.Equals(""))
                {
                    txt_CampoDiTesto.MaxLength = 150;
                }
                else
                {
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }

                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;

                TableRow row_1 = new TableRow();
                TableCell cell_1 = new TableCell();
                cell_1.Controls.Add(etichettaCampoDiTesto);
                cell_1.ColumnSpan = 2;
                row_1.Cells.Add(cell_1);
                table.Rows.Add(row_1);

                TableRow row_2 = new TableRow();
                TableCell cell_2 = new TableCell();
                cell_2.Controls.Add(txt_CampoDiTesto);
                cell_2.ColumnSpan = 2;
                row_2.Cells.Add(cell_2);
                table.Rows.Add(row_2);
            }
            else
            {
                if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE + " *";
                }
                else
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;
                }

                etichettaCampoDiTesto.CssClass = "titolo_scheda";

                if (!oggettoCustom.NUMERO_DI_CARATTERI.Equals(""))
                {
                    //ATTENZIONE : La lunghezza della textBox non è speculare al numero massimo di
                    //caratteri che l'utente inserisce.
                    txt_CampoDiTesto.Width = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6;
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }
                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.CssClass = "testo_grigio";

                TableRow row = new TableRow();
                TableCell cell_1 = new TableCell();
                cell_1.Controls.Add(etichettaCampoDiTesto);
                row.Cells.Add(cell_1);
                TableCell cell_2 = new TableCell();
                cell_2.Controls.Add(txt_CampoDiTesto);
                row.Cells.Add(cell_2);
                table.Rows.Add(row);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaCampoDiTesto, txt_CampoDiTesto, oggettoCustom, template, !readOnly);
            if (template.OLD_OGG_CUSTOM[index] == null)//se true bisogna valorizzare OLD_OGG_CUSTOM[index] con 
            //i dati da inserire nello storico per questo campo
            {
                //blocco storico profilazione campi di testo 
                //salvo il valore corrente del campo di testo in oldObjCustom.
                oldObjText.IDTemplate = template.ID_TIPO_FASC;
                Fascicolo fascicolo = FascicoliManager.getFascicoloSelezionato(this);
                oldObjText.ID_Doc_Fasc = fascicolo.systemID;
                oldObjText.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                oldObjText.Valore = oggettoCustom.VALORE_DATABASE;
                oldObjText.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                InfoUtente user = UserManager.getInfoUtente(this);
                oldObjText.ID_People = user.idPeople;
                oldObjText.ID_Ruolo_In_UO = user.idCorrGlobali;
                template.OLD_OGG_CUSTOM[index] = oldObjText;
            }
        }
        #endregion inserisciCampoDiTesto
        private int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
        {
            for (int i = 0; i < rbl.Items.Count; i++)
            {
                if (rbl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }
        #region InserisciCasellaDiSelezione
        public void inserisciCasellaDiSelezione(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            DocsPAWA.DocsPaWR.StoricoProfilatiOldValue casellaSelOldObj = new StoricoProfilatiOldValue();
            Label etichettaCasellaSelezione = new Label();
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE;
            }

            //etichettaCasellaSelezione.Width = 430;
            etichettaCasellaSelezione.Width = Unit.Percentage(100);
            etichettaCasellaSelezione.CssClass = "titolo_scheda";

            CheckBoxList casellaSelezione = new CheckBoxList();
            casellaSelezione.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreElenco = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                if (i < oggettoCustom.VALORI_SELEZIONATI.Length)
                {
                    string valoreSelezionato = (string)(oggettoCustom.VALORI_SELEZIONATI[i]);
                    if (valoreElenco.ABILITATO == 1 || (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato)))
                    {
                        //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                        if (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato))
                            valoreElenco.ABILITATO = 1;

                        casellaSelezione.Items.Add(new ListItem(valoreElenco.VALORE, valoreElenco.VALORE));
                        //Valore di default
                        if (valoreElenco.VALORE_DI_DEFAULT.Equals("SI"))
                        {
                            valoreDiDefault = i;
                        }
                    }
                }
            }
            casellaSelezione.CssClass = "testo_grigio";
            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Vertical;
            }
            if (valoreDiDefault != -1)
            {
                casellaSelezione.SelectedIndex = valoreDiDefault;
            }

            if (oggettoCustom.VALORI_SELEZIONATI != null)
            {
                impostaSelezioneCaselleDiSelezione(oggettoCustom, casellaSelezione);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaCasellaSelezione, casellaSelezione, oggettoCustom, template, !readOnly);

            TableRow row_1 = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaCasellaSelezione);
            cell_1.ColumnSpan = 2;
            row_1.Cells.Add(cell_1);
            table.Rows.Add(row_1);

            TableRow row_2 = new TableRow();
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(casellaSelezione);
            cell_2.ColumnSpan = 2;
            row_2.Cells.Add(cell_2);
            table.Rows.Add(row_2);

            if (template.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
            {
                //blocco storico profilazione casella di selezione 
                casellaSelOldObj.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                casellaSelOldObj.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                //per questo oggetto faccio un merge dei valori selezionati e salvo la stringa nel db
                for (int x = 0; x < oggettoCustom.VALORI_SELEZIONATI.Length; x++)
                {
                    if (!string.IsNullOrEmpty(oggettoCustom.VALORI_SELEZIONATI[x]))
                        casellaSelOldObj.Valore += string.IsNullOrEmpty(casellaSelOldObj.Valore) ?
                            oggettoCustom.VALORI_SELEZIONATI[x] : "*#?" + oggettoCustom.VALORI_SELEZIONATI[x];
                }
                casellaSelOldObj.IDTemplate = template.ID_TIPO_FASC;
                Fascicolo fascicolo = FascicoliManager.getFascicoloSelezionato(this);
                casellaSelOldObj.ID_Doc_Fasc = fascicolo.systemID;
                InfoUtente user = UserManager.getInfoUtente(this);
                casellaSelOldObj.ID_People = user.idPeople;
                casellaSelOldObj.ID_Ruolo_In_UO = user.idCorrGlobali;
                template.OLD_OGG_CUSTOM[index] = casellaSelOldObj;
            }
        }
        #endregion inserisciCasellaDiSelezione

        #region InserisciMenuATendina
        public void inserisciMenuATendina(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            DocsPAWA.DocsPaWR.StoricoProfilatiOldValue menuOldObj = new StoricoProfilatiOldValue();
            Label etichettaMenuATendina = new Label();
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE;
            }

            etichettaMenuATendina.CssClass = "titolo_scheda";

            DropDownList menuATendina = new DropDownList();
            menuATendina.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    menuATendina.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    {
                        valoreDiDefault = i;
                    }
                }
            }
            menuATendina.CssClass = "testo_grigio";
            if (valoreDiDefault != -1)
            {
                menuATendina.SelectedIndex = valoreDiDefault;
            }
            if (!(valoreDiDefault != -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
            {
                menuATendina.Items.Insert(0, "");
            }
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                //menuATendina.SelectedIndex = Convert.ToInt32(oggettoCustom.VALORE_DATABASE);
                menuATendina.SelectedIndex = impostaSelezioneMenuATendina(oggettoCustom.VALORE_DATABASE, menuATendina);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaMenuATendina, menuATendina, oggettoCustom, template, !readOnly);

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaMenuATendina);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(menuATendina);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);

            if (template.OLD_OGG_CUSTOM[index] == null)//se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
            {
                //blocco storico profilazione del campo menu di selezione 
                //salvo il valore corrente del campo menu di selezione in oldObjCustom.
                menuOldObj.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                menuOldObj.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                menuOldObj.Valore = oggettoCustom.VALORE_DATABASE;
                InfoUtente user = UserManager.getInfoUtente(this);
                menuOldObj.ID_People = user.idPeople;
                menuOldObj.ID_Ruolo_In_UO = user.idCorrGlobali;
                menuOldObj.IDTemplate = template.ID_TIPO_FASC;
                Fascicolo fascicolo = FascicoliManager.getFascicoloSelezionato(this);
                menuOldObj.ID_Doc_Fasc = fascicolo.systemID;
                template.OLD_OGG_CUSTOM[index] = menuOldObj;
            }
        }
        #endregion inserisciMenuATendina


        #region InserisciSelezioneEsclusiva
        public void inserisciSelezioneEsclusiva(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index)
        {
            TableRow row_1 = new TableRow();
            TableCell cell_1 = new TableCell();

            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            DocsPAWA.DocsPaWR.StoricoProfilatiOldValue selezEsclOldObj = new StoricoProfilatiOldValue();
            Label etichettaSelezioneEsclusiva = new Label();
            HtmlAnchor cancella_selezioneEsclusiva = new HtmlAnchor();

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;

                cancella_selezioneEsclusiva.HRef = "javascript:clearSelezioneEsclusiva(" + oggettoCustom.SYSTEM_ID.ToString() + "," + oggettoCustom.ELENCO_VALORI.Length + ");";
                cancella_selezioneEsclusiva.InnerHtml = "<img src=\"../images/cancella.gif\" width=\"10\" height=\"10\" border=\"0\" alt=\"Cacella selezione esclusiva\" class=\"resettaSelezioneEsclusiva\">";
                cell_1.Controls.Add(cancella_selezioneEsclusiva);
            }

            //etichettaSelezioneEsclusiva.Width = 400;
            etichettaSelezioneEsclusiva.Width = Unit.Percentage(90);
            etichettaSelezioneEsclusiva.CssClass = "titolo_scheda";

            RadioButtonList selezioneEsclusiva = new RadioButtonList();
            selezioneEsclusiva.AutoPostBack = false;
            selezioneEsclusiva.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    selezioneEsclusiva.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    {
                        valoreDiDefault = i;
                    }
                }
            }
            selezioneEsclusiva.CssClass = "testo_grigio";
            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Vertical;
            }
            if (valoreDiDefault != -1)
            {
                selezioneEsclusiva.SelectedIndex = valoreDiDefault;
            }
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                //selezioneEsclusiva.SelectedIndex = Convert.ToInt32(oggettoCustom.VALORE_DATABASE);
                selezioneEsclusiva.SelectedIndex = impostaSelezioneEsclusiva(oggettoCustom.VALORE_DATABASE, selezioneEsclusiva);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSelezioneEsclusiva(etichettaSelezioneEsclusiva, selezioneEsclusiva, cancella_selezioneEsclusiva, oggettoCustom, template, !readOnly);

            cell_1.Controls.Add(etichettaSelezioneEsclusiva);
            cell_1.ColumnSpan = 2;
            row_1.Cells.Add(cell_1);
            table.Rows.Add(row_1);

            TableRow row_2 = new TableRow();
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(selezioneEsclusiva);
            cell_2.ColumnSpan = 2;
            row_2.Cells.Add(cell_2);
            table.Rows.Add(row_2);

            if (template.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
            {
                //blocco storico profilazione del campo di selezione esclusiva 
                //salvo il valore corrente del campo di selezione esclusiva in oldObjCustom.
                selezEsclOldObj.IDTemplate = template.ID_TIPO_FASC;
                Fascicolo fascicolo = FascicoliManager.getFascicoloSelezionato(this);
                selezEsclOldObj.ID_Doc_Fasc = fascicolo.systemID;
                selezEsclOldObj.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                selezEsclOldObj.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                InfoUtente user = UserManager.getInfoUtente(this);
                selezEsclOldObj.ID_People = user.idPeople;
                selezEsclOldObj.ID_Ruolo_In_UO = user.idCorrGlobali;
                selezEsclOldObj.Valore = oggettoCustom.VALORE_DATABASE;
                template.OLD_OGG_CUSTOM[index] = selezEsclOldObj;
            }
        }
        #endregion inserisciSelezioneEsclusiva
        #region InserisciContatore
        public void inserisciContatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, bool readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaContatore = new Label();
            etichettaContatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaContatore.CssClass = "titolo_scheda";

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaContatore);
            row.Cells.Add(cell_1);

            TableCell cell_2 = new TableCell();

            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            DropDownList ddl = new DropDownList();

            if (oggettoCustom.VALORE_DATABASE == null || oggettoCustom.VALORE_DATABASE == "")
            {
                DocsPaWR.Ruolo ruoloUtente = UserManager.getRuolo(this);
                DocsPaWR.Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "", "");

                switch (oggettoCustom.TIPO_CONTATORE)
                {
                    case "T":
                        break;
                    case "A":
                        etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                        etichettaDDL.CssClass = "titolo_scheda";
                        etichettaDDL.Width = 30;
                        cell_2.Controls.Add(etichettaDDL);
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "comp_profilazione_anteprima";
                        //Distinguere se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "0")
                            {
                                ListItem item = new ListItem();
                                item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = "";
                            it.Value = "";
                            ddl.Items.Add(it);
                            ddl.SelectedValue = "";
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;

                        ddl.Width = 100;
                        cell_2.Controls.Add(ddl);

                        break;
                    case "R":
                        etichettaDDL.Text = "&nbsp;RF&nbsp;";
                        etichettaDDL.CssClass = "titolo_scheda";
                        etichettaDDL.Width = 34;
                        cell_2.Controls.Add(etichettaDDL);
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "comp_profilazione_anteprima";
                        //Distinguere se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "1" && ((DocsPaWR.Registro)registriRfVisibili[i]).rfDisabled == "0")
                            {
                                ListItem item = new ListItem();
                                item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = "";
                            it.Value = "";
                            ddl.Items.Add(it);
                            ddl.SelectedValue = "";
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;

                        ddl.Width = 100;
                        cell_2.Controls.Add(ddl);

                        break;
                }


            }

            TextBox contatore = new TextBox();
            contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
            if (oggettoCustom.FORMATO_CONTATORE != "")
            {
                contatore.Text = oggettoCustom.FORMATO_CONTATORE;
                if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                {
                    if (!string.IsNullOrEmpty(oggettoCustom.ANNO_ACC))
                    {
                        
                        contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO_ACC);
                    }
                    else
                    { contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO); }
                    
                    contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);
                    string codiceAmministrazione = UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice;
                    contatore.Text = contatore.Text.Replace("COD_AMM", codiceAmministrazione);
                    contatore.Text = contatore.Text.Replace("COD_UO", oggettoCustom.CODICE_DB);

                    if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
                    {
                        int fine = oggettoCustom.DATA_INSERIMENTO.LastIndexOf(".");
                        if (fine > 0)
                        {
                            contatore.Text = contatore.Text.Replace("gg/mm/aaaa hh:mm",
                                                                    oggettoCustom.DATA_INSERIMENTO.Substring(0, fine));
                        }
                        contatore.Text = contatore.Text.Replace("gg/mm/aaaa", oggettoCustom.DATA_INSERIMENTO.Substring(0, 10));
                    }

                    if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
                    {
                        DocsPAWA.DocsPaWR.Registro reg = UserManager.getRegistroBySistemId(this, oggettoCustom.ID_AOO_RF);
                        if (reg != null)
                        {
                            contatore.Text = contatore.Text.Replace("RF", reg.codRegistro);
                            contatore.Text = contatore.Text.Replace("AOO", reg.codRegistro);
                        }
                    }
                }
                else
                {
                    contatore.Text = "";
                    //contatore.Text = contatore.Text.Replace("ANNO", "");
                    //contatore.Text = contatore.Text.Replace("CONTATORE", "");
                    //contatore.Text = contatore.Text.Replace("RF", "");
                    //contatore.Text = contatore.Text.Replace("AOO", "");
                }
            }
            else
            {
                contatore.Text = oggettoCustom.VALORE_DATABASE;
            }

            CheckBox cbContaDopo = new CheckBox();

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloContatore(etichettaContatore, contatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, template, !readOnly);

            contatore.Width = Unit.Percentage(100);
            contatore.Enabled = true;
            contatore.ReadOnly = true;

            //contatore.BackColor = System.Drawing.Color.WhiteSmoke;
            contatore.CssClass = "testo_grigio";
            contatore.Style.Add("TEXT-ALIGN", "left");
            cell_2.Controls.Add(contatore);
            row.Cells.Add(cell_2);

            //Inserisco il cb per il conta dopo
            if (oggettoCustom.CONTA_DOPO == "1")
            {
                cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
                cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";
                cell_2.Controls.Add(cbContaDopo);
            }

            table.Rows.Add(row);
        }
        #endregion inserisciContatore

        #region InserisciData
        public void inserisciData(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index)
        {
            //Per il momento questo tipo di campo è stato implementato con tre semplici textBox
            //Sarebbe opportuno creare un oggetto personalizzato, che espone le stesse funzionalità
            //della textBox, ma che mi permette di gestire la data con i tre campi separati.
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            DocsPAWA.DocsPaWR.StoricoProfilatiOldValue dataOldOb = new StoricoProfilatiOldValue();
            Label etichettaData = new Label();
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaData.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaData.Text = oggettoCustom.DESCRIZIONE;
            }
            etichettaData.CssClass = "titolo_scheda";

            DocsPAWA.UserControls.Calendar data = (DocsPAWA.UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data.fromUrl = "../UserControls/DialogCalendar.aspx";
            data.CSS = "testo_grigio";
            data.ID = oggettoCustom.SYSTEM_ID.ToString();
            data.VisibleTimeMode = ProfilazioneFascManager.getVisibleTimeMode(oggettoCustom);

            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                //data.txt_Data.Text = oggettoCustom.VALORE_DATABASE;
                data.Text = oggettoCustom.VALORE_DATABASE;

            //Verifico i diritti del ruolo sul campo

            impostaDirittiRuoloSulCampo(etichettaData, data, oggettoCustom, template, !readOnly);

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaData);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(data);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
            data.ReadOnly = readOnly;
            data.EnableBtnCal = !readOnly;

            if (template.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
            {
                //blocco storico profilazione campo data
                dataOldOb.IDTemplate = template.ID_TIPO_FASC;
                Fascicolo fascicolo = FascicoliManager.getFascicoloSelezionato(this);
                dataOldOb.ID_Doc_Fasc = fascicolo.systemID;
                dataOldOb.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                dataOldOb.Valore = oggettoCustom.VALORE_DATABASE;
                dataOldOb.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                InfoUtente user = UserManager.getInfoUtente(this);
                dataOldOb.ID_People = user.idPeople;
                dataOldOb.ID_Ruolo_In_UO = user.idCorrGlobali;
                template.OLD_OGG_CUSTOM[index] = dataOldOb;
            }
        }
        #endregion inserisciData

        #region inserisciCorrispondente
        public void inserisciCorrispondente(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            DocsPAWA.DocsPaWR.StoricoProfilatiOldValue corrOldOb = new StoricoProfilatiOldValue();
            Label etichetta = new Label();
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            //etichetta.Font.Size = FontUnit.Point(8);
            //etichetta.Font.Bold = true;
            //etichetta.Font.Name = "Verdana";
            etichetta.CssClass = "titolo_scheda";

            DocsPAWA.UserControls.Corrispondente corrispondente = (DocsPAWA.UserControls.Corrispondente)this.LoadControl("../UserControls/Corrispondente.ascx");
            corrispondente.CSS_CODICE = "testo_grigio";
            corrispondente.CSS_DESCRIZIONE = "testo_grigio";
            corrispondente.DESCRIZIONE_READ_ONLY = true;
            corrispondente.TIPO_CORRISPONDENTE = oggettoCustom.TIPO_RICERCA_CORR;
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();

            if (Session["dictionaryCorrispondente"] != null)
                dic_Corr = (Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>)Session["dictionaryCorrispondente"];

            if (dic_Corr != null && dic_Corr.ContainsKey(corrispondente.ID) && dic_Corr[corrispondente.ID] != null)
            {
                corrispondente.SYSTEM_ID_CORR = dic_Corr[corrispondente.ID].systemId;
                corrispondente.CODICE_TEXT = dic_Corr[corrispondente.ID].codiceRubrica;
                corrispondente.DESCRIZIONE_TEXT = dic_Corr[corrispondente.ID].descrizione;
                oggettoCustom.VALORE_DATABASE = dic_Corr[corrispondente.ID].systemId;
            }
            else
            {
                //Da amministrazione è stato impostato un ruolo di default per questo campo.
                if (oggettoCustom.ID_RUOLO_DEFAULT != null && oggettoCustom.ID_RUOLO_DEFAULT != "" && oggettoCustom.ID_RUOLO_DEFAULT != "0")
                {
                    DocsPaWR.Ruolo ruolo = (DocsPaWR.Ruolo)UserManager.getRuoloById(oggettoCustom.ID_RUOLO_DEFAULT, this);
                    if (ruolo != null)
                    {
                        corrispondente.SYSTEM_ID_CORR = ruolo.systemId;
                        corrispondente.CODICE_TEXT = ruolo.codiceRubrica;
                        corrispondente.DESCRIZIONE_TEXT = ruolo.descrizione;
                    }
                    oggettoCustom.ID_RUOLO_DEFAULT = "0";
                }
                
                //Il campo è valorizzato.
                if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                {
                    DocsPAWA.DocsPaWR.Corrispondente corr_1 = (DocsPAWA.DocsPaWR.Corrispondente)UserManager.getCorrispondenteBySystemIDDisabled(this, oggettoCustom.VALORE_DATABASE);
                    if (corr_1 != null)
                    {
                        corrispondente.SYSTEM_ID_CORR = corr_1.systemId;
                        corrispondente.CODICE_TEXT = corr_1.codiceRubrica.ToString();
                        corrispondente.DESCRIZIONE_TEXT = corr_1.descrizione.ToString();
                        oggettoCustom.VALORE_DATABASE = corr_1.systemId;
                        if (dic_Corr == null)
                            dic_Corr = new Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>();
                        dic_Corr[corrispondente.ID] = corr_1;
                        Session["dictionaryCorrispondente"] = dic_Corr;
                    }
                }
                
                //E' stato selezionato un corrispondente da rubrica.
                if (Session["rubrica.campoCorrispondente"] != null)
                {
                    DocsPAWA.DocsPaWR.Corrispondente corr_3 = (DocsPAWA.DocsPaWR.Corrispondente)Session["rubrica.campoCorrispondente"];
                    if (corr_3 != null)
                    {
                        //Verifico che l'id del campo sia quello che mi interessa.
                        //Questo id viene messo in sessione dallo UserControl e serve a 
                        //distinguere i diversi campi corrispondete che una popup di profilazione puo' contenere
                        if (Session["rubrica.idCampoCorrispondente"] != null && Session["rubrica.idCampoCorrispondente"].ToString() == corrispondente.ID)
                        {
                            corrispondente.SYSTEM_ID_CORR = corr_3.systemId;
                            corrispondente.CODICE_TEXT = corr_3.codiceRubrica;
                            corrispondente.DESCRIZIONE_TEXT = corr_3.descrizione;
                            if (dic_Corr == null)
                                dic_Corr = new Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>();
                            dic_Corr[corrispondente.ID] = corr_3;
                            oggettoCustom.VALORE_DATABASE = corr_3.systemId;
                            Session.Remove("rubrica.campoCorrispondente");
                            Session["noRicercaCodice"] = true;
                            Session["noRicercaDesc"] = true;
                            Session["dictionaryCorrispondente"] = dic_Corr;
                        }
                    }
                }

                //E' stato selezionato un corrispondente dalla popup dei corrispondenti multipli.
                //if (Session["CorrSelezionatoDaMulti"] != null)
                //{
                //    DocsPAWA.DocsPaWR.Corrispondente corr_4 = (DocsPAWA.DocsPaWR.Corrispondente)Session["CorrSelezionatoDaMulti"];
                //    int idCorrMulti = 0;
                //    if (Session["idCorrMulti"] != null)
                //        idCorrMulti = (int)Session["idCorrMulti"];

                //    if (corr_4 != null && idCorrMulti.ToString().Equals(corrispondente.ID))
                //    {
                //        corrispondente.CODICE_TEXT = corr_4.codiceRubrica;
                //        corrispondente.DESCRIZIONE_TEXT = corr_4.descrizione;
                //        corrispondente.SYSTEM_ID_CORR = corr_4.systemId;
                //        if (dic_Corr == null)
                //            dic_Corr = new Dictionary<string, Corrispondente>();
                //        dic_Corr[corrispondente.ID] = corr_4;
                //        oggettoCustom.VALORE_DATABASE = corr_4.systemId;
                //        Session.Remove("CorrSelezionatoDaMulti");
                //        Session.Remove("noDoppiaRicerca");
                //        Session["dictionaryCorrispondente"] = dic_Corr;
                //        Session.Remove("idCorrMulti");
                //    }
                //}

                if (readOnly)
                {
                    corrispondente.CODICE_READ_ONLY = true;
                    corrispondente.DISABLED_CORR = true;
                }
            }

            //Verifico i diritti del ruolo sul campo
            corrispondente.WIDTH_DESCRIZIONE = 20;
            impostaDirittiRuoloSulCampo(etichetta, corrispondente, oggettoCustom, template, !readOnly);

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichetta);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            corrispondente.WIDTH_CODICE = 60;
            corrispondente.WIDTH_DESCRIZIONE = 200;
            cell_2.Controls.Add(corrispondente);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);

            if (template.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
            {
                //blocco storico profilazione campo corrispondente
                corrOldOb.IDTemplate = template.ID_TIPO_FASC;
                Fascicolo fascicolo = FascicoliManager.getFascicoloSelezionato(this);
                corrOldOb.ID_Doc_Fasc = fascicolo.systemID;
                corrOldOb.Tipo_Ogg_Custom = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                corrOldOb.ID_Oggetto = oggettoCustom.SYSTEM_ID.ToString();
                corrOldOb.Valore = corrispondente.CODICE_TEXT + "<br/>" + "----------" + "<br/>" + corrispondente.DESCRIZIONE_TEXT;
                InfoUtente user = UserManager.getInfoUtente(this);
                corrOldOb.ID_People = user.idPeople;
                corrOldOb.ID_Ruolo_In_UO = user.idCorrGlobali;
                template.OLD_OGG_CUSTOM[index] = corrOldOb;
            }
        }
        #endregion inserisciCorrispondente

        #region inserisciLink
        public void inserisciLink(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, bool readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichetta = new Label();
            if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            etichetta.CssClass = "titolo_scheda";
            DocsPAWA.UserControls.LinkDocFasc link = (DocsPAWA.UserControls.LinkDocFasc)this.LoadControl("../UserControls/LinkDocFasc.ascx");
            link.ID = oggettoCustom.SYSTEM_ID.ToString();
            link.TextCssClass = "testo_grigio";
            link.IsEsterno = (oggettoCustom.TIPO_LINK.Equals("ESTERNO"));
            link.IsFascicolo = ("FASCICOLO".Equals(oggettoCustom.TIPO_OBJ_LINK));
            link.HandleInternalDoc = HandleInternalDoc;
            link.HandleInternalFasc = HandleInternalFasc;
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, link, oggettoCustom, template, !readOnly);
            if (!readOnly) link.HideLink = true;
            link.Value = oggettoCustom.VALORE_DATABASE;
            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichetta);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(link);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
        }
        #endregion inserisciLink

        #region inserisciOggettoEsterno
        public void inserisciOggettoEsterno(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, bool readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichetta = new Label();
            if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            etichetta.CssClass = "titolo_scheda";
            DocsPaWA.UserControls.IntegrationAdapter intAd = (DocsPaWA.UserControls.IntegrationAdapter)this.LoadControl("../UserControls/IntegrationAdapter.ascx");
            intAd.ID = oggettoCustom.SYSTEM_ID.ToString();
            intAd.View = DocsPaWA.UserControls.IntegrationAdapterView.INSERT_MODIFY;
            intAd.CssClass = "testo_grigio";
            intAd.ManualInsertCssClass = "testo_red";
            intAd.IsFasc = true;
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, intAd, oggettoCustom, template, !readOnly);
            intAd.ConfigurationValue = oggettoCustom.CONFIG_OBJ_EST;
            if (!string.IsNullOrEmpty(oggettoCustom.CODICE_DB) && !string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                IntegrationAdapterValue value = new IntegrationAdapterValue(oggettoCustom.CODICE_DB, oggettoCustom.VALORE_DATABASE, oggettoCustom.MANUAL_INSERT);
                intAd.Value = value;
            }
            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichetta);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(intAd);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
        }
        #endregion inserisciOggettoEsterno

        #region imposta diritti ruolo sul campo
        public void impostaDirittiRuoloSulCampo(Object etichetta, Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, bool editMode)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneFascManager.getDirittiCampoTipologiaFasc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            ((System.Web.UI.WebControls.TextBox)campo).ReadOnly = !editMode;
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                            {
                                ((System.Web.UI.WebControls.TextBox)campo).ReadOnly = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.TextBox)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "CasellaDiSelezione":
                            ((System.Web.UI.WebControls.CheckBoxList)campo).Enabled = editMode;
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                            {
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Enabled = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "MenuATendina":
                            ((System.Web.UI.WebControls.DropDownList)campo).Enabled = editMode;
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                            {
                                ((System.Web.UI.WebControls.DropDownList)campo).Enabled = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.DropDownList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "SelezioneEsclusiva":
                            //Per la selezione esclusiva è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Contatore":
                            //Per il contatore è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Data":
                            DocsPAWA.UserControls.Calendar data = (DocsPAWA.UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
                            ((DocsPAWA.UserControls.Calendar)campo).txt_Data.ReadOnly = !editMode;
                            ((DocsPAWA.UserControls.Calendar)campo).btn_Cal.Enabled = editMode;
                            data.ReadOnly = !editMode;
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                            {
                                ((DocsPAWA.UserControls.Calendar)campo).txt_Data.ReadOnly = true;
                                ((DocsPAWA.UserControls.Calendar)campo).btn_Cal.Enabled = false;
                                data.ReadOnly = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPAWA.UserControls.Calendar)campo).txt_Data.Visible = false;
                                ((DocsPAWA.UserControls.Calendar)campo).btn_Cal.Visible = false;
                                data.ReadOnly = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Corrispondente":
                            ((DocsPAWA.UserControls.Corrispondente)campo).CODICE_READ_ONLY = !editMode;
                            ((DocsPAWA.UserControls.Corrispondente)campo).DESCRIZIONE_READ_ONLY = !editMode;

                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                            {
                                ((DocsPAWA.UserControls.Corrispondente)campo).CODICE_READ_ONLY = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPAWA.UserControls.Corrispondente)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Link":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                            {
                                ((DocsPAWA.UserControls.LinkDocFasc)campo).IsInsertModify = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            else
                            {
                                ((DocsPAWA.UserControls.LinkDocFasc)campo).IsInsertModify = true;
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPAWA.UserControls.LinkDocFasc)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (!editMode) ((DocsPAWA.UserControls.LinkDocFasc)campo).IsInsertModify = false;
                            break;
                        case "OggettoEsterno":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                            {
                                ((DocsPaWA.UserControls.IntegrationAdapter)campo).View = DocsPaWA.UserControls.IntegrationAdapterView.READ_ONLY;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            else
                            {
                                ((DocsPaWA.UserControls.IntegrationAdapter)campo).View = DocsPaWA.UserControls.IntegrationAdapterView.INSERT_MODIFY;
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPaWA.UserControls.IntegrationAdapter)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (!editMode) ((DocsPaWA.UserControls.IntegrationAdapter)campo).View = DocsPaWA.UserControls.IntegrationAdapterView.READ_ONLY;
                            break;
                    }
                }
            }
        }

        public void impostaDirittiRuoloSelezioneEsclusiva(Object etichetta, Object campo, Object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, bool editMode)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneFascManager.getDirittiCampoTipologiaFasc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);
            ((System.Web.UI.WebControls.RadioButtonList)campo).Enabled = editMode;
            ((System.Web.UI.HtmlControls.HtmlAnchor)button).Visible = editMode;
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Enabled = false;
                        ((System.Web.UI.HtmlControls.HtmlAnchor)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Visible = false;
                        ((System.Web.UI.HtmlControls.HtmlAnchor)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                }
            }
        }

        public void impostaDirittiRuoloContatore(Object etichettaContatore, Object campo, Object checkBox, Object etichettaDDL, Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, bool editMode)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneFascManager.getDirittiCampoTipologiaFasc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                ((System.Web.UI.WebControls.CheckBox)checkBox).Enabled = editMode;
                ((System.Web.UI.WebControls.DropDownList)ddl).Enabled = editMode;
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Enabled = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Enabled = false;

                        //Se il contatore è solo visibile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatore).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((System.Web.UI.WebControls.TextBox)campo).Visible = false;
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;

                        //Se il contatore non è nè visibile nè modificabile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                }
            }
        }
        #endregion

        private void inserisciComponenti(bool readOnly, DocsPaWR.Templates template)
        {
            if (template.OLD_OGG_CUSTOM.Length < 1)
                template.OLD_OGG_CUSTOM = new StoricoProfilatiOldValue[template.ELENCO_OGGETTI.Length];
            panel_Contenuto.Controls.Clear();
            table = new Table();
            table.Width = Unit.Percentage(100);
            dirittiCampiRuolo = ProfilazioneFascManager.getDirittiCampiTipologiaFasc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), this);
            for (int i = 0, index = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                ProfilazioneFascManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                {
                    case "CampoDiTesto":
                        inserisciCampoDiTesto(oggettoCustom, readOnly, index++);
                        break;
                    case "CasellaDiSelezione":
                        inserisciCasellaDiSelezione(oggettoCustom, readOnly, index++);
                        break;
                    case "MenuATendina":
                        inserisciMenuATendina(oggettoCustom, readOnly, index++);
                        break;
                    case "SelezioneEsclusiva":
                        inserisciSelezioneEsclusiva(oggettoCustom, readOnly, index++);
                        break;
                    case "Contatore":
                        inserisciContatore(oggettoCustom, readOnly);
                        break;
                    case "Data":
                        inserisciData(oggettoCustom, readOnly, index++);
                        break;
                    case "Corrispondente":
                        inserisciCorrispondente(oggettoCustom, readOnly, index++);
                        break;
                    case "Link":
                        inserisciLink(oggettoCustom, readOnly);
                        break;
                    case "OggettoEsterno":
                        inserisciOggettoEsterno(oggettoCustom, readOnly);
                        break;

                }
            }
            panel_Contenuto.Controls.Add(table);

            //controlla che vi sia almeno un campo visibile per attivare il pulsante per lo storico
            int btn_HistoryIsVisible = 0;
            foreach (AssDocFascRuoli diritti in dirittiCampiRuolo)
            {
                if (!diritti.VIS_OGG_CUSTOM.Equals("0"))
                    ++btn_HistoryIsVisible;
            }
            if (btn_HistoryIsVisible == 0)
                btn_HistoryField.Visible = false;
        }

        private int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        private void impostaSelezioneCaselleDiSelezione(DocsPAWA.DocsPaWR.OggettoCustom objCustom, CheckBoxList cbl)
        {
            for (int i = 0; i < objCustom.VALORI_SELEZIONATI.Length; i++)
            {
                for (int j = 0; j < cbl.Items.Count; j++)
                {
                    if ((string)objCustom.VALORI_SELEZIONATI[i] == cbl.Items[j].Text)
                    {
                        cbl.Items[j].Selected = true;
                    }
                }
            }
        }
        #region update

        private void UpdateCampiprofilati()
        {
            //Controllo i campi obbligatori della profilazione dinamica
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
            {
                if (controllaCampiObbligatori())
                {
                    string errore = "La modifica del fascicolo non può essere effettuata.\\nCi sono dei campi obbligatori non valorizzati.";
                    Response.Write("<script>alert(" + "'" + errore + "'" + ");</script>");
                    return;
                }
                else
                {
                    string messag = ProfilazioneFascManager.verificaOkContatoreFasc((DocsPAWA.DocsPaWR.Templates)Session["template"]);
                    if (messag != string.Empty)
                    {
                        messag = messag.Replace("'", "\\'");
                        Response.Write("<script>alert('" + messag + "');</script>");
                        return;
                    }

                    Session["template"] = template;
                    if (fasc != null)
                    {
                        fasc.template = template;
                        FascicoliManager.setFascicoloSelezionato(fasc);
                    }
                }


            }
        }
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
        private bool controllaCampiObbligatori()
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione.

            DocsPaWR.Templates template = fasc.template;// (DocsPaWR.Templates)Session["template"];
            if (template != null)
            {
                for (int j = 0; j < template.ELENCO_OGGETTI.Length; j++)
                {
                    DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[j];

                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            TextBox textBox = (TextBox)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            if (textBox.Text.Equals("") && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            {
                                return true;
                            }
                            oggettoCustom.VALORE_DATABASE = textBox.Text;
                            break;
                        case "CasellaDiSelezione":
                            CheckBoxList casellaSelezione = (CheckBoxList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            if (casellaSelezione.SelectedIndex == -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            {
                                for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                                    oggettoCustom.VALORI_SELEZIONATI[i] = null;
                                return true;
                            }

                            //Controllo eventuali selezioni
                            oggettoCustom.VALORI_SELEZIONATI = new string[oggettoCustom.ELENCO_VALORI.Length];
                            oggettoCustom.VALORE_DATABASE = "";

                            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
                            {
                                DocsPaWR.ValoreOggetto valoreOggetto = (DocsPaWR.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i];
                                foreach (ListItem valoreSelezionato in casellaSelezione.Items)
                                {
                                    if (valoreOggetto.VALORE == valoreSelezionato.Text && valoreSelezionato.Selected)
                                        oggettoCustom.VALORI_SELEZIONATI[i] = valoreSelezionato.Text;
                                }
                            }
                            break;
                        case "MenuATendina":
                            DropDownList dropDwonList = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            if (dropDwonList.SelectedItem.Text.Equals("") && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            {
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }
                            oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                            break;
                        case "SelezioneEsclusiva":
                            RadioButtonList radioButtonList = (RadioButtonList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            if ((oggettoCustom.VALORE_DATABASE == "-1" && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
                            {
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }
                            if (oggettoCustom.VALORE_DATABASE == "-1")
                            {
                                oggettoCustom.VALORE_DATABASE = "";
                            }
                            else
                            {
                                if (radioButtonList.SelectedItem != null)
                                    oggettoCustom.VALORE_DATABASE = radioButtonList.SelectedItem.Text;
                            }
                            break;
                        case "Data":
                            UserControls.Calendar data = (UserControls.Calendar)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            //if (data.txt_Data.Text.Equals("") && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            if (data.Text.Equals("") && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            {
                                //SetFocus(data.txt_Data);
                                return true;
                            }
                            //if (data.txt_Data.Text.Equals(""))
                            if (data.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            else
                                //oggettoCustom.VALORE_DATABASE = data.txt_Data.Text;
                                oggettoCustom.VALORE_DATABASE = data.Text;
                            break;
                        case "Corrispondente":
                            UserControls.Corrispondente corr = (UserControls.Corrispondente)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            DocsPaWR.Corrispondente corrispondente = new DocsPaWR.Corrispondente();

                            if (Session["dictionaryCorrispondente"] != null)
                            {
                                dic_Corr = (Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>)Session["dictionaryCorrispondente"];
                                if (dic_Corr != null && dic_Corr.ContainsKey(oggettoCustom.SYSTEM_ID.ToString()) && dic_Corr[oggettoCustom.SYSTEM_ID.ToString()] != null)
                                    oggettoCustom.VALORE_DATABASE = dic_Corr[oggettoCustom.SYSTEM_ID.ToString()].systemId;
                            }
                            else
                            {
                                if (Session["CorrSelezionatoDaMulti"] != null)
                                {
                                    DocsPaWR.Corrispondente corr1 = (DocsPaWR.Corrispondente)Session["CorrSelezionatoDaMulti"];
                                    oggettoCustom.VALORE_DATABASE = corr1.systemId;
                                }
                                else
                                {
                                    if (Session["rubrica.campoCorrispondente"] != null)
                                    {
                                        DocsPAWA.DocsPaWR.Corrispondente corr_3 = (DocsPAWA.DocsPaWR.Corrispondente)Session["rubrica.campoCorrispondente"];
                                        oggettoCustom.VALORE_DATABASE = corr_3.systemId;
                                    }
                                    else
                                    {
                                        //Correzione fatta quando un corrispondente non è visibile al destinatario di una trasmissione
                                        //if (corrispondente == null && !string.IsNullOrEmpty(corr.CODICE_TEXT))
                                            if (!string.IsNullOrEmpty(corr.SYSTEM_ID_CORR))
                                                corrispondente = UserManager.getCorrispondenteBySystemIDDisabled(this, corr.SYSTEM_ID_CORR);
                                            else
                                                corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);
                                        //Fine Correzione

                                        if ((corr.CODICE_TEXT == "" &&
                                                corr.DESCRIZIONE_TEXT == "" &&
                                                oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                                                ||
                                                (corrispondente == null &&
                                                oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                                            )
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }

                            if (corrispondente != null)
                                oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                            else
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                            ////Correzione fatta quando un corrispondente non è visibile al destinatario di una trasmissione
                            //if (corrispondente == null && !string.IsNullOrEmpty(corr.CODICE_TEXT))
                            //    corrispondente = UserManager.getCorrispondenteBySystemIDDisabled(this, corr.SYSTEM_ID_CORR);
                            ////Fine Correzione

                            //if ((corr.CODICE_TEXT == "" &&
                            //        corr.DESCRIZIONE_TEXT == "" &&
                            //        oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            //        ||
                            //        (corrispondente == null &&
                            //        oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            //    )
                            //{
                            //    return true;
                            //}
                            //if (corrispondente != null)
                            //    oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                            //else
                            //    oggettoCustom.VALORE_DATABASE = "";
                            //break;
                        case "Contatore":
                            if (oggettoCustom.VALORE_DATABASE == null || oggettoCustom.VALORE_DATABASE == "")
                            {
                                switch (oggettoCustom.TIPO_CONTATORE)
                                {
                                    case "A":
                                        DropDownList ddlAoo = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                                        oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                                        break;
                                    case "R":
                                        DropDownList ddlRf = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                                        oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                                        break;
                                }
                                if (oggettoCustom.CONTA_DOPO == "1")
                                {
                                    CheckBox cbContaDopo = (CheckBox)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo");
                                    oggettoCustom.CONTATORE_DA_FAR_SCATTARE = cbContaDopo.Checked;
                                    //Verifico che il calcolo del contatore si abilitato
                                    //In caso affermativo metto in sessione un valore che mi permetterà poi di riaprire o meno
                                    //la popup di profilazione, per far verificare il numero generato.
                                    if (cbContaDopo.Checked)
                                    {
                                        Session.Add("contaDopoChecked", true);
                                    }
                                }
                                else
                                {
                                    oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                }
                            }
                            break;
                        case "OggettoEsterno":
                            DocsPaWA.UserControls.IntegrationAdapter intAd = (DocsPaWA.UserControls.IntegrationAdapter)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            IntegrationAdapterValue value = intAd.Value;

                            if (value == null && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            {
                                return true;
                            }
                            oggettoCustom.CODICE_DB = value.Codice;
                            oggettoCustom.VALORE_DATABASE = value.Descrizione;
                            oggettoCustom.MANUAL_INSERT = value.ManualInsert;
                            break;
                    }
                }
            }
            return false;
        }


        #endregion

        private void tabFascCampiProf_PreRender(object sender, System.EventArgs e)
        {
            //if (hd_Update.Value != "")
            //{
            //    this.UpdateCampiprofilati();

            //}
            if (template != null)
            {
                for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                {
                    DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                    salvaValoreCampo(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString());
                }

                Session.Add("template", template);
            }
        }

        private void salvaValoreCampo(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
            //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"

            //  Label_Avviso.Visible = false;
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto":
                    TextBox textBox = (TextBox)panel_Contenuto.FindControl(idOggetto);
                    oggettoCustom.VALORE_DATABASE = textBox.Text;
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList casellaSelezione = (CheckBoxList)panel_Contenuto.FindControl(idOggetto);
                    //Nessuna selezione
                    if (casellaSelezione.SelectedIndex == -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    {
                        for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                            oggettoCustom.VALORI_SELEZIONATI[i] = null;
                        return;
                    }

                    //Controllo eventuali selezioni
                    oggettoCustom.VALORI_SELEZIONATI = new string[oggettoCustom.ELENCO_VALORI.Length];
                    oggettoCustom.VALORE_DATABASE = "";

                    for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
                    {
                        DocsPaWR.ValoreOggetto valoreOggetto = (DocsPaWR.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i];
                        foreach (ListItem valoreSelezionato in casellaSelezione.Items)
                        {
                            if (valoreOggetto.VALORE == valoreSelezionato.Text && valoreSelezionato.Selected)
                                oggettoCustom.VALORI_SELEZIONATI[i] = valoreSelezionato.Text;
                        }
                    }
                    break;
                case "MenuATendina":
                    DropDownList dropDwonList = (DropDownList)panel_Contenuto.FindControl(idOggetto);
                    oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)panel_Contenuto.FindControl(idOggetto);
                    if ((oggettoCustom.VALORE_DATABASE == "-1" && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        return;
                    }
                    if (oggettoCustom.VALORE_DATABASE == "-1")
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                    }
                    else
                    {
                        if (radioButtonList.SelectedItem != null)
                            oggettoCustom.VALORE_DATABASE = radioButtonList.SelectedItem.Text;
                    }
                    break;
                case "Data":
                    UserControls.Calendar data = (UserControls.Calendar)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    //if (data.txt_Data.Text.Equals("") && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    if (data.Text.Equals("") && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        return;
                    }
                    //if (data.txt_Data.Text.Equals(""))
                    if (data.Text.Equals(""))
                        oggettoCustom.VALORE_DATABASE = "";
                    else
                        //oggettoCustom.VALORE_DATABASE = data.txt_Data.Text;
                        oggettoCustom.VALORE_DATABASE = data.Text;
                    break;
                case "Corrispondente":
                    UserControls.Corrispondente corr = (UserControls.Corrispondente)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    DocsPaWR.Corrispondente corrispondente = null;
                    //Controllo se è una selezione da rubrica 
                    if (Session["dictionaryCorrispondente"] != null)
                    {
                        dic_Corr = (Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>)Session["dictionaryCorrispondente"];
                        if (dic_Corr != null && dic_Corr.ContainsKey(oggettoCustom.SYSTEM_ID.ToString()) && dic_Corr[oggettoCustom.SYSTEM_ID.ToString()] != null)
                            corrispondente = dic_Corr[oggettoCustom.SYSTEM_ID.ToString()];
                        if (corrispondente == null) // && Session["resetCorrispondente"] != null)
                        {
                            //Session.Remove("resetCorrispondente");
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                        }
                    }
                    else
                    {
                        if (Session["rubrica.campoCorrispondente"] != null)
                        {
                            corrispondente = (DocsPAWA.DocsPaWR.Corrispondente)Session["rubrica.campoCorrispondente"];
                            if (corrispondente != null)
                            {
                                if (Session["rubrica.idCampoCorrispondente"] != null && Session["rubrica.idCampoCorrispondente"].ToString() == corr.ID)
                                {
                                    corr.CODICE_TEXT = corrispondente.codiceRubrica;
                                    corr.DESCRIZIONE_TEXT = corrispondente.descrizione;
                                    Session.Remove("rubrica.campoCorrispondente");
                                    Session.Remove("rubrica.idCampoCorrispondente");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            //Controllo se è stato digitato un codice
                            //corr = (UserControls.Corrispondente)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            if (Session["CorrSelezionatoDaMulti"] != null)
                            {
                                corrispondente = (DocsPaWR.Corrispondente)Session["CorrSelezionatoDaMulti"];
                                if (corrispondente != null)
                                {
                                    oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                                    corr.CODICE_TEXT = corrispondente.codiceRubrica;
                                    corr.DESCRIZIONE_TEXT = corrispondente.descrizione;
                                    return;
                                }
                            }
                            else
                            {
                                //Correzione fatta quando un corrispondente non è visibile al destinatario di una trasmissione
                                if (corrispondente == null)
                                    if (!string.IsNullOrEmpty(corr.CODICE_TEXT))
                                        corrispondente = UserManager.getCorrispondenteBySystemIDDisabled(this, corr.SYSTEM_ID_CORR);
                                    else
                                        corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);

                                //Fine Correzione
                                if (corrispondente != null && Session["multiCorr"] == null)
                                {
                                    oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                                    corr.CODICE_TEXT = corrispondente.codiceRubrica;
                                    corr.DESCRIZIONE_TEXT = corrispondente.descrizione;
                                    return;
                                }
                            }
                        }

                        ////Controllo se è stato digitato un codice
                        //corr = (UserControls.Corrispondente)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                        //corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);

                        ////Correzione fatta quando un corrispondente non è visibile al destinatario di una trasmissione
                        //if (corrispondente == null && !string.IsNullOrEmpty(corr.CODICE_TEXT))
                        //    corrispondente = UserManager.getCorrispondenteBySystemIDDisabled(this, corr.SYSTEM_ID_CORR);
                        ////Fine Correzione

                        //if (corrispondente != null)
                        //{
                        //    oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                        //    corr.CODICE_TEXT = corrispondente.codiceRubrica;
                        //    corr.DESCRIZIONE_TEXT = corrispondente.descrizione;
                        //    return;
                        //}

                        //Eventualmente resetto i valori
                        //if (corr.CODICE_TEXT == "")
                        //{
                        //    oggettoCustom.VALORE_DATABASE = "";
                        //    corr.CODICE_TEXT = "";
                        //    corr.DESCRIZIONE_TEXT = "";
                        //    return;
                        //}
                    }
                    break;
                case "Contatore":
                    if (oggettoCustom.VALORE_DATABASE == null || oggettoCustom.VALORE_DATABASE == "")
                    {
                        switch (oggettoCustom.TIPO_CONTATORE)
                        {
                            case "A":
                                DropDownList ddlAoo = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                                oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                                break;
                            case "R":
                                DropDownList ddlRf = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                                oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                                break;
                        }
                        if (oggettoCustom.CONTA_DOPO == "1")
                        {
                            CheckBox cbContaDopo = (CheckBox)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo");
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = cbContaDopo.Checked;
                        }
                        else
                        {
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE) && oggettoCustom.FORMATO_CONTATORE.LastIndexOf("COD_UO") != -1)
                        {
                            if (UserManager.getRuolo(this) != null && UserManager.getRuolo(this).uo != null)
                                oggettoCustom.CODICE_DB = UserManager.getRuolo(this).uo.codice;
                        }
                    }
                    break;
                case "Link":
                    UserControls.LinkDocFasc link = (UserControls.LinkDocFasc)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    string value = link.Value;
                    if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI") && string.IsNullOrEmpty(value)) return;
                    if (string.IsNullOrEmpty(value))
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = value;
                    }
                    break;
                case "OggettoEsterno":
                    IntegrationAdapter intAd = (IntegrationAdapter)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    IntegrationAdapterValue val = intAd.Value;
                    if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI") && val == null) return;
                    if (val == null)
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        oggettoCustom.CODICE_DB = "";
                        oggettoCustom.MANUAL_INSERT = false;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = val.Descrizione;
                        oggettoCustom.CODICE_DB = val.Codice;
                        oggettoCustom.MANUAL_INSERT = val.ManualInsert;
                    }
                    break;
            }

            Session.Remove("whichCorr");
        }

        #region delegates per LinkDocFasc
        private void HandleInternalDoc(string idDoc)
        {
            InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(idDoc, null, this);
            if (infoDoc == null)
            {
                Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
                return;
            }
            string errorMessage = "";
            int result = DocumentManager.verificaACL("D", infoDoc.idProfile, UserManager.getInfoUtente(), out errorMessage);
            if (result != 2)
            {
                Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            }
            else
            {
                DocumentManager.setRisultatoRicerca(this, infoDoc);
                Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=protocollo&forceNewContext=true';</script>");
            }
        }

        private void HandleInternalFasc(string idFasc)
        {
            Fascicolo fasc = FascicoliManager.getFascicoloById(this, idFasc);
            if (fasc == null)
                fasc = FascicoliManager.getFascicoloDaCodice(this, idFasc);  //.getFascicoloById(this, idFasc);
            if (fasc == null)
            {
                Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
                return;
            }
            string errorMessage = "";
            int result = DocumentManager.verificaACL("F", fasc.systemID, UserManager.getInfoUtente(), out errorMessage);
            if (result != 2)
            {
                Response.Write("<script language='javascript'>alert('Non si possiedono i diritti per la visualizzazione')</script>");
            }
            else
            {
                FascicoliManager.setFascicoloSelezionato(this, fasc);
                string newUrl = "../fascicolo/gestioneFasc.aspx?tab=documenti&forceNewContext=true";
                Response.Write("<script language='javascript'>top.principale.document.location='" + newUrl + "';</script>");
            }
        }
        #endregion delegates per LinkDocFasc
    }
}
