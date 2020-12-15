using System;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Helper
{

    public enum ActionNavigationType
    {
        ActionMenu,
        ActionPlus,
        ActionFilter,
        ActionBack,
        ActionTabletBack,
        ActionTabletClose,
        ActionSignDocument
    }

    public class Navigation
    {
        public Navigation()
        {
        }

        private static string GetTitleFormActionType(ActionNavigationType item)
        {
            if (item == ActionNavigationType.ActionMenu)
            {
                return "menu_navigation";
            }
            else if (item == ActionNavigationType.ActionBack)
            {
                return "leftback";
            }
            else if (item == ActionNavigationType.ActionPlus)
            {
                return "plus";
            }
            else if (item == ActionNavigationType.ActionFilter)
            {
                return "filters";
            }
            else if (item == ActionNavigationType.ActionSignDocument)
            {
                return "ic_more";
            }
         
            return "";
        }

        /// <summary>
        /// Creates the button .
        /// </summary>
        /// <returns>The button.</returns>
        /// <param name="nameImage">Name image.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="color">Color.</param>
        /// <param name="actiontype">Actiontype.</param>
        private static UIBarButtonItem CreateButton(string nameImage, Action<ActionNavigationType> callback, UIColor color, ActionNavigationType actiontype)
        {
            UIBarButtonItem buttonItem = new UIBarButtonItem(UIImage.FromBundle(nameImage), UIBarButtonItemStyle.Plain, (sender, args) =>
            {
                if (callback != null)
                    callback(actiontype);
            });

            buttonItem.TintColor = color;

            return buttonItem;
        }

        /// <summary>
        /// Navigations the custom.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="controller">Controller.</param>
        /// <param name="listLeft">List left.</param>
        /// <param name="listRight">List right.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="other">If set to <c>true</c> other.</param> 
        public static void NavigationCustom(string title, UIViewController controller, ActionNavigationType[] listLeft, ActionNavigationType[] listRight, Action<ActionNavigationType> callback, bool other = true , bool upper =true)
        {
            if (controller.NavigationItem != null && controller.NavigationController != null)
            {
                SetStyleNavigation(title, controller.NavigationItem, controller.NavigationController);
                if (listLeft != null)
                {
                    UIBarButtonItem[] items = new UIBarButtonItem[listLeft.Length];
                    int i = 0;
                    foreach (var item in listLeft)
                    {
                        items[i] = CreateButton(GetTitleFormActionType(item), callback, UIColor.White, item);
                        i++;
                    }
                    controller.NavigationItem.SetLeftBarButtonItems(items, true);
                }

                if (listRight != null)
                {
                    UIBarButtonItem[] items = new UIBarButtonItem[listRight.Length];
                    int i = 0;
                    foreach (var item in listRight)
                    {
                        items[i] = CreateButton(GetTitleFormActionType(item), callback, UIColor.White, item);
                        i++;
                    }
                    controller.NavigationItem.SetRightBarButtonItems(items, true);
                }
            }
            else
                StyleTablet.NavigationForTablet(title, controller, callback, other , upper);
            
         

        }

       
        private static void SetStyleNavigation(string title, UINavigationItem NavigationItem, UINavigationController NavigationController)
        {
            NavigationItem.Title = Utility.LanguageConvert(title);
            NavigationController.NavigationBar.BarTintColor = Colors.COLOR_NAVIGATION;


            NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
            {
                ForegroundColor = UIColor.White ,
              //  Font = UIFont.FromName(Font.PAGE_TITLE.family, 10 )  

            };

        

            NavigationItem.RightBarButtonItem = null;
            NavigationItem.LeftBarButtonItem = null;
        }
    }
}