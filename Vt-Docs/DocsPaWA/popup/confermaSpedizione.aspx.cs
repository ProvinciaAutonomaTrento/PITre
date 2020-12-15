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

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for visualizzaLog.
	/// </summary>
	public class confermaSpedizione : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label Label2;
		protected DocsPAWA.dataSet.DataSetStoriaObj dataSetStoriaObj1;
		protected System.Web.UI.WebControls.Label lb_doc_con_documenti;
        protected System.Web.UI.WebControls.Label lb_doc_spedito;
        protected System.Web.UI.WebControls.Label lbl_doc_rf;

        string conferma;
        protected System.Web.UI.WebControls.Button Btn_ok;
        protected System.Web.UI.WebControls.Button Btn_annulla;
        protected System.Web.UI.WebControls.DropDownList ddl_regRF;
        protected System.Web.UI.WebControls.Panel pnl_regRF;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
            conferma = Request.QueryString["conferma"];

            if (!IsPostBack)
            {
                if (!conferma.Equals("0^1^0") && conferma.Contains("^"))
                {
                    string[] valori = conferma.Split('^');
                    if (valori.Length == 3)
                    {
                        if (valori[0].Equals("1"))
                        {
                            // l'utente può selezionare registro o RF da utilizzare
                            this.pnl_regRF.Visible = true;
                            this.ddl_regRF.Visible = true;
                            this.lbl_doc_rf.Visible = true;
                            this.lbl_doc_rf.Text = "Selezionare registro o RF con cui effettuare la spedizione";

                            CaricaComboRegistri(this.ddl_regRF);
                        }

                        if (valori[1].Equals("0"))
                        {
                            this.lb_doc_con_documenti.Visible = true;
                            this.lb_doc_con_documenti.Text = "Non risulta acquisito un documento elettronico: confermi la spedizione?";
                        }

                        if (valori[2].Equals("1"))
                        {
                            this.lb_doc_spedito.Text = "Il documento risulta già spedito: confermi la spedizione?";
                            this.lb_doc_spedito.Visible = true;
                        }
                    }
                }
            }
        }

        private void CaricaComboRegistri(DropDownList ddl)
        {
            DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato();
            //verifico se il registro ha RF associati
            //DocsPaWR.Registro[] listaRF = DocsPAWA.UserManager.getListRFByIdRegistro(this.Page, schedaDocumento.registro.systemId);
            DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(this, "1", schedaDocumento.registro.systemId);
            if (listaRF != null && listaRF.Length > 0)
            {
                if (listaRF.Length == 1)
                {
                    ListItem li = new ListItem();
                    li.Value = listaRF[0].systemId;
                    li.Text = listaRF[0].codRegistro + " - " + listaRF[0].descrizione;
                    this.ddl_regRF.Items.Add(li);
                    ListItem lit2 = new ListItem();
                    lit2.Value = schedaDocumento.registro.systemId;
                    lit2.Text = schedaDocumento.registro.codRegistro + " - " + schedaDocumento.registro.descrizione;
                    this.ddl_regRF.Items.Add(lit2);
                }
                else
                {
                    ListItem lit = new ListItem();
                    lit.Value = "";
                    lit.Text = " ";
                    this.ddl_regRF.Items.Add(lit);
                    ListItem lit2 = new ListItem();
                    lit2.Value = schedaDocumento.registro.systemId;
                    lit2.Text = schedaDocumento.registro.codRegistro + " - " + schedaDocumento.registro.descrizione;
                    this.ddl_regRF.Items.Add(lit2);

                    foreach (DocsPaWR.Registro regis in listaRF)
                    {
                        ListItem li = new ListItem();
                        li.Value = regis.systemId;
                        li.Text = regis.codRegistro + " - " + regis.descrizione;
                        this.ddl_regRF.Items.Add(li);
                    }
                }
            }
            else
            {
                this.pnl_regRF.Visible = false;
                this.ddl_regRF.Visible = false;
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
            this.Btn_ok.Click += new System.EventHandler(this.Btn_ok_Click);
            this.Btn_annulla.Click += new EventHandler(Btn_annulla_Click);
		}

		#endregion

        private void Btn_ok_Click(object sender, System.EventArgs e)
        {
            if (this.ddl_regRF.Visible)
            {
                if (this.ddl_regRF.SelectedValue.Equals(""))
                {
                    Response.Write("<script>alert('Selezionare un registro o un RF come mittente');</script>");
                }
                else
                    Response.Write("<script>window.returnValue='OK^" + this.ddl_regRF.SelectedValue + "'; window.close();</script>");
            }
            else
                Response.Write("<script>window.returnValue='OK^OK'; window.close();</script>");
        }

        private void Btn_annulla_Click(object sender, EventArgs e)
        {
            Response.Write("<script>window.returnValue='KO^KO'; window.close();</script>");
        }
    }
}
