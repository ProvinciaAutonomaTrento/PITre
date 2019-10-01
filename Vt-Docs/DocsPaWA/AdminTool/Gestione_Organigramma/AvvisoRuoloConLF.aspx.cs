using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.Gestione_Organigramma
{
    public partial class AvvisoRuoloConLF : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                string tipoTitolare = this.Request.QueryString["tipoTitolare"];
                this.btn_si.Attributes.Add("onclick", "window.returnValue = 'Y'; window.close();");
                this.btn_no.Attributes.Add("onclick", "window.returnValue = 'N'; window.close();");
                this.btn_si_senza_interruzione.Attributes.Add("onclick", "window.returnValue = 'Y_SENZA_INTERRUZIONE'; window.close();");

                if (tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.RUOLO))
                    this.InitializePageRuolo(tipoTitolare);
                else
                    this.InitializePageUtente(tipoTitolare);


                string idRuoloTitolare = !string.IsNullOrEmpty(this.Request.QueryString["idRuolo"]) ? this.Request.QueryString["idRuolo"].ToString() : string.Empty;
                string idUtenteTitolare = !string.IsNullOrEmpty(this.Request.QueryString["idUtente"]) ? this.Request.QueryString["idUtente"].ToString() : string.Empty;

                //Se stò disabilitando l'utente
                if(tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.DISABILITA_UTENTE))
                {
                    this.btn_dettaglio.Attributes.Add("onclick", "ApriProcessiFirmaRuoliUtente('" + idUtenteTitolare + "');");
                }
                //Se è l'ultimo utente del ruolo visualizzo i processi non del solo utente ma di tutto il ruolo
                else if (tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE_RUOLO)
                    || tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE)
                    || tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE_E_RUOLO))
                    this.btn_dettaglio.Attributes.Add("onclick", "ApriProcessiFirmaRuolo('" + idRuoloTitolare + "');");
                else
                {
                    if (!string.IsNullOrEmpty(idRuoloTitolare) && !string.IsNullOrEmpty(idUtenteTitolare))
                        this.btn_dettaglio.Attributes.Add("onclick", "ApriProcessiFirma('" + idRuoloTitolare + "','" + idUtenteTitolare + "');");
                    else if (!string.IsNullOrEmpty(idRuoloTitolare))
                        this.btn_dettaglio.Attributes.Add("onclick", "ApriProcessiFirmaRuolo('" + idRuoloTitolare + "');");
                }

                //int countProcessiCoinvolti = Convert.ToInt32(Request.QueryString["numProcessi"]);
                //int countIstanzaProcessiCoinvolti = Convert.ToInt32(Request.QueryString["numIstanze"]);
                //string messagio = GetMessaggio(tipoTitolare, countProcessiCoinvolti, countIstanzaProcessiCoinvolti);

                //this.lbl_utente.Text = messagio;
            }
        }

        private void InitializePageRuolo(string tipoTitolare)
        {
            this.btn_si_senza_interruzione.Visible = true;
            string tipoOperazione = this.Request.QueryString["tipoOperazione"];

            //Se disabilito la trasmissione, devo necessariamente invalidare i processi
            if (!string.IsNullOrEmpty(tipoOperazione)
                && tipoOperazione.Equals(Amministrazione.Manager.OrganigrammaManager.TipoOperazione.MODIFICA_RUOLO_CON_DISABILITAZIONE_TRASM))
            {
                this.btn_si_senza_interruzione.Visible = false;
            }

            //Imposto il messaggio da visualizzare
            string messaggio = string.Empty;
            messaggio = "Il ruolo che si sta modificando è coinvolto in processi di firma o processi di firma avviati pertanto, se si procede con interruzione, verranno rispettivamente invalidati ed interrotti.<br />";
            messaggio += "Proseguendo con interruzione, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tale processo.";
            messaggio += "<br />Importante avvisare il disegnatore dei processi coinvolti e il proponente.<br />Per ulteriori informazioni, visualizza dettaglio.<br />Sei sicuro di voler procedere?";
            this.lbl_utente.Text = messaggio;
        }

        private void InitializePageUtente(string tipoTitolare)
        {
            this.btn_add_user.Visible = true;

            //In caso di rimozione ultimo utente, posso aggiungere
            if (tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE_RUOLO)
            || tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE)
            || tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE_E_RUOLO))
                this.btn_add_user.Attributes.Add("onclick", "window.returnValue = 'ADD_USER'; window.close();");

            //In caso di utente non ultimo, posso sostituirlo
            if (tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.UTENTE))
                this.btn_add_user.Attributes.Add("onclick", "window.returnValue = 'REPLACE_USER'; window.close();");

            //Imposto il messaggio da visualizzare
            string messaggio = string.Empty;
            if (tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE_RUOLO))
            {
                messaggio = "L'utente che si sta modificando è l'ultimo utente del ruolo pertanto, se non si procede con la sostituzione dello stesso, verranno invalidati i processi di firma ed interrotti i processi avviati in cui il ruolo è coinvolto.<br />";
                messaggio += "Proseguendo con interruzione, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tale processo.";
            }
            if (tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE_E_RUOLO))
            {
                messaggio = "L'utente che si sta modificando è l'ultimo utente del ruolo pertanto, se non si procede con la sostituzione dello stesso, verranno invalidati i processi ed ed interrotti i processi avviati in cui il ruolo è coinvolto.<br />";
                messaggio += "Inoltre, l'utente stesso è coinvolto in processi di firma e in processi di firma avviati.<br />";
                messaggio += "Proseguendo con interruzione, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tale processo.";
            }
            if (tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE)
                || tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.UTENTE))
            {
                messaggio = "L'utente che si sta modificando è coinvolto in processi di firma e processi di firma avviati pertanto, se non si procede con la sostituzione dello stesso, verranno rispettivamente invalidati ed interrotti.<br />";
                messaggio += "Proseguendo con interruzione, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tale processo.";
            }
            if (tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.DISABILITA_UTENTE))
            {
                this.btn_si_senza_interruzione.Visible = false;
                this.btn_add_user.Visible = false;
                messaggio = "L'utente che si stà disabilitando è coinvolto in processi di firma. Procedendo con l'operazione verranno invalidati/interrotti tutti i processi in cui l'utente o il ruolo in cui appartiene(se ultimo utente) è coinvolto.<br />";
                messaggio += "Se si desidera non invalidare/interrompere i processi è necessario proseguire con la sostituzione dell'utente in 'Organigramma'.<br />";
                messaggio += "Proseguendo con interruzione, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tale processo.";
            }

            messaggio += "<br />Importante avvisare il disegnatore dei processi coinvolti e il proponente.<br />Per ulteriori informazioni, visualizza dettaglio.<br />Sei sicuro di voler procedere?";
            this.lbl_utente.Text = messaggio;
        }

        /*
        private string GetMessaggio(string tipoTitolare, int countProcessi, int countIstanzeAttive)
        {
            string messaggio = string.Empty;
            if (tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE_RUOLO))
            {
                messaggio = "L'utente che si sta modificando è l'ultimo utente del ruolo pertanto, se non si procede con la sostituzione dello stesso, verranno invalidati i processi di firma ed interrotti i processi avviati in cui il ruolo è coinvolto.<br />";
                messaggio += "Proseguendo con interruzione, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tale processo.";
            }
            if (tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE_E_RUOLO))
            {
                messaggio = "L'utente che si sta modificando è l'ultimo utente del ruolo pertanto, se non si procede con la sostituzione dello stesso, verranno invalidati i processi ed ed interrotti i processi avviati in cui il ruolo è coinvolto.<br />";
                messaggio += "Inoltre, l'utente stesso è coinvolto in processi di firma e in processi di firma avviati.<br />";
                messaggio += "Proseguendo con interruzione, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tale processo.";
            }
            if (tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.ULTIMO_UTENTE) 
                || tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.UTENTE))
            {
                messaggio = "L'utente che si sta modificando è coinvolto in processi di firma e processi di firma avviati pertanto, se non si procede con la sostituzione dello stesso, verranno rispettivamente invalidati ed interrotti.<br />";
                messaggio += "Proseguendo con interruzione, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tale processo.";
            }
            if (tipoTitolare.Equals(Amministrazione.Manager.OrganigrammaManager.SoggettoInModifica.RUOLO))
            {
                messaggio = "Il ruolo che si sta modificando è coinvolto in processi di firma o processi di firma avviati pertanto, se si procede con interruzione, verranno rispettivamente invalidati ed interrotti.<br />";
                messaggio += "Proseguendo con interruzione, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tale processo.";
            }
            //if (countProcessi > 0)
            //{
            //    messaggio += GetMessaggioProcessi(tipoTitolare, countProcessi);
            //}
            //if (countIstanzeAttive > 0)
            //{
            //    messaggio += GetMessaggioIstanzeAttive(tipoTitolare, countProcessi, countProcessi);
            //}
            //messaggio += "<br />Importante avvisare il creatore dei processi coinvolti e il proponente.<br />Sei sicuro di voler procedere?";
            messaggio += "<br />Importante avvisare il creatore dei processi coinvolti e il proponente.<br />Per ulteriori informazioni, visualizza dettaglio.<br />Sei sicuro di voler procedere?";
            return messaggio;
        }

        private string GetMessaggioProcessi(string tipoTitolare, int countProcessi)
        {
            string messaggio = string.Empty;
            switch (tipoTitolare)
            {
                case UTENTE_COINVOLTO:
                    messaggio += "L'utente ";
                    break;
                case RUOLO:
                    messaggio += "Il ruolo ";
                    break;       
            }

            messaggio += (
            (countProcessi > 1) ?
            "in modifica è coinvolto in " + countProcessi + " processi di firma. Proseguendo nella modifica, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tali processi." :
            "in modifica è coinvolto in un processo di firma. Proseguendo nella modifica, sarà necessario modificare i passi in cui esso è coinvolto per poter avviare tale processo.");

            return messaggio;
        }

        private string GetMessaggioIstanzeAttive(string tipoTitolare, int countProcessi, int countIstanzeAttive)
        {
            string messaggio = string.Empty;

            switch (tipoTitolare)
            {
                case UTENTE_COINVOLTO:
                    messaggio += countProcessi > 0 ? "<br />Inoltre l'utente " : "L'utente in modifica ";
                    break;
                case ULTIMO_UTENTE_COINVOLTO:
                case RUOLO:
                    messaggio += countProcessi > 0 ? "<br />Inoltre il ruolo " : "Il ruolo in modifica ";
                    break;
            }
            messaggio += (
                    (countIstanzeAttive > 1) ?
                    "è coinvolto in " + countIstanzeAttive + " processi di firma avviati e non ancora conclusi. Proseguendo nella modifica, tutti i processi coivolti saranno interrotti." :
                    "è coinvolto in un processo di firma avviato e non ancora concluso. Proseguendo nella modifica, il processo coinvolto sarà interrotto.");
            return messaggio;
        }
        */
    }
}