<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="AddNewProcess.aspx.cs" Inherits="NttDataWA.Popup.AddNewProcess" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            position: fixed;
            top: 1px;
            left: 0px;
            bottom: 71px;
            right: 0px;
            overflow: auto;
            background: #ffffff;
            text-align: left;
            padding: 10px;
        }
        .txt_textarea
        {
            width: 100%;
            border: 1px solid #cccccc;
            line-height: 18px;
            font-family: Verdana, Arial, Verdana, sans-serif;
            font-size: 13px;
            height: 50px;
            overflow: auto;
            border-radius: 5px;
            -ms-border-radius: 5px; /* ie */
            -moz-border-radius: 5px; /* firefox */
            -webkit-border-radius: 5px; /* safari, chrome */
            -o-border-radius: 5px; /* opera */
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <p id="rowDescription" runat="server">
            <span class="weight">
                <asp:Literal ID="ProcessName" runat="server"></asp:Literal></span><br />
            <cc1:CustomTextArea ID="txt_processName" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled"
                ClientIDMode="Static" TextMode="MultiLine">
            </cc1:CustomTextArea>
            <span class="col-right">
                <asp:Literal ID="VersionsLitChars" runat="server"></asp:Literal>
                <span id="VersionDescription_chars"></span></span>
        </p>
        <asp:CheckBox id="cbxCopiaVisibilita" runat="server"/>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="AddNewProcessSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddNewProcessSave_Click"
        OnClientClick="disallowOp('Content1')" />
    <cc1:CustomButton ID="AddNewProcessClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content1')"
        OnClick="AddNewProcessClose_Click" />
</asp:Content>
