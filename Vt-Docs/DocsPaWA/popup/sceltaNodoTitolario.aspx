<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sceltaNodoTitolario.aspx.cs" Inherits="DocsPAWA.popup.sceltaNodoTitolario" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
	<base target="_self">
</head>
<body>
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Scelta Titolario" />
    <TABLE class="info" id="Table1" height="100%" width="100%" align="center" border="0">
        <TR>
			<TD align="center">
			    <br />
				<P class="boxform_item">Selezionarne un nodo di titolario.</P>
			</TD>
		</TR>
		<TR>
			<TD align="center">
			    <DIV id="gw_listaNodi" style="OVERFLOW: auto; HEIGHT: 205px; width:100%">
			        <br/>
			        <asp:GridView ID="gw_Nodi" runat="server" SkinID="gridview" AutoGenerateColumns="False" Width="90%">
                        <AlternatingRowStyle CssClass="bg_grigioA"></AlternatingRowStyle>
                        <RowStyle CssClass="bg_grigioN"></RowStyle>
			            <HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
			            <Columns>
                            <asp:BoundField DataField="ID_TITOLARIO" HeaderText="ID_TITOLARIO" />
                            <asp:BoundField DataField="TITOLARIO" HeaderText="Titolario" >
                                <HeaderStyle Width="20%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CODICE_NODO" HeaderText="CodiceNodo" >
                                <HeaderStyle Width="10%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="NODO" HeaderText="Nodo" >
                                <HeaderStyle Width="20%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="REGISTRO" HeaderText="Registro" >
                                <HeaderStyle Width="20%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="NUM_PROTO_TIT" HeaderText="ProtTit" >
                                <HeaderStyle Width="20%" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Selezione">
                                <ItemTemplate>
                                    <asp:CheckBox ID="cb_selezione" runat="server" OnCheckedChanged="cb_selezione_CheckedChanged" AutoPostBack="true" />
                                </ItemTemplate>
                                <HeaderStyle Width="10%" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <br/>
                 </DIV>
            </TD>
		</TR>
		<TR>
			<TD align="center">
			    <asp:Button ID="btn_Conferma" runat="server" Text="Conferma" Width="80px" CssClass="pulsante_hand" OnClick="btn_Conferma_Click" />&nbsp;
                <asp:Button ID="btn_Chiudi" runat="server" Text="Chiudi" Width="80px" CssClass="pulsante_hand" OnClick="btn_Chiudi_Click" />                
             </TD>
		</TR>
	</TABLE>	
    </form>
</body>
</html>
