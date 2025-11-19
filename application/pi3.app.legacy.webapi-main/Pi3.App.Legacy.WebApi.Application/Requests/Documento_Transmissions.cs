// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.addressbook;
using DocsPaVO.amministrazione;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.filtri;
using DocsPaVO.LibroFirma;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ricerche;
using DocsPaVO.rubrica;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record ChiudiLavorazioneTaskResult(bool output);

    public record ChiudiLavorazioneTask(DocsPaVO.Task.Task task, string note, InfoUtente infoUtente) : IRequest<ChiudiLavorazioneTaskResult>;
    public record RiapriLavorazioneResult(bool output);

    public record RiapriLavorazione(DocsPaVO.Task.Task task, InfoUtente infoUtente, Ruolo ruolo) : IRequest<RiapriLavorazioneResult>;
    public record AnnullaTaskResult(bool output);

    public record AnnullaTask(Task task, InfoUtente infoUtente) : IRequest<AnnullaTaskResult>;
    public record ChiudiTaskResult(bool output);

    public record ChiudiTask(Task task, InfoUtente infoUtente) : IRequest<ChiudiTaskResult>;
    public record getStatoSuccessivoAutomaticoResult(Stato output);

    public record getStatoSuccessivoAutomatico(string docNumber) : IRequest<getStatoSuccessivoAutomaticoResult>;
    public record getDiagrammaByIdResult(DiagrammaStato output);

    public record getDiagrammaById(string idDiagramma) : IRequest<getDiagrammaByIdResult>;
    public record salvaModificaStatoResult();

    public record salvaModificaStato(string docNumber, string idStato, DiagrammaStato diagramma, string idUtente, InfoUtente user, string dataScadenza) : IRequest<salvaModificaStatoResult>;
    public record isEnableConversionePdfLatoServerResult(bool output);

    public record isEnableConversionePdfLatoServer() : IRequest<isEnableConversionePdfLatoServerResult>;

    public record IsEnabledInteropInternaResult(bool output);
    public record IsEnabledInteropInterna(): IRequest<IsEnabledInteropInternaResult>;
    public record AsposeServerPdfConversionResult();

    public record AsposeServerPdfConversion(byte[] content, FileRequest fileReq,	InfoUtente infoUtente)																		: IRequest<AsposeServerPdfConversionResult>;
    public record EnqueueServerPdfConversionResult();

    public record EnqueueServerPdfConversion(InfoUtente infoUtente, ObjServerPdfConversion objServerPdfConversion) : IRequest<EnqueueServerPdfConversionResult>;
    public record cambiaDirittiDocumentiResult();

    public record cambiaDirittiDocumenti(int accessRight, string idDocumento) : IRequest<cambiaDirittiDocumentiResult>;
    public record getIdTemplateResult(string output);

    public record getIdTemplate(string docNumber) : IRequest<getIdTemplateResult>;
    public record isStatoTrasmAutoResult(ModelloTrasmissione[] output);

    public record isStatoTrasmAuto(string idAmm, string idStato, string idTemplate) : IRequest<isStatoTrasmAutoResult>;
    public record getRagioneByIdResult(RagioneTrasmissione output);

    public record getRagioneById(string idRagione) : IRequest<getRagioneByIdResult>;
    public record AddressbookGetCorrispondenteByCodRubricaIEResult(Corrispondente output);

    public record AddressbookGetCorrispondenteByCodRubricaIE(string codice, TipoUtente tipoIE, InfoUtente u) : IRequest<AddressbookGetCorrispondenteByCodRubricaIEResult>;
    public record AddressbookGetCorrispondenteByIdPeopleResult(Corrispondente output);

    public record AddressbookGetCorrispondenteByIdPeople(string idPeople, TipoUtente tipoIE, InfoUtente u) : IRequest<AddressbookGetCorrispondenteByIdPeopleResult>;
    public record AddressbookGetCorrispondenteCompletoBySystemIdResult(Corrispondente output);

    public record AddressbookGetCorrispondenteCompletoBySystemId(string systemId, TipoUtente tipoIE, InfoUtente u) : IRequest<AddressbookGetCorrispondenteCompletoBySystemIdResult>;
    public record AddressbookGetRuoloRespUoFromUoResult(string output);

    public record AddressbookGetRuoloRespUoFromUo(string idCorrGlobaliUo, string tipoRuolo, string idCorr) : IRequest<AddressbookGetRuoloRespUoFromUoResult>;
    public record AddressbookGetListaCorrispondentiResult(Corrispondente[] output);

    public record AddressbookGetListaCorrispondenti(QueryCorrispondente queryCorrispondente) : IRequest<AddressbookGetListaCorrispondentiResult>;
    public record TrasmissioneSaveExecuteTrasmResult(Trasmissione output);

    public record TrasmissioneSaveExecuteTrasm(string path, Trasmissione trasmissione, InfoUtente infoUtente) : IRequest<TrasmissioneSaveExecuteTrasmResult>;
    public record salvaStoricoTrasmDiagrammiFascResult();

    public record salvaStoricoTrasmDiagrammiFasc(string idTrasm, string idProject, string idStato) : IRequest<salvaStoricoTrasmDiagrammiFascResult>;
    public record GetInfoTrasmissioniFilteredResult(InfoTrasmissione[] output, SearchPagingContext pagingContext);

    public record GetInfoTrasmissioniFiltered(InfoUtente infoUtente, string idDocOrFasc, string docOrFasc, FiltroRicerca[] objListaFiltri, SearchPagingContext pagingContext) : IRequest<GetInfoTrasmissioniFilteredResult>;
    public record GetTaskByTrasmSingolaResult(Task output);

    public record GetTaskByTrasmSingola(string idTrasmSingola, InfoUtente infoUtente) : IRequest<GetTaskByTrasmSingolaResult>;
    public record NewSchedaDocumentoResult(SchedaDocumento output);

    public record NewSchedaDocumento(InfoUtente infoUtente) : IRequest<NewSchedaDocumentoResult>;
    public record getTemplateByIdResult(Templates output);

    public record getTemplateById(string idTemplate) : IRequest<getTemplateByIdResult>;
    public record getTemplateCampiComuniByIdResult(Templates output);

    public record getTemplateCampiComuniById(InfoUtente infoUtente, string idTemplate) : IRequest<getTemplateCampiComuniByIdResult>;
    public record getTemplateDettagliResult(Templates output);

    public record getTemplateDettagli(string docNumber) : IRequest<getTemplateDettagliResult>;
    public record getTemplateFascDettagliResult(Templates output);

    public record getTemplateFascDettagli(string idProject) : IRequest<getTemplateFascDettagliResult>;
    public record FascicolazioneGetFascicoloByIdResult(Fascicolo output);

    public record FascicolazioneGetFascicoloById(string idFascicolo, InfoUtente infoUtente) : IRequest<FascicolazioneGetFascicoloByIdResult>;
    public record AddressbookGetCorrispondenteBySystemIdResult(Corrispondente output);

    public record AddressbookGetCorrispondenteBySystemId(string system_id) : IRequest<AddressbookGetCorrispondenteBySystemIdResult>;
    public record SelectSecurityResult(bool output, string accessRights, string idGruppoTrasm, string tipoDiritto);

    public record SelectSecurity(string thing, string personOrgroup, string accessRightsToTest) : IRequest<SelectSecurityResult>;
    public record getUtenteByIdResult(Utente output);

    public record getUtenteById(string idPeople) : IRequest<getUtenteByIdResult>;
    public record DocumentoGetVisibilitaSemplificataResult(DocsPaVO.documento.DirittoOggetto[] output);

    public record DocumentoGetVisibilitaSemplificata(InfoUtente infoUtente, string idProfile, bool cercaRimossi) : IRequest<DocumentoGetVisibilitaSemplificataResult>;
    public record getModelloByIDResult(ModelloTrasmissione output);

    public record getModelloByID(string idAmm, string idModello) : IRequest<getModelloByIDResult>;
    public record TrasmissioneExecuteTrasmResult(Trasmissione output);

    public record TrasmissioneExecuteTrasm(string path, Trasmissione trasmissione, InfoUtente infoUtente) : IRequest<TrasmissioneExecuteTrasmResult>;
    public record AddressbookGetCorrispondenteByCodRubricaIENotDisabledResult(Corrispondente output);

    public record AddressbookGetCorrispondenteByCodRubricaIENotDisabled(string codice, TipoUtente tipoIE, InfoUtente u) : IRequest<AddressbookGetCorrispondenteByCodRubricaIENotDisabledResult>;


    public record salvaStoricoTrasmDiagrammiResult();

    public record salvaStoricoTrasmDiagrammi(string idTrasm, string docNumber, string idStato) : IRequest<salvaStoricoTrasmDiagrammiResult>;
    public record UtenteGetRegistriWithRfResult(Registro[] output);

    public record UtenteGetRegistriWithRf(string idCorrGlobali, string all, string idAooColl, bool protocolloAbilitato) : IRequest<UtenteGetRegistriWithRfResult>;
    public record TrasmissioneGetRagioniResult(RagioneTrasmissione[] output);

    public record TrasmissioneGetRagioni(Diritti diritti, bool flgDaRicercaTrasm) : IRequest<TrasmissioneGetRagioniResult>;
    public record getDgByIdTipoDocResult(DiagrammaStato output);

    public record getDgByIdTipoDoc(string systemIdTipoDoc, string idAmm) : IRequest<getDgByIdTipoDocResult>;
    public record IsAssociatoRuoloDiagrammaResult(bool output);

    public record IsAssociatoRuoloDiagramma(string idDiagramma, string idRuolo) : IRequest<IsAssociatoRuoloDiagrammaResult>;
    public record getModelliPerTrasmLiteFascResult(object[] output);

    public record getModelliPerTrasmLiteFasc(string idAmm, Registro[] registri, string idPeople, string idCorrGlobali, string idTipoFasc, string idDiagramma, string idStato, string cha_tipo_oggetto, string system_id, string idRuoloUtente, bool AllReg, string accessrights) : IRequest<getModelliPerTrasmLiteFascResult>;
    public record DocumentoExecAddLavoroResult(bool output);

    public record DocumentoExecAddLavoro(string idProfile, string tipoProto, Fascicolo fasc, InfoUtente infoUtente, string idRegistro) : IRequest<DocumentoExecAddLavoroResult>;
    public record GetRoleChainsIdResult(RoleChainResponse output);

    public record GetRoleChainsId(RoleChainRequest request) : IRequest<GetRoleChainsIdResult>;
    public record DocumentoGetDettaglioDocumentoNoSecurityResult(SchedaDocumento output);

    public record DocumentoGetDettaglioDocumentoNoSecurity(InfoUtente infoutente, string idProfile, string docNumber) : IRequest<DocumentoGetDettaglioDocumentoNoSecurityResult>;
    public record InserimentoInLibroFirmaResult(bool output);

    public record InserimentoInLibroFirma(ElementoInLibroFirma elemento, InfoUtente infoUtente) : IRequest<InserimentoInLibroFirmaResult>;
    public record isUltimaDaAccettareResult(bool output);

    public record isUltimaDaAccettare(string idTrasmissione) : IRequest<isUltimaDaAccettareResult>;

    public record getTemplateResult(Templates output);

    public record getTemplate(string idAmministrazione, string tipoAtto, string docNumber) : IRequest<getTemplateResult>;
    public record isStatoAutoResult(bool output);

    public record isStatoAuto(string idStato, string idDiagramma) : IRequest<isStatoAutoResult>;
    public record deleteStoricoTrasmDiagrammiResult();

    public record deleteStoricoTrasmDiagrammi(string docNumber, string idStato) : IRequest<deleteStoricoTrasmDiagrammiResult>;
    public record AcquireRightsFromExtSysResult(bool output);

    public record AcquireRightsFromExtSys(string idObject, string idRuolo, string idUtente, string idSE, string idUSE) : IRequest<AcquireRightsFromExtSysResult>;
    public record checkTrasm_UNO_TUTTI_AccettataRifiutataResult(bool output);

    public record checkTrasm_UNO_TUTTI_AccettataRifiutata(TrasmissioneSingola trasmSingola) : IRequest<checkTrasm_UNO_TUTTI_AccettataRifiutataResult>;
    public record getIfDocOrFascIsInToDoListResult(bool output);

    public record getIfDocOrFascIsInToDoList(InfoUtente infoUtente, string idTrasmissione) : IRequest<getIfDocOrFascIsInToDoListResult>;
    public record TrasmissioneExecuteAccRifConFascicolazioneResult(bool output, string errore);

    public record TrasmissioneExecuteAccRifConFascicolazione(TrasmissioneUtente trasmissioneUtente, string idTrasmissione, Ruolo ruolo, InfoUtente infoUtente, Fascicolo fascicolo) : IRequest<TrasmissioneExecuteAccRifConFascicolazioneResult>;
    public record TrasmissioneExecuteAccRifResult(bool output, string errore);

    public record TrasmissioneExecuteAccRif(TrasmissioneUtente trasmissioneUtente, string idTrasmissione, Ruolo ruolo, InfoUtente infoUtente) : IRequest<TrasmissioneExecuteAccRifResult>;
    public record DocumentoExecAddLavoroRoleResult(bool output);

    public record DocumentoExecAddLavoroRole(string idProfile, string tipoProto, Fascicolo fasc, InfoUtente infoUtente, string idRegistro) : IRequest<DocumentoExecAddLavoroRoleResult>;
    public record SetDataVistaSP_TVResult(bool output);

    public record SetDataVistaSP_TV(InfoUtente infoutente, string docNumber, string docOrFasc, string idRegistro, string idTrasm) : IRequest<SetDataVistaSP_TVResult>;
    public record getSistemaEsternoByCodeAppResult(SistemaEsterno output);

    public record getSistemaEsternoByCodeApp(string idAmm, string codeApp) : IRequest<getSistemaEsternoByCodeAppResult>;
    public record IsDocInLibroFirmaResult(bool output);

    public record IsDocInLibroFirma(string docNumber) : IRequest<IsDocInLibroFirmaResult>;
    public record isDocInADLRoleResult(int output);

    public record isDocInADLRole(string idProfile, string idRole) : IRequest<isDocInADLRoleResult>;
    public record IsDocumentReceivedWithISResult(bool output);

    public record IsDocumentReceivedWithIS(string documentId) : IRequest<IsDocumentReceivedWithISResult>;
    public record rubricaGetElementiRubricaResult(ElementoRubrica[] output);

    public record rubricaGetElementiRubrica(ParametriRicercaRubrica qc, InfoUtente u, SmistamentoRubrica smistamentoRubrica) : IRequest<rubricaGetElementiRubricaResult>;
    public record GetCorrRubricaComuneResult(Corrispondente output);

    public record GetCorrRubricaComune(string codice, InfoUtente u) : IRequest<GetCorrRubricaComuneResult>;
    public record FascicolazioneGetGerarchiaResult(Classifica[] output);

    public record FascicolazioneGetGerarchia(string idClassificazione, string idAmm) : IRequest<FascicolazioneGetGerarchiaResult>;
    public record FascicolazioneGetFascicoloDaCodiceResult(Fascicolo output);

    public record FascicolazioneGetFascicoloDaCodice(InfoUtente infoUtente, string codiceFascicolo, Registro registro, bool enableUffRef, bool enableProfilazione) : IRequest<FascicolazioneGetFascicoloDaCodiceResult>;
    public record FascicolazioneGetListaFascicoliDaCodiceResult(Fascicolo[] output);

    public record FascicolazioneGetListaFascicoliDaCodice(InfoUtente infoUtente, string codiceFascicolo, Registro registro, bool enableUffRef, bool enableProfilazione, string insRic) : IRequest<FascicolazioneGetListaFascicoliDaCodiceResult>;
    public record FascicolazioneGetListaFolderDaCodiceResult(Folder[] output);

    public record FascicolazioneGetListaFolderDaCodice(InfoUtente infoUtente, string codiceFascicolo, string descrFolder, Registro registro, bool enableUffRef, bool enableProfilazione) : IRequest<FascicolazioneGetListaFolderDaCodiceResult>;
    public record FascicolazioneAddDocFascicoloResult(bool output, string msg);

    public record FascicolazioneAddDocFascicolo(InfoUtente infoutente, string idProfile, Fascicolo Fascicolo, bool fascRapida) : IRequest<FascicolazioneAddDocFascicoloResult>;
    public record FascicolazioneGetTitolario2Result(Classificazione[] output);

    public record FascicolazioneGetTitolario2(string idAmministrazione, string idGruppo, string idPeople, Registro registro, string codiceClassifica, bool getFigli, string idTitolario) : IRequest<FascicolazioneGetTitolario2Result>;
    public record getTitolariUtilizzabiliResult(OrgTitolario[] output);

    public record getTitolariUtilizzabili(string idAmministrazione) : IRequest<getTitolariUtilizzabiliResult>;
}
