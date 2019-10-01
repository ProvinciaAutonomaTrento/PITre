<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="MassiveReport_iframe.aspx.cs" Inherits="NttDataWA.Popup.MassiveReport_iframe" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
        p {text-align: center; margin: 20% auto 0 auto;}
    </style>
    <script type="text/javascript">
        function resizePrintIframe() {
            var height = document.documentElement.clientHeight;
            height -= 90; /* whatever you set your body bottom margin/padding to be */
            document.getElementById('frame').style.height = height + "px";
        };

        $(function () {
            resizePrintIframe();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <div class="row">
        <iframe src="MassiveReport.aspx" id="frame" width="100%" frameborder="0"></iframe>
    </div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static"  onclick="BtnClose_Click" OnClientClick="disallowOp('Content2'); $('iframe').hide();" />
</asp:Content>
