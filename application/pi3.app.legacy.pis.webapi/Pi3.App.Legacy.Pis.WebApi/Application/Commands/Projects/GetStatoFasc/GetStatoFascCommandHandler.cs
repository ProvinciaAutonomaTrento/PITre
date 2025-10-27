// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetStatoFasc
{
    public class GetStatoFascCommandHandler : IRequestHandler<GetStatoFascCommand, GetStatoFascCommandResponse>
    {
        public GetStatoFascCommandHandler(ILogger<GetStatoFascCommandHandler> logger,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }

        public async Task<GetStatoFascCommandResponse> Handle(GetStatoFascCommand request, CancellationToken cancellationToken)
        {
            DocsPaVO.DiagrammaStato.Stato stato = null;

            try
            {
                var idProject = request.IdProject.AsLong();
                var diagramma = await this._dbContext.DiagrammiEntities.AsNoTracking().Where(d => d.ID_PROJECT == idProject).FirstOrDefaultAsync();

                if (diagramma != null)
                {
                    stato = new DocsPaVO.DiagrammaStato.Stato();
                    stato.SYSTEM_ID = Convert.ToInt32(diagramma.ID_STATO);
                    stato.ID_DIAGRAMMA = Convert.ToInt32(diagramma.ID_DIAGRAMMA);

                    var stEnt = await this._dbContext.StatoEntities.AsNoTracking().FirstOrDefaultAsync(d => d.SYSTEM_ID == diagramma.ID_STATO);
                    
                    if(stEnt != null)
                    {
                        stato.DESCRIZIONE = stEnt.VAR_DESCRIZIONE;
                        stato.STATO_INIZIALE = "1".Equals(stEnt.STATO_INIZIALE);
                        stato.STATO_FINALE = "1".Equals(stEnt.STATO_FINALE);
                        stato.CONVERSIONE_PDF = "1".Equals(stEnt.CONV_PDF);
                        stato.NON_RICERCABILE = "1".Equals(stEnt.NON_RICERCABILE);
                    }
                }
            }
            catch (Exception ex)
            {
                stato = null;
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new()
            {
                Output = stato
            };
        }

        #region Private Members

        protected readonly ILogger<GetStatoFascCommandHandler> _logger;
        protected readonly IPi3DbContext _dbContext;


        #endregion
    }


}
