using System;
using System.Xml.Serialization;

namespace DocsPaVO.filtri.stampaRegistro 
{
	/// <summary>
	/// </summary>
	[XmlType("FiltriStampaRegistro")]
	public enum listaArgomenti 
	{
		REGISTRO,
		TIPO,
		NUM_PROTOCOLLO_STAMPA,
        NUM_PROTOCOLLO_STAMPA_DAL,
        NUM_PROTOCOLLO_STAMPA_AL,
        ANNO_PROTOCOLLO_STAMPA,
		DATA_STAMPA_REGISTRO,
		DATA_STAMPA_REGISTRO_DAL,
		DATA_STAMPA_REGISTRO_AL,
        ORDER_FILTER,        
        ID_REPERTORIO,
        NUM_REP_STAMPA,
        NUM_REP_STAMPA_DAL,
        NUM_REP_STAMPA_AL,
        ANNO_REP_STAMPA,
        DATA_STAMPA_REP,
        DATA_STAMPA_REP_DAL,
        DATA_STAMPA_REP_AL,
        REP_FIRMATO    
    }
}
