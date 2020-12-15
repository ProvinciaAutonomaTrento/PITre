using System;
using Android.Content;
using Android.Util;
using Android.Widget;
using InformaticaTrentinaPCL.Home;

namespace InformaticaTrentinaPCL.Droid.Home.Tabbar
{

    /// <summary>
    /// Custom bottom navigation view. Classe utilizzata per la gestione della bottom bar.
    /// </summary>
    public class CustomBottomNavigationView : RelativeLayout
    {
        Context mContext;
        RadioGroup radioGroup;

        public CustomBottomNavigationView(Context context) : base(context)
        {
            init(context);
        }
        public CustomBottomNavigationView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init(context);
        }
        public CustomBottomNavigationView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            init(context);
        }

        void init(Context ctx)
        {
            mContext = ctx;
            Inflate(mContext, Resource.Layout.custom_bottom_navigation_view, this);
            radioGroup = FindViewById<RadioGroup>(Resource.Id.radio_group);
        }

        public void SetListener(IOnCustomBottomNavigationItemSelected listener)
        {
            radioGroup.SetOnCheckedChangeListener(new OnCheckedChangeListener(listener));
        }

        /// <summary>
        /// Metodo utilizzato per selezionare uno specifico item della bottom bar; 
        /// viene di conseguenza invocato il metodo OnCustomBottomNavigationItemSelected del listener per notificare l'item selezionato.
        /// </summary>
        /// <param name="resourceId">Resource identifier.</param>
        public void SelectTab(Tab tab)
        {
            radioGroup.Check(TabMapper.GetLayoutFromTab(tab));
        }

        #region interface

        public interface IOnCustomBottomNavigationItemSelected
        {
            void OnCustomBottomNavigationItemSelected(int resourceId, Tab tab);
        }

        #endregion
    }
}
