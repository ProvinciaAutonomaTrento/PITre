// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MODELS = Pi3.App.Legacy.Mobile.Models;

namespace Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
public interface IProfileRepository
{
    Task<MODELS.SelectTemplates.DettagliDocumento?> GetDocumentoUltimaVersioneByDocNumberAsync(
        long docNumber,
        CancellationToken cancellationToken );

    Task<IEnumerable<MODELS.SelectTemplates.DettagliDocumento>> GetAllegatiDocumentoUltimaVersioneByIdDocumentoPrincipaleAsync(
        long id,
        CancellationToken cancellationToken );

    Task<MODELS.SelectTemplates.DettagliProtocollo?> GetDettagliProtocolloInArrivoAsync( 
        long idProfile, 
        CancellationToken cancellationToken );

    Task<IEnumerable<MODELS.SelectTemplates.DettagliProtocollo>?> GetDettagliProtocolloInUscitaAsync(
        long idProfile, 
        CancellationToken cancellationToken );

}
