using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ConservazioneWA.Utils;
using System.Drawing;
using System.Net.Mail;
using Debugger = ConservazioneWA.Utils.Debugger;
using ConservazioneWA.DocsPaWR;

namespace ConservazioneWA
{
    public partial class HomePageNew : System.Web.UI.Page
    {
        protected WSConservazioneLocale.InfoUtente infoUtente;
        protected WSConservazioneLocale.InfoAmministrazione amm;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            amm = Utils.ConservazioneManager.GetInfoAmmCorrente(this.infoUtente.idAmministrazione);
            InizializzaPagina();
            //menuTop.ProfiloUtente = CalcolaProfiloUtente();
            //menuTop.ProfiloUtente = ConservazioneManager.CalcolaProfiloUtente(this.infoUtente.idPeople, this.infoUtente.idAmministrazione);
            menuTop.ProfiloUtente = "CONSERVAZIONE";
        }

        protected void InizializzaPagina()
        {
            WSConservazioneLocale.Contatori contatori = Utils.ConservazioneManager.GetContatori(this.infoUtente);

            //MEV CS 1.4
            //contatori istanze di esibizione
            WSConservazioneLocale.ContatoriEsibizione contatoriEsibizione = Utils.ConservazioneManager.GetContatoriEsibizioneConservazione(this.infoUtente);
            this.lbl_daCertificare.Text = (contatoriEsibizione.InAttesaDiCertificazione + contatoriEsibizione.InAttesaDiCertificazione_Certificata).ToString();

            this.lbl_nuove.Text = contatori.Inviate.ToString();
            this.lbl_lavorazione.Text = contatori.InLavorazione.ToString();
            this.lbl_firmate.Text = contatori.Firmate.ToString();
            this.lbl_conservate.Text = contatori.Conservate.ToString();
            this.lbl_chiuse.Text = contatori.Chiuse.ToString();
            this.lbl_notifiche.Text = contatori.Notifiche.ToString();
            this.lbl_amm.Text = this.amm.Codice + " - " + this.amm.Descrizione;
        }

        /// <summary>
        /// Metodo di prova. Da eliminare al termine dello sviluppo MEV CS 1.4 - Esibizione
        /// </summary>
        /// <returns></returns>
        public string CalcolaProfiloUtente() 
        {
            string ProfiloUtente = string.Empty;
            bool AbilitatoCentroServizi = false;
            bool AbilitatoEsibizione = false;

            // Chiamata al Backend che calcola il profilo Utente a partire dall'infoutente che si è loggato
            // passando i seguenti parametri: this.infoUtente.idAmministrazione, this.infoUtente.idPeople
            
            // Simulo l'esito della chiamata
            try
            {
                AbilitatoEsibizione = System.Configuration.ConfigurationManager.AppSettings["ABILITATOESIBIZIONE"].Equals("ESIBIZIONE") ? true : false;
            }
            catch (Exception e) 
            {
                AbilitatoEsibizione = false;
            }

            // Controllo Flag
            if (AbilitatoCentroServizi) 
            {
                ProfiloUtente = "CONSERVAZIONE";
            }

            if (AbilitatoEsibizione) 
            {
                ProfiloUtente = "ESIBIZIONE";
            }
            
            return ProfiloUtente;
        }
    }
}