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
using System.Globalization;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ConservazioneWA.PopUp
{
    public partial class VerificaSupporto : System.Web.UI.Page
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
                btnVerifica.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnVerifica.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnChiudi.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnChiudi.Attributes.Add("onmouseout", "this.className='cbtn';");

                if (!verificaSingoloDocumento)
                    this.txtDataProssimaVerifica.Text = FetchDataProssimaVerifica();
                else
                {
                    this.verificaSingoloFile();
                    lb_intestazione.Text = "Verifica su singolo file...";
                    tabella.Visible = false;
                }

                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVerifica_Click(object sender, EventArgs e)
        {
            int percent = 200;
            if (!(!string.IsNullOrEmpty(txtPercentuale.Text) && Int32.TryParse(txtPercentuale.Text, out percent) && percent < 101 && percent > 0))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_errore", "alert('Inserire un valore valido per la percentuale.');", true);
                
            }else{
            bool integritaVerificata = true;

            WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                
            Dictionary<String, String> documentiMemorizzati = ConservazioneWA.Utils.ConservazioneManager.getFilesFromUniSincro(infoUtente, IdIstanza,false);
            WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(IdIstanza, infoUtente);
            int totDocs = items.Length;
            int number = 0;
            percent = Int32.Parse( txtPercentuale.Text);
            double op = (totDocs * 100 / percent);
            number = (int)Math.Ceiling((double)((double)(totDocs * percent) / 100));
            Random rdm = new Random();
            string[] presi = new string[number];
            int i = 0;
            while (i < number)
            {
                int j = rdm.Next(totDocs);
                if (!presi.Contains(items[j].DocNumber))
                {
                    //files += items[j].DocNumber + ",";
                    presi[i] = items[j].DocNumber;
                    i++;
                }
            }
            string unisincroitem = "",path ="", hash="";
            string[] uniSincroItems = null;
            string hashValue = null; int validi= 0, invalidi=0;
         
            foreach (string docnum in presi)
            {
                unisincroitem = documentiMemorizzati[docnum];
                uniSincroItems = unisincroitem.Split('§');
                path = uniSincroItems[2];
                hash = uniSincroItems[3];

                hashValue = ConservazioneWA.Utils.ConservazioneManager.getFileHashFromStore(infoUtente, IdIstanza, path, false);
               
                if (hash.ToLower () != hashValue.ToLower ()) {integritaVerificata = false; invalidi++;}
                else{ validi++;}

            }
            if (!integritaVerificata)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_fallimento", "alert('Verifica Integrità fallita.');", true);
                //ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(hd_idCons.Value, false);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_successo", "alert('Verifica Integrità eseguita con successo.');", true);
                //ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(hd_idCons.Value, true);
            }
                        
                // Memorizzazione esito verifica solamente per le verifiche sull'intero supporto, non sul singolo documento
                
            Utils.ConservazioneManager.RegistraEsitoVerificaSupportoRegistrato(
                                (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                                this.IdIstanza,
                                this.IdSupporto,
                                integritaVerificata,
                                this.txtPercentuale.Text,
                                this.txtDataProssimaVerifica.Text,
                                this.txtNoteDiVerifica.Text,
                                "I");

            this.ClientScript.RegisterStartupScript(this.GetType(), "close", "window.returnValue=true; window.close();", true);
            ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroControlli(IdIstanza, "", infoUtente, "Verifica Integrità storage", integritaVerificata, number, validi, invalidi);
            }
        }
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
        protected string IdSupporto
        {
            get
            {
                return this.Request.QueryString["idSupporto"];
            }
        }


        /// <summary>
        /// Nel caso la verifica venga fatta su di un singolo file, per l'inserimento nel registro delle verifica dei supporti registrati
        /// c'è bisogno della data della prossima verifica.
        /// Questo metodo la estrae.
        /// </summary>
        /// <returns></returns>
        protected string FetchDataProssimaVerifica()
        {
            // Se l'istanza è stata creata tramite policy, riporta il numero dei mesi definiti nella stessa, altrimenti il default è 6 mesi
            WSConservazioneLocale.Policy policy = Utils.ConservazioneManager.GetPolicyIstanza((WSConservazioneLocale.InfoUtente)Session["infoutCons"], IdIstanza);
            //int mesiDefault = 6;

            // MEV CS 1.5
            // l'intervallo tra due verifiche è definito in amministrazione
            // valore espresso in GIORNI
            int intervalloVerifica = Convert.ToInt32(Utils.ConservazioneManager.GetChiaveConfigurazione(((WSConservazioneLocale.InfoUtente)Session["infoutCons"]).idAmministrazione, "BE_CONSERVAZIONE_INTERVALLO"));

            if (policy != null)
            {
                // Istanza creata tramite policy: impostazione del numero di mesi definiti in essa
                int mesiPolicy;
                Int32.TryParse(policy.avvisoMesi, out mesiPolicy);
                if (mesiPolicy == 0)
                {
                    //mesiPolicy = mesiDefault;
                    //return DateTime.Today.AddMonths(mesiPolicy).ToString("dd/MM/yyyy");
                    return DateTime.Today.AddDays(intervalloVerifica).ToString("dd/MM/yyyy");
                }
                else
                {
                    return DateTime.Today.AddMonths(mesiPolicy).ToString("dd/MM/yyyy");
                }
            }
            else
            {
                // Default: prossimo controllo a 6 mesi da oggi
                //return DateTime.Today.AddMonths(mesiDefault).ToString("dd/MM/yyyy");
                return DateTime.Today.AddDays(intervalloVerifica).ToString("dd/MM/yyyy");
            }
        }

        /// <summary>
        /// Nel caso la verifica venga fatta su di un singolo file, per l'inserimento nel registro delle verifica dei supporti registrati
        /// c'è bisogno dell'id del supporto remoto.
        /// Questo metodo estrae l'id del primo supporto remoto (per definizione dovrebbe essere unico).
        /// </summary>
        /// <returns></returns>
        protected string FetchIdSupporto()
        {
            string filters = " A.ID_CONSERVAZIONE= '" + IdIstanza + "'  AND  ( A.ID_TIPO_SUPPORTO = (SELECT system_id from dpa_tipo_supporto where var_tipo='REMOTO') ) and a.ID_CONSERVAZIONE = c.SYSTEM_ID and c.id_amm = " + ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]).idAmministrazione + " ORDER BY A.ID_CONSERVAZIONE DESC, A.COPIA ASC";
            WSConservazioneLocale.InfoSupporto[] supporti = ConservazioneWA.Utils.ConservazioneManager.getInfoSupporto(filters, (WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            return supporti.FirstOrDefault().SystemID;
        }

        /// <summary>
        /// Metodo utilizzato per la verifica su di un singolo file. Viene richiamato se nella chiamata della pagina viene specificato
        /// un idDocumento. L'idDocumento considerato in realtà è il docnumber nel database.
        /// </summary>
        private void verificaSingoloFile()
        {
            string descrizione, esito, segnaturaOrId;
            WSConservazioneLocale.Esito Cha_Esito;
            WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            segnaturaOrId = ConservazioneWA.Utils.ConservazioneManager.getSegnatura_ID_Doc(IdDocumento);

            Dictionary<String, String> documentiMemorizzati = ConservazioneWA.Utils.ConservazioneManager.getFilesFromUniSincro(infoUtente, IdIstanza,false);
            WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(IdIstanza, infoUtente);
            
            //Codice per ottenere il primo supporto remoto di un'istanza. 
            //In questo caso abbiamo il path del supporto nelle chiavi di configurazione e quindi il supporto è sempre lo stesso.
            //
            //string filtro = " A.ID_CONSERVAZIONE= '"+IdIstanza+"'  AND  ( A.ID_TIPO_SUPPORTO = 1 ) and a.ID_CONSERVAZIONE = c.SYSTEM_ID and c.id_amm = "+infoUtente.idAmministrazione+" ORDER BY A.ID_CONSERVAZIONE DESC, A.COPIA ASC";
            //WSConservazioneLocale.InfoSupporto[] supporti = ConservazioneWA.Utils.ConservazioneManager.getInfoSupporto(filtro, infoUtente);
            //string idSupporto2 = supporti[0].SystemID;
            int percent = (int)Math.Ceiling((double)((double)(100 / items.Length)));
                    
            string unisincroitem = documentiMemorizzati[IdDocumento];
            string[] uniSincroItems = unisincroitem.Split('§');
            string path = uniSincroItems[2];
            string hash = uniSincroItems[3];
            string hashValue = ConservazioneWA.Utils.ConservazioneManager.getFileHashFromStore(infoUtente, IdIstanza, path,false);
            bool integritaVerificata = true;
            int invalidi = 0, validi = 0;
            if (hash.ToLower() != hashValue.ToLower()) { integritaVerificata = false; invalidi++; }
            else { validi++; }
            if (!integritaVerificata)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_fallimento", "alert('Verifica Integrità fallita. File danneggiato');", true);
                //ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(hd_idCons.Value, false);
                descrizione = " Verifica Fallita Integrità Documento " + segnaturaOrId + " in istanza " + IdIstanza;
                esito = "0";
                Cha_Esito = WSConservazioneLocale.Esito.KO;

            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_successo", "alert('Verifica Integrità sul file eseguita con successo.');", true);
                //ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(hd_idCons.Value, true);
                descrizione = " Verifica effettuata con successo integrità documento " + segnaturaOrId+ " in istanza " + IdIstanza;
                esito = "1";
                Cha_Esito = WSConservazioneLocale.Esito.OK;

            }
            Utils.ConservazioneManager.RegistraEsitoVerificaSupportoRegistrato(
                                (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                                this.IdIstanza,
                                FetchIdSupporto(),
                                integritaVerificata,
                                percent.ToString(),
                                FetchDataProssimaVerifica(),
                                "Verifica Integrità su singolo file",
                                "I");
            this.ClientScript.RegisterStartupScript(this.GetType(), "close", "window.returnValue=true; window.close();", true);
            ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroControlli(IdIstanza, IdDocumento, infoUtente, "Verifica Integrità singolo file", integritaVerificata, 1, validi, invalidi);

            // Modifica scrittura Registro di Conservazione per la scrittura di Verifica Integrità sul singolo documento
            WSConservazioneLocale.RegistroCons regCons2 = new WSConservazioneLocale.RegistroCons();
            regCons2.idAmm = infoUtente.idAmministrazione;
            regCons2.idOggetto = IdDocumento;
            regCons2.idIstanza = IdIstanza;
            regCons2.tipoOggetto = "I";
            regCons2.tipoAzione = "";
            regCons2.userId = infoUtente.userId;
            regCons2.codAzione = "INTEGRITA_STORAGE";
            regCons2.descAzione = descrizione;
            regCons2.esito = esito;
            ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons2, infoUtente);

            // Inserisce nel DPA_LOG la Verifica dell'istanza
            //ConservazioneWA.Utils.ConservazioneManager.inserimentoInDpaLog(infoUtente, "INTEGRITA_STORAGE", IdIstanza, descrizione, Cha_Esito);

        }

       
    }
}
