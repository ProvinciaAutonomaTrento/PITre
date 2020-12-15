using System;

namespace DocsPaVO.documento
{
	/// <summary>
	/// definizione oggetto InfoProtocolloDuplicato
	/// relativo alla funzionalità di ricerca dei duplicati di un protocollo.
	/// </summary>
    [Serializable()]
	public class InfoProtocolloDuplicato
	{
		// data di procollazione del duplicato
		public string dataProtocollo = string.Empty;
		
		// segnatura del protocollo duplicato
		public string segnaturaProtocollo = string.Empty;
		
		// Unità Organizzativa che ha emesso il protocollo duplicato
		public string uoProtocollatore = string.Empty;

        //Profile System_id
        public string idProfile = string.Empty;

        //numero di protocollo
        public string numProto = string.Empty;

        //Documento acquisito
        public string docAcquisito = string.Empty;
	}
}
