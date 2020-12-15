using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA.PopUp
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VerificaSupportoRegistrato : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;

            if (!this.IsPostBack)
            {
                bool verificaSingoloDocumento = !string.IsNullOrEmpty(this.IdDocumento);

                this.trPercentuale.Visible = !verificaSingoloDocumento;
                this.trDataProssimaVerifica.Visible = !verificaSingoloDocumento;
                this.trNoteVerifica.Visible = !verificaSingoloDocumento;

                if (!verificaSingoloDocumento)
                    this.FetchDataProssimaVerifica();

                btnVerifica.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnVerifica.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnChiudi.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnChiudi.Attributes.Add("onmouseout", "this.className='cbtn';");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVerifica_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.IdDocumento))
            {
                // Memorizzazione esito verifica solamente per le verifiche sull'intero supporto, non sul singolo documento

                Utils.ConservazioneManager.RegistraEsitoVerificaSupportoRegistrato(
                                    (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                                    this.IdIstanza,
                                    this.IdSupporto,
                                    this.verificaSupportoSmartClient.Success,
                                    this.txtPercentuale.Text,
                                    this.txtDataProssimaVerifica.Text,
                                    this.txtNoteDiVerifica.Text,
                                    "?");

                this.ClientScript.RegisterStartupScript(this.GetType(), "close", "window.returnValue=true; window.close();", true);
            }
        }


//if (isFolderCons==string.Empty)
//{

//    //verifico la firma del file p7m
//    int verificaFirma = this.verificaFirma(urlP7m, ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]),idConservazione );
//    string resultFirma = string.Empty;
                            
//    bool verifica = this.decodificaStato(verificaFirma, ref resultFirma);
//    if (verifica)
//    {
//        //verifico la marca temporale
//        string verificaMarca = this.verificaMarca(urlTsr, ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]),idConservazione);
//        //se nn ci sono errori nella lettura della marca
//        if (verificaMarca == string.Empty)
//        {
//            int isChiusa = this.UpdateSupporto(collFisica, dataProxVer, note, idSupporto, Convert.ToString(numVer), "1", Convert.ToString(percInt));
//            bool resulInsertVer = ConservazioneManager.insertRisultatoVerifica(this.infoUtente, idSupporto, idConservazione, note, Convert.ToString(percInt), Convert.ToString(numVer), "1");
                                          
//            //se l'istanza viene chiusa ed è la prima verifica allora trasmetto
//            if (isChiusa == 1 && string.IsNullOrEmpty(dataPrecVerifica))
//            {
//                bool result = ConservazioneManager.trasmettiNotifica(idConservazione, ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]));
//            }
//            if (resultFirma != "Valido")
//            {
//                //per il momento nn utilizziamo questo metodo perché nn sappiamo quando scade la marca
//                // msgRigeneraMarca.Confirm("La verifica è andata a buon fine, si vuole procedere con la generazione di una nuova marca temporale?");
//                Response.Write("<script>alert('" + resultFirma + "')</script>");
//            }
//            else
//            {
//                Response.Write("<script>alert(\"La verifica è stata effettuata con successo.\")</script>");
//            }

//            //se l'istanza è chiusa richiamiamo il rigenera marca in modo che la trasmissione di chiusura venga fatta sempre con un solo tsr.
//            //if (isChiusa == 1 && !string.IsNullOrEmpty(dataPrecVerifica))                                              
//            if (isChiusa == 1)
//            {
//                //recupero l'idProfile della trasmissione appena effettuata
//                string filtro = this.Filtra();

//                infoSupp = ConservazioneManager.getInfoSupporto(filtro, infoUtente);                                           
                                            
//                this.caricaGridViewDettaglio(infoSupp, this.gv_dettaglio, this.hd_idIstanza.Value);
                                            
//                this.hd_ProfileTrasm.Value = infoSupp[index].idProfileTrasmissione;
                                            
