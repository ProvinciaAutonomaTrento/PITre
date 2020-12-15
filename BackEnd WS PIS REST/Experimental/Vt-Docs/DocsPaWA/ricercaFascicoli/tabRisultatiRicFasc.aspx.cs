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
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.ricercaFascicoli
{
    /// <summary>
    /// Summary description for tabRisultatiRicFasc.
    /// </summary>
    public class tabRisultatiRicFasc : DocsPAWA.CssPage
    {
        protected System.Web.UI.WebControls.DataGrid DataGrid1;
        //protected System.Web.UI.HtmlControls.HtmlTableCell TD2;
        //protected System.Web.UI.WebControls.Label Label1;
        private int idClass;
        private bool allClass;
        private DocsPAWA.DocsPaWR.FiltroRicerca[][] ListaFiltri;
        private DocsPAWA.DocsPaWR.Fascicolo[] ListaFasc;
        protected DocsPAWA.dataSet.DataSetRFasc dataSetRFasc1;
        protected DataView dv;
        protected int currentPage;
        protected int numTotPage;
        protected int nRec;
        private bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
            && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
        protected DocsPAWA.DocsPaWR.Utente utente;
        protected DocsPAWA.DocsPaWR.Ruolo ruolo;
        // Spostato in bottoniera operazioni massive
        //protected System.Web.UI.WebControls.ImageButton btn_stampa;
        protected System.Web.UI.WebControls.Label titolo;
        protected System.Web.UI.WebControls.Label msgADL;
        protected System.Web.UI.WebControls.Label lbl_messaggio;
        protected System.Web.UI.HtmlControls.HtmlTableRow tr1;
        protected System.Web.UI.HtmlControls.HtmlTableCell Td2;
        protected System.Web.UI.HtmlControls.HtmlTableRow trHeader;
        protected System.Web.UI.HtmlControls.HtmlTableRow trBody;
        protected DocsPAWA.DocsPaWR.Trasmissione trasmissione;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd1;
        protected System.Web.UI.WebControls.ImageButton btn_archivia;
        protected System.Web.UI.WebControls.ImageButton img_Archivio;
        protected bool isRicercaSottofascicoli;
        protected string sottofascicolo;

        #region Bottniera

        protected DocsPAWA.UserControls.MassiveOperationButtons moButtons;
        protected System.Web.UI.WebControls.CheckBox chkSelected;
        protected System.Web.UI.WebControls.HiddenField hfIdProject;

        #endregion

        private void Page_Load(object sender, System.EventArgs e)
        {
            // Se non si è in postback viene inizializzata la bottoniera
            if (!IsPostBack)
                this.moButtons.InitializeOrUpdateUserControl(new SearchResultInfo[] { });

            Utils.startUp(this);
            getParameter();

            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_ID_LEG) != null &&
                ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_ID_LEG).Equals("1"))
            {
                //visibilità della legislatura
                DataGrid1.Columns[10].Visible = true;
            }

            if (!Page.IsPostBack)
            {
                this.DataGrid1.PageSize = FascicoliManager.getGrdFascicoliPageSize(this);
                btn_archivia.Visible = false;

                this.AttatchGridPagingWaitControl();

                currentPage = this.GetCurrentPageOnContext();
                nRec = 0;

                aggiornaRisultatiRicerca(currentPage, out nRec, out numTotPage);

                if (!UserManager.ruoloIsAutorized(this.Page, "DO_CONS"))
                    this.DataGrid1.Columns[13].Visible = false;
            }

