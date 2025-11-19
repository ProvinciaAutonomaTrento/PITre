// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.filtri;
using DocsPaVO.ProfilazioneDinamica;
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
    public record DO_getIdProfileByDataResult(int output, string inArchivio);

    public record DO_getIdProfileByData (string numProto,   string AnnoProto,  string idRegistro, InfoUtente  infoUtente)																							
        : IRequest<DO_getIdProfileByDataResult>;																																									
    public record DocumentoGetQueryDocumentoPagingResult(InfoDocumento[] output,  int numTotPage, int nRec, SearchResultInfo[] idProfileList);

    public record DocumentoGetQueryDocumentoPaging(string idGruppo,   string idPeople, FiltroRicerca[][] queryList,  bool comingPopUp,    bool grigi,  int numPage,    bool security,   bool getIdProfilesList)					
        : IRequest<DocumentoGetQueryDocumentoPagingResult>;																																									
    public record getTemplatesArchivioDepositoResult(Templates[] output);

    public record getTemplatesArchivioDeposito(InfoUtente infoUtente, string idAmm,  bool seRepertorio)																												
        : IRequest<getTemplatesArchivioDepositoResult>;																																									

}
