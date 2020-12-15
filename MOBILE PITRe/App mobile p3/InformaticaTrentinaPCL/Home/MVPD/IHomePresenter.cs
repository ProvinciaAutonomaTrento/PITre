using System;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;

namespace InformaticaTrentinaPCL.Home.MVPD
{
    public interface IHomePresenter : IBasePresenter
    {
        void DoLogout();
        void OnImpostazioniTapped();
        Tab GetCurrentTab();
        void UpdateCurrentTab(Tab tab);
    }
}
