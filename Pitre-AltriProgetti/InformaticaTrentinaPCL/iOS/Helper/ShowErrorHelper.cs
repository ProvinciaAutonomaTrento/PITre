using System;
using InformaticaTrentinaPCL.Utils;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class ShowErrorHelper
    {
        public ShowErrorHelper()
        {

        }

        public static void Show(UIViewController viewController, bool isLight, string errorMessage, string titleAlert = null, Action action = null)
        {
            string message = string.IsNullOrEmpty(errorMessage) ? LocalizedString.GENERIC_ERROR.Get() : errorMessage;
            if (isLight)
            {
                Alert.AlertToast(message, viewController);
            }
            else
            {
                Alert.BaseAlert(null != titleAlert ? titleAlert : LocalizedString.TITLE_ALERT.Get(), message, null, viewController, action);
            }
        }

        public static UIAlertController CreateChoiceAlert(UIViewController viewController, string errorMessage, string titleAlert, Action cancelAction, Action doneAction)
        {
            string message = string.IsNullOrEmpty(errorMessage) ? LocalizedString.GENERIC_ERROR.Get() : errorMessage;

            return Alert.ChoiceAlert(null != titleAlert ? titleAlert : LocalizedString.TITLE_ALERT.Get(), message, null, viewController, cancelAction, doneAction);

        }
    }
}
