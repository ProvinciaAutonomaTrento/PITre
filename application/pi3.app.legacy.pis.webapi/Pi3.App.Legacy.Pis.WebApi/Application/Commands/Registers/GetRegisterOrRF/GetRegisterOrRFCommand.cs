// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers.GetRegisterOrRF
{
    public class GetRegisterOrRFCommand: IRequest<GetRegisterOrRFCommandResponse>
    {
        public string idRegister {  get; set; }
        public string codeRegister { get; set; }
    }

    public class GetRegisterOrRFCommandResponse
    {
        public Register Register { get; set; }
        public string ErrorMessage { get; set; }
        public GetRegisterOrRFResponseCode Code { get; set; }
    }

    public enum GetRegisterOrRFResponseCode { OK, SYSTEM_ERROR }
}
