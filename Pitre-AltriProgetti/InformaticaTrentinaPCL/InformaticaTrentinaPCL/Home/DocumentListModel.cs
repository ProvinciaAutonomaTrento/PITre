using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home.MVP;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.OpenFile.Network;
using InformaticaTrentinaPCL.Signature.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Home
{
    public class DocumentListModel : WS, IDocumentListModel
    {
        public DocumentListModel()
        {
        }

        public async Task<LoadToDoDocumentsResponseModel> LoadToDoDocuments(LoadToDoDocumentsRequestModel request)
        {
            LoadToDoDocumentsResponseModel response;
            try
            {
                response = await api.GetToDoList(request.body.pageSize, request.body.page, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new LoadToDoDocumentsResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<ADLListResponseModel> LoadADLDocuments(ADLListRequestModel request)
        {
            ADLListResponseModel response;
            try
            {
                response = await api.GetADLList(request.body, request.token, getCancellationToken());

            }
            catch (Exception e)
            {
                response = new ADLListResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<LibroFirmaResponseModel> LoadSignDocuments(LibroFirmaRequestModel request)
        {
            LibroFirmaResponseModel response;
            try
            {
                response = await api.GetFirmaList(request.body, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new LibroFirmaResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<CondividiResponseModel> Condividi(CondividiRequestModel request)
        {
            CondividiResponseModel response;
            try
            {
                string key = (await api.Condividi(request.body.idPeople, request.body.idDocumento, request.token, getCancellationToken())).Replace("\"", "");
                response = new CondividiResponseModel(key);
            }
            catch (Exception e)
            {
                response = new CondividiResponseModel();
                ResolveError(response, e);
            }
            return response;

        }

        public async Task<GetDocumentoCondivisoResponseModel> GetDocumentoCondiviso(GetDocumentoCondivisoRequestModel request)
        {
            GetDocumentoCondivisoResponseModel response;
            try
            {
                response = await api.GetDocumentoCondiviso(request.body.chiaveDoc, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new GetDocumentoCondivisoResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        #region document sign and reject
        public async Task<CambiaStatoElementoResponseModel> ChangeDocumentsState(CambiaStatoElementoRequestModel request)
        {
            CambiaStatoElementoResponseModel response;
            try
            {
                response = await api.ChangeDocumentsState(request, request.token, getCancellationToken(0));
                return response;
            }
            catch (Exception e)
            {
                response = new CambiaStatoElementoResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<RespingiElementiResponseModel> RejectDocuments(LibroFirmaRequestModel request)
        {
            RespingiElementiResponseModel response;
            try
            {
                response = await api.RejectDocuments(request.body, request.token, getCancellationToken(0));
                return response;
            }
            catch (Exception e)
            {
                response = new RespingiElementiResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<FirmaElementiResponseModel> SignDocuments(LibroFirmaRequestModel request)
        {
            FirmaElementiResponseModel response;
            try
            {
                response = await api.SignDocuments(request.body, request.token, getCancellationToken(0));
                return response;
            }
            catch (Exception e)
            {
                response = new FirmaElementiResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
        #endregion
    }
}
