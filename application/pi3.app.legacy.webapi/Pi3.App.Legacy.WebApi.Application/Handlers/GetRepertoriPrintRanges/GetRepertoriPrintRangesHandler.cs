// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente.Repertori;
using DocsPaVO.utente.Repertori.RequestAndResponse;
using LinqKit;
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
using GetRepertoriPrintRangesRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetRepertoriPrintRanges;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRepertoriPrintRanges
{
    public class GetRepertoriPrintRangesHandler : IRequestHandler<GetRepertoriPrintRangesRequest, GetRepertoriPrintRangesResult>
    {
        #region Public members
        public GetRepertoriPrintRangesHandler(ILogger<GetRepertoriPrintRangesHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetRepertoriPrintRangesResult> Handle(GetRepertoriPrintRangesRequest request, CancellationToken cancellationToken)
        {
            var output = new GetRepertoriPrintRangesResponse();

            var tipoOggettoEntity = await this._dbContext.TipoOggettoEntities.FirstAsync(x => x.DESCRIZIONE.ToLower() == "contatore");

            var regRf = this.GetRegRfList(request.request.RegistryId, request.request.RfId);

            if (request.repairBrokenPrint)
            {
                var stampaRepertoriQueryable = this._dbContext.StampaRepertoriEntities.AsNoTracking()
                    .Join(this._dbContext.ComponentEntities.AsNoTracking(), a => a.DOCNUMBER, b => b.DOCNUMBER, (a, b) => new { a, b })
                    .Where(x => string.IsNullOrWhiteSpace(x.b.PATH)
                    && x.a.ID_REPERTORIO == request.request.CounterId.AsLong()
                    && x.a.REGISTRYID == regRf.AsLong())
                    .Select(x => new StampaRepertoriEntity
                    {
                        NUM_ANNO = x.a.NUM_ANNO,
                        NUM_REP_START = x.a.NUM_REP_START 
                    });

                var ranges = new List<RepertorioPrintRange>();

                var rangeEntities = stampaRepertoriQueryable.GroupBy(x => x.NUM_ANNO).ToDictionary(y => y.Key, y => y.Select(y => y.NUM_REP_START).ToList());

                rangeEntities.Keys.ForEach(x => ranges.Add(new RepertorioPrintRange
                {
                    Year = (int)x,
                    FirstNumber = (int)rangeEntities[x].Min(),
                    LastNumber = (int)rangeEntities[x].Max()
                }));

                output.Ranges = ranges;

            }
            else
            {
                var registriRepertorioQueryable = this._dbContext.RegistriRepertorioEntities
                .Where(x => x.COUNTERID == request.request.CounterId.AsLong());

                var docCustomQueryable = this._dbContext.MvDocumentiCustomEntities
                    .Where(x => x.SYSTEM_ID_OGG_CUSTOM == request.request.CounterId.AsLong()
                    && x.ID_AOO_RF == regRf.AsLong());


                if (string.IsNullOrWhiteSpace(request.request.RegistryId))
                {
                    registriRepertorioQueryable = registriRepertorioQueryable.Where(x => x.REGISTRYID == null);
                }
                else
                {
                    registriRepertorioQueryable = registriRepertorioQueryable.Where(x => x.REGISTRYID == request.request.RegistryId.AsLong());
                }

                if (string.IsNullOrWhiteSpace(request.request.RfId))
                {
                    registriRepertorioQueryable = registriRepertorioQueryable.Where(x => x.RFID == null);
                }
                else
                {
                    registriRepertorioQueryable = registriRepertorioQueryable.Where(x => x.RFID == request.request.RfId.AsLong());
                }

                var lastPrintedNumber = await registriRepertorioQueryable.Select(x => x.LASTPRINTEDNUMBER).FirstOrDefaultAsync();

                var firstDateFilter = await registriRepertorioQueryable.Select(x => x.DTALASTPRINT).FirstOrDefaultAsync();

                var secondDateFilter = await docCustomQueryable.Where(x => x.VALORE_OGGETTO_DB == lastPrintedNumber.ToString())
                    .OrderByDescending(x => x.DTA_INS)
                    .Select(x => x.DTA_INS)
                    .FirstOrDefaultAsync();


                var entitiesQueryable = this._dbContext.MvDocumentiCustomEntities.AsNoTracking()
                    .Join(this._dbContext.OggettiCustomEntities.AsNoTracking(), a => a.SYSTEM_ID_OGG_CUSTOM, b => b.SYSTEM_ID, (a, b) => new { a, b })
                    .Where(x => x.a.ID_AOO_RF == regRf.AsLong()
                    && x.a.SYSTEM_ID_OGG_CUSTOM == request.request.CounterId.AsLong()
                    && x.b.ID_TIPO_OGGETTO == tipoOggettoEntity.SYSTEM_ID);

                if (firstDateFilter.HasValue) entitiesQueryable = entitiesQueryable.Where(x => x.a.DTA_INS >= firstDateFilter);
                if (secondDateFilter.HasValue) entitiesQueryable = entitiesQueryable.Where(x => x.a.DTA_INS > secondDateFilter);

                var docCustomEntities = await entitiesQueryable
                    .Where(x => !string.IsNullOrWhiteSpace(x.a.VALORE_OGGETTO_DB) && x.a.ANNO.HasValue)
                    .Select(x => new
                    {
                        x.a.ANNO,
                        x.a.VALORE_OGGETTO_DB
                    }).ToListAsync();

                var ranges = new List<RepertorioPrintRange>();

                var rangeEntities = docCustomEntities.GroupBy(x => x.ANNO!.Value).ToDictionary(y => y.Key, y => y.Select(y => int.Parse(y.VALORE_OGGETTO_DB!)).ToList());

                rangeEntities.Keys.ForEach(x => ranges.Add(new RepertorioPrintRange
                {
                    Year = (int)x,
                    FirstNumber = rangeEntities[x].Min(),
                    LastNumber = rangeEntities[x].Max()
                }));

                output.Ranges = ranges;
            }

            return new GetRepertoriPrintRangesResult(output);

        }
        #endregion

        #region Private members
        protected ILogger<GetRepertoriPrintRangesHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;

        private string GetRegRfList(string? registryId, string? rfId)
        {
            if (string.IsNullOrWhiteSpace(registryId) && string.IsNullOrWhiteSpace(rfId)) return "0";
            else return string.IsNullOrWhiteSpace(registryId) ? rfId! : registryId;

        }

        #endregion
    }
}
