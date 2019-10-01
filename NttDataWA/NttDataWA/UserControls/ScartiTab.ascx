<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScartiTab.ascx.cs" Inherits="NttDataWA.UserControls.ScartiTab" %>
<asp:UpdatePanel runat="server" ID="UpScartiTabs" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="containerDocumentTabOrangeSx">
            <ul>
                <li class="searchOtherD" id="LiSearchDocumentSimple" runat="server">
                    <asp:HyperLink ID="LinkGestioneScarto" runat="server" NavigateUrl="~/Deposito/Scarto.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
            </ul>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
