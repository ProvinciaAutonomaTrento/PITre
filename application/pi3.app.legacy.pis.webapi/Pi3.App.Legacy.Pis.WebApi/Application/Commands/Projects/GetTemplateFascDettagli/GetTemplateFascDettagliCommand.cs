// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProfilazioneDinamica;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateFascDettagli
{
    public class GetTemplateFascDettagliCommand : IRequest<GetTemplateFascDettagliCommandResponse>
    {
        public string IdProject { get; set; }
    }

    public class GetTemplateFascDettagliCommandResponse
    {
        public Templates Output { get; set; }
    }
}
