<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleHistory.aspx.cs" Inherits="DocsPAWA.popup.RoleHistory" %>

<%@ Register Src="../UserControls/AjaxMessageBox.ascx" TagName="AjaxMessageBox" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Storico modifiche al ruolo</title>
    <link href="../CSS/docspa_30.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .externaldiv-style
        {
            border-style: solid;
            border-width: 1px;
            padding: 4px;
            margin: 4px;
            width: 93%;
        }
        .topdiv-style
        {
            background-color: #9d9e9c;
            padding: 1px;
            text-align: center;
        }
        .datagriddiv-style
        {
            padding-top: 5pt;
            padding-bottom: 5pt;
        }
        .bottomdiv-style
        {
            text-align: center;
        }
        
        .datagrid
        {
            border-color: Gray;
            border-width: 1px;
            padding: 1;
            text-align: center;
        }
        
        .value-style
        {
            float: left;
            margin-bottom: 3;
            text-align: left;
        }
        
        .action-style
        {
            text-align: center;
            vertical-align: middle;
            font-weight: bold;
        }
        
        .legend-style
        {
            list-style: none;
        }
        
        .line-style
        {
            width: 30%;
            border-width: 1px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>
    <div class="externaldiv-style">
        <div id="divTitle" class="topdiv-style">
            <span class="menu_grigio">Storia modifiche apportate al ruolo </span>
        </div>
        <div class="datagriddiv-style">
            <asp:DataGrid ID="dgHistory" runat="server" AutoGenerateColumns="false" CssClass="datagrid"
                SkinID="datagrid">
                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="Maroon" HorizontalAlign="Center">
                </HeaderStyle>
                <Columns>
                    <asp:BoundColumn DataField="HistoryAction" ItemStyle-CssClass="action-style" HeaderText="Azione" />
                    <asp:TemplateColumn HeaderText="Data azione" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle"
                        ItemStyle-Width="20%" HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="lblActionDate" CssClass="testo_grigio" runat="server" Text="<%# this.FormatDate(((DocsPAWA.DocsPaWR.RoleHistoryItem)Container.DataItem).ActionDate) %>" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Dettaglio ruolo" ItemStyle-VerticalAlign="Middle">
                        <ItemTemplate>
                            <div style="float: left; margin-bottom: 3;" class="titolo_scheda">
                                Descrizione:&nbsp;
                            </div>
                            <div class="value-style">
                                <asp:Label ID="lblRoleDescription" runat="server" Text="<%# this.GetRoleDescription(((DocsPAWA.DocsPaWR.RoleHistoryItem)Container.DataItem)) %>" />
                            </div>
                            <div style="clear: both;" />
                            <div style="float: left; margin-bottom: 3;" class="titolo_scheda">
                                Descrizione UO:&nbsp;
                            </div>
                            <div class="value-style">
                                <asp:Label ID="lblUoDescription" runat="server" Text="<%# ((DocsPAWA.DocsPaWR.RoleHistoryItem)Container.DataItem).UoDescription %>" /><br />
                            </div>
                            <div style="clear: both; margin-bottom: 3;" />
                            <div style="float: left;" class="titolo_scheda">
                                Descrizione tipo ruolo:&nbsp;
                            </div>
                            <div class="value-style">
                                <asp:Label ID="lblRoleType" runat="server" Text="<%# ((DocsPAWA.DocsPaWR.RoleHistoryItem)Container.DataItem).RoleTypeDescription %>" />
                            </div>
                            <div style="clear: both;" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </div>
        <div class="testo10N" style="border-top: 1px solid black;">
            Legenda azione:
            <ul class="legend-style">
                <li><strong>C</strong> = Ottenuto per creazione del ruolo</li>
                <li><strong>M</strong> = Ottenuto per modifica del ruolo</li>
                <li><strong>S</strong> = Ottenuto per storicizzazione del ruolo</li>
            </ul>
        </div>
        <hr class="line-style" />
        <asp:UpdatePanel ID="upButtons" runat="server">
            <ContentTemplate>
                <div class="bottomdiv-style">
                    <asp:Button ID="btnClose" Text="Chiudi" ToolTip="Chiudi" 
                        CssClass="pulsante_hand" OnClientClick="self.close()" runat="server" 
                        onclick="btnClose_Click" />
                    &nbsp;<asp:Button ID="btnEsporta" runat="server" CssClass="pulsante_hand" Text="Esporta" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
