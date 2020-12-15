<%@ Page language="c#" Codebehind="MailCheckResponse.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.Interoperabilita.MailCheckResponse" EnableEventValidation="true" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language="javascript">
			
			function CloseWindow()
			{
				window.close();
            }

		</script>
	</HEAD>
	<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout">
		<form id="frmMailCheckResponse" method="post" runat="server">
        <asp:HiddenField ID="tipo" runat="server" Value="" />
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Esito controllo casella istituzionale" />
			<TABLE class="info" id="tblContainer" cellSpacing="0" cellPadding="5" width="870" align="center"
				border="0" runat="server">
				<TR>
					<TD class="item_editbox">
						<P class="boxform_item"><asp:label id="lblTitle" runat="server">
								Esito controllo casella istituzionale
							</asp:label></P>
					</TD>
				</TR>
				<TR>
					<TD class="item_editbox">
						<TABLE class="info_grigio" id="tblCheckDetails" cellSpacing="0" cellPadding="2" width="100%"
							align="center" border="0" runat="server">
							<TR>
								<TD class="titolo_scheda" vAlign="top" width="25%"><asp:label id="lblMailUserID" runat="server">Mail User ID:</asp:label></TD>
								<TD class="titolo_scheda" vAlign="top" width="75%"><asp:label id="txtMailUserID" runat="server"></asp:label></TD>
							</TR>
							<TR>
								<TD class="titolo_scheda" vAlign="top" width="25%"><asp:label id="lblMailServer" runat="server">Mail server:</asp:label></TD>
								<TD class="titolo_scheda" vAlign="top" width="75%"><asp:label id="txtMailServer" runat="server"></asp:label></TD>
							</TR>
							<TR>
								<TD class="titolo_scheda" vAlign="top" width="25%"><asp:label id="lblMailAddress" runat="server">Indirizzo email:</asp:label></TD>
								<TD class="titolo_scheda" vAlign="top" width="75%"><asp:label id="txtMailAddress" runat="server"></asp:label></TD>
							</TR>
							<tr>
								<TD class="titolo_scheda" vAlign="top" width="25%"><asp:label id="lblCodiceRegistro" runat="server">Registro:</asp:label></TD>
								<TD class="titolo_scheda" vAlign="top" width="75%"><asp:label id="txtCodiceRegistro" runat="server"></asp:label></TD>
							</tr>
							<TR>
								<TD class="titolo_scheda" vAlign="top" width="25%"><asp:label id="lblCheckResponse" runat="server">Esito controllo casella:</asp:label></TD>
								<TD class="titolo_scheda" vAlign="top" width="75%">
									<DIV id="panelCheckResponse" style="OVERFLOW: auto; WIDTH: 100%; HEIGHT: 40px" runat="server"></DIV>
								</TD>
							</TR>
							<TR>
								<TD class="titolo_scheda" vAlign="top" width="25%">
									<asp:label id="lblMailMessageCount" runat="server" CssClass="titolo_scheda"> Messaggi:</asp:label>
								</TD>
								<TD class="titolo_scheda" vAlign="top" width="75%">
									<%-- <asp:label id="txtMailMessageCount" runat="server" CssClass="titolo_scheda"></asp:label> --%>
								</TD>
							</TR>
                            <tr>
                            	<td align="left"  Width="70%">
						            <asp:table id="tbl_MailTypelist" runat="server" HorizontalAlign="Center" Width="100%" BorderWidth="1px"
							            BorderColor="LightGrey" BorderStyle="Solid" >
					                </asp:table>
				                </td>
                                <td  align = "right">
                                    <asp:button id="btnEsporta" runat="server" CssClass="PULSANTE" Text="Esporta" 
                                        Width="80" onclick="btn_esporta_Click"></asp:button>
                                </td>
                            </tr>
						</TABLE>
					</TD>
				</TR>
				<TR>
					<TD class="item_editbox" align="right">
						<DIV id="panelGridCheckResponse" style="OVERFLOW: auto; WIDTH: 100%; HEIGHT: 400px" align="center">
                        <asp:datagrid id="grdCheckResponse" runat="server" SkinID="datagrid" AutoGenerateColumns="False" BorderColor="Gray"
								CellPadding="1" BorderWidth="1px" Width="100%">
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle HorizontalAlign="Center" CssClass="testo_biancoN"></HeaderStyle>
								<Columns>
									<asp:BoundColumn Visible="False" DataField="MailID" HeaderText="ID"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="Type" HeaderText="Tipo">
										<HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="From" HeaderText="Mittente">
										<HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="Subject" HeaderText="Oggetto">
										<HeaderStyle Width="25%"></HeaderStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="Date" HeaderText="Inviato il">
										<HeaderStyle Width="5%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="CountAttatchments" HeaderText="Allegati">
										<HeaderStyle HorizontalAlign="Center" Width="5px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="CheckResult" HeaderText="Esito controllo messaggio">
										<HeaderStyle Width="25%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
									</asp:BoundColumn>
								</Columns>
							</asp:datagrid></DIV>
					</TD>
				</TR>
				<TR>
					<TD class="item_editbox" align="center">
                        <asp:button id="btnDocGrigio" runat="server" Width="55px" Text="Crea documento" CssClass="PULSANTE" Height="19px"></asp:button>
                        &nbsp;&nbsp;
						<asp:button id="btnClose" runat="server" Width="55px" Text="CHIUDI" CssClass="PULSANTE" Height="19px"></asp:button>
                        
                    </TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
