<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HomeTabs.ascx.cs" Inherits="NttDataWA.UserControls.HomeTabs" %>
<asp:UpdatePanel runat="server" ID="UpHomeTabs" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="containerHomeTab">
                <ul>
                    <li class="homeOtherD" id="LiNotificationCenter" runat="server">
                        <asp:HyperLink ID="LinkNotificationCenter" runat="server" Enabled="false" NavigateUrl="~/Index.aspx"
                            CssClass="clickable"></asp:HyperLink></li>
                    <li class="homeOtherD" id="LiAdlDocument" runat="server" >
                        <asp:HyperLink ID="LinkAdlDocument" runat="server" Enabled="false" NavigateUrl="~/Home/AdlDocument.aspx"
                            CssClass="clickable"></asp:HyperLink></li>
                    <li class="homeOtherD" id="LiAdlProject" runat="server">
                        <asp:HyperLink ID="LinkAdlProject" runat="server" Enabled="false" NavigateUrl="~/Home/AdlProject.aspx"
                            CssClass="clickable"></asp:HyperLink></li>
                     <li class="homeOtherD" id="LiLibroFirma" runat="server">
                        <asp:HyperLink ID="LinkLibroFirma" runat="server" Enabled="false" NavigateUrl="~/Home/LibroFirma.aspx" 
                            CssClass="clickable"></asp:HyperLink></li>
                    <li class="homeOtherD" id="LiTask" runat="server">
                        <asp:HyperLink ID="LinkTask" runat="server" Enabled="false" NavigateUrl="~/Home/Task.aspx" 
                            CssClass="clickable"></asp:HyperLink></li>
                </ul>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>