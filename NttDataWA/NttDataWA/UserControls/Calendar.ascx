<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendar.ascx.cs" Inherits="NttDataWA.UserControls.Calendar" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<div class="col-no-margin">
    <asp:UpdatePanel runat="server" ID="UpCalendarHours" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="TxtDataCalendar" />
        </Triggers>
        <ContentTemplate>
            <cc1:CustomTextArea ID="TxtDataCalendar" runat="server" CssClass="txt_textdata_custom datepicker"
                CssClassReadOnly="txt_textdata_custom_disabled" OnTextChanged="TxtDataCalendar_TextChanged" AutoPostBack="true"></cc1:CustomTextArea>
        </ContentTemplate>
    </asp:UpdatePanel>
    
</div>
<div class="col-no-margin">
    <cc1:CustomTextArea ID="TxtHours" runat="server" Visible="false" CssClass="txt_texthours_custom"
        CssClassReadOnly="txt_texthours_custom_disabled" MaxLength="2" onKeyPress="ValidateNumKey();"></cc1:CustomTextArea>
</div>
<div class="col-no-margin">
    <asp:Label ID="lbl_minutes" runat="server" Text=":" Visible="false"></asp:Label>
</div>
<div class="col-no-margin">
    <cc1:CustomTextArea ID="TxtMinute" runat="server" MaxLength="2" CssClass="txt_texthours_custom"
        CssClassReadOnly="txt_texthours_custom_disabled" Visible="false" onKeyPress="ValidateNumKey();">
    </cc1:CustomTextArea>
</div>
<div class="col-no-margin">
    <asp:Label ID="lbl_seconds" runat="server" Text=":" Visible="false"></asp:Label>
</div>
<div class="col-no-margin">
    <cc1:CustomTextArea ID="TxtSeconds" runat="server" MaxLength="2" CssClass="txt_texthours_custom"
        CssClassReadOnly="txt_texthours_custom_disabled" Visible="false" onKeyPress="ValidateNumKey();">
    </cc1:CustomTextArea>
</div>
