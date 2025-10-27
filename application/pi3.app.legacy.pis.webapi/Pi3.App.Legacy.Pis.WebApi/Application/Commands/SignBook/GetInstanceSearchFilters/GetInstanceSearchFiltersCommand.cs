// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetInstanceSearchFilters
{
    public class GetInstanceSearchFiltersCommand: IRequest<GetInstanceSearchFiltersCommandResponse>
    {        
    }

    public class GetInstanceSearchFiltersCommandResponse: GetFiltersResponse
    {
	}
}