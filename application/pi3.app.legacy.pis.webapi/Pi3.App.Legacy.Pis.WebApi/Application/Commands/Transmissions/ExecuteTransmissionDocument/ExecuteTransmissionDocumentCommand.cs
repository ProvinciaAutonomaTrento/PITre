// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
  using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmissionDocument
{
    public class ExecuteTransmissionDocumentCommand : IRequest<ExecuteTransmissionDocumentCommandResponse>
    {
        /// <summary>
        /// Id del documento da trasmettere
        /// </summary>
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Ragione di trasmissione
        /// </summary>
        public string TransmissionReason
        {
            get;
            set;
        }

        /// <summary>
        /// Destinatario
        /// </summary>
        public Correspondent Receiver
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se la trasmissione viene inserita nella todoList.
        /// </summary>
        public bool Notify
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del registro
        /// </summary>
        public string CodeReg
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo Trasmissione. S= Uno, T=tutti
        /// </summary>
        public string TransmissionType
        {
            get;
            set;
        }
    }

    public class ExecuteTransmissionDocumentCommandResponse: TransmissionResponse
    {
    }
}
