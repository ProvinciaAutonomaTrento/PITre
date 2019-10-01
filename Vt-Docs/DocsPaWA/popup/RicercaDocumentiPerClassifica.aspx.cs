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
using System.Text.RegularExpressions;
using DocsPAWA.DocsPaWR;
using System.Text;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for RicercaDocumentiPerClassifica.
	/// </summary>
    public class RicercaDocumentiPerClassifica : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label lbl_message;
		

		protected System.Web.UI.WebControls.Label lblEndDataProtocollo;
		//protected DocsPaWebCtrlLibrary.DateMask txtEndDataProtocollo;
        protected DocsPAWA.UserControls.Calendar txtEndDataProtocollo;
		protected System.Web.UI.WebControls.Label lblInitNumProto;
		protected System.Web.UI.WebControls.TextBox txtInitNumProto;
        protected System.Web.UI.WebControls.TextBox txt_annoDoc;
        protected System.Web.UI.WebControls.Label lbl_annoDoc;
		protected System.Web.UI.WebControls.Label lblEndNumProto;
		protected System.Web.UI.WebControls.TextBox txtEndNumProto;
		protected System.Web.UI.WebControls.DropDownList ddl_dtaProto;
		protected System.Web.UI.WebControls.Label lblInitDtaProto;
		//protected DocsPaWebCtrlLibrary.DateMask txtInitDtaProto;
        protected DocsPAWA.UserControls.Calendar txtInitDtaProto;
		protected System.Web.UI.WebControls.DropDownList ddl_numProto;
		protected System.Web.UI.WebControls.Button btn_find;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
		protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
		protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;	
		protected static int currentPage;
		protected DocsPAWA.DocsPaWR.InfoDocumento[] infoDoc;
		protected ArrayList Dg_elem;
		protected DocsPAWA.DocsPaWR.FiltroRicerca[][] ListaFiltri;
		protected int numTotPage;
		protected System.Web.UI.WebControls.Label lbl_annoProto;
		protected System.Web.UI.WebControls.TextBox txt_annoProto;
		protected System.Web.UI.WebControls.Label lbl_countRecord;
		protected int nRec;
		protected DataSet ds;
		protected System.Web.UI.WebControls.Button btn_ok;
		protected DataTable dt;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;
		protected string str_indexSel;
		protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDocIngresso;
		protected System.Web.UI.WebControls.Button btn_elimina;
		protected System.Web.UI.WebControls.RadioButtonList rbl_sceltaRicerca;
		protected System.Web.UI.HtmlControls.HtmlTable tbl_cont;
		protected System.Web.UI.WebControls.Panel pnl_filtri_prot;
		protected System.Web.UI.WebControls.DropDownList ddl_idDocumento;
		protected System.Web.UI.WebControls.Label lblDAidDoc;
		protected System.Web.UI.WebControls.TextBox txt_initIdDoc;
		protected System.Web.UI.WebControls.Label lblAidDoc;
		protected System.Web.UI.WebControls.TextBox txt_fineIdDoc;
		protected System.Web.UI.WebControls.DropDownList ddl_dataCreazione;
		protected System.Web.UI.WebControls.Label lblDAdataCreaz;
		//protected DocsPaWebCtrlLibrary.DateMask txt_initDataCreaz;
        protected DocsPAWA.UserControls.Calendar txt_initDataCreaz;
		protected System.Web.UI.WebControls.Label lblAdataCreaz;
		//protected DocsPaWebCtrlLibrary.DateMask txt_fineDataCreaz;
        protected DocsPAWA.UserControls.Calendar txt_fineDataCreaz;
		protected System.Web.UI.WebControls.Panel pnl_filtri_grigi;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected DocsPAWA.DocsPaWR.InfoDocumento infoDocSel = null;
		protected ArrayList chk;
		protected System.Web.UI.WebControls.CheckBox chkSelectDeselectAll;
		protected Hashtable hash_checked;
        protected Utilities.MessageBox msg_InserisciDoc;
        protected System.Web.UI.WebControls.TextBox txt_oggetto;
        protected System.Web.UI.WebControls.ImageButton btn_RubrOgget_E;

        protected DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;
        
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.rbl_sceltaRicerca.SelectedIndexChanged += new System.EventHandler(this.rbl_sceltaRicerca_SelectedIndexChanged);
			this.ddl_numProto.SelectedIndexChanged += new System.EventHandler(this.ddl_numProto_SelectedIndexChanged);
			this.ddl_dtaProto.SelectedIndexChanged += new System.EventHandler(this.ddl_dtaProto_SelectedIndexChanged);
			this.ddl_idDocumento.SelectedIndexChanged += new System.EventHandler(this.ddl_idDocumento_SelectedIndexChanged);
			this.ddl_dataCreazione.SelectedIndexChanged += new System.EventHandler(this.ddl_dataCreazione_SelectedIndexChanged);
			this.btn_find.Click += new System.EventHandler(this.btn_find_Click);
			this.chkSelectDeselectAll.CheckedChanged += new System.EventHandler(this.chkSelectDeselectAll_CheckedChanged);
			this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
			this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChange);
			this.DataGrid1.PreRender += new System.EventHandler(this.DataGrid1_PreRender);
			this.DataGrid1.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid1_ItemDataBound);
			this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);
            this.msg_InserisciDoc.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_InserisciDoc_GetMessageBoxResponse);
        }
		#endregion

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.Response.Expires=-1;

			DocsPAWA.Utils.DefaultButton(this,ref this.GetCalendarControl("txtInitDtaProto").txt_Data,ref btn_find);
			DocsPAWA.Utils.DefaultButton(this,ref this.GetCalendarControl("txtEndDataProtocollo").txt_Data,ref btn_find);
			DocsPAWA.Utils.DefaultButton(this,ref txt_annoProto,ref btn_find);

			DocsPAWA.Utils.DefaultButton(this,ref txtInitNumProto,ref btn_find);
			DocsPAWA.Utils.DefaultButton(this,ref txtEndNumProto,ref btn_find);

			DocsPAWA.Utils.DefaultButton(this,ref this.GetCalendarControl("txt_initDataCreaz").txt_Data,ref btn_find);
			DocsPAWA.Utils.DefaultButton(this,ref this.GetCalendarControl("txt_fineDataCreaz").txt_Data,ref btn_find);
		
			DocsPAWA.Utils.DefaultButton(this,ref txt_initIdDoc,ref btn_find);
			DocsPAWA.Utils.DefaultButton(this,ref txt_fineIdDoc,ref btn_find);
			
			
			
			if (!IsPostBack)
			{
                ViewState["SelectDeselectAllChecked"] = false;
				RicercaDocumentiClassificaSessionMng.SetAsLoaded(this);	//la pagina è stata caricata
				settaConfigurazioneInizialePagina();

                this.btn_RubrOgget_E.Attributes.Add("onclick", "ApriModalOggettarioClassifica('ric_Classifica');");
			}

			if(DataGrid1.Visible)
			{
				//prendo la hasc in sessione
				hash_checked = DocumentManager.getHash(this);
				if(hash_checked==null)
				{
					hash_checked = new Hashtable();
				}
				
				//salvo i check spuntati alla pagina cliccata in precedenza
				foreach(DataGridItem dgItem in DataGrid1.Items)
				{
					CheckBox checkBox = dgItem.Cells[0].Controls[0].FindControl("chkFascicola") as CheckBox;
					Label lbl_key = (Label) dgItem.Cells[8].Controls[1];

					if (lbl_key!= null && checkBox!=null)
					{
						if(checkBox.Checked)//se è spuntato lo inserisco
						{
							if(!hash_checked.ContainsKey(lbl_key.Text))
							{
								hash_checked.Add(lbl_key.Text, lbl_key.Text);
							}
						}
						else //se non è selezionato vedo se è in hashtable, in caso lo rimuovo
						{
							if(hash_checked.ContainsKey(lbl_key.Text))
							{
								hash_checked.Remove(lbl_key.Text);
							}
						}
					}
				}
				//setto il sessione la HASH che contiene gli item selezionati
				DocumentManager.setHash(this, hash_checked);
			}
			else
			{
				this.chkSelectDeselectAll.Visible=false;
			}

            getLettereProtocolli();
			
		}
			
		/// <summary>
		/// Al primo caricamento della pagina viene selezionato di default
		/// il radio button relativo alla ricerca dei protocolli
		/// </summary>
		private void settaConfigurazioneInizialePagina()
		{
			rbl_sceltaRicerca.SelectedIndex = 0;
			this.pnl_filtri_grigi.Visible = false;
			this.pnl_filtri_prot.Visible = true;

			//viene settato l'anno corrente al page load, ma non al postback
			this.txt_annoProto.Text=System.DateTime.Now.Year.ToString();					
		}

		#region ricerca documenti 

        private void annullaDDL()
        {
            this.ddl_dataCreazione.SelectedIndex = 0;
            this.ddl_dtaProto.SelectedIndex = 0;
            this.ddl_idDocumento.SelectedIndex = 0;
            this.ddl_numProto.SelectedIndex = 0;
        }

		private void abilitaCampiRicerca(string valore)
		{
            this.annullaDDL();

			if(valore.Equals("P"))
			{
				pnl_filtri_prot.Visible = true;
				pnl_filtri_grigi.Visible = false;
                this.lbl_annoProto.Visible = true;
                this.txt_annoProto.Visible = true;

				ResetCampiRicercaProtocollo();

				//viene settato l'anno corrente al page load, ma non al postback
				this.txt_annoProto.Text=System.DateTime.Now.Year.ToString();
				string s = "<SCRIPT language='javascript'>try{document.getElementById('" + txtInitNumProto.ID + "').focus();} catch(e){}</SCRIPT>";
				RegisterStartupScript("focus", s);
				
			}
			else if (valore.Equals("G"))
			{
				pnl_filtri_prot.Visible = false;
				pnl_filtri_grigi.Visible = true;
                //CH
                this.txt_annoDoc.Visible = true;
                this.lbl_annoDoc.Visible = true;

				ResetCampiRicercaGrigi();

				//set data corrente corrente al page load, ma non al postback
                //this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text=Utils.getDataOdiernaDocspa();
                this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = String.Empty;
                this.txt_annoDoc.Text = System.DateTime.Now.Year.ToString();

				string s = "<SCRIPT language='javascript'>try{document.getElementById('" + txt_initIdDoc.ID + "').focus();} catch(e){}</SCRIPT>";
				RegisterStartupScript("focus", s);
			}
            else if (valore.Equals("PRED"))
            {
                pnl_filtri_prot.Visible = false;
                pnl_filtri_grigi.Visible = true;
                //CH
                this.txt_annoDoc.Visible = true;
                this.lbl_annoDoc.Visible = true;

                ResetCampiRicercaGrigi();

                //set data corrente corrente al page load, ma non ap postback
                //this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = Utils.getDataOdiernaDocspa();
                this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = String.Empty;
                this.txt_annoDoc.Text = System.DateTime.Now.Year.ToString();

                string s = "<SCRIPT language='javascript'>try{document.getElementById('" + txt_initIdDoc.ID + "').focus();} catch(e){}</SCRIPT>";
                RegisterStartupScript("focus", s);
            }
			else if (valore.Equals("ADL"))
			{
				//quando si clicca su ADL viene disabilitato il pannello visibile in quel momento
				if(pnl_filtri_prot.Visible==true)
					disabilitaPannelloRicProtocolli();
				else if (pnl_filtri_grigi.Visible==true)
					disabilitaPannelloRicGrigi();
			}
		}

		private void btn_find_Click(object sender, System.EventArgs e)
		{
			try
			{
                ViewState["SelectDeselectAllChecked"] = false;
				string idReg = Request.QueryString["idReg"];
				//impostazioni iniziali
				this.DataGrid1.Visible = false;
				this.lbl_countRecord.Visible = false;
				//fine impostazioni

				//si ripulisce la sessione da risultati di ricerca precedente
				RicercaDocumentiClassificaSessionMng.ClearSessionData(this);
			
				//VALIDAZIONE DEI DATI DI RICERCA
			
				switch(rbl_sceltaRicerca.SelectedItem.Value)
				{
						/* **************    Inizio validazione CASO 1     **************
						CASO 1 - si stanno ricercando i protocolli (si è selezionata l'opzione: Doc. protocollati )*/ 
					
					case "P":
					{
						//controllo su validità numero di protocollo
						if(txtInitNumProto.Text!="")
						{
							if(IsValidNumber(txtInitNumProto)==false)
							{
								Response.Write("<script>alert('Il numero di protocollo deve essere numerico!');</script>");
								string s = "<SCRIPT language='javascript'>document.getElementById('" + txtInitNumProto.ID + "').focus();</SCRIPT>";
								RegisterStartupScript("focus", s);
								this.chkSelectDeselectAll.Visible=false;
								return;
							}
                            
						}
						if(txtEndNumProto.Text!="")
						{
							if(IsValidNumber(txtEndNumProto)==false)
							{
								Response.Write("<script>alert('Il numero di protocollo deve essere numerico!');</script>");
								string s = "<SCRIPT language='javascript'>document.getElementById('" + txtEndNumProto.ID + "').focus();</SCRIPT>";
								RegisterStartupScript("focus", s);
								this.chkSelectDeselectAll.Visible=false;
								return;
							}
						}

						//Controllo intervallo date
                        if (this.ddl_dtaProto.SelectedIndex != 0)
                        {
                            if (Utils.isDate(this.GetCalendarControl("txtInitDtaProto").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txtInitDtaProto").txt_Data.Text, this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text))
                            {
                                Response.Write("<script>alert('Verificare intervallo date !');</script>");
                                //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaProto").txt_Data.ID + "').focus();</SCRIPT>";
                                //RegisterStartupScript("focus", s);
                                this.chkSelectDeselectAll.Visible = false;
                                return;
                            }
                        }
						//Controllo range valori relativi al numero di protocollo
						if(Utils.verificaRangeValori(txtInitNumProto.Text, txtEndNumProto.Text))
						{
							Response.Write("<script>alert('Verificare intervallo varori!');</script>");
							string s = "<SCRIPT language='javascript'>document.getElementById('" + txtInitNumProto.ID + "').focus();</SCRIPT>";
							RegisterStartupScript("focus", s);
							this.chkSelectDeselectAll.Visible=false;
							return;					
						}

						//controllo su validità anno di protocollo
						if(txt_annoProto.Text!="")
						{
							if(IsValidYear(txt_annoProto.Text)==false)
							{
								Response.Write("<script>alert('Formato anno non corretto !');</script>");
								string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_annoProto.ID + "').focus();</SCRIPT>";
								RegisterStartupScript("focus", s);
								this.chkSelectDeselectAll.Visible=false;
								return;
							}
						}
					}
						/* ****************    Fine validazione CASO 1     *********************/

						break;

                    case "PRED":
                    {
                        //Controllo intervallo date
                        if (this.ddl_dataCreazione.SelectedIndex != 0)
                        {
                            if (Utils.isDate(this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text, this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text))
                            {
                                Response.Write("<script>alert('Attenzione: verificare l\\'intervallo delle date immesse!');</script>");
                                this.chkSelectDeselectAll.Visible = false;
                                return;
                            }
                        }
                        //Fine controllo intervallo date

                        //CH
                        if (txt_annoDoc.Text != "")
                        {
                            if (IsValidYear(txt_annoDoc.Text) == false)
                            {
                                Response.Write("<script>alert('Formato anno non corretto !');</script>");
                                string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_annoDoc.ID + "').focus();</SCRIPT>";
                                RegisterStartupScript("focus", s);
                                this.chkSelectDeselectAll.Visible = false;
                                return;
                            }
                        }
                        break;
                    }

					case "G":
					{
					
						/* ***************     Inizio validazione CASO 2      ******************
							si stanno ricercando i documenti grigi (si è selezionata l'opzione: Doc. grigi )*/
						// controllo dei campi NUM. PROTOCOLLO numerici				
						if(txt_initIdDoc.Text!="")
						{
							if(IsValidNumber(txt_initIdDoc)==false)
							{
								Response.Write("<script>alert('Il numero di ID documento deve essere numerico!');</SCRIPT>");
								string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initIdDoc.ID + "').focus();</SCRIPT>";
								RegisterStartupScript("focus", s);
								this.chkSelectDeselectAll.Visible=false;
								return;
							}
						}
						if(txt_fineIdDoc.Text!="")
						{
							if(IsValidNumber(txt_fineIdDoc)==false)
							{
								Response.Write("<script>alert('Il numero di ID documento deve essere numerico!');</script>");
								string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_fineIdDoc.ID + "').focus();</SCRIPT>";
								RegisterStartupScript("focus", s);
								this.chkSelectDeselectAll.Visible=false;
								return;
							}
						}

						//Controllo intervallo date
                        if (this.ddl_dataCreazione.SelectedIndex != 0)
                        {
                            if (Utils.isDate(this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text, this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text))
                            {
                                Response.Write("<script>alert('Attenzione: verificare l\\'intervallo delle date immesse!');</script>");
                                //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreaz").txt_Data.ID + "').focus();</SCRIPT>";
                                //RegisterStartupScript("focus", s);
                                this.chkSelectDeselectAll.Visible = false;
                                return;
                            }
                        }
						//Fine controllo intervallo date
			
						//Controllo range valori relativi all'id documento
						if(Utils.verificaRangeValori(txt_initIdDoc.Text, txt_fineIdDoc.Text))
						{
							Response.Write("<script>alert('Attenzione: verificare l\\'intervallo dei valori immessi!');</script>");
							string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initIdDoc.ID + "').focus();</SCRIPT>";
							RegisterStartupScript("focus", s);
							this.chkSelectDeselectAll.Visible=false;
							return;					
						}

                        //CH
                        if (txt_annoDoc.Text != "")
                        {
                            if (IsValidYear(txt_annoDoc.Text) == false)
                            {
                                Response.Write("<script>alert('Formato anno non corretto !');</script>");
                                string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_annoDoc.ID + "').focus();</SCRIPT>";
                                RegisterStartupScript("focus", s);
                                this.chkSelectDeselectAll.Visible = false;
                                return;
                            }
                        }
						/* ***************     Fine validazione CASO 2         ******************/
				
					}
						break;
				}

				currentPage = 1 ;
				
				if (RicercaDocDaFascicolare(rbl_sceltaRicerca.SelectedItem.Value.ToString(), idReg))
				{		
					DocumentManager.setFiltroRicDoc(this,qV);
					LoadData(true);
				}

			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}

		protected bool RicercaDocDaFascicolare(string tipoSelezione, string idReg)
		{
			try
			{	
				
				//array contenitore degli array filtro di ricerca
				qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
				qV[0]= new DocsPAWA.DocsPaWR.FiltroRicerca[1];
				fVList= new DocsPAWA.DocsPaWR.FiltroRicerca[0];

				if(tipoSelezione.Equals("P"))
				{
					#region FILTRI PER RICERCA DOCUMENTI PROTOCOLLATI	

					#region Filtro per REGISTRO
					// se è "" allora il registro associato nodo di Titolario nel quale si 
					//classifica è NULL, cioè visibile a tutti i registri
                    if (idReg != null && idReg != String.Empty && (!UserManager.isFiltroAooEnabled(this))) 
					{
						fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.REGISTRO.ToString();
						fV1.valore = UserManager.getRegistroSelezionato(this).systemId;
						fVList = Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
					#endregion

					#region filtro NUMERO DI PROTOCOLLO
					if (this.ddl_numProto.SelectedIndex == 0)
					{

						if (this.txtInitNumProto.Text!=null && !this.txtInitNumProto.Text.Equals(""))
						{
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
							fV1.valore=this.txtInitNumProto.Text;
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
					}
					else
					{//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
						if (!this.txtInitNumProto.Text.Equals(""))
						{
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
							fV1.valore=this.txtInitNumProto.Text;
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
						if (!this.txtEndNumProto.Text.Equals(""))
						{
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
							fV1.valore=this.txtEndNumProto.Text;
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
					}
					#endregion 

					#region filtro ANNO DI PROTOCOLLO
					
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
					fV1.valore=this.txt_annoProto.Text;
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					#endregion

					#region filtro DATA PROTOCOLLO
					if (this.ddl_dtaProto.SelectedIndex == 0)
					{//valore singolo specificato per DATA_PROTOCOLLO
						if (!this.GetCalendarControl("txtInitDtaProto").txt_Data.Text.Equals(""))
						{
							if(!Utils.isDate(this.GetCalendarControl("txtInitDtaProto").txt_Data.Text))
							{
								Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
								string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaProto").txt_Data.ID + "').focus();</SCRIPT>";
								RegisterStartupScript("focus", s);
								return false;
							}
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
							fV1.valore=this.GetCalendarControl("txtInitDtaProto").txt_Data.Text;
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
					}
					else
					{//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
						if (!this.GetCalendarControl("txtInitDtaProto").txt_Data.Text.Equals(""))
						{
							if(!Utils.isDate(this.GetCalendarControl("txtInitDtaProto").txt_Data.Text))
							{
								Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
								string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaProto").txt_Data.ID + "').focus();</SCRIPT>";
								RegisterStartupScript("focus", s);
								return false;
							}
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
							fV1.valore=this.GetCalendarControl("txtInitDtaProto").txt_Data.Text;
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
						if (!this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text.Equals(""))
						{
							if(!Utils.isDate(this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text))
							{
								Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
								string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtEndDataProtocollo").txt_Data.ID + "').focus();</SCRIPT>";
								RegisterStartupScript("focus", s);
								return false;
							}
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
							fV1.valore=this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text;
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
					}
					#endregion 

					#region filtro per DA_PROTOCOLLARE
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
					fV1.valore= "0";  //corrisponde a 'false'
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
                    //deprecato da elimanare
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriDocumento.TIPO.ToString();
					fV1.valore = "TIPO";
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					#endregion

                    #region PROTOCOLLO_ARRIVO
                                          fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    #endregion
                    #region PROTOCOLLO_PARTENZA
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    #endregion
                    #region PROTOCOLLO_INTERNO
                     fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    #endregion
                   

					#endregion
				}
				if(tipoSelezione.Equals("G"))
				{
					#region FILTRI PER RICERCA DOCUMENTI GRIGI
				
					#region filtro data creazione
					if(this.ddl_dataCreazione.SelectedIndex==0)
					{
						if (this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text!=null && !this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text.Equals(""))
						{
							if(!Utils.isDate(this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text))
							{
								Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
								string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreaz").txt_Data.ID + "').focus();</SCRIPT>";
								RegisterStartupScript("focus", s);
								return false;
							}
							
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
							fV1.valore=this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text;
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
					}
					else
					{
						if (this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text!=null && !this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text.Equals(""))
						{
							if(!Utils.isDate(this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text))
							{
								Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
								string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreaz").txt_Data.ID + "').focus();</SCRIPT>";
								RegisterStartupScript("focus", s);
								return false;
							}
							
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
							fV1.valore=this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text;
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
						if (this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text!=null && !this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text.Equals(""))
						{
							if(!Utils.isDate(this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text))
							{
								Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
								string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataCreaz").txt_Data.ID + "').focus();</SCRIPT>";
								RegisterStartupScript("focus", s);
								return false;
							}
							
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
							fV1.valore=this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text;
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
					}
						
					#endregion

					#region filtro docNumber
					if(this.ddl_idDocumento.SelectedIndex==0)
					{
						if (this.txt_initIdDoc.Text!=null && !this.txt_initIdDoc.Text.Equals(""))
						{
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
							fV1.valore=this.txt_initIdDoc.Text;
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
					}
					else
					{
						if (this.txt_initIdDoc.Text!=null && !this.txt_initIdDoc.Text.Equals(""))
						{
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
							fV1.valore=this.txt_initIdDoc.Text;
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
						if (this.txt_fineIdDoc.Text!=null && !this.txt_fineIdDoc.Text.Equals(""))
						{
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
							fV1.valore=this.txt_fineIdDoc.Text;
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
					}
					#endregion

					#region filtro per tipoProto

					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriDocumento.TIPO.ToString();
				    fV1.valore = "TIPO";
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);

                    #region GRIGI
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    #endregion
                   
					#endregion 

                    #region filtro ANNO DI DOCUMENTO

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                    fV1.valore = this.txt_annoDoc.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    #endregion

					#endregion
				}

                if (tipoSelezione.Equals("PRED"))
                {
                    #region FILTRI PER RICERCA PREDISPOSTI
                    #region Filtro per REGISTRO
                    // se è "" allora il registro associato nodo di Titolario nel quale si 
                    //classifica è NULL, cioè visibile a tutti i registri
                    if (idReg != null && idReg != String.Empty && (!UserManager.isFiltroAooEnabled(this)))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                        fV1.valore = UserManager.getRegistroSelezionato(this).systemId;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    #endregion
                    #region filtro data creazione
                    if (this.ddl_dataCreazione.SelectedIndex == 0)
                    {
                        if (this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text.Equals(""))
                        {
                            if (!Utils.isDate(this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text))
                            {
                                Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                                string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreaz").txt_Data.ID + "').focus();</SCRIPT>";
                                RegisterStartupScript("focus", s);
                                return false;
                            }

                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                            fV1.valore = this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                    else
                    {
                        if (this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text.Equals(""))
                        {
                            if (!Utils.isDate(this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text))
                            {
                                Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                                string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreaz").txt_Data.ID + "').focus();</SCRIPT>";
                                RegisterStartupScript("focus", s);
                                return false;
                            }

                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                            fV1.valore = this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        if (this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text != null && !this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text.Equals(""))
                        {
                            if (!Utils.isDate(this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text))
                            {
                                Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                                string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataCreaz").txt_Data.ID + "').focus();</SCRIPT>";
                                RegisterStartupScript("focus", s);
                                return false;
                            }

                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                            fV1.valore = this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }

                    #endregion

                    #region filtro docNumber
                    if (this.ddl_idDocumento.SelectedIndex == 0)
                    {
                        if (this.txt_initIdDoc.Text != null && !this.txt_initIdDoc.Text.Equals(""))
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                            fV1.valore = this.txt_initIdDoc.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                    else
                    {
                        if (this.txt_initIdDoc.Text != null && !this.txt_initIdDoc.Text.Equals(""))
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                            fV1.valore = this.txt_initIdDoc.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        if (this.txt_fineIdDoc.Text != null && !this.txt_fineIdDoc.Text.Equals(""))
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                            fV1.valore = this.txt_fineIdDoc.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                    #endregion

                    #region filtro per tipoProto

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                    fV1.valore = "TIPO";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                   
                    #region PREDISPOSTI

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    #endregion
                    #endregion

                    #region filtro ANNO DI DOCUMENTO

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                    fV1.valore = this.txt_annoDoc.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    #endregion

                    #endregion
                }

                if (tipoSelezione.Equals("ADL"))
                {
                    #region FILTRI PER RICERCA ADL
                    DocsPAWA.DocsPaWR.Utente userHome = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                    DocsPAWA.DocsPaWR.Ruolo userRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_IN_ADL.ToString();
                    fV1.valore = userHome.idPeople.ToString() + "@" + userRuolo.systemId.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                    fV1.valore = "tipo";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ALLEGATO.ToString();
                    fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                    fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    if (!UserManager.isFiltroAooEnabled(this))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                        fV1.valore = idReg;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
                    fV1.valore = "0";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    #endregion
                }

                #region FILTRO NO ANNULLATI
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ANNULLATO.ToString();
                fV1.valore = "0";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                #region filtro oggetto
                if (!this.txt_oggetto.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.OGGETTO.ToString();
                    fV1.valore = Utils.DO_AdattaString(this.txt_oggetto.Text);
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
			
				qV[0]=fVList;
			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
				return false;
			}
			
			return true;
		}
		#endregion

		#region metodi per il fascicolamento del documento

		private void btn_ok_Click(object sender, System.EventArgs e)
		{														
			setDocInFolder();				
		}

		/// <summary>
		/// Questo metodo inserisce il documento selezionato nella classifica richiesta
		/// </summary>
		private void setDocInFolder()
		{	
			hash_checked = DocumentManager.getHash(this);
			bool outValue = false;
			if(hash_checked!=null  && hash_checked.Count>0)
			{
                infoDoc = RicercaDocumentiClassificaSessionMng.GetListaInfoDocumenti(this);
                string docPrivati = string.Empty;
                string docUtente = string.Empty;
                foreach (DictionaryEntry de in hash_checked)
                {
                    foreach (DocsPAWA.DocsPaWR.InfoDocumento doc in infoDoc)
                    {
                        if (de.Value.ToString().Equals(doc.docNumber))
                        {
                            if ((doc.privato != null) && (doc.privato == "1"))
                            {
                                if (doc.numProt != null)
                                    docPrivati += doc.numProt + "-" + doc.dataApertura + ", ";
                                else
                                    docPrivati += doc.docNumber + ", ";
                            }

                            if ((doc.personale != null) && (doc.personale == "1"))
                            {
                                docUtente += doc.docNumber + ", ";
                            }
                        }
                    }

                }
                if (!docPrivati.Equals(string.Empty) || !docUtente.Equals(string.Empty))
                {
                    string messaggio = "";
                    string msgPrivato = "";
                    string msgPersonale = "";
                    if (!docPrivati.Equals(string.Empty))
                    {
                        msgPrivato = InitMessageXml.getInstance().getMessage("InsDocPrivatoInFasc");
                        docPrivati = docPrivati.Substring(0, docPrivati.Length - 2);
                        msgPrivato = msgPrivato + docPrivati;
                    }
                    if (!docUtente.Equals(string.Empty))
                    {
                        msgPersonale = InitMessageXml.getInstance().getMessage("InsDocsPersonaleInFasc");
                        docUtente = docUtente.Substring(0, docUtente.Length - 2);
                        msgPersonale = msgPersonale + docUtente;
                    }
                    messaggio = msgPrivato + "\\n" + msgPersonale;
                    msg_InserisciDoc.Confirm(messaggio); 

                    
                }
                else
                {
                    //string errori = "I seguenti documenti non possono essere fascicolati perchè bloccati o già fascicolati:\\n"; 

                    //bool IsError = false;
                    bool result = false;
                    //prendo la folder su cui si deve classificare in QueryString
                    string folderId = Request.QueryString["folderId"];
                    //string appoAlert = "";
                    if (folderId != null && folderId != "")
                    {
                        string msg_alert = string.Empty;
                        string initAlert = "Attenzione! I seguenti documenti risultano essere ancora da accettare: impossibile fascicolarli!\\n";
                        String msgFascicolazione = String.Empty;
                        //string initAlert = "Attenzione - i documenti sottostanti sono gia\\' classificati nel \\nfascicolo indicato:\\n";
                        foreach (DictionaryEntry de in hash_checked)
                        {
                            string diritti = DocumentManager.getAccessRightDocBySystemID(de.Value.ToString(), UserManager.getInfoUtente());
                            if (!string.IsNullOrEmpty(diritti))
                            {
                                if (diritti != "20")
                                {
                                    String message = String.Empty;
                                    result = DocumentManager.addDocumentoInFolder(this, de.Value.ToString(), folderId, false, out outValue, out message);
                                    if (!String.IsNullOrEmpty(message))
                                    {
                                        msgFascicolazione += "\\n";
                                        msgFascicolazione += message + "\\n";
                                    }
                                }
                                else
                                    msg_alert += de.Value.ToString() + "\\n";
                            }
                            //if (!result)
                            //{
                            //    DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
                            //    DocsPAWA.DocumentManager.GetInfoDocumento(infoUtente.id
                            //    IsError= true;
                            //    errori += de.Value + "\\n";
                            //    //ClientScript.RegisterClientScriptBlock(this.GetType(), "DocumentoInCheckout", "alert('Il documento n. " +de.Value.ToString()+ " risulta bloccato.\\nImpossibile fascicolarlo  ');", true);
                            //}
                            //						if(!outValue)
                            //						{
                            //							appoAlert += "- " + de.Value.ToString() + " \\n";
                            //						}
                        }
                        //					if (appoAlert!="")
                        //						appoAlert = initAlert + appoAlert;
                        if (!string.IsNullOrEmpty(msg_alert) || msgFascicolazione.Length > 0)
                        {
                            String alert = String.Empty;

                            if (!String.IsNullOrEmpty(msg_alert))
                                alert = initAlert + msg_alert;
                            
                            if(msgFascicolazione.Length > 0)
                                alert += msgFascicolazione;

                            Response.Write("<script>alert('" + alert +  "');</script>");
                        }
                    }

                    RicercaDocumentiClassificaSessionMng.ClearSessionData(this);

                    //				if(appoAlert!="")
                    //				{
                    //					//non va .. non fa il submit del frame a destra
                    //					Page.RegisterStartupScript("","<script>alert('"+appoAlert+"');</script>");
                    //					Response.Write("<script>window.document.location='../fascicolo/tabFascListaDoc.aspx?idFolder="+folderId+"';</script>");
                    //					return;
                    //				}
                    //if (IsError)
                    //{
                    //    ClientScript.RegisterClientScriptBlock(this.GetType(), "DocumentoInCheckout", "alert('" + errori + " ');", true);
                    //}
                    //else
                        Response.Write("<script>window.close();</script>");
                }
            				
			}
			else
			{
				if(hash_checked!=null  && hash_checked.Count==0)
				{
					Response.Write("<script>alert('Selezionare almeno un documento!');</script>");
				}
				else
				{
					Response.Write("<script>alert('Non è stata effettuata nessuna ricerca!');</script>");
				}
			
			}

			#region commento
//			if(DataGrid1.Items.Count > 0)
//			{
//				if(this.DataGrid1.SelectedIndex >=0)
//				{
//					if(this.DataGrid1.SelectedIndex >=0)
//					{
//						str_indexSel = ((Label) this.DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[6].Controls[1]).Text;
//						int indexSel = Int32.Parse(str_indexSel);
//				
//						this.infoDoc = RicercaDocumentiClassificaSessionMng.GetListaInfoDocumenti(this);
//					
//						if (indexSel > -1) 
//							infoDocSel = (DocsPAWA.DocsPaWR.InfoDocumento) this.infoDoc[indexSel];
//
//						//prendo la folder su cui si deve classificare in QueryString
//						string folderId=Request.QueryString["folderId"];
//						if (folderId!=null && folderId != "")
//						{			
//							DocumentManager.addDocumentoInFolder(this,infoDocSel.idProfile, folderId);
//						}
//					}
//
//					DocumentManager.removeDataGridDocumentiPerClassifica(this);
//					RicercaDocumentiClassificaSessionMng.ClearSessionData(this);
//				
//					Response.Write("<script>window.returnValue = 'Y'; window.close();</script>");
//
//				}
//				else
//				{
//					Response.Write("<script>alert('Nessun documento selezionato!');</script>");
//				}
//			}
			#endregion
			
		}

        private void msg_InserisciDoc_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                bool outValue = false;
                string folderId = Request.QueryString["folderId"];
                if (folderId != null && folderId != "")
                {
                    foreach (DictionaryEntry de in hash_checked)
                    {
                        String message = String.Empty;
                        DocumentManager.addDocumentoInFolder(this, de.Value.ToString(), folderId, false, out outValue, out message);
                        
                    }
                }

                RicercaDocumentiClassificaSessionMng.ClearSessionData(this);

                Response.Write("<script>window.close();</script>");
                
            }
        }
		
		#endregion
		
		#region validazione dati in input

	
		private bool IsValidNumber(TextBox numberText)
		{
			bool retValue=true;
			
			try
			{
				int.Parse(numberText.Text);
			}
			catch 
			{
				retValue=false;
			}

			return retValue;
		}

		public bool IsValidYear(string strYear)
		{
			Regex onlyNumberPattern = new Regex(@"\d{4}");
			return onlyNumberPattern.IsMatch(strYear);
		}

		#endregion

		#region gestione pannello ricerca documenti procollati

		/// <summary>
		/// Ripulisce e abilita tutti i campi nel pannello relativo
		/// alla ricerca dei documenti protocollati
		/// </summary>
		private void ResetCampiRicercaProtocollo()
		{
			
			this.ddl_dtaProto.SelectedIndex = 0;
			this.ddl_numProto.SelectedIndex = 0;
			this.ddl_dtaProto.Enabled = true;
			this.ddl_numProto.Enabled = true;

			//inizializzo la sezione dataProtocollo
			this.GetCalendarControl("txtEndDataProtocollo").Visible=false;
            this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Visible = false;
            this.GetCalendarControl("txtEndDataProtocollo").btn_Cal.Visible = false;
			this.lblEndDataProtocollo.Visible=false;
			this.lblInitDtaProto.Visible=false;
			//inizializzo la sezione numeroProtocollo
			this.txtEndNumProto.Visible=false;
			this.lblEndNumProto.Visible=false;
			this.lblInitNumProto.Visible=false;
	
			this.GetCalendarControl("txtInitDtaProto").txt_Data.Enabled=true;
            this.GetCalendarControl("txtInitDtaProto").btn_Cal.Visible = true;
            this.GetCalendarControl("txtInitDtaProto").txt_Data.Visible = true;
			this.GetCalendarControl("txtInitDtaProto").txt_Data.Text=String.Empty;
			this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text = String.Empty;

			this.txtInitNumProto.Enabled=true;
			this.txtInitNumProto.Text = String.Empty;

			this.txt_annoProto.Enabled=true;
			this.lbl_annoProto.Visible = true;
			this.txt_annoProto.Text = String.Empty;
		
		}

		/// <summary>
		/// Disabilita tutti i campi contenuti nel pannello per 
		/// la ricerca dei documenti protocollati
		/// </summary>
		private void disabilitaPannelloRicProtocolli()
		{
			this.ddl_numProto.SelectedIndex = 0;
			this.ddl_dtaProto.SelectedIndex = 0;
			this.ddl_numProto.Enabled=false;
			this.ddl_dtaProto.Enabled=false;

			this.lblInitDtaProto.Visible=false;
			this.lblInitNumProto.Visible=false;
			this.lblEndDataProtocollo.Visible=false;
			this.lblEndNumProto.Visible=false;

            this.GetCalendarControl("txtInitDtaProto").Visible = false;
			this.GetCalendarControl("txtInitDtaProto").txt_Data.Enabled=false;
            this.GetCalendarControl("txtInitDtaProto").btn_Cal.Visible = false;
			this.GetCalendarControl("txtInitDtaProto").txt_Data.Text=String.Empty;
				
			this.GetCalendarControl("txtEndDataProtocollo").Visible=false;
            this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Visible = false;
            this.GetCalendarControl("txtEndDataProtocollo").btn_Cal.Visible = false;
			this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text = String.Empty;

			this.txtInitNumProto.Enabled=false;
			this.txtInitNumProto.Text = String.Empty;
            this.txtEndNumProto.Visible = false;

			this.txt_annoProto.Enabled=false;
			this.txt_annoProto.Text = String.Empty;
            
            this.lbl_annoProto.Visible = false;
            this.txt_annoProto.Visible = false;
        }

		#endregion

		#region gestione pannello ricerca documenti grigi

		/// <summary>
		/// Ripulisce e abilita tutti i campi nel pannello relativo
		/// alla ricerca dei documenti grigi
		/// </summary>
		private void ResetCampiRicercaGrigi()
		{
			
			this.ddl_dataCreazione.SelectedIndex = 0;
			this.ddl_idDocumento.SelectedIndex = 0;

			this.ddl_dataCreazione.Enabled=true;
			this.ddl_idDocumento.Enabled=true;

			//inizializzo la sezione dataCreazione
			this.GetCalendarControl("txt_fineDataCreaz").Visible=false;
            this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Visible = false;
            this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Visible = false;
			this.lblAdataCreaz.Visible=false;
			this.lblDAdataCreaz.Visible=false;
			//inizializzo la sezione idDocumento
			this.txt_fineIdDoc.Visible=false;
			this.lblDAidDoc.Visible=false;
			this.lblAidDoc.Visible=false;
		
			this.GetCalendarControl("txt_initDataCreaz").txt_Data.Enabled=true;
            this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Visible = true;
            this.GetCalendarControl("txt_initDataCreaz").txt_Data.Visible = true;
			this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text=String.Empty;
			this.txt_fineIdDoc.Text = String.Empty;

			this.txt_initIdDoc.Enabled=true;
            this.txt_initIdDoc.Text = String.Empty;

            this.txt_annoDoc.Enabled = true;
            this.lbl_annoDoc.Visible = true;
            this.txt_annoDoc.Text = String.Empty;
		}
		
		/// <summary>
		/// Disabilita tutti i campi contenuti nel pannello per 
		/// la ricerca dei documenti grigi
		/// </summary>
		private void disabilitaPannelloRicGrigi()
		{
			this.ddl_dataCreazione.SelectedIndex = 0;
			this.ddl_idDocumento.SelectedIndex = 0;
			this.ddl_dataCreazione.Enabled=false;
			this.ddl_idDocumento.Enabled=false;

			this.lblDAdataCreaz.Visible=false;
			this.lblDAidDoc.Visible=false;
			this.lblAdataCreaz.Visible=false;
			this.lblAidDoc.Visible=false;

			this.txt_initIdDoc.Enabled=false;
			this.txt_initIdDoc.Text=String.Empty;
				
			this.txt_fineIdDoc.Visible=false;
			this.txt_fineIdDoc.Text = String.Empty;

			this.GetCalendarControl("txt_initDataCreaz").txt_Data.Enabled=false;
            this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Visible = false;
			this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = String.Empty;

            this.GetCalendarControl("txt_finedataCreaz").txt_Data.Visible = false;
            this.GetCalendarControl("txt_finedataCreaz").btn_Cal.Visible = false;
            this.GetCalendarControl("txt_finedataCreaz").txt_Data.Text = String.Empty;

            this.txt_annoDoc.Enabled = false;
            this.lbl_annoDoc.Visible = false;
            this.txt_annoDoc.Text = String.Empty;
        }

		#endregion

		#region gestione eventi pannello ricerca documenti grigi
		
		private void ddl_idDocumento_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.txt_fineIdDoc.Text="";
			
			if(this.ddl_idDocumento.SelectedIndex==0)
			{
				this.txt_fineIdDoc.Visible=false;
				this.lblAidDoc.Visible=false;
				this.lblDAidDoc.Visible=false;
			}
			else
			{
				this.txt_fineIdDoc.Visible=true;
				this.lblAidDoc.Visible=true;
				this.lblDAidDoc.Visible=true;
			}
		}

		private void ddl_dataCreazione_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text="";
			if(this.ddl_dataCreazione.SelectedIndex==0)
			{
				this.GetCalendarControl("txt_fineDataCreaz").Visible=false;
                this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Visible = false;
				this.lblAdataCreaz.Visible=false;
				this.lblDAdataCreaz.Visible=false;
			}
			else
			{
				this.GetCalendarControl("txt_fineDataCreaz").Visible=true;
                this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Visible = true;
				this.lblAdataCreaz.Visible=true;
				this.lblDAdataCreaz.Visible=true;
			}
		}

		#endregion
	
		#region gestione eventi pannello ricerca documenti protocollati

		private void ddl_dtaProto_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text="";
			if(this.ddl_dtaProto.SelectedIndex==0)
			{
				this.GetCalendarControl("txtEndDataProtocollo").Visible=false;
                this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").btn_Cal.Visible = false;
				this.lblEndDataProtocollo.Visible=false;
				this.lblInitDtaProto.Visible=false;
			}
			else
			{
				this.GetCalendarControl("txtEndDataProtocollo").Visible=true;
                this.GetCalendarControl("txtEndDataProtocollo").btn_Cal.Visible = true;
                this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Visible = true;
				this.lblEndDataProtocollo.Visible=true;
				this.lblInitDtaProto.Visible=true;
                this.GetCalendarControl("txtInitDtaProto").Visible = true;
                this.GetCalendarControl("txtInitDtaProto").btn_Cal.Visible = true;
                this.GetCalendarControl("txtInitDtaProto").txt_Data.Visible = true;
            }
		}

		private void ddl_numProto_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.txtEndNumProto.Text="";
			
			if(this.ddl_numProto.SelectedIndex==0)
			{
				this.txtEndNumProto.Visible=false;
				this.lblEndNumProto.Visible=false;
				this.lblInitNumProto.Visible=false;
			}
			else
			{
				this.txtEndNumProto.Visible=true;
				this.lblEndNumProto.Visible=true;
				this.lblInitNumProto.Visible=true;
			}
		}

		#endregion

		#region gestione eventi datagrid

		

		private void DataGrid1_PageIndexChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			DataGrid1.CurrentPageIndex = e.NewPageIndex;
			currentPage = e.NewPageIndex + 1;
			// Cricamento del DataGrid
			this.LoadData(true);

            //if(this.chkSelectDeselectAll.Checked)
            //{
            //    //se l'opzione 'seleziona tutti' è spuntato e cambio pagina del datagrid 
            //    //lo deseleziono
            //    chkSelectDeselectAll.Checked = false;
            //}
		}

        protected void Grid_OnItemCreated(object sender, DataGridItemEventArgs e)
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
		/// Impostazione del colore del carattere per la prima colonna della griglia:
		/// rosso se doc protocollato, altrimenti grigio, sbarrato se il protocollo è annullato
		/// </summary>
		/// <param name="item"></param>
		private void DataGrid1_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) 
			{
				e.Item.Cells[1].Font.Bold=true;
				CheckBox checkBox = e.Item.Cells[0].Controls[0].FindControl("chkFascicola") as CheckBox;
					
				Label lbl=(Label) e.Item.Cells[9].Controls[1];
				Label key = (Label) e.Item.Cells[8].Controls[1];

				string dataAnnull = ((Label) e.Item.Cells[5].Controls[1]).Text;

				if (lbl.Text=="") //doc grigi/pred
				{
					e.Item.Cells[1].ForeColor=Color.Black; 
				}
				else //doc protocollati
				{
					e.Item.Cells[1].ForeColor=Color.Red;

					//disabilito l'immagine per selezionare il documento protocollato
					//poichè se il protocollo è annullato non deve poter essere classificato
						
					if(dataAnnull!=null)
					{
						if(dataAnnull==String.Empty)//se il protocollo non è annullato
						{
							checkBox.Enabled= true;
						}
						else
						{
							checkBox.Enabled = false;
							checkBox.ToolTip = "Il documento è annullato, non è possibile \ninserirlo in un fascicolo";
						}

						try
						{
							DateTime dt = Convert.ToDateTime(dataAnnull);
							e.Item.ForeColor = Color.Red;
							e.Item.Font.Bold=true;
							e.Item.Font.Strikeout=true;	
						}
						catch {}
					}
				}
				//prendo la hashTable dei checkbox ceccati
				hash_checked = DocumentManager.getHash(this);
				if(hash_checked!=null)
				{
					if(hash_checked.ContainsKey(key.Text))
					{
						checkBox.Checked= true;
					}
				}
			}
		}
				
		/// <summary>
		/// 
		/// </summary>
		/// <param name="chiave"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public string GetCheckBoxLabel(int chiave, string flag)
		{
			string lbl = "";
			try 
			{
				lbl="<input type='checkbox' class='testo_grigio' id='chk" + chiave.ToString() + "' name='chk" + chiave.ToString() + "' value='" + chiave.ToString() + "' " + flag + ">";
				return lbl;
			}
			catch(Exception ex)
			
			{
				ErrorManager.redirectToErrorPage(this,ex);
				return lbl;
			}							
		}
		#endregion

		#region caricamento del Datagrid

		private void LoadData(bool updateGrid)
		{
			DocsPaWR.InfoUtente infoUt= new DocsPAWA.DocsPaWR.InfoUtente();
			
			//ricavo i parametri dal QueryString
			string tipoDoc = Request.QueryString["tipoDoc"];
			string idReg = Request.QueryString["idReg"];
			//string folderId=Request.QueryString["folderId"];

			infoUt = UserManager.getInfoUtente(this);
			ListaFiltri = DocumentManager.getFiltroRicDoc(this);
			DocsPaWR.Registro regSel = UserManager.getRegistroSelezionato(this);
            SearchResultInfo[] idProfileList;
            //if(this.rbl_sceltaRicerca.SelectedItem.Value!="ADL")
            //{

            /* ABBATANGELI GIANLUIGI
             * blocco necessario per effettuare il conteggio del numero di documenti ottenuti dalla ricerca
             * Per non modificare getQueryInfoDocumentoPaging è stata forzata la chiamata a getQueryInfoDocumentoPagingCustom
             * che restituisce numTotPage = -2 nel caso sia raggiunto il limite massimo di record accettati in risposta ad una ricerca */
            SearchObject[] resultTemp = null;
            Field[] visibleArray = null;
            resultTemp = DocumentManager.getQueryInfoDocumentoPagingCustom(infoUt, this, this.ListaFiltri, currentPage, out numTotPage, out nRec, true, true, false, 20, false, visibleArray, null, out idProfileList);
            bool outOfMaxRowSearchable = (numTotPage == -2);
            if (!outOfMaxRowSearchable)
            {

                //ricerca dei documenti grigi o protocollati
                infoDoc = DocumentManager.getQueryInfoDocumentoPaging(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, currentPage, out numTotPage, out nRec, true, false, true, true, out idProfileList);

                //}
                //else
                //{
                //    //se l'utente ha selezionato l'opzione 'Ricerca Documenti in ADL', verranno ricercati tutti i doc
                //    //in ADL, grigi e protocollati
                //    DocsPaWR.AreaLavoro areaLavoro = DocumentManager.getListaAreaLavoro(this,tipoDoc,null ,currentPage,out numTotPage,out nRec,idReg);

                //    infoDoc=new DocsPAWA.DocsPaWR.InfoDocumento[areaLavoro.lista.Length];

                //    for (int i=0;i<areaLavoro.lista.Length;i++)
                //        infoDoc[i]=(DocsPAWA.DocsPaWR.InfoDocumento) areaLavoro.lista[i];
                //}


                this.lbl_countRecord.Visible = true;
                this.lbl_countRecord.Text = "Documenti tot:  " + nRec;

                this.DataGrid1.VirtualItemCount = nRec;
                this.DataGrid1.CurrentPageIndex = currentPage - 1;

                //appoggio il risultato in sessione.
                string[] idProfs = new string[idProfileList.Length];
                for (int i = 0; i < idProfileList.Length; i++)
                {
                    idProfs[i] = idProfileList[i].Id;
                }
                RicercaDocumentiClassificaSessionMng.SetListaIdProfile(this, idProfs);
                RicercaDocumentiClassificaSessionMng.SetListaInfoDocumenti(this, infoDoc);

                if (infoDoc != null && infoDoc.Length > 0)
                {
                    this.BindGrid(infoDoc);
                }
                else
                {
                    //rendo invisibile il check per la selezione di tutti i checkbox
                    this.chkSelectDeselectAll.Visible = false;
                }
            }
            else
            {
                utils.AlertPostLoad.OutOfMaxRowSearchable(Page, nRec);
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numprot"></param>
        /// <returns></returns>
        public string getNumProt(string numprot)
        {
            string rtn = " ";
            try
            {
                if(numprot==null)
                    return rtn = "";
                else
                if (numprot == "0" || numprot.Trim()=="")
                    return rtn = "";
                else
                    return rtn = numprot;

            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
                return rtn;
            }

        }

		public void BindGrid(DocsPAWA.DocsPaWR.InfoDocumento[] infos)
		{
			DocsPaWR.InfoDocumento currentDoc;

			if (infos != null  && infos.Length > 0)
			{
				//Costruisco il datagrid
				Dg_elem = new ArrayList();
				string descrDoc=string.Empty;
				int numProt=new Int32();

				for(int i= 0; i< infos.Length ; i++)
				{					
					currentDoc = ((DocsPAWA.DocsPaWR.InfoDocumento) infos[i]);
	
					string data = "";
					if (currentDoc.dataApertura != null && currentDoc.dataApertura .Length > 0)
						data = currentDoc.dataApertura.Substring(0,10);

					if(currentDoc.numProt!= null && !currentDoc.numProt.Equals(""))
					{
						numProt=Int32.Parse(currentDoc.numProt);
						descrDoc=numProt.ToString();
					}
					else //se il doc è grigio
					{
						descrDoc=currentDoc.docNumber;
					}

					descrDoc=descrDoc + "\n" + data;

					bool fascicola = true;
                    string nuova_etichetta = string.Empty;
					if(currentDoc.dataAnnullamento!=null && currentDoc.dataAnnullamento!="")
					{
						fascicola=false;
					}
                    string tipoProto = string.Empty;
                    if (currentDoc.tipoProto != null && currentDoc.tipoProto == "G")
                        tipoProto = "NP";
                    else
                        tipoProto = currentDoc.tipoProto;
                   
                    nuova_etichetta = getEtichetta(tipoProto);
                    Dg_elem.Add(new RicercaDocumentiPerClassificaDataGridItem(descrDoc, currentDoc.idProfile, currentDoc.numProt, currentDoc.codRegistro, currentDoc.oggetto, nuova_etichetta, currentDoc.dataAnnullamento, i, fascicola));
				}
				
				this.DataGrid1.SelectedIndex=-1;
				this.DataGrid1.DataSource = Dg_elem;
				this.DataGrid1.DataBind();
				this.DataGrid1.Visible = true;
				this.chkSelectDeselectAll.Enabled=true;
				this.chkSelectDeselectAll.Visible=true;
			}
			else
			{
				this.DataGrid1.Visible = false;
				this.chkSelectDeselectAll.Visible=false;
				this.lbl_message.Visible = true;
			}
		}

		#endregion

		#region classe per la creazione del datagrid

		public class RicercaDocumentiPerClassificaDataGridItem 
		{		
			private string descDoc;
            private string idProfile;
            private string numProt;
			private string registro;
			private string oggetto;
			private string tipo;
			private string dataAnnullamento;
			private int chiave;
			private bool fascicola;
		

			public RicercaDocumentiPerClassificaDataGridItem(string descDoc,string idProfile,string numProt, string registro, string oggetto, string tipo, string dataAnnullamento, int chiave, bool fascicola)
			{
				this.descDoc = descDoc;
				this.idProfile = idProfile;
                this.numProt = numProt;
				this.registro = registro;
				this.oggetto = oggetto;
				this.tipo = tipo;
				this.dataAnnullamento = dataAnnullamento;
				this.chiave = chiave;
				this.fascicola=fascicola;
			}
					
			public string DescDoc{get{return descDoc;}}
            public string IdProfile { get { return idProfile; } }
            public string NumProt { get { return numProt; } }
			public string Registro{get{return registro;}}
			public string Oggetto{get{return oggetto;}}
			public string Tipo{get{return tipo;}}
			public string DataAnnullamento{get{return dataAnnullamento;}}
			public int    Chiave{get{return chiave;}}
			public bool   Fascicola{get{return fascicola;}}
		}

		#endregion

		#region gestione eventi della pagina

		private void rbl_sceltaRicerca_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//impostazioni iniziali
			this.DataGrid1.Visible = false;
			this.lbl_countRecord.Visible = false;
			//si ripulisce la sessione da risultati di ricerca precedente
			RicercaDocumentiClassificaSessionMng.ClearSessionData(this);
			this.chkSelectDeselectAll.Visible=false;
			this.chkSelectDeselectAll.Checked=false;
			//fine impostazioni

			switch(this.rbl_sceltaRicerca.SelectedItem.Value)
			{
				case "P":
					abilitaCampiRicerca("P");
					break;
				case "G":
					abilitaCampiRicerca("G");
					break;
				case "ADL":
					abilitaCampiRicerca("ADL");
					break;
                case "PRED":
                    abilitaCampiRicerca("PRED");
                    break;
			}
		}

		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{			
			Response.Write("<SCRIPT>window.close();</SCRIPT>");
		}

		#endregion

		/// <summary>
		/// Gestione selezione / deselezione di tutti i checkbox colonna associa
		/// </summary>
		/// <param name="value"></param>
		private void SelectAllCheck(bool value)
		{

            
            DataGridItemCollection gridItems = this.DataGrid1.Items;
            string [] IdProfiles = RicercaDocumentiClassificaSessionMng.GetListaIdProfile(this);
            if (IdProfiles != null)
            {
                ViewState["SelectDeselectAllChecked"] = value;

                // foreach (DataGridItem gridItem in gridItems)
                foreach (string infoD in IdProfiles)
                {



                    if (value)
                    {
                        if (!hash_checked.ContainsKey(infoD))
                            hash_checked.Add(infoD, infoD);
                    }
                    else
                    {
                        if(hash_checked.Contains(infoD))
                        hash_checked.Remove(infoD);
                    }
                   
                }
                foreach (DataGridItem gridItem in gridItems)
                {
                    string dataAnnull = ((Label)gridItem.Cells[5].Controls[1]).Text;
                    CheckBox checkBox =
                        gridItem.Cells[0].Controls[0].FindControl("chkFascicola") as CheckBox;

                    //salto il settaggio del valore per i checkbox relativi a documenti annullati
                    if (checkBox != null && string.IsNullOrEmpty(dataAnnull))
                        checkBox.Checked = value;
                }
            }
		}

		private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if(e.CommandName=="ShowInfo")
			{
				
				DocsPaWR.InfoDocumento newinfoDoc = null;
				if(e.Item.ItemIndex >=0)
				{
					string str_indexSel = ((Label) this.DataGrid1.Items[e.Item.ItemIndex].Cells[7].Controls[1]).Text;
					int indexSel = Int32.Parse(str_indexSel);

					this.infoDoc = RicercaDocumentiClassificaSessionMng.GetListaInfoDocumenti(this);
			
					if (indexSel > -1) 
						newinfoDoc = (DocsPAWA.DocsPaWR.InfoDocumento) this.infoDoc[indexSel];
			
					if(newinfoDoc!=null)
					{
						DocumentManager.setRisultatoRicerca(this,newinfoDoc);
						FascicoliManager.removeFascicoloSelezionato(this);
						FascicoliManager.removeFolderSelezionato(this);
						RicercaDocumentiClassificaSessionMng.ClearSessionData(this);
						Response.Write("<script>window.open('../documento/gestionedoc.aspx?tab=protocollo','principale');</script>");
						Response.Write("<script>window.close();</script>");
					
					}
				}
			}
		}

		private void DataGrid1_PreRender(object sender, System.EventArgs e)
		{
            
                bool SelectDeselectAllChecked = (bool)ViewState["SelectDeselectAllChecked"];
          
			chkSelectDeselectAll.Checked=SelectDeselectAllChecked;
            DataGrid dg=((DataGrid) sender);
			for(int i=0;i<dg.Items.Count;i++)
			{
				string dataAnnull=((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text;
							
				if(dataAnnull!=null)
				{	
					if(dataAnnull!=String.Empty)//se il protocollo è annullato
					{
						dg.Items[i].ToolTip = "Il documento è annullato: non è possibile inserirlo in un fascicolo.";
					}
				}					
			}
			//se il datagrid ha un solo item allora lo rendo spuntato di default
			if(dg!=null && dg.Items!=null && dg.Items.Count==1 && dg.PageCount==1)
			{
				CheckBox checkBox = dg.Items[0].Cells[0].Controls[0].FindControl("chkFascicola") as CheckBox;
				checkBox.Checked = true;
				this.chkSelectDeselectAll.Enabled = false;
			}
			//Per la ricerca dei doc grigi nascondo la colonna REGISTRO
			if(this.rbl_sceltaRicerca.SelectedItem.Value=="G")
			{
				dg.Columns[2].Visible=false;
			}
		}

		private void chkSelectDeselectAll_CheckedChanged(object sender, System.EventArgs e)
		{
			this.SelectAllCheck(((CheckBox) sender).Checked);
		}

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }

        //INSERITA DA FABIO PRENDE LE ETICHETTE DEI PROTOCOLLI
        private void getLettereProtocolli()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = session.getUserAmmSession();
            String idAmm = null;
            if (UserManager.getInfoUtente() != null && !string.IsNullOrEmpty(UserManager.getInfoUtente().idAmministrazione))
                idAmm = UserManager.getInfoUtente().idAmministrazione;
            this.etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
        }

        //CALCOLA ETICHETTA PROTOCOLLI
        private string getEtichetta(String etichetta)
        {
            if (etichetta.Equals("A"))
            {
                return this.etichette[0].Descrizione;
            }
            else
            {
                if (etichetta.Equals("P"))
                {
                    return this.etichette[1].Descrizione;
                }
                else
                {
                    if (etichetta.Equals("I"))
                    {
                        return this.etichette[2].Descrizione;
                    }
                    else
                    {
                        if (etichetta.Equals("ALL"))
                        {
                            return this.etichette[4].Descrizione;
                        }
                        else
                        {
                            return this.etichette[3].Descrizione;
                        }
                    }
                }
            }
        }


		#region classe per la gestione della sessione
		/// <summary>
		/// Classe per la gestione dei dati in sessione relativamente
		/// alla dialog "RicercaDocumentiPerClassifica"
		/// </summary>
		public sealed class RicercaDocumentiClassificaSessionMng
		{
			private RicercaDocumentiClassificaSessionMng()
			{
			}
			/// <summary>
			/// Gestione rimozione dati in sessione
			/// </summary>
			/// <param name="page"></param>
			public static void ClearSessionData(Page page)
			{
				DocumentManager.removeFiltroRicDoc(page);
				DocumentManager.removeDataGridDocumentiPerClassifica(page);
				
				DocumentManager.removeInfoDocumento(page);
				DocumentManager.removeHash(page);

				RemoveListaInfoDocumenti(page);
				page.Session.Remove("DocumentiPerClassificaSessionMng.dialogReturnValue");
				
			}

            public static void SetListaIdProfile(Page page, string[] listaIdprofile)
            {
                page.Session["DocumentiPerClassifica.ListaIdprofile"] = listaIdprofile;
            }

            public static string[] GetListaIdProfile(Page page)
            {
                return page.Session["DocumentiPerClassifica.ListaIdprofile"] as string[];
            }

			public static void SetListaInfoDocumenti(Page page,DocsPaWR.InfoDocumento[] listaDocumenti)
			{
				page.Session["DocumentiPerClassifica.ListaInfoDoc"]=listaDocumenti;
			}

			public static DocsPAWA.DocsPaWR.InfoDocumento[] GetListaInfoDocumenti(Page page)
			{
				return page.Session["DocumentiPerClassifica.ListaInfoDoc"] as DocsPAWA.DocsPaWR.InfoDocumento[];
			}

			public static void RemoveListaInfoDocumenti(Page page)
			{
				page.Session.Remove("DocumentiPerClassifica.ListaInfoDoc");
			}

			/// <summary>
			/// Impostazione flag booleano, se true, la dialog è stata caricata almeno una volta
			/// </summary>
			/// <param name="page"></param>
			public static void SetAsLoaded(Page page)
			{
				page.Session["DocumentiPerClassificaSessionMng.isLoaded"]=true;
			}

			/// <summary>
			/// Impostazione flag relativo al caricamento della dialog
			/// </summary>
			/// <param name="page"></param>
			public static void SetAsNotLoaded(Page page)
			{
				page.Session.Remove("DocumentiPerClassificaSessionMng.isLoaded");
			}

			/// <summary>
			/// Verifica se la dialog è stata caricata almeno una volta
			/// </summary>
			/// <param name="page"></param>
			/// <returns></returns>
			public static bool IsLoaded(Page page)
			{
				return (page.Session["DocumentiPerClassificaSessionMng.isLoaded"]!=null);
			}

			/// <summary>
			/// Impostazione valore di ritorno
			/// </summary>
			/// <param name="page"></param>
			/// <param name="dialogReturnValue"></param>
			public static void SetDialogReturnValue(Page page,bool dialogReturnValue)
			{
				page.Session["DocumentiPerClassificaSessionMng.dialogReturnValue"]=dialogReturnValue;
			}

			/// <summary>
			/// Reperimento valore di ritorno
			/// </summary>
			/// <param name="page"></param>
			/// <returns></returns>
			public static bool GetDialogReturnValue(Page page)
			{
				bool retValue=false;

				if (IsLoaded(page))
					retValue=Convert.ToBoolean(page.Session["DocumentiPerClassificaSessionMng.dialogReturnValue"]);

				page.Session.Remove("DocumentiPerClassificaSessionMng.isLoaded");

				return retValue;
			}
	
			#endregion
		}
	}
}
