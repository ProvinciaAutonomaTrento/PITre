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
using DocsPAWA.DocsPaWR;
using DocsPaWA.UserControls;
using DocsPAWA.UserControls;
using System.Collections.Generic;
using System.Linq;
using DocsPAWA.utils;

namespace DocsPAWA.documento
{
    public class AnteprimaProfDinamica : DocsPAWA.CssPage
    {
        protected DocsPaWebCtrlLibrary.ImageButton btn_HistoryField;
        protected System.Web.UI.WebControls.Label lbl_NomeModello;
        protected System.Web.UI.WebControls.Panel panel_Contenuto;
        protected System.Web.UI.WebControls.Label Label_Avviso;
        protected System.Web.UI.WebControls.Button btn_ConfermaProfilazione;
        protected System.Web.UI.WebControls.ImageButton btn_salva;
        protected System.Web.UI.WebControls.ImageButton btnChiudi;
        protected DocsPAWA.DocsPaWR.Templates template;
        protected System.Web.UI.WebControls.Button btn_Chiudi;
        protected bool focus;
        protected Table table;
        protected ArrayList dirittiCampiRuolo;
        protected bool isRiprodotto = false;
        protected Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente> dic_Corr;

        private void Page_Load(object sender, System.EventArgs e)
        {
            //template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            template = (DocumentManager.getDocumentoSelezionato(this).template);
            isRiprodotto = DocumentManager.getDocumentoSelezionato(this).isRiprodotto;

            //calcolo campi di tipo corrispondente per attivazione rubrica ajax SAB
            DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
            string count = ws.CountTipoFromTipologia("CORRISPONDENTE", template.DESCRIZIONE);
            Session.Add("CountCorr", count);

            lbl_NomeModello.Text = template.DESCRIZIONE.ToUpper();

            if (Session["focus"] == null)
            {
                Session.Add("focus", true);
            }

            if (Request.QueryString["pulsanti"] != null)
            {
                string pulsanti = Request.QueryString["pulsanti"];
                string[] valueButton = pulsanti.Split('-');
                inserisciComponenti(setReadOnly(valueButton));
            }
            else
            {
                SchedaDocumento sd = DocumentManager.getDocumentoInLavorazione();
                
                if (Request.QueryString["ProtoSempl"] != null && Request.QueryString["ProtoSempl"].Equals("yes"))
                {
                    this.btn_salva.Visible = true;
                    this.btnChiudi.Visible = true;
                }
                else
                {
                    this.btn_salva.Visible = false;
                    this.btnChiudi.Visible = false;
                }

                if (sd != null && sd.protocollo != null && sd.protocollo.segnatura != null && !sd.protocollo.segnatura.Equals("") && !(Session["isDocModificato"] != null && Session["isDocModificato"].ToString().ToUpper() == "TRUE"))
                    inserisciComponenti("SI");
                else
                    inserisciComponenti("NO");
            }
        }

