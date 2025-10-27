// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Profili;

public class StoreProfiloCommandHandler : IRequestHandler<StoreProfiloCommand, StoreProfiloCommandResponse>
{
    private readonly IClaimsPrincipalService _claimsPrincipalService;
    private readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
    private readonly IMediator _mediator;
    private readonly IPi3DbContext _context;

    public StoreProfiloCommandHandler(
        IClaimsPrincipalService claimsPrincipalService,
        IDocumentoAmministrativoRepository documentoAmministrativoRepository,
        IMediator mediator,
        IPi3DbContext context
        )
    {
        this._claimsPrincipalService = claimsPrincipalService;
        this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        this._mediator = mediator;
        this._context = context;
    }

    public async Task<StoreProfiloCommandResponse> Handle(StoreProfiloCommand request, CancellationToken cancellationToken)
    {
        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

        var aggregate = await _documentoAmministrativoRepository.Get(idTenant, request.Id);

        await Helpers.RegistraProfilo(_context, aggregate, request.Profilo);

        //var tipoAttoEntity = await _context.TipoAttoEntities.FirstOrDefaultAsync(ta => ta.VAR_DESC_ATTO.ToUpper() == request.Profilo.Nome.ToUpper());
        //if (tipoAttoEntity == null)
        //    throw new TipoFascicoloNotFoundPi3Exception(request.Profilo.Nome);

        //if (aggregate.Profiles?.FirstOrDefault()?.Id != tipoAttoEntity.SYSTEM_ID.ToString()) {
        //    aggregate.RemoveProfile(aggregate.Profiles.FirstOrDefault().Id);
        //    // Aggiunta del profilo all'aggregato
        //    aggregate.AddProfile(
        //        tipoAttoEntity.SYSTEM_ID.ToString(),
        //        new TextValue(tipoAttoEntity.VAR_DESC_ATTO));
        //}

        //// Reperimento dei campi presenti nella tipologia
        //var oggettiCustomEntities = await (from occ in this._context.OggettiCustomCompEntities.AsNoTracking()
        //                                   join oc in this._context.OggettiCustomEntities.AsNoTracking() on occ.ID_OGG_CUSTOM equals oc.SYSTEM_ID
        //                                   join to in this._context.TipoOggettoEntities.AsNoTracking() on oc.ID_TIPO_OGGETTO equals to.SYSTEM_ID
        //                                   where occ.ID_TEMPLATE == tipoAttoEntity.SYSTEM_ID
        //                                   select new
        //                                   {
        //                                       SYSTEM_ID_OGG_CUSTOM = oc.SYSTEM_ID,
        //                                       DESCRIZIONE_OGG_CUSTOM = oc.DESCRIZIONE,
        //                                       SYSTEM_ID_TIPO_OGGETTO = to.SYSTEM_ID,
        //                                       DESCRIZIONE_TIPO_OGGETTO = to.DESCRIZIONE
        //                                   }).ToListAsync();

        //foreach (var campo in request.Profilo.Campi)
        //{
        //    var oggettoCustomEntity = oggettiCustomEntities.FirstOrDefault(o => o.DESCRIZIONE_OGG_CUSTOM == campo.Nome);
        //    if (oggettoCustomEntity == null)
        //        throw new OggettoCustomNotFoundPi3Exception(campo.Nome);

        //    ElementFieldValue fieldElement = campo.Tipo switch
        //    {
        //        TipoCampoEnum.CampoDiTesto => new ElementFieldSingleValue(new TextValue(campo.Valore)),
        //        //                    CasellaSelezione casellaSelezione => new ElementFieldMultiValue(casellaSelezione.Valori.Select(v => new TextValue(v)).ToArray()),
        //        TipoCampoEnum.CasellaSelezione => new ElementFieldSingleValue(new TextValue(campo.Valori[0])),
        //        TipoCampoEnum.CasellaSelezioneEsclusiva => new ElementFieldSingleValue(new TextValue(campo.Valore)),
        //        TipoCampoEnum.MenuTendina => new ElementFieldSingleValue(new TextValue(campo.Valore)),
        //        TipoCampoEnum.Corrispondente => new ElementFieldSingleValue(new TextValue(campo.Valore)),
        //        TipoCampoEnum.Contatore => new ElementFieldSingleValue(new TextValue(campo.Valore)),
        //        TipoCampoEnum.Data => new ElementFieldSingleValue(new TextValue(campo.Valore)),
        //        TipoCampoEnum.Orario => new ElementFieldSingleValue(new TextValue(campo.Valore)),
        //        TipoCampoEnum.OrarioSecondi => new ElementFieldSingleValue(new TextValue(campo.Valore)),
        //        _ => throw new TipoCampoNotFoundPi3Exception(campo.Tipo.ToString())
        //    }; //

        //    if (campo.Tipo == TipoCampoEnum.Corrispondente)
        //    {

        //        var associazione = await _context.CorrGlobaliEntities.FirstOrDefaultAsync
        //            (itm => itm.VAR_CODICE.ToUpper() == campo.Valore.ToUpper());
        //        fieldElement = new ElementFieldSingleValue(new TextValue(associazione.SYSTEM_ID.ToString()));
        //    }
        //    var campiData = new TipoCampoEnum[] { TipoCampoEnum.Data, TipoCampoEnum.Orario, TipoCampoEnum.OrarioSecondi };

        //    if (campiData.Contains(campo.Tipo))
        //        _ = Convert.ToDateTime(campo.Valore, new CultureInfo("it-IT"));


        //    //var oggettoCustomEntity = oggettiCustomEntities.FirstOrDefault(o => o.DESCRIZIONE_OGG_CUSTOM == nome);
        //    //if (oggettoCustomEntity == null)
        //    //    throw new OggettoCustomNotFoundPi3Exception(nome);

        //    var esistente = aggregate.Profiles.First().Fields.FirstOrDefault(fld => fld.Id == oggettoCustomEntity.SYSTEM_ID_OGG_CUSTOM.ToString());
        //    if (esistente == null)
        //        aggregate.AddProfileField(tipoAttoEntity.SYSTEM_ID.ToString(),
        //                oggettoCustomEntity.SYSTEM_ID_OGG_CUSTOM.ToString(),
        //                new TextValue(oggettoCustomEntity.DESCRIZIONE_OGG_CUSTOM),
        //                oggettoCustomEntity.DESCRIZIONE_TIPO_OGGETTO,
        //                fieldElement);
        //    else
        //        aggregate.ChangeProfileFieldValue(tipoAttoEntity.SYSTEM_ID.ToString(),
        //                oggettoCustomEntity.SYSTEM_ID_OGG_CUSTOM.ToString(),
        //                fieldElement);

        //}

        await _documentoAmministrativoRepository.Update(aggregate);

        var result = new StoreProfiloCommandResponse();
        return result;

    }
}
