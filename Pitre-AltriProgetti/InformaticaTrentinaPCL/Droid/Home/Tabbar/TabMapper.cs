using System;
using InformaticaTrentinaPCL.Home;

namespace InformaticaTrentinaPCL.Droid.Home.Tabbar
{
    public class TabMapper
    {

        readonly static int[] tabResIds = {
            Resource.Id.item_area_di_lavoro,
            Resource.Id.item_toSign,
            Resource.Id.item_toDo,
            Resource.Id.item_switchTo,
            Resource.Id.item_search
        };

        TabMapper() { }

        public static Tab GetTabFromLayout(int layoutResId){
            int tab = 0;
            for (int i = 0; i < tabResIds.Length; i++)
            {
                if (tabResIds[i] == layoutResId)
                {
                    tab = i;
                    break;
                }
            }
            return (Tab)tab;
        }

        public static int GetLayoutFromTab(Tab tab){
            return tabResIds[(int)tab];
        }
    

    }
}
