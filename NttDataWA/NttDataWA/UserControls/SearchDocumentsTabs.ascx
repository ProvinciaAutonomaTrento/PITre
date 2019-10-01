<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchDocumentsTabs.ascx.cs"
    Inherits="NttDataWA.UserControls.SearchDocumentsTabs" %>
<asp:UpdatePanel runat="server" ID="UpSearchDocumentTabs" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="containerDocumentTabOrangeSx">
            <ul>
                <li class="searchOtherD" id="LiSearchDocumentSimple" runat="server">
                    <asp:HyperLink ID="LinkSearchDocumentSimple" runat="server" NavigateUrl="~/Search/SearchDocument.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
                <li class="searchOtherD" id="LiSearchDocumentAdvanced" runat="server">
                    <asp:HyperLink ID="LinkSearchAdvanced" runat="server" NavigateUrl="~/Search/SearchDocumentAdvanced.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
                <li class="searchOtherD" id="LiSearchDocumentPrint" runat="server">
                    <asp:HyperLink ID="LinkSearchDocumentPrint" runat="server" NavigateUrl="~/Search/SearchDocumentPrints.aspx" Enabled="false"
                        CssClass="clickable"></asp:HyperLink></li>
            </ul>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
