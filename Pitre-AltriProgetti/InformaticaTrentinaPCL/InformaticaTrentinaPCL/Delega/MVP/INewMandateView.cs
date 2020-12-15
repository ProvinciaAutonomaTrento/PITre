using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Delega.MVP
{
    public interface INewMandateView : IGeneralView
    {
        void OnNewMandateOK();
        void EnableButton(bool enabled);
        void ShowRolePage();
        void RemoveRoleToo();
    }
}
