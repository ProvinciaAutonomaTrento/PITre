// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetStampAndSignature
{
    public class GetStampAndSignatureCommand: IRequest<GetStampAndSignatureCommandResponse>
    {        
        public string idDocument { get; set; }
        public string signature {  get; set; }
    }

    public class GetStampAndSignatureCommandResponse
    {
        public Stamp Stamp { get; set; }
        public string ErrorMessage { get; set; }
        public GetStampResponseCode Code { get; set; }
    }
    public enum GetStampResponseCode { OK, SYSTEM_ERROR }
}