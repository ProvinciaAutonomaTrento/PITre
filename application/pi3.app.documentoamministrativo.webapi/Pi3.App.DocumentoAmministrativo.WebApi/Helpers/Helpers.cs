// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Newtonsoft.Json;
using Pi3.App.DocumentoAmministrativo.WebApi.Application;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.App.DocumentoAmministrativo.WebApi.Resources;
using System.Text;

namespace System;

public static class Helpers
{
    public static bool IsValidAggregateId(this string id) {
        if (string.IsNullOrWhiteSpace(id)) return false;

        if (!int.TryParse(id, out var identifier))
            return false;
        else if (identifier <= 0) return false;
        else return true;
    }

    public static async Task<string> GetRawBodyAsync(
        this HttpRequest request,
        Encoding ?encoding = null)
    {
        if (!request.Body.CanSeek)
        {
            // We only do this if the stream isn't *already* seekable,
            // as EnableBuffering will create a new stream instance
            // each time it's called
            request.EnableBuffering();
        }

        request.Body.Position = 0;

        var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);

        var body = await reader.ReadToEndAsync().ConfigureAwait(false);

        request.Body.Position = 0;

        return body;
    }

    public static IEnumerable<Link> GetLinksUrl(string instanceId, string? idDocumento = null,
        string ?idVersione = null, string ?keyword = null, string? idNota = null, string? idDocumentoCollegato = null,
        string ?idTrasmissione = null, string? codiceModello = null) {

        var result = JsonConvert.DeserializeObject<IEnumerable<Link>>(Files.IndexGetHeader_Actual);
        if (result == null)
            throw new LinksResourceNotFoundPi3Exception();

        foreach (var link in result) {
            if (!string.IsNullOrWhiteSpace(idDocumento))
                link.url = link.url.Replace("{idAggregato}", idDocumento);
            if(!string.IsNullOrWhiteSpace(instanceId))
                link.url = link.url.Replace("{instance}", instanceId);
            if (!string.IsNullOrWhiteSpace(idDocumento))
                link.url = link.url.Replace("{idDocumento}", idDocumento);
            if (!string.IsNullOrWhiteSpace(idVersione))
                link.url = link.url.Replace("{idVersione}", idVersione);
            if (!string.IsNullOrWhiteSpace(idNota))
                link.url = link.url.Replace("{idNota}", idNota);
            if (!string.IsNullOrWhiteSpace(codiceModello))
                link.url = link.url.Replace("{codiceModello}", idNota);
            if (!string.IsNullOrWhiteSpace(keyword))
                link.url = link.url.Replace("{keyword}", keyword);
            if (!string.IsNullOrWhiteSpace(keyword))
                link.url = link.url.Replace("{idDocumentoCollegato}", idDocumentoCollegato);
            if (!string.IsNullOrWhiteSpace(keyword))
                link.url = link.url.Replace("{idTrasmissione}", idTrasmissione);
        }
        return result;
    }

    public static void AddDocumentoMapping(this IMapperConfigurationExpression cfg) {
        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo, Documento>();

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.OggettoDelDocumento, OggettoDelDocumento>()
            .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.Descrizione.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.ElementAggregate.Entities.ElementField, ProfileField>()
            .ForMember(dest => dest.ValueAsString, opt => opt.MapFrom(opt => opt.Value.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.ElementAggregate.Entities.ElementProfile, Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.Profile>();

        cfg.CreateMap<Pi3.Core.AggregateModels.ContentElementAggregate.Entities.ContentElementClassification, Classification>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(opt => opt.Name.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.ContentElementAggregate.Entities.RelatedContentElement, RelatedElement>();

        cfg.CreateMap<Pi3.Core.AggregateModels.ContentElementAggregate.ValueObjects.ContentElementPermission, Permission>()
            .ForMember(dest => dest.MemberType, opt => opt.MapFrom(src => src.MemberType.Value.ToString()))
            .ForMember(dest => dest.PermissionType, opt => opt.MapFrom(src => src.PermissionType.ToString()))
            .ForMember(dest => dest.RightType, opt => opt.MapFrom(src => src.RightType.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentAggregate.Entities.DocumentVersion, DocumentVersion>()
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => (src.DocumentBlobRef != null ? src.DocumentBlobRef.FileName : null)))
            .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => (src.DocumentBlobRef != null ? src.DocumentBlobRef.FileSize : null)))
            .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => (src.DocumentBlobRef != null ? src.DocumentBlobRef.ContentType : null)));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.DatiRegistrazioneProtocollo, DatiRegistrazione>()
            .ForMember(dest => dest.TipologiaRegistrazione, opt => opt.MapFrom(src => "Protocollo"))
            .ForMember(dest => dest.TipologiaFlusso, opt => opt.MapFrom(src => src.TipologiaFlusso.ToString()))
            .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.NumeroProtocollo))
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.DataProtocollazione));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.DatiRegistrazioneRepertorio, DatiRegistrazione>()
            .ForMember(dest => dest.TipologiaRegistrazione, opt => opt.MapFrom(src => "Repertorio"))
            .ForMember(dest => dest.TipologiaFlusso, opt => opt.MapFrom(src => src.TipologiaFlusso.ToString()))
            .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.NumeroRegistrazione))
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.DataRegistrazione));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.IdDoc, IdDoc>()
            .ForMember(dest => dest.ImprontaCrittograficaDelDocumento, opt => opt.MapFrom(src => src.ImprontaCrittograficaDelDocumento.Impronta));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.AmministrazioneCheEffettuaLaRegistrazione, Soggetto>()
            .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Autore, Soggetto>()
            .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Mittente, Soggetto>()
            .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Destinatario, Soggetto>()
            .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Assegnatario, Soggetto>()
            .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Operatore, Soggetto>()
            .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ResponsabileGestioneDocumentale, Soggetto>()
            .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ResponsabileServizioProtocollo, Soggetto>()
            .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.RUP, Soggetto>()
            .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.SwProduttore, Soggetto>()
            .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Allegato, Allegato>();

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities.Fascicolo, Aggregazione>()
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.GetType().Name))
            .ForMember(dest => dest.Denominazione, opt => opt.MapFrom(opt => opt.Denominazione.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities.SerieDocumentale, Aggregazione>()
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.GetType().Name))
            .ForMember(dest => dest.Denominazione, opt => opt.MapFrom(opt => opt.Denominazione.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities.SerieDiFascicoli, Aggregazione>()
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.GetType().Name))
            .ForMember(dest => dest.Denominazione, opt => opt.MapFrom(opt => opt.Denominazione.ToString()));

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Annullamento, Annullamento>();

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloMittente, ProtocolloMittente>();

        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloEmergenza, ProtocolloEmergenza>();
        cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities.Nota, Nota>();

    }

}
