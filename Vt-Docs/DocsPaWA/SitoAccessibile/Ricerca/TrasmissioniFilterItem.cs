using System;
using System.Collections;
using DocsPAWA.SitoAccessibile.Documenti.Trasmissioni;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Elemento di ricerca delle trasmissioni
	/// </summary>
	public abstract class TrasmissioniFilterItem : BaseFilterItem
	{
		private TipiOggettiTrasmissioniEnum _tipoOggettoTrasmissione=TipiOggettiTrasmissioniEnum.Tutti;
		private RangeDateFilter _dataTrasmissione=null;
		private RangeDateFilter _dataAccRif=null;
		private RangeDateFilter _dataRisposta=null;
		private string _ragioneTrasmissione=string.Empty;
		private string _noteGenerali=string.Empty;
		private string _noteIndividuali=string.Empty;
		//private TrasmissioniAdditionalFilterItem[] _additionalFilterItems=new TrasmissioniAdditionalFilterItem[0];
		
		public TipiOggettiTrasmissioniEnum TipoOggettoTrasmissione
		{
			get
			{
				return this._tipoOggettoTrasmissione;
			}
			set
			{
				this._tipoOggettoTrasmissione=value;
			}
		}

		public RangeDateFilter DataTrasmissione
		{
			get
			{
				return this._dataTrasmissione;
			}
			set
			{
				this._dataTrasmissione=value;
			}
		}

		public RangeDateFilter DataAccRif
		{
			get
			{
				return this._dataAccRif;
			}
			set
			{
				this._dataAccRif=value;
			}
		}

		public RangeDateFilter DataRisposta
		{
			get
			{
				return this._dataRisposta;
			}
			set
			{
				this._dataRisposta=value;
			}
		}

		public string RagioneTrasmissione
		{
			get
			{
				return this._ragioneTrasmissione;
			}
			set
			{
				this._ragioneTrasmissione=value;
			}
		}

		public string NoteIndividuali
		{
			get
			{
				return this._noteIndividuali;
			}
			set
			{
				this._noteIndividuali=value;
			}
		}

		public string NoteGenerali
		{
			get
			{
				return this._noteGenerali;
			}
			set
			{
				this._noteGenerali=value;
			}
		}

//		public TrasmissioniAdditionalFilterItem[] AdditionalFilters
//		{
//			get
//			{
//				return this._additionalFilterItems;
//			}
//			set
//			{
//				this._additionalFilterItems=value;
//			}
//		}

		/// <summary>
		/// Conversione dell'oggetto "TrasmissioniFilterItem" in un array di oggetti "FiltroRicerca"
		/// </summary>
		/// <param name="filterItem"></param>
		/// <returns></returns>
		public override FiltroRicerca[] ToFiltriRicerca()
		{
			ArrayList list=new ArrayList();
			
			list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.STATO.ToString(),"null"));

			string tipoOggetto=string.Empty;
			if (this.TipoOggettoTrasmissione.Equals(TipiOggettiTrasmissioniEnum.Protocollati))
				tipoOggetto=DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROTOCOLLATI.ToString();
			else if (this.TipoOggettoTrasmissione.Equals(TipiOggettiTrasmissioniEnum.ProtocollatiArrivo))
				tipoOggetto=DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_ARRIVO.ToString();
			else if (this.TipoOggettoTrasmissione.Equals(TipiOggettiTrasmissioniEnum.ProtocollatiPartenza))
				tipoOggetto=DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_PROT_PARTENZA.ToString();
			else if (this.TipoOggettoTrasmissione.Equals(TipiOggettiTrasmissioniEnum.NonProtocollati))
				tipoOggetto=DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_DOC_NON_PROTOCOLLATI.ToString();
			if (tipoOggetto!=string.Empty)
				list.Add(this.GetFiltroRicerca(tipoOggetto,"null"));

			if (this.TipoOggettoTrasmissione==TipiOggettiTrasmissioniEnum.Fascicoli)
				tipoOggetto="F";
			else
				tipoOggetto="D";
			list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString(),tipoOggetto));
			
			if (this.RagioneTrasmissione!=null && this.RagioneTrasmissione!=string.Empty)
				list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.RAGIONE.ToString(),this.RagioneTrasmissione));
			
			if (this.DataTrasmissione!=null)
			{
				if (this.DataTrasmissione.SearchInitDate)
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IL.ToString(),this.DataTrasmissione.InitDateString));
				}
				else
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SUCCESSIVA_AL.ToString(),this.DataTrasmissione.InitDateString));
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_PRECEDENTE_IL.ToString(),this.DataTrasmissione.EndDateString));
				}
			}
			
            /*
			if (this.DataAccRif!=null)
			{
				if (this.DataAccRif.SearchInitDate)
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissione.ACCETTATA_RIFIUTATA_IL.ToString(),this.DataAccRif.InitDateString));
				}
				else
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissione.ACCETTATA_RIFIUTATA_SUCCESSIVA_AL.ToString(),this.DataAccRif.InitDateString));
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissione.ACCETTATA_RIFIUTATA_PRECEDENTE_IL.ToString(),this.DataAccRif.EndDateString));
				}
			}
            */

			if (this.DataRisposta!=null)
			{
				if (this.DataRisposta.SearchInitDate)
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissione.RISPOSTA_IL.ToString(),this.DataRisposta.InitDateString));
				}
				else
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissione.RISPOSTA_SUCCESSIVA_AL.ToString(),this.DataRisposta.InitDateString));
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissione.RISPOSTA_PRECEDENTE_IL.ToString(),this.DataRisposta.EndDateString));
				}
			}

			if (this.NoteIndividuali!=string.Empty)
				list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissione.NOTE_INDIVIDUALI.ToString(),this.NoteIndividuali));

			if (this.NoteGenerali!=string.Empty)
				list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriTrasmissione.NOTE_GENERALI.ToString(),this.NoteGenerali));

//			// Aggiunta parametri addizionali di ricerca
//			foreach (TrasmissioniAdditionalFilterItem item in this.AdditionalFilters)
//				list.Add(this.GetFiltroRicerca(item.FilterName,item.FilterValue));

			return (FiltroRicerca[]) list.ToArray(typeof(FiltroRicerca));
		}

		public TrasmissioniFilterItem()
		{
		}
	}

	/// <summary>
	/// Enumeration, identifica le tipologie di oggetti 
	/// per i quali ricercare le trasmissioni
	/// </summary>
	public enum TipiOggettiTrasmissioniEnum
	{
		Tutti,
		Protocollati,
		ProtocollatiArrivo,
		ProtocollatiPartenza,
		NonProtocollati,
		Fascicoli
	}
//
//	/// <summary>
//	/// Elemento di filtro aggiuntivo per le trasmissioni
//	/// </summary>
//	public class TrasmissioniAdditionalFilterItem
//	{
//		private string _filterName=string.Empty;
//		private string _filterValue=string.Empty;
//
//		public TrasmissioniAdditionalFilterItem(string filterName,string filterValue)
//		{
//			this._filterName=filterName;
//			this._filterValue=filterValue;
//		}
//
//		public string FilterName
//		{
//			get
//			{
//				return this._filterName;
//			}
//		}
//
//		public string FilterValue
//		{
//			get
//			{
//				return this._filterValue;
//			}
//		}
//	}
}