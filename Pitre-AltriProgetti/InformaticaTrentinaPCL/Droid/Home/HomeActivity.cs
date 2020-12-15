using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using Android.App;
using Android.OS;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Preferences;
using Android.Provider;
using Android.Support.V4.Content;
using Android.Support.V4.Graphics;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Filter;
using InformaticaTrentinaPCL.Droid.ChangeRole;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Droid.Login;
using InformaticaTrentinaPCL.Droid.CommonAction;
using InformaticaTrentinaPCL.Droid.Assign;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Droid.Delegation;
using InformaticaTrentinaPCL.Droid.Home.Tabbar;
using InformaticaTrentinaPCL.Droid.Sign;
using InformaticaTrentinaPCL.Droid.Search;
using InformaticaTrentinaPCL.Droid.Filter;
using InformaticaTrentinaPCL.Droid.CustomBottomSheet;
using InformaticaTrentinaPCL.Home.ActionDialog;
using InformaticaTrentinaPCL.Utils;
using Java.IO;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Resource.Bitmap;
using Com.Bumptech.Glide.Request;
using Newtonsoft.Json;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using String = System.String;
using Uri = Android.Net.Uri;
using Com.Bumptech.Glide.Load.Engine;
using static InformaticaTrentinaPCL.Home.Network.LibroFirmaResponseModel;

namespace InformaticaTrentinaPCL.Droid.Home
{
    [Activity(Label = "@string/app_name")]
    public class HomeActivity : BaseActivity, IHomeView, CustomBottomNavigationView.IOnCustomBottomNavigationItemSelected, IMenuItemOnMenuItemClickListener, CustomFilterView.ICustomFilterViewListener
    {
        AbstractListFragment fragment = null;
        UserInfo user;

        DrawerLayout drawerLayout;
        private View quitView;

        ActionBarDrawerToggle drawerToggle;

        IMenuItem filterMenuItem;
        IMenuItem addMenuItem;
        IMenuItem showDialogActionsDocuments;

        String[] menuEntries;
        TypedArray menuIcons;

        ImageView userImage;
        TextView userName;
        TextView userRole;
        CustomBottomNavigationView bottomBar;

        CustomBottomSheetDialog customBottomSheetDialog;

        RelativeLayout leftDrawer;

        HomeNativePresenter homePresenter;

        CustomLoaderUtility loaderUtility;

        CustomFilterView filterView;

        private ProfileImageManager profileImageManager;

        protected override int LayoutResource => Resource.Layout.activity_home;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            homePresenter = new HomeNativePresenter(this, AndroidNativeFactory.Instance());
            homePresenter.OnRestoreInstanceState(savedInstanceState);
            loaderUtility = new CustomLoaderUtility();
            profileImageManager = new ProfileImageManager(ApplicationContext);
            menuEntries = this.Resources.GetStringArray(Resource.Array.navigation_drawer_items);
            menuIcons = Resources.ObtainTypedArray(Resource.Array.navigation_drawer_icons);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerlayout_mainContainer);
            quitView = FindViewById(Resource.Id.quit);
            quitView.Click += delegate { homePresenter.DoLogout(); };

            leftDrawer = FindViewById<RelativeLayout>(Resource.Id.relativeLayout_leftDrawer);

            if (drawerLayout != null)
            {
                //Caso normale
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                Toolbar.SetNavigationIcon(Resource.Drawable.ic_burger_menu);

                drawerToggle = new MyActionBarDrawerToggle(this, drawerLayout, Toolbar, 0, 0);
                drawerLayout.AddDrawerListener(drawerToggle);
            }
            else
            {
                //Caso di Tablet in Landscape
                SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                SupportActionBar.SetHomeButtonEnabled(false);
            }
            bottomBar = FindViewById<CustomBottomNavigationView>(Resource.Id.customBottomNavigationView);
            bottomBar.SetListener(this);
            bottomBar.SelectTab(homePresenter.GetCurrentTab());
            ShowNewFragment(homePresenter.GetCurrentTab());

            userImage = FindViewById<ImageView>(Resource.Id.imageView_userImage);
            userName = FindViewById<TextView>(Resource.Id.textView_userName);
            userRole = FindViewById<TextView>(Resource.Id.textView_userRole);

            user = MainApplication.GetSessionData().userInfo;
            UpdateImageProfile(profileImageManager.LoadImage());

            userName.Text = user.descrizione;
            userRole.Text = user.ruoli[0].descrizione;

