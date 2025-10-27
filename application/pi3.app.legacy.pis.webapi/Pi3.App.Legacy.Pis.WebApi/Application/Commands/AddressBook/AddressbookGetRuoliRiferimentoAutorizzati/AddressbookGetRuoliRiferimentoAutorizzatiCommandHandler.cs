// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetRuoliRiferimentoAutorizzati
{
    public class AddressbookGetRuoliRiferimentoAutorizzatiCommandHandler : IRequestHandler<AddressbookGetRuoliRiferimentoAutorizzatiCommand, AddressbookGetRuoliRiferimentoAutorizzatiCommandResponse>
    {

        public AddressbookGetRuoliRiferimentoAutorizzatiCommandHandler(
            ILogger<AddressbookGetRuoliRiferimentoAutorizzatiCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }


        public async Task<AddressbookGetRuoliRiferimentoAutorizzatiCommandResponse> Handle(AddressbookGetRuoliRiferimentoAutorizzatiCommand request, CancellationToken cancellationToken)
        {
            Ruolo[] output = null;
            List<Ruolo> ruoli = new List<Ruolo>();
            var tipoDestinatario = request.Qca.ragione.tipoDestinatario.ToString();
            var idUO = request.Uo.systemId.AsLong();

            try
            {
                var listIdUO = new List<long>();
                var livelloUO = request.Qca.ruolo.uo.livello.AsLong();
                var livelloRuolo = request.Qca.ruolo.livello.AsLong();
                long? idCurrentUO = request.Qca.ruolo.uo.systemId.AsLong();

                //SUPERIORI
                if (tipoDestinatario == TipoGerarchia.SUPERIORE.ToString())
                {
                    while ((idCurrentUO ?? 0) != 0)
                    {
                        listIdUO.Add((long)idCurrentUO);
                        idCurrentUO = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.SYSTEM_ID == idCurrentUO)
                            .Select(c => c.ID_PARENT)
                            .FirstAsync();
                    }
                }

                //SOTTOPOSTI
                if (tipoDestinatario == TipoGerarchia.INFERIORE.ToString())
                {
                    var listUOInferiori = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.CHA_TIPO_URP == "U" && c.CHA_TIPO_IE == "I" && !c.DTA_FINE.HasValue && c.NUM_LIVELLO >= livelloUO)
                        .Select(c => new
                        {
                            c.ID_PARENT,
                            c.SYSTEM_ID
                        })
                        .ToListAsync();

                    while ((idCurrentUO ?? 0) != 0)
                    {
                        listIdUO.Add((long)idCurrentUO);
                        idCurrentUO = listUOInferiori.Where(i => i.ID_PARENT == idCurrentUO).Select(c => c.SYSTEM_ID).FirstOrDefault();
                    }
                }

                //PARILIVELLO
                if (tipoDestinatario == TipoGerarchia.PARILIVELLO.ToString())
                {
                    var listUOPariLivello = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.CHA_TIPO_URP == "U" && c.CHA_TIPO_IE == "I" && !c.DTA_FINE.HasValue && c.NUM_LIVELLO == livelloUO)
                        .Select(c => c.SYSTEM_ID)
                        .ToListAsync();

                    listIdUO.Add((long)idCurrentUO);
                    listIdUO.AddRange(listUOPariLivello);
                }
                var ruoliRiferimentoQueryable = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Join(this._dbContext.RuoloRegistroEntities.AsNoTracking(), c => c.SYSTEM_ID, r => r.ID_RUOLO_IN_UO, (c, r) => new { c, r })
                    .Join(this._dbContext.TipoRuoloEntities, j => j.c.ID_TIPO_RUOLO, t => t.SYSTEM_ID, (j, t) => new
                    {
                        j.c.SYSTEM_ID,
                        j.c.VAR_DESC_CORR,
                        j.c.VAR_CODICE,
                        j.c.VAR_COD_RUBRICA,
                        j.c.ID_PARENT,
                        j.c.DTA_FINE,
                        j.c.CHA_TIPO_URP,
                        j.c.CHA_TIPO_IE,
                        j.c.CHA_RIFERIMENTO,
                        j.c.ID_UO,
                        j.c.ID_GRUPPO,
                        ID_REGISTRO = j.r.SYSTEM_ID,
                        t.NUM_LIVELLO
                    })
                    .Where(c => c.CHA_TIPO_URP == "R" && c.CHA_TIPO_IE == "I" && c.CHA_RIFERIMENTO == "1" && c.ID_UO == idUO);

                if (request.Qca.queryCorrispondente != null && request.Qca.queryCorrispondente.fineValidita)
                    ruoliRiferimentoQueryable = ruoliRiferimentoQueryable.Where(c => !c.DTA_FINE.HasValue);

                if (!string.IsNullOrEmpty(request.Qca.idRegistro))
                {
                    var idRegistroAsLong = request.Qca.idRegistro.AsLong();
                    ruoliRiferimentoQueryable = ruoliRiferimentoQueryable.Where(c => c.ID_REGISTRO == idRegistroAsLong);
                }

                if (listIdUO != null && listIdUO.Count > 0)
                    ruoliRiferimentoQueryable = ruoliRiferimentoQueryable.Where(c => listIdUO.Contains((long)c.ID_UO));

                if (tipoDestinatario == TipoGerarchia.SUPERIORE.ToString())
                    ruoliRiferimentoQueryable = ruoliRiferimentoQueryable.Where(c => c.NUM_LIVELLO < livelloRuolo);

                if (tipoDestinatario == TipoGerarchia.INFERIORE.ToString())
                    ruoliRiferimentoQueryable = ruoliRiferimentoQueryable.Where(c => c.NUM_LIVELLO > livelloRuolo);

                if (tipoDestinatario == TipoGerarchia.PARILIVELLO.ToString())
                    ruoliRiferimentoQueryable = ruoliRiferimentoQueryable.Where(c => c.NUM_LIVELLO == livelloRuolo);

                var entity = await ruoliRiferimentoQueryable.Select(c => new
                {
                    c.SYSTEM_ID,
                    c.VAR_DESC_CORR,
                    c.VAR_CODICE,
                    c.VAR_COD_RUBRICA,
                    c.ID_PARENT,
                    c.ID_UO,
                    c.ID_GRUPPO,
                    c.NUM_LIVELLO
                })
                .Distinct()
                .ToListAsync();

                foreach (var e in entity)
                {
                    ruoli.Add(new Ruolo
                    {
                        codiceRubrica = e.VAR_COD_RUBRICA,
                        descrizione = e.VAR_DESC_CORR,
                        systemId = e.SYSTEM_ID.ToString(),
                        livello = e.NUM_LIVELLO.ToString(),
                        idGruppo = e.ID_GRUPPO.ToString(),
                        codiceCorrispondente = e.VAR_CODICE,
                        idAmministrazione = request.Qca.ruolo.uo.idAmministrazione,
                        uo = request.Qca.ruolo.uo
                    });
                }

                output = ruoli.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = null;
            }

            return new()
            {
                Output = output
            };

        }


        #region Private Members

        protected readonly ILogger<AddressbookGetRuoliRiferimentoAutorizzatiCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion


    }
}
