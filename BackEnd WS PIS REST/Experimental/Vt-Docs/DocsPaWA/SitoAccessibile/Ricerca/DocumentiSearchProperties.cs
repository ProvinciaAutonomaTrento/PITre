using System;
using System.Collections;
using System.Web;
using System.Web.UI;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	public class SearchProperties
	{
		private bool protin = true;
		private bool protout = true;
		private bool protint = true;
		private bool docs = true;

		private bool advDocProps = false;
		private bool advProtProps = false;
		private DocsPAWA.DocsPaWR.TipologiaAtto[] tipidoc = new DocsPAWA.DocsPaWR.TipologiaAtto[0];
		private string message = null;

		private DocProperties docProperties = new DocProperties();
		private ProtProperties protProperties = new ProtProperties();

		public SearchProperties()
		{
			message = null;
			docProperties = new DocProperties();
			protProperties = new ProtProperties();

			DocsPaWR.Ruolo ruolo = UserManager.getRuolo();
			protProperties.Registri = new SearchRegistry[ruolo.registri.Length];
			for (int i=0; i<ruolo.registri.Length; i++)
			{
				SearchRegistry sr = new SearchRegistry();
				sr.Id = ruolo.registri[i].systemId;
				sr.Codice = ruolo.registri[i].codRegistro;
				sr.Descrizione = ruolo.registri[i].descrizione;
				sr.Selezionato = (UserManager.getRegistroSelezionato(new Page()).systemId==sr.Id);

				protProperties.Registri[i] = sr;
			}

			try
			{
				//this.protin=this.IsEnabledProtocolloInterno();

				ArrayList aux_td = new ArrayList();
				DocsPaWR.DocsPaWebService DocsPaWS = ProxyManager.getWS();
				
				if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && 
					System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1") 
				{
					string idAmm = UserManager.getInfoUtente().idAmministrazione;
					aux_td.AddRange(DocsPaWS.GetTipologiaAttoProfDin(idAmm));
				}
				else
				{
					aux_td.AddRange(DocsPaWS.DocumentoGetTipologiaAtto());
				}
					
				DocsPaWR.TipologiaAtto all = new DocsPAWA.DocsPaWR.TipologiaAtto();
				all.systemId=string.Empty;
				all.descrizione = "(Tutti)";
				aux_td.Insert(0,all);

				tipidoc = new DocsPAWA.DocsPaWR.TipologiaAtto[aux_td.Count];
				aux_td.CopyTo(tipidoc);
			}
			catch (Exception ex) { Console.WriteLine(ex.Message); }
		}

		/// <summary>
		/// Verifica se l'utente è abilitato alla protocollazione interna
		/// </summary>
		/// <returns></returns>
		private bool IsEnabledProtocolloInterno()
		{	
			DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();
			return ws.IsInternalProtocolEnabled(UserManager.getUtente().idAmministrazione);
		}

		public string Message
		{
			get { return message; }
			set { message = value; }
		}
		public DocsPAWA.DocsPaWR.TipologiaAtto[] TipiDocumento
		{
			get { return tipidoc; }
		}

		public bool AdvancedDocProperties
		{
			get { return advDocProps; }
			set { advDocProps = value; }
		}

		public bool AdvancedProtProperties
		{
			get { return advProtProps; }
			set { advProtProps = value; }
		}


		public bool ProtocolliArrivo
		{
			get { return protin; }
			set { protin = value; }
		}
		public bool ProtocolliPartenza
		{
			get { return protout; }
			set { protout = value; }
		}
		public bool ProtocolliInterni
		{
			get { return protint; }
			set { protint = value; }
		}
		public bool DocumentiGrigi
		{
			get { return docs; }
			set { docs = value; }
		}
		public DocProperties Documento
		{
			get { return docProperties; }
		}
		public ProtProperties Protocollo
		{
			get { return protProperties; }
		}
	}

	public class DocProperties
	{
		private string dr_obj = "";
		private string dr_note = "";
		private string dr_tipodoc = "";
		private string dr_evid = "all";
		private string dr_crea_da = "";
		private string dr_crea_a = "";
		private string dr_id_da = "";
		private string dr_id_a = "";
		private DocsPAWA.DocsPaWR.DocumentoParolaChiave[] _paroleChiavi=new DocsPAWA.DocsPaWR.DocumentoParolaChiave[0];
		private string de_firma_nome = "";
		private string de_firma_cognome = "";
		//			private DocsPAWA.DocsPaWR.Fascicolo de_fasc = new DocsPAWA.DocsPaWR.Fascicolo();
		private string de_fasc_cod = "";
		private string de_fasc_desc = "";

		private bool _priviDiAssegnatario=false;
		private bool _priviDiImmagine=false;
		private bool _priviDiFascicolazione=false;

		public string Oggetto
		{
			get { return dr_obj; }
			set { dr_obj = (value!=null) ? value : ""; }
		}

		public string Note
		{
			get { return dr_note; }
			set { dr_note = (value!=null) ? value : ""; }
		}

		public string Tipologia
		{
			get { return dr_tipodoc; }
			set { dr_tipodoc = (value!=null) ? value : ""; }
		}

		public string Evidenza
		{
			get { return dr_evid; }
			set { dr_evid = (value!=null) ? value : "all"; }
		}

		public string DataDocumentoDa
		{
			get { return dr_crea_da; }
			set { dr_crea_da = (value!=null) ? value : ""; }
		}
		public string DataDocumentoA
		{
			get { return dr_crea_a; }
			set { dr_crea_a = (value!=null) ? value : ""; }
		}
		public string IdDocumentoDa
		{
			get { return dr_id_da; }
			set { dr_id_da = (value!=null) ? value : ""; }
		}
		public string IdDocumentoA
		{
			get { return dr_id_a; }
			set { dr_id_a = (value!=null) ? value : ""; }
		}

		public DocsPAWA.DocsPaWR.DocumentoParolaChiave[] ParoleChiavi
		{
			get 
			{ 
				return this._paroleChiavi; 
			}
			set 
			{
				this._paroleChiavi=value;
			}
		}

		public string NomeFirmatario
		{
			get { return de_firma_nome; }
			set { de_firma_nome = (value!=null) ? value : ""; }
		}

		public string CognomeFirmatario
		{
			get { return de_firma_cognome; }
			set { de_firma_cognome = (value!=null) ? value : ""; }
		}

		public bool PriviDiAssegnatario
		{
			get
			{
				return this._priviDiAssegnatario;
			}
			set
			{
				this._priviDiAssegnatario=value;
			}
		}

		public bool PriviDiImmagine
		{
			get
			{
				return this._priviDiImmagine;
			}
			set
			{
				this._priviDiImmagine=value;
			}
		}

		public bool PriviDiFascicolazione
		{
			get
			{
				return this._priviDiFascicolazione;
			}
			set
			{
				this._priviDiFascicolazione=value;
			}
		}

		public string CodiceFascicolo
		{
			get { return de_fasc_cod; }
			set { de_fasc_cod = (value!=null) ? value : ""; }
		}

		public string DescrizioneFascicolo
		{
			get { return de_fasc_desc; }
			set { de_fasc_desc = (value!=null) ? value : ""; }
		}
		/*
					public DocsPAWA.DocsPaWR.Fascicolo Fascicolo
					{
						get { return de_fasc; }
						set { de_fasc = (value!=null) ? value : new DocsPAWA.DocsPaWR.Fascicolo(); }
					}
		*/			
	}

	public class ProtProperties
	{
		private SearchRegistry[] pr_reg = new SearchRegistry[0];
		private string pr_segn = "";
		private DocsPAWA.DocsPaWR.Corrispondente pr_mitt_dest = new DocsPAWA.DocsPaWR.Corrispondente();
		private string pr_dataarr_da = "";
		private string pr_dataarr_a = "";
		private string pr_dataprot_da = "";
		private string pr_dataprot_a = "";
		private string pr_numprot_da = "";
		private string pr_numprot_a = "";
		private string pe_segnmitt = "";
		private string pe_dataprotmitt_da = null;
		private string pe_dataprotmitt_a = "";
		private string de_stato = "all";
		private DocsPAWA.DocsPaWR.Corrispondente pe_mitt_inter = new DocsPAWA.DocsPaWR.Corrispondente();
		private string pe_dataproteme = "";
		private string pe_proteme = "";

		/// <summary>
		/// Impostazione anno corrente
		/// </summary>
		private string _annoProtocollo=DateTime.Now.Year.ToString();
		
		public string AnnoProtocollo
		{
			get
			{
				return this._annoProtocollo;
			}
			set
			{
				this._annoProtocollo=value;
			}
		}


		public SearchRegistry[] Registri 
		{
			get { return pr_reg; }
			set { pr_reg = value; }
		}

		public string Segnatura
		{
			get { return pr_segn; }
			set { pr_segn = (value!=null) ? value : ""; }
		}

		public DocsPAWA.DocsPaWR.Corrispondente Corrispondente
		{
			get { return pr_mitt_dest; }
			set { pr_mitt_dest = (value!=null) ? value : new DocsPAWA.DocsPaWR.Corrispondente(); }
		}

		public string DataArrivoDa
		{
			get { return pr_dataarr_da; }
			set { pr_dataarr_da = (value!=null) ? value : ""; }
		}
		public string DataArrivoA
		{
			get { return pr_dataarr_a; }
			set { pr_dataarr_a = (value!=null) ? value : ""; }
		}

		public string DataProtocolloDa
		{
			get { return pr_dataprot_da; }
			set { pr_dataprot_da = (value!=null) ? value : ""; }
		}
		public string DataProtocolloA
		{
			get { return pr_dataprot_a; }
			set { pr_dataprot_a = (value!=null) ? value : ""; }
		}


		public string NumProtocolloDa
		{
			get { return pr_numprot_da; }
			set { pr_numprot_da = (value!=null) ? value : ""; }
		}
		public string NumProtocolloA
		{
			get { return pr_numprot_a; }
			set { pr_numprot_a = (value!=null) ? value : ""; }
		}

		public string SegnaturaMittente
		{
			get { return pe_segnmitt; }
			set { pe_segnmitt = (value!=null) ? value : ""; }
		}

		public string DataProtMittenteDa
		{
			get { return pe_dataprotmitt_da; }
			set { pe_dataprotmitt_da = (value!=null) ? value : ""; }
		}
		public string DataProtMittenteA
		{
			get { return pe_dataprotmitt_a; }
			set { pe_dataprotmitt_a = (value!=null) ? value : ""; }
		}

		public string Stato
		{
			get { return de_stato; }
			set { de_stato = (value!=null) ? value : "all"; }
		}

		public DocsPAWA.DocsPaWR.Corrispondente MittenteIntermedio
		{
			get { return pe_mitt_inter; }
			set { pe_mitt_inter = (value!=null) ? value : new DocsPAWA.DocsPaWR.Corrispondente(); }
		}

		public string DataProtEmergenza
		{
			get { return pe_dataproteme; }
			set { pe_dataproteme = (value!=null) ? value : ""; }
		}

		public string ProtocolloEmergenza
		{
			get { return pe_proteme; }
			set { pe_proteme = value; }
		}

		public void SelectAllRegistries()
		{
			foreach (SearchRegistry sr in pr_reg)
				sr.Selezionato = true;
		}

		public void SelectNoRegistries()
		{
			foreach (SearchRegistry sr in pr_reg)
				sr.Selezionato = false;
		}

		public void SelectRegistryWithId(string id, bool val)
		{
			foreach (SearchRegistry sr in pr_reg)
				if (sr.Id==id)
					sr.Selezionato = val;
		}

		public int SelectedRegistryCount()
		{
			int count = 0;
			foreach (SearchRegistry sr in pr_reg)
				if (sr.Selezionato)
					count++;
			return count;
		}

	}

	public class SearchRegistry
	{
		private string id = "";
		private string cod = "";
		private string desc = "";
		private bool sel = false;

		public string Id
		{ 
			get { return id; }
			set { id = (value!=null) ? value : ""; }
		}
		public string Codice
		{ 
			get { return cod; }
			set { cod = (value!=null) ? value : ""; }
		}
		public string Descrizione
		{ 
			get { return desc; }
			set { desc = (value!=null) ? value : ""; }
		}
		public bool Selezionato
		{ 
			get { return sel; }
			set { sel = value; }
		}
	}

}
