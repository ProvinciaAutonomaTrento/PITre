// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.filtri;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record GetLastThirtyOneDayResult(string output);

    public record GetLastThirtyOneDay() : IRequest<GetLastThirtyOneDayResult>;

    public record GetIdRuoloRespConservazioneResult(string output);

    public record GetIdRuoloRespConservazione(string idAmm, string idAOO) : IRequest<GetIdRuoloRespConservazioneResult>;

    public record CancellaRicercaResult(bool output);

    public record CancellaRicerca(int id) : IRequest<CancellaRicercaResult>;
    public record ListaRicercheSalvateResult(SearchItemList output);

    public record ListaRicercheSalvate(int idPeople, int idGruppo, string pgName, bool inADL, string tipo) : IRequest<ListaRicercheSalvateResult>;
}
