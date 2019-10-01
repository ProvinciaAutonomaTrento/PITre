using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.DocsPaWR;
using System.Collections;
using System.Data;

namespace SAAdminTool.AdminTool.Gestione_ProfDinamica
{
    public partial class GestioneContatore : System.Web.UI.Page
    {
        protected OggettoCustom oggettoCustom = null;
        protected SAAdminTool.DocsPaWR.Contatore[] contatori = null;
        protected string idOggetto = string.Empty;
        protected string idTemplate = string.Empty;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["IdOggCustom"] != null && Request.QueryString["IdTemplate"] != null)
            {
                idOggetto = Request.QueryString["IdOggCustom"];
                idTemplate = Request.QueryString["IdTemplate"];
                if (!string.IsNullOrEmpty(idOggetto) && !string.IsNullOrEmpty(idTemplate))
                {
                    oggettoCustom = ProfilazioneDocManager.getOggettoById(idOggetto, this);
                    lbl_titolo.Text = oggettoCustom.DESCRIZIONE;
                    contatori = ProfilazioneDocManager.GetValuesContatoriDoc(this, oggettoCustom);
                    msg_Elimina.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_Elimina_GetMessageBoxResponse);
                }
            }

            if (!IsPostBack)
            {
                caricaDgContatori();
                impostaVisualizzazione();
            }
        }

        private void caricaDgContatori()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SYSTEM_ID");
            dt.Columns.Add("ID_OGG");
            dt.Columns.Add("ID_TIPOLOGIA");
            dt.Columns.Add("ID_AOO");
            dt.Columns.Add("ID_RF");
            dt.Columns.Add("ABILITATO");
            dt.Columns.Add("ANNO");
            dt.Columns.Add("CODICE_RF_AOO");
            dt.Columns.Add("DESC_RF_AOO");
            dt.Columns.Add("VALORE");
            dt.Columns.Add("VALORE_SC");

            if (contatori != null && contatori.Length > 0)
            {
                foreach (Contatore contatore in contatori)
                {
                    DataRow rw = dt.NewRow();
                    rw[0] = contatore.SYSTEM_ID;
                    rw[1] = contatore.ID_OGG;
                    rw[2] = contatore.ID_TIPOLOGIA;
                    rw[3] = contatore.ID_AOO;
                    rw[4] = contatore.ID_RF;
                    rw[5] = contatore.ABILITATO;
                    rw[6] = contatore.ANNO;
                    rw[7] = contatore.CODICE_RF_AOO;
                    rw[8] = contatore.DESC_RF_AOO;
                    rw[9] = contatore.VALORE;
                    rw[10] = contatore.VALORE_SC;

                    dt.Rows.Add(rw);
                }
            }

            dt.AcceptChanges();
            dgContatori.DataSource = dt;
            dgContatori.DataBind();            
        }

        protected void dgContatori_OnItemCommand(object sender, DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Edit":
                    this.dgContatori.EditItemIndex = e.Item.ItemIndex;
                    break;

                case "Cancel":
                    this.dgContatori.EditItemIndex = -1;
                    break;

                case "Update":
                    Contatore contatoreSelezionato = contatori[e.Item.ItemIndex];
                    string contatore = ((TextBox)dgContatori.Items[e.Item.ItemIndex].Cells[9].FindControl("txt_valore")).Text;
                    string sottoContatore = ((TextBox)dgContatori.Items[e.Item.ItemIndex].Cells[10].FindControl("txt_valoreSc")).Text;

                    if (string.IsNullOrEmpty(contatore) || string.IsNullOrEmpty(sottoContatore))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alertValoriContatore", "alert('Inserire valori validi per i contatori.');", true);
                    }
                    else
                    {
                        if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("CONTATORESOTTOCONTATORE"))
                        {
                            if (!string.IsNullOrEmpty(oggettoCustom.MODULO_SOTTOCONTATORE) && Convert.ToInt32(sottoContatore) > Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE))
                            {
                                ((TextBox)dgContatori.Items[e.Item.ItemIndex].Cells[10].FindControl("txt_valoreSc")).Text = contatoreSelezionato.VALORE_SC;
                                ClientScript.RegisterStartupScript(this.GetType(), "alertValoriContatore", "alert('Il valore del sottocontatore deve essere minore del modulo del campo.');", true);                                
                            }
                            else
                            {
                                contatoreSelezionato.VALORE = contatore;
                                contatoreSelezionato.VALORE_SC = sottoContatore;
                                ProfilazioneDocManager.SetValuesContatoreDoc(this, contatoreSelezionato);
                            }
                        }
                        else
                        {
                            contatoreSelezionato.VALORE = contatore;
                            contatoreSelezionato.VALORE_SC = sottoContatore;
                            ProfilazioneDocManager.SetValuesContatoreDoc(this, contatoreSelezionato);
                        }
                        
                        this.dgContatori.EditItemIndex = -1;
                    }
                    break;  
                case "Delete":
                    dgContatori.SelectedIndex = e.Item.ItemIndex;
                    msg_Elimina.Confirm("Eliminare il contatore ?");
                    break;
            }

            caricaDgContatori();
        }

        private void impostaVisualizzazione()
        {
            if (!oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("CONTATORESOTTOCONTATORE"))
                dgContatori.Columns[10].Visible = false;

            switch (oggettoCustom.TIPO_CONTATORE)
            {
                case "T":
                    pnl_addContatori_AOO.Visible = false;
                    pnl_addContatori_RF.Visible = false;
                    if(dgContatori.Items.Count == 0)
                        pnl_addContatori_T.Visible = true;
                    else
                        pnl_addContatori_T.Visible = false;
                    break;
                case "A":
                    pnl_addContatori_T.Visible = false;
                    pnl_addContatori_AOO.Visible = true;
                    pnl_addContatori_RF.Visible = false;

                    ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_systemIdAoo")).Value = string.Empty;
                    txt_codAoo.Text = string.Empty;
                    txt_descAoo.Text = string.Empty;
                    break;
                case "R":
                    pnl_addContatori_T.Visible = false;
                    pnl_addContatori_AOO.Visible = false;
                    pnl_addContatori_RF.Visible = true;

                    ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_systemIdRF")).Value = string.Empty;
                    txt_codRF.Text = string.Empty;
                    txt_descRF.Text = string.Empty;
                    break;
            }
        }

        private void msg_Elimina_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                int elSelezionato = dgContatori.SelectedIndex;
                Contatore contatore = new Contatore();
                contatore.SYSTEM_ID = dgContatori.Items[elSelezionato].Cells[0].Text;

                ProfilazioneDocManager.DeleteValueContatoreDoc(this, contatore);

                contatori = ProfilazioneDocManager.GetValuesContatoriDoc(this, oggettoCustom);
                caricaDgContatori();
                impostaVisualizzazione();
            }

            dgContatori.SelectedIndex = -1;
        }

        protected void txt_codAoo_OnTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_codAoo.Text))
            {
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codAmm = amministrazione[0];
                OrgRegistro[] registri = UserManager.getRegistriByCodAmm(codAmm, "0");

                OrgRegistro orgRegistro = registri.Where(obj => obj.Codice.ToUpper().Equals(txt_codAoo.Text.ToUpper())).FirstOrDefault();
                if (orgRegistro != null)
                {
                    ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_systemIdAoo")).Value = orgRegistro.IDRegistro;
                    txt_descAoo.Text = orgRegistro.Descrizione;
                }
                else
                {
                    ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_systemIdAoo")).Value = string.Empty;
                    txt_codAoo.Text = string.Empty;
                    txt_descAoo.Text = string.Empty;
                    ClientScript.RegisterStartupScript(this.GetType(), "alertNoAoo", "alert('Nessun registro trovato con il codice inserito.');", true);
                }
            }
        }

        protected void txt_codRF_OnTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_codRF.Text))
            {
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codAmm = amministrazione[0];
                OrgRegistro[] rf = UserManager.getRegistriByCodAmm(codAmm, "1");

                OrgRegistro orgRegistro = rf.Where(obj => obj.Codice.ToUpper().Equals(txt_codRF.Text.ToUpper())).FirstOrDefault();
                if (orgRegistro != null)
                {
                    ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_systemIdRF")).Value = orgRegistro.IDRegistro;
                    txt_descRF.Text = orgRegistro.Descrizione;
                }
                else
                {
                    ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_systemIdRF")).Value = string.Empty;
                    txt_codRF.Text = string.Empty;
                    txt_descRF.Text = string.Empty;
                    ClientScript.RegisterStartupScript(this.GetType(), "alertNoRF", "alert('Nessun RF trovato con il codice inserito.');", true);
                }
            }

        }

        protected void btn_addContatore_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            switch (oggettoCustom.TIPO_CONTATORE)
            {
                case "T":
                    if (dgContatori.Items.Count == 0)
                    {
                        Contatore contatore = new Contatore();
                        contatore.ID_AOO = "0";
                        contatore.ID_RF = "0";
                        contatore.ANNO = System.DateTime.Now.Year.ToString();
                        contatore.ID_OGG = idOggetto;
                        contatore.ID_TIPOLOGIA = idTemplate;
                        contatore.ABILITATO = "1";
                        contatore.VALORE = "0";
                        contatore.VALORE_SC = "0";

                        ProfilazioneDocManager.InsertValuesContatoreDoc(this, contatore);

                        contatori = ProfilazioneDocManager.GetValuesContatoriDoc(this, oggettoCustom);
                        caricaDgContatori();
                        impostaVisualizzazione();
                    }
                    break;
                case "A":
                    if (!string.IsNullOrEmpty(((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_systemIdAoo")).Value))
                    {
                        Contatore contatore = new Contatore();
                        contatore.ID_AOO = ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_systemIdAoo")).Value;
                        contatore.ID_RF = "0";
                        contatore.ANNO = System.DateTime.Now.Year.ToString();
                        contatore.ID_OGG = idOggetto;
                        contatore.ID_TIPOLOGIA = idTemplate;
                        contatore.ABILITATO = "1";
                        contatore.VALORE = "0";
                        contatore.VALORE_SC = "0";

                        ProfilazioneDocManager.InsertValuesContatoreDoc(this, contatore);

                        contatori = ProfilazioneDocManager.GetValuesContatoriDoc(this, oggettoCustom);
                        caricaDgContatori();
                        impostaVisualizzazione();
                    }                    
                    break;
                case "R":
                    if (!string.IsNullOrEmpty(((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_systemIdRF")).Value))
                    {
                        Contatore contatore = new Contatore();
                        contatore.ID_RF = ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_systemIdRF")).Value;
                        contatore.ID_AOO = "0";
                        contatore.ANNO = System.DateTime.Now.Year.ToString();
                        contatore.ID_OGG = idOggetto;
                        contatore.ID_TIPOLOGIA = idTemplate;
                        contatore.ABILITATO = "1";
                        contatore.VALORE = "0";
                        contatore.VALORE_SC = "0";

                        ProfilazioneDocManager.InsertValuesContatoreDoc(this, contatore);

                        contatori = ProfilazioneDocManager.GetValuesContatoriDoc(this, oggettoCustom);
                        caricaDgContatori();
                        impostaVisualizzazione();
                    }
                    break;
            }
        }
    }
}