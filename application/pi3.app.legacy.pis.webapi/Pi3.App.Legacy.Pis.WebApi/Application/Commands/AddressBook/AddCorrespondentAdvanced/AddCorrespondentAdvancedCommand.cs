// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddCorrespondentAdvanced
{
    public class AddCorrespondentAdvancedCommand : IRequest<AddCorrespondentAdvancedCommandResponse>
    {
        public CorrespondentAdvanced Correspondent {  get; set; }
    }

    public class AddCorrespondentAdvancedCommandResponse: CorrAdvDetailsResponse
    {
    }
}
