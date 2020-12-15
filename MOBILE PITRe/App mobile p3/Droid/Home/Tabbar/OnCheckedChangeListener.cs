using System;
using Android.Widget;
using static InformaticaTrentinaPCL.Droid.Home.Tabbar.CustomBottomNavigationView;

namespace InformaticaTrentinaPCL.Droid.Home.Tabbar
{
    public class OnCheckedChangeListener : Java.Lang.Object, RadioGroup.IOnCheckedChangeListener
    {
        IOnCustomBottomNavigationItemSelected listener;
        public OnCheckedChangeListener(IOnCustomBottomNavigationItemSelected listener)
        {
            this.listener = listener;
        }

        public void OnCheckedChanged(RadioGroup group, int checkedId)
        {
            listener?.OnCustomBottomNavigationItemSelected(checkedId, TabMapper.GetTabFromLayout(checkedId));
        }
    }
}
