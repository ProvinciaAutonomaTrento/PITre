// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetTitolario2
{
    public class FascicolazioneGetTitolario2Command : IRequest<FascicolazioneGetTitolario2CommandResponse>
    {
        public string IdAmministrazione { get; set; }
        public string IdGruppo { get; set; }
        public string IdPeople { get; set; }
        public DocsPaVO.utente.Registro Registro { get; set; }
        public string CodiceClassifica { get; set; }
        public bool GetFigli { get; set; }
        public string IdTitolario { get; set; }
    }

    public class FascicolazioneGetTitolario2CommandResponse
    {
        public DocsPaVO.fascicolazione.Classificazione[] Output { get; set; }
    }

}
