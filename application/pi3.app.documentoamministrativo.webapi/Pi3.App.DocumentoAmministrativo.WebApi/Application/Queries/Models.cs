// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries
{
    public class ProtocolloEmergenza : ValueObject
    {
        public string Segnatura { get; init; }

        public DateTime Data { get; init; }

        public string Cognome { get; init; }

        public string Nome { get; init; }
    }

    public class ProtocolloMittente : ValueObject
    {
        public string? Segnatura { get; init; }
        public DateTime? Data { get; init; }
        public DateTime? DataArrivo { get; init; }
    }

    public class Annullamento : ValueObject
    {
        public DateTime Data { get; init; }

        public string Motivo { get; init; }
    }

    public class Aggregazione : ValueObject
    {
        public virtual string Tipo { get; init; }

        public string Id { get; init; }

        public string Denominazione { get; init; }
    }

    public class Allegato : ValueObject
    {
        public IdDoc IdDoc { get; init; }

        public string? Descrizione { get; init; }
    }

    public class Soggetto : ValueObject
    {
        public string Ruolo { get; init; }

        public string Descrizione { get; init; }
    }


    public class IdDoc : ValueObject
    {
        public byte[] ImprontaCrittograficaDelDocumento { get; init; }

        public string Identiticativo { get; init; }

        public string? Segnatura { get; init; }
    }

    public class DatiRegistrazione : ValueObject
    {
        public string TipologiaRegistrazione { get; init; }

        public string TipologiaFlusso { get; init; }

        public bool IsRegistrato { get; init; }

        public string IdRegistro { get; init; }

        public string CodiceRegistro { get; init; }

        public DateTime? Data { get; init; }

        public long? Numero { get; init; }
    }

    public class DocumentVersion : ValueObject
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public int VersionNumber { get; init; }
        public DateTime CreationDate { get; init; }
        public string? FileName { get; init; }
        public long? FileSize { get; init; }
        public string? ContentType { get; init; }
    }

    public class Permission
    {
        public string IdMember { get; init; }

        public string MemberName { get; init; }

        public string MemberType { get; init; }

        public string RightType { get; init; }

        public string PermissionType { get; init; }
    }

    public class RelatedElement : ValueObject
    {
        public string Id { get; init; }

        public bool AsParent { get; init; }
    }

    public class Classification : ValueObject
    {
        public string Id { get; init; }

        public string Name { get; init; }
    }

    public class ProfileField : ValueObject
    {
        public string Id { get; init; }

        public TextValue Name { get; init; }

        public string Type { get; init; }

        public string ValueAsString { get; init; }
    }

    public class Profile : ValueObject
    {
        public string Id { get; init; }

        public TextValue Name { get; init; }

        public IReadOnlyList<ProfileField> Fields { get; init; }
    }

    public enum TipologiaVisibilitaNotaEnum
    {
        Personale,
        Ruolo,
        RF,
        Pubblica
    }

    public class Nota : ValueObject
    {
        public string Id { get; init; }

        public TipologiaVisibilitaNotaEnum TipologiaVisibilita { get; init; }

        public string Testo { get; init; }
    }

    public class OggettoDelDocumento : ValueObject
    {
        [Required(AllowEmptyStrings = false)]
        public string Descrizione { get; init; }

        public string? Id { get; set; } = null;
    }

    public class Documento : ValueObject
    {
        public string Id { get; init; }

        public string IdTenant { get; init; }

        public DateTime CreationDate { get; init; }

        public OggettoDelDocumento OggettoDelDocumento { get; init; }

        public IReadOnlyList<Classification> Classifications { get; set; }

        public bool Reserved { get; init; }

        public DateTime? ReservedDate { get; init; }

        public string? ReserveIdUser { get; init; }

        public IReadOnlyList<RelatedElement> RelatedElements { get; init; }

        public IReadOnlyList<Permission> Permissions { get; init; }

        public string? IdParentDocument { get; init; }

        public IReadOnlyList<DocumentVersion> Versions { get; init; }

        public bool InRecycleBin { get; init; }

        public DatiRegistrazione DatiRegistrazione { get; init; }

        public IdDoc IdDoc { get; init; }

        public string? TipologiaDocumentale { get; init; }

        public IReadOnlyList<Soggetto> Soggetti { get; init; }

        public IReadOnlyList<Allegato> Allegati { get; init; }

        public bool? Riservato { get; init; }

        public IReadOnlyList<Aggregazione> Aggregazioni { get; init; }

        public IReadOnlyList<string>? Keywords { get; init; }

        public IReadOnlyList<Profile> Profiles { get; init; }

        public IdDoc? IdDocPrimario { get; init; }

        public int? TempoDiConservazione { get; init; }

        public IReadOnlyList<Nota> Note { get; init; }

        public Annullamento? Annullamento { get; init; }

        public ProtocolloMittente? ProtocolloMittente { get; init; }

        public ProtocolloEmergenza? ProtocolloEmergenza { get; init; }
    }

}
