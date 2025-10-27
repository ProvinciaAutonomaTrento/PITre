// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoCercaDuplicatiInfoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoCercaDuplicatiInfo;
using EsitoRicercaDuplicatiEnum = DocsPaVO.documento.RicercaDuplicati.EsitoRicercaDuplicatiEnum;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoCercaDuplicatiInfo
{
    public class DocumentoCercaDuplicatiInfoHandler : IRequestHandler<DocumentoCercaDuplicatiInfoRequest, DocumentoCercaDuplicatiInfoResult>
    {
        #region Public Members

        public DocumentoCercaDuplicatiInfoHandler(ILogger<DocumentoCercaDuplicatiInfoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DocumentoCercaDuplicatiInfoResult> Handle(DocumentoCercaDuplicatiInfoRequest request, CancellationToken cancellationToken)
        {
            var output = EsitoRicercaDuplicatiEnum.NessunDuplicato;
            List<InfoProtocolloDuplicato> datiProtDupl = new List<InfoProtocolloDuplicato>();
            try
            {
                if (request.schedaDocumento.protocollo == null)
                    return new DocumentoCercaDuplicatiInfoResult(EsitoRicercaDuplicatiEnum.ProtocolloNullo, datiProtDupl.ToArray());

                if (request.schedaDocumento.protocollo.GetType() != typeof(DocsPaVO.documento.ProtocolloEntrata))
                    return new DocumentoCercaDuplicatiInfoResult(EsitoRicercaDuplicatiEnum.NoProtocolloIngresso, datiProtDupl.ToArray());

                ProtocolloEntrata protocolloEntrata = (DocsPaVO.documento.ProtocolloEntrata)request.schedaDocumento.protocollo;

                if (string.IsNullOrEmpty(protocolloEntrata.mittente.descrizione))
                    return new DocumentoCercaDuplicatiInfoResult(EsitoRicercaDuplicatiEnum.NoMittente, datiProtDupl.ToArray());

                if(string.IsNullOrEmpty(protocolloEntrata.dataProtocolloMittente) && string.IsNullOrEmpty(protocolloEntrata.descrizioneProtocolloMittente))
                    return new DocumentoCercaDuplicatiInfoResult(EsitoRicercaDuplicatiEnum.NessunDuplicato, datiProtDupl.ToArray());

                var idDestinatario = !string.IsNullOrEmpty(protocolloEntrata.mittente.systemId) ? new List<long> { protocolloEntrata.mittente.systemId.AsLong() }
                        : await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.VAR_DESC_CORR.ToUpper() == protocolloEntrata.mittente.descrizione.ToUpper())
                            .Select(c => c.SYSTEM_ID)
                            .ToListAsync();

                var idProfiles = this._dbContext.DocArrivoParEntities.AsNoTracking()
                    .Where(d => idDestinatario.Contains(d.ID_MITT_DEST.Value) && (new string[] { "M", "D" }).Contains(d.CHA_TIPO_MITT_DEST))
                    .Select(d => d.ID_PROFILE);

                var varProtoIn = !string.IsNullOrEmpty(protocolloEntrata.descrizioneProtocolloMittente) ? protocolloEntrata.descrizioneProtocolloMittente.ToUpper() : null;
                DateTime? dataProtoIn = !string.IsNullOrEmpty(protocolloEntrata.dataProtocolloMittente) ? protocolloEntrata.dataProtocolloMittente.AsDateTime().Date : null;
                var idRegistro = request.schedaDocumento.registro.systemId.AsLong();
                var systemId = !string.IsNullOrEmpty(request.schedaDocumento.systemId) ? request.schedaDocumento.systemId.AsLong() : 0;

                var profileEntity = await this._dbContext.ProfileEntities.AsNoTracking()
                    .Where(p => p.CHA_DA_PROTO == "0" && p.CHA_TIPO_PROTO == "A" && p.VAR_PROTO_IN.ToUpper() == varProtoIn && p.DTA_PROTO_IN == dataProtoIn && p.ID_REGISTRO == idRegistro && p.SYSTEM_ID != systemId
                        && idProfiles.Contains(p.SYSTEM_ID))
                    .Select(p => new
                    {
                        p.SYSTEM_ID,
                        p.VAR_SEGNATURA,
                        p.DTA_PROTO,
                        p.EXT,
                        p.NUM_PROTO,
                        p.ID_UO_PROT
                    })
                    .OrderByDescending(p => p.SYSTEM_ID)
                    .ToListAsync();

                foreach (var p in profileEntity)
                {
                    datiProtDupl.Add(new InfoProtocolloDuplicato
                    {
                        segnaturaProtocollo = p.VAR_SEGNATURA,
                        dataProtocollo = p.DTA_PROTO.ToString(),
                        docAcquisito = p.EXT,
                        idProfile = p.SYSTEM_ID.ToString(),
                        numProto = p.NUM_PROTO.ToString(),
                        uoProtocollatore = p.ID_UO_PROT != null ? await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == p.ID_UO_PROT).Select(c => c.VAR_DESC_CORR).FirstOrDefaultAsync() : string.Empty

                    });
                }

                if (datiProtDupl.Count > 0)
                {
                    output = EsitoRicercaDuplicatiEnum.DuplicatiMittenteData;
                    if (!string.IsNullOrEmpty(protocolloEntrata.descrizioneProtocolloMittente))
                        output = !string.IsNullOrEmpty(protocolloEntrata.mittente.descrizione) ? EsitoRicercaDuplicatiEnum.DuplicatiMittenteProtocollo : EsitoRicercaDuplicatiEnum.DuplicatiMittenteOggetto;
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = EsitoRicercaDuplicatiEnum.ErroreGenerico;
            }

            return new DocumentoCercaDuplicatiInfoResult(output, datiProtDupl.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoCercaDuplicatiInfoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
