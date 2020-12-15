// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.TabBar.Document.Action.Storyboard
{
	public partial class UITVCellHeaderActionDocument : UITableViewCell
	{
        Action<bool> Callback;
		public UITVCellHeaderActionDocument (IntPtr handle) : base (handle)
		{
		}

		public UITVCellHeaderActionDocument()
		{
		}

        public void Update(string title_,Action<bool> Callback)
		{
            if (label_title != null)
            label_title.Text = title_;
            this.Callback = Callback;
            Helper.Font.SetCustomStyleFont(label_title, Helper.Font.INPUT_TEXT);
            label_title.TextColor = UIColor.Black;
            label_title.Text = title_.ToUpper();
		}

		partial void ActionbuttonHeader(Foundation.NSObject sender)
        {
            this.Callback(true);
        }

	}
}
