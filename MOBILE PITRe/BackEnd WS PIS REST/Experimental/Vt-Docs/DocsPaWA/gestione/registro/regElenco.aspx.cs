using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Globalization;

namespace DocsPAWA.gestione.registro
{
	/// <summary>
	/// Summary description for regElenco.
	/// </summary>
	public class regElenco : DocsPAWA.CssPage
	{
		
        protected System.Web.UI.WebControls.DataGrid DataGrid2;
		// my var
        private DocsPAWA.AdminTool.UserControl.ScrollKeeper skDgTemplate;
		protected DocsPAWA.DocsPaWR.Registro[] listaRegistri;
        protected DocsPAWA.DocsPaWR.Registro[] listaRF;
		protected DocsPaWebCtrlLibrary.ImageButton btn_cambiaStato;
		protected DocsPaWebCtrlLibrary.ImageButton btn_salvaRe;
		protected DocsPaWebCtrlLibrary.ImageButton btn_modificReg;
		protected DocsPaWebCtrlLibrary.ImageButton btn_cambiaStatoReg;
		protected System.Web.UI.HtmlControls.HtmlTableCell TD1;
		protected DocsPaWebCtrlLibrary.ImageButton btn_stampaRegistro;
		protected DocsPaWebCtrlLibrary.ImageButton btn_casellaIstituzionale;
		protected ArrayList Dt_elem;
        protected System.Web.UI.WebControls.Panel pnlRF;

		private void Page_Load(object sender, System.EventArgs e)
		{
				Utils.startUp(this);
			if(!Page.IsPostBack)
			{
				try
				{
					setFormProperties();
					//BindGrid();
                    BindGridRF();
                    
					//setto il ruolo da visualizzare
					//this.lbl_ruolo.Text = UserManager.getRuolo(this).descrizione;

                    if (!this.IsPostBack)
                    {
                        // Selezione primo regitro disponibile
                        //this.PerformActionSelectFirstRegistro();
                        this.PerformActionSelectFirstElemento();
                    }

				}
				catch (System.Exception ex)
				{
					ErrorManager.redirect(this, ex);
				}
			}
            DocsPAWA.AdminTool.UserControl.ScrollKeeper skDgTemplate = new DocsPAWA.AdminTool.UserControl.ScrollKeeper();
            skDgTemplate.WebControl = "DivDGList";
            this.Form.Controls.Add(skDgTemplate);

		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{

            if (GestManager.getRegistroSel() == null)
                this.PerformActionSelectFirstElemento();
            
            
			//abilitazione delle funzioni in base al ruolo
			UserManager.disabilitaFunzNonAutorizzate(this);

            //PEC 3 gestione visibilità(FLAG CONSULTA)
            if (DocsPAWA.utils.MultiCasellaManager.RoleIsAuthorizedConsult(GestManager.getRegistroSel(), UserManager.getRuolo().systemId))
                this.btn_casellaIstituzionale.Enabled = true;
            else
                this.btn_casellaIstituzionale.Enabled = false;
            if (GestManager.getRegistroSel().chaRF != null && GestManager.getRegistroSel().chaRF.Equals("1") ||
                    (!this.btn_casellaIstituzionale.Enabled))
            {
                if (Session["TipoRegistro"] != null)
                    Session["TipoRegistro"] = "disable";
                else
                    Session.Add("TipoRegistro", "disable");
            }
            else
                Session.Remove("TipoRegistro");
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btn_cambiaStatoReg.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cambiaStatoReg_Click);
			this.btn_casellaIstituzionale.Click += new System.Web.UI.ImageClickEventHandler(this.btn_casellaIstituzionale_Click);
			this.btn_modificReg.Click += new System.Web.UI.ImageClickEventHandler(this.btn_modificReg_Click);
			this.btn_stampaRegistro.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampaRegistro_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);
            this.DataGrid2.PreRender += new EventHandler(DataGrid2_PreRender);
		}

        private void DataGrid2_PreRender(object sender, EventArgs e)
        {
            for(int a = 0; a < this.DataGrid2.Items.Count; a++)
            {
                string disab = ((Label)this.DataGrid2.Items[a].Cells[6].Controls[1]).Text;
                if (disab.ToUpper().Equals("TRUE"))
                    this.DataGrid2.Items[a].ForeColor = Color.Red;
            }
        }
		#endregion
		
		#region datagrid

