using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class FirmaResult
	{
		public FileRequest fileRequest;
        public EsitoFirma esito;
        public string errore;
	}

    [Serializable()]
    public class EsitoFirma
    {
        public string Id;
        public string Codice;
        public string Messaggio;
    }
}