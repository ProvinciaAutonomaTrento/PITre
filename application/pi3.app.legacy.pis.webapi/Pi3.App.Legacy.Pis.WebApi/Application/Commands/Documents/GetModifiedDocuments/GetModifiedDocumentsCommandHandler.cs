// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using crypto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using System.Linq;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetModifiedDocuments
{
    // Richiede libreria MediatR
    public class GetModifiedDocumentsCommandHandler : IRequestHandler<GetModifiedDocumentsCommand, GetModifiedDocumentsCommandResponse>
    {
        #region Public Members

        public GetModifiedDocumentsCommandHandler(ILogger<GetModifiedDocumentsCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;

            this.InitializeActions();
        }

        public async Task<GetModifiedDocumentsCommandResponse> Handle(GetModifiedDocumentsCommand request, CancellationToken cancellationToken)
        {

            this._logger.LogInformation(LogMessages.InfoMsgCompleted);

            GetModifiedDocumentsCommandResponse response = new GetModifiedDocumentsCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                var security = request.security;
                bool secBool = true;
                if (!string.IsNullOrWhiteSpace(security) && security.ToUpper() == "ALL_DOCS_NO_SECURITY")
                {
                    secBool = false;
                }
                var docs = await this.GetDocModifiedByDates(request.dateFrom,request.dateTo,request.modifiedOnly,request.allEvents,secBool, infoUtente);
                #endregion

                // BUILD RESPONSE
                response.Documents = docs.ToArray();
                response.Code = SearchDocumentsResponseCode.OK;
                response.TotalDocumentsNumber = docs.Count;

                this._logger.LogInformation(LogMessages.InfoMsgCompleted);

                return response;
            }
            catch (RestException pisEx)
            {
                _logger.LogError(LogMessages.RestExcMsg, pisEx.ErrorCode, pisEx.Description);
                response = new GetModifiedDocumentsCommandResponse();
                response.Code = SearchDocumentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, LogMessages.GenericExcMsg);
                response = new GetModifiedDocumentsCommandResponse();
                response.Code = SearchDocumentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;

        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetModifiedDocumentsCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly List<string> _actions = new();
        protected enum ActionsEnum
        {
            ADD_VERSION, AGG_DOC_GRIGIO,
            AGG_PROT, ANN_PRED, ANNULLA_PROTO, ANNULL_PROTOCOLLO,
            CESTINA_DOC, CONSOLIDADOCUMENTO, CREATE_DOC_AND_ADD_IN_FASC, DEL_DOC, DOC_ADD_INFASC,
            DOC_ADD_INFOLDER, DOC_CAMBIO_STATO, DOC_CAMBIO_STATO_ADMIN, DOC_DEL_FROM_FOLDER,
            DOC_NEW_ALLEGATO, DOC_PROT, DOC_RIMUOVI_ALLEGATO, DOC_RIMUOVI_VERSIONE, DOC_SIGNATURE,
            DOC_SIGNATURE_P, DOCUMENTOCONVERSIONEPDF, DOCUMENTO_EXEC_ANNULLA_REPERTORIO,
            DOCUMENTO_REPERTORIATO, DOCUMENTOTIMESTAMP, DOC_VERIFIED, MODIFICADOCSTATOFINALE,
            MODIFIED_OBJECT_DOC, MODIFIED_OBJECT_PROTO, MOD_MITT_DEST, MOD_MITT_INT, PUT_FILE,
            RECORD_PREDISPOSED, SCAMBIA_DOC, DOC_STEP_OVER,
            INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE, INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE,
            CONCLUSIONE_PROCESSO_LF_DOCUMENTO, TRONCAMENTO_PROCESSO, AVVIATO_PROCESSO_DI_FIRMA_DOCUMENTO,
            AVVIATO_PROCESSO_DI_FIRMA_ALLEGATO, INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN
        };
        protected void InitializeActions()
        {
            foreach(var ac in Enum.GetNames(typeof(ActionsEnum)))
            {
                this._actions.Add(ac);
            }
        }



        private async Task<List<Document>> GetDocModifiedByDates(string dateFrom, string dateTo, bool soloModificati, 
                                                    bool allActions, bool security, DocsPaVO.utente.InfoUtente infoUt)
        {
            List<Document> output = new();

            var queryLog = this._pi3DbContext.LogEntities.AsNoTracking()
                .Where(l => "DOCUMENTO".Equals(l.VAR_OGGETTO) && 
                !"OPEN_DET_DOC".Equals(l.VAR_COD_AZIONE) && l.ID_AMM == infoUt.idAmministrazione.AsLong());

            var queryLogSto = this._pi3DbContext.LogStoricoEntities.AsNoTracking()
                .Where(l => "DOCUMENTO".Equals(l.VAR_OGGETTO) && 
                !"OPEN_DET_DOC".Equals(l.VAR_COD_AZIONE) && l.ID_AMM == infoUt.idAmministrazione.AsLong());

            var queryProfile = this._pi3DbContext.ProfileEntities.AsNoTracking();

            if (security)
            {
                queryProfile = queryProfile.Where(p => this._pi3DbContext.SecurityEntities.AsNoTracking()
                                .Where(s => s.THING == p.SYSTEM_ID
                                        && s.ACCESSRIGHTS > 0
                                        && (s.PERSONORGROUP == infoUt.idGruppo.AsLong()
                                        || s.PERSONORGROUP == infoUt.idPeople.AsLong()))
                                .Select(s => s.THING)
                                .Any());
            }
            #region Compute data range
            DateTime? fDataFrom = null, fDataTo = null;

            if (!string.IsNullOrWhiteSpace(dateFrom))
            {
                fDataFrom = this.GetDateCurrDay(dateFrom.Trim().AsDateTime());
            }
            else
            {
                fDataFrom = this.GetDateCurrDay(DateTime.Now);
            }

            if (!string.IsNullOrWhiteSpace(dateTo))
            {
                fDataTo = this.GetDateCurrDayEndOfDay(dateTo.Trim().AsDateTime());
            }
            else
            {
                fDataTo = this.GetDateCurrDayEndOfDay(DateTime.Now);
            }
            #endregion


            //ADDING DTA FILTERS
            queryLog = queryLog.Where(l => l.DTA_AZIONE >= fDataFrom && l.DTA_AZIONE <= fDataTo);
            queryLogSto = queryLogSto.Where(l => l.DTA_AZIONE >= fDataFrom && l.DTA_AZIONE <= fDataTo);

            if (soloModificati)
            {
                queryProfile = queryProfile.Where(l => l.CREATION_TIME >= fDataFrom && l.CREATION_TIME <= fDataTo);
            }

            //ADDING FILTER ACTIONS
            if (!allActions)
            {
                queryLog = queryLog.Where(l => this._actions.Contains(l.VAR_COD_AZIONE));
                queryLogSto = queryLogSto.Where(l => this._actions.Contains(l.VAR_COD_AZIONE));
            }

            var viableDocs = await queryLog.Select(a => a.ID_OGGETTO).Union(queryLogSto.Select(a => a.ID_OGGETTO)).ToListAsync();
            var docQuery = queryProfile.Where(p => viableDocs.Contains(p.SYSTEM_ID)).OrderBy(p => p.DOCNUMBER).Select(p => new
            {
                p.VAR_SEGNATURA,
                p.DOCNUMBER,
                p.VAR_PROF_OGGETTO
            });

            foreach (var d in docQuery)
            {
                output.Add(new()
                {
                    DocNumber = d.DOCNUMBER != null? d.DOCNUMBER.ToString() : string.Empty,
                    Signature = d.VAR_SEGNATURA,
                    Object = d.VAR_PROF_OGGETTO
                });
            }

            return output;

        }

        #region Date utils
        private DateTime GetDateCurrDay(DateTime dateTime)
        {
            var initDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
            return (initDate);
        }

        private DateTime GetDateCurrDayEndOfDay(DateTime dateTime)
        {
            var initDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
            return (initDate);
        }
        #endregion

        #endregion
    }

}