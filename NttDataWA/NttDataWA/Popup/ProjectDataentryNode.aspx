<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="ProjectDataentryNode.aspx.cs" Inherits="NttDataWA.Popup.ProjectDataentryNode" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#FolderDescription").focus();

            $('.defaultAction').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('#BtnSave').click();
                }
            });
        });
    </script>
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <div id="rowMessage" runat="server" style="color: #f00;" />
        <p id="rowCode" runat="server">
            <span class="weight">
                <asp:Literal ID="lbl_description" runat="server" />
            </span>*</p>
        <p id="rowDescription" runat="server">
            <cc1:CustomTextArea ID="FolderDescription" runat="server" TextMode="MultiLine" ClientIDMode="Static"
                CssClass="txt_textarea defaultAction" Columns="50" Rows="4"></cc1:CustomTextArea></p>
        </p>
        <div class="row">
            <div class="col-right-no-margin">
                <span class="charactersAvailable">
                    <asp:Literal ID="FolderDescriptionChars" runat="server" ClientIDMode="static"></asp:Literal>
                    <span id="FolderDescription_chars" runat="server" clientidmode="static"></span></span>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2')" OnClick="BtnSave_Click" />
    <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" Text="Chiudi" ClientIDMode="Static" OnClick="BtnClose_Click" />
</asp:Content>
