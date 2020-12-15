<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="HSM_Signature.aspx.cs" Inherits="NttDataWA.Popup.HSM_Signature" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function() {
            $('.HsmCenter input, .HsmCenter textarea').keypress(function (e) {
            if(e.which == 13) {
                e.preventDefault();
                $('#BtnSign').click();
            }
        });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <div class="HsmCenter">
            <div class="HsmCenterSx">
                <p style="margin-left: 20px; font-weight: bold;">
                    <asp:Label ID="HsmLitAlias" runat="server"></asp:Label></p>
                <p style="margin-left: 20px;">
                    <cc1:CustomTextArea runat="server" ID="TxtHsmAlias" CssClass="txt_hsmtxt"></cc1:CustomTextArea></p>
            </div>
            <div class="HsmCenterDx">
                <p style="margin-left: 20px; font-weight: bold;">
                    <asp:Label ID="HsmLitDomain" runat="server"></asp:Label></p>
                <p style="margin-left: 20px;">
                    <cc1:CustomTextArea runat="server" ID="TxtHsmDomain" CssClass="txt_hsmtxt"></cc1:CustomTextArea></p>
            </div>
            <div class="HsmCenterSx">
               <p style="margin-left: 20px; font-weight: bold;">
                    <asp:Label ID="HsmLitPin" runat="server"></asp:Label></p>
                <p style="margin-left: 20px;">
                    <cc1:CustomTextArea runat="server" ID="TxtHsmPin" CssClass="txt_hsmtxt" TextMode="Password"></cc1:CustomTextArea></p>
            </div>
            <div class="HsmCenterDx">
                <p style="margin-left: 20px; font-weight: bold;">
                    <asp:Label ID="HsmLitOtp" runat="server"></asp:Label></p>
                <p style="margin-left: 20px;">
                    <cc1:CustomTextArea runat="server" ID="TxtHsmLitOtp" CssClass="txt_hsmtxt" TextMode="Password"></cc1:CustomTextArea></p>
            </div>
            <br />
            <div class="HsmCenterBottom">
                <p style="margin-left: 20px; margin-top: 10px;">
                    <asp:RadioButton ID="optFirma" GroupName="grpTipoFirma" runat="server" ClientIDMode="Static"></asp:RadioButton>
                    <asp:RadioButton ID="optCofirma" GroupName="grpTipoFirma" runat="server" ClientIDMode="Static"></asp:RadioButton>
                </p>
            </div>
            <asp:UpdatePanel runat="server" ID="UpPnlSign" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="HsmCenterBottom">
                        <p style="margin-left: 20px; margin-top: 10px;">
                            <asp:RadioButton GroupName="choose" runat="server" ID="HsmLitPades" TextAlign="Right"
                                OnCheckedChanged="HsmLitPades_Change" AutoPostBack="true" onchange="disallowOp('Content2');" />
                            <asp:RadioButton GroupName="choose" runat="server" ID="HsmLitP7M" TextAlign="Right"
                                Checked="true" OnCheckedChanged="HsmLitP7M_Change" AutoPostBack="true" onchange="disallowOp('Content2');" />
                        </p>
                        <p style="margin-left: 20px; margin-top: 10px;">
                            <asp:CheckBox runat="server" ID="HsmCheckMarkTemporal" TextAlign="Right" /></p>
                        
                        <p style="margin-left: 20px; margin-top: 5px;">
                            <asp:CheckBox runat="server" ID="HsmCheckConvert" TextAlign="Right" /></p>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnRequestOTP" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" Visible = "false"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnRequestOTP_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnSign" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnSign_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
