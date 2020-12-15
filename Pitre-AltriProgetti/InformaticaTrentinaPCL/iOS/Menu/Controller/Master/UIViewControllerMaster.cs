﻿// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.iOS.Helper;
using InformaticaTrentinaPCL.iOS.Login.Session;
using InformaticaTrentinaPCL.iOS.Login.Storyboard;
using InformaticaTrentinaPCL.iOS.TabBar.Root.Storyboard;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;
using Photos;
using SDWebImage;
using UIKit;
using Xamarin.iOS.DGActivityIndicatorViewBinding;

namespace InformaticaTrentinaPCL.iOS.Menu.Storyboard
{
    public partial class UIViewControllerMaster : UIViewController, IUITableViewDataSource, IUITableViewDelegate, IHomeView, IUIImagePickerControllerDelegate
    {
        private int CONST_LOGOUT = 0;
        private int CONST_DIFF = 1;
        private UIImagePickerController imagePicker;
        private UIImage originalImageGallery;
        Preferences preferences = new Preferences();

        public String[] TitlesTab()
        {
            String[] titlesTab = new String[]
          {
            Utility.LanguageConvert(""),
            Utility.LanguageConvert("area_jobs"),
            Utility.LanguageConvert("title_navigation_signature"),
            Utility.LanguageConvert("title_navigation_todo"),
            Utility.LanguageConvert("title_navigation_delegate"),
            Utility.LanguageConvert("title_navigation_search"),
            Utility.LanguageConvert(""),
          };

            return titlesTab;
        }


        string[] imgsMenuTabBar =
        {
            "************",
            "tab1",
            "tab2",
            "tab3",
            "tab4",
            "search",
            "************"
        };


        string[] titlesMenu =
        {
           // Utility.LanguageConvert("Impostazioni"),
            Utility.LanguageConvert("Esci da PiTre")
        };

        string[] imgsMenu = { //"settings",
            "exit"};

        public UIViewControllerMaster(IntPtr handle) : base(handle)
        {
        }

        HomePresenter homePresenter;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            homePresenter = new HomePresenter(this, IosNativeFactory.Instance());
            InitTable();
            SetUserProfile();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            homePresenter.Dispose();
        }

        public void InitTable()
        {
            tableView.Delegate = this;
            tableView.DataSource = this;
            tabBar.Delegate = this;
            tabBar.DataSource = this;
            tabBar.Hidden = !Utility.IsTablet();
            viewTopTabBar.Hidden = !Utility.IsTablet();
            viewDownTabBar.Hidden = !Utility.IsTablet();
            viewTopTabBar.BackgroundColor = Colors.FOOTER_SEPARATOR;
            viewDownTabBar.BackgroundColor = Colors.FOOTER_SEPARATOR;
            ViewStyle();
            RegisterForNotificationsOrientations();
            heightViewTop.Constant = !Utility.IsTablet() ? 50 : 0;
        }

        public void SetUserProfile()
        {
            UserInfo userModel = IosNativeFactory.Instance().GetSessionData().userInfo;
            if (userModel == null) return;

            labelName.Text = "";
            labelOrganization.Text = "";

            //FOR IMAGE ON LOGIN RESPONSE - (UNUSED)
            /*string imageName = "";
            if (!String.IsNullOrEmpty(userModel.URLImageUser))
                imageName = userModel.URLImageUser;

            profileImg.SetImage(
                url: new NSUrl(imageName),
                placeholder: UIImage.FromBundle("imageUserDefault"),
                options: SDWebImageOptions.CacheMemoryOnly,
                completedBlock: (imageDownloaded, error, cacheType, imageUrl) =>
                     {
                          if (error != null)
                              Console.WriteLine(error.Description);
                      }
          );*/

            UIImage savedImage = RetrieveSavedImage();
            profileImg.Image = null != savedImage ? savedImage : UIImage.FromBundle("imageUserDefault");
            profileImg.Layer.CornerRadius = profileImg.Layer.Frame.Height / 2;

            if (!String.IsNullOrEmpty(userModel.descrizione))
                labelName.Text = userModel.descrizione;

            if (!String.IsNullOrEmpty(userModel.ruoli[0].descrizione))
                labelOrganization.Text = userModel.ruoli[0].descrizione;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            UserInfo userModel = IosNativeFactory.Instance().GetSessionData().userInfo;
            // update : in questo caso non è troppo oneroso farlo altrimenti si deve sistemare in modo tale da aggiornare la grafica solo se ce ne bisogno 
            // dovrebbe essere chiamato solo se si è fatto un cambio ruolo dalla schermata change role
            if (!String.IsNullOrEmpty(userModel.ruoli[0].descrizione))
                labelOrganization.Text = userModel.ruoli[0].descrizione;

        }

