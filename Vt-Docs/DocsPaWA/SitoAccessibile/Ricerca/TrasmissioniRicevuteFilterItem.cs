using System;
using System.Collections;
using DocsPAWA.DocsPaWR;


namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Elemento di ricerca delle trasmissioni ricevute
	/// </summary>
	public class TrasmissioniRicevuteFilterItem : TrasmissioniFilterItem
	{
//		private string _descrizioneMittente=string.Empty;
		private Corrispondente _mittente=null;

		public TrasmissioniRicevuteFilterItem()
		{
		}

//		public string DescrizioneMittente
//		{
//			get
//			{
//				return this._descrizioneMittente;
//			}
//			set
//			{
//				this._descrizioneMittente=value;
//			}
//		}

		public Corrispondente Mittente
		{
			get
			{
				return this._mittente;
			}
			set
			{
				this._mittente=value;
			}
		}

		/// <summary>
		/// Conversione dell'oggetto "TrasmissioniRicevuteFilterItem" in un array di oggetti "FiltroRicerca"
		/// </summary>
		/// <param name="filterItem"></param>
		/// <returns></returns>
		public override FiltroRicerca[] ToFiltriRicerca()
		{
			ArrayList list=new ArrayList(base.ToFiltriRicerca());
			
			list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.NO_CERCA_INFERIORI.ToString(),"null"));

			if (this.Mittente!=null)
				list.Add(this.GetFiltroRicercaMittente());

			return (FiltroRicerca[]) list.ToArray(typeof(FiltroRicerca));
		}

		/// <summary>
		/// Creazione elemento di filtro mittente
		/// </summary>
		/// <returns></returns>
		private FiltroRicerca GetFiltroRicercaMittente()
		{
			string filterCode=string.Empty;
			string filterValue=string.Empty;

			if (this.Mittente.tipoCorrispondente.Equals("P"))
			{
				filterCode=DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_UTENTE.ToString();
				filterValue=this.Mittente.codiceRubrica;
			}
			else if (this.Mittente.tipoCorrispondente.Equals("R"))
			{
				filterCode=DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_MITT_RUOLO.ToString();
				filterValue=this.Mittente.codiceRubrica;
			}
			else if (this.Mittente.tipoCorrispondente.Equals("U"))
			{
				filterCode=DocsPaWR.FiltriTrasmissioneNascosti.ID_UO_MITT.ToString();
				filterValue=this.Mittente.systemId;
			}

			return this.GetFiltroRicerca(filterCode,filterValue);
		}
	}
}
