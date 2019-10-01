<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectNextMessage.aspx.cs"
    Inherits="NttDataWA.Popup.SelectNextMessage" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #container
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
    </style>
    >
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <%--        <div class="row">
            <p>
                <asp:Label ID="LblSelectMessage" runat="server"></asp:Label></p>
        </div>--%>
        <uc:messager ID="messager" runat="server" />
        <div class="row">
            <%--<asp:DropDownList ID="DdlSelectMessage" runat="server" CssClass="chzn-select-deselect" Width="60%">
            </asp:DropDownList>--%>
            <asp:RadioButtonList ID="RblSelectMessage" runat="server" RepeatDirection="Vertical"></asp:RadioButtonList>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="SelectNextMessageOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="SelectNextMessageOk_Click" />
            <cc1:CustomButton ID="SelectNextMessageClose" runat="server" CssClass="btnEnable"
                OnClientClick="disallowOp('Content2');" CssClassDisabled="btnDisable" OnMouseOver="btnHover"
                ClientIDMode="Static" OnClick="SelectNextMessageClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
