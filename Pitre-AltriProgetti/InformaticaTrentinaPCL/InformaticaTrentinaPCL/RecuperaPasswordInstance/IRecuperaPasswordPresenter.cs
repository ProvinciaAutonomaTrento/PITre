using System;
using InformaticaTrentinaPCL.ChangePassword.MVP;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.MVP;

namespace InformaticaTrentinaPCL.RecuperaPassword
{
    public interface IRecuperaPasswordPresenter : ILoginPresenter , IChangePasswordPresenter
    {
        void UpdateOTP(string otp);
       
    }
}
