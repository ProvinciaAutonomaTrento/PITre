<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotificationCenterSearch.aspx.cs"
    Inherits="Test.NotificationCenterSearch" %>

<%@ Register TagPrefix="cc1" Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/NotificationCenter.css" rel="stylesheet" type="text/css" />
    <link href="../CSS/docspa_30.css" rel="Stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="../LIBRERIE/NotificationCenterScript.js">
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server">
    </asp:ScriptManager>
    <uc1:AppTitleProvider ID="AppTitleProvider1" runat="server" PageName="Ricerca notifiche" />
    <div class="menu_grigio" style="text-align: center;">
        Ricerca notifiche
    </div>
    <div class="testo10N">
        <div class="searchRow">
            Numero di protocollo:
        </div>
        <div class="searchRow">
            <asp:DropDownList ID="ddlProtoNum" runat="server" CssClass="testo_grigio">
                <asp:ListItem Text="Singolo" Value="S" />
                <asp:ListItem Text="Intervallo" Value="I" Selected="True" />
            </asp:DropDownList>
        </div>
        <div class="searchRow">
            <asp:Label Text="Da: " runat="server" ID="lblProtoNumFrom" />
            <asp:TextBox runat="server" ID="txtProtoNumFrom" CssClass="testo_grigio" />
            <asp:Label ID="lblProtoNumTo" runat="server" Text="A: " />
            <asp:TextBox runat="server" ID="txtProtoNumTo" CssClass="testo_grigio" />
        </div>
    </div>
    <div class="searchRowClear" />
    <div class="testo10N">
        <div class="searchRow">
            Data di ricezione notifica:
        </div>
        <div class="searchRow">
            <asp:DropDownList ID="ddlRecDate" runat="server" CssClass="testo_grigio">
                <asp:ListItem Text="Singola" Value="S" />
                <asp:ListItem Text="Intervallo" Value="I" Selected="True" />
            </asp:DropDownList>
        </div>
        <div class="searchRow">
            <asp:Label ID="lblRecDateFrom" runat="server" Text="Da: " />
            <asp:TextBox runat="server" ID="txtRecDateFrom" CssClass="testo_grigio" />
            <cc1:CalendarExtender ID="ceRecDateFrom" TargetControlID="txtRecDateFrom" runat="server"
                Format="dd/MM/yyyy">
            </cc1:CalendarExtender>
            <asp:Label ID="lblRecDateTo" runat="server" Text="A: " />
            <asp:TextBox runat="server" ID="txtRecDateTo" CssClass="testo_grigio" />
            <cc1:CalendarExtender ID="ceRecDateTo" TargetControlID="txtRecDateTo" runat="server"
                Format="dd/MM/yyyy">
            </cc1:CalendarExtender>
        </div>
    </div>
    <div class="searchRowClear" />
    <div class="testo10N">
        <div class="searchRow">
            Tipo evento:
        </div>
        <div class="searchRow">
            <asp:DropDownList ID="ddlEvent" runat="server" CssClass="testo_grigio">
                <asp:ListItem Text="Qualsiasi" Value="" Selected="True" />
                <asp:ListItem Text="Mancata consegna" Value="M" />
                <asp:ListItem Text="Eccezione" Value="E" />
            </asp:DropDownList>
        </div>
    </div>
    <div class="searchRowClear" />
    <div>
    </div>
    <div class="searchButtonRow">
        <asp:Button ID="btnSearch" runat="server" Text="Cerca" OnClick="btnSearch_Click"
            CssClass="pulsante_hand" />&nbsp
        <input type="button" value="Pulisci filtri" onclick="clearFilters();" class="pulsante_hand" />
        <input type="button" value="Chiudi" onclick="self.close();" class="pulsante_hand" />
    </div>
    <div class="searchResultBox">
        <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:DataGrid ID="dgResult" runat="server" AutoGenerateColumns="false"
                    CssClass="datagrid">
                    <AlternatingItemStyle CssClass="bg_grigioA" />
                    <Columns>
                        <asp:TemplateColumn HeaderText="Segnatura">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CssClass="signature" Text="<%# this.GetSignature(Container.DataItem) %>"
                                    OnClientClick="<%# this.GetSignatureScript(Container.DataItem) %>" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn HeaderText="Tipo evento" DataField="Title" />
                        <asp:BoundColumn HeaderText="Descrizione" DataField="Text" />
                        <asp:BoundColumn HeaderText="Soggetto" DataField="Author" />
                        <asp:BoundColumn HeaderText="Data notifica" DataField="PublishDate" />
                    </Columns>
                    <HeaderStyle BackColor="Maroon" CssClass="menu_1_bianco_dg" />
                    <ItemStyle CssClass="bg_grigioN" />
                    <SelectedItemStyle CssClass="bg_grigioS" />
                </asp:DataGrid>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>