// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static System.Net.Mime.MediaTypeNames;

using SERVICE_REQUEST = Pi3.App.Legacy.Mobile.Models.ServiceRequests;
using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;
using SELECT_TEMPLATES = Pi3.App.Legacy.Mobile.Models.SelectTemplates;
using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;


namespace Pi3.App.Legacy.Mobile.Data.Services;

/**
 * 
 * ATTENZIONE !!!
 * Codice non ottimizzato
 * Riportato da Pi3.App.Legacy.WebApi
 *
*/
public class FascicoloService(
    ILogger<FascicoloService> logger,
    IClaimsPrincipalService claimsPrincipalService,
    IPi3DbContext pi3DbContext ) : IFascicoloService
{
    private readonly ILogger<FascicoloService> _logger = logger;
    private readonly IClaimsPrincipalService _claimsPrincipalService = claimsPrincipalService;
    private readonly IPi3DbContext _dbContext = pi3DbContext;


    public async Task<SERVICE_DTO.Fascicolo> GetFascicoloById(long idFascicolo, CancellationToken cancellationToken = default)
    {
        SERVICE_DTO.Fascicolo? result = null;
        try
        {
            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser).ToString();
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();

            var rights = await _dbContext.GetSecurityRights(idFascicolo.ToString(), idUser, idGroup);
            var rigthsAsLong = Convert.ToInt32(rights);

            var f = await _dbContext.ProjectEntities.Where(x => x.SYSTEM_ID == idFascicolo && rigthsAsLong > 0 && x.CHA_TIPO_PROJ.Equals("F"))
                .Select(x => new
                {
                    x.SYSTEM_ID,
                    x.DESCRIPTION,
                    x.CHA_TIPO_PROJ,
                    x.VAR_CODICE,
                    x.ID_AMM,
                    x.NUM_LIVELLO,
                    x.CHA_TIPO_FASCICOLO,
                    x.ID_FASCICOLO,
                    x.ID_PARENT,
                    x.VAR_COD_ULTIMO,
                    x.VAR_NOTE,
                    x.DTA_APERTURA,
                    x.DTA_CHIUSURA,
                    x.CHA_STATO,
                    x.ID_TIPO_PROC,
                    x.ID_REGISTRO,
                    x.ID_UO_LF,
                    x.DTA_UO_LF,
                    x.DTA_CREAZIONE,
                    x.CHA_IN_ARCHIVIO,
                    ACCESSRIGTHS = GetAccessRigths(rights),
                    x.AUTHOR,
                    x.ID_RUOLO_CREATORE,
                    x.ID_UO_CREATORE,
                    x.CHA_CONTROLLATO,
                    x.CHA_COD_T_A,
                    x.COD_EXT_APP,
                    x.ID_TITOLARIO,
                    DTA_SCADENZA = x.DTA_SCADENZA.HasValue ? x.DTA_SCADENZA.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : String.Empty,
                    x.CHA_PUBBLICO,
                    x.ID_PIANO_CONSERVAZIONE,
                    PRIVATO = x.CHA_PRIVATO

                })
                .Distinct()
                .FirstOrDefaultAsync(cancellationToken);

            if ( f != null )
            {
                result = new()
                {
                    IdFasc = f.SYSTEM_ID.ToString(),
                    Codice = f.VAR_CODICE?.ToString(),
                    Descrizione = f.DESCRIPTION?.ToString(),
                    Note = f.VAR_NOTE,
                    DataApertura = f.DTA_APERTURA,
                    DataChiusura = f.DTA_CHIUSURA,
                    AccessRights = f.ACCESSRIGTHS
                };
            }
        }
        catch ( Exception ex )
        {
            this._logger.LogError(ex, ex.Message);
        }

        return result;
    }


    private string GetAccessRigths( SecurityRightTypesEnum rights )
    {
        switch ( rights )
        {
            case SecurityRightTypesEnum.FullControl:
                return "255";
            case SecurityRightTypesEnum.Write:
                return "63";
            case SecurityRightTypesEnum.Read:
                return "45";
            default:
                return "-1";
        }
    }
}
