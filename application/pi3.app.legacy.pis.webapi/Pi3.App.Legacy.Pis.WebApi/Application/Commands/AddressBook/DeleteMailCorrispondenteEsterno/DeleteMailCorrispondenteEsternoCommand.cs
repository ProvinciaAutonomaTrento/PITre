// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.DeleteMailCorrispondenteEsterno
{
    public class DeleteMailCorrispondenteEsternoCommand : IRequest<DeleteMailCorrispondenteEsternoCommandResponse>
    {
        public string IdCorrispondente { get; set; }
    }

    public class DeleteMailCorrispondenteEsternoCommandResponse
    {
        public bool Output { get; set; }
    }
}
