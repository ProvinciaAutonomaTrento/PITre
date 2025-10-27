// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetFascicoloById
{
    public class FascicolazioneGetFascicoloByIdCommand : IRequest<FascicolazioneGetFascicoloByIdCommandResponse>
    {
        public string IdFascicolo{ get; set; }
        public InfoUtente InfoUtente{ get; set; }
    }

    public class FascicolazioneGetFascicoloByIdCommandResponse
    {
        public DocsPaVO.fascicolazione.Fascicolo Output { get; set; }
    }
}