        private void AnteprimaProfDinamica_PreRender(object sender, System.EventArgs e)
        {
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                salvaValoreCampo(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString());
            }
            Session.Add("template", template);
        }


        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.btn_ConfermaProfilazione.Click += new System.EventHandler(this.btn_ConfermaProfilazione_Click);
            this.btn_salva.Click += new ImageClickEventHandler(this.btn_salva_Click);
            this.btnChiudi.Click += new ImageClickEventHandler(this.btn_Chiudi_Click);
            this.btn_Chiudi.Click += new System.EventHandler(this.btn_Chiudi_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.AnteprimaProfDinamica_PreRender);
            this.btn_HistoryField.Attributes.Add("onclick", "ApriFinestraStoriaMod('campiProfilati');return false");
        }
        #endregion

        public string setReadOnly(string[] valueButton)
        {
            if (valueButton[0] == "False" && valueButton[1] == "False" && valueButton[2] == "False")
                return "SI";
            if (valueButton[0] == "False" && valueButton[1] == "True" && valueButton[2] == "False")
                return "NO";
            if (valueButton[0] == "True" && valueButton[1] == "False" && valueButton[2] == "False")
                return "NO";
            if (valueButton[0] == "True" && valueButton[1] == "True" && valueButton[2] == "False")
                return "NO";
            if (valueButton[0] == "False" && valueButton[1] == "False" && valueButton[2] == "True")
                return "NO";
            if (valueButton[0] == "True" && valueButton[1] == "False" && valueButton[2] == "True")
                return "NO";

            return "NO";
        }

        public void inserisciComponenti(string readOnly)
        {
            if (template.OLD_OGG_CUSTOM.Length < 1)
                template.OLD_OGG_CUSTOM = new StoricoProfilatiOldValue[template.ELENCO_OGGETTI.Length];
            table = new Table();
            table.Width = Unit.Percentage(100);
            dirittiCampiRuolo = ProfilazioneDocManager.getDirittiCampiTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), this);

            for (int i = 0, index = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];

                if (readOnly == "SI" || ((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45")
                    btn_ConfermaProfilazione.Enabled = false;
                else
                    btn_ConfermaProfilazione.Enabled = true;

                ProfilazioneDocManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

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
                    case "ContatoreSottocontatore":
                        inserisciContatoreSottocontatore(oggettoCustom, readOnly);
                        break;
                    case "Separatore":
                        inserisciCampoSeparatore(oggettoCustom);
                        break;
                    case "OggettoEsterno":
                        inserisciOggettoEsterno(oggettoCustom, readOnly);
                        break;
                }
            }

            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.Rows[i].Cells.Count == 2)
                {
                    table.Rows[i].Cells[0].Width = Unit.Percentage(30);
                    table.Rows[i].Cells[1].Width = Unit.Percentage(70);
                }
                if (table.Rows[i].Cells.Count == 1)
                    table.Rows[i].Cells[0].Width = Unit.Percentage(100);
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

        #region inserisciCampoDiTesto
        public void inserisciCampoDiTesto(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly, int index)
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

                txt_CampoDiTesto.Width = Unit.Percentage(100);
                txt_CampoDiTesto.TextMode = TextBoxMode.MultiLine;
                txt_CampoDiTesto.CssClass = "testo_grigio";

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
            impostaDirittiRuoloSulCampo(etichettaCampoDiTesto, txt_CampoDiTesto, oggettoCustom, template);

            if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI")
                txt_CampoDiTesto.ReadOnly = true;

            /*if(!(bool)Session["focus"] && txt_CampoDiTesto.Enabled != false)
			{
				Session.Add("focus",true);
                string s = "<SCRIPT language='javascript'>if(document.getElementById('" + txt_CampoDiTesto.ClientID + "')!=null){document.getElementById('" + txt_CampoDiTesto.ClientID + "').focus();} </SCRIPT>";
                Page.RegisterStartupScript("focus", s);
			}*/
            txt_CampoDiTesto.Attributes.Add("onClick", "window.document.getElementById('" + txt_CampoDiTesto.ClientID + "').focus();");
            if (template.OLD_OGG_CUSTOM[index] == null)//se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
            {
                //blocco storico profilazione campi di testo 
                //salvo il valore corrente del campo di testo in oldObjCustom.
                oldObjText.IDTemplate = template.ID_TIPO_ATTO;
                SchedaDocumento doc = DocumentManager.getDocumentoSelezionato(this);
                oldObjText.ID_Doc_Fasc = doc.systemId;
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

        #region inserisciCasellaDiSelezione
        public void inserisciCasellaDiSelezione(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly, int index)
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
            impostaDirittiRuoloSulCampo(etichettaCasellaSelezione, casellaSelezione, oggettoCustom, template);

            if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI")
                casellaSelezione.Enabled = false;

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

            //if((bool)Session["focus"] && casellaSelezione.Enabled != false)
            //{
            //    Session.Add("focus",false);	
            //}
            /*if (!(bool)Session["focus"] && casellaSelezione.Enabled != false)
            {
                Session.Add("focus", true);
                string s = "<SCRIPT language='javascript'>if(document.getElementById('" + casellaSelezione.ClientID + "')!=null){document.getElementById('" + casellaSelezione.ClientID + "').focus();} </SCRIPT>";
                Page.RegisterStartupScript("focus", s);
            }*/
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
                InfoUtente user = UserManager.getInfoUtente(this);
                casellaSelOldObj.IDTemplate = template.ID_TIPO_ATTO;
                SchedaDocumento doc = DocumentManager.getDocumentoSelezionato(this);
                casellaSelOldObj.ID_Doc_Fasc = doc.systemId;
                casellaSelOldObj.ID_People = user.idPeople;
                casellaSelOldObj.ID_Ruolo_In_UO = user.idCorrGlobali;
                template.OLD_OGG_CUSTOM[index] = casellaSelOldObj;
            }
        }
        #endregion inserisciCasellaDiSelezione

        #region inserisciMenuATendina
        public void inserisciMenuATendina(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly, int index)
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
                menuATendina.SelectedIndex = impostaSelezioneMenuATendina(oggettoCustom.VALORE_DATABASE, menuATendina);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaMenuATendina, menuATendina, oggettoCustom, template);

            if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI")
                menuATendina.Enabled = false;

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaMenuATendina);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(menuATendina);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);

            //if((bool)Session["focus"] && menuATendina.Enabled != false)
            //{
            //    Session.Add("focus",false);	
            //}
            /*if (!(bool)Session["focus"] && menuATendina.Enabled != false)
            {
                Session.Add("focus", true);
                string s = "<SCRIPT language='javascript'>if(document.getElementById('" + menuATendina.ClientID + "')){document.getElementById('" + menuATendina.ClientID + "').focus();} </SCRIPT>";
                Page.RegisterStartupScript("focus", s);
            }*/
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
                menuOldObj.IDTemplate = template.ID_TIPO_ATTO;
                SchedaDocumento doc = DocumentManager.getDocumentoSelezionato(this);
                menuOldObj.ID_Doc_Fasc = doc.systemId;
                template.OLD_OGG_CUSTOM[index] = menuOldObj;
            }
        }
        #endregion inserisciMenuATendina

        #region inserisciSelezioneEsclusiva
        public void inserisciSelezioneEsclusiva(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly, int index)
        {
            TableRow row_1 = new TableRow();
            TableCell cell_1 = new TableCell();
            DocsPAWA.DocsPaWR.StoricoProfilatiOldValue selezEsclOldObj = new StoricoProfilatiOldValue();
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaSelezioneEsclusiva = new Label();
            ImageButton cancella_selezioneEsclusiva = new ImageButton();

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;

                cancella_selezioneEsclusiva.ID = "_" + oggettoCustom.SYSTEM_ID.ToString();
                cancella_selezioneEsclusiva.ImageUrl = "../images/cancella.gif";
                cancella_selezioneEsclusiva.CssClass = "resettaSelezioneEsclusiva";
                cancella_selezioneEsclusiva.Width = 10;
                cancella_selezioneEsclusiva.Height = 10;
                cancella_selezioneEsclusiva.Click += new System.Web.UI.ImageClickEventHandler(cancella_selezioneEsclusiva_Click);
                cell_1.Controls.Add(cancella_selezioneEsclusiva);
            }

            etichettaSelezioneEsclusiva.Width = Unit.Percentage(90);
            etichettaSelezioneEsclusiva.CssClass = "titolo_scheda";

            RadioButtonList selezioneEsclusiva = new RadioButtonList();
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
                selezioneEsclusiva.SelectedIndex = impostaSelezioneEsclusiva(oggettoCustom.VALORE_DATABASE, selezioneEsclusiva);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSelezioneEsclusiva(etichettaSelezioneEsclusiva, selezioneEsclusiva, cancella_selezioneEsclusiva, oggettoCustom, template);

            if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI")
                selezioneEsclusiva.Enabled = false;

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

            //if((bool)Session["focus"] && selezioneEsclusiva.Enabled != false)
            //{
            //    Session.Add("focus",false);	
            //}
            /*if (!(bool)Session["focus"] && selezioneEsclusiva.Enabled != false)
            {
                Session.Add("focus", true);
                string s = "<SCRIPT language='javascript'>if(document.getElementById('" + selezioneEsclusiva.ClientID + "')){document.getElementById('" + selezioneEsclusiva.ClientID + "').focus();} </SCRIPT>";
                Page.RegisterStartupScript("focus", s);
            }*/
            if (template.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
            {
                //blocco storico profilazione del campo di selezione esclusiva 
                //salvo il valore corrente del campo di selezione esclusiva in oldObjCustom.
                selezEsclOldObj.IDTemplate = template.ID_TIPO_ATTO;
                SchedaDocumento doc = DocumentManager.getDocumentoSelezionato(this);
                selezEsclOldObj.ID_Doc_Fasc = doc.systemId;
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

        #region inserisciContatore
        public void inserisciContatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
                return;

            if (Session["templateRiproposto"] != null)
            {
                oggettoCustom.VALORE_DATABASE = string.Empty;
                oggettoCustom.DATA_ANNULLAMENTO = string.Empty;
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
                Ruolo ruoloUtente = UserManager.getRuolo(this);
                Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "", "");

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
                        ddl.CssClass = "testo_grigio";

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "0" && !((Registro)registriRfVisibili[i]).Sospeso && !((Registro)registriRfVisibili[i]).flag_pregresso)
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
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
                        if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI")
                            ddl.Enabled = false;
                        ddl.Width = 100;
                        cell_2.Controls.Add(ddl);
                        break;
                    case "R":
                        etichettaDDL.Text = "&nbsp;RF&nbsp;";
                        etichettaDDL.CssClass = "titolo_scheda";
                        etichettaDDL.Width = 34;
                        cell_2.Controls.Add(etichettaDDL);
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "testo_grigio";

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "1" && ((Registro)registriRfVisibili[i]).rfDisabled == "0" && !((Registro)registriRfVisibili[i]).flag_pregresso)
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Value = "";
                            it.Text = "";
                            ddl.Items.Add(it);
                            ddl.SelectedValue = "";
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                        if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI")
                            ddl.Enabled = false;
                        ddl.Width = 100;
                        cell_2.Controls.Add(ddl);
                        break;
                }
            }

            //Imposto il contatore in funzione del formato
            TextBox contatore = new TextBox();
            contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
            contatore.Attributes.Add("onClick", "window.document.getElementById('" + contatore.ClientID + "').focus();");
            if (oggettoCustom.FORMATO_CONTATORE != "")
            {
                contatore.Text = oggettoCustom.FORMATO_CONTATORE;
                if (Session["templateRiproposto"] != null)
                {
                    contatore.Text = contatore.Text.Replace("ANNO", "");
                    contatore.Text = contatore.Text.Replace("CONTATORE", "");
                    contatore.Text = contatore.Text.Replace("RF", "");
                    contatore.Text = contatore.Text.Replace("AOO", "");
                    contatore.Text = contatore.Text.Replace("COD_AMM", "");
                    contatore.Text = contatore.Text.Replace("COD_UO", "");
                    contatore.Text = contatore.Text.Replace("gg/mm/aaaa hh:mm", "");
                    contatore.Text = contatore.Text.Replace("gg/mm/aaaa", "");
                }
                else
                {
                    if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                    {
                        //controllo se il contatore è custom in tal caso visualizzo anno accademico e non il semplice anno solare
                        if (!string.IsNullOrEmpty(oggettoCustom.ANNO_ACC))
                        {
                            
                            contatore.Text = contatore.Text.Replace("ANNO",oggettoCustom.ANNO_ACC );
                        }

                        //else if (!string.IsNullOrEmpty(oggettoCustom.DATA_FINE) && string.IsNullOrEmpty(oggettoCustom.ANNO_ACC))
                        //{
                        //    string AnnoAccademico = oggettoCustom.DATA_INIZIO.ToString().Substring(6, 4) + oggettoCustom.DATA_FINE.ToString().Substring(5, 5);
                        //    contatore.Text = contatore.Text.Replace("ANNO", AnnoAccademico);
                        //}
                        else
                        { contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO); }
                        
                        contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);
                        string codiceAmministrazione = UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice;
                        contatore.Text = contatore.Text.Replace("COD_AMM", codiceAmministrazione);
                        contatore.Text = contatore.Text.Replace("COD_UO", oggettoCustom.CODICE_DB);
                        if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
                        {
                            int fine = oggettoCustom.DATA_INSERIMENTO.LastIndexOf(".");
                            if (fine == -1)
                                fine = oggettoCustom.DATA_INSERIMENTO.LastIndexOf(":");
                            contatore.Text = contatore.Text.Replace("gg/mm/aaaa hh:mm", oggettoCustom.DATA_INSERIMENTO.Substring(0, fine));
                            contatore.Text = contatore.Text.Replace("gg/mm/aaaa", oggettoCustom.DATA_INSERIMENTO.Substring(0, 10));
                        }

                        if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
                        {
                            Registro reg = UserManager.getRegistroBySistemId(this, oggettoCustom.ID_AOO_RF);
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
                        //contatore.Text = contatore.Text.Replace("COD_AMM", "");
                        //contatore.Text = contatore.Text.Replace("COD_UO", "");
                        //contatore.Text = contatore.Text.Replace("gg/mm/aaaa", "");
                    }
                }
            }
            else
            {
                contatore.Text = oggettoCustom.VALORE_DATABASE;
            }

            CheckBox cbContaDopo = new CheckBox();

            //Pulsante annulla
            Button btn_annulla = new Button();
            btn_annulla.ID = "btn_a_" + oggettoCustom.SYSTEM_ID.ToString();
            btn_annulla.Text = "Annulla";
            btn_annulla.Visible = false;
            btn_annulla.CssClass = "pulsante";
            if (!String.IsNullOrEmpty(DocumentManager.getDocumentoSelezionato().docNumber))
                btn_annulla.OnClientClick = "apriPopupAnnullamentoContatore(" + oggettoCustom.SYSTEM_ID.ToString() + "," + DocumentManager.getDocumentoSelezionato().docNumber + ");";


            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloContatore(etichettaContatore, contatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, template, btn_annulla);

            contatore.Width = Unit.Percentage(80);
            contatore.Enabled = true;
            contatore.ReadOnly = true;
            contatore.CssClass = "testo_grigio";
            contatore.Style.Add("TEXT-ALIGN", "left");
            if (oggettoCustom != null && !String.IsNullOrEmpty(oggettoCustom.DATA_ANNULLAMENTO) && Utils.isEnableRepertori(template.ID_AMMINISTRAZIONE))
            {
                contatore.Style.Add("COLOR", "red");
                contatore.Text += " -- Annullato il :" + oggettoCustom.DATA_ANNULLAMENTO;
            }

            cell_2.Controls.Add(contatore);
            cell_2.Controls.Add(btn_annulla);
            row.Cells.Add(cell_2);

            //Inserisco il cb per il conta dopo
            if (oggettoCustom.CONTA_DOPO == "1")
            {
                cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
                cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";

                if ((oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "") || (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI"))
                {
                    cbContaDopo.Checked = false;
                    cbContaDopo.Visible = false;
                    cbContaDopo.Enabled = false;
                }

                cell_2.Controls.Add(cbContaDopo);
            }

            table.Rows.Add(row);
        }

        #endregion inserisciContatore

        #region inserisciData
        public void inserisciData(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly, int index)
        {
            //Per il momento questo tipo di campo è stato implementato con tre semplici textBox
            //Sarebbe opportuno creare un oggetto personalizzato, che espone le stesse funzionalità
            //della textBox, ma che mi permette di gestire la data con i tre campi separati.
            DocsPAWA.DocsPaWR.StoricoProfilatiOldValue dataOldOb = new StoricoProfilatiOldValue();
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
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
            data.VisibleTimeMode = ProfilazioneDocManager.getVisibleTimeMode(oggettoCustom);
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                data.Text = oggettoCustom.VALORE_DATABASE;

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaData, data, oggettoCustom, template);


            /*if (!(bool)Session["focus"] && data.txt_Data.Enabled != false)
            {
                Session.Add("focus", true);
                string s = "<SCRIPT language='javascript'>if(document.getElementById('" + data.txt_Data.ClientID + "')!=null){document.getElementById('" + data.txt_Data.ClientID + "').focus();} </SCRIPT>";
                Page.RegisterStartupScript("focus", s);
            }*/
            data.txt_Data.Attributes.Add("onClick", "window.document.getElementById('" + data.txt_Data.ClientID + "').focus();");
            if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI")
            {
                TableRow row = new TableRow();
                TableCell cell_1 = new TableCell();
                cell_1.Controls.Add(etichettaData);
                row.Cells.Add(cell_1);
                TableCell cell_2 = new TableCell();
                cell_2.Controls.Add(data);
                row.Cells.Add(cell_2);
                table.Rows.Add(row);
                data.ReadOnly = true;
                data.EnableBtnCal = false;
            }
            else
            {
                TableRow row = new TableRow();
                TableCell cell_1 = new TableCell();
                cell_1.Controls.Add(etichettaData);
                row.Cells.Add(cell_1);
                TableCell cell_2 = new TableCell();
                cell_2.Controls.Add(data);
                row.Cells.Add(cell_2);
                table.Rows.Add(row);
            }

            if (template.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
            {
                //blocco storico profilazione campo data
                dataOldOb.IDTemplate = template.ID_TIPO_ATTO;
                SchedaDocumento doc = DocumentManager.getDocumentoSelezionato(this);
                dataOldOb.ID_Doc_Fasc = doc.systemId;
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
        public void inserisciCorrispondente(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly, int index)
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
            etichetta.CssClass = "titolo_scheda";

            DocsPAWA.UserControls.Corrispondente corrispondente = (DocsPAWA.UserControls.Corrispondente)this.LoadControl("../UserControls/Corrispondente.ascx");
            corrispondente.CSS_CODICE = "testo_grigio";
            corrispondente.CSS_DESCRIZIONE = "testo_grigio";
            corrispondente.DESCRIZIONE_READ_ONLY = false;
            corrispondente.TIPO_CORRISPONDENTE = oggettoCustom.TIPO_RICERCA_CORR;
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();
            corrispondente.IS_RIPRODOTTO = isRiprodotto;

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
                    //DocsPAWA.DocsPaWR.Corrispondente corr_1 = (DocsPAWA.DocsPaWR.Corrispondente)UserManager.getCorrispondenteBySystemID(this,oggettoCustom.VALORE_DATABASE);

                    //Nel caso in cui si debbano vedere dei corrispondenti che risultano ora essere disabilitati, il metodo getCorrispondenteBySystemId
                    // ritornerebbe un valore null: per questo, inserisco uun nuovo metodo che riporti anche i corrispondenti disabilitati

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
                            Session.Remove("rubrica.idCampoCorrispondente");
                            Session["noRicercaCodice"] = true;
                            Session["noRicercaDesc"] = true;
                            Session["dictionaryCorrispondente"] = dic_Corr;
                        }
                    }
                }

                //E' stato selezionato un corrispondente da multidestinatari.
                //if (Session["CorrSelezionatoDaMulti"] != null)
                //{
                //    DocsPaWR.Corrispondente corr1 = (DocsPaWR.Corrispondente)Session["CorrSelezionatoDaMulti"];
                //    int idCorrMulti = 0;
                //    if (Session["idCorrMulti"] != null)
                //        idCorrMulti = (int)Session["idCorrMulti"];

                //    if (corr1 != null && idCorrMulti.ToString().Equals(corrispondente.ID))
                //    {
                //        corrispondente.SYSTEM_ID_CORR = corr1.systemId;
                //        corrispondente.CODICE_TEXT = corr1.codiceRubrica;
                //        corrispondente.DESCRIZIONE_TEXT = corr1.descrizione;
                //        if (dic_Corr == null)
                //            dic_Corr = new Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>();
                //        dic_Corr[corrispondente.ID] = corr1;
                //        oggettoCustom.VALORE_DATABASE = corr1.systemId;
                //        Session.Remove("CorrSelezionatoDaMulti");
                //        Session["dictionaryCorrispondente"] = dic_Corr;
                //        Session.Remove("idCorrMulti");
                //    }
                //}
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, corrispondente, oggettoCustom, template);

            if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI")
            {
                corrispondente.CODICE_READ_ONLY = true;
                corrispondente.DISABLED_CORR = true;
            }

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichetta);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(corrispondente);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
            if (template.OLD_OGG_CUSTOM[index] == null) //se true bisogna valorizzare OLD_OGG_CUSTOM[index] con i dati da inserire nello storico per questo campo
            {
                //blocco storico profilazione campo corrispondente
                corrOldOb.IDTemplate = template.ID_TIPO_ATTO;
                SchedaDocumento doc = DocumentManager.getDocumentoSelezionato(this);
                corrOldOb.ID_Doc_Fasc = doc.systemId;
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
        public void inserisciLink(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
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
            impostaDirittiRuoloSulCampo(etichetta, link, oggettoCustom, template);
            SchedaDocumento sd = (SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this);
            string accessRights = sd.accessRights;
            if (accessRights == "45" || readOnly == "SI")
            {
                link.IsInsertModify = false;
            }
            if (readOnly != "SI")
            {
                bool greyWithDocNum = "G".Equals(sd.tipoProto) && !string.IsNullOrEmpty(sd.docNumber);
                if (accessRights != "45" && !greyWithDocNum)
                {
                    link.HideLink = true;
                }
                else
                {
                    link.HideLink = false;
                }
            }
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

        #region inserisciContatoreSottocontatore
        public void inserisciContatoreSottocontatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
                return;

            if (Session["templateRiproposto"] != null)
            {
                oggettoCustom.VALORE_DATABASE = string.Empty;
                oggettoCustom.VALORE_SOTTOCONTATORE = string.Empty;
                oggettoCustom.DATA_INSERIMENTO = string.Empty;
            }
            Label etichettaContatoreSottocontatore = new Label();
            etichettaContatoreSottocontatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaContatoreSottocontatore.CssClass = "titolo_scheda";

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaContatoreSottocontatore);
            row.Cells.Add(cell_1);

            TableCell cell_2 = new TableCell();

            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            DropDownList ddl = new DropDownList();

            if (oggettoCustom.VALORE_DATABASE == null || oggettoCustom.VALORE_DATABASE == "")
            {
                Ruolo ruoloUtente = UserManager.getRuolo(this);
                Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "", "");

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
                        ddl.CssClass = "testo_grigio";

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "0" && !((Registro)registriRfVisibili[i]).Sospeso)
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
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
                        if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI")
                            ddl.Enabled = false;
                        ddl.Width = 100;
                        cell_2.Controls.Add(ddl);
                        break;
                    case "R":
                        etichettaDDL.Text = "&nbsp;RF&nbsp;";
                        etichettaDDL.CssClass = "titolo_scheda";
                        etichettaDDL.Width = 34;
                        cell_2.Controls.Add(etichettaDDL);
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "testo_grigio";

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "1" && ((Registro)registriRfVisibili[i]).rfDisabled == "0")
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Value = "";
                            it.Text = "";
                            ddl.Items.Add(it);
                            ddl.SelectedValue = "";
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                        if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI")
                            ddl.Enabled = false;
                        ddl.Width = 100;
                        cell_2.Controls.Add(ddl);
                        break;
                }
            }

            //Imposto il contatore in funzione del formato
            TextBox contatore = new TextBox();
            TextBox sottocontatore = new TextBox();
            TextBox dataInserimentoSottocontatore = new TextBox();
            contatore.Attributes.Add("onClick", "window.document.getElementById('" + contatore.ClientID + "').focus();");
            sottocontatore.Attributes.Add("onClick", "window.document.getElementById('" + sottocontatore.ClientID + "').focus();");
            dataInserimentoSottocontatore.Attributes.Add("onClick", "window.document.getElementById('" + dataInserimentoSottocontatore.ClientID + "').focus();");
            contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
            sottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_sottocontatore";
            dataInserimentoSottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_dataSottocontatore";
            if (oggettoCustom.FORMATO_CONTATORE != "")
            {
                contatore.Text = oggettoCustom.FORMATO_CONTATORE;
                sottocontatore.Text = oggettoCustom.FORMATO_CONTATORE;
                if (Session["templateRiproposto"] != null)
                {
                    contatore.Text = contatore.Text.Replace("ANNO", "");
                    contatore.Text = contatore.Text.Replace("CONTATORE", "");
                    contatore.Text = contatore.Text.Replace("RF", "");
                    contatore.Text = contatore.Text.Replace("AOO", "");

                    sottocontatore.Text = sottocontatore.Text.Replace("ANNO", "");
                    sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", "");
                    sottocontatore.Text = sottocontatore.Text.Replace("RF", "");
                    sottocontatore.Text = sottocontatore.Text.Replace("AOO", "");
                }
                else
                {
                    if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) && !string.IsNullOrEmpty(oggettoCustom.VALORE_SOTTOCONTATORE))
                    {
                        contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO);
                        contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);

                        sottocontatore.Text = sottocontatore.Text.Replace("ANNO", oggettoCustom.ANNO);
                        sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_SOTTOCONTATORE);

                        if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
                        {
                            Registro reg = UserManager.getRegistroBySistemId(this, oggettoCustom.ID_AOO_RF);
                            if (reg != null)
                            {
                                contatore.Text = contatore.Text.Replace("RF", reg.codRegistro);
                                contatore.Text = contatore.Text.Replace("AOO", reg.codRegistro);

                                sottocontatore.Text = sottocontatore.Text.Replace("RF", reg.codRegistro);
                                sottocontatore.Text = sottocontatore.Text.Replace("AOO", reg.codRegistro);
                            }
                        }
                    }
                    else
                    {
                        contatore.Text = contatore.Text.Replace("ANNO", "");
                        contatore.Text = contatore.Text.Replace("CONTATORE", "");
                        contatore.Text = contatore.Text.Replace("RF", "");
                        contatore.Text = contatore.Text.Replace("AOO", "");

                        sottocontatore.Text = sottocontatore.Text.Replace("ANNO", "");
                        sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", "");
                        sottocontatore.Text = sottocontatore.Text.Replace("RF", "");
                        sottocontatore.Text = sottocontatore.Text.Replace("AOO", "");
                    }
                }
            }
            else
            {
                contatore.Text = oggettoCustom.VALORE_DATABASE;
                sottocontatore.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            }

            if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
            {
                dataInserimentoSottocontatore.Text = oggettoCustom.DATA_INSERIMENTO;
            }

            CheckBox cbContaDopo = new CheckBox();

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloContatoreSottocontatore(etichettaContatoreSottocontatore, contatore, sottocontatore, dataInserimentoSottocontatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, template);

            contatore.Width = Unit.Percentage(15);
            contatore.ReadOnly = true;
            contatore.CssClass = "testo_segnatura";
            contatore.Style.Add("TEXT-ALIGN", "right");
            cell_2.Controls.Add(contatore);
            row.Cells.Add(cell_2);

            sottocontatore.Width = Unit.Percentage(15);
            sottocontatore.ReadOnly = true;
            sottocontatore.CssClass = "testo_segnatura";
            sottocontatore.Style.Add("TEXT-ALIGN", "right");
            cell_2.Controls.Add(sottocontatore);
            row.Cells.Add(cell_2);

            dataInserimentoSottocontatore.Width = Unit.Percentage(30);
            dataInserimentoSottocontatore.ReadOnly = true;
            dataInserimentoSottocontatore.CssClass = "testo_segnatura";
            dataInserimentoSottocontatore.Style.Add("TEXT-ALIGN", "right");
            dataInserimentoSottocontatore.Visible = false;
            cell_2.Controls.Add(dataInserimentoSottocontatore);
            row.Cells.Add(cell_2);

            //Inserisco il cb per il conta dopo
            if (oggettoCustom.CONTA_DOPO == "1")
            {
                cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
                cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";

                if ((oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "") || (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI"))
                {
                    cbContaDopo.Checked = false;
                    cbContaDopo.Visible = false;
                    cbContaDopo.Enabled = false;
                }

                cell_2.Controls.Add(cbContaDopo);
            }

            table.Rows.Add(row);
        }
        #endregion inserisciContatoreSottocontatore

        #region inserisciSeparatore
        public void inserisciCampoSeparatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }

            Label etichettaCampoSeparatore = new Label();
            etichettaCampoSeparatore.CssClass = "separatore_prof_dinamica";
            etichettaCampoSeparatore.Text = oggettoCustom.DESCRIZIONE.ToUpper();

            System.Web.UI.HtmlControls.HtmlGenericControl rigaSeparatore = new HtmlGenericControl("hr");
            rigaSeparatore.Style.Value = "width=100%;";

            TableRow row_1 = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaCampoSeparatore);
            cell_1.Controls.Add(rigaSeparatore);
            cell_1.ColumnSpan = 2;
            row_1.Cells.Add(cell_1);
            row_1.Height = 50;
            table.Rows.Add(row_1);

        }
        #endregion inserisciSeparatore

        #region inserisciOggettoEsterno
        public void inserisciOggettoEsterno(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
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
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, intAd, oggettoCustom, template);
            if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45" || readOnly == "SI")
                intAd.View = DocsPaWA.UserControls.IntegrationAdapterView.READ_ONLY;
            intAd.ConfigurationValue = oggettoCustom.CONFIG_OBJ_EST;
            IntegrationAdapterValue value = new IntegrationAdapterValue(oggettoCustom.CODICE_DB, oggettoCustom.VALORE_DATABASE, oggettoCustom.MANUAL_INSERT);
            intAd.Value = value;
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

        #region SetFocus
        //private void SetFocus(System.Web.UI.Control ctrl)
        //{
        //    string s = "<SCRIPT language=\"javascript\">document.getElementById('" + ctrl.ID + "').focus(); </SCRIPT>";
        //    RegisterStartupScript("focus", s);
        //}
        #endregion SetFocus

        #region cancella_selezioneEsclusiva_Click
        private void cancella_selezioneEsclusiva_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string idOggetto = (((ImageButton)sender).ID).Substring(1);
            ((RadioButtonList)panel_Contenuto.FindControl(idOggetto)).SelectedIndex = -1;
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                if (((DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i]).SYSTEM_ID.ToString().Equals(idOggetto))
                {
                    ((DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i]).VALORE_DATABASE = "-1";
                    Session.Add("template", template);
                }
            }
        }
        #endregion cancella_selezioneEsclusiva_Click

        #region impostaValori
        private int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        private int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
        {
            for (int i = 0; i < rbl.Items.Count; i++)
            {
                if (rbl.Items[i].Text == valore)
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

        private void impostaSelezioneCaselleDiSelezione(string valore, CheckBoxList cbl)
        {
            string[] caselleSelezionate = valore.Split('-');
            for (int i = 0; i < caselleSelezionate.Length; i++)
            {
                if (!caselleSelezionate[i].Equals(""))
                {
                    for (int j = 0; j < cbl.Items.Count; j++)
                    {
                        if (caselleSelezionate[i] == cbl.Items[j].Text)
                        {
                            cbl.Items[j].Selected = true;
                        }
                    }
                }
            }
        }
        #endregion

        #region Button_ConfermaProfilazione_Click - controllaCampiObbligatori - chiudi
        private void btn_ConfermaProfilazione_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                if (controllaCampiObbligatori(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString()))
                {
                    Label_Avviso.Text = "Inserire tutti i campi obbligatori !";
                    Label_Avviso.Visible = true;
                    return;
                }
                if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("Data"))
                {
                    try
                    {
                        UserControls.Calendar data = (UserControls.Calendar)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                        //if (data.txt_Data.Text != null && data.txt_Data.Text != "")
                        if (data.Text != null && data.Text != "")
                        {
                            //DateTime dataAppoggio = Convert.ToDateTime(data.txt_Data.Text);
                            DateTime dataAppoggio = Convert.ToDateTime(data.Text);
                        }
                    }
                    catch (Exception)
                    {
                        Label_Avviso.Text = "Inserire valori validi per il campo data !";
                        Label_Avviso.Visible = true;
                        return;
                    }
                }
            }
            Session.Add("template", template);
            //Session.Add("modificaProfilazione",true);
            RegisterStartupScript("chiudi finestra", "<script language=\"javascript\">chiudiFinestra();</script>");
        }

        private void btn_salva_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                if (controllaCampiObbligatori(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString()))
                {
                    Label_Avviso.Text = "Inserire tutti i campi obbligatori !";
                    Label_Avviso.Visible = true;
                    RegisterStartupScript("alert", "<script language=\"javascript\">alert('Inserire tutti i campi obbligatori !');</script>");
                    return;
                }
                if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("Data"))
                {
                    try
                    {
                        UserControls.Calendar data = (UserControls.Calendar)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                        //if (data.txt_Data.Text != null && data.txt_Data.Text != "")
                        if (data.Text != null && data.Text != "")
                        {
                            //DateTime dataAppoggio = Convert.ToDateTime(data.txt_Data.Text);
                            DateTime dataAppoggio = Convert.ToDateTime(data.Text);
                        }
                    }
                    catch (Exception)
                    {
                        Label_Avviso.Text = "Inserire valori validi per il campo data !";
                        Label_Avviso.Visible = true;
                        RegisterStartupScript("alert", "<script language=\"javascript\">alert('Inserire valori validi per il campo data !');</script>");
                        return;
                    }
                }
            }
            Session.Add("template", template);
            //Session.Add("modificaProfilazione",true);
            RegisterStartupScript("chiudi finestra", "<script language=\"javascript\">chiudiFinestra();</script>");
        }

        private bool controllaCampiObbligatori(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
            //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"

            Label_Avviso.Visible = false;
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto":
                    TextBox textBox = (TextBox)panel_Contenuto.FindControl(idOggetto);
                    if (textBox.Text.Equals("") && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    {
                        //SetFocus(textBox);
                        return true;
                    }
                    oggettoCustom.VALORE_DATABASE = textBox.Text;
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList casellaSelezione = (CheckBoxList)panel_Contenuto.FindControl(idOggetto);
                    if (casellaSelezione.SelectedIndex == -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    {
                        //SetFocus(casellaSelezione);
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
                    DropDownList dropDwonList = (DropDownList)panel_Contenuto.FindControl(idOggetto);
                    if (dropDwonList.SelectedItem.Text.Equals("") && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    {
                        //SetFocus(dropDwonList);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }
                    oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)panel_Contenuto.FindControl(idOggetto);
                    if ((oggettoCustom.VALORE_DATABASE == "-1" && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
                    {
                        //SetFocus(radioButtonList);
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
                        //if (Session["CorrSelezionatoDaMulti"] != null)
                        //{
                        //    DocsPaWR.Corrispondente corr1 = (DocsPaWR.Corrispondente)Session["CorrSelezionatoDaMulti"];
                        //    oggettoCustom.VALORE_DATABASE = corr1.systemId;
                        //}
                        //else
                        //{
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
                        //}
                    }

                    if (corrispondente != null)
                        oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                    else
                        oggettoCustom.VALORE_DATABASE = "";
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
                    }
                    break;
                case "OggettoEsterno":
                    IntegrationAdapter intAd = (IntegrationAdapter)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    if (intAd.Value == null && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void salvaValoreCampo(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
            //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"

            Label_Avviso.Visible = false;
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
                            //if (Session["CorrSelezionatoDaMulti"] != null)
                            //{
                            //    corrispondente = (DocsPaWR.Corrispondente)Session["CorrSelezionatoDaMulti"];
                            //    if (corrispondente != null)
                            //    {
                            //        oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                            //        corr.CODICE_TEXT = corrispondente.codiceRubrica;
                            //        corr.DESCRIZIONE_TEXT = corrispondente.descrizione;
                            //        return;
                            //    }
                            //}
                            //else
                            //{
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
                            //}
                        }
                    }

                    //Eventualmente resetto i valori
                    //if (corr.CODICE_TEXT == "")
                    //{
                    //    oggettoCustom.VALORE_DATABASE = "";
                    //    corr.CODICE_TEXT = "";
                    //    corr.DESCRIZIONE_TEXT = "";
                    //    return;
                    //}
                    break;
                case "Contatore":
                case "ContatoreSottocontatore":
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
                    LinkDocFasc link = (LinkDocFasc)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    oggettoCustom.VALORE_DATABASE = link.Value;
                    break;
                case "OggettoEsterno":
                    IntegrationAdapter intAd = (IntegrationAdapter)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    IntegrationAdapterValue value = intAd.Value;
                    if (value != null)
                    {
                        oggettoCustom.VALORE_DATABASE = value.Descrizione;
                        oggettoCustom.CODICE_DB = value.Codice;
                        oggettoCustom.MANUAL_INSERT = value.ManualInsert;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        oggettoCustom.CODICE_DB = "";
                        oggettoCustom.MANUAL_INSERT = false;
                    }
                    break;
            }
            //MODIFICA GIORDANO 27/03/2012
            //Modifica relativa alla rubrica ajax, rimuovendo ora la 
            //session CountCorr la pagina non sa più quanti controlli
            //custom "Corrispondenti" ci sono, quindi non scrive i relativi JS.
            //Session.Remove("CountCorr");
            //FINE MODIFICA
            Session.Remove("whichCorr");
        }


        private void btn_Chiudi_Click(object sender, System.EventArgs e)
        {
            Session.Remove("focus");
            RegisterStartupScript("chiudi finestra", "<script language=\"javascript\">chiudiFinestra();</script>");
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                controllaCampiObbligatori(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString());
            }
        }
        #endregion Button_ConfermaProfilazione_Click - controllaCampiObbligatori - chiudi

        #region imposta diritti ruolo sul campo
        public void impostaDirittiRuoloSulCampo(Object etichetta, Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                            {
                                if (Session["templateRiproposto"] != null)
                                    ((System.Web.UI.WebControls.TextBox)campo).Text = "";

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
                            if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                            {
                                if (Session["templateRiproposto"] != null)
                                    ((DocsPAWA.UserControls.Calendar)campo).Text = "";
                                ((DocsPAWA.UserControls.Calendar)campo).ReadOnly = true;
                                ((DocsPAWA.UserControls.Calendar)campo).btn_Cal.Enabled = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPAWA.UserControls.Calendar)campo).Visible = false;
                                ((DocsPAWA.UserControls.Calendar)campo).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                                ((DocsPAWA.UserControls.Calendar)campo).btn_Cal.Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Corrispondente":
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
                            break;
                    }
                }
            }
        }

        public void impostaDirittiRuoloSelezioneEsclusiva(Object etichetta, Object campo, Object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                    {
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Enabled = false;
                        ((System.Web.UI.WebControls.ImageButton)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Visible = false;
                        ((System.Web.UI.WebControls.ImageButton)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                }
            }
        }

        public void impostaDirittiRuoloContatore(Object etichettaContatore, Object campo, Object checkBox, Object etichettaDDL, Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, Button btn_annulla)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
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
                    //Gestione del pulsante annulla
                    if (Utils.isEnableRepertori(template.ID_AMMINISTRAZIONE) && assDocFascRuoli != null && assDocFascRuoli.ANNULLA_REPERTORIO == "1" && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "1" &&
                        !String.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) && String.IsNullOrEmpty(oggettoCustom.DATA_ANNULLAMENTO) && oggettoCustom.REPERTORIO.Equals("1"))
                    {
                        RepertorioState state = RegistriRepertorioUtils.GetRepertorioState(oggettoCustom.SYSTEM_ID.ToString(), oggettoCustom.TIPO_CONTATORE, oggettoCustom.ID_AOO_RF);
                        btn_annulla.Visible = state == RepertorioState.O;
                    }
                    else
                    {
                        btn_annulla.Visible = false;
                    }
                }
            }
        }

        public void impostaDirittiRuoloContatoreSottocontatore(Object etichettaContatoreSottocontatore, Object contatore, Object sottocontatore, Object dataInserimentoSottocontatore, Object checkBox, Object etichettaDDL, Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
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
                        ((System.Web.UI.WebControls.Label)etichettaContatoreSottocontatore).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((System.Web.UI.WebControls.TextBox)contatore).Visible = false;
                        ((System.Web.UI.WebControls.TextBox)sottocontatore).Visible = false;
                        ((System.Web.UI.WebControls.TextBox)dataInserimentoSottocontatore).Visible = false;
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
