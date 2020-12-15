
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using InformaticaTrentinaPCL.Droid.Login;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Login.Network;
using Android.Content.PM;
//using static InformaticaTrentinaPCL.Droid.RecuperaPassword.SelectInstanceRecyclerViewAdapter;

namespace InformaticaTrentinaPCL.Droid.RecuperaPassword

{

    [Activity(Label = "RecuperaPasswordActivity",  ScreenOrientation = ScreenOrientation.Portrait)]
    public class _RecuperaPasswordActivity : BaseActivity// , RecuperaPasswordInstanceView //, IInstanceRecyclerViewItemClickListener
    {

        RecuperaPasswordNativePresenter presenter;
        CustomLoaderUtility loaderUtility;
        RecyclerView recyclerView;

        public static string KEY_INSTANCE_SAVED = "KEY_INSTANCE_SAVED";
        public static string KEY_INSTANCE_URL = "KEY_INSTANCE_URL";

        protected override int LayoutResource => Resource.Layout.activity_login_password_dimenticata;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

         //   presenter = new RecuperaPasswordNativePresenter(this, AndroidNativeFactory.Instance());
            loaderUtility = new CustomLoaderUtility();

/* dd

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView_instanceList);
            recyclerView.HasFixedSize = true;
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));

            presenter.UpdateListInstance();
   */     }

        /// <summary>
        /// Ons the update loader.
        /// </summary>
        /// <param name="isShow">If set to <c>true</c> is show.</param>
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

        /// <summary>
        /// Shows the error.
        /// </summary>
        /// <param name="e">E.</param>
        /// <param name="isLight">If set to <c>true</c> is light.</param>
        public void ShowError(string e, bool isLight = false)
        {
            ShowErrorHelper.Show(this, e, isLight);
        }

        /// <summary>
        /// Updates the list.
        /// </summary>
        /// <param name="list">List.</param>
      /*  public void UpdateList(List<InstanceModel> list)
        {
            recyclerView.SetAdapter(new SelectInstanceRecyclerViewAdapter(list, this));
        }
*/
        /// <summary>
        /// Ons the click.
        /// </summary>
        /// <param name="selectedInstance">Selected instance.</param>
        public void OnClick(InstanceModel selectedInstance)
        {
       //dd     presenter.SetInstancePreferred(selectedInstance);
        }

        /// <summary>
        /// Saves the prefered instance.
        /// </summary>
        /// <param name="description">Description.</param>
        /// <param name="url">URL.</param>
        public void SavePreferredInstance(string description, string url)
        {
    /*dd        ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(KEY_INSTANCE_SAVED, description);
            editor.PutString(KEY_INSTANCE_URL, url);
            editor.Apply();

            presenter.OpenViewLogin();
    */    }

        public void OpenLoginView()
        {
            StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
        }

    }

}

