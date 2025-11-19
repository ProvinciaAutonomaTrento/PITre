// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.AddressBook
{
    public record CorrispondentiDeleteModifyCorrispondenteEsternoResult(bool output, string message, string newIdCorrGlobali);

    public record CorrispondentiDeleteModifyCorrispondenteEsterno(DocsPaVO.utente.InfoUtente infoutente, DocsPaVO.utente.DatiModificaCorr datiModificaCorr, int flagListe, string action) : IRequest<CorrispondentiDeleteModifyCorrispondenteEsternoResult>;
}
