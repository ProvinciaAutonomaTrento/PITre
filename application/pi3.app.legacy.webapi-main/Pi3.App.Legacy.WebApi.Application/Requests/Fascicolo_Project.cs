// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.DiagrammaStato.AvanzamentoDiagramma;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.filtri;
using DocsPaVO.Grid;
using DocsPaVO.Grids;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ricerche;
using DocsPaVO.SmartClient;
using DocsPaVO.utente;
using DocsPaVO.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Grid.Grid;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record FindRealIdProject(long idProject) : IRequest<FindRealIdProjectResult>;
    public record FindRealIdProjectResult(long output);

    public record getTemplateFascByIdResult(Templates output);

    public record getTemplateFascById(string idTemplate) : IRequest<getTemplateFascByIdResult>;
    public record getTemplateFascCampiComuniByIdResult(Templates output);

    public record getTemplateFascCampiComuniById(InfoUtente infoUtente, string idTemplate) : IRequest<getTemplateFascCampiComuniByIdResult>;
    public record getAccessRightFascBySystemIDResult(string output);

    public record getAccessRightFascBySystemID(string fascSystemId, InfoUtente infoUtente) : IRequest<getAccessRightFascBySystemIDResult>;
    public record getDgByIdTipoFascResult(DiagrammaStato output);

    public record getDgByIdTipoFasc(string systemIdTipoFasc, string idAmm) : IRequest<getDgByIdTipoFascResult>;
    public record getStatoFascResult(Stato output);

    public record getStatoFasc(string idProject) : IRequest<getStatoFascResult>;

    public record GetFascicoloDaCodiceNoSecurityResult(DocsPaVO.fascicolazione.Fascicolo[] output);
    public record GetFascicoloDaCodiceNoSecurity(string codiceFasc, string idAmm, string titolari, bool soloGenerali) : IRequest<GetFascicoloDaCodiceNoSecurityResult>;
    public record GetFasiStatiDiagrammaResult(AssPhaseStatoDiagramma[] output);

    public record GetFasiStatiDiagramma(string idDiagramma, InfoUtente infoUtente) : IRequest<GetFasiStatiDiagrammaResult>;

    public record GetRapportoVersamentoFascicoloResult(string output);
    public record GetRapportoVersamentoFascicolo(string idProject, InfoUtente utente) : IRequest<GetRapportoVersamentoFascicoloResult>;

    public record getEtichetteDocumentiResult(EtichettaInfo[] output);

    public record getEtichetteDocumenti(InfoUtente infoUtente, string idAmm) : IRequest<getEtichetteDocumentiResult>;
    public record GetUserGridCustomResult(Grid output);

    public record GetUserGridCustom(InfoUtente userInfo, GridTypeEnumeration gridType) : IRequest<GetUserGridCustomResult>;
    public record GetStandardGridForUserResult(Grid output);

    public record LoadGrid(string gridId,Grid.GridTypeEnumeration gridType,InfoUtente userInfo,Ruolo role,List<string> templatesId,bool getEmergencyGrid) : IRequest<LoadGridResult>;
    public record LoadGridResult(Grid output);

    public record GetStandardGridForUser(InfoUtente userInfo, GridTypeEnumeration gridType) : IRequest<GetStandardGridForUserResult>;
    public record GetSmartClientConfigurationsPerUserResult(SmartClientConfigurations output);

    public record GetSmartClientConfigurationsPerUser(InfoUtente infoUtente) : IRequest<GetSmartClientConfigurationsPerUserResult>;
    public record salvaModificaStatoFascResult();

    public record salvaModificaStatoFasc(string idProject, string idStato, DiagrammaStato diagramma, string idUtente, InfoUtente user, string dataScadenza) : IRequest<salvaModificaStatoFascResult>;
    public record isStatoTrasmAutoFascResult(ModelloTrasmissione[] output);

    public record isStatoTrasmAutoFasc(string idAmm, string idStato, string idTipoFasc) : IRequest<isStatoTrasmAutoFascResult>;
    public record FascicolazioneCanRemoveFascicoloResult(bool output, string nFasc);

    public record FascicolazioneCanRemoveFascicolo(string project_Id) : IRequest<FascicolazioneCanRemoveFascicoloResult>;
    public record FascicolazioneDelFolderResult(bool output);

    public record FascicolazioneDelFolder(Folder folder, InfoUtente infoUtente) : IRequest<FascicolazioneDelFolderResult>;
    
    public record salvaDataScadenzaFascResult();

    public record salvaDataScadenzaFasc(string idProject, string dataScadenza, string idTipoFasc) : IRequest<salvaDataScadenzaFascResult>;
    
    public record getTipoFascFromRuoloResult(Templates[] output);

    public record getTipoFascFromRuolo(string idAmministrazione, string idRuolo, string diritti) : IRequest<getTipoFascFromRuoloResult>;
    public record FascicolazioneGetGerarchiaDaCodice2Result(Classifica[] output);

    public record FascicolazioneGetGerarchiaDaCodice2(string codiceClassificazione, Registro registro, string idAmm, string idTitolario) : IRequest<FascicolazioneGetGerarchiaDaCodice2Result>;
    public record FascicolazioneNewFascicoloResult(Fascicolo output, ResultCreazioneFascicolo resultCreazione);

    public record FascicolazioneNewFascicolo(Classificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo, bool enableUffRef) : IRequest<FascicolazioneNewFascicoloResult>;
    public record IsDocumentoClassificatoInFolderResult(bool output);

    public record IsDocumentoClassificatoInFolder(string idProfile, string idFolder) : IRequest<IsDocumentoClassificatoInFolderResult>;

    public record RiproponiStrutturaFascicoloResult(bool output);

    public record RiproponiStrutturaFascicolo(string idFascicoloOrigine, Fascicolo fascicolo, InfoUtente infoUtente) : IRequest<RiproponiStrutturaFascicoloResult>;

    public record ExistsTrasmPendenteConWorkflowFascicoloResult(bool output);

    public record ExistsTrasmPendenteConWorkflowFascicolo(string idProject, string idRuoloInUO, string idPeople, InfoUtente infoUtente) : IRequest<ExistsTrasmPendenteConWorkflowFascicoloResult>;
    public record ExistsTrasmPendenteSenzaWorkflowFascicoloResult(bool output);

    public record ExistsTrasmPendenteSenzaWorkflowFascicolo(string idProject, string idRuoloInUO, string idPeople, InfoUtente infoUtente) : IRequest<ExistsTrasmPendenteSenzaWorkflowFascicoloResult>;
    public record FascicolazioneCanRemoveFascicoloPrincipaleResult(bool output, string nDoc);

    public record FascicolazioneCanRemoveFascicoloPrincipale(string project_Id) : IRequest<FascicolazioneCanRemoveFascicoloPrincipaleResult>;
    public record FascicolazioneSetFascicoloResult(bool output, Fascicolo fascicolo);

    public record FascicolazioneSetFascicolo(InfoUtente infoUtente, Fascicolo fascicolo) : IRequest<FascicolazioneSetFascicoloResult>;
    public record FascicolazioneEliminaFascicoloResult(bool output);

    public record FascicolazioneEliminaFascicolo(Fascicolo Fascicolo, InfoUtente infoutente) : IRequest<FascicolazioneEliminaFascicoloResult>;
    public record isFascInADLRoleResult(int output);

    public record isFascInADLRole(string idProject, string idRole) : IRequest<isFascInADLRoleResult>;
    public record getSegnAmmResult(string output);

    public record getSegnAmm(string idAmm) : IRequest<getSegnAmmResult>;
    public record GetSegnaturaRepertorioByIdObjectCustomResult(string output);

    public record GetSegnaturaRepertorioByIdObjectCustom(string docnumber, string codiceAmm, string idObjectCustom) : IRequest<GetSegnaturaRepertorioByIdObjectCustomResult>;

    public record GetSegnaturaRepertorioNoHTMLByIdOggettoResult(string output);

    public record GetSegnaturaRepertorioNoHTMLByIdOggetto(string docnumber, string codiceAmm, string idObjectCustom) : IRequest<GetSegnaturaRepertorioNoHTMLByIdOggettoResult>;

    public record FascicolazioneGetDocumentiPagingWithFiltersCustomResult(SearchObject[] output, int numTotPage, int nRec, SearchResultInfo[] idProfiles);

    public record FascicolazioneGetDocumentiPagingWithFiltersCustom(InfoUtente infoUtente, Folder folder, FiltroRicerca[][] filtriRicerca, int numPage, bool compileIdProfileList, bool showGridPersonalization, bool export, Field[] visibleFieldsTemplate, string[] documentsSystemId, int pageSize,  FiltroRicerca[][] orderRicerca) : IRequest<FascicolazioneGetDocumentiPagingWithFiltersCustomResult>;
    public record DocumentoIsPrimaIstanzaConsResult(int output);

    public record DocumentoIsPrimaIstanzaCons(string idPeople, string idGruppo) : IRequest<DocumentoIsPrimaIstanzaConsResult>;
    public record getDimensioneMassimaIstanze_ByteResult(int output);

    public record getDimensioneMassimaIstanze_Byte(string idAmm) : IRequest<getDimensioneMassimaIstanze_ByteResult>;
    public record getDimensioneMassimaIstanze_NumDocResult(int output);

    public record getDimensioneMassimaIstanze_NumDoc(string idAmm) : IRequest<getDimensioneMassimaIstanze_NumDocResult>;
    public record getPercentualeTolleranzaDinesioneIstanzeResult(int output);

    public record getPercentualeTolleranzaDinesioneIstanze(string idAmm) : IRequest<getPercentualeTolleranzaDinesioneIstanzeResult>;
    public record SerializeSchedaDocResult(int output);

    public record SerializeSchedaDoc(SchedaDocumento schDoc) : IRequest<SerializeSchedaDocResult>;
    public record isVincoloNumeroDocumentiIstanzaViolatoResult(bool output);

    public record isVincoloNumeroDocumentiIstanzaViolato(int numDocInIstanza, int vincoloNumDocInStanza) : IRequest<isVincoloNumeroDocumentiIstanzaViolatoResult>;
    public record isVincoloDimensioneIstanzaViolatoResult(bool output);

    public record isVincoloDimensioneIstanzaViolato(int DimensioneInIstanza, int vincoloDimensioneInIStanza, int percentualeTolleranza) : IRequest<isVincoloDimensioneIstanzaViolatoResult>;
    public record DocumentoExecAddConservazione_WithConstraintsResult(string output);

    public record DocumentoExecAddConservazione_WithConstraints(string idProfile, string idProject, string docNumber, InfoUtente infoUtente, string tipoOggetto, bool numDocIstanzaViolato, bool dimIstanzaViolato, int vincoloDimIstanza, int vincoloNumDocIstanza, int sizeItem) : IRequest<DocumentoExecAddConservazione_WithConstraintsResult>;
    public record SerializeSchedaResult(int output);

    public record SerializeScheda(SchedaDocumento schDoc, string systemID) : IRequest<SerializeSchedaResult>;
    public record UpdateSizeItemConsResult(bool output);

    public record UpdateSizeItemCons(string sysId, int size) : IRequest<UpdateSizeItemConsResult>;
    public record updateItemsConsResult(bool output);

    public record updateItemsCons(string tipoFile, string numAllegati, string systemId) : IRequest<updateItemsConsResult>;
    public record CanDeleteFromItemConsResult(int output);

    public record CanDeleteFromItemCons(string idProfile, string idPeople, string idGruppo) : IRequest<CanDeleteFromItemConsResult>;
    public record DocumentoCancellaAreaConservazioneResult(bool output);

    public record DocumentoCancellaAreaConservazione(string idProfile, Fascicolo fasc, string idIstanza, bool deleteIstanza, string systemId) : IRequest<DocumentoCancellaAreaConservazioneResult>;
    public record FascicolazioneDeleteDocFromFolderResult(ValidationResultInfo output, string msg);

    public record FascicolazioneDeleteDocFromFolder(InfoUtente infoUtente, string idProfile, Folder folder, string fascRapida) : IRequest<FascicolazioneDeleteDocFromFolderResult>;
    public record getDirittiCampiTipologiaFascResult(AssDocFascRuoli[] output);

    public record getDirittiCampiTipologiaFasc(string idRuolo, string idTemplate) : IRequest<getDirittiCampiTipologiaFascResult>;

    public record getRootFolderFascResult(string? Output);

    public record getRootFolderFasc(string IdFasc) : IRequest<getRootFolderFascResult>;
    public record FascicolazioneGetVisibilitaSemplificataResult(DocsPaVO.fascicolazione.DirittoOggetto[] output);

    public record FascicolazioneGetVisibilitaSemplificata(InfoFascicolo infoFascicolo, bool cercaRimossi, string rootFolder) : IRequest<FascicolazioneGetVisibilitaSemplificataResult>;
    
    public record GetInfoDocumentoResult(InfoDocumento output);

    public record GetInfoDocumento(InfoUtente infoUtente, string idProfile, string docNumber) : IRequest<GetInfoDocumentoResult>;
    public record AcceptMassiveTrasmFascResult(bool output);

    public record AcceptMassiveTrasmFasc(string idProject, Ruolo ruolo, InfoUtente infoUtente) : IRequest<AcceptMassiveTrasmFascResult>;
    public record ViewMassiveTrasmFascResult(bool output);

    public record ViewMassiveTrasmFasc(string idProject, Ruolo ruolo, InfoUtente infoUtente) : IRequest<ViewMassiveTrasmFascResult>;
    public record FascicolazioneGetFascicoloByIdNoSecurityResult(Fascicolo output);

    public record FascicolazioneGetFascicoloByIdNoSecurity(string idFascicolo) : IRequest<FascicolazioneGetFascicoloByIdNoSecurityResult>;

    public record FascicoloGetListaStoricoProfilatiResult(StoricoProfilati[] output);
    public record FascicoloGetListaStoricoProfilati(string id_tipo_fasc, string idProject) : IRequest<FascicoloGetListaStoricoProfilatiResult>;
}
