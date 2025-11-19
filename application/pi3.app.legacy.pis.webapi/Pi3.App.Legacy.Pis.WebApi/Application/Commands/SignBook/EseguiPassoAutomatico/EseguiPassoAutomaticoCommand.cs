// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.LibroFirma;
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.EseguiPassoAutomatico
{
    public class EseguiPassoAutomaticoCommand : IRequest<EseguiPassoAutomaticoCommandResponse>
    {
        public long idIstanzaProcessoFirma { get; set; }
    }

    public class EseguiPassoAutomaticoCommandResponse : MessageResponse
    {
    }
}
