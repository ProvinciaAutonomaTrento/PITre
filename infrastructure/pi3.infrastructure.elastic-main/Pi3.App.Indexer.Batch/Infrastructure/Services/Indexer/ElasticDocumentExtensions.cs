// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Indexer.Infrastructure.Services.Elastic.Entities;
using Pi3.Core.AggregateModels.ContentElementAggregate;
using Pi3.Core.AggregateModels.ContentElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentAggregate.Entities;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Aggregates = Pi3.Core.AggregateModels;

namespace Pi3.App.Indexer.Infrastructure.Services.Elastic
{
    //public static class ElasticDocumentExtensions
    //{
        

    //    public static void FillDocumentoAmministrativo(this Entities.DocumentoAmministrativoEntity doc, DocumentoAmministrativo aggregate)
    //    {
    //        doc.AsJson = JsonSerializer.Serialize(aggregate, aggregate.GetType());

    //        doc.FillElement(aggregate);

    //        doc.FillContentElement(aggregate);

    //        if (aggregate.CurrentVersion is not null) doc.Versions = aggregate.CurrentVersion.ToVersionEntity();

    //        if (!string.IsNullOrWhiteSpace(aggregate.IdParentDocument)) doc.IdParentDocument = aggregate.IdParentDocument.AsLong();

    //        if(aggregate.Annullamento is not null)
    //        {
    //            doc.Annullato = true;
    //            doc.Annullamento = new AnnullamentoEntity
    //            {
    //                Data = aggregate.Annullamento.Data,
    //                Motivo = aggregate.Annullamento.Motivo.ToString()
    //            };
    //        }

    //        if (aggregate.IdDoc is not null)  doc.IdDoc = aggregate.IdDoc.ToIdentificativoEntity();

    //        if (!string.IsNullOrWhiteSpace(aggregate.TipologiaDocumentale)) doc.TipologiaDocumentale = aggregate.TipologiaDocumentale;

    //        if(aggregate.DatiRegistrazione is not null)
    //        {
    //            if(aggregate.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo))
    //            {
    //                doc.DatiRegistrazione = new RegistrazioneProtocollo
    //                {
    //                    IsRegistrato = aggregate.DatiRegistrazione.IsRegistrato,
    //                    IdRegistro = aggregate.DatiRegistrazione.IdRegistro,
    //                    CodiceRegistro = aggregate.DatiRegistrazione.CodiceRegistro,
    //                    TipologiaFlusso = aggregate.DatiRegistrazione.TipologiaFlusso.ToString(),
    //                    TipoRegistro = typeof(DatiRegistrazioneProtocollo).Name,
    //                    NumeroProtocollo = ((DatiRegistrazioneProtocollo)aggregate.DatiRegistrazione).NumeroProtocollo,
    //                    DataProtocollazione = ((DatiRegistrazioneProtocollo)aggregate.DatiRegistrazione).DataProtocollazione,
    //                    AnnoProtocollo = ((DatiRegistrazioneProtocollo)aggregate.DatiRegistrazione).DataProtocollazione.HasValue ? ((DatiRegistrazioneProtocollo)aggregate.DatiRegistrazione).DataProtocollazione.Value.Year : null
    //                };

    //            }
    //            else if(aggregate.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneRepertorio))
    //            {
    //                doc.DatiRegistrazione = new RegistrazioneRepertorio
    //                {
    //                    IsRegistrato = aggregate.DatiRegistrazione.IsRegistrato,
    //                    IdRegistro = aggregate.DatiRegistrazione.IdRegistro,
    //                    CodiceRegistro = aggregate.DatiRegistrazione.CodiceRegistro,
    //                    TipologiaFlusso = aggregate.DatiRegistrazione.TipologiaFlusso.ToString(),
    //                    TipoRegistro = typeof(DatiRegistrazioneRepertorio).Name,
    //                    NumeroRegistrazione = ((DatiRegistrazioneRepertorio)aggregate.DatiRegistrazione).NumeroRegistrazione,
    //                    DataRegistrazione = ((DatiRegistrazioneRepertorio)aggregate.DatiRegistrazione).DataRegistrazione,
    //                    AnnoRegistrazione = ((DatiRegistrazioneRepertorio)aggregate.DatiRegistrazione).DataRegistrazione.HasValue ? ((DatiRegistrazioneRepertorio)aggregate.DatiRegistrazione).DataRegistrazione.Value.Year : null
    //                };
    //            }
    //        }

    //        if(aggregate.Mittente is not null) doc.Mittente = aggregate.Mittente.ToSoggettoEntity();

    //        if(aggregate.MittentiMultipli is not null && aggregate.MittentiMultipli.Any())
    //        {
    //            doc.MittentiMultipli = new List<SoggettoEntity>();
    //            foreach(var m in aggregate.MittentiMultipli) { doc.MittentiMultipli.Add(m.ToSoggettoEntity()); }
    //        }

