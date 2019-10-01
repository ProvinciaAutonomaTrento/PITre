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
using ConservazioneWA.Utils;
using System.Drawing;
using System.Net.Mail;
using Debugger = ConservazioneWA.Utils.Debugger;
using ConservazioneWA.DocsPaWR;
using System.Globalization;
using System.IO;

namespace ConservazioneWA.Esibizione
{
    public partial class RicercaIstanzeConsEsibizione : System.Web.UI.Page
    {
        protected ConservazioneWA.DataSet.DataSetRicerca dataSetRicerca = new ConservazioneWA.DataSet.DataSetRicerca();
        protected ConservazioneWA.DataSet.DataSetRDettaglio dataSetDettaglio = new ConservazioneWA.DataSet.DataSetRDettaglio();
        protected WSConservazioneLocale.InfoConservazione[] infoCons;
        protected WSConservazioneLocale.ItemsConservazione[] itemsCons;
        protected WSConservazioneLocale.InfoUtente infoUtente;
        protected WSConservazioneLocale.InfoAmministrazione amm;

        private ConservazioneWA.WSConservazioneLocale.TipoIstanzaConservazione[] _tipiIstanze = null;

        private ConservazioneWA.WSConservazioneLocale.StatoIstanza[] _statiIstanze = null;

        /// <summary>
        /// 
        /// </summary>
        // Non dovrebbe essere invocato
        private void MettiInLavorazioneAsync()
        {
            // Se il campo nascosto "hd_istanza_in_deroga" ha valore true,
            // significa che l'istanza sarà creata in deroga alle policy,
            // pertanto sarà inserita una nota informativa
            bool istanzaInDeroga = this.IstanzaInDeroga;

            string noteIstanza = string.Empty;

            if (istanzaInDeroga)
                noteIstanza = "Istanza creata in deroga alle policy sui formati file";

            //this.hd_istanza_in_deroga.Value = string.Empty;
            this.IstanzaInDeroga = false;

            int copieSupportiRimovibili = 0;

            if (Session["MotivoMessaInLavorazione"] != null)
            {
                string motivo = Session["MotivoMessaInLavorazione"].ToString();
                ConservazioneManager.inserimentoInRegistroControlli(this.hd_idIstanza.Value, null, this.infoUtente, "MOTIVO_MESSA_IN_LAVORAZIONE§" + motivo, true, 0, 0, 0);
                Session["MotivoMessaInLavorazione"] = null;
            }

            if (Session["copie"] != null)
                Int32.TryParse(Session["copie"].ToString(), out copieSupportiRimovibili);

            // Avvio del task di metti in lavorazione
            ConservazioneManager.MettiInLavorazioneAsync(this.infoUtente, this.hd_idIstanza.Value, noteIstanza, copieSupportiRimovibili);

            this.hd_lavorazione.Value = null;

            //// L''istanza è in fase di preparazione. Premere il pulsante ''Aggiorna'' per verificare lo stato di elaborazione.
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + this.lblStatoInLavorazione.Text.Replace("'", "''") + "');", true);
            btn_lavorazione.Enabled = false;
            this.RefreshControlsInPreparazione(true);
        }

        // Non dovrebbe essere invocato
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAggiornaStatoInLavorazione_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {

            if (this.gv_istanzeCons.SelectedIndex != -1)
            {
                try
                {
                    if (this.gv_istanzeCons.SelectedItem != null)
                    {
                        string stato = ((Label)this.gv_istanzeCons.SelectedItem.FindControl("lb_statoIstanza")).Text.Trim();
                        if (stato == "Nuova")
                        {
                            if (ConservazioneManager.abilitaLavorazione(this.hd_idIstanza.Value) && !this.pnlStatoInLavorazione.Visible)
                                this.btn_lavorazione.Enabled = true;
                            else
                                this.btn_lavorazione.Enabled = false;

                        }
                    }
                }
                catch (Exception ex) { }
            }
            gestisciClassiPulsanti();

            //
            // Mev CS 1.4 Esibizione
            if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
            {
                // Recupero l'idGruppo selezionato
                //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                string idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
                DocsPaWR.Utente user = (DocsPaWR.Utente)Session["userData"];

                if (user != null)
                {
                    //Recupero il ruolo selezionato a partire dall'idGruppo
                    Ruolo ruoloSelected = user.ruoli.FirstOrDefault(item => item.idGruppo == idGruppo);
                    //Ruolo ruoloSelected = (Ruolo)Session["RuoloSelezionato"];

                    if (ruoloSelected != null)
                    {
                        lbl_selectedRole.Text = ruoloSelected.descrizione;
                    }
                }

                // Rimetto in sessione il valore dell'idGruppo utilizzato dall'utente
                HttpContext.Current.Session["RuoloSelezionato_idGruppo"] = idGruppo;
            }
            // End Mev CS 1.4 Esibizione
            //
        }

        /// <summary>
        /// Metodo di prova MEV CS 1.4 - Esibizione. Da eliminare al termine della MEV
        /// </summary>
        /// <returns></returns>
        //public string CalcolaProfiloUtente()
        //{
        //    string ProfiloUtente = string.Empty;
        //    bool AbilitatoCentroServizi = false;
        //    bool AbilitatoEsibizione = false;

        //    // Chiamata al Backend che calcola il profilo Utente a partire dall'infoutente che si è loggato
        //    // passando i seguenti parametri: this.infoUtente.idAmministrazione, this.infoUtente.idPeople

        //    // Simulo l'esito della chiamata
        //    try
        //    {
        //        AbilitatoEsibizione = System.Configuration.ConfigurationManager.AppSettings["ABILITATOESIBIZIONE"].Equals("ESIBIZIONE") ? true : false;
        //    }
        //    catch (Exception e) 
        //    {
        //        AbilitatoEsibizione = false;
        //    }

        //    // Controllo flag
        //    if (AbilitatoCentroServizi)
        //    {
        //        ProfiloUtente = "CONSERVAZIONE";
        //    }

        //    if (AbilitatoEsibizione)
        //    {
        //        ProfiloUtente = "ESIBIZIONE";
        //    }

        //    return ProfiloUtente;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            
            // PROVA
            //menuTop.ProfiloUtente = CalcolaProfiloUtente();
            //menuTop.ProfiloUtente = ConservazioneManager.CalcolaProfiloUtente(this.infoUtente.idPeople, this.infoUtente.idAmministrazione);
            menuTop.ProfiloUtente = "ESIBIZIONE";
            // END PROVA

            _tipiIstanze = ConservazioneManager.GetTipiIstanza();
            _statiIstanze = ConservazioneManager.GetStatiIstanza();

            if (!IsPostBack)
            {
                this.GestioneGrafica();

                this.CaricaDropDownListPolicy();

                this.FetchTipiConservazione();

                if (Request.QueryString["q"] != null && !string.IsNullOrEmpty(Request.QueryString["q"].ToString()))
                {
                    string filtro = Request.QueryString["q"].ToString();
                    string query = " WHERE CHA_STATO='" + filtro + "' and id_amm = " + this.infoUtente.idAmministrazione;
                    this.infoCons = ConservazioneManager.getAreaConservazioneFiltro(query, infoUtente);
                    if (this.infoCons != null)
                    {
                        this.caricaGridviewIstanze(this.infoCons);
                    }
                    this.chkTipo.SelectedValue = filtro;
                }
                if (Request.QueryString["id"] != null && !string.IsNullOrEmpty(Request.QueryString["id"].ToString()))
                {
                    string idIstanza = Request.QueryString["id"].ToString();
                    string query = " WHERE SYSTEM_ID= " + idIstanza + " and id_amm = " + this.infoUtente.idAmministrazione;
                    infoCons = ConservazioneManager.getAreaConservazioneFiltro(query, infoUtente);
                    if (infoCons != null)
                    {
                        this.caricaGridviewIstanze(infoCons);
                    }
                    this.txt_idIstanza.Text = idIstanza;
                    //itemsCons = ConservazioneManager.getItemsConservazione(idIstanza, infoUtente);
                    //
                    // MEV CS 1.4 - Esibizione
                    string idGruppo = string.Empty;
                    // Recupero dalla sessione l'idGruppo selezionato
                    if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
                        // Recupero l'idGruppo selezionato
                        //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                        idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
                    itemsCons = ConservazioneManager.getItemsConservazioneWithSecurity(idIstanza, infoUtente, idGruppo);
                    // End Mev Cs 1.4 - Esibizione
                    //

                    if (itemsCons != null)
                    {
                        this.hd_idIstanza.Value = idIstanza;
                        this.caricaGridviewDettaglio(itemsCons);


                        abilitaFunzioniByCodStato(infoCons[0].StatoConservazione);

                        if (!string.IsNullOrEmpty(infoCons[0].idPolicyValidata))
                        {
                            if (this.ddl_policy.Items.FindByValue(infoCons[0].idPolicyValidata) != null)
                            {
                                this.ddl_policy.SelectedValue = infoCons[0].idPolicyValidata;
                            }
                        }

                        this.Panel_dettaglio.Visible = true;
                    }
                }
            }

