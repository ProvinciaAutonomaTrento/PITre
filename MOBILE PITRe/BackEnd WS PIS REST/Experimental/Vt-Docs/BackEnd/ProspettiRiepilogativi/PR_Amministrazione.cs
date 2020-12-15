using System;

namespace ProspettiRiepilogativi
{
	/// <summary>
	/// Summary description for Amministrazione.
	/// </summary>
	public class PR_Amministrazione
	{
		#region oggetto PR_Amministrazione
		private string _codice;
		private string _descrizione;
		private string _libreria;
		private string _system_id;
		

		public PR_Amministrazione()
		{

		}

		public PR_Amministrazione(string system_id,string codice,string descrizione,string libreria)
		{
			_system_id = system_id;
			_codice = codice;
			_descrizione = descrizione;
			_libreria = libreria;
		}

		#region proprietà

		public string System_id
		{
			get
			{
				return _system_id;
			}
			set
			{
				_system_id = value;
			}
		}

		public string Codice
		{
			get
			{
				return _codice;
			}
			set
			{
				_codice = value;
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

		public string Libreria
		{
			get
			{
				return _libreria;
			}
			set
			{
				_libreria = value;
			}
		}

		

		#endregion


}
	#endregion

		#region oggetto registro

	public class PR_Registro
	{
		private string _system_id;
		private string _codice;
		private string _descrizione;

		

		public PR_Registro()
		{
		}

		public PR_Registro(string system_id,string codice,string descrizione)
		{
			_system_id = system_id;
			_codice = codice;
			_descrizione = descrizione;
		}
		
		#region Proprietà

		public string System_id
		{
			get
			{
				return _system_id;
			}
			set
			{
				_system_id = value;
			}
		}

		public string Codice
		{
			get
			{
				return _codice;
			}
			set
			{
				_codice = value;
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



		#endregion
	}
    
	#endregion

    public class PR_Titolario
    {
        private string _system_id;
        private string _descrizione;
        private string _etTitolario;

        public PR_Titolario()
        { }

        public PR_Titolario(string systemId, string descrizione, string etTitolario)
        {
            _system_id = systemId;
            _descrizione = descrizione;
            _etTitolario = etTitolario;
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

        public string SystemId
        {
            get
            {
                return _system_id;
            }
            set
            {
                _system_id = value;
            }
        }

        public string EtTitolario
        {
            get
            {
                return _etTitolario;
            }
            set
            {
                _etTitolario = value;
            }
        }       

    }
	}
