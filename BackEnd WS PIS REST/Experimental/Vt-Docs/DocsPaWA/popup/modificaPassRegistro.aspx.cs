using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{
    public partial class modificaPassRegistro : System.Web.UI.Page
    {
        protected DocsPAWA.DocsPaWR.OrgRegistro registro;
        protected DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                registro = ws.AmmGetRegistro(GestManager.getRegistroSel().systemId);
                Session.Add("regToModify", registro);
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
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            this.btn_chiudi.Click += new EventHandler(btn_chiudi_Click);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        private void btn_ok_Click(object sender, System.EventArgs e)
        {
            if (Session["regToModify"] != null)
            {
                var orgRegistro = ((OrgRegistro)Session["regToModify"]);

                if (checkField(orgRegistro.Mail.Password))
                {

                    orgRegistro.Mail.Password = txtNuovaPass.Text;
                    DocsPAWA.DocsPaWR.ValidationResultInfo result = null;
                    result = this.UpdateRegistro(ref orgRegistro);
                    if (result.Value)
                    {
                        string funct = "alert('Modifica Password avvenuta con successo');window.close();";
                        Session.Remove("regToModify");
                        Response.Write("<script> " + funct + "</script>");
                    }

                }

            }
        }

        private void btn_chiudi_Click(object sender, EventArgs e)
        {
            Session.Remove("regToModify");
            string funct = "window.close();";
            Response.Write("<script> " + funct + "</script>");
        }

        /// <summary>
        /// Aggiornamento registro
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.ValidationResultInfo UpdateRegistro(ref DocsPAWA.DocsPaWR.OrgRegistro registro)
        {
            return ws.AmmUpdateRegistro(ref registro);


        }

        private bool checkField(string password)
        {
            if (txtOldPass.Text.Equals(""))
            {
                RegisterStartupScript("checkOldPass", "<script language=\"javascript\">alert ('Campo Vecchia Password obbligatorio');</script>");
                return false;
            }
            if (txtNuovaPass.Text.Equals(""))
            {
                RegisterStartupScript("checkNewPass", "<script language=\"javascript\">alert ('Campo Nuova Password obbligatorio');</script>");
                return false;
            }
            if (!txtOldPass.Text.Equals(password))
            {
                RegisterStartupScript("checkOldPass", "<script language=\"javascript\">alert ('Vecchia Password errata');</script>");
                return false;
            }
            if (!txtNuovaPass.Text.Equals(txtConfermaPass.Text))
            {
                RegisterStartupScript("checkPass", "<script language=\"javascript\">alert ('Conferma Password fallita');</script>");
                return false;
            }
            return true;
        }
    }
}