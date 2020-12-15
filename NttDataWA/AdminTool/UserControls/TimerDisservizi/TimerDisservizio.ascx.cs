using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAAdminTool.UserControls.TimerDisservizi
{
    public partial class TimerDisservizio : System.Web.UI.UserControl
    {
        private SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            string valorechiave;
            valorechiave = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "FE_TIMER_DISSERVIZIO");
            if (!string.IsNullOrEmpty(valorechiave))
            {
                int interval = int.Parse(valorechiave);
                UpdateTimer.Interval = interval;
            }
            else
                UpdateTimer.Interval = 60000;

        }

        /// <summary>
        /// Url della pagina web necessaria per effettuare il download semplice di un documento 
        /// </summary>
        protected string AccettaDisservizioPageUrl
        {
            get
            {
                return Utils.getHttpFullPath() + "/UserControls/TimerDisservizi/AccettaDisservizioPage.aspx";
            }
        }

        protected void UpdateTimer_Tick(object sender, EventArgs e)
        {
            //Se la chiave di configurazione è settata a 0 disattivo il timer
            string valoreChiave;
            valoreChiave= SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "FE_GESTIONE_DISSERVIZIO");
            //if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("0"))
            //{
            //    UpdateTimer.Enabled = false;
            //}  
             string stato="";
            if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1")){

                DocsPaWR.Utente utente = (SAAdminTool.DocsPaWR.Utente)this.Session["userData"];
                SAAdminTool.DocsPaWR.Disservizio disservizio = wws.getInfoDisservizio();
                if(utente!=null)
                 stato = wws.getStatoAccettazioneUtente(utente.idPeople);

                if (stato.ToUpper().Equals("N"))
                {
                
                    ltlMessage.Text = disservizio.testo_notifica;
                    btnD.Focus();

                    mpeMsg.Show();
                    udpMsj.Update();
                }
                if (disservizio.stato.ToUpper().Equals("ATTIVO"))
                {
                    Session["Disservizio"] = "attivo";
                    UpdateTimer.Enabled = false;
                    Response.Redirect("disservizio.aspx");
                }
                else
                    Session["Disservizio"] = "disattivo";
            }
            else
                UpdateTimer.Enabled = false;
        }
    }
       
}
