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

//Andrea
using DocsPAWA.utils;
//End Andrea

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for templateTrasmissioni.
	/// </summary>
    public class templateTrasmissioni : DocsPAWA.CssPage
	{

        //Andrea
        private ArrayList listaExceptionTrasmissioni = new ArrayList();
        private string messError = "";
        //End Andrea

		//protected System.Web.UI.WebControls.Label LabelMsg;
		protected System.Web.UI.HtmlControls.HtmlTableCell TD2;
		//protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_cn;
		
		protected System.Web.UI.HtmlControls.HtmlTableRow tr2;
	
		protected DocsPAWA.DocsPaWR.TemplateTrasmissione[] listaTemplate;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label lbl_message;
		protected System.Web.UI.WebControls.Button btn_elimina;
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected ArrayList Dt_elem;

		protected DocsPAWA.DocsPaWR.Utente utente;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected DocsPAWA.DocsPaWR.Ruolo ruolo;
		protected System.Web.UI.WebControls.DataGrid Datagrid2;
		protected System.Web.UI.HtmlControls.HtmlGenericControl iFrame_cn;
		protected DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
		protected System.Web.UI.HtmlControls.HtmlInputHidden txt_confirmDel;
		protected string tipoOggetto;
		protected string idRegistroSel = "";
        protected bool userAutorizedEditingACL;

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.btn_elimina.Attributes.Add("onClick","confirmDel();");

			utente = UserManager.getUtente(this);
			ruolo = UserManager.getRuolo(this);
			tipoOggetto = Request.QueryString["tipoObj"];

            // gestione cessione diritti            
            this.checkIsAutorizedEditingACL();

			if (!Page.IsPostBack)
			{
				
				//string tipoOggetto = Request.QueryString["tipoObj"];
				listaTemplate = TrasmManager.getListaTemplate(this,utente, ruolo, tipoOggetto);

				//appoggio il risultato in sessione 
				Session.Add("templateTrasmissioni.listaTemplate",listaTemplate);
			//	TrasmManager.setDataGridListaTemplate(this, listaTemplate);
				
				

				if (listaTemplate != null)
				{
					this.BindGrid(listaTemplate);
					this.btn_ok.Visible = true;
					this.btn_elimina.Visible = true;
				}

				if (listaTemplate == null || listaTemplate.Length <= 0)
				{
					this.lbl_message.Text = "Modelli non trovati";
					//this.lbl_message.Visible = true;
					//this.btn_ok.Visible = false;
					//this.btn_elimina.Visible = false;
				}

				//Quando in sessione non c'è il registro selezionato, viene preso il primo registro utile
				//associato al ruolo che sta facendo l'operazione
				if(UserManager.getRegistroSelezionato(this) != null)
					idRegistroSel = UserManager.getRegistroSelezionato(this).systemId;
				else
					idRegistroSel = ((DocsPAWA.DocsPaWR.Registro) (UserManager.GetRegistriByRuolo(this,UserManager.getRuolo().systemId))[0]).systemId;
			
				//aggiungo i modelli di trasmissione.
				caricaModelliTrasm(tipoOggetto);
			}

            ////Andrea
            //if (Session["MessError"] != null)
            //    {
            //        messError = Session["MessError"].ToString();
            //        Response.Write("<script language=\"javascript\">alert('" + messError + "');</script>");
            //        Session.Remove("MessError");
            //    }
            ////End Andrea
		}

		#region ModelliTrasmissione
		private void caricaModelliTrasm(string tipoOggetto)
		{
			//string idRegistro = idRegistroSel;
            DocsPAWA.DocsPaWR.Registro[] registri = new DocsPAWA.DocsPaWR.Registro[1];
            //if (idRegistro == "")
            //{
                if (UserManager.getRegistroSelezionato(this) != null)
                {
                    //idRegistro = UserManager.getRegistroSelezionato(this).systemId;
                    registri[0] = UserManager.getRegistroSelezionato(this);
                }
                else
                {
                    //idRegistro = ((DocsPAWA.DocsPaWR.Registro)(UserManager.GetRegistriByRuolo(this, UserManager.getRuolo().systemId))[0]).systemId;
                    registri[0] = ((DocsPAWA.DocsPaWR.Registro)(UserManager.GetRegistriByRuolo(this, UserManager.getRuolo().systemId))[0]);
                }
			//}
			
            //string idRegistro = UserManager.getRegistroSelezionato(this).systemId;
			string idAmm = UserManager.getInfoUtente(this).idAmministrazione;
			string idPeople = UserManager.getInfoUtente(this).idPeople;
			string idCorrGlobali = UserManager.getInfoUtente(this).idCorrGlobali;
            string idRuoloUtente = UserManager.getInfoUtente(this).idGruppo;
			string idTipoDoc = "";
			string idDiagramma = "";
			string idStato = "";
            DocsPaWR.SchedaDocumento schedaDocumento = null;
            schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
			if(System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
			{
				if(tipoOggetto=="D")
				{
					if(schedaDocumento.tipologiaAtto != null)
					{
                        //DocsPaWR.Templates template = wws.getTemplate(idAmm, schedaDocumento.tipologiaAtto.descrizione, schedaDocumento.docNumber);				
                        //if(template != null)
                        //{
                        //    idTipoDoc = template.SYSTEM_ID.ToString();
                        if(schedaDocumento.template != null && schedaDocumento.template.SYSTEM_ID.ToString() != "")
                        {
                            idTipoDoc = schedaDocumento.template.SYSTEM_ID.ToString();

							if(System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
							{
                                DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoDoc(schedaDocumento.tipologiaAtto.systemId, idAmm,this);
								if(dg != null)
								{
									idDiagramma = dg.SYSTEM_ID.ToString();
                                    DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber,this);
									if(stato != null)
										idStato = stato.SYSTEM_ID.ToString();				
								}
							}
						}
					}
                    
				}
				else
				{
					DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato(this);

				}

			}

            if (schedaDocumento != null && schedaDocumento.tipoProto != null && schedaDocumento.tipoProto == "G")
            {
                DocsPAWA.DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistri(this);
                registri = userRegistri;
            }


            ArrayList idModelli = null;
            if (tipoOggetto == "D")
            {
                //idModelli = new ArrayList(wws.getModelliPerTrasm(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, tipoOggetto, schedaDocumento.systemId, idRuoloUtente, false, schedaDocumento.accessRights));
                idModelli = new ArrayList(wws.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, tipoOggetto, schedaDocumento.systemId, idRuoloUtente, false, schedaDocumento.accessRights));
            }
            else
            {
                //idModelli = new ArrayList(wws.getModelliPerTrasm(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, tipoOggetto, FascicoliManager.getFascicoloSelezionato(this).systemID, idRuoloUtente, false, FascicoliManager.getFascicoloSelezionato(this).accessRights));
                idModelli = new ArrayList(wws.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, tipoOggetto, FascicoliManager.getFascicoloSelezionato(this).systemID, idRuoloUtente, false, FascicoliManager.getFascicoloSelezionato(this).accessRights));
            }
			//Costruisco il datagrid
			if(idModelli.Count>0)
			{

				DataTable dt = new DataTable();
				dt.Columns.Add("SYSTEM_ID");
				dt.Columns.Add("MODELLO");

				for(int i=0; i<idModelli.Count; i++)
				{
					DocsPaWR.ModelloTrasmissione mod = (DocsPAWA.DocsPaWR.ModelloTrasmissione) idModelli[i];

                    if (mod.CEDE_DIRITTI != null && mod.CEDE_DIRITTI.Equals("1") && !userAutorizedEditingACL)
                    {
                        continue;
                    }
                    else
                    {
                        if (mod.MITTENTE!=null && mod.MITTENTE.Length <= 1)
                        {
                            DataRow row = dt.NewRow();
                            row[0] = mod.SYSTEM_ID;
                            row[1] = mod.NOME;
                            dt.Rows.Add(row);
                        }
                    }
				}
				this.Datagrid2.DataSource = dt;
                if (this.Datagrid2.Items.Count - 1 == 0 && this.Datagrid2.CurrentPageIndex > 0)
					this.Datagrid2.CurrentPageIndex = this.Datagrid2.CurrentPageIndex - 1;
				this.Datagrid2.DataBind();
				
				this.Datagrid2.Visible = true;
				this.btn_ok.Visible = true;
				this.btn_elimina.Visible = true;
			}
			else
			{
				if(listaTemplate == null || listaTemplate.Length <= 0)
				{
					btn_ok.Visible = false;
					btn_elimina.Visible = false;
				}
				this.Datagrid2.Visible = false;
				this.lbl_message.Text = "Modelli non trovati";
				this.lbl_message.Visible = true;
			}
		}

		#endregion
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
			this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageChange);
			this.DataGrid1.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
			this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
			this.Datagrid2.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.Datagrid2_PageIndexChanged);
			this.Datagrid2.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.Datagrid2_UpdateCommand);
			this.Datagrid2.SelectedIndexChanged += new System.EventHandler(this.Datagrid2_SelectedIndexChanged);
			this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.btn_elimina.Click += new System.EventHandler(this.btn_elimina_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion


		#region datagrid

		private void Datagrid1_PreRender(object sender, System.EventArgs e)
		{
			try
			{
				for(int i=0;i<this.DataGrid1.Items.Count;i++)
				{
					if(this.DataGrid1.Items[i].ItemIndex>=0)
					{	
						switch(this.DataGrid1.Items[i].ItemType.ToString().Trim())
						{
							case "Item":
							{
								this.DataGrid1.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
								this.DataGrid1.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioN'");
								break;
							}
							case "AlternatingItem":
					
							{
								this.DataGrid1.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
								this.DataGrid1.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioA'");
								break;
							}
				
						}
					}
				}
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}
		}

		
		public void BindGrid(DocsPAWA.DocsPaWR.TemplateTrasmissione[] listaTemplate)
		{
			if (listaTemplate != null && listaTemplate.Length > 0)
			{
				//Costruisco il datagrid
				Dt_elem = new ArrayList();

				for(int i= 0; i< listaTemplate.Length ; i++)
				{					
					Dt_elem.Add(new Cols(listaTemplate[i].descrizione,i));
				}
				
				TrasmManager.setDataGridListaTemplate(this,Dt_elem);					
				this.DataGrid1.DataSource=Dt_elem;
				if(this.DataGrid1.Items.Count -1 == 0)
					this.DataGrid1.CurrentPageIndex = this.DataGrid1.CurrentPageIndex - 1;
				this.DataGrid1.DataBind();
				return;
			}
			this.lbl_message.Text = "Modelli non trovati";
			this.DataGrid1.Visible = false;
			//this.lbl_message.Visible = true;
			//this.btn_ok.Visible = false;
			//this.btn_elimina.Visible = false;
			TrasmManager.removeDataGridListaTemplate(this);
	
		}
		
		public class Cols 
		{		
			private string titolo;
			private int chiave;

			public Cols(string titolo, int chiave)
			{
				this.titolo = titolo;
				this.chiave = chiave;
			}
					
			public string Titolo{get{return titolo;}}
			public int    Chiave{get{return chiave;}}						
		}


		private void DataGrid1_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.DataGrid1.CurrentPageIndex=e.NewPageIndex;
			ArrayList ColNew=new ArrayList();
			ColNew=(ArrayList) TrasmManager.getDataGridListaTemplate(this); 
			
			DataGrid1.DataSource=ColNew; 					
			DataGrid1.DataBind();


		}


		private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Datagrid2.SelectedIndex = -1;			
		}

		private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Update")
			{
				Datagrid2.SelectedIndex = -1;
				string str_indexSel = ((Label) this.DataGrid1.Items[e.Item.ItemIndex].Cells[1].Controls[1]).Text;
				int indexSel = Int32.Parse(str_indexSel);
				if (indexSel < 0)
					return;
				setTrasmissioneDaTemplate(indexSel);
				//visualizza il frame con i dettagli
				string tipoOggetto = Request.QueryString["tipoObj"];
				string url;
				if(tipoOggetto.Equals("D"))
				{
					//this.iFrame_cn.NavigateTo="../documento/tabTrasmissioniEff.aspx";
					url="../documento/tabTrasmissioniEff.aspx";

				}
				else
				{
					//this.iFrame_cn.NavigateTo="../fascicolo/tabTrasmissioniEffFasc.aspx";
					url="../fascicolo/tabTrasmissioniEffFasc.aspx";
				}
				//Response.Write("<script>parent.window.document.IFrame_cn.location='"+ url +"';</script>");
				//RegisterStartupScript("frame","<script>parent.window.document.iFrame_cn.location='"+ url +"';</script>");
				RegisterStartupScript("frame","<script>document.iFrame_cn.location='"+ url +"';</script>");
				//Response.Write("<script>document.IFrame_cn.location='"+ url +"';</script>");
				
				//luluciani
				//old : DataGrid1.SelectedIndex = indexSel;
				DataGrid1.SelectedIndex = e.Item.ItemIndex;
			}
		}


		#endregion datagrid

		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			Session.Remove("templateTrasmissioni.listaTemplate");
            Session.Remove("Modello");
            TrasmManager.removeDataGridListaTemplate(this);
            TrasmManager.removeDocTrasmSel(this);
            //TrasmManager.removeGestioneTrasmissione(this);
			Response.Write("<script>window.close();</script>");	

		}

		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			
			// Fa il refresh del frame 
			//(trasmDatiTrasm_dx.aspx per i DOC - trasmFascDatiTras_dx.aspx per i FASC) 
			// mette il flag Salva a "S" 
