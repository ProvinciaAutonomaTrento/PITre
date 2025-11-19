// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SendDocumentAdvanced
{
    public class SendDocumentAdvancedCommand: IRequest<SendDocumentAdvancedCommandResponse>
    {
        public string IdDocument { get; set; }

        public string Signature { get; set; }

        public string IdRegister { get; set; }

        public string CodeRegister { get; set; }

        public string SenderMail { get; set; }

        public Correspondent[] Recipients { get; set; }
    }

    public class SendDocumentAdvancedCommandResponse
    {
        public string ResultMessage { get; set; }

        public SendingResult[] SendingResults { get; set; }

        public string ErrorMessage { get; set; }
        public SendDocAdvResponseCode Code { get; set; }
    }
    public enum SendDocAdvResponseCode { OK, SYSTEM_ERROR }
}