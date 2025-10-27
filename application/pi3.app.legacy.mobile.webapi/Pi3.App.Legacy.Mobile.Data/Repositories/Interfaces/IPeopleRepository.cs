// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;

using Serilog;
using MODELS = Pi3.App.Legacy.Mobile.Models;

public interface IPeopleRepository
{
    Task<MODELS.SelectTemplates.UserDetails?> GetUserInformationForClaims(
        string userId,
        long idAmministrazione,
        long idGroup,
        CancellationToken cancellationToken );

    Task<IEnumerable<string>> GetUserFunctions(
        long IdRuoloInUo,
        CancellationToken cancellationToken );

    Task<bool> CheckIfUserIsValidAsync(
        string userId,
        CancellationToken cancellationToken );

    Task<MODELS.SelectTemplates.Utente?> GetUtenteByUserIdAndIdAdminAsync( 
        string userId, 
        long idAmministrazione, 
        CancellationToken cancellationToken );

    Task<MODELS.SelectTemplates.Utente?> GetUtenteByUserIdAndIdAdminForLoginAsync(
        string userId,
        long idAmministrazione,
        CancellationToken cancellationToken );

    Task<MODELS.SelectTemplates.Utente?> GetUtenteByIdAsync(
        long id,
        CancellationToken cancellationToken );

    Task<IEnumerable<long>> GetIdAmministrazioniUtenteByUserIdAsync(
        string userId,
        CancellationToken cancellationToken );

}
