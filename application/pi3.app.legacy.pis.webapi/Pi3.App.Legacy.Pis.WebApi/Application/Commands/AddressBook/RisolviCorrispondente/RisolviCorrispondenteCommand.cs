// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.RisolviCorrispondente
{
    public class RisolviCorrispondenteCommand :IRequest<RisolviCorrispondenteCommandResponse>
    {
        public string SearchKey { get; set; }
        public InfoUtente InfoUtente{ get; set; }
    }

    public class RisolviCorrispondenteCommandResponse
    {
        public Corrispondente Corrispondente { get; set; }
    }



}
