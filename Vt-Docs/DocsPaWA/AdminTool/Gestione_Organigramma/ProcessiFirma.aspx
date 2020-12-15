<%@ Page Language="c#" CodeBehind="ProcessiFirma.aspx.cs" AutoEventWireup="true" Inherits="DocsPAWA.AdminTool.Gestione_Organigramma.ProcessiFirma" %>

<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider" TagPrefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <base target="_self">
    <style type="text/css">
        .externaldiv-style {
            border-style: solid;
            border-width: 1px;
            padding: 4px;
            margin: 4px;
            width: 98%;
        }

        .topdiv-style {
            background-color: #9d9e9c;
            padding: 1px;
            text-align: center;
        }

        .datagriddiv-style {
            padding-top: 5pt;
            padding-bottom: 20pt;
        }

        .bottomdiv-style {
            text-align: center;
        }

        .datagrid {
            border-color: Gray;
            border-width: 1px;
            padding: 1;
            text-align: center;
        }

        .value-style {
            float: left;
            margin-bottom: 3;
            text-align: left;
        }

        .action-style {
            text-align: center;
            vertical-align: middle;
            font-weight: bold;
        }

        .legend-style {
            list-style: none;
        }

        .line-style {
            width: 30%;
            border-width: 1px;
        }

        .recordNavigator2 td {
            background-color: #d9d9d9;
            FONT-WEIGHT: normal;
            FONT-SIZE: 12px;
            COLOR: #000000;
            TEXT-INDENT: 0px;
            FONT-FAMILY: Verdana;
            text-align: center;
            letter-spacing: 4px;
        }
    </style>
    <script type="text/javascript">
        function OpenExport(idRuoloTitolare, idUtenteTitolare) {
            var myUrl = "ExportDettagli.aspx?idRuoloTitolare=" + idRuoloTitolare + "&idUtenteTitolare=" + idUtenteTitolare;
            rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:600px;dialogHeight:200px;status:no;resizable:no;scroll:no;center:yes;help:no;");
        }
    </script>
</head>
<body>
    <form id="frm_ProcessiFirma" runat="server">
        <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Processi di firma" />
        <div class="externaldiv-style">
            <div class="topdiv-style">
                <span class="menu_grigio">
                    <asp:Label ID="LblProcessiFirma" runat="server"></asp:Label>
                </span>
            </div>
            <asp:Panel ID="pnlNoProcessiFirma" runat="server">
                <span class="testo_grigio_scuro">Non ci sono processi di firma</span>
            </asp:Panel>
            <div class="datagriddiv-style">
                <asp:UpdatePanel ID="UpPnlGrdProcessi" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DataGrid ID="grd_processi_firma" runat="server" BorderColor="Gray" CellPadding="1" BorderWidth="1px"
                            AllowCustomPaging="True" AllowPaging="True" PageSize="10" AutoGenerateColumns="False" ShowHeader="True" Width="100%"
                            OnPageIndexChanged="grdProcessi_PageIndexChanged">
                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                            <Columns>
                                <asp:TemplateColumn HeaderText='Nome processo' HeaderStyle-Width="40%">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID='lblNomeProcesso' Text='<%# Bind("nome") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText='disegnatore' HeaderStyle-Width="40%">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID='lblUtenteCreatore' Text='<%#this.GetUtenteCreatore((DocsPAWA.DocsPaWR.ProcessoFirma) Container.DataItem) %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText='Diagrammi di stato' HeaderStyle-Width="20%">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID='lblDiagrammaStato' Text='<%#this.GetDiagrammaStato((DocsPAWA.DocsPaWR.ProcessoFirma) Container.DataItem) %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                            <PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
                        </asp:DataGrid>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="topdiv-style">
                <span class="menu_grigio">
                    <asp:Label ID="LblIstanzaProcessi" runat="server"></asp:Label>
                </span>
            </div>
            <asp:Panel ID="pnlNoIstanzeProcessiFirma" runat="server">
                <span class="testo_grigio_scuro">Non ci sono istanze di processi di firma</span>
            </asp:Panel>
            <div class="datagriddiv-style">
                <asp:UpdatePanel ID="UpPnlGrdIstanzaProcessi" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DataGrid ID="grd_istanza_processi" runat="server" BorderColor="Gray" CellPadding="1" BorderWidth="1px"
                            AllowCustomPaging="True" AllowPaging="True" PageSize="10" AutoGenerateColumns="False" ShowHeader="True" Width="100%"
                            OnPageIndexChanged="grdIstanzaProcessi_PageIndexChanged">
                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                            <Columns>
                                <asp:TemplateColumn HeaderText='Nome processo' HeaderStyle-Width="30%">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID='lblNomeProcesso' Text='<%# Bind("Descrizione") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText='Proponente' HeaderStyle-Width="30%">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID='lblUtenteProponente' Text='<%#this.GetUtenteProponente((DocsPAWA.DocsPaWR.IstanzaProcessoDiFirma) Container.DataItem) %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText='Data avvio' HeaderStyle-Width="10%">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID='lblUtenteProponente' Text='<%# Bind("dataAttivazione") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText='Id documento' HeaderStyle-Width="10%">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID='lblUtenteProponente' Text='<%# Bind("docNumber") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText='Tipo documento' HeaderStyle-Width="15%">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID='lblTipoDocumento' Text='<%#this.GetTipoDocumento((DocsPAWA.DocsPaWR.IstanzaProcessoDiFirma) Container.DataItem) %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                            <PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
                        </asp:DataGrid>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <%--                <div class="recordNavigator2">
                    <asp:PlaceHolder ID="plcNavigator" runat="server" />
                </div>
                <asp:UpdatePanel ID="upPnlGridIndexes" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:HiddenField ID="grid_pageindex" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>--%>
            </div>
            <hr class="line-style" />
            <asp:UpdatePanel ID="upButtons" runat="server">
                <ContentTemplate>
                    <div class="bottomdiv-style">
                        <asp:Button ID="btnClose" Text="Chiudi" ToolTip="Chiudi"
                            CssClass="pulsante_hand" OnClientClick="window.close();" runat="server" />
                        &nbsp;<asp:Button ID="btnEsporta" runat="server" CssClass="pulsante_hand" Text="Esporta"  OnClick="btnExport_Click"/>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
