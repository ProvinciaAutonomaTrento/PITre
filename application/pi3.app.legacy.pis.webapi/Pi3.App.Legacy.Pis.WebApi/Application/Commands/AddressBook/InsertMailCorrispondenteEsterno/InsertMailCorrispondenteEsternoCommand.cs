// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.InsertMailCorrispondenteEsterno
{
    public class InsertMailCorrispondenteEsternoCommand : IRequest<InsertMailCorrispondenteEsternoCommandResponse>
    {
        public System.Collections.Generic.List<MailCorrispondente> ListCaselle { get; set; }
        public string IdCorrispondente { get; set; }
    }

    public class InsertMailCorrispondenteEsternoCommandResponse
    {
        public bool Output { get; set; }
    }
}
