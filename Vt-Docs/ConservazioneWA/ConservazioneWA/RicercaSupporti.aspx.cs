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
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.IO;
using CAPICOM;
using System.Globalization;

namespace ConservazioneWA
{
    public partial class RicercaSupporti : ConservazioneWA.CssPage
    {
        protected WSConservazioneLocale.InfoSupporto[] infoVerificaSupporto;
        
        protected WSConservazioneLocale.InfoUtente _infoUtente;

        private WSConservazioneLocale.TipoSupporto[] _tipiSupporto = null;

        private WSConservazioneLocale.StatoSupporto[] _statiSupporto = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            this._infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            this._tipiSupporto = ConservazioneManager.GetTipiSupporto();
            this._statiSupporto = ConservazioneManager.GetStatiSupporto();

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["idConservazione"] != null && Request.QueryString["idConservazione"].ToString() != string.Empty)
                {
                    string idConservazione=Request.QueryString["idConservazione"].ToString();
            
                    this.txt_idIstanza.Text=idConservazione;

                    this.FetchSupporti();
                    
                    this.hd_idIstanza.Value = idConservazione;
                }
                else
                {
                    this.cb_in_prod.Checked = true;
                    
                    this.FetchSupporti();
                }
            }
        }
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
        private void InitializeComponent()
        {
        }


        protected int UpdateSupporto(string collFisica, string dataProxVer, string note, string idSupporto, string numVer, string esitoUltimaVer, string percVerifica)
        {
            int newidSupporto = 0;
            int result = ConservazioneManager.setSupporto("", collFisica, "", "", esitoUltimaVer, numVer, dataProxVer, "", "", "", "", "", "V", note, "U", idSupporto, percVerifica, this._infoUtente, "",  out newidSupporto);
            return result;
        }


        protected void disabilitaCheckBox(string stato)
        {
            if (stato == "L")
            {
                //this.cb_prod.Checked = false;
                this.cb_ver.Checked = false;
                this.cb_eliminato.Checked = false;
            }
            if (stato == "P")
            {
                this.cb_in_prod.Checked = false;
                this.cb_ver.Checked = false;
                this.cb_eliminato.Checked = false;
            }
            if (stato == "V")
            {
                this.cb_in_prod.Checked = false;
                //this.cb_prod.Checked = false;
                this.cb_eliminato.Checked = false;
            }
            if (stato == "E")
            {
                this.cb_in_prod.Checked = false;
               // this.cb_prod.Checked = false;
                this.cb_ver.Checked = false;
            }
        }

        //protected void abilitaDate(string stato)
        //{
        //    if (stato == "L")
        //    {
        //        this.Panel_data_prod.Visible = true;
        //        this.Panel_data_prox_ver.Visible = true;
        //        this.Panel_data_ultima_ver.Visible = true;
        //    }
        //    if (stato == "P" || stato=="E" || stato=="V")
        //    {
        //        this.Panel_data_prod.Visible = true;
        //        this.Panel_data_prox_ver.Visible = true;
        //        this.Panel_data_ultima_ver.Visible = true;
        //        this.panel_data_scad_marca.Visible = true;
        //    }
        //}

        protected void btn_cerca_Click(object sender, EventArgs e)
        {
            this.Panel_verifica.Visible = false;

            this.FetchSupporti();
        }

        
        //protected void caricaDatiGridViewSupporti(WSConservazioneLocale.InfoSupporto[] infoSupp, GridView gv)
        //{
        //    ArrayList idIstanza = new ArrayList();
        //    this.dataSetSupporti = new ConservazioneWA.DataSet.DataSetRSupporti();
 
        //    for (int i = 0; i < infoSupp.Length; i++)
        //    {
        //        if (!idIstanza.Contains((string)infoSupp[i].idConservazione))
        //        {
        //            this.dataSetSupporti.dt_supporti.Adddt_supportiRow(infoSupp[i].TipoSupporto, infoSupp[i].Capacita, infoSupp[i].periodoVerifica, this.convertDate(infoSupp[i].dataProduzione),
        //               convertDate(infoSupp[i].dataUltimaVerifica), convertDate(infoSupp[i].dataEliminazione), infoSupp[i].numVerifiche, infoSupp[i].esitoVerifica, convertDate(infoSupp[i].dataProxVerifica), infoSupp[i].statoSupporto,
        //                infoSupp[i].numCopia, infoSupp[i].collocazioneFisica, infoSupp[i].Note, infoSupp[i].idConservazione, infoSupp[i].SystemID, convertDate(infoSupp[i].dataInsTSR), convertDate(infoSupp[i].dataScadenzaMarca), infoSupp[i].marcaTemporale, infoSupp[i].percVerifica + "%");
        //            idIstanza.Add((string)infoSupp[i].idConservazione);
        //        }
        //    }
        //    gv.DataSource = this.dataSetSupporti.Tables[0];
        //    gv.DataBind();
        //    Session["InfoSupp"] = infoSupp;
        //}



        //protected void caricaGridViewVerifiche(WSConservazioneLocale.InfoSupporto[] infoSupp, GridView gv)
        //{
        //    this.dataSetSupporti = new ConservazioneWA.DataSet.DataSetRSupporti();
        //    for (int i = 0; i < infoSupp.Length; i++)
        //    {
        //        this.dataSetSupporti.dt_supporti.Adddt_supportiRow("", "", "", "",
        //           convertDate(infoSupp[i].dataUltimaVerifica), "", infoSupp[i].numVerifiche, infoSupp[i].esitoVerifica, "", "",
        //            "", "", infoSupp[i].Note, infoSupp[i].idConservazione, infoSupp[i].SystemID, "", "", "", infoSupp[i].percVerifica + "%", infoSupp[i].progressivoMarca, infoSupp[i].idProfileTrasmissione, infoSupp[i].istanzaDownloadUrl,
        //            infoSupp[i].istanzaBrowseUrl);
        //    }
        //    gv.DataSource = this.dataSetSupporti.Tables[0];
        //    gv.DataBind();
        //    Session["InfoVerificheSupp"] = infoSupp;
        //}

        protected string Filtra()
        {
            string query = "";
            string[] queryStato = new string[6];
            
            string ordinamento = "";
            int dim = 0;
            if (this.cb_in_prod.Checked)
            {
                queryStato[dim] = "L";
                dim++;
                queryStato[dim] = "F";
                dim++;
            }
            //if (this.cb_prod.Checked)
            //{
            //    queryStato[dim] = "P";
            //    dim++;
            //}
            if (this.cb_ver.Checked)
            {
                queryStato[dim] = "V";
                dim++;
                //queryStato[dim] = "D";
                //dim++;
                //queryStato[dim] = "F";
                //dim++;
                ordinamento = " ORDER BY ID_CONSERVAZIONE DESC, COPIA ASC";
            }
            if (this.cb_danneggiato.Checked)
            {
                queryStato[dim] = "D";
                dim++;
            }

            if (this.cb_eliminato.Checked)
            {
                queryStato[dim] = "E";
                dim++;
            }

            if (ordinamento == string.Empty)
                ordinamento = " ORDER BY ID_CONSERVAZIONE DESC, COPIA ASC";

            string queryPar = "";

            if (this.dataProd_da.Text != "")
            {
                if (this.dataProd_a.Text != "")
                {
                    if (!string.IsNullOrEmpty(queryPar))
                        queryPar += " AND ";
                    queryPar += " (DATA_PRODUZIONE >= " + this.dbDateConverter(dataProd_da.Text, true) + " AND DATA_PRODUZIONE <= " + this.dbDateConverter(dataProd_a.Text, false) + ") ";//" AND (DATA_PRODUZIONE BETWEEN TO_DATE('" + dataProd_da.Text + "','DD/MM/YYYY') AND TO_DATE('" + dataProd_a.Text + "', 'DD/MM/YYYY'))";
                }
                else
                {
                    if (!string.IsNullOrEmpty(queryPar))
                        queryPar += " AND ";
                    queryPar += " DATA_PRODUZIONE >=" + this.dbDateConverter(dataProd_da.Text, true);// TO_DATE('" + dataProd_da.Text + "','DD/MM/YYYY')";
                }
            }
            else
            {
                if (dataProd_a.Text != "")
                {
                    if (!string.IsNullOrEmpty(queryPar))
                        queryPar += " AND ";
                    queryPar += " DATA_PRODUZIONE <= " + this.dbDateConverter(dataProd_a.Text, false);//TO_DATE('" + dataProd_a.Text + "','DD/MM/YYYY')";
                }
            }

            if (this.dataProxVer_da.Text != "")
            {
                if (this.dataProxVer_a.Text != "")
                {
                    if (!string.IsNullOrEmpty(queryPar))
                        queryPar += " AND ";
                    queryPar += " (DATA_PROX_VERIFICA >= " + this.dbDateConverter(dataProxVer_da.Text, true) + " AND DATA_PROX_VERIFICA <= " + this.dbDateConverter(dataProxVer_a.Text, false) + ") ";//" AND (DATA_PROX_VERIFICA BETWEEN TO_DATE('" + this.dataProxVer_da.Text + "','DD/MM/YYYY') AND TO_DATE('" + this.dataProxVer_a.Text + "', 'DD/MM/YYYY'))";
                }
                else
                {
                    if (!string.IsNullOrEmpty(queryPar))
                        queryPar += " AND ";
                    queryPar += " DATA_PROX_VERIFICA >=" + dbDateConverter(dataProxVer_da.Text, true);// TO_DATE('" + this.dataProxVer_da.Text + "','DD/MM/YYYY')";
                }
            }
            else
            {
                if (this.dataProxVer_a.Text != "")
                {
                    if (!string.IsNullOrEmpty(queryPar))
                        queryPar += " AND ";
                    queryPar += " DATA_PROX_VERIFICA <=" + dbDateConverter(dataProxVer_a.Text,false);// TO_DATE('" + this.dataProxVer_a.Text + "','DD/MM/YYYY')";
                }
            }

            if (this.dataUltimaVer_da.Text != "")
            {
                if (this.dataUltimaVer_a.Text != "")
                {
                    if (!string.IsNullOrEmpty(queryPar))
                        queryPar += " AND ";
                    queryPar += " (DATA_ULTIMA_VERIFICA >= " + this.dbDateConverter(dataUltimaVer_da.Text, true) + " AND DATA_ULTIMA_VERIFICA <= " + this.dbDateConverter(dataUltimaVer_a.Text, false) + ") ";//" AND (DATA_ULTIMA_VERIFICA BETWEEN TO_DATE('" + this.dataUltimaVer_da.Text + "','DD/MM/YYYY') AND TO_DATE('" + this.dataUltimaVer_a.Text + "', 'DD/MM/YYYY'))";
                }
                else
                {
                    if (!string.IsNullOrEmpty(queryPar))
                        queryPar += " AND ";
                    queryPar += " DATA_ULTIMA_VERIFICA >=" + dbDateConverter(dataUltimaVer_da.Text, true);// TO_DATE('" + this.dataUltimaVer_da.Text + "','DD/MM/YYYY')";
                }
            }
            else
            {
                if (this.dataUltimaVer_a.Text != "")
                {
                    if (!string.IsNullOrEmpty(queryPar))
                        queryPar += " AND ";
                    queryPar += " DATA_ULTIMA_VERIFICA <=" + dbDateConverter(dataUltimaVer_a.Text, false);// TO_DATE('" + this.dataUltimaVer_a.Text + "','DD/MM/YYYY')";
                }
            }

            if (this.dataScadMarca_da.Text != "")
            {
                if (dataScadMarca_a.Text != "")
                {
                    if (!string.IsNullOrEmpty(queryPar))
                        queryPar += " AND ";
                    queryPar += " (DATA_SCADENZA_MARCA >=" + this.dbDateConverter(dataScadMarca_da.Text, true) + " AND DATA_SCADENZA_MARCA <= " + this.dbDateConverter(dataScadMarca_a.Text, false) + ") ";//" AND (DATA_SCADENZA_MARCA BETWEEN TO_DATE('" + this.dataScadMarca_da.Text + "','DD/MM/YYYY') AND TO_DATE('" + this.dataScadMarca_a.Text + "', 'DD/MM/YYYY'))";
                }
                else
                {
                    if (!string.IsNullOrEmpty(queryPar))
                        queryPar += " AND ";
                    queryPar += " DATA_SCADENZA_MARCA >=" + dbDateConverter(dataScadMarca_da.Text, true);// TO_DATE('" + this.dataUltimaVer_da.Text + "','DD/MM/YYYY')";
                }
            }
            else
            {
                if (this.dataScadMarca_a.Text != "")
                {
                    if (!string.IsNullOrEmpty(queryPar))
                        queryPar += " AND ";
                    queryPar += " DATA_SCADENZA_MARCA <=" + dbDateConverter(dataScadMarca_a.Text, false);// TO_DATE('" + this.dataScadMarca_a.Text + "','DD/MM/YYYY')";
                }
            }

            if (this.txt_idIstanza.Text != "")
            {
                if (!string.IsNullOrEmpty(queryPar))
                    queryPar += " AND ";
                queryPar += " ID_CONSERVAZIONE='" + this.txt_idIstanza.Text + "'";

                this.hd_idIstanza.Value = this.txt_idIstanza.Text;
            }
            else
            {
                this.hd_idIstanza.Value = string.Empty;
            }

            if (this.txt_collFisica.Text != "")
            {
                if (!string.IsNullOrEmpty(queryPar))
                    queryPar += " AND ";
                queryPar = queryPar + " VAR_COLLOCAZIONE_FISICA='" + this.txt_collFisica.Text + "'";
            }

            if (this.txt_idSupp.Text != "")
            {
                if (!string.IsNullOrEmpty(queryPar))
                    queryPar += " AND ";
                queryPar = queryPar + " A.SYSTEM_ID='" + this.txt_idSupp.Text + "'";
            }

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
                query = query + queryPar + ordinamento;
            }
            else
            {
                if (queryPar != "")
                {
                    query = queryPar + ordinamento;
                }
                else
                {
                    query = ordinamento;
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
                    queryDate = " TO_DATE('" + data + " 00:00:00','DD/MM/YYYY HH24:mi:ss')";
                else
                    queryDate = " TO_DATE('" + data + " 23:59:59','DD/MM/YYYY HH24:mi:ss')";
            }
            else
            {
                if (dbType.ToLower().Equals("sql"))
                {
                    if (beginDay)
                        queryDate = " convert(datetime,'" + data + " 00:00:00', 103)";
                    else
                        queryDate = " convert(datetime,'" + data + " 23:59:59', 103)";
                }
            }
            return queryDate;
        }
        
        //protected void gv_supporti_SelectedIndexChanged(object sender, EventArgs e)
        //{  
        //    this.Panel_dettaglio.Visible = true;
        //    string periodoVerSupp = ((Label)this.gv_supporti.SelectedRow.FindControl("lb_periodoVer")).Text.ToString();
        //    this.hd_periodoVer.Value = periodoVerSupp;
        //    string idIstanza = ((Label)this.gv_supporti.SelectedRow.FindControl("lbl_idIstanza")).Text.ToString();
        //    this.hd_idIstanza.Value = idIstanza;
        //    infoSupp = ((WSConservazioneLocale.InfoSupporto[])Session["InfoSupp"]);
        //    this.caricaGridViewDettaglio(infoSupp, this.gv_dettaglio, idIstanza);
        //}

        protected void caricaGridViewDettaglio(WSConservazioneLocale.InfoSupporto[] infoSupp, string idIstanza)
        {
            this.grdSupporti.DataSource = infoSupp;
            this.grdSupporti.DataBind();

            


            ////WSConservazioneLocale.InfoSupporto[] newInfoSupp=new ConservazioneWA.WSConservazioneLocale.InfoSupporto[infoSupp.Length];
            ////int j = 0;
            ////this.dataSetSupporti = new ConservazioneWA.DataSet.DataSetRSupporti();
            ////for (int i = 0; i < infoSupp.Length; i++)
            ////{
            ////    if (infoSupp[i] != null)
            ////    {
            ////        //se sto facendo una ricerca aggregata per istanza
            ////        if (idIstanza != "")
            ////        {
            ////            if (infoSupp[i].idConservazione.Trim() == idIstanza.Trim())
            ////            {
            ////                this.dataSetSupporti.dt_supporti.Adddt_supportiRow(infoSupp[i].TipoSupporto, infoSupp[i].Capacita, infoSupp[i].periodoVerifica, this.convertDate(infoSupp[i].dataProduzione),
            ////                 convertDate(infoSupp[i].dataUltimaVerifica), convertDate(infoSupp[i].dataEliminazione), infoSupp[i].numVerifiche, infoSupp[i].esitoVerifica, convertDate(infoSupp[i].dataProxVerifica), infoSupp[i].statoSupporto,
            ////                   infoSupp[i].numCopia, infoSupp[i].collocazioneFisica, infoSupp[i].Note, infoSupp[i].idConservazione, infoSupp[i].SystemID, convertDate(infoSupp[i].dataInsTSR), convertDate(infoSupp[i].dataScadenzaMarca), infoSupp[i].marcaTemporale, infoSupp[i].percVerifica + "%", infoSupp[i].progressivoMarca, infoSupp[i].idProfileTrasmissione, infoSupp[i].istanzaDownloadUrl,
            ////                   infoSupp[i].istanzaBrowseUrl);
            ////                newInfoSupp[j] = (WSConservazioneLocale.InfoSupporto)infoSupp[i];
            ////                j++;
            ////            }
            ////        }
            ////        else
            ////        {
            ////            this.dataSetSupporti.dt_supporti.Adddt_supportiRow(infoSupp[i].TipoSupporto, infoSupp[i].Capacita, infoSupp[i].periodoVerifica, convertDate(infoSupp[i].dataProduzione),
            ////                 convertDate(infoSupp[i].dataUltimaVerifica), convertDate(infoSupp[i].dataEliminazione), infoSupp[i].numVerifiche, infoSupp[i].esitoVerifica, convertDate(infoSupp[i].dataProxVerifica), infoSupp[i].statoSupporto,
            ////                  infoSupp[i].numCopia, infoSupp[i].collocazioneFisica, infoSupp[i].Note, infoSupp[i].idConservazione, infoSupp[i].SystemID, convertDate(infoSupp[i].dataInsTSR), convertDate(infoSupp[i].dataScadenzaMarca), infoSupp[i].marcaTemporale, infoSupp[i].percVerifica + "%", infoSupp[i].progressivoMarca, infoSupp[i].idProfileTrasmissione, infoSupp[i].istanzaDownloadUrl,
            ////                  infoSupp[i].istanzaBrowseUrl);
            ////        }
            ////    }
            ////}
            ////gv.DataSource = this.dataSetSupporti.Tables[0];
            ////gv.DataBind();

            //if (idIstanza != "")
            //{
            //    Session["InfoSuppDett"] = newInfoSupp;
            //}
            //else
            //{
                Session["InfoSupp"] = infoSupp;
                Session["InfoSuppDett"] = infoSupp;
            //}
        }
       

        //protected void gv_supporti_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    this.gv_supporti.PageIndex = e.NewPageIndex;
        //    infoSupp = (WSConservazioneLocale.InfoSupporto[])Session["InfoSupp"];
        //    this.caricaDatiGridViewSupporti(infoSupp, this.gv_supporti);
        //    this.Panel_dettaglio.Visible = false;
        //}

        //protected void gv_dettaglio_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    this.gv_dettaglio.PageIndex = e.NewPageIndex;
        //    infoSupp = (WSConservazioneLocale.InfoSupporto[])Session["InfoSuppDett"];
        //    this.caricaGridViewDettaglio(infoSupp, this.gv_dettaglio, this.hd_idIstanza.Value);
        //}

        //protected void gv_dettaglio_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    string idSupporto = ((Label)this.gv_dettaglio.SelectedRow.FindControl("lb_idSupp")).Text.ToString();
        //    string idIstanza = ((LinkButton)this.gv_dettaglio.SelectedRow.FindControl("link_istanza")).Text;//((Label)this.gv_dettaglio.SelectedRow.FindControl("lb_idConservazione")).Text;
        //    this.hd_idSupporto.Value = idSupporto;
        //    if (this.hd_generaSupp.Value != null && this.hd_generaSupp.Value != String.Empty && this.hd_generaSupp.Value != "undefined")
        //    {
        //        string note = Session["noteSupporto"].ToString();
        //        string collFisica = Session["collFisicaSupp"].ToString();
        //        string dataProxVer = Session["dataProxVer"].ToString();
        //        int isChiusa = this.UpdateSupporto(collFisica, dataProxVer, note, hd_idSupporto.Value, "1", "1", "");
        //        //this.UpdateIstanza(collFisica, dataProxVer, idIstanza); 
        //        //TODO: ws creaTRASM NOTIFICA 
        //        this.hd_generaSupp.Value = null;
        //        Session["noteSupporto"] = "";
        //        Session["collFisicaSupp"] = "";
        //        Session["dataProxVer"] = "";
        //        if (isChiusa == 1)
        //        {
        //            bool result = ConservazioneManager.trasmettiNotifica(idIstanza, ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]));
        //            Response.Write("<script>alert(\"L\'istanza verrà chiusa\")</script>");
        //        }
        //        else
        //        {
        //            if (isChiusa == -1)
        //            {
        //                Response.Write("<script>alert(\"Errore di comunicazione con il server ripetere l\'operazione in seguito\")</script>");
        //            }
        //        }
        //    }
        //    string filtro = this.Filtra();
        //    infoSupp = ConservazioneManager.getInfoSupporto(filtro, infoUtente);
        //    // this.caricaDatiGridViewSupporti(infoSupp, this.gv_supporti);
        //    this.caricaGridViewDettaglio(infoSupp, this.gv_dettaglio, this.hd_idIstanza.Value);
        //   // this.txt_idSupp.Text = idSupporto;
        //}

        //protected void gv_dettaglio_PreRender(object sender, EventArgs e)
        //{
        //    for (int i = 0; i < this.gv_dettaglio.Rows.Count; i++)
        //    {
        //        string stato = ((Label)this.gv_dettaglio.Rows[i].FindControl("lb_statoSupp")).Text.Trim();
        //        string esitoVer = ((Label)this.gv_dettaglio.Rows[i].FindControl("lb_esitoUltimaVer")).Text;
        //        if (esitoVer == "0")
        //        {
        //            ((Label)this.gv_dettaglio.Rows[i].FindControl("lb_esitoUltimaVer")).Text = "Fallita";
        //        }
        //        else
        //        {
        //            if(esitoVer == "1")
        //             ((Label)this.gv_dettaglio.Rows[i].FindControl("lb_esitoUltimaVer")).Text = "Riuscita";
        //        }

        //        //supporto prodotto
        //        //if (stato == "P")
        //        //{
        //        //    ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[16].Controls[1]).Text = "Inserisci in area verifica";
        //        //    ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[16].Controls[1]).OnClientClick = "showModalDialogSuppProdotti();";
        //        //    ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[16].Controls[1]).CommandName = "SuppProdotto";
        //        //    ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[16].Controls[1]).Width = 146;
        //        //    ((Label)this.gv_dettaglio.Rows[i].FindControl("lb_statoSupp")).Text = "Prodotto";
        //        //}
        //        //supporto nell'area di verifica
        //        if (stato == "V" || stato == "P")
        //        {                    
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).Text = "Verifica Supporto";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).OnClientClick = "showModalDialogSuppInVerifica();";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).CommandName = "SuppInVerifica";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).Width = 146;
        //            ((Label)this.gv_dettaglio.Rows[i].FindControl("lb_statoSupp")).Text = "In Verifica";
        //            //((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[19].Controls[1]).Enabled = true;
        //            ((System.Web.UI.HtmlControls.HtmlAnchor)this.gv_dettaglio.Rows[i].Cells[19].Controls[1]).Disabled = false;
        //        }
        //        //supporto danneggiato da sostituire
        //        if (stato == "D")
        //        {
        //            ((Label)this.gv_dettaglio.Rows[i].FindControl("lb_statoSupp")).Text = "Danneggiato";
        //            ((Label)this.gv_dettaglio.Rows[i].FindControl("lb_statoSupp")).ForeColor = Color.Red;
        //            ((Label)this.gv_dettaglio.Rows[i].FindControl("lb_statoSupp")).Font.Bold = true;
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).Text = "Sostituisci supporto";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).OnClientClick = "showModalDialogSuppInVerifica();";//"return true";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).CommandName = "SuppDanneggiato";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).Width = 146;
        //            //((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[19].Controls[1]).Enabled = true; 
        //            ((System.Web.UI.HtmlControls.HtmlAnchor)this.gv_dettaglio.Rows[i].Cells[19].Controls[1]).Disabled = false;
        //        }
        //        //supporto eliminato
        //        if (stato == "E")
        //        {
        //            ((Label)this.gv_dettaglio.Rows[i].FindControl("lb_statoSupp")).Text = "Eliminato";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).Enabled = false;
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).Width = 146;
        //            //((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[19].Controls[1]).Enabled = false;
        //            ((System.Web.UI.HtmlControls.HtmlAnchor)this.gv_dettaglio.Rows[i].Cells[19].Controls[1]).Disabled = true;
        //        }
        //        //L = appena creati ma nn firmati M=istanza firmata ma manca la marca.
        //        if(stato=="L" || stato=="M")
        //        {
        //            ((Label)this.gv_dettaglio.Rows[i].FindControl("lb_statoSupp")).Text = "Nuovo";
        //            //((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).OnClientClick = "alertSupporto();";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).Enabled = false;
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).ToolTip = "Per generare il supporto è necessario prima procedere alla firma";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).Width = 146;
        //            //((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[19].Controls[1]).Enabled = false; 
        //            ((System.Web.UI.HtmlControls.HtmlAnchor)this.gv_dettaglio.Rows[i].Cells[19].Controls[1]).Disabled = true;
        //        }

        //        if (stato == "F")//i files di supporto sono già stati firmati
        //        {
        //            ((Label)this.gv_dettaglio.Rows[i].FindControl("lb_statoSupp")).Text = "In Lavorazione";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).Text = "Verifica Supporto";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).OnClientClick = "showModalDialogSuppInVerifica();";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).CommandName = "SuppInVerifica";
        //            ((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[18].Controls[1]).Width = 146;
        //            //((System.Web.UI.WebControls.Button)this.gv_dettaglio.Rows[i].Cells[19].Controls[1]).Enabled = true;
        //            ((System.Web.UI.HtmlControls.HtmlAnchor)this.gv_dettaglio.Rows[i].Cells[19].Controls[1]).Disabled = false;
        //        }
       
        //    }
        //}


        //protected void gv_dettaglio_RowCommand(object sender, GridViewCommandEventArgs e)
        //{                 
        //    string idSupporto = "";

        //    if (e.CommandName == "SuppInVerifica")
        //    {
        //        if (!string.IsNullOrEmpty(this.hd_verifica.Value) && this.hd_verifica.Value == "ok")
        //        {
        //            int index = Convert.ToInt32(e.CommandArgument.ToString());
                    
        //            this.effettuaVerifica(index, "verifica");

        //            idSupporto = infoSupp[index].SystemID;
                    
        //            string idIstanza = infoSupp[index].idConservazione;
                    
        //            this.txt_idIstanza.Text = idIstanza;
                    
        //            if (this.Panel_verifica.Visible == true)
        //            {
        //                infoVerificaSupporto = ConservazioneManager.getReportVerificheSupporto(idIstanza, idSupporto, this.infoUtente);

        //                this.caricaGridViewVerifiche(infoVerificaSupporto, this.gv_verifica);
        //            }
        //        }
        //    }
        //    else if (e.CommandName == "SuppDanneggiato")
        //    {
        //        infoSupp = (WSConservazioneLocale.InfoSupporto[])Session["infoSuppDett"];
                
        //        int index = Convert.ToInt32(e.CommandArgument.ToString());      

        //        this.effettuaVerifica(index, "sostituzione");
        //    }
        //    else if (e.CommandName == "linkIstanza")
        //    {
        //        infoSupp = (WSConservazioneLocale.InfoSupporto[])Session["infoSuppDett"];
                
        //        int index = Convert.ToInt32(e.CommandArgument.ToString());
                
        //        string idIstanza = infoSupp[index].idConservazione;
                
        //        Response.Redirect("Ricerca.aspx?idIstanza=" + idIstanza);
        //    }
        //    else if (e.CommandName == "dettaglio")
        //    {
        //        this.Panel_verifica.Visible = true;

        //        infoSupp = (WSConservazioneLocale.InfoSupporto[])Session["infoSuppDett"];
                
        //        int index = Convert.ToInt32(e.CommandArgument.ToString());
                
        //        string idIstanza = infoSupp[index].idConservazione;
                
        //        string Supporto = infoSupp[index].SystemID;
                
        //        infoVerificaSupporto = ConservazioneManager.getReportVerificheSupporto(idIstanza, Supporto, this.infoUtente);
                
        //        if(infoVerificaSupporto!=null)
        //            this.caricaGridViewVerifiche(infoVerificaSupporto, this.gv_verifica);
        //    }
        //    else if (e.CommandName == "BROWSE_SUPPORTO")
        //    {
        //        // Azione di browse dell'istanza di conservazione
        //        this.ClientScript.RegisterClientScriptBlock(this.GetType(), "openWindow", "window.open('" + e.CommandArgument + "')", true);
        //    }
        
        //    string filtro = this.Filtra();

        //    infoSupp = ConservazioneManager.getInfoSupporto(filtro, infoUtente);

        //    if (infoSupp != null)
        //    {
        //        if (this.hd_idIstanza.Value != null && this.hd_idIstanza.Value != string.Empty)
        //        {
        //            this.caricaGridViewDettaglio(infoSupp, this.gv_dettaglio, this.hd_idIstanza.Value);
        //        }
        //        else
        //        {
        //            this.caricaGridViewDettaglio(infoSupp, this.gv_dettaglio, "");
        //        }
        //    }
        //}

        protected void btn_pulisci_Click(object sender, EventArgs e)
        {
            this.PulisciCampi();   
        }

        protected void PulisciCampi()
        {
            dataProd_da.Text = "";
            dataProd_a.Text = "";
            dataProxVer_a.Text = "";
            dataProxVer_da.Text = "";
            dataScadMarca_a.Text = "";
            dataScadMarca_da.Text = "";
            dataUltimaVer_a.Text = "";
            dataUltimaVer_da.Text = "";
            this.txt_collFisica.Text = "";
            txt_idIstanza.Text = "";
            txt_idSupp.Text = "";
            //this.cb_ricAggregata.Checked = false;
            this.cb_eliminato.Checked = false;
            this.cb_in_prod.Checked = false;
            //this.cb_prod.Checked = false;
            this.cb_ver.Checked = false;
            this.cb_danneggiato.Checked = false;
        }

        //protected void cb_prod_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.cb_prod.Checked)
        //    {
        //        this.abilitaDate("L");
        //    }
        //    else
        //    {
        //        if (!this.cb_eliminato.Checked && !this.cb_ver.Checked)
        //        {
        //            this.Panel_data_prod.Visible = false;
        //            this.Panel_data_prox_ver.Visible = false;
        //            this.Panel_data_ultima_ver.Visible = false;
        //        }
        //    }
        //}

        //protected void cb_ver_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.cb_ver.Checked)
        //    {
        //        this.abilitaDate("L");
        //    }
        //    else
        //    {
        //        if (!this.cb_eliminato.Checked )//&& !this.cb_prod.Checked)
        //        {
        //            this.Panel_data_prod.Visible = false;
        //            this.Panel_data_prox_ver.Visible = false;
        //            this.Panel_data_ultima_ver.Visible = false;
        //        }
        //    }
        //}

        //protected void cb_eliminato_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.cb_eliminato.Checked)
        //    {
        //        this.abilitaDate("L");
        //    }
        //    else
        //    {
        //        if (!this.cb_ver.Checked)// && !this.cb_prod.Checked)
        //        {
        //            this.Panel_data_prod.Visible = false;
        //            this.Panel_data_prox_ver.Visible = false;
        //            this.Panel_data_ultima_ver.Visible = false;
        //        }
        //    }
        //}

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            bool result = UserManager.logOff(this._infoUtente, Page);
            this.Session.Abandon();
            string logOff = "<script>parent.close();</script>";
            Response.Write(logOff);
        }

        //protected void gv_supporti_PreRender(object sender, EventArgs e)
        //{
        //    for (int i = 0; i < this.gv_supporti.Rows.Count; i++)
        //    {
        //        string stato = ((Label)this.gv_supporti.Rows[i].FindControl("lb_statoSupp")).Text.Trim();
        //        string esitoVer = ((Label)this.gv_supporti.Rows[i].FindControl("lbl_esito_ultima_ver")).Text;

        //        if (esitoVer == "0")
        //        {
        //            ((Label)this.gv_supporti.Rows[i].FindControl("lbl_esito_ultima_ver")).Text = "Fallita";
        //        }
        //        else
        //        {
        //            ((Label)this.gv_supporti.Rows[i].FindControl("lbl_esito_ultima_ver")).Text = "Riuscita";
        //        }
        //        if (stato == "P")
        //            ((Label)this.gv_supporti.Rows[i].FindControl("lb_statoSupp")).Text = "Prodotto";
        //        if (stato == "V")
        //            ((Label)this.gv_supporti.Rows[i].FindControl("lb_statoSupp")).Text = "In Verifica";
        //        if (stato == "D")
        //            ((Label)this.gv_supporti.Rows[i].FindControl("lb_statoSupp")).Text = "Danneggiato";
        //        if (stato == "E")
        //            ((Label)this.gv_supporti.Rows[i].FindControl("lb_statoSupp")).Text = "Eliminato";
        //        if (stato == "L")
        //            ((Label)this.gv_supporti.Rows[i].FindControl("lb_statoSupp")).Text = "Nuovo";
        //        if (stato == "F")
        //            ((Label)this.gv_supporti.Rows[i].FindControl("lb_statoSupp")).Text = "In Lavorazione";

        //    }
        //}

        protected string convertDate(string oldDate)
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

        protected void EffettuaVerifica(int index)
        {


        }

        //protected void effettuaVerifica(int index, string operazione)
        //{
        //    if (this.hd_verifica != null && !string.IsNullOrEmpty(this.hd_verifica.Value) && this.hd_verifica.Value != "undefined")
        //    {
        //        //riprendo i valori inseriti nella popup                
        //        string url = (string)Session["url"];
        //        double percentuale = Convert.ToDouble((string)Session["percentuale"]);
        //        int percInt = Convert.ToInt32(percentuale);
        //        string note = (string)Session["note"];
        //        string collFisica = Session["collFisicaSupp"].ToString();
        //        string dataProxVer = Session["dataProxVer"].ToString();

        //        infoSupp = ((WSConservazioneLocale.InfoSupporto[])Session["InfoSuppDett"]);
        //        string idSupporto = infoSupp[index].SystemID;
        //        int numVer = 0;
        //        if (!string.IsNullOrEmpty(infoSupp[index].numVerifiche))
        //            numVer = Convert.ToInt32(infoSupp[index].numVerifiche) + 1;
        //        else
        //            numVer = 1;
        //        string idConservazione = infoSupp[index].idConservazione;
        //        this.hd_idIstanza.Value = idConservazione;
        //        string dataPrecVerifica = infoSupp[index].dataProxVerifica;
        //        this.hd_numMarca.Value = infoSupp[index].progressivoMarca;
        //        this.hd_ProfileTrasm.Value = infoSupp[index].idProfileTrasmissione;

        //        //verifico che l'url sia quello di una istanza di conservazione
        //        string urlXml = string.Empty;
        //        string urlP7m = string.Empty;
        //        string urlTsr = string.Empty;

        //        string isFolderCons = this.isIstanzaConservazione(url, idConservazione, ref urlXml, ref urlP7m, ref urlTsr);
                
        //        string rootPath = Path.GetDirectoryName(url);
        //        //string rootDocumenti = Path.Combine(rootPath, "documenti");

        //        switch(operazione)
        //        {
        //            case "verifica":                        
        //                if (isFolderCons==string.Empty)
        //                {

        //                        //verifico la firma del file p7m
        //                        int verificaFirma = this.verificaFirma(urlP7m, ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]),idConservazione );
        //                        string resultFirma = string.Empty;
                            
        //                        bool verifica = this.decodificaStato(verificaFirma, ref resultFirma);
        //                        if (verifica)
        //                        {
        //                            //verifico la marca temporale
        //                            string verificaMarca = this.verificaMarca(urlTsr, ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]),idConservazione);
        //                            //se nn ci sono errori nella lettura della marca
        //                            if (verificaMarca == string.Empty)
        //                            {
        //                                int isChiusa = this.UpdateSupporto(collFisica, dataProxVer, note, idSupporto, Convert.ToString(numVer), "1", Convert.ToString(percInt));
        //                                bool resulInsertVer = ConservazioneManager.insertRisultatoVerifica(this.infoUtente, idSupporto, idConservazione, note, Convert.ToString(percInt), Convert.ToString(numVer), "1");
                                          
        //                                //se l'istanza viene chiusa ed è la prima verifica allora trasmetto
        //                                if (isChiusa == 1 && string.IsNullOrEmpty(dataPrecVerifica))
        //                                {
        //                                    bool result = ConservazioneManager.trasmettiNotifica(idConservazione, ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]));
        //                                }
        //                                if (resultFirma != "Valido")
        //                                {
        //                                    //per il momento nn utilizziamo questo metodo perché nn sappiamo quando scade la marca
        //                                    // msgRigeneraMarca.Confirm("La verifica è andata a buon fine, si vuole procedere con la generazione di una nuova marca temporale?");
        //                                    Response.Write("<script>alert('" + resultFirma + "')</script>");
        //                                }
        //                                else
        //                                {
        //                                    Response.Write("<script>alert(\"La verifica è stata effettuata con successo.\")</script>");
        //                                }

        //                                //se l'istanza è chiusa richiamiamo il rigenera marca in modo che la trasmissione di chiusura venga fatta sempre con un solo tsr.
        //                                //if (isChiusa == 1 && !string.IsNullOrEmpty(dataPrecVerifica))                                              
        //                                if (isChiusa == 1)
        //                                {
        //                                    //recupero l'idProfile della trasmissione appena effettuata
        //                                    string filtro = this.Filtra();

        //                                    infoSupp = ConservazioneManager.getInfoSupporto(filtro, infoUtente);                                           
                                            
        //                                    this.caricaGridViewDettaglio(infoSupp, this.hd_idIstanza.Value);
                                            
        //                                    this.hd_ProfileTrasm.Value = infoSupp[index].idProfileTrasmissione;
                                            
        //                                    msgRigeneraMarca.Confirm("La verifica è andata a buon fine, si vuole procedere con la generazione di una nuova marca temporale?");
        //                                }
        //                            }
        //                            else
        //                            {
        //                                string messaggio = string.Empty;
        //                                if (percentualeVerifica != Convert.ToDouble(-1))
        //                                {
        //                                    string perc = Convert.ToString(percentualeVerifica);
        //                                    string percAppo = "";
        //                                    if (perc.Contains(","))
        //                                    {
        //                                        percAppo = perc.Substring(0, perc.IndexOf(",") + 2);
        //                                    }
        //                                    else
        //                                    {
        //                                        percAppo = perc;
        //                                    }
        //                                    messaggio = "La verifica è riuscita solo per il " + percAppo + "% dei documenti. Il supporto risulta danneggiato";
        //                                    this.setSupportoDanneggiato(numVer, collFisica, dataProxVer, note, idSupporto, idConservazione, Convert.ToInt32(percInt), messaggio);
        //                                    bool resulInsertVer = ConservazioneManager.insertRisultatoVerifica(this.infoUtente, idSupporto, idConservazione, note, Convert.ToString(percInt), Convert.ToString(numVer), "0");
        //                                    Response.Write("<script>alert('" + messaggio + "')</script>");
        //                                }
        //                                else
        //                                {
        //                                    Response.Write("<script>alert(\"Errore di comunicazione con il server, riprovare in seguito.\")</script>");
        //                                }
                                            
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //if (verificaMarca != "KO")
        //                            //{
        //                            //    this.setSupportoDanneggiato(numVer, collFisica, dataProxVer, note, idSupporto, idConservazione, Convert.ToInt32(percInt), verificaMarca);
        //                            //    bool resulInsertVer = ConservazioneManager.insertRisultatoVerifica(this.infoUtente, idSupporto, idConservazione, note, Convert.ToString(percInt), Convert.ToString(numVer), "0");
        //                            //    Response.Write("<script>alert('La verifica della marca non è andata a buon fine. Il supporto risulta danneggiato.')</script>");
        //                            //}
        //                            //else
        //                            //{
        //                            //    Response.Write("<script>alert(\"Errore di comunicazione con il server per la verifica della marca, riprovare in seguito.\")</script>");
        //                            //}
        //                        }
        //                }
        //                else
        //                {
        //                    if (isFolderCons.Equals("Istanza di conservazione non completa"))
        //                    {
        //                        this.setSupportoDanneggiato(numVer, collFisica, dataProxVer, note, idSupporto, idConservazione, Convert.ToInt32(percInt), "Istanza di conservazione non completa. Supporto danneggiato.");
        //                        bool resulInsertVer = ConservazioneManager.insertRisultatoVerifica(this.infoUtente, idSupporto, idConservazione, note, Convert.ToString(percInt), Convert.ToString(numVer), "0");
        //                    }
        //                    else
        //                    {
        //                        Response.Write("<script>alert(\"" + isFolderCons + "\")</script>");
        //                    }
        //                }
        //            break;
        //                case "sostituzione":
        //                    infoSupp = (WSConservazioneLocale.InfoSupporto[])Session["infoSuppDett"];
        //                    //dati supporto eliminato
        //                    string numCopia = infoSupp[index].numCopia;
        //                    string dataAppoMarca = infoSupp[index].dataInsTSR;
        //                    string dataScadMarca = infoSupp[index].dataScadenzaMarca;
        //                    string marca = infoSupp[index].marcaTemporale;
        //                    string idTipoSupp = infoSupp[index].idTipoSupporto;
        //                    int newIdSupporto = 0;
        //                    string filtroSuppUpdate = " SET CHA_STATO='E', DATA_ELIMINAZIONE=SYSDATE" + " WHERE SYSTEM_ID='" + idSupporto + "'";
        //                    ConservazioneManager.UpdateSupporto(filtroSuppUpdate, infoUtente);

        //                    System.DateTime now = System.DateTime.Now;
        //                    CultureInfo ci = new CultureInfo("en-US");
                                
        //                    //effettuo verifica                           

        //                    if (isFolderCons == string.Empty)
        //                    {
        //                        //verifico la firma del file p7m
        //                        int verificaFirma = this.verificaFirma(urlP7m, ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]),idConservazione );
        //                        string resultFirma = string.Empty;
        //                        bool verifica = this.decodificaStato(verificaFirma, ref resultFirma);
        //                        if (verifica)
        //                        {
        //                            //verifico la marca temporale
        //                            string verificaMarca = this.verificaMarca(urlTsr, ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]), idConservazione);
        //                            //se nn ci sono errori nella lettura della marca
        //                            if (verificaMarca == string.Empty)
        //                            {
        //                                //verifico l'impronta di una percentuale di documenti indicata, che sia uguale a quella originale
        //                                bool esistoVerifica = this.verificaXml(urlXml, percentuale, rootPath);
        //                                if (esistoVerifica)
        //                                {
        //                                    int isChiusa = ConservazioneManager.setSupporto(numCopia, collFisica, now.ToString("dd/MM/yyyy", ci), "", "1", "1", dataProxVer, dataAppoMarca, dataScadMarca, marca, infoSupp[index].idConservazione, idTipoSupp, "V", note, "I", "", Convert.ToString(percInt), infoUtente, this.hd_numMarca.Value, out newIdSupporto);                                            
        //                                    bool resulInsertVer = ConservazioneManager.insertRisultatoVerifica(this.infoUtente, Convert.ToString(newIdSupporto), idConservazione, note, Convert.ToString(percInt), "1", "1");
                                           
        //                                    if (resultFirma != "Valido")
        //                                    {
        //                                        //per il momento nn utilizziamo questo metodo perché nn sappiamo quando scade la marca
        //                                        // msgRigeneraMarca.Confirm("La verifica è andata a buon fine, si vuole procedere con la generazione di una nuova marca temporale?");
        //                                        Response.Write("<script>alert('" + resultFirma + "')</script>");
        //                                    }
        //                                    else
        //                                    {
        //                                        Response.Write("<script>alert(\"La verifica è stata effettuata con successo.\")</script>");
        //                                    }

        //                                }
        //                                else
        //                                {
        //                                    string messaggio = string.Empty;
        //                                    if (percentualeVerifica != Convert.ToDouble(-1))
        //                                    {
        //                                        string perc = Convert.ToString(percentualeVerifica);
        //                                        string percAppo = "";
        //                                        if (perc.Contains(","))
        //                                        {
        //                                            percAppo = perc.Substring(0, perc.IndexOf(",") + 2);
        //                                        }
        //                                        else
        //                                        {
        //                                            percAppo = perc;
        //                                        }
        //                                        messaggio = "La verifica è riuscita solo per il " + percAppo + "% dei documenti. Il supporto risulta danneggiato";
        //                                        int isChiusa = ConservazioneManager.setSupporto(numCopia, collFisica, now.ToString("dd/MM/yyyy", ci), "", "0", "1", dataProxVer, dataAppoMarca, dataScadMarca, marca, infoSupp[index].idConservazione, idTipoSupp, "D", note, "I", "", Convert.ToString(percInt), infoUtente, this.hd_numMarca.Value, out newIdSupporto);                  
        //                                       // this.setSupportoDanneggiato(1, collFisica, dataProxVer, note, Convert.ToString(newIdSupporto), idConservazione, Convert.ToInt32(percInt), messaggio);
        //                                        bool resulInsertVer = ConservazioneManager.insertRisultatoVerifica(this.infoUtente, Convert.ToString(newIdSupporto), idConservazione, note, Convert.ToString(percInt), "1", "0");
        //                                        Response.Write("<script>alert('" + messaggio + "')</script>");
        //                                    }
        //                                    else
        //                                    {
        //                                        Response.Write("<script>alert(\"Errore di comunicazione con il server, riprovare in seguito.\")</script>");
        //                                    }

        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (verificaMarca != "KO")
        //                                {
        //                                    int isChiusa = ConservazioneManager.setSupporto(numCopia, collFisica, now.ToString("dd/MM/yyyy", ci), "", "0", "1", dataProxVer, dataAppoMarca, dataScadMarca, marca, infoSupp[index].idConservazione, idTipoSupp, "D", note, "I", "", Convert.ToString(percInt), infoUtente, this.hd_numMarca.Value, out newIdSupporto);                                            
        //                                    //this.setSupportoDanneggiato(1, collFisica, dataProxVer, note, Convert.ToString(newIdSupporto), idConservazione, Convert.ToInt32(percInt), verificaMarca);
        //                                    bool resulInsertVer = ConservazioneManager.insertRisultatoVerifica(this.infoUtente, Convert.ToString(newIdSupporto), idConservazione, note, Convert.ToString(percInt), "1", "0");
        //                                    Response.Write("<script>alert('La verifica della marca non è andata a buon fine. Il supporto risulta danneggiato.')</script>");
        //                                }
        //                                else
        //                                {
        //                                    Response.Write("<script>alert(\"Errore di comunicazione con il server per la verifica della marca, riprovare in seguito.\")</script>");
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            int isChiusa = ConservazioneManager.setSupporto(numCopia, collFisica, now.ToString("dd/MM/yyyy", ci), "", "0", "1", dataProxVer, dataAppoMarca, dataScadMarca, marca, infoSupp[index].idConservazione, idTipoSupp, "D", note, "I", "", Convert.ToString(percInt), infoUtente, this.hd_numMarca.Value,  out newIdSupporto);                                            
        //                           // this.setSupportoDanneggiato(1, collFisica, dataProxVer, note, Convert.ToString(newIdSupporto), idConservazione, Convert.ToInt32(percentuale), resultFirma);
        //                            bool resulInsertVer = ConservazioneManager.insertRisultatoVerifica(this.infoUtente, Convert.ToString(newIdSupporto), idConservazione, note, Convert.ToString(percInt), "1", "0");
        //                            Response.Write("<script>alert('La verifica della firma non è andata a buon fine. Il supporto risulta danneggiato.')</script>");
        //                        }
        //                    }
        //                    else
        //                    {

        //                        if (isFolderCons.Equals("Istanza di conservazione non completa"))
        //                        {
        //                            int isChiusa = ConservazioneManager.setSupporto(numCopia, collFisica, now.ToString("dd/MM/yyyy", ci), "", "0", "1", dataProxVer, dataAppoMarca, dataScadMarca, marca, infoSupp[index].idConservazione, idTipoSupp, "D", note, "I", "", Convert.ToString(percInt), infoUtente, this.hd_numMarca.Value,  out newIdSupporto);
        //                            bool resulInsertVer = ConservazioneManager.insertRisultatoVerifica(this.infoUtente, Convert.ToString(newIdSupporto), idConservazione, note, Convert.ToString(percInt), "1", "0");
        //                            Response.Write("<script>alert(\"" + isFolderCons + ". Supporto danneggiato." + "\")</script>");
        //                        }
        //                        else
        //                        {
        //                            Response.Write("<script>alert(\"" + isFolderCons + "\")</script>");
        //                        }
        //                    }
        //                    //if (this.Panel_verifica.Visible == true)
        //                    //{
        //                    //    infoVerificaSupporto = ConservazioneManager.getReportVerificheSupporto(infoSupp[index].idConservazione, Convert.ToString(newIdSupporto), this.infoUtente);
        //                    //    this.caricaGridViewVerifiche(infoVerificaSupporto, this.gv_verifica);
        //                    //}
        //                    break;
        //        }    
        //    }
        //}

        //private bool decodificaStato(int stato,  ref string  result)
        //{
        //    bool proseguiVerifica = true;
        //    switch (stato)
        //    {
        //        case -1:
        //            result = "Non è stato possibile verificare la firma";                //"STATUS_REVOCATION_CHECK_ERROR";
        //            break;
        //        case 0:
        //            result = "Valido";
        //            break;
        //        case (int)CAPICOMHelper.CAPICOM_CHAIN_STATUS.CAPICOM_TRUST_IS_NOT_TIME_VALID:
        //            result = "Il certificato è scaduto o non ancora valido";
        //            break;
        //        case (int)CAPICOMHelper.CAPICOM_CHAIN_STATUS.CAPICOM_TRUST_IS_NOT_SIGNATURE_VALID:
        //            result = "La signature del certificato non è valida";
        //            proseguiVerifica = false;
        //            break;
        //        case (int)CAPICOMHelper.CAPICOM_CHAIN_STATUS.CAPICOM_TRUST_IS_UNTRUSTED_ROOT:
        //            result = "Il certificato della root authority che ha emesso il certificato in esame non è presente nel SYSTEM store della macchina che esegue il servizio di verifica";
        //            break;
        //        case (int)CAPICOMHelper.CAPICOM_CHAIN_STATUS.CAPICOM_TRUST_REVOCATION_STATUS_UNKNOWN:
        //            result = "Non è stato possible verificare lo stato di revoca del certificato (nella maggior parte dei casi questo si verifica quando il sistema non riesce a contattare via internet il CDP per scaricare una CRL valida)";
        //            break;
        //        case (int)CAPICOMHelper.CAPICOM_CHAIN_STATUS.CAPICOM_TRUST_IS_REVOKED:
        //            result = "Il certificato è stato revocato in quanto presente nella CRL";
        //            break;
        //        default:
        //            result = "Non è stato possibile verificare la firma";//"STATUS_DESCRIPTION_UNKNOWN";                    
        //            break;
        //    }
        //    return proseguiVerifica;
        ////}

        //private void setSupportoDanneggiato(int numVer, string collFisica, string dataProxVer, string note, string idSupporto, string idConservazione, int percVerifica, string messaggio)
        //{
        //    string filtroSupp = " SET CHA_STATO='D', VAR_COLLOCAZIONE_FISICA='" + collFisica + "', DATA_ULTIMA_VERIFICA=SYSDATE, ESITO_ULTIMA_VERIFICA='0', VERIFICHE_EFFETTUATE='" + numVer + "', DATA_PROX_VERIFICA=TO_DATE('" + dataProxVer + "','DD/MM/YYYY'), VAR_NOTE='" + note + "', PERC_VERIFICA='" + percVerifica + "'" + "  WHERE SYSTEM_ID='" + idSupporto + "'";
        //    string filtroIstanza = " SET CHA_STATO='F' WHERE SYSTEM_ID='" + idConservazione + "'";
        //    bool result = ConservazioneManager.UpdateSupporto(filtroSupp, infoUtente);
        //    if (result)
        //    {
        //        bool result2 = ConservazioneManager.updateInfoConservazione(filtroIstanza, infoUtente);
        //        Response.Write("<script>alert('" + messaggio + "')</script>");
        //    }
        //    else
        //    {
        //        Response.Write("<script>alert(\"Errore di comunicazione con il server ripetere l\'operazione in seguito\")</script>");
        //    }
        //}

        //private string isIstanzaConservazione(string url, string idIstanza, ref string urlXml, ref string urlP7m, ref string urlTsr)
        //{
        //    string result = string.Empty;

        //    string nameFolder = idIstanza; 
        //    string pathName = Path.GetFileName(url);
        //    string rootPath = Path.GetDirectoryName(url);
           
        //    int numeroMarche = 0;

        //    if (!string.IsNullOrEmpty(this.hd_numMarca.Value))
        //    {
        //        try
        //        {
        //            numeroMarche = Convert.ToInt32(this.hd_numMarca.Value);
        //        }
        //        catch (Exception e)
        //        {
                    
        //        }
        //    }     

        //    if (pathName.Trim().ToLowerInvariant() == "chiusura")
        //    {
        //        urlXml = Path.Combine(url, "file_chiusura.xml");
        //        urlP7m = Path.Combine(url, "file_chiusura.p7m");
        //        urlTsr = Path.Combine(url, "file_chiusura.tsr");
        //        string html = Path.Combine(rootPath, "html");
        //        string indexHtml = Path.Combine(rootPath, "index.html");

        //        if (!File.Exists(urlXml) || 
        //            !File.Exists(urlP7m) || 
        //            !File.Exists(urlTsr) || 
        //            !Directory.Exists(html) || 
        //            !File.Exists(indexHtml))
        //        {
        //            result = "Istanza di conservazione non completa";
        //        }
        //        if (numeroMarche > 0)
        //        {
        //            for (int i = 1; i <= numeroMarche; i++)
        //            {
        //                string add = "_" + i.ToString();
        //                string pathTsr = Path.Combine(url, nameFolder + add + ".tsr");
        //                if (!File.Exists(pathTsr))
        //                {
        //                    result = result = "Istanza di conservazione non completa";
        //                    return result;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        result = "Selezionare la cartella chiusura";
        //    }
           
        //    return result;
        //}

        //private bool verificaXml(string url, double percentuale, string rootPath)
        //{
        //    bool result = false;

        //    XmlDocument xmlFile = new XmlDocument();

        //    // per la conservazione l'url del file xml da leggere deve essere quello in cui ha fatto download dell'istanza
        //    try
        //    {
        //        XmlTextReader reader = new XmlTextReader(url);
        //        reader.WhitespaceHandling = WhitespaceHandling.None;
        //        xmlFile.Load(reader);

        //        XmlNodeList listaNodiDocNumber = xmlFile.GetElementsByTagName("sincro:File");

        //        //numero totali di documenti che fanno parte dell'istanza
        //        int numeroDocumenti = listaNodiDocNumber.Count;

        //        //numero di documenti di cui voglio verificare l'impronta
        //        int n = 0;
        //        if (percentuale < 100)
        //        {
        //            n = ((int)percentuale * numeroDocumenti / 100);
        //            if (n == 0)
        //                n = n + 1;
        //        }
        //        else
        //        {
        //            n = numeroDocumenti;
        //        }

        //        //genero una sequenza di numeri casuali senza ripetizione compresi tra 0 e numeroDocumenti-1
        //        int[] numeriCasuali = this.eseguiEstrazioneRandom(numeroDocumenti);

        //        //verifico l'impronta di n documenti tra quelli presenti nel supporto e calcolo la percentuale di quelli che nn sono stati corrotti

        //        int contatore = 0;
               
        //        for (int i = 0; i < n; i++)
        //        {
        //            //prendo l'impronta del nodo con posizione tra quelli generati in maniera casuale
        //            int indiceNodo = numeriCasuali[i];
        //            XmlNode nodoDoc = ((XmlNode)listaNodiDocNumber[indiceNodo]);
        //            XmlNodeList childList = (XmlNodeList)nodoDoc.ChildNodes;

        //            //verifico che l'impronta del documento e dei suoi allegati corrisponda a quella originale
        //            bool verifyImpronta = this.verificaImprontaFile(childList, rootPath);    
                
        //            if (verifyImpronta)
        //            {
        //                contatore = contatore + 1;
        //            }
        //        }
        //        if (contatore == n)
        //        {
        //            result = true;
        //        }
        //        else
        //        {
        //            percentualeVerifica = ((double)contatore * 100 / numeroDocumenti);
        //            result = false;
        //        }
        //        reader.Close();
        //        reader = null;
        //    }
        //    catch(Exception e)
        //    {
        //        Debugger.Write("Errore nella pagina RicercaSupporti (metodo verificaXml):" + e.Message);
        //        result = false;
        //    }    
        //    return result;
        //}

        //public bool verificaImprontaFile(XmlNodeList childList, string rootPath)
        //{
        //    bool result = false;

        //    XmlNode idNode = (XmlNode)childList[0];
        //    XmlNode pathNode = (XmlNode)childList[1];
        //    XmlNode hashNode = (XmlNode)childList[2];

        //    string docNumber = idNode.InnerText;
        //    string pathFile = pathNode.InnerText.Substring(1);
        //    string impronta = hashNode.InnerText;
                
        //    string path = Path.Combine(rootPath, pathFile);

        //    string improntaOriginale = this.calcolaImpronta(this.infoUtente, path);
                
        //    if (impronta == improntaOriginale)
        //    {
        //        result = true;
        //    }

        //    return result;
        //}

        //private int[] eseguiEstrazioneRandom(int numeroElementi)
        //{
        //    // inizializzazione dell'array di ritorno
        //    int[] iValues = new int[numeroElementi];

        //    SortedList sList = new SortedList();
        //    // popolo la SortedList con i valori

        //    for (int k = 0; k < numeroElementi; k++)

        //        sList.Add(k, k);

        //    // inizializzo il generatore di numeri random

        //    System.Random rnd = new System.Random(unchecked((int)DateTime.Now.Ticks));

        //    // estrazione 

        //    for (int k = 0; k < numeroElementi; k++)
        //    {

        //        // ad ogni ciclo il count della sortedlist diminuisce di uno

        //        int x = rnd.Next(0, sList.Count);

        //        // prendiamo il numero che troviamo alla posizione relativa

        //        iValues[k] = (int)sList.GetByIndex(x);

        //        // rimozione della posizione già utilizzata
        //        sList.RemoveAt(x);

        //    }
        //    return iValues;
        //}

        //private string calcolaImpronta(WSConservazioneLocale.InfoUtente infoUtente, string pathFile)
        //{                        
        //    string impronta = ConservazioneManager.getImpronta(infoUtente, pathFile);
        //    return impronta;
        //}

        //private int verificaFirma(string url, WSConservazioneLocale.InfoUtente infoUtente,string IDCons)        
        //{
        //    int result = 0;
        //    result = ConservazioneManager.verificaFileFirmato(url ,((WSConservazioneLocale.InfoUtente)Session["infoutCons"]),IDCons  );
        //    return result;
        //}
        //private string verificaMarca(string url, WSConservazioneLocale.InfoUtente infoUtente, string IDCons)
        //{
        //    string result = "";
        //    result = ConservazioneManager.verificaMarca(url,((WSConservazioneLocale.InfoUtente)Session["infoutCons"]),IDCons );
        //    return result;
        //}

        //private void msgRigeneraMarca_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        //{
        //    if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
        //    {
        //        string numeroVerifica = string.Empty;
        //        if (!string.IsNullOrEmpty(this.hd_numMarca.Value))
        //        {
        //            int numVer = Convert.ToInt32(this.hd_numMarca.Value) + 1;
        //            numeroVerifica = Convert.ToString(numVer);
        //        }
        //        else
        //        {
        //            numeroVerifica = "1";
        //        }
        //        bool result = ConservazioneManager.apponiMarca(this.hd_idIstanza.Value, infoUtente, numeroVerifica, this.hd_ProfileTrasm.Value);
        //    }
        //}

        //protected void gv_verifica_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    this.gv_verifica.PageIndex = e.NewPageIndex;
        //    infoVerificaSupporto = (WSConservazioneLocale.InfoSupporto[])Session["InfoVerificheSupp"];
        //    this.caricaGridViewVerifiche(infoVerificaSupporto, this.gv_verifica);
        //    this.Panel_verifica.Visible = true;
        //}


        #region DataGridSupporti functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdSupporti_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "DOWNLOAD")
            {
            }
            else if (e.CommandName == "BROWSE")
            {
                // Azione di browse dell'istanza di conservazione
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "openWindow", "window.open('" + e.CommandArgument + "')", true);
            }
            else if (e.CommandName == "REGISTRA_SUPPORTO")
            {
                // Azione successiva alla registrazione del supporto rimovibile

                // TODO: AGGIORNA GRIGLIA

            }
            else if (e.CommandName == "VERIFICA_SUPPORTO")
            {
                // Azione successiva alla verifica del supporto

                string esitoVerifica = this.hd_verifica.Value;

                // TODO: AGGIORNA STATO SUPPORTO
            }
            else if (e.CommandName == "STORIA_VERIFICHE_SUPPORTO")
            {
            }
            else if (e.CommandName == "GO_TO_ISTANZA")
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdSupporti_PreRender(object sender, EventArgs e)
        {
            foreach (DataGridItem itm in this.grdSupporti.Items)
            {
                Label lblIdIstanza = (Label)itm.FindControl("lblIdIstanza");
                Label lblIdSupporto = (Label)itm.FindControl("lblIdSupporto");

                Button btnRegistraSupporto = (Button) itm.FindControl("btnRegistraSupporto");
                if (btnRegistraSupporto != null)
                    btnRegistraSupporto.OnClientClick = string.Format("return showModalDialogRegistraSupportoRimovibile('{0}', '{1}');", lblIdIstanza.Text, lblIdSupporto.Text); 

                
                Button btnVerificaSupporto = (Button) itm.FindControl("btnVerificaSupporto");
                if (btnVerificaSupporto != null)
                    btnVerificaSupporto.OnClientClick = string.Format("return showModalDialogVerificaSupportoRegistrato('{0}', '{1}');", lblIdIstanza.Text, lblIdSupporto.Text);                
            }
        }

        /// <summary>
        /// Caricamento dati dei supporti
        /// </summary>
        protected virtual void FetchSupporti()
        {
            string filtro = this.Filtra();
            WSConservazioneLocale.InfoSupporto[] supporti = ConservazioneManager.getInfoSupporto(filtro, this._infoUtente);

            this.grdSupporti.DataSource = supporti;
            this.grdSupporti.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdSupporti_PageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            this.FetchSupporti();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoSupporto"></param>
        /// <returns></returns>
        protected string GetTipoSupporto(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            return infoSupporto.TipoSupporto;
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
        protected string GetDatiVerifica(WSConservazioneLocale.InfoSupporto infoSupporto)
        {
            string retValue = string.Empty;

            if (!this.IsSupportoRemoto(infoSupporto))
            {
                retValue = string.Format("Ver. il: {0}<BR/>Ver. num.: {1}<BR/>Esito: {2}<BR/>Perc.: {3}%<BR/>Prossima ver. il: {4}",
                                        infoSupporto.dataUltimaVerifica,
                                        infoSupporto.numVerifiche,
                                        infoSupporto.esitoVerifica,
                                        infoSupporto.percVerifica,
                                        infoSupporto.dataProxVerifica);
            }
            else
            {
                retValue = string.Format("Esito: {0}<BR/>Perc.: {1}%",
                                        infoSupporto.esitoVerifica,
                                        infoSupporto.percVerifica);
            }

            return retValue;

//<table>
//<tr>
//<td>Ultima verifica il:</td>
//<td><asp:Label ID="lblDataUltimaVerifica" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).dataUltimaVerifica%>"></asp:Label></td>
//</tr>
//<tr>
//<td>Num. verifica:</td>
//<td><asp:Label ID="lblNumeroVerifica" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).numVerifiche%>"></asp:Label></td>
//</tr>
//<tr>
//<td>Esito:</td>
//<td><asp:Label ID="lblEsitoUltimaVerifica" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).esitoVerifica%>"></asp:Label></td>
//</tr>
//<tr>
//<td>Perc. di verifica:</td>
//<td><asp:Label ID="lblPercentualeVerifica" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).percVerifica%>"></asp:Label></td>
//</tr>
//<tr>
//<td>Prossima verifica il:</td>
//<td><asp:Label ID="lblDataProssimaVerifica" runat="server" Text="<%#((ConservazioneWA.WSConservazioneLocale.InfoSupporto) Container.DataItem).dataProxVerifica%>"></asp:Label></td>
//</tr>
//</table>
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supporto"></param>
        /// <returns></returns>
        protected bool IsSupportoRimovibile(object rowSupporto)
        {
            return ((System.Data.DataRowView) rowSupporto).Row["tipo"].ToString() == string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowSupporto"></param>
        /// <returns></returns>
        protected string GetSfogliaSupportoUrl(object rowSupporto)
        {
            return string.Format("window.open('{0}')", ((System.Data.DataRowView)rowSupporto).Row["istanzaBrowseUrl"].ToString());
        }

        
    }
}
