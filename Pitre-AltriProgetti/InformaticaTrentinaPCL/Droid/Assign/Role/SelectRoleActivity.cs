
using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Assegna;
using InformaticaTrentinaPCL.Assegna.MVPD;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.Droid.Delegation;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Utils;
using Java.Lang;

using Newtonsoft.Json;
using static InformaticaTrentinaPCL.Droid.Assign.AssigneeView;

namespace InformaticaTrentinaPCL.Droid.Assign.Role
{
    [Activity(Label = "@string/app_name")]
    public class SelectRoleActivity : BaseActivity, ISelectRoleView, RecipientRecyclerViewAdapter.IRecipientRecyclerViewItemClickListener, IAssigneeFavoriteListener
    {
 
	    public static Intent CreateOpenIntent(Context context, AbstractRecipient abstractRecipient)
	    {
		    string json = JsonConvert.SerializeObject(abstractRecipient);
		    Intent intent = new Intent(context, typeof(SelectRoleActivity));
		    intent.PutExtra(AssignActivity.ASSIGNEE_KEY, json);
		    // preferito passato tramite Extra perché non gestito nel parser
		    intent.PutExtra(IS_FAVORITE, abstractRecipient.isPreferred());
		    return intent;
	    }
	    
	    protected override int LayoutResource => Resource.Layout.activity_select_role;

		ISelectRolePresenter presenter;

	    AssigneeView assigneeView;

		RecyclerView assignableRecyclerView;
		CustomLoaderUtility loaderUtility;

	    private AbstractRecipient recipient;
	    private RecipientRecyclerViewAdapter adapter;    
	    public const string IS_FAVORITE = "IS_FAVORITE";	
        public const string IS_DELEGATION= "IS_DELEGATION";

		protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

	        if (!Intent.HasExtra(AssignActivity.ASSIGNEE_KEY))
	        {
		        throw new RuntimeException("ASSIGNEE_KEY param missing in SelectRoleActivity invocation");
	        }

	        recipient = AbstractRecipientHelper.DeserializeAbstractRecipient(Intent.GetStringExtra(AssignActivity.ASSIGNEE_KEY));
	        
	        recipient.setPreferred(Intent.GetBooleanExtra(IS_FAVORITE, true));
	        
            loaderUtility = new CustomLoaderUtility();
            assigneeView = FindViewById<AssigneeView>(Resource.Id.assigneeView_container);

            assignableRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView_roleList);
			assignableRecyclerView.HasFixedSize = true;
			assignableRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
	        adapter = new RecipientRecyclerViewAdapter(this, true);
	        assignableRecyclerView.SetAdapter(adapter);

            ImageView back = FindViewById<ImageView>(Resource.Id.button_back);
	        back.Visibility = ViewStates.Visible;    
			back.Click += delegate
			{
				presenter.OnBackPressed();
			};
		
	        ImageView close = FindViewById<ImageView>(Resource.Id.button_close);
			close.Click += delegate
			{
				SetResult(Result.FirstUser);
				Finish();
			};

	        assigneeView.ShowFavoriteAssignee(recipient);
			presenter = new SelectRolePresenter(this, AndroidNativeFactory.Instance(), recipient);

	        assigneeView.Click += delegate
	        {
		        OnClick(recipient);
	        };
	        
	        assigneeView.favoriteListener = this;
	        presenter.OnViewReady();
        }

	    public void GoBack(AbstractRecipient abstractRecipient, bool isFavoriteChanged)
	    {
		    Intent data = new Intent();
		    if (isFavoriteChanged)
		    {
			    string json = JsonConvert.SerializeObject(recipient);
			    data.PutExtra(AssignActivity.ASSIGNEE_KEY, json);
		    }
		    SetResult(Result.Canceled, data);
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

	    public void UpdateView(AbstractRecipient user, List<RuoloInfo> listRoles)
	    {
		    recipient = user;
		    assigneeView.ShowFavoriteAssignee(recipient);

            SetEnableAssigneeView();

		    List<AbstractRecipient> list = new List<AbstractRecipient>();
		    list.AddRange(listRoles);
            adapter.OnDatasetChanged(list, ListType.ROLE);
        }

        public void SetEnableAssigneeView()
        {
            bool isEnabled = Intent.GetBooleanExtra(IS_DELEGATION, true);
            assigneeView.Enabled = isEnabled;
        }

        public void OnFavoriteError(AbstractRecipient recipient)
	    {
  			assigneeView.UpdateRecipient(recipient);
	    }

	    public void ShowFavoriteError(string message)
	    {
		    Toast.MakeText(this, message, ToastLength.Short).Show();
	    }

	    #region RecipientRecyclerViewAdapter.IRecipientRecyclerViewItemClickListener
	    public void OnClick(AbstractRecipient selectedRecipient)
	    {
		    Intent.PutExtra(AssignActivity.ASSIGNEE_KEY, JsonConvert.SerializeObject(selectedRecipient));
		    SetResult(Result.Ok, Intent);
		    Finish();
	    }

	    public override void OnBackPressed()
	    {
		    presenter.OnBackPressed();
	    }

	    public void OnFavoriteClick(AbstractRecipient selectedRecipient, bool isChecked)
	    {
		    //NON Usato perché la lista di ruoli non mostra i preferiti.
		    throw new NotImplementedException();
	    }

        public void OnFavoriteChanged(bool isChecked)
        {
	        presenter.setFavorite(isChecked);
        }
        #endregion

    }
}
