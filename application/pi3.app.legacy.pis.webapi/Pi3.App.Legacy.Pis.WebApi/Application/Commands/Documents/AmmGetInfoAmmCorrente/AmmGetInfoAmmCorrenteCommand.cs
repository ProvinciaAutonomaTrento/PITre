// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.AmmGetInfoAmmCorrente
{
    public class AmmGetInfoAmmCorrenteCommand :IRequest<AmmGetInfoAmmCorrenteCommandResponse>
    {
        public string IdAmm { get; set; }
    }
    public class AmmGetInfoAmmCorrenteCommandResponse
    {
        public DocsPaVO.amministrazione.InfoAmministrazione Output { get; set; }
    }
}
