// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.GetTemplateFascById
{
    public class GetTemplateFascByIdCommand : IRequest<GetTemplateFascByIdCommandResponse>
    {
        public string IdTemplate { get; set; }
    }

    public class GetTemplateFascByIdCommandResponse
    {
        public DocsPaVO.ProfilazioneDinamica.Templates Output { get; set; }
    }
}
