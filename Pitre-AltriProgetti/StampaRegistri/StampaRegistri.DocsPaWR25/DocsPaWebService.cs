// Decompiled with JetBrains decompiler
// Type: StampaRegistri.DocsPaWR25.DocsPaWebService
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

namespace StampaRegistri.DocsPaWR25
{
    [XmlInclude(typeof(Storico))]
    [GeneratedCode("System.Web.Services", "2.0.50727.42")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [WebServiceBinding(Name = "DocsPaWebServiceSoap", Namespace = "http://localhost")]
    [XmlInclude(typeof(MarshalByRefObject))]
    public class DocsPaWebService : SoapHttpClientProtocol
    {
        private SendOrPostCallback documentoImportaProtocolloEmergenzaOperationCompleted;
        private SendOrPostCallback addressbookGetListaCorrispondentiSempliceOperationCompleted;
        private SendOrPostCallback licenzaOperationCompleted;
        private SendOrPostCallback pauseOperationCompleted;
        private SendOrPostCallback importDelibereOperationCompleted;
        private SendOrPostCallback reportTitolarioOperationCompleted;
        private SendOrPostCallback reportBustaOperationCompleted;
        private SendOrPostCallback reportSchedaDocOperationCompleted;
        private SendOrPostCallback faxInvioOperationCompleted;
        private SendOrPostCallback faxProcessaCasellaOperationCompleted;
        private SendOrPostCallback amministrazioneSetPasswordOperationCompleted;
        private SendOrPostCallback amministrazioneSetDatiServerPostaOperationCompleted;
        private SendOrPostCallback amministrazioneGetListaServerPostaOperationCompleted;
        private SendOrPostCallback amministrazioneGetDatiUtenteOperationCompleted;
        private SendOrPostCallback amministrazioneGetDatiUOOperationCompleted;
        private SendOrPostCallback amministrazioneGetUtentiOperationCompleted;
        private SendOrPostCallback amministrazioneGetDatiCorrispondenteOperationCompleted;
        private SendOrPostCallback amministrazioneGetRegistriInUOOperationCompleted;
        private SendOrPostCallback amministrazioneSetFunzioniInRuoloOperationCompleted;
        private SendOrPostCallback amministrazioneGetFunzioniRuoloOperationCompleted;
        private SendOrPostCallback amministrazioneSetRuoliUtenteOperationCompleted;
        private SendOrPostCallback amministrazioneGetRuoliInUOOperationCompleted;
        private SendOrPostCallback amministrazioneGetRuoliUtenteOperationCompleted;
        private SendOrPostCallback amministrazioneUpdateCorrispondenteOperationCompleted;
        private SendOrPostCallback amministrazioneGetDominioOperationCompleted;
        private SendOrPostCallback amministrazioneGetDettagliCorrOperationCompleted;
        private SendOrPostCallback amministrazioneSetUtentiInRuoloOperationCompleted;
        private SendOrPostCallback amministrazioneGetUtentiInRuoloOperationCompleted;
        private SendOrPostCallback MDDSelectDocumentoInMDDOperationCompleted;
        private SendOrPostCallback MDDUpdateDocumentoInMDDOperationCompleted;
        private SendOrPostCallback MDDInsertDocumentoInMDDOperationCompleted;
        private SendOrPostCallback MDDGetCampiTipoAttoOperationCompleted;
        private SendOrPostCallback amministrazioneUpdateTipologiaAttoOperationCompleted;
        private SendOrPostCallback amministrazioneInsertTipologiaAttoOperationCompleted;
        private SendOrPostCallback amministrazioneInsertCorrispondenteOperationCompleted;
        private SendOrPostCallback amministrazioneGetTipiFunzioneOperationCompleted;
        private SendOrPostCallback amministrazioneGetCanaliOperationCompleted;
        private SendOrPostCallback amministrazioneGetTipoRuoliOperationCompleted;
        private SendOrPostCallback amministrazioneGetRegistriOperationCompleted;
        private SendOrPostCallback amministrazioneGetAmministrazioniOperationCompleted;
        private SendOrPostCallback amministrazioneGetParametroConfigurazioneOperationCompleted;
        private SendOrPostCallback registriStampaOperationCompleted;
        private SendOrPostCallback registriModificaOperationCompleted;
        private SendOrPostCallback registriCambiaStatoOperationCompleted;
        private SendOrPostCallback interoperabilitaAggiornamentoConfermaOperationCompleted;
        private SendOrPostCallback interoperabilitaInvioRicevutaOperationCompleted;
        private SendOrPostCallback interoperabilitaInvioOperationCompleted;
        private SendOrPostCallback interoperabilitaRicezioneOperationCompleted;
        private SendOrPostCallback trasmissioneGetInRispostaAOperationCompleted;
        private SendOrPostCallback trasmissioneGetRispostaOperationCompleted;
        private SendOrPostCallback trasmissioneSetTxUtAsVisteOperationCompleted;
        private SendOrPostCallback trasmissioneExecuteAccRifOperationCompleted;
        private SendOrPostCallback trasmissioneGetRagioniOperationCompleted;
        private SendOrPostCallback trasmissioneGetQueryEffettuateOperationCompleted;
        private SendOrPostCallback trasmissioneGetQueryRicevuteOperationCompleted;
        private SendOrPostCallback trasmissioneExecuteTrasmOperationCompleted;
        private SendOrPostCallback trasmissioneSaveTrasmOperationCompleted;
        private SendOrPostCallback trasmissioneGetQueryEffettuatePagingOperationCompleted;
        private SendOrPostCallback trasmissioneGetQueryRicevutePagingOperationCompleted;
        private SendOrPostCallback fascicolazioneAddDocFolderOperationCompleted;
        private SendOrPostCallback fascicolazioneDelFolderOperationCompleted;
        private SendOrPostCallback fascicolazioneModifyFolderOperationCompleted;
        private SendOrPostCallback fascicolazioneMoveFolderOperationCompleted;
        private SendOrPostCallback fascicolazioneNewFolderOperationCompleted;
        private SendOrPostCallback fascicolazioneDeleteDocumentoOperationCompleted;
        private SendOrPostCallback fascicolazioneGetDocumentiOperationCompleted;
        private SendOrPostCallback fascicolazioneGetFolderOperationCompleted;
        private SendOrPostCallback fascicolazioneGetVisibilitaOperationCompleted;
        private SendOrPostCallback fascicolazioneSospendiRiattivaUtenteOperationCompleted;
        private SendOrPostCallback fascicolazioneGetFascicoloDaCodiceOperationCompleted;
        private SendOrPostCallback fascicolazioneGetFascicoliDaDocOperationCompleted;
        private SendOrPostCallback fascicolazionesetDataVistaFascOperationCompleted;
        private SendOrPostCallback fascicolazioneGetDettaglioFascicoloOperationCompleted;
        private SendOrPostCallback fascicolazioneAddDocFascicoloOperationCompleted;
        private SendOrPostCallback fascicolazioneSetFascicoloOperationCompleted;
        private SendOrPostCallback fascicolazioneNewFascicoloOperationCompleted;
        private SendOrPostCallback fascicolazioneGetDocumentiPagingOperationCompleted;
        private SendOrPostCallback fascicolazioneGetListaFascicoliPagingOperationCompleted;
        private SendOrPostCallback fascicolazioneGetListaFascicoliOperationCompleted;
        private SendOrPostCallback fascicolazioneGetFigliClassificaOperationCompleted;
        private SendOrPostCallback fascicolazioneGetGerarchiaDaCodiceOperationCompleted;
        private SendOrPostCallback fascicolazioneGetGerarchiaOperationCompleted;
        private SendOrPostCallback fascicolazioneGetTitolarioOperationCompleted;
        private SendOrPostCallback documentoGetQueryDocumentoPagingOperationCompleted;
        private SendOrPostCallback documentoSpedisciOperationCompleted;
        private SendOrPostCallback documentoSetFlagDaInviareOperationCompleted;
        private SendOrPostCallback documentoGetVisibilitaOperationCompleted;
        private SendOrPostCallback documentoGetTipologiaCanaleOperationCompleted;
        private SendOrPostCallback documentoGetListaStoriciOggettoOperationCompleted;
        private SendOrPostCallback documentoGetCatenaDocOperationCompleted;
        private SendOrPostCallback documentoExecRimuoviSchedaOperationCompleted;
        private SendOrPostCallback documentoExecAnnullaProtOperationCompleted;
        private SendOrPostCallback documentoModificaVersioneOperationCompleted;
        private SendOrPostCallback documentoScambiaVersioniOperationCompleted;
        private SendOrPostCallback documentoAggiungiVersioneOperationCompleted;
        private SendOrPostCallback documentoRimuoviVersioneOperationCompleted;
        private SendOrPostCallback documentoModificaAllegatoOperationCompleted;
        private SendOrPostCallback documentoAggiungiAllegatoOperationCompleted;
        private SendOrPostCallback documentoGetAllegatiOperationCompleted;
        private SendOrPostCallback documentoCercaDuplicatiOperationCompleted;
        private SendOrPostCallback documentoVerificaFirmaOperationCompleted;
        private SendOrPostCallback documentoSaveDocumentoOperationCompleted;
        private SendOrPostCallback documentoProtocollaOperationCompleted;
        private SendOrPostCallback documentoGetDettaglioDocumentoOperationCompleted;
        private SendOrPostCallback MDDGetCodStatoOperationCompleted;
        private SendOrPostCallback documentoGetCategoriaAttoOperationCompleted;
        private SendOrPostCallback documentoGetTipologiaAttoOperationCompleted;
        private SendOrPostCallback documentoAddParolaChiaveOperationCompleted;
        private SendOrPostCallback documentoGetParoleChiaveOperationCompleted;
        private SendOrPostCallback documentoAddOggettoOperationCompleted;
        private SendOrPostCallback documentoGetListaOggettiOperationCompleted;
        private SendOrPostCallback documentoPutFileOperationCompleted;
        private SendOrPostCallback documentoGetFileOperationCompleted;
        private SendOrPostCallback documentoAddDocGrigiaOperationCompleted;
        private SendOrPostCallback documentoGetAreaLavoroPagingOperationCompleted;
        private SendOrPostCallback documentoGetSegnaturaCampiVariabiliOperationCompleted;
        private SendOrPostCallback documentoGetQueryDocumentoOperationCompleted;
        private SendOrPostCallback addressbookGetCanaliOperationCompleted;
        private SendOrPostCallback addressbookGetRuoliSuperioriInUOOperationCompleted;
        private SendOrPostCallback addressbookInsertCorrispondenteOperationCompleted;
        private SendOrPostCallback addressbookGetRootUOOperationCompleted;
        private SendOrPostCallback addressbookGetListaCorrispondentiAutorizzatiOperationCompleted;
        private SendOrPostCallback addressbookGetDettagliCorrispondenteOperationCompleted;
        private SendOrPostCallback addressbookGetListaCorrispondentiOperationCompleted;
        private SendOrPostCallback documentoCancellaAreaLavoroOperationCompleted;
        private SendOrPostCallback documentoGetAreaLavoroOperationCompleted;
        private SendOrPostCallback documentoGetApplicazioniOperationCompleted;
        private SendOrPostCallback documentoExecAddLavoroOperationCompleted;
        private SendOrPostCallback utenteGetRegistriOperationCompleted;
        private SendOrPostCallback utenteChangePasswordOperationCompleted;
        private SendOrPostCallback logoffOperationCompleted;
        private SendOrPostCallback loginOperationCompleted;
        private SendOrPostCallback fascicolazioneUpdateTitolarioOperationCompleted;
        private SendOrPostCallback fascicolazioneDelTitolarioOperationCompleted;
        private SendOrPostCallback fascicolazioneGetAutorizzazioniNodoTitolarioOperationCompleted;
        private SendOrPostCallback fascicolazioneGetCodiceFiglioTitolarioOperationCompleted;
        private SendOrPostCallback fascicolazioneNewTitolarioOperationCompleted;
        private SendOrPostCallback fascicolazioneSetAutorizzazioniNodoTitolarioOperationCompleted;
        private SendOrPostCallback trasmissioneAddTemplateOperationCompleted;
        private SendOrPostCallback trasmissioneGetListaTemplateOperationCompleted;
        private SendOrPostCallback trasmissioniUpdateTemplateOperationCompleted;
        private SendOrPostCallback trasmissioniDeleteTemplateOperationCompleted;
        private SendOrPostCallback reportCorrispondentiOperationCompleted;
        private SendOrPostCallback reportFascetteFascicoloOperationCompleted;
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

        public event documentoImportaProtocolloEmergenzaCompletedEventHandler documentoImportaProtocolloEmergenzaCompleted;

        public event addressbookGetListaCorrispondentiSempliceCompletedEventHandler addressbookGetListaCorrispondentiSempliceCompleted;

        public event licenzaCompletedEventHandler licenzaCompleted;

        public event pauseCompletedEventHandler pauseCompleted;

        public event importDelibereCompletedEventHandler importDelibereCompleted;

        public event reportTitolarioCompletedEventHandler reportTitolarioCompleted;

        public event reportBustaCompletedEventHandler reportBustaCompleted;

        public event reportSchedaDocCompletedEventHandler reportSchedaDocCompleted;

        public event faxInvioCompletedEventHandler faxInvioCompleted;

        public event faxProcessaCasellaCompletedEventHandler faxProcessaCasellaCompleted;

        public event amministrazioneSetPasswordCompletedEventHandler amministrazioneSetPasswordCompleted;

        public event amministrazioneSetDatiServerPostaCompletedEventHandler amministrazioneSetDatiServerPostaCompleted;

        public event amministrazioneGetListaServerPostaCompletedEventHandler amministrazioneGetListaServerPostaCompleted;

        public event amministrazioneGetDatiUtenteCompletedEventHandler amministrazioneGetDatiUtenteCompleted;

        public event amministrazioneGetDatiUOCompletedEventHandler amministrazioneGetDatiUOCompleted;

        public event amministrazioneGetUtentiCompletedEventHandler amministrazioneGetUtentiCompleted;

        public event amministrazioneGetDatiCorrispondenteCompletedEventHandler amministrazioneGetDatiCorrispondenteCompleted;

        public event amministrazioneGetRegistriInUOCompletedEventHandler amministrazioneGetRegistriInUOCompleted;

        public event amministrazioneSetFunzioniInRuoloCompletedEventHandler amministrazioneSetFunzioniInRuoloCompleted;

        public event amministrazioneGetFunzioniRuoloCompletedEventHandler amministrazioneGetFunzioniRuoloCompleted;

        public event amministrazioneSetRuoliUtenteCompletedEventHandler amministrazioneSetRuoliUtenteCompleted;

        public event amministrazioneGetRuoliInUOCompletedEventHandler amministrazioneGetRuoliInUOCompleted;

        public event amministrazioneGetRuoliUtenteCompletedEventHandler amministrazioneGetRuoliUtenteCompleted;

        public event amministrazioneUpdateCorrispondenteCompletedEventHandler amministrazioneUpdateCorrispondenteCompleted;

        public event amministrazioneGetDominioCompletedEventHandler amministrazioneGetDominioCompleted;

        public event amministrazioneGetDettagliCorrCompletedEventHandler amministrazioneGetDettagliCorrCompleted;

        public event amministrazioneSetUtentiInRuoloCompletedEventHandler amministrazioneSetUtentiInRuoloCompleted;

        public event amministrazioneGetUtentiInRuoloCompletedEventHandler amministrazioneGetUtentiInRuoloCompleted;

        public event MDDSelectDocumentoInMDDCompletedEventHandler MDDSelectDocumentoInMDDCompleted;

        public event MDDUpdateDocumentoInMDDCompletedEventHandler MDDUpdateDocumentoInMDDCompleted;

        public event MDDInsertDocumentoInMDDCompletedEventHandler MDDInsertDocumentoInMDDCompleted;

        public event MDDGetCampiTipoAttoCompletedEventHandler MDDGetCampiTipoAttoCompleted;

        public event amministrazioneUpdateTipologiaAttoCompletedEventHandler amministrazioneUpdateTipologiaAttoCompleted;

        public event amministrazioneInsertTipologiaAttoCompletedEventHandler amministrazioneInsertTipologiaAttoCompleted;

        public event amministrazioneInsertCorrispondenteCompletedEventHandler amministrazioneInsertCorrispondenteCompleted;

        public event amministrazioneGetTipiFunzioneCompletedEventHandler amministrazioneGetTipiFunzioneCompleted;

        public event amministrazioneGetCanaliCompletedEventHandler amministrazioneGetCanaliCompleted;

        public event amministrazioneGetTipoRuoliCompletedEventHandler amministrazioneGetTipoRuoliCompleted;

        public event amministrazioneGetRegistriCompletedEventHandler amministrazioneGetRegistriCompleted;

        public event amministrazioneGetAmministrazioniCompletedEventHandler amministrazioneGetAmministrazioniCompleted;

        public event amministrazioneGetParametroConfigurazioneCompletedEventHandler amministrazioneGetParametroConfigurazioneCompleted;

        public event registriStampaCompletedEventHandler registriStampaCompleted;

        public event registriModificaCompletedEventHandler registriModificaCompleted;

        public event registriCambiaStatoCompletedEventHandler registriCambiaStatoCompleted;

        public event interoperabilitaAggiornamentoConfermaCompletedEventHandler interoperabilitaAggiornamentoConfermaCompleted;

        public event interoperabilitaInvioRicevutaCompletedEventHandler interoperabilitaInvioRicevutaCompleted;

        public event interoperabilitaInvioCompletedEventHandler interoperabilitaInvioCompleted;

        public event interoperabilitaRicezioneCompletedEventHandler interoperabilitaRicezioneCompleted;

        public event trasmissioneGetInRispostaACompletedEventHandler trasmissioneGetInRispostaACompleted;

        public event trasmissioneGetRispostaCompletedEventHandler trasmissioneGetRispostaCompleted;

        public event trasmissioneSetTxUtAsVisteCompletedEventHandler trasmissioneSetTxUtAsVisteCompleted;

        public event trasmissioneExecuteAccRifCompletedEventHandler trasmissioneExecuteAccRifCompleted;

        public event trasmissioneGetRagioniCompletedEventHandler trasmissioneGetRagioniCompleted;

        public event trasmissioneGetQueryEffettuateCompletedEventHandler trasmissioneGetQueryEffettuateCompleted;

        public event trasmissioneGetQueryRicevuteCompletedEventHandler trasmissioneGetQueryRicevuteCompleted;

        public event trasmissioneExecuteTrasmCompletedEventHandler trasmissioneExecuteTrasmCompleted;

        public event trasmissioneSaveTrasmCompletedEventHandler trasmissioneSaveTrasmCompleted;

        public event trasmissioneGetQueryEffettuatePagingCompletedEventHandler trasmissioneGetQueryEffettuatePagingCompleted;

        public event trasmissioneGetQueryRicevutePagingCompletedEventHandler trasmissioneGetQueryRicevutePagingCompleted;

        public event fascicolazioneAddDocFolderCompletedEventHandler fascicolazioneAddDocFolderCompleted;

        public event fascicolazioneDelFolderCompletedEventHandler fascicolazioneDelFolderCompleted;

        public event fascicolazioneModifyFolderCompletedEventHandler fascicolazioneModifyFolderCompleted;

        public event fascicolazioneMoveFolderCompletedEventHandler fascicolazioneMoveFolderCompleted;

        public event fascicolazioneNewFolderCompletedEventHandler fascicolazioneNewFolderCompleted;

        public event fascicolazioneDeleteDocumentoCompletedEventHandler fascicolazioneDeleteDocumentoCompleted;

        public event fascicolazioneGetDocumentiCompletedEventHandler fascicolazioneGetDocumentiCompleted;

        public event fascicolazioneGetFolderCompletedEventHandler fascicolazioneGetFolderCompleted;

        public event fascicolazioneGetVisibilitaCompletedEventHandler fascicolazioneGetVisibilitaCompleted;

        public event fascicolazioneSospendiRiattivaUtenteCompletedEventHandler fascicolazioneSospendiRiattivaUtenteCompleted;

        public event fascicolazioneGetFascicoloDaCodiceCompletedEventHandler fascicolazioneGetFascicoloDaCodiceCompleted;

        public event fascicolazioneGetFascicoliDaDocCompletedEventHandler fascicolazioneGetFascicoliDaDocCompleted;

        public event fascicolazionesetDataVistaFascCompletedEventHandler fascicolazionesetDataVistaFascCompleted;

        public event fascicolazioneGetDettaglioFascicoloCompletedEventHandler fascicolazioneGetDettaglioFascicoloCompleted;

        public event fascicolazioneAddDocFascicoloCompletedEventHandler fascicolazioneAddDocFascicoloCompleted;

        public event fascicolazioneSetFascicoloCompletedEventHandler fascicolazioneSetFascicoloCompleted;

        public event fascicolazioneNewFascicoloCompletedEventHandler fascicolazioneNewFascicoloCompleted;

        public event fascicolazioneGetDocumentiPagingCompletedEventHandler fascicolazioneGetDocumentiPagingCompleted;

        public event fascicolazioneGetListaFascicoliPagingCompletedEventHandler fascicolazioneGetListaFascicoliPagingCompleted;

        public event fascicolazioneGetListaFascicoliCompletedEventHandler fascicolazioneGetListaFascicoliCompleted;

        public event fascicolazioneGetFigliClassificaCompletedEventHandler fascicolazioneGetFigliClassificaCompleted;

        public event fascicolazioneGetGerarchiaDaCodiceCompletedEventHandler fascicolazioneGetGerarchiaDaCodiceCompleted;

        public event fascicolazioneGetGerarchiaCompletedEventHandler fascicolazioneGetGerarchiaCompleted;

        public event fascicolazioneGetTitolarioCompletedEventHandler fascicolazioneGetTitolarioCompleted;

        public event documentoGetQueryDocumentoPagingCompletedEventHandler documentoGetQueryDocumentoPagingCompleted;

        public event documentoSpedisciCompletedEventHandler documentoSpedisciCompleted;

        public event documentoSetFlagDaInviareCompletedEventHandler documentoSetFlagDaInviareCompleted;

        public event documentoGetVisibilitaCompletedEventHandler documentoGetVisibilitaCompleted;

        public event documentoGetTipologiaCanaleCompletedEventHandler documentoGetTipologiaCanaleCompleted;

        public event documentoGetListaStoriciOggettoCompletedEventHandler documentoGetListaStoriciOggettoCompleted;

        public event documentoGetCatenaDocCompletedEventHandler documentoGetCatenaDocCompleted;

        public event documentoExecRimuoviSchedaCompletedEventHandler documentoExecRimuoviSchedaCompleted;

        public event documentoExecAnnullaProtCompletedEventHandler documentoExecAnnullaProtCompleted;

        public event documentoModificaVersioneCompletedEventHandler documentoModificaVersioneCompleted;

        public event documentoScambiaVersioniCompletedEventHandler documentoScambiaVersioniCompleted;

        public event documentoAggiungiVersioneCompletedEventHandler documentoAggiungiVersioneCompleted;

        public event documentoRimuoviVersioneCompletedEventHandler documentoRimuoviVersioneCompleted;

        public event documentoModificaAllegatoCompletedEventHandler documentoModificaAllegatoCompleted;

        public event documentoAggiungiAllegatoCompletedEventHandler documentoAggiungiAllegatoCompleted;

        public event documentoGetAllegatiCompletedEventHandler documentoGetAllegatiCompleted;

        public event documentoCercaDuplicatiCompletedEventHandler documentoCercaDuplicatiCompleted;

        public event documentoVerificaFirmaCompletedEventHandler documentoVerificaFirmaCompleted;

        public event documentoSaveDocumentoCompletedEventHandler documentoSaveDocumentoCompleted;

        public event documentoProtocollaCompletedEventHandler documentoProtocollaCompleted;

        public event documentoGetDettaglioDocumentoCompletedEventHandler documentoGetDettaglioDocumentoCompleted;

        public event MDDGetCodStatoCompletedEventHandler MDDGetCodStatoCompleted;

        public event documentoGetCategoriaAttoCompletedEventHandler documentoGetCategoriaAttoCompleted;

        public event documentoGetTipologiaAttoCompletedEventHandler documentoGetTipologiaAttoCompleted;

        public event documentoAddParolaChiaveCompletedEventHandler documentoAddParolaChiaveCompleted;

        public event documentoGetParoleChiaveCompletedEventHandler documentoGetParoleChiaveCompleted;

        public event documentoAddOggettoCompletedEventHandler documentoAddOggettoCompleted;

        public event documentoGetListaOggettiCompletedEventHandler documentoGetListaOggettiCompleted;

        public event documentoPutFileCompletedEventHandler documentoPutFileCompleted;

        public event documentoGetFileCompletedEventHandler documentoGetFileCompleted;

        public event documentoAddDocGrigiaCompletedEventHandler documentoAddDocGrigiaCompleted;

        public event documentoGetAreaLavoroPagingCompletedEventHandler documentoGetAreaLavoroPagingCompleted;

        public event documentoGetSegnaturaCampiVariabiliCompletedEventHandler documentoGetSegnaturaCampiVariabiliCompleted;

        public event documentoGetQueryDocumentoCompletedEventHandler documentoGetQueryDocumentoCompleted;

        public event addressbookGetCanaliCompletedEventHandler addressbookGetCanaliCompleted;

        public event addressbookGetRuoliSuperioriInUOCompletedEventHandler addressbookGetRuoliSuperioriInUOCompleted;

        public event addressbookInsertCorrispondenteCompletedEventHandler addressbookInsertCorrispondenteCompleted;

        public event addressbookGetRootUOCompletedEventHandler addressbookGetRootUOCompleted;

        public event addressbookGetListaCorrispondentiAutorizzatiCompletedEventHandler addressbookGetListaCorrispondentiAutorizzatiCompleted;

        public event addressbookGetDettagliCorrispondenteCompletedEventHandler addressbookGetDettagliCorrispondenteCompleted;

        public event addressbookGetListaCorrispondentiCompletedEventHandler addressbookGetListaCorrispondentiCompleted;

        public event documentoCancellaAreaLavoroCompletedEventHandler documentoCancellaAreaLavoroCompleted;

        public event documentoGetAreaLavoroCompletedEventHandler documentoGetAreaLavoroCompleted;

        public event documentoGetApplicazioniCompletedEventHandler documentoGetApplicazioniCompleted;

        public event documentoExecAddLavoroCompletedEventHandler documentoExecAddLavoroCompleted;

        public event utenteGetRegistriCompletedEventHandler utenteGetRegistriCompleted;

        public event utenteChangePasswordCompletedEventHandler utenteChangePasswordCompleted;

        public event logoffCompletedEventHandler logoffCompleted;

        public event loginCompletedEventHandler loginCompleted;

        public event fascicolazioneUpdateTitolarioCompletedEventHandler fascicolazioneUpdateTitolarioCompleted;

        public event fascicolazioneDelTitolarioCompletedEventHandler fascicolazioneDelTitolarioCompleted;

        public event fascicolazioneGetAutorizzazioniNodoTitolarioCompletedEventHandler fascicolazioneGetAutorizzazioniNodoTitolarioCompleted;

        public event fascicolazioneGetCodiceFiglioTitolarioCompletedEventHandler fascicolazioneGetCodiceFiglioTitolarioCompleted;

        public event fascicolazioneNewTitolarioCompletedEventHandler fascicolazioneNewTitolarioCompleted;

        public event fascicolazioneSetAutorizzazioniNodoTitolarioCompletedEventHandler fascicolazioneSetAutorizzazioniNodoTitolarioCompleted;

        public event trasmissioneAddTemplateCompletedEventHandler trasmissioneAddTemplateCompleted;

        public event trasmissioneGetListaTemplateCompletedEventHandler trasmissioneGetListaTemplateCompleted;

        public event trasmissioniUpdateTemplateCompletedEventHandler trasmissioniUpdateTemplateCompleted;

        public event trasmissioniDeleteTemplateCompletedEventHandler trasmissioniDeleteTemplateCompleted;

        public event reportCorrispondentiCompletedEventHandler reportCorrispondentiCompleted;

        public event reportFascetteFascicoloCompletedEventHandler reportFascetteFascicoloCompleted;

        public DocsPaWebService()
        {
            this.Url = Settings.Default.StampaRegistri_DocsPaWR25_DocsPaWebService;
            if (this.IsLocalFileSystemWebService(this.Url))
            {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else
                this.useDefaultCredentialsSetExplicitly = true;
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

        [SoapDocumentMethod("http://localhost/addressbookGetListaCorrispondentiSemplice", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente[] addressbookGetListaCorrispondentiSemplice(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente)
        {
            return (Corrispondente[])this.Invoke("addressbookGetListaCorrispondentiSemplice", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginaddressbookGetListaCorrispondentiSemplice(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("addressbookGetListaCorrispondentiSemplice", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Corrispondente[] EndaddressbookGetListaCorrispondentiSemplice(IAsyncResult asyncResult)
        {
            return (Corrispondente[])this.EndInvoke(asyncResult)[0];
        }

        public void addressbookGetListaCorrispondentiSempliceAsync(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente)
        {
            this.addressbookGetListaCorrispondentiSempliceAsync(queryCorrispondente, infoUtente, (object)null);
        }

        public void addressbookGetListaCorrispondentiSempliceAsync(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente, object userState)
        {
            if (this.addressbookGetListaCorrispondentiSempliceOperationCompleted == null)
                this.addressbookGetListaCorrispondentiSempliceOperationCompleted = new SendOrPostCallback(this.OnaddressbookGetListaCorrispondentiSempliceOperationCompleted);
            this.InvokeAsync("addressbookGetListaCorrispondentiSemplice", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      }, this.addressbookGetListaCorrispondentiSempliceOperationCompleted, userState);
        }

        private void OnaddressbookGetListaCorrispondentiSempliceOperationCompleted(object arg)
        {
            if (this.addressbookGetListaCorrispondentiSempliceCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.addressbookGetListaCorrispondentiSempliceCompleted((object)this, new addressbookGetListaCorrispondentiSempliceCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/licenza", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string licenza()
        {
            return (string)this.Invoke("licenza", new object[0])[0];
        }

        public IAsyncResult Beginlicenza(AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("licenza", new object[0], callback, asyncState);
        }

        public string Endlicenza(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void licenzaAsync()
        {
            this.licenzaAsync((object)null);
        }

        public void licenzaAsync(object userState)
        {
            if (this.licenzaOperationCompleted == null)
                this.licenzaOperationCompleted = new SendOrPostCallback(this.OnlicenzaOperationCompleted);
            this.InvokeAsync("licenza", new object[0], this.licenzaOperationCompleted, userState);
        }

        private void OnlicenzaOperationCompleted(object arg)
        {
            if (this.licenzaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.licenzaCompleted((object)this, new licenzaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/pause", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void pause(double timeToPause)
        {
            this.Invoke("pause", new object[1]
      {
        (object) timeToPause
      });
        }

        public IAsyncResult Beginpause(double timeToPause, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("pause", new object[1]
      {
        (object) timeToPause
      }, callback, asyncState);
        }

        public void Endpause(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void pauseAsync(double timeToPause)
        {
            this.pauseAsync(timeToPause, (object)null);
        }

        public void pauseAsync(double timeToPause, object userState)
        {
            if (this.pauseOperationCompleted == null)
                this.pauseOperationCompleted = new SendOrPostCallback(this.OnpauseOperationCompleted);
            this.InvokeAsync("pause", new object[1]
      {
        (object) timeToPause
      }, this.pauseOperationCompleted, userState);
        }

        private void OnpauseOperationCompleted(object arg)
        {
            if (this.pauseCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.pauseCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/importDelibere", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool importDelibere(string path, string data, InfoUtente infoUtente)
        {
            return (bool)this.Invoke("importDelibere", new object[3]
      {
        (object) path,
        (object) data,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginimportDelibere(string path, string data, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("importDelibere", new object[3]
      {
        (object) path,
        (object) data,
        (object) infoUtente
      }, callback, asyncState);
        }

        public bool EndimportDelibere(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void importDelibereAsync(string path, string data, InfoUtente infoUtente)
        {
            this.importDelibereAsync(path, data, infoUtente, (object)null);
        }

        public void importDelibereAsync(string path, string data, InfoUtente infoUtente, object userState)
        {
            if (this.importDelibereOperationCompleted == null)
                this.importDelibereOperationCompleted = new SendOrPostCallback(this.OnimportDelibereOperationCompleted);
            this.InvokeAsync("importDelibere", new object[3]
      {
        (object) path,
        (object) data,
        (object) infoUtente
      }, this.importDelibereOperationCompleted, userState);
        }

        private void OnimportDelibereOperationCompleted(object arg)
        {
            if (this.importDelibereCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.importDelibereCompleted((object)this, new importDelibereCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/reportTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileDocumento reportTitolario(Registro registro, InfoUtente infoUtente)
        {
            return (FileDocumento)this.Invoke("reportTitolario", new object[2]
      {
        (object) registro,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginreportTitolario(Registro registro, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("reportTitolario", new object[2]
      {
        (object) registro,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FileDocumento EndreportTitolario(IAsyncResult asyncResult)
        {
            return (FileDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void reportTitolarioAsync(Registro registro, InfoUtente infoUtente)
        {
            this.reportTitolarioAsync(registro, infoUtente, (object)null);
        }

        public void reportTitolarioAsync(Registro registro, InfoUtente infoUtente, object userState)
        {
            if (this.reportTitolarioOperationCompleted == null)
                this.reportTitolarioOperationCompleted = new SendOrPostCallback(this.OnreportTitolarioOperationCompleted);
            this.InvokeAsync("reportTitolario", new object[2]
      {
        (object) registro,
        (object) infoUtente
      }, this.reportTitolarioOperationCompleted, userState);
        }

        private void OnreportTitolarioOperationCompleted(object arg)
        {
            if (this.reportTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.reportTitolarioCompleted((object)this, new reportTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/reportBusta", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileDocumento reportBusta(SchedaDocumento schedaDoc, InfoUtente infoUtente)
        {
            return (FileDocumento)this.Invoke("reportBusta", new object[2]
      {
        (object) schedaDoc,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginreportBusta(SchedaDocumento schedaDoc, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("reportBusta", new object[2]
      {
        (object) schedaDoc,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FileDocumento EndreportBusta(IAsyncResult asyncResult)
        {
            return (FileDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void reportBustaAsync(SchedaDocumento schedaDoc, InfoUtente infoUtente)
        {
            this.reportBustaAsync(schedaDoc, infoUtente, (object)null);
        }

        public void reportBustaAsync(SchedaDocumento schedaDoc, InfoUtente infoUtente, object userState)
        {
            if (this.reportBustaOperationCompleted == null)
                this.reportBustaOperationCompleted = new SendOrPostCallback(this.OnreportBustaOperationCompleted);
            this.InvokeAsync("reportBusta", new object[2]
      {
        (object) schedaDoc,
        (object) infoUtente
      }, this.reportBustaOperationCompleted, userState);
        }

        private void OnreportBustaOperationCompleted(object arg)
        {
            if (this.reportBustaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.reportBustaCompleted((object)this, new reportBustaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/reportSchedaDoc", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileDocumento reportSchedaDoc(SchedaDocumento schedaDoc, InfoUtente infoUtente)
        {
            return (FileDocumento)this.Invoke("reportSchedaDoc", new object[2]
      {
        (object) schedaDoc,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginreportSchedaDoc(SchedaDocumento schedaDoc, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("reportSchedaDoc", new object[2]
      {
        (object) schedaDoc,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FileDocumento EndreportSchedaDoc(IAsyncResult asyncResult)
        {
            return (FileDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void reportSchedaDocAsync(SchedaDocumento schedaDoc, InfoUtente infoUtente)
        {
            this.reportSchedaDocAsync(schedaDoc, infoUtente, (object)null);
        }

        public void reportSchedaDocAsync(SchedaDocumento schedaDoc, InfoUtente infoUtente, object userState)
        {
            if (this.reportSchedaDocOperationCompleted == null)
                this.reportSchedaDocOperationCompleted = new SendOrPostCallback(this.OnreportSchedaDocOperationCompleted);
            this.InvokeAsync("reportSchedaDoc", new object[2]
      {
        (object) schedaDoc,
        (object) infoUtente
      }, this.reportSchedaDocOperationCompleted, userState);
        }

        private void OnreportSchedaDocOperationCompleted(object arg)
        {
            if (this.reportSchedaDocCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.reportSchedaDocCompleted((object)this, new reportSchedaDocCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/faxInvio", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void faxInvio(InfoUtente infoUtente, Registro registro, SchedaDocumento schedaDoc)
        {
            this.Invoke("faxInvio", new object[3]
      {
        (object) infoUtente,
        (object) registro,
        (object) schedaDoc
      });
        }

        public IAsyncResult BeginfaxInvio(InfoUtente infoUtente, Registro registro, SchedaDocumento schedaDoc, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("faxInvio", new object[3]
      {
        (object) infoUtente,
        (object) registro,
        (object) schedaDoc
      }, callback, asyncState);
        }

        public void EndfaxInvio(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void faxInvioAsync(InfoUtente infoUtente, Registro registro, SchedaDocumento schedaDoc)
        {
            this.faxInvioAsync(infoUtente, registro, schedaDoc, (object)null);
        }

        public void faxInvioAsync(InfoUtente infoUtente, Registro registro, SchedaDocumento schedaDoc, object userState)
        {
            if (this.faxInvioOperationCompleted == null)
                this.faxInvioOperationCompleted = new SendOrPostCallback(this.OnfaxInvioOperationCompleted);
            this.InvokeAsync("faxInvio", new object[3]
      {
        (object) infoUtente,
        (object) registro,
        (object) schedaDoc
      }, this.faxInvioOperationCompleted, userState);
        }

        private void OnfaxInvioOperationCompleted(object arg)
        {
            if (this.faxInvioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.faxInvioCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/faxProcessaCasella", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public int faxProcessaCasella(string serverName, InfoUtente infoUtente, Ruolo ruolo, Registro registro)
        {
            return (int)this.Invoke("faxProcessaCasella", new object[4]
      {
        (object) serverName,
        (object) infoUtente,
        (object) ruolo,
        (object) registro
      })[0];
        }

        public IAsyncResult BeginfaxProcessaCasella(string serverName, InfoUtente infoUtente, Ruolo ruolo, Registro registro, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("faxProcessaCasella", new object[4]
      {
        (object) serverName,
        (object) infoUtente,
        (object) ruolo,
        (object) registro
      }, callback, asyncState);
        }

        public int EndfaxProcessaCasella(IAsyncResult asyncResult)
        {
            return (int)this.EndInvoke(asyncResult)[0];
        }

        public void faxProcessaCasellaAsync(string serverName, InfoUtente infoUtente, Ruolo ruolo, Registro registro)
        {
            this.faxProcessaCasellaAsync(serverName, infoUtente, ruolo, registro, (object)null);
        }

        public void faxProcessaCasellaAsync(string serverName, InfoUtente infoUtente, Ruolo ruolo, Registro registro, object userState)
        {
            if (this.faxProcessaCasellaOperationCompleted == null)
                this.faxProcessaCasellaOperationCompleted = new SendOrPostCallback(this.OnfaxProcessaCasellaOperationCompleted);
            this.InvokeAsync("faxProcessaCasella", new object[4]
      {
        (object) serverName,
        (object) infoUtente,
        (object) ruolo,
        (object) registro
      }, this.faxProcessaCasellaOperationCompleted, userState);
        }

        private void OnfaxProcessaCasellaOperationCompleted(object arg)
        {
            if (this.faxProcessaCasellaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.faxProcessaCasellaCompleted((object)this, new faxProcessaCasellaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneSetPassword", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void amministrazioneSetPassword(Utente utente, string newPWD, string oldPWD, InfoUtente infoUtente)
        {
            this.Invoke("amministrazioneSetPassword", new object[4]
      {
        (object) utente,
        (object) newPWD,
        (object) oldPWD,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginamministrazioneSetPassword(Utente utente, string newPWD, string oldPWD, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneSetPassword", new object[4]
      {
        (object) utente,
        (object) newPWD,
        (object) oldPWD,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndamministrazioneSetPassword(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void amministrazioneSetPasswordAsync(Utente utente, string newPWD, string oldPWD, InfoUtente infoUtente)
        {
            this.amministrazioneSetPasswordAsync(utente, newPWD, oldPWD, infoUtente, (object)null);
        }

        public void amministrazioneSetPasswordAsync(Utente utente, string newPWD, string oldPWD, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneSetPasswordOperationCompleted == null)
                this.amministrazioneSetPasswordOperationCompleted = new SendOrPostCallback(this.OnamministrazioneSetPasswordOperationCompleted);
            this.InvokeAsync("amministrazioneSetPassword", new object[4]
      {
        (object) utente,
        (object) newPWD,
        (object) oldPWD,
        (object) infoUtente
      }, this.amministrazioneSetPasswordOperationCompleted, userState);
        }

        private void OnamministrazioneSetPasswordOperationCompleted(object arg)
        {
            if (this.amministrazioneSetPasswordCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneSetPasswordCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneSetDatiServerPosta", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void amministrazioneSetDatiServerPosta(Corrispondente corr, InfoUtente infoUtente)
        {
            this.Invoke("amministrazioneSetDatiServerPosta", new object[2]
      {
        (object) corr,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginamministrazioneSetDatiServerPosta(Corrispondente corr, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneSetDatiServerPosta", new object[2]
      {
        (object) corr,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndamministrazioneSetDatiServerPosta(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void amministrazioneSetDatiServerPostaAsync(Corrispondente corr, InfoUtente infoUtente)
        {
            this.amministrazioneSetDatiServerPostaAsync(corr, infoUtente, (object)null);
        }

        public void amministrazioneSetDatiServerPostaAsync(Corrispondente corr, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneSetDatiServerPostaOperationCompleted == null)
                this.amministrazioneSetDatiServerPostaOperationCompleted = new SendOrPostCallback(this.OnamministrazioneSetDatiServerPostaOperationCompleted);
            this.InvokeAsync("amministrazioneSetDatiServerPosta", new object[2]
      {
        (object) corr,
        (object) infoUtente
      }, this.amministrazioneSetDatiServerPostaOperationCompleted, userState);
        }

        private void OnamministrazioneSetDatiServerPostaOperationCompleted(object arg)
        {
            if (this.amministrazioneSetDatiServerPostaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneSetDatiServerPostaCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetListaServerPosta", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public ServerPosta[] amministrazioneGetListaServerPosta(InfoUtente infoUtente)
        {
            return (ServerPosta[])this.Invoke("amministrazioneGetListaServerPosta", new object[1]
      {
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetListaServerPosta(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetListaServerPosta", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public ServerPosta[] EndamministrazioneGetListaServerPosta(IAsyncResult asyncResult)
        {
            return (ServerPosta[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetListaServerPostaAsync(InfoUtente infoUtente)
        {
            this.amministrazioneGetListaServerPostaAsync(infoUtente, (object)null);
        }

        public void amministrazioneGetListaServerPostaAsync(InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetListaServerPostaOperationCompleted == null)
                this.amministrazioneGetListaServerPostaOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetListaServerPostaOperationCompleted);
            this.InvokeAsync("amministrazioneGetListaServerPosta", new object[1]
      {
        (object) infoUtente
      }, this.amministrazioneGetListaServerPostaOperationCompleted, userState);
        }

        private void OnamministrazioneGetListaServerPostaOperationCompleted(object arg)
        {
            if (this.amministrazioneGetListaServerPostaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetListaServerPostaCompleted((object)this, new amministrazioneGetListaServerPostaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetDatiUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Utente amministrazioneGetDatiUtente(Utente utente, InfoUtente infoUtente)
        {
            return (Utente)this.Invoke("amministrazioneGetDatiUtente", new object[2]
      {
        (object) utente,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetDatiUtente(Utente utente, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetDatiUtente", new object[2]
      {
        (object) utente,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Utente EndamministrazioneGetDatiUtente(IAsyncResult asyncResult)
        {
            return (Utente)this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetDatiUtenteAsync(Utente utente, InfoUtente infoUtente)
        {
            this.amministrazioneGetDatiUtenteAsync(utente, infoUtente, (object)null);
        }

        public void amministrazioneGetDatiUtenteAsync(Utente utente, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetDatiUtenteOperationCompleted == null)
                this.amministrazioneGetDatiUtenteOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetDatiUtenteOperationCompleted);
            this.InvokeAsync("amministrazioneGetDatiUtente", new object[2]
      {
        (object) utente,
        (object) infoUtente
      }, this.amministrazioneGetDatiUtenteOperationCompleted, userState);
        }

        private void OnamministrazioneGetDatiUtenteOperationCompleted(object arg)
        {
            if (this.amministrazioneGetDatiUtenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetDatiUtenteCompleted((object)this, new amministrazioneGetDatiUtenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetDatiUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public UnitaOrganizzativa amministrazioneGetDatiUO(UnitaOrganizzativa uo, InfoUtente infoUtente)
        {
            return (UnitaOrganizzativa)this.Invoke("amministrazioneGetDatiUO", new object[2]
      {
        (object) uo,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetDatiUO(UnitaOrganizzativa uo, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetDatiUO", new object[2]
      {
        (object) uo,
        (object) infoUtente
      }, callback, asyncState);
        }

        public UnitaOrganizzativa EndamministrazioneGetDatiUO(IAsyncResult asyncResult)
        {
            return (UnitaOrganizzativa)this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetDatiUOAsync(UnitaOrganizzativa uo, InfoUtente infoUtente)
        {
            this.amministrazioneGetDatiUOAsync(uo, infoUtente, (object)null);
        }

        public void amministrazioneGetDatiUOAsync(UnitaOrganizzativa uo, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetDatiUOOperationCompleted == null)
                this.amministrazioneGetDatiUOOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetDatiUOOperationCompleted);
            this.InvokeAsync("amministrazioneGetDatiUO", new object[2]
      {
        (object) uo,
        (object) infoUtente
      }, this.amministrazioneGetDatiUOOperationCompleted, userState);
        }

        private void OnamministrazioneGetDatiUOOperationCompleted(object arg)
        {
            if (this.amministrazioneGetDatiUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetDatiUOCompleted((object)this, new amministrazioneGetDatiUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetUtenti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Utente[] amministrazioneGetUtenti(string idAmm, string search, InfoUtente infoUtente)
        {
            return (Utente[])this.Invoke("amministrazioneGetUtenti", new object[3]
      {
        (object) idAmm,
        (object) search,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetUtenti(string idAmm, string search, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetUtenti", new object[3]
      {
        (object) idAmm,
        (object) search,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Utente[] EndamministrazioneGetUtenti(IAsyncResult asyncResult)
        {
            return (Utente[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetUtentiAsync(string idAmm, string search, InfoUtente infoUtente)
        {
            this.amministrazioneGetUtentiAsync(idAmm, search, infoUtente, (object)null);
        }

        public void amministrazioneGetUtentiAsync(string idAmm, string search, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetUtentiOperationCompleted == null)
                this.amministrazioneGetUtentiOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetUtentiOperationCompleted);
            this.InvokeAsync("amministrazioneGetUtenti", new object[3]
      {
        (object) idAmm,
        (object) search,
        (object) infoUtente
      }, this.amministrazioneGetUtentiOperationCompleted, userState);
        }

        private void OnamministrazioneGetUtentiOperationCompleted(object arg)
        {
            if (this.amministrazioneGetUtentiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetUtentiCompleted((object)this, new amministrazioneGetUtentiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetDatiCorrispondente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente amministrazioneGetDatiCorrispondente(Corrispondente corr, InfoUtente infoUtente)
        {
            return (Corrispondente)this.Invoke("amministrazioneGetDatiCorrispondente", new object[2]
      {
        (object) corr,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetDatiCorrispondente(Corrispondente corr, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetDatiCorrispondente", new object[2]
      {
        (object) corr,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Corrispondente EndamministrazioneGetDatiCorrispondente(IAsyncResult asyncResult)
        {
            return (Corrispondente)this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetDatiCorrispondenteAsync(Corrispondente corr, InfoUtente infoUtente)
        {
            this.amministrazioneGetDatiCorrispondenteAsync(corr, infoUtente, (object)null);
        }

        public void amministrazioneGetDatiCorrispondenteAsync(Corrispondente corr, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetDatiCorrispondenteOperationCompleted == null)
                this.amministrazioneGetDatiCorrispondenteOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetDatiCorrispondenteOperationCompleted);
            this.InvokeAsync("amministrazioneGetDatiCorrispondente", new object[2]
      {
        (object) corr,
        (object) infoUtente
      }, this.amministrazioneGetDatiCorrispondenteOperationCompleted, userState);
        }

        private void OnamministrazioneGetDatiCorrispondenteOperationCompleted(object arg)
        {
            if (this.amministrazioneGetDatiCorrispondenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetDatiCorrispondenteCompleted((object)this, new amministrazioneGetDatiCorrispondenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetRegistriInUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Registro[] amministrazioneGetRegistriInUO(UnitaOrganizzativa uo, InfoUtente infoUtente)
        {
            return (Registro[])this.Invoke("amministrazioneGetRegistriInUO", new object[2]
      {
        (object) uo,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetRegistriInUO(UnitaOrganizzativa uo, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetRegistriInUO", new object[2]
      {
        (object) uo,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Registro[] EndamministrazioneGetRegistriInUO(IAsyncResult asyncResult)
        {
            return (Registro[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetRegistriInUOAsync(UnitaOrganizzativa uo, InfoUtente infoUtente)
        {
            this.amministrazioneGetRegistriInUOAsync(uo, infoUtente, (object)null);
        }

        public void amministrazioneGetRegistriInUOAsync(UnitaOrganizzativa uo, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetRegistriInUOOperationCompleted == null)
                this.amministrazioneGetRegistriInUOOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetRegistriInUOOperationCompleted);
            this.InvokeAsync("amministrazioneGetRegistriInUO", new object[2]
      {
        (object) uo,
        (object) infoUtente
      }, this.amministrazioneGetRegistriInUOOperationCompleted, userState);
        }

        private void OnamministrazioneGetRegistriInUOOperationCompleted(object arg)
        {
            if (this.amministrazioneGetRegistriInUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetRegistriInUOCompleted((object)this, new amministrazioneGetRegistriInUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneSetFunzioniInRuolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void amministrazioneSetFunzioniInRuolo(Ruolo ruolo, TipiFunzione[] funzAdd, TipiFunzione[] funzRemove, InfoUtente infoUtente)
        {
            this.Invoke("amministrazioneSetFunzioniInRuolo", new object[4]
      {
        (object) ruolo,
        (object) funzAdd,
        (object) funzRemove,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginamministrazioneSetFunzioniInRuolo(Ruolo ruolo, TipiFunzione[] funzAdd, TipiFunzione[] funzRemove, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneSetFunzioniInRuolo", new object[4]
      {
        (object) ruolo,
        (object) funzAdd,
        (object) funzRemove,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndamministrazioneSetFunzioniInRuolo(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void amministrazioneSetFunzioniInRuoloAsync(Ruolo ruolo, TipiFunzione[] funzAdd, TipiFunzione[] funzRemove, InfoUtente infoUtente)
        {
            this.amministrazioneSetFunzioniInRuoloAsync(ruolo, funzAdd, funzRemove, infoUtente, (object)null);
        }

        public void amministrazioneSetFunzioniInRuoloAsync(Ruolo ruolo, TipiFunzione[] funzAdd, TipiFunzione[] funzRemove, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneSetFunzioniInRuoloOperationCompleted == null)
                this.amministrazioneSetFunzioniInRuoloOperationCompleted = new SendOrPostCallback(this.OnamministrazioneSetFunzioniInRuoloOperationCompleted);
            this.InvokeAsync("amministrazioneSetFunzioniInRuolo", new object[4]
      {
        (object) ruolo,
        (object) funzAdd,
        (object) funzRemove,
        (object) infoUtente
      }, this.amministrazioneSetFunzioniInRuoloOperationCompleted, userState);
        }

        private void OnamministrazioneSetFunzioniInRuoloOperationCompleted(object arg)
        {
            if (this.amministrazioneSetFunzioniInRuoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneSetFunzioniInRuoloCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetFunzioniRuolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TipiFunzione[] amministrazioneGetFunzioniRuolo(Ruolo ruolo, InfoUtente infoUtente)
        {
            return (TipiFunzione[])this.Invoke("amministrazioneGetFunzioniRuolo", new object[2]
      {
        (object) ruolo,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetFunzioniRuolo(Ruolo ruolo, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetFunzioniRuolo", new object[2]
      {
        (object) ruolo,
        (object) infoUtente
      }, callback, asyncState);
        }

        public TipiFunzione[] EndamministrazioneGetFunzioniRuolo(IAsyncResult asyncResult)
        {
            return (TipiFunzione[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetFunzioniRuoloAsync(Ruolo ruolo, InfoUtente infoUtente)
        {
            this.amministrazioneGetFunzioniRuoloAsync(ruolo, infoUtente, (object)null);
        }

        public void amministrazioneGetFunzioniRuoloAsync(Ruolo ruolo, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetFunzioniRuoloOperationCompleted == null)
                this.amministrazioneGetFunzioniRuoloOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetFunzioniRuoloOperationCompleted);
            this.InvokeAsync("amministrazioneGetFunzioniRuolo", new object[2]
      {
        (object) ruolo,
        (object) infoUtente
      }, this.amministrazioneGetFunzioniRuoloOperationCompleted, userState);
        }

        private void OnamministrazioneGetFunzioniRuoloOperationCompleted(object arg)
        {
            if (this.amministrazioneGetFunzioniRuoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetFunzioniRuoloCompleted((object)this, new amministrazioneGetFunzioniRuoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneSetRuoliUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void amministrazioneSetRuoliUtente(Ruolo[] ruoliAdd, Ruolo[] ruoliRemove, Utente utente, InfoUtente infoUtente)
        {
            this.Invoke("amministrazioneSetRuoliUtente", new object[4]
      {
        (object) ruoliAdd,
        (object) ruoliRemove,
        (object) utente,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginamministrazioneSetRuoliUtente(Ruolo[] ruoliAdd, Ruolo[] ruoliRemove, Utente utente, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneSetRuoliUtente", new object[4]
      {
        (object) ruoliAdd,
        (object) ruoliRemove,
        (object) utente,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndamministrazioneSetRuoliUtente(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void amministrazioneSetRuoliUtenteAsync(Ruolo[] ruoliAdd, Ruolo[] ruoliRemove, Utente utente, InfoUtente infoUtente)
        {
            this.amministrazioneSetRuoliUtenteAsync(ruoliAdd, ruoliRemove, utente, infoUtente, (object)null);
        }

        public void amministrazioneSetRuoliUtenteAsync(Ruolo[] ruoliAdd, Ruolo[] ruoliRemove, Utente utente, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneSetRuoliUtenteOperationCompleted == null)
                this.amministrazioneSetRuoliUtenteOperationCompleted = new SendOrPostCallback(this.OnamministrazioneSetRuoliUtenteOperationCompleted);
            this.InvokeAsync("amministrazioneSetRuoliUtente", new object[4]
      {
        (object) ruoliAdd,
        (object) ruoliRemove,
        (object) utente,
        (object) infoUtente
      }, this.amministrazioneSetRuoliUtenteOperationCompleted, userState);
        }

        private void OnamministrazioneSetRuoliUtenteOperationCompleted(object arg)
        {
            if (this.amministrazioneSetRuoliUtenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneSetRuoliUtenteCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
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

        [SoapDocumentMethod("http://localhost/amministrazioneGetRuoliUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Ruolo[] amministrazioneGetRuoliUtente(Utente utente, InfoUtente infoUtente)
        {
            return (Ruolo[])this.Invoke("amministrazioneGetRuoliUtente", new object[2]
      {
        (object) utente,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetRuoliUtente(Utente utente, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetRuoliUtente", new object[2]
      {
        (object) utente,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Ruolo[] EndamministrazioneGetRuoliUtente(IAsyncResult asyncResult)
        {
            return (Ruolo[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetRuoliUtenteAsync(Utente utente, InfoUtente infoUtente)
        {
            this.amministrazioneGetRuoliUtenteAsync(utente, infoUtente, (object)null);
        }

        public void amministrazioneGetRuoliUtenteAsync(Utente utente, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetRuoliUtenteOperationCompleted == null)
                this.amministrazioneGetRuoliUtenteOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetRuoliUtenteOperationCompleted);
            this.InvokeAsync("amministrazioneGetRuoliUtente", new object[2]
      {
        (object) utente,
        (object) infoUtente
      }, this.amministrazioneGetRuoliUtenteOperationCompleted, userState);
        }

        private void OnamministrazioneGetRuoliUtenteOperationCompleted(object arg)
        {
            if (this.amministrazioneGetRuoliUtenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetRuoliUtenteCompleted((object)this, new amministrazioneGetRuoliUtenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneUpdateCorrispondente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente amministrazioneUpdateCorrispondente(Corrispondente corrispondente, Login login, InfoUtente infoUtente)
        {
            return (Corrispondente)this.Invoke("amministrazioneUpdateCorrispondente", new object[3]
      {
        (object) corrispondente,
        (object) login,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneUpdateCorrispondente(Corrispondente corrispondente, Login login, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneUpdateCorrispondente", new object[3]
      {
        (object) corrispondente,
        (object) login,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Corrispondente EndamministrazioneUpdateCorrispondente(IAsyncResult asyncResult)
        {
            return (Corrispondente)this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneUpdateCorrispondenteAsync(Corrispondente corrispondente, Login login, InfoUtente infoUtente)
        {
            this.amministrazioneUpdateCorrispondenteAsync(corrispondente, login, infoUtente, (object)null);
        }

        public void amministrazioneUpdateCorrispondenteAsync(Corrispondente corrispondente, Login login, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneUpdateCorrispondenteOperationCompleted == null)
                this.amministrazioneUpdateCorrispondenteOperationCompleted = new SendOrPostCallback(this.OnamministrazioneUpdateCorrispondenteOperationCompleted);
            this.InvokeAsync("amministrazioneUpdateCorrispondente", new object[3]
      {
        (object) corrispondente,
        (object) login,
        (object) infoUtente
      }, this.amministrazioneUpdateCorrispondenteOperationCompleted, userState);
        }

        private void OnamministrazioneUpdateCorrispondenteOperationCompleted(object arg)
        {
            if (this.amministrazioneUpdateCorrispondenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneUpdateCorrispondenteCompleted((object)this, new amministrazioneUpdateCorrispondenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetDominio", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Login amministrazioneGetDominio(Corrispondente corr, InfoUtente infoUtente)
        {
            return (Login)this.Invoke("amministrazioneGetDominio", new object[2]
      {
        (object) corr,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetDominio(Corrispondente corr, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetDominio", new object[2]
      {
        (object) corr,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Login EndamministrazioneGetDominio(IAsyncResult asyncResult)
        {
            return (Login)this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetDominioAsync(Corrispondente corr, InfoUtente infoUtente)
        {
            this.amministrazioneGetDominioAsync(corr, infoUtente, (object)null);
        }

        public void amministrazioneGetDominioAsync(Corrispondente corr, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetDominioOperationCompleted == null)
                this.amministrazioneGetDominioOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetDominioOperationCompleted);
            this.InvokeAsync("amministrazioneGetDominio", new object[2]
      {
        (object) corr,
        (object) infoUtente
      }, this.amministrazioneGetDominioOperationCompleted, userState);
        }

        private void OnamministrazioneGetDominioOperationCompleted(object arg)
        {
            if (this.amministrazioneGetDominioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetDominioCompleted((object)this, new amministrazioneGetDominioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetDettagliCorr", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void amministrazioneGetDettagliCorr(Corrispondente corr, InfoUtente infoUtente)
        {
            this.Invoke("amministrazioneGetDettagliCorr", new object[2]
      {
        (object) corr,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginamministrazioneGetDettagliCorr(Corrispondente corr, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetDettagliCorr", new object[2]
      {
        (object) corr,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndamministrazioneGetDettagliCorr(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void amministrazioneGetDettagliCorrAsync(Corrispondente corr, InfoUtente infoUtente)
        {
            this.amministrazioneGetDettagliCorrAsync(corr, infoUtente, (object)null);
        }

        public void amministrazioneGetDettagliCorrAsync(Corrispondente corr, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetDettagliCorrOperationCompleted == null)
                this.amministrazioneGetDettagliCorrOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetDettagliCorrOperationCompleted);
            this.InvokeAsync("amministrazioneGetDettagliCorr", new object[2]
      {
        (object) corr,
        (object) infoUtente
      }, this.amministrazioneGetDettagliCorrOperationCompleted, userState);
        }

        private void OnamministrazioneGetDettagliCorrOperationCompleted(object arg)
        {
            if (this.amministrazioneGetDettagliCorrCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetDettagliCorrCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneSetUtentiInRuolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void amministrazioneSetUtentiInRuolo(InfoUtente[] utAdd, InfoUtente[] utRemove, InfoUtente infoUtente)
        {
            this.Invoke("amministrazioneSetUtentiInRuolo", new object[3]
      {
        (object) utAdd,
        (object) utRemove,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginamministrazioneSetUtentiInRuolo(InfoUtente[] utAdd, InfoUtente[] utRemove, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneSetUtentiInRuolo", new object[3]
      {
        (object) utAdd,
        (object) utRemove,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndamministrazioneSetUtentiInRuolo(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void amministrazioneSetUtentiInRuoloAsync(InfoUtente[] utAdd, InfoUtente[] utRemove, InfoUtente infoUtente)
        {
            this.amministrazioneSetUtentiInRuoloAsync(utAdd, utRemove, infoUtente, (object)null);
        }

        public void amministrazioneSetUtentiInRuoloAsync(InfoUtente[] utAdd, InfoUtente[] utRemove, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneSetUtentiInRuoloOperationCompleted == null)
                this.amministrazioneSetUtentiInRuoloOperationCompleted = new SendOrPostCallback(this.OnamministrazioneSetUtentiInRuoloOperationCompleted);
            this.InvokeAsync("amministrazioneSetUtentiInRuolo", new object[3]
      {
        (object) utAdd,
        (object) utRemove,
        (object) infoUtente
      }, this.amministrazioneSetUtentiInRuoloOperationCompleted, userState);
        }

        private void OnamministrazioneSetUtentiInRuoloOperationCompleted(object arg)
        {
            if (this.amministrazioneSetUtentiInRuoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneSetUtentiInRuoloCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetUtentiInRuolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public InfoUtente[] amministrazioneGetUtentiInRuolo(Ruolo ruolo, InfoUtente infoUtente, bool appartenente)
        {
            return (InfoUtente[])this.Invoke("amministrazioneGetUtentiInRuolo", new object[3]
      {
        (object) ruolo,
        (object) infoUtente,
        (object) appartenente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetUtentiInRuolo(Ruolo ruolo, InfoUtente infoUtente, bool appartenente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetUtentiInRuolo", new object[3]
      {
        (object) ruolo,
        (object) infoUtente,
        (object) appartenente
      }, callback, asyncState);
        }

        public InfoUtente[] EndamministrazioneGetUtentiInRuolo(IAsyncResult asyncResult)
        {
            return (InfoUtente[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetUtentiInRuoloAsync(Ruolo ruolo, InfoUtente infoUtente, bool appartenente)
        {
            this.amministrazioneGetUtentiInRuoloAsync(ruolo, infoUtente, appartenente, (object)null);
        }

        public void amministrazioneGetUtentiInRuoloAsync(Ruolo ruolo, InfoUtente infoUtente, bool appartenente, object userState)
        {
            if (this.amministrazioneGetUtentiInRuoloOperationCompleted == null)
                this.amministrazioneGetUtentiInRuoloOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetUtentiInRuoloOperationCompleted);
            this.InvokeAsync("amministrazioneGetUtentiInRuolo", new object[3]
      {
        (object) ruolo,
        (object) infoUtente,
        (object) appartenente
      }, this.amministrazioneGetUtentiInRuoloOperationCompleted, userState);
        }

        private void OnamministrazioneGetUtentiInRuoloOperationCompleted(object arg)
        {
            if (this.amministrazioneGetUtentiInRuoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetUtentiInRuoloCompleted((object)this, new amministrazioneGetUtentiInRuoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/MDDSelectDocumentoInMDD", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DataSet MDDSelectDocumentoInMDD(string idDelibera, string[] arrCampi, InfoUtente infoUtente)
        {
            return (DataSet)this.Invoke("MDDSelectDocumentoInMDD", new object[3]
      {
        (object) idDelibera,
        (object) arrCampi,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginMDDSelectDocumentoInMDD(string idDelibera, string[] arrCampi, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("MDDSelectDocumentoInMDD", new object[3]
      {
        (object) idDelibera,
        (object) arrCampi,
        (object) infoUtente
      }, callback, asyncState);
        }

        public DataSet EndMDDSelectDocumentoInMDD(IAsyncResult asyncResult)
        {
            return (DataSet)this.EndInvoke(asyncResult)[0];
        }

        public void MDDSelectDocumentoInMDDAsync(string idDelibera, string[] arrCampi, InfoUtente infoUtente)
        {
            this.MDDSelectDocumentoInMDDAsync(idDelibera, arrCampi, infoUtente, (object)null);
        }

        public void MDDSelectDocumentoInMDDAsync(string idDelibera, string[] arrCampi, InfoUtente infoUtente, object userState)
        {
            if (this.MDDSelectDocumentoInMDDOperationCompleted == null)
                this.MDDSelectDocumentoInMDDOperationCompleted = new SendOrPostCallback(this.OnMDDSelectDocumentoInMDDOperationCompleted);
            this.InvokeAsync("MDDSelectDocumentoInMDD", new object[3]
      {
        (object) idDelibera,
        (object) arrCampi,
        (object) infoUtente
      }, this.MDDSelectDocumentoInMDDOperationCompleted, userState);
        }

        private void OnMDDSelectDocumentoInMDDOperationCompleted(object arg)
        {
            if (this.MDDSelectDocumentoInMDDCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.MDDSelectDocumentoInMDDCompleted((object)this, new MDDSelectDocumentoInMDDCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/MDDUpdateDocumentoInMDD", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void MDDUpdateDocumentoInMDD(string idDelibera, string[] arrCampi, string[] arrValoreCampi, InfoUtente infoUtente)
        {
            this.Invoke("MDDUpdateDocumentoInMDD", new object[4]
      {
        (object) idDelibera,
        (object) arrCampi,
        (object) arrValoreCampi,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginMDDUpdateDocumentoInMDD(string idDelibera, string[] arrCampi, string[] arrValoreCampi, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("MDDUpdateDocumentoInMDD", new object[4]
      {
        (object) idDelibera,
        (object) arrCampi,
        (object) arrValoreCampi,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndMDDUpdateDocumentoInMDD(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void MDDUpdateDocumentoInMDDAsync(string idDelibera, string[] arrCampi, string[] arrValoreCampi, InfoUtente infoUtente)
        {
            this.MDDUpdateDocumentoInMDDAsync(idDelibera, arrCampi, arrValoreCampi, infoUtente, (object)null);
        }

        public void MDDUpdateDocumentoInMDDAsync(string idDelibera, string[] arrCampi, string[] arrValoreCampi, InfoUtente infoUtente, object userState)
        {
            if (this.MDDUpdateDocumentoInMDDOperationCompleted == null)
                this.MDDUpdateDocumentoInMDDOperationCompleted = new SendOrPostCallback(this.OnMDDUpdateDocumentoInMDDOperationCompleted);
            this.InvokeAsync("MDDUpdateDocumentoInMDD", new object[4]
      {
        (object) idDelibera,
        (object) arrCampi,
        (object) arrValoreCampi,
        (object) infoUtente
      }, this.MDDUpdateDocumentoInMDDOperationCompleted, userState);
        }

        private void OnMDDUpdateDocumentoInMDDOperationCompleted(object arg)
        {
            if (this.MDDUpdateDocumentoInMDDCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.MDDUpdateDocumentoInMDDCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/MDDInsertDocumentoInMDD", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void MDDInsertDocumentoInMDD(string idDelibera, string[] arrCampi, string[] arrValoreCampi, InfoUtente infoUtente)
        {
            this.Invoke("MDDInsertDocumentoInMDD", new object[4]
      {
        (object) idDelibera,
        (object) arrCampi,
        (object) arrValoreCampi,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginMDDInsertDocumentoInMDD(string idDelibera, string[] arrCampi, string[] arrValoreCampi, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("MDDInsertDocumentoInMDD", new object[4]
      {
        (object) idDelibera,
        (object) arrCampi,
        (object) arrValoreCampi,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndMDDInsertDocumentoInMDD(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void MDDInsertDocumentoInMDDAsync(string idDelibera, string[] arrCampi, string[] arrValoreCampi, InfoUtente infoUtente)
        {
            this.MDDInsertDocumentoInMDDAsync(idDelibera, arrCampi, arrValoreCampi, infoUtente, (object)null);
        }

        public void MDDInsertDocumentoInMDDAsync(string idDelibera, string[] arrCampi, string[] arrValoreCampi, InfoUtente infoUtente, object userState)
        {
            if (this.MDDInsertDocumentoInMDDOperationCompleted == null)
                this.MDDInsertDocumentoInMDDOperationCompleted = new SendOrPostCallback(this.OnMDDInsertDocumentoInMDDOperationCompleted);
            this.InvokeAsync("MDDInsertDocumentoInMDD", new object[4]
      {
        (object) idDelibera,
        (object) arrCampi,
        (object) arrValoreCampi,
        (object) infoUtente
      }, this.MDDInsertDocumentoInMDDOperationCompleted, userState);
        }

        private void OnMDDInsertDocumentoInMDDOperationCompleted(object arg)
        {
            if (this.MDDInsertDocumentoInMDDCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.MDDInsertDocumentoInMDDCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/MDDGetCampiTipoAtto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string[] MDDGetCampiTipoAtto(string idTipoAtto, InfoUtente infoUtente)
        {
            return (string[])this.Invoke("MDDGetCampiTipoAtto", new object[2]
      {
        (object) idTipoAtto,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginMDDGetCampiTipoAtto(string idTipoAtto, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("MDDGetCampiTipoAtto", new object[2]
      {
        (object) idTipoAtto,
        (object) infoUtente
      }, callback, asyncState);
        }

        public string[] EndMDDGetCampiTipoAtto(IAsyncResult asyncResult)
        {
            return (string[])this.EndInvoke(asyncResult)[0];
        }

        public void MDDGetCampiTipoAttoAsync(string idTipoAtto, InfoUtente infoUtente)
        {
            this.MDDGetCampiTipoAttoAsync(idTipoAtto, infoUtente, (object)null);
        }

        public void MDDGetCampiTipoAttoAsync(string idTipoAtto, InfoUtente infoUtente, object userState)
        {
            if (this.MDDGetCampiTipoAttoOperationCompleted == null)
                this.MDDGetCampiTipoAttoOperationCompleted = new SendOrPostCallback(this.OnMDDGetCampiTipoAttoOperationCompleted);
            this.InvokeAsync("MDDGetCampiTipoAtto", new object[2]
      {
        (object) idTipoAtto,
        (object) infoUtente
      }, this.MDDGetCampiTipoAttoOperationCompleted, userState);
        }

        private void OnMDDGetCampiTipoAttoOperationCompleted(object arg)
        {
            if (this.MDDGetCampiTipoAttoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.MDDGetCampiTipoAttoCompleted((object)this, new MDDGetCampiTipoAttoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneUpdateTipologiaAtto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void amministrazioneUpdateTipologiaAtto(TipologiaAtto tipoAtto, InfoUtente infoUtente)
        {
            this.Invoke("amministrazioneUpdateTipologiaAtto", new object[2]
      {
        (object) tipoAtto,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginamministrazioneUpdateTipologiaAtto(TipologiaAtto tipoAtto, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneUpdateTipologiaAtto", new object[2]
      {
        (object) tipoAtto,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndamministrazioneUpdateTipologiaAtto(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void amministrazioneUpdateTipologiaAttoAsync(TipologiaAtto tipoAtto, InfoUtente infoUtente)
        {
            this.amministrazioneUpdateTipologiaAttoAsync(tipoAtto, infoUtente, (object)null);
        }

        public void amministrazioneUpdateTipologiaAttoAsync(TipologiaAtto tipoAtto, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneUpdateTipologiaAttoOperationCompleted == null)
                this.amministrazioneUpdateTipologiaAttoOperationCompleted = new SendOrPostCallback(this.OnamministrazioneUpdateTipologiaAttoOperationCompleted);
            this.InvokeAsync("amministrazioneUpdateTipologiaAtto", new object[2]
      {
        (object) tipoAtto,
        (object) infoUtente
      }, this.amministrazioneUpdateTipologiaAttoOperationCompleted, userState);
        }

        private void OnamministrazioneUpdateTipologiaAttoOperationCompleted(object arg)
        {
            if (this.amministrazioneUpdateTipologiaAttoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneUpdateTipologiaAttoCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneInsertTipologiaAtto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TipologiaAtto amministrazioneInsertTipologiaAtto(TipologiaAtto tipoAtto, InfoUtente infoUtente)
        {
            return (TipologiaAtto)this.Invoke("amministrazioneInsertTipologiaAtto", new object[2]
      {
        (object) tipoAtto,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneInsertTipologiaAtto(TipologiaAtto tipoAtto, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneInsertTipologiaAtto", new object[2]
      {
        (object) tipoAtto,
        (object) infoUtente
      }, callback, asyncState);
        }

        public TipologiaAtto EndamministrazioneInsertTipologiaAtto(IAsyncResult asyncResult)
        {
            return (TipologiaAtto)this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneInsertTipologiaAttoAsync(TipologiaAtto tipoAtto, InfoUtente infoUtente)
        {
            this.amministrazioneInsertTipologiaAttoAsync(tipoAtto, infoUtente, (object)null);
        }

        public void amministrazioneInsertTipologiaAttoAsync(TipologiaAtto tipoAtto, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneInsertTipologiaAttoOperationCompleted == null)
                this.amministrazioneInsertTipologiaAttoOperationCompleted = new SendOrPostCallback(this.OnamministrazioneInsertTipologiaAttoOperationCompleted);
            this.InvokeAsync("amministrazioneInsertTipologiaAtto", new object[2]
      {
        (object) tipoAtto,
        (object) infoUtente
      }, this.amministrazioneInsertTipologiaAttoOperationCompleted, userState);
        }

        private void OnamministrazioneInsertTipologiaAttoOperationCompleted(object arg)
        {
            if (this.amministrazioneInsertTipologiaAttoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneInsertTipologiaAttoCompleted((object)this, new amministrazioneInsertTipologiaAttoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneInsertCorrispondente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente amministrazioneInsertCorrispondente(Corrispondente corrispondente, Login login, InfoUtente infoUtente)
        {
            return (Corrispondente)this.Invoke("amministrazioneInsertCorrispondente", new object[3]
      {
        (object) corrispondente,
        (object) login,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneInsertCorrispondente(Corrispondente corrispondente, Login login, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneInsertCorrispondente", new object[3]
      {
        (object) corrispondente,
        (object) login,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Corrispondente EndamministrazioneInsertCorrispondente(IAsyncResult asyncResult)
        {
            return (Corrispondente)this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneInsertCorrispondenteAsync(Corrispondente corrispondente, Login login, InfoUtente infoUtente)
        {
            this.amministrazioneInsertCorrispondenteAsync(corrispondente, login, infoUtente, (object)null);
        }

        public void amministrazioneInsertCorrispondenteAsync(Corrispondente corrispondente, Login login, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneInsertCorrispondenteOperationCompleted == null)
                this.amministrazioneInsertCorrispondenteOperationCompleted = new SendOrPostCallback(this.OnamministrazioneInsertCorrispondenteOperationCompleted);
            this.InvokeAsync("amministrazioneInsertCorrispondente", new object[3]
      {
        (object) corrispondente,
        (object) login,
        (object) infoUtente
      }, this.amministrazioneInsertCorrispondenteOperationCompleted, userState);
        }

        private void OnamministrazioneInsertCorrispondenteOperationCompleted(object arg)
        {
            if (this.amministrazioneInsertCorrispondenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneInsertCorrispondenteCompleted((object)this, new amministrazioneInsertCorrispondenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetTipiFunzione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TipiFunzione[] amministrazioneGetTipiFunzione(bool daVis, InfoUtente infoUtente)
        {
            return (TipiFunzione[])this.Invoke("amministrazioneGetTipiFunzione", new object[2]
      {
        (object) daVis,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetTipiFunzione(bool daVis, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetTipiFunzione", new object[2]
      {
        (object) daVis,
        (object) infoUtente
      }, callback, asyncState);
        }

        public TipiFunzione[] EndamministrazioneGetTipiFunzione(IAsyncResult asyncResult)
        {
            return (TipiFunzione[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetTipiFunzioneAsync(bool daVis, InfoUtente infoUtente)
        {
            this.amministrazioneGetTipiFunzioneAsync(daVis, infoUtente, (object)null);
        }

        public void amministrazioneGetTipiFunzioneAsync(bool daVis, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetTipiFunzioneOperationCompleted == null)
                this.amministrazioneGetTipiFunzioneOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetTipiFunzioneOperationCompleted);
            this.InvokeAsync("amministrazioneGetTipiFunzione", new object[2]
      {
        (object) daVis,
        (object) infoUtente
      }, this.amministrazioneGetTipiFunzioneOperationCompleted, userState);
        }

        private void OnamministrazioneGetTipiFunzioneOperationCompleted(object arg)
        {
            if (this.amministrazioneGetTipiFunzioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetTipiFunzioneCompleted((object)this, new amministrazioneGetTipiFunzioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetCanali", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Canale[] amministrazioneGetCanali(InfoUtente infoUtente)
        {
            return (Canale[])this.Invoke("amministrazioneGetCanali", new object[1]
      {
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetCanali(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetCanali", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public Canale[] EndamministrazioneGetCanali(IAsyncResult asyncResult)
        {
            return (Canale[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetCanaliAsync(InfoUtente infoUtente)
        {
            this.amministrazioneGetCanaliAsync(infoUtente, (object)null);
        }

        public void amministrazioneGetCanaliAsync(InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetCanaliOperationCompleted == null)
                this.amministrazioneGetCanaliOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetCanaliOperationCompleted);
            this.InvokeAsync("amministrazioneGetCanali", new object[1]
      {
        (object) infoUtente
      }, this.amministrazioneGetCanaliOperationCompleted, userState);
        }

        private void OnamministrazioneGetCanaliOperationCompleted(object arg)
        {
            if (this.amministrazioneGetCanaliCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetCanaliCompleted((object)this, new amministrazioneGetCanaliCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetTipoRuoli", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TipoRuolo[] amministrazioneGetTipoRuoli(string idAmm, InfoUtente infoUtente)
        {
            return (TipoRuolo[])this.Invoke("amministrazioneGetTipoRuoli", new object[2]
      {
        (object) idAmm,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetTipoRuoli(string idAmm, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetTipoRuoli", new object[2]
      {
        (object) idAmm,
        (object) infoUtente
      }, callback, asyncState);
        }

        public TipoRuolo[] EndamministrazioneGetTipoRuoli(IAsyncResult asyncResult)
        {
            return (TipoRuolo[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetTipoRuoliAsync(string idAmm, InfoUtente infoUtente)
        {
            this.amministrazioneGetTipoRuoliAsync(idAmm, infoUtente, (object)null);
        }

        public void amministrazioneGetTipoRuoliAsync(string idAmm, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetTipoRuoliOperationCompleted == null)
                this.amministrazioneGetTipoRuoliOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetTipoRuoliOperationCompleted);
            this.InvokeAsync("amministrazioneGetTipoRuoli", new object[2]
      {
        (object) idAmm,
        (object) infoUtente
      }, this.amministrazioneGetTipoRuoliOperationCompleted, userState);
        }

        private void OnamministrazioneGetTipoRuoliOperationCompleted(object arg)
        {
            if (this.amministrazioneGetTipoRuoliCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetTipoRuoliCompleted((object)this, new amministrazioneGetTipoRuoliCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetRegistri", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Registro[] amministrazioneGetRegistri(string idAmm, InfoUtente infoUtente)
        {
            return (Registro[])this.Invoke("amministrazioneGetRegistri", new object[2]
      {
        (object) idAmm,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetRegistri(string idAmm, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetRegistri", new object[2]
      {
        (object) idAmm,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Registro[] EndamministrazioneGetRegistri(IAsyncResult asyncResult)
        {
            return (Registro[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetRegistriAsync(string idAmm, InfoUtente infoUtente)
        {
            this.amministrazioneGetRegistriAsync(idAmm, infoUtente, (object)null);
        }

        public void amministrazioneGetRegistriAsync(string idAmm, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetRegistriOperationCompleted == null)
                this.amministrazioneGetRegistriOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetRegistriOperationCompleted);
            this.InvokeAsync("amministrazioneGetRegistri", new object[2]
      {
        (object) idAmm,
        (object) infoUtente
      }, this.amministrazioneGetRegistriOperationCompleted, userState);
        }

        private void OnamministrazioneGetRegistriOperationCompleted(object arg)
        {
            if (this.amministrazioneGetRegistriCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetRegistriCompleted((object)this, new amministrazioneGetRegistriCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetAmministrazioni", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Amministrazione[] amministrazioneGetAmministrazioni(InfoUtente infoUtente)
        {
            return (Amministrazione[])this.Invoke("amministrazioneGetAmministrazioni", new object[1]
      {
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetAmministrazioni(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetAmministrazioni", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public Amministrazione[] EndamministrazioneGetAmministrazioni(IAsyncResult asyncResult)
        {
            return (Amministrazione[])this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetAmministrazioniAsync(InfoUtente infoUtente)
        {
            this.amministrazioneGetAmministrazioniAsync(infoUtente, (object)null);
        }

        public void amministrazioneGetAmministrazioniAsync(InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetAmministrazioniOperationCompleted == null)
                this.amministrazioneGetAmministrazioniOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetAmministrazioniOperationCompleted);
            this.InvokeAsync("amministrazioneGetAmministrazioni", new object[1]
      {
        (object) infoUtente
      }, this.amministrazioneGetAmministrazioniOperationCompleted, userState);
        }

        private void OnamministrazioneGetAmministrazioniOperationCompleted(object arg)
        {
            if (this.amministrazioneGetAmministrazioniCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetAmministrazioniCompleted((object)this, new amministrazioneGetAmministrazioniCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/amministrazioneGetParametroConfigurazione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Configurazione amministrazioneGetParametroConfigurazione(string parametro, InfoUtente infoUtente)
        {
            return (Configurazione)this.Invoke("amministrazioneGetParametroConfigurazione", new object[2]
      {
        (object) parametro,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginamministrazioneGetParametroConfigurazione(string parametro, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("amministrazioneGetParametroConfigurazione", new object[2]
      {
        (object) parametro,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Configurazione EndamministrazioneGetParametroConfigurazione(IAsyncResult asyncResult)
        {
            return (Configurazione)this.EndInvoke(asyncResult)[0];
        }

        public void amministrazioneGetParametroConfigurazioneAsync(string parametro, InfoUtente infoUtente)
        {
            this.amministrazioneGetParametroConfigurazioneAsync(parametro, infoUtente, (object)null);
        }

        public void amministrazioneGetParametroConfigurazioneAsync(string parametro, InfoUtente infoUtente, object userState)
        {
            if (this.amministrazioneGetParametroConfigurazioneOperationCompleted == null)
                this.amministrazioneGetParametroConfigurazioneOperationCompleted = new SendOrPostCallback(this.OnamministrazioneGetParametroConfigurazioneOperationCompleted);
            this.InvokeAsync("amministrazioneGetParametroConfigurazione", new object[2]
      {
        (object) parametro,
        (object) infoUtente
      }, this.amministrazioneGetParametroConfigurazioneOperationCompleted, userState);
        }

        private void OnamministrazioneGetParametroConfigurazioneOperationCompleted(object arg)
        {
            if (this.amministrazioneGetParametroConfigurazioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.amministrazioneGetParametroConfigurazioneCompleted((object)this, new amministrazioneGetParametroConfigurazioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/registriStampa", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public StampaRegistroResult registriStampa(InfoUtente infoUtente, Ruolo ruolo, Registro registro)
        {
            return (StampaRegistroResult)this.Invoke("registriStampa", new object[3]
      {
        (object) infoUtente,
        (object) ruolo,
        (object) registro
      })[0];
        }

        public IAsyncResult BeginregistriStampa(InfoUtente infoUtente, Ruolo ruolo, Registro registro, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("registriStampa", new object[3]
      {
        (object) infoUtente,
        (object) ruolo,
        (object) registro
      }, callback, asyncState);
        }

        public StampaRegistroResult EndregistriStampa(IAsyncResult asyncResult)
        {
            return (StampaRegistroResult)this.EndInvoke(asyncResult)[0];
        }

        public void registriStampaAsync(InfoUtente infoUtente, Ruolo ruolo, Registro registro)
        {
            this.registriStampaAsync(infoUtente, ruolo, registro, (object)null);
        }

        public void registriStampaAsync(InfoUtente infoUtente, Ruolo ruolo, Registro registro, object userState)
        {
            if (this.registriStampaOperationCompleted == null)
                this.registriStampaOperationCompleted = new SendOrPostCallback(this.OnregistriStampaOperationCompleted);
            this.InvokeAsync("registriStampa", new object[3]
      {
        (object) infoUtente,
        (object) ruolo,
        (object) registro
      }, this.registriStampaOperationCompleted, userState);
        }

        private void OnregistriStampaOperationCompleted(object arg)
        {
            if (this.registriStampaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.registriStampaCompleted((object)this, new registriStampaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/registriModifica", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Registro registriModifica(Registro registro, InfoUtente infoUtente)
        {
            return (Registro)this.Invoke("registriModifica", new object[2]
      {
        (object) registro,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginregistriModifica(Registro registro, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("registriModifica", new object[2]
      {
        (object) registro,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Registro EndregistriModifica(IAsyncResult asyncResult)
        {
            return (Registro)this.EndInvoke(asyncResult)[0];
        }

        public void registriModificaAsync(Registro registro, InfoUtente infoUtente)
        {
            this.registriModificaAsync(registro, infoUtente, (object)null);
        }

        public void registriModificaAsync(Registro registro, InfoUtente infoUtente, object userState)
        {
            if (this.registriModificaOperationCompleted == null)
                this.registriModificaOperationCompleted = new SendOrPostCallback(this.OnregistriModificaOperationCompleted);
            this.InvokeAsync("registriModifica", new object[2]
      {
        (object) registro,
        (object) infoUtente
      }, this.registriModificaOperationCompleted, userState);
        }

        private void OnregistriModificaOperationCompleted(object arg)
        {
            if (this.registriModificaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.registriModificaCompleted((object)this, new registriModificaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/registriCambiaStato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Registro registriCambiaStato(Registro registro, InfoUtente infoUtente)
        {
            return (Registro)this.Invoke("registriCambiaStato", new object[2]
      {
        (object) registro,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginregistriCambiaStato(Registro registro, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("registriCambiaStato", new object[2]
      {
        (object) registro,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Registro EndregistriCambiaStato(IAsyncResult asyncResult)
        {
            return (Registro)this.EndInvoke(asyncResult)[0];
        }

        public void registriCambiaStatoAsync(Registro registro, InfoUtente infoUtente)
        {
            this.registriCambiaStatoAsync(registro, infoUtente, (object)null);
        }

        public void registriCambiaStatoAsync(Registro registro, InfoUtente infoUtente, object userState)
        {
            if (this.registriCambiaStatoOperationCompleted == null)
                this.registriCambiaStatoOperationCompleted = new SendOrPostCallback(this.OnregistriCambiaStatoOperationCompleted);
            this.InvokeAsync("registriCambiaStato", new object[2]
      {
        (object) registro,
        (object) infoUtente
      }, this.registriCambiaStatoOperationCompleted, userState);
        }

        private void OnregistriCambiaStatoOperationCompleted(object arg)
        {
            if (this.registriCambiaStatoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.registriCambiaStatoCompleted((object)this, new registriCambiaStatoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/interoperabilitaAggiornamentoConferma", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public ProtocolloDestinatario[] interoperabilitaAggiornamentoConferma(InfoDocumento infoDocumento, Corrispondente corrispondente, InfoUtente infoUtente)
        {
            return (ProtocolloDestinatario[])this.Invoke("interoperabilitaAggiornamentoConferma", new object[3]
      {
        (object) infoDocumento,
        (object) corrispondente,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegininteroperabilitaAggiornamentoConferma(InfoDocumento infoDocumento, Corrispondente corrispondente, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("interoperabilitaAggiornamentoConferma", new object[3]
      {
        (object) infoDocumento,
        (object) corrispondente,
        (object) infoUtente
      }, callback, asyncState);
        }

        public ProtocolloDestinatario[] EndinteroperabilitaAggiornamentoConferma(IAsyncResult asyncResult)
        {
            return (ProtocolloDestinatario[])this.EndInvoke(asyncResult)[0];
        }

        public void interoperabilitaAggiornamentoConfermaAsync(InfoDocumento infoDocumento, Corrispondente corrispondente, InfoUtente infoUtente)
        {
            this.interoperabilitaAggiornamentoConfermaAsync(infoDocumento, corrispondente, infoUtente, (object)null);
        }

        public void interoperabilitaAggiornamentoConfermaAsync(InfoDocumento infoDocumento, Corrispondente corrispondente, InfoUtente infoUtente, object userState)
        {
            if (this.interoperabilitaAggiornamentoConfermaOperationCompleted == null)
                this.interoperabilitaAggiornamentoConfermaOperationCompleted = new SendOrPostCallback(this.OninteroperabilitaAggiornamentoConfermaOperationCompleted);
            this.InvokeAsync("interoperabilitaAggiornamentoConferma", new object[3]
      {
        (object) infoDocumento,
        (object) corrispondente,
        (object) infoUtente
      }, this.interoperabilitaAggiornamentoConfermaOperationCompleted, userState);
        }

        private void OninteroperabilitaAggiornamentoConfermaOperationCompleted(object arg)
        {
            if (this.interoperabilitaAggiornamentoConfermaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.interoperabilitaAggiornamentoConfermaCompleted((object)this, new interoperabilitaAggiornamentoConfermaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/interoperabilitaInvioRicevuta", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void interoperabilitaInvioRicevuta(string idProfile, Registro reg, InfoUtente infoUtente)
        {
            this.Invoke("interoperabilitaInvioRicevuta", new object[3]
      {
        (object) idProfile,
        (object) reg,
        (object) infoUtente
      });
        }

        public IAsyncResult BegininteroperabilitaInvioRicevuta(string idProfile, Registro reg, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("interoperabilitaInvioRicevuta", new object[3]
      {
        (object) idProfile,
        (object) reg,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndinteroperabilitaInvioRicevuta(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void interoperabilitaInvioRicevutaAsync(string idProfile, Registro reg, InfoUtente infoUtente)
        {
            this.interoperabilitaInvioRicevutaAsync(idProfile, reg, infoUtente, (object)null);
        }

        public void interoperabilitaInvioRicevutaAsync(string idProfile, Registro reg, InfoUtente infoUtente, object userState)
        {
            if (this.interoperabilitaInvioRicevutaOperationCompleted == null)
                this.interoperabilitaInvioRicevutaOperationCompleted = new SendOrPostCallback(this.OninteroperabilitaInvioRicevutaOperationCompleted);
            this.InvokeAsync("interoperabilitaInvioRicevuta", new object[3]
      {
        (object) idProfile,
        (object) reg,
        (object) infoUtente
      }, this.interoperabilitaInvioRicevutaOperationCompleted, userState);
        }

        private void OninteroperabilitaInvioRicevutaOperationCompleted(object arg)
        {
            if (this.interoperabilitaInvioRicevutaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.interoperabilitaInvioRicevutaCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/interoperabilitaInvio", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void interoperabilitaInvio(InfoUtente infoUtente, Corrispondente mittSegnatura, Registro reg, SchedaDocumento schedaDoc, bool confermaRic)
        {
            this.Invoke("interoperabilitaInvio", new object[5]
      {
        (object) infoUtente,
        (object) mittSegnatura,
        (object) reg,
        (object) schedaDoc,
        (object) confermaRic
      });
        }

        public IAsyncResult BegininteroperabilitaInvio(InfoUtente infoUtente, Corrispondente mittSegnatura, Registro reg, SchedaDocumento schedaDoc, bool confermaRic, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("interoperabilitaInvio", new object[5]
      {
        (object) infoUtente,
        (object) mittSegnatura,
        (object) reg,
        (object) schedaDoc,
        (object) confermaRic
      }, callback, asyncState);
        }

        public void EndinteroperabilitaInvio(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void interoperabilitaInvioAsync(InfoUtente infoUtente, Corrispondente mittSegnatura, Registro reg, SchedaDocumento schedaDoc, bool confermaRic)
        {
            this.interoperabilitaInvioAsync(infoUtente, mittSegnatura, reg, schedaDoc, confermaRic, (object)null);
        }

        public void interoperabilitaInvioAsync(InfoUtente infoUtente, Corrispondente mittSegnatura, Registro reg, SchedaDocumento schedaDoc, bool confermaRic, object userState)
        {
            if (this.interoperabilitaInvioOperationCompleted == null)
                this.interoperabilitaInvioOperationCompleted = new SendOrPostCallback(this.OninteroperabilitaInvioOperationCompleted);
            this.InvokeAsync("interoperabilitaInvio", new object[5]
      {
        (object) infoUtente,
        (object) mittSegnatura,
        (object) reg,
        (object) schedaDoc,
        (object) confermaRic
      }, this.interoperabilitaInvioOperationCompleted, userState);
        }

        private void OninteroperabilitaInvioOperationCompleted(object arg)
        {
            if (this.interoperabilitaInvioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.interoperabilitaInvioCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/interoperabilitaRicezione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void interoperabilitaRicezione(string serverName, Registro reg, InfoUtente infoUtente)
        {
            this.Invoke("interoperabilitaRicezione", new object[3]
      {
        (object) serverName,
        (object) reg,
        (object) infoUtente
      });
        }

        public IAsyncResult BegininteroperabilitaRicezione(string serverName, Registro reg, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("interoperabilitaRicezione", new object[3]
      {
        (object) serverName,
        (object) reg,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndinteroperabilitaRicezione(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void interoperabilitaRicezioneAsync(string serverName, Registro reg, InfoUtente infoUtente)
        {
            this.interoperabilitaRicezioneAsync(serverName, reg, infoUtente, (object)null);
        }

        public void interoperabilitaRicezioneAsync(string serverName, Registro reg, InfoUtente infoUtente, object userState)
        {
            if (this.interoperabilitaRicezioneOperationCompleted == null)
                this.interoperabilitaRicezioneOperationCompleted = new SendOrPostCallback(this.OninteroperabilitaRicezioneOperationCompleted);
            this.InvokeAsync("interoperabilitaRicezione", new object[3]
      {
        (object) serverName,
        (object) reg,
        (object) infoUtente
      }, this.interoperabilitaRicezioneOperationCompleted, userState);
        }

        private void OninteroperabilitaRicezioneOperationCompleted(object arg)
        {
            if (this.interoperabilitaRicezioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.interoperabilitaRicezioneCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioneGetInRispostaA", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione trasmissioneGetInRispostaA(TrasmissioneSingola trasmSingola, InfoUtente infoUtente)
        {
            return (Trasmissione)this.Invoke("trasmissioneGetInRispostaA", new object[2]
      {
        (object) trasmSingola,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegintrasmissioneGetInRispostaA(TrasmissioneSingola trasmSingola, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneGetInRispostaA", new object[2]
      {
        (object) trasmSingola,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Trasmissione EndtrasmissioneGetInRispostaA(IAsyncResult asyncResult)
        {
            return (Trasmissione)this.EndInvoke(asyncResult)[0];
        }

        public void trasmissioneGetInRispostaAAsync(TrasmissioneSingola trasmSingola, InfoUtente infoUtente)
        {
            this.trasmissioneGetInRispostaAAsync(trasmSingola, infoUtente, (object)null);
        }

        public void trasmissioneGetInRispostaAAsync(TrasmissioneSingola trasmSingola, InfoUtente infoUtente, object userState)
        {
            if (this.trasmissioneGetInRispostaAOperationCompleted == null)
                this.trasmissioneGetInRispostaAOperationCompleted = new SendOrPostCallback(this.OntrasmissioneGetInRispostaAOperationCompleted);
            this.InvokeAsync("trasmissioneGetInRispostaA", new object[2]
      {
        (object) trasmSingola,
        (object) infoUtente
      }, this.trasmissioneGetInRispostaAOperationCompleted, userState);
        }

        private void OntrasmissioneGetInRispostaAOperationCompleted(object arg)
        {
            if (this.trasmissioneGetInRispostaACompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneGetInRispostaACompleted((object)this, new trasmissioneGetInRispostaACompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioneGetRisposta", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione trasmissioneGetRisposta(TrasmissioneUtente trasmUtente, InfoUtente infoUtente)
        {
            return (Trasmissione)this.Invoke("trasmissioneGetRisposta", new object[2]
      {
        (object) trasmUtente,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegintrasmissioneGetRisposta(TrasmissioneUtente trasmUtente, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneGetRisposta", new object[2]
      {
        (object) trasmUtente,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Trasmissione EndtrasmissioneGetRisposta(IAsyncResult asyncResult)
        {
            return (Trasmissione)this.EndInvoke(asyncResult)[0];
        }

        public void trasmissioneGetRispostaAsync(TrasmissioneUtente trasmUtente, InfoUtente infoUtente)
        {
            this.trasmissioneGetRispostaAsync(trasmUtente, infoUtente, (object)null);
        }

        public void trasmissioneGetRispostaAsync(TrasmissioneUtente trasmUtente, InfoUtente infoUtente, object userState)
        {
            if (this.trasmissioneGetRispostaOperationCompleted == null)
                this.trasmissioneGetRispostaOperationCompleted = new SendOrPostCallback(this.OntrasmissioneGetRispostaOperationCompleted);
            this.InvokeAsync("trasmissioneGetRisposta", new object[2]
      {
        (object) trasmUtente,
        (object) infoUtente
      }, this.trasmissioneGetRispostaOperationCompleted, userState);
        }

        private void OntrasmissioneGetRispostaOperationCompleted(object arg)
        {
            if (this.trasmissioneGetRispostaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneGetRispostaCompleted((object)this, new trasmissioneGetRispostaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
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

        [SoapDocumentMethod("http://localhost/trasmissioneExecuteAccRif", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void trasmissioneExecuteAccRif(TrasmissioneUtente trasmissioneUtente, InfoUtente infoUtente)
        {
            this.Invoke("trasmissioneExecuteAccRif", new object[2]
      {
        (object) trasmissioneUtente,
        (object) infoUtente
      });
        }

        public IAsyncResult BegintrasmissioneExecuteAccRif(TrasmissioneUtente trasmissioneUtente, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneExecuteAccRif", new object[2]
      {
        (object) trasmissioneUtente,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndtrasmissioneExecuteAccRif(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void trasmissioneExecuteAccRifAsync(TrasmissioneUtente trasmissioneUtente, InfoUtente infoUtente)
        {
            this.trasmissioneExecuteAccRifAsync(trasmissioneUtente, infoUtente, (object)null);
        }

        public void trasmissioneExecuteAccRifAsync(TrasmissioneUtente trasmissioneUtente, InfoUtente infoUtente, object userState)
        {
            if (this.trasmissioneExecuteAccRifOperationCompleted == null)
                this.trasmissioneExecuteAccRifOperationCompleted = new SendOrPostCallback(this.OntrasmissioneExecuteAccRifOperationCompleted);
            this.InvokeAsync("trasmissioneExecuteAccRif", new object[2]
      {
        (object) trasmissioneUtente,
        (object) infoUtente
      }, this.trasmissioneExecuteAccRifOperationCompleted, userState);
        }

        private void OntrasmissioneExecuteAccRifOperationCompleted(object arg)
        {
            if (this.trasmissioneExecuteAccRifCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneExecuteAccRifCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioneGetRagioni", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public RagioneTrasmissione[] trasmissioneGetRagioni(TrasmissioneDiritti diritti, InfoUtente infoUtente)
        {
            return (RagioneTrasmissione[])this.Invoke("trasmissioneGetRagioni", new object[2]
      {
        (object) diritti,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegintrasmissioneGetRagioni(TrasmissioneDiritti diritti, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneGetRagioni", new object[2]
      {
        (object) diritti,
        (object) infoUtente
      }, callback, asyncState);
        }

        public RagioneTrasmissione[] EndtrasmissioneGetRagioni(IAsyncResult asyncResult)
        {
            return (RagioneTrasmissione[])this.EndInvoke(asyncResult)[0];
        }

        public void trasmissioneGetRagioniAsync(TrasmissioneDiritti diritti, InfoUtente infoUtente)
        {
            this.trasmissioneGetRagioniAsync(diritti, infoUtente, (object)null);
        }

        public void trasmissioneGetRagioniAsync(TrasmissioneDiritti diritti, InfoUtente infoUtente, object userState)
        {
            if (this.trasmissioneGetRagioniOperationCompleted == null)
                this.trasmissioneGetRagioniOperationCompleted = new SendOrPostCallback(this.OntrasmissioneGetRagioniOperationCompleted);
            this.InvokeAsync("trasmissioneGetRagioni", new object[2]
      {
        (object) diritti,
        (object) infoUtente
      }, this.trasmissioneGetRagioniOperationCompleted, userState);
        }

        private void OntrasmissioneGetRagioniOperationCompleted(object arg)
        {
            if (this.trasmissioneGetRagioniCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneGetRagioniCompleted((object)this, new trasmissioneGetRagioniCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioneGetQueryEffettuate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione[] trasmissioneGetQueryEffettuate(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo)
        {
            return (Trasmissione[])this.Invoke("trasmissioneGetQueryEffettuate", new object[4]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo
      })[0];
        }

        public IAsyncResult BegintrasmissioneGetQueryEffettuate(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneGetQueryEffettuate", new object[4]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo
      }, callback, asyncState);
        }

        public Trasmissione[] EndtrasmissioneGetQueryEffettuate(IAsyncResult asyncResult)
        {
            return (Trasmissione[])this.EndInvoke(asyncResult)[0];
        }

        public void trasmissioneGetQueryEffettuateAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo)
        {
            this.trasmissioneGetQueryEffettuateAsync(oggettoTrasmesso, listaFiltri, utente, ruolo, (object)null);
        }

        public void trasmissioneGetQueryEffettuateAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, object userState)
        {
            if (this.trasmissioneGetQueryEffettuateOperationCompleted == null)
                this.trasmissioneGetQueryEffettuateOperationCompleted = new SendOrPostCallback(this.OntrasmissioneGetQueryEffettuateOperationCompleted);
            this.InvokeAsync("trasmissioneGetQueryEffettuate", new object[4]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo
      }, this.trasmissioneGetQueryEffettuateOperationCompleted, userState);
        }

        private void OntrasmissioneGetQueryEffettuateOperationCompleted(object arg)
        {
            if (this.trasmissioneGetQueryEffettuateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneGetQueryEffettuateCompleted((object)this, new trasmissioneGetQueryEffettuateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioneGetQueryRicevute", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione[] trasmissioneGetQueryRicevute(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo)
        {
            return (Trasmissione[])this.Invoke("trasmissioneGetQueryRicevute", new object[4]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo
      })[0];
        }

        public IAsyncResult BegintrasmissioneGetQueryRicevute(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneGetQueryRicevute", new object[4]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo
      }, callback, asyncState);
        }

        public Trasmissione[] EndtrasmissioneGetQueryRicevute(IAsyncResult asyncResult)
        {
            return (Trasmissione[])this.EndInvoke(asyncResult)[0];
        }

        public void trasmissioneGetQueryRicevuteAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo)
        {
            this.trasmissioneGetQueryRicevuteAsync(oggettoTrasmesso, listaFiltri, utente, ruolo, (object)null);
        }

        public void trasmissioneGetQueryRicevuteAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, object userState)
        {
            if (this.trasmissioneGetQueryRicevuteOperationCompleted == null)
                this.trasmissioneGetQueryRicevuteOperationCompleted = new SendOrPostCallback(this.OntrasmissioneGetQueryRicevuteOperationCompleted);
            this.InvokeAsync("trasmissioneGetQueryRicevute", new object[4]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo
      }, this.trasmissioneGetQueryRicevuteOperationCompleted, userState);
        }

        private void OntrasmissioneGetQueryRicevuteOperationCompleted(object arg)
        {
            if (this.trasmissioneGetQueryRicevuteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneGetQueryRicevuteCompleted((object)this, new trasmissioneGetQueryRicevuteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioneExecuteTrasm", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione trasmissioneExecuteTrasm(string path, Trasmissione trasmissione, InfoUtente infoUtente)
        {
            return (Trasmissione)this.Invoke("trasmissioneExecuteTrasm", new object[3]
      {
        (object) path,
        (object) trasmissione,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegintrasmissioneExecuteTrasm(string path, Trasmissione trasmissione, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneExecuteTrasm", new object[3]
      {
        (object) path,
        (object) trasmissione,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Trasmissione EndtrasmissioneExecuteTrasm(IAsyncResult asyncResult)
        {
            return (Trasmissione)this.EndInvoke(asyncResult)[0];
        }

        public void trasmissioneExecuteTrasmAsync(string path, Trasmissione trasmissione, InfoUtente infoUtente)
        {
            this.trasmissioneExecuteTrasmAsync(path, trasmissione, infoUtente, (object)null);
        }

        public void trasmissioneExecuteTrasmAsync(string path, Trasmissione trasmissione, InfoUtente infoUtente, object userState)
        {
            if (this.trasmissioneExecuteTrasmOperationCompleted == null)
                this.trasmissioneExecuteTrasmOperationCompleted = new SendOrPostCallback(this.OntrasmissioneExecuteTrasmOperationCompleted);
            this.InvokeAsync("trasmissioneExecuteTrasm", new object[3]
      {
        (object) path,
        (object) trasmissione,
        (object) infoUtente
      }, this.trasmissioneExecuteTrasmOperationCompleted, userState);
        }

        private void OntrasmissioneExecuteTrasmOperationCompleted(object arg)
        {
            if (this.trasmissioneExecuteTrasmCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneExecuteTrasmCompleted((object)this, new trasmissioneExecuteTrasmCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioneSaveTrasm", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione trasmissioneSaveTrasm(Trasmissione trasmissione, InfoUtente infoUtente)
        {
            return (Trasmissione)this.Invoke("trasmissioneSaveTrasm", new object[2]
      {
        (object) trasmissione,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegintrasmissioneSaveTrasm(Trasmissione trasmissione, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneSaveTrasm", new object[2]
      {
        (object) trasmissione,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Trasmissione EndtrasmissioneSaveTrasm(IAsyncResult asyncResult)
        {
            return (Trasmissione)this.EndInvoke(asyncResult)[0];
        }

        public void trasmissioneSaveTrasmAsync(Trasmissione trasmissione, InfoUtente infoUtente)
        {
            this.trasmissioneSaveTrasmAsync(trasmissione, infoUtente, (object)null);
        }

        public void trasmissioneSaveTrasmAsync(Trasmissione trasmissione, InfoUtente infoUtente, object userState)
        {
            if (this.trasmissioneSaveTrasmOperationCompleted == null)
                this.trasmissioneSaveTrasmOperationCompleted = new SendOrPostCallback(this.OntrasmissioneSaveTrasmOperationCompleted);
            this.InvokeAsync("trasmissioneSaveTrasm", new object[2]
      {
        (object) trasmissione,
        (object) infoUtente
      }, this.trasmissioneSaveTrasmOperationCompleted, userState);
        }

        private void OntrasmissioneSaveTrasmOperationCompleted(object arg)
        {
            if (this.trasmissioneSaveTrasmCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneSaveTrasmCompleted((object)this, new trasmissioneSaveTrasmCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioneGetQueryEffettuatePaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione[] trasmissioneGetQueryEffettuatePaging(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, ref int risultati, int pagina, int righe)
        {
            object[] objArray = this.Invoke("trasmissioneGetQueryEffettuatePaging", new object[7]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) risultati,
        (object) pagina,
        (object) righe
      });
            risultati = (int)objArray[1];
            return (Trasmissione[])objArray[0];
        }

        public IAsyncResult BegintrasmissioneGetQueryEffettuatePaging(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int risultati, int pagina, int righe, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneGetQueryEffettuatePaging", new object[7]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) risultati,
        (object) pagina,
        (object) righe
      }, callback, asyncState);
        }

        public Trasmissione[] EndtrasmissioneGetQueryEffettuatePaging(IAsyncResult asyncResult, out int risultati)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            risultati = (int)objArray[1];
            return (Trasmissione[])objArray[0];
        }

        public void trasmissioneGetQueryEffettuatePagingAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int risultati, int pagina, int righe)
        {
            this.trasmissioneGetQueryEffettuatePagingAsync(oggettoTrasmesso, listaFiltri, utente, ruolo, risultati, pagina, righe, (object)null);
        }

        public void trasmissioneGetQueryEffettuatePagingAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int risultati, int pagina, int righe, object userState)
        {
            if (this.trasmissioneGetQueryEffettuatePagingOperationCompleted == null)
                this.trasmissioneGetQueryEffettuatePagingOperationCompleted = new SendOrPostCallback(this.OntrasmissioneGetQueryEffettuatePagingOperationCompleted);
            this.InvokeAsync("trasmissioneGetQueryEffettuatePaging", new object[7]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) risultati,
        (object) pagina,
        (object) righe
      }, this.trasmissioneGetQueryEffettuatePagingOperationCompleted, userState);
        }

        private void OntrasmissioneGetQueryEffettuatePagingOperationCompleted(object arg)
        {
            if (this.trasmissioneGetQueryEffettuatePagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneGetQueryEffettuatePagingCompleted((object)this, new trasmissioneGetQueryEffettuatePagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioneGetQueryRicevutePaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Trasmissione[] trasmissioneGetQueryRicevutePaging(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, ref int risultati, int pagina, int righe)
        {
            object[] objArray = this.Invoke("trasmissioneGetQueryRicevutePaging", new object[7]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) risultati,
        (object) pagina,
        (object) righe
      });
            risultati = (int)objArray[1];
            return (Trasmissione[])objArray[0];
        }

        public IAsyncResult BegintrasmissioneGetQueryRicevutePaging(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int risultati, int pagina, int righe, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneGetQueryRicevutePaging", new object[7]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) risultati,
        (object) pagina,
        (object) righe
      }, callback, asyncState);
        }

        public Trasmissione[] EndtrasmissioneGetQueryRicevutePaging(IAsyncResult asyncResult, out int risultati)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            risultati = (int)objArray[1];
            return (Trasmissione[])objArray[0];
        }

        public void trasmissioneGetQueryRicevutePagingAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int risultati, int pagina, int righe)
        {
            this.trasmissioneGetQueryRicevutePagingAsync(oggettoTrasmesso, listaFiltri, utente, ruolo, risultati, pagina, righe, (object)null);
        }

        public void trasmissioneGetQueryRicevutePagingAsync(TrasmissioneOggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int risultati, int pagina, int righe, object userState)
        {
            if (this.trasmissioneGetQueryRicevutePagingOperationCompleted == null)
                this.trasmissioneGetQueryRicevutePagingOperationCompleted = new SendOrPostCallback(this.OntrasmissioneGetQueryRicevutePagingOperationCompleted);
            this.InvokeAsync("trasmissioneGetQueryRicevutePaging", new object[7]
      {
        (object) oggettoTrasmesso,
        (object) listaFiltri,
        (object) utente,
        (object) ruolo,
        (object) risultati,
        (object) pagina,
        (object) righe
      }, this.trasmissioneGetQueryRicevutePagingOperationCompleted, userState);
        }

        private void OntrasmissioneGetQueryRicevutePagingOperationCompleted(object arg)
        {
            if (this.trasmissioneGetQueryRicevutePagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneGetQueryRicevutePagingCompleted((object)this, new trasmissioneGetQueryRicevutePagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneAddDocFolder", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void fascicolazioneAddDocFolder(string idProfile, string idFolder, InfoUtente infoUtente)
        {
            this.Invoke("fascicolazioneAddDocFolder", new object[3]
      {
        (object) idProfile,
        (object) idFolder,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginfascicolazioneAddDocFolder(string idProfile, string idFolder, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneAddDocFolder", new object[3]
      {
        (object) idProfile,
        (object) idFolder,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndfascicolazioneAddDocFolder(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void fascicolazioneAddDocFolderAsync(string idProfile, string idFolder, InfoUtente infoUtente)
        {
            this.fascicolazioneAddDocFolderAsync(idProfile, idFolder, infoUtente, (object)null);
        }

        public void fascicolazioneAddDocFolderAsync(string idProfile, string idFolder, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneAddDocFolderOperationCompleted == null)
                this.fascicolazioneAddDocFolderOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneAddDocFolderOperationCompleted);
            this.InvokeAsync("fascicolazioneAddDocFolder", new object[3]
      {
        (object) idProfile,
        (object) idFolder,
        (object) infoUtente
      }, this.fascicolazioneAddDocFolderOperationCompleted, userState);
        }

        private void OnfascicolazioneAddDocFolderOperationCompleted(object arg)
        {
            if (this.fascicolazioneAddDocFolderCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneAddDocFolderCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneDelFolder", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void fascicolazioneDelFolder(Folder folder, InfoUtente infoUtente)
        {
            this.Invoke("fascicolazioneDelFolder", new object[2]
      {
        (object) folder,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginfascicolazioneDelFolder(Folder folder, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneDelFolder", new object[2]
      {
        (object) folder,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndfascicolazioneDelFolder(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void fascicolazioneDelFolderAsync(Folder folder, InfoUtente infoUtente)
        {
            this.fascicolazioneDelFolderAsync(folder, infoUtente, (object)null);
        }

        public void fascicolazioneDelFolderAsync(Folder folder, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneDelFolderOperationCompleted == null)
                this.fascicolazioneDelFolderOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneDelFolderOperationCompleted);
            this.InvokeAsync("fascicolazioneDelFolder", new object[2]
      {
        (object) folder,
        (object) infoUtente
      }, this.fascicolazioneDelFolderOperationCompleted, userState);
        }

        private void OnfascicolazioneDelFolderOperationCompleted(object arg)
        {
            if (this.fascicolazioneDelFolderCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneDelFolderCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneModifyFolder", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void fascicolazioneModifyFolder(Folder folder, InfoUtente infoUtente)
        {
            this.Invoke("fascicolazioneModifyFolder", new object[2]
      {
        (object) folder,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginfascicolazioneModifyFolder(Folder folder, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneModifyFolder", new object[2]
      {
        (object) folder,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndfascicolazioneModifyFolder(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void fascicolazioneModifyFolderAsync(Folder folder, InfoUtente infoUtente)
        {
            this.fascicolazioneModifyFolderAsync(folder, infoUtente, (object)null);
        }

        public void fascicolazioneModifyFolderAsync(Folder folder, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneModifyFolderOperationCompleted == null)
                this.fascicolazioneModifyFolderOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneModifyFolderOperationCompleted);
            this.InvokeAsync("fascicolazioneModifyFolder", new object[2]
      {
        (object) folder,
        (object) infoUtente
      }, this.fascicolazioneModifyFolderOperationCompleted, userState);
        }

        private void OnfascicolazioneModifyFolderOperationCompleted(object arg)
        {
            if (this.fascicolazioneModifyFolderCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneModifyFolderCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneMoveFolder", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Folder fascicolazioneMoveFolder(Folder folderOld, Folder folderNew, InfoUtente infoUtente)
        {
            return (Folder)this.Invoke("fascicolazioneMoveFolder", new object[3]
      {
        (object) folderOld,
        (object) folderNew,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginfascicolazioneMoveFolder(Folder folderOld, Folder folderNew, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneMoveFolder", new object[3]
      {
        (object) folderOld,
        (object) folderNew,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Folder EndfascicolazioneMoveFolder(IAsyncResult asyncResult)
        {
            return (Folder)this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneMoveFolderAsync(Folder folderOld, Folder folderNew, InfoUtente infoUtente)
        {
            this.fascicolazioneMoveFolderAsync(folderOld, folderNew, infoUtente, (object)null);
        }

        public void fascicolazioneMoveFolderAsync(Folder folderOld, Folder folderNew, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneMoveFolderOperationCompleted == null)
                this.fascicolazioneMoveFolderOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneMoveFolderOperationCompleted);
            this.InvokeAsync("fascicolazioneMoveFolder", new object[3]
      {
        (object) folderOld,
        (object) folderNew,
        (object) infoUtente
      }, this.fascicolazioneMoveFolderOperationCompleted, userState);
        }

        private void OnfascicolazioneMoveFolderOperationCompleted(object arg)
        {
            if (this.fascicolazioneMoveFolderCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneMoveFolderCompleted((object)this, new fascicolazioneMoveFolderCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneNewFolder", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Folder fascicolazioneNewFolder(Folder folder, InfoUtente infoUtente, Ruolo ruolo)
        {
            return (Folder)this.Invoke("fascicolazioneNewFolder", new object[3]
      {
        (object) folder,
        (object) infoUtente,
        (object) ruolo
      })[0];
        }

        public IAsyncResult BeginfascicolazioneNewFolder(Folder folder, InfoUtente infoUtente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneNewFolder", new object[3]
      {
        (object) folder,
        (object) infoUtente,
        (object) ruolo
      }, callback, asyncState);
        }

        public Folder EndfascicolazioneNewFolder(IAsyncResult asyncResult)
        {
            return (Folder)this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneNewFolderAsync(Folder folder, InfoUtente infoUtente, Ruolo ruolo)
        {
            this.fascicolazioneNewFolderAsync(folder, infoUtente, ruolo, (object)null);
        }

        public void fascicolazioneNewFolderAsync(Folder folder, InfoUtente infoUtente, Ruolo ruolo, object userState)
        {
            if (this.fascicolazioneNewFolderOperationCompleted == null)
                this.fascicolazioneNewFolderOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneNewFolderOperationCompleted);
            this.InvokeAsync("fascicolazioneNewFolder", new object[3]
      {
        (object) folder,
        (object) infoUtente,
        (object) ruolo
      }, this.fascicolazioneNewFolderOperationCompleted, userState);
        }

        private void OnfascicolazioneNewFolderOperationCompleted(object arg)
        {
            if (this.fascicolazioneNewFolderCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneNewFolderCompleted((object)this, new fascicolazioneNewFolderCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneDeleteDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void fascicolazioneDeleteDocumento(InfoDocumento infoDoc, Folder folder, InfoUtente infoUtente)
        {
            this.Invoke("fascicolazioneDeleteDocumento", new object[3]
      {
        (object) infoDoc,
        (object) folder,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginfascicolazioneDeleteDocumento(InfoDocumento infoDoc, Folder folder, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneDeleteDocumento", new object[3]
      {
        (object) infoDoc,
        (object) folder,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndfascicolazioneDeleteDocumento(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void fascicolazioneDeleteDocumentoAsync(InfoDocumento infoDoc, Folder folder, InfoUtente infoUtente)
        {
            this.fascicolazioneDeleteDocumentoAsync(infoDoc, folder, infoUtente, (object)null);
        }

        public void fascicolazioneDeleteDocumentoAsync(InfoDocumento infoDoc, Folder folder, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneDeleteDocumentoOperationCompleted == null)
                this.fascicolazioneDeleteDocumentoOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneDeleteDocumentoOperationCompleted);
            this.InvokeAsync("fascicolazioneDeleteDocumento", new object[3]
      {
        (object) infoDoc,
        (object) folder,
        (object) infoUtente
      }, this.fascicolazioneDeleteDocumentoOperationCompleted, userState);
        }

        private void OnfascicolazioneDeleteDocumentoOperationCompleted(object arg)
        {
            if (this.fascicolazioneDeleteDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneDeleteDocumentoCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetDocumenti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public InfoDocumento[] fascicolazioneGetDocumenti(Folder folder, InfoUtente infoUtente)
        {
            return (InfoDocumento[])this.Invoke("fascicolazioneGetDocumenti", new object[2]
      {
        (object) folder,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetDocumenti(Folder folder, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetDocumenti", new object[2]
      {
        (object) folder,
        (object) infoUtente
      }, callback, asyncState);
        }

        public InfoDocumento[] EndfascicolazioneGetDocumenti(IAsyncResult asyncResult)
        {
            return (InfoDocumento[])this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetDocumentiAsync(Folder folder, InfoUtente infoUtente)
        {
            this.fascicolazioneGetDocumentiAsync(folder, infoUtente, (object)null);
        }

        public void fascicolazioneGetDocumentiAsync(Folder folder, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneGetDocumentiOperationCompleted == null)
                this.fascicolazioneGetDocumentiOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetDocumentiOperationCompleted);
            this.InvokeAsync("fascicolazioneGetDocumenti", new object[2]
      {
        (object) folder,
        (object) infoUtente
      }, this.fascicolazioneGetDocumentiOperationCompleted, userState);
        }

        private void OnfascicolazioneGetDocumentiOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetDocumentiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetDocumentiCompleted((object)this, new fascicolazioneGetDocumentiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetFolder", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Folder fascicolazioneGetFolder(Fascicolo fascicolo, InfoUtente infoUtente)
        {
            return (Folder)this.Invoke("fascicolazioneGetFolder", new object[2]
      {
        (object) fascicolo,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetFolder(Fascicolo fascicolo, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetFolder", new object[2]
      {
        (object) fascicolo,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Folder EndfascicolazioneGetFolder(IAsyncResult asyncResult)
        {
            return (Folder)this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetFolderAsync(Fascicolo fascicolo, InfoUtente infoUtente)
        {
            this.fascicolazioneGetFolderAsync(fascicolo, infoUtente, (object)null);
        }

        public void fascicolazioneGetFolderAsync(Fascicolo fascicolo, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneGetFolderOperationCompleted == null)
                this.fascicolazioneGetFolderOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetFolderOperationCompleted);
            this.InvokeAsync("fascicolazioneGetFolder", new object[2]
      {
        (object) fascicolo,
        (object) infoUtente
      }, this.fascicolazioneGetFolderOperationCompleted, userState);
        }

        private void OnfascicolazioneGetFolderOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetFolderCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetFolderCompleted((object)this, new fascicolazioneGetFolderCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetVisibilita", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicoloDiritto[] fascicolazioneGetVisibilita(InfoFascicolo infoFascicolo, InfoUtente infoUtente)
        {
            return (FascicoloDiritto[])this.Invoke("fascicolazioneGetVisibilita", new object[2]
      {
        (object) infoFascicolo,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetVisibilita(InfoFascicolo infoFascicolo, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetVisibilita", new object[2]
      {
        (object) infoFascicolo,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FascicoloDiritto[] EndfascicolazioneGetVisibilita(IAsyncResult asyncResult)
        {
            return (FascicoloDiritto[])this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetVisibilitaAsync(InfoFascicolo infoFascicolo, InfoUtente infoUtente)
        {
            this.fascicolazioneGetVisibilitaAsync(infoFascicolo, infoUtente, (object)null);
        }

        public void fascicolazioneGetVisibilitaAsync(InfoFascicolo infoFascicolo, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneGetVisibilitaOperationCompleted == null)
                this.fascicolazioneGetVisibilitaOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetVisibilitaOperationCompleted);
            this.InvokeAsync("fascicolazioneGetVisibilita", new object[2]
      {
        (object) infoFascicolo,
        (object) infoUtente
      }, this.fascicolazioneGetVisibilitaOperationCompleted, userState);
        }

        private void OnfascicolazioneGetVisibilitaOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetVisibilitaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetVisibilitaCompleted((object)this, new fascicolazioneGetVisibilitaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneSospendiRiattivaUtente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void fascicolazioneSospendiRiattivaUtente(InfoUtente infoUtente, Corrispondente corr, Fascicolo fasc)
        {
            this.Invoke("fascicolazioneSospendiRiattivaUtente", new object[3]
      {
        (object) infoUtente,
        (object) corr,
        (object) fasc
      });
        }

        public IAsyncResult BeginfascicolazioneSospendiRiattivaUtente(InfoUtente infoUtente, Corrispondente corr, Fascicolo fasc, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneSospendiRiattivaUtente", new object[3]
      {
        (object) infoUtente,
        (object) corr,
        (object) fasc
      }, callback, asyncState);
        }

        public void EndfascicolazioneSospendiRiattivaUtente(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void fascicolazioneSospendiRiattivaUtenteAsync(InfoUtente infoUtente, Corrispondente corr, Fascicolo fasc)
        {
            this.fascicolazioneSospendiRiattivaUtenteAsync(infoUtente, corr, fasc, (object)null);
        }

        public void fascicolazioneSospendiRiattivaUtenteAsync(InfoUtente infoUtente, Corrispondente corr, Fascicolo fasc, object userState)
        {
            if (this.fascicolazioneSospendiRiattivaUtenteOperationCompleted == null)
                this.fascicolazioneSospendiRiattivaUtenteOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneSospendiRiattivaUtenteOperationCompleted);
            this.InvokeAsync("fascicolazioneSospendiRiattivaUtente", new object[3]
      {
        (object) infoUtente,
        (object) corr,
        (object) fasc
      }, this.fascicolazioneSospendiRiattivaUtenteOperationCompleted, userState);
        }

        private void OnfascicolazioneSospendiRiattivaUtenteOperationCompleted(object arg)
        {
            if (this.fascicolazioneSospendiRiattivaUtenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneSospendiRiattivaUtenteCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetFascicoloDaCodice", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo fascicolazioneGetFascicoloDaCodice(string codiceFascicolo, Registro registro, InfoUtente infoUtente)
        {
            return (Fascicolo)this.Invoke("fascicolazioneGetFascicoloDaCodice", new object[3]
      {
        (object) codiceFascicolo,
        (object) registro,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetFascicoloDaCodice(string codiceFascicolo, Registro registro, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetFascicoloDaCodice", new object[3]
      {
        (object) codiceFascicolo,
        (object) registro,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Fascicolo EndfascicolazioneGetFascicoloDaCodice(IAsyncResult asyncResult)
        {
            return (Fascicolo)this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetFascicoloDaCodiceAsync(string codiceFascicolo, Registro registro, InfoUtente infoUtente)
        {
            this.fascicolazioneGetFascicoloDaCodiceAsync(codiceFascicolo, registro, infoUtente, (object)null);
        }

        public void fascicolazioneGetFascicoloDaCodiceAsync(string codiceFascicolo, Registro registro, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneGetFascicoloDaCodiceOperationCompleted == null)
                this.fascicolazioneGetFascicoloDaCodiceOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetFascicoloDaCodiceOperationCompleted);
            this.InvokeAsync("fascicolazioneGetFascicoloDaCodice", new object[3]
      {
        (object) codiceFascicolo,
        (object) registro,
        (object) infoUtente
      }, this.fascicolazioneGetFascicoloDaCodiceOperationCompleted, userState);
        }

        private void OnfascicolazioneGetFascicoloDaCodiceOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetFascicoloDaCodiceCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetFascicoloDaCodiceCompleted((object)this, new fascicolazioneGetFascicoloDaCodiceCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetFascicoliDaDoc", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo[] fascicolazioneGetFascicoliDaDoc(InfoDocumento infoDocumento, InfoUtente infoUtente)
        {
            return (Fascicolo[])this.Invoke("fascicolazioneGetFascicoliDaDoc", new object[2]
      {
        (object) infoDocumento,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetFascicoliDaDoc(InfoDocumento infoDocumento, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetFascicoliDaDoc", new object[2]
      {
        (object) infoDocumento,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Fascicolo[] EndfascicolazioneGetFascicoliDaDoc(IAsyncResult asyncResult)
        {
            return (Fascicolo[])this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetFascicoliDaDocAsync(InfoDocumento infoDocumento, InfoUtente infoUtente)
        {
            this.fascicolazioneGetFascicoliDaDocAsync(infoDocumento, infoUtente, (object)null);
        }

        public void fascicolazioneGetFascicoliDaDocAsync(InfoDocumento infoDocumento, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneGetFascicoliDaDocOperationCompleted == null)
                this.fascicolazioneGetFascicoliDaDocOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetFascicoliDaDocOperationCompleted);
            this.InvokeAsync("fascicolazioneGetFascicoliDaDoc", new object[2]
      {
        (object) infoDocumento,
        (object) infoUtente
      }, this.fascicolazioneGetFascicoliDaDocOperationCompleted, userState);
        }

        private void OnfascicolazioneGetFascicoliDaDocOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetFascicoliDaDocCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetFascicoliDaDocCompleted((object)this, new fascicolazioneGetFascicoliDaDocCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazionesetDataVistaFasc", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void fascicolazionesetDataVistaFasc(string idFasc, InfoUtente infoUtente)
        {
            this.Invoke("fascicolazionesetDataVistaFasc", new object[2]
      {
        (object) idFasc,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginfascicolazionesetDataVistaFasc(string idFasc, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazionesetDataVistaFasc", new object[2]
      {
        (object) idFasc,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndfascicolazionesetDataVistaFasc(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void fascicolazionesetDataVistaFascAsync(string idFasc, InfoUtente infoUtente)
        {
            this.fascicolazionesetDataVistaFascAsync(idFasc, infoUtente, (object)null);
        }

        public void fascicolazionesetDataVistaFascAsync(string idFasc, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazionesetDataVistaFascOperationCompleted == null)
                this.fascicolazionesetDataVistaFascOperationCompleted = new SendOrPostCallback(this.OnfascicolazionesetDataVistaFascOperationCompleted);
            this.InvokeAsync("fascicolazionesetDataVistaFasc", new object[2]
      {
        (object) idFasc,
        (object) infoUtente
      }, this.fascicolazionesetDataVistaFascOperationCompleted, userState);
        }

        private void OnfascicolazionesetDataVistaFascOperationCompleted(object arg)
        {
            if (this.fascicolazionesetDataVistaFascCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazionesetDataVistaFascCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetDettaglioFascicolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo fascicolazioneGetDettaglioFascicolo(InfoFascicolo infoFascicolo, InfoUtente infoUtente)
        {
            return (Fascicolo)this.Invoke("fascicolazioneGetDettaglioFascicolo", new object[2]
      {
        (object) infoFascicolo,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetDettaglioFascicolo(InfoFascicolo infoFascicolo, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetDettaglioFascicolo", new object[2]
      {
        (object) infoFascicolo,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Fascicolo EndfascicolazioneGetDettaglioFascicolo(IAsyncResult asyncResult)
        {
            return (Fascicolo)this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetDettaglioFascicoloAsync(InfoFascicolo infoFascicolo, InfoUtente infoUtente)
        {
            this.fascicolazioneGetDettaglioFascicoloAsync(infoFascicolo, infoUtente, (object)null);
        }

        public void fascicolazioneGetDettaglioFascicoloAsync(InfoFascicolo infoFascicolo, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneGetDettaglioFascicoloOperationCompleted == null)
                this.fascicolazioneGetDettaglioFascicoloOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetDettaglioFascicoloOperationCompleted);
            this.InvokeAsync("fascicolazioneGetDettaglioFascicolo", new object[2]
      {
        (object) infoFascicolo,
        (object) infoUtente
      }, this.fascicolazioneGetDettaglioFascicoloOperationCompleted, userState);
        }

        private void OnfascicolazioneGetDettaglioFascicoloOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetDettaglioFascicoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetDettaglioFascicoloCompleted((object)this, new fascicolazioneGetDettaglioFascicoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneAddDocFascicolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void fascicolazioneAddDocFascicolo(string idProfile, string idFascicolo, InfoUtente infoUtente)
        {
            this.Invoke("fascicolazioneAddDocFascicolo", new object[3]
      {
        (object) idProfile,
        (object) idFascicolo,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginfascicolazioneAddDocFascicolo(string idProfile, string idFascicolo, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneAddDocFascicolo", new object[3]
      {
        (object) idProfile,
        (object) idFascicolo,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndfascicolazioneAddDocFascicolo(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void fascicolazioneAddDocFascicoloAsync(string idProfile, string idFascicolo, InfoUtente infoUtente)
        {
            this.fascicolazioneAddDocFascicoloAsync(idProfile, idFascicolo, infoUtente, (object)null);
        }

        public void fascicolazioneAddDocFascicoloAsync(string idProfile, string idFascicolo, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneAddDocFascicoloOperationCompleted == null)
                this.fascicolazioneAddDocFascicoloOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneAddDocFascicoloOperationCompleted);
            this.InvokeAsync("fascicolazioneAddDocFascicolo", new object[3]
      {
        (object) idProfile,
        (object) idFascicolo,
        (object) infoUtente
      }, this.fascicolazioneAddDocFascicoloOperationCompleted, userState);
        }

        private void OnfascicolazioneAddDocFascicoloOperationCompleted(object arg)
        {
            if (this.fascicolazioneAddDocFascicoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneAddDocFascicoloCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneSetFascicolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void fascicolazioneSetFascicolo(Fascicolo fascicolo, InfoUtente infoUtente)
        {
            this.Invoke("fascicolazioneSetFascicolo", new object[2]
      {
        (object) fascicolo,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginfascicolazioneSetFascicolo(Fascicolo fascicolo, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneSetFascicolo", new object[2]
      {
        (object) fascicolo,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndfascicolazioneSetFascicolo(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void fascicolazioneSetFascicoloAsync(Fascicolo fascicolo, InfoUtente infoUtente)
        {
            this.fascicolazioneSetFascicoloAsync(fascicolo, infoUtente, (object)null);
        }

        public void fascicolazioneSetFascicoloAsync(Fascicolo fascicolo, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneSetFascicoloOperationCompleted == null)
                this.fascicolazioneSetFascicoloOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneSetFascicoloOperationCompleted);
            this.InvokeAsync("fascicolazioneSetFascicolo", new object[2]
      {
        (object) fascicolo,
        (object) infoUtente
      }, this.fascicolazioneSetFascicoloOperationCompleted, userState);
        }

        private void OnfascicolazioneSetFascicoloOperationCompleted(object arg)
        {
            if (this.fascicolazioneSetFascicoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneSetFascicoloCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneNewFascicolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo fascicolazioneNewFascicolo(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo)
        {
            return (Fascicolo)this.Invoke("fascicolazioneNewFascicolo", new object[4]
      {
        (object) classificazione,
        (object) fascicolo,
        (object) infoUtente,
        (object) ruolo
      })[0];
        }

        public IAsyncResult BeginfascicolazioneNewFascicolo(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneNewFascicolo", new object[4]
      {
        (object) classificazione,
        (object) fascicolo,
        (object) infoUtente,
        (object) ruolo
      }, callback, asyncState);
        }

        public Fascicolo EndfascicolazioneNewFascicolo(IAsyncResult asyncResult)
        {
            return (Fascicolo)this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneNewFascicoloAsync(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo)
        {
            this.fascicolazioneNewFascicoloAsync(classificazione, fascicolo, infoUtente, ruolo, (object)null);
        }

        public void fascicolazioneNewFascicoloAsync(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, InfoUtente infoUtente, Ruolo ruolo, object userState)
        {
            if (this.fascicolazioneNewFascicoloOperationCompleted == null)
                this.fascicolazioneNewFascicoloOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneNewFascicoloOperationCompleted);
            this.InvokeAsync("fascicolazioneNewFascicolo", new object[4]
      {
        (object) classificazione,
        (object) fascicolo,
        (object) infoUtente,
        (object) ruolo
      }, this.fascicolazioneNewFascicoloOperationCompleted, userState);
        }

        private void OnfascicolazioneNewFascicoloOperationCompleted(object arg)
        {
            if (this.fascicolazioneNewFascicoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneNewFascicoloCompleted((object)this, new fascicolazioneNewFascicoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetDocumentiPaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public InfoDocumento[] fascicolazioneGetDocumentiPaging(Folder folder, int pagina, int righe, ref int risultati, InfoUtente infoUtente)
        {
            object[] objArray = this.Invoke("fascicolazioneGetDocumentiPaging", new object[5]
      {
        (object) folder,
        (object) pagina,
        (object) righe,
        (object) risultati,
        (object) infoUtente
      });
            risultati = (int)objArray[1];
            return (InfoDocumento[])objArray[0];
        }

        public IAsyncResult BeginfascicolazioneGetDocumentiPaging(Folder folder, int pagina, int righe, int risultati, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetDocumentiPaging", new object[5]
      {
        (object) folder,
        (object) pagina,
        (object) righe,
        (object) risultati,
        (object) infoUtente
      }, callback, asyncState);
        }

        public InfoDocumento[] EndfascicolazioneGetDocumentiPaging(IAsyncResult asyncResult, out int risultati)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            risultati = (int)objArray[1];
            return (InfoDocumento[])objArray[0];
        }

        public void fascicolazioneGetDocumentiPagingAsync(Folder folder, int pagina, int righe, int risultati, InfoUtente infoUtente)
        {
            this.fascicolazioneGetDocumentiPagingAsync(folder, pagina, righe, risultati, infoUtente, (object)null);
        }

        public void fascicolazioneGetDocumentiPagingAsync(Folder folder, int pagina, int righe, int risultati, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneGetDocumentiPagingOperationCompleted == null)
                this.fascicolazioneGetDocumentiPagingOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetDocumentiPagingOperationCompleted);
            this.InvokeAsync("fascicolazioneGetDocumentiPaging", new object[5]
      {
        (object) folder,
        (object) pagina,
        (object) righe,
        (object) risultati,
        (object) infoUtente
      }, this.fascicolazioneGetDocumentiPagingOperationCompleted, userState);
        }

        private void OnfascicolazioneGetDocumentiPagingOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetDocumentiPagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetDocumentiPagingCompleted((object)this, new fascicolazioneGetDocumentiPagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetListaFascicoliPaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo[] fascicolazioneGetListaFascicoliPaging(FascicolazioneClassificazione classificazione, InfoUtente infoUtente, FiltroRicerca[] listaFiltri, int pagina, ref int risultati, ref int rTot, int righe)
        {
            object[] objArray = this.Invoke("fascicolazioneGetListaFascicoliPaging", new object[7]
      {
        (object) classificazione,
        (object) infoUtente,
        (object) listaFiltri,
        (object) pagina,
        (object) risultati,
        (object) rTot,
        (object) righe
      });
            risultati = (int)objArray[1];
            rTot = (int)objArray[2];
            return (Fascicolo[])objArray[0];
        }

        public IAsyncResult BeginfascicolazioneGetListaFascicoliPaging(FascicolazioneClassificazione classificazione, InfoUtente infoUtente, FiltroRicerca[] listaFiltri, int pagina, int risultati, int rTot, int righe, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetListaFascicoliPaging", new object[7]
      {
        (object) classificazione,
        (object) infoUtente,
        (object) listaFiltri,
        (object) pagina,
        (object) risultati,
        (object) rTot,
        (object) righe
      }, callback, asyncState);
        }

        public Fascicolo[] EndfascicolazioneGetListaFascicoliPaging(IAsyncResult asyncResult, out int risultati, out int rTot)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            risultati = (int)objArray[1];
            rTot = (int)objArray[2];
            return (Fascicolo[])objArray[0];
        }

        public void fascicolazioneGetListaFascicoliPagingAsync(FascicolazioneClassificazione classificazione, InfoUtente infoUtente, FiltroRicerca[] listaFiltri, int pagina, int risultati, int rTot, int righe)
        {
            this.fascicolazioneGetListaFascicoliPagingAsync(classificazione, infoUtente, listaFiltri, pagina, risultati, rTot, righe, (object)null);
        }

        public void fascicolazioneGetListaFascicoliPagingAsync(FascicolazioneClassificazione classificazione, InfoUtente infoUtente, FiltroRicerca[] listaFiltri, int pagina, int risultati, int rTot, int righe, object userState)
        {
            if (this.fascicolazioneGetListaFascicoliPagingOperationCompleted == null)
                this.fascicolazioneGetListaFascicoliPagingOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetListaFascicoliPagingOperationCompleted);
            this.InvokeAsync("fascicolazioneGetListaFascicoliPaging", new object[7]
      {
        (object) classificazione,
        (object) infoUtente,
        (object) listaFiltri,
        (object) pagina,
        (object) risultati,
        (object) rTot,
        (object) righe
      }, this.fascicolazioneGetListaFascicoliPagingOperationCompleted, userState);
        }

        private void OnfascicolazioneGetListaFascicoliPagingOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetListaFascicoliPagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetListaFascicoliPagingCompleted((object)this, new fascicolazioneGetListaFascicoliPagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetListaFascicoli", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Fascicolo[] fascicolazioneGetListaFascicoli(FascicolazioneClassificazione classificazione, InfoUtente infoUtente, FiltroRicerca[] listaFiltri, bool childs)
        {
            return (Fascicolo[])this.Invoke("fascicolazioneGetListaFascicoli", new object[4]
      {
        (object) classificazione,
        (object) infoUtente,
        (object) listaFiltri,
        (object) childs
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetListaFascicoli(FascicolazioneClassificazione classificazione, InfoUtente infoUtente, FiltroRicerca[] listaFiltri, bool childs, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetListaFascicoli", new object[4]
      {
        (object) classificazione,
        (object) infoUtente,
        (object) listaFiltri,
        (object) childs
      }, callback, asyncState);
        }

        public Fascicolo[] EndfascicolazioneGetListaFascicoli(IAsyncResult asyncResult)
        {
            return (Fascicolo[])this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetListaFascicoliAsync(FascicolazioneClassificazione classificazione, InfoUtente infoUtente, FiltroRicerca[] listaFiltri, bool childs)
        {
            this.fascicolazioneGetListaFascicoliAsync(classificazione, infoUtente, listaFiltri, childs, (object)null);
        }

        public void fascicolazioneGetListaFascicoliAsync(FascicolazioneClassificazione classificazione, InfoUtente infoUtente, FiltroRicerca[] listaFiltri, bool childs, object userState)
        {
            if (this.fascicolazioneGetListaFascicoliOperationCompleted == null)
                this.fascicolazioneGetListaFascicoliOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetListaFascicoliOperationCompleted);
            this.InvokeAsync("fascicolazioneGetListaFascicoli", new object[4]
      {
        (object) classificazione,
        (object) infoUtente,
        (object) listaFiltri,
        (object) childs
      }, this.fascicolazioneGetListaFascicoliOperationCompleted, userState);
        }

        private void OnfascicolazioneGetListaFascicoliOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetListaFascicoliCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetListaFascicoliCompleted((object)this, new fascicolazioneGetListaFascicoliCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetFigliClassifica", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicolazioneClassifica[] fascicolazioneGetFigliClassifica(FascicolazioneClassifica classifica, string idRegistro, InfoUtente infoUtente)
        {
            return (FascicolazioneClassifica[])this.Invoke("fascicolazioneGetFigliClassifica", new object[3]
      {
        (object) classifica,
        (object) idRegistro,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetFigliClassifica(FascicolazioneClassifica classifica, string idRegistro, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetFigliClassifica", new object[3]
      {
        (object) classifica,
        (object) idRegistro,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FascicolazioneClassifica[] EndfascicolazioneGetFigliClassifica(IAsyncResult asyncResult)
        {
            return (FascicolazioneClassifica[])this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetFigliClassificaAsync(FascicolazioneClassifica classifica, string idRegistro, InfoUtente infoUtente)
        {
            this.fascicolazioneGetFigliClassificaAsync(classifica, idRegistro, infoUtente, (object)null);
        }

        public void fascicolazioneGetFigliClassificaAsync(FascicolazioneClassifica classifica, string idRegistro, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneGetFigliClassificaOperationCompleted == null)
                this.fascicolazioneGetFigliClassificaOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetFigliClassificaOperationCompleted);
            this.InvokeAsync("fascicolazioneGetFigliClassifica", new object[3]
      {
        (object) classifica,
        (object) idRegistro,
        (object) infoUtente
      }, this.fascicolazioneGetFigliClassificaOperationCompleted, userState);
        }

        private void OnfascicolazioneGetFigliClassificaOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetFigliClassificaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetFigliClassificaCompleted((object)this, new fascicolazioneGetFigliClassificaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetGerarchiaDaCodice", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicolazioneClassifica[] fascicolazioneGetGerarchiaDaCodice(string codiceClassificazione, Registro registro, InfoUtente infoUtente)
        {
            return (FascicolazioneClassifica[])this.Invoke("fascicolazioneGetGerarchiaDaCodice", new object[3]
      {
        (object) codiceClassificazione,
        (object) registro,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetGerarchiaDaCodice(string codiceClassificazione, Registro registro, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetGerarchiaDaCodice", new object[3]
      {
        (object) codiceClassificazione,
        (object) registro,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FascicolazioneClassifica[] EndfascicolazioneGetGerarchiaDaCodice(IAsyncResult asyncResult)
        {
            return (FascicolazioneClassifica[])this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetGerarchiaDaCodiceAsync(string codiceClassificazione, Registro registro, InfoUtente infoUtente)
        {
            this.fascicolazioneGetGerarchiaDaCodiceAsync(codiceClassificazione, registro, infoUtente, (object)null);
        }

        public void fascicolazioneGetGerarchiaDaCodiceAsync(string codiceClassificazione, Registro registro, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneGetGerarchiaDaCodiceOperationCompleted == null)
                this.fascicolazioneGetGerarchiaDaCodiceOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetGerarchiaDaCodiceOperationCompleted);
            this.InvokeAsync("fascicolazioneGetGerarchiaDaCodice", new object[3]
      {
        (object) codiceClassificazione,
        (object) registro,
        (object) infoUtente
      }, this.fascicolazioneGetGerarchiaDaCodiceOperationCompleted, userState);
        }

        private void OnfascicolazioneGetGerarchiaDaCodiceOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetGerarchiaDaCodiceCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetGerarchiaDaCodiceCompleted((object)this, new fascicolazioneGetGerarchiaDaCodiceCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetGerarchia", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicolazioneClassifica[] fascicolazioneGetGerarchia(string idClassificazione, InfoUtente infoUtente)
        {
            return (FascicolazioneClassifica[])this.Invoke("fascicolazioneGetGerarchia", new object[2]
      {
        (object) idClassificazione,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetGerarchia(string idClassificazione, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetGerarchia", new object[2]
      {
        (object) idClassificazione,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FascicolazioneClassifica[] EndfascicolazioneGetGerarchia(IAsyncResult asyncResult)
        {
            return (FascicolazioneClassifica[])this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetGerarchiaAsync(string idClassificazione, InfoUtente infoUtente)
        {
            this.fascicolazioneGetGerarchiaAsync(idClassificazione, infoUtente, (object)null);
        }

        public void fascicolazioneGetGerarchiaAsync(string idClassificazione, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneGetGerarchiaOperationCompleted == null)
                this.fascicolazioneGetGerarchiaOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetGerarchiaOperationCompleted);
            this.InvokeAsync("fascicolazioneGetGerarchia", new object[2]
      {
        (object) idClassificazione,
        (object) infoUtente
      }, this.fascicolazioneGetGerarchiaOperationCompleted, userState);
        }

        private void OnfascicolazioneGetGerarchiaOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetGerarchiaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetGerarchiaCompleted((object)this, new fascicolazioneGetGerarchiaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicolazioneClassificazione[] fascicolazioneGetTitolario(InfoUtente infoUtente, Registro registro, string codiceClassifica)
        {
            return (FascicolazioneClassificazione[])this.Invoke("fascicolazioneGetTitolario", new object[3]
      {
        (object) infoUtente,
        (object) registro,
        (object) codiceClassifica
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetTitolario(InfoUtente infoUtente, Registro registro, string codiceClassifica, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetTitolario", new object[3]
      {
        (object) infoUtente,
        (object) registro,
        (object) codiceClassifica
      }, callback, asyncState);
        }

        public FascicolazioneClassificazione[] EndfascicolazioneGetTitolario(IAsyncResult asyncResult)
        {
            return (FascicolazioneClassificazione[])this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetTitolarioAsync(InfoUtente infoUtente, Registro registro, string codiceClassifica)
        {
            this.fascicolazioneGetTitolarioAsync(infoUtente, registro, codiceClassifica, (object)null);
        }

        public void fascicolazioneGetTitolarioAsync(InfoUtente infoUtente, Registro registro, string codiceClassifica, object userState)
        {
            if (this.fascicolazioneGetTitolarioOperationCompleted == null)
                this.fascicolazioneGetTitolarioOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetTitolarioOperationCompleted);
            this.InvokeAsync("fascicolazioneGetTitolario", new object[3]
      {
        (object) infoUtente,
        (object) registro,
        (object) codiceClassifica
      }, this.fascicolazioneGetTitolarioOperationCompleted, userState);
        }

        private void OnfascicolazioneGetTitolarioOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetTitolarioCompleted((object)this, new fascicolazioneGetTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetQueryDocumentoPaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public InfoDocumento[] documentoGetQueryDocumentoPaging([XmlArrayItem("ArrayOfFiltroRicerca"), XmlArrayItem(NestingLevel = 1)] FiltroRicerca[][] queryList, ref int risultati, ref int risultTot, int pagina, int max, InfoUtente infoUtente)
        {
            object[] objArray = this.Invoke("documentoGetQueryDocumentoPaging", new object[6]
      {
        (object) queryList,
        (object) risultati,
        (object) risultTot,
        (object) pagina,
        (object) max,
        (object) infoUtente
      });
            risultati = (int)objArray[1];
            risultTot = (int)objArray[2];
            return (InfoDocumento[])objArray[0];
        }

        public IAsyncResult BegindocumentoGetQueryDocumentoPaging(FiltroRicerca[][] queryList, int risultati, int risultTot, int pagina, int max, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetQueryDocumentoPaging", new object[6]
      {
        (object) queryList,
        (object) risultati,
        (object) risultTot,
        (object) pagina,
        (object) max,
        (object) infoUtente
      }, callback, asyncState);
        }

        public InfoDocumento[] EnddocumentoGetQueryDocumentoPaging(IAsyncResult asyncResult, out int risultati, out int risultTot)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            risultati = (int)objArray[1];
            risultTot = (int)objArray[2];
            return (InfoDocumento[])objArray[0];
        }

        public void documentoGetQueryDocumentoPagingAsync(FiltroRicerca[][] queryList, int risultati, int risultTot, int pagina, int max, InfoUtente infoUtente)
        {
            this.documentoGetQueryDocumentoPagingAsync(queryList, risultati, risultTot, pagina, max, infoUtente, (object)null);
        }

        public void documentoGetQueryDocumentoPagingAsync(FiltroRicerca[][] queryList, int risultati, int risultTot, int pagina, int max, InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetQueryDocumentoPagingOperationCompleted == null)
                this.documentoGetQueryDocumentoPagingOperationCompleted = new SendOrPostCallback(this.OndocumentoGetQueryDocumentoPagingOperationCompleted);
            this.InvokeAsync("documentoGetQueryDocumentoPaging", new object[6]
      {
        (object) queryList,
        (object) risultati,
        (object) risultTot,
        (object) pagina,
        (object) max,
        (object) infoUtente
      }, this.documentoGetQueryDocumentoPagingOperationCompleted, userState);
        }

        private void OndocumentoGetQueryDocumentoPagingOperationCompleted(object arg)
        {
            if (this.documentoGetQueryDocumentoPagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetQueryDocumentoPagingCompleted((object)this, new documentoGetQueryDocumentoPagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoSpedisci", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void documentoSpedisci(SchedaDocumento schedaDocumento, InfoUtente infoUtente, bool confermaRic)
        {
            this.Invoke("documentoSpedisci", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) confermaRic
      });
        }

        public IAsyncResult BegindocumentoSpedisci(SchedaDocumento schedaDocumento, InfoUtente infoUtente, bool confermaRic, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoSpedisci", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) confermaRic
      }, callback, asyncState);
        }

        public void EnddocumentoSpedisci(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void documentoSpedisciAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, bool confermaRic)
        {
            this.documentoSpedisciAsync(schedaDocumento, infoUtente, confermaRic, (object)null);
        }

        public void documentoSpedisciAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, bool confermaRic, object userState)
        {
            if (this.documentoSpedisciOperationCompleted == null)
                this.documentoSpedisciOperationCompleted = new SendOrPostCallback(this.OndocumentoSpedisciOperationCompleted);
            this.InvokeAsync("documentoSpedisci", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) confermaRic
      }, this.documentoSpedisciOperationCompleted, userState);
        }

        private void OndocumentoSpedisciOperationCompleted(object arg)
        {
            if (this.documentoSpedisciCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoSpedisciCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoSetFlagDaInviare", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento documentoSetFlagDaInviare(SchedaDocumento schedaDocumento, InfoUtente infoUtente)
        {
            return (SchedaDocumento)this.Invoke("documentoSetFlagDaInviare", new object[2]
      {
        (object) schedaDocumento,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoSetFlagDaInviare(SchedaDocumento schedaDocumento, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoSetFlagDaInviare", new object[2]
      {
        (object) schedaDocumento,
        (object) infoUtente
      }, callback, asyncState);
        }

        public SchedaDocumento EnddocumentoSetFlagDaInviare(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void documentoSetFlagDaInviareAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente)
        {
            this.documentoSetFlagDaInviareAsync(schedaDocumento, infoUtente, (object)null);
        }

        public void documentoSetFlagDaInviareAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, object userState)
        {
            if (this.documentoSetFlagDaInviareOperationCompleted == null)
                this.documentoSetFlagDaInviareOperationCompleted = new SendOrPostCallback(this.OndocumentoSetFlagDaInviareOperationCompleted);
            this.InvokeAsync("documentoSetFlagDaInviare", new object[2]
      {
        (object) schedaDocumento,
        (object) infoUtente
      }, this.documentoSetFlagDaInviareOperationCompleted, userState);
        }

        private void OndocumentoSetFlagDaInviareOperationCompleted(object arg)
        {
            if (this.documentoSetFlagDaInviareCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoSetFlagDaInviareCompleted((object)this, new documentoSetFlagDaInviareCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetVisibilita", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DocumentoDiritto[] documentoGetVisibilita(InfoDocumento infoDocumento, InfoUtente infoUtente)
        {
            return (DocumentoDiritto[])this.Invoke("documentoGetVisibilita", new object[2]
      {
        (object) infoDocumento,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetVisibilita(InfoDocumento infoDocumento, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetVisibilita", new object[2]
      {
        (object) infoDocumento,
        (object) infoUtente
      }, callback, asyncState);
        }

        public DocumentoDiritto[] EnddocumentoGetVisibilita(IAsyncResult asyncResult)
        {
            return (DocumentoDiritto[])this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetVisibilitaAsync(InfoDocumento infoDocumento, InfoUtente infoUtente)
        {
            this.documentoGetVisibilitaAsync(infoDocumento, infoUtente, (object)null);
        }

        public void documentoGetVisibilitaAsync(InfoDocumento infoDocumento, InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetVisibilitaOperationCompleted == null)
                this.documentoGetVisibilitaOperationCompleted = new SendOrPostCallback(this.OndocumentoGetVisibilitaOperationCompleted);
            this.InvokeAsync("documentoGetVisibilita", new object[2]
      {
        (object) infoDocumento,
        (object) infoUtente
      }, this.documentoGetVisibilitaOperationCompleted, userState);
        }

        private void OndocumentoGetVisibilitaOperationCompleted(object arg)
        {
            if (this.documentoGetVisibilitaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetVisibilitaCompleted((object)this, new documentoGetVisibilitaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetTipologiaCanale", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TipologiaCanale[] documentoGetTipologiaCanale(InfoUtente infoUtente)
        {
            return (TipologiaCanale[])this.Invoke("documentoGetTipologiaCanale", new object[1]
      {
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetTipologiaCanale(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetTipologiaCanale", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public TipologiaCanale[] EnddocumentoGetTipologiaCanale(IAsyncResult asyncResult)
        {
            return (TipologiaCanale[])this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetTipologiaCanaleAsync(InfoUtente infoUtente)
        {
            this.documentoGetTipologiaCanaleAsync(infoUtente, (object)null);
        }

        public void documentoGetTipologiaCanaleAsync(InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetTipologiaCanaleOperationCompleted == null)
                this.documentoGetTipologiaCanaleOperationCompleted = new SendOrPostCallback(this.OndocumentoGetTipologiaCanaleOperationCompleted);
            this.InvokeAsync("documentoGetTipologiaCanale", new object[1]
      {
        (object) infoUtente
      }, this.documentoGetTipologiaCanaleOperationCompleted, userState);
        }

        private void OndocumentoGetTipologiaCanaleOperationCompleted(object arg)
        {
            if (this.documentoGetTipologiaCanaleCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetTipologiaCanaleCompleted((object)this, new documentoGetTipologiaCanaleCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetListaStoriciOggetto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DocumentoStoricoOggetto[] documentoGetListaStoriciOggetto(string idProfile, InfoUtente infoUtente)
        {
            return (DocumentoStoricoOggetto[])this.Invoke("documentoGetListaStoriciOggetto", new object[2]
      {
        (object) idProfile,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetListaStoriciOggetto(string idProfile, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetListaStoriciOggetto", new object[2]
      {
        (object) idProfile,
        (object) infoUtente
      }, callback, asyncState);
        }

        public DocumentoStoricoOggetto[] EnddocumentoGetListaStoriciOggetto(IAsyncResult asyncResult)
        {
            return (DocumentoStoricoOggetto[])this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetListaStoriciOggettoAsync(string idProfile, InfoUtente infoUtente)
        {
            this.documentoGetListaStoriciOggettoAsync(idProfile, infoUtente, (object)null);
        }

        public void documentoGetListaStoriciOggettoAsync(string idProfile, InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetListaStoriciOggettoOperationCompleted == null)
                this.documentoGetListaStoriciOggettoOperationCompleted = new SendOrPostCallback(this.OndocumentoGetListaStoriciOggettoOperationCompleted);
            this.InvokeAsync("documentoGetListaStoriciOggetto", new object[2]
      {
        (object) idProfile,
        (object) infoUtente
      }, this.documentoGetListaStoriciOggettoOperationCompleted, userState);
        }

        private void OndocumentoGetListaStoriciOggettoOperationCompleted(object arg)
        {
            if (this.documentoGetListaStoriciOggettoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetListaStoriciOggettoCompleted((object)this, new documentoGetListaStoriciOggettoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetCatenaDoc", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public AnelloDocumentale documentoGetCatenaDoc(InfoDocumento infoDoc, InfoUtente infoUtente)
        {
            return (AnelloDocumentale)this.Invoke("documentoGetCatenaDoc", new object[2]
      {
        (object) infoDoc,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetCatenaDoc(InfoDocumento infoDoc, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetCatenaDoc", new object[2]
      {
        (object) infoDoc,
        (object) infoUtente
      }, callback, asyncState);
        }

        public AnelloDocumentale EnddocumentoGetCatenaDoc(IAsyncResult asyncResult)
        {
            return (AnelloDocumentale)this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetCatenaDocAsync(InfoDocumento infoDoc, InfoUtente infoUtente)
        {
            this.documentoGetCatenaDocAsync(infoDoc, infoUtente, (object)null);
        }

        public void documentoGetCatenaDocAsync(InfoDocumento infoDoc, InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetCatenaDocOperationCompleted == null)
                this.documentoGetCatenaDocOperationCompleted = new SendOrPostCallback(this.OndocumentoGetCatenaDocOperationCompleted);
            this.InvokeAsync("documentoGetCatenaDoc", new object[2]
      {
        (object) infoDoc,
        (object) infoUtente
      }, this.documentoGetCatenaDocOperationCompleted, userState);
        }

        private void OndocumentoGetCatenaDocOperationCompleted(object arg)
        {
            if (this.documentoGetCatenaDocCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetCatenaDocCompleted((object)this, new documentoGetCatenaDocCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoExecRimuoviScheda", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void documentoExecRimuoviScheda(SchedaDocumento schedaDoc, InfoUtente infoUtente)
        {
            this.Invoke("documentoExecRimuoviScheda", new object[2]
      {
        (object) schedaDoc,
        (object) infoUtente
      });
        }

        public IAsyncResult BegindocumentoExecRimuoviScheda(SchedaDocumento schedaDoc, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoExecRimuoviScheda", new object[2]
      {
        (object) schedaDoc,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EnddocumentoExecRimuoviScheda(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void documentoExecRimuoviSchedaAsync(SchedaDocumento schedaDoc, InfoUtente infoUtente)
        {
            this.documentoExecRimuoviSchedaAsync(schedaDoc, infoUtente, (object)null);
        }

        public void documentoExecRimuoviSchedaAsync(SchedaDocumento schedaDoc, InfoUtente infoUtente, object userState)
        {
            if (this.documentoExecRimuoviSchedaOperationCompleted == null)
                this.documentoExecRimuoviSchedaOperationCompleted = new SendOrPostCallback(this.OndocumentoExecRimuoviSchedaOperationCompleted);
            this.InvokeAsync("documentoExecRimuoviScheda", new object[2]
      {
        (object) schedaDoc,
        (object) infoUtente
      }, this.documentoExecRimuoviSchedaOperationCompleted, userState);
        }

        private void OndocumentoExecRimuoviSchedaOperationCompleted(object arg)
        {
            if (this.documentoExecRimuoviSchedaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoExecRimuoviSchedaCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoExecAnnullaProt", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public ProtocolloAnnullato documentoExecAnnullaProt(InfoDocumento infoDoc, ProtocolloAnnullato protAnn, InfoUtente infoUtente)
        {
            return (ProtocolloAnnullato)this.Invoke("documentoExecAnnullaProt", new object[3]
      {
        (object) infoDoc,
        (object) protAnn,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoExecAnnullaProt(InfoDocumento infoDoc, ProtocolloAnnullato protAnn, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoExecAnnullaProt", new object[3]
      {
        (object) infoDoc,
        (object) protAnn,
        (object) infoUtente
      }, callback, asyncState);
        }

        public ProtocolloAnnullato EnddocumentoExecAnnullaProt(IAsyncResult asyncResult)
        {
            return (ProtocolloAnnullato)this.EndInvoke(asyncResult)[0];
        }

        public void documentoExecAnnullaProtAsync(InfoDocumento infoDoc, ProtocolloAnnullato protAnn, InfoUtente infoUtente)
        {
            this.documentoExecAnnullaProtAsync(infoDoc, protAnn, infoUtente, (object)null);
        }

        public void documentoExecAnnullaProtAsync(InfoDocumento infoDoc, ProtocolloAnnullato protAnn, InfoUtente infoUtente, object userState)
        {
            if (this.documentoExecAnnullaProtOperationCompleted == null)
                this.documentoExecAnnullaProtOperationCompleted = new SendOrPostCallback(this.OndocumentoExecAnnullaProtOperationCompleted);
            this.InvokeAsync("documentoExecAnnullaProt", new object[3]
      {
        (object) infoDoc,
        (object) protAnn,
        (object) infoUtente
      }, this.documentoExecAnnullaProtOperationCompleted, userState);
        }

        private void OndocumentoExecAnnullaProtOperationCompleted(object arg)
        {
            if (this.documentoExecAnnullaProtCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoExecAnnullaProtCompleted((object)this, new documentoExecAnnullaProtCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoModificaVersione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void documentoModificaVersione(FileRequest fileReq, InfoUtente infoUtente)
        {
            this.Invoke("documentoModificaVersione", new object[2]
      {
        (object) fileReq,
        (object) infoUtente
      });
        }

        public IAsyncResult BegindocumentoModificaVersione(FileRequest fileReq, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoModificaVersione", new object[2]
      {
        (object) fileReq,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EnddocumentoModificaVersione(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void documentoModificaVersioneAsync(FileRequest fileReq, InfoUtente infoUtente)
        {
            this.documentoModificaVersioneAsync(fileReq, infoUtente, (object)null);
        }

        public void documentoModificaVersioneAsync(FileRequest fileReq, InfoUtente infoUtente, object userState)
        {
            if (this.documentoModificaVersioneOperationCompleted == null)
                this.documentoModificaVersioneOperationCompleted = new SendOrPostCallback(this.OndocumentoModificaVersioneOperationCompleted);
            this.InvokeAsync("documentoModificaVersione", new object[2]
      {
        (object) fileReq,
        (object) infoUtente
      }, this.documentoModificaVersioneOperationCompleted, userState);
        }

        private void OndocumentoModificaVersioneOperationCompleted(object arg)
        {
            if (this.documentoModificaVersioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoModificaVersioneCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoScambiaVersioni", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void documentoScambiaVersioni(Documento documento, Allegato allegato, InfoUtente infoUtente)
        {
            this.Invoke("documentoScambiaVersioni", new object[3]
      {
        (object) documento,
        (object) allegato,
        (object) infoUtente
      });
        }

        public IAsyncResult BegindocumentoScambiaVersioni(Documento documento, Allegato allegato, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoScambiaVersioni", new object[3]
      {
        (object) documento,
        (object) allegato,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EnddocumentoScambiaVersioni(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void documentoScambiaVersioniAsync(Documento documento, Allegato allegato, InfoUtente infoUtente)
        {
            this.documentoScambiaVersioniAsync(documento, allegato, infoUtente, (object)null);
        }

        public void documentoScambiaVersioniAsync(Documento documento, Allegato allegato, InfoUtente infoUtente, object userState)
        {
            if (this.documentoScambiaVersioniOperationCompleted == null)
                this.documentoScambiaVersioniOperationCompleted = new SendOrPostCallback(this.OndocumentoScambiaVersioniOperationCompleted);
            this.InvokeAsync("documentoScambiaVersioni", new object[3]
      {
        (object) documento,
        (object) allegato,
        (object) infoUtente
      }, this.documentoScambiaVersioniOperationCompleted, userState);
        }

        private void OndocumentoScambiaVersioniOperationCompleted(object arg)
        {
            if (this.documentoScambiaVersioniCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoScambiaVersioniCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoAggiungiVersione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileRequest documentoAggiungiVersione(FileRequest fileRequest, InfoUtente infoUtente)
        {
            return (FileRequest)this.Invoke("documentoAggiungiVersione", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoAggiungiVersione(FileRequest fileRequest, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoAggiungiVersione", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FileRequest EnddocumentoAggiungiVersione(IAsyncResult asyncResult)
        {
            return (FileRequest)this.EndInvoke(asyncResult)[0];
        }

        public void documentoAggiungiVersioneAsync(FileRequest fileRequest, InfoUtente infoUtente)
        {
            this.documentoAggiungiVersioneAsync(fileRequest, infoUtente, (object)null);
        }

        public void documentoAggiungiVersioneAsync(FileRequest fileRequest, InfoUtente infoUtente, object userState)
        {
            if (this.documentoAggiungiVersioneOperationCompleted == null)
                this.documentoAggiungiVersioneOperationCompleted = new SendOrPostCallback(this.OndocumentoAggiungiVersioneOperationCompleted);
            this.InvokeAsync("documentoAggiungiVersione", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      }, this.documentoAggiungiVersioneOperationCompleted, userState);
        }

        private void OndocumentoAggiungiVersioneOperationCompleted(object arg)
        {
            if (this.documentoAggiungiVersioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoAggiungiVersioneCompleted((object)this, new documentoAggiungiVersioneCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoRimuoviVersione", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void documentoRimuoviVersione(FileRequest fileRequest, InfoUtente infoUtente)
        {
            this.Invoke("documentoRimuoviVersione", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      });
        }

        public IAsyncResult BegindocumentoRimuoviVersione(FileRequest fileRequest, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoRimuoviVersione", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EnddocumentoRimuoviVersione(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void documentoRimuoviVersioneAsync(FileRequest fileRequest, InfoUtente infoUtente)
        {
            this.documentoRimuoviVersioneAsync(fileRequest, infoUtente, (object)null);
        }

        public void documentoRimuoviVersioneAsync(FileRequest fileRequest, InfoUtente infoUtente, object userState)
        {
            if (this.documentoRimuoviVersioneOperationCompleted == null)
                this.documentoRimuoviVersioneOperationCompleted = new SendOrPostCallback(this.OndocumentoRimuoviVersioneOperationCompleted);
            this.InvokeAsync("documentoRimuoviVersione", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      }, this.documentoRimuoviVersioneOperationCompleted, userState);
        }

        private void OndocumentoRimuoviVersioneOperationCompleted(object arg)
        {
            if (this.documentoRimuoviVersioneCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoRimuoviVersioneCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoModificaAllegato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void documentoModificaAllegato(Allegato allegato, InfoUtente infoUtente)
        {
            this.Invoke("documentoModificaAllegato", new object[2]
      {
        (object) allegato,
        (object) infoUtente
      });
        }

        public IAsyncResult BegindocumentoModificaAllegato(Allegato allegato, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoModificaAllegato", new object[2]
      {
        (object) allegato,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EnddocumentoModificaAllegato(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void documentoModificaAllegatoAsync(Allegato allegato, InfoUtente infoUtente)
        {
            this.documentoModificaAllegatoAsync(allegato, infoUtente, (object)null);
        }

        public void documentoModificaAllegatoAsync(Allegato allegato, InfoUtente infoUtente, object userState)
        {
            if (this.documentoModificaAllegatoOperationCompleted == null)
                this.documentoModificaAllegatoOperationCompleted = new SendOrPostCallback(this.OndocumentoModificaAllegatoOperationCompleted);
            this.InvokeAsync("documentoModificaAllegato", new object[2]
      {
        (object) allegato,
        (object) infoUtente
      }, this.documentoModificaAllegatoOperationCompleted, userState);
        }

        private void OndocumentoModificaAllegatoOperationCompleted(object arg)
        {
            if (this.documentoModificaAllegatoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoModificaAllegatoCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoAggiungiAllegato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Allegato documentoAggiungiAllegato(InfoUtente infoUtente, Allegato allegato)
        {
            return (Allegato)this.Invoke("documentoAggiungiAllegato", new object[2]
      {
        (object) infoUtente,
        (object) allegato
      })[0];
        }

        public IAsyncResult BegindocumentoAggiungiAllegato(InfoUtente infoUtente, Allegato allegato, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoAggiungiAllegato", new object[2]
      {
        (object) infoUtente,
        (object) allegato
      }, callback, asyncState);
        }

        public Allegato EnddocumentoAggiungiAllegato(IAsyncResult asyncResult)
        {
            return (Allegato)this.EndInvoke(asyncResult)[0];
        }

        public void documentoAggiungiAllegatoAsync(InfoUtente infoUtente, Allegato allegato)
        {
            this.documentoAggiungiAllegatoAsync(infoUtente, allegato, (object)null);
        }

        public void documentoAggiungiAllegatoAsync(InfoUtente infoUtente, Allegato allegato, object userState)
        {
            if (this.documentoAggiungiAllegatoOperationCompleted == null)
                this.documentoAggiungiAllegatoOperationCompleted = new SendOrPostCallback(this.OndocumentoAggiungiAllegatoOperationCompleted);
            this.InvokeAsync("documentoAggiungiAllegato", new object[2]
      {
        (object) infoUtente,
        (object) allegato
      }, this.documentoAggiungiAllegatoOperationCompleted, userState);
        }

        private void OndocumentoAggiungiAllegatoOperationCompleted(object arg)
        {
            if (this.documentoAggiungiAllegatoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoAggiungiAllegatoCompleted((object)this, new documentoAggiungiAllegatoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetAllegati", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Allegato[] documentoGetAllegati(InfoDocumento infoDoc, InfoUtente infoUtente)
        {
            return (Allegato[])this.Invoke("documentoGetAllegati", new object[2]
      {
        (object) infoDoc,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetAllegati(InfoDocumento infoDoc, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetAllegati", new object[2]
      {
        (object) infoDoc,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Allegato[] EnddocumentoGetAllegati(IAsyncResult asyncResult)
        {
            return (Allegato[])this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetAllegatiAsync(InfoDocumento infoDoc, InfoUtente infoUtente)
        {
            this.documentoGetAllegatiAsync(infoDoc, infoUtente, (object)null);
        }

        public void documentoGetAllegatiAsync(InfoDocumento infoDoc, InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetAllegatiOperationCompleted == null)
                this.documentoGetAllegatiOperationCompleted = new SendOrPostCallback(this.OndocumentoGetAllegatiOperationCompleted);
            this.InvokeAsync("documentoGetAllegati", new object[2]
      {
        (object) infoDoc,
        (object) infoUtente
      }, this.documentoGetAllegatiOperationCompleted, userState);
        }

        private void OndocumentoGetAllegatiOperationCompleted(object arg)
        {
            if (this.documentoGetAllegatiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetAllegatiCompleted((object)this, new documentoGetAllegatiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoCercaDuplicati", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public bool documentoCercaDuplicati(SchedaDocumento schedaDocumento, InfoUtente infoUtente)
        {
            return (bool)this.Invoke("documentoCercaDuplicati", new object[2]
      {
        (object) schedaDocumento,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoCercaDuplicati(SchedaDocumento schedaDocumento, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoCercaDuplicati", new object[2]
      {
        (object) schedaDocumento,
        (object) infoUtente
      }, callback, asyncState);
        }

        public bool EnddocumentoCercaDuplicati(IAsyncResult asyncResult)
        {
            return (bool)this.EndInvoke(asyncResult)[0];
        }

        public void documentoCercaDuplicatiAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente)
        {
            this.documentoCercaDuplicatiAsync(schedaDocumento, infoUtente, (object)null);
        }

        public void documentoCercaDuplicatiAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, object userState)
        {
            if (this.documentoCercaDuplicatiOperationCompleted == null)
                this.documentoCercaDuplicatiOperationCompleted = new SendOrPostCallback(this.OndocumentoCercaDuplicatiOperationCompleted);
            this.InvokeAsync("documentoCercaDuplicati", new object[2]
      {
        (object) schedaDocumento,
        (object) infoUtente
      }, this.documentoCercaDuplicatiOperationCompleted, userState);
        }

        private void OndocumentoCercaDuplicatiOperationCompleted(object arg)
        {
            if (this.documentoCercaDuplicatiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoCercaDuplicatiCompleted((object)this, new documentoCercaDuplicatiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoVerificaFirma", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FirmaResult documentoVerificaFirma(string base64content, FileRequest fileReq, bool cofirma, InfoUtente infoUtente)
        {
            return (FirmaResult)this.Invoke("documentoVerificaFirma", new object[4]
      {
        (object) base64content,
        (object) fileReq,
        (object) cofirma,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoVerificaFirma(string base64content, FileRequest fileReq, bool cofirma, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoVerificaFirma", new object[4]
      {
        (object) base64content,
        (object) fileReq,
        (object) cofirma,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FirmaResult EnddocumentoVerificaFirma(IAsyncResult asyncResult)
        {
            return (FirmaResult)this.EndInvoke(asyncResult)[0];
        }

        public void documentoVerificaFirmaAsync(string base64content, FileRequest fileReq, bool cofirma, InfoUtente infoUtente)
        {
            this.documentoVerificaFirmaAsync(base64content, fileReq, cofirma, infoUtente, (object)null);
        }

        public void documentoVerificaFirmaAsync(string base64content, FileRequest fileReq, bool cofirma, InfoUtente infoUtente, object userState)
        {
            if (this.documentoVerificaFirmaOperationCompleted == null)
                this.documentoVerificaFirmaOperationCompleted = new SendOrPostCallback(this.OndocumentoVerificaFirmaOperationCompleted);
            this.InvokeAsync("documentoVerificaFirma", new object[4]
      {
        (object) base64content,
        (object) fileReq,
        (object) cofirma,
        (object) infoUtente
      }, this.documentoVerificaFirmaOperationCompleted, userState);
        }

        private void OndocumentoVerificaFirmaOperationCompleted(object arg)
        {
            if (this.documentoVerificaFirmaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoVerificaFirmaCompleted((object)this, new documentoVerificaFirmaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoSaveDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento documentoSaveDocumento(SchedaDocumento schedaDocumento, InfoUtente infoUtente)
        {
            return (SchedaDocumento)this.Invoke("documentoSaveDocumento", new object[2]
      {
        (object) schedaDocumento,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoSaveDocumento(SchedaDocumento schedaDocumento, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoSaveDocumento", new object[2]
      {
        (object) schedaDocumento,
        (object) infoUtente
      }, callback, asyncState);
        }

        public SchedaDocumento EnddocumentoSaveDocumento(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void documentoSaveDocumentoAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente)
        {
            this.documentoSaveDocumentoAsync(schedaDocumento, infoUtente, (object)null);
        }

        public void documentoSaveDocumentoAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, object userState)
        {
            if (this.documentoSaveDocumentoOperationCompleted == null)
                this.documentoSaveDocumentoOperationCompleted = new SendOrPostCallback(this.OndocumentoSaveDocumentoOperationCompleted);
            this.InvokeAsync("documentoSaveDocumento", new object[2]
      {
        (object) schedaDocumento,
        (object) infoUtente
      }, this.documentoSaveDocumentoOperationCompleted, userState);
        }

        private void OndocumentoSaveDocumentoOperationCompleted(object arg)
        {
            if (this.documentoSaveDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoSaveDocumentoCompleted((object)this, new documentoSaveDocumentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoProtocolla", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento documentoProtocolla(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo)
        {
            return (SchedaDocumento)this.Invoke("documentoProtocolla", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) ruolo
      })[0];
        }

        public IAsyncResult BegindocumentoProtocolla(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoProtocolla", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) ruolo
      }, callback, asyncState);
        }

        public SchedaDocumento EnddocumentoProtocolla(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void documentoProtocollaAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo)
        {
            this.documentoProtocollaAsync(schedaDocumento, infoUtente, ruolo, (object)null);
        }

        public void documentoProtocollaAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo, object userState)
        {
            if (this.documentoProtocollaOperationCompleted == null)
                this.documentoProtocollaOperationCompleted = new SendOrPostCallback(this.OndocumentoProtocollaOperationCompleted);
            this.InvokeAsync("documentoProtocolla", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) ruolo
      }, this.documentoProtocollaOperationCompleted, userState);
        }

        private void OndocumentoProtocollaOperationCompleted(object arg)
        {
            if (this.documentoProtocollaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoProtocollaCompleted((object)this, new documentoProtocollaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetDettaglioDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento documentoGetDettaglioDocumento(InfoDocumento infoDocumento, InfoUtente infoUtente)
        {
            return (SchedaDocumento)this.Invoke("documentoGetDettaglioDocumento", new object[2]
      {
        (object) infoDocumento,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetDettaglioDocumento(InfoDocumento infoDocumento, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetDettaglioDocumento", new object[2]
      {
        (object) infoDocumento,
        (object) infoUtente
      }, callback, asyncState);
        }

        public SchedaDocumento EnddocumentoGetDettaglioDocumento(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetDettaglioDocumentoAsync(InfoDocumento infoDocumento, InfoUtente infoUtente)
        {
            this.documentoGetDettaglioDocumentoAsync(infoDocumento, infoUtente, (object)null);
        }

        public void documentoGetDettaglioDocumentoAsync(InfoDocumento infoDocumento, InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetDettaglioDocumentoOperationCompleted == null)
                this.documentoGetDettaglioDocumentoOperationCompleted = new SendOrPostCallback(this.OndocumentoGetDettaglioDocumentoOperationCompleted);
            this.InvokeAsync("documentoGetDettaglioDocumento", new object[2]
      {
        (object) infoDocumento,
        (object) infoUtente
      }, this.documentoGetDettaglioDocumentoOperationCompleted, userState);
        }

        private void OndocumentoGetDettaglioDocumentoOperationCompleted(object arg)
        {
            if (this.documentoGetDettaglioDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetDettaglioDocumentoCompleted((object)this, new documentoGetDettaglioDocumentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/MDDGetCodStato", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public MDDCodStato[] MDDGetCodStato(InfoUtente infoUtente)
        {
            return (MDDCodStato[])this.Invoke("MDDGetCodStato", new object[1]
      {
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginMDDGetCodStato(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("MDDGetCodStato", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public MDDCodStato[] EndMDDGetCodStato(IAsyncResult asyncResult)
        {
            return (MDDCodStato[])this.EndInvoke(asyncResult)[0];
        }

        public void MDDGetCodStatoAsync(InfoUtente infoUtente)
        {
            this.MDDGetCodStatoAsync(infoUtente, (object)null);
        }

        public void MDDGetCodStatoAsync(InfoUtente infoUtente, object userState)
        {
            if (this.MDDGetCodStatoOperationCompleted == null)
                this.MDDGetCodStatoOperationCompleted = new SendOrPostCallback(this.OnMDDGetCodStatoOperationCompleted);
            this.InvokeAsync("MDDGetCodStato", new object[1]
      {
        (object) infoUtente
      }, this.MDDGetCodStatoOperationCompleted, userState);
        }

        private void OnMDDGetCodStatoOperationCompleted(object arg)
        {
            if (this.MDDGetCodStatoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.MDDGetCodStatoCompleted((object)this, new MDDGetCodStatoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetCategoriaAtto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public CategoriaAtto[] documentoGetCategoriaAtto(InfoUtente infoUtente)
        {
            return (CategoriaAtto[])this.Invoke("documentoGetCategoriaAtto", new object[1]
      {
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetCategoriaAtto(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetCategoriaAtto", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public CategoriaAtto[] EnddocumentoGetCategoriaAtto(IAsyncResult asyncResult)
        {
            return (CategoriaAtto[])this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetCategoriaAttoAsync(InfoUtente infoUtente)
        {
            this.documentoGetCategoriaAttoAsync(infoUtente, (object)null);
        }

        public void documentoGetCategoriaAttoAsync(InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetCategoriaAttoOperationCompleted == null)
                this.documentoGetCategoriaAttoOperationCompleted = new SendOrPostCallback(this.OndocumentoGetCategoriaAttoOperationCompleted);
            this.InvokeAsync("documentoGetCategoriaAtto", new object[1]
      {
        (object) infoUtente
      }, this.documentoGetCategoriaAttoOperationCompleted, userState);
        }

        private void OndocumentoGetCategoriaAttoOperationCompleted(object arg)
        {
            if (this.documentoGetCategoriaAttoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetCategoriaAttoCompleted((object)this, new documentoGetCategoriaAttoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetTipologiaAtto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TipologiaAtto[] documentoGetTipologiaAtto(InfoUtente infoUtente)
        {
            return (TipologiaAtto[])this.Invoke("documentoGetTipologiaAtto", new object[1]
      {
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetTipologiaAtto(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetTipologiaAtto", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public TipologiaAtto[] EnddocumentoGetTipologiaAtto(IAsyncResult asyncResult)
        {
            return (TipologiaAtto[])this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetTipologiaAttoAsync(InfoUtente infoUtente)
        {
            this.documentoGetTipologiaAttoAsync(infoUtente, (object)null);
        }

        public void documentoGetTipologiaAttoAsync(InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetTipologiaAttoOperationCompleted == null)
                this.documentoGetTipologiaAttoOperationCompleted = new SendOrPostCallback(this.OndocumentoGetTipologiaAttoOperationCompleted);
            this.InvokeAsync("documentoGetTipologiaAtto", new object[1]
      {
        (object) infoUtente
      }, this.documentoGetTipologiaAttoOperationCompleted, userState);
        }

        private void OndocumentoGetTipologiaAttoOperationCompleted(object arg)
        {
            if (this.documentoGetTipologiaAttoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetTipologiaAttoCompleted((object)this, new documentoGetTipologiaAttoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoAddParolaChiave", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DocumentoParolaChiave documentoAddParolaChiave(DocumentoParolaChiave parolaC, InfoUtente infoUtente)
        {
            return (DocumentoParolaChiave)this.Invoke("documentoAddParolaChiave", new object[2]
      {
        (object) parolaC,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoAddParolaChiave(DocumentoParolaChiave parolaC, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoAddParolaChiave", new object[2]
      {
        (object) parolaC,
        (object) infoUtente
      }, callback, asyncState);
        }

        public DocumentoParolaChiave EnddocumentoAddParolaChiave(IAsyncResult asyncResult)
        {
            return (DocumentoParolaChiave)this.EndInvoke(asyncResult)[0];
        }

        public void documentoAddParolaChiaveAsync(DocumentoParolaChiave parolaC, InfoUtente infoUtente)
        {
            this.documentoAddParolaChiaveAsync(parolaC, infoUtente, (object)null);
        }

        public void documentoAddParolaChiaveAsync(DocumentoParolaChiave parolaC, InfoUtente infoUtente, object userState)
        {
            if (this.documentoAddParolaChiaveOperationCompleted == null)
                this.documentoAddParolaChiaveOperationCompleted = new SendOrPostCallback(this.OndocumentoAddParolaChiaveOperationCompleted);
            this.InvokeAsync("documentoAddParolaChiave", new object[2]
      {
        (object) parolaC,
        (object) infoUtente
      }, this.documentoAddParolaChiaveOperationCompleted, userState);
        }

        private void OndocumentoAddParolaChiaveOperationCompleted(object arg)
        {
            if (this.documentoAddParolaChiaveCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoAddParolaChiaveCompleted((object)this, new documentoAddParolaChiaveCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetParoleChiave", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public DocumentoParolaChiave[] documentoGetParoleChiave(InfoUtente infoUtente)
        {
            return (DocumentoParolaChiave[])this.Invoke("documentoGetParoleChiave", new object[1]
      {
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetParoleChiave(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetParoleChiave", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public DocumentoParolaChiave[] EnddocumentoGetParoleChiave(IAsyncResult asyncResult)
        {
            return (DocumentoParolaChiave[])this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetParoleChiaveAsync(InfoUtente infoUtente)
        {
            this.documentoGetParoleChiaveAsync(infoUtente, (object)null);
        }

        public void documentoGetParoleChiaveAsync(InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetParoleChiaveOperationCompleted == null)
                this.documentoGetParoleChiaveOperationCompleted = new SendOrPostCallback(this.OndocumentoGetParoleChiaveOperationCompleted);
            this.InvokeAsync("documentoGetParoleChiave", new object[1]
      {
        (object) infoUtente
      }, this.documentoGetParoleChiaveOperationCompleted, userState);
        }

        private void OndocumentoGetParoleChiaveOperationCompleted(object arg)
        {
            if (this.documentoGetParoleChiaveCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetParoleChiaveCompleted((object)this, new documentoGetParoleChiaveCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoAddOggetto", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Oggetto documentoAddOggetto(Oggetto oggetto, Registro registro, InfoUtente infoUtente)
        {
            return (Oggetto)this.Invoke("documentoAddOggetto", new object[3]
      {
        (object) oggetto,
        (object) registro,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoAddOggetto(Oggetto oggetto, Registro registro, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoAddOggetto", new object[3]
      {
        (object) oggetto,
        (object) registro,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Oggetto EnddocumentoAddOggetto(IAsyncResult asyncResult)
        {
            return (Oggetto)this.EndInvoke(asyncResult)[0];
        }

        public void documentoAddOggettoAsync(Oggetto oggetto, Registro registro, InfoUtente infoUtente)
        {
            this.documentoAddOggettoAsync(oggetto, registro, infoUtente, (object)null);
        }

        public void documentoAddOggettoAsync(Oggetto oggetto, Registro registro, InfoUtente infoUtente, object userState)
        {
            if (this.documentoAddOggettoOperationCompleted == null)
                this.documentoAddOggettoOperationCompleted = new SendOrPostCallback(this.OndocumentoAddOggettoOperationCompleted);
            this.InvokeAsync("documentoAddOggetto", new object[3]
      {
        (object) oggetto,
        (object) registro,
        (object) infoUtente
      }, this.documentoAddOggettoOperationCompleted, userState);
        }

        private void OndocumentoAddOggettoOperationCompleted(object arg)
        {
            if (this.documentoAddOggettoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoAddOggettoCompleted((object)this, new documentoAddOggettoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetListaOggetti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Oggetto[] documentoGetListaOggetti(DocumentoQueryOggetto queryOggetto, InfoUtente infoUtente)
        {
            return (Oggetto[])this.Invoke("documentoGetListaOggetti", new object[2]
      {
        (object) queryOggetto,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetListaOggetti(DocumentoQueryOggetto queryOggetto, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetListaOggetti", new object[2]
      {
        (object) queryOggetto,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Oggetto[] EnddocumentoGetListaOggetti(IAsyncResult asyncResult)
        {
            return (Oggetto[])this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetListaOggettiAsync(DocumentoQueryOggetto queryOggetto, InfoUtente infoUtente)
        {
            this.documentoGetListaOggettiAsync(queryOggetto, infoUtente, (object)null);
        }

        public void documentoGetListaOggettiAsync(DocumentoQueryOggetto queryOggetto, InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetListaOggettiOperationCompleted == null)
                this.documentoGetListaOggettiOperationCompleted = new SendOrPostCallback(this.OndocumentoGetListaOggettiOperationCompleted);
            this.InvokeAsync("documentoGetListaOggetti", new object[2]
      {
        (object) queryOggetto,
        (object) infoUtente
      }, this.documentoGetListaOggettiOperationCompleted, userState);
        }

        private void OndocumentoGetListaOggettiOperationCompleted(object arg)
        {
            if (this.documentoGetListaOggettiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetListaOggettiCompleted((object)this, new documentoGetListaOggettiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoPutFile", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileRequest documentoPutFile(FileRequest fileRequest, FileDocumento fileDocument, InfoUtente infoUtente)
        {
            return (FileRequest)this.Invoke("documentoPutFile", new object[3]
      {
        (object) fileRequest,
        (object) fileDocument,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoPutFile(FileRequest fileRequest, FileDocumento fileDocument, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoPutFile", new object[3]
      {
        (object) fileRequest,
        (object) fileDocument,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FileRequest EnddocumentoPutFile(IAsyncResult asyncResult)
        {
            return (FileRequest)this.EndInvoke(asyncResult)[0];
        }

        public void documentoPutFileAsync(FileRequest fileRequest, FileDocumento fileDocument, InfoUtente infoUtente)
        {
            this.documentoPutFileAsync(fileRequest, fileDocument, infoUtente, (object)null);
        }

        public void documentoPutFileAsync(FileRequest fileRequest, FileDocumento fileDocument, InfoUtente infoUtente, object userState)
        {
            if (this.documentoPutFileOperationCompleted == null)
                this.documentoPutFileOperationCompleted = new SendOrPostCallback(this.OndocumentoPutFileOperationCompleted);
            this.InvokeAsync("documentoPutFile", new object[3]
      {
        (object) fileRequest,
        (object) fileDocument,
        (object) infoUtente
      }, this.documentoPutFileOperationCompleted, userState);
        }

        private void OndocumentoPutFileOperationCompleted(object arg)
        {
            if (this.documentoPutFileCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoPutFileCompleted((object)this, new documentoPutFileCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetFile", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileDocumento documentoGetFile(FileRequest fileRequest, InfoUtente infoUtente)
        {
            return (FileDocumento)this.Invoke("documentoGetFile", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetFile(FileRequest fileRequest, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetFile", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FileDocumento EnddocumentoGetFile(IAsyncResult asyncResult)
        {
            return (FileDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetFileAsync(FileRequest fileRequest, InfoUtente infoUtente)
        {
            this.documentoGetFileAsync(fileRequest, infoUtente, (object)null);
        }

        public void documentoGetFileAsync(FileRequest fileRequest, InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetFileOperationCompleted == null)
                this.documentoGetFileOperationCompleted = new SendOrPostCallback(this.OndocumentoGetFileOperationCompleted);
            this.InvokeAsync("documentoGetFile", new object[2]
      {
        (object) fileRequest,
        (object) infoUtente
      }, this.documentoGetFileOperationCompleted, userState);
        }

        private void OndocumentoGetFileOperationCompleted(object arg)
        {
            if (this.documentoGetFileCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetFileCompleted((object)this, new documentoGetFileCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoAddDocGrigia", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public SchedaDocumento documentoAddDocGrigia(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo)
        {
            return (SchedaDocumento)this.Invoke("documentoAddDocGrigia", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) ruolo
      })[0];
        }

        public IAsyncResult BegindocumentoAddDocGrigia(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoAddDocGrigia", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) ruolo
      }, callback, asyncState);
        }

        public SchedaDocumento EnddocumentoAddDocGrigia(IAsyncResult asyncResult)
        {
            return (SchedaDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void documentoAddDocGrigiaAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo)
        {
            this.documentoAddDocGrigiaAsync(schedaDocumento, infoUtente, ruolo, (object)null);
        }

        public void documentoAddDocGrigiaAsync(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo, object userState)
        {
            if (this.documentoAddDocGrigiaOperationCompleted == null)
                this.documentoAddDocGrigiaOperationCompleted = new SendOrPostCallback(this.OndocumentoAddDocGrigiaOperationCompleted);
            this.InvokeAsync("documentoAddDocGrigia", new object[3]
      {
        (object) schedaDocumento,
        (object) infoUtente,
        (object) ruolo
      }, this.documentoAddDocGrigiaOperationCompleted, userState);
        }

        private void OndocumentoAddDocGrigiaOperationCompleted(object arg)
        {
            if (this.documentoAddDocGrigiaCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoAddDocGrigiaCompleted((object)this, new documentoAddDocGrigiaCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetAreaLavoroPaging", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public AreaLavoro documentoGetAreaLavoroPaging(AreaLavoroQueryAreaLavoro queryAreaLavoro, InfoUtente infoUtente, int pagina, int righe, ref int risultati, ref int rtot)
        {
            object[] objArray = this.Invoke("documentoGetAreaLavoroPaging", new object[6]
      {
        (object) queryAreaLavoro,
        (object) infoUtente,
        (object) pagina,
        (object) righe,
        (object) risultati,
        (object) rtot
      });
            risultati = (int)objArray[1];
            rtot = (int)objArray[2];
            return (AreaLavoro)objArray[0];
        }

        public IAsyncResult BegindocumentoGetAreaLavoroPaging(AreaLavoroQueryAreaLavoro queryAreaLavoro, InfoUtente infoUtente, int pagina, int righe, int risultati, int rtot, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetAreaLavoroPaging", new object[6]
      {
        (object) queryAreaLavoro,
        (object) infoUtente,
        (object) pagina,
        (object) righe,
        (object) risultati,
        (object) rtot
      }, callback, asyncState);
        }

        public AreaLavoro EnddocumentoGetAreaLavoroPaging(IAsyncResult asyncResult, out int risultati, out int rtot)
        {
            object[] objArray = this.EndInvoke(asyncResult);
            risultati = (int)objArray[1];
            rtot = (int)objArray[2];
            return (AreaLavoro)objArray[0];
        }

        public void documentoGetAreaLavoroPagingAsync(AreaLavoroQueryAreaLavoro queryAreaLavoro, InfoUtente infoUtente, int pagina, int righe, int risultati, int rtot)
        {
            this.documentoGetAreaLavoroPagingAsync(queryAreaLavoro, infoUtente, pagina, righe, risultati, rtot, (object)null);
        }

        public void documentoGetAreaLavoroPagingAsync(AreaLavoroQueryAreaLavoro queryAreaLavoro, InfoUtente infoUtente, int pagina, int righe, int risultati, int rtot, object userState)
        {
            if (this.documentoGetAreaLavoroPagingOperationCompleted == null)
                this.documentoGetAreaLavoroPagingOperationCompleted = new SendOrPostCallback(this.OndocumentoGetAreaLavoroPagingOperationCompleted);
            this.InvokeAsync("documentoGetAreaLavoroPaging", new object[6]
      {
        (object) queryAreaLavoro,
        (object) infoUtente,
        (object) pagina,
        (object) righe,
        (object) risultati,
        (object) rtot
      }, this.documentoGetAreaLavoroPagingOperationCompleted, userState);
        }

        private void OndocumentoGetAreaLavoroPagingOperationCompleted(object arg)
        {
            if (this.documentoGetAreaLavoroPagingCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetAreaLavoroPagingCompleted((object)this, new documentoGetAreaLavoroPagingCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetSegnaturaCampiVariabili", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string documentoGetSegnaturaCampiVariabili(SchedaDocumento schedaDocumento, string[] campiVariabili, InfoUtente infoUtente)
        {
            return (string)this.Invoke("documentoGetSegnaturaCampiVariabili", new object[3]
      {
        (object) schedaDocumento,
        (object) campiVariabili,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetSegnaturaCampiVariabili(SchedaDocumento schedaDocumento, string[] campiVariabili, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetSegnaturaCampiVariabili", new object[3]
      {
        (object) schedaDocumento,
        (object) campiVariabili,
        (object) infoUtente
      }, callback, asyncState);
        }

        public string EnddocumentoGetSegnaturaCampiVariabili(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetSegnaturaCampiVariabiliAsync(SchedaDocumento schedaDocumento, string[] campiVariabili, InfoUtente infoUtente)
        {
            this.documentoGetSegnaturaCampiVariabiliAsync(schedaDocumento, campiVariabili, infoUtente, (object)null);
        }

        public void documentoGetSegnaturaCampiVariabiliAsync(SchedaDocumento schedaDocumento, string[] campiVariabili, InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetSegnaturaCampiVariabiliOperationCompleted == null)
                this.documentoGetSegnaturaCampiVariabiliOperationCompleted = new SendOrPostCallback(this.OndocumentoGetSegnaturaCampiVariabiliOperationCompleted);
            this.InvokeAsync("documentoGetSegnaturaCampiVariabili", new object[3]
      {
        (object) schedaDocumento,
        (object) campiVariabili,
        (object) infoUtente
      }, this.documentoGetSegnaturaCampiVariabiliOperationCompleted, userState);
        }

        private void OndocumentoGetSegnaturaCampiVariabiliOperationCompleted(object arg)
        {
            if (this.documentoGetSegnaturaCampiVariabiliCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetSegnaturaCampiVariabiliCompleted((object)this, new documentoGetSegnaturaCampiVariabiliCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetQueryDocumento", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public InfoDocumento[] documentoGetQueryDocumento([XmlArrayItem("ArrayOfFiltroRicerca"), XmlArrayItem(NestingLevel = 1)] FiltroRicerca[][] queryList, InfoUtente infoUtente)
        {
            return (InfoDocumento[])this.Invoke("documentoGetQueryDocumento", new object[2]
      {
        (object) queryList,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetQueryDocumento(FiltroRicerca[][] queryList, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetQueryDocumento", new object[2]
      {
        (object) queryList,
        (object) infoUtente
      }, callback, asyncState);
        }

        public InfoDocumento[] EnddocumentoGetQueryDocumento(IAsyncResult asyncResult)
        {
            return (InfoDocumento[])this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetQueryDocumentoAsync(FiltroRicerca[][] queryList, InfoUtente infoUtente)
        {
            this.documentoGetQueryDocumentoAsync(queryList, infoUtente, (object)null);
        }

        public void documentoGetQueryDocumentoAsync(FiltroRicerca[][] queryList, InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetQueryDocumentoOperationCompleted == null)
                this.documentoGetQueryDocumentoOperationCompleted = new SendOrPostCallback(this.OndocumentoGetQueryDocumentoOperationCompleted);
            this.InvokeAsync("documentoGetQueryDocumento", new object[2]
      {
        (object) queryList,
        (object) infoUtente
      }, this.documentoGetQueryDocumentoOperationCompleted, userState);
        }

        private void OndocumentoGetQueryDocumentoOperationCompleted(object arg)
        {
            if (this.documentoGetQueryDocumentoCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetQueryDocumentoCompleted((object)this, new documentoGetQueryDocumentoCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/addressbookGetCanali", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Canale[] addressbookGetCanali(InfoUtente infoUtente)
        {
            return (Canale[])this.Invoke("addressbookGetCanali", new object[1]
      {
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginaddressbookGetCanali(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("addressbookGetCanali", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public Canale[] EndaddressbookGetCanali(IAsyncResult asyncResult)
        {
            return (Canale[])this.EndInvoke(asyncResult)[0];
        }

        public void addressbookGetCanaliAsync(InfoUtente infoUtente)
        {
            this.addressbookGetCanaliAsync(infoUtente, (object)null);
        }

        public void addressbookGetCanaliAsync(InfoUtente infoUtente, object userState)
        {
            if (this.addressbookGetCanaliOperationCompleted == null)
                this.addressbookGetCanaliOperationCompleted = new SendOrPostCallback(this.OnaddressbookGetCanaliOperationCompleted);
            this.InvokeAsync("addressbookGetCanali", new object[1]
      {
        (object) infoUtente
      }, this.addressbookGetCanaliOperationCompleted, userState);
        }

        private void OnaddressbookGetCanaliOperationCompleted(object arg)
        {
            if (this.addressbookGetCanaliCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.addressbookGetCanaliCompleted((object)this, new addressbookGetCanaliCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/addressbookGetRuoliSuperioriInUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Ruolo[] addressbookGetRuoliSuperioriInUO(Ruolo ruolo, InfoUtente infoUtente)
        {
            return (Ruolo[])this.Invoke("addressbookGetRuoliSuperioriInUO", new object[2]
      {
        (object) ruolo,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginaddressbookGetRuoliSuperioriInUO(Ruolo ruolo, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("addressbookGetRuoliSuperioriInUO", new object[2]
      {
        (object) ruolo,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Ruolo[] EndaddressbookGetRuoliSuperioriInUO(IAsyncResult asyncResult)
        {
            return (Ruolo[])this.EndInvoke(asyncResult)[0];
        }

        public void addressbookGetRuoliSuperioriInUOAsync(Ruolo ruolo, InfoUtente infoUtente)
        {
            this.addressbookGetRuoliSuperioriInUOAsync(ruolo, infoUtente, (object)null);
        }

        public void addressbookGetRuoliSuperioriInUOAsync(Ruolo ruolo, InfoUtente infoUtente, object userState)
        {
            if (this.addressbookGetRuoliSuperioriInUOOperationCompleted == null)
                this.addressbookGetRuoliSuperioriInUOOperationCompleted = new SendOrPostCallback(this.OnaddressbookGetRuoliSuperioriInUOOperationCompleted);
            this.InvokeAsync("addressbookGetRuoliSuperioriInUO", new object[2]
      {
        (object) ruolo,
        (object) infoUtente
      }, this.addressbookGetRuoliSuperioriInUOOperationCompleted, userState);
        }

        private void OnaddressbookGetRuoliSuperioriInUOOperationCompleted(object arg)
        {
            if (this.addressbookGetRuoliSuperioriInUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.addressbookGetRuoliSuperioriInUOCompleted((object)this, new addressbookGetRuoliSuperioriInUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/addressbookInsertCorrispondente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public AddressbookRisultatoInsert addressbookInsertCorrispondente(Corrispondente corrispondente, Corrispondente parent, InfoUtente infoUtente)
        {
            return (AddressbookRisultatoInsert)this.Invoke("addressbookInsertCorrispondente", new object[3]
      {
        (object) corrispondente,
        (object) parent,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginaddressbookInsertCorrispondente(Corrispondente corrispondente, Corrispondente parent, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("addressbookInsertCorrispondente", new object[3]
      {
        (object) corrispondente,
        (object) parent,
        (object) infoUtente
      }, callback, asyncState);
        }

        public AddressbookRisultatoInsert EndaddressbookInsertCorrispondente(IAsyncResult asyncResult)
        {
            return (AddressbookRisultatoInsert)this.EndInvoke(asyncResult)[0];
        }

        public void addressbookInsertCorrispondenteAsync(Corrispondente corrispondente, Corrispondente parent, InfoUtente infoUtente)
        {
            this.addressbookInsertCorrispondenteAsync(corrispondente, parent, infoUtente, (object)null);
        }

        public void addressbookInsertCorrispondenteAsync(Corrispondente corrispondente, Corrispondente parent, InfoUtente infoUtente, object userState)
        {
            if (this.addressbookInsertCorrispondenteOperationCompleted == null)
                this.addressbookInsertCorrispondenteOperationCompleted = new SendOrPostCallback(this.OnaddressbookInsertCorrispondenteOperationCompleted);
            this.InvokeAsync("addressbookInsertCorrispondente", new object[3]
      {
        (object) corrispondente,
        (object) parent,
        (object) infoUtente
      }, this.addressbookInsertCorrispondenteOperationCompleted, userState);
        }

        private void OnaddressbookInsertCorrispondenteOperationCompleted(object arg)
        {
            if (this.addressbookInsertCorrispondenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.addressbookInsertCorrispondenteCompleted((object)this, new addressbookInsertCorrispondenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/addressbookGetRootUO", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public UnitaOrganizzativa[] addressbookGetRootUO(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente)
        {
            return (UnitaOrganizzativa[])this.Invoke("addressbookGetRootUO", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginaddressbookGetRootUO(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("addressbookGetRootUO", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      }, callback, asyncState);
        }

        public UnitaOrganizzativa[] EndaddressbookGetRootUO(IAsyncResult asyncResult)
        {
            return (UnitaOrganizzativa[])this.EndInvoke(asyncResult)[0];
        }

        public void addressbookGetRootUOAsync(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente)
        {
            this.addressbookGetRootUOAsync(queryCorrispondente, infoUtente, (object)null);
        }

        public void addressbookGetRootUOAsync(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente, object userState)
        {
            if (this.addressbookGetRootUOOperationCompleted == null)
                this.addressbookGetRootUOOperationCompleted = new SendOrPostCallback(this.OnaddressbookGetRootUOOperationCompleted);
            this.InvokeAsync("addressbookGetRootUO", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      }, this.addressbookGetRootUOOperationCompleted, userState);
        }

        private void OnaddressbookGetRootUOOperationCompleted(object arg)
        {
            if (this.addressbookGetRootUOCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.addressbookGetRootUOCompleted((object)this, new addressbookGetRootUOCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/addressbookGetListaCorrispondentiAutorizzati", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente[] addressbookGetListaCorrispondentiAutorizzati(AddressbookQueryCorrispondenteAutorizzato queryCorrispondenteAutorizzato, InfoUtente infoUtente)
        {
            return (Corrispondente[])this.Invoke("addressbookGetListaCorrispondentiAutorizzati", new object[2]
      {
        (object) queryCorrispondenteAutorizzato,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginaddressbookGetListaCorrispondentiAutorizzati(AddressbookQueryCorrispondenteAutorizzato queryCorrispondenteAutorizzato, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("addressbookGetListaCorrispondentiAutorizzati", new object[2]
      {
        (object) queryCorrispondenteAutorizzato,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Corrispondente[] EndaddressbookGetListaCorrispondentiAutorizzati(IAsyncResult asyncResult)
        {
            return (Corrispondente[])this.EndInvoke(asyncResult)[0];
        }

        public void addressbookGetListaCorrispondentiAutorizzatiAsync(AddressbookQueryCorrispondenteAutorizzato queryCorrispondenteAutorizzato, InfoUtente infoUtente)
        {
            this.addressbookGetListaCorrispondentiAutorizzatiAsync(queryCorrispondenteAutorizzato, infoUtente, (object)null);
        }

        public void addressbookGetListaCorrispondentiAutorizzatiAsync(AddressbookQueryCorrispondenteAutorizzato queryCorrispondenteAutorizzato, InfoUtente infoUtente, object userState)
        {
            if (this.addressbookGetListaCorrispondentiAutorizzatiOperationCompleted == null)
                this.addressbookGetListaCorrispondentiAutorizzatiOperationCompleted = new SendOrPostCallback(this.OnaddressbookGetListaCorrispondentiAutorizzatiOperationCompleted);
            this.InvokeAsync("addressbookGetListaCorrispondentiAutorizzati", new object[2]
      {
        (object) queryCorrispondenteAutorizzato,
        (object) infoUtente
      }, this.addressbookGetListaCorrispondentiAutorizzatiOperationCompleted, userState);
        }

        private void OnaddressbookGetListaCorrispondentiAutorizzatiOperationCompleted(object arg)
        {
            if (this.addressbookGetListaCorrispondentiAutorizzatiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.addressbookGetListaCorrispondentiAutorizzatiCompleted((object)this, new addressbookGetListaCorrispondentiAutorizzatiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/addressbookGetDettagliCorrispondente", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public AddressbookDettagliCorrispondente addressbookGetDettagliCorrispondente(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente)
        {
            return (AddressbookDettagliCorrispondente)this.Invoke("addressbookGetDettagliCorrispondente", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginaddressbookGetDettagliCorrispondente(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("addressbookGetDettagliCorrispondente", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      }, callback, asyncState);
        }

        public AddressbookDettagliCorrispondente EndaddressbookGetDettagliCorrispondente(IAsyncResult asyncResult)
        {
            return (AddressbookDettagliCorrispondente)this.EndInvoke(asyncResult)[0];
        }

        public void addressbookGetDettagliCorrispondenteAsync(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente)
        {
            this.addressbookGetDettagliCorrispondenteAsync(queryCorrispondente, infoUtente, (object)null);
        }

        public void addressbookGetDettagliCorrispondenteAsync(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente, object userState)
        {
            if (this.addressbookGetDettagliCorrispondenteOperationCompleted == null)
                this.addressbookGetDettagliCorrispondenteOperationCompleted = new SendOrPostCallback(this.OnaddressbookGetDettagliCorrispondenteOperationCompleted);
            this.InvokeAsync("addressbookGetDettagliCorrispondente", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      }, this.addressbookGetDettagliCorrispondenteOperationCompleted, userState);
        }

        private void OnaddressbookGetDettagliCorrispondenteOperationCompleted(object arg)
        {
            if (this.addressbookGetDettagliCorrispondenteCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.addressbookGetDettagliCorrispondenteCompleted((object)this, new addressbookGetDettagliCorrispondenteCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/addressbookGetListaCorrispondenti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Corrispondente[] addressbookGetListaCorrispondenti(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente)
        {
            return (Corrispondente[])this.Invoke("addressbookGetListaCorrispondenti", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginaddressbookGetListaCorrispondenti(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("addressbookGetListaCorrispondenti", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      }, callback, asyncState);
        }

        public Corrispondente[] EndaddressbookGetListaCorrispondenti(IAsyncResult asyncResult)
        {
            return (Corrispondente[])this.EndInvoke(asyncResult)[0];
        }

        public void addressbookGetListaCorrispondentiAsync(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente)
        {
            this.addressbookGetListaCorrispondentiAsync(queryCorrispondente, infoUtente, (object)null);
        }

        public void addressbookGetListaCorrispondentiAsync(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente, object userState)
        {
            if (this.addressbookGetListaCorrispondentiOperationCompleted == null)
                this.addressbookGetListaCorrispondentiOperationCompleted = new SendOrPostCallback(this.OnaddressbookGetListaCorrispondentiOperationCompleted);
            this.InvokeAsync("addressbookGetListaCorrispondenti", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      }, this.addressbookGetListaCorrispondentiOperationCompleted, userState);
        }

        private void OnaddressbookGetListaCorrispondentiOperationCompleted(object arg)
        {
            if (this.addressbookGetListaCorrispondentiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.addressbookGetListaCorrispondentiCompleted((object)this, new addressbookGetListaCorrispondentiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoCancellaAreaLavoro", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void documentoCancellaAreaLavoro(InfoDocumento infoDoc, Fascicolo fasc, InfoUtente infoUtente)
        {
            this.Invoke("documentoCancellaAreaLavoro", new object[3]
      {
        (object) infoDoc,
        (object) fasc,
        (object) infoUtente
      });
        }

        public IAsyncResult BegindocumentoCancellaAreaLavoro(InfoDocumento infoDoc, Fascicolo fasc, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoCancellaAreaLavoro", new object[3]
      {
        (object) infoDoc,
        (object) fasc,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EnddocumentoCancellaAreaLavoro(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void documentoCancellaAreaLavoroAsync(InfoDocumento infoDoc, Fascicolo fasc, InfoUtente infoUtente)
        {
            this.documentoCancellaAreaLavoroAsync(infoDoc, fasc, infoUtente, (object)null);
        }

        public void documentoCancellaAreaLavoroAsync(InfoDocumento infoDoc, Fascicolo fasc, InfoUtente infoUtente, object userState)
        {
            if (this.documentoCancellaAreaLavoroOperationCompleted == null)
                this.documentoCancellaAreaLavoroOperationCompleted = new SendOrPostCallback(this.OndocumentoCancellaAreaLavoroOperationCompleted);
            this.InvokeAsync("documentoCancellaAreaLavoro", new object[3]
      {
        (object) infoDoc,
        (object) fasc,
        (object) infoUtente
      }, this.documentoCancellaAreaLavoroOperationCompleted, userState);
        }

        private void OndocumentoCancellaAreaLavoroOperationCompleted(object arg)
        {
            if (this.documentoCancellaAreaLavoroCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoCancellaAreaLavoroCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetAreaLavoro", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public AreaLavoro documentoGetAreaLavoro(AreaLavoroQueryAreaLavoro queryAreaLavoro, InfoUtente infoUtente)
        {
            return (AreaLavoro)this.Invoke("documentoGetAreaLavoro", new object[2]
      {
        (object) queryAreaLavoro,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetAreaLavoro(AreaLavoroQueryAreaLavoro queryAreaLavoro, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetAreaLavoro", new object[2]
      {
        (object) queryAreaLavoro,
        (object) infoUtente
      }, callback, asyncState);
        }

        public AreaLavoro EnddocumentoGetAreaLavoro(IAsyncResult asyncResult)
        {
            return (AreaLavoro)this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetAreaLavoroAsync(AreaLavoroQueryAreaLavoro queryAreaLavoro, InfoUtente infoUtente)
        {
            this.documentoGetAreaLavoroAsync(queryAreaLavoro, infoUtente, (object)null);
        }

        public void documentoGetAreaLavoroAsync(AreaLavoroQueryAreaLavoro queryAreaLavoro, InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetAreaLavoroOperationCompleted == null)
                this.documentoGetAreaLavoroOperationCompleted = new SendOrPostCallback(this.OndocumentoGetAreaLavoroOperationCompleted);
            this.InvokeAsync("documentoGetAreaLavoro", new object[2]
      {
        (object) queryAreaLavoro,
        (object) infoUtente
      }, this.documentoGetAreaLavoroOperationCompleted, userState);
        }

        private void OndocumentoGetAreaLavoroOperationCompleted(object arg)
        {
            if (this.documentoGetAreaLavoroCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetAreaLavoroCompleted((object)this, new documentoGetAreaLavoroCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoGetApplicazioni", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Applicazione[] documentoGetApplicazioni(InfoUtente infoUtente)
        {
            return (Applicazione[])this.Invoke("documentoGetApplicazioni", new object[1]
      {
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegindocumentoGetApplicazioni(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoGetApplicazioni", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public Applicazione[] EnddocumentoGetApplicazioni(IAsyncResult asyncResult)
        {
            return (Applicazione[])this.EndInvoke(asyncResult)[0];
        }

        public void documentoGetApplicazioniAsync(InfoUtente infoUtente)
        {
            this.documentoGetApplicazioniAsync(infoUtente, (object)null);
        }

        public void documentoGetApplicazioniAsync(InfoUtente infoUtente, object userState)
        {
            if (this.documentoGetApplicazioniOperationCompleted == null)
                this.documentoGetApplicazioniOperationCompleted = new SendOrPostCallback(this.OndocumentoGetApplicazioniOperationCompleted);
            this.InvokeAsync("documentoGetApplicazioni", new object[1]
      {
        (object) infoUtente
      }, this.documentoGetApplicazioniOperationCompleted, userState);
        }

        private void OndocumentoGetApplicazioniOperationCompleted(object arg)
        {
            if (this.documentoGetApplicazioniCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoGetApplicazioniCompleted((object)this, new documentoGetApplicazioniCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/documentoExecAddLavoro", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void documentoExecAddLavoro(InfoDocumento infoDoc, Fascicolo fasc, InfoUtente infoUtente)
        {
            this.Invoke("documentoExecAddLavoro", new object[3]
      {
        (object) infoDoc,
        (object) fasc,
        (object) infoUtente
      });
        }

        public IAsyncResult BegindocumentoExecAddLavoro(InfoDocumento infoDoc, Fascicolo fasc, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("documentoExecAddLavoro", new object[3]
      {
        (object) infoDoc,
        (object) fasc,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EnddocumentoExecAddLavoro(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void documentoExecAddLavoroAsync(InfoDocumento infoDoc, Fascicolo fasc, InfoUtente infoUtente)
        {
            this.documentoExecAddLavoroAsync(infoDoc, fasc, infoUtente, (object)null);
        }

        public void documentoExecAddLavoroAsync(InfoDocumento infoDoc, Fascicolo fasc, InfoUtente infoUtente, object userState)
        {
            if (this.documentoExecAddLavoroOperationCompleted == null)
                this.documentoExecAddLavoroOperationCompleted = new SendOrPostCallback(this.OndocumentoExecAddLavoroOperationCompleted);
            this.InvokeAsync("documentoExecAddLavoro", new object[3]
      {
        (object) infoDoc,
        (object) fasc,
        (object) infoUtente
      }, this.documentoExecAddLavoroOperationCompleted, userState);
        }

        private void OndocumentoExecAddLavoroOperationCompleted(object arg)
        {
            if (this.documentoExecAddLavoroCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.documentoExecAddLavoroCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/utenteGetRegistri", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Registro[] utenteGetRegistri(InfoUtente infoUtente)
        {
            return (Registro[])this.Invoke("utenteGetRegistri", new object[1]
      {
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginutenteGetRegistri(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("utenteGetRegistri", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public Registro[] EndutenteGetRegistri(IAsyncResult asyncResult)
        {
            return (Registro[])this.EndInvoke(asyncResult)[0];
        }

        public void utenteGetRegistriAsync(InfoUtente infoUtente)
        {
            this.utenteGetRegistriAsync(infoUtente, (object)null);
        }

        public void utenteGetRegistriAsync(InfoUtente infoUtente, object userState)
        {
            if (this.utenteGetRegistriOperationCompleted == null)
                this.utenteGetRegistriOperationCompleted = new SendOrPostCallback(this.OnutenteGetRegistriOperationCompleted);
            this.InvokeAsync("utenteGetRegistri", new object[1]
      {
        (object) infoUtente
      }, this.utenteGetRegistriOperationCompleted, userState);
        }

        private void OnutenteGetRegistriOperationCompleted(object arg)
        {
            if (this.utenteGetRegistriCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.utenteGetRegistriCompleted((object)this, new utenteGetRegistriCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/utenteChangePassword", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void utenteChangePassword(Login login, InfoUtente infoUtente)
        {
            this.Invoke("utenteChangePassword", new object[2]
      {
        (object) login,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginutenteChangePassword(Login login, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("utenteChangePassword", new object[2]
      {
        (object) login,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndutenteChangePassword(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void utenteChangePasswordAsync(Login login, InfoUtente infoUtente)
        {
            this.utenteChangePasswordAsync(login, infoUtente, (object)null);
        }

        public void utenteChangePasswordAsync(Login login, InfoUtente infoUtente, object userState)
        {
            if (this.utenteChangePasswordOperationCompleted == null)
                this.utenteChangePasswordOperationCompleted = new SendOrPostCallback(this.OnutenteChangePasswordOperationCompleted);
            this.InvokeAsync("utenteChangePassword", new object[2]
      {
        (object) login,
        (object) infoUtente
      }, this.utenteChangePasswordOperationCompleted, userState);
        }

        private void OnutenteChangePasswordOperationCompleted(object arg)
        {
            if (this.utenteChangePasswordCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.utenteChangePasswordCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/logoff", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void logoff(InfoUtente infoUtente)
        {
            this.Invoke("logoff", new object[1]
      {
        (object) infoUtente
      });
        }

        public IAsyncResult Beginlogoff(InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("logoff", new object[1]
      {
        (object) infoUtente
      }, callback, asyncState);
        }

        public void Endlogoff(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void logoffAsync(InfoUtente infoUtente)
        {
            this.logoffAsync(infoUtente, (object)null);
        }

        public void logoffAsync(InfoUtente infoUtente, object userState)
        {
            if (this.logoffOperationCompleted == null)
                this.logoffOperationCompleted = new SendOrPostCallback(this.OnlogoffOperationCompleted);
            this.InvokeAsync("logoff", new object[1]
      {
        (object) infoUtente
      }, this.logoffOperationCompleted, userState);
        }

        private void OnlogoffOperationCompleted(object arg)
        {
            if (this.logoffCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.logoffCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/login", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public Utente login([XmlElement("login")] Login login1)
        {
            return (Utente)this.Invoke("login", new object[1]
      {
        (object) login1
      })[0];
        }

        public IAsyncResult Beginlogin(Login login1, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("login", new object[1]
      {
        (object) login1
      }, callback, asyncState);
        }

        public Utente Endlogin(IAsyncResult asyncResult)
        {
            return (Utente)this.EndInvoke(asyncResult)[0];
        }

        public void loginAsync(Login login1)
        {
            this.loginAsync(login1, (object)null);
        }

        public void loginAsync(Login login1, object userState)
        {
            if (this.loginOperationCompleted == null)
                this.loginOperationCompleted = new SendOrPostCallback(this.OnloginOperationCompleted);
            this.InvokeAsync("login", new object[1]
      {
        (object) login1
      }, this.loginOperationCompleted, userState);
        }

        private void OnloginOperationCompleted(object arg)
        {
            if (this.loginCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.loginCompleted((object)this, new loginCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneUpdateTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicolazioneClassificazione fascicolazioneUpdateTitolario(FascicolazioneClassificazione nodoTitolario, InfoUtente infoUtente)
        {
            return (FascicolazioneClassificazione)this.Invoke("fascicolazioneUpdateTitolario", new object[2]
      {
        (object) nodoTitolario,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginfascicolazioneUpdateTitolario(FascicolazioneClassificazione nodoTitolario, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneUpdateTitolario", new object[2]
      {
        (object) nodoTitolario,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FascicolazioneClassificazione EndfascicolazioneUpdateTitolario(IAsyncResult asyncResult)
        {
            return (FascicolazioneClassificazione)this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneUpdateTitolarioAsync(FascicolazioneClassificazione nodoTitolario, InfoUtente infoUtente)
        {
            this.fascicolazioneUpdateTitolarioAsync(nodoTitolario, infoUtente, (object)null);
        }

        public void fascicolazioneUpdateTitolarioAsync(FascicolazioneClassificazione nodoTitolario, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneUpdateTitolarioOperationCompleted == null)
                this.fascicolazioneUpdateTitolarioOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneUpdateTitolarioOperationCompleted);
            this.InvokeAsync("fascicolazioneUpdateTitolario", new object[2]
      {
        (object) nodoTitolario,
        (object) infoUtente
      }, this.fascicolazioneUpdateTitolarioOperationCompleted, userState);
        }

        private void OnfascicolazioneUpdateTitolarioOperationCompleted(object arg)
        {
            if (this.fascicolazioneUpdateTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneUpdateTitolarioCompleted((object)this, new fascicolazioneUpdateTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneDelTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void fascicolazioneDelTitolario(FascicolazioneClassificazione nodoTitolario, InfoUtente infoUtente)
        {
            this.Invoke("fascicolazioneDelTitolario", new object[2]
      {
        (object) nodoTitolario,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginfascicolazioneDelTitolario(FascicolazioneClassificazione nodoTitolario, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneDelTitolario", new object[2]
      {
        (object) nodoTitolario,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndfascicolazioneDelTitolario(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void fascicolazioneDelTitolarioAsync(FascicolazioneClassificazione nodoTitolario, InfoUtente infoUtente)
        {
            this.fascicolazioneDelTitolarioAsync(nodoTitolario, infoUtente, (object)null);
        }

        public void fascicolazioneDelTitolarioAsync(FascicolazioneClassificazione nodoTitolario, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneDelTitolarioOperationCompleted == null)
                this.fascicolazioneDelTitolarioOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneDelTitolarioOperationCompleted);
            this.InvokeAsync("fascicolazioneDelTitolario", new object[2]
      {
        (object) nodoTitolario,
        (object) infoUtente
      }, this.fascicolazioneDelTitolarioOperationCompleted, userState);
        }

        private void OnfascicolazioneDelTitolarioOperationCompleted(object arg)
        {
            if (this.fascicolazioneDelTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneDelTitolarioCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetAutorizzazioniNodoTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicoloDiritto[] fascicolazioneGetAutorizzazioniNodoTitolario(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario)
        {
            return (FascicoloDiritto[])this.Invoke("fascicolazioneGetAutorizzazioniNodoTitolario", new object[2]
      {
        (object) infoUtente,
        (object) nodoTitolario
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetAutorizzazioniNodoTitolario(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetAutorizzazioniNodoTitolario", new object[2]
      {
        (object) infoUtente,
        (object) nodoTitolario
      }, callback, asyncState);
        }

        public FascicoloDiritto[] EndfascicolazioneGetAutorizzazioniNodoTitolario(IAsyncResult asyncResult)
        {
            return (FascicoloDiritto[])this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetAutorizzazioniNodoTitolarioAsync(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario)
        {
            this.fascicolazioneGetAutorizzazioniNodoTitolarioAsync(infoUtente, nodoTitolario, (object)null);
        }

        public void fascicolazioneGetAutorizzazioniNodoTitolarioAsync(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario, object userState)
        {
            if (this.fascicolazioneGetAutorizzazioniNodoTitolarioOperationCompleted == null)
                this.fascicolazioneGetAutorizzazioniNodoTitolarioOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetAutorizzazioniNodoTitolarioOperationCompleted);
            this.InvokeAsync("fascicolazioneGetAutorizzazioniNodoTitolario", new object[2]
      {
        (object) infoUtente,
        (object) nodoTitolario
      }, this.fascicolazioneGetAutorizzazioniNodoTitolarioOperationCompleted, userState);
        }

        private void OnfascicolazioneGetAutorizzazioniNodoTitolarioOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetAutorizzazioniNodoTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetAutorizzazioniNodoTitolarioCompleted((object)this, new fascicolazioneGetAutorizzazioniNodoTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneGetCodiceFiglioTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public string fascicolazioneGetCodiceFiglioTitolario(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario)
        {
            return (string)this.Invoke("fascicolazioneGetCodiceFiglioTitolario", new object[2]
      {
        (object) infoUtente,
        (object) nodoTitolario
      })[0];
        }

        public IAsyncResult BeginfascicolazioneGetCodiceFiglioTitolario(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneGetCodiceFiglioTitolario", new object[2]
      {
        (object) infoUtente,
        (object) nodoTitolario
      }, callback, asyncState);
        }

        public string EndfascicolazioneGetCodiceFiglioTitolario(IAsyncResult asyncResult)
        {
            return (string)this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneGetCodiceFiglioTitolarioAsync(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario)
        {
            this.fascicolazioneGetCodiceFiglioTitolarioAsync(infoUtente, nodoTitolario, (object)null);
        }

        public void fascicolazioneGetCodiceFiglioTitolarioAsync(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario, object userState)
        {
            if (this.fascicolazioneGetCodiceFiglioTitolarioOperationCompleted == null)
                this.fascicolazioneGetCodiceFiglioTitolarioOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneGetCodiceFiglioTitolarioOperationCompleted);
            this.InvokeAsync("fascicolazioneGetCodiceFiglioTitolario", new object[2]
      {
        (object) infoUtente,
        (object) nodoTitolario
      }, this.fascicolazioneGetCodiceFiglioTitolarioOperationCompleted, userState);
        }

        private void OnfascicolazioneGetCodiceFiglioTitolarioOperationCompleted(object arg)
        {
            if (this.fascicolazioneGetCodiceFiglioTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneGetCodiceFiglioTitolarioCompleted((object)this, new fascicolazioneGetCodiceFiglioTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneNewTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FascicolazioneClassificazione fascicolazioneNewTitolario(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario, string idParent, bool ereditaDiritti)
        {
            return (FascicolazioneClassificazione)this.Invoke("fascicolazioneNewTitolario", new object[4]
      {
        (object) infoUtente,
        (object) nodoTitolario,
        (object) idParent,
        (object) ereditaDiritti
      })[0];
        }

        public IAsyncResult BeginfascicolazioneNewTitolario(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario, string idParent, bool ereditaDiritti, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneNewTitolario", new object[4]
      {
        (object) infoUtente,
        (object) nodoTitolario,
        (object) idParent,
        (object) ereditaDiritti
      }, callback, asyncState);
        }

        public FascicolazioneClassificazione EndfascicolazioneNewTitolario(IAsyncResult asyncResult)
        {
            return (FascicolazioneClassificazione)this.EndInvoke(asyncResult)[0];
        }

        public void fascicolazioneNewTitolarioAsync(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario, string idParent, bool ereditaDiritti)
        {
            this.fascicolazioneNewTitolarioAsync(infoUtente, nodoTitolario, idParent, ereditaDiritti, (object)null);
        }

        public void fascicolazioneNewTitolarioAsync(InfoUtente infoUtente, FascicolazioneClassificazione nodoTitolario, string idParent, bool ereditaDiritti, object userState)
        {
            if (this.fascicolazioneNewTitolarioOperationCompleted == null)
                this.fascicolazioneNewTitolarioOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneNewTitolarioOperationCompleted);
            this.InvokeAsync("fascicolazioneNewTitolario", new object[4]
      {
        (object) infoUtente,
        (object) nodoTitolario,
        (object) idParent,
        (object) ereditaDiritti
      }, this.fascicolazioneNewTitolarioOperationCompleted, userState);
        }

        private void OnfascicolazioneNewTitolarioOperationCompleted(object arg)
        {
            if (this.fascicolazioneNewTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneNewTitolarioCompleted((object)this, new fascicolazioneNewTitolarioCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/fascicolazioneSetAutorizzazioniNodoTitolario", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void fascicolazioneSetAutorizzazioniNodoTitolario(FascicolazioneClassificazione nodoTitolario, Corrispondente[] corrAdd, Corrispondente[] corrRemove, bool ereditaDiritti, InfoUtente infoUtente)
        {
            this.Invoke("fascicolazioneSetAutorizzazioniNodoTitolario", new object[5]
      {
        (object) nodoTitolario,
        (object) corrAdd,
        (object) corrRemove,
        (object) ereditaDiritti,
        (object) infoUtente
      });
        }

        public IAsyncResult BeginfascicolazioneSetAutorizzazioniNodoTitolario(FascicolazioneClassificazione nodoTitolario, Corrispondente[] corrAdd, Corrispondente[] corrRemove, bool ereditaDiritti, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("fascicolazioneSetAutorizzazioniNodoTitolario", new object[5]
      {
        (object) nodoTitolario,
        (object) corrAdd,
        (object) corrRemove,
        (object) ereditaDiritti,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndfascicolazioneSetAutorizzazioniNodoTitolario(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void fascicolazioneSetAutorizzazioniNodoTitolarioAsync(FascicolazioneClassificazione nodoTitolario, Corrispondente[] corrAdd, Corrispondente[] corrRemove, bool ereditaDiritti, InfoUtente infoUtente)
        {
            this.fascicolazioneSetAutorizzazioniNodoTitolarioAsync(nodoTitolario, corrAdd, corrRemove, ereditaDiritti, infoUtente, (object)null);
        }

        public void fascicolazioneSetAutorizzazioniNodoTitolarioAsync(FascicolazioneClassificazione nodoTitolario, Corrispondente[] corrAdd, Corrispondente[] corrRemove, bool ereditaDiritti, InfoUtente infoUtente, object userState)
        {
            if (this.fascicolazioneSetAutorizzazioniNodoTitolarioOperationCompleted == null)
                this.fascicolazioneSetAutorizzazioniNodoTitolarioOperationCompleted = new SendOrPostCallback(this.OnfascicolazioneSetAutorizzazioniNodoTitolarioOperationCompleted);
            this.InvokeAsync("fascicolazioneSetAutorizzazioniNodoTitolario", new object[5]
      {
        (object) nodoTitolario,
        (object) corrAdd,
        (object) corrRemove,
        (object) ereditaDiritti,
        (object) infoUtente
      }, this.fascicolazioneSetAutorizzazioniNodoTitolarioOperationCompleted, userState);
        }

        private void OnfascicolazioneSetAutorizzazioniNodoTitolarioOperationCompleted(object arg)
        {
            if (this.fascicolazioneSetAutorizzazioniNodoTitolarioCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.fascicolazioneSetAutorizzazioniNodoTitolarioCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioneAddTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TemplateTrasmissione trasmissioneAddTemplate(TemplateTrasmissione template, InfoUtente infoUtente)
        {
            return (TemplateTrasmissione)this.Invoke("trasmissioneAddTemplate", new object[2]
      {
        (object) template,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BegintrasmissioneAddTemplate(TemplateTrasmissione template, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneAddTemplate", new object[2]
      {
        (object) template,
        (object) infoUtente
      }, callback, asyncState);
        }

        public TemplateTrasmissione EndtrasmissioneAddTemplate(IAsyncResult asyncResult)
        {
            return (TemplateTrasmissione)this.EndInvoke(asyncResult)[0];
        }

        public void trasmissioneAddTemplateAsync(TemplateTrasmissione template, InfoUtente infoUtente)
        {
            this.trasmissioneAddTemplateAsync(template, infoUtente, (object)null);
        }

        public void trasmissioneAddTemplateAsync(TemplateTrasmissione template, InfoUtente infoUtente, object userState)
        {
            if (this.trasmissioneAddTemplateOperationCompleted == null)
                this.trasmissioneAddTemplateOperationCompleted = new SendOrPostCallback(this.OntrasmissioneAddTemplateOperationCompleted);
            this.InvokeAsync("trasmissioneAddTemplate", new object[2]
      {
        (object) template,
        (object) infoUtente
      }, this.trasmissioneAddTemplateOperationCompleted, userState);
        }

        private void OntrasmissioneAddTemplateOperationCompleted(object arg)
        {
            if (this.trasmissioneAddTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneAddTemplateCompleted((object)this, new trasmissioneAddTemplateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioneGetListaTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public TemplateTrasmissione[] trasmissioneGetListaTemplate(InfoUtente infoUtente, string tipoOggetto)
        {
            return (TemplateTrasmissione[])this.Invoke("trasmissioneGetListaTemplate", new object[2]
      {
        (object) infoUtente,
        (object) tipoOggetto
      })[0];
        }

        public IAsyncResult BegintrasmissioneGetListaTemplate(InfoUtente infoUtente, string tipoOggetto, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioneGetListaTemplate", new object[2]
      {
        (object) infoUtente,
        (object) tipoOggetto
      }, callback, asyncState);
        }

        public TemplateTrasmissione[] EndtrasmissioneGetListaTemplate(IAsyncResult asyncResult)
        {
            return (TemplateTrasmissione[])this.EndInvoke(asyncResult)[0];
        }

        public void trasmissioneGetListaTemplateAsync(InfoUtente infoUtente, string tipoOggetto)
        {
            this.trasmissioneGetListaTemplateAsync(infoUtente, tipoOggetto, (object)null);
        }

        public void trasmissioneGetListaTemplateAsync(InfoUtente infoUtente, string tipoOggetto, object userState)
        {
            if (this.trasmissioneGetListaTemplateOperationCompleted == null)
                this.trasmissioneGetListaTemplateOperationCompleted = new SendOrPostCallback(this.OntrasmissioneGetListaTemplateOperationCompleted);
            this.InvokeAsync("trasmissioneGetListaTemplate", new object[2]
      {
        (object) infoUtente,
        (object) tipoOggetto
      }, this.trasmissioneGetListaTemplateOperationCompleted, userState);
        }

        private void OntrasmissioneGetListaTemplateOperationCompleted(object arg)
        {
            if (this.trasmissioneGetListaTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioneGetListaTemplateCompleted((object)this, new trasmissioneGetListaTemplateCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioniUpdateTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void trasmissioniUpdateTemplate(TemplateTrasmissione template, InfoUtente infoUtente)
        {
            this.Invoke("trasmissioniUpdateTemplate", new object[2]
      {
        (object) template,
        (object) infoUtente
      });
        }

        public IAsyncResult BegintrasmissioniUpdateTemplate(TemplateTrasmissione template, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioniUpdateTemplate", new object[2]
      {
        (object) template,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndtrasmissioniUpdateTemplate(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void trasmissioniUpdateTemplateAsync(TemplateTrasmissione template, InfoUtente infoUtente)
        {
            this.trasmissioniUpdateTemplateAsync(template, infoUtente, (object)null);
        }

        public void trasmissioniUpdateTemplateAsync(TemplateTrasmissione template, InfoUtente infoUtente, object userState)
        {
            if (this.trasmissioniUpdateTemplateOperationCompleted == null)
                this.trasmissioniUpdateTemplateOperationCompleted = new SendOrPostCallback(this.OntrasmissioniUpdateTemplateOperationCompleted);
            this.InvokeAsync("trasmissioniUpdateTemplate", new object[2]
      {
        (object) template,
        (object) infoUtente
      }, this.trasmissioniUpdateTemplateOperationCompleted, userState);
        }

        private void OntrasmissioniUpdateTemplateOperationCompleted(object arg)
        {
            if (this.trasmissioniUpdateTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioniUpdateTemplateCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/trasmissioniDeleteTemplate", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public void trasmissioniDeleteTemplate(TemplateTrasmissione template, InfoUtente infoUtente)
        {
            this.Invoke("trasmissioniDeleteTemplate", new object[2]
      {
        (object) template,
        (object) infoUtente
      });
        }

        public IAsyncResult BegintrasmissioniDeleteTemplate(TemplateTrasmissione template, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("trasmissioniDeleteTemplate", new object[2]
      {
        (object) template,
        (object) infoUtente
      }, callback, asyncState);
        }

        public void EndtrasmissioniDeleteTemplate(IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        public void trasmissioniDeleteTemplateAsync(TemplateTrasmissione template, InfoUtente infoUtente)
        {
            this.trasmissioniDeleteTemplateAsync(template, infoUtente, (object)null);
        }

        public void trasmissioniDeleteTemplateAsync(TemplateTrasmissione template, InfoUtente infoUtente, object userState)
        {
            if (this.trasmissioniDeleteTemplateOperationCompleted == null)
                this.trasmissioniDeleteTemplateOperationCompleted = new SendOrPostCallback(this.OntrasmissioniDeleteTemplateOperationCompleted);
            this.InvokeAsync("trasmissioniDeleteTemplate", new object[2]
      {
        (object) template,
        (object) infoUtente
      }, this.trasmissioniDeleteTemplateOperationCompleted, userState);
        }

        private void OntrasmissioniDeleteTemplateOperationCompleted(object arg)
        {
            if (this.trasmissioniDeleteTemplateCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.trasmissioniDeleteTemplateCompleted((object)this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/reportCorrispondenti", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileDocumento reportCorrispondenti(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente)
        {
            return (FileDocumento)this.Invoke("reportCorrispondenti", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginreportCorrispondenti(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("reportCorrispondenti", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FileDocumento EndreportCorrispondenti(IAsyncResult asyncResult)
        {
            return (FileDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void reportCorrispondentiAsync(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente)
        {
            this.reportCorrispondentiAsync(queryCorrispondente, infoUtente, (object)null);
        }

        public void reportCorrispondentiAsync(AddressbookQueryCorrispondente queryCorrispondente, InfoUtente infoUtente, object userState)
        {
            if (this.reportCorrispondentiOperationCompleted == null)
                this.reportCorrispondentiOperationCompleted = new SendOrPostCallback(this.OnreportCorrispondentiOperationCompleted);
            this.InvokeAsync("reportCorrispondenti", new object[2]
      {
        (object) queryCorrispondente,
        (object) infoUtente
      }, this.reportCorrispondentiOperationCompleted, userState);
        }

        private void OnreportCorrispondentiOperationCompleted(object arg)
        {
            if (this.reportCorrispondentiCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.reportCorrispondentiCompleted((object)this, new reportCorrispondentiCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
        }

        [SoapDocumentMethod("http://localhost/reportFascetteFascicolo", ParameterStyle = SoapParameterStyle.Wrapped, RequestNamespace = "http://localhost", ResponseNamespace = "http://localhost", Use = SoapBindingUse.Literal)]
        public FileDocumento reportFascetteFascicolo(Fascicolo fascicolo, InfoUtente infoUtente)
        {
            return (FileDocumento)this.Invoke("reportFascetteFascicolo", new object[2]
      {
        (object) fascicolo,
        (object) infoUtente
      })[0];
        }

        public IAsyncResult BeginreportFascetteFascicolo(Fascicolo fascicolo, InfoUtente infoUtente, AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("reportFascetteFascicolo", new object[2]
      {
        (object) fascicolo,
        (object) infoUtente
      }, callback, asyncState);
        }

        public FileDocumento EndreportFascetteFascicolo(IAsyncResult asyncResult)
        {
            return (FileDocumento)this.EndInvoke(asyncResult)[0];
        }

        public void reportFascetteFascicoloAsync(Fascicolo fascicolo, InfoUtente infoUtente)
        {
            this.reportFascetteFascicoloAsync(fascicolo, infoUtente, (object)null);
        }

        public void reportFascetteFascicoloAsync(Fascicolo fascicolo, InfoUtente infoUtente, object userState)
        {
            if (this.reportFascetteFascicoloOperationCompleted == null)
                this.reportFascetteFascicoloOperationCompleted = new SendOrPostCallback(this.OnreportFascetteFascicoloOperationCompleted);
            this.InvokeAsync("reportFascetteFascicolo", new object[2]
      {
        (object) fascicolo,
        (object) infoUtente
      }, this.reportFascetteFascicoloOperationCompleted, userState);
        }

        private void OnreportFascetteFascicoloOperationCompleted(object arg)
        {
            if (this.reportFascetteFascicoloCompleted == null)
                return;
            InvokeCompletedEventArgs completedEventArgs = (InvokeCompletedEventArgs)arg;
            this.reportFascetteFascicoloCompleted((object)this, new reportFascetteFascicoloCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
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
