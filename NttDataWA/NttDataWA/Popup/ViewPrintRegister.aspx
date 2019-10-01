<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewPrintRegister.aspx.cs" Inherits="NttDataWA.Popup.ViewPrintRegister" MasterPageFile="~/MasterPages/Popup.Master"%>
<%@ Register src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="contentViewPrintRegister" align="center">
        <iframe width="100%"  src="../Document/AttachmentViewer.aspx?printRegister=t" frameborder="0" marginheight="0" marginwidth="0" id="frame"
         runat="server" clientidmode="Static" style="z-index: 0;"></iframe>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnViewPrintRegisterClose" OnClick="BtnViewPrintRegisterClose_Click"  runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" />
</asp:Content>
