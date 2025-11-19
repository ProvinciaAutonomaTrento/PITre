// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetFiltersProjectsFromPis
{

    public class GetFiltersProjectsFromPisCommand : IRequest<GetFiltersProjectsFromPisCommandResponse>
    {
        public Filter[] Filters { get; set; }
        public DocsPaVO.utente.InfoUtente InfoUtente { get; set; }
    }
    public class GetFiltersProjectsFromPisCommandResponse
    {
        public DocsPaVO.utente.Registro Registro { get; set; }
        public DocsPaVO.fascicolazione.Classificazione ObjClassificazione { get; set; }
        public DocsPaVO.filtri.FiltroRicerca[][] Output { get; set; }
    }
}
