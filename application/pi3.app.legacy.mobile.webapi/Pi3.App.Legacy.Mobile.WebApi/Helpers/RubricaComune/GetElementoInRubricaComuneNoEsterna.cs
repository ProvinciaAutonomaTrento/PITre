// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.RubricaComune
{
    public record GetElementoInRubricaComuneNoEsternaResult(DocsPaVO.utente.Corrispondente output);
    public record GetElementoInRubricaComuneNoEsterna(string codice, InfoUtente u) : IRequest<GetElementoInRubricaComuneNoEsternaResult>;
}
