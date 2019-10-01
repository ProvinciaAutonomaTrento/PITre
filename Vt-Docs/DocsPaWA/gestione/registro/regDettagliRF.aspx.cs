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
using DocsPaUtils.Security;

namespace DocsPAWA.gestione.registro
{
    public partial class regDettagliRF : System.Web.UI.Page
    {
        DocsPaWR.Registro elemento;
 
        protected void Page_Load(object sender, EventArgs e)
        {
   
            try
            {
                Response.Expires = -1;
                Utils.startUp(this);
                if (!Page.IsPostBack)
                {
                    elemento = GestManager.getRegistroSel(this);
                    if (elemento != null)
                        setDettagli(elemento);
                }
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        protected void setDettagli(DocsPAWA.DocsPaWR.Registro rf)
        {
            this.lbl_registro.Text = rf.codRegistro;
            this.lbl_descrizione.Text = rf.descrizione;
            DocsPaWR.Registro aooColl = UserManager.getRegistroBySistemId(this, rf.idAOOCollegata);
            //codice Aoo Collegata
            this.lbl_AooColl.Text = aooColl.codRegistro;
            //descrizione Aoo Collegata
            this.lbl_DescAooColl.Text = aooColl.descrizione;
            this.panel_Det.Visible = true;
            #region multi casella
            
            System.Collections.Generic.List<DocsPAWA.DocsPaWR.CasellaRegistro> listCaselle = DocsPAWA.utils.MultiCasellaManager.GetComboRegisterConsult(GestManager.getRegistroSel().systemId);
            foreach(DocsPAWA.DocsPaWR.CasellaRegistro c in listCaselle)
            {
                System.Text.StringBuilder formatMail = new System.Text.StringBuilder();
                if (c.Principale.Equals("1"))
                    formatMail.Append("* ");
                formatMail.Append(c.EmailRegistro);
                if(!string.IsNullOrEmpty(c.Note))
                {
                    formatMail.Append(" - ");
                    formatMail.Append(c.Note);
                }
                ddl_Caselle.Items.Add(new ListItem(formatMail.ToString(), c.EmailRegistro));
                
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

            //salvo in sessione l'indirizzo della casella correntemente selezionata
            GestManager.setCasellaSel(ddl_Caselle.SelectedValue);
            #endregion            
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