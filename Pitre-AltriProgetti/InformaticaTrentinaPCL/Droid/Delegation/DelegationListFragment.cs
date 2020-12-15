using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Droid.Home;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Droid.Utils.SwipeRecyclerView;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Utils;
using static InformaticaTrentinaPCL.Droid.Delegation.DelegationRecyclerViewAdapter;

namespace InformaticaTrentinaPCL.Droid.Delegation
{
    public class DelegationListFragment : AbstractListFragment, IListaDelegheView, 
        IDelegationRecyclerViewItemClickListener, RevokeMandateBottomSheetDialog.IRevokeMandateListener
    {

        private CustomLoaderUtility loaderUtility;
        RecyclerSwipeListView delegationRecyclerView;
        SwipeRefreshLayout swipeRefreshLayout;
        DelegationRecyclerViewAdapter delegationRecyclerViewAdapter;
        IListaDeleghePresenter presenter;

        public DelegationListFragment() { }

        public static DelegationListFragment CreateInstance()
        {
            Bundle bundle = new Bundle();
            DelegationListFragment fragment = new DelegationListFragment();
            fragment.Arguments = bundle;
            return fragment;
        }

        public override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (this.Arguments != null && !this.Arguments.IsEmpty)
            {
            }
            presenter = new DelegaPresenter(this, AndroidNativeFactory.Instance());
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_delegationList, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            loaderUtility = new CustomLoaderUtility();

            delegationRecyclerView = View.FindViewById<RecyclerSwipeListView>(Resource.Id.recyclerView_delegationList);
            delegationRecyclerView.HasFixedSize = true;
            LinearLayoutManager layoutManager = new LinearLayoutManager(Activity);
            layoutManager.Orientation = (int)Orientation.Vertical;
            DividerItemDecoration dividerItemDecoration = new DividerItemDecoration(delegationRecyclerView.Context, layoutManager.Orientation);
            delegationRecyclerView.AddItemDecoration(dividerItemDecoration);
            dividerItemDecoration.SetDrawable(Resources.GetDrawable(Resource.Drawable.line_divider_delega));
            delegationRecyclerView.SetLayoutManager(layoutManager);
            delegationRecyclerViewAdapter = new DelegationRecyclerViewAdapter(this);
            delegationRecyclerView.SetAdapter(delegationRecyclerViewAdapter);

            swipeRefreshLayout = View.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetOnRefreshListener(new PullToRefreshListener(this));
            
            string userID = MainApplication.GetSessionData().userInfo.idPeople;
            presenter.GetDelegaDocumentsList();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            presenter.Dispose();
        }

        #region abstractclass AbstractListFragment
        public override void reloadData()
        {
            presenter.OnPullToRefresh();
        }
        #endregion

        #region interface IDelegaView
        public void UpdateList(List<DelegaDocumentModel> list)
        {
            delegationRecyclerView.resetSwipe();
            delegationRecyclerViewAdapter.addDelegationList(list);
            swipeRefreshLayout.Refreshing = false;
        }

        public void ShowError(string e, bool isLight)
        {
            ShowErrorHelper.Show(this.Activity, e, isLight);
        }

        public void OnUpdateLoader(bool isShow)
        {
            if (isShow)
            {
                loaderUtility.showLoader(Activity);
            }
            else
            {
                loaderUtility.hideLoader();
            }
        }
        #endregion

        #region interface IDelegationRecyclerViewItemClickListener
        public void OnItemClick(int position)
        {

        }

        public void OnRevocationClick(int position)
        {
            delegationRecyclerView.resetSwipe();

            DelegaDocumentModel delega = delegationRecyclerViewAdapter.GetDocumentModelAt(position);
            RevokeMandateBottomSheetDialog bottomSheetDialog = 
                new RevokeMandateBottomSheetDialog(Activity, this, delega);
            bottomSheetDialog.Show();
        }

        public void OnRevoke(DelegaDocumentModel delegaDocumentModel)
        {
            presenter.DoRevoke(delegaDocumentModel);
        }

        public void OnLastDelegationInListShown()
        {
            /* Do nothing: non abbiamo la paginazione */
        }

        public void OnRevokeOk()
        {
            reloadData();
        }
        #endregion

    }
}