        partial void ActionButtonChangeRole(Foundation.NSObject sender)
        {
            this.PerformSegue("UIViewControllerChangeRole", null);
        }

        private void ViewStyle()
        {
            this.View.BackgroundColor = (Utility.IsTablet() && UIApplication.SharedApplication.StatusBarOrientation.IsLandscape()) ? Colors.MASTER_LANDSCAPE : Colors.MASTER_PORTRAIT;
            viewTable.BackgroundColor = (Utility.IsTablet() && UIApplication.SharedApplication.StatusBarOrientation.IsLandscape()) ? Colors.MASTER_LANDSCAPE : Colors.MASTER_PORTRAIT;
            viewProfile.BackgroundColor = (Utility.IsTablet() && UIApplication.SharedApplication.StatusBarOrientation.IsLandscape()) ? Colors.MASTER_LANDSCAPE : Colors.MASTER_PORTRAIT;

            if (Utility.IsTablet())
            {
                topName.Constant = UIApplication.SharedApplication.StatusBarOrientation.IsLandscape() ? 20 : 40;
                topDescRole.Constant = UIApplication.SharedApplication.StatusBarOrientation.IsLandscape() ? 20 : 40;
                heightTable.Constant = UIApplication.SharedApplication.StatusBarOrientation.IsLandscape() ? 40 * TitlesTab().Length : 60 * TitlesTab().Length;
                topImage.Constant = UIApplication.SharedApplication.StatusBarOrientation.IsLandscape() ? 40 : 60;
                heightTableDown.Constant = UIApplication.SharedApplication.StatusBarOrientation.IsLandscape() ? 40 * titlesMenu.Length : 60 * titlesMenu.Length;
            }
            else
            {
                topImage.Constant = 40;
                topName.Constant = 10;
                topDescRole.Constant = 10;
                heightTable.Constant = 0;
                heightTableDown.Constant = 50;
                labelOrganization.Lines = 0;
            }
        }

