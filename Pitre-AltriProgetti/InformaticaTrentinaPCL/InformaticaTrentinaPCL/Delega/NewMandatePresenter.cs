using System;
using InformaticaTrentinaPCL.Assign;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Delega
{
    public class NewMandatePresenter : INewMandatePresenter
    {

        protected INewMandateView view;
        protected INewMandateModel model;
        protected SessionData sessionData;
        protected IReachability rechability;

        //--- Items to save: ---------------------
        protected DateTime startDate;
        protected DateTime endDate;
        protected AbstractRecipient assignableModel;
        //----------------------------------------

        public NewMandatePresenter(INewMandateView view, INativeFactory nativeFactory)
        {
            this.view = view;
#if CUSTOM
            this.model = new DummyNewMandateModel();
#else
            //TODO implements
            this.model = new NewMandateModel();
#endif
            this.sessionData = nativeFactory.GetSessionData();
            this.rechability = nativeFactory.GetReachability();
        }

        public async void OnConfirm()
        {
            var body = new NewMandateRequestModel.Body(startDate, endDate, assignableModel.getId(), sessionData.userInfo.idPeople, assignableModel.GetIdRuoloDelegato());
            var request = new NewMandateRequestModel(body, sessionData.userInfo.token);
            NewMandateResponseModel response = await model.DoNewMandate(request);

            ResponseHelper responseHelper = new ResponseHelper(view, response,rechability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0: view.OnNewMandateOK(); 
                        break;
                    case 1:
                        view.ShowError(LocalizedString.NEW_MANDATE_CREATION_ERROR.Get());
                        break;
                    case 2:
                        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                    case 3:
                        view.ShowError(LocalizedString.MANDATE_OVERLAP.Get());
                        break;
                    default: 
                        view.ShowError(LocalizedString.SERVICE_ERROR.Get()); 
                        break;
                }
            }
        }

        #region InterfaceImplementation
        public void SetAssignee(AbstractRecipient assegnee)
        {
            if (assegnee == null)
            {
                view.RemoveRoleToo();
            }
            this.assignableModel = assegnee;
            UpdateConfirmButton();
        }

        public void SetAssigneeRole(string idRole)
        {
            this.assignableModel.SetIdRuoloDelegato(idRole);
            UpdateConfirmButton();
        }

        public void SetEndDate(DateTime dateTime)
        {
            this.endDate = dateTime;
            UpdateConfirmButton();
        }

        public void SetStartDate(DateTime dateTime)
        {
            this.startDate = dateTime;
            UpdateConfirmButton();
        }

        public void OnViewReady()
        {
            this.UpdateConfirmButton();
        }
        public void Dispose()
        {
            model.Dispose();
        }
        #endregion

        protected void UpdateConfirmButton()
        {
            bool isButtonEnabled = endDate.IsSet() && startDate.IsSet() && assignableModel != null;
            view.EnableButton(isButtonEnabled);
        }
    
        public void OnSelectedRole(AbstractRecipient assignee)
        {
            if (assignee == null)
            {
                view.ShowError(LocalizedString.MESSAGE_NO_ASSIGNEE_SELECTED.Get());
            }
            else
            {
                view.ShowRolePage();
            }
        }
    }
}
