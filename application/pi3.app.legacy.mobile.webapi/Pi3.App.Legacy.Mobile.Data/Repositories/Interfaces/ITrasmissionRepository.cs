// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;

using MODELS = Pi3.App.Legacy.Mobile.Models;

public interface ITrasmissionRepository
{
    Task<MODELS.SelectTemplates.TrasmissioneJoinSingolaRagioneUtente?> Get_TrasmissioneSignola_By_IdTrasmissioneBy_IdUtente_Async(
          long id,
          long peopleId,
          CancellationToken cancellationToken );

}
