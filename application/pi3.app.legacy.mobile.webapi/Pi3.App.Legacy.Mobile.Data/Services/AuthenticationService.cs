// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;
using SERVICE_REQUESTS = Pi3.App.Legacy.Mobile.Models.ServiceRequests;
using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;
using SELECT_TEMPLATES = Pi3.App.Legacy.Mobile.Models.SelectTemplates;


namespace Pi3.App.Legacy.Mobile.Data.Services;
public class AuthenticationService( 
    ILogger<AuthenticationService> logger,
    IMapper mapper,
    IPeopleRepository peopleRepository,
    IAdministrationRepository administrationRepository) : IAuthenticationService
{
    readonly ILogger<AuthenticationService> _logger = logger;
    readonly IMapper _mapper = mapper;
    readonly IPeopleRepository _peopleRepository = peopleRepository;
    readonly IAdministrationRepository _administrationRepository = administrationRepository;

    public async Task<SERVICE_DTO.AuthenticationResult> AuthenticateUser( 
        SERVICE_REQUESTS.AuthenticateUserRequest request, 
        CancellationToken cancellationToken )
    {
        // reminder SSO:
        // l'SSO include il session id che non viene utilizzato sul codice origiale, restituisce solo l'userID
        // l'utenza di dominio sul nuovo ambiente cluod non può funzionare perchè utilizza
        // WindowsIdentity wi = WindowsIdentity.GetCurrent();
        // ma sui container linux non può funzionare

        EXCEPTIONS.Internal.LoginResponseCode loginResult = EXCEPTIONS.Internal.LoginResponseCode.OK;
        SELECT_TEMPLATES.Utente? utente;
        try
        {
            if ( !(await this._peopleRepository.CheckIfUserIsValidAsync(request.Username, cancellationToken)) )
            {
                this._logger.LogError("Utente non trovato");
                throw new EXCEPTIONS.Internal.LoginException(EXCEPTIONS.Internal.LoginResponseCode.USER_NOT_FOUND);
            }

            if( !request.IdAmministrazione.HasValue )
            {
                IEnumerable<long> idAmministrazioni 
                    = await this._peopleRepository.GetIdAmministrazioniUtenteByUserIdAsync(request.Username, cancellationToken);

                if ( !idAmministrazioni.Any() )
                {
                    this._logger.LogError("Nessuna amministrazione trovata per l'utente");
                    throw new EXCEPTIONS.Internal.LoginException(EXCEPTIONS.Internal.LoginResponseCode.SYSTEM_ERROR);
                }

                request.IdAmministrazione = idAmministrazioni.Count() switch
                {
                    1 => (long?)idAmministrazioni.First(),
                    _ => throw new EXCEPTIONS.Internal.LoginException(EXCEPTIONS.Internal.LoginResponseCode.MULTIAMM),
                };
            }



            utente = await this._peopleRepository.GetUtenteByUserIdAndIdAdminForLoginAsync(
                request.Username,
                request.IdAmministrazione!.Value,
                cancellationToken)
                    ?? throw new EXCEPTIONS.Internal.LoginException(EXCEPTIONS.Internal.LoginResponseCode.USER_NOT_FOUND);

            if ( utente!.Disabled?.Equals("Y", StringComparison.CurrentCultureIgnoreCase) ?? false )
            {
                this._logger.LogWarning("Utente disabilitato");
                throw new EXCEPTIONS.Internal.LoginException(EXCEPTIONS.Internal.LoginResponseCode.SYSTEM_ERROR);
            }

            string encryptedRequestPassword = Shared.Helpers.AuthenticationHelper.CalcolaImpronta(request.Password);
            bool test = "CBA4E545B7EC918129725154B29F055E4CD5AEA8" == encryptedRequestPassword;

            if ( !encryptedRequestPassword.Equals(utente.EncryptedPassword) )
            {
                throw new EXCEPTIONS.Internal.LoginException(EXCEPTIONS.Internal.LoginResponseCode.USER_NOT_FOUND);
            }

            SELECT_TEMPLATES.AdministrationPasswordSettings? administrationPasswordSettings
                = await this._administrationRepository.GetAdministrationPasswordSettingsAsync(
                    request.IdAmministrazione!.Value, cancellationToken);

            if ( administrationPasswordSettings?.IsPasswordExpirationEnabled?.Equals("1") ?? false )
            {
                // ToDo gestisci password scaduta, gestisci i return code di errore
                if ( utente.UserType?.Equals("0") ?? true ) // utente
                {
                    if ( !utente.PasswordCreationDate.HasValue || utente.PasswordCreationDate.Equals(DateTime.MinValue) )
                    {
                        throw new EXCEPTIONS.Internal.LoginException(EXCEPTIONS.Internal.LoginResponseCode.PASSWORD_EXPIRED);
                    }

                    if ( Double.TryParse(administrationPasswordSettings.PasswordExpirationDays, out double validityDays)
                        & utente.PasswordNeverExpire?.Equals("0") ?? true )
                    {

                        if ( DateTime.Today > utente.PasswordCreationDate?.Date.AddDays(validityDays) )
                        {
                            throw new EXCEPTIONS.Internal.LoginException(EXCEPTIONS.Internal.LoginResponseCode.PASSWORD_EXPIRED);
                        }
                    }
                }
            }
        } 
        catch ( EXCEPTIONS.Internal.LoginException ex )
        {
            utente = null;
            loginResult = ex.Code;
        }
        catch ( Exception ) { throw; }

        SERVICE_DTO.AuthenticationResult authenticationResult;
        if (loginResult == EXCEPTIONS.Internal.LoginResponseCode.OK )
        {
            authenticationResult = this._mapper.Map<SERVICE_DTO.AuthenticationResult>(utente);
            authenticationResult.Dst = Guid.NewGuid().ToString("N");
        } 
        else
        {
            authenticationResult = new();
        }

        authenticationResult.Code = loginResult;
        

        return authenticationResult;
    }

}
