using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DocsPaVO.Mobile.Responses;
using DocsPaVO.Mobile.Requests;
using log4net;
using DocsPaVO.Mobile;
using System.ServiceModel.Activation;

namespace DocsPaWS.Mobile
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WTDocsWSMobile" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class VTDocsWSMobile : IVTDocsWSMobile
    {

        public bool beIsReady()
        {
            return true;
        }

        public bool checkConnection(out string message)
        {
            return MobileManager.checkConnection(out message);
        }

        public LoginResponse login(LoginRequest loginRequest)
        {
            SetUserId(loginRequest.UserName);
            return MobileManager.login(loginRequest);
        }

        public LogoutResponse logout(LogoutRequest logoutRequest)
        {
            SetUserId(logoutRequest.UserInfo);
            return MobileManager.logout(logoutRequest);
        }

        public ToDoListResponse getTodoList(ToDoListRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.getNotify(request);
        }
        #region Documento
        public GetDocInfoResponse getDocInfo(GetDocInfoRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.getDocInfo(request);
        }

        public bool rimuoviNotifica(string idEvento)
        {
            return MobileManager.rimuoviNotifica(idEvento);
        }

        public GetFileResponse getFile(GetFileRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.getFile(request);
        }

        public GetPreviewResponse getPreview(GetPreviewRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.getPreview(request);
        }

        public ADLActionResponse ADLAction(ADLActionRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.ADLAction(request);
        }
        #endregion
        #region Fascicolo
        public GetFascInfoResponse getFascInfo(GetFascInfoRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.getFascInfo(request);
        }
        #endregion
        #region Ricerca
        public RicercaResponse ricerca(RicercaRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.ricerca(request);
        }

        public GetRicSalvateResponse getRicercheSalvate(GetRicSalvateRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.getRicercheSalvate(request);
        }
        #endregion
        #region Deleghe

        public CountDelegheAttiveResponse getCountDelegheAttive(CountDelegheAttiveRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.getCountDelegheAttive(request);
        }

        public DelegheResponse getListaDeleghe(DelegheRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.getListaDeleghe(request);
        }

        public ListaModelliDelegaResponse getListaModelliDelega(ListaModelliDelegaRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.getListaModelliDelega(request);
        }

        public CreaDelegaDaModelloResponse creaDelegaDaModello(CreaDelegaDaModelloRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.creaDelegaDaModello(request);
        }

        public CreaDelegaResponse creaDelega(CreaDelegaRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.creaDelega(request);
        }

        public AccettaDelegaResponse accettaDelega(AccettaDelegaRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.accettaDelega(request);
        }

        public DismettiDelegaResponse dismettiDelega(DismettiDelegaRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.dismettiDelega(request);
        }

        public RevocaDelegheResponse revocaDeleghe(RevocaDelegheRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.revocaDeleghe(request);

        }

        public ListaTipiRuoloResponse getListaRuoli(ListaTipiRuoloRequest request)
        {
            return MobileManager.getListaRuoli(request);
        }

        public ListaUtentiResponse getListaUtenti(ListaUtentiRequest request)
        {
            return MobileManager.getListaUtenti(request);
        }

        public RicercaUtentiResponse ricercaUtenti(RicercaUtentiRequest request)
        {
            return MobileManager.ricercaUtenti(request);
        }
        #endregion
        #region Trasmissioni

        public AccettaRifiutaTrasmResponse setDataVistaSP_TV(AccettaRifiutaTrasmRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.setDataVistaSP_TV(request);
        }

        public AccettaRifiutaTrasmResponse accettaRifiutaTrasm(AccettaRifiutaTrasmRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.accettaRifiutaTrasm(request);
        }

        public ListaModelliTrasmResponse getListaModelliTrasm(ListaModelliTrasmRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.getListaModelliTrasm(request);
        }

        public EseguiTrasmResponse eseguiTrasm(EseguiTrasmRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.eseguiTrasm(request);
        }
        #endregion

        #region Smistamento

        public GetSmistamentoTreeResponse getSmistamentoTree(GetSmistamentoTreeRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.getSmistamentoTree(request);
        }

        public RicercaSmistamentoResponse ricercaSmistamento(RicercaSmistamentoRequest request)
        {
            SetUserId(request.UserInfo);
            //return MobileManager.ricercaSmistamento(request);
            return MobileManager.GetListaCorrispondentiVeloce(request);
        }

        /// <summary>
        /// MEV SMISTAMENTO
        /// Aggiunge gli utenti appartenenti ad un ruolo in seguito ad una ricerca ajax
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RicercaSmistamentoResponse aggiungiSmistamentoElement(RicercaSmistamentoRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.ricercaSmistamento(request);
        }

        public GetSmistamentoElementsResponse getSmistamentoElements(GetSmistamentoElementsRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.getSmistamentoElements(request);
        }

        public EseguiSmistamentoResponse eseguiSmistamento(EseguiSmistamentoRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.eseguiSmistamento(request);
            //return MobileManager.eseguiSmistamentoOLD(request);
        }
        #endregion

        #region HSM Signature
        /// <summary>
        /// Firma un documento com metodologia HSM
        /// </summary>
        /// <returns>True=Documento firmato; false= Documento non firmato</returns>
        public HSMSignResponse hsmSign(HSMSignRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.hsmSign(request);
        }

        /// <summary>
        /// Richiesta al servizio OTP
        /// </summary>
        /// <returns></returns>
        public HSMSignResponse hsmRequestOTP(HSMSignRequest request)
        {
            return MobileManager.hsmRequestOTP(request);
        }

        /// <summary>
        /// Verifica Documento Firmato Digitalmente
        /// </summary>
        /// <returns></returns>
        public HSMSignResponse hsmVerifySign(HSMSignRequest request)
        {
             return MobileManager.hsmVerifySign(request);
        }

        /// <summary>
        /// Dettaglio Informativo Documento Firmato
        /// </summary>
        /// <returns></returns>
        public HSMSignResponse hsmInfoSign(HSMSignRequest request)
        {
            return MobileManager.hsmInfoSign(request);
        }


        /// <summary>
        /// Recupero info memento
        /// </summary>
        /// <returns></returns>
        public HSMSignResponse hsmInfoMemento(HSMSignRequest request)
        {
            return MobileManager.hsmGetMementoForUser(request);
        }

        /// <summary>
        /// Verifica se il ruolo è abilitato alla funzione richedi OTP
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool isAllowedOTP(RuoloInfo info)
        {
            return MobileManager.isAllowedOTP(info);
        }


        #endregion

        #region Libro firma

        public LibroFirmaResponse GetLibroFirmaElements(LibroFirmaRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.GetLibroFirmaElements(request);
        }

        public LibroFirmaResponse CambiaStatoElementoLibroFirma(LibroFirmaCambiaStatoRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.CambiaStatoElementoLibroFirma(request);
        }

        public LibroFirmaResponse RespingiSelezionatiElementiLf(LibroFirmaRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.RespingiSelezionatiElementiLf(request);
        }

        public LibroFirmaResponse FirmaSelezionatiElementiLf(LibroFirmaRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.FirmaSelezionatiElementiLf(request);
        }

        public bool ExistsElementWithSignCades(UserInfo userInfo, RuoloInfo roleInfo)
        {
            return MobileManager.ExistsElementWithSignCades(userInfo, roleInfo);
        }

        public bool ExistsElementWithSignPades(UserInfo userInfo, RuoloInfo roleInfo)
        {
            return MobileManager.ExistsElementWithSignPades(userInfo, roleInfo);
        }

        /// <summary>
        /// Firma i documenti del libro firma
        /// </summary>
        /// <returns>True=Documento firmato; false= Documento non firmato</returns>
        public HSMSignResponse HsmMultiSign(HSMSignRequest request)
        {
            SetUserId(request.UserInfo);
            return MobileManager.HsmMultiSign(request);
        }

        public bool LibroFirmaIsAutorized(RuoloInfo info)
        {
            return MobileManager.LibroFirmaIsAutorized(info);
        }
        #endregion

        private void SetUserId(UserInfo userInfo)
        {
            if (userInfo != null) SetUserId(userInfo.UserId);
        }

        private void SetUserId(string userId)
        {
            if (!string.IsNullOrEmpty(userId)) LogicalThreadContext.Properties["userId"] = userId.ToUpper();
        }
     
    }
}
