// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateMezzoSpedizioneDocumento
{

    // Richiede libreria MediatR
    public class UpdateMezzoSpedizioneDocumentoHandler : IRequestHandler<Application.Requests.updateMezzoSpedizioneDocumento, updateMezzoSpedizioneDocumentoResult>
    {
        #region Public Members

        public UpdateMezzoSpedizioneDocumentoHandler(ILogger<UpdateMezzoSpedizioneDocumentoHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, IPi3DbContext dbContext, IDistributedCache distributedCache, IConfiguration configuration, IEmailSenderService emailSenderService)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
            _distributedCache = distributedCache;
            _configuration = configuration;
            _emailSenderService = emailSenderService;

        }

        public async Task<updateMezzoSpedizioneDocumentoResult> Handle(updateMezzoSpedizioneDocumento request, CancellationToken cancellationToken)
        {
            long idAmm = request.info.idAmministrazione.AsLong();
            long idProfile = request.idProfile.AsLong();
            long idDocumentType = request.idDocumentTypes.AsLong();

            if(_dbContext.CollMSpedizDocumentoEntities.Where(w=>w.IDAMM==idAmm && w.ID_PROFILE==idProfile).Any())
            {
                var mSped = _dbContext.CollMSpedizDocumentoEntities.SingleOrDefault(u => u.IDAMM == idAmm && u.ID_PROFILE == idProfile);

                if (mSped != null)
                {
                    mSped.ID_DOCUMENTTYPES = idDocumentType;
                    ((Pi3DbContext)_dbContext).SaveChanges();
                }
            }
            else
            {

                _dbContext.CollMSpedizDocumentoEntities.Add(new CollMSpedizDocumentoEntity { IDAMM = idAmm, ID_RUOLO = null, ID_UTENTE = null, ID_DOCUMENTTYPES = idDocumentType, ID_PROFILE = idProfile });
                ((Pi3DbContext)_dbContext).SaveChanges();
            }

            return new updateMezzoSpedizioneDocumentoResult(true);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UpdateMezzoSpedizioneDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IConfiguration _configuration;
        protected readonly IEmailSenderService _emailSenderService;


        #endregion
    }

}
