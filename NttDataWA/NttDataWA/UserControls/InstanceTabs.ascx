<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InstanceTabs.ascx.cs"
    Inherits="NttDataWA.UserControls.InstanceTabs" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:UpdatePanel runat="server" ID="UpProjectTabs" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="containerDocumentTabOrangeSx">
            <ul>
                <li class="prjOtherD" id="LiInstance" runat="server">
                    <asp:HyperLink ID="LinkInstance" runat="server" NavigateUrl="~/Management/InstanceDetails.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
                <li class="prjOtherD" id="LiInstanceStructure" runat="server">
                    <asp:HyperLink ID="LinkInstanceStructure" runat="server" NavigateUrl="~/Management/InstanceStructure.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
            </ul>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
