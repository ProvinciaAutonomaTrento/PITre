// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
