// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.LibroFirma;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    //public record DocumentoGetDettaglioDocumentoResult(DocsPaVO.documento.SchedaDocumento output);

    //public record DocumentoGetDettaglioDocumento(DocsPaVO.utente.InfoUtente infoutente, string idProfile, string docNumber) : IRequest<DocumentoGetDettaglioDocumentoResult>;
    //public record VerificaACLResult(int output);

    //public record VerificaACL(string tipoObj, string idObj, DocsPaVO.utente.InfoUtente infoUtente, out string errorMessage) : IRequest<VerificaACLResult>;
    public record SetSignTypePreferenceResult(bool output);

    public record SetSignTypePreference(string idPeople, string chaPreference) : IRequest<SetSignTypePreferenceResult>;
    public record RejectElementsSignatureProcessResult();

    public record RejectElementsSignatureProcess(List<DocsPaVO.LibroFirma.ElementoInLibroFirma> elements, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<RejectElementsSignatureProcessResult>;
    public record AggiornaStatoElementiInLibroFirmaResult(bool output, string message);

    public record AggiornaStatoElementiInLibroFirma(List<DocsPaVO.LibroFirma.ElementoInLibroFirma> elementi, string nuovoStato, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<AggiornaStatoElementiInLibroFirmaResult>;
    //public record UtenteGetRegistriWithRfResult(ArrayList output);

    //public record UtenteGetRegistriWithRf(string idCorrGlobali, string all, string idAooColl) : IRequest<UtenteGetRegistriWithRfResult>;
    //public record CountElementiInLibroFirmaResult(int output);

    //public record CountElementiInLibroFirma(DocsPaVO.utente.InfoUtente infoUtente) : IRequest<CountElementiInLibroFirmaResult>;
    public record GetSignTypePreferenceResult(string output);

    public record GetSignTypePreference(string idPeople) : IRequest<GetSignTypePreferenceResult>;
    //public record getFirstDayOfWeekResult(string output);

    //public record getFirstDayOfWeek() : IRequest<getFirstDayOfWeekResult>;
    //public record getLastDayOfWeekResult(string output);

    //public record getLastDayOfWeek() : IRequest<getLastDayOfWeekResult>;
    public record GetElementiInLibroFirmaByDestinatarioResult(string[] output);

    public record GetElementiInLibroFirmaByDestinatario(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetElementiInLibroFirmaByDestinatarioResult>;
    public record GetElementiLibroFirmaResult(DocsPaVO.LibroFirma.ElementoInLibroFirma[] output);

    public record GetElementiLibroFirma(DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetElementiLibroFirmaResult>;

    public record InterruzioneProcessoFirmaResult();
    public record InterruzioneProcessoFirma(string docnumber, string noteInterruzione, string interrottoDa, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<InterruzioneProcessoFirmaResult>;
}
