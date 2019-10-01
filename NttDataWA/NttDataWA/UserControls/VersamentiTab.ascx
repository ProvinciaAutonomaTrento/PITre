<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VersamentiTab.ascx.cs"
    Inherits="NttDataWA.UserControls.VersamentiTab" %>
<asp:UpdatePanel runat="server" ID="UpAutorizzazioniTabs" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="containerDocumentTabOrangeSx">
            <ul>
                <li class="searchOtherD" id="LiSearchDocumentSimple" runat="server">
                    <asp:HyperLink ID="LinkGestioneVersamento" runat="server" NavigateUrl="~/Deposito/Versamento.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
                <li class="searchOtherD" id="LiVersamentoImpact" runat="server">
                    <asp:HyperLink ID="LinkVersamentoImpact" runat="server" NavigateUrl="~/Deposito/VersamentoImpact.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
            </ul>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
