<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RuoloResponsabile.ascx.cs"
    Inherits="DocsPAWA.AdminTool.UserControl.RuoloResponsabile" %>
<%@ Register Src="../../UserControls/AjaxMessageBox.ascx" TagName="AjaxMessageBox"
    TagPrefix="uc1" %>
<asp:ScriptManagerProxy ID="ScriptManager" runat="server">
</asp:ScriptManagerProxy>
<div style="float:left; padding-top:0px;">
<div style="float: left;margin-top:1px;">
    <asp:TextBox ID="txtRoleDescription" runat="server" CssClass="testo" Width="260px" BackColor="#E0E0E0"
        ReadOnly="True"></asp:TextBox>
</div>
<div style="float: left;margin-top:1px;">
    <asp:UpdatePanel ID="upRubrica" runat="server">
        <ContentTemplate>
            <asp:ImageButton ImageAlign="Top" ID="btnRubricaRuoloResp" runat="server" Visible="true" ImageUrl="~/images/proto/rubrica.gif"
                ToolTip="Rubrica" OnClick="btnRubricaRuoloResp_Click" OnClientClick="OpenAddressBook()">
            </asp:ImageButton>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
<div style="float: left;padding:1px; margin-top:1px;">
    <asp:UpdatePanel ID="upDelete" runat="server">
        <ContentTemplate>
            <asp:ImageButton ID="imgDelRuoloResp" runat="server" ImageUrl="~/AdminTool/Images/cestino.gif"
                ToolTip="Elimina" OnClick="imgDelRuoloResp_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
</div>

<div style="clear:both;" />

<asp:UpdatePanel ID="upUsers" runat="server">
    <ContentTemplate>
        <asp:DropDownList ID="ddlUsers" runat="server" CssClass="testo" DataValueField="SystemId"
            Visible="false" DataTextField="Description" 
            onprerender="ddlUsers_PreRender" 
            onselectedindexchanged="ddlUsers_SelectedIndexChanged">
        </asp:DropDownList>
    </ContentTemplate>
</asp:UpdatePanel>
<uc1:AjaxMessageBox ID="AjaxMessageBox1" runat="server" />

