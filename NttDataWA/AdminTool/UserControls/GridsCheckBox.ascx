<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GridsCheckBox.ascx.cs" Inherits="SAAdminTool.UserControls.GridsCheckBox" %>
<asp:UpdatePanel ID="upCheckbox" runat="server">
    <ContentTemplate>
        <asp:CheckBox ID="chkSelectDeselect" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelectDeselect_CheckedChanged" OnDataBinding="chkSelectDeselect_DataBinding" />
    </ContentTemplate>
</asp:UpdatePanel>
<asp:HiddenField ID="hfObjectId" runat="server" OnDataBinding="hfObjectId_DataBinding" />