using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAAdminTool.AdminTool.Gestione_StatoFinale
{
    public partial class GestioneDocsStatoFinale : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.DropDownList ddl_Contatori;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        protected System.Web.UI.WebControls.DropDownList ddl_tipologiaDoc;
        protected System.Web.UI.WebControls.RadioButtonList rblTipo;
        protected System.Web.UI.WebControls.Label lblAnno;
        protected System.Web.UI.WebControls.TextBox tbx_anno;
        protected System.Web.UI.WebControls.TextBox tbx_numProto;
        protected System.Web.UI.WebControls.TextBox tbxDoc;
        protected System.Web.UI.WebControls.DropDownList ddlAooRF;
        protected System.Web.UI.WebControls.Panel pnlAnno;
        protected System.Web.UI.WebControls.Panel panel_Contenuto;
        protected System.Web.UI.WebControls.Panel pnl_RFAOO;
        protected System.Web.UI.WebControls.Label lblAooRF;
        protected System.Web.UI.WebControls.Panel pnlNumero;
        protected System.Web.UI.WebControls.Panel pnlDOC;
        protected System.Web.UI.WebControls.Panel pnlProt;
        protected System.Web.UI.WebControls.Panel pnlTipologia;
        

        
        protected Table table;
        protected SAAdminTool.DocsPaWR.FiltroRicerca[][] qV;
        protected SAAdminTool.DocsPaWR.FiltroRicerca fV1;
        protected SAAdminTool.DocsPaWR.FiltroRicerca[] fVList;
        //protected System.Web.UI.WebControls.Panel pnlAnno;
        protected AmmUtils.WebServiceLink ws;
        protected string codAmm;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
               /* Session["AdminBookmark"] = "StatoFinaleDocumenti";

                //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
                if (Session.IsNewSession)
                {
                    Response.Redirect("../Exit.aspx?FROM=EXPIRED");
                }

                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                if (!ws.CheckSession(Session.SessionID))
                {
                    Response.Redirect("../Exit.aspx?FROM=ABORT");
                }*/
                // ---------------------------------------------------------------
                Page.Response.Expires = -1;

                this.codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");

                ws = new AmmUtils.WebServiceLink();

                if (!IsPostBack)
                {
                    //this.IF_VisDoc.NavigateTo = "../blank_page.htm";
                    ddl_registri = GetRegistriByRuolo(ddl_registri, this); ;

                    tbx_anno.Text = string.Empty;//System.DateTime.Now.Year.ToString();

                    CaricaTipologia(this.ddl_tipologiaDoc);
                    ddl_tipologiaDoc.SelectedIndex = 0;

                    if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
                    {
                        ListItem tipDoc = new ListItem();
                        tipDoc.Value = "Tipologia";
                        tipDoc.Text = "Cerca per Tipologia";
                        this.rblTipo.Items.Add(tipDoc);
                    }
                }

                if (Session["template"] != null)
                    inizializzaPanelContenuto();

              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdDocumento"></param>
        /// <param name="anno"></param>
        /// <param name="IdRegistro"></param>
        /// <returns></returns>
        private DocsPaWR.DocumentoStatoFinale[] GetDocumentiStatoFinale(string IdDocumento, string anno, string IdRegistro, string IdTipologia,string IdAmministrazione, bool Protocollati)
        {
            try
            {
                AdminTool.Manager.SessionManager sessionManager = new Manager.SessionManager();

                return ProxyManager.getWS().GetDocumentiStatoFinale(sessionManager.getUserAmmSession(), IdDocumento, anno, IdRegistro,cbSbloccati.Checked,IdTipologia,IdAmministrazione,Protocollati);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(ex);
            }
        }

        private DocsPaWR.DocumentoStatoFinale[] GetDocumentiStatoFinalePerTipologia( string idOggetto,string id_Aoo_RF,string annoDa,string annoA,string numeroDa,string numeroA,bool sbloccati,string IdAmministrazione)
        {
            try
            {
                AdminTool.Manager.SessionManager sessionManager = new Manager.SessionManager();

                return ProxyManager.getWS().GetDocumentiStatoFinalePerTipologia(sessionManager.getUserAmmSession(),idOggetto,id_Aoo_RF,annoDa,annoA,numeroDa,numeroA,sbloccati,IdAmministrazione);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(ex);
            }
        }



        private DocsPaWR.DocumentoStatoFinale[] GetDocumentiStatoFinalePerTipologia(string idOggetto, string idTemplates,string id_Aoo_RF, string anno, string numero, bool sbloccati,string IdAmministrazione)
        {
            try
            {
                AdminTool.Manager.SessionManager sessionManager = new Manager.SessionManager();

                return ProxyManager.getWS().GetDocumentiStatoFinalePerContatore(sessionManager.getUserAmmSession(), idTemplates ,idOggetto, id_Aoo_RF, anno, numero, sbloccati,IdAmministrazione);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(ex);
            }
        }


        private DocsPaWR.DocumentoStatoFinale[] GetDocumentiStatoFinalePerTipologia(string idTemplate, string anno, bool sbloccati,string IdAmministrazione)
        {
            try
            {
                AdminTool.Manager.SessionManager sessionManager = new Manager.SessionManager();

                return ProxyManager.getWS().GetDocumentiStatoFinalePerTipologiaDocumento(sessionManager.getUserAmmSession(),idTemplate,anno,sbloccati,IdAmministrazione);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(ex);
            }
        }

        protected void btn_cerca_Click(object sender, EventArgs e)
        {
            //cerco idProfile partendo dai dati inseriti
            Cerca();

        }

        protected void Cerca()
        {
            string idDocProt = string.Empty;
            int idProfile = 0;
            bool numeroRisultati = true;
            DocsPaWR.InfoDocumento[] ListaDoc = null;
            string inArchivio = "-1";

            DocsPaWR.DocumentoStatoFinale[] docs = null;
            string IdAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            switch (rblTipo.SelectedValue.ToString())
            {
                case "P":
                    
                    idDocProt = tbx_numProto.Text;

                    docs = this.GetDocumentiStatoFinale(idDocProt, tbx_anno.Text, ddl_registri.SelectedValue, string.Empty, IdAmm, true);
                    bindGridview(docs);
                    break;

                case "NP":
                    idDocProt = tbxDoc.Text;
                    //da verificare,perchè non ho il ruolo utente (amm)----dimitri
                    docs = this.GetDocumentiStatoFinale(idDocProt, string.Empty, string.Empty, string.Empty,IdAmm, false);
                    bindGridview(docs);
                    break;


                case "Tipologia":
                    DropDownList ddl = (DropDownList)panel_Contenuto.FindControl("ddl_Contatori");
                    string id_Aoo_RF = string.Empty;
                    string idOggetto = string.Empty;
                    
                    if (ddl != null && ddl.SelectedValue == "")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert('Attenzione selezionare un contatore')</script>");
                        this.panel_Contenuto.Visible = true;
                        this.pnl_RFAOO.Visible = false;
                        return;
                    }
                    else
                      if(ddl != null)
                          idOggetto = ddl.SelectedValue;

                    if (ddlAooRF != null && ddlAooRF.SelectedIndex > 0)
                        id_Aoo_RF = this.ddlAooRF.SelectedValue;
                    
                    if (string.IsNullOrEmpty(txt_anno.Text) || string.IsNullOrEmpty(txt_numero.Text))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert('Il campo Anno e Numero sono obbligatori')</script>");
                        this.panel_Contenuto.Visible = true;
                        this.pnl_RFAOO.Visible = false;

                    }
                    else
                    {

                        string anno = txt_anno.Text.Trim();
                        string numero = txt_numero.Text.Trim();
                     
                        string idTemplate = ddl_tipologiaDoc.SelectedValue;
                        bool sbloccato = cbSbloccati.Checked;

                        
                        docs = this.GetDocumentiStatoFinalePerTipologia(idOggetto, idTemplate,id_Aoo_RF, anno, numero, sbloccato,IdAmm);
                        bindGridview(docs);
                    }
                    break;
            }
        }

        private void bindGridview(DocsPaWR.DocumentoStatoFinale[] docs)
        {
            if (docs.Length > 0)
            {
                grdDocuments.DataSource = docs;
                grdDocuments.DataBind();
            }
            else
            {
                List<DocsPaWR.DocumentoStatoFinale> docsEmpty=new List<DocsPaWR.DocumentoStatoFinale>();
                grdDocuments.DataSource = docsEmpty;
                grdDocuments.DataBind();
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert('I parametri di ricerca inseriti non hanno fornito risultati.')</script>");
            }
       
        
        }

        #region Metodi di supporto
        public DropDownList GetRegistriByRuolo(DropDownList list, Page page)
        {
           /* try
            {
                if ( page.Session["AMMDATASET"] != null)
                {

                    

                    string IdAmministrazione = page.Session["AMMDATASET"].ToString().Split('@')[3].ToString();
                    string codAmm = page.Session["AMMDATASET"].ToString().Split('@')[0].ToString();
                    System.Collections.ArrayList registriArrayList = null;
                    bool filtroAoo = false;
                    //da verificare,perchè non ho il ruolo utente (amm)----dimitri
                    

                    DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistriNoFiltroAOO(IdAmministrazione, out filtroAoo);

                    if (userRegistri != null && filtroAoo)
                    {
                        registriArrayList = new System.Collections.ArrayList(userRegistri);
                    }
                    else
                    {

                        //SAAdminTool.DocsPaWR.Ruolo ruolo = UserManager.getRuolo(page);
                        registriArrayList = new System.Collections.ArrayList(UserManager.getRegistriByCodAmm(codAmm, string.Empty));
                    }
                    list.Items.Clear();
                    foreach (SAAdminTool.DocsPaWR.OrgRegistro reg in registriArrayList)
                    {
                        if(reg.chaRF != "1")
                            list.Items.Add(new ListItem(reg.Codice, reg.IDRegistro));
                    }
                   
                }
                return list;
            }
            catch (Exception ex)
            {
                //errore nel recupero dei dati
                throw ex;
            }*/
            SAAdminTool.DocsPaWR.OrgRegistro[] listaTotale = null;
            //voglio la lista dei soli RF, quindi al webMethod passero come chaRF il valore 1 (solo RF)
            listaTotale = ws.AmmGetRegistri(codAmm, "0");

            if (listaTotale != null && listaTotale.Length > 0)
            {
                int y = 0;
                for (int i = 0; i < listaTotale.Length; i++)
                {
                    string testo = listaTotale[i].Codice;
                    list.Items.Add(testo);
                    list.Items[y].Value = listaTotale[i].IDRegistro;
                    y++;
                }
            }

            return list;
        }
        #endregion

        protected void IF_VisDoc_Navigate(object sender, EventArgs e)
        {

        }

        protected void ddl_registri_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void rblTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (rblTipo.SelectedValue.ToString())
            {
                case "NP":
                    pnlDOC.Visible = true;
                    pnlProt.Visible = false;
                    pnlTipologia.Visible = false;
                    pnl_RFAOO.Visible = false;
                    panel_Contenuto.Visible = false;
                    pnlAnno.Visible = false;
                    pnlNumero.Visible = false;
                    break;

                case "P":
                    pnlDOC.Visible = false;
                    pnlProt.Visible = true;
                    pnlTipologia.Visible = false;
                    pnl_RFAOO.Visible = false;
                    panel_Contenuto.Visible = false;
                    pnlAnno.Visible = false;
                    pnlNumero.Visible = false;
                    break;

                case "Tipologia":
                    pnlDOC.Visible = false;
                    pnlProt.Visible = false;
                    pnlTipologia.Visible = true;
                    this.ddl_tipologiaDoc.SelectedIndex = 0;
                    pnl_RFAOO.Visible = false;
                    panel_Contenuto.Visible = false;
                    pnlAnno.Visible = true;
                    pnlNumero.Visible = true;
                    txt_anno.Visible = false;
                    lblAnno.Visible = false;
                    txt_numero.Visible = false;
                    lblNumero.Visible = false;
                    ddl_tipologiaDoc_SelectedIndexChanged(new object(), new EventArgs());
                    break;
            }
        }

        #region metodi per la ricerca per contatori

        public static SAAdminTool.DocsPaWR.Templates[] GetTipiDocumento(string idAmministrazione)
        {
            SAAdminTool.AdminTool.Manager.SessionManager manager = new Manager.SessionManager();

            object[] list = SAAdminTool.ProxyManager.getWS().getTemplates(idAmministrazione);

            
            if (list != null && list.Length > 0)
            {
            
                
                System.Collections.ArrayList ret = new System.Collections.ArrayList(list);
                return (DocsPaWR.Templates[])ret.ToArray(typeof(DocsPaWR.Templates));
            }
            else
                return new SAAdminTool.DocsPaWR.Templates[0];
        }
        
        public static SAAdminTool.DocsPaWR.Templates[] getTipologiaConContatore(string idAmministrazione)
        {
            
            SAAdminTool.AdminTool.Manager.SessionManager manager = new Manager.SessionManager();

            object[] list = SAAdminTool.ProxyManager.getWS().getTemplates(idAmministrazione);

            
            if (list != null && list.Length > 0)
            {
            
                
                System.Collections.ArrayList ret = new System.Collections.ArrayList(list);
                SAAdminTool.DocsPaWR.Templates[] template = (DocsPaWR.Templates[])ret.ToArray(typeof(DocsPaWR.Templates));
                List<SAAdminTool.DocsPaWR.Templates> lista = new List<DocsPaWR.Templates>();
                for(int i =0; i<template.Length;i++)
                {
                       SAAdminTool.DocsPaWR.Templates t = SAAdminTool.ProxyManager.getWS().getTemplateById(template[i].SYSTEM_ID.ToString());
                       foreach(DocsPaWR.OggettoCustom o in t.ELENCO_OGGETTI)
                       {
                           if(o.TIPO.DESCRIZIONE_TIPO.ToString().ToUpper().Equals("CONTATORE"))
                           {
                               lista.Add(t);//template[i]);
                               break;
                           }
                       }
                }
                return lista.ToArray();
            }
            else
                return new SAAdminTool.DocsPaWR.Templates[0];
  
        }


        //Recupera tutte le tipologie di documento che hanno almeno un contatore che dipende dall'anno
        // viene utilizzato il metodo che restituisce i contatori di repertorio (vedi Archivio/OpzioniArchivio)
        // a cui viene passato il booleano false per non prendere i contatori di repertorio 
        private void CaricaTipologia(DropDownList ddl)
        {
            if (Session["AMMDATASET"] != null)
            {
                string IdAmministrazione = Session["AMMDATASET"].ToString().Split('@')[3].ToString();

                DocsPaWR.Templates[] listaTemplates = getTipologiaConContatore(IdAmministrazione);
                    //GetTipiDocumento(IdAmministrazione);

                if (listaTemplates.Length > 0)
                {

                    ddl.DataValueField = "SYSTEM_ID";
                    ddl.DataTextField = "DESCRIZIONE";
                 
                    ddl.DataSource = listaTemplates.Where(e => e.ABILITATO_SI_NO == "1" && e.IN_ESERCIZIO == "SI").ToArray();
                    ddl.DataBind();
                }
            
            }
        }

        protected void ddl_tipologiaDoc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
                string idTemplate = ddl_tipologiaDoc.SelectedValue;
                DocsPaWR.Templates templateInSessione = (DocsPaWR.Templates)Session["template"];
                if (!string.IsNullOrEmpty(idTemplate) && templateInSessione != null 
                    && !string.IsNullOrEmpty(templateInSessione.SYSTEM_ID.ToString()))
                {
                    this.pnlAnno.Visible = true;
                    this.panel_Contenuto.Visible = true;
                    this.pnl_RFAOO.Visible = true;
                    this.ddlAooRF.Visible = false;
                    this.lblAooRF.Visible = false;
                    this.pnlNumero.Visible = true;
                    this.txt_anno.Text = "";
                    this.txt_numero.Text = "";
                    if (ddl_tipologiaDoc.SelectedValue != templateInSessione.SYSTEM_ID.ToString())
                    {
                        Session.Remove("template");
                        panel_Contenuto.Controls.Clear();
                    }
                    panel_Contenuto.Controls.Clear();
                }
                if (idTemplate != "")
                {
                   
                    DocsPaWR.Templates template = ProfilazioneDocManager.getTemplateById(idTemplate, this);
                    if (template != null)
                    {
                        Session.Add("template", template);
                        ddlAooRF.Items.Clear();
                        inizializzaPanelContenuto();
                        this.txt_anno.Visible = true;
                        this.txt_anno.Text = "";
                        this.txt_numero.Visible = true;
                        this.txt_numero.Text = "";
                        this.lblAnno.Visible = true;
                        this.lblNumero.Visible = true;
                    }
                    else
                    {
                        pnl_RFAOO.Visible = false;
                    }

                }
           
        }
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            this.PreRenderComplete += new EventHandler(GestioneDocsStatoFinale_PreRenderComplete);
            
            base.OnInit(e);
        }
        protected void GestioneDocsStatoFinale_PreRenderComplete(object sender, EventArgs e)
        {

            //if (hdRefreshGrid.Value == "1")
            //{
            //    //Cerca();
            //    hdRefreshGrid.Value = "";
            //}
       } 

        //Recupera i contatori per una scelta tipologia di documento e li inserisce nella 
        //dropdownlist ddl_Contatori
        private void inizializzaPanelContenuto()
        {

            this.PreRenderComplete += new EventHandler(GestioneDocsStatoFinale_PreRenderComplete);
            lblAooRF.Visible = false;
            //pnl_RFAOO.Visible = false;
            if (Session["template"] != null)
            {
                SAAdminTool.DocsPaWR.Templates template = (SAAdminTool.DocsPaWR.Templates)Session["template"];
                table = new Table();
                table.ID = "table_Contatori";
                TableCell cell_2 = new TableCell();
                int numContatori = 0;
                string testoUnicoContatore = "";
                string idUnicoContatore = "";
                SAAdminTool.DocsPaWR.OggettoCustom oggettoUnico = null;
                ddl_Contatori = new DropDownList();
                ddl_Contatori.ID = "ddl_Contatori";
                //ddl_Contatori.Font.Size = FontUnit.Point(7);
                ddl_Contatori.CssClass = "testo";
                //ddl_Contatori.Font.Name = "Verdana";
                foreach (SAAdminTool.DocsPaWR.OggettoCustom oggetto in template.ELENCO_OGGETTI)
                {
                    //if (oggetto.REPERTORIO == "1")
                    //{
                    //rendo visibili i pannelli
                    if (oggetto.TIPO.DESCRIZIONE_TIPO == "Contatore")
                    {
                        if (oggetto.DESCRIZIONE.Equals(""))
                        {
                            return;
                        }
                        //testoUnicoContatore e idUnicoContatore servono nel caso in cui sia presente un solo
                        //contatore, in questo caso non visualizzo la dropdownlist ma una semplice label
                        testoUnicoContatore = oggetto.DESCRIZIONE.ToString();
                        idUnicoContatore = oggetto.SYSTEM_ID.ToString();
                        oggettoUnico = oggetto;
                        ddl_Contatori.Items.Add(new ListItem(oggetto.DESCRIZIONE.ToString(), oggetto.SYSTEM_ID.ToString()));
                        numContatori++;
                    }
                    //}
                }
                if (oggettoUnico != null)
                {
                    TableRow row = new TableRow();
                    row.ID = "row_Contatori";
                    TableCell cell_1 = new TableCell();
                    TableCell cell_3 = new TableCell();
                    if (numContatori > 1)
                    {
                        ListItem emptyCont = new ListItem();
                        emptyCont.Value = "";
                        emptyCont.Text = "";
                        ddl_Contatori.Items.Add(emptyCont);
                        ddl_Contatori.SelectedValue = "";

                        this.ddlAooRF.Visible = false;

                        cell_1.Controls.Add(ddl_Contatori);
                        ddl_Contatori.AutoPostBack = true;
                        this.ddl_Contatori.SelectedIndexChanged += new System.EventHandler(this.ddl_Contatori_SelectedIndexChanged);
                    }
                    else
                    {
                        Label lblContatore = new Label();
                        lblContatore.ID = "lblContatore";
                        //lblContatore.Font.Size = FontUnit.Point(7);
                        lblContatore.CssClass = "testo_grigio_scuro";
                        //lblContatore.Font.Name = "Verdana";
                        lblContatore.Text = testoUnicoContatore;
                        cell_1.Controls.Add(lblContatore);
                        Label lblContatoreID = new Label();
                        lblContatoreID.ID = "lblContID";
                        lblContatoreID.Text = idUnicoContatore;
                        lblContatoreID.Visible = false;
                        cell_3.Controls.Add(lblContatoreID);
                        ddl_Contatori.Visible = false;

                        //da verificare, perchè non c'è ruolo utente (amm)------dimitri
                        if (ddlAooRF.SelectedIndex == -1)
                        {
                           // DocsPaWR.Ruolo ruoloUtente = UserManager.getRuolo(this);
                            DocsPaWR.OrgRegistro[] registriRfVisibili = this.GetRegistriAmministrazione();

                            switch (oggettoUnico.TIPO_CONTATORE)
                            {
                                case "T":
                                    break;
                                case "A":
                                    lblAooRF.Text = "&nbsp;AOO";
                                    ////Aggiungo un elemento vuoto
                                    ListItem it = new ListItem();
                                    it.Value = "";
                                    it.Text = "";
                                    ddlAooRF.Items.Add(it);
                                    //Distinguo se è un registro o un rf
                                      registriRfVisibili.Where(e => e.chaRF == "0" );
                                    foreach(DocsPaWR.OrgRegistro r in registriRfVisibili)
                                    {
                                        ListItem item = new ListItem();

                                        item.Value = r.IDRegistro;
                                        item.Text = r.Codice;
                                            ddlAooRF.Items.Add(item);
                                        
                                    }
                           
                                    ddlAooRF.Width = 100;
                                    this.pnl_RFAOO.Visible = true;
                                    break;
                                case "R":
                                    lblAooRF.Text = "&nbsp;RF";
                                    ////Aggiungo un elemento vuoto
                                    ListItem it_1 = new ListItem();
                                    it_1.Value = "";
                                    it_1.Text = "";
                                    ddlAooRF.Items.Add(it_1);
                                    //Distinguo se è un registro o un rf
                                    registriRfVisibili.Where(e => e.chaRF == "1" && e.rfDisabled == "0");
                                    foreach(DocsPaWR.OrgRegistro r in registriRfVisibili)
                                    {
                                        ListItem item = new ListItem();

                                        item.Value = r.IDRegistro;
                                        item.Text = r.Codice;
                                            ddlAooRF.Items.Add(item);
                                        
                                    }
                                    ddlAooRF.Width = 100;
                                    this.pnl_RFAOO.Visible = true;
                                    break;
                            }
                        }
                    }
                    row.Cells.Add(cell_1);
                    if (cell_3 != null)
                        row.Cells.Add(cell_3);
                    row.Cells.Add(cell_2);
                    table.Rows.Add(row);

                    panel_Contenuto.Controls.Add(table);
                    this.panel_Contenuto.Visible = true;
                }
                //this.btn_ricerca.Visible = true;
            }
        }

        

        private SAAdminTool.DocsPaWR.OrgRegistro[] GetRegistriAmministrazione()
        {
            string codiceAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
            return this.GetRegistriAmministrazione(codiceAmministrazione, "0");
        }
        private SAAdminTool.DocsPaWR.OrgRegistro[] GetRegistriAmministrazione(string codiceAmministrazione, string chaRF)
        {
            return ProxyManager.getWS().AmmGetRegistri(codiceAmministrazione, chaRF);
        }
        #endregion

        ///// <summary>
        ///// 
        ///// </summary>
        //protected string SelectedIdDocument
        //{
        //    get
        //    {
        //        if (this.grdDocuments.SelectedItem != null)
        //        {
        //            Label lblIdDocumento = this.grdDocuments.SelectedItem.FindControl("lblIdDocumento") as Label;

        //            if (lblIdDocumento != null)
        //                return lblIdDocumento.Text;
        //            else
        //                return string.Empty;
        //        }
        //        else
        //            return string.Empty;
        //    }
        //}

        protected void grdDocuments_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
        e.Item.ItemType == ListItemType.AlternatingItem)
            {

                Label lblId = (Label)e.Item.FindControl("lblIdDoc");

                string IdDoc = lblId.Text;



                ImageButton imgB = (ImageButton)e.Item.FindControl("btnSblocca");
               
                imgB.OnClientClick = "return showDialogVisibilitaStatoFinale('" + IdDoc + "')";
            }
        }

        //Se il contatore è di tipo AOO o rf recupera la lista di AOO o la lista di rf 
        //e li inserisci nella dropdownlist ddlAooRF
        private void ddl_Contatori_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ddl_Contatori.SelectedValue == "")
                this.lblAooRF.Visible = false;
            ddlAooRF.Items.Clear();
            //this.pnl_RFAOO.Visible = false;
            this.panel_Contenuto.Visible = true;
            this.pnl_RFAOO.Visible = true;
            this.pnlAnno.Visible = true;
            this.pnlNumero.Visible = true;
            this.txt_anno.Text = "";
            this.txt_numero.Text = "";
            Session["aoo_rf"] = "";

            Session.Remove("template");
            string idTemplate = ddl_tipologiaDoc.SelectedValue;
            DocsPaWR.Templates template = ProfilazioneDocManager.getTemplateById(idTemplate, this);
            Session.Add("template", template);

            template.ELENCO_OGGETTI.Where(o => o.TIPO.DESCRIZIONE_TIPO == "Contatore");
            foreach (SAAdminTool.DocsPaWR.OggettoCustom oggetto in template.ELENCO_OGGETTI)
            {

                if (oggetto.DESCRIZIONE.Equals(""))
                {
                    return;
                }

                if (oggetto.SYSTEM_ID.ToString().Equals(ddl_Contatori.SelectedItem.Value))
                {
                    //da verificare,perchè non ho il ruolo utente (amm)----dimitri
                    // DocsPaWR.Ruolo ruoloUtente = UserManager.getRuolo(this);

                    DocsPaWR.OrgRegistro[] registriRfVisibili = this.GetRegistriAmministrazione();
                    this.pnl_RFAOO.Visible = false;
                    this.lblAooRF.Visible = true;


                    //da verificare,perchè non ho il ruolo utente (amm)----dimitri
                    switch (oggetto.TIPO_CONTATORE)
                    {
                        case "T":
                            this.pnl_RFAOO.Visible = false;
                            break;
                        case "A":
                            lblAooRF.Text = "&nbsp;AOO";
                            ////Aggiungo un elemento vuoto
                            ListItem it = new ListItem();
                            it.Value = "";
                            it.Text = "";
                            ddlAooRF.Items.Add(it);
                            //Distinguo se è un registro o un rf
                            registriRfVisibili.Where(r => r.chaRF == "0");
                            foreach (DocsPaWR.OrgRegistro r in registriRfVisibili)
                            {
                                ListItem item = new ListItem();

                                item.Value = r.IDRegistro;
                                item.Text = r.Codice;
                                ddlAooRF.Items.Add(item);

                            }
                            ddlAooRF.Width = 100;
                            this.pnl_RFAOO.Visible = true;
                            this.ddlAooRF.Visible = true;
                            break;
                        case "R":
                            lblAooRF.Text = "&nbsp;RF";
                            ////Aggiungo un elemento vuoto
                            ListItem it_1 = new ListItem();
                            it_1.Value = "";
                            it_1.Text = "";
                            ddlAooRF.Items.Add(it_1);
                            //Distinguo se è un registro o un rf
                            registriRfVisibili.Where(r => r.chaRF == "1" && r.rfDisabled == "0");
                            foreach (DocsPaWR.OrgRegistro r in registriRfVisibili)
                            {
                                ListItem item = new ListItem();

                                item.Value = r.IDRegistro;
                                item.Text = r.Codice;
                                ddlAooRF.Items.Add(item);

                            }
                            ddlAooRF.Width = 100;
                            this.pnl_RFAOO.Visible = true;
                            this.ddlAooRF.Visible = true;
                            break;
                    }

                }
                else
                {
                    // poichè la ricerca deve essere fatta per un solo contatore, metto a
                    // stringa vuota il valore di tutti gli altri oggetti del template
                    oggetto.VALORE_DATABASE = string.Empty;
                }

            }
        }
        
    }
}
