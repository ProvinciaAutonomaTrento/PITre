// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.YearFilter
{
    public class YearFilterQueryHandler: IRequestHandler<YearFilterQuery, YearFilterQueryResponse>
    {
        private readonly ILogger<YearFilterQueryHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IPi3DbContext _context;

        public YearFilterQueryHandler(
            ILogger<YearFilterQueryHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IPi3DbContext context
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._context = context;
        }

        public async Task<YearFilterQueryResponse> Handle(YearFilterQuery request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);
            var idUtente = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);

            var idRegistro = await _context.RegistroEntities.SystemIdDaCodiceRegistro(request.CodiceRegistro);
			if (idRegistro == 0)
				throw new InvalidDataException();
            //var idRegistro = 86107;
            if (request.Paginazione == null) {
				request.Paginazione = new Pagination
				{
					Prendi = 100,
					Salta = 0
				};
            }

			// lista
			var projectsQry = _context.UserAggregates(idTenant, Convert.ToInt32(idRegistro), Convert.ToInt32(idUtente), Convert.ToInt32(idGruppo));
            var countQry = _context.UserAggregates(idTenant, Convert.ToInt32(idRegistro), Convert.ToInt32(idUtente), Convert.ToInt32(idGruppo));

			if (request.Anno != null)
			{
				projectsQry = projectsQry.Where(prj => prj.DataApertura.Value.Year == request.Anno);
				//countQry = countQry.Where(prj => prj.DTA_APERTURA.Value.Year == request.Anno);
			}
            return GetResponse(projectsQry, countQry);

            YearFilterQueryResponse GetResponse(IQueryable<AggregateResult>query, IQueryable<AggregateResult> count) {
                var projects = query.Skip(request.Paginazione.Salta).Take(request.Paginazione.Prendi+1).ToList();
                //var totalCount = count.Count();
                var filtrati = projects.GetRange(0, request.Paginazione.Prendi);

                return new YearFilterQueryResponse()
                {
                    Aggregati = filtrati,
                    //RecordTotali = totalCount,
                    //NumeroRecord = request.Paginazione.Prendi + 1 projects.Count,
                    PiuDati = projects.Count() == request.Paginazione.Prendi + 1
                };
            }
        }
    }
}
