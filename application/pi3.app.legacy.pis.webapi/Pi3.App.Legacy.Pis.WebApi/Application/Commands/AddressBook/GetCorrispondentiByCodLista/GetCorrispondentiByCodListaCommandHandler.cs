// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteBySystemId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteCompletoBySystemId;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections.Generic;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondentiByCodLista
{
    public class GetCorrispondentiByCodListaCommandHandler : IRequestHandler<GetCorrispondentiByCodListaCommand, GetCorrispondentiByCodListaCommandResponse>
    {
        public GetCorrispondentiByCodListaCommandHandler(
            ILogger<GetCorrispondentiByCodListaCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext
            )
        {
            this._pi3DbContext = pi3DbContext;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;
            this._logger = logger;
        }


        public async Task<GetCorrispondentiByCodListaCommandResponse> Handle(GetCorrispondentiByCodListaCommand request, CancellationToken cancellationToken)
        {
            GetCorrispondentiByCodListaCommandResponse output = new();

            var cors = await this.GetCorrispondentiByCodList(request.CodiceLista,request.InfoUtente.idAmministrazione,request.InfoUtente);
            output.Corrispondenti = cors;

            return output;
        }


        #region Private Members
        protected readonly ILogger<GetCorrispondentiByCodListaCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        protected async Task<List<DocsPaVO.utente.Corrispondente>> GetCorrispondentiByCodList(string codiceLista, string idAmm,InfoUtente infoUtente)
        {
            List<DocsPaVO.utente.Corrispondente> output = new();
            try
            {
                var queryList = (from a in this._pi3DbContext.ListeDistrEntities.AsNoTracking()
                 from b in this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                 from c in this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                 where b.VAR_COD_RUBRICA.ToUpper().Equals(codiceLista.ToUpper()) &&
                 b.SYSTEM_ID == a.ID_LISTA_DPA_CORR &&
                 c.SYSTEM_ID == a.ID_DPA_CORR &&
                 !c.DTA_FINE.HasValue &&
                 b.ID_AMM == idAmm.AsLong()
                 orderby c.VAR_DESC_CORR.Trim()
                 select new
                 {
                     a.ID_DPA_CORR,
                     b.ID_AMM
                 });

                foreach (var corr in queryList)
                {
                    if (corr.ID_DPA_CORR != null)
                    {
                        var corrResponse = await this._mediator.Send(new AddressbookGetCorrispondenteBySystemIdCommand()
                        {
                            SystemId = corr.ID_DPA_CORR.ToString(),
                        });
                        if(corrResponse != null)
                            output.Add(corrResponse.Output);
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex,Resources.ErrorGetCorrLista);
                output = new();
            }
            return output;
        }

        #endregion
    }
}
