// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.IsDocumentoInLibroFirmaConCambioSato
{

    public class IsDocumentoInLibroFirmaConCambioSatoHandler : IRequestHandler<Application.Requests.IsDocumentoInLibroFirmaConCambioSato, IsDocumentoInLibroFirmaConCambioSatoResult>
    {
        #region Public Members

        public IsDocumentoInLibroFirmaConCambioSatoHandler(ILogger<IsDocumentoInLibroFirmaConCambioSatoHandler> logger, IPi3DbContext dbContext, IMediator mediator,
            IClaimsPrincipalService claimsPrincipalService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mediator = mediator;
            _claimsPrincipalService = claimsPrincipalService;
        }

        public async Task<IsDocumentoInLibroFirmaConCambioSatoResult> Handle(Application.Requests.IsDocumentoInLibroFirmaConCambioSato request, CancellationToken cancellationToken)
        {
            long idDoc = long.Parse(request.idDocumento);

            return new IsDocumentoInLibroFirmaConCambioSatoResult(await checkIfExist(_dbContext.IstanzaProcessoFirmaEntities.Where(p => p.ID_DOCUMENTO == idDoc && p.STATO.Equals("IN_EXEC")).Select(p => p.ID_ISTANZA).ToList()));
        }

        private async Task<bool> checkIfExist(List<long> id_istanza)
        {
            foreach (long id in id_istanza)
            {
                if (_dbContext.IstanzaPassoFirmaEntities.Where(p => p.ID_ISTANZA_PROCESSO == id && p.TIPO_FIRMA.Equals("DOC_CAMBIO_STATO")).Select(p => p.ID_ISTANZA_PASSO).Count() > 0)
                {
                    return true;
                }
            }       

            return false;
        }

        #endregion

        #region Private Members
        protected ILogger<IsDocumentoInLibroFirmaConCambioSatoHandler> _logger;
        protected IPi3DbContext _dbContext;
        protected IMediator _mediator;
        protected IClaimsPrincipalService _claimsPrincipalService;
        #endregion
    }

}
