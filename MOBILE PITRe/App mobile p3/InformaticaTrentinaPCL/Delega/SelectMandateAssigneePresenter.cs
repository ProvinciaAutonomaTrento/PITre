using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Delega
{
    public class SelectMandateAssigneePresenter : ISelectMandateAssigneePresenter
    {
        protected ISelectMandateAssigneeView view;
        protected ISelectMandateAssigneeModel model;
        protected SessionData sessionData;
        protected IReachability reachability;

        public SelectMandateAssigneePresenter(ISelectMandateAssigneeView view, INativeFactory nativeFactory)
        {
            this.view = view;
            this.sessionData = nativeFactory.GetSessionData();
            this.reachability = nativeFactory.GetReachability();
#if CUSTOM
            this.model = new DummySelectMandateAssigneeModel();
#else
            this.model = new SelectMandateAssigneeModel();
#endif
            GetListFavorites();
        }

        #region InterfaceMethods
        public void ClearSearch()
        {
            this.GetListFavorites(); //ritorna alla lista di preferiti come primo accesso alla view
        }

        public void Dispose()
        {
            model.Dispose();
        }

        public async void GetListFavorites()
        {
            view.ClearList();
            view.OnUpdateLoader(true);

            FavoritesRequestModel.Body body = new FavoritesRequestModel.Body(true);
            FavoritesRequestModel request = new FavoritesRequestModel(body, sessionData.userInfo.token);
            FavoritesResponseModel response = await model.GetListFavorites(request);
            ManageResponse(response);

            view.OnUpdateLoader(false);
        }

        public async void SearchAssignee(string text)
        {
            if (text.Trim().Length < NetworkConstants.CONSTANT_SEARCH_CHARS)
            {
                view.ShowError(LocalizedString.SEARCH_STRING_TOO_SHORT.Get(),true);
            }
            else
            {
                view.ClearList();
                view.OnUpdateLoader(true);

                SearchMandateAssigneeRequestModel request = new SearchMandateAssigneeRequestModel(new SearchMandateAssigneeRequestModel.Body(text), sessionData.userInfo.token);
                SearchMandateAssigneeResponseModel response = await model.SearchAssignee(request);
                ManageResponse(response);

                view.OnUpdateLoader(false);
            }
        }

        public async void SetFavorite(AbstractRecipient user, bool favorite)
        {
            //view.OnUpdateLoader(true);

            SetFavoriteRequestModel.Body body = SetFavoriteRequestModel.Body.CreateBodyForPersona(user.getTitle(), user.getId());
            SetFavoriteRequestModel request = new SetFavoriteRequestModel(body, favorite, sessionData.userInfo.token);
            SetFavoriteResponseModel response = await model.SetFavorite(request);
            ManageResponse(user, response);

            //view.OnUpdateLoader(false);
        }

        private void ManageResponse(FavoritesResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        List<AbstractRecipient> list = null != response.preferiti ? new List<AbstractRecipient>(response.preferiti) : new List<AbstractRecipient>();
                        view.UpdateFavoriteList(list); //TODO check if is the correct way  
                        break;
                    default:
                        view.ShowError(LocalizedString.SERVICE_ERROR.Get());
                        break;
                }
            }

        }

        private void ManageResponse(SearchMandateAssigneeResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        List<AbstractRecipient> list = new List<AbstractRecipient>();
                        if (response.Risultati != null)
                        {
                            list.AddRange(response.Risultati);
                        }
                        view.UpdateSearchList(list);
                        break;
                    default:
                        view.ShowError(LocalizedString.SERVICE_ERROR.Get());
                        break;
                }
            }
        }

        private void ManageResponse(AbstractRecipient user, SetFavoriteResponseModel response)
        {
            //The favorite response is managed only if there is an error or is cancelled, otherwise there is nothing to do.
            bool rollback = response == null ||
                            response.IsCancelled ||
                            !string.IsNullOrEmpty(response.Error) ||
                            response.Code != 0;

            if (rollback)
            {
                view.ShowFavoriteError(LocalizedString.SET_FAVORITE_ERROR.Get());
                view.OnFavoriteError(user);
            }
        }

        #endregion
    }
}
