﻿// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.Generic;
using Foundation;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.iOS.EmbedComponent;
using InformaticaTrentinaPCL.iOS.Helper;
using InformaticaTrentinaPCL.iOS.Login.Session;
using InformaticaTrentinaPCL.OpenFile;
using InformaticaTrentinaPCL.OpenFile.MVP;
using InformaticaTrentinaPCL.Utils;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.TabBar.Document.Open.Storyboard
{
    public partial class UIViewControllerSearchDossier : UIViewController, ListenerEmbedSearch, IUITableViewDelegate, IUITableViewDataSource, IOpenFascView ,IUIScrollViewDelegate
    {
        public AbstractDocumentListItem AbstractDocument;
        private List<AbstractDocumentListItem> list;
        private OpenFascPresenter presenter;
        String textsearch = "";
        EmbedComponentListEmpty empty;
        bool isScrollService = false;
        private UIAlertController alertDownloadCancel;


        public UIViewControllerSearchDossier(IntPtr handle) : base(handle)
        {
        }

        #region interfaccia scroll for infinite scrolled
        [Export("scrollViewDidScroll:")]
        public virtual void Scrolled(UIScrollView scrollView)
        {
            var offsetY = scrollView.ContentOffset.Y;
            var contentHeight = scrollView.ContentSize.Height;
           // Console.WriteLine(!isScrollService + "-" + offsetY + ">" + (contentHeight - scrollView.Frame.Size.Height));
            if (!isScrollService && offsetY > contentHeight - scrollView.Frame.Size.Height)
            {
                isScrollService = true;
                presenter.LoadFileList();
            }
        }
        #endregion

        private void ConfigureTable()
        {
            this.tableView.Delegate = this;
            this.tableView.DataSource = this;
            this.tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.tableView.RowHeight = UITableView.AutomaticDimension;
            this.tableView.EstimatedRowHeight = 100;
        }

        private void EmptyViewList(bool force = false)
        {
            if (force)
            {
                containerEmptyView.Hidden = force;
                return;
            }

            if (list != null && list.Count > 0 /*&& !string.IsNullOrEmpty(textsearch)*/)
                containerEmptyView.Hidden = true;
            else
                containerEmptyView.Hidden = false;
        }

        private void ConfigureStyle()
        {
            buttonClose.TintColor = UIColor.Black;
            labelTitle.Text = Utility.LanguageConvert("openDossier");
            Font.SetCustomStyleFont(labelTitle, Font.MODALE_TITLE, UITextAlignment.Center);
        }

        private void ConfigurePresenter()
        {
            presenter = new OpenFascPresenter(this, IosNativeFactory.Instance(), AbstractDocument.GetIdDocumento());
            presenter.OnViewReady();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ConfigureStyle();
            ConfigureTable();
            ConfigurePresenter();
            EmptyViewList(true);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            ConfigureTablet();
        }

        private void ConfigureTablet()
        {
            if (!Utility.IsTablet()) return;

            this.leading.Constant = StyleTablet.WidthCommonPresentation(this.View);
            this.trailing.Constant = StyleTablet.WidthCommonPresentation(this.View);
            this.top.Constant = 40;
            this.bottom.Constant = StyleTablet.TOP_FROM_NAVIGATION;
        }

        partial void ActionButtonClose(NSObject sender)
        {
            DismissViewController(true, null);
        }

        #region interfaccia di ascolto per il segue di nuovi controller

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.DestinationViewController is EmbedComponentSearch)
            {
                EmbedComponentSearch embedSearch = (EmbedComponentSearch)segue.DestinationViewController;
                string s = String.Format(LocalizedString.PLACEHOLDER_SEARCH_OPEN_FASC.Get(), AbstractDocument.GetOggetto());
                string c = Utility.LanguageConvert("cancel");
                embedSearch.ConfigureEmbedComponedSearch(s, c, null, this);
            }
            else if (segue.DestinationViewController is EmbedComponentListEmpty)
            {
                empty = (EmbedComponentListEmpty)segue.DestinationViewController;
            }
        }
        #endregion

        #region interface search

        public void ActionButtonClick()
        {
            textsearch = "";
            presenter.ClearSearch();
            EmptyViewList(true);
        }

        public void EditingTextField(UITextField textField)
        {

        }
        public void KeyboardReturnDone(UITextField textField)
        {
            textsearch = textField.Text;
            presenter.OnSearch(textField.Text);
        }

        #endregion

        #region table view

        [Export("tableView:didSelectRowAtIndexPath:")]
        public virtual void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if ((list != null && list.Count > 0) && list[indexPath.Row] is  AbstractDocumentListItem)
            {
                AbstractDocumentListItem docInfo = (list != null && list.Count > 0) ? list[indexPath.Row] : null;
                presenter.OnSelect(docInfo);
            }
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return list != null ? list.Count : 0;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var model = list[indexPath.Row];


            if (model is DocumentListLoader)
            {
                CellLoaderDossier cellLoader = (CellLoaderDossier)tableView.DequeueReusableCell("CellLoaderDossier");
                cellLoader.Update();
                return cellLoader;
            }

            string CellIdentifier = "styleIos";
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);

            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, CellIdentifier);

            cell.TextLabel.Text = model.GetOggetto();
            cell.TextLabel.Lines = 0;

            Font.SetCustomStyleFont(cell.TextLabel, Font.LINK_TO_PAGE_SECTION);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }

        #endregion

        #region

        public void ShowList(List<AbstractDocumentListItem> list)
        {
            isScrollService = false;
            this.list = list;
            EmptyViewList();
            tableView.ReloadData();
        }

        public void ShowError(string e, bool isLight)
        {
            ShowErrorHelper.Show(this, isLight, e, null, () =>
            {
                ActionButtonClose(null);
            });
        }

        public void OnUpdateLoader(bool isShow)
        {
            Utility.Loading(this.View, isShow);
            if (isShow)
                EmptyViewList(isShow);
        }

        private void DownloadAlert()
        {
            alertDownloadCancel = ShowErrorHelper.CreateChoiceAlert(this, LocalizedString.CANCEL_DOWNLOAD.Get(), LocalizedString.TITLE_ALERT.Get(), () => { }, StopDownload);
            this.PresentViewController(alertDownloadCancel, animated: true, completionHandler: null);
        }

        private void StopDownload()
        {
            presenter.CancelDownload();
        }

        public void OnUpdateLoaderWithAction(bool isShow)
        {
            if (!isShow)
            {
                if (null != alertDownloadCancel)
                {
                    alertDownloadCancel.DismissViewController(false, null);
                    alertDownloadCancel = null;
                }
            }

            Utility.Loading(this.View, isShow, DownloadAlert);
        }

        public void OpenDocumentBundle(SharedDocumentBundle bundle)
        {
            UIViewControllerOpenDocumentList controller = (UIViewControllerOpenDocumentList)Utility.GetControllerFromStoryboard("Storyboard_OpenDocument", "UIViewControllerOpenDocumentList");
            controller.sharedDocument = bundle;
            this.PresentViewController(controller, true, null);
        }

        #endregion
    }
}
