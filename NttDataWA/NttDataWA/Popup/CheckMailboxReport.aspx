<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="CheckMailboxReport.aspx.cs" Inherits="NttDataWA.Popup.CheckMailboxReport" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
            </style>
</asp:Content>

<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="ReportGenerator" runat="server" Url="../Popup/ReportGenerator.aspx"
        Width="700" Height="800" IsFullScreen="False" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', '');}" />
    <div id="containerCheck">
        <div class="containerInfoMailbox">
             <div class="riga" style="margin:0; padding-top:5px">
                    <div class="colonnasx3">
                        <asp:Label id="lblMailUserId" runat="server"></asp:Label>
                    </div>
                    <div class="colonnadx3">
                        <asp:Label id="txtMailUserId" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="riga">
                    <div class="colonnasx3">
                        <asp:Label id="lblMailServer" runat="server"></asp:Label>
                    </div>
                    <div class="colonnadx3">
                        <asp:Label id="txtMailServer" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="riga">
                    <div class="colonnasx3">
                        <asp:Label id="lblIndirizzoEmail" runat="server"></asp:Label>
                    </div>
                    <div class="colonnadx3">
                        <asp:Label id="txtIndirizzoEmail" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="riga">
                    <div class="colonnasx3">
                        <asp:Label id="lblRegistro" runat="server"></asp:Label>
                    </div>
                    <div class="colonnadx3">
                        <asp:Label id="txtRegistro" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="riga">
                    <div class="colonnasx3">
                        <asp:Label id="lblEsitoCasella" runat="server"></asp:Label>
                    </div>
                    <div class="colonnadx3">
                        <div id="txtEsitoCasella" style=" overflow: auto; width: 100%; height: 40px" runat="server"></div>
                    </div>
                </div>
             <div id="containerInfoMail" style=" margin:0; border:0; padding-top:20px">
                    <span class="weight">
                        <asp:Label id="lblMessages" runat="server"></asp:Label>
                    </span>
                    <div id="contentMailTypelist" style=" margin:0; border:0; padding-top:10px;">
                         <asp:table id="tbl_MailTypelist" runat="server" Width="30%" BorderWidth="1px"
							BorderColor="LightGrey" BorderStyle="Solid" >
					    </asp:table>
                    </div>
                </div>
        </div>
       <div id="containerGridMail" style=" margin:0; border:0; padding-top:10px;">
        <asp:UpdatePanel ID="panelGrdMails" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate> 
                 <asp:GridView ID="GrdMails" runat="server" Width="100%" AutoGenerateColumns="false"
                              CssClass="tbl_rounded round_onlyextreme" BorderWidth="0">
                              <RowStyle CssClass="NormalRow" Height="20" />
                              <AlternatingRowStyle CssClass="AltRow" />
                              <PagerStyle CssClass="recordNavigator2" />
                              <HeaderStyle Font-Size="Small" Wrap="false" />
                              <Columns>
                                    <asp:BoundField DataField="Type" ItemStyle-Wrap="false" runat="server" HeaderText='<%$ localizeByText:CheckMailboxReportType%>' />
                                    <asp:BoundField DataField="From" ItemStyle-Wrap="false" runat="server" HeaderText='<%$ localizeByText:CheckMailboxReportSender%>' />
                                    <asp:BoundField DataField="Subject" runat="server" HtmlEncode="false" HeaderText='<%$ localizeByText:CheckMailboxReportObject%>' />
                                    <asp:BoundField DataField="Date" runat="server" HeaderText='<%$ localizeByText:CheckMailboxReportDateSent%>' />
                                    <asp:BoundField DataField="CountAttatchments" runat="server" HeaderText='<%$ localizeByText:CheckMailboxReportAttachments%>' />
                                    <asp:BoundField DataField="CheckResult" runat="server" HeaderText='<%$ localizeByText:CheckMailboxReportErrorMessage%>' />
                              </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <asp:HiddenField ID="tipo" runat="server" Value="" />
    </div>
</asp:Content>


<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="static">
        <ContentTemplate>
            <cc1:CustomButton ID="CheckMailboxReportExport" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="CheckMailboxReportExport_Click" />
            <cc1:CustomButton ID="CheckMailboxReportCreateDoc" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="CheckMailboxReportCreateDoc_Click" />
            <cc1:CustomButton ID="CheckMailboxReportClose" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="parent.closeAjaxModal('CheckMailboxReport','close');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

