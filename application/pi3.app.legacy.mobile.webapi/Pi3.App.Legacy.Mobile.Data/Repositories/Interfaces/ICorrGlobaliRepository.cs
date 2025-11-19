// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MODELS = Pi3.App.Legacy.Mobile.Models;

namespace Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;

public interface ICorrGlobaliRepository
{
    Task<MODELS.SelectTemplates.Ruolo?> GetRuoloByIdCorGlobali( long idCorrGlobali, CancellationToken cancellationToken );
}
