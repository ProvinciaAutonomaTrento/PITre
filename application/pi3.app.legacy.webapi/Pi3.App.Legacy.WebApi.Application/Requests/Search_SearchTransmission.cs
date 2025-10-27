// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.filtri;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{

    public record getUtentiInRuoloSottopostoResult(System.Data.DataSet output);

    public record getUtentiInRuoloSottoposto(InfoUtente infoUtente, string corrispondente) : IRequest<getUtentiInRuoloSottopostoResult>;

    public record TrasmissioneGetQueryEffettuatePagingLiteWithoutTrasmUtenteResult(Trasmissione[] output, int totalPageNumber, int recordCount);

    public record TrasmissioneGetQueryEffettuatePagingLiteWithoutTrasmUtente(OggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber, bool excel, int pageSize) : IRequest<TrasmissioneGetQueryEffettuatePagingLiteWithoutTrasmUtenteResult>;
    public record TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteResult(Trasmissione[] output, int totalPageNumber, int recordCount);

    public record TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtente(OggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber, bool excel, int pageSize) : IRequest<TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteResult>;
    
    public record TrasmissioneGetQueryEffettuatePagingLiteResult(Trasmissione[] output, int totalPageNumber, int recordCount);

    public record TrasmissioneGetQueryEffettuatePagingLite(OggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber, bool excel, int pageSize) : IRequest<TrasmissioneGetQueryEffettuatePagingLiteResult>;
    public record trasmissioniSendSollecito_newWAResult(bool output);

    public record trasmissioniSendSollecito_newWA(string path, Trasmissione trasm) : IRequest<trasmissioniSendSollecito_newWAResult>;
}
