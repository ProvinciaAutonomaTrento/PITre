<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChiaviConfig.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_ChiaviConfig.ChiaviConfig" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<HTML>
	<HEAD runat = "server">
	    <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
        <SCRIPT language="JavaScript">
		
			var cambiapass;
			var hlp;
			
			function apriPopup() {
				hlp = window.open('../help.aspx?from=RU','','width=450,height=500,scrollbars=YES');
			}
			function DivHeight()
			{
				if (DivDGList.scrollHeight< 229) 
					DivDGList.style.height=DivDGList.scrollHeight;
				else
					DivDGList.style.height=229;
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
	</SCRIPT>
		<SCRIPT language="JavaScript">
		
			function ShowValidationMessage(message,warning)
			{
				if (warning)
				{
					if (window.confirm(message + "\n\nContinuare?"))
					{
						Form1.submit();
					}
				}
				else
				{
					alert(message);
				}
			}
		
			// Permette di inserire solamente caratteri numerici
			function ValidateNumericKey()
			{
				var inputKey=event.keyCode;
				var returnCode=true;
				
				if(inputKey > 47 && inputKey < 58)
				{
					return;
				}
				else
				{
					returnCode=false; 
					event.keyCode=0;
				}
				
				event.returnValue = returnCode;
			}
					
		</SCRIPT>
	    <style type="text/css">
            .style1
            {
                font-weight: bold;
                font-size: 10px;
                color: #666666;
                font-family: Verdana;
                width: 268435328px;
            }
        </style>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" onload="javascript: DivHeight();"
		rightMargin="0" onunload="ClosePopUp()">
		<form id="Form1" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Chiavi Configurazione" />
			<!-- Gestione del menu a tendina --><uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
			<table height="100%" cellSpacing="1" cellPadding="0" width="100%" border="0">
				<tr>
					<td>
						<!-- TESTATA CON MENU' --><uc1:testata id="Testata" runat="server"></uc1:testata></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20"><asp:label id="lbl_position" runat="server"></asp:label></td>
				</tr>
				<tr>
					<!-- TITOLO PAGINA -->
					<td class="titolo" align="center" bgColor="#e0e0e0" height="20">Chiavi configurazione</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<table cellSpacing="0" cellPadding="0" align="center" border="0" width="97%">
							<tr>
								<td align="center" height="25"></td>
							</tr>
							<tr>
								<td class="pulsanti" align="center">
									<table width="100%">
										<tr>
											<td align="left"><asp:label id="lbl_tit" CssClass="titolo" Runat="server">Lista 
                                                chiavi configurazione</asp:label></td>
											<td align="right">&nbsp;</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td style="HEIGHT: 157px" align="center" valign="top">
									<DIV id="DivDGList" style="OVERFLOW: auto; HEIGHT: 150px">
										<asp:datagrid id="dg_chiavi" runat="server" BorderColor="Gray" CellPadding="1" BorderWidth="1px"
											AutoGenerateColumns="False" Width="100%" onselectedindexchanged="dg_chiavi_SelectedIndexChanged">
											<SelectedItemStyle HorizontalAlign="Left" CssClass="bg_grigioS"></SelectedItemStyle>
											<EditItemStyle HorizontalAlign="Left"></EditItemStyle>
											<AlternatingItemStyle HorizontalAlign="Left" CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle HorizontalAlign="Left" CssClass="bg_grigioN"></ItemStyle>
											<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
											<Columns>
												<asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID">
													<HeaderStyle Width="0px"></HeaderStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="ID_AMMINISTRAZIONE" HeaderText="Amministrazione" Visible=false>
													<HeaderStyle Width="1%"></HeaderStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="CODICE" HeaderText="Codice" Visible=true>
													<HeaderStyle Width="15%"></HeaderStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="DESCRIZIONE" HeaderText="Descrizione">
													<HeaderStyle Width="55%"></HeaderStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="VALORE" HeaderText="Valore">
													<HeaderStyle Width="20%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="TIPO" HeaderText="Tipo">
													<HeaderStyle Width="15%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
												</asp:BoundColumn>
												<asp:ButtonColumn Text="&lt;img src=../Images/lentePreview.gif border=0 alt='Dettaglio'&gt;" CommandName="Select">
													<HeaderStyle Width="5%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
												</asp:ButtonColumn>
												<asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 alt='Elimina'&gt;" CommandName="Delete" Visible=false>
													<HeaderStyle Width="1%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
												</asp:ButtonColumn>
											</Columns>
										</asp:datagrid>
									</DIV>
								</td>
							</tr>
							<tr>
								<td height="10"></td>
							</tr>
							<tr>
								<td vAlign="top">
									<asp:panel id="pnl_info" Runat="server" Visible="False">
										<TABLE class="contenitore" width="80%" >
											<TR>
												<TD colSpan="2">
													<TABLE cellSpacing="0" cellPadding="0" width="100%">
														<TR>
															<TD class="titolo_pnl" align="left">Dettagli chiave</TD>
															<TD class="titolo_pnl" align="right">
																<asp:ImageButton id="btn_chiudiPnlInfo" runat="server" ToolTip="Chiudi" ImageUrl="../Images/cancella.gif"></asp:ImageButton></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD width="35%">
													<TABLE width="100%">
														<TR>
															<TD class="testo_grigio_scuro" vAlign="middle" align="left" width="40%">Codice*&nbsp;</TD>
															<TD vAlign="top" align="left"  width="60%">
                                                                <asp:Label id="lbl_cod" runat="server" CssClass="testo" Width="150px"></asp:Label>
																<asp:TextBox id="txt_codice" Runat="server" CssClass="testo" Width="10px" MaxLength="20"></asp:TextBox>
															</TD>
														</TR>
													</TABLE>
												</TD>
												<TD width="65%">
													<TABLE width="100%">
														<TR>
															<TD class="testo_grigio_scuro" align="left" width="40%">Descrizione*&nbsp;</TD>
															<TD align="left" width="60%">
                                                                <asp:TextBox id="txt_descrizione" Runat="server" CssClass="testo" Width="400px" MaxLength="256" TextMode=MultiLine ReadOnly=true></asp:TextBox></TD>
														</TR>
													</TABLE>
												</TD> 
											</TR>
											<tr>
											<td width="30%">
											    <TABLE width="100%">
											        <TR>
													    <TD class="testo_grigio_scuro" align="left"  width="40%">Tipo chiave*&nbsp;</TD>
														<TD align="left"  width="60%" valign="top">
                                                            <asp:Label id="lbl_tipo_chiave" runat="server" CssClass="testo" Width="150px"></asp:Label></TD>
													</TR>
												</TABLE>
											    
											</td>
											<td width="70%">
											   <TABLE width="100%">
											        <TR>
													    <TD class="testo_grigio_scuro" align="left"  width="30%">Valore*&nbsp;</TD>
														<TD align="left"  width="70%"><asp:TextBox id="txt_valore" Runat="server" CssClass="testo" Width="400px" MaxLength="256" ></asp:TextBox></TD>
													</TR>
												</TABLE>
											</td>
											</tr>
											<TR>
												<TD align="left" colSpan="2">
													<TABLE cellSpacing="0" cellPadding="0" width="100%">
														<TR>
															<TD align="left" width="70%">
                                                                <asp:Label ID="lbl_idAmm" runat="server" CssClass="testo" 
                                                Text="0" Visible="False"></asp:Label>
                                                            </TD>
															<TD vAlign="bottom" align="right" width="30%">
																&nbsp;&nbsp;&nbsp;&nbsp;
																<asp:Button id="btn_salva" runat="server" 
                                                CssClass="testo_btn" Text="Salva" onclick="btn_salva_Click"></asp:Button>&nbsp;
															</TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
										</TABLE>
									</asp:panel>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
</body>
</html>
