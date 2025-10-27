// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Commands;

#region Notifiche
public record RimuoviNotificaCommand(long IdEvento) : IRequest<RESULT.Notifiche.RimuoviNotificaResult>;
#endregion

#region Deleghe
public record CreaDelegaCommand(DTO.Deleghe.Delega Delega): IRequest<RESULT.Deleghe.CreaDelegaResult>;
public record RevocaDelegheRequest( IEnumerable<DTO.Deleghe.Delega> Deleghe ) : IRequest<RESULT.Deleghe.RevocaDelegheResult>;

#endregion
