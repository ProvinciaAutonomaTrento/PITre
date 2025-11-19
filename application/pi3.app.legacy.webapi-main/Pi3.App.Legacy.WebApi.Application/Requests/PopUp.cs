// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;
using DocsPaVO.areaConservazione;
using DocsPaVO.areaLavoro;
using DocsPaVO.Conservazione;
using DocsPaVO.DatiCert;
using DocsPaVO.Deposito;
using DocsPaVO.DesktopApps;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.filtri;
using DocsPaVO.FlussoAutomatico;
using DocsPaVO.FormatiDocumento;
using DocsPaVO.Grid;
using DocsPaVO.LibroFirma;
using DocsPaVO.LiveCycle;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.Note;
using DocsPaVO.PrjDocImport;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ricerche;
using DocsPaVO.rubrica;
using DocsPaVO.Smistamento;
using DocsPaVO.Spedizione;
using DocsPaVO.StatoInvio;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using DocsPaVO.utente.Repertori.RequestAndResponse;
using DocsPaVO.Validations;
using MediatR;
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Grid.Grid;
using Oggetto = DocsPaVO.documento.Oggetto;
using TipologiaAtto = DocsPaVO.documento.TipologiaAtto;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record AddressBookGetDatiCanalePref_ExperimentalResult(DocsPaVO.utente.Canale output);
    public record AddressBookGetDatiCanalePref_Experimental(DocsPaVO.utente.Corrispondente corr) : IRequest<AddressBookGetDatiCanalePref_ExperimentalResult>;
    
    public record DocumentoAggiungiAllegatoResult(DocsPaVO.documento.Allegato output);
    public record DocumentoAggiungiAllegato(InfoUtente infoUtente, DocsPaVO.documento.Allegato allegato) : IRequest<DocumentoAggiungiAllegatoResult>;
   
    public record amministrazioneGetAmministrazioniResult(Amministrazione[] output, string returnMsg);
    public record amministrazioneGetAmministrazioni() : IRequest<amministrazioneGetAmministrazioniResult>;
    
    public record GetARCHIVE_JOB_TransferPolicyByTransfer_IDResult(ARCHIVE_JOB_TransferPolicy[] output);
    public record GetARCHIVE_JOB_TransferPolicyByTransfer_ID(int transfer_ID) : IRequest<GetARCHIVE_JOB_TransferPolicyByTransfer_IDResult>;
    
    public record GetARCHIVE_LOG_TransferAndPolicyResult(ARCHIVE_LOG_TransferAndPolicy[] output);
    public record GetARCHIVE_LOG_TransferAndPolicy(string ListaVersamentoIDANDPolicyID) : IRequest<GetARCHIVE_LOG_TransferAndPolicyResult>;
    
    public record GetARCHIVE_Profile_TransferPolicyByTransferPolicy_IDResult(ARCHIVE_Profile_TransferPolicy[] output);
    public record GetARCHIVE_Profile_TransferPolicyByTransferPolicy_ID(int TransferPolicy_ID) : IRequest<GetARCHIVE_Profile_TransferPolicyByTransferPolicy_IDResult>;
    
    public record GetARCHIVE_TransferBySystem_IDResult(ARCHIVE_Transfer[] output);
    public record GetARCHIVE_TransferBySystem_ID(int system_ID) : IRequest<GetARCHIVE_TransferBySystem_IDResult>;
    
    public record GetARCHIVE_TransferPolicyByTransfer_IDResult(ARCHIVE_TransferPolicy[] output);
    public record GetARCHIVE_TransferPolicyByTransfer_ID(int? transfer_ID)		
        : IRequest<GetARCHIVE_TransferPolicyByTransfer_IDResult>;
    public record GetARCHIVE_TransferPolicy_ProfileTypeByTransferPolicy_IDResult(ARCHIVE_TransferPolicy_ProfileType[] output);
    public record GetARCHIVE_TransferPolicy_ProfileTypeByTransferPolicy_ID(int TransferPolicy_ID) : IRequest<GetARCHIVE_TransferPolicy_ProfileTypeByTransferPolicy_IDResult>;
    
    public record GetARCHIVE_View_FascReportByTransfer_IDResult(ARCHIVE_View_FascReport[] output);
    public record GetARCHIVE_View_FascReportByTransfer_ID(int transfer_ID) : IRequest<GetARCHIVE_View_FascReportByTransfer_IDResult>;

    public record GetAllARCHIVE_View_PolicyResult(ARCHIVE_View_Policy[] output);
    public record GetAllARCHIVE_View_Policy(int transferID) : IRequest<GetAllARCHIVE_View_PolicyResult>;

    public record ValidateIstanzaConservazioneResult(AreaConservazioneValidationResult output);
    public record ValidateIstanzaConservazione(string idConservazione) : IRequest<ValidateIstanzaConservazioneResult>;

    public record DocumentoGetListaStoricoProfilatiResult(StoricoProfilati[] output);
    public record DocumentoGetListaStoricoProfilati(string id_tipo_atto, string doc_number, string idGroup) : IRequest<DocumentoGetListaStoricoProfilatiResult>;
    
    public record filtra_trasmissioniPerListeResult(ElementoRubrica[] output);
    public record filtra_trasmissioniPerListe(DocsPaVO.rubrica.ParametriRicercaRubrica qc, DocsPaVO.utente.InfoUtente u, DocsPaVO.rubrica.ElementoRubrica[] ers) : IRequest<filtra_trasmissioniPerListeResult>;
  
    public record getListaExtFileAcquisitiResult(string[] output);
    public record getListaExtFileAcquisiti(string idamm) : IRequest<getListaExtFileAcquisitiResult>;

    public record GetStatoConservazioneFascicoloResult(string output);
    public record GetStatoConservazioneFascicolo(string idProject) : IRequest<GetStatoConservazioneFascicoloResult>;

    public record GetListaIdTipiFascByIdPianoConservazione(string idPianoConservazione, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetListaIdTipiFascByIdPianoConservazioneResult>;
    public record GetListaIdTipiFascByIdPianoConservazioneResult(List<string> output);
    public record GetListaRuoliUtenteByIdCorrResult(Ruolo[] output);
    public record GetListaRuoliUtenteByIdCorr(string idCorr) : IRequest<GetListaRuoliUtenteByIdCorrResult>;

    public record GetListaTitoliResult(string[] output);
    public record GetListaTitoli() : IRequest<GetListaTitoliResult>;

    public record getLogImportListeDistrResult(object[] output);
    public record getLogImportListeDistr() : IRequest<getLogImportListeDistrResult>;

    public record GetLogImportRubricaResult(object[] Output);
    public record getLogImportRubrica() : IRequest<GetLogImportRubricaResult>;

    public record GetPianoConservazioneByIdClassificazioneResult(List<DocsPaVO.PianoConservazione> output);
    public record GetPianoConservazioneByIdClassificazione(string idClassificazione, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetPianoConservazioneByIdClassificazioneResult>;

    public record getStatiPerRicercaResult(Stato[] output);

    public record getStatiPerRicerca(String idDiagramma, string docOrFasc) : IRequest<getStatiPerRicercaResult>;
    public record getTimestampsDocResult(TimestampDoc[] output);

    public record getTimestampsDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fileRequest) : IRequest<getTimestampsDocResult>;
    public record rubricaGetChildrenElementResult(ElementoRubrica[] output);

    public record rubricaGetChildrenElement(string elementID, string childrensType, DocsPaVO.utente.InfoUtente u) : IRequest<rubricaGetChildrenElementResult>;
    public record rubricaGetElementiRubricaPagingResult(ElementoRubrica[] output, int totale);

    public record rubricaGetElementiRubricaPaging(DocsPaVO.rubrica.ParametriRicercaRubrica qc, DocsPaVO.utente.InfoUtente u, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica, int firstRowNum, int maxRowForPage) : IRequest<rubricaGetElementiRubricaPagingResult>;
    public record rubricaGetGerarchiaElementoResult(ElementoRubrica[] output);

    public record rubricaGetGerarchiaElemento(string codice, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.utente.InfoUtente u, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica) : IRequest<rubricaGetGerarchiaElementoResult>;
    public record rubricaGetRootItemsResult(ElementoRubrica[] output);

    public record rubricaGetRootItems(DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.utente.InfoUtente u, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica) : IRequest<rubricaGetRootItemsResult>;
   
    public record AggiornaDescrizioneFascicoloResult(bool output, DocsPaVO.fascicolazione.ResultDescrizioniFascicolo resultUpdateDescrizioniFascicolo);

    public record AggiornaDescrizioneFascicolo(DescrizioneFascicolo descFasc, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<AggiornaDescrizioneFascicoloResult>;
    public record AggiornaStatoElementoInLibroFirmaResult(bool output, string message);

    public record AggiornaStatoElementoInLibroFirma(DocsPaVO.LibroFirma.ElementoInLibroFirma elemento, string nuovoStato, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<AggiornaStatoElementoInLibroFirmaResult>;
    public record Albo_InsFileDaPubblicareResult(bool output);

    public record Albo_InsFileDaPubblicare(string idDocPrincipale, string docNumber, string daPubb) : IRequest<Albo_InsFileDaPubblicareResult>;
    public record asyncCheckAndValidateIstanzaConservazioneResult(bool output);

    public record asyncCheckAndValidateIstanzaConservazione(string idConservazione) : IRequest<asyncCheckAndValidateIstanzaConservazioneResult>;
    public record asyncConvertAndSendForConservationResult(bool output);

    public record asyncConvertAndSendForConservation(string idConservazione, string tipo_cons, string note, string descr, string idTipoSupp, bool consolida) : IRequest<asyncConvertAndSendForConservationResult>;
    public record checkConsolidationResult(bool output);

    public record checkConsolidation(object[] idList, InfoUtente infoUtente) : IRequest<checkConsolidationResult>;

    public record CorrispondentiDeleteModifyCorrispondenteEsternoResult(bool output, string message, string newIdCorrGlobali);

    public record CorrispondentiDeleteModifyCorrispondenteEsterno(DocsPaVO.utente.InfoUtente infoutente, DocsPaVO.utente.DatiModificaCorr datiModificaCorr, int flagListe, string action) : IRequest<CorrispondentiDeleteModifyCorrispondenteEsternoResult>;
    
    
    public record UpdateCorrispondenteRc(InfoUtente i, Services.RubricaComune.Corrispondente? corrRubCom):IRequest<UpdateCorrispondenteRcResult>;
    public record UpdateCorrispondenteRcResult(DocsPaVO.utente.Corrispondente corr);
    
    public record CorrispondentiDeleteModifyCorrispondenteEsternoWithIdResult(bool output, string message, string newIdCorr);

    public record CorrispondentiDeleteModifyCorrispondenteEsternoWithId(DocsPaVO.utente.InfoUtente infoutente, DocsPaVO.utente.DatiModificaCorr datiModificaCorr, int flagListe, string action) : IRequest<CorrispondentiDeleteModifyCorrispondenteEsternoWithIdResult>;
    public record DelegaCreaNuovaResult(bool output);

    public record DelegaCreaNuova(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Deleghe.InfoDelega delega) : IRequest<DelegaCreaNuovaResult>;
    public record DelegaModificaResult(bool output);

    public record DelegaModifica(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Deleghe.InfoDelega delega, string tipoDelega, string idRuoloOld, string idUtenteOld, string dataScadenzaOld, string dataDecorrenzaOld, string idRuoloDeleganteOld) : IRequest<DelegaModificaResult>;
    public record DelegaVerificaUnicaAssegnataResult(bool output);

    public record DelegaVerificaUnicaAssegnata(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Deleghe.InfoDelega delega) : IRequest<DelegaVerificaUnicaAssegnataResult>;
    public record DeleteARCHIVE_TransferPolicyListResult(bool output);

    public record DeleteARCHIVE_TransferPolicyList(string ListSystemID) : IRequest<DeleteARCHIVE_TransferPolicyListResult>;
    public record DeleteMailCorrispondenteEsternoResult(bool output);

    public record DeleteMailCorrispondenteEsterno(string idCorrispondente) : IRequest<DeleteMailCorrispondenteEsternoResult>;
    public record DeleteValidateIstanzaConservazioneConPolicyResult(bool output);

    public record DeleteValidateIstanzaConservazioneConPolicy(string idPolicy, string idConservazione, InfoUtente infoUtente) : IRequest<DeleteValidateIstanzaConservazioneConPolicyResult>;
    public record DocumentoCheckUserVisibilityResult(bool output);

    public record DocumentoCheckUserVisibility(string docNumber, InfoUtente infoUtente) : IRequest<DocumentoCheckUserVisibilityResult>;

    public record DocumentoExecAnnullaProtResult(bool output, DocsPaVO.documento.SchedaDocumento schedaDocumento);

    public record DocumentoExecAnnullaProt(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloAnnullato protAnn) : IRequest<DocumentoExecAnnullaProtResult>;
    public record DocumentoExecCestinaResult(bool output, string errorMsg);

    public record DocumentoExecCestina(InfoUtente infoutente, SchedaDocumento schedaDoc, string tipoDoc, string note) : IRequest<DocumentoExecCestinaResult>;
    public record DocumentoModificaVersioneResult(bool output);

    public record DocumentoModificaVersione(InfoUtente infoUtente, DocsPaVO.documento.FileRequest fileReq) : IRequest<DocumentoModificaVersioneResult>;
    public record DocumentoPutFileNoExceptionResult(bool output, DocsPaVO.documento.FileRequest fileRequest, string errorMessage);

    public record DocumentoPutFileNoException(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocument, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<DocumentoPutFileNoExceptionResult>;
    public record DocumentoRimuoviVersioneResult(bool output);

    public record DocumentoRimuoviVersione(DocsPaVO.documento.FileRequest fileRequest, InfoUtente infoUtente, SchedaDocumento schedaDocumento) : IRequest<DocumentoRimuoviVersioneResult>;
    public record DocumentoUpdateAreaConsResult(bool output);

    public record DocumentoUpdateAreaCons(string sysId, string tipo_cons, string note, string descr, string idTipoSupp, InfoUtente infoUtente, bool consolida) : IRequest<DocumentoUpdateAreaConsResult>;
    public record DocumentoVersioneConSegnaturaResult(bool output);

    public record DocumentoVersioneConSegnatura(InfoUtente infoUtente, string versionId) : IRequest<DocumentoVersioneConSegnaturaResult>;
    public record EditingACLResult(bool output);

    public record EditingACL(DocsPaVO.documento.DirittoOggetto docDiritto, string personOrGroup, DocsPaVO.utente.InfoUtente infoUtente, string typeObject) : IRequest<EditingACLResult>;
    public record EditingACLWithTypeResult(bool output);

    public record EditingACLWithType(DocsPaVO.documento.DirittoOggetto docDiritto, string personOrGroup, InfoUtente infoUtente, string typeObject) : IRequest<EditingACLWithTypeResult>;
    public record EditingFascACLResult(bool output);

    public record EditingFascACL(DocsPaVO.fascicolazione.DirittoOggetto fascDiritto, string personOrGroup, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<EditingFascACLResult>;
    public record EliminaDescrizioneFascicoloResult(bool output);

    public record EliminaDescrizioneFascicolo(string systemId, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<EliminaDescrizioneFascicoloResult>;
    public record EliminaDocumentiNonConformiPolicyDaIstanzaResult(bool output);

    public record EliminaDocumentiNonConformiPolicyDaIstanza(string idPolicy, string idConservazione, InfoUtente infoUtente) : IRequest<EliminaDocumentiNonConformiPolicyDaIstanzaResult>;
    public record FascicolaDocumentoAMResult(bool output, string msg);

    public record FascicolaDocumentoAM(DocsPaVO.utente.InfoUtente infoutente, string idProfile, DocsPaVO.fascicolazione.Fascicolo Fascicolo, bool fascRapida) : IRequest<FascicolaDocumentoAMResult>;

    public record FascicolazioneModifyFolderResult(bool output);

    public record FascicolazioneModifyFolder(InfoUtente infoUtente, Folder folder) : IRequest<FascicolazioneModifyFolderResult>;
    public record FunzioneEsistenteResult(bool output);

    public record FunzioneEsistente(string codiceFunzione) : IRequest<FunzioneEsistenteResult>;
    public record HSM_RequestOTPResult(bool output);

    public record HSM_RequestOTP(String AliasCertificato, String DominioCertificato) : IRequest<HSM_RequestOTPResult>;
    public record HSM_SetMementoForUserResult(bool output);

    public record HSM_SetMementoForUser(InfoUtente infoUtente, String dominio, String alias) : IRequest<HSM_SetMementoForUserResult>;
    public record HSM_SignConEsitoResult(bool output, DocsPaVO.documento.FirmaResult firmaResult);

    public record HSM_SignConEsito(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fr, bool cofirma, bool timestamp, String TipoFirma, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool ConvertPdf) : IRequest<HSM_SignConEsitoResult>;
    public record ImportaListaDistribuzioneResult(bool output, int ListeCreate, int ListeNonCreate, int corrInseriti, int corrRimossi, int corrNonInseriti, int corrNonRimossi);

    public record ImportaListaDistribuzione(InfoUtente infoUtente, byte[] data, string title) : IRequest<ImportaListaDistribuzioneResult>;
    public record ImportaRubricaResult(bool output, int corrInseriti, int corrAggiornati, int corrRimossi, int corrNonInseriti, int corrNonAggiornati, int corrNonRimossi);

    public record ImportaRubrica(DocsPaVO.utente.InfoUtente infoUtente, byte[] dati, int flagListe, string nomeFile, int corrInseriti, int corrAggiornati, int corrRimossi, int corrNonInseriti, int corrNonAggiornati, int corrNonRimossi) : IRequest<ImportaRubricaResult>;
    public record InsertDescrizioneFascicoloResult(bool output, DocsPaVO.fascicolazione.ResultDescrizioniFascicolo resultInsertDescrizioniFascicolo);

    public record InsertDescrizioneFascicolo(DescrizioneFascicolo descFasc, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<InsertDescrizioneFascicoloResult>;
    public record InsertMailCorrispondenteEsternoResult(bool output);

    public record InsertMailCorrispondenteEsterno(System.Collections.Generic.List<MailCorrispondente> listCaselle, string idCorrispondente) : IRequest<InsertMailCorrispondenteEsternoResult>;
    public record InsertNotaInElencoResult(bool output, string message);

    public record InsertNotaInElenco(InfoUtente infoUtente, NotaElenco nota) : IRequest<InsertNotaInElencoResult>;
    public record InsertVisibilitaProcessoResult(bool output);

    public record InsertVisibilitaProcesso(VisibilitaProcessoRuolo[] visibilita, InfoUtente infoUtente) : IRequest<InsertVisibilitaProcessoResult>;
    public record InterruptionSignatureProcessByHolderResult(bool output, string errore);

    public record InterruptionSignatureProcessByHolder(DocsPaVO.LibroFirma.ElementoInLibroFirma elemento, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<InterruptionSignatureProcessByHolderResult>;
    public record InterruptionSignatureProcessByProponentResult(bool output);

    public record InterruptionSignatureProcessByProponent(DocsPaVO.LibroFirma.IstanzaProcessoDiFirma istanza, string noteInterruzione, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente, string idDocPrincipale) : IRequest<InterruptionSignatureProcessByProponentResult>;
    public record IsCodRubricaPresenteResult(bool output);

    public record IsCodRubricaPresente(string codRubrica, string tipoCorr, string idAmm, string idReg, bool inRubricaComune) : IRequest<IsCodRubricaPresenteResult>;
   
    public record CheckCodRubricaPresenteResult(bool output);
    public record CheckCodRubricaPresente(string codRubrica, string tipoCorr, string idAmm, string idReg, bool inRubricaComune) : IRequest<CheckCodRubricaPresenteResult>;
    
    public record IsDocAnnullatoByIdProfileResult(bool output);

    public record IsDocAnnullatoByIdProfile(string idProfile) : IRequest<IsDocAnnullatoByIdProfileResult>;

    public record IsDocumentInFolderOrSubFolderResult(bool output);

    public record IsDocumentInFolderOrSubFolder(DocsPaVO.utente.InfoUtente userInfo, String idProfile, DocsPaVO.fascicolazione.Fascicolo project) : IRequest<IsDocumentInFolderOrSubFolderResult>;
    public record IsElectronicallySignedResult(bool output);

    public record IsElectronicallySigned(string docnumber, string versionId) : IRequest<IsElectronicallySignedResult>;
    public record IsEnabledConversionePdfLatoServerSincronaResult(bool output);

    public record IsEnabledConversionePdfLatoServerSincrona() : IRequest<IsEnabledConversionePdfLatoServerSincronaResult>;
    public record IsEnabledMultiMailResult(bool output);

    public record IsEnabledMultiMail(string idAmm) : IRequest<IsEnabledMultiMailResult>;
    public record IsEnabledProfilazioneAllegatiResult(bool output);

    public record IsEnabledProfilazioneAllegati() : IRequest<IsEnabledProfilazioneAllegatiResult>;

    public record isEnableIndiceSistematicoResult(bool output);

    public record isEnableIndiceSistematico() : IRequest<isEnableIndiceSistematicoResult>;
    public record IsFascicoloGeneraleResult(bool output);

    public record IsFascicoloGenerale(DocsPaVO.utente.InfoUtente userInfo, String idFascicolo) : IRequest<IsFascicoloGeneraleResult>;

    public record ModNotaInElencoResult(bool output, string message);

    public record ModNotaInElenco(InfoUtente infoUtente, NotaElenco nota) : IRequest<ModNotaInElencoResult>;

    public record DeleteNoteInElencoResult(bool output);

    public record DeleteNoteInElenco(InfoUtente infoUtente, DocsPaVO.Note.NotaElenco[] listaNote) : IRequest<DeleteNoteInElencoResult>;
    public record PopolaUnitaOrganizzativaResult(bool output);

    public record PopolaUnitaOrganizzativa(string idUO, InfoUtente infoUtente) : IRequest<PopolaUnitaOrganizzativaResult>;
    public record ProtocolloInvioNotificaAnnullaResult(bool output);

    public record ProtocolloInvioNotificaAnnulla(string idProfile, DocsPaVO.utente.Registro registro) : IRequest<ProtocolloInvioNotificaAnnullaResult>;
    public record PulisciUnitaOrganizzativaResult(bool output);

    public record PulisciUnitaOrganizzativa(string idUo, InfoUtente infoUtente) : IRequest<PulisciUnitaOrganizzativaResult>;
    public record RemoveGridResult(bool output);

    public record RemoveGrid(GridBaseInfo gridBase, InfoUtente userInfo) : IRequest<RemoveGridResult>;
    public record RemoveNotificationOfTransmissionNoWFResult(bool output);

    public record RemoveNotificationOfTransmissionNoWF(string idPeople, string idGroup, string date) : IRequest<RemoveNotificationOfTransmissionNoWFResult>;
    public record RimuoviVisibilitaProcessoResult(bool output);

    public record RimuoviVisibilitaProcesso(string idProcesso, string idGruppo, InfoUtente infoUtente) : IRequest<RimuoviVisibilitaProcessoResult>;
    public record RipristinaACLResult(bool output);

    public record RipristinaACL(DocsPaVO.documento.DirittoOggetto docDiritto, string personOrGroup, DocsPaVO.utente.InfoUtente infoUtente, string typeObject) : IRequest<RipristinaACLResult>;
    public record RipristinaFascACLResult(bool output);

    public record RipristinaFascACL(DocsPaVO.fascicolazione.DirittoOggetto fascDiritto, string personOrGroup, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<RipristinaFascACLResult>;
    public record SalvaModificaStatoStartSignatureProcessResult(bool output, ResultProcessoFirma resultAvvioProcesso);

    public record SalvaModificaStatoStartSignatureProcess(ProcessoFirma processoDiFirma, DocsPaVO.documento.FileRequest file, InfoUtente infoUtente, string modalita, string note, OpzioniNotifica opzioniNotifiche, string idStato, DiagrammaStato diagramma, string dataScadenza) : IRequest<SalvaModificaStatoStartSignatureProcessResult>;
    public record SetDataVistaSPResult(bool output);

    public record SetDataVistaSP(InfoUtente infoutente, string docNumber, string docOrFasc) : IRequest<SetDataVistaSPResult>;
    public record SmistamentoExistUOInfResult(bool output);

    public record SmistamentoExistUOInf(string idUO, MittenteSmistamento mittente) : IRequest<SmistamentoExistUOInfResult>;
    public record StartProcessoDiFirmaResult(bool output, ResultProcessoFirma resultAvvioProcesso);

    public record StartProcessoDiFirma(ProcessoFirma processoDiFirma, DocsPaVO.documento.FileRequest file, InfoUtente infoUtente, string modalita, string note, OpzioniNotifica opzioniNotifiche) : IRequest<StartProcessoDiFirmaResult>;
    public record UpdateARCHIVE_TransferPolicyResult(bool output);

    public record UpdateARCHIVE_TransferPolicy(ARCHIVE_TransferPolicy aRCHIVE_TransferPolicy) : IRequest<UpdateARCHIVE_TransferPolicyResult>;
    public record UpdateIstanzaProcessoDiFirmaResult(bool output);

    public record UpdateIstanzaProcessoDiFirma(DocsPaVO.LibroFirma.IstanzaProcessoDiFirma istanzaProcesso, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<UpdateIstanzaProcessoDiFirmaResult>;
    public record UpdatePreferredInstanceResult(bool output);

    public record UpdatePreferredInstance(string idIstanza, InfoUtente infoUtente, Ruolo ruolo) : IRequest<UpdatePreferredInstanceResult>;
    public record UpdateTipoVisibilitaProcessoResult(bool output);

    public record UpdateTipoVisibilitaProcesso(VisibilitaProcessoRuolo visibilita, InfoUtente infoUtente) : IRequest<UpdateTipoVisibilitaProcessoResult>;
    public record ValidateIstanzaConservazioneConPolicyResult(bool output);

    public record ValidateIstanzaConservazioneConPolicy(string idPolicy, string idConservazione, InfoUtente infoUtente) : IRequest<ValidateIstanzaConservazioneConPolicyResult>;
    public record verificaNomeRicercaResult(bool output);

    public record verificaNomeRicerca(string nome, InfoUtente infoUtente, string pagina, string adl) : IRequest<verificaNomeRicercaResult>;
    public record verificaNomeRicercaModificaResult(bool output);

    public record verificaNomeRicercaModifica(string nome, InfoUtente infoUtente, string pagina, string idRicerca) : IRequest<verificaNomeRicercaModificaResult>;
    public record AmmGetMailRegistroResult(CasellaRegistro[] output);

    public record AmmGetMailRegistro(string idRegistro) : IRequest<AmmGetMailRegistroResult>;
    public record CertificateGetInfoResult(CertificateInfo output);

    public record CertificateGetInfo(CertificateInfo certInfo, InfoUtente infoUtente) : IRequest<CertificateGetInfoResult>;
    public record amministrazioneGetParametroConfigurazioneResult(Configurazione output);

    public record amministrazioneGetParametroConfigurazione() : IRequest<amministrazioneGetParametroConfigurazioneResult>;
    public record AddressbookGetCorrispondenteByCodRubricaResult(DocsPaVO.utente.Corrispondente output);

    public record AddressbookGetCorrispondenteByCodRubrica(string codice, InfoUtente u, string condRegistri, bool storicizzato) : IRequest<AddressbookGetCorrispondenteByCodRubricaResult>;

    public record ValorizeInfoCorrResult(DocsPaVO.utente.Corrispondente output);

    public record ValorizeInfoCorr(DocsPaVO.utente.Corrispondente corr, string address, string city, string zipCode, string district, string country, string phone, string phone2, string fax, string taxId, string note, string place, string title, string birthPlace, string birthDay, string commercialId) : IRequest<ValorizeInfoCorrResult>;
    public record AddressbookGetRuoliUtenteIntResult(DataSet output);

    public record AddressbookGetRuoliUtenteInt(string codRubrica) : IRequest<AddressbookGetRuoliUtenteIntResult>;

    public record getDiagrammaStoricoFascResult(DataSet output);

    public record getDiagrammaStoricoFasc(string idProject) : IRequest<getDiagrammaStoricoFascResult>;
    public record isCorrInListaDistrResult(DataSet output);

    public record isCorrInListaDistr(string idCorr) : IRequest<isCorrInListaDistrResult>;
    public record GetDataRiferimentoValitaDocumentoResult(DateTime output);

    public record GetDataRiferimentoValitaDocumento(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetDataRiferimentoValitaDocumentoResult>;
    public record GetDesktopAppResult(DesktopApp output);

    public record GetDesktopApp(string AppName, InfoUtente infoUtente) : IRequest<GetDesktopAppResult>;

    public record GetTipologieIstanzeConservazioneResult(TipoIstanzaConservazione[] output);

    public record GetTipologieIstanzeConservazione() : IRequest<GetTipologieIstanzeConservazioneResult>;
    public record GetCorrespondentDetailsResult(DocsPaVO.addressbook.CorrespondentDetails output);

    public record GetCorrespondentDetails(string idCorr) : IRequest<GetCorrespondentDetailsResult>;

    public record gettDettaglioConsFascResult(DocsPaVO.areaConservazione.DettItemsConservazione[] output);

    public record gettDettaglioConsFasc(string idProject) : IRequest<gettDettaglioConsFascResult>;
    public record gettDettaglioItemsConsResult(DocsPaVO.areaConservazione.DettItemsConservazione[] output);

    public record gettDettaglioItemsCons(string idProfile) : IRequest<gettDettaglioItemsConsResult>;
    public record ConservazioneGetItemsByIdLiteResult(DocsPaVO.areaConservazione.ItemsConservazione[] output);

    public record ConservazioneGetItemsByIdLite(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<ConservazioneGetItemsByIdLiteResult>;

    public record GetPianoConservazioneByIdResult(DocsPaVO.PianoConservazione output);

    public record isEnableProtocolloTitolarioResult(string output);
    public record isEnableProtocolloTitolario() : IRequest<isEnableProtocolloTitolarioResult>;
    public record GetPianoConservazioneById(string idPianoConservazione, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetPianoConservazioneByIdResult>;

    public record GetPianoConservazioneByIdTipoAttoResult(DocsPaVO.PianoConservazione output);
    public record GetPianoConservazioneByIdTipoAtto(string idTipoAtto, DocsPaVO.utente.InfoUtente infoUtente) :IRequest<GetPianoConservazioneByIdTipoAttoResult>;
    public record InsertFascInQueueConsResult(string output);
    public record InsertFascInQueueCons(string idFasc, InfoUtente utente) : IRequest<InsertFascInQueueConsResult>;
    public record GetMaxTempoConservazioneDocumentoResult(string output);
    public record GetMaxTempoConservazioneDocumento(string idProfile, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetMaxTempoConservazioneDocumentoResult>;


    public record executeAndSaveTSR_AMResult(DocsPaVO.areaConservazione.OutputResponseMarca output);

    public record executeAndSaveTSR_AM(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.areaConservazione.InputMarca richiesta, DocsPaVO.documento.FileRequest fileRequest) : IRequest<executeAndSaveTSR_AMResult>;
    public record DocumentoGetAreaLavoroPaging1Result(DocsPaVO.areaLavoro.AreaLavoro output, int numTotPage, int nRec);

    public record DocumentoGetAreaLavoroPaging1(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo, bool enableUffRef, DocsPaVO.areaLavoro.TipoOggetto tipoObj, DocsPaVO.areaLavoro.TipoDocumento tipoDoc, DocsPaVO.areaLavoro.TipoFascicolo tipoFasc, string chaDaProto, int numPage, DocsPaVO.filtri.FiltroRicerca[][] query, string idRegistro) : IRequest<DocumentoGetAreaLavoroPaging1Result>;

    public record ConsolidateDocumentByIdResult(DocsPaVO.documento.DocumentConsolidationStateInfo output);

    public record ConsolidateDocumentById(DocsPaVO.utente.InfoUtente userInfo, string idDocument, DocsPaVO.documento.DocumentConsolidationStateEnum toState) : IRequest<ConsolidateDocumentByIdResult>;
    public record ConsolidateDocumentById_AMResult(DocsPaVO.documento.DocumentConsolidationStateInfo output);

    public record ConsolidateDocumentById_AM(DocsPaVO.utente.InfoUtente userInfo, string idDocument, DocsPaVO.documento.DocumentConsolidationStateEnum toState) : IRequest<ConsolidateDocumentById_AMResult>;
    public record DocumentoGetFileResult(DocsPaVO.documento.FileDocumento output);

    public record DocumentoGetFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<DocumentoGetFileResult>;

    public record ExportIndiceSistematicoResult(DocsPaVO.documento.FileDocumento output);

    public record ExportIndiceSistematico(DocsPaVO.amministrazione.OrgTitolario titolario) : IRequest<ExportIndiceSistematicoResult>;
    public record ExportPregressiExcelResult(DocsPaVO.documento.FileDocumento output);

    public record ExportPregressiExcel(DocsPaVO.Import.Pregressi.ReportPregressi report, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<ExportPregressiExcelResult>;
    public record FatturazioneGetPreviewPdfResult(DocsPaVO.documento.FileDocumento output);

    public record FatturazioneGetPreviewPdf(byte[] content) : IRequest<FatturazioneGetPreviewPdfResult>;
    public record GeneratePDFInSyncModResult(DocsPaVO.documento.FileDocumento output);

    public record GeneratePDFInSyncMod(DocsPaVO.documento.FileDocumento docToConvert) : IRequest<GeneratePDFInSyncModResult>;
    public record VerificaValiditaFirmaAllaDataResult(DocsPaVO.documento.FileDocumento output);

    public record VerificaValiditaFirmaAllaData(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente, DateTime dataDiVerifica) : IRequest<VerificaValiditaFirmaAllaDataResult>;
    public record DocumentoPutFileResult(DocsPaVO.documento.FileRequest output);

    public record DocumentoPutFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocument, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<DocumentoPutFileResult>;
    public record HSM_SignMultiSignSessionResult(DocsPaVO.documento.FirmaResult[] output, DocsPaVO.documento.FirmaResult esitoComplessivo);

    public record HSM_SignMultiSignSession(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest[] fileRequestList, string MultiSignToken, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool cofirma) : IRequest<HSM_SignMultiSignSessionResult>;
    public record DocumentoAddParolaChiaveResult(DocsPaVO.documento.ParolaChiave output);

    public record DocumentoAddParolaChiave(string idAmministrazione, DocsPaVO.documento.ParolaChiave parolaChiave) : IRequest<DocumentoAddParolaChiaveResult>;

    public record DocumentoGetDettaglioDocumentoNoDataVistaResult(DocsPaVO.documento.SchedaDocumento output);

    public record DocumentoGetDettaglioDocumentoNoDataVista(DocsPaVO.utente.InfoUtente infoutente, string idProfile, string docNumber) : IRequest<DocumentoGetDettaglioDocumentoNoDataVistaResult>;
    public record GetSchedaDocumentoInoltroMassivoResult(DocsPaVO.documento.SchedaDocumento output, String error);

    public record GetSchedaDocumentoInoltroMassivo(List<String> idProfiles, DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.utente.Ruolo userRole) : IRequest<GetSchedaDocumentoInoltroMassivoResult>;

    public record InfoReportMailboxResult(DocsPaVO.Interoperabilita.MailAccountCheckResponse output);

    public record InfoReportMailbox(string idCheckMailbox) : IRequest<InfoReportMailboxResult>;
    public record GetInfoProcessesStartedForDocumentResult(DocsPaVO.LibroFirma.IstanzaProcessoDiFirma[] output);

    public record GetInfoProcessesStartedForDocument(string idMainDocument) : IRequest<GetInfoProcessesStartedForDocumentResult>;
    public record FindAndReplaceRoleInModelliTrasmissioneResult(DocsPaVO.Modelli_Trasmissioni.FindAndReplaceResponse output);

    public record FindAndReplaceRoleInModelliTrasmissione(DocsPaVO.Modelli_Trasmissioni.FindAndReplaceRequest request) : IRequest<FindAndReplaceRoleInModelliTrasmissioneResult>;
    
    public record GenerateReportResult(DocsPaVO.Report.PrintReportResponse output);

    public record GenerateReport(DocsPaVO.Report.PrintReportRequest request) : IRequest<GenerateReportResult>;
    public record GetReportRegistryResult(DocsPaVO.Report.PrintReportResponse output);

    public record GetReportRegistry(String contextName) : IRequest<GetReportRegistryResult>;

    public record GetConfigurazioniRubricaComuneResult(DocsPaVO.RubricaComune.ConfigurazioniRubricaComune output);

    public record GetConfigurazioniRubricaComune(DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetConfigurazioniRubricaComuneResult>;
    public record GetRagioneNotificaResult(DocsPaVO.trasmissione.RagioneTrasmissione output);

    public record GetRagioneNotifica(string idAmm) : IRequest<GetRagioneNotificaResult>;
    public record TrasmissioneSaveExecuteTrasmAMResult(DocsPaVO.trasmissione.Trasmissione output);

    public record TrasmissioneSaveExecuteTrasmAM(string path, DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<TrasmissioneSaveExecuteTrasmAMResult>;
    public record getCanaleBySystemIdResult(DocsPaVO.utente.Canale output);

    public record getCanaleBySystemId(string idDocumenttypes) : IRequest<getCanaleBySystemIdResult>;

    public record GetPianoConservazioneByIdTipoFascResult(DocsPaVO.PianoConservazione output);
    public record GetPianoConservazioneByIdTipoFasc(string idTipoFasc, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetPianoConservazioneByIdTipoFascResult>;
    public record GetCanalePreferenzialeByIdCorrResult(DocsPaVO.utente.Canale output);

    public record GetCanalePreferenzialeByIdCorr(string system_id) : IRequest<GetCanalePreferenzialeByIdCorrResult>;

    public record AddressbookInsertCorrispondenteResult(DocsPaVO.utente.Corrispondente output);

    public record AddressbookInsertCorrispondente(DocsPaVO.utente.Corrispondente corrispondente, DocsPaVO.utente.Corrispondente parent, DocsPaVO.utente.InfoUtente iu) : IRequest<AddressbookInsertCorrispondenteResult>;
    public record GetRuoloResult(DocsPaVO.utente.Ruolo output);

    public record GetRuolo(string idCorrGlobali) : IRequest<GetRuoloResult>;

    public record GetRagioneResult(DocsPaVO.trasmissione.RagioneTrasmissione output);
    public record GetRagione(string tipoDest, string idAmm) : IRequest<GetRagioneResult>;

    public record GetRuoloEnabledAndDisabledResult(DocsPaVO.utente.Ruolo output);
    public record GetRuoloEnabledAndDisabled(string idRuolo) : IRequest<GetRuoloEnabledAndDisabledResult>;
    public record getRuoloByIdGruppoResult(DocsPaVO.utente.Ruolo output);

    public record getRuoloByIdGruppo(string idGruppo) : IRequest<getRuoloByIdGruppoResult>;
            
    public record DocumentoGetParoleChiaveResult(DocsPaVO.documento.ParolaChiave[] output);

    public record DocumentoGetParoleChiave(string idAmministrazione) : IRequest<DocumentoGetParoleChiaveResult>;
    public record GetDocumentoSmistamentoResult(DocumentoSmistamento output);

    public record GetDocumentoSmistamento(string idDocumento, InfoUtente infoUtente, bool content) : IRequest<GetDocumentoSmistamentoResult>;
    public record GetDocumentoSmistamentoAsPdfResult(DocumentoSmistamento output, bool pdfConverted);

    public record GetDocumentoSmistamentoAsPdf(string idDocumento, InfoUtente infoUtente, bool content) : IRequest<GetDocumentoSmistamentoAsPdfResult>;
    public record SpedizioneGetElementiStoricoSpedizioneResult(ElStoricoSpedizioni[] output);

    public record SpedizioneGetElementiStoricoSpedizione(string idDocument) : IRequest<SpedizioneGetElementiStoricoSpedizioneResult>;

    public record FascicolazioneGetFascicoloDaCodice2Result(Fascicolo output);

    public record FascicolazioneGetFascicoloDaCodice2(string idAmministrazione, string idGruppo, string idPeople, string codiceFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione, string idTitolario) : IRequest<FascicolazioneGetFascicoloDaCodice2Result>;
    public record createReportResult(FileDocumento output);

    public record createReport(FiltroRicerca[] filtriReport, System.Data.DataSet dataSet, string tipoRep, string titoloReport, string Sottotitolo, string reportKey, string contextName, InfoUtente infoUt) : IRequest<createReportResult>;

    public record StampaRicevutaProtocolloPdfResult(FileDocumento output);

    public record StampaRicevutaProtocolloPdf(InfoUtente infoUtente, string idDocument) : IRequest<StampaRicevutaProtocolloPdfResult>;
    public record CreateTSDVersionResult(DocsPaVO.documento.FileRequest output);

    public record CreateTSDVersion(InfoUtente infoUtente, DocsPaVO.documento.FileRequest fileRequest) : IRequest<CreateTSDVersionResult>;
    public record DocumentoAggiungiVersioneResult(DocsPaVO.documento.FileRequest output);

    public record DocumentoAggiungiVersione(DocsPaVO.documento.FileRequest fileRequest, InfoUtente infoUtente) : IRequest<DocumentoAggiungiVersioneResult>;
    public record PutElectronicSignatureMassiveResult(FirmaResult[] output);

    public record PutElectronicSignatureMassive(DocsPaVO.documento.FileRequest[] approvingFiles, InfoUtente infoUtente, bool isAdvancementProcess) : IRequest<PutElectronicSignatureMassiveResult>;
    public record StartProcessoDiFirmaMassiveResult(FirmaResult[] output);

    public record StartProcessoDiFirmaMassive(DocsPaVO.LibroFirma.ProcessoFirma processoDiFirma, DocsPaVO.documento.FileRequest[] fileRequest, InfoUtente infoUtente, string modalita, string note, OpzioniNotifica opzioniNotifiche) : IRequest<StartProcessoDiFirmaMassiveResult>;

    public record FascicolazioneNewFolderResult(Folder output, ResultCreazioneFolder result);

    public record FascicolazioneNewFolder(Folder folder, InfoUtente infoute, Ruolo ruolo) : IRequest<FascicolazioneNewFolderResult>;
    public record ModifyGrid(DocsPaVO.Grid.Grid grid, InfoUtente userInfo, string visibility, bool isPreferred) : IRequest<ModifyGridResult>;
    public record ModifyGridResult(string output);
    public record RemovePreferredTypeGrid(InfoUtente infoUser, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType) : IRequest<RemovePreferredTypeGridResult>;
    public record RemovePreferredTypeGridResult();
    public record GetGridFromSearchIdResult(Grid output);

    public record GetGridFromSearchId(InfoUtente userInfo, string searchId, GridTypeEnumeration gridType) : IRequest<GetGridFromSearchIdResult>;

    public record AddPreferredGrid(string gridId,InfoUtente infoUser, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType) : IRequest<AddPreferredGridResult>;
    public record AddPreferredGridResult();
    public record GetGridsBaseInfoResult(GridBaseInfo[] output);

    public record GetGridsBaseInfo(InfoUtente infoUtente, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType, bool allGrids) : IRequest<GetGridsBaseInfoResult>;

    public record DocumentoRimuoviVersioniDaGrigioAMResult(ImportResult output);

    public record DocumentoRimuoviVersioniDaGrigioAM(string idProfile, DocsPaVO.utente.InfoUtente infoUtente, RemoveVersionType type) : IRequest<DocumentoRimuoviVersioniDaGrigioAMResult>;
    public record InsertNotaInElencoDaExcelResult(ImportResult[] output);

    public record InsertNotaInElencoDaExcel(InfoUtente infoUtente, byte[] dati,  string nomeFile)																																				: IRequest<InsertNotaInElencoDaExcelResult>;
    public record AmmGetListAmministrazioniResult(InfoAmministrazione[] output);

    public record AmmGetListAmministrazioni() : IRequest<AmmGetListAmministrazioniResult>;

    public record DocumentoGetListaStoricoDataResult(DocumentoStoricoDataArrivo[] output);
    public record DocumentoGetListaStoricoData(string docnumber) : IRequest<DocumentoGetListaStoricoDataResult>;

    public record GetCapComuniResult(InfoComune output);

    public record GetCapComuni(string cap, string comune) : IRequest<GetCapComuniResult>;
    public record GetProvinciaComuneResult(InfoComune output);

    public record GetProvinciaComune(string comune) : IRequest<GetProvinciaComuneResult>;
    public record GetReportSpedizioniResult(InfoDocumentoSpedito[] output);

    public record GetReportSpedizioni(FiltriReportSpedizioni filters, InfoUtente infoUtente) : IRequest<GetReportSpedizioniResult>;
    public record GetReportSpedizioniDocumentiResult(InfoDocumentoSpedito[] output);

    public record GetReportSpedizioniDocumenti(FiltriReportSpedizioni filters, string[] idDocumenti, InfoUtente infoUtente) : IRequest<GetReportSpedizioniDocumentiResult>;
    public record getCountDocumentiInFolderCustomResult(int output, SearchResultInfo[] idProfiles);

    public record getCountDocumentiInFolderCustom(InfoUtente infoUtente, Folder folder, FiltroRicerca[][] filtriRicerca)																															
            : IRequest<getCountDocumentiInFolderCustomResult>;
    public record getDiagrammaAssociatoResult(int output);

    public record getDiagrammaAssociato(string idTipoDoc) : IRequest<getDiagrammaAssociatoResult>;
    public record getDiagrammaAssociatoFascResult(int output);

    public record getDiagrammaAssociatoFasc(string idTipoFasc) : IRequest<getDiagrammaAssociatoFascResult>;
    public record getDimensioneCorrenteIstanzaByte_ByIDIstanzaResult(int output);

    public record getDimensioneCorrenteIstanzaByte_ByIDIstanza(string idIstanza) : IRequest<getDimensioneCorrenteIstanzaByte_ByIDIstanzaResult>;
    public record getNumeroDocIstanza_ByIDIstanzaResult(int output);

    public record getNumeroDocIstanza_ByIDIstanza(string idIstanza) : IRequest<getNumeroDocIstanza_ByIDIstanzaResult>;
    public record Insert_UpdateARCHIVE_TransferPolicyAndProfileTypeResult(int output);

    public record Insert_UpdateARCHIVE_TransferPolicyAndProfileType(ARCHIVE_TransferPolicy aRCHIVE_TransferPolicy, bool isA, bool isP, bool isI, bool isNonProt, bool isStRegProt, bool isStRep) : IRequest<Insert_UpdateARCHIVE_TransferPolicyAndProfileTypeResult>;
    public record InsertARCHIVE_TransferPolicyResult(int output);

    public record InsertARCHIVE_TransferPolicy(ARCHIVE_TransferPolicy aRCHIVE_TransferPolicy) : IRequest<InsertARCHIVE_TransferPolicyResult>;

    public record TotalFileSizeDocumentResult(int output);

    public record TotalFileSizeDocument(string idDocumento) : IRequest<TotalFileSizeDocumentResult>;
    public record UpdateDocArrivoFromInteropResult(int output);

    public record UpdateDocArrivoFromInterop(string oldCorrId, string newCorrId) : IRequest<UpdateDocArrivoFromInteropResult>;
    public record UpdateDocArrivoFromInteropOccasionaleResult(int output);

    public record UpdateDocArrivoFromInteropOccasionale(string docId, string newCorrId) : IRequest<UpdateDocArrivoFromInteropOccasionaleResult>;
    public record StartAsyncSearchForTransferPolicyListResult(int[] output);

    public record StartAsyncSearchForTransferPolicyList(string ListSystemID) : IRequest<StartAsyncSearchForTransferPolicyListResult>;
    public record GetIstanzaProcessoDiFirmaByIdIstanzaProcessoResult(IstanzaProcessoDiFirma output);

    public record GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(string idIstanzaProcesso, InfoUtente infoUtente) : IRequest<GetIstanzaProcessoDiFirmaByIdIstanzaProcessoResult>;
    public record GetIstanzaProcessiDiFirmaByFilterResult(IstanzaProcessoDiFirma[] output, int numTotPage, int nRec, System.Data.DataSet istanzeProcessi);

    public record GetIstanzaProcessiDiFirmaByFilter(FiltroIstanzeProcessoFirma[] filtro, InfoUtente infoUtente, int numPage, int pageSize) : IRequest<GetIstanzaProcessiDiFirmaByFilterResult>;
    public record GetListDescrizioniFascicoloResult(List<DescrizioneFascicolo> output, int numTotPage, int nRec);

    public record GetListDescrizioniFascicolo(List<DocsPaVO.fascicolazione.FiltroDescrizioniFascicolo> filters, DocsPaVO.utente.InfoUtente infoUtente, int numPage, int pageSize) : IRequest<GetListDescrizioniFascicoloResult>;
    public record GetListInfoFileDocumentResult(List<DocsPaVO.documento.InfoFile> output);

    public record GetListInfoFileDocument(string idProfile, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetListInfoFileDocumentResult>;
    public record GetListaFlussoDocumentoResult(List<DocsPaVO.FlussoAutomatico.Flusso> output);

    public record GetListaFlussoDocumento(DocsPaVO.documento.SchedaDocumento schedaDoc, InfoUtente infoUtente) : IRequest<GetListaFlussoDocumentoResult>;
    public record CopiaProcessiFirmaResult(List<DocsPaVO.LibroFirma.CopiaProcessiFirmaResult> output);

    public record CopiaProcessiFirma(List<DocsPaVO.LibroFirma.ProcessoFirma> processiFirma, bool copiaVisibilita, bool mantieniInRuoloOrigine, string idRuoloDest, string idPeopleDest, DocsPaVO.utente.InfoUtente utenteOrigine) : IRequest<CopiaProcessiFirmaResult>;
    public record GetElectronicSignatureDocumentResult(List<DocsPaVO.LibroFirma.FirmaElettronica> output);

    public record GetElectronicSignatureDocument(string docnumber, string versionId, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetElectronicSignatureDocumentResult>;
    public record GetIstanzaProcessoDiFirmaByDocnumberResult(List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> output);

    public record GetIstanzaProcessoDiFirmaByDocnumber(string docnumber, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<GetIstanzaProcessoDiFirmaByDocnumberResult>;
    public record GetTrasmissioniPendentiConWorkflowResult(List<DocsPaVO.trasmissione.InfoTrasmissione> output, List<string> idTrasmsSingola, DocsPaVO.ricerche.SearchPagingContext pagingContext);

    public record GetTrasmissioniPendentiConWorkflow(string idDocOrFasc, string docOrFasc, string idRuoloInUO, string idPeople, SearchPagingContext pagingContext) : IRequest<GetTrasmissioniPendentiConWorkflowResult>;
    public record GetMailCorrEsternoResult(List<DocsPaVO.utente.MailCorrispondente> output);

    public record GetMailCorrEsterno(string idCorrispondente) : IRequest<GetMailCorrEsternoResult>;
    public record GetMessaggiSuccessiviFlussoProceduraleResult(Messaggio[] output);

    public record GetMessaggiSuccessiviFlussoProcedurale(SchedaDocumento schedaDocumento, InfoUtente infoUtente) : IRequest<GetMessaggiSuccessiviFlussoProceduraleResult>;

    public record ricercaNotificaResult(Notifica[] output);

    public record ricercaNotifica(string docNumber) : IRequest<ricercaNotificaResult>;
    public record GetElementiRubricaResult(object[] output);

    public record GetElementiRubrica(ParametriRicercaRubrica qc, InfoUtente infoUtente) : IRequest<GetElementiRubricaResult>;
    public record DocumentoAddOggettoResult(Oggetto output, string errMsg);

    public record DocumentoAddOggetto(InfoUtente infoUtente, Oggetto oggetto, DocsPaVO.utente.Registro registro) : IRequest<DocumentoAddOggettoResult>;
    public record AmmGetRegistroResult(OrgRegistro output);

    public record AmmGetRegistro(string idRegistro) : IRequest<AmmGetRegistroResult>;
    public record executeAndSaveTSRResult(OutputResponseMarca output);

    public record executeAndSaveTSR(InfoUtente infoUtente, InputMarca richiesta, DocsPaVO.documento.FileRequest fileRequest) : IRequest<executeAndSaveTSRResult>;
    public record GetListaPolicyResult(Policy[] output);

    public record GetListaPolicy(int idAmm, string tipo) : IRequest<GetListaPolicyResult>;
    public record ProcessBarcodeFormPdfResult(ProcessFormOutput output);

    public record ProcessBarcodeFormPdf(InfoUtente infoUtente, ProcessFormInput processFormInput) : IRequest<ProcessBarcodeFormPdfResult>;
    public record ProcessFormPdfResult(ProcessFormOutput output);

    public record ProcessFormPdf(InfoUtente infoUtente, ProcessFormInput processFormInput) : IRequest<ProcessFormPdfResult>;
    public record GetProcessoDiFirmaResult(ProcessoFirma output);

    public record GetProcessoDiFirma(string idProcesso, InfoUtente infoUtente) : IRequest<GetProcessoDiFirmaResult>;
    public record GetProcessesSignatureVisibleRoleResult(ProcessoFirma[] output);

    public record GetProcessesSignatureVisibleRole(bool asProponente, bool asMonitoratore, bool includiVisibilitaRimosse, InfoUtente infoUtente) : IRequest<GetProcessesSignatureVisibleRoleResult>;

    public record SmistamentoGetRagioniTrasmissioneResult(RagioneTrasmissione[] output);

    public record SmistamentoGetRagioniTrasmissione(string idAmm) : IRequest<SmistamentoGetRagioniTrasmissioneResult>;

    public record getDettaglioListReportFormatiConservazioneByIdConsResult(ReportFormatiConservazione[] output);

    public record getDettaglioListReportFormatiConservazioneByIdCons(string idIstanzaCons) : IRequest<getDettaglioListReportFormatiConservazioneByIdConsResult>;
    public record AddMassiveObjectInADLResult(ResultAddAreaLavoro[] output);

    public record AddMassiveObjectInADL(WorkingArea[] listAreaLavoro, InfoUtente infoUtente) : IRequest<AddMassiveObjectInADLResult>;
    public record GetRoleHistoryResult(RoleHistoryResponse output);

    public record GetRoleHistory(RoleHistoryRequest request) : IRequest<GetRoleHistoryResult>;
    public record getRuoliUoSmistamentoResult(RuoloSmistamento[] output);

    public record getRuoliUoSmistamento(string idUo) : IRequest<getRuoliUoSmistamentoResult>;

    public record DocumentoSetFlagDaInviareResult(SchedaDocumento output);

    public record DocumentoSetFlagDaInviare(SchedaDocumento schedaDocumento) : IRequest<DocumentoSetFlagDaInviareResult>;
    public record ModificaRicercaResult(SearchItem output);

    public record ModificaRicerca(SearchItem item, InfoUtente infoUtente, bool customGrid, string gridId, Grid tempGrid, GridTypeEnumeration gridType) : IRequest<ModificaRicercaResult>;
    public record RecuperaRicercaResult(SearchItem output);

    public record RecuperaRicerca(int id) : IRequest<RecuperaRicercaResult>;
    public record SalvaRicercaResult(SearchItem output);

    public record SalvaRicerca(SearchItem item, InfoUtente infoUtente, bool ADL, bool customGrid, string gridId, Grid tempGrid, GridTypeEnumeration gridType) : IRequest<SalvaRicercaResult>;
    public record GetSpedizioneDocumentoResult(SpedizioneDocumento output);

    public record GetSpedizioneDocumento(InfoUtente infoUtente, SchedaDocumento documento) : IRequest<GetSpedizioneDocumentoResult>;
    public record SpedisciDocumentoResult(SpedizioneDocumento output);

    public record SpedisciDocumento(InfoUtente infoUtente, SchedaDocumento documento, SpedizioneDocumento infoSpedizione) : IRequest<SpedisciDocumentoResult>;
    public record GetListaSpedizioniResult(StatoInvio[] output);

    public record GetListaSpedizioni(string idProfile) : IRequest<GetListaSpedizioniResult>;
    public record GetStatoTrasmissioneUtenteResult(StatoTrasmissioneUtente output);

    public record GetStatoTrasmissioneUtente(InfoUtente infoUtente, string idTrasmissioneUtente) : IRequest<GetStatoTrasmissioneUtenteResult>;
    public record DocumentoGetStoriaVisibilitaResult(StoriaDirittoDocumento[] output);

    public record DocumentoGetStoriaVisibilita(string idProfile, string tipoObj, InfoUtente infoUtente) : IRequest<DocumentoGetStoriaVisibilitaResult>;

    public record AllegatoIsPECResult(string output);

    public record AllegatoIsPEC(string version_id) : IRequest<AllegatoIsPECResult>;

    public record DocumentoExecAddConservazioneAMResult(string output);

    public record DocumentoExecAddConservazioneAM(string idProfile, string idProject, string docNumber, DocsPaVO.utente.InfoUtente infoUtente, string tipoOggetto) : IRequest<DocumentoExecAddConservazioneAMResult>;
    public record DocumentoExecAddConservazioneAM_WithConstraintsResult(string output);

    public record DocumentoExecAddConservazioneAM_WithConstraints(string idProfile, string idProject, string docNumber, DocsPaVO.utente.InfoUtente infoUtente, string tipoOggetto, bool numDocIstanzaViolato, bool dimIstanzaViolato, int vincoloDimIstanza, int vincoloNumDocIstanza, int sizeItem) : IRequest<DocumentoExecAddConservazioneAM_WithConstraintsResult>;
    public record filtroRicercaTitDocspaResult(string output);

    public record filtroRicercaTitDocspa(string codice, string descrizione, string note, string indice, string idAmm, string idGruppo, string idRegistro, string idTitolario) : IRequest<filtroRicercaTitDocspaResult>;
    public record findNodoRootResult(string output);

    public record findNodoRoot(string idrecord, string idparent, int livello) : IRequest<findNodoRootResult>;

    public record GetDispositivoStampaUtenteResult(string output);

    public record GetDispositivoStampaUtente(string idPeople) : IRequest<GetDispositivoStampaUtenteResult>;

    public record getIdAmmByCodResult(string output);

    public record getIdAmmByCod(string codiceAmministrazione) : IRequest<getIdAmmByCodResult>;

    public record GetLastSevenDayResult(string output);

    public record GetLastSevenDay() : IRequest<GetLastSevenDayResult>;
    public record GetMailPrincipaleRegistroResult(string output);

    public record GetMailPrincipaleRegistro(string idRegistro) : IRequest<GetMailPrincipaleRegistroResult>;
    public record getModelloSystemIdResult(string output);

    public record getModelloSystemId() : IRequest<getModelloSystemIdResult>;
    
    public record getPrimaIstanzaAreaConsResult(string output);

    public record getPrimaIstanzaAreaCons(string idPeople, string idGruppo) : IRequest<getPrimaIstanzaAreaConsResult>;
    public record GetRapportoVersamentoResult(string output);

    public record GetRapportoVersamento(string idDoc, InfoUtente utente) : IRequest<GetRapportoVersamentoResult>;

    public record GetRepertorioStateResult(GetRepertorioStateResponse output);
    public record GetRepertorioState(GetRepertorioStateRequest request) : IRequest<GetRepertorioStateResult>;

    public record GetSegnaturaRepertorioResult(string output);

    public record GetSegnaturaRepertorio(string docnumber, string codiceAmm) : IRequest<GetSegnaturaRepertorioResult>;

    public record getSegnaturaRepertorioNoHTMLResult(string output);

    public record getSegnaturaRepertorioNoHTML(string docnumber, string codiceAmm) : IRequest<getSegnaturaRepertorioNoHTMLResult>;

    public record getSetRicevutaPecResult(string output);

    public record getSetRicevutaPec(string idRegistro, string ricevutaPecDefault, string ricevutaPecOneTime, bool getData, string addressMail) : IRequest<getSetRicevutaPecResult>;
    public record getStatoConservazioneResult(string output);

    public record getStatoConservazione(string idDoc) : IRequest<getStatoConservazioneResult>;
    public record GetTipoFirmaDocumentoResult(string output);

    public record GetTipoFirmaDocumento(string docnumber) : IRequest<GetTipoFirmaDocumentoResult>;
    public record GetYesterdayResult(string output);

    public record GetYesterday() : IRequest<GetYesterdayResult>;
    public record HSM_OpenMultiSignSessionResult(string output);

    public record HSM_OpenMultiSignSession(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest[] fileRequestList, bool cofirma, bool timestamp, string TipoFirma) : IRequest<HSM_OpenMultiSignSessionResult>;
    public record InsertDocInQueueConsResult(string output);

    public record InsertDocInQueueCons(string idDoc, InfoUtente utente) : IRequest<InsertDocInQueueConsResult>;
    public record NodoTitolarioSecurityResult(string output);

    public record NodoTitolarioSecurity(string idAmm, string idParent, string idGruppo, string idRegistro, string idTitolario) : IRequest<NodoTitolarioSecurityResult>;
    public record recuperoStatoConsResult(string output);

    public record recuperoStatoCons(string idDoc, InfoUtente utente) : IRequest<recuperoStatoConsResult>;
    public record salvaModelloResult(string output);

    public record salvaModello(ModelloTrasmissione modelloTrasmissione, InfoUtente infoUtente) : IRequest<salvaModelloResult>;
    public record SaveNewGridResult(String output);

    public record SaveNewGrid(DocsPaVO.Grid.Grid grid, InfoUtente userInfo, String gridName, String visibility, Boolean isPreferred) : IRequest<SaveNewGridResult>;

    public record Albo_GetFileDaPubblicareResult(string[] output);

    public record Albo_GetFileDaPubblicare(string idDocPrincipale, string option) : IRequest<Albo_GetFileDaPubblicareResult>;
    
    public record GetIdDestinatariTrasmDocInUoResult(string[] output);

    public record GetIdDestinatariTrasmDocInUo(string idUo, string docnumber) : IRequest<GetIdDestinatariTrasmDocInUoResult>;
    public record HSM_GetMementoForUserResult(string[] output);

    public record HSM_GetMementoForUser(InfoUtente infoUtente) : IRequest<HSM_GetMementoForUserResult>;
    public record SmistaGetRegistriRuoloResult(string[] output);

    public record SmistaGetRegistriRuolo(string idRuolo) : IRequest<SmistaGetRegistriRuoloResult>;
    public record AmmGetRegistriResult(OrgRegistro[] output);

    public record AmmGetRegistri(string codiceAmministrazione, string chaRF) : IRequest<AmmGetRegistriResult>;
    public record DocumentoGetListaStoriciOggettoResult(StoricoOggetto[] output);

    public record DocumentoGetListaStoriciOggetto(string idProfile) : IRequest<DocumentoGetListaStoriciOggettoResult>;
    public record GetBaseInfoForDocumentResult(System.Collections.Generic.List<DocsPaVO.documento.BaseInfoDoc> output);

    public record GetBaseInfoForDocument(string idProfile, string docNumber, string versionNumber) : IRequest<GetBaseInfoForDocumentResult>;
    public record GetAmmRightMailRegistroResult(System.Data.DataSet output);

    public record GetAmmRightMailRegistro(string idRegistro, string idRuoloInUO) : IRequest<GetAmmRightMailRegistroResult>;
    public record getAttributiTipoFascResult(Templates output);

    public record getAttributiTipoFasc(InfoUtente infoUtente, string idTipoFasc) : IRequest<getAttributiTipoFascResult>;

    public record getTipoFascResult(Templates[] output);

    public record getTipoFasc(string idAmministrazione) : IRequest<getTipoFascResult>;

    public record ARCHIVE_GetTipologiaAttoResult(TipologiaAtto[] output);

    public record ARCHIVE_GetTipologiaAtto(string idAmministrazione) : IRequest<ARCHIVE_GetTipologiaAttoResult>;

    public record getTipoAttoPDInsRicWithStateDiagramResult(TipologiaAtto[] output);

    public record getTipoAttoPDInsRicWithStateDiagram(string idAmministrazione, string idGruppo, string diritti) : IRequest<getTipoAttoPDInsRicWithStateDiagramResult>;
    public record getTipoNotificaResult(TipoNotifica output);

    public record getTipoNotifica(string systemIdTipoNotifica) : IRequest<getTipoNotificaResult>;
    public record CheckExistsRoleSupByTypeRolesResult(TipoRuolo[] output);

    public record CheckExistsRoleSupByTypeRoles(TipoRuolo[] typeRole, InfoUtente infoUtente) : IRequest<CheckExistsRoleSupByTypeRolesResult>;
    public record GetStatoElementoInLibroFirmaResult(TipoStatoElemento output);

    public record GetStatoElementoInLibroFirma(string istanzaPassoFirma, InfoUtente infoUtente) : IRequest<GetStatoElementoInLibroFirmaResult>;
    public record GetTrasmissioneByIdTrasmSingResult(Trasmissione output);

    public record GetTrasmissioneByIdTrasmSing(OggettoTrasm oggettoTrasmesso, string idTrasmSing, Utente utente, Ruolo ruolo) : IRequest<GetTrasmissioneByIdTrasmSingResult>;
    public record GetUOInferioriResult(UOSmistamento[] output);

    public record GetUOInferiori(string idUOAppartenenza, MittenteSmistamento mittente) : IRequest<GetUOInferioriResult>;
    public record getUserInRoleByIdGruppoResult(Utente[] output);

    public record getUserInRoleByIdGruppo(string idGruppo) : IRequest<getUserInRoleByIdGruppoResult>;
    public record getUtentiRuoloSmistamentoResult(UtenteSmistamento[] output);

    public record getUtentiRuoloSmistamento(string id, string queryParam) : IRequest<getUtentiRuoloSmistamentoResult>;
    public record AmmUpdateMailRegistroResult(ValidationResultInfo output);

    public record AmmUpdateMailRegistro(string idRegistro, CasellaRegistro[] caselle) : IRequest<AmmUpdateMailRegistroResult>;
    public record AmmUpdateRegistroResult(ValidationResultInfo output, OrgRegistro registro);

    public record AmmUpdateRegistro(OrgRegistro registro) : IRequest<AmmUpdateRegistroResult>;
    public record GetVisibilitaProcessoResult(VisibilitaProcessoRuolo[] output);

    public record GetVisibilitaProcesso(string idProcesso, FiltroProcessoFirma[] filtroRicerca, InfoUtente infoUtente) : IRequest<GetVisibilitaProcessoResult>;
    public record AnnullaContatoreDiRepertorioResult();

    public record AnnullaContatoreDiRepertorio(string idOggetto, string docNumber, InfoUtente infoUtente) : IRequest<AnnullaContatoreDiRepertorioResult>;
    public record ChangeStateTransmissionsMissingRolesResult();

    public record ChangeStateTransmissionsMissingRoles(string[] missingRoles, InfoUtente infoUtente, InfoFascicolo infoFascicolo, string idStato) : IRequest<ChangeStateTransmissionsMissingRolesResult>;
    public record EnqueueServerPdfConversionAMResult();

    public record EnqueueServerPdfConversionAM(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.ObjServerPdfConversion objServerPdfConversion) : IRequest<EnqueueServerPdfConversionAMResult>;
    public record getDatiAggiuntiviUtenteSmistamentoResult(UtenteSmistamento utenteSmistamento);

    public record getDatiAggiuntiviUtenteSmistamento(UtenteSmistamento utenteSmistamento, string idUtente) : IRequest<getDatiAggiuntiviUtenteSmistamentoResult>;
    public record rubricaCheckChildrenExistenceExResult(DocsPaVO.rubrica.ElementoRubrica[] ers);

    public record rubricaCheckChildrenExistenceEx(DocsPaVO.rubrica.ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti, DocsPaVO.utente.InfoUtente u) : IRequest<rubricaCheckChildrenExistenceExResult>;

    public record StoricizzaResult();

    public record Storicizza(DocsPaVO.ProfilazioneDinamica.Storicizzazione storico) : IRequest<StoricizzaResult>;

    public record AvvioProcessoDiFirmaResult(bool output, ResultProcessoFirma resultAvvioProcesso);

    public record AvvioProcessoDiFirma(ProcessoFirma processoDiFirma, DocsPaVO.documento.FileRequest file, InfoUtente infoUtente, string modalita, string note, OpzioniNotifica opzioniNotifiche, bool daCambioStato = false) : IRequest<AvvioProcessoDiFirmaResult>;

    public record GetFunzioniRuoloResult(Funzione[] funzioni);

    public record GetFunzioniRuolo(string idCorrGlobali) : IRequest<GetFunzioniRuoloResult>;

    public record DuplicaProcessoFirmaResult(DocsPaVO.LibroFirma.ProcessoFirma output, DocsPaVO.LibroFirma.ResultProcessoFirma resultCreazioneProcesso);
    public record DuplicaProcessoFirma(DocsPaVO.LibroFirma.ProcessoFirma processoOld, string nomeNuovoProcesso, bool copiaVisibilita, DocsPaVO.utente.InfoUtente utente) : IRequest<DuplicaProcessoFirmaResult>;

    public record GetReportSpedizioniSearchResult(InfoDocumentoSpedito[] output);
    public record GetReportSpedizioniSearch(FiltriReportSpedizioni filters, string idGruppo, bool allReceivers) : IRequest<GetReportSpedizioniSearchResult>;

    public record AmmGetListRegistriRFResult(OrgRegistro[] output);
    public record AmmGetListRegistriRF(string idAmm, string idRuolo, string chaRF) : IRequest<AmmGetListRegistriRFResult>;

    public record getApplicationNameResult(string output);
    public record getApplicationName() : IRequest<getApplicationNameResult>;

    public record GetSupportedFileTypeResult(SupportedFileType output);
    public record GetSupportedFileType(int idAmministrazione, string fileExtension) : IRequest<GetSupportedFileTypeResult>;

    public record UpdateOggettoResult(bool output);
    public record UpdateOggetto(InfoUtente infoUtente, Oggetto oggetto) : IRequest<UpdateOggettoResult>;

    public record SmistaDocumentoResult(DocsPaVO.Smistamento.EsitoSmistamentoDocumento[] output);

    public record SmistaDocumento(DocsPaVO.Smistamento.MittenteSmistamento mittente,
            DocsPaVO.utente.InfoUtente infoUtente,
            DocsPaVO.Smistamento.DocumentoSmistamento documentoTrasmesso,
            DocsPaVO.Smistamento.DatiTrasmissioneDocumento datiTrasmissioneDocumento,
            DocsPaVO.Smistamento.UOSmistamento uoAppartenenza,
            DocsPaVO.Smistamento.UOSmistamento[] uoInferiori,
            string httpFullPath) : IRequest<SmistaDocumentoResult>;

    public record AcceptTransmissionsResult(bool output);

    public record AcceptTransmissions(
        List<string> idTrasmSingole, 
        string noteAccettazione, 
        DocsPaVO.utente.Ruolo ruolo, 
        DocsPaVO.utente.InfoUtente infoUtente)
        : IRequest<AcceptTransmissionsResult>;

    public record AppendContentFirmatoResult(bool output, DocsPaVO.documento.FileRequest fileRequest);

    public record AppendContentFirmato(byte[] sigedContent,
            bool cofirma,
            DocsPaVO.documento.FileRequest fileRequest,
            DocsPaVO.utente.InfoUtente infoUtente)
        : IRequest<AppendContentFirmatoResult>;

    public record AppendDocumentoFirmatoResult(
        bool output,
        DocsPaVO.documento.FileRequest fileRequest);

    public record AppendDocumentoFirmato(
            string base64content,
            bool cofirma,
            DocsPaVO.documento.FileRequest fileRequest,
            DocsPaVO.utente.InfoUtente infoUtente,
            bool idPades = false)
        : IRequest<AppendDocumentoFirmatoResult>;

    public record CheckInDocumentResult(DocsPaVO.Validations.ValidationResultInfo output);

    public record CheckInDocument(
        DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, DocsPaVO.utente.InfoUtente checkOutOwner, byte[] content, string checkInComments)
        : IRequest<CheckInDocumentResult>;

    public record CheckOutDocumentWithFileResult(
        DocsPaVO.Validations.ValidationResultInfo output,
        DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, 
        byte[] content);

    public record CheckOutDocumentWithFile(
        string idDocument, string documentNumber, string documentLocation, string machineName, DocsPaVO.utente.InfoUtente utente)
        : IRequest<CheckOutDocumentWithFileResult>;

    public record CheckOutDocumentResult(
        DocsPaVO.Validations.ValidationResultInfo output,
        DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus);

    public record CheckOutDocument(string idDocument, string documentNumber, string documentLocation, string machineName, DocsPaVO.utente.InfoUtente utente)
        : IRequest<CheckOutDocumentResult>;

    public record CreateZipFromReportResult(
        byte[] output);

    public record CreateZipFromReport(ResultsContainer report, InfoUtente infoUtente)
        : IRequest<CreateZipFromReportResult>;

    public record DocumentoAnnullaPredisposizioneResult(bool output);

    public record DocumentoAnnullaPredisposizione(
        DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        : IRequest<DocumentoAnnullaPredisposizioneResult>;

    public record DO_GetListaStoriciMittenteResult(DocsPaVO.documento.StoricoMittente[] output);

    public record DO_GetListaStoriciMittente(string idProfile, string tipo)
        : IRequest<DO_GetListaStoriciMittenteResult>;

    public record DeleteFileFormazioneResult(bool output);

    public record DeleteFileFormazione(string idUO, InfoUtente infoUtente)
        : IRequest<DeleteFileFormazioneResult>;

    public record DeletePersonalFileResult(bool output);

    public record DeletePersonalFile(DocsPaVO.documento.FileDocumento fileDocument, string repositoryId, InfoUtente infoUtente)
        : IRequest<DeletePersonalFileResult>;

    public record DocumentoPutFileImportResult(DocsPaVO.documento.FileRequest output, string errorMsg);

    public record DocumentoPutFileImport(
        DocsPaVO.documento.FileRequest fileRequest, 
        DocsPaVO.documento.FileDocumento fileDocument, 
        DocsPaVO.utente.InfoUtente infoUtente)
        : IRequest<DocumentoPutFileImportResult>;

    public record ExportRicercaTrasmResult(DocsPaVO.documento.FileDocumento output);

    public record ExportRicercaTrasm(DocsPaVO.trasmissione.OggettoTrasm oggettoTrasmesso, string tipoRicerca, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.filtri.FiltroRicerca[] listaFiltri, string exportType, string title, DocsPaVO.utente.InfoUtente infoUtente, ArrayList campiSelezionati)
        : IRequest<ExportRicercaTrasmResult>;

    public record ExportRicercaFascCustomResult(DocsPaVO.documento.FileDocumento output);

    public record ExportRicercaFascCustom(DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, bool enableChilds, DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.filtri.FiltroRicerca[][] filtri, string exportType, string title, ArrayList campiSelezionati, String[] idProjectsList, DocsPaVO.Grid.Grid grid, bool gridPersonalization, Field[] visibleFieldsTemplate, bool security)
        : IRequest<ExportRicercaFascCustomResult>;

    public record ExportRubricaWithTitleNewResult(DocsPaVO.documento.FileDocumento output);

    public record ExportRubricaWithTitleNew(DocsPaVO.utente.InfoUtente infoUtente, bool store, string registri, string title, string tipologia)
        : IRequest<ExportRubricaWithTitleNewResult>;

    public record ExportDocCustomResult(DocsPaVO.documento.FileDocumento output);

    public record ExportDocCustom(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] filtri, string exportType, string title, DocsPaVO.ricerche.FullTextSearchContext context, ArrayList campiSelezionati, String[] documentsSystemId, DocsPaVO.Grid.Grid grid, bool gridPersonalization, Field[] visibleFieldsTemplate)
            : IRequest<ExportDocCustomResult>;

    public record ExportSearchAddressBookNewResult(DocsPaVO.documento.FileDocumento output);

    public record ExportSearchAddressBookNew(DocsPaVO.utente.InfoUtente infoUtente, bool store, DocsPaVO.rubrica.ParametriRicercaRubrica qr, string title, string tipologia)
        : IRequest<ExportSearchAddressBookNewResult>;

    public record FattElAttiveDaImportResult(string output);

    public record FattElAttiveDaImport(string idDoc, InfoUtente infoUt)
        : IRequest<FattElAttiveDaImportResult>;

    public record FinalizeUploadBigFileResult(bool output, DocsPaVO.documento.FileRequest fileRequest);

    public record FinalizeUploadBigFile(
        DocsPaVO.documento.FileRequest fileRequest,
        DocsPaVO.documento.FileDocumento fileDocument,
        DocsPaVO.utente.InfoUtente infoUtente,
        string uuid, string fileName, bool convertPdfSync)
            : IRequest<FinalizeUploadBigFileResult>;

    public record GetCheckedOutFileDocumentResult(byte[] output);

    public record GetCheckedOutFileDocument(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, DocsPaVO.utente.InfoUtente checkOutOwner)
                : IRequest<GetCheckedOutFileDocumentResult>;

    public record GetCheckOutDocumentModelTypesResult(string[] output);

    public record GetCheckOutDocumentModelTypes(int idAdmin)
        : IRequest<GetCheckOutDocumentModelTypesResult>;

    public record getConfigurazioneCacheResult(DocsPaVO.Caching.CacheConfig output);

    public record getConfigurazioneCache(string idAmministrazione)
        : IRequest<getConfigurazioneCacheResult>;
     
    public record getCorrispondentiByCodRFIdAmmResult(DocsPaVO.utente.Corrispondente[] output);
    
    public record getCorrispondentiByCodRFIdAmm(string codiceRF, string idAmm)
        : IRequest<getCorrispondentiByCodRFIdAmmResult>;

    public record GetDocumentListVersionsLiteResult(DocsPaVO.documento.Documento[] output);

    public record GetDocumentListVersionsLite(DocsPaVO.utente.InfoUtente infoutente, string idProfile, string docNumber)
        : IRequest<GetDocumentListVersionsLiteResult>;

    public record GetCurrentModelProcessorResult(DocsPaVO.Modelli.ModelProcessorInfo output);

    public record GetCurrentModelProcessor(DocsPaVO.utente.InfoUtente infoUtente)
        : IRequest<GetCurrentModelProcessorResult>;

    public record GetDocumentModelAsXmlResult(string output);

    public record GetDocumentModelAsXml(DocsPaVO.Modelli.ModelRequest modelRequest)
        : IRequest<GetDocumentModelAsXmlResult>;

    public record GetDocumentTabResult(DocsPaVO.documento.Tab output);
    
    public record GetDocumentTab(string documentId, InfoUtente infoUser)
        : IRequest<GetDocumentTabResult>;

    public record getElementiRubricaVeloceResult(string[] output);

    public record getElementiRubricaVeloce(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.rubrica.ParametriRicercaRubrica qco)
            : IRequest<getElementiRubricaVeloceResult>;

    public record GetFatturaXMLResult(string output);

    public record GetFatturaXML(DocsPaVO.utente.InfoUtente infoUtente, string idFattura)
        : IRequest<GetFatturaXMLResult>;

    public record GetFileDocumentoResult(DocsPaVO.ExportFascicolo.ContentDocumento output);

    public record GetFileDocumento(InfoUtente UserInfo, string docNumber)
        : IRequest<GetFileDocumentoResult>;

    public record getLuceneDocumentsResult(DocsPaVO.Grids.SearchObject[] result, int nRec);

    public record getLuceneDocuments(List<string> docNumbers, int numPage, DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.Grid.Field[] visibleFieldsTemplate)
        : IRequest<getLuceneDocumentsResult>;

    public record GetInfoFascicoloAsXmlResult(string output);
    
    public record GetInfoFascicoloAsXml(string requestAsXml)
        : IRequest<GetInfoFascicoloAsXmlResult>;

    public record GetListaRuoliUtenteResult(DocsPaVO.utente.Ruolo[] output);
    
    public record GetListaRuoliUtente(string idPeople)
        : IRequest<GetListaRuoliUtenteResult>;

    public record GetModelProcessorsResult(DocsPaVO.Modelli.ModelProcessorInfo[] output);

    public record GetModelProcessors(DocsPaVO.utente.InfoUtente infoUtente)
        : IRequest<GetModelProcessorsResult>;

    public record GetReportPregressiResult(DocsPaVO.Import.Pregressi.ReportPregressi output);

    public record GetReportPregressi(string sysId, bool getItems)
        : IRequest<GetReportPregressiResult>;

    public record GetReportProcedimentoResult(DocsPaVO.Procedimento.Report.ReportProcedimentoResponse output);

    public record GetReportProcedimento(DocsPaVO.Procedimento.Report.ReportProcedimentoRequest request) 
        : IRequest<GetReportProcedimentoResult>;

    public record GetReportsResult(DocsPaVO.Import.Pregressi.ReportPregressi[] output);

    public record GetReports(bool getItems, InfoUtente infoUtente, bool daAmministrazione)
        : IRequest<GetReportsResult>;

    public record getSha256Result(DocsPaVO.documento.MassSignature output);

    public record getSha256(DocsPaVO.documento.MassSignature massSignature, DocsPaVO.utente.InfoUtente infoUtente)
        : IRequest<getSha256Result>;

    public record GetTipoAttoByRuoloAndPianoConservazioneResult(DocsPaVO.documento.TipologiaAtto[] output);
    
    public record GetTipoAttoByRuoloAndPianoConservazione(string idAmministrazione, string idGruppo, string diritti, DocsPaVO.fascicolazione.Fascicolo fascicolo, string idProfile)
        : IRequest<GetTipoAttoByRuoloAndPianoConservazioneResult>;

    public record GetTipologiaFascicoloByRuoloAndPianoConservazioneResult(DocsPaVO.ProfilazioneDinamica.Templates[] output);

    public record GetTipologiaFascicoloByRuoloAndPianoConservazione(string idAmministrazione, string idRuolo, string diritti, string idPianoConservazione)
        : IRequest<GetTipologiaFascicoloByRuoloAndPianoConservazioneResult>;

    public record GetUOAppartenenzaResult(DocsPaVO.Smistamento.UOSmistamento output);

    public record GetUOAppartenenza(string idUnitaOrganizzativa, DocsPaVO.Smistamento.MittenteSmistamento mittente, bool isCurrentUO)
        : IRequest<GetUOAppartenenzaResult>;

    public record GetUploadedFilesResult(DocsPaVO.UploadFiles.FileInUpload[] output);

    public record GetUploadedFiles(InfoUtente infoUtente)
        : IRequest<GetUploadedFilesResult>;

    public record ImportAndAcquireDocumentResult(
        DocsPaVO.PrjDocImport.ImportResult output,
        DocsPaVO.PrjDocImport.ResultsContainer resultsContainer);

    public record ImportAndAcquireDocument(
            DocsPaVO.PrjDocImport.DocumentRowData documentToImport,
            ProtoType protoType,
            string serverPath,
            DocsPaVO.utente.InfoUtente userInfo,
            DocsPaVO.utente.Ruolo role,
            bool isProfilationRequired,
            bool isRapidClassificationRequired,
            bool isSmistamentoEnabled,
            DocsPaVO.PrjDocImport.ResultsContainer resultsContainer,
            bool isEnabledPregressi)
        : IRequest<ImportAndAcquireDocumentResult>;


    public record ImportDocumentResult(
    DocsPaVO.PrjDocImport.ImportResult output,
    DocsPaVO.PrjDocImport.ResultsContainer resultsContainer);

    public record ImportDocument(
            DocsPaVO.PrjDocImport.DocumentRowData documentToImport,
            ProtoType protoType,
            string serverPath,
            DocsPaVO.utente.InfoUtente userInfo,
            DocsPaVO.utente.Ruolo role,
            bool isProfilationRequired,
            bool isRapidClassificationRequired,
            bool isSmistamentoEnabled,
            DocsPaVO.PrjDocImport.ResultsContainer resultsContainer,
            bool isEnabledPregressi)
        : IRequest<ImportDocumentResult>;

    public record ImportDocResult(
    DocsPaVO.PrjDocImport.ImportResult output,
    DocsPaVO.PrjDocImport.ResultsContainer resultsContainer);

    public record ImportDoc(
            DocsPaVO.PrjDocImport.DocumentRowData documentToImport,
            ProtoType protoType,
            string serverPath,
            DocsPaVO.utente.InfoUtente userInfo,
            DocsPaVO.utente.Ruolo role,
            bool isProfilationRequired,
            bool isRapidClassificationRequired,
            bool isSmistamentoEnabled,
            DocsPaVO.PrjDocImport.ResultsContainer resultsContainer,
            bool isEnabledPregressi,
            bool isImportAndAcquire)
        : IRequest<ImportDocResult>;



    public record ImportProjectsResult(DocsPaVO.PrjDocImport.ImportResult[] output);

    public record ImportProjects(byte[] content,
                    string fileName,
                    string serverPath,
                    DocsPaVO.utente.InfoUtente userInfo,
                    DocsPaVO.utente.Ruolo role,
                    bool isSmistamentoEnabled)
        : IRequest<ImportProjectsResult>;

    public record ImportRDEResult(DocsPaVO.PrjDocImport.ResultsContainer output);

    public record ImportRDE(
            byte[] content,
            string fileName,
            string serverPath,
            DocsPaVO.utente.InfoUtente userInfo,
            DocsPaVO.utente.Ruolo role,
            bool isRapidClassificationRequired,
            bool isSmistamentoEnabled,
            int versionNumber)
        : IRequest<ImportRDEResult>;

    public record InsertFirmatarioDocumentoResult(bool output);
    
    public record InsertFirmatarioDocumento(DocsPaVO.documento.FirmatarioDocumento firmatarioDoc)
        : IRequest<InsertFirmatarioDocumentoResult>;

    public record IsCheckedOutDocumentResult(bool output);

    public record IsCheckedOutDocument(string idDocument, string documentNumber, DocsPaVO.utente.InfoUtente infoUtente, bool checkAllegati)
        : IRequest<IsCheckedOutDocumentResult>;

    public record isRFEnabledResult(bool output);

    public record isRFEnabled()
        : IRequest<isRFEnabledResult>;

    public record PutFileFromUploadManagerLightResult(
        bool output,
        DocsPaVO.documento.FileRequest fileRequest);

    public record PutFileFromUploadManagerLight(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocument, string repositoryId, InfoUtente infoUtente)
        : IRequest<PutFileFromUploadManagerLightResult>;

    public record ReadDocumentDataFromExcelFileResult(
        DocsPaVO.PrjDocImport.DocumentRowDataContainer output,
        string error);

    public record ReadDocumentDataFromExcelFile(
            byte[] content,
            DocsPaVO.utente.InfoUtente userInfo,
            DocsPaVO.utente.Ruolo role,
            bool isEnabledPregressi,
            bool isStampaUnione)
        : IRequest<ReadDocumentDataFromExcelFileResult>;

    public record ReportSchedaDocResult(DocsPaVO.documento.FileDocumento output);

    public record ReportSchedaDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc)
        : IRequest<ReportSchedaDocResult>;

    public record ReportTrasmissioniDocFascUtenteResult(bool output,
                DocsPaVO.documento.FileDocumento fileDoc);

    public record ReportTrasmissioniDocFascUtente(DocsPaVO.trasmissione.OggettoTrasm obj, DocsPaVO.utente.InfoUtente infoUt)
        : IRequest<ReportTrasmissioniDocFascUtenteResult>;

    public record SendMailSupportForDocumentDelivery(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Utente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        : IRequest;

    public record SendFatturaResult(bool output);

    public record SendFattura(string fattura, DocsPaVO.utente.InfoUtente infoUtente, string idGruppo)
        : IRequest<SendFatturaResult>;

    public record SetMTextFullQualifiedName(DocsPaVO.MText.MTextDocumentInfo mTextDocInfo)
        : IRequest;

    public record signDocumentResult(DocsPaVO.documento.MassSignature output);

    public record signDocument(DocsPaVO.documento.MassSignature massSignature, DocsPaVO.utente.InfoUtente infoUtente)
        : IRequest<signDocumentResult>;

    public record StampaRicevutaProtocolloRtfResult(DocsPaVO.documento.FileDocumento output);

    public record StampaRicevutaProtocolloRtf(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc)
        : IRequest<StampaRicevutaProtocolloRtfResult>;

    public record UndoCheckOutDocumentResult(DocsPaVO.Validations.ValidationResultInfo output);

    public record UndoCheckOutDocument(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, DocsPaVO.utente.InfoUtente checkOutOwner)
        : IRequest<UndoCheckOutDocumentResult>;

    public record UploadAttachmentOnSharedFolderResult(bool output);

    public record UploadAttachmentOnSharedFolder(byte[] fileStream, string fileName, InfoUtente infoUtente)
        : IRequest<UploadAttachmentOnSharedFolderResult>;

    public record UploadFilePartResult(bool output);

    public record UploadFilePart(byte[] fileStream, int partIndex, int totalPart, string uuid, string fileName, string hash256)
        : IRequest<UploadFilePartResult>;

    public record UploadFileToFormazionePathResult(bool output);
    
    public record UploadFileToFormazionePath(byte[] fileStream, string fileName, string idUO, InfoUtente infoUtente, bool isAllegato)
        : IRequest<UploadFileToFormazionePathResult>;

    public record UploadTemplateFormazionePathResult(bool output);

    public record UploadTemplateFormazionePath(byte[] fileStream, string idUO, InfoUtente infoUtente)
        : IRequest<UploadTemplateFormazionePathResult>;

    public record ValidateLoginContainerResult(DocsPaVO.utente.UserLogin.ValidationResult result);

    //public record ValidateLoginResult(ValidateLoginContainerResult output);
    public record ValidateLoginResult(DocsPaVO.utente.UserLogin.ValidationResult output);

    public record ValidateLogin(string userID, string idAmm, string webSessionId)
            : IRequest<ValidateLoginResult>;

    public record AppendDocumentoFirmatoManagerResult(bool output, DocsPaVO.documento.FileRequest fileRequest);
    public record AppendDocumentoFirmatoManager(byte[] signedContent, bool cofirma, DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente, bool isPades = false, bool isConvertedToPdf = false) : IRequest<AppendDocumentoFirmatoManagerResult>;
}
