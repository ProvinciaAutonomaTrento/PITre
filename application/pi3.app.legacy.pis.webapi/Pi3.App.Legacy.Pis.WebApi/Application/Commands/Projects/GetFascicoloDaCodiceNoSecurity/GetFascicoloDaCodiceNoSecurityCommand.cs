// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetFascicoloDaCodiceNoSecurity
{
    public class GetFascicoloDaCodiceNoSecurityCommand : IRequest<GetFascicoloDaCodiceNoSecurityCommandResponse>
    {
        public string CodiceFasc { get; set; }
        public string IdAmm { get; set; }
        public string Titolari { get; set; }
        public bool SoloGenerali { get; set; }
    }

    public class GetFascicoloDaCodiceNoSecurityCommandResponse
    {
        public DocsPaVO.fascicolazione.Fascicolo[] Output { get; set; }
    }
}
