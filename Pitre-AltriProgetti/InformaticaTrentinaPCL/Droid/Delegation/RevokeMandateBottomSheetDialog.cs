using Android.Content;
using Android.Support.Design.Widget;
using Android.Views;
using InformaticaTrentinaPCL.Home;

namespace InformaticaTrentinaPCL.Droid.Delegation
{
    public class RevokeMandateBottomSheetDialog : BottomSheetDialog, IDialogInterfaceOnShowListener
    {
        private BottomSheetBehavior mBehavior;
        private View bottomSheetView;
        private IRevokeMandateListener Listener;
        private DelegaDocumentModel delegaDocumentModel;
        
        public RevokeMandateBottomSheetDialog(Context context, IRevokeMandateListener Listener, DelegaDocumentModel delegaDocumentModel) : base(context)
        {
            bottomSheetView= LayoutInflater.Inflate(Resource.Layout.bottom_sheet_dialog_confirm_revocation, null);
            SetContentView(bottomSheetView);
            
            bottomSheetView.FindViewById(Resource.Id.button_close).Click += (sender, ea) =>
            {
                Hide();
            };

            bottomSheetView.FindViewById(Resource.Id.button_confirm).Click += (sender, ea) =>
            {
                Listener?.OnRevoke(delegaDocumentModel);
                Hide();   
            };
            //Fix: https://stackoverflow.com/questions/41591733/bottom-sheet-landscape-issue
            mBehavior = BottomSheetBehavior.From((View) bottomSheetView.Parent);
            SetOnShowListener(this);
            
            this.Listener = Listener;
            this.delegaDocumentModel = delegaDocumentModel;
        }
        
        public void OnShow(IDialogInterface dialog)
        {
            mBehavior.PeekHeight = bottomSheetView.Height; //get the height dynamically
        }
        
        public interface IRevokeMandateListener
        {
            void OnRevoke(DelegaDocumentModel delegaDocumentModel);
        }
    }
}