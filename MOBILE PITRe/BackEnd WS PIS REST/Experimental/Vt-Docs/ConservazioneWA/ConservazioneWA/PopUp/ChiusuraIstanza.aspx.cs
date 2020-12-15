using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA.PopUp
{
    public partial class ChiusuraIstanza : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btn_chiudi.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btn_chiudi.Attributes.Add("onmouseout", "this.className='cbtn';");
            if (string.IsNullOrEmpty(hd_stato.Value.ToString()))
            {
                btn_chiudi.Visible = false;
                img_loading.Visible = true;
                lbl_messaggio.Text = "Verifica Leggibilità effettuata con successo.<br />Caricamento dei file sullo storage remoto.";
                timer1.Enabled = true;
                //this.caricamento();
                //WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                //if (ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(infoUtente, IdIstanza, true))
                //    lbl_messaggio.Text = "Chiusura dell'istanza avvenuta con successo.";
                //else
                //    lbl_messaggio.Text = "Errore durante l'upload dell'istanza nello storage remoto.<br />L'istanza rimane in stato firmata.";
                //btn_chiudi.Visible = true;
                //img_loading.Visible = false;
            }
            else
            {
                if(hd_stato.Value=="ok")
                    lbl_messaggio.Text = "Chiusura dell'istanza avvenuta con successo.";
                else if(hd_stato.Value=="ko")
                    lbl_messaggio.Text = "Errore durante l'upload dell'istanza nello storage remoto.<br />L'istanza rimane in stato firmata.";
                btn_chiudi.Visible = true;
                img_loading.Visible = false;
            }
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            Response.Write("<script>window.close();</script>");
        }

        protected string IdIstanza
        {
            get
            {
                return this.Request.QueryString["idCons"];
            }
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            //this.caricamento();
        }

        protected void caricamento()
        {
            WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            if (ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(infoUtente, IdIstanza, true))
                hd_stato.Value = "ok";
            else
                hd_stato.Value = "ko";
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ricarica", "<script>window.reload();</script>",true);
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ricarica", "<script>document.getElementById('form1').submit();</script>", true);
        }

        protected void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            caricamento();
            this.Page_Load(sender, e);
        }
    }
}