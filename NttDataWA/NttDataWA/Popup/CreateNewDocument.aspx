<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="CreateNewDocument.aspx.cs" Inherits="NttDataWA.Popup.CreateNewDocument" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .txt_addressBookLeft
        {
            line-height: 18px;
            font-family: Verdana, Arial, Verdana, sans-serif;
            font-size: 13px;
            height: 18px;
            color: #333333;
            border-radius: 5px;
            -ms-border-radius: 5px; /* ie */
            -moz-border-radius: 5px; /* firefox */
            -webkit-border-radius: 5px; /* safari, chrome */
            -o-border-radius: 5px; /* opera */
            width: 100%;
            background-color: #ffffff;
        }
        .txt_project
        {
            line-height: 18px;
            font-family: Verdana, Arial, Verdana, sans-serif;
            font-size: 13px;
            height: 18px;
            width: 100%;
        }
        
        .txt_textarea
        {
            /* width: 100%; */
            width: 100px;
            min-width: 100%;
            max-width: 100%;
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
        .col-cod-object
        {
            float: left;
            text-align: left;
            width: 18%;
        }
        .col-text-object
        {
            float: right;
            margin: 0px;
            text-align: right;
            width: 80%;
        }
        .content
        {
            border-top: 1px solid #2e82bc;
            border-bottom: 1px solid #2e82bc;
            border-left: 1px solid #2e82bc;
            border-right: 1px solid #2e82bc;
            background-color: #edf4f8;
            width: 100%;
            height: 100%;
        }
        .container
        {
            padding: 10px;
            overflow: hidden;
        }
        .row1
        {
            clear: both;
            min-height: 10px;
            margin: 0 0 1px 0;
            text-align: left;
            vertical-align: top;
        }
    </style>
    <script type="text/javascript">
        function closeObjectPopup() {
            $('#btnObjectPostback').click();
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div class="content">
        <div class="container">
            <asp:Panel runat="server" ID="PnlObject">
                <div class="row">
                    <div class="col">
                        <p>
                            <span class="weight">
                                <asp:Literal ID="DocumentLitObject" runat="server"></asp:Literal></span><span class="little">*</span></p>
                    </div>
                    <div class="col-right-no-margin">
                        <asp:UpdatePanel ID="UpPnlIconObjects" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cc1:CustomImageButton runat="server" ID="DocumentImgObjectary" ImageUrl="../Images/Icons/obj_objects.png"
                                    OnMouseOutImage="../Images/Icons/obj_objects.png" OnMouseOverImage="../Images/Icons/obj_objects_hover.png"
                                    CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/obj_objects_disabled.png"
                                    OnClientClick="return parent.ajaxModalPopupObject();" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <asp:UpdatePanel ID="UpdPnlObject" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                    <ContentTemplate>
                        <div class="row">
                            <div class="colHalf">
                                <asp:Panel ID="PnlCodeObject" runat="server" Visible="true">
                                    <cc1:CustomTextArea ID="TxtCodeObject" runat="server" CssClass="txt_addressBookLeft"
                                        autocomplete="off" CssClassReadOnly="txt_addressBookLeft_disabled" AutoPostBack="true"
                                        OnTextChanged="TxtCodeObject_Click" onchange="disallowOp('Content2')">
                                    </cc1:CustomTextArea>
                                </asp:Panel>
                            </div>
                            <div class="colHalf2">
                                <div class="colHalf3">
                                    <asp:Panel ID="PnlCodeObject2" runat="server">
                                        <cc1:CustomTextArea ID="TxtObject" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                                            CssClassReadOnly="txt_textarea_disabled" ClientIDMode="Static">
                                        </cc1:CustomTextArea>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-right-no-margin">
                                <span class="charactersAvailable">
                                    <asp:Literal ID="DocumentLitObjectChAv" runat="server"></asp:Literal>
                                    <span id="TxtObject_chars"></span></span>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlPjct">
                <div class="row">
                    <div class="col">
                        <p>
                            <span class="weight">
                                <asp:Literal runat="server" ID="CreateNewDocumentProject"></asp:Literal></span><asp:Label
                                    ID="LblClassRequired" CssClass="little" runat="server"></asp:Label></p>
                    </div>
                </div>
                <div style="margin-left: 10px">
                    <asp:UpdatePanel ID="UpPnlProject" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:PlaceHolder runat="server" ID="PnlProject">
                                <asp:HiddenField ID="IdProject" runat="server" />
                                <div class="row">
                                    <span class="weight">
                                        <asp:Literal runat="server" ID="CreateNewDocumentCodeProject"></asp:Literal></span><asp:Label
                                            ID="Label1" CssClass="little" runat="server"></asp:Label>
                                    <div class="row">
                                        <asp:Label ID="TxtCodeProject" runat="server" CssClass="txt_project" AutoComplete="off"
                                            CssClassReadOnly="txt_project">
                                        </asp:Label>
                                    </div>
                                </div>
                                </div>
                                <div class="row">
                                    <span class="weight">
                                        <asp:Literal runat="server" ID="CreateNewDocumentDescriptionProject"></asp:Literal></span><asp:Label
                                            ID="Label2" CssClass="little" runat="server"></asp:Label>
                                    <div class="row">
                                        <asp:Label ID="TxtDescriptionProject" runat="server" TextMode="MultiLine" CssClass="txt_project"
                                            CssClassReadOnly="txt_project" ClientIDMode="Static">
                                        </asp:Label>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpButton" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="CreateNewDocumentBtnSave" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="CreateNewDocumentBtnSave_Click"
                OnClientClick="disallowOp('Content2')" />
            <cc1:CustomButton ID="CreateNewDocumentBtnCancel" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="CreateNewDocumentBtnCancel_Click"
                OnClientClick="disallowOp('Content2')" />
            <asp:Button ID="btnObjectPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnObjectPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
