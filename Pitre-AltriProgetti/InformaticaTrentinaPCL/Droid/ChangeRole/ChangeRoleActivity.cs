using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.ChangeRole.MVPD;
using InformaticaTrentinaPCL.Droid.Assign;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Droid.ChangeRole
{

    [Activity(Label = "@string/app_name")]
    public class ChangeRoleActivity : BaseActivity, ChangeRoleRecyclerViewAdapter.IChangeRoleRecyclerViewItemClickListener, IRoleView
    {
		public const string SELECTED_ROLE_KEY = "SELECTED_ROLE_KEY";
		
        protected override int LayoutResource => Resource.Layout.activity_select_role;

        IRolePresenter presenter;

        List<RuoloInfo> roleList;

        RecyclerView rolesRecyclerView;
		CustomLoaderUtility loaderUtility;

		protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

			loaderUtility = new CustomLoaderUtility();
			
            UserInfo user = MainApplication.GetSessionData().userInfo;

            FindViewById<TextView>(Resource.Id.textView_header).SetText(Resource.String.activity_change_role_title);

            AssigneeView userView = FindViewById<AssigneeView>(Resource.Id.assigneeView_container);
            userView.ShowUser(user);

			rolesRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView_roleList);
			rolesRecyclerView.HasFixedSize = true;
			rolesRecyclerView.SetLayoutManager(new LinearLayoutManager(this));

			ImageView close = FindViewById<ImageView>(Resource.Id.button_close);
			close.Click += delegate {
				SetResult(Result.Canceled);
				Finish();
			};

            roleList = user.ruoliFiltered;
            presenter = new RolePresenter(this, AndroidNativeFactory.Instance());
            rolesRecyclerView.SetAdapter(new ChangeRoleRecyclerViewAdapter(roleList, this));
	        
	        TextView currentRole = FindViewById<TextView>(Resource.Id.textView_current_role);
	        currentRole.Visibility =
		        Resources.GetBoolean(Resource.Boolean.isSmartphone) ? ViewStates.Gone : ViewStates.Visible;

            currentRole.Text = LocalizedString.ACTUAL_ROLE.Get();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            presenter.Dispose();
        }
        public void OnClick(View view, int position)
        {
            presenter.SetChangeRole(roleList[position]);	
        }

        public void OnChangeRoleOK(RuoloInfo role)
        {
            Intent.PutExtra(SELECTED_ROLE_KEY, role.descrizione);
            SetResult(Result.Ok, Intent);
			Finish();
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
    }
}
