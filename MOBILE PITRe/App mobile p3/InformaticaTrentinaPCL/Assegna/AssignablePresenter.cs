using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.Assegna.MVPD;
using InformaticaTrentinaPCL.Assegna.Network;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using InformaticaTrentinaPCL.Delega;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Login;
using static InformaticaTrentinaPCL.Interfaces.AbstractRecipient;

namespace InformaticaTrentinaPCL.Assegna
{
    public class AssignablePresenter : IAssignablePresenter
    {
        protected IAssignableView view;
        protected IAssignableModel model;
        protected ISelectMandateAssigneeModel modelMandate; //Used only for service setFavorite
        protected SessionData sessionData;
        protected IAnalyticsManager analyticsManager;
        protected IReachability rechability;
        protected AbstractDocumentListItem currentDocument;

        public AssignablePresenter(IAssignableView view, INativeFactory nativeFactory, AbstractDocumentListItem currentDocument)
        {
            this.view = view;
            this.sessionData = nativeFactory.GetSessionData();
            this.analyticsManager = nativeFactory.GetAnalyticsManager();
            this.rechability = nativeFactory.GetReachability();
            this.currentDocument = currentDocument;

#if CUSTOM
            this.model = new DummyAssignableModel();
                this.modelMandate =  new DummySelectMandateAssigneeModel();

#else
            this.modelMandate = new SelectMandateAssigneeModel();
            this.model = new AssignableModel();
#endif
        }
        #region IImplementation
        public void ClearSearch()
        {
            this.GetLists();
        }

        public void Dispose()
        {
            model.Dispose();
        }

        public async Task OnFavoritePullToRefresh()
        {
            Debug.WriteLine("OnFavoritePullToRefresh START {0}:{1}:{2}:{3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);

            view.OnUpdateLoader(true);
            FavoritesRequestModel.Body body = new FavoritesRequestModel.Body(false);
            FavoritesRequestModel request = new FavoritesRequestModel(body, sessionData.userInfo.token);
            FavoritesResponseModel response = await model.ListaPreferiti(request);
            this.ManageResponse(response, successCallback: () =>
            {
                Debug.WriteLine("OnFavoritePullToRefresh FINISH {0}:{1}:{2}:{3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
                view.OnUpdateLoader(false);
                List<AbstractRecipient> list = new List<AbstractRecipient>();
                if (null != response.preferiti)
                {
                    list = new List<AbstractRecipient>(response.preferiti);
                }
                view.UpdateFavorite(list);
            }, errorCallback: () =>
             {
                 view.OnUpdateLoader(false);
                 view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                 view.UpdateSearchResults(new List<AbstractRecipient>());
             });
        }

        public async Task OnModelsPullToRefresh()
        {
            Debug.WriteLine("OnModelsPullToRefresh START {0}:{1}:{2}:{3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);

            view.OnUpdateLoader(true);
            bool isFascicolo = currentDocument.tipoDocumento == TypeDocument.FASCICOLO;
            ListaModelliTrasmissioneRequestModel request = new ListaModelliTrasmissioneRequestModel(isFascicolo, sessionData.userInfo.token);
            ListaModelliTrasmissioneResponseModel response = await model.ListaModelliTrasmissone(request);
            this.ManageResponse(response, successCallback: () =>
            {
                Debug.WriteLine("OnModelsPullToRefresh FINISH {0}:{1}:{2}:{3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
                view.OnUpdateLoader(false);
                view.UpdateModel(new List<AbstractRecipient>(response.modelli));
            }, errorCallback: () =>
             {
                 view.OnUpdateLoader(false);
                 view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                 view.UpdateSearchResults(new List<AbstractRecipient>());
             });
        }

        public async void SearchCorrispondenti(string text, string ragione = "")
        {

            if (text.Trim().Length < NetworkConstants.CONSTANT_SEARCH_CHARS)
            {
                view.ShowError(LocalizedString.SEARCH_STRING_TOO_SHORT.Get(), true);
            }
            else
            {
                view.OnUpdateLoader(true);
                this.ShowSearchView(true);

                ListaCorrispondentiRequestModel request = new ListaCorrispondentiRequestModel(new ListaCorrispondentiRequestModel.Body(text.Trim(), ragione), sessionData.userInfo.token);
                ListaCorrispondentiResponseModel response = await model.ListaCorrispondenti(request);
                this.ManageResponse(response, successCallback: () =>
                 {
                     view.OnUpdateLoader(false);
                     view.UpdateSearchResults(new List<AbstractRecipient>(response.elements));
                 }, errorCallback: () =>
                 {
                     view.OnUpdateLoader(false);
                     view.ShowError(LocalizedString.ON_SEARCH_ERROR.Get());
                     view.UpdateSearchResults(new List<AbstractRecipient>());
                 });
            }
        }

        public async void SetFavorite(AbstractRecipient recipient, bool favorite)
        {
            SetFavoriteRequestModel.Body body = SetFavoriteRequestModel.Body.CreateBodyForPersona(recipient.getTitle(), recipient.getId());
            SetFavoriteRequestModel request = new SetFavoriteRequestModel(body, favorite, sessionData.userInfo.token);
            SetFavoriteResponseModel response = await modelMandate.SetFavorite(request);
            ManageResponse(recipient, response);
        }

        public void OnViewReady()
        {
            this.GetLists();
        }

        /// <summary>
        /// Metodo per gestire il tipo di Assegnatario selezionato; Se USER viene notificato alla View per far visualizzare la scelta de ruolo eventuale
        /// </summary>
        /// <param name="selected">Selected.</param>
        public void OnSelect(AbstractRecipient selected)
        {
            switch (selected.getRecipientType())
            {
                case RecipientType.USER:
                    view.UserSelected(selected);
                    break;
                default:
                    view.OnAssigneeSelected(selected);
                    break;
            }
        }

        /// <summary>
        /// Metodo da chiamare quando si seleziona un Assegnee dalla schermata di Seleziona Ruolo
        /// </summary>
        /// <param name="selected">l'AbstractReceipient selezionato</param>
        public void OnAssigneeReceivedFromChooser(AbstractRecipient selected)
        {
            view.OnAssigneeSelected(selected);
        }

        #endregion
        #region Utility
        //Per visualizzare o meno la view di ricerca 
        private void ShowSearchView(bool show)
        {
            view.ShowTabsView(!show);
            view.ShowSearchView(show);
        }

        public async void GetLists()
        {
            this.ShowSearchView(false);

            var tasks = new List<Task>(){
                OnModelsPullToRefresh(),
                OnFavoritePullToRefresh()
            };

            await Task.WhenAll(tasks);
        }

        #endregion
        #region ManageResponse
        private void ManageResponse(BaseResponseModel response, Action successCallback, Action errorCallback)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, rechability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        successCallback();
                        break;
                    default:
                        errorCallback();
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
