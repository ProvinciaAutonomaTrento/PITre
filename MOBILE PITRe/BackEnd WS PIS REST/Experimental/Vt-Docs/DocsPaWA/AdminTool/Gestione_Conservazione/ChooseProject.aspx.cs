using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class ChooseProject : System.Web.UI.Page
    {
        private DocsPaWR.DocsPaWebService _wsInstance = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);
            if (!IsPostBack)
            {
                FetchData();
            }
        }

        protected void FetchData()
        {

            this.grvFileType.DataSource = this.FascicoliSelezionati;
            this.grvFileType.CurrentPageIndex = 0;
            this.grvFileType.DataBind();

        }

        /// <summary>
        /// Reperimento idamministrazione corrente
        /// </summary>
        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }

        protected DocsPaWR.DocsPaWebService WsInstance
        {
            get
            {
                if (this._wsInstance == null)
                    this._wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();
                return this._wsInstance;
            }
        }

        protected String GetProjectID(Fascicolo temp)
        {
            return temp.systemID;
        }

        protected String GetProjectCode(Fascicolo temp)
        {
            return temp.codice;
        }

        protected String GetProjectDescription(Fascicolo temp)
        {
            return temp.descrizione;
        }

        protected String GetProjectTitolario(Fascicolo temp)
        {
            return temp.codiceRegistroNodoTit;
        }


        /// Al clic viene salvata la lista dei formato documenti ammessi
        /// </summary>
        protected void BtnSaveDocumentFormat_Click(object sender, EventArgs e)
        {
            string idProject = string.Empty;
            if (Request.Form["rbl_pref"] != null && !string.IsNullOrEmpty(Request.Form["rbl_pref"].ToString()))
            {
                idProject = Request.Form["rbl_pref"].ToString();
                Fascicolo selezionato = null;
                foreach (Fascicolo tempFasc in FascicoliSelezionati)
                {
                    if (tempFasc.systemID.Equals(idProject))
                    {
                        selezionato = tempFasc;
                        FascicoliSelezionati = null;
                        FascicoliSelezionati = new Fascicolo[1];
                        FascicoliSelezionati[0] = selezionato;
                        break;
                    }
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('" + selezionato.systemID + "');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "alert(Selezionare un fascicolo);", true);
            }
        }

        /// <summary>
        /// Fascicoli selezionati
        /// </summary>
        public Fascicolo[] FascicoliSelezionati
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["FascicoliSelezionati"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["FascicoliSelezionati"] as Fascicolo[];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["FascicoliSelezionati"] = value;
            }
        }

    }
}