// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetFunzioniRuolo
{
    public class GetFunzioniRuoloCommand : IRequest<GetFunzioniRuoloCommandResponse>
    {
        public string IdCorrGlobali { get; set; }
    }

    public class GetFunzioniRuoloCommandResponse
    {
        public Funzione[] Funzioni { get; set; }
    }
}
