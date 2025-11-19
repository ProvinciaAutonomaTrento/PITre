// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
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

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.InteroperabilitaIsDocPecPendente
{

    // Richiede libreria MediatR
    public class InteroperabilitaIsDocPecPendenteHandler : IRequestHandler<Application.Requests.InteroperabilitaIsDocPecPendente, InteroperabilitaIsDocPecPendenteResult>
    {
        #region Public Members

        public InteroperabilitaIsDocPecPendenteHandler(ILogger<InteroperabilitaIsDocPecPendenteHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<InteroperabilitaIsDocPecPendenteResult> Handle(Application.Requests.InteroperabilitaIsDocPecPendente request, CancellationToken cancellationToken)
        {
            bool retval = false;

            try
            {
                var assDocMailInterop = _dbContext.AssDocMailInteropEntities;
                var mailRegistri = _dbContext.MailRegistriEntities;

                long idDocument = Convert.ToInt64(request.idDocument);

                var res = assDocMailInterop
                    .Where(x => x.ID_PROFILE == idDocument)
                    .Join(
                        mailRegistri,
                        assDocMailInterop => new { assDocMailInterop.ID_REGISTRO, assDocMailInterop.VAR_EMAIL_REGISTRO },
                        mailRegistri => new { mailRegistri.ID_REGISTRO, mailRegistri.VAR_EMAIL_REGISTRO },
                        (a, b) => new
                        {
                            SoloMailPec = b.VAR_SOLO_MAIL_PEC,
                            MailRicPendente = b.VAR_MAIL_RIC_PENDENTE
                        })
                    .ToList()
                    .FirstOrDefault(); //rows[0]

                retval = res != null && !string.IsNullOrEmpty(res.SoloMailPec) && !res.SoloMailPec.Equals("1") && !string.IsNullOrEmpty(res.MailRicPendente) && res.MailRicPendente.Equals("1");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new InteroperabilitaIsDocPecPendenteResult(retval);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InteroperabilitaIsDocPecPendenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
