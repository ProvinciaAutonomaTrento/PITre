using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class Firmatario 
	{
		public string systemId;
		public string nome;
		public string cognome;
		public string codiceFiscale;
		public string dataNascita;
		public string identificativoCA;
		public int livello;
	}
}