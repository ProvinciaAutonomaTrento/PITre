using System;
using Android.OS;
using InformaticaTrentinaPCL.ChooseInstance;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Droid.ChooseInstance
{
    public class ChooseNativePresenter : ChooseInstancePresenter ,  IAndroidPresenter
    {
        
        public ChooseNativePresenter(IChooseInstanceView view, INativeFactory nativeFactory) : base(view,nativeFactory){ }

        public Bundle GetStateToSave()
        {
            Bundle bundle = new Bundle();
            return bundle;
        }

        public void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            if (savedInstanceState == null) return;
        }
    }
}
