using System;
using Android.Support.V4.Widget;

namespace InformaticaTrentinaPCL.Droid.Core
{
    class PullToRefreshListener : Java.Lang.Object, SwipeRefreshLayout.IOnRefreshListener
    {
        AbstractListFragment container;

        public PullToRefreshListener(AbstractListFragment container)
        {
            this.container = container;
        }

        public void OnRefresh()
        {
            container.reloadData();
        }
    }
}
