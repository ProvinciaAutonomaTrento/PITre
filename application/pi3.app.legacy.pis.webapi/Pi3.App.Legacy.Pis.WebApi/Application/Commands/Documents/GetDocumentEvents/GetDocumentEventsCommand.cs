// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentEvents
{
    public class GetDocumentEventsCommand: IRequest<GetDocumentEventsCommandResponse>
    {        
        public string IdDocument { get; set; }

        public bool AllEvents { get; set; }
    }

    public class GetDocumentEventsCommandResponse
    {
        public List<LogEvent> Events { get; set; }
        public GetDocumentEventsResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum GetDocumentEventsResponseCode { OK, SYSTEM_ERROR }
}