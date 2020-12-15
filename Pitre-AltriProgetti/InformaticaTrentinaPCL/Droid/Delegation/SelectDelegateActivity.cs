
using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using InformaticaTrentinaPCL.Delega;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Droid.Search;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Droid.Delegation
{
    [Activity(Label = "@string/app_name")]
    public class SelectDelegateActivity : BaseActivity, CustomEditText.CustomEditTextListener, ISelectMandateAssigneeView, RecipientRecyclerViewAdapter.IRecipientRecyclerViewItemClickListener
    {
        public const string SELECTED_DELEGATE_KEY = "SELECTED_DELEGATE_KEY";
        public const string TITLE_KEY = "TITLE_KEY";

        CustomEditText customEditText;
        TextView favoritesTextView;
        RecyclerView recyclerView;

        CustomLoaderUtility loaderUtility;
        
        ISelectMandateAssigneePresenter presenter;

        RecipientRecyclerViewAdapter adapter;
        
        protected override int LayoutResource => Resource.Layout.activity_select_delegate;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            loaderUtility = new CustomLoaderUtility();

            if (Intent.HasExtra(TITLE_KEY))
            {
                FindViewById<TextView>(Resource.Id.textView_header).Text = Intent.GetStringExtra(TITLE_KEY);
            }

            customEditText = FindViewById<CustomEditText>(Resource.Id.customEditText);
            customEditText.listener = this;
            
            favoritesTextView = FindViewById<TextView>(Resource.Id.textView_favorites);

            ImageView close = FindViewById<ImageView>(Resource.Id.button_close);
            close.Click += delegate {
                SetResult(Result.Canceled);
                Finish();
            };

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.HasFixedSize = true;
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            adapter = new RecipientRecyclerViewAdapter(this);
            recyclerView.SetAdapter(adapter);

            presenter = new SelectMandateAssigneePresenter(this, AndroidNativeFactory.Instance());
            presenter.GetListFavorites();
        }
        
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            String textToSearch = customEditText.GetCurrentSearch();
            if (!String.IsNullOrEmpty(textToSearch))
            {
                presenter.SearchAssignee(textToSearch);
            }
        }

        #region CustomEditText.CustomEditTextListener
        public void OnSearch(string textToSearch)
        {
            presenter.SearchAssignee(textToSearch);
        }

        public void OnDelete()
        {
            presenter.ClearSearch();
        }
        #endregion

        #region ISelectMandateAssigneeView
        
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

        public void UpdateFavoriteList(List<AbstractRecipient> list)
        {
            favoritesTextView.Visibility = ViewStates.Visible;
            adapter.OnDatasetChanged(list, ListType.FAVORITE);
        }

        public void UpdateSearchList(List<AbstractRecipient> list)
        {
            favoritesTextView.Visibility = ViewStates.Gone;
            adapter.OnDatasetChanged(list, ListType.SEARCH);
        }

        public void ClearList()
        {
            adapter.OnDatasetChanged(new List<AbstractRecipient>());
        }

        public void OnFavoriteError(AbstractRecipient recipient)
        {
            Console.WriteLine("OnFavoriteError ");
            adapter.UpdateRecipient(recipient);
        }

        public void ShowFavoriteError(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }

        #endregion

        #region RecipientRecyclerViewAdapter.IRecipientRecyclerViewItemClickListener

        public void OnClick(AbstractRecipient selectedRecipient)
        {
            Intent.PutExtra(SELECTED_DELEGATE_KEY, JsonConvert.SerializeObject(selectedRecipient));
            SetResult(Result.Ok, Intent);
            Finish();
        }

        public void OnFavoriteClick(AbstractRecipient selectedRecipient, bool isChecked)
        {
            presenter.SetFavorite(selectedRecipient, isChecked);
        }
        
        #endregion
    }

}
