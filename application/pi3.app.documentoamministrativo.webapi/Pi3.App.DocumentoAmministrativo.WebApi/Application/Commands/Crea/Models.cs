// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.App.DocumentoAmministrativo.WebApi.Resources;
using Pi3.Core.SeedWork;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea;

public class OggettoDelDocumento : ValueObject
{
    [Required(AllowEmptyStrings = false)]
    public string Descrizione { get; init; }

    public string? Id { get; set; } = null;
}

[ResourceSwaggerSchema(nameof(Documentation.Classification))]
public class Classification : ValueObject
{
    [ResourceSwaggerSchema("CodiceClassificazione")]
    [Required(AllowEmptyStrings = false)]
    public string CodiceClassificazione { get; set; }

    [ResourceSwaggerSchema("TipologiaFascicolo")]
    public string TipologiaFascicolo { get; set; }
}

public class ClassificationNonClassificato : ValueObject
{
    [ResourceSwaggerSchema("CodiceClassificazione")]
    [Required(AllowEmptyStrings = false)]
    public string CodiceClassificazione { get; set; }
}


[ResourceSwaggerSchema(nameof(Documentation.TipiAggregazioneEnum))]
public enum TipiAggregazioneEnum
{
    Fascicolo,
    SerieDocumentale,
    SerieDiFascicoli
}

[ResourceSwaggerSchema(nameof(Documentation.Aggregazione))]
public class Aggregazione : ValueObject
{
    [ResourceSwaggerSchema("Aggregazione_Id")]
    [Required(AllowEmptyStrings = false)]
    public string Id { get; init; }

    [ResourceSwaggerSchema("Aggregazione_Tipo")]
    [Required]
    public TipiAggregazioneEnum Tipo { get; init; }
}

/// <summary>
/// dati relativi ad un soggetto da associare con vari usi al documento. Il suo significato
/// varia a seconda della proprietà dell'aggregato in cui viene utilizzato
/// </summary>
public class Soggetto : ValueObject
{
    /// <summary>
    /// denominazione del soggetto
    /// </summary>
    [Required()]
    public string Denominazione { get; init; }
    /// <summary>
    /// id del soggetto
    /// </summary>
    public string? Id { get; set; } = null;
}

[ResourceSwaggerSchema(nameof(Documentation.ProtocolloMittente))]
public class ProtocolloMittente : ValueObject
{
    [ResourceSwaggerSchema("Segnatura")]
    public string? Segnatura { get; init; }

    [ResourceSwaggerSchema("DataProtocollazioneMittente")]
    public DateTime? Data { get; init; }

    [ResourceSwaggerSchema("DataArrivo")]
    public DateTime? DataArrivo { get; init; }
}

[ResourceSwaggerSchema(nameof(Documentation.ProtocolloEmergenza))]
public class ProtocolloEmergenza : ValueObject
{
    [ResourceSwaggerSchema("ProtocolloEmergenza_Segnatura")]
    public string Segnatura { get; init; }

    [ResourceSwaggerSchema("ProtocolloEmergenza_Data")]
    public DateTime Data { get; init; }

    public string Cognome { get; init; }

    public string Nome { get; init; }
}

public abstract class ProfileField : ValueObject
{
    [Required(AllowEmptyStrings = false)]
    public string Id { get; init; }

    [Required()]
    public TextValue Name { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string FieldType { get; init; }
}

public enum ProfileFieldSingleValueValueTypesEnum
{
    [AmbientValue("System.Int32")]
    Int32,
    [AmbientValue("System.Int64")]
    Int64,
    [AmbientValue("System.Float")]
    Float,
    [AmbientValue("System.Double")]
    Double,
    [AmbientValue("System.Decimal")]
    Decimal,
    [AmbientValue("Pi3.Core.SeedWork.TextValue")]
    TextValue,
    [AmbientValue("System.Boolean")]
    Boolean,
    [AmbientValue("System.DateTime")]
    DateTime
}

public class ProfileSingleValueField : ProfileField
{
    [Required()]
    public ProfileFieldSingleValueValueTypesEnum ValueType { get; init; }

    [Required()]
    public string ValueAsString { get; init; }
}

public class ProfileMultiValueField : ProfileField
{
    [Required()]
    public ProfileFieldSingleValueValueTypesEnum ValueType { get; init; }

    [Required()]
    public string[] ValuesAsStringArray { get; init; }
}

public class ProfileLookupValueField : ProfileField
{
    [Required(AllowEmptyStrings = false)]
    public string IdValue { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string DescriptionValue { get; init; }
}

public class Profile : ValueObject
{
    [Required(AllowEmptyStrings = false)]
    public string Id { get; init; }

    [Required()]
    public TextValue Name { get; init; }

    public IReadOnlyList<ProfileSingleValueField> SingleValueFields { get; init; }

    public IReadOnlyList<ProfileMultiValueField> MultiValueFields { get; init; }

    public IReadOnlyList<ProfileLookupValueField> LookupValueFields { get; init; }
}

[ResourceSwaggerSchema("IdDoc")]
public class IdDoc : ValueObject
{
    [ResourceSwaggerSchema("IdDoc_UploadId")]
    [Required]
    public Guid UploadId { get; set; }

    [ResourceSwaggerSchema("IdDoc_Descrizione")]
    public string Descrizione { get; init; }
}
