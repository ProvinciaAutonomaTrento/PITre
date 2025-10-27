// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.Legacy.Mobile.ApiHandler.Models.QueryResponses.Deleghe;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Queries;

#region Users
public record GetUserQuery(string Username, long IdGroup, long IdAmministrazione) : IRequest<DTO.Users.GetUserDto>;
#endregion


#region Trasmissions
public record GetUserTrasmissionById(long IdTrasmissione) : IRequest<DTO.Trasmissions.GetTrasmissionByIdDTO>;
public record GetTrasmissionById(long IdTrasmissione) : IRequest<RESPONSE.GetTrasmissioneByIdResponse>;
#endregion

#region Documents
public record GetDocumentInfoQuery(long IdDocument) : IRequest<DTO.Documents.GetDocumentInfoDto>;
public record GetFileByIdDocumentRequest(long Id) : IRequest<RESPONSE.Documents.GetFileByIdDocumentResponse>;
#endregion

#region Authentication
public record AuthenticateUserQuery(
    string Username, 
    string Password, 
    string? IdAmministrazione ) : IRequest<DTO.Authentication.AuthenticateUserDTO>;
#endregion

#region Instance
public record GetInstanceList() : IRequest<IEnumerable<DTO.Instances.Instance>>;
#endregion

#region Notifications

#endregion

#region Deleghe
public record GetDelegheRequest(string Stato, String Tipo) : IRequest<GetDelegheResponse>;
public record GetModelliDelegheRequest() : IRequest<GetModelliDelegheResponse>;
#endregion

#region Fascioli
public record GetFascicoloInfoByIdQuery(long IdFascicolo, long IdTrasmissione) : IRequest<RESPONSE.Fascicoli.GetFascicoloInfoByIdResponse>;
#endregion