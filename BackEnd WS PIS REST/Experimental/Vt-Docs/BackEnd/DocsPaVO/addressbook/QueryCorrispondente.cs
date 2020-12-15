using System;
using System.Xml.Serialization;

namespace DocsPaVO.addressbook
{
	/// <summary>
	/// </summary>
    [XmlType("AddressbookQueryCorrispondente")]
	public class QueryCorrispondente
	{
		public string systemId;
		public string codiceRubrica;
		public string codiceGruppo;
		public string descrizioneGruppo;
		public string codiceUO;
		public string descrizioneUO;
		public string codiceRuolo;
		public string descrizioneRuolo;
		public string nomeUtente;
		public string cognomeUtente;
		public string idAmministrazione;
		public bool getChildren;
		public bool fineValidita;
		public TipoUtente tipoUtente;
        public string email;
		
		[XmlArray()]
		[XmlArrayItem(typeof(string))]
		public System.Collections.ArrayList idRegistri;

		/// <summary>
		/// </summary>
		public QueryCorrispondente()
		{
			idRegistri=new System.Collections.ArrayList();
		}

		/// <summary>
		/// </summary>
		public bool isUODefined()
		{
			if(this.codiceUO!=null||this.descrizioneUO!=null)
			{
				return true;
			}
			else
			{
			    return false;
			}
		}

		/// <summary>
		/// </summary>
		public bool isRuoloDefined()
		{
			if(this.codiceRuolo!=null||this.descrizioneRuolo!=null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// </summary>
		public bool isUtenteDefined()
		{
			if(this.cognomeUtente!=null||this.nomeUtente!=null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
