<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Messager.ascx.cs" Inherits="NttDataWA.UserControls.messager" %>
<asp:PlaceHolder ID="plc" runat="server" Visible="false">
<div class="messager">
    <div class="messager_c1"><img src="<%=Page.ResolveClientUrl("~/Images/Common/messager_warning.png") %>" alt="" /></div>
    <div class="messager_c2"><span><asp:Literal runat="server" ID="MessangerWarning"></asp:Literal></span><asp:Literal ID="msg" runat="server" /></div>
    <div class="messager_c3"><img src="<%=Page.ResolveClientUrl("~/Images/Common/messager_warning.png") %>" alt="" /></div>
</div>
</asp:PlaceHolder>