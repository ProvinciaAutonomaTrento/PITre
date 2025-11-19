// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
