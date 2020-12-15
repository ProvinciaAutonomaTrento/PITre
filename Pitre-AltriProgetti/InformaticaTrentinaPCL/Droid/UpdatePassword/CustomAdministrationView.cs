using System;
using Android.Content;
using Android.OS;
using Android.Support.Constraints;
using Android.Util;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.ChangePassword;
using InformaticaTrentinaPCL.Login;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Droid.UpdatePassword
{
    public class CustomAdministrationView : ConstraintLayout
    {
        const string KEY_INSTANCE_STATE = "KEY_INSTANCE_STATE";
        const string KEY_LOGIN_ADMINISTATION_STATE = "KEY_LOGIN_ADMINISTATION_STATE";
        const string KEY_ADMNISTRATION = "KEY_ADMNISTRATION";

        Context mContext;

        TextView administration;
        ImageView removeAdministration;
        ImageView selectAdministration;

        LoginAdministrationState loginAdministrationState = LoginAdministrationState.DEFAULT;
        AmministrazioneModel administrationModel;

        public CustomAdministrationViewListener listener { get; set; }

        public bool isShowingAdministrator { get; private set; }

        public CustomAdministrationView(Context context) : base(context)
        {
            init(context);
        }

        public CustomAdministrationView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init(context);
        }

        public CustomAdministrationView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            init(context);
        }

        void init(Context context)
        {
            mContext = context;

            Inflate(mContext, Resource.Layout.custom_administration_view, this);

            administration = FindViewById<TextView>(Resource.Id.textView_administration);
            removeAdministration = FindViewById<ImageView>(Resource.Id.imageView_removeAdministration);
            selectAdministration = FindViewById<ImageView>(Resource.Id.imageView_selectAdministration);

            removeAdministration.Click += delegate {
                deleteAdministration();
            };
            
            selectAdministration.Click += delegate {
                listener.OnSelectAdministrationClick();
            };

            
        }

        protected override IParcelable OnSaveInstanceState()
        {
            string serializedModel = JsonConvert.SerializeObject(administrationModel);

            Bundle bundle = new Bundle();
            bundle.PutParcelable(KEY_INSTANCE_STATE, base.OnSaveInstanceState());
            bundle.PutString(KEY_ADMNISTRATION, serializedModel);
            bundle.PutInt(KEY_LOGIN_ADMINISTATION_STATE, (int)loginAdministrationState);

            return bundle;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            if (state != null && state is Bundle)
            {
                Bundle bundle = (Bundle)state;
                
                loginAdministrationState = (LoginAdministrationState) bundle.GetInt(KEY_LOGIN_ADMINISTATION_STATE, 
                                           (int)LoginAdministrationState.DEFAULT);
                
                if (loginAdministrationState != LoginAdministrationState.DEFAULT && bundle.ContainsKey(KEY_ADMNISTRATION))
                {
                    administrationModel = JsonConvert.DeserializeObject<AmministrazioneModel>(bundle.GetString(KEY_ADMNISTRATION));

                    if (administrationModel != null)
                        showAdministration(administrationModel, false);
                    else
                        deleteAdministration(false);
                }

            
                
                state = (IParcelable)bundle.GetParcelable(KEY_INSTANCE_STATE);
            }
            base.OnRestoreInstanceState(state);
        }

        public void showAdministration(AmministrazioneModel pAdministrationModel, bool isUserAction = true)
        {
            administrationModel = pAdministrationModel;
            loginAdministrationState = LoginAdministrationState.SELECTED;

            isShowingAdministrator = true;

            administration.Visibility = ViewStates.Visible;
            removeAdministration.Visibility = ViewStates.Visible;
            selectAdministration.Visibility = ViewStates.Gone;

            administration.Text = this.administrationModel.descrizione;

            Visibility = ViewStates.Visible;

            listener.OnUpdateAdministrationViewState(administrationModel, loginAdministrationState, isUserAction);
        }

        void deleteAdministration(bool isUserAction = true)
        {
            loginAdministrationState = LoginAdministrationState.UNSELECTED;

            administrationModel = null;

            isShowingAdministrator = false;

            administration.Visibility = ViewStates.Visible;
            administration.Text = mContext.GetString(Resource.String.custom_view_administration_select);
            removeAdministration.Visibility = ViewStates.Gone;
            selectAdministration.Visibility = ViewStates.Visible;


            Visibility = ViewStates.Visible;

            listener.OnUpdateAdministrationViewState(administrationModel, loginAdministrationState, isUserAction);
        }

        #region inner interface
        public interface CustomAdministrationViewListener
        {
            void OnSelectAdministrationClick();
            void OnUpdateAdministrationViewState(AmministrazioneModel model, LoginAdministrationState state, bool isUserAction);
        }
        #endregion
    }
}
