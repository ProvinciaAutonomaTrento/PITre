using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Assegna.Network;
using InformaticaTrentinaPCL.ChangeRole.Network;
using InformaticaTrentinaPCL.CommonAction.Network;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.OpenFile.Network;
using Refit;
using InformaticaTrentinaPCL.Signature.Network;
using InformaticaTrentinaPCL.Login.Network;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.VerifyUpdate;

namespace InformaticaTrentinaPCL.Network
{
    public interface INetworkAPI
    {
        //[Get("/api/")]
        //Task<string> DoLogin([AliasAs("q")] string query, [AliasAs("p")] string page);

        [Post("/Login")]
        Task<LoginResponseModel> DoLogin([Body(BodySerializationMethod.UrlEncoded)] LoginRequestModel.Body request, CancellationToken ct);

        [Post("/RecuperaPasswordOTP")]
        Task<LoginResponseModel> DoRecuperaPasswordOTP([Body(BodySerializationMethod.UrlEncoded)] LoginRequestModel.Body request, CancellationToken ct);

        [Post("/Update")]
        Task<VerifyUpdateResponseModel> DoVerifyUpdate([Body(BodySerializationMethod.UrlEncoded)] VerifyUpdateRequestModel.Body request, CancellationToken ct);

        [Post("/CambioRuolo")]
        Task<ChangeRoleResponseModel> ChangeRole([AliasAs("idRuolo")] string idRuolo, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/LogOut")]
        Task<LogoutResponseModel> DoLogOut([AliasAs("dst")] string dst, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/TodoList")]
        Task<LoadToDoDocumentsResponseModel> GetToDoList([AliasAs("pagesize")] int pageSize, [AliasAs("requestedpage")] int requestedPage, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/ListaAmmByUser")]
        Task<List<AmministrazioneModel>> GetListaAmministrazioniByUser([AliasAs("userid")] string userid, CancellationToken ct);

        [Post("/LibroFirmaElements")]
        Task<LibroFirmaResponseModel> GetFirmaList([Body(BodySerializationMethod.UrlEncoded)] LibroFirmaRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/Ricerca")]
        Task<ADLListResponseModel> GetADLList([Body(BodySerializationMethod.UrlEncoded)] ADLListRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/Deleghe")]
        Task<DelegaListResponseModel> GetListaDelega([AliasAs("statoDelega")] string statoDelega, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/EsercitaDelegha")]
        Task<LoginResponseModel> EsercitaDelegha([AliasAs("webSessionId")] string webSessionId, [AliasAs("idDelega")] string idDelega, [AliasAs("idRuoloDElegante")] string idRuoloDElegante, [Header("authtoken")] string authtoken, CancellationToken ct);


        [Post("/AccettaRifiutaTrasm")]
        Task<AccettaRifiutaResponseModel> DoActionAccettaRifiuta([Body(BodySerializationMethod.UrlEncoded)] AccettaRifiutaRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/AdlAction")]
        Task<ActionADLResponseModel> DoActionADL([Body(BodySerializationMethod.UrlEncoded)] ActionADLRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/Smista")]
        Task<SmistaResponseModel> Smista([Body(BodySerializationMethod.UrlEncoded)] SmistaRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/ListaRagioni")]
        Task<ListaRagioniResponseModel> GetListaRagioni([AliasAs("idObject")] string idObject, [AliasAs("docFasc")] string docFasc, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/ListaModelliTrasmissione")]
        Task<ListaModelliTrasmissioneResponseModel> GetListaModelliTrasmissione([AliasAs("fascicoli")] bool fascicoli, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/ListaCorrispondenti")]
        Task<ListaCorrispondentiResponseModel> ListaCorrispondenti([Body(BodySerializationMethod.UrlEncoded)] ListaCorrispondentiRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/Ricerca")]
        Task<RicercaResponseModel> Search([Body(BodySerializationMethod.UrlEncoded)] RicercaRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/Delega")]
        Task<NewMandateResponseModel> Delega([Body(BodySerializationMethod.Json)] NewMandateRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/RevocaDelega")]
        Task<bool> DoRevoke([Body(BodySerializationMethod.Json)] DelegaDocumentModel body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/RicercaUtenti")]
        Task<SearchMandateAssigneeResponseModel> RicercaUtenti([AliasAs("descrizione")] string descrizione, [AliasAs("numMaxResult")] int numMaxResult, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/ListaIstanze")]
        Task<List<InstanceModel>> ListIstanze(CancellationToken ct);

        [Get("/DocInfo")]
        Task<GetDocInfoResponseModel> GetDocInfo([AliasAs("idDoc")] string idDoc, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/File")]
        Task<GetFileResponseModel> GetFile([AliasAs("idDoc")] string idDoc, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/ListaPreferiti")]
        Task<FavoritesResponseModel> GetFavorites([AliasAs("soloPers")] bool soloPers, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Put("/Preferito")]
        Task<bool> PutFavorite([Body(BodySerializationMethod.UrlEncoded)] SetFavoriteRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Delete("/Preferito")]
        Task<bool> DeleteFavorite([Body(BodySerializationMethod.UrlEncoded)] SetFavoriteRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/Condividi")]
        Task<string> Condividi([AliasAs("idpeople")] string idpeople, [AliasAs("idDocumento")] string idDocumento, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/FascRicerca")]
        Task<GetFascRicercaResponseModel> GetFascRicerca([AliasAs("IdFascicolo")] string idFascicolo, [AliasAs("PageSize")] int pageSize, [AliasAs("RequestedPage")] int requestedPage, [AliasAs("TestoDaCercare")] string testoDaCercare, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/DocumentoCondiviso")]
        Task<GetDocumentoCondivisoResponseModel> GetDocumentoCondiviso([AliasAs("chiaveDoc")] string chiaveDoc, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Get("/ListaRuoliUtente")]
        Task<ListaRuoliUserResponseModel> GetListaRuoliUser([AliasAs("idUtente")] string idUtente, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/RichiestaOTP")]
        Task<RequestOTPResponseModel>  RequestOTP([Body(BodySerializationMethod.UrlEncoded)] RequestOTPRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/FirmaHSM")]
        Task<SignDocumentResponseModel> FirmaHSM([Body(BodySerializationMethod.UrlEncoded)] SignDocumentRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/TrasmissioneVista")]
        Task<ViewedResponseModel> DoViewed([Body(BodySerializationMethod.UrlEncoded)] ViewedRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/RespingiElementi")]
        Task<RespingiElementiResponseModel> RejectDocuments([Body(BodySerializationMethod.Json)] LibroFirmaRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/FirmaElementi")]
        Task<FirmaElementiResponseModel> SignDocuments([Body(BodySerializationMethod.Json)] LibroFirmaRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/CambiaStatoElementiMultipli")]
        Task<CambiaStatoElementoResponseModel> ChangeDocumentsState([Body(BodySerializationMethod.Json)] CambiaStatoElementoRequestModel body, [Header("authtoken")] string authtoken, CancellationToken ct);

        [Post("/FirmaElementiHSM")]
        Task<SignDocumentResponseModel> SignDocumentsHSM([Body(BodySerializationMethod.Json)] RequestOTPRequestModel.Body body, [Header("authtoken")] string authtoken, CancellationToken ct);
    }
}
