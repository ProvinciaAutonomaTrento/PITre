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

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for datagridRubrica.
	/// </summary>
	public class datagridRubrica : DocsPaWA.Utils.Page
	{
		protected DocsPAWA.DocsPaWR.Corrispondente[] listaCorr;
		protected DocsPAWA.DocsPaWR.Corrispondente[] listaCorrExp;
		protected DataTable cont;
		protected DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente qco;
		protected System.Web.UI.WebControls.PlaceHolder ph;
		protected System.Web.UI.HtmlControls.HtmlInputRadioButton rb;
		protected string wnd;
		protected ArrayList listaExp;
		protected string target;
		protected DataTable dtcont;
		protected Hashtable listCorrHash;
		protected DocsPAWA.DocsPaWR.Corrispondente corr;
		protected string rhd;
		protected ArrayList chk;
		protected string firstime;
		protected DataView dv;
		protected string indexchk;
		protected System.Web.UI.WebControls.Label Label5;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hiddenField;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hiddenField2;
		protected DocsPAWA.dataSet.DataSetRubrica dataSetRubrica1;
		
		protected DocsPAWA.DocsPaWR.Ruolo userRuolo;	
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{

				if(this.DataGrid1!=null && this.DataGrid1.Items!=null && this.DataGrid1.Items.Count>0)
				{
					chk=new ArrayList();
					for(int i=0;i<this.DataGrid1.Items.Count;i++)
					{
						indexchk="chk"+i.ToString();
						if(Request.Form[indexchk]!=null)
							chk.Add(Request.Form["chk"+i.ToString()]);
					}
				}

				rhd=Request.Form["dt"]; 
				firstime=Request.QueryString["firstime"];
				wnd=Request.QueryString["wnd"];
				target=Request.QueryString["target"];
				
				userRuolo = (DocsPAWA.DocsPaWR.Ruolo) Session["userRuolo"];

				//target=(String)Session["target"];
				if(Session["Rubrica.qco"]!=null)
					//se la carico la prima volta senza filtri settati, entro qui.
				{
					qco=(DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente)Session["Rubrica.qco"];

					qco.idRegistri = UserManager.getListaIdRegistri(this);

					if(Session["rubrica.listaCorr"]!=null && IsPostBack)
					{
						listaCorr=(DocsPAWA.DocsPaWR.Corrispondente[])Session["rubrica.listaCorr"];
						//						this.DataGrid1.DataSource=(DocsPAWA.dataSet.DataSetRubrica)Session["dt_rubrica"];
						//						this.DataGrid1.DataBind();

					
					}
					else
					{
						listaCorr=this.searchCorr(qco,wnd,"noexp");
						if(listaCorr.Length!=0)
						{
							
							Session["rubrica.listaCorr"]=listaCorr;
							Session["dt_rubrica"]=caricaDataTableContenitore();
						}
						else
						{
							if(this.firstime.Equals("Y"))
								printMsg(" ");
							else
							{
								printMsg("Nessun Risultato");
								if(this.hiddenField2.Value=="T")
									this.Inserimento();
								return;
							}
								
						}	

						
						//this.DataGrid1.DataSource=(DataTable)Session["dt_rubrica"];
					}
				
					if(!IsPostBack )
					{
						listCorrHash=new Hashtable();
						listaExp=new ArrayList();
						this.DataGrid1.Visible=true;
						
						dv=this.dataSetRubrica1.Tables[0].DefaultView;
						dv.Sort="Descrizione ASC,Tipo DESC";
						
						this.DataGrid1.DataSource=dv;
						Session["rubrica_dt"]=this.dataSetRubrica1;
						this.DataGrid1.DataBind();

						this.DataGrid1.Columns[0].Visible=false;
 
						for(int i=0;i<this.listaCorr.Length;i++)

						{
							if(!(listCorrHash.ContainsKey(listaCorr[i].codiceRubrica)))
								listCorrHash.Add(listaCorr[i].codiceRubrica,listaCorr[i]);
						}
						Session["hashRub"]=listCorrHash;
						Session["listaExp"]=this.listaExp;
					}
					listCorrHash=(Hashtable)Session["hashRub"];
				}
			
				if (wnd!=null) 
				{
					if ((wnd == "proto" && target == "dest")||(wnd == "trasm" ) || (wnd == "protoInt" && target == "dest") )
					{
						this.DataGrid1.Columns[0].Visible = true; 
						this.DataGrid1.Columns[1].Visible = false;
					}
					else
					{
						this.DataGrid1.Columns[0].Visible = false; 
						this.DataGrid1.Columns[1].Visible = true;
					}
				}
				if(IsPostBack && hiddenField.Value.Equals("noexp"))
					
					if(hiddenField2.Value.Equals("F"))
						CtrlCheck(target,wnd);
					else
					{
						listCorrHash=(Hashtable)Session["hashRub"];
						Inserimento();
					}
			}
			catch (System.Exception ex)
			{
				ErrorManager.redirect(this, ex);
			}
		}

		public string getDescrizioneECodice(string descrizione,string codice)
		{
			string retValue;
			if ( codice.Equals("") )
					retValue=descrizione;
			else
				retValue=descrizione+" ("+codice+")";
			return retValue;
		}

		public string GetCheckBoxLabel(string codice,int chiave,string flag)
		{
			string lbl="<input type='checkbox' class='testo_grigio'  name='chk' value='' >";
			try 
			{
				lbl="<input type='checkbox' class='testo_grigio' id='chk"+chiave+"' name='chk"+chiave.ToString()+"' value='"+codice+"' "+flag+" >";
				return lbl;

			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
				return lbl;
			}
				
				
		}
		private string getDecrizioneCorr(DocsPAWA.DocsPaWR.Corrispondente myCorr) 
		{
			string link_ut = "";
			if (myCorr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo)) 
			{
				DocsPaWR.Ruolo corrRuolo = (DocsPAWA.DocsPaWR.Ruolo) myCorr;
				//inserisco il link per visualizzare gli utenti del ruolo
				//	link_ut = "<a href='rubrica.aspx?codice="+corrRuolo.codiceRubrica + "&TypeQ=C&wnd="+wnd+"&target="+target+"'><img src='../images/info.gif' border='0'/></a>";
				
			}
			//			if (myCorr.GetType() == typeof(DocsPAWA.DocsPaWR.Utente)) 
			//			{
			//				DocsPaWR.Utente corrUtente = (DocsPAWA.DocsPaWR.Utente) myCorr;
			//				//inserisco il link per visualizzare gli utenti del ruolo
			//				//	link_ut = "<a href='rubrica.aspx?codice="+corrRuolo.codiceRubrica + "&TypeQ=C&wnd="+wnd+"&target="+target+"'><img src='../images/info.gif' border='0'/></a>";
			//				
			//			} 
			return link_ut + UserManager.getDecrizioneCorrispondente(this,myCorr);
		}
		private void printMsg(string msg)
		{
			this.Label5.Text=msg;
			this.Label5.Visible=true;
		}
		private string setCorrispondentiTrasmissione() 
		{
			string esito="";
			try
			{
				//gestione trasmissione
				//controlla i corrispondenti selezionati, li mette nella trasmisione e poi carica la pagina con i dettagli

				DocsPaWR.Trasmissione trasmissione = TrasmManager.getGestioneTrasmissione(this);
				//creo l'oggetto qca in caso di trasmissioni a UO
				DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
				qco.fineValidita = true;
				DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = setQCA(qco);
		
				for(int i=0;i<this.DataGrid1.Items.Count;i++) 
				{ 
					//	Label AppoChk=(Label)this.DataGrid1.Items[i].Cells[0].Controls[1];
					if(chk.Contains((((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text)))
					{
						string codiceR=((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text; 
						corr = (DocsPAWA.DocsPaWR.Corrispondente)listCorrHash[codiceR];
						if (corr.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
						{
							// se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
							DocsPaWR.Ruolo[] listaRuoli = UserManager.getRuoliRiferimentoAutorizzati(this,qca,(DocsPAWA.DocsPaWR.UnitaOrganizzativa) corr);
							if (listaRuoli!=null && listaRuoli.Length>0)
							{
								for (int index=0; index < listaRuoli.Length; index++)
									trasmissione = addTrasmissioneSingola(trasmissione, (DocsPAWA.DocsPaWR.Ruolo) listaRuoli[index]);
							}
							else
							{
								if (esito.Equals("")) 
									esito += "Ruoli di riferimento non trovati o non autorizzati nella: ";
								esito += "\\nUO: " + corr.descrizione;
							}
						}
						else
						trasmissione = addTrasmissioneSingola(trasmissione, corr);
					}
				}
			
				TrasmManager.setGestioneTrasmissione(this, trasmissione);
				
//				if ((string)Session["OggettoDellaTrasm"]=="DOC")
//				{
//					Response.Write("<script>window.parent.opener.top.principale.iFrame_dx.document.trasmDatiTrasm_dx.submit();</script>");
//				}
//				else
//				{
//					Response.Write("<script>window.parent.opener.top.principale.iFrame_dx.document.trasmDatiFascTras_dx.submit();</script>");
//				}
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}	
			return esito;
		}

		private void setCorrispondentiRicerca()
		{
			// Corrispondente per Ricerca
			try
			{
				if(rhd!=null)
				{
					for(int i=0;i<this.DataGrid1.Items.Count;i++)
					{
						if((((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text)==rhd)
							corr = (DocsPAWA.DocsPaWR.Corrispondente)listCorrHash[rhd];
					}
				
				}
				if (wnd.Equals("ric_C") && (target != null && target.Equals("mittInt")))
					UserManager.setCorrispondenteIntSelezionato(this, corr);
				else
				{
					if(target != null && !target.Equals("U"))//se voglio popolare il campo mitt/dest
					{
						UserManager.setCorrispondenteSelezionato(this, corr);
					}
					if(target != null && target.Equals("U"))//se voglio popolare il campo ufficio referente
					{
						UserManager.setCorrispondenteReferenteSelezionato(this, corr);
					}
				}

				if (target == "flt_vis_fasc") 
					Response.Write ("<script>window.parent.opener.document.forms[0].submit();</script>");

				//rimozione variabili dalla sessione
				Session.Remove("rubrica.listaCorr");
				Session.Remove("Rubrica.qco");
				Session.Remove("hashRub");
				//submit del tab protocollo
				Response.Write("<script>try { var k=parent.window.opener.document.forms[0].submit(); } catch (e) {}</script>");
				//chiusura della rubrica
				Response.Write("<script>parent.window.close();</script>");
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}
		}
		protected void Inserimento()
		{
			try
			{
				//ok ho selezionato un solo corr.
				Session["rubrica.errMultiSel"]="N";
				if(rhd!=null)
				{
					for(int i=0;i<this.DataGrid1.Items.Count;i++)
					{
						if((((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text)==rhd)
						{
							corr = (DocsPAWA.DocsPaWR.Corrispondente)listCorrHash[rhd];
							corr.tipoCorrispondente = (((Label)this.DataGrid1.Items[i].Cells[3].Controls[1]).Text);
						}
					}
					UserManager.setParentCorr(this,corr);
					//	Session.Remove("rubrica.listaCorr");
					//	Session.Remove("Rubrica.qco");
					Response.Write("<script>window.open(\"InsDest.aspx?target="+target+"\",\"new\",\"width=450,height=580,toolbar=no,directories=no,menubar=no, scrollbars=no\");</script>");
				}
				else
					if(this.chk!=null && this.chk.Count==1)
				{
					for(int i=0;i<this.DataGrid1.Items.Count;i++) 
					{ 

						//Label AppoChk =(Label)this.DataGrid1.Items[i].Cells[0].Controls[1];
						if(chk.Contains((((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text))) 
						{
							string codiceR=((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text; 
							corr = (DocsPAWA.DocsPaWR.Corrispondente)listCorrHash[codiceR];
							corr.tipoCorrispondente = (((Label)this.DataGrid1.Items[i].Cells[3].Controls[1]).Text);
						}
					}
				
					UserManager.setParentCorr(this,corr);
					//	Session.Remove("rubrica.listaCorr");
					//	Session.Remove("Rubrica.qco");
					Response.Write("<script>window.open(\"InsDest.aspx?target="+target+"\",\"new\",\"width=450,height=580,toolbar=no,directories=no,menubar=no, scrollbars=no\");</script>");
					
				}
				else
					if( chk!=null && chk.Count>0)
				{
					//errore selezionato più di un corr.
					Session["rubrica.errMultiSel"]="T";
					Response.Write("<script>alert(\"Selezionare un solo Corrispondente !\")</script>");
					//	Session.Remove("rubrica.listaCorr");
					//	Session.Remove("Rubrica.qco");
					//Response.Write("<script>window.open(\"InsDest.aspx\",\"new\",\"width=450,height=580,toolbar=no,directories=no,menubar=no, scrollbars=no\");</script>");
					Response.Write("<script>window.open(\"InsDest.aspx?target="+target+"\",\"new\",\"width=450,height=580,toolbar=no,directories=no,menubar=no, scrollbars=no\");</script>");
				}
				else
				{
					UserManager.setParentCorr(this,null);
					//	Session.Remove("rubrica.listaCorr");
					//	Session.Remove("Rubrica.qco");
					Response.Write("<script>window.open(\"InsDest.aspx?target="+target+"\",\"new\",\"width=450,height=580,toolbar=no,directories=no,menubar=no, scrollbars=no\");</script>");
					
				}
			
				
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}
		
		}
		protected DocsPAWA.dataSet.DataSetRubrica caricaDataTableContenitore()
		{
			
			for(int i=0;i<listaCorr.Length;i++)

			{
				string tipoIE=listaCorr[i].tipoIE;
				string tipo=CheckTipoItem(listaCorr[i]);
				this.dataSetRubrica1.contenitore.AddcontenitoreRow(i,tipo,getDecrizioneCorr(listaCorr[i]),listaCorr[i].codiceRubrica,"0",this.listaCorr[i].systemId,tipoIE);
			}
			 
			return this.dataSetRubrica1;
		}
		protected string CheckTipoItem(DocsPAWA.DocsPaWR.Corrispondente corr)
		{
			string rtn="";
			if(corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)))
				rtn="U.O.";
			else if((corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Ruolo))))
				rtn="RUOLO";
			else {	 rtn="UTENTE";	}
			return rtn;
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

//this.dataSetRubrica1 = new DocsPAWA.dataSet.DataSetRubrica();
//((System.ComponentModel.ISupportInitialize)(this.dataSetRubrica1)).BeginInit();
//this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
//this.DataGrid1.PreRender += new System.EventHandler(this.Datagrid1_PreRender);
//this.DataGrid1.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.DataGrid1_SortCommand);
//this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
//// 
//// dataSetRubrica1
//// 
//this.dataSetRubrica1.DataSetName = "DataSetRubrica";
//this.dataSetRubrica1.Locale = new System.Globalization.CultureInfo("en-US");
//this.Load += new System.EventHandler(this.Page_Load);
//((System.ComponentModel.ISupportInitialize)(this.dataSetRubrica1)).EndInit();

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.dataSetRubrica1 = new DocsPAWA.dataSet.DataSetRubrica();
			((System.ComponentModel.ISupportInitialize)(this.dataSetRubrica1)).BeginInit();
			this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
			this.DataGrid1.PreRender += new System.EventHandler(this.Datagrid1_PreRender);
			this.DataGrid1.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.DataGrid1_SortCommand);
			this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
			// 
			// dataSetRubrica1
			// 
			this.dataSetRubrica1.DataSetName = "DataSetRubrica";
			this.dataSetRubrica1.Locale = new System.Globalization.CultureInfo("en-US");
			this.Load += new System.EventHandler(this.Page_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataSetRubrica1)).EndInit();

		}
		#endregion

		

		protected void CtrlCheck(string target,string wnd)
		{
			string esito="";
			try 
			{
				if (wnd!=null && (wnd.Equals("proto") || wnd.Equals("protoInt"))) 	
				{
					setCorrispondentiProtocollo();
					GestioneChiusura(esito);
					//					Page.RegisterClientScriptBlock("chiudi","<script>window.parent.opener.document.forms[0].submit();</script>");
				}
				else 
					if (wnd!=null && (wnd.Equals("ric_E") ||wnd.Equals("ric_C") || wnd.Equals("ric_CT") || wnd.Equals("ric_Trasm")))
				{
					setCorrispondentiRicerca();
					GestioneChiusura(esito);
					Page.RegisterClientScriptBlock("chiudi","<script>window.parent.opener.document.forms[0].submit();</script>");
				}
				else
					if (wnd!=null && wnd.Equals("trasm"))
				{
					esito = setCorrispondentiTrasmissione();
					GestioneChiusura(esito);
					if ((string)Session["OggettoDellaTrasm"]=="DOC")
					{
						//Page.RegisterClientScriptBlock("chiudi","<script>window.parent.opener.top.principale.iFrame_dx.document.trasmDatiTrasm_dx.submit();</script>");
						Page.RegisterStartupScript("chiudi","<script>window.parent.opener.top.principale.iFrame_dx.document.trasmDatiTrasm_dx.submit();</script>");
					}
					else 
					{
						Page.RegisterStartupScript("chiudi","<script>window.parent.opener.top.principale.iFrame_dx.document.trasmDatiFascTras_dx.submit();</script>");
					}
				}
				else
					if (wnd!=null && wnd.Equals("fascLF")) 	
				{
					setCorrispondentiFascLF();
					GestioneChiusura(esito);
					//					Page.RegisterClientScriptBlock("chiudi","<script>window.parent.opener.document.forms[0].submit();</script>");
				}
				else
				{
					if (wnd!=null && wnd.StartsWith("fascUffRef")) 
					{
						// Questo è necessario perché alcune pagine prendono
						// i dati direttamente dalla Sessione...
						listaCorr = (DocsPAWA.DocsPaWR.Corrispondente[]) Session["rubrica.listaCorr"];
						DocsPaWR.Corrispondente myCorr = listaCorr[0];
						if(rhd != null)
						{
							for(int i=0; i < listaCorr.Length; i++)
							{
								if(((DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i]).codiceRubrica == rhd) 
								{
									myCorr = (DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i];
									break;
								}
							}
						
						}								
						Session["rubrica.listaCorr"] = new DocsPAWA.DocsPaWR.Corrispondente[1] {myCorr};
						// ****************************************

						setCorrispondentiFascUffReferente();
						GestioneChiusura(esito);
					}
					else
						if (wnd == "FiltriFascLF" || wnd == "fascUffRef") 	
						{
							// MR 20051220
							listaCorr = (DocsPAWA.DocsPaWR.Corrispondente[]) Session["rubrica.listaCorr"];
							DocsPaWR.Corrispondente myCorr = listaCorr[0];
							if(rhd != null)
							{
								for(int i=0; i < listaCorr.Length; i++)
								{
									if(((DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i]).codiceRubrica == rhd) 
									{
										myCorr = (DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i];
										break;
									}
								}
						
							}
								
							Session["rubrica.listaCorr"] = new DocsPAWA.DocsPaWR.Corrispondente[1] {myCorr};
							Response.Write("<script>parent.window.close();</script>");
						}
				}

			}	   
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
			
		}
		private void GestioneChiusura(string esito)
		{
			try
			{
				string alert="";
				if (!esito.Equals(""))
				{
					esito = esito.Replace("'", "''");
					alert = "alert('" + esito + "'); ";
					Page.RegisterClientScriptBlock("chiudi","<script>" + alert + "</script>");
					esito = "";
				}
				
//				if(!Page.IsClientScriptBlockRegistered("chiudi"))
//					Page.RegisterClientScriptBlock("chiudi","<script>" + alert + "</script>");
				//Response.Write("<script language='javascript'>top.frames[1].close();</script> ");		
				/* 08/02/2005 
				 * DataGrid1.Dispose();
				DataGrid1.DataBind();*/
				Response.Write("<script>parent.window.document.IFrame_info.location='../blank_page.htm';</script>");
				
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		
		}

//elisa 28/09/2005
		private void setCorrispondentiFascLF()
		{
			if(rhd!=null)
			{
				DocsPaWR.Fascicolo fascLF = FascicoliManager.getFascicoloSelezionato(this);
				
					for(int i=0;i<this.DataGrid1.Items.Count;i++)
					{
						if((((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text)==rhd)
							corr = (DocsPAWA.DocsPaWR.Corrispondente)listCorrHash[rhd];
					}
				
				if(fascLF!=null)
				{
					if (wnd.Equals("fascLF") && target!= null && target.Equals("U"))	
					{
					
					DocsPaVO.LocazioneFisica.LocazioneFisica LF = new DocsPaVO.LocazioneFisica.LocazioneFisica();
					FascicoliManager.DO_SetFlagLF();
					LF.UO_ID=corr.systemId;
					LF.CodiceRubrica=corr.codiceRubrica;
					LF.Descrizione=corr.descrizione;


					FascicoliManager.DO_SetLocazioneFisica(LF);
					}
				}
				else
				{
				
					//se vengo da ricFasc
					if(wnd.Equals("fascLF"))
					{


						DocsPaVO.LocazioneFisica.LocazioneFisica LF = new DocsPaVO.LocazioneFisica.LocazioneFisica();
						FascicoliManager.DO_SetFlagLF();
						LF.UO_ID=corr.systemId;
						LF.CodiceRubrica=corr.codiceRubrica;
						LF.Descrizione=corr.descrizione;

						FascicoliManager.DO_SetLocazioneFisica(LF);



//						DocsPaWR.UnitaOrganizzativa _uo = ((DocsPAWA.DocsPaWR.Ruolo)UserManager.getRuolo(this)).uo;
//
//						DocsPaVO.LocazioneFisica.LocazioneFisica _lf = new DocsPaVO.LocazioneFisica.LocazioneFisica();
//						_lf.CodiceRubrica = _uo.codiceRubrica;
//						_lf.UO_ID = _uo.systemId;
//						_lf.Descrizione = _uo.descrizione;
//						_lf.Data = (string)DateTime.Now.ToShortDateString();
//
//						FascicoliManager.DO_SetLocazioneFisica(_lf);
//						FascicoliManager.DO_SetFlagLF();
					}
					
				}
				Response.Write("<script>try { var k=window.parent.opener.document.forms[0].submit(); } catch (e) {}</script>");
				Response.Write("<script>parent.window.close();</script>");
			}
			
		}
		private void setCorrispondentiFascUffReferente()
		{
			if(rhd!=null)
			{
				DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato(this);
				
				for(int i=0;i<this.DataGrid1.Items.Count;i++)
				{
					if((((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text)==rhd)
						corr = (DocsPAWA.DocsPaWR.Corrispondente)listCorrHash[rhd];
				}
				
				if(fasc!=null)
				{
					//se ho cliccato la rubrica per popolare l'ufficio referente
					if (wnd.Equals("fascUffRef") && target!=null && target.Equals("U"))	
					{
						if(corr!=null)
						{
							fasc.ufficioReferente = corr;
						}
						FascicoliManager.setFascicoloSelezionato(this,fasc);
						FascicoliManager.DO_SetFlagUR();
					}
					if (wnd.Equals("fascUffRefMod") && target!=null && target.Equals("U"))	
					{
						if(corr!=null)
						{
							FascicoliManager.setUoReferenteSelezionato(this.Page,corr);
							FascicoliManager.DO_SetFlagUR();
						}
					}
				}
				else
				{
					FascicoliManager.setUoReferenteSelezionato(this.Page,corr);
					FascicoliManager.DO_SetFlagUR();

				}
				Response.Write("<script>try { var k=window.parent.opener.document.forms[0].submit(); } catch (e) {}</script>");
				Response.Write("<script>parent.window.close();</script>");
			}
			
		}

		private void setCorrispondentiProtocollo() 
		{
			try
			{
				if(target==null)
					return;
				//DocsPaWR.SchedaDocumento schedaDoc = (DocsPAWA.DocsPaWR.SchedaDocumento)Session["docProtocollo.schedaDocumento"];
/*massimo digregorio 2005-06-01 
old: DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoInLavorazione(this);
*new:*/	 DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = (DocsPAWA.DocsPaWR.SchedaDocumento) Session["tabDoc.schedaDocumento"];

				//controlla i corrispondenti selezionati, li mette in una hashTable e poi carica la pagina 
				if(target.Equals("dest")) 
				{
					// Protocollo in uscita
					for(int i=0;i<this.DataGrid1.Items.Count;i++) 
					{ 
						//Label AppoChk =(Label)this.DataGrid1.Items[i].Cells[0].Controls[1];
						if(chk.Contains((((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text))) 
						{
							string codiceR=((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text; 
							corr = (DocsPAWA.DocsPaWR.Corrispondente)listCorrHash[codiceR];
							schedaDoc = addDestinatari(schedaDoc, corr);
							//Logger.log("Corrispondente Proto= "+corr.descrizione);
						}
					}
						
					DocumentManager.setDocumentoInLavorazione(this,schedaDoc);
					//submit del tab protocollo
					Response.Write("<script>try {var k=window.parent.opener.document.forms[0].submit(); } catch (e) {}</script>");
					
					#region commento
					/* modifica rubrica 08/02/2005 
					Session.Remove("rubrica.listaCorr");
					Session.Remove("Rubrica.qco");
					Session.Remove("hashRub");
					Response.Write("<script>window.opener.document.form[0].submit();</script>");
					Response.Write("<script>window.open('../documento/docProtocollo.aspx','IframeTabs');</script>");
					Page.RegisterClientScriptBlock("Chiudi","<script>parent.window.close();</script>");*/
					#endregion
						
				} 
				else if (target.Equals("mitt") || target.Equals("mittInt") || target.Equals("mittP") || target.Equals("U")) 
				{
					// Protocollo in entrata
					if(rhd!=null)
					{
							
						for(int i=0;i<this.DataGrid1.Items.Count;i++)
						{
							if((((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text)==rhd)
								corr = (DocsPAWA.DocsPaWR.Corrispondente)listCorrHash[rhd];

						}

						//se ho cliccato la rubrica per popolare l'ufficio referente
						if (wnd.Equals("proto") && target.Equals("U"))	
						{
							if (schedaDoc.tipoProto == "A")
							{
								((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).ufficioReferente = corr;
							}
							if (schedaDoc.tipoProto == "P")
							{
								((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).ufficioReferente = corr;
							}							
							if (schedaDoc.tipoProto == "I")
							{
								((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).ufficioReferente = corr;
							}
						}
						else
						{
							if(target.Equals("mitt") || target.Equals("mittP"))
							{	
								
								if (schedaDoc.tipoProto == "A")
								{
									((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente = corr;
									((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).daAggiornareMittente = true;
								}
								if (schedaDoc.tipoProto == "P")
								{
									((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).mittente = corr;
									//((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).daAggiornareMittente = true;
									if(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
										&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"))
									{
										//se il corrispondente è un ruolo allora metto nell'ufficio referente la sua UO
										if(corr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
										{
											((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.Ruolo)corr).uo;
										}
										if(corr.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
										{
											((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).ufficioReferente = corr;
										}
										if(corr.GetType() == typeof(DocsPAWA.DocsPaWR.Utente))
										{
											
											if (((DocsPAWA.DocsPaWR.Utente)corr).ruoli.Length == 1)
											{
												((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.Utente)corr).ruoli[0].uo;
											}
											else
											{
												((DocsPAWA.DocsPaWR.Utente)corr).ruoli = UserManager.getRuoliFiltrati(((DocsPAWA.DocsPaWR.Utente)corr).ruoli);
												Response.Write("<script>alert('l\\'utente "  +corr.descrizione + " appartiene a più UO\\n selezionare quella di interesse');</script>");	
												Response.Write("<script>var win = window.open(\"scegliUoUtente.aspx?win=rubrica&rubr="+corr.descrizione+"\",\"new\",\"width=550,height=280,toolbar=no,directories=no,menubar=no, scrollbars=no\"); var newLeft=(screen.availWidth-590); var newTop=(screen.availHeight-540); win.moveTo(newLeft,newTop)</script>");
												
											}
										}
									}
								}
								if (schedaDoc.tipoProto == "I")
								{
									((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).mittente = corr;
									//((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).daAggiornareMittente = true;
									if(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
										&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"))
									{
										
											//se il corrispondente è un ruolo allora metto nell'ufficio referente la sua UO
											if(corr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
											{
												((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.Ruolo)corr).uo;
											}
											if(corr.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
											{
												((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).ufficioReferente = corr;
											}
											if(corr.GetType() == typeof(DocsPAWA.DocsPaWR.Utente))
											{
												((DocsPAWA.DocsPaWR.Utente)corr).ruoli = UserManager.getRuoliFiltrati(((DocsPAWA.DocsPaWR.Utente)corr).ruoli);
												if (((DocsPAWA.DocsPaWR.Utente)corr).ruoli.Length == 1)
												{
													((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).ufficioReferente = ((DocsPAWA.DocsPaWR.Utente)corr).ruoli[0].uo;
												}
												else
												{
												
													Response.Write("<script>alert('l\\'utente "  +corr.descrizione + " appartiene a più UO\\n selezionare quella di interesse');</script>");	
													Response.Write("<script>var win = window.open(\"scegliUoUtente.aspx?win=rubrica&rubr="+corr.descrizione+"\",\"new\",\"width=550,height=350,toolbar=no,directories=no,menubar=no, scrollbars=no\"); var newLeft=(screen.availWidth-590); var newTop=(screen.availHeight-540); win.moveTo(newLeft,newTop)</script>");
													//Response.Write("<script>rtnValue=window.showModalDialog(\"scegliUoUtente.aspx?win=rubrica&rubr="+corr.descrizione+"\",\"new\",'dialogWidth:615px;dialogHeight:380px;status:no;resizable:no;scroll:no;dialogLeft:100;dialogTop:100;center:no;help:no;');if(rtnValue){window.close();}</script>");
													
												}
											}
										
									}
								}
							}
							else 
							{
								((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenteIntermedio = corr;
								((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).daAggiornareMittenteIntermedio = true;
							}
						}
						
						DocumentManager.setDocumentoInLavorazione(this,schedaDoc);
						
						//rimozione variabili dalla sessione
						Session.Remove("rubrica.listaCorr");
						Session.Remove("Rubrica.qco");
						Session.Remove("hashRub");
//						//submit del tab protocollo
//						Response.Write("<script>var k=window.parent.opener.document.forms[0].submit(); </script>");
//						//chiusura della popup rubricaDT.aspx
						if(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
							&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"))
						{
							if(corr!=null)
							{
								if ((corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente))) && (((DocsPAWA.DocsPaWR.Utente)corr).ruoli != null && ((DocsPAWA.DocsPaWR.Utente)corr).ruoli.Length > 1) && !schedaDoc.tipoProto.Equals("A"))
								{
								}
								else
								{
									//chiudo e faccio submit del tab protocollo
									Response.Write("<script>parent.window.close();</script>");
									Response.Write("<script>try { var k=window.parent.opener.document.forms[0].submit(); } catch (e) {}</script>");
								}
							}
							
						}
						else
						{
							Response.Write("<script>parent.window.close();</script>");
							//submit del tab protocollo
							Response.Write("<script>try { var k=window.parent.opener.document.forms[0].submit(); } catch (e) {}</script>");
												
						}
					}
				}
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}
		}

//		private DocsPAWA.DocsPaWR.Ruolo[] getRuoliFiltrati(DocsPAWA.DocsPaWR.Ruolo[] ruoli)
//		{
//			string l_oldSystemId="";								
//			System.Object[] l_objects=new System.Object[0];
//			
//			foreach(DocsPAWA.DocsPaWR.Ruolo ruolo in ruoli)
//			{
//				string t_systemId = ruolo.uo.systemId;						
//				if (t_systemId!=l_oldSystemId)
//				{
//					l_objects=Utils.addToArray(l_objects, ruolo); 	
//					l_oldSystemId=t_systemId;
//				}
//			}
//			
//			DocsPaWR.Ruolo[] ruoliFiltrati=new DocsPAWA.DocsPaWR.Ruolo[l_objects.Length];	
//			l_objects.CopyTo(ruoliFiltrati,0);
//
//			return ruoliFiltrati;
//		}

		private DocsPAWA.DocsPaWR.SchedaDocumento addDestinatari(DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc, DocsPAWA.DocsPaWR.Corrispondente corr) 
		{
			//controlo se esiste già il corrispondente selezionato
			DocsPaWR.Corrispondente[] listaDest;

			listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari;
			
			if (UserManager.esisteCorrispondente(listaDest,corr))
				return schedaDoc;
			if (UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza,corr))
				return schedaDoc;

			//aggiungo il corrispondente
			//di default lo aggiungo tra i destinatari principali
			if (schedaDoc.protocollo != null)
			{
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari = UserManager.addCorrispondente(listaDest, corr);
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).daAggiornareDestinatari = true;
			}
			return schedaDoc;
		}

		private void Datagrid1_PreRender(object sender, System.EventArgs e)
		{
			try
			{//DataGrid dg=((DataGrid) sender);
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
						//checkbox memory
						if(chk!=null && chk.Count>0)
						{

							if(chk.Contains(((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text))
							{
								if(this.DataGrid1.Items[i].Cells[0].Controls[1].GetType().Equals(typeof(Label)))
								{
									Label lbc=((Label)this.DataGrid1.Items[i].Cells[0].Controls[1]);
									lbc.Text=this.GetCheckBoxLabel(((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text,Int32.Parse(((Label)this.DataGrid1.Items[i].Cells[2].Controls[1]).Text),"checked");
								}
							}
						
						}
						/// radiobutton memory
						if(Request.Form["dt"]!=null && !Request.Form["dt"].Equals(""))
						{
							if((((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text.Equals(Request.Form["dt"])))
							{
								if(this.DataGrid1.Items[i].Cells[0].Controls[2].GetType().Equals(typeof(Label)))
								{
									Label lb=((Label)this.DataGrid1.Items[i].Cells[1].Controls[1]);
									lb.Text="<input type='radio' name='dt' checked='true'class='testo_grigio' value='"+Request.Form["dt"]+"' >";
								}
							}
						
						}
						((ImageButton)this.DataGrid1.Items[i].Cells[7].Controls[1]).Attributes.Add("onclick","window.document.datagridRubrica.hiddenField.value='exp';");
						ArrayList lst=(ArrayList)Session["listaExp"];
						for(int h=0;h<lst.Count;h++)
						{
							if(((Label)this.DataGrid1.Items[i].Cells[10].Controls[1]).Text==lst[h].ToString())
							{
								this.DataGrid1.Items[i].Cells[7].Controls[1].Visible=false;
								this.DataGrid1.Items[i].Cells[7].Controls[3].Visible=true;
							}
						}
						if(((Label)this.DataGrid1.Items[i].Cells[3].Controls[1]).Text=="UTENTE")
						{
							this.DataGrid1.Items[i].Cells[7].Controls[1].Visible=false;
							this.DataGrid1.Items[i].Cells[7].Controls[3].Visible=true;
						}
						//controllo aggiunto per le UO visualizzate nelle trasmissioni. Non devono espandersi.
						//controllo aggiunto anche pei i RUOLI nel caso di ricerche di trasmissioni
						if (wnd == null || wnd.Equals(""))
							wnd=Request.QueryString["wnd"];
						if (wnd.Equals("trasm") || wnd.Equals("ric_Trasm") || (wnd.Equals("proto") && target!= null && target.Equals("U") || wnd.StartsWith("fasc") && target!= null && target.Equals("U")))
						{
							string label = ((Label)this.DataGrid1.Items[i].Cells[3].Controls[1]).Text;
							if(label.Equals("U.O.") || (label.Equals("RUOLO") && wnd.Equals("ric_Trasm")))
							{
								this.DataGrid1.Items[i].Cells[7].Controls[1].Visible=false;
								this.DataGrid1.Items[i].Cells[7].Controls[3].Visible=true;
							}
						}
					}
				}
				if(this.DataGrid1.Items.Count==1)
				{
				
					if(Request.QueryString["target"].Equals("dest") || Request.QueryString["target"].Equals("trasm"))
					{
						Label lbc=((Label)this.DataGrid1.Items[0].Cells[0].Controls[1]);
						lbc.Text=this.GetCheckBoxLabel(((Label)this.DataGrid1.Items[0].Cells[5].Controls[1]).Text,Int32.Parse(((Label)this.DataGrid1.Items[0].Cells[2].Controls[1]).Text),"checked");
					}
					if(Request.QueryString["target"].Equals("mitt") || Request.QueryString["target"].Equals("mittP"))
					{
						string cod_corr=((Label)this.DataGrid1.Items[0].Cells[5].Controls[1]).Text;
						Label lb=((Label)this.DataGrid1.Items[0].Cells[1].Controls[1]);
						lb.Text="<input type='radio' name='dt' checked='true'class='testo_grigio' value='"+cod_corr+"' >";
					}
				}
				
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}


		private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			
			string codRub=((Label)e.Item.Cells[5].Controls[1]).Text;
			//string key=((Label)e.Item.Cells[2].Controls[1]).Text;
			if(e.CommandName=="exp")
			{
				queryFigli_Exec(codRub,wnd,e.Item.ItemIndex,e.CommandName,e.Item.ItemIndex);
				DataGrid1.SelectedIndex=e.Item.ItemIndex;
				
			}
			if(e.CommandName=="clp")
			{
				DocsPAWA.dataSet.DataSetRubrica dtcont=(DocsPAWA.dataSet.DataSetRubrica)Session["rubrica_dt"];
				string numrig=((Label)this.DataGrid1.Items[e.Item.ItemIndex].Cells[2].Controls[1]).Text;
				DataRow[] drt=dtcont.contenitore.Select("idcont='"+codRub+"'");
				
				for(int i=0;i<drt.Length;i++)
				{
					this.DataGrid1.Items[i+1+Int32.Parse(numrig)].Visible=false;
				}
				
			}
		}
	
		private void queryFigli_Exec(string codiceRubrica, string wnd,int index,string tipoRic,int itemindex) 
		{
			try
			{
				//negli altri casi vengono visualizzate UO, ruoli e/o utenti - viene specificato il codice rubrica 
				if(Session["Rubrica.qco"]!=null)
					this.qco=(DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente)Session["Rubrica.qco"];
				this.qco.codiceRubrica=codiceRubrica;
				this.qco.getChildren=true;
				this.listaCorrExp=searchCorr(qco,wnd,tipoRic);

				if(this.listaCorrExp.Length!=0)
				{
					DocsPAWA.dataSet.DataSetRubrica dtcont=(DocsPAWA.dataSet.DataSetRubrica)Session["rubrica_dt"];
					DataRow[] drw=dtcont.contenitore.Select("idcont='"+codiceRubrica+"'");
					int h=dtcont.contenitore.Rows.Count;

					for(int i=0;i<listaCorrExp.Length;i++)
					{
						//			if(!listaCorrExp[i].codiceRubrica.Equals(codiceRubrica))
						//			{
				
						DocsPAWA.dataSet.DataSetRubrica.contenitoreRow dr=dtcont.contenitore.NewcontenitoreRow();
					
						//(i,tipo,listaCorr[i].descrizione,listaCorr[i].codiceRubrica)
						//.contenitore.AddcontenitoreRow(i,tipo,getdecrizioneCorr(listaCorr[i]),listaCorr[i].codiceRubrica,0,this.listaCorr[i].systemId,"F");
						dr[0]=i+h;
						dr[1]=this.CheckTipoItem(listaCorrExp[i]);
						string descrPadre=((Label)this.DataGrid1.Items[itemindex].Cells[4].Controls[1]).Text;
						dr[2]=descrPadre+"&nbsp;>&nbsp;"+listaCorrExp[i].descrizione;
						dr[3]=listaCorrExp[i].codiceRubrica;
						dr[4]=codiceRubrica;
						dr[5]=listaCorrExp[i].systemId;
						dtcont.contenitore.Rows.InsertAt(dr,index+i+1);
						//			}
						if(!listCorrHash.ContainsKey(listaCorrExp[i].codiceRubrica))
							listCorrHash.Add(listaCorrExp[i].codiceRubrica,listaCorrExp[i]);

						Session["hashRub"]=listCorrHash;
					}
					dv=dtcont.Tables[0].DefaultView;
					dv.Sort="Descrizione ASC,Tipo DESC";
					this.DataGrid1.DataSource=dv;
					Session["rubrica_dt"]=dtcont;
					
					this.DataGrid1.DataBind();
					listaExp=(ArrayList)Session["listaExp"];
					this.listaExp.Add(((Label)this.DataGrid1.Items[itemindex].Cells[10].Controls[1]).Text);
					Session["listaExp"]=listaExp;
			
					//				//Session["rubrica.listaCorr"] = listaCorr;
					//			}
					//			else
					//			{
					////				for(int i=0;i<drw.Length;i++)
					////				{
					////					this.DataGrid1.Items[index+i+1].Visible=true;
					////				}
					//			}
				}
			}
			catch (System.Exception ex)
			{
				ErrorManager.redirect(this, ex);
			}
		}
		private DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr) 
		{
			
			if (trasmissione.trasmissioniSingole != null)
			{
				// controllo se esiste la trasmissione singola associata a corrispondente selezionato
				for(int i = 0; i < trasmissione.trasmissioniSingole.Length; i++) 
				{
					DocsPaWR.TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
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
			trasmissioneSingola.tipoTrasm = "S";
			trasmissioneSingola.corrispondenteInterno = corr;
			trasmissioneSingola.ragione = TrasmManager.getRagioneSel(this);
			
			// Aggiungo la lista di trasmissioniUtente
			if( corr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo)) 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
				DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr.codiceRubrica);
				if (listaUtenti == null || listaUtenti.Length == 0)
					return trasmissione;
				//ciclo per utenti se dest è gruppo o ruolo
				for(int i= 0; i < listaUtenti.Length; i++) 
				{
					DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
					trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente) listaUtenti[i];
					if(TrasmManager.getRagioneSel(this).descrizione.Equals("RISPOSTA"))
						trasmissioneUtente.idTrasmRispSing=trasmissioneSingola.systemId;
					trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
				}
			}
			else 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
				DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
				trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente) corr;
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
			
			DocsPaWR.Corrispondente[] l_corrispondenti=UserManager.getListaCorrispondenti(this.Page,qco);
			
			return pf_getCorrispondentiFiltrati(l_corrispondenti);
	
		}

		private DocsPAWA.DocsPaWR.Corrispondente[] searchCorr(DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente qco, string wnd,string tipoRic) 
		{
			DocsPaWR.Corrispondente[] corrSearch=new DocsPAWA.DocsPaWR.Corrispondente[0];
			target=Request.QueryString["target"];
			if(wnd.Equals("protoInt"))
			{
				corrSearch=UserManager.getListaCorrispondentiAutProtoInt(this.Page, qco);
			}
			else
			{
				if ((wnd != null && wnd.StartsWith("fasc")) || (wnd != null && wnd.Equals("proto") 
					|| wnd != null && wnd.ToLower()=="filtrifasclf"
					|| wnd.Equals("protoS") || wnd == "ric_E" || wnd == "ric_C" 
					|| wnd == "ric_CT" || wnd.Equals("ric_Trasm")) 
					|| !(qco.codiceRubrica == null || qco.codiceRubrica.Equals("")) )
					if(!tipoRic.Equals("exp"))
					{
						if(firstime.Equals("Y"))
						{
							corrSearch=null;
							if(ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_UO_ROOT)!=null 
								&& ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_UO_ROOT).Equals("1"))
							{
								corrSearch=UserManager.getListaRootUO(this, qco);
							}
							else
							{
								corrSearch=new DocsPAWA.DocsPaWR.Corrispondente[0];
							}
						}
						else
						{
							corrSearch=null;
							corrSearch=UserManager.getListaCorrispondenti(this.Page, qco);
						}
					}
					else
					{
						corrSearch=null;
						corrSearch=UserManager.getListaCorrispondenti(this.Page, qco);
					}

				else 
				{
					// C'è bisogno di ragione della trasmissione e dell'id del documento!!!
					if(firstime.Equals("N"))
					{
						DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qcAut = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
						qcAut = setQCA(qco);
						//qcAut.idRegistro = qco.idRegistri[0];
						corrSearch=null;
						corrSearch=UserManager.getListaCorrispondentiAutorizzati(this, qcAut); 									
					}
				}
			}
			return pf_getCorrispondentiFiltrati(corrSearch);
		}

		private DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato setQCA(DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente qco)
		{
			DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qcAut = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
			
			if ((string)Session["OggettoDellaTrasm"]=="DOC")
			{ 
				qcAut.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
				DocsPaWR.SchedaDocumento docCorr=DocumentManager.getDocumentoSelezionato(this);
				if(docCorr!=null)
					if(docCorr.protocollo!=null)
						if(docCorr.protocollo.numero!=null)
							if(!docCorr.protocollo.numero.Equals(""))
								qcAut.idRegistro = docCorr.registro.systemId; 
								qcAut.isProtoInterno=true;
			}	
			//Aggiunto il caso di trasmissione di un fascicolo
			if ((string)Session["OggettoDellaTrasm"]=="FASC")
			{
				qcAut.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;			
				qcAut.idNodoTitolario = FascicoliManager.getFascicoloSelezionato(this).idClassificazione;
				qcAut.idRegistro = FascicoliManager.getFascicoloSelezionato(this).idRegistroNodoTit;
				if (qcAut.idRegistro != null && qcAut.idRegistro.Equals(""))
					qcAut.idRegistro = null;
			}
			//cerco la ragione in base all'id che ho nella querystring
			qcAut.ragione = TrasmManager.getRagioneSel(this);
			if (TrasmManager.getGestioneTrasmissione(this) != null)
			{
				qcAut.ruolo = TrasmManager.getGestioneTrasmissione(this).ruolo;
			}
			qcAut.queryCorrispondente = qco;	
			return qcAut;
		}
		private DocsPAWA.DocsPaWR.Corrispondente[] pf_getCorrispondentiFiltrati(DocsPAWA.DocsPaWR.Corrispondente[] corrispondenti)
		{
			string l_oldSystemId="";								
			System.Object[] l_objects=new System.Object[0];
			System.Object[] l_objects_ruoli = new System.Object[0];
			DocsPaWR.Ruolo[] lruolo = new DocsPAWA.DocsPaWR.Ruolo[0];
			int i = 0;
			foreach(DocsPAWA.DocsPaWR.Corrispondente t_corrispondente in corrispondenti)
			{
				string t_systemId=t_corrispondente.systemId;						
				if (t_systemId!=l_oldSystemId)
				{
					l_objects=Utils.addToArray(l_objects,t_corrispondente); 	
					l_oldSystemId=t_systemId;
					i = i + 1 ;
					continue;
				}
				else
				{
					/* il corrispondente non viene aggiunto, in quanto sarebbe un duplicato 
					 * ma viene aggiunto solamente il ruolo */
					
					if(t_corrispondente.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
					{
						if ((l_objects[i - 1]).GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
						{
							l_objects_ruoli = ((Utils.addToArray(((DocsPAWA.DocsPaWR.Utente)(l_objects[i -1])).ruoli, ((DocsPAWA.DocsPaWR.Utente)t_corrispondente).ruoli[0])));			
							DocsPaWR.Ruolo[] l_ruolo=new DocsPAWA.DocsPaWR.Ruolo[l_objects_ruoli.Length];
						    ((DocsPAWA.DocsPaWR.Utente)(l_objects[i -1])).ruoli = l_ruolo;
							l_objects_ruoli.CopyTo(((DocsPAWA.DocsPaWR.Utente)(l_objects[i -1])).ruoli, 0);
						}


//						{
//						if ((l_objects[i - 1]).GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
//						{
//							
//							l_objects_ruoli = ((Utils.addToArray((((DocsPAWA.DocsPaWR.Utente)t_corrispondente).ruoli), ((DocsPAWA.DocsPaWR.Utente)(l_objects[i - 1])).ruoli[0])));
//							DocsPaWR.Ruolo[] l_ruolo=new DocsPAWA.DocsPaWR.Ruolo[l_objects_ruoli.Length];
//							((DocsPAWA.DocsPaWR.Utente)corrispondenti[i]).ruoli = l_ruolo;
//							l_objects_ruoli.CopyTo(((DocsPAWA.DocsPaWR.Utente)corrispondenti[i]).ruoli, 0);
//						}
						
					}
				}
				
			}
			
			DocsPaWR.Corrispondente[] l_corrSearch=new DocsPAWA.DocsPaWR.Corrispondente[l_objects.Length];	
			l_objects.CopyTo(l_corrSearch,0);

			return l_corrSearch;
		}
		private void DataGrid1_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			
			DataTable dt=((DocsPAWA.dataSet.DataSetRubrica) Session["rubrica_dt"]).Tables[0];
			dv=dt.DefaultView;
			dv.Sort = e.SortExpression.ToString()+" ASC";

			this.DataGrid1.DataSource=dv;
			this.DataGrid1.DataBind();
		}

		private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string tipo=((Label)this.DataGrid1.SelectedItem.Cells[3].Controls[1]).Text;
			string tipoIE=((Label)this.DataGrid1.SelectedItem.Cells[11].Controls[1]).Text;
			string  codCorr=((Label)this.DataGrid1.SelectedItem.Cells[5].Controls[1]).Text;
			if(tipo.Equals("UTENTE") && tipoIE.Equals("I"))
			{
				Response.Write("<script>parent.window.document.IFrame_info.location='InfoRuoliCorr.aspx?codCor="+codCorr+"';</script>");
			}
			else
			{
				Response.Write("<script>parent.window.document.IFrame_info.location='InfoRubrica.aspx?codCor="+codCorr+"';</script>");
			}
			
		}

	}
}
