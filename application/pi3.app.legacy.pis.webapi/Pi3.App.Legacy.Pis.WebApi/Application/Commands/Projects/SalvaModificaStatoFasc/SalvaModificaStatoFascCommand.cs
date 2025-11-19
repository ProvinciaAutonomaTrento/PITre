// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.DiagrammaStato;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SalvaModificaStatoFasc
{
    public class SalvaModificaStatoFascCommand : IRequest<SalvaModificaStatoFascCommandResponse>
    {
        public string IdProject { get; set; }
        public string IdStato { get; set; }
        public DiagrammaStato Diagramma { get; set; }
        public string IdUtente { get; set; }
        public InfoUtente User { get; set; }
        public string DataScadenza { get; set; }
    }
    public class SalvaModificaStatoFascCommandResponse
    {

    }
}