//                msgRigeneraMarca.Confirm("La verifica è andata a buon fine, si vuole procedere con la generazione di una nuova marca temporale?");
//            }
//        }
//        else
//        {
//            string messaggio = string.Empty;
//            if (percentualeVerifica != Convert.ToDouble(-1))
//            {
//                string perc = Convert.ToString(percentualeVerifica);
//                string percAppo = "";
//                if (perc.Contains(","))
//                {
//                    percAppo = perc.Substring(0, perc.IndexOf(",") + 2);
//                }
//                else
//                {
//                    percAppo = perc;
//                }
//                messaggio = "La verifica è riuscita solo per il " + percAppo + "% dei documenti. Il supporto risulta danneggiato";
//                this.setSupportoDanneggiato(numVer, collFisica, dataProxVer, note, idSupporto, idConservazione, Convert.ToInt32(percInt), messaggio);
//                bool resulInsertVer = ConservazioneManager.insertRisultatoVerifica(this.infoUtente, idSupporto, idConservazione, note, Convert.ToString(percInt), Convert.ToString(numVer), "0");
//                Response.Write("<script>alert('" + messaggio + "')</script>");
//            }
//            else
//            {
//                Response.Write("<script>alert(\"Errore di comunicazione con il server, riprovare in seguito.\")</script>");
//            }
                                            
//        }
//    }
//    else
//    {
//        //if (verificaMarca != "KO")
//        //{
//        //    this.setSupportoDanneggiato(numVer, collFisica, dataProxVer, note, idSupporto, idConservazione, Convert.ToInt32(percInt), verificaMarca);
//        //    bool resulInsertVer = ConservazioneManager.insertRisultatoVerifica(this.infoUtente, idSupporto, idConservazione, note, Convert.ToString(percInt), Convert.ToString(numVer), "0");
//        //    Response.Write("<script>alert('La verifica della marca non è andata a buon fine. Il supporto risulta danneggiato.')</script>");
//        //}
//        //else
//        //{
//        //    Response.Write("<script>alert(\"Errore di comunicazione con il server per la verifica della marca, riprovare in seguito.\")</script>");
//        //}
//    }

        /// <summary>
        /// 
        /// </summary>
        protected string IdIstanza
        {
            get
            {
                return this.Request.QueryString["idIstanza"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string IdSupporto
        {
            get
            {
                return this.Request.QueryString["idSupporto"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string IdDocumento
        {
            get
            {
                return this.Request.QueryString["idDocumento"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void FetchDataProssimaVerifica()
        {
            // Se l'istanza è stata creata tramite policy, riporta il numero dei mesi definiti nella stessa, altrimenti il default è 6 mesi
            // MEV CS 1.5
            // la scadenza dei termini per le verifiche di integrità è definita dalla chiave di db
            // BE_CONSERVAZIONE_INTERVALLO
            
            WSConservazioneLocale.Policy policy = Utils.ConservazioneManager.GetPolicyIstanza((WSConservazioneLocale.InfoUtente)Session["infoutCons"], this.IdIstanza);
            string idAmministrazione = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]).idAmministrazione;

            try
            {

                // valore espresso in GIORNI
                int giorniIntervallo = Convert.ToInt32(Utils.ConservazioneManager.GetChiaveConfigurazione(idAmministrazione, "BE_CONSERVAZIONE_INTERVALLO"));

                //int mesiDefault = 6;

                if (policy != null)
                {
                    // Istanza creata tramite policy: impostazione del numero di mesi definiti in essa

                    int mesiPolicy;
                    Int32.TryParse(policy.avvisoMesi, out mesiPolicy);

                    //if (mesiPolicy == 0)
                    //    mesiPolicy = mesiDefault;

                    //this.txtDataProssimaVerifica.Text = DateTime.Today.AddMonths(mesiPolicy).ToString("dd/MM/yyyy");

                    if (mesiPolicy == 0)
                    {
                        // intervallo non definito nella policy
                        // utilizzo il valore preso da db
                        this.txtDataProssimaVerifica.Text = DateTime.Now.AddDays(giorniIntervallo).ToString("dd/MM/yyyy");

                    }
                    else
                    {
                        this.txtDataProssimaVerifica.Text = DateTime.Today.AddMonths(mesiPolicy).ToString("dd/MM/yyyy");
                    }

                }
                else
                {
                    //// Default: prossimo controllo a 6 mesi da oggi
                    //this.txtDataProssimaVerifica.Text = DateTime.Today.AddMonths(mesiDefault).ToString("dd/MM/yyyy");
                    this.txtDataProssimaVerifica.Text = DateTime.Today.AddDays(giorniIntervallo).ToString("dd/MM/yyyy");
                }

            }
            catch (Exception ex)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "fetchDataKO", "alert('Errore nel recupero della data di prossima verifica.');", true);
                this.txtDataProssimaVerifica.Text = string.Empty;
            }
        }
    }
}