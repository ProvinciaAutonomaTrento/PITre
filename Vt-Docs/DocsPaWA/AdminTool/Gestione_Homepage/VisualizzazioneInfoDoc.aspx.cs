using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.Gestione_Homepage
{
    public partial class VisualizzazioneInfoDoc : System.Web.UI.Page
    {
        private DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
            }
        }

        protected void btnCerca_Click(Object sender, EventArgs e)
        {
            string idDocumento = this.idDocumento.Text.Trim();
            string codiceRegistro = this.codiceRegistro.Text.Trim();
            string numProto = this.numProtocollo.Text.Trim();
            string anno = this.anno.Text.Trim();
            if (!string.IsNullOrEmpty(idDocumento) || (!string.IsNullOrEmpty(codiceRegistro) && !string.IsNullOrEmpty(numProto) && !string.IsNullOrEmpty(anno)))
            {
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                DocsPAWA.DocsPaWR.VisualizzaStatoDoc info = ws.GetInfoDocument(idDocumento, numProto, anno, idAmm, codiceRegistro);
                if (info != null)
                {
                    this.LblSegnatura.Text = info.segnatura;
                    if (!string.IsNullOrEmpty(info.descrizioneTipologia))
                    {
                        this.PnlTipologia.Visible = true;
                        this.LblTipologia.Text = info.descrizioneTipologia;
                    }
                    else
                    {
                        this.PnlTipologia.Visible = false;
                    }

                    this.LblRuoloProtocollatore.Text = info.ruoloProtocollatore;
                    this.LblUtenteProtocollatore.Text = info.utenteProtocollatore;
                    this.LblUoProtocollazione.Text = info.uoProtocollatore;

                    this.LblFascicoliList.Text = string.Empty;
                    if (info.fascicoliDocumento != null && info.fascicoliDocumento.Count() > 0)
                    {
                        this.LblFascicolo.Text = "Il documento è inserito nei seguenti fascicoli: ";
                        foreach (string fasc in info.fascicoliDocumento)
                        {
                            this.LblFascicoliList.Text += "<li> " + fasc + "</li>";
                        }
                    }
                    else
                    {
                        this.LblFascicolo.Text = "Il documento non è inserito in nessun fascicolo ";
                    }

                    string trasmMessage = string.Empty;
                    string listaTrasmissioni = string.Empty;
                    listaTrasmissioni = string.Empty;
                    if (info.trasmissioniDocumento != null && info.trasmissioniDocumento.Count() > 0)
                    {
                        trasmMessage = "Sono state effettuate le seguenti trasmissioni: ";
                        foreach (string trasm in info.trasmissioniDocumento)
                        {
                            listaTrasmissioni += "<li> " + trasm + "</li>";
                        }
                        this.LblTrasmissioniList.Text = listaTrasmissioni;
                    }
                    else
                    {
                        trasmMessage = "Non ci sono trasmissioni a destinatari interni.";
                    }
                    this.LblTrasmissione.Text = trasmMessage;

                    if (info.spedizioniDocumento)
                        LblSpedizioni.Text = "Il documento è stato spedito";
                    else
                        LblSpedizioni.Text = "Il documento non è stato spedito";

                    this.pnlResult.Visible = true;
                    this.UpPnlResult.Update();
                }
                else
                {
                    string s = "<script language='javascript'>alert('Attenzione! Verificare di aver inserito correttamente i dati.');</script>";
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "error", s, false);
                    return;
                }
            }
            else
            {
                string s = "<script language='javascript'>alert('Attenzione! Si deve inserire id del documento oppure il numero di protocollo e anno di protocollazione.');</script>";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "error", s, false);
                return;
            }
        }
    }
}