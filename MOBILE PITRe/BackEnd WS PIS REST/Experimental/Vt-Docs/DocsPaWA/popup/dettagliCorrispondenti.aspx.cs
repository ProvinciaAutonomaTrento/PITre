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
using System.Linq;
using System.Collections.Generic;
namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for dettagliCorrispondenti.
	/// </summary>
	public class dettagliCorrispondenti : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label lbl_nomeCorr;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label lbl_DescIntOp;
		protected System.Web.UI.WebControls.Repeater Repeater1;
		protected System.Web.UI.WebControls.Label lbl_indirizzo;
		protected System.Web.UI.WebControls.TextBox txt_indirizzo;
		protected System.Web.UI.WebControls.Label lbl_cap;
		protected System.Web.UI.WebControls.TextBox txt_cap;
		protected System.Web.UI.WebControls.Label lbl_citta;
		protected System.Web.UI.WebControls.TextBox txt_citta;
        protected System.Web.UI.WebControls.Label lbl_local;
        protected System.Web.UI.WebControls.TextBox txt_local;
        protected System.Web.UI.WebControls.Label lbl_provincia;
		protected System.Web.UI.WebControls.TextBox txt_provincia;
		protected System.Web.UI.WebControls.Label lbl_nazione;
		protected System.Web.UI.WebControls.Label lbl_telefono;
		protected System.Web.UI.WebControls.Label lbl_telefono2;
		protected System.Web.UI.WebControls.TextBox txt_nazione;
		protected System.Web.UI.WebControls.TextBox txt_telefono;
		protected System.Web.UI.WebControls.TextBox txt_telefono2;
		protected System.Web.UI.WebControls.Label lbl_fax;
		protected System.Web.UI.WebControls.TextBox txt_fax;
		protected System.Web.UI.WebControls.Label lbl_codfisc;
		protected System.Web.UI.WebControls.TextBox txt_codfisc;
        protected System.Web.UI.WebControls.Label lbl_partita_iva;
        protected System.Web.UI.WebControls.TextBox txt_partita_iva;
		protected System.Web.UI.WebControls.Label lbl_note;
		protected System.Web.UI.WebControls.Label lbl_email;
		protected System.Web.UI.WebControls.TextBox txt_email;
		protected System.Web.UI.WebControls.Button btn_CreaCorDoc;
		protected System.Web.UI.WebControls.TextBox txt_note;

		protected DocsPAWA.DocsPaWR.Corrispondente corr;
		protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;
		protected string tipoCor;
		protected string indexCor;
		protected bool readOnly;
		protected DocsPaWebCtrlLibrary.ImageButton btn_ModMit;
		protected System.Web.UI.WebControls.TextBox txt_codAmm;
		protected System.Web.UI.WebControls.TextBox txt_codAOO;
		protected System.Web.UI.WebControls.Label lbl_codAOO;
		protected System.Web.UI.WebControls.Label lbl_codAmm;
		protected System.Web.UI.WebControls.Label lbl_codRubr_corr;
		protected System.Web.UI.WebControls.Panel pnl_notcommon;
	
		protected System.Web.UI.WebControls.Panel panel2;
        protected System.Web.UI.WebControls.Panel Panel1;
		protected System.Web.UI.WebControls.Panel common;
		protected bool modificato = false;
        protected System.Web.UI.WebControls.Panel PanelListaCorrispondenti;
        protected System.Web.UI.WebControls.DataGrid dg_listCorr;
        protected System.Web.UI.WebControls.Panel pnl_email;
        
        //multicasella
        protected System.Web.UI.WebControls.GridView gvCaselle;
        protected System.Web.UI.UpdatePanel updPanelMail;
        protected System.Web.UI.UpdatePanel updPanel1;
        protected System.Web.UI.UpdatePanel updPanel2;
        protected System.Web.UI.WebControls.TextBox txtCasella;
        protected System.Web.UI.WebControls.TextBox txtNote;
        protected System.Web.UI.WebControls.ImageButton imgAggiungiCasella;
        protected System.Web.UI.WebControls.RadioButton rdbPrincipale;
        protected System.Web.UI.WebControls.TextBox txtEmailCorr;
        protected System.Web.UI.WebControls.TextBox txtNoteMailCorr;
        protected System.Web.UI.WebControls.Label txtPrincipale;


        private void Page_PreRender(object sender, System.EventArgs e)
        {
            // La creazione del corrispondente occasionale è consentita solo 
            // per i corrispondenti che non provengono da rubrica comune
            this.btn_CreaCorDoc.Enabled = (this.corr != null && !this.corr.inRubricaComune);
        }

		private void Page_Load(object sender, System.EventArgs e)
		{
			//carica_Info
            tipoCor = Request.QueryString["tipoCor"];
			indexCor = Request.QueryString["indexCor"];
            if (indexCor.Contains("________"))
            {
                indexCor = indexCor.Replace("________", "&");
            }
            schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);

			if ((tipoCor == null) || (tipoCor == ""))
				return;
			if (schedaDocumento == null)
				return;

			if(tipoCor.Equals("M"))
			{
				if (schedaDocumento.tipoProto=="A")
				{
					btn_ModMit.Tipologia = "DO_IN_MIT_MODIFICA";
					btn_CreaCorDoc.Visible = true;
					corr = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente;
			
					if(corr==null)
						//da protocollo inserendo solo la descrizione e aprendo subito dopo la maschera di dettagli,
						//la pagina non ha ancora creato il mitt, perchè non c'è stato alcun post back, quindi lo creo io.
					{
						corr=new DocsPAWA.DocsPaWR.Corrispondente();
						corr.tipoCorrispondente="O";
						corr.idAmministrazione=UserManager.getUtente(this).idAmministrazione;
						corr.descrizione=indexCor;
					}
				}
				if (schedaDocumento.tipoProto=="P")
				{
					btn_ModMit.Tipologia = "DO_IN_MIT_MODIFICA";
					btn_CreaCorDoc.Visible = false;
					corr = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).mittente;
				}
				if (schedaDocumento.tipoProto=="I")
				{
					btn_ModMit.Tipologia = "DO_IN_MIT_MODIFICA";
					btn_CreaCorDoc.Visible = false;
					corr = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocumento.protocollo).mittente;
				}
				
			}
			else if (tipoCor.Equals("I"))
			{
				btn_ModMit.Tipologia = "DO_IN_MII_MODIFICA";
				btn_CreaCorDoc.Visible = true;
				corr = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio;
			}
			else if (tipoCor.Equals("D"))
			{
				if (indexCor == null || indexCor.Equals("") || Int32.Parse(indexCor)<0)
					return;
				btn_ModMit.Tipologia = "DO_OUT_DES_MODIFICA";
				corr = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[Int32.Parse(indexCor)];
				if (schedaDocumento.tipoProto=="P")
					btn_CreaCorDoc.Visible = true;
				else
					btn_CreaCorDoc.Visible = false;
			}
			else if (tipoCor.Equals("C"))
			{
				if (indexCor == null || indexCor.Equals("") || Int32.Parse(indexCor)<0)
					return;
				btn_ModMit.Tipologia = "DO_OUT_DES_MODIFICA";
				corr = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[Int32.Parse(indexCor)];
				if (schedaDocumento.tipoProto=="P")
					btn_CreaCorDoc.Visible = true;
				else
					btn_CreaCorDoc.Visible = false;
			}
            else if (tipoCor.Equals("MD"))
            {
                if (schedaDocumento.tipoProto == "A")
                {
                    //btn_ModMit.Tipologia = "DO_IN_MIT_MODIFICA";
                    //btn_CreaCorDoc.Visible = true;
                    corr = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenti[Int32.Parse(indexCor)];

                    if (corr == null)
                    //da protocollo inserendo solo la descrizione e aprendo subito dopo la maschera di dettagli,
                    //la pagina non ha ancora creato il mitt, perchè non c'è stato alcun post back, quindi lo creo io.
                    {
                        corr = new DocsPAWA.DocsPaWR.Corrispondente();
                        corr.tipoCorrispondente = "O";
                        corr.idAmministrazione = UserManager.getUtente(this).idAmministrazione;
                        corr.descrizione = indexCor;
                    }
                }
            }

			if (corr == null)
				return;

			if(corr.info == null && (corr.systemId != null && !corr.systemId.Equals("")))
				corr.info = UserManager.getDettagliCorrispondente(this,corr.systemId);

            if (!string.IsNullOrEmpty(corr.tipoCorrispondente) && ((corr.tipoCorrispondente.Equals("F") && !corr.inRubricaComune) || corr.tipoCorrispondente.Equals("L")))
            {
                this.PanelListaCorrispondenti.Visible = true;
                this.pnl_notcommon.Visible = false;
                this.pnl_email.Visible = false;
                this.btn_CreaCorDoc.Visible = false;
                this.btn_ModMit.Visible = false;
                ArrayList listaCorrispondenti = new ArrayList();
                if (corr.tipoCorrispondente.Equals("F"))
                {
                    listaCorrispondenti = UserManager.getCorrispondentiByCodRF(this, corr.codiceRubrica);
                    lbl_nomeCorr.Text = UserManager.getNomeRF(this, corr.codiceRubrica);
                }
                else
                {
                    listaCorrispondenti = UserManager.getCorrispondentiByCodLista(this, corr.codiceRubrica,UserManager.getInfoUtente().idAmministrazione);
                    lbl_nomeCorr.Text = UserManager.getNomeLista(this, corr.codiceRubrica, UserManager.getInfoUtente().idAmministrazione);
                }
                dg_listCorr.DataSource = creaDataTable(listaCorrispondenti);
                dg_listCorr.DataBind();
            }

			if(!this.Page.IsPostBack)
			{
				if ((schedaDocumento.systemId != null) && (!schedaDocumento.predisponiProtocollazione))
				{
					readOnly = true;
                    // se si tratta di un destinatario visualizzo la mail principale
                    if(!string.IsNullOrEmpty(corr.tipoIE) && corr.tipoIE.Equals("E"))
                    {
                        this.lbl_email.Text = "Email Principale";
                        corr.email = (from c in utils.MultiCasellaManager.GetMailCorrispondenteEsterno(corr.systemId) where c.Principale.Equals("1") select c).Count() > 0 ?
                            "* " + (from c in utils.MultiCasellaManager.GetMailCorrispondenteEsterno(corr.systemId) where c.Principale.Equals("1") select c.Email).First() : string.Empty;
                    }
					setFieldsProperty();
				}
				getInfoCor(corr);
                if (corr != null && !string.IsNullOrEmpty(corr.email))
                    corr.email = corr.email.Replace("* ", "");
			}
			
			//this.btn_CreaCorDoc.Visible = true;
            BindGridViewCaselle(corr);

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
			this.btn_ModMit.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ModMit_Click);
			this.txt_indirizzo.TextChanged += new System.EventHandler(this.txt_indirizzo_TextChanged);
			this.txt_cap.TextChanged += new System.EventHandler(this.txt_cap_TextChanged);
			this.txt_citta.TextChanged += new System.EventHandler(this.txt_citta_TextChanged);
            this.txt_local.TextChanged += new System.EventHandler(this.txt_local_TextChanged);
            this.txt_provincia.TextChanged += new System.EventHandler(this.txt_provincia_TextChanged);
			this.txt_nazione.TextChanged += new System.EventHandler(this.txt_nazione_TextChanged);
			this.txt_telefono.TextChanged += new System.EventHandler(this.txt_telefono_TextChanged);
			this.txt_telefono2.TextChanged += new System.EventHandler(this.txt_telefono2_TextChanged);
			this.txt_fax.TextChanged += new System.EventHandler(this.txt_fax_TextChanged);
			this.txt_email.TextChanged += new System.EventHandler(this.txt_email_TextChanged);
			this.txt_codAOO.TextChanged += new System.EventHandler(this.txt_codAOO_TextChanged);
			this.txt_codAmm.TextChanged += new System.EventHandler(this.txt_codAmm_TextChanged);
            this.txt_codfisc.TextChanged += new System.EventHandler(this.txt_codfisc_TextChanged);
            this.txt_partita_iva.TextChanged += new System.EventHandler(this.txt_partita_iva_TextChanged);
			this.txt_note.TextChanged += new System.EventHandler(this.txt_note_TextChanged);
            
			this.btn_CreaCorDoc.Click += new System.EventHandler(this.btn_CreaCorDoc_Click);
			this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(this.Page_PreRender);
			this.PreRender += new System.EventHandler(this.dettagliCorrispondenti_PreRender);

		}
		#endregion

        protected bool TypeMailCorrEsterno(string typeMail)
        {
            return (typeMail.Equals("1")) ? true : false;
        }

        protected void BindGridViewCaselle(DocsPaWR.Corrispondente corr)
        {
            List<DocsPaWR.MailCorrispondente> listaMail = DocsPAWA.utils.MultiCasellaManager.GetMailCorrispondenteEsterno(corr.systemId);
            if (listaMail != null && listaMail.Count > 0)
            {
                if (listaMail.Count == 1)
                {
                    this.lbl_email.Visible = true;
                    this.txt_email.Visible = true;
                    this.gvCaselle.Visible = false;
                }
                else
                {
                    this.lbl_email.Visible = true;
                    this.lbl_email.Text = "Email";
                    this.txt_email.Visible = false;
                    this.gvCaselle.Visible = true;
                    gvCaselle.DataSource = listaMail;
                    gvCaselle.DataBind();
                }
            }
        }

        private DataTable creaDataTable(ArrayList listaCorrispondenti)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CODICE");
            dt.Columns.Add("DESCRIZIONE");

            for (int i = 0; i < listaCorrispondenti.Count; i++)
            {
                DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente)listaCorrispondenti[i];
                DataRow dr = dt.NewRow();
                dr[0] = c.codiceRubrica;
                dr[1] = c.descrizione;
                dt.Rows.Add(dr);
            }
            return dt;
        }

		private void setFieldsProperty()
		{
			this.txt_cap.ReadOnly = readOnly;
			this.txt_citta.ReadOnly = readOnly;
			this.txt_codfisc.ReadOnly = readOnly;
            this.txt_partita_iva.ReadOnly = readOnly;
			this.txt_email.ReadOnly = readOnly;
			this.txt_fax.ReadOnly = readOnly;
			this.txt_indirizzo.ReadOnly = readOnly;
			this.txt_nazione.ReadOnly = readOnly;
			this.txt_note.ReadOnly = readOnly;
			this.txt_provincia.ReadOnly = readOnly;
			this.txt_telefono.ReadOnly = readOnly;
			this.txt_telefono2.ReadOnly = readOnly;
            this.txt_local.ReadOnly = readOnly;
			this.btn_CreaCorDoc.Enabled = !readOnly;
		}

		//		private HtmlContainerControl GetDivNotCommon()
		//		{
		//			HtmlForm frmDettagliCorrispondenti=this.Page.FindControl("dettagliCorrispondenti") as HtmlForm;
		//			return frmDettagliCorrispondenti.FindControl("div_notcommon") as HtmlContainerControl;
		//		}

		private void getInfoCor(DocsPAWA.DocsPaWR.Corrispondente corr) 
		{
			if(corr!=null)
			{
				System.Web.UI.WebControls.Label[] dettLabel= {this.lbl_indirizzo,this.lbl_cap,this.lbl_citta,this.lbl_provincia,this.lbl_local,this.lbl_nazione,this.lbl_telefono,this.lbl_telefono2,this.lbl_fax,this.lbl_codfisc, this.lbl_partita_iva, this.lbl_note};
				System.Web.UI.WebControls.TextBox[] dettText= {this.txt_indirizzo,this.txt_cap,this.txt_citta,this.txt_provincia,this.txt_local,this.txt_nazione,this.txt_telefono,this.txt_telefono2,this.txt_fax,this.txt_codfisc, this.txt_partita_iva, this.txt_note};
				if(corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Ruolo)))
				{
					//					setLblDett(dettLabel,false);
					//					setTxtDett(dettText,false);
					
					pnl_notcommon.Visible=false;
					
					
					//Panel1.Visible=true;
					//panel2.Visible=true;
					txt_note.Visible=false;
					lbl_note.Visible=false;
				}
                if (string.IsNullOrEmpty(corr.tipoCorrispondente) || (!(corr.tipoCorrispondente.Equals("F") && !corr.inRubricaComune) && !corr.tipoCorrispondente.Equals("L")))
                {
                    drawInfoCor(corr);
                    drawDettagliCorr(corr);
                }
				if ( schedaDocumento.systemId == null )
				{
					/*
					   in attesa di future implementazioni
					   per la gestione di questa casisitica
					*/
				}
				else
				{
					this.lbl_DescIntOp.Text=drawDescrIntOp(corr);
				}
			}
			
		}
		
		private void drawInfoCor(DocsPAWA.DocsPaWR.Corrispondente myCorr) 
		{			
			string desc = "";
			if (myCorr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo)) 
			{
				DocsPaWR.Ruolo ruo;
				if(corr.tipoCorrispondente!=null && corr.tipoCorrispondente.Equals("O"))
				{
					ruo = (DocsPAWA.DocsPaWR.Ruolo)myCorr;
					desc = myCorr.descrizione;
				}
				else
				{
					//	INIZIO MODIFICA PER ALLEGGERIRE LA RISOLUZIONE DEL DESTINATARIO
					DocsPaWR.AddressbookQueryCorrispondente qco =new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
					qco.codiceRubrica=myCorr.codiceRubrica;				
					qco.idAmministrazione=myCorr.idAmministrazione;
					//GLOBALE: perchè se vengo dalla ricerca non ho l'informazione 
					//se il mitt/dest è Interno o Esterno
					qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE;
                    qco.fineValidita = true;
								
					DocsPaWR.UnitaOrganizzativa corrUO;
					string descrUO = "";
					ruo=(DocsPAWA.DocsPaWR.Ruolo) UserManager.getListaCorrispondenti(this,qco)[0];
						
					if(ruo!=null)
					{
						DocsPaWR.Ruolo corrRuolo = ruo;			
						corrUO = corrRuolo.uo;
						while(corrUO!=null) 
						{
							descrUO = descrUO + " - " + corrUO.descrizione;
							corrUO = corrUO.parent;
						}
					
						desc = corrRuolo.descrizione + descrUO;
					} 
					else
					{ 
						desc = myCorr.descrizione;
					}
				}

				this.lbl_nomeCorr.Text = desc;
				if(ruo.codiceRubrica!=null && ruo.codiceRubrica!="")
				{
					this.lbl_codRubr_corr.Text = "(" + ruo.codiceRubrica + ")";
				}
				// FINE MODIFICA

			} 
			else 
				if (myCorr.GetType() == typeof(DocsPAWA.DocsPaWR.Utente)) 
			{
				DocsPaWR.Utente corrUtente = (DocsPAWA.DocsPaWR.Utente) myCorr;
				DocsPaWR.Ruolo corrRuolo;
				if(corrUtente.ruoli!=null && corrUtente.ruoli.Length >0)
				{
					corrRuolo = (DocsPAWA.DocsPaWR.Ruolo) corrUtente.ruoli[0];
				}
				lbl_nomeCorr.Text= corrUtente.descrizione;
				if(corrUtente.codiceRubrica!=null && corrUtente.codiceRubrica!="")
				{
					this.lbl_codRubr_corr.Text = "(" + corrUtente.codiceRubrica + ")";
				}
			}
			else
				if (myCorr.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)) 
			{
				DocsPaWR.UnitaOrganizzativa corrUnitOrg = (DocsPAWA.DocsPaWR.UnitaOrganizzativa) myCorr;
				string descrUO = "";
				
				DocsPaWR.UnitaOrganizzativa corrUO;
				corrUO = corrUnitOrg.parent;
                if(corrUnitOrg.parent!=null && corrUnitOrg.parent.systemId != null && corrUnitOrg.parent.systemId != "")
				while(corrUO!=null) 
				{
					if(corrUO.descrizione==null || (corrUO.descrizione!=null && corrUO.descrizione.Equals(String.Empty)))
					{
						corrUO.descrizione = UserManager.getCorrispondenteBySystemID(this,corrUO.systemId).descrizione;
					}
					descrUO = descrUO+"&nbsp;-&nbsp;"+ corrUO.descrizione;
					corrUO=corrUO.parent;
				}
					
				this.lbl_nomeCorr.Text = corrUnitOrg.descrizione + descrUO;
				if(corrUnitOrg.codiceRubrica!=null && corrUnitOrg.codiceRubrica!="")
				{
					this.lbl_codRubr_corr.Text = "(" + corrUnitOrg.codiceRubrica + ")";
				}
			} 
			else
			{
				this.lbl_nomeCorr.Text = myCorr.descrizione;
				if(myCorr.codiceRubrica!= null && !myCorr.codiceRubrica.Equals(""))
					this.lbl_codRubr_corr.Text = "(" + myCorr.codiceRubrica + ")";
			}
														
		}

		private void drawDettagliCorr(DocsPAWA.DocsPaWR.Corrispondente corr) 
		{			
			if(corr == null) 
			{return;}
			if (corr.info!=null)
			{
				DocsPaVO.addressbook.DettagliCorrispondente dettagliCorr = new DocsPaVO.addressbook.DettagliCorrispondente();
				DocsPaUtils.Data.TypedDataSetManager.MakeTyped(corr.info, dettagliCorr.Corrispondente.DataSet);

				if (dettagliCorr == null) return;



			
				if(dettagliCorr.Corrispondente.Rows.Count > 0)
				{
					this.txt_indirizzo.Text=dettagliCorr.Corrispondente[0].indirizzo;
					this.txt_citta.Text=dettagliCorr.Corrispondente[0].citta;
                    this.txt_local.Text = dettagliCorr.Corrispondente[0].localita;
					this.txt_provincia.Text=dettagliCorr.Corrispondente[0].provincia;
					this.txt_nazione.Text=dettagliCorr.Corrispondente[0].nazione;
					this.txt_telefono.Text=dettagliCorr.Corrispondente[0].telefono;
					this.txt_telefono2.Text=dettagliCorr.Corrispondente[0].telefono2;
					this.txt_fax.Text=dettagliCorr.Corrispondente[0].fax;
					this.txt_codfisc.Text=dettagliCorr.Corrispondente[0].codiceFiscale;
                    this.txt_partita_iva.Text = dettagliCorr.Corrispondente[0].partitaIva;
					this.txt_note.Text=dettagliCorr.Corrispondente[0].note;
					this.txt_cap.Text=dettagliCorr.Corrispondente[0].cap;    
				}
			}

			this.txt_email.Text=corr.email;
			this.txt_codAmm.Text=corr.codiceAmm;
			this.txt_codAOO.Text=corr.codiceAOO;
		}

		protected string GetValtDisplay(string val)
		{	
			try
			{
				string valrtn="&nbsp;";
				if(val!=null && val.Equals(""))
					return valrtn;
				else
					return val;
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
				return "";
			}
				
		}
		private string addRow(string label, string valore) 
		{
			if (valore == null)
				return "";
			else
				return "<TR><TD class='testo_grigio_scuro' height='35%'>" + label+ ":&nbsp;</TD><TD  class='testo_grigio'>" + valore+ "</TD></TR>";
		}
		protected string drawDescrIntOp(DocsPAWA.DocsPaWR.Corrispondente corr)
		{
			
			string CoddAOO="";
			string CodAmm="";
			string dataProt="";
			string dataSpedizione="";
			string Segn="";
			string rtn="";
			string rtncell="";
			//string TipoSpedizione="";
			string tblInit="<table class='contenitore' align=center width=360px cellpadding='3' cellspacing='1'>";
			// modifica sab string Row1="<TR><TD align='center' class='testo_grigio' colspan='2'><b>Ricevuta di ritorno</b></TD></TR>";
            string Row1 = "<TR><TD align='center' class='testo_grigio' colspan='2'><b>Spedizioni</b></TD></TR>";

			string DestDescr="";
			string tblFin="</table>";
			try
			{
				if (schedaDocumento == null)
					schedaDocumento=DocumentManager.getDocumentoInLavorazione(this);
				DocsPaWR.InfoDocumento infoDoc=DocumentManager.getInfoDocumento(schedaDocumento);
				DocsPaWR.ProtocolloDestinatario[] dest = null;
				if(corr.systemId != null)
					dest=DocumentManager.getDestinatariInteropAggConferma(this,infoDoc.idProfile,corr);
				if(dest!=null)
				{
					if (dest.Length>0 )
					{
				
						for(int i=0;i<dest.Length;i++)
						{
							CodAmm=dest[i].codiceAmm; 
							CoddAOO=dest[i].codiceAOO;
							dataProt=dest[i].dataProtocolloDestinatario;
							Segn=dest[i].protocolloDestinatario;
							DestDescr=dest[i].descrizioneCorr;
							dataSpedizione = dest[i].dta_spedizione;

							//TipoSpedizione = dest[i].documentType;
							
                            // commentato per nuova gestione notifiche - sab
                            //rtncell+=this.addRow("Destinatario",DestDescr);
                            //rtncell+=this.addRow("Cod. Amm",CodAmm);
                            //rtncell+=this.addRow("Cod. AOO",CoddAOO);
                            //rtncell+=this.addRow("Data Protocollo",dataProt);
                            //rtncell+=this.addRow("Num. Protocollo",Segn);
                             // fine - commentato per nuova gestione notificheDocumento

							//rtncell+=this.addRow("Tipo Spedizione",TipoSpedizione);
							rtncell+=this.addRow("Data Spedizione",dataSpedizione);
						}
						if((dataSpedizione==null) ||(dataSpedizione!=null && dataSpedizione.Equals(String.Empty)))
						{
							lbl_DescIntOp.Visible=false;
							return "";
						}
						else
						{
							lbl_DescIntOp.Visible=true;
							return rtn=tblInit+Row1+rtncell+tblFin;
						}
					}
					else
					{
						lbl_DescIntOp.Visible=false;
						return "";
					}
				}
			
				else
					return "";
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
				return "";
			}
		}

		private static void setLblDett(System.Web.UI.WebControls.Label[] dettLabel,bool visible)
		{
			for(int i=0;i<dettLabel.Length;i++)
			{
				dettLabel[i].Visible=visible;
			}
		}

		private static void setTxtDett(System.Web.UI.WebControls.TextBox[] dettText,bool visible)
		{
			for(int i=0;i<dettText.Length;i++)
			{
				dettText[i].Visible=visible;
			}
		}

		private bool checkFields()
		{
			if((this.txt_telefono==null || this.txt_telefono.Text.Equals("")) && !(this.txt_telefono2==null || this.txt_telefono2.Text.Equals("")))
			{
				Response.Write("<script language='javascript'>window.alert(\"Attenzione, inserire il campo telefono princ.\");</script>");
				return false;
			}
			else if(this.txt_cap!=null && !this.txt_cap.Text.Equals("") &&this.txt_cap.Text.Length!=5)
			{
				Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo CAP deve essere di 5 cifre\");</script>");
				return false;
			}
			else if(this.txt_cap!=null && !this.txt_cap.Text.Equals("") && !Utils.isNumeric(this.txt_cap.Text))
			{
				Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo CAP deve un numero\");</script>");
				return false;
			}
			else if(this.txt_provincia!=null && !this.txt_provincia.Text.Equals("") && this.txt_provincia.Text.Length!=2)
			{
				Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo provincia deve essere di 2 cifre\");</script>");
				return false;
			}
			else if(this.txt_provincia!=null && !this.txt_provincia.Text.Equals("") && !Utils.isCorrectProv(this.txt_provincia.Text))
			{
				Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo provincia non è corretto\");</script>");
				return false;
			}

            //Controllo campo codice fiscale: se la lunghezza del campo è di 11 applico il controllo della partita iva supponendo che si tratti di UO, altrimenti quello
            // di codice fiscale 
            else if ((this.txt_codfisc != null && !this.txt_codfisc.Text.Trim().Equals("")) && ((this.txt_codfisc.Text.Trim().Length == 11 && Utils.CheckVatNumber(this.txt_codfisc.Text.Trim()) != 0) || (this.txt_codfisc.Text.Trim().Length == 16 && Utils.CheckTaxCode(this.txt_codfisc.Text.Trim()) != 0) || (this.txt_codfisc.Text.Trim().Length != 11 && this.txt_codfisc.Text.Trim().Length != 16)))
            {
                Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo codice fiscale non è corretto\");</script>");
                return false;
            }          

            else if (this.txt_partita_iva != null && !this.txt_partita_iva.Text.Trim().Equals("") && Utils.CheckVatNumber(this.txt_partita_iva.Text.Trim()) != 0)
            {
                Response.Write("<script language='javascript'>window.alert(\"Attenzione, il campo partita iva non è corretto\");</script>");
                return false;
            }
           
			return true;
		}

		/// <summary></summary>
		/// <param name="newCorr"></param>
		private void aggiornaSchedaDocumento(DocsPAWA.DocsPaWR.Corrispondente newCorr)
		{		
			if(tipoCor == null || tipoCor.Equals(""))
				tipoCor = Request.QueryString["tipoCor"];
			if(indexCor == null || indexCor.Equals(""))
				indexCor = Request.QueryString["indexCor"];
			if(schedaDocumento == null)
				schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
			
			if((tipoCor == null) || (tipoCor == ""))
				return;
			if(schedaDocumento == null)
				return;

			if(tipoCor.Equals("M"))
			{
				((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = newCorr;
				((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = true;
			}
			else if(tipoCor.Equals("I"))
			{
				((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio = newCorr;
				((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittenteIntermedio = true;
			}
			else if(tipoCor.Equals("D"))
			{
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[Int32.Parse(indexCor)] = newCorr;
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatari = true;
			}
			else if(tipoCor.Equals("C"))
			{
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[Int32.Parse(indexCor)] = newCorr;
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza = true;
			}

			DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);		
		}
							
		/// <summary></summary>
		/// <returns></returns>
		private DocsPaVO.addressbook.DettagliCorrispondente getDettagliCor()
		{
			DocsPaVO.addressbook.DettagliCorrispondente dettagliCorr = new DocsPaVO.addressbook.DettagliCorrispondente();
			
			
			dettagliCorr.Corrispondente.AddCorrispondenteRow(this.txt_indirizzo.Text,
				this.txt_citta.Text,
				this.txt_cap.Text,
				this.txt_provincia.Text,
				this.txt_nazione.Text,
				this.txt_telefono.Text,
				this.txt_telefono2.Text,
				this.txt_fax.Text,
				this.txt_codfisc.Text.Trim(),
				this.txt_note.Text,
                this.txt_local.Text,
                String.Empty,
                String.Empty,
                String.Empty,
                this.txt_partita_iva.Text.Trim()
                );
			
			return dettagliCorr;
		}

		/// <summary></summary>
		private void creaCorr()
		{
			try 
			{
				corr.codiceRubrica = null;
				corr.idOld = "0";
				corr.systemId = null;
				//28/11/2005 Fede: se il corrisp è occasionale allora corr.tipoIE deve essere null,
				//mentre corr.tipoCorrispondente deve essere valorizzato con "O";
				//corr.tipoIE = "O";
				corr.tipoCorrispondente = "O";
				corr.tipoIE=null;
				//
				corr.email=this.txt_email.Text;
				corr.idRegistro = schedaDocumento.registro.systemId;
				corr.codiceAmm = this.txt_codAmm.Text;
				corr.codiceAOO = this.txt_codAOO.Text;
				
				corr.info = getDettagliCor();	
				Session["dettagliCorr.corrInfo"] = corr.info;

				aggiornaSchedaDocumento(corr);
				Response.Write("<script>var k=window.opener.document.forms[0].submit(); window.close();</script>");		
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}
		}
		


		private void btn_CreaCorDoc_Click(object sender, System.EventArgs e)
		{
			if (!modificato)
			{
				string msg = "Non è stata effettuata alcuna modifica";
				Response.Write("<script language='javascript'>window.alert('" + msg +"');</script>");
				return;
			}
			if (!checkFields())
				return;
			creaCorr();
		}




		private void txt_indirizzo_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}

		private void txt_cap_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}

		private void txt_citta_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}

        private void txt_local_TextChanged(object sender, System.EventArgs e)
        {
            modificato = true;
        }

		private void txt_provincia_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}

		private void txt_nazione_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}

		private void txt_telefono_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}

		private void txt_telefono2_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}

		private void txt_fax_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}

		private void txt_email_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}

		private void txt_codfisc_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}

        private void txt_partita_iva_TextChanged(object sender, System.EventArgs e)
        {
            modificato = true;
        }


		private void txt_note_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}
		
		
		private void dettagliCorrispondenti_PreRender(object sender, System.EventArgs e) 
		{
			//abilitazione delle funzioni in base al ruolo
			if ((schedaDocumento.systemId != null) && (!schedaDocumento.predisponiProtocollazione)) 
			{
				this.btn_ModMit.Enabled = true;
			}
			else
			{
				this.btn_ModMit.Enabled = false;
			}
			UserManager.disabilitaFunzNonAutorizzate(this);

			if(schedaDocumento!=null && schedaDocumento.systemId != null 
				&& schedaDocumento.systemId!="" ) //se già protocollato non si può modificare
				this.btn_ModMit.Enabled = false;
		}

		private void btn_ModMit_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			this.readOnly = false;
			setFieldsProperty();
		}

		private void txt_codAOO_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}

		private void txt_codAmm_TextChanged(object sender, System.EventArgs e)
		{
			modificato = true;	
		}
	}

}

