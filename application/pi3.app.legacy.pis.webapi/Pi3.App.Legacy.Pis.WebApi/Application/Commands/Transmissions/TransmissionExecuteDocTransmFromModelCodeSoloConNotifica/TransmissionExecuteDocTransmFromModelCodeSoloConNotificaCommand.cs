// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.TransmissionExecuteDocTransmFromModelCodeSoloConNotifica
{
    public class TransmissionExecuteDocTransmFromModelCodeSoloConNotificaCommand : IRequest<TransmissionExecuteDocTransmFromModelCodeSoloConNotificaCommandResponse>
    {
        public SchedaDocumento Documento { get; set; }
        public InfoUtente InfoUtente { get; set; }
        public Ruolo Role { get; set; }
        public string ModelCode { get; set; }
    }

    public class TransmissionExecuteDocTransmFromModelCodeSoloConNotificaCommandResponse
    {
        public bool Output{ get; set; }
    }
}
