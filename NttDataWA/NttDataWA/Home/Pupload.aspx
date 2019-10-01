<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pupload.aspx.cs" Inherits="NttDataWA.Home.Pupload"
    MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Src="../UserControls/HomeTabs.ascx" TagPrefix="uc1" TagName="HomeTabs" %>
<%@ Register Src="../UserControls/HeaderHome.ascx" TagPrefix="uc2" TagName="HeaderHome" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
<script src="../Scripts/resumable.js" type="text/javascript"></script>
<script type="text/javascript">
    var r = new Resumable({
        target: 'test.html'
    });

    r.assignBrowse(document.getElementById('browseButton'));

    r.on('fileSuccess', function (file) {
        console.debug('fileSuccess', file);
    });
    r.on('fileProgress', function (file) {
        console.debug('fileProgress', file);
    });
    r.on('fileAdded', function (file, event) {
        r.upload();
        console.debug('fileAdded', event);
    });
    r.on('filesAdded', function (array) {
        r.upload();
        console.debug('filesAdded', array);
    });
    r.on('fileRetry', function (file) {
        console.debug(file);
    });
    r.on('fileError', function (file, message) {
        console.debug('fileError', file, message);
    });
    r.on('uploadStart', function () {
        console.debug('uploadStart');
    });
    r.on('complete', function () {
        console.debug('complete');
    });
    r.on('progress', function () {
        console.debug('progress');
    });
    r.on('error', function (message, file) {
        console.debug('error', message, file);
    });
    r.on('pause', function () {
        console.debug('pause');
    });
    r.on('cancel', function () {
        console.debug('cancel');
    });
</script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div id="containerTop">
        <div id="containerTabHome">
            <asp:UpdatePanel runat="server" ID="UpHeaderHome" UpdateMode="Conditional" ClientIDMode="static">
                <ContentTemplate>
                    <div id="containerStandardTop">
                        <div id="containerStandardTopCxHome">
                            <img src="../Images/Common/griff.png" alt="" title="" />
                        </div>
                        <div id="containerHomeHeader">
                            <div id="containerHeaderHomeSx">
                                <strong>
                                    <asp:Label runat="server" ID="headerHomeLblRole"></asp:Label>
                                </strong>
                            </div>
                            <div id="containerHeaderHomeDx">
                                <div class="styled-select_full">
                                    <asp:DropDownList ID="ddlRolesUser" runat="server" CssClass="chzn-select-deselect"
                                        AutoPostBack="true" Width="700px" OnSelectedIndexChanged="ddlRolesUser_SelectedIndexChange">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
                ClientIDMode="Static">
                <ContentTemplate>
                    <div id="containerTabIndex">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc1:HomeTabs runat="server" PageCaller="PUPLOAD" ID="HomeTabs"></uc1:HomeTabs>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="containerInvioFile">
                
                <a href="#" id="browseButton">Select files</a>

            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
    </asp:UpdatePanel>
    
</asp:Content>
