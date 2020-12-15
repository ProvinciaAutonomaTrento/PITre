using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ConservazioneWA.Utils;


namespace ConservazioneWA
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RicercaSupportiIstanze : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        protected WSConservazioneLocale.InfoUtente _infoUtente;

        /// <summary>
        /// 
        /// </summary>
        private WSConservazioneLocale.TipoSupporto[] _tipiSupporto = null;

        /// <summary>
        /// 
        /// </summary>
        private WSConservazioneLocale.StatoSupporto[] _statiSupporto = null;

        protected WSConservazioneLocale.InfoAmministrazione amm;

       
        #region Data Management

        /// <summary>
        /// Preparazione dei filtri 
        /// </summary>
        private void PrepareFilters()
        {
            this.PrepareFiltersTipiSupporto();

            this.PrepareFiltersStatiSupporto();
        }

        /// <summary>
        /// 
        /// </summary>
        private void PrepareFiltersTipiSupporto()
        {
            this.cblFilterTipiSupporto.DataSource = this._tipiSupporto;
            this.cblFilterTipiSupporto.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        private void PrepareFiltersStatiSupporto()
        {
            this.cblFilterStatiSupporto.DataSource = this._statiSupporto;
            this.cblFilterStatiSupporto.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetFilters()
        {
            string filters = string.Empty;


            // --------------------------------------
            // Filtro su id istanza

            if (!string.IsNullOrEmpty(this.NormalizeFilterCriteria(Request.QueryString["idConservazione"])))
            {
                // Istanza chiamante
                filters = string.Format(" A.ID_CONSERVAZIONE= '{0}' ", this.NormalizeFilterCriteria(Request.QueryString["idConservazione"]));
            }
            else
            {
                if (!string.IsNullOrEmpty(this.NormalizeFilterCriteria(this.txtFilterIdIstanza.Text)))
                {
                    // Istanza immessa come filtro
                    filters = string.Format(" A.ID_CONSERVAZIONE= '{0}' ", this.NormalizeFilterCriteria(this.txtFilterIdIstanza.Text.Trim()));
                }
            }

            // --------------------------------------
            // Filtro su Id supporto
            if (!string.IsNullOrEmpty(this.NormalizeFilterCriteria(Request.QueryString["id"])))
            {
                // Istanza chiamante
                filters = string.Format(" A.SYSTEM_ID= '{0}' ", this.NormalizeFilterCriteria(Request.QueryString["id"]));
            }
            else
            {
                if (!string.IsNullOrEmpty(this.NormalizeFilterCriteria(this.txtFilterIdSupporto.Text)))
                {
                    if (!string.IsNullOrEmpty(filters))
                        filters += " AND ";
                    filters += string.Format(" A.SYSTEM_ID = '{0}' ", this.NormalizeFilterCriteria(this.txtFilterIdSupporto.Text));
                }
            }

            // --------------------------------------
            // Filtro su tipo supporto
            string filtersTipo = string.Empty;

            foreach (ListItem itm in this.cblFilterTipiSupporto.Items)
            {
                if (itm.Selected)
                {
                    if (!string.IsNullOrEmpty(filtersTipo))
                        filtersTipo += " OR ";

                    filtersTipo += string.Format(" A.ID_TIPO_SUPPORTO = {0} ", this.NormalizeFilterCriteria(itm.Value));
                }
            }

            if (!string.IsNullOrEmpty(filtersTipo))
            {
                filtersTipo = string.Format(" ({0}) ", filtersTipo);

                if (!string.IsNullOrEmpty(filters))
                    filters += " AND ";
                filters += filtersTipo;
            }


            
            // --------------------------------------
            // Filtro su stato supporto
            string filtersStato = string.Empty;

            foreach (ListItem itm in this.cblFilterStatiSupporto.Items)
            {
                if (itm.Selected)
                {
                    if (!string.IsNullOrEmpty(filtersStato))
                        filtersStato += " OR ";
                    filtersStato += string.Format(" A.CHA_STATO = '{0}' ", itm.Value.Trim());
                }
            }

            if (!string.IsNullOrEmpty(filtersStato))
            {
                filtersStato = string.Format(" ({0}) ", filtersStato);

                if (!string.IsNullOrEmpty(filters))
                    filters += " AND ";
                filters += filtersStato;
            }
            // --------------------------------------


            // --------------------------------------
            // Filtro su collocazione fisica
            if (!string.IsNullOrEmpty(this.NormalizeFilterCriteria(this.txtFilterCollocazioneFisica.Text)))
            {
                if (!string.IsNullOrEmpty(filters))
                    filters += " AND ";

                filters += string.Format(" A.VAR_COLLOCAZIONE_FISICA LIKE '%{0}%' ", this.NormalizeFilterCriteria(this.txtFilterCollocazioneFisica.Text));
            }

            // --------------------------------------
            // Filtro su note
            if (!string.IsNullOrEmpty(this.NormalizeFilterCriteria(this.txtFilterNote.Text)))
            {
                if (!string.IsNullOrEmpty(filters))
                    filters += " AND ";

                filters += string.Format(" A.VAR_NOTE LIKE '%{0}%' ", this.NormalizeFilterCriteria(this.txtFilterNote.Text));
            }

            // --------------------------------------



            // --------------------------------------
            // Filtro su data ultima verifica
            string filterDataUltimaVerifica = this.GetFilterDate(this.txtFilterDataUltimaVerificaFrom.Text, this.txtFilterDataUltimaVerificaTo.Text, "A.DATA_ULTIMA_VERIFICA");

            // --------------------------------------

            // MEV CS 1.5
            // filtro su data ultima verifica integrità
            if (!string.IsNullOrEmpty(filterDataUltimaVerifica))
            {
                if (!string.IsNullOrEmpty(filters))
                    filters = filters + " AND ";
                filters = filters + filterDataUltimaVerifica;
            }

            // filtro su data prossima verifica integrità
            string filterDataProxVerificaInt = this.GetFilterDate(this.txtFilterDataProssimaVerificaFrom.Text, this.txtFilterDataProssimaVerificaTo.Text, "A.DATA_PROX_VERIFICA");
            if (!string.IsNullOrEmpty(filterDataProxVerificaInt))
            {
                if (!string.IsNullOrEmpty(filters))
                    filters = filters + " AND ";
                filters = filters + filterDataProxVerificaInt;
            }

            // filtro su data ultima verifica leggibilità
            string filterDataUltimaVerLeg = this.GetFilterDate(this.txtFilterDataUltimaVerLegFrom.Text, this.txtFilterDataUltimaVerLegTo.Text, "A.DATA_ULTIMA_VERIFICA_LEGG");
            if (!string.IsNullOrEmpty(filterDataUltimaVerLeg))
            {
                if (!string.IsNullOrEmpty(filters))
                    filters = filters + " AND ";
                filters = filters + filterDataUltimaVerLeg;
            }

            // filtro su data prossima verifica leggibilità
            string filterDataProxVerLeg = this.GetFilterDate(this.txtFilterDataProxVerLegFrom.Text, this.txtFilterDataProxVerLegTo.Text, "A.DATA_PROX_VERIFICA_LEGG");
            if (!string.IsNullOrEmpty(filterDataProxVerLeg))
            {
                if (!string.IsNullOrEmpty(filters))
                    filters = filters + " AND ";
                filters = filters + filterDataProxVerLeg;
            }

            // filtro su data produzione
            string filterDataProd = this.GetFilterDate(this.txtFilterDataProduzioneFrom.Text, this.txtFilterDataProduzioneTo.Text, "A.DATA_PRODUZIONE");
            if (!string.IsNullOrEmpty(filterDataProd))
            {
                if (!string.IsNullOrEmpty(filters))
                    filters = filters + " AND ";
                filters = filters + filterDataProd;
            }

            // filtro su data verifica marca
            string filterDataMarca = this.GetFilterDate(this.txtFilterDataScadenzaMarcaFrom.Text, this.txtFilterDataScadenzaMarcaTo.Text, "A.DATA_SCADENZA_MARCA");
            if (!string.IsNullOrEmpty(filterDataMarca))
            {
                if (!string.IsNullOrEmpty(filters))
                    filters = filters + " AND ";
                filters = filters + filterDataMarca;
            }

            // fine MEV CS 1.5

            filters += "and a.ID_CONSERVAZIONE = c.SYSTEM_ID and c.id_amm = " + this._infoUtente.idAmministrazione;

            filters += " ORDER BY A.ID_CONSERVAZIONE DESC, A.COPIA ASC";




            return filters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="dbFieldName"></param>
        /// <returns></returns>
        private string GetFilterDate(string dateFrom, string dateTo, string dbFieldName)
        {
            string filter = string.Empty;

            if (dateFrom != "")
            {
                if (dateTo != "")
                {
                    filter = string.Format(" ({0} >= " + this.DbDateConverter(dateFrom, true) + " AND {0} <= " + this.DbDateConverter(dateTo, false) + ") ", dbFieldName);
                }
                else
                {
                    filter = string.Format(" {0} >= " + this.DbDateConverter(dateFrom, true), dbFieldName);
                }
            }
            else
            {
                if (dateTo != "")
                {
                    filter = string.Format(" {0} <= " + this.DbDateConverter(dateTo, false), dbFieldName);
                }
            }

            return filter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="beginDay"></param>
        /// <returns></returns>
        private string DbDateConverter(string data, bool beginDay)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private string NormalizeFilterCriteria(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
                return filter.Trim().Replace("'", string.Empty);
            else
                return string.Empty;
        }

        /// <summary>
        /// Caricamento dati dei supporti
        /// </summary>
        protected virtual void FetchSupporti()
        {
            string filters = this.GetFilters();

            WSConservazioneLocale.InfoSupporto[] supporti = ConservazioneManager.getInfoSupporto(filters, this._infoUtente);

            this.grdSupporti.DataSource = supporti;
            this.grdSupporti.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <param name="idSupporto"></param>
        protected virtual void FetchStoriaVerifiche(string idIstanza, string idSupporto)
        {
            WSConservazioneLocale.InfoSupporto[] verifiche = ConservazioneManager.getReportVerificheSupporto(idIstanza, idSupporto, this._infoUtente);

            this.grdStoriaVerifiche.DataSource = verifiche;
            this.grdStoriaVerifiche.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSupporto"></param>
        /// <returns></returns>
        protected string GetCodiceTipoSupporto(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            return infoSupporto.TipoSupporto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSupporto"></param>
        /// <returns></returns>
        protected string GetTipoSupporto(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            WSConservazioneLocale.TipoSupporto tipoSupporto = this._tipiSupporto.FirstOrDefault(e => e.TipoSupp == infoSupporto.TipoSupporto);

            if (tipoSupporto == null)
                return infoSupporto.TipoSupporto;
            else
                return tipoSupporto.Descrizione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSupporto"></param>
        /// <returns></returns>
        protected string GetStatoSupporto(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            string stato = string.Empty;

            WSConservazioneLocale.StatoSupporto statoSupporto = this._statiSupporto.FirstOrDefault(e => e.Codice == infoSupporto.statoSupporto);

            if (statoSupporto != null)
                stato = statoSupporto.Descrizione;
            else
                stato = infoSupporto.statoSupporto;

            return stato;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSupporto"></param>
        /// <returns></returns>
        protected bool IsSupportoRemoto(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            return (infoSupporto.TipoSupporto == "REMOTO");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSupporto"></param>
        /// <returns></returns>
        protected bool IsDownlodable(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            return (!string.IsNullOrEmpty(infoSupporto.istanzaDownloadUrl));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSupporto"></param>
        /// <returns></returns>
        protected bool IsBrowsable(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            return (!string.IsNullOrEmpty(infoSupporto.istanzaBrowseUrl));
        }

                protected bool IsDamaged(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            string stato = string.Empty;
            stato = GetStatoSupporto(infoSupporto);
            if (stato == "Danneggiato") return true;
            else return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSupporto"></param>
        /// <returns></returns>
        protected bool IsSupportoRimovibileRegistrato(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            WSConservazioneLocale.TipoSupporto tipoSupporto = this._tipiSupporto.First(e => e.SystemId == infoSupporto.idTipoSupporto);

            if (tipoSupporto.TipoSupp == "RIMOVIBILE")
                return (infoSupporto.statoSupporto == "R" || infoSupporto.statoSupporto == "V" || infoSupporto.statoSupporto == "D");
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSupporto"></param>
        /// <returns></returns>
        protected bool AreSupportiRimovibiliVerificabili(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            if (IsSupportoRimovibileRegistrato(infoSupporto))
            {
                return ConservazioneManager.supportiRimovibiliVerificabili();
            }
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSupporto"></param>
        /// <returns></returns>
        protected string GetEsitoVerifica(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            if (infoSupporto.esitoVerifica == "1")
                return "Riuscita";
            else if (infoSupporto.esitoVerifica == "0")
                return "Fallita";
            else
                return string.Empty;
       }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSupporto"></param>
        /// <returns></returns>
        protected string GetDatiVerifica(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            string retValue = string.Empty;

            //if (!this.IsSupportoRemoto(infoSupporto))

            // MEV CS 1.5
            string dataUltimaVerifica = string.Empty;
            string dataProxVerifica = string.Empty;
            if (!string.IsNullOrEmpty(infoSupporto.dataUltimaVerifica))
            {
                dataUltimaVerifica = infoSupporto.dataUltimaVerifica.Substring(0, 10);
            }
            if (!string.IsNullOrEmpty(infoSupporto.dataProxVerifica))
            {
                dataProxVerifica = infoSupporto.dataProxVerifica.Substring(0, 10);
            }

            // fine MEV CS 1.5


            if(this.IsSupportoRemoto(infoSupporto)||this.AreSupportiRimovibiliVerificabili(infoSupporto))
            {
                retValue = string.Format("Ver. il: {0}<BR/>Ver. num.: {1}<BR/>Esito: {2}<BR/>Perc.: {3}%<BR/>Prossima ver. il: {4}",
                                        //this.ToShortDateString(infoSupporto.dataUltimaVerifica),
                                        dataUltimaVerifica,
                                        infoSupporto.numVerifiche,
                                        this.GetEsitoVerifica(infoSupporto),
                                        infoSupporto.percVerifica,
                                        //this.ToShortDateString(infoSupporto.dataProxVerifica)
                                        dataProxVerifica
                                        );
           
            }
            /*
            else
            {
                //retValue = string.Format("Esito: {0}<BR/>Perc.: {1}%",
                //                        this.GetEsitoVerifica(infoSupporto),
                //                        infoSupporto.percVerifica);
            }
            */
            return retValue;
        }

        protected string GetDatiVerificaLeggibilita(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            string retValue = string.Empty;

            //if (this.IsSupportoRemoto(infoSupporto) || this.AreSupportiRimovibiliVerificabili(infoSupporto))
            if(this.IsSupportoRemoto(infoSupporto))
            {
                string esito = string.Empty;
                if (infoSupporto.esitoVerificaLeggibilita == "1")
                    esito = "Riuscita";
                else if (infoSupporto.esitoVerificaLeggibilita == "0")
                    esito = "Fallita";

                string dataUltimaVerifica = string.Empty;
                string dataProxVerifica = string.Empty;

                if (!string.IsNullOrEmpty(infoSupporto.dataUltimaVerificaLeggibilita))
                {
                    dataUltimaVerifica = infoSupporto.dataUltimaVerificaLeggibilita.Substring(0, 10);
                }
                if (!string.IsNullOrEmpty(infoSupporto.dataProxVerificaLeggibilita))
                {
                    dataProxVerifica = infoSupporto.dataProxVerificaLeggibilita.Substring(0, 10);
                }

                retValue = string.Format("Ver. il: {0}<BR/>Ver. num.: {1}<BR/>Esito: {2}<BR/>Perc.: {3}%<BR/>Prossima ver. il: {4}",
                                        dataUltimaVerifica,
                                        infoSupporto.numVerificheLeggibilita,
                                        esito,
                                        infoSupporto.percVerificheLeggibilita,
                                        dataProxVerifica
                                        );
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string ToShortDateString(string dateString)
        {
            return dateString;

            //if (!string.IsNullOrEmpty(dateString))
            //{
            //    DateTime date;
            //    DateTime.TryParse(dateString, out date);
            //    return date.ToString("dd/MM/yyyy");
            //}
            //else
            //    return string.Empty;
        }

        #endregion

        #region UI Management

        /// <summary>
        /// 
        /// </summary>
        protected virtual void RefreshControlsEnabled()
        {
            this.txtFilterIdIstanza.Enabled = string.IsNullOrEmpty(this.NormalizeFilterCriteria(Request.QueryString["idConservazione"]));

        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this._infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            this._tipiSupporto = ConservazioneManager.GetTipiSupporto();
            this._statiSupporto = ConservazioneManager.GetStatiSupporto();

            if (!Page.IsPostBack)
            {
                this.PrepareFilters();

                string idConservazione = this.NormalizeFilterCriteria(Request.QueryString["idConservazione"]);

                if (!string.IsNullOrEmpty(idConservazione))
                {
                    this.FetchSupporti();

                    this.txtFilterIdIstanza.Text = idConservazione;
                }
                else
                {
                    string id = this.NormalizeFilterCriteria(Request.QueryString["id"]);

                    if (!string.IsNullOrEmpty(id))
                    {
                        this.FetchSupporti();

                        this.txtFilterIdSupporto.Text = id;
                    }
                }

                this.GestioneGrafica();
            }
           
            //se l'istanza viene messa in lavorazione
            if (this.hd_istanza_da_rigenerare != null && this.hd_istanza_da_rigenerare.Value != String.Empty && this.hd_istanza_da_rigenerare.Value != "undefined")
            {
                WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                string idSupporto = this.hd_supporto_da_rigenerare.Value;
                string idIstanza = this.hd_ID_istanza_da_rigenerare.Value;
                string message = "";
                try
                {
                    if (!string.IsNullOrEmpty(idSupporto) && !string.IsNullOrEmpty(idIstanza))
                    {
                        message = ConservazioneManager.rigeneraIstanza(idIstanza, idSupporto, infoUtente);
                        this.hd_istanza_da_rigenerare.Value = null;
                        this.hd_supporto_da_rigenerare.Value = null;
                        this.hd_ID_istanza_da_rigenerare.Value = null;

                        if (string.IsNullOrEmpty(message))
                            //Response.Write("<script>alert('Operazione avvenuta con successo')</script>");
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_successo", "alert('Operazione avvenuta con successo.');", true);
                        else
                            //Response.Write("<script>alert('"+message+"')</script>");
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_rigenerazione_fallita", "alert('Si è verificato un errore.');", true);

                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_rigenerazione_fallita", "alert('Si è verificato un errore.');", true);

                }
                catch
                {
                     this.hd_istanza_da_rigenerare.Value = null;
                     this.hd_supporto_da_rigenerare.Value = null;
                     this.hd_ID_istanza_da_rigenerare.Value = null;
                }

                

            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            this.RefreshControlsEnabled();
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFind_Click(object sender, EventArgs e)
        {
            this.grdSupporti.CurrentPageIndex = 0;
            this.FetchSupporti();
            this.grdStoriaVerifiche.DataSource = null;
            this.grdStoriaVerifiche.DataBind();
            this.upDettaglio.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdSupporti_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            this.grdSupporti.SelectedIndex = e.Item.ItemIndex;

            if (e.CommandName == "DOWNLOAD")
            {

                // Modifica per la scrittura dell'evento Download su Registro Conservazione e sul Log

                WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                string idConservazione = this.NormalizeFilterCriteria(Request.QueryString["idConservazione"]);
                bool redirectflag = false;
                try
                {
                    Response.Redirect(e.CommandArgument.ToString(), redirectflag);

                    // MEV CS 1.5 - Alert Conservazione
                    // il download è avvenuto correttamente - verifico se l'alert è attivo
                    if (Utils.ConservazioneManager.IsAlertConservazioneAttivo(infoUtente.idAmministrazione, "DOWNLOAD"))
                    {
                        //task asincrono di incremento contatore ed eventuale invio alert
                        Utils.ConservazioneManager.InvioAlertAsync(infoUtente, "DOWNLOAD", string.Empty, string.Empty);
                    }
                    // end MEV CS 1.5

                    WSConservazioneLocale.RegistroCons regCons = new WSConservazioneLocale.RegistroCons();
                    regCons.idAmm = infoUtente.idAmministrazione;
                    regCons.idIstanza = idConservazione;
                    regCons.tipoOggetto = "I";
                    regCons.tipoAzione = "";
                    regCons.userId = infoUtente.userId;
                    regCons.codAzione = "DOWNLOAD_ISTANZA";
                    regCons.descAzione = "Download Istanza " + idConservazione;
                    regCons.esito = "1";
                    ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons, infoUtente);

                    // Inserisce nel DPA_LOG la Verifica Integrià dell'istanza
                    ConservazioneWA.Utils.ConservazioneManager.inserimentoInDpaLog(infoUtente, "DOWNLOAD_ISTANZA", idConservazione, "Download Istanza "+ idConservazione, WSConservazioneLocale.Esito.OK);
                }
                catch
                {
                    WSConservazioneLocale.RegistroCons regCons = new WSConservazioneLocale.RegistroCons();
                    regCons.idAmm = infoUtente.idAmministrazione;
                    regCons.idIstanza = idConservazione;
                    regCons.tipoOggetto = "I";
                    regCons.tipoAzione = "";
                    regCons.userId = infoUtente.userId;
                    regCons.codAzione = "DOWNLOAD_ISTANZA";
                    regCons.descAzione = "Download Istanza " + idConservazione;
                    regCons.esito = "0";
                    ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons, infoUtente);

                    // Inserisce nel DPA_LOG la Verifica Integrià dell'istanza
                    ConservazioneWA.Utils.ConservazioneManager.inserimentoInDpaLog(infoUtente, "DOWNLOAD_ISTANZA", idConservazione, "Download Istanza " + idConservazione, WSConservazioneLocale.Esito.KO);

                }



                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "download", "", true);
            }
            else if (e.CommandName == "BROWSE")
            {


                // Modifica per la scrittura dell'evento Browse su Registro Conservazione e sul Log

                WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                string idConservazione = this.NormalizeFilterCriteria(Request.QueryString["idConservazione"]);

                try
                {
                    // Azione di browse dell'istanza di conservazione
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "openWindow", "window.open('" + e.CommandArgument + "');", true);

                    //Response.Redirect(e.CommandArgument.ToString());

                    // MEV CS 1.5 - Alert Conservazione
                    // l'operazione è avvenuta correttamente - verifico se l'alert è attivo
                    if (Utils.ConservazioneManager.IsAlertConservazioneAttivo(infoUtente.idAmministrazione, "SFOGLIA"))
                    {
                        // task asincrono di incremento contatore ed eventuale invio alert
                        Utils.ConservazioneManager.InvioAlertAsync(infoUtente, "SFOGLIA", string.Empty, string.Empty);
                    }
                    // end MEV CS 1.5

                    WSConservazioneLocale.RegistroCons regCons = new WSConservazioneLocale.RegistroCons();
                    regCons.idAmm = infoUtente.idAmministrazione;
                    regCons.idIstanza = idConservazione;
                    regCons.tipoOggetto = "I";
                    regCons.tipoAzione = "";
                    regCons.userId = infoUtente.userId;
                    regCons.codAzione = "SFOGLIA_ISTANZA";
                    regCons.descAzione = "Visualizzazione contenuti istanza " + idConservazione;
                    regCons.esito = "1";
                    ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons, infoUtente);

                    // Inserisce nel DPA_LOG la Verifica Integrià dell'istanza
                    ConservazioneWA.Utils.ConservazioneManager.inserimentoInDpaLog(infoUtente, "SFOGLIA_ISTANZA", idConservazione, "Visualizzazione contenuti istanza ", WSConservazioneLocale.Esito.OK);
                }
                catch
                {
                    WSConservazioneLocale.RegistroCons regCons = new WSConservazioneLocale.RegistroCons();
                    regCons.idAmm = infoUtente.idAmministrazione;
                    regCons.idIstanza = idConservazione;
                    regCons.tipoOggetto = "I";
                    regCons.tipoAzione = "";
                    regCons.userId = infoUtente.userId;
                    regCons.codAzione = "SFOGLIA_ISTANZA";
                    regCons.descAzione = "Visualizzazione contenuti istanza " + idConservazione;
                    regCons.esito = "0";
                    ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons, infoUtente);

                    // Inserisce nel DPA_LOG la Verifica Integrià dell'istanza
                    ConservazioneWA.Utils.ConservazioneManager.inserimentoInDpaLog(infoUtente, "SFOGLIA_ISTANZA", idConservazione, "Visualizzazione contenuti istanza ", WSConservazioneLocale.Esito.KO);

                }




            }
            else if (e.CommandName == "REGISTRA_SUPPORTO")
            {
                // Azione successiva alla registrazione del supporto rimovibile

                this.FetchSupporti();
            }
            else if (e.CommandName == "VERIFICA_SUPPORTO")
            {
                // Azione successiva alla verifica del supporto

                //string esitoVerifica = this.hd_verifica.Value;

                this.FetchSupporti();

                Label lblIdIstanza = (Label)e.Item.FindControl("lblIdIstanza");
                Label lblIdSupporto = (Label)e.Item.FindControl("lblIdSupporto");

                this.FetchStoriaVerifiche(lblIdIstanza.Text, lblIdSupporto.Text);
            }
            else if (e.CommandName == "STORIA_VERIFICHE_SUPPORTO")
            {
                Label lblIdIstanza = (Label) e.Item.FindControl("lblIdIstanza");
                Label lblIdSupporto = (Label)e.Item.FindControl("lblIdSupporto");

                this.FetchStoriaVerifiche(lblIdIstanza.Text, lblIdSupporto.Text);
            }
            else if (e.CommandName == "GO_TO_ISTANZA")
            {
                Label lblIdIstanza = (Label)e.Item.FindControl("lblIdIstanza");

                //istanze += "<li><a href=\"RicercaIstanze.aspx?id=" + singoleIstanze[i] + "\" title=\"Istanza numero " + singoleIstanze[i] + "\">" + singoleIstanze[i] + "</a></li>";

                this.Response.Redirect(string.Format("~/RicercaIstanze.aspx?id={0}", lblIdIstanza.Text));
            }
            else if (e.CommandName == "VERIFICA_INTEGRITA_STORAGE")
            {
                this.FetchSupporti();
            }
            else if (e.CommandName == "VERIFICA_LEGGIBILITA")
            {
                this.FetchSupporti();
            }
            else if (e.CommandName == "VERIFICHE_IL")
            {
                this.FetchSupporti();
            }
            // Modifica Rigenerazione Istanza Danneggiata a.sigalot
            else if (e.CommandName == "RIGENERAZIONE_ISTANZA")
            {

                string idConservazione = this.NormalizeFilterCriteria(Request.QueryString["idConservazione"]);
              
                WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                Label lblIdSupporto = (Label)e.Item.FindControl("lblIdSupporto");

                if (string.IsNullOrEmpty(idConservazione))
                {
                    Label lblIdIstanza = (Label)e.Item.FindControl("lblIdIstanza");
                    idConservazione = lblIdIstanza.Text;
                }

                //ricerca x vedere se già ci sono istanze rigenerate DA FARE
                bool isIstanzaRig = ConservazioneManager.isIstanzaRigenerata(idConservazione, infoUtente);

                if (isIstanzaRig)
                {
                    // Istanza già rigenerata
                    this.hd_supporto_da_rigenerare.Value = lblIdSupporto.Text;
                    this.hd_ID_istanza_da_rigenerare.Value = idConservazione;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "notifyRigeneraIstanza", "notifyRigeneraIstanza('" + idConservazione + "','" + lblIdSupporto.Text + "');", true);
                }
                else
                {
                    string message = ConservazioneManager.rigeneraIstanza(idConservazione, lblIdSupporto.Text, infoUtente);
                    //if (string.IsNullOrEmpty(message))
                    //    Response.Write("<script>alert('Operazione avvenuta con successo')</script>");
                    //else
                    //    Response.Write("<script>alert('" + message + "')</script>");

                    if (string.IsNullOrEmpty(message))
                        //Response.Write("<script>alert('Operazione avvenuta con successo')</script>");
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_successo", "alert('Operazione avvenuta con successo.');", true);
                    else
                        //Response.Write("<script>alert('"+message+"')</script>");
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_rigenerazione_fallita", "alert('Si è verificato un errore.');", true);
                }
            }

            this.upDettaglio.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdSupporti_PreRender(object sender, EventArgs e)
        {
            bool grigioChiaro = false;
            foreach (DataGridItem itm in this.grdSupporti.Items)
            {
                Label lblIdIstanza = (Label)itm.FindControl("lblIdIstanza");
                Label lblIdSupporto = (Label)itm.FindControl("lblIdSupporto");

                Button btnRegistraSupporto = (Button)itm.FindControl("btnRegistraSupporto");
                if (btnRegistraSupporto != null)
                    btnRegistraSupporto.OnClientClick = string.Format("return showModalDialogRegistraSupportoRimovibile('{0}', '{1}');", lblIdIstanza.Text, lblIdSupporto.Text);


                Button btnVerificaSupporto = (Button)itm.FindControl("btnVerificaSupporto");
                if (btnVerificaSupporto != null)
                    btnVerificaSupporto.OnClientClick = string.Format("return showModalDialogVerificaSupportoRegistrato('{0}', '{1}');", lblIdIstanza.Text, lblIdSupporto.Text);

                Button btnVerificaLeggibilita = (Button)itm.FindControl("btn_verifica_leggibilita");
                if (btnVerificaLeggibilita != null)
                    btnVerificaLeggibilita.OnClientClick = string.Format("return showTestLeggibilita('{0}');", lblIdIstanza.Text);

                Button btnVerificaIntegritaStorage = (Button)itm.FindControl("btn_verifica_integrita_storage");
                if (btnVerificaIntegritaStorage != null)
                    btnVerificaIntegritaStorage.OnClientClick = string.Format("return showVerificaIntegrita('{0}','{1}');", lblIdIstanza.Text, lblIdSupporto.Text);

                Button btnVerificheIL = (Button)itm.FindControl("btn_verificheIL");
                if (btnVerificheIL != null)
                    btnVerificheIL.OnClientClick = string.Format("return showVerificheIL('{0}','{1}');", lblIdIstanza.Text, lblIdSupporto.Text);


                // Se il supporto è remoto, il backcolor è di uno stile diverso dagli altri
                Label lblCodiceTipoSupporto = (Label)itm.FindControl("lblCodiceTipoSupporto");
                if (lblCodiceTipoSupporto != null)
                {
                    if (lblCodiceTipoSupporto.Text == "REMOTO")
                    {
                        if (grigioChiaro)
                        {
                            itm.CssClass = "tab_istanze_d";
                            grigioChiaro = false;
                        }
                        else
                        {
                            itm.CssClass = "tab_istanze_c";
                            grigioChiaro = true;
                        }
                        
                    }
                    else
                    {
                        grigioChiaro = false;
                    }
                }
                else
                {
                    grigioChiaro = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdSupporti_PageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            this.grdSupporti.CurrentPageIndex = e.NewPageIndex;

            this.FetchSupporti();

            this.upDettaglio.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdStoriaVerifiche_PageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            if (this.grdSupporti.SelectedItem != null)
            {
                this.grdStoriaVerifiche.CurrentPageIndex = e.NewPageIndex;

                Label lblIdIstanza = (Label)this.grdSupporti.SelectedItem.FindControl("lblIdIstanza");
                Label lblIdSupporto = (Label)this.grdSupporti.SelectedItem.FindControl("lblIdSupporto");

                this.FetchStoriaVerifiche(lblIdIstanza.Text, lblIdSupporto.Text);

                this.upDettaglio.Update();
            }
        }

        /// <summary>
        /// Bug paginazione datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnDataGridItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }

            Button btnRegistraSupporto = (Button)e.Item.FindControl("btnRegistraSupporto");
            if (btnRegistraSupporto != null)
            {
                btnRegistraSupporto.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnRegistraSupporto.Attributes.Add("onmouseout", "this.className='cbtn';");
            }

            Button btnVerificaSupporto = (Button)e.Item.FindControl("btnVerificaSupporto");
            if (btnRegistraSupporto != null)
            {
                btnVerificaSupporto.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnVerificaSupporto.Attributes.Add("onmouseout", "this.className='cbtn';");
            }

            Button btnStoriaVerifiche = (Button)e.Item.FindControl("btnStoriaVerifiche");
            if (btnStoriaVerifiche != null)
            {
                btnStoriaVerifiche.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnStoriaVerifiche.Attributes.Add("onmouseout", "this.className='cbtn';");
            }
            Button btn_storia_verifiche_remoto = (Button)e.Item.FindControl("btn_storia_verifiche_remoto");
            if (btn_storia_verifiche_remoto != null)
            {
                btn_storia_verifiche_remoto.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_storia_verifiche_remoto.Attributes.Add("onmouseout", "this.className='cbtn';");
            }
            Button btnBrowse = (Button)e.Item.FindControl("btnBrowse");
            if (btnBrowse != null)
            {
                btnBrowse.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnBrowse.Attributes.Add("onmouseout", "this.className='cbtn';");
            }

            Button btnVerificaLeggibilita = (Button)e.Item.FindControl("btn_verifica_integrita_storage");
            if (btnVerificaLeggibilita != null)
            {
                btnVerificaLeggibilita.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnVerificaLeggibilita.Attributes.Add("onmouseout", "this.className='cbtn';");
            }

            Button btnVerificaIntegrita = (Button)e.Item.FindControl("btn_verifica_leggibilita");
            if (btnVerificaIntegrita != null)
            {
                btnVerificaIntegrita.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnVerificaIntegrita.Attributes.Add("onmouseout", "this.className='cbtn';");
            }

            Button btnVerificheIL = (Button)e.Item.FindControl("btn_verificheIL");
            if (btnVerificheIL != null)
            {
                btnVerificheIL.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnVerificheIL.Attributes.Add("onmouseout", "this.className='cbtn';");
            }

            Button btn_rigenerazione_istanza = (Button)e.Item.FindControl("btn_rigenerazione_istanza");
            if (btn_rigenerazione_istanza != null)
            {
                btn_rigenerazione_istanza.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_rigenerazione_istanza.Attributes.Add("onmouseout", "this.className='cbtn';");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRegistraSupporto_Click(object sender, EventArgs e)
        {
            this.FetchSupporti();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVerificaSupporto_Click(object sender, EventArgs e)
        {
            this.FetchSupporti();
        }

        
        
        protected void GestioneGrafica()
        {
            this.btnFind.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            this.btnFind.Attributes.Add("onmouseout", "this.className='cbtn';");
            WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            amm = Utils.ConservazioneManager.GetInfoAmmCorrente(infoUtente.idAmministrazione);
            this.lbl_amm.Text = this.amm.Codice + " - " + this.amm.Descrizione;
        }

        #endregion

        
    }
}
