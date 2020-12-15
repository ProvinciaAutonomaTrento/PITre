using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Home.ActionDialog;

namespace InformaticaTrentinaPCL.Droid.CustomBottomSheet
{
    public class CustomBottomSheetDialog : BottomSheetDialog, IDialogInterfaceOnShowListener
    {
        List<DialogItem> actionItems;
        private BottomSheetBehavior mBehavior;
        private View bottomSheetView;

        public CustomBottomSheetDialog(Context context, List<DialogItem> actionItems) :
            base(context)
        {
            bottomSheetView = LayoutInflater.Inflate(Resource.Layout.custom_bottom_sheet_dialog, null);
            
            bottomSheetView.FindViewById(Resource.Id.textView_title).Click += delegate{ Hide(); };
            bottomSheetView.FindViewById(Resource.Id.imageView_close).Click += delegate{ Hide(); };
            
            LinearLayout actionsContainer = bottomSheetView.FindViewById<LinearLayout>(Resource.Id.linearLayout_actionsContainer);
            TextView textView;

            for (int i = 0; i < actionItems.Count; i++)
            {
                textView = (TextView)LayoutInflater.Inflate(Resource.Layout.component_action_dialog_item, null);
                textView.Text = actionItems[i].Title;
                Action action = actionItems[i].OnClickHandler;
                textView.Click += delegate
                {
                    Dismiss();
                    action();
                }; 
                actionsContainer.AddView(textView);
            }

            try
            {

                SetContentView(bottomSheetView);
                
                //Fix: https://stackoverflow.com/questions/41591733/bottom-sheet-landscape-issue
                mBehavior = BottomSheetBehavior.From((View)bottomSheetView.Parent);
                SetOnShowListener(this);
            }
            catch(Exception e)
            {
                string s = e.Message; 
                Console.WriteLine("Errore: in custombottonsheetdialog -> "+s);

              
            }


        }

        public void OnShow(IDialogInterface dialog)
        {
            mBehavior.PeekHeight = bottomSheetView.Height; //get the height dynamically
        }
    }
    
}