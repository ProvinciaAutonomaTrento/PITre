// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.RicercaLite;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetAllegatiRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetAllegati;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetAllegati
{
    public class DocumentoGetAllegatiHandler : IRequestHandler<DocumentoGetAllegatiRequest, DocumentoGetAllegatiResult>
    {
        #region Public Members

        public DocumentoGetAllegatiHandler(ILogger<DocumentoGetAllegatiHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DocumentoGetAllegatiResult> Handle(DocumentoGetAllegatiRequest request, CancellationToken cancellationToken)
        {
            List<Allegato> output = new List<Allegato>();

            try
            {
                long docnumber = request.docNumber.AsLong();

                var filterAllegati = request.filterAllegatiPec;
                if (request.filterAllegatiPec.Equals(request.simplifiedInteroperabilityId))
                    filterAllegati = "pitre";

                var allegatoQuery = this._dbContext
                    .ProfileEntities
                    .AsNoTracking()
                    .Where(p => (p.CHA_IN_CESTINO ?? "0") != "1" && p.ID_DOCUMENTO_PRINCIPALE == docnumber)
                    .Select(p => new ProfileEntity
                    {
                        DOCNUMBER = p.DOCNUMBER,
                        DOCSERVER_LOC = p.DOCSERVER_LOC,
                        VAR_PROF_OGGETTO = p.VAR_PROF_OGGETTO,
                        PATH = p.PATH,
                        FORWARDING_SOURCE = p.FORWARDING_SOURCE,
                        IN_LIBROFIRMA = p.IN_LIBROFIRMA,
                        CHA_TASK_STATUS = p.CHA_TASK_STATUS,
                        CHA_IN_CESTINO = p.CHA_IN_CESTINO,
                    });

                switch(request.filterAllegatiPec)
                {
                    case "pec":
                        allegatoQuery = allegatoQuery.Where(a => 
                        this._dbContext.NotificaEntities
                            .AsNoTracking()
                            .Any(n => n.DOCNUMBER == docnumber && a.VAR_PROF_OGGETTO.StartsWith("Ricevuta di ritorno delle Mail")));
                        break;
                    case "user":
                        allegatoQuery = allegatoQuery.Where(a => 
                            !this._dbContext.NotificaEntities
                            .AsNoTracking()
                            .Any(n => n.DOCNUMBER == docnumber && (a.VAR_PROF_OGGETTO.StartsWith("Ricevuta di ritorno delle Mail") 
                                || a.VAR_PROF_OGGETTO.StartsWith("Ricevuta di mancata consegna") || a.VAR_PROF_OGGETTO.StartsWith("Ricevuta di avvenuta"))
                                && !this._dbContext.VersionEntities
                                .AsNoTracking()
                                .Any(v => v.DOCNUMBER == a.DOCNUMBER &&( v.CHA_ALLEGATI_ESTERNO == "1" || v.CHA_ALLEGATI_ESTERNO == "D"))));
                        break;
                    case "pitre":
                        allegatoQuery = allegatoQuery.Where(a => 
                            this._dbContext.NotificaEntities
                                .AsNoTracking()
                                .Any(n => n.DOCNUMBER == docnumber && 
                                        (a.VAR_PROF_OGGETTO.StartsWith("Ricevuta di avvenuta") || a.VAR_PROF_OGGETTO.StartsWith("Ricevuta di mancata consegna"))));
                        break;
                    case "esterni":
                        allegatoQuery = allegatoQuery.Where(a => 
                                this._dbContext.VersionEntities
                                .AsNoTracking()
                                .Any(v => v.DOCNUMBER == a.DOCNUMBER && v.CHA_ALLEGATI_ESTERNO == "1"));
                        break;
                    case "derivati":
                        allegatoQuery = allegatoQuery.Where(a => 
                                this._dbContext.VersionEntities
                                    .AsNoTracking()
                                    .Any(v => v.DOCNUMBER == a.DOCNUMBER && v.CHA_ALLEGATI_ESTERNO == "D"));
                        break;
                    case "albopubb":
                        allegatoQuery = allegatoQuery.Where(a => 
                                this._dbContext.VersionEntities
                                    .AsNoTracking()
                                    .Any(v => v.DOCNUMBER == a.DOCNUMBER && v.CHA_ALLEGATI_ESTERNO == "0") 
                                    && this._dbContext.AlboDocPubbEntities
                                        .AsNoTracking()
                                        .Any(p => p.DOCNUMBER == a.DOCNUMBER && p.DA_PUBB == "S"));
                        break;
                }
                var allegatoEntities = await allegatoQuery.OrderBy(a => a.DOCNUMBER).ToListAsync();

                foreach (ProfileEntity allegatoEntity in allegatoEntities)
                {
                    var allegato = new DocsPaVO.documento.Allegato();
                    var index = output.Count + 1;
                    var lastVersionEntity = await this._dbContext.VersionEntities
                                    .AsNoTracking()
                                    .Where(v => v.DOCNUMBER == allegatoEntity.DOCNUMBER)
                                    .OrderByDescending(v => v.VERSION_ID).FirstAsync();

                    var lastComponentsEntity = await this._dbContext.ComponentEntities
                                        .AsNoTracking() 
                                        .Where(c => c.DOCNUMBER == allegatoEntity.DOCNUMBER)
                                        .OrderByDescending(c => c.VERSION_ID).FirstAsync();

                    allegato.version = lastVersionEntity.VERSION.ToString();
                    allegato.versionLabel = allegato.version == "0" ? lastVersionEntity.VERSION_LABEL : string.Format("A{0:0#}", index);
                    allegato.descrizione = allegato.version == "0" ? lastVersionEntity.COMMENTS : allegatoEntity.VAR_PROF_OGGETTO;
                    allegato.position = index;
                    allegato.autore = await this._dbContext.PeopleEntities
                                    .AsNoTracking()
                                    .Where(p => p.SYSTEM_ID == lastVersionEntity.AUTHOR)
                                    .Select(p => p.FULL_NAME)
                                    .FirstAsync();
                    
                    allegato.idPeopleDelegato = allegatoEntity.ID_PEOPLE_DELEGATO != null && allegatoEntity.ID_PEOPLE_DELEGATO  != 0 ? allegatoEntity.ID_PEOPLE_DELEGATO.ToString() : null;        
                    if(allegato.idPeopleDelegato != null)
                        allegato.autore = await this._dbContext.PeopleEntities
                                .AsNoTracking()
                                .Where(p => p.SYSTEM_ID == allegatoEntity.ID_PEOPLE_DELEGATO)
                                .Select(p => p.FULL_NAME)
                                .FirstAsync()
                            + Resources.SostitutoDi + allegato.autore;

                    allegato.versionId = lastVersionEntity.VERSION_ID.ToString();
                    allegato.docNumber = lastVersionEntity.DOCNUMBER.ToString();
                    allegato.numeroPagine = (int)lastVersionEntity.NUM_PAG_ALLEGATI.GetValueOrDefault();
                    allegato.dataInserimento = lastVersionEntity.DTA_CREAZIONE.AsDateTimeFormat();
                    allegato.docServerLoc = allegatoEntity.DOCSERVER_LOC;
                    allegato.fileName = lastComponentsEntity.VAR_NOMEORIGINALE != null ? lastComponentsEntity.VAR_NOMEORIGINALE : lastComponentsEntity.PATH ?? string.Empty;
                    allegato.fileSize = lastComponentsEntity.FILE_SIZE.GetValueOrDefault().ToString();
                    allegato.impronta = lastComponentsEntity.VAR_IMPRONTA;
                    allegato.idPeople = lastVersionEntity.AUTHOR.GetValueOrDefault().ToString();
                    allegato.path = lastComponentsEntity.PATH;
                    allegato.subVersion = lastVersionEntity.SUBVERSION;
                    allegato.cartaceo = lastVersionEntity.CARTACEO > 0;
                    allegato.firmato = lastComponentsEntity.CHA_FIRMATO;
                    allegato.tipoFirma = lastComponentsEntity.CHA_TIPO_FIRMA;
                    allegato.ForwardingSource = allegatoEntity.FORWARDING_SOURCE.GetValueOrDefault().ToString();

                    allegato.autoreFile = lastComponentsEntity.ID_PEOPLE_PUTFILE != null ? await this._dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == lastComponentsEntity.ID_PEOPLE_PUTFILE).Select(p => p.FULL_NAME).FirstAsync(): null;
                    if(lastComponentsEntity.ID_PEOPLE_DELEGATO_PUTFILE != null && lastComponentsEntity.ID_PEOPLE_DELEGATO_PUTFILE != 0)
                        allegato.autore = await this._dbContext.PeopleEntities
                            .AsNoTracking()
                            .Where(p => p.SYSTEM_ID == lastComponentsEntity.ID_PEOPLE_DELEGATO_PUTFILE)
                            .Select(p => p.FULL_NAME)
                            .FirstAsync()
                        + Resources.SostitutoDi + allegato.autoreFile;

                    allegato.dataAcquisizione = lastComponentsEntity.DTA_FILE_ACQUIRED.AsDateFormat();
                    allegato.inLibroFirma = allegatoEntity.IN_LIBROFIRMA == "1";

                    switch (lastVersionEntity.CHA_ALLEGATI_ESTERNO)
                    {
                        case "P":
                            allegato.TypeAttachment = 2;
                            break;
                        case "I":
                            allegato.TypeAttachment = 3;
                            break;
                        case "1":
                            allegato.TypeAttachment = 4;
                            break;
                        case "D":
                            allegato.TypeAttachment = 5;
                            break;
                        case "S":
                            allegato.TypeAttachment = 6; //Allegato per il file segnatura.xml
                            break;
                        default:
                            allegato.TypeAttachment = 1;
                            break;
                    }

                    output.Add(allegato);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new DocumentoGetAllegatiResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetAllegatiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
