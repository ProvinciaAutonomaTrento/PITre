// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using InformaticaTrentinaPCL.iOS.TabBar.Document.Common.Storyboard;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.TabBar.Root.Storyboard
{
	public partial class SegueEmbedSearch : UIViewController
	{
		public SegueEmbedSearch (IntPtr handle) : base (handle)
		{
		}

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.DestinationViewController is UIViewControllerDocumentList)
            {
                ((UIViewControllerDocumentList)segue.DestinationViewController).stateDocument = Home.Network.SectionType.SEARCH;
            }
        }
	}
}