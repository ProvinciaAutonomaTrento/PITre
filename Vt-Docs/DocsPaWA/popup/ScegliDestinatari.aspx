<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScegliDestinatari.aspx.cs" Inherits="DocsPAWA.popup.ScegliDestinatari" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
   <title>Untitled Page</title>
   <meta name="vs_showGrid" content="False">
   <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
   <meta content="C#" name="CODE_LANGUAGE">
   <meta content="JavaScript" name="vs_defaultClientScript">
   <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
   <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
   <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
   <base target="_self">
</head>
<body bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5" MS_POSITIONING="GridLayout">
   <form id="frm_ScegliDestinatari" method="post" runat="server">
      <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Seleziona " />
      <TABLE class="contenitore" id="tbl_ScegliDestinatari" height="100%" width="100%" align="center" border="0">
      <TR valign="top" height="180">
					<TD>
					<asp:panel id="pnl_corr" Runat="server" Visible="true">
							<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
								<TR>
									<TD class="menu_1_rosso" align="center"  height="20">Seleziona 
										il mittente per il protocollo in ingresso
									</TD>
									
								</TR>
								<TR>
									<TD align="center" colSpan="4">
										<DIV id="div_corr" style="OVERFLOW: auto; HEIGHT: 165px">
											<asp:datagrid id="dg_lista_corr" runat="server" SkinID="datagrid" Width="96%" 
                           Visible="True" BorderColor="Gray"
												CellPadding="1" BorderWidth="1px" AutoGenerateColumns="False" PageSize="6" 
                           AllowPaging="True" onpageindexchanged="dg_lista_corr_PageIndexChanged">
												<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
												<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
												<ItemStyle CssClass="bg_grigioN"></ItemStyle>
												<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
												<Columns>
													<asp:BoundColumn Visible="False" DataField="SYSTEM_ID"></asp:BoundColumn>
													<asp:BoundColumn DataField="DESC_CORR" HeaderText="Descrizione">
														<HeaderStyle Width="490px"></HeaderStyle>
														<ItemStyle Font-Names="verdana"></ItemStyle>
													</asp:BoundColumn>
													<asp:BoundColumn Visible="False" DataField="TIPO_CORR" HeaderText="Tipo">
														<HeaderStyle Width="30px"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
													</asp:BoundColumn>
													<asp:TemplateColumn>
														<HeaderStyle Width="30px"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
														<ItemTemplate>
															<asp:RadioButton id="OptCorr" runat="server" AutoPostBack="True" Visible="True"
																TextAlign="Right" Text=""></asp:RadioButton>
														</ItemTemplate>
													</asp:TemplateColumn>
												</Columns>
												<PagerStyle VerticalAlign="Middle" BorderColor="White" HorizontalAlign="Center" BackColor="#C2C2C2"
													CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
											</asp:datagrid></DIV>
									</TD>
								</TR>
							</TABLE>
						</asp:panel>
						</TD>
				</TR>
				<TR height="30">
					<TD align="center" height="30"><asp:button id="btn_ok" runat="server" Text="OK" 
                     CssClass="PULSANTE" onclick="btn_ok_Click"></asp:button>&nbsp;
						<asp:button id="btn_chiudi" runat="server" Text="CHIUDI" CssClass="PULSANTE" 
                     onclick="btn_chiudi_Click"></asp:button></TD>
				</TR>
      </TABLE>
   </form>
</body>
</html>
