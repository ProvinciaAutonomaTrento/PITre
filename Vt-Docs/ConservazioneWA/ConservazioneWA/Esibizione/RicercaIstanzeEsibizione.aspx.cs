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

namespace ConservazioneWA.Esibizione
{
    public partial class RicercaIstanzeEsibizione : System.Web.UI.Page
    {
        protected ConservazioneWA.DataSet.DataSetRicerca dataSetRicerca = new ConservazioneWA.DataSet.DataSetRicerca();
        protected ConservazioneWA.DataSet.DataSetRDettaglio dataSetDettaglio = new ConservazioneWA.DataSet.DataSetRDettaglio();
        protected WSConservazioneLocale.InfoConservazione[] infoCons;
        protected WSConservazioneLocale.ItemsConservazione[] itemsCons;
        protected WSConservazioneLocale.InfoUtente infoUtente;
        protected WSConservazioneLocale.InfoAmministrazione amm;

        protected string profiloUtente = null;

        //private ConservazioneWA.WSConservazioneLocale.TipoIstanzaConservazione[] _tipiIstanze = null;

        //private ConservazioneWA.WSConservazioneLocale.StatoIstanza[] _statiIstanze = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            //recupero dalla sessione il parametro che mi dice se sono utente di conservazione o esibizione
            menuTop.ProfiloUtente = Session["GestioneEsibizione"].ToString();

            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);

            //se sono in esibizione
            //cambio il ruolo nell'infoutente con quello corrente e lo rimetto in sessione
            if (menuTop.profiloUtente == "ESIBIZIONE")
            {
                this.uPnl_SelectedRole.Visible = true;
                this.pnl_cons.Visible = false;

                if (HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString() != null)
                    this.infoUtente.idGruppo = HttpContext.Current.Session["RuoloSelezionato_idGruppo"].ToString();
                if (HttpContext.Current.Session["RuoloSelezionato_idCorrGlobali"].ToString() != null)
                    this.infoUtente.idCorrGlobali = HttpContext.Current.Session["RuoloSelezionato_idCorrGlobali"].ToString();

                HttpContext.Current.Session["infoutCons"] = this.infoUtente;
            }
            else
            {
                this.uPnl_SelectedRole.Visible = false;
                this.pnl_cons.Visible = true;
            }
                               
            //_tipiIstanze = ConservazioneManager.GetTipiIstanza();
            //_statiIstanze = ConservazioneManager.GetStatiIstanza();
            this.profiloUtente = Session["GestioneEsibizione"].ToString();

            if (!IsPostBack)
            {
                this.GestioneGrafica();

                if (Request.QueryString["q"] != null && !string.IsNullOrEmpty(Request.QueryString["q"].ToString()))
                {
                    //se accedo dal riepilogo dell'homepage di conservazione
                    //recupero il tipo di istanze da visualizzare dalla querystring, 
                    //effettuo la ricerca e carico la datagrid
                    string filtro = Request.QueryString["q"].ToString();
                    this.chkTipo.SelectedValue = filtro;

                    WSConservazioneLocale.InfoEsibizione[] infoEs = Utils.ConservazioneManager.GetInfoEsibizione(this.infoUtente, this.Filtra());
                    Session["infoEs"] = infoEs;
                    this.caricaGridViewIstanze(infoEs);

                }

                #region OLD CODE
                /*

                if (Request.QueryString["q"] != null && !string.IsNullOrEmpty(Request.QueryString["q"].ToString()))
                {
                    string filtro = Request.QueryString["q"].ToString();
                    //filtro (se accedo dal counter dell'homepage)


                    if (this.infoCons != null)
                    {
                        //this.caricaGridviewIstanze(this.infoCons);
                    }
                    //this.chkTipo.SelectedValue = filtro;
                }

                
                if (Request.QueryString["id"] != null && !string.IsNullOrEmpty(Request.QueryString["id"].ToString()))
                {
                    string idIstanza = Request.QueryString["id"].ToString();
                    string query = " WHERE SYSTEM_ID= " + idIstanza + " and id_amm = " + this.infoUtente.idAmministrazione;
                    infoCons = ConservazioneManager.getAreaConservazioneFiltro(query, infoUtente);
                    if (infoCons != null)
                    {
                        //this.caricaGridviewIstanze(infoCons);
                    }
                    this.txt_idIstanza.Text = idIstanza;
                    itemsCons = ConservazioneManager.getItemsConservazione(idIstanza, infoUtente);

                    if (itemsCons != null)
                    {
                        //this.hd_idIstanza.Value = idIstanza;
                        //this.caricaGridviewDettaglio(itemsCons);


                        //abilitaFunzioniByCodStato(infoCons[0].StatoConservazione);

                    }
                }
                */
                #endregion
            }

            //se l'istanza viene messa in lavorazione
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
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

