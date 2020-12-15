using System;
using Android.App;
using Android.OS;
using Android.Widget;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Droid.Utils;
using System.Collections.Generic;
using Android.Views;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Droid.ActionCompleted
{
    [Activity(Label = "@string/app_name")]
    public class ActionCompletedActivity : BaseActivity
	{
        public const string KEY_ACTION_TYPE = "KEY_ACTION_TYPE";
        public const string KEY_EXTRA = "KEY_EXTRA";
        string typeCurrentDocument;

        protected override int LayoutResource => Resource.Layout.activity_action_completed;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

            int actionTypeId = Intent.GetIntExtra(KEY_ACTION_TYPE, -1);

            if (actionTypeId < 0)
                throw new Exception("actionTypeId is not valid or unspecified!");

            ActionType actionType = ActionType.GetFromId(actionTypeId);


            TextView resultTV= FindViewById<TextView>(Resource.Id.textView_result);

            Button close = FindViewById<Button>(Resource.Id.button_close);
            Button nextDocument = FindViewById<Button>(Resource.Id.button_next);

            if (string.IsNullOrEmpty(actionType.doneButton))
            {
                nextDocument.Visibility = Android.Views.ViewStates.Gone;
            }
            else
            {
                nextDocument.Text = actionType.doneButton;
                nextDocument.Click += delegate {
                    Finish();
                };
            }

            close.Click += delegate {
                Finish();
            };

            TextView objectsTV = FindViewById<TextView>(Resource.Id.textView_ojects);
            resultTV.Text = actionType.doneText;

            if( actionType == ActionType.SIGN_THANK_YOU_PAGE )
            {
                nextDocument.Visibility = Android.Views.ViewStates.Gone;
            }

            string description = resultTV.Text;
            resultTV.TextFormatted = Tools.GetSpannedString(description);
            objectsTV.TextFormatted = Tools.GetSpannedString(GetStringBundle());
            resultTV.Text = actionType.SetDescriptionForTypeDocumentString(actionType.doneText, typeCurrentDocument);
            nextDocument.Text=actionType.SetDescriptionForTypeDocumentString(actionType.doneButton, typeCurrentDocument);
        }

        public string GetStringBundle()
        {
            var result = string.Empty;
            bool isFirst = true;

            var bundle = Intent.Extras;
            if (bundle != null)
            {
                foreach (String key in bundle.KeySet())
                {
                    if (!key.Equals(KEY_ACTION_TYPE) && !key.Equals(Constants.DOCUMENT_TYPE_DECRIPTION_KEY))
                    {                       
                        var value = bundle.Get(key);
                        result += value + "<br>";

                        if(isFirst)
                        {
                            result += "<br>";
                        }
                        isFirst = false;
                    }
                }

              typeCurrentDocument = bundle.GetString(Constants.DOCUMENT_TYPE_DECRIPTION_KEY);
            }

            return result;
        }
    }
}
