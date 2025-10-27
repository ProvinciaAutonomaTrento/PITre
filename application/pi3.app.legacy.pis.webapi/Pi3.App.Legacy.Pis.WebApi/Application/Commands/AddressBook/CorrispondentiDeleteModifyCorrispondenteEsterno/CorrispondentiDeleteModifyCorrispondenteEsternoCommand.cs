// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.CorrispondentiDeleteModifyCorrispondenteEsterno
{
    public class CorrispondentiDeleteModifyCorrispondenteEsternoCommand : IRequest<CorrispondentiDeleteModifyCorrispondenteEsternoCommandResponse>
    {
        public DocsPaVO.utente.InfoUtente Infoutente { get; set; }
        public DocsPaVO.utente.DatiModificaCorr DatiModificaCorr { get; set; }
        public int FlagListe { get; set; }
        public string Action { get; set; }
    }

    public class CorrispondentiDeleteModifyCorrispondenteEsternoCommandResponse
    {
        public bool Output{ get; set; }
        public string Message{ get; set; }
        public string NewIdCorr { get; set; }
    }
}
