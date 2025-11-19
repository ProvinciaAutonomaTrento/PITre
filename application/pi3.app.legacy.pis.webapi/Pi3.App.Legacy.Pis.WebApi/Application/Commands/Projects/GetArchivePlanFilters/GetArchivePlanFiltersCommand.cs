// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetArchivePlanFilters
{
    public class GetArchivePlanFiltersCommand: IRequest<GetArchivePlanFiltersCommandResponse>
    {        
    }

    public class GetArchivePlanFiltersCommandResponse: GetFiltersResponse
    {
	}
}