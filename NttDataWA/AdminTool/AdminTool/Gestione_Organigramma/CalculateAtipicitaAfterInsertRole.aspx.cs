using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Amministrazione.Manager;
using SAAdminTool.DocsPaWR;
using SAAdminTool.SiteNavigation;

namespace SAAdminTool.AdminTool.Gestione_Organigramma
{
    public partial class CalculateAtipicitaAfterInsertRole : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                this.btnClose.OnClientClick = String.Format("window.returnValue='{0}';window.close();", Request["ret"]);
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            OrganigrammaManager theManager = new OrganigrammaManager();

            // Inserimento del ruolo 
            theManager.InsNuovoRuolo(Role, this.calculateAtipicitaOptions.CalculateAtipicita());

            String scriptForClose = String.Empty;

            // Se il risultato è negativo viene visualizzato il messaggio
            if (theManager.getEsitoOperazione().Codice != 0)
            {
                this.AjaxMessageBox.ShowMessage(
                    String.IsNullOrEmpty(theManager.getEsitoOperazione().Descrizione) ?
                        "Si è verificato un problema non identificato durante l'inserimento del ruolo" :
                        theManager.getEsitoOperazione().Descrizione);
                //scriptForClose = "window.close();";
                scriptForClose = String.Format("window.returnValue='{0}';window.close();", Request["ret"]);

            }
            else
            {
                String idUo = Role.IDUo;
                theManager.ListaRuoliUO(Role.IDUo);
                Role = ((OrgRuolo[])theManager.getListaRuoliUO().ToArray(typeof(OrgRuolo))).Where(r => r.Codice == Role.Codice).FirstOrDefault();

                if (Role != null)
                    scriptForClose = String.Format("window.returnValue='{0}_{1}';window.close();", Role.IDCorrGlobale, idUo);
                else
                    //scriptForClose = String.Format("window.close();");
                    scriptForClose = String.Format("window.returnValue='{0}';window.close();", Request["ret"]);
            }


            // Chiusura della finestra
            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "ClosePage",
                scriptForClose,
                true);
                                
        }

        /// <summary>
        /// Metodo per la creazione dello script per l'apertura di questa finestra
        /// </summary>
        /// <returns>Script per l'apertura di questa finestra</returns>
        public static String GetOpenScript(String idUO)
        {
            return String.Format(
                "var retVal = window.showModalDialog('{0}/AdminTool/Gestione_Organigramma/CalculateAtipicitaAfterInsertRole.aspx?ret={1}_{2}', '', 'dialogWidth:588px;dialogHeight:300px; resizable: no;status:no;scroll:yes;help:no;close:no;center:yes;');Form1.hfRetValModSposta.value = retVal;document.forms[0].submit();",
                Utils.getHttpFullPath(),
                idUO,
                idUO);
        }

        /// <summary>
        /// Ruolo da inserire
        /// </summary>
        public static OrgRuolo Role
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["Role"] as OrgRuolo;
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                    CallContextStack.CurrentContext = new CallContext("Inserisci");
                CallContextStack.CurrentContext.ContextState["Role"] = value;
            }
        }
    }
}