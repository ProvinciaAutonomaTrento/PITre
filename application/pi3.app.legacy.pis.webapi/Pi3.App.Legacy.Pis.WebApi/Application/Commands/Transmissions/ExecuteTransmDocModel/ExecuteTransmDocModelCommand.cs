// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
  using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmDocModel
{
    public class ExecuteTransmDocModelCommand : IRequest<ExecuteTransmDocModelCommandResponse>
    {
        public string IdModel
        {
            get;
            set;
        }

        /// <summary>
        /// Id del documento
        /// </summary>
        public string DocumentId
        {
            get;
            set;
        }
    }

    public class ExecuteTransmDocModelCommandResponse : TransmissionResponse
    {
    }
}
