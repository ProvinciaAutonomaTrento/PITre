// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SalvaDataScadenzaFasc
{
    public class SalvaDataScadenzaFascCommand : IRequest<SalvaDataScadenzaFascCommandResponse>
    {
        public string IdProject { get; set; }
        public string DataScadenza { get; set; }
        public string IdTipoFasc { get; set; }
    }

    public class SalvaDataScadenzaFascCommandResponse
    {

    }
}
