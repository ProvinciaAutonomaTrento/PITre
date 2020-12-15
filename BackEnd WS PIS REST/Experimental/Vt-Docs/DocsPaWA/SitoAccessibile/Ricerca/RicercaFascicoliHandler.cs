using System;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Paging;
using DocsPAWA.SitoAccessibile.Documenti.Classificazioni;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Gestione della logica di ricerca dei fascicoli
	/// </summary>
	public sealed class RicercaFascicoliHandler
	{
		private RicercaFascicoliHandler()
		{
		}

		/// <summary>
		/// Filtro correntemente impostato
		/// </summary>
		public static FascicoliFilterItem CurrentFilter
		{
			get
			{
				if (System.Web.HttpContext.Current.Session["FascicoliFilterItem"]!=null)
					return System.Web.HttpContext.Current.Session["FascicoliFilterItem"] as FascicoliFilterItem;
				else
					return null;
			}
			set
			{
				System.Web.HttpContext.Current.Session["FascicoliFilterItem"]=value;
			}
		}

		/// <summary>
		/// Ricerca fascicoli
		/// </summary>
		/// <param name="filterItem"></param>
		/// <param name="pagingContext"></param>
		/// <returns></returns>
		public static Fascicolo[] SearchFascicoli(FascicoliFilterItem filterItem,PagingContext pagingContext)
		{	
			FiltroRicerca[] filters=filterItem.ToFiltriRicerca();
			
			int pageCount;
			int recordCount;


			Utente user=UserManager.getUtente();
			InfoUtente infoUtente=UserManager.getInfoUtente();

			Registro registro=null;
			foreach (Registro item in UserManager.getRuolo().registri)
			{
				if (item.systemId.Equals(filterItem.IDRegistro))
				{
					registro=item;
					break;
				}
			}

			FascicolazioneClassificazione classificazione=null;

			if (filterItem.CodiceNodoTitolario!=string.Empty)
			{
				ClassificaHandler classificaHandler=new ClassificaHandler();
				classificazione=classificaHandler.GetClassificazione(filterItem.CodiceNodoTitolario,registro);
			}

			DocsPaWebService ws=new DocsPaWebService();

            // Lista dei system id dei fascicoli. Non utilizzata
            SearchResultInfo[] idProjects = null;

            Fascicolo[] retValue=ws.FascicolazioneGetListaFascicoliPaging(
							infoUtente,
							classificazione,
							registro,
							filters,
							false,
							false,
                            false,
							pagingContext.PageNumber,
							pagingContext.PageSize,
                            false,
                             null,
                            out pageCount,
                            out recordCount,
							out idProjects);

            
			pagingContext.PageCount=pageCount;
			pagingContext.RecordCount=recordCount;

			if (retValue==null)
				retValue=new Fascicolo[0];

			return retValue;
		}
	}
}
