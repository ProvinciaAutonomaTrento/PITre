// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper.Execution;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.RicercaDocumenti
{
	public class RicercaDocumentiQueryHandler : 
        IRequestHandler<RicercaDocumentiQuery, RicercaDocumentiQueryResponse>
    {
        #region Public Members

        public RicercaDocumentiQueryHandler(
			ILogger<RicercaDocumentiQueryHandler> logger,
			IClaimsPrincipalService claimsPrincipalService,
			IPi3DbContext context)
		{ 
			this._logger = logger;
			this._claimsPrincipalService = claimsPrincipalService;
			this._context = context;
		}

        public async Task<RicercaDocumentiQueryResponse> Handle(RicercaDocumentiQuery request, CancellationToken cancellationToken)
        {
            Validator.ValidateObject(request, new ValidationContext(request));

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var idUtente = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);

            var query = (from prf in this._context.ProfileEntities.AsNoTracking()
                         join peop in this._context.PeopleEntities.AsNoTracking() on prf.AUTHOR equals peop.SYSTEM_ID
                         where prf.ID_DOCUMENTO_PRINCIPALE == null
                             && (prf.CHA_IN_CESTINO == null || prf.CHA_IN_CESTINO == "0")
                             && this._context.SecurityEntities.AsNoTracking()
                                 .Any(sec => sec.THING == prf.SYSTEM_ID &&
                                     (sec.PERSONORGROUP == idUtente || sec.PERSONORGROUP == idGruppo))
                                     && peop.ID_AMM == idTenant
                         select prf);

            query = await this.ApplyFiltersAnno(request, query);

            query = await this.ApplyFiltersTipoProto(request, query);

            query = await this.ApplyFiltersNumProto(request, query);
            
            query = await this.ApplyFiltersPredisposto(request, query);

            query = await this.ApplyFiltersCodiceRegistro(request, query);

            query = await this.ApplyOrderBy(request, query);

            return await this.GetResponse(request, query);
        }

        #endregion

        #region Private Members

        private readonly ILogger<RicercaDocumentiQueryHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IPi3DbContext _context;

        private async Task<IQueryable<ProfileEntity>> ApplyFiltersAnno(RicercaDocumentiQuery request, IQueryable<ProfileEntity> query)
        {
            if (request.Anno != null)
            {
                if (request.TipoRicerca == TipiRicercheEnum.NonProtocollati || request.TipoRicerca == TipiRicercheEnum.Predisposti)
                {
                    var beginDate = new DateTime(request.Anno.Value, 1, 1, 0, 0, 0);
                    var endDate = new DateTime(request.Anno.Value, 12, 31, 23, 59, 59);

                    query = query.Where(prj => prj.CREATION_TIME >= beginDate && prj.CREATION_DATE <= endDate);
                }
                else
                    query = query.Where(prj => prj.NUM_ANNO_PROTO == request.Anno);
            }

            return query;
        }

        private async Task<IQueryable<ProfileEntity>> ApplyFiltersTipoProto(RicercaDocumentiQuery request, IQueryable<ProfileEntity> query)
        {
            string? chaTipoProto = null!;

            if (request.TipoRicerca == TipiRicercheEnum.NonProtocollati)
            {
                chaTipoProto = "G";
            }
            else if (request.TipologiaFlusso.HasValue)
            {
                chaTipoProto = request.TipologiaFlusso switch
                {
                    TipologieFlussiRicercheEnum.E => "A",
                    TipologieFlussiRicercheEnum.I => "I",
                    TipologieFlussiRicercheEnum.U => "P",
                    _ => null!
                };
            }
            
            if (!string.IsNullOrWhiteSpace(chaTipoProto))
                query = query.Where(prj => prj.CHA_TIPO_PROTO!.ToUpper() == chaTipoProto.ToUpper());

            return query;
        }

        private async Task<IQueryable<ProfileEntity>> ApplyFiltersNumProto(RicercaDocumentiQuery request, IQueryable<ProfileEntity> query)
        {
            if (request.TipoRicerca == TipiRicercheEnum.Protocollati)
            {
                if (request.NumeroProtocollo.HasValue && request.NumeroProtocolloFinale.HasValue)
                    query = query.Where(prj => prj.NUM_PROTO >= request.NumeroProtocollo.Value && prj.NUM_PROTO <= request.NumeroProtocolloFinale.Value);
                else if (request.NumeroProtocollo.HasValue)
                    query = query.Where(prj => prj.NUM_PROTO == request.NumeroProtocollo.Value);
            }
            
            return query;
        }

        private async Task<IQueryable<ProfileEntity>> ApplyFiltersCodiceRegistro(RicercaDocumentiQuery request, IQueryable<ProfileEntity> query)
        {
            if (!string.IsNullOrWhiteSpace(request.CodiceRegistro))
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

                var registroEntity = await this._context.RegistroEntities.AsNoTracking()
                    .Where(r => r.VAR_CODICE!.ToUpper() == request.CodiceRegistro.ToUpper()
                        && r.ID_AMM == idTenant)
                    .Select(r => new
                    {
                        r.SYSTEM_ID,
                        r.ID_AOO_COLLEGATA
                    })
                    .FirstOrDefaultAsync();

                if (registroEntity == null)
                    throw new RegistroNotFoundPi3Exception(request.CodiceRegistro);

                var idRegistro = registroEntity.SYSTEM_ID;
                if (registroEntity.ID_AOO_COLLEGATA.HasValue)
                    idRegistro = registroEntity.ID_AOO_COLLEGATA.Value;

                query = query.Where(prj => prj.ID_REGISTRO == idRegistro);
            }

            return query;
        }

        private async Task<IQueryable<ProfileEntity>> ApplyFiltersPredisposto(RicercaDocumentiQuery request, IQueryable<ProfileEntity> query)
        {
            if (request.TipoRicerca == TipiRicercheEnum.Predisposti)
            {
                query = query.Where(prj => prj.CHA_DA_PROTO == "1");
            }

            return query;
        }

        private async Task<IQueryable<ProfileEntity>> ApplyOrderBy(RicercaDocumentiQuery request, IQueryable<ProfileEntity> query)
        {
            if (request.TipoRicerca == TipiRicercheEnum.NonProtocollati || request.TipoRicerca == TipiRicercheEnum.Predisposti)
                query = query.OrderByDescending(prg => prg.CREATION_DATE);
            else
                query = query.OrderByDescending(prg => prg.DTA_PROTO);

            return query;
        }

        private async Task<RicercaDocumentiQueryResponse> GetResponse(RicercaDocumentiQuery request, IQueryable<ProfileEntity> query)
        {
            var documenti = await query
                .Skip(request.Paginazione!.Ignora)
                .Take(request.Paginazione!.Prendi + 1)
                .Select(d => new Documento()
                {
                    Id = d.SYSTEM_ID.ToString(),
                    CreationDate = d.CREATION_DATE!.Value,
                    OggettoDelDocumento = d.VAR_PROF_OGGETTO!,
                    DataProtocollo = d.DTA_PROTO,
                    NumeroProtocollo = d.NUM_PROTO,
                    TipologiaFlusso = AsTipologiaFlusso(d.CHA_TIPO_PROTO),
                    Segnatura = d.VAR_SEGNATURA,
                    Annullato = request.TipoRicerca == TipiRicercheEnum.Protocollati ? d.DTA_ANNULLA.HasValue : null
                })
                .ToListAsync();

            var numRecord = documenti.Count();

            if (numRecord > request.Paginazione.Prendi)
                numRecord = request.Paginazione.Prendi;

            return new RicercaDocumentiQueryResponse()
            {
                Documenti = documenti.GetRange(0, numRecord),
                PiuDati = documenti.Count() == request.Paginazione.Prendi + 1
            };
        }

        private static string? AsTipologiaFlusso(string? chaTipoProto)
        {
            return chaTipoProto switch
                    {  
                        "A" => TipologieFlussiRicercheEnum.E.ToString(), 
                        "P" => TipologieFlussiRicercheEnum.U.ToString(), 
                        "I" => TipologieFlussiRicercheEnum.I.ToString(), 
                        _ => null!
                    };
        }

        #endregion
    }
}
