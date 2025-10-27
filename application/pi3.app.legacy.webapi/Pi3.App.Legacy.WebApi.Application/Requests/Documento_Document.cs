// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.Grids;
using DocsPaVO.Interoperabilita.Semplificata;
using DocsPaVO.LibroFirma;
using DocsPaVO.Notification;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ricerche;
using DocsPaVO.rubrica;
using DocsPaVO.utente;
using MediatR;
using Pi3.Core.Services.File.FileValidator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record AcquisisciDirittiDocumentoResult(bool output);

    public record AcquisisciDirittiDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<AcquisisciDirittiDocumentoResult>;
    public record AmmListaMezzoSpedizioneResult(DocsPaVO.amministrazione.MezzoSpedizione[] output);

    public record AmmListaMezzoSpedizione(string idAmm, bool vediTutti) : IRequest<AmmListaMezzoSpedizioneResult>;
    public record getTipoAttoPDInsRicResult(TipologiaAtto[] output);

    public record getTipoAttoPDInsRic(string idAmministrazione, string idGruppo, string diritti) : IRequest<getTipoAttoPDInsRicResult>;
    public record DocumentoGetTipologiaAttoResult(TipologiaAtto[] output);

    public record DocumentoGetTipologiaAtto() : IRequest<DocumentoGetTipologiaAttoResult>;
    public record CheckDocumentIsSentResult(bool output);

    public record CheckDocumentIsSent(string idDocument) : IRequest<CheckDocumentIsSentResult>;
    public record ExistsTrasmPendenteConWorkflowDocumentoResult(bool output);

    public record ExistsTrasmPendenteConWorkflowDocumento(string idProfile, string idRuoloInUO, string idPeople, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<ExistsTrasmPendenteConWorkflowDocumentoResult>;
    public record ExistsTrasmPendenteSenzaWorkflowDocumentoResult(bool output);

    public record ExistsTrasmPendenteSenzaWorkflowDocumento(string idProfile, string idRuoloInUO, string idPeople, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<ExistsTrasmPendenteSenzaWorkflowDocumentoResult>;
    public record IsOggettoModificatoResult(bool output);

    public record IsOggettoModificato(string idProfile) : IRequest<IsOggettoModificatoResult>;
    public record InteroperabilitaAggiornamentoConfermaResult(ProtocolloDestinatario[] output);

    public record InteroperabilitaAggiornamentoConferma(string idProfile, DocsPaVO.utente.Corrispondente corrispondente) : IRequest<InteroperabilitaAggiornamentoConfermaResult>;
    public record GetFascicolazionePrimariaResult(string output);

    public record GetFascicolazionePrimaria(DocsPaVO.utente.InfoUtente infoUtente, string idProfile) : IRequest<GetFascicolazionePrimariaResult>;
    public record VerificaDirittiCestinaDocumentoResult(string output);

    public record VerificaDirittiCestinaDocumento(DocsPaVO.utente.InfoUtente infoutente, DocsPaVO.documento.SchedaDocumento schedaDoc) : IRequest<VerificaDirittiCestinaDocumentoResult>;
    public record InteroperabilitaIsDocPecPendenteResult(bool output);

    public record InteroperabilitaIsDocPecPendente(string idDocument) : IRequest<InteroperabilitaIsDocPecPendenteResult>;
    public record isDocInADLResult(int output);

    public record isDocInADL(string idProfile, string idPeople, string idRole) : IRequest<isDocInADLResult>;
    public record DocumentoGetNumDocInRispostaResult(int output);

    public record DocumentoGetNumDocInRisposta(string idGruppo, string idPeople, DocsPaVO.filtri.FiltroRicerca[][] queryList, bool security) : IRequest<DocumentoGetNumDocInRispostaResult>;
    public record LoadSimplifiedInteroperabilitySettingsResult(InteroperabilitySettings output);

    public record LoadSimplifiedInteroperabilitySettings(String registryId) : IRequest<LoadSimplifiedInteroperabilitySettingsResult>;
    public record getDirittiCampiTipologiaDocResult(AssDocFascRuoli[] output);

    public record getDirittiCampiTipologiaDoc(string idRuolo, string idTemplate) : IRequest<getDirittiCampiTipologiaDocResult>;
    public record FascicolazioneGetFolderByIdResult(DocsPaVO.fascicolazione.Folder output);

    public record FascicolazioneGetFolderById(string idPeople, string idGruppo, string idFolder) : IRequest<FascicolazioneGetFolderByIdResult>;      
    public record GetPrefChannelAllDestResult(Corrispondente[] output);

    public record GetPrefChannelAllDest(string idProfile, string typeDest) : IRequest<GetPrefChannelAllDestResult>;  
    public record GetRegistroBySistemIdResult(DocsPaVO.utente.Registro output);

    public record GetRegistroBySistemId(string idRegistro) : IRequest<GetRegistroBySistemIdResult>;                                
    public record getOldDescByCorrResult(string output);

    public record getOldDescByCorr(string systemId) : IRequest<getOldDescByCorrResult>; 
    public record getCheckInteropFromSysIdCorrGlobResult(string output);

    public record getCheckInteropFromSysIdCorrGlob(string systemId) : IRequest<getCheckInteropFromSysIdCorrGlobResult>;                                      
    public record GetCorrByEmailResult(DataSet output);

    public record GetCorrByEmail(string email, string idRegistri) : IRequest<GetCorrByEmailResult>;
    public record GetCorrByEmailAndDescrResult(DataSet output);

    public record GetCorrByEmailAndDescr(string email, string descr, string idRegistri) : IRequest<GetCorrByEmailAndDescrResult>;
    public record DocumentAlreadyTransmitted_OptResult(bool output);

    public record DocumentAlreadyTransmitted_Opt(string idDocument) : IRequest<DocumentAlreadyTransmitted_OptResult>;

    public record DocumentoGetListaOggettiResult(Oggetto[] output);

    public record DocumentoGetListaOggetti(DocsPaVO.documento.QueryOggetto queryOggetto) : IRequest<DocumentoGetListaOggettiResult>;
    
    public record IsRuoloAssociatoStatoDiaResult(bool output);

    public record IsRuoloAssociatoStatoDia(string idDiagramma, string idRuolo, string idStato) : IRequest<IsRuoloAssociatoStatoDiaResult>;
    public record getStatoDocStoricoResult(string output);

    public record getStatoDocStorico(string docNumber) : IRequest<getStatoDocStoricoResult>;
    public record IsDocumentoInLibroFirmaConCambioSatoResult(bool output);

    public record IsDocumentoInLibroFirmaConCambioSato(string idDocumento) : IRequest<IsDocumentoInLibroFirmaConCambioSatoResult>;
    public record ResetCorrVarInsertIteropResult(int output);

    public record ResetCorrVarInsertIterop(string corrSystemId, string val) : IRequest<ResetCorrVarInsertIteropResult>;
    public record DocumentoCambiaPersonalePrivatoResult(bool output);

    public record DocumentoCambiaPersonalePrivato(string idProfile, string idGruppo) : IRequest<DocumentoCambiaPersonalePrivatoResult>;
    public record AmmGetInfoAmmCorrenteResult(DocsPaVO.amministrazione.InfoAmministrazione output);

    public record AmmGetInfoAmmCorrente(string idAmm) : IRequest<AmmGetInfoAmmCorrenteResult>;
    public record ResetCodRubCorrIteropResult(int output);

    public record ResetCodRubCorrIterop(string corrSystemId, string val) : IRequest<ResetCodRubCorrIteropResult>;
    public record GetAssDocAddressResult(DataSet output);

    public record GetAssDocAddress(string docNumber) : IRequest<GetAssDocAddressResult>;
    public record UpdateAssDocAddressResult(bool output);

    public record UpdateAssDocAddress(string docNumber, string idRegistro, string mailAddress) : IRequest<UpdateAssDocAddressResult>;
    public record ProtocolloInvioRicevutaDiRitornoResult(bool output, string errorMessage);

    public record ProtocolloInvioRicevutaDiRitorno(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Registro registro, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<ProtocolloInvioRicevutaDiRitornoResult>;
    public record doucmentoGetDocTypeResult(string output);

    public record doucmentoGetDocType() : IRequest<doucmentoGetDocTypeResult>;
    public record DocumentoProtocollaResult(DocsPaVO.documento.SchedaDocumento output, DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione);

    public record DocumentoProtocolla(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo) : IRequest<DocumentoProtocollaResult>;
    public record RemotePdfSignStampResult(DocsPaVO.documento.SchedaDocumento output, DocsPaVO.documento.ResultSigilloElettronico result);

    public record RemotePdfSignStamp(DocsPaVO.documento.labelPdf labelPdf, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fr, DocsPaVO.documento.SchedaDocumento schedaDoc) : IRequest<RemotePdfSignStampResult>;
    public record TrasmissioneAddDaTemplResult(DocsPaVO.trasmissione.Trasmissione output);

    public record TrasmissioneAddDaTempl(DocsPaVO.documento.InfoDocumento infoDoc, DocsPaVO.trasmissione.TemplateTrasmissione template, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo) : IRequest<TrasmissioneAddDaTemplResult>;
    public record AssociaContributoAlTaskResult(bool output);

    public record AssociaContributoAlTask(DocsPaVO.Task.Task task, DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<AssociaContributoAlTaskResult>;
    public record ModifyNotificationsResult();

    public record ModifyNotifications(InfoUtente infoUtente, TypeOperation[] typeOperation, string idObject, string domainObject) : IRequest<ModifyNotificationsResult>;
    public record collegaMezzoSpedizioneDocumentoResult(bool output);

    public record collegaMezzoSpedizioneDocumento(DocsPaVO.utente.InfoUtente info, string idDocumentTypes, string idProfile) : IRequest<collegaMezzoSpedizioneDocumentoResult>;
    public record IsModelloDiFirmaResult(bool output);

    public record IsModelloDiFirma(string idProcesso) : IRequest<IsModelloDiFirmaResult>;
    public record GetDocumentConsolidationStateResult(DocsPaVO.documento.DocumentConsolidationStateInfo output);

    public record GetDocumentConsolidationState(DocsPaVO.utente.InfoUtente userInfo, string idDocument) : IRequest<GetDocumentConsolidationStateResult>;
    public record GetIstanzaPassoFirmaInAttesaByDocnumberResult(DocsPaVO.LibroFirma.IstanzaPassoDiFirma output);

    public record GetIstanzaPassoFirmaInAttesaByDocnumber(string docnumber) : IRequest<GetIstanzaPassoFirmaInAttesaByDocnumberResult>;
    public record DocumentoGetSeDocFascicolatoResult(bool output);

    public record DocumentoGetSeDocFascicolato(string idDocumento) : IRequest<DocumentoGetSeDocFascicolatoResult>;
    public record VerifyAndSetTipoDocResult(string output, DocsPaVO.documento.SchedaDocumento schedaDocumento);

    public record VerifyAndSetTipoDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento) : IRequest<VerifyAndSetTipoDocResult>;
    public record UOHasReferenceRoleResult(bool output);

    public record UOHasReferenceRole(string idUO) : IRequest<UOHasReferenceRoleResult>;
    public record GetConfigSpedizioneDocumentoResult(DocsPaVO.Spedizione.ConfigSpedizioneDocumento output);

    public record GetConfigSpedizioneDocumento(DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetConfigSpedizioneDocumentoResult>;
    public record SpedisciDocumentoInAutomaticoResult(DocsPaVO.Spedizione.SpedizioneDocumento output);

    public record SpedisciDocumentoInAutomatico(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento) : IRequest<SpedisciDocumentoInAutomaticoResult>;
    public record TrasmettiProtocolloInternoResult(bool output, bool RagioniVerificate, string message);

    public record TrasmettiProtocolloInterno(DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.utente.Ruolo ruolo, string serverName, bool isEnableRef, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<TrasmettiProtocolloInternoResult>;
    public record ereditaVisibilitaResult(bool output);

    public record ereditaVisibilita(string idAmm, string idModello) : IRequest<ereditaVisibilitaResult>;
    public record DocumentoCercaDuplicatiInfoResult(DocsPaVO.documento.RicercaDuplicati.EsitoRicercaDuplicatiEnum output, DocsPaVO.documento.InfoProtocolloDuplicato[] datiProtDupl);

    public record DocumentoCercaDuplicatiInfo(DocsPaVO.documento.SchedaDocumento schedaDocumento, string cercaDuplicati2) : IRequest<DocumentoCercaDuplicatiInfoResult>;
    public record IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivateResult(bool output);

    public record IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivate(String documentId) : IRequest<IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivateResult>;
    public record getCorrispondentiByCodListaByUtenteResult(ArrayList output);

    public record getCorrispondentiByCodListaByUtente(string codiceLista, string idAmm, InfoUtente infoUtente) : IRequest<getCorrispondentiByCodListaByUtenteResult>;
    public record getNomeListaResult(string output);

    public record getNomeLista(string codiceLista, string idAmm) : IRequest<getNomeListaResult>;
    public record getNomeRFResult(string output);

    public record getNomeRF(string codiceRF) : IRequest<getNomeRFResult>;

    public record GetRuoloByIdResult(DocsPaVO.utente.Ruolo? Output);

    public record GetRuoloById(string IdCorrGlobali) : IRequest<GetRuoloByIdResult>;

    public record AddressbookGetCorrispondenteBySystemIdDisabledResult(DocsPaVO.utente.Corrispondente output);

    public record AddressbookGetCorrispondenteBySystemIdDisabled(string system_id) : IRequest<AddressbookGetCorrispondenteBySystemIdDisabledResult>;
    public record CheckAllegatiInLibroFirmaResult(bool output);

    public record CheckAllegatiInLibroFirma(string idDocumentoPrincipale) : IRequest<CheckAllegatiInLibroFirmaResult>;
    public record IsTitolarePassoInAttesaResult(bool output);

    public record IsTitolarePassoInAttesa(string docNumber, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.LibroFirma.Azione azione) : IRequest<IsTitolarePassoInAttesaResult>;
    public record UpdateNoteResult(DocsPaVO.Note.InfoNota[] output);

    public record UpdateNote(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Note.AssociazioneNota oggettoAssociato, DocsPaVO.Note.InfoNota[] note) : IRequest<UpdateNoteResult>;
    public record DocumentoAddDocGrigiaResult(DocsPaVO.documento.SchedaDocumento output);

    public record DocumentoAddDocGrigia(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo) : IRequest<DocumentoAddDocGrigiaResult>;
    public record DocumentoSaveDocumentoResult(DocsPaVO.documento.SchedaDocumento output, bool daAggiornareUffRef);

    public record DocumentoSaveDocumento(DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento, bool enableUffRef) : IRequest<DocumentoSaveDocumentoResult>;
    public record DocumentoScollegaCollegamentoResult(bool output);

    public record DocumentoScollegaCollegamento(string systemId) : IRequest<DocumentoScollegaCollegamentoResult>;
    public record updateMezzoSpedizioneDocumentoResult(bool output);

    public record updateMezzoSpedizioneDocumento(DocsPaVO.utente.InfoUtente info, string oldDocumentTypes, string idDocumentTypes, string idProfile) : IRequest<updateMezzoSpedizioneDocumentoResult>;
    public record deleteMezzoSpedizioneDocumentoResult(bool output);

    public record deleteMezzoSpedizioneDocumento(DocsPaVO.utente.InfoUtente info, string idProfile) : IRequest<deleteMezzoSpedizioneDocumentoResult>;
    public record DO_TrasmettiDestinatariModificatiResult(bool output);

    public record DO_TrasmettiDestinatariModificati(DocsPaVO.documento.SchedaDocumento scheda, DocsPaVO.utente.Ruolo ruolo, string serverName) : IRequest<DO_TrasmettiDestinatariModificatiResult>;
    
    public record GetListaRegistriByRuoloResult(Registro[] output);

    public record GetListaRegistriByRuolo(string IdRuolo) : IRequest<GetListaRegistriByRuoloResult>;
    
    public record GetUoInterneAooResult(string[] Output);

    public record GetUoInterneAoo(string id_reg, DocsPaVO.utente.InfoUtente u) : IRequest<GetUoInterneAooResult>;
    public record getUtenteInternoAOOResult(string[] output);

    public record getUtenteInternoAOO(string idPeople, string systemIdRegistro, DocsPaVO.utente.InfoUtente user) : IRequest<getUtenteInternoAOOResult>;
    public record IsCheckedOutDocumentSimpleResult(bool output);

    public record IsCheckedOutDocumentSimple(string idDocument, string documentNumber, DocsPaVO.utente.InfoUtente infoUtente, bool checkAllegati, DocsPaVO.documento.SchedaDocumento doc) : IRequest<IsCheckedOutDocumentSimpleResult>;
    public record DocumentoGetQueryDocumentoPagingCustomResult(SearchObject[] output, int numTotPage, int nRec, List<SearchResultInfo> idProfileList);

    public record DocumentoGetQueryDocumentoPagingCustom(InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] queryList, int numPage, bool security, int pageSize, bool getIdProfilesList, bool gridPersonalization, bool export, DocsPaVO.Grid.Field[] visibleFieldsTemplate, String[] documentsSystemId) : IRequest<DocumentoGetQueryDocumentoPagingCustomResult>;
    public record FullTextSearchResult(InfoDocumento[] output, DocsPaVO.ricerche.FullTextSearchContext context);

    public record FullTextSearch(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.ricerche.FullTextSearchContext context) : IRequest<FullTextSearchResult>;
    public record ReportBustaResult(DocsPaVO.documento.FileDocumento output);

    public record ReportBusta(DocsPaVO.documento.SchedaDocumento schedaDoc) : IRequest<ReportBustaResult>;
    public record IsDocOrAllInLibroFirmaResult(bool output);

    public record IsDocOrAllInLibroFirma(string docNumber) : IRequest<IsDocOrAllInLibroFirmaResult>;
    public record DocumentoRiproponiConCopiaDocResult(DocsPaVO.documento.SchedaDocumento output);

    public record DocumentoRiproponiConCopiaDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.documento.SchedaDocumento schedaDocumento) : IRequest<DocumentoRiproponiConCopiaDocResult>;
    public record DocumentoInoltraDocResult(DocsPaVO.documento.SchedaDocumento output);

    public record DocumentoInoltraDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.documento.SchedaDocumento schedaDocumento) : IRequest<DocumentoInoltraDocResult>;
    public record DocumentoCancellaAreaLavoroResult(bool output);

    public record DocumentoCancellaAreaLavoro(string idPeople, string idRuoloInUO, string idProfile, DocsPaVO.fascicolazione.Fascicolo fasc) : IRequest<DocumentoCancellaAreaLavoroResult>;
    public record AcceptMassiveTrasmDocumentResult(bool output);

    public record AcceptMassiveTrasmDocument(string idProfile, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<AcceptMassiveTrasmDocumentResult>;
    public record ViewMassiveTrasmDocumentResult(bool output);

    public record ViewMassiveTrasmDocument(string idProfile, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<ViewMassiveTrasmDocumentResult>;
    public record GetStatoConformitaDocumentoResult(DocsPaVO.documento.StatoConformitaDocumento output);

    public record GetStatoConformitaDocumento(string idProfile, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetStatoConformitaDocumentoResult>;

    public record salvaDataScadenzaDocResult();

    public record salvaDataScadenzaDoc(string docNumber, string dataScadenza, string idTipoAtto) : IRequest<salvaDataScadenzaDocResult>;

    public record GetFileDocument(FileRequest request, InfoUtente infoUtente) : IRequest<GetFileDocumentResult>;

    public record GetFileDocumentResult(FileDocumento output);

    public record AggiornaEsecuzioneElementoInLibroFirma(string docnumber, string descAzione, bool eseguitaFirma, TipoStatoElemento stato, string errore = null) : IRequest<AggiornaEsecuzioneElementoInLibroFirmaResult>;

    public record AggiornaEsecuzioneElementoInLibroFirmaResult();

    public record AllegatoIsIS(string version_id) : IRequest<AllegatoIsISResult>;
    public record AllegatoIsISResult(string output);

    public record AllegatoIsEsterno(string version_id) : IRequest<AllegatoIsEsternoResult>;
    public record AllegatoIsEsternoResult(string output);

    public record AddressbookGetRuoliUtente(string id_amm, string cod_rubrica) : IRequest<AddressbookGetRuoliUtenteResult>;
    public record AddressbookGetRuoliUtenteResult(ElementoRubrica[] output);

    public record GetDescrizioneTipoDocumento(string typeId) : IRequest<GetDescrizioneTipoDocumentoResult>;
    public record GetDescrizioneTipoDocumentoResult(string output);

    public record UpdateLastDocumentsView(string docnumber) : IRequest<UpdateLastDocumentsViewResult>;
    public record UpdateLastDocumentsViewResult();
    public record GetLabelTipoDocumento(string typeId) : IRequest<GetLabelTipoDocumentoResult>;
    public record GetLabelTipoDocumentoResult(string output);

    public record AddAllegatoSegnaturaXMLResult();
    public record AddAllegatoSegnaturaXML(string docnumber) : IRequest<AddAllegatoSegnaturaXMLResult>;

    public record getDiagrammaStoricoDocResult(DataSet output);
    public record getDiagrammaStoricoDoc(string docNumber) : IRequest<getDiagrammaStoricoDocResult>;

    public record EseguiPassoAutomatico(string idIstanzaProcessoFirma) : IRequest<EseguiPassoAutomaticoResult>;
    public record EseguiPassoAutomaticoResult();

    public record VerifyCertificateExpired(CertificateInfo certificate, InfoUtente infoUtente) : IRequest<VerifyCertificateExpiredResult>;
    public record VerifyCertificateExpiredResult(CertificateInfo output);

    public record DocumentSaveTimestamp(FileDocumento fileDocumento, FileRequest fileRequest) : IRequest<DocumentSaveTimestampResult>;
    public record DocumentSaveTimestampResult(bool output);

    public record ExtractXmlSuapResult();
    public record ExtractXmlSuapRequest(DocsPaVO.documento.SchedaDocumento schedaDoc, string fileName, byte[] filecontents) : IRequest<ExtractXmlSuapResult>;

    public record AttachXmlSuapResult(DocsPaVO.documento.Allegato[] allegati);
    public record AttachXmlSuapRequest(string docnumber, string mailFrom, DocsPaVO.utente.InfoUtente infoUtenteInterop) : IRequest<AttachXmlSuapResult>;

    public record DocumentoAddInfoFileResult();
    public record DocumentoAddInfoFileRequest(FileRequest fileRequest, long? idDocumentoPrincipale, FileValidationResult? fileValidateResult) : IRequest<DocumentoAddInfoFileResult>;
}
