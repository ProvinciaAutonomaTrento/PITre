using System;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Resource;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Delega.MVP
{
    public class DelegaPresenter : IListaDeleghePresenter
    {
        IListaDelegheView view;
        IListaDelegheModel model;
        SessionData sessionData;
        protected bool isPullToRefresh = false;
        protected IReachability reachability;

        public DelegaPresenter(IListaDelegheView view, INativeFactory nativeFactory)
        {
            this.view = view;
            this.sessionData = nativeFactory.GetSessionData();
            this.reachability = nativeFactory.GetReachability();
#if CUSTOM
            this.model = new DummyDelegaModel();
#else
            this.model = new DelegaModel();
#endif
        }

        public void Dispose()
        {
            model.Dispose();
        }

        public async void GetDelegaDocumentsList()
        {
            shouldUpdateLoader(true);
            DelegaListRequestModel request = new DelegaListRequestModel(sessionData.userInfo.token);
            DelegaListResponseModel response = await model.GetListaModelliDelega(request);
            shouldUpdateLoader(false);
            this.ManageResponse(response, () =>{
				view.UpdateList(response.elements);
			});
        }

        public void OnPullToRefresh()
        {
            isPullToRefresh = true;
            GetDelegaDocumentsList();
            isPullToRefresh = false;
        }

        private void shouldUpdateLoader(bool show)
        {
            if (!isPullToRefresh)
            {
                view.OnUpdateLoader(show);
            }
        }

        private void ManageResponse(BaseResponseModel response, Action callback)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response,reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        callback();
                        break;
                    default: view.ShowError(LocalizedString.SERVICE_ERROR.Get()); 
                        break;
                }
            }
        }

        public async void DoRevoke(DelegaDocumentModel delega){
            this.shouldUpdateLoader(true);
            DoRevokeRequestModel request = new DoRevokeRequestModel(delega, sessionData.userInfo.token);
            DoRevokeResponseModel response = await model.DoRevoke(request);
            this.shouldUpdateLoader(false);
            this.ManageResponse(response, ()=>{
                view.OnRevokeOk();
            });
        }
    }
}
