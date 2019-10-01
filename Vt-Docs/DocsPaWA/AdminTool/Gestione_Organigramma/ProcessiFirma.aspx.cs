using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Gestione_Organigramma
{
    public partial class ProcessiFirma : System.Web.UI.Page
    {
        #region CONSTANT

        private const string RUOLO = "R";
        private const string UTENTE = "U";
        private const string RUOLO_UTENTE = "RU";

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
        }

        private void InitializePage()
        {
            string tipo = this.Request.QueryString["from"].ToString();
            switch (tipo)
            {
                case RUOLO:
                    this.LblProcessiFirma.Text = "Processi di firma in cui il ruolo è coinvolto";
                    this.LblIstanzaProcessi.Text = "Istanza processi di firma in cui il ruolo è coinvolto";
                    break;
                case UTENTE:
                    this.LblProcessiFirma.Text = "Processi di firma in cui l'utente è coinvolto";
                    this.LblIstanzaProcessi.Text = "Istanza processi di firma in cui l'utente è coinvolto";
                    break;
                case RUOLO_UTENTE:
                    this.LblProcessiFirma.Text = "Processi di firma in cui l'utente è coinvolto nel ruolo selezionato";
                    this.LblIstanzaProcessi.Text = "Istanza processi di firma in cui l'utente è coinvolto nel ruolo selezionato";
                    break;
            }
            BuildGrdProcessiFirma();
            BuildGrdIstanzaProcessiFirma();
        }

        #region PROCESSI FIRMA
        private void BuildGrdProcessiFirma()
        {
            string idRuoloTitolare = string.Empty;
            string idUtenteTitolare = string.Empty;
            string tipo = this.Request.QueryString["from"].ToString();
            switch (tipo)
            {
                case RUOLO:
                    idRuoloTitolare = this.Request.QueryString["idGruppo"].ToString();
                    break;
                case UTENTE:
                    idUtenteTitolare = this.Request.QueryString["idUtente"].ToString();
                    break;
                case RUOLO_UTENTE:
                    idUtenteTitolare = this.Request.QueryString["idUtente"].ToString();
                    idRuoloTitolare = this.Request.QueryString["idGruppo"].ToString();
                    break;
            }
            int numPage = grd_istanza_processi.CurrentPageIndex + 1;
            int numTotPage = 0;
            int nRec = 0;
            int pageSize = this.grd_istanza_processi.PageSize;
            DocsPAWA.DocsPaWR.ProcessoFirma[] listaProcessi = LoadProcessiFirmaTitolare(idRuoloTitolare, idUtenteTitolare, numPage, pageSize, out numTotPage, out nRec);
            if (listaProcessi != null && listaProcessi.Length > 0)
            {
                this.pnlNoProcessiFirma.Visible = false;
                this.grd_processi_firma.DataSource = listaProcessi;
                grd_istanza_processi.VirtualItemCount = nRec;
                grd_processi_firma.DataBind();
            }
            else
            {
                this.pnlNoProcessiFirma.Visible = true; 
            }
        }

        private DocsPAWA.DocsPaWR.ProcessoFirma[] LoadProcessiFirmaTitolare(string idRuoloTitolare, string idUtenteTitolare, int numPage, int pageSize, out int numTotPage, out int nRec)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            numTotPage = 0;
            nRec = 0;
            return ws.GetProcessiDiFirmaByTitolare(idRuoloTitolare, idUtenteTitolare, numPage, pageSize, out numTotPage, out nRec);
        }

        protected string GetUtenteCreatore(DocsPAWA.DocsPaWR.ProcessoFirma processo)
        {
            string utente = UserManager.GetUtenteByIdPeople(processo.idPeopleAutore).descrizione;
            string ruolo = UserManager.getRuoloByIdGruppo(processo.idRuoloAutore , this.Page).descrizione;

            return utente + " (" + ruolo + ")";
        }

        protected string GetDiagrammaStato(DocsPAWA.DocsPaWR.ProcessoFirma processo)
        {
            string descDiagrammi = string.Empty;

            descDiagrammi = new DocsPaWebService().GetDescDiagrammiByIdProcesso(processo.idProcesso);

            return descDiagrammi;
        }

        protected void grdProcessi_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            grd_processi_firma.CurrentPageIndex = e.NewPageIndex;
            BuildGrdProcessiFirma();
            this.UpPnlGrdProcessi.Update();
        }

        #endregion
        #region ISTANZA PROCESSI FIRMA
        private void BuildGrdIstanzaProcessiFirma()
        {
            string idRuoloTitolare = string.Empty;
            string idUtenteTitolare = string.Empty;
            string tipo = this.Request.QueryString["from"].ToString();
            switch (tipo)
            {
                case RUOLO:
                    idRuoloTitolare = this.Request.QueryString["idGruppo"].ToString();
                    break;
                case UTENTE:
                    idUtenteTitolare = this.Request.QueryString["idUtente"].ToString();
                    break;
                case RUOLO_UTENTE:
                    idUtenteTitolare = this.Request.QueryString["idUtente"].ToString();
                    idRuoloTitolare = this.Request.QueryString["idGruppo"].ToString();
                    break;
            }
            int numPage = grd_istanza_processi.CurrentPageIndex + 1;
            int numTotPage = 0;
            int nRec = 0;
            int pageSize = this.grd_istanza_processi.PageSize;
            DocsPAWA.DocsPaWR.IstanzaProcessoDiFirma[] listaIstanzaProcessi = LoadIstanzaProcessiFirmaTitolare(idRuoloTitolare, idUtenteTitolare, numPage, pageSize, out numTotPage, out nRec);
            if (listaIstanzaProcessi != null && listaIstanzaProcessi.Length > 0)
            {
                this.pnlNoIstanzeProcessiFirma.Visible = false;
                grd_istanza_processi.DataSource = listaIstanzaProcessi;
                grd_istanza_processi.VirtualItemCount = nRec;
                grd_istanza_processi.DataBind();
            }
            else
            {
                this.pnlNoIstanzeProcessiFirma.Visible = true;
            }
        }

        protected void grdIstanzaProcessi_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            grd_istanza_processi.CurrentPageIndex = e.NewPageIndex;
            BuildGrdIstanzaProcessiFirma();
            this.UpPnlGrdIstanzaProcessi.Update();
        }

        private DocsPAWA.DocsPaWR.IstanzaProcessoDiFirma[] LoadIstanzaProcessiFirmaTitolare(string idRuoloTitolare, string idUtenteTitolare, int numPage, int pageSize, out int numTotPage, out int nRec)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            numTotPage = 0;
            nRec = 0;
            return ws.GetIstanzeProcessiDiFirmaByTitolare(idRuoloTitolare, idUtenteTitolare, numPage, pageSize, out numTotPage, out nRec);
        }

        protected string GetUtenteCreatore(DocsPAWA.DocsPaWR.IstanzaProcessoDiFirma istanza)
        {
            return string.Empty;
        }

        protected string GetUtenteProponente(DocsPAWA.DocsPaWR.IstanzaProcessoDiFirma istanza)
        {
            return istanza.UtenteProponente.descrizione + " (" + istanza.RuoloProponente.descrizione + ")";
        }

        protected string GetTipoDocumento(DocsPAWA.DocsPaWR.IstanzaProcessoDiFirma istanza)
        {
            return istanza.docAll.Equals("D") ? "Documento" : "Allegato";
        }
        #endregion
        #region EXPORT
        protected void btnExport_Click(object sender, EventArgs e)
        {
            string idRuoloTitolare = string.Empty;
            string idUtenteTitolare = string.Empty;
            string tipo = this.Request.QueryString["from"].ToString();
            switch (tipo)
            {
                case RUOLO:
                    idRuoloTitolare = this.Request.QueryString["idGruppo"].ToString();
                    break;
                case UTENTE:
                    idUtenteTitolare = this.Request.QueryString["idUtente"].ToString();
                    break;
                case RUOLO_UTENTE:
                    idUtenteTitolare = this.Request.QueryString["idUtente"].ToString();
                    idRuoloTitolare = this.Request.QueryString["idGruppo"].ToString();
                    break;
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenExport", "OpenExport('" + idRuoloTitolare + "','" + idUtenteTitolare + "');", true);
        }
        #endregion
    }
}