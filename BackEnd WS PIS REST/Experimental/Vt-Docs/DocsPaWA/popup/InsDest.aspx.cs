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

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for Inserimento_Destinatari.
	/// </summary>
    public class InsDest : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label Label1;
		protected DocsPAWA.DocsPaWR.Corrispondente newCorr;
		protected DocsPAWA.DocsPaWR.Corrispondente currCorr;
		protected System.Web.UI.WebControls.TextBox txt_codCorr;
		protected System.Web.UI.WebControls.TextBox txt_codRub;
		protected System.Web.UI.WebControls.Button btn_Insert;
		protected System.Web.UI.WebControls.DropDownList ddl_tipoCorr;
		protected System.Web.UI.WebControls.TextBox txt_descr;
		protected System.Web.UI.WebControls.Label lbl_desc_nome;
		protected System.Web.UI.WebControls.TextBox txt_cognome;
		protected System.Web.UI.WebControls.Label lbl_cognome;
		protected System.Web.UI.WebControls.Label lbl_email;
		protected System.Web.UI.WebControls.TextBox txt_email;
		protected System.Web.UI.WebControls.TextBox txt_indirizzo;
		protected System.Web.UI.WebControls.TextBox txt_cap;
		protected System.Web.UI.WebControls.TextBox txt_citta;
		protected System.Web.UI.WebControls.TextBox txt_provincia;
		protected System.Web.UI.WebControls.TextBox txt_nazione;
		protected System.Web.UI.WebControls.TextBox TextBox6;
		protected System.Web.UI.WebControls.TextBox txt_telefono2;
		protected System.Web.UI.WebControls.TextBox txt_codfisc;
		protected System.Web.UI.WebControls.TextBox txt_telefono1;
        protected System.Web.UI.WebControls.TextBox txt_local;
		protected System.Web.UI.WebControls.Label lbl_indirizzo;
		protected System.Web.UI.WebControls.Label lbl_cap;
		protected System.Web.UI.WebControls.Label lbl_citta;
		protected System.Web.UI.WebControls.Label lbl_provincia;
		protected System.Web.UI.WebControls.Label lbl_nazione;
		protected System.Web.UI.WebControls.Label lbl_telefono2;
		protected System.Web.UI.WebControls.Label lbl_codfisc;
		protected System.Web.UI.WebControls.Label lbl_telefono1;
		protected System.Web.UI.WebControls.TextBox txt_fax;
		protected System.Web.UI.WebControls.Label lbl_fax;
		protected System.Web.UI.WebControls.Label lbl_note;
        protected System.Web.UI.WebControls.Label lbl_local;
		protected System.Web.UI.WebControls.TextBox txt_note;
		protected System.Web.UI.WebControls.Label lbl_canpref;
		protected System.Web.UI.WebControls.DropDownList dd_canpref;
		protected System.Web.UI.WebControls.Label lbl_parent;
		protected System.Web.UI.WebControls.Label lbl_parent_desc;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.TextBox txt_codAmm;
		protected System.Web.UI.WebControls.TextBox txt_codAOO;
		protected DocsPAWA.DocsPaWR.Corrispondente parentCorr;
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				//errore: l'utente ha selezionato più di un corr.
				if(Session["rubrica.errMultiSel"]!=null && Session["rubrica.errMultiSel"].ToString().Equals("T"))
				{				
					Response.Write("<script language='javascript'>window.close();</script>");
					return;
				}
						
				string tipo;
				ListItem item;			

				// è stato selezionato un item nel datagrid rubrica
				if(UserManager.getParentCorr(this)!=null)
				{
					DocsPaWR.Corrispondente corr=UserManager.getParentCorr(this);

					this.lbl_parent.Text="Selezione: ";
					this.lbl_parent_desc.Text=corr.descrizione;

					tipo = corr.tipoCorrispondente;
					switch(tipo)
					{
						case "U.O.":	// se provengo da una UO posso inserire solo un altra UO o un ruolo						
							item = new ListItem("U.O.", "U");
							ddl_tipoCorr.Items.Add(item);
							item = new ListItem("RUOLO", "R");
							ddl_tipoCorr.Items.Add(item);
							this.lbl_desc_nome.Text="Descrizione *";
							this.txt_cognome.Visible=false;
							this.lbl_cognome.Visible=false;
							this.txt_email.Visible=true;
							this.lbl_email.Visible=true;
							//parte dettagli
							this.lbl_indirizzo.Visible=true;
							this.lbl_cap.Visible=true;
							this.lbl_citta.Visible=true;
                            this.lbl_local.Visible = true;
							this.lbl_provincia.Visible=true;
							this.lbl_nazione.Visible=true;
							this.lbl_telefono1.Visible=true;
							this.lbl_telefono2.Visible=true;
							this.lbl_fax.Visible=true;
							this.lbl_codfisc.Visible=true;
							this.lbl_note.Visible=true;
							this.txt_indirizzo.Visible=true;
							this.txt_cap.Visible=true;
							this.txt_citta.Visible=true;
                            this.txt_local.Visible = true;
							this.txt_provincia.Visible=true;
							this.txt_nazione.Visible=true;
							this.txt_telefono1.Visible=true;
							this.txt_telefono2.Visible=true;
							this.txt_fax.Visible=true;
							this.txt_codfisc.Visible=true;
							this.txt_note.Visible=true;
							LoadCanali();
							break;

						case "RUOLO":	// se provengo da un ruolo posso inserire solo un utente
							//item = new ListItem("RUOLO", "R");
							//ddl_tipoCorr.Items.Add(item);
							item = new ListItem("UTENTE", "P");
							ddl_tipoCorr.Items.Add(item);
							this.lbl_desc_nome.Text="Nome";
							this.txt_cognome.Visible=true;
							this.lbl_cognome.Visible=true;
							this.txt_email.Visible=true;
							this.lbl_email.Visible=true;
							//parte dettagli
							this.lbl_indirizzo.Visible=true;
							this.lbl_cap.Visible=true;
							this.lbl_citta.Visible=true;
							this.lbl_provincia.Visible=true;
							this.lbl_nazione.Visible=true;
							this.lbl_telefono1.Visible=true;
							this.lbl_telefono2.Visible=true;
							this.lbl_fax.Visible=true;
							this.lbl_codfisc.Visible=true;
							this.lbl_note.Visible=true;
							this.txt_indirizzo.Visible=true;
							this.txt_cap.Visible=true;
							this.txt_citta.Visible=true;
							this.txt_provincia.Visible=true;
							this.txt_nazione.Visible=true;
							this.txt_telefono1.Visible=true;
							this.txt_telefono2.Visible=true;
							this.txt_fax.Visible=true;
							this.txt_codfisc.Visible=true;
							this.txt_note.Visible=true;
							LoadCanali();
							break;
						default:
							Response.Write("<script language='javascript'>window.close();</script>");
							return;						
					}					
				}
				else
				{
					// non avendo selezionato nulla posso inserire solo UO o utenti...
					item = new ListItem("U.O.", "U");
					ddl_tipoCorr.Items.Add(item);
					item = new ListItem("UTENTE", "P");
					ddl_tipoCorr.Items.Add(item);
					string tipoC = "";
					tipoC = (string)Session["TipoInsDest"];
					if(tipoC != "" && tipoC != null)
					{
						ddl_tipoCorr.SelectedIndex = ddl_tipoCorr.Items.IndexOf(ddl_tipoCorr.Items.FindByValue(tipoC));	
					}
					switch(ddl_tipoCorr.SelectedItem.Value)
					{
						case "P":
						{
							this.lbl_desc_nome.Text="Nome";
							this.txt_cognome.Visible=true;
							this.lbl_cognome.Visible=true;
							this.txt_email.Visible=true;
							this.lbl_email.Visible=true;
							//parte dettagli
							this.lbl_indirizzo.Visible=true;
							this.lbl_cap.Visible=true;
							this.lbl_citta.Visible=true;
                            this.lbl_local.Visible = true;
							this.lbl_provincia.Visible=true;
							this.lbl_nazione.Visible=true;
							this.lbl_telefono1.Visible=true;
							this.lbl_telefono2.Visible=true;
							this.lbl_fax.Visible=true;
							this.lbl_codfisc.Visible=true;
							this.lbl_note.Visible=true;
							this.txt_indirizzo.Visible=true;
							this.txt_cap.Visible=true;
							this.txt_citta.Visible=true;
                            this.txt_local.Visible = true;
							this.txt_provincia.Visible=true;
							this.txt_nazione.Visible=true;
							this.txt_telefono1.Visible=true;
							this.txt_telefono2.Visible=true;
							this.txt_fax.Visible=true;
							this.txt_codfisc.Visible=true;
							this.txt_note.Visible=true;
							break;
						}
						case "U":
						{
							this.lbl_desc_nome.Text="Descrizione *";
							this.txt_cognome.Visible=false;
							this.lbl_cognome.Visible=false;
							this.txt_email.Visible=true;
							this.lbl_email.Visible=true;
							//parte dettagli
							this.lbl_indirizzo.Visible=true;
							this.lbl_cap.Visible=true;
							this.lbl_citta.Visible=true;
                            this.lbl_local.Visible = true;
							this.lbl_provincia.Visible=true;
							this.lbl_nazione.Visible=true;
							this.lbl_telefono1.Visible=true;
							this.lbl_telefono2.Visible=true;
							this.lbl_fax.Visible=true;
							this.lbl_codfisc.Visible=true;
							this.lbl_note.Visible=true;
							this.txt_indirizzo.Visible=true;
							this.txt_cap.Visible=true;
							this.txt_citta.Visible=true;
                            this.txt_local.Visible = true;
							this.txt_provincia.Visible=true;
							this.txt_nazione.Visible=true;
							this.txt_telefono1.Visible=true;
							this.txt_telefono2.Visible=true;
							this.txt_fax.Visible=true;
							this.txt_codfisc.Visible=true;
							this.txt_note.Visible=true;
							break;
						}
					}

					LoadCanali();
				}					
			
				DocsPaWR.Corrispondente newCorr=new DocsPAWA.DocsPaWR.Corrispondente();
			}	

			// Impostazione maxlenght campi UI
			this.SetMaxLenght();
		}

		/// <summary>
		/// Carica i canali preferenziali nella combo-box
		/// </summary>
		private void LoadCanali()
		{
			// combo-box dei canali preferenziali
			DocsPaWR.Canale[] canali=UserManager.getListaCanali(this);
			if(canali != null && canali.Length > 0)
			{
				dd_canpref.Items.Clear();
				for(int i=0;i<canali.Length;i++)
				{
					ListItem item=new ListItem(canali[i].descrizione,canali[i].systemId);
					this.dd_canpref.Items.Add(item);
				}
				dd_canpref.SelectedIndex = dd_canpref.Items.IndexOf(dd_canpref.Items.FindByText("MAIL"));		
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
			this.ddl_tipoCorr.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoCorr_SelectedIndexChanged);
			this.btn_Insert.Click += new System.EventHandler(this.btn_Insert_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void after_InsCorr(string codiceRubrica,DocsPaWR.Corrispondente tipoUT,string wnd,string firstime,string target) 
		{
			
			try
			{
			
				Session.Remove("Rubrica.qco");
				DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
				qco.codiceRubrica = codiceRubrica;
				//qco.getChildren = true;
		
				qco.idAmministrazione=UserManager.getInfoUtente(this).idAmministrazione;// ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];
				qco.idRegistri = UserManager.getListaIdRegistri(this); //rappresenta il registro selezionato dall'utente e non quello del documento				
				qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.ESTERNO;
				qco.fineValidita = true;

				Session["Rubrica.qco"]=qco;
				Response.Write("<script language='javascript'>window.opener.parent.parent.frames[0].location.href='DatagridRubrica.aspx?wnd="+wnd+"&target="+target+"&firstime="+firstime+"';</script>");
					
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
				
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Insert_Click(object sender, System.EventArgs e)
		{
		
			try
			{
				bool tuttoOk=true;
				this.newCorr=new DocsPAWA.DocsPaWR.Corrispondente();
				this.parentCorr=UserManager.getParentCorr(this);
				DocsPaWR.Corrispondente insCorr=new DocsPAWA.DocsPaWR.Corrispondente();
			    
				//creo l'oggetto canale
				DocsPaWR.Canale canale = new DocsPAWA.DocsPaWR.Canale();
				canale.systemId = this.dd_canpref.SelectedItem.Value;

				//inserisco campi
				switch(this.ddl_tipoCorr.SelectedItem.Value)
				{
					case "U":
					{
						if(this.parentCorr==null || (this.parentCorr!=null && ((DocsPAWA.DocsPaWR.UnitaOrganizzativa)parentCorr).tipoIE.Equals("E")))
						{
							if(this.txt_descr.Text!=null && !this.txt_descr.Text.Equals(""))
							{
								if((this.txt_telefono1==null || this.txt_telefono1.Text.Equals("")) && !(this.txt_telefono2==null || this.txt_telefono2.Text.Equals("")))
								{
									Response.Write("<script language='javascript'>window.alert(\"Attenzione, inserire il campo telefono princ.\");</script>");
									tuttoOk=false;
								}
								else if(this.txt_cap!=null && !this.txt_cap.Text.Equals("") &&this.txt_cap.Text.Length!=5){
									Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo CAP deve essere di 5 cifre\");</script>");
									tuttoOk=false;
								}
								else if(this.txt_cap!=null && !this.txt_cap.Text.Equals("") && !Utils.isNumeric(this.txt_cap.Text))
								{
									Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo CAP deve un numero\");</script>");
									tuttoOk=false;
								}
								else if(this.txt_provincia!=null && !this.txt_provincia.Text.Equals("") && this.txt_provincia.Text.Length!=2)
								{
									Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo provincia deve essere di 2 cifre\");</script>");
									tuttoOk=false;
								}
								else if(this.txt_provincia!=null && !this.txt_provincia.Text.Equals("") && !Utils.isCorrectProv(this.txt_provincia.Text))
								{
									Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo provincia non è corretto\");</script>");
									tuttoOk=false;
								}
								else
								{
									DocsPaWR.UnitaOrganizzativa new_UO=new DocsPAWA.DocsPaWR.UnitaOrganizzativa();
									new_UO.codiceCorrispondente=this.txt_codCorr.Text;
									new_UO.codiceRubrica=this.txt_codRub.Text;
									new_UO.descrizione=this.txt_descr.Text;
									new_UO.idAmministrazione=UserManager.getInfoUtente(this).idAmministrazione;
									new_UO.idRegistro=UserManager.getRegistroSelezionato(this).systemId;//UserManager.getListaIdRegistriUtente(this)[0];
									new_UO.tipoCorrispondente=this.ddl_tipoCorr.SelectedItem.Value;
									new_UO.email=this.txt_email.Text;
									new_UO.canalePref=canale;
									new_UO.codiceAmm =txt_codAmm.Text;
									new_UO.codiceAOO =txt_codAOO.Text;
                                    if ((this.txt_indirizzo.Text != null && !this.txt_indirizzo.Equals("")) || (this.txt_cap.Text != null && !this.txt_cap.Equals("")) || (this.txt_citta.Text != null && !this.txt_citta.Equals("")) || (this.txt_provincia.Text != null && !this.txt_provincia.Equals("")) || (this.txt_nazione.Text != null && !this.txt_nazione.Equals("")) || (this.txt_telefono1.Text != null && !this.txt_telefono1.Equals("")) || (this.txt_telefono2 != null && !this.txt_telefono2.Equals("")) || (this.txt_fax.Text != null && !this.txt_fax.Equals("")) || (this.txt_codfisc.Text != null && !this.txt_codfisc.Equals("")) || (this.txt_note.Text != null && !this.txt_note.Equals("")) || (this.txt_local.Text != null && !this.txt_local.Equals("")))
									{
										new_UO.info=insertDettagli();
										new_UO.dettagli = true;
									}
									insCorr = UserManager.addressbookInsertCorrispondente(this,new_UO,this.parentCorr);
								}
							}
							else
							{
								Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo descrizione è obbligatorio.\");</script>");
								tuttoOk=false;
							}
						}
						else{
							Response.Write("<script language='javascript'>window.alert(\"Attenzione, l'Unità organizzativa superiore deve essere esterna.\");</script>");
							tuttoOk=false;			
						}
						break;
					}
					case "P":
					{
							if(this.txt_cognome.Text!=null && !this.txt_cognome.Text.Equals(""))
							{
								if((this.txt_telefono1==null || this.txt_telefono1.Text.Equals("")) && !(this.txt_telefono2==null || this.txt_telefono2.Text.Equals("")))
								{
									Response.Write("<script language='javascript'>window.alert(\"Attenzione, inserire il campo telefono princ.\");</script>");
									tuttoOk=false;
								}
								else if(this.txt_cap!=null && !this.txt_cap.Text.Equals("") && this.txt_cap.Text.Length!=5)
								{
									Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo CAP deve essere di 5 cifre\");</script>");
									tuttoOk=false;
								}
								else if(this.txt_cap!=null && !this.txt_cap.Text.Equals("") && !Utils.isNumeric(this.txt_cap.Text))
								{
									Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo CAP deve un numero\");</script>");
									tuttoOk=false;
								}
								else if(this.txt_provincia!=null && !this.txt_provincia.Text.Equals("") && this.txt_provincia.Text.Length!=2)
								{
									Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo provincia deve essere di 2 cifre\");</script>");
									tuttoOk=false;
								}
								else if(this.txt_provincia!=null && !this.txt_provincia.Text.Equals("") && !Utils.isCorrectProv(this.txt_provincia.Text))
								{
									Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo provincia non è corretto\");</script>");
									tuttoOk=false;
								}
								else
								{
									DocsPaWR.Utente new_UT=new DocsPAWA.DocsPaWR.Utente();
									new_UT.codiceCorrispondente=this.txt_codCorr.Text;
									new_UT.codiceRubrica=this.txt_codRub.Text;
									new_UT.cognome=this.txt_cognome.Text;
									new_UT.nome=this.txt_descr.Text;
									new_UT.email=this.txt_email.Text;
									new_UT.descrizione=this.txt_cognome.Text+this.txt_descr.Text;
									new_UT.idAmministrazione=UserManager.getInfoUtente(this).idAmministrazione;
									new_UT.idRegistro=UserManager.getRegistroSelezionato(this).systemId;//UserManager.getListaIdRegistriUtente(this)[0];
									new_UT.tipoCorrispondente=this.ddl_tipoCorr.SelectedItem.Value;
									new_UT.canalePref=canale;
									new_UT.codiceAmm =txt_codAmm.Text;
									new_UT.codiceAOO =txt_codAOO.Text;
                                    if ((this.txt_indirizzo.Text != null && !this.txt_indirizzo.Equals("")) || (this.txt_cap.Text != null && !this.txt_cap.Equals("")) || (this.txt_citta.Text != null && !this.txt_citta.Equals("")) || (this.txt_provincia.Text != null && !this.txt_provincia.Equals("")) || (this.txt_nazione.Text != null && !this.txt_nazione.Equals("")) || (this.txt_telefono1.Text != null && !this.txt_telefono1.Equals("")) || (this.txt_telefono2 != null && !this.txt_telefono2.Equals("")) || (this.txt_fax.Text != null && !this.txt_fax.Equals("")) || (this.txt_codfisc.Text != null && !this.txt_codfisc.Equals("")) || (this.txt_note.Text != null && !this.txt_note.Equals("")) || (this.txt_local.Text != null && !this.txt_local.Equals("")))
									{
										new_UT.info=insertDettagli();
										new_UT.dettagli=true;
									}
									insCorr = UserManager.addressbookInsertCorrispondente(this,new_UT,this.parentCorr);
								}
							}
							else
							{
								Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo cognome è obbligatorio.\");</script>");
								tuttoOk=false;
							}

						if(this.parentCorr!=null && (!this.parentCorr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Ruolo)) || !this.parentCorr.tipoIE.Equals("E")))
						{
							Response.Write("<script language='javascript'>window.alert(\"Attenzione, il parent deve essere un RUOLO esterno.\");</script>");
							tuttoOk=false;	
						}
						break;
					}
					case "R":
					{
						if(this.parentCorr!=null && this.parentCorr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)) && ((DocsPAWA.DocsPaWR.UnitaOrganizzativa)parentCorr).tipoIE.Equals("E"))
						{
							if(this.txt_descr.Text!=null && !this.txt_descr.Text.Equals(""))
							{
								DocsPaWR.Ruolo new_RUO=new DocsPAWA.DocsPaWR.Ruolo();
								new_RUO.codiceCorrispondente=this.txt_codCorr.Text;
								new_RUO.codiceRubrica=this.txt_codRub.Text;
								new_RUO.descrizione=this.txt_descr.Text;
								((DocsPAWA.DocsPaWR.Corrispondente)(new_RUO)).email = this.txt_email.Text;
								new_RUO.idAmministrazione=UserManager.getInfoUtente(this).idAmministrazione;
								new_RUO.idRegistro=UserManager.getRegistroSelezionato(this).systemId;//UserManager.getListaIdRegistriUtente(this)[0];
								new_RUO.tipoCorrispondente=this.ddl_tipoCorr.SelectedItem.Value;
								new_RUO.canalePref=canale;
								new_RUO.codiceAmm =txt_codAmm.Text;
								new_RUO.codiceAOO =txt_codAOO.Text;
								insCorr = UserManager.addressbookInsertCorrispondente(this,new_RUO,this.parentCorr);
				
							}
							else
							{
								Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo descrizione è obbligatorio.\");</script>");
								tuttoOk=false;
							}
						}
						else
						{
							Response.Write("<script language='javascript'>window.alert(\"Attenzione,\\n Per inserire un nuovo RUOLO deve essere selezionata una U.O. esterna nella Rubrica.\\n Ripetere la procedura d'inserimento. \");</script>");
							tuttoOk=false;
						}
						break;

					}
				}
				if(insCorr!=null && insCorr.errore==null  )
				{
					if(tuttoOk)
					{
						Response.Write("<script>window.close();</script>");
						String trg= Request.QueryString["target"];
						after_InsCorr(insCorr.codiceRubrica,insCorr,"proto","N",trg);
						Response.Write("<script>window.opener.parent.parent.frames[1].location='../blank_page.htm';</script>");
					}
					else
					{
						Response.Write("<script>window.close();</script>");
					}
				}
				else
					Response.Write("<script language=\"javascript\">window.alert(\""+insCorr.errore+"\");</script>");

			}
			catch(Exception es)
			{
				ErrorManager.redirectToErrorPage(this,es);
			}
			//Session.Remove("target");
		}

		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ddl_tipoCorr_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Session.Remove("TipoInsDest");
			Session["TipoInsDest"] = ddl_tipoCorr.SelectedItem.Value;
			switch(ddl_tipoCorr.SelectedItem.Value)
			{
				case "P":
				{
					this.lbl_desc_nome.Text="Nome";
					this.txt_descr.Text="";
					this.txt_cognome.Visible=true;
					this.lbl_cognome.Visible=true;
					this.txt_email.Visible=true;
					this.lbl_email.Visible=true;
					//parte dettagli
					this.lbl_indirizzo.Visible=true;
					this.lbl_cap.Visible=true;
					this.lbl_citta.Visible=true;
                    this.lbl_local.Visible = true;
					this.lbl_provincia.Visible=true;
					this.lbl_nazione.Visible=true;
					this.lbl_telefono1.Visible=true;
					this.lbl_telefono2.Visible=true;
					this.lbl_fax.Visible=true;
					this.lbl_codfisc.Visible=true;
					this.lbl_note.Visible=true;
					this.txt_indirizzo.Visible=true;
					this.txt_cap.Visible=true;
					this.txt_citta.Visible=true;
                    this.txt_local.Visible = true;
					this.txt_provincia.Visible=true;
					this.txt_nazione.Visible=true;
					this.txt_telefono1.Visible=true;
					this.txt_telefono2.Visible=true;
                    this.txt_fax.Visible=true;
					this.txt_codfisc.Visible=true;
					this.txt_note.Visible=true;
					break;
				}
				case "R":
				{
					this.lbl_desc_nome.Text="Descrizione *";
					this.txt_cognome.Visible=false;
					this.lbl_cognome.Visible=false;
					this.txt_email.Visible=true;
					this.lbl_email.Visible=true;
					//parte dettagli
					this.lbl_indirizzo.Visible=false;
					this.lbl_cap.Visible=false;
					this.lbl_citta.Visible=false;
					this.lbl_provincia.Visible=false;
					this.lbl_nazione.Visible=false;
					this.lbl_telefono1.Visible=false;
					this.lbl_telefono2.Visible=false;
					this.lbl_fax.Visible=false;
					this.lbl_codfisc.Visible=false;
					this.lbl_note.Visible=false;
					this.txt_indirizzo.Visible=false;
					this.txt_cap.Visible=false;
					this.txt_citta.Visible=false;
					this.txt_provincia.Visible=false;
					this.txt_nazione.Visible=false;
					this.txt_telefono1.Visible=false;
					this.txt_telefono2.Visible=false;
					this.txt_fax.Visible=false;
					this.txt_codfisc.Visible=false;
					this.txt_note.Visible=false;
					break;
				}
				case "U":
				{
					this.lbl_desc_nome.Text="Descrizione *";
					this.txt_cognome.Visible=false;
					this.lbl_cognome.Visible=false;
					this.txt_email.Visible=true;
					this.lbl_email.Visible=true;
					//parte dettagli
					this.lbl_indirizzo.Visible=true;
					this.lbl_cap.Visible=true;
					this.lbl_citta.Visible=true;
                    this.lbl_local.Visible = true;
					this.lbl_provincia.Visible=true;
					this.lbl_nazione.Visible=true;
					this.lbl_telefono1.Visible=true;
					this.lbl_telefono2.Visible=true;
					this.lbl_fax.Visible=true;
					this.lbl_codfisc.Visible=true;
					this.lbl_note.Visible=true;
					this.txt_indirizzo.Visible=true;
					this.txt_cap.Visible=true;
					this.txt_citta.Visible=true;
                    this.txt_local.Visible = true;
					this.txt_provincia.Visible=true;
					this.txt_nazione.Visible=true;
					this.txt_telefono1.Visible=true;
					this.txt_telefono2.Visible=true;
					this.txt_fax.Visible=true;
					this.txt_codfisc.Visible=true;
					this.txt_note.Visible=true;
					break;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private DocsPaVO.addressbook.DettagliCorrispondente insertDettagli()
		{
		    DocsPaVO.addressbook.DettagliCorrispondente dettagli=new DocsPaVO.addressbook.DettagliCorrispondente();
			dettagli.Corrispondente.AddCorrispondenteRow(this.txt_indirizzo.Text,
														 this.txt_citta.Text,
														 this.txt_cap.Text,
														 this.txt_provincia.Text,
														 this.txt_nazione.Text,
														 this.txt_telefono1.Text,
														 this.txt_telefono2.Text,
														 this.txt_fax.Text,
														 this.txt_codfisc.Text,
														 this.txt_note.Text,
                                                         this.txt_local.Text,
                                                         string.Empty,
                                                         string.Empty,
                                                         string.Empty,
                                                         "");

		   return dettagli;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			Session.Remove("TipoInsDest");
			Response.Write("<script language='javascript'>window.close();</script>");
		}

		/// <summary>
		/// Impostazione maxlenght sui campi della UI
		/// </summary>
		private void SetMaxLenght()
		{
			if (ddl_tipoCorr.SelectedValue=="P")
			{
				// Campo visibile solo in questo caso
				this.txt_cognome.MaxLength=50;
				// Per persona fisica è il campo "Nome"
				this.txt_descr.MaxLength=50;
			}
			else
			{
				this.txt_descr.MaxLength=128;
			}
		}	
	}
}
