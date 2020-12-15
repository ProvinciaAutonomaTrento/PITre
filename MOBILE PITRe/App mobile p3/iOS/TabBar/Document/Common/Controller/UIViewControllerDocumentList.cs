﻿// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using InformaticaTrentinaPCL.CommonAction;
using InformaticaTrentinaPCL.CommonAction.MVP;
using InformaticaTrentinaPCL.Filter;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVP;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.iOS.Filter;
using InformaticaTrentinaPCL.iOS.Helper;
using InformaticaTrentinaPCL.iOS.Login.Session;
using InformaticaTrentinaPCL.iOS.PopUp.Storyboard;
using InformaticaTrentinaPCL.iOS.TabBar.Document.Action.Root.Storyboard;
using InformaticaTrentinaPCL.iOS.TabBar.Document.Action.Storyboard;
using InformaticaTrentinaPCL.iOS.TabBar.Root.Controller;
using InformaticaTrentinaPCL.iOS.TabBar.Root.Storyboard;
using InformaticaTrentinaPCL.Login;
using UIKit;
using InformaticaTrentinaPCL.Utils;
using InformaticaTrentinaPCL.Home.ActionDialog;
using InformaticaTrentinaPCL.iOS.TabBar.Document.Open.Storyboard;
using InformaticaTrentinaPCL.iOS.Delega.Storyboard;
using InformaticaTrentinaPCL.iOS.EmbedComponent;
using InformaticaTrentinaPCL.OpenFile;
using InformaticaTrentinaPCL.Search.MVP;
using InformaticaTrentinaPCL.Search;
using InformaticaTrentinaPCL.Network;
using static InformaticaTrentinaPCL.iOS.TabBar.Document.Action.Root.Storyboard.UIVControllerSign;
using static InformaticaTrentinaPCL.Home.Network.LibroFirmaResponseModel;

namespace InformaticaTrentinaPCL.iOS.TabBar.Document.Common.Storyboard
{
    public partial class UIViewControllerDocumentList : MyUIViewController, InterfaceNavigation, IUITableViewDelegate, IUITableViewDataSource, IDocumentListView, IUIScrollViewDelegate, ListenerEmbedSearch, ISearchView
    {

        #region variabili di classe 
        public SectionType stateDocument;
        private List<AbstractDocumentListItem> documents = new List<AbstractDocumentListItem>();
        private DocumentListPresenter presenter;
        private UserInfo userModel = IosNativeFactory.Instance().GetSessionData().userInfo;
        public UIViewControllerDocumentList(IntPtr handle) : base(handle) { }
        bool isScrollService = false;
        private UIRefreshControl refresh;
        private List<DialogItem> itemList;
        private String LastToken;
        EmbedComponentListEmpty empty;
        private String textString = "";
        private UIViewControllerActionDocument controllerActionDocument;
        public static string signFlowFinishedType = null;
        #endregion

        #region refresh

        void HandleEventHandler(object sender, EventArgs e)
        {
            refresh.EndRefreshing();
            presenter.OnPullToRefresh();
        }

        #endregion

        #region interfaccia scroll for infinite scrolled
        [Export("scrollViewDidScroll:")]
        public virtual void Scrolled(UIScrollView scrollView)
        {
            var offsetY = scrollView.ContentOffset.Y;
            var contentHeight = scrollView.ContentSize.Height;
            var diffContentHeightScrollHeight = contentHeight - scrollView.Frame.Size.Height;
            //Console.WriteLine((!isScrollService) + "*/*" + offsetY + ">" + (diffContentHeightScrollHeight));

            if (!isScrollService && diffContentHeightScrollHeight >= 0 && offsetY > diffContentHeightScrollHeight)
            {
                isScrollService = true;
                presenter.LoadDocumentList();
            }
        }
        #endregion

        #region method private 

        private void LaunchActionSheet(UIStoryboardSegue segue)
        {
            controllerActionDocument = (UIViewControllerActionDocument)segue.DestinationViewController;
            controllerActionDocument.itemList = this.itemList;
        }

