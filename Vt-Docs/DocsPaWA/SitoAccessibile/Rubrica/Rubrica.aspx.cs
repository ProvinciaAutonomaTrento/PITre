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

namespace DocsPAWA.SitoAccessibile.Rubrica
{
	/// <summary>
	/// Summary description for Rubrica.
	/// </summary>
	public class Rubrica : SessionWebPage
	{
		public enum Action 
		{
			none,
			New,
			DoSearch, 
			Add, 
			Remove, 
			ChPage, 
			Confirm,
			Cancel,
			Abort
		}

		public enum RecipientCapacity { One, Many }
		//private RecipientCapacity capacity = RecipientCapacity.One;


		private AddressbookProperties abookProperties = null;
		private SearchContainer searchResult = null;
		protected System.Web.UI.HtmlControls.HtmlForm frmUserControls;
		private AddressbookContainer abookResult = null;

		private void Page_Load(object sender, System.EventArgs e)
		{
			QuickSearch();

			// Put user code to initialize the page here
			abookProperties = (Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookProperties"]!=null) 
				? (AddressbookProperties)Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookProperties"]
				: new AddressbookProperties();

			searchResult = (Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.SearchContainer"]!=null) 
				? (SearchContainer)Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.SearchContainer"]
				: new SearchContainer();
			
			abookResult=AddressbookResult;

			#region Recupero parametro ACTION
			Action action = Action.none;
			try
			{
				action = (Action)System.Enum.Parse(typeof(Action),(string)this.Context.Request.Params["action"],true);
			} 
			catch (Exception) {}
			#endregion Recupero parametro ACTION

			#region Elaborazione del parametro ACTION
			switch (action)
			{
				case Action.New:
					//Richiesta di apertura di una nuova 
					//pagina della rubrica:
					//Devo impostare le proprietà ...
					New();
					break;
				case Action.DoSearch:
					Search();
					break;
				case Action.Add:
					Add();
					break;
				case Action.Remove:
					Remove();
					break;
				case Action.ChPage:
					string table = this.Context.Request.Params["table"];
					int pn = 0;
					try
					{
						pn = Int32.Parse((string)this.Context.Request.Params["pgnum"]);
					} 
					catch (Exception) {}

					if (table=="search")
						searchResult.PageNumber = pn;
					else if (table=="recipient")
						abookResult.PageNumber = pn;
					break;
				case Action.Confirm:
					Confirm();
					break;
				case Action.Cancel:
					Cancel();
					break;
				case Action.Abort:
					Abort();
					break;
				default:
					break;
			}
			#endregion Elaborazione del parametro ACTION
		}

		public AddressbookProperties Properties
		{
			get { return abookProperties; }
		}

		public SearchContainer SearchResult
		{
			get { return searchResult; }
		}

		public AddressbookContainer Result
		{
			get { return abookResult; }
		}


		public void New()
		{
			string urpl = "";
			string ie = "";
			RecipientCapacity capacity = RecipientCapacity.One;
			string pgCall = "";
			string pgField = "";

			#region Recupero parametro URPL
			//Indica quali check-box del gruppo tipologia devono essere attivate
			//Questo parametro viene espresso con una stringa:
			//	l=Lista
			//	p=Persona
			//	r=Ruolo
			//  u=Unità Organizzativa
			urpl = (string)this.Context.Request.Params["urpl"];
			#endregion Recupero parametro URPL

			#region Recupero parametro IE
			//Indica quali check-box del gruppo locazione devono essere attivate
			//Questo parametro viene espresso con una stringa:
			//	i=Interni
			//	e=Esterni
			ie = (string)this.Context.Request.Params["ie"];
			#endregion Recupero parametro IE

			#region Recupero parametro CAPACITY
			//Indica quanti corrispondenti possono essere selezionati nel risulato:
			//	one=Uno
			//	many=Molti
			try
			{
				capacity = (RecipientCapacity)System.Enum.Parse(typeof(RecipientCapacity),(string)this.Context.Request.Params["capacity"],true);
			}
			catch (Exception) {}
			#endregion Recupero parametro CAPACITY

			#region Recupero parametro PGCALL
			pgCall = (string)this.Context.Request.Params["pgcall"];
			#endregion Recupero parametro PGCALL

			#region Recupero parametro FIELD
			pgField = (string)this.Context.Request.Params["field"];
			#endregion Recupero parametro FIELD

			abookProperties = new AddressbookProperties();
			abookProperties.SetURPL(urpl);
			abookProperties.SetIE(ie);
			abookProperties.Capacity = capacity;
			abookProperties.CallPage = pgCall;
			abookProperties.PageField = pgField;
			Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookProperties"] = abookProperties;

			searchResult = new SearchContainer();
			Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.SearchContainer"] = searchResult;

			abookResult = new AddressbookContainer();
			Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookContainer"] = abookResult;

		}

		private void Search()
		{
			DocsPaWR.DocsPaWebService DocsPaWS = ProxyManager.getWS();
			DocsPaWR.ParametriRicercaRubrica qco = new DocsPAWA.DocsPaWR.ParametriRicercaRubrica();
			UserManager.setQueryRubricaCaller (ref qco);
			
			DocsPaWR.Registro reg = GetCurrentRegistry();
			qco.caller.IdRegistro = (reg!=null) ? reg.systemId : null;

			DocsPaWR.Ruolo r = GetCurrentRule();
			qco.caller.IdRuolo = (r!=null) ? r.systemId : null;

			DocsPaWR.Utente u = GetCurrentUser();
			qco.caller.IdUtente = (u!=null) ? u.systemId : null;

			#region Interni/Esterni
			abookProperties.IStatus = (this.Context.Request.Params["i"]!=null);
			abookProperties.EStatus = (this.Context.Request.Params["e"]!=null);
			
			if (abookProperties.IStatus && abookProperties.EStatus)
				qco.tipoIE = DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE;
			else if (abookProperties.IStatus)
				qco.tipoIE = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
			else if (abookProperties.EStatus)
				qco.tipoIE = DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO;
			#endregion Interni/Esterni

			#region Tipo Corrispondente
			qco.doUo = (abookProperties.UStatus=(this.Context.Request.Params["u"]!=null));
			qco.doRuoli = (abookProperties.RStatus=(this.Context.Request.Params["r"]!=null));
			qco.doUtenti = (abookProperties.PStatus=(this.Context.Request.Params["p"]!=null));
			qco.doListe = (abookProperties.LStatus=(this.Context.Request.Params["l"]!=null));
			#endregion Tipo Corrispondente

			#region Filtri di Ricerca
			qco.parent = "";
			abookProperties.Codice = (this.Context.Request.Params["cod"]!=null) 
				? (string)this.Context.Request.Params["cod"] 
				: "";
			qco.codice = abookProperties.Codice;
			abookProperties.Descrizione = (this.Context.Request.Params["desc"]!=null) 
				? (string)this.Context.Request.Params["desc"]
				: "";
			qco.descrizione = abookProperties.Descrizione;
			abookProperties.Citta = (this.Context.Request.Params["citta"]!=null) 
				? (string)this.Context.Request.Params["citta"]
				: "";
			qco.citta = abookProperties.Citta;
	
			#endregion Filtri di Ricerca

            qco.caller.filtroRegistroPerRicerca = string.Empty;

			searchResult.GlobalRecipient = DocsPaWS.rubricaGetElementiRubrica(qco, UserManager.getInfoUtente(),null);

			Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.SearchContainer"] = searchResult;
		}

		private void Add()
		{
			string selection = (string)this.Context.Request.Params["selection"];

			switch (Properties.Capacity)
			{
				case RecipientCapacity.One:
					if (selection=="all")
					{
						//...opzione non contemplata: non eseguire
					}
					else if (selection=="single")
					{
						//...e viene richiesta la selezione di un solo elemento...
						DocsPaWR.ElementoRubrica elem = null;
						string id = (string)this.Context.Request.Params["cod"];
						bool found = false;
						for (int i=0; id!=null && !found && i<searchResult.PageRecipient.Length; i++)
						{
							//Se il parametro in query-string corrisponde a
							//un elemento della ricerca...
							elem = searchResult.PageRecipient[i];
							if (id==elem.codice)
							{
								if (elem.tipo!="L")//Non vanno considerate le liste di distribuzione
								{
									//...svuoto il vettore e lo aggiorno con il nuovo risultato
									abookResult.AuxArray.Clear();
									abookResult.AuxArray.Add(searchResult.ConvertToCorrispondente(searchResult.PageRecipient[i].codice)[0]);
									found = true;
								}
							}
						}
					}
					break;
				case RecipientCapacity.Many:
					//Il recipiente ammette più risultati...
					if (selection=="all")
					{
						//...e viene richiesta la selezione di più elementi...
						foreach (DocsPAWA.DocsPaWR.ElementoRubrica elem in searchResult.PageRecipient)
						{
							if (elem.tipo=="L")
							{
								DocsPaWR.Corrispondente[] lista = searchResult.ConvertToCorrispondente(elem.codice);
								foreach (DocsPAWA.DocsPaWR.Corrispondente c in lista)
								{
									string tipo=null;
									if (c is DocsPAWA.DocsPaWR.UnitaOrganizzativa)
										tipo="U";
									else if (c is DocsPAWA.DocsPaWR.Ruolo)
										tipo="R";
									else if (c is DocsPAWA.DocsPaWR.Utente)
										tipo="P";
									if (!IsInAuxArray(c.codiceRubrica,tipo))
										//Se non è già presente nel vettore lo aggiungo
										abookResult.AuxArray.Add(c);
								}
							}
							else
							{
								if (!IsInAuxArray(elem.codice,elem.tipo))
									//Se non è già presente nel vettore lo aggiungo
									abookResult.AuxArray.Add(searchResult.ConvertToCorrispondente(elem.codice)[0]);
							}
						}
					}
					else if (selection=="single")
					{
						//...e viene richiesta la selezione di un solo elemento...
						DocsPaWR.ElementoRubrica elem = null;
						string id = (string)this.Context.Request.Params["cod"];
						bool found = false;
						for (int i=0; id!=null && !found && i<searchResult.PageRecipient.Length; i++)
						{
							//Se il parametro in query-string corrisponde a
							//un elemento della ricerca...
							elem = searchResult.PageRecipient[i];
							if (id==elem.codice)
								found = true;
						}

						if (elem.tipo=="L")
						{
							DocsPaWR.Corrispondente[] lista = searchResult.ConvertToCorrispondente(elem.codice);
							foreach (DocsPAWA.DocsPaWR.Corrispondente c in lista)
							{
								string tipo=null;
								if (c is DocsPAWA.DocsPaWR.UnitaOrganizzativa)
									tipo="U";
								else if (c is DocsPAWA.DocsPaWR.Ruolo)
									tipo="R";
								else if (c is DocsPAWA.DocsPaWR.Utente)
									tipo="P";
								if (!IsInAuxArray(c.codiceRubrica,tipo))
									//Se non è già presente nel vettore lo aggiungo
									abookResult.AuxArray.Add(c);
							}
						}
						else
						{
							if (!IsInAuxArray(elem.codice,elem.tipo))
								abookResult.AuxArray.Add(searchResult.ConvertToCorrispondente(elem.codice)[0]);
						}
					}
					break;
				default: 
					break;
			}

			//Trasferisco il contenuto del vettore temporaneo al recipiente...
			PastAuxArrayToRecipient();
		}

		private void Remove()
		{
			string selection = (string)this.Context.Request.Params["selection"];

			if (selection=="all")
			{
				foreach (DocsPAWA.DocsPaWR.Corrispondente corr in abookResult.PageRecipient)
					abookResult.AuxArray.Remove(corr);
			}
			else if (selection=="single")
			{
				//...e viene richiesta la selezione di un solo elemento...
				string id = (string)this.Context.Request.Params["cod"];

				if (IsInAuxArray(id,null))
					abookResult.AuxArray.Remove(abookResult.Get(id));
			}

			//Trasferisco il contenuto del vettore temporaneo al recipiente...
			PastAuxArrayToRecipient();
		}

		/// <summary>
		/// Redirect alla pagina chiamante della rubrica
		/// </summary>
		private void RedirectCallerPage()
		{
			string retPage = abookProperties.CallPage;
			string retField = abookProperties.PageField;

			string dstPg = RelativeDestinationPage(retPage);
			if (retField!=null && retField!="")
				dstPg = dstPg+"?rubrica=true&field="+retField;

			Response.Redirect(dstPg,true);
		}

		private void Confirm()
		{
			if (Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookProperties"]!=null)
				Session.Remove("DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookProperties");
			if (Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.SearchContainer"]!=null)
				Session.Remove("DocsPAWA.SitoAccessibile.Rubrica.Rubrica.SearchContainer");

			// Redirect alla pagina chiamante della rubrica
			this.RedirectCallerPage();
		}

		/// <summary>
		/// Esito della ricerca
		/// </summary>
		public static AddressbookContainer AddressbookResult
		{
			get
			{
				AddressbookContainer retValue=null;

				if (HttpContext.Current.Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookContainer"]!=null)
					retValue=(AddressbookContainer) HttpContext.Current.Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookContainer"];

				return retValue;
			}
			set
			{
				if (value==null)
					HttpContext.Current.Session.Remove("DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookContainer");
				else
					HttpContext.Current.Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookContainer"]=value;
			}
		}

		private void Cancel()
		{
			AddressbookResult=null;

			// Redirect alla pagina chiamante della rubrica
			this.RedirectCallerPage();
		}

		private void Abort()
		{
			string retPage = abookProperties.CallPage;
			string retField = abookProperties.PageField;
			if (Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookProperties"]!=null)
				Session.Remove("DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookProperties");
			if (Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.SearchContainer"]!=null)
				Session.Remove("DocsPAWA.SitoAccessibile.Rubrica.Rubrica.SearchContainer");
			if (Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookContainer"]!=null)
				Session.Remove("DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookContainer");

			string dstPg = RelativeDestinationPage(retPage);

			Response.Redirect(dstPg,true);
		}



		private void PastAuxArrayToRecipient()
		{
			if (abookResult.AuxArray.Count!=0)
			{
				DocsPaWR.Corrispondente[] res = new DocsPAWA.DocsPaWR.Corrispondente[abookResult.AuxArray.Count];
				abookResult.AuxArray.CopyTo(res);
				abookResult.GlobalRecipient = res;
			}
			else
			{
				abookResult.GlobalRecipient = null;
			}
		}

		private bool IsInAuxArray(string codice, string tipo)
		{
			bool found = false;

			ArrayList cv = new ArrayList();
			if (tipo!=null && tipo=="L")
			{
				foreach (DocsPAWA.DocsPaWR.Corrispondente c in searchResult.ConvertToCorrispondente(codice) )
				{
					cv.Add(c.codiceRubrica);
				}
			}
			else
			{
				cv.Add(codice);
			}


			for (int i=0; !found && i<cv.Count; i++)
				for (int j=0; !found && j<abookResult.AuxArray.Count; j++)
				{
					string cod = (string)cv[i];
					if (cod == ((DocsPAWA.DocsPaWR.Corrispondente)abookResult.AuxArray[j]).codiceRubrica)
						found = true;
				}

			return found;
		}

		public string ParseCorrType(string type)
		{
			string tipologia = "";

			switch (type)
			{
				case "U":
					tipologia = "Unit&agrave; Organizzativa";
					break;
				case "R":
					tipologia = "Ruolo";
					break;
				case "P":
					tipologia = "Utente";
					break;
				case "L":
					tipologia = "Lista";
					break;
				default:
					break;
			}

			return tipologia;
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion


		public class AddressbookProperties
		{
			bool ucheck = false;
			bool rcheck = false;
			bool pcheck = false;
			bool lcheck = false;
			bool icheck = false;
			bool echeck = false;
			bool ustatus = false;
			bool rstatus = false;
			bool pstatus = false;
			bool lstatus = false;
			bool istatus = false;
			bool estatus = false;
			RecipientCapacity capacity = RecipientCapacity.One;
			string codice = "";
			string descrizione = "";
			string citta = "";
			string pgcall = "";
			string pgfield = "";

			public AddressbookProperties() {}
			public void SetURPL(string urpl)
			{
				if (urpl!=null)
				{
					if (urpl.IndexOf("l")!=-1)
					{
						bool enabled = (System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"]=="1");
						lcheck = (enabled && true);
						lstatus = (enabled && true);
					}
					if (urpl.IndexOf("p")!=-1)
					{
						pcheck = true;
						pstatus = true;
					}
					if (urpl.IndexOf("r")!=-1)
					{
						rcheck = true;
						rstatus = true;
					}
					if (urpl.IndexOf("u")!=-1)
					{
						ucheck = true;
						ustatus = true;
					}
				}
			}

			public void SetIE(string ie)
			{
				if (ie!=null)
				{
					if (ie.IndexOf("i")!=-1)
					{
						icheck = true;
						istatus = true;
					}
					if (ie.IndexOf("e")!=-1)
					{
						echeck = true;
						estatus = true;
					}
				}
			}

			public bool UCheck
			{
				get { return ucheck; }
				set { ucheck = value; }
			}
			public bool RCheck
			{
				get { return rcheck; }
				set { rcheck = value; }
			}
			public bool PCheck
			{
				get { return pcheck; }
				set { pcheck = value; }
			}
			public bool LCheck
			{
				get { return lcheck; }
				set { lcheck = value; }
			}
			public bool ICheck
			{
				get { return icheck; }
				set { icheck = value; }
			}
			public bool ECheck
			{
				get { return echeck; }
				set { echeck = value; }
			}
			public bool UStatus
			{
				get { return ustatus; }
				set { ustatus = value; }
			}
			public bool RStatus
			{
				get { return rstatus; }
				set { rstatus = value; }
			}
			public bool PStatus
			{
				get { return pstatus; }
				set { pstatus = value; }
			}
			public bool LStatus
			{
				get { return lstatus; }
				set { lstatus = value; }
			}
			public bool IStatus
			{
				get { return istatus; }
				set { istatus = value; }
			}
			public bool EStatus
			{
				get { return estatus; }
				set { estatus = value; }
			}
			public RecipientCapacity Capacity
			{
				get { return capacity; }
				set { capacity = value; }
			}

			public string Codice
			{
				get { return codice; }
				set { codice = (value!=null) ? value : ""; }
			}
			public string Descrizione
			{
				get { return descrizione; }
				set { descrizione = (value!=null) ? value : ""; }
			}
			public string Citta
			{
				get { return citta; }
				set { citta = (value!=null) ? value : ""; }
			}
			public string CallPage
			{
				get { return pgcall; }
				set { pgcall = value; }
			}
			public string PageField
			{
				get { return pgfield; }
				set { pgfield = value; }
			}
		}


		public class SearchContainer
		{
			public const int PAGE_LENGTH = 5;

			DocsPaWR.ElementoRubrica[] globalRecipient = null;
			DocsPaWR.ElementoRubrica[] pageRecipient = null;
			Hashtable corrConversionTable = new Hashtable();
			private int pgNum = 0;
			private int pgCount = 0;

			public SearchContainer() {}

			public DocsPAWA.DocsPaWR.ElementoRubrica[] GlobalRecipient
			{
				get { return globalRecipient; }
				set 
				{ 
					globalRecipient = (value!=null) ? value : new DocsPAWA.DocsPaWR.ElementoRubrica[0]; 

					double div = (double)globalRecipient.Length/(double)PAGE_LENGTH;
					pgCount = (int)Math.Round(div+0.49);
					PageNumber = 0;
				}
			}

			public DocsPAWA.DocsPaWR.ElementoRubrica[] PageRecipient
			{
				get { return pageRecipient; }
			}

			public int PageNumber
			{
				get { return pgNum; }
				set
				{
					if (globalRecipient==null)
						return;

					if (value < 0 || value>pgCount)
						return;

					int minIdx = value*PAGE_LENGTH;
					int maxIdx = Math.Min((minIdx + PAGE_LENGTH),globalRecipient.Length);

					pageRecipient = new DocsPAWA.DocsPaWR.ElementoRubrica[maxIdx-minIdx];
					for (int i=minIdx; i<maxIdx; i++)
						pageRecipient[i-minIdx] = globalRecipient[i];

					pgNum = value;
				}
			}
			public int PageCount
			{
				get { return pgCount; }
			}

			public DocsPAWA.DocsPaWR.ElementoRubrica Get(string codice)
			{
				DocsPaWR.ElementoRubrica er = null;
				for (int i=0; i<GlobalRecipient.Length; i++)
				{
					er = GlobalRecipient[i];
					if (er.codice == codice)
						return er;
				}

				try
				{
					
					DocsPaWR.DocsPaWebService DocsPaWS = ProxyManager.getWS();
					//Federica 23 gennaio 2007
					er = DocsPaWS.rubricaGetElementoRubrica(codice, UserManager.getInfoUtente(),null, "");
				}
				catch (Exception) {}

				return er;
			}

			public DocsPAWA.DocsPaWR.Corrispondente[] ConvertToCorrispondente(string codice)
			{
				DocsPaWR.ElementoRubrica er = Get(codice);
				if (er!=null) 
				{
					if (er.tipo=="L")
					{
						if (!corrConversionTable.ContainsKey(codice))
                        {
                            string idAmm = UserManager.getInfoUtente().idAmministrazione;
							DocsPaWR.DocsPaWebService DocsPaWS = ProxyManager.getWS();
							ArrayList aux = new ArrayList(DocsPaWS.getCorrispondentiByCodLista(er.codice,idAmm, UserManager.getInfoUtente()));
							DocsPaWR.Corrispondente[] lista = new DocsPAWA.DocsPaWR.Corrispondente[aux.Count];
							aux.CopyTo(lista);
							corrConversionTable.Add(er.codice, lista);
						}
						return (DocsPAWA.DocsPaWR.Corrispondente[])corrConversionTable[codice];
					}
					else 
					{
						if (!corrConversionTable.ContainsKey(codice))
						{
							DocsPaWR.DocsPaWebService DocsPaWS = ProxyManager.getWS();
							DocsPaWR.Corrispondente[] lista = new DocsPAWA.DocsPaWR.Corrispondente[1];
							DocsPaWR.AddressbookTipoUtente tipo = (er.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);
							DocsPaWR.Corrispondente corr = DocsPaWS.AddressbookGetCorrispondenteByCodRubricaIE (er.codice, tipo, UserManager.getInfoUtente());
							if (corr!=null)
							{
								corr.tipoCorrispondente = er.tipo;
								lista[0]=corr;
								corrConversionTable.Add(er.codice, lista);
							}
						}
						return (DocsPAWA.DocsPaWR.Corrispondente[])corrConversionTable[codice];
					}
				}

				return new DocsPAWA.DocsPaWR.Corrispondente[0];
			}
		}


		public class AddressbookContainer
		{
			public const int PAGE_LENGTH = 5;

			DocsPaWR.Corrispondente[] globalRecipient = null;
			DocsPaWR.Corrispondente[] pageRecipient = null;
			private int pgNum = 0;
			private int pgCount = 0;
			ArrayList aux = new ArrayList();

			public AddressbookContainer() {}

			public DocsPAWA.DocsPaWR.Corrispondente[] GlobalRecipient
			{
				get { return globalRecipient; }
				set 
				{ 
					globalRecipient = (value!=null) ? value : new DocsPAWA.DocsPaWR.Corrispondente[0]; 

					double div = (double)globalRecipient.Length/(double)PAGE_LENGTH;
					pgCount = (int)Math.Round(div+0.49);
					PageNumber = 0;
					aux = new ArrayList(globalRecipient);
				}
			}

			public DocsPAWA.DocsPaWR.Corrispondente[] PageRecipient
			{
				get { return pageRecipient; }
			}

			public int PageNumber
			{
				get { return pgNum; }
				set
				{
					if (globalRecipient==null)
						return;

					if (value < 0 || value>pgCount)
						return;

					int minIdx = value*PAGE_LENGTH;
					int maxIdx = Math.Min((minIdx + PAGE_LENGTH),globalRecipient.Length);

					pageRecipient = new DocsPAWA.DocsPaWR.Corrispondente[maxIdx-minIdx];
					for (int i=minIdx; i<maxIdx; i++)
						pageRecipient[i-minIdx] = globalRecipient[i];

					pgNum = value;
				}
			}
			public int PageCount
			{
				get { return pgCount; }
			}

			public ArrayList AuxArray
			{
				get { return aux; }
				set { aux = value; }
			}
			public DocsPAWA.DocsPaWR.Corrispondente Get(string codice)
			{
				foreach (DocsPAWA.DocsPaWR.Corrispondente c in GlobalRecipient)
				{
					if (c.codiceRubrica == codice)
						return c;
				}
				return null;
			}
		}
	}
}
