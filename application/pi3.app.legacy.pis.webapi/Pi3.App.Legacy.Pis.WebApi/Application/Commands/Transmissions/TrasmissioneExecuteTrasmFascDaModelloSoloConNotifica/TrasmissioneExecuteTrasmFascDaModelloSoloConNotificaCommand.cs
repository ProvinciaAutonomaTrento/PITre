// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.TrasmissioneExecuteTrasmFascDaModelloSoloConNotifica
{
    public class TrasmissioneExecuteTrasmFascDaModelloSoloConNotificaCommand :IRequest<TrasmissioneExecuteTrasmFascDaModelloSoloConNotificaCommandResponse>
    {
        public DocsPaVO.fascicolazione.Fascicolo Fascicolo { get; set; }
        public DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione Modello { get; set; }
        public DocsPaVO.utente.InfoUtente InfoUtente { get; set; }
    }

    public class TrasmissioneExecuteTrasmFascDaModelloSoloConNotificaCommandResponse
    {
        public bool Output { get; set; }
    }
}
