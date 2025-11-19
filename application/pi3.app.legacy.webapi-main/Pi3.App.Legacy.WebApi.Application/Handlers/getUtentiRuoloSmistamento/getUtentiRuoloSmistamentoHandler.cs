// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.LibroFirma;
using DocsPaVO.Smistamento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getUtentiRuoloSmistamentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.getUtentiRuoloSmistamento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getUtentiRuoloSmistamento
{
    public class getUtentiRuoloSmistamentoHandler : IRequestHandler<getUtentiRuoloSmistamentoRequest, getUtentiRuoloSmistamentoResult>
    {
        #region Public Members

        public getUtentiRuoloSmistamentoHandler(ILogger<getUtentiRuoloSmistamentoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<getUtentiRuoloSmistamentoResult> Handle(getUtentiRuoloSmistamentoRequest request, CancellationToken cancellationToken)
        {
            UtenteSmistamento[] output = null;

            try
            {
                var idAsLong = request.id.AsLong();

                var queryableUtentiRuolo = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Join(this._dbContext.PeopleGroupEntities.AsNoTracking(), g => g.ID_PEOPLE, pg => pg.PEOPLE_SYSTEM_ID, (g, pg) => new { g, pg })
                    .Join(this._dbContext.PeopleEntities.AsNoTracking(), j => j.pg.PEOPLE_SYSTEM_ID, p => p.SYSTEM_ID, (j, p) => new { j.g, j.pg, p })
                    .Where(j => j.p.DISABLED == "N" && !j.g.DTA_FINE.HasValue && !j.pg.DTA_FINE.HasValue && j.g.CHA_TIPO_URP == "P" && j.g.CHA_TIPO_IE == "I");

                if (request.queryParam.Equals("R"))
                {
                    var idGruppo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idAsLong).Select(c => c.ID_GRUPPO).FirstAsync();
                    queryableUtentiRuolo = queryableUtentiRuolo.Where(j => j.pg.GROUPS_SYSTEM_ID == idGruppo);
                }
                else
                {
                    queryableUtentiRuolo = queryableUtentiRuolo.Where(j => j.g.SYSTEM_ID == idAsLong);
                }

                var utentiRuoloEntities = await queryableUtentiRuolo
                    .Select(j => new UtentiRuoloEntity()
                    {
                        ID = j.p.SYSTEM_ID,
                        ID_CORR_GLOBALI = j.g.SYSTEM_ID,
                        CODICE_UTENTE = j.p.USER_ID,
                        DESCRIZIONE_UTENTE = j.p.FULL_NAME,
                        EMAIL_UTENTE = j.p.EMAIL_ADDRESS,
                        CHA_NOTIFICA = j.p.CHA_NOTIFICA,
                        CHA_NOTIFICA_CON_ALLEGATO = j.p.CHA_NOTIFICA_CON_ALLEGATO,
                        VAR_COGNOME = j.p.VAR_COGNOME
                    })
                    .OrderBy(p => p.VAR_COGNOME)
                    .Distinct()
                    .ToListAsync();

                output = this._mapper.Map<UtenteSmistamento[]>(utentiRuoloEntities);

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new getUtentiRuoloSmistamentoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getUtentiRuoloSmistamentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UtentiRuoloEntity, UtenteSmistamento>()
                     .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                     .ForMember(dest => dest.IDCorrGlobali, opt => opt.MapFrom(src => src.ID_CORR_GLOBALI))
                     .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.CODICE_UTENTE))
                     .ForMember(dest => dest.Denominazione, src => src.MapFrom(src => src.DESCRIZIONE_UTENTE))
                     .ForMember(dest => dest.EMail, src => src.MapFrom(src => src.EMAIL_UTENTE))
                     .AfterMap((src, dest) =>
                     {
                         dest.TipoNotificaSmistamento = TipoNotificaSmistamentoEnum.NoMail;

                         var tipoNotifica = src.CHA_NOTIFICA + src.CHA_NOTIFICA_CON_ALLEGATO;
                         if (tipoNotifica.Equals(""))
                             dest.TipoNotificaSmistamento = TipoNotificaSmistamentoEnum.NoMail;
                         else if (tipoNotifica.Equals("E") || tipoNotifica.Equals("E0"))
                             dest.TipoNotificaSmistamento = TipoNotificaSmistamentoEnum.Mail;
                         else if (tipoNotifica.Equals("E1"))
                             dest.TipoNotificaSmistamento = TipoNotificaSmistamentoEnum.MailConAllegati;
                         else if (tipoNotifica.Equals("1"))
                             dest.TipoNotificaSmistamento = TipoNotificaSmistamentoEnum.SoloAllegati;
                     });
            });

            _mapper = configuration.CreateMapper();
        }

        protected class UtentiRuoloEntity
        {
            public long? ID { get; set; }
            public long? ID_CORR_GLOBALI { get; set; }
            public string? CODICE_UTENTE { get; set; }
            public string? DESCRIZIONE_UTENTE { get; set; }
            public string? EMAIL_UTENTE { get; set; }
            public string? CHA_NOTIFICA { get; set; }
            public string? CHA_NOTIFICA_CON_ALLEGATO { get; set; }
            public string? VAR_COGNOME { get; set; }

        }

        #endregion
    }
}
