// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SetDataVistaSP
{
    public class SetDataVistaSPCommand : IRequest<SetDataVistaSPCommandResponse>
    {
        public InfoUtente InfoUtente { get; set; }
        public string DocNumber { get; set; }
        public string DocOrFasc { get; set; }
    }

    public class SetDataVistaSPCommandResponse
    {
        public bool Output { get; set; }
    }
}
