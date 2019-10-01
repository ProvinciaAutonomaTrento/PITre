using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;

namespace DocsPAWA.RicercaCampiComuni
{
    public partial class RisultatoRicercaCampiComuni : DocsPAWA.CssPage
    {
        protected ArrayList listaDocFasc;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] listaFiltri;
        protected int currentPage = 1;
        protected int nRec = 0;
        protected int pageSize = 20;
        protected DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;
        protected string arrivo;
        protected string partenza;
        protected string interno;
        protected string grigio;

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);

            listaFiltri = (DocsPAWA.DocsPaWR.FiltroRicerca[][])SiteNavigation.CallContextStack.CurrentContext.ContextState["filtriRicercaCampiComuni"];
            getLettereProtocolli();

            if (!IsPostBack)
            {
                this.AttatchGridPagingWaitControl();
                popolaDataGrid();
            }
        }

        private void popolaDataGrid()
        {
            if (listaFiltri != null)
            {
                currentPage = GetCurrentPageOnContext();
                listaDocFasc = DocsPAWA.Utils.eseguiRicercaCampiComuni(UserManager.getInfoUtente(this), listaFiltri, currentPage, pageSize, out nRec, this);
                this.DataGrid.VirtualItemCount = nRec;
                
                DataTable dt = new DataTable();
                dt.Columns.Add("SYSTEM_ID");
                dt.Columns.Add("Descrizione-Oggetto");
                dt.Columns.Add("Tipo");
                dt.Columns.Add("Codice-Segnatura");
                dt.Columns.Add("Data-Creazione");

                foreach (DocsPAWA.DocsPaWR.ItemRicCampiComuni itemCampiComuni in listaDocFasc)
                {
                    DataRow dr = dt.NewRow();
                    dr["SYSTEM_ID"] = itemCampiComuni.SYSTEM_ID;
                    dr["Descrizione-Oggetto"] = itemCampiComuni.DESCRIPTION;
                    if (itemCampiComuni.TIPO != "F")
                        dr["Tipo"] = getEtichetta(itemCampiComuni.TIPO);
                    else
                        dr["Tipo"] = itemCampiComuni.TIPO;   
                    dr["Codice-Segnatura"] = itemCampiComuni.CODICE_SEGNATURA;
                    dr["Data-Creazione"] = itemCampiComuni.DATA_CREAZIONE;

                    dt.Rows.Add(dr);
                }

                titolo.Text = "Trovati " + nRec + " Fascicoli-Documenti";

                DataGrid.DataSource = dt;
                DataGrid.CurrentPageIndex = currentPage - 1;
                DataGrid.SelectedIndex = GetDocFascIndexFromQueryString();
                DataGrid.DataBind();

                if (listaDocFasc.Count == 0)
                    DataGrid.Visible = false;
            }
        }

        protected void DataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            DataGrid.CurrentPageIndex = e.NewPageIndex;
            currentPage = e.NewPageIndex + 1;

            SetCurrentPageOnContext(currentPage);
            SetCurrentDocFascIndexOnContext(-1);
            popolaDataGrid();            
        }

        private void AttatchGridPagingWaitControl()
        {
            DataGridPagingWaitControl.DataGridID = this.DataGrid.ClientID;
            DataGridPagingWaitControl.WaitScriptCallback = "WaitDataGridCallback(eventTarget,eventArgument);";
        }

        private waiting.DataGridPagingWait DataGridPagingWaitControl
        {
            get
            {
                return this.FindControl("DataGridPagingWait") as waiting.DataGridPagingWait;
            }
        }
        
        protected void DataGrid_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            int elSelezionato = e.Item.ItemIndex;
            
            switch (e.CommandName)
            {
                case "Select":
                    string idDocOrFasc = DataGrid.Items[elSelezionato].Cells[0].Text;
                    string tipo = DataGrid.Items[elSelezionato].Cells[2].Text;
                    //Documenti    
                    if (tipo == this.arrivo || tipo == this.partenza || tipo == this.interno || tipo == this.grigio)
                    {
                        DocsPAWA.DocsPaWR.InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(idDocOrFasc, null, this);
                        DocumentManager.setRisultatoRicerca(this, infoDoc);
                        Session.Remove("listInArea");
                        DocumentManager.removeListaDocProt(this);
                        FascicoliManager.removeFascicoloSelezionatoFascRapida();
                        //Documenti Grigi
                        if (tipo == this.grigio)
                            Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=profilo';</script>");
                        //Protocolli
                        else
                            Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=protocollo';</script>");
                    }
                    //Fascicoli
                    if (tipo == "F")
                    {
                        DocsPAWA.DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloById(this, idDocOrFasc);
                        FascicoliManager.setFascicoloSelezionato(this, fasc);
                        Response.Write("<script language='javascript'> top.principale.document.location = '../fascicolo/gestioneFasc.aspx?tab=documenti';</script>");
                    }

                    SetCurrentDocFascIndexOnContext(elSelezionato);
                    break;

                case "Page":
                    DataGrid.SelectedIndex = -1;
                    break;
            }            
        }

        private int GetCurrentPageOnContext()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null)
                return currentContext.PageNumber;
            else
                return 1;
        }

        private void SetCurrentPageOnContext(int currentPage)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null)
                currentContext.PageNumber = currentPage;
        }

        private int GetDocFascIndexFromQueryString()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            int docFascIndex = -1;

            if (currentContext != null)
            {
                try
                {
                    docFascIndex = Int32.Parse(currentContext.QueryStringParameters["docFascIndex"].ToString());
                }
                catch{}
            }
            return docFascIndex;
        }

        private void SetCurrentDocFascIndexOnContext(int docFascIndex)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null)
                currentContext.QueryStringParameters["docFascIndex"] = docFascIndex.ToString();
        }

        private void getLettereProtocolli()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.InfoUtente infoUtente = new DocsPAWA.DocsPaWR.InfoUtente();
            infoUtente = UserManager.getInfoUtente(this);
            String idAmm = UserManager.getInfoUtente().idAmministrazione;

            this.etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
            this.arrivo = etichette[0].Descrizione; //Valore A
            this.partenza = etichette[1].Descrizione; //Valore P
            this.interno = etichette[2].Descrizione; //Valore I
            this.grigio = etichette[3].Descrizione; //Valore G
        }

        private string getEtichetta(string value)
        {
            if ((value == "G") || (value == "R"))
            {
                return this.grigio;
            }
            else
            {
                if (value == "A")
                {
                    return this.arrivo;
                }
                else
                {
                    if (value == "I")
                    {
                        return this.interno;
                    }
                    else
                    {
                        return this.partenza;
                    }
                }
            }
        }

    }
}