using System;
using System.Net;
using InformaticaTrentinaPCL.Filter;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Resources;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Home
{
    public class HomePresenter : IHomePresenter
    {
        

        IHomeView view;
        IHomeModel model;
        SessionData sessionData;

        //State to save
        protected Tab tab = Tab.TODO;
        protected FilterModel filter = null;
        

        public HomePresenter(IHomeView view, INativeFactory nf)
        {
            this.view = view;
            this.sessionData = nf.GetSessionData();
#if CUSTOM
            model = new DummyHomeModel();
#else
            model = new HomeModel();
#endif
        }

        public void Dispose()
        {
            model.Dispose();
        }

        public async void DoLogout()
        {
            view.OnUpdateLoader(true);
            var request = new LogoutRequestModel(sessionData.userInfo.dst, sessionData.userInfo.token);
            LogoutResponseModel response = await model.DoLogOut(request);
            view.OnUpdateLoader(false);
            sessionData.clear();
            view.OnLogoutOk();
			this.ManageResponse(response);
		}

        private void ManageResponse(LogoutResponseModel response){
            if (response.IsCancelled)
            {
                return;
            }
            if (!response.StatusOk())
			{
				//TODO evntuale report su tool di analytics/crash
			}
        }

        public void OnImpostazioniTapped()
        {
            string message = "Versione App: " + LocalizedString.APP_VERSION.Get();
            view.DoShowMessage(message);
        }

        public Tab GetCurrentTab()
        {
            return tab;
        }

        public void UpdateCurrentTab(Tab tab)
        {
            this.tab = tab;
        }

        public FilterModel GetFilter()
        {
            return filter;
        }

        public void UpdateFilter(FilterModel filterModel)
        {
            this.filter = filterModel;
        }
    }

    /// <summary>
    /// Map the UI TABS in an Integer.
    /// /!\ WARNING /!\
    /// Keep it updated if something changes in the TABBAR
    /// </summary>
    public enum Tab
    {
        ADL,
        SIGN,
        TODO,
        MANDATES,
        SEARCH
    }
}
