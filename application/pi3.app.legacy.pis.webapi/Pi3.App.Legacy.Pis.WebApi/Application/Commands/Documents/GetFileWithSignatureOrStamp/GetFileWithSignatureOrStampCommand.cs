// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFileWithSignatureOrStamp
{
    public class GetFileWithSignatureOrStampCommand: IRequest<GetFileWithSignatureOrStampCommandResponse>
    {
        public string idDocument {  get; set; }

        public string signature {  get; set; }

        public string signOrStamp { get; set; }
    }

    public class GetFileWithSignatureOrStampCommandResponse
    {
        public File File { get; set; }
        public GetFileDocumentByIdResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }

    }
    public enum GetFileDocumentByIdResponseCode { OK, SYSTEM_ERROR }
}