//in caso di ultima ricerca precedente è stato  ricerca ADL FASC, devo rimuove i 
                //Filtri altrimenti Microsoft vengono fuori direttamente gli stessi fasc come risultato della ric fasc normale
                //if (string.IsNullOrEmpty(Request.QueryString["ricADL"]) ||string.IsNullOrEmpty(Request.QueryString["RICERCA"]))// quindi sto facendo una ricerca normale non in AdL
                //{
                //    FascicoliManager.removeMemoriaRicFasc(this);
                //}

            // nuova ADL
            if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
            {
                DataGrid1.Columns[12].Visible = true;
            }

            //Verifico che è stato selezionato il calcolo di un contatore,
            //in caso affermativo, riapro la popup di profilazione, per far verificare il numero generato
            if (Session["contaDopoChecked"] != null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "apriPopUpProfilazioneFasc", "window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamicaFasc.aspx','','dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;');", true);
                Session.Remove("contaDopoChecked");
            }

            DataGrid1.Columns[2].Visible = this.isVisibleColContatore();
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            // Ripristino del fascicolo selezionato precedentemente
            this.SetFascIndexFromQueryString(this.DataGrid1);

            if (!this.IsPostBack)
            {
                // Aggiornamento numero fascicoli trovati
                this.RefreshCountFascicoli();
                // Impostazione visibilità controlli
                this.SetControlsVisibility();
            }
        }

        /// <summary>
        /// Impostazione visibilità controlli
        /// </summary>
        private void SetControlsVisibility()
        {
            this.trHeader.Visible = true;
            this.lbl_messaggio.Visible = false;
            this.trBody.Visible = (this.DataGrid1.Items.Count > 0);

            // Spostato in bottoniera operazioni massive
            //this.btn_stampa.Visible = this.trBody.Visible;

            //Lista di fascicoli in deposito, non si può:
            //- inserire in area lavoro
            //- inserire in area conservazione
            //Trasferimento deposito
            //AL MOMENTO NON SERVE
            //if (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA"))
            //{
            //    if (ListaFiltri != null)
            //    {
            //        foreach (DocsPAWA.DocsPaWR.FiltroRicerca[] filterArray in this.ListaFiltri)
            //            foreach (DocsPAWA.DocsPaWR.FiltroRicerca filterItem in filterArray)
            //            {
            //                if (filterItem.argomento.Equals("DEPOSITO") && !filterItem.valore.Equals(""))
            //                {
            //                    if (filterItem.valore.Equals("D"))
            //                    {
            //                        this.DataGrid1.Columns[10].Visible = false;
            //                        this.DataGrid1.Columns[11].Visible = false;
            //                        this.DataGrid1.Columns[12].Visible = false;
            //                    }

            //                }
            //            }
            //    }
            //}
        }

        private void getParameter()
        {
            if (Request.QueryString["idClass"] != null)
            { idClass = Int32.Parse(Request.QueryString["idClass"]); }
            else
            { idClass = 0; }

            allClass = FascicoliManager.getAllClassValue(this);
        }

        private void bindDataGrid(DataView dv)
        {
            if (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA"))
            {
                int requisitiArchivio = 0;
                DocsPaWR.FiltroRicerca[] filterItems = ListaFiltri[0];

                foreach (DocsPaWR.FiltroRicerca filtro in filterItems)

                //foreach (DocsPAWA.DocsPaWR.FiltroRicerca[] filtro in ListaFiltri)
                {
                    if (filtro.argomento.Equals(DocsPaWR.FiltriFascicolazione.TIPO_FASCICOLO.ToString()) && filtro.valore.Equals("P"))
                    {
                        requisitiArchivio++;
                    }
                    if (filtro.argomento.Equals(DocsPaWR.FiltriFascicolazione.STATO.ToString()) && filtro.valore.Equals("C"))
                    {
                        requisitiArchivio++;
                    }
                }
                if (requisitiArchivio == 2)
                {
                    btn_archivia.ImageUrl = "../images/proto/btn_corrente.gif";
                    btn_archivia.AlternateText = "Inserisci tutti i fascicoli in archivio corrente";
                    this.DataGrid1.Columns[15].Visible = true;
                    this.DataGrid1.Columns[15].HeaderText = "Trasf. corrente";
                    for (int i = 0; i < this.DataGrid1.Items.Count; i++)
                    {
                        ImageButton img = this.DataGrid1.Items[i].Cells[15].FindControl("img_Archivio") as ImageButton;
                        img.ImageUrl = "../images/proto/btn_corrente.gif";
                        img.AlternateText = "Inserisci il fascicolo in archivio corrente";
                    }
                }
                else
                {
                    btn_archivia.Visible = false;
                    this.DataGrid1.Columns[15].Visible = false;
                }
            }

            if (ListaFiltri != null)
            {
                DocsPaWR.FiltroRicerca[] filterItems2 = ListaFiltri[0];
                foreach (DocsPaWR.FiltroRicerca filtro in filterItems2)
                {
                    if (filtro.argomento.Equals(DocsPaWR.FiltriFascicolazione.SOTTOFASCICOLO.ToString()) && !string.IsNullOrEmpty(filtro.valore))
                    {
                        Session.Add("isRicercaSottofascicoli", filtro.valore);
                    }
                }
            }
            DataGrid1.DataSource = dv;
            DataGrid1.DataBind();


        }

        private string getCodiceGerarchia(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
        {
            string retValue = "";
            DocsPaWR.FascicolazioneClassifica[] classifica = FascicoliManager.getGerarchia(this, fascicolo.idClassificazione, UserManager.getUtente(this).idAmministrazione);

            //			for (int i=0;i<classifica.Length;i++)
            //			{
            //				retValue=retValue+classifica[i].codice.ToString();
            //				if (i<classifica.Length-1)
            //				{
            //					retValue=retValue+".";
            //				}
            //			}

            if (classifica != null)
                retValue = classifica[classifica.Length - 1].codice;

            return retValue;
        }

        private void caricaDatagridFasc(DataGrid dg, DocsPAWA.DocsPaWR.Fascicolo[] listaFascicoli)
        {
            DataTable datatable = new DataTable();
            string codiceGerarchia = null;


            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["templateRicerca"];
            DocsPAWA.DocsPaWR.OggettoCustom og1 = null;

            if (template != null)
            {


                foreach (DocsPAWA.DocsPaWR.OggettoCustom og in template.ELENCO_OGGETTI)
                {
                    if (og.DA_VISUALIZZARE_RICERCA.Equals("1"))
                    {
                        og1 = og;
                        break;
                    }
                }
            }
            try
            {
                if (listaFascicoli != null)
                {
                    if (listaFascicoli.Length > 0)
                    {
                        for (int i = 0; i < listaFascicoli.Length; i++)
                        {
                            DocsPaWR.Fascicolo Fasc = (DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[i];
                            if (Fasc.codiceGerarchia != null)
                            {
                                codiceGerarchia = Fasc.codiceGerarchia.ToString();
                            }
                            //string codiceGerarchia=getCodiceGerarchia(Fasc);
                            //this.dataSetRFasc1.element1.Addelement1Row(Fasc.stato.ToString(),Fasc.descrizione.ToString(),Fasc.apertura.ToString(),Fasc.chiusura.ToString(),Fasc.tipo.ToString(),i,codiceGerarchia,Fasc.codice);
                            //							DateTime dataApertura=Utils.formatStringToDate(Fasc.apertura);
                            //							DateTime dataChiusura=Utils.formatStringToDate(Fasc.chiusura);
                            string dataApertura = Fasc.apertura;
                            string dataChiusura = Fasc.chiusura;

                            //this.dataSetRFasc1.element1.Addelement1Row(Fasc.stato, Fasc.descrizione.ToString(), dataApertura, dataChiusura, Fasc.tipo.ToString(), i, codiceGerarchia, Fasc.codice, Fasc.codLegislatura, Fasc.systemID, Fasc.inConservazione);
                            //modifica
                            if (string.IsNullOrEmpty(Fasc.contatore))
                                DataGrid1.Columns[2].Visible = false;
                            if (og1 != null)
                            {
                                this.DataGrid1.Columns[2].HeaderText = og1.DESCRIZIONE;
                                this.dataSetRFasc1.element1.Addelement1Row(Fasc.stato, Fasc.descrizione.ToString(), dataApertura, dataChiusura, Fasc.tipo.ToString(), i, codiceGerarchia, Fasc.codice, Fasc.codLegislatura, Fasc.systemID, this.inserisciContatore(og1, Fasc.contatore), Fasc.inConservazione);
                            }
                            else
                            {
                                this.dataSetRFasc1.element1.Addelement1Row(Fasc.stato, Fasc.descrizione.ToString(), dataApertura, dataChiusura, Fasc.tipo.ToString(), i, codiceGerarchia, Fasc.codice, Fasc.codLegislatura, Fasc.systemID, Fasc.contatore, Fasc.inConservazione);
                            }
                            //fine modifica
                        }

                        datatable = this.dataSetRFasc1.Tables[0];

                        //this.TD2.BgColor="#c08582";
                    }
                    else
                    {
                        dg.Visible = false;
                    }
                }
                else
                {
                    dg.Visible = false;
                }

                FascicoliManager.setDatagridFascicolo(this, datatable);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(this, es);
            }
        }


        private void Datagrid1_PreRender(object sender, System.EventArgs e)
        {

            //			DataGrid dg=((DataGrid) sender);
            //			for(int i=0;i<this.DataGrid1.Items.Count;i++)
            //			{
            //				if(this.DataGrid1.Items[i].ItemIndex>=0)
            //				{	
            //					switch(this.DataGrid1.Items[i].ItemType.ToString().Trim())
            //					{
            //						case "Item":
            //						{
            //							this.DataGrid1.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
            //							this.DataGrid1.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioN'");
            //							break;
            //						}
            //						case "AlternatingItem":
            //					
            //						{
            //							this.DataGrid1.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
            //							this.DataGrid1.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioA'");
            //							break;
            //						}
            //					}
            //				}
            //			}
            for (int i = 0; i < this.DataGrid1.Items.Count; i++)
            {
                if (((Label)DataGrid1.Items[i].Cells[14].Controls[1]).Text == "1")
                {
                    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[13].Controls[1]).ToolTip = "Elimina questo fascicolo da 'Area di conservazione'";
                    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[13].Controls[1]).ImageUrl = "../images/proto/cancella.gif";
                    ((DocsPaWebCtrlLibrary.ImageButton)this.DataGrid1.Items[i].Cells[13].Controls[1]).CommandName = "EliminaAreaCons";


                }

                //if (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA"))
                //{
                //    ImageButton img = this.DataGrid1.Items[i].Cells[13].FindControl("img_Archivio") as ImageButton;
                //    if (img != null)
                //    {
                //        img.ImageUrl = "../images/proto/btn_corrente.gif";
                //        img.AlternateText = "Inserisci il fascicolo in archivio corrente";
                //    }
                //}
                if(this.DataGrid1.Items[i].ItemIndex == this.DataGrid1.SelectedIndex)
                    ((LinkButton)this.DataGrid1.Items[i].Cells[5].Controls[1]).ForeColor = Color.White;
            }
            //if (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA"))
            //{
            //    btn_archivia.ImageUrl = "../images/proto/btn_corrente.gif";
            //    btn_archivia.AlternateText = "Inserisci tutti i fascicoli in archivio corrente";
            //    this.DataGrid1.Columns[13].Visible = true;
            //    this.DataGrid1.Columns[13].HeaderText = "Trasf. corrente";
            //}
            this.DataGrid1.Columns[2].Visible = isVisibleColContatore();
        }


        private void DataGrad1_OnPageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.DataGrid1.SelectedIndex = -1;

            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            /*dv=FascicoliManager.getDatagridFascicolo(this).DefaultView;
            bindDataGrid(dv);*/

            currentPage = e.NewPageIndex + 1;
            //			#region	Gestione tasto back
            //			FascicoliManager.setMemoriaNumPag(this, currentPage.ToString());
            //			#endregion	Gestione tasto back

            Session["tabRicFasc.RigheDG"] = null;

            // Aggiornamento dello stato di checking memorizzato dalla bottoniera
            this.moButtons.UpdateSavedCheckingStatus();

            aggiornaRisultatiRicerca(currentPage, out nRec, out numTotPage);

            this.SetCurrentPageOnContext(currentPage);

            // Impostazione dello stato di flagging delle checkbox nella griglia in base
            // al valore salvato dalla bottoniera
            this.moButtons.UpdateItemCheckingStatus();
        }


        private void loadNewFrameset()
        {
            string newUrl = "../fascicolo/gestioneFasc.aspx?tab=documenti";

            //	Response.Write("<script>window.open('"+newUrl+"','principale');</script>");
            Response.Write("<script language='javascript'>top.principale.document.location='" + newUrl + "';</script>");
        }

        private DocsPAWA.DocsPaWR.Fascicolo[] recursiveListaFascForAllClassif(DocsPAWA.DocsPaWR.FascicolazioneClassificazione classificazione, DocsPaWR.Fascicolo[] allFasc)
        {
            //recupero lista fascicoli per la classificazione corrente
            DocsPaWR.Fascicolo[] listaFasc = FascicoliManager.getListaFascicoli(this, classificazione, ListaFiltri[0], false, null);

            DocsPaWR.Fascicolo[] totalArrayFasc = new DocsPAWA.DocsPaWR.Fascicolo[0];
            DocsPaWR.Fascicolo[] appoFasc = new DocsPAWA.DocsPaWR.Fascicolo[0];

            if (classificazione.childs != null)
            {
                if (classificazione.childs.Length > 0)
                {
                    totalArrayFasc = addArrayFascicolo(allFasc, listaFasc);
                    for (int i = 0; i < classificazione.childs.Length; i++)
                    {
                        //						DocsPaWR.FascicolazioneClassificazione childClassificazione=(DocsPAWA.DocsPaWR.FascicolazioneClassificazione)classificazione.childs[i];
                        //						appoFasc=addArrayFascicolo(listaFasc,allFasc);
                        //						totalArrayFasc=recursiveListaFascForAllClassif(childClassificazione,appoFasc);
                        DocsPaWR.FascicolazioneClassificazione childClassificazione = (DocsPAWA.DocsPaWR.FascicolazioneClassificazione)classificazione.childs[i];
                        //appoFasc=addArrayFascicolo(listaFasc,allFasc);
                        totalArrayFasc = recursiveListaFascForAllClassif(childClassificazione, totalArrayFasc);
                    }
                }
                else
                {
                    totalArrayFasc = addArrayFascicolo(allFasc, listaFasc);
                }
            }
            else
            {
                totalArrayFasc = addArrayFascicolo(allFasc, listaFasc);
            }

            return totalArrayFasc;
        }

        private DocsPAWA.DocsPaWR.Fascicolo[] addArrayFascicolo(DocsPAWA.DocsPaWR.Fascicolo[] arr1, DocsPaWR.Fascicolo[] arr2)
        {
            DocsPaWR.Fascicolo[] somma = new DocsPAWA.DocsPaWR.Fascicolo[arr1.Length + arr2.Length];
            int newBase = -1;

            for (int i = 0; i < arr1.Length; i++)
            {
                somma[i] = arr1[i];
                newBase = i;
            }
            newBase++;

            for (int i = 0; i < arr2.Length; i++)
            {
                somma[i + newBase] = arr2[i];
            }

            return somma;
        }

        private DocsPAWA.DocsPaWR.Fascicolo[] getListaFascForAllClassific(DocsPAWA.DocsPaWR.FascicolazioneClassificazione classificazione)
        {
            DocsPaWR.Fascicolo[] allFasc = new DocsPAWA.DocsPaWR.Fascicolo[0];

            allFasc = recursiveListaFascForAllClassif(classificazione, new DocsPAWA.DocsPaWR.Fascicolo[0]);

            return allFasc;
        }

        private void aggiornaRisultatiRicerca(int numPage, out int nRec, out int numTotPage)
        {
            // Lista dei system id dei fascicoli restituiti dalla ricerca
            SearchResultInfo[] idProjects = null;

            // Impostazione indice pagina corrente
            DataGrid1.CurrentPageIndex = (numPage - 1);

            nRec = 0;
            numTotPage = 0;
            DocsPaWR.FascicolazioneClassificazione classificazione;

            if (!IsPostBack)
            {
                if (this.Request.QueryString["newFasc"] != null && this.Request.QueryString["newFasc"].Equals("1"))
                {
                    ListaFasc = new DocsPAWA.DocsPaWR.Fascicolo[1];
                    ListaFasc[0] = FascicoliManager.getFascicoloSelezionato(this);
                    FascicoliManager.setListaFascicoliInGriglia(this, ListaFasc);
                    if (ListaFasc.Length == 0)
                    {
                        this.PrintMsg("Nessun Fascicolo Trovato");
                        // Spostato in bottoniera operazioni massive
                        //this.btn_stampa.Visible = false;
                        return;
                    }
                    caricaDatagridFasc(DataGrid1, ListaFasc);

                    // In caso di inserimento nuovo fascicolo,
                    // vengono impostate le variabili di output
                    nRec = 1;
                    numTotPage = 1;
                }
                else
                {
                    FascicoliManager.removeFascicoloSelezionato(this);
                    FascicoliManager.removeDatagridFascicolo(this);
                    // Spostato in bottoniera operazioni massive
                    //this.btn_stampa.Visible = true;
                    // Spostato in bottoniera operazioni massive
                    //this.btn_stampa.Attributes.Add("onclick", "StampaRisultatoRicerca();");
                }
            }
            //se nella session non ho valori relativi ad un precedente caricamento della griglia
            //costruisco la griglia da riempire
            if (FascicoliManager.getDatagridFascicolo(this) == null)
            {
                if (idClass == 0)
                {
                    classificazione = null;
                    FascicoliManager.setClassificazioneSelezionata(this, classificazione);
                }
                else
                {
                    classificazione = FascicoliManager.getClassificazioneSelezionata(this);
                }

                DocsPaWR.Registro userReg = UserManager.getRegistroSelezionato(this);

                if (FascicoliManager.getFiltroRicFasc(this) != null)
                {
                    // Risoluzione bug 3481 INPS
                    if (classificazione == null && Session["classificaSelezionata"] != null)
                        try
                        {
                            classificazione = FascicoliManager.fascicolazioneGetTitolario2(
                                this,
                                ((DocsPaWR.FascicolazioneClassifica)Session["classificaSelezionata"]).codice,
                                false,
                                ((DocsPaWR.FascicolazioneClassifica)Session["classificaSelezionata"]).idTitolario)[0];
                        }
                        catch (Exception e) { /* Non si fa nulla */ }

                    this.ListaFiltri = FascicoliManager.getFiltroRicFasc(this);
                    FascicoliManager.setMemoriaFiltriRicFasc(this, this.ListaFiltri);
                    FascicoliManager.setMemoriaClassificaRicFasc(this, classificazione);
                    FascicoliManager.setMemoriaRegistroRicFasc(this, UserManager.getRegistroSelezionato(this));
                }
                else if (FascicoliManager.getMemoriaFiltriRicFasc(this) != null)
                {
                    this.ListaFiltri = FascicoliManager.getMemoriaFiltriRicFasc(this);
                    classificazione = FascicoliManager.getMemoriaClassificaRicFasc(this);
                }

                if (this.ListaFiltri != null)
                {
                    /* Se la variabile booleana 'allClass' è true ciò significa che è stata selezionata 
                         * l'opzione SI relativa al 'Mostra tutti i fascicoli' (ovvero se tale variabile è 
                         * TRUE allora vengono caricati, oltre ai fascicoli relativi alla classificazione 
                         * selezionata, anche quelli presenti nelle classificazioni figlie */
                    // Se non si è in postback viene caricata anche la lista dei system id dei fascicoli,
                    // altrimenti non viene caricata
                    if (!IsPostBack)
                    {
                        ListaFasc = FascicoliManager.getListaFascicoliPaging(this, classificazione, userReg, ListaFiltri[0], allClass, numPage, out numTotPage, out nRec, this.DataGrid1.PageSize, true, out idProjects, null);
                        this.moButtons.InitializeOrUpdateUserControl(idProjects);
                    }
                    else
                    {
                        ListaFasc = FascicoliManager.getListaFascicoliPaging(this, classificazione, userReg, ListaFiltri[0], allClass, numPage, out numTotPage, out nRec, this.DataGrid1.PageSize, true, out idProjects, null);
                    }

                    //La lista di fascicoli ottenuta viene messa in sessione
                    FascicoliManager.setListaFascicoliInGriglia(this, ListaFasc);

                    if (ListaFasc != null && ListaFasc.Length == 0)
                    {
                        this.PrintMsg("Nessun Fascicolo Trovato");
                        // Spostato in bottoniera operazioni massive
                        //this.btn_stampa.Visible = false;
                        return;
                    }
                    caricaDatagridFasc(DataGrid1, ListaFasc);

                }
                else
                {
                    //Per trasferimento in deposito... è possibile trasferire in deposito
                    //solo se si sta ricercando fascicoli procedimentali chiusi... questo è il
                    //caso in cui non si è effettuato alcun filtro, quindi non si può archiviare
                    this.btn_archivia.Visible = false;
                    this.DataGrid1.Columns[15].Visible = false;
                }
            }

            DataTable dataTable = FascicoliManager.getDatagridFascicolo(this);

            if (dataTable != null)
            {
                dv = dataTable.DefaultView;

                if (!Page.IsPostBack)
                {
                    this.DataGrid1.VirtualItemCount = nRec;

                }

                bindDataGrid(dv);

                int requisitiArchivio = 0;

                DocsPaWR.FiltroRicerca[] filterItems = null;
                if (ListaFiltri != null && ListaFiltri.Length > 0)
                    filterItems = ListaFiltri[0];
                if (filterItems != null)
                    foreach (DocsPaWR.FiltroRicerca filtro in filterItems)

                    //foreach (DocsPAWA.DocsPaWR.FiltroRicerca[] filtro in ListaFiltri)
                    {
                        if (filtro.argomento.Equals(DocsPaWR.FiltriFascicolazione.TIPO_FASCICOLO.ToString()) && filtro.valore.Equals("P"))
                        {
                            requisitiArchivio++;
                        }
                        if (filtro.argomento.Equals(DocsPaWR.FiltriFascicolazione.STATO.ToString()) && filtro.valore.Equals("C"))
                        {
                            requisitiArchivio++;
                        }
                    }
                if ((requisitiArchivio == 2) && ((UserManager.ruoloIsAutorized(this, "DO_ARCHIVIA")) || (UserManager.ruoloIsAutorized(this, "FASC_PROC_CHIUSI_ARCHIVIA"))))
                {
                    btn_archivia.Visible = true;
                    this.DataGrid1.Columns[15].Visible = true;

                }
                if ((requisitiArchivio == 2) && (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA")))
                {
                    btn_archivia.Visible = true;
                    btn_archivia.ImageUrl = "../images/proto/btn_corrente.gif";
                    btn_archivia.AlternateText = "Inserisci tutti i fascicoli in archivio corrente";
                    this.DataGrid1.Columns[15].Visible = true;
                    this.DataGrid1.Columns[15].HeaderText = "Trasf. corrente";
                    for (int i = 0; i < this.DataGrid1.Items.Count; i++)
                    {
                        ImageButton img = this.DataGrid1.Items[i].Cells[15].FindControl("img_Archivio") as ImageButton;
                        img.ImageUrl = "../images/proto/btn_corrente.gif";
                        img.AlternateText = "Inserisci il fascicolo in archivio corrente";
                    }
                }

            }

            if (!Page.IsPostBack)
            {
                if (enableUfficioRef && this.Request.QueryString["newFasc"] != null && this.Request.QueryString["newFasc"].Equals("1"))
                {
                    //Invia la trasmissione ai ruoli di riferimento dell'Ufficio Referente
                    if (!getRagTrasmissioneUfficioReferente())
                    {
                        string theAlert = "<script>alert('Attenzione! Ragione di trasmissione assente per l\\'ufficio referente.";
                        theAlert = theAlert + "\\nLa trasmissione non è stata effettuata.');</script>";
                        Response.Write(theAlert);
                    }
                    else
                    {
                        //implementare il resto qui
                        string esito = setCorrispondentiTrasmissione();
                        if (!esito.Equals(""))
                        {
                            esito = esito.Replace("'", "''");
                            //Page.RegisterStartupScript("chiudi","<script>alert('" + esito + "')</script>");
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "chiudi", "<script>alert('" + esito + "')</script>");
                            esito = "";
                        }
                        else
                        {
                            if (Session["EreditaSI_NO"] != null)
                            {
                                if (Session["EreditaSI_NO"].ToString() == "false")
                                {
                                    DocsPAWA.DocsPaWR.TrasmissioneSingola[] appoTrasmSingole = new DocsPAWA.DocsPaWR.TrasmissioneSingola[trasmissione.trasmissioniSingole.Length];
                                    for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                                    {
                                        DocsPAWA.DocsPaWR.TrasmissioneSingola trasmSing = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
                                        trasmSing = trasmissione.trasmissioniSingole[i];
                                        trasmSing.ragione.eredita = "0";
                                        appoTrasmSingole[i] = trasmSing;
                                    }
                                    trasmissione.trasmissioniSingole = appoTrasmSingole;
                                }
                            }
                            Session.Remove("EreditaSI_NO");
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
                    //rimozione variabili di sessione dopo la trasmissione
                    TrasmManager.removeGestioneTrasmissione(this);
                    TrasmManager.removeRagioneSel(this);
                    FascicoliManager.removeUoReferenteSelezionato(this);
                    //					FascicoliManager.removeFascicoloSelezionato(this);

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        protected void PrintMsg(string msg)
        {
            this.titolo.Text = msg;
            if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                this.msgADL.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshCountFascicoli()
        {
            string msg = "Elenco fascicoli";

            if (this.ListaFasc != null)
                msg += " - Trovati " + this.nRec.ToString() + " elementi.";
            else
                msg += " - Trovati 0 elementi.";

            this.PrintMsg(msg);
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
                            esito += "Trasmissione non effettuata - ruoli di riferimento non autorizzati nella UO:\\n";
                        esito += corrRef.descrizione;
                    }

                    TrasmManager.setGestioneTrasmissione(this, trasmissione);
                }

            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
            return esito;
        }

        private DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato setQCA(DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente qco)
        {
            DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qcAut = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();

            //Aggiunto il caso di trasmissione di un fascicolo
            if ((string)Session["OggettoDellaTrasm"] == "FASC")
            {
                qcAut.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                qcAut.idNodoTitolario = FascicoliManager.getFascicoloSelezionato(this).idClassificazione;
                qcAut.idRegistro = FascicoliManager.getFascicoloSelezionato(this).idRegistroNodoTit;
                if (qcAut.idRegistro != null && qcAut.idRegistro.Equals(""))
                    qcAut.idRegistro = null;
            }
            //cerco la ragione in base all'id che ho nella querystring
            qcAut.ragione = TrasmManager.getRagioneSel(this);
            if (TrasmManager.getGestioneTrasmissione(this) != null)
            {
                qcAut.ruolo = TrasmManager.getGestioneTrasmissione(this).ruolo;
            }
            qcAut.queryCorrispondente = qco;
            return qcAut;
        }

        private DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr)
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
            this.dataSetRFasc1 = new DocsPAWA.dataSet.DataSetRFasc();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetRFasc1)).BeginInit();
            this.btn_archivia.Click += new System.Web.UI.ImageClickEventHandler(this.btn_archivia_Click);
            this.DataGrid1.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid1_ItemCreated);
            this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
            this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrad1_OnPageIndexChanged);
            this.DataGrid1.PreRender += new System.EventHandler(this.Datagrid1_PreRender);
            this.DataGrid1.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.DataGrid1_SortCommand);
            this.DataGrid1.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid1_ItemDataBound);
            this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
            // 
            // dataSetRFasc1
            // 
            this.dataSetRFasc1.DataSetName = "DataSetRFasc";
            this.dataSetRFasc1.Locale = new System.Globalization.CultureInfo("en-US");
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);
            ((System.ComponentModel.ISupportInitialize)(this.dataSetRFasc1)).EndInit();

        }
        #endregion

        public string getDecodeForStato(string stato)
        {
            return FascicoliManager.decodeStatoFasc(this, stato);
        }

        public string getDecodeForTipo(string stato)
        {
            return FascicoliManager.decodeTipoFasc(this, stato);
        }

        private void ChangeDirectionSorting(string oldDirection)
        {
            string newValue;
            if (oldDirection != null && oldDirection.Equals("ASC"))
            {
                newValue = "DESC";
            }
            else
            {
                newValue = "ASC";
            }
            DirectionSorting = newValue;
        }

        private string DirectionSorting
        {
            get
            {
                string retValue;
                if (ViewState["directionSorting"] == null)
                {
                    ViewState["directionSorting"] = "ASC";
                }

                retValue = (string)ViewState["directionSorting"];
                return retValue;
            }
            set
            {
                ViewState["directionSorting"] = value;
            }
        }

        private void DataGrid1_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
        {
            string sortExpression;
            string direction;
            sortExpression = e.SortExpression;
            direction = this.DirectionSorting;
            ChangeDirectionSorting(direction);
            //DataSet ds  = ((DataSet)DataGrid1.DataSource);
            DataTable dataTable = FascicoliManager.getDatagridFascicolo(this);
            dv = dataTable.DefaultView;
            dv.Sort = sortExpression + " " + direction;
            bindDataGrid(dv);
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

        private void DataGrid1_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            //Utils.checkForDateNullOnItem(e.Item);
        }

        private void DataGrid1_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            TableCell pager = e.Item.Cells[0];
            foreach (object o in pager.Controls)
            {
                if (o is LinkButton)
                {
                    /*LinkButton lb = (LinkButton) o ;
                    lb.Font.Size = 10 ;
                    lb.Text = "[" + lb.Text + "]";
                    lb.ForeColor = Color.Black ;*/
                }
                /*if(o is Label )
                {
                    Label  lb = (Label ) o ;
                    lb.Font.Size = 10 ;
                    lb.Text = "[" + lb.Text + "]";
                    lb.ForeColor = Color.Black ;

                }*/
            }
        }
        //add massimo digregorio
        private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {                
                if (e.CommandName.Equals("delADL"))
                {
                    string h = Request.Form["hd1"];

                    if (this.hd1.Value.Equals("Yes"))
                    {
                        //elimino da todolist
                        DocsPaWR.Fascicolo fascicoloSelezionato;

                        this.ListaFasc = FascicoliManager.getListaFascicoliInGriglia(this);

                        int indexFascicolo = this.getIndexFascicoloSelezionato(e.Item);

                        this.SetCurrentFascIndexOnContext(indexFascicolo);

                        fascicoloSelezionato = ListaFasc[indexFascicolo];

                        DocumentManager.eliminaDaAreaLavoro(this, null, fascicoloSelezionato);

                        //ClientScript.RegisterStartupScript(this.GetType(), "success", "alert('Fascicolo Eliminato correttamente in ADL');", true);

                        ClientScript.RegisterStartupScript(this.GetType(), "LanciaRic", "top.principale.iFrame_sx.document.location='ricFasc.aspx?ricADL=1';", true);
                    }
                }
                if (e.CommandName.Equals("AreaConservazione"))
                {
                    DocsPaWR.Fascicolo fascicoloSelezionato;
                    this.ListaFasc = FascicoliManager.getListaFascicoliInGriglia(this);
                    int indexFascicolo = this.getIndexFascicoloSelezionato(e.Item);
                    this.SetCurrentFascIndexOnContext(indexFascicolo);
                    fascicoloSelezionato = ListaFasc[indexFascicolo];
                    //int isPrimaIstanza = DocumentManager.isPrimaConservazione(this, UserManager.getInfoUtente(this).idPeople, UserManager.getInfoUtente(this).idGruppo);
                    //if (isPrimaIstanza == 1)
                    //{
                    //    string popup = "<script> alert('Si sta per creare una nuova istanza di conservazione')</script>";
                    //    Page.RegisterClientScriptBlock("popUp", popup);
                    //}
                    string[] listaDoc;
                    listaDoc = FascicoliManager.getIdDocumentiFromFascicolo(fascicoloSelezionato.systemID);
                    if (listaDoc.Length > 0)
                    {
                        int isPrimaIstanza = DocumentManager.isPrimaConservazione(this, UserManager.getInfoUtente(this).idPeople, UserManager.getInfoUtente(this).idGruppo);
                        if (isPrimaIstanza == 1)
                        {
                            string popup = "<script> alert('Si sta per creare una nuova istanza di conservazione')</script>";
                            Page.RegisterClientScriptBlock("popUp", popup);
                        }
                        for (int i = 0; i < listaDoc.Length; i++)
                        {
                            DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = new DocsPAWA.DocsPaWR.SchedaDocumento();
                            //schedaDoc = DocumentManager.getDettaglioDocumento(this, listaDoc[i].ToString(), "");
                            schedaDoc = DocumentManager.getDettaglioDocumentoNoSecurity(this, listaDoc[i].ToString(), "");
                            if (schedaDoc != null)
                            {
                                string sysId = DocumentManager.addAreaConservazione(Page, schedaDoc.systemId, fascicoloSelezionato.systemID, schedaDoc.docNumber, UserManager.getInfoUtente(this), "F");
                                int size_xml = DocumentManager.getItemSize(Page, schedaDoc, sysId);
                                int doc_size = Convert.ToInt32(schedaDoc.documenti[0].fileSize);
                                int size_allegati = 0;
                                for (int j = 0; j < schedaDoc.allegati.Length; j++)
                                {
                                    size_allegati = size_allegati + Convert.ToInt32(schedaDoc.allegati[j].fileSize);
                                }
                                int total_size = size_allegati + doc_size + size_xml;

                                int numeroAllegati = schedaDoc.allegati.Length;
                                string fileName = schedaDoc.documenti[0].fileName;
                                string tipoFile = System.IO.Path.GetExtension(fileName);

                                DocumentManager.insertSizeInItemCons(Page, sysId, total_size);

                                DocumentManager.updateItemsConservazione(Page, tipoFile, Convert.ToString(numeroAllegati), sysId);
                            }
                        }
                        ((Label)e.Item.Cells[13].Controls[1]).Text = "1";
                    }
                    else
                    {
                        Response.Write("<script> alert('Il fascicolo non contiene alcun documento')</script>");
                    }
                }
                if (e.CommandName.Equals("EliminaAreaCons"))
                {
                    DocsPaWR.Fascicolo fascicoloSelezionato;
                    this.ListaFasc = FascicoliManager.getListaFascicoliInGriglia(this);
                    int indexFascicolo = this.getIndexFascicoloSelezionato(e.Item);
                    this.SetCurrentFascIndexOnContext(indexFascicolo);
                    fascicoloSelezionato = ListaFasc[indexFascicolo];
                    DocumentManager.eliminaDaAreaConservazione(Page, "", fascicoloSelezionato, null, false, "");
                    ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[12].Controls[1]).ToolTip = "Inserisci questo fascicolo in 'Area di conservazione'";
                    ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[12].Controls[1]).ImageUrl = "../images/proto/conservazione_d.gif";
                    ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[12].Controls[1]).CommandName = "AreaConservazione";
                    ((Label)e.Item.Cells[13].Controls[1]).Text = "0";
                }
                if (e.CommandName.Equals("InArchivio"))
                {
                    //Inserimento del fascicolo in archivio
                    DocsPaWR.Fascicolo fascicoloSelezionato;
                    string resultDoc = "";

                    string tipoOperazione = "";
                    if (UserManager.ruoloIsAutorized(this, "DO_ARCHIVIA") || UserManager.ruoloIsAutorized(this, "FASC_PROC_CHIUSI_ARCHIVIA"))
                    {
                        tipoOperazione = "IN_DEPOSITO";
                    }
                    else
                    {
                        tipoOperazione = "IN_CORRENTE";
                    }

                    this.ListaFasc = FascicoliManager.getListaFascicoliInGriglia(this);
                    int indexFascicolo = this.getIndexFascicoloSelezionato(e.Item);
                    this.SetCurrentFascIndexOnContext(indexFascicolo);
                    fascicoloSelezionato = ListaFasc[indexFascicolo];
                    //Ricarico il datagrid
                    if (FascicoliManager.trasfInDepositoFascProc(this, fascicoloSelezionato, UserManager.getInfoUtente(), tipoOperazione))
                        CaricaGridPostTD();
                }
                if (e.CommandName == "Dettaglio")
                {
                    this.selezionaRiga(this.DataGrid1.Items[e.Item.ItemIndex]);
                    ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.RemoveCurrentFilter();
                }
            }
            catch (Exception ex)
            {
                Exception eApp = null;
                var err = "";
                string PagChiamante = "RicercaFasc";
                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    PagChiamante = "RicercaADLFasc";
                err = string.Format("{0} \r\n {1} \r\n {2}", "Pagina chiamante: " + PagChiamante, "Errore: " + ex.Message.ToString(), "StackTrace: " + ex.StackTrace.ToString());
                eApp = new Exception(err);

                //ErrorManager.OpenErrorPage(this, err,PagChiamante);
                ErrorManager.redirectToErrorPage(this, eApp);
            }
        }

        //modify by massimo digregorio
        private void selezionaRiga(System.Web.UI.WebControls.DataGridItem dgItem)
        {
            try
            {
                Session.Add("fascicolo", "RICERCA");
                DocsPaWR.Fascicolo fascicoloSelezionato;
                //aggiornaRisultatiRicerca(currentPage,out nRec,out numTotPage);
                //if (ListaFasc!=null)
                //{
                this.ListaFasc = FascicoliManager.getListaFascicoliInGriglia(this);

                int indexFascicolo = this.getIndexFascicoloSelezionato(dgItem);

                // Impostazione indice fascicolo selezionato nel contesto
                this.SetCurrentFascIndexOnContext(indexFascicolo);

                fascicoloSelezionato = ListaFasc[indexFascicolo];
                FascicoliManager.setFascicoloSelezionato(this, fascicoloSelezionato);

                //quando ho effettuato una ricerca per sottofascicolo, viene mostrato nel
                // datagrid il fascicolo risultato della ricerca, ma se si effettua il 
                // click sul dettaglio si vuole andare al dettaglio del sottofascicolo e 
                // non del fascicolo padre.
                if (Session["isRicercaSottofascicoli"] != null && !((string)Session["isRicercaSottofascicoli"]).Equals(""))
                {
                    string sottofascicolo = (string)Session["isRicercaSottofascicoli"];
                    DocsPaWR.Registro registroFascicolo = null;
                    if (!string.IsNullOrEmpty(fascicoloSelezionato.idRegistroNodoTit))
                        registroFascicolo = UserManager.getRegistroBySistemId(this, fascicoloSelezionato.idRegistroNodoTit);
                    DocsPaWR.Folder[] folders = FascicoliManager.getListaFolderDaIdFascicolo(this, fascicoloSelezionato.systemID, registroFascicolo);
                    if (folders != null && folders.Length > 0)
                    {
                        bool trovato = false;
                        foreach (DocsPaWR.Folder fol in folders)
                        {
                            if (fol.descrizione.Equals(sottofascicolo))
                            {

                                fascicoloSelezionato.folderSelezionato = fol;
                                trovato = true;
                            }
                        }
                        if (!trovato)
                            fascicoloSelezionato.folderSelezionato = folders[0];
                    }
                    FascicoliManager.setFascicoloSelezionato(this, fascicoloSelezionato);
                    FascicoliManager.setFolderSelezionato(this, fascicoloSelezionato.folderSelezionato);
                }
                else
                    //{
                    FascicoliManager.setFolderSelezionato(this, null);

                loadNewFrameset();
                /* Metto in sessione il registro del fascicolo selezionato, come avviene quando
                ricerco un fascicolo da doc Classifica, attraverso il link al fascicolo.
                Se è NULL lo rimuovo dalla sessione */
                if (fascicoloSelezionato.idRegistroNodoTit != null && fascicoloSelezionato.idRegistroNodoTit != string.Empty)
                {
                    DocsPaWR.Registro registroFascicolo = UserManager.getRegistroBySistemId(this, fascicoloSelezionato.idRegistroNodoTit);
                    if (registroFascicolo != null)
                    {
                        UserManager.setRegistroSelezionato(this, registroFascicolo);
                    }
                }
                else
                {
                    //UserManager.setRegistroSelezionato(this, registroFascicolo);
                    //    }
                    //}
                    //else
                    //{
                    UserManager.removeRegistroSelezionato(this);
                }
                //}

                //Inserisco in sessione l'oggetto per lo scorrimento continuo della lista
                //UserControls.ScrollElementsList.ScrollManager.setInSessionNewObjScrollElementsList(this.DataGrid1.VirtualItemCount, this.DataGrid1.PageCount, this.DataGrid1.PageSize, indexFascicolo, this.DataGrid1.CurrentPageIndex, new ArrayList(ListaFasc), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_FASCICOLI);
                UserControls.ScrollElementsList.ScrollManager.setInContextNewObjScrollElementsList(this.DataGrid1.VirtualItemCount, this.DataGrid1.PageCount, this.DataGrid1.PageSize, indexFascicolo, this.DataGrid1.CurrentPageIndex, new ArrayList(ListaFasc), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_FASCICOLI);
            }
            catch (Exception ex)
            {
                Exception eApp = null;
                var err = "";
                string PagChiamante = "RicercaDoc";
                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    PagChiamante = "RicercaADLFasc";
                err = string.Format("{0} \r\n {1} \r\n {2}", "Pagina chiamante: " + PagChiamante, "Errore: " + ex.Message.ToString(), "StackTrace: " + ex.StackTrace.ToString());
                eApp = new Exception(err);

                //ErrorManager.OpenErrorPage(this, err,PagChiamante);
                ErrorManager.redirectToErrorPage(this, eApp);
            }
        }

        private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            /*massimo digregorio spostato in selezionaRiga():
            * old:
                                    DocsPaWR.Fascicolo fascicoloSelezionato;
                                    aggiornaRisultatiRicerca(currentPage,out nRec,out numTotPage);
                                    if (ListaFasc!=null)
                                    {
                                        fascicoloSelezionato=ListaFasc[getIndexFascicoloSelezionato()];
                                        FascicoliManager.setFascicoloSelezionato(this,fascicoloSelezionato);
                                        loadNewFrameset();
                                    }
            */
            this.selezionaRiga(this.DataGrid1.Items[this.DataGrid1.SelectedIndex]);
        }
        //modify by massimo digregorio
        private int getIndexFascicoloSelezionato(System.Web.UI.WebControls.DataGridItem dgItem)
        {
            //modifica
            string key = ((Label)dgItem.Cells[9].Controls[1]).Text;
            //fine modifica
            return Int32.Parse(key);
        }

        #region Gestione CallerContext

        /// <summary>
        /// Reperimento numero pagina corrente dal contesto di ricerca
        /// </summary>
        /// <returns></returns>
        private int GetCurrentPageOnContext()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_FASCICOLI)
                return currentContext.PageNumber;
            else
                return 1;
        }

        /// <summary>
        /// Impostazione numero pagina corrente del contesto di ricerca
        /// </summary>
        /// <param name="currentPage"></param>
        private void SetCurrentPageOnContext(int currentPage)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_FASCICOLI)
                currentContext.PageNumber = currentPage;
        }

        /// <summary>
        /// Impostazione dell'indice del fascicolo
        /// selezionato nel contesto di ricerca
        /// </summary>
        /// <param name="fascIndex"></param>
        private void SetCurrentFascIndexOnContext(int fascIndex)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_FASCICOLI)
                currentContext.QueryStringParameters["fascIndex"] = fascIndex.ToString();
        }

        /// <summary>
        /// Ripristino del fascicolo selezionato precedentemente
        /// </summary>
        /// <param name="dataGrid"></param>
        private void SetFascIndexFromQueryString(DataGrid dataGrid)
        {
            if (!this.IsPostBack)
            {
                string param = this.Request.QueryString["fascIndex"];

                if (param != null && param != string.Empty)
                {
                    int documentIndex = -1;
                    try
                    {
                        documentIndex = Int32.Parse(param);
                    }
                    catch
                    {
                    }

                    dataGrid.SelectedIndex = documentIndex;
                }
            }
        }

        #endregion

        private void AttatchGridPagingWaitControl()
        {
            DataGridPagingWaitControl.DataGridID = this.DataGrid1.ClientID;
            DataGridPagingWaitControl.WaitScriptCallback = "WaitDataGridCallback(eventTarget,eventArgument);";
        }

        private waiting.DataGridPagingWait DataGridPagingWaitControl
        {
            get
            {
                return this.FindControl("DataGridPagingWait1") as waiting.DataGridPagingWait;
            }
        }

        protected void DataGrid1_ItemCreated1(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }


        }

        #region Trasferimento deposito

        private void btn_archivia_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //inserisce tutti i fascicoli procedimentali chiusi in archivio
            //devo riprendere la query per tirare fuori tutti i fascicoli senza paginazione
            string tipoOperazione = "";
            if (UserManager.ruoloIsAutorized(this, "DO_ARCHIVIA") || UserManager.ruoloIsAutorized(this, "FASC_PROC_CHIUSI_ARCHIVIA"))
            {
                tipoOperazione = "IN_DEPOSITO";
            }
            else
            {
                tipoOperazione = "IN_CORRENTE";
            }

            this.ListaFiltri = FascicoliManager.getMemoriaFiltriRicFasc(this);
            FascicoliManager.trasfInDepositoALLFascicoliProc(Page, ListaFiltri, UserManager.getInfoUtente(this), tipoOperazione);
            this.DataGrid1.Visible = false;
            this.trHeader.Visible = false;
            this.lbl_messaggio.Visible = true;
            if (tipoOperazione == "IN_DEPOSITO")
                this.lbl_messaggio.Text = "Trasferimento in archivio di deposito avvenuto con successo.";
            else
                this.lbl_messaggio.Text = "Trasferimento in archivio corrente avvenuto con successo.";
        }

        private void CaricaGridPostTD()
        {
            DocsPaWR.FascicolazioneClassificazione classificazione;
            if (idClass == 0)
            {
                classificazione = null;
                FascicoliManager.setClassificazioneSelezionata(this, classificazione);
            }
            else
            {
                classificazione = FascicoliManager.getClassificazioneSelezionata(this);
            }
            if (ListaFiltri == null)
                this.ListaFiltri = FascicoliManager.getMemoriaFiltriRicFasc(this);

            // Lista dei system id dei fascicoli. Non utilizzata
            SearchResultInfo[] idProjectList = null;

            ListaFasc = FascicoliManager.getListaFascicoliPaging(this, classificazione, UserManager.getRegistroSelezionato(this), ListaFiltri[0], allClass, this.GetCurrentPageOnContext(), out numTotPage, out nRec, this.DataGrid1.PageSize, false, out idProjectList, null);
            //La lista di fascicoli ottenuta viene messa in sessione
            FascicoliManager.setListaFascicoliInGriglia(this, ListaFasc);
            if (ListaFasc != null && ListaFasc.Length == 0)
            {
                this.PrintMsg("Nessun Fascicolo Trovato");
                // Spostato in bottoniera operazioni massive
                //this.btn_stampa.Visible = false;
                this.DataGrid1.Visible = false;
                this.btn_archivia.Visible = false;
                return;
            }
            caricaDatagridFasc(DataGrid1, ListaFasc);
            DataTable dataTable = FascicoliManager.getDatagridFascicolo(this);
            if (dataTable != null)
            {
                dv = dataTable.DefaultView;
                this.DataGrid1.VirtualItemCount = nRec;
                bindDataGrid(dv);
            }
        }

        #endregion
        protected bool isVisibleColContatore()
        {
            bool result = false;
            if (Session["templateRicerca"] != null)
            {
                DocsPaWR.Templates template = (DocsPaWR.Templates)Session["templateRicerca"];
                foreach (DocsPaWR.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                {
                    if (oggettoCustom.DA_VISUALIZZARE_RICERCA == "1")
                    {

                        this.DataGrid1.Columns[2].HeaderText = oggettoCustom.DESCRIZIONE;
                        result = true;
                    }
                }
            }

            return result;
        }


        public string inserisciContatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string paramContatore)
        {
            string[] formatoContDaFunzione = paramContatore.Split('-');
            string[] formatoContDaSostituire = new string[] { "A", "A", "A" };

            //for (int i = 0; i < formatoContDaSostituire.Length; i++)
            //    formatoContDaSostituire[i] = string.Empty;

            formatoContDaFunzione.CopyTo(formatoContDaSostituire, 0);

            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return paramContatore;
            }

            //Imposto il contatore in funzione del formato
            string contatore = string.Empty;
            if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
            {
                contatore = oggettoCustom.FORMATO_CONTATORE;
                contatore = contatore.Replace("ANNO", formatoContDaSostituire[1].ToString());
                contatore = contatore.Replace("CONTATORE", formatoContDaSostituire[2].ToString());
                if (!string.IsNullOrEmpty(formatoContDaSostituire[0]))
                {
                    contatore = contatore.Replace("RF", formatoContDaSostituire[0].ToString());
                    contatore = contatore.Replace("AOO", formatoContDaSostituire[0].ToString());
                }
                else
                {
                    contatore = contatore.Replace("RF", "A");
                    contatore = contatore.Replace("AOO", "A");
                }
            }
            else
            {
                contatore = paramContatore;
            }

            return this.eliminaBlankSegnatura(contatore);

        }
        //fine modifica



        private string eliminaBlankSegnatura(string paramSegnatura)
        {
            char separatore = '|';
            string[] temp = paramSegnatura.Split('|');
            string appoggio = string.Empty;

            if (temp.Length == 1)
            {
                temp = paramSegnatura.Split('-');
                separatore = '-';
            }

            for (int i = 0; i < temp.Length; i++)
            {
                if (!temp[i].Equals("A"))
                {
                    appoggio += temp[i];
                    if (i != temp.Length - 1)
                        appoggio += separatore;
                }
            }
            return appoggio;
        }


        //fine modifica

    }
}
