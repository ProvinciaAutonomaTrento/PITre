using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.Network;
using InformaticaTrentinaPCL.LoginListAdministration.MVP;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.LoginListAdministration
{
    public class LoginListAdministrationsPresenter : ILoginListAdministrationsPresenter
    {
        protected ILoginListAdministrationsView view;
        protected ILoginListAdministrationsModel model;
        protected string username;
		protected IAnalyticsManager analyticsManager;
        protected IReachability reachability;
  
        public LoginListAdministrationsPresenter(ILoginListAdministrationsView view,INativeFactory nativeFactory,string username)
        {
            this.view = view;
            this.username = username;
            this.analyticsManager = nativeFactory.GetAnalyticsManager();
            this.reachability = nativeFactory.GetReachability();
#if CUSTOM
            this.model = new DummyLoginListAdministrationModel();
#else
			this.model = new LoginListAdministrationsModel();
#endif
        }

        public void Dispose()
        {
            model.Dispose();
        }

        public async void UpdateList()
        {
			view.OnUpdateLoader(true);
            ListaAmministrazioniRequestModel request = new ListaAmministrazioniRequestModel(username);
			ListaAmministrazioniResponseModel response =  await model.GetListaAmministrazioniByUser(request);
            this.ManageResponse(response);
            view.OnUpdateLoader(false);

        }
		#region ManageResponse
		private void ManageResponse(ListaAmministrazioniResponseModel response)
		{
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
			if (responseHelper.IsValidResponse())
			{
				switch (response.Code)
				{
					case 0:
                        view.UpdateList(response.amministrazioni);
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
