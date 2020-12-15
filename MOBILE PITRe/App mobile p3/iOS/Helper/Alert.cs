using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.iOS.PopUp.Storyboard;
using InformaticaTrentinaPCL.Utils;
using MBProgressHUD;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class Alert
    {
        public Alert()
        {
        }

        /// <summary>
        /// Pops up state document.
        /// </summary>
        /// <param name="stateDocument">State document.</param>
        /// <param name="context">Context.</param>
        /// <param name="actionType">Action type.</param>
        /// <param name="extra">Extra.</param>
        public static void PopUpStateDocument(SectionType stateDocument, UIViewController context, ActionType actionType,Dictionary<string, string> extra = null)
        {
            UIViewControllerPopUpDocumentState controller = (UIViewControllerPopUpDocumentState)UIStoryboard.FromName(UIViewControllerPopUpDocumentState.NAME_STORYBOARD, null)
            .InstantiateViewController(UIViewControllerPopUpDocumentState.NAME_CONTROLLER);
            controller.stateDocument = stateDocument;
            controller.actionType = actionType;
            if (extra != null)
            {
                controller.extra = extra; 
            }
            context.PresentViewController(controller, true, null);
        }

        public static void BaseAlert(string title, string descr, string[] buttons, UIViewController controller, Action calback)
        {

            var alert = UIAlertController.Create(title, descr, UIAlertControllerStyle.Alert);

            //alert.AddAction(UIAlertAction.Create(Utility.LanguageConvert("no"), UIAlertActionStyle.Cancel, null));

            alert.AddAction(UIAlertAction.Create(Utility.LanguageConvert("OK"), UIAlertActionStyle.Default, action =>
            {
                if (calback != null)
                    calback();
            }));


            if (controller != null)
                controller.PresentViewController(alert, animated: true, completionHandler: null);

        }

        public static void AlertToast(string descr, UIViewController controller)
        {
           var hud = new MTMBProgressHUD(controller.View)
           {
               //LabelText = descr,
               RemoveFromSuperViewOnHide = true,
               Mode = MBProgressHUDMode.Text,
               DetailsLabelText = descr,
                             
            };

            controller.View.AddSubview(hud);

            hud.Show(animated: true);
            hud.Hide(animated: true, delay: 1); 
        }

        public static UIAlertController ChoiceAlert(string title, string descr, string[] buttons, UIViewController controller, Action callbackCancel, Action calbackDone){

            var alert = UIAlertController.Create(title, descr, UIAlertControllerStyle.Alert);

            //alert.AddAction(UIAlertAction.Create(Utility.LanguageConvert("no"), UIAlertActionStyle.Cancel, null));

            alert.AddAction(UIAlertAction.Create(LocalizedString.CANCEL_ACTION.Get(), UIAlertActionStyle.Default, action =>
            {
                if (callbackCancel != null)
                    callbackCancel();
            }));

            alert.AddAction(UIAlertAction.Create(LocalizedString.DONE_ACTION.Get(), UIAlertActionStyle.Default, action =>
            {
                if (calbackDone != null)
                    calbackDone();
            }));

            return alert;
           
        }
    }
}
