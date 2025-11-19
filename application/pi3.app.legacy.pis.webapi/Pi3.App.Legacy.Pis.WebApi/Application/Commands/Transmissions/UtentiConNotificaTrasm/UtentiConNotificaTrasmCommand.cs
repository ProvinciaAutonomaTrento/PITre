// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using DocsPaVO.Modelli_Trasmissioni;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.UtentiConNotificaTrasm
{
    public class UtentiConNotificaTrasmCommand : IRequest<UtentiConNotificaTrasmCommandResponse>
    {
        public ModelloTrasmissione ObjModTrasm { get; set; }
        public object[] UtentiDaInserire { get; set; }
        public object[] UtentiDaCancellare { get; set; }
        public string Operazione { get; set; }
    }

    public class UtentiConNotificaTrasmCommandResponse
    {
        public ModelloTrasmissione Output { get; set; }
    }
}
