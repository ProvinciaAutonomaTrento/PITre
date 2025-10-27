// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.LibroFirma.Requests
{
    public record EseguiPassoAutomatico(string idIstanzaProcessoFirma) : IRequest<EseguiPassoAutomaticoResult>;
    public record EseguiPassoAutomaticoResult();
}
