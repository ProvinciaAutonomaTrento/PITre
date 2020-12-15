using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class Documento : FileRequest 
	{
        /// <summary>
        /// Costante che identifica un tipo documento come stampa registro
        /// </summary>
        public const string STAMPA_REGISTRO = "STAMPA_REGISTRO";

		public string daInviare;
		public string dataArrivo;
		public TipologiaCanale tipologia;

        /// <summary>
        /// Data di archiviazione del documento in fascicolo cartaceo
        /// </summary>
        public string dataArchiviazione;
	}
}