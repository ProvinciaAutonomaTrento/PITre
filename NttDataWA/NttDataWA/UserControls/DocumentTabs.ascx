<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentTabs.ascx.cs"
    Inherits="NttDataWA.UserControls.DocumentTabs" %>
<asp:UpdatePanel runat="server" ID="UpDocumentTabs" UpdateMode="Conditional">
    <ContentTemplate>
        <ul>
            <li class="docOtherD" id="LiProfile" runat="server">
                <asp:HyperLink ID="LinkProfile" runat="server" Text="Profilo" NavigateUrl
                ="~/Document/Document.aspx" Enabled="false"></asp:HyperLink></li>
            <li class="docOtherD" id="LiClassificationSchemes" runat="server">
                <asp:HyperLink ID="LinkClassificationSchemes" runat="server" NavigateUrl="~/Document/Classifications.aspx" Enabled="false" CssClass="clickable"></asp:HyperLink></li>
            <li class="docOtherD" id="LiAttachedFiles" runat="server">
                <asp:HyperLink ID="LinkAttachedFiles" runat="server" NavigateUrl="~/Document/Attachments.aspx" Enabled="false" CssClass="clickable"></asp:HyperLink></li>
            <li class="docOtherD" id="LiTransmissions" runat="server">
                <asp:HyperLink ID="LinkTransmissions" runat="server" NavigateUrl="~/Document/Transmissions.aspx" Enabled="false" CssClass="clickable"></asp:HyperLink></li>
            <li class="docOtherD" id="LiVisibility" runat="server">
                <asp:HyperLink ID="LinkVisibility" runat="server" NavigateUrl="~/Document/Visibility.aspx" Enabled="false" CssClass="clickable"></asp:HyperLink></li>
            <li class="docOtherD" id="LiEvents" runat="server">
                <asp:HyperLink ID="LinkEvents" runat="server" Text="Eventi" NavigateUrl="~/Document/Events.aspx" Enabled="false" ></asp:HyperLink></li>
        </ul>
    </ContentTemplate>
</asp:UpdatePanel>
