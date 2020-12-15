using System;
namespace InformaticaTrentinaPCL.Login.MVPD
{
    public interface ILoginNativeDelegate
    {
        void SaveUserInSession(UserInfo user);
    }
}
