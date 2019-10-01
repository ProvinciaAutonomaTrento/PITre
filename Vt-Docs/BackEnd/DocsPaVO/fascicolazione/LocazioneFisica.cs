using System;

namespace DocsPaVO.LocazioneFisica
{
	/// <summary>
	/// Summary description for LocazioneFisica.
	/// </summary>
    [Serializable()]
	public class LocazioneFisica
	{
		private string _idUoLF;
		private string _descrizione;
		private string _data; 
		private string _codiceRubrica;

		#region Properties
		public string UO_ID
		{
			get
			{
				return _idUoLF;
			}
			set
			{
				_idUoLF = value;
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

		
		
		public string Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
			}
		}

		public string CodiceRubrica
		{
			get
			{
				return _codiceRubrica;
			}
			set
			{
				_codiceRubrica = value;
			}
		}
		
		#endregion

		public LocazioneFisica()
		{
		}

		public LocazioneFisica(string idUO, string descrizione, string data, string codiceRubrica)
		{
			_idUoLF = idUO;
			_descrizione = descrizione;
			_data = data;
			_codiceRubrica = codiceRubrica;
		}
	}
}

