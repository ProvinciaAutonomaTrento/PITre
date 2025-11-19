// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.AddSegnaturaXml;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using StackExchange.Redis;
using System.Collections;
using System.Linq;
using System.Xml.Linq;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ProtocolPredisposed
{
    // Richiede libreria MediatR
    public class ProtocolPredisposedCommandHandler : IRequestHandler<ProtocolPredisposedCommand, ProtocolPredisposedCommandResponse>
    {
        #region Public Members

        public ProtocolPredisposedCommandHandler(ILogger<ProtocolPredisposedCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IDocumentoAmministrativoRepository docAmmRepository, IWebMethodLoggerService loggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._loggerService = loggerService;
            this._docAmmRepository = docAmmRepository;
        }

        public async Task<ProtocolPredisposedCommandResponse> Handle(ProtocolPredisposedCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("ProtocolPredisposed - START");

            ProtocolPredisposedCommandResponse response = new ProtocolPredisposedCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new RestException("REQUIRED_DOCUMENT");
                }

                if (string.IsNullOrEmpty(request.CodeRegister))
                {
                    throw new RestException("REQUIRED_REGISTER");
                }
                #endregion

                #region implementazione
                //TODO: protocollazione su RF

                // Controllo visibilit� documento
                try
                {
                    await _pi3DbContext.AssertSecurityRights(request.IdDocument, infoUtente.idPeople, infoUtente.idGruppo, SecurityRightTypesEnum.Write);
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                var registro = DBUtils.getRegistroByCodAOO(request.CodeRegister, infoUtente.idAmministrazione, _pi3DbContext);
                if (registro == null) throw new RestException("REGISTER_NOT_FOUND");

                var controllo = (from a in _pi3DbContext.ProfileEntities where a.SYSTEM_ID == request.IdDocument.AsLong() && a.CHA_DA_PROTO == "1" select a).FirstOrDefault();
                if (controllo == null || controllo.SYSTEM_ID == 0) throw new RestException("PREDISPOSED_NOT_VALID");
                if (!string.IsNullOrEmpty(request.CodeRF))
                {
                    var reg = DBUtils.getRegistroByCodAOO(request.CodeRF, infoUtente.idAmministrazione, _pi3DbContext);
                    if (!(reg != null && reg.chaRF == "1"))
                    {
                        throw new RestException("RF_NOT_FOUND");
                    }
                    else
                    {
                        registro = reg;
                    }

                }

                if (!string.IsNullOrEmpty(request.IdDocument))
                {
                    AssertLibroFirma(request.IdDocument);
                    AssertDocumentoProtocollato(request.IdDocument);
                }

                var predisposto = await _docAmmRepository.Get(infoUtente.idAmministrazione, request.IdDocument, 
                new Core.SeedWork.ILoadBehavior[1]{ new Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories.GetDocumentoAmministrativoLoadBehavior()
                {
                    LoadMittentiDestinatari = true
                }});
                if (predisposto == null) throw new RestException("DOCUMENT_NOT_FOUND");
                
                
                predisposto.RichiediRegistrazione(new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.DatiRichiestaRegistrazione()
                {
                    DatiRegistro = new DatiRegistro() { IdRegistro = registro.systemId },
                    Predisponi = false
                });

                var errori = predisposto.GetErrors();

                await _docAmmRepository.Update(predisposto);

                if (predisposto.TipologiaFlusso == TipologiaFlussoEnum.U)
                {
                    await this._mediator.Send(new AddSegnaturaXmlCommand()
                    {
                        DocNumber = request.IdDocument
                    });
                }

                response.Document = RestUtils.getDocFromAggregate(predisposto, _pi3DbContext);
                var note = DBUtils.getNoteOggetto(predisposto.Id, _pi3DbContext);
                if (note != null)
                {
                    response.Document.Note = note.ToArray();
                }
                response.Document.Template = DBUtils.getTemplateFromDocumentId(predisposto.Id, _pi3DbContext);
                response.Document.ParentDocument = await DBUtils.getParentDocInfoFromDocId(predisposto.Id, _pi3DbContext);
                var childDocs = await DBUtils.getChildDocsInfoFromDocId(predisposto.Id, _pi3DbContext);
                if (childDocs != null && childDocs.Any())
                    response.Document.LinkedDocuments = childDocs.ToArray();

                //var linkedDocs = this._pi3DbContext.ProfileEntities.AsNoTracking().Where(p => p.ID_PARENT == request.IdDocument.AsLong()).Select(p => new
                //{
                //    DocNumber = p.SYSTEM_ID,
                //    Name = p.DOCNAME,
                //    Path = p.CHA_TIPO_PROTO,
                //    VersionLabel = "LINKED",
                //    Description = p.VAR_PROF_OGGETTO
                //}).Union(this._pi3DbContext.ProfileEntities.AsNoTracking().Where(p => 
                //this._pi3DbContext.ProfileEntities.AsNoTracking().Where(o => o.SYSTEM_ID == request.IdDocument.AsLong() && o.ID_PARENT == p.SYSTEM_ID).Any())
                //.Select(p => new
                //{
                //    DocNumber =p.SYSTEM_ID,
                //    Name = p.DOCNAME,
                //    Path = p.CHA_TIPO_PROTO,
                //    VersionLabel = "PARENT",
                //    Description = p.VAR_PROF_OGGETTO
                //}));

                //if (linkedDocs.Any())
                //{
                //    List<LinkedDocument> linkedDocs2 = new List<LinkedDocument>();

                //    foreach (var doc in linkedDocs)
                //    {
                //        if (doc.VersionLabel.ToUpper() == "PARENT")
                //        {
                //            response.Document.ParentDocument = new()
                //            {
                //                Id = doc.DocNumber.ToString(),
                //                Object = doc.Description,
                //                DocumentType = doc.Path,
                //                LinkType = doc.VersionLabel
                //            };
                //            if (doc.Name != response.Document.ParentDocument.Id)
                //                response.Document.ParentDocument.Signature = doc.Name;
                                
                //        }
                //        else if (doc.VersionLabel.ToUpper() == "LINKED")
                //        {
                //            LinkedDocument linkedDoc = new()
                //            {
                //                Id = doc.DocNumber.ToString(),
                //                Object = doc.Description,
                //                DocumentType = doc.Path,
                //                LinkType = doc.VersionLabel
                //            };
                //            if (doc.Name != linkedDoc.Id)
                //                linkedDoc.Signature = doc.Name;

                //            linkedDocs2.Add(linkedDoc);
                //        }
                //    }
                //    if(linkedDocs2.Count > 0)
                //    {
                //        response.Document.LinkedDocuments = linkedDocs2.ToArray();
                //    }
                //}

                await _loggerService.LogOK("RECORDPREDISPOSED",
                        predisposto.Id, $"PIS REST: Protocollazione del documento ID: {predisposto.Id}, Tipo documento {response.Document.DocumentType}, Segnatura {response.Document.Signature}",
                        null, infoUtente.codWorkingApplication);

                #endregion

                response.Code = GetDocumentResponseCode.OK;

                _logger.LogInformation("end ProtocolPredisposed");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione ProtocolPredisposed: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new ProtocolPredisposedCommandResponse();
                response.Code = GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione ProtocolPredisposed");
                response = new ProtocolPredisposedCommandResponse();
                response.Code = GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<ProtocolPredisposedCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IDocumentoAmministrativoRepository _docAmmRepository;
        protected readonly IWebMethodLoggerService _loggerService;

        protected void AssertDocumentoProtocollato(string? docnumber)
        {

            if (!string.IsNullOrEmpty(docnumber))
            {
                var docnumberAsLong = docnumber.AsLong();
                if (this._pi3DbContext.ProfileEntities.Any(p => p.DOCNUMBER == docnumberAsLong && p.NUM_PROTO != null))
                {
                    throw new Exception(docnumber);
                }
            }
        }
        protected void AssertLibroFirma(string? docNumber)
        {
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

            if (!string.IsNullOrEmpty(docNumber))
            {
                var docnumberAsLong = docNumber.AsLong();
                var istanzaPassoFirmaAttesaEntity = this._pi3DbContext.IstanzaProcessoFirmaEntities
                    .Join(this._pi3DbContext.IstanzaPassoFirmaEntities, p => p.ID_ISTANZA, a => a.ID_ISTANZA_PROCESSO, (p, i) => new { p, i })
                    .Where(j => j.p.ID_DOCUMENTO == docnumberAsLong && j.p.STATO == "IN_EXEC" && j.i.STATO_PASSO == "LOOK")
                    .Select(j => new
                    {
                        j.i.ID_RUOLO_COINVOLTO,
                        j.i.ID_UTENTE_COINVOLTO,
                        j.i.ID_UTENTE_LOCKER,
                        j.i.TIPO_FIRMA
                    })
                    .FirstOrDefault();

                if (istanzaPassoFirmaAttesaEntity != null)
                {
                    if (istanzaPassoFirmaAttesaEntity.ID_RUOLO_COINVOLTO != idGroup
                        || (istanzaPassoFirmaAttesaEntity.ID_UTENTE_COINVOLTO != null && istanzaPassoFirmaAttesaEntity.ID_UTENTE_COINVOLTO != idUser)
                        || (istanzaPassoFirmaAttesaEntity.ID_UTENTE_LOCKER != null && istanzaPassoFirmaAttesaEntity.ID_UTENTE_LOCKER != idUser)
                        || !istanzaPassoFirmaAttesaEntity.TIPO_FIRMA.Equals(Azione.RECORD_PREDISPOSED.ToString()))
                    {
                        throw new Exception();
                    }

                }
            }
        }

        #endregion
    }

}