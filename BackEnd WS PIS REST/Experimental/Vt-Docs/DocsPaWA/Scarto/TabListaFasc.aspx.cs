using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using log4net;

namespace DocsPAWA.Scarto
{
    public partial class TabListaFasc : DocsPAWA.CssPage
    {
        private ILog logger = LogManager.GetLogger(typeof(TabListaFasc));
        protected ArrayList dataTableProt;
        protected int nRec;
        protected int numTotPage;

        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    Utils.startUp(this);
                    this.AttachWaitingControl();
                    
                    
                    this.FillData(this.dg_Fasc.CurrentPageIndex + 1);
                    
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
            }
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
            this.Load += new System.EventHandler(this.Page_Load);
            this.dg_Fasc.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_Fasc_ItemCommand);
            //this.dg_Fasc.PreRender += new System.EventHandler(this.dg_Fasc_PreRender);
            this.dg_Fasc.SelectedIndexChanged += new System.EventHandler(this.dg_Fasc_SelectedIndexChanged);
        }
        #endregion

        #region DATAGRID
        private void FillData(int requestedPage)
        {
            try
            {
                dataTableProt = new ArrayList();


                caricaDataTablesFascicoli(requestedPage, out nRec, out numTotPage);
                
               
                this.dg_Fasc.DataSource = dataTableProt;
                this.dg_Fasc.DataBind();

                // Tentativo di ripristino del fascicolo selezionato (se è presente un contesto precedente)
                string prevIdFasc = this.GetCurrentContextIdFasc();

                int selectedIndex = -1;

                foreach (DataGridItem item in dg_Fasc.Items)
                {
                    selectedIndex++;

                    if (prevIdFasc.Equals(dataTableProt[selectedIndex].ToString()))
                    {
                        dg_Fasc.SelectedIndex = selectedIndex;
                        break;
                    }

                }

                this.dg_Fasc.Visible = (this.TotalRecordCount > 0);
                this.trHeader.Visible = (this.TotalRecordCount > 0);
                this.lbl_messaggio.Visible = false;
                if (TotalRecordCount == 0)
                {
                    this.lbl_messaggio.Text = "Nessun fascicolo presente.";
                    this.lbl_messaggio.Visible = true;
                }
            }
            catch (Exception es)
            {
                string error = es.ToString();
            }
        }

        //Recupera la lista di fascicoli procedimentali chiusi appartenenti al nodo selezionato, in deposito 
        private void caricaDataTablesFascicoli(int numPage, out int nRec, out int numTotPage)
        {
            nRec = 0;
            numTotPage = 0;
            try
            {
                dataTableProt = new ArrayList();
                DocsPaWR.Fascicolo[] listaFasc = null;

                //Posso ricercare i fascicoli procedimentali chiusi per:
                //codice fascicolo: C
                //tutti: T
                //mesi di conservazione: M
                string tipoRic = Request.QueryString["tipoR"];
                if (tipoRic == "C")
                {
                    DocsPAWA.DocsPaWR.Fascicolo fascicolo;
                    fascicolo = FascicoliManager.getFascicoloSelezionato(this);
                    if (fascicolo != null)
                        listaFasc = FascicoliManager.getListaFascicoliInDeposito(this, fascicolo, numPage, out numTotPage, out nRec, "C");
                    FascicoliManager.removeFascicoloSelezionato();
                    Session.Add("totRec", nRec);
                }
                if (tipoRic == "T")
                {
                    listaFasc = FascicoliManager.getListaFascicoliInDeposito(this, null, numPage, out numTotPage, out nRec, tipoRic);
                    Session.Add("totRec", nRec);
                }
                if (tipoRic == "R")
                {
                    listaFasc = FascicoliManager.getListaFascicoliInGriglia(this);
                    dataTableProt = FascicoliManager.getDataTableDocDaArchiv(this);
                    nRec = Convert.ToInt32(Session["totRec"]);
                }
                
                this.TotalRecordCount = nRec;
                this.dg_Fasc.VirtualItemCount = this.TotalRecordCount;
                if (listaFasc != null && tipoRic!="R")
                {
                    if (listaFasc.Length > 0)
                    {
                        for (int i = 0; i < listaFasc.Length; i++)
                        {
                            DocsPaWR.Fascicolo fasc = listaFasc[i];
                            //if (fasc.inScarto == "0")
                            //{
                                //calcolo mesi: oggi - chiusura = mesi dalla chiusura
                                int numMesi = 0;
                                DateTime today = DateTime.Today;
                                DateTime chiusura = Convert.ToDateTime(fasc.chiusura);

                                if (today.Year == chiusura.Year)
                                    numMesi = today.Month - chiusura.Month;
                                if (today.Year > chiusura.Year)
                                {
                                    int intervallo = today.Year - chiusura.Year;
                                    numMesi = today.Month - chiusura.Month + (12 * intervallo);
                                }

                                string numMesiChiusura = numMesi.ToString(); 

                                dataTableProt.Add(new Cols(fasc.stato, fasc.tipo, fasc.codiceGerarchia, fasc.codice, fasc.descrizione, fasc.chiusura, fasc.numMesiConservazione, numMesiChiusura, fasc.inScarto, i));
                            //}
                        }
                    }
                }
               
                FascicoliManager.setListaFascicoliInGriglia(this, listaFasc);
                //impostazione datatable in sessione
                FascicoliManager.setDataTableDocDaArchiv(this, dataTableProt);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void dg_Fasc_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName.Equals("AreaScarto"))
            {
                DocsPaWR.Fascicolo fascicoloSelezionato;
                DocsPaWR.Fascicolo[] listaFasc = null;
                listaFasc = FascicoliManager.getListaFascicoliInGriglia(this);
                fascicoloSelezionato = listaFasc[e.Item.ItemIndex];
                
                string[] listaDoc;
                listaDoc = FascicoliManager.getIdDocumentiFromFascicolo(fascicoloSelezionato.systemID);
                if (listaDoc.Length > 0)
                {
                    int isPrimaIstanza = FascicoliManager.isPrimaIstanzaScarto(this, UserManager.getInfoUtente(this).idPeople, UserManager.getInfoUtente(this).idGruppo);
                    if (isPrimaIstanza == 1)
                    {
                        string popup = "<script> alert('Si sta per creare una nuova istanza di scarto')</script>";
                        Page.RegisterClientScriptBlock("popUp", popup);
                    }
                    for (int i = 0; i < listaDoc.Length; i++)
                    {
                        DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = new DocsPAWA.DocsPaWR.SchedaDocumento();
                        schedaDoc = DocumentManager.getDettaglioDocumento(this, listaDoc[i].ToString(), "");
                        string sysId = FascicoliManager.addAreaScarto(Page, schedaDoc.systemId, fascicoloSelezionato.systemID, schedaDoc.docNumber, UserManager.getInfoUtente(this));
                    }
                    FillData(this.dg_Fasc.CurrentPageIndex + 1);
                    //caricaDataTablesFascicoli(this.dg_Fasc.CurrentPageIndex + 1, out nRec, out numTotPage);
                    //((Label)e.Item.Cells[9].Controls[1]).Text = "1";
                }
                else
                {
                    Response.Write("<script> alert('Il fascicolo non contiene alcun documento')</script>");
                }
            }
            
        }

        

        private void dg_Fasc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            DocsPaWR.Fascicolo fascicoloSelezionato;
            DocsPaWR.Fascicolo[] listaFasc = FascicoliManager.getListaFascicoliInGriglia(this);
            string key = ((Label)(this.dg_Fasc.Items[this.dg_Fasc.SelectedIndex].Cells[10].Controls[1])).Text;
            int indexFascicolo = Int32.Parse(key);

            this.SetCurrentFascIndexOnContext(indexFascicolo);
            // Aggiornamento contesto
            this.SetCurrentContext(((DataGrid)sender).CurrentPageIndex + 1, key);
            
            fascicoloSelezionato = listaFasc[indexFascicolo];
            FascicoliManager.setFascicoloSelezionato(this, fascicoloSelezionato);

            string newUrl = "../fascicolo/gestioneFasc.aspx?tab=documenti";
            Response.Write("<script language='javascript'>top.principale.document.location='" + newUrl + "';</script>");
            //TODOREGISTRO:
        }

        #endregion

        #region pulsanti

        protected string WaitingPanelTitle
        {
            get
            {
               return "Trasferimento in area di scarto in corso...";
            }
        }

        protected void btn_scarto_Click(object sender, ImageClickEventArgs e)
        {
            bool result = true;
            string tipoRic = Request.QueryString["tipoR"];
            DocsPAWA.DocsPaWR.Fascicolo fascicolo;
            fascicolo = FascicoliManager.getFascicoloSelezionato(this);
            if (fascicolo != null)
                result = FascicoliManager.addAllFascAreaScarto(this, fascicolo, UserManager.getInfoUtente(this), tipoRic);
            else
                result = FascicoliManager.addAllFascAreaScarto(this, null, UserManager.getInfoUtente(this), tipoRic);
            if (result)
            {
                Response.Write("<SCRIPT>alert('Inserimento in area di scarto avvenuto con successo.')</SCRIPT>");
                dg_Fasc.Visible = false;
                btn_scarto.Visible = false;
                trHeader.Visible = false;
            }
        }
        #endregion


        #region Gestione DataGridPagingWait

        /// <summary>
        /// Attatch del controllo "DataGridPagingWait" al datagrid
        /// </summary>
        private void AttachWaitingControl()
        {
            this.WaitingControl.DataGridID = this.dg_Fasc.ClientID;
            this.WaitingControl.WaitScriptCallback = "WaitGridPagingAction();";
        }

        /// <summary>
        /// Reperimento controllo "DataGridPagingWait"
        /// </summary>
        private waiting.DataGridPagingWait WaitingControl
        {
            get
            {
                return this.FindControl("dg_Fasc_pagingWait") as waiting.DataGridPagingWait;
            }
        }

        #endregion

        private int TotalRecordCount
        {
            get
            {
                int count = 0;

                if (this.ViewState["TotalRecordCount"] != null)
                    Int32.TryParse(this.ViewState["TotalRecordCount"].ToString(), out count);

                return count;
            }
            set
            {
                this.ViewState["TotalRecordCount"] = value;
            }
        }

        private void RefreshCountDocumenti()
        {
            this.titolo.Text = "Elenco fascicoli - Trovati " + this.TotalRecordCount.ToString() + " elementi.";
            
        }

        #region Gestione CallContext

        /// <summary>
        /// Reperimento numero pagina selezionata nel contesto corrente
        /// </summary>
        /// <returns></returns>
        protected int GetCurrentContextPage()
        {
            int currentPage = 1;

            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null &&
                currentContext.ContextName == SiteNavigation.NavigationKeys.GESTIONE_SCARTO)
            {
                // Ripristino pagina precedentemente visualizzata
                currentPage = currentContext.PageNumber;
            }

            return currentPage;
        }

        /// <summary>
        /// Reperimento id trasmissione selezionata nel contesto corrente
        /// </summary>
        /// <returns></returns>
        protected string GetCurrentContextIdFasc()
        {
            string idFasc = string.Empty;

            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null &&
                currentContext.ContextName == SiteNavigation.NavigationKeys.GESTIONE_SCARTO &&
                currentContext.IsBack &&
                currentContext.QueryStringParameters.ContainsKey("idFasc"))
            {
                // Ripristino trasmissione precedentemente selezionata
                idFasc = currentContext.QueryStringParameters["idFasc"].ToString();
            }

            return idFasc;
        }

        /// <summary>
        /// Aggiornamento contesto corrente
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="idTrasmissione"></param>
        private void SetCurrentContext(int currentPage, string idFascicolo)
        {
            try
            {
                SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

                if (currentContext.ContextName == SiteNavigation.NavigationKeys.GESTIONE_SCARTO)
                {
                    currentContext.PageNumber = currentPage;

                    if (idFascicolo != string.Empty)
                        currentContext.QueryStringParameters["idFasc"] = idFascicolo;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel SetCurrentContext dello SCARTO: " + ex.Message);
            }
        }

        /// <summary>
        /// Impostazione dell'indice del fascicolo
        /// selezionato nel contesto di ricerca
        /// </summary>
        /// <param name="fascIndex"></param>
        private void SetCurrentFascIndexOnContext(int fascIndex)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.GESTIONE_SCARTO)
                currentContext.QueryStringParameters["fascIndex"] = fascIndex.ToString();
        }

        #endregion


        public class Cols
        {
            private string stato;
            private string tipo;
            private string codClass;
            private string codice;
            private string descrizione;
            //private string apertura;
            private string mesiCons;
            private string mesiChiusura;
            private string chiusura;
            private string inScarto;
            private int chiave;

            public Cols(string stato, string tipo, string codClass, string codice, string descrizione, string chiusura, string mesiCons, string mesiChiusura, string inScarto, int chiave)
            {
                this.stato = stato;
                this.tipo = tipo;
                this.codClass = codClass;
                this.codice = codice;
                this.descrizione = descrizione;
                //this.apertura = apertura;
                this.mesiCons = mesiCons;
                this.mesiChiusura = mesiChiusura;
                this.chiusura = chiusura;
                this.inScarto = inScarto;
                this.chiave = chiave;
            }

            public string Stato { get { return stato; } }
            public string Tipo { get { return tipo; } }
            public string CodClass { get { return codClass; } }
            public string Codice { get { return codice; } }
            public string Descrizione { get { return descrizione; } }
            //public string Apertura { get { return apertura; } }
            public string MesiCons { get { return mesiCons; } }
            public string MesiChiusura { get { return mesiChiusura; } }
            public string Chiusura { get { return chiusura; } }
            public string InScarto { get { return inScarto; } }
            public int Chiave { get { return chiave; } }
        }

    }
}
