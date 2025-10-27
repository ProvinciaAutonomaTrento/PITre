// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
using System.Configuration;
using System.Data;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.VerificaDirittiCestinaDocumento
{
    public class VerificaDirittiCestinaDocumentoHandler : IRequestHandler<Application.Requests.VerificaDirittiCestinaDocumento, VerificaDirittiCestinaDocumentoResult>
    {
        #region Public Members

        public VerificaDirittiCestinaDocumentoHandler(ILogger<VerificaDirittiCestinaDocumentoHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<VerificaDirittiCestinaDocumentoResult> Handle(Application.Requests.VerificaDirittiCestinaDocumento request, CancellationToken cancellationToken)
        {
            string result = "Del";
            SchedaDocumento schedaDoc = request.schedaDoc;

            try
            {
                var idPeople = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
                var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                if (schedaDoc != null)
                {
                    //Documento protocollato --> non si può cestinare
                    var docNumber = schedaDoc.docNumber.AsLong();
                    var isProtocollato = await _dbContext.ProfileEntities.AsNoTracking()
                                .AnyAsync(p => p.DOCNUMBER == docNumber && p.NUM_PROTO != null);

                    if (isProtocollato)
                        return new VerificaDirittiCestinaDocumentoResult(Messages.NoRimozioneProtocollati);

                    /* TOLTA LA PARTE CHE NON PERMETTEVA LA RIMOZIONE DI DOCUMENTI TRASMESSI ABILITATA TRAMITE CHIAVE ENABLE_CANC_DOC_TRASMESSI */

                    //Se l'utente che vuole rimuovere il documento non è il proprietario 
                    //allora lo può rimuovere solo se gli è stato trasmesso con ragione interoperabilità
                    if (schedaDoc.creatoreDocumento != null && !schedaDoc.creatoreDocumento.idPeople.Equals(idPeople))
                    {
                        var descRagione = "INTEROPERABILITA";
                        var idCorrGlobali = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.ID_GRUPPO == idGroup)
                            .Select(c => c.SYSTEM_ID)
                            .FirstOrDefaultAsync();

                        var existsTrasm = await _dbContext.TrasmissioneEntities.AsNoTracking()
                            .Join(_dbContext.TrasmSingolaEntities.AsNoTracking(),
                                  t => t.SYSTEM_ID,
                                  s => s.ID_TRASMISSIONE,
                                  (t, s) => new { t, s })
                            .Join(_dbContext.RagioneTrasmissioneEntities.AsNoTracking(),
                                  j => j.s.ID_RAGIONE,
                                  r => r.SYSTEM_ID,
                                  (j, r) => new { j.t, j.s, r })
                            .Join(_dbContext.ProfileEntities.AsNoTracking(),
                                  j => j.t.ID_PROFILE,
                                  p => p.SYSTEM_ID,
                                  (j, p) => new { j.t, j.s, j.r, p })
                            .AnyAsync(j => j.t.ID_PROFILE == docNumber && j.p.CHA_DA_PROTO == "1" && j.s.ID_CORR_GLOBALE == idCorrGlobali &&
                                      j.p.NUM_PROTO == null && j.r.VAR_DESC_RAGIONE.ToUpper() == descRagione);

                        if (!existsTrasm)
                            return new VerificaDirittiCestinaDocumentoResult(Messages.NoDirittiRimozione);
                    }

                    //Verifica se il documento è repertoriato
                    if (schedaDoc.tipologiaAtto != null)
                    {
                        var idTipoAtto = schedaDoc.tipologiaAtto.systemId.AsLong();
                        var q1 = _dbContext.ProfileEntities.AsNoTracking().Join(_dbContext.AssociazioneTemplatesEntities, p => p.SYSTEM_ID, at => Convert.ToInt64(at.DOC_NUMBER), (p, at) => new { p, at });
                        var q2 = q1.Join(_dbContext.OggettiCustomEntities.AsNoTracking(), q => q.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (q, oc) => new { q.p, q.at, oc });
                        var query = q2.Where(q => q.p.DOCNUMBER == docNumber
                            && q.at.ID_TEMPLATE == idTipoAtto
                            && q.oc.REPERTORIO == 1
                            && !string.IsNullOrEmpty(q.at.VALORE_OGGETTO_DB));
                        var queryResult = query.Any();

                        if (queryResult)
                            return new VerificaDirittiCestinaDocumentoResult(Messages.NoRimozioneRepertoriati);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new VerificaDirittiCestinaDocumentoResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<VerificaDirittiCestinaDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        #endregion
    }

}
