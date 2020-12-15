using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
	public class RicercaDuplicati : FileRequest 
	{
        public enum EsitoRicercaDuplicatiEnum
        {
            // Ecco i valori possibili di ritorno
            NessunDuplicato,
            ProtocolloNullo,
            NoProtocolloIngresso,
            NoMittente,
            DuplicatiMittenteProtocollo,
            DuplicatiMittenteOggetto,
            DuplicatiMittenteData,
            ErroreGenerico
        }
    }
}