// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SalvaDataScadenzaDoc
{
    public class SalvaDataScadenzaDocCommand : IRequest<SalvaDataScadenzaDocCommandResponse>
    {
        public string DocNumber{ get; set; }
        public string DataScadenza{ get; set; }
        public string IdTipoAtto{ get; set; }
    }

    public class SalvaDataScadenzaDocCommandResponse
    {

    }
}
