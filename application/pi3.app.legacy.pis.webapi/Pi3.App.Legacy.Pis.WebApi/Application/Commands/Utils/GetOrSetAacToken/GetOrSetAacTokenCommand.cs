// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils.GetOrSetAacToken
{
    public class GetOrSetAacTokenCommand : IRequest<GetOrSetAacTokenCommandResponse>
    {
    }

    public class GetOrSetAacTokenCommandResponse
    {
        public string Token { get; set; }
    }

}
