<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InteroperabilitySettings.ascx.cs"
    Inherits="DocsPAWA.AdminTool.UserControl.InteroperabilitySettings" %>
<%@ Register Src="RuoloResponsabile.ascx" TagName="RuoloResponsabile" TagPrefix="uc1" %>
<div style="text-align: left; vertical-align: middle;" class="testo_grigio_scuro">
    <div style="vertical-align: baseline; float: left;">
        Attivazione interoperabilità:&nbsp;
        <asp:CheckBox ID="chkEnableInterop" runat="server" CssClass="testo_piccoloB" 
            AutoPostBack="true" OnCheckedChanged="chkEnableInterop_CheckedChanged" />&nbsp;
    </div>
    <div style="vertical-align: baseline;">
        <asp:UpdatePanel ID="upSettings" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnlSettings" runat="server">
                    <div style="float: left; vertical-align: baseline;" class="testo_grigio_scuro">
                        &nbsp;Url:&nbsp;
                        <asp:TextBox ID="txtInteroperabilityUrl" runat="server" CssClass="testo" Width="300px"
                            BackColor="#E0E0E0" ReadOnly="True" />
                    </div>
                    <div style="clear: both;">
                    </div>
                    <div style="float: left; margin-top: 3px;">
                        <div style="float: left; vertical-align: baseline;" class="testo_grigio_scuro">
                            Modalità di gestione:&nbsp;
                            <asp:DropDownList ID="ddlManagement" runat="server" CssClass="testo_piccoloB" OnSelectedIndexChanged="ddlManagement_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                        <div style="float: left;">
                            <asp:UpdatePanel ID="upCanCheckPrivate" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:CheckBox ID="chkKeepPrivate" runat="server" CssClass="testo_piccoloB" Text="Mantieni documento ricevuto come pendente" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlManagement" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div style="clear: both;">
                    </div>
                    <div style="float: left; margin-top: 5px;" class="testo_grigio_scuro">
                        Ruolo e utente creatore:&nbsp;
                    </div>
                    <div style="float: left; margin-top: 3px;">
                        <asp:UpdatePanel ID="c" runat="server">
                            <ContentTemplate>
                                <uc1:RuoloResponsabile ID="RuoloResponsabile1" runat="server" ShowUserSelection="true" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="chkEnableInterop" EventName="CheckedChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <div style="clear: both;" />
</div>