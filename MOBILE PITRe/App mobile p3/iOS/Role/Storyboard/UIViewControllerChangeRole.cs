﻿// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.Generic;
using Foundation;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.ChangeRole.MVPD;
using InformaticaTrentinaPCL.iOS.Helper;
using InformaticaTrentinaPCL.iOS.Login.Session;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Utils;
using SDWebImage;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Role.Storyboard
{
  public partial class UIViewControllerChangeRole : UIViewController, IUITableViewDelegate, IUITableViewDataSource, IRoleView
  {
    private IRolePresenter presenter;
    private List<ChangeRole.RuoloInfo> roles;
    private UserInfo user = IosNativeFactory.Instance().GetSessionData().userInfo;

    public UIViewControllerChangeRole(IntPtr handle) : base(handle)
    {
    }

    public void ConfigureTable()
    {
      this.roles = user.ruoliFiltered;
      tableView.Delegate = this;
      tableView.DataSource = this;
      tableView.EstimatedRowHeight = 100;
      tableView.RowHeight = UITableView.AutomaticDimension;
    }

    public nint RowsInSection(UITableView tableView, nint section)
    {
      if (this.roles != null)
        return this.roles.Count + 1;

      return 1;
    }

    public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
      UITVCellChangeRole cell = (UITVCellChangeRole) tableView.DequeueReusableCell("UITVCellChangeRole");

      if (cell == null)
        cell = new UITVCellChangeRole();

      if (indexPath.Row == 0)
        cell.Update(Utility.LanguageConvert("title_Rolo_table").ToUpper(), CellType.Header);
      else
        cell.Update(roles[indexPath.Row - 1].descrizione, CellType.Row);

      return cell;
    }

    partial void ActionButtonClose(Foundation.NSObject sender)
    {
      this.DismissViewController(true, null);
    }

    [Export("tableView:didSelectRowAtIndexPath:")]
    public virtual void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
      // se è solo se row e non header 
      if (indexPath.Row != 0)
        presenter.SetChangeRole(roles[indexPath.Row - 1]);
    }

    public override void ViewDidLoad()
    {
      base.ViewDidLoad();
      ConfigureTable();
      presenter = new RolePresenter(this, IosNativeFactory.Instance());
      ProfileUSer();
      ConfigureStyle();
    }

    public void ConfigureStyle()
    {
      title_page.Text = Utility.LanguageConvert("title_ChangeRole").ToUpper();
      Font.SetCustomStyleFont(title_page, Font.TITLE_BLACK, UITextAlignment.Center);
      Font.SetCustomStyleFont(subTitle, Font.HEADER_TABLE);
      subTitle.TextColor = UIColor.Black;
      Font.SetCustomStyleFont(labelName, Font.ROW_TABLE_TITLE_BLACK);
      Font.SetCustomStyleFont(labelRole, Font.ROW_TABLE_SUB_TITLE_BLACK);

      subTitle.Text = LocalizedString.ACTUAL_ROLE.Get();
      subTitle.Hidden = !Utility.IsTablet();
      profileImg.Hidden = Utility.IsTablet();
      heightSubTitle.Constant = Utility.IsTablet() ? heightSubTitle.Constant : 0;
      leftConstraintLabelone.Constant = Utility.IsTablet() ? 20 : leftConstraintLabelone.Constant;
    }

    public override void ViewDidLayoutSubviews()
    {
      base.ViewDidLayoutSubviews();

      this.marginLeft.Constant = StyleTablet.MarginLeftDefault();
      this.marginRight.Constant = StyleTablet.MarginRightDefault();
      this.marginDown.Constant = StyleTablet.MarginBottonDefault();
      this.marginTop.Constant = StyleTablet.MarginTopDefault();
    }

    public override void ViewDidDisappear(bool animated)
    {
      base.ViewDidDisappear(animated);
      presenter.Dispose();
    }

    public void ProfileUSer()
    {
      profileImg.Image = UIImage.FromBundle("imageUserDefault"); // default 
      if (!String.IsNullOrEmpty(IosNativeFactory.Instance().GetSessionData().userInfo.URLImageUser))
      {
        profileImg.SetImage(
          url: new NSUrl(IosNativeFactory.Instance().GetSessionData().userInfo.URLImageUser),
          placeholder: UIImage.FromBundle("imageUserDefault"),
          options: SDWebImageOptions.CacheMemoryOnly,
          completedBlock: (imageDownloaded, error, cacheType, imageUrl) =>
          {
            if (error != null)
              Console.WriteLine(error.Description);
          }
        );
      }

      profileImg.Layer.CornerRadius = 15;
      labelName.Text = user.descrizione;
      labelRole.Text = user.ruoli[0].descrizione;
    }

    public void ShowError(string e, bool isLight)
    {
      ShowErrorHelper.Show(this, isLight, e);
    }

    public void OnUpdateLoader(bool isShow)
    {
      Utility.Loading(this.View, isShow);
    }

    public void OnChangeRoleOK(ChangeRole.RuoloInfo role)
    {
      user.ruoli[0].descrizione = role.descrizione;
      labelRole.Text = user.ruoli[0].descrizione;
      DismissViewController(true, null);
    }
  }
}