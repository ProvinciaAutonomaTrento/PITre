// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.InstanceAccess.Metadata;
using DocsPaVO.Interoperabilita.Segnatura;
using DocsPaVO.Mobile;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetVersionsMainDocument
{
    public class GetVersionsMainDocumentHandler : IRequestHandler<Application.Requests.GetVersionsMainDocument, GetVersionsMainDocumentResult>
    {
        #region Public Members

        public GetVersionsMainDocumentHandler(ILogger<GetVersionsMainDocumentHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetVersionsMainDocumentResult> Handle(Application.Requests.GetVersionsMainDocument request, CancellationToken cancellationToken)
        {
            List<Documento> output = null;

            try
            {
                var docnumber = request.docNumber.AsLong();

                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
                
                var idCorrGlobaliPeople = 
                    await this._dbContext.CorrGlobaliEntities
                        .AsNoTracking()
                        .Where(c => c.ID_PEOPLE == idPeople)
                        .Select(c => c.SYSTEM_ID)
                        .FirstAsync();
                
                var idCorrGlobaliGruppo = request.infoUser.idCorrGlobali.AsLong();

                var profile = await this._dbContext.ProfileEntities
                    .Join(
                        this._dbContext.DocumentTypesEntities,
                        profile => profile.DOCUMENTTYPE,
                        documentTypes => documentTypes.SYSTEM_ID,
                        (profile, documentType) => new
                        {
                            profile.SYSTEM_ID,
                            profile.DOCNUMBER,
                            profile.DOCSERVER_LOC,
                            profile.PATH,
                            profile.IN_LIBROFIRMA,
                            documentType.TYPE_ID,
                            documentType.DESCRIPTION
                        }
                    ).FirstOrDefaultAsync(p => p.DOCNUMBER == docnumber);

                var versions = await this._dbContext.ComponentEntities
                    .Join(
                        this._dbContext.VersionEntities,
                        components => components.VERSION_ID,
                        versions => versions.VERSION_ID,
                        (components, versions) => new
                        {
                            versions.VERSION_ID,
                            versions.DOCNUMBER,
                            versions.VERSION,
                            versions.SUBVERSION,
                            versions.VERSION_LABEL,
                            versions.AUTHOR,
                            versions.COMMENTS,
                            versions.NUM_PAG_ALLEGATI,
                            versions.CHA_DA_INVIARE,
                            versions.CHA_SEGNATURA,
                            versions.DTA_CREAZIONE,
                            versions.DTA_ARRIVO,
                            versions.CARTACEO,
                            versions.ID_PEOPLE_DELEGATO,
                            components.ID_PEOPLE_PUTFILE,
                            components.DTA_FILE_ACQUIRED,
                            components.ID_PEOPLE_DELEGATO_PUTFILE,
                            components.PATH,
                            components.VAR_IMPRONTA,
                            components.FILE_SIZE,
                            components.CHA_FIRMATO,
                            components.CHA_TIPO_FIRMA,
                            components.VAR_NOMEORIGINALE
                        }
                     )
                    .Join(
                        this._dbContext.PeopleEntities,
                        versions => versions.AUTHOR,
                        people => people.SYSTEM_ID,
                        (versions, people) => new { people.FULL_NAME, versions }
                    )
                    .Where(v => v.versions.DOCNUMBER == docnumber && v.versions.VERSION > 0)
                    .ToListAsync();

                var hide_doc_versions = await this._dbContext.TrasmissioneEntities
                    .Join(
                        this._dbContext.TrasmSingolaEntities,
                        trasmissione => trasmissione.SYSTEM_ID,
                        trasmSingola => trasmSingola.ID_TRASMISSIONE,
                        (trasmissione, trasmSingola) => new { trasmissione.ID_PROFILE, trasmSingola.HIDE_DOC_VERSIONS, trasmSingola.ID_CORR_GLOBALE }
                    )
                    .FirstOrDefaultAsync(t => t.ID_PROFILE == docnumber && (t.ID_CORR_GLOBALE == idCorrGlobaliGruppo || t.ID_CORR_GLOBALE == idCorrGlobaliPeople));

                if (hide_doc_versions != null && hide_doc_versions.HIDE_DOC_VERSIONS == "1")
                {
                    var security = await this._dbContext.SecurityEntities
                        .FirstOrDefaultAsync(s => s.THING == docnumber && (s.PERSONORGROUP == idGruppo || s.PERSONORGROUP == idPeople) && (s.CHA_TIPO_DIRITTO == "P") || s.CHA_TIPO_DIRITTO == "A");

                    if(security == null)
                    {
                        var max_version = versions.Max(v => v.versions.VERSION_ID);
                        versions = versions.Where(v => v.versions.VERSION_ID == max_version).ToList();
                    }
                }

                output = new List<Documento>();
                foreach (var version in versions.OrderByDescending(v => v.versions.VERSION_ID))
                {
                    var documento = new DocsPaVO.documento.Documento();

                    documento.docNumber = version.versions.DOCNUMBER.ToString();
                    documento.versionId = version.versions.VERSION_ID.ToString();
                    documento.version = version.versions.VERSION.ToString();
                    documento.subVersion = version.versions.SUBVERSION;
                    documento.versionLabel = version.versions.VERSION_LABEL;
                    documento.idPeople = version.versions.AUTHOR.ToString();
                    documento.descrizione = version.versions.COMMENTS;
                    documento.fileName = version.versions.VAR_NOMEORIGINALE != null ? version.versions.VAR_NOMEORIGINALE : (version.versions.PATH ?? string.Empty);
                    documento.fileSize = version.versions.FILE_SIZE.ToString();
                    documento.autore = version.FULL_NAME;
                    documento.idPeopleDelegato = "0";
                    if (version.versions.ID_PEOPLE_DELEGATO != null && version.versions.ID_PEOPLE_DELEGATO != 0)
                    {
                        documento.idPeopleDelegato = version.versions.ID_PEOPLE_DELEGATO.ToString();
                        documento.autore = this._dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == version.versions.ID_PEOPLE_DELEGATO).First().FULL_NAME + Resources.SostitutoDi + version.FULL_NAME;
                    }

                    if (version.versions.ID_PEOPLE_PUTFILE != null && version.versions.ID_PEOPLE_PUTFILE > 0)
                        documento.autoreFile = this._dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == version.versions.ID_PEOPLE_PUTFILE).First().FULL_NAME;

                    if (version.versions.ID_PEOPLE_DELEGATO_PUTFILE != null && version.versions.ID_PEOPLE_DELEGATO_PUTFILE > 0)
                        documento.autoreFile = this._dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == version.versions.ID_PEOPLE_DELEGATO_PUTFILE).First().FULL_NAME + Resources.SostitutoDi + documento.autoreFile;

                    documento.dataAcquisizione = version.versions.DTA_FILE_ACQUIRED != null ? version.versions.DTA_FILE_ACQUIRED.AsDateTimeFormat() : null;
                    documento.docServerLoc = profile.DOCSERVER_LOC;
                    documento.path = version.versions.PATH;
                    documento.dataInserimento = version.versions.DTA_CREAZIONE.AsDateTimeFormat();
                    documento.dataArrivo = version.versions.DTA_ARRIVO.AsDateTimeFormat();
                    documento.daInviare = version.versions.CHA_DA_INVIARE;
                    documento.firmato = version.versions.CHA_FIRMATO;
                    documento.tipoFirma = version.versions.CHA_TIPO_FIRMA;
                    documento.tipologia = new TipologiaCanale { codice = profile.TYPE_ID, descrizione = profile.DESCRIPTION };
                    documento.cartaceo = version.versions.CARTACEO > 0;
                    documento.impronta = version.versions.VAR_IMPRONTA != null ? version.versions.VAR_IMPRONTA : string.Empty;
                    documento.inLibroFirma = profile.IN_LIBROFIRMA == "1";
                    documento.conSegnaturaPermanente = version.versions.CHA_SEGNATURA == "1";

                    output.Add(documento);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new GetVersionsMainDocumentResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetVersionsMainDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
