using System;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Classe base astratta per la gestione degli elementi di filtro di ricerca
	/// </summary>
	public abstract class BaseFilterItem
	{
		public BaseFilterItem()
		{
		}

		/// <summary>
		/// Conversione dell'oggetto "BaseFilterItem" in un array di oggetti "FiltroRicerca"
		/// </summary>
		/// <returns></returns>
		public abstract FiltroRicerca[] ToFiltriRicerca();

		/// <summary>
		/// Creazione elemento filtro ricerca
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual FiltroRicerca GetFiltroRicerca(string name,string value)
		{
			FiltroRicerca filtroRicerca=new FiltroRicerca();
			filtroRicerca.argomento=name;
			filtroRicerca.valore=value;
			return filtroRicerca;
		}

		protected bool IsStringFilterSet(string filterValue)
		{
			return (filterValue!=null && filterValue!=string.Empty);
		}

		protected bool IsIntFilterSet(int filterValue)
		{
			return (filterValue>0);
		}
	}
}
