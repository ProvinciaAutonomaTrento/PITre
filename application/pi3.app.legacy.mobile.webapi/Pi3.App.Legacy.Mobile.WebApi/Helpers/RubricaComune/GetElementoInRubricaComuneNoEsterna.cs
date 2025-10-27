// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.RubricaComune
{
    public record GetElementoInRubricaComuneNoEsternaResult(DocsPaVO.utente.Corrispondente output);
    public record GetElementoInRubricaComuneNoEsterna(string codice, InfoUtente u) : IRequest<GetElementoInRubricaComuneNoEsternaResult>;
}
