// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later

  using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmPrjModel
{
    public class ExecuteTransmPrjModelCommand : IRequest<ExecuteTransmPrjModelCommandResponse>
    {
        /// <summary>
        /// I del modello di trasmissione
        /// </summary>
        public string IdModel
        {
            get;
            set;
        }

        /// <summary>
        /// Id del fascciolo
        /// </summary>
        public string IdProject
        {
            get;
            set;
        }
    }

    public class ExecuteTransmPrjModelCommandResponse: TransmissionResponse
    {
    }
}
