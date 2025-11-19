// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Models;

public class MapperProfileHandler : Profile
{
    public MapperProfileHandler()
    {
        this.InitMapper();
    }

    private void InitMapper()
    {
        CreateMap<MODELS.SelectTemplates.Utente, DTOs.Users.GetUserDto>()
            .ForMember(dest => dest.IdPeople, opt => opt.MapFrom(src => src.IdPeople))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.IdAmministrazione, opt => opt.MapFrom(src => src.IdAmministrazione ))
            .ForMember(dest => dest.Cognome, opt => opt.MapFrom(src => src.Cognome))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome));

        CreateMap<MODELS.SelectTemplates.Ruolo, DTOs.Users.GetUserDto>()
           .ForMember(dest => dest.IdCorrGlobali, src => src.MapFrom(opt => opt.SystemId))
           .ForMember(dest => dest.IdGruppo, src => src.MapFrom(opt => opt.IdGruppo));

        CreateMap<MODELS.SelectTemplates.UserDetails, DTOs.Users.GetUserDto>()
            .ForMember(dest => dest.IdPeople, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Cognome, opt => opt.MapFrom(src => src.Cognome))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.UserType == "1"))
            .ForMember(dest => dest.IdGruppo, opt => opt.MapFrom(src => src.GroupSystemId))
            .ForMember(dest => dest.GroupCode, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.GroupDescription, opt => opt.MapFrom(src => src.GroupName))
            .ForMember(dest => dest.IdAmministrazione, opt => opt.MapFrom(src => src.AmministrazioneId)) // IdTenant
            .ForMember(dest => dest.CodiceAmministrazione, opt => opt.MapFrom(src => src.AmministrazioneCodice))
            .ForMember(dest => dest.DescrizioneAmministrazione, opt => opt.MapFrom(src => src.AmministrazioneDescrizione))
            .ForMember(dest => dest.IdCorrGlobali, opt => opt.MapFrom(src => src.CorrGlobaliId));


        #region SERVICE DTOs

        CreateMap<SERVICE_DTO.Documento, Documento>()
            .ForMember(dest => dest.IdDoc, src => src.MapFrom(opt => opt.IdDocumento.ToString()))
            .ForMember(dest => dest.OriginalFileName, src => src.MapFrom(opt => opt.NomeFileOriginale))
            .ForMember(dest => dest.DataDoc, src => src.MapFrom(opt => opt.DataCreazione))
            .ForMember(dest => dest.HasPreview, src => src.MapFrom(opt => opt.HasAnteprima))
            .ForMember(dest => dest.TipoProto, src => src.MapFrom(opt => opt.TipoProtocollo))
            .ForMember(dest => dest.AccessRights, src => src.MapFrom(opt => opt.DirittiDiAccesso))
            .ForMember(dest => dest.CanTransmit, src => src.MapFrom(opt => opt.TrasmissioneAbilitata))
            .ForMember(dest => dest.DataProto, src => src.MapFrom(opt => opt.DataProtocollazione))
            .ForMember(dest => dest.IdDocPrincipale, src => src.MapFrom(opt => opt.IdDocPrincipale));

        CreateMap<SERVICE_DTO.Fascicolo, DTO.Fascicoli.Fascicolo>();

        CreateMap<SERVICE_DTO.Trasmissione, DTO.Trasmissione>()
            .ForMember(dest => dest.IdTrasm, src => src.MapFrom(opt => opt.Id))
            .ForMember(dest => dest.NoteIndividuali, src => src.MapFrom(opt => opt.NoteIndividuali ?? String.Empty));


        CreateMap<SERVICE_DTO.Trasmissione, DTOs.Trasmissions.GetTrasmissionByIdDTO>()
            .ForMember(dest => dest.IdTrasm, src => src.MapFrom(opt => opt.Id))
            .ForMember(dest => dest.NoteIndividuali, src => src.MapFrom(opt => opt.NoteIndividuali ?? String.Empty));

        // Da AuthenticationResult -> AuthenticateUserDTO
        CreateMap<SERVICE_DTO.AuthenticationResult, DTOs.Authentication.AuthenticateUserDTO>()
            .ForMember(dest => dest.UserInfo, act => act.MapFrom(src => src))
            .ForMember(dest => dest.InfoMemento, act => act.MapFrom(src => src));

        CreateMap<SERVICE_DTO.AuthenticationResult, DTOs.Users.UserDetails>()
            .ForMember(dest => dest.FullName, src => src.MapFrom(opt => opt.Descrizione));

        CreateMap<SERVICE_DTO.AuthenticationResult, DTOs.Users.Memento>()
            .ForMember(dest => dest.Dominio, src => src.MapFrom(opt => MementoExtractor(opt.Memento, 0))) 
            .ForMember(dest => dest.Alias, src => src.MapFrom(opt => MementoExtractor(opt.Memento, 1)));

        CreateMap<SERVICE_DTO.Delega, DTOs.Deleghe.Delega>();



        CreateMap<SERVICE_DTO.GetRuoliUtenteResult, DTOs.Roles.RoleDetails>()
            .ForMember(dest => dest.Registri, src => src.MapFrom(opt =>  opt.Registri != null 
                                                    ? opt.Registri.Select(  id => new DTOs.RegisterDetails { SystemId = id.ToString() }).ToList() 
                                                    : new List<DTOs.RegisterDetails>() ));


        CreateMap<SERVICE_DTO.Istanza, DTOs.Instances.Instance>()
            .ForMember(dest => dest.URL, src => src.MapFrom(opt => opt.Url));

        CreateMap<SERVICE_DTO.FileInfo, DTOs.Documents.File>();

        CreateMap<DTOs.Deleghe.Delega, SERVICE_REQUESTS.CreaDelegaRequest>();

        #endregion
    }

    private static string? MementoExtractor(string? memento, int index)
    {
        return memento?.Split('§')?.ElementAtOrDefault(index);
    }

}