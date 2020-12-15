using System;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for rubricaDT.
	/// </summary>
    public class rubricaDT : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.RadioButtonList rbl_tipoCorr;
		protected System.Web.UI.WebControls.Label lblUO;
		protected System.Web.UI.WebControls.DropDownList DropDownList1;
		protected System.Web.UI.WebControls.TextBox TextUO;
		protected System.Web.UI.WebControls.Label lblRuolo;
		protected System.Web.UI.WebControls.DropDownList DropDownList2;
		protected System.Web.UI.WebControls.TextBox TextRuolo;
		protected System.Web.UI.WebControls.Label lblUtente;
		protected System.Web.UI.WebControls.DropDownList DropDownList3;
		protected System.Web.UI.WebControls.TextBox TextUtente;
		protected System.Web.UI.WebControls.TextBox TextBox1;
		protected System.Web.UI.WebControls.Button btn_Ok;
		protected System.Web.UI.WebControls.Button btn_Chiudi;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.HtmlControls.HtmlInputHidden h_corrispondenti;
	
		//my var
		
		protected System.Web.UI.WebControls.RadioButtonList rbl_corrispondente;
		protected TableRow tr;
		protected TableCell tc;
		private DocsPAWA.DocsPaWR.Corrispondente[] listaCorr;
		protected string wnd;
		protected string oggTrasm;
		protected string target;
		protected RadioButtonList rb_Corr;
		protected System.Web.UI.WebControls.Label Label;
		protected System.Web.UI.WebControls.Label lbl_tipoCorr;
		protected System.Web.UI.WebControls.ImageButton btn_ricerca;
		protected System.Web.UI.WebControls.Label lbl_message;
		protected DocsPaWebCtrlLibrary.IFrameWebControl IFrame_dt;
		protected CheckBoxList cbl_Corr;
		protected DocsPaWebCtrlLibrary.IFrameWebControl IFrame_info;
		protected System.Web.UI.HtmlControls.HtmlForm rubrica;
		protected string index;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label lbl_ricerca;
		protected System.Web.UI.WebControls.Label lbl_inserimento;
		protected System.Web.UI.WebControls.Panel pnl_ins;
		protected System.Web.UI.WebControls.Panel pnl_dettaglio;
		protected System.Web.UI.HtmlControls.HtmlGenericControl tipoCorrispondenteAreaReal;
		protected System.Web.UI.HtmlControls.HtmlGenericControl tipoCorrispondenteAreaFake;
		//protected System.Web.UI.WebControls.Label id_lbl_dettCorr;
		protected string btnOK;
	
		private void Page_Load(object sender, System.EventArgs e) 
		{
			try 
			{
				//caso in cui la rubrica viene aperta dal protocollo in ingresso e uscita
				if(Request.QueryString["interno"] == null)
				{
					if(Request.QueryString["target"] != null)
					{
						if (!((string)Request.QueryString["target"]).Equals("mittP"))
						{
							this.tipoCorrispondenteAreaReal.Visible = true;
							this.tipoCorrispondenteAreaFake.Visible = false;
							pnl_ins.Visible=true;
							//rubrica relativa al filtro mittente intermedio nella ricarca completa
							if (((string)Request.QueryString["target"]).Equals("mittInt"))
							{
								pnl_ins.Visible=false;
							}	
						} 
						//rubrica relativa al mittente del protocollo in uscita
						else 
						{
							this.tipoCorrispondenteAreaReal.Visible = false;
							this.tipoCorrispondenteAreaFake.Visible = true;
							// setta cmq l'oggetto radio button invisibile a INTERNO
							this.rbl_tipoCorr.SelectedItem.Value = "I";
							pnl_ins.Visible=false;
						}
					} 
					else 
					{
						this.tipoCorrispondenteAreaReal.Visible = true;
						this.tipoCorrispondenteAreaFake.Visible = false;
						pnl_ins.Visible=false;
					}
				}
				//caso in cui la rubrica viene aperta dal protocollo interno
				else
				{
					/* Poiche' sono stati utilizzati i webControl per i quali non ho alcune 
					 * proprieta' di visualizzazione, simulo la visualizzazione dei checkBox HTML
					 * nascondendo gli originali e creandone altri che comunque non sono utilizzati.
					 * In futuro sarebbe opportuno risolvere meglio questa situazione.
					*/
					this.tipoCorrispondenteAreaReal.Visible = false;
					this.tipoCorrispondenteAreaFake.Visible = true;
					// setta cmq l'oggetto radio button invisibile a INTERNO
					this.rbl_tipoCorr.SelectedItem.Value = "I";
					pnl_ins.Visible=false;
				}				

				// chi mi chiama
				wnd = Request.QueryString["wnd"];
				this.Session["typeRequest"]=wnd;

				target = Request.QueryString["target"];
				if(Request.QueryString["clear"] !=null && Request.QueryString["clear"]=="1")
				{
					FascicoliManager.removeFascicoloSelezionato(this);
				}

				/*Disabilita l'inserimento di un nuovo corrispondente in rubrica
				 * ad eccezione del fatto che la rubrica sia richiamata dal tab protocollo e
				 * il non si è nel caso di protocollazione interna */
				
				if(((wnd.Equals("fascLF") || wnd.ToLower()== "filtrifasclf") && (target!=null && target.Equals("U"))) || (wnd.StartsWith("fascUffRef") && (target!=null && target.Equals("U"))) || wnd.Equals("trasm") || (wnd.Equals("ric_C") && (target!=null && target.Equals("U")) || (wnd.Equals("proto") && (target!=null && target.Equals("U")))))	
				{
					pnl_ins.Visible=false;
				}
				// se wnd=proto: corrispondente (mitt, mittInt o dest)
				
				
				setVisibleFields(wnd);

				if(!IsPostBack)
				{
					queryExec(!IsPostBack);
				}
					
				listaCorr = (DocsPAWA.DocsPaWR.Corrispondente[]) Session["rubrica.listaCorr"];
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}


		private void setVisibleFields(string wnd)
		{
			System.Web.UI.WebControls.TextBox field;
			field = this.TextUO;
			//focus
			if (wnd!=null) 
			{
				if ((wnd == "proto" && target != null && !target.Equals("U")) || wnd == "ric_E" || (wnd == "ric_C" && target!= null && !target.Equals("U")) || wnd == "ric_CT" ) 
				{
					this.rbl_tipoCorr.Visible = true; 
					this.lbl_tipoCorr.Visible = true;
				} 
				else if (wnd == "ric_Trasm" || (wnd.Equals("proto") && target != null && target.Equals("U")) || 
					(wnd.Equals("fascLF") && target != null && target.Equals("U")) || (wnd.StartsWith("fascUffRef") && target!=null && (target.Equals("U"))) || (wnd == "ric_C"&& target!= null && target.Equals("U"))
					|| wnd.ToLower()=="filtrifasclf") 
				{
					//disabilito tutto
					//UO
					this.TextUO.Enabled = false;
					this.DropDownList1.Enabled = false;
					this.DropDownList1.Visible = false;
					this.TextUO.Visible = false;
					this.lblUO.Visible = false;
					//RUOLO
					this.TextRuolo.Enabled = false;
					this.DropDownList2.Enabled = false;
					this.DropDownList2.Visible = false;
					this.TextRuolo.Visible = false;
					this.lblRuolo.Visible = false;
					//UTENTE
					this.TextUtente.Enabled = false;
					this.DropDownList3.Enabled = false;
					this.DropDownList3.Visible = false;
					this.TextUtente.Visible = false;
					this.lblUtente.Visible = false;

					switch (target)
					{
						case "U":
							this.TextUO.Enabled = true;
							this.DropDownList1.Enabled = true;
							this.DropDownList1.Visible = true;
							this.TextUO.Visible = true;
							this.lblUO.Visible = true;
							break;
						case "R":
							this.TextRuolo.Enabled = true;
							this.DropDownList2.Enabled = true;
							this.DropDownList2.Visible = true;
							this.TextRuolo.Visible = true;
							this.lblRuolo.Visible = true;
							field = this.TextRuolo;
							break;
						case "P":
							this.TextUtente.Enabled = true;
							this.DropDownList3.Enabled = true;
							this.DropDownList3.Visible = true;
							this.TextUtente.Visible = true;
							this.lblUtente.Visible = true;
							field = TextUtente;
							break;
					}					
				}	
				if (!Page.IsPostBack)
				{
//					string s = "<SCRIPT language='javascript'>document.getElementById('" + field.ID + "').focus() </SCRIPT>";
//					RegisterStartupScript("focus", s);
				}
			}
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
			this.DropDownList1.SelectedIndexChanged += new System.EventHandler(this.DropDownList1_SelectedIndexChanged);
			this.btn_Chiudi.Click += new System.EventHandler(this.btn_Chiudi_Click);
			this.btn_ricerca.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ricerca_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.rubricaDT_PreRender);

		}
		#endregion

		private void queryExec(bool flag) 
		{
			try
			{
				string codice = Request.QueryString["codice"];
				string typeQuery = Request.QueryString["typeQ"];
				string wnd = Request.QueryString["wnd"];

				if(flag)
				{
					this.queryC_Exec("",wnd,"Y");					
				}
				else
				{
					this.queryC_Exec("",wnd,"N");					
				}
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}	

		}
		
		protected string checkFields() 
		{
			string msg = "";

			if (this.TextUO.Text.Equals("") && this.TextRuolo.Text.Equals("") && this.TextUtente.Text.Equals("")) 
			{
				msg = "Inserire almeno un criterio di ricerca";
			}
			else if (!this.TextUO.Text.Equals("") && this.DropDownList1.SelectedItem.Value.Equals("D") && this.TextUO.Text.Length<2) 
			{
				msg = "Il campo descrizione UO deve avere almeno 2 caratteri";
			}
			else if (!this.TextRuolo.Text.Equals("") && this.DropDownList2.SelectedItem.Value.Equals("D") && this.TextRuolo.Text.Length<2) 
			{
				msg = "Il campo descrizione Ruolo deve avere almeno 2 caratteri";
			}
			return msg;
		}
		


		private void queryC_Exec(string codiceRubrica, string wnd,string firstime) 
		{
			
			try
			{
				Session.Remove("Rubrica.qco");
			
				//costruzione oggetto queryCorrispondente
				DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
				
				if (codiceRubrica.Equals("")) 
				{
					if(!this.TextUO.Text.Equals("")) 
					{
						if(this.DropDownList1.SelectedItem.Value.Equals("C")) 
							qco.codiceUO=this.TextUO.Text;
						else 
							qco.descrizioneUO=this.TextUO.Text;
					}
					if(!this.TextRuolo.Text.Equals("")) 
					{
						qco.descrizioneRuolo=this.TextRuolo.Text;
					}
					if(!this.TextUtente.Text.Equals("")) 
					{
						if(this.DropDownList3.SelectedItem.Value.Equals("N"))
							qco.nomeUtente=this.TextUtente.Text;
						else 
							qco.cognomeUtente=this.TextUtente.Text;
					}
				} 
				else 
				{
					qco.codiceRubrica = codiceRubrica;
					qco.getChildren = true;
				}

				qco.idAmministrazione=UserManager.getInfoUtente(this).idAmministrazione;// ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];
				qco.idRegistri = UserManager.getListaIdRegistri(this); //rappresenta il registro selezionato dall'utente e non quello del documento				

				if (wnd!=null)
				{
					if (wnd == "protoInt" || wnd == "proto" || wnd == "ric_E" || wnd == "ric_C" || wnd == "ric_CT" || 
						wnd == "fascLF" || wnd.Equals("fascUffRef") || wnd.ToLower()=="filtrifasclf") 
					{
						if (target!= null && target.Equals("U"))
						{
							//se sto inserendo l'ufficio referente
							qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.INTERNO;
						}
						else
						{
							if (this.rbl_tipoCorr.SelectedItem.Value.Equals("I"))
								qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.INTERNO;
							else
								if (this.rbl_tipoCorr.SelectedItem.Value.Equals("E"))
								qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.ESTERNO;
							else
								qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.GLOBALE;
						}
					}
				
					//per i corrispondenti disabilitati da non considerare nel caso di creazione di un doc e di una trasmissione
					if(wnd == "proto" || wnd == "trasm" || wnd == "protoInt")
					{
						qco.fineValidita = true;
					}
				}
				else 
					//	if(btnOK!=null && btnOK!="false")
				{  
					//corrispondenti interni
					qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.INTERNO;			
				}

				Session["Rubrica.qco"]=qco;
				this.IFrame_dt.NavigateTo="DatagridRubrica.aspx?wnd="+wnd+"&target="+target+"&firstime="+firstime;
				this.lbl_message.Visible = false;
				
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
				
		}
		public string _wnd
		{
			get
			{
				return this.wnd;
			}
			set
			{
				_wnd=value;
			}
		}
		public string _oggTrasm
		{
			get
			{
				return ((string) Session["OggettoDellaTrasm"]);
			}
			set
			{
				_oggTrasm=value;
			}
		}
		public string _target
		{
			get
			{
				return this.target;
			}
			set
			{
				_target=value;
			}
		}
		public string _firstime
		{
			get
			{
				return "Y";
			}
			set
			{
				_firstime=value;
			}
		}
		private void queryD_Exec(string systemID, string wnd) 
		{
		}


		private void btn_Chiudi_Click(object sender, System.EventArgs e) 
		{
			try 
			{			
				Session.Remove("rubrica.listaCorr");
				Session.Remove("hashRub");
				Session.Remove("Rubrica.qco");
				Session.Remove("listaExp");
				Response.Write("<script>window.close();</script>");
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}

		}

		private void btn_ricerca_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			try
			{
				Session.Remove("rubrica.listaCorr");
				Session.Remove("Rubrica.qco");
				Session.Remove("hashRub");

				string msg = checkFields();
				if (msg.Equals("")) 
				{
					Session.Remove("rubrica.listaCorr");
					queryC_Exec("",wnd,"N");
				} 
				else 
				{
					Response.Write("<script>alert('" + msg + "');</script>");	
				}
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}

		}

		private void DropDownList1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			
		}

		
			

			
		

		private void btn_Ok_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (wnd!=null && (wnd.Equals("proto") || wnd.Equals("protoS"))) 
				{
					//	Response.Write("<script>window.open('../documento/docProtocollo.aspx','IframeTabs');</script>");
					Response.Write("<script language='javascript'>var k=window.opener.document.forms[0].submit();</script>");
				}
				else 
					if (wnd!=null && (wnd.Equals("ric_E") ||wnd.Equals("ric_C") || wnd.Equals("ric_CT") || wnd.Equals("ric_Trasm") ))
					Response.Write("<script language='javascript'>var k=window.opener.document.forms[0].submit(); </script>");

				else
					if (wnd!=null && wnd.Equals("trasm"))
				{
					if(Session["OggettoDellaTrasm"]!=null && Session["OggettoDellaTrasm"].ToString().Equals("DOC"))	
						Response.Write("<script language='javascript'>var k=window.opener.top.principale.iFrame_dx.document.trasmDatiTrasm_dx.submit();</script>");
					else
						Response.Write("<script language='javascript'>var k=window.opener.top.principale.iFrame_dx.document.trasmDatiFascTras_dx.submit();</script>");
				}		
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}

				
		}

		private void btn_InsDest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{}


		private void rubricaDT_PreRender(object sender, System.EventArgs e)
		{
			//this.btn_InsDest.Attributes.Add("onClick","ApriInserimentoDest();");
		}

		
			


	}
}