        protected List<WSConservazioneLocale.FiltroRicerca> Filtra()
        {

            List<WSConservazioneLocale.FiltroRicerca> filters = new List<WSConservazioneLocale.FiltroRicerca>();

            #region profilo utente

            filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "profiloUtente", valore = this.profiloUtente });

            #endregion

            #region amministrazione

            filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "idAmm", valore = infoUtente.idAmministrazione });
            
            #endregion

            #region tipo

            //nuove
            if (this.chkTipo.Items[0].Selected)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "nuoveIstEsib", valore = "1" });
            else
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "nuoveIstEsib", valore = "0" });
            //da certificare
            if (this.chkTipo.Items[1].Selected)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "certIstEsib", valore = "1" });
            else
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "certIstEsib", valore = "0" });
            //rifiutate
            if (this.chkTipo.Items[2].Selected)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "rifIstEsib", valore = "1" });
            else
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "rifIstEsib", valore = "0" });
            //in chiusura
            if (this.chkTipo.Items[3].Selected)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "trIstEsib", valore = "1" });
            else
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "trIstEsib", valore = "0" });
            //chiuse
            if (this.chkTipo.Items[4].Selected)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "chIstEsib", valore = "1" });
            else
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "chIstEsib", valore = "0" });

            #endregion

            #region id istanza

            if (txt_idIstanza.Text != string.Empty)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "idIstanza", valore = txt_idIstanza.Text });

            #endregion

            #region descrizione

            if (txt_descIstanza.Text != string.Empty)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "descIstanza", valore = txt_descIstanza.Text });

            #endregion

            #region certificazione

            //con certificazione
            if (this.chkTipologia.Items[0].Selected && !this.chkTipologia.Items[1].Selected)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "certificazione", valore = "1" });
            //senza certificazione
            else if (!this.chkTipologia.Items[0].Selected && this.chkTipologia.Items[1].Selected)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "certificazione", valore = "0" });

            #endregion

            #region data creazione

            //valore singolo
            if (this.ddl_dataCr.SelectedIndex == 0)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "DATA_CREAZIONE_IL", valore = this.txt_dataCr_da.Text });

            //intervallo
            if (this.ddl_dataCr.SelectedIndex == 1)
            {
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "DATA_CREAZIONE_SUCCESSIVA_AL", valore = this.txt_dataCr_da.Text });
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "DATA_CREAZIONE_PRECEDENTE_IL", valore = this.txt_dataCr_a.Text });
            }


            #endregion

            #region data certificazione

            //valore singolo
            if (this.ddl_dataCert.SelectedIndex == 0)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "DATA_CERTIFICAZIONE_IL", valore = this.txt_dataCert_da.Text });

            //intervallo
            if (this.ddl_dataCert.SelectedIndex == 1)
            {
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "DATA_CERTIFICAZIONE_SUCCESSIVA_AL", valore = this.txt_dataCert_da.Text });
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "DATA_CERTIFICAZIONE_PRECEDENTE_IL", valore = this.txt_dataCert_a.Text });
            }

            #endregion

            #region data chiusura

            //valore singolo
            if (this.ddl_dataChiusura.SelectedIndex == 0)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "DATA_CHIUSURA_IL", valore = this.txt_dataChiusura_da.Text });

            //intervallo
            if (this.ddl_dataChiusura.SelectedIndex == 1)
            {
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "DATA_CHIUSURA_DAL", valore = this.txt_dataChiusura_da.Text });
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "DATA_CHIUSURA_AL", valore = this.txt_dataChiusura_a.Text });
            }

            #endregion

            #region data rifiuto

            //valore singolo
            if (this.ddl_dataRifiuto.SelectedIndex == 0)
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "DATA_RIFIUTO_IL", valore = this.txt_dataRifiuto_da.Text });

            //intervallo
            if (this.ddl_dataRifiuto.SelectedIndex == 1)
            {
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "DATA_RIFIUTO_DAL", valore = this.txt_dataRifiuto_da.Text });
                filters.Add(new WSConservazioneLocale.FiltroRicerca() { argomento = "DATA_RIFIUTO_AL", valore = this.txt_dataRifiuto_a.Text });
            }

            #endregion

            return filters;

        }

        #region ddl date

        protected void ddl_DataCr_IndexChanged(object sender, EventArgs e)
        {
            switch (ddl_dataCr.SelectedIndex)
            {
                //valore singolo
                case 0:
                    this.lbl_dataCr_a.Visible = false;
                    this.lbl_dataCr_da.Visible = true;
                    this.lbl_dataCr_da.Text = "Il";
                    this.txt_dataCr_da.Visible = true;
                    this.txt_dataCr_a.Visible = false;
                    break;

                //intervallo
                case 1:
                    this.lbl_dataCr_da.Visible = true;
                    this.lbl_dataCr_da.Text = "Da";
                    this.lbl_dataCr_a.Visible = true;
                    this.txt_dataCr_da.Visible = true;
                    this.txt_dataCr_a.Visible = true;
                    break;

            }

        }

        protected void ddl_DataCert_IndexChanged(object sender, EventArgs e)
        {
            switch (ddl_dataCert.SelectedIndex)
            {
                //valore singolo
                case 0:
                    this.lbl_dataCert_da.Visible = true;
                    this.lbl_dataCert_da.Text = "Il";
                    this.lbl_dataCert_a.Visible = false;
                    this.txt_dataCert_da.Visible = true;
                    this.txt_dataCert_a.Visible = false;
                    break;

                //intervallo
                case 1:
                    this.lbl_dataCert_da.Visible = true;
                    this.lbl_dataCert_da.Text = "Da";
                    this.lbl_dataCert_a.Visible = true;
                    this.txt_dataCert_da.Visible = true;
                    this.txt_dataCert_a.Visible = true;
                    break;
            }


        }

        protected void ddl_DataChiusura_IndexChanged(object sender, EventArgs e)
        {
            switch (ddl_dataChiusura.SelectedIndex)
            {
                //valore singolo
                case 0:
                    this.lbl_dataChiusura_da.Visible = true;
                    this.lbl_dataChiusura_da.Text = "Il";
                    this.lbl_dataChiusura_a.Visible = false;
                    this.txt_dataChiusura_da.Visible = true;
                    this.txt_dataChiusura_a.Visible = false;
                    break;

                //intervallo
                case 1:
                    this.lbl_dataChiusura_da.Visible = true;
                    this.lbl_dataChiusura_da.Text = "Da";
                    this.lbl_dataChiusura_a.Visible = true;
                    this.txt_dataChiusura_da.Visible = true;
                    this.txt_dataChiusura_a.Visible = true;
                    break;

            }


        }

        protected void ddl_DataRifiuto_IndexChanged(object sender, EventArgs e)
        {
            switch (ddl_dataRifiuto.SelectedIndex)
            {
                //valore singolo
                case 0:
                    this.lbl_dataRifiuto_da.Visible = true;
                    this.lbl_dataRifiuto_da.Text = "Il";
                    this.lbl_dataRifiuto_a.Visible = false;
                    this.txt_dataRifiuto_da.Visible = true;
                    this.txt_dataRifiuto_a.Visible = false;
                    break;

                //intervallo
                case 1:
                    this.lbl_dataRifiuto_da.Visible = true;
                    this.lbl_dataRifiuto_da.Text = "Da";
                    this.lbl_dataRifiuto_a.Visible = true;
                    this.txt_dataRifiuto_da.Visible = true;
                    this.txt_dataRifiuto_a.Visible = true;
                    break;

            }

        }

        #endregion

        private ConservazioneWA.UserControl.Calendar GetCalendarControl(string controlId)
        {
            return (ConservazioneWA.UserControl.Calendar)this.FindControl(controlId);
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
           
            //imposto i filtri di ricerca
            List<WSConservazioneLocale.FiltroRicerca> filters = this.Filtra();

            //carico i dati
            WSConservazioneLocale.InfoEsibizione[] infoEs = ConservazioneManager.GetInfoEsibizione(infoUtente, filters);
            Session["infoEs"] = infoEs;

            //inizializzo e carico la datagrid
            this.gv_IstanzeEs.CurrentPageIndex = 0;
            this.gv_IstanzeEs.SelectedIndex = -1;
            this.caricaGridViewIstanze(infoEs);
            this.upIstanze.Update();
            
            this.mdlPopupWait.Hide();
        }

        protected void BtnReset_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < this.chkTipo.Items.Count; i++)
            {
                this.chkTipo.Items[i].Selected = false;
            }

            this.txt_idIstanza.Text = string.Empty;
            this.txt_descIstanza.Text = string.Empty;

            this.chkTipologia.Items[0].Selected = false;
            this.chkTipologia.Items[1].Selected = false;

            this.ddl_dataCr.SelectedIndex = 1;
            this.ddl_dataCert.SelectedIndex = 1;
            this.ddl_dataChiusura.SelectedIndex = 1;
            this.ddl_dataRifiuto.SelectedIndex = 1;

            this.txt_dataCr_da.Text = string.Empty;
            this.txt_dataCr_a.Text = string.Empty;
            this.txt_dataCert_da.Text = string.Empty;
            this.txt_dataCert_a.Text = string.Empty;
            this.txt_dataChiusura_da.Text = string.Empty;
            this.txt_dataChiusura_a.Text = string.Empty;
            this.txt_dataRifiuto_da.Text = string.Empty;
            this.txt_dataRifiuto_a.Text = string.Empty;

            this.txt_dataCr_da.Visible = true;
            this.txt_dataCr_a.Visible = true;
            this.txt_dataCert_da.Visible = true;
            this.txt_dataCert_a.Visible = true;
            this.txt_dataChiusura_da.Visible = true;
            this.txt_dataChiusura_a.Visible = true;
            this.txt_dataRifiuto_da.Visible = true;
            this.txt_dataRifiuto_a.Visible = true;

            this.lbl_dataCr_da.Text = "Da";
            this.lbl_dataCert_da.Text = "Da";
            this.lbl_dataChiusura_da.Text = "Da";
            this.lbl_dataRifiuto_da.Text = "Da";

            this.lbl_dataCr_a.Visible = true;
            this.lbl_dataCert_a.Visible = true;
            this.lbl_dataChiusura_a.Visible = true;
            this.lbl_dataRifiuto_a.Visible = true;

        }

        protected void GestioneGrafica()
        {
            btnFind.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btnFind.Attributes.Add("onmouseout", "this.className='cbtn';");

            btnReset.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btnReset.Attributes.Add("onmouseout", "this.className='cbtn';");

            amm = Utils.ConservazioneManager.GetInfoAmmCorrente(this.infoUtente.idAmministrazione);
            this.lbl_amm.Text = this.amm.Codice + " - " + this.amm.Descrizione;
            this.lbl_amm_cons.Text = this.amm.Codice + " - " + this.amm.Descrizione;
            

        }

        protected void GestionePulsanti(string stato)
        {

            #region GESTIONE PULSANTI ENABLED/DISABLED

            switch (stato)
            {
                //ISTANZE NUOVE
                case "N":
                    btnRifiuta.Visible = false;
                    btnScarica.Visible = false;
                    btnRiabilita.Visible = false;
                    btnDocCert.Visible = false;
                    rowNoteRifiuto.Visible = false;
                    if (this.profiloUtente == "ESIBIZIONE")
                    {
                        txtIstDescr.Enabled = true;
                        txtIstNote.Enabled = true;
                        btnSalva.Enabled = true;
                        btnSalva.Visible = true;
                        btnDocumenti.Enabled = true;
                        btnDocumenti.Visible = true;
                        btnRichiediCertificazione.Enabled = true;
                        btnRichiediCertificazione.Visible = true;
                        btnCertifica.Visible = false;
                        btnChiudi.Enabled = true;
                        btnChiudi.Visible = true;
                        btnElimina.Enabled = true;
                        btnElimina.Visible = true;
                        btnEliminaDoc.Enabled = true;
                    }
                    else
                    {
                        txtIstDescr.Enabled = false;
                        txtIstNote.Enabled = false;
                        btnSalva.Visible = false;
                        btnDocumenti.Visible = true;
                        btnDocumenti.Enabled = true;
                        btnRichiediCertificazione.Visible = false;
                        btnCertifica.Visible = false;
                        btnChiudi.Visible = false;
                        btnElimina.Visible = false;
                        btnEliminaDoc.Visible = false;
                    }
                    break;

                //ISTANZE DA CERTIFICARE
                case "I":
                    btnSalva.Visible = false;
                    btnRichiediCertificazione.Visible = false;
                    btnScarica.Visible = false;
                    btnRiabilita.Visible = false;
                    btnDocCert.Visible = false;
                    btnElimina.Visible = false;
                    rowNoteRifiuto.Visible = false;
                    if (this.profiloUtente == "ESIBIZIONE")
                    {
                        txtIstDescr.Enabled = false;
                        txtIstNote.Enabled = false;
                        btnDocumenti.Enabled = true;
                        btnDocumenti.Visible = true;
                        btnCertifica.Visible = false;
                        btnChiudi.Enabled = false;
                        btnChiudi.Visible = false;
                        btnRifiuta.Enabled = false;
                        btnRifiuta.Visible = false;
                    }
                    else
                    {
                        txtIstDescr.Enabled = false;
                        txtIstNote.Enabled = false;
                        btnDocumenti.Enabled = true;
                        btnDocumenti.Visible = true;
                        btnCertifica.Visible = true;
                        btnCertifica.Enabled = true;
                        btnCertifica.OnClientClick = string.Format("return showDialogFirma('{0}','{1}', 'y');", this.hd_idIstanza.Value, this.hd_idCertificazione.Value);
                        btnChiudi.Enabled = false;
                        btnChiudi.Visible = false;
                        btnRifiuta.Enabled = true;
                        btnRifiuta.Visible = true;
                    }
                    break;

                //ISTANZE RIFIUTATE
                case "R":
                    txtIstDescr.Enabled = false;
                    txtIstNote.Enabled = false;
                    btnSalva.Visible = false;
                    btnRichiediCertificazione.Visible = false;
                    btnCertifica.Visible = false;
                    btnRifiuta.Visible = false;
                    btnChiudi.Visible = false;
                    btnElimina.Visible = false;
                    btnScarica.Visible = false;
                    btnDocCert.Visible = false;
                    rowNoteRifiuto.Visible = true;
                    txtIstNoteRifiuto.Enabled = false;
                    if (this.profiloUtente == "ESIBIZIONE")
                    {
                        btnDocumenti.Visible = true;
                        btnDocumenti.Enabled = true;
                        btnRiabilita.Visible = true;
                        btnRiabilita.Enabled = true;
                    }
                    else
                    {
                        btnDocumenti.Visible = true;
                        btnDocumenti.Enabled = true;
                        btnRiabilita.Visible = false;
                    }
                    break;


                //ISTANZE IN CHIUSURA
                case "T":
                    txtIstDescr.Enabled = false;
                    txtIstNote.Enabled = false;
                    btnSalva.Visible = false;
                    btnDocumenti.Visible = false;
                    btnDocumenti.Enabled = true;
                    btnRichiediCertificazione.Visible = false;
                    btnCertifica.Visible = false;
                    btnElimina.Visible = false;
                    btnChiudi.Visible = false; 
                    btnRifiuta.Visible = false;
                    btnScarica.Visible = false;
                    btnRiabilita.Visible = false;
                    btnDocCert.Visible = false;
                    rowNoteRifiuto.Visible = false;
                    break;

                //ISTANZE CHIUSE
                case "C":
                    txtIstDescr.Enabled = false;
                    txtIstNote.Enabled = false;
                    btnSalva.Visible = false;
                    btnDocumenti.Enabled = true;
                    btnDocumenti.Visible = true;
                    btnRichiediCertificazione.Visible = false;
                    btnCertifica.Visible = false;
                    btnElimina.Visible = false;
                    btnChiudi.Visible = false;
                    btnRifiuta.Visible = false;
                    btnRiabilita.Visible = false;
                    rowNoteRifiuto.Visible = false;
                    if (!string.IsNullOrEmpty(this.hd_idCertificazione.Value))
                    {
                        btnDocCert.Visible = true;
                        btnDocCert.Enabled = true;
                        btnDocCert.OnClientClick = string.Format("return showDialogFirma('{0}','{1}', 'n');", this.hd_idIstanza.Value, this.hd_idCertificazione.Value);
                    }
                    else
                        btnDocCert.Visible = false;
                    if (this.profiloUtente == "ESIBIZIONE")
                    {
                        btnScarica.Enabled = true;
                        btnScarica.Visible = true;
                    }
                    else
                    {
                        btnScarica.Visible = false;
                    }

                    break;


            }

            #endregion

            #region GESTIONE CLASSI GRAFICHE

            if (btnSalva.Enabled)
            {
                btnSalva.Attributes.Remove("class");
                btnSalva.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnSalva.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnSalva.Attributes.Add("class", "cbtn");
                btnSalva.ToolTip = "Salva le modifiche";
            }
            else
            {
                btnSalva.Attributes.Remove("onmouseover");
                btnSalva.Attributes.Remove("onmouseout");
                btnSalva.Attributes.Remove("class");
                btnSalva.Attributes.Add("class", "cbtnDisabled");
                btnSalva.ToolTip = string.Empty;
            }

            if (btnDocumenti.Enabled)
            {
                btnDocumenti.Attributes.Remove("class");
                btnDocumenti.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnDocumenti.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnDocumenti.Attributes.Add("class", "cbtn");
                btnDocumenti.ToolTip = "Visualizza il contenuto dell'istanza";
            }
            else
            {
                btnDocumenti.Attributes.Remove("onmouseover");
                btnDocumenti.Attributes.Remove("onmouseout");
                btnDocumenti.Attributes.Remove("class");
                btnDocumenti.Attributes.Add("class", "cbtnDisabled");
                btnDocumenti.ToolTip = string.Empty;
            }

            if (btnRichiediCertificazione.Enabled)
            {
                btnRichiediCertificazione.Attributes.Remove("class");
                btnRichiediCertificazione.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnRichiediCertificazione.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnRichiediCertificazione.Attributes.Add("class", "cbtn");
                btnRichiediCertificazione.ToolTip = "Richiedi la certificazione dell'istanza";
            }
            else
            {
                btnRichiediCertificazione.Attributes.Remove("onmouseover");
                btnRichiediCertificazione.Attributes.Remove("onmouseout");
                btnRichiediCertificazione.Attributes.Remove("class");
                btnRichiediCertificazione.Attributes.Add("class", "cbtnDisabled");
                btnRichiediCertificazione.ToolTip = string.Empty;
            }

            if (btnCertifica.Enabled)
            {
                btnCertifica.Attributes.Remove("class");
                btnCertifica.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnCertifica.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnCertifica.Attributes.Add("class", "cbtn");
                btnCertifica.ToolTip = "Certifica l'istanza";
            }
            else
            {
                btnCertifica.Attributes.Remove("onmouseover");
                btnCertifica.Attributes.Remove("onmouseout");
                btnCertifica.Attributes.Remove("class");
                btnCertifica.Attributes.Add("class", "cbtnDisabled");
                btnCertifica.ToolTip = string.Empty;
            }

            if (btnChiudi.Enabled)
            {
                btnChiudi.Attributes.Remove("class");
                btnChiudi.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnChiudi.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnChiudi.Attributes.Add("class", "cbtn");
                btnChiudi.ToolTip = "Chiudi l'istanza";
            }
            else
            {
                btnChiudi.Attributes.Remove("onmouseover");
                btnChiudi.Attributes.Remove("onmouseout");
                btnChiudi.Attributes.Remove("class");
                btnChiudi.Attributes.Add("class", "cbtnDisabled");
                btnChiudi.ToolTip = string.Empty;
            }
            if (btnElimina.Enabled)
            {
                btnElimina.Attributes.Remove("class");
                btnElimina.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnElimina.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnElimina.Attributes.Add("class", "cbtn");
                btnElimina.ToolTip = "Elimina l'istanza";
            }
            else
            {
                btnElimina.Attributes.Remove("onmouseover");
                btnElimina.Attributes.Remove("onmouseout");
                btnElimina.Attributes.Remove("class");
                btnElimina.Attributes.Add("class", "cbtnDisabled");
                btnElimina.ToolTip = string.Empty;
            }

            if (btnRifiuta.Enabled)
            {
                btnRifiuta.Attributes.Remove("class");
                btnRifiuta.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnRifiuta.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnRifiuta.Attributes.Add("class", "cbtn");
                btnRifiuta.ToolTip = "Rifiuta l'istanza";
            }
            else
            {
                btnRifiuta.Attributes.Remove("onmouseover");
                btnRifiuta.Attributes.Remove("onmouseout");
                btnRifiuta.Attributes.Remove("class");
                btnRifiuta.Attributes.Add("class", "cbtnDisabled");
                btnRifiuta.ToolTip = string.Empty;
            }

            if (btnRiabilita.Enabled)
            {
                btnRiabilita.Attributes.Remove("class");
                btnRiabilita.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnRiabilita.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnRiabilita.Attributes.Add("class", "cbtn");
                btnRiabilita.ToolTip = "Riabilita l'istanza";
            }
            else
            {
                btnRiabilita.Attributes.Remove("onmouseover");
                btnRiabilita.Attributes.Remove("onmouseout");
                btnRiabilita.Attributes.Remove("class");
                btnRiabilita.Attributes.Add("class", "cbtnDisabled");
                btnRiabilita.ToolTip = string.Empty;
            }

            if (btnScarica.Enabled)
            {
                btnScarica.Attributes.Remove("class");
                btnScarica.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnScarica.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnScarica.Attributes.Add("class", "cbtn");
                btnScarica.ToolTip = "Scarica l'istanza";
            }
            else
            {
                btnScarica.Attributes.Remove("onmouseover");
                btnScarica.Attributes.Remove("onmouseout");
                btnScarica.Attributes.Remove("class");
                btnScarica.Attributes.Add("class", "cbtnDisabled");
                btnScarica.ToolTip = string.Empty;
            }

            if (btnDocCert.Enabled)
            {
                btnDocCert.Attributes.Remove("class");
                btnDocCert.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnDocCert.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnDocCert.Attributes.Add("class", "cbtn");
                btnDocCert.ToolTip = "Visualizza il documento di certificazione dell'istanza";
            }
            else
            {
                btnDocCert.Attributes.Remove("onmouseover");
                btnDocCert.Attributes.Remove("onmouseout");
                btnDocCert.Attributes.Remove("class");
                btnDocCert.Attributes.Add("class", "cbtnDisabled");
                btnDocCert.ToolTip = string.Empty;
            }

            if (btnEliminaDoc.Enabled)
            {
                btnEliminaDoc.Attributes.Remove("class");
                btnEliminaDoc.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnEliminaDoc.Attributes.Add("onmouseout", "this.className='cbtn';");
                btnEliminaDoc.Attributes.Add("class", "cbtn");
                btnEliminaDoc.ToolTip = "Elimina i documenti selezionati dall'istanza di esibizione";
            }
            else
            {
                btnEliminaDoc.Attributes.Remove("onmouseover");
                btnEliminaDoc.Attributes.Remove("onmouseout");
                btnEliminaDoc.Attributes.Remove("class");
                btnEliminaDoc.Attributes.Add("class", "cbtnDisabled");
                btnEliminaDoc.ToolTip = string.Empty;
            }

            #endregion

        }

        protected void caricaGridViewIstanze(WSConservazioneLocale.InfoEsibizione[] infoEs)
        {
            gv_IstanzeEs.DataSource = infoEs;
            gv_IstanzeEs.DataBind();

            this.panel_dettaglio.Visible = false;
            this.panel_Documenti.Visible = false;
            this.upDettaglio.Update();
        }

        protected void caricaGridViewDocumenti(WSConservazioneLocale.ItemsConservazione[] items)
        {
            this.CalcolaDimensioneIstanza(items);

            gv_DocumentiEs.DataSource = items;
            //se ho eliminato l'ultimo elemento dell'ultima pagina
            //devo reimpostare come pagina corrente la pagina immediatamente precedente
            if (gv_DocumentiEs.CurrentPageIndex == gv_DocumentiEs.PageCount)
            {
                if (gv_DocumentiEs.PageCount == 1)
                    gv_DocumentiEs.CurrentPageIndex = 0;
                else
                {
                    if (gv_DocumentiEs.CurrentPageIndex > 0)
                    {
                        gv_DocumentiEs.CurrentPageIndex = gv_DocumentiEs.CurrentPageIndex - 1;
                    }
                }
            }

            gv_DocumentiEs.DataBind();

            this.panel_Documenti.Visible = true;
            if (this.hd_stato.Value == "N")
            {
                this.pnl_bottoniera.Visible = true;
            }
            else
            {
                this.pnl_bottoniera.Visible = false;
            }
            this.upDettaglio.Update();


        }

        protected void gv_IstanzeEs_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (this.gv_IstanzeEs.Controls.Count > 0)
                {
                    Table tab = (Table)this.gv_IstanzeEs.Controls[0];
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
        }

        protected void gv_DocumentiEs_PreRender(object sender, EventArgs e)
        {
            string statoIstanza = this.hd_stato.Value;
            int n = gv_DocumentiEs.Columns.Count -1 ;
            foreach (DataGridItem item in gv_DocumentiEs.Items)
            {
                CheckBox checkElimina = (CheckBox)item.FindControl("chkElimina");
                if (statoIstanza == "N")
                    checkElimina.Enabled = true;
                else
                    checkElimina.Enabled = false;

                if (!string.IsNullOrEmpty(((Label)item.FindControl("lblNumProt")).Text))
                {
                    ((Label)item.FindControl("lblDataProtOrCrea")).ForeColor = Color.Red;
                }

            }


               
        }

        protected void SelectedIndexChangedIst(object sender, DataGridPageChangedEventArgs e)
        {
            this.gv_IstanzeEs.CurrentPageIndex = e.NewPageIndex;

            this.caricaGridViewIstanze((WSConservazioneLocale.InfoEsibizione[])Session["infoEs"]);
            this.upIstanze.Update();

        }

        protected void SelectedIndexChangedDoc(object sender, DataGridPageChangedEventArgs e)
        {
            this.gv_DocumentiEs.CurrentPageIndex = e.NewPageIndex;

            this.caricaGridViewDocumenti((WSConservazioneLocale.ItemsEsibizione[])Session["itemsEs"]);
            this.upDettaglio.Update();

        }

        protected void btnSelezionaIstanza_Click(object sender, EventArgs e)
        {
            
            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            //Label a = (Label)dgItem.FindControl("lblIdIstanza");
            gv_IstanzeEs.SelectedIndex = dgItem.ItemIndex;

            string idIstanza = ((Label)dgItem.FindControl("lblIdIstanza")).Text;
            string stato = ((Label)dgItem.FindControl("lblChaStato")).Text;
            string idCertificazione = ((Label)dgItem.FindControl("lblIdCert")).Text;

            if (idIstanza != this.hd_idIstanza.Value)
            {
                this.panel_Documenti.Visible = false;
            }
            
            //valorizzo i campi hidden
            this.hd_idIstanza.Value = idIstanza;
            this.hd_stato.Value = stato;
            this.hd_idCertificazione.Value = idCertificazione;
            this.txtIstDescr.Text = ((Label)dgItem.FindControl("lblDescrizione")).Text;
            this.txtIstNote.Text = ((Label)dgItem.FindControl("lblNote")).Text;
            if (stato == "R")
                this.txtIstNoteRifiuto.Text = ((Label)dgItem.FindControl("lblNoteRifiuto")).Text;

            
            this.GestionePulsanti(stato);

            this.panel_dettaglio.Visible = true;
            this.upDettaglio.Update();
            this.upIstanze.Update();
        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {
            //recupero i valori dai campi
            string descrizione = this.txtIstDescr.Text;
            string note = this.txtIstNote.Text;

            bool esito = ConservazioneManager.SaveEsibizioneFields(this.infoUtente, this.hd_idIstanza.Value, descrizione, note);
            if (!esito)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "saveFieldsKO", "alert('Errore nell'aggiornamento delle informazioni');", true);
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "saveFieldsOK", "alert('Le informazioni sono state aggiornate.');", true);
                WSConservazioneLocale.InfoEsibizione[] infoEs = ConservazioneManager.GetInfoEsibizione(this.infoUtente, this.Filtra());
                this.caricaGridViewIstanze(infoEs);
                panel_dettaglio.Visible = true;

            }
            this.upDettaglio.Update();


        }

        protected void btnDocumenti_Click(object sender, EventArgs e)
        {
            string idIstanza = this.hd_idIstanza.Value;
            
            //carico i documenti appartenenti all'istanza e li conservo in sessione
            WSConservazioneLocale.ItemsConservazione[] itemsEs = ConservazioneManager.GetItemsEsibizione(this.infoUtente, idIstanza);
            Session["itemsEs"] = itemsEs;

            this.gv_DocumentiEs.CurrentPageIndex = 0;
            this.gv_DocumentiEs.SelectedIndex = -1;
            this.caricaGridViewDocumenti(itemsEs);

        }

        protected void btnRichiediCertificazione_Click(object sender, EventArgs e)
        {
            string idIstanza = this.hd_idIstanza.Value;
            bool conferma;
            Boolean.TryParse(this.hd_richiediCert.Value, out conferma);

            if (conferma)
            {
                if (!string.IsNullOrEmpty(this.txtIstDescr.Text))
                {
                    string descrizione = this.txtIstDescr.Text;
                    string note = this.txtIstNote.Text;
                    bool esito = ConservazioneManager.RichiediCertificazioneIstanzaEsibizione(infoUtente, idIstanza, descrizione, note);

                    if (!esito)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "richiediCertifKO", "alert('Si è verificato un errore nella richiesta.');", true);
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "richiediCertifOK", "alert('Richiesta di certificazione inoltrata con successo.');", true);
                        WSConservazioneLocale.InfoEsibizione[] infoEs = ConservazioneManager.GetInfoEsibizione(this.infoUtente, this.Filtra());
                        this.caricaGridViewIstanze(infoEs);
                        panel_dettaglio.Visible = false;
                    }
                    this.upDettaglio.Update();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "checkDescrizione", "controllaDescrizione();", true);
                }
            }


        }

        protected void btnChiudi_Click(object sender, EventArgs e)
        {
            string idIstanza = this.hd_idIstanza.Value;
            
            bool conferma;
            Boolean.TryParse(this.hd_chiudi.Value, out conferma);

            if (conferma)
            {
                if (!string.IsNullOrEmpty(this.txtIstDescr.Text))
                {

                    ConservazioneManager.ChiudiIstanzaEsibizione(this.infoUtente, this.hd_idIstanza.Value, this.txtIstDescr.Text, this.txtIstNote.Text);
                    WSConservazioneLocale.InfoEsibizione[] infoEs = ConservazioneManager.GetInfoEsibizione(this.infoUtente, this.Filtra());
                    this.caricaGridViewIstanze(infoEs);
                    panel_dettaglio.Visible = false;
                    this.upDettaglio.Update();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "checkDescrizione", "controllaDescrizione();", true);
                }

            }

        }

        protected void btnEliminaDoc_Click(object sender, EventArgs e)
        {
            bool conferma;
            Boolean.TryParse(this.hd_eliminaDoc.Value, out conferma);

            if (conferma)
            {

                string selectedIDs = string.Empty;

                foreach (DataGridItem dgItem in gv_DocumentiEs.Items)
                {
                    CheckBox chkSelected = (CheckBox)dgItem.FindControl("chkElimina");
                    if (chkSelected.Checked)
                    {
                        string sysId = ((Label)dgItem.FindControl("lblIdDoc")).Text;
                        selectedIDs += string.Format("{0},", sysId);
                    }
                }

                if (string.IsNullOrEmpty(selectedIDs))
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "eliminaDocNessunDocSelezionato", "alert('Nessun documento selezionato.');", true);
                else
                {
                    selectedIDs = selectedIDs.Remove(selectedIDs.Length - 1);
                    
                    bool esito = ConservazioneManager.RemoveItemsEsibizione(this.infoUtente, selectedIDs);
                    if (!esito)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "eliminaKO", "alert('Rimozione documenti fallita.');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "eliminaKo", "alert('Rimozione documenti avvenuta con successo.');", true);
                        WSConservazioneLocale.ItemsConservazione[] items = ConservazioneManager.GetItemsEsibizione(this.infoUtente, this.hd_idIstanza.Value);
                        this.caricaGridViewDocumenti(items);
                        this.upDettaglio.Update();

                    }
                    
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "eliminaDocSelezionati", "alert('" + selectedIDs + "');", true);
                }

            }

        }

        protected void btnElimina_Click(object sender, EventArgs e)
        {
            bool conferma;
            Boolean.TryParse(this.hd_eliminaIst.Value, out conferma);

            if (conferma)
            {
                bool esito = Utils.ConservazioneManager.RemoveIstanzaEsibizione(this.infoUtente, this.hd_idIstanza.Value);
                if (!esito)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "eliminaIstKO", "alert('Eliminazione istanza fallita.');", true);
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "eliminaIstOK", "alert('Eliminazione istanza avvenuta con successo.');", true);
                    WSConservazioneLocale.InfoEsibizione[] infoEs = ConservazioneManager.GetInfoEsibizione(this.infoUtente, this.Filtra());
                    this.caricaGridViewIstanze(infoEs);
                    this.upIstanze.Update();
                }
            }

        }

        protected void btnRifiuta_Click(object sender, EventArgs e)
        {
            if (this.hd_noteRifiuto.Value != null && this.hd_noteRifiuto.Value != string.Empty && this.hd_noteRifiuto.Value != "undefined")
            {
                string note = this.hd_noteRifiuto.Value.Replace("'", "''");
                bool esito = ConservazioneManager.RifiutaCertificazioneIstanzaEsibizione(this.infoUtente, this.hd_idIstanza.Value, note);

                if (!esito)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "rifiutaCertifKO", "alert('Si è verificato un errore nel rifiuto della certificazione.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "rifiutaCertifOK", "alert('La richiesta di certificazione è stata rifiutata.');", true);
                    WSConservazioneLocale.InfoEsibizione[] infoEs = ConservazioneManager.GetInfoEsibizione(this.infoUtente, this.Filtra());
                    this.caricaGridViewIstanze(infoEs);
                    panel_dettaglio.Visible = false;

                }
            }

        }

        protected void btnRiabilita_Click(object sender, EventArgs e)
        {

            //controllo se esiste già un'istanza di esibizione
            //in questo caso non posso riabilitare l'istanza
            if (this.CheckIstanzaNuova())
            {
                //esiste un'istanza nuova: non è possibile riabilitare
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noRiabilitazione", "controllaRiabilitazione();", true);
            }
            else
            {

                bool conferma;
                string idEsibizione = this.hd_idIstanza.Value;
                conferma = Boolean.TryParse(this.hd_riabilita.Value, out conferma);

                if (conferma)
                {
                    string newIdEsibizione = ConservazioneManager.RiabilitaIstanzaEsibizione(this.infoUtente, idEsibizione);
                    if (string.IsNullOrEmpty(newIdEsibizione))
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "riabilitaIstKO", "alert('Si è verificato un errore nella riabilitazione dell\'istanza');", true);
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "riabilitaIstOK", "riabilitaOK();", true);
                        WSConservazioneLocale.InfoEsibizione[] infoEs = ConservazioneManager.GetInfoEsibizione(this.infoUtente, this.Filtra());
                        this.caricaGridViewIstanze(infoEs);
                        panel_dettaglio.Visible = false;
                    }
                }

            }

        }

        protected void btnScarica_Click(object sender, EventArgs e)
        {
            string downloadUrl = ConservazioneManager.GetEsibizioneDownloadUrl(this.infoUtente, this.hd_idIstanza.Value);

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "downloadurl", "downloadurl('" + downloadUrl + "');", true);
            //Response.Redirect(downloadUrl, false);
            
            // Chiudo la Rotellina
            this.mdlPopupWait.Hide();
        }

        protected string GetStatoIstanza(object obj)
        {
            string chaStato = obj.ToString();
            string retVal = string.Empty;

            switch (chaStato)
            {
                case "N":
                    retVal = "Nuova";
                    break;
                case "I":
                    retVal = "Da certificare";
                    break;
                case "R":
                    retVal = "Rifiutata";
                    break;
                case "T":
                    retVal = "In chiusura";
                    break;
                case "C":
                    retVal = "Chiusa";
                    break;
            }

            return retVal;
        }

        protected string GetCertificazione(object obj, object stato)
        {
            string retVal = string.Empty;

            bool cert = Convert.ToBoolean(obj);
            string chaStato = stato.ToString();
            if (!(chaStato == "N"))
            {
                if (cert)
                    retVal = "Con certificazione";
                else
                    retVal = "Senza certificazione";
            }

            return retVal;

        }

        protected string GetIdSegnaturaData(object objSegn, object objData)
        {
            string segnatura = objSegn.ToString();
            string data = objData.ToString();

            string retVal = segnatura + "<br />" + data;
            return retVal;


        }

        protected string GetTotalSize(object obj)
        {
            string size;

            if (!obj.Equals(null))
                size = obj.ToString();
            else
                size = string.Empty;

            return string.Format("{0:n0}", size);

        }

        protected string GetIstanze(object obj)
        {

            string link = string.Format("<a href=\"RicercaIstanzeConsEsibizione.aspx?id={0}\">{1}</a>", obj.ToString(), obj.ToString());
            return link;

        }

        protected string SetTipo(object obj)
        {
            if (obj != null)
            {
                if (obj.ToString() == "G")
                    return "NP";
                else
                    return obj.ToString();
            }
            else
            {
                return string.Empty;
            }

        }

        protected string SetRichiedente(object obj)
        {
            string[] richiedente = (obj.ToString()).Split('_');
            string retVal = string.Format("{0}<br/>({1})", richiedente[0], richiedente[1]);
            
            return retVal;
        }

        protected void CalcolaDimensioneIstanza(WSConservazioneLocale.ItemsConservazione[] items)
        {

            float size = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (!string.IsNullOrEmpty(items[i].SizeItem))
                    size = size + Convert.ToSingle(items[i].SizeItem);
            }

            try
            {
                float sizeF = size / 1048576;
                string size_appo = sizeF.ToString();
                if (size_appo.Contains(","))
                    size_appo = size_appo.Substring(0, size_appo.IndexOf(",") + 2);
                else
                {
                    if (size_appo.Contains("."))
                        size_appo = size_appo.Substring(0, size_appo.IndexOf(".") + 2);
                }
                this.lblDimensioneIstanza.Text = string.Format("Numero Documenti: {2} - Totale bytes: {0:n0} - Totale MB.: {1}", size, size_appo, items.Length.ToString());
                this.lblDimensioneIstanza.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);


            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "calcoloDimIstKO", "alert('Errore nel calcolo della dimensione dell'istanza.');", true);
                Debugger.Write(ex);
            }



        }

        /// <summary>
        /// Verifica se esiste già un'istanza nuova
        /// per l'utente e il ruolo corrente
        /// </summary>
        /// <returns></returns>
        protected bool CheckIstanzaNuova()
        {
            bool retVal = true;
            WSConservazioneLocale.ContatoriEsibizione counter = Utils.ConservazioneManager.GetContatoriEsibizione(this.infoUtente, this.infoUtente.idGruppo);
            int nuove = counter.Nuove + counter.Nuove_Certificata;

            if (nuove == 0)
                retVal = false;

            return retVal;

        }


    }
}
