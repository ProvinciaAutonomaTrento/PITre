using System;
using System.Collections;

namespace ProspettiRiepilogativi
{
	/// <summary>
	/// Summary description for Report.
	/// </summary>
	public class Report
	{
		private string _descrizione;
		private string _valore;
		private ArrayList _subReport = new ArrayList();

		#region Proprietà
		/// <summary>
		/// Descrizione - Titolo mostrato a video -
		/// </summary>
		public string Descrizione
		{
			get
			{
				return _descrizione;
			}
			set
			{
				_descrizione = value;
			}
		}

		/// <summary>
		/// Valore - Identificativo del Report -
		/// </summary>
		public string Valore
		{
			get
			{
				return _valore;
			}
			set
			{
				_valore = value;
			}
		}

		/// <summary>
		/// SubReports - Elenco dei sottoReport -
		/// </summary>
		public ArrayList SubReports
		{
			get
			{
				return _subReport;
			}
			set
			{
				_subReport = value;
			}
		}
		#endregion

		#region Costruttori
		public Report()
		{
		}

		/// <summary>
		/// Report
		/// </summary>
		/// <param name="descrizione">Descrizione</param>
		/// <param name="valore">Valore</param>
		public Report(string descrizione, string valore)
		{
			_descrizione = descrizione;
			_valore = valore;
		}
		#endregion

		#region Metodi Pubblici
		/// <summary>
		/// DO_AddSubReport
		/// </summary>
		/// <param name="report">SubReport da aggiungere</param>
		public void DO_AddSubReport(Report report)
		{
			_subReport.Add(report);
		}
		#endregion
	}
}
