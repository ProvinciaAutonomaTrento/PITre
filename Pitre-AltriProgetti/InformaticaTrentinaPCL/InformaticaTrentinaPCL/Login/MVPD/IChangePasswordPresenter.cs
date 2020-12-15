using System;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.MVP;

namespace InformaticaTrentinaPCL.ChangePassword.MVP
{
    public interface IChangePasswordPresenter : ILoginPresenter
    {
        void UpdateOldPassword(string oldPassword);
        void UpdateNewPassword(string newPassword);
        void UpdateRepeatedNewPassword(string repeatedNewPassword);
    }
}
