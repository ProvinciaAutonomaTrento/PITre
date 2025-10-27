// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.CheckInOut;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetCheckOutDocumentStatusRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetCheckOutDocumentStatus;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetCheckOutDocumentStatus
{

    public class GetCheckOutDocumentStatusHandler : IRequestHandler<GetCheckOutDocumentStatusRequest, GetCheckOutDocumentStatusResult>
    {
        #region Public Members

        public GetCheckOutDocumentStatusHandler(ILogger<GetCheckOutDocumentStatusHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<GetCheckOutDocumentStatusResult> Handle(GetCheckOutDocumentStatusRequest request, CancellationToken cancellationToken)
        {
            CheckOutStatus output = null;

            try
            {
                if (string.IsNullOrEmpty(request.idDocument))
                    return new GetCheckOutDocumentStatusResult(null);

                long idProfile = request.idDocument.AsLong();

                var checkin_checkout_entity = await _dbContext.CheckinCheckoutEntities
                    .Join(
                        _dbContext.PeopleEntities,
                        checkinCheckout => checkinCheckout.ID_USER,
                        people => people.SYSTEM_ID,
                        (checkinCheckout, people) => new
                        {
                            checkinCheckout,
                            USER_NAME = people.USER_ID
                        }
                     )
                    .Join(
                        _dbContext.CorrGlobaliEntities,
                        checkinCheckout_people => checkinCheckout_people.checkinCheckout.ID_ROLE,
                        corrGlobali => corrGlobali.SYSTEM_ID,
                        (checkinCheckout_people, corrGlobali) => new
                        {
                            checkinCheckout_people.checkinCheckout,
                            checkinCheckout_people.USER_NAME,
                            ROLE_NAME = corrGlobali.VAR_DESC_CORR
                        }
                    )
                    .Join(
                        _dbContext.ProfileEntities,
                        checkinCheckout_people_corr => checkinCheckout_people_corr.checkinCheckout.ID_DOCUMENT,
                        profile => profile.SYSTEM_ID,
                        (checkinCheckout_people_corr, profile) => new
                        {
                            checkinCheckout_people_corr.checkinCheckout.SYSTEM_ID,
                            checkinCheckout_people_corr.checkinCheckout.ID_DOCUMENT,
                            checkinCheckout_people_corr.checkinCheckout.DOCUMENT_NUMBER,
                            checkinCheckout_people_corr.checkinCheckout.ID_USER,
                            checkinCheckout_people_corr.checkinCheckout.ID_ROLE,
                            checkinCheckout_people_corr.checkinCheckout.CHECK_OUT_DATE,
                            checkinCheckout_people_corr.checkinCheckout.DOCUMENT_LOCATION,
                            checkinCheckout_people_corr.checkinCheckout.MACHINE_NAME,
                            checkinCheckout_people_corr.USER_NAME,
                            checkinCheckout_people_corr.ROLE_NAME,
                            profile.VAR_SEGNATURA,
                            profile.ID_DOCUMENTO_PRINCIPALE
                        }
                    )
                    .Where(checkinCheckout_people_corr_profile => checkinCheckout_people_corr_profile.ID_DOCUMENT == idProfile).FirstOrDefaultAsync();

                if (checkin_checkout_entity != null)
                {
                    output = new CheckOutStatus
                    {
                        ID = checkin_checkout_entity.SYSTEM_ID.ToString(),
                        IDDocument = checkin_checkout_entity.ID_DOCUMENT.ToString(),
                        DocumentNumber = checkin_checkout_entity.DOCUMENT_NUMBER.ToString(),
                        Segnature = checkin_checkout_entity.VAR_SEGNATURA,
                        IDUser = checkin_checkout_entity.ID_USER.ToString(),
                        UserName = checkin_checkout_entity.USER_NAME,
                        IDRole = checkin_checkout_entity.ID_ROLE.ToString(),
                        RoleName = checkin_checkout_entity.ROLE_NAME,
                        CheckOutDate = checkin_checkout_entity.CHECK_OUT_DATE,
                        DocumentLocation = checkin_checkout_entity.DOCUMENT_LOCATION ?? string.Empty,
                        MachineName = checkin_checkout_entity.MACHINE_NAME,
                        IsAllegato = checkin_checkout_entity.ID_DOCUMENTO_PRINCIPALE != null && checkin_checkout_entity.ID_DOCUMENTO_PRINCIPALE != 0 ? true : false
                    };

                    output.InConversionePdf = await _dbContext.ConvPdfServerEntities.Where(x => x.ID_PROFILE == idProfile).AnyAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new GetCheckOutDocumentStatusResult(output);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetCheckOutDocumentStatusHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
