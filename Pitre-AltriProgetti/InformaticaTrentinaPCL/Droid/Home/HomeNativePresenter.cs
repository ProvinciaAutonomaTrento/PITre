using System;
using Android.OS;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Filter;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Droid.Home
{
    public class HomeNativePresenter : HomePresenter, IAndroidPresenter
    {
        public HomeNativePresenter(IHomeView view, INativeFactory nf) : base(view, nf) {}

        public Bundle GetStateToSave()
        {
            Bundle bundle = new Bundle();
            bundle.PutInt(Key.TAB_POSITION, (int)tab);
            return bundle;
        }

        public void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            if(savedInstanceState == null) return;

            int savedTabPosition = savedInstanceState.GetInt(Key.TAB_POSITION, -1);
            if(savedTabPosition >= 0){
                tab = (Tab)savedTabPosition;
            }
        }

        class Key{
            internal static string TAB_POSITION = "TAB_POSITION";
        }
    }
}
