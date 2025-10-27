// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.amministrazione;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record GetTitolarioByIdResult(OrgTitolario output);

    public record getTitolarioById(string idTitolario) : IRequest<GetTitolarioByIdResult>;

    public record GetStatoByIdResult(Stato output);

    public record GetStatoById(string idStato, InfoUtente infoUtente) : IRequest<GetStatoByIdResult>;
    public record ChangeStateSendTransmissionsResult();

    public record ChangeStateSendTransmissions(string idStato, InfoFascicolo infoFascicolo, InfoUtente infoUtente) : IRequest<ChangeStateSendTransmissionsResult>;
    public record reportFascetteFascicoloResult(FileDocumento output);

    public record reportFascetteFascicolo(Fascicolo fascicolo, InfoUtente infoUtente) : IRequest<reportFascetteFascicoloResult>;

}
