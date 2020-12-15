<%@ Page language="c#" Codebehind="StoricoStati.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.StoricoStati" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
	    <title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspA_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
		function chiudiFinestra(){
			window.close();
		}
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Storico stati Documento" />
			<table width="100%">
				<tr>
					<td class="titolo" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid"
						align="center" width="80%" bgColor="#e0e0e0" height="20"><asp:label id="lbl_NomeModello" runat="server" Font-Bold="True">Storico stati Documento</asp:label></td>
					<td class="titolo" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid"
						align="center" bgColor="#e0e0e0" height="20"><asp:button id="btn_Chiudi" runat="server" Width="80px" CssClass="pulsante" Text="Chiudi"></asp:button></td>
				</tr>
				<tr>
					<td colSpan="2"><br>
						<div id="divStoricoStati"><asp:datagrid id="dgStoricoStati" runat="server" SkinID="datagrid" 
                                Width="100%" AutoGenerateColumns="False"
								AllowPaging="True" PageSize="10">
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle Height="20px" CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
								<Columns>
									<asp:BoundColumn DataField="RUOLO" HeaderText="Ruolo">
										<HeaderStyle Width="30%"></HeaderStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="UTENTE" HeaderText="Utente">
										<HeaderStyle Width="30%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="DATA" HeaderText="Data">
										<HeaderStyle Width="20%"></HeaderStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="Nuovo Stato" HeaderText="Mod. Stato">
										<HeaderStyle Width="20%"></HeaderStyle>
									</asp:BoundColumn>
								</Columns>
								<PagerStyle Font-Size="X-Small" HorizontalAlign="Center" ForeColor="Gray" Mode="NumericPages"></PagerStyle>
							</asp:datagrid></div>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
