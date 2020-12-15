<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="AddressBook_import.aspx.cs" Inherits="NttDataWA.Popup.AddressBook_import" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
        .message {text-align: left; line-height: 2em; width: 90%;/* min-height: 200px; margin: 150px auto 0 auto;*/}
        .message img {float: left; display: block; margin: 0 20px 20px 0;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
<%--    <asp:UpdatePanel ID="UpPnlMessage" runat="server" UpdateMode="Conditional">
        <ContentTemplate>--%>
            <asp:PlaceHolder ID="plcMessage" runat="server" Visible="false">
                <div class="message">
                    <asp:Literal ID="litMessage" runat="server" />
                </div>
            </asp:PlaceHolder>
<%--        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="UpPnlFile" runat="server" UpdateMode="Conditional">
        <ContentTemplate>--%>
            <asp:PlaceHolder ID="plcFile" runat="server">
                <div class="row">
                    <div class="col">
                        <asp:Literal ID="litFile" runat="server" />
                    </div>
                    <div class="col">
                        <input type="file" id="uploadFile" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
<%--        </ContentTemplate>
    </asp:UpdatePanel>--%>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
<%--    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>--%>
            <cc1:CustomButton ID="BtnConfirm" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnConfirm_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnLog" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnLog_Click" OnClientClick="disallowOp('Content2');" Visible="false" />
<%--        </ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
