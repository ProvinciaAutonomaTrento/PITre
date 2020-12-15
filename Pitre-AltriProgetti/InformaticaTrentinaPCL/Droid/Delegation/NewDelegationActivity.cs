
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using InformaticaTrentinaPCL.Assign;
using InformaticaTrentinaPCL.Delega;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Droid.ActionCompleted;
using InformaticaTrentinaPCL.Droid.Assign;
using InformaticaTrentinaPCL.Droid.Assign.Role;
using InformaticaTrentinaPCL.Droid.CustomDateRange;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Droid.Delegation
{

    [Activity(Label = "@string/app_name")]
    public class NewDelegationActivity : BaseActivity, AssigneeView.IAssigneeViewListener,AssigneeRoleView.IAssigneeRoleViewListener,CustomDateRangeView.IDateRangeSelectedListener, INewMandateView
    {
        AssigneeView assigneeView;
        AssigneeRoleView assigneeRoleView;
        AbstractRecipient currentAssignee;
        //AssignableModel assignee;

        CustomDateRangeView dateRangeView;

        Button confirmButton;

        CustomLoaderUtility loaderUtility;

        INewMandatePresenter presenter;

        public const string IS_DELEGA = "IS_DELEGA";

        protected override int LayoutResource => Resource.Layout.activity_new_delegation;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            loaderUtility = new CustomLoaderUtility();
            presenter = new NewMandatePresenter(this, AndroidNativeFactory.Instance());

            assigneeView = FindViewById<AssigneeView>(Resource.Id.assigneeView_container);
            assigneeView.Listener = this;

            assigneeRoleView = FindViewById<AssigneeRoleView>(Resource.Id.assigneeRoleView_container);
            assigneeRoleView.Listener = this;

            FindViewById<TextView>(Resource.Id.textView_your_role).Text = MainApplication.GetSessionData().userInfo.ruoli[0].descrizione;

            dateRangeView = FindViewById<CustomDateRangeView>(Resource.Id.custom_date_range_view);

            dateRangeView.initializeCustomDateRangeView(this, FragmentManager,true);

            confirmButton = FindViewById<Button>(Resource.Id.button_confirm);

            ImageView buttonClose = FindViewById<ImageView>(Resource.Id.button_close);
            buttonClose.Click += delegate
            {
                SetResult(Result.Canceled);
                Finish();
            };
        }
        
        protected override void OnStart()
        {
            base.OnStart();
            confirmButton.Click += ConfirmButton_Click;
            presenter.OnViewReady();
        }

        protected override void OnStop()
        {
            base.OnStop();
            confirmButton.Click -= ConfirmButton_Click;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if ((resultCode == Result.Ok) && (requestCode == (int)ActivityRequestCodes.SelectDelegate))
            {

                AbstractRecipient assignee = AbstractRecipientHelper.DeserializeAbstractRecipient(
                    data.GetStringExtra(SelectDelegateActivity.SELECTED_DELEGATE_KEY));

                presenter.SetAssignee(assignee);
                assigneeView.ShowRemovableAssignee(assignee);
            }
            else if ((resultCode == Result.Ok) && (requestCode == (int)ActivityRequestCodes.SelectRole))
            {
                AbstractRecipient assignee = AbstractRecipientHelper.DeserializeAbstractRecipient(
                    data.GetStringExtra(AssignActivity.ASSIGNEE_KEY));
                //presenter.SetAssigneeRole(assignee.getId());
                assigneeRoleView.ShowRemovableAssignee(assignee);
            }
        }

        void ConfirmButton_Click(object sender, System.EventArgs e)
        {
            presenter.OnConfirm();
        }

        #region interface IAssigneeViewListener

        public void SelectAssigneeClick()
        {
            Intent intent = new Intent(this, typeof(SelectDelegateActivity));
            StartActivityForResult(intent, (int)ActivityRequestCodes.SelectDelegate);
        }

        public void RemoveAssigneeClick()
        {            
            presenter.SetAssignee(null);      
        }

        public void RemoveAssigneeRoleClick()
        {
            presenter.SetAssigneeRole(null);
        }


        #endregion

        #region interface IDateRangeSelectedListener

        public void OnDateSelected(CustomDateRangeView.DateType dateType, DateTime date, int idView)
        {
            switch(dateType)
            {
                case CustomDateRangeView.DateType.START_DATE:
                //    if(!(date < DateTime.Now))
                    presenter.SetStartDate(date);
                    break;

                case CustomDateRangeView.DateType.END_DATE:
                    presenter.SetEndDate(date);
                    break;
                    
            }

        }
        #endregion

        #region interface INewMandateView

        public void OnNewMandateOK()
        {
            SetResult(Result.Ok);
            Finish();
            Intent intent = new Intent(this, typeof(ActionCompletedActivity));
            intent.PutExtra(ActionCompletedActivity.KEY_ACTION_TYPE, ActionType.MANDATE.id);
            StartActivity(intent);
        }

        public void EnableButton(bool isEnabled)
        {
            confirmButton.Enabled = isEnabled;
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
       
        public void AssigneeUpdated(AbstractRecipient assignee)
        {
            currentAssignee = assignee;
            presenter.SetAssignee(assignee);
       
        }

        public void AssigneeRoleUpdated(string idRole)
        {
            presenter.SetAssigneeRole(idRole);
        }

        public void SelectAssigneeRoleClick()
        {
            presenter.OnSelectedRole(currentAssignee);
        }

        public void ShowRolePage()
        {
            Intent openIntent = SelectRoleActivity.CreateOpenIntent(this, currentAssignee);
            openIntent.PutExtra(SelectRoleActivity.IS_DELEGATION, false);
            StartActivityForResult(openIntent, (int)ActivityRequestCodes.SelectRole);
        }

        public void RemoveRoleToo()
        {
            assigneeRoleView.ShowSelectAssigneeButton();
        }

        public bool ShowUserDefaultRole()
        {
            return false;
        }

        #endregion

    }
}
