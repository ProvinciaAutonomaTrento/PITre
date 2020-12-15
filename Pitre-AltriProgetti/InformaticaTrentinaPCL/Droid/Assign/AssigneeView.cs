using System;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;
using Com.Bumptech.Glide;

namespace InformaticaTrentinaPCL.Droid.Assign
{
    public class AssigneeView : FrameLayout
    {
        const string KEY_INSTANCE_STATE = "KEY_INSTANCE_STATE";
        const string KEY_ASSIGNEE = "KEY_ASSIGNEE";
        const string KEY_ASSIGNEE_TYPE = "KEY_ASSIGNEE_TYPE";

        public IAssigneeViewListener Listener { set; get; }
        public IAssigneeFavoriteListener favoriteListener { get; set; }

		Button selectAssignee;

        View assigneeItem;
        ImageView assigneeIV;
		TextView nameTV;
		CheckBox favoriteCB;
		ImageView removeIV;

		AbstractRecipient assignee;
        AssigneeType assigneeType;

        Context mContext;

	    private bool isImageVisible = true;
	    
	    private bool disableCheckboxListener = false;
	    private bool isShowingModels = false;
        private bool isSmistaSection = false;

        public AssigneeView(Context context) : base(context)
		{
			Init(context);
		}
		public AssigneeView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			SetupAttrs(context, attrs);
			Init(context);
		}

