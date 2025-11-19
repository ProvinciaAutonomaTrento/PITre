// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using DocsPaVO.fascicolazione;
using DocsPaVO.utente;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetListaFascicoliDaCodice
{
    public class FascicolazioneGetListaFascicoliDaCodiceCommand : IRequest<FascicolazioneGetListaFascicoliDaCodiceCommandResponse>
    {
        public InfoUtente InfoUtente { get; set; }
        public string CodiceFascicolo { get; set; }
        public Registro Registro { get; set; }
        public bool EnableUffRef { get; set; }
        public bool EnableProfilazione { get; set; }
        public string InsRic { get; set; }
    }


    public class FascicolazioneGetListaFascicoliDaCodiceCommandResponse
    {
        public Fascicolo[] Output { get; set; }
    }
}