    //        if (aggregate.Destinatari is not null && aggregate.Destinatari.Any())
    //        {
    //            doc.Destinatari = new List<SoggettoEntity>();
    //            foreach(var d in aggregate.Destinatari) { doc.Destinatari.Add(d.ToSoggettoEntity()); }
    //        }

    //        if(aggregate.DestinatariCc is not null && aggregate.DestinatariCc.Any())
    //        {
    //            doc.DestinatariCC = new List<SoggettoEntity>();
    //            foreach(var d in aggregate.DestinatariCc) { doc.DestinatariCC.Add(d.ToSoggettoEntity()); }
    //        }

    //        doc.Riservato = aggregate.Riservato ?? false;

    //        if (aggregate.Allegati is not null && aggregate.Allegati.Any())
    //        {
    //            doc.Allegati = new List<AllegatoEntity>();
    //            foreach (var a in aggregate.Allegati)
    //            {
    //                doc.Allegati.Add(new AllegatoEntity
    //                {
    //                    Descrizione = a.Descrizione.NormalizeTextValue(),
    //                    IdDoc = a.IdDoc.ToIdentificativoEntity()
    //                });
    //            }
    //        }

    //        if(aggregate.Aggregazioni is not null && aggregate.Aggregazioni.Any())
    //        {
    //            doc.Aggregazioni = new List<AggregazioneEntity>();
    //            foreach(var a in aggregate.Aggregazioni) {
    //                doc.Aggregazioni.Add(new AggregazioneEntity
    //                {
    //                    Id = a.Id,
    //                    Tipologia = a.Tipo,
    //                    Denominazione = a.Denominazione.NormalizeTextValue()
    //                });
    //            }
    //        }

    //        if (aggregate.IdDocPrimario is not null) doc.IdIdentificativoDocumentoPrimario = aggregate.IdDocPrimario.ToIdentificativoEntity();
    //    }

    //    public static void FillDocumentContent(this DocumentoAmministrativoEntity doc, string base64Content)
    //    {
    //        if (doc.Versions is not null) doc.Versions.Content = base64Content;
    //    }

    //    private static void FillElement(this ElasticElement element, Element aggregate)
    //    {
    //        element.Id = Convert.ToInt32(aggregate.Id);
    //        element.IdTenant = aggregate.IdTenant;
    //        element.Name = aggregate.Name.ToString();
    //        element.TypeName = aggregate.TypeName;
    //        element.CreationDate = aggregate.CreationDate;
    //        element.CreationYear = aggregate.CreationDate.Year;


    //        foreach(var p in aggregate.Profiles)
    //        {
    //            element.Profiles.Add(new ProfileEntity
    //            {
    //                Id = p.Id,
    //                Name = p.Name.ToString()
    //            });
    //        }

    //    }

    //    private static void FillContentElement(this ElasticElement element, ContentElement aggregate)
    //    {
    //        foreach(var p in aggregate.Permissions)
    //        {
    //            var permission = new PermissionEntity();

    //            if(!string.IsNullOrEmpty(p.IdMember))
    //            {
    //                permission.IdMember = p.IdMember;
    //            }

    //            if(p.MemberType.HasValue)
    //            {
    //                switch(p.MemberType.Value)
    //                {
    //                    case ContentElementMemberTypesEnum.User:
    //                        if (p.RightType == ContentElementRightTypesEnum.FullControlAllowed) permission.IdOwnerUser = p.IdMember ?? string.Empty;
    //                        else permission.IdUser = p.IdMember ?? string.Empty;
    //                        break;
    //                    case ContentElementMemberTypesEnum.Role:
    //                        if (p.RightType == ContentElementRightTypesEnum.FullControlAllowed) permission.IdOwnerGroup = p.IdMember ?? string.Empty;
    //                        else permission.IdGroup = p.IdMember ?? string.Empty;
    //                        break;
    //                }
    //            }

    //            if(!string.IsNullOrWhiteSpace(p.MemberName)) permission.MemberName = p.MemberName;

    //            permission.PermissionType = (int)p.PermissionType;
    //            permission.RightType = (int)p.RightType;

    //            element.Permissions.Add(permission);
    //        }

    //        if(aggregate.RelatedElements != null)
    //        {
    //            foreach(var r in aggregate.RelatedElements)
    //            {
    //                element.RelatedElements.Add(new RelatedElementEntity
    //                {
    //                    Id = r.Id,
    //                    AsParent = r.AsParent
    //                });
    //            }
    //        }

    //        foreach(var c in aggregate.Classifications)
    //        {
    //            element.Classifications.Add(new ClassificationEntity
    //            {
    //                Id = c.Id,
    //                Name = c.Name.ToString()
    //            });
    //        }

    //    }

