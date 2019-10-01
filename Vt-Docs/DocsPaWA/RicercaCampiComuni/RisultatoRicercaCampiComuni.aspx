<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RisultatoRicercaCampiComuni.aspx.cs" Inherits="DocsPAWA.RicercaCampiComuni.RisultatoRicercaCampiComuni" %>
<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>

<head runat="server">
    <title></title>
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <LINK href="../CSS/docspA_30.css" type="text/css" rel="stylesheet">
	<link href="../CSS/StyleSheet.css" type="text/css" rel="Stylesheet" />
    <script language="javascript">	
    function go()
    {
        document.getElementById("divWait").style.display="none";
    }

    function WaitDataGridCallback(eventTarget, eventArgument) {
        document.body.style.cursor = "wait";

        document.getElementById("titolo").innerText = "Ricerca in corso...";
    }

    var abilita = 'false';
    function wait() {
        if (abilita == 'false') {
            document.getElementById("divWait").style.display = "none";
        }
        else {
            document.getElementById("divWait").style.display = "block";
        }
    }
    </script>
</head>
<body text="#660066" vLink="#ff3366" aLink="#cc0066" link="#660066" MS_POSITIONING="GridLayout" onload="go();">
    <form id="form1" runat="server" onsubmit="wait();">
        <table cellSpacing="0" cellPadding="0" width="100%" align="center">
            <tr>
			    <td height="1"><uc1:datagridpagingwait id="DataGridPagingWait" runat="server"></uc1:datagridpagingwait></td>
			</tr>
            <tr id="trHeader" runat="server">
				<td class="pulsanti">
					<table width="100%">
						<tr id="tr1" runat="server">
							<td id="Td2" align="left" height="90%" runat="server"><asp:label id="titolo" CssClass="titolo_real" Runat="server">Elenco Documenti e Fascicoli</asp:label></td>
							<td class="testo_grigio_scuro" style="HEIGHT: 19px" vAlign="middle" align="right" width="5%"></td>
							<td vAlign="middle" align="center" width="5%"></td>
							<td valign="middle" align="center" width="5%"></td>
						</tr>
					</table>                    
				</td>
			</tr>
            <tr id="trBody" runat="server">
				<td vAlign="top">
                    <asp:datagrid SkinID="datagrid" id="DataGrid" runat="server" BorderWidth="1px" 
                        Width="100%" AllowSorting="True" BorderStyle="Inset" 
                        AutoGenerateColumns="False" CellPadding="1" BorderColor="Gray" 
                        HorizontalAlign="Center" AllowPaging="True" PageSize="20" AllowCustomPaging="True" 
                        onpageindexchanged="DataGrid_PageIndexChanged" 
                        onitemcommand="DataGrid_ItemCommand">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle Height="20px" CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg"></HeaderStyle>
							<Columns>
                                <asp:BoundColumn DataField="SYSTEM_ID" HeaderText="SYSTEM_ID" Visible="false">
									<HeaderStyle></HeaderStyle>
								</asp:BoundColumn>
                                <asp:BoundColumn DataField="Descrizione-Oggetto" HeaderText="Descrizione-Oggetto">
									<HeaderStyle Width="55%"></HeaderStyle>
								</asp:BoundColumn>
                                <asp:BoundColumn DataField="TIPO" HeaderText="Tipo">
									<HeaderStyle Width="5%" HorizontalAlign="Center"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" />
								</asp:BoundColumn>
                                <asp:BoundColumn DataField="Codice-Segnatura" HeaderText="Codice-Segnatura">
									<HeaderStyle Width="25%"></HeaderStyle>
								</asp:BoundColumn>
                                <asp:BoundColumn DataField="Data-Creazione" HeaderText="Data-Creazione">
									<HeaderStyle Width="10%" HorizontalAlign="Center"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" />
								</asp:BoundColumn>
                                <asp:ButtonColumn Text="&lt;img src=../images/proto/dettaglio.gif border=0 &gt;" HeaderText="Dettaglio" CommandName="Select">
									<HeaderStyle HorizontalAlign="Center" Width="8%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:ButtonColumn>                         
                            </Columns>
                            <PagerStyle HorizontalAlign="Center" Position="TopAndBottom" BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
                    </asp:datagrid>
                </td>
            </tr>
        </table>
    </form>
    <div id="divWait" style="display:none; position:absolute; top:0; left:0; width:100%; height:600px">
        <div id="waitTrans"></div>            
        <div id="waitImg"></div>
    </div>
</body>
</html>
