using System;
using Android.OS;

namespace InformaticaTrentinaPCL.Droid.Core
{
    public interface IAndroidPresenter
    {
        Bundle GetStateToSave();
        void OnRestoreInstanceState(Bundle savedInstanceState);
    }
}
