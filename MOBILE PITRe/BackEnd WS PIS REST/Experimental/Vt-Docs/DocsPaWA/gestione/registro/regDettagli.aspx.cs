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

namespace DocsPAWA.gestione.registro
{
	/// <summary>
	/// Summary description for regDettagli.
	/// </summary>
	public class regDettagli : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Panel panel_Det;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected System.Web.UI.WebControls.Label lbl_dataUltProto;
		protected System.Web.UI.WebControls.Label lbl_registro;
		protected System.Web.UI.WebControls.Label lbl_descrizione;
		protected System.Web.UI.WebControls.Label lbl_dataApertura;
		protected System.Web.UI.WebControls.Label lbl_dataChiusura;
		protected System.Web.UI.WebControls.Label lbl_prossimoProtocollo;
		protected System.Web.UI.HtmlControls.HtmlTableCell TD1;
		protected System.Web.UI.WebControls.Label titolo;
        protected System.Web.UI.WebControls.DropDownList ddl_Caselle;

		//my var
		DocsPaWR.Registro registroSel;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			try
			{
				Response.Expires = -1;
				Utils.startUp(this);
                if (!Page.IsPostBack)
                {
                    //registroSel = GestManager.getRegistroSel(this);
                    //if (registroSel != null)
                    //    setDettagli(registroSel);
                    if (Request.QueryString["idReg"] != null && Request.QueryString["idReg"] != string.Empty)
                    {
                        string systemId = Request.QueryString["idReg"].ToString();

                        if (systemId != null)
                        {
                            registroSel = UserManager.getRegistroBySistemId(this, systemId);

                            if (registroSel != null)
                            {
                                setDettagli(registroSel);
                            }
                        }
                    }

                    else
                    {
                        registroSel = GestManager.getRegistroSel(this);
                        if (registroSel != null)
                            setDettagli(registroSel);
                    }
                }
			}
			catch (System.Exception ex)
			{
				ErrorManager.redirect(this, ex);
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

		}
		#endregion

		private void setDettagli(DocsPAWA.DocsPaWR.Registro registro)
		{
			
			this.lbl_registro.Text = registro.codRegistro;
			this.lbl_descrizione.Text = registro.descrizione;
			this.lbl_dataApertura.Text = registro.dataApertura;
			this.lbl_dataChiusura.Text = registro.dataChiusura;
			this.lbl_dataUltProto.Text = registro.dataUltimoProtocollo;
			this.lbl_prossimoProtocollo.Text = registro.ultimoNumeroProtocollo;
			this.panel_Det.Visible = true;
			//modificato da Marco il 20/02/04 su richiesta per salute
			if(registro.stato =="A")
			{
				this.lbl_dataUltProto.Visible = false;
				this.lbl_prossimoProtocollo.Visible = false;
			}
            #region multi casella

            System.Collections.Generic.List<DocsPAWA.DocsPaWR.CasellaRegistro> listCaselle = DocsPAWA.utils.MultiCasellaManager.GetComboRegisterConsult(registro.systemId);
            foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in listCaselle)
            {
                if (!string.IsNullOrEmpty(c.EmailRegistro) && !c.EmailRegistro.Equals("&nbsp;"))
                {
                    System.Text.StringBuilder formatMail = new System.Text.StringBuilder();
                    if (c.Principale.Equals("1"))
                        formatMail.Append("* ");
                    formatMail.Append(c.EmailRegistro);
                    if (!string.IsNullOrEmpty(c.Note))
                    {
                        formatMail.Append(" - ");
                        formatMail.Append(c.Note);
                    }
                    ddl_Caselle.Items.Add(new ListItem(formatMail.ToString(), c.EmailRegistro));
                }
            }

            if (listCaselle.Count == 0)
            {
                ddl_Caselle.Enabled = false;
                ddl_Caselle.Width = new Unit(200);
                return;
            }
            //imposto la casella principale come selezionata
            foreach (ListItem i in ddl_Caselle.Items)
            {
                if (i.Text.Split(new string[] { "*" }, 2, System.StringSplitOptions.None).Length > 1)
                {
                    ddl_Caselle.SelectedValue = i.Value;
                    break;
                }
            }
            if (Session["TipoRegistro"] != null && Session["TipoRegistro"].Equals("disable"))
                ddl_Caselle.Enabled = false;
            else
            {
                //salvo in sessione l'indirizzo della casella correntemente selezionata
                GestManager.setCasellaSel(ddl_Caselle.SelectedValue);
            }
            #endregion            
		}
		private void Textbox1_TextChanged(object sender, System.EventArgs e)
		{
		
		}

        /// <summary>
        /// Aggiorna in sessione la casella di posta selezionata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddl_Caselle_SelectedIndexChanged(object sender, EventArgs e)
        {
            GestManager.setCasellaSel(ddl_Caselle.SelectedValue);
        }
	}
}