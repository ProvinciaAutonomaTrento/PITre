using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using InformaticaTrentinaPCL.Assegna;
using InformaticaTrentinaPCL.Assegna.MVPD;
using InformaticaTrentinaPCL.Droid.Assign.Assignable;
using InformaticaTrentinaPCL.Droid.Assign.Role;
using InformaticaTrentinaPCL.Droid.Delegation;
using InformaticaTrentinaPCL.Droid.Search;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Droid.Assign
{

    [Activity(Label = "@string/app_name")]
    public class SelectAssigneeActivity : BaseActivity, CustomEditText.CustomEditTextListener, IAssignableView, RecipientRecyclerViewAdapter.IRecipientRecyclerViewItemClickListener
    {
        TabLayout tabLayout;
        ViewPager viewPager;

        CustomEditText customEditText;

        AssignablesFragment modelsFragment;
        AssignablesFragment favoritesFragment;

        RecyclerView searchRecyclerView;
        RecipientRecyclerViewAdapter searchAdapter;
            
        CustomLoaderUtility loaderUtility;

        IAssignablePresenter presenter;
        AbstractDocumentListItem document;

        private string currentSearch;
        
        
        int readyFragments = 0;
        
        protected override int LayoutResource => Resource.Layout.activity_select_assignee;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            document = AbstractDocumentListItemHelper.DeserializeAbstractDocumentListItem(Intent.GetStringExtra(AssignActivity.KEY_DOCUMENT));
            
            loaderUtility = new CustomLoaderUtility();
            
            tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            tabLayout.AddTab(tabLayout.NewTab().SetText(Resource.String.activity_select_assignee_models));
            tabLayout.AddTab(tabLayout.NewTab().SetText(Resource.String.activity_select_assignee_favorites));
            tabLayout.TabGravity = TabLayout.GravityFill;

            modelsFragment = RetrieveOrCreateFragment(0);
            favoritesFragment = RetrieveOrCreateFragment(1);
            
            List<Android.Support.V4.App.Fragment> titles = new List<Android.Support.V4.App.Fragment>();
            titles.Add(modelsFragment);
            titles.Add(favoritesFragment);

            viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
            PagerAdapter adapter = new SelectAssigneePagerAdapter(SupportFragmentManager, titles);
            viewPager.Adapter = adapter;
            viewPager.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout));
            tabLayout.AddOnTabSelectedListener(new TabSelectedListener(viewPager));

            customEditText = FindViewById<CustomEditText>(Resource.Id.customEditText);
            customEditText.listener = this;

            searchRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView_search);
            searchRecyclerView.HasFixedSize = true;
            searchRecyclerView.SetLayoutManager(new LinearLayoutManager(this));

            ImageView close = FindViewById<ImageView>(Resource.Id.button_close);
			close.Click += delegate {
                SetResult(Result.Canceled);
				Finish();
			};

            presenter = new AssignablePresenter(this, AndroidNativeFactory.Instance(), document);
            
            searchAdapter = new RecipientRecyclerViewAdapter(this);
            searchRecyclerView.SetAdapter(searchAdapter);
		}

        private AssignablesFragment RetrieveOrCreateFragment(int index)
        {
            string Tag = makeFragmentTag(index);
            return (AssignablesFragment)SupportFragmentManager.FindFragmentByTag(Tag) ?? AssignablesFragment.CreateInstance(); 
        }
        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void notifyFragmentReady()
        {
            readyFragments++;
            if (readyFragments == 2)
            {
                presenter.OnViewReady();
                readyFragments = 0;
            }
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            String textToSearch = customEditText.GetCurrentSearch();
            if (!String.IsNullOrEmpty(textToSearch))
            {
                presenter.SearchCorrispondenti(textToSearch);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            switch (resultCode)
            {
                case Result.Ok: 
                        presenter.OnAssigneeReceivedFromChooser(AbstractRecipientHelper.DeserializeAbstractRecipient(data.GetStringExtra(AssignActivity.ASSIGNEE_KEY)));
                        break;
                
                case Result.FirstUser: 
                        SetResult(Result.Canceled);
                        Finish();
                        break;
                    
                case Result.Canceled:
                    if (data.HasExtra(AssignActivity.ASSIGNEE_KEY))
                    {
                        AbstractRecipient recipient = AbstractRecipientHelper.DeserializeAbstractRecipient(data.GetStringExtra(AssignActivity.ASSIGNEE_KEY));
                        recipient.setPreferred(!recipient.isPreferred());
                        favoritesFragment?.UpdateRecipient(recipient);
                        searchAdapter?.UpdateRecipient(recipient);    
                    }
                    break;
            }                    
        }

        #region CustomEditText.CustomEditTextListener
        public void OnSearch(string textToSearch)
        {
            currentSearch = textToSearch;
            presenter.SearchCorrispondenti(textToSearch);
        }

        public void OnDelete()
        {
            presenter.ClearSearch();
        }
        #endregion

        #region IAssignableView
        public void ShowError(string e, bool isLight)
        {
            ShowErrorHelper.Show(this, e, isLight);
        }

        public void OnUpdateLoader(bool isShow)
        {
            if (isShow)
            {
                loaderUtility.showLoader(this);
            }
            else
            {
                loaderUtility.hideLoader();
            }
        }

        public void UpdateModel(List<AbstractRecipient> listModels)
        {
            modelsFragment.showList(listModels, this, ListType.MODEL);
        }

        public void UpdateFavorite(List<AbstractRecipient> listFavorites)
        {
            favoritesFragment.showList(listFavorites, this, ListType.FAVORITE);
        }

        public void UpdateSearchResults(List<AbstractRecipient> listResults)
        {
            searchAdapter.OnDatasetChanged(listResults, ListType.SEARCH);
        }

        public void ShowTabsView(bool show)
        {
            viewPager.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
            tabLayout.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
        }

        public void ShowSearchView(bool show)
        {
            searchRecyclerView.Visibility = show ? ViewStates.Visible : ViewStates.Gone;
        }

        public void OnFavoriteError(AbstractRecipient recipient)
        {
            favoritesFragment.UpdateRecipient(recipient);
            searchAdapter.UpdateRecipient(recipient);
        }

        public void ShowFavoriteError(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }

        public void OnAssigneeSelected(AbstractRecipient selected)
        {
            string json = JsonConvert.SerializeObject(selected);
            Intent.PutExtra(AssignActivity.ASSIGNEE_KEY, json);
            SetResult(Result.Ok, Intent);
            Finish();
        }

        public void UserSelected(AbstractRecipient userSelected)
        {
            Intent openIntent = SelectRoleActivity.CreateOpenIntent(this, userSelected);
            StartActivityForResult(openIntent, (int)ActivityRequestCodes.SelectRole);
        }

        #endregion
        
        #region IRecipientRecyclerViewItemClickListener
        public void OnClick(AbstractRecipient selectedRecipient)
        {
            presenter.OnSelect(selectedRecipient);
        }

        public void OnFavoriteClick(AbstractRecipient selectedRecipient, bool isChecked)
        {
            presenter.SetFavorite(selectedRecipient, isChecked);
        }
        #endregion
        
        class TabSelectedListener : Java.Lang.Object, TabLayout.IOnTabSelectedListener
        {

            ViewPager viewPager;

            public TabSelectedListener(ViewPager viewPager)
            {
                this.viewPager = viewPager;
            }

            public void OnTabReselected(TabLayout.Tab tab)
            {
            }

            public void OnTabSelected(TabLayout.Tab tab)
            {
                viewPager.SetCurrentItem(tab.Position, true);
            }

            public void OnTabUnselected(TabLayout.Tab tab)
            {
            }
        }

        class SelectAssigneePagerAdapter : FragmentPagerAdapter
        {

            List<Android.Support.V4.App.Fragment> fragments;

            public SelectAssigneePagerAdapter(Android.Support.V4.App.FragmentManager fm, List<Android.Support.V4.App.Fragment> pages)
                : base(fm)
            {
                fragments = pages;
            }


            public override int Count
            {
                get { return fragments.Count; }
            }

            public override Android.Support.V4.App.Fragment GetItem(int position)
            {
                return fragments[position];
            }
        }
 
        /// <summary>
        /// Usato per ripristinare i riferimenti ai fragment dell'Adapter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static String makeFragmentTag(int index)
        {
            int viewPagerId = Resource.Id.viewPager;
            return "android:switcher:" + viewPagerId + ":" + index;
        }
    }
}
