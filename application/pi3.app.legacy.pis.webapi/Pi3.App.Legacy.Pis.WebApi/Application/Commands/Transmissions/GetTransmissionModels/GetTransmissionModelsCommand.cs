// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
  using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetTransmissionModels
{
    public class GetTransmissionModelsCommand : IRequest<GetTransmissionModelsCommandResponse>
    {
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Registri sui quali sono disponibili i modelli
        /// </summary>
        public Register[] Registers
        {
            get;
            set;
        }
    }

    public class GetTransmissionModelsCommandResponse
    {
        public TransmissionModel[] TransmissionModels { get; set; }
        public string ErrorMessage { get; set; }
        public GetTransmModelsResponseCode Code { get; set; }
    }
    public enum GetTransmModelsResponseCode { OK, SYSTEM_ERROR }
}
