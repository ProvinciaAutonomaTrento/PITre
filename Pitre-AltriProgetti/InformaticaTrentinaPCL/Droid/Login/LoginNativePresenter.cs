using System;
using Android.OS;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.MVP;
using InformaticaTrentinaPCL.Login.MVPD;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Droid.Login
{
    public class LoginNativePresenter : LoginPresenter, IAndroidPresenter
    {
        public LoginNativePresenter(ILoginView view, INativeFactory nativeFactory, ILoginViewChooseInstance loginChooseInstanceView) : base(view, nativeFactory, loginChooseInstanceView) { }

        public void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            if(savedInstanceState == null) return;
        }

        public Bundle GetStateToSave()
        {
            Bundle bundle = new Bundle();
            return bundle;
        }
    }
}