//			string page;
//			string tipoOggetto = Request.QueryString["tipoObj"];
//			if(tipoOggetto.Equals("D"))
//				page = "trasmDatiTrasm_dx";
//			else
//				page = "trasmFascDatiTras_dx";
//
//			String funct;
//			funct = "top.iFrame_cn."+ page + ".flag_save.value='S'; ";
//			funct += "top.iFrame_cn." + page + ".submit(); ";
//			Response.Write("<script language='javascript'> "+ funct + "</script>");
			
			DocsPaWR.Trasmissione trasmSel;
			if (this.DataGrid1.SelectedIndex <0 && this.Datagrid2.SelectedIndex<0)
			{
				string msg = "Selezionare un template";
				Response.Write("<script>alert('" + msg + "')</script>");
				return;
			}
			string str_indexSel="";
			int indexSel;
			if(this.DataGrid1.SelectedIndex >=0)
			{
				//rimuovo l'eventuale session del modello selezionato.
				if(Session["Modello"]!=null)
				{
					Session.Remove("Modello");	
				}

				str_indexSel = ((Label) this.DataGrid1.SelectedItem.Cells[1].Controls[1]).Text;
				indexSel = Int32.Parse(str_indexSel);
				if (indexSel <0)
					return;
				trasmSel= TrasmManager.getGestioneTrasmissione(this);
				if (trasmSel == null)
					setTrasmissioneDaTemplate(indexSel);
				else
				{
					DocsPaWR.TemplateTrasmissione template;
					listaTemplate = (DocsPAWA.DocsPaWR.TemplateTrasmissione[]) Session["templateTrasmissioni.listaTemplate"];	
					template = (DocsPAWA.DocsPaWR.TemplateTrasmissione) listaTemplate[indexSel];
		
					if (!template.idTrasmissione.Equals(trasmSel.systemId))
						setTrasmissioneDaTemplate(indexSel);
				}
			}
			else if(this.Datagrid2.SelectedIndex>=0)
			{
				//MODELLI TRASMISSIONE
                indexSel = this.Datagrid2.SelectedIndex;
                if (indexSel < 0)
                    return;

                //string sId= this.Datagrid2.Items[indexSel].Cells[0].Text;

				
                //DocsPaWR.ModelloTrasmissione modello = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
                //modello = wws.getModelloByID(UserManager.getInfoUtente(this).idAmministrazione,sId);
               
                //// gestione notifiche trasmissioni
                //DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                //modello = ws.UtentiConNotificaTrasm(modello, "GET");
             
                //Session.Add("Modello",modello);	

                //TrasmManager.removeGestioneTrasmissione(this);
                //TrasmManager.removeDocTrasmSel(this);
                //DocsPaWR.Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
				
                //// gestione della cessione diritti
                //if (modello.CEDE_DIRITTI != null && modello.CEDE_DIRITTI.Equals("1"))
                //{
                //    DocsPaWR.CessioneDocumento objCessione = new DocsPAWA.DocsPaWR.CessioneDocumento();

                //    objCessione.docCeduto = true;
                //    objCessione.idPeople = UserManager.getInfoUtente(this).idPeople;
                //    objCessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;
                //    objCessione.idPeopleNewPropr = modello.ID_PEOPLE_NEW_OWNER;
                //    objCessione.idRuoloNewPropr = modello.ID_GROUP_NEW_OWNER;                    
                //    objCessione.userId = UserManager.getInfoUtente(this).userId;

                //    trasmissione.cessione = objCessione;
                //}

                ////Parametri della trasmissione
                //trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

                //if(modello.CHA_TIPO_OGGETTO == "D")
                //{
                //    DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
				
                //    trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
                //    trasmissione.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
                //}
                //else
                //{
                //    trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                //    trasmissione.infoFascicolo = FascicoliManager.getInfoFascicolo(this);
                //}
                //trasmissione.utente = UserManager.getUtente(this);
                //trasmissione.ruolo = UserManager.getRuolo(this);
				
                ////Parametri delle trasmissioni singole
                //for(int i=0; i<modello.RAGIONI_DESTINATARI.Length; i++)
                //{
                //    DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                //    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                //    for(int j=0; j<destinatari.Count; j++)
                //    {
                        
                //        DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest) destinatari[j];
                //        DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                //        DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this,mittDest.VAR_COD_RUBRICA,DocsPaWR.AddressbookTipoUtente.INTERNO);
                //        if(corr!=null) //corr nullo se non esiste o se è stato disabilitato                           
                //          trasmissione = addTrasmissioneSingola(trasmissione,corr,ragione,mittDest.VAR_NOTE_SING,mittDest.CHA_TIPO_TRASM,mittDest.SCADENZA);	
                //    }
                //}
                DocsPaWR.Trasmissione trasmissione = this.PerformTrasmByMod(indexSel);

				if (trasmissione != null)
				{
					//Mette la tasmissione in Sessione 
					TrasmManager.setGestioneTrasmissione(this,trasmissione);
					TrasmManager.setDocTrasmSel(this,trasmissione);
				}
				//FINE MODELLI TRASMISSIONE

			}

			TrasmManager.removeDataGridListaTemplate(this);
			Session.Remove("templateTrasmissioni.listaTemplate");
			TrasmManager.removeDocTrasmSel(this);

		   //Rimuovo l'eventuale systemID
			trasmSel= TrasmManager.getGestioneTrasmissione(this);
			trasmSel.systemId = null;
			
            TrasmManager.setGestioneTrasmissione(this, trasmSel);
			if(trasmSel != null && trasmSel.tipoOggetto == DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
			{
				Response.Write("<script>window.open('../trasmissione/trasmDatiTrasm_dx.aspx','iFrame_dx'); window.close();</script>");
			}
			else
			{
				Response.Write("<script>window.open('../trasmissione/trasmFascDatiTras_dx.aspx','iFrame_dx'); window.close();</script>");
			}

		}

        private DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr, DocsPAWA.DocsPaWR.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, DocsPaWR.MittDest mittDest) 
		{
			if (trasmissione.trasmissioniSingole != null)
			{
				// controllo se esiste la trasmissione singola associata a corrispondente selezionato
				for(int i = 0; i < trasmissione.trasmissioniSingole.Length; i++) 
				{
					DocsPaWR.TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
					
                  if(corr!=null)
                    if (ts.corrispondenteInterno.systemId.Equals(corr.systemId)) 
					{
						if(ts.daEliminare) 
						{
							((DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
							return trasmissione;
						}
						else
							return trasmissione;
					}
				}			
			}

			// Aggiungo la trasmissione singola
			DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
			trasmissioneSingola.tipoTrasm = tipoTrasm;
			trasmissioneSingola.corrispondenteInterno = corr;
			trasmissioneSingola.ragione = ragione;
			trasmissioneSingola.noteSingole = note;
            //Imposto la data di scadenza
            if (scadenza > 0)
            {
                //string dataScadenza = "";
                System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                //dataScadenza = data.Day + "/" + data.Month + "/" + data.Year;
                trasmissioneSingola.dataScadenza = DocsPAWA.Utils.formatDataDocsPa(data);
            }

			// Aggiungo la lista di trasmissioniUtente
			if(corr is DocsPAWA.DocsPaWR.Ruolo) 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
				DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);
				
                //Andrea
                if (listaUtenti.Length == 0)
                {
                    trasmissioneSingola = null;
                    throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                                                    + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + ".");
                }
                else
                {
                    //ciclo per utenti se dest è gruppo o ruolo
                    for (int i = 0; i < listaUtenti.Length; i++)
                    {
                        DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                        trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)listaUtenti[i];
                        trasmissioneUtente.daNotificare = this.selezionaNotificaDaModello(mittDest, trasmissioneUtente.utente); //TrasmManager.getTxRuoloUtentiChecked();
                        trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    }
                }
			}

			if (corr is DocsPAWA.DocsPaWR.Utente) 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
				DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
				trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente) corr;
                trasmissioneUtente.daNotificare = this.selezionaNotificaDaModello(mittDest, trasmissioneUtente.utente); //TrasmManager.getTxRuoloUtentiChecked();
				
                //Andrea
                if (trasmissioneUtente.utente == null)
                {
                    throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                    + " è inesistente.");
                }
                //End Andrea
                else
                    trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
			}

			if (corr is DocsPAWA.DocsPaWR.UnitaOrganizzativa) 
			{
				DocsPaWR.UnitaOrganizzativa theUo = (DocsPAWA.DocsPaWR.UnitaOrganizzativa) corr;
				DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
				qca.ragione = trasmissioneSingola.ragione;
				qca.ruolo = UserManager.getRuolo();

                //DocsPaWR.Ruolo[] ruoli = UserManager.getListaRuoliInUO(this, theUo, UserManager.getInfoUtente());
				DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati (this, qca, theUo);

                //Andrea
                if (ruoli.Length == 0)
                {
                    throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per la UO: "
                                                        + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                                                        + ".");
                }
                //End Andrea
                else
                {
                    foreach (DocsPAWA.DocsPaWR.Ruolo r in ruoli)
                        if (r != null && r.systemId != null)
                            trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza, mittDest);
                }
				return trasmissione;
			}

			if (trasmissioneSingola != null)
				trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
			/*
			else
			{
				// In questo caso questa trasmissione non può avvenire perché la
				// struttura non ha utenti (TICKET #1608)
				trasm_strutture_vuote.Add (String.Format ("{0} ({1})", corr.descrizione, corr.codiceRubrica));

			}
			*/
			return trasmissione;

		}

        private bool selezionaNotificaDaModello(DocsPaWR.MittDest mittDest, DocsPaWR.Utente utente)
        {
            bool retValue = false;

            try
            {
                if (!mittDest.CHA_TIPO_URP.Equals("U"))
                {
                    if (mittDest.UTENTI_NOTIFICA != null)
                    {
                        foreach (DocsPaWR.UtentiConNotificaTrasm utNot in mittDest.UTENTI_NOTIFICA)
                        {
                            if (utente.idPeople.Equals(utNot.ID_PEOPLE))
                                return Convert.ToBoolean(utNot.FLAG_NOTIFICA.Replace("1", "true").Replace("0", "false"));
                        }
                    }
                    else
                    {
                        retValue = TrasmManager.getTxRuoloUtentiChecked();
                    }
                }
                else
                {
                    retValue = TrasmManager.getTxRuoloUtentiChecked();
                }
            }
            catch
            {
                retValue = false;
            }

            return retValue;
        }

		private DocsPAWA.DocsPaWR.Corrispondente[] queryUtenti(DocsPAWA.DocsPaWR.Corrispondente corr) 
		{
			
			//costruzione oggetto queryCorrispondente
			DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();

			qco.codiceRubrica = corr.codiceRubrica;
			qco.getChildren = true;
		
			qco.idAmministrazione= UserManager.getInfoUtente (this).idAmministrazione;
            qco.fineValidita = true;
			
			//corrispondenti interni
			qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.INTERNO;			
			
			return UserManager.getListaCorrispondenti(this, qco);
		}

		private void setTrasmissioneDaTemplate(int indexSel)
		{
			DocsPaWR.TemplateTrasmissione template;

			//rimuovo la trasmissione dalla sessione
			TrasmManager.removeGestioneTrasmissione(this);
			TrasmManager.removeDocTrasmSel(this);
				
			listaTemplate = (DocsPAWA.DocsPaWR.TemplateTrasmissione[]) Session["templateTrasmissioni.listaTemplate"];
			
			if (indexSel > -1) 
				template = (DocsPAWA.DocsPaWR.TemplateTrasmissione) listaTemplate[indexSel];
			else 
				template = null;

			DocsPaWR.Trasmissione trasmissione = creaTrasmDaTemplate(template);

			if (trasmissione != null)
			{
				//Mette la tasmissione in Sessione 
				TrasmManager.setGestioneTrasmissione(this,trasmissione);
				TrasmManager.setDocTrasmSel(this,trasmissione);
			}

		}

		private void setTrasmissioneDaTemplateNew(int indexSel)
		{
			if (indexSel <0)
				return;

			string sId= this.Datagrid2.Items[indexSel].Cells[0].Text;

				
			DocsPaWR.ModelloTrasmissione modello = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
			modello = wws.getModelloByID(UserManager.getInfoUtente(this).idAmministrazione,sId);
			Session.Add("Modello",modello);	
			TrasmManager.removeGestioneTrasmissione(this);
			TrasmManager.removeDocTrasmSel(this);
			DocsPaWR.Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
				
			//Parametri della trasmissione
			trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
            if (modello != null)
                trasmissione.NO_NOTIFY = modello.NO_NOTIFY;
			if(modello.CHA_TIPO_OGGETTO == "D")
			{
				DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
				
				trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
				trasmissione.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
			}
			else
			{
				trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
				trasmissione.infoFascicolo = FascicoliManager.getInfoFascicolo(this);
			}
			trasmissione.utente = UserManager.getUtente(this);
			trasmissione.ruolo = UserManager.getRuolo(this);
				
			//Parametri delle trasmissioni singole
            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                //Inserito per errore
                if (ragDest != null)
                {
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                        DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                        DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                        if (corr != null)
                            trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, mittDest);
                    }
                }
            }


			if (trasmissione != null)
			{
				//Mette la tasmissione in Sessione 
				TrasmManager.setGestioneTrasmissione(this,trasmissione);
				TrasmManager.setDocTrasmSel(this,trasmissione);
			}
		}

		private DocsPAWA.DocsPaWR.Trasmissione creaTrasmDaTemplate(DocsPAWA.DocsPaWR.TemplateTrasmissione template)
		{
			if (template == null)
				return null;
			DocsPaWR.Trasmissione trasmTemplate = cercaTrasmissioneTemplate(template);
			DocsPaWR.Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione(); 
			//Ricostruisci la nuova trasmissione, con le trasmissioni utenti aggiornate
			trasmissione.utente = utente;
			trasmissione.ruolo = ruolo;
			trasmissione.tipoOggetto = trasmTemplate.tipoOggetto;
			trasmissione.systemId = trasmTemplate.systemId;  //ATTENZIONE è solo temporaneo per un controllo
			trasmissione.noteGenerali = template.descrizione; // CONTROLLARE DOVE PRENDERE L?INFORMAZIONE
			
			if (trasmTemplate.trasmissioniSingole != null && trasmTemplate.trasmissioniSingole.Length > 0)
			for (int i = 0; i < trasmTemplate.trasmissioniSingole.Length; i++)
				trasmissione = addTrasmissioneSingola(trasmissione, trasmTemplate.trasmissioniSingole[i]);
			
			return trasmissione;

		}


		#region cerca la trasmissione relativa al template
		private DocsPAWA.DocsPaWR.Trasmissione cercaTrasmissioneTemplate(DocsPAWA.DocsPaWR.TemplateTrasmissione template)
		{
			DocsPaWR.FiltroRicerca[][] qV;
			DocsPaWR.FiltroRicerca fV1;
			DocsPaWR.FiltroRicerca[] fVList;

			//array contenitore degli array filtro di ricerca
			qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
			qV[0]=new DocsPAWA.DocsPaWR.FiltroRicerca[1];
			fVList=new DocsPAWA.DocsPaWR.FiltroRicerca[0];

			//# filtro "visualizza sottoposti"
				fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
				fV1.argomento=DocsPaWR.FiltriTrasmissioneNascosti.NO_CERCA_INFERIORI.ToString();
				fV1.valore="N";
				fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
			

			//# filtro "id trasmissione"
				fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
				fV1.argomento=DocsPaWR.FiltriTrasmissioneNascosti.ID_TRASMISSIONE.ToString();
				fV1.valore=template.idTrasmissione;
				fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
			

			//# filtro "oggetto trasmesso"
				string tipoOggetto = Request.QueryString["tipoObj"];
				fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
				fV1.argomento=DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString();
				fV1.valore=tipoOggetto;
				fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
			
	
				
			qV[0]=fVList;
			return queryTrasmissioni(qV[0]);

		}

		protected DocsPAWA.DocsPaWR.Trasmissione queryTrasmissioni(DocsPAWA.DocsPaWR.FiltroRicerca[] filt)
		{
			try
			{
				DocsPaWR.Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione(); 
				DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();

				//chiamo web services get trasm effettuate:
				DocsPaWR.Trasmissione[] listaTrasmEff =  TrasmManager.getQueryEffettuate(this,oggettoTrasm, utente,ruolo,filt);
				if(listaTrasmEff!=null)
					if(listaTrasmEff.Length>0)
						trasmissione = listaTrasmEff[0];
					else
					{
						lbl_message.Text="Nessuna Trasmissione Trovata";
						lbl_message.Visible=true;
					}	
				return trasmissione;
			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
				return null;
			}
		}

		#endregion


		#region crea la trasmissione a partire da quella di template
		private DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.TrasmissioneSingola trasmSingolaTemplate) 
		{
			
			DocsPaWR.Corrispondente corr = trasmSingolaTemplate.corrispondenteInterno;
			DocsPaWR.RagioneTrasmissione ragione = trasmSingolaTemplate.ragione;
			// Aggiungo la trasmissione singola
			DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
			trasmissioneSingola.tipoTrasm = "S";
			trasmissioneSingola.corrispondenteInterno = corr;
			trasmissioneSingola.ragione = ragione;
			
			// Aggiungo la lista di trasmissioniUtente
			if( corr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo)) 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
				DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr.codiceRubrica);
				//ciclo per utenti se dest è gruppo o ruolo
				for(int i= 0; i < listaUtenti.Length; i++) 
				{
					DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
					trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente) listaUtenti[i];
                    trasmissioneUtente.daNotificare = TrasmManager.getTxRuoloUtentiChecked();
				    if(ragione.descrizione.Equals("RISPOSTA"))
						trasmissioneUtente.idTrasmRispSing=trasmissioneSingola.systemId;
					trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
				}
			}
			else 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
				DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
				trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente) corr;
                trasmissioneUtente.daNotificare = TrasmManager.getTxRuoloUtentiChecked();
				trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
			}
			trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);

			return trasmissione;

		}

		private DocsPAWA.DocsPaWR.Corrispondente[] queryUtenti(string codiceRubrica) 
		{
			
			//costruzione oggetto queryCorrispondente
			DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();

			qco.codiceRubrica = codiceRubrica;
			qco.getChildren = true;
			qco.fineValidita = true;
			
			qco.idAmministrazione=UserManager.getInfoUtente(this).idAmministrazione;// ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];
			
			//corrispondenti interni
			qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.INTERNO;			
			
			return UserManager.getListaCorrispondenti(this, qco);
		}

		#endregion

		private void btn_elimina_Click(object sender, System.EventArgs e)
		{
			if(this.txt_confirmDel.Value=="si")
			{
				eliminaTemplate();
				this.txt_confirmDel.Value="";
			}
		}

		private void eliminaTemplate()
		{
			try
			{
				int indexSel;
				if(this.DataGrid1.SelectedIndex >=0)
				{
					DocsPaWR.TemplateTrasmissione template;
					string str_indexSel = ((Label) this.DataGrid1.SelectedItem.Cells[1].Controls[1]).Text;
					indexSel = Int32.Parse(str_indexSel);
					if (indexSel <0)
						return;
					listaTemplate = (DocsPAWA.DocsPaWR.TemplateTrasmissione[]) Session["templateTrasmissioni.listaTemplate"];	
					template = (DocsPAWA.DocsPaWR.TemplateTrasmissione) listaTemplate[indexSel];
	
					//elimino il doc (per ora gestiamo solo i doc)
					TrasmManager.deleteTemplate(this, template);
					//aggiorno il dataGrid
					listaTemplate = TrasmManager.rimuoviTemplate(listaTemplate, indexSel);
					Session.Add("templateTrasmissioni.listaTemplate", listaTemplate);
					this.BindGrid(listaTemplate);
				}

				if(this.Datagrid2.SelectedIndex >=0)
				{
					indexSel = this.Datagrid2.SelectedIndex;
					string sId= this.Datagrid2.Items[indexSel].Cells[0].Text;

					DocsPaWR.ModelloTrasmissione modello = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
					modello = wws.getModelloByID(UserManager.getInfoUtente(this).idAmministrazione,sId);
					if(modello.SINGLE == "1")
					{
						RegisterStartupScript("nonCancellabile","<script>alert('Non è possibile cancellare questo modello di tramissione !');</script>");
					}
					else
					{
						UserManager.cancellaModello(this,UserManager.getInfoUtente(this).idAmministrazione,sId);
						caricaModelliTrasm(tipoOggetto);
						this.Datagrid2.SelectedIndex=-1;
					}

				}
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void Datagrid2_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.Datagrid2.CurrentPageIndex=e.NewPageIndex;
			caricaModelliTrasm(tipoOggetto);
//			ArrayList ColNew=new ArrayList();
//			ColNew=(ArrayList) TrasmManager.getDataGridListaTemplate(this); 
//			
//			Datagrid2.DataSource=ColNew; 					
//			Datagrid2.DataBind();

		
		}

		private void Datagrid2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DataGrid1.SelectedIndex = -1;
		}

		private void Datagrid2_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Update")
			{
				DataGrid1.SelectedIndex = -1;
				//string str_indexSel = ((Label) this.Datagrid2.Items[e.Item.ItemIndex].Cells[1].Controls[1]).Text;
				//int indexSel = Int32.Parse(str_indexSel);
				int indexSel =  e.Item.ItemIndex;
				if (indexSel < 0)
					return;
                //setTrasmissioneDaTemplateNew(indexSel);

                // crea una trasmissione momentanea per mostrare nei dettagli come verrebbe la trasmissione
                // se utilizziamo il modello selezionato
                DocsPaWR.Trasmissione trasmMomentanea = this.PerformTrasmByMod(indexSel);
                if (trasmMomentanea != null)
                {
                    //Mette la tasmissione in Sessione 
                    TrasmManager.setGestioneTrasmissione(this, trasmMomentanea);
                    TrasmManager.setDocTrasmSel(this, trasmMomentanea);
                }

				//visualizza il frame con i dettagli
				string tipoOggetto = Request.QueryString["tipoObj"];
				string url;
				if(tipoOggetto.Equals("D"))
				{
					//this.iFrame_cn.NavigateTo="../documento/tabTrasmissioniEff.aspx";
					url="../documento/tabTrasmissioniEff.aspx";

				}
				else
				{
					//this.iFrame_cn.NavigateTo="../fascicolo/tabTrasmissioniEffFasc.aspx";
					url="../fascicolo/tabTrasmissioniEffFasc.aspx";
				}
				//Response.Write("<script>parent.window.document.IFrame_cn.location='"+ url +"';</script>");
				//RegisterStartupScript("frame","<script>parent.window.document.iFrame_cn.location='"+ url +"';</script>");
				RegisterStartupScript("frame","<script>document.iFrame_cn.location='"+ url +"';</script>");
				//Response.Write("<script>document.IFrame_cn.location='"+ url +"';</script>");
				
				//luluciani
				//old : DataGrid1.SelectedIndex = indexSel;
				Datagrid2.SelectedIndex = e.Item.ItemIndex;

                TrasmManager.removeGestioneTrasmissione(this);
                //TrasmManager.removeDocTrasmSel(this);
                Session.Remove("Modello");	
			}		
		}

        protected void DataGrid1_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }
        }

        protected void Datagrid2_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }
        }

        /// <summary>
        /// GESTIONE CESSIONE DIRITTI:
        /// verifica se l'utente è abilitato alla funzione ABILITA_CEDI_DIRITTI_DOC
        /// </summary>
        private void checkIsAutorizedEditingACL()
        {
            userAutorizedEditingACL = UserManager.ruoloIsAutorized(this, "ABILITA_CEDI_DIRITTI_DOC");
        }

        private DocsPaWR.Trasmissione PerformTrasmByMod(int indexSel)
        {                                                
            DocsPaWR.Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
            DocsPaWR.ModelloTrasmissione modello = new DocsPAWA.DocsPaWR.ModelloTrasmissione();

            string sId = this.Datagrid2.Items[indexSel].Cells[0].Text;
            modello = wws.getModelloByID(UserManager.getInfoUtente(this).idAmministrazione, sId);

            // gestione notifiche trasmissioni
            //DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            //modello = ws.UtentiConNotificaTrasm(modello, "GET");

            Session.Add("Modello", modello);

            TrasmManager.removeGestioneTrasmissione(this);
            TrasmManager.removeDocTrasmSel(this);
            if (modello != null)
                trasmissione.NO_NOTIFY = modello.NO_NOTIFY;
            // gestione della cessione diritti
            if (modello.CEDE_DIRITTI != null && modello.CEDE_DIRITTI.Equals("1"))
            {
                DocsPaWR.CessioneDocumento objCessione = new DocsPAWA.DocsPaWR.CessioneDocumento();

                objCessione.docCeduto = true;
                objCessione.idPeople = UserManager.getInfoUtente(this).idPeople;
                objCessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;
                objCessione.idPeopleNewPropr = modello.ID_PEOPLE_NEW_OWNER;
                objCessione.idRuoloNewPropr = modello.ID_GROUP_NEW_OWNER;
                objCessione.userId = UserManager.getInfoUtente(this).userId;

                trasmissione.cessione = objCessione;
            }

            //Parametri della trasmissione
            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

            if (modello.CHA_TIPO_OGGETTO == "D")
            {
                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(this);

                trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
                trasmissione.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
            }
            else
            {
                trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
                trasmissione.infoFascicolo = FascicoliManager.getInfoFascicolo(this);
            }
            trasmissione.utente = UserManager.getUtente(this);
            trasmissione.ruolo = UserManager.getRuolo(this);

            //Parametri delle trasmissioni singole
            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {

                    DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                    DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                    DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    if (corr != null) //corr nullo se non esiste o se è stato disabilitato                           
                        
                        //Andrea - try - catch
                        try
                        {
                            trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, mittDest);
                        }
                        catch (ExceptionTrasmissioni e) 
                        {
                            //Aggiungo l'errore alla lista
                            listaExceptionTrasmissioni.Add(e.Messaggio);

                            //foreach (string s in listaExceptionTrasmissioni)
                            //{
                            //    //messError = messError + s + "\r\n";
                            //    messError = messError + s + "|";
                            //}

                            //if (messError != "")
                            //{
                            //    Session.Add("MessError", messError);
                            //}
                        }
                        //End Andrea
                }

            }

            //Andrea
            foreach (string s in listaExceptionTrasmissioni)
            {
                //messError = messError + s + "\r\n";
                messError = messError + s + "\\n";
            }

            if (messError != "")
            {
                Session.Add("MessError", messError);
            }
            //End Andrea

            if (modello.CEDE_DIRITTI != null && modello.CEDE_DIRITTI.Equals("1"))
                trasmissione.trasmissioniSingole[0].ragione.cessioneImpostata = true;

            DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato(this);

            if (fasc != null)
            {
                trasmissione.infoFascicolo = new DocsPaWR.InfoFascicolo();
                trasmissione.infoFascicolo.codice = fasc.codice;
                trasmissione.infoFascicolo.descrizione = fasc.descrizione;
            }
             
            return trasmissione;
        }
	}
}
