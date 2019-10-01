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

namespace DocsPAWA.AdminTool.Gestione_DiagrammiStato
{
    public partial class GestioneStato : System.Web.UI.Page
    {
        private bool inModifica = false;

        private DocsPAWA.DocsPaWR.Stato st = new DocsPAWA.DocsPaWR.Stato();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["statoDaModificare"] != null)
            {
                st = (DocsPAWA.DocsPaWR.Stato)Session["statoDaModificare"];
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
                InitializeComboProcessiFirma();
            }
        }

        private void InitializeComboProcessiFirma()
        {
            if (!string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_LIBRO_FIRMA")) && DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_LIBRO_FIRMA").Equals("1"))
                this.PnlProcessiFirma.Visible = true;

            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = DocsPAWA.Utils.getIdAmmByCod(codiceAmministrazione, this);

            DocsPaWR.DocsPaWebService ws = DocsPAWA.ProxyManager.getWS();
            DocsPaWR.ProcessoFirma[] listaProcessi = ws.GetProcessiDiFirmaByIdAmm(idAmministrazione);
            if (listaProcessi != null && listaProcessi.Length > 0)
            {
                this.DdlProcessoFirma.Items.Clear();
                this.DdlProcessoFirma.Items.Add(new ListItem(""));
                ListItem item;
                foreach (DocsPaWR.ProcessoFirma p in listaProcessi)
                {
                    item = new ListItem();
                    item.Value = p.idProcesso;
                    item.Text = p.IsProcessModel ? p.nome + " [MODELLO]" : p.nome;
                    this.DdlProcessoFirma.Items.Add(item);
                }

                if (!string.IsNullOrEmpty(st.ID_PROCESSO_FIRMA))
                {
                    this.DdlProcessoFirma.SelectedValue = st.ID_PROCESSO_FIRMA;
                }
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
                DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"];
                if (dg.STATI != null)
                {
                    for (int i = 0; i < dg.STATI.Length; i++)
                    {
                        DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)dg.STATI[i];
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
                DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"];
                if (dg.STATI != null)
                {
                    for (int i = 0; i < dg.STATI.Length; i++)
                    {
                        if (nomeVecchio == ((DocsPAWA.DocsPaWR.Stato)dg.STATI[i]).DESCRIZIONE)
                        {
                            ((DocsPAWA.DocsPaWR.Stato)dg.STATI[i]).DESCRIZIONE = nomeNuovo;
                            ((DocsPAWA.DocsPaWR.Stato)dg.STATI[i]).STATO_INIZIALE = cb_statoIniziale.Checked;
                            ((DocsPAWA.DocsPaWR.Stato)dg.STATI[i]).CONVERSIONE_PDF = cb_convertiInPdf.Checked;
                            ((DocsPAWA.DocsPaWR.Stato)dg.STATI[i]).STATO_CONSOLIDAMENTO = (DocsPaWR.DocumentConsolidationStateEnum) Enum.Parse(typeof(DocsPaWR.DocumentConsolidationStateEnum), cb_consolidamento.SelectedValue, true);
                            ((DocsPAWA.DocsPaWR.Stato)dg.STATI[i]).NON_RICERCABILE = cb_statoNonRicercabile.Checked;
                            ((DocsPAWA.DocsPaWR.Stato)dg.STATI[i]).STATO_SISTEMA = cbxTransactionSystem.Checked;
                            ((DocsPAWA.DocsPaWR.Stato)dg.STATI[i]).ID_PROCESSO_FIRMA = this.DdlProcessoFirma.SelectedValue;
                        }
                        
                        if (dg.PASSI != null)
                        {
                            for (int j = 0; j < dg.PASSI.Length; j++)
                            {
                                DocsPaWR.Passo step = (DocsPAWA.DocsPaWR.Passo)dg.PASSI[j];
                                if (nomeVecchio == step.STATO_PADRE.DESCRIZIONE)
                                    ((DocsPAWA.DocsPaWR.Passo)dg.PASSI[j]).STATO_PADRE.DESCRIZIONE = nomeNuovo;

                                if (step.SUCCESSIVI != null)
                                {
                                    for (int k = 0; k < step.SUCCESSIVI.Length; k++)
                                    {
                                        if (nomeVecchio == ((DocsPAWA.DocsPaWR.Stato)step.SUCCESSIVI[k]).DESCRIZIONE)
                                            ((DocsPAWA.DocsPaWR.Stato)((DocsPAWA.DocsPaWR.Passo)dg.PASSI[j]).SUCCESSIVI[k]).DESCRIZIONE = nomeNuovo;
                                    }
                                }
                            }
                        }
                    }
                }
                Session.Add("DiagrammaStato", dg);
            }
        }

        private void aggiungiStato(DocsPAWA.DocsPaWR.Stato st)
        {
            if (Session["DiagrammaStato"] != null)
            {
                ArrayList stati;
                if (((DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"]).STATI != null)
                {
                    stati = new ArrayList(((DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"]).STATI);
                    stati.Add(st);
                }
                else
                {
                    stati = new ArrayList();
                    stati.Add(st);
                }
                ((DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"]).STATI = new DocsPAWA.DocsPaWR.Stato[stati.Count];
                stati.CopyTo(((DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"]).STATI);
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
            DocsPaWR.DocsPaWebService ws = DocsPAWA.ProxyManager.getWS();

            this.pnl_consolidamento.Visible = ws.IsConsolidationEnabled();

            if (this.pnl_consolidamento.Visible)
            {
                this.cb_consolidamento.SelectedValue = ((int) st.STATO_CONSOLIDAMENTO).ToString();
            }
        }

        private void controlloCbConversionePdf()
        {
            //Verifico che sia abilitata la conversione pdf lato server
            if (DocsPAWA.Utils.isEnableConversionePdfLatoServer() == "true")
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
