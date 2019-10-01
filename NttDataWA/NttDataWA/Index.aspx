<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="NttDataWA.Index"
    MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Src="~/UserControls/HomeTabs.ascx" TagPrefix="uc3" TagName="HomeTabs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <link href="Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function generateRendomExportFileName() {
            var text = "_";
            var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            for (var i = 0; i < 5; i++)
                text += possible.charAt(Math.floor(Math.random() * possible.length));

            return text;
        }

        function changeBgImage(image, id, image2, id2) {
            var element = document.getElementById(id);
            element.style.backgroundImage = "url(" + image + ")";

            var element2 = document.getElementById(id2);
            element2.style.backgroundImage = "url(" + image2 + ")";
        }

        var ajaxCall = function (urlPost) {
            $.ajax({
                type: 'POST',
                url: urlPost,
                success: function (data, textStatus, jqXHR) {
                    if(jqXHR && (jqXHR.status === 500 )){ 
                        ajaxModalPopupVerificaConnettoreSocket();
                    }
                    //try {
                    //    // oggeto settato in VerificaConnettoreSocket.aspx
                    //    if (data.check === "error") {
                    //        ajaxModalPopupVerificaConnettoreSocket();
                    //    }
                    //    // console.log("ajacCall: " + JSON.stringify(data));
                    //} catch (error) {
                    //    console.error(error);
                    //}
                    //if(jqXHR && (jqXHR.status === 500 )){ 
                    //    ajaxModalPopupVerificaConnettoreSocket();
                    //}
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    if(jqXHR && (jqXHR.status === 500 )){ 
                        ajaxModalPopupVerificaConnettoreSocket();
                    }
                },
                async: true
            });
        };

        function CheckConnector() {
            try {
                var url = 'Popup/VerificaConnettoreSocket.aspx?from=index&version=';
                var frame = parent.fra_sessionend; 
                var isIEmin9 = false;
                if ($.browser.msie && $.browser.version < 10) isIEmin9 = true;
                if(!isIEmin9){
                    getVersion(
                         function (version, connection) {
                             connection.close(); 
                             if(!version)
                                 version = '';
                             url = url + version;
                             ajaxCall(url);
                         
                         }, 
                        function (error, connection) {
                            console.error("Connection Error",error);
                            connection.close();
                            ajaxCall(url);
                        });
                }
            }catch(ex){
                ajaxCall(url);
            }
        }

    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <asp:UpdatePanel ID="UpPnlJs" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <script type="text/javascript">
            $(window).load(function() {
                <asp:Literal ID="js_code" runat="server" />
            });                
            </script>
        </ContentTemplate>
    </asp:UpdatePanel>
    <uc:ajaxpopup2 Id="DocumentViewer" runat="server" Url="Popup/DocumentViewer.aspx"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closeZoom');}" />
    <uc:ajaxpopup2 Id="AddFilterNotificationCenter" runat="server" Url="Popup/AddFilterNotificationCenter.aspx"
        PermitClose="false" Width="660" Height="600" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closeAddFilterNotificationCenter');}" />
    <uc:ajaxpopup2 Id="ViewDetailNotify" runat="server" Url="Popup/ViewDetailNotify.aspx"
        PermitClose="false" Width="1100" Height="1000" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closePopupViewDetailNotify');}" />
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="Popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui){__doPostBack('UpPnlButtons', 'closePopupAddressBook');}" />
    <uc:ajaxpopup2 Id="Object" runat="server" Url="Popup/Object.aspx" PermitClose="false"
        PermitScroll="false" Width="800" Height="1000" IsFullScreen="false" CloseFunction="function (event, ui){__doPostBack('UpPnlButtons', 'closePopupObject');}" />
    <uc:ajaxpopup2 Id="NotificationNoticeDays" runat="server" Url="Popup/NotificationNoticeDays.aspx"
        PermitClose="false" PermitScroll="false" Width="500" Height="400" IsFullScreen="false"
        CloseFunction="function (event, ui){__doPostBack('UpPnlButtons', 'closePopupNotificationNoticeDays');}" />
    <uc:ajaxpopup2 Id="SmistamentoDocumenti" runat="server" Url="Popup/SmistamentoDocumenti.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closePopupViewDetailNotify');}" />
    <uc:ajaxpopup2 Id="ExportDati" runat="server" Url="ExportDati/exportDatiSelection.aspx?export=notify"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="DigitalSignDetails" runat="server" Url="popup/DigitalSignDetails.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="Disservizio" runat="server" Url="Popup/Disservizio.aspx"
        PermitClose="false" PermitScroll="false" Width="600" Height="400" IsFullScreen="false"
        CloseFunction="function (event, ui){__doPostBack('UpPnlButtons', 'closePopupDisservizio');}" />
    <uc:ajaxpopup2 Id="VerificaConnettoreSocket" runat="server" Url="Popup/VerificaConnettoreSocket.aspx"
        Width="550" Height="400" PermitClose="true" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', ''); }" />
    <uc:ajaxpopup2 Id="OpenTitolario" runat="server" Url="Popup/ClassificationScheme.aspx"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closePopupOpenTitolario'); }" />
    <uc:ajaxpopup2 Id="SearchProject" runat="server" Url="Popup/SearchProject.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpPnlButtons', 'closePopupSearchProject');}" />
    <div id="containerTop">
        <div id="containerTabHome">
            <asp:UpdatePanel runat="server" ID="UpHeaderHome" UpdateMode="Conditional" ClientIDMode="static">
                <ContentTemplate>
                    <div id="containerStandardTop">
                        <div id="containerStandardTopCxHome">
                            <img src="Images/Common/griff.png" alt="" title="" />
                        </div>
                        <div id="containerHomeHeader">
                            <div id="containerHeaderHomeSx">
                                <strong>
                                    <asp:Label runat="server" ID="headerHomeLblRole"></asp:Label>
                                </strong>
                            </div>
                            <div id="containerHeaderHomeDx">
                                <div class="styled-select_full">
                                    <asp:DropDownList ID="ddlRolesUser" runat="server" OnSelectedIndexChanged="HomeDdlRoles_SelectedIndexChange"
                                        CssClass="chzn-select-deselect" AutoPostBack="true" Width="700">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="containerTabIndex">
                <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
                    ClientIDMode="Static">
                    <ContentTemplate>
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc3:HomeTabs runat="server" PageCaller="NOTIFICATION_CENTER" ID="HomeTabs"></uc3:HomeTabs>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="containerNotificationCenter">
                <div id="containerNotificationCenterSx">
                    <asp:UpdatePanel runat="server" ID="UpdatePanelFilterSx" UpdateMode="Conditional"
                        ClientIDMode="Static">
                        <ContentTemplate>
                            <asp:UpdatePanel runat="server" ID="UpdatePanelNumberNotifyInTheRole" UpdateMode="Conditional"
                                ClientIDMode="Static">
                                <ContentTemplate>
                                    <div class="col14">
                                        <asp:Label ID="notificationCenterLblNotifyInTheRole" runat="server"></asp:Label>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel runat="server" ID="UpdatePanelNumberNotifyOtherRoles" UpdateMode="Conditional"
                                ClientIDMode="Static">
                                <ContentTemplate>
                                    <div id="containerNotificationCenterLblNotifyOtherRole" class="col14" runat="server">
                                        <asp:Label ID="notificationCenterLblNotifyOtherRole" runat="server"></asp:Label>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <fieldset>
                                <div class="weight">
                                    <div style="float: left; text-align: left; margin-right: 7px;">
                                        <asp:CheckBox ID="IndexChkRead" CssClass="clickableLeftN" Checked="true" runat="server"
                                            AutoPostBack="true" OnCheckedChanged="IndexChkRead_CheckedChanged" />
                                    </div>
                                    <div class="col13">
                                        <asp:CheckBox ID="IndexChkNotRead" CssClass="clickableLeftN" Checked="true" runat="server"
                                            AutoPostBack="true" OnCheckedChanged="IndexChkRead_CheckedChanged" />
                                    </div>
                                </div>
                            </fieldset>
                            <fieldset>
                                <div class="weight">
                                    <div class="col13">
                                        <asp:CheckBox ID="IndexChkDoc" CssClass="clickableLeftN" Checked="true" runat="server"
                                            AutoPostBack="true" OnCheckedChanged="IndexCkbFilterObject_CheckedChanged" />
                                    </div>
                                    <div class="col13">
                                        <asp:CheckBox ID="IndexChkProj" CssClass="clickableLeftN" Checked="true" runat="server"
                                            AutoPostBack="true" OnCheckedChanged="IndexCkbFilterObject_CheckedChanged" />
                                    </div>
                                    <div class="col13">
                                        <asp:CheckBox ID="IndexChkOther" CssClass="clickableLeftN" Checked="true" runat="server"
                                            AutoPostBack="true" OnCheckedChanged="IndexCkbFilterObject_CheckedChanged" />
                                    </div>
                                </div>
                            </fieldset>
                            <asp:UpdatePanel runat="server" ID="UpdatePanelFilterOperational" UpdateMode="Conditional"
                                ClientIDMode="Static">
                                <ContentTemplate>
                                    <div id="containerOperationalNotify" runat="server">
                                        <fieldset>
                                            <div class="weight">
                                                <asp:CheckBox ID="IndexChkOperational" CssClass="clickableLeftN" Checked="true" runat="server"
                                                    AutoPostBack="true" OnCheckedChanged="IndexChkOperational_CheckedChanged" />
                                            </div>
                                            <div class="col14">
                                                <asp:CheckBoxList ID="IndexCblOperational" runat="server" AutoPostBack="true" OnSelectedIndexChanged="IndexCblOperational_SelectedIndexChanged">
                                                </asp:CheckBoxList>
                                            </div>
                                        </fieldset>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel runat="server" ID="UpdatePanelFilterInformation" UpdateMode="Conditional"
                                ClientIDMode="Static">
                                <ContentTemplate>
                                    <div id="containerInformationNotify" runat="server">
                                        <fieldset>
                                            <div class="weight">
                                                <asp:CheckBox ID="IndexChkInformation" CssClass="clickableLeftN" Checked="true" AutoPostBack="true"
                                                    runat="server" OnCheckedChanged="IndexChkInformation_CheckedChanged" />
                                            </div>
                                            <div class="col14">
                                                <asp:CheckBoxList ID="IndexCblInformation" runat="server" AutoPostBack="true" OnSelectedIndexChanged="IndexCblInformation_SelectedIndexChanged">
                                                </asp:CheckBoxList>
                                            </div>
                                        </fieldset>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div id="containerNotificationCenterDx">
                    <div id="containerNotificationFilter">
                        <asp:UpdatePanel runat="server" ID="UpPnlBAction" UpdateMode="Conditional" ClientIDMode="static">
                            <ContentTemplate>
                                <div class="col11" style="padding-left: 10px">
                                    <asp:CheckBox ID="cbSelectAllNotify" runat="server" CssClass="clickableLeft" OnCheckedChanged="SelectAllNotify_CheckedChanged"
                                        AutoPostBack="true" />
                                    <cc1:CustomImageButton runat="server" ID="IndexImgRefresh" ImageUrl="Images/Icons/home_refresh.png"
                                        OnMouseOutImage="Images/Icons/home_refresh.png" OnMouseOverImage="Images/Icons/home_refresh_hover.png"
                                        CssClass="clickable" ImageUrlDisabled="Images/Icons/home_refresh_disabled.png"
                                        OnClick="IndexImgRefresh_OnClick" />
                                    <cc1:CustomImageButton runat="server" ID="IndexImgAddFilter" ImageUrl="Images/Icons/home_add_filters.png"
                                        OnMouseOutImage="Images/Icons/home_add_filters.png" OnMouseOverImage="Images/Icons/home_add_filters_hover.png"
                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/home_add_filters_disabled.png"
                                        OnClientClick="return ajaxModalPopupAddFilterNotificationCenter();" />
                                    <cc1:CustomImageButton runat="server" ID="IndexImgRemoveFilter" ImageUrl="Images/Icons/home_delete_filters.png"
                                        OnMouseOutImage="Images/Icons/home_delete_filters.png" OnMouseOverImage="Images/Icons/home_delete_filters_hover.png"
                                        CssClass="clickable" ImageUrlDisabled="Images/Icons/home_delete_filters_disabled.png"
                                        OnClick="IndexImgRemoveFilter_OnClick" Enabled="false" />
                                    <cc1:CustomImageButton runat="server" ID="IndexImgDelete" ImageUrl="Images/Icons/home_delete.png"
                                        OnMouseOutImage="Images/Icons/home_delete.png" OnMouseOverImage="Images/Icons/home_delete_hover.png"
                                        CssClass="clickable" ImageUrlDisabled="Images/Icons/home_delete_disabled.png"
                                        OnClick="IndexImgRemove_OnClick" />
                                    <cc1:CustomImageButton runat="server" ID="IndexImgExport" ImageUrl="Images/Icons/home_export.png"
                                        OnMouseOutImage="Images/Icons/home_export.png" OnMouseOverImage="Images/Icons/home_export_hover.png"
                                        CssClass="clickable" ImageUrlDisabled="Images/Icons/home_export_disabled.png"
                                        OnClientClick="return ajaxModalPopupExportDati();" />
                                    <cc1:CustomImageButton runat="server" ID="IndexImgSmista" ImageUrl="Images/Icons/home_smista.png"
                                        OnMouseOutImage="Images/Icons/home_smista.png" OnMouseOverImage="Images/Icons/home_smista_hover.png"
                                        CssClass="clickable" ImageUrlDisabled="Images/Icons/home_smista_disabled.png"
                                        OnClick="IndexImgSmista_Click" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col10">
                        <asp:UpdatePanel ID="UpdatePanelDdlOrderBy" runat="server" UpdateMode="Conditional"
                            ClientIDMode="Static">
                            <ContentTemplate>
                                <asp:DropDownList ID="ddlOrderBy" runat="server" OnSelectedIndexChanged="DdlOrderBy_SelectedIndexChange"
                                    CssClass="chzn-select-deselect" AutoPostBack="true" Width="300px">
                                    <asp:ListItem id="ddlOrderByDateNotifyDescending" Value="1"></asp:ListItem>
                                    <asp:ListItem id="ddlOrderByDateNotifyAscending" Value="2"></asp:ListItem>
                                    <asp:ListItem id="ddlOrderByTypeEvent" Value="3"></asp:ListItem>
                                    <asp:ListItem id="ddlOrderByProduttore" Value="4"></asp:ListItem>
                                    <asp:ListItem id="ddlOrderByIdObject" Value="5"></asp:ListItem>
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanelExpandAll" runat="server" UpdateMode="Conditional"
                        ClientIDMode="Static">
                        <ContentTemplate>
                            <div class="linkTree" style="padding-top: 10px">
                                <asp:LinkButton ID="litTreeExpandAll" runat="server" OnClick="ExpandAll_Click"></asp:LinkButton>
                                <asp:LinkButton ID="litTreeCollapseAll" runat="server" OnClick="CollapseAll_Click"></asp:LinkButton>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div id="containerListNotify" style="padding-left: 5px;">
                        <asp:UpdatePanel ID="UpdatePanelRepListNotify" runat="server" UpdateMode="Conditional"
                            ClientIDMode="Static">
                            <ContentTemplate>
                                <asp:Repeater ID="repListNotify" runat="server" OnItemCommand="RepListNotify_ItemCommand"
                                    OnItemDataBound="RepListNotify_ItemCreated">
                                    <ItemTemplate>
                                        <div class="boxNotifyHome" runat="server">
                                            <div id="containerListNotifyTop" runat="server" class="containerListNotifyTop">
                                                <div id="boxNotifyHomeSx" runat="server">
                                                    <div class="col2" style="padding-left: 20px; padding-top: 5px;">
                                                        <asp:CheckBox ID="IndexChkRemoveNotify" CssClass="clickableLeftN" runat="server" />
                                                    </div>
                                                    <div class="col2" style="padding-left: 5px; padding-top: 5px;">
                                                        <cc1:CustomImageButton ID="IndexImgDetailsDocument" CommandName="ViewDetailsDocument"
                                                            runat="server" ImageUrl="Images/Icons/search_response_documents.png" OnMouseOutImage="Images/Icons/search_response_documents.png"
                                                            OnMouseOverImage="Images/Icons/search_response_documents_hover.png" CssClass="clickable"
                                                            ImageUrlDisabled="Images/Icons/search_response_documents_disabled.png" Height="22px" />
                                                    </div>
                                                </div>
                                                <div id="boxNotifyHomeDx" runat="server">
                                                    <div id="labelContainerTop" runat="server">
                                                        <div class="col12" style="width: 50%">
                                                            <div id="nameNotify" class="nameNotify" runat="server">
                                                                <asp:Label ID="descProducerTrasmLabel" Text='<%#this.GetHeader((NttDataWA.DocsPaWR.Notification) Container.DataItem) %>'
                                                                    CssClass="clickable" ToolTip='<%# Bind("PRODUCER") %>' runat="server" />
                                                            </div>
                                                        </div>
                                                        <div class="col9">
                                                            <img src="Images/Icons/home_time_ico.png" alt="" />
                                                            <asp:Label ID="IndexLblDate" runat="server" Text='<%# Bind("DTA_EVENT") %>'></asp:Label>
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <cc1:CustomImageButton ID="IndexImgRemoveNotify" CommandName="RemoveNotify" runat="server"
                                                                ImageUrl="Images/Icons/delete.png" OnMouseOutImage="Images/Icons/delete.png"
                                                                OnMouseOverImage="Images/Icons/delete_hover.png" CssClass="clickableLeft" ImageUrlDisabled="Images/Icons/delete_disabled.png" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="boxNotifyHome">
                                            <div id="containerListNotifyBt" class="containerListNotifyBt" runat="server">
                                                <div class="col2" style="padding-left: 20px; padding-top: 6px;">
                                                    <cc1:CustomImageButton ID="ExpandCollapse" CommandName="ViewDetailsDocument" runat="server"
                                                        ImageUrl="Images/Icons/collapsed.png" OnMouseOutImage="Images/Icons/collapsed.png"
                                                        OnMouseOverImage="Images/Icons/collapsed_hover.png" CssClass="clickable" ImageUrlDisabled="Images/Icons/collapsed_disabled.png" />
                                                </div>
                                                <div class="notifyField">
                                                   <span class="noLink"><asp:LinkButton  ID="lblNotifyLink" runat="server" Text='<%#this.getLabelFieldLink((NttDataWA.DocsPaWR.Notification) Container.DataItem) %>' CommandName="viewLinkObject" OnClientClick="disallowOpHome('Content2');"></asp:LinkButton></span>
                                                    <asp:Label ID="lblNotifyField" runat="server" Text='<%#this.getLabelField((NttDataWA.DocsPaWR.Notification) Container.DataItem) %>'></asp:Label>
                                                </div>
                                                <div class="col9" style="float: right">
                                                    <cc1:CustomImageButton ID="btnTypeDoc" CommandName="ViewDocument" runat="server"
                                                        CssClass="clickableLeft" Height="22px" ToolTip='<%$ localizeByText:IndexBtnTypeDocTooltip%>' />
                                                    <cc1:CustomImageButton ID="btnSignatureDetails" CommandName="SignatureDetails" runat="server" ImageUrl="Images/Icons/icon_p7m.png"
                                                        OnMouseOutImage="Images/Icons/icon_p7m.png" OnMouseOverImage="Images/Icons/icon_p7m_hover.png"
                                                        CssClass="clickable" ImageUrlDisabled="Images/Icons/icon_p7m_disabled.png" Height="22px"
                                                        ToolTip='<%$ localizeByText:DocumentSignatureDetails%>' Visible="false" />
                                                    <cc1:CustomImageButton ID="IndexImgDetailsNotify" CommandName="ViewDetailsNotify"
                                                        runat="server" ImageUrl="Images/Icons/dettaglio-notifica-icona2.png" OnMouseOutImage="Images/Icons/dettaglio-notifica-icona2.png"
                                                        OnMouseOverImage="Images/Icons/dettaglio-notifica-icona2_hover.png" CssClass="clickableLeft"
                                                        ImageUrlDisabled="Images/Icons/dettaglio-notifica-icona2_disabled.png" Height="22px"
                                                        ToolTip='<%$ localizeByText:IndexImgDetailsNotifyTooltip%>' />
                                                    <cc1:CustomImageButton ID="IndexImgAdd" CommandName="AddNote" runat="server" Height="22px"
                                                        ImageUrl="Images/Icons/home_add_note.png" OnMouseOutImage="Images/Icons/home_add_note.png"
                                                        OnMouseOverImage="Images/Icons/home_add_note_hover.png" CssClass="clickableLeft" Visible='<%#this.GetEnableNote() %>' />
                                                </div>
                                            </div>
                                        </div>
                                        <div id="containerNotifyDetails" runat="server">
                                            <div id="containerNotifyDetail" style="float: left; width: 100%; clear: both;">
                                                <div id="containerNoteHomeSx" runat="server">
                                                    <div style="float: left; padding-left: 10px; padding-bottom: 10px; padding-top: 10px;padding-right:10px;
                                                        ">
                                                        <asp:Label ID="specializedFieldLabel" runat="server" Text='<%#this.GetLabelSpecializedField((NttDataWA.DocsPaWR.Notification) Container.DataItem) %>'
                                                            CssClass="nameHomeNot"></asp:Label>
                                                    </div>
                                                </div>
                                                <div id="containerNoteNotify" runat="server">
                                                    <div class="fieldNotesHome">
                                                        <div class="fieldNotesHomeBt">
                                                            <div class="weight">
                                                                <asp:Label ID="lblNotes" Text='<%$ localizeByText:IndexLblNotes%>' runat="server"
                                                                    Visible="false"></asp:Label>
                                                            </div>
                                                            <div style="padding-top: 30px; padding-left: 55px;">
                                                                <cc1:CustomTextArea ID="txtNoteNotify" runat="server" TextMode="MultiLine" ClientIDMode="Static"
                                                                    CssClass="homeNotes">
                                                                </cc1:CustomTextArea>
                                                            </div>
                                                            <div style="text-align: right;">
                                                                <asp:Button ID="litSaveNotes" Text='<%$ localizeByText:IndexLitSaveNotes%>' CommandName="SaveNotesNotify"
                                                                    runat="server" CssClass="buttonNotes" ImageAlign="Right"></asp:Button>
                                                                <asp:Button ID="litRemoveNotes" Text='<%$ localizeByText:IndexLitRemoveNotes%>' CommandName="RemoveNotesNotify"
                                                                    runat="server" CssClass="buttonNotes" ImageAlign="Right"></asp:Button>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <asp:HiddenField runat="server" ID="NotifyId" Value='<%# Bind("ID_NOTIFY") %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="HiddenRemoveNotifications" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="HiddenRemoveNotification" runat="server" ClientIDMode="Static" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <asp:HiddenField ID="grid_pageindex" runat="server" ClientIDMode="Static" />
                    <asp:UpdatePanel ID="upPnlGridIndexes" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="navHome">
                                <asp:PlaceHolder ID="plcNavigator" runat="server" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:HiddenField ID="mandate_exercise" runat="server" ClientIDMode="Static" />
            <script type="text/javascript">
                $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
                $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
            </script>
            <asp:Literal ID="debug" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
