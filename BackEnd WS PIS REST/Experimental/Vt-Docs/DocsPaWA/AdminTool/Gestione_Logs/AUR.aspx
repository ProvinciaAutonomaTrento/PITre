<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AUR.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Logs.AUR" %>

<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <script language="JavaScript" src="../CSS/caricamenujs.js"></script>
         <script type="text/javascript">
             function SetDdlAur() {
                 var ddl = document.getElementById('ddlAur');
                 var opt = ddl.options[ddl.selectedIndex].value;
                 if (opt != 'AMM') {
                     document.getElementById('pnlCodiceDescrizione').style.display = 'block';
                 } else {
                     document.getElementById('pnlCodiceDescrizione').style.display = 'none';
                 }
            }
    </script>
     <style type="text/css">
        .recordNavigator2 td
        {
	        background-color: #d9d9d9;
	        FONT-WEIGHT: normal; FONT-SIZE: 10px; 
	        COLOR: #000000; TEXT-INDENT: 0px;
	        FONT-FAMILY: Verdana;
	        text-align: center;
        }
     </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="scriptManager1" runat="server">
    </asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Gestione asserzioni eventi" />
    <!-- Gestione del menu a tendina -->
    <uc2:MenuTendina ID="MenuTendina" runat="server"></uc2:MenuTendina>
    <table height="100%" cellspacing="1" cellpadding="0" width="100%" border="0">
        <tr>
            <td>
                <!-- TESTATA CON MENU' -->
                <uc1:Testata ID="Testata" runat="server"></uc1:Testata>
            </td>
        </tr>
        <tr>
            <td class="testo_grigio_scuro" bgcolor="#c0c0c0" height="20">
                <asp:Label ID="lbl_position" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <!-- TITOLO PAGINA -->
            <td class="titolo" align="center" bgcolor="#e0e0e0" height="20">
                Gestione asserzioni eventi
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <table cellspacing="0" cellpadding="0" align="center" border="0">
                    <tr><td height="10"></td></tr>
                    <tr>
                        <!-- modifica/aggiungi asserzione -->
                        <td>
                            <asp:UpdatePanel ID="pnlAssertion" runat="server" Visible="true" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="panelAssertion" runat="server">
                                            <table class="contenitore" width="100%">
                                                <tr>
                                                    <td>
                                                        <table cellspacing="0" cellpadding="0" width="100%">
                                                            <tr>
                                                                <td class="titolo_pnl" align="left">
                                                                    Asserzione
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="30px">
                                                        <table cellspacing="0" cellpadding="0" width="100%">
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right" width="100">
                                                                    Tipo evento:&nbsp;
                                                                </td>
                                                                <td >
                                                                    <asp:DropDownList ID="ddlTypeEvent" runat="server" Width="700px" CssClass="testo"></asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 30px">
                                                        <table>
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right" width="100">
                                                                    Tipo aggregato di ruoli:&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="ddlAur" runat="server" Width="150px" CssClass="testo">
                                                                        <asp:ListItem Value="AMM" Text="Amministrazione"></asp:ListItem>
                                                                        <asp:ListItem Value="RF" Text="RF" Enabled="false"></asp:ListItem>
                                                                        <asp:ListItem Value="TR" Text="Tipo ruolo"></asp:ListItem>
                                                                        <asp:ListItem Value="R" Text="Ruolo"></asp:ListItem>
                                                                        <asp:ListItem Value="UO" Text="UO"></asp:ListItem>
                                                                     </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:UpdatePanel ID="UpPnlCodiceDescrizione" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <asp:Panel ID="pnlCodiceDescrizione" runat="server">
                                                                    <table cellspacing="0" cellpadding="0" width="100%" style=" border:1px solid">
                                                                        <tr>
                                                                            <td class="testo_grigio_scuro" align="right" width="100">
                                                                                Codice:&nbsp;
                                                                            </td>
                                                                            <td width="850">
                                                                                <asp:TextBox ID="txt_codice" runat="server" CssClass="testo" Width="200px"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="testo_grigio_scuro" align="right" width="100">
                                                                                Descrizione:&nbsp;
                                                                            </td>
                                                                            <td width="850">
                                                                                <asp:TextBox ID="txt_descrizione" runat="server" CssClass="testo" Width="400px"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2" align="right" width="505">
                                                                                <asp:Button ID="btnCerca" Text="Cerca" CssClass="testo_btn" runat="server" OnClientClick="__doPostBack('upPnlResult', 'btnCerca');return false;" />
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td height="5">
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2">
                                                                                    <asp:UpdatePanel runat="server" ID="upPnlResult" UpdateMode="Conditional">
                                                                                        <ContentTemplate>
                                                                                            <asp:Panel ID="panelLabelNoResult" runat="server" Visible = "false">
                                                                                                <div style=" color:Red; text-align:center">
                                                                                                    <asp:Label id="lblNoResult" Runat="server" CssClass="testo" Text="La ricerca non ha prodotto alcun risultato."></asp:Label>
                                                                                                </div>
                                                                                            </asp:Panel>
                                                                                            <asp:GridView ID="GrdAssertionResult" runat="server" BorderColor="Gray" CellPadding="1" BorderWidth="1px" AllowPaging="true"
											                                                    AutoGenerateColumns="False" Width="100%" OnRowDataBound="GrdAssertionResult_RowDataBound" Style="cursor: pointer;"
                                                                                                PageSize="5" OnPageIndexChanging="GrdAssertionResult_PageIndexChanging">
                                                                                                <SelectedRowStyle HorizontalAlign="Left" CssClass="bg_grigioS" />
                                                                                                <RowStyle HorizontalAlign="Left" CssClass="bg_grigioN" Height="20"/>
                                                                                                <AlternatingRowStyle HorizontalAlign="Left" CssClass="bg_grigioA" />
                                                                                                <HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg" BackColor="#810D06"/>
                                                                                                <PagerStyle CssClass="recordNavigator2"/>
                                                                                                <Columns>
                                                                                                    <asp:TemplateField HeaderText='ID' HeaderStyle-Width="0%" Visible="false">
                                                                                                        <ItemTemplate>
                                                                                                            <asp:Label runat="server" ID='lblSystemIdAggregator' Text='<%# this.GetSystemIdAggregatorRole((DocsPAWA.AdminTool.Gestione_Logs.AggregatorRole) Container.DataItem) %>' />
                                                                                                        </ItemTemplate>
                                                                                                    </asp:TemplateField>
                                                                                                    <asp:TemplateField HeaderText='Codice' HeaderStyle-Width="35%">
                                                                                                        <ItemTemplate>
                                                                                                            <asp:Label runat='server' ID='lblCode' Text='<%# this.GetCodeAggregatorRole((DocsPAWA.AdminTool.Gestione_Logs.AggregatorRole) Container.DataItem) %>'/>
                                                                                                        </ItemTemplate>
                                                                                                    </asp:TemplateField>
                                                                                                    <asp:TemplateField HeaderText='Descrizione' HeaderStyle-Width="65%">
                                                                                                        <ItemTemplate>
                                                                                                            <asp:Label runat='server' ID='lblDescription' Text='<%# this.GetDescriptionAggregatorRole((DocsPAWA.AdminTool.Gestione_Logs.AggregatorRole) Container.DataItem) %>'/>
                                                                                                        </ItemTemplate>
                                                                                                    </asp:TemplateField>
                                                                                                </Columns>
                                                                                            </asp:GridView>
                                                                                            <asp:HiddenField ID="grdAssertionResult_rowindex" runat="server"/>
                                                                                    </ContentTemplate>
                                                                                </asp:UpdatePanel>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </asp:Panel>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                        <table>
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right" width="100">
                                                                    Classifica notifica:&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:RadioButtonList ID="rbTipologiaNotifica" runat="server" RepeatDirection="Horizontal" CssClass="testo">
                                                                        <asp:ListItem Text="Operativa" Value="O" Selected="True"></asp:ListItem>
                                                                        <asp:ListItem Text="Informativa" Value="I"></asp:ListItem>
                                                                    </asp:RadioButtonList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table>
                                                            <tr>
                                                                <td class="testo_grigio_scuro" align="right" width="100">
                                                                    In esercizio:&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:CheckBox ID="cbInEsercizio" runat="server" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        </td>
                                                    </tr>
                                                <tr>
                                                    <td align="right">
                                                        <asp:Button ID="btnModificaInserisci" Text="Modifica" CssClass="testo_btn" runat="server" OnClick="btnModificaInserisci_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                            </asp:Panel>
                                        </contentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr><td height="8"></td></tr>
                    <tr>
                        <td class="pulsanti" align="center" width="998">
                            <table width="100%">
                                <tr>
                                    <td>
                                    <asp:UpdatePanel runat="server" ID="UpPanelLabelNoGrid" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Panel ID="panelLabelNoGrid" runat="server">
                                                <asp:Label ID="lblListAssertions" CssClass="titolo" runat="server" Text="Lista Asserzioni" />
                                            </asp:Panel>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    </td>
                                    <td align="right">
                                        <asp:Button ID="btnNuova" runat="server" Text="Nuova" CssClass="testo_btn" OnClientClick="__doPostBack('pnlAssertion');" OnClick="btnNuovo_Click" />
                                        <asp:Button ID="btnElimina" runat="server" Text="Elimina" OnClientClick="__doPostBack('UpdatePanelGridView', 'RemoveSelectedAssertion'); return false;"  CssClass="testo_btn" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td height="3">
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <div id="DivDGList" style="overflow: auto; width: 1000px;">
                                <asp:UpdatePanel runat="server" ID="UpdatePanelGridView" UpdateMode="Conditional">
                                    <contenttemplate>
                                                <asp:GridView ID="GrdAsserzioni" runat="server" BorderColor="Gray" CellPadding="1" BorderWidth="1px"
                                                     AllowPaging="True" PageSize="5" OnPageIndexChanging="GrdAsserzioni_PageIndexChanging"
											        AutoGenerateColumns="False" Width="100%" Style="cursor: pointer;" OnRowDataBound="GrdAsserzioni_RowDataBound">
                                                    <SelectedRowStyle HorizontalAlign="Left" CssClass="bg_grigioA"  />
                                                    <RowStyle HorizontalAlign="Left" CssClass="bg_grigioA" Height="20"/>
                                                    <HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg" BackColor="#810D06"/>
                                                    <PagerStyle CssClass="recordNavigator2" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText='Id Asserzione' HeaderStyle-Width="1%" Visible="false">
                                                           <ItemTemplate>
                                                           <asp:Label runat="server" ID="lblSystemId" Text='<%# this.GetSystemId((DocsPAWA.DocsPaWR.Assertion) Container.DataItem) %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText='Tipo evento' HeaderStyle-Width="20%">
                                                           <ItemTemplate>
                                                                <asp:Label runat="server" ID="lblTypeEvent" Text='<%# this.GetTypeEvent((DocsPAWA.DocsPaWR.Assertion) Container.DataItem) %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText='Tipo aggregato di ruoli' HeaderStyle-Width="20%">
                                                            <ItemTemplate>
                                                                <asp:Label runat="server" ID="lblArg" Text='<%#this.GetTypeAur((DocsPAWA.DocsPaWR.Assertion) Container.DataItem) %>'/>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText='Aggregato di ruoli' HeaderStyle-Width="20%">
                                                            <ItemTemplate>
                                                                <asp:Label runat="server" ID="lblAur" Text='<%# this.GetDescAur((DocsPAWA.DocsPaWR.Assertion) Container.DataItem) %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText='Classifica notifica' HeaderStyle-Width="20%">
                                                            <ItemTemplate>
                                                                <asp:Label runat="server" ID="lblIO" Text='<%#this.GetNotificationType((DocsPAWA.DocsPaWR.Assertion) Container.DataItem) %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText='In esercizio' HeaderStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Label runat="server" ID="lblState" Text='<%#this.GetInExercise((DocsPAWA.DocsPaWR.Assertion) Container.DataItem) %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText='Elimina' HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="cbElimina" runat="server" AutoPostBack="true" OnCheckedChanged="cbElimina_OnCheckedChanged"/>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        </Columns>
                                                </asp:GridView>
                                                <asp:HiddenField ID="grid_rowindex" runat="server"/>
                                                <asp:HiddenField ID="rowIndexChecked" runat="server"/>
                                            </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:UpdatePanel runat="server" ID="UpPnlLegend" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="panelLegend" runat="server">
                                                <div>
                                                    <div style="float:left; margin-top:6px; font-size:8px; width:16px; background-color: Red;"></div>
                                                    <asp:Label ID="lblNonNotificabile" runat="server" CssClass="testo">Il tipo evento è stato configurato come non notificabile pertanto l'asserzione non ha effetto sulle notifiche che non saranno quindi notificate</asp:Label>
                                                </div>
                                                <div>
                                                    <div style="float: left; margin-top:6px; font-size:8px; width:16px; background-color: Gray;"></div>
                                                    <asp:Label ID="lblObbligatoria" runat="server" CssClass="testo">Il tipo evento è stato configurato come obbligatorio pertanto l'asserzione non ha effetto sulle notifiche che saranno sempre notificate come operative</asp:Label>
                                                </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
