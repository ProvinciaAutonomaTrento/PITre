// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetActiveClassificationScheme
{
    public class GetActiveClassificationSchemeCommand : IRequest<GetActiveClassificationSchemeCommandResponse>
    {
        
    }

    public class GetActiveClassificationSchemeCommandResponse
    {
        public ClassificationScheme ClassificationScheme { get; set; }
        public string ErrorMessage { get; set; }
        public GetClassificationSchemeResponseCode Code { get; set; }
    }

    public enum GetClassificationSchemeResponseCode { OK, SYSTEM_ERROR }
}
