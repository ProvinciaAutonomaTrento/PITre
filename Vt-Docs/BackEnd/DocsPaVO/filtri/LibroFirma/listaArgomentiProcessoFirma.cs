using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.filtri.LibroFirma
{
    [XmlType("FiltriProcessoFirma")]
    public enum listaArgomentiProcessoFirma
    {
        TIPO_MODELLO, //Processi di firma per cui non è definito almeno in un passo un ruolo coinvolto
        TIPO_PROCESSO, //Processi di firma per cui in tutti i suoi passi è definito il ruolo coinvolto
        VALIDO,
        INVALIDO,
        NOME,
        RUOLO_COINVOLTO,
        UTENTE_COINVOLTO,
        ORDER_FIELD,
        ORDER_DIRECTION,
        ID_RUOLO_VISIBILITA,
        COD_RUOLO_VISIBILITA,
        DESC_RUOLO_VISIBILITA

    }
    
    public enum OrderBy
    {
        ID_PROCESSO,
        NOME,
        DTA_CREAZIONE
    }
}