        private string GetImage(string chaRF)
        {
            string retValue = string.Empty;
            string spaceIndent = string.Empty;

            switch (chaRF)
            {
                case "0":
                    retValue = "aoo";
                    break;

                case "1":
                    retValue = "rf";
                    spaceIndent = "&nbsp;&nbsp;";
                    break;
            }

            retValue = spaceIndent + "<img src='../../images/gestione/registri/" + retValue + ".gif' border='0'>";

            return retValue;
        }


        private string GetImageStatoRF(string chaDisabled)
        {
            string retValue = string.Empty;
        
            switch (chaDisabled)
            {               
                case "1":
                    retValue = "disabled";
                    break;

                default:
                    retValue = "abled";
                    break;
            }

            retValue = "<img src='../../images/gestione/registri/" + retValue + ".gif' border='0'>";

            return retValue;
        }

        public void BindGridRF()
        {
            DocsPaWR.Registro[] listaRegistri = UserManager.getListaRegistriWithRF(this,"0","");
            string labelCombo;
            int c = 0;

            if (listaRegistri != null && listaRegistri.Length > 0)
            {
                Dt_elem = new ArrayList();
                string img_stato = "";
                for (int i = 0; i < listaRegistri.Length; i++)
                {

                    img_stato = this.setStatoReg(listaRegistri[i]);
                    Dt_elem.Add(new ColsRF(img_stato, listaRegistri[i].codRegistro, GetImage(listaRegistri[i].chaRF) + " " + listaRegistri[i].descrizione, c, listaRegistri[i].systemId, listaRegistri[i].Sospeso.ToString()));

                    c = c + 1;

                    //prendo gli RF di ciascun registro
                    this.listaRF = UserManager.getListaRegistriWithRF(this, "1", (listaRegistri[i]).systemId);
                    if (listaRF != null)
                    {
                        for (int l = 0; l < listaRF.Length; l++)
                        {
                            Dt_elem.Add(new ColsRF(GetImageStatoRF(listaRF[l].rfDisabled), listaRF[l].codRegistro, GetImage(listaRF[l].chaRF) + " " + listaRF[l].descrizione, c, listaRF[l].systemId, listaRF[l].Sospeso.ToString()));
                          
                            c = c + 1;
                        }
                    }
                }
            }
            if (Dt_elem.Count> 0)
            {
              
                this.DataGrid2.DataSource = Dt_elem;
                this.DataGrid2.DataBind();
              //  this.DataGrid2.SelectedIndex = -1;
            }
           
        }

        public class ColsRF
        {
            //private string img_stato;
            private string codice;
            private string descrizione;
            private int chiave;
            private string systemId;
            private string img_stato;
            private string Sospeso;

            public ColsRF(string img_stato, string codice, string descrizione, int chiave, string systemId, string Sospeso)
            {
                this.codice = codice;
                this.descrizione = descrizione;
                this.chiave = chiave;
                this.systemId = systemId;
                this.img_stato = img_stato;
                this.Sospeso = Sospeso;
            }
            public string Img_stato { get { return img_stato; } }

            public string Codice { get { return codice; } }
            public string Descrizione { get { return descrizione; } }
            public int Chiave { get { return chiave; } }
            public string SystemId { get { return systemId; } }
            public string sospeso { get { return Sospeso; } }
        }

       


        protected void Grid2_OnItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        private void PerformActionSelectFirstElemento()
        {
            if (this.DataGrid2.Items.Count > 0)
            {
                string idReg = ((Label)this.DataGrid2.Items[0].Cells[5].Controls[1]).Text;

                this.PerformActionSelectElemento(idReg);

                this.DataGrid2.SelectedIndex = 0;
            }
        }

       		
        protected void DataGrid2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string idReg = ((Label)this.DataGrid2.SelectedItem.Cells[5].Controls[1]).Text;

