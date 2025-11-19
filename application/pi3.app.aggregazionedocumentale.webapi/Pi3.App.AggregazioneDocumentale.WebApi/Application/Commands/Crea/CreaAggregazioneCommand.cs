// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Crea
{
    [ResourceSwaggerSchema(nameof(Documentation.CreaAggregazioneCommandResponse))]
    public class CreaAggregazioneCommandResponse : ValueObject {
        [ResourceSwaggerSchema(nameof(Documentation.idAggregazioneCreata))]
        public string Id { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisica_Head))]
    public class CollocazioneFisica : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.CodiceCollocazioneFisica))]
        public string Codice { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.Cartaceo))]
        public bool Cartaceo { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.Classification_Head))]
    public class Classification : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.TipologiaFascicolo))]
        public string TipologiaFascicolo { get; set;}

        [ResourceSwaggerSchema(nameof(Documentation.CodiceClassificazione))]
        public string CodiceClassificazione { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.CreaAggregazioneCommand_Head))]
    public class CreaAggregazioneCommand : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.DescrizioneAggregazione))]
        public string Descrizione { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.TipoAggregazione))]
        public TipiAggregazioneEnum TipoAggregazione { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.TipologieFascicoloEnum))]
        public TipologieFascicoloEnum? TipologiaFascicolo { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.TipologieVisibilitaEnum))]
        public TipologieVisibilitaEnum? TipologiaVisibilita { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.Classificazione))]
        public Classification Classificazione { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.Documenti))]
        public IEnumerable<IdDoc> Documenti { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.Sottofascicoli))]
        public FolderHierarcy Sottofascicoli { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisica))]
        public CollocazioneFisica CollocazioneFisica { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.CodiceRegistro))]
        public string CodiceRegistro { get; set; }
    }


    public class CreaAggregazioneRequest: IRequest<CreaAggregazioneCommandResponse>
    {
        public CreaAggregazioneCommand CreaAggregazione { get; set; }
    }
}
