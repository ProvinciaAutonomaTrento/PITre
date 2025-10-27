// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.AggregateModels.DelegaAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Core.Extensions;

using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;
using SERVICE_REQUEST = Pi3.App.Legacy.Mobile.Models.ServiceRequests;

namespace Pi3.App.Legacy.Mobile.Data.Services;
public class DelegheService(
    ILogger<DelegheService> logger,
    IPi3DbContext pi3DbContext,
    IClaimsPrincipalService claimsPrincipalService,
    IDelegaRepository delegaRepository ) : IDelegheService
{
    private readonly ILogger<DelegheService> _logger = logger;
    private readonly IPi3DbContext _dbContext = pi3DbContext;
    private readonly IClaimsPrincipalService _claimsPrincipalService = claimsPrincipalService;
    private readonly IDelegaRepository _delegaRepository = delegaRepository;

    public async Task<IEnumerable<SERVICE_DTO.Delega>> GetDeleghe(string stato, string tipo, CancellationToken cancellationToken )
    {
        this._logger.LogInformation("DelegheService > GetDeleghe");
        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
        var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
        var delegatedIdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);

        var delegatedIdRuolo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdGroup);
        var delegatedIdName = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedUserName);

        var statoDelega = stato;
        List<SERVICE_DTO.Delega> output = [];

        var queryable = this._dbContext.DelegheEntities.AsNoTracking();
        if ( statoDelega == "A" )
            queryable = this._dbContext.DelegheEntities.AsNoTracking().Where(d => d.DATA_DECORRENZA <= DateTime.Now && (d.DATA_SCADENZA == null || d.DATA_SCADENZA > DateTime.Now));
        if ( statoDelega == "I" )
            queryable = this._dbContext.DelegheEntities.AsNoTracking().Where(d => d.DATA_DECORRENZA > DateTime.Now);
        if ( statoDelega == "S" )
            queryable = this._dbContext.DelegheEntities.AsNoTracking().Where(d => d.DATA_SCADENZA < DateTime.Now);

        switch ( tipo )
        {
            case "assegnate":
                queryable = queryable.Where(d => d.ID_PEOPLE_DELEGANTE == idPeople);
                if ( delegatedIdRuolo > 0 )
                {
                    var idRuoloDelegante = delegatedIdRuolo;
                    queryable = queryable.Where(d => d.ID_RUOLO_DELEGANTE == idRuoloDelegante);
                }
                break;
            case "ricevute":
                queryable = queryable.Where(d => d.ID_PEOPLE_DELEGATO == idPeople);
                break;
            case "esercizio":
                queryable = this._dbContext.DelegheEntities
                    .Where(d => d.ID_PEOPLE_DELEGATO == delegatedIdUser && d.CHA_IN_ESERCIZIO == "1" && d.DATA_DECORRENZA <= DateTime.Now && (d.DATA_SCADENZA == null || d.DATA_SCADENZA >= DateTime.Now));
                break;
        }

        queryable = queryable.OrderByDescending(d => d.DATA_DECORRENZA);
        var delegaEntity = await queryable.ToListAsync(cancellationToken);


        foreach ( var delega in delegaEntity )
        {
            var infoDelega = new SERVICE_DTO.Delega
            {
                Id = delega.SYSTEM_ID.ToString(),
                id_utente_delegante = delega.ID_PEOPLE_DELEGANTE.ToString(),
                CodiceDelegante = delega.COD_PEOPLE_DELEGANTE,
                IdRuoloDelegante = delega.ID_RUOLO_DELEGANTE.ToString(),
                RuoloDelegante = delega.COD_RUOLO_DELEGANTE,
                IdDelegato = delega.ID_PEOPLE_DELEGATO.ToString(),
                //id_uo_delegato = delega.ID_UO_DELEGATO.ToString(),
                IdRuoloDelegato = delega.ID_RUOLO_DELEGATO.ToString(),
                RuoloDelegato = delega.COD_RUOLO_DELEGATO,
                DataDecorrenza = delega.DATA_DECORRENZA,
                DataScadenza = delega.DATA_SCADENZA,
                InEsercizio = delega.CHA_IN_ESERCIZIO
            };


            if ( delega.DATA_SCADENZA != null && delega.CHA_IN_ESERCIZIO == "1" && delega.DATA_SCADENZA < DateTime.Now )
            {
                var aggregate = await _delegaRepository.Get(idTenant.ToString(), delega.SYSTEM_ID.ToString());
                aggregate.Dismetti();

                await this._delegaRepository.Update(aggregate);

                infoDelega.InEsercizio = "0";
            }

            if ( delega.ID_PEOPLE_DELEGANTE != 0 )
            {
                var utenteEntity = await this._dbContext.PeopleEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == delega.ID_PEOPLE_DELEGANTE)
                    .Select(p => new
                    {
                        p.VAR_COGNOME,
                        p.VAR_NOME,
                        p.DISABLED
                    })
                    .FirstAsync(cancellationToken);

                infoDelega.CodiceDelegante = String.Format("{0} {1}", utenteEntity.VAR_COGNOME, utenteEntity.VAR_NOME);
            }

            output.Add(infoDelega);

        }


        return output;
    }


    public async Task<bool> CreaDelega( SERVICE_REQUEST.CreaDelegaRequest request, CancellationToken cancellationToken)
    {
        bool output = true;
        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

        try
        {
            var idCorrGlobaliDelegato = request.IdRuoloDelegato!.AsLong();
            var idCorrGlobaliDelegante = request.IdRuoloDelegante!.AsLong();

            var delegatoEnity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(c => c.SYSTEM_ID == idCorrGlobaliDelegato)
                .Select(c => new
                {
                    c.ID_GRUPPO,
                    c.VAR_COD_RUBRICA,
                    c.VAR_DESC_CORR
                })
                .FirstAsync(cancellationToken);

            RuoloDeleganteEntity? deleganteEnity = null;
            if ( idCorrGlobaliDelegante != 0 )
            {
                deleganteEnity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == idCorrGlobaliDelegante)
                    .Select(c => new RuoloDeleganteEntity()
                    {
                        ID_GRUPPO = c.ID_GRUPPO,
                        VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                        VAR_DESC_CORR = c.VAR_DESC_CORR
                    })
                    .FirstAsync(cancellationToken);
            }
            else
            {
                deleganteEnity = new RuoloDeleganteEntity()
                {
                    ID_GRUPPO = 0,
                    VAR_COD_RUBRICA = "TUTTI"
                };
            }

            var aggregate = new Core.AggregateModels.DelegaAggregate.Delega(idTenant!, DateTime.Now, null, null);
            aggregate.AssignGruppoDelegante(deleganteEnity.ID_GRUPPO!.Value.ToString(), deleganteEnity.VAR_COD_RUBRICA, deleganteEnity.VAR_DESC_CORR);
            aggregate.AssignGruppoDelegato(delegatoEnity.ID_GRUPPO!.Value.ToString(), delegatoEnity.VAR_COD_RUBRICA, delegatoEnity.VAR_DESC_CORR);
            aggregate.AssignUtenteDelegante(request.id_utente_delegante!, request.CodiceDelegante, null, null);
            aggregate.ChangeDataDecorrenza(request.DataDecorrenza!.Value);
            aggregate.ChangeDataScadenza(request.DataScadenza);

            await this._delegaRepository.Add(aggregate);
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex.Message, ex);
            output = false;
        }

        return output;
    }

    public async Task<bool> RimuoviDeleghe(IEnumerable<SERVICE_DTO.Delega> deleghe)
    {
        bool serviceResult = true;

        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

        try
        {
            foreach(var delega in deleghe)
            {
                var aggregate = await _delegaRepository.Get(idTenant, delega.Id);
                aggregate.Revoca();

                await this._delegaRepository.Update(aggregate);
            }
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex.Message, ex);
            serviceResult = false;
        }

        return serviceResult;
    }

    protected class RuoloDeleganteEntity
    {
        public long? ID_GRUPPO { get; set; }
        public string? VAR_COD_RUBRICA { get; set; }
        public string? VAR_DESC_CORR { get; set; }
    }
}
