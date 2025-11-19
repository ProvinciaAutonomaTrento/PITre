// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using Elastic.Apm.Api;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.RicercaTrasmissioni
{
    public class RicercaTrasmissioniQueryHandler : IRequestHandler<RicercaTrasmissioniQuery, RicercaTrasmissioniQueryResponse>
    {
        public RicercaTrasmissioniQueryHandler(
            ILogger<RicercaTrasmissioniQueryHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,           
            IPi3DbContext context,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._context = context;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<RicercaTrasmissioniQueryResponse> Handle(RicercaTrasmissioniQuery request, CancellationToken cancellationToken)
        {
            if (!request.IdDocumento.IsValidAggregateId())
                throw new InvalidIdDocumentPi3Exception(request.IdDocumento);

            Validator.ValidateObject(request, new ValidationContext(request), true);

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            if (!await _documentoAmministrativoRepository.Exists(idTenant!, request.IdDocumento))
                throw new DocumentoAmministrativoNotFoundPi3Exception(request.IdDocumento);

            var trasmissioni = await (from t in this._context.TrasmissioneEntities.AsNoTracking()
                              join p in this._context.PeopleEntities.AsNoTracking() on t.ID_PEOPLE equals p.SYSTEM_ID
                              join pDel in this._context.PeopleEntities on t.ID_PEOPLE_DELEGATO equals pDel.SYSTEM_ID into pDel
                                from p2 in pDel.DefaultIfEmpty()
                              join cg in this._context.CorrGlobaliEntities.AsNoTracking() on t.ID_RUOLO_IN_UO equals cg.SYSTEM_ID
                              where t.ID_PROFILE == request.IdDocumento.AsLong()
                              orderby t.DTA_INVIO                              
                              select new Trasmissione()
                              {
                                  Id = t.SYSTEM_ID.ToString(),
                                  DataInvio = t.DTA_INVIO,
                                  Autore = new Autore()
                                  {
                                      UserId = p.USER_ID!,
                                      Nome = p.VAR_NOME!,
                                      Cognome = p.VAR_COGNOME!,
                                      CodiceGruppo = cg.VAR_CODICE!,
                                      DescrizioneGruppo = cg.VAR_DESC_CORR!,
                                      UserIdDelegato = (p2! != null! ? p2.USER_ID!  : null!),
                                      NomeDelegato = (p2! != null! ? p2.VAR_NOME! : null!),
                                      CognomeDelegato = (p2! != null! ? p2.VAR_COGNOME! : null!)
                                  },
                                  NoteGenerali = t.VAR_NOTE_GENERALI!
                              })
                              .Skip(request.Ignora)
                              .Take(request.Prendi + 1)
                        .ToListAsync();
            
            var piuDati = trasmissioni.Count > request.Prendi;
            return new RicercaTrasmissioniQueryResponse()
            {
                Trasmissioni = piuDati ? trasmissioni.SkipLast(request.Prendi) : trasmissioni,
                PiuDati = piuDati
            };
        }

        #region Private Methods
        private readonly ILogger<RicercaTrasmissioniQueryHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IMediator _mediator;
        private readonly IPi3DbContext _context;
        private IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        
        #endregion
    }
}
