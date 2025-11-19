// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils.Date.GetLastSevenDay
{
    public class GetLastSevenDayCommand : IRequest<GetLastSevenDayCommandResponse>
    {
    }

    public class GetLastSevenDayCommandResponse
    {
        public string output { get; set; }
    }
}
