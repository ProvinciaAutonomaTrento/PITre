// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetDocumentoSmistamentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetDocumentoSmistamento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetDocumentoSmistamento
{
    public class GetDocumentoSmistamentoHandler : IRequestHandler<GetDocumentoSmistamentoRequest, GetDocumentoSmistamentoResult>
    {
        #region Public Members

        public GetDocumentoSmistamentoHandler(
            ILogger<GetDocumentoSmistamentoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetDocumentoSmistamentoResult> Handle(GetDocumentoSmistamentoRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.Smistamento.DocumentoSmistamento output = null!;

            try
            {
                var getDettaglioDocumentoResult = await this._mediator.Send(
                    new Requests.DocumentoGetDettaglioDocumento(
                            request.infoUtente, request.idDocumento, request.idDocumento),
                    cancellationToken);
                
                var schedaDocumento = getDettaglioDocumentoResult.output;
                if (schedaDocumento != null)
                {
                    DocsPaVO.documento.FileDocumento? fileDocumento = null;

                    if (schedaDocumento.documenti != null
                        && schedaDocumento.documenti.Length > 0
                        && !string.IsNullOrWhiteSpace(schedaDocumento.documenti[0].impronta))
                    {
                        if (request.content)
                        {
                            var getFileResult = await this._mediator.Send(
                                new Requests.DocumentoGetFile(
                                    schedaDocumento.documenti[0],
                                    request.infoUtente),
                                cancellationToken);

                            fileDocumento = getFileResult.output;
                        }

                        if (fileDocumento == null)
                        {
                            // Non � stato possibile reperire il contenuto del documento,
                            // sono comunque estratte le informazioni del file
                            var getInfoFileResult = await this._mediator.Send(
                            new Requests.DocumentoGetInfoFile(
                                schedaDocumento.documenti[0],
                                request.infoUtente),
                            cancellationToken);

                            fileDocumento = getInfoFileResult.output;
                        }
                    }

                    string mittenteDocumento = null!;
                    string[] destinatariDocumento = new string[0];

                    if (schedaDocumento.protocollo is DocsPaVO.documento.ProtocolloEntrata)
                    {
                        var protocollo = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;

                        mittenteDocumento = protocollo.mittente! != null!
                                ? protocollo.mittente.descrizione : null!;
                    }
                    else if (schedaDocumento.protocollo is DocsPaVO.documento.ProtocolloUscita)
                    {
                        var protocollo = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;

                        destinatariDocumento = protocollo.destinatari! != null!
                            ? protocollo.destinatari.Select(d => new string(d.descrizione)).ToArray()
                            : new string[0];
                    }

                    output = new DocsPaVO.Smistamento.DocumentoSmistamento()
                    {
                        IDDocumento = schedaDocumento.systemId,
                        DocNumber = schedaDocumento.docNumber,
                        TipoDocumento = schedaDocumento.tipoProto,
                        Oggetto = schedaDocumento.oggetto.descrizione,
                        Segnatura = schedaDocumento.protocollo! != null
                            ? schedaDocumento.protocollo.segnatura : null,
                        IDRegistro = schedaDocumento.registro != null
                            ? schedaDocumento.registro.systemId : null,
                        DataCreazione = schedaDocumento.dataCreazione,
                        Versioni = schedaDocumento.documenti!.Length.ToString(),
                        Allegati = schedaDocumento.allegati.Length.ToString(),
                        TipologyDescription = schedaDocumento.tipologiaAtto != null
                            ? schedaDocumento.tipologiaAtto.descrizione : null,
                        MittenteDocumento = mittenteDocumento,
                        DestinatariDocumento = destinatariDocumento,
                        ImmagineDocumento = fileDocumento!
                    };
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                output = null!;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                output = null!;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new GetDocumentoSmistamentoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentoSmistamentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }
}