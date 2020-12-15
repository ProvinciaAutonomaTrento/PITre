using System;
using System.Collections.Generic;
using System.IO;
using CoreGraphics;
using Foundation;
using InformaticaTrentinaPCL.iOS.EmbedComponent;
using InformaticaTrentinaPCL.iOS.PasswordExpired;
using InformaticaTrentinaPCL.Login;
using UIKit;
using static System.Net.Mime.MediaTypeNames;
using static InformaticaTrentinaPCL.Interfaces.AbstractRecipient;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class Utility
    {
        static Dictionary<UIView, LoadingOverlay> dictionary = new Dictionary<UIView, LoadingOverlay>();

        public Utility()
        {
        }

        public static string LanguageConvert(string key)
        {
            return NSBundle.MainBundle.LocalizedString(key, "");
        }

        /// <summary>
        /// Il metodo Aggiunge la pull refesh in alto ad ogni tabella che lo richiama. In ingresso bisogna passare il metodo in ascolto e una tabella dove aggiungere loggetto.
        /// Al metodo in ascolto dopo il refresh si deve chiamare  refresh.EndRefreshing(); per chiudere lanimazione-
        /// </summary>
        /// <returns> oggetto UIRefreshControl : refresh.EndRefreshing() </returns>
        /// <param name="HandleValueChanged">Handle value changed.</param>
        /// <param name="tableView">Table view.</param>
        public static UIRefreshControl AddRefreshToTable(EventHandler HandleValueChanged, UITableView tableView)
        {
            UIRefreshControl refresh = new UIRefreshControl();
            refresh.AddTarget(HandleValueChanged, UIControlEvent.ValueChanged);
            tableView.AddSubview(refresh);
            return refresh;
        }

        public static void ButtonStyleDefault(UIButton button, UIColor color, UIColor borderColor, UIColor textColor)
        {
            button.Layer.BorderColor = borderColor.CGColor;
            button.Layer.BackgroundColor = color.CGColor;
            button.SetTitleColor(textColor, UIControlState.Normal);
        }

        public static void Loading(UIView view, bool shows, Action clickHandler = null)
        {
            var bounds = UIScreen.MainScreen.Bounds;
            if (shows && !dictionary.ContainsKey(view))
            {
                LoadingOverlay loadingOverlay = new LoadingOverlay();
                view.Add(loadingOverlay.View);
                dictionary.Add(view, loadingOverlay);
                if (null != clickHandler)
                {
                    view.AddGestureRecognizer(TapGestureRecognizer(clickHandler));
                }
                Console.WriteLine("Loading presenti :" + dictionary.Count);
            }
            else if (shows && dictionary.ContainsKey(view))
            {

            }
            else if (!shows && dictionary.ContainsKey(view))
            {
                LoadingOverlay loadingBusy = dictionary[view];
                loadingBusy.Hide();
                if (null != clickHandler)
                {
                    RemoveGestureOn(view);
                }
                dictionary.Remove(view);
                loadingBusy.View.RemoveFromSuperview();
                loadingBusy = null;
            }
            else
            {
                Console.WriteLine("Errore loading... caso non previsto");
                Console.WriteLine("dizionario " + dictionary.Count);
            }
            Console.WriteLine("Loading presenti :" + dictionary.Count);
        }

        private static UITapGestureRecognizer TapGestureRecognizer(Action action){
            
            var tapGestureRecognizer = new UITapGestureRecognizer(() => {
                if(null != action){
                    action();
                }
            });

            return tapGestureRecognizer;
        }

        private static void RemoveGestureOn(UIView view){
            UIGestureRecognizer[] gestures = view.GestureRecognizers;
            foreach (UIGestureRecognizer gest in gestures)
            {
                view.RemoveGestureRecognizer(gest);
            }
        }

        public static NSDateComponents splitDateForGGMMAAAA(NSDate date)
        {
            NSDateComponents component = NSCalendar.CurrentCalendar.Components(NSCalendarUnit.Day | NSCalendarUnit.Month | NSCalendarUnit.Year, date);
            return component;

        }

        public static NSDateComponents splitDateForGGMMAAAAHHmm(NSDate date)
        {
            NSDateComponents component = NSCalendar.CurrentCalendar.Components(NSCalendarUnit.Day | NSCalendarUnit.Month | NSCalendarUnit.Year | NSCalendarUnit.Hour | NSCalendarUnit.Minute, date);
            return component;

        }

        public static UIViewController GetControllerFromStoryboard(string nameStoryboard, string idController)
        {
            UIStoryboard storyboard = UIStoryboard.FromName(nameStoryboard, NSBundle.MainBundle);
            UIViewController controller = (UIViewController)storyboard.InstantiateViewController(idController);
            return controller;
        }

        public static void CreateCustomTabBar(UITabBarController TabBar, int numberTab)
        {
            nfloat tabHeight = TabBar.TabBar.Frame.Height;
            nfloat selectedBorderHeight = 10f;
            UIView bgSelectedView = new UIView(new CGRect(new CGPoint(0, 0), new CGSize(TabBar.View.Frame.Width / numberTab, tabHeight)));
            UIImageView borderSelected = new UIImageView(new CGRect(new CGPoint(0, 0), new CGSize(TabBar.View.Frame.Width / numberTab, tabHeight)));
            borderSelected.BackgroundColor = UIColor.Clear;
            bgSelectedView.AddSubview(borderSelected);
            UIView viewBorder = new UIView(new CGRect(new CGPoint(0, tabHeight - selectedBorderHeight), new CGSize(TabBar.View.Frame.Width / numberTab, selectedBorderHeight)));
            viewBorder.BackgroundColor = Colors.COLOR_NAVIGATION;
            bgSelectedView.Add(viewBorder);
            TabBar.TabBar.SelectionIndicatorImage = Utility.ViewForImage(bgSelectedView);
        }

        public static UIVControllerPasswordExpired ControllerChangePassword(UIViewController controller, string username,Action<UserInfo> Callback)
        {
            UIVControllerPasswordExpired controllerPasswordExpired = (UIVControllerPasswordExpired)Utility.GetControllerFromStoryboard("Storyboard_password", "UIVControllerPasswordExpired");
            controllerPasswordExpired.username = username;
            controllerPasswordExpired.OnExpiredOk = Callback;
            controller.PresentViewController(controllerPasswordExpired, true, null);
        
            return controllerPasswordExpired;
        }


        public static UIImage ViewForImage(UIView view)
        {
            UIGraphics.BeginImageContext(view.Frame.Size);
            try
            {
                using (var context = UIGraphics.GetCurrentContext())
                {
                    view.Layer.RenderInContext(context);
                    return UIGraphics.GetImageFromCurrentImageContext();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return default(UIImage);
            }
            finally
            {
                UIGraphics.EndImageContext();

            }
        }


        /// <summary>
        /// Ises the tablet.
        /// </summary>
        /// <returns><c>true</c>, if tablet was ised, <c>false</c> otherwise.</returns>
        public static bool IsTablet()
        {
            var idiom = UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom;
            return UIUserInterfaceIdiom.Pad == idiom;
        }

        /// <summary>
        /// Gets the width of the original screen.
        /// </summary>
        /// <returns>The original screen width.</returns>
        public static nfloat getOriginalScreenWidth()
        {
            return 375;  // width iphone 6
        }

        /// <summary>
        /// Gets the height of the original screen.
        /// </summary>
        /// <returns>The original screen height.</returns>
		public static nfloat getOriginalScreenHeight()
        {
            return 667;  // width iphone 6
        }

        public static nfloat getScreenWidth()
        {
            var idiom = UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom;

            if (UIUserInterfaceIdiom.Pad == idiom)
                return getOriginalScreenWidth();

            return UIScreen.MainScreen.Bounds.Width;
        }

        public static nfloat getScreenHeight()
        {
            var idiom = UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom;

            if (UIUserInterfaceIdiom.Pad == idiom)
                return getOriginalScreenHeight();

            return UIScreen.MainScreen.Bounds.Height;
        }

        /// <summary>
        /// Actions the document list.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="title">Title.</param>
        /// <param name="listItems">List items.</param>
        public static void ActionDocumentList(UIView view, Action<int> callback, string title, string[] listItems)
        {
            
            UIActionSheet actionSheet;
            actionSheet = new UIActionSheet(title);
            actionSheet.AddButton(Utility.LanguageConvert("cancel"));
            foreach (var v in listItems)
                actionSheet.AddButton(v);
            actionSheet.DestructiveButtonIndex = 0;
            //actionSheet.CancelButtonIndex = 1;
            //actionSheet.FirstOtherButtonIndex = 2;
            actionSheet.Clicked += delegate (object a, UIButtonEventArgs b)
            {
                Console.WriteLine("Button " + b.ButtonIndex.ToString() + " clicked");
                if (callback != null && ((int)b.ButtonIndex != 0))
                    callback((int)b.ButtonIndex - 1);
            };
            actionSheet.ShowInView(view);
        }

        /// <summary>
        /// Shows the controller administrator.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="Callback">Callback.</param>
        /// <param name="controller">Controller.</param>
        public static void ShowAdministrator(string username,Action<AmministrazioneModel> Callback,UIViewController controller)
        {
            UIViewControllerChooseAdministrator administrator = (UIViewControllerChooseAdministrator)Utility.GetControllerFromStoryboard("Storyboard_password","UIViewControllerChooseAdministrator");
            administrator.username = username;
            administrator.Callback = Callback;
            controller.PresentViewController(administrator, true, null);
        }

        public static String GetImageDocument(RecipientType type)
        {
            String nameImage = "imageUserDefault";

            if (type  == RecipientType.USER)
            {
                nameImage = "imageUserDefault";
            }
            else if ( type == RecipientType.ROLE)
            {
                nameImage = "role";
            }
            else if (type == RecipientType.OFFICE)
            {
                nameImage = "office";
            }
                
            return nameImage;
        }

        public static string CreateBase64(NSData imageData)
        {
            if (imageData != null)
            {
                string encodedImage = imageData.GetBase64EncodedData(NSDataBase64EncodingOptions.None).ToString();
                return encodedImage;
            }
            return "";
        }

        public static UIImage ImageFromBase64(string base64Image)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(base64Image);
            NSData data = NSData.FromArray(encodedDataAsBytes);
            var image = UIImage.LoadFromData(data);
            return image;
        }

    }
}