using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DocsPaVO.Mobile.Responses;
using DocsPaVO.Mobile.Requests;

namespace DocsPaWS.Mobile
{
    [ServiceContract]
    public interface IVTDocsWSMobile
    {
        [OperationContract]
        bool beIsReady();

        [OperationContract]
        LoginResponse login(LoginRequest loginRequest);

        [OperationContract]
        LogoutResponse logout(LogoutRequest logoutRequest);

        [OperationContract]
        ToDoListResponse getTodoList(ToDoListRequest request);

        [OperationContract]
        bool rimuoviNotifica(string idEvento);

        #region CheckPage
        [OperationContract]
        bool checkConnection(out string message);
        #endregion

        #region Documento
        [OperationContract]
        GetDocInfoResponse getDocInfo(GetDocInfoRequest request);

        [OperationContract]
        GetFileResponse getFile(GetFileRequest request);

        [OperationContract]
        GetPreviewResponse getPreview(GetPreviewRequest request);

        [OperationContract]
        ADLActionResponse ADLAction(ADLActionRequest request);

        #endregion
        #region Fascicolo
        [OperationContract]
        GetFascInfoResponse getFascInfo(GetFascInfoRequest request);
        #endregion
        #region Ricerca
        [OperationContract]
        RicercaResponse ricerca(RicercaRequest request);
        [OperationContract]
        GetRicSalvateResponse getRicercheSalvate(GetRicSalvateRequest request);
        #endregion
        #region Deleghe

        [OperationContract]
        CountDelegheAttiveResponse getCountDelegheAttive(CountDelegheAttiveRequest request);

        [OperationContract]
        DelegheResponse getListaDeleghe(DelegheRequest request);

        [OperationContract]
        ListaModelliDelegaResponse getListaModelliDelega(ListaModelliDelegaRequest request);

        [OperationContract]
        CreaDelegaDaModelloResponse creaDelegaDaModello(CreaDelegaDaModelloRequest request);

        [OperationContract]
        CreaDelegaResponse creaDelega(CreaDelegaRequest request);

        [OperationContract]
        AccettaDelegaResponse accettaDelega(AccettaDelegaRequest request);

        [OperationContract]
        DismettiDelegaResponse dismettiDelega(DismettiDelegaRequest request);

        [OperationContract]
        RevocaDelegheResponse revocaDeleghe(RevocaDelegheRequest request);

        [OperationContract]
        RicercaUtentiResponse ricercaUtenti(RicercaUtentiRequest request);

        [OperationContract]
        ListaTipiRuoloResponse getListaRuoli(ListaTipiRuoloRequest request);

        [OperationContract]
        ListaUtentiResponse getListaUtenti(ListaUtentiRequest request);
        #endregion
        #region Trasmissioni

        [OperationContract]
        AccettaRifiutaTrasmResponse setDataVistaSP_TV(AccettaRifiutaTrasmRequest request);

        [OperationContract]
        AccettaRifiutaTrasmResponse accettaRifiutaTrasm(AccettaRifiutaTrasmRequest request);

        [OperationContract]
        ListaModelliTrasmResponse getListaModelliTrasm(ListaModelliTrasmRequest request);

        [OperationContract]
        EseguiTrasmResponse eseguiTrasm(EseguiTrasmRequest request);
        #endregion

        #region Smistamento

        [OperationContract]
        RicercaSmistamentoResponse ricercaSmistamento(RicercaSmistamentoRequest request);

        [OperationContract]
        RicercaSmistamentoResponse aggiungiSmistamentoElement(RicercaSmistamentoRequest request);

        [OperationContract]
        GetSmistamentoTreeResponse getSmistamentoTree(GetSmistamentoTreeRequest request);

        [OperationContract]
        GetSmistamentoElementsResponse getSmistamentoElements(GetSmistamentoElementsRequest request);

        [OperationContract]
        EseguiSmistamentoResponse eseguiSmistamento(EseguiSmistamentoRequest request);
        #endregion

        #region HSM Signature 

        [OperationContract]
        HSMSignResponse hsmSign(HSMSignRequest request);

        [OperationContract]
        HSMSignResponse hsmRequestOTP(HSMSignRequest request);

        [OperationContract]
        HSMSignResponse hsmVerifySign(HSMSignRequest request);

        [OperationContract]
        HSMSignResponse hsmInfoSign(HSMSignRequest request);

        [OperationContract]
        HSMSignResponse hsmInfoMemento(HSMSignRequest request);

        [OperationContract]
        bool isAllowedOTP(DocsPaVO.Mobile.RuoloInfo info);


        #endregion

        #region Libro Firma
        [OperationContract]
        LibroFirmaResponse GetLibroFirmaElements(LibroFirmaRequest request);

        [OperationContract]
        LibroFirmaResponse CambiaStatoElementoLibroFirma(LibroFirmaCambiaStatoRequest request);

        [OperationContract]
        LibroFirmaResponse RespingiSelezionatiElementiLf(LibroFirmaRequest request);
        
        [OperationContract]
        LibroFirmaResponse FirmaSelezionatiElementiLf(LibroFirmaRequest request);

        [OperationContract]
        bool ExistsElementWithSignCades(DocsPaVO.Mobile.UserInfo userInfo, DocsPaVO.Mobile.RuoloInfo roleInfo);

        [OperationContract]
        bool ExistsElementWithSignPades(DocsPaVO.Mobile.UserInfo userInfo, DocsPaVO.Mobile.RuoloInfo roleInfo);

        [OperationContract]
        HSMSignResponse HsmMultiSign(HSMSignRequest request);

        [OperationContract]
        bool LibroFirmaIsAutorized(DocsPaVO.Mobile.RuoloInfo info);
        
        
        #endregion
    }
}
