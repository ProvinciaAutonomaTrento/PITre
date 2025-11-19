// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record GetTrasmissioneByIdResult(DocsPaVO.trasmissione.Trasmissione output);

    public record GetTrasmissioneById(DocsPaVO.trasmissione.OggettoTrasm oggettoTrasmesso, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo, string systemId) : IRequest<GetTrasmissioneByIdResult>;
    public record getCorrispondentiByCodListaResult(Corrispondente[] output);

    public record getCorrispondentiByCodLista(string codiceLista, string idAmm, InfoUtente infoUtente) : IRequest<getCorrispondentiByCodListaResult>;
    public record rubricaGetElementoRubricaSimpleBySystemIdResult(DocsPaVO.rubrica.ElementoRubrica output);

    public record rubricaGetElementoRubricaSimpleBySystemId(string systemId, DocsPaVO.utente.InfoUtente u) : IRequest<rubricaGetElementoRubricaSimpleBySystemIdResult>;
    public record IsEnabledRFResult(bool output);

    public record IsEnabledRF(string idAmm) : IRequest<IsEnabledRFResult>;
    public record getCorrispondentiByCodRFResult(Corrispondente[] output);

    public record getCorrispondentiByCodRF(string codiceRF) : IRequest<getCorrispondentiByCodRFResult>;

    public record getModelliPerTrasmLiteResult(ModelloTrasmissione[] output);

    public record getModelliPerTrasmLite(string idAmm, DocsPaVO.utente.Registro[] registri, string idPeople, string idCorrGlobali, string idTipoDoc, string idDiagramma, string idStato, string cha_tipo_oggetto, string system_id, string idRuoloUtente, bool AllReg, string accessrights) : IRequest<getModelliPerTrasmLiteResult>;
    public record AddressbookGetRuoliRiferimentoAutorizzatiResult(Ruolo[] output);

    public record AddressbookGetRuoliRiferimentoAutorizzati(DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca, DocsPaVO.utente.UnitaOrganizzativa uo) : IRequest<AddressbookGetRuoliRiferimentoAutorizzatiResult>;
    public record isUserDisabledResult(bool output);

    public record isUserDisabled(string username, string idAmm) : IRequest<isUserDisabledResult>;
    
    public record TrasmissioneSaveTrasmResult(DocsPaVO.trasmissione.Trasmissione output);

    public record TrasmissioneSaveTrasm(DocsPaVO.trasmissione.Trasmissione trasmissione) : IRequest<TrasmissioneSaveTrasmResult>;
    
    public record TrasmissioniDeleteTrasmissioneResult(bool output);

    public record TrasmissioniDeleteTrasmissione(DocsPaVO.trasmissione.Trasmissione objTrasm) : IRequest<TrasmissioniDeleteTrasmissioneResult>;
    public record DirittoProprietarioResult(bool output);
    public record DirittoProprietario(string idObj, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<DirittoProprietarioResult>;
}
