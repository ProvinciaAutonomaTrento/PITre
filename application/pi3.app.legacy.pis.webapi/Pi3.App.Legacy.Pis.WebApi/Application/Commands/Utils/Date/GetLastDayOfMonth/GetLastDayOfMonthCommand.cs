// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils.Date.GetLastDayOfMonth
{
    public class GetLastDayOfMonthCommand : IRequest<GetLastDayOfMonthCommandResponse>
    {
    }

    public class GetLastDayOfMonthCommandResponse
    {
        public string output { get; set; }
    }
}