            userRole.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ChangeRoleActivity));
                StartActivityForResult(intent, (int)ActivityRequestCodes.ChangeRole);
            };

            filterView = FindViewById<CustomFilterView>(Resource.Id.customFilterView);
            filterView.filterViewListener = this;

            //        userImage.Click += delegate { profileImageManager.ShowChooserDialog(this, TakePhotoFromCamera, ChoosePhotoFromGallery, DeletePhoto); };
            userImage.Click += delegate { profileImageManager.ShowChooserDialog(this,  ChoosePhotoFromGallery, DeletePhoto); };
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutAll(homePresenter.GetStateToSave());
            base.OnSaveInstanceState(outState);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            homePresenter.Dispose();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_bar_buttons, menu);

            filterMenuItem = menu.FindItem(Resource.Id.action_filter);
            addMenuItem = menu.FindItem(Resource.Id.action_add);
            showDialogActionsDocuments = menu.FindItem(Resource.Id.action_show_dialog);

            PorterDuff.Mode mode = PorterDuff.Mode.SrcAtop;
            filterMenuItem.Icon.SetColorFilter(Android.Graphics.Color.White, mode);
            addMenuItem.Icon.SetColorFilter(Android.Graphics.Color.White, mode);
            showDialogActionsDocuments.Icon.SetColorFilter(Android.Graphics.Color.White, mode);

            filterMenuItem.SetOnMenuItemClickListener(this);
            addMenuItem.SetOnMenuItemClickListener(this);

            showDialogActionsDocuments.SetOnMenuItemClickListener(this);

            UpdateMenu(homePresenter.GetCurrentTab());
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_filter)
            {
                fragment.RequestOpenFilterView();
            }
            else if (item.ItemId == Resource.Id.action_show_dialog)
            {
                fragment.RequestShowActionsDocuments();
            }
            return base.OnOptionsItemSelected(item);
        }


        #region OnActivityResult 

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Canceled) return;
            if (resultCode == Result.Ok)
            {
                switch (requestCode)
                {
                    case (int)ActivityRequestCodes.ChangeRole:
                        handleChangeRoleActivityResult(data);
                        break;

                    case (int)ActivityRequestCodes.DocumentAction:
                        handleDocumentActionActivityResult(data);
                        break;

                    case (int)ActivityRequestCodes.Assign:
                        handleAssignActivityResult(data);
                        break;

                    case (int)ActivityRequestCodes.NewDelegation:
                        handleNewDelegationActivityResult(data);
                        break;

                    case (int)ActivityRequestCodes.Sign:
                        handleSignActivityResult(data);
                        break;

                    case (int)ActivityRequestCodes.Filter:
                        handleFilterActivityResult(data);
                        break;
                    case (int)ActivityRequestCodes.NewImageFromCamera:
                        OnCaptureImageResult(data);
                        break;
                    case (int)ActivityRequestCodes.NewImageFromGallery:
                        OnSelectFromGalleryResult(data);
                        break;
                }
            }
        }

        void handleChangeRoleActivityResult(Intent data)
        {
            user = MainApplication.GetSessionData().userInfo;
            userRole.Text = user.ruoli[0].descrizione;
            fragment.reloadData();
        }

        void handleDocumentActionActivityResult(Intent data)
        {
            fragment.reloadData();
        }

        void handleAssignActivityResult(Intent data)
        {
            fragment.reloadData();
        }

        void handleNewDelegationActivityResult(Intent data)
        {
            fragment.reloadData();
        }

        void handleSignActivityResult(Intent data)
        {
            fragment.reloadData();
        }

        void handleFilterActivityResult(Intent data)
        {
            FilterModel filter = JsonConvert.DeserializeObject<FilterModel>(data.GetStringExtra(FilterActivity.KEY_FILTER));
            //Setting the filter, a reloadData is performed automatically.
            fragment.SetFilter(filter);
        }

        #endregion

        public override void OnBackPressed()
        {
            if (drawerLayout.IsDrawerOpen(leftDrawer))
            {
                drawerLayout.CloseDrawer(leftDrawer);
            }
            else
            {
                //TODO: verificare se effettuare chiaamta al presenter (logout?)
                MainApplication.GetSessionData().clear();
                Finish();
            }
        }

       

        private void UpdateMenu(Tab tab)
        {
            if (!IsToolbarMenuSet())
            {
                return;
            }

          


            switch (tab)
            {
                case Tab.ADL:
                    filterMenuItem.SetVisible(true);
                    addMenuItem.SetVisible(false);
                    showDialogActionsDocuments.SetVisible(false);

            
                    break;

                case Tab.SIGN:
                    filterMenuItem.SetVisible(true);
                    addMenuItem.SetVisible(false);
                    showDialogActionsDocuments.SetVisible(true);
                    break;

                case Tab.TODO:
                    filterMenuItem.SetVisible(false);
                    addMenuItem.SetVisible(false);
                    showDialogActionsDocuments.SetVisible(false);
                    break;

                case Tab.MANDATES:
                    filterMenuItem.SetVisible(false);
                    addMenuItem.SetVisible(true);
                    showDialogActionsDocuments.SetVisible(false);
                    break;

                case Tab.SEARCH:
                    filterMenuItem.SetVisible(true);
                    addMenuItem.SetVisible(false);
                    showDialogActionsDocuments.SetVisible(false);
                    break;
            }

          }

        private void ShowNewFragment(Tab tab)
        {
            if (filterView != null)
                filterView.Visibility = ViewStates.Gone;

            switch (tab)
            {
                case Tab.TODO:
                    SetPageTitle(GetString(Resource.String.title_todo));
                    fragment = (DocumentListFragment)FragmentManager.FindFragmentByTag(tab.ToString());
                    if (fragment == null)
                    {
                        fragment = DocumentListFragment.CreateInstance(SectionType.TODO);
                    }
                    break;

                case Tab.MANDATES:
                    SetPageTitle(GetString(Resource.String.title_switchTo));
                    fragment = (DelegationListFragment)FragmentManager.FindFragmentByTag(tab.ToString());
                    if (fragment == null)
                    {
                        fragment = DelegationListFragment.CreateInstance();
                    }
                    break;

                case Tab.SIGN:
                    SetPageTitle(GetString(Resource.String.title_sign));
                    fragment = (DocumentListFragment)FragmentManager.FindFragmentByTag(tab.ToString());
                    if (fragment == null)
                    {
                        fragment = DocumentListFragment.CreateInstance(SectionType.SIGN);
                    }
                    break;

                case Tab.ADL:
                    SetPageTitle(GetString(Resource.String.title_area_di_lavoro));
                    fragment = (DocumentListFragment)FragmentManager.FindFragmentByTag(tab.ToString());
                    if (fragment == null)
                    {
                        fragment = DocumentListFragment.CreateInstance(SectionType.ADL);
                    }
                    break;

                case Tab.SEARCH:
                    SetPageTitle(GetString(Resource.String.search));
                    fragment = (SearchFragment)FragmentManager.FindFragmentByTag(tab.ToString());
                    if (fragment == null)
                    {
                        fragment = SearchFragment.CreateInstance();
                    }
                    break;
            }

            if (fragment != null)
            {
                FragmentTransaction ft = FragmentManager.BeginTransaction();
                ft.Replace(Resource.Id.frameLayout_container, fragment, tab.ToString());
                ft.Commit();
            }

            UpdateMenu(tab);

            drawerLayout?.CloseDrawer(leftDrawer);
        }

        public void OnLogoutOk()
        {
            Intent intent = new Intent(this, typeof(LoginActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
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

        internal class MyActionBarDrawerToggle : ActionBarDrawerToggle
        {
            HomeActivity owner;

            public MyActionBarDrawerToggle(HomeActivity activity, DrawerLayout layout, Toolbar toolbar, int openRes, int closeRes)
              : base(activity, layout, toolbar, openRes, closeRes)
            {
                owner = activity;
            }

            public override void OnDrawerClosed(View drawerView)
            {
            }

            public override void OnDrawerOpened(View drawerView)
            {
            }
        }

        public void startCommonActionActivity(int actionTypeId, string serializedDocument, SectionType documentInfo)
        {
            customBottomSheetDialog.Hide();
            Intent intent = new Intent(this, typeof(CommonActionActivity));
            intent.PutExtra(CommonActionActivity.KEY_CONFIGURE_ACTIVITY_FOR, actionTypeId);
            intent.PutExtra(CommonActionActivity.KEY_DOCUMENT, serializedDocument);
            intent.PutExtra(CommonActionActivity.KEY_DOCUMENT_INFO, (int)documentInfo);
            StartActivityForResult(intent, (int)ActivityRequestCodes.DocumentAction);
        }

        public void startAssignActivity(int actionTypeId, string serializedDocument)
        {
            customBottomSheetDialog.Hide();
            Intent intent = new Intent(this, typeof(AssignActivity));
            intent.PutExtra(AssignActivity.KEY_CONFIGURE_ACTIVITY_FOR, actionTypeId);
            intent.PutExtra(AssignActivity.KEY_DOCUMENT, serializedDocument);
            StartActivityForResult(intent, (int)ActivityRequestCodes.Assign);
        }

        public Intent StartSignActivity(AbstractDocumentListItem model, SectionType documentInfo)
        {
            string serializedDocument = JsonConvert.SerializeObject(model);
            Intent intent = new Intent(this, typeof(SignActivity));
            intent.PutExtra(SignActivity.KEY_CONFIGURE_ACTIVITY_FOR, ActionType.SIGN.id);
            intent.PutExtra(SignActivity.KEY_DOCUMENT, serializedDocument);
            intent.PutExtra(SignActivity.KEY_DOCUMENT_INFO, (int)documentInfo);
            return intent;
        }

        public Intent StartSignActivityWithOtp(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned, string signType, SignFlowType signFlowType)
        {
            string serializedListDocument = JsonConvert.SerializeObject(listStateDaFirmareOTP);
            Intent intent = new Intent(this, typeof(SignActivity));
            intent.PutExtra(SignActivity.KEY_SIGN_TYPE, signType);
            intent.PutExtra(SignActivity.KEY_SIGN_FLOW, signFlowType.ToString());
            intent.PutExtra(SignActivity.KEY_CONFIGURE_ACTIVITY_FOR, ActionType.VIEW_OTP.id);
            intent.PutExtra(SignActivity.KEY_LIST_DOCUMENT_OTP_SIGN, serializedListDocument);
            intent.PutExtra(SignActivity.TOTAL_RECORD_COUNT_SIGNED, TotalRecordCountSigned.ToString());
            return intent;
        }

        public void ShowBottomSheetDialog(List<DialogItem> itemList)
        {
            customBottomSheetDialog = new CustomBottomSheetDialog(this, itemList);
            customBottomSheetDialog.Show();
        }

        public void OnCustomBottomNavigationItemSelected(int resourceId, Tab tab)
        {
            if (tab != homePresenter.GetCurrentTab())
            {
                homePresenter.UpdateCurrentTab(tab);
                ShowNewFragment(tab);
            }
        }

        public void DoShowMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            if (item.Equals(addMenuItem))
            {
                Intent intent = new Intent(this, typeof(NewDelegationActivity));
                StartActivityForResult(intent, (int)ActivityRequestCodes.NewDelegation);
                return true;
            }
            return false;
        }

        private bool IsToolbarMenuSet()
        {
            return filterMenuItem != null &&
                   addMenuItem != null && showDialogActionsDocuments != null;
        }

        public void UpdateFilterView(FilterModel filterModel)
        {
            homePresenter.UpdateFilter(filterModel);
            filterView.ShowFilter(filterModel);
        }

        #region CustomFilterView.ICustomFilterViewListener

        public void OnRemove()
        {
            fragment.SetFilter(null);
        }

        #endregion

        public void OpenShareView(string linkToShare)
        {
            Intent sendIntent = new Intent();
            sendIntent.SetAction(Intent.ActionSend);
            sendIntent.PutExtra(Intent.ExtraText, linkToShare);
            sendIntent.SetType("text/plain");
            StartActivity(sendIntent);
        }

        private void ChoosePhotoFromGallery()
        {
            Intent intent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
            StartActivityForResult(intent, (int)ActivityRequestCodes.NewImageFromGallery);
        }

        private void TakePhotoFromCamera()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, (int)ActivityRequestCodes.NewImageFromCamera);
        }

        private void DeletePhoto()
        {
            profileImageManager.SaveImage(null);
            UpdateImageProfile(null);
        }

        private void OnSelectFromGalleryResult(Intent data)
        {
            if (data != null)
            {
                SaveImageAndLoad(MediaStore.Images.Media.GetBitmap(this.ContentResolver, data.Data));
            }
        }

        private void OnCaptureImageResult(Intent data)
        {
            if (data != null)
            {
                SaveImageAndLoad((Bitmap)data.Extras.Get("data"));
            }
        }

        private async void SaveImageAndLoad(Bitmap bitmap)
        {
            Uri imageUri = await profileImageManager.SaveImage(bitmap);
            UpdateImageProfile(imageUri);
        }

        private void UpdateImageProfile(Uri imageUri)
        {
            int radius = (int)Resources.GetDimension(Resource.Dimension.profile_photo_radius);

            var requestOptions = new RequestOptions()
                .Apply(new RequestOptions().Transform(new RoundedCorners(radius)))
                .Apply(RequestOptions.SkipMemoryCacheOf(true))
                .Apply(RequestOptions.DiskCacheStrategyOf(DiskCacheStrategy.None));

            if (imageUri != null)
            {
                Glide.With(this)
                  .Load(imageUri)
                  .Apply(requestOptions)
                  .Into(userImage);
            }
            else
            {

                Glide.With(this)
                  .Load(user.URLImageUser)
                  .Apply(requestOptions)
                  .Into(userImage);
            }
        }

        /*
                private Bitmap DecodeBitmap(string bitmap)
                {
                    return  string.IsNullOrEmpty(bitmap)? null : BitmapFactory.DecodeByteArray(Convert.FromBase64String(bitmap), 0, Convert.FromBase64String(bitmap).Length);
                }       
                */
    }
}