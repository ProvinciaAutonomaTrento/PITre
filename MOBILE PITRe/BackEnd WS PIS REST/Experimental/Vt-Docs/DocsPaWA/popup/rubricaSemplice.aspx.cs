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
	/// Summary description for rubricaSemplice.
	/// </summary>
    public class rubricaSemplice : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.ImageButton btn_aggiungi;
		protected System.Web.UI.WebControls.ImageButton btn_cerca;
		protected System.Web.UI.WebControls.ListBox ListCorrispondenti;
		protected System.Web.UI.WebControls.Label LabelRubrica;
		protected System.Web.UI.WebControls.Label Label1;
		string    wnd;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.TextBox txt_Rubrica;
		protected System.Web.UI.WebControls.Label LabelIns;
		protected System.Web.UI.WebControls.RadioButtonList rbFiltro;
		protected System.Web.UI.WebControls.Label Label2;
		string    target;

	
		private void Page_Load(object sender, System.EventArgs e)
		{
			wnd=Request.QueryString["wnd"];
			target=Request.QueryString["target"];
			if (target.Equals("dest"))
				ListCorrispondenti.SelectionMode = ListSelectionMode.Multiple; 

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
			this.rbFiltro.SelectedIndexChanged += new System.EventHandler(this.rbFiltro_SelectedIndexChanged);
			this.btn_cerca.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cerca_Click);
			this.ListCorrispondenti.SelectedIndexChanged += new System.EventHandler(this.ListCorrispondenti_SelectedIndexChanged);
			this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btn_aggiungi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		
		}

		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			UserManager.removeParentCorr(this);
			if (wnd != null && (wnd.Equals("proto")) ) 
			{
				
				int selIndex = ListCorrispondenti.SelectedIndex;				
				if (selIndex > -1)	
				{		
					setCorrispondentiProtocollo(); 
					Response.Write("<script>var k=window.opener.document.forms[0].submit(); window.close();</script>");	 
					Cache.Remove("filtroRubrica"); 
				}
				else
				{
					Response.Write("<script>alert('Selezionare almeno un corrispondente');</script>");	 
				}
			}
			Session.Remove("rubricaSempl.listaCorr");
			//Session.Remove("filtroRubrica"); 
			
		}

		private void setCorrispondentiProtocollo() 
		{
			try
			{
				if(target==null)
				{
					return;
				}
				DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoInLavorazione(this);
				DocsPaWR.Corrispondente[] searchCorr = (DocsPAWA.DocsPaWR.Corrispondente[]) Session["rubricaSempl.listaCorr"];	
				DocsPaWR.Corrispondente corr;
				//controlla i corrispondenti selezionati, li mette in una hashTable e poi carica la pagina 
				if(target.Equals("dest")) 
				{
					// Protocollo in uscita
					for(int i=0;i<ListCorrispondenti.Items.Count;i++) 
					{ 

						if(ListCorrispondenti.Items[i].Selected) 
						{
							corr = (DocsPAWA.DocsPaWR.Corrispondente)searchCorr[i];
							schedaDoc = addDestinatari(schedaDoc, corr);
							//Logger.log("Corrispondente Proto= "+corr.descrizione);
						}
					}
						
					DocumentManager.setDocumentoInLavorazione(this,schedaDoc);
						
				} 
				else if (target.Equals("mitt") || target.Equals("mittInt")) 
				{
					
					if (ListCorrispondenti.SelectedIndex > -1)
					{
						corr = (DocsPAWA.DocsPaWR.Corrispondente)searchCorr[ListCorrispondenti.SelectedIndex];
						
						if(target.Equals("mitt"))	
							((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente = corr;
						else
							((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenteIntermedio = corr;
									
						//Session["docProtocollo.schedaDocumento"] = schedaDoc;
						DocumentManager.setDocumentoInLavorazione(this,schedaDoc);					
					}
				}
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}
		}
		private DocsPAWA.DocsPaWR.SchedaDocumento addDestinatari(DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc, DocsPAWA.DocsPaWR.Corrispondente corr) 
		{
			//controllo se esiste già il corrispondente selezionato
			DocsPaWR.Corrispondente[] listaDest;

			listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari;
			
			if (UserManager.esisteCorrispondente(listaDest,corr))
				return schedaDoc;
			if (UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza,corr))
				return schedaDoc;

			//aggiungo il corrispondente
			//di default lo aggiungo tra i destinatari principali
			if (schedaDoc.protocollo != null)
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari = UserManager.addCorrispondente(listaDest, corr);

			return schedaDoc;
		}

		
		private void ListCorrispondenti_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (ListCorrispondenti.SelectedIndex > -1)
			{
				DocsPaWR.Corrispondente[] searchCorr = (DocsPAWA.DocsPaWR.Corrispondente[]) Session["rubricaSempl.listaCorr"];	 
				DocsPaWR.Corrispondente corr = (DocsPAWA.DocsPaWR.Corrispondente)searchCorr[ListCorrispondenti.SelectedIndex];
				corr = UserManager.getCorrispondente(this, corr.codiceRubrica,true);
				UserManager.setParentCorr(this, corr);
			}
		}

		
		private void btn_cerca_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			//modifica per filtri utenti e ruoli
			//si devono inserire almeno 3 caratteri

			if ((string)Cache.Get("filtroRubrica")== null)
			{
				Page.RegisterStartupScript("","<script>alert('Selezionare un filtro per la ricerca')</script>");
				return;
			} 
			if (this.txt_Rubrica.Text.Length < 3)
			{
				Page.RegisterStartupScript("","<script>alert('Inserire almeno 3 caratteri')</script>");
				return;
			}			
			Session.Remove("rubricaSempl.listaCorr");
			UserManager.removeParentCorr(this);
			
			DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
			qco.idAmministrazione=UserManager.getInfoUtente(this).idAmministrazione;// ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];
			qco.idRegistri = UserManager.getListaIdRegistri(this); //rappresenta il registro selezionato dall'utente e non quello del documento
			qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE;
			//in base alla selezione setto la descrizione corrispondente
			switch((string)Cache.Get("filtroRubrica"))
			{
				case("fUO"):
					qco.descrizioneUO = txt_Rubrica.Text;
					break;
				case("fRuolo"):
					qco.descrizioneRuolo = this.txt_Rubrica.Text;
					break;
					case("fUtente"):
					qco.cognomeUtente = this.txt_Rubrica.Text;  
					break;
			}
			
			DocsPaWR.Corrispondente[] listaCorr=UserManager.getListaCorrispondentiSemplice(this);

			//riempio la lista Corrispondenti
			ListCorrispondenti.Items.Clear();
			if(listaCorr.Length==0)
			{
				Page.RegisterStartupScript("","<script>alert('La ricerca non ha prodotto nessun risultato')</script>");
				return;
			} 
			for(int i= 0; i< listaCorr.Length ; i++) 
			{
				ListCorrispondenti.Items.Add(listaCorr[i].codiceRubrica + " - "+ listaCorr[i].descrizione);	
				ListCorrispondenti.Items[i].Value=listaCorr[i].systemId;
			}

			Session.Add("rubricaSempl.listaCorr", listaCorr);
		}

		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			Session.Remove("rubricaSempl.listaCorr");
			UserManager.removeParentCorr(this);
			//rimuovo la chiave per il filtro
			//Session.Remove("filtroRubrica"); 
			Cache.Remove("filtroRubrica"); 
			Response.Write("<script>window.close();</script>");	

		}

		//private string valFiltro="";
		private void rbFiltro_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Cache.Insert("filtroRubrica",rbFiltro.SelectedItem.Value);   
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
	}
}