        public void RegisterForNotificationsOrientations()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidChangeStatusBarOrientationNotification, (obj) =>
            {
                tableView.ReloadData();
            });
        }

        public void ShowSignFromTabBar()
        {
            ShowSign(ActionType.SIGN, documents[0]);
        }

        private void ShowSign(ActionType type, AbstractDocumentListItem abstractDocument)
        {
            UIVControllerSign controller = (UIVControllerSign)Utility.GetControllerFromStoryboard("Storyboard_ActionDocument", "UIVControllerSign");
            controller.actionType = type;
            controller.model = abstractDocument;
            if (Utility.IsTablet())
            {
                this.PresentViewController(controller, true, null);
            }
            else
            {
                this.NavigationController.PushViewController(controller, true);
            }
        }

        private void ShowSignWithOTp(ActionType type, List<AbstractDocumentListItem> abstractDocumentOtp, int TotalRecordSignOtp, string signType)
        {
            UIVControllerSign controller = (UIVControllerSign)Utility.GetControllerFromStoryboard("Storyboard_ActionDocument", "UIVControllerSign");
            controller.ConfigureForSignOtp(abstractDocumentOtp, type, TotalRecordSignOtp, signType, presenter.signFlowType);
            if (Utility.IsTablet())
            {
                this.PresentViewController(controller, true, null);
            }
            else
            {
                this.NavigationController.PushViewController(controller, true);
            }
        }

        private void ShowAccept_ADL_Refuse(ActionType type, AbstractDocumentListItem abstractDocument)
        {
            UIVControllerIActionPresenter controller = (UIVControllerIActionPresenter)Utility.GetControllerFromStoryboard("Storyboard_ActionDocument", "UIVControllerIActionPresenter");
            controller.AbstractDocument = abstractDocument;
            controller.actionType = type;

            if (Utility.IsTablet())
            {
                this.PresentViewController(controller, true, null);
            }
            else
            {
                this.NavigationController.PushViewController(controller, true);
            }
        }

        private void ShowAssign(ActionType type, AbstractDocumentListItem abstractDocument)
        {
            UIVControllerAssign controller = (UIVControllerAssign)Utility.GetControllerFromStoryboard("Storyboard_ActionDocument", "UIVControllerAssign");
            controller.AbstractDocumentListItem = abstractDocument;
            controller.actionType = type;

            if (Utility.IsTablet())
            {
                this.PresentViewController(controller, true, null);
            }
            else
            {
                this.NavigationController.PushViewController(controller, true);
            }
        }

        #endregion

        #region FILTER 

        public void ConfigureFilter(bool isVisible, string text = "")
        {
            try
            {
                viewFilter.Hidden = !isVisible;
                labelDateFilter.Text = text;
                heightFilterView.Constant = isVisible ? 50 : 0;
                viewFilter.BackgroundColor = Colors.COLOR_RED_DELEGATE_SWIPE;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ConfigureFilter Error: " + ex.Message);
                viewFilter.Hidden = true;
                heightFilterView.Constant = 0;

            }
        }

        /// <summary>
        ///  interefacci navigation Callbacks the button filter.
        /// </summary>
        public void CallbackButtonFilter()
        {
            this.presenter.OpenFilterView();
        }

        private void Callback(FilterModel filter)
        {
            presenter.SetFilterModel(filter);
        }

        partial void ActionButtonDeleteFilter(UIButton sender)
        {
            presenter.SetFilterModel(null);
        }

        #endregion

        #region SEARCH

        public void ConfigureSearch(bool isVisible)
        {
            heightSearchView.Constant = isVisible ? 50 : 0;
            viewSearch.Hidden = !isVisible;
            empty.SetTopMargin(isVisible ? -18 : 0);
        }

        #endregion

        #region Embed search component 

        public void ActionButtonClick()
        {
            ((SearchPresenter)presenter).ClearSearch();
            textString = "";
            PageEmptyList(true);
        }

        public void EditingTextField(UITextField textField)
        {

        }

        public void KeyboardReturnDone(UITextField textField)
        {
            textString = textField.Text;
            ((SearchPresenter)presenter).UpdateSearchString(textString);
        }

        #endregion

        #region interfaccia di ascolto per il segue di nuovi controller
        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            // chiamato da PerformSegue("Storyboard_ActionDocument", null);
            if (segue.DestinationViewController is UIViewControllerActionDocument)
                this.LaunchActionSheet(segue);
            else if (segue.DestinationViewController is EmbedComponentSearch)
            {
                EmbedComponentSearch embedSearch = (EmbedComponentSearch)segue.DestinationViewController;
                string s = Utility.LanguageConvert("search_document_list");
                string c = Utility.LanguageConvert("cancel");
                embedSearch.ConfigureEmbedComponedSearch(s, c, null, this);
            }
            else if (segue.DestinationViewController is EmbedComponentListEmpty)
            {
                empty = (EmbedComponentListEmpty)segue.DestinationViewController;
                empty.RefreshCallback = HandleEventHandler;
                empty.stateDocument = stateDocument;
            }
        }

        #endregion

        #region interfaccia table view

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            AbstractDocumentListItem item = documents[indexPath.Row];

            if (item is DocumentListLoader)
            {
                UITVCellLoader cellLoader = (UITVCellLoader)tableView.DequeueReusableCell("UITVCellLoader");
                if (cellLoader == null)
                    cellLoader = new UITVCellLoader();
                cellLoader.Update();
                return cellLoader;
            }

            UITableViewCellDocument cell = (UITableViewCellDocument)tableView.DequeueReusableCell("UITableViewCellDocument");
            cell.Update(this, item, stateDocument, this.presenter);

            return cell;
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return documents.Count;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public virtual void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (stateDocument != SectionType.SIGN)
                presenter.OnTapDocument(documents[indexPath.Row]);
        }

        [Export("tableView:canEditRowAtIndexPath:")]
        public bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            //Console.WriteLine("CanEditRow" + indexPath.Section + "-" + indexPath.Row);
            return !(documents[indexPath.Row] is DocumentListLoader);
        }

        [Export("tableView:editActionsForRowAtIndexPath:")]
        public UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
        {
            //Console.WriteLine("EditActionsForRow" + indexPath.Section + "-" + indexPath.Row);
            string title_a = Utility.LanguageConvert("open_swipe");

            var open = UITableViewRowAction.Create(UITableViewRowActionStyle.Normal, title_a.ToUpper(), (arg1, indexPathSelected) =>
            {
                presenter.OnOpenDocument(documents[indexPath.Row]);
            });
            open.BackgroundColor = Colors.COLOR_SWIPE_BUTTON_TABLE_DOCUMENT;
            return new UITableViewRowAction[] { open };
        }
        #endregion 

        public void ConfigureAutomaticTable()
        {
            tableView.Delegate = this;
            tableView.RowHeight = UITableView.AutomaticDimension;
            tableView.EstimatedRowHeight = 100;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            PageEmptyList(true);
            ConfigureSearch(stateDocument == SectionType.SEARCH);
            if (stateDocument == SectionType.SEARCH)
                presenter = new SearchPresenter(this, IosNativeFactory.Instance());
            else
                presenter = new DocumentListPresenter(this, IosNativeFactory.Instance(), this.stateDocument);
            ConfigureAutomaticTable();
            refresh = Utility.AddRefreshToTable(HandleEventHandler, this.tableView);
            ConfigureFilter(false);
            RegisterForNotificationsOrientations();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            // getione searche in alto
            if (Utility.IsTablet())
            {
                trailing.Constant = StyleTablet.WidthCommonPresentation(this.View) - 20;
                leading.Constant = StyleTablet.WidthCommonPresentation(this.View) - 20;
            }
        }

        public override void ViewDidAppear(bool animated)
        {

           BeginInvokeOnMainThread(() =>
            {
                base.ViewDidAppear(animated);
                TabBarNavigationManager.Instance().SetCallbackButton(this);
                var tokenNow = SessionDataManager.Instance().GetSessionData().userInfo.token;
                if (String.IsNullOrEmpty(LastToken) && stateDocument != SectionType.SEARCH)
                {
                    LastToken = tokenNow;
                    presenter.LoadDocumentList();
                }
                else
                {
                    presenter.OnPullToRefresh();  

                }

                if (AppDelegate.sharedUrl != null && AppDelegate.sharedUrl.isValid)
                {
                    presenter.OnShareDocumentReceived(AppDelegate.sharedUrlString);
                    AppDelegate.sharedUrlString = null; // consumo l app url
                    AppDelegate.sharedUrl = null; // consumo l app url
                }

                //if the sign of previous document was occured then continue with others
                if (signFlowFinishedType != null)
                {
                    if (signFlowFinishedType == Constants.ACTION_SIGN_CADES)
                        presenter.OnDocumentsSignCadesActionFinished(true);
                    if (signFlowFinishedType == Constants.ACTION_SIGN_CADES_NOT_SIGNED)
                        presenter.OnDocumentsSignCadesNotSignedActionFinished(true);
                    if (signFlowFinishedType == Constants.ACTION_SIGN_PADES)
                        presenter.OnDocumentsSignPadesActionFinished(true);
                    signFlowFinishedType = null;
                }
            });

        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        /// <summary>
        /// Pages the empty list. force = true nasconde sempre la view 
        /// </summary>
        /// <param name="force">If set to <c>true</c> force.</param>
        private void PageEmptyList(bool force = false)
        {
            if (force)
            {
                errorView.Hidden = true;
                ContainerEmptyView.Hidden = true;
                return;
            }
            if (this.documents != null && this.documents.Count() == 0) {
                errorView.Hidden = false;
                ContainerEmptyView.Hidden = false;
            } else {
                errorView.Hidden = true;
                ContainerEmptyView.Hidden = true;
            }

            //   if (stateDocument == SectionType.SEARCH && textString == "")
            if (stateDocument == SectionType.SEARCH)
            {
                errorView.Hidden = true;
                ContainerEmptyView.Hidden = true;
            }
        }

        #region interfaccia pcl

        public void UpdateList(List<AbstractDocumentListItem> documents)
        {
            BeginInvokeOnMainThread(() =>
            {
                isScrollService = false;
                this.documents.Clear();
                if (documents != null)
                {
                    this.documents.AddRange(documents);
                }
                bool forceEmptyList = documents == null;
                PageEmptyList(forceEmptyList);
                tableView.ReloadData();
            });

        }

        private void ResetTableView()
        {
            PageEmptyList();
            this.tableView.SetContentOffset(new CoreGraphics.CGPoint(0, 0), true);
            this.tableView.ReloadData();
        }

        public void ShowError(string e, bool isLight)
        {
            ShowErrorHelper.Show(this, isLight, e, null, () =>
            {
                ResetTableView();
            });
        }

        public void OnUpdateLoader(bool isShow)
        {
            PageEmptyList(isShow);
            Utility.Loading(this.View, isShow);
        }

        /// <summary>
        /// visible view
        /// </summary>
        /// <param name="filterModel">Filter model.</param>
        public void UpdateFilterUI(FilterModel filterModel)
        {
            bool isFilterViewVisible = filterModel != null;
            string label = filterModel != null ? filterModel.GetLabel() : "";
            ConfigureFilter(isFilterViewVisible, label);
        }

        /// <summary>
        /// Apri filter View 
        /// </summary>
        /// <param name="filterModel">Filter model.</param>
        public void OpenFilterView(FilterModel filterModel)
        {
            UIVControllerFilter filter = (UIVControllerFilter)Utility.GetControllerFromStoryboard("Storyboard_Filter", "UIVControllerFilter");
            filter.CallbackFinish = Callback;
            filter.sectionType = this.stateDocument;
            filter.InitPresenter(filterModel);
            this.PresentViewController(filter, true, () =>
            {
                Console.WriteLine("Filter loaded");
            });
        }

        public void DoOpenFirma(AbstractDocumentListItem abstractDocument)
        {
            controllerActionDocument.DismissViewController(false, null);
            ShowSign(ActionType.SIGN, abstractDocument);
        }

        public void DoOpenDialog(List<DialogItem> itemList)
        {
            this.itemList = itemList;
            PerformSegue("Storyboard_ActionDocument", null);
        }

        public void DoAccetta(AbstractDocumentListItem abstractDocument)
        {
            controllerActionDocument.DismissViewController(false, null);
            ShowAccept_ADL_Refuse(ActionType.ACCEPT, abstractDocument);
        }

        public void DoAccettaEADL(AbstractDocumentListItem abstractDocument)
        {
            controllerActionDocument.DismissViewController(false, null);
            ShowAccept_ADL_Refuse(ActionType.ACCEPT_AND_ADL, abstractDocument);
        }

        public void DoRifiuta(AbstractDocumentListItem abstractDocument)
        {
            controllerActionDocument.DismissViewController(false, null);
            ShowAccept_ADL_Refuse(ActionType.REFUSE, abstractDocument);
        }

        public void DoApriFascicolo(AbstractDocumentListItem abstractDocument)
        {
            UIViewControllerSearchDossier controller = (UIViewControllerSearchDossier)Utility.GetControllerFromStoryboard("Storyboard_OpenDocument", "UIViewControllerSearchDossier");
            controller.AbstractDocument = abstractDocument;
            this.PresentViewController(controller, true, null);
        }

        public void DoRimuoviDaADL(AbstractDocumentListItem abstractDocument)
        {
            presenter.RemoveADLAction(abstractDocument);
        }

        public void DoAssegna(AbstractDocumentListItem abstractDocument)
        {
            controllerActionDocument.DismissViewController(false, null);
            ShowAssign(ActionType.ASSIGN, abstractDocument);
        }

        public void DoInserisciInADL(AbstractDocumentListItem abstractDocument)
        {
            controllerActionDocument.DismissViewController(false, null);
            presenter.AddADLAction(abstractDocument);
        }

        public void OnRemoveFromADLOk(string message)
        {
            controllerActionDocument.DismissViewController(false, null);
            if (stateDocument == SectionType.ADL)
            {
                presenter.OnPullToRefresh();
                tableView.ReloadData();
            }
            else
            {
                Alert.AlertToast(message, this);
            }

        }

        public void OnAddInADLOk(string message)
        {
            if (stateDocument == SectionType.ADL)
            {
                presenter.LoadDocumentList();
            }
            else
            {
                Alert.AlertToast(message, this);
            }
        }
        #region Share/Open 
        public void OnShareLinkReady(string link)
        {
            var item = NSObject.FromObject(link);
            var activityItems = new NSObject[] { item };
            UIActivity[] applicationActivities = null;
            var activityController = new UIActivityViewController(activityItems, applicationActivities);

            if (Utility.IsTablet())
            {
                activityController.PopoverPresentationController.SourceView = View;
                activityController.PopoverPresentationController.SourceRect = new CoreGraphics.CGRect((View.Bounds.Width / 2), (View.Bounds.Height / 4), 0, 0);
            }

            this.PresentViewController(activityController, true, null);

        }

        public void DoApriDocumentoCondiviso(SharedDocumentBundle sharedDocument)
        {
            UIViewControllerOpenDocumentList controller = (UIViewControllerOpenDocumentList)Utility.GetControllerFromStoryboard("Storyboard_OpenDocument", "UIViewControllerOpenDocumentList");
            controller.sharedDocument = sharedDocument;
            this.PresentViewController(controller, true, null);
        }

        public void DoCondividi(AbstractDocumentListItem abstractDocument)
        {
            controllerActionDocument.DismissViewController(false, null);
            UIViewControllerPeopleChooseFavorite choose = (UIViewControllerPeopleChooseFavorite)Utility.GetControllerFromStoryboard("Storyboard_Delegation", "UIViewControllerPeopleChooseFavorite");
            choose.Label_Title_Custom = Utility.LanguageConvert("Sharing_witch");
            choose.Label_Search_Custom = Utility.LanguageConvert("Search_user");
            choose.operationType = OperationTypeEnum.Share;
            // row selected 
            choose.CallbackRowSelected += (abstractRecipient) =>
            {
                choose.DismissViewController(true, null);
                presenter.ShareDocument(abstractDocument, abstractRecipient);
            };
            this.PresentViewController(choose, true, null);
        }

        public void DoApriDocumento(AbstractDocumentListItem abstractDocument)
        {
            UIViewControllerOpenDocumentList controller = (UIViewControllerOpenDocumentList)Utility.GetControllerFromStoryboard("Storyboard_OpenDocument", "UIViewControllerOpenDocumentList");
            controller.AbstractDocument = abstractDocument;
            this.PresentViewController(controller, true, null);
        }

        public void DoViewed(AbstractDocumentListItem abstractDocument)
        {
            presenter.DoViewed(abstractDocument);
        }

        public void DoViewedADL(AbstractDocumentListItem abstractDocument)
        {
            presenter.DoViewedADL(abstractDocument);
        }

        public void OnViewedOk(string message)
        {
            presenter.OnPullToRefresh();
            controllerActionDocument.DismissViewController(false, null);
            Alert.AlertToast(message, this);
        }

        #endregion

        #region  Navigation button sign all / button reject all

        public void CallSignDocument()
        {
            presenter.ShowDialogActionsDocuments();
        }

        /// <summary>
        /// Dos the sign all.
        /// </summary>
        public void DoSignAll()
        {
            controllerActionDocument?.DismissViewController(false, null);
            presenter.TapSignAll();
        }

        /// <summary>
        /// Dos the reject all.
        /// </summary>
        public void DoRejectAll()
        {
            controllerActionDocument?.DismissViewController(false, null);
            presenter.TapRejectAll();
        }

        /// <summary>
        /// Completeds the action ok.
        /// </summary>
        /// <param name="extra">Extra.</param>
        public void CompletedActionOK(Dictionary<string, string> extra)
        {
            ActionType actionTypeCustom = ActionType.SIGN_THANK_YOU_PAGE;
            Alert.PopUpStateDocument(stateDocument, this, actionTypeCustom, extra);
        }

        public void DoOpenViewOtpCades(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned)
        {
            ShowSignWithOTp(ActionType.VIEW_OTP, listStateDaFirmareOTP, TotalRecordCountSigned, Constants.ACTION_SIGN_CADES);
        }

        public void DoOpenViewOtpCadesNotSigned(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned)
        {
            ShowSignWithOTp(ActionType.VIEW_OTP, listStateDaFirmareOTP, TotalRecordCountSigned, Constants.ACTION_SIGN_CADES_NOT_SIGNED);
        }

        public void DoOpenViewOtpPades(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned)
        {
            ShowSignWithOTp(ActionType.VIEW_OTP, listStateDaFirmareOTP, TotalRecordCountSigned, Constants.ACTION_SIGN_PADES);
        }

        public void ShowAlertWithNumberDocuments(string description, string actionType)
        {
            UIAlertController alertWithNumberDocuments = ShowErrorHelper.CreateChoiceAlert(this, description, LocalizedString.TITLE_ALERT.Get(), () =>
            {
                presenter.ActionContinue(actionType);
            }, () => { });
            this.PresentViewController(alertWithNumberDocuments, animated: true, completionHandler: null);
        }

        public void OnSignCompleted(string message)
        {
            Alert.AlertToast(message, this);
        }

        public void OnDocumentUpdated(AbstractDocumentListItem document)
        {
            tableView.ReloadRows(new NSIndexPath[] { NSIndexPath.FromRowSection(documents.IndexOf(document), 0) }, UITableViewRowAnimation.None);
        }

        public void OnViewedADLOk(string message)
        {
            presenter.OnPullToRefresh();
            controllerActionDocument.DismissViewController(false, null);
            Alert.AlertToast(message, this);
        }

        #endregion

        

        #endregion
    }
}
