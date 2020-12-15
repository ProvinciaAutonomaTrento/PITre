<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="VerifyPrevious.aspx.cs" Inherits="NttDataWA.Popup.VerifyPrevious" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        p.title {font-weight: bold; text-align: center;}
        strong.red {color: #f00;}
        .row {text-align: center; margin: 0 0 20px 0;}
        .col {text-align: right;}
        .big {font-size: 1.5em; margin: 20px 0;}
        .col-II {display: inline; padding: 0 0 0 30px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <uc:messager id="messager" runat="server" />
    <div class="row big">
        <asp:Literal ID="litNumProt" runat="server" />:
        <strong><asp:Literal ID="lblNumProtocollo" runat="server" /></strong>
    </div>
    <div class="row">
        <cc1:CustomImageButton runat="server" ID="btnVisibility" ImageUrl="../Images/Icons/ico_visibility.png"
            OnMouseOutImage="../Images/Icons/ico_visibility.png" OnMouseOverImage="../Images/Icons/ico_visibility_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/ico_visibility_disabled.png" onclick="btnVisibility_Click" />
        &nbsp;
        <cc1:CustomImageButton runat="server" ID="btn_VisDoc" ImageUrl="../Images/Icons/ico_previous_details.png"
            OnMouseOutImage="../Images/Icons/ico_previous_details.png" OnMouseOverImage="../Images/Icons/ico_previous_details_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/ico_previous_details_disabled.png" OnClick="btn_VisDoc_Click" />
    </div>
    <div class="row">
        <asp:Literal ID="litDate" runat="server" />:
        <asp:Literal ID="lblData" runat="server" />
        <div class="col-II">
            <asp:Literal ID="litID" runat="server" />:
            <asp:Literal ID="lblIdDocumento" runat="server" />
        </div>
    </div>
    <div class="row">
        <asp:Literal ID="litSegnature" runat="server" />:
        <strong class="red"><asp:Literal ID="txtSegnatura" runat="server" /></strong>
    </div>
    <div class="row">
        <asp:Literal ID="litOffice" runat="server" />:
        <asp:Literal ID="txtOggetto" runat="server" />
    </div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" onclick="BtnOk_Click" />
    <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" onclick="BtnClose_Click" />
</asp:Content>
