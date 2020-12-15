using System;
using System.Collections;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Fascicoli;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Elemento di ricerca dei fascicoli
	/// </summary>
	public class FascicoliFilterItem : BaseFilterItem
	{
		private string _idRegistro=string.Empty;
		private string _codiceNodoTitolario=string.Empty;
		private RangeDateFilter _dataApertura=null;
		private RangeDateFilter _dataChiusura=null;
		private RangeDateFilter _dataCreazione=null;
		private RangeDateFilter _dataCollocazione=null;
		private int _numero=0;
		private int _anno=0;
		private string _descrizione=string.Empty;
		private string _tipo=string.Empty;
		private string _stato=string.Empty;
		private Corrispondente _corrispondenteLocazioneFisica=null;

		public string IDRegistro
		{
			get
			{
				return this._idRegistro;
			}
			set
			{
				this._idRegistro=value;
			}
		}

		public string CodiceNodoTitolario
		{
			get
			{
				return this._codiceNodoTitolario;
			}
			set
			{
				this._codiceNodoTitolario=value;
			}
		}

		public RangeDateFilter DataApertura
		{
			get
			{
				return this._dataApertura;
			}
			set
			{
				this._dataApertura=value;
			}
		}

		public RangeDateFilter DataChiusura
		{
			get
			{
				return this._dataChiusura;
			}
			set
			{
				this._dataChiusura=value;
			}
		}

		public RangeDateFilter DataCreazione
		{
			get
			{
				return this._dataCreazione;
			}
			set
			{
				this._dataCreazione=value;
			}
		}

		public RangeDateFilter DataCollocazione
		{
			get
			{
				return this._dataCollocazione;
			}
			set
			{
				this._dataCollocazione=value;
			}
		}

		public int Numero
		{
			get
			{
				return this._numero;
			}
			set
			{
				this._numero=value;
			}
		}

		public int Anno
		{
			get
			{
				return this._anno;
			}
			set
			{
				this._anno=value;
			}
		}

		public string Descrizione
		{
			get
			{
				return this._descrizione;
			}
			set
			{
				this._descrizione=value;
			}
		}

		public string Tipo
		{
			get
			{
				return this._tipo;
			}
			set
			{
				this._tipo=value;
			}
		}

		public string Stato
		{
			get
			{
				return this._stato;
			}
			set
			{
				this._stato=value;
			}
		}

		public Corrispondente CorrispondenteLocazioneFisica
		{
			get
			{
				return this._corrispondenteLocazioneFisica;
			}
			set
			{
				this._corrispondenteLocazioneFisica=value;
			}
		}

		public FascicoliFilterItem()
		{
		}

		/// <summary>
		/// Conversione dell'oggetto "FascicoliFilterItem" in un array di oggetti "FiltroRicerca"
		/// </summary>
		/// <param name="filterItem"></param>
		/// <returns></returns>
		public override FiltroRicerca[] ToFiltriRicerca()
		{
			ArrayList list=new ArrayList();
			
			if (this.IsStringFilterSet(this.Tipo))
				list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.TIPO_FASCICOLO.ToString(),this.Tipo));

			if (this.IsStringFilterSet(this.Stato))
				list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.STATO.ToString(),this.Stato));

			if (this.IsIntFilterSet(this.Numero))
				list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.NUMERO_FASCICOLO.ToString(),this.Numero.ToString()));

			if (this.IsIntFilterSet(this.Anno))
				list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.ANNO_FASCICOLO.ToString(),this.Anno.ToString()));
			
			if (this.DataApertura!=null)
			{
				if (this.DataApertura.SearchInitDate)
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.APERTURA_IL.ToString(),this.DataApertura.InitDateString));
				}
				else
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.APERTURA_SUCCESSIVA_AL.ToString(),this.DataApertura.InitDateString));
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.APERTURA_PRECEDENTE_IL.ToString(),this.DataApertura.EndDateString));
				}
			}

			if (this.DataChiusura!=null)
			{
				if (this.DataChiusura.SearchInitDate)
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.CHIUSURA_IL.ToString(),this.DataChiusura.InitDateString));
				}
				else
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.CHIUSURA_SUCCESSIVA_AL.ToString(),this.DataChiusura.InitDateString));
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.CHIUSURA_PRECEDENTE_IL.ToString(),this.DataChiusura.EndDateString));
				}
			}

			if (this.DataCreazione!=null)
			{
				if (this.DataCreazione.SearchInitDate)
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.CREAZIONE_IL.ToString(),this.DataCreazione.InitDateString));
				}
				else
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.CREAZIONE_SUCCESSIVA_AL.ToString(),this.DataCreazione.InitDateString));
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.CREAZIONE_PRECEDENTE_IL.ToString(),this.DataCreazione.EndDateString));
				}
			}

			if (this.DataCollocazione!=null)
			{
				if (this.DataCollocazione.SearchInitDate)
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.DATA_LF_IL.ToString(),this.DataCollocazione.InitDateString));
				}
				else
				{
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.DATA_LF_SUCCESSIVA_AL.ToString(),this.DataCollocazione.InitDateString));
					list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.DATA_LF_PRECEDENTE_IL.ToString(),this.DataCollocazione.EndDateString));
				}
			}

			if (IsStringFilterSet(this.Descrizione))
				list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.TITOLO.ToString(),this.Descrizione));

			if (this.CorrispondenteLocazioneFisica!=null)
				list.Add(this.GetFiltroRicerca(DocsPAWA.DocsPaWR.FiltriFascicolazione.ID_UO_LF.ToString(),this.CorrispondenteLocazioneFisica.systemId));

			return (FiltroRicerca[]) list.ToArray(typeof(FiltroRicerca));
		}
	}
}
