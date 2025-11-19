// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
    //public record CanDeleteFromItemConsResult(int output);

    //public record CanDeleteFromItemCons(string idProfile, string idPeople, string idGruppo) : IRequest<CanDeleteFromItemConsResult>;
    //public record DocumentoCancellaAreaConservazioneResult(bool output);

    //public record DocumentoCancellaAreaConservazione(string idProfile, DocsPaVO.fascicolazione.Fascicolo fasc, string idIstanza, bool deleteIstanza, string systemId) : IRequest<DocumentoCancellaAreaConservazioneResult>;
    //public record DocumentoIsPrimaIstanzaConsResult(int output);

    //public record DocumentoIsPrimaIstanzaCons(string idPeople, string idGruppo) : IRequest<DocumentoIsPrimaIstanzaConsResult>;
    //public record getDimensioneMassimaIstanze_ByteResult(int output);

    //public record getDimensioneMassimaIstanze_Byte(string idAmm) : IRequest<getDimensioneMassimaIstanze_ByteResult>;
    //public record getDimensioneMassimaIstanze_NumDocResult(int output);

    //public record getDimensioneMassimaIstanze_NumDoc(string idAmm) : IRequest<getDimensioneMassimaIstanze_NumDocResult>;
    //public record getPercentualeTolleranzaDinesioneIstanzeResult(int output);

    //public record getPercentualeTolleranzaDinesioneIstanze(string idAmm) : IRequest<getPercentualeTolleranzaDinesioneIstanzeResult>;
    //public record SerializeSchedaDocResult(int output);

    //public record SerializeSchedaDoc(DocsPaVO.documento.SchedaDocumento schDoc) : IRequest<SerializeSchedaDocResult>;
    //public record isVincoloNumeroDocumentiIstanzaViolatoResult(bool output);

    //public record isVincoloNumeroDocumentiIstanzaViolato(int numDocInIstanza, int vincoloNumDocInStanza) : IRequest<isVincoloNumeroDocumentiIstanzaViolatoResult>;
    //public record isVincoloDimensioneIstanzaViolatoResult(bool output);

    //public record isVincoloDimensioneIstanzaViolato(int DimensioneInIstanza, int vincoloDimensioneInIStanza, int percentualeTolleranza) : IRequest<isVincoloDimensioneIstanzaViolatoResult>;
    //public record DocumentoExecAddConservazione_WithConstraintsResult(string output);

    //public record DocumentoExecAddConservazione_WithConstraints(string idProfile, string idProject, string docNumber, DocsPaVO.utente.InfoUtente infoUtente, string tipoOggetto, bool numDocIstanzaViolato, bool dimIstanzaViolato, int vincoloDimIstanza, int vincoloNumDocIstanza, int sizeItem) : IRequest<DocumentoExecAddConservazione_WithConstraintsResult>;
    //public record SerializeSchedaResult(int output);

    //public record SerializeScheda(DocsPaVO.documento.SchedaDocumento schDoc, string systemID) : IRequest<SerializeSchedaResult>;
    //public record UpdateSizeItemConsResult(bool output);

    //public record UpdateSizeItemCons(string sysId, int size) : IRequest<UpdateSizeItemConsResult>;
    //public record updateItemsConsResult(bool output);

    //public record updateItemsCons(string tipoFile, string numAllegati, string systemId) : IRequest<updateItemsConsResult>;
    //public record DocumentoCancellaAreaLavoroResult(bool output);

    //public record DocumentoCancellaAreaLavoro(string idPeople, string idRuoloInUO, string idProfile, DocsPaVO.fascicolazione.Fascicolo fasc) : IRequest<DocumentoCancellaAreaLavoroResult>;
    //public record DocumentoExecAddLavoroRoleResult(bool output);

    //public record DocumentoExecAddLavoroRole(string idProfile, string tipoProto, DocsPaVO.fascicolazione.Fascicolo fasc, DocsPaVO.utente.InfoUtente infoUtente, string idRegistro) : IRequest<DocumentoExecAddLavoroRoleResult>;
    //public record DocumentoCancellaAreaLavoroResult(bool output);

    //public record DocumentoCancellaAreaLavoro(string idPeople, string idRuoloInUO, string idProfile, DocsPaVO.fascicolazione.Fascicolo fasc) : IRequest<DocumentoCancellaAreaLavoroResult>;
    //public record DocumentoExecAddLavoroRoleResult(bool output);

    //public record DocumentoExecAddLavoroRole(string idProfile, string tipoProto, DocsPaVO.fascicolazione.Fascicolo fasc, DocsPaVO.utente.InfoUtente infoUtente, string idRegistro) : IRequest<DocumentoExecAddLavoroRoleResult>;
    //public record getEtichetteDocumentiResult(DocsPaVO.documento.EtichettaInfo[] output);

    //public record getEtichetteDocumenti(DocsPaVO.utente.InfoUtente infoUtente, string idAmm) : IRequest<getEtichetteDocumentiResult>;
    //public record GetStandardGridForUserResult(DocsPaVO.Grid.Grid output);

    //public record GetStandardGridForUser(InfoUtente userInfo, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType) : IRequest<GetStandardGridForUserResult>;
    //public record GetUserGridCustomResult(DocsPaVO.Grid.Grid output);

    //public record GetUserGridCustom(InfoUtente userInfo, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType) : IRequest<GetUserGridCustomResult>;
    //public record DocumentoGetQueryDocumentoPagingCustomResult(ArrayList output);

    //public record DocumentoGetQueryDocumentoPagingCustom(InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] queryList, int numPage, bool security, int pageSize, out int numTotPage, out int nRec, bool getIdProfilesList, bool gridPersonalization, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, out List<SearchResultInfo> idProfileList) : IRequest<DocumentoGetQueryDocumentoPagingCustomResult>;
    //public record getSegnAmmResult(string output);

    //public record getSegnAmm(string idAmm) : IRequest<getSegnAmmResult>;
    //public record VerificaACLResult(int output);

    //public record VerificaACL(string tipoObj, string idObj, DocsPaVO.utente.InfoUtente infoUtente, out string errorMessage) : IRequest<VerificaACLResult>;

}
