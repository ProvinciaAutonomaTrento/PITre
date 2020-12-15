using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.popup
{
    public partial class avvisoProtocolloEsistente : DocsPAWA.CssPage
    {
        //protected System.Web.UI.WebControls.Label lbl_messaggio;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    string queryS = Server.UrlDecode(Request.QueryString[0]);
                    string[] values = queryS.Split("&".ToCharArray());
                    //carico i valori di querystring
                    if (values.Length > 6 && values[0] != null && values[1] != null
                        && values[2] != null && values[3] != null && values[4] != null
                        && values[5] != null)
                    {
                        lblData.Text = (values[0].Split('='))[1];
                        txtSegnatura.Text = (values[1].Split('='))[1];
                        txtOggetto.Text = (values[2].Split('='))[1].Replace("|@ap@|", "'");
                        IdProf.Value = (values[3].Split('='))[1];
                        lblIdDocumento.Text = (values[3].Split('='))[1];
                        lblNumProtocollo.Text = (values[4].Split('='))[1];                        
                        Session.Add("ProtoExist", true);
                    }

                    if (values.Length > 7 && values[7]!=null)
                    {
                        modelloTrasm.Value = (values[7].Split('='))[1];
                        if (!modelloTrasm.Value.Equals(""))
                            Session.Add("modelloTrasmDuplicato", modelloTrasm.Value);
                    }


                    if (((values[5].Split('='))[1]) == "NO")
                    {
                        btn_protocolla.Visible = false;
                    }

                    if (values.Length > 8)
                    {
                        if (((values[8].Split('='))[1]) == "0")
                        {
                            this.btn_VisDoc.Visible = false;
                        }
                        else
                            this.btn_VisDoc.Visible = true;
                    }
                    else
                        this.btn_VisDoc.Visible = false;

                    string res = (values[6].Split('='))[1];
                    if (res != null)
                    {
                        if (res.Equals(DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum.ProtocolloNullo.ToString()))
                            this.lbl_messaggio.Text = "Nessuna scheda documento presente con questi dati";
                        if (res.Equals(DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum.NoProtocolloIngresso.ToString()))
                            this.lbl_messaggio.Text = "Il protocollo utilizzato non è quello in entrata";
                        if (res.Equals(DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum.NoMittente.ToString()))
                            this.lbl_messaggio.Text = "Manca la descrizione del protocollo mittente";
                        if (res.Equals(DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum.DuplicatiMittenteProtocollo.ToString()))
                            this.lbl_messaggio.Text = "Esiste già un documento con il Protocollo Mittente indicato.";
                        if (res.Equals(DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum.DuplicatiMittenteData.ToString()))
                            this.lbl_messaggio.Text = "Esiste già un documento dello stesso mittente nella stessa data.";
                        if (res.Equals(DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum.DuplicatiMittenteOggetto.ToString()))
                            this.lbl_messaggio.Text = "Esiste già un documento inviato da questo mittente e con lo stesso oggetto";
                        if (res.Equals(DocsPAWA.DocsPaWR.EsitoRicercaDuplicatiEnum.ErroreGenerico.ToString()))
                            this.lbl_messaggio.Text = "Si è verificato un errore durante la ricerca dei documenti duplicati";
                    }
                }
                catch (Exception ex)
                {
                    ErrorManager.redirectToErrorPage(this, ex);
                }
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

        private void InitializeComponent()
        {
            this.btn_VisDoc.Click += new System.Web.UI.ImageClickEventHandler(this.btn_VisDoc_Click);
        }

        #endregion

        private void btn_VisDoc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //Verifico se ho visibilità sul documento
            //Se ho visibilità apro la pagina di visualizzazione del documento, altrimenti messaggio
            Session.Add("protocolloEsistente", true);

            try
            {
                // Viene impostato l'id documento e viene verificato se si possiedono i diritti
                // di visibilità sul documento
                string errorMessage = string.Empty;

                int diritti = DocumentManager.verificaACL("D", this.lblIdDocumento.Text, 
                    UserManager.getInfoUtente(this), out errorMessage);

                // Se si possiedono i diritti...
                if (!(diritti == 0))
                {
                    // ...viene caricata la scheda del documento e viene i 
                    // impostato come documento selezionato
                    DocsPAWA.DocsPaWR.SchedaDocumento schedaSel = DocumentManager.getDettaglioDocumentoNoDataVista(
                            this, null, lblIdDocumento.Text);

                    if (schedaSel != null)
                    {
                        DocumentManager.setDocumentoSelezionato(this, schedaSel);
                        FileManager.setSelectedFile(this, schedaSel.documenti[0], false);
                        ClientScript.RegisterStartupScript(this.GetType(), "lanciaVIs", "loadVisualizzatoreDocModal('" + Session.SessionID + "','" + "" + "','" + lblIdDocumento.Text + "');", true);
                    }
                }
                else
                    // ...altrimenti viene visualizzato un messaggio di avviso
                    ClientScript.RegisterStartupScript(this.GetType(), "avviso",
                        "alert(\"Non si possiedono i diritti per la visualizzazione di questo documento\");", true);

            }
            catch (Exception ex)
            {
                Session["ErrorManager.error"] = ex.ToString() + "\n" + ex.StackTrace;
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }
    }
}
