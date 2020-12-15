
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Constraints;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.ActionCompleted;
using InformaticaTrentinaPCL.Droid.DocumentDetail;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Signature;
using InformaticaTrentinaPCL.Signature.MVP;
using InformaticaTrentinaPCL.Utils;
using Java.Util;
using Newtonsoft.Json;
using static InformaticaTrentinaPCL.Home.Network.LibroFirmaResponseModel;

namespace InformaticaTrentinaPCL.Droid.Sign
{
    [Activity(Label = "@string/app_name")]
    public class SignActivity : BaseActivity, ISignatureView
    {
        public const string KEY_CONFIGURE_ACTIVITY_FOR = "KEY_CONFIGURE_ACTIVITY_FOR";
        public const string KEY_DOCUMENT = "KEY_DOCUMENT";
        public const string KEY_DOCUMENT_INFO = "KEY_DOCUMENT_INFO";
        public const string KEY_LIST_DOCUMENT_OTP_SIGN = "KEY_LIST_DOCUMENT_OTP_SIGN";
        public const string TOTAL_RECORD_COUNT_SIGNED = "TOTAL_RECORD_COUNT_SIGNED";
        public const string KEY_SIGN_TYPE = "KEY_SIGN_TYPE";
        public const string KEY_SIGN_FLOW = "KEY_SIGN_FLOW";

        SignaturePresenter presenter;

        ActionType actionType;
        SectionType documentInfo;

        TextView textViewFiscalCode;
        TextView headerTextView;
        Button buttonNotRememberPin;
        DocumentDetailView documentDetailView;
        EditText aliasEditText;
        EditText domainEditText;
        EditText pinEditText;
        EditText otpEditText;
        Button requestOTP;
        Button confirmButton;
        ConstraintLayout mConstraintLayout;
        SignFlowType signFlowType;

        CustomLoaderUtility loaderUtility;

        AbstractDocumentListItem _abstractDocument = null;
        private List<AbstractDocumentListItem> documentToSignList = new List<AbstractDocumentListItem>();
        
        protected override int LayoutResource => Resource.Layout.activity_sign;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            GetIntentParams();

            loaderUtility = new CustomLoaderUtility();

            Toolbar.SetNavigationIcon(Resource.Drawable.ic_ico_back_grigia);

            Toolbar.NavigationClick += delegate
            {
                Finish();
            };

            buttonNotRememberPin = FindViewById<Button>(Resource.Id.button_not_remember_pin);
            textViewFiscalCode = FindViewById<TextView>(Resource.Id.textView_cod_fisc_str);
            headerTextView = FindViewById<TextView>(Resource.Id.textView_header);
            documentDetailView = FindViewById<DocumentDetailView>(Resource.Id.documentDetailView);
            aliasEditText = FindViewById<EditText>(Resource.Id.editText_alias);
            domainEditText = FindViewById<EditText>(Resource.Id.editText_domain);
            pinEditText = FindViewById<EditText>(Resource.Id.editText_pin);
            otpEditText = FindViewById<EditText>(Resource.Id.editText_otp);
            requestOTP = FindViewById<Button>(Resource.Id.button_request_otp);
            confirmButton = FindViewById<Button>(Resource.Id.button_confirm);
            mConstraintLayout = FindViewById<ConstraintLayout>(Resource.Id.constraintLayout);
         
            presenter = new SignaturePresenter(this, AndroidNativeFactory.Instance());

            SetSectionTitle(actionType.titleBar);

            headerTextView.Text = actionType.SetDescriptionForTypeDocument(actionType.description, _abstractDocument != null ? _abstractDocument.tipoDocumento : TypeDocument.NONE);

            string strAlias = MainApplication.GetSessionData().userInfo.signConfiguration.alias;
            string strDomain = MainApplication.GetSessionData().userInfo.signConfiguration.dominio;
            bool otpAllowed = MainApplication.GetSessionData().userInfo.signConfiguration.isOTPAllowed;

            aliasEditText.TextChanged += delegate {
                presenter.UpdateAlias(aliasEditText.Text);
            };

            domainEditText.TextChanged += delegate {
                presenter.UpdateDomain(domainEditText.Text);
            };

            pinEditText.TextChanged += delegate {
                presenter.UpdatePIN(pinEditText.Text);
            };
            otpEditText.TextChanged += delegate {
                presenter.UpdateOTP(otpEditText.Text);
            };

            aliasEditText.Text = string.IsNullOrEmpty(strAlias) ? "" : strAlias;
            domainEditText.Text = string.IsNullOrEmpty(strDomain) ? "" : strDomain;           

            requestOTP.Visibility = otpAllowed ? ViewStates.Visible : ViewStates.Gone;

            requestOTP.Click += delegate
            {
                presenter.RequestOTP();
            };

