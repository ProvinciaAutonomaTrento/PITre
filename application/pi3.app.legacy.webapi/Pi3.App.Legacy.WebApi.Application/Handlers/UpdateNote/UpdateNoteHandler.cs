// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Note;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.NotaAggregate;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateNote
{

    public class UpdateNoteHandler : IRequestHandler<Application.Requests.UpdateNote, UpdateNoteResult>
    {
        #region Public Members

        public UpdateNoteHandler(ILogger<UpdateNoteHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, IPi3DbContext dbContext, IDistributedCache distributedCache, IConfiguration configuration,
            INotaRepository note)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
            _distributedCache = distributedCache;
            _configuration = configuration;
            _notaRepository = note;

        }



        public async Task<UpdateNoteResult> Handle(Application.Requests.UpdateNote request, CancellationToken cancellationToken)
        {
            var IdTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var IdGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var IdUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var oggetto = request.oggettoAssociato.Id.AsLong();

            try
            {
                TipiOggettoEnum tipiOggetto = (TipiOggettoEnum)request.oggettoAssociato.TipoOggetto;
                string idOggettoAssociato = request.oggettoAssociato.Id;
                foreach (InfoNota nota in request.note)
                {
                    if (nota.DaInserire)
                    {
                        var tipoAccesso = (TipoAccessoNotaEnum)nota.TipoVisibilita;
                        var autore = new AutoreNota() { IdRuolo = nota.UtenteCreatore.IdRuolo, IdUtente = nota.UtenteCreatore.IdUtente, IdUtenteDelegato = nota.IdPeopleDelegato };
                        var testo = new TextValue(nota.Testo);

                        var aggregate = new Nota(IdTenant, nota.DataCreazione, testo, null, autore, idOggettoAssociato,
                            tipiOggetto, tipoAccesso, tipoAccesso == TipoAccessoNotaEnum.RF ? nota.IdRfAssociato : null);

                        await _notaRepository.Add(aggregate);

                        nota.Id = aggregate.Id;
                        nota.DaInserire = false;
                    }
                    else if (nota.DaRimuovere)
                    {
                        if (await this._notaRepository.Exists(IdTenant, nota.Id))
                        {
                            // fare get
                            var aggregate = await this._notaRepository.Get(IdTenant, nota.Id);

                            // delete
                            await _notaRepository.Delete(aggregate);

                            // nota.DaRimuovere = false;
                        }
                    }
                    else
                    {
                        long idn = nota.Id.AsLong();
                        //if (await this._noteRepository.Exists(idAmm, nota.Id))
                        if (_dbContext.NoteEntities.Where(w => w.SYSTEM_ID == idn).Any())
                        {
                            // fare get
                            var aggregate = await this._notaRepository.Get(IdTenant, nota.Id);
                            aggregate.ChangeDescription(new TextValue(nota.Testo));

                            if (aggregate.TipoAccesso != (TipoAccessoNotaEnum)nota.TipoVisibilita)
                            {
                                switch ((TipoAccessoNotaEnum)nota.TipoVisibilita)
                                {
                                    case TipoAccessoNotaEnum.Pubblica: aggregate.SetAccessoPubblico(); break;

                                    case TipoAccessoNotaEnum.RF: aggregate.SetAccessoRF(nota.IdRfAssociato); break;

                                    case TipoAccessoNotaEnum.Personale: aggregate.SetAccessoPersonale(); break;

                                    case TipoAccessoNotaEnum.Ruolo: aggregate.SetAccessoRuolo(); break;
                                }
                            }

                            // update
                            await _notaRepository.Update(aggregate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                throw new Exception(ex.Message);
            }

            return new UpdateNoteResult(request.note);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UpdateNoteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IConfiguration _configuration;
        protected readonly INotaRepository _notaRepository;


        #endregion
    }

}
