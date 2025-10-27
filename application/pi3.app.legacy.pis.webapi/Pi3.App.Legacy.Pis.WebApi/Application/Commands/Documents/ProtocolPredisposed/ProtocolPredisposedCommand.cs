// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ProtocolPredisposed
{
    public class ProtocolPredisposedCommand: IRequest<ProtocolPredisposedCommandResponse>
    {
        /// <summary>
        /// Nel caso di protocollo specificare il registro
        /// </summary>
        public string CodeRegister
        {
            get;
            set;
        }

        /// <summary>
        /// Codice dell'RF in cui si vuole protocollare (opzionale)
        /// </summary>
        public string CodeRF
        {
            get;
            set;
        }

        /// <summary>
        /// Documento che si vuole creare
        /// </summary>
        public string IdDocument
        {
            get;
            set;
        }
    }

    public class ProtocolPredisposedCommandResponse: GetDocumentResponse
    {
	}
}