            //se l'istanza viene messa in lavorazione
            if (this.hd_lavorazione != null && this.hd_lavorazione.Value != String.Empty && this.hd_lavorazione.Value != "undefined")
            {
                this.MettiInLavorazioneAsync();
            }
        }


        protected void caricaGridviewIstanze(WSConservazioneLocale.InfoConservazione[] infoCons)
        {
            this.dataSetRicerca = new ConservazioneWA.DataSet.DataSetRicerca();
            for (int i = 0; i < infoCons.Length; i++)
            {
                this.dataSetRicerca.tbl_ricerca.Addtbl_ricercaRow(infoCons[i].TipoConservazione, infoCons[i].Note, infoCons[i].Descrizione, infoCons[i].TipoSupporto, infoCons[i].Data_Apertura, this.convertDate(infoCons[i].Data_Invio),
                   this.convertDate(infoCons[i].Data_Conservazione), convertDate(infoCons[i].Data_Riversamento), this.convertDate(infoCons[i].Data_Prox_Verifica), this.convertDate(infoCons[i].Data_Ultima_Verifica), infoCons[i].MarcaTemporale, infoCons[i].FirmaResponsabile,
                    infoCons[i].LocazioneFisica, infoCons[i].StatoConservazione, infoCons[i].SystemID, infoCons[i].numCopie, infoCons[i].userID + "<br>" + "(" + infoCons[i].IdGruppo + ")", infoCons[i].userID, infoCons[i].idPolicyValidata, infoCons[i].IstanzaInPreparazione, infoCons[i].validationMask.ToString());
            }
            gv_istanzeCons.DataSource = this.dataSetRicerca.Tables[0];

            //
            // Andrea - 31-05-2013
            // Se sono sull'ultimo elemento dell'ultima pagina, 
            // quando lo rimuovo imposto come pagina corrente la prima pagina
            if (gv_istanzeCons.CurrentPageIndex == gv_istanzeCons.PageCount)
            {
                gv_istanzeCons.CurrentPageIndex = 0;
            }
            // End Andrea
            //

            gv_istanzeCons.DataBind();
            Session["istanza"] = infoCons;
            this.Panel_dettaglio.Visible = false;
            this.upDettaglio.Update();
        }

        protected string convertDate_orig(string oldDate)
        {

            string newDate = string.Empty;
            try
            {
                newDate = (Convert.ToDateTime(oldDate)).ToShortDateString();
            }
            catch (Exception e)
            {

            }
            return newDate;

        }

        protected string convertDate(string oldDate)
        {

            return oldDate;
        }

        protected string convertDate_old1(string date)
        {
            CultureInfo culture = new CultureInfo("it-IT");
            DateTime newDateTime;
            ////*** Correzione del bug sul formato dell'ora su alcuni SO ---
            date = date.Replace(":", ".");
            DateTime dateOut = new DateTime();
            string[] formati = { "dd/MM/yyyy" };
            if (DateTime.TryParseExact(date, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out dateOut))
            {
                newDateTime = DateTime.ParseExact(date, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                ////*** Fine correzione ---
            }
            else
            {
                date = date.Replace(".", ":");
                newDateTime = Convert.ToDateTime(date, culture);
            }
            date = date.Trim();

            return newDateTime.ToShortDateString();


            if (date.Length <= 10)
            {
                return newDateTime.ToShortDateString();
            }
            else
            {
                string data = newDateTime.ToShortDateString();
                string ora = newDateTime.ToString("HH:mm").Replace(".", ":");
                return data + " " + ora;

            }
        }


        protected void caricaGridviewDettaglio(WSConservazioneLocale.ItemsConservazione[] itemsCons)
        {

            this.dataSetDettaglio = new ConservazioneWA.DataSet.DataSetRDettaglio();
            float size = 0;
            for (int i = 0; i < itemsCons.Length; i++)
            {
                string segnatura = itemsCons[i].numProt_or_id;
                string data = itemsCons[i].data_prot_or_create;
                string data_doc = segnatura + "<br />" + data;
                this.dataSetDettaglio.dt_dettaglio_cons.Adddt_dettaglio_consRow(
                                itemsCons[i].DocNumber, itemsCons[i].ID_Profile, itemsCons[i].TipoDoc,
                                itemsCons[i].desc_oggetto, itemsCons[i].CodFasc, this.convertDate(itemsCons[i].Data_Ins),
                                data_doc, itemsCons[i].SizeItem, itemsCons[i].numProt, itemsCons[i].tipoFile, itemsCons[i].numAllegati,
                                itemsCons[i].invalidFileFormat,
                                itemsCons[i].esitoValidazioneFirma.ToString(), itemsCons[i].policyValida, itemsCons[i].SizeItem, itemsCons[i].Check_Mask_Policy,itemsCons[i].ID_Project);

                if (!string.IsNullOrEmpty(itemsCons[i].SizeItem))
                {
                    size = size + Convert.ToSingle(itemsCons[i].SizeItem);
                }
            }

            try
            {
                float sizeF = (float)size / 1048576;
                string size_appo = Convert.ToString(sizeF);
                if (size_appo.Contains(","))
                {
                    size_appo = size_appo.Substring(0, size_appo.IndexOf(",") + 2);
                }
                else
                {
                    if (size_appo.Contains("."))
                    {
                        size_appo = size_appo.Substring(0, size_appo.IndexOf(".") + 2);
                    }
                }

                //this.lblDimesioneIstanza.Text = string.Format("Numero Documenti: {2} - Totale Bytes: {0:n0} - Totale MB.: {1}", size, size_appo, itemsCons.Length.ToString());
                //this.lblDimesioneIstanza.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);
                //
                // Mev CS 1.4 - Esibizione
                if (itemsCons != null && itemsCons.Length > 0)
                {
                    this.lblDimesioneIstanza.Text = string.Format("Numero Documenti Visibili per l'utente: {2} - Totale Bytes: {0:n0} - Totale MB.: {1}", size, size_appo, itemsCons.Length.ToString());
                    this.lblDimesioneIstanza.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);
                }
                else
                {
                    this.lblDimesioneIstanza.Text = "Nessun Documento Visibile per l'utente";
                    this.lblDimesioneIstanza.ForeColor = Color.Red;
                }
                // End Mev cs 1.4 - Esibizione
                //
                

                //InfoUtente infoUt = new InfoUtente();

                //infoUt.codWorkingApplication = infoUtente.codWorkingApplication;
                //infoUt.idAmministrazione = infoUtente.idAmministrazione;
                //infoUt.idGruppo = infoUtente.idGruppo;
                //infoUt.idPeople = infoUtente.idPeople;


                if (this.hd_statoIstanza.Value == "Nuova" && !ConservazioneManager.dimensioneValida(itemsCons.FirstOrDefault().ID_Conservazione, infoUtente))
                {
                    this.lblDimesioneIstanza.Text += ConservazioneManager.getErrorMaxDimensioneIstanza(itemsCons.FirstOrDefault().ID_Conservazione);
                    this.lblDimesioneIstanza.ForeColor = System.Drawing.Color.Red;
                }

            }
            catch (Exception e)
            {
                Debugger.Write("Errore nel calcolo del size del documento: " + e.Message);
            }

            gv_dettaglioCons.DataSource = this.dataSetDettaglio.Tables[0];
            gv_dettaglioCons.DataBind();
            Session["itemsCons"] = itemsCons;
        }

        protected void gv_dettaglioCons_DataBound(object sender, EventArgs e)
        {

            
        }

        protected void abilitaFunzioni(string stato)
        {
            if (stato == "Nuova")
            {
                this.btn_rifiuta.Enabled = true;

                if (ConservazioneManager.abilitaLavorazione(this.hd_idIstanza.Value))
                    this.btn_lavorazione.Enabled = true;
                else
                    this.btn_lavorazione.Enabled = false;

                this.btn_verificaFirma.Enabled = true;
                this.btn_firma.Enabled = false;
                this.btn_standBy.Enabled = false;
                this.btn_verificaLeggibilita.Enabled = false;
            }
            else if (stato == "In Lavorazione")
            {
                this.btn_rifiuta.Enabled = false;
                this.btn_lavorazione.Enabled = false;
                this.btn_verificaFirma.Enabled = false;
                this.btn_firma.Enabled = true;
                this.btn_standBy.Enabled = false;
                this.btn_verificaLeggibilita.Enabled = false;

            }
            else if (stato == "Firmata" || stato == "Conservata")
            {

                this.btn_lavorazione.Enabled = false;
                this.btn_verificaFirma.Enabled = false;
                this.btn_firma.Enabled = false;
                this.btn_standBy.Enabled = false;
                if (stato == "Firmata")
                {
                    this.btn_verificaLeggibilita.Enabled = true;
                    this.btn_rifiuta.Enabled = true;
                }
                else
                {
                    this.btn_verificaLeggibilita.Enabled = false;
                    this.btn_rifiuta.Enabled = false;
                }
            }
            //else if (stato == "Rifiutata" || stato == "In Transizione" || stato == "In fase di verifica")
            else if (stato == "Rifiutata" || stato == "In Chiusura" || stato == "In fase di verifica")
            {
                this.btn_rifiuta.Enabled = false;
                this.btn_lavorazione.Enabled = false;
                this.btn_verificaFirma.Enabled = false;
                this.btn_firma.Enabled = false;
                this.btn_standBy.Enabled = false;
                this.btn_verificaLeggibilita.Enabled = false;
            }
            else if (stato == "Chiusa")
            {
                this.btn_rifiuta.Enabled = false;
                this.btn_lavorazione.Enabled = false;
                this.btn_verificaFirma.Enabled = false;
                this.btn_firma.Enabled = false;
                this.btn_standBy.Enabled = false;
                this.btn_verificaLeggibilita.Enabled = false;
            }
            else if (stato == "Rigenera marca")
            {
                this.btn_rifiuta.Enabled = false;
                this.btn_lavorazione.Enabled = false;
                this.btn_verificaFirma.Enabled = false;
                this.btn_firma.Enabled = false;
                this.btn_standBy.Enabled = true;
                this.btn_verificaLeggibilita.Enabled = false;
            }
            gestisciClassiPulsanti();
        }

        private void gestisciClassiPulsanti()
        {

            if (btn_firma.Enabled)
            {
                btn_firma.Attributes.Remove("class");
                btn_firma.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_firma.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_firma.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_firma.Attributes.Remove("onmouseover");
                btn_firma.Attributes.Remove("onmouseout");
                btn_firma.Attributes.Remove("class");
                btn_firma.Attributes.Add("class", "cbtnDisabled");
            }
            if (btn_standBy.Enabled)
            {
                btn_standBy.Attributes.Remove("class");
                btn_standBy.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_standBy.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_standBy.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_standBy.Attributes.Remove("onmouseover");
                btn_standBy.Attributes.Remove("onmouseout");
                btn_standBy.Attributes.Remove("class");
                btn_standBy.Attributes.Add("class", "cbtnDisabled");
            }
            if (btn_rifiuta.Enabled)
            {
                btn_rifiuta.Attributes.Remove("class");
                btn_rifiuta.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_rifiuta.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_rifiuta.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_rifiuta.Attributes.Remove("onmouseover");
                btn_rifiuta.Attributes.Remove("onmouseout");
                btn_rifiuta.Attributes.Remove("class");
                btn_rifiuta.Attributes.Add("class", "cbtnDisabled");
            }
            if (btn_lavorazione.Enabled)
            {
                btn_lavorazione.Attributes.Remove("class");
                btn_lavorazione.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_lavorazione.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_lavorazione.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_lavorazione.Attributes.Remove("onmouseover");
                btn_lavorazione.Attributes.Remove("onmouseout");
                btn_lavorazione.Attributes.Remove("class");
                btn_lavorazione.Attributes.Add("class", "cbtnDisabled");
            }
            if (btn_verificaLeggibilita.Enabled)
            {
                btn_verificaLeggibilita.Attributes.Remove("class");
                btn_verificaLeggibilita.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_verificaLeggibilita.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_verificaLeggibilita.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_verificaLeggibilita.Attributes.Remove("onmouseover");
                btn_verificaLeggibilita.Attributes.Remove("onmouseout");
                btn_verificaLeggibilita.Attributes.Remove("class");
                btn_verificaLeggibilita.Attributes.Add("class", "cbtnDisabled");
            }
        }

        protected void abilitaFunzioniByCodStato(string stato)
        {
            if (stato == "I")       //Inviata
            {
                this.btn_rifiuta.Enabled = true;
                if (ConservazioneManager.abilitaLavorazione(this.hd_idIstanza.Value))
                    this.btn_lavorazione.Enabled = true;
                else
                    this.btn_lavorazione.Enabled = true;
                this.btn_verificaFirma.Enabled = true;
                this.btn_firma.Enabled = false;
                this.btn_standBy.Enabled = false;
                this.pnl_bottoniera.Visible = true;
                this.btn_verificaLeggibilita.Enabled = false;
            }
            else if (stato == "L")      //In lavorazione
            {
                this.btn_rifiuta.Enabled = false;
                this.btn_lavorazione.Enabled = false;
                this.btn_verificaFirma.Enabled = false;
                this.btn_firma.Enabled = true;
                this.btn_standBy.Enabled = false;
                this.btn_verificaLeggibilita.Enabled = false;
            }
            else if (stato == "F" || stato == "V")      //Firmata -  Conservata
            {
                this.btn_rifiuta.Enabled = false;
                this.btn_lavorazione.Enabled = false;
                this.btn_verificaFirma.Enabled = false;
                this.btn_firma.Enabled = false;
                this.btn_standBy.Enabled = false;
                //solo per lo stato firmata (ma non acora conservata)
                //si accende la verifica di leggibilità e l'enventuale rifiuto

                if (stato == "F")
                {
                    this.btn_verificaLeggibilita.Enabled = true;
                    this.btn_rifiuta.Enabled = true;
                }
                else
                {
                    this.btn_verificaLeggibilita.Enabled = false;
                    this.btn_rifiuta.Enabled = false;
                }

            }
            else if (stato == "R" || stato == "T" || stato == "Q")      //Rifutata, In Transizione o In fase di verifica
            {
                this.btn_rifiuta.Enabled = false;
                this.btn_lavorazione.Enabled = false;
                this.btn_verificaFirma.Enabled = false;
                this.btn_firma.Enabled = false;
                this.btn_standBy.Enabled = false;
                this.btn_verificaLeggibilita.Enabled = false;
            }
            else if (stato == "C")      //Chiusa
            {
                this.btn_rifiuta.Enabled = false;
                this.btn_lavorazione.Enabled = false;
                this.btn_verificaFirma.Enabled = false;
                this.btn_firma.Enabled = false;
                this.btn_standBy.Enabled = false;
                this.btn_verificaLeggibilita.Enabled = false;
            }
            else if (stato == "M")      //Rigenera Marca
            {
                this.btn_rifiuta.Enabled = false;
                this.btn_lavorazione.Enabled = false;
                this.btn_verificaFirma.Enabled = false;
                this.btn_firma.Enabled = false;
                this.btn_standBy.Enabled = true;
                this.btn_verificaLeggibilita.Enabled = false;
            }

            if (btn_firma.Enabled)
            {
                btn_firma.Attributes.Remove("class");
                btn_firma.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_firma.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_firma.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_firma.Attributes.Remove("onmouseover");
                btn_firma.Attributes.Remove("onmouseout");
                btn_firma.Attributes.Remove("class");
                btn_firma.Attributes.Add("class", "cbtnDisabled");
            }

            if (btn_standBy.Enabled)
            {
                btn_standBy.Attributes.Remove("class");
                btn_standBy.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_standBy.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_standBy.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_standBy.Attributes.Remove("onmouseover");
                btn_standBy.Attributes.Remove("onmouseout");
                btn_standBy.Attributes.Remove("class");
                btn_standBy.Attributes.Add("class", "cbtnDisabled");
            }

            if (btn_rifiuta.Enabled)
            {
                btn_rifiuta.Attributes.Remove("class");
                btn_rifiuta.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_rifiuta.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_rifiuta.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_rifiuta.Attributes.Remove("onmouseover");
                btn_rifiuta.Attributes.Remove("onmouseout");
                btn_rifiuta.Attributes.Remove("class");
                btn_rifiuta.Attributes.Add("class", "cbtnDisabled");
            }

            if (btn_lavorazione.Enabled)
            {
                btn_lavorazione.Attributes.Remove("class");
                btn_lavorazione.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_lavorazione.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_lavorazione.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_lavorazione.Attributes.Remove("onmouseover");
                btn_lavorazione.Attributes.Remove("onmouseout");
                btn_lavorazione.Attributes.Remove("class");
                btn_lavorazione.Attributes.Add("class", "cbtnDisabled");
            }
            if (btn_verificaLeggibilita.Enabled)
            {
                btn_verificaLeggibilita.Attributes.Remove("class");
                btn_verificaLeggibilita.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_verificaLeggibilita.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_verificaLeggibilita.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_verificaLeggibilita.Attributes.Remove("onmouseover");
                btn_verificaLeggibilita.Attributes.Remove("onmouseout");
                btn_verificaLeggibilita.Attributes.Remove("class");
                btn_verificaLeggibilita.Attributes.Add("class", "cbtnDisabled");
            }
        }

        protected void gv_istanzeCons_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        protected string Filtra()
        {
            string query = string.Empty;
            string[] queryStato = new string[9];
            string queryPar = "";
            int dim = 0;

            if (this.chkTipo.Items[0].Selected)
            {
                queryStato[dim] = this.chkTipo.Items[0].Value;
                dim++;
            }

            if (this.chkTipo.Items[1].Selected)
            {
                queryStato[dim] = this.chkTipo.Items[1].Value;
                dim++;
            }

            //
            // Mev CS 1.4 - Esibizione
            // Commentato poichè le altre check sono state tolte
            //if (this.chkTipo.Items[2].Selected)
            //{
            //    queryStato[dim] = this.chkTipo.Items[2].Value;
            //    dim++;
            //}

            //if (this.chkTipo.Items[3].Selected)
            //{
            //    queryStato[dim] = this.chkTipo.Items[3].Value;
            //    dim++;
            //}

            //if (this.chkTipo.Items[4].Selected)
            //{
            //    queryStato[dim] = this.chkTipo.Items[4].Value;
            //    dim++;
            //}

            //if (this.chkTipo.Items[5].Selected)
            //{
            //    queryStato[dim] = this.chkTipo.Items[5].Value;
            //    dim++;
            //}

            //if (this.chkTipo.Items[6].Selected)
            //{
            //    queryStato[dim] = this.chkTipo.Items[6].Value;
            //    dim++;
            //}
            // End Mev CS 1.4 - Esibizione
            //

            if (txt_dataAp_da.Text != "")
            {
                if (txt_dataAp_a.Text != "")
                {
                    queryPar = queryPar + " AND (DATA_APERTURA >= " + this.dbDateConverter(txt_dataAp_da.Text, true) + " AND DATA_APERTURA <= " + this.dbDateConverter(txt_dataAp_a.Text, false) + ") "; //" AND (DATA_APERTURA BETWEEN TO_DATE('" + txt_dataAp_da.Text + "','DD/MM/YYYY') AND TO_DATE('" + txt_dataAp_a.Text + "', 'DD/MM/YYYY'))";                    
                }
                else
                {
                    queryPar = queryPar + " AND DATA_APERTURA >= " + this.dbDateConverter(txt_dataAp_da.Text, true);//" AND DATA_APERTURA >= TO_DATE('" + txt_dataAp_da.Text + "','DD/MM/YYYY')";
                }
            }
            else
            {
                if (txt_dataAp_a.Text != "")
                {

                    queryPar = queryPar + " AND DATA_APERTURA <= " + this.dbDateConverter(txt_dataAp_a.Text, false);//" AND DATA_APERTURA <= TO_DATE('" + txt_dataAp_a.Text + "','DD/MM/YYYY')";

                }
            }

            queryPar = queryPar + " AND ID_AMM = " + infoUtente.idAmministrazione;

            #region filtro data invio
            if (dataInvio_da.Text != "")
            {
                if (dataInvio_a.Text != "")
                {
                    queryPar = queryPar + " AND (DATA_INVIO >= " + this.dbDateConverter(dataInvio_da.Text, true) + " AND DATA_INVIO <= " + this.dbDateConverter(dataInvio_a.Text, false) + ") ";//" AND (DATA_INVIO BETWEEN TO_DATE('" + dataInvio_da.Text + "','DD/MM/YYYY') AND TO_DATE('" + dataInvio_a.Text + "', 'DD/MM/YYYY'))";
                }
                else
                {
                    queryPar = queryPar + " AND DATA_INVIO >= " + this.dbDateConverter(dataInvio_da.Text, true);//TO_DATE('" + dataInvio_da.Text + "','DD/MM/YYYY')";
                }
            }
            else
            {
                if (dataInvio_a.Text != "")
                {
                    queryPar = queryPar + " AND DATA_INVIO <= " + this.dbDateConverter(dataInvio_a.Text, false);// TO_DATE('" + dataInvio_a.Text + "','DD/MM/YYYY')";
                }
            }
            #endregion


            if (this.cboTipiConservazione.SelectedItem != null && !string.IsNullOrEmpty(this.cboTipiConservazione.SelectedItem.Value))
            {
                queryPar = queryPar + " AND VAR_TIPO_CONS='" + this.cboTipiConservazione.SelectedItem.Value + "'";
            }

            if (txt_idIstanza.Text != string.Empty)
            {
                queryPar = queryPar + " AND SYSTEM_ID='" + txt_idIstanza.Text + "'";
            }

            /*
            //
            // MEV CS 1.4
            queryPar = queryPar + " AND CHA_STATO!='N' AND CHA_STATO!='R' AND CHA_STATO!='I' AND CHA_STATO!='T' AND CHA_STATO!='L' AND CHA_STATO!='F' AND CHA_STATO!='E' AND CHA_STATO!='Q'";
            // End MEV CS 1.4
            //
            */

            // MEV CS 1.5
            // non posso vedere nel CS le istanze negli stati introdotti per la gestione del processo di verifica
            queryPar = queryPar + " AND CHA_STATO NOT IN ('N', 'R', 'I', 'T', 'L', 'F', 'E', 'Q', 'A', 'B', 'Y', 'Z' ) ";
            // end MEV CS 1.5

            if (dim != 0)
            {
                query = query + " (";
                for (int i = 0; i < dim; i++)
                {
                    if (i != dim - 1)
                        query = query + " CHA_STATO='" + queryStato[i].ToString() + "' OR ";
                    else
                        query = query + " CHA_STATO='" + queryStato[i].ToString() + "'";
                }
                query = query + ")";
                query = " WHERE " + query + queryPar;
            }
            else
            {
                if (queryPar != "")
                {
                    string appo = queryPar.Substring(queryPar.IndexOf("AND") + 3);
                    query = " WHERE " + appo;
                }
            }

            return query;
        }

        protected string dbDateConverter(string data, bool beginDay)
        {
            string dbType = (string)Session["DbType"];
            string queryDate = "";
            if (dbType.ToLower().Equals("oracle"))
            {
                if (beginDay)
                    queryDate = "TO_DATE('" + data + " 00:00:00','DD/MM/YYYY HH24:mi:ss')";
                else
                    queryDate = "TO_DATE('" + data + " 23:59:59','DD/MM/YYYY HH24:mi:ss')";
            }
            else
            {
                if (dbType.ToLower().Equals("sql"))
                {
                    if (beginDay)
                        queryDate = "convert(datetime,'" + data + " 00:00:00', 103)";
                    else
                        queryDate = "convert(datetime,'" + data + " 23:59:59', 103)";
                }
            }
            return queryDate;
        }

        // Non dovrebbe essere invocato
        protected void btn_firma_Click(object sender, EventArgs e)
        {
            if (this.hd_firma.Value == "true")
            {
                if ((string)Session["resultFirma"] == "1")
                {
                    if (!ConservazioneManager.IsIstanzaConservazioneInterna(this.infoUtente, this.hd_idIstanza.Value))
                    {

                        //string filtroIstanza = " SET CHA_STATO='F', DATA_CONSERVAZIONE = SYSDATE WHERE SYSTEM_ID='" + this.hd_idIstanza.Value + "'";

                        //bool result = ConservazioneManager.updateInfoConservazione(filtroIstanza, infoUtente);

                        ////mettere lo stato dell'istanza e anche di tutti i supporti dell'istanza a F
                        //if (result)
                        //{
                        //    string filtroSupp = " SET CHA_STATO='F' WHERE ID_CONSERVAZIONE='" + this.hd_idIstanza.Value + "'";

                        //    bool result2 = ConservazioneManager.UpdateSupporto(filtroSupp, infoUtente);

                        //    if (result2)
                        //    {
                        string idIstanza = this.hd_idIstanza.Value;

                        string filtro = this.Filtra();
                        infoCons = ConservazioneManager.getAreaConservazioneFiltro(filtro, infoUtente);
                        if (infoCons != null)
                            this.caricaGridviewIstanze(infoCons);

                        //itemsCons = ConservazioneManager.getItemsConservazione(idIstanza, infoUtente);
                        //
                        // MEV CS 1.4 - Esibizione
                        string idGruppo = string.Empty;
                        // Recupero dalla sessione l'idGruppo selezionato
                        if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
                            // Recupero l'idGruppo selezionato
                            //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                            idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
                        itemsCons = ConservazioneManager.getItemsConservazioneWithSecurity(idIstanza, infoUtente, idGruppo);
                        // End Mev Cs 1.4 - Esibizione
                        //

                        if (itemsCons != null)
                            this.caricaGridviewDettaglio(itemsCons);
                        Session["stato"] = "Firmata";
                        this.abilitaFunzioni("Firmata");
                        //ConservazioneManager.createZipFile(idIstanza, infoUtente);
                        //ConservazioneManager.SubmitToRemoteFolder(idIstanza, infoUtente)
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alertCaricamento", "alert('Caricamento avvenuto correttamente');", true);
                        this.upDettaglio.Update();
                        this.upIstanze.Update();

                        //}
                        //else
                        //{
                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert(\"Errore di comunicazione con il server ripetere l\'operazione in seguito\");", true);

                        //}
                        //}
                        //else
                        //{
                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert(\"Errore di comunicazione con il server ripetere l\'operazione in seguito\");", true);
                        //}
                    }
                }
                else
                {
                    if ((string)Session["resultFirma"] == "-1")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('La firma non è avvenuta correttamente riprovare in seguito');", true);
                    }
                    else //se uguale a 0 ossia se la firma è avvenuta ma nn è stata apposta la marca
                    {
                        if ((string)Session["resultFirma"] == "0")
                        {
                            Session["stato"] = "Rigenera marca";
                            this.abilitaFunzioni("Rigenera marca");
                            string filtroIstanza = " SET CHA_STATO='E' WHERE SYSTEM_ID='" + this.hd_idIstanza.Value + "' and id_amm = " + this.infoUtente.idAmministrazione;
                            bool result = ConservazioneManager.updateInfoConservazione(filtroIstanza, infoUtente);
                            string filtroSupp = " SET CHA_STATO='M' WHERE ID_CONSERVAZIONE='" + this.hd_idIstanza.Value + "' and id_amm = " + this.infoUtente.idAmministrazione;
                            bool result2 = ConservazioneManager.UpdateSupporto(filtroSupp, infoUtente);
                            if (result)
                            {
                                this.Panel_dettaglio.Visible = false;
                                this.txt_idIstanza.Text = this.hd_idIstanza.Value;
                                string filtro = this.Filtra();
                                infoCons = ConservazioneManager.getAreaConservazioneFiltro(filtro, infoUtente);
                                if (infoCons != null)
                                    this.caricaGridviewIstanze(infoCons);

                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Errore apposizione marca temporale, rigenera marca temporale');", true);
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert(\"Errore di comunicazione con il server ripetere l\'operazione in seguito\");", true);
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.hd_firma.Value == "false")
                {
                    Session["stato"] = "In Lavorazione";
                    this.abilitaFunzioni("In Lavorazione");
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Impossibile contattare il server');", true);
                }
                Session["stato"] = "In Lavorazione";
                this.abilitaFunzioni("In Lavorazione");
            }
            Session["resultFirma"] = "";
        }

        // Non dovrebbe essere invocato
        /// <summary>
        /// Azione di verifica firma e marca temporale applicata sui documenti nell'istanza di conservazione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_verificaFirma_Click(object sender, EventArgs e)
        {
            int totale = 0;
            int valid = 0;
            int invalid = 0;

            WSConservazioneLocale.AreaConservazioneValidationResult result = ConservazioneManager.ValidaFileFirmati(this.hd_idIstanza.Value, this.infoUtente, out totale, out valid, out invalid);

            //WSConservazioneLocale.ItemsConservazione[] itemsConservazione = ConservazioneManager.getItemsConservazione(this.hd_idIstanza.Value, this.infoUtente);
            //
            // MEV CS 1.4 - Esibizione
            string idGruppo = string.Empty;
            // Recupero dalla sessione l'idGruppo selezionato
            if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
                // Recupero l'idGruppo selezionato
                //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
            WSConservazioneLocale.ItemsConservazione[] itemsConservazione = ConservazioneManager.getItemsConservazioneWithSecurity(this.hd_idIstanza.Value, this.infoUtente, idGruppo);
            // End Mev Cs 1.4 - Esibizione
            //

            this.caricaGridviewDettaglio(itemsConservazione);

            if (!result.IsValid)
            {
                ScriptManager.RegisterStartupScript(
                     this.Page,
                     this.GetType(),
                     "notifyEsitoVerificaFirma",
                     "notifyEsitoVerificaFirma(false," + totale + "," + valid + "," + invalid + ");",
                     true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(
                     this.Page,
                     this.GetType(),
                     "notifyEsitoVerificaFirma",
                     "notifyEsitoVerificaFirma(true," + totale + "," + valid + "," + invalid + ");",
                     true);
            }

            this.VerificaFirmaEffettuata = true;
            UpdateValidationMaskField();
        }

        private void UpdateValidationMaskField()
        {
            int mask = ConservazioneManager.getValidationMask(this.hd_idIstanza.Value);
            ((Label)this.gv_istanzeCons.SelectedItem.FindControl("lbl_validationMask")).Text = mask.ToString();

            if (infoCons != null)
                this.caricaGridviewIstanze(infoCons);
        }

        protected void SelectedIndexChanged(object sender, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.gv_istanzeCons.CurrentPageIndex = e.NewPageIndex;

            infoCons = (WSConservazioneLocale.InfoConservazione[])Session["istanza"];
            caricaGridviewIstanze(infoCons);
            this.upIstanze.Update();
            //  this.Panel_dettaglio.Visible = false;
        }

        protected void gv_dettaglioCons_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gv_dettaglioCons.CurrentPageIndex = e.NewPageIndex;

            itemsCons = (WSConservazioneLocale.ItemsConservazione[])Session["itemsCons"];

            this.caricaGridviewDettaglio(itemsCons);

            this.abilitaFunzioni((string)Session["stato"]);
        }

        protected void gv_istanzeCons_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            infoCons = (WSConservazioneLocale.InfoConservazione[])Session["istanza"];
            if (e.CommandName == "listaSupp")
            {
                int index = Convert.ToInt32(e.CommandArgument.ToString());
                string idIstanza = infoCons[index].SystemID;

                Response.Redirect("RicercaSupportiIstanze.aspx?idConservazione=" + idIstanza);
            }
        }

        protected void VediSupporto(object sender, EventArgs e)
        {
            Button image = (Button)sender;
            TableCell cell = (TableCell)image.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label a = (Label)dgItem.FindControl("lbl_idIstanza");

            Response.Redirect("RicercaSupportiIstanze.aspx?idConservazione=" + a.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_dettaglioCons_OnItemCommand(object sender, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "VERIFICA_INTEGRITA_DOCUMENTO")
            {
                Button btnVerificaIntegrita = (Button)e.Item.FindControl("btnVerificaIntegrita");

                if (btnVerificaIntegrita != null)
                {
                    // Verifica integrità sul singolo documento
                    Label lbl_idProfile = (Label)e.Item.FindControl("lbl_idProfile");

                    btnVerificaIntegrita.OnClientClick =
                        string.Format("return showModalDialogVerificaDocumentoSuSupportoRegistrato('{0}', '{1}', '{2}');",
                                    this.hd_idIstanza.Value, string.Empty, lbl_idProfile.Text);
                }
            }

            //
            // Mev Cs 1.4 - Esibizione
            if (e.CommandName == "INVIA_AREA_ESIBIZIONE")
            {
                //Button btnInviaAreaEsibizione = (Button)e.Item.FindControl("btnInviaAreaEsibizione");
                ImageButton btnInviaAreaEsibizione = (ImageButton)e.Item.FindControl("btnInviaAreaEsibizione");

                if (btnInviaAreaEsibizione != null)
                { 
                    // TO DO
                    // Metodo che fa l'invio
                    string IdItemsEsibizione = "-1";
                    string eccezione = string.Empty;
                    try
                    {
                        //
                        // Occorre recuperare i valori dell'elemento (istanza di conservazione) selezionato
                        itemsCons = (WSConservazioneLocale.ItemsConservazione[]) Session["itemsCons"];
                        if (itemsCons != null && itemsCons.Length > 0)
                        {
                            
                            //int index = Convert.ToInt32(e.CommandArgument.ToString());
                            //string idItemCons = itemsCons[index].SystemID;

                            // Per adesso c'è l'id profile
                            // Mi serve per recuperare l'elemento selezionato
                            string idProfile = e.CommandArgument.ToString().Split('$')[0];
                            string idProject = e.CommandArgument.ToString().Split('$')[1];

                            if (!string.IsNullOrEmpty(idProfile))
                            {
                                // Occorre reperire i parametri da passare alla chiamata di Backend
                                // Costruzione dei Parametri
                                var query = from p in itemsCons.ToList()
                                            where p.ID_Profile == idProfile && p.ID_Project == idProject
                                            select new { p.ID_Profile, p.ID_Project, p.DocNumber, p.tipo_oggetto };

                                string id_profile = query.FirstOrDefault().ID_Profile;
                                string id_project = query.FirstOrDefault().ID_Project;
                                string doc_number = query.FirstOrDefault().DocNumber;
                                string tipo_oggetto = query.FirstOrDefault().tipo_oggetto;

                                //string id_profile = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).ID_Profile;
                                //string id_project = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).ID_Project;
                                //string doc_number = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).DocNumber;
                                //string tipo_oggetto = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).tipo_oggetto;
                                string idIstanzaConservazione = itemsCons.FirstOrDefault(itmCons => itmCons.ID_Profile == idProfile).ID_Conservazione;
                                
                                // Verifico la presenza del documento con idProfile nell'istanza di esibizione con idIstanzaConservazione da cui è stato prelevato il file 
                                // e per lo stesso idPeople e idGruppo
                                bool docPresente = ConservazioneManager.checkItemEsibizionePresenteInIstanzaEsibizione(id_profile, id_project, tipo_oggetto, infoUtente, idIstanzaConservazione);

                                if (!docPresente)
                                {
                                    WSConservazioneLocale.SchedaDocumento documentDetails = null;

                                    // Il metodo deve invocare la Stored Procedure SP_INSERT_AREA_ESIB
                                    IdItemsEsibizione = ConservazioneManager.CreateAndAddDocInAreaEsibizione(id_profile, id_project, doc_number, infoUtente, tipo_oggetto, idIstanzaConservazione, out documentDetails);

                                    try
                                    {
                                        int size_xml = ConservazioneManager.getItemSizeEsib(
                                            this.Page,
                                            documentDetails,
                                            IdItemsEsibizione);

                                        int doc_size = Convert.ToInt32(documentDetails.documenti[0].fileSize);

                                        int numeroAllegati = documentDetails.allegati.Length;
                                        string fileName = documentDetails.documenti[0].fileName;
                                        string tipoFile = Path.GetExtension(fileName);
                                        int size_allegati = 0;
                                        for (int i = 0; i < documentDetails.allegati.Length; i++)
                                        {
                                            size_allegati = size_allegati + Convert.ToInt32(documentDetails.allegati[i].fileSize);
                                        }
                                        int total_size = size_allegati + doc_size + size_xml;

                                        ConservazioneManager.insertSizeInItemEsib(Page, IdItemsEsibizione, total_size);

                                        ConservazioneManager.updateItemsEsibizione(
                                            this.Page,
                                            tipoFile,
                                            Convert.ToString(numeroAllegati),
                                            IdItemsEsibizione);
                                    }
                                    catch (Exception exc1)
                                    {
                                    }
                                }
                                else 
                                {
                                    // Documento già presente
                                    IdItemsEsibizione = "-2";
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Invio_Esibizione_OK", "<script>alert('Attenzione, Documento già presente.');</script>", false);
                                    //ClientScript.RegisterStartupScript(this.GetType(), "Invio_Esibizione_Doc_Presente", "<script>alert('Attenzione, Documento presente.');</script>");
                                }
                            }
                        }
                    }
                    catch (Exception exc) 
                    {
                        IdItemsEsibizione = "-1";
                        eccezione = exc.Message;
                    }

                    // esito -1 -> KO
                    if (!string.IsNullOrEmpty(IdItemsEsibizione) && IdItemsEsibizione.Equals("-1")) 
                        // Alert Javascript
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Invio_Esibizione_KO", "<script>alert('Attenzione, Invio istanza di esibizione non riuscita');</script>", false);
                        //ClientScript.RegisterStartupScript(this.GetType(), "Invio_Esibizione_KO", "<script>alert('Attenzione, Invio istanza di esibizione non riuscita');</script>");
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Invio_Esibizione_OK", "<script>alert('Invio istanza di esibizione avvenuto correttamente');</script>", false);
                        //ClientScript.RegisterStartupScript(this.GetType(), "Invio_Esibizione_OK", "<script>alert('Invio istanza di esibizione avvenuto correttamente');</script>");

                }
            }
            // End Mev Cs 1.4 - Esibizione
            //
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_dettaglioCons_PreRender(object sender, EventArgs e)
        {
            bool foundPolicy = false;

            string policyUtilizzata = string.Empty;

            for (int i = 0; i < this.gv_dettaglioCons.Items.Count; i++)
            {
                if (this.gv_dettaglioCons.Items[i].Cells[3].Text == "G")
                {
                    this.gv_dettaglioCons.Items[i].Cells[3].Text = "NP";
                }

                Label lblInvalidFileFormat = (Label)this.gv_dettaglioCons.Items[i].FindControl("lblInvalidFileFormat");

                bool invalidFileFormat;
                bool.TryParse(lblInvalidFileFormat.Text, out invalidFileFormat);

                /*  //sbarramento per file invalido
                if (invalidFileFormat)
                    this.gv_dettaglioCons.Items[i].Font.Strikeout = true;
                */

                ((Label)this.gv_dettaglioCons.Items[i].FindControl("lbl_numProt")).Font.Bold = true;

                if (((Label)this.gv_dettaglioCons.Items[i].FindControl("lbl_numProt")).Text != String.Empty)
                {
                    ((Label)this.gv_dettaglioCons.Items[i].FindControl("lbl_data_prot_or_crea")).ForeColor = Color.Red;
                }
                else
                {
                    ((Label)this.gv_dettaglioCons.Items[i].FindControl("lbl_data_prot_or_crea")).ForeColor = Color.Black;
                }

                //

                Label lblEsitoValidazioneFirma = (Label)this.gv_dettaglioCons.Items[i].FindControl("lblEsitoValidazioneFirma");

                if (lblEsitoValidazioneFirma != null)
                {
                    System.Web.UI.WebControls.Image imgEsitoValidazionefirma = (System.Web.UI.WebControls.Image)
                                    this.gv_dettaglioCons.Items[i].FindControl("imgEsitoValidazionefirma");



                    if (imgEsitoValidazionefirma != null)
                    {
                        WSConservazioneLocale.EsitoValidazioneFirmaEnum esitoValidazione = (WSConservazioneLocale.EsitoValidazioneFirmaEnum)
                            Enum.Parse(typeof(WSConservazioneLocale.EsitoValidazioneFirmaEnum), lblEsitoValidazioneFirma.Text, true);
                        /*
                        if (esitoValidazione == WSConservazioneLocale.EsitoValidazioneFirmaEnum.Valida)
                        {
                            imgEsitoValidazionefirma.Visible = true;
                            imgEsitoValidazionefirma.ImageUrl = "Img/firmaValida.gif";
                            imgEsitoValidazionefirma.ToolTip = "Firma digitale o marca temporale valida";
                        }
                        else
                         */

                        if (esitoValidazione == WSConservazioneLocale.EsitoValidazioneFirmaEnum.FirmaNonValida)
                        {
                            imgEsitoValidazionefirma.Visible = true;
                            imgEsitoValidazionefirma.ImageUrl = "Img/BadSignature.png";
                            imgEsitoValidazionefirma.ToolTip = "Firma digitale non valida";
                        }

                        else if (esitoValidazione == WSConservazioneLocale.EsitoValidazioneFirmaEnum.MarcaNonValida)
                        {
                            imgEsitoValidazionefirma.Visible = true;
                            imgEsitoValidazionefirma.ImageUrl = "Img/BadTimeStamp.png";
                            imgEsitoValidazionefirma.ToolTip = "Marca temporale non valida";
                        }
                        else
                        {
                            imgEsitoValidazionefirma.Visible = false;
                            imgEsitoValidazionefirma.ImageUrl = string.Empty;
                            imgEsitoValidazionefirma.ToolTip = string.Empty;
                        }
                    }
                }

                //per il documento 
                if (invalidFileFormat)
                {
                    System.Web.UI.WebControls.Image imgEsitoValidazioneTipo = (System.Web.UI.WebControls.Image)
                                this.gv_dettaglioCons.Items[i].FindControl("imgEsitoValidazioneTipo");

                    imgEsitoValidazioneTipo.Visible = true;
                    imgEsitoValidazioneTipo.ImageUrl = "Img/baddocument.png";
                    imgEsitoValidazioneTipo.ToolTip = "Formato file non valido";
                }


                Label lblPolicyValidazione = (Label)this.gv_dettaglioCons.Items[i].FindControl("lbl_policyValida");

                if (lblPolicyValidazione != null)
                {
                    string policy_validazione = lblPolicyValidazione.Text;
                    if (!string.IsNullOrEmpty(policy_validazione) && policy_validazione.Equals("1"))
                    {

                        //Per la policy
                        System.Web.UI.WebControls.Image imgEsitoValidazionePolicy = (System.Web.UI.WebControls.Image)
                            this.gv_dettaglioCons.Items[i].FindControl("imgEsitoValidazionePolicy");

                        imgEsitoValidazionePolicy.Visible = true;
                        imgEsitoValidazionePolicy.ImageUrl = "Img/policyNonVaida.gif";

                        Label lblCheckMaskPolicy = (Label)this.gv_dettaglioCons.Items[i].FindControl("lblMaskPolicy");
                        imgEsitoValidazionePolicy.ToolTip = string.Format("{0}\n{1}", "Policy non valida.", Utils.ConservazioneManager.GetListNonConfPolicy(lblCheckMaskPolicy.Text)); //"Policy non valida";

                        //sbarramento per policy non valida
                        /*
                        for (int t = 0; t < 13; t++)
                        {
                            this.gv_dettaglioCons.Items[i].Cells[t].Attributes.Add("class", "non_valido");
                        }
                        */
                    }
                    foundPolicy = true;
                }

                Button btnVerificaIntegrita = (Button)this.gv_dettaglioCons.Items[i].FindControl("btnVerificaIntegrita");

                if (btnVerificaIntegrita != null)
                {
                    Label lbl_idProfile = (Label)this.gv_dettaglioCons.Items[i].FindControl("lbl_idProfile");

                    btnVerificaIntegrita.OnClientClick = string.Format("return showModalDialogVerificaDocumentoSuSupportoRegistrato('{0}', '{1}', '{2}');",
                                                                this.hd_idIstanza.Value, "", lbl_idProfile.Text);

                }

                Button btnVerificaLeggibilita = (Button)this.gv_dettaglioCons.Items[i].FindControl("btnVerificaLeggibilita2");

                if (btnVerificaLeggibilita != null)
                {
                    Label lbl_idProfile = (Label)this.gv_dettaglioCons.Items[i].FindControl("lbl_idProfile");

                    btnVerificaLeggibilita.OnClientClick = string.Format("return showVerificaLeggibilita('{0}', '{1}', {2});",
                                                                this.hd_idIstanza.Value, lbl_idProfile.Text + ",", "0");
                }

                Button btnVerificaIntegritaStorage = (Button)this.gv_dettaglioCons.Items[i].FindControl("btnVerificaIntegritaStorage");
                if (btnVerificaIntegritaStorage != null)
                {
                    Label lbl_idProfile = (Label)this.gv_dettaglioCons.Items[i].FindControl("lbl_idProfile");

                    btnVerificaIntegritaStorage.OnClientClick = string.Format("return showVerificaIntegritaStorage('{0}', '{1}', {2});",
                                                                this.hd_idIstanza.Value, "", lbl_idProfile.Text);

                }
            }
            if (foundPolicy)
            {
                ListItem temp = new ListItem();
                temp.Value = this.hd_idPolicy.Value;
                if (this.ddl_policy.Items.FindByValue(temp.Value) != null)
                {
                    this.ddl_policy.SelectedValue = this.hd_idPolicy.Value;
                }
            }
            else
                this.ddl_policy.SelectedValue = string.Empty;
        }


        private ConservazioneWA.UserControl.Calendar GetCalendarControl(string controlId)
        {
            return (ConservazioneWA.UserControl.Calendar)this.FindControl(controlId);
        }


        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            this.ClearFlags();

            string query = this.Filtra();


            infoCons = ConservazioneManager.getAreaConservazioneFiltro(query, infoUtente);

            if (infoCons != null)
            {
                this.gv_istanzeCons.CurrentPageIndex = 0;
                this.gv_istanzeCons.SelectedIndex = -1;
                this.caricaGridviewIstanze(infoCons);
                this.upIstanze.Update();
            }

            this.mdlPopupWait.Hide();
        }

        protected void GestioneGrafica()
        {
            btnFind.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btnFind.Attributes.Add("onmouseout", "this.className='cbtn';");

            btn_lavorazione.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btn_lavorazione.Attributes.Add("onmouseout", "this.className='cbtn';");

            btn_standBy.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btn_standBy.Attributes.Add("onmouseout", "this.className='cbtn';");

            btn_rifiuta.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btn_rifiuta.Attributes.Add("onmouseout", "this.className='cbtn';");

            btn_verificaFirma.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btn_verificaFirma.Attributes.Add("onmouseout", "this.className='cbtn';");

            btn_verificaFormati.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btn_verificaFormati.Attributes.Add("onmouseout", "this.className='cbtn';");

            btn_verificaMarca.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btn_verificaMarca.Attributes.Add("onmouseout", "this.className='cbtn';");

            amm = Utils.ConservazioneManager.GetInfoAmmCorrente(this.infoUtente.idAmministrazione);
            this.lbl_amm.Text = this.amm.Codice + " - " + this.amm.Descrizione;

        }

        protected void BtnSelezionaIstanza_Click(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label a = (Label)dgItem.FindControl("lbl_idIstanza");
            gv_istanzeCons.SelectedIndex = dgItem.ItemIndex;
            string idIstanza = a.Text;
            //Mev CS 1.4 - Esibizione
            //itemsCons = ConservazioneManager.getItemsConservazione(idIstanza, infoUtente);
            string idGruppo = string.Empty;
            // Recupero dalla sessione l'idGruppo selezionato
            if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
                // Recupero l'idGruppo selezionato
                //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
            itemsCons = ConservazioneManager.getItemsConservazioneWithSecurity(idIstanza, infoUtente, idGruppo);

            if (itemsCons != null)
            {

                string oldIdIstanza = this.hd_idIstanza.Value;
                this.hd_idIstanza.Value = idIstanza;
                Label stato = (Label)dgItem.FindControl("lb_statoIstanza");
                this.hd_statoIstanza.Value = stato.Text;

                if (idIstanza != oldIdIstanza)
                {
                    this.ClearFlags();
                }

                if (this.gv_dettaglioCons != null)
                {
                    gv_dettaglioCons.CurrentPageIndex = 0;
                }

                this.caricaGridviewDettaglio(itemsCons);

                Label b = (Label)dgItem.FindControl("idpolicyvalidazione");
                if (b != null)
                {
                    this.hd_idPolicy.Value = b.Text;
                }

                int validationMask = Int32.Parse(((Label)dgItem.FindControl("lbl_validationMask")).Text);
                if ((b == null || (b != null && string.IsNullOrEmpty(b.Text))) && ((validationMask & 0x08) != 0x08))
                {
                    //metodo per aggiornare la validation mask, inserendo policy verificata nel caso sia assente.
                    ConservazioneManager.setPolicyVerificataLite(idIstanza);
                }

                Label lbl_IsInPreparazione = (Label)dgItem.FindControl("lbl_IsInPreparazione");

                if (lbl_IsInPreparazione != null)
                {
                    bool inPreparazione = Convert.ToBoolean(lbl_IsInPreparazione.Text);

                    if (inPreparazione)
                        this.RefreshControlsInPreparazione(true);
                    else
                    {
                        this.RefreshControlsInPreparazione(false);

                        abilitaFunzioni(stato.Text);
                    }
                }

                if (stato.Text.Equals("Nuova") && ConservazioneManager.dimensioneValidaVM(idIstanza))
                {
                    this.pnl_bottoniera.Visible = true;
                }
                else
                {
                    this.pnl_bottoniera.Visible = false;
                }


                this.Panel_dettaglio.Visible = true;

                this.upDettaglio.Update();
            }
        }

        protected void DataGrid_ItemCreated(object sender, DataGridItemEventArgs e)
        {

            Button btn_dettSupp = (Button)e.Item.FindControl("btn_dettSupp");
            if (btn_dettSupp != null)
            {
                if (btn_dettSupp.Enabled)
                {
                    btn_dettSupp.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                    btn_dettSupp.Attributes.Add("onmouseout", "this.className='cbtn';");
                    btn_dettSupp.Attributes.Add("class", "cbtn");
                }
                else
                {
                    btn_dettSupp.Attributes.Add("class", "cbtnDisabled");
                }
            }

            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }


        #region MASSIVA

        /// <summary>
        /// Invio massivo del contenuto dell'intera istanza in esibizione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnIvioEsibMass_Click(object sender, EventArgs e)
        {
            List<WSConservazioneLocale.ItemsConservazione> listIstanzeOK = new List<WSConservazioneLocale.ItemsConservazione>();
            List<WSConservazioneLocale.ItemsConservazione> listIstanzeWarning = new List<WSConservazioneLocale.ItemsConservazione>();
            List<WSConservazioneLocale.ItemsConservazione> listIstanzeKO = new List<WSConservazioneLocale.ItemsConservazione>();

            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label a = (Label)dgItem.FindControl("lbl_idIstanza");
            gv_istanzeCons.SelectedIndex = dgItem.ItemIndex;
            string idIstanza = a.Text;
            
            //Mev CS 1.4 - Esibizione
            //itemsCons = ConservazioneManager.getItemsConservazione(idIstanza, infoUtente);
            string idGruppo = string.Empty;
            // Recupero dalla sessione l'idGruppo selezionato
            if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
                // Recupero l'idGruppo selezionato
                //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
            
            itemsCons = ConservazioneManager.getItemsConservazioneWithSecurity(idIstanza, infoUtente, idGruppo);

            if (itemsCons != null && itemsCons.Length > 0)
            {
                string id_profile = string.Empty;
                string id_project = string.Empty;
                string doc_number = string.Empty;
                string tipo_oggetto = string.Empty;
                string idIstanzaConservazione = string.Empty;

                string IdItemsEsibizione = "-1";
                string eccezione = string.Empty;

                foreach (WSConservazioneLocale.ItemsConservazione itemIstanza in itemsCons)
                {
                    id_profile = itemIstanza.ID_Profile.ToString();
                    id_project = itemIstanza.ID_Project.ToString();
                    doc_number = itemIstanza.DocNumber.ToString();
                    tipo_oggetto = itemIstanza.tipo_oggetto.ToString();
                    idIstanzaConservazione = itemIstanza.ID_Conservazione.ToString();

                    try
                    {
                        // Verifico la presenza del documento con idProfile nell'istanza di esibizione con idIstanzaConservazione da cui è stato prelevato il file 
                        // e per lo stesso idPeople e idGruppo
                        bool docPresente = ConservazioneManager.checkItemEsibizionePresenteInIstanzaEsibizione(id_profile, id_project, tipo_oggetto, infoUtente, idIstanzaConservazione);

                        if (!docPresente)
                        {
                            WSConservazioneLocale.SchedaDocumento documentDetails = null;

                            // Il metodo deve invocare la Stored Procedure SP_INSERT_AREA_ESIB
                            IdItemsEsibizione = ConservazioneManager.CreateAndAddDocInAreaEsibizione(id_profile, id_project, doc_number, infoUtente, tipo_oggetto, idIstanzaConservazione, out documentDetails);

                            try
                            {
                                int size_xml = ConservazioneManager.getItemSizeEsib(
                                    this.Page,
                                    documentDetails,
                                    IdItemsEsibizione);

                                int doc_size = Convert.ToInt32(documentDetails.documenti[0].fileSize);

                                int numeroAllegati = documentDetails.allegati.Length;
                                string fileName = documentDetails.documenti[0].fileName;
                                string tipoFile = Path.GetExtension(fileName);
                                int size_allegati = 0;
                                for (int i = 0; i < documentDetails.allegati.Length; i++)
                                {
                                    size_allegati = size_allegati + Convert.ToInt32(documentDetails.allegati[i].fileSize);
                                }
                                int total_size = size_allegati + doc_size + size_xml;

                                ConservazioneManager.insertSizeInItemEsib(Page, IdItemsEsibizione, total_size);

                                ConservazioneManager.updateItemsEsibizione(
                                    this.Page,
                                    tipoFile,
                                    Convert.ToString(numeroAllegati),
                                    IdItemsEsibizione);
                            }
                            catch (Exception exc1)
                            {
                            }
                        }
                        else
                        {
                            // Documento già presente
                            IdItemsEsibizione = "-2";
                        }
                    }
                    catch (Exception exc) 
                    {
                        IdItemsEsibizione = "-1";
                        eccezione = exc.Message;
                    }

                    // esito -1 -> KO
                    if (!string.IsNullOrEmpty(IdItemsEsibizione) && IdItemsEsibizione.Equals("-1"))
                    {
                        //KO
                        listIstanzeKO.Add(itemIstanza);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(IdItemsEsibizione) && IdItemsEsibizione.Equals("-2"))
                        {
                            //
                            // Lista elementi già presenti
                            listIstanzeWarning.Add(itemIstanza);
                        }
                        else
                        {
                            //
                            // Lista elementi ok.
                            listIstanzeOK.Add(itemIstanza);
                        } 
                    }
                }// Foreach

                // Metto le liste in sessione
                // le liste 
                Session["listIstanzeOK"] = listIstanzeOK;
                Session["listIstanzeWarning"] = listIstanzeWarning;
                Session["listIstanzeKO"] = listIstanzeKO;

                // Solo per sicurezza - non necessario
                Session.Remove("dictRiepilogoMass");

            }// End if

            ScriptManager.RegisterStartupScript(this,
                            this.GetType(),
                            "Apri_Riepilogo",
                            "showDialogMassiveOperation();",
                            true);
        }

        /// <summary>
        /// Verifica se L'istanza ha contenuti visibili
        /// </summary>
        /// <param name="rowIstanza"></param>
        /// <returns></returns>
        protected bool IsContenutoIstanzaVisible(object rowIstanza)
        {
            bool retval = false;
            string idIstanza = ((System.Data.DataRowView)rowIstanza).Row["idIstanza"].ToString();

            //Mev CS 1.4 - Esibizione
            //itemsCons = ConservazioneManager.getItemsConservazione(idIstanza, infoUtente);
            string idGruppo = string.Empty;
            // Recupero dalla sessione l'idGruppo selezionato
            if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
                // Recupero l'idGruppo selezionato
                //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
            itemsCons = ConservazioneManager.getItemsConservazioneWithSecurity(idIstanza, infoUtente, idGruppo);

            if (itemsCons != null && itemsCons.Length > 0)
                retval = true;
            else
                retval = false;

            return retval;
        }


        #endregion

        protected void ImageCreatedRender(Object sender, DataGridItemEventArgs e)
        {
            Button btnVerificaIntegrita = (Button)e.Item.FindControl("btnVerificaIntegrita");
            if (btnVerificaIntegrita != null)
            {
                if (btnVerificaIntegrita.Enabled)
                {
                    btnVerificaIntegrita.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                    btnVerificaIntegrita.Attributes.Add("onmouseout", "this.className='cbtn';");
                    btnVerificaIntegrita.Attributes.Add("class", "cbtn");
                }
                else
                {
                    btnVerificaIntegrita.Attributes.Add("class", "cbtnDisabled");

                }
                btnVerificaIntegrita.Visible = this.isVisibleVerificaIntegritaRemoto();
            }
            Button btnVerificaLeggibilita = (Button)e.Item.FindControl("btnVerificaLeggibilita2");
            if (btnVerificaLeggibilita != null)
            {
                if (btnVerificaLeggibilita.Enabled)
                {
                    btnVerificaLeggibilita.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                    btnVerificaLeggibilita.Attributes.Add("onmouseout", "this.className='cbtn';");
                    btnVerificaLeggibilita.Attributes.Add("class", "cbtn");
                }
                else
                {
                    btnVerificaLeggibilita.Attributes.Add("class", "cbtnDisabled");
                }
            }
            Button btnVerificaIntegritaStorage = (Button)e.Item.FindControl("btnVerificaIntegritaStorage");
            if (btnVerificaIntegritaStorage != null)
            {
                if (btnVerificaIntegritaStorage.Enabled)
                {
                    btnVerificaIntegritaStorage.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                    btnVerificaIntegritaStorage.Attributes.Add("onmouseout", "this.className='cbtn';");
                    btnVerificaIntegritaStorage.Attributes.Add("class", "cbtn");
                }
                else
                {
                    btnVerificaIntegritaStorage.Attributes.Add("class", "cbtnDisabled");
                }
            }

            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        protected void gv_istanzeCons_PreRender(object sender, EventArgs e)
        {

            //MEV CS 1.4
            //bug nel pager della datagrid
            try
            {
                if (this.gv_istanzeCons.Controls.Count > 0)
                {
                    Table tab = (Table)this.gv_istanzeCons.Controls[0];
                    //pager superiore
                    TableRow pagerTop = tab.Rows[0];
                    pagerTop.Cells[0].Attributes.Add("colspan", tab.Rows[1].Cells.Count.ToString());
                    //pager inferiore
                    TableRow pagerBottom = tab.Rows[tab.Rows.Count - 1];
                    pagerBottom.Cells[0].Attributes.Add("colspan", tab.Rows[1].Cells.Count.ToString());
                }
            }
            catch (Exception ex)
            {

            }

            for (int i = 0; i < this.gv_istanzeCons.Items.Count; i++)
            {
                ((Label)this.gv_istanzeCons.Items[i].FindControl("lb_statoIstanza")).Text.Trim();
                string stato = ((Label)this.gv_istanzeCons.Items[i].FindControl("lb_statoIstanza")).Text.Trim();

                if (stato == "R")
                {
                    ((Label)this.gv_istanzeCons.Items[i].FindControl("lb_statoIstanza")).Text = "Rifiutata";
                    ((System.Web.UI.WebControls.Button)this.gv_istanzeCons.Items[i].Cells[18].Controls[1]).Enabled = false;
                }
                if (stato == "L")
                {
                    ((Label)this.gv_istanzeCons.Items[i].FindControl("lb_statoIstanza")).Text = "In Lavorazione";
                }
                if (stato == "I")
                {
                    ((Label)this.gv_istanzeCons.Items[i].FindControl("lb_statoIstanza")).Text = "Nuova";
                    ((System.Web.UI.WebControls.Button)this.gv_istanzeCons.Items[i].Cells[18].Controls[1]).Enabled = false;
                }
                if (stato == "C")
                {
                    ((Label)this.gv_istanzeCons.Items[i].FindControl("lb_statoIstanza")).Text = "Chiusa";
                }
                if (stato == "F")
                {
                    ((Label)this.gv_istanzeCons.Items[i].FindControl("lb_statoIstanza")).Text = "Firmata";
                }
                // l'istanza è firmata ma l'apposizione della marca nn è avvenuta correttamente
                if (stato == "E")
                {
                    ((Label)this.gv_istanzeCons.Items[i].FindControl("lb_statoIstanza")).Text = "Rigenera marca";
                }

                // l'istanza è firmata ma l'apposizione della marca nn è avvenuta correttamente
                if (stato == "V")
                {
                    ((Label)this.gv_istanzeCons.Items[i].FindControl("lb_statoIstanza")).Text = "Conservata";
                }

                // l'istanza è in transizione nello storage remoto dopo la verifica leggibilità
                if (stato == "T")
                {
                    //((Label)this.gv_istanzeCons.Items[i].FindControl("lb_statoIstanza")).Text = "In Transizione";
                    ((Label)this.gv_istanzeCons.Items[i].FindControl("lb_statoIstanza")).Text = "In Chiusura";
                }
                // l'istanza è in fase di verifica automatica
                if (stato == "Q")
                {
                    ((Label)this.gv_istanzeCons.Items[i].FindControl("lb_statoIstanza")).Text = "In fase di verifica";
                }

                if (((Label)this.gv_istanzeCons.Items[i].FindControl("lbl_tipoCons")) != null)
                {
                    if (!string.IsNullOrEmpty(((Label)this.gv_istanzeCons.Items[i].FindControl("lbl_tipoCons")).Text) && ((Label)this.gv_istanzeCons.Items[i].FindControl("lbl_tipoCons")).Text.Equals("Conservazione interna"))

                        if ((Button)this.gv_istanzeCons.Items[i].FindControl("btn_dettSupp") != null)
                        {
                            Button btn_dettSupp = (Button)this.gv_istanzeCons.Items[i].FindControl("btn_dettSupp");
                            btn_dettSupp.Enabled = false;

                            btn_dettSupp.Attributes.Remove("onmouseover");
                            btn_dettSupp.Attributes.Remove("onmouseout");
                            btn_dettSupp.Attributes.Remove("class");

                            btn_dettSupp.Attributes.Add("class", "cbtnDisabled");

                        }

                }

                //prova codice immagine per VALIDATION_MASK
                string vm = ((Label)this.gv_istanzeCons.Items[i].FindControl("lbl_validationMask")).Text.Trim();
                System.Web.UI.WebControls.Image imgvm = (System.Web.UI.WebControls.Image)
                                    this.gv_istanzeCons.Items[i].FindControl("img_validationMask");
                int valMask;
                Int32.TryParse(vm, out valMask);

                if ((valMask & 0x80) == 0)  //Se i controlli automatici non sono hanno avuto logo non visuaalizzare l'icona
                {
                    if ((valMask & 0x1F) == 0x1F) //marca / firma  /formato / policy valida /dimensione
                    {
                        imgvm.Visible = true;
                        imgvm.ImageUrl = "Img/firmaValida.gif";
                        imgvm.ToolTip = "Verifiche eseguite con successo";
                    }
                    else if ((valMask & 0x17) == 0x17)
                    {
                        imgvm.Visible = true;
                        imgvm.ToolTip = "L'istanza non è aderente alla policy";
                        imgvm.ImageUrl = "Img/verificapolicynonvalida.gif";
                    }
                    else imgvm.Visible = false;
                }
                else
                {
                    if ((valMask & 0x10) != 0x10)
                    {
                        imgvm.Visible = true;
                        imgvm.ImageUrl = "Img/firmaNonValida.gif";
                        imgvm.ToolTip = "Dimensione dell'istanza troppo grande";
                    }
                    else
                    {
                        //specializzare i tooltip
                        if ((valMask & 0xF) != 0xf)
                        {
                            imgvm.Visible = true;
                            imgvm.ImageUrl = "Img/firmaNonValida.gif";
                            imgvm.ToolTip = "Qualche verifica automatica ha dato esito negativo";
                            if ((valMask & 0x2) != 0x2) imgvm.ToolTip = "Verifica della firma fallita per almeno un documento e/o allegato";
                            else if ((valMask & 0x1) != 0x1) imgvm.ToolTip = "Verifica della marca fallita per almeno un documento e/o allegato";
                            else if ((valMask & 0x4) != 0x4) imgvm.ToolTip = "Verifica del formato fallita per almeno un documento e/o allegato";
                            else if ((valMask & 0x8) != 0x8)
                            {
                                imgvm.ToolTip = "Verifica della policy fallita";
                                imgvm.ImageUrl = "Img/verificapolicynonvalida.gif";
                            }
                        }
                        else if ((valMask & 0xf) == 0xf)  //marca / firma  /formato / policy valida
                        {
                            imgvm.Visible = true;
                            imgvm.ImageUrl = "Img/firmaValida.gif";
                            imgvm.ToolTip = "Verifiche Automatiche eseguite con successo";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_lavorazione_OnClick(object sender, EventArgs e)
        {
            if (false)
            {
                //if (!this.VerificaFormatiEffettuata)
                {
                    WSConservazioneLocale.AreaConservazioneValidationResult retval = ConservazioneManager.validateIstanzaConservazione(this.hd_idIstanza.Value, infoUtente);

                    this.AleastOneInvalidDocumentFormat = !retval.IsValid;

                    this.VerificaFormatiEffettuata = true;
                }

                if (this.AleastOneInvalidDocumentFormat)
                {
                    // Istanza di conservazione non valida
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "notifyInvalidIstanzaConservazione", "notifyInvalidIstanzaConservazione();", true);
                }
            }
            else
            {
                WSConservazioneLocale.TipoIstanzaConservazione[] tipiIstanza = ConservazioneManager.GetTipiIstanza();

                string query = " WHERE SYSTEM_ID= " + this.hd_idIstanza.Value + " ";
                WSConservazioneLocale.InfoConservazione[] infoConservazione = ConservazioneManager.getAreaConservazioneFiltro(query, infoUtente);
                //Metodo per popup. WIP

                this.hd_lavorazione_policy.Value = "";

                if (!string.IsNullOrEmpty(ddl_policy.SelectedValue.ToString()) && !ConservazioneManager.policyVerificata(this.hd_idIstanza.Value))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "notifyInvalidIstanzaConservazione", "showModalDialogLavorazione(true);", true);
                }
                else
                {

                    if (infoConservazione[0].TipoConservazione == "CONSERVAZIONE_INTERNA")
                    {
                        this.MettiInLavorazioneAsync();
                    }
                    else
                    {
                        // TODO: NUMERO COPIE, SOLO SE L'ISTANZA è DI TIPO 
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "notifyInvalidIstanzaConservazione", "showModalDialogLavorazione();", true);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnRifiuta_Click(object sender, EventArgs e)
        {
            //se l'istanza viene rifiutata
            if (this.hf_note.Value != null && this.hf_note.Value != String.Empty && this.hf_note.Value != "undefined")
            {
                string note = hf_note.Value.Replace("'", "''");
                bool result = UpdateIstanza("", true, note, "");
                if (result)
                {
                    bool resultTrasm = ConservazioneManager.trasmettiNotificaRifiuto(hf_note.Value, this.hd_idIstanza.Value, infoUtente);
                    Session["stato"] = "Rifiutata";
                    this.abilitaFunzioni("Rifiutata");
                    this.hf_note.Value = null;
                    this.txt_idIstanza.Text = string.Empty;
                    string query = this.Filtra();
                    infoCons = ConservazioneManager.getAreaConservazioneFiltro(query, infoUtente);
                    if (infoCons != null)
                    {
                        this.caricaGridviewIstanze(infoCons);
                    }
                    this.Panel_dettaglio.Visible = false;
                    this.upDettaglio.Update();
                    this.upIstanze.Update();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert(\"Errore di comunicazione con il server ripetere l\'operazione in seguito\");", true);
                }
            }
        }

        protected bool UpdateIstanza(string n_copie, bool isRefused, string note, string stato)
        {
            bool result = true;
            if (!isRefused)
            {
                string filtroIstanza = " SET CHA_STATO='" + stato + "', VAR_NOTE='" + note + "', COPIE_SUPPORTI='" + n_copie + "' WHERE SYSTEM_ID='" + this.hd_idIstanza.Value + "' and id_amm = " + this.infoUtente.idAmministrazione;


                WSConservazioneLocale.TipoSupporto[] tipiSupporto = ConservazioneManager.GetTipiSupporto();

                // Creazione del supporto remoto
                int newIdSupportoRemoto;
                if (ConservazioneManager.setSupporto("0", "", "", "", "", "", "", "", "", "", this.hd_idIstanza.Value, tipiSupporto.First(e => e.TipoSupp == "REMOTO").SystemId, "L", "", "I", "", "", infoUtente, "", out newIdSupportoRemoto) == -1)
                {

                }

                for (int i = 1; i <= Convert.ToInt32(n_copie); i++)
                {
                    int newId = 0;
                    if (ConservazioneManager.setSupporto("" + i + "", "", "", "", "", "", "", "", "", "", this.hd_idIstanza.Value, tipiSupporto.First(e => e.TipoSupp == "RIMOVIBILE").SystemId, "L", "", "I", "", "", infoUtente, "", out newId) == -1)
                    {
                        result = false;
                        break;
                    }
                }
                if (result)
                    result = ConservazioneManager.updateInfoConservazione(filtroIstanza, infoUtente);
            }
            else
            {
                //SAB pezza per gestire campi date -- se c'è sql la querystring viene modificata da docsPaConservazione
                string curDate = " SYSDATE ";

                string filtroIstanza = " SET CHA_STATO='R', VAR_NOTE_RIFIUTO='" + note + "', DATA_RIFIUTO= " +
                curDate + " WHERE SYSTEM_ID='" + this.hd_idIstanza.Value + "' and id_amm = " + this.infoUtente.idAmministrazione;
                result = ConservazioneManager.updateInfoConservazione(filtroIstanza, infoUtente);
            }

            infoCons = (WSConservazioneLocale.InfoConservazione[])Session["istanza"];

            string filtro = this.Filtra();
            infoCons = ConservazioneManager.getAreaConservazioneFiltro(filtro, infoUtente);
            this.caricaGridviewIstanze(infoCons);
            return result;
        }

        // Non dovrebbe essere utilizzato
        /// <summary>
        /// Azione di verifica marca temporale applicata sui documenti nell'istanza di conservazione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_verificaMarca_Click(object sender, EventArgs e)
        {
            int totale = 0;
            int valid = 0;
            int invalid = 0;

            WSConservazioneLocale.AreaConservazioneValidationResult result = ConservazioneManager.ValidaFileMarcati(this.hd_idIstanza.Value, this.infoUtente, out totale, out valid, out invalid);

            //
            // Mev CS 1.4 - Esibizione
            //WSConservazioneLocale.ItemsConservazione[] itemsConservazione = ConservazioneManager.getItemsConservazione(this.hd_idIstanza.Value, this.infoUtente);
            string idGruppo = string.Empty;
            // Recupero dalla sessione l'idGruppo selezionato
            if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
                // Recupero l'idGruppo selezionato
                //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
            WSConservazioneLocale.ItemsConservazione[] itemsConservazione = ConservazioneManager.getItemsConservazioneWithSecurity(this.hd_idIstanza.Value, this.infoUtente, idGruppo);
            // End Mev CS 1.4 - Esibizione
            //
            this.caricaGridviewDettaglio(itemsConservazione);

            if (!result.IsValid)
            {
                ScriptManager.RegisterStartupScript(
                     this.Page,
                     this.GetType(),
                     "notifyEsitoVerificaMarca",
                     "notifyEsitoVerificaMarca(false," + totale + "," + valid + "," + invalid + ");",
                     true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(
                     this.Page,
                     this.GetType(),
                     "notifyEsitoVerificaMarca",
                     "notifyEsitoVerificaMarca(true," + totale + "," + valid + "," + invalid + ");",
                     true);
            }

            this.VerificaMarcaEffettuata = true;
            UpdateValidationMaskField();
        }

        /// <summary>
        /// Verifica validità formati
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_verificaFormati_Click(object sender, EventArgs e)
        {
            this.itemsCons = ConservazioneManager.getItemsConservazioneWithContentValidation(this.hd_idIstanza.Value, this.infoUtente);

            this.caricaGridviewDettaglio(this.itemsCons);

            if (this.itemsCons.Count(itm => itm.invalidFileFormat) > 0)
            {
                this.AleastOneInvalidDocumentFormat = true;

                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.GetType(),
                    "notifyEsitoVerificaFormati",
                    "notifyEsitoVerificaFormati(false);",
                    true);
            }
            else
            {
                this.AleastOneInvalidDocumentFormat = false;

                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.GetType(),
                    "notifyEsitoVerificaFormati",
                    "notifyEsitoVerificaFormati(true);",
                    true);
            }

            this.VerificaFormatiEffettuata = true;
            UpdateValidationMaskField();
        }

        // Non dovrebbe essere invocato
        protected void ChangePolicy(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ddl_policy.SelectedValue))
            {
                bool result = ConservazioneManager.ValidateIstanzaConservazioneConPolicy(this.ddl_policy.SelectedValue, this.hd_idIstanza.Value, infoUtente);
                if (result)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Istanza verficata');", true);
                }
            }
            else
            {
                bool result = ConservazioneManager.DeleteValidateIstanzaConservazioneConPolicy(this.ddl_policy.SelectedValue, this.hd_idIstanza.Value, infoUtente);
                if (result)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", @"alert('Policy non più associata all\'istanza');", true);
                }
            }
            
            //WSConservazioneLocale.ItemsConservazione[] itemsConservazione = ConservazioneManager.getItemsConservazione(this.hd_idIstanza.Value, this.infoUtente);
            //
            // MEV cs 1.4 - Esibizione
            string idGruppo = string.Empty;
            // Recupero dalla sessione l'idGruppo selezionato
            if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
                // Recupero l'idGruppo selezionato
                //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
            WSConservazioneLocale.ItemsConservazione[] itemsConservazione = ConservazioneManager.getItemsConservazioneWithSecurity(this.hd_idIstanza.Value, this.infoUtente, idGruppo);
            // End Mev CS 1.4 - Esibizione
            //
            this.caricaGridviewDettaglio(itemsConservazione);
            this.hd_idPolicy.Value = this.ddl_policy.SelectedValue;
            this.upDettaglio.Update();
            UpdateValidationMaskField();
        }


        protected void CaricaDropDownListPolicy()
        {
            WSConservazioneLocale.Policy[] policyListaDocumenti = ConservazioneManager.GetListaPolicy(Int32.Parse(infoUtente.idAmministrazione), "D");
            WSConservazioneLocale.Policy[] policyListaFascicoli = ConservazioneManager.GetListaPolicy(Int32.Parse(infoUtente.idAmministrazione), "F");
            WSConservazioneLocale.Policy[] policyListaStampe = ConservazioneManager.GetListaPolicy(Int32.Parse(infoUtente.idAmministrazione), "R");
            WSConservazioneLocale.Policy[] policyListaRepertori = ConservazioneManager.GetListaPolicy(Int32.Parse(infoUtente.idAmministrazione), "C");

            ddl_policy.Items.Clear();
            ddl_policy.Items.Add("");
            int y = 0;
            if (policyListaDocumenti != null)
            {
                for (int i = 0; i < policyListaDocumenti.Length; i++, y++)
                {
                    ddl_policy.Items.Add("[D] " + policyListaDocumenti[i].nome);
                    ddl_policy.Items[i + 1].Value = policyListaDocumenti[i].system_id;
                }
            }
            if (policyListaFascicoli != null)
            {
                for (int i = 0; i < policyListaFascicoli.Length; i++, y++)
                {
                    ddl_policy.Items.Add("[F] " + policyListaFascicoli[i].nome);
                    ddl_policy.Items[y + 1].Value = policyListaFascicoli[i].system_id;
                }
            }
            if (policyListaStampe != null)
            {
                for (int i = 0; i < policyListaStampe.Length; i++, y++)
                {
                    ddl_policy.Items.Add("[S] " + policyListaStampe[i].nome);
                    ddl_policy.Items[y + 1].Value = policyListaStampe[i].system_id;
                }
            }
            if (policyListaRepertori != null)
            {
                for (int i = 0; i < policyListaRepertori.Length; i++, y++)
                {
                    ddl_policy.Items.Add("[R] " + policyListaRepertori[i].nome);
                    ddl_policy.Items[y + 1].Value = policyListaRepertori[i].system_id;
                }
            }
        }

        // Non dovrebbe essere invocato
        protected void btn_standBy_Click(object sender, EventArgs e)
        {
            ////TODO: RICHIAMARE IL SERVIZIO PER APPOSIZIONE MARCA
            bool result = ConservazioneManager.apponiMarca(this.hd_idIstanza.Value, infoUtente, "", "");
            if (result)
            {
                string filtroIstanza = " SET CHA_STATO='F', DATA_CONSERVAZIONE = SYSDATE WHERE SYSTEM_ID='" + this.hd_idIstanza.Value + "' and id_amm = " + this.infoUtente.idAmministrazione;
                bool result2 = ConservazioneManager.updateInfoConservazione(filtroIstanza, infoUtente);
                if (result2)
                {
                    string filtroSupp = " SET CHA_STATO='F' WHERE ID_CONSERVAZIONE='" + this.hd_idIstanza.Value + "'";
                    bool result3 = ConservazioneManager.UpdateSupporto(filtroSupp, infoUtente);
                    ConservazioneManager.createZipFile(this.hd_idIstanza.Value, infoUtente);
                    if (result3)
                    {
                        // Importante: inzio verifica supporto remoto per mettere l'istanza in stato "Conservata"
                        ConservazioneManager.IniziaVerificaSupportoRemoto(infoUtente, this.hd_idIstanza.Value);

                        string filtro = this.Filtra();
                        infoCons = ConservazioneManager.getAreaConservazioneFiltro(filtro, infoUtente);
                        if (infoCons != null)
                            this.caricaGridviewIstanze(infoCons);
                        string idIstanza = this.hd_idIstanza.Value;
                        //
                        // MEV cs 1.4 - Esibizione
                        //itemsCons = ConservazioneManager.getItemsConservazione(idIstanza, infoUtente);
                        string idGruppo = string.Empty;
                        // Recupero dalla sessione l'idGruppo selezionato
                        if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"] != null)
                            // Recupero l'idGruppo selezionato
                            //string idGruppo = Request.QueryString["RuoloSelezionato_idGruppo"];
                            idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
                        itemsCons = ConservazioneManager.getItemsConservazioneWithSecurity(idIstanza, infoUtente, idGruppo);
                        // End Mev Cs 1.4 - Esibizione
                        //
                        if (itemsCons != null)
                            this.caricaGridviewDettaglio(itemsCons);
                        Session["stato"] = "Firmata";
                        this.abilitaFunzioni("Firmata");
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert(\"L\'operazione è andata a buon fine\");", true);
                        this.upIstanze.Update();
                        this.upDettaglio.Update();
                        ConservazioneManager.createZipFile(idIstanza, infoUtente);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert(\"Errore di comunicazione con il server ripetere l\'operazione in seguito\");", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert(\"Errore di comunicazione con il server ripetere l\'operazione in seguito\");", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Impossibile contattare il server');", true);
            }
        }


        /// <summary>
        /// Caricamento combo dei tipi conservazione
        /// </summary>
        protected void FetchTipiConservazione()
        {
            List<WSConservazioneLocale.TipoIstanzaConservazione> tipiIstanza = new List<WSConservazioneLocale.TipoIstanzaConservazione>(Utils.ConservazioneManager.GetTipiIstanza());

            //
            // Mev CS 1.4 Esibizione
            try
            {
                if (tipiIstanza.Contains(tipiIstanza.FirstOrDefault(itm => itm.Codice.Equals("ESIBIZIONE"))))
                    tipiIstanza.Remove(tipiIstanza.FirstOrDefault(itm => itm.Codice.Equals("ESIBIZIONE")));
            }
            catch (Exception ex) 
            {
            }
            // End Mev cs 1.4 Esibizione
            //

            tipiIstanza.Insert(0, new WSConservazioneLocale.TipoIstanzaConservazione { Codice = string.Empty, Descrizione = string.Empty });

            this.cboTipiConservazione.DataSource = tipiIstanza;
            this.cboTipiConservazione.DataBind();
        }


        /// <summary>
        /// Istanza di conservazione in fase di preparazione, tutti i controlli sono disabilitati
        /// </summary>
        protected void RefreshControlsInPreparazione(bool inPreparazione)
        {
            this.btn_rifiuta.Enabled = !inPreparazione;
            if (btn_rifiuta.Enabled)
            {
                btn_rifiuta.Attributes.Remove("class");
                btn_rifiuta.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_rifiuta.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_rifiuta.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_rifiuta.Attributes.Remove("onmouseover");
                btn_rifiuta.Attributes.Remove("onmouseout");
                btn_rifiuta.Attributes.Remove("class");
                btn_rifiuta.Attributes.Add("class", "cbtnDisabled");
            }
            this.btn_lavorazione.Enabled = !inPreparazione;
            if (btn_lavorazione.Enabled)
            {
                btn_lavorazione.Attributes.Remove("class");
                btn_lavorazione.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_lavorazione.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_lavorazione.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_lavorazione.Attributes.Remove("onmouseover");
                btn_lavorazione.Attributes.Remove("onmouseout");
                btn_lavorazione.Attributes.Remove("class");
                btn_lavorazione.Attributes.Add("class", "cbtnDisabled");
            }
            this.btn_firma.Enabled = !inPreparazione;
            if (btn_firma.Enabled)
            {
                btn_firma.Attributes.Remove("class");
                btn_firma.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_firma.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_firma.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_firma.Attributes.Remove("onmouseover");
                btn_firma.Attributes.Remove("onmouseout");
                btn_firma.Attributes.Remove("class");
                btn_firma.Attributes.Add("class", "cbtnDisabled");
            }
            this.btn_standBy.Enabled = !inPreparazione;
            if (btn_standBy.Enabled)
            {
                btn_standBy.Attributes.Remove("class");
                btn_standBy.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_standBy.Attributes.Add("onmouseout", "this.className='cbtn';");
                btn_standBy.Attributes.Add("class", "cbtn");
            }
            else
            {
                btn_standBy.Attributes.Remove("onmouseover");
                btn_standBy.Attributes.Remove("onmouseout");
                btn_standBy.Attributes.Remove("class");
                btn_standBy.Attributes.Add("class", "cbtnDisabled");
            }

            this.pnlStatoInLavorazione.Visible = inPreparazione;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIstanza"></param>
        /// <returns></returns>
        protected string GetTipologiaIstanza(object rowIstanza)
        {
            string tipoCons = ((System.Data.DataRowView)rowIstanza).Row["tipo_cons"].ToString();

            WSConservazioneLocale.TipoIstanzaConservazione tipoIstanza = this._tipiIstanze.FirstOrDefault(e => e.Codice == tipoCons);

            if (tipoIstanza != null)
                return tipoIstanza.Descrizione;
            else
                return tipoCons;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIstanza"></param>
        /// <returns></returns>
        protected string GetTotalSize(object rowIstanza)
        {
            int totalSize = 0;

            if (((System.Data.DataRowView)rowIstanza).Row["totalSize"] != DBNull.Value)
                Int32.TryParse(((System.Data.DataRowView)rowIstanza).Row["totalSize"].ToString(), out totalSize);

            return string.Format("{0:n0}", totalSize);
        }

        /// <summary>
        /// Verifica se i supporti dell'istanza sono disponibili o meno
        /// </summary>
        /// <param name="rowIstanza"></param>
        /// <returns></returns>
        protected bool IsSupportiAvailable(object rowIstanza)
        {
            string stato = ((System.Data.DataRowView)rowIstanza).Row["stato"].ToString();

            WSConservazioneLocale.StatoIstanza statoIstanza = _statiIstanze.FirstOrDefault(e => e.Codice == stato);

            if (statoIstanza != null)
                return (statoIstanza.Codice.ToUpper() == "F" || statoIstanza.Codice.ToUpper() == "V" || statoIstanza.Codice.ToUpper() == "C");
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        private WSConservazioneLocale.InfoConservazione _conservazioneSelezionata = null;

        /// <summary>
        /// Reperimento istanza di conservazione selezionata
        /// </summary>
        /// <returns></returns>
        protected WSConservazioneLocale.InfoConservazione GetConservazioneSelezionata()
        {
            if (this._conservazioneSelezionata == null)
            {
                string query = string.Format(" WHERE SYSTEM_ID = '{0}' and id_amm = " + this.infoUtente.idAmministrazione, this.hd_idIstanza.Value);

                WSConservazioneLocale.InfoConservazione[] conservazione = Utils.ConservazioneManager.getAreaConservazioneFiltro(query, this.infoUtente);

                if (conservazione != null && conservazione.Length == 1)
                    this._conservazioneSelezionata = conservazione[0];
            }

            return this._conservazioneSelezionata;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool IsEnabledVerificaIntegritaDocumento()
        {
            if (!string.IsNullOrEmpty(this.hd_idIstanza.Value))
            {
                WSConservazioneLocale.InfoConservazione istanzaSelezionata = this.GetConservazioneSelezionata();

                if (istanzaSelezionata != null)
                {
                    int numeroCopie;
                    Int32.TryParse(istanzaSelezionata.numCopie, out numeroCopie);

                    return (istanzaSelezionata.StatoConservazione == "C" &&
                            numeroCopie > 0 &&
                            istanzaSelezionata.TipoConservazione != "CONSERVAZIONE_INTERNA");
                }
                else
                    return false;
            }
            else
                return false;
        }

        protected void SelectedIndexChangedDoc(object sender, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.gv_dettaglioCons.CurrentPageIndex = e.NewPageIndex;
            itemsCons = (WSConservazioneLocale.ItemsConservazione[])Session["itemsCons"];
            caricaGridviewDettaglio(itemsCons);
            this.upDettaglio.Update();
            //  this.Panel_dettaglio.Visible = false;
        }

        protected void btn_verificaLeggibilita_Click(object sender, EventArgs e)
        {
            //WSConservazioneLocale.InfoUtente utente = this.infoUtente; 
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "testLeggibilità", "showTestLeggibilita("+hd_idIstanza.Value+");", true);
            string idIstanza = this.hd_idIstanza.Value;
            this.ClearFlags();
            string filtro = this.Filtra();
            infoCons = ConservazioneManager.getAreaConservazioneFiltro(filtro, infoUtente);
            if (infoCons != null)
                this.caricaGridviewIstanze(infoCons);


            this.gv_istanzeCons.SelectedIndex = -1;
            this.upIstanze.Update();


        }

        protected bool isEnabledVerificaLeggibilitaDocumento()
        {

            if (!string.IsNullOrEmpty(this.hd_idIstanza.Value))
            {
                WSConservazioneLocale.InfoConservazione istanzaSelezionata = this.GetConservazioneSelezionata();

                if (istanzaSelezionata != null)
                {


                    return ((istanzaSelezionata.StatoConservazione == "C" || istanzaSelezionata.StatoConservazione == "V") &&
                            istanzaSelezionata.TipoConservazione != "CONSERVAZIONE_INTERNA");
                }
                else
                    return false;
            }
            else
                return false;

        }

        protected bool isVisibleVerificaIntegritaRemoto()
        {
            return ConservazioneWA.Utils.ConservazioneManager.supportiRimovibiliVerificabili();
        }

        #region Gestione flag di stato nascosti


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected bool GetBooleanHiddenField(HiddenField field)
        {
            bool retValue;
            bool.TryParse(field.Value, out retValue);
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        protected void SetBooleanHiddenField(HiddenField field, bool value)
        {
            field.Value = value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ClearFlags()
        {
            // Annullamento flags
            this.IstanzaInDeroga = false;
            this.VerificaFirmaEffettuata = false;
            this.VerificaMarcaEffettuata = false;
            this.VerificaFormatiEffettuata = false;
            this.AleastOneInvalidDocumentFormat = false;
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool VerificaFirmaEffettuata
        {
            get
            {
                return this.GetBooleanHiddenField(this.hd_verificaFirmaEffettuata);
            }
            set
            {
                this.SetBooleanHiddenField(this.hd_verificaFirmaEffettuata, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool VerificaMarcaEffettuata
        {
            get
            {
                return this.GetBooleanHiddenField(this.hd_verificaMarcaEffettuata);
            }
            set
            {
                this.SetBooleanHiddenField(this.hd_verificaMarcaEffettuata, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool VerificaFormatiEffettuata
        {
            get
            {
                return this.GetBooleanHiddenField(this.hd_verificaFormatiEffettuata);
            }
            set
            {
                this.SetBooleanHiddenField(this.hd_verificaFormatiEffettuata, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AleastOneInvalidDocumentFormat
        {
            get
            {
                return this.GetBooleanHiddenField(this.hd_almostOneInvalidDocumentFormat);
            }
            set
            {
                this.SetBooleanHiddenField(this.hd_almostOneInvalidDocumentFormat, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IstanzaInDeroga
        {
            get
            {
                return this.GetBooleanHiddenField(this.hd_istanza_in_deroga);
            }
            set
            {
                this.SetBooleanHiddenField(this.hd_istanza_in_deroga, value);
            }
        }

        #endregion

    }
}