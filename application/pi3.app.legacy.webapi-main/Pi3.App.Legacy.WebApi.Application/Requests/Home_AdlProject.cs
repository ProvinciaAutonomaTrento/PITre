// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Grids;
using DocsPaVO.ricerche;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    //public record FascicolazioneGetFascicoloByIdResult(DocsPaVO.fascicolazione.Fascicolo output);

    //public record FascicolazioneGetFascicoloById(string idFascicolo, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<FascicolazioneGetFascicoloByIdResult>;
    //public record getTemplateFascDettagliResult(DocsPaVO.ProfilazioneDinamica.Templates output);

    //public record getTemplateFascDettagli(string idProject) : IRequest<getTemplateFascDettagliResult>;
    //public record DocumentoCancellaAreaLavoroResult(bool output);

    //public record DocumentoCancellaAreaLavoro(string idPeople, string idRuoloInUO, string idProfile, DocsPaVO.fascicolazione.Fascicolo fasc) : IRequest<DocumentoCancellaAreaLavoroResult>;
    //public record DocumentoExecAddLavoroRoleResult(bool output);

    //public record DocumentoExecAddLavoroRole(string idProfile, string tipoProto, DocsPaVO.fascicolazione.Fascicolo fasc, DocsPaVO.utente.InfoUtente infoUtente, string idRegistro) : IRequest<DocumentoExecAddLavoroRoleResult>;
    //public record DocumentoCancellaAreaLavoroResult(bool output);

    //public record DocumentoCancellaAreaLavoro(string idPeople, string idRuoloInUO, string idProfile, DocsPaVO.fascicolazione.Fascicolo fasc) : IRequest<DocumentoCancellaAreaLavoroResult>;
    //public record DocumentoExecAddLavoroRoleResult(bool output);

    //public record DocumentoExecAddLavoroRole(string idProfile, string tipoProto, DocsPaVO.fascicolazione.Fascicolo fasc, DocsPaVO.utente.InfoUtente infoUtente, string idRegistro) : IRequest<DocumentoExecAddLavoroRoleResult>;
    //public record UtenteGetRegistriWithRfResult(ArrayList output);

    //public record UtenteGetRegistriWithRf(string idCorrGlobali, string all, string idAooColl) : IRequest<UtenteGetRegistriWithRfResult>;
    //public record GetStandardGridForUserResult(DocsPaVO.Grid.Grid output);

    //public record GetStandardGridForUser(InfoUtente userInfo, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType) : IRequest<GetStandardGridForUserResult>;
    //public record GetUserGridCustomResult(DocsPaVO.Grid.Grid output);

    //public record GetUserGridCustom(InfoUtente userInfo, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType) : IRequest<GetUserGridCustomResult>;
    //public record getTitolariUtilizzabiliResult(ArrayList output);

    //public record getTitolariUtilizzabili(string idAmministrazione) : IRequest<getTitolariUtilizzabiliResult>;
    public record FascicolazioneGetListaFascicoliPagingCustomResult(SearchObject[] output, int numTotPage, int nRec, List<SearchResultInfo> idProjectList);

    public record FascicolazioneGetListaFascicoliPagingCustom(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.utente.Registro registro, DocsPaVO.filtri.FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool enableProfilazione, bool childs, int numPage, int pageSize, bool getSystemIdList, byte[] excelDati, bool showGridPersonalization, bool export, DocsPaVO.Grid.Field[] visibleFieldsTemplate, String[] documentsSystemId, bool security) : IRequest<FascicolazioneGetListaFascicoliPagingCustomResult>;
    //public record getSegnAmmResult(string output);

    //public record getSegnAmm(string idAmm) : IRequest<getSegnAmmResult>;
    public record getDocumentiInFascicoloResult(List<SearchResultInfo> output);

    public record getDocumentiInFascicolo(string idProject) : IRequest<getDocumentiInFascicoloResult>;
    //public record DocumentoGetDettaglioDocumentoResult(DocsPaVO.documento.SchedaDocumento output);

    //public record DocumentoGetDettaglioDocumento(DocsPaVO.utente.InfoUtente infoutente, string idProfile, string docNumber) : IRequest<DocumentoGetDettaglioDocumentoResult>;
    //public record CanDeleteFromItemConsResult(int output);

    //public record CanDeleteFromItemCons(string idProfile, string idPeople, string idGruppo) : IRequest<CanDeleteFromItemConsResult>;
    //public record DocumentoCancellaAreaConservazioneResult(bool output);

    //public record DocumentoCancellaAreaConservazione(string idProfile, DocsPaVO.fascicolazione.Fascicolo fasc, string idIstanza, bool deleteIstanza, string systemId) : IRequest<DocumentoCancellaAreaConservazioneResult>;
    //public record getDimensioneMassimaIstanze_ByteResult(int output);

    //public record getDimensioneMassimaIstanze_Byte(string idAmm) : IRequest<getDimensioneMassimaIstanze_ByteResult>;
    //public record getDimensioneMassimaIstanze_NumDocResult(int output);

    //public record getDimensioneMassimaIstanze_NumDoc(string idAmm) : IRequest<getDimensioneMassimaIstanze_NumDocResult>;
    //public record getPercentualeTolleranzaDinesioneIstanzeResult(int output);

    //public record getPercentualeTolleranzaDinesioneIstanze(string idAmm) : IRequest<getPercentualeTolleranzaDinesioneIstanzeResult>;
    public record DocumentoExecAddConservazioneResult(string output);

    public record DocumentoExecAddConservazione(string idProfile, string idProject, string docNumber, DocsPaVO.utente.InfoUtente infoUtente, string tipoOggetto) : IRequest<DocumentoExecAddConservazioneResult>;
    //public record SerializeSchedaResult(int output);

    //public record SerializeScheda(DocsPaVO.documento.SchedaDocumento schDoc, string systemID) : IRequest<SerializeSchedaResult>;
    //public record UpdateSizeItemConsResult(bool output);

    //public record UpdateSizeItemCons(string sysId, int size) : IRequest<UpdateSizeItemConsResult>;
    //public record updateItemsConsResult(bool output);

    //public record updateItemsCons(string tipoFile, string numAllegati, string systemId) : IRequest<updateItemsConsResult>;
    //public record SerializeSchedaDocResult(int output);

    //public record SerializeSchedaDoc(DocsPaVO.documento.SchedaDocumento schDoc) : IRequest<SerializeSchedaDocResult>;
    //public record isVincoloNumeroDocumentiIstanzaViolatoResult(bool output);

    //public record isVincoloNumeroDocumentiIstanzaViolato(int numDocInIstanza, int vincoloNumDocInStanza) : IRequest<isVincoloNumeroDocumentiIstanzaViolatoResult>;
    //public record isVincoloDimensioneIstanzaViolatoResult(bool output);

    //public record isVincoloDimensioneIstanzaViolato(int DimensioneInIstanza, int vincoloDimensioneInIStanza, int percentualeTolleranza) : IRequest<isVincoloDimensioneIstanzaViolatoResult>;
    //public record DocumentoExecAddConservazione_WithConstraintsResult(string output);

    //public record DocumentoExecAddConservazione_WithConstraints(string idProfile, string idProject, string docNumber, DocsPaVO.utente.InfoUtente infoUtente, string tipoOggetto, bool numDocIstanzaViolato, bool dimIstanzaViolato, int vincoloDimIstanza, int vincoloNumDocIstanza, int sizeItem) : IRequest<DocumentoExecAddConservazione_WithConstraintsResult>;
    //public record VerificaACLResult(int output);

    //public record VerificaACL(string tipoObj, string idObj, DocsPaVO.utente.InfoUtente infoUtente, out string errorMessage) : IRequest<VerificaACLResult>;

}
