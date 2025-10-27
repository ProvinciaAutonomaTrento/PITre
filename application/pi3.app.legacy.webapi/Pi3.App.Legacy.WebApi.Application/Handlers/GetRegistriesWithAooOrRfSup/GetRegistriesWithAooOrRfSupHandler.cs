// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.utente.Repertori;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetRegistriesWithAooOrRfSupRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetRegistriesWithAooOrRfSup;
namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRegistriesWithAooOrRfSup
{

    public class GetRegistriesWithAooOrRfSupHandler : IRequestHandler<GetRegistriesWithAooOrRfSupRequest, GetRegistriesWithAooOrRfSupResult>
    {
        #region Public Members

        public GetRegistriesWithAooOrRfSupHandler(ILogger<GetRegistriesWithAooOrRfSupHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetRegistriesWithAooOrRfSupResult> Handle(GetRegistriesWithAooOrRfSupRequest request, CancellationToken cancellationToken)
        {
            List<RegistroRepertorio> output = new List<RegistroRepertorio>();

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            var idRoleRespCons = await this._dbContext.AmministraEntities.AsNoTracking()
                .Where(a => a.SYSTEM_ID == idTenant)
                .Select(a => a.ID_RUOLO_RESP_CONS).FirstOrDefaultAsync();

            var idRoleRespConsAsString = idRoleRespCons == null ? null : idRoleRespCons.ToString();

            if (request.idRoleResp.Equals(idRoleRespConsAsString) || request.idRolePrinter.Equals(idRoleRespConsAsString))
                output = await this.GetRegistriesRespCons();
            else
                output = await GetRegistriesWithAooOrRfSup(request.idRoleResp, request.idRolePrinter);

            return new GetRegistriesWithAooOrRfSupResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetRegistriesWithAooOrRfSupHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected virtual async Task<List<long>> GetUoInferiori(long idUO)
        {
            var listUOSottoposte = new List<long>();

            var idUoSottoposte = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_PARENT == idUO).Select(c => c.SYSTEM_ID).ToListAsync();

            if (idUoSottoposte != null && idUoSottoposte.Count > 0)
            {
                listUOSottoposte.AddRange(idUoSottoposte);
                foreach (var id in idUoSottoposte)
                {
                    var sottoposte = await this.GetUoInferiori(id);
                    listUOSottoposte.AddRange(sottoposte);
                }
            }

            return listUOSottoposte;
        }

        protected virtual async Task<List<RegistroRepertorio>> GetRegistriesRespCons()
        {
            List<RegistroRepertorio> output = new List<RegistroRepertorio>();

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            var repertoriEntity = await this._dbContext.RegistriRepertorioEntities.AsNoTracking()
                .Join(this._dbContext.OggettiCustomEntities.AsNoTracking(), reg => reg.COUNTERID, ogg => ogg.SYSTEM_ID, (reg, ogg) => new { reg, ogg })
                .Join(this._dbContext.TipoAttoEntities.AsNoTracking(), a => a.reg.TIPOLOGYID, tipologia => tipologia.SYSTEM_ID, (a, tipologia) => new { a.reg, a.ogg, tipologia })
                .Where(a => a.tipologia.ID_AMM == idTenant)
                .OrderBy(a => a.tipologia.VAR_DESC_ATTO)
                .Select(a => new
                {
                    a.reg.COUNTERID,
                    DESCRIZIONE_OGGETTO = a.ogg.DESCRIZIONE,
                    DESCRIZIONE_TIPOLOGIA = a.tipologia.VAR_DESC_ATTO
                })
                .Distinct()
                .ToListAsync();

            foreach (var rep in repertoriEntity)
            {
                RegistroRepertorio repertorio = new RegistroRepertorio
                {
                    CounterId = rep.COUNTERID.ToString(),
                    CounterDescription = rep.DESCRIZIONE_OGGETTO,
                    TipologyDescription = rep.DESCRIZIONE_TIPOLOGIA,
                    SingleSettings = new List<RegistroRepertorioSingleSettings>()
                };

                var statoRepertorioEntity = await this._dbContext.RegistriRepertorioEntities.AsNoTracking()
                    .Where(r => r.COUNTERID == rep.COUNTERID && r.REGISTRYID == null && r.RFID == null)
                    .Select(r => r.COUNTERSTATE)
                    .ToListAsync();

                statoRepertorioEntity.ForEach(counterstate =>
                    repertorio.SingleSettings.Add(new RegistroRepertorioSingleSettings
                    {
                        CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), counterstate))
                    })
                );

                var registroEntity = await this._dbContext.RegistriRepertorioEntities.AsNoTracking()
                    .Join(this._dbContext.RegistroEntities.AsNoTracking(), rep => rep.REGISTRYID, reg => reg.SYSTEM_ID, (rep, reg) => new { reg, rep })
                    .Where(r => r.rep.COUNTERID == rep.COUNTERID && r.reg.ID_AMM == idTenant)
                    .OrderBy(r => r.reg.VAR_DESC_REGISTRO)
                    .Select(r => new
                    {
                        r.rep.REGISTRYID,
                        r.reg.VAR_DESC_REGISTRO,
                        r.rep.COUNTERSTATE
                    })
                    .ToListAsync();

                registroEntity.ForEach(reg =>
                    repertorio.SingleSettings.Add(new RegistroRepertorioSingleSettings
                    {
                        RegistryId = reg.REGISTRYID.ToString(),
                        RegistryOrRfDescription = reg.VAR_DESC_REGISTRO,
                        CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), reg.COUNTERSTATE))
                    })
                );

                var rfEntity = await this._dbContext.RegistriRepertorioEntities.AsNoTracking()
                    .Join(this._dbContext.RegistroEntities.AsNoTracking(), rep => rep.RFID, reg => reg.SYSTEM_ID, (rep, reg) => new { reg, rep })
                    .Where(r => r.rep.COUNTERID == rep.COUNTERID && r.reg.ID_AMM == idTenant)
                    .OrderBy(r => r.reg.VAR_DESC_REGISTRO)
                    .Select(r => new
                    {
                        r.rep.RFID,
                        r.reg.VAR_DESC_REGISTRO,
                        r.rep.COUNTERSTATE
                    })
                    .ToListAsync();

                rfEntity.ForEach(rf =>
                    repertorio.SingleSettings.Add(new RegistroRepertorioSingleSettings
                    {
                        RFId = rf.RFID.ToString(),
                        RegistryOrRfDescription = rf.VAR_DESC_REGISTRO,
                        CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), rf.COUNTERSTATE))
                    })
                );

                output.Add(repertorio);
            }

            return output;
        }

        protected virtual async Task<List<RegistroRepertorio>> GetRegistriesWithAooOrRfSup(string idRoleResp, string idRolePrinter)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            List<RegistroRepertorio> output = new List<RegistroRepertorio>();

            long? idRoleRespAsLong = string.IsNullOrEmpty(idRoleResp) ? null : idRoleResp.AsLong();
            long? idRolePrinterAsLong = string.IsNullOrEmpty(idRolePrinter) ? null : idRolePrinter.AsLong();

            List<long?> ruoliSottoposti = new List<long?>();
            if (idRolePrinterAsLong != null)
            {
                var printerRoleRespId = this._dbContext.RegistriRepertorioEntities.AsNoTracking().Select(r => r.PRINTERROLERESPID);

                var numLivello = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Join(this._dbContext.TipoRuoloEntities.AsNoTracking(), corr => corr.ID_TIPO_RUOLO, tipo => tipo.SYSTEM_ID, (corr, tipo) => new { corr.ID_GRUPPO, tipo.NUM_LIVELLO })
                    .Where(c => c.ID_GRUPPO == idRolePrinterAsLong)
                    .Select(c => c.NUM_LIVELLO)
                    .FirstAsync();

                var idUo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idRolePrinterAsLong).Select(c => c.ID_UO).FirstAsync();

                var uoInferiori = await this.GetUoInferiori((long)idUo);
                uoInferiori.Add((long)idUo);

                ruoliSottoposti = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Join(this._dbContext.TipoRuoloEntities.AsNoTracking(), corr => corr.ID_TIPO_RUOLO, tipo => tipo.SYSTEM_ID, (corr, tipo) => new { corr.ID_GRUPPO, tipo.NUM_LIVELLO, corr.ID_UO })
                    .Where(c => c.NUM_LIVELLO >= numLivello && printerRoleRespId.Contains(c.ID_GRUPPO) && uoInferiori.Contains(c.ID_UO ?? 0))
                    .Select(c => c.ID_GRUPPO)
                    .ToListAsync();

                ruoliSottoposti.Add(idRolePrinterAsLong);
                ruoliSottoposti = ruoliSottoposti.Where(r => r != null).ToList();
            }

            var repertoriQueryable = this._dbContext.RegistriRepertorioEntities.AsNoTracking()
                .Join(this._dbContext.OggettiCustomEntities.AsNoTracking(), reg => reg.COUNTERID, ogg => ogg.SYSTEM_ID, (reg, ogg) => new { reg, ogg })
                .Join(this._dbContext.TipoAttoEntities.AsNoTracking(), a => a.reg.TIPOLOGYID, tipologia => tipologia.SYSTEM_ID, (a, tipologia) => new { a.reg, a.ogg, tipologia });

            if (idRoleRespAsLong != null && idRolePrinterAsLong != null)
                repertoriQueryable = repertoriQueryable.Where(a => a.reg.ROLERESPID == idRoleRespAsLong || (ruoliSottoposti != null && ruoliSottoposti.Contains(a.reg.PRINTERROLERESPID)));
            else if (idRoleRespAsLong != null)
                repertoriQueryable = repertoriQueryable.Where(a => a.reg.ROLERESPID == idRoleRespAsLong);
            else if (idRolePrinterAsLong != null)
                repertoriQueryable = repertoriQueryable.Where(a => a.reg.PRINTERROLERESPID == idRolePrinterAsLong);

            var repertoriEntity = await repertoriQueryable
                .OrderBy(a => a.tipologia.VAR_DESC_ATTO)
                .Select(a => new
                {
                    a.reg.COUNTERID,
                    DESCRIZIONE_OGGETTO = a.ogg.DESCRIZIONE,
                    DESCRIZIONE_TIPOLOGIA = a.tipologia.VAR_DESC_ATTO
                })
                .Distinct()
                .ToListAsync();

            foreach (var rep in repertoriEntity)
            {
                RegistroRepertorio repertorio = new RegistroRepertorio
                {
                    CounterId = rep.COUNTERID.ToString(),
                    CounterDescription = rep.DESCRIZIONE_OGGETTO,
                    TipologyDescription = rep.DESCRIZIONE_TIPOLOGIA,
                    SingleSettings = new List<RegistroRepertorioSingleSettings>()
                };

                var statoRepertorioQueryable = this._dbContext.RegistriRepertorioEntities.AsNoTracking();
                if (idRoleRespAsLong != null && idRolePrinterAsLong != null)
                    statoRepertorioQueryable = statoRepertorioQueryable.Where(a => a.COUNTERID == rep.COUNTERID && a.REGISTRYID == null && a.RFID == null
                    && (a.ROLERESPID == idRoleRespAsLong || (ruoliSottoposti != null && ruoliSottoposti.Contains(a.PRINTERROLERESPID))));
                else if (idRoleRespAsLong != null)
                    statoRepertorioQueryable = statoRepertorioQueryable.Where(a => a.COUNTERID == rep.COUNTERID && a.REGISTRYID == null && a.RFID == null && a.ROLERESPID == idRoleRespAsLong);
                else if (idRolePrinterAsLong != null)
                    statoRepertorioQueryable = statoRepertorioQueryable.Where(a => a.COUNTERID == rep.COUNTERID && a.REGISTRYID == null && a.RFID == null && a.PRINTERROLERESPID == idRolePrinterAsLong);

                var statoRepertorioEntity = await statoRepertorioQueryable.Where(r => r.COUNTERID == rep.COUNTERID && r.REGISTRYID == null && r.RFID == null)
                    .Select(r => r.COUNTERSTATE)
                    .ToListAsync();

                statoRepertorioEntity.ForEach(counterstate =>
                    repertorio.SingleSettings.Add(new RegistroRepertorioSingleSettings
                    {
                        CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), counterstate))
                    })
                );

                var registroQueryable = this._dbContext.RegistriRepertorioEntities.AsNoTracking()
                    .Join(this._dbContext.RegistroEntities.AsNoTracking(), rep => rep.REGISTRYID, reg => reg.SYSTEM_ID, (rep, reg) => new { reg, rep });
                if (idRoleRespAsLong != null && idRolePrinterAsLong != null)
                    registroQueryable = registroQueryable.Where(a => a.rep.COUNTERID == rep.COUNTERID && (a.rep.ROLERESPID == idRoleRespAsLong || (ruoliSottoposti != null && ruoliSottoposti.Contains(a.rep.PRINTERROLERESPID))));
                else if (idRoleRespAsLong != null)
                    registroQueryable = registroQueryable.Where(a => a.rep.COUNTERID == rep.COUNTERID && a.rep.ROLERESPID == idRoleRespAsLong);
                else if (idRolePrinterAsLong != null)
                    registroQueryable = registroQueryable.Where(a => a.rep.COUNTERID == rep.COUNTERID && a.rep.PRINTERROLERESPID == idRolePrinterAsLong);

                var registroEntity = await registroQueryable
                    .Where(r => r.reg.ID_AMM == idTenant)
                    .OrderBy(r => r.reg.VAR_DESC_REGISTRO)
                    .Select(r => new
                    {
                        r.rep.REGISTRYID,
                        r.reg.VAR_DESC_REGISTRO,
                        r.rep.COUNTERSTATE
                    })
                    .ToListAsync();

                registroEntity.ForEach(reg =>
                    repertorio.SingleSettings.Add(new RegistroRepertorioSingleSettings
                    {
                        RegistryId = reg.REGISTRYID.ToString(),
                        RegistryOrRfDescription = reg.VAR_DESC_REGISTRO,
                        CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), reg.COUNTERSTATE))
                    })
                );

                var rfQueryable = this._dbContext.RegistriRepertorioEntities.AsNoTracking()
                .Join(this._dbContext.RegistroEntities.AsNoTracking(), rep => rep.RFID, reg => reg.SYSTEM_ID, (rep, reg) => new { reg, rep });
                if (idRoleRespAsLong != null && idRolePrinterAsLong != null)
                    rfQueryable = rfQueryable.Where(a => a.rep.COUNTERID == rep.COUNTERID && (a.rep.ROLERESPID == idRoleRespAsLong || (ruoliSottoposti != null && ruoliSottoposti.Contains(a.rep.PRINTERROLERESPID))));
                else if (idRoleRespAsLong != null)
                    rfQueryable = rfQueryable.Where(a => a.rep.COUNTERID == rep.COUNTERID && a.rep.ROLERESPID == idRoleRespAsLong);
                else if (idRolePrinterAsLong != null)
                    rfQueryable = rfQueryable.Where(a => a.rep.COUNTERID == rep.COUNTERID && a.rep.PRINTERROLERESPID == idRolePrinterAsLong);

                var rfEntity = await rfQueryable
                    .Where(r => r.reg.ID_AMM == idTenant)
                    .OrderBy(r => r.reg.VAR_DESC_REGISTRO)
                    .Select(r => new
                    {
                        r.rep.RFID,
                        r.reg.VAR_DESC_REGISTRO,
                        r.rep.COUNTERSTATE
                    })
                    .ToListAsync();

                rfEntity.ForEach(reg =>
                    repertorio.SingleSettings.Add(new RegistroRepertorioSingleSettings
                    {
                        RFId = reg.RFID.ToString(),
                        RegistryOrRfDescription = reg.VAR_DESC_REGISTRO,
                        CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), reg.COUNTERSTATE))
                    })
                );

                output.Add(repertorio);
            }

            return output;
        }


        #endregion
    }
}
