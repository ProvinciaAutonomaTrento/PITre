// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFileDocumentById
{
    public class GetFileDocumentByIdCommand: IRequest<GetFileDocumentByIdCommandResponse>
    {
        public string IdDocument { get; set; }

        public string VersionId { get; set; }
    }

    public class GetFileDocumentByIdCommandResponse
    {
        public File File { get; set; }
        public GetFileDocumentByIdResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }

    }
    public enum GetFileDocumentByIdResponseCode { OK, SYSTEM_ERROR }
}