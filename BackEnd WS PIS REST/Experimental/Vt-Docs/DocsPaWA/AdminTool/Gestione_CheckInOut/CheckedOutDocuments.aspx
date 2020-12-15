<%@ Page language="c#" Codebehind="CheckedOutDocuments.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.AdminTool.Gestione_CheckInOut.CheckedOutDocuments" %>
<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
        <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
		<script language="javascript">
			function apriPopup() {
				hlp = window.open('../help.aspx?from=SD','','width=450,height=500,scrollbars=YES');
			}				
			
			function cambiaPwd() {
				cambiapass = window.open('../CambiaPwd.aspx','','width=410,height=300,scrollbars=NO');
			}	
			
			function ClosePopUp()
			{	
				if(typeof(cambiapass) != 'undefined')
				{
					if(!cambiapass.closed)
						cambiapass.close();
				}
				if(typeof(hlp) != 'undefined')
				{
					if(!hlp.closed)
						hlp.close();
				}				
			}
			function ConfirmUndoCheckOut()
			{
				var ret=confirm("Annullare il blocco sul documento selezionato?");
				
				frmCheckedOutDocuments.undoCheckOutRequested.value=ret;
				
				if (ret)
				    ShowWaitCursor();
				    
				return ret;
			}
			
			function UndoCheckOutCommitted()
			{
				alert('Blocco sul documento rimosso correttamente');
			}		
			
			function ShowWaitCursor()
			{
				window.document.body.style.cursor="wait";
            }
            
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout"
		onunload="ClosePopUp()">
		<form id="frmCheckedOutDocuments" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Sblocca documenti" />
			<!-- Gestione del menu a tendina -->
			<uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
			<table cellSpacing="1" cellPadding="0" width="100%" border="0">
				<tr>
					<td>
						<!-- TESTATA CON MENU' --><uc1:testata id="Testata" runat="server"></uc1:testata></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20"><asp:label id="lbl_position" runat="server"></asp:label></td>
				</tr>
				<tr>
					<!-- TITOLO PAGINA -->
					<td class="titolo" align="center" bgColor="#e0e0e0" height="20">Documenti bloccati</td>
				</tr>
				<tr>
					<td height="5">
					</td>
				</tr>
				<tr>
					<td height="5" align="right" width="80%">
						<asp:Button id="btnRefresh" runat="server" Text="Aggiorna" CssClass="testo_btn_p" ToolTip="Aggiorna lista documenti bloccati"></asp:Button>
					</td>
				</tr>
				<tr>
					<td height="5" align="center">
						<asp:Label id="lblMessage" runat="server" CssClass="testo_rosso"></asp:Label>
					</td>
				</tr>
				<tr>
					<td align="center" width="80%">
						<DIV id="divGrdCheckOutDocuments" style="OVERFLOW: auto; WIDTH: 100%; HEIGHT: 357px"
							align="center"><asp:datagrid id="grdCheckOutDocuments" runat="server" AutoGenerateColumns="False" BorderColor="Gray"
								CellPadding="1" BorderWidth="1px" Width="100%">
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
								<Columns>
									<asp:BoundColumn Visible="False" DataField="ID"></asp:BoundColumn>
									<asp:BoundColumn DataField="Document" HeaderText="ID Doc. - Segnatura">
										<HeaderStyle HorizontalAlign="Center" Width="25%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False"></ItemStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="CheckOutUser" HeaderText="Bloccato da">
										<HeaderStyle HorizontalAlign="Center" Width="30%"></HeaderStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="CheckOutDate" HeaderText="Data">
										<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="DocumentLocation" HeaderText="Percorso documento">
										<HeaderStyle HorizontalAlign="Center" Width="30%"></HeaderStyle>
									</asp:BoundColumn>
									<asp:TemplateColumn HeaderText="Sblocca">
										<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
											<cc2:imagebutton id="btnUndoCheckOut" runat="server" Width="20px" CommandName="UNDO_CHECK_OUT" ImageUrl="../Images/cestino.gif"
												Height="20px" ToolTip="Annulla blocco"></cc2:imagebutton>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</asp:datagrid>
						</DIV>
					</td>
				</tr>
			</table>
			<input type="hidden" id="undoCheckOutRequested" runat="server">
		</form>
	</body>
</HTML>
