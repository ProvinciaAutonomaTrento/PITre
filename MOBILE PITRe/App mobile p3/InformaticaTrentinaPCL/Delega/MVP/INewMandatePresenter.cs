using System;
using InformaticaTrentinaPCL.Assign;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Delega.MVP
{
    public interface INewMandatePresenter : IBasePresenter
    {
        void SetStartDate(DateTime dateTime);
        void SetEndDate(DateTime dateTime);
        void SetAssignee(AbstractRecipient assegnee);
        void SetAssigneeRole(string idRole);
        void OnSelectedRole(AbstractRecipient assegnee);
        void OnViewReady();
        void OnConfirm();
    }
}
