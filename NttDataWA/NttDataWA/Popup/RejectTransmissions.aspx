<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RejectTransmissions.aspx.cs"
    Inherits="NttDataWA.Popup.RejectTransmissions" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <style type="text/css">
        .container
        {
            margin-left: 5px;
        }
        .container img
        {
            padding-left: 2px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <div class="row">
            <div class="col">
                <span class="weight">
                    <asp:Literal ID="SmistalblRejectTransmNote" runat="server" />
                </span>
            </div>
        </div>
        <div class="row">
            <div class="col-full">
                <cc1:CustomTextArea ID="txtRejectTransm" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                    CssClassReadOnly="txt_textarea_disabled" ClientIDMode="Static" Width="98%">
                </cc1:CustomTextArea>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" runat="server">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnRejectSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnRejectSave_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnRejectClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnRejectClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
