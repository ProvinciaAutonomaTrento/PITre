using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.MVPD
{
    public interface IHomeView:IGeneralView
    {
        void OnLogoutOk();
        void DoShowMessage(string message);
    }
}
