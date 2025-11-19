// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.Carica
{
    [ResourceSwaggerSchema(nameof(Documentation.Nota_Head))]
    public class Nota : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.Nota_Id))]
        public string Id { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Nota_TipologiaVisibilita))]
        public string TipologiaVisibilita { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Nota_Testo))]
        public string Testo { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.ElementField_Head))]
    public class ElementField : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.ElementField_Id))]
        public string Id { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.ElementField_Nome))]
        public string Nome { get; protected set; }

        [ResourceSwaggerSchema(nameof(Documentation.ElementField_Tipo))]
        public string Tipo { get; protected set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.IdDoc_Head))]
    public class IdDoc : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.IdDoc_Identiticativo))]
        public string Identiticativo { get; init; }

        [ResourceSwaggerSchema(nameof(Documentation.IdDoc_Segnatura))]
        public string? Segnatura { get; init; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisica_Head))]
    public class CollocazioneFisica : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisica_Id))]
        public string Id { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisica_Descrizione))]
        public string Descrizione { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisica_Cartaceo))]
        public bool Cartaceo { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.FolderHierarcy_Head))]
    public class FolderHierarcy : ValueObject
    {
        [ResourceSwaggerSchema("FolderHierarcy_Id")]
        public string Id { get; set; }

        [ResourceSwaggerSchema("FolderHierarcy_Nome")]
        public string Nome { get; set; }

        [ResourceSwaggerSchema("FolderHierarcy_Documenti")]
        public IEnumerable<IdDoc> Documenti { get; set; }

        [ResourceSwaggerSchema("FolderHierarcy_Sottofascicoli")]
        public IEnumerable<FolderHierarcy>? Sottofascicoli { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.Classificazione))]
    public class Classification : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.idClassificazione))]
        public string Id { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.nomeClassificazione))]
        public string Nome { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.IdSerieDocumentale))]
        public string IdSerieDocumentale { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.NameSerieDocumentale))]
        public string NameSerieDocumentale { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.Pagination_Head))]
    public class Pagination : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.Pagination_Salta))]
        public int Salta { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.Pagination_Prendi))]
        public int Prendi { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.Profile_Head))]
    public class Profile: ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.Profile_Id))]
        public string Id { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Profile_Nome))]
        public string Nome { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Profile_Metadata))]
        public IReadOnlyDictionary<string, string> Metadata { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Profile_Campi))]
        public IReadOnlyList<ElementField> Campi { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.Permission))]
    public class Permission : ValueObject
    {
        public string IdMembro { get; set; }
        public string NomeMembro { get; set; }
        public string TipoMembro { get; set; }
        public string TipoPermesso { get; set; }
        public string TipoDiritto { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.AggregazioneDocumentale))]
    public class AggregazioneDocumentale : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.IdAggregazioneDocumentale))]
        public string Id { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.DescrizioneAggregazione))]
        public string Descrizione { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.TipoAggregazione))]
        public TipiAggregazioneEnum TipoAggregazione { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.TipologieFascicoloEnum))]
        public TipologieFascicoloEnum? TipologiaFascicolo { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.TipologieVisibilitaEnum))]
        public TipologieVisibilitaEnum? TipologiaVisibilita { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.ClassificazioneAggregazione))]
        public IEnumerable<Classification> Classificazioni { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.Documenti))]
        public IEnumerable<IdDoc> Documenti { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.Sottofascicoli))]
        public IEnumerable<FolderHierarcy> Sottofascicoli { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.CollocazioneFisica))]
        public CollocazioneFisica CollocazioneFisica { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.IdRegistro))]
        public string IdRegistro { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.DescrizioneRegistro))]
        public string DescrizioneRegistro { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.Permessi))]
        public IEnumerable<Permission> Permessi { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.Profili))]
        public IEnumerable<Profile> Profili { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.Note))]
        public IEnumerable<Nota> Note { get; set; }
    }

    [ResourceSwaggerSchema(nameof(Documentation.CaricaAggregazioneQueryResult))]
    public class CaricaAggregazioneQueryResult : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.AggregazioneDocumentale))]
        public AggregazioneDocumentale AggregazioneDocumentale {  get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    public class CaricaAggregazioneQuery: IRequest<CaricaAggregazioneQueryResult>
    {
        [Required]
        public string Id { get; set; }

        public bool LoadFolderHierarchy { get; set; }

        public Pagination? FoldersPagination { get; set; }

        public bool LoadDocuments { get; set; }

        public Pagination? DocumentsPagination { get; set; }

        public bool LoadPermissions { get; set; }

        public bool LoadProfiles { get; set; }
        public bool LoadNote { get; set; }
    }
}
