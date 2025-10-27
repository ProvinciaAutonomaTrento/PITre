// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetTransmissionModel
{
    public class GetTransmissionModelCommand : IRequest<GetTransmissionModelCommandResponse>
    {
        public string idModel { get; set; }
        public string codeModel { get; set; }
    }

    public class GetTransmissionModelCommandResponse
    {
        public TransmissionModel TransmissionModel { get; set; }
        public string ErrorMessage { get; set; }
        public GetTransmModelResponseCode Code { get; set; }
    }
    public enum GetTransmModelResponseCode { OK, SYSTEM_ERROR }
}
