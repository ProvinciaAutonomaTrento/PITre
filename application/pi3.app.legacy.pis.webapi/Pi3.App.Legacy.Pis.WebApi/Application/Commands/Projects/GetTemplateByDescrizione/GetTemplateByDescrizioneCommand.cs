// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateByDescrizione
{
    public class GetTemplateByDescrizioneCommand : IRequest<GetTemplateByDescrizioneCommandResponse>
    {
        public string Desc { get; set; }
    }

    public class GetTemplateByDescrizioneCommandResponse
    {
        public DocsPaVO.ProfilazioneDinamica.Templates Output { get; set; }
    }
}
