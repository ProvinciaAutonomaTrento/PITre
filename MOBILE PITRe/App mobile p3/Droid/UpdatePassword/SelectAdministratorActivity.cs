
using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.Login;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.LoginListAdministration;
using InformaticaTrentinaPCL.LoginListAdministration.MVP;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Droid.UpdatePassword
{
    [Activity(Label = "@string/app_name")]
    public class SelectAdministratorActivity : BaseActivity, SelectAdministrationRecyclerViewAdapter.IAdministrationRecyclerViewItemClickListener, ILoginListAdministrationsView
    {
        public const string KEY_SELECTED_ADMINISTRATOR = "KEY_SELECTED_ADMINISTRATOR";

        ILoginListAdministrationsPresenter presenter;

        CustomLoaderUtility loaderUtility;

        RecyclerView recyclerView;

        protected override int LayoutResource => Resource.Layout.activity_select_administration;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string userParam = Intent.GetStringExtra(LoginActivity.KEY_USERNAME);

            if (string.IsNullOrEmpty(userParam))
                throw new Exception("UserParam missing in UpdatePasswordActivity invocation");
                
            presenter = new LoginListAdministrationsPresenter(this,AndroidNativeFactory.Instance(), userParam);

            loaderUtility = new CustomLoaderUtility();

            FindViewById(Resource.Id.button_back).Click += delegate
            {
                SetResult(Result.Canceled);
                Finish();
            };

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView_administrationList);
            recyclerView.HasFixedSize = true;
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));

            presenter.UpdateList();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            presenter.Dispose();
        }

        #region SelectAdministrationRecyclerViewAdapter.IAdministrationRecyclerViewItemClickListener
        public void OnClick(AmministrazioneModel selectedAdministration)
        {
            Intent.PutExtra(KEY_SELECTED_ADMINISTRATOR, JsonConvert.SerializeObject(selectedAdministration));
            SetResult(Result.Ok, Intent);
            Finish();
        }
        #endregion

        #region ILoginListAdministrationsView
        public void UpdateList(List<AmministrazioneModel> list)
        {
            recyclerView.SetAdapter(new SelectAdministrationRecyclerViewAdapter(list, this));
        }

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
        #endregion
    }
}
