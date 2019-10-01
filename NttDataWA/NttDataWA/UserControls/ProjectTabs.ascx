<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectTabs.ascx.cs"
    Inherits="NttDataWA.UserControls.ProjectTabs" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:UpdatePanel runat="server" ID="UpProjectTabs" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="containerDocumentTabOrangeSx">
            <ul>
                <li class="prjOtherD" id="LiProfile" runat="server">
                    <asp:HyperLink ID="LinkProfile" runat="server" NavigateUrl="~/Project/Project.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
                <li class="prjOtherD" id="LiStructure" runat="server">
                    <asp:HyperLink ID="LinkStructure" runat="server" NavigateUrl="~/Project/Structure.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
                <li class="prjOtherD" id="LiTransmissions" runat="server">
                    <asp:HyperLink ID="LinkTransmissions" runat="server" NavigateUrl="~/Project/TransmissionsP.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
                <li class="prjOtherD" id="LiVisibility" runat="server">
                    <asp:HyperLink ID="LinkVisibility" runat="server" NavigateUrl="~/Project/VisibilityP.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
                <li class="prjOtherD" id="LiEvents" runat="server">
                    <asp:HyperLink ID="LinkEvents" runat="server" NavigateUrl="~/Project/EventsP.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
            </ul>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
