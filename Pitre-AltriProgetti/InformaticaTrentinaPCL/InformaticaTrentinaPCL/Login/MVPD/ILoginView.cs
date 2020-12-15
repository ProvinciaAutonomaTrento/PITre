using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Login.MVP
{
    public interface ILoginView: IGeneralView
    {
        void OnLoginOK(UserInfo user);
        void ShowChangePassword();
        void EnableButton(bool enabled);
		void ShowListAdministration();
        void OnServerChanged(string message);
        void ShowUpdatePopup(string url);
    }
}
