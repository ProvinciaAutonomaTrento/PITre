using System;
using InformaticaTrentinaPCL.ChangePassword.MVP;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.MVP;

namespace InformaticaTrentinaPCL.RecuperaPassword
{
    public interface IRecuperaOtpPresenter //: ILoginPresenter
    {
        void GetOTPAsync();
        void UpdateUsername(string username);

    }
}
