// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetDocumentListVersionsLiteRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetDocumentListVersionsLite;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetDocumentListVersionsLite
{
    public class GetDocumentListVersionsLiteHandler : IRequestHandler<GetDocumentListVersionsLiteRequest, GetDocumentListVersionsLiteResult>
    {
        #region Public Members

        public GetDocumentListVersionsLiteHandler(
            ILogger<GetDocumentListVersionsLiteHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetDocumentListVersionsLiteResult> Handle(GetDocumentListVersionsLiteRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.Documento[] output = null!;

            try
            {
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

                var inLibroFirma = await this._pi3DbContext.ProfileEntities
                                .AsNoTracking()
                                .Where(p => p.SYSTEM_ID == request.docNumber.AsLong())
                                .Select(p => p.IN_LIBROFIRMA)
                                .FirstAsync();

                output = await (from v in this._pi3DbContext.VersionEntities.AsNoTracking()
                                join c in this._pi3DbContext.ComponentEntities.AsNoTracking()
                                 on v.VERSION_ID equals c.VERSION_ID
                                where v.DOCNUMBER == request.docNumber.AsLong()
                                && IPi3DbContextMappedFunctions.IsVersionVisible(v.VERSION_ID.Value, idUser, idGroup) > 0
                                && v.VERSION > 0
                                orderby v.VERSION_ID descending
                                select new DocsPaVO.documento.Documento()
                                {
                                    versionId = (v.VERSION_ID.HasValue ? v.VERSION_ID.ToString() : null),
                                    docNumber = (v.DOCNUMBER.HasValue ? v.DOCNUMBER.ToString() : null),
                                    version = (v.VERSION.HasValue ? v.VERSION.ToString() : null),
                                    subVersion = v.SUBVERSION,
                                    versionLabel = v.VERSION_LABEL,
                                    descrizione = v.COMMENTS,
                                    idPeople = (c.ID_PEOPLE_PUTFILE.HasValue ? c.ID_PEOPLE_PUTFILE.ToString() : null),
                                    idPeopleDelegato = (c.ID_PEOPLE_DELEGATO_PUTFILE.HasValue && c.ID_PEOPLE_DELEGATO_PUTFILE.Value != 0 ? c.ID_PEOPLE_DELEGATO_PUTFILE.ToString() : null),
                                    autore = (v.ID_PEOPLE_DELEGATO.HasValue && v.ID_PEOPLE_DELEGATO.Value != 0?
                                                    string.Format(Resources.SostitutoDi, IPi3DbContextMappedFunctions.GetPeopleName(v.ID_PEOPLE_DELEGATO.Value), IPi3DbContextMappedFunctions.GetPeopleName(v.AUTHOR.Value)) :
                                                    IPi3DbContextMappedFunctions.GetPeopleName(v.AUTHOR.Value)),
                                    autoreFile = (c.ID_PEOPLE_DELEGATO_PUTFILE.HasValue && c.ID_PEOPLE_DELEGATO_PUTFILE.Value != 0?
                                                    string.Format(Resources.SostitutoDi, IPi3DbContextMappedFunctions.GetPeopleName(c.ID_PEOPLE_DELEGATO_PUTFILE.Value), IPi3DbContextMappedFunctions.GetPeopleName(c.ID_PEOPLE_PUTFILE.Value)) :
                                                    (c.ID_PEOPLE_PUTFILE.HasValue ?
                                                        IPi3DbContextMappedFunctions.GetPeopleName(c.ID_PEOPLE_PUTFILE.Value) :
                                                        null)),
                                    dataAcquisizione = (c.DTA_FILE_ACQUIRED.HasValue ? c.DTA_FILE_ACQUIRED.AsDateTimeFormat() : null),
                                    dataInserimento = v.DTA_CREAZIONE.HasValue ? v.DTA_CREAZIONE.AsDateTimeFormat() : null,
                                    path = c.PATH,
                                    fileName = (!string.IsNullOrWhiteSpace(c.PATH) ? Path.GetFileName(c.PATH) : string.Empty),
                                    impronta = c.VAR_IMPRONTA,
                                    fileSize = (c.FILE_SIZE.HasValue ? c.FILE_SIZE.ToString() : null),
                                    daInviare = v.CHA_DA_INVIARE,
                                    conSegnaturaPermanente = (!string.IsNullOrWhiteSpace(v.CHA_SEGNATURA) ? v.CHA_SEGNATURA == "1" : false),
                                    dataArchiviazione = (c.DTA_FILE_ACQUIRED.HasValue ? c.DTA_FILE_ACQUIRED.AsDateTimeFormat() : null),
                                    dataArrivo = (v.DTA_ARRIVO.HasValue ? v.DTA_ARRIVO.AsDateTimeFormat() : null),
                                    cartaceo = (v.CARTACEO.HasValue ? v.CARTACEO > 0 : false),
                                    firmato = c.CHA_FIRMATO,
                                    tipoFirma = c.CHA_TIPO_FIRMA,
                                    inLibroFirma = (!string.IsNullOrWhiteSpace(inLibroFirma) ? inLibroFirma == "1" : false)
                                })
                                .ToArrayAsync();
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new GetDocumentListVersionsLiteResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentListVersionsLiteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }
}