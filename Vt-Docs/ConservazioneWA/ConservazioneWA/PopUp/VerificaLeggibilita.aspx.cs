using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA.PopUp
{
    public partial class VerificaLeggibilita : System.Web.UI.Page
    {
        private string idConservazione, file;
        private int numero;
        string locale ="false";
        bool localStore = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            string frameSrc = "docVisualizza.aspx";
            WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            if (!Page.IsPostBack)
            {
               
                idConservazione = this.Request.QueryString["idCons"];
                hd_idCons.Value = idConservazione;
                file= this.Request.QueryString["file"];
                //Cambiamento del reperimento file per la multiselezione

                string[] files = file.Split(',');
                numero = files.Length-1;
                this.hd_indice_docs.Value = "1";
                this.hd_num_docs.Value = numero.ToString();
                this.hd_files.Value = file;

                locale= this.Request.QueryString["locale"];
                
                if (String.IsNullOrEmpty (locale))
                    locale ="false";
                this.hd_localstore.Value = locale;
                Boolean.TryParse(locale, out localStore);
                // Prima di modificare il resto modifico testleggibilita
                
                //if (file != "no")
                //{
                //    btn_avanti.Visible = false;
                //    btn_chiudi.Visible = true;
                //    hd_indice_docs.Value = null;
                    
                //    frameSrc = "docVisualizza.aspx?idC=" + idConservazione + "&file=" + file;
                //}
                //if (Int32.TryParse(this.Request.QueryString["num"], out numero))
                //{
                //    this.hd_indice_docs.Value = "1";
                //    this.hd_num_docs.Value = numero.ToString();
                //}

           


            if (!String.IsNullOrEmpty(hd_indice_docs.Value) 
                && hd_num_docs.Value != "-1")
            {
                Dictionary<String, String> documentiMemorizzati = ConservazioneWA.Utils.ConservazioneManager.getFilesFromUniSincro(infoUtente, hd_idCons.Value,localStore);

                if (Int32.TryParse(hd_indice_docs.Value, out numero))
                {
                    // Modifica Lembo 09-09-2013: Per le prestazioni, prendo tutto dall'unisincro.
                    //WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(hd_idCons.Value, infoUtente);
                    //string file = hd_files.Value;
                    //string[] files = file.Split(',');
                    int percent = (int)Math.Ceiling((double)((double)(100 * (files.Length - 1)) / documentiMemorizzati.Count));
                    hd_percent.Value = percent.ToString();
                    //foreach (WSConservazioneLocale.ItemsConservazione item in items)
                    //{
                    string uniSincroItem = documentiMemorizzati[files[numero - 1]];
                    string[] uniSincroItems = uniSincroItem.Split('§');

                        string fileType = String.Format(".{0}", uniSincroItems[2].ToString().Split('.')[uniSincroItems[2].ToString().Split('.').Length-1]);
                        //if (item.DocNumber == files[numero - 1])
                        //{
                            frameSrc = "docVisualizza.aspx?idC=" + hd_idCons.Value + "&file=" + uniSincroItem +"&ext="+ fileType+"&locale="+localStore.ToString().ToLower();
                            hd_docnumber.Value = files[numero - 1];
                        //}
                    //}
                    //frameSrc = "docVisualizza.aspx?idC=" + hd_idCons.Value + "&file=" + items[numero - 1].DocNumber /*+ items[numero - 1].tipoFile*/;
                    int numdocs = Int32.Parse(hd_num_docs.Value);
                    if (numero < numdocs)
                    {
                        numero++;
                        hd_indice_docs.Value = numero.ToString();
                    }
                    else
                    {
                        btn_avanti.Visible = false;
                        btn_chiudi.Visible = true;
                    }
                }
            }
            
            this.iframeDoc.Attributes["src"] = frameSrc;
            }
            //RadioButtonList1.SelectedIndex = -1;
            
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            string segnaturaOrId;
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            segnaturaOrId = ConservazioneWA.Utils.ConservazioneManager.getSegnatura_ID_Doc(hd_docnumber.Value);


            if (RadioButtonList1.SelectedItem == null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_selezione", "alert('Indicare se il file è leggibile o meno.');", true);

            }
            else
            {
                WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                if (RadioButtonList1.Items[1].Selected)
                {

                    risultatoLeggibilita.Value = "fallito";

                    // Modifica per inserimento Registro di conservazione caso successo di Leggibilità del documento
                    WSConservazioneLocale.RegistroCons regConsOk = new WSConservazioneLocale.RegistroCons();
                    regConsOk.idAmm = infoUtente.idAmministrazione;
                    regConsOk.idIstanza = hd_idCons.Value;
                    regConsOk.idOggetto = hd_docnumber.Value;
                    regConsOk.tipoOggetto = "D";
                    regConsOk.tipoAzione = "";
                    regConsOk.userId = infoUtente.userId;
                    regConsOk.codAzione = "LEGGIBILITA";
                    regConsOk.descAzione = "Esecuzione della verifica di leggibilità del documento " + segnaturaOrId + " dell'istanza id: " + hd_idCons.Value;
                    regConsOk.esito = "1";
                    ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regConsOk, infoUtente);

                }
                else
                {

                    // Modifica per inserimento Registro di conservazione caso fallito di Leggibilità del documento
                    WSConservazioneLocale.RegistroCons regCons = new WSConservazioneLocale.RegistroCons();
                    regCons.idAmm = infoUtente.idAmministrazione;
                    regCons.idIstanza = hd_idCons.Value;
                    regCons.idOggetto = hd_docnumber.Value;
                    regCons.tipoOggetto = "D";
                    regCons.tipoAzione = "";
                    regCons.userId = infoUtente.userId;
                    regCons.codAzione = "LEGGIBILITA";
                    regCons.descAzione = "Esecuzione della verifica di leggibilità del documento " + segnaturaOrId + " dell'istanza id: " + hd_idCons.Value;
                    regCons.esito = "1";
                    ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons, infoUtente);

                }
                                
                bool leggibilitaVerificata = (risultatoLeggibilita.Value != "fallito");
                if (risultatoLeggibilita.Value == "fallito")
                {

                    
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_fallimento", "alert('Sono presenti documenti non leggibili.');", true);
                    ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(infoUtente, hd_idCons.Value, false);
                }
                else
                {
                    if (hd_localstore.Value.ToString().ToLower() == "true")
                    {
                        this.iframeDoc.Attributes["src"] = "about:blank";
                        if (ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(infoUtente, hd_idCons.Value, true))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_successo", "alert('Verifica di leggibilità effettuata con successo.');", true);

                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_caricamento_fallito", "alert('Caricamento nello storage remoto fallito.');", true);
                    
                    }
                    //Modifica: bisogna notificare la verifica leggibilità effettuata prima del caricamento su storage remoto.
                    //dato che locale è true solo nella verifica antecedente il caricamento, lo usiamo per richiamare la popup
                    //if (hd_localstore.Value.ToString().ToLower()=="true")
                    //{
                    //    Session["timer_chiusura"] = "no";
                    //    ScriptManager.RegisterStartupScript(this.Page,this.GetType(),"show_chiusura_istanza","showChiusuraIstanza('"+hd_idCons.Value+"');",true);
                    //}
                    else
                    {
                        if (ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(infoUtente, hd_idCons.Value, true))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_successo", "alert('Verifica di leggibilità effettuata con successo.');", true);
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_fallito", "alert('Errore in database.');", true);
                    }
                }
                // MEV CS 1.5
                // notifiche scadenza verifiche leggibilità
                // nella tabella DPA_SUPPORTO devo tenere separate le info sulle
                // verifiche di leggibilità da quelle di integrità
                if (TipoVerifica != "Verifica per chiusura")
                {
                    if (!string.IsNullOrEmpty(this.IdSupporto))
                    {
                        //Utils.ConservazioneManager.RegistraEsitoVerificaSupportoRegistrato(
                        //                   (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                        //                   hd_idCons.Value,
                        //                   this.IdSupporto,
                        //                   leggibilitaVerificata,
                        //                   hd_percent.Value,
                        //                   this.DataProssimaVerifica,
                        //                   this.TipoVerifica + this.NoteVerifica,
                        //                   this.codiceVerifica);
                        Utils.ConservazioneManager.RegistraEsitoVerificaLeggibilitaSupportoRegistrato(
                                            (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                                            hd_idCons.Value,
                                            this.IdSupporto,
                                            leggibilitaVerificata,
                                            hd_percent.Value,
                                            this.DataProssimaVerifica,
                                            this.TipoVerifica + this.NoteVerifica,
                                            this.codiceVerifica);
                    }
                    else
                    {
                        //Utils.ConservazioneManager.RegistraEsitoVerificaSupportoRegistrato(
                        //                   (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                        //                   hd_idCons.Value,
                        //                   FetchIdSupporto(),
                        //                   leggibilitaVerificata,
                        //                   hd_percent.Value,
                        //                   FetchDataProssimaVerifica(),
                        //                   this.TipoVerifica + "verifica di un file singolo",
                        //                   this.codiceVerifica);
                        Utils.ConservazioneManager.RegistraEsitoVerificaLeggibilitaSupportoRegistrato(
                                            (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                                            hd_idCons.Value,
                                            FetchIdSupporto(),
                                            leggibilitaVerificata,
                                            hd_percent.Value,
                                            FetchDataProssimaVerifica(),
                                            this.TipoVerifica + "verifica di un file singolo",
                                            this.codiceVerifica);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.IdSupporto))
                    {
                        //Utils.ConservazioneManager.RegistraEsitoVerificaSupportoRegistrato(
                        //                   (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                        //                   hd_idCons.Value,
                        //                   this.IdSupporto,
                        //                   leggibilitaVerificata,
                        //                   hd_percent.Value,
                        //                   this.DataProssimaVerifica,
                        //                   "Verifica Leggibilità per chiusura",
                        //                   this.codiceVerifica);
                        Utils.ConservazioneManager.RegistraEsitoVerificaLeggibilitaSupportoRegistrato(
                                            (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                                            hd_idCons.Value,
                                            this.IdSupporto,
                                            leggibilitaVerificata,
                                            hd_percent.Value,
                                            this.DataProssimaVerifica,
                                            "Verifica Leggibilità per chiusura",
                                            this.codiceVerifica);
                    }
                    else
                    {
                        //Utils.ConservazioneManager.RegistraEsitoVerificaSupportoRegistrato(
                        //                   (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                        //                   hd_idCons.Value,
                        //                   FetchIdSupporto(),
                        //                   leggibilitaVerificata,
                        //                   hd_percent.Value,
                        //                   FetchDataProssimaVerifica(),
                        //                   "Verifica Leggibilità per chiusura",
                        //                   this.codiceVerifica);
                        Utils.ConservazioneManager.RegistraEsitoVerificaLeggibilitaSupportoRegistrato(
                                           (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                                            hd_idCons.Value,
                                            FetchIdSupporto(),
                                            leggibilitaVerificata,
                                            hd_percent.Value,
                                            FetchDataProssimaVerifica(),
                                            "Verifica Leggibilità per chiusura",
                                            this.codiceVerifica);
                    }
                }
                
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_selezione", "window.close();", true);

                //Response.Write("<script>window.close();</script>");
            }
        }

        protected string IdSupporto
        {
            get
            {
                return this.Request.QueryString["idSupporto"];
            }
        }

        protected string DataProssimaVerifica
        {
            get
            {
                return this.Request.QueryString["dataProx"];
            }
        }

        protected string NoteVerifica
        {
            get
            {
                return this.Request.QueryString["note"];
            }
        }

        protected string codiceVerifica
        {
            get
            {
                if (this.Request.QueryString["num"] == "0") return "L";
                else if (this.Request.QueryString["num"] == "1") return "U";
                else if (this.Request.QueryString["num"] == "2") return "C";
                else return "";
            }
        }

        protected string TipoVerifica
        {
            get
            {
                if (this.Request.QueryString["num"] == "0") return "Verifica Leggibilità: ";
                // MEV CS 1.5
                // tengo traccia nella storia delle verifiche dell'operazione effettuata
                //else if (this.Request.QueryString["num"] == "1") return "Verifica unificata: ";
                else if (this.Request.QueryString["num"] == "1") return "Verifica unificata (Leggibilità): ";
                // end MEV CS 1.5
                else if (this.Request.QueryString["num"] == "2") return "Verifica per chiusura";
                else return "";
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
            WSConservazioneLocale.Policy policy = Utils.ConservazioneManager.GetPolicyIstanza((WSConservazioneLocale.InfoUtente)Session["infoutCons"], this.hd_idCons.Value);
            
            //MEV CS 1.5 - Alert Conservazione
            //La data di prossima verifica è stabilita a partire dal parametro di configurazione definito in amministrazione
            //il valore è definito in GIORNI
            string idAmministrazione = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]).idAmministrazione;
            int intervalloProssimaVerifica = Convert.ToInt32(Utils.ConservazioneManager.GetChiaveConfigurazione(idAmministrazione, "BE_CONS_VER_LEG_INTERVALLO"));

            //int mesiDefault = 6;
            if (policy != null)
            {
                // Istanza creata tramite policy: impostazione del numero di mesi definiti in essa
                int mesiPolicy;
                Int32.TryParse(policy.avvisoMesiLegg, out mesiPolicy);
                if (mesiPolicy == 0)
                {
                    //mesiPolicy = mesiDefault;
                    return DateTime.Today.AddDays(intervalloProssimaVerifica).ToString("dd/MM/yyyy");
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

                return DateTime.Today.AddDays(intervalloProssimaVerifica).ToString("dd/MM/yyyy");
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
            string filters = " A.ID_CONSERVAZIONE= '" + this.hd_idCons.Value + "'  AND  ( A.ID_TIPO_SUPPORTO = (SELECT system_id from dpa_tipo_supporto where var_tipo='REMOTO') ) and a.ID_CONSERVAZIONE = c.SYSTEM_ID and c.id_amm = " + ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]).idAmministrazione + " ORDER BY A.ID_CONSERVAZIONE DESC, A.COPIA ASC";
            WSConservazioneLocale.InfoSupporto[] supporti = ConservazioneWA.Utils.ConservazioneManager.getInfoSupporto(filters, (WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            return supporti.FirstOrDefault().SystemID;
        }

        protected void btn_avanti_Click(object sender, EventArgs e)
        {
            string descrizione, esito, segnaturaOrId;

            //DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();

            segnaturaOrId = ConservazioneWA.Utils.ConservazioneManager.getSegnatura_ID_Doc(hd_docnumber.Value);

            if (RadioButtonList1.SelectedItem ==  null)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_selezione", "alert('Indicare se il file è leggibile o meno.');", true);
                
            }
            else
            {
                if (RadioButtonList1.Items[1].Selected)
                {
                    risultatoLeggibilita.Value = "fallito";
                    //ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroControlli(hd_idCons.Value,docnumber,infoUtente,"Verifica Leggibilità",RadioButtonList1.Items[1].Selected,0,0,0);
                    esito = "0";
                    descrizione = "Esecuzione della verifica di leggibilità del documento" + segnaturaOrId + "dell'istanza "+ hd_idCons.Value;
                }

                else
                {
                    descrizione = "Esecuzione della verifica di leggibilità del documento " + segnaturaOrId + "dell'istanza " + hd_idCons.Value; ;
                    esito = "1";
                }

                // Modifica Registro di Conservazione Leggibilità e Log
                WSConservazioneLocale.InfoUtente infoUtente2 = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                WSConservazioneLocale.RegistroCons regCons2 = new WSConservazioneLocale.RegistroCons();
                regCons2.idAmm = infoUtente2.idAmministrazione;
                regCons2.idIstanza = hd_idCons.Value;
                regCons2.idOggetto = hd_docnumber.Value;
                regCons2.tipoOggetto = "D";
                regCons2.tipoAzione = "";
                regCons2.userId = infoUtente2.userId;
                regCons2.codAzione = "LEGGIBILITA";
                regCons2.descAzione = descrizione;
                regCons2.esito = esito;
                ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons2, infoUtente2);

                WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroControlli(hd_idCons.Value, segnaturaOrId, infoUtente, "Verifica Leggibilità", RadioButtonList1.Items[0].Selected, 0, 0, 0);

                string frameSrc = "";
                //Page_Load(this, e);
                if (!String.IsNullOrEmpty(hd_indice_docs.Value)
                    && hd_num_docs.Value != "0")
                {
                    Dictionary<String, String> documentiMemorizzati = ConservazioneWA.Utils.ConservazioneManager.getFilesFromUniSincro(infoUtente, hd_idCons.Value,Boolean.Parse(hd_localstore.Value));

                    if (Int32.TryParse(hd_indice_docs.Value, out numero))
                    {
                        //WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(hd_idCons.Value, infoUtente);
                        string file = hd_files.Value;
                        string[] files = file.Split(',');
                        // Modifica Lembo 09-09-2013: Per le prestazioni, prendo tutto dall'unisincro.
                        //WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(hd_idCons.Value, infoUtente);
                        string uniSincroItem = documentiMemorizzati[files[numero - 1]];
                        string[] uniSincroItems = uniSincroItem.Split('§');

                        string fileType = String.Format(".{0}", uniSincroItems[2].ToString().Split('.')[uniSincroItems[2].ToString().Split('.').Length - 1]);
                        //if (item.DocNumber == files[numero - 1])
                        //{
                        frameSrc = "docVisualizza.aspx?idC=" + hd_idCons.Value + "&file=" + uniSincroItem + "&ext=" + fileType + "&locale=" + hd_localstore.Value;
                        hd_docnumber.Value = files[numero - 1];
                        //}
                        //}
                        //foreach (WSConservazioneLocale.ItemsConservazione item in items)
                        //{
                        //    string uniSincroItem = documentiMemorizzati[item.DocNumber];
                        //    if (item.DocNumber == files[numero - 1])
                        //    {
                        //        frameSrc = "docVisualizza.aspx?idC=" + hd_idCons.Value + "&file=" + uniSincroItem + "&ext=" + item.tipoFile + "&locale=" + hd_localstore.Value;
                        //        hd_docnumber.Value = item.DocNumber;
                        //    }
                        //}
                        //frameSrc = "docVisualizza.aspx?idC=" + hd_idCons.Value + "&file=" + items[numero - 1].DocNumber /*+ items[numero - 1].tipoFile*/;
                        int numdocs = Int32.Parse(hd_num_docs.Value);
                        if (numero < numdocs)
                        {
                            numero++;
                            hd_indice_docs.Value = numero.ToString();
                        }
                        else
                        {
                            btn_avanti.Visible = false;
                            btn_chiudi.Visible = true;
                        }
                    }
                }

                this.iframeDoc.Attributes["src"] = frameSrc;
                RadioButtonList1.SelectedIndex = -1;
            }
        }
        

        
    }
}