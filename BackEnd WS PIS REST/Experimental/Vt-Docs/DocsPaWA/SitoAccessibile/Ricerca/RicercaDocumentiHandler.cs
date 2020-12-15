using System;
using System.Web;
using System.Web.UI;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Paging;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Gestione della logica di ricerca dei fascicoli
	/// </summary>
	public class RicercaDocumentiHandler
	{
		private const string SESSION_KEY="SearchProperties";

		/// <summary>
		/// Reperimento / impostazione filtro di ricerca documenti corrente
		/// </summary>
		public static SearchProperties CurrentFilter
		{
			get
			{
				if (HttpContext.Current.Session[SESSION_KEY]!=null)
					return HttpContext.Current.Session[SESSION_KEY] as SearchProperties;
				else
					return null;
			}
			set
			{
				HttpContext.Current.Session[SESSION_KEY]=value;
			}
		}

		public RicercaDocumentiHandler()
		{
		}

		/// <summary>
		/// Reperimento dei documenti per il contesto di paginazione richiesto
		/// </summary>
		/// <param name="filters"></param>
		/// <param name="pagingContext"></param>
		/// <returns></returns>
		public InfoDocumento[] GetDocumenti(FiltroRicerca[] filters,PagingContext pagingContext)
		{
			InfoDocumento[] documenti=null;

			int pageCount;
			int recordCount;

            // Array degli idProfile dei documenti restituiti dalla ricerca
            SearchResultInfo[] idProfilesList = new SearchResultInfo[0];

			DocsPaWR.InfoUtente infoUtente=UserManager.getInfoUtente();
            
			DocsPaWebService ws=new DocsPaWebService();
            documenti = ws.DocumentoGetQueryDocumentoPaging(infoUtente.idGruppo,
                                                            infoUtente.idPeople,
                                                            new FiltroRicerca[1][] { filters }, 
                                                            false,
                                                            false,
                                                            pagingContext.PageNumber,
                                                            true,
                                                            false,
                                                            out pageCount,
                                                            out recordCount,
                                                            out idProfilesList);

			if (documenti==null)
				documenti=new DocsPAWA.DocsPaWR.InfoDocumento[0];

			pagingContext.PageCount=pageCount;
			pagingContext.RecordCount=recordCount;

            if (idProfilesList != null)
            {
                // Salavtaggio della lista di idProfile dei documenti individuati
                pagingContext.IdProfilesList = new string[idProfilesList.Length];
                for (int i = 0; i < idProfilesList.Length; i++) 
                    pagingContext.IdProfilesList[i] = idProfilesList[i].Id;
            }

			return documenti;
		}
	}
}
