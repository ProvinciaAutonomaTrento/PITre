using System;
using System.Collections;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Elemento di ricerca delle trasmissioni effettuate
	/// </summary>
	public class TrasmissioniEffettuateFilterItem : TrasmissioniFilterItem
	{
		private Corrispondente _destinatario=null;
		private bool _visualizzaTrasmissioniSottoposti=false;

		public TrasmissioniEffettuateFilterItem()
		{
		}

		public Corrispondente Destinatario
		{
			get
			{
				return this._destinatario;
			}
			set
			{
				this._destinatario=value;
			}
		}

		public bool VisualizzaTrasmissioniSottoposti
		{
			get
			{
				return this._visualizzaTrasmissioniSottoposti;
			}
			set
			{
				this._visualizzaTrasmissioniSottoposti=value;
			}
		}

		/// <summary>
		/// Conversione dell'oggetto "TrasmissioniEffettuateFilterItem" in un array di oggetti "FiltroRicerca"
		/// </summary>
		/// <returns></returns>
		public override FiltroRicerca[] ToFiltriRicerca()
		{
			ArrayList list=new ArrayList(base.ToFiltriRicerca());

			if (!this.VisualizzaTrasmissioniSottoposti)
				list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.NO_CERCA_INFERIORI.ToString(),"null"));

			if (this.Destinatario!=null)
				list.Add(this.GetFiltroRicercaDestinatario());

			return (FiltroRicerca[]) list.ToArray(typeof(FiltroRicerca));
		}

		/// <summary>
		/// Creazione elemento di filtro destinatario
		/// </summary>
		/// <returns></returns>
		private FiltroRicerca GetFiltroRicercaDestinatario()
		{
			string filterCode=string.Empty;
			string filterValue=string.Empty;

			if (this.Destinatario.tipoCorrispondente.Equals("P"))
			{
				filterCode=DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_DEST_UTENTE.ToString();
				filterValue=this.Destinatario.codiceRubrica;
			}
			else if (this.Destinatario.tipoCorrispondente.Equals("R"))
			{
				filterCode=DocsPaWR.FiltriTrasmissioneNascosti.COD_RUBR_DEST_RUOLO.ToString();
				filterValue=this.Destinatario.codiceRubrica;
			}
			else if (this.Destinatario.tipoCorrispondente.Equals("U"))
			{
				filterCode=DocsPaWR.FiltriTrasmissioneNascosti.ID_UO_DEST.ToString();
				filterValue=this.Destinatario.systemId;
			}

			return this.GetFiltroRicerca(filterCode,this.Destinatario.systemId);
		}
	}
}
