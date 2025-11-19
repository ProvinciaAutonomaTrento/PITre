// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
  using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmissionProject
{
    public class ExecuteTransmissionProjectCommand : IRequest<ExecuteTransmissionProjectCommandResponse>
    {
        /// <summary>
        /// Id del fascicolo da trasmettere
        /// </summary>
        public string IdProject
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
        /// destinatario
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

    public class ExecuteTransmissionProjectCommandResponse: TransmissionResponse
    {
    }
}
