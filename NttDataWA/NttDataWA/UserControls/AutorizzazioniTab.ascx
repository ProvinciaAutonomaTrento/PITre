<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AutorizzazioniTab.ascx.cs"
    Inherits="NttDataWA.UserControls.AutorizzazioniTab" %>
<asp:UpdatePanel runat="server" ID="UpAutorizzazioniTabs" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="containerDocumentTabOrangeSx">
            <ul>
                <li class="searchOtherD" id="LiSearchDocumentSimple" runat="server">
                    <asp:HyperLink ID="LinkAutorizzazioniVersamento" runat="server" NavigateUrl="~/Deposito/AutorizzazioniVersamento.aspx?PAGESTATE=NEW"
                        Enabled="false" CssClass="clickable" ></asp:HyperLink></li>
                <li class="searchOtherD" id="LiSearchDocumentAdvanced" runat="server">
                    <asp:HyperLink ID="LinkSearchAutorizzazioniVersamento" runat="server" NavigateUrl="~/Search/SearchAutorizzazioniVersamento.aspx"
                        Enabled="false" CssClass="clickable"></asp:HyperLink></li>
            </ul>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
