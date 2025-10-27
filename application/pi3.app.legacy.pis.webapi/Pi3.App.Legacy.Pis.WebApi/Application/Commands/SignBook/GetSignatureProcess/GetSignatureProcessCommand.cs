// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignatureProcess
{
    public class GetSignatureProcessCommand: IRequest<GetSignatureProcessCommandResponse>
    {        
        public string idProcess { get; set; }
    }

    public class GetSignatureProcessCommandResponse
    {
        public SignatureProcess SignatureProcess { get; set; }
        public string ErrorMessage { get; set; }
        public GetSignatureProcessResponseCode Code { get; set; }
    }
    public enum GetSignatureProcessResponseCode { OK, SYSTEM_ERROR }
}