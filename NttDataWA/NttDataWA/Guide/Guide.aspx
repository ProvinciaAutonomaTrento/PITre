<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Guide.aspx.cs" Inherits="NttDataWA.Guide.Guide"
    MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="divFrame" class="row" runat="server">
        <fieldset>
            <div class="row">
                <iframe width="100%" frameborder="0" marginheight="0" marginwidth="0" id="frame"
                    runat="server" clientidmode="Static" style="z-index: 0;" src="../Guide/guide.pdf#page=13"></iframe>
                <script type="text/javascript">
                    if ($.cookie('guide_cookie')=='ok')
                        $('iframe').remove();
                </script>
            </div>
        </fieldset>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="static">
        <ContentTemplate>
            <cc1:CustomButton ID="GuideBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="GuideBtnClose_Click" OnClientClick="disallowOp('Content2'); $('iframe').remove(); $.cookie('guide_cookie', 'ok', { expires: 7, path: '/' });" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
