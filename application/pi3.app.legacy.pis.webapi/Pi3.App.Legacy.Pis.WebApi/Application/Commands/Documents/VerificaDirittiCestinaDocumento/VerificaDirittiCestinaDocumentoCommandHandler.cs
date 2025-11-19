// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadBigFileInChunks;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using DocsPaVO.documento;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.VerificaDirittiCestinaDocumento
{
    public class VerificaDirittiCestinaDocumentoCommandHandler : IRequestHandler<VerificaDirittiCestinaDocumentoCommand, VerificaDirittiCestinaDocumentoCommandResponse>
    {
        public VerificaDirittiCestinaDocumentoCommandHandler(ILogger<VerificaDirittiCestinaDocumentoCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<VerificaDirittiCestinaDocumentoCommandResponse> Handle(VerificaDirittiCestinaDocumentoCommand request, CancellationToken cancellationToken)
        {
            string result = "Del";
            SchedaDocumento schedaDoc = request.SchedaDoc;

            try
            {
                var idPeople = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser).ToString();
                var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();

                if (schedaDoc != null)
                {
                    //Documento protocollato --> non si può cestinare
                    long docNumber = Convert.ToInt64(schedaDoc.docNumber);
                    bool isProtocollato = _dbContext.ProfileEntities
                        .Where(p => p.DOCNUMBER == docNumber && p.NUM_PROTO != null)
                        .Select(x => x.NUM_PROTO)
                        .Count() > 0;

                    if (isProtocollato)
                    {
                        result = Messages.NoRimozioneProtocollati;
                        return new VerificaDirittiCestinaDocumentoCommandResponse()
                        {
                            Output = result
                        };
                    }

                    /* TOLTA LA PARTE CHE NON PERMETTEVA LA RIMOZIONE DI DOCUMENTI TRASMESSI ABILITATA TRAMITE CHIAVE ENABLE_CANC_DOC_TRASMESSI */

                    //Se l'utente che vuole rimuovere il documento non è il proprietario 
                    //allora lo può rimuovere solo se gli è stato trasmesso con ragione interoperabilità
                    if (schedaDoc.creatoreDocumento != null && !schedaDoc.creatoreDocumento.idPeople.Equals(idPeople))
                    {
                        var q1 = _dbContext.TrasmUtenteEntities.Join(_dbContext.TrasmSingolaEntities, tu => tu.ID_TRASM_SINGOLA, ts => Convert.ToInt32(ts.SYSTEM_ID), (tu, ts) => new { tu, ts });
                        var q2 = q1.Join(_dbContext.TrasmissioneEntities, q => q.ts.ID_TRASMISSIONE, t => Convert.ToInt32(t.SYSTEM_ID), (q, t) => new { q.tu, q.ts, t });
                        var q3 = q2.Join(_dbContext.RagioneTrasmissioneEntities, p => p.ts.ID_RAGIONE, r => Convert.ToInt32(r.SYSTEM_ID), (p, r) => new { p.tu, p.ts, p.t, r });
                        var q4 = q3.Join(_dbContext.ProfileEntities, x => x.t.ID_PROFILE, p => Convert.ToInt32(p.SYSTEM_ID), (x, p) => new { x.tu, x.ts, x.t, x.r, p });
                        var query = q4.Where(q => !q.p.CHA_DA_PROTO.Equals("0") && q.p.NUM_PROTO == null && q.t.ID_PROFILE == docNumber && q.r.VAR_DESC_RAGIONE.ToUpper().Equals("INTEROPERABILITA"));
                        var queryResult = query.ToList().Count();

                        if (queryResult == 0)
                        {
                            result = Messages.NoDirittiRimozione;
                            return new VerificaDirittiCestinaDocumentoCommandResponse()
                            {
                                Output = result
                            };
                        }
                    }

                    //Verifica se il documento è repertoriato
                    if (schedaDoc.tipologiaAtto != null)
                    {
                        var idTipoAtto = schedaDoc.tipologiaAtto.systemId;
                        var q1 = _dbContext.ProfileEntities.Join(_dbContext.AssociazioneTemplatesEntities, p => p.SYSTEM_ID, at => Convert.ToInt64(at.DOC_NUMBER), (p, at) => new { p, at });
                        var q2 = q1.Join(_dbContext.OggettiCustomEntities, q => q.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (q, oc) => new { q.p, q.at, oc });
                        var query = q2.Where(q => q.p.DOCNUMBER == docNumber
                            && q.at.ID_TEMPLATE.ToString().Equals(idTipoAtto)
                            && q.oc.REPERTORIO == 1
                            && !string.IsNullOrEmpty(q.at.VALORE_OGGETTO_DB));
                        var queryResult = query.ToList().Count();

                        if (queryResult != 0)
                        {
                            result = Messages.NoRimozioneRepertoriati;
                            return new VerificaDirittiCestinaDocumentoCommandResponse()
                            {
                                Output = result
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new VerificaDirittiCestinaDocumentoCommandResponse()
            {
                Output = result
            };
        }


        #region Private Members

        protected readonly ILogger<VerificaDirittiCestinaDocumentoCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }
}
