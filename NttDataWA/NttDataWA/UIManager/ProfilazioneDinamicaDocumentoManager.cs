using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using NttDataWA.UserControls;

namespace NttDataWA.UIManager
{
    public class ProfilazioneDinamicaDocumentoManager
    {
        #region inserisciComponenti
        public static Panel inserisciComponenti(Panel panel, Templates template, Ruolo ruolo, InfoUtente infoutente, Page page)
        {
            // ricercaFiltro();
            try
            {
                List<AssDocFascRuoli> dirittiCampiRuolo = ProfilerDocManager.getDirittiCampiTipologiaDoc(ruolo.systemId, template.SYSTEM_ID.ToString());
                foreach (OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                {
                    ProfilerDocManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            panel = inserisciCampoDiTesto(oggettoCustom, panel, template, dirittiCampiRuolo);
                            break;
                        case "CasellaDiSelezione":
                            panel = inserisciCasellaDiSelezione(oggettoCustom, panel, template, dirittiCampiRuolo);
                            break;
                        case "MenuATendina":
                            panel = inserisciMenuATendina(oggettoCustom, panel, template, dirittiCampiRuolo);
                            break;
                        case "SelezioneEsclusiva":
                            panel = inserisciSelezioneEsclusiva(oggettoCustom, panel, template, dirittiCampiRuolo);
                            break;
                        case "Contatore":
                            panel = inserisciContatore(oggettoCustom, panel, template, dirittiCampiRuolo, ruolo, infoutente);
                            break;
                        case "Data":
                            panel = inserisciData(oggettoCustom, panel, template, dirittiCampiRuolo, page);
                            break;
                        case "Corrispondente":
                            panel = inserisciCorrispondente(oggettoCustom, panel, template, dirittiCampiRuolo, page);
                            break;
                        case "ContatoreSottocontatore":
                            panel = inserisciContatoreSottocontatore(oggettoCustom, panel, template, dirittiCampiRuolo, ruolo, page);
                            break;
                        case "OggettoEsterno":
                            panel = inserisciOggettoEsterno(oggettoCustom, panel, template, dirittiCampiRuolo, page);
                            break;
                    }
                }
                return panel;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion inserisciComponenti

        #region inserisciCampoDiTesto
        private static Panel inserisciCampoDiTesto(OggettoCustom oggettoCustom, Panel panel, Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try
            {
                if (!string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                {
                    DocsPaWR.StoricoProfilatiOldValue oldObjText = new StoricoProfilatiOldValue();

                    Label etichettaCampoDiTesto = new Label();
                    etichettaCampoDiTesto.EnableViewState = true;

                    CustomTextArea txt_CampoDiTesto = new CustomTextArea();
                    txt_CampoDiTesto.EnableViewState = true;

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
                        etichettaCampoDiTesto.CssClass = "weight";

                        txt_CampoDiTesto.CssClass = "txt_textarea";
                        txt_CampoDiTesto.CssClassReadOnly = "txt_textarea_disabled";

                        if (string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_LINEE))
                        {
                            txt_CampoDiTesto.Height = 55;
                        }
                        else
                        {
                            txt_CampoDiTesto.Rows = Convert.ToInt32(oggettoCustom.NUMERO_DI_LINEE);
                        }

                        if (string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_CARATTERI))
                        {
                            txt_CampoDiTesto.MaxLength = 150;
                        }
                        else
                        {
                            txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                        }

                        txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                        txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                        txt_CampoDiTesto.TextMode = TextBoxMode.MultiLine;


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
                        etichettaCampoDiTesto.CssClass = "weight";

                        if (!string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_CARATTERI))
                        {
                            //ATTENZIONE : La lunghezza della textBox non è speculare al numero massimo di
                            //caratteri che l'utente inserisce.
                            if (((Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6) <= 400))
                            {
                                txt_CampoDiTesto.Width = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6;
                            }
                            txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                        }
                        txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                        txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                        txt_CampoDiTesto.CssClass = "txt_input_full";
                        txt_CampoDiTesto.CssClassReadOnly = "txt_input_full_disabled";
                        txt_CampoDiTesto.TextMode = TextBoxMode.SingleLine;


                    }

                    Panel divRowDesc = new Panel();
                    divRowDesc.CssClass = "row";
                    divRowDesc.EnableViewState = true;

                    Panel divColDesc = new Panel();
                    divColDesc.CssClass = "col";
                    divColDesc.EnableViewState = true;

                    Panel divRowValue = new Panel();
                    divRowValue.CssClass = "row";
                    divRowValue.EnableViewState = true;

                    Panel divColValue = new Panel();
                    divColValue.CssClass = "col_full";
                    divColValue.EnableViewState = true;


                    //Verifico i diritti del ruolo sul campo
                    impostaDirittiRuoloSulCampo(etichettaCampoDiTesto, txt_CampoDiTesto, oggettoCustom, template, dirittiCampiRuolo);

                    if (etichettaCampoDiTesto.Visible)
                    {
                        HtmlGenericControl parDesc = new HtmlGenericControl("p");
                        parDesc.Controls.Add(etichettaCampoDiTesto);
                        parDesc.EnableViewState = true;
                        divColDesc.Controls.Add(parDesc);
                        divRowDesc.Controls.Add(divColDesc);
                        panel.Controls.Add(divRowDesc);
                    }

                    if (txt_CampoDiTesto.Visible)
                    {
                        divColValue.Controls.Add(txt_CampoDiTesto);
                        divRowValue.Controls.Add(divColValue);
                        panel.Controls.Add(divRowValue);
                    }

                }

                return panel;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion inserisciCampoDiTesto

        #region inserisciCasellaDiSelezione
        private static Panel inserisciCasellaDiSelezione(OggettoCustom oggettoCustom, Panel panel, Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try
            {
                if (!string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                {

                    DocsPaWR.StoricoProfilatiOldValue casellaSelOldObj = new StoricoProfilatiOldValue();
                    Label etichettaCasellaSelezione = new Label();
                    etichettaCasellaSelezione.EnableViewState = true;

                    if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    {
                        etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE + " *";
                    }
                    else
                    {
                        etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE;
                    }

                    etichettaCasellaSelezione.Width = Unit.Percentage(100);
                    etichettaCasellaSelezione.CssClass = "weight";

                    CheckBoxList casellaSelezione = new CheckBoxList();
                    casellaSelezione.EnableViewState = true;
                    casellaSelezione.ID = oggettoCustom.SYSTEM_ID.ToString();
                    int valoreDiDefault = -1;
                    for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
                    {
                        DocsPaWR.ValoreOggetto valoreElenco = ((ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
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

                    Panel divRowDesc = new Panel();
                    divRowDesc.CssClass = "row";
                    divRowDesc.EnableViewState = true;

                    Panel divColDesc = new Panel();
                    divColDesc.CssClass = "col";
                    divColDesc.EnableViewState = true;

                    HtmlGenericControl parDesc = new HtmlGenericControl("p");
                    parDesc.Controls.Add(etichettaCasellaSelezione);
                    parDesc.EnableViewState = true;



                    Panel divRowValue = new Panel();
                    divRowValue.CssClass = "row";
                    divRowValue.EnableViewState = true;

                    Panel divColValue = new Panel();
                    divColValue.CssClass = "col_full";
                    divColDesc.EnableViewState = true;



                    //Verifico i diritti del ruolo sul campo
                    impostaDirittiRuoloSulCampo(etichettaCasellaSelezione, casellaSelezione, oggettoCustom, template, dirittiCampiRuolo);


                    if (etichettaCasellaSelezione.Visible)
                    {
                        divColDesc.Controls.Add(parDesc);
                        divRowDesc.Controls.Add(divColDesc);
                        panel.Controls.Add(divRowDesc);
                    }

                    if (casellaSelezione.Visible)
                    {

                        divColValue.Controls.Add(casellaSelezione);
                        divRowValue.Controls.Add(divColValue);

                        //this.PnlTypeDocument.Controls.Add(divRowDesc);
                        panel.Controls.Add(divRowValue);
                    }

                }
                return panel;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion inserisciCasellaDiSelezione

        #region impostaSelezioneCaselleDiSelezione
        private static void impostaSelezioneCaselleDiSelezione(DocsPaWR.OggettoCustom objCustom, CheckBoxList cbl)
        {
            try
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);

            }
        }
        #endregion

        #region inserisciMenuATendina

        private static Panel inserisciMenuATendina(OggettoCustom oggettoCustom, Panel panel, Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try
            {
                if (!string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                {

                    DocsPaWR.StoricoProfilatiOldValue menuOldObj = new StoricoProfilatiOldValue();
                    Label etichettaMenuATendina = new Label();
                    etichettaMenuATendina.EnableViewState = true;
                    if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    {
                        etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE + " *";
                    }
                    else
                    {
                        etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE;
                    }
                    etichettaMenuATendina.CssClass = "weight";

                    int maxLenght = 0;
                    DropDownList menuATendina = new DropDownList();
                    menuATendina.EnableViewState = true;
                    menuATendina.ID = oggettoCustom.SYSTEM_ID.ToString();
                    int valoreDiDefault = -1;
                    for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
                    {
                        DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
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

                            if (maxLenght < valoreOggetto.VALORE.Length)
                            {
                                maxLenght = valoreOggetto.VALORE.Length;
                            }
                        }
                    }
                    menuATendina.CssClass = "chzn-select-deselect";
                    string language = UIManager.UserManager.GetUserLanguage();
                    menuATendina.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));
                    menuATendina.Width = maxLenght + 250;

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

                    Panel divRowDesc = new Panel();
                    divRowDesc.CssClass = "row";
                    divRowDesc.EnableViewState = true;

                    Panel divColDesc = new Panel();
                    divColDesc.CssClass = "col";

                    HtmlGenericControl parDesc = new HtmlGenericControl("p");
                    parDesc.Controls.Add(etichettaMenuATendina);
                    parDesc.EnableViewState = true;



                    Panel divRowValue = new Panel();
                    divRowValue.CssClass = "row";
                    divRowValue.EnableViewState = true;

                    Panel divColValue = new Panel();
                    divColValue.CssClass = "col_full";
                    divColValue.EnableViewState = true;




                    //Verifico i diritti del ruolo sul campo
                    impostaDirittiRuoloSulCampo(etichettaMenuATendina, menuATendina, oggettoCustom, template, dirittiCampiRuolo);

                    if (etichettaMenuATendina.Visible)
                    {
                        divColDesc.Controls.Add(parDesc);
                        divRowDesc.Controls.Add(divColDesc);
                        panel.Controls.Add(divRowDesc);
                    }


                    if (menuATendina.Visible)
                    {
                        divColValue.Controls.Add(menuATendina);
                        divRowValue.Controls.Add(divColValue);
                        panel.Controls.Add(divRowValue);
                    }


                }
                return panel;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion inserisciMenuATendina

        #region inserisciSelezioneEsclusiva
        private static Panel inserisciSelezioneEsclusiva(OggettoCustom oggettoCustom, Panel panel, Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try
            {
                DocsPaWR.StoricoProfilatiOldValue selezEsclOldObj = new StoricoProfilatiOldValue();
                if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                {

                    Label etichettaSelezioneEsclusiva = new Label();
                    etichettaSelezioneEsclusiva.EnableViewState = true;
                    CustomImageButton cancella_selezioneEsclusiva = new CustomImageButton();
                    string language = UIManager.UserManager.GetUserLanguage();
                    cancella_selezioneEsclusiva.AlternateText = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
                    cancella_selezioneEsclusiva.ToolTip = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
                    cancella_selezioneEsclusiva.EnableViewState = true;

                    if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    {
                        etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE + " *";
                    }
                    else
                    {
                        etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;
                    }

                    cancella_selezioneEsclusiva.ID = "_" + oggettoCustom.SYSTEM_ID.ToString();
                    cancella_selezioneEsclusiva.ImageUrl = "../Images/Icons/delete.png";
                    cancella_selezioneEsclusiva.OnMouseOutImage = "../Images/Icons/delete.png";
                    cancella_selezioneEsclusiva.OnMouseOverImage = "../Images/Icons/delete_hover.png";
                    cancella_selezioneEsclusiva.CssClass = "clickable";
                    cancella_selezioneEsclusiva.Click += cancella_selezioneEsclusiva_Click;
                    etichettaSelezioneEsclusiva.CssClass = "weight";

                    RadioButtonList selezioneEsclusiva = new RadioButtonList();
                    selezioneEsclusiva.EnableViewState = true;
                    selezioneEsclusiva.ID = oggettoCustom.SYSTEM_ID.ToString();
                    int valoreDiDefault = -1;
                    for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
                    {
                        DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
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
                    if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                    {
                        selezioneEsclusiva.SelectedIndex = impostaSelezioneEsclusiva(oggettoCustom.VALORE_DATABASE, selezioneEsclusiva);
                    }

                    Panel divRowDesc = new Panel();
                    divRowDesc.CssClass = "row";
                    divRowDesc.EnableViewState = true;

                    Panel divColDesc = new Panel();
                    divColDesc.CssClass = "col";
                    divColDesc.EnableViewState = true;

                    Panel divColImage = new Panel();
                    divColImage.CssClass = "col-right-no-margin";
                    divColImage.EnableViewState = true;

                    divColImage.Controls.Add(cancella_selezioneEsclusiva);

                    HtmlGenericControl parDesc = new HtmlGenericControl("p");
                    parDesc.Controls.Add(etichettaSelezioneEsclusiva);
                    parDesc.EnableViewState = true;

                    divColDesc.Controls.Add(parDesc);

                    divRowDesc.Controls.Add(divColDesc);
                    divRowDesc.Controls.Add(divColImage);


                    Panel divRowValue = new Panel();
                    divRowValue.CssClass = "row";
                    divRowValue.EnableViewState = true;

                    Panel divColValue = new Panel();
                    divColValue.CssClass = "col_full";
                    divColValue.EnableViewState = true;

                    divColValue.Controls.Add(selezioneEsclusiva);
                    divRowValue.Controls.Add(divColValue);



                    //Verifico i diritti del ruolo sul campo
                    impostaDirittiRuoloSelezioneEsclusiva(etichettaSelezioneEsclusiva, selezioneEsclusiva, cancella_selezioneEsclusiva, oggettoCustom, template, dirittiCampiRuolo);

                    if (etichettaSelezioneEsclusiva.Visible)
                    {
                        panel.Controls.Add(divRowDesc);
                    }

                    if (selezioneEsclusiva.Visible)
                    {
                        panel.Controls.Add(divRowValue);
                    }
                }
                return panel;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion inserisciSelezioneEsclusiva

        #region impostaSelezioneMenuATendina
        private static int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
        {
            try
            {
                for (int i = 0; i < ddl.Items.Count; i++)
                {
                    if (ddl.Items[i].Text == valore)
                        return i;
                }
                return 0;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
        }
        #endregion

        #region inserisciContatore
        private static Panel inserisciContatore(OggettoCustom oggettoCustom, Panel panel, Templates template, List<AssDocFascRuoli> dirittiCampiRuolo, Ruolo ruolo, InfoUtente infoutente)
        {
            try
            {
                bool paneldll = false;
                if (!string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                {

                    Label etichettaContatore = new Label();
                    etichettaContatore.Text = oggettoCustom.DESCRIZIONE;
                    etichettaContatore.CssClass = "weight";
                    etichettaContatore.EnableViewState = true;

                    Panel divRowDesc = new Panel();
                    divRowDesc.CssClass = "row";
                    divRowDesc.EnableViewState = true;

                    Panel divColDesc = new Panel();
                    divColDesc.CssClass = "col";
                    divColDesc.EnableViewState = true;

                    HtmlGenericControl parDesc = new HtmlGenericControl("p");
                    parDesc.Controls.Add(etichettaContatore);
                    parDesc.EnableViewState = true;

                    divColDesc.Controls.Add(parDesc);
                    divRowDesc.Controls.Add(divColDesc);



                    //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
                    //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
                    //del contatore come da formato prescelto e in readOnly
                    Label etichettaDDL = new Label();
                    etichettaDDL.EnableViewState = true;
                    etichettaDDL.Width = 50;
                    DropDownList ddl = new DropDownList();
                    ddl.EnableViewState = true;

                    string language = UIManager.UserManager.GetUserLanguage();
                    ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

                    Panel divRowDll = new Panel();
                    divRowDll.CssClass = "row";
                    divRowDll.EnableViewState = true;

                    Panel divRowEtiDll = new Panel();
                    divRowEtiDll.CssClass = "row";
                    divRowEtiDll.EnableViewState = true;

                    HtmlGenericControl parDll = new HtmlGenericControl("p");
                    parDll.EnableViewState = true;

                    if (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                    {


                        Registro[] registriRfVisibili = UIManager.RegistryManager.GetListRegistriesAndRF(ruolo.systemId, string.Empty, string.Empty);

                        Panel divColDllEti = new Panel();
                        divColDllEti.CssClass = "col";
                        divColDllEti.EnableViewState = true;

                        Panel divColDll = new Panel();
                        divColDll.CssClass = "col";
                        divColDll.EnableViewState = true;

                        switch (oggettoCustom.TIPO_CONTATORE)
                        {
                            case "T":
                                break;
                            case "A":
                                paneldll = true;
                                etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                                etichettaDDL.CssClass = "weight";
                                ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                                ddl.CssClass = "chzn-select-deselect";
                                ddl.Width = 240;

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
                                    it.Text = string.Empty;
                                    it.Value = string.Empty;
                                    ddl.Items.Add(it);
                                    ddl.SelectedValue = string.Empty;
                                }
                                else
                                {
                                    ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                                }


                                parDll.Controls.Add(etichettaDDL);
                                divColDllEti.Controls.Add(parDll);
                                divRowEtiDll.Controls.Add(divColDllEti);

                                divColDll.Controls.Add(ddl);
                                divRowDll.Controls.Add(divColDll);

                                break;
                            case "R":
                                paneldll = true;
                                etichettaDDL.Text = "&nbsp;RF&nbsp;";
                                etichettaDDL.CssClass = "weight";
                                ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                                ddl.CssClass = "chzn-select-deselect";
                                ddl.Width = 240;

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
                                    it.Value = string.Empty;
                                    it.Text = string.Empty;
                                    ddl.Items.Add(it);
                                    ddl.SelectedValue = string.Empty;
                                }
                                else
                                    ddl.SelectedValue = oggettoCustom.ID_AOO_RF;


                                ddl.CssClass = "chzn-select-deselect";

                                parDll.Controls.Add(etichettaDDL);
                                divColDllEti.Controls.Add(parDll);
                                divRowEtiDll.Controls.Add(divColDllEti);

                                divColDll.Controls.Add(ddl);
                                divRowDll.Controls.Add(divColDll);
                                break;

                        }
                    }

                    //Imposto il contatore in funzione del formato
                    CustomTextArea contatore = new CustomTextArea();
                    contatore.EnableViewState = true;
                    contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
                    contatore.CssClass = "txt_textdata_counter";
                    contatore.CssClassReadOnly = "txt_textdata_counter_disabled";
                    if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
                    {
                        contatore.Text = oggettoCustom.FORMATO_CONTATORE;
                        if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                        {
                            contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO);
                            contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);
                            string codiceAmministrazione = UIManager.AdministrationManager.AmmGetInfoAmmCorrente(infoutente.idAmministrazione).Codice;
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
                                Registro reg = RegistryManager.getRegistroBySistemId(oggettoCustom.ID_AOO_RF);
                                if (reg != null)
                                {
                                    contatore.Text = contatore.Text.Replace("RF", reg.codRegistro);
                                    contatore.Text = contatore.Text.Replace("AOO", reg.codRegistro);
                                }
                            }
                        }
                        else
                        {
                            contatore.Text = string.Empty;
                        }
                    }
                    else
                    {
                        contatore.Text = oggettoCustom.VALORE_DATABASE;
                    }

                    Panel divRowCounter = new Panel();
                    divRowCounter.CssClass = "row";
                    divRowCounter.EnableViewState = true;

                    Panel divColCountCounter = new Panel();
                    divColCountCounter.CssClass = "col";
                    divColCountCounter.EnableViewState = true;


                    CheckBox cbContaDopo = new CheckBox();
                    cbContaDopo.EnableViewState = true;
                    //Pulsante annulla
                    CustomButton btn_annulla = new CustomButton();
                    btn_annulla.EnableViewState = true;
                    btn_annulla.ID = "btn_a_" + oggettoCustom.SYSTEM_ID.ToString();
                    btn_annulla.Text = Utils.Languages.GetLabelFromCode("BtnAbortProflier", language);
                    btn_annulla.Visible = false;
                    btn_annulla.CssClass = "buttonAbort";
                    btn_annulla.OnMouseOver = "buttonAbortHover";
                    btn_annulla.CssClassDisabled = "buttonAbortDisable";

                    Panel divColCountAbort = new Panel();
                    divColCountAbort.CssClass = "col-right-no-margin-no-top";
                    divColCountAbort.EnableViewState = true;

                    //Verifico i diritti del ruolo sul campo
                    impostaDirittiRuoloContatore(etichettaContatore, contatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, template, dirittiCampiRuolo);


                    if (etichettaContatore.Visible)
                    {
                        panel.Controls.Add(divRowDesc);
                    }



                    divColCountCounter.Controls.Add(contatore);
                    divRowCounter.Controls.Add(divColCountCounter);

                    divColCountAbort.Controls.Add(btn_annulla);
                    divRowCounter.Controls.Add(divColCountAbort);



                    if (paneldll)
                    {
                        panel.Controls.Add(divRowEtiDll);
                        panel.Controls.Add(divRowDll);
                    }

                    if (contatore.Visible)
                    {
                        panel.Controls.Add(divRowCounter);
                    }

                }
                return panel;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion inserisciContatore

        #region inserisciData
        private static Panel inserisciData(OggettoCustom oggettoCustom, Panel panel, Templates template, List<AssDocFascRuoli> dirittiCampiRuolo, Page page)
        {
            try
            {
                //Per il momento questo tipo di campo è stato implementato con tre semplici textBox
                //Sarebbe opportuno creare un oggetto personalizzato, che espone le stesse funzionalità
                //della textBox, ma che mi permette di gestire la data con i tre campi separati.

                if (!string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                {

                    Label etichettaData = new Label();
                    etichettaData.EnableViewState = true;

                    if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    {
                        etichettaData.Text = oggettoCustom.DESCRIZIONE + " *";
                    }
                    else
                    {
                        etichettaData.Text = oggettoCustom.DESCRIZIONE;
                    }
                    etichettaData.CssClass = "weight";

                    NttDataWA.UserControls.Calendar data = (NttDataWA.UserControls.Calendar)page.LoadControl("../UserControls/Calendar.ascx");
                    data.EnableViewState = true;
                    data.ID = oggettoCustom.SYSTEM_ID.ToString();
                    data.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);
                    data.SetEnableTimeMode();

                    if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                    {
                        data.Text = oggettoCustom.VALORE_DATABASE;
                    }

                    //Verifico i diritti del ruolo sul campo
                    impostaDirittiRuoloSulCampo(etichettaData, data, oggettoCustom, template, dirittiCampiRuolo);



                    Panel divRowDesc = new Panel();
                    divRowDesc.CssClass = "row";
                    divRowDesc.EnableViewState = true;

                    Panel divColDesc = new Panel();
                    divColDesc.CssClass = "col";
                    divColDesc.EnableViewState = true;

                    HtmlGenericControl parDesc = new HtmlGenericControl("p");
                    parDesc.Controls.Add(etichettaData);
                    parDesc.EnableViewState = true;

                    divColDesc.Controls.Add(parDesc);
                    divRowDesc.Controls.Add(divColDesc);

                    if (etichettaData.Visible)
                    {
                        panel.Controls.Add(divRowDesc);
                    }

                    Panel divRowValue = new Panel();
                    divRowValue.CssClass = "row";
                    divRowValue.EnableViewState = true;

                    Panel divColValue = new Panel();
                    divColValue.CssClass = "col";
                    divColValue.EnableViewState = true;

                    divColValue.Controls.Add(data);
                    divRowValue.Controls.Add(divColValue);

                    if (data.Visible)
                    {
                        panel.Controls.Add(divRowValue);
                    }

                    ScriptManager.RegisterStartupScript(page, page.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
                }
                return panel;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion inserisciData

        #region inserisciCorrispondente
        private static Panel inserisciCorrispondente(OggettoCustom oggettoCustom, Panel panel, Templates template, List<AssDocFascRuoli> dirittiCampiRuolo, Page page)
        {
            try
            {
                if (!string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                {

                    StoricoProfilatiOldValue corrOldOb = new StoricoProfilatiOldValue();

                    UserControls.CorrespondentCustom corrispondente = page.LoadControl("../UserControls/CorrespondentCustom.ascx") as UserControls.CorrespondentCustom;
                    corrispondente.EnableViewState = true;

                    if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    {
                        corrispondente.TxtEtiCustomCorrespondent = oggettoCustom.DESCRIZIONE + " *";
                    }
                    else
                    {
                        corrispondente.TxtEtiCustomCorrespondent = oggettoCustom.DESCRIZIONE;
                    }


                    corrispondente.TypeCorrespondentCustom = oggettoCustom.TIPO_RICERCA_CORR;
                    corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();
                    
                    //Da amministrazione è stato impostato un ruolo di default per questo campo.
                    if (!string.IsNullOrEmpty(oggettoCustom.ID_RUOLO_DEFAULT) && oggettoCustom.ID_RUOLO_DEFAULT != "0")
                    {
                        DocsPaWR.Ruolo ruolo = RoleManager.getRuoloById(oggettoCustom.ID_RUOLO_DEFAULT);
                        if (ruolo != null)
                        {
                            corrispondente.IdCorrespondentCustom = ruolo.systemId;
                            corrispondente.TxtCodeCorrespondentCustom = ruolo.codiceRubrica;
                            corrispondente.TxtDescriptionCorrespondentCustom = ruolo.descrizione;
                        }
                        oggettoCustom.ID_RUOLO_DEFAULT = "0";
                    }

                    //Il campo è valorizzato.
                    if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                    {
                        DocsPaWR.Corrispondente corr_1 = AddressBookManager.getCorrispondenteBySystemIDDisabled(oggettoCustom.VALORE_DATABASE);
                        if (corr_1 != null)
                        {
                            corrispondente.IdCorrespondentCustom = corr_1.systemId;
                            corrispondente.TxtCodeCorrespondentCustom = corr_1.codiceRubrica.ToString();
                            corrispondente.TxtDescriptionCorrespondentCustom = corr_1.descrizione.ToString();
                            oggettoCustom.VALORE_DATABASE = corr_1.systemId;
                        }
                    }
                  
                    //Verifico i diritti del ruolo sul campo
                    impostaDirittiRuoloSulCampo(corrispondente.TxtEtiCustomCorrespondent, corrispondente, oggettoCustom, template, dirittiCampiRuolo);
                    if (corrispondente.Visible)
                    {
                        panel.Controls.Add(corrispondente);
                    }
                   
                }
                return panel;
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion inserisciCorrispondente

        #region inserisciContatoreSottocontatore
        private static Panel inserisciContatoreSottocontatore(DocsPaWR.OggettoCustom oggettoCustom, Panel panel, Templates template, List<AssDocFascRuoli> dirittiCampiRuolo, Ruolo ruolo, Page page)
        {
            try
            {
                bool paneldll = false;

                if (!string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                {

                    Label etichettaContatoreSottocontatore = new Label();
                    etichettaContatoreSottocontatore.EnableViewState = true;
                    etichettaContatoreSottocontatore.Text = oggettoCustom.DESCRIZIONE;
                    etichettaContatoreSottocontatore.CssClass = "weight";

                    Panel divRowDesc = new Panel();
                    divRowDesc.CssClass = "row";
                    divRowDesc.EnableViewState = true;

                    Panel divColDesc = new Panel();
                    divColDesc.CssClass = "col";
                    divColDesc.EnableViewState = true;

                    HtmlGenericControl parDesc = new HtmlGenericControl("p");
                    parDesc.Controls.Add(etichettaContatoreSottocontatore);
                    parDesc.EnableViewState = true;

                    divColDesc.Controls.Add(parDesc);
                    divRowDesc.Controls.Add(divColDesc);



                    //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
                    //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
                    //del contatore come da formato prescelto e in readOnly
                    Label etichettaDDL = new Label();
                    etichettaDDL.EnableViewState = true;
                    DropDownList ddl = new DropDownList();
                    ddl.EnableViewState = true;

                    string language = UIManager.UserManager.GetUserLanguage();
                    ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

                    Panel divRowDll = new Panel();
                    divRowDll.CssClass = "row";
                    divRowDll.EnableViewState = true;

                    Panel divRowEtiDll = new Panel();
                    divRowEtiDll.CssClass = "row";
                    divRowEtiDll.EnableViewState = true;

                    HtmlGenericControl parDll = new HtmlGenericControl("p");
                    parDll.EnableViewState = true;

                    if (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                    {

                        Registro[] registriRfVisibili = UIManager.RegistryManager.GetListRegistriesAndRF(ruolo.systemId, string.Empty, string.Empty);

                        Panel divColDllEti = new Panel();
                        divColDllEti.CssClass = "col";
                        divColDllEti.EnableViewState = true;

                        Panel divColDll = new Panel();
                        divColDll.CssClass = "col";
                        divColDll.EnableViewState = true;

                        switch (oggettoCustom.TIPO_CONTATORE)
                        {
                            case "T":
                                break;
                            case "A":
                                paneldll = true;
                                etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                                etichettaDDL.CssClass = "weight";
                                etichettaDDL.Width = 50;
                                ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                                ddl.CssClass = "chzn-select-deselect";
                                ddl.Width = 240;

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
                                    it.Text = string.Empty;
                                    it.Value = string.Empty;
                                    ddl.Items.Add(it);
                                    ddl.SelectedValue = string.Empty;
                                }
                                else
                                    ddl.SelectedValue = oggettoCustom.ID_AOO_RF;


                                parDll.Controls.Add(etichettaDDL);
                                divColDllEti.Controls.Add(parDll);
                                divRowEtiDll.Controls.Add(divColDllEti);

                                divColDll.Controls.Add(ddl);
                                divRowDll.Controls.Add(divColDll);
                                break;
                            case "R":
                                paneldll = true;
                                etichettaDDL.Text = "&nbsp;RF&nbsp;";
                                etichettaDDL.CssClass = "weight";
                                etichettaDDL.Width = 50;
                                ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                                ddl.CssClass = "chzn-select-deselect";
                                ddl.Width = 240;

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
                                    it.Value = string.Empty;
                                    it.Text = string.Empty;
                                    ddl.Items.Add(it);
                                    ddl.SelectedValue = string.Empty;
                                }
                                else
                                {
                                    ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                                }


                                parDll.Controls.Add(etichettaDDL);
                                divColDllEti.Controls.Add(parDll);
                                divRowEtiDll.Controls.Add(divColDllEti);

                                divColDll.Controls.Add(ddl);
                                divRowDll.Controls.Add(divColDll);
                                break;
                        }
                    }

                    //Imposto il contatore in funzione del formato
                    CustomTextArea contatore = new CustomTextArea();
                    CustomTextArea sottocontatore = new CustomTextArea();
                    CustomTextArea dataInserimentoSottocontatore = new CustomTextArea();
                    contatore.EnableViewState = true;
                    sottocontatore.EnableViewState = true;
                    dataInserimentoSottocontatore.EnableViewState = true;
                    contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
                    sottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_sottocontatore";
                    dataInserimentoSottocontatore.ID = oggettoCustom.SYSTEM_ID.ToString() + "_dataSottocontatore";
                    if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
                    {
                        contatore.Text = oggettoCustom.FORMATO_CONTATORE;
                        sottocontatore.Text = oggettoCustom.FORMATO_CONTATORE;

                        if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) && !string.IsNullOrEmpty(oggettoCustom.VALORE_SOTTOCONTATORE))
                        {
                            contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO);
                            contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);

                            sottocontatore.Text = sottocontatore.Text.Replace("ANNO", oggettoCustom.ANNO);
                            sottocontatore.Text = sottocontatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_SOTTOCONTATORE);

                            if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
                            {
                                Registro reg = UserManager.getRegistroBySistemId(page, oggettoCustom.ID_AOO_RF);
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
                    else
                    {
                        contatore.Text = oggettoCustom.VALORE_DATABASE;
                        sottocontatore.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
                    }

                    Panel divRowCounter = new Panel();
                    divRowCounter.CssClass = "row";
                    divRowCounter.EnableViewState = true;

                    Panel divColCountCounter = new Panel();
                    divColCountCounter.CssClass = "col_full";
                    divColCountCounter.EnableViewState = true;
                    divColCountCounter.Controls.Add(contatore);
                    divColCountCounter.Controls.Add(sottocontatore);
                    divRowCounter.Controls.Add(divColCountCounter);

                    if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
                    {
                        dataInserimentoSottocontatore.Text = oggettoCustom.DATA_INSERIMENTO;

                        Panel divColCountAbort = new Panel();
                        divColCountAbort.CssClass = "col";
                        divColCountAbort.EnableViewState = true;
                        divColCountAbort.Controls.Add(dataInserimentoSottocontatore);
                        divRowCounter.Controls.Add(divColCountAbort);
                    }

                    CheckBox cbContaDopo = new CheckBox();
                    cbContaDopo.EnableViewState = true;

                    //Verifico i diritti del ruolo sul campo
                    impostaDirittiRuoloContatoreSottocontatore(etichettaContatoreSottocontatore, contatore, sottocontatore, dataInserimentoSottocontatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, template, dirittiCampiRuolo);

                    if (etichettaContatoreSottocontatore.Visible)
                    {
                        panel.Controls.Add(divRowDesc);
                    }

                    contatore.ReadOnly = true;
                    contatore.CssClass = "txt_input_half";
                    contatore.CssClassReadOnly = "txt_input_half_disabled";

                    sottocontatore.ReadOnly = true;
                    sottocontatore.CssClass = "txt_input_half";
                    sottocontatore.CssClassReadOnly = "txt_input_half_disabled";

                    dataInserimentoSottocontatore.ReadOnly = true;
                    dataInserimentoSottocontatore.CssClass = "txt_input_full";
                    dataInserimentoSottocontatore.CssClassReadOnly = "txt_input_full_disabled";
                    dataInserimentoSottocontatore.Visible = false;


                    //Inserisco il cb per il conta dopo
                    if (oggettoCustom.CONTA_DOPO == "1")
                    {
                        cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
                        cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                        cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";




                        Panel divColCountAfter = new Panel();
                        divColCountAfter.CssClass = "col";
                        divColCountAfter.EnableViewState = true;
                        divColCountAfter.Controls.Add(cbContaDopo);
                        divRowDll.Controls.Add(divColCountAfter);
                    }

                    if (paneldll)
                    {
                        panel.Controls.Add(divRowEtiDll);
                        panel.Controls.Add(divRowDll);
                    }

                    if (contatore.Visible || sottocontatore.Visible)
                    {
                        panel.Controls.Add(divRowCounter);
                    }
                }
                return panel;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion

        #region inserisciOggettoEsterno
        private static Panel inserisciOggettoEsterno(DocsPaWR.OggettoCustom oggettoCustom, Panel panel, Templates template, List<AssDocFascRuoli> dirittiCampiRuolo, Page page)
        {
            try
            {
                if (!string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                {

                    Label etichetta = new Label();
                    etichetta.EnableViewState = true;

                    if ((oggettoCustom.CAMPO_OBBLIGATORIO).Equals("SI"))
                    {
                        etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
                    }
                    else
                    {
                        etichetta.Text = oggettoCustom.DESCRIZIONE;
                    }
                    etichetta.CssClass = "weight";
                    UserControls.IntegrationAdapter intAd = (UserControls.IntegrationAdapter)page.LoadControl("../UserControls/IntegrationAdapter.ascx");
                    intAd.ID = oggettoCustom.SYSTEM_ID.ToString();
                    intAd.View = UserControls.IntegrationAdapterView.INSERT_MODIFY;
                    intAd.ManualInsertCssClass = "txt_textdata_counter_disabled_red";
                    intAd.EnableViewState = true;
                    //Verifico i diritti del ruolo sul campo
                    impostaDirittiRuoloSulCampo(etichetta, intAd, oggettoCustom, template, dirittiCampiRuolo);
                    intAd.ConfigurationValue = oggettoCustom.CONFIG_OBJ_EST;
                    IntegrationAdapterValue value = new IntegrationAdapterValue(oggettoCustom.CODICE_DB, oggettoCustom.VALORE_DATABASE, oggettoCustom.MANUAL_INSERT);
                    intAd.Value = value;

                    Panel divRowDesc = new Panel();
                    divRowDesc.CssClass = "row";
                    divRowDesc.EnableViewState = true;

                    Panel divColDesc = new Panel();
                    divColDesc.CssClass = "col";
                    divColDesc.EnableViewState = true;

                    Panel divRowValue = new Panel();
                    divRowValue.CssClass = "row";
                    divRowValue.EnableViewState = true;

                    Panel divColValue = new Panel();
                    divColValue.CssClass = "col_full";
                    divColValue.EnableViewState = true;

                    if (etichetta.Visible)
                    {
                        HtmlGenericControl parDesc = new HtmlGenericControl("p");
                        parDesc.Controls.Add(etichetta);
                        parDesc.EnableViewState = true;
                        divColDesc.Controls.Add(parDesc);
                        divRowDesc.Controls.Add(divColDesc);
                        panel.Controls.Add(divRowDesc);
                    }

                    if (intAd.Visible)
                    {
                        divColValue.Controls.Add(intAd);
                        divRowValue.Controls.Add(divColValue);
                        panel.Controls.Add(divRowValue);
                    }
                }
                return panel;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion inserisciOggettoEsterno

        #region cancella_selezioneEsclusiva_Click
        protected static void cancella_selezioneEsclusiva_Click(object sender, EventArgs e)
        {
            try
            {
                string idOggetto = (((CustomImageButton)sender).ID).Substring(1);
                //((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).SelectedIndex = -1;
                //((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).EnableViewState = true;
                //for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
                //{
                //    if (((DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).SYSTEM_ID.ToString().Equals(idOggetto))
                //    {
                //        ((DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).VALORE_DATABASE = "-1";
                //    }
                //}
                //this.UpPnlTypeDocument.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        #endregion

        #region impostaSelezioneEsclusiva
        private static int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
        {
            try
            {
                for (int i = 0; i < rbl.Items.Count; i++)
                {
                    if (rbl.Items[i].Text == valore)
                        return i;
                }
                return 0;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
        }
        #endregion

        #region imposta diritti ruolo sul campo
        public static void impostaDirittiRuoloSulCampo(object etichetta, object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try
            {
                //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

                foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
                {
                    if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                    {
                        switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "CampoDiTesto":
                                //((System.Web.UI.WebControls.Label)etichetta).Visible = true;
                                ((CustomTextArea)campo).Visible = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";

                                break;
                            case "CasellaDiSelezione":
                                //((System.Web.UI.WebControls.Label)etichetta).Visible = true;
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Visible = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";

                                break;
                            case "MenuATendina":
                                //((System.Web.UI.WebControls.Label)etichetta).Visible = true;
                                ((System.Web.UI.WebControls.DropDownList)campo).Visible = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";

                                break;
                            case "SelezioneEsclusiva":
                                //Per la selezione esclusiva è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                                break;
                            case "Contatore":
                                //Per il contatore è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                                break;
                            case "Data":

                                //((System.Web.UI.WebControls.Label)etichetta).Visible = true;
                                ((UserControls.Calendar)campo).Visible = true;
                                ((UserControls.Calendar)campo).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";

                                break;
                            case "Corrispondente":

                                //((System.Web.UI.WebControls.Label)etichetta).Visible = true;
                                ((UserControls.CorrespondentCustom)campo).Visible = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";

                                break;
                            case "Link":
                                if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                                {
                                    ((UserControls.LinkDocFasc)campo).IsInsertModify = false;
                                    oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                                }
                                else
                                {
                                    ((UserControls.LinkDocFasc)campo).IsInsertModify = true;
                                }
                                if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                                {
                                    ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                    ((UserControls.LinkDocFasc)campo).Visible = false;
                                    oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                                }
                                break;
                            case "OggettoEsterno":
                                if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                                {
                                    ((UserControls.IntegrationAdapter)campo).View = UserControls.IntegrationAdapterView.READ_ONLY;
                                    oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                                }
                                else
                                {
                                    ((UserControls.IntegrationAdapter)campo).View = UserControls.IntegrationAdapterView.INSERT_MODIFY;
                                }

                                //((System.Web.UI.WebControls.Label)etichetta).Visible = true;
                                ((UserControls.IntegrationAdapter)campo).Visible = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";

                                break;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void impostaDirittiRuoloSelezioneEsclusiva(object etichetta, object campo, object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try
            {
                foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
                {
                    if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                    {
                        if ((assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1") || template.IN_ESERCIZIO.ToUpper().Equals("NO"))
                        {
                            ((System.Web.UI.WebControls.RadioButtonList)campo).Enabled = false;
                            ((CustomImageButton)button).Visible = false;
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private static void impostaDirittiRuoloContatore(object etichettaContatore, object campo, object checkBox, object etichettaDDL, object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try
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
                            ((System.Web.UI.WebControls.Label)etichettaContatore).Visible = false;
                            ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                            ((CustomTextArea)campo).Visible = false;
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void impostaDirittiRuoloData(object etichettaData, object etichettaDataDa, object etichettaDataA, object dataDa, object dataA, OggettoCustom oggettoCustom, Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try
            {
                //AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

                foreach (AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
                {
                    if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                    {
                        if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                        {
                            ((System.Web.UI.WebControls.Label)etichettaData).Visible = false;
                            ((System.Web.UI.WebControls.Label)etichettaDataDa).Visible = false;
                            ((System.Web.UI.WebControls.Label)etichettaDataA).Visible = false;

                            ((UserControls.Calendar)dataDa).Visible = false;
                            ((UserControls.Calendar)dataDa).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                            ((UserControls.Calendar)dataA).Visible = false;
                            ((UserControls.Calendar)dataA).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void impostaDirittiRuoloContatoreSottocontatore(object etichettaContatoreSottocontatore, object contatore, object sottocontatore, object dataInserimentoSottocontatore, object checkBox, object etichettaDDL, object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            try
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
                            ((CustomTextArea)contatore).Visible = false;
                            ((CustomTextArea)sottocontatore).Visible = false;
                            ((CustomTextArea)dataInserimentoSottocontatore).Visible = false;
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        #endregion
    }
}