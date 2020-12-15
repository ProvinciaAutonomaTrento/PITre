using System;
using Android.App;
using Android.Content;
using Android.Widget;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Droid.Utils

{
    public class ShowErrorHelper
    {
        /// <summary>
        /// Se isLight == true, mostra una Toast, altrimenti mostra un'AlertDialog.
        /// NOTA: Se isLight == false, allora il Context deve essere un'Activity (o sottoclassi) o un Service (o sottoclassi)
        /// </summary>
        /// <param name="context">Se isLight == false, allora il Context deve essere un'Activity (o sottoclassi) 
        ///                       o un Service (o sottoclassi)</param>
        /// <param name="error">Il messaggio di errore da utilizzare</param>
        /// <param name="isLight">true, mostra un Toast, false un'AlertDialog</param>
        /// <param name="action">L'azione da eseguire al click su Ok dell'AlertDialog, può essere null</param>
        public static void Show(Context context, string error, bool isLight, Action action = null)
        {
            var message = string.IsNullOrEmpty(error) ? LocalizedString.GENERIC_ERROR.Get() : error;
            if (isLight)
            {
                Toast.MakeText(context, message, ToastLength.Short).Show();
            }
            else
            {           
                new AlertDialog.Builder(context).SetTitle(LocalizedString.TITLE_ALERT.Get()).SetMessage(message)
                    .SetPositiveButton(LocalizedString.OK_BUTTON.Get(), (senderAlert, args) =>
                    {
                        action?.Invoke();
                    }).Show();
            }
        }

        public static void CreateChoiceAlert(Context context, string message, string titleAlert, Action continueAction=null, Action deleteAction=null)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(context);
            alert.SetTitle(!string.IsNullOrEmpty(titleAlert) ? titleAlert : LocalizedString.TITLE_ALERT.Get());
            alert.SetMessage(message);
            alert.SetPositiveButton(LocalizedString.CONTINUE_ACTION.Get(), (senderAlert, args) => {
                continueAction?.Invoke();
            });

            alert.SetNegativeButton(LocalizedString.DELETE_ACTION.Get(), (senderAlert, args) => {
                deleteAction?.Invoke();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }
    }
}