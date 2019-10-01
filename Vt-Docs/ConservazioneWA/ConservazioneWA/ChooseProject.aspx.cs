using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ConservazioneWA.WSConservazioneLocale;

namespace ConservazioneWA
{
    public partial class ChooseProject : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            if (!IsPostBack)
            {
                FetchData();
            }
        }

        protected void FetchData()
        {
            ConservazioneWA.WSConservazioneLocale.Fascicolo[] projectList = HttpContext.Current.Session["fascicoli"] as Fascicolo[];
            this.grvFileType.DataSource = projectList;
            this.grvFileType.CurrentPageIndex = 0;
            this.grvFileType.DataBind();
            this.btn_salva.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            this.btn_salva.Attributes.Add("onmouseout", "this.className='cbtn';");

            this.btn_annulla.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            this.btn_annulla.Attributes.Add("onmouseout", "this.className='cbtn';");
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

                WSConservazioneLocale.Fascicolo[] projectList = HttpContext.Current.Session["fascicoli"] as Fascicolo[];
                foreach (Fascicolo tempFasc in projectList)
                {
                    if (tempFasc.systemID.Equals(idProject))
                    {
                        selezionato = tempFasc;
                        projectList = null;
                        projectList = new Fascicolo[1];
                        projectList[0] = selezionato;
                        break;
                    }
                }
                //Gabriele Melini 07/11/2013
                //devo rimettere i fascicoli in sessione dopo la scelta, sennò prende sempre il primo
                HttpContext.Current.Session["fascicoli"] = projectList as WSConservazioneLocale.Fascicolo[];
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('" + selezionato.systemID + "');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "alert('Selezionare un fascicolo');", true);
            }
        }

    }
}