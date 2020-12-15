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
	/// Summary description for modificaMittTrasm.
	/// </summary>
    public class modificaMittTrasm : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btn_salva;
		protected System.Web.UI.WebControls.Label lbl_nome;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.DropDownList ddl_ruolo;
		protected System.Web.UI.WebControls.DropDownList ddl_utente;
		protected System.Web.UI.WebControls.Label Label1;

		protected DocsPAWA.DocsPaWR.Utente utente;
		protected DocsPAWA.DocsPaWR.Ruolo ruolo;
		protected Hashtable m_hashTableRuoliSup;
		protected DocsPAWA.DocsPaWR.Ruolo[] listaRuoliSup;

		protected Hashtable m_hashTableUtenti;
		protected DocsPAWA.DocsPaWR.Corrispondente[] listaUtenti;
		protected DocsPAWA.DocsPaWR.Trasmissione trasmissione;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if(!IsPostBack)
			{
				utente = UserManager.getUtente(this);
				ruolo = UserManager.getRuolo(this);
				caricaValoriRuoli(ruolo);
				caricaUtentiInRuolo(ruolo.codiceRubrica, utente.systemId);
			}
		}

		private void caricaValoriRuoli( DocsPAWA.DocsPaWR.Ruolo ruoloCorr)
		{
					
			if (!Page.IsPostBack)
			{
				m_hashTableRuoliSup=new Hashtable();

				//inserisco il ruolo corrente
				m_hashTableRuoliSup.Add(0,ruoloCorr);
				ListItem newItem=new ListItem(ruoloCorr.descrizione,ruoloCorr.systemId);
				this.ddl_ruolo.Items.Add(newItem);

				//calcolo i ruoli superiori
				listaRuoliSup = UserManager.getListaRuoliSup(this, ruoloCorr);

				if (listaRuoliSup!=null && listaRuoliSup.Length>0)
				{
					for (int i=0; i<listaRuoliSup.Length; i++)
					{
						m_hashTableRuoliSup.Add(i+1,listaRuoliSup[i]);
							
						newItem=new ListItem(listaRuoliSup[i].descrizione,listaRuoliSup[i].systemId);
						this.ddl_ruolo.Items.Add(newItem);
					}								
				}
				TrasmManager.setHashRuoliSup(this,m_hashTableRuoliSup);
				//il 1° ruolo è quello corrente - controllare: valido solo se la trasm è nuova 
				this.ddl_ruolo.SelectedIndex = 0;
			}
			else
			{
				m_hashTableRuoliSup=TrasmManager.getHashRuoliSup(this);
			}
		}



		private void caricaUtentiInRuolo(string codRubrica, string codUtente)
		{
					
			m_hashTableUtenti=new Hashtable();
			ddl_utente.Items.Clear();

			//calcolo gli utenti con quel Ruolo
			listaUtenti = queryUtenti(codRubrica);

			if (listaUtenti!=null && listaUtenti.Length>0)
			{
				for (int i=0; i<listaUtenti.Length; i++)
				{
					m_hashTableUtenti.Add(i,listaUtenti[i]);
							
					ListItem newItem=new ListItem(listaUtenti[i].descrizione,listaUtenti[i].systemId);
					this.ddl_utente.Items.Add(newItem);												
					if (codUtente != null)
					{
						if (listaUtenti[i].systemId.Equals(codUtente))
							this.ddl_utente.SelectedIndex = i;
					}
				}
				TrasmManager.setHashUtenti(this,m_hashTableUtenti);
				
			}
			else
			{
				m_hashTableUtenti=TrasmManager.getHashUtenti(this);
			}
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
			
			return UserManager.getListaCorrispondenti(this.Page, qco);
		}

		private void ddl_ruolo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (m_hashTableRuoliSup == null)
				m_hashTableRuoliSup = TrasmManager.getHashRuoliSup(this);

			DocsPaWR.Ruolo ruoloSel = (DocsPAWA.DocsPaWR.Ruolo) m_hashTableRuoliSup[ddl_ruolo.SelectedIndex];
			caricaUtentiInRuolo(ruoloSel.codiceRubrica,null);
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
			this.ddl_ruolo.SelectedIndexChanged += new System.EventHandler(this.ddl_ruolo_SelectedIndexChanged);
			this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btn_salva_Click(object sender, System.EventArgs e)
		{
			trasmissione = TrasmManager.getGestioneTrasmissione(this);
			m_hashTableRuoliSup = TrasmManager.getHashRuoliSup(this);
			m_hashTableUtenti =   TrasmManager.getHashUtenti(this);

			if (trasmissione != null)
			{
				ruolo = (DocsPAWA.DocsPaWR.Ruolo) m_hashTableRuoliSup[this.ddl_ruolo.SelectedIndex];
				utente = (DocsPAWA.DocsPaWR.Utente) m_hashTableUtenti[this.ddl_utente.SelectedIndex];
			}

				trasmissione.ruolo = ruolo;
				trasmissione.utente = utente;
				TrasmManager.setGestioneTrasmissione(this, trasmissione);
			    Response.Write("<script>var k=window.opener.document.forms[0].submit(); window.close();</script>");


		}
	}
}
