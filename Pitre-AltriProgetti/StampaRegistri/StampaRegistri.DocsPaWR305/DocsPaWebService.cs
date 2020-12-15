// Decompiled with JetBrains decompiler
// Type: StampaRegistri.DocsPaWR305.DocsPaWebService
// Assembly: StampaRegistri, Version=1.0.2666.24565, Culture=neutral, PublicKeyToken=null
// MVID: FC84A413-4958-415B-A670-C7209C4C46FD
// Assembly location: C:\TERNA\StampaRegistri\StampaRegistri.exe

using StampaRegistri.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
    [DesignerCategory("code")]
    [DebuggerStepThrough]
    [WebServiceBinding(Name = "DocsPaWebServiceSoap", Namespace = "http://localhost")]
    [GeneratedCode("System.Web.Services", "2.0.50727.42")]
    [XmlInclude(typeof(MarshalByRefObject))]
    [XmlInclude(typeof(Storico))]
    public class DocsPaWebService : SoapHttpClientProtocol
    {
        private SendOrPostCallback DO_GetListaStoriciMittenteOperationCompleted;
        private SendOrPostCallback DO_TrasmettiDestinatariModificatiOperationCompleted;
        private SendOrPostCallback DO_RemoveDestinatrioCCModificatoOperationCompleted;
        private SendOrPostCallback DO_ClearDestinatariCCModificatiOperationCompleted;
        private SendOrPostCallback DO_ClearDestinatariModificatiOperationCompleted;
        private SendOrPostCallback DO_RemoveDestinatrioModificatoOperationCompleted;
        private SendOrPostCallback DO_AddDestinatrioCCModificatoOperationCompleted;
        private SendOrPostCallback DO_AddDestinatrioModificatoOperationCompleted;
        private SendOrPostCallback GetUrlUploadAcquisizioneOperationCompleted;
        private SendOrPostCallback GetPathAcquisizioneBatchOperationCompleted;
        private SendOrPostCallback getTemplatePerRicercaOperationCompleted;
        private SendOrPostCallback spostaOggettoOperationCompleted;
        private SendOrPostCallback messaInEserizioTemplateOperationCompleted;
        private SendOrPostCallback salvaInserimentoUtenteProfDimOperationCompleted;
        private SendOrPostCallback getIdAmmByCodOperationCompleted;
        private SendOrPostCallback getTemplateOperationCompleted;
        private SendOrPostCallback disabilitaTemplateOperationCompleted;
        private SendOrPostCallback getTemplatesOperationCompleted;
        private SendOrPostCallback aggiornaTemplateOperationCompleted;
        private SendOrPostCallback salvaTemplateOperationCompleted;
        private SendOrPostCallback aggiungiOggettoValoreOggettoCustomOperationCompleted;
        private SendOrPostCallback eliminaOggettoCustomTemplateOperationCompleted;
        private SendOrPostCallback aggiungiOggettoCustomTemplateOperationCompleted;
        private SendOrPostCallback getValoreOggettoOperationCompleted;
        private SendOrPostCallback getTipoOggettoOperationCompleted;
        private SendOrPostCallback getOggettoCustomOperationCompleted;
        private SendOrPostCallback getTipiDocumentoOperationCompleted;
        private SendOrPostCallback getTemaplatesVuotoOperationCompleted;
        private SendOrPostCallback getTipiOggettoOperationCompleted;
        private SendOrPostCallback GetTipologiaAttoProfDinOperationCompleted;
        private SendOrPostCallback AddressbookGetRuoliUtenteIntOperationCompleted;
        private SendOrPostCallback rubricaGetRootItemsOperationCompleted;
        private SendOrPostCallback amministrazioneGetRuoliInUOOperationCompleted;
        private SendOrPostCallback rubricaGetElementiRubricaRangeOperationCompleted;
        private SendOrPostCallback AddressbookGetCorrispondenteByCodRubricaIEOperationCompleted;
        private SendOrPostCallback AddressbookGetCorrispondenteBySystemIdOperationCompleted;
        private SendOrPostCallback AddressbookGetCorrispondenteByCodRubricaOperationCompleted;
        private SendOrPostCallback rubricaCheckChildrenExistenceExOperationCompleted;
        private SendOrPostCallback rubricaCheckChildrenExistenceOperationCompleted;
        private SendOrPostCallback rubricaGetElementoRubricaOperationCompleted;
        private SendOrPostCallback rubricaGetGerarchiaElementoOperationCompleted;
        private SendOrPostCallback rubricaGetElementiRubricaOperationCompleted;
        private SendOrPostCallback GetUOSmistamentoOperationCompleted;
        private SendOrPostCallback RifiutaDocumentoOperationCompleted;
        private SendOrPostCallback ScartaDocumentoOperationCompleted;
        private SendOrPostCallback SmistaDocumentoNonTrasmessoOperationCompleted;
        private SendOrPostCallback SmistaDocumentoOperationCompleted;
        private SendOrPostCallback GetUOInferioriOperationCompleted;
        private SendOrPostCallback GetUOAppartenenzaOperationCompleted;
        private SendOrPostCallback GetDocumentoSmistamentoOperationCompleted;
        private SendOrPostCallback GetListDocumentiTrasmessiOperationCompleted;
        private SendOrPostCallback DO_GetDSReportDocXUoOperationCompleted;
        private SendOrPostCallback DO_GetDSReportDocXSedeOperationCompleted;
        private SendOrPostCallback DO_GetDSTempiMediLavorazioneCompactOperationCompleted;
        private SendOrPostCallback DO_GetDSTempiMediLavorazioneOperationCompleted;
        private SendOrPostCallback DO_GetDSReportAnnualeByFascOperationCompleted;
        private SendOrPostCallback DO_GetDSReportFascicoliPerVTCompactOperationCompleted;
        private SendOrPostCallback DO_GetDSReportFascicoliPerVTOperationCompleted;
        private SendOrPostCallback DO_GetDSReportDocTrasmToAOOOperationCompleted;
        private SendOrPostCallback DO_GetDSReportDocClassCompactOperationCompleted;
        private SendOrPostCallback DO_GetDSReportDocClassOperationCompleted;
        private SendOrPostCallback DO_GetDSReportAnnualeByRegOperationCompleted;
        private SendOrPostCallback DO_GetEmptySimpleLogOperationCompleted;
        private SendOrPostCallback DO_GetEmptyReportOperationCompleted;
        private SendOrPostCallback DO_GetEmptyRegitroOperationCompleted;
        private SendOrPostCallback DO_GetEmptyParametroOperationCompleted;
        private SendOrPostCallback DO_NewAmministrazioneOperationCompleted;
        private SendOrPostCallback DO_NewRegistroOperationCompleted;
        private SendOrPostCallback DO_NewReportOperationCompleted;
        private SendOrPostCallback DO_AddSubReportOperationCompleted;
        private SendOrPostCallback DO_ReadXMLOperationCompleted;
        private SendOrPostCallback DO_LOG_3_PARAMSOperationCompleted;
        private SendOrPostCallback DO_LOGOperationCompleted;
        private SendOrPostCallback DO_GetDescRegistroOperationCompleted;
        private SendOrPostCallback Do_GetAmmByIdAmmOperationCompleted;
        private SendOrPostCallback Do_GetVarDescAmmByIdAmmOperationCompleted;
        private SendOrPostCallback DO_GetIdAmmByCodAmmOperationCompleted;
        private SendOrPostCallback DO_GetSediOperationCompleted;
        private SendOrPostCallback DO_GetAnniProfilazioneOperationCompleted;
        private SendOrPostCallback DO_GetRegistriOperationCompleted;
        private SendOrPostCallback DO_GetAmministrazioniOperationCompleted;
        private SendOrPostCallback DO_GetDBTypeOperationCompleted;
        private SendOrPostCallback DO_GetConnectionStringOperationCompleted;
        private SendOrPostCallback UpdateUserFilenetOperationCompleted;
        private SendOrPostCallback DisableUserFilenetOperationCompleted;
        private SendOrPostCallback DeleteUserFilenetOperationCompleted;
        private SendOrPostCallback AddUserFilenetOperationCompleted;
        private SendOrPostCallback VerificaUtenteOperationCompleted;
        private SendOrPostCallback LoginRDEOperationCompleted;
        private SendOrPostCallback GetIdDKOperationCompleted;
        private SendOrPostCallback GetDKPathOperationCompleted;
        private SendOrPostCallback amministrazioneGetUtentiOperationCompleted;
        private SendOrPostCallback amministrazioneGetAmministrazioniOperationCompleted;
        private SendOrPostCallback amministrazioneGetParametroConfigurazioneOperationCompleted;
        private SendOrPostCallback AmmSostituzioneUtenteOperationCompleted;
        private SendOrPostCallback AmmRifiutaTrasmConWFOperationCompleted;
        private SendOrPostCallback AmmVerificaTrasmRuoloOperationCompleted;
        private SendOrPostCallback AmmVerificaUtenteLoggatoOperationCompleted;
        private SendOrPostCallback AmmEliminaADLUtenteOperationCompleted;
        private SendOrPostCallback AmmCancUtenteInRuoloOperationCompleted;
        private SendOrPostCallback AmmInsTrasmUtenteOperationCompleted;
        private SendOrPostCallback AmmInsUtenteInRuoloOperationCompleted;
        private SendOrPostCallback AmmInsTipoFunzioniOperationCompleted;
        private SendOrPostCallback AmmInsRegistriOperationCompleted;
        private SendOrPostCallback AmmEliminaRuoloOperationCompleted;
        private SendOrPostCallback AmmModRuoloOperationCompleted;
        private SendOrPostCallback AmmInsNuovoRuoloOperationCompleted;
        private SendOrPostCallback AmmEliminaUOOperationCompleted;
        private SendOrPostCallback AmmModUOOperationCompleted;
        private SendOrPostCallback AmmInsNuovaUOOperationCompleted;
        private SendOrPostCallback AmmGetDatiUtenteOperationCompleted;
        private SendOrPostCallback AmmGetListTipiRuoloOperationCompleted;
        private SendOrPostCallback AmmGetListFunzioniOperationCompleted;
        private SendOrPostCallback AmmGetListRegistriOperationCompleted;
        private SendOrPostCallback AmmGetListUtentiOperationCompleted;
        private SendOrPostCallback AmmGetListUtentiRuoloOperationCompleted;
        private SendOrPostCallback AmmGetListRuoliUOOperationCompleted;
        private SendOrPostCallback AmmGetListUOOperationCompleted;
        private SendOrPostCallback addressbookGetListaCorrispondentiSempliceOperationCompleted;
        private SendOrPostCallback ReportTrasmissioniUOOperationCompleted;
        private SendOrPostCallback ReportTrasmissioniDocFascOperationCompleted;
        private SendOrPostCallback ReportCorrispondentiOperationCompleted;
        private SendOrPostCallback ReportTitolarioOperationCompleted;
        private SendOrPostCallback ReportBustaOperationCompleted;
        private SendOrPostCallback ReportSchedaDocOperationCompleted;
        private SendOrPostCallback FaxInvioOperationCompleted;
        private SendOrPostCallback FaxProcessaCasellaOperationCompleted;
        private SendOrPostCallback RegistriStampaWithFiltersOperationCompleted;
        private SendOrPostCallback RegistriStampaOperationCompleted;
        private SendOrPostCallback InteroperabilitaAggiornamentoConfermaOperationCompleted;
        private SendOrPostCallback InteroperabilitaRicezioneOperationCompleted;
        private SendOrPostCallback AddressbookGetCanaliOperationCompleted;
        private SendOrPostCallback AddressbookGetRuoliRiferimentoAutorizzatiOperationCompleted;
        private SendOrPostCallback AddressbookGetRuoliSuperioriInUOOperationCompleted;
        private SendOrPostCallback AddressbookInsertCorrispondenteOperationCompleted;
        private SendOrPostCallback AddressbookGetRootUOOperationCompleted;
        private SendOrPostCallback AddressbookGetListaCorrispondentiAutorizzatiOperationCompleted;
        private SendOrPostCallback AddressbookGetDettagliCorrispondenteOperationCompleted;
        private SendOrPostCallback AddressbookGetListaCorrispondentiOperationCompleted;
        private SendOrPostCallback AddressbookGetListaCorrispondenti_AutOperationCompleted;
        private SendOrPostCallback TrasmissioniDeleteTemplateOperationCompleted;
        private SendOrPostCallback TrasmissioniUpdateTemplateOperationCompleted;
        private SendOrPostCallback TrasmissioneGetListaTemplateOperationCompleted;
        private SendOrPostCallback TrasmissioneAddTemplateOperationCompleted;
        private SendOrPostCallback TrasmissioneGetInRispostaAOperationCompleted;
        private SendOrPostCallback TrasmissioneGetRispostaOperationCompleted;
        private SendOrPostCallback TrasmissioneExecuteAccRifOperationCompleted;
        private SendOrPostCallback TrasmissioneGetRagioniOperationCompleted;
        private SendOrPostCallback TrasmissioneGetQueryEffettuatePagingOperationCompleted;
        private SendOrPostCallback TrasmissioneGetQueryEffettuateDocPagingOperationCompleted;
        private SendOrPostCallback TrasmissioneGetQueryEffettuateOperationCompleted;
        private SendOrPostCallback TrasmissioneGetQueryRicevutePagingOperationCompleted;
        private SendOrPostCallback TrasmissioneGetQueryRicevuteOperationCompleted;
        private SendOrPostCallback TrasmissioneExecuteTrasmOperationCompleted;
        private SendOrPostCallback TrasmissioneSaveTrasmOperationCompleted;
        private SendOrPostCallback TrasmissioneFascicoloAddDaTemplOperationCompleted;
        private SendOrPostCallback TrasmissioneAddDaTemplOperationCompleted;
        private SendOrPostCallback trasmissioneSetTxUtAsVisteOperationCompleted;
        private SendOrPostCallback FascicolazioneGetVisibilitaOperationCompleted;
        private SendOrPostCallback FascicolazioneSospendiRiattivaUtenteOperationCompleted;
        private SendOrPostCallback FascicolazioneGetFigliClassificaOperationCompleted;
        private SendOrPostCallback FascicolazioneGetGerarchiaDaCodiceOperationCompleted;
        private SendOrPostCallback FascicolazioneGetGerarchiaOperationCompleted;
        private SendOrPostCallback FascicolazioneGetFascicoloDaCodiceOperationCompleted;
        private SendOrPostCallback FascicolazioneGetFascicoliDaDocOperationCompleted;
        private SendOrPostCallback FascicolazioneGetDettaglioFascicoloOperationCompleted;
        private SendOrPostCallback FascicolazioneAddDocFolderOperationCompleted;
        private SendOrPostCallback FascicolazioneAddDocFascicoloOperationCompleted;
        private SendOrPostCallback FascicolazioneDelFolderOperationCompleted;
        private SendOrPostCallback FascicolazioneDelTitolarioOperationCompleted;
        private SendOrPostCallback FascicolazioneUpdateTitolarioOperationCompleted;
        private SendOrPostCallback FascicolazioneModifyFolderOperationCompleted;
        private SendOrPostCallback FascicolazioneNewFolderOperationCompleted;
        private SendOrPostCallback FascicolazioneSetFascicoloOperationCompleted;
        private SendOrPostCallback FascicolazioneDeleteDocumentoOperationCompleted;
        private SendOrPostCallback FascicolazioneGetDocumentiPagingOperationCompleted;
        private SendOrPostCallback FascicolazioneGetDocumentiOperationCompleted;
        private SendOrPostCallback FascicolazioneGetFoldersDocumentFascicoloOperationCompleted;
        private SendOrPostCallback FascicolazioneGetFoldersDocumentOperationCompleted;
        private SendOrPostCallback FascicolazioneGetFolderOperationCompleted;
        private SendOrPostCallback FascicolazioneNewFascicoloLFOperationCompleted;
        private SendOrPostCallback FascicolazioneNewFascicoloOperationCompleted;
        private SendOrPostCallback FascicolazioneGetListaFascicoliPagingOperationCompleted;
        private SendOrPostCallback FascicolazioneGetListaFascicoliOperationCompleted;
        private SendOrPostCallback FascicolazioneGetAllListaFascOperationCompleted;
        private SendOrPostCallback FascicolazioneSetAutorizzazioniNodoTitolarioOperationCompleted;
        private SendOrPostCallback FascicolazioneGetAutorizzazioniNodoTitolarioOperationCompleted;
        private SendOrPostCallback FascicolazioneCanRemoveFascicoloOperationCompleted;
        private SendOrPostCallback FascicolazioneGetCodiceFiglioTitolarioOperationCompleted;
        private SendOrPostCallback FascicolazioneNewTitolarioOperationCompleted;
        private SendOrPostCallback FascicolazioneGetTitolarioOperationCompleted;
        private SendOrPostCallback DocumentoCercaDuplicatiOperationCompleted;
        private SendOrPostCallback DocumentoGetCatenaDocOperationCompleted;
        private SendOrPostCallback DocumentoAddTipologiaAttoOperationCompleted;
        private SendOrPostCallback DocumentoAddParolaChiaveOperationCompleted;
        private SendOrPostCallback DocumentoAddOggettoOperationCompleted;
        private SendOrPostCallback DocumentoSetFlagDaInviareOperationCompleted;
        private SendOrPostCallback DocumentoGetListaStoriciOggettoOperationCompleted;
        private SendOrPostCallback DocumentoVerificaFirmaOperationCompleted;
        private SendOrPostCallback DocumentoSpedisciOperationCompleted;
        private SendOrPostCallback DocumentoGetTipologiaAttoOperationCompleted;
        private SendOrPostCallback DocumentoModificaVersioneOperationCompleted;
        private SendOrPostCallback DocumentoScambiaVersioniOperationCompleted;
        private SendOrPostCallback DocumentoAggiungiVersioneOperationCompleted;
        private SendOrPostCallback DocumentoRimuoviVersioneOperationCompleted;
        private SendOrPostCallback DocumentoModificaAllegatoOperationCompleted;
        private SendOrPostCallback DocumentoAggiungiAllegatoOperationCompleted;
        private SendOrPostCallback DocumentoGetAllegatiOperationCompleted;
        private SendOrPostCallback DocumentoExecRimuoviSchedaOperationCompleted;
        private SendOrPostCallback DocumentoGetParoleChiaveOperationCompleted;
        private SendOrPostCallback DocumentoGetVisibilitaOperationCompleted;
        private SendOrPostCallback DocumentoAddDocGrigiaOperationCompleted;
        private SendOrPostCallback DocumentoSaveDocumentoOperationCompleted;
        private SendOrPostCallback DocumentoCancellaAreaLavoroOperationCompleted;
        private SendOrPostCallback DocumentoGetAreaLavoroPagingOperationCompleted;
        private SendOrPostCallback DocumentoGetAreaLavoroOperationCompleted;
        private SendOrPostCallback DocumentoExecAnnullaProtOperationCompleted;
        private SendOrPostCallback DocumentoExecAddLavoroOperationCompleted;
        private SendOrPostCallback documentoImportaProtocolloEmergenzaOperationCompleted;
        private SendOrPostCallback DocumentoProtocollaOperationCompleted;
        private SendOrPostCallback DocumentoGetListaOggettiOperationCompleted;
        private SendOrPostCallback DocumentoGetDettaglioDocumentoOperationCompleted;
        private SendOrPostCallback DocumentoPutFileBatchOperationCompleted;
        private SendOrPostCallback DocumentoPutFileOperationCompleted;
        private SendOrPostCallback DocumentoGetFileOperationCompleted;
        private SendOrPostCallback DocumentoGetQueryFullTextDocumentoPagingOperationCompleted;
        private SendOrPostCallback DocumentoGetQueryDocumentoPagingOperationCompleted;
        private SendOrPostCallback DocumentoGetQueryDocumentoOperationCompleted;
        private SendOrPostCallback doucmentoGetDocTypeOperationCompleted;
        private SendOrPostCallback UtenteChangePasswordOperationCompleted;
        private SendOrPostCallback UtenteGetRegistriOperationCompleted;
        private SendOrPostCallback RegistriModificaOperationCompleted;
        private SendOrPostCallback RegistriCambiaStatoOperationCompleted;
        private SendOrPostCallback ElencoUtentiConnessiOperationCompleted;
        private SendOrPostCallback DisconnettiUtenteOperationCompleted;
        private SendOrPostCallback LogoffRDEOperationCompleted;
        private SendOrPostCallback LogoffOperationCompleted;
        private SendOrPostCallback ValidateLoginOperationCompleted;
        private SendOrPostCallback LoginOperationCompleted;
        private SendOrPostCallback SimpleLoginOperationCompleted;
        private SendOrPostCallback CheckDatabaseConnectionOperationCompleted;
        private SendOrPostCallback CheckDPA_AmministraOperationCompleted;
        private SendOrPostCallback IsInternalProtocolEnabledOperationCompleted;
        private SendOrPostCallback DocumentoGetTipoProtoOperationCompleted;
        private SendOrPostCallback GetVisibilitaRuoliOperationCompleted;
        private SendOrPostCallback trasmissioniSendSollecitoOperationCompleted;
        private SendOrPostCallback TrasmettiProtocolloInternoOperationCompleted;
        private SendOrPostCallback GetRagioneTrasmissioneOperationCompleted;
        private SendOrPostCallback UOHasReferenceRoleOperationCompleted;
        private SendOrPostCallback reportFascetteFascicoloOperationCompleted;
        private SendOrPostCallback AmmGetIDAmmOperationCompleted;
        private SendOrPostCallback GetFilesLogOperationCompleted;
        private SendOrPostCallback StampaPDFLogOperationCompleted;
        private SendOrPostCallback ContaLogOperationCompleted;
        private SendOrPostCallback GetXMLLogFiltratoOperationCompleted;
        private SendOrPostCallback SetXMLLogOperationCompleted;
        private SendOrPostCallback GetXMLLogOperationCompleted;
        private SendOrPostCallback MoveRoleToNewUOOperationCompleted;
        private SendOrPostCallback DeleteUserStatusOperationCompleted;
        private SendOrPostCallback DeleteUserInRoleOperationCompleted;
        private SendOrPostCallback DeleteOrDisablePeopleOperationCompleted;
        private SendOrPostCallback GetUserStatusOperationCompleted;
        private SendOrPostCallback CheckUserLoginOperationCompleted;
        private SendOrPostCallback MoveUserFromRoleOperationCompleted;
        private SendOrPostCallback ExportTitolarioOperationCompleted;
        private SendOrPostCallback ExportAmministrazioniOperationCompleted;
        private SendOrPostCallback ImportTitolarioReadStateOperationCompleted;
        private SendOrPostCallback ImportDocumentiReadStateOperationCompleted;
        private SendOrPostCallback ImportAmministrazioniReadStateOperationCompleted;
        private SendOrPostCallback ImportCorrispondentiReadStateOperationCompleted;
        private SendOrPostCallback CreateTitolarioOperationCompleted;
        private SendOrPostCallback CreateAmministrazioniOperationCompleted;
        private SendOrPostCallback UpdateDataOperationCompleted;
        private SendOrPostCallback RefreshAmministrazioneOperationCompleted;
        private SendOrPostCallback ImportCorrispondentiOperationCompleted;
        private SendOrPostCallback ImportDocumentiOperationCompleted;
        private SendOrPostCallback ImportOggettarioOperationCompleted;
        private SendOrPostCallback ImportStoricoOggettarioOperationCompleted;
        private SendOrPostCallback ImportTipiAttoOperationCompleted;
        private SendOrPostCallback ImportTipiDocumentoOperationCompleted;
        private SendOrPostCallback LoginAmministratoreOperationCompleted;
        private SendOrPostCallback LogoutAmministratoreOperationCompleted;
        private SendOrPostCallback CheckAdministratorOperationCompleted;
        private SendOrPostCallback CambiaPwdAmministratoreOperationCompleted;
        private SendOrPostCallback NodoTitolarioOperationCompleted;
        private SendOrPostCallback NodoTitolarioSecurityOperationCompleted;
        private SendOrPostCallback RegistriInAmmOperationCompleted;
        private SendOrPostCallback SecurityNodoRuoliOperationCompleted;
        private SendOrPostCallback UpdNodo_TitolarioOperationCompleted;
        private SendOrPostCallback filtroRicercaTitOperationCompleted;
        private SendOrPostCallback filtroRicercaTitDocspaOperationCompleted;
        private SendOrPostCallback filtroRicercaTitAmmOperationCompleted;
        private SendOrPostCallback findNodoRootOperationCompleted;
        private SendOrPostCallback findNodoByCodOperationCompleted;
        private SendOrPostCallback AggNewNodoOperationCompleted;
        private SendOrPostCallback EliminaNodoOperationCompleted;
        private SendOrPostCallback GetCodLivOperationCompleted;
        private SendOrPostCallback SpostaNodoTitolarioOperationCompleted;
        private bool useDefaultCredentialsSetExplicitly;

        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                if (this.IsLocalFileSystemWebService(base.Url) && !this.useDefaultCredentialsSetExplicitly && !this.IsLocalFileSystemWebService(value))
                    base.UseDefaultCredentials = false;
                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        public event DO_GetListaStoriciMittenteCompletedEventHandler DO_GetListaStoriciMittenteCompleted;

        public event DO_TrasmettiDestinatariModificatiCompletedEventHandler DO_TrasmettiDestinatariModificatiCompleted;

        public event DO_RemoveDestinatrioCCModificatoCompletedEventHandler DO_RemoveDestinatrioCCModificatoCompleted;

        public event DO_ClearDestinatariCCModificatiCompletedEventHandler DO_ClearDestinatariCCModificatiCompleted;

        public event DO_ClearDestinatariModificatiCompletedEventHandler DO_ClearDestinatariModificatiCompleted;

        public event DO_RemoveDestinatrioModificatoCompletedEventHandler DO_RemoveDestinatrioModificatoCompleted;

        public event DO_AddDestinatrioCCModificatoCompletedEventHandler DO_AddDestinatrioCCModificatoCompleted;

        public event DO_AddDestinatrioModificatoCompletedEventHandler DO_AddDestinatrioModificatoCompleted;

        public event GetUrlUploadAcquisizioneCompletedEventHandler GetUrlUploadAcquisizioneCompleted;

        public event GetPathAcquisizioneBatchCompletedEventHandler GetPathAcquisizioneBatchCompleted;

        public event getTemplatePerRicercaCompletedEventHandler getTemplatePerRicercaCompleted;

        public event spostaOggettoCompletedEventHandler spostaOggettoCompleted;

        public event messaInEserizioTemplateCompletedEventHandler messaInEserizioTemplateCompleted;

        public event salvaInserimentoUtenteProfDimCompletedEventHandler salvaInserimentoUtenteProfDimCompleted;

        public event getIdAmmByCodCompletedEventHandler getIdAmmByCodCompleted;

        public event getTemplateCompletedEventHandler getTemplateCompleted;

        public event disabilitaTemplateCompletedEventHandler disabilitaTemplateCompleted;

        public event getTemplatesCompletedEventHandler getTemplatesCompleted;

        public event aggiornaTemplateCompletedEventHandler aggiornaTemplateCompleted;

        public event salvaTemplateCompletedEventHandler salvaTemplateCompleted;

        public event aggiungiOggettoValoreOggettoCustomCompletedEventHandler aggiungiOggettoValoreOggettoCustomCompleted;

        public event eliminaOggettoCustomTemplateCompletedEventHandler eliminaOggettoCustomTemplateCompleted;

        public event aggiungiOggettoCustomTemplateCompletedEventHandler aggiungiOggettoCustomTemplateCompleted;

        public event getValoreOggettoCompletedEventHandler getValoreOggettoCompleted;

        public event getTipoOggettoCompletedEventHandler getTipoOggettoCompleted;

        public event getOggettoCustomCompletedEventHandler getOggettoCustomCompleted;

        public event getTipiDocumentoCompletedEventHandler getTipiDocumentoCompleted;

        public event getTemaplatesVuotoCompletedEventHandler getTemaplatesVuotoCompleted;

        public event getTipiOggettoCompletedEventHandler getTipiOggettoCompleted;

        public event GetTipologiaAttoProfDinCompletedEventHandler GetTipologiaAttoProfDinCompleted;

        public event AddressbookGetRuoliUtenteIntCompletedEventHandler AddressbookGetRuoliUtenteIntCompleted;

        public event rubricaGetRootItemsCompletedEventHandler rubricaGetRootItemsCompleted;

        public event amministrazioneGetRuoliInUOCompletedEventHandler amministrazioneGetRuoliInUOCompleted;

        public event rubricaGetElementiRubricaRangeCompletedEventHandler rubricaGetElementiRubricaRangeCompleted;

        public event AddressbookGetCorrispondenteByCodRubricaIECompletedEventHandler AddressbookGetCorrispondenteByCodRubricaIECompleted;

        public event AddressbookGetCorrispondenteBySystemIdCompletedEventHandler AddressbookGetCorrispondenteBySystemIdCompleted;

        public event AddressbookGetCorrispondenteByCodRubricaCompletedEventHandler AddressbookGetCorrispondenteByCodRubricaCompleted;

        public event rubricaCheckChildrenExistenceExCompletedEventHandler rubricaCheckChildrenExistenceExCompleted;

        public event rubricaCheckChildrenExistenceCompletedEventHandler rubricaCheckChildrenExistenceCompleted;

        public event rubricaGetElementoRubricaCompletedEventHandler rubricaGetElementoRubricaCompleted;

        public event rubricaGetGerarchiaElementoCompletedEventHandler rubricaGetGerarchiaElementoCompleted;

        public event rubricaGetElementiRubricaCompletedEventHandler rubricaGetElementiRubricaCompleted;

        public event GetUOSmistamentoCompletedEventHandler GetUOSmistamentoCompleted;

        public event RifiutaDocumentoCompletedEventHandler RifiutaDocumentoCompleted;

        public event ScartaDocumentoCompletedEventHandler ScartaDocumentoCompleted;

        public event SmistaDocumentoNonTrasmessoCompletedEventHandler SmistaDocumentoNonTrasmessoCompleted;

        public event SmistaDocumentoCompletedEventHandler SmistaDocumentoCompleted;

        public event GetUOInferioriCompletedEventHandler GetUOInferioriCompleted;

        public event GetUOAppartenenzaCompletedEventHandler GetUOAppartenenzaCompleted;

        public event GetDocumentoSmistamentoCompletedEventHandler GetDocumentoSmistamentoCompleted;

        public event GetListDocumentiTrasmessiCompletedEventHandler GetListDocumentiTrasmessiCompleted;

        public event DO_GetDSReportDocXUoCompletedEventHandler DO_GetDSReportDocXUoCompleted;

        public event DO_GetDSReportDocXSedeCompletedEventHandler DO_GetDSReportDocXSedeCompleted;

        public event DO_GetDSTempiMediLavorazioneCompactCompletedEventHandler DO_GetDSTempiMediLavorazioneCompactCompleted;

        public event DO_GetDSTempiMediLavorazioneCompletedEventHandler DO_GetDSTempiMediLavorazioneCompleted;

        public event DO_GetDSReportAnnualeByFascCompletedEventHandler DO_GetDSReportAnnualeByFascCompleted;

        public event DO_GetDSReportFascicoliPerVTCompactCompletedEventHandler DO_GetDSReportFascicoliPerVTCompactCompleted;

        public event DO_GetDSReportFascicoliPerVTCompletedEventHandler DO_GetDSReportFascicoliPerVTCompleted;

        public event DO_GetDSReportDocTrasmToAOOCompletedEventHandler DO_GetDSReportDocTrasmToAOOCompleted;

        public event DO_GetDSReportDocClassCompactCompletedEventHandler DO_GetDSReportDocClassCompactCompleted;

        public event DO_GetDSReportDocClassCompletedEventHandler DO_GetDSReportDocClassCompleted;

        public event DO_GetDSReportAnnualeByRegCompletedEventHandler DO_GetDSReportAnnualeByRegCompleted;

        public event DO_GetEmptySimpleLogCompletedEventHandler DO_GetEmptySimpleLogCompleted;

        public event DO_GetEmptyReportCompletedEventHandler DO_GetEmptyReportCompleted;

        public event DO_GetEmptyRegitroCompletedEventHandler DO_GetEmptyRegitroCompleted;

        public event DO_GetEmptyParametroCompletedEventHandler DO_GetEmptyParametroCompleted;

        public event DO_NewAmministrazioneCompletedEventHandler DO_NewAmministrazioneCompleted;

        public event DO_NewRegistroCompletedEventHandler DO_NewRegistroCompleted;

        public event DO_NewReportCompletedEventHandler DO_NewReportCompleted;

        public event DO_AddSubReportCompletedEventHandler DO_AddSubReportCompleted;

        public event DO_ReadXMLCompletedEventHandler DO_ReadXMLCompleted;

        public event DO_LOG_3_PARAMSCompletedEventHandler DO_LOG_3_PARAMSCompleted;

        public event DO_LOGCompletedEventHandler DO_LOGCompleted;

        public event DO_GetDescRegistroCompletedEventHandler DO_GetDescRegistroCompleted;

        public event Do_GetAmmByIdAmmCompletedEventHandler Do_GetAmmByIdAmmCompleted;

        public event Do_GetVarDescAmmByIdAmmCompletedEventHandler Do_GetVarDescAmmByIdAmmCompleted;

        public event DO_GetIdAmmByCodAmmCompletedEventHandler DO_GetIdAmmByCodAmmCompleted;

        public event DO_GetSediCompletedEventHandler DO_GetSediCompleted;

        public event DO_GetAnniProfilazioneCompletedEventHandler DO_GetAnniProfilazioneCompleted;

        public event DO_GetRegistriCompletedEventHandler DO_GetRegistriCompleted;

        public event DO_GetAmministrazioniCompletedEventHandler DO_GetAmministrazioniCompleted;

        public event DO_GetDBTypeCompletedEventHandler DO_GetDBTypeCompleted;

        public event DO_GetConnectionStringCompletedEventHandler DO_GetConnectionStringCompleted;

        public event UpdateUserFilenetCompletedEventHandler UpdateUserFilenetCompleted;

        public event DisableUserFilenetCompletedEventHandler DisableUserFilenetCompleted;

        public event DeleteUserFilenetCompletedEventHandler DeleteUserFilenetCompleted;

        public event AddUserFilenetCompletedEventHandler AddUserFilenetCompleted;

        public event VerificaUtenteCompletedEventHandler VerificaUtenteCompleted;

        public event LoginRDECompletedEventHandler LoginRDECompleted;

        public event GetIdDKCompletedEventHandler GetIdDKCompleted;

        public event GetDKPathCompletedEventHandler GetDKPathCompleted;

        public event amministrazioneGetUtentiCompletedEventHandler amministrazioneGetUtentiCompleted;

        public event amministrazioneGetAmministrazioniCompletedEventHandler amministrazioneGetAmministrazioniCompleted;

        public event amministrazioneGetParametroConfigurazioneCompletedEventHandler amministrazioneGetParametroConfigurazioneCompleted;

        public event AmmSostituzioneUtenteCompletedEventHandler AmmSostituzioneUtenteCompleted;

        public event AmmRifiutaTrasmConWFCompletedEventHandler AmmRifiutaTrasmConWFCompleted;

        public event AmmVerificaTrasmRuoloCompletedEventHandler AmmVerificaTrasmRuoloCompleted;

        public event AmmVerificaUtenteLoggatoCompletedEventHandler AmmVerificaUtenteLoggatoCompleted;

        public event AmmEliminaADLUtenteCompletedEventHandler AmmEliminaADLUtenteCompleted;

        public event AmmCancUtenteInRuoloCompletedEventHandler AmmCancUtenteInRuoloCompleted;

        public event AmmInsTrasmUtenteCompletedEventHandler AmmInsTrasmUtenteCompleted;

        public event AmmInsUtenteInRuoloCompletedEventHandler AmmInsUtenteInRuoloCompleted;

        public event AmmInsTipoFunzioniCompletedEventHandler AmmInsTipoFunzioniCompleted;

        public event AmmInsRegistriCompletedEventHandler AmmInsRegistriCompleted;

        public event AmmEliminaRuoloCompletedEventHandler AmmEliminaRuoloCompleted;

        public event AmmModRuoloCompletedEventHandler AmmModRuoloCompleted;

        public event AmmInsNuovoRuoloCompletedEventHandler AmmInsNuovoRuoloCompleted;

        public event AmmEliminaUOCompletedEventHandler AmmEliminaUOCompleted;

        public event AmmModUOCompletedEventHandler AmmModUOCompleted;

        public event AmmInsNuovaUOCompletedEventHandler AmmInsNuovaUOCompleted;

        public event AmmGetDatiUtenteCompletedEventHandler AmmGetDatiUtenteCompleted;

        public event AmmGetListTipiRuoloCompletedEventHandler AmmGetListTipiRuoloCompleted;

        public event AmmGetListFunzioniCompletedEventHandler AmmGetListFunzioniCompleted;

        public event AmmGetListRegistriCompletedEventHandler AmmGetListRegistriCompleted;

        public event AmmGetListUtentiCompletedEventHandler AmmGetListUtentiCompleted;

        public event AmmGetListUtentiRuoloCompletedEventHandler AmmGetListUtentiRuoloCompleted;

        public event AmmGetListRuoliUOCompletedEventHandler AmmGetListRuoliUOCompleted;

        public event AmmGetListUOCompletedEventHandler AmmGetListUOCompleted;

        public event addressbookGetListaCorrispondentiSempliceCompletedEventHandler addressbookGetListaCorrispondentiSempliceCompleted;

        public event ReportTrasmissioniUOCompletedEventHandler ReportTrasmissioniUOCompleted;

        public event ReportTrasmissioniDocFascCompletedEventHandler ReportTrasmissioniDocFascCompleted;

        public event ReportCorrispondentiCompletedEventHandler ReportCorrispondentiCompleted;

        public event ReportTitolarioCompletedEventHandler ReportTitolarioCompleted;

        public event ReportBustaCompletedEventHandler ReportBustaCompleted;

        public event ReportSchedaDocCompletedEventHandler ReportSchedaDocCompleted;

        public event FaxInvioCompletedEventHandler FaxInvioCompleted;

        public event FaxProcessaCasellaCompletedEventHandler FaxProcessaCasellaCompleted;

        public event RegistriStampaWithFiltersCompletedEventHandler RegistriStampaWithFiltersCompleted;

        public event RegistriStampaCompletedEventHandler RegistriStampaCompleted;

        public event InteroperabilitaAggiornamentoConfermaCompletedEventHandler InteroperabilitaAggiornamentoConfermaCompleted;

        public event InteroperabilitaRicezioneCompletedEventHandler InteroperabilitaRicezioneCompleted;

        public event AddressbookGetCanaliCompletedEventHandler AddressbookGetCanaliCompleted;

        public event AddressbookGetRuoliRiferimentoAutorizzatiCompletedEventHandler AddressbookGetRuoliRiferimentoAutorizzatiCompleted;

        public event AddressbookGetRuoliSuperioriInUOCompletedEventHandler AddressbookGetRuoliSuperioriInUOCompleted;

        public event AddressbookInsertCorrispondenteCompletedEventHandler AddressbookInsertCorrispondenteCompleted;

        public event AddressbookGetRootUOCompletedEventHandler AddressbookGetRootUOCompleted;

        public event AddressbookGetListaCorrispondentiAutorizzatiCompletedEventHandler AddressbookGetListaCorrispondentiAutorizzatiCompleted;

        public event AddressbookGetDettagliCorrispondenteCompletedEventHandler AddressbookGetDettagliCorrispondenteCompleted;

        public event AddressbookGetListaCorrispondentiCompletedEventHandler AddressbookGetListaCorrispondentiCompleted;

        public event AddressbookGetListaCorrispondenti_AutCompletedEventHandler AddressbookGetListaCorrispondenti_AutCompleted;

        public event TrasmissioniDeleteTemplateCompletedEventHandler TrasmissioniDeleteTemplateCompleted;

        public event TrasmissioniUpdateTemplateCompletedEventHandler TrasmissioniUpdateTemplateCompleted;

        public event TrasmissioneGetListaTemplateCompletedEventHandler TrasmissioneGetListaTemplateCompleted;

        public event TrasmissioneAddTemplateCompletedEventHandler TrasmissioneAddTemplateCompleted;

        public event TrasmissioneGetInRispostaACompletedEventHandler TrasmissioneGetInRispostaACompleted;

        public event TrasmissioneGetRispostaCompletedEventHandler TrasmissioneGetRispostaCompleted;

        public event TrasmissioneExecuteAccRifCompletedEventHandler TrasmissioneExecuteAccRifCompleted;

        public event TrasmissioneGetRagioniCompletedEventHandler TrasmissioneGetRagioniCompleted;

        public event TrasmissioneGetQueryEffettuatePagingCompletedEventHandler TrasmissioneGetQueryEffettuatePagingCompleted;

        public event TrasmissioneGetQueryEffettuateDocPagingCompletedEventHandler TrasmissioneGetQueryEffettuateDocPagingCompleted;

        public event TrasmissioneGetQueryEffettuateCompletedEventHandler TrasmissioneGetQueryEffettuateCompleted;

        public event TrasmissioneGetQueryRicevutePagingCompletedEventHandler TrasmissioneGetQueryRicevutePagingCompleted;

        public event TrasmissioneGetQueryRicevuteCompletedEventHandler TrasmissioneGetQueryRicevuteCompleted;

        public event TrasmissioneExecuteTrasmCompletedEventHandler TrasmissioneExecuteTrasmCompleted;

        public event TrasmissioneSaveTrasmCompletedEventHandler TrasmissioneSaveTrasmCompleted;

        public event TrasmissioneFascicoloAddDaTemplCompletedEventHandler TrasmissioneFascicoloAddDaTemplCompleted;

        public event TrasmissioneAddDaTemplCompletedEventHandler TrasmissioneAddDaTemplCompleted;

        public event trasmissioneSetTxUtAsVisteCompletedEventHandler trasmissioneSetTxUtAsVisteCompleted;

        public event FascicolazioneGetVisibilitaCompletedEventHandler FascicolazioneGetVisibilitaCompleted;

        public event FascicolazioneSospendiRiattivaUtenteCompletedEventHandler FascicolazioneSospendiRiattivaUtenteCompleted;

        public event FascicolazioneGetFigliClassificaCompletedEventHandler FascicolazioneGetFigliClassificaCompleted;

        public event FascicolazioneGetGerarchiaDaCodiceCompletedEventHandler FascicolazioneGetGerarchiaDaCodiceCompleted;

        public event FascicolazioneGetGerarchiaCompletedEventHandler FascicolazioneGetGerarchiaCompleted;

        public event FascicolazioneGetFascicoloDaCodiceCompletedEventHandler FascicolazioneGetFascicoloDaCodiceCompleted;

        public event FascicolazioneGetFascicoliDaDocCompletedEventHandler FascicolazioneGetFascicoliDaDocCompleted;

        public event FascicolazioneGetDettaglioFascicoloCompletedEventHandler FascicolazioneGetDettaglioFascicoloCompleted;

        public event FascicolazioneAddDocFolderCompletedEventHandler FascicolazioneAddDocFolderCompleted;

        public event FascicolazioneAddDocFascicoloCompletedEventHandler FascicolazioneAddDocFascicoloCompleted;

        public event FascicolazioneDelFolderCompletedEventHandler FascicolazioneDelFolderCompleted;

        public event FascicolazioneDelTitolarioCompletedEventHandler FascicolazioneDelTitolarioCompleted;

        public event FascicolazioneUpdateTitolarioCompletedEventHandler FascicolazioneUpdateTitolarioCompleted;

        public event FascicolazioneModifyFolderCompletedEventHandler FascicolazioneModifyFolderCompleted;

        public event FascicolazioneNewFolderCompletedEventHandler FascicolazioneNewFolderCompleted;

        public event FascicolazioneSetFascicoloCompletedEventHandler FascicolazioneSetFascicoloCompleted;

        public event FascicolazioneDeleteDocumentoCompletedEventHandler FascicolazioneDeleteDocumentoCompleted;

        public event FascicolazioneGetDocumentiPagingCompletedEventHandler FascicolazioneGetDocumentiPagingCompleted;

        public event FascicolazioneGetDocumentiCompletedEventHandler FascicolazioneGetDocumentiCompleted;

        public event FascicolazioneGetFoldersDocumentFascicoloCompletedEventHandler FascicolazioneGetFoldersDocumentFascicoloCompleted;

        public event FascicolazioneGetFoldersDocumentCompletedEventHandler FascicolazioneGetFoldersDocumentCompleted;

        public event FascicolazioneGetFolderCompletedEventHandler FascicolazioneGetFolderCompleted;

        public event FascicolazioneNewFascicoloLFCompletedEventHandler FascicolazioneNewFascicoloLFCompleted;

        public event FascicolazioneNewFascicoloCompletedEventHandler FascicolazioneNewFascicoloCompleted;

        public event FascicolazioneGetListaFascicoliPagingCompletedEventHandler FascicolazioneGetListaFascicoliPagingCompleted;

        public event FascicolazioneGetListaFascicoliCompletedEventHandler FascicolazioneGetListaFascicoliCompleted;

        public event FascicolazioneGetAllListaFascCompletedEventHandler FascicolazioneGetAllListaFascCompleted;

        public event FascicolazioneSetAutorizzazioniNodoTitolarioCompletedEventHandler FascicolazioneSetAutorizzazioniNodoTitolarioCompleted;

        public event FascicolazioneGetAutorizzazioniNodoTitolarioCompletedEventHandler FascicolazioneGetAutorizzazioniNodoTitolarioCompleted;

        public event FascicolazioneCanRemoveFascicoloCompletedEventHandler FascicolazioneCanRemoveFascicoloCompleted;

        public event FascicolazioneGetCodiceFiglioTitolarioCompletedEventHandler FascicolazioneGetCodiceFiglioTitolarioCompleted;

        public event FascicolazioneNewTitolarioCompletedEventHandler FascicolazioneNewTitolarioCompleted;

        public event FascicolazioneGetTitolarioCompletedEventHandler FascicolazioneGetTitolarioCompleted;

        public event DocumentoCercaDuplicatiCompletedEventHandler DocumentoCercaDuplicatiCompleted;

        public event DocumentoGetCatenaDocCompletedEventHandler DocumentoGetCatenaDocCompleted;

        public event DocumentoAddTipologiaAttoCompletedEventHandler DocumentoAddTipologiaAttoCompleted;

        public event DocumentoAddParolaChiaveCompletedEventHandler DocumentoAddParolaChiaveCompleted;

        public event DocumentoAddOggettoCompletedEventHandler DocumentoAddOggettoCompleted;

        public event DocumentoSetFlagDaInviareCompletedEventHandler DocumentoSetFlagDaInviareCompleted;

        public event DocumentoGetListaStoriciOggettoCompletedEventHandler DocumentoGetListaStoriciOggettoCompleted;

        public event DocumentoVerificaFirmaCompletedEventHandler DocumentoVerificaFirmaCompleted;

        public event DocumentoSpedisciCompletedEventHandler DocumentoSpedisciCompleted;

        public event DocumentoGetTipologiaAttoCompletedEventHandler DocumentoGetTipologiaAttoCompleted;

        public event DocumentoModificaVersioneCompletedEventHandler DocumentoModificaVersioneCompleted;

        public event DocumentoScambiaVersioniCompletedEventHandler DocumentoScambiaVersioniCompleted;

        public event DocumentoAggiungiVersioneCompletedEventHandler DocumentoAggiungiVersioneCompleted;

        public event DocumentoRimuoviVersioneCompletedEventHandler DocumentoRimuoviVersioneCompleted;

        public event DocumentoModificaAllegatoCompletedEventHandler DocumentoModificaAllegatoCompleted;

        public event DocumentoAggiungiAllegatoCompletedEventHandler DocumentoAggiungiAllegatoCompleted;

        public event DocumentoGetAllegatiCompletedEventHandler DocumentoGetAllegatiCompleted;

        public event DocumentoExecRimuoviSchedaCompletedEventHandler DocumentoExecRimuoviSchedaCompleted;

        public event DocumentoGetParoleChiaveCompletedEventHandler DocumentoGetParoleChiaveCompleted;

        public event DocumentoGetVisibilitaCompletedEventHandler DocumentoGetVisibilitaCompleted;

        public event DocumentoAddDocGrigiaCompletedEventHandler DocumentoAddDocGrigiaCompleted;

        public event DocumentoSaveDocumentoCompletedEventHandler DocumentoSaveDocumentoCompleted;

        public event DocumentoCancellaAreaLavoroCompletedEventHandler DocumentoCancellaAreaLavoroCompleted;

        public event DocumentoGetAreaLavoroPagingCompletedEventHandler DocumentoGetAreaLavoroPagingCompleted;

        public event DocumentoGetAreaLavoroCompletedEventHandler DocumentoGetAreaLavoroCompleted;

        public event DocumentoExecAnnullaProtCompletedEventHandler DocumentoExecAnnullaProtCompleted;

        public event DocumentoExecAddLavoroCompletedEventHandler DocumentoExecAddLavoroCompleted;

        public event documentoImportaProtocolloEmergenzaCompletedEventHandler documentoImportaProtocolloEmergenzaCompleted;

        public event DocumentoProtocollaCompletedEventHandler DocumentoProtocollaCompleted;

        public event DocumentoGetListaOggettiCompletedEventHandler DocumentoGetListaOggettiCompleted;

        public event DocumentoGetDettaglioDocumentoCompletedEventHandler DocumentoGetDettaglioDocumentoCompleted;

        public event DocumentoPutFileBatchCompletedEventHandler DocumentoPutFileBatchCompleted;

        public event DocumentoPutFileCompletedEventHandler DocumentoPutFileCompleted;

        public event DocumentoGetFileCompletedEventHandler DocumentoGetFileCompleted;

        public event DocumentoGetQueryFullTextDocumentoPagingCompletedEventHandler DocumentoGetQueryFullTextDocumentoPagingCompleted;

        public event DocumentoGetQueryDocumentoPagingCompletedEventHandler DocumentoGetQueryDocumentoPagingCompleted;

        public event DocumentoGetQueryDocumentoCompletedEventHandler DocumentoGetQueryDocumentoCompleted;

        public event doucmentoGetDocTypeCompletedEventHandler doucmentoGetDocTypeCompleted;

        public event UtenteChangePasswordCompletedEventHandler UtenteChangePasswordCompleted;

        public event UtenteGetRegistriCompletedEventHandler UtenteGetRegistriCompleted;

        public event RegistriModificaCompletedEventHandler RegistriModificaCompleted;

        public event RegistriCambiaStatoCompletedEventHandler RegistriCambiaStatoCompleted;

        public event ElencoUtentiConnessiCompletedEventHandler ElencoUtentiConnessiCompleted;

        public event DisconnettiUtenteCompletedEventHandler DisconnettiUtenteCompleted;

        public event LogoffRDECompletedEventHandler LogoffRDECompleted;

        public event LogoffCompletedEventHandler LogoffCompleted;

        public event ValidateLoginCompletedEventHandler ValidateLoginCompleted;

        public event LoginCompletedEventHandler LoginCompleted;

        public event SimpleLoginCompletedEventHandler SimpleLoginCompleted;

        public event CheckDatabaseConnectionCompletedEventHandler CheckDatabaseConnectionCompleted;

        public event CheckDPA_AmministraCompletedEventHandler CheckDPA_AmministraCompleted;

        public event IsInternalProtocolEnabledCompletedEventHandler IsInternalProtocolEnabledCompleted;

        public event DocumentoGetTipoProtoCompletedEventHandler DocumentoGetTipoProtoCompleted;

        public event GetVisibilitaRuoliCompletedEventHandler GetVisibilitaRuoliCompleted;

        public event trasmissioniSendSollecitoCompletedEventHandler trasmissioniSendSollecitoCompleted;

        public event TrasmettiProtocolloInternoCompletedEventHandler TrasmettiProtocolloInternoCompleted;

        public event GetRagioneTrasmissioneCompletedEventHandler GetRagioneTrasmissioneCompleted;

        public event UOHasReferenceRoleCompletedEventHandler UOHasReferenceRoleCompleted;

        public event reportFascetteFascicoloCompletedEventHandler reportFascetteFascicoloCompleted;

        public event AmmGetIDAmmCompletedEventHandler AmmGetIDAmmCompleted;

        public event GetFilesLogCompletedEventHandler GetFilesLogCompleted;

        public event StampaPDFLogCompletedEventHandler StampaPDFLogCompleted;

        public event ContaLogCompletedEventHandler ContaLogCompleted;

        public event GetXMLLogFiltratoCompletedEventHandler GetXMLLogFiltratoCompleted;

        public event SetXMLLogCompletedEventHandler SetXMLLogCompleted;

        public event GetXMLLogCompletedEventHandler GetXMLLogCompleted;

        public event MoveRoleToNewUOCompletedEventHandler MoveRoleToNewUOCompleted;

        public event DeleteUserStatusCompletedEventHandler DeleteUserStatusCompleted;

        public event DeleteUserInRoleCompletedEventHandler DeleteUserInRoleCompleted;

        public event DeleteOrDisablePeopleCompletedEventHandler DeleteOrDisablePeopleCompleted;

        public event GetUserStatusCompletedEventHandler GetUserStatusCompleted;

        public event CheckUserLoginCompletedEventHandler CheckUserLoginCompleted;

        public event MoveUserFromRoleCompletedEventHandler MoveUserFromRoleCompleted;

        public event ExportTitolarioCompletedEventHandler ExportTitolarioCompleted;

        public event ExportAmministrazioniCompletedEventHandler ExportAmministrazioniCompleted;

        public event ImportTitolarioReadStateCompletedEventHandler ImportTitolarioReadStateCompleted;

        public event ImportDocumentiReadStateCompletedEventHandler ImportDocumentiReadStateCompleted;

        public event ImportAmministrazioniReadStateCompletedEventHandler ImportAmministrazioniReadStateCompleted;

        public event ImportCorrispondentiReadStateCompletedEventHandler ImportCorrispondentiReadStateCompleted;

        public event CreateTitolarioCompletedEventHandler CreateTitolarioCompleted;

        public event CreateAmministrazioniCompletedEventHandler CreateAmministrazioniCompleted;

        public event UpdateDataCompletedEventHandler UpdateDataCompleted;

        public event RefreshAmministrazioneCompletedEventHandler RefreshAmministrazioneCompleted;

        public event ImportCorrispondentiCompletedEventHandler ImportCorrispondentiCompleted;

        public event ImportDocumentiCompletedEventHandler ImportDocumentiCompleted;

        public event ImportOggettarioCompletedEventHandler ImportOggettarioCompleted;

        public event ImportStoricoOggettarioCompletedEventHandler ImportStoricoOggettarioCompleted;

        public event ImportTipiAttoCompletedEventHandler ImportTipiAttoCompleted;

        public event ImportTipiDocumentoCompletedEventHandler ImportTipiDocumentoCompleted;

        public event LoginAmministratoreCompletedEventHandler LoginAmministratoreCompleted;

        public event LogoutAmministratoreCompletedEventHandler LogoutAmministratoreCompleted;

        public event CheckAdministratorCompletedEventHandler CheckAdministratorCompleted;

        public event CambiaPwdAmministratoreCompletedEventHandler CambiaPwdAmministratoreCompleted;

        public event NodoTitolarioCompletedEventHandler NodoTitolarioCompleted;

        public event NodoTitolarioSecurityCompletedEventHandler NodoTitolarioSecurityCompleted;

        public event RegistriInAmmCompletedEventHandler RegistriInAmmCompleted;

        public event SecurityNodoRuoliCompletedEventHandler SecurityNodoRuoliCompleted;

        public event UpdNodo_TitolarioCompletedEventHandler UpdNodo_TitolarioCompleted;

        public event filtroRicercaTitCompletedEventHandler filtroRicercaTitCompleted;

        public event filtroRicercaTitDocspaCompletedEventHandler filtroRicercaTitDocspaCompleted;

        public event filtroRicercaTitAmmCompletedEventHandler filtroRicercaTitAmmCompleted;

        public event findNodoRootCompletedEventHandler findNodoRootCompleted;

        public event findNodoByCodCompletedEventHandler findNodoByCodCompleted;

        public event AggNewNodoCompletedEventHandler AggNewNodoCompleted;

        public event EliminaNodoCompletedEventHandler EliminaNodoCompleted;

        public event GetCodLivCompletedEventHandler GetCodLivCompleted;

        public event SpostaNodoTitolarioCompletedEventHandler SpostaNodoTitolarioCompleted;

        public DocsPaWebService()
        {
            this.Url = Settings.Default.StampaRegistri_DocsPaWR305_DocsPaWebService;
            if (this.IsLocalFileSystemWebService(this.Url))
            {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else
                this.useDefaultCredentialsSetExplicitly = true;
        }

        [SoapDocumentMethod("http://localhost/DO_GetListaStoriciMittente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DocumentoStoricoMittente[] DO_GetListaStoriciMittente(string idProfile, string tipo)
        {
            return (DocumentoStoricoMittente[])this.Invoke("DO_GetListaStoriciMittente", new object[2]
      {
        (object) idProfile,
        (object) tipo
      })[0];
        }

        public IAsyncResult BeginDO_GetListaStoriciMittente(string idProfile, string tipo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetListaStoriciMittente", new object[2]
      {
        (object) idProfile,
        (object) tipo
      }, callback, asyncState);
        }

        public DocumentoStoricoMittente[] EndDO_GetListaStoriciMittente(IAsyncResult asyncResult)
        {
            return (DocumentoStoricoMittente[])this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetListaStoriciMittenteAsync(string idProfile, string tipo)
        {
            this.DO_GetListaStoriciMittenteAsync(idProfile, tipo, (object)null);
        }

        public void DO_GetListaStoriciMittenteAsync(string idProfile, string tipo, object userState)
        {
            if (this.DO_GetListaStoriciMittenteOperationCompleted == null)
                this.DO_GetListaStoriciMittenteOperationCompleted = new SendOrPostCallback(this.OnDO_GetListaStoriciMittenteOperationCompleted);
            this.InvokeAsync("DO_GetListaStoriciMittente", new object[2]
      {
        (object) idProfile,
        (object) tipo
      }, this.DO_GetListaStoriciMittenteOperationCompleted, userState);
        }

        private void OnDO_GetListaStoriciMittenteOperationCompleted(object arg)
        {
            if (this.DO_GetListaStoriciMittenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetListaStoriciMittenteCompleted((object)this, new DO_GetListaStoriciMittenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_TrasmettiDestinatariModificati", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DO_TrasmettiDestinatariModificati(SchedaDocumento scheda, Ruolo ruolo, string serverName)
        {
            return (bool)this.Invoke("DO_TrasmettiDestinatariModificati", new object[3]
      {
        (object) scheda,
        (object) ruolo,
        (object) serverName
      })[0];
        }

        public IAsyncResult BeginDO_TrasmettiDestinatariModificati(SchedaDocumento scheda, Ruolo ruolo, string serverName, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_TrasmettiDestinatariModificati", new object[3]
      {
        (object) scheda,
        (object) ruolo,
        (object) serverName
      }, callback, asyncState);
        }

        public bool EndDO_TrasmettiDestinatariModificati(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DO_TrasmettiDestinatariModificatiAsync(SchedaDocumento scheda, Ruolo ruolo, string serverName)
        {
            this.DO_TrasmettiDestinatariModificatiAsync(scheda, ruolo, serverName, (object)null);
        }

        public void DO_TrasmettiDestinatariModificatiAsync(SchedaDocumento scheda, Ruolo ruolo, string serverName, object userState)
        {
            if (this.DO_TrasmettiDestinatariModificatiOperationCompleted == null)
                this.DO_TrasmettiDestinatariModificatiOperationCompleted = new SendOrPostCallback(this.OnDO_TrasmettiDestinatariModificatiOperationCompleted);
            this.InvokeAsync("DO_TrasmettiDestinatariModificati", new object[3]
      {
        (object) scheda,
        (object) ruolo,
        (object) serverName
      }, this.DO_TrasmettiDestinatariModificatiOperationCompleted, userState);
        }

        private void OnDO_TrasmettiDestinatariModificatiOperationCompleted(object arg)
        {
            if (this.DO_TrasmettiDestinatariModificatiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_TrasmettiDestinatariModificatiCompleted((object)this, new DO_TrasmettiDestinatariModificatiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_RemoveDestinatrioCCModificato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento DO_RemoveDestinatrioCCModificato(SchedaDocumento scheda, string systemID)
        {
            return (SchedaDocumento)this.Invoke("DO_RemoveDestinatrioCCModificato", new object[2]
      {
        (object) scheda,
        (object) systemID
      })[0];
        }

        public IAsyncResult BeginDO_RemoveDestinatrioCCModificato(SchedaDocumento scheda, string systemID, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_RemoveDestinatrioCCModificato", new object[2]
      {
        (object) scheda,
        (object) systemID
      }, callback, asyncState);
        }

        public SchedaDocumento EndDO_RemoveDestinatrioCCModificato(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void DO_RemoveDestinatrioCCModificatoAsync(SchedaDocumento scheda, string systemID)
        {
            this.DO_RemoveDestinatrioCCModificatoAsync(scheda, systemID, (object)null);
        }

        public void DO_RemoveDestinatrioCCModificatoAsync(SchedaDocumento scheda, string systemID, object userState)
        {
            if (this.DO_RemoveDestinatrioCCModificatoOperationCompleted == null)
                this.DO_RemoveDestinatrioCCModificatoOperationCompleted = new SendOrPostCallback(this.OnDO_RemoveDestinatrioCCModificatoOperationCompleted);
            this.InvokeAsync("DO_RemoveDestinatrioCCModificato", new object[2]
      {
        (object) scheda,
        (object) systemID
      }, this.DO_RemoveDestinatrioCCModificatoOperationCompleted, userState);
        }

        private void OnDO_RemoveDestinatrioCCModificatoOperationCompleted(object arg)
        {
            if (this.DO_RemoveDestinatrioCCModificatoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_RemoveDestinatrioCCModificatoCompleted((object)this, new DO_RemoveDestinatrioCCModificatoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_ClearDestinatariCCModificati", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento DO_ClearDestinatariCCModificati(SchedaDocumento scheda)
        {
            return (SchedaDocumento)this.Invoke("DO_ClearDestinatariCCModificati", new object[1]
      {
        (object) scheda
      })[0];
        }

        public IAsyncResult BeginDO_ClearDestinatariCCModificati(SchedaDocumento scheda, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_ClearDestinatariCCModificati", new object[1]
      {
        (object) scheda
      }, callback, asyncState);
        }

        public SchedaDocumento EndDO_ClearDestinatariCCModificati(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void DO_ClearDestinatariCCModificatiAsync(SchedaDocumento scheda)
        {
            this.DO_ClearDestinatariCCModificatiAsync(scheda, (object)null);
        }

        public void DO_ClearDestinatariCCModificatiAsync(SchedaDocumento scheda, object userState)
        {
            if (this.DO_ClearDestinatariCCModificatiOperationCompleted == null)
                this.DO_ClearDestinatariCCModificatiOperationCompleted = new SendOrPostCallback(this.OnDO_ClearDestinatariCCModificatiOperationCompleted);
            this.InvokeAsync("DO_ClearDestinatariCCModificati", new object[1]
      {
        (object) scheda
      }, this.DO_ClearDestinatariCCModificatiOperationCompleted, userState);
        }

        private void OnDO_ClearDestinatariCCModificatiOperationCompleted(object arg)
        {
            if (this.DO_ClearDestinatariCCModificatiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_ClearDestinatariCCModificatiCompleted((object)this, new DO_ClearDestinatariCCModificatiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_ClearDestinatariModificati", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento DO_ClearDestinatariModificati(SchedaDocumento scheda)
        {
            return (SchedaDocumento)this.Invoke("DO_ClearDestinatariModificati", new object[1]
      {
        (object) scheda
      })[0];
        }

        public IAsyncResult BeginDO_ClearDestinatariModificati(SchedaDocumento scheda, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_ClearDestinatariModificati", new object[1]
      {
        (object) scheda
      }, callback, asyncState);
        }

        public SchedaDocumento EndDO_ClearDestinatariModificati(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void DO_ClearDestinatariModificatiAsync(SchedaDocumento scheda)
        {
            this.DO_ClearDestinatariModificatiAsync(scheda, (object)null);
        }

        public void DO_ClearDestinatariModificatiAsync(SchedaDocumento scheda, object userState)
        {
            if (this.DO_ClearDestinatariModificatiOperationCompleted == null)
                this.DO_ClearDestinatariModificatiOperationCompleted = new SendOrPostCallback(this.OnDO_ClearDestinatariModificatiOperationCompleted);
            this.InvokeAsync("DO_ClearDestinatariModificati", new object[1]
      {
        (object) scheda
      }, this.DO_ClearDestinatariModificatiOperationCompleted, userState);
        }

        private void OnDO_ClearDestinatariModificatiOperationCompleted(object arg)
        {
            if (this.DO_ClearDestinatariModificatiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_ClearDestinatariModificatiCompleted((object)this, new DO_ClearDestinatariModificatiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_RemoveDestinatrioModificato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento DO_RemoveDestinatrioModificato(SchedaDocumento scheda, string systemID)
        {
            return (SchedaDocumento)this.Invoke("DO_RemoveDestinatrioModificato", new object[2]
      {
        (object) scheda,
        (object) systemID
      })[0];
        }

        public IAsyncResult BeginDO_RemoveDestinatrioModificato(SchedaDocumento scheda, string systemID, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_RemoveDestinatrioModificato", new object[2]
      {
        (object) scheda,
        (object) systemID
      }, callback, asyncState);
        }

        public SchedaDocumento EndDO_RemoveDestinatrioModificato(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void DO_RemoveDestinatrioModificatoAsync(SchedaDocumento scheda, string systemID)
        {
            this.DO_RemoveDestinatrioModificatoAsync(scheda, systemID, (object)null);
        }

        public void DO_RemoveDestinatrioModificatoAsync(SchedaDocumento scheda, string systemID, object userState)
        {
            if (this.DO_RemoveDestinatrioModificatoOperationCompleted == null)
                this.DO_RemoveDestinatrioModificatoOperationCompleted = new SendOrPostCallback(this.OnDO_RemoveDestinatrioModificatoOperationCompleted);
            this.InvokeAsync("DO_RemoveDestinatrioModificato", new object[2]
      {
        (object) scheda,
        (object) systemID
      }, this.DO_RemoveDestinatrioModificatoOperationCompleted, userState);
        }

        private void OnDO_RemoveDestinatrioModificatoOperationCompleted(object arg)
        {
            if (this.DO_RemoveDestinatrioModificatoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_RemoveDestinatrioModificatoCompleted((object)this, new DO_RemoveDestinatrioModificatoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_AddDestinatrioCCModificato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento DO_AddDestinatrioCCModificato(SchedaDocumento scheda, string systemID)
        {
            return (SchedaDocumento)this.Invoke("DO_AddDestinatrioCCModificato", new object[2]
      {
        (object) scheda,
        (object) systemID
      })[0];
        }

        public IAsyncResult BeginDO_AddDestinatrioCCModificato(SchedaDocumento scheda, string systemID, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_AddDestinatrioCCModificato", new object[2]
      {
        (object) scheda,
        (object) systemID
      }, callback, asyncState);
        }

        public SchedaDocumento EndDO_AddDestinatrioCCModificato(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void DO_AddDestinatrioCCModificatoAsync(SchedaDocumento scheda, string systemID)
        {
            this.DO_AddDestinatrioCCModificatoAsync(scheda, systemID, (object)null);
        }

        public void DO_AddDestinatrioCCModificatoAsync(SchedaDocumento scheda, string systemID, object userState)
        {
            if (this.DO_AddDestinatrioCCModificatoOperationCompleted == null)
                this.DO_AddDestinatrioCCModificatoOperationCompleted = new SendOrPostCallback(this.OnDO_AddDestinatrioCCModificatoOperationCompleted);
            this.InvokeAsync("DO_AddDestinatrioCCModificato", new object[2]
      {
        (object) scheda,
        (object) systemID
      }, this.DO_AddDestinatrioCCModificatoOperationCompleted, userState);
        }

        private void OnDO_AddDestinatrioCCModificatoOperationCompleted(object arg)
        {
            if (this.DO_AddDestinatrioCCModificatoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_AddDestinatrioCCModificatoCompleted((object)this, new DO_AddDestinatrioCCModificatoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_AddDestinatrioModificato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento DO_AddDestinatrioModificato(SchedaDocumento scheda, string systemID)
        {
            return (SchedaDocumento)this.Invoke("DO_AddDestinatrioModificato", new object[2]
      {
        (object) scheda,
        (object) systemID
      })[0];
        }

        public IAsyncResult BeginDO_AddDestinatrioModificato(SchedaDocumento scheda, string systemID, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_AddDestinatrioModificato", new object[2]
      {
        (object) scheda,
        (object) systemID
      }, callback, asyncState);
        }

        public SchedaDocumento EndDO_AddDestinatrioModificato(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void DO_AddDestinatrioModificatoAsync(SchedaDocumento scheda, string systemID)
        {
            this.DO_AddDestinatrioModificatoAsync(scheda, systemID, (object)null);
        }

        public void DO_AddDestinatrioModificatoAsync(SchedaDocumento scheda, string systemID, object userState)
        {
            if (this.DO_AddDestinatrioModificatoOperationCompleted == null)
                this.DO_AddDestinatrioModificatoOperationCompleted = new SendOrPostCallback(this.OnDO_AddDestinatrioModificatoOperationCompleted);
            this.InvokeAsync("DO_AddDestinatrioModificato", new object[2]
      {
        (object) scheda,
        (object) systemID
      }, this.DO_AddDestinatrioModificatoOperationCompleted, userState);
        }

        private void OnDO_AddDestinatrioModificatoOperationCompleted(object arg)
        {
            if (this.DO_AddDestinatrioModificatoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_AddDestinatrioModificatoCompleted((object)this, new DO_AddDestinatrioModificatoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetUrlUploadAcquisizione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string GetUrlUploadAcquisizione()
        {
            return (string)this.Invoke("GetUrlUploadAcquisizione", new object[0])[0];
        }

        public IAsyncResult BeginGetUrlUploadAcquisizione(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetUrlUploadAcquisizione", new object[0], callback, asyncState);
        }

        public string EndGetUrlUploadAcquisizione(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void GetUrlUploadAcquisizioneAsync()
        {
            this.GetUrlUploadAcquisizioneAsync((object)null);
        }

        public void GetUrlUploadAcquisizioneAsync(object userState)
        {
            if (this.GetUrlUploadAcquisizioneOperationCompleted == null)
                this.GetUrlUploadAcquisizioneOperationCompleted = new SendOrPostCallback(this.OnGetUrlUploadAcquisizioneOperationCompleted);
            this.InvokeAsync("GetUrlUploadAcquisizione", new object[0], this.GetUrlUploadAcquisizioneOperationCompleted, userState);
        }

        private void OnGetUrlUploadAcquisizioneOperationCompleted(object arg)
        {
            if (this.GetUrlUploadAcquisizioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetUrlUploadAcquisizioneCompleted((object)this, new GetUrlUploadAcquisizioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetPathAcquisizioneBatch", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string GetPathAcquisizioneBatch()
        {
            return (string)this.Invoke("GetPathAcquisizioneBatch", new object[0])[0];
        }

        public IAsyncResult BeginGetPathAcquisizioneBatch(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetPathAcquisizioneBatch", new object[0], callback, asyncState);
        }

        public string EndGetPathAcquisizioneBatch(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void GetPathAcquisizioneBatchAsync()
        {
            this.GetPathAcquisizioneBatchAsync((object)null);
        }

        public void GetPathAcquisizioneBatchAsync(object userState)
        {
            if (this.GetPathAcquisizioneBatchOperationCompleted == null)
                this.GetPathAcquisizioneBatchOperationCompleted = new SendOrPostCallback(this.OnGetPathAcquisizioneBatchOperationCompleted);
            this.InvokeAsync("GetPathAcquisizioneBatch", new object[0], this.GetPathAcquisizioneBatchOperationCompleted, userState);
        }

        private void OnGetPathAcquisizioneBatchOperationCompleted(object arg)
        {
            if (this.GetPathAcquisizioneBatchCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetPathAcquisizioneBatchCompleted((object)this, new GetPathAcquisizioneBatchCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/getTemplatePerRicerca", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Templates getTemplatePerRicerca(string idAmministrazione, string tipoAtto)
        {
            return (Templates)this.Invoke("getTemplatePerRicerca", new object[2]
      {
        (object) idAmministrazione,
        (object) tipoAtto
      })[0];
        }

        public IAsyncResult BegingetTemplatePerRicerca(string idAmministrazione, string tipoAtto, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getTemplatePerRicerca", new object[2]
      {
        (object) idAmministrazione,
        (object) tipoAtto
      }, callback, asyncState);
        }

        public Templates EndgetTemplatePerRicerca(IAsyncResult asyncResult)
        {
            return (Templates)this.EndInvoke(asyncResult)[0];
        }

        public void getTemplatePerRicercaAsync(string idAmministrazione, string tipoAtto)
        {
            this.getTemplatePerRicercaAsync(idAmministrazione, tipoAtto, (object)null);
        }

        public void getTemplatePerRicercaAsync(string idAmministrazione, string tipoAtto, object userState)
        {
            if (this.getTemplatePerRicercaOperationCompleted == null)
                this.getTemplatePerRicercaOperationCompleted = new SendOrPostCallback(this.OngetTemplatePerRicercaOperationCompleted);
            this.InvokeAsync("getTemplatePerRicerca", new object[2]
      {
        (object) idAmministrazione,
        (object) tipoAtto
      }, this.getTemplatePerRicercaOperationCompleted, userState);
        }

        private void OngetTemplatePerRicercaOperationCompleted(object arg)
        {
            if (this.getTemplatePerRicercaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.getTemplatePerRicercaCompleted((object)this, new getTemplatePerRicercaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/spostaOggetto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Templates spostaOggetto(Templates template, int oggettoSelezionato, string spostamento)
        {
            return (Templates)this.Invoke("spostaOggetto", new object[3]
      {
        (object) template,
        (object) oggettoSelezionato,
        (object) spostamento
      })[0];
        }

        public IAsyncResult BeginspostaOggetto(Templates template, int oggettoSelezionato, string spostamento, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("spostaOggetto", new object[3]
      {
        (object) template,
        (object) oggettoSelezionato,
        (object) spostamento
      }, callback, asyncState);
        }

        public Templates EndspostaOggetto(IAsyncResult asyncResult)
        {
            return (Templates)this.EndInvoke(asyncResult)[0];
        }

        public void spostaOggettoAsync(Templates template, int oggettoSelezionato, string spostamento)
        {
            this.spostaOggettoAsync(template, oggettoSelezionato, spostamento, (object)null);
        }

        public void spostaOggettoAsync(Templates template, int oggettoSelezionato, string spostamento, object userState)
        {
            if (this.spostaOggettoOperationCompleted == null)
                this.spostaOggettoOperationCompleted = new SendOrPostCallback(this.OnspostaOggettoOperationCompleted);
            this.InvokeAsync("spostaOggetto", new object[3]
      {
        (object) template,
        (object) oggettoSelezionato,
        (object) spostamento
      }, this.spostaOggettoOperationCompleted, userState);
        }

        private void OnspostaOggettoOperationCompleted(object arg)
        {
            if (this.spostaOggettoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.spostaOggettoCompleted((object)this, new spostaOggettoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/messaInEserizioTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void messaInEserizioTemplate(Templates template, string idAmministrazione)
        {
            this.Invoke("messaInEserizioTemplate", new object[2]
      {
        (object) template,
        (object) idAmministrazione
      });
        }

        public IAsyncResult BeginmessaInEserizioTemplate(Templates template, string idAmministrazione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("messaInEserizioTemplate", new object[2]
      {
        (object) template,
        (object) idAmministrazione
      }, callback, asyncState);
        }

        public void EndmessaInEserizioTemplate(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void messaInEserizioTemplateAsync(Templates template, string idAmministrazione)
        {
            this.messaInEserizioTemplateAsync(template, idAmministrazione, (object)null);
        }

        public void messaInEserizioTemplateAsync(Templates template, string idAmministrazione, object userState)
        {
            if (this.messaInEserizioTemplateOperationCompleted == null)
                this.messaInEserizioTemplateOperationCompleted = new SendOrPostCallback(this.OnmessaInEserizioTemplateOperationCompleted);
            this.InvokeAsync("messaInEserizioTemplate", new object[2]
      {
        (object) template,
        (object) idAmministrazione
      }, this.messaInEserizioTemplateOperationCompleted, userState);
        }

        private void OnmessaInEserizioTemplateOperationCompleted(object arg)
        {
            if (this.messaInEserizioTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.messaInEserizioTemplateCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/salvaInserimentoUtenteProfDim", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void salvaInserimentoUtenteProfDim(Templates template, string docNumber)
        {
            this.Invoke("salvaInserimentoUtenteProfDim", new object[2]
      {
        (object) template,
        (object) docNumber
      });
        }

        public IAsyncResult BeginsalvaInserimentoUtenteProfDim(Templates template, string docNumber, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("salvaInserimentoUtenteProfDim", new object[2]
      {
        (object) template,
        (object) docNumber
      }, callback, asyncState);
        }

        public void EndsalvaInserimentoUtenteProfDim(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void salvaInserimentoUtenteProfDimAsync(Templates template, string docNumber)
        {
            this.salvaInserimentoUtenteProfDimAsync(template, docNumber, (object)null);
        }

        public void salvaInserimentoUtenteProfDimAsync(Templates template, string docNumber, object userState)
        {
            if (this.salvaInserimentoUtenteProfDimOperationCompleted == null)
                this.salvaInserimentoUtenteProfDimOperationCompleted = new SendOrPostCallback(this.OnsalvaInserimentoUtenteProfDimOperationCompleted);
            this.InvokeAsync("salvaInserimentoUtenteProfDim", new object[2]
      {
        (object) template,
        (object) docNumber
      }, this.salvaInserimentoUtenteProfDimOperationCompleted, userState);
        }

        private void OnsalvaInserimentoUtenteProfDimOperationCompleted(object arg)
        {
            if (this.salvaInserimentoUtenteProfDimCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.salvaInserimentoUtenteProfDimCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/getIdAmmByCod", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string getIdAmmByCod(string codiceAmministrazione)
        {
            return (string)this.Invoke("getIdAmmByCod", new object[1]
      {
        (object) codiceAmministrazione
      })[0];
        }

        public IAsyncResult BegingetIdAmmByCod(string codiceAmministrazione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getIdAmmByCod", new object[1]
      {
        (object) codiceAmministrazione
      }, callback, asyncState);
        }

        public string EndgetIdAmmByCod(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void getIdAmmByCodAsync(string codiceAmministrazione)
        {
            this.getIdAmmByCodAsync(codiceAmministrazione, (object)null);
        }

        public void getIdAmmByCodAsync(string codiceAmministrazione, object userState)
        {
            if (this.getIdAmmByCodOperationCompleted == null)
                this.getIdAmmByCodOperationCompleted = new SendOrPostCallback(this.OngetIdAmmByCodOperationCompleted);
            this.InvokeAsync("getIdAmmByCod", new object[1]
      {
        (object) codiceAmministrazione
      }, this.getIdAmmByCodOperationCompleted, userState);
        }

        private void OngetIdAmmByCodOperationCompleted(object arg)
        {
            if (this.getIdAmmByCodCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.getIdAmmByCodCompleted((object)this, new getIdAmmByCodCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/getTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Templates getTemplate(string idAmministrazione, string tipoAtto, string docNumber)
        {
            return (Templates)this.Invoke("getTemplate", new object[3]
      {
        (object) idAmministrazione,
        (object) tipoAtto,
        (object) docNumber
      })[0];
        }

        public IAsyncResult BegingetTemplate(string idAmministrazione, string tipoAtto, string docNumber, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getTemplate", new object[3]
      {
        (object) idAmministrazione,
        (object) tipoAtto,
        (object) docNumber
      }, callback, asyncState);
        }

        public Templates EndgetTemplate(IAsyncResult asyncResult)
        {
            return (Templates)this.EndInvoke(asyncResult)[0];
        }

        public void getTemplateAsync(string idAmministrazione, string tipoAtto, string docNumber)
        {
            this.getTemplateAsync(idAmministrazione, tipoAtto, docNumber, (object)null);
        }

        public void getTemplateAsync(string idAmministrazione, string tipoAtto, string docNumber, object userState)
        {
            if (this.getTemplateOperationCompleted == null)
                this.getTemplateOperationCompleted = new SendOrPostCallback(this.OngetTemplateOperationCompleted);
            this.InvokeAsync("getTemplate", new object[3]
      {
        (object) idAmministrazione,
        (object) tipoAtto,
        (object) docNumber
      }, this.getTemplateOperationCompleted, userState);
        }

        private void OngetTemplateOperationCompleted(object arg)
        {
            if (this.getTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.getTemplateCompleted((object)this, new getTemplateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/disabilitaTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool disabilitaTemplate(Templates template, string idAmministrazione)
        {
            return (bool)this.Invoke("disabilitaTemplate", new object[2]
      {
        (object) template,
        (object) idAmministrazione
      })[0];
        }

        public IAsyncResult BegindisabilitaTemplate(Templates template, string idAmministrazione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("disabilitaTemplate", new object[2]
      {
        (object) template,
        (object) idAmministrazione
      }, callback, asyncState);
        }

        public bool EnddisabilitaTemplate(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void disabilitaTemplateAsync(Templates template, string idAmministrazione)
        {
            this.disabilitaTemplateAsync(template, idAmministrazione, (object)null);
        }

        public void disabilitaTemplateAsync(Templates template, string idAmministrazione, object userState)
        {
            if (this.disabilitaTemplateOperationCompleted == null)
                this.disabilitaTemplateOperationCompleted = new SendOrPostCallback(this.OndisabilitaTemplateOperationCompleted);
            this.InvokeAsync("disabilitaTemplate", new object[2]
      {
        (object) template,
        (object) idAmministrazione
      }, this.disabilitaTemplateOperationCompleted, userState);
        }

        private void OndisabilitaTemplateOperationCompleted(object arg)
        {
            if (this.disabilitaTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.disabilitaTemplateCompleted((object)this, new disabilitaTemplateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/getTemplates", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public object[] getTemplates(string idAmministrazione)
        {
            return (object[])this.Invoke("getTemplates", new object[1]
      {
        (object) idAmministrazione
      })[0];
        }

        public IAsyncResult BegingetTemplates(string idAmministrazione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getTemplates", new object[1]
      {
        (object) idAmministrazione
      }, callback, asyncState);
        }

        public object[] EndgetTemplates(IAsyncResult asyncResult)
        {
            return (object[])this.EndInvoke(asyncResult)[0];
        }

        public void getTemplatesAsync(string idAmministrazione)
        {
            this.getTemplatesAsync(idAmministrazione, (object)null);
        }

        public void getTemplatesAsync(string idAmministrazione, object userState)
        {
            if (this.getTemplatesOperationCompleted == null)
                this.getTemplatesOperationCompleted = new SendOrPostCallback(this.OngetTemplatesOperationCompleted);
            this.InvokeAsync("getTemplates", new object[1]
      {
        (object) idAmministrazione
      }, this.getTemplatesOperationCompleted, userState);
        }

        private void OngetTemplatesOperationCompleted(object arg)
        {
            if (this.getTemplatesCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.getTemplatesCompleted((object)this, new getTemplatesCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/aggiornaTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool aggiornaTemplate(Templates template)
        {
            return (bool)this.Invoke("aggiornaTemplate", new object[1]
      {
        (object) template
      })[0];
        }

        public IAsyncResult BeginaggiornaTemplate(Templates template, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("aggiornaTemplate", new object[1]
      {
        (object) template
      }, callback, asyncState);
        }

        public bool EndaggiornaTemplate(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void aggiornaTemplateAsync(Templates template)
        {
            this.aggiornaTemplateAsync(template, (object)null);
        }

        public void aggiornaTemplateAsync(Templates template, object userState)
        {
            if (this.aggiornaTemplateOperationCompleted == null)
                this.aggiornaTemplateOperationCompleted = new SendOrPostCallback(this.OnaggiornaTemplateOperationCompleted);
            this.InvokeAsync("aggiornaTemplate", new object[1]
      {
        (object) template
      }, this.aggiornaTemplateOperationCompleted, userState);
        }

        private void OnaggiornaTemplateOperationCompleted(object arg)
        {
            if (this.aggiornaTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.aggiornaTemplateCompleted((object)this, new aggiornaTemplateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/salvaTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool salvaTemplate(Templates template, string idAmministrazione)
        {
            return (bool)this.Invoke("salvaTemplate", new object[2]
      {
        (object) template,
        (object) idAmministrazione
      })[0];
        }

        public IAsyncResult BeginsalvaTemplate(Templates template, string idAmministrazione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("salvaTemplate", new object[2]
      {
        (object) template,
        (object) idAmministrazione
      }, callback, asyncState);
        }

        public bool EndsalvaTemplate(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void salvaTemplateAsync(Templates template, string idAmministrazione)
        {
            this.salvaTemplateAsync(template, idAmministrazione, (object)null);
        }

        public void salvaTemplateAsync(Templates template, string idAmministrazione, object userState)
        {
            if (this.salvaTemplateOperationCompleted == null)
                this.salvaTemplateOperationCompleted = new SendOrPostCallback(this.OnsalvaTemplateOperationCompleted);
            this.InvokeAsync("salvaTemplate", new object[2]
      {
        (object) template,
        (object) idAmministrazione
      }, this.salvaTemplateOperationCompleted, userState);
        }

        private void OnsalvaTemplateOperationCompleted(object arg)
        {
            if (this.salvaTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.salvaTemplateCompleted((object)this, new salvaTemplateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/aggiungiOggettoValoreOggettoCustom", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Templates aggiungiOggettoValoreOggettoCustom(ValoreOggetto valoreOggetto, int oggettoCustom, Templates template, bool cancellazione)
        {
            return (Templates)this.Invoke("aggiungiOggettoValoreOggettoCustom", new object[4]
      {
        (object) valoreOggetto,
        (object) oggettoCustom,
        (object) template,
        (object) cancellazione
      })[0];
        }

        public IAsyncResult BeginaggiungiOggettoValoreOggettoCustom(ValoreOggetto valoreOggetto, int oggettoCustom, Templates template, bool cancellazione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("aggiungiOggettoValoreOggettoCustom", new object[4]
      {
        (object) valoreOggetto,
        (object) oggettoCustom,
        (object) template,
        (object) cancellazione
      }, callback, asyncState);
        }

        public Templates EndaggiungiOggettoValoreOggettoCustom(IAsyncResult asyncResult)
        {
            return (Templates)this.EndInvoke(asyncResult)[0];
        }

        public void aggiungiOggettoValoreOggettoCustomAsync(ValoreOggetto valoreOggetto, int oggettoCustom, Templates template, bool cancellazione)
        {
            this.aggiungiOggettoValoreOggettoCustomAsync(valoreOggetto, oggettoCustom, template, cancellazione, (object)null);
        }

        public void aggiungiOggettoValoreOggettoCustomAsync(ValoreOggetto valoreOggetto, int oggettoCustom, Templates template, bool cancellazione, object userState)
        {
            if (this.aggiungiOggettoValoreOggettoCustomOperationCompleted == null)
                this.aggiungiOggettoValoreOggettoCustomOperationCompleted = new SendOrPostCallback(this.OnaggiungiOggettoValoreOggettoCustomOperationCompleted);
            this.InvokeAsync("aggiungiOggettoValoreOggettoCustom", new object[4]
      {
        (object) valoreOggetto,
        (object) oggettoCustom,
        (object) template,
        (object) cancellazione
      }, this.aggiungiOggettoValoreOggettoCustomOperationCompleted, userState);
        }

        private void OnaggiungiOggettoValoreOggettoCustomOperationCompleted(object arg)
        {
            if (this.aggiungiOggettoValoreOggettoCustomCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.aggiungiOggettoValoreOggettoCustomCompleted((object)this, new aggiungiOggettoValoreOggettoCustomCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/eliminaOggettoCustomTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Templates eliminaOggettoCustomTemplate(Templates template, int oggettoCustom)
        {
            return (Templates)this.Invoke("eliminaOggettoCustomTemplate", new object[2]
      {
        (object) template,
        (object) oggettoCustom
      })[0];
        }

        public IAsyncResult BegineliminaOggettoCustomTemplate(Templates template, int oggettoCustom, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("eliminaOggettoCustomTemplate", new object[2]
      {
        (object) template,
        (object) oggettoCustom
      }, callback, asyncState);
        }

        public Templates EndeliminaOggettoCustomTemplate(IAsyncResult asyncResult)
        {
            return (Templates)this.EndInvoke(asyncResult)[0];
        }

        public void eliminaOggettoCustomTemplateAsync(Templates template, int oggettoCustom)
        {
            this.eliminaOggettoCustomTemplateAsync(template, oggettoCustom, (object)null);
        }

        public void eliminaOggettoCustomTemplateAsync(Templates template, int oggettoCustom, object userState)
        {
            if (this.eliminaOggettoCustomTemplateOperationCompleted == null)
                this.eliminaOggettoCustomTemplateOperationCompleted = new SendOrPostCallback(this.OneliminaOggettoCustomTemplateOperationCompleted);
            this.InvokeAsync("eliminaOggettoCustomTemplate", new object[2]
      {
        (object) template,
        (object) oggettoCustom
      }, this.eliminaOggettoCustomTemplateOperationCompleted, userState);
        }

        private void OneliminaOggettoCustomTemplateOperationCompleted(object arg)
        {
            if (this.eliminaOggettoCustomTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.eliminaOggettoCustomTemplateCompleted((object)this, new eliminaOggettoCustomTemplateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/aggiungiOggettoCustomTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Templates aggiungiOggettoCustomTemplate(OggettoCustom oggettoCustom, Templates template)
        {
            return (Templates)this.Invoke("aggiungiOggettoCustomTemplate", new object[2]
      {
        (object) oggettoCustom,
        (object) template
      })[0];
        }

        public IAsyncResult BeginaggiungiOggettoCustomTemplate(OggettoCustom oggettoCustom, Templates template, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("aggiungiOggettoCustomTemplate", new object[2]
      {
        (object) oggettoCustom,
        (object) template
      }, callback, asyncState);
        }

        public Templates EndaggiungiOggettoCustomTemplate(IAsyncResult asyncResult)
        {
            return (Templates)this.EndInvoke(asyncResult)[0];
        }

        public void aggiungiOggettoCustomTemplateAsync(OggettoCustom oggettoCustom, Templates template)
        {
            this.aggiungiOggettoCustomTemplateAsync(oggettoCustom, template, (object)null);
        }

        public void aggiungiOggettoCustomTemplateAsync(OggettoCustom oggettoCustom, Templates template, object userState)
        {
            if (this.aggiungiOggettoCustomTemplateOperationCompleted == null)
                this.aggiungiOggettoCustomTemplateOperationCompleted = new SendOrPostCallback(this.OnaggiungiOggettoCustomTemplateOperationCompleted);
            this.InvokeAsync("aggiungiOggettoCustomTemplate", new object[2]
      {
        (object) oggettoCustom,
        (object) template
      }, this.aggiungiOggettoCustomTemplateOperationCompleted, userState);
        }

        private void OnaggiungiOggettoCustomTemplateOperationCompleted(object arg)
        {
            if (this.aggiungiOggettoCustomTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.aggiungiOggettoCustomTemplateCompleted((object)this, new aggiungiOggettoCustomTemplateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/getValoreOggetto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public ValoreOggetto getValoreOggetto()
        {
            return (ValoreOggetto)this.Invoke("getValoreOggetto", new object[0])[0];
        }

        public IAsyncResult BegingetValoreOggetto(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getValoreOggetto", new object[0], callback, asyncState);
        }

        public ValoreOggetto EndgetValoreOggetto(IAsyncResult asyncResult)
        {
            return (ValoreOggetto)this.EndInvoke(asyncResult)[0];
        }

        public void getValoreOggettoAsync()
        {
            this.getValoreOggettoAsync((object)null);
        }

        public void getValoreOggettoAsync(object userState)
        {
            if (this.getValoreOggettoOperationCompleted == null)
                this.getValoreOggettoOperationCompleted = new SendOrPostCallback(this.OngetValoreOggettoOperationCompleted);
            this.InvokeAsync("getValoreOggetto", new object[0], this.getValoreOggettoOperationCompleted, userState);
        }

        private void OngetValoreOggettoOperationCompleted(object arg)
        {
            if (this.getValoreOggettoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.getValoreOggettoCompleted((object)this, new getValoreOggettoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/getTipoOggetto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TipoOggetto getTipoOggetto()
        {
            return (TipoOggetto)this.Invoke("getTipoOggetto", new object[0])[0];
        }

        public IAsyncResult BegingetTipoOggetto(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getTipoOggetto", new object[0], callback, asyncState);
        }

        public TipoOggetto EndgetTipoOggetto(IAsyncResult asyncResult)
        {
            return (TipoOggetto)this.EndInvoke(asyncResult)[0];
        }

        public void getTipoOggettoAsync()
        {
            this.getTipoOggettoAsync((object)null);
        }

        public void getTipoOggettoAsync(object userState)
        {
            if (this.getTipoOggettoOperationCompleted == null)
                this.getTipoOggettoOperationCompleted = new SendOrPostCallback(this.OngetTipoOggettoOperationCompleted);
            this.InvokeAsync("getTipoOggetto", new object[0], this.getTipoOggettoOperationCompleted, userState);
        }

        private void OngetTipoOggettoOperationCompleted(object arg)
        {
            if (this.getTipoOggettoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.getTipoOggettoCompleted((object)this, new getTipoOggettoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/getOggettoCustom", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public OggettoCustom getOggettoCustom()
        {
            return (OggettoCustom)this.Invoke("getOggettoCustom", new object[0])[0];
        }

        public IAsyncResult BegingetOggettoCustom(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getOggettoCustom", new object[0], callback, asyncState);
        }

        public OggettoCustom EndgetOggettoCustom(IAsyncResult asyncResult)
        {
            return (OggettoCustom)this.EndInvoke(asyncResult)[0];
        }

        public void getOggettoCustomAsync()
        {
            this.getOggettoCustomAsync((object)null);
        }

        public void getOggettoCustomAsync(object userState)
        {
            if (this.getOggettoCustomOperationCompleted == null)
                this.getOggettoCustomOperationCompleted = new SendOrPostCallback(this.OngetOggettoCustomOperationCompleted);
            this.InvokeAsync("getOggettoCustom", new object[0], this.getOggettoCustomOperationCompleted, userState);
        }

        private void OngetOggettoCustomOperationCompleted(object arg)
        {
            if (this.getOggettoCustomCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.getOggettoCustomCompleted((object)this, new getOggettoCustomCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/getTipiDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public object[] getTipiDocumento(string idAmministrazione)
        {
            return (object[])this.Invoke("getTipiDocumento", new object[1]
      {
        (object) idAmministrazione
      })[0];
        }

        public IAsyncResult BegingetTipiDocumento(string idAmministrazione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getTipiDocumento", new object[1]
      {
        (object) idAmministrazione
      }, callback, asyncState);
        }

        public object[] EndgetTipiDocumento(IAsyncResult asyncResult)
        {
            return (object[])this.EndInvoke(asyncResult)[0];
        }

        public void getTipiDocumentoAsync(string idAmministrazione)
        {
            this.getTipiDocumentoAsync(idAmministrazione, (object)null);
        }

        public void getTipiDocumentoAsync(string idAmministrazione, object userState)
        {
            if (this.getTipiDocumentoOperationCompleted == null)
                this.getTipiDocumentoOperationCompleted = new SendOrPostCallback(this.OngetTipiDocumentoOperationCompleted);
            this.InvokeAsync("getTipiDocumento", new object[1]
      {
        (object) idAmministrazione
      }, this.getTipiDocumentoOperationCompleted, userState);
        }

        private void OngetTipiDocumentoOperationCompleted(object arg)
        {
            if (this.getTipiDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.getTipiDocumentoCompleted((object)this, new getTipiDocumentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/getTemaplatesVuoto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Templates getTemaplatesVuoto()
        {
            return (Templates)this.Invoke("getTemaplatesVuoto", new object[0])[0];
        }

        public IAsyncResult BegingetTemaplatesVuoto(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getTemaplatesVuoto", new object[0], callback, asyncState);
        }

        public Templates EndgetTemaplatesVuoto(IAsyncResult asyncResult)
        {
            return (Templates)this.EndInvoke(asyncResult)[0];
        }

        public void getTemaplatesVuotoAsync()
        {
            this.getTemaplatesVuotoAsync((object)null);
        }

        public void getTemaplatesVuotoAsync(object userState)
        {
            if (this.getTemaplatesVuotoOperationCompleted == null)
                this.getTemaplatesVuotoOperationCompleted = new SendOrPostCallback(this.OngetTemaplatesVuotoOperationCompleted);
            this.InvokeAsync("getTemaplatesVuoto", new object[0], this.getTemaplatesVuotoOperationCompleted, userState);
        }

        private void OngetTemaplatesVuotoOperationCompleted(object arg)
        {
            if (this.getTemaplatesVuotoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.getTemaplatesVuotoCompleted((object)this, new getTemaplatesVuotoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/getTipiOggetto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public object[] getTipiOggetto()
        {
            return (object[])this.Invoke("getTipiOggetto", new object[0])[0];
        }

        public IAsyncResult BegingetTipiOggetto(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("getTipiOggetto", new object[0], callback, asyncState);
        }

        public object[] EndgetTipiOggetto(IAsyncResult asyncResult)
        {
            return (object[])this.EndInvoke(asyncResult)[0];
        }

        public void getTipiOggettoAsync()
        {
            this.getTipiOggettoAsync((object)null);
        }

        public void getTipiOggettoAsync(object userState)
        {
            if (this.getTipiOggettoOperationCompleted == null)
                this.getTipiOggettoOperationCompleted = new SendOrPostCallback(this.OngetTipiOggettoOperationCompleted);
            this.InvokeAsync("getTipiOggetto", new object[0], this.getTipiOggettoOperationCompleted, userState);
        }

        private void OngetTipiOggettoOperationCompleted(object arg)
        {
            if (this.getTipiOggettoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.getTipiOggettoCompleted((object)this, new getTipiOggettoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetTipologiaAttoProfDin", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TipologiaAtto[] GetTipologiaAttoProfDin(string idAmministrazione)
        {
            return (TipologiaAtto[])this.Invoke("GetTipologiaAttoProfDin", new object[1]
      {
        (object) idAmministrazione
      })[0];
        }

        public IAsyncResult BeginGetTipologiaAttoProfDin(string idAmministrazione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetTipologiaAttoProfDin", new object[1]
      {
        (object) idAmministrazione
      }, callback, asyncState);
        }

        public TipologiaAtto[] EndGetTipologiaAttoProfDin(IAsyncResult asyncResult)
        {
            return (TipologiaAtto[])this.EndInvoke(asyncResult)[0];
        }

        public void GetTipologiaAttoProfDinAsync(string idAmministrazione)
        {
            this.GetTipologiaAttoProfDinAsync(idAmministrazione, (object)null);
        }

        public void GetTipologiaAttoProfDinAsync(string idAmministrazione, object userState)
        {
            if (this.GetTipologiaAttoProfDinOperationCompleted == null)
                this.GetTipologiaAttoProfDinOperationCompleted = new SendOrPostCallback(this.OnGetTipologiaAttoProfDinOperationCompleted);
            this.InvokeAsync("GetTipologiaAttoProfDin", new object[1]
      {
        (object) idAmministrazione
      }, this.GetTipologiaAttoProfDinOperationCompleted, userState);
        }

        private void OnGetTipologiaAttoProfDinOperationCompleted(object arg)
        {
            if (this.GetTipologiaAttoProfDinCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetTipologiaAttoProfDinCompleted((object)this, new GetTipologiaAttoProfDinCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookGetRuoliUtenteInt", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet AddressbookGetRuoliUtenteInt(string codRubrica)
        {
            return (DataSet)this.Invoke("AddressbookGetRuoliUtenteInt", new object[1]
      {
        (object) codRubrica
      })[0];
        }

        public IAsyncResult BeginAddressbookGetRuoliUtenteInt(string codRubrica, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookGetRuoliUtenteInt", new object[1]
      {
        (object) codRubrica
      }, callback, asyncState);
        }

        public DataSet EndAddressbookGetRuoliUtenteInt(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookGetRuoliUtenteIntAsync(string codRubrica)
        {
            this.AddressbookGetRuoliUtenteIntAsync(codRubrica, (object)null);
        }

        public void AddressbookGetRuoliUtenteIntAsync(string codRubrica, object userState)
        {
            if (this.AddressbookGetRuoliUtenteIntOperationCompleted == null)
                this.AddressbookGetRuoliUtenteIntOperationCompleted = new SendOrPostCallback(this.OnAddressbookGetRuoliUtenteIntOperationCompleted);
            this.InvokeAsync("AddressbookGetRuoliUtenteInt", new object[1]
      {
        (object) codRubrica
      }, this.AddressbookGetRuoliUtenteIntOperationCompleted, userState);
        }

        private void OnAddressbookGetRuoliUtenteIntOperationCompleted(object arg)
        {
            if (this.AddressbookGetRuoliUtenteIntCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookGetRuoliUtenteIntCompleted((object)this, new AddressbookGetRuoliUtenteIntCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/rubricaGetRootItems", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public ElementoRubrica[] rubricaGetRootItems(AddressbookTipoUtente tipoIE, InfoUtente u)
        {
            return (ElementoRubrica[])this.Invoke("rubricaGetRootItems", new object[2]
      {
        (object) tipoIE,
        (object) u
      })[0];
        }

        public IAsyncResult BeginrubricaGetRootItems(AddressbookTipoUtente tipoIE, InfoUtente u, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("rubricaGetRootItems", new object[2]
      {
        (object) tipoIE,
        (object) u
      }, callback, asyncState);
        }

        public ElementoRubrica[] EndrubricaGetRootItems(IAsyncResult asyncResult)
        {
            return (ElementoRubrica[])this.EndInvoke(asyncResult)[0];
        }

        public void rubricaGetRootItemsAsync(AddressbookTipoUtente tipoIE, InfoUtente u)
        {
            this.rubricaGetRootItemsAsync(tipoIE, u, (object)null);
        }

        public void rubricaGetRootItemsAsync(AddressbookTipoUtente tipoIE, InfoUtente u, object userState)
        {
            if (this.rubricaGetRootItemsOperationCompleted == null)
                this.rubricaGetRootItemsOperationCompleted = new SendOrPostCallback(this.OnrubricaGetRootItemsOperationCompleted);
            this.InvokeAsync("rubricaGetRootItems", new object[2]
      {
        (object) tipoIE,
        (object) u
      }, this.rubricaGetRootItemsOperationCompleted, userState);
        }

        private void OnrubricaGetRootItemsOperationCompleted(object arg)
        {
            if (this.rubricaGetRootItemsCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.rubricaGetRootItemsCompleted((object)this, new rubricaGetRootItemsCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetRuoliInUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Ruolo[] amministrazioneGetRuoliInUO(UnitaOrganizzativa uo, InfoUtente infoUtente)
        {
            return (Ruolo[])this.Invoke("amministrazioneGetRuoliInUO", new object[2]
      {
        (object) uo,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetRuoliInUO(UnitaOrganizzativa uo, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetRuoliInUO", new object[2]
      {
        (object) uo,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Ruolo[] EndamministrazioneGetRuoliInUO(IAsyncResult asyncResult)
        {
            return (Ruolo[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetRuoliInUOAsync(UnitaOrganizzativa uo, InfoUtente infoUtente)
        {
            this.amministrazioneGetRuoliInUOAsync(uo, infoUtente, (object)null);
        }

        public void amministrazioneGetRuoliInUOAsync(UnitaOrganizzativa uo, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetRuoliInUOOperationCompleted == null)
                this.amministrazioneGetRuoliInUOOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetRuoliInUOOperationCompleted);
            this.InvokeAsync("amministrazioneGetRuoliInUO", new object[2]
      {
        (object) uo,
        (object) infoUtente
      }, this.amministrazioneGetRuoliInUOOperationCompleted, userState);
        }

        private void OnamministrazioneGetRuoliInUOOperationCompleted(object arg)
        {
            if (this.amministrazioneGetRuoliInUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetRuoliInUOCompleted((object)this, new amministrazioneGetRuoliInUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/rubricaGetElementiRubricaRange", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public ElementoRubrica[] rubricaGetElementiRubricaRange(string[] codici, AddressbookTipoUtente tipoIE, InfoUtente u)
        {
            return (ElementoRubrica[])this.Invoke("rubricaGetElementiRubricaRange", new object[3]
      {
        (object) codici,
        (object) tipoIE,
        (object) u
      })[0];
        }

        public IAsyncResult BeginrubricaGetElementiRubricaRange(string[] codici, AddressbookTipoUtente tipoIE, InfoUtente u, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("rubricaGetElementiRubricaRange", new object[3]
      {
        (object) codici,
        (object) tipoIE,
        (object) u
      }, callback, asyncState);
        }

        public ElementoRubrica[] EndrubricaGetElementiRubricaRange(IAsyncResult asyncResult)
        {
            return (ElementoRubrica[])this.EndInvoke(asyncResult)[0];
        }

        public void rubricaGetElementiRubricaRangeAsync(string[] codici, AddressbookTipoUtente tipoIE, InfoUtente u)
        {
            this.rubricaGetElementiRubricaRangeAsync(codici, tipoIE, u, (object)null);
        }

        public void rubricaGetElementiRubricaRangeAsync(string[] codici, AddressbookTipoUtente tipoIE, InfoUtente u, object userState)
        {
            if (this.rubricaGetElementiRubricaRangeOperationCompleted == null)
                this.rubricaGetElementiRubricaRangeOperationCompleted = new SendOrPostCallback(this.OnrubricaGetElementiRubricaRangeOperationCompleted);
            this.InvokeAsync("rubricaGetElementiRubricaRange", new object[3]
      {
        (object) codici,
        (object) tipoIE,
        (object) u
      }, this.rubricaGetElementiRubricaRangeOperationCompleted, userState);
        }

        private void OnrubricaGetElementiRubricaRangeOperationCompleted(object arg)
        {
            if (this.rubricaGetElementiRubricaRangeCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.rubricaGetElementiRubricaRangeCompleted((object)this, new rubricaGetElementiRubricaRangeCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookGetCorrispondenteByCodRubricaIE", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente AddressbookGetCorrispondenteByCodRubricaIE(string codice, AddressbookTipoUtente tipoIE, InfoUtente u)
        {
            return (Corrispondente)this.Invoke("AddressbookGetCorrispondenteByCodRubricaIE", new object[3]
      {
        (object) codice,
        (object) tipoIE,
        (object) u
      })[0];
        }

        public IAsyncResult BeginAddressbookGetCorrispondenteByCodRubricaIE(string codice, AddressbookTipoUtente tipoIE, InfoUtente u, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookGetCorrispondenteByCodRubricaIE", new object[3]
      {
        (object) codice,
        (object) tipoIE,
        (object) u
      }, callback, asyncState);
        }

        public Corrispondente EndAddressbookGetCorrispondenteByCodRubricaIE(IAsyncResult asyncResult)
        {
            return (Corrispondente)this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookGetCorrispondenteByCodRubricaIEAsync(string codice, AddressbookTipoUtente tipoIE, InfoUtente u)
        {
            this.AddressbookGetCorrispondenteByCodRubricaIEAsync(codice, tipoIE, u, (object)null);
        }

        public void AddressbookGetCorrispondenteByCodRubricaIEAsync(string codice, AddressbookTipoUtente tipoIE, InfoUtente u, object userState)
        {
            if (this.AddressbookGetCorrispondenteByCodRubricaIEOperationCompleted == null)
                this.AddressbookGetCorrispondenteByCodRubricaIEOperationCompleted = new SendOrPostCallback(this.OnAddressbookGetCorrispondenteByCodRubricaIEOperationCompleted);
            this.InvokeAsync("AddressbookGetCorrispondenteByCodRubricaIE", new object[3]
      {
        (object) codice,
        (object) tipoIE,
        (object) u
      }, this.AddressbookGetCorrispondenteByCodRubricaIEOperationCompleted, userState);
        }

        private void OnAddressbookGetCorrispondenteByCodRubricaIEOperationCompleted(object arg)
        {
            if (this.AddressbookGetCorrispondenteByCodRubricaIECompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookGetCorrispondenteByCodRubricaIECompleted((object)this, new AddressbookGetCorrispondenteByCodRubricaIECompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookGetCorrispondenteBySystemId", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente AddressbookGetCorrispondenteBySystemId(string system_id)
        {
            return (Corrispondente)this.Invoke("AddressbookGetCorrispondenteBySystemId", new object[1]
      {
        (object) system_id
      })[0];
        }

        public IAsyncResult BeginAddressbookGetCorrispondenteBySystemId(string system_id, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookGetCorrispondenteBySystemId", new object[1]
      {
        (object) system_id
      }, callback, asyncState);
        }

        public Corrispondente EndAddressbookGetCorrispondenteBySystemId(IAsyncResult asyncResult)
        {
            return (Corrispondente)this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookGetCorrispondenteBySystemIdAsync(string system_id)
        {
            this.AddressbookGetCorrispondenteBySystemIdAsync(system_id, (object)null);
        }

        public void AddressbookGetCorrispondenteBySystemIdAsync(string system_id, object userState)
        {
            if (this.AddressbookGetCorrispondenteBySystemIdOperationCompleted == null)
                this.AddressbookGetCorrispondenteBySystemIdOperationCompleted = new SendOrPostCallback(this.OnAddressbookGetCorrispondenteBySystemIdOperationCompleted);
            this.InvokeAsync("AddressbookGetCorrispondenteBySystemId", new object[1]
      {
        (object) system_id
      }, this.AddressbookGetCorrispondenteBySystemIdOperationCompleted, userState);
        }

        private void OnAddressbookGetCorrispondenteBySystemIdOperationCompleted(object arg)
        {
            if (this.AddressbookGetCorrispondenteBySystemIdCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookGetCorrispondenteBySystemIdCompleted((object)this, new AddressbookGetCorrispondenteBySystemIdCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookGetCorrispondenteByCodRubrica", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente AddressbookGetCorrispondenteByCodRubrica(string codice, InfoUtente u)
        {
            return (Corrispondente)this.Invoke("AddressbookGetCorrispondenteByCodRubrica", new object[2]
      {
        (object) codice,
        (object) u
      })[0];
        }

        public IAsyncResult BeginAddressbookGetCorrispondenteByCodRubrica(string codice, InfoUtente u, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookGetCorrispondenteByCodRubrica", new object[2]
      {
        (object) codice,
        (object) u
      }, callback, asyncState);
        }

        public Corrispondente EndAddressbookGetCorrispondenteByCodRubrica(IAsyncResult asyncResult)
        {
            return (Corrispondente)this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookGetCorrispondenteByCodRubricaAsync(string codice, InfoUtente u)
        {
            this.AddressbookGetCorrispondenteByCodRubricaAsync(codice, u, (object)null);
        }

        public void AddressbookGetCorrispondenteByCodRubricaAsync(string codice, InfoUtente u, object userState)
        {
            if (this.AddressbookGetCorrispondenteByCodRubricaOperationCompleted == null)
                this.AddressbookGetCorrispondenteByCodRubricaOperationCompleted = new SendOrPostCallback(this.OnAddressbookGetCorrispondenteByCodRubricaOperationCompleted);
            this.InvokeAsync("AddressbookGetCorrispondenteByCodRubrica", new object[2]
      {
        (object) codice,
        (object) u
      }, this.AddressbookGetCorrispondenteByCodRubricaOperationCompleted, userState);
        }

        private void OnAddressbookGetCorrispondenteByCodRubricaOperationCompleted(object arg)
        {
            if (this.AddressbookGetCorrispondenteByCodRubricaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookGetCorrispondenteByCodRubricaCompleted((object)this, new AddressbookGetCorrispondenteByCodRubricaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/rubricaCheckChildrenExistenceEx", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void rubricaCheckChildrenExistenceEx(ref ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti, InfoUtente u)
        {
            object[] objArray = this.Invoke("rubricaCheckChildrenExistenceEx", new object[5]
      {
        (object) ers,
        (object) checkUo,
        (object) checkRuoli,
        (object) checkUtenti,
        (object) u
      });
            ers = (ElementoRubrica[])objArray[0];
        }

        public IAsyncResult BeginrubricaCheckChildrenExistenceEx(ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti, InfoUtente u, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("rubricaCheckChildrenExistenceEx", new object[5]
      {
        (object) ers,
        (object) checkUo,
        (object) checkRuoli,
        (object) checkUtenti,
        (object) u
      }, callback, asyncState);
        }

        public void EndrubricaCheckChildrenExistenceEx(IAsyncResult asyncResult, out ElementoRubrica[] ers)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            ers = (ElementoRubrica[])objArray[0];
        }

        public void rubricaCheckChildrenExistenceExAsync(ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti, InfoUtente u)
        {
            this.rubricaCheckChildrenExistenceExAsync(ers, checkUo, checkRuoli, checkUtenti, u, (object)null);
        }

        public void rubricaCheckChildrenExistenceExAsync(ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti, InfoUtente u, object userState)
        {
            if (this.rubricaCheckChildrenExistenceExOperationCompleted == null)
                this.rubricaCheckChildrenExistenceExOperationCompleted = new SendOrPostCallback(this.OnrubricaCheckChildrenExistenceExOperationCompleted);
            this.InvokeAsync("rubricaCheckChildrenExistenceEx", new object[5]
      {
        (object) ers,
        (object) checkUo,
        (object) checkRuoli,
        (object) checkUtenti,
        (object) u
      }, this.rubricaCheckChildrenExistenceExOperationCompleted, userState);
        }

        private void OnrubricaCheckChildrenExistenceExOperationCompleted(object arg)
        {
            if (this.rubricaCheckChildrenExistenceExCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.rubricaCheckChildrenExistenceExCompleted((object)this, new rubricaCheckChildrenExistenceExCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/rubricaCheckChildrenExistence", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void rubricaCheckChildrenExistence(ref ElementoRubrica[] ers, InfoUtente u)
        {
            object[] objArray = this.Invoke("rubricaCheckChildrenExistence", new object[2]
      {
        (object) ers,
        (object) u
      });
            ers = (ElementoRubrica[])objArray[0];
        }

        public IAsyncResult BeginrubricaCheckChildrenExistence(ElementoRubrica[] ers, InfoUtente u, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("rubricaCheckChildrenExistence", new object[2]
      {
        (object) ers,
        (object) u
      }, callback, asyncState);
        }

        public void EndrubricaCheckChildrenExistence(IAsyncResult asyncResult, out ElementoRubrica[] ers)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            ers = (ElementoRubrica[])objArray[0];
        }

        public void rubricaCheckChildrenExistenceAsync(ElementoRubrica[] ers, InfoUtente u)
        {
            this.rubricaCheckChildrenExistenceAsync(ers, u, (object)null);
        }

        public void rubricaCheckChildrenExistenceAsync(ElementoRubrica[] ers, InfoUtente u, object userState)
        {
            if (this.rubricaCheckChildrenExistenceOperationCompleted == null)
                this.rubricaCheckChildrenExistenceOperationCompleted = new SendOrPostCallback(this.OnrubricaCheckChildrenExistenceOperationCompleted);
            this.InvokeAsync("rubricaCheckChildrenExistence", new object[2]
      {
        (object) ers,
        (object) u
      }, this.rubricaCheckChildrenExistenceOperationCompleted, userState);
        }

        private void OnrubricaCheckChildrenExistenceOperationCompleted(object arg)
        {
            if (this.rubricaCheckChildrenExistenceCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.rubricaCheckChildrenExistenceCompleted((object)this, new rubricaCheckChildrenExistenceCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/rubricaGetElementoRubrica", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public ElementoRubrica rubricaGetElementoRubrica(string cod, InfoUtente u)
        {
            return (ElementoRubrica)this.Invoke("rubricaGetElementoRubrica", new object[2]
      {
        (object) cod,
        (object) u
      })[0];
        }

        public IAsyncResult BeginrubricaGetElementoRubrica(string cod, InfoUtente u, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("rubricaGetElementoRubrica", new object[2]
      {
        (object) cod,
        (object) u
      }, callback, asyncState);
        }

        public ElementoRubrica EndrubricaGetElementoRubrica(IAsyncResult asyncResult)
        {
            return (ElementoRubrica)this.EndInvoke(asyncResult)[0];
        }

        public void rubricaGetElementoRubricaAsync(string cod, InfoUtente u)
        {
            this.rubricaGetElementoRubricaAsync(cod, u, (object)null);
        }

        public void rubricaGetElementoRubricaAsync(string cod, InfoUtente u, object userState)
        {
            if (this.rubricaGetElementoRubricaOperationCompleted == null)
                this.rubricaGetElementoRubricaOperationCompleted = new SendOrPostCallback(this.OnrubricaGetElementoRubricaOperationCompleted);
            this.InvokeAsync("rubricaGetElementoRubrica", new object[2]
      {
        (object) cod,
        (object) u
      }, this.rubricaGetElementoRubricaOperationCompleted, userState);
        }

        private void OnrubricaGetElementoRubricaOperationCompleted(object arg)
        {
            if (this.rubricaGetElementoRubricaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.rubricaGetElementoRubricaCompleted((object)this, new rubricaGetElementoRubricaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/rubricaGetGerarchiaElemento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public ElementoRubrica[] rubricaGetGerarchiaElemento(string codice, AddressbookTipoUtente tipoIE, InfoUtente u)
        {
            return (ElementoRubrica[])this.Invoke("rubricaGetGerarchiaElemento", new object[3]
      {
        (object) codice,
        (object) tipoIE,
        (object) u
      })[0];
        }

        public IAsyncResult BeginrubricaGetGerarchiaElemento(string codice, AddressbookTipoUtente tipoIE, InfoUtente u, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("rubricaGetGerarchiaElemento", new object[3]
      {
        (object) codice,
        (object) tipoIE,
        (object) u
      }, callback, asyncState);
        }

        public ElementoRubrica[] EndrubricaGetGerarchiaElemento(IAsyncResult asyncResult)
        {
            return (ElementoRubrica[])this.EndInvoke(asyncResult)[0];
        }

        public void rubricaGetGerarchiaElementoAsync(string codice, AddressbookTipoUtente tipoIE, InfoUtente u)
        {
            this.rubricaGetGerarchiaElementoAsync(codice, tipoIE, u, (object)null);
        }

        public void rubricaGetGerarchiaElementoAsync(string codice, AddressbookTipoUtente tipoIE, InfoUtente u, object userState)
        {
            if (this.rubricaGetGerarchiaElementoOperationCompleted == null)
                this.rubricaGetGerarchiaElementoOperationCompleted = new SendOrPostCallback(this.OnrubricaGetGerarchiaElementoOperationCompleted);
            this.InvokeAsync("rubricaGetGerarchiaElemento", new object[3]
      {
        (object) codice,
        (object) tipoIE,
        (object) u
      }, this.rubricaGetGerarchiaElementoOperationCompleted, userState);
        }

        private void OnrubricaGetGerarchiaElementoOperationCompleted(object arg)
        {
            if (this.rubricaGetGerarchiaElementoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.rubricaGetGerarchiaElementoCompleted((object)this, new rubricaGetGerarchiaElementoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/rubricaGetElementiRubrica", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public ElementoRubrica[] rubricaGetElementiRubrica(ParametriRicercaRubrica qc, InfoUtente u)
        {
            return (ElementoRubrica[])this.Invoke("rubricaGetElementiRubrica", new object[2]
      {
        (object) qc,
        (object) u
      })[0];
        }

        public IAsyncResult BeginrubricaGetElementiRubrica(ParametriRicercaRubrica qc, InfoUtente u, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("rubricaGetElementiRubrica", new object[2]
      {
        (object) qc,
        (object) u
      }, callback, asyncState);
        }

        public ElementoRubrica[] EndrubricaGetElementiRubrica(IAsyncResult asyncResult)
        {
            return (ElementoRubrica[])this.EndInvoke(asyncResult)[0];
        }

        public void rubricaGetElementiRubricaAsync(ParametriRicercaRubrica qc, InfoUtente u)
        {
            this.rubricaGetElementiRubricaAsync(qc, u, (object)null);
        }

        public void rubricaGetElementiRubricaAsync(ParametriRicercaRubrica qc, InfoUtente u, object userState)
        {
            if (this.rubricaGetElementiRubricaOperationCompleted == null)
                this.rubricaGetElementiRubricaOperationCompleted = new SendOrPostCallback(this.OnrubricaGetElementiRubricaOperationCompleted);
            this.InvokeAsync("rubricaGetElementiRubrica", new object[2]
      {
        (object) qc,
        (object) u
      }, this.rubricaGetElementiRubricaOperationCompleted, userState);
        }

        private void OnrubricaGetElementiRubricaOperationCompleted(object arg)
        {
            if (this.rubricaGetElementiRubricaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.rubricaGetElementiRubricaCompleted((object)this, new rubricaGetElementiRubricaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetUOSmistamento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public UOSmistamento[] GetUOSmistamento(string idRegistro, MittenteSmistamento mittente)
        {
            return (UOSmistamento[])this.Invoke("GetUOSmistamento", new object[2]
      {
        (object) idRegistro,
        (object) mittente
      })[0];
        }

        public IAsyncResult BeginGetUOSmistamento(string idRegistro, MittenteSmistamento mittente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetUOSmistamento", new object[2]
      {
        (object) idRegistro,
        (object) mittente
      }, callback, asyncState);
        }

        public UOSmistamento[] EndGetUOSmistamento(IAsyncResult asyncResult)
        {
            return (UOSmistamento[])this.EndInvoke(asyncResult)[0];
        }

        public void GetUOSmistamentoAsync(string idRegistro, MittenteSmistamento mittente)
        {
            this.GetUOSmistamentoAsync(idRegistro, mittente, (object)null);
        }

        public void GetUOSmistamentoAsync(string idRegistro, MittenteSmistamento mittente, object userState)
        {
            if (this.GetUOSmistamentoOperationCompleted == null)
                this.GetUOSmistamentoOperationCompleted = new SendOrPostCallback(this.OnGetUOSmistamentoOperationCompleted);
            this.InvokeAsync("GetUOSmistamento", new object[2]
      {
        (object) idRegistro,
        (object) mittente
      }, this.GetUOSmistamentoOperationCompleted, userState);
        }

        private void OnGetUOSmistamentoOperationCompleted(object arg)
        {
            if (this.GetUOSmistamentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetUOSmistamentoCompleted((object)this, new GetUOSmistamentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/RifiutaDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool RifiutaDocumento(string notaRifiuto, string IDTrasmUtente)
        {
            return (bool)this.Invoke("RifiutaDocumento", new object[2]
      {
        (object) notaRifiuto,
        (object) IDTrasmUtente
      })[0];
        }

        public IAsyncResult BeginRifiutaDocumento(string notaRifiuto, string IDTrasmUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RifiutaDocumento", new object[2]
      {
        (object) notaRifiuto,
        (object) IDTrasmUtente
      }, callback, asyncState);
        }

        public bool EndRifiutaDocumento(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void RifiutaDocumentoAsync(string notaRifiuto, string IDTrasmUtente)
        {
            this.RifiutaDocumentoAsync(notaRifiuto, IDTrasmUtente, (object)null);
        }

        public void RifiutaDocumentoAsync(string notaRifiuto, string IDTrasmUtente, object userState)
        {
            if (this.RifiutaDocumentoOperationCompleted == null)
                this.RifiutaDocumentoOperationCompleted = new SendOrPostCallback(this.OnRifiutaDocumentoOperationCompleted);
            this.InvokeAsync("RifiutaDocumento", new object[2]
      {
        (object) notaRifiuto,
        (object) IDTrasmUtente
      }, this.RifiutaDocumentoOperationCompleted, userState);
        }

        private void OnRifiutaDocumentoOperationCompleted(object arg)
        {
            if (this.RifiutaDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.RifiutaDocumentoCompleted((object)this, new RifiutaDocumentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ScartaDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool ScartaDocumento(string IDTrasmUtente, bool IsTrasmConWorkFlow)
        {
            return (bool)this.Invoke("ScartaDocumento", new object[2]
      {
        (object) IDTrasmUtente,
        (object) IsTrasmConWorkFlow
      })[0];
        }

        public IAsyncResult BeginScartaDocumento(string IDTrasmUtente, bool IsTrasmConWorkFlow, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ScartaDocumento", new object[2]
      {
        (object) IDTrasmUtente,
        (object) IsTrasmConWorkFlow
      }, callback, asyncState);
        }

        public bool EndScartaDocumento(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void ScartaDocumentoAsync(string IDTrasmUtente, bool IsTrasmConWorkFlow)
        {
            this.ScartaDocumentoAsync(IDTrasmUtente, IsTrasmConWorkFlow, (object)null);
        }

        public void ScartaDocumentoAsync(string IDTrasmUtente, bool IsTrasmConWorkFlow, object userState)
        {
            if (this.ScartaDocumentoOperationCompleted == null)
                this.ScartaDocumentoOperationCompleted = new SendOrPostCallback(this.OnScartaDocumentoOperationCompleted);
            this.InvokeAsync("ScartaDocumento", new object[2]
      {
        (object) IDTrasmUtente,
        (object) IsTrasmConWorkFlow
      }, this.ScartaDocumentoOperationCompleted, userState);
        }

        private void OnScartaDocumentoOperationCompleted(object arg)
        {
            if (this.ScartaDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ScartaDocumentoCompleted((object)this, new ScartaDocumentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/SmistaDocumentoNonTrasmesso", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoSmistamentoDocumento[] SmistaDocumentoNonTrasmesso(MittenteSmistamento mittente, InfoUtente infoUtente, DocumentoSmistamento documentoSmistamento, UOSmistamento[] uoDestinatarie)
        {
            return (EsitoSmistamentoDocumento[])this.Invoke("SmistaDocumentoNonTrasmesso", new object[4]
      {
        (object) mittente,
        (object) infoUtente,
        (object) documentoSmistamento,
        (object) uoDestinatarie
      })[0];
        }

        public IAsyncResult BeginSmistaDocumentoNonTrasmesso(MittenteSmistamento mittente, InfoUtente infoUtente, DocumentoSmistamento documentoSmistamento, UOSmistamento[] uoDestinatarie, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SmistaDocumentoNonTrasmesso", new object[4]
      {
        (object) mittente,
        (object) infoUtente,
        (object) documentoSmistamento,
        (object) uoDestinatarie
      }, callback, asyncState);
        }

        public EsitoSmistamentoDocumento[] EndSmistaDocumentoNonTrasmesso(IAsyncResult asyncResult)
        {
            return (EsitoSmistamentoDocumento[])this.EndInvoke(asyncResult)[0];
        }

        public void SmistaDocumentoNonTrasmessoAsync(MittenteSmistamento mittente, InfoUtente infoUtente, DocumentoSmistamento documentoSmistamento, UOSmistamento[] uoDestinatarie)
        {
            this.SmistaDocumentoNonTrasmessoAsync(mittente, infoUtente, documentoSmistamento, uoDestinatarie, (object)null);
        }

        public void SmistaDocumentoNonTrasmessoAsync(MittenteSmistamento mittente, InfoUtente infoUtente, DocumentoSmistamento documentoSmistamento, UOSmistamento[] uoDestinatarie, object userState)
        {
            if (this.SmistaDocumentoNonTrasmessoOperationCompleted == null)
                this.SmistaDocumentoNonTrasmessoOperationCompleted = new SendOrPostCallback(this.OnSmistaDocumentoNonTrasmessoOperationCompleted);
            this.InvokeAsync("SmistaDocumentoNonTrasmesso", new object[4]
      {
        (object) mittente,
        (object) infoUtente,
        (object) documentoSmistamento,
        (object) uoDestinatarie
      }, this.SmistaDocumentoNonTrasmessoOperationCompleted, userState);
        }

        private void OnSmistaDocumentoNonTrasmessoOperationCompleted(object arg)
        {
            if (this.SmistaDocumentoNonTrasmessoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.SmistaDocumentoNonTrasmessoCompleted((object)this, new SmistaDocumentoNonTrasmessoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/SmistaDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoSmistamentoDocumento[] SmistaDocumento(MittenteSmistamento mittente, InfoUtente infoUtente, DocumentoSmistamento documentoTrasmesso, DatiTrasmissioneDocumento datiTrasmissioneDocumento, UOSmistamento uoAppartenenza, UOSmistamento[] uoInferiori)
        {
            return (EsitoSmistamentoDocumento[])this.Invoke("SmistaDocumento", new object[6]
      {
        (object) mittente,
        (object) infoUtente,
        (object) documentoTrasmesso,
        (object) datiTrasmissioneDocumento,
        (object) uoAppartenenza,
        (object) uoInferiori
      })[0];
        }

        public IAsyncResult BeginSmistaDocumento(MittenteSmistamento mittente, InfoUtente infoUtente, DocumentoSmistamento documentoTrasmesso, DatiTrasmissioneDocumento datiTrasmissioneDocumento, UOSmistamento uoAppartenenza, UOSmistamento[] uoInferiori, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SmistaDocumento", new object[6]
      {
        (object) mittente,
        (object) infoUtente,
        (object) documentoTrasmesso,
        (object) datiTrasmissioneDocumento,
        (object) uoAppartenenza,
        (object) uoInferiori
      }, callback, asyncState);
        }

        public EsitoSmistamentoDocumento[] EndSmistaDocumento(IAsyncResult asyncResult)
        {
            return (EsitoSmistamentoDocumento[])this.EndInvoke(asyncResult)[0];
        }

        public void SmistaDocumentoAsync(MittenteSmistamento mittente, InfoUtente infoUtente, DocumentoSmistamento documentoTrasmesso, DatiTrasmissioneDocumento datiTrasmissioneDocumento, UOSmistamento uoAppartenenza, UOSmistamento[] uoInferiori)
        {
            this.SmistaDocumentoAsync(mittente, infoUtente, documentoTrasmesso, datiTrasmissioneDocumento, uoAppartenenza, uoInferiori, (object)null);
        }

        public void SmistaDocumentoAsync(MittenteSmistamento mittente, InfoUtente infoUtente, DocumentoSmistamento documentoTrasmesso, DatiTrasmissioneDocumento datiTrasmissioneDocumento, UOSmistamento uoAppartenenza, UOSmistamento[] uoInferiori, object userState)
        {
            if (this.SmistaDocumentoOperationCompleted == null)
                this.SmistaDocumentoOperationCompleted = new SendOrPostCallback(this.OnSmistaDocumentoOperationCompleted);
            this.InvokeAsync("SmistaDocumento", new object[6]
      {
        (object) mittente,
        (object) infoUtente,
        (object) documentoTrasmesso,
        (object) datiTrasmissioneDocumento,
        (object) uoAppartenenza,
        (object) uoInferiori
      }, this.SmistaDocumentoOperationCompleted, userState);
        }

        private void OnSmistaDocumentoOperationCompleted(object arg)
        {
            if (this.SmistaDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.SmistaDocumentoCompleted((object)this, new SmistaDocumentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetUOInferiori", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public UOSmistamento[] GetUOInferiori(string idUOAppartenenza, MittenteSmistamento mittente)
        {
            return (UOSmistamento[])this.Invoke("GetUOInferiori", new object[2]
      {
        (object) idUOAppartenenza,
        (object) mittente
      })[0];
        }

        public IAsyncResult BeginGetUOInferiori(string idUOAppartenenza, MittenteSmistamento mittente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetUOInferiori", new object[2]
      {
        (object) idUOAppartenenza,
        (object) mittente
      }, callback, asyncState);
        }

        public UOSmistamento[] EndGetUOInferiori(IAsyncResult asyncResult)
        {
            return (UOSmistamento[])this.EndInvoke(asyncResult)[0];
        }

        public void GetUOInferioriAsync(string idUOAppartenenza, MittenteSmistamento mittente)
        {
            this.GetUOInferioriAsync(idUOAppartenenza, mittente, (object)null);
        }

        public void GetUOInferioriAsync(string idUOAppartenenza, MittenteSmistamento mittente, object userState)
        {
            if (this.GetUOInferioriOperationCompleted == null)
                this.GetUOInferioriOperationCompleted = new SendOrPostCallback(this.OnGetUOInferioriOperationCompleted);
            this.InvokeAsync("GetUOInferiori", new object[2]
      {
        (object) idUOAppartenenza,
        (object) mittente
      }, this.GetUOInferioriOperationCompleted, userState);
        }

        private void OnGetUOInferioriOperationCompleted(object arg)
        {
            if (this.GetUOInferioriCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetUOInferioriCompleted((object)this, new GetUOInferioriCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetUOAppartenenza", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public UOSmistamento GetUOAppartenenza(string idUnitaOrganizzativa, MittenteSmistamento mittente)
        {
            return (UOSmistamento)this.Invoke("GetUOAppartenenza", new object[2]
      {
        (object) idUnitaOrganizzativa,
        (object) mittente
      })[0];
        }

        public IAsyncResult BeginGetUOAppartenenza(string idUnitaOrganizzativa, MittenteSmistamento mittente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetUOAppartenenza", new object[2]
      {
        (object) idUnitaOrganizzativa,
        (object) mittente
      }, callback, asyncState);
        }

        public UOSmistamento EndGetUOAppartenenza(IAsyncResult asyncResult)
        {
            return (UOSmistamento)this.EndInvoke(asyncResult)[0];
        }

        public void GetUOAppartenenzaAsync(string idUnitaOrganizzativa, MittenteSmistamento mittente)
        {
            this.GetUOAppartenenzaAsync(idUnitaOrganizzativa, mittente, (object)null);
        }

        public void GetUOAppartenenzaAsync(string idUnitaOrganizzativa, MittenteSmistamento mittente, object userState)
        {
            if (this.GetUOAppartenenzaOperationCompleted == null)
                this.GetUOAppartenenzaOperationCompleted = new SendOrPostCallback(this.OnGetUOAppartenenzaOperationCompleted);
            this.InvokeAsync("GetUOAppartenenza", new object[2]
      {
        (object) idUnitaOrganizzativa,
        (object) mittente
      }, this.GetUOAppartenenzaOperationCompleted, userState);
        }

        private void OnGetUOAppartenenzaOperationCompleted(object arg)
        {
            if (this.GetUOAppartenenzaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetUOAppartenenzaCompleted((object)this, new GetUOAppartenenzaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetDocumentoSmistamento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DocumentoSmistamento GetDocumentoSmistamento(string idDocumento, InfoUtente infoUtente)
        {
            return (DocumentoSmistamento)this.Invoke("GetDocumentoSmistamento", new object[2]
      {
        (object) idDocumento,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginGetDocumentoSmistamento(string idDocumento, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetDocumentoSmistamento", new object[2]
      {
        (object) idDocumento,
        (object) infoUtente
      }, callback, asyncState);
        }

        public DocumentoSmistamento EndGetDocumentoSmistamento(IAsyncResult asyncResult)
        {
            return (DocumentoSmistamento)this.EndInvoke(asyncResult)[0];
        }

        public void GetDocumentoSmistamentoAsync(string idDocumento, InfoUtente infoUtente)
        {
            this.GetDocumentoSmistamentoAsync(idDocumento, infoUtente, (object)null);
        }

        public void GetDocumentoSmistamentoAsync(string idDocumento, InfoUtente infoUtente, object userState)
        {
            if (this.GetDocumentoSmistamentoOperationCompleted == null)
                this.GetDocumentoSmistamentoOperationCompleted = new SendOrPostCallback(this.OnGetDocumentoSmistamentoOperationCompleted);
            this.InvokeAsync("GetDocumentoSmistamento", new object[2]
      {
        (object) idDocumento,
        (object) infoUtente
      }, this.GetDocumentoSmistamentoOperationCompleted, userState);
        }

        private void OnGetDocumentoSmistamentoOperationCompleted(object arg)
        {
            if (this.GetDocumentoSmistamentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetDocumentoSmistamentoCompleted((object)this, new GetDocumentoSmistamentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetListDocumentiTrasmessi", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DatiTrasmissioneDocumento[] GetListDocumentiTrasmessi(MittenteSmistamento mittente)
        {
            return (DatiTrasmissioneDocumento[])this.Invoke("GetListDocumentiTrasmessi", new object[1]
      {
        (object) mittente
      })[0];
        }

        public IAsyncResult BeginGetListDocumentiTrasmessi(MittenteSmistamento mittente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetListDocumentiTrasmessi", new object[1]
      {
        (object) mittente
      }, callback, asyncState);
        }

        public DatiTrasmissioneDocumento[] EndGetListDocumentiTrasmessi(IAsyncResult asyncResult)
        {
            return (DatiTrasmissioneDocumento[])this.EndInvoke(asyncResult)[0];
        }

        public void GetListDocumentiTrasmessiAsync(MittenteSmistamento mittente)
        {
            this.GetListDocumentiTrasmessiAsync(mittente, (object)null);
        }

        public void GetListDocumentiTrasmessiAsync(MittenteSmistamento mittente, object userState)
        {
            if (this.GetListDocumentiTrasmessiOperationCompleted == null)
                this.GetListDocumentiTrasmessiOperationCompleted = new SendOrPostCallback(this.OnGetListDocumentiTrasmessiOperationCompleted);
            this.InvokeAsync("GetListDocumentiTrasmessi", new object[1]
      {
        (object) mittente
      }, this.GetListDocumentiTrasmessiOperationCompleted, userState);
        }

        private void OnGetListDocumentiTrasmessiOperationCompleted(object arg)
        {
            if (this.GetListDocumentiTrasmessiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetListDocumentiTrasmessiCompleted((object)this, new GetListDocumentiTrasmessiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDSReportDocXUo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet DO_GetDSReportDocXUo(int idAmm, int idReg, int anno)
        {
            return (DataSet)this.Invoke("DO_GetDSReportDocXUo", new object[3]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno
      })[0];
        }

        public IAsyncResult BeginDO_GetDSReportDocXUo(int idAmm, int idReg, int anno, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDSReportDocXUo", new object[3]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno
      }, callback, asyncState);
        }

        public DataSet EndDO_GetDSReportDocXUo(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDSReportDocXUoAsync(int idAmm, int idReg, int anno)
        {
            this.DO_GetDSReportDocXUoAsync(idAmm, idReg, anno, (object)null);
        }

        public void DO_GetDSReportDocXUoAsync(int idAmm, int idReg, int anno, object userState)
        {
            if (this.DO_GetDSReportDocXUoOperationCompleted == null)
                this.DO_GetDSReportDocXUoOperationCompleted = new SendOrPostCallback(this.OnDO_GetDSReportDocXUoOperationCompleted);
            this.InvokeAsync("DO_GetDSReportDocXUo", new object[3]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno
      }, this.DO_GetDSReportDocXUoOperationCompleted, userState);
        }

        private void OnDO_GetDSReportDocXUoOperationCompleted(object arg)
        {
            if (this.DO_GetDSReportDocXUoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDSReportDocXUoCompleted((object)this, new DO_GetDSReportDocXUoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDSReportDocXSede", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet DO_GetDSReportDocXSede(int idAmm, int idReg, int anno)
        {
            return (DataSet)this.Invoke("DO_GetDSReportDocXSede", new object[3]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno
      })[0];
        }

        public IAsyncResult BeginDO_GetDSReportDocXSede(int idAmm, int idReg, int anno, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDSReportDocXSede", new object[3]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno
      }, callback, asyncState);
        }

        public DataSet EndDO_GetDSReportDocXSede(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDSReportDocXSedeAsync(int idAmm, int idReg, int anno)
        {
            this.DO_GetDSReportDocXSedeAsync(idAmm, idReg, anno, (object)null);
        }

        public void DO_GetDSReportDocXSedeAsync(int idAmm, int idReg, int anno, object userState)
        {
            if (this.DO_GetDSReportDocXSedeOperationCompleted == null)
                this.DO_GetDSReportDocXSedeOperationCompleted = new SendOrPostCallback(this.OnDO_GetDSReportDocXSedeOperationCompleted);
            this.InvokeAsync("DO_GetDSReportDocXSede", new object[3]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno
      }, this.DO_GetDSReportDocXSedeOperationCompleted, userState);
        }

        private void OnDO_GetDSReportDocXSedeOperationCompleted(object arg)
        {
            if (this.DO_GetDSReportDocXSedeCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDSReportDocXSedeCompleted((object)this, new DO_GetDSReportDocXSedeCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDSTempiMediLavorazioneCompact", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet DO_GetDSTempiMediLavorazioneCompact(int idAmm, int idReg, int anno, int mese)
        {
            return (DataSet)this.Invoke("DO_GetDSTempiMediLavorazioneCompact", new object[4]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese
      })[0];
        }

        public IAsyncResult BeginDO_GetDSTempiMediLavorazioneCompact(int idAmm, int idReg, int anno, int mese, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDSTempiMediLavorazioneCompact", new object[4]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese
      }, callback, asyncState);
        }

        public DataSet EndDO_GetDSTempiMediLavorazioneCompact(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDSTempiMediLavorazioneCompactAsync(int idAmm, int idReg, int anno, int mese)
        {
            this.DO_GetDSTempiMediLavorazioneCompactAsync(idAmm, idReg, anno, mese, (object)null);
        }

        public void DO_GetDSTempiMediLavorazioneCompactAsync(int idAmm, int idReg, int anno, int mese, object userState)
        {
            if (this.DO_GetDSTempiMediLavorazioneCompactOperationCompleted == null)
                this.DO_GetDSTempiMediLavorazioneCompactOperationCompleted = new SendOrPostCallback(this.OnDO_GetDSTempiMediLavorazioneCompactOperationCompleted);
            this.InvokeAsync("DO_GetDSTempiMediLavorazioneCompact", new object[4]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese
      }, this.DO_GetDSTempiMediLavorazioneCompactOperationCompleted, userState);
        }

        private void OnDO_GetDSTempiMediLavorazioneCompactOperationCompleted(object arg)
        {
            if (this.DO_GetDSTempiMediLavorazioneCompactCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDSTempiMediLavorazioneCompactCompleted((object)this, new DO_GetDSTempiMediLavorazioneCompactCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDSTempiMediLavorazione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet DO_GetDSTempiMediLavorazione(int idAmm, int idReg, int anno, int mese)
        {
            return (DataSet)this.Invoke("DO_GetDSTempiMediLavorazione", new object[4]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese
      })[0];
        }

        public IAsyncResult BeginDO_GetDSTempiMediLavorazione(int idAmm, int idReg, int anno, int mese, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDSTempiMediLavorazione", new object[4]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese
      }, callback, asyncState);
        }

        public DataSet EndDO_GetDSTempiMediLavorazione(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDSTempiMediLavorazioneAsync(int idAmm, int idReg, int anno, int mese)
        {
            this.DO_GetDSTempiMediLavorazioneAsync(idAmm, idReg, anno, mese, (object)null);
        }

        public void DO_GetDSTempiMediLavorazioneAsync(int idAmm, int idReg, int anno, int mese, object userState)
        {
            if (this.DO_GetDSTempiMediLavorazioneOperationCompleted == null)
                this.DO_GetDSTempiMediLavorazioneOperationCompleted = new SendOrPostCallback(this.OnDO_GetDSTempiMediLavorazioneOperationCompleted);
            this.InvokeAsync("DO_GetDSTempiMediLavorazione", new object[4]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese
      }, this.DO_GetDSTempiMediLavorazioneOperationCompleted, userState);
        }

        private void OnDO_GetDSTempiMediLavorazioneOperationCompleted(object arg)
        {
            if (this.DO_GetDSTempiMediLavorazioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDSTempiMediLavorazioneCompleted((object)this, new DO_GetDSTempiMediLavorazioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDSReportAnnualeByFasc", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet DO_GetDSReportAnnualeByFasc(int idAmm, int idReg, int anno, int mese, bool simpleSP)
        {
            return (DataSet)this.Invoke("DO_GetDSReportAnnualeByFasc", new object[5]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese,
        (object) simpleSP
      })[0];
        }

        public IAsyncResult BeginDO_GetDSReportAnnualeByFasc(int idAmm, int idReg, int anno, int mese, bool simpleSP, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDSReportAnnualeByFasc", new object[5]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese,
        (object) simpleSP
      }, callback, asyncState);
        }

        public DataSet EndDO_GetDSReportAnnualeByFasc(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDSReportAnnualeByFascAsync(int idAmm, int idReg, int anno, int mese, bool simpleSP)
        {
            this.DO_GetDSReportAnnualeByFascAsync(idAmm, idReg, anno, mese, simpleSP, (object)null);
        }

        public void DO_GetDSReportAnnualeByFascAsync(int idAmm, int idReg, int anno, int mese, bool simpleSP, object userState)
        {
            if (this.DO_GetDSReportAnnualeByFascOperationCompleted == null)
                this.DO_GetDSReportAnnualeByFascOperationCompleted = new SendOrPostCallback(this.OnDO_GetDSReportAnnualeByFascOperationCompleted);
            this.InvokeAsync("DO_GetDSReportAnnualeByFasc", new object[5]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese,
        (object) simpleSP
      }, this.DO_GetDSReportAnnualeByFascOperationCompleted, userState);
        }

        private void OnDO_GetDSReportAnnualeByFascOperationCompleted(object arg)
        {
            if (this.DO_GetDSReportAnnualeByFascCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDSReportAnnualeByFascCompleted((object)this, new DO_GetDSReportAnnualeByFascCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDSReportFascicoliPerVTCompact", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet DO_GetDSReportFascicoliPerVTCompact(int idAmm, int idReg, int anno, int mese)
        {
            return (DataSet)this.Invoke("DO_GetDSReportFascicoliPerVTCompact", new object[4]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese
      })[0];
        }

        public IAsyncResult BeginDO_GetDSReportFascicoliPerVTCompact(int idAmm, int idReg, int anno, int mese, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDSReportFascicoliPerVTCompact", new object[4]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese
      }, callback, asyncState);
        }

        public DataSet EndDO_GetDSReportFascicoliPerVTCompact(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDSReportFascicoliPerVTCompactAsync(int idAmm, int idReg, int anno, int mese)
        {
            this.DO_GetDSReportFascicoliPerVTCompactAsync(idAmm, idReg, anno, mese, (object)null);
        }

        public void DO_GetDSReportFascicoliPerVTCompactAsync(int idAmm, int idReg, int anno, int mese, object userState)
        {
            if (this.DO_GetDSReportFascicoliPerVTCompactOperationCompleted == null)
                this.DO_GetDSReportFascicoliPerVTCompactOperationCompleted = new SendOrPostCallback(this.OnDO_GetDSReportFascicoliPerVTCompactOperationCompleted);
            this.InvokeAsync("DO_GetDSReportFascicoliPerVTCompact", new object[4]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese
      }, this.DO_GetDSReportFascicoliPerVTCompactOperationCompleted, userState);
        }

        private void OnDO_GetDSReportFascicoliPerVTCompactOperationCompleted(object arg)
        {
            if (this.DO_GetDSReportFascicoliPerVTCompactCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDSReportFascicoliPerVTCompactCompleted((object)this, new DO_GetDSReportFascicoliPerVTCompactCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDSReportFascicoliPerVT", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet DO_GetDSReportFascicoliPerVT(int idAmm, int idReg, int anno, int mese)
        {
            return (DataSet)this.Invoke("DO_GetDSReportFascicoliPerVT", new object[4]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese
      })[0];
        }

        public IAsyncResult BeginDO_GetDSReportFascicoliPerVT(int idAmm, int idReg, int anno, int mese, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDSReportFascicoliPerVT", new object[4]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese
      }, callback, asyncState);
        }

        public DataSet EndDO_GetDSReportFascicoliPerVT(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDSReportFascicoliPerVTAsync(int idAmm, int idReg, int anno, int mese)
        {
            this.DO_GetDSReportFascicoliPerVTAsync(idAmm, idReg, anno, mese, (object)null);
        }

        public void DO_GetDSReportFascicoliPerVTAsync(int idAmm, int idReg, int anno, int mese, object userState)
        {
            if (this.DO_GetDSReportFascicoliPerVTOperationCompleted == null)
                this.DO_GetDSReportFascicoliPerVTOperationCompleted = new SendOrPostCallback(this.OnDO_GetDSReportFascicoliPerVTOperationCompleted);
            this.InvokeAsync("DO_GetDSReportFascicoliPerVT", new object[4]
      {
        (object) idAmm,
        (object) idReg,
        (object) anno,
        (object) mese
      }, this.DO_GetDSReportFascicoliPerVTOperationCompleted, userState);
        }

        private void OnDO_GetDSReportFascicoliPerVTOperationCompleted(object arg)
        {
            if (this.DO_GetDSReportFascicoliPerVTCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDSReportFascicoliPerVTCompleted((object)this, new DO_GetDSReportFascicoliPerVTCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDSReportDocTrasmToAOO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet DO_GetDSReportDocTrasmToAOO(int idReg, int anno)
        {
            return (DataSet)this.Invoke("DO_GetDSReportDocTrasmToAOO", new object[2]
      {
        (object) idReg,
        (object) anno
      })[0];
        }

        public IAsyncResult BeginDO_GetDSReportDocTrasmToAOO(int idReg, int anno, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDSReportDocTrasmToAOO", new object[2]
      {
        (object) idReg,
        (object) anno
      }, callback, asyncState);
        }

        public DataSet EndDO_GetDSReportDocTrasmToAOO(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDSReportDocTrasmToAOOAsync(int idReg, int anno)
        {
            this.DO_GetDSReportDocTrasmToAOOAsync(idReg, anno, (object)null);
        }

        public void DO_GetDSReportDocTrasmToAOOAsync(int idReg, int anno, object userState)
        {
            if (this.DO_GetDSReportDocTrasmToAOOOperationCompleted == null)
                this.DO_GetDSReportDocTrasmToAOOOperationCompleted = new SendOrPostCallback(this.OnDO_GetDSReportDocTrasmToAOOOperationCompleted);
            this.InvokeAsync("DO_GetDSReportDocTrasmToAOO", new object[2]
      {
        (object) idReg,
        (object) anno
      }, this.DO_GetDSReportDocTrasmToAOOOperationCompleted, userState);
        }

        private void OnDO_GetDSReportDocTrasmToAOOOperationCompleted(object arg)
        {
            if (this.DO_GetDSReportDocTrasmToAOOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDSReportDocTrasmToAOOCompleted((object)this, new DO_GetDSReportDocTrasmToAOOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDSReportDocClassCompact", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet DO_GetDSReportDocClassCompact(int idReg, int anno, int idamm, string sede)
        {
            return (DataSet)this.Invoke("DO_GetDSReportDocClassCompact", new object[4]
      {
        (object) idReg,
        (object) anno,
        (object) idamm,
        (object) sede
      })[0];
        }

        public IAsyncResult BeginDO_GetDSReportDocClassCompact(int idReg, int anno, int idamm, string sede, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDSReportDocClassCompact", new object[4]
      {
        (object) idReg,
        (object) anno,
        (object) idamm,
        (object) sede
      }, callback, asyncState);
        }

        public DataSet EndDO_GetDSReportDocClassCompact(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDSReportDocClassCompactAsync(int idReg, int anno, int idamm, string sede)
        {
            this.DO_GetDSReportDocClassCompactAsync(idReg, anno, idamm, sede, (object)null);
        }

        public void DO_GetDSReportDocClassCompactAsync(int idReg, int anno, int idamm, string sede, object userState)
        {
            if (this.DO_GetDSReportDocClassCompactOperationCompleted == null)
                this.DO_GetDSReportDocClassCompactOperationCompleted = new SendOrPostCallback(this.OnDO_GetDSReportDocClassCompactOperationCompleted);
            this.InvokeAsync("DO_GetDSReportDocClassCompact", new object[4]
      {
        (object) idReg,
        (object) anno,
        (object) idamm,
        (object) sede
      }, this.DO_GetDSReportDocClassCompactOperationCompleted, userState);
        }

        private void OnDO_GetDSReportDocClassCompactOperationCompleted(object arg)
        {
            if (this.DO_GetDSReportDocClassCompactCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDSReportDocClassCompactCompleted((object)this, new DO_GetDSReportDocClassCompactCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDSReportDocClass", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet DO_GetDSReportDocClass(int idReg, int anno, int idamm, string sede)
        {
            return (DataSet)this.Invoke("DO_GetDSReportDocClass", new object[4]
      {
        (object) idReg,
        (object) anno,
        (object) idamm,
        (object) sede
      })[0];
        }

        public IAsyncResult BeginDO_GetDSReportDocClass(int idReg, int anno, int idamm, string sede, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDSReportDocClass", new object[4]
      {
        (object) idReg,
        (object) anno,
        (object) idamm,
        (object) sede
      }, callback, asyncState);
        }

        public DataSet EndDO_GetDSReportDocClass(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDSReportDocClassAsync(int idReg, int anno, int idamm, string sede)
        {
            this.DO_GetDSReportDocClassAsync(idReg, anno, idamm, sede, (object)null);
        }

        public void DO_GetDSReportDocClassAsync(int idReg, int anno, int idamm, string sede, object userState)
        {
            if (this.DO_GetDSReportDocClassOperationCompleted == null)
                this.DO_GetDSReportDocClassOperationCompleted = new SendOrPostCallback(this.OnDO_GetDSReportDocClassOperationCompleted);
            this.InvokeAsync("DO_GetDSReportDocClass", new object[4]
      {
        (object) idReg,
        (object) anno,
        (object) idamm,
        (object) sede
      }, this.DO_GetDSReportDocClassOperationCompleted, userState);
        }

        private void OnDO_GetDSReportDocClassOperationCompleted(object arg)
        {
            if (this.DO_GetDSReportDocClassCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDSReportDocClassCompleted((object)this, new DO_GetDSReportDocClassCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDSReportAnnualeByReg", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet DO_GetDSReportAnnualeByReg(int id_amm, int idReg, int anno, int mese, string sede, bool simpleSP)
        {
            return (DataSet)this.Invoke("DO_GetDSReportAnnualeByReg", new object[6]
      {
        (object) id_amm,
        (object) idReg,
        (object) anno,
        (object) mese,
        (object) sede,
        (object) simpleSP
      })[0];
        }

        public IAsyncResult BeginDO_GetDSReportAnnualeByReg(int id_amm, int idReg, int anno, int mese, string sede, bool simpleSP, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDSReportAnnualeByReg", new object[6]
      {
        (object) id_amm,
        (object) idReg,
        (object) anno,
        (object) mese,
        (object) sede,
        (object) simpleSP
      }, callback, asyncState);
        }

        public DataSet EndDO_GetDSReportAnnualeByReg(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDSReportAnnualeByRegAsync(int id_amm, int idReg, int anno, int mese, string sede, bool simpleSP)
        {
            this.DO_GetDSReportAnnualeByRegAsync(id_amm, idReg, anno, mese, sede, simpleSP, (object)null);
        }

        public void DO_GetDSReportAnnualeByRegAsync(int id_amm, int idReg, int anno, int mese, string sede, bool simpleSP, object userState)
        {
            if (this.DO_GetDSReportAnnualeByRegOperationCompleted == null)
                this.DO_GetDSReportAnnualeByRegOperationCompleted = new SendOrPostCallback(this.OnDO_GetDSReportAnnualeByRegOperationCompleted);
            this.InvokeAsync("DO_GetDSReportAnnualeByReg", new object[6]
      {
        (object) id_amm,
        (object) idReg,
        (object) anno,
        (object) mese,
        (object) sede,
        (object) simpleSP
      }, this.DO_GetDSReportAnnualeByRegOperationCompleted, userState);
        }

        private void OnDO_GetDSReportAnnualeByRegOperationCompleted(object arg)
        {
            if (this.DO_GetDSReportAnnualeByRegCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDSReportAnnualeByRegCompleted((object)this, new DO_GetDSReportAnnualeByRegCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetEmptySimpleLog", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SimpleLog DO_GetEmptySimpleLog()
        {
            return (SimpleLog)this.Invoke("DO_GetEmptySimpleLog", new object[0])[0];
        }

        public IAsyncResult BeginDO_GetEmptySimpleLog(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetEmptySimpleLog", new object[0], callback, asyncState);
        }

        public SimpleLog EndDO_GetEmptySimpleLog(IAsyncResult asyncResult)
        {
            return (SimpleLog)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetEmptySimpleLogAsync()
        {
            this.DO_GetEmptySimpleLogAsync((object)null);
        }

        public void DO_GetEmptySimpleLogAsync(object userState)
        {
            if (this.DO_GetEmptySimpleLogOperationCompleted == null)
                this.DO_GetEmptySimpleLogOperationCompleted = new SendOrPostCallback(this.OnDO_GetEmptySimpleLogOperationCompleted);
            this.InvokeAsync("DO_GetEmptySimpleLog", new object[0], this.DO_GetEmptySimpleLogOperationCompleted, userState);
        }

        private void OnDO_GetEmptySimpleLogOperationCompleted(object arg)
        {
            if (this.DO_GetEmptySimpleLogCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetEmptySimpleLogCompleted((object)this, new DO_GetEmptySimpleLogCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetEmptyReport", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Report DO_GetEmptyReport()
        {
            return (Report)this.Invoke("DO_GetEmptyReport", new object[0])[0];
        }

        public IAsyncResult BeginDO_GetEmptyReport(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetEmptyReport", new object[0], callback, asyncState);
        }

        public Report EndDO_GetEmptyReport(IAsyncResult asyncResult)
        {
            return (Report)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetEmptyReportAsync()
        {
            this.DO_GetEmptyReportAsync((object)null);
        }

        public void DO_GetEmptyReportAsync(object userState)
        {
            if (this.DO_GetEmptyReportOperationCompleted == null)
                this.DO_GetEmptyReportOperationCompleted = new SendOrPostCallback(this.OnDO_GetEmptyReportOperationCompleted);
            this.InvokeAsync("DO_GetEmptyReport", new object[0], this.DO_GetEmptyReportOperationCompleted, userState);
        }

        private void OnDO_GetEmptyReportOperationCompleted(object arg)
        {
            if (this.DO_GetEmptyReportCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetEmptyReportCompleted((object)this, new DO_GetEmptyReportCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetEmptyRegitro", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public PR_Registro DO_GetEmptyRegitro()
        {
            return (PR_Registro)this.Invoke("DO_GetEmptyRegitro", new object[0])[0];
        }

        public IAsyncResult BeginDO_GetEmptyRegitro(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetEmptyRegitro", new object[0], callback, asyncState);
        }

        public PR_Registro EndDO_GetEmptyRegitro(IAsyncResult asyncResult)
        {
            return (PR_Registro)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetEmptyRegitroAsync()
        {
            this.DO_GetEmptyRegitroAsync((object)null);
        }

        public void DO_GetEmptyRegitroAsync(object userState)
        {
            if (this.DO_GetEmptyRegitroOperationCompleted == null)
                this.DO_GetEmptyRegitroOperationCompleted = new SendOrPostCallback(this.OnDO_GetEmptyRegitroOperationCompleted);
            this.InvokeAsync("DO_GetEmptyRegitro", new object[0], this.DO_GetEmptyRegitroOperationCompleted, userState);
        }

        private void OnDO_GetEmptyRegitroOperationCompleted(object arg)
        {
            if (this.DO_GetEmptyRegitroCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetEmptyRegitroCompleted((object)this, new DO_GetEmptyRegitroCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetEmptyParametro", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Parametro DO_GetEmptyParametro()
        {
            return (Parametro)this.Invoke("DO_GetEmptyParametro", new object[0])[0];
        }

        public IAsyncResult BeginDO_GetEmptyParametro(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetEmptyParametro", new object[0], callback, asyncState);
        }

        public Parametro EndDO_GetEmptyParametro(IAsyncResult asyncResult)
        {
            return (Parametro)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetEmptyParametroAsync()
        {
            this.DO_GetEmptyParametroAsync((object)null);
        }

        public void DO_GetEmptyParametroAsync(object userState)
        {
            if (this.DO_GetEmptyParametroOperationCompleted == null)
                this.DO_GetEmptyParametroOperationCompleted = new SendOrPostCallback(this.OnDO_GetEmptyParametroOperationCompleted);
            this.InvokeAsync("DO_GetEmptyParametro", new object[0], this.DO_GetEmptyParametroOperationCompleted, userState);
        }

        private void OnDO_GetEmptyParametroOperationCompleted(object arg)
        {
            if (this.DO_GetEmptyParametroCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetEmptyParametroCompleted((object)this, new DO_GetEmptyParametroCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_NewAmministrazione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public PR_Amministrazione DO_NewAmministrazione(string system_id, string codice, string descrizione, string libreria)
        {
            return (PR_Amministrazione)this.Invoke("DO_NewAmministrazione", new object[4]
      {
        (object) system_id,
        (object) codice,
        (object) descrizione,
        (object) libreria
      })[0];
        }

        public IAsyncResult BeginDO_NewAmministrazione(string system_id, string codice, string descrizione, string libreria, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_NewAmministrazione", new object[4]
      {
        (object) system_id,
        (object) codice,
        (object) descrizione,
        (object) libreria
      }, callback, asyncState);
        }

        public PR_Amministrazione EndDO_NewAmministrazione(IAsyncResult asyncResult)
        {
            return (PR_Amministrazione)this.EndInvoke(asyncResult)[0];
        }

        public void DO_NewAmministrazioneAsync(string system_id, string codice, string descrizione, string libreria)
        {
            this.DO_NewAmministrazioneAsync(system_id, codice, descrizione, libreria, (object)null);
        }

        public void DO_NewAmministrazioneAsync(string system_id, string codice, string descrizione, string libreria, object userState)
        {
            if (this.DO_NewAmministrazioneOperationCompleted == null)
                this.DO_NewAmministrazioneOperationCompleted = new SendOrPostCallback(this.OnDO_NewAmministrazioneOperationCompleted);
            this.InvokeAsync("DO_NewAmministrazione", new object[4]
      {
        (object) system_id,
        (object) codice,
        (object) descrizione,
        (object) libreria
      }, this.DO_NewAmministrazioneOperationCompleted, userState);
        }

        private void OnDO_NewAmministrazioneOperationCompleted(object arg)
        {
            if (this.DO_NewAmministrazioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_NewAmministrazioneCompleted((object)this, new DO_NewAmministrazioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_NewRegistro", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public PR_Registro DO_NewRegistro(string system_id, string codice, string descrizione)
        {
            return (PR_Registro)this.Invoke("DO_NewRegistro", new object[3]
      {
        (object) system_id,
        (object) codice,
        (object) descrizione
      })[0];
        }

        public IAsyncResult BeginDO_NewRegistro(string system_id, string codice, string descrizione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_NewRegistro", new object[3]
      {
        (object) system_id,
        (object) codice,
        (object) descrizione
      }, callback, asyncState);
        }

        public PR_Registro EndDO_NewRegistro(IAsyncResult asyncResult)
        {
            return (PR_Registro)this.EndInvoke(asyncResult)[0];
        }

        public void DO_NewRegistroAsync(string system_id, string codice, string descrizione)
        {
            this.DO_NewRegistroAsync(system_id, codice, descrizione, (object)null);
        }

        public void DO_NewRegistroAsync(string system_id, string codice, string descrizione, object userState)
        {
            if (this.DO_NewRegistroOperationCompleted == null)
                this.DO_NewRegistroOperationCompleted = new SendOrPostCallback(this.OnDO_NewRegistroOperationCompleted);
            this.InvokeAsync("DO_NewRegistro", new object[3]
      {
        (object) system_id,
        (object) codice,
        (object) descrizione
      }, this.DO_NewRegistroOperationCompleted, userState);
        }

        private void OnDO_NewRegistroOperationCompleted(object arg)
        {
            if (this.DO_NewRegistroCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_NewRegistroCompleted((object)this, new DO_NewRegistroCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_NewReport", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Report DO_NewReport(string descrizione, string valore)
        {
            return (Report)this.Invoke("DO_NewReport", new object[2]
      {
        (object) descrizione,
        (object) valore
      })[0];
        }

        public IAsyncResult BeginDO_NewReport(string descrizione, string valore, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_NewReport", new object[2]
      {
        (object) descrizione,
        (object) valore
      }, callback, asyncState);
        }

        public Report EndDO_NewReport(IAsyncResult asyncResult)
        {
            return (Report)this.EndInvoke(asyncResult)[0];
        }

        public void DO_NewReportAsync(string descrizione, string valore)
        {
            this.DO_NewReportAsync(descrizione, valore, (object)null);
        }

        public void DO_NewReportAsync(string descrizione, string valore, object userState)
        {
            if (this.DO_NewReportOperationCompleted == null)
                this.DO_NewReportOperationCompleted = new SendOrPostCallback(this.OnDO_NewReportOperationCompleted);
            this.InvokeAsync("DO_NewReport", new object[2]
      {
        (object) descrizione,
        (object) valore
      }, this.DO_NewReportOperationCompleted, userState);
        }

        private void OnDO_NewReportOperationCompleted(object arg)
        {
            if (this.DO_NewReportCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_NewReportCompleted((object)this, new DO_NewReportCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_AddSubReport", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Report DO_AddSubReport(Report report, Report subReport)
        {
            return (Report)this.Invoke("DO_AddSubReport", new object[2]
      {
        (object) report,
        (object) subReport
      })[0];
        }

        public IAsyncResult BeginDO_AddSubReport(Report report, Report subReport, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_AddSubReport", new object[2]
      {
        (object) report,
        (object) subReport
      }, callback, asyncState);
        }

        public Report EndDO_AddSubReport(IAsyncResult asyncResult)
        {
            return (Report)this.EndInvoke(asyncResult)[0];
        }

        public void DO_AddSubReportAsync(Report report, Report subReport)
        {
            this.DO_AddSubReportAsync(report, subReport, (object)null);
        }

        public void DO_AddSubReportAsync(Report report, Report subReport, object userState)
        {
            if (this.DO_AddSubReportOperationCompleted == null)
                this.DO_AddSubReportOperationCompleted = new SendOrPostCallback(this.OnDO_AddSubReportOperationCompleted);
            this.InvokeAsync("DO_AddSubReport", new object[2]
      {
        (object) report,
        (object) subReport
      }, this.DO_AddSubReportOperationCompleted, userState);
        }

        private void OnDO_AddSubReportOperationCompleted(object arg)
        {
            if (this.DO_AddSubReportCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_AddSubReportCompleted((object)this, new DO_AddSubReportCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_ReadXML", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public object[] DO_ReadXML(string xmlPath)
        {
            return (object[])this.Invoke("DO_ReadXML", new object[1]
      {
        (object) xmlPath
      })[0];
        }

        public IAsyncResult BeginDO_ReadXML(string xmlPath, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_ReadXML", new object[1]
      {
        (object) xmlPath
      }, callback, asyncState);
        }

        public object[] EndDO_ReadXML(IAsyncResult asyncResult)
        {
            return (object[])this.EndInvoke(asyncResult)[0];
        }

        public void DO_ReadXMLAsync(string xmlPath)
        {
            this.DO_ReadXMLAsync(xmlPath, (object)null);
        }

        public void DO_ReadXMLAsync(string xmlPath, object userState)
        {
            if (this.DO_ReadXMLOperationCompleted == null)
                this.DO_ReadXMLOperationCompleted = new SendOrPostCallback(this.OnDO_ReadXMLOperationCompleted);
            this.InvokeAsync("DO_ReadXML", new object[1]
      {
        (object) xmlPath
      }, this.DO_ReadXMLOperationCompleted, userState);
        }

        private void OnDO_ReadXMLOperationCompleted(object arg)
        {
            if (this.DO_ReadXMLCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_ReadXMLCompleted((object)this, new DO_ReadXMLCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_LOG_3_PARAMS", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string DO_LOG_3_PARAMS(string exceptionType, string msg, string stackTrace)
        {
            return (string)this.Invoke("DO_LOG_3_PARAMS", new object[3]
      {
        (object) exceptionType,
        (object) msg,
        (object) stackTrace
      })[0];
        }

        public IAsyncResult BeginDO_LOG_3_PARAMS(string exceptionType, string msg, string stackTrace, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_LOG_3_PARAMS", new object[3]
      {
        (object) exceptionType,
        (object) msg,
        (object) stackTrace
      }, callback, asyncState);
        }

        public string EndDO_LOG_3_PARAMS(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void DO_LOG_3_PARAMSAsync(string exceptionType, string msg, string stackTrace)
        {
            this.DO_LOG_3_PARAMSAsync(exceptionType, msg, stackTrace, (object)null);
        }

        public void DO_LOG_3_PARAMSAsync(string exceptionType, string msg, string stackTrace, object userState)
        {
            if (this.DO_LOG_3_PARAMSOperationCompleted == null)
                this.DO_LOG_3_PARAMSOperationCompleted = new SendOrPostCallback(this.OnDO_LOG_3_PARAMSOperationCompleted);
            this.InvokeAsync("DO_LOG_3_PARAMS", new object[3]
      {
        (object) exceptionType,
        (object) msg,
        (object) stackTrace
      }, this.DO_LOG_3_PARAMSOperationCompleted, userState);
        }

        private void OnDO_LOG_3_PARAMSOperationCompleted(object arg)
        {
            if (this.DO_LOG_3_PARAMSCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_LOG_3_PARAMSCompleted((object)this, new DO_LOG_3_PARAMSCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_LOG", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string DO_LOG(string exceptionType, string msg, string error, string function, string stackTrace)
        {
            return (string)this.Invoke("DO_LOG", new object[5]
      {
        (object) exceptionType,
        (object) msg,
        (object) error,
        (object) function,
        (object) stackTrace
      })[0];
        }

        public IAsyncResult BeginDO_LOG(string exceptionType, string msg, string error, string function, string stackTrace, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_LOG", new object[5]
      {
        (object) exceptionType,
        (object) msg,
        (object) error,
        (object) function,
        (object) stackTrace
      }, callback, asyncState);
        }

        public string EndDO_LOG(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void DO_LOGAsync(string exceptionType, string msg, string error, string function, string stackTrace)
        {
            this.DO_LOGAsync(exceptionType, msg, error, function, stackTrace, (object)null);
        }

        public void DO_LOGAsync(string exceptionType, string msg, string error, string function, string stackTrace, object userState)
        {
            if (this.DO_LOGOperationCompleted == null)
                this.DO_LOGOperationCompleted = new SendOrPostCallback(this.OnDO_LOGOperationCompleted);
            this.InvokeAsync("DO_LOG", new object[5]
      {
        (object) exceptionType,
        (object) msg,
        (object) error,
        (object) function,
        (object) stackTrace
      }, this.DO_LOGOperationCompleted, userState);
        }

        private void OnDO_LOGOperationCompleted(object arg)
        {
            if (this.DO_LOGCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_LOGCompleted((object)this, new DO_LOGCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDescRegistro", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string DO_GetDescRegistro(string idReg)
        {
            return (string)this.Invoke("DO_GetDescRegistro", new object[1]
      {
        (object) idReg
      })[0];
        }

        public IAsyncResult BeginDO_GetDescRegistro(string idReg, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDescRegistro", new object[1]
      {
        (object) idReg
      }, callback, asyncState);
        }

        public string EndDO_GetDescRegistro(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDescRegistroAsync(string idReg)
        {
            this.DO_GetDescRegistroAsync(idReg, (object)null);
        }

        public void DO_GetDescRegistroAsync(string idReg, object userState)
        {
            if (this.DO_GetDescRegistroOperationCompleted == null)
                this.DO_GetDescRegistroOperationCompleted = new SendOrPostCallback(this.OnDO_GetDescRegistroOperationCompleted);
            this.InvokeAsync("DO_GetDescRegistro", new object[1]
      {
        (object) idReg
      }, this.DO_GetDescRegistroOperationCompleted, userState);
        }

        private void OnDO_GetDescRegistroOperationCompleted(object arg)
        {
            if (this.DO_GetDescRegistroCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDescRegistroCompleted((object)this, new DO_GetDescRegistroCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/Do_GetAmmByIdAmm", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public PR_Amministrazione Do_GetAmmByIdAmm(int idAmm)
        {
            return (PR_Amministrazione)this.Invoke("Do_GetAmmByIdAmm", new object[1]
      {
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginDo_GetAmmByIdAmm(int idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Do_GetAmmByIdAmm", new object[1]
      {
        (object) idAmm
      }, callback, asyncState);
        }

        public PR_Amministrazione EndDo_GetAmmByIdAmm(IAsyncResult asyncResult)
        {
            return (PR_Amministrazione)this.EndInvoke(asyncResult)[0];
        }

        public void Do_GetAmmByIdAmmAsync(int idAmm)
        {
            this.Do_GetAmmByIdAmmAsync(idAmm, (object)null);
        }

        public void Do_GetAmmByIdAmmAsync(int idAmm, object userState)
        {
            if (this.Do_GetAmmByIdAmmOperationCompleted == null)
                this.Do_GetAmmByIdAmmOperationCompleted = new SendOrPostCallback(this.OnDo_GetAmmByIdAmmOperationCompleted);
            this.InvokeAsync("Do_GetAmmByIdAmm", new object[1]
      {
        (object) idAmm
      }, this.Do_GetAmmByIdAmmOperationCompleted, userState);
        }

        private void OnDo_GetAmmByIdAmmOperationCompleted(object arg)
        {
            if (this.Do_GetAmmByIdAmmCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.Do_GetAmmByIdAmmCompleted((object)this, new Do_GetAmmByIdAmmCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/Do_GetVarDescAmmByIdAmm", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string Do_GetVarDescAmmByIdAmm(int idAmm)
        {
            return (string)this.Invoke("Do_GetVarDescAmmByIdAmm", new object[1]
      {
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginDo_GetVarDescAmmByIdAmm(int idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Do_GetVarDescAmmByIdAmm", new object[1]
      {
        (object) idAmm
      }, callback, asyncState);
        }

        public string EndDo_GetVarDescAmmByIdAmm(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void Do_GetVarDescAmmByIdAmmAsync(int idAmm)
        {
            this.Do_GetVarDescAmmByIdAmmAsync(idAmm, (object)null);
        }

        public void Do_GetVarDescAmmByIdAmmAsync(int idAmm, object userState)
        {
            if (this.Do_GetVarDescAmmByIdAmmOperationCompleted == null)
                this.Do_GetVarDescAmmByIdAmmOperationCompleted = new SendOrPostCallback(this.OnDo_GetVarDescAmmByIdAmmOperationCompleted);
            this.InvokeAsync("Do_GetVarDescAmmByIdAmm", new object[1]
      {
        (object) idAmm
      }, this.Do_GetVarDescAmmByIdAmmOperationCompleted, userState);
        }

        private void OnDo_GetVarDescAmmByIdAmmOperationCompleted(object arg)
        {
            if (this.Do_GetVarDescAmmByIdAmmCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.Do_GetVarDescAmmByIdAmmCompleted((object)this, new Do_GetVarDescAmmByIdAmmCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetIdAmmByCodAmm", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public int DO_GetIdAmmByCodAmm(string codAmm)
        {
            return (int)this.Invoke("DO_GetIdAmmByCodAmm", new object[1]
      {
        (object) codAmm
      })[0];
        }

        public IAsyncResult BeginDO_GetIdAmmByCodAmm(string codAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetIdAmmByCodAmm", new object[1]
      {
        (object) codAmm
      }, callback, asyncState);
        }

        public int EndDO_GetIdAmmByCodAmm(IAsyncResult asyncResult)
        {
            return (int)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetIdAmmByCodAmmAsync(string codAmm)
        {
            this.DO_GetIdAmmByCodAmmAsync(codAmm, (object)null);
        }

        public void DO_GetIdAmmByCodAmmAsync(string codAmm, object userState)
        {
            if (this.DO_GetIdAmmByCodAmmOperationCompleted == null)
                this.DO_GetIdAmmByCodAmmOperationCompleted = new SendOrPostCallback(this.OnDO_GetIdAmmByCodAmmOperationCompleted);
            this.InvokeAsync("DO_GetIdAmmByCodAmm", new object[1]
      {
        (object) codAmm
      }, this.DO_GetIdAmmByCodAmmOperationCompleted, userState);
        }

        private void OnDO_GetIdAmmByCodAmmOperationCompleted(object arg)
        {
            if (this.DO_GetIdAmmByCodAmmCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetIdAmmByCodAmmCompleted((object)this, new DO_GetIdAmmByCodAmmCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetSedi", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public object[] DO_GetSedi(int idAmm)
        {
            return (object[])this.Invoke("DO_GetSedi", new object[1]
      {
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginDO_GetSedi(int idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetSedi", new object[1]
      {
        (object) idAmm
      }, callback, asyncState);
        }

        public object[] EndDO_GetSedi(IAsyncResult asyncResult)
        {
            return (object[])this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetSediAsync(int idAmm)
        {
            this.DO_GetSediAsync(idAmm, (object)null);
        }

        public void DO_GetSediAsync(int idAmm, object userState)
        {
            if (this.DO_GetSediOperationCompleted == null)
                this.DO_GetSediOperationCompleted = new SendOrPostCallback(this.OnDO_GetSediOperationCompleted);
            this.InvokeAsync("DO_GetSedi", new object[1]
      {
        (object) idAmm
      }, this.DO_GetSediOperationCompleted, userState);
        }

        private void OnDO_GetSediOperationCompleted(object arg)
        {
            if (this.DO_GetSediCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetSediCompleted((object)this, new DO_GetSediCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetAnniProfilazione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public object[] DO_GetAnniProfilazione(int idReg)
        {
            return (object[])this.Invoke("DO_GetAnniProfilazione", new object[1]
      {
        (object) idReg
      })[0];
        }

        public IAsyncResult BeginDO_GetAnniProfilazione(int idReg, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetAnniProfilazione", new object[1]
      {
        (object) idReg
      }, callback, asyncState);
        }

        public object[] EndDO_GetAnniProfilazione(IAsyncResult asyncResult)
        {
            return (object[])this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetAnniProfilazioneAsync(int idReg)
        {
            this.DO_GetAnniProfilazioneAsync(idReg, (object)null);
        }

        public void DO_GetAnniProfilazioneAsync(int idReg, object userState)
        {
            if (this.DO_GetAnniProfilazioneOperationCompleted == null)
                this.DO_GetAnniProfilazioneOperationCompleted = new SendOrPostCallback(this.OnDO_GetAnniProfilazioneOperationCompleted);
            this.InvokeAsync("DO_GetAnniProfilazione", new object[1]
      {
        (object) idReg
      }, this.DO_GetAnniProfilazioneOperationCompleted, userState);
        }

        private void OnDO_GetAnniProfilazioneOperationCompleted(object arg)
        {
            if (this.DO_GetAnniProfilazioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetAnniProfilazioneCompleted((object)this, new DO_GetAnniProfilazioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetRegistri", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public object[] DO_GetRegistri(int idAmm)
        {
            return (object[])this.Invoke("DO_GetRegistri", new object[1]
      {
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginDO_GetRegistri(int idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetRegistri", new object[1]
      {
        (object) idAmm
      }, callback, asyncState);
        }

        public object[] EndDO_GetRegistri(IAsyncResult asyncResult)
        {
            return (object[])this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetRegistriAsync(int idAmm)
        {
            this.DO_GetRegistriAsync(idAmm, (object)null);
        }

        public void DO_GetRegistriAsync(int idAmm, object userState)
        {
            if (this.DO_GetRegistriOperationCompleted == null)
                this.DO_GetRegistriOperationCompleted = new SendOrPostCallback(this.OnDO_GetRegistriOperationCompleted);
            this.InvokeAsync("DO_GetRegistri", new object[1]
      {
        (object) idAmm
      }, this.DO_GetRegistriOperationCompleted, userState);
        }

        private void OnDO_GetRegistriOperationCompleted(object arg)
        {
            if (this.DO_GetRegistriCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetRegistriCompleted((object)this, new DO_GetRegistriCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetAmministrazioni", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public object[] DO_GetAmministrazioni()
        {
            return (object[])this.Invoke("DO_GetAmministrazioni", new object[0])[0];
        }

        public IAsyncResult BeginDO_GetAmministrazioni(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetAmministrazioni", new object[0], callback, asyncState);
        }

        public object[] EndDO_GetAmministrazioni(IAsyncResult asyncResult)
        {
            return (object[])this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetAmministrazioniAsync()
        {
            this.DO_GetAmministrazioniAsync((object)null);
        }

        public void DO_GetAmministrazioniAsync(object userState)
        {
            if (this.DO_GetAmministrazioniOperationCompleted == null)
                this.DO_GetAmministrazioniOperationCompleted = new SendOrPostCallback(this.OnDO_GetAmministrazioniOperationCompleted);
            this.InvokeAsync("DO_GetAmministrazioni", new object[0], this.DO_GetAmministrazioniOperationCompleted, userState);
        }

        private void OnDO_GetAmministrazioniOperationCompleted(object arg)
        {
            if (this.DO_GetAmministrazioniCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetAmministrazioniCompleted((object)this, new DO_GetAmministrazioniCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetDBType", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string DO_GetDBType()
        {
            return (string)this.Invoke("DO_GetDBType", new object[0])[0];
        }

        public IAsyncResult BeginDO_GetDBType(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetDBType", new object[0], callback, asyncState);
        }

        public string EndDO_GetDBType(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetDBTypeAsync()
        {
            this.DO_GetDBTypeAsync((object)null);
        }

        public void DO_GetDBTypeAsync(object userState)
        {
            if (this.DO_GetDBTypeOperationCompleted == null)
                this.DO_GetDBTypeOperationCompleted = new SendOrPostCallback(this.OnDO_GetDBTypeOperationCompleted);
            this.InvokeAsync("DO_GetDBType", new object[0], this.DO_GetDBTypeOperationCompleted, userState);
        }

        private void OnDO_GetDBTypeOperationCompleted(object arg)
        {
            if (this.DO_GetDBTypeCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetDBTypeCompleted((object)this, new DO_GetDBTypeCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DO_GetConnectionString", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string DO_GetConnectionString()
        {
            return (string)this.Invoke("DO_GetConnectionString", new object[0])[0];
        }

        public IAsyncResult BeginDO_GetConnectionString(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DO_GetConnectionString", new object[0], callback, asyncState);
        }

        public string EndDO_GetConnectionString(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void DO_GetConnectionStringAsync()
        {
            this.DO_GetConnectionStringAsync((object)null);
        }

        public void DO_GetConnectionStringAsync(object userState)
        {
            if (this.DO_GetConnectionStringOperationCompleted == null)
                this.DO_GetConnectionStringOperationCompleted = new SendOrPostCallback(this.OnDO_GetConnectionStringOperationCompleted);
            this.InvokeAsync("DO_GetConnectionString", new object[0], this.DO_GetConnectionStringOperationCompleted, userState);
        }

        private void OnDO_GetConnectionStringOperationCompleted(object arg)
        {
            if (this.DO_GetConnectionStringCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DO_GetConnectionStringCompleted((object)this, new DO_GetConnectionStringCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/UpdateUserFilenet", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool UpdateUserFilenet(string username, string oldpwd, string newpwd, string userfullname, string idamministrazione)
        {
            return (bool)this.Invoke("UpdateUserFilenet", new object[5]
      {
        (object) username,
        (object) oldpwd,
        (object) newpwd,
        (object) userfullname,
        (object) idamministrazione
      })[0];
        }

        public IAsyncResult BeginUpdateUserFilenet(string username, string oldpwd, string newpwd, string userfullname, string idamministrazione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("UpdateUserFilenet", new object[5]
      {
        (object) username,
        (object) oldpwd,
        (object) newpwd,
        (object) userfullname,
        (object) idamministrazione
      }, callback, asyncState);
        }

        public bool EndUpdateUserFilenet(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void UpdateUserFilenetAsync(string username, string oldpwd, string newpwd, string userfullname, string idamministrazione)
        {
            this.UpdateUserFilenetAsync(username, oldpwd, newpwd, userfullname, idamministrazione, (object)null);
        }

        public void UpdateUserFilenetAsync(string username, string oldpwd, string newpwd, string userfullname, string idamministrazione, object userState)
        {
            if (this.UpdateUserFilenetOperationCompleted == null)
                this.UpdateUserFilenetOperationCompleted = new SendOrPostCallback(this.OnUpdateUserFilenetOperationCompleted);
            this.InvokeAsync("UpdateUserFilenet", new object[5]
      {
        (object) username,
        (object) oldpwd,
        (object) newpwd,
        (object) userfullname,
        (object) idamministrazione
      }, this.UpdateUserFilenetOperationCompleted, userState);
        }

        private void OnUpdateUserFilenetOperationCompleted(object arg)
        {
            if (this.UpdateUserFilenetCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.UpdateUserFilenetCompleted((object)this, new UpdateUserFilenetCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DisableUserFilenet", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DisableUserFilenet(string username)
        {
            return (bool)this.Invoke("DisableUserFilenet", new object[1]
      {
        (object) username
      })[0];
        }

        public IAsyncResult BeginDisableUserFilenet(string username, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DisableUserFilenet", new object[1]
      {
        (object) username
      }, callback, asyncState);
        }

        public bool EndDisableUserFilenet(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DisableUserFilenetAsync(string username)
        {
            this.DisableUserFilenetAsync(username, (object)null);
        }

        public void DisableUserFilenetAsync(string username, object userState)
        {
            if (this.DisableUserFilenetOperationCompleted == null)
                this.DisableUserFilenetOperationCompleted = new SendOrPostCallback(this.OnDisableUserFilenetOperationCompleted);
            this.InvokeAsync("DisableUserFilenet", new object[1]
      {
        (object) username
      }, this.DisableUserFilenetOperationCompleted, userState);
        }

        private void OnDisableUserFilenetOperationCompleted(object arg)
        {
            if (this.DisableUserFilenetCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DisableUserFilenetCompleted((object)this, new DisableUserFilenetCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DeleteUserFilenet", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DeleteUserFilenet(string username)
        {
            return (bool)this.Invoke("DeleteUserFilenet", new object[1]
      {
        (object) username
      })[0];
        }

        public IAsyncResult BeginDeleteUserFilenet(string username, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeleteUserFilenet", new object[1]
      {
        (object) username
      }, callback, asyncState);
        }

        public bool EndDeleteUserFilenet(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DeleteUserFilenetAsync(string username)
        {
            this.DeleteUserFilenetAsync(username, (object)null);
        }

        public void DeleteUserFilenetAsync(string username, object userState)
        {
            if (this.DeleteUserFilenetOperationCompleted == null)
                this.DeleteUserFilenetOperationCompleted = new SendOrPostCallback(this.OnDeleteUserFilenetOperationCompleted);
            this.InvokeAsync("DeleteUserFilenet", new object[1]
      {
        (object) username
      }, this.DeleteUserFilenetOperationCompleted, userState);
        }

        private void OnDeleteUserFilenetOperationCompleted(object arg)
        {
            if (this.DeleteUserFilenetCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DeleteUserFilenetCompleted((object)this, new DeleteUserFilenetCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddUserFilenet", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool AddUserFilenet(string userID, string userPwd, string idAmministrazione, string userFullName, string userDefaultGroup)
        {
            return (bool)this.Invoke("AddUserFilenet", new object[5]
      {
        (object) userID,
        (object) userPwd,
        (object) idAmministrazione,
        (object) userFullName,
        (object) userDefaultGroup
      })[0];
        }

        public IAsyncResult BeginAddUserFilenet(string userID, string userPwd, string idAmministrazione, string userFullName, string userDefaultGroup, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddUserFilenet", new object[5]
      {
        (object) userID,
        (object) userPwd,
        (object) idAmministrazione,
        (object) userFullName,
        (object) userDefaultGroup
      }, callback, asyncState);
        }

        public bool EndAddUserFilenet(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void AddUserFilenetAsync(string userID, string userPwd, string idAmministrazione, string userFullName, string userDefaultGroup)
        {
            this.AddUserFilenetAsync(userID, userPwd, idAmministrazione, userFullName, userDefaultGroup, (object)null);
        }

        public void AddUserFilenetAsync(string userID, string userPwd, string idAmministrazione, string userFullName, string userDefaultGroup, object userState)
        {
            if (this.AddUserFilenetOperationCompleted == null)
                this.AddUserFilenetOperationCompleted = new SendOrPostCallback(this.OnAddUserFilenetOperationCompleted);
            this.InvokeAsync("AddUserFilenet", new object[5]
      {
        (object) userID,
        (object) userPwd,
        (object) idAmministrazione,
        (object) userFullName,
        (object) userDefaultGroup
      }, this.AddUserFilenetOperationCompleted, userState);
        }

        private void OnAddUserFilenetOperationCompleted(object arg)
        {
            if (this.AddUserFilenetCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddUserFilenetCompleted((object)this, new AddUserFilenetCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/VerificaUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public UserLogin VerificaUtente(string userName)
        {
            return (UserLogin)this.Invoke("VerificaUtente", new object[1]
      {
        (object) userName
      })[0];
        }

        public IAsyncResult BeginVerificaUtente(string userName, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("VerificaUtente", new object[1]
      {
        (object) userName
      }, callback, asyncState);
        }

        public UserLogin EndVerificaUtente(IAsyncResult asyncResult)
        {
            return (UserLogin)this.EndInvoke(asyncResult)[0];
        }

        public void VerificaUtenteAsync(string userName)
        {
            this.VerificaUtenteAsync(userName, (object)null);
        }

        public void VerificaUtenteAsync(string userName, object userState)
        {
            if (this.VerificaUtenteOperationCompleted == null)
                this.VerificaUtenteOperationCompleted = new SendOrPostCallback(this.OnVerificaUtenteOperationCompleted);
            this.InvokeAsync("VerificaUtente", new object[1]
      {
        (object) userName
      }, this.VerificaUtenteOperationCompleted, userState);
        }

        private void OnVerificaUtenteOperationCompleted(object arg)
        {
            if (this.VerificaUtenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.VerificaUtenteCompleted((object)this, new VerificaUtenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/login", ParameterStyle = SoapParameterStyle.Wrapped, RequestElementName = "login", RequestNamespace = "http://localhost", ResponseElementName = "loginResponse", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        [return: XmlElement("loginResult")]
        public Utente LoginRDE(login login)
        {
            return (Utente)this.Invoke("LoginRDE", new object[1]
      {
        (object) login
      })[0];
        }

        public IAsyncResult BeginLoginRDE(login login, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("LoginRDE", new object[1]
      {
        (object) login
      }, callback, asyncState);
        }

        public Utente EndLoginRDE(IAsyncResult asyncResult)
        {
            return (Utente)this.EndInvoke(asyncResult)[0];
        }

        public void LoginRDEAsync(login login)
        {
            this.LoginRDEAsync(login, (object)null);
        }

        public void LoginRDEAsync(login login, object userState)
        {
            if (this.LoginRDEOperationCompleted == null)
                this.LoginRDEOperationCompleted = new SendOrPostCallback(this.OnLoginRDEOperationCompleted);
            this.InvokeAsync("LoginRDE", new object[1]
      {
        (object) login
      }, this.LoginRDEOperationCompleted, userState);
        }

        private void OnLoginRDEOperationCompleted(object arg)
        {
            if (this.LoginRDECompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.LoginRDECompleted((object)this, new LoginRDECompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetIdDK", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string GetIdDK(string collid)
        {
            return (string)this.Invoke("GetIdDK", new object[1]
      {
        (object) collid
      })[0];
        }

        public IAsyncResult BeginGetIdDK(string collid, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetIdDK", new object[1]
      {
        (object) collid
      }, callback, asyncState);
        }

        public string EndGetIdDK(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void GetIdDKAsync(string collid)
        {
            this.GetIdDKAsync(collid, (object)null);
        }

        public void GetIdDKAsync(string collid, object userState)
        {
            if (this.GetIdDKOperationCompleted == null)
                this.GetIdDKOperationCompleted = new SendOrPostCallback(this.OnGetIdDKOperationCompleted);
            this.InvokeAsync("GetIdDK", new object[1]
      {
        (object) collid
      }, this.GetIdDKOperationCompleted, userState);
        }

        private void OnGetIdDKOperationCompleted(object arg)
        {
            if (this.GetIdDKCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetIdDKCompleted((object)this, new GetIdDKCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetDKPath", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string GetDKPath(string docNumber)
        {
            return (string)this.Invoke("GetDKPath", new object[1]
      {
        (object) docNumber
      })[0];
        }

        public IAsyncResult BeginGetDKPath(string docNumber, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetDKPath", new object[1]
      {
        (object) docNumber
      }, callback, asyncState);
        }

        public string EndGetDKPath(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void GetDKPathAsync(string docNumber)
        {
            this.GetDKPathAsync(docNumber, (object)null);
        }

        public void GetDKPathAsync(string docNumber, object userState)
        {
            if (this.GetDKPathOperationCompleted == null)
                this.GetDKPathOperationCompleted = new SendOrPostCallback(this.OnGetDKPathOperationCompleted);
            this.InvokeAsync("GetDKPath", new object[1]
      {
        (object) docNumber
      }, this.GetDKPathOperationCompleted, userState);
        }

        private void OnGetDKPathOperationCompleted(object arg)
        {
            if (this.GetDKPathCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetDKPathCompleted((object)this, new GetDKPathCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetUtenti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Utente[] amministrazioneGetUtenti(string idAmm, string search)
        {
            return (Utente[])this.Invoke("amministrazioneGetUtenti", new object[2]
      {
        (object) idAmm,
        (object) search
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetUtenti(string idAmm, string search, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetUtenti", new object[2]
      {
        (object) idAmm,
        (object) search
      }, callback, asyncState);
        }

        public Utente[] EndamministrazioneGetUtenti(IAsyncResult asyncResult)
        {
            return (Utente[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetUtentiAsync(string idAmm, string search)
        {
            this.amministrazioneGetUtentiAsync(idAmm, search, (object)null);
        }

        public void amministrazioneGetUtentiAsync(string idAmm, string search, object userState)
        {
            if (this.amministrazioneGetUtentiOperationCompleted == null)
                this.amministrazioneGetUtentiOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetUtentiOperationCompleted);
            this.InvokeAsync("amministrazioneGetUtenti", new object[2]
      {
        (object) idAmm,
        (object) search
      }, this.amministrazioneGetUtentiOperationCompleted, userState);
        }

        private void OnamministrazioneGetUtentiOperationCompleted(object arg)
        {
            if (this.amministrazioneGetUtentiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetUtentiCompleted((object)this, new amministrazioneGetUtentiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetAmministrazioni", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Amministrazione[] amministrazioneGetAmministrazioni()
        {
            return (Amministrazione[])this.Invoke("amministrazioneGetAmministrazioni", new object[0])[0];
        }

        public IAsyncResult BeginamministrazioneGetAmministrazioni(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetAmministrazioni", new object[0], callback, asyncState);
        }

        public Amministrazione[] EndamministrazioneGetAmministrazioni(IAsyncResult asyncResult)
        {
            return (Amministrazione[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetAmministrazioniAsync()
        {
            this.amministrazioneGetAmministrazioniAsync((object)null);
        }

        public void amministrazioneGetAmministrazioniAsync(object userState)
        {
            if (this.amministrazioneGetAmministrazioniOperationCompleted == null)
                this.amministrazioneGetAmministrazioniOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetAmministrazioniOperationCompleted);
            this.InvokeAsync("amministrazioneGetAmministrazioni", new object[0], this.amministrazioneGetAmministrazioniOperationCompleted, userState);
        }

        private void OnamministrazioneGetAmministrazioniOperationCompleted(object arg)
        {
            if (this.amministrazioneGetAmministrazioniCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetAmministrazioniCompleted((object)this, new amministrazioneGetAmministrazioniCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetParametroConfigurazione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Configurazione amministrazioneGetParametroConfigurazione()
        {
            return (Configurazione)this.Invoke("amministrazioneGetParametroConfigurazione", new object[0])[0];
        }

        public IAsyncResult BeginamministrazioneGetParametroConfigurazione(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetParametroConfigurazione", new object[0], callback, asyncState);
        }

        public Configurazione EndamministrazioneGetParametroConfigurazione(IAsyncResult asyncResult)
        {
            return (Configurazione)this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetParametroConfigurazioneAsync()
        {
            this.amministrazioneGetParametroConfigurazioneAsync((object)null);
        }

        public void amministrazioneGetParametroConfigurazioneAsync(object userState)
        {
            if (this.amministrazioneGetParametroConfigurazioneOperationCompleted == null)
                this.amministrazioneGetParametroConfigurazioneOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetParametroConfigurazioneOperationCompleted);
            this.InvokeAsync("amministrazioneGetParametroConfigurazione", new object[0], this.amministrazioneGetParametroConfigurazioneOperationCompleted, userState);
        }

        private void OnamministrazioneGetParametroConfigurazioneOperationCompleted(object arg)
        {
            if (this.amministrazioneGetParametroConfigurazioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetParametroConfigurazioneCompleted((object)this, new amministrazioneGetParametroConfigurazioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmSostituzioneUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmSostituzioneUtente(string idPeopleNewUT, string idCorrGlobRuolo)
        {
            return (EsitoOperazione)this.Invoke("AmmSostituzioneUtente", new object[2]
      {
        (object) idPeopleNewUT,
        (object) idCorrGlobRuolo
      })[0];
        }

        public IAsyncResult BeginAmmSostituzioneUtente(string idPeopleNewUT, string idCorrGlobRuolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmSostituzioneUtente", new object[2]
      {
        (object) idPeopleNewUT,
        (object) idCorrGlobRuolo
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmSostituzioneUtente(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmSostituzioneUtenteAsync(string idPeopleNewUT, string idCorrGlobRuolo)
        {
            this.AmmSostituzioneUtenteAsync(idPeopleNewUT, idCorrGlobRuolo, (object)null);
        }

        public void AmmSostituzioneUtenteAsync(string idPeopleNewUT, string idCorrGlobRuolo, object userState)
        {
            if (this.AmmSostituzioneUtenteOperationCompleted == null)
                this.AmmSostituzioneUtenteOperationCompleted = new SendOrPostCallback(this.OnAmmSostituzioneUtenteOperationCompleted);
            this.InvokeAsync("AmmSostituzioneUtente", new object[2]
      {
        (object) idPeopleNewUT,
        (object) idCorrGlobRuolo
      }, this.AmmSostituzioneUtenteOperationCompleted, userState);
        }

        private void OnAmmSostituzioneUtenteOperationCompleted(object arg)
        {
            if (this.AmmSostituzioneUtenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmSostituzioneUtenteCompleted((object)this, new AmmSostituzioneUtenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmRifiutaTrasmConWF", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmRifiutaTrasmConWF(string idCorrGlobRuolo)
        {
            return (EsitoOperazione)this.Invoke("AmmRifiutaTrasmConWF", new object[1]
      {
        (object) idCorrGlobRuolo
      })[0];
        }

        public IAsyncResult BeginAmmRifiutaTrasmConWF(string idCorrGlobRuolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmRifiutaTrasmConWF", new object[1]
      {
        (object) idCorrGlobRuolo
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmRifiutaTrasmConWF(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmRifiutaTrasmConWFAsync(string idCorrGlobRuolo)
        {
            this.AmmRifiutaTrasmConWFAsync(idCorrGlobRuolo, (object)null);
        }

        public void AmmRifiutaTrasmConWFAsync(string idCorrGlobRuolo, object userState)
        {
            if (this.AmmRifiutaTrasmConWFOperationCompleted == null)
                this.AmmRifiutaTrasmConWFOperationCompleted = new SendOrPostCallback(this.OnAmmRifiutaTrasmConWFOperationCompleted);
            this.InvokeAsync("AmmRifiutaTrasmConWF", new object[1]
      {
        (object) idCorrGlobRuolo
      }, this.AmmRifiutaTrasmConWFOperationCompleted, userState);
        }

        private void OnAmmRifiutaTrasmConWFOperationCompleted(object arg)
        {
            if (this.AmmRifiutaTrasmConWFCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmRifiutaTrasmConWFCompleted((object)this, new AmmRifiutaTrasmConWFCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmVerificaTrasmRuolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmVerificaTrasmRuolo(string idCorrGlobRuolo)
        {
            return (EsitoOperazione)this.Invoke("AmmVerificaTrasmRuolo", new object[1]
      {
        (object) idCorrGlobRuolo
      })[0];
        }

        public IAsyncResult BeginAmmVerificaTrasmRuolo(string idCorrGlobRuolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmVerificaTrasmRuolo", new object[1]
      {
        (object) idCorrGlobRuolo
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmVerificaTrasmRuolo(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmVerificaTrasmRuoloAsync(string idCorrGlobRuolo)
        {
            this.AmmVerificaTrasmRuoloAsync(idCorrGlobRuolo, (object)null);
        }

        public void AmmVerificaTrasmRuoloAsync(string idCorrGlobRuolo, object userState)
        {
            if (this.AmmVerificaTrasmRuoloOperationCompleted == null)
                this.AmmVerificaTrasmRuoloOperationCompleted = new SendOrPostCallback(this.OnAmmVerificaTrasmRuoloOperationCompleted);
            this.InvokeAsync("AmmVerificaTrasmRuolo", new object[1]
      {
        (object) idCorrGlobRuolo
      }, this.AmmVerificaTrasmRuoloOperationCompleted, userState);
        }

        private void OnAmmVerificaTrasmRuoloOperationCompleted(object arg)
        {
            if (this.AmmVerificaTrasmRuoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmVerificaTrasmRuoloCompleted((object)this, new AmmVerificaTrasmRuoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmVerificaUtenteLoggato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmVerificaUtenteLoggato(string userId, string idAmm)
        {
            return (EsitoOperazione)this.Invoke("AmmVerificaUtenteLoggato", new object[2]
      {
        (object) userId,
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginAmmVerificaUtenteLoggato(string userId, string idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmVerificaUtenteLoggato", new object[2]
      {
        (object) userId,
        (object) idAmm
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmVerificaUtenteLoggato(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmVerificaUtenteLoggatoAsync(string userId, string idAmm)
        {
            this.AmmVerificaUtenteLoggatoAsync(userId, idAmm, (object)null);
        }

        public void AmmVerificaUtenteLoggatoAsync(string userId, string idAmm, object userState)
        {
            if (this.AmmVerificaUtenteLoggatoOperationCompleted == null)
                this.AmmVerificaUtenteLoggatoOperationCompleted = new SendOrPostCallback(this.OnAmmVerificaUtenteLoggatoOperationCompleted);
            this.InvokeAsync("AmmVerificaUtenteLoggato", new object[2]
      {
        (object) userId,
        (object) idAmm
      }, this.AmmVerificaUtenteLoggatoOperationCompleted, userState);
        }

        private void OnAmmVerificaUtenteLoggatoOperationCompleted(object arg)
        {
            if (this.AmmVerificaUtenteLoggatoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmVerificaUtenteLoggatoCompleted((object)this, new AmmVerificaUtenteLoggatoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmEliminaADLUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmEliminaADLUtente(string idPeople, string idCorrGlobGruppo)
        {
            return (EsitoOperazione)this.Invoke("AmmEliminaADLUtente", new object[2]
      {
        (object) idPeople,
        (object) idCorrGlobGruppo
      })[0];
        }

        public IAsyncResult BeginAmmEliminaADLUtente(string idPeople, string idCorrGlobGruppo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmEliminaADLUtente", new object[2]
      {
        (object) idPeople,
        (object) idCorrGlobGruppo
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmEliminaADLUtente(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmEliminaADLUtenteAsync(string idPeople, string idCorrGlobGruppo)
        {
            this.AmmEliminaADLUtenteAsync(idPeople, idCorrGlobGruppo, (object)null);
        }

        public void AmmEliminaADLUtenteAsync(string idPeople, string idCorrGlobGruppo, object userState)
        {
            if (this.AmmEliminaADLUtenteOperationCompleted == null)
                this.AmmEliminaADLUtenteOperationCompleted = new SendOrPostCallback(this.OnAmmEliminaADLUtenteOperationCompleted);
            this.InvokeAsync("AmmEliminaADLUtente", new object[2]
      {
        (object) idPeople,
        (object) idCorrGlobGruppo
      }, this.AmmEliminaADLUtenteOperationCompleted, userState);
        }

        private void OnAmmEliminaADLUtenteOperationCompleted(object arg)
        {
            if (this.AmmEliminaADLUtenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmEliminaADLUtenteCompleted((object)this, new AmmEliminaADLUtenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmCancUtenteInRuolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmCancUtenteInRuolo(string idPeople, string idGruppo)
        {
            return (EsitoOperazione)this.Invoke("AmmCancUtenteInRuolo", new object[2]
      {
        (object) idPeople,
        (object) idGruppo
      })[0];
        }

        public IAsyncResult BeginAmmCancUtenteInRuolo(string idPeople, string idGruppo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmCancUtenteInRuolo", new object[2]
      {
        (object) idPeople,
        (object) idGruppo
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmCancUtenteInRuolo(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmCancUtenteInRuoloAsync(string idPeople, string idGruppo)
        {
            this.AmmCancUtenteInRuoloAsync(idPeople, idGruppo, (object)null);
        }

        public void AmmCancUtenteInRuoloAsync(string idPeople, string idGruppo, object userState)
        {
            if (this.AmmCancUtenteInRuoloOperationCompleted == null)
                this.AmmCancUtenteInRuoloOperationCompleted = new SendOrPostCallback(this.OnAmmCancUtenteInRuoloOperationCompleted);
            this.InvokeAsync("AmmCancUtenteInRuolo", new object[2]
      {
        (object) idPeople,
        (object) idGruppo
      }, this.AmmCancUtenteInRuoloOperationCompleted, userState);
        }

        private void OnAmmCancUtenteInRuoloOperationCompleted(object arg)
        {
            if (this.AmmCancUtenteInRuoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmCancUtenteInRuoloCompleted((object)this, new AmmCancUtenteInRuoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmInsTrasmUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmInsTrasmUtente(string idPeople, string idCorrGlobRuolo)
        {
            return (EsitoOperazione)this.Invoke("AmmInsTrasmUtente", new object[2]
      {
        (object) idPeople,
        (object) idCorrGlobRuolo
      })[0];
        }

        public IAsyncResult BeginAmmInsTrasmUtente(string idPeople, string idCorrGlobRuolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmInsTrasmUtente", new object[2]
      {
        (object) idPeople,
        (object) idCorrGlobRuolo
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmInsTrasmUtente(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmInsTrasmUtenteAsync(string idPeople, string idCorrGlobRuolo)
        {
            this.AmmInsTrasmUtenteAsync(idPeople, idCorrGlobRuolo, (object)null);
        }

        public void AmmInsTrasmUtenteAsync(string idPeople, string idCorrGlobRuolo, object userState)
        {
            if (this.AmmInsTrasmUtenteOperationCompleted == null)
                this.AmmInsTrasmUtenteOperationCompleted = new SendOrPostCallback(this.OnAmmInsTrasmUtenteOperationCompleted);
            this.InvokeAsync("AmmInsTrasmUtente", new object[2]
      {
        (object) idPeople,
        (object) idCorrGlobRuolo
      }, this.AmmInsTrasmUtenteOperationCompleted, userState);
        }

        private void OnAmmInsTrasmUtenteOperationCompleted(object arg)
        {
            if (this.AmmInsTrasmUtenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmInsTrasmUtenteCompleted((object)this, new AmmInsTrasmUtenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmInsUtenteInRuolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmInsUtenteInRuolo(string idPeople, string idGruppo)
        {
            return (EsitoOperazione)this.Invoke("AmmInsUtenteInRuolo", new object[2]
      {
        (object) idPeople,
        (object) idGruppo
      })[0];
        }

        public IAsyncResult BeginAmmInsUtenteInRuolo(string idPeople, string idGruppo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmInsUtenteInRuolo", new object[2]
      {
        (object) idPeople,
        (object) idGruppo
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmInsUtenteInRuolo(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmInsUtenteInRuoloAsync(string idPeople, string idGruppo)
        {
            this.AmmInsUtenteInRuoloAsync(idPeople, idGruppo, (object)null);
        }

        public void AmmInsUtenteInRuoloAsync(string idPeople, string idGruppo, object userState)
        {
            if (this.AmmInsUtenteInRuoloOperationCompleted == null)
                this.AmmInsUtenteInRuoloOperationCompleted = new SendOrPostCallback(this.OnAmmInsUtenteInRuoloOperationCompleted);
            this.InvokeAsync("AmmInsUtenteInRuolo", new object[2]
      {
        (object) idPeople,
        (object) idGruppo
      }, this.AmmInsUtenteInRuoloOperationCompleted, userState);
        }

        private void OnAmmInsUtenteInRuoloOperationCompleted(object arg)
        {
            if (this.AmmInsUtenteInRuoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmInsUtenteInRuoloCompleted((object)this, new AmmInsUtenteInRuoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmInsTipoFunzioni", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmInsTipoFunzioni(OrgTipoFunzione[] listaFunzioni)
        {
            return (EsitoOperazione)this.Invoke("AmmInsTipoFunzioni", new object[1]
      {
        (object) listaFunzioni
      })[0];
        }

        public IAsyncResult BeginAmmInsTipoFunzioni(OrgTipoFunzione[] listaFunzioni, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmInsTipoFunzioni", new object[1]
      {
        (object) listaFunzioni
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmInsTipoFunzioni(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmInsTipoFunzioniAsync(OrgTipoFunzione[] listaFunzioni)
        {
            this.AmmInsTipoFunzioniAsync(listaFunzioni, (object)null);
        }

        public void AmmInsTipoFunzioniAsync(OrgTipoFunzione[] listaFunzioni, object userState)
        {
            if (this.AmmInsTipoFunzioniOperationCompleted == null)
                this.AmmInsTipoFunzioniOperationCompleted = new SendOrPostCallback(this.OnAmmInsTipoFunzioniOperationCompleted);
            this.InvokeAsync("AmmInsTipoFunzioni", new object[1]
      {
        (object) listaFunzioni
      }, this.AmmInsTipoFunzioniOperationCompleted, userState);
        }

        private void OnAmmInsTipoFunzioniOperationCompleted(object arg)
        {
            if (this.AmmInsTipoFunzioniCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmInsTipoFunzioniCompleted((object)this, new AmmInsTipoFunzioniCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmInsRegistri", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmInsRegistri(OrgRegistro[] listaRegistri)
        {
            return (EsitoOperazione)this.Invoke("AmmInsRegistri", new object[1]
      {
        (object) listaRegistri
      })[0];
        }

        public IAsyncResult BeginAmmInsRegistri(OrgRegistro[] listaRegistri, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmInsRegistri", new object[1]
      {
        (object) listaRegistri
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmInsRegistri(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmInsRegistriAsync(OrgRegistro[] listaRegistri)
        {
            this.AmmInsRegistriAsync(listaRegistri, (object)null);
        }

        public void AmmInsRegistriAsync(OrgRegistro[] listaRegistri, object userState)
        {
            if (this.AmmInsRegistriOperationCompleted == null)
                this.AmmInsRegistriOperationCompleted = new SendOrPostCallback(this.OnAmmInsRegistriOperationCompleted);
            this.InvokeAsync("AmmInsRegistri", new object[1]
      {
        (object) listaRegistri
      }, this.AmmInsRegistriOperationCompleted, userState);
        }

        private void OnAmmInsRegistriOperationCompleted(object arg)
        {
            if (this.AmmInsRegistriCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmInsRegistriCompleted((object)this, new AmmInsRegistriCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmEliminaRuolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmEliminaRuolo(OrgRuolo ruolo)
        {
            return (EsitoOperazione)this.Invoke("AmmEliminaRuolo", new object[1]
      {
        (object) ruolo
      })[0];
        }

        public IAsyncResult BeginAmmEliminaRuolo(OrgRuolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmEliminaRuolo", new object[1]
      {
        (object) ruolo
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmEliminaRuolo(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmEliminaRuoloAsync(OrgRuolo ruolo)
        {
            this.AmmEliminaRuoloAsync(ruolo, (object)null);
        }

        public void AmmEliminaRuoloAsync(OrgRuolo ruolo, object userState)
        {
            if (this.AmmEliminaRuoloOperationCompleted == null)
                this.AmmEliminaRuoloOperationCompleted = new SendOrPostCallback(this.OnAmmEliminaRuoloOperationCompleted);
            this.InvokeAsync("AmmEliminaRuolo", new object[1]
      {
        (object) ruolo
      }, this.AmmEliminaRuoloOperationCompleted, userState);
        }

        private void OnAmmEliminaRuoloOperationCompleted(object arg)
        {
            if (this.AmmEliminaRuoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmEliminaRuoloCompleted((object)this, new AmmEliminaRuoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmModRuolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmModRuolo(OrgRuolo ruolo)
        {
            return (EsitoOperazione)this.Invoke("AmmModRuolo", new object[1]
      {
        (object) ruolo
      })[0];
        }

        public IAsyncResult BeginAmmModRuolo(OrgRuolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmModRuolo", new object[1]
      {
        (object) ruolo
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmModRuolo(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmModRuoloAsync(OrgRuolo ruolo)
        {
            this.AmmModRuoloAsync(ruolo, (object)null);
        }

        public void AmmModRuoloAsync(OrgRuolo ruolo, object userState)
        {
            if (this.AmmModRuoloOperationCompleted == null)
                this.AmmModRuoloOperationCompleted = new SendOrPostCallback(this.OnAmmModRuoloOperationCompleted);
            this.InvokeAsync("AmmModRuolo", new object[1]
      {
        (object) ruolo
      }, this.AmmModRuoloOperationCompleted, userState);
        }

        private void OnAmmModRuoloOperationCompleted(object arg)
        {
            if (this.AmmModRuoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmModRuoloCompleted((object)this, new AmmModRuoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmInsNuovoRuolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmInsNuovoRuolo(OrgRuolo newRuolo)
        {
            return (EsitoOperazione)this.Invoke("AmmInsNuovoRuolo", new object[1]
      {
        (object) newRuolo
      })[0];
        }

        public IAsyncResult BeginAmmInsNuovoRuolo(OrgRuolo newRuolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmInsNuovoRuolo", new object[1]
      {
        (object) newRuolo
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmInsNuovoRuolo(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmInsNuovoRuoloAsync(OrgRuolo newRuolo)
        {
            this.AmmInsNuovoRuoloAsync(newRuolo, (object)null);
        }

        public void AmmInsNuovoRuoloAsync(OrgRuolo newRuolo, object userState)
        {
            if (this.AmmInsNuovoRuoloOperationCompleted == null)
                this.AmmInsNuovoRuoloOperationCompleted = new SendOrPostCallback(this.OnAmmInsNuovoRuoloOperationCompleted);
            this.InvokeAsync("AmmInsNuovoRuolo", new object[1]
      {
        (object) newRuolo
      }, this.AmmInsNuovoRuoloOperationCompleted, userState);
        }

        private void OnAmmInsNuovoRuoloOperationCompleted(object arg)
        {
            if (this.AmmInsNuovoRuoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmInsNuovoRuoloCompleted((object)this, new AmmInsNuovoRuoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmEliminaUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmEliminaUO(string idCorrGlob)
        {
            return (EsitoOperazione)this.Invoke("AmmEliminaUO", new object[1]
      {
        (object) idCorrGlob
      })[0];
        }

        public IAsyncResult BeginAmmEliminaUO(string idCorrGlob, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmEliminaUO", new object[1]
      {
        (object) idCorrGlob
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmEliminaUO(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmEliminaUOAsync(string idCorrGlob)
        {
            this.AmmEliminaUOAsync(idCorrGlob, (object)null);
        }

        public void AmmEliminaUOAsync(string idCorrGlob, object userState)
        {
            if (this.AmmEliminaUOOperationCompleted == null)
                this.AmmEliminaUOOperationCompleted = new SendOrPostCallback(this.OnAmmEliminaUOOperationCompleted);
            this.InvokeAsync("AmmEliminaUO", new object[1]
      {
        (object) idCorrGlob
      }, this.AmmEliminaUOOperationCompleted, userState);
        }

        private void OnAmmEliminaUOOperationCompleted(object arg)
        {
            if (this.AmmEliminaUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmEliminaUOCompleted((object)this, new AmmEliminaUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmModUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmModUO(OrgUO theUO)
        {
            return (EsitoOperazione)this.Invoke("AmmModUO", new object[1]
      {
        (object) theUO
      })[0];
        }

        public IAsyncResult BeginAmmModUO(OrgUO theUO, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmModUO", new object[1]
      {
        (object) theUO
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmModUO(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmModUOAsync(OrgUO theUO)
        {
            this.AmmModUOAsync(theUO, (object)null);
        }

        public void AmmModUOAsync(OrgUO theUO, object userState)
        {
            if (this.AmmModUOOperationCompleted == null)
                this.AmmModUOOperationCompleted = new SendOrPostCallback(this.OnAmmModUOOperationCompleted);
            this.InvokeAsync("AmmModUO", new object[1]
      {
        (object) theUO
      }, this.AmmModUOOperationCompleted, userState);
        }

        private void OnAmmModUOOperationCompleted(object arg)
        {
            if (this.AmmModUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmModUOCompleted((object)this, new AmmModUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmInsNuovaUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public EsitoOperazione AmmInsNuovaUO(OrgUO nuovaUO)
        {
            return (EsitoOperazione)this.Invoke("AmmInsNuovaUO", new object[1]
      {
        (object) nuovaUO
      })[0];
        }

        public IAsyncResult BeginAmmInsNuovaUO(OrgUO nuovaUO, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmInsNuovaUO", new object[1]
      {
        (object) nuovaUO
      }, callback, asyncState);
        }

        public EsitoOperazione EndAmmInsNuovaUO(IAsyncResult asyncResult)
        {
            return (EsitoOperazione)this.EndInvoke(asyncResult)[0];
        }

        public void AmmInsNuovaUOAsync(OrgUO nuovaUO)
        {
            this.AmmInsNuovaUOAsync(nuovaUO, (object)null);
        }

        public void AmmInsNuovaUOAsync(OrgUO nuovaUO, object userState)
        {
            if (this.AmmInsNuovaUOOperationCompleted == null)
                this.AmmInsNuovaUOOperationCompleted = new SendOrPostCallback(this.OnAmmInsNuovaUOOperationCompleted);
            this.InvokeAsync("AmmInsNuovaUO", new object[1]
      {
        (object) nuovaUO
      }, this.AmmInsNuovaUOOperationCompleted, userState);
        }

        private void OnAmmInsNuovaUOOperationCompleted(object arg)
        {
            if (this.AmmInsNuovaUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmInsNuovaUOCompleted((object)this, new AmmInsNuovaUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmGetDatiUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public OrgUtente AmmGetDatiUtente(string idCorrGlob)
        {
            return (OrgUtente)this.Invoke("AmmGetDatiUtente", new object[1]
      {
        (object) idCorrGlob
      })[0];
        }

        public IAsyncResult BeginAmmGetDatiUtente(string idCorrGlob, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmGetDatiUtente", new object[1]
      {
        (object) idCorrGlob
      }, callback, asyncState);
        }

        public OrgUtente EndAmmGetDatiUtente(IAsyncResult asyncResult)
        {
            return (OrgUtente)this.EndInvoke(asyncResult)[0];
        }

        public void AmmGetDatiUtenteAsync(string idCorrGlob)
        {
            this.AmmGetDatiUtenteAsync(idCorrGlob, (object)null);
        }

        public void AmmGetDatiUtenteAsync(string idCorrGlob, object userState)
        {
            if (this.AmmGetDatiUtenteOperationCompleted == null)
                this.AmmGetDatiUtenteOperationCompleted = new SendOrPostCallback(this.OnAmmGetDatiUtenteOperationCompleted);
            this.InvokeAsync("AmmGetDatiUtente", new object[1]
      {
        (object) idCorrGlob
      }, this.AmmGetDatiUtenteOperationCompleted, userState);
        }

        private void OnAmmGetDatiUtenteOperationCompleted(object arg)
        {
            if (this.AmmGetDatiUtenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmGetDatiUtenteCompleted((object)this, new AmmGetDatiUtenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmGetListTipiRuolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public OrgTipoRuolo[] AmmGetListTipiRuolo(string idAmm)
        {
            return (OrgTipoRuolo[])this.Invoke("AmmGetListTipiRuolo", new object[1]
      {
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginAmmGetListTipiRuolo(string idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmGetListTipiRuolo", new object[1]
      {
        (object) idAmm
      }, callback, asyncState);
        }

        public OrgTipoRuolo[] EndAmmGetListTipiRuolo(IAsyncResult asyncResult)
        {
            return (OrgTipoRuolo[])this.EndInvoke(asyncResult)[0];
        }

        public void AmmGetListTipiRuoloAsync(string idAmm)
        {
            this.AmmGetListTipiRuoloAsync(idAmm, (object)null);
        }

        public void AmmGetListTipiRuoloAsync(string idAmm, object userState)
        {
            if (this.AmmGetListTipiRuoloOperationCompleted == null)
                this.AmmGetListTipiRuoloOperationCompleted = new SendOrPostCallback(this.OnAmmGetListTipiRuoloOperationCompleted);
            this.InvokeAsync("AmmGetListTipiRuolo", new object[1]
      {
        (object) idAmm
      }, this.AmmGetListTipiRuoloOperationCompleted, userState);
        }

        private void OnAmmGetListTipiRuoloOperationCompleted(object arg)
        {
            if (this.AmmGetListTipiRuoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmGetListTipiRuoloCompleted((object)this, new AmmGetListTipiRuoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmGetListFunzioni", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public OrgTipoFunzione[] AmmGetListFunzioni(string idAmm, string idRuolo)
        {
            return (OrgTipoFunzione[])this.Invoke("AmmGetListFunzioni", new object[2]
      {
        (object) idAmm,
        (object) idRuolo
      })[0];
        }

        public IAsyncResult BeginAmmGetListFunzioni(string idAmm, string idRuolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmGetListFunzioni", new object[2]
      {
        (object) idAmm,
        (object) idRuolo
      }, callback, asyncState);
        }

        public OrgTipoFunzione[] EndAmmGetListFunzioni(IAsyncResult asyncResult)
        {
            return (OrgTipoFunzione[])this.EndInvoke(asyncResult)[0];
        }

        public void AmmGetListFunzioniAsync(string idAmm, string idRuolo)
        {
            this.AmmGetListFunzioniAsync(idAmm, idRuolo, (object)null);
        }

        public void AmmGetListFunzioniAsync(string idAmm, string idRuolo, object userState)
        {
            if (this.AmmGetListFunzioniOperationCompleted == null)
                this.AmmGetListFunzioniOperationCompleted = new SendOrPostCallback(this.OnAmmGetListFunzioniOperationCompleted);
            this.InvokeAsync("AmmGetListFunzioni", new object[2]
      {
        (object) idAmm,
        (object) idRuolo
      }, this.AmmGetListFunzioniOperationCompleted, userState);
        }

        private void OnAmmGetListFunzioniOperationCompleted(object arg)
        {
            if (this.AmmGetListFunzioniCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmGetListFunzioniCompleted((object)this, new AmmGetListFunzioniCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmGetListRegistri", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public OrgRegistro[] AmmGetListRegistri(string idAmm, string idRuolo)
        {
            return (OrgRegistro[])this.Invoke("AmmGetListRegistri", new object[2]
      {
        (object) idAmm,
        (object) idRuolo
      })[0];
        }

        public IAsyncResult BeginAmmGetListRegistri(string idAmm, string idRuolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmGetListRegistri", new object[2]
      {
        (object) idAmm,
        (object) idRuolo
      }, callback, asyncState);
        }

        public OrgRegistro[] EndAmmGetListRegistri(IAsyncResult asyncResult)
        {
            return (OrgRegistro[])this.EndInvoke(asyncResult)[0];
        }

        public void AmmGetListRegistriAsync(string idAmm, string idRuolo)
        {
            this.AmmGetListRegistriAsync(idAmm, idRuolo, (object)null);
        }

        public void AmmGetListRegistriAsync(string idAmm, string idRuolo, object userState)
        {
            if (this.AmmGetListRegistriOperationCompleted == null)
                this.AmmGetListRegistriOperationCompleted = new SendOrPostCallback(this.OnAmmGetListRegistriOperationCompleted);
            this.InvokeAsync("AmmGetListRegistri", new object[2]
      {
        (object) idAmm,
        (object) idRuolo
      }, this.AmmGetListRegistriOperationCompleted, userState);
        }

        private void OnAmmGetListRegistriOperationCompleted(object arg)
        {
            if (this.AmmGetListRegistriCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmGetListRegistriCompleted((object)this, new AmmGetListRegistriCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmGetListUtenti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public OrgUtente[] AmmGetListUtenti(string idAmm, string ricercaPer, string testoDaRicercare, string IDesclusi)
        {
            return (OrgUtente[])this.Invoke("AmmGetListUtenti", new object[4]
      {
        (object) idAmm,
        (object) ricercaPer,
        (object) testoDaRicercare,
        (object) IDesclusi
      })[0];
        }

        public IAsyncResult BeginAmmGetListUtenti(string idAmm, string ricercaPer, string testoDaRicercare, string IDesclusi, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmGetListUtenti", new object[4]
      {
        (object) idAmm,
        (object) ricercaPer,
        (object) testoDaRicercare,
        (object) IDesclusi
      }, callback, asyncState);
        }

        public OrgUtente[] EndAmmGetListUtenti(IAsyncResult asyncResult)
        {
            return (OrgUtente[])this.EndInvoke(asyncResult)[0];
        }

        public void AmmGetListUtentiAsync(string idAmm, string ricercaPer, string testoDaRicercare, string IDesclusi)
        {
            this.AmmGetListUtentiAsync(idAmm, ricercaPer, testoDaRicercare, IDesclusi, (object)null);
        }

        public void AmmGetListUtentiAsync(string idAmm, string ricercaPer, string testoDaRicercare, string IDesclusi, object userState)
        {
            if (this.AmmGetListUtentiOperationCompleted == null)
                this.AmmGetListUtentiOperationCompleted = new SendOrPostCallback(this.OnAmmGetListUtentiOperationCompleted);
            this.InvokeAsync("AmmGetListUtenti", new object[4]
      {
        (object) idAmm,
        (object) ricercaPer,
        (object) testoDaRicercare,
        (object) IDesclusi
      }, this.AmmGetListUtentiOperationCompleted, userState);
        }

        private void OnAmmGetListUtentiOperationCompleted(object arg)
        {
            if (this.AmmGetListUtentiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmGetListUtentiCompleted((object)this, new AmmGetListUtentiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmGetListUtentiRuolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public OrgUtente[] AmmGetListUtentiRuolo(string idRuolo)
        {
            return (OrgUtente[])this.Invoke("AmmGetListUtentiRuolo", new object[1]
      {
        (object) idRuolo
      })[0];
        }

        public IAsyncResult BeginAmmGetListUtentiRuolo(string idRuolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmGetListUtentiRuolo", new object[1]
      {
        (object) idRuolo
      }, callback, asyncState);
        }

        public OrgUtente[] EndAmmGetListUtentiRuolo(IAsyncResult asyncResult)
        {
            return (OrgUtente[])this.EndInvoke(asyncResult)[0];
        }

        public void AmmGetListUtentiRuoloAsync(string idRuolo)
        {
            this.AmmGetListUtentiRuoloAsync(idRuolo, (object)null);
        }

        public void AmmGetListUtentiRuoloAsync(string idRuolo, object userState)
        {
            if (this.AmmGetListUtentiRuoloOperationCompleted == null)
                this.AmmGetListUtentiRuoloOperationCompleted = new SendOrPostCallback(this.OnAmmGetListUtentiRuoloOperationCompleted);
            this.InvokeAsync("AmmGetListUtentiRuolo", new object[1]
      {
        (object) idRuolo
      }, this.AmmGetListUtentiRuoloOperationCompleted, userState);
        }

        private void OnAmmGetListUtentiRuoloOperationCompleted(object arg)
        {
            if (this.AmmGetListUtentiRuoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmGetListUtentiRuoloCompleted((object)this, new AmmGetListUtentiRuoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmGetListRuoliUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public OrgRuolo[] AmmGetListRuoliUO(string idUO)
        {
            return (OrgRuolo[])this.Invoke("AmmGetListRuoliUO", new object[1] { (object)idUO })[0];
        }

        public IAsyncResult BeginAmmGetListRuoliUO(string idUO, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmGetListRuoliUO", new object[1] { (object)idUO }, callback, asyncState);
        }

        public OrgRuolo[] EndAmmGetListRuoliUO(IAsyncResult asyncResult)
        {
            return (OrgRuolo[])this.EndInvoke(asyncResult)[0];
        }

        public void AmmGetListRuoliUOAsync(string idUO)
        {
            this.AmmGetListRuoliUOAsync(idUO, (object)null);
        }

        public void AmmGetListRuoliUOAsync(string idUO, object userState)
        {
            if (this.AmmGetListRuoliUOOperationCompleted == null)
                this.AmmGetListRuoliUOOperationCompleted = new SendOrPostCallback(this.OnAmmGetListRuoliUOOperationCompleted);
            this.InvokeAsync("AmmGetListRuoliUO", new object[1] { (object)idUO }, this.AmmGetListRuoliUOOperationCompleted, userState);
        }

        private void OnAmmGetListRuoliUOOperationCompleted(object arg)
        {
            if (this.AmmGetListRuoliUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmGetListRuoliUOCompleted((object)this, new AmmGetListRuoliUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmGetListUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public OrgUO[] AmmGetListUO(string idParent, string livello, string idAmm)
        {
            return (OrgUO[])this.Invoke("AmmGetListUO", new object[3]
      {
        (object) idParent,
        (object) livello,
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginAmmGetListUO(string idParent, string livello, string idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmGetListUO", new object[3]
      {
        (object) idParent,
        (object) livello,
        (object) idAmm
      }, callback, asyncState);
        }

        public OrgUO[] EndAmmGetListUO(IAsyncResult asyncResult)
        {
            return (OrgUO[])this.EndInvoke(asyncResult)[0];
        }

        public void AmmGetListUOAsync(string idParent, string livello, string idAmm)
        {
            this.AmmGetListUOAsync(idParent, livello, idAmm, (object)null);
        }

        public void AmmGetListUOAsync(string idParent, string livello, string idAmm, object userState)
        {
            if (this.AmmGetListUOOperationCompleted == null)
                this.AmmGetListUOOperationCompleted = new SendOrPostCallback(this.OnAmmGetListUOOperationCompleted);
            this.InvokeAsync("AmmGetListUO", new object[3]
      {
        (object) idParent,
        (object) livello,
        (object) idAmm
      }, this.AmmGetListUOOperationCompleted, userState);
        }

        private void OnAmmGetListUOOperationCompleted(object arg)
        {
            if (this.AmmGetListUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmGetListUOCompleted((object)this, new AmmGetListUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/addressbookGetListaCorrispondentiSemplice", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente[] addressbookGetListaCorrispondentiSemplice()
        {
            return (Corrispondente[])this.Invoke("addressbookGetListaCorrispondentiSemplice", new object[0])[0];
        }

        public IAsyncResult BeginaddressbookGetListaCorrispondentiSemplice(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("addressbookGetListaCorrispondentiSemplice", new object[0], callback, asyncState);
        }

        public Corrispondente[] EndaddressbookGetListaCorrispondentiSemplice(IAsyncResult asyncResult)
        {
            return (Corrispondente[])this.EndInvoke(asyncResult)[0];
        }

        public void addressbookGetListaCorrispondentiSempliceAsync()
        {
            this.addressbookGetListaCorrispondentiSempliceAsync((object)null);
        }

        public void addressbookGetListaCorrispondentiSempliceAsync(object userState)
        {
            if (this.addressbookGetListaCorrispondentiSempliceOperationCompleted == null)
                this.addressbookGetListaCorrispondentiSempliceOperationCompleted = new SendOrPostCallback(this.OnaddressbookGetListaCorrispondentiSempliceOperationCompleted);
            this.InvokeAsync("addressbookGetListaCorrispondentiSemplice", new object[0], this.addressbookGetListaCorrispondentiSempliceOperationCompleted, userState);
        }

        private void OnaddressbookGetListaCorrispondentiSempliceOperationCompleted(object arg)
        {
            if (this.addressbookGetListaCorrispondentiSempliceCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.addressbookGetListaCorrispondentiSempliceCompleted((object)this, new addressbookGetListaCorrispondentiSempliceCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ReportTrasmissioniUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public int ReportTrasmissioniUO(FiltroRicerca[] filtriTrasm, string UO, out FileDocumento fileDoc)
        {
            object[] objArray = this.Invoke("ReportTrasmissioniUO", new object[2]
      {
        (object) filtriTrasm,
        (object) UO
      });
            fileDoc = (FileDocumento)objArray[1];
            return (int)objArray[0];
        }

        public IAsyncResult BeginReportTrasmissioniUO(FiltroRicerca[] filtriTrasm, string UO, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ReportTrasmissioniUO", new object[2]
      {
        (object) filtriTrasm,
        (object) UO
      }, callback, asyncState);
        }

        public int EndReportTrasmissioniUO(IAsyncResult asyncResult, out FileDocumento fileDoc)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            fileDoc = (FileDocumento)objArray[1];
            return (int)objArray[0];
        }

        public void ReportTrasmissioniUOAsync(FiltroRicerca[] filtriTrasm, string UO)
        {
            this.ReportTrasmissioniUOAsync(filtriTrasm, UO, (object)null);
        }

        public void ReportTrasmissioniUOAsync(FiltroRicerca[] filtriTrasm, string UO, object userState)
        {
            if (this.ReportTrasmissioniUOOperationCompleted == null)
                this.ReportTrasmissioniUOOperationCompleted = new SendOrPostCallback(this.OnReportTrasmissioniUOOperationCompleted);
            this.InvokeAsync("ReportTrasmissioniUO", new object[2]
      {
        (object) filtriTrasm,
        (object) UO
      }, this.ReportTrasmissioniUOOperationCompleted, userState);
        }

        private void OnReportTrasmissioniUOOperationCompleted(object arg)
        {
            if (this.ReportTrasmissioniUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ReportTrasmissioniUOCompleted((object)this, new ReportTrasmissioniUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ReportTrasmissioniDocFasc", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public int ReportTrasmissioniDocFasc(TrasmissioneOggettoTrasm obj, out FileDocumento fileDoc)
        {
            object[] objArray = this.Invoke("ReportTrasmissioniDocFasc", new object[1] { (object)obj });
            fileDoc = (FileDocumento)objArray[1];
            return (int)objArray[0];
        }

        public IAsyncResult BeginReportTrasmissioniDocFasc(TrasmissioneOggettoTrasm obj, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ReportTrasmissioniDocFasc", new object[1] { (object)obj }, callback, asyncState);
        }

        public int EndReportTrasmissioniDocFasc(IAsyncResult asyncResult, out FileDocumento fileDoc)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            fileDoc = (FileDocumento)objArray[1];
            return (int)objArray[0];
        }

        public void ReportTrasmissioniDocFascAsync(TrasmissioneOggettoTrasm obj)
        {
            this.ReportTrasmissioniDocFascAsync(obj, (object)null);
        }

        public void ReportTrasmissioniDocFascAsync(TrasmissioneOggettoTrasm obj, object userState)
        {
            if (this.ReportTrasmissioniDocFascOperationCompleted == null)
                this.ReportTrasmissioniDocFascOperationCompleted = new SendOrPostCallback(this.OnReportTrasmissioniDocFascOperationCompleted);
            this.InvokeAsync("ReportTrasmissioniDocFasc", new object[1] { (object)obj }, this.ReportTrasmissioniDocFascOperationCompleted, userState);
        }

        private void OnReportTrasmissioniDocFascOperationCompleted(object arg)
        {
            if (this.ReportTrasmissioniDocFascCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ReportTrasmissioniDocFascCompleted((object)this, new ReportTrasmissioniDocFascCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ReportCorrispondenti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileDocumento ReportCorrispondenti(AddressbookQueryCorrispondente queryCorrispondente)
        {
            return (FileDocumento)this.Invoke("ReportCorrispondenti", new object[1]
      {
        (object) queryCorrispondente
      })[0];
        }

        public IAsyncResult BeginReportCorrispondenti(AddressbookQueryCorrispondente queryCorrispondente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ReportCorrispondenti", new object[1]
      {
        (object) queryCorrispondente
      }, callback, asyncState);
        }

        public FileDocumento EndReportCorrispondenti(IAsyncResult asyncResult)
        {
            return (FileDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void ReportCorrispondentiAsync(AddressbookQueryCorrispondente queryCorrispondente)
        {
            this.ReportCorrispondentiAsync(queryCorrispondente, (object)null);
        }

        public void ReportCorrispondentiAsync(AddressbookQueryCorrispondente queryCorrispondente, object userState)
        {
            if (this.ReportCorrispondentiOperationCompleted == null)
                this.ReportCorrispondentiOperationCompleted = new SendOrPostCallback(this.OnReportCorrispondentiOperationCompleted);
            this.InvokeAsync("ReportCorrispondenti", new object[1]
      {
        (object) queryCorrispondente
      }, this.ReportCorrispondentiOperationCompleted, userState);
        }

        private void OnReportCorrispondentiOperationCompleted(object arg)
        {
            if (this.ReportCorrispondentiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ReportCorrispondentiCompleted((object)this, new ReportCorrispondentiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ReportTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileDocumento ReportTitolario(string idAmministrazione, string idGruppo, string idPeople, Registro registro)
        {
            return (FileDocumento)this.Invoke("ReportTitolario", new object[4]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) registro
      })[0];
        }

        public IAsyncResult BeginReportTitolario(string idAmministrazione, string idGruppo, string idPeople, Registro registro, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ReportTitolario", new object[4]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) registro
      }, callback, asyncState);
        }

        public FileDocumento EndReportTitolario(IAsyncResult asyncResult)
        {
            return (FileDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void ReportTitolarioAsync(string idAmministrazione, string idGruppo, string idPeople, Registro registro)
        {
            this.ReportTitolarioAsync(idAmministrazione, idGruppo, idPeople, registro, (object)null);
        }

        public void ReportTitolarioAsync(string idAmministrazione, string idGruppo, string idPeople, Registro registro, object userState)
        {
            if (this.ReportTitolarioOperationCompleted == null)
                this.ReportTitolarioOperationCompleted = new SendOrPostCallback(this.OnReportTitolarioOperationCompleted);
            this.InvokeAsync("ReportTitolario", new object[4]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) registro
      }, this.ReportTitolarioOperationCompleted, userState);
        }

        private void OnReportTitolarioOperationCompleted(object arg)
        {
            if (this.ReportTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ReportTitolarioCompleted((object)this, new ReportTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ReportBusta", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileDocumento ReportBusta(SchedaDocumento schedaDoc)
        {
            return (FileDocumento)this.Invoke("ReportBusta", new object[1]
      {
        (object) schedaDoc
      })[0];
        }

        public IAsyncResult BeginReportBusta(SchedaDocumento schedaDoc, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ReportBusta", new object[1]
      {
        (object) schedaDoc
      }, callback, asyncState);
        }

        public FileDocumento EndReportBusta(IAsyncResult asyncResult)
        {
            return (FileDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void ReportBustaAsync(SchedaDocumento schedaDoc)
        {
            this.ReportBustaAsync(schedaDoc, (object)null);
        }

        public void ReportBustaAsync(SchedaDocumento schedaDoc, object userState)
        {
            if (this.ReportBustaOperationCompleted == null)
                this.ReportBustaOperationCompleted = new SendOrPostCallback(this.OnReportBustaOperationCompleted);
            this.InvokeAsync("ReportBusta", new object[1]
      {
        (object) schedaDoc
      }, this.ReportBustaOperationCompleted, userState);
        }

        private void OnReportBustaOperationCompleted(object arg)
        {
            if (this.ReportBustaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ReportBustaCompleted((object)this, new ReportBustaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ReportSchedaDoc", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileDocumento ReportSchedaDoc(string idPeople, string idGruppo, SchedaDocumento schedaDoc)
        {
            return (FileDocumento)this.Invoke("ReportSchedaDoc", new object[3]
      {
        (object) idPeople,
        (object) idGruppo,
        (object) schedaDoc
      })[0];
        }

        public IAsyncResult BeginReportSchedaDoc(string idPeople, string idGruppo, SchedaDocumento schedaDoc, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ReportSchedaDoc", new object[3]
      {
        (object) idPeople,
        (object) idGruppo,
        (object) schedaDoc
      }, callback, asyncState);
        }

        public FileDocumento EndReportSchedaDoc(IAsyncResult asyncResult)
        {
            return (FileDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void ReportSchedaDocAsync(string idPeople, string idGruppo, SchedaDocumento schedaDoc)
        {
            this.ReportSchedaDocAsync(idPeople, idGruppo, schedaDoc, (object)null);
        }

        public void ReportSchedaDocAsync(string idPeople, string idGruppo, SchedaDocumento schedaDoc, object userState)
        {
            if (this.ReportSchedaDocOperationCompleted == null)
                this.ReportSchedaDocOperationCompleted = new SendOrPostCallback(this.OnReportSchedaDocOperationCompleted);
            this.InvokeAsync("ReportSchedaDoc", new object[3]
      {
        (object) idPeople,
        (object) idGruppo,
        (object) schedaDoc
      }, this.ReportSchedaDocOperationCompleted, userState);
        }

        private void OnReportSchedaDocOperationCompleted(object arg)
        {
            if (this.ReportSchedaDocCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ReportSchedaDocCompleted((object)this, new ReportSchedaDocCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FaxInvio", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool FaxInvio(InfoUtente infoUtente, Registro registro, SchedaDocumento schedaDoc)
        {
            return (bool)this.Invoke("FaxInvio", new object[3]
      {
        (object) infoUtente,
        (object) registro,
        (object) schedaDoc
      })[0];
        }

        public IAsyncResult BeginFaxInvio(InfoUtente infoUtente, Registro registro, SchedaDocumento schedaDoc, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FaxInvio", new object[3]
      {
        (object) infoUtente,
        (object) registro,
        (object) schedaDoc
      }, callback, asyncState);
        }

        public bool EndFaxInvio(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void FaxInvioAsync(InfoUtente infoUtente, Registro registro, SchedaDocumento schedaDoc)
        {
            this.FaxInvioAsync(infoUtente, registro, schedaDoc, (object)null);
        }

        public void FaxInvioAsync(InfoUtente infoUtente, Registro registro, SchedaDocumento schedaDoc, object userState)
        {
            if (this.FaxInvioOperationCompleted == null)
                this.FaxInvioOperationCompleted = new SendOrPostCallback(this.OnFaxInvioOperationCompleted);
            this.InvokeAsync("FaxInvio", new object[3]
      {
        (object) infoUtente,
        (object) registro,
        (object) schedaDoc
      }, this.FaxInvioOperationCompleted, userState);
        }

        private void OnFaxInvioOperationCompleted(object arg)
        {
            if (this.FaxInvioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FaxInvioCompleted((object)this, new FaxInvioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FaxProcessaCasella", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public int FaxProcessaCasella(string serverName, InfoUtente infoUtente, Ruolo ruolo, Registro registro)
        {
            return (int)this.Invoke("FaxProcessaCasella", new object[4]
      {
        (object) serverName,
        (object) infoUtente,
        (object) ruolo,
        (object) registro
      })[0];
        }

        public IAsyncResult BeginFaxProcessaCasella(string serverName, InfoUtente infoUtente, Ruolo ruolo, Registro registro, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FaxProcessaCasella", new object[4]
      {
        (object) serverName,
        (object) infoUtente,
        (object) ruolo,
        (object) registro
      }, callback, asyncState);
        }

        public int EndFaxProcessaCasella(IAsyncResult asyncResult)
        {
            return (int)this.EndInvoke(asyncResult)[0];
        }

        public void FaxProcessaCasellaAsync(string serverName, InfoUtente infoUtente, Ruolo ruolo, Registro registro)
        {
            this.FaxProcessaCasellaAsync(serverName, infoUtente, ruolo, registro, (object)null);
        }

        public void FaxProcessaCasellaAsync(string serverName, InfoUtente infoUtente, Ruolo ruolo, Registro registro, object userState)
        {
            if (this.FaxProcessaCasellaOperationCompleted == null)
                this.FaxProcessaCasellaOperationCompleted = new SendOrPostCallback(this.OnFaxProcessaCasellaOperationCompleted);
            this.InvokeAsync("FaxProcessaCasella", new object[4]
      {
        (object) serverName,
        (object) infoUtente,
        (object) ruolo,
        (object) registro
      }, this.FaxProcessaCasellaOperationCompleted, userState);
        }

        private void OnFaxProcessaCasellaOperationCompleted(object arg)
        {
            if (this.FaxProcessaCasellaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FaxProcessaCasellaCompleted((object)this, new FaxProcessaCasellaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/RegistriStampaWithFilters", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public StampaRegistroResult RegistriStampaWithFilters(InfoUtente infoUtente, Ruolo ruolo, Registro registro, [XmlArrayItem(NestingLevel = 1), XmlArrayItem("ArrayOfFiltroRicerca")] FiltroRicerca[][] filters, out FileDocumento fileDoc)
        {
            object[] objArray = this.Invoke("RegistriStampaWithFilters", new object[4]
      {
        (object) infoUtente,
        (object) ruolo,
        (object) registro,
        (object) filters
      });
            fileDoc = (FileDocumento)objArray[1];
            return (StampaRegistroResult)objArray[0];
        }

        public IAsyncResult BeginRegistriStampaWithFilters(InfoUtente infoUtente, Ruolo ruolo, Registro registro, FiltroRicerca[][] filters, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RegistriStampaWithFilters", new object[4]
      {
        (object) infoUtente,
        (object) ruolo,
        (object) registro,
        (object) filters
      }, callback, asyncState);
        }

        public StampaRegistroResult EndRegistriStampaWithFilters(IAsyncResult asyncResult, out FileDocumento fileDoc)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            fileDoc = (FileDocumento)objArray[1];
            return (StampaRegistroResult)objArray[0];
        }

        public void RegistriStampaWithFiltersAsync(InfoUtente infoUtente, Ruolo ruolo, Registro registro, FiltroRicerca[][] filters)
        {
            this.RegistriStampaWithFiltersAsync(infoUtente, ruolo, registro, filters, (object)null);
        }

        public void RegistriStampaWithFiltersAsync(InfoUtente infoUtente, Ruolo ruolo, Registro registro, FiltroRicerca[][] filters, object userState)
        {
            if (this.RegistriStampaWithFiltersOperationCompleted == null)
                this.RegistriStampaWithFiltersOperationCompleted = new SendOrPostCallback(this.OnRegistriStampaWithFiltersOperationCompleted);
            this.InvokeAsync("RegistriStampaWithFilters", new object[4]
      {
        (object) infoUtente,
        (object) ruolo,
        (object) registro,
        (object) filters
      }, this.RegistriStampaWithFiltersOperationCompleted, userState);
        }

        private void OnRegistriStampaWithFiltersOperationCompleted(object arg)
        {
            if (this.RegistriStampaWithFiltersCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.RegistriStampaWithFiltersCompleted((object)this, new RegistriStampaWithFiltersCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/RegistriStampa", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public StampaRegistroResult RegistriStampa(InfoUtente infoUtente, Ruolo ruolo, Registro registro)
        {
            return (StampaRegistroResult)this.Invoke("RegistriStampa", new object[3]
      {
        (object) infoUtente,
        (object) ruolo,
        (object) registro
      })[0];
        }

        public IAsyncResult BeginRegistriStampa(InfoUtente infoUtente, Ruolo ruolo, Registro registro, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RegistriStampa", new object[3]
      {
        (object) infoUtente,
        (object) ruolo,
        (object) registro
      }, callback, asyncState);
        }

        public StampaRegistroResult EndRegistriStampa(IAsyncResult asyncResult)
        {
            return (StampaRegistroResult)this.EndInvoke(asyncResult)[0];
        }

        public void RegistriStampaAsync(InfoUtente infoUtente, Ruolo ruolo, Registro registro)
        {
            this.RegistriStampaAsync(infoUtente, ruolo, registro, (object)null);
        }

        public void RegistriStampaAsync(InfoUtente infoUtente, Ruolo ruolo, Registro registro, object userState)
        {
            if (this.RegistriStampaOperationCompleted == null)
                this.RegistriStampaOperationCompleted = new SendOrPostCallback(this.OnRegistriStampaOperationCompleted);
            this.InvokeAsync("RegistriStampa", new object[3]
      {
        (object) infoUtente,
        (object) ruolo,
        (object) registro
      }, this.RegistriStampaOperationCompleted, userState);
        }

        private void OnRegistriStampaOperationCompleted(object arg)
        {
            if (this.RegistriStampaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.RegistriStampaCompleted((object)this, new RegistriStampaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/InteroperabilitaAggiornamentoConferma", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public ProtocolloDestinatario[] InteroperabilitaAggiornamentoConferma(string idProfile, Corrispondente corrispondente)
        {
            return (ProtocolloDestinatario[])this.Invoke("InteroperabilitaAggiornamentoConferma", new object[2]
      {
        (object) idProfile,
        (object) corrispondente
      })[0];
        }

        public IAsyncResult BeginInteroperabilitaAggiornamentoConferma(string idProfile, Corrispondente corrispondente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("InteroperabilitaAggiornamentoConferma", new object[2]
      {
        (object) idProfile,
        (object) corrispondente
      }, callback, asyncState);
        }

        public ProtocolloDestinatario[] EndInteroperabilitaAggiornamentoConferma(IAsyncResult asyncResult)
        {
            return (ProtocolloDestinatario[])this.EndInvoke(asyncResult)[0];
        }

        public void InteroperabilitaAggiornamentoConfermaAsync(string idProfile, Corrispondente corrispondente)
        {
            this.InteroperabilitaAggiornamentoConfermaAsync(idProfile, corrispondente, (object)null);
        }

        public void InteroperabilitaAggiornamentoConfermaAsync(string idProfile, Corrispondente corrispondente, object userState)
        {
            if (this.InteroperabilitaAggiornamentoConfermaOperationCompleted == null)
                this.InteroperabilitaAggiornamentoConfermaOperationCompleted = new SendOrPostCallback(this.OnInteroperabilitaAggiornamentoConfermaOperationCompleted);
            this.InvokeAsync("InteroperabilitaAggiornamentoConferma", new object[2]
      {
        (object) idProfile,
        (object) corrispondente
      }, this.InteroperabilitaAggiornamentoConfermaOperationCompleted, userState);
        }

        private void OnInteroperabilitaAggiornamentoConfermaOperationCompleted(object arg)
        {
            if (this.InteroperabilitaAggiornamentoConfermaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.InteroperabilitaAggiornamentoConfermaCompleted((object)this, new InteroperabilitaAggiornamentoConfermaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/InteroperabilitaRicezione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool InteroperabilitaRicezione(string serverName, Registro reg, Utente ut)
        {
            return (bool)this.Invoke("InteroperabilitaRicezione", new object[3]
      {
        (object) serverName,
        (object) reg,
        (object) ut
      })[0];
        }

        public IAsyncResult BeginInteroperabilitaRicezione(string serverName, Registro reg, Utente ut, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("InteroperabilitaRicezione", new object[3]
      {
        (object) serverName,
        (object) reg,
        (object) ut
      }, callback, asyncState);
        }

        public bool EndInteroperabilitaRicezione(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void InteroperabilitaRicezioneAsync(string serverName, Registro reg, Utente ut)
        {
            this.InteroperabilitaRicezioneAsync(serverName, reg, ut, (object)null);
        }

        public void InteroperabilitaRicezioneAsync(string serverName, Registro reg, Utente ut, object userState)
        {
            if (this.InteroperabilitaRicezioneOperationCompleted == null)
                this.InteroperabilitaRicezioneOperationCompleted = new SendOrPostCallback(this.OnInteroperabilitaRicezioneOperationCompleted);
            this.InvokeAsync("InteroperabilitaRicezione", new object[3]
      {
        (object) serverName,
        (object) reg,
        (object) ut
      }, this.InteroperabilitaRicezioneOperationCompleted, userState);
        }

        private void OnInteroperabilitaRicezioneOperationCompleted(object arg)
        {
            if (this.InteroperabilitaRicezioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.InteroperabilitaRicezioneCompleted((object)this, new InteroperabilitaRicezioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookGetCanali", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Canale[] AddressbookGetCanali()
        {
            return (Canale[])this.Invoke("AddressbookGetCanali", new object[0])[0];
        }

        public IAsyncResult BeginAddressbookGetCanali(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookGetCanali", new object[0], callback, asyncState);
        }

        public Canale[] EndAddressbookGetCanali(IAsyncResult asyncResult)
        {
            return (Canale[])this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookGetCanaliAsync()
        {
            this.AddressbookGetCanaliAsync((object)null);
        }

        public void AddressbookGetCanaliAsync(object userState)
        {
            if (this.AddressbookGetCanaliOperationCompleted == null)
                this.AddressbookGetCanaliOperationCompleted = new SendOrPostCallback(this.OnAddressbookGetCanaliOperationCompleted);
            this.InvokeAsync("AddressbookGetCanali", new object[0], this.AddressbookGetCanaliOperationCompleted, userState);
        }

        private void OnAddressbookGetCanaliOperationCompleted(object arg)
        {
            if (this.AddressbookGetCanaliCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookGetCanaliCompleted((object)this, new AddressbookGetCanaliCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookGetRuoliRiferimentoAutorizzati", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Ruolo[] AddressbookGetRuoliRiferimentoAutorizzati(AddressbookQueryCorrispondenteAutorizzato qca, UnitaOrganizzativa uo)
        {
            return (Ruolo[])this.Invoke("AddressbookGetRuoliRiferimentoAutorizzati", new object[2]
      {
        (object) qca,
        (object) uo
      })[0];
        }

        public IAsyncResult BeginAddressbookGetRuoliRiferimentoAutorizzati(AddressbookQueryCorrispondenteAutorizzato qca, UnitaOrganizzativa uo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookGetRuoliRiferimentoAutorizzati", new object[2]
      {
        (object) qca,
        (object) uo
      }, callback, asyncState);
        }

        public Ruolo[] EndAddressbookGetRuoliRiferimentoAutorizzati(IAsyncResult asyncResult)
        {
            return (Ruolo[])this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookGetRuoliRiferimentoAutorizzatiAsync(AddressbookQueryCorrispondenteAutorizzato qca, UnitaOrganizzativa uo)
        {
            this.AddressbookGetRuoliRiferimentoAutorizzatiAsync(qca, uo, (object)null);
        }

        public void AddressbookGetRuoliRiferimentoAutorizzatiAsync(AddressbookQueryCorrispondenteAutorizzato qca, UnitaOrganizzativa uo, object userState)
        {
            if (this.AddressbookGetRuoliRiferimentoAutorizzatiOperationCompleted == null)
                this.AddressbookGetRuoliRiferimentoAutorizzatiOperationCompleted = new SendOrPostCallback(this.OnAddressbookGetRuoliRiferimentoAutorizzatiOperationCompleted);
            this.InvokeAsync("AddressbookGetRuoliRiferimentoAutorizzati", new object[2]
      {
        (object) qca,
        (object) uo
      }, this.AddressbookGetRuoliRiferimentoAutorizzatiOperationCompleted, userState);
        }

        private void OnAddressbookGetRuoliRiferimentoAutorizzatiOperationCompleted(object arg)
        {
            if (this.AddressbookGetRuoliRiferimentoAutorizzatiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookGetRuoliRiferimentoAutorizzatiCompleted((object)this, new AddressbookGetRuoliRiferimentoAutorizzatiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookGetRuoliSuperioriInUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Ruolo[] AddressbookGetRuoliSuperioriInUO(Ruolo ruolo)
        {
            return (Ruolo[])this.Invoke("AddressbookGetRuoliSuperioriInUO", new object[1]
      {
        (object) ruolo
      })[0];
        }

        public IAsyncResult BeginAddressbookGetRuoliSuperioriInUO(Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookGetRuoliSuperioriInUO", new object[1]
      {
        (object) ruolo
      }, callback, asyncState);
        }

        public Ruolo[] EndAddressbookGetRuoliSuperioriInUO(IAsyncResult asyncResult)
        {
            return (Ruolo[])this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookGetRuoliSuperioriInUOAsync(Ruolo ruolo)
        {
            this.AddressbookGetRuoliSuperioriInUOAsync(ruolo, (object)null);
        }

        public void AddressbookGetRuoliSuperioriInUOAsync(Ruolo ruolo, object userState)
        {
            if (this.AddressbookGetRuoliSuperioriInUOOperationCompleted == null)
                this.AddressbookGetRuoliSuperioriInUOOperationCompleted = new SendOrPostCallback(this.OnAddressbookGetRuoliSuperioriInUOOperationCompleted);
            this.InvokeAsync("AddressbookGetRuoliSuperioriInUO", new object[1]
      {
        (object) ruolo
      }, this.AddressbookGetRuoliSuperioriInUOOperationCompleted, userState);
        }

        private void OnAddressbookGetRuoliSuperioriInUOOperationCompleted(object arg)
        {
            if (this.AddressbookGetRuoliSuperioriInUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookGetRuoliSuperioriInUOCompleted((object)this, new AddressbookGetRuoliSuperioriInUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookInsertCorrispondente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente AddressbookInsertCorrispondente(Corrispondente corrispondente, Corrispondente parent)
        {
            return (Corrispondente)this.Invoke("AddressbookInsertCorrispondente", new object[2]
      {
        (object) corrispondente,
        (object) parent
      })[0];
        }

        public IAsyncResult BeginAddressbookInsertCorrispondente(Corrispondente corrispondente, Corrispondente parent, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookInsertCorrispondente", new object[2]
      {
        (object) corrispondente,
        (object) parent
      }, callback, asyncState);
        }

        public Corrispondente EndAddressbookInsertCorrispondente(IAsyncResult asyncResult)
        {
            return (Corrispondente)this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookInsertCorrispondenteAsync(Corrispondente corrispondente, Corrispondente parent)
        {
            this.AddressbookInsertCorrispondenteAsync(corrispondente, parent, (object)null);
        }

        public void AddressbookInsertCorrispondenteAsync(Corrispondente corrispondente, Corrispondente parent, object userState)
        {
            if (this.AddressbookInsertCorrispondenteOperationCompleted == null)
                this.AddressbookInsertCorrispondenteOperationCompleted = new SendOrPostCallback(this.OnAddressbookInsertCorrispondenteOperationCompleted);
            this.InvokeAsync("AddressbookInsertCorrispondente", new object[2]
      {
        (object) corrispondente,
        (object) parent
      }, this.AddressbookInsertCorrispondenteOperationCompleted, userState);
        }

        private void OnAddressbookInsertCorrispondenteOperationCompleted(object arg)
        {
            if (this.AddressbookInsertCorrispondenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookInsertCorrispondenteCompleted((object)this, new AddressbookInsertCorrispondenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookGetRootUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public UnitaOrganizzativa[] AddressbookGetRootUO(AddressbookQueryCorrispondente queryCorrispondente)
        {
            return (UnitaOrganizzativa[])this.Invoke("AddressbookGetRootUO", new object[1]
      {
        (object) queryCorrispondente
      })[0];
        }

        public IAsyncResult BeginAddressbookGetRootUO(AddressbookQueryCorrispondente queryCorrispondente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookGetRootUO", new object[1]
      {
        (object) queryCorrispondente
      }, callback, asyncState);
        }

        public UnitaOrganizzativa[] EndAddressbookGetRootUO(IAsyncResult asyncResult)
        {
            return (UnitaOrganizzativa[])this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookGetRootUOAsync(AddressbookQueryCorrispondente queryCorrispondente)
        {
            this.AddressbookGetRootUOAsync(queryCorrispondente, (object)null);
        }

        public void AddressbookGetRootUOAsync(AddressbookQueryCorrispondente queryCorrispondente, object userState)
        {
            if (this.AddressbookGetRootUOOperationCompleted == null)
                this.AddressbookGetRootUOOperationCompleted = new SendOrPostCallback(this.OnAddressbookGetRootUOOperationCompleted);
            this.InvokeAsync("AddressbookGetRootUO", new object[1]
      {
        (object) queryCorrispondente
      }, this.AddressbookGetRootUOOperationCompleted, userState);
        }

        private void OnAddressbookGetRootUOOperationCompleted(object arg)
        {
            if (this.AddressbookGetRootUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookGetRootUOCompleted((object)this, new AddressbookGetRootUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookGetListaCorrispondentiAutorizzati", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente[] AddressbookGetListaCorrispondentiAutorizzati(AddressbookQueryCorrispondenteAutorizzato queryCorrispondenteAutorizzato)
        {
            return (Corrispondente[])this.Invoke("AddressbookGetListaCorrispondentiAutorizzati", new object[1]
      {
        (object) queryCorrispondenteAutorizzato
      })[0];
        }

        public IAsyncResult BeginAddressbookGetListaCorrispondentiAutorizzati(AddressbookQueryCorrispondenteAutorizzato queryCorrispondenteAutorizzato, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookGetListaCorrispondentiAutorizzati", new object[1]
      {
        (object) queryCorrispondenteAutorizzato
      }, callback, asyncState);
        }

        public Corrispondente[] EndAddressbookGetListaCorrispondentiAutorizzati(IAsyncResult asyncResult)
        {
            return (Corrispondente[])this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookGetListaCorrispondentiAutorizzatiAsync(AddressbookQueryCorrispondenteAutorizzato queryCorrispondenteAutorizzato)
        {
            this.AddressbookGetListaCorrispondentiAutorizzatiAsync(queryCorrispondenteAutorizzato, (object)null);
        }

        public void AddressbookGetListaCorrispondentiAutorizzatiAsync(AddressbookQueryCorrispondenteAutorizzato queryCorrispondenteAutorizzato, object userState)
        {
            if (this.AddressbookGetListaCorrispondentiAutorizzatiOperationCompleted == null)
                this.AddressbookGetListaCorrispondentiAutorizzatiOperationCompleted = new SendOrPostCallback(this.OnAddressbookGetListaCorrispondentiAutorizzatiOperationCompleted);
            this.InvokeAsync("AddressbookGetListaCorrispondentiAutorizzati", new object[1]
      {
        (object) queryCorrispondenteAutorizzato
      }, this.AddressbookGetListaCorrispondentiAutorizzatiOperationCompleted, userState);
        }

        private void OnAddressbookGetListaCorrispondentiAutorizzatiOperationCompleted(object arg)
        {
            if (this.AddressbookGetListaCorrispondentiAutorizzatiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookGetListaCorrispondentiAutorizzatiCompleted((object)this, new AddressbookGetListaCorrispondentiAutorizzatiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookGetDettagliCorrispondente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet AddressbookGetDettagliCorrispondente(AddressbookQueryCorrispondente queryCorrispondente)
        {
            return (DataSet)this.Invoke("AddressbookGetDettagliCorrispondente", new object[1]
      {
        (object) queryCorrispondente
      })[0];
        }

        public IAsyncResult BeginAddressbookGetDettagliCorrispondente(AddressbookQueryCorrispondente queryCorrispondente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookGetDettagliCorrispondente", new object[1]
      {
        (object) queryCorrispondente
      }, callback, asyncState);
        }

        public DataSet EndAddressbookGetDettagliCorrispondente(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookGetDettagliCorrispondenteAsync(AddressbookQueryCorrispondente queryCorrispondente)
        {
            this.AddressbookGetDettagliCorrispondenteAsync(queryCorrispondente, (object)null);
        }

        public void AddressbookGetDettagliCorrispondenteAsync(AddressbookQueryCorrispondente queryCorrispondente, object userState)
        {
            if (this.AddressbookGetDettagliCorrispondenteOperationCompleted == null)
                this.AddressbookGetDettagliCorrispondenteOperationCompleted = new SendOrPostCallback(this.OnAddressbookGetDettagliCorrispondenteOperationCompleted);
            this.InvokeAsync("AddressbookGetDettagliCorrispondente", new object[1]
      {
        (object) queryCorrispondente
      }, this.AddressbookGetDettagliCorrispondenteOperationCompleted, userState);
        }

        private void OnAddressbookGetDettagliCorrispondenteOperationCompleted(object arg)
        {
            if (this.AddressbookGetDettagliCorrispondenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookGetDettagliCorrispondenteCompleted((object)this, new AddressbookGetDettagliCorrispondenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookGetListaCorrispondenti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente[] AddressbookGetListaCorrispondenti(AddressbookQueryCorrispondente queryCorrispondente)
        {
            return (Corrispondente[])this.Invoke("AddressbookGetListaCorrispondenti", new object[1]
      {
        (object) queryCorrispondente
      })[0];
        }

        public IAsyncResult BeginAddressbookGetListaCorrispondenti(AddressbookQueryCorrispondente queryCorrispondente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookGetListaCorrispondenti", new object[1]
      {
        (object) queryCorrispondente
      }, callback, asyncState);
        }

        public Corrispondente[] EndAddressbookGetListaCorrispondenti(IAsyncResult asyncResult)
        {
            return (Corrispondente[])this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookGetListaCorrispondentiAsync(AddressbookQueryCorrispondente queryCorrispondente)
        {
            this.AddressbookGetListaCorrispondentiAsync(queryCorrispondente, (object)null);
        }

        public void AddressbookGetListaCorrispondentiAsync(AddressbookQueryCorrispondente queryCorrispondente, object userState)
        {
            if (this.AddressbookGetListaCorrispondentiOperationCompleted == null)
                this.AddressbookGetListaCorrispondentiOperationCompleted = new SendOrPostCallback(this.OnAddressbookGetListaCorrispondentiOperationCompleted);
            this.InvokeAsync("AddressbookGetListaCorrispondenti", new object[1]
      {
        (object) queryCorrispondente
      }, this.AddressbookGetListaCorrispondentiOperationCompleted, userState);
        }

        private void OnAddressbookGetListaCorrispondentiOperationCompleted(object arg)
        {
            if (this.AddressbookGetListaCorrispondentiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookGetListaCorrispondentiCompleted((object)this, new AddressbookGetListaCorrispondentiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AddressbookGetListaCorrispondenti_Aut", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente[] AddressbookGetListaCorrispondenti_Aut(AddressbookQueryCorrispondente queryCorrispondente)
        {
            return (Corrispondente[])this.Invoke("AddressbookGetListaCorrispondenti_Aut", new object[1]
      {
        (object) queryCorrispondente
      })[0];
        }

        public IAsyncResult BeginAddressbookGetListaCorrispondenti_Aut(AddressbookQueryCorrispondente queryCorrispondente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AddressbookGetListaCorrispondenti_Aut", new object[1]
      {
        (object) queryCorrispondente
      }, callback, asyncState);
        }

        public Corrispondente[] EndAddressbookGetListaCorrispondenti_Aut(IAsyncResult asyncResult)
        {
            return (Corrispondente[])this.EndInvoke(asyncResult)[0];
        }

        public void AddressbookGetListaCorrispondenti_AutAsync(AddressbookQueryCorrispondente queryCorrispondente)
        {
            this.AddressbookGetListaCorrispondenti_AutAsync(queryCorrispondente, (object)null);
        }

        public void AddressbookGetListaCorrispondenti_AutAsync(AddressbookQueryCorrispondente queryCorrispondente, object userState)
        {
            if (this.AddressbookGetListaCorrispondenti_AutOperationCompleted == null)
                this.AddressbookGetListaCorrispondenti_AutOperationCompleted = new SendOrPostCallback(this.OnAddressbookGetListaCorrispondenti_AutOperationCompleted);
            this.InvokeAsync("AddressbookGetListaCorrispondenti_Aut", new object[1]
      {
        (object) queryCorrispondente
      }, this.AddressbookGetListaCorrispondenti_AutOperationCompleted, userState);
        }

        private void OnAddressbookGetListaCorrispondenti_AutOperationCompleted(object arg)
        {
            if (this.AddressbookGetListaCorrispondenti_AutCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AddressbookGetListaCorrispondenti_AutCompleted((object)this, new AddressbookGetListaCorrispondenti_AutCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioniDeleteTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool TrasmissioniDeleteTemplate(TemplateTrasmissione template)
        {
            return (bool)this.Invoke("TrasmissioniDeleteTemplate", new object[1]
      {
        (object) template
      })[0];
        }

        public IAsyncResult BeginTrasmissioniDeleteTemplate(TemplateTrasmissione template, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioniDeleteTemplate", new object[1]
      {
        (object) template
      }, callback, asyncState);
        }

        public bool EndTrasmissioniDeleteTemplate(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioniDeleteTemplateAsync(TemplateTrasmissione template)
        {
            this.TrasmissioniDeleteTemplateAsync(template, (object)null);
        }

        public void TrasmissioniDeleteTemplateAsync(TemplateTrasmissione template, object userState)
        {
            if (this.TrasmissioniDeleteTemplateOperationCompleted == null)
                this.TrasmissioniDeleteTemplateOperationCompleted = new SendOrPostCallback(this.OnTrasmissioniDeleteTemplateOperationCompleted);
            this.InvokeAsync("TrasmissioniDeleteTemplate", new object[1]
      {
        (object) template
      }, this.TrasmissioniDeleteTemplateOperationCompleted, userState);
        }

        private void OnTrasmissioniDeleteTemplateOperationCompleted(object arg)
        {
            if (this.TrasmissioniDeleteTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioniDeleteTemplateCompleted((object)this, new TrasmissioniDeleteTemplateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioniUpdateTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool TrasmissioniUpdateTemplate(TemplateTrasmissione template)
        {
            return (bool)this.Invoke("TrasmissioniUpdateTemplate", new object[1]
      {
        (object) template
      })[0];
        }

        public IAsyncResult BeginTrasmissioniUpdateTemplate(TemplateTrasmissione template, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioniUpdateTemplate", new object[1]
      {
        (object) template
      }, callback, asyncState);
        }

        public bool EndTrasmissioniUpdateTemplate(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioniUpdateTemplateAsync(TemplateTrasmissione template)
        {
            this.TrasmissioniUpdateTemplateAsync(template, (object)null);
        }

        public void TrasmissioniUpdateTemplateAsync(TemplateTrasmissione template, object userState)
        {
            if (this.TrasmissioniUpdateTemplateOperationCompleted == null)
                this.TrasmissioniUpdateTemplateOperationCompleted = new SendOrPostCallback(this.OnTrasmissioniUpdateTemplateOperationCompleted);
            this.InvokeAsync("TrasmissioniUpdateTemplate", new object[1]
      {
        (object) template
      }, this.TrasmissioniUpdateTemplateOperationCompleted, userState);
        }

        private void OnTrasmissioniUpdateTemplateOperationCompleted(object arg)
        {
            if (this.TrasmissioniUpdateTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioniUpdateTemplateCompleted((object)this, new TrasmissioniUpdateTemplateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneGetListaTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TemplateTrasmissione[] TrasmissioneGetListaTemplate(string idPeople, string idCorrGlobali, string tipo)
        {
            return (TemplateTrasmissione[])this.Invoke("TrasmissioneGetListaTemplate", new object[3]
      {
        (object) idPeople,
        (object) idCorrGlobali,
        (object) tipo
      })[0];
        }

        public IAsyncResult BeginTrasmissioneGetListaTemplate(string idPeople, string idCorrGlobali, string tipo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneGetListaTemplate", new object[3]
      {
        (object) idPeople,
        (object) idCorrGlobali,
        (object) tipo
      }, callback, asyncState);
        }

        public TemplateTrasmissione[] EndTrasmissioneGetListaTemplate(IAsyncResult asyncResult)
        {
            return (TemplateTrasmissione[])this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioneGetListaTemplateAsync(string idPeople, string idCorrGlobali, string tipo)
        {
            this.TrasmissioneGetListaTemplateAsync(idPeople, idCorrGlobali, tipo, (object)null);
        }

        public void TrasmissioneGetListaTemplateAsync(string idPeople, string idCorrGlobali, string tipo, object userState)
        {
            if (this.TrasmissioneGetListaTemplateOperationCompleted == null)
                this.TrasmissioneGetListaTemplateOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneGetListaTemplateOperationCompleted);
            this.InvokeAsync("TrasmissioneGetListaTemplate", new object[3]
      {
        (object) idPeople,
        (object) idCorrGlobali,
        (object) tipo
      }, this.TrasmissioneGetListaTemplateOperationCompleted, userState);
        }

        private void OnTrasmissioneGetListaTemplateOperationCompleted(object arg)
        {
            if (this.TrasmissioneGetListaTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneGetListaTemplateCompleted((object)this, new TrasmissioneGetListaTemplateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneAddTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TemplateTrasmissione TrasmissioneAddTemplate(TemplateTrasmissione template)
        {
            return (TemplateTrasmissione)this.Invoke("TrasmissioneAddTemplate", new object[1]
      {
        (object) template
      })[0];
        }

        public IAsyncResult BeginTrasmissioneAddTemplate(TemplateTrasmissione template, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneAddTemplate", new object[1]
      {
        (object) template
      }, callback, asyncState);
        }

        public TemplateTrasmissione EndTrasmissioneAddTemplate(IAsyncResult asyncResult)
        {
            return (TemplateTrasmissione)this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioneAddTemplateAsync(TemplateTrasmissione template)
        {
            this.TrasmissioneAddTemplateAsync(template, (object)null);
        }

        public void TrasmissioneAddTemplateAsync(TemplateTrasmissione template, object userState)
        {
            if (this.TrasmissioneAddTemplateOperationCompleted == null)
                this.TrasmissioneAddTemplateOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneAddTemplateOperationCompleted);
            this.InvokeAsync("TrasmissioneAddTemplate", new object[1]
      {
        (object) template
      }, this.TrasmissioneAddTemplateOperationCompleted, userState);
        }

        private void OnTrasmissioneAddTemplateOperationCompleted(object arg)
        {
            if (this.TrasmissioneAddTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneAddTemplateCompleted((object)this, new TrasmissioneAddTemplateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneGetInRispostaA", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione TrasmissioneGetInRispostaA(TrasmissioneSingola trasmSingola)
        {
            return (Trasmissione)this.Invoke("TrasmissioneGetInRispostaA", new object[1]
      {
        (object) trasmSingola
      })[0];
        }

        public IAsyncResult BeginTrasmissioneGetInRispostaA(TrasmissioneSingola trasmSingola, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneGetInRispostaA", new object[1]
      {
        (object) trasmSingola
      }, callback, asyncState);
        }

        public Trasmissione EndTrasmissioneGetInRispostaA(IAsyncResult asyncResult)
        {
            return (Trasmissione)this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioneGetInRispostaAAsync(TrasmissioneSingola trasmSingola)
        {
            this.TrasmissioneGetInRispostaAAsync(trasmSingola, (object)null);
        }

        public void TrasmissioneGetInRispostaAAsync(TrasmissioneSingola trasmSingola, object userState)
        {
            if (this.TrasmissioneGetInRispostaAOperationCompleted == null)
                this.TrasmissioneGetInRispostaAOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneGetInRispostaAOperationCompleted);
            this.InvokeAsync("TrasmissioneGetInRispostaA", new object[1]
      {
        (object) trasmSingola
      }, this.TrasmissioneGetInRispostaAOperationCompleted, userState);
        }

        private void OnTrasmissioneGetInRispostaAOperationCompleted(object arg)
        {
            if (this.TrasmissioneGetInRispostaACompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneGetInRispostaACompleted((object)this, new TrasmissioneGetInRispostaACompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneGetRisposta", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione TrasmissioneGetRisposta(TrasmissioneUtente trasmUtente)
        {
            return (Trasmissione)this.Invoke("TrasmissioneGetRisposta", new object[1]
      {
        (object) trasmUtente
      })[0];
        }

        public IAsyncResult BeginTrasmissioneGetRisposta(TrasmissioneUtente trasmUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneGetRisposta", new object[1]
      {
        (object) trasmUtente
      }, callback, asyncState);
        }

        public Trasmissione EndTrasmissioneGetRisposta(IAsyncResult asyncResult)
        {
            return (Trasmissione)this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioneGetRispostaAsync(TrasmissioneUtente trasmUtente)
        {
            this.TrasmissioneGetRispostaAsync(trasmUtente, (object)null);
        }

        public void TrasmissioneGetRispostaAsync(TrasmissioneUtente trasmUtente, object userState)
        {
            if (this.TrasmissioneGetRispostaOperationCompleted == null)
                this.TrasmissioneGetRispostaOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneGetRispostaOperationCompleted);
            this.InvokeAsync("TrasmissioneGetRisposta", new object[1]
      {
        (object) trasmUtente
      }, this.TrasmissioneGetRispostaOperationCompleted, userState);
        }

        private void OnTrasmissioneGetRispostaOperationCompleted(object arg)
        {
            if (this.TrasmissioneGetRispostaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneGetRispostaCompleted((object)this, new TrasmissioneGetRispostaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneExecuteAccRif", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool TrasmissioneExecuteAccRif(TrasmissioneUtente trasmissioneUtente)
        {
            return (bool)this.Invoke("TrasmissioneExecuteAccRif", new object[1]
      {
        (object) trasmissioneUtente
      })[0];
        }

        public IAsyncResult BeginTrasmissioneExecuteAccRif(TrasmissioneUtente trasmissioneUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneExecuteAccRif", new object[1]
      {
        (object) trasmissioneUtente
      }, callback, asyncState);
        }

        public bool EndTrasmissioneExecuteAccRif(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioneExecuteAccRifAsync(TrasmissioneUtente trasmissioneUtente)
        {
            this.TrasmissioneExecuteAccRifAsync(trasmissioneUtente, (object)null);
        }

        public void TrasmissioneExecuteAccRifAsync(TrasmissioneUtente trasmissioneUtente, object userState)
        {
            if (this.TrasmissioneExecuteAccRifOperationCompleted == null)
                this.TrasmissioneExecuteAccRifOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneExecuteAccRifOperationCompleted);
            this.InvokeAsync("TrasmissioneExecuteAccRif", new object[1]
      {
        (object) trasmissioneUtente
      }, this.TrasmissioneExecuteAccRifOperationCompleted, userState);
        }

        private void OnTrasmissioneExecuteAccRifOperationCompleted(object arg)
        {
            if (this.TrasmissioneExecuteAccRifCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneExecuteAccRifCompleted((object)this, new TrasmissioneExecuteAccRifCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneGetRagioni", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public RagioneTrasmissione[] TrasmissioneGetRagioni(TrasmissioneDiritti diritti)
        {
            return (RagioneTrasmissione[])this.Invoke("TrasmissioneGetRagioni", new object[1]
      {
        (object) diritti
      })[0];
        }

        public IAsyncResult BeginTrasmissioneGetRagioni(TrasmissioneDiritti diritti, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneGetRagioni", new object[1]
      {
        (object) diritti
      }, callback, asyncState);
        }

        public RagioneTrasmissione[] EndTrasmissioneGetRagioni(IAsyncResult asyncResult)
        {
            return (RagioneTrasmissione[])this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioneGetRagioniAsync(TrasmissioneDiritti diritti)
        {
            this.TrasmissioneGetRagioniAsync(diritti, (object)null);
        }

        public void TrasmissioneGetRagioniAsync(TrasmissioneDiritti diritti, object userState)
        {
            if (this.TrasmissioneGetRagioniOperationCompleted == null)
                this.TrasmissioneGetRagioniOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneGetRagioniOperationCompleted);
            this.InvokeAsync("TrasmissioneGetRagioni", new object[1]
      {
        (object) diritti
      }, this.TrasmissioneGetRagioniOperationCompleted, userState);
        }

        private void OnTrasmissioneGetRagioniOperationCompleted(object arg)
        {
            if (this.TrasmissioneGetRagioniCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneGetRagioniCompleted((object)this, new TrasmissioneGetRagioniCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneGetQueryEffettuatePaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione[] TrasmissioneGetQueryEffettuatePaging(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber, out int totalPageNumber, out int recordCount)
        {
            object[] objArray = this.Invoke("TrasmissioneGetQueryEffettuatePaging", new object[5]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) pageNumber
      });
            totalPageNumber = (int)objArray[1];
            recordCount = (int)objArray[2];
            return (Trasmissione[])objArray[0];
        }

        public IAsyncResult BeginTrasmissioneGetQueryEffettuatePaging(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneGetQueryEffettuatePaging", new object[5]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) pageNumber
      }, callback, asyncState);
        }

        public Trasmissione[] EndTrasmissioneGetQueryEffettuatePaging(IAsyncResult asyncResult, out int totalPageNumber, out int recordCount)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            totalPageNumber = (int)objArray[1];
            recordCount = (int)objArray[2];
            return (Trasmissione[])objArray[0];
        }

        public void TrasmissioneGetQueryEffettuatePagingAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber)
        {
            this.TrasmissioneGetQueryEffettuatePagingAsync(oggettoTrasmesso, listaFiltri, utente, ruolo, pageNumber, (object)null);
        }

        public void TrasmissioneGetQueryEffettuatePagingAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber, object userState)
        {
            if (this.TrasmissioneGetQueryEffettuatePagingOperationCompleted == null)
                this.TrasmissioneGetQueryEffettuatePagingOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneGetQueryEffettuatePagingOperationCompleted);
            this.InvokeAsync("TrasmissioneGetQueryEffettuatePaging", new object[5]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) pageNumber
      }, this.TrasmissioneGetQueryEffettuatePagingOperationCompleted, userState);
        }

        private void OnTrasmissioneGetQueryEffettuatePagingOperationCompleted(object arg)
        {
            if (this.TrasmissioneGetQueryEffettuatePagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneGetQueryEffettuatePagingCompleted((object)this, new TrasmissioneGetQueryEffettuatePagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneGetQueryEffettuateDocPaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione[] TrasmissioneGetQueryEffettuateDocPaging(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber, out int totalPageNumber, out int recordCount)
        {
            object[] objArray = this.Invoke("TrasmissioneGetQueryEffettuateDocPaging", new object[5]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) pageNumber
      });
            totalPageNumber = (int)objArray[1];
            recordCount = (int)objArray[2];
            return (Trasmissione[])objArray[0];
        }

        public IAsyncResult BeginTrasmissioneGetQueryEffettuateDocPaging(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneGetQueryEffettuateDocPaging", new object[5]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) pageNumber
      }, callback, asyncState);
        }

        public Trasmissione[] EndTrasmissioneGetQueryEffettuateDocPaging(IAsyncResult asyncResult, out int totalPageNumber, out int recordCount)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            totalPageNumber = (int)objArray[1];
            recordCount = (int)objArray[2];
            return (Trasmissione[])objArray[0];
        }

        public void TrasmissioneGetQueryEffettuateDocPagingAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber)
        {
            this.TrasmissioneGetQueryEffettuateDocPagingAsync(oggettoTrasmesso, listaFiltri, utente, ruolo, pageNumber, (object)null);
        }

        public void TrasmissioneGetQueryEffettuateDocPagingAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber, object userState)
        {
            if (this.TrasmissioneGetQueryEffettuateDocPagingOperationCompleted == null)
                this.TrasmissioneGetQueryEffettuateDocPagingOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneGetQueryEffettuateDocPagingOperationCompleted);
            this.InvokeAsync("TrasmissioneGetQueryEffettuateDocPaging", new object[5]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) pageNumber
      }, this.TrasmissioneGetQueryEffettuateDocPagingOperationCompleted, userState);
        }

        private void OnTrasmissioneGetQueryEffettuateDocPagingOperationCompleted(object arg)
        {
            if (this.TrasmissioneGetQueryEffettuateDocPagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneGetQueryEffettuateDocPagingCompleted((object)this, new TrasmissioneGetQueryEffettuateDocPagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneGetQueryEffettuate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione[] TrasmissioneGetQueryEffettuate(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo)
        {
            return (Trasmissione[])this.Invoke("TrasmissioneGetQueryEffettuate", new object[4]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo
      })[0];
        }

        public IAsyncResult BeginTrasmissioneGetQueryEffettuate(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneGetQueryEffettuate", new object[4]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo
      }, callback, asyncState);
        }

        public Trasmissione[] EndTrasmissioneGetQueryEffettuate(IAsyncResult asyncResult)
        {
            return (Trasmissione[])this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioneGetQueryEffettuateAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo)
        {
            this.TrasmissioneGetQueryEffettuateAsync(oggettoTrasmesso, listaFiltri, utente, ruolo, (object)null);
        }

        public void TrasmissioneGetQueryEffettuateAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, object userState)
        {
            if (this.TrasmissioneGetQueryEffettuateOperationCompleted == null)
                this.TrasmissioneGetQueryEffettuateOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneGetQueryEffettuateOperationCompleted);
            this.InvokeAsync("TrasmissioneGetQueryEffettuate", new object[4]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo
      }, this.TrasmissioneGetQueryEffettuateOperationCompleted, userState);
        }

        private void OnTrasmissioneGetQueryEffettuateOperationCompleted(object arg)
        {
            if (this.TrasmissioneGetQueryEffettuateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneGetQueryEffettuateCompleted((object)this, new TrasmissioneGetQueryEffettuateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneGetQueryRicevutePaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione[] TrasmissioneGetQueryRicevutePaging(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber, out int totalPageNumber, out int recordCount)
        {
            object[] objArray = this.Invoke("TrasmissioneGetQueryRicevutePaging", new object[5]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) pageNumber
      });
            totalPageNumber = (int)objArray[1];
            recordCount = (int)objArray[2];
            return (Trasmissione[])objArray[0];
        }

        public IAsyncResult BeginTrasmissioneGetQueryRicevutePaging(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneGetQueryRicevutePaging", new object[5]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) pageNumber
      }, callback, asyncState);
        }

        public Trasmissione[] EndTrasmissioneGetQueryRicevutePaging(IAsyncResult asyncResult, out int totalPageNumber, out int recordCount)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            totalPageNumber = (int)objArray[1];
            recordCount = (int)objArray[2];
            return (Trasmissione[])objArray[0];
        }

        public void TrasmissioneGetQueryRicevutePagingAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber)
        {
            this.TrasmissioneGetQueryRicevutePagingAsync(oggettoTrasmesso, listaFiltri, utente, ruolo, pageNumber, (object)null);
        }

        public void TrasmissioneGetQueryRicevutePagingAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int pageNumber, object userState)
        {
            if (this.TrasmissioneGetQueryRicevutePagingOperationCompleted == null)
                this.TrasmissioneGetQueryRicevutePagingOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneGetQueryRicevutePagingOperationCompleted);
            this.InvokeAsync("TrasmissioneGetQueryRicevutePaging", new object[5]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) pageNumber
      }, this.TrasmissioneGetQueryRicevutePagingOperationCompleted, userState);
        }

        private void OnTrasmissioneGetQueryRicevutePagingOperationCompleted(object arg)
        {
            if (this.TrasmissioneGetQueryRicevutePagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneGetQueryRicevutePagingCompleted((object)this, new TrasmissioneGetQueryRicevutePagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneGetQueryRicevute", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione[] TrasmissioneGetQueryRicevute(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo)
        {
            return (Trasmissione[])this.Invoke("TrasmissioneGetQueryRicevute", new object[4]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo
      })[0];
        }

        public IAsyncResult BeginTrasmissioneGetQueryRicevute(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneGetQueryRicevute", new object[4]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo
      }, callback, asyncState);
        }

        public Trasmissione[] EndTrasmissioneGetQueryRicevute(IAsyncResult asyncResult)
        {
            return (Trasmissione[])this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioneGetQueryRicevuteAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo)
        {
            this.TrasmissioneGetQueryRicevuteAsync(oggettoTrasmesso, listaFiltri, utente, ruolo, (object)null);
        }

        public void TrasmissioneGetQueryRicevuteAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, object userState)
        {
            if (this.TrasmissioneGetQueryRicevuteOperationCompleted == null)
                this.TrasmissioneGetQueryRicevuteOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneGetQueryRicevuteOperationCompleted);
            this.InvokeAsync("TrasmissioneGetQueryRicevute", new object[4]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo
      }, this.TrasmissioneGetQueryRicevuteOperationCompleted, userState);
        }

        private void OnTrasmissioneGetQueryRicevuteOperationCompleted(object arg)
        {
            if (this.TrasmissioneGetQueryRicevuteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneGetQueryRicevuteCompleted((object)this, new TrasmissioneGetQueryRicevuteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneExecuteTrasm", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione TrasmissioneExecuteTrasm(string path, Trasmissione trasmissione)
        {
            return (Trasmissione)this.Invoke("TrasmissioneExecuteTrasm", new object[2]
      {
        (object) path,
        (object) trasmissione
      })[0];
        }

        public IAsyncResult BeginTrasmissioneExecuteTrasm(string path, Trasmissione trasmissione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneExecuteTrasm", new object[2]
      {
        (object) path,
        (object) trasmissione
      }, callback, asyncState);
        }

        public Trasmissione EndTrasmissioneExecuteTrasm(IAsyncResult asyncResult)
        {
            return (Trasmissione)this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioneExecuteTrasmAsync(string path, Trasmissione trasmissione)
        {
            this.TrasmissioneExecuteTrasmAsync(path, trasmissione, (object)null);
        }

        public void TrasmissioneExecuteTrasmAsync(string path, Trasmissione trasmissione, object userState)
        {
            if (this.TrasmissioneExecuteTrasmOperationCompleted == null)
                this.TrasmissioneExecuteTrasmOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneExecuteTrasmOperationCompleted);
            this.InvokeAsync("TrasmissioneExecuteTrasm", new object[2]
      {
        (object) path,
        (object) trasmissione
      }, this.TrasmissioneExecuteTrasmOperationCompleted, userState);
        }

        private void OnTrasmissioneExecuteTrasmOperationCompleted(object arg)
        {
            if (this.TrasmissioneExecuteTrasmCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneExecuteTrasmCompleted((object)this, new TrasmissioneExecuteTrasmCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneSaveTrasm", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione TrasmissioneSaveTrasm(Trasmissione trasmissione)
        {
            return (Trasmissione)this.Invoke("TrasmissioneSaveTrasm", new object[1]
      {
        (object) trasmissione
      })[0];
        }

        public IAsyncResult BeginTrasmissioneSaveTrasm(Trasmissione trasmissione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneSaveTrasm", new object[1]
      {
        (object) trasmissione
      }, callback, asyncState);
        }

        public Trasmissione EndTrasmissioneSaveTrasm(IAsyncResult asyncResult)
        {
            return (Trasmissione)this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioneSaveTrasmAsync(Trasmissione trasmissione)
        {
            this.TrasmissioneSaveTrasmAsync(trasmissione, (object)null);
        }

        public void TrasmissioneSaveTrasmAsync(Trasmissione trasmissione, object userState)
        {
            if (this.TrasmissioneSaveTrasmOperationCompleted == null)
                this.TrasmissioneSaveTrasmOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneSaveTrasmOperationCompleted);
            this.InvokeAsync("TrasmissioneSaveTrasm", new object[1]
      {
        (object) trasmissione
      }, this.TrasmissioneSaveTrasmOperationCompleted, userState);
        }

        private void OnTrasmissioneSaveTrasmOperationCompleted(object arg)
        {
            if (this.TrasmissioneSaveTrasmCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneSaveTrasmCompleted((object)this, new TrasmissioneSaveTrasmCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneFascicoloAddDaTempl", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione TrasmissioneFascicoloAddDaTempl(InfoFascicolo infoFascicolo, TemplateTrasmissione template, Utente utente, Ruolo ruolo)
        {
            return (Trasmissione)this.Invoke("TrasmissioneFascicoloAddDaTempl", new object[4]
      {
        (object) infoFascicolo,
        (object) template,
        (object) utente,
        (object) ruolo
      })[0];
        }

        public IAsyncResult BeginTrasmissioneFascicoloAddDaTempl(InfoFascicolo infoFascicolo, TemplateTrasmissione template, Utente utente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneFascicoloAddDaTempl", new object[4]
      {
        (object) infoFascicolo,
        (object) template,
        (object) utente,
        (object) ruolo
      }, callback, asyncState);
        }

        public Trasmissione EndTrasmissioneFascicoloAddDaTempl(IAsyncResult asyncResult)
        {
            return (Trasmissione)this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioneFascicoloAddDaTemplAsync(InfoFascicolo infoFascicolo, TemplateTrasmissione template, Utente utente, Ruolo ruolo)
        {
            this.TrasmissioneFascicoloAddDaTemplAsync(infoFascicolo, template, utente, ruolo, (object)null);
        }

        public void TrasmissioneFascicoloAddDaTemplAsync(InfoFascicolo infoFascicolo, TemplateTrasmissione template, Utente utente, Ruolo ruolo, object userState)
        {
            if (this.TrasmissioneFascicoloAddDaTemplOperationCompleted == null)
                this.TrasmissioneFascicoloAddDaTemplOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneFascicoloAddDaTemplOperationCompleted);
            this.InvokeAsync("TrasmissioneFascicoloAddDaTempl", new object[4]
      {
        (object) infoFascicolo,
        (object) template,
        (object) utente,
        (object) ruolo
      }, this.TrasmissioneFascicoloAddDaTemplOperationCompleted, userState);
        }

        private void OnTrasmissioneFascicoloAddDaTemplOperationCompleted(object arg)
        {
            if (this.TrasmissioneFascicoloAddDaTemplCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneFascicoloAddDaTemplCompleted((object)this, new TrasmissioneFascicoloAddDaTemplCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmissioneAddDaTempl", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione TrasmissioneAddDaTempl(InfoDocumento infoDoc, TemplateTrasmissione template, Utente utente, Ruolo ruolo)
        {
            return (Trasmissione)this.Invoke("TrasmissioneAddDaTempl", new object[4]
      {
        (object) infoDoc,
        (object) template,
        (object) utente,
        (object) ruolo
      })[0];
        }

        public IAsyncResult BeginTrasmissioneAddDaTempl(InfoDocumento infoDoc, TemplateTrasmissione template, Utente utente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmissioneAddDaTempl", new object[4]
      {
        (object) infoDoc,
        (object) template,
        (object) utente,
        (object) ruolo
      }, callback, asyncState);
        }

        public Trasmissione EndTrasmissioneAddDaTempl(IAsyncResult asyncResult)
        {
            return (Trasmissione)this.EndInvoke(asyncResult)[0];
        }

        public void TrasmissioneAddDaTemplAsync(InfoDocumento infoDoc, TemplateTrasmissione template, Utente utente, Ruolo ruolo)
        {
            this.TrasmissioneAddDaTemplAsync(infoDoc, template, utente, ruolo, (object)null);
        }

        public void TrasmissioneAddDaTemplAsync(InfoDocumento infoDoc, TemplateTrasmissione template, Utente utente, Ruolo ruolo, object userState)
        {
            if (this.TrasmissioneAddDaTemplOperationCompleted == null)
                this.TrasmissioneAddDaTemplOperationCompleted = new SendOrPostCallback(this.OnTrasmissioneAddDaTemplOperationCompleted);
            this.InvokeAsync("TrasmissioneAddDaTempl", new object[4]
      {
        (object) infoDoc,
        (object) template,
        (object) utente,
        (object) ruolo
      }, this.TrasmissioneAddDaTemplOperationCompleted, userState);
        }

        private void OnTrasmissioneAddDaTemplOperationCompleted(object arg)
        {
            if (this.TrasmissioneAddDaTemplCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmissioneAddDaTemplCompleted((object)this, new TrasmissioneAddDaTemplCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioneSetTxUtAsViste", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void trasmissioneSetTxUtAsViste(InfoUtente infoUtente)
        {
            this.Invoke("trasmissioneSetTxUtAsViste", new object[1]
      {
        (object) infoUtente
      });
        }

        public IAsyncResult BegintrasmissioneSetTxUtAsViste(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneSetTxUtAsViste", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndtrasmissioneSetTxUtAsViste(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void trasmissioneSetTxUtAsVisteAsync(InfoUtente infoUtente)
        {
            this.trasmissioneSetTxUtAsVisteAsync(infoUtente, (object)null);
        }

        public void trasmissioneSetTxUtAsVisteAsync(InfoUtente infoUtente, object userState)
        {
            if (this.trasmissioneSetTxUtAsVisteOperationCompleted == null)
                this.trasmissioneSetTxUtAsVisteOperationCompleted = new SendOrPostCallback(this.OntrasmissioneSetTxUtAsVisteOperationCompleted);
            this.InvokeAsync("trasmissioneSetTxUtAsViste", new object[1]
      {
        (object) infoUtente
      }, this.trasmissioneSetTxUtAsVisteOperationCompleted, userState);
        }

        private void OntrasmissioneSetTxUtAsVisteOperationCompleted(object arg)
        {
            if (this.trasmissioneSetTxUtAsVisteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneSetTxUtAsVisteCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetVisibilita", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicoloDiritto[] FascicolazioneGetVisibilita(InfoFascicolo infoFascicolo)
        {
            return (FascicoloDiritto[])this.Invoke("FascicolazioneGetVisibilita", new object[1]
      {
        (object) infoFascicolo
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetVisibilita(InfoFascicolo infoFascicolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetVisibilita", new object[1]
      {
        (object) infoFascicolo
      }, callback, asyncState);
        }

        public FascicoloDiritto[] EndFascicolazioneGetVisibilita(IAsyncResult asyncResult)
        {
            return (FascicoloDiritto[])this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetVisibilitaAsync(InfoFascicolo infoFascicolo)
        {
            this.FascicolazioneGetVisibilitaAsync(infoFascicolo, (object)null);
        }

        public void FascicolazioneGetVisibilitaAsync(InfoFascicolo infoFascicolo, object userState)
        {
            if (this.FascicolazioneGetVisibilitaOperationCompleted == null)
                this.FascicolazioneGetVisibilitaOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetVisibilitaOperationCompleted);
            this.InvokeAsync("FascicolazioneGetVisibilita", new object[1]
      {
        (object) infoFascicolo
      }, this.FascicolazioneGetVisibilitaOperationCompleted, userState);
        }

        private void OnFascicolazioneGetVisibilitaOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetVisibilitaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetVisibilitaCompleted((object)this, new FascicolazioneGetVisibilitaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneSospendiRiattivaUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool FascicolazioneSospendiRiattivaUtente(string idPeople, Corrispondente corr, Fascicolo fasc)
        {
            return (bool)this.Invoke("FascicolazioneSospendiRiattivaUtente", new object[3]
      {
        (object) idPeople,
        (object) corr,
        (object) fasc
      })[0];
        }

        public IAsyncResult BeginFascicolazioneSospendiRiattivaUtente(string idPeople, Corrispondente corr, Fascicolo fasc, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneSospendiRiattivaUtente", new object[3]
      {
        (object) idPeople,
        (object) corr,
        (object) fasc
      }, callback, asyncState);
        }

        public bool EndFascicolazioneSospendiRiattivaUtente(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneSospendiRiattivaUtenteAsync(string idPeople, Corrispondente corr, Fascicolo fasc)
        {
            this.FascicolazioneSospendiRiattivaUtenteAsync(idPeople, corr, fasc, (object)null);
        }

        public void FascicolazioneSospendiRiattivaUtenteAsync(string idPeople, Corrispondente corr, Fascicolo fasc, object userState)
        {
            if (this.FascicolazioneSospendiRiattivaUtenteOperationCompleted == null)
                this.FascicolazioneSospendiRiattivaUtenteOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneSospendiRiattivaUtenteOperationCompleted);
            this.InvokeAsync("FascicolazioneSospendiRiattivaUtente", new object[3]
      {
        (object) idPeople,
        (object) corr,
        (object) fasc
      }, this.FascicolazioneSospendiRiattivaUtenteOperationCompleted, userState);
        }

        private void OnFascicolazioneSospendiRiattivaUtenteOperationCompleted(object arg)
        {
            if (this.FascicolazioneSospendiRiattivaUtenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneSospendiRiattivaUtenteCompleted((object)this, new FascicolazioneSospendiRiattivaUtenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetFigliClassifica", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicolazioneClassifica[] FascicolazioneGetFigliClassifica(string idGruppo, string idPeople, FascicolazioneClassifica classifica, string idRegistro, string idAmm)
        {
            return (FascicolazioneClassifica[])this.Invoke("FascicolazioneGetFigliClassifica", new object[5]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) classifica,
        (object) idRegistro,
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetFigliClassifica(string idGruppo, string idPeople, FascicolazioneClassifica classifica, string idRegistro, string idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetFigliClassifica", new object[5]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) classifica,
        (object) idRegistro,
        (object) idAmm
      }, callback, asyncState);
        }

        public FascicolazioneClassifica[] EndFascicolazioneGetFigliClassifica(IAsyncResult asyncResult)
        {
            return (FascicolazioneClassifica[])this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetFigliClassificaAsync(string idGruppo, string idPeople, FascicolazioneClassifica classifica, string idRegistro, string idAmm)
        {
            this.FascicolazioneGetFigliClassificaAsync(idGruppo, idPeople, classifica, idRegistro, idAmm, (object)null);
        }

        public void FascicolazioneGetFigliClassificaAsync(string idGruppo, string idPeople, FascicolazioneClassifica classifica, string idRegistro, string idAmm, object userState)
        {
            if (this.FascicolazioneGetFigliClassificaOperationCompleted == null)
                this.FascicolazioneGetFigliClassificaOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetFigliClassificaOperationCompleted);
            this.InvokeAsync("FascicolazioneGetFigliClassifica", new object[5]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) classifica,
        (object) idRegistro,
        (object) idAmm
      }, this.FascicolazioneGetFigliClassificaOperationCompleted, userState);
        }

        private void OnFascicolazioneGetFigliClassificaOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetFigliClassificaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetFigliClassificaCompleted((object)this, new FascicolazioneGetFigliClassificaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetGerarchiaDaCodice", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicolazioneClassifica[] FascicolazioneGetGerarchiaDaCodice(string codiceClassificazione, Registro registro, string idAmm)
        {
            return (FascicolazioneClassifica[])this.Invoke("FascicolazioneGetGerarchiaDaCodice", new object[3]
      {
        (object) codiceClassificazione,
        (object) registro,
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetGerarchiaDaCodice(string codiceClassificazione, Registro registro, string idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetGerarchiaDaCodice", new object[3]
      {
        (object) codiceClassificazione,
        (object) registro,
        (object) idAmm
      }, callback, asyncState);
        }

        public FascicolazioneClassifica[] EndFascicolazioneGetGerarchiaDaCodice(IAsyncResult asyncResult)
        {
            return (FascicolazioneClassifica[])this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetGerarchiaDaCodiceAsync(string codiceClassificazione, Registro registro, string idAmm)
        {
            this.FascicolazioneGetGerarchiaDaCodiceAsync(codiceClassificazione, registro, idAmm, (object)null);
        }

        public void FascicolazioneGetGerarchiaDaCodiceAsync(string codiceClassificazione, Registro registro, string idAmm, object userState)
        {
            if (this.FascicolazioneGetGerarchiaDaCodiceOperationCompleted == null)
                this.FascicolazioneGetGerarchiaDaCodiceOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetGerarchiaDaCodiceOperationCompleted);
            this.InvokeAsync("FascicolazioneGetGerarchiaDaCodice", new object[3]
      {
        (object) codiceClassificazione,
        (object) registro,
        (object) idAmm
      }, this.FascicolazioneGetGerarchiaDaCodiceOperationCompleted, userState);
        }

        private void OnFascicolazioneGetGerarchiaDaCodiceOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetGerarchiaDaCodiceCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetGerarchiaDaCodiceCompleted((object)this, new FascicolazioneGetGerarchiaDaCodiceCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetGerarchia", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicolazioneClassifica[] FascicolazioneGetGerarchia(string idClassificazione, string idAmm)
        {
            return (FascicolazioneClassifica[])this.Invoke("FascicolazioneGetGerarchia", new object[2]
      {
        (object) idClassificazione,
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetGerarchia(string idClassificazione, string idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetGerarchia", new object[2]
      {
        (object) idClassificazione,
        (object) idAmm
      }, callback, asyncState);
        }

        public FascicolazioneClassifica[] EndFascicolazioneGetGerarchia(IAsyncResult asyncResult)
        {
            return (FascicolazioneClassifica[])this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetGerarchiaAsync(string idClassificazione, string idAmm)
        {
            this.FascicolazioneGetGerarchiaAsync(idClassificazione, idAmm, (object)null);
        }

        public void FascicolazioneGetGerarchiaAsync(string idClassificazione, string idAmm, object userState)
        {
            if (this.FascicolazioneGetGerarchiaOperationCompleted == null)
                this.FascicolazioneGetGerarchiaOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetGerarchiaOperationCompleted);
            this.InvokeAsync("FascicolazioneGetGerarchia", new object[2]
      {
        (object) idClassificazione,
        (object) idAmm
      }, this.FascicolazioneGetGerarchiaOperationCompleted, userState);
        }

        private void OnFascicolazioneGetGerarchiaOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetGerarchiaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetGerarchiaCompleted((object)this, new FascicolazioneGetGerarchiaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetFascicoloDaCodice", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo FascicolazioneGetFascicoloDaCodice(string idAmministrazione, string idGruppo, string idPeople, string codiceFascicolo, Registro registro, bool enableUffRef)
        {
            return (Fascicolo)this.Invoke("FascicolazioneGetFascicoloDaCodice", new object[6]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) codiceFascicolo,
        (object) registro,
        (object) enableUffRef
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetFascicoloDaCodice(string idAmministrazione, string idGruppo, string idPeople, string codiceFascicolo, Registro registro, bool enableUffRef, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetFascicoloDaCodice", new object[6]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) codiceFascicolo,
        (object) registro,
        (object) enableUffRef
      }, callback, asyncState);
        }

        public Fascicolo EndFascicolazioneGetFascicoloDaCodice(IAsyncResult asyncResult)
        {
            return (Fascicolo)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetFascicoloDaCodiceAsync(string idAmministrazione, string idGruppo, string idPeople, string codiceFascicolo, Registro registro, bool enableUffRef)
        {
            this.FascicolazioneGetFascicoloDaCodiceAsync(idAmministrazione, idGruppo, idPeople, codiceFascicolo, registro, enableUffRef, (object)null);
        }

        public void FascicolazioneGetFascicoloDaCodiceAsync(string idAmministrazione, string idGruppo, string idPeople, string codiceFascicolo, Registro registro, bool enableUffRef, object userState)
        {
            if (this.FascicolazioneGetFascicoloDaCodiceOperationCompleted == null)
                this.FascicolazioneGetFascicoloDaCodiceOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetFascicoloDaCodiceOperationCompleted);
            this.InvokeAsync("FascicolazioneGetFascicoloDaCodice", new object[6]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) codiceFascicolo,
        (object) registro,
        (object) enableUffRef
      }, this.FascicolazioneGetFascicoloDaCodiceOperationCompleted, userState);
        }

        private void OnFascicolazioneGetFascicoloDaCodiceOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetFascicoloDaCodiceCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetFascicoloDaCodiceCompleted((object)this, new FascicolazioneGetFascicoloDaCodiceCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetFascicoliDaDoc", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo[] FascicolazioneGetFascicoliDaDoc(string idPeople, string idGruppo, string idProfile)
        {
            return (Fascicolo[])this.Invoke("FascicolazioneGetFascicoliDaDoc", new object[3]
      {
        (object) idPeople,
        (object) idGruppo,
        (object) idProfile
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetFascicoliDaDoc(string idPeople, string idGruppo, string idProfile, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetFascicoliDaDoc", new object[3]
      {
        (object) idPeople,
        (object) idGruppo,
        (object) idProfile
      }, callback, asyncState);
        }

        public Fascicolo[] EndFascicolazioneGetFascicoliDaDoc(IAsyncResult asyncResult)
        {
            return (Fascicolo[])this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetFascicoliDaDocAsync(string idPeople, string idGruppo, string idProfile)
        {
            this.FascicolazioneGetFascicoliDaDocAsync(idPeople, idGruppo, idProfile, (object)null);
        }

        public void FascicolazioneGetFascicoliDaDocAsync(string idPeople, string idGruppo, string idProfile, object userState)
        {
            if (this.FascicolazioneGetFascicoliDaDocOperationCompleted == null)
                this.FascicolazioneGetFascicoliDaDocOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetFascicoliDaDocOperationCompleted);
            this.InvokeAsync("FascicolazioneGetFascicoliDaDoc", new object[3]
      {
        (object) idPeople,
        (object) idGruppo,
        (object) idProfile
      }, this.FascicolazioneGetFascicoliDaDocOperationCompleted, userState);
        }

        private void OnFascicolazioneGetFascicoliDaDocOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetFascicoliDaDocCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetFascicoliDaDocCompleted((object)this, new FascicolazioneGetFascicoliDaDocCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetDettaglioFascicolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo FascicolazioneGetDettaglioFascicolo(string idGruppo, string idPeople, InfoFascicolo infoFascicolo, bool enableUffRef)
        {
            return (Fascicolo)this.Invoke("FascicolazioneGetDettaglioFascicolo", new object[4]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) infoFascicolo,
        (object) enableUffRef
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetDettaglioFascicolo(string idGruppo, string idPeople, InfoFascicolo infoFascicolo, bool enableUffRef, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetDettaglioFascicolo", new object[4]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) infoFascicolo,
        (object) enableUffRef
      }, callback, asyncState);
        }

        public Fascicolo EndFascicolazioneGetDettaglioFascicolo(IAsyncResult asyncResult)
        {
            return (Fascicolo)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetDettaglioFascicoloAsync(string idGruppo, string idPeople, InfoFascicolo infoFascicolo, bool enableUffRef)
        {
            this.FascicolazioneGetDettaglioFascicoloAsync(idGruppo, idPeople, infoFascicolo, enableUffRef, (object)null);
        }

        public void FascicolazioneGetDettaglioFascicoloAsync(string idGruppo, string idPeople, InfoFascicolo infoFascicolo, bool enableUffRef, object userState)
        {
            if (this.FascicolazioneGetDettaglioFascicoloOperationCompleted == null)
                this.FascicolazioneGetDettaglioFascicoloOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetDettaglioFascicoloOperationCompleted);
            this.InvokeAsync("FascicolazioneGetDettaglioFascicolo", new object[4]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) infoFascicolo,
        (object) enableUffRef
      }, this.FascicolazioneGetDettaglioFascicoloOperationCompleted, userState);
        }

        private void OnFascicolazioneGetDettaglioFascicoloOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetDettaglioFascicoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetDettaglioFascicoloCompleted((object)this, new FascicolazioneGetDettaglioFascicoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneAddDocFolder", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool FascicolazioneAddDocFolder(InfoUtente infoutente, string idProfile, string idFolder)
        {
            return (bool)this.Invoke("FascicolazioneAddDocFolder", new object[3]
      {
        (object) infoutente,
        (object) idProfile,
        (object) idFolder
      })[0];
        }

        public IAsyncResult BeginFascicolazioneAddDocFolder(InfoUtente infoutente, string idProfile, string idFolder, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneAddDocFolder", new object[3]
      {
        (object) infoutente,
        (object) idProfile,
        (object) idFolder
      }, callback, asyncState);
        }

        public bool EndFascicolazioneAddDocFolder(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneAddDocFolderAsync(InfoUtente infoutente, string idProfile, string idFolder)
        {
            this.FascicolazioneAddDocFolderAsync(infoutente, idProfile, idFolder, (object)null);
        }

        public void FascicolazioneAddDocFolderAsync(InfoUtente infoutente, string idProfile, string idFolder, object userState)
        {
            if (this.FascicolazioneAddDocFolderOperationCompleted == null)
                this.FascicolazioneAddDocFolderOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneAddDocFolderOperationCompleted);
            this.InvokeAsync("FascicolazioneAddDocFolder", new object[3]
      {
        (object) infoutente,
        (object) idProfile,
        (object) idFolder
      }, this.FascicolazioneAddDocFolderOperationCompleted, userState);
        }

        private void OnFascicolazioneAddDocFolderOperationCompleted(object arg)
        {
            if (this.FascicolazioneAddDocFolderCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneAddDocFolderCompleted((object)this, new FascicolazioneAddDocFolderCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneAddDocFascicolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool FascicolazioneAddDocFascicolo(InfoUtente infoutente, string idProfile, string idFascicolo)
        {
            return (bool)this.Invoke("FascicolazioneAddDocFascicolo", new object[3]
      {
        (object) infoutente,
        (object) idProfile,
        (object) idFascicolo
      })[0];
        }

        public IAsyncResult BeginFascicolazioneAddDocFascicolo(InfoUtente infoutente, string idProfile, string idFascicolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneAddDocFascicolo", new object[3]
      {
        (object) infoutente,
        (object) idProfile,
        (object) idFascicolo
      }, callback, asyncState);
        }

        public bool EndFascicolazioneAddDocFascicolo(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneAddDocFascicoloAsync(InfoUtente infoutente, string idProfile, string idFascicolo)
        {
            this.FascicolazioneAddDocFascicoloAsync(infoutente, idProfile, idFascicolo, (object)null);
        }

        public void FascicolazioneAddDocFascicoloAsync(InfoUtente infoutente, string idProfile, string idFascicolo, object userState)
        {
            if (this.FascicolazioneAddDocFascicoloOperationCompleted == null)
                this.FascicolazioneAddDocFascicoloOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneAddDocFascicoloOperationCompleted);
            this.InvokeAsync("FascicolazioneAddDocFascicolo", new object[3]
      {
        (object) infoutente,
        (object) idProfile,
        (object) idFascicolo
      }, this.FascicolazioneAddDocFascicoloOperationCompleted, userState);
        }

        private void OnFascicolazioneAddDocFascicoloOperationCompleted(object arg)
        {
            if (this.FascicolazioneAddDocFascicoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneAddDocFascicoloCompleted((object)this, new FascicolazioneAddDocFascicoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneDelFolder", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool FascicolazioneDelFolder(Folder folder, InfoUtente infoUtente)
        {
            return (bool)this.Invoke("FascicolazioneDelFolder", new object[2]
      {
        (object) folder,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginFascicolazioneDelFolder(Folder folder, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneDelFolder", new object[2]
      {
        (object) folder,
        (object) infoUtente
      }, callback, asyncState);
        }

        public bool EndFascicolazioneDelFolder(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneDelFolderAsync(Folder folder, InfoUtente infoUtente)
        {
            this.FascicolazioneDelFolderAsync(folder, infoUtente, (object)null);
        }

        public void FascicolazioneDelFolderAsync(Folder folder, InfoUtente infoUtente, object userState)
        {
            if (this.FascicolazioneDelFolderOperationCompleted == null)
                this.FascicolazioneDelFolderOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneDelFolderOperationCompleted);
            this.InvokeAsync("FascicolazioneDelFolder", new object[2]
      {
        (object) folder,
        (object) infoUtente
      }, this.FascicolazioneDelFolderOperationCompleted, userState);
        }

        private void OnFascicolazioneDelFolderOperationCompleted(object arg)
        {
            if (this.FascicolazioneDelFolderCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneDelFolderCompleted((object)this, new FascicolazioneDelFolderCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneDelTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool FascicolazioneDelTitolario(FascicolazioneClassificazione nodoTitolario, InfoUtente infoUtente)
        {
            return (bool)this.Invoke("FascicolazioneDelTitolario", new object[2]
      {
        (object) nodoTitolario,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginFascicolazioneDelTitolario(FascicolazioneClassificazione nodoTitolario, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneDelTitolario", new object[2]
      {
        (object) nodoTitolario,
        (object) infoUtente
      }, callback, asyncState);
        }

        public bool EndFascicolazioneDelTitolario(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneDelTitolarioAsync(FascicolazioneClassificazione nodoTitolario, InfoUtente infoUtente)
        {
            this.FascicolazioneDelTitolarioAsync(nodoTitolario, infoUtente, (object)null);
        }

        public void FascicolazioneDelTitolarioAsync(FascicolazioneClassificazione nodoTitolario, InfoUtente infoUtente, object userState)
        {
            if (this.FascicolazioneDelTitolarioOperationCompleted == null)
                this.FascicolazioneDelTitolarioOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneDelTitolarioOperationCompleted);
            this.InvokeAsync("FascicolazioneDelTitolario", new object[2]
      {
        (object) nodoTitolario,
        (object) infoUtente
      }, this.FascicolazioneDelTitolarioOperationCompleted, userState);
        }

        private void OnFascicolazioneDelTitolarioOperationCompleted(object arg)
        {
            if (this.FascicolazioneDelTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneDelTitolarioCompleted((object)this, new FascicolazioneDelTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneUpdateTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicolazioneClassificazione FascicolazioneUpdateTitolario(FascicolazioneClassificazione nodoTitolario)
        {
            return (FascicolazioneClassificazione)this.Invoke("FascicolazioneUpdateTitolario", new object[1]
      {
        (object) nodoTitolario
      })[0];
        }

        public IAsyncResult BeginFascicolazioneUpdateTitolario(FascicolazioneClassificazione nodoTitolario, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneUpdateTitolario", new object[1]
      {
        (object) nodoTitolario
      }, callback, asyncState);
        }

        public FascicolazioneClassificazione EndFascicolazioneUpdateTitolario(IAsyncResult asyncResult)
        {
            return (FascicolazioneClassificazione)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneUpdateTitolarioAsync(FascicolazioneClassificazione nodoTitolario)
        {
            this.FascicolazioneUpdateTitolarioAsync(nodoTitolario, (object)null);
        }

        public void FascicolazioneUpdateTitolarioAsync(FascicolazioneClassificazione nodoTitolario, object userState)
        {
            if (this.FascicolazioneUpdateTitolarioOperationCompleted == null)
                this.FascicolazioneUpdateTitolarioOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneUpdateTitolarioOperationCompleted);
            this.InvokeAsync("FascicolazioneUpdateTitolario", new object[1]
      {
        (object) nodoTitolario
      }, this.FascicolazioneUpdateTitolarioOperationCompleted, userState);
        }

        private void OnFascicolazioneUpdateTitolarioOperationCompleted(object arg)
        {
            if (this.FascicolazioneUpdateTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneUpdateTitolarioCompleted((object)this, new FascicolazioneUpdateTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneModifyFolder", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool FascicolazioneModifyFolder(Folder folder)
        {
            return (bool)this.Invoke("FascicolazioneModifyFolder", new object[1]
      {
        (object) folder
      })[0];
        }

        public IAsyncResult BeginFascicolazioneModifyFolder(Folder folder, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneModifyFolder", new object[1]
      {
        (object) folder
      }, callback, asyncState);
        }

        public bool EndFascicolazioneModifyFolder(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneModifyFolderAsync(Folder folder)
        {
            this.FascicolazioneModifyFolderAsync(folder, (object)null);
        }

        public void FascicolazioneModifyFolderAsync(Folder folder, object userState)
        {
            if (this.FascicolazioneModifyFolderOperationCompleted == null)
                this.FascicolazioneModifyFolderOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneModifyFolderOperationCompleted);
            this.InvokeAsync("FascicolazioneModifyFolder", new object[1]
      {
        (object) folder
      }, this.FascicolazioneModifyFolderOperationCompleted, userState);
        }

        private void OnFascicolazioneModifyFolderOperationCompleted(object arg)
        {
            if (this.FascicolazioneModifyFolderCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneModifyFolderCompleted((object)this, new FascicolazioneModifyFolderCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneNewFolder", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Folder FascicolazioneNewFolder(Folder folder, InfoUtente infoUtente, Ruolo ruolo)
        {
            return (Folder)this.Invoke("FascicolazioneNewFolder", new object[3]
      {
        (object) folder,
        (object) infoUtente,
        (object) ruolo
      })[0];
        }

        public IAsyncResult BeginFascicolazioneNewFolder(Folder folder, InfoUtente infoUtente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneNewFolder", new object[3]
      {
        (object) folder,
        (object) infoUtente,
        (object) ruolo
      }, callback, asyncState);
        }

        public Folder EndFascicolazioneNewFolder(IAsyncResult asyncResult)
        {
            return (Folder)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneNewFolderAsync(Folder folder, InfoUtente infoUtente, Ruolo ruolo)
        {
            this.FascicolazioneNewFolderAsync(folder, infoUtente, ruolo, (object)null);
        }

        public void FascicolazioneNewFolderAsync(Folder folder, InfoUtente infoUtente, Ruolo ruolo, object userState)
        {
            if (this.FascicolazioneNewFolderOperationCompleted == null)
                this.FascicolazioneNewFolderOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneNewFolderOperationCompleted);
            this.InvokeAsync("FascicolazioneNewFolder", new object[3]
      {
        (object) folder,
        (object) infoUtente,
        (object) ruolo
      }, this.FascicolazioneNewFolderOperationCompleted, userState);
        }

        private void OnFascicolazioneNewFolderOperationCompleted(object arg)
        {
            if (this.FascicolazioneNewFolderCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneNewFolderCompleted((object)this, new FascicolazioneNewFolderCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneSetFascicolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool FascicolazioneSetFascicolo(Fascicolo fascicolo)
        {
            return (bool)this.Invoke("FascicolazioneSetFascicolo", new object[1]
      {
        (object) fascicolo
      })[0];
        }

        public IAsyncResult BeginFascicolazioneSetFascicolo(Fascicolo fascicolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneSetFascicolo", new object[1]
      {
        (object) fascicolo
      }, callback, asyncState);
        }

        public bool EndFascicolazioneSetFascicolo(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneSetFascicoloAsync(Fascicolo fascicolo)
        {
            this.FascicolazioneSetFascicoloAsync(fascicolo, (object)null);
        }

        public void FascicolazioneSetFascicoloAsync(Fascicolo fascicolo, object userState)
        {
            if (this.FascicolazioneSetFascicoloOperationCompleted == null)
                this.FascicolazioneSetFascicoloOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneSetFascicoloOperationCompleted);
            this.InvokeAsync("FascicolazioneSetFascicolo", new object[1]
      {
        (object) fascicolo
      }, this.FascicolazioneSetFascicoloOperationCompleted, userState);
        }

        private void OnFascicolazioneSetFascicoloOperationCompleted(object arg)
        {
            if (this.FascicolazioneSetFascicoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneSetFascicoloCompleted((object)this, new FascicolazioneSetFascicoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneDeleteDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool FascicolazioneDeleteDocumento(string idProfile, Folder folder)
        {
            return (bool)this.Invoke("FascicolazioneDeleteDocumento", new object[2]
      {
        (object) idProfile,
        (object) folder
      })[0];
        }

        public IAsyncResult BeginFascicolazioneDeleteDocumento(string idProfile, Folder folder, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneDeleteDocumento", new object[2]
      {
        (object) idProfile,
        (object) folder
      }, callback, asyncState);
        }

        public bool EndFascicolazioneDeleteDocumento(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneDeleteDocumentoAsync(string idProfile, Folder folder)
        {
            this.FascicolazioneDeleteDocumentoAsync(idProfile, folder, (object)null);
        }

        public void FascicolazioneDeleteDocumentoAsync(string idProfile, Folder folder, object userState)
        {
            if (this.FascicolazioneDeleteDocumentoOperationCompleted == null)
                this.FascicolazioneDeleteDocumentoOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneDeleteDocumentoOperationCompleted);
            this.InvokeAsync("FascicolazioneDeleteDocumento", new object[2]
      {
        (object) idProfile,
        (object) folder
      }, this.FascicolazioneDeleteDocumentoOperationCompleted, userState);
        }

        private void OnFascicolazioneDeleteDocumentoOperationCompleted(object arg)
        {
            if (this.FascicolazioneDeleteDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneDeleteDocumentoCompleted((object)this, new FascicolazioneDeleteDocumentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetDocumentiPaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public InfoDocumento[] FascicolazioneGetDocumentiPaging(string idGruppo, string idPeople, Folder folder, int numPage, out int numTotPage, out int nRec)
        {
            object[] objArray = this.Invoke("FascicolazioneGetDocumentiPaging", new object[4]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) folder,
        (object) numPage
      });
            numTotPage = (int)objArray[1];
            nRec = (int)objArray[2];
            return (InfoDocumento[])objArray[0];
        }

        public IAsyncResult BeginFascicolazioneGetDocumentiPaging(string idGruppo, string idPeople, Folder folder, int numPage, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetDocumentiPaging", new object[4]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) folder,
        (object) numPage
      }, callback, asyncState);
        }

        public InfoDocumento[] EndFascicolazioneGetDocumentiPaging(IAsyncResult asyncResult, out int numTotPage, out int nRec)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            numTotPage = (int)objArray[1];
            nRec = (int)objArray[2];
            return (InfoDocumento[])objArray[0];
        }

        public void FascicolazioneGetDocumentiPagingAsync(string idGruppo, string idPeople, Folder folder, int numPage)
        {
            this.FascicolazioneGetDocumentiPagingAsync(idGruppo, idPeople, folder, numPage, (object)null);
        }

        public void FascicolazioneGetDocumentiPagingAsync(string idGruppo, string idPeople, Folder folder, int numPage, object userState)
        {
            if (this.FascicolazioneGetDocumentiPagingOperationCompleted == null)
                this.FascicolazioneGetDocumentiPagingOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetDocumentiPagingOperationCompleted);
            this.InvokeAsync("FascicolazioneGetDocumentiPaging", new object[4]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) folder,
        (object) numPage
      }, this.FascicolazioneGetDocumentiPagingOperationCompleted, userState);
        }

        private void OnFascicolazioneGetDocumentiPagingOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetDocumentiPagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetDocumentiPagingCompleted((object)this, new FascicolazioneGetDocumentiPagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetDocumenti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public InfoDocumento[] FascicolazioneGetDocumenti(string idGruppo, string idPeople, Folder folder)
        {
            return (InfoDocumento[])this.Invoke("FascicolazioneGetDocumenti", new object[3]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) folder
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetDocumenti(string idGruppo, string idPeople, Folder folder, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetDocumenti", new object[3]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) folder
      }, callback, asyncState);
        }

        public InfoDocumento[] EndFascicolazioneGetDocumenti(IAsyncResult asyncResult)
        {
            return (InfoDocumento[])this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetDocumentiAsync(string idGruppo, string idPeople, Folder folder)
        {
            this.FascicolazioneGetDocumentiAsync(idGruppo, idPeople, folder, (object)null);
        }

        public void FascicolazioneGetDocumentiAsync(string idGruppo, string idPeople, Folder folder, object userState)
        {
            if (this.FascicolazioneGetDocumentiOperationCompleted == null)
                this.FascicolazioneGetDocumentiOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetDocumentiOperationCompleted);
            this.InvokeAsync("FascicolazioneGetDocumenti", new object[3]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) folder
      }, this.FascicolazioneGetDocumentiOperationCompleted, userState);
        }

        private void OnFascicolazioneGetDocumentiOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetDocumentiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetDocumentiCompleted((object)this, new FascicolazioneGetDocumentiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetFoldersDocumentFascicolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Folder[] FascicolazioneGetFoldersDocumentFascicolo(string systemIdDocumento, string systemIdFascicolo)
        {
            return (Folder[])this.Invoke("FascicolazioneGetFoldersDocumentFascicolo", new object[2]
      {
        (object) systemIdDocumento,
        (object) systemIdFascicolo
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetFoldersDocumentFascicolo(string systemIdDocumento, string systemIdFascicolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetFoldersDocumentFascicolo", new object[2]
      {
        (object) systemIdDocumento,
        (object) systemIdFascicolo
      }, callback, asyncState);
        }

        public Folder[] EndFascicolazioneGetFoldersDocumentFascicolo(IAsyncResult asyncResult)
        {
            return (Folder[])this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetFoldersDocumentFascicoloAsync(string systemIdDocumento, string systemIdFascicolo)
        {
            this.FascicolazioneGetFoldersDocumentFascicoloAsync(systemIdDocumento, systemIdFascicolo, (object)null);
        }

        public void FascicolazioneGetFoldersDocumentFascicoloAsync(string systemIdDocumento, string systemIdFascicolo, object userState)
        {
            if (this.FascicolazioneGetFoldersDocumentFascicoloOperationCompleted == null)
                this.FascicolazioneGetFoldersDocumentFascicoloOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetFoldersDocumentFascicoloOperationCompleted);
            this.InvokeAsync("FascicolazioneGetFoldersDocumentFascicolo", new object[2]
      {
        (object) systemIdDocumento,
        (object) systemIdFascicolo
      }, this.FascicolazioneGetFoldersDocumentFascicoloOperationCompleted, userState);
        }

        private void OnFascicolazioneGetFoldersDocumentFascicoloOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetFoldersDocumentFascicoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetFoldersDocumentFascicoloCompleted((object)this, new FascicolazioneGetFoldersDocumentFascicoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetFoldersDocument", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Folder[] FascicolazioneGetFoldersDocument(string systemIdDocumento)
        {
            return (Folder[])this.Invoke("FascicolazioneGetFoldersDocument", new object[1]
      {
        (object) systemIdDocumento
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetFoldersDocument(string systemIdDocumento, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetFoldersDocument", new object[1]
      {
        (object) systemIdDocumento
      }, callback, asyncState);
        }

        public Folder[] EndFascicolazioneGetFoldersDocument(IAsyncResult asyncResult)
        {
            return (Folder[])this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetFoldersDocumentAsync(string systemIdDocumento)
        {
            this.FascicolazioneGetFoldersDocumentAsync(systemIdDocumento, (object)null);
        }

        public void FascicolazioneGetFoldersDocumentAsync(string systemIdDocumento, object userState)
        {
            if (this.FascicolazioneGetFoldersDocumentOperationCompleted == null)
                this.FascicolazioneGetFoldersDocumentOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetFoldersDocumentOperationCompleted);
            this.InvokeAsync("FascicolazioneGetFoldersDocument", new object[1]
      {
        (object) systemIdDocumento
      }, this.FascicolazioneGetFoldersDocumentOperationCompleted, userState);
        }

        private void OnFascicolazioneGetFoldersDocumentOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetFoldersDocumentCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetFoldersDocumentCompleted((object)this, new FascicolazioneGetFoldersDocumentCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetFolder", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Folder FascicolazioneGetFolder(string idPeople, string idGruppo, Fascicolo fascicolo)
        {
            return (Folder)this.Invoke("FascicolazioneGetFolder", new object[3]
      {
        (object) idPeople,
        (object) idGruppo,
        (object) fascicolo
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetFolder(string idPeople, string idGruppo, Fascicolo fascicolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetFolder", new object[3]
      {
        (object) idPeople,
        (object) idGruppo,
        (object) fascicolo
      }, callback, asyncState);
        }

        public Folder EndFascicolazioneGetFolder(IAsyncResult asyncResult)
        {
            return (Folder)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetFolderAsync(string idPeople, string idGruppo, Fascicolo fascicolo)
        {
            this.FascicolazioneGetFolderAsync(idPeople, idGruppo, fascicolo, (object)null);
        }

        public void FascicolazioneGetFolderAsync(string idPeople, string idGruppo, Fascicolo fascicolo, object userState)
        {
            if (this.FascicolazioneGetFolderOperationCompleted == null)
                this.FascicolazioneGetFolderOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetFolderOperationCompleted);
            this.InvokeAsync("FascicolazioneGetFolder", new object[3]
      {
        (object) idPeople,
        (object) idGruppo,
        (object) fascicolo
      }, this.FascicolazioneGetFolderOperationCompleted, userState);
        }

        private void OnFascicolazioneGetFolderOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetFolderCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetFolderCompleted((object)this, new FascicolazioneGetFolderCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneNewFascicoloLF", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo FascicolazioneNewFascicoloLF(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo, bool LF, string idUoLF, string dtaLF, bool enableUffRef)
        {
            return (Fascicolo)this.Invoke("FascicolazioneNewFascicoloLF", new object[8]
      {
        (object) classificazione,
        (object) fascicolo,
        (object) infoUtente,
        (object) ruolo,
        (object) LF,
        (object) idUoLF,
        (object) dtaLF,
        (object) enableUffRef
      })[0];
        }

        public IAsyncResult BeginFascicolazioneNewFascicoloLF(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo, bool LF, string idUoLF, string dtaLF, bool enableUffRef, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneNewFascicoloLF", new object[8]
      {
        (object) classificazione,
        (object) fascicolo,
        (object) infoUtente,
        (object) ruolo,
        (object) LF,
        (object) idUoLF,
        (object) dtaLF,
        (object) enableUffRef
      }, callback, asyncState);
        }

        public Fascicolo EndFascicolazioneNewFascicoloLF(IAsyncResult asyncResult)
        {
            return (Fascicolo)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneNewFascicoloLFAsync(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo, bool LF, string idUoLF, string dtaLF, bool enableUffRef)
        {
            this.FascicolazioneNewFascicoloLFAsync(classificazione, fascicolo, infoUtente, ruolo, LF, idUoLF, dtaLF, enableUffRef, (object)null);
        }

        public void FascicolazioneNewFascicoloLFAsync(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo, bool LF, string idUoLF, string dtaLF, bool enableUffRef, object userState)
        {
            if (this.FascicolazioneNewFascicoloLFOperationCompleted == null)
                this.FascicolazioneNewFascicoloLFOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneNewFascicoloLFOperationCompleted);
            this.InvokeAsync("FascicolazioneNewFascicoloLF", new object[8]
      {
        (object) classificazione,
        (object) fascicolo,
        (object) infoUtente,
        (object) ruolo,
        (object) LF,
        (object) idUoLF,
        (object) dtaLF,
        (object) enableUffRef
      }, this.FascicolazioneNewFascicoloLFOperationCompleted, userState);
        }

        private void OnFascicolazioneNewFascicoloLFOperationCompleted(object arg)
        {
            if (this.FascicolazioneNewFascicoloLFCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneNewFascicoloLFCompleted((object)this, new FascicolazioneNewFascicoloLFCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneNewFascicolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo FascicolazioneNewFascicolo(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo, bool enableUffRef)
        {
            return (Fascicolo)this.Invoke("FascicolazioneNewFascicolo", new object[5]
      {
        (object) classificazione,
        (object) fascicolo,
        (object) infoUtente,
        (object) ruolo,
        (object) enableUffRef
      })[0];
        }

        public IAsyncResult BeginFascicolazioneNewFascicolo(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo, bool enableUffRef, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneNewFascicolo", new object[5]
      {
        (object) classificazione,
        (object) fascicolo,
        (object) infoUtente,
        (object) ruolo,
        (object) enableUffRef
      }, callback, asyncState);
        }

        public Fascicolo EndFascicolazioneNewFascicolo(IAsyncResult asyncResult)
        {
            return (Fascicolo)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneNewFascicoloAsync(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo, bool enableUffRef)
        {
            this.FascicolazioneNewFascicoloAsync(classificazione, fascicolo, infoUtente, ruolo, enableUffRef, (object)null);
        }

        public void FascicolazioneNewFascicoloAsync(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo, bool enableUffRef, object userState)
        {
            if (this.FascicolazioneNewFascicoloOperationCompleted == null)
                this.FascicolazioneNewFascicoloOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneNewFascicoloOperationCompleted);
            this.InvokeAsync("FascicolazioneNewFascicolo", new object[5]
      {
        (object) classificazione,
        (object) fascicolo,
        (object) infoUtente,
        (object) ruolo,
        (object) enableUffRef
      }, this.FascicolazioneNewFascicoloOperationCompleted, userState);
        }

        private void OnFascicolazioneNewFascicoloOperationCompleted(object arg)
        {
            if (this.FascicolazioneNewFascicoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneNewFascicoloCompleted((object)this, new FascicolazioneNewFascicoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetListaFascicoliPaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo[] FascicolazioneGetListaFascicoliPaging(string idAmministrazione, string idGruppo, string idPeople, FascicolazioneClassificazione classificazione, Registro registro, FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool childs, int numPage, out int numTotPage, out int nRec)
        {
            object[] objArray = this.Invoke("FascicolazioneGetListaFascicoliPaging", new object[9]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) classificazione,
        (object) registro,
        (object) listaFiltri,
        (object) enableUfficioRef,
        (object) childs,
        (object) numPage
      });
            numTotPage = (int)objArray[1];
            nRec = (int)objArray[2];
            return (Fascicolo[])objArray[0];
        }

        public IAsyncResult BeginFascicolazioneGetListaFascicoliPaging(string idAmministrazione, string idGruppo, string idPeople, FascicolazioneClassificazione classificazione, Registro registro, FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool childs, int numPage, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetListaFascicoliPaging", new object[9]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) classificazione,
        (object) registro,
        (object) listaFiltri,
        (object) enableUfficioRef,
        (object) childs,
        (object) numPage
      }, callback, asyncState);
        }

        public Fascicolo[] EndFascicolazioneGetListaFascicoliPaging(IAsyncResult asyncResult, out int numTotPage, out int nRec)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            numTotPage = (int)objArray[1];
            nRec = (int)objArray[2];
            return (Fascicolo[])objArray[0];
        }

        public void FascicolazioneGetListaFascicoliPagingAsync(string idAmministrazione, string idGruppo, string idPeople, FascicolazioneClassificazione classificazione, Registro registro, FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool childs, int numPage)
        {
            this.FascicolazioneGetListaFascicoliPagingAsync(idAmministrazione, idGruppo, idPeople, classificazione, registro, listaFiltri, enableUfficioRef, childs, numPage, (object)null);
        }

        public void FascicolazioneGetListaFascicoliPagingAsync(string idAmministrazione, string idGruppo, string idPeople, FascicolazioneClassificazione classificazione, Registro registro, FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool childs, int numPage, object userState)
        {
            if (this.FascicolazioneGetListaFascicoliPagingOperationCompleted == null)
                this.FascicolazioneGetListaFascicoliPagingOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetListaFascicoliPagingOperationCompleted);
            this.InvokeAsync("FascicolazioneGetListaFascicoliPaging", new object[9]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) classificazione,
        (object) registro,
        (object) listaFiltri,
        (object) enableUfficioRef,
        (object) childs,
        (object) numPage
      }, this.FascicolazioneGetListaFascicoliPagingOperationCompleted, userState);
        }

        private void OnFascicolazioneGetListaFascicoliPagingOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetListaFascicoliPagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetListaFascicoliPagingCompleted((object)this, new FascicolazioneGetListaFascicoliPagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetListaFascicoli", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo[] FascicolazioneGetListaFascicoli(string idAmministrazione, string idGruppo, string idPeople, FascicolazioneClassificazione classificazione, FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool childs)
        {
            return (Fascicolo[])this.Invoke("FascicolazioneGetListaFascicoli", new object[7]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) classificazione,
        (object) listaFiltri,
        (object) enableUfficioRef,
        (object) childs
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetListaFascicoli(string idAmministrazione, string idGruppo, string idPeople, FascicolazioneClassificazione classificazione, FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool childs, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetListaFascicoli", new object[7]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) classificazione,
        (object) listaFiltri,
        (object) enableUfficioRef,
        (object) childs
      }, callback, asyncState);
        }

        public Fascicolo[] EndFascicolazioneGetListaFascicoli(IAsyncResult asyncResult)
        {
            return (Fascicolo[])this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetListaFascicoliAsync(string idAmministrazione, string idGruppo, string idPeople, FascicolazioneClassificazione classificazione, FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool childs)
        {
            this.FascicolazioneGetListaFascicoliAsync(idAmministrazione, idGruppo, idPeople, classificazione, listaFiltri, enableUfficioRef, childs, (object)null);
        }

        public void FascicolazioneGetListaFascicoliAsync(string idAmministrazione, string idGruppo, string idPeople, FascicolazioneClassificazione classificazione, FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool childs, object userState)
        {
            if (this.FascicolazioneGetListaFascicoliOperationCompleted == null)
                this.FascicolazioneGetListaFascicoliOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetListaFascicoliOperationCompleted);
            this.InvokeAsync("FascicolazioneGetListaFascicoli", new object[7]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) classificazione,
        (object) listaFiltri,
        (object) enableUfficioRef,
        (object) childs
      }, this.FascicolazioneGetListaFascicoliOperationCompleted, userState);
        }

        private void OnFascicolazioneGetListaFascicoliOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetListaFascicoliCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetListaFascicoliCompleted((object)this, new FascicolazioneGetListaFascicoliCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetAllListaFasc", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo[] FascicolazioneGetAllListaFasc(string idAmministrazione, string idGruppo, string idPeople, FascicolazioneClassificazione classificazione, FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool childs, Registro registro)
        {
            return (Fascicolo[])this.Invoke("FascicolazioneGetAllListaFasc", new object[8]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) classificazione,
        (object) listaFiltri,
        (object) enableUfficioRef,
        (object) childs,
        (object) registro
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetAllListaFasc(string idAmministrazione, string idGruppo, string idPeople, FascicolazioneClassificazione classificazione, FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool childs, Registro registro, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetAllListaFasc", new object[8]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) classificazione,
        (object) listaFiltri,
        (object) enableUfficioRef,
        (object) childs,
        (object) registro
      }, callback, asyncState);
        }

        public Fascicolo[] EndFascicolazioneGetAllListaFasc(IAsyncResult asyncResult)
        {
            return (Fascicolo[])this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetAllListaFascAsync(string idAmministrazione, string idGruppo, string idPeople, FascicolazioneClassificazione classificazione, FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool childs, Registro registro)
        {
            this.FascicolazioneGetAllListaFascAsync(idAmministrazione, idGruppo, idPeople, classificazione, listaFiltri, enableUfficioRef, childs, registro, (object)null);
        }

        public void FascicolazioneGetAllListaFascAsync(string idAmministrazione, string idGruppo, string idPeople, FascicolazioneClassificazione classificazione, FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool childs, Registro registro, object userState)
        {
            if (this.FascicolazioneGetAllListaFascOperationCompleted == null)
                this.FascicolazioneGetAllListaFascOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetAllListaFascOperationCompleted);
            this.InvokeAsync("FascicolazioneGetAllListaFasc", new object[8]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) classificazione,
        (object) listaFiltri,
        (object) enableUfficioRef,
        (object) childs,
        (object) registro
      }, this.FascicolazioneGetAllListaFascOperationCompleted, userState);
        }

        private void OnFascicolazioneGetAllListaFascOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetAllListaFascCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetAllListaFascCompleted((object)this, new FascicolazioneGetAllListaFascCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneSetAutorizzazioniNodoTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool FascicolazioneSetAutorizzazioniNodoTitolario(FascicolazioneClassificazione nodoTitolario, Corrispondente[] corrAdd, Corrispondente[] corrRemove, bool ereditaDiritti)
        {
            return (bool)this.Invoke("FascicolazioneSetAutorizzazioniNodoTitolario", new object[4]
      {
        (object) nodoTitolario,
        (object) corrAdd,
        (object) corrRemove,
        (object) ereditaDiritti
      })[0];
        }

        public IAsyncResult BeginFascicolazioneSetAutorizzazioniNodoTitolario(FascicolazioneClassificazione nodoTitolario, Corrispondente[] corrAdd, Corrispondente[] corrRemove, bool ereditaDiritti, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneSetAutorizzazioniNodoTitolario", new object[4]
      {
        (object) nodoTitolario,
        (object) corrAdd,
        (object) corrRemove,
        (object) ereditaDiritti
      }, callback, asyncState);
        }

        public bool EndFascicolazioneSetAutorizzazioniNodoTitolario(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneSetAutorizzazioniNodoTitolarioAsync(FascicolazioneClassificazione nodoTitolario, Corrispondente[] corrAdd, Corrispondente[] corrRemove, bool ereditaDiritti)
        {
            this.FascicolazioneSetAutorizzazioniNodoTitolarioAsync(nodoTitolario, corrAdd, corrRemove, ereditaDiritti, (object)null);
        }

        public void FascicolazioneSetAutorizzazioniNodoTitolarioAsync(FascicolazioneClassificazione nodoTitolario, Corrispondente[] corrAdd, Corrispondente[] corrRemove, bool ereditaDiritti, object userState)
        {
            if (this.FascicolazioneSetAutorizzazioniNodoTitolarioOperationCompleted == null)
                this.FascicolazioneSetAutorizzazioniNodoTitolarioOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneSetAutorizzazioniNodoTitolarioOperationCompleted);
            this.InvokeAsync("FascicolazioneSetAutorizzazioniNodoTitolario", new object[4]
      {
        (object) nodoTitolario,
        (object) corrAdd,
        (object) corrRemove,
        (object) ereditaDiritti
      }, this.FascicolazioneSetAutorizzazioniNodoTitolarioOperationCompleted, userState);
        }

        private void OnFascicolazioneSetAutorizzazioniNodoTitolarioOperationCompleted(object arg)
        {
            if (this.FascicolazioneSetAutorizzazioniNodoTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneSetAutorizzazioniNodoTitolarioCompleted((object)this, new FascicolazioneSetAutorizzazioniNodoTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetAutorizzazioniNodoTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicoloDiritto[] FascicolazioneGetAutorizzazioniNodoTitolario(FascicolazioneClassificazione nodoTitolario)
        {
            return (FascicoloDiritto[])this.Invoke("FascicolazioneGetAutorizzazioniNodoTitolario", new object[1]
      {
        (object) nodoTitolario
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetAutorizzazioniNodoTitolario(FascicolazioneClassificazione nodoTitolario, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetAutorizzazioniNodoTitolario", new object[1]
      {
        (object) nodoTitolario
      }, callback, asyncState);
        }

        public FascicoloDiritto[] EndFascicolazioneGetAutorizzazioniNodoTitolario(IAsyncResult asyncResult)
        {
            return (FascicoloDiritto[])this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetAutorizzazioniNodoTitolarioAsync(FascicolazioneClassificazione nodoTitolario)
        {
            this.FascicolazioneGetAutorizzazioniNodoTitolarioAsync(nodoTitolario, (object)null);
        }

        public void FascicolazioneGetAutorizzazioniNodoTitolarioAsync(FascicolazioneClassificazione nodoTitolario, object userState)
        {
            if (this.FascicolazioneGetAutorizzazioniNodoTitolarioOperationCompleted == null)
                this.FascicolazioneGetAutorizzazioniNodoTitolarioOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetAutorizzazioniNodoTitolarioOperationCompleted);
            this.InvokeAsync("FascicolazioneGetAutorizzazioniNodoTitolario", new object[1]
      {
        (object) nodoTitolario
      }, this.FascicolazioneGetAutorizzazioniNodoTitolarioOperationCompleted, userState);
        }

        private void OnFascicolazioneGetAutorizzazioniNodoTitolarioOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetAutorizzazioniNodoTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetAutorizzazioniNodoTitolarioCompleted((object)this, new FascicolazioneGetAutorizzazioniNodoTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneCanRemoveFascicolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool FascicolazioneCanRemoveFascicolo(string project_Id, out string nFasc)
        {
            object[] objArray = this.Invoke("FascicolazioneCanRemoveFascicolo", new object[1]
      {
        (object) project_Id
      });
            nFasc = (string)objArray[1];
            return (bool)objArray[0];
        }

        public IAsyncResult BeginFascicolazioneCanRemoveFascicolo(string project_Id, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneCanRemoveFascicolo", new object[1]
      {
        (object) project_Id
      }, callback, asyncState);
        }

        public bool EndFascicolazioneCanRemoveFascicolo(IAsyncResult asyncResult, out string nFasc)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            nFasc = (string)objArray[1];
            return (bool)objArray[0];
        }

        public void FascicolazioneCanRemoveFascicoloAsync(string project_Id)
        {
            this.FascicolazioneCanRemoveFascicoloAsync(project_Id, (object)null);
        }

        public void FascicolazioneCanRemoveFascicoloAsync(string project_Id, object userState)
        {
            if (this.FascicolazioneCanRemoveFascicoloOperationCompleted == null)
                this.FascicolazioneCanRemoveFascicoloOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneCanRemoveFascicoloOperationCompleted);
            this.InvokeAsync("FascicolazioneCanRemoveFascicolo", new object[1]
      {
        (object) project_Id
      }, this.FascicolazioneCanRemoveFascicoloOperationCompleted, userState);
        }

        private void OnFascicolazioneCanRemoveFascicoloOperationCompleted(object arg)
        {
            if (this.FascicolazioneCanRemoveFascicoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneCanRemoveFascicoloCompleted((object)this, new FascicolazioneCanRemoveFascicoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetCodiceFiglioTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string FascicolazioneGetCodiceFiglioTitolario(string idAmministrazione, FascicolazioneClassificazione nodoTitolario)
        {
            return (string)this.Invoke("FascicolazioneGetCodiceFiglioTitolario", new object[2]
      {
        (object) idAmministrazione,
        (object) nodoTitolario
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetCodiceFiglioTitolario(string idAmministrazione, FascicolazioneClassificazione nodoTitolario, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetCodiceFiglioTitolario", new object[2]
      {
        (object) idAmministrazione,
        (object) nodoTitolario
      }, callback, asyncState);
        }

        public string EndFascicolazioneGetCodiceFiglioTitolario(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetCodiceFiglioTitolarioAsync(string idAmministrazione, FascicolazioneClassificazione nodoTitolario)
        {
            this.FascicolazioneGetCodiceFiglioTitolarioAsync(idAmministrazione, nodoTitolario, (object)null);
        }

        public void FascicolazioneGetCodiceFiglioTitolarioAsync(string idAmministrazione, FascicolazioneClassificazione nodoTitolario, object userState)
        {
            if (this.FascicolazioneGetCodiceFiglioTitolarioOperationCompleted == null)
                this.FascicolazioneGetCodiceFiglioTitolarioOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetCodiceFiglioTitolarioOperationCompleted);
            this.InvokeAsync("FascicolazioneGetCodiceFiglioTitolario", new object[2]
      {
        (object) idAmministrazione,
        (object) nodoTitolario
      }, this.FascicolazioneGetCodiceFiglioTitolarioOperationCompleted, userState);
        }

        private void OnFascicolazioneGetCodiceFiglioTitolarioOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetCodiceFiglioTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetCodiceFiglioTitolarioCompleted((object)this, new FascicolazioneGetCodiceFiglioTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneNewTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicolazioneClassificazione FascicolazioneNewTitolario(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario, string idParent, bool ereditaDiritti)
        {
            return (FascicolazioneClassificazione)this.Invoke("FascicolazioneNewTitolario", new object[4]
      {
        (object) infoUtente,
        (object) nodoTitolario,
        (object) idParent,
        (object) ereditaDiritti
      })[0];
        }

        public IAsyncResult BeginFascicolazioneNewTitolario(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario, string idParent, bool ereditaDiritti, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneNewTitolario", new object[4]
      {
        (object) infoUtente,
        (object) nodoTitolario,
        (object) idParent,
        (object) ereditaDiritti
      }, callback, asyncState);
        }

        public FascicolazioneClassificazione EndFascicolazioneNewTitolario(IAsyncResult asyncResult)
        {
            return (FascicolazioneClassificazione)this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneNewTitolarioAsync(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario, string idParent, bool ereditaDiritti)
        {
            this.FascicolazioneNewTitolarioAsync(infoUtente, nodoTitolario, idParent, ereditaDiritti, (object)null);
        }

        public void FascicolazioneNewTitolarioAsync(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario, string idParent, bool ereditaDiritti, object userState)
        {
            if (this.FascicolazioneNewTitolarioOperationCompleted == null)
                this.FascicolazioneNewTitolarioOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneNewTitolarioOperationCompleted);
            this.InvokeAsync("FascicolazioneNewTitolario", new object[4]
      {
        (object) infoUtente,
        (object) nodoTitolario,
        (object) idParent,
        (object) ereditaDiritti
      }, this.FascicolazioneNewTitolarioOperationCompleted, userState);
        }

        private void OnFascicolazioneNewTitolarioOperationCompleted(object arg)
        {
            if (this.FascicolazioneNewTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneNewTitolarioCompleted((object)this, new FascicolazioneNewTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/FascicolazioneGetTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicolazioneClassificazione[] FascicolazioneGetTitolario(string idAmministrazione, string idGruppo, string idPeople, Registro registro, string codiceClassifica, bool getFigli)
        {
            return (FascicolazioneClassificazione[])this.Invoke("FascicolazioneGetTitolario", new object[6]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) registro,
        (object) codiceClassifica,
        (object) getFigli
      })[0];
        }

        public IAsyncResult BeginFascicolazioneGetTitolario(string idAmministrazione, string idGruppo, string idPeople, Registro registro, string codiceClassifica, bool getFigli, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("FascicolazioneGetTitolario", new object[6]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) registro,
        (object) codiceClassifica,
        (object) getFigli
      }, callback, asyncState);
        }

        public FascicolazioneClassificazione[] EndFascicolazioneGetTitolario(IAsyncResult asyncResult)
        {
            return (FascicolazioneClassificazione[])this.EndInvoke(asyncResult)[0];
        }

        public void FascicolazioneGetTitolarioAsync(string idAmministrazione, string idGruppo, string idPeople, Registro registro, string codiceClassifica, bool getFigli)
        {
            this.FascicolazioneGetTitolarioAsync(idAmministrazione, idGruppo, idPeople, registro, codiceClassifica, getFigli, (object)null);
        }

        public void FascicolazioneGetTitolarioAsync(string idAmministrazione, string idGruppo, string idPeople, Registro registro, string codiceClassifica, bool getFigli, object userState)
        {
            if (this.FascicolazioneGetTitolarioOperationCompleted == null)
                this.FascicolazioneGetTitolarioOperationCompleted = new SendOrPostCallback(this.OnFascicolazioneGetTitolarioOperationCompleted);
            this.InvokeAsync("FascicolazioneGetTitolario", new object[6]
      {
        (object) idAmministrazione,
        (object) idGruppo,
        (object) idPeople,
        (object) registro,
        (object) codiceClassifica,
        (object) getFigli
      }, this.FascicolazioneGetTitolarioOperationCompleted, userState);
        }

        private void OnFascicolazioneGetTitolarioOperationCompleted(object arg)
        {
            if (this.FascicolazioneGetTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.FascicolazioneGetTitolarioCompleted((object)this, new FascicolazioneGetTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoCercaDuplicati", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DocumentoCercaDuplicati(SchedaDocumento schedaDocumento)
        {
            return (bool)this.Invoke("DocumentoCercaDuplicati", new object[1]
      {
        (object) schedaDocumento
      })[0];
        }

        public IAsyncResult BeginDocumentoCercaDuplicati(SchedaDocumento schedaDocumento, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoCercaDuplicati", new object[1]
      {
        (object) schedaDocumento
      }, callback, asyncState);
        }

        public bool EndDocumentoCercaDuplicati(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoCercaDuplicatiAsync(SchedaDocumento schedaDocumento)
        {
            this.DocumentoCercaDuplicatiAsync(schedaDocumento, (object)null);
        }

        public void DocumentoCercaDuplicatiAsync(SchedaDocumento schedaDocumento, object userState)
        {
            if (this.DocumentoCercaDuplicatiOperationCompleted == null)
                this.DocumentoCercaDuplicatiOperationCompleted = new SendOrPostCallback(this.OnDocumentoCercaDuplicatiOperationCompleted);
            this.InvokeAsync("DocumentoCercaDuplicati", new object[1]
      {
        (object) schedaDocumento
      }, this.DocumentoCercaDuplicatiOperationCompleted, userState);
        }

        private void OnDocumentoCercaDuplicatiOperationCompleted(object arg)
        {
            if (this.DocumentoCercaDuplicatiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoCercaDuplicatiCompleted((object)this, new DocumentoCercaDuplicatiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetCatenaDoc", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public AnelloDocumentale DocumentoGetCatenaDoc(string idGruppo, string idPeople, string idProfile)
        {
            return (AnelloDocumentale)this.Invoke("DocumentoGetCatenaDoc", new object[3]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) idProfile
      })[0];
        }

        public IAsyncResult BeginDocumentoGetCatenaDoc(string idGruppo, string idPeople, string idProfile, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetCatenaDoc", new object[3]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) idProfile
      }, callback, asyncState);
        }

        public AnelloDocumentale EndDocumentoGetCatenaDoc(IAsyncResult asyncResult)
        {
            return (AnelloDocumentale)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoGetCatenaDocAsync(string idGruppo, string idPeople, string idProfile)
        {
            this.DocumentoGetCatenaDocAsync(idGruppo, idPeople, idProfile, (object)null);
        }

        public void DocumentoGetCatenaDocAsync(string idGruppo, string idPeople, string idProfile, object userState)
        {
            if (this.DocumentoGetCatenaDocOperationCompleted == null)
                this.DocumentoGetCatenaDocOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetCatenaDocOperationCompleted);
            this.InvokeAsync("DocumentoGetCatenaDoc", new object[3]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) idProfile
      }, this.DocumentoGetCatenaDocOperationCompleted, userState);
        }

        private void OnDocumentoGetCatenaDocOperationCompleted(object arg)
        {
            if (this.DocumentoGetCatenaDocCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetCatenaDocCompleted((object)this, new DocumentoGetCatenaDocCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoAddTipologiaAtto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TipologiaAtto DocumentoAddTipologiaAtto(TipologiaAtto tipologiaAtto, InfoUtente infoUtente)
        {
            return (TipologiaAtto)this.Invoke("DocumentoAddTipologiaAtto", new object[2]
      {
        (object) tipologiaAtto,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginDocumentoAddTipologiaAtto(TipologiaAtto tipologiaAtto, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoAddTipologiaAtto", new object[2]
      {
        (object) tipologiaAtto,
        (object) infoUtente
      }, callback, asyncState);
        }

        public TipologiaAtto EndDocumentoAddTipologiaAtto(IAsyncResult asyncResult)
        {
            return (TipologiaAtto)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoAddTipologiaAttoAsync(TipologiaAtto tipologiaAtto, InfoUtente infoUtente)
        {
            this.DocumentoAddTipologiaAttoAsync(tipologiaAtto, infoUtente, (object)null);
        }

        public void DocumentoAddTipologiaAttoAsync(TipologiaAtto tipologiaAtto, InfoUtente infoUtente, object userState)
        {
            if (this.DocumentoAddTipologiaAttoOperationCompleted == null)
                this.DocumentoAddTipologiaAttoOperationCompleted = new SendOrPostCallback(this.OnDocumentoAddTipologiaAttoOperationCompleted);
            this.InvokeAsync("DocumentoAddTipologiaAtto", new object[2]
      {
        (object) tipologiaAtto,
        (object) infoUtente
      }, this.DocumentoAddTipologiaAttoOperationCompleted, userState);
        }

        private void OnDocumentoAddTipologiaAttoOperationCompleted(object arg)
        {
            if (this.DocumentoAddTipologiaAttoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoAddTipologiaAttoCompleted((object)this, new DocumentoAddTipologiaAttoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoAddParolaChiave", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DocumentoParolaChiave DocumentoAddParolaChiave(string idAmministrazione, DocumentoParolaChiave parolaChiave)
        {
            return (DocumentoParolaChiave)this.Invoke("DocumentoAddParolaChiave", new object[2]
      {
        (object) idAmministrazione,
        (object) parolaChiave
      })[0];
        }

        public IAsyncResult BeginDocumentoAddParolaChiave(string idAmministrazione, DocumentoParolaChiave parolaChiave, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoAddParolaChiave", new object[2]
      {
        (object) idAmministrazione,
        (object) parolaChiave
      }, callback, asyncState);
        }

        public DocumentoParolaChiave EndDocumentoAddParolaChiave(IAsyncResult asyncResult)
        {
            return (DocumentoParolaChiave)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoAddParolaChiaveAsync(string idAmministrazione, DocumentoParolaChiave parolaChiave)
        {
            this.DocumentoAddParolaChiaveAsync(idAmministrazione, parolaChiave, (object)null);
        }

        public void DocumentoAddParolaChiaveAsync(string idAmministrazione, DocumentoParolaChiave parolaChiave, object userState)
        {
            if (this.DocumentoAddParolaChiaveOperationCompleted == null)
                this.DocumentoAddParolaChiaveOperationCompleted = new SendOrPostCallback(this.OnDocumentoAddParolaChiaveOperationCompleted);
            this.InvokeAsync("DocumentoAddParolaChiave", new object[2]
      {
        (object) idAmministrazione,
        (object) parolaChiave
      }, this.DocumentoAddParolaChiaveOperationCompleted, userState);
        }

        private void OnDocumentoAddParolaChiaveOperationCompleted(object arg)
        {
            if (this.DocumentoAddParolaChiaveCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoAddParolaChiaveCompleted((object)this, new DocumentoAddParolaChiaveCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoAddOggetto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Oggetto DocumentoAddOggetto(string idAmministrazione, Oggetto oggetto, Registro registro)
        {
            return (Oggetto)this.Invoke("DocumentoAddOggetto", new object[3]
      {
        (object) idAmministrazione,
        (object) oggetto,
        (object) registro
      })[0];
        }

        public IAsyncResult BeginDocumentoAddOggetto(string idAmministrazione, Oggetto oggetto, Registro registro, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoAddOggetto", new object[3]
      {
        (object) idAmministrazione,
        (object) oggetto,
        (object) registro
      }, callback, asyncState);
        }

        public Oggetto EndDocumentoAddOggetto(IAsyncResult asyncResult)
        {
            return (Oggetto)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoAddOggettoAsync(string idAmministrazione, Oggetto oggetto, Registro registro)
        {
            this.DocumentoAddOggettoAsync(idAmministrazione, oggetto, registro, (object)null);
        }

        public void DocumentoAddOggettoAsync(string idAmministrazione, Oggetto oggetto, Registro registro, object userState)
        {
            if (this.DocumentoAddOggettoOperationCompleted == null)
                this.DocumentoAddOggettoOperationCompleted = new SendOrPostCallback(this.OnDocumentoAddOggettoOperationCompleted);
            this.InvokeAsync("DocumentoAddOggetto", new object[3]
      {
        (object) idAmministrazione,
        (object) oggetto,
        (object) registro
      }, this.DocumentoAddOggettoOperationCompleted, userState);
        }

        private void OnDocumentoAddOggettoOperationCompleted(object arg)
        {
            if (this.DocumentoAddOggettoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoAddOggettoCompleted((object)this, new DocumentoAddOggettoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoSetFlagDaInviare", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento DocumentoSetFlagDaInviare(SchedaDocumento schedaDocumento)
        {
            return (SchedaDocumento)this.Invoke("DocumentoSetFlagDaInviare", new object[1]
      {
        (object) schedaDocumento
      })[0];
        }

        public IAsyncResult BeginDocumentoSetFlagDaInviare(SchedaDocumento schedaDocumento, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoSetFlagDaInviare", new object[1]
      {
        (object) schedaDocumento
      }, callback, asyncState);
        }

        public SchedaDocumento EndDocumentoSetFlagDaInviare(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoSetFlagDaInviareAsync(SchedaDocumento schedaDocumento)
        {
            this.DocumentoSetFlagDaInviareAsync(schedaDocumento, (object)null);
        }

        public void DocumentoSetFlagDaInviareAsync(SchedaDocumento schedaDocumento, object userState)
        {
            if (this.DocumentoSetFlagDaInviareOperationCompleted == null)
                this.DocumentoSetFlagDaInviareOperationCompleted = new SendOrPostCallback(this.OnDocumentoSetFlagDaInviareOperationCompleted);
            this.InvokeAsync("DocumentoSetFlagDaInviare", new object[1]
      {
        (object) schedaDocumento
      }, this.DocumentoSetFlagDaInviareOperationCompleted, userState);
        }

        private void OnDocumentoSetFlagDaInviareOperationCompleted(object arg)
        {
            if (this.DocumentoSetFlagDaInviareCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoSetFlagDaInviareCompleted((object)this, new DocumentoSetFlagDaInviareCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetListaStoriciOggetto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DocumentoStoricoOggetto[] DocumentoGetListaStoriciOggetto(string idProfile)
        {
            return (DocumentoStoricoOggetto[])this.Invoke("DocumentoGetListaStoriciOggetto", new object[1]
      {
        (object) idProfile
      })[0];
        }

        public IAsyncResult BeginDocumentoGetListaStoriciOggetto(string idProfile, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetListaStoriciOggetto", new object[1]
      {
        (object) idProfile
      }, callback, asyncState);
        }

        public DocumentoStoricoOggetto[] EndDocumentoGetListaStoriciOggetto(IAsyncResult asyncResult)
        {
            return (DocumentoStoricoOggetto[])this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoGetListaStoriciOggettoAsync(string idProfile)
        {
            this.DocumentoGetListaStoriciOggettoAsync(idProfile, (object)null);
        }

        public void DocumentoGetListaStoriciOggettoAsync(string idProfile, object userState)
        {
            if (this.DocumentoGetListaStoriciOggettoOperationCompleted == null)
                this.DocumentoGetListaStoriciOggettoOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetListaStoriciOggettoOperationCompleted);
            this.InvokeAsync("DocumentoGetListaStoriciOggetto", new object[1]
      {
        (object) idProfile
      }, this.DocumentoGetListaStoriciOggettoOperationCompleted, userState);
        }

        private void OnDocumentoGetListaStoriciOggettoOperationCompleted(object arg)
        {
            if (this.DocumentoGetListaStoriciOggettoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetListaStoriciOggettoCompleted((object)this, new DocumentoGetListaStoriciOggettoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoVerificaFirma", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FirmaResult DocumentoVerificaFirma(string base64content, FileRequest fileReq, bool cofirma, InfoUtente infoUtente)
        {
            return (FirmaResult)this.Invoke("DocumentoVerificaFirma", new object[4]
      {
        (object) base64content,
        (object) fileReq,
        (object) cofirma,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginDocumentoVerificaFirma(string base64content, FileRequest fileReq, bool cofirma, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoVerificaFirma", new object[4]
      {
        (object) base64content,
        (object) fileReq,
        (object) cofirma,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FirmaResult EndDocumentoVerificaFirma(IAsyncResult asyncResult)
        {
            return (FirmaResult)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoVerificaFirmaAsync(string base64content, FileRequest fileReq, bool cofirma, InfoUtente infoUtente)
        {
            this.DocumentoVerificaFirmaAsync(base64content, fileReq, cofirma, infoUtente, (object)null);
        }

        public void DocumentoVerificaFirmaAsync(string base64content, FileRequest fileReq, bool cofirma, InfoUtente infoUtente, object userState)
        {
            if (this.DocumentoVerificaFirmaOperationCompleted == null)
                this.DocumentoVerificaFirmaOperationCompleted = new SendOrPostCallback(this.OnDocumentoVerificaFirmaOperationCompleted);
            this.InvokeAsync("DocumentoVerificaFirma", new object[4]
      {
        (object) base64content,
        (object) fileReq,
        (object) cofirma,
        (object) infoUtente
      }, this.DocumentoVerificaFirmaOperationCompleted, userState);
        }

        private void OnDocumentoVerificaFirmaOperationCompleted(object arg)
        {
            if (this.DocumentoVerificaFirmaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoVerificaFirmaCompleted((object)this, new DocumentoVerificaFirmaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoSpedisci", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DocumentoSpedisci(SchedaDocumento schedaDocumento, InfoUtente infoUtente, bool confermaRic)
        {
            return (bool)this.Invoke("DocumentoSpedisci", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) confermaRic
      })[0];
        }

        public IAsyncResult BeginDocumentoSpedisci(SchedaDocumento schedaDocumento, InfoUtente infoUtente, bool confermaRic, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoSpedisci", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) confermaRic
      }, callback, asyncState);
        }

        public bool EndDocumentoSpedisci(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoSpedisciAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, bool confermaRic)
        {
            this.DocumentoSpedisciAsync(schedaDocumento, infoUtente, confermaRic, (object)null);
        }

        public void DocumentoSpedisciAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, bool confermaRic, object userState)
        {
            if (this.DocumentoSpedisciOperationCompleted == null)
                this.DocumentoSpedisciOperationCompleted = new SendOrPostCallback(this.OnDocumentoSpedisciOperationCompleted);
            this.InvokeAsync("DocumentoSpedisci", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) confermaRic
      }, this.DocumentoSpedisciOperationCompleted, userState);
        }

        private void OnDocumentoSpedisciOperationCompleted(object arg)
        {
            if (this.DocumentoSpedisciCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoSpedisciCompleted((object)this, new DocumentoSpedisciCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetTipologiaAtto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TipologiaAtto[] DocumentoGetTipologiaAtto()
        {
            return (TipologiaAtto[])this.Invoke("DocumentoGetTipologiaAtto", new object[0])[0];
        }

        public IAsyncResult BeginDocumentoGetTipologiaAtto(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetTipologiaAtto", new object[0], callback, asyncState);
        }

        public TipologiaAtto[] EndDocumentoGetTipologiaAtto(IAsyncResult asyncResult)
        {
            return (TipologiaAtto[])this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoGetTipologiaAttoAsync()
        {
            this.DocumentoGetTipologiaAttoAsync((object)null);
        }

        public void DocumentoGetTipologiaAttoAsync(object userState)
        {
            if (this.DocumentoGetTipologiaAttoOperationCompleted == null)
                this.DocumentoGetTipologiaAttoOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetTipologiaAttoOperationCompleted);
            this.InvokeAsync("DocumentoGetTipologiaAtto", new object[0], this.DocumentoGetTipologiaAttoOperationCompleted, userState);
        }

        private void OnDocumentoGetTipologiaAttoOperationCompleted(object arg)
        {
            if (this.DocumentoGetTipologiaAttoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetTipologiaAttoCompleted((object)this, new DocumentoGetTipologiaAttoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoModificaVersione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DocumentoModificaVersione(FileRequest fileReq)
        {
            return (bool)this.Invoke("DocumentoModificaVersione", new object[1]
      {
        (object) fileReq
      })[0];
        }

        public IAsyncResult BeginDocumentoModificaVersione(FileRequest fileReq, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoModificaVersione", new object[1]
      {
        (object) fileReq
      }, callback, asyncState);
        }

        public bool EndDocumentoModificaVersione(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoModificaVersioneAsync(FileRequest fileReq)
        {
            this.DocumentoModificaVersioneAsync(fileReq, (object)null);
        }

        public void DocumentoModificaVersioneAsync(FileRequest fileReq, object userState)
        {
            if (this.DocumentoModificaVersioneOperationCompleted == null)
                this.DocumentoModificaVersioneOperationCompleted = new SendOrPostCallback(this.OnDocumentoModificaVersioneOperationCompleted);
            this.InvokeAsync("DocumentoModificaVersione", new object[1]
      {
        (object) fileReq
      }, this.DocumentoModificaVersioneOperationCompleted, userState);
        }

        private void OnDocumentoModificaVersioneOperationCompleted(object arg)
        {
            if (this.DocumentoModificaVersioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoModificaVersioneCompleted((object)this, new DocumentoModificaVersioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoScambiaVersioni", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DocumentoScambiaVersioni(Documento documento, Allegato allegato)
        {
            return (bool)this.Invoke("DocumentoScambiaVersioni", new object[2]
      {
        (object) documento,
        (object) allegato
      })[0];
        }

        public IAsyncResult BeginDocumentoScambiaVersioni(Documento documento, Allegato allegato, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoScambiaVersioni", new object[2]
      {
        (object) documento,
        (object) allegato
      }, callback, asyncState);
        }

        public bool EndDocumentoScambiaVersioni(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoScambiaVersioniAsync(Documento documento, Allegato allegato)
        {
            this.DocumentoScambiaVersioniAsync(documento, allegato, (object)null);
        }

        public void DocumentoScambiaVersioniAsync(Documento documento, Allegato allegato, object userState)
        {
            if (this.DocumentoScambiaVersioniOperationCompleted == null)
                this.DocumentoScambiaVersioniOperationCompleted = new SendOrPostCallback(this.OnDocumentoScambiaVersioniOperationCompleted);
            this.InvokeAsync("DocumentoScambiaVersioni", new object[2]
      {
        (object) documento,
        (object) allegato
      }, this.DocumentoScambiaVersioniOperationCompleted, userState);
        }

        private void OnDocumentoScambiaVersioniOperationCompleted(object arg)
        {
            if (this.DocumentoScambiaVersioniCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoScambiaVersioniCompleted((object)this, new DocumentoScambiaVersioniCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoAggiungiVersione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileRequest DocumentoAggiungiVersione(FileRequest fileRequest, InfoUtente infoUtente)
        {
            return (FileRequest)this.Invoke("DocumentoAggiungiVersione", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginDocumentoAggiungiVersione(FileRequest fileRequest, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoAggiungiVersione", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FileRequest EndDocumentoAggiungiVersione(IAsyncResult asyncResult)
        {
            return (FileRequest)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoAggiungiVersioneAsync(FileRequest fileRequest, InfoUtente infoUtente)
        {
            this.DocumentoAggiungiVersioneAsync(fileRequest, infoUtente, (object)null);
        }

        public void DocumentoAggiungiVersioneAsync(FileRequest fileRequest, InfoUtente infoUtente, object userState)
        {
            if (this.DocumentoAggiungiVersioneOperationCompleted == null)
                this.DocumentoAggiungiVersioneOperationCompleted = new SendOrPostCallback(this.OnDocumentoAggiungiVersioneOperationCompleted);
            this.InvokeAsync("DocumentoAggiungiVersione", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      }, this.DocumentoAggiungiVersioneOperationCompleted, userState);
        }

        private void OnDocumentoAggiungiVersioneOperationCompleted(object arg)
        {
            if (this.DocumentoAggiungiVersioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoAggiungiVersioneCompleted((object)this, new DocumentoAggiungiVersioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoRimuoviVersione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DocumentoRimuoviVersione(FileRequest fileRequest, InfoUtente infoUtente)
        {
            return (bool)this.Invoke("DocumentoRimuoviVersione", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginDocumentoRimuoviVersione(FileRequest fileRequest, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoRimuoviVersione", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      }, callback, asyncState);
        }

        public bool EndDocumentoRimuoviVersione(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoRimuoviVersioneAsync(FileRequest fileRequest, InfoUtente infoUtente)
        {
            this.DocumentoRimuoviVersioneAsync(fileRequest, infoUtente, (object)null);
        }

        public void DocumentoRimuoviVersioneAsync(FileRequest fileRequest, InfoUtente infoUtente, object userState)
        {
            if (this.DocumentoRimuoviVersioneOperationCompleted == null)
                this.DocumentoRimuoviVersioneOperationCompleted = new SendOrPostCallback(this.OnDocumentoRimuoviVersioneOperationCompleted);
            this.InvokeAsync("DocumentoRimuoviVersione", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      }, this.DocumentoRimuoviVersioneOperationCompleted, userState);
        }

        private void OnDocumentoRimuoviVersioneOperationCompleted(object arg)
        {
            if (this.DocumentoRimuoviVersioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoRimuoviVersioneCompleted((object)this, new DocumentoRimuoviVersioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoModificaAllegato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DocumentoModificaAllegato(Allegato allegato)
        {
            return (bool)this.Invoke("DocumentoModificaAllegato", new object[1]
      {
        (object) allegato
      })[0];
        }

        public IAsyncResult BeginDocumentoModificaAllegato(Allegato allegato, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoModificaAllegato", new object[1]
      {
        (object) allegato
      }, callback, asyncState);
        }

        public bool EndDocumentoModificaAllegato(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoModificaAllegatoAsync(Allegato allegato)
        {
            this.DocumentoModificaAllegatoAsync(allegato, (object)null);
        }

        public void DocumentoModificaAllegatoAsync(Allegato allegato, object userState)
        {
            if (this.DocumentoModificaAllegatoOperationCompleted == null)
                this.DocumentoModificaAllegatoOperationCompleted = new SendOrPostCallback(this.OnDocumentoModificaAllegatoOperationCompleted);
            this.InvokeAsync("DocumentoModificaAllegato", new object[1]
      {
        (object) allegato
      }, this.DocumentoModificaAllegatoOperationCompleted, userState);
        }

        private void OnDocumentoModificaAllegatoOperationCompleted(object arg)
        {
            if (this.DocumentoModificaAllegatoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoModificaAllegatoCompleted((object)this, new DocumentoModificaAllegatoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoAggiungiAllegato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Allegato DocumentoAggiungiAllegato(InfoUtente infoUtente, Allegato allegato)
        {
            return (Allegato)this.Invoke("DocumentoAggiungiAllegato", new object[2]
      {
        (object) infoUtente,
        (object) allegato
      })[0];
        }

        public IAsyncResult BeginDocumentoAggiungiAllegato(InfoUtente infoUtente, Allegato allegato, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoAggiungiAllegato", new object[2]
      {
        (object) infoUtente,
        (object) allegato
      }, callback, asyncState);
        }

        public Allegato EndDocumentoAggiungiAllegato(IAsyncResult asyncResult)
        {
            return (Allegato)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoAggiungiAllegatoAsync(InfoUtente infoUtente, Allegato allegato)
        {
            this.DocumentoAggiungiAllegatoAsync(infoUtente, allegato, (object)null);
        }

        public void DocumentoAggiungiAllegatoAsync(InfoUtente infoUtente, Allegato allegato, object userState)
        {
            if (this.DocumentoAggiungiAllegatoOperationCompleted == null)
                this.DocumentoAggiungiAllegatoOperationCompleted = new SendOrPostCallback(this.OnDocumentoAggiungiAllegatoOperationCompleted);
            this.InvokeAsync("DocumentoAggiungiAllegato", new object[2]
      {
        (object) infoUtente,
        (object) allegato
      }, this.DocumentoAggiungiAllegatoOperationCompleted, userState);
        }

        private void OnDocumentoAggiungiAllegatoOperationCompleted(object arg)
        {
            if (this.DocumentoAggiungiAllegatoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoAggiungiAllegatoCompleted((object)this, new DocumentoAggiungiAllegatoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetAllegati", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Allegato[] DocumentoGetAllegati(string docNumber)
        {
            return (Allegato[])this.Invoke("DocumentoGetAllegati", new object[1]
      {
        (object) docNumber
      })[0];
        }

        public IAsyncResult BeginDocumentoGetAllegati(string docNumber, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetAllegati", new object[1]
      {
        (object) docNumber
      }, callback, asyncState);
        }

        public Allegato[] EndDocumentoGetAllegati(IAsyncResult asyncResult)
        {
            return (Allegato[])this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoGetAllegatiAsync(string docNumber)
        {
            this.DocumentoGetAllegatiAsync(docNumber, (object)null);
        }

        public void DocumentoGetAllegatiAsync(string docNumber, object userState)
        {
            if (this.DocumentoGetAllegatiOperationCompleted == null)
                this.DocumentoGetAllegatiOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetAllegatiOperationCompleted);
            this.InvokeAsync("DocumentoGetAllegati", new object[1]
      {
        (object) docNumber
      }, this.DocumentoGetAllegatiOperationCompleted, userState);
        }

        private void OnDocumentoGetAllegatiOperationCompleted(object arg)
        {
            if (this.DocumentoGetAllegatiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetAllegatiCompleted((object)this, new DocumentoGetAllegatiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoExecRimuoviScheda", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string DocumentoExecRimuoviScheda(InfoUtente infoutente, SchedaDocumento schedaDoc)
        {
            return (string)this.Invoke("DocumentoExecRimuoviScheda", new object[2]
      {
        (object) infoutente,
        (object) schedaDoc
      })[0];
        }

        public IAsyncResult BeginDocumentoExecRimuoviScheda(InfoUtente infoutente, SchedaDocumento schedaDoc, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoExecRimuoviScheda", new object[2]
      {
        (object) infoutente,
        (object) schedaDoc
      }, callback, asyncState);
        }

        public string EndDocumentoExecRimuoviScheda(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoExecRimuoviSchedaAsync(InfoUtente infoutente, SchedaDocumento schedaDoc)
        {
            this.DocumentoExecRimuoviSchedaAsync(infoutente, schedaDoc, (object)null);
        }

        public void DocumentoExecRimuoviSchedaAsync(InfoUtente infoutente, SchedaDocumento schedaDoc, object userState)
        {
            if (this.DocumentoExecRimuoviSchedaOperationCompleted == null)
                this.DocumentoExecRimuoviSchedaOperationCompleted = new SendOrPostCallback(this.OnDocumentoExecRimuoviSchedaOperationCompleted);
            this.InvokeAsync("DocumentoExecRimuoviScheda", new object[2]
      {
        (object) infoutente,
        (object) schedaDoc
      }, this.DocumentoExecRimuoviSchedaOperationCompleted, userState);
        }

        private void OnDocumentoExecRimuoviSchedaOperationCompleted(object arg)
        {
            if (this.DocumentoExecRimuoviSchedaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoExecRimuoviSchedaCompleted((object)this, new DocumentoExecRimuoviSchedaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetParoleChiave", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DocumentoParolaChiave[] DocumentoGetParoleChiave(string idAmministrazione)
        {
            return (DocumentoParolaChiave[])this.Invoke("DocumentoGetParoleChiave", new object[1]
      {
        (object) idAmministrazione
      })[0];
        }

        public IAsyncResult BeginDocumentoGetParoleChiave(string idAmministrazione, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetParoleChiave", new object[1]
      {
        (object) idAmministrazione
      }, callback, asyncState);
        }

        public DocumentoParolaChiave[] EndDocumentoGetParoleChiave(IAsyncResult asyncResult)
        {
            return (DocumentoParolaChiave[])this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoGetParoleChiaveAsync(string idAmministrazione)
        {
            this.DocumentoGetParoleChiaveAsync(idAmministrazione, (object)null);
        }

        public void DocumentoGetParoleChiaveAsync(string idAmministrazione, object userState)
        {
            if (this.DocumentoGetParoleChiaveOperationCompleted == null)
                this.DocumentoGetParoleChiaveOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetParoleChiaveOperationCompleted);
            this.InvokeAsync("DocumentoGetParoleChiave", new object[1]
      {
        (object) idAmministrazione
      }, this.DocumentoGetParoleChiaveOperationCompleted, userState);
        }

        private void OnDocumentoGetParoleChiaveOperationCompleted(object arg)
        {
            if (this.DocumentoGetParoleChiaveCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetParoleChiaveCompleted((object)this, new DocumentoGetParoleChiaveCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetVisibilita", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DocumentoDiritto[] DocumentoGetVisibilita(string idProfile)
        {
            return (DocumentoDiritto[])this.Invoke("DocumentoGetVisibilita", new object[1]
      {
        (object) idProfile
      })[0];
        }

        public IAsyncResult BeginDocumentoGetVisibilita(string idProfile, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetVisibilita", new object[1]
      {
        (object) idProfile
      }, callback, asyncState);
        }

        public DocumentoDiritto[] EndDocumentoGetVisibilita(IAsyncResult asyncResult)
        {
            return (DocumentoDiritto[])this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoGetVisibilitaAsync(string idProfile)
        {
            this.DocumentoGetVisibilitaAsync(idProfile, (object)null);
        }

        public void DocumentoGetVisibilitaAsync(string idProfile, object userState)
        {
            if (this.DocumentoGetVisibilitaOperationCompleted == null)
                this.DocumentoGetVisibilitaOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetVisibilitaOperationCompleted);
            this.InvokeAsync("DocumentoGetVisibilita", new object[1]
      {
        (object) idProfile
      }, this.DocumentoGetVisibilitaOperationCompleted, userState);
        }

        private void OnDocumentoGetVisibilitaOperationCompleted(object arg)
        {
            if (this.DocumentoGetVisibilitaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetVisibilitaCompleted((object)this, new DocumentoGetVisibilitaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoAddDocGrigia", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento DocumentoAddDocGrigia(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo)
        {
            return (SchedaDocumento)this.Invoke("DocumentoAddDocGrigia", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) ruolo
      })[0];
        }

        public IAsyncResult BeginDocumentoAddDocGrigia(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoAddDocGrigia", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) ruolo
      }, callback, asyncState);
        }

        public SchedaDocumento EndDocumentoAddDocGrigia(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoAddDocGrigiaAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo)
        {
            this.DocumentoAddDocGrigiaAsync(schedaDocumento, infoUtente, ruolo, (object)null);
        }

        public void DocumentoAddDocGrigiaAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo, object userState)
        {
            if (this.DocumentoAddDocGrigiaOperationCompleted == null)
                this.DocumentoAddDocGrigiaOperationCompleted = new SendOrPostCallback(this.OnDocumentoAddDocGrigiaOperationCompleted);
            this.InvokeAsync("DocumentoAddDocGrigia", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) ruolo
      }, this.DocumentoAddDocGrigiaOperationCompleted, userState);
        }

        private void OnDocumentoAddDocGrigiaOperationCompleted(object arg)
        {
            if (this.DocumentoAddDocGrigiaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoAddDocGrigiaCompleted((object)this, new DocumentoAddDocGrigiaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoSaveDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento DocumentoSaveDocumento(InfoUtente infoUtente, SchedaDocumento schedaDocumento, bool enableUffRef, out bool daAggiornareUffRef)
        {
            object[] objArray = this.Invoke("DocumentoSaveDocumento", new object[3]
      {
        (object) infoUtente,
        (object) schedaDocumento,
        (object) enableUffRef
      });
            daAggiornareUffRef = (bool)objArray[1];
            return (SchedaDocumento)objArray[0];
        }

        public IAsyncResult BeginDocumentoSaveDocumento(InfoUtente infoUtente, SchedaDocumento schedaDocumento, bool enableUffRef, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoSaveDocumento", new object[3]
      {
        (object) infoUtente,
        (object) schedaDocumento,
        (object) enableUffRef
      }, callback, asyncState);
        }

        public SchedaDocumento EndDocumentoSaveDocumento(IAsyncResult asyncResult, out bool daAggiornareUffRef)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            daAggiornareUffRef = (bool)objArray[1];
            return (SchedaDocumento)objArray[0];
        }

        public void DocumentoSaveDocumentoAsync(InfoUtente infoUtente, SchedaDocumento schedaDocumento, bool enableUffRef)
        {
            this.DocumentoSaveDocumentoAsync(infoUtente, schedaDocumento, enableUffRef, (object)null);
        }

        public void DocumentoSaveDocumentoAsync(InfoUtente infoUtente, SchedaDocumento schedaDocumento, bool enableUffRef, object userState)
        {
            if (this.DocumentoSaveDocumentoOperationCompleted == null)
                this.DocumentoSaveDocumentoOperationCompleted = new SendOrPostCallback(this.OnDocumentoSaveDocumentoOperationCompleted);
            this.InvokeAsync("DocumentoSaveDocumento", new object[3]
      {
        (object) infoUtente,
        (object) schedaDocumento,
        (object) enableUffRef
      }, this.DocumentoSaveDocumentoOperationCompleted, userState);
        }

        private void OnDocumentoSaveDocumentoOperationCompleted(object arg)
        {
            if (this.DocumentoSaveDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoSaveDocumentoCompleted((object)this, new DocumentoSaveDocumentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoCancellaAreaLavoro", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DocumentoCancellaAreaLavoro(string idPeople, string idRuoloInUO, string idProfile, Fascicolo fasc)
        {
            return (bool)this.Invoke("DocumentoCancellaAreaLavoro", new object[4]
      {
        (object) idPeople,
        (object) idRuoloInUO,
        (object) idProfile,
        (object) fasc
      })[0];
        }

        public IAsyncResult BeginDocumentoCancellaAreaLavoro(string idPeople, string idRuoloInUO, string idProfile, Fascicolo fasc, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoCancellaAreaLavoro", new object[4]
      {
        (object) idPeople,
        (object) idRuoloInUO,
        (object) idProfile,
        (object) fasc
      }, callback, asyncState);
        }

        public bool EndDocumentoCancellaAreaLavoro(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoCancellaAreaLavoroAsync(string idPeople, string idRuoloInUO, string idProfile, Fascicolo fasc)
        {
            this.DocumentoCancellaAreaLavoroAsync(idPeople, idRuoloInUO, idProfile, fasc, (object)null);
        }

        public void DocumentoCancellaAreaLavoroAsync(string idPeople, string idRuoloInUO, string idProfile, Fascicolo fasc, object userState)
        {
            if (this.DocumentoCancellaAreaLavoroOperationCompleted == null)
                this.DocumentoCancellaAreaLavoroOperationCompleted = new SendOrPostCallback(this.OnDocumentoCancellaAreaLavoroOperationCompleted);
            this.InvokeAsync("DocumentoCancellaAreaLavoro", new object[4]
      {
        (object) idPeople,
        (object) idRuoloInUO,
        (object) idProfile,
        (object) fasc
      }, this.DocumentoCancellaAreaLavoroOperationCompleted, userState);
        }

        private void OnDocumentoCancellaAreaLavoroOperationCompleted(object arg)
        {
            if (this.DocumentoCancellaAreaLavoroCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoCancellaAreaLavoroCompleted((object)this, new DocumentoCancellaAreaLavoroCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetAreaLavoroPaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public AreaLavoro DocumentoGetAreaLavoroPaging(Utente utente, Ruolo ruolo, bool enableUffRef, AreaLavoroTipoOggetto tipoObj, AreaLavoroTipoDocumento tipoDoc, AreaLavoroTipoFascicolo tipoFasc, int numPage, string idRegistro, out int numTotPage, out int nRec)
        {
            object[] objArray = this.Invoke("DocumentoGetAreaLavoroPaging", new object[8]
      {
        (object) utente,
        (object) ruolo,
        (object) enableUffRef,
        (object) tipoObj,
        (object) tipoDoc,
        (object) tipoFasc,
        (object) numPage,
        (object) idRegistro
      });
            numTotPage = (int)objArray[1];
            nRec = (int)objArray[2];
            return (AreaLavoro)objArray[0];
        }

        public IAsyncResult BeginDocumentoGetAreaLavoroPaging(Utente utente, Ruolo ruolo, bool enableUffRef, AreaLavoroTipoOggetto tipoObj, AreaLavoroTipoDocumento tipoDoc, AreaLavoroTipoFascicolo tipoFasc, int numPage, string idRegistro, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetAreaLavoroPaging", new object[8]
      {
        (object) utente,
        (object) ruolo,
        (object) enableUffRef,
        (object) tipoObj,
        (object) tipoDoc,
        (object) tipoFasc,
        (object) numPage,
        (object) idRegistro
      }, callback, asyncState);
        }

        public AreaLavoro EndDocumentoGetAreaLavoroPaging(IAsyncResult asyncResult, out int numTotPage, out int nRec)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            numTotPage = (int)objArray[1];
            nRec = (int)objArray[2];
            return (AreaLavoro)objArray[0];
        }

        public void DocumentoGetAreaLavoroPagingAsync(Utente utente, Ruolo ruolo, bool enableUffRef, AreaLavoroTipoOggetto tipoObj, AreaLavoroTipoDocumento tipoDoc, AreaLavoroTipoFascicolo tipoFasc, int numPage, string idRegistro)
        {
            this.DocumentoGetAreaLavoroPagingAsync(utente, ruolo, enableUffRef, tipoObj, tipoDoc, tipoFasc, numPage, idRegistro, (object)null);
        }

        public void DocumentoGetAreaLavoroPagingAsync(Utente utente, Ruolo ruolo, bool enableUffRef, AreaLavoroTipoOggetto tipoObj, AreaLavoroTipoDocumento tipoDoc, AreaLavoroTipoFascicolo tipoFasc, int numPage, string idRegistro, object userState)
        {
            if (this.DocumentoGetAreaLavoroPagingOperationCompleted == null)
                this.DocumentoGetAreaLavoroPagingOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetAreaLavoroPagingOperationCompleted);
            this.InvokeAsync("DocumentoGetAreaLavoroPaging", new object[8]
      {
        (object) utente,
        (object) ruolo,
        (object) enableUffRef,
        (object) tipoObj,
        (object) tipoDoc,
        (object) tipoFasc,
        (object) numPage,
        (object) idRegistro
      }, this.DocumentoGetAreaLavoroPagingOperationCompleted, userState);
        }

        private void OnDocumentoGetAreaLavoroPagingOperationCompleted(object arg)
        {
            if (this.DocumentoGetAreaLavoroPagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetAreaLavoroPagingCompleted((object)this, new DocumentoGetAreaLavoroPagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetAreaLavoro", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public AreaLavoro DocumentoGetAreaLavoro(Utente utente, Ruolo ruolo, AreaLavoroTipoOggetto tipoObj, AreaLavoroTipoDocumento tipoDoc, AreaLavoroTipoFascicolo tipoFasc, string idRegistro, bool enableUffRef)
        {
            return (AreaLavoro)this.Invoke("DocumentoGetAreaLavoro", new object[7]
      {
        (object) utente,
        (object) ruolo,
        (object) tipoObj,
        (object) tipoDoc,
        (object) tipoFasc,
        (object) idRegistro,
        (object) enableUffRef
      })[0];
        }

        public IAsyncResult BeginDocumentoGetAreaLavoro(Utente utente, Ruolo ruolo, AreaLavoroTipoOggetto tipoObj, AreaLavoroTipoDocumento tipoDoc, AreaLavoroTipoFascicolo tipoFasc, string idRegistro, bool enableUffRef, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetAreaLavoro", new object[7]
      {
        (object) utente,
        (object) ruolo,
        (object) tipoObj,
        (object) tipoDoc,
        (object) tipoFasc,
        (object) idRegistro,
        (object) enableUffRef
      }, callback, asyncState);
        }

        public AreaLavoro EndDocumentoGetAreaLavoro(IAsyncResult asyncResult)
        {
            return (AreaLavoro)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoGetAreaLavoroAsync(Utente utente, Ruolo ruolo, AreaLavoroTipoOggetto tipoObj, AreaLavoroTipoDocumento tipoDoc, AreaLavoroTipoFascicolo tipoFasc, string idRegistro, bool enableUffRef)
        {
            this.DocumentoGetAreaLavoroAsync(utente, ruolo, tipoObj, tipoDoc, tipoFasc, idRegistro, enableUffRef, (object)null);
        }

        public void DocumentoGetAreaLavoroAsync(Utente utente, Ruolo ruolo, AreaLavoroTipoOggetto tipoObj, AreaLavoroTipoDocumento tipoDoc, AreaLavoroTipoFascicolo tipoFasc, string idRegistro, bool enableUffRef, object userState)
        {
            if (this.DocumentoGetAreaLavoroOperationCompleted == null)
                this.DocumentoGetAreaLavoroOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetAreaLavoroOperationCompleted);
            this.InvokeAsync("DocumentoGetAreaLavoro", new object[7]
      {
        (object) utente,
        (object) ruolo,
        (object) tipoObj,
        (object) tipoDoc,
        (object) tipoFasc,
        (object) idRegistro,
        (object) enableUffRef
      }, this.DocumentoGetAreaLavoroOperationCompleted, userState);
        }

        private void OnDocumentoGetAreaLavoroOperationCompleted(object arg)
        {
            if (this.DocumentoGetAreaLavoroCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetAreaLavoroCompleted((object)this, new DocumentoGetAreaLavoroCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoExecAnnullaProt", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public ProtocolloAnnullato DocumentoExecAnnullaProt(InfoUtente infoutente, string idProfile, ProtocolloAnnullato protAnn, string segnatura)
        {
            return (ProtocolloAnnullato)this.Invoke("DocumentoExecAnnullaProt", new object[4]
      {
        (object) infoutente,
        (object) idProfile,
        (object) protAnn,
        (object) segnatura
      })[0];
        }

        public IAsyncResult BeginDocumentoExecAnnullaProt(InfoUtente infoutente, string idProfile, ProtocolloAnnullato protAnn, string segnatura, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoExecAnnullaProt", new object[4]
      {
        (object) infoutente,
        (object) idProfile,
        (object) protAnn,
        (object) segnatura
      }, callback, asyncState);
        }

        public ProtocolloAnnullato EndDocumentoExecAnnullaProt(IAsyncResult asyncResult)
        {
            return (ProtocolloAnnullato)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoExecAnnullaProtAsync(InfoUtente infoutente, string idProfile, ProtocolloAnnullato protAnn, string segnatura)
        {
            this.DocumentoExecAnnullaProtAsync(infoutente, idProfile, protAnn, segnatura, (object)null);
        }

        public void DocumentoExecAnnullaProtAsync(InfoUtente infoutente, string idProfile, ProtocolloAnnullato protAnn, string segnatura, object userState)
        {
            if (this.DocumentoExecAnnullaProtOperationCompleted == null)
                this.DocumentoExecAnnullaProtOperationCompleted = new SendOrPostCallback(this.OnDocumentoExecAnnullaProtOperationCompleted);
            this.InvokeAsync("DocumentoExecAnnullaProt", new object[4]
      {
        (object) infoutente,
        (object) idProfile,
        (object) protAnn,
        (object) segnatura
      }, this.DocumentoExecAnnullaProtOperationCompleted, userState);
        }

        private void OnDocumentoExecAnnullaProtOperationCompleted(object arg)
        {
            if (this.DocumentoExecAnnullaProtCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoExecAnnullaProtCompleted((object)this, new DocumentoExecAnnullaProtCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoExecAddLavoro", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DocumentoExecAddLavoro(string idProfile, string tipoProto, Fascicolo fasc, InfoUtente infoUtente, string idRegistro)
        {
            return (bool)this.Invoke("DocumentoExecAddLavoro", new object[5]
      {
        (object) idProfile,
        (object) tipoProto,
        (object) fasc,
        (object) infoUtente,
        (object) idRegistro
      })[0];
        }

        public IAsyncResult BeginDocumentoExecAddLavoro(string idProfile, string tipoProto, Fascicolo fasc, InfoUtente infoUtente, string idRegistro, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoExecAddLavoro", new object[5]
      {
        (object) idProfile,
        (object) tipoProto,
        (object) fasc,
        (object) infoUtente,
        (object) idRegistro
      }, callback, asyncState);
        }

        public bool EndDocumentoExecAddLavoro(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoExecAddLavoroAsync(string idProfile, string tipoProto, Fascicolo fasc, InfoUtente infoUtente, string idRegistro)
        {
            this.DocumentoExecAddLavoroAsync(idProfile, tipoProto, fasc, infoUtente, idRegistro, (object)null);
        }

        public void DocumentoExecAddLavoroAsync(string idProfile, string tipoProto, Fascicolo fasc, InfoUtente infoUtente, string idRegistro, object userState)
        {
            if (this.DocumentoExecAddLavoroOperationCompleted == null)
                this.DocumentoExecAddLavoroOperationCompleted = new SendOrPostCallback(this.OnDocumentoExecAddLavoroOperationCompleted);
            this.InvokeAsync("DocumentoExecAddLavoro", new object[5]
      {
        (object) idProfile,
        (object) tipoProto,
        (object) fasc,
        (object) infoUtente,
        (object) idRegistro
      }, this.DocumentoExecAddLavoroOperationCompleted, userState);
        }

        private void OnDocumentoExecAddLavoroOperationCompleted(object arg)
        {
            if (this.DocumentoExecAddLavoroCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoExecAddLavoroCompleted((object)this, new DocumentoExecAddLavoroCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoImportaProtocolloEmergenza", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public resultProtoEmergenza documentoImportaProtocolloEmergenza(ProtocolloEmergenza protoEmergenza, InfoUtente infoUtente)
        {
            return (resultProtoEmergenza)this.Invoke("documentoImportaProtocolloEmergenza", new object[2]
      {
        (object) protoEmergenza,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoImportaProtocolloEmergenza(ProtocolloEmergenza protoEmergenza, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoImportaProtocolloEmergenza", new object[2]
      {
        (object) protoEmergenza,
        (object) infoUtente
      }, callback, asyncState);
        }

        public resultProtoEmergenza EnddocumentoImportaProtocolloEmergenza(IAsyncResult asyncResult)
        {
            return (resultProtoEmergenza)this.EndInvoke(asyncResult)[0];
        }

        public void documentoImportaProtocolloEmergenzaAsync(ProtocolloEmergenza protoEmergenza, InfoUtente infoUtente)
        {
            this.documentoImportaProtocolloEmergenzaAsync(protoEmergenza, infoUtente, (object)null);
        }

        public void documentoImportaProtocolloEmergenzaAsync(ProtocolloEmergenza protoEmergenza, InfoUtente infoUtente, object userState)
        {
            if (this.documentoImportaProtocolloEmergenzaOperationCompleted == null)
                this.documentoImportaProtocolloEmergenzaOperationCompleted = new SendOrPostCallback(this.OndocumentoImportaProtocolloEmergenzaOperationCompleted);
            this.InvokeAsync("documentoImportaProtocolloEmergenza", new object[2]
      {
        (object) protoEmergenza,
        (object) infoUtente
      }, this.documentoImportaProtocolloEmergenzaOperationCompleted, userState);
        }

        private void OndocumentoImportaProtocolloEmergenzaOperationCompleted(object arg)
        {
            if (this.documentoImportaProtocolloEmergenzaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoImportaProtocolloEmergenzaCompleted((object)this, new documentoImportaProtocolloEmergenzaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoProtocolla", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento DocumentoProtocolla(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo, out ResultProtocollazione risultatoProtocollazione)
        {
            object[] objArray = this.Invoke("DocumentoProtocolla", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) ruolo
      });
            risultatoProtocollazione = (ResultProtocollazione)objArray[1];
            return (SchedaDocumento)objArray[0];
        }

        public IAsyncResult BeginDocumentoProtocolla(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoProtocolla", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) ruolo
      }, callback, asyncState);
        }

        public SchedaDocumento EndDocumentoProtocolla(IAsyncResult asyncResult, out ResultProtocollazione risultatoProtocollazione)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            risultatoProtocollazione = (ResultProtocollazione)objArray[1];
            return (SchedaDocumento)objArray[0];
        }

        public void DocumentoProtocollaAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo)
        {
            this.DocumentoProtocollaAsync(schedaDocumento, infoUtente, ruolo, (object)null);
        }

        public void DocumentoProtocollaAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo, object userState)
        {
            if (this.DocumentoProtocollaOperationCompleted == null)
                this.DocumentoProtocollaOperationCompleted = new SendOrPostCallback(this.OnDocumentoProtocollaOperationCompleted);
            this.InvokeAsync("DocumentoProtocolla", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) ruolo
      }, this.DocumentoProtocollaOperationCompleted, userState);
        }

        private void OnDocumentoProtocollaOperationCompleted(object arg)
        {
            if (this.DocumentoProtocollaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoProtocollaCompleted((object)this, new DocumentoProtocollaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetListaOggetti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Oggetto[] DocumentoGetListaOggetti(DocumentoQueryOggetto queryOggetto)
        {
            return (Oggetto[])this.Invoke("DocumentoGetListaOggetti", new object[1]
      {
        (object) queryOggetto
      })[0];
        }

        public IAsyncResult BeginDocumentoGetListaOggetti(DocumentoQueryOggetto queryOggetto, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetListaOggetti", new object[1]
      {
        (object) queryOggetto
      }, callback, asyncState);
        }

        public Oggetto[] EndDocumentoGetListaOggetti(IAsyncResult asyncResult)
        {
            return (Oggetto[])this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoGetListaOggettiAsync(DocumentoQueryOggetto queryOggetto)
        {
            this.DocumentoGetListaOggettiAsync(queryOggetto, (object)null);
        }

        public void DocumentoGetListaOggettiAsync(DocumentoQueryOggetto queryOggetto, object userState)
        {
            if (this.DocumentoGetListaOggettiOperationCompleted == null)
                this.DocumentoGetListaOggettiOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetListaOggettiOperationCompleted);
            this.InvokeAsync("DocumentoGetListaOggetti", new object[1]
      {
        (object) queryOggetto
      }, this.DocumentoGetListaOggettiOperationCompleted, userState);
        }

        private void OnDocumentoGetListaOggettiOperationCompleted(object arg)
        {
            if (this.DocumentoGetListaOggettiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetListaOggettiCompleted((object)this, new DocumentoGetListaOggettiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetDettaglioDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento DocumentoGetDettaglioDocumento(InfoUtente infoutente, string idProfile, string docNumber)
        {
            return (SchedaDocumento)this.Invoke("DocumentoGetDettaglioDocumento", new object[3]
      {
        (object) infoutente,
        (object) idProfile,
        (object) docNumber
      })[0];
        }

        public IAsyncResult BeginDocumentoGetDettaglioDocumento(InfoUtente infoutente, string idProfile, string docNumber, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetDettaglioDocumento", new object[3]
      {
        (object) infoutente,
        (object) idProfile,
        (object) docNumber
      }, callback, asyncState);
        }

        public SchedaDocumento EndDocumentoGetDettaglioDocumento(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoGetDettaglioDocumentoAsync(InfoUtente infoutente, string idProfile, string docNumber)
        {
            this.DocumentoGetDettaglioDocumentoAsync(infoutente, idProfile, docNumber, (object)null);
        }

        public void DocumentoGetDettaglioDocumentoAsync(InfoUtente infoutente, string idProfile, string docNumber, object userState)
        {
            if (this.DocumentoGetDettaglioDocumentoOperationCompleted == null)
                this.DocumentoGetDettaglioDocumentoOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetDettaglioDocumentoOperationCompleted);
            this.InvokeAsync("DocumentoGetDettaglioDocumento", new object[3]
      {
        (object) infoutente,
        (object) idProfile,
        (object) docNumber
      }, this.DocumentoGetDettaglioDocumentoOperationCompleted, userState);
        }

        private void OnDocumentoGetDettaglioDocumentoOperationCompleted(object arg)
        {
            if (this.DocumentoGetDettaglioDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetDettaglioDocumentoCompleted((object)this, new DocumentoGetDettaglioDocumentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoPutFileBatch", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string DocumentoPutFileBatch(string DocumentoFileBatchXmlStream)
        {
            return (string)this.Invoke("DocumentoPutFileBatch", new object[1]
      {
        (object) DocumentoFileBatchXmlStream
      })[0];
        }

        public IAsyncResult BeginDocumentoPutFileBatch(string DocumentoFileBatchXmlStream, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoPutFileBatch", new object[1]
      {
        (object) DocumentoFileBatchXmlStream
      }, callback, asyncState);
        }

        public string EndDocumentoPutFileBatch(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoPutFileBatchAsync(string DocumentoFileBatchXmlStream)
        {
            this.DocumentoPutFileBatchAsync(DocumentoFileBatchXmlStream, (object)null);
        }

        public void DocumentoPutFileBatchAsync(string DocumentoFileBatchXmlStream, object userState)
        {
            if (this.DocumentoPutFileBatchOperationCompleted == null)
                this.DocumentoPutFileBatchOperationCompleted = new SendOrPostCallback(this.OnDocumentoPutFileBatchOperationCompleted);
            this.InvokeAsync("DocumentoPutFileBatch", new object[1]
      {
        (object) DocumentoFileBatchXmlStream
      }, this.DocumentoPutFileBatchOperationCompleted, userState);
        }

        private void OnDocumentoPutFileBatchOperationCompleted(object arg)
        {
            if (this.DocumentoPutFileBatchCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoPutFileBatchCompleted((object)this, new DocumentoPutFileBatchCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoPutFile", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileRequest DocumentoPutFile(FileRequest fileRequest, FileDocumento fileDocument, InfoUtente infoUtente)
        {
            return (FileRequest)this.Invoke("DocumentoPutFile", new object[3]
      {
        (object) fileRequest,
        (object) fileDocument,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginDocumentoPutFile(FileRequest fileRequest, FileDocumento fileDocument, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoPutFile", new object[3]
      {
        (object) fileRequest,
        (object) fileDocument,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FileRequest EndDocumentoPutFile(IAsyncResult asyncResult)
        {
            return (FileRequest)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoPutFileAsync(FileRequest fileRequest, FileDocumento fileDocument, InfoUtente infoUtente)
        {
            this.DocumentoPutFileAsync(fileRequest, fileDocument, infoUtente, (object)null);
        }

        public void DocumentoPutFileAsync(FileRequest fileRequest, FileDocumento fileDocument, InfoUtente infoUtente, object userState)
        {
            if (this.DocumentoPutFileOperationCompleted == null)
                this.DocumentoPutFileOperationCompleted = new SendOrPostCallback(this.OnDocumentoPutFileOperationCompleted);
            this.InvokeAsync("DocumentoPutFile", new object[3]
      {
        (object) fileRequest,
        (object) fileDocument,
        (object) infoUtente
      }, this.DocumentoPutFileOperationCompleted, userState);
        }

        private void OnDocumentoPutFileOperationCompleted(object arg)
        {
            if (this.DocumentoPutFileCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoPutFileCompleted((object)this, new DocumentoPutFileCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetFile", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileDocumento DocumentoGetFile(FileRequest fileRequest, InfoUtente infoUtente)
        {
            return (FileDocumento)this.Invoke("DocumentoGetFile", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginDocumentoGetFile(FileRequest fileRequest, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetFile", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FileDocumento EndDocumentoGetFile(IAsyncResult asyncResult)
        {
            return (FileDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoGetFileAsync(FileRequest fileRequest, InfoUtente infoUtente)
        {
            this.DocumentoGetFileAsync(fileRequest, infoUtente, (object)null);
        }

        public void DocumentoGetFileAsync(FileRequest fileRequest, InfoUtente infoUtente, object userState)
        {
            if (this.DocumentoGetFileOperationCompleted == null)
                this.DocumentoGetFileOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetFileOperationCompleted);
            this.InvokeAsync("DocumentoGetFile", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      }, this.DocumentoGetFileOperationCompleted, userState);
        }

        private void OnDocumentoGetFileOperationCompleted(object arg)
        {
            if (this.DocumentoGetFileCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetFileCompleted((object)this, new DocumentoGetFileCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetQueryFullTextDocumentoPaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public InfoDocumento[] DocumentoGetQueryFullTextDocumentoPaging(string testo, string idRegistro, InfoUtente infoUtente, [XmlArrayItem(NestingLevel = 1), XmlArrayItem("ArrayOfFiltroRicerca")] FiltroRicerca[][] queryList, int numPage, out int numTotPage, out int nRec)
        {
            object[] objArray = this.Invoke("DocumentoGetQueryFullTextDocumentoPaging", new object[5]
      {
        (object) testo,
        (object) idRegistro,
        (object) infoUtente,
        (object) queryList,
        (object) numPage
      });
            numTotPage = (int)objArray[1];
            nRec = (int)objArray[2];
            return (InfoDocumento[])objArray[0];
        }

        public IAsyncResult BeginDocumentoGetQueryFullTextDocumentoPaging(string testo, string idRegistro, InfoUtente infoUtente, FiltroRicerca[][] queryList, int numPage, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetQueryFullTextDocumentoPaging", new object[5]
      {
        (object) testo,
        (object) idRegistro,
        (object) infoUtente,
        (object) queryList,
        (object) numPage
      }, callback, asyncState);
        }

        public InfoDocumento[] EndDocumentoGetQueryFullTextDocumentoPaging(IAsyncResult asyncResult, out int numTotPage, out int nRec)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            numTotPage = (int)objArray[1];
            nRec = (int)objArray[2];
            return (InfoDocumento[])objArray[0];
        }

        public void DocumentoGetQueryFullTextDocumentoPagingAsync(string testo, string idRegistro, InfoUtente infoUtente, FiltroRicerca[][] queryList, int numPage)
        {
            this.DocumentoGetQueryFullTextDocumentoPagingAsync(testo, idRegistro, infoUtente, queryList, numPage, (object)null);
        }

        public void DocumentoGetQueryFullTextDocumentoPagingAsync(string testo, string idRegistro, InfoUtente infoUtente, FiltroRicerca[][] queryList, int numPage, object userState)
        {
            if (this.DocumentoGetQueryFullTextDocumentoPagingOperationCompleted == null)
                this.DocumentoGetQueryFullTextDocumentoPagingOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetQueryFullTextDocumentoPagingOperationCompleted);
            this.InvokeAsync("DocumentoGetQueryFullTextDocumentoPaging", new object[5]
      {
        (object) testo,
        (object) idRegistro,
        (object) infoUtente,
        (object) queryList,
        (object) numPage
      }, this.DocumentoGetQueryFullTextDocumentoPagingOperationCompleted, userState);
        }

        private void OnDocumentoGetQueryFullTextDocumentoPagingOperationCompleted(object arg)
        {
            if (this.DocumentoGetQueryFullTextDocumentoPagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetQueryFullTextDocumentoPagingCompleted((object)this, new DocumentoGetQueryFullTextDocumentoPagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetQueryDocumentoPaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public InfoDocumento[] DocumentoGetQueryDocumentoPaging(string idGruppo, string idPeople, [XmlArrayItem("ArrayOfFiltroRicerca"), XmlArrayItem(NestingLevel = 1)] FiltroRicerca[][] queryList, int numPage, out int numTotPage, out int nRec)
        {
            object[] objArray = this.Invoke("DocumentoGetQueryDocumentoPaging", new object[4]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) queryList,
        (object) numPage
      });
            numTotPage = (int)objArray[1];
            nRec = (int)objArray[2];
            return (InfoDocumento[])objArray[0];
        }

        public IAsyncResult BeginDocumentoGetQueryDocumentoPaging(string idGruppo, string idPeople, FiltroRicerca[][] queryList, int numPage, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetQueryDocumentoPaging", new object[4]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) queryList,
        (object) numPage
      }, callback, asyncState);
        }

        public InfoDocumento[] EndDocumentoGetQueryDocumentoPaging(IAsyncResult asyncResult, out int numTotPage, out int nRec)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            numTotPage = (int)objArray[1];
            nRec = (int)objArray[2];
            return (InfoDocumento[])objArray[0];
        }

        public void DocumentoGetQueryDocumentoPagingAsync(string idGruppo, string idPeople, FiltroRicerca[][] queryList, int numPage)
        {
            this.DocumentoGetQueryDocumentoPagingAsync(idGruppo, idPeople, queryList, numPage, (object)null);
        }

        public void DocumentoGetQueryDocumentoPagingAsync(string idGruppo, string idPeople, FiltroRicerca[][] queryList, int numPage, object userState)
        {
            if (this.DocumentoGetQueryDocumentoPagingOperationCompleted == null)
                this.DocumentoGetQueryDocumentoPagingOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetQueryDocumentoPagingOperationCompleted);
            this.InvokeAsync("DocumentoGetQueryDocumentoPaging", new object[4]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) queryList,
        (object) numPage
      }, this.DocumentoGetQueryDocumentoPagingOperationCompleted, userState);
        }

        private void OnDocumentoGetQueryDocumentoPagingOperationCompleted(object arg)
        {
            if (this.DocumentoGetQueryDocumentoPagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetQueryDocumentoPagingCompleted((object)this, new DocumentoGetQueryDocumentoPagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetQueryDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public InfoDocumento[] DocumentoGetQueryDocumento(string idGruppo, string idPeople, [XmlArrayItem(NestingLevel = 1), XmlArrayItem("ArrayOfFiltroRicerca")] FiltroRicerca[][] queryList)
        {
            return (InfoDocumento[])this.Invoke("DocumentoGetQueryDocumento", new object[3]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) queryList
      })[0];
        }

        public IAsyncResult BeginDocumentoGetQueryDocumento(string idGruppo, string idPeople, FiltroRicerca[][] queryList, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetQueryDocumento", new object[3]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) queryList
      }, callback, asyncState);
        }

        public InfoDocumento[] EndDocumentoGetQueryDocumento(IAsyncResult asyncResult)
        {
            return (InfoDocumento[])this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoGetQueryDocumentoAsync(string idGruppo, string idPeople, FiltroRicerca[][] queryList)
        {
            this.DocumentoGetQueryDocumentoAsync(idGruppo, idPeople, queryList, (object)null);
        }

        public void DocumentoGetQueryDocumentoAsync(string idGruppo, string idPeople, FiltroRicerca[][] queryList, object userState)
        {
            if (this.DocumentoGetQueryDocumentoOperationCompleted == null)
                this.DocumentoGetQueryDocumentoOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetQueryDocumentoOperationCompleted);
            this.InvokeAsync("DocumentoGetQueryDocumento", new object[3]
      {
        (object) idGruppo,
        (object) idPeople,
        (object) queryList
      }, this.DocumentoGetQueryDocumentoOperationCompleted, userState);
        }

        private void OnDocumentoGetQueryDocumentoOperationCompleted(object arg)
        {
            if (this.DocumentoGetQueryDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetQueryDocumentoCompleted((object)this, new DocumentoGetQueryDocumentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/doucmentoGetDocType", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string doucmentoGetDocType()
        {
            return (string)this.Invoke("doucmentoGetDocType", new object[0])[0];
        }

        public IAsyncResult BegindoucmentoGetDocType(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("doucmentoGetDocType", new object[0], callback, asyncState);
        }

        public string EnddoucmentoGetDocType(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void doucmentoGetDocTypeAsync()
        {
            this.doucmentoGetDocTypeAsync((object)null);
        }

        public void doucmentoGetDocTypeAsync(object userState)
        {
            if (this.doucmentoGetDocTypeOperationCompleted == null)
                this.doucmentoGetDocTypeOperationCompleted = new SendOrPostCallback(this.OndoucmentoGetDocTypeOperationCompleted);
            this.InvokeAsync("doucmentoGetDocType", new object[0], this.doucmentoGetDocTypeOperationCompleted, userState);
        }

        private void OndoucmentoGetDocTypeOperationCompleted(object arg)
        {
            if (this.doucmentoGetDocTypeCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.doucmentoGetDocTypeCompleted((object)this, new doucmentoGetDocTypeCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/UtenteChangePassword", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool UtenteChangePassword(UserLogin login)
        {
            return (bool)this.Invoke("UtenteChangePassword", new object[1]
      {
        (object) login
      })[0];
        }

        public IAsyncResult BeginUtenteChangePassword(UserLogin login, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("UtenteChangePassword", new object[1]
      {
        (object) login
      }, callback, asyncState);
        }

        public bool EndUtenteChangePassword(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void UtenteChangePasswordAsync(UserLogin login)
        {
            this.UtenteChangePasswordAsync(login, (object)null);
        }

        public void UtenteChangePasswordAsync(UserLogin login, object userState)
        {
            if (this.UtenteChangePasswordOperationCompleted == null)
                this.UtenteChangePasswordOperationCompleted = new SendOrPostCallback(this.OnUtenteChangePasswordOperationCompleted);
            this.InvokeAsync("UtenteChangePassword", new object[1]
      {
        (object) login
      }, this.UtenteChangePasswordOperationCompleted, userState);
        }

        private void OnUtenteChangePasswordOperationCompleted(object arg)
        {
            if (this.UtenteChangePasswordCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.UtenteChangePasswordCompleted((object)this, new UtenteChangePasswordCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/UtenteGetRegistri", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Registro[] UtenteGetRegistri(string idCorrGlobali)
        {
            return (Registro[])this.Invoke("UtenteGetRegistri", new object[1]
      {
        (object) idCorrGlobali
      })[0];
        }

        public IAsyncResult BeginUtenteGetRegistri(string idCorrGlobali, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("UtenteGetRegistri", new object[1]
      {
        (object) idCorrGlobali
      }, callback, asyncState);
        }

        public Registro[] EndUtenteGetRegistri(IAsyncResult asyncResult)
        {
            return (Registro[])this.EndInvoke(asyncResult)[0];
        }

        public void UtenteGetRegistriAsync(string idCorrGlobali)
        {
            this.UtenteGetRegistriAsync(idCorrGlobali, (object)null);
        }

        public void UtenteGetRegistriAsync(string idCorrGlobali, object userState)
        {
            if (this.UtenteGetRegistriOperationCompleted == null)
                this.UtenteGetRegistriOperationCompleted = new SendOrPostCallback(this.OnUtenteGetRegistriOperationCompleted);
            this.InvokeAsync("UtenteGetRegistri", new object[1]
      {
        (object) idCorrGlobali
      }, this.UtenteGetRegistriOperationCompleted, userState);
        }

        private void OnUtenteGetRegistriOperationCompleted(object arg)
        {
            if (this.UtenteGetRegistriCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.UtenteGetRegistriCompleted((object)this, new UtenteGetRegistriCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/RegistriModifica", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Registro RegistriModifica(Registro registro, InfoUtente infoUtente)
        {
            return (Registro)this.Invoke("RegistriModifica", new object[2]
      {
        (object) registro,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginRegistriModifica(Registro registro, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RegistriModifica", new object[2]
      {
        (object) registro,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Registro EndRegistriModifica(IAsyncResult asyncResult)
        {
            return (Registro)this.EndInvoke(asyncResult)[0];
        }

        public void RegistriModificaAsync(Registro registro, InfoUtente infoUtente)
        {
            this.RegistriModificaAsync(registro, infoUtente, (object)null);
        }

        public void RegistriModificaAsync(Registro registro, InfoUtente infoUtente, object userState)
        {
            if (this.RegistriModificaOperationCompleted == null)
                this.RegistriModificaOperationCompleted = new SendOrPostCallback(this.OnRegistriModificaOperationCompleted);
            this.InvokeAsync("RegistriModifica", new object[2]
      {
        (object) registro,
        (object) infoUtente
      }, this.RegistriModificaOperationCompleted, userState);
        }

        private void OnRegistriModificaOperationCompleted(object arg)
        {
            if (this.RegistriModificaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.RegistriModificaCompleted((object)this, new RegistriModificaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/RegistriCambiaStato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Registro RegistriCambiaStato(InfoUtente infoutente, Registro registro)
        {
            return (Registro)this.Invoke("RegistriCambiaStato", new object[2]
      {
        (object) infoutente,
        (object) registro
      })[0];
        }

        public IAsyncResult BeginRegistriCambiaStato(InfoUtente infoutente, Registro registro, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RegistriCambiaStato", new object[2]
      {
        (object) infoutente,
        (object) registro
      }, callback, asyncState);
        }

        public Registro EndRegistriCambiaStato(IAsyncResult asyncResult)
        {
            return (Registro)this.EndInvoke(asyncResult)[0];
        }

        public void RegistriCambiaStatoAsync(InfoUtente infoutente, Registro registro)
        {
            this.RegistriCambiaStatoAsync(infoutente, registro, (object)null);
        }

        public void RegistriCambiaStatoAsync(InfoUtente infoutente, Registro registro, object userState)
        {
            if (this.RegistriCambiaStatoOperationCompleted == null)
                this.RegistriCambiaStatoOperationCompleted = new SendOrPostCallback(this.OnRegistriCambiaStatoOperationCompleted);
            this.InvokeAsync("RegistriCambiaStato", new object[2]
      {
        (object) infoutente,
        (object) registro
      }, this.RegistriCambiaStatoOperationCompleted, userState);
        }

        private void OnRegistriCambiaStatoOperationCompleted(object arg)
        {
            if (this.RegistriCambiaStatoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.RegistriCambiaStatoCompleted((object)this, new RegistriCambiaStatoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ElencoUtentiConnessi", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string ElencoUtentiConnessi(string codiceAmm)
        {
            return (string)this.Invoke("ElencoUtentiConnessi", new object[1]
      {
        (object) codiceAmm
      })[0];
        }

        public IAsyncResult BeginElencoUtentiConnessi(string codiceAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ElencoUtentiConnessi", new object[1]
      {
        (object) codiceAmm
      }, callback, asyncState);
        }

        public string EndElencoUtentiConnessi(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void ElencoUtentiConnessiAsync(string codiceAmm)
        {
            this.ElencoUtentiConnessiAsync(codiceAmm, (object)null);
        }

        public void ElencoUtentiConnessiAsync(string codiceAmm, object userState)
        {
            if (this.ElencoUtentiConnessiOperationCompleted == null)
                this.ElencoUtentiConnessiOperationCompleted = new SendOrPostCallback(this.OnElencoUtentiConnessiOperationCompleted);
            this.InvokeAsync("ElencoUtentiConnessi", new object[1]
      {
        (object) codiceAmm
      }, this.ElencoUtentiConnessiOperationCompleted, userState);
        }

        private void OnElencoUtentiConnessiOperationCompleted(object arg)
        {
            if (this.ElencoUtentiConnessiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ElencoUtentiConnessiCompleted((object)this, new ElencoUtentiConnessiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DisconnettiUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DisconnettiUtente(string userId, string codiceAmm)
        {
            return (bool)this.Invoke("DisconnettiUtente", new object[2]
      {
        (object) userId,
        (object) codiceAmm
      })[0];
        }

        public IAsyncResult BeginDisconnettiUtente(string userId, string codiceAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DisconnettiUtente", new object[2]
      {
        (object) userId,
        (object) codiceAmm
      }, callback, asyncState);
        }

        public bool EndDisconnettiUtente(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DisconnettiUtenteAsync(string userId, string codiceAmm)
        {
            this.DisconnettiUtenteAsync(userId, codiceAmm, (object)null);
        }

        public void DisconnettiUtenteAsync(string userId, string codiceAmm, object userState)
        {
            if (this.DisconnettiUtenteOperationCompleted == null)
                this.DisconnettiUtenteOperationCompleted = new SendOrPostCallback(this.OnDisconnettiUtenteOperationCompleted);
            this.InvokeAsync("DisconnettiUtente", new object[2]
      {
        (object) userId,
        (object) codiceAmm
      }, this.DisconnettiUtenteOperationCompleted, userState);
        }

        private void OnDisconnettiUtenteOperationCompleted(object arg)
        {
            if (this.DisconnettiUtenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DisconnettiUtenteCompleted((object)this, new DisconnettiUtenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/logoff", ParameterStyle = SoapParameterStyle.Wrapped, RequestElementName = "logoff", RequestNamespace = "http://localhost", ResponseElementName = "logoffResponse", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        [return: XmlElement("logoffResult")]
        public bool LogoffRDE(InfoUtente infoUtente)
        {
            return (bool)this.Invoke("LogoffRDE", new object[1]
      {
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginLogoffRDE(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("LogoffRDE", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public bool EndLogoffRDE(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void LogoffRDEAsync(InfoUtente infoUtente)
        {
            this.LogoffRDEAsync(infoUtente, (object)null);
        }

        public void LogoffRDEAsync(InfoUtente infoUtente, object userState)
        {
            if (this.LogoffRDEOperationCompleted == null)
                this.LogoffRDEOperationCompleted = new SendOrPostCallback(this.OnLogoffRDEOperationCompleted);
            this.InvokeAsync("LogoffRDE", new object[1]
      {
        (object) infoUtente
      }, this.LogoffRDEOperationCompleted, userState);
        }

        private void OnLogoffRDEOperationCompleted(object arg)
        {
            if (this.LogoffRDECompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.LogoffRDECompleted((object)this, new LogoffRDECompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/LogoffUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestElementName = "LogoffUtente", RequestNamespace = "http://localhost", ResponseElementName = "LogoffUtenteResponse", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        [return: XmlElement("LogoffUtenteResult")]
        public bool Logoff(string userId, string idAmm, string sessionId, string dst)
        {
            return (bool)this.Invoke("Logoff", new object[4]
      {
        (object) userId,
        (object) idAmm,
        (object) sessionId,
        (object) dst
      })[0];
        }

        public IAsyncResult BeginLogoff(string userId, string idAmm, string sessionId, string dst, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Logoff", new object[4]
      {
        (object) userId,
        (object) idAmm,
        (object) sessionId,
        (object) dst
      }, callback, asyncState);
        }

        public bool EndLogoff(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void LogoffAsync(string userId, string idAmm, string sessionId, string dst)
        {
            this.LogoffAsync(userId, idAmm, sessionId, dst, (object)null);
        }

        public void LogoffAsync(string userId, string idAmm, string sessionId, string dst, object userState)
        {
            if (this.LogoffOperationCompleted == null)
                this.LogoffOperationCompleted = new SendOrPostCallback(this.OnLogoffOperationCompleted);
            this.InvokeAsync("Logoff", new object[4]
      {
        (object) userId,
        (object) idAmm,
        (object) sessionId,
        (object) dst
      }, this.LogoffOperationCompleted, userState);
        }

        private void OnLogoffOperationCompleted(object arg)
        {
            if (this.LogoffCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.LogoffCompleted((object)this, new LogoffCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ValidateLogin", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public ValidationResult ValidateLogin(string userID, string idAmm, string webSessionId)
        {
            return (ValidationResult)this.Invoke("ValidateLogin", new object[3]
      {
        (object) userID,
        (object) idAmm,
        (object) webSessionId
      })[0];
        }

        public IAsyncResult BeginValidateLogin(string userID, string idAmm, string webSessionId, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ValidateLogin", new object[3]
      {
        (object) userID,
        (object) idAmm,
        (object) webSessionId
      }, callback, asyncState);
        }

        public ValidationResult EndValidateLogin(IAsyncResult asyncResult)
        {
            return (ValidationResult)this.EndInvoke(asyncResult)[0];
        }

        public void ValidateLoginAsync(string userID, string idAmm, string webSessionId)
        {
            this.ValidateLoginAsync(userID, idAmm, webSessionId, (object)null);
        }

        public void ValidateLoginAsync(string userID, string idAmm, string webSessionId, object userState)
        {
            if (this.ValidateLoginOperationCompleted == null)
                this.ValidateLoginOperationCompleted = new SendOrPostCallback(this.OnValidateLoginOperationCompleted);
            this.InvokeAsync("ValidateLogin", new object[3]
      {
        (object) userID,
        (object) idAmm,
        (object) webSessionId
      }, this.ValidateLoginOperationCompleted, userState);
        }

        private void OnValidateLoginOperationCompleted(object arg)
        {
            if (this.ValidateLoginCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ValidateLoginCompleted((object)this, new ValidateLoginCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/LoginUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestElementName = "LoginUtente", RequestNamespace = "http://localhost", ResponseElementName = "LoginUtenteResponse", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        [return: XmlElement("LoginUtenteResult")]
        public LoginResult Login([XmlElement("login")] UserLogin login1, bool forced, string webSessionId, out Utente utente, out string ipAddress)
        {
            object[] objArray = this.Invoke("Login", new object[3]
      {
        (object) login1,
        (object) forced,
        (object) webSessionId
      });
            utente = (Utente)objArray[1];
            ipAddress = (string)objArray[2];
            return (LoginResult)objArray[0];
        }

        public IAsyncResult BeginLogin(UserLogin login1, bool forced, string webSessionId, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Login", new object[3]
      {
        (object) login1,
        (object) forced,
        (object) webSessionId
      }, callback, asyncState);
        }

        public LoginResult EndLogin(IAsyncResult asyncResult, out Utente utente, out string ipAddress)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            utente = (Utente)objArray[1];
            ipAddress = (string)objArray[2];
            return (LoginResult)objArray[0];
        }

        public void LoginAsync(UserLogin login1, bool forced, string webSessionId)
        {
            this.LoginAsync(login1, forced, webSessionId, (object)null);
        }

        public void LoginAsync(UserLogin login1, bool forced, string webSessionId, object userState)
        {
            if (this.LoginOperationCompleted == null)
                this.LoginOperationCompleted = new SendOrPostCallback(this.OnLoginOperationCompleted);
            this.InvokeAsync("Login", new object[3]
      {
        (object) login1,
        (object) forced,
        (object) webSessionId
      }, this.LoginOperationCompleted, userState);
        }

        private void OnLoginOperationCompleted(object arg)
        {
            if (this.LoginCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.LoginCompleted((object)this, new LoginCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/SimpleLogin", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Utente SimpleLogin(string login)
        {
            return (Utente)this.Invoke("SimpleLogin", new object[1]
      {
        (object) login
      })[0];
        }

        public IAsyncResult BeginSimpleLogin(string login, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SimpleLogin", new object[1]
      {
        (object) login
      }, callback, asyncState);
        }

        public Utente EndSimpleLogin(IAsyncResult asyncResult)
        {
            return (Utente)this.EndInvoke(asyncResult)[0];
        }

        public void SimpleLoginAsync(string login)
        {
            this.SimpleLoginAsync(login, (object)null);
        }

        public void SimpleLoginAsync(string login, object userState)
        {
            if (this.SimpleLoginOperationCompleted == null)
                this.SimpleLoginOperationCompleted = new SendOrPostCallback(this.OnSimpleLoginOperationCompleted);
            this.InvokeAsync("SimpleLogin", new object[1]
      {
        (object) login
      }, this.SimpleLoginOperationCompleted, userState);
        }

        private void OnSimpleLoginOperationCompleted(object arg)
        {
            if (this.SimpleLoginCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.SimpleLoginCompleted((object)this, new SimpleLoginCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/CheckDatabaseConnection", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool CheckDatabaseConnection(out string exceptionMessage)
        {
            object[] objArray = this.Invoke("CheckDatabaseConnection", new object[0]);
            exceptionMessage = (string)objArray[1];
            return (bool)objArray[0];
        }

        public IAsyncResult BeginCheckDatabaseConnection(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CheckDatabaseConnection", new object[0], callback, asyncState);
        }

        public bool EndCheckDatabaseConnection(IAsyncResult asyncResult, out string exceptionMessage)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            exceptionMessage = (string)objArray[1];
            return (bool)objArray[0];
        }

        public void CheckDatabaseConnectionAsync()
        {
            this.CheckDatabaseConnectionAsync((object)null);
        }

        public void CheckDatabaseConnectionAsync(object userState)
        {
            if (this.CheckDatabaseConnectionOperationCompleted == null)
                this.CheckDatabaseConnectionOperationCompleted = new SendOrPostCallback(this.OnCheckDatabaseConnectionOperationCompleted);
            this.InvokeAsync("CheckDatabaseConnection", new object[0], this.CheckDatabaseConnectionOperationCompleted, userState);
        }

        private void OnCheckDatabaseConnectionOperationCompleted(object arg)
        {
            if (this.CheckDatabaseConnectionCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.CheckDatabaseConnectionCompleted((object)this, new CheckDatabaseConnectionCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/CheckDPA_Amministra", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool CheckDPA_Amministra(out string exceptionMessage)
        {
            object[] objArray = this.Invoke("CheckDPA_Amministra", new object[0]);
            exceptionMessage = (string)objArray[1];
            return (bool)objArray[0];
        }

        public IAsyncResult BeginCheckDPA_Amministra(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CheckDPA_Amministra", new object[0], callback, asyncState);
        }

        public bool EndCheckDPA_Amministra(IAsyncResult asyncResult, out string exceptionMessage)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            exceptionMessage = (string)objArray[1];
            return (bool)objArray[0];
        }

        public void CheckDPA_AmministraAsync()
        {
            this.CheckDPA_AmministraAsync((object)null);
        }

        public void CheckDPA_AmministraAsync(object userState)
        {
            if (this.CheckDPA_AmministraOperationCompleted == null)
                this.CheckDPA_AmministraOperationCompleted = new SendOrPostCallback(this.OnCheckDPA_AmministraOperationCompleted);
            this.InvokeAsync("CheckDPA_Amministra", new object[0], this.CheckDPA_AmministraOperationCompleted, userState);
        }

        private void OnCheckDPA_AmministraOperationCompleted(object arg)
        {
            if (this.CheckDPA_AmministraCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.CheckDPA_AmministraCompleted((object)this, new CheckDPA_AmministraCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/IsInternalProtocolEnabled", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool IsInternalProtocolEnabled(string idAmm)
        {
            return (bool)this.Invoke("IsInternalProtocolEnabled", new object[1]
      {
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginIsInternalProtocolEnabled(string idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("IsInternalProtocolEnabled", new object[1]
      {
        (object) idAmm
      }, callback, asyncState);
        }

        public bool EndIsInternalProtocolEnabled(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void IsInternalProtocolEnabledAsync(string idAmm)
        {
            this.IsInternalProtocolEnabledAsync(idAmm, (object)null);
        }

        public void IsInternalProtocolEnabledAsync(string idAmm, object userState)
        {
            if (this.IsInternalProtocolEnabledOperationCompleted == null)
                this.IsInternalProtocolEnabledOperationCompleted = new SendOrPostCallback(this.OnIsInternalProtocolEnabledOperationCompleted);
            this.InvokeAsync("IsInternalProtocolEnabled", new object[1]
      {
        (object) idAmm
      }, this.IsInternalProtocolEnabledOperationCompleted, userState);
        }

        private void OnIsInternalProtocolEnabledOperationCompleted(object arg)
        {
            if (this.IsInternalProtocolEnabledCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.IsInternalProtocolEnabledCompleted((object)this, new IsInternalProtocolEnabledCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DocumentoGetTipoProto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string DocumentoGetTipoProto(string docNumber)
        {
            return (string)this.Invoke("DocumentoGetTipoProto", new object[1]
      {
        (object) docNumber
      })[0];
        }

        public IAsyncResult BeginDocumentoGetTipoProto(string docNumber, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DocumentoGetTipoProto", new object[1]
      {
        (object) docNumber
      }, callback, asyncState);
        }

        public string EndDocumentoGetTipoProto(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void DocumentoGetTipoProtoAsync(string docNumber)
        {
            this.DocumentoGetTipoProtoAsync(docNumber, (object)null);
        }

        public void DocumentoGetTipoProtoAsync(string docNumber, object userState)
        {
            if (this.DocumentoGetTipoProtoOperationCompleted == null)
                this.DocumentoGetTipoProtoOperationCompleted = new SendOrPostCallback(this.OnDocumentoGetTipoProtoOperationCompleted);
            this.InvokeAsync("DocumentoGetTipoProto", new object[1]
      {
        (object) docNumber
      }, this.DocumentoGetTipoProtoOperationCompleted, userState);
        }

        private void OnDocumentoGetTipoProtoOperationCompleted(object arg)
        {
            if (this.DocumentoGetTipoProtoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DocumentoGetTipoProtoCompleted((object)this, new DocumentoGetTipoProtoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetVisibilitaRuoli", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string[] GetVisibilitaRuoli(string docNumber, string idGruppi)
        {
            return (string[])this.Invoke("GetVisibilitaRuoli", new object[2]
      {
        (object) docNumber,
        (object) idGruppi
      })[0];
        }

        public IAsyncResult BeginGetVisibilitaRuoli(string docNumber, string idGruppi, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetVisibilitaRuoli", new object[2]
      {
        (object) docNumber,
        (object) idGruppi
      }, callback, asyncState);
        }

        public string[] EndGetVisibilitaRuoli(IAsyncResult asyncResult)
        {
            return (string[])this.EndInvoke(asyncResult)[0];
        }

        public void GetVisibilitaRuoliAsync(string docNumber, string idGruppi)
        {
            this.GetVisibilitaRuoliAsync(docNumber, idGruppi, (object)null);
        }

        public void GetVisibilitaRuoliAsync(string docNumber, string idGruppi, object userState)
        {
            if (this.GetVisibilitaRuoliOperationCompleted == null)
                this.GetVisibilitaRuoliOperationCompleted = new SendOrPostCallback(this.OnGetVisibilitaRuoliOperationCompleted);
            this.InvokeAsync("GetVisibilitaRuoli", new object[2]
      {
        (object) docNumber,
        (object) idGruppi
      }, this.GetVisibilitaRuoliOperationCompleted, userState);
        }

        private void OnGetVisibilitaRuoliOperationCompleted(object arg)
        {
            if (this.GetVisibilitaRuoliCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetVisibilitaRuoliCompleted((object)this, new GetVisibilitaRuoliCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioniSendSollecito", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void trasmissioniSendSollecito(string path, Trasmissione trasm)
        {
            this.Invoke("trasmissioniSendSollecito", new object[2]
      {
        (object) path,
        (object) trasm
      });
        }

        public IAsyncResult BegintrasmissioniSendSollecito(string path, Trasmissione trasm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioniSendSollecito", new object[2]
      {
        (object) path,
        (object) trasm
      }, callback, asyncState);
        }

        public void EndtrasmissioniSendSollecito(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void trasmissioniSendSollecitoAsync(string path, Trasmissione trasm)
        {
            this.trasmissioniSendSollecitoAsync(path, trasm, (object)null);
        }

        public void trasmissioniSendSollecitoAsync(string path, Trasmissione trasm, object userState)
        {
            if (this.trasmissioniSendSollecitoOperationCompleted == null)
                this.trasmissioniSendSollecitoOperationCompleted = new SendOrPostCallback(this.OntrasmissioniSendSollecitoOperationCompleted);
            this.InvokeAsync("trasmissioniSendSollecito", new object[2]
      {
        (object) path,
        (object) trasm
      }, this.trasmissioniSendSollecitoOperationCompleted, userState);
        }

        private void OntrasmissioniSendSollecitoOperationCompleted(object arg)
        {
            if (this.trasmissioniSendSollecitoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioniSendSollecitoCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/TrasmettiProtocolloInterno", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool TrasmettiProtocolloInterno(SchedaDocumento schedaDoc, Ruolo ruolo, string serverName, bool isEnableRef, out bool RagioniVerificate, out string message)
        {
            object[] objArray = this.Invoke("TrasmettiProtocolloInterno", new object[4]
      {
        (object) schedaDoc,
        (object) ruolo,
        (object) serverName,
        (object) isEnableRef
      });
            RagioniVerificate = (bool)objArray[1];
            message = (string)objArray[2];
            return (bool)objArray[0];
        }

        public IAsyncResult BeginTrasmettiProtocolloInterno(SchedaDocumento schedaDoc, Ruolo ruolo, string serverName, bool isEnableRef, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("TrasmettiProtocolloInterno", new object[4]
      {
        (object) schedaDoc,
        (object) ruolo,
        (object) serverName,
        (object) isEnableRef
      }, callback, asyncState);
        }

        public bool EndTrasmettiProtocolloInterno(IAsyncResult asyncResult, out bool RagioniVerificate, out string message)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            RagioniVerificate = (bool)objArray[1];
            message = (string)objArray[2];
            return (bool)objArray[0];
        }

        public void TrasmettiProtocolloInternoAsync(SchedaDocumento schedaDoc, Ruolo ruolo, string serverName, bool isEnableRef)
        {
            this.TrasmettiProtocolloInternoAsync(schedaDoc, ruolo, serverName, isEnableRef, (object)null);
        }

        public void TrasmettiProtocolloInternoAsync(SchedaDocumento schedaDoc, Ruolo ruolo, string serverName, bool isEnableRef, object userState)
        {
            if (this.TrasmettiProtocolloInternoOperationCompleted == null)
                this.TrasmettiProtocolloInternoOperationCompleted = new SendOrPostCallback(this.OnTrasmettiProtocolloInternoOperationCompleted);
            this.InvokeAsync("TrasmettiProtocolloInterno", new object[4]
      {
        (object) schedaDoc,
        (object) ruolo,
        (object) serverName,
        (object) isEnableRef
      }, this.TrasmettiProtocolloInternoOperationCompleted, userState);
        }

        private void OnTrasmettiProtocolloInternoOperationCompleted(object arg)
        {
            if (this.TrasmettiProtocolloInternoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.TrasmettiProtocolloInternoCompleted((object)this, new TrasmettiProtocolloInternoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetRagioneTrasmissione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public RagioneTrasmissione GetRagioneTrasmissione(string tipoRagione, Ruolo ruolo, out bool RagioniVerificate)
        {
            object[] objArray = this.Invoke("GetRagioneTrasmissione", new object[2]
      {
        (object) tipoRagione,
        (object) ruolo
      });
            RagioniVerificate = (bool)objArray[1];
            return (RagioneTrasmissione)objArray[0];
        }

        public IAsyncResult BeginGetRagioneTrasmissione(string tipoRagione, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetRagioneTrasmissione", new object[2]
      {
        (object) tipoRagione,
        (object) ruolo
      }, callback, asyncState);
        }

        public RagioneTrasmissione EndGetRagioneTrasmissione(IAsyncResult asyncResult, out bool RagioniVerificate)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            RagioniVerificate = (bool)objArray[1];
            return (RagioneTrasmissione)objArray[0];
        }

        public void GetRagioneTrasmissioneAsync(string tipoRagione, Ruolo ruolo)
        {
            this.GetRagioneTrasmissioneAsync(tipoRagione, ruolo, (object)null);
        }

        public void GetRagioneTrasmissioneAsync(string tipoRagione, Ruolo ruolo, object userState)
        {
            if (this.GetRagioneTrasmissioneOperationCompleted == null)
                this.GetRagioneTrasmissioneOperationCompleted = new SendOrPostCallback(this.OnGetRagioneTrasmissioneOperationCompleted);
            this.InvokeAsync("GetRagioneTrasmissione", new object[2]
      {
        (object) tipoRagione,
        (object) ruolo
      }, this.GetRagioneTrasmissioneOperationCompleted, userState);
        }

        private void OnGetRagioneTrasmissioneOperationCompleted(object arg)
        {
            if (this.GetRagioneTrasmissioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetRagioneTrasmissioneCompleted((object)this, new GetRagioneTrasmissioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/UOHasReferenceRole", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool UOHasReferenceRole(string idUO)
        {
            return (bool)this.Invoke("UOHasReferenceRole", new object[1] { (object)idUO })[0];
        }

        public IAsyncResult BeginUOHasReferenceRole(string idUO, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("UOHasReferenceRole", new object[1] { (object)idUO }, callback, asyncState);
        }

        public bool EndUOHasReferenceRole(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void UOHasReferenceRoleAsync(string idUO)
        {
            this.UOHasReferenceRoleAsync(idUO, (object)null);
        }

        public void UOHasReferenceRoleAsync(string idUO, object userState)
        {
            if (this.UOHasReferenceRoleOperationCompleted == null)
                this.UOHasReferenceRoleOperationCompleted = new SendOrPostCallback(this.OnUOHasReferenceRoleOperationCompleted);
            this.InvokeAsync("UOHasReferenceRole", new object[1] { (object)idUO }, this.UOHasReferenceRoleOperationCompleted, userState);
        }

        private void OnUOHasReferenceRoleOperationCompleted(object arg)
        {
            if (this.UOHasReferenceRoleCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.UOHasReferenceRoleCompleted((object)this, new UOHasReferenceRoleCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/reportFascetteFascicolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileDocumento reportFascetteFascicolo(Fascicolo fascicolo)
        {
            return (FileDocumento)this.Invoke("reportFascetteFascicolo", new object[1]
      {
        (object) fascicolo
      })[0];
        }

        public IAsyncResult BeginreportFascetteFascicolo(Fascicolo fascicolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("reportFascetteFascicolo", new object[1]
      {
        (object) fascicolo
      }, callback, asyncState);
        }

        public FileDocumento EndreportFascetteFascicolo(IAsyncResult asyncResult)
        {
            return (FileDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void reportFascetteFascicoloAsync(Fascicolo fascicolo)
        {
            this.reportFascetteFascicoloAsync(fascicolo, (object)null);
        }

        public void reportFascetteFascicoloAsync(Fascicolo fascicolo, object userState)
        {
            if (this.reportFascetteFascicoloOperationCompleted == null)
                this.reportFascetteFascicoloOperationCompleted = new SendOrPostCallback(this.OnreportFascetteFascicoloOperationCompleted);
            this.InvokeAsync("reportFascetteFascicolo", new object[1]
      {
        (object) fascicolo
      }, this.reportFascetteFascicoloOperationCompleted, userState);
        }

        private void OnreportFascetteFascicoloOperationCompleted(object arg)
        {
            if (this.reportFascetteFascicoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.reportFascetteFascicoloCompleted((object)this, new reportFascetteFascicoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AmmGetIDAmm", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string AmmGetIDAmm(string codAmm)
        {
            return (string)this.Invoke("AmmGetIDAmm", new object[1]
      {
        (object) codAmm
      })[0];
        }

        public IAsyncResult BeginAmmGetIDAmm(string codAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AmmGetIDAmm", new object[1]
      {
        (object) codAmm
      }, callback, asyncState);
        }

        public string EndAmmGetIDAmm(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void AmmGetIDAmmAsync(string codAmm)
        {
            this.AmmGetIDAmmAsync(codAmm, (object)null);
        }

        public void AmmGetIDAmmAsync(string codAmm, object userState)
        {
            if (this.AmmGetIDAmmOperationCompleted == null)
                this.AmmGetIDAmmOperationCompleted = new SendOrPostCallback(this.OnAmmGetIDAmmOperationCompleted);
            this.InvokeAsync("AmmGetIDAmm", new object[1]
      {
        (object) codAmm
      }, this.AmmGetIDAmmOperationCompleted, userState);
        }

        private void OnAmmGetIDAmmOperationCompleted(object arg)
        {
            if (this.AmmGetIDAmmCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AmmGetIDAmmCompleted((object)this, new AmmGetIDAmmCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetFilesLog", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string[] GetFilesLog(string codAmm)
        {
            return (string[])this.Invoke("GetFilesLog", new object[1]
      {
        (object) codAmm
      })[0];
        }

        public IAsyncResult BeginGetFilesLog(string codAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetFilesLog", new object[1]
      {
        (object) codAmm
      }, callback, asyncState);
        }

        public string[] EndGetFilesLog(IAsyncResult asyncResult)
        {
            return (string[])this.EndInvoke(asyncResult)[0];
        }

        public void GetFilesLogAsync(string codAmm)
        {
            this.GetFilesLogAsync(codAmm, (object)null);
        }

        public void GetFilesLogAsync(string codAmm, object userState)
        {
            if (this.GetFilesLogOperationCompleted == null)
                this.GetFilesLogOperationCompleted = new SendOrPostCallback(this.OnGetFilesLogOperationCompleted);
            this.InvokeAsync("GetFilesLog", new object[1]
      {
        (object) codAmm
      }, this.GetFilesLogOperationCompleted, userState);
        }

        private void OnGetFilesLogOperationCompleted(object arg)
        {
            if (this.GetFilesLogCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetFilesLogCompleted((object)this, new GetFilesLogCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/StampaPDFLog", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool StampaPDFLog(string codAmm)
        {
            return (bool)this.Invoke("StampaPDFLog", new object[1]
      {
        (object) codAmm
      })[0];
        }

        public IAsyncResult BeginStampaPDFLog(string codAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("StampaPDFLog", new object[1]
      {
        (object) codAmm
      }, callback, asyncState);
        }

        public bool EndStampaPDFLog(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void StampaPDFLogAsync(string codAmm)
        {
            this.StampaPDFLogAsync(codAmm, (object)null);
        }

        public void StampaPDFLogAsync(string codAmm, object userState)
        {
            if (this.StampaPDFLogOperationCompleted == null)
                this.StampaPDFLogOperationCompleted = new SendOrPostCallback(this.OnStampaPDFLogOperationCompleted);
            this.InvokeAsync("StampaPDFLog", new object[1]
      {
        (object) codAmm
      }, this.StampaPDFLogOperationCompleted, userState);
        }

        private void OnStampaPDFLogOperationCompleted(object arg)
        {
            if (this.StampaPDFLogCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.StampaPDFLogCompleted((object)this, new StampaPDFLogCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ContaLog", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string ContaLog(string codAmm)
        {
            return (string)this.Invoke("ContaLog", new object[1]
      {
        (object) codAmm
      })[0];
        }

        public IAsyncResult BeginContaLog(string codAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ContaLog", new object[1]
      {
        (object) codAmm
      }, callback, asyncState);
        }

        public string EndContaLog(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void ContaLogAsync(string codAmm)
        {
            this.ContaLogAsync(codAmm, (object)null);
        }

        public void ContaLogAsync(string codAmm, object userState)
        {
            if (this.ContaLogOperationCompleted == null)
                this.ContaLogOperationCompleted = new SendOrPostCallback(this.OnContaLogOperationCompleted);
            this.InvokeAsync("ContaLog", new object[1]
      {
        (object) codAmm
      }, this.ContaLogOperationCompleted, userState);
        }

        private void OnContaLogOperationCompleted(object arg)
        {
            if (this.ContaLogCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ContaLogCompleted((object)this, new ContaLogCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetXMLLogFiltrato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string GetXMLLogFiltrato(string dataDa, string dataA, string user, string oggetto, string azione, string codAmm, string esito)
        {
            return (string)this.Invoke("GetXMLLogFiltrato", new object[7]
      {
        (object) dataDa,
        (object) dataA,
        (object) user,
        (object) oggetto,
        (object) azione,
        (object) codAmm,
        (object) esito
      })[0];
        }

        public IAsyncResult BeginGetXMLLogFiltrato(string dataDa, string dataA, string user, string oggetto, string azione, string codAmm, string esito, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetXMLLogFiltrato", new object[7]
      {
        (object) dataDa,
        (object) dataA,
        (object) user,
        (object) oggetto,
        (object) azione,
        (object) codAmm,
        (object) esito
      }, callback, asyncState);
        }

        public string EndGetXMLLogFiltrato(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void GetXMLLogFiltratoAsync(string dataDa, string dataA, string user, string oggetto, string azione, string codAmm, string esito)
        {
            this.GetXMLLogFiltratoAsync(dataDa, dataA, user, oggetto, azione, codAmm, esito, (object)null);
        }

        public void GetXMLLogFiltratoAsync(string dataDa, string dataA, string user, string oggetto, string azione, string codAmm, string esito, object userState)
        {
            if (this.GetXMLLogFiltratoOperationCompleted == null)
                this.GetXMLLogFiltratoOperationCompleted = new SendOrPostCallback(this.OnGetXMLLogFiltratoOperationCompleted);
            this.InvokeAsync("GetXMLLogFiltrato", new object[7]
      {
        (object) dataDa,
        (object) dataA,
        (object) user,
        (object) oggetto,
        (object) azione,
        (object) codAmm,
        (object) esito
      }, this.GetXMLLogFiltratoOperationCompleted, userState);
        }

        private void OnGetXMLLogFiltratoOperationCompleted(object arg)
        {
            if (this.GetXMLLogFiltratoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetXMLLogFiltratoCompleted((object)this, new GetXMLLogFiltratoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/SetXMLLog", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool SetXMLLog(string stream, string codAmm)
        {
            return (bool)this.Invoke("SetXMLLog", new object[2]
      {
        (object) stream,
        (object) codAmm
      })[0];
        }

        public IAsyncResult BeginSetXMLLog(string stream, string codAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SetXMLLog", new object[2]
      {
        (object) stream,
        (object) codAmm
      }, callback, asyncState);
        }

        public bool EndSetXMLLog(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void SetXMLLogAsync(string stream, string codAmm)
        {
            this.SetXMLLogAsync(stream, codAmm, (object)null);
        }

        public void SetXMLLogAsync(string stream, string codAmm, object userState)
        {
            if (this.SetXMLLogOperationCompleted == null)
                this.SetXMLLogOperationCompleted = new SendOrPostCallback(this.OnSetXMLLogOperationCompleted);
            this.InvokeAsync("SetXMLLog", new object[2]
      {
        (object) stream,
        (object) codAmm
      }, this.SetXMLLogOperationCompleted, userState);
        }

        private void OnSetXMLLogOperationCompleted(object arg)
        {
            if (this.SetXMLLogCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.SetXMLLogCompleted((object)this, new SetXMLLogCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetXMLLog", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string GetXMLLog(string codAmm)
        {
            return (string)this.Invoke("GetXMLLog", new object[1]
      {
        (object) codAmm
      })[0];
        }

        public IAsyncResult BeginGetXMLLog(string codAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetXMLLog", new object[1]
      {
        (object) codAmm
      }, callback, asyncState);
        }

        public string EndGetXMLLog(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void GetXMLLogAsync(string codAmm)
        {
            this.GetXMLLogAsync(codAmm, (object)null);
        }

        public void GetXMLLogAsync(string codAmm, object userState)
        {
            if (this.GetXMLLogOperationCompleted == null)
                this.GetXMLLogOperationCompleted = new SendOrPostCallback(this.OnGetXMLLogOperationCompleted);
            this.InvokeAsync("GetXMLLog", new object[1]
      {
        (object) codAmm
      }, this.GetXMLLogOperationCompleted, userState);
        }

        private void OnGetXMLLogOperationCompleted(object arg)
        {
            if (this.GetXMLLogCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetXMLLogCompleted((object)this, new GetXMLLogCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/MoveRoleToNewUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool MoveRoleToNewUO(string codRuolo, string codAmm, string codNewUO, string descNewUO, string codTipoRuolo, string descTipoRuolo, string codNewRuolo, string descNewRuolo)
        {
            return (bool)this.Invoke("MoveRoleToNewUO", new object[8]
      {
        (object) codRuolo,
        (object) codAmm,
        (object) codNewUO,
        (object) descNewUO,
        (object) codTipoRuolo,
        (object) descTipoRuolo,
        (object) codNewRuolo,
        (object) descNewRuolo
      })[0];
        }

        public IAsyncResult BeginMoveRoleToNewUO(string codRuolo, string codAmm, string codNewUO, string descNewUO, string codTipoRuolo, string descTipoRuolo, string codNewRuolo, string descNewRuolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("MoveRoleToNewUO", new object[8]
      {
        (object) codRuolo,
        (object) codAmm,
        (object) codNewUO,
        (object) descNewUO,
        (object) codTipoRuolo,
        (object) descTipoRuolo,
        (object) codNewRuolo,
        (object) descNewRuolo
      }, callback, asyncState);
        }

        public bool EndMoveRoleToNewUO(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void MoveRoleToNewUOAsync(string codRuolo, string codAmm, string codNewUO, string descNewUO, string codTipoRuolo, string descTipoRuolo, string codNewRuolo, string descNewRuolo)
        {
            this.MoveRoleToNewUOAsync(codRuolo, codAmm, codNewUO, descNewUO, codTipoRuolo, descTipoRuolo, codNewRuolo, descNewRuolo, (object)null);
        }

        public void MoveRoleToNewUOAsync(string codRuolo, string codAmm, string codNewUO, string descNewUO, string codTipoRuolo, string descTipoRuolo, string codNewRuolo, string descNewRuolo, object userState)
        {
            if (this.MoveRoleToNewUOOperationCompleted == null)
                this.MoveRoleToNewUOOperationCompleted = new SendOrPostCallback(this.OnMoveRoleToNewUOOperationCompleted);
            this.InvokeAsync("MoveRoleToNewUO", new object[8]
      {
        (object) codRuolo,
        (object) codAmm,
        (object) codNewUO,
        (object) descNewUO,
        (object) codTipoRuolo,
        (object) descTipoRuolo,
        (object) codNewRuolo,
        (object) descNewRuolo
      }, this.MoveRoleToNewUOOperationCompleted, userState);
        }

        private void OnMoveRoleToNewUOOperationCompleted(object arg)
        {
            if (this.MoveRoleToNewUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.MoveRoleToNewUOCompleted((object)this, new MoveRoleToNewUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DeleteUserStatus", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DeleteUserStatus(string userId, string idAmm, string[] newUser, string codRuolo)
        {
            return (bool)this.Invoke("DeleteUserStatus", new object[4]
      {
        (object) userId,
        (object) idAmm,
        (object) newUser,
        (object) codRuolo
      })[0];
        }

        public IAsyncResult BeginDeleteUserStatus(string userId, string idAmm, string[] newUser, string codRuolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeleteUserStatus", new object[4]
      {
        (object) userId,
        (object) idAmm,
        (object) newUser,
        (object) codRuolo
      }, callback, asyncState);
        }

        public bool EndDeleteUserStatus(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DeleteUserStatusAsync(string userId, string idAmm, string[] newUser, string codRuolo)
        {
            this.DeleteUserStatusAsync(userId, idAmm, newUser, codRuolo, (object)null);
        }

        public void DeleteUserStatusAsync(string userId, string idAmm, string[] newUser, string codRuolo, object userState)
        {
            if (this.DeleteUserStatusOperationCompleted == null)
                this.DeleteUserStatusOperationCompleted = new SendOrPostCallback(this.OnDeleteUserStatusOperationCompleted);
            this.InvokeAsync("DeleteUserStatus", new object[4]
      {
        (object) userId,
        (object) idAmm,
        (object) newUser,
        (object) codRuolo
      }, this.DeleteUserStatusOperationCompleted, userState);
        }

        private void OnDeleteUserStatusOperationCompleted(object arg)
        {
            if (this.DeleteUserStatusCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DeleteUserStatusCompleted((object)this, new DeleteUserStatusCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DeleteUserInRole", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DeleteUserInRole(string codUtente, string codAmm, string codNewUtente, string codRuolo)
        {
            return (bool)this.Invoke("DeleteUserInRole", new object[4]
      {
        (object) codUtente,
        (object) codAmm,
        (object) codNewUtente,
        (object) codRuolo
      })[0];
        }

        public IAsyncResult BeginDeleteUserInRole(string codUtente, string codAmm, string codNewUtente, string codRuolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeleteUserInRole", new object[4]
      {
        (object) codUtente,
        (object) codAmm,
        (object) codNewUtente,
        (object) codRuolo
      }, callback, asyncState);
        }

        public bool EndDeleteUserInRole(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DeleteUserInRoleAsync(string codUtente, string codAmm, string codNewUtente, string codRuolo)
        {
            this.DeleteUserInRoleAsync(codUtente, codAmm, codNewUtente, codRuolo, (object)null);
        }

        public void DeleteUserInRoleAsync(string codUtente, string codAmm, string codNewUtente, string codRuolo, object userState)
        {
            if (this.DeleteUserInRoleOperationCompleted == null)
                this.DeleteUserInRoleOperationCompleted = new SendOrPostCallback(this.OnDeleteUserInRoleOperationCompleted);
            this.InvokeAsync("DeleteUserInRole", new object[4]
      {
        (object) codUtente,
        (object) codAmm,
        (object) codNewUtente,
        (object) codRuolo
      }, this.DeleteUserInRoleOperationCompleted, userState);
        }

        private void OnDeleteUserInRoleOperationCompleted(object arg)
        {
            if (this.DeleteUserInRoleCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DeleteUserInRoleCompleted((object)this, new DeleteUserInRoleCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/DeleteOrDisablePeople", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool DeleteOrDisablePeople(string codUtente, string codAmm, bool delete)
        {
            return (bool)this.Invoke("DeleteOrDisablePeople", new object[3]
      {
        (object) codUtente,
        (object) codAmm,
        (object) delete
      })[0];
        }

        public IAsyncResult BeginDeleteOrDisablePeople(string codUtente, string codAmm, bool delete, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeleteOrDisablePeople", new object[3]
      {
        (object) codUtente,
        (object) codAmm,
        (object) delete
      }, callback, asyncState);
        }

        public bool EndDeleteOrDisablePeople(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void DeleteOrDisablePeopleAsync(string codUtente, string codAmm, bool delete)
        {
            this.DeleteOrDisablePeopleAsync(codUtente, codAmm, delete, (object)null);
        }

        public void DeleteOrDisablePeopleAsync(string codUtente, string codAmm, bool delete, object userState)
        {
            if (this.DeleteOrDisablePeopleOperationCompleted == null)
                this.DeleteOrDisablePeopleOperationCompleted = new SendOrPostCallback(this.OnDeleteOrDisablePeopleOperationCompleted);
            this.InvokeAsync("DeleteOrDisablePeople", new object[3]
      {
        (object) codUtente,
        (object) codAmm,
        (object) delete
      }, this.DeleteOrDisablePeopleOperationCompleted, userState);
        }

        private void OnDeleteOrDisablePeopleOperationCompleted(object arg)
        {
            if (this.DeleteOrDisablePeopleCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.DeleteOrDisablePeopleCompleted((object)this, new DeleteOrDisablePeopleCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetUserStatus", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool GetUserStatus(string userId, string idAmm, string codRuolo)
        {
            return (bool)this.Invoke("GetUserStatus", new object[3]
      {
        (object) userId,
        (object) idAmm,
        (object) codRuolo
      })[0];
        }

        public IAsyncResult BeginGetUserStatus(string userId, string idAmm, string codRuolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetUserStatus", new object[3]
      {
        (object) userId,
        (object) idAmm,
        (object) codRuolo
      }, callback, asyncState);
        }

        public bool EndGetUserStatus(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void GetUserStatusAsync(string userId, string idAmm, string codRuolo)
        {
            this.GetUserStatusAsync(userId, idAmm, codRuolo, (object)null);
        }

        public void GetUserStatusAsync(string userId, string idAmm, string codRuolo, object userState)
        {
            if (this.GetUserStatusOperationCompleted == null)
                this.GetUserStatusOperationCompleted = new SendOrPostCallback(this.OnGetUserStatusOperationCompleted);
            this.InvokeAsync("GetUserStatus", new object[3]
      {
        (object) userId,
        (object) idAmm,
        (object) codRuolo
      }, this.GetUserStatusOperationCompleted, userState);
        }

        private void OnGetUserStatusOperationCompleted(object arg)
        {
            if (this.GetUserStatusCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetUserStatusCompleted((object)this, new GetUserStatusCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/CheckUserLogin", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool CheckUserLogin(string userId, string idAmm)
        {
            return (bool)this.Invoke("CheckUserLogin", new object[2]
      {
        (object) userId,
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginCheckUserLogin(string userId, string idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CheckUserLogin", new object[2]
      {
        (object) userId,
        (object) idAmm
      }, callback, asyncState);
        }

        public bool EndCheckUserLogin(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void CheckUserLoginAsync(string userId, string idAmm)
        {
            this.CheckUserLoginAsync(userId, idAmm, (object)null);
        }

        public void CheckUserLoginAsync(string userId, string idAmm, object userState)
        {
            if (this.CheckUserLoginOperationCompleted == null)
                this.CheckUserLoginOperationCompleted = new SendOrPostCallback(this.OnCheckUserLoginOperationCompleted);
            this.InvokeAsync("CheckUserLogin", new object[2]
      {
        (object) userId,
        (object) idAmm
      }, this.CheckUserLoginOperationCompleted, userState);
        }

        private void OnCheckUserLoginOperationCompleted(object arg)
        {
            if (this.CheckUserLoginCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.CheckUserLoginCompleted((object)this, new CheckUserLoginCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/MoveUserFromRole", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool MoveUserFromRole(string codUtente, string codAmm, string codOldRole, string codNewRole, string codUtenteErede)
        {
            return (bool)this.Invoke("MoveUserFromRole", new object[5]
      {
        (object) codUtente,
        (object) codAmm,
        (object) codOldRole,
        (object) codNewRole,
        (object) codUtenteErede
      })[0];
        }

        public IAsyncResult BeginMoveUserFromRole(string codUtente, string codAmm, string codOldRole, string codNewRole, string codUtenteErede, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("MoveUserFromRole", new object[5]
      {
        (object) codUtente,
        (object) codAmm,
        (object) codOldRole,
        (object) codNewRole,
        (object) codUtenteErede
      }, callback, asyncState);
        }

        public bool EndMoveUserFromRole(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void MoveUserFromRoleAsync(string codUtente, string codAmm, string codOldRole, string codNewRole, string codUtenteErede)
        {
            this.MoveUserFromRoleAsync(codUtente, codAmm, codOldRole, codNewRole, codUtenteErede, (object)null);
        }

        public void MoveUserFromRoleAsync(string codUtente, string codAmm, string codOldRole, string codNewRole, string codUtenteErede, object userState)
        {
            if (this.MoveUserFromRoleOperationCompleted == null)
                this.MoveUserFromRoleOperationCompleted = new SendOrPostCallback(this.OnMoveUserFromRoleOperationCompleted);
            this.InvokeAsync("MoveUserFromRole", new object[5]
      {
        (object) codUtente,
        (object) codAmm,
        (object) codOldRole,
        (object) codNewRole,
        (object) codUtenteErede
      }, this.MoveUserFromRoleOperationCompleted, userState);
        }

        private void OnMoveUserFromRoleOperationCompleted(object arg)
        {
            if (this.MoveUserFromRoleCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.MoveUserFromRoleCompleted((object)this, new MoveUserFromRoleCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ExportTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string ExportTitolario()
        {
            return (string)this.Invoke("ExportTitolario", new object[0])[0];
        }

        public IAsyncResult BeginExportTitolario(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ExportTitolario", new object[0], callback, asyncState);
        }

        public string EndExportTitolario(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void ExportTitolarioAsync()
        {
            this.ExportTitolarioAsync((object)null);
        }

        public void ExportTitolarioAsync(object userState)
        {
            if (this.ExportTitolarioOperationCompleted == null)
                this.ExportTitolarioOperationCompleted = new SendOrPostCallback(this.OnExportTitolarioOperationCompleted);
            this.InvokeAsync("ExportTitolario", new object[0], this.ExportTitolarioOperationCompleted, userState);
        }

        private void OnExportTitolarioOperationCompleted(object arg)
        {
            if (this.ExportTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ExportTitolarioCompleted((object)this, new ExportTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ExportAmministrazioni", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string ExportAmministrazioni()
        {
            return (string)this.Invoke("ExportAmministrazioni", new object[0])[0];
        }

        public IAsyncResult BeginExportAmministrazioni(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ExportAmministrazioni", new object[0], callback, asyncState);
        }

        public string EndExportAmministrazioni(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void ExportAmministrazioniAsync()
        {
            this.ExportAmministrazioniAsync((object)null);
        }

        public void ExportAmministrazioniAsync(object userState)
        {
            if (this.ExportAmministrazioniOperationCompleted == null)
                this.ExportAmministrazioniOperationCompleted = new SendOrPostCallback(this.OnExportAmministrazioniOperationCompleted);
            this.InvokeAsync("ExportAmministrazioni", new object[0], this.ExportAmministrazioniOperationCompleted, userState);
        }

        private void OnExportAmministrazioniOperationCompleted(object arg)
        {
            if (this.ExportAmministrazioniCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ExportAmministrazioniCompleted((object)this, new ExportAmministrazioniCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ImportTitolarioReadState", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool ImportTitolarioReadState(out int counter, out int total, out string message)
        {
            object[] objArray = this.Invoke("ImportTitolarioReadState", new object[0]);
            counter = (int)objArray[1];
            total = (int)objArray[2];
            message = (string)objArray[3];
            return (bool)objArray[0];
        }

        public IAsyncResult BeginImportTitolarioReadState(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ImportTitolarioReadState", new object[0], callback, asyncState);
        }

        public bool EndImportTitolarioReadState(IAsyncResult asyncResult, out int counter, out int total, out string message)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            counter = (int)objArray[1];
            total = (int)objArray[2];
            message = (string)objArray[3];
            return (bool)objArray[0];
        }

        public void ImportTitolarioReadStateAsync()
        {
            this.ImportTitolarioReadStateAsync((object)null);
        }

        public void ImportTitolarioReadStateAsync(object userState)
        {
            if (this.ImportTitolarioReadStateOperationCompleted == null)
                this.ImportTitolarioReadStateOperationCompleted = new SendOrPostCallback(this.OnImportTitolarioReadStateOperationCompleted);
            this.InvokeAsync("ImportTitolarioReadState", new object[0], this.ImportTitolarioReadStateOperationCompleted, userState);
        }

        private void OnImportTitolarioReadStateOperationCompleted(object arg)
        {
            if (this.ImportTitolarioReadStateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ImportTitolarioReadStateCompleted((object)this, new ImportTitolarioReadStateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ImportDocumentiReadState", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool ImportDocumentiReadState(out int counter, out int total, out string message)
        {
            object[] objArray = this.Invoke("ImportDocumentiReadState", new object[0]);
            counter = (int)objArray[1];
            total = (int)objArray[2];
            message = (string)objArray[3];
            return (bool)objArray[0];
        }

        public IAsyncResult BeginImportDocumentiReadState(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ImportDocumentiReadState", new object[0], callback, asyncState);
        }

        public bool EndImportDocumentiReadState(IAsyncResult asyncResult, out int counter, out int total, out string message)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            counter = (int)objArray[1];
            total = (int)objArray[2];
            message = (string)objArray[3];
            return (bool)objArray[0];
        }

        public void ImportDocumentiReadStateAsync()
        {
            this.ImportDocumentiReadStateAsync((object)null);
        }

        public void ImportDocumentiReadStateAsync(object userState)
        {
            if (this.ImportDocumentiReadStateOperationCompleted == null)
                this.ImportDocumentiReadStateOperationCompleted = new SendOrPostCallback(this.OnImportDocumentiReadStateOperationCompleted);
            this.InvokeAsync("ImportDocumentiReadState", new object[0], this.ImportDocumentiReadStateOperationCompleted, userState);
        }

        private void OnImportDocumentiReadStateOperationCompleted(object arg)
        {
            if (this.ImportDocumentiReadStateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ImportDocumentiReadStateCompleted((object)this, new ImportDocumentiReadStateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ImportAmministrazioniReadState", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool ImportAmministrazioniReadState(out int counter, out int total, out string message)
        {
            object[] objArray = this.Invoke("ImportAmministrazioniReadState", new object[0]);
            counter = (int)objArray[1];
            total = (int)objArray[2];
            message = (string)objArray[3];
            return (bool)objArray[0];
        }

        public IAsyncResult BeginImportAmministrazioniReadState(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ImportAmministrazioniReadState", new object[0], callback, asyncState);
        }

        public bool EndImportAmministrazioniReadState(IAsyncResult asyncResult, out int counter, out int total, out string message)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            counter = (int)objArray[1];
            total = (int)objArray[2];
            message = (string)objArray[3];
            return (bool)objArray[0];
        }

        public void ImportAmministrazioniReadStateAsync()
        {
            this.ImportAmministrazioniReadStateAsync((object)null);
        }

        public void ImportAmministrazioniReadStateAsync(object userState)
        {
            if (this.ImportAmministrazioniReadStateOperationCompleted == null)
                this.ImportAmministrazioniReadStateOperationCompleted = new SendOrPostCallback(this.OnImportAmministrazioniReadStateOperationCompleted);
            this.InvokeAsync("ImportAmministrazioniReadState", new object[0], this.ImportAmministrazioniReadStateOperationCompleted, userState);
        }

        private void OnImportAmministrazioniReadStateOperationCompleted(object arg)
        {
            if (this.ImportAmministrazioniReadStateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ImportAmministrazioniReadStateCompleted((object)this, new ImportAmministrazioniReadStateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ImportCorrispondentiReadState", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool ImportCorrispondentiReadState(out int counter, out int total, out string message)
        {
            object[] objArray = this.Invoke("ImportCorrispondentiReadState", new object[0]);
            counter = (int)objArray[1];
            total = (int)objArray[2];
            message = (string)objArray[3];
            return (bool)objArray[0];
        }

        public IAsyncResult BeginImportCorrispondentiReadState(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ImportCorrispondentiReadState", new object[0], callback, asyncState);
        }

        public bool EndImportCorrispondentiReadState(IAsyncResult asyncResult, out int counter, out int total, out string message)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            counter = (int)objArray[1];
            total = (int)objArray[2];
            message = (string)objArray[3];
            return (bool)objArray[0];
        }

        public void ImportCorrispondentiReadStateAsync()
        {
            this.ImportCorrispondentiReadStateAsync((object)null);
        }

        public void ImportCorrispondentiReadStateAsync(object userState)
        {
            if (this.ImportCorrispondentiReadStateOperationCompleted == null)
                this.ImportCorrispondentiReadStateOperationCompleted = new SendOrPostCallback(this.OnImportCorrispondentiReadStateOperationCompleted);
            this.InvokeAsync("ImportCorrispondentiReadState", new object[0], this.ImportCorrispondentiReadStateOperationCompleted, userState);
        }

        private void OnImportCorrispondentiReadStateOperationCompleted(object arg)
        {
            if (this.ImportCorrispondentiReadStateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ImportCorrispondentiReadStateCompleted((object)this, new ImportCorrispondentiReadStateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/CreateTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool CreateTitolario(string xmlPath)
        {
            return (bool)this.Invoke("CreateTitolario", new object[1]
      {
        (object) xmlPath
      })[0];
        }

        public IAsyncResult BeginCreateTitolario(string xmlPath, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CreateTitolario", new object[1]
      {
        (object) xmlPath
      }, callback, asyncState);
        }

        public bool EndCreateTitolario(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void CreateTitolarioAsync(string xmlPath)
        {
            this.CreateTitolarioAsync(xmlPath, (object)null);
        }

        public void CreateTitolarioAsync(string xmlPath, object userState)
        {
            if (this.CreateTitolarioOperationCompleted == null)
                this.CreateTitolarioOperationCompleted = new SendOrPostCallback(this.OnCreateTitolarioOperationCompleted);
            this.InvokeAsync("CreateTitolario", new object[1]
      {
        (object) xmlPath
      }, this.CreateTitolarioOperationCompleted, userState);
        }

        private void OnCreateTitolarioOperationCompleted(object arg)
        {
            if (this.CreateTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.CreateTitolarioCompleted((object)this, new CreateTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/CreateAmministrazioni", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool CreateAmministrazioni(string xmlPath)
        {
            return (bool)this.Invoke("CreateAmministrazioni", new object[1]
      {
        (object) xmlPath
      })[0];
        }

        public IAsyncResult BeginCreateAmministrazioni(string xmlPath, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CreateAmministrazioni", new object[1]
      {
        (object) xmlPath
      }, callback, asyncState);
        }

        public bool EndCreateAmministrazioni(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void CreateAmministrazioniAsync(string xmlPath)
        {
            this.CreateAmministrazioniAsync(xmlPath, (object)null);
        }

        public void CreateAmministrazioniAsync(string xmlPath, object userState)
        {
            if (this.CreateAmministrazioniOperationCompleted == null)
                this.CreateAmministrazioniOperationCompleted = new SendOrPostCallback(this.OnCreateAmministrazioniOperationCompleted);
            this.InvokeAsync("CreateAmministrazioni", new object[1]
      {
        (object) xmlPath
      }, this.CreateAmministrazioniOperationCompleted, userState);
        }

        private void OnCreateAmministrazioniOperationCompleted(object arg)
        {
            if (this.CreateAmministrazioniCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.CreateAmministrazioniCompleted((object)this, new CreateAmministrazioniCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/UpdateData", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool UpdateData(string xmlStreamAmm, string xmlStreamTitolario)
        {
            return (bool)this.Invoke("UpdateData", new object[2]
      {
        (object) xmlStreamAmm,
        (object) xmlStreamTitolario
      })[0];
        }

        public IAsyncResult BeginUpdateData(string xmlStreamAmm, string xmlStreamTitolario, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("UpdateData", new object[2]
      {
        (object) xmlStreamAmm,
        (object) xmlStreamTitolario
      }, callback, asyncState);
        }

        public bool EndUpdateData(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void UpdateDataAsync(string xmlStreamAmm, string xmlStreamTitolario)
        {
            this.UpdateDataAsync(xmlStreamAmm, xmlStreamTitolario, (object)null);
        }

        public void UpdateDataAsync(string xmlStreamAmm, string xmlStreamTitolario, object userState)
        {
            if (this.UpdateDataOperationCompleted == null)
                this.UpdateDataOperationCompleted = new SendOrPostCallback(this.OnUpdateDataOperationCompleted);
            this.InvokeAsync("UpdateData", new object[2]
      {
        (object) xmlStreamAmm,
        (object) xmlStreamTitolario
      }, this.UpdateDataOperationCompleted, userState);
        }

        private void OnUpdateDataOperationCompleted(object arg)
        {
            if (this.UpdateDataCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.UpdateDataCompleted((object)this, new UpdateDataCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/RefreshAmministrazione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string RefreshAmministrazione()
        {
            return (string)this.Invoke("RefreshAmministrazione", new object[0])[0];
        }

        public IAsyncResult BeginRefreshAmministrazione(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RefreshAmministrazione", new object[0], callback, asyncState);
        }

        public string EndRefreshAmministrazione(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void RefreshAmministrazioneAsync()
        {
            this.RefreshAmministrazioneAsync((object)null);
        }

        public void RefreshAmministrazioneAsync(object userState)
        {
            if (this.RefreshAmministrazioneOperationCompleted == null)
                this.RefreshAmministrazioneOperationCompleted = new SendOrPostCallback(this.OnRefreshAmministrazioneOperationCompleted);
            this.InvokeAsync("RefreshAmministrazione", new object[0], this.RefreshAmministrazioneOperationCompleted, userState);
        }

        private void OnRefreshAmministrazioneOperationCompleted(object arg)
        {
            if (this.RefreshAmministrazioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.RefreshAmministrazioneCompleted((object)this, new RefreshAmministrazioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ImportCorrispondenti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool ImportCorrispondenti(string filePath)
        {
            return (bool)this.Invoke("ImportCorrispondenti", new object[1]
      {
        (object) filePath
      })[0];
        }

        public IAsyncResult BeginImportCorrispondenti(string filePath, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ImportCorrispondenti", new object[1]
      {
        (object) filePath
      }, callback, asyncState);
        }

        public bool EndImportCorrispondenti(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void ImportCorrispondentiAsync(string filePath)
        {
            this.ImportCorrispondentiAsync(filePath, (object)null);
        }

        public void ImportCorrispondentiAsync(string filePath, object userState)
        {
            if (this.ImportCorrispondentiOperationCompleted == null)
                this.ImportCorrispondentiOperationCompleted = new SendOrPostCallback(this.OnImportCorrispondentiOperationCompleted);
            this.InvokeAsync("ImportCorrispondenti", new object[1]
      {
        (object) filePath
      }, this.ImportCorrispondentiOperationCompleted, userState);
        }

        private void OnImportCorrispondentiOperationCompleted(object arg)
        {
            if (this.ImportCorrispondentiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ImportCorrispondentiCompleted((object)this, new ImportCorrispondentiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ImportDocumenti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool ImportDocumenti(string filePath)
        {
            return (bool)this.Invoke("ImportDocumenti", new object[1]
      {
        (object) filePath
      })[0];
        }

        public IAsyncResult BeginImportDocumenti(string filePath, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ImportDocumenti", new object[1]
      {
        (object) filePath
      }, callback, asyncState);
        }

        public bool EndImportDocumenti(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void ImportDocumentiAsync(string filePath)
        {
            this.ImportDocumentiAsync(filePath, (object)null);
        }

        public void ImportDocumentiAsync(string filePath, object userState)
        {
            if (this.ImportDocumentiOperationCompleted == null)
                this.ImportDocumentiOperationCompleted = new SendOrPostCallback(this.OnImportDocumentiOperationCompleted);
            this.InvokeAsync("ImportDocumenti", new object[1]
      {
        (object) filePath
      }, this.ImportDocumentiOperationCompleted, userState);
        }

        private void OnImportDocumentiOperationCompleted(object arg)
        {
            if (this.ImportDocumentiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ImportDocumentiCompleted((object)this, new ImportDocumentiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ImportOggettario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool ImportOggettario(string filePath)
        {
            return (bool)this.Invoke("ImportOggettario", new object[1]
      {
        (object) filePath
      })[0];
        }

        public IAsyncResult BeginImportOggettario(string filePath, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ImportOggettario", new object[1]
      {
        (object) filePath
      }, callback, asyncState);
        }

        public bool EndImportOggettario(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void ImportOggettarioAsync(string filePath)
        {
            this.ImportOggettarioAsync(filePath, (object)null);
        }

        public void ImportOggettarioAsync(string filePath, object userState)
        {
            if (this.ImportOggettarioOperationCompleted == null)
                this.ImportOggettarioOperationCompleted = new SendOrPostCallback(this.OnImportOggettarioOperationCompleted);
            this.InvokeAsync("ImportOggettario", new object[1]
      {
        (object) filePath
      }, this.ImportOggettarioOperationCompleted, userState);
        }

        private void OnImportOggettarioOperationCompleted(object arg)
        {
            if (this.ImportOggettarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ImportOggettarioCompleted((object)this, new ImportOggettarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ImportStoricoOggettario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool ImportStoricoOggettario(string filePath)
        {
            return (bool)this.Invoke("ImportStoricoOggettario", new object[1]
      {
        (object) filePath
      })[0];
        }

        public IAsyncResult BeginImportStoricoOggettario(string filePath, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ImportStoricoOggettario", new object[1]
      {
        (object) filePath
      }, callback, asyncState);
        }

        public bool EndImportStoricoOggettario(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void ImportStoricoOggettarioAsync(string filePath)
        {
            this.ImportStoricoOggettarioAsync(filePath, (object)null);
        }

        public void ImportStoricoOggettarioAsync(string filePath, object userState)
        {
            if (this.ImportStoricoOggettarioOperationCompleted == null)
                this.ImportStoricoOggettarioOperationCompleted = new SendOrPostCallback(this.OnImportStoricoOggettarioOperationCompleted);
            this.InvokeAsync("ImportStoricoOggettario", new object[1]
      {
        (object) filePath
      }, this.ImportStoricoOggettarioOperationCompleted, userState);
        }

        private void OnImportStoricoOggettarioOperationCompleted(object arg)
        {
            if (this.ImportStoricoOggettarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ImportStoricoOggettarioCompleted((object)this, new ImportStoricoOggettarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ImportTipiAtto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool ImportTipiAtto(string filePath)
        {
            return (bool)this.Invoke("ImportTipiAtto", new object[1]
      {
        (object) filePath
      })[0];
        }

        public IAsyncResult BeginImportTipiAtto(string filePath, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ImportTipiAtto", new object[1]
      {
        (object) filePath
      }, callback, asyncState);
        }

        public bool EndImportTipiAtto(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void ImportTipiAttoAsync(string filePath)
        {
            this.ImportTipiAttoAsync(filePath, (object)null);
        }

        public void ImportTipiAttoAsync(string filePath, object userState)
        {
            if (this.ImportTipiAttoOperationCompleted == null)
                this.ImportTipiAttoOperationCompleted = new SendOrPostCallback(this.OnImportTipiAttoOperationCompleted);
            this.InvokeAsync("ImportTipiAtto", new object[1]
      {
        (object) filePath
      }, this.ImportTipiAttoOperationCompleted, userState);
        }

        private void OnImportTipiAttoOperationCompleted(object arg)
        {
            if (this.ImportTipiAttoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ImportTipiAttoCompleted((object)this, new ImportTipiAttoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/ImportTipiDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool ImportTipiDocumento(string filePath)
        {
            return (bool)this.Invoke("ImportTipiDocumento", new object[1]
      {
        (object) filePath
      })[0];
        }

        public IAsyncResult BeginImportTipiDocumento(string filePath, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ImportTipiDocumento", new object[1]
      {
        (object) filePath
      }, callback, asyncState);
        }

        public bool EndImportTipiDocumento(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void ImportTipiDocumentoAsync(string filePath)
        {
            this.ImportTipiDocumentoAsync(filePath, (object)null);
        }

        public void ImportTipiDocumentoAsync(string filePath, object userState)
        {
            if (this.ImportTipiDocumentoOperationCompleted == null)
                this.ImportTipiDocumentoOperationCompleted = new SendOrPostCallback(this.OnImportTipiDocumentoOperationCompleted);
            this.InvokeAsync("ImportTipiDocumento", new object[1]
      {
        (object) filePath
      }, this.ImportTipiDocumentoOperationCompleted, userState);
        }

        private void OnImportTipiDocumentoOperationCompleted(object arg)
        {
            if (this.ImportTipiDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.ImportTipiDocumentoCompleted((object)this, new ImportTipiDocumentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/LoginAmministratore", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string LoginAmministratore(string utente, string password)
        {
            return (string)this.Invoke("LoginAmministratore", new object[2]
      {
        (object) utente,
        (object) password
      })[0];
        }

        public IAsyncResult BeginLoginAmministratore(string utente, string password, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("LoginAmministratore", new object[2]
      {
        (object) utente,
        (object) password
      }, callback, asyncState);
        }

        public string EndLoginAmministratore(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void LoginAmministratoreAsync(string utente, string password)
        {
            this.LoginAmministratoreAsync(utente, password, (object)null);
        }

        public void LoginAmministratoreAsync(string utente, string password, object userState)
        {
            if (this.LoginAmministratoreOperationCompleted == null)
                this.LoginAmministratoreOperationCompleted = new SendOrPostCallback(this.OnLoginAmministratoreOperationCompleted);
            this.InvokeAsync("LoginAmministratore", new object[2]
      {
        (object) utente,
        (object) password
      }, this.LoginAmministratoreOperationCompleted, userState);
        }

        private void OnLoginAmministratoreOperationCompleted(object arg)
        {
            if (this.LoginAmministratoreCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.LoginAmministratoreCompleted((object)this, new LoginAmministratoreCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/LogoutAmministratore", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool LogoutAmministratore()
        {
            return (bool)this.Invoke("LogoutAmministratore", new object[0])[0];
        }

        public IAsyncResult BeginLogoutAmministratore(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("LogoutAmministratore", new object[0], callback, asyncState);
        }

        public bool EndLogoutAmministratore(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void LogoutAmministratoreAsync()
        {
            this.LogoutAmministratoreAsync((object)null);
        }

        public void LogoutAmministratoreAsync(object userState)
        {
            if (this.LogoutAmministratoreOperationCompleted == null)
                this.LogoutAmministratoreOperationCompleted = new SendOrPostCallback(this.OnLogoutAmministratoreOperationCompleted);
            this.InvokeAsync("LogoutAmministratore", new object[0], this.LogoutAmministratoreOperationCompleted, userState);
        }

        private void OnLogoutAmministratoreOperationCompleted(object arg)
        {
            if (this.LogoutAmministratoreCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.LogoutAmministratoreCompleted((object)this, new LogoutAmministratoreCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/CheckAdministrator", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool CheckAdministrator(string userId)
        {
            return (bool)this.Invoke("CheckAdministrator", new object[1]
      {
        (object) userId
      })[0];
        }

        public IAsyncResult BeginCheckAdministrator(string userId, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CheckAdministrator", new object[1]
      {
        (object) userId
      }, callback, asyncState);
        }

        public bool EndCheckAdministrator(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void CheckAdministratorAsync(string userId)
        {
            this.CheckAdministratorAsync(userId, (object)null);
        }

        public void CheckAdministratorAsync(string userId, object userState)
        {
            if (this.CheckAdministratorOperationCompleted == null)
                this.CheckAdministratorOperationCompleted = new SendOrPostCallback(this.OnCheckAdministratorOperationCompleted);
            this.InvokeAsync("CheckAdministrator", new object[1]
      {
        (object) userId
      }, this.CheckAdministratorOperationCompleted, userState);
        }

        private void OnCheckAdministratorOperationCompleted(object arg)
        {
            if (this.CheckAdministratorCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.CheckAdministratorCompleted((object)this, new CheckAdministratorCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/CambiaPwdAmministratore", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool CambiaPwdAmministratore(string utente, string password)
        {
            return (bool)this.Invoke("CambiaPwdAmministratore", new object[2]
      {
        (object) utente,
        (object) password
      })[0];
        }

        public IAsyncResult BeginCambiaPwdAmministratore(string utente, string password, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CambiaPwdAmministratore", new object[2]
      {
        (object) utente,
        (object) password
      }, callback, asyncState);
        }

        public bool EndCambiaPwdAmministratore(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void CambiaPwdAmministratoreAsync(string utente, string password)
        {
            this.CambiaPwdAmministratoreAsync(utente, password, (object)null);
        }

        public void CambiaPwdAmministratoreAsync(string utente, string password, object userState)
        {
            if (this.CambiaPwdAmministratoreOperationCompleted == null)
                this.CambiaPwdAmministratoreOperationCompleted = new SendOrPostCallback(this.OnCambiaPwdAmministratoreOperationCompleted);
            this.InvokeAsync("CambiaPwdAmministratore", new object[2]
      {
        (object) utente,
        (object) password
      }, this.CambiaPwdAmministratoreOperationCompleted, userState);
        }

        private void OnCambiaPwdAmministratoreOperationCompleted(object arg)
        {
            if (this.CambiaPwdAmministratoreCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.CambiaPwdAmministratoreCompleted((object)this, new CambiaPwdAmministratoreCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/NodoTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string NodoTitolario(string codAmm, string idParent)
        {
            return (string)this.Invoke("NodoTitolario", new object[2]
      {
        (object) codAmm,
        (object) idParent
      })[0];
        }

        public IAsyncResult BeginNodoTitolario(string codAmm, string idParent, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("NodoTitolario", new object[2]
      {
        (object) codAmm,
        (object) idParent
      }, callback, asyncState);
        }

        public string EndNodoTitolario(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void NodoTitolarioAsync(string codAmm, string idParent)
        {
            this.NodoTitolarioAsync(codAmm, idParent, (object)null);
        }

        public void NodoTitolarioAsync(string codAmm, string idParent, object userState)
        {
            if (this.NodoTitolarioOperationCompleted == null)
                this.NodoTitolarioOperationCompleted = new SendOrPostCallback(this.OnNodoTitolarioOperationCompleted);
            this.InvokeAsync("NodoTitolario", new object[2]
      {
        (object) codAmm,
        (object) idParent
      }, this.NodoTitolarioOperationCompleted, userState);
        }

        private void OnNodoTitolarioOperationCompleted(object arg)
        {
            if (this.NodoTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.NodoTitolarioCompleted((object)this, new NodoTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/NodoTitolarioSecurity", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string NodoTitolarioSecurity(string idAmm, string idParent, string idGruppo, string idRegistro)
        {
            return (string)this.Invoke("NodoTitolarioSecurity", new object[4]
      {
        (object) idAmm,
        (object) idParent,
        (object) idGruppo,
        (object) idRegistro
      })[0];
        }

        public IAsyncResult BeginNodoTitolarioSecurity(string idAmm, string idParent, string idGruppo, string idRegistro, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("NodoTitolarioSecurity", new object[4]
      {
        (object) idAmm,
        (object) idParent,
        (object) idGruppo,
        (object) idRegistro
      }, callback, asyncState);
        }

        public string EndNodoTitolarioSecurity(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void NodoTitolarioSecurityAsync(string idAmm, string idParent, string idGruppo, string idRegistro)
        {
            this.NodoTitolarioSecurityAsync(idAmm, idParent, idGruppo, idRegistro, (object)null);
        }

        public void NodoTitolarioSecurityAsync(string idAmm, string idParent, string idGruppo, string idRegistro, object userState)
        {
            if (this.NodoTitolarioSecurityOperationCompleted == null)
                this.NodoTitolarioSecurityOperationCompleted = new SendOrPostCallback(this.OnNodoTitolarioSecurityOperationCompleted);
            this.InvokeAsync("NodoTitolarioSecurity", new object[4]
      {
        (object) idAmm,
        (object) idParent,
        (object) idGruppo,
        (object) idRegistro
      }, this.NodoTitolarioSecurityOperationCompleted, userState);
        }

        private void OnNodoTitolarioSecurityOperationCompleted(object arg)
        {
            if (this.NodoTitolarioSecurityCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.NodoTitolarioSecurityCompleted((object)this, new NodoTitolarioSecurityCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/RegistriInAmm", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string RegistriInAmm(string codAmm)
        {
            return (string)this.Invoke("RegistriInAmm", new object[1]
      {
        (object) codAmm
      })[0];
        }

        public IAsyncResult BeginRegistriInAmm(string codAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RegistriInAmm", new object[1]
      {
        (object) codAmm
      }, callback, asyncState);
        }

        public string EndRegistriInAmm(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void RegistriInAmmAsync(string codAmm)
        {
            this.RegistriInAmmAsync(codAmm, (object)null);
        }

        public void RegistriInAmmAsync(string codAmm, object userState)
        {
            if (this.RegistriInAmmOperationCompleted == null)
                this.RegistriInAmmOperationCompleted = new SendOrPostCallback(this.OnRegistriInAmmOperationCompleted);
            this.InvokeAsync("RegistriInAmm", new object[1]
      {
        (object) codAmm
      }, this.RegistriInAmmOperationCompleted, userState);
        }

        private void OnRegistriInAmmOperationCompleted(object arg)
        {
            if (this.RegistriInAmmCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.RegistriInAmmCompleted((object)this, new RegistriInAmmCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/SecurityNodoRuoli", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string SecurityNodoRuoli(string idNodo, string codAmm)
        {
            return (string)this.Invoke("SecurityNodoRuoli", new object[2]
      {
        (object) idNodo,
        (object) codAmm
      })[0];
        }

        public IAsyncResult BeginSecurityNodoRuoli(string idNodo, string codAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SecurityNodoRuoli", new object[2]
      {
        (object) idNodo,
        (object) codAmm
      }, callback, asyncState);
        }

        public string EndSecurityNodoRuoli(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void SecurityNodoRuoliAsync(string idNodo, string codAmm)
        {
            this.SecurityNodoRuoliAsync(idNodo, codAmm, (object)null);
        }

        public void SecurityNodoRuoliAsync(string idNodo, string codAmm, object userState)
        {
            if (this.SecurityNodoRuoliOperationCompleted == null)
                this.SecurityNodoRuoliOperationCompleted = new SendOrPostCallback(this.OnSecurityNodoRuoliOperationCompleted);
            this.InvokeAsync("SecurityNodoRuoli", new object[2]
      {
        (object) idNodo,
        (object) codAmm
      }, this.SecurityNodoRuoliOperationCompleted, userState);
        }

        private void OnSecurityNodoRuoliOperationCompleted(object arg)
        {
            if (this.SecurityNodoRuoliCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.SecurityNodoRuoliCompleted((object)this, new SecurityNodoRuoliCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/UpdNodo_Titolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string UpdNodo_Titolario(string idrecord, string codice, string descrizione, string idregistro, string streamRuoli, string codAmm, string cosaModificare)
        {
            return (string)this.Invoke("UpdNodo_Titolario", new object[7]
      {
        (object) idrecord,
        (object) codice,
        (object) descrizione,
        (object) idregistro,
        (object) streamRuoli,
        (object) codAmm,
        (object) cosaModificare
      })[0];
        }

        public IAsyncResult BeginUpdNodo_Titolario(string idrecord, string codice, string descrizione, string idregistro, string streamRuoli, string codAmm, string cosaModificare, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("UpdNodo_Titolario", new object[7]
      {
        (object) idrecord,
        (object) codice,
        (object) descrizione,
        (object) idregistro,
        (object) streamRuoli,
        (object) codAmm,
        (object) cosaModificare
      }, callback, asyncState);
        }

        public string EndUpdNodo_Titolario(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void UpdNodo_TitolarioAsync(string idrecord, string codice, string descrizione, string idregistro, string streamRuoli, string codAmm, string cosaModificare)
        {
            this.UpdNodo_TitolarioAsync(idrecord, codice, descrizione, idregistro, streamRuoli, codAmm, cosaModificare, (object)null);
        }

        public void UpdNodo_TitolarioAsync(string idrecord, string codice, string descrizione, string idregistro, string streamRuoli, string codAmm, string cosaModificare, object userState)
        {
            if (this.UpdNodo_TitolarioOperationCompleted == null)
                this.UpdNodo_TitolarioOperationCompleted = new SendOrPostCallback(this.OnUpdNodo_TitolarioOperationCompleted);
            this.InvokeAsync("UpdNodo_Titolario", new object[7]
      {
        (object) idrecord,
        (object) codice,
        (object) descrizione,
        (object) idregistro,
        (object) streamRuoli,
        (object) codAmm,
        (object) cosaModificare
      }, this.UpdNodo_TitolarioOperationCompleted, userState);
        }

        private void OnUpdNodo_TitolarioOperationCompleted(object arg)
        {
            if (this.UpdNodo_TitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.UpdNodo_TitolarioCompleted((object)this, new UpdNodo_TitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/filtroRicercaTit", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string filtroRicercaTit(string codice, string descrizione, string idAmm)
        {
            return (string)this.Invoke("filtroRicercaTit", new object[3]
      {
        (object) codice,
        (object) descrizione,
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginfiltroRicercaTit(string codice, string descrizione, string idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("filtroRicercaTit", new object[3]
      {
        (object) codice,
        (object) descrizione,
        (object) idAmm
      }, callback, asyncState);
        }

        public string EndfiltroRicercaTit(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void filtroRicercaTitAsync(string codice, string descrizione, string idAmm)
        {
            this.filtroRicercaTitAsync(codice, descrizione, idAmm, (object)null);
        }

        public void filtroRicercaTitAsync(string codice, string descrizione, string idAmm, object userState)
        {
            if (this.filtroRicercaTitOperationCompleted == null)
                this.filtroRicercaTitOperationCompleted = new SendOrPostCallback(this.OnfiltroRicercaTitOperationCompleted);
            this.InvokeAsync("filtroRicercaTit", new object[3]
      {
        (object) codice,
        (object) descrizione,
        (object) idAmm
      }, this.filtroRicercaTitOperationCompleted, userState);
        }

        private void OnfiltroRicercaTitOperationCompleted(object arg)
        {
            if (this.filtroRicercaTitCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.filtroRicercaTitCompleted((object)this, new filtroRicercaTitCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/filtroRicercaTitDocspa", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string filtroRicercaTitDocspa(string codice, string descrizione, string idAmm, string idGruppo, string idRegistro)
        {
            return (string)this.Invoke("filtroRicercaTitDocspa", new object[5]
      {
        (object) codice,
        (object) descrizione,
        (object) idAmm,
        (object) idGruppo,
        (object) idRegistro
      })[0];
        }

        public IAsyncResult BeginfiltroRicercaTitDocspa(string codice, string descrizione, string idAmm, string idGruppo, string idRegistro, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("filtroRicercaTitDocspa", new object[5]
      {
        (object) codice,
        (object) descrizione,
        (object) idAmm,
        (object) idGruppo,
        (object) idRegistro
      }, callback, asyncState);
        }

        public string EndfiltroRicercaTitDocspa(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void filtroRicercaTitDocspaAsync(string codice, string descrizione, string idAmm, string idGruppo, string idRegistro)
        {
            this.filtroRicercaTitDocspaAsync(codice, descrizione, idAmm, idGruppo, idRegistro, (object)null);
        }

        public void filtroRicercaTitDocspaAsync(string codice, string descrizione, string idAmm, string idGruppo, string idRegistro, object userState)
        {
            if (this.filtroRicercaTitDocspaOperationCompleted == null)
                this.filtroRicercaTitDocspaOperationCompleted = new SendOrPostCallback(this.OnfiltroRicercaTitDocspaOperationCompleted);
            this.InvokeAsync("filtroRicercaTitDocspa", new object[5]
      {
        (object) codice,
        (object) descrizione,
        (object) idAmm,
        (object) idGruppo,
        (object) idRegistro
      }, this.filtroRicercaTitDocspaOperationCompleted, userState);
        }

        private void OnfiltroRicercaTitDocspaOperationCompleted(object arg)
        {
            if (this.filtroRicercaTitDocspaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.filtroRicercaTitDocspaCompleted((object)this, new filtroRicercaTitDocspaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/filtroRicercaTitAmm", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string filtroRicercaTitAmm(string codice, string descrizione, string codAmm)
        {
            return (string)this.Invoke("filtroRicercaTitAmm", new object[3]
      {
        (object) codice,
        (object) descrizione,
        (object) codAmm
      })[0];
        }

        public IAsyncResult BeginfiltroRicercaTitAmm(string codice, string descrizione, string codAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("filtroRicercaTitAmm", new object[3]
      {
        (object) codice,
        (object) descrizione,
        (object) codAmm
      }, callback, asyncState);
        }

        public string EndfiltroRicercaTitAmm(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void filtroRicercaTitAmmAsync(string codice, string descrizione, string codAmm)
        {
            this.filtroRicercaTitAmmAsync(codice, descrizione, codAmm, (object)null);
        }

        public void filtroRicercaTitAmmAsync(string codice, string descrizione, string codAmm, object userState)
        {
            if (this.filtroRicercaTitAmmOperationCompleted == null)
                this.filtroRicercaTitAmmOperationCompleted = new SendOrPostCallback(this.OnfiltroRicercaTitAmmOperationCompleted);
            this.InvokeAsync("filtroRicercaTitAmm", new object[3]
      {
        (object) codice,
        (object) descrizione,
        (object) codAmm
      }, this.filtroRicercaTitAmmOperationCompleted, userState);
        }

        private void OnfiltroRicercaTitAmmOperationCompleted(object arg)
        {
            if (this.filtroRicercaTitAmmCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.filtroRicercaTitAmmCompleted((object)this, new filtroRicercaTitAmmCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/findNodoRoot", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string findNodoRoot(string idrecord, string idparent, int livello)
        {
            return (string)this.Invoke("findNodoRoot", new object[3]
      {
        (object) idrecord,
        (object) idparent,
        (object) livello
      })[0];
        }

        public IAsyncResult BeginfindNodoRoot(string idrecord, string idparent, int livello, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("findNodoRoot", new object[3]
      {
        (object) idrecord,
        (object) idparent,
        (object) livello
      }, callback, asyncState);
        }

        public string EndfindNodoRoot(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void findNodoRootAsync(string idrecord, string idparent, int livello)
        {
            this.findNodoRootAsync(idrecord, idparent, livello, (object)null);
        }

        public void findNodoRootAsync(string idrecord, string idparent, int livello, object userState)
        {
            if (this.findNodoRootOperationCompleted == null)
                this.findNodoRootOperationCompleted = new SendOrPostCallback(this.OnfindNodoRootOperationCompleted);
            this.InvokeAsync("findNodoRoot", new object[3]
      {
        (object) idrecord,
        (object) idparent,
        (object) livello
      }, this.findNodoRootOperationCompleted, userState);
        }

        private void OnfindNodoRootOperationCompleted(object arg)
        {
            if (this.findNodoRootCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.findNodoRootCompleted((object)this, new findNodoRootCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/findNodoByCod", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string findNodoByCod(string codClass, string idAmm)
        {
            return (string)this.Invoke("findNodoByCod", new object[2]
      {
        (object) codClass,
        (object) idAmm
      })[0];
        }

        public IAsyncResult BeginfindNodoByCod(string codClass, string idAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("findNodoByCod", new object[2]
      {
        (object) codClass,
        (object) idAmm
      }, callback, asyncState);
        }

        public string EndfindNodoByCod(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void findNodoByCodAsync(string codClass, string idAmm)
        {
            this.findNodoByCodAsync(codClass, idAmm, (object)null);
        }

        public void findNodoByCodAsync(string codClass, string idAmm, object userState)
        {
            if (this.findNodoByCodOperationCompleted == null)
                this.findNodoByCodOperationCompleted = new SendOrPostCallback(this.OnfindNodoByCodOperationCompleted);
            this.InvokeAsync("findNodoByCod", new object[2]
      {
        (object) codClass,
        (object) idAmm
      }, this.findNodoByCodOperationCompleted, userState);
        }

        private void OnfindNodoByCodOperationCompleted(object arg)
        {
            if (this.findNodoByCodCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.findNodoByCodCompleted((object)this, new findNodoByCodCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/AggNewNodo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string AggNewNodo(string padre, string codice, string descrizione, string idregistro, string livello, string codAmm, string streamRuoli, string codliv, string r_w)
        {
            return (string)this.Invoke("AggNewNodo", new object[9]
      {
        (object) padre,
        (object) codice,
        (object) descrizione,
        (object) idregistro,
        (object) livello,
        (object) codAmm,
        (object) streamRuoli,
        (object) codliv,
        (object) r_w
      })[0];
        }

        public IAsyncResult BeginAggNewNodo(string padre, string codice, string descrizione, string idregistro, string livello, string codAmm, string streamRuoli, string codliv, string r_w, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AggNewNodo", new object[9]
      {
        (object) padre,
        (object) codice,
        (object) descrizione,
        (object) idregistro,
        (object) livello,
        (object) codAmm,
        (object) streamRuoli,
        (object) codliv,
        (object) r_w
      }, callback, asyncState);
        }

        public string EndAggNewNodo(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void AggNewNodoAsync(string padre, string codice, string descrizione, string idregistro, string livello, string codAmm, string streamRuoli, string codliv, string r_w)
        {
            this.AggNewNodoAsync(padre, codice, descrizione, idregistro, livello, codAmm, streamRuoli, codliv, r_w, (object)null);
        }

        public void AggNewNodoAsync(string padre, string codice, string descrizione, string idregistro, string livello, string codAmm, string streamRuoli, string codliv, string r_w, object userState)
        {
            if (this.AggNewNodoOperationCompleted == null)
                this.AggNewNodoOperationCompleted = new SendOrPostCallback(this.OnAggNewNodoOperationCompleted);
            this.InvokeAsync("AggNewNodo", new object[9]
      {
        (object) padre,
        (object) codice,
        (object) descrizione,
        (object) idregistro,
        (object) livello,
        (object) codAmm,
        (object) streamRuoli,
        (object) codliv,
        (object) r_w
      }, this.AggNewNodoOperationCompleted, userState);
        }

        private void OnAggNewNodoOperationCompleted(object arg)
        {
            if (this.AggNewNodoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.AggNewNodoCompleted((object)this, new AggNewNodoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/EliminaNodo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string EliminaNodo(string idrecord)
        {
            return (string)this.Invoke("EliminaNodo", new object[1]
      {
        (object) idrecord
      })[0];
        }

        public IAsyncResult BeginEliminaNodo(string idrecord, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("EliminaNodo", new object[1]
      {
        (object) idrecord
      }, callback, asyncState);
        }

        public string EndEliminaNodo(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void EliminaNodoAsync(string idrecord)
        {
            this.EliminaNodoAsync(idrecord, (object)null);
        }

        public void EliminaNodoAsync(string idrecord, object userState)
        {
            if (this.EliminaNodoOperationCompleted == null)
                this.EliminaNodoOperationCompleted = new SendOrPostCallback(this.OnEliminaNodoOperationCompleted);
            this.InvokeAsync("EliminaNodo", new object[1]
      {
        (object) idrecord
      }, this.EliminaNodoOperationCompleted, userState);
        }

        private void OnEliminaNodoOperationCompleted(object arg)
        {
            if (this.EliminaNodoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.EliminaNodoCompleted((object)this, new EliminaNodoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/GetCodLiv", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string GetCodLiv(string codliv, string livello, string codAmm)
        {
            return (string)this.Invoke("GetCodLiv", new object[3]
      {
        (object) codliv,
        (object) livello,
        (object) codAmm
      })[0];
        }

        public IAsyncResult BeginGetCodLiv(string codliv, string livello, string codAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetCodLiv", new object[3]
      {
        (object) codliv,
        (object) livello,
        (object) codAmm
      }, callback, asyncState);
        }

        public string EndGetCodLiv(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void GetCodLivAsync(string codliv, string livello, string codAmm)
        {
            this.GetCodLivAsync(codliv, livello, codAmm, (object)null);
        }

        public void GetCodLivAsync(string codliv, string livello, string codAmm, object userState)
        {
            if (this.GetCodLivOperationCompleted == null)
                this.GetCodLivOperationCompleted = new SendOrPostCallback(this.OnGetCodLivOperationCompleted);
            this.InvokeAsync("GetCodLiv", new object[3]
      {
        (object) codliv,
        (object) livello,
        (object) codAmm
      }, this.GetCodLivOperationCompleted, userState);
        }

        private void OnGetCodLivOperationCompleted(object arg)
        {
            if (this.GetCodLivCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.GetCodLivCompleted((object)this, new GetCodLivCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/SpostaNodoTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string SpostaNodoTitolario(string currentCodLiv, string newCodLiv, string codAmm)
        {
            return (string)this.Invoke("SpostaNodoTitolario", new object[3]
      {
        (object) currentCodLiv,
        (object) newCodLiv,
        (object) codAmm
      })[0];
        }

        public IAsyncResult BeginSpostaNodoTitolario(string currentCodLiv, string newCodLiv, string codAmm, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SpostaNodoTitolario", new object[3]
      {
        (object) currentCodLiv,
        (object) newCodLiv,
        (object) codAmm
      }, callback, asyncState);
        }

        public string EndSpostaNodoTitolario(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void SpostaNodoTitolarioAsync(string currentCodLiv, string newCodLiv, string codAmm)
        {
            this.SpostaNodoTitolarioAsync(currentCodLiv, newCodLiv, codAmm, (object)null);
        }

        public void SpostaNodoTitolarioAsync(string currentCodLiv, string newCodLiv, string codAmm, object userState)
        {
            if (this.SpostaNodoTitolarioOperationCompleted == null)
                this.SpostaNodoTitolarioOperationCompleted = new SendOrPostCallback(this.OnSpostaNodoTitolarioOperationCompleted);
            this.InvokeAsync("SpostaNodoTitolario", new object[3]
      {
        (object) currentCodLiv,
        (object) newCodLiv,
        (object) codAmm
      }, this.SpostaNodoTitolarioOperationCompleted, userState);
        }

        private void OnSpostaNodoTitolarioOperationCompleted(object arg)
        {
            if (this.SpostaNodoTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.SpostaNodoTitolarioCompleted((object)this, new SpostaNodoTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (url == null || url == string.Empty)
                return false;
            Uri uri = new Uri(url);
            return uri.Port >= 1024 && string.Compare(uri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
