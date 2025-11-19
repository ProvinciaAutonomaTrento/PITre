// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ricerche;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.ExportModelliTrasmissioneUtente;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetQueryDocumentoPagingRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetQueryDocumentoPaging;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetQueryDocumentoPaging
{
    public class DocumentoGetQueryDocumentoPagingHandler : IRequestHandler<DocumentoGetQueryDocumentoPagingRequest, DocumentoGetQueryDocumentoPagingResult>
    {
        #region Public Members

        public DocumentoGetQueryDocumentoPagingHandler(ILogger<DocumentoGetQueryDocumentoPagingHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<DocumentoGetQueryDocumentoPagingResult> Handle(DocumentoGetQueryDocumentoPagingRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.InfoDocumento[] output = null!;
            int nRec = 0;
            int numTotPage = 0;
            List<SearchResultInfo> idProfiles = null!;

            try
            {
                var result = await this._mediator.Send(new Requests.DocumentoGetQueryDocumentoPagingCustom(
                        new DocsPaVO.utente.InfoUtente()
                        {
                            idPeople = request.idPeople,
                            idGruppo = request.idGruppo
                        },
                        request.queryList,
                        request.numPage,
                        request.security,
                        (request.comingPopUp ? 10 : 20),
                        request.getIdProfilesList,
                        false,
                        false,
                        null!,
                        null!));
                
                output = result.output.Select(s => this.AsInfoDocumento(s)).ToArray();
                nRec = result.nRec;
                numTotPage = result.numTotPage;
                idProfiles = result.idProfileList;
            }
            catch (Pi3Exception pi3Ex)
            {
                output = null!;

                this._logger.LogError(pi3Ex, pi3Ex.Message);
            }
            catch (Exception ex)
            {
                output = null!;

                this._logger.LogCritical(ex, ex.Message);
            }

            return new DocumentoGetQueryDocumentoPagingResult(
                   output,
                   numTotPage,
                   nRec,
                   idProfiles?.ToArray() ?? new SearchResultInfo[0]);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetQueryDocumentoPagingHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        protected DocsPaVO.documento.InfoDocumento AsInfoDocumento(DocsPaVO.Grids.SearchObject searchObject)
        {
            return new DocsPaVO.documento.InfoDocumento()
            {
                idProfile = searchObject.SearchObjectID,
                docNumber = searchObject.GetFieldValue<string>("D1"),
                numProt = searchObject.GetFieldValue<string>("CODICE"),
                idRegistro = searchObject.GetFieldValue<string>("ID_REGISTRO"),
                codRegistro = searchObject.GetFieldValue<string>("D2"),
                tipoProto = searchObject.GetFieldValue<string>("D3"),
                oggetto = searchObject.GetFieldValue<string>("D4"),
                Mittenti = searchObject.GetFieldValue<string>("D6")?.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList() ??  new List<string>(),
                Destinatari = searchObject.GetFieldValue<string>("D7")?.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                codiceApplicazione = searchObject.GetFieldValue<string>("COD_EXT_APP"),
                dataAnnullamento = searchObject.GetFieldValue<string>("D11"),
                acquisitaImmagine = searchObject.GetFieldValue<string>("D23"),
                personale = searchObject.GetFieldValue<string>("D15"),
                privato = searchObject.GetFieldValue<string>("D16"),
                ultimaNota = searchObject.GetFieldValue<string>("D17"),
                dataApertura = searchObject.GetFieldValue<string>("D9"),
                segnatura = searchObject.GetFieldValue<string>("D8"),
                mittDest = searchObject.GetFieldValue<string>("D3") == "I" ? (searchObject.GetFieldValue<string>("D7")?.Split(";", StringSplitOptions.RemoveEmptyEntries).ToArray() ?? new string[0]) : 
                    (searchObject.GetFieldValue<string>("D6")?.Split(";", StringSplitOptions.RemoveEmptyEntries).ToArray() ?? new string[0])
            };
        }

        

        #endregion
    }

    internal static class SearchObjectExtensions
    {
        public static T GetFieldValue<T>(this DocsPaVO.Grids.SearchObject searchObject, string fieldName)
        {
            if (searchObject.SearchObjectField?.Any(f => f.SearchObjectFieldID == fieldName) ?? false)
            {
                var value = searchObject.SearchObjectField.First(f => f.SearchObjectFieldID == fieldName).SearchObjectFieldValue;

                return (T)Convert.ChangeType(value, typeof(T));
            }
            else
                return default(T)!;
        }
    }
}