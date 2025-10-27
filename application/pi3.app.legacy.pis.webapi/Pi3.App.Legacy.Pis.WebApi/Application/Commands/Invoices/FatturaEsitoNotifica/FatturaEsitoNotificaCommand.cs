// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.Serialization;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Invoices.FatturaEsitoNotifica
{
    public class FatturaEsitoNotificaCommand: IRequest<FatturaEsitoNotificaCommandResponse>
    {
        /// <summary>
        /// DocNumber del documento a cui associare la notifica
        /// </summary>
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// File della notifica
        /// </summary>
        public Commands.Documents.File File
        {
            get;
            set;
        }

        /// <summary>
        /// Esito da inserire nel campo profilato.
        /// </summary>
        public string EsitoNotifica
        {
            get;
            set;
        }
    }

    public class FatturaEsitoNotificaCommandResponse : MessageResponse
    {
	}
}