using System;
using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.Assegna.MVPD;
using InformaticaTrentinaPCL.Assegna.Network;
using InformaticaTrentinaPCL.Delega;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Assegna
{
    public class SelectRolePresenter : ISelectRolePresenter
    {
        protected ISelectRoleView view;
        protected ISelectMandateAssigneeModel modelMandate; //Used only for service setFavorite
        protected ISelectRoleModel model;
        protected IAnalyticsManager analyticsManager;
        protected SessionData sessionData;
        protected AbstractRecipient currentUser;
        protected IReachability reachability;
        
        protected bool isFavoriteChanged;

        public SelectRolePresenter(ISelectRoleView view, INativeFactory nativeFactory, AbstractRecipient currentUser)
        {
            this.view = view;
            this.analyticsManager = nativeFactory.GetAnalyticsManager();
            this.sessionData = nativeFactory.GetSessionData();
            this.reachability = nativeFactory.GetReachability();
            this.currentUser = currentUser;
#if CUSTOM
            this.modelMandate = new DummySelectMandateAssigneeModel();
            this.model = new DummySelectRoleModel();
#else
            this.modelMandate = new SelectMandateAssigneeModel();
            this.model = new SelectRoleModel();
#endif
        }
        
        #region IImplementation
        public void Dispose()
        {
            modelMandate.Dispose();
        }

        public async void setFavorite(bool favorite)
        {
            isFavoriteChanged = !isFavoriteChanged;
            currentUser.setPreferred(favorite);  
            SetFavoriteRequestModel.Body body = SetFavoriteRequestModel.Body.CreateBodyForPersona(currentUser.getTitle(), currentUser.getId());
            SetFavoriteRequestModel request = new SetFavoriteRequestModel(body, favorite, sessionData.userInfo.token);
            SetFavoriteResponseModel response = await modelMandate.SetFavorite(request);
            ManageResponse(currentUser, response);
        }

        public void OnBackPressed()
        {
            view.GoBack(currentUser, isFavoriteChanged);
        }

        public async void GetListaRuoliUser()
        {
            view.OnUpdateLoader(true);
            ListaRuoliUserRequestModel.Body body = new ListaRuoliUserRequestModel.Body(currentUser.getId());
            ListaRuoliUserRequestModel request = new ListaRuoliUserRequestModel(body, sessionData.userInfo.token);
            ListaRuoliUserResponseModel response = await model.ListaRuoliUtente(request);
            ManageResponse(response);
            view.OnUpdateLoader(false);
        }

        public void OnViewReady()
        {
            GetListaRuoliUser();
        }
        
        #endregion
        #region ManageResponse
        private void ManageResponse(AbstractRecipient user, SetFavoriteResponseModel response)
        {
            //The favorite response is managed only if there is an error or is cancelled, otherwise there is nothing to do.
            bool rollback = response == null ||
                            response.IsCancelled ||
                            !string.IsNullOrEmpty(response.Error) ||
                            response.Code != 0;

            if (rollback)
            {
                isFavoriteChanged = !isFavoriteChanged;
                currentUser.setPreferred(!user.isPreferred());
                view.ShowFavoriteError(LocalizedString.SET_FAVORITE_ERROR.Get());
                view.OnFavoriteError(user);
            }
        }

        private void ManageResponse(ListaRuoliUserResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        view.UpdateView(currentUser, response.ListaRuoli);
                        break;
                    default:
                        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }

            }
        }
        #endregion
    }
}