        public void RegisterForNotificationsOrientations()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidChangeStatusBarOrientationNotification, (obj) =>
            {
                ViewStyle();
                tableView.ReloadData();
                tabBar.ReloadData();
            });
        }

        [Export("tableView:heightForRowAtIndexPath:")]
        public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UIApplication.SharedApplication.StatusBarOrientation.IsLandscape() ? 40 : 60;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public virtual void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (tableView == this.tableView && indexPath.Row == CONST_LOGOUT)
                homePresenter.DoLogout();
            else if (tableView == this.tabBar && indexPath.Row != 0 && indexPath.Row != TitlesTab().Length - CONST_DIFF)
            {
                var row = indexPath.Row;
                SelectedTabbar(row);
            }
        }

        private void SelectedTabbar(int row)
        {
            UITabBarControllerRoot.SetItemTabBar(row - CONST_DIFF);
            UITabBarControllerRoot.IsClickTabBar(true);
            tabBar.ReloadData();
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            if (tableView == this.tableView)
                return titlesMenu.Length;
            else if (Utility.IsTablet() && tableView == this.tabBar)
                return TitlesTab().Length;

            return 0;
        }

        public UITableViewCell CreateRow(string title, string nameImg, bool Selected, bool tab)
        {
            UITableViewCellMaster cell = (UITableViewCellMaster)tableView.DequeueReusableCell("UITableViewCellMaster");

            if (cell == null)
                cell = new UITableViewCellMaster();

            cell.Update(title, nameImg, Selected, tab);

            return cell;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (tableView == this.tableView)
                return CreateRow(titlesMenu[indexPath.Row], imgsMenu[indexPath.Row], false, false);

            return CreateRow(TitlesTab()[indexPath.Row], imgsMenuTabBar[indexPath.Row], UITabBarControllerRoot.GetItemSelected() + CONST_DIFF == indexPath.Row ? true : false, true);
        }

        public void OnLogoutOk()
        {
            SelectedTabbar(UITabBarControllerRoot.DEFAULT_OPEN_TAB_PAGE + CONST_DIFF);

            var window = UIApplication.SharedApplication.KeyWindow;

            window.RootViewController = UIStoryboard.FromName(UIViewControllerLogin.NAME_STORYBOARD, null)
.InstantiateViewController(UIViewControllerLogin.NAME_CONTROLLER);

        }

        public void ShowError(string e, bool isLight)
        {
            ShowErrorHelper.Show(this, isLight, e);
        }

        public void OnUpdateLoader(bool isShow)
        {

            Utility.Loading(this.View, isShow);
        }

        public void DoShowMessage(string message)
        {
            UIAlertView alert = new UIAlertView()
            {
                Message = message
            };
            alert.AddButton("OK");
            alert.Show();
        }

        #region USER_IMAGE
        private void SaveImage()
        {
            NSData imageData = originalImageGallery.AsPNG();
            originalImageGallery = UIImage.LoadFromData(imageData);
            string base64Image = Utility.CreateBase64(imageData);
            preferences.Set(NetworkConstants.KEY_USER_IMAGE, base64Image);
        }

        protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            // determine what was selected, video or image
            bool isImage = false;
            switch (e.Info[UIImagePickerController.MediaType].ToString())
            {
                case "public.image":
                    Console.WriteLine("Image selected");
                    isImage = true;
                    break;
                case "public.video":
                    Console.WriteLine("Video selected");
                    break;
            }

            // get common info (shared between images and video)
            NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
            if (referenceURL != null)
                Console.WriteLine("Url:" + referenceURL.ToString());

            // if it was an image, get the other image info
            if (isImage)
            {
                // get the original image
                originalImageGallery = e.Info[UIImagePickerController.OriginalImage] as UIImage;

                if (originalImageGallery != null)
                {
                    originalImageGallery = UIImageExt.ScaleAndRotateImage(originalImageGallery, originalImageGallery.Orientation, 90);
                    profileImg.Image = originalImageGallery;
                    SaveImage();
                }
            }
            else
            {
                Alert.AlertToast(LocalizedString.ERROR_USING_WRONG_FORMAT.Get(), this);
            }
            imagePicker.DismissViewController(true, null);
        }

        public void OpenMedia(UIImagePickerControllerSourceType type = UIImagePickerControllerSourceType.PhotoLibrary)
        {
            PHPhotoLibrary.RequestAuthorization(status =>
            {
                switch (status)
                {
                    case PHAuthorizationStatus.Authorized:
                        break;
                    case PHAuthorizationStatus.Denied:
                        break;
                    case PHAuthorizationStatus.Restricted:
                        break;
                    default:
                        break;
                }
            });

            imagePicker = new UIImagePickerController();
            imagePicker.SourceType = type;
            imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(type);
            imagePicker.Delegate = this;
            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;

            imagePicker.Canceled += (object sender, EventArgs e) =>
            {
                imagePicker.DismissViewController(true, null);
            };


            BeginInvokeOnMainThread(() =>
            {
                PresentViewController(imagePicker, true, null);
            });

        }

        partial void actionProfileImage(NSObject sender)
        {
            UIImage savedImage = RetrieveSavedImage();
            UIActionSheet actionSheet = new UIActionSheet(LocalizedString.SELECT_USER_IMAGE.Get());
            actionSheet.AddButton(LocalizedString.LABEL_CANCEL.Get());
            actionSheet.AddButton(LocalizedString.LABEL_USE_GALLERY.Get());
   
            if (null != savedImage)
            {
                actionSheet.AddButton(LocalizedString.LABEL_REMOVE_IMAGE.Get());
            }
            actionSheet.CancelButtonIndex = 0;

            actionSheet.Clicked += delegate (object a, UIButtonEventArgs b)
            {
                switch (b.ButtonIndex)
                {
                    case 1:
                        OpenMedia(UIImagePickerControllerSourceType.SavedPhotosAlbum);
                        break;
                    case 2:
                        OpenMedia(UIImagePickerControllerSourceType.Camera);
                        break;
                    case 3:
                        RemoveSavedImage();
                        break;
                    default:
                        break;
                }

            };
            actionSheet.ShowInView(View);
        }

        private UIImage RetrieveSavedImage()
        {
            string base64Image = preferences.GetString(NetworkConstants.KEY_USER_IMAGE);
            if (!string.IsNullOrEmpty(base64Image))
            {
                return Utility.ImageFromBase64(base64Image);
            }
            return null;
        }

        private void RemoveSavedImage()
        {
            preferences.Set(NetworkConstants.KEY_USER_IMAGE, String.Empty);
            profileImg.Image = UIImage.FromBundle("imageUserDefault");
        }
        #endregion
    }
}