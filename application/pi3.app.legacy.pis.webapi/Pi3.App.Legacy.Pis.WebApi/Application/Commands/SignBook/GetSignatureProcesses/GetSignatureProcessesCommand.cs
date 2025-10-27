// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignatureProcesses
{
    public class GetSignatureProcessesCommand: IRequest<GetSignatureProcessesCommandResponse>
    {        
    }

    public class GetSignatureProcessesCommandResponse
    {
        public SignatureProcess[] Processes
        {
            get;
            set;
        }

        public int TotalProcessesNumber
        {
            get;
            set;
        }

        public string ErrorMessage { get; set; }
        public GetSignatureProcessesResponseCode Code { get; set; }
    }
    public enum GetSignatureProcessesResponseCode { OK, SYSTEM_ERROR }
}