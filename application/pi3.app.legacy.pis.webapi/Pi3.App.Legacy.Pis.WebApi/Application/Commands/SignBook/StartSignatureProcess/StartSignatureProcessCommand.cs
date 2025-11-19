// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.StartSignatureProcess
{
    public class StartSignatureProcessCommand: IRequest<StartSignatureProcessCommandResponse>
    {
        public SignatureProcess SignatureProcess { get; set; }

        public string IdDocument { get; set; }

        public string Note { get; set; }

        public bool InterruptionGeneratesNote { get; set; }

        public bool EndGeneratesNote { get; set; }
    }

    public class StartSignatureProcessCommandResponse: MessageResponse
    {
	}
}