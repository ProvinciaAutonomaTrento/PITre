// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
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
using DocumentoGetStoriaVisibilitaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetStoriaVisibilita;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetStoriaVisibilita
{
    public class DocumentoGetStoriaVisibilitaHandler : IRequestHandler<DocumentoGetStoriaVisibilitaRequest, DocumentoGetStoriaVisibilitaResult>
    {
        #region Public Members

        public DocumentoGetStoriaVisibilitaHandler(ILogger<DocumentoGetStoriaVisibilitaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<DocumentoGetStoriaVisibilitaResult> Handle(DocumentoGetStoriaVisibilitaRequest request, CancellationToken cancellationToken)
        {
            StoriaDirittoDocumento[] output = null;

            try
            {
                var idOggetto = request.idProfile.AsLong();
                string[] varCodAzione = request.tipoObj == "D" ? new string[] { "EDITING_ACL", "CESSIONE_DOC" } : new string[] { "EDITING_FASC_ACL", "CESSIONE_FASC" };

                var storiaVisibilitaEntities = await this._dbContext.LogEntities
                    .Join(this._dbContext.CorrGlobaliEntities, log => log.ID_GRUPPO_OPERATORE, corr => corr.ID_GRUPPO, (log, corr) => new { log, corr })
                    .Where(l => l.log.ID_OGGETTO == idOggetto && l.log.CHA_ESITO == "1" && varCodAzione.Contains(l.log.VAR_COD_AZIONE))
                    .Select(l => new StoriaVisibilitaEntity()
                    {
                         USERID_OPERATORE = l.log.USERID_OPERATORE,
                         VAR_DESC_CORR = l.corr.VAR_DESC_CORR,
                         VAR_DESC_OGGETTO = l.log.VAR_DESC_OGGETTO,
                         DTA_AZIONE = l.log.DTA_AZIONE
                    })
                    .AsNoTracking()
                    .ToListAsync();

                output = this._mapper.Map<StoriaDirittoDocumento[]>(storiaVisibilitaEntities);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new DocumentoGetStoriaVisibilitaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetStoriaVisibilitaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StoriaVisibilitaEntity, StoriaDirittoDocumento>()
                     .ForMember(dest => dest.utente, opt => opt.MapFrom(src => src.USERID_OPERATORE))
                     .ForMember(dest => dest.ruolo, opt => opt.MapFrom(src => src.VAR_DESC_CORR))
                     .ForMember(dest => dest.data, opt => opt.MapFrom(src => src.DTA_AZIONE.AsDateTimeFormat()))
                     .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_OGGETTO))
                     .AfterMap((src, dest) =>
                     {
                         if (src.VAR_DESC_OGGETTO.StartsWith("Revoca"))
                             dest.codOperazione = "REVOCA";
                         if (src.VAR_DESC_OGGETTO.StartsWith("Ripristino"))
                             dest.codOperazione = "RIPRISTINO";
                         if (src.VAR_DESC_OGGETTO.StartsWith("Cede"))
                             dest.codOperazione = "CESSIONE";
                     });
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class StoriaVisibilitaEntity
        {
            public string? USERID_OPERATORE { get; set; }
            public string? VAR_DESC_CORR { get; set; }
            public string? VAR_DESC_OGGETTO { get; set; }
            public DateTime? DTA_AZIONE { get; set; }
        }
        #endregion
    }
}
