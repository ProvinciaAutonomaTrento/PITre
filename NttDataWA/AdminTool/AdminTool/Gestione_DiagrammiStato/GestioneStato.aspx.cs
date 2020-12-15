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

namespace SAAdminTool.AdminTool.Gestione_DiagrammiStato
{
    public partial class GestioneStato : System.Web.UI.Page
    {
        private bool inModifica = false;

        private SAAdminTool.DocsPaWR.Stato st = new SAAdminTool.DocsPaWR.Stato();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["statoDaModificare"] != null)
            {
                st = (SAAdminTool.DocsPaWR.Stato)Session["statoDaModificare"];
                if (!IsPostBack)
                {
                    txt_descrizioneStato.Text = st.DESCRIZIONE;
                    cb_statoIniziale.Checked = st.STATO_INIZIALE;
                    cbxTransactionSystem.Checked = st.STATO_SISTEMA;
                }
                inModifica = true;
            }
            else
            {
                inModifica = false;
            }

            if (!IsPostBack)
            {
                controlloCbConversionePdf();
                controlloCbConsolidamento();
                controlloCbNonRicercabile();
                SetFocus(txt_descrizioneStato);
            }
        }

        protected void btn_conferma_Click(object sender, EventArgs e)
        {
            //Modifica di uno stato esistente
            if (inModifica)
            {
                modificaStato(txt_descrizioneStato.Text, st.DESCRIZIONE);
            }
            //Inserimento di un nuovo stato
            else
            {
                //Controllo l'unicità dello stato
                if (verificaEistenzaStato(txt_descrizioneStato.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "statoDuplicato", "alert('Nome stato già presente !');", true);
                    return;
                }

                st.DESCRIZIONE = txt_descrizioneStato.Text;
                st.STATO_INIZIALE = cb_statoIniziale.Checked;
                st.CONVERSIONE_PDF = cb_convertiInPdf.Checked;
                st.STATO_CONSOLIDAMENTO = (DocsPaWR.DocumentConsolidationStateEnum) Enum.Parse(typeof(DocsPaWR.DocumentConsolidationStateEnum), cb_consolidamento.SelectedValue, true);
                st.NON_RICERCABILE = cb_statoNonRicercabile.Checked;
                aggiungiStato(st);
            }

            Session.Add("statoModificatoSalvato", true);
            ClientScript.RegisterStartupScript(this.GetType(), "chiudiFinestra", "window.close();", true);
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chiudiFinestra", "window.close();", true);
        }

        private bool verificaEistenzaStato(string nomeStato)
        {
            if (Session["DiagrammaStato"] != null)
            {
                DocsPaWR.DiagrammaStato dg = (SAAdminTool.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"];
                if (dg.STATI != null)
                {
                    for (int i = 0; i < dg.STATI.Length; i++)
                    {
                        DocsPaWR.Stato st = (SAAdminTool.DocsPaWR.Stato)dg.STATI[i];
                        if (st.DESCRIZIONE == txt_descrizioneStato.Text)
                            return true;
                    }
                }
            }
            return false;
        }

        private void modificaStato(string nomeNuovo, string nomeVecchio)
        {
            if (Session["DiagrammaStato"] != null)
            {
                DocsPaWR.DiagrammaStato dg = (SAAdminTool.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"];
                if (dg.STATI != null)
                {
                    for (int i = 0; i < dg.STATI.Length; i++)
                    {
                        if (nomeVecchio == ((SAAdminTool.DocsPaWR.Stato)dg.STATI[i]).DESCRIZIONE)
                        {
                            ((SAAdminTool.DocsPaWR.Stato)dg.STATI[i]).DESCRIZIONE = nomeNuovo;
                            ((SAAdminTool.DocsPaWR.Stato)dg.STATI[i]).STATO_INIZIALE = cb_statoIniziale.Checked;
                            ((SAAdminTool.DocsPaWR.Stato)dg.STATI[i]).CONVERSIONE_PDF = cb_convertiInPdf.Checked;
                            ((SAAdminTool.DocsPaWR.Stato)dg.STATI[i]).STATO_CONSOLIDAMENTO = (DocsPaWR.DocumentConsolidationStateEnum) Enum.Parse(typeof(DocsPaWR.DocumentConsolidationStateEnum), cb_consolidamento.SelectedValue, true);
                            ((SAAdminTool.DocsPaWR.Stato)dg.STATI[i]).NON_RICERCABILE = cb_statoNonRicercabile.Checked;
                            ((SAAdminTool.DocsPaWR.Stato)dg.STATI[i]).STATO_SISTEMA = cbxTransactionSystem.Checked;
                        }
                        
                        if (dg.PASSI != null)
                        {
                            for (int j = 0; j < dg.PASSI.Length; j++)
                            {
                                DocsPaWR.Passo step = (SAAdminTool.DocsPaWR.Passo)dg.PASSI[j];
                                if (nomeVecchio == step.STATO_PADRE.DESCRIZIONE)
                                    ((SAAdminTool.DocsPaWR.Passo)dg.PASSI[j]).STATO_PADRE.DESCRIZIONE = nomeNuovo;

                                if (step.SUCCESSIVI != null)
                                {
                                    for (int k = 0; k < step.SUCCESSIVI.Length; k++)
                                    {
                                        if (nomeVecchio == ((SAAdminTool.DocsPaWR.Stato)step.SUCCESSIVI[k]).DESCRIZIONE)
                                            ((SAAdminTool.DocsPaWR.Stato)((SAAdminTool.DocsPaWR.Passo)dg.PASSI[j]).SUCCESSIVI[k]).DESCRIZIONE = nomeNuovo;
                                    }
                                }
                            }
                        }
                    }
                }
                Session.Add("DiagrammaStato", dg);
            }
        }

        private void aggiungiStato(SAAdminTool.DocsPaWR.Stato st)
        {
            if (Session["DiagrammaStato"] != null)
            {
                ArrayList stati;
                if (((SAAdminTool.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"]).STATI != null)
                {
                    stati = new ArrayList(((SAAdminTool.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"]).STATI);
                    stati.Add(st);
                }
                else
                {
                    stati = new ArrayList();
                    stati.Add(st);
                }
                ((SAAdminTool.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"]).STATI = new SAAdminTool.DocsPaWR.Stato[stati.Count];
                stati.CopyTo(((SAAdminTool.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"]).STATI);
            }
        }

        protected void cb_statoIniziale_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_statoIniziale.Checked)
            {
                cb_convertiInPdf.Checked = false;
                cb_convertiInPdf.Enabled = false;
            }
            else
            {
                cb_convertiInPdf.Enabled = true;
            }
        }

        private void controlloCbConsolidamento()
        {
            DocsPaWR.DocsPaWebService ws = SAAdminTool.ProxyManager.getWS();

            this.pnl_consolidamento.Visible = ws.IsConsolidationEnabled();

            if (this.pnl_consolidamento.Visible)
            {
                this.cb_consolidamento.SelectedValue = ((int) st.STATO_CONSOLIDAMENTO).ToString();
            }
        }

        private void controlloCbConversionePdf()
        {
            //Verifico che sia abilitata la conversione pdf lato server
            if (SAAdminTool.Utils.isEnableConversionePdfLatoServer() == "true")
            {
                if (Session["statoDaModificare"] != null)
                {
                    //Rendo disponibile l'opzione per la conversione pdf lato server
                    pnl_convertiInPdf.Visible = true;
                    cb_convertiInPdf.Checked = st.CONVERSIONE_PDF;

                    if (cb_statoIniziale.Checked)
                    {
                        cb_convertiInPdf.Checked = false;
                        cb_convertiInPdf.Enabled = false;
                    }
                }
                else
                {
                    pnl_convertiInPdf.Visible = true;
                    cb_convertiInPdf.Checked = false;
                    cb_convertiInPdf.Enabled = true;
                }
            }
        }

        private void controlloCbNonRicercabile()
        {
            if (Session["statoDaModificare"] != null)
            {
                cb_statoNonRicercabile.Checked = st.NON_RICERCABILE;
            }
            else
            {
                cb_statoNonRicercabile.Checked = false;                
            }
        }
    }
}
