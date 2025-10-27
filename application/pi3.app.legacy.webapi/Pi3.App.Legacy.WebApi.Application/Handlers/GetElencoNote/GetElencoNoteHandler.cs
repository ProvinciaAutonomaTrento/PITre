// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProfilazioneDinamicaLite;
using DocumentFormat.OpenXml.Drawing.Charts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetListaNote;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetListaPolicy;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetElencoNote
{
    internal class GetElencoNoteHandler : IRequestHandler<Application.Requests.GetElencoNote, GetElencoNoteResult>
    {
        #region Public Members

        public GetElencoNoteHandler(
            ILogger<GetElencoNoteHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
             IPi3DbContext dbContext
            //IMediator mediator
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._dbContext = dbContext;
            //this._mediator = mediator;
        }



        public async Task<GetElencoNoteResult> Handle(Requests.GetElencoNote request, CancellationToken cancellationToken)
        {
            List<string> result = new List<string>();
            string idRegRf = request.contextKey;
            string descNota = request.prefixText.ToUpper();

            try
            {
                var queryable = this._dbContext.ElencoNoteEntities.AsNoTracking();
                if (!idRegRf.Equals("TUTTE"))
                {
                    var idRegRFAsLong = idRegRf.AsLong();
                    queryable = queryable.Where(n => (n.ID_REG_RF == idRegRFAsLong || n.ID_REG_RF == 0)
                        && EF.Functions.Like(n.VAR_DESC_NOTA.ToUpper(), $"%{descNota}%"));
                }

                var elencoNoteEntities = await queryable
                           .OrderBy(e => e.ID_REG_RF)
                           .ThenBy(x => x.VAR_DESC_NOTA)
                           .ToListAsync();

                if (elencoNoteEntities != null && elencoNoteEntities.Any())
                {
                    elencoNoteEntities.ForEach(x =>
                    {
                        result.Add(x.VAR_DESC_NOTA + " [" + x.COD_REG_RF + "]");
                    });
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new GetElencoNoteResult(result.ToArray());
        }

        #endregion

        #region Private members

        protected readonly ILogger<GetElencoNoteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IPi3DbContext _dbContext;
        #endregion


    }
}
