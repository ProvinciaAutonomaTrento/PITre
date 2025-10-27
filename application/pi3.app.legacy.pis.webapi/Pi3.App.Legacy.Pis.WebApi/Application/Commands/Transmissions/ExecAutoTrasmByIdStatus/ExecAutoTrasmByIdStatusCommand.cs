// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecAutoTrasmByIdStatus
{
    public class ExecAutoTrasmByIdStatusCommand : IRequest<ExecAutoTrasmByIdStatusCommandResponse>
    {
        public InfoUtente InfoUt { get; set; }
        public DocsPaVO.documento.SchedaDocumento Doc { get; set; }
        public DocsPaVO.DiagrammaStato.Stato Stato { get; set; }
        public string IdTemplate { get; set; }

    }

    public class ExecAutoTrasmByIdStatusCommandResponse
    {
    }
}
