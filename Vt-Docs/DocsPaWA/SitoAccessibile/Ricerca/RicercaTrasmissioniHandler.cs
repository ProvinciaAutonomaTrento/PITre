using System;
using System.Collections;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Trasmissioni;
using DocsPAWA.SitoAccessibile.Documenti.Trasmissioni;
using DocsPAWA.SitoAccessibile.Paging;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Summary description for RicercaTrasmissioniHandler.
	/// </summary>
	public class RicercaTrasmissioniHandler
	{
		private const string SESSION_KEY="TrasmissioniFilterItem";

		/// <summary>
		/// Filtro correntemente impostato
		/// </summary>
		public static TrasmissioniFilterItem CurrentFilter
		{
			get
			{
				if (System.Web.HttpContext.Current.Session[SESSION_KEY]!=null)
					return System.Web.HttpContext.Current.Session[SESSION_KEY] as TrasmissioniFilterItem;
				else
					return null;
			}
			set
			{
				System.Web.HttpContext.Current.Session[SESSION_KEY]=value;
			}
		}

		public RicercaTrasmissioniHandler()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		/// <summary>
		/// Reperimento delle ragioni trasmissione per il contesto della ricerca
		/// </summary>
		/// <returns></returns>
		public RagioneTrasmissione[] GetRagioniTrasmissione()
		{
			return this.GetRagioniTrasmissione(string.Empty);
		}

		/// <summary>
		/// Reperimento ragioni trasmissione per il contesto della ricerca
		/// </summary>
		/// <param name="accessRights"></param>
		/// <returns></returns>
		public RagioneTrasmissione[] GetRagioniTrasmissione(string accessRights)
		{
			DocsPaWR.TrasmissioneDiritti diritti=new DocsPAWA.DocsPaWR.TrasmissioneDiritti();
			
			if (accessRights!=null && accessRights!=string.Empty)
				diritti.accessRights=accessRights;

			diritti.idAmministrazione=UserManager.getUtente().idAmministrazione;

			DocsPaWebService ws=new DocsPaWebService();
			return ws.TrasmissioneGetRagioni(diritti,true);
		}

		/// <summary>
		/// Reperimento tipologie filtri trasmissione
		/// </summary>
		/// <returns></returns>
		public string[] GetFiltriTrasmissione()
		{
			ArrayList list=new ArrayList(Enum.GetNames(typeof(DocsPAWA.DocsPaWR.FiltriTrasmissione)));
			list.Sort();
			return (string[]) list.ToArray(typeof(string));
		}		

		/// <summary>
		/// Reperimento trasmissioni dell'utente e del ruolo
		/// </summary>
		/// <param name="searchType"></param>
		/// <param name="pagingContext"></param>
		/// <returns></returns>
		public Trasmissione[] GetTrasmissioni(TipiTrasmissioniEnum searchType,PagingContext pagingContext)
		{
			return this.GetTrasmissioni(searchType,pagingContext,null);
		}

		/// <summary>
		/// Reperimento trasmissioni dell'utente e del ruolo, con la possibilità di effettuare filtri
		/// </summary>
		/// <param name="searchType"></param>
		/// <param name="pagingContext"></param>
		/// <param name="filters"></param>
		/// <returns></returns>
		public Trasmissione[] GetTrasmissioni(TipiTrasmissioniEnum searchType,PagingContext pagingContext,FiltroRicerca[] filters)
		{
			Trasmissione[] retValue=null;

			Utente utente=UserManager.getUtente();
			Ruolo ruolo=UserManager.getRuolo();
			
			DocsPaWebService ws=new DocsPaWebService();

			int pageCount;
			int recordCount;

			if (searchType==TipiTrasmissioniEnum.Effettuate)
			{	
				retValue=ws.TrasmissioneGetQueryEffettuatePaging(
					new TrasmissioneOggettoTrasm(),
					filters,
					utente,
					ruolo,
					pagingContext.PageNumber,
					out pageCount,
					out recordCount);
			}
			else
			{
				retValue=ws.TrasmissioneGetQueryRicevutePaging(
					new TrasmissioneOggettoTrasm(),
					filters,
					utente,
					ruolo,
					pagingContext.PageNumber,
					out pageCount,
					out recordCount);
			}

			pagingContext.PageCount=pageCount;
			pagingContext.RecordCount=recordCount;

			if (retValue==null)
				retValue=new Trasmissione[0];

			return retValue;
		}
	}
}
