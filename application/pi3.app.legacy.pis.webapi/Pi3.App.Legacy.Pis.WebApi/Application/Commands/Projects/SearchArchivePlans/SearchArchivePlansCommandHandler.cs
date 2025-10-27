// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SearchArchivePlans
{
    // Richiede libreria MediatR
    public class SearchArchivePlansCommandHandler : IRequestHandler<SearchArchivePlansCommand, SearchArchivePlansCommandResponse>
    {
        #region Public Members

        public SearchArchivePlansCommandHandler(ILogger<SearchArchivePlansCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<SearchArchivePlansCommandResponse> Handle(SearchArchivePlansCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("SearchArchivePlans - START");

            SearchArchivePlansCommandResponse response = new SearchArchivePlansCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (request.Filters == null || request.Filters.Length == 0)
                {
                    throw new RestException("REQUIRED_FILTER");
                }

                //Chiamata al metodo CheckFilterType(request.Filters)
                RestUtils.CheckFilterTypes(request.Filters);
                #endregion

                #region implementazione
                try
                {

                    var query = (from a in _pi3DbContext.PianoConservazioneEntities
                                 join b in _pi3DbContext.RegistroEntities on a.ID_REGISTRO equals b.SYSTEM_ID
                                 join c in _pi3DbContext.ProjectEntities on a.ID_TITOLARIO equals c.SYSTEM_ID
                                 join d in _pi3DbContext.PianoConsAssTempoConsEntities on a.TEMPO_CONSERVAZIONE equals d.TEMPO_CONSERVAZIONE
                                 from e in _pi3DbContext.PianoConsTipoAttoEntities.Where(x => x.ID_PIANO_CONSERVAZIONE == a.SYSTEM_ID).DefaultIfEmpty()
                                 from f in _pi3DbContext.PianoConsTipoFascEntities.Where(x => x.ID_PIANO_CONSERVAZIONE == a.SYSTEM_ID).DefaultIfEmpty()
                                 select new
                                 {
                                     a,
                                     idregistro = b.SYSTEM_ID,
                                     coderegitro = b.VAR_CODICE,
                                     descRegistro = b.VAR_DESC_REGISTRO,
                                     statoRegistro = b.CHA_STATO,
                                     isRf = b.CHA_RF,
                                     idTitolario = c.SYSTEM_ID,
                                     descTitolario = c.DESCRIPTION,
                                     titoAttivo = c.CHA_STATO,
                                     d.TEMPO_CONSERVAZIONE_IN_ANNI,
                                     e.ID_TIPO_ATTO,
                                     f.ID_TIPO_FASC
                                 }
                                 );
                    foreach (var filtro in request.Filters)
                    {
                        switch (filtro.Name)
                        {

                            case "CLASSIFICATION_NODE_CODE":
                                query = query.Where(x => x.a.CODICE_CLASSIFICAZIONE == filtro.Value);
                                break;
                            case "CLASSIFICATION_NODE_ID":
                                query = query.Where(x => x.a.ID_CLASSIFICAZIONE == filtro.Value.AsLong());
                                break;
                            case "DESCRIPTION":
                                query = query.Where(x => x.a.TIPOLOGIA_FASCICOLO.ToUpper().Contains(filtro.Value.ToUpper()));
                                break;
                            case "CLASSIFICATION_SCHEME_ID":
                                query = query.Where(x => x.a.ID_TITOLARIO == filtro.Value.AsLong());
                                break;
                            case "REGISTER_ID":
                                query = query.Where(x => x.a.ID_REGISTRO == filtro.Value.AsLong());
                                break;
                            case "REGISTER_CODE":
                                var reg = DBUtils.getRegistroByCodAOO(filtro.Value, infoUtente.idAmministrazione, _pi3DbContext);
                                if (reg != null)
                                    query = query.Where(x => x.a.ID_REGISTRO == reg.systemId.AsLong());
                                else
                                    throw new RestException("REGISTER_NOT_FOUND");
                                break;
                            case "ID":
                                query = query.Where(x => x.a.SYSTEM_ID == filtro.Value.AsLong());
                                break;
                            case "DOCUMENT_TEMPLATE_ID":
                                query = query.Where(x => x.ID_TIPO_ATTO == filtro.Value.AsLong());
                                break;
                            case "PROJECT_TEMPLATE_ID":
                                query = query.Where(x => x.ID_TIPO_FASC == filtro.Value.AsLong());
                                break;
                        }
                    }

                    response.TotalArchivePlansNumber = query.Count();
                    if (response.TotalArchivePlansNumber > 0)
                    {
                        List<ArchivePlan> archivePlans = new List<ArchivePlan>();
                        if (request.PageNumber != null && request.PageNumber > 0 && request.ElementsInPage != null && request.ElementsInPage > 0)
                        {
                            var tempresults = query
                                .Skip(((request.PageNumber ?? 1) - 1) * (request.ElementsInPage ?? 20))
                                .Take(request.ElementsInPage ?? 20)
                                .ToList();
                            if (tempresults != null && tempresults.Any())
                            {

                                foreach (var item in tempresults)
                                {
                                    archivePlans.Add(new ArchivePlan()
                                    {
                                        Id = item.a.SYSTEM_ID.ToString(),
                                        ArchivePeriod = item.a.TEMPO_CONSERVAZIONE,
                                        ArchivePeriodYears = item.TEMPO_CONSERVAZIONE_IN_ANNI.ToString(),
                                        ClassificationNodeCode = item.a.CODICE_CLASSIFICAZIONE,
                                        ClassificationNodeId = item.a.ID_CLASSIFICAZIONE.ToString(),
                                        ClassificationScheme = new ClassificationSchemes.ClassificationScheme() { Id = item.idTitolario.ToString(), Description = item.descTitolario, Active = item.titoAttivo == "A" },
                                        Description = item.a.TIPOLOGIA_FASCICOLO,
                                        DocumentDiscardabilityNotes = item.a.NOTE_SCARTABILITA_DOC,
                                        DocumentNotes = item.a.NOTE_DOCUMENTI,
                                        ProceedingNumber = item.a.NUMERO_PROCEDIMENTO,
                                        ProceedingVoice = item.a.VOCE_PROCEDIMENTO,
                                        ProjectClosureNotes = item.a.NOTE_CHIUSURA_FASCICOLO,
                                        Register = new Registers.Register()
                                        {
                                            Code = item.coderegitro,
                                            Id = item.idregistro.ToString(),
                                            Description = item.descRegistro,
                                            State = item.statoRegistro,
                                            IsRF = item.isRf == "1"
                                        }
                                    });
                                }
                            }
                        }
                        else
                        {
                            var tempresults = query.ToList();
                            if(tempresults!= null && tempresults.Any())
                            {

                                foreach(var item in tempresults )
                                {
                                    archivePlans.Add(new ArchivePlan()
                                    {
                                        Id = item.a.SYSTEM_ID.ToString(),
                                        ArchivePeriod = item.a.TEMPO_CONSERVAZIONE,
                                        ArchivePeriodYears = item.TEMPO_CONSERVAZIONE_IN_ANNI.ToString(),
                                        ClassificationNodeCode = item.a.CODICE_CLASSIFICAZIONE,
                                        ClassificationNodeId = item.a.ID_CLASSIFICAZIONE.ToString(),
                                        ClassificationScheme = new ClassificationSchemes.ClassificationScheme() { Id = item.idTitolario.ToString(), Description = item.descTitolario, Active = item.titoAttivo == "A" },
                                        Description = item.a.TIPOLOGIA_FASCICOLO,
                                        DocumentDiscardabilityNotes = item.a.NOTE_SCARTABILITA_DOC,
                                        DocumentNotes = item.a.NOTE_DOCUMENTI,
                                        ProceedingNumber = item.a.NUMERO_PROCEDIMENTO,
                                        ProceedingVoice = item.a.VOCE_PROCEDIMENTO,
                                        ProjectClosureNotes = item.a.NOTE_CHIUSURA_FASCICOLO,
                                        Register = new Registers.Register()
                                        {
                                            Code = item.coderegitro,
                                            Id = item.idregistro.ToString(),
                                            Description = item.descRegistro,
                                            State = item.statoRegistro,
                                            IsRF = item.isRf == "1"
                                        }
                                    });
                                }
                            }
                        }
                        response.ArchivePlans = archivePlans.ToArray();
                    }


                }
                catch (Exception ex)
                {
                    throw new Exception("Errore nell'esecuzione della ricerca");
                }

                //TODO
                //throw new RestException("METHOD_NOT_IMPLEMENTED");

                #endregion

                response.Code = SearchArchivePlansResponseCode.OK;

                _logger.LogInformation("end SearchArchivePlans");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione SearchArchivePlans: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchArchivePlansCommandResponse();
                response.Code = SearchArchivePlansResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione SearchArchivePlans");
                response = new SearchArchivePlansCommandResponse();
                response.Code = SearchArchivePlansResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<SearchArchivePlansCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}