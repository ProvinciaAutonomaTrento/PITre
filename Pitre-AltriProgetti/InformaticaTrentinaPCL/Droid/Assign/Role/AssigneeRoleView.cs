using System;
using Android.Util;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Utils;
using static InformaticaTrentinaPCL.Droid.Assign.AssigneeView;
using Android.Content;
using InformaticaTrentinaPCL.Droid.Utils;

namespace InformaticaTrentinaPCL.Droid.Assign.Role
{
    public class AssigneeRoleView : FrameLayout
    {
        Button selectRole;
        const string KEY_INSTANCE_STATE = "KEY_INSTANCE_STATE";
        const string KEY_ASSIGNEE = "KEY_ASSIGNEE";
        const string KEY_ASSIGNEE_TYPE = "KEY_ASSIGNEE_TYPE";

        public IAssigneeRoleViewListener Listener { set; get; }

        View assigneeItem;
        ImageView assigneeIV;
        TextView nameTV;
        CheckBox favoriteCB;
        ImageView removeIV;

        AbstractRecipient assignee;

        Context mContext;

        private bool isImageVisible = true;

        private bool disableCheckboxListener = false;
        private bool isShowingModels = false;

        public AssigneeRoleView(Context context) : base(context)
        {
            Init(context);
        }

        public AssigneeRoleView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

            Init(context);
        }

        public AssigneeRoleView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {

            Init(context);
        }

        public void Init(Context ctx)
        {
            mContext = ctx;

            Inflate(mContext, Resource.Layout.view_assignee, this);

            assigneeItem = FindViewById(Resource.Id.include_assigneeItem);
            assigneeIV = FindViewById<ImageView>(Resource.Id.imageView_assignableImage);
            nameTV = FindViewById<TextView>(Resource.Id.textView_assignableName);
            favoriteCB = FindViewById<CheckBox>(Resource.Id.checkbox_favorite);
            removeIV = FindViewById<ImageView>(Resource.Id.imageView_remove);

            removeIV.Click += delegate
            {
                ShowSelectAssigneeButton();
                if (Listener != null)
                    Listener.RemoveAssigneeRoleClick();
            };

            selectRole = FindViewById<Button>(Resource.Id.button_selectAssignee);
            selectRole.Text = isShowingModels ? LocalizedString.CHOOSE_ASSIGNEE_AND_MANDATE.Get() :
                LocalizedString.CHOOSE_ASSIGNEE_ROLE.Get();
            selectRole.Click += delegate
            {
                if (Listener != null)
                    Listener.SelectAssigneeRoleClick();
            };
        }

        public void ShowRemovableAssignee(AbstractRecipient recipient)
        {
            ShowAssigneeRole(recipient);
            showRemoveIV();
        }

        private void showRemoveIV()
        {          
            removeIV.Visibility = ViewStates.Visible;
        }

        private void ShowAssigneeRole(AbstractRecipient pAssignee)
        {     
            if (Listener != null)
            {
                Listener.AssigneeRoleUpdated(pAssignee.getId());
            }
        
            assigneeItem.Visibility = ViewStates.Visible;
            selectRole.Visibility = ViewStates.Gone;

            UIUtility.setTitleSubtitle(nameTV, pAssignee.getTitle(), pAssignee.getSubtitle());
            AssigneeUiHelper.UpdateIcon(assigneeIV, pAssignee.getRecipientType(), isImageVisible);
        }

        public void ShowSelectAssigneeButton()
        {
            this.assignee = null;
            assigneeItem.Visibility = ViewStates.Gone;
            selectRole.Visibility = ViewStates.Visible;
        }

        #region inner classes

        public interface IAssigneeRoleViewListener
        {
            void SelectAssigneeRoleClick();
            void RemoveAssigneeRoleClick();
            void AssigneeRoleUpdated(string idRole);
      
        }
        #endregion
    }
}
