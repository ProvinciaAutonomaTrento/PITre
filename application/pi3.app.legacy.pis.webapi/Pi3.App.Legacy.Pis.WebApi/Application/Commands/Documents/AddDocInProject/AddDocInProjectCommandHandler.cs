// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using crypto;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using StackExchange.Redis;
using System.Collections;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities;
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Repository;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.AddDocInProject
{
    // Richiede libreria MediatR
    public class AddDocInProjectCommandHandler : IRequestHandler<AddDocInProjectCommand, AddDocInProjectCommandResponse>
    {
        #region Public Members

        public AddDocInProjectCommandHandler(ILogger<AddDocInProjectCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IWebMethodLoggerService loggerService, IAggregazioneDocumentaleRepository aggDocRepository, ITrasmissioneRepository trasmissioneRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._aggDocRepository = aggDocRepository;
            this._trasmissioneRepository = trasmissioneRepository;
            this._loggerService = loggerService;
        }

        public async Task<AddDocInProjectCommandResponse> Handle(AddDocInProjectCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("AddDocInProject - START");

            AddDocInProjectCommandResponse response = new AddDocInProjectCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (string.IsNullOrWhiteSpace(request.IdDocument))
                    throw new RestException("REQUIRED_ID");
                if (string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if (!string.IsNullOrEmpty(request.CodeProject) && !string.IsNullOrEmpty(request.IdProject) && !string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new RestException("REQUIRED_ONLY_CODE_OR_ID_PROJECT");
                }

                #endregion

                #region implementazione
                // Controllo visibilit� documento
                try
                {
                    await _pi3DbContext.AssertSecurityRights(request.IdDocument, infoUtente.idPeople, infoUtente.idGruppo, SecurityRightTypesEnum.Write);
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }



                //TODO: sottofascicoli
                string idfascicolo = "";

                long idSottoFascicolo = 0;

                long idfascPrincipale;

                string idTitAttivo = null;

                var titolari = DBUtils.getTitolariUtilizzabili(infoUtente.idAmministrazione, _pi3DbContext);
                if (titolari != null && titolari.Count > 0)
                {
                    foreach (DocsPaVO.amministrazione.OrgTitolario tempTit in titolari)
                    {
                        if (tempTit.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                        {
                            idTitAttivo = tempTit.ID;
                            break;
                        }
                    }
                }

                long idCartellaPrincipale = 0;

                AggregazioneDocumentale? fascicolo = null;

                if (!string.IsNullOrWhiteSpace(request.IdProject))
                {
                    var rigaFasc = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.SYSTEM_ID == request.IdProject.AsLong() select a).FirstOrDefault();
                    if (rigaFasc == null || rigaFasc.SYSTEM_ID < 1) throw new RestException("PROJECT_NOT_FOUND");
                    var idfasctemp = rigaFasc.SYSTEM_ID;
                    if (rigaFasc.CHA_TIPO_PROJ == "T")
                    {
                        idfasctemp = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.ID_PARENT == rigaFasc.SYSTEM_ID select a.SYSTEM_ID).FirstOrDefault();
                    }

                    //var idfasctemp = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.SYSTEM_ID == request.IdProject.AsLong() select a.SYSTEM_ID).FirstOrDefault();
                    idfascicolo = idfasctemp.ToString();
                    if (string.IsNullOrWhiteSpace(idfascicolo) || idfascicolo == "0") throw new RestException("PROJECT_NOT_FOUND");

                    idCartellaPrincipale = (from a in _pi3DbContext.ProjectEntities where a.ID_PARENT == idfascicolo.AsLong() && a.ID_FASCICOLO == idfascicolo.AsLong() select a.SYSTEM_ID).FirstOrDefault();



                    fascicolo = await _aggDocRepository.Get(infoUtente.idAmministrazione, idCartellaPrincipale.ToString());
                }
                else if (request.CodeProject.IndexOf("//") > -1)
                {
                    string separatore = "//";
                    // MODIFICA per inserimento in sottocartelle
                    string[] separatoreAr = new string[] { separatore };

                    string[] pathCartelle = request.CodeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);

                    var rigaFasc = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.VAR_CODICE == pathCartelle[0] && a.ID_TITOLARIO == idTitAttivo.AsLong() select a).FirstOrDefault();
                    if (rigaFasc == null || rigaFasc.SYSTEM_ID < 1) throw new RestException("PROJECT_NOT_FOUND");
                    //idfascPrincipale = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.VAR_CODICE == pathCartelle[0] && a.ID_TITOLARIO == idTitAttivo.AsLong() select a.SYSTEM_ID).FirstOrDefault();
                    idfascPrincipale = rigaFasc.SYSTEM_ID;
                    if (rigaFasc.CHA_TIPO_PROJ == "T")
                    {
                        idfascPrincipale = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.ID_PARENT == rigaFasc.SYSTEM_ID select a.SYSTEM_ID).FirstOrDefault();
                    }

                    idfascicolo = idfascPrincipale.ToString();

                    if (string.IsNullOrWhiteSpace(idfascicolo) || idfascicolo == "0") throw new RestException("PROJECT_NOT_FOUND");

                    var idCartellaPrincipale2 = (from a in _pi3DbContext.ProjectEntities where a.ID_PARENT == idfascicolo.AsLong() && a.ID_FASCICOLO == idfascicolo.AsLong() select a.SYSTEM_ID).FirstOrDefault();


                    var CartellaPrincipale2 = await _aggDocRepository.Get(infoUtente.idAmministrazione, idCartellaPrincipale2.ToString(), new ILoadBehavior[1] { new GetAggregatoDocumentaleLoadBehavior() { BypassSecurityCheck = true, LoadFolderHierarchy = true } });

                    if (pathCartelle.Length > 1)
                    {
                        var sottocartelle = await (from a in _pi3DbContext.ProjectEntities
                                                   where a.ID_FASCICOLO == idfascPrincipale
                                                   select
                                                   new Folder()
                                                   {
                                                       Id = a.SYSTEM_ID.ToString(),
                                                       Description = a.DESCRIPTION,
                                                       IdParent = a.ID_PARENT.ToString(),
                                                       IdProject = a.ID_FASCICOLO.ToString()


                                                   }).ToListAsync();
                        idSottoFascicolo = CartellaPrincipale2.Id.AsLong();

                        string idFascicoloPrincipale = idfascPrincipale.ToString();
                        try
                        {
                            for (int i = 0; i < pathCartelle.Length; i++)
                            {

                                //var folderSelezionato = ((from f in sottocartelle where f.IdParent == idSottoFascicolo.ToString() && f.Description == pathCartelle[i] select f).ToList().First());

                                var folderSelezionato = ((from f in sottocartelle where f.IdProject == idFascicoloPrincipale.ToString() && f.Description == pathCartelle[i] select f).ToList().First());
                                idSottoFascicolo = folderSelezionato.Id.AsLong();
                            }

                        }
                        catch (Exception e)
                        {
                            throw new Exception("Cartella non trovata nel fascicolo");
                        }


                        fascicolo = await _aggDocRepository.Get(infoUtente.idAmministrazione, idCartellaPrincipale2.ToString(), new ILoadBehavior[1] {
                                new GetAggregatoDocumentaleLoadBehavior()
                                {
                                    BypassSecurityCheck = true,
                                    LoadFolderHierarchy = true,
                                }
                            });

                    }
                    else
                    {

                        var idfasctemp = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.VAR_CODICE == request.CodeProject && a.ID_TITOLARIO == idTitAttivo.AsLong() select a.SYSTEM_ID).FirstOrDefault();
                        idfascicolo = idfasctemp.ToString();
                        if (string.IsNullOrWhiteSpace(idfascicolo) || idfascicolo == "0") throw new RestException("PROJECT_NOT_FOUND");


                        idCartellaPrincipale = (from a in _pi3DbContext.ProjectEntities where a.ID_PARENT == idfascicolo.AsLong() && a.ID_FASCICOLO == idfascicolo.AsLong() select a.SYSTEM_ID).FirstOrDefault();

                        fascicolo = await _aggDocRepository.Get(infoUtente.idAmministrazione, idCartellaPrincipale.ToString());
                    }

                }
                else
                {
                    //var idfasctemp = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.VAR_CODICE == request.CodeProject && a.ID_TITOLARIO == idTitAttivo.AsLong() select a.SYSTEM_ID).FirstOrDefault();
                    var rigaFasc = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.VAR_CODICE == request.CodeProject && a.ID_TITOLARIO == idTitAttivo.AsLong() select a).FirstOrDefault();
                    if (rigaFasc == null || rigaFasc.SYSTEM_ID < 1) throw new RestException("PROJECT_NOT_FOUND");
                    var idfasctemp = rigaFasc.SYSTEM_ID;
                    if (rigaFasc.CHA_TIPO_PROJ == "T")
                    {
                        idfasctemp = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.CHA_TIPO_FASCICOLO == "G" && a.ID_PARENT == rigaFasc.SYSTEM_ID select a.SYSTEM_ID).FirstOrDefault();
                    }

                    idfascicolo = idfasctemp.ToString();
                    if (string.IsNullOrWhiteSpace(idfascicolo) || idfascicolo == "0") throw new RestException("PROJECT_NOT_FOUND");


                    idCartellaPrincipale = (from a in _pi3DbContext.ProjectEntities where a.ID_PARENT == idfascicolo.AsLong() && a.ID_FASCICOLO == idfascicolo.AsLong() select a.SYSTEM_ID).FirstOrDefault();

                    fascicolo = await _aggDocRepository.Get(infoUtente.idAmministrazione, idCartellaPrincipale.ToString());
                }


                if (fascicolo == null) throw new RestException("PROJECT_NOT_FOUND");

                //Controllo visibilit� fascicolo
                var securityRigths = await _pi3DbContext.GetSecurityRights(idfascicolo, infoUtente.idPeople, infoUtente.idGruppo);
                switch (securityRigths)
                {
                    case SecurityRightTypesEnum.Deny:
                        throw new RestException("PROJECT_NOT_VISIBLE");
                    case SecurityRightTypesEnum.Read:
                        //cerco una trasmissione pending al ruolo con workflow
                        var trasmPendente = await DBUtils.GetTrasmPendenteConWorkflowFascicolo(idfascicolo, infoUtente.idCorrGlobali, infoUtente.idPeople, _pi3DbContext);
                        if (trasmPendente != 0)
                        {
                            //Accetta
                            var aggregate = await _trasmissioneRepository.Get(infoUtente.idAmministrazione, trasmPendente.ToString());
                            aggregate.Accetta(infoUtente.idGruppo.ToString(), infoUtente.idPeople, new Accetta()
                            {
                                Data = DateTime.Now,
                                //Note = new TextValue(trasmUtente.noteAccettazione), //Non ce le metto
                                IdDelegato = null
                            });
                            await _trasmissioneRepository.Update(aggregate);
                            break;
                        }
                        else
                            throw new RestException("PROJECT_NOT_EDITABLE");
                    case SecurityRightTypesEnum.Write:
                    case SecurityRightTypesEnum.FullControl:
                        break;
                    default:
                        throw new RestException("PROJECT_NOT_FOUND");
                }

                //Controllo se il fascicolo � chiuso 
                if(fascicolo.DataChiusura.HasValue)
                    throw new RestException("PROJECT_CLOSED");

                if (idSottoFascicolo > 0)
                {
                    fascicolo.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc() { Identiticativo = request.IdDocument }, idSottoFascicolo.ToString());
                }
                else
                {
                    fascicolo.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc() { Identiticativo = request.IdDocument });

                }
                await _aggDocRepository.Update(fascicolo);
                response.ResultMessage = $"Documento {request.IdDocument} inserito nel fascicolo {fascicolo.DatiRegistrazione.Codice}";
                await _loggerService.LogOK("DOCADDINFASC",
                        request.IdDocument, $"PIS REST: Documento {request.IdDocument} inserito nel fascicolo {fascicolo.DatiRegistrazione.Codice}",
                        null, infoUtente.codWorkingApplication);

                #endregion

                response.Code = MessageResponseCode.OK;

                _logger.LogInformation("end AddDocInProject");



            }

            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione AddDocInProject: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new AddDocInProjectCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione AddDocInProject");
                response = new AddDocInProjectCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AddDocInProjectCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IAggregazioneDocumentaleRepository _aggDocRepository;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _loggerService;




        #endregion
    }

}