// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;

using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;
using SELECT_TEMPLATES = Pi3.App.Legacy.Mobile.Models.SelectTemplates;

namespace Pi3.App.Legacy.Mobile.Models;
public class MapperProfileData : Profile
{
    public MapperProfileData()
    {
        this.InitMapper();
    }
    private void InitMapper()
    {
        #region SELECT
        CreateMap<SELECT_TEMPLATES.DettagliDocumento, SERVICE_DTO.Documento>()
            .ForMember(dest => dest.IdDocumento, src => src.MapFrom(opt => opt.SystemId))
            .ForMember(dest => dest.NomeFileOriginale, src => src.MapFrom(opt => opt.NomeOriginale))
            .ForMember(dest => dest.Oggetto, src => src.MapFrom(opt => opt.Oggetto))
            .ForMember(dest => dest.DataCreazione, src => src.MapFrom(opt => opt.CreationDate))
            .ForMember(dest => dest.HasAnteprima, src => src.MapFrom(opt => (opt.FileSize > 0L) 
                                                                            && !String.IsNullOrEmpty(opt.Path)
                                                                            && opt.Path.Contains("PDF", StringComparison.CurrentCultureIgnoreCase) ))
            .ForMember(dest => dest.DimensioneFile, src => src.MapFrom(opt => opt.FileSize))
            .ForMember(dest => dest.FilePath, src => src.MapFrom(opt => opt.Path))
            .ForMember(dest => dest.IsAcquisito, src => src.MapFrom(opt => opt.FileSize > 0L))
            .ForMember(dest => dest.TipoProtocollo, src => src.MapFrom(opt => opt.TipoProto))
            .ForMember(dest => dest.DirittiDiAccesso, src => src.MapFrom(opt => opt.AccessRight))
            .ForMember(dest => dest.TrasmissioneAbilitata, src => src.MapFrom(opt => opt.AccessRight > 45L))
            //.ForMember(dest => dest.IsProtocollato, src => src.MapFrom(opt => String.IsNullOrEmpty(opt.DaProtocollare) || opt.DaProtocollare == "0"))
            .ForMember(dest => dest.IdDocPrincipale, src => src.MapFrom(opt => opt.IdDocumentoPrincipale));

        CreateMap<SELECT_TEMPLATES.TrasmissioneJoinSingolaRagioneUtente, SERVICE_DTO.Trasmissione>()
            .ForMember(dest => dest.Id, src => src.MapFrom(opt => opt.Id))
            .ForMember(dest => dest.Data, src => src.MapFrom(opt => opt.DataInvio))
            .ForMember(dest => dest.NoteGenerali, src => src.MapFrom(opt => opt.NoteGenerali))
            .ForMember(dest => dest.NoteIndividuali, src => src.MapFrom(opt => opt.NoteSingole))
            .ForMember(dest => dest.Ragione, src => src.MapFrom(opt => opt.Ragione))
            .ForMember(dest => dest.IdTrasmUtente, src => src.MapFrom(opt => opt.IdTrasmissioneUtente))
            .ForMember(dest => dest.Accettata, src => src.MapFrom(opt => opt.DataAccettata.HasValue))
            .ForMember(dest => dest.Rifiutata, src => src.MapFrom(opt => opt.DataRifiutata.HasValue))
            .ForMember(dest => dest.HasWorkflow, src => src.MapFrom(opt => "W".Equals(opt.TipoRagione, StringComparison.CurrentCultureIgnoreCase)));

        CreateMap<SELECT_TEMPLATES.Utente, SERVICE_DTO.AuthenticationResult>()
            .ForMember(dest => dest.IdPeople, src => src.MapFrom(opt => opt.IdPeople))
            .ForMember(dest => dest.UserId, src => src.MapFrom(opt => opt.UserId))
            .ForMember(dest => dest.IdAmministrazione, src => src.MapFrom(opt => opt.IdAmministrazione))
            .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => $"{opt.Cognome} {opt.Nome}"))
            .ForMember(dest => dest.Memento, src => src.MapFrom(opt => opt.Memento));

        CreateMap<SELECT_TEMPLATES.RuoloUtente, SERVICE_DTO.GetRuoliUtenteResult>()
            .ForMember(dest => dest.Id, src => src.MapFrom(opt => opt.Id))
            .ForMember(dest => dest.IdUO, src => src.MapFrom(opt => opt.IdUO))
            .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.Descrizione))
            .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.Codice))
            .ForMember(dest => dest.Livello, src => src.MapFrom(opt => opt.Livello))
            .ForMember(dest => dest.IdGruppo, src => src.MapFrom(opt => opt.IdGruppo));

        #endregion


    }
}
