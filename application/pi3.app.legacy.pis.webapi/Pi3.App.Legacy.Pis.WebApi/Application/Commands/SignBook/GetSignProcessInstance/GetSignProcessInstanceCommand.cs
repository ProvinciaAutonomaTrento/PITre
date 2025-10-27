// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignProcessInstance
{
    public class GetSignProcessInstanceCommand: IRequest<GetSignProcessInstanceCommandResponse>
    {        
        public string idProcessInstance {  get; set; }
    }

    public class GetSignProcessInstanceCommandResponse
    {
        public SignatureProcessInstance ProcessInstance { get; set; }
        public string ErrorMessage { get; set; }
        public GetSignProcessInstanceResponseCode Code { get; set; }
    }
    public enum GetSignProcessInstanceResponseCode { OK, SYSTEM_ERROR }
}