		public AssigneeView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
			SetupAttrs(context, attrs);
			Init(context);
		}

	    private void SetupAttrs(Context context, IAttributeSet attrs)
	    {
		    TypedArray a = context.Theme.ObtainStyledAttributes(
			    attrs,
			    Resource.Styleable.AssigneeView,
			    0, 0);

		    try
		    {
			    isImageVisible = a.GetBoolean(Resource.Styleable.AssigneeView_showImage, true);
			    isShowingModels = a.GetBoolean(Resource.Styleable.AssigneeView_isShowingModels, true);
                isSmistaSection = a.GetBoolean(Resource.Styleable.AssigneeView_isSmistaSection, true);
            }
		    finally
		    {
			    a.Recycle();
		    }
	    }

		private void Init(Context ctx)
		{
			mContext = ctx;

            Inflate(mContext, Resource.Layout.view_assignee, this);

            assigneeItem = FindViewById(Resource.Id.include_assigneeItem);
			assigneeIV = FindViewById<ImageView>(Resource.Id.imageView_assignableImage);
			nameTV = FindViewById<TextView>(Resource.Id.textView_assignableName);
            favoriteCB = FindViewById<CheckBox>(Resource.Id.checkbox_favorite);
            removeIV = FindViewById<ImageView>(Resource.Id.imageView_remove);

			removeIV.Click += delegate {
				ShowSelectAssigneeButton();
                if(Listener != null)
				    Listener.RemoveAssigneeClick();
			};

			selectAssignee = FindViewById<Button>(Resource.Id.button_selectAssignee);

            var label = isSmistaSection ? LocalizedString.CHOOSE_ASSIGNEE_AND_MANDATE_SMISTA_SECTION.Get() :
                                         LocalizedString.CHOOSE_ASSIGNEE_AND_MANDATE.Get();

            selectAssignee.Text = isShowingModels ? label : 
													LocalizedString.CHOOSE_ASSIGNEE.Get();
			selectAssignee.Click += delegate {
                if (Listener != null) 
                    Listener.SelectAssigneeClick();
			};
			
			ShowSelectAssigneeButton();

			favoriteCB.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
			{
				if (favoriteListener != null && !disableCheckboxListener)
				{
					favoriteListener.OnFavoriteChanged(checkedChangeEventArgs.IsChecked);
				}
			};
		}

	    protected override IParcelable OnSaveInstanceState()
        {
            Bundle bundle = new Bundle();
            bundle.PutParcelable(KEY_INSTANCE_STATE, base.OnSaveInstanceState());

            if (assignee != null)
            {
                bundle.PutString(KEY_ASSIGNEE, JsonConvert.SerializeObject(assignee));
                bundle.PutInt(KEY_ASSIGNEE_TYPE, (int)assigneeType);
            }

            return bundle;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            if (state != null && state is Bundle)
            {
                Bundle bundle = (Bundle)state;

                if (bundle.ContainsKey(KEY_ASSIGNEE))
                {
	               	assignee = AbstractRecipientHelper.DeserializeAbstractRecipient(bundle.GetString(KEY_ASSIGNEE));
                    assigneeType = (AssigneeType)bundle.GetInt(KEY_ASSIGNEE_TYPE);

                    switch(assigneeType) 
                    {
                        case AssigneeType.FAVORITE:
                            ShowFavoriteAssignee(assignee);
                            break;

                        case AssigneeType.REMOVABLE:
                            ShowRemovableAssignee(assignee);
                            break;
                    }
 
                }

                state = (IParcelable)bundle.GetParcelable(KEY_INSTANCE_STATE);
            }
            base.OnRestoreInstanceState(state);

        }
	    
	    public bool isShowImage() {
		    return isImageVisible;
	    }

	    public void setShowImage(bool showImage) {
		    isImageVisible = showImage;
	    }

		private void ShowAssignee(AbstractRecipient pAssignee)
		{
            this.assignee = pAssignee;

			if (Listener != null)
			{
				Listener.AssigneeUpdated(pAssignee);
			}
			
			assigneeItem.Visibility = ViewStates.Visible;
            selectAssignee.Visibility = ViewStates.Gone;

            UIUtility.setTitleSubtitle(nameTV, assignee.getTitle(), (Listener == null || Listener.ShowUserDefaultRole()) ? assignee.getSubtitle() : null);

			//Rendo i testi cliccabili in blu
			if (assigneeType == AssigneeType.FAVORITE)
			{
				UIUtility.SetTextColor(nameTV, Resource.Color.textBlue);
				favoriteCB.Checked = assignee.isPreferred();
			}
			
			AssigneeUiHelper.UpdateIcon(assigneeIV, assignee.getRecipientType(), isImageVisible);

        }

        public void ShowRemovableAssignee(AbstractRecipient recipient)
        {
	        assigneeType = AssigneeType.REMOVABLE;
            ShowAssignee(recipient);
            showRemoveIV();
        }

        public void ShowSelectAssigneeButton()
        {
            
            this.assignee = null;

            if(Listener != null)
                Listener.AssigneeUpdated(null);

            assigneeItem.Visibility = ViewStates.Gone;
            selectAssignee.Visibility = ViewStates.Visible;
			
		}

        public void ShowFavoriteAssignee(AbstractRecipient assignee)
        {
            assigneeType = AssigneeType.FAVORITE;
            ShowAssignee(assignee);
            showFavoriteIV();
        }

        public void ShowUser(UserInfo user)
		{
			assigneeItem.Visibility = ViewStates.Visible;
			selectAssignee.Visibility = ViewStates.Gone;

            UIUtility.setTitleSubtitle(nameTV, user.descrizione, (Listener == null || Listener.ShowUserDefaultRole()) ? user.ruoli[0].descrizione : null);

            AssigneeUiHelper.UpdateIcon(assigneeIV, AbstractRecipient.RecipientType.USER, isImageVisible);

			favoriteCB.Visibility = ViewStates.Gone;
            removeIV.Visibility = ViewStates.Gone;
			
		}

        private void showFavoriteIV()
        {
	        favoriteCB.Visibility = ViewStates.Visible;
            removeIV.Visibility = ViewStates.Gone;
        }

        private void showRemoveIV()
		{
			favoriteCB.Visibility = ViewStates.Gone;
			removeIV.Visibility = ViewStates.Visible;
		}

	    public void UpdateRecipient(AbstractRecipient recipient)
	    {
		    disableCheckboxListener = true;
		    favoriteCB.Checked = recipient.isPreferred();
		    disableCheckboxListener = false;
	    }	
	    
        #region inner classes

        public interface IAssigneeViewListener
        {
            void SelectAssigneeClick();
            void RemoveAssigneeClick();
            void AssigneeUpdated(AbstractRecipient assignee);
            bool ShowUserDefaultRole();
        }

        public interface IAssigneeFavoriteListener{
            void OnFavoriteChanged(bool isChanged);
        }

        enum AssigneeType
        {
            REMOVABLE,
            FAVORITE
        }

        #endregion
	}
}