            this.PerformActionSelectElemento(idReg);
        }

        private void updateElementoSelezionato(DocsPAWA.DocsPaWR.Registro registro)
        {
            
            this.btn_modificReg.Enabled = false;
            this.btn_stampaRegistro.Enabled = false;
      
            this.btn_cambiaStatoReg.Enabled = false;

            this.btn_casellaIstituzionale.Enabled = true;

            if (registro.chaRF == "1")
            {
                Response.Write("<script>parent.iFrame_dettagli.location='regDettagli.aspx?idReg="+ registro.idAOOCollegata+"';</script>");
                Response.Write("<script>if(parent.iFrame_dettagliRF!=null){parent.iFrame_dettagliRF.location='regDettagliRF.aspx';}</script>");
            }
            else
            {
                if (registro.stato.Equals("C"))
                {
                    this.btn_modificReg.Enabled = true;
                    this.btn_stampaRegistro.Enabled = true;
                }
                else
                {
                    this.btn_modificReg.Enabled = false;
                    this.btn_stampaRegistro.Enabled = false;
                }
                this.btn_cambiaStatoReg.Enabled = true;
                this.btn_casellaIstituzionale.Enabled = true;
                Response.Write("<script>parent.iFrame_dettagli.location='regDettagli.aspx';</script>");
                Response.Write("<script>if(parent.iFrame_dettagliRF!=null){parent.iFrame_dettagliRF.location='../../blank_page.htm'};</script>");
	
            }
        }

        private void PerformActionSelectElemento(string idReg)
        {
            DocsPaWR.Registro rf = UserManager.getRegistroBySistemId(this, idReg);
            GestManager.setRegistroSel(this, rf);
            updateElementoSelezionato(rf);

            //Andrea De Marco - controllo IF-ELSE per Import Pregressi
            if (rf.flag_pregresso == true)
            {
                this.btn_cambiaStatoReg.Enabled = false;
            }
            else
            {
                this.btn_cambiaStatoReg.Enabled = true;
            }

           //
        }
 
        #endregion

        #region setStatoReg
        private string setStatoReg(DocsPAWA.DocsPaWR.Registro  reg)
		{
			string dataApertura=reg.dataApertura;
			string nomeImg;
			string img;
			DateTime dt_cor = DateTime.Now;

			if(UserManager.getStatoRegistro(reg).Equals("G"))
				nomeImg = "stato_giallo2.gif";
			else if (UserManager.getStatoRegistro(reg).Equals("V"))
				nomeImg = "stato_verde2.gif";
			else
				nomeImg = "stato_rosso2.gif";


			img = "<img src=" + "../../images/" + nomeImg + " border=0 width=52, height=18>";

			return img;
		}

		#endregion

		private void setFormProperties()
		{
			//paramentro: I=inserimento, M=modifica 
//			this.btn_nuovoReg.Attributes.Add("onclick", "ApriFinestraGestioneReg('I');");
//			this.btn_modificReg.Attributes.Add("onclick", "ApriFinestraGestioneReg('M');");
			this.btn_modificReg.Enabled = false;
			this.btn_cambiaStatoReg.Enabled = false;
			this.btn_stampaRegistro.Enabled = false;
			this.btn_casellaIstituzionale.Enabled = false;
			
		}
		
		private void btn_modificReg_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			int i = DataGrid2.SelectedIndex;
			if (i>=0)
			{
				Response.Write("<script language='javascript' src='../../LIBRERIE/DocsPA_Func.js'></script>");
				//Response.Write("<script>ApriFinestraGestioneReg('M');</script>");
                Response.Write("<script>ApriFinestraModificaPassReg();</script>");
			}
		}

		private void btn_cambiaStatoReg_Click(object sender, System.Web.UI.ImageClickEventArgs e) 
        {
            DocsPAWA.DocsPaWR.Registro registro = null;
            if (this.DataGrid2.Items.Count > 0)
            {
                string idReg = ((Label)this.DataGrid2.SelectedItem.Cells[5].Controls[1]).Text;
                registro = GestManager.getRegistroById(this.Page, idReg);
            }
            if (registro == null)
            {
                RegisterStartupScript("alertRegistroMancante", "<script language=javascript>alert('Selezionare un registro!');</script>");
                return;
            }

            if (registro != null && registro.Sospeso)
            {
                RegisterStartupScript("alertRegistroSospeso", "<script language=javascript>alert('Attenzione! Lo stato del registro non può essere modificato perchè risulta essere sospeso!');</script>");
                //this.DataGrid2.SelectedIndex = -1;
                //RegisterStartupScript("refresh", "<script language=javascript>document.forms[0].submit();</script>");
                return;
            }

			GestManager.cambiaStatoRegistro(this);			
			BindGridRF();
			//old: updateRegistroSelezionato(GestManager.getRegistroSel(this));
            updateElementoSelezionato(GestManager.getRegistroSel(this));
			this.RefreshDocumentsCallContext();
		}

		private void btn_stampaRegistro_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            try
            {
                DocsPaWR.InfoUtente infoUt = UserManager.getInfoUtente(this);
                DocsPaWR.InfoDocumento infoDoc = new DocsPAWA.DocsPaWR.InfoDocumento();
                DocsPaWR.Registro registro = GestManager.getRegistroSel(this);
                DocsPaWR.Ruolo ruolo = UserManager.getRuolo(this);
                DocsPaWR.StampaRegistroResult StpRegRS = GestManager.StampaRegistro(this, infoUt, ruolo, registro);
                if (StpRegRS != null && StpRegRS.errore != null && StpRegRS.errore != "")
                {
                    string errore = StpRegRS.errore;
                    errore = errore.Replace("'", "\\'");
                    string l_script = "<script language=javascript>alert('" + errore + "');</script>";
                    if (!this.IsStartupScriptRegistered("startup"))
                    {
                        this.RegisterStartupScript("startup", l_script);
                    }

                }
                else
                {
                    infoDoc.docNumber = StpRegRS.docNumber;
                    DocsPaWR.SchedaDocumento schedaDoc = new DocsPAWA.DocsPaWR.SchedaDocumento();
                    schedaDoc = DocumentManager.getDettaglioDocumento(this, infoDoc.idProfile, infoDoc.docNumber);
                    FileManager.setSelectedFileReg(this, schedaDoc.documenti[0], "../../popup");
                    string sval = @"../../popup/ModalVisualStampaReg.aspx?id=" + this.Session.SessionID;
                    RegisterStartupScript("ApriModale", "<script>OpenMyDialog('" + sval + "');</script>");


                }
                Response.Write("<script>parent.iFrame_dettagli.location='regDettagli.aspx';</script>");


            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
		}

		private void btn_casellaIstituzionale_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{	
			DocsPaWR.Registro registro=GestManager.getRegistroSel(this);
			if (registro!=null)
			{
                if (registro.Sospeso)
                {
                    RegisterClientScriptBlock("alertRegistroSospeso", "alert('Il registro selezionato è sospeso!');");
                    return;
                }
				string pageBackGroundService="chkCasellaIst.aspx";
				string request=pageBackGroundService;
				//string wndArgument="height=100,width=100,left=0,top=0";
				string scriptName="startService";
				string scriptBody="<script language=jscript>";
				scriptBody+="function startService(){";
				scriptBody+="alert('Inizio della verifica casella istituzionale\\n\\nATTENZIONE!\\nQuesta operazione potrebbe richiedere anche alcuni minuti.');";
				//width=420,height=150,toolbar=no,directories=no,menubar=no,resizable=yes,scrollbars=no');";
				scriptBody+="wnd=window.open('"+request+"','','width=420,height=150,toolbar=no,directories=no,menubar=no,resizable=yes,scrollbars=no');";
				//scriptBody+="wnd=window.showModelessDialog('"+request+"','"+wndArgument+"');";
				scriptBody+="wnd.focus();";
				//scriptBody+="alert('istitutional mailbox checking is started');";
				scriptBody+="}";
				scriptBody+="</script>";
				Page.RegisterClientScriptBlock(scriptName,scriptBody);
				
				string callingScriptName="call"+scriptName;
				string callingScriptBody="";
				callingScriptBody+="<script language=jscript>";
				callingScriptBody+="startService();";
				callingScriptBody+="</script>";
				Page.RegisterStartupScript(callingScriptName,callingScriptBody);
			}
		}


		#region Gestione callcontext

		/// <summary>
		/// Analisi in tutti i contesti di chiamata
		/// per aggiornare il registro modificato in tutti i documenti
		/// </summary>
		private void RefreshDocumentsCallContext()
		{
			foreach (SiteNavigation.CallContext context in SiteNavigation.CallContextStack.GetContextList())
			{
				if (context.ContextName.Equals(SiteNavigation.NavigationKeys.DOCUMENTO))
				{
					DocsPaWR.SchedaDocumento schedaDocumento=context.SessionState["gestioneDoc.schedaDocumento"] as DocsPAWA.DocsPaWR.SchedaDocumento;
					if (schedaDocumento!=null)
					{
						DocsPaWR.Registro changedRegistro=GestManager.getRegistroSel(this);

						if (schedaDocumento.registro.codRegistro.Equals(changedRegistro.codRegistro))
							schedaDocumento.registro=changedRegistro;
					}
				}
			}
		}

		#endregion
	}
}
