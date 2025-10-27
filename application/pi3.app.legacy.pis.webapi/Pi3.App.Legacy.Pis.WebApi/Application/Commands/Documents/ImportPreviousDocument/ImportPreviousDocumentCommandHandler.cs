// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Ocsp;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.AmmGetInfoAmmCorrente;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentWithFromPrevious;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.RemoveDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using StackExchange.Redis;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ImportPreviousDocument
{
    // Richiede libreria MediatR
    public class ImportPreviousDocumentCommandHandler : IRequestHandler<ImportPreviousDocumentCommand, ImportPreviousDocumentCommandResponse>
    {
        #region Public Members

        public ImportPreviousDocumentCommandHandler(
            IWebMethodLoggerService loggerService,ILogger<ImportPreviousDocumentCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._loggerService = loggerService;
        }

        public async Task<ImportPreviousDocumentCommandResponse> Handle(ImportPreviousDocumentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("ImportPreviousDocument - START");

            ImportPreviousDocumentCommandResponse response = new ImportPreviousDocumentCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (request == null || request.Document == null)
                {
                    throw new RestException("REQUIRED_DOCUMENT");
                }
                if (request != null && string.IsNullOrEmpty(request.Document.DocumentType))
                {
                    throw new RestException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && string.IsNullOrEmpty(request.Document.Object))
                {
                    throw new RestException("MISSING_OBJECT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (!request.Document.DocumentType.ToUpper().Equals("A") && !request.Document.DocumentType.ToUpper().Equals("P") && !request.Document.DocumentType.ToUpper().Equals("I") && !request.Document.DocumentType.ToUpper().Equals("G")))
                {
                    throw new RestException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (request.Document.DocumentType.ToUpper().Equals("A") || request.Document.DocumentType.ToUpper().Equals("P") || request.Document.DocumentType.ToUpper().Equals("I")) && string.IsNullOrEmpty(request.CodeRegister))
                {
                    throw new RestException("REQUIRED_REGISTER");
                }
                if (request != null && string.IsNullOrEmpty(request.Document.ProtocolNumber))
                {
                    throw new RestException("REQUIRED_PROTOCOL_NUMBER_IMPPREVIOUS");
                }
                if (request != null && string.IsNullOrEmpty(request.Document.ProtocolYear))
                {
                    throw new RestException("REQUIRED_PROTOCOL_YEAR_IMPPREVIOUS");
                }

                string formatoSegnatura = DBUtils.getFormatoSegnatura(infoUtente.idAmministrazione,_pi3DbContext);
                if (formatoSegnatura.ToUpper().Contains("COD_RF_PROT") && request.Document.DocumentType != "G" && string.IsNullOrWhiteSpace(request.CodeRF))
                {
                    throw new RestException("REQUIRED_CODERF");
                }

                request.Document.Predisposed = true;
                DocsPaVO.utente.Registro reg = null;
                if (request.Document.DocumentType != "G")
                {
                    reg = DBUtils.getRegistroByCodAOO(request.CodeRegister,infoUtente.idAmministrazione,this._pi3DbContext);
                    if (reg == null)
                    {
                        //Registro mancante
                        throw new RestException("REGISTER_NOT_FOUND");
                    }
                }

                #endregion

                #region implementazione

                var createDocumentRequest = new CreateDocumentWithFromPreviousCommand()
                {
                    Document = request.Document,
                    CodeRegister = request.CodeRegister,
                    CodeRF = request.CodeRF,
                    FromPregressi = true
                };

                var createDocumentResponse = await this._mediator.Send(createDocumentRequest);

                if (createDocumentResponse == null)
                {
                    throw new Exception();
                }
                switch (createDocumentResponse.Code)
                {
                    case CreateDocumentWithFromPreviousResponseCode.OK:
                        response.Document = await this.ImportPreviousDocument(createDocumentRequest, formatoSegnatura, reg, infoUtente, createDocumentResponse);
                        break;
                    case CreateDocumentWithFromPreviousResponseCode.SYSTEM_ERROR:
                        throw createDocumentResponse.Ex;
                    case CreateDocumentWithFromPreviousResponseCode.PIS_ERROR:
                        throw createDocumentResponse.Ex;
                }

                #endregion

                response.Code = CreateDocumentResponseCode.OK;

                _logger.LogInformation("end ImportPreviousDocument");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione ImportPreviousDocument: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new ImportPreviousDocumentCommandResponse();
                response.Code = CreateDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione ImportPreviousDocument");
                response = new ImportPreviousDocumentCommandResponse();
                response.Code = CreateDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<ImportPreviousDocumentCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _loggerService;


        private async Task<Document> ImportPreviousDocument(CreateDocumentWithFromPreviousCommand request, string formatoSegnatura, Registro reg, InfoUtente infoUtente, CreateDocumentWithFromPreviousCommandResponse response)
        {
            Document? output = null;
            if (request.Document.DocumentType != "G")
            {
                string segnatura = formatoSegnatura.ToUpper();
                if (segnatura.Contains("COD_AMM"))
                    segnatura = segnatura.Replace("COD_AMM", (await this._mediator.Send(new AmmGetInfoAmmCorrenteCommand() 
                    { 
                        IdAmm = infoUtente.idAmministrazione
                    }  )).Output.Codice);
                if (segnatura.Contains("COD_REG"))
                    segnatura = segnatura.Replace("COD_REG", reg.codRegistro);

                if (segnatura.Contains("DATA_COMP"))
                    segnatura = segnatura.Replace("DATA_COMP", request.Document.ProtocolDate);
                if (segnatura.Contains("IN_OUT"))
                    segnatura = segnatura.Replace("IN_OUT", request.Document.DocumentType);
                if (segnatura.Contains("COD_RF_PROT"))
                    segnatura = segnatura.Replace("COD_RF_PROT", request.CodeRF.ToUpper());
                if (segnatura.Contains("DATA_ANNO"))
                    segnatura = segnatura.Replace("DATA_ANNO", request.Document.ProtocolYear);
                if (segnatura.Contains("NUM_PROTO"))
                    segnatura = segnatura.Replace("NUM_PROTO", request.Document.ProtocolNumber.PadLeft(7, '0'));

                try
                {
                    await this.ImportPregressiRestUpdProfile(response.Document.Id, request.Document.ProtocolNumber, request.Document.ProtocolYear, request.Document.ProtocolDate, reg.systemId, reg.codRegistro, segnatura);
                }
                catch (Exception ex)
                {
                    await this.MigrazioneSpostaDocInCest(response.Document.Id);
                    throw new Exception(string.Format(Messages.MigrErr, ex.Message, ex.StackTrace, ex.ToString()));
                }

                GetDocumentCommand reqGetDoc = new()
                {
                    GetFile = false,
                    IdDocument = response.Document.Id
                };
                var respGetDoc = await this._mediator.Send(reqGetDoc);
                if (respGetDoc != null && respGetDoc.Document != null)
                {
                    await _loggerService.LogOK("DOCUMENTOADDDOCGRIGIA",
                    respGetDoc.Document.Id, $"Creazione del documento pregresso {respGetDoc.Document.Id}. Tipo documento: { respGetDoc.Document.DocumentType}. Predisposto: {respGetDoc.Document.Predisposed}",
                    null, infoUtente.codWorkingApplication);

                    return respGetDoc.Document;
                }

            }

            return output;
        }

        private async Task<bool> MigrazioneSpostaDocInCest(string idProfile)
        {
            bool output = false;
            var docEntToUp = await this._pi3DbContext.ProfileEntities.Where(p => p.DOCNUMBER == idProfile.AsLong()).FirstOrDefaultAsync();
            if (docEntToUp != null)
            {
                docEntToUp.CHA_IN_CESTINO = "1";
                docEntToUp.VAR_CHIAVE_PROTO = idProfile;
                output = await ((DbContext)this._pi3DbContext).SaveChangesAsync() > 0;
            }

            return output;
        }

        private async Task ImportPregressiRestUpdProfile(string idDocument, string numProto, string numAnnoProto,
            string dtaProto, string idRegistro, string codRegistro, string segnatura)
        {
            var profileEntity = this._pi3DbContext.ProfileEntities.Where(p => p.SYSTEM_ID == idDocument.AsLong()).FirstOrDefault();

            if (profileEntity != null)
            {
                var chiaveProtocollo = $"{numProto}_{numAnnoProto}_{idRegistro}";


                profileEntity.LAST_EDIT_DATE = await this._pi3DbContext.GetSystemDateTime();

                try
                {
                    profileEntity.CHA_DA_PROTO = "0";
                    profileEntity.NUM_PROTO = !string.IsNullOrEmpty(numProto) ? numProto.AsLong() : null;
                    profileEntity.DTA_PROTO = !string.IsNullOrEmpty(dtaProto) ? dtaProto.AsDateTime() : null;
                    profileEntity.NUM_ANNO_PROTO = !string.IsNullOrEmpty(numAnnoProto) ? numAnnoProto.AsLong() : null;
                    profileEntity.VAR_CHIAVE_PROTO = chiaveProtocollo;
                    profileEntity.ID_REGISTRO = !string.IsNullOrEmpty(idRegistro) ? idRegistro.AsLong() : null;
                    profileEntity.DOCNAME = segnatura;
                    profileEntity.VAR_SEGNATURA = segnatura;

                    await ((DbContext)this._pi3DbContext).SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        #endregion
    }

}