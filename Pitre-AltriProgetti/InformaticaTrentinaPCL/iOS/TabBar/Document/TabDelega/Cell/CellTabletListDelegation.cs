// This file has been autogenerated from a class added in the UI designer.

using System;
using Foundation;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.iOS.Helper;
using InformaticaTrentinaPCL.Utils;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.TabBar.Document.TabDelega.Storyboard
{
  public partial class CellTabletListDelegation : UITableViewCell
  {
    public CellTabletListDelegation(IntPtr handle) : base(handle)
    {
    }

    public void Update(DelegaDocumentModel dd)
    {
      viewFooter.BackgroundColor = Colors.FOOTER_SEPARATOR;
      labelTitle.Text = dd.delegato;
      labelDescRight.Text = dd.delegante;
      labelDescCenter.Text = dd.dataScadenzaDelega;
      labelDescLeft.Text = dd.dataDecorrenzaDelega;
      labelTitleLeft.Text = Utility.LanguageConvert("decorrenza");
      labelTitleRight.Text = Utility.LanguageConvert("delegante");
      labelTitleCenter.Text = Utility.LanguageConvert("scadenza");
      labelDescCenter.Text = EndDateHelper.CheckVisibilityEndDate(dd) ? "" : dd.dataScadenzaDelega;
      Font.SetCustomStyleFont(labelTitle, Font.ROW_TABLE_TITLE_BLACK);
      Font.SetCustomStyleFont(labelTitleLeft, Font.HEADER_TABLE);
      Font.SetCustomStyleFont(labelTitleRight, Font.HEADER_TABLE);
      Font.SetCustomStyleFont(labelTitleCenter, Font.HEADER_TABLE);
      Font.SetCustomStyleFont(labelDescRight, Font.ROW_TABLE_SUB_TITLE_BLACK);
      Font.SetCustomStyleFont(labelDescCenter, Font.ROW_TABLE_SUB_TITLE_BLACK);
      Font.SetCustomStyleFont(labelDescLeft, Font.ROW_TABLE_SUB_TITLE_BLACK);
      ConfigureTablet();
    }

    private void ConfigureTablet()
    {
      leading.Constant = StyleTablet.MarginRightAndLeftForTableView(150);
      trailing.Constant = StyleTablet.MarginRightAndLeftForTableView(150);
    }
  }
}