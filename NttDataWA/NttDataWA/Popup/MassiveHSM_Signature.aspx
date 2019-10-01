<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="MassiveHSM_Signature.aspx.cs" Inherits="NttDataWA.Popup.MassiveHSM_Signature" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
            <asp:UpdatePanel runat="server" ID="UpOTP" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="HsmCenterDx">
                        <p style="margin-left: 20px; font-weight: bold;">
                            <asp:Label ID="HsmLitOtp" runat="server"></asp:Label></p>
                        <p style="margin-left: 20px;">
                            <cc1:CustomTextArea runat="server" ID="TxtHsmLitOtp" CssClass="txt_hsmtxt" TextMode="Password"></cc1:CustomTextArea></p>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpPnlSign" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="HsmCenterBottom">
                        <p style="margin-left: 20px; margin-top: 10px;">
                            <asp:RadioButton GroupName="choose" runat="server" ID="HsmLitPades" TextAlign="Right" />
                            <asp:RadioButton GroupName="choose" runat="server" ID="HsmLitP7M" TextAlign="Right" Checked="true" />
                        </p>
                        <p style="margin-left: 20px; margin-top: 10px;">
                            <asp:CheckBox runat="server" ID="HsmCheckMarkTemporal" TextAlign="Right" /></p>
                        <p style="margin-left: 20px; margin-top: 10px;">
                            <asp:RadioButton GroupName="typeSign" runat="server" ID="HsmRadioSign" TextAlign="Right"
                                Checked="true" />
                            <asp:RadioButton GroupName="typeSign" runat="server" ID="HsmRadioCoSign" TextAlign="Right" /></p>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

           <asp:UpdatePanel ID="upReport" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnlReport" runat="server" CssClass="row" Visible="false">
                    <asp:GridView id="grdReport" runat="server" Width="100%" AutoGenerateColumns="False" CssClass="tbl_rounded_custom round_onlyextreme">         
                        <RowStyle CssClass="NormalRow" />
                        <AlternatingRowStyle CssClass="AltRow" />
                        <PagerStyle CssClass="recordNavigator2" />
                        <Columns>
                            <asp:BoundField HeaderText='<%$ localizeByText:MassiveActionLblGrdReport%>' DataField="ObjId">
                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                            </asp:BoundField>
                            <asp:BoundField HeaderText="Esito" DataField="Result">
                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                            </asp:BoundField>
                            <asp:BoundField HeaderText="Dettagli" DataField="Details" HtmlEncode="false">
                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                            </asp:BoundField>
                            </Columns>
                    </asp:GridView>
                </asp:Panel>
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
            <cc1:CustomButton ID="BtnReport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" Visible = "false"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnReport_Click" OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
