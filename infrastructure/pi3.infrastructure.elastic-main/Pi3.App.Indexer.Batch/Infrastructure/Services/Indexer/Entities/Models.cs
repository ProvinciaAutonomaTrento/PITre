// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Indexer.Infrastructure.Services.Elastic.Entities
{
    //public class ElasticElement
    //{
    //    public int Id { get; set; }

    //    public string AsJson { get; set; }

    //    public string IdTenant { get; set; }

    //    public string TypeName { get; set; }

    //    public string Name { get; set; }

    //    public string Description { get; set; }

    //    public DateTime CreationDate { get; set; }

    //    public int CreationYear { get; set; }

    //    public List<ProfileEntity> Profiles { get; set; }

    //    public List<PermissionEntity> Permissions { get; set; }

    //    public List<ClassificationEntity> Classifications { get; set; }

    //    public List<RelatedElementEntity> RelatedElements { get; set; }
    //}

    //public class ProfileEntity
    //{
    //    public string Id { get; set; }

    //    public string Name { get; set; }


    //}

    //public class Field
    //{
    //    public string Id { get; set; }
    //}

    //public class PermissionEntity
    //{
    //    public string IdMember { get; set; }

    //    public string? MemberName { get; set; }

    //    public string? IdUser { get; set; }

    //    public string? IdOwnerUser { get; set; }

    //    public string? IdGroup { get; set; }

    //    public string? IdOwnerGroup { get; set; }

    //    public int PermissionType { get; set; }

    //    public int RightType { get; set; }

    //}

    //public class ClassificationEntity
    //{
    //    public string Id { get; set; }

    //    public string? Name { get; set; }
    //}

    //public class RelatedElementEntity
    //{
    //    public string Id { get; set; }

    //    public bool AsParent { get; set; }
    //}

    //public class AnnullamentoEntity
    //{
    //    public DateTime Data { get; set; }

    //    public string Motivo { get; set; }
    //}

    //public class IdentificativoEntity
    //{
    //    public string? Identificativo { get; set; }

    //    public string? ImpontaCrittograficaDelDocumento { get; set; }

    //    public string? Segnatura { get; set; }
    //}

    //public class VersionEntity
    //{
    //    public string Id { get; set; }

    //    public DateTime CreationDate { get; set; }

    //    public string? Name { get; set; }

    //    public string? IdBlob { get; set; }

    //    public string? FileName { get; set; }
        
    //    public string? FileExtension { get; set; }

    //    public long? FileSize { get; set; }

    //    public string? ContentType { get; set; }

    //    public string? Content { get; set; }
    //}

    #region Registrazione
    //public class Registrazione
    //{
    //    public bool IsRegistrato { get; set; }

    //    public string TipologiaFlusso { get; set; }

    //    public string TipoRegistro { get; set; }

    //    public string CodiceRegistro { get; set; }

    //    public string IdRegistro { get; set; }

    //}

    //public class RegistrazioneProtocollo : Registrazione
    //{
    //    public long? NumeroProtocollo { get; set; }

    //    public int? AnnoProtocollo { get; set; }

    //    public DateTime? DataProtocollazione { get; set; }
    //}

    //public class RegistrazioneRepertorio : Registrazione
    //{
    //    public long? NumeroRegistrazione { get; set; }

    //    public int? AnnoRegistrazione { get; set; }

    //    public DateTime? DataRegistrazione { get; set; }
    //}
    #endregion

    #region Soggetti
    //public class SoggettoEntity
    //{
    //    public PFEntity? PF { get; set; }

    //    public PGEntity? PG { get; set; }

    //    public PAIEntity? PAI { get; set; }

    //    public PAEEntity? PAE { get; set; }
    //}

    //public class PFEntity
    //{
    //    public string Cognome { get; set; }

    //    public string Nome { get; set; }

    //    public string CodiceFiscale { get; set; }

    //    public List<string>? IndirizzoDigitale { get; set; }

    //    public AmministrazioneEntity? Amministrazione { get; set; }

    //    public AmministrazioneEntity? AOO { get; set; }

    //    public AmministrazioneEntity? UOR { get; set; }
    //}

    //public class PGEntity
    //{
    //    public string? DenominazioneOrganizzazione { get; set; }

    //    public string? DenominazioneUfficio { get; set; }

    //    public string? CodiceFiscalePartitaIVA { get; set; }

    //    public List<string>? IndirizzoDigitale { get; set; }
    //}

    //public class PAEEntity
    //{
    //    public string? DenominazioneAmministrazione { get; set; }

    //    public string? DenominazioneUfficio { get; set; }

    //    public List<string>? IndirizzoDigitale { get; set; }
    //}

    //public class PAIEntity
    //{
    //    public AmministrazioneEntity? Amministrazione { get; set; }

    //    public AmministrazioneEntity? AOO { get; set; }

    //    public AmministrazioneEntity? UOR { get; set; }

    //    public List<string>? IndirizzoDigitale { get; set; }
    //}

    //public class AmministrazioneEntity
    //{
    //    public string Denominazione { get; set; }

    //    public string CodiceIPA { get; set; }
    //}


    #endregion

    //public class AllegatoEntity
    //{
    //    public string? Descrizione { get; set; }

    //    public IdentificativoEntity? IdDoc { get; set; }
    //}

    //public class AggregazioneEntity
    //{
    //    public string Id { get; set; }

    //    public string? Tipologia { get; set; }

    //    public string? Denominazione { get; set; }
    //}
    //public class DocumentoAmministrativoEntity : ElasticElement
    //{
    //    public DocumentoAmministrativoEntity()
    //    {
    //        Permissions = new List<PermissionEntity>();
    //        Profiles = new List<ProfileEntity>();
    //        Classifications = new List<ClassificationEntity>();
    //        RelatedElements = new List<RelatedElementEntity>();
    //    }

    //    public VersionEntity? Versions { get; set; }

    //    public long? IdParentDocument { get; set; }

    //    public bool Annullato { get; set; }

    //    public AnnullamentoEntity? Annullamento { get; set; }

    //    public IdentificativoEntity IdDoc { get; set; }

    //    public string? TipologiaDocumentale { get; set; }

    //    public Registrazione? DatiRegistrazione { get; set; }

    //    public SoggettoEntity? Mittente { get; set; }

    //    public List<SoggettoEntity>? MittentiMultipli { get; set; }

    //    public List<SoggettoEntity>? Destinatari { get; set; }

    //    public List<SoggettoEntity>? DestinatariCC { get; set; }

    //    public bool Riservato { get; set; }

    //    public List<AllegatoEntity>? Allegati { get; set; }

    //    public List<AggregazioneEntity>? Aggregazioni { get; set; }    

    //    public IdentificativoEntity? IdIdentificativoDocumentoPrimario { get; set; }
    //}
}
