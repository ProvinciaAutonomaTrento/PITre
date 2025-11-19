// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Globalization;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Profili;

public class StoreProfiloCommandHandler : IRequestHandler<StoreProfiloCommand, StoreProfiloCommandResponse>
{
    private readonly IClaimsPrincipalService _claimsPrincipalService;
    private readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;
    private readonly IMediator _mediator;
    private readonly IPi3DbContext _context;

    public StoreProfiloCommandHandler(
        IClaimsPrincipalService claimsPrincipalService,
        IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository,
        IMediator mediator,
        IPi3DbContext context
        )
    {
        this._claimsPrincipalService = claimsPrincipalService;
        this._aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;
        this._mediator = mediator;
        this._context = context;
    }

    public async Task<StoreProfiloCommandResponse> Handle(StoreProfiloCommand request, CancellationToken cancellationToken)
    {
        var tipoFascicoloEntity = await _context.TipoFascEntities.EntityFromNome(request.Profilo.Nome);
        if (tipoFascicoloEntity == null)
            throw new TipoFascicoloNotFoundPi3Exception(request.Profilo.Nome);

        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

        var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.Id });
        request.Id = safeId.Id;

        var aggregate = await _aggregazioneDocumentaleRepository.Get(idTenant, request.Id);

        // Aggiunta del profilo all'aggregato
        aggregate.AddProfile(
            tipoFascicoloEntity.SYSTEM_ID.ToString(),
            new TextValue(tipoFascicoloEntity.VAR_DESC_FASC));

        // Reperimento dei campi presenti nella tipologia
        var oggettiCustomEntities = await (from occ in this._context.OggettiCustomCompFascEntities.AsNoTracking()
                                           join oc in this._context.OggettiCustomFascEntities.AsNoTracking() on occ.ID_OGG_CUSTOM equals oc.SYSTEM_ID
                                           join to in this._context.TipoOggettoFascEntities.AsNoTracking() on oc.ID_TIPO_OGGETTO equals to.SYSTEM_ID
                                           where occ.ID_TEMPLATE == tipoFascicoloEntity.SYSTEM_ID
                                           select new
                                           {
                                               SYSTEM_ID_OGG_CUSTOM = oc.SYSTEM_ID,
                                               DESCRIZIONE_OGG_CUSTOM = oc.DESCRIZIONE,
                                               SYSTEM_ID_TIPO_OGGETTO = to.SYSTEM_ID,
                                               DESCRIZIONE_TIPO_OGGETTO = to.DESCRIZIONE
                                           }).ToListAsync();


        foreach (var campo in request.Profilo.Campi) {
            var oggettoCustomEntity = oggettiCustomEntities.FirstOrDefault(o => o.DESCRIZIONE_OGG_CUSTOM == campo.Nome);
            if (oggettoCustomEntity == null)
                throw new OggettoCustomNotFoundPi3Exception(campo.Nome);

            ElementFieldValue fieldElement = campo.Tipo switch
            {
                TipoCampoEnum.CampoDiTesto => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                //                    CasellaSelezione casellaSelezione => new ElementFieldMultiValue(casellaSelezione.Valori.Select(v => new TextValue(v)).ToArray()),
                TipoCampoEnum.CasellaSelezione => new ElementFieldSingleValue(new TextValue(campo.Valori[0])),
                TipoCampoEnum.CasellaSelezioneEsclusiva => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                TipoCampoEnum.MenuTendina => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                TipoCampoEnum.Corrispondente => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                TipoCampoEnum.Contatore => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                TipoCampoEnum.Data => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                TipoCampoEnum.Orario => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                TipoCampoEnum.OrarioSecondi => new ElementFieldSingleValue(new TextValue(campo.Valore)),
                _ => throw new TipoCampoNotFoundPi3Exception(campo.Tipo.ToString())
            }; //

            if (campo.Tipo == TipoCampoEnum.Corrispondente)
            {

                var associazione = await _context.CorrGlobaliEntities.FirstOrDefaultAsync
                    (itm => itm.VAR_CODICE.ToUpper() == campo.Valore.ToUpper());
                fieldElement = new ElementFieldSingleValue(new TextValue(associazione.SYSTEM_ID.ToString()));
            }
            var campiData = new TipoCampoEnum[] { TipoCampoEnum.Data, TipoCampoEnum.Orario, TipoCampoEnum.OrarioSecondi };

            if (campiData.Contains(campo.Tipo))
                _ = Convert.ToDateTime(campo.Valore, new CultureInfo("it-IT"));
            //if (campo is CasellaSelezione)
            //{
            //    var casellaSelezione = campo as CasellaSelezione;
            //    var listaValori = new List<string>();
            //    foreach (var selezione in casellaSelezione.Valori) {
            //        var associazione = await _context.AssValoriFascEntities.FirstAsync(itm => itm.ID_OGGETTO_CUSTOM == oggettoCustomEntity.SYSTEM_ID_OGG_CUSTOM
            //            && itm.ABILITATO == 1 && itm.VALORE == selezione );
            //        listaValori.Add(associazione.SYSTEM_ID.ToString());
            //    }
            //    fieldElement = new ElementFieldMultiValue(listaValori.Select(v => new TextValue(v)).ToArray());
            //}
            //else if (campo is MenuTendina) {
            //}


            // Aggiunta del campo
            aggregate.AddProfileField(tipoFascicoloEntity.SYSTEM_ID.ToString(),
                    oggettoCustomEntity.SYSTEM_ID_OGG_CUSTOM.ToString(),
                    new TextValue(oggettoCustomEntity.DESCRIZIONE_OGG_CUSTOM),
                    oggettoCustomEntity.DESCRIZIONE_TIPO_OGGETTO,
                    fieldElement);

        }
        await _aggregazioneDocumentaleRepository.Update(aggregate);

        var result = new StoreProfiloCommandResponse();
        return result;

    }
}
/*
if (request.Profile! != null!)
        {
            var tipoFascicoloEntity = await _pi3DbContext.TipoFascEntities
                .AsNoTracking()
                .Where(ta => ta.VAR_DESC_FASC == request.Profile.Name
&& ta.IN_ESERCIZIO == "SI")
                .Select(ta => new
                {
                    ta.SYSTEM_ID,
                    ta.VAR_DESC_FASC
                })
                .FirstOrDefaultAsync();

            if (tipoFascicoloEntity == null)
            {
                // TODO: lanciare eccezione perché tipologia non trovata
            }

            // Aggiunta del profilo all'aggregato
            aggregate.AddProfile(
                tipoFascicoloEntity.SYSTEM_ID.ToString(),
                new TextValue(tipoFascicoloEntity.VAR_DESC_FASC));

            // Reperimento dei campi presenti nella tipologia
            var oggettiCustomEntities = await (from occ in this._pi3DbContext.OggettiCustomCompFascEntities.AsNoTracking()
                                                 join oc in this._pi3DbContext.OggettiCustomFascEntities.AsNoTracking() on occ.ID_OGG_CUSTOM equals oc.SYSTEM_ID
                                                 join to in this._pi3DbContext.TipoOggettoFascEntities.AsNoTracking() on oc.ID_TIPO_OGGETTO equals to.SYSTEM_ID
                                                 where occ.ID_TEMPLATE == tipoFascicoloEntity.SYSTEM_ID
                                                 select new
                                                 {
                                                     SYSTEM_ID_OGG_CUSTOM = oc.SYSTEM_ID,
                                                     DESCRIZIONE_OGG_CUSTOM = oc.DESCRIZIONE,
                                                     SYSTEM_ID_TIPO_OGGETTO = to.SYSTEM_ID,
                                                     DESCRIZIONE_TIPO_OGGETTO = to.DESCRIZIONE
                                                 }).ToListAsync();

            foreach (var f in request.Profile.SingleValueFields ?? new ProfileSingleValueField[0])
            {
                var oggettoCustomEntity = oggettiCustomEntities.FirstOrDefault(o => o.DESCRIZIONE_OGG_CUSTOM == f.Name);
                if (oggettoCustomEntity == null)
                {
                    // TODO: eccezione, campo non trovato
                }
                // Aggiunta del campo
                aggregate.AddProfileField(tipoFascicoloEntity.SYSTEM_ID.ToString(),
                        oggettoCustomEntity.SYSTEM_ID_OGG_CUSTOM.ToString(),
                        new TextValue(oggettoCustomEntity.DESCRIZIONE_OGG_CUSTOM),
                        oggettoCustomEntity.DESCRIZIONE_TIPO_OGGETTO,
                        new ElementFieldSingleValue(new TextValue(f.ValueAsString)));
            }

            foreach (var f in request.Profile.MultiValueFields ?? new ProfileMultiValueField[0])
            {
                var oggettoCustomEntity = oggettiCustomEntities.FirstOrDefault(o => o.DESCRIZIONE_OGG_CUSTOM == f.Name);
                if (oggettoCustomEntity == null)
                {
                    // TODO: eccezione, campo non trovato
                }

                // Aggiunta del campo
                aggregate.AddProfileField(tipoFascicoloEntity.SYSTEM_ID.ToString(),
                        oggettoCustomEntity.SYSTEM_ID_OGG_CUSTOM.ToString(),
                        new TextValue(oggettoCustomEntity.DESCRIZIONE_OGG_CUSTOM),
                        oggettoCustomEntity.DESCRIZIONE_TIPO_OGGETTO,
                        new ElementFieldMultiValue(f.ValuesAsString.Select(v => new TextValue(v)).ToArray()));
            }

            foreach (var f in request.Profile.LookupValueFields ?? new ProfileLookupValueField[0])
            {
                var oggettoCustomEntity = oggettiCustomEntities.FirstOrDefault(o => o.DESCRIZIONE_OGG_CUSTOM == f.Name);
                if (oggettoCustomEntity == null)
                {
                    // TODO: eccezione, campo non trovato
                }

                // Aggiunta del campo
                aggregate.AddProfileField(tipoFascicoloEntity.SYSTEM_ID.ToString(),
                    oggettoCustomEntity.SYSTEM_ID_OGG_CUSTOM.ToString(),
                    new TextValue(oggettoCustomEntity.DESCRIZIONE_OGG_CUSTOM),
                    oggettoCustomEntity.DESCRIZIONE_TIPO_OGGETTO,
                    new ElementFieldLookupValue(f.IdValue, f.DescriptionValue.ToString()));
            }
        }
*/
