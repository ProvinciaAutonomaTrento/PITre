using System;

namespace ProspettiRiepilogativi.Frontend
{
	/// <summary>
	/// Parametro, oggetto da passare alle 
	/// StoredProcedure.
	/// </summary>
	public class Parametro
	{
		private string _descrizione;
		private string _valore;
		private string _nome;

		#region Proprietà
		public string Nome
		{
			get
			{
				return _nome;
			}
			set
			{
				_nome = value;
			}
		}

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
		#endregion

		/// <summary>
		/// Parametro, overload del parametro.
		/// </summary>
		public Parametro()
		{
		}

		/// <summary>
		/// Parametro.
		/// </summary>
		/// <param name="descrizione">descrizione: nome del parametro</param>
		/// <param name="valore">valore: valore del parametro</param>
		public Parametro(string descrizione, string valore)
		{
			_descrizione = descrizione;
			_valore = valore;
		}

		/// <summary>
		/// Parametro.
		/// </summary>
		/// <param name="descrizione">descrizione: nome del parametro</param>
		/// <param name="valore">valore: valore del parametro</param>
		/// <param name="nome">nome: nome del parametro, serve quando
		/// usato come filtro di ricerca</param>
		public Parametro(string nome,string descrizione, string valore)
		{
			_nome = nome;
			_descrizione = descrizione;
			_valore = valore;
		}
	}
}
