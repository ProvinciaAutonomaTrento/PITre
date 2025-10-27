// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente.Repertori;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetRegistriesWithAooOrRfRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetRegistriesWithAooOrRf;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRegistriesWithAooOrRf
{
    public class GetRegistriesWithAooOrRfHandler : IRequestHandler<GetRegistriesWithAooOrRfRequest, GetRegistriesWithAooOrRfResult>
    {
        #region Public Members

        public GetRegistriesWithAooOrRfHandler(ILogger<GetRegistriesWithAooOrRfHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetRegistriesWithAooOrRfResult> Handle(GetRegistriesWithAooOrRfRequest request, CancellationToken cancellationToken)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            List<RegistroRepertorio> output = new List<RegistroRepertorio>();

            long? idRoleRespAsLong = string.IsNullOrEmpty(request.idRoleResp) ? null : request.idRoleResp.AsLong();
            long? idRolePrinterAsLong = string.IsNullOrEmpty(request.idRolePrinter) ? null : request.idRolePrinter.AsLong();

            var repertoriQueryable = this._dbContext.RegistriRepertorioEntities.AsNoTracking()
                .Join(this._dbContext.OggettiCustomEntities.AsNoTracking(), reg => reg.COUNTERID, ogg => ogg.SYSTEM_ID, (reg, ogg) => new { reg, ogg })
                .Join(this._dbContext.TipoAttoEntities.AsNoTracking(), a => a.reg.TIPOLOGYID, tipologia => tipologia.SYSTEM_ID, (a, tipologia) => new { a.reg, a.ogg, tipologia });

            if (idRoleRespAsLong != null && idRolePrinterAsLong != null)
                repertoriQueryable = repertoriQueryable.Where(a => a.reg.ROLERESPID == idRoleRespAsLong || a.reg.PRINTERROLERESPID == idRolePrinterAsLong);
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
                    && (a.ROLERESPID == idRoleRespAsLong || a.PRINTERROLERESPID == idRolePrinterAsLong));
                else if (idRoleRespAsLong != null)
                    statoRepertorioQueryable = statoRepertorioQueryable.Where(a => a.COUNTERID == rep.COUNTERID && a.REGISTRYID == null && a.RFID == null && a.ROLERESPID == idRoleRespAsLong);
                else if (idRolePrinterAsLong != null)
                    statoRepertorioQueryable = statoRepertorioQueryable.Where(a => a.COUNTERID == rep.COUNTERID && a.REGISTRYID == null && a.RFID == null && a.PRINTERROLERESPID == idRolePrinterAsLong);

                var statoRepertorioEntity = await statoRepertorioQueryable.Where(r => r.COUNTERID == rep.COUNTERID && r.REGISTRYID == null && r.RFID == null)
                    .Select(r => new
                    {
                        r.COUNTERSTATE,
                        r.DTALASTPRINT,
                        r.ROLERESPID
                    })
                    .ToListAsync();

                foreach (var r in statoRepertorioEntity)
                {
                    repertorio.SingleSettings.Add(new RegistroRepertorioSingleSettings
                    {
                        CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), r.COUNTERSTATE)),
                        DateLastPrint = r.DTALASTPRINT != null ? (DateTime)r.DTALASTPRINT : DateTime.MinValue,
                        RoleAndUserDescription = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == r.ROLERESPID).Select(c => c.VAR_DESC_CORR).FirstOrDefaultAsync()
                    });
                }
                
                var registroQueryable = this._dbContext.RegistriRepertorioEntities.AsNoTracking()
                    .Join(this._dbContext.RegistroEntities.AsNoTracking(), rep => rep.REGISTRYID, reg => reg.SYSTEM_ID, (rep, reg) => new { reg, rep });
                if (idRoleRespAsLong != null && idRolePrinterAsLong != null)
                    registroQueryable = registroQueryable.Where(a => a.rep.COUNTERID == rep.COUNTERID && (a.rep.ROLERESPID == idRoleRespAsLong || a.rep.PRINTERROLERESPID == idRolePrinterAsLong));
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
                        DESCRIPTION = "[" +r.reg.VAR_CODICE  + "]" + r.reg.VAR_DESC_REGISTRO,
                        r.rep.COUNTERSTATE,
                        r.rep.DTALASTPRINT,
                        r.rep.ROLERESPID
                    })
                    .ToListAsync();

                foreach (var reg in registroEntity)
                {
                    repertorio.SingleSettings.Add(new RegistroRepertorioSingleSettings
                    {
                        RegistryId = reg.REGISTRYID.ToString(),
                        RegistryOrRfDescription = reg.DESCRIPTION,
                        CounterState = (RegistroRepertorioSingleSettings.RepertorioState)
                                (Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), reg.COUNTERSTATE)),
                        DateLastPrint = reg.DTALASTPRINT != null ? (DateTime)reg.DTALASTPRINT : DateTime.MinValue,
                        RoleAndUserDescription = await this._dbContext.CorrGlobaliEntities
                                    .AsNoTracking()
                                    .Where(c => c.ID_GRUPPO == reg.ROLERESPID)
                                    .Select(c => c.VAR_DESC_CORR)
                                    .FirstOrDefaultAsync()
                    });
                }

                var rfQueryable = this._dbContext.RegistriRepertorioEntities.AsNoTracking()
                .Join(this._dbContext.RegistroEntities.AsNoTracking(), rep => rep.RFID, reg => reg.SYSTEM_ID, (rep, reg) => new { reg, rep });
                if (idRoleRespAsLong != null && idRolePrinterAsLong != null)
                    rfQueryable = rfQueryable.Where(a => a.rep.COUNTERID == rep.COUNTERID && (a.rep.ROLERESPID == idRoleRespAsLong || a.rep.PRINTERROLERESPID == idRolePrinterAsLong));
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
                        DESCRIPTION = "[" + r.reg.VAR_CODICE + "]" + r.reg.VAR_DESC_REGISTRO,
                        r.rep.COUNTERSTATE,
                        r.rep.DTALASTPRINT,
                        r.rep.ROLERESPID
                    })
                    .ToListAsync();

                foreach (var reg in rfEntity)
                {
                    repertorio.SingleSettings.Add(new RegistroRepertorioSingleSettings
                    {
                        RFId = reg.RFID.ToString(),
                        RegistryOrRfDescription = reg.DESCRIPTION,
                        CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), reg.COUNTERSTATE)),
                        DateLastPrint = reg.DTALASTPRINT != null ? (DateTime)reg.DTALASTPRINT : DateTime.MinValue,
                        RoleAndUserDescription = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == reg.ROLERESPID).Select(c => c.VAR_DESC_CORR).FirstOrDefaultAsync()
                    });
                }

                output.Add(repertorio);
            }

            return new GetRegistriesWithAooOrRfResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetRegistriesWithAooOrRfHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
