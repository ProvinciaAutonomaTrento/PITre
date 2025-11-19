// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetCorrespondentDetailsRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetCorrespondentDetails;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetCorrespondentDetails
{
    public class GetCorrespondentDetailsHandler : IRequestHandler<GetCorrespondentDetailsRequest, GetCorrespondentDetailsResult>
    {
        protected readonly ILogger<GetCorrespondentDetailsHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        public GetCorrespondentDetailsHandler(
            ILogger<GetCorrespondentDetailsHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }


        public async Task<GetCorrespondentDetailsResult> Handle(GetCorrespondentDetailsRequest request , CancellationToken cancellationToken)
        {
            DocsPaVO.addressbook.CorrespondentDetails output = new DocsPaVO.addressbook.CorrespondentDetails();

            try
            {
                var corrDetEnt = await this._dbContext.DettGlobaliEntities.AsNoTracking().
                    Where(corrDet => corrDet.ID_CORR_GLOBALI == request.idCorr.AsLong())
                    .Select(corrDet => new
                    {
                        corrDet.SYSTEM_ID,
                        corrDet.ID_CORR_GLOBALI,
                        corrDet.VAR_INDIRIZZO,
                        corrDet.VAR_CAP,
                        corrDet.VAR_PROVINCIA,
                        corrDet.VAR_NAZIONE,
                        corrDet.VAR_TELEFONO,
                        corrDet.VAR_TELEFONO2,
                        corrDet.VAR_FAX,
                        corrDet.VAR_NOTE,
                        corrDet.VAR_COD_FISC,
                        corrDet.VAR_CITTA,
                        corrDet.VAR_LOCALITA,
                        corrDet.VAR_LUOGO_NASCITA,
                        corrDet.DTA_NASCITA,
                        corrDet.VAR_TITOLO,
                        corrDet.VAR_COD_PI,
                        corrDet.VAR_COD_IPA,
                        corrDet.VAR_COD_FISCALE
                    }).FirstOrDefaultAsync();

                if(corrDetEnt != null)
                {
                    output.SystemId = corrDetEnt.SYSTEM_ID.ToString();
                    output.IdCorr = corrDetEnt.ID_CORR_GLOBALI != null ? corrDetEnt.ID_CORR_GLOBALI.ToString() : string.Empty;
                    output.Address = corrDetEnt.VAR_INDIRIZZO ?? string.Empty;
                    output.ZipCode = corrDetEnt.VAR_CAP ?? string.Empty;
                    output.District = corrDetEnt.VAR_PROVINCIA ?? string.Empty;
                    output.Country = corrDetEnt.VAR_NAZIONE ?? string.Empty;
                    output.Phone = corrDetEnt.VAR_TELEFONO ?? string.Empty;
                    output.Phone2 = corrDetEnt.VAR_TELEFONO2 ?? string.Empty;
                    output.Fax = corrDetEnt.VAR_FAX ?? string.Empty;
                    output.Note = corrDetEnt.VAR_NOTE ?? string.Empty;
                    output.TaxId = corrDetEnt.VAR_COD_FISC != null ? ((string.IsNullOrEmpty(corrDetEnt.VAR_COD_FISC.Trim()) ? corrDetEnt.VAR_COD_FISCALE.Trim() : corrDetEnt.VAR_COD_FISC) ) : string.Empty;
                    output.City = corrDetEnt.VAR_CITTA ?? string.Empty;
                    output.Place = corrDetEnt.VAR_LOCALITA ?? string.Empty;
                    output.BirthPlace = corrDetEnt.VAR_LUOGO_NASCITA ?? string.Empty;
                    output.BirthDay = !string.IsNullOrEmpty(corrDetEnt.DTA_NASCITA) ? corrDetEnt.DTA_NASCITA : string.Empty;
                    output.Title = corrDetEnt.VAR_TITOLO ?? string.Empty;
                    output.CommercialId = corrDetEnt.VAR_COD_PI ?? string.Empty;
                }

            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }


            return new GetCorrespondentDetailsResult(output);
        }

    }
}
