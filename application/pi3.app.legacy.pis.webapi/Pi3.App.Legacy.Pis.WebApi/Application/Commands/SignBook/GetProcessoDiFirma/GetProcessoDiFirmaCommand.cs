// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetProcessoDiFirma
{
    public class GetProcessoDiFirmaCommand : IRequest<GetProcessoDiFirmaCommandResponse>
    {
        public string IdProcesso{ get; set; }
        public InfoUtente InfoUtente { get; set; }
    }

    public class GetProcessoDiFirmaCommandResponse
    {
        public ProcessoFirma Output { get; set; }
    }
}
