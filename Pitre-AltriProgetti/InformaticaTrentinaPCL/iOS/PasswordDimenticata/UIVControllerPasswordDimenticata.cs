﻿// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.Generic;
using Foundation;
using InformaticaTrentinaPCL.ChangePassword;
using InformaticaTrentinaPCL.ChangePassword.MVP;
using InformaticaTrentinaPCL.iOS.ChooseInstance.Storyboard;
using InformaticaTrentinaPCL.iOS.Helper;
using InformaticaTrentinaPCL.iOS.Login.Session;
using InformaticaTrentinaPCL.iOS.Login.Storyboard;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.MVP;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.PasswordDimenticata
{
    public enum TypeCellPassword
    {
        OLD,
        NEW,
        CONFIRM,
        ADMIN
    }


    public class ClassCellPasswordDimenticata
    {
        public ClassCellPasswordDimenticata(string title,string image, UIVControllerPasswordDimenticata controller, RecuperaPasswordPresenter presenter,TypeCellPassword typeCellPassword)
        {
            this.title = title;
            this.image = image;
            this.controller = controller;
            this.presenterPassword = presenter;
            this.typeCell = typeCellPassword;
        }

        public string title = "";
		public string image = "";
        public UIVControllerPasswordDimenticata controller;
        public RecuperaPasswordPresenter presenterPassword;
        public TypeCellPassword typeCell;
	}

    public partial class UIVControllerPasswordDimenticata : UIViewController, IUITableViewDelegate, IUITableViewDataSource,ILoginView
    {
        private bool enabled = false;
        private List<ClassCellPasswordDimenticata> titleTab = new List<ClassCellPasswordDimenticata>();
        public LoginAdministrationState state;
        ClassCellPasswordDimenticata ammi;
        ClassCellPasswordDimenticata passwordNew;
        RecuperaPasswordPresenter presenterPassword;
        public string username;
        private WeakReference<UIViewControllerLogin> controllerReference;
        public UIViewControllerLogin LoginController
        {
            get
            {
                UIViewControllerLogin _controller = null;
                controllerReference.TryGetTarget(out _controller);
                return _controller;
            }
            set { controllerReference = new WeakReference<UIViewControllerLogin>(value); }
        }
        //public Action<UserInfo> OnExpiredOk;
        Keyboard keyboard;

		public UIVControllerPasswordDimenticata(IntPtr handle) : base (handle)
		{
		}

        public static UIVControllerPasswordDimenticata CreateChooseInstanceViewController(UIViewControllerLogin loginController)
        {
        
            var storyboard = UIStoryboard.FromName("Storyboard_passworddimeticata", null);
            var vc = storyboard.InstantiateViewController("UIVControllerPasswordDimenticata") as UIVControllerPasswordDimenticata;
            return vc;
        }

        public void ConfigureTable()
		{
            var UserData = NSUserDefaults.StandardUserDefaults;
            username = UserData.StringForKey("info_USER_ID");
            presenterPassword = new RecuperaPasswordPresenter(this,IosNativeFactory.Instance(),username);
			tableView.Delegate = this;
			tableView.DataSource = this;
            tableView.EstimatedRowHeight = 100;
            tableView.RowHeight = UITableView.AutomaticDimension;
            var preferences = new Preferences();

            ammi = new ClassCellPasswordDimenticata(Utility.LanguageConvert(""), "",this, presenterPassword,TypeCellPassword.ADMIN);
            passwordNew = new ClassCellPasswordDimenticata(Utility.LanguageConvert("password_new"),"", this,presenterPassword,TypeCellPassword.NEW);
            titleTab.Clear();
            titleTab.Add(new ClassCellPasswordDimenticata("Password Temporanea", "", this, presenterPassword,TypeCellPassword.OLD));
            titleTab.Add(passwordNew);
            titleTab.Add(new ClassCellPasswordDimenticata(Utility.LanguageConvert("password_ripet"), "", this, presenterPassword,TypeCellPassword.CONFIRM));
			titleTab.Add(ammi);

            if (state == LoginAdministrationState.DEFAULT)
                titleTab.Remove(ammi);

            ammi.title = (state == LoginAdministrationState.UNSELECTED) ? Utility.LanguageConvert("choose_amm") : "";
        }

        public void ConfigureKeyboardNotification()
        {
            if (Utility.IsTablet()) return;

            keyboard = new Keyboard();

            keyboard.KeyboardListenerWillDidShow( (obj) => {

                this.View.Frame = new CoreGraphics.CGRect(0,-100, this.View.Frame.Width, this.View.Frame.Height);

            });

            keyboard.KeyboardListenerWillDidHide((obj) => {

                this.View.Frame = new CoreGraphics.CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);

            });
        }

        private void ConfigurationNavigation()
        {
            ActionNavigationType[] buttons = { ActionNavigationType.ActionBack };
            Navigation.NavigationCustom("P.I.Tre", this, buttons, null, (obj) =>
            {
                if (obj == ActionNavigationType.ActionTabletClose)
                {
                    DismissViewController(true, null);
                }
                else if (obj == ActionNavigationType.ActionTabletBack)
                {
                    DismissViewController(true, null);
                }
                else
                {
                    NavigationController.PopViewController(true);
                }
            }, true, false);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ConfigurationNavigation();
            ConfigureTable();
            buttonDone.Enabled = false;
            buttonDone.SetTitle(Utility.LanguageConvert("button_done").ToUpper(), UIControlState.Normal);
			Utility.ButtonStyleDefault(buttonDone,
									  enabled ? Colors.COLOR_BUTTON_DEFAULT : Colors.COLOR_BUTTON_DISABLED_DEFAULT,
									  UIColor.LightGray,
									  UIColor.White);

            var UserData = NSUserDefaults.StandardUserDefaults;

            labelTitle.Text = "Reimposta password";//Utility.LanguageConvert("password_expired");
            Font.SetCustomStyleFont(labelTitle, Font.MODALE_TITLE, UITextAlignment.Center);

            labelSubtitle.Text = "Password temporanea inviata all'email "; //Utility.LanguageConvert("password_desc");
            Font.SetCustomStyleFont(labelSubtitle, Font.SENDER,UITextAlignment.Center);

            labeltitleUsername.Text = UserData.StringForKey("info_email");// Utility.LanguageConvert("username_Login").ToUpper();
            Font.SetCustomStyleFont(labeltitleUsername, Font.SENDER, UITextAlignment.Center);

           // UserData.StringForKey("info_email");
            labelName.Text = "In caso di mancata ricezione contattare l'assitenza"; // username;
			Font.SetCustomStyleFont(labelName, Font.SENDER, UITextAlignment.Center );

		}

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ConfigureKeyboardNotification();

        }

        public void OnServerChanged(string message)
        {
            throw new NotImplementedException();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            keyboard.RemoveListener();
        }

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			this.marginLeft.Constant = StyleTablet.MarginRightAndLeftForViewController();
			this.marginRight.Constant = StyleTablet.MarginRightAndLeftForViewController();
			this.marginDown.Constant = StyleTablet.MarginBottomAndTopForViewController();
            this.marginUp.Constant = StyleTablet.TOP_FROM_NAVIGATION;
		}

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (titleTab.Count == 4 && indexPath.Row == titleTab.Count - 1)
            {
                PerformSegue("UIViewControllerChooseAdministrator", null);
            }
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return titleTab.Count;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
           
            UITVCellPasswordDimenticata cell = (UITVCellPasswordDimenticata)tableView.DequeueReusableCell("UITVCellPasswordDimenticata");
            cell.Update(titleTab[indexPath.Row]);
            return cell;

        }

		partial void ActionButtonClose(Foundation.NSObject sender)
		{
            presenterPassword.LoginAsync(false);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

          
       }

        public void Callback(AmministrazioneModel model)
        {
            if (model == null)
            {
                state = LoginAdministrationState.UNSELECTED;
                ammi.title = Utility.LanguageConvert("choose_amm");
            }
            else
            {
                state = LoginAdministrationState.SELECTED;
                ammi.title = model.descrizione;
                presenterPassword.UpdateAdministration(model, state);
            }
           
            tableView.ReloadData();
        }

        public override bool ShouldPerformSegue(string segueIdentifier, NSObject sender)
        {
            return base.ShouldPerformSegue(segueIdentifier, sender);
        }

        public void EnableButton(bool enabled)
        {
            buttonDone.Enabled = enabled;
            Utility.ButtonStyleDefault(buttonDone,
                                       enabled ? Colors.COLOR_BUTTON_DEFAULT : Colors.COLOR_BUTTON_DISABLED_DEFAULT,
                                   UIColor.LightGray,
                                   UIColor.White);
        }

        public void ChangePasswordOk(UserInfo userLogged)
        {
            if (userLogged != null)
            {
                SaveCredentials();
            }
        }

        /// <summary>
        /// Saves the credenzial all interno dello store
        /// </summary>
        public void SaveCredentials()
        {
            var preferences = new Preferences();
            var username = preferences.GetString(Constant.USERNAME_KEY, null);
            preferences.Set(Constant.PASSWORD_KEY, passwordNew.title);
        }

        public void ShowListAdministration()
        {
            state = LoginAdministrationState.UNSELECTED;
            ConfigureTable();
            tableView.ReloadData();
            this.PerformSegue("UIViewControllerChooseAdministrator", null);
        }

        public void ShowError(string e, bool isLight)
        {
            ShowErrorHelper.Show(this,isLight,e);
        }

        public void OnUpdateLoader(bool isShow)
        {
            Utility.Loading(this.View, isShow);
        }

        public void OnLoginOK(UserInfo user)
        {
            

            if (user != null)
            {
                SaveCredentials();
                AppDelegate.StarMenuController();
            }

        }

        public void ShowChangePassword()
        {
            Console.WriteLine("Error : ShowChangePassword");
        }

        public void ShowUpdatePopup(string url)
        {
         
        }
    }
}