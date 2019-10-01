<%@ Page language="c#" Codebehind="AvvisoRispProtocollo.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.AvvisoRispProtocollo" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
	</HEAD>
	<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout">
		<form id="frmAvviso" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Avviso Risposta Protocollo" />
			<table class="contenitore" height="100%" cellSpacing="0" cellPadding="0" width="100%" align="center"
				border="0">
				<tr>
					<td align="center" colSpan="2"><br>
						<asp:image id="img_alert" ImageUrl="../images/alert.gif" Runat="server"></asp:image></td>
				</tr>
				<tr>
					<td align="center" colSpan="2" class="menu_1_rosso" style="FONT-SIZE: 14px">ATTENZIONE!</td>
				</tr>
				<tr vAlign="top">
					<td class="testo_grigio_scuro_grande" align="center">
						<table id="tbl_alert" height="100%" width="100%" border="0" runat="server">
							<tr>
								<td height="7"></td>
							</tr>
							<tr>
								<td><table class="menu_1_rosso_minuscolo">
										<tr>
											<td>
												<ul>
													<asp:panel id="pnl_mitt" Runat="server">
														<asp:label id="Label1" runat="server">
															<li>
																&nbsp; Destinatario e mittente non coincidono<br>
																<br>
															</li>
														</asp:label>
													</asp:panel>
                                                    <asp:panel id="pnl_ogg" Runat="server">
														<asp:label id="Label2" runat="server">
															<li>
																&nbsp; Gli oggetti non coincidono<br>
                                                                <br>
															</li>
														</asp:label>
													</asp:panel>
                                                     <asp:panel id="pnl_occ" Runat="server">
														<asp:label id="Label3" runat="server">
															<li>
																&nbsp; I documenti sono di registri differenti. I corrispondenti diventeranno occasionali<br>
															</li>
														</asp:label>
													</asp:panel>
                                                     <asp:panel id="pnl_occ_proto" Runat="server">
														<asp:label id="Label4" runat="server">
															<li>
																&nbsp; I documenti sono di registri differenti.<br>
															</li>
														</asp:label>
													</asp:panel>
                                                    </ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td></td>
							</tr>
							<tr>
								<td align="center" height="10" class="testo_grigio_scuro">COME SI DESIDERA 
									PROCEDERE?&nbsp;
								</td>
							</tr>
							<tr>
								<td height="3"></td>
							</tr>
							<TR>
								<TD><asp:panel id="pnl_rispNoProto" Runat="server" Visible="True"><table class="info_grigio" width="100%">
											<tr>
												<td>
													<asp:RadioButtonList id="rbl_scelta" Runat="server" Visible="True" RepeatDirection="Vertical" CssClass="testo_grigio_scuro">
														<asp:ListItem Value="N">Continua e sovrascrivi i dati</asp:ListItem>
														<asp:ListItem Value="Y">Continua utilizzando i dati immessi</asp:ListItem>
														<asp:ListItem Value="C">Seleziona un altro documento</asp:ListItem>
													</asp:RadioButtonList>
												</td>
											</tr>
										</table>
									</asp:panel></TD>
							</TR>
							<tr>
								<td><asp:panel Runat="server" ID="pnl_rispProto" Visible="False"><table class="info_grigio" width="100%">
											<tr>
												<td>
													<asp:radiobuttonlist id="rbl_scelta2" Runat="server" Visible="True" RepeatDirection="Vertical" CssClass="testo_grigio_scuro">
														<asp:ListItem Value="S">Continua</asp:ListItem>
														<asp:ListItem Value="C">Seleziona un altro documento</asp:ListItem>
													</asp:radiobuttonlist>
												</td>
											</tr>
										</table>
									</asp:panel></td>
							</tr>
                            <tr>
								<td><asp:panel Runat="server" ID="pnl_sovrascriviCorr" Visible="False"><table class="info_grigio" width="100%">
											<tr>
												<td>
													<asp:radiobuttonlist id="rbl_scelta3" Runat="server" Visible="True" RepeatDirection="Vertical" CssClass="testo_grigio_scuro">
														<asp:ListItem Value="Y">Continua</asp:ListItem>
														<asp:ListItem Value="C">Seleziona un altro documento</asp:ListItem>
													</asp:radiobuttonlist>
												</td>
											</tr>
										</table>
									</asp:panel></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td align="center"><!--<asp:button id="btn_si" Runat="server" Text="si, continua" CssClass="PULSANTE" Width="80px"></asp:button>&nbsp;
						<asp:button id="btn_no" Runat="server" Text="no, resetta" CssClass="PULSANTE" Width="80px"></asp:button>&nbsp;--><asp:button id="btn_ok" Runat="server" CssClass="PULSANTE" Width="80px" Text="OK"></asp:button>
						<asp:Button ID="btn_chiudi" CssClass="PULSANTE" Runat="server" Text="chiudi" Width="80px" Visible="False"></asp:Button>&nbsp;
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
