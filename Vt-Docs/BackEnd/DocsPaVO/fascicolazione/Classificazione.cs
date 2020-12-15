using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.fascicolazione
{	
	/// <summary>
	/// </summary>
	[XmlType("FascicolazioneClassificazione")]
    [Serializable()]
	public class Classificazione 
	{
		public ArrayList childs;
		public string codice;
		public string descrizione;
		public string systemID;
		public string accessRights;
		public DocsPaVO.utente.Registro registro;
		public string codUltimo;
		public string livello;
		public string varcodliv1;
		public string idRegistroNodoTit; //per fascicoli procedimentali e ricerca tramite registro(ADL)
		public string codiceRegistroNodoTit;
        public string idTipoFascicolo=string.Empty;
        public string bloccaTipoFascicolo = string.Empty;
		/// <summary>
		/// </summary>
		public Classificazione()
		{
		   childs = new ArrayList();
		}
	}
}
