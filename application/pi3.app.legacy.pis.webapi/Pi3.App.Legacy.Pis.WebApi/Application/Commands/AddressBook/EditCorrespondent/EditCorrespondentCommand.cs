// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.EditCorrespondent
{
    public class EditCorrespondentCommand : IRequest<EditCorrespondentCommandResponse>
    {
        public Correspondent Correspondent { get; set; }
    }

    public class EditCorrespondentCommandResponse: GetCorrespondentResponse
    {
    }
}
