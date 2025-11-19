// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.WebApi.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{

    public record GetSwitchServices() : IRequest<GetSwitchServicesResult>;

    public record GetSwitchServicesResult(IReadOnlyList<SwitchServiceModel> output);
}
