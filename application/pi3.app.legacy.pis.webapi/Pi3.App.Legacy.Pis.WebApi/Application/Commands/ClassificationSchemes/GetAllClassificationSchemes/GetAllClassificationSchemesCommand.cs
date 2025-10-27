// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetAllClassificationSchemes
{
    public class GetAllClassificationSchemesCommand : IRequest<GetAllClassificationSchemesCommandResponse>
    {

    }

    public class GetAllClassificationSchemesCommandResponse
    {
        public ClassificationScheme[] ClassificationSchemes { get; set; }
        public string ErrorMessage { get; set; }
        public GetAllClassificationSchemesCommandResponseCode Code { get; set; }
    }

    public enum GetAllClassificationSchemesCommandResponseCode { OK, SYSTEM_ERROR }

}
