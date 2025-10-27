// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetSchedaDocumentoInoltroMassivoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetSchedaDocumentoInoltroMassivo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetSchedaDocumentoInoltroMassivo
{
    public class GetSchedaDocumentoInoltroMassivoHandler : IRequestHandler<GetSchedaDocumentoInoltroMassivoRequest, GetSchedaDocumentoInoltroMassivoResult>
    {
        #region Public Members

        public GetSchedaDocumentoInoltroMassivoHandler(ILogger<GetSchedaDocumentoInoltroMassivoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            ISessionRepositoryService sessionRepositoryService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._sessionRepositoryService = sessionRepositoryService;
        }

        public async Task<GetSchedaDocumentoInoltroMassivoResult> Handle(GetSchedaDocumentoInoltroMassivoRequest request, CancellationToken cancellationToken)
        {
            var error = string.Empty;
            var haveError = false;
            StringBuilder errorInCreation = new StringBuilder(Resources.LogErrors);

            SchedaDocumento schedaDocumento = null;

            // L'allegato da aggiungere al documento
            Allegato attachment = null;
            List<Allegato> allegati = new List<Allegato>();
            // Descrizione da assegnare all'allegato
            var description = string.Empty;

            // Il file request
            FileRequest fileRequest = null;
            try
            {
                var userId = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
                var idPeople = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);

                schedaDocumento = (await this._mediator.Send(new Requests.NewSchedaDocumento(request.userInfo))).output;

                //La scheda deve essere un protocollo in uscita, predisposto e da protocollare
                schedaDocumento.tipoProto = "P";
                schedaDocumento.predisponiProtocollazione = true;
                schedaDocumento.protocollo = new ProtocolloUscita();
                schedaDocumento.protocollo.daProtocollare = "1";

                //Il mittente del documento � la UO cui appartiene l'utente che ha lanciato la procedura
                ((ProtocolloUscita)schedaDocumento.protocollo).mittente = request.userRole.uo;

                // Per ogni idProfile vengono recuperare le informazioni sul documento
                // e quindi viene creato un allegato per il file principale ed un allegato
                // per ogni allegato del documento
                var position = 1;
                foreach(var idProfile in request.idProfiles)
                {
                    List<BaseInfoDoc> baseInfoDocs = null;
                    try
                    {
                        var idProfileAslLong = idProfile.AsLong();

                        baseInfoDocs = await _dbContext.ProfileEntities.AsNoTracking()
                            .Join(_dbContext.VersionEntities,
                                p => p.DOCNUMBER,
                                v => v.DOCNUMBER,
                                (p, v) => new { p, v })
                            .Join(_dbContext.ComponentEntities,
                                j => j.v.VERSION_ID,
                                c => c.VERSION_ID,
                                (j, c) => new { j.p, j.v, c })
                            .Where(j => (j.p.DOCNUMBER == idProfileAslLong || j.p.ID_DOCUMENTO_PRINCIPALE == idProfileAslLong)
                                && j.v.VERSION_ID == (_dbContext.VersionEntities
                                    .Where(v => v.DOCNUMBER == j.p.DOCNUMBER && v.DOCNUMBER != null && v.CHA_SEGNATURA != "1")
                                    .OrderByDescending(v => v.VERSION_ID)
                                    .Select(v => v.VERSION_ID).First()))
                            .OrderBy(j => j.v.VERSION_ID)
                            .Select(j => new BaseInfoDoc()
                            {
                                ID_DOCUMENTO_PRINCIPALE = j.p.ID_DOCUMENTO_PRINCIPALE,
                                DOCNAME = j.p.DOCNAME,
                                VAR_PROF_OGGETTO = j.p.VAR_PROF_OGGETTO,
                                CHA_TIPO_PROTO = j.p.CHA_TIPO_PROTO,
                                DOCNUMBER = j.p.DOCNUMBER,
                                SYSTEM_ID = j.p.SYSTEM_ID,
                                FILE_SIZE = j.c.FILE_SIZE,
                                PATH = j.c.PATH,
                                VAR_NOMEORIGINALE = j.c.VAR_NOMEORIGINALE,
                                CHA_FIRMATO = j.c.CHA_FIRMATO,
                                VERSION_LABEL = j.v.VERSION_LABEL,
                                VERSION = j.v.VERSION,
                                VERSION_ID = j.v.VERSION_ID
                            })
                            .ToListAsync();
                    }
                    catch (Exception ex)
                    {
                        haveError = true;
                        errorInCreation.AppendFormat(Resources.LogErrorGetInfoDoc, idProfile);
                    }

                    if (baseInfoDocs != null)
                    {
                        // Informazioni di base sul documento pricipale
                        var baseInfoDocPrincipale = baseInfoDocs.Where(p => p.ID_DOCUMENTO_PRINCIPALE == null).FirstOrDefault();

                        // Nome con cui referenziare il documento principale di ogni documento da inoltrare
                        // Sar� impostato pari al system id del documento se questo � un grigio o alla segnatura
                        // in caso di protocollo
                        foreach (var infoDoc in baseInfoDocs)
                        {
                            description = infoDoc.ID_DOCUMENTO_PRINCIPALE == null ? string.Format(Resources.LogInoltroDocumentoPrincipale, infoDoc.DOCNAME) :
                                (baseInfoDocPrincipale != null) ? string.Format(Resources.LogInoltroAllegatoDelDocumento, infoDoc.VAR_PROF_OGGETTO, baseInfoDocPrincipale.DOCNAME) : string.Format(Resources.LogInoltroAllegato, infoDoc.VAR_PROF_OGGETTO);

                            //Creazione dell'allegato
                            attachment = new Allegato()
                            {
                                descrizione = description,
                                fileName = infoDoc.VAR_NOMEORIGINALE,
                                fileSize = infoDoc.FILE_SIZE.ToString(),
                                firmato = infoDoc.CHA_FIRMATO,
                                path = infoDoc.PATH,
                                subVersion = infoDoc.VERSION.ToString(),
                                version = "1",
                                versionId = infoDoc.VERSION_ID.ToString(),
                                versionLabel = string.Format("A{0:0#}", position),
                                numeroPagine = 0,
                                dataInserimento = string.Empty,
                                repositoryContext = schedaDocumento.repositoryContext,
                                ForwardingSource = baseInfoDocPrincipale != null ? baseInfoDocPrincipale.SYSTEM_ID.ToString() : string.Empty,
                                position = position,
                                TypeAttachment = 1
                            };

                            if (infoDoc.FILE_SIZE > 0)
                            {
                                // Costruzione dell'oggetto file request
                                fileRequest = new FileRequest()
                                {
                                    fileName = infoDoc.VAR_NOMEORIGINALE,
                                    path = infoDoc.PATH,
                                    version = infoDoc.VERSION.ToString(),
                                    versionId = infoDoc.VERSION_ID.ToString(),
                                    versionLabel = "" + position,
                                    docNumber = infoDoc.DOCNUMBER.ToString()
                                };

                                // Recupero del contenuto del file
                                try
                                {
                                    FileDocumento file = (await this._mediator.Send(new Application.Requests.GetFileDocument(fileRequest, request.userInfo))).output;
                                    await _sessionRepositoryService.SetFile(attachment.repositoryContext, attachment, file);
                                }
                                catch (Exception ex)
                                {
                                    haveError = true;
                                    errorInCreation.AppendFormat(Resources.LogErrorGetFile, infoDoc.DOCNUMBER);
                                }
                            }

                            allegati.Add(attachment);
                            position++;
                        }
                    }
                }

                schedaDocumento.allegati = allegati.ToArray();

                if (!haveError)
                    errorInCreation = new StringBuilder();

                errorInCreation.AppendLine(Resources.LogChiudi);

                error = errorInCreation.ToString();
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                error = Resources.LogErrorGetSchedaDocumentoInoltroMassivo;
            }

            return new GetSchedaDocumentoInoltroMassivoResult(schedaDocumento, error);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetSchedaDocumentoInoltroMassivoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly ISessionRepositoryService _sessionRepositoryService;

        protected class BaseInfoDoc
        {
            public long? DOCNUMBER { get; set;}
            public long? SYSTEM_ID { get; set; }
            public long? ID_DOCUMENTO_PRINCIPALE { get; set; }
            public string? DOCNAME { get; set; }
            public string? VAR_PROF_OGGETTO { get; set; }
            public string? CHA_TIPO_PROTO { get; set; }
            public long? FILE_SIZE { get; set; }
            public string? PATH { get; set; }
            public string? VAR_NOMEORIGINALE { get; set; }
            public string? CHA_FIRMATO { get; set; }
            public string? VERSION_LABEL { get; set; }
            public long? VERSION { get; set; }
            public long? VERSION_ID { get; set; }
        }
        #endregion
    }
}