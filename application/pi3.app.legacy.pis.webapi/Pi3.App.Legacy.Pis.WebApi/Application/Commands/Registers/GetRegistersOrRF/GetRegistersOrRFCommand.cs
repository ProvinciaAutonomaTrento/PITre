// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers.GetRegistersOrRF
{
    public class GetRegistersOrRFCommand: IRequest<GetRegistersOrRFCommandResponse>
    {
        public string codeRole { get; set; }
        public string idRole { get; set; }

        public string RegOrRF { get; set; }
    }

    public class GetRegistersOrRFCommandResponse
    {
        public Register[] Registers { get; set; }
        public string ErrorMessage { get; set; }
        public GetRegistersOrRFResponseCode Code { get; set; }
    }

    public enum GetRegistersOrRFResponseCode { OK, SYSTEM_ERROR }
}
