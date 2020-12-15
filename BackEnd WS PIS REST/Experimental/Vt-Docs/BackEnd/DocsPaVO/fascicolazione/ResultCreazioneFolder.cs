using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.fascicolazione
{
    /// <summary>
    /// Esito della creazione di un sottofascicolo
    /// </summary>
    public enum ResultCreazioneFolder
    {
        OK,
        FOLDER_EXIST,
        DM_ERROR,       // Errore nel sw documentale
        GENERIC_ERROR
    }
}
