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
using System.Collections.Generic;

namespace DocsPAWA
{
    public partial class visualizzaLink : System.Web.UI.Page
    {
        // L'utente loggato
        protected DocsPAWA.DocsPaWR.Utente userHome;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Distruzione della response
            this.Response.Expires = -1;

            // L'id del gruppo cui appartiene l'utente che 
            // ha richiesto la creazione del link
            string groupId;

            // Il numero del documento da visualizzare
            string docNumber = string.Empty;

            // Il system id del documento da visualizzare
            string idProfile = string.Empty;

            // Prelevamento dalla query string del docNumber, dell'idProfile
            // e del groupId
            docNumber = Request["docNumber"];
            idProfile = Request["idProfile"];
            groupId = Request["groupId"];


            // Verifica della correttezza dei parametri. In particolare non
            // possono essere nulli sia idProfile che docNumber e deve essere
            // presente groupId
            if ((!(String.IsNullOrEmpty(docNumber) && String.IsNullOrEmpty(idProfile))))
            {
                // Si prova a prelevare l'utente attualmente loggato
                try
                {
                    userHome = UserManager.getUtente();
                }
                catch (Exception ex)
                {
                    // ...se si sono verificati errori si considera 
                    // l'utente come non loggato...
                    userHome = null;
                }

                // ...se l'utente non è loggato...
                if (userHome == null)
                {
                    // ...si crea il link alla pagina di login...
                    string lgnPage = Utils.getHttpFullPath(this) + "/login.aspx?" +
                        Request.QueryString.ToString();

                    // ...e si immerge uno script per l'apertura della pagina di login
                    string scriptString = String.Format("redirectToLogin('{0}');", lgnPage);

                    ClientScript.RegisterStartupScript(this.GetType(), "redLogin", scriptString, true);

                }
                else
                {
                    if(string.IsNullOrEmpty(groupId)){
                        groupId = UserManager.getInfoUtente().idGruppo;
                    }
                    // ...altrimenti si visualizza il documento richiesto
                    this.visualizzaDocumento(docNumber, idProfile, Request["numVersion"],
                        Request["isAttachement"],groupId);

                }

            }
            else
                // In caso di mancanza di parametri viene visualizzato un messaggio di errore
                this.ltlMessage.Text = "Alcuni parametri non sono validi.";

        }

        /// <summary>
        /// Funzione per la visualizzazione del documento
        /// </summary>
        /// <param name="docNumber">Il numero di documento</param>
        /// <param name="idProfile">Il system id del documento</param>
        /// <param name="numVersion"></param>
        /// <param name="isAttachement"></param>
        /// <param name="groupId">L'id del gruppo cui appartiene l'utente che ha creato il link</param>
        private void visualizzaDocumento(string docNumber, string idProfile, string numVersion, string isAttachement, string groupId)
        {
            // Valore booleano utilizzato per indicare se l'utente ha i 
            // diritti necessari per poter vedere il documento
            bool canView = false;

            // Viene verificata per prima cosa se l'utente ha almeno un ruolo con
            // cui può vedere il documento
            canView = this.VerificaDirittiVisibilita(groupId, docNumber, idProfile);

            // Se l'utente possiede i diritti...
            if (canView)
            {
                // ...si crea l'url per l'apertura della pagina che si occuperà di visualizzare
                // il documento...
                /*
                string newUrl = String.Format(
                    "{0}/documento/Visualizzatore/VisualizzaFrame.aspx?docNumber={1}&idProfile={2}&numVersion={3}&isAttachement={4}",
                    Utils.getHttpFullPath(this),
                    docNumber,
                    idProfile,
                    numVersion,
                    isAttachement);
                */

                string newUrl = String.Format(
                    "{0}/documento/Visualizzatore/VisualizzaDocEsterno.aspx?docNumber={1}&idProfile={2}&numVersion={3}&isAttachement={4}",
                    Utils.getHttpFullPath(this),
                    docNumber,
                    idProfile,
                    numVersion,
                    isAttachement);
                
                                // Viene creato lo script per l'apertura della pagina
                string scriptString = "openViewer('" + newUrl + "');";

                // ...viene immerso il codice creato nella pagina
                ClientScript.RegisterStartupScript(this.GetType(), "visualizza", scriptString, true);

            }

        }

        /// <summary>
        /// Funzione per la verifica dei diritti di visibilità del documento in base ai ruoli
        /// esercitati dall'utente loggato
        /// </summary>
        /// <param name="groupId">L'id del gruppo cui appartiene l'utente che ha creato il link</param>
        /// <param name="docNumber">Il numero del documento da visualizzare</param>
        /// <param name="idProfile">Il system id del documento da visualizzare</param>
        /// <returns>True se l'utente ha visibilità sul documento, false altrimenti</returns>
        private bool VerificaDirittiVisibilita(string groupId, string docNumber, string idProfile)
        {
            string id = idProfile;
            if(!string.IsNullOrEmpty(docNumber)){
                id = docNumber;
            }
            // La scheda del documento
            DocsPaWR.SchedaDocumento result = null;

            // L'eventuale messaggio di errore resituito dalla verifica della visisbilità
            string errorMessage = String.Empty;

            // Variabile utilizzata per indicare che un ruolo valido per
            // visalizzare il documento è stato trovato
            bool trovato = false;

            // Se l'utente non ha già un ruolo impostato...
            if (Session["userRuolo"] == null)
            {
                // ...si verifica se l'utente loggato ha, fra i vari ruoli,
                // un ruolo uguale a quello dell'utente che ha creato il link...
                foreach (DocsPaWR.Ruolo ruolo in userHome.ruoli)
                    if (ruolo.idGruppo.Equals(groupId))
                    {
                        Session["userRuolo"] = ruolo;
                        trovato = true;
                    }

                // Se non è stato trovato lo stesso id gruppo...
                if (!trovato)
                {
                    // ...viene inizializzato un iteratore per scorrere i ruoli associati
                    // all'utente...
                    System.Collections.Generic.IEnumerator<DocsPaWR.Ruolo> itRuoli =
                        userHome.ruoli.ToList().GetEnumerator();

                    // ...finchè non si triva un ruolo valido per 
                    // la visualizzazione del documento o finché i ruoli
                    // ricoperti dall'utente loggato non terminano...
                    while (itRuoli.MoveNext() && !trovato)
                    {
                        // ...viene immesso il ruolo corrente in sessione...
                        Session["userRuolo"] = itRuoli.Current;
                        // ...se con il ruolo attuale l'utente ha i diritti 
                        // necessari alla visualizzazione...
                        if (DocumentManager.verificaACL("D",
                                id, UserManager.getInfoUtente(), out errorMessage) == 2)
                            // ...è stato trovato un ruolo adatto...
                            trovato = true;
                    }

                }

            }
            else
                // ...altrimenti se il ruolo con cui è loggato gli concede 
                // visibilità sul documento...
                if (DocumentManager.verificaACL("D",
                        id, UserManager.getInfoUtente(), out errorMessage) == 2)
                    // ...ho trovato un ruolo valido
                    trovato = true;

            // Se non è stato trovato un ruolo adatto
            if (!trovato)
                // viene segnalato all'utente che non ha i diritti
                // necessari
                this.ltlMessage.Text = errorMessage;

            return trovato;

        }        
    }
}
