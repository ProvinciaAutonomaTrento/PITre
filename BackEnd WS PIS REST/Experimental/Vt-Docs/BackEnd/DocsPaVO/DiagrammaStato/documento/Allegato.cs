using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class Allegato : FileRequest 
	{
		public int numeroPagine;

        /// <summary>
        /// Posizione di inserimento dell'allegato nell'ambito del suo documento principale
        /// </summary>
        public int position;

        /// <summary>
        /// Identificativo del documento da cui è stato ottenuto questo documento tramite la funzionalità di Inoltro
        /// </summary>
        public String ForwardingSource { get; set; }
	}
}
