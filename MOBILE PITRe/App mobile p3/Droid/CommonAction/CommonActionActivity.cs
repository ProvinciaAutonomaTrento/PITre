
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

using InformaticaTrentinaPCL.Droid.ActionCompleted;
using InformaticaTrentinaPCL.Droid.DocumentDetail;
using InformaticaTrentinaPCL.Droid.Home;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.CommonAction.MVP;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.CommonAction;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace InformaticaTrentinaPCL.Droid.CommonAction
{
    [Activity(Label = "@string/app_name")]
    public class CommonActionActivity : BaseActivity, IActionView
    {
        public const string KEY_CONFIGURE_ACTIVITY_FOR = "KEY_CONFIGURE_ACTIVITY_FOR";
        public const string KEY_DOCUMENT = "KEY_DOCUMENT";
		public const string KEY_DOCUMENT_INFO = "KEY_DOCUMENT_INFO";

        IActionPresenter presenter;

        CustomLoaderUtility loaderUtility;

        ActionType actionType;
        AbstractDocumentListItem _abstractDocument = null;
        SectionType documentInfo;

        TextView headerTextView;
        DocumentDetailView documentDetailView;
        EditText noteEditText;
        Button confirmButton;

        protected override int LayoutResource => Resource.Layout.activity_common_action;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            getIntentParams();

            loaderUtility = new CustomLoaderUtility();

            headerTextView = FindViewById<TextView>(Resource.Id.textView_header);
            documentDetailView = FindViewById<DocumentDetailView>(Resource.Id.documentDetailView);
            noteEditText = FindViewById<EditText>(Resource.Id.editText_notes);
            confirmButton = FindViewById<Button>(Resource.Id.button_confirm);

            Toolbar.SetNavigationIcon(Resource.Drawable.ic_ico_back_grigia);

            Toolbar.NavigationClick += delegate
            {
                Finish();
            };

            SetSectionTitle(actionType.titleBar);

            headerTextView.Text = actionType.SetDescriptionForTypeDocument(actionType.description, _abstractDocument.tipoDocumento);

            documentDetailView.showDocumentDetails(_abstractDocument);

            noteEditText.TextChanged += delegate
            {
                presenter.UpdateNote(noteEditText.Text);
            };

            confirmButton.Text = actionType.confirmButton;

            confirmButton.Click += delegate
            {
                presenter.ButtonConfirm(_abstractDocument);
            };
        
        }

        protected override void OnStart()
        {
            base.OnStart();
            presenter.OnViewReady();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            presenter.Dispose();
        }

        void getIntentParams() 
        {
            int actionTypeId = Intent.GetIntExtra(KEY_CONFIGURE_ACTIVITY_FOR, -1);
            string stringDocument = Intent.GetStringExtra(KEY_DOCUMENT);
            documentInfo = (SectionType)Intent.GetIntExtra(KEY_DOCUMENT_INFO, -1);

            if (actionTypeId < 0 || (string.IsNullOrEmpty(stringDocument)) || (int)documentInfo==-1)
            {
                throw new Exception("One or more params missing in CommonActionActivity invocation");
            }

            actionType = ActionType.GetFromId(actionTypeId);

            if (actionType == ActionType.ACCEPT)
                presenter = new ActionAccettaPresenter(this, AndroidNativeFactory.Instance());
            else if (actionType == ActionType.ACCEPT_AND_ADL)
                presenter = new ActionADLPresenter(this, AndroidNativeFactory.Instance());
            else if (actionType == ActionType.REFUSE)
                presenter = new ActionRifiutaPresenter(this, AndroidNativeFactory.Instance());
            else
                throw new Exception("Presenter not initialized");

            switch (documentInfo)
            {
                case SectionType.TODO:
                    _abstractDocument = JsonConvert.DeserializeObject<ToDoDocumentModel>(stringDocument);
                    break;

                case SectionType.SIGN:
                    _abstractDocument = JsonConvert.DeserializeObject<SignDocumentModel>(stringDocument);
                    break;

            }
        }

        public void CompletedActionOK(Dictionary<string, string> extra)
        {
            Intent.PutExtra(KEY_DOCUMENT_INFO, (int)documentInfo);
            SetResult(Result.Ok, Intent);
            Bundle bundle = Tools.ConvertDictionaryInBundle(extra);
            Intent intent = new Intent(this, typeof(ActionCompletedActivity));
            intent.PutExtra(ActionCompletedActivity.KEY_ACTION_TYPE, actionType.id);
            intent.PutExtras(bundle);
            StartActivity(intent);

            Finish();
        }

        public void EnableConfirmButton(bool enabled)
        {
            confirmButton.Enabled = enabled;
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
