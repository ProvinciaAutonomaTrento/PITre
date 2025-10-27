// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetClassificationSchemeById
{
    public class GetClassificationSchemeByIdCommand: IRequest<GetClassificationSchemeByIdCommandResponse>
    {
        public string idClassificationScheme { get; set; }
    }


    public class GetClassificationSchemeByIdCommandResponse
    {

        public ClassificationScheme ClassificationScheme { get; set; }
        public string ErrorMessage { get; set; }
        public GetClassificationSchemeResponseCode Code { get; set; }
    }

    public enum GetClassificationSchemeResponseCode { OK, SYSTEM_ERROR }


}