    //    private static VersionEntity ToVersionEntity(this DocumentVersion v)
    //    {
    //        var entity = new VersionEntity
    //        {
    //            Id = v.Id,
    //            CreationDate = v.CreationDate
    //        };

    //        entity.Name = v.Name.NormalizeTextValue();

    //        if(v.DocumentBlobRef is not null)
    //        {
    //            entity.IdBlob = v.DocumentBlobRef.IdBlob;
    //            entity.FileName = v.DocumentBlobRef.FileName;
    //            entity.FileExtension = Path.GetExtension(v.DocumentBlobRef.FileName);
    //            entity.FileSize = v.DocumentBlobRef.FileSize;
    //            entity.ContentType = v.DocumentBlobRef.ContentType;
    //        }

    //        return entity;
    //    }

    //    private static IdentificativoEntity ToIdentificativoEntity(this IdDoc idDoc)
    //    {
    //        var i = new IdentificativoEntity { Identificativo = idDoc.Identiticativo };

    //        if(idDoc.ImprontaCrittograficaDelDocumento is not null)
    //        {
    //            i.ImpontaCrittograficaDelDocumento = Convert.ToBase64String(idDoc.ImprontaCrittograficaDelDocumento.Impronta);
    //            i.Segnatura = idDoc.Segnatura;
    //        }

    //        return i;
    //    }

    //    private static SoggettoEntity ToSoggettoEntity(this Soggetto s)
    //    {
    //        if (s is Mittente)
    //        {
    //            var mittente = (Mittente)s;

    //            return new SoggettoEntity
    //            {
    //                PF = mittente.PF.ToPFEntity(),
    //                PG = mittente.PG.ToPGEntity(),
    //                PAE = mittente.PAE.ToPAEEntity(),
    //                PAI = mittente.PAI.ToPAIEntity()
    //            };
    //        }
    //        else
    //        {
    //            var destinatario = (Destinatario)s;

    //            return new SoggettoEntity
    //            {
    //                PF = destinatario.PF.ToPFEntity(),
    //                PG = destinatario.PG.ToPGEntity(),
    //                PAE = destinatario.PAE.ToPAEEntity(),
    //                PAI = destinatario.PAI.ToPAIEntity()
    //            };
    //        }
    //    }

    //    private static PFEntity? ToPFEntity(this PF pf)
    //    {
    //        if (pf is null) return default;
    //        else return new PFEntity
    //        {
    //            Cognome = pf.Cognome,
    //            Nome = pf.Nome,
    //            CodiceFiscale = pf.CodiceFiscale,
    //            Amministrazione = pf.Amministrazione.ToAmministrazioneEntity(),
    //            AOO = pf.AOO.ToAmministrazioneEntity(),
    //            UOR = pf.UOR.ToAmministrazioneEntity(),
    //            IndirizzoDigitale = pf.IndirizziDigitaliDiRiferimento
    //        };
    //    }

    //    private static PGEntity? ToPGEntity(this PG pg)
    //    {
    //        if (pg is null) return default;
    //        else return new PGEntity
    //        {
    //            DenominazioneOrganizzazione = pg.DenominazioneOrganizzazione.NormalizeTextValue(),
    //            DenominazioneUfficio = pg.DenominazioneUfficio.NormalizeTextValue(),
    //            IndirizzoDigitale = pg.IndirizziDigitaliDiRiferimento
    //        };
    //    }

    //    private static PAEEntity? ToPAEEntity(this PAE pae)
    //    {
    //        if (pae is null) return default;
    //        else return new PAEEntity
    //        {
    //            DenominazioneAmministrazione = pae.DenominazioneAmministrazione.NormalizeTextValue(),
    //            DenominazioneUfficio = pae.DenominazioneUfficio.ToString(),
    //            IndirizzoDigitale = pae.IndirizziDigitaliDiRiferimento
    //        };
    //    }

    //    private static PAIEntity? ToPAIEntity(this PAI pai)
    //    {
    //        if (pai is null) return default;
    //        else return new PAIEntity
    //        {
    //            Amministrazione = pai.Amministrazione.ToAmministrazioneEntity(),
    //            AOO = pai.AOO.ToAmministrazioneEntity(),
    //            UOR = pai.UOR.ToAmministrazioneEntity(),
    //            IndirizzoDigitale = pai.IndirizziDigitaliDiRiferimento
    //        };
    //    }

    //    private static AmministrazioneEntity? ToAmministrazioneEntity(this Amministrazione a)
    //    {
    //        if (a is null) return default;
    //        else return new AmministrazioneEntity
    //        {
    //            Denominazione = a.Denominazione.NormalizeTextValue(),
    //            CodiceIPA = a.CodiceIPA
    //        };
    //    }

    //    private static string NormalizeTextValue(this TextValue? t)
    //    {
    //        return t?.ToString() ?? string.Empty;
    //    }
    //}
}