            confirmButton.Text = actionType.confirmButton;
            confirmButton.Click += delegate
            {
                if (actionType == ActionType.VIEW_OTP)
                {
                    presenter.SignDocumentsHSM(documentToSignList, signFlowType);
                }
                else
                {
                    presenter.SignDocument(_abstractDocument);
                }
            };

            ConfigureDocumentsView();
        }

        public void OnOTPRequested()
        {
            Toast.MakeText(this, LocalizedString.OTP_RICHIESTO_CON_SUCCESSO.Get(), ToastLength.Short).Show();
        }

        public void ConfigureDocumentsView()
        {
            if (actionType != ActionType.VIEW_OTP)
            {
                ConfigurePropertiesIsNotNewOtpSection();
            }
            else
            {
                ConfigurePropertiesIsNewOtpSection();
            }
        }

        public void ConfigurePropertiesIsNewOtpSection()
        {
            documentDetailView.Visibility = ViewStates.Gone;
            headerTextView.Visibility = ViewStates.Gone;
            textViewFiscalCode.SetPadding(0, 20, 0, 0);
            buttonNotRememberPin.Visibility = ViewStates.Visible;
            buttonNotRememberPin.SetTextColor(Color.ParseColor("#c5c5c5"));
        }

        public void ConfigurePropertiesIsNotNewOtpSection()
        {
            documentDetailView.showDocumentDetails(_abstractDocument);
            buttonNotRememberPin.Visibility = ViewStates.Gone;

            SetConstraintPinEditText();
        }

        public void SetConstraintPinEditText()
        {
            ConstraintSet constraintSet = new ConstraintSet();
            constraintSet.Clone(mConstraintLayout);
            constraintSet.Connect(pinEditText.Id, ConstraintSet.Right, ConstraintSet.ParentId, ConstraintSet.Right, 0);
            constraintSet.ApplyTo(mConstraintLayout);
        }

        protected override void OnStart()
        {
            base.OnStart();
            presenter.OnViewReady();
        }

        protected override void OnStop()
        {
            base.OnStop();
            presenter.Dispose();
        }

        void GetIntentParams()
        {
            int actionTypeId = Intent.GetIntExtra(KEY_CONFIGURE_ACTIVITY_FOR, -1);

            if(actionTypeId < 0)
            {
                throw new Exception("One or more params missing in AssignActivity invocation");
            }
            actionType = ActionType.GetFromId(actionTypeId);

            signFlowType = (SignFlowType)Enum.Parse(typeof(SignFlowType),Intent.GetStringExtra(KEY_SIGN_FLOW));

            if (actionType != ActionType.VIEW_OTP)
            {
                string stringDocument = Intent.GetStringExtra(KEY_DOCUMENT);
                documentInfo = (SectionType)Intent.GetIntExtra(KEY_DOCUMENT_INFO, -1);

                if (string.IsNullOrEmpty(stringDocument) || (int)documentInfo == -1)
                    throw new Exception("One or more params missing in AssignActivity invocation");

                switch (documentInfo)
                {
                    case SectionType.ADL:
                        _abstractDocument = JsonConvert.DeserializeObject<AdlDocumentModel>(stringDocument);
                        break;

                    case SectionType.SEARCH:
                        _abstractDocument = JsonConvert.DeserializeObject<RicercaDocumentModel>(stringDocument);
                        break;

                    case SectionType.SIGN:
                        _abstractDocument = JsonConvert.DeserializeObject<SignDocumentModel>(stringDocument);
                        break;

                    default:
                        throw new Exception("Unsupported SectionType");
                }
            }

            string documentListRaw = Intent.GetStringExtra(KEY_LIST_DOCUMENT_OTP_SIGN);
            if (documentListRaw != null)
            {
                List<SignDocumentModel> signDocumentModels = JsonConvert.DeserializeObject<List<SignDocumentModel>>(documentListRaw).ToList();
                foreach (var d in signDocumentModels)
                {
                    documentToSignList.Add(d);
                }
            }
        }

        #region interface ISignatureView
        public void EnableRequestOTPButton(bool enabled)
        {
            requestOTP.Enabled = enabled;
        }

        public void EnableSignatureButton(bool enabled)
        {
            confirmButton.Enabled = enabled;
        }

        public void OnUpdateLoader(bool isShow)
        {
            if (isShow)
                loaderUtility.showLoader(this);
            else
                loaderUtility.hideLoader();
        }

        public void ShowError(string e, bool isLight)
        {
            ShowErrorHelper.Show(this, e, isLight);
        }

        public void SignatureDone(Dictionary<string, string> extra)
        {
            SetResult(Result.Ok, Intent);
            Bundle bundle = Tools.ConvertDictionaryInBundle(extra);
            Intent intent = new Intent(this, typeof(ActionCompletedActivity));
            intent.PutExtra(ActionCompletedActivity.KEY_ACTION_TYPE, actionType.id);     
            intent.PutExtras(bundle);
            StartActivity(intent);
            Finish();
        }


        #endregion
    }
}
