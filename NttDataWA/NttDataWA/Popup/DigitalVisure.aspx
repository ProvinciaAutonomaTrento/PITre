<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DigitalVisure.aspx.cs"
    Inherits="NttDataWA.Popup.DigitalVisure" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            padding: 10px;
            text-align: left;
            line-height: 2em;
            width: 95%;
            height: 80%;
            top: 20%;
        }
        .title
        {
            color: #521110;
            font-size: medium;
        }
        .description
        {
            color: #521110;
            font-size: small;
        }
        .definition
        {
            color: #151052;
            font-size: small;
        }
        .divisor
        {
            border-bottom-style: solid;
            border-bottom-width: thin;
            border-bottom-color: Silver;
        }
    </style>
    <script type="text/javascript">
        function confirmAction() {
            var retval = true;
            //reallowOp();

            return retval;
        }
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <asp:UpdatePanel ID="upPnlInfoDigitalVisure" runat="server">
        <ContentTemplate>
            <div class="container">
                <div class="divisor">
                    <span class="weight">
                        <asp:Label ID="lblMessage" runat="server" class="title"></asp:Label>
                    </span>
                </div>
                <div class="row">
                    <asp:Label ID="lblMessageDetails" runat="server"></asp:Label>
                </div>
<%--                <div class="divisor">
                    <asp:Label ID="lblTitOggetto" runat="server" class="description"></asp:Label>
                    <asp:Label ID="lblOggetto" runat="server" class="definition"></asp:Label>
                </div>
                <div class="divisor">
                    <asp:Label ID="lblTitNomeFile" runat="server" class="description"></asp:Label>
                    <asp:Label ID="lblNomeFile" runat="server" class="definition"></asp:Label>
                </div>--%>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="cpnlOldersButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel runat="server" ID="UpUpdateButtons">
        <ContentTemplate>
            <cc1:CustomButton ID="ConfirmButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="ConfirmButton_Click" />
            <cc1:CustomButton ID="UndoButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="UndoButton_Click" />
            <asp:HiddenField ID="HiddenDigitalVisure" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
