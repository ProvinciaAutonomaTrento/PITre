<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchProjectsTabs.ascx.cs"
    Inherits="NttDataWA.UserControls.SearchProjectTabs" %>
<asp:UpdatePanel runat="server" ID="UpSearchDocumentTabs" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="containerDocumentTabOrangeSx">
            <ul>
                <li class="searchOtherD" id="LiSearchProject" runat="server">
                    <asp:HyperLink ID="LinkSearchProject" runat="server" NavigateUrl="~/Search/SearchProject.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
                <li class="searchOtherD" id="LiSearchArchive" runat="server">
                    <asp:HyperLink ID="LinkSearchArchive" runat="server" NavigateUrl="~/Search/SearchArchive.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
            </ul>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
