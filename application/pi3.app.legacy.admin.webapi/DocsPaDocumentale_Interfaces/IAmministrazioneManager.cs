// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaDocumentale.Interfaces;

/// <summary>
/// Interfaccia per la gestione dell'amministrazione nel documentale
/// </summary>
public interface IAmministrazioneManager
{
    /// <summary>
    /// Inserimento di una nuova amministrazione nel documentale
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    EsitoOperazione Insert(InfoAmministrazione info);

    /// <summary>
    /// Aggiornamento di un'amministrazione esistente nel documentale
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    EsitoOperazione Update(InfoAmministrazione info);

    /// <summary>
    /// Cancellazione di un'amministrazione nel documentale
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    EsitoOperazione Delete(InfoAmministrazione info);
}
