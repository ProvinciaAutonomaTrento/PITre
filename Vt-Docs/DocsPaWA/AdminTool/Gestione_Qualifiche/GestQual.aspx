<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Page language="c#" Codebehind="GestQual.aspx.cs" AutoEventWireup="true" Inherits="DocsPAWA.AdminTool.Gestione_Qualifiche.GestQual" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat = "server">
    	<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
		<script language="javascript" src="../../LIBRERIE/rubrica.js"></script>
		<script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
		<SCRIPT language="JavaScript">

		    var cambiapass;
		    var hlp;

		    function apriPopup() {
		        window.open('../help.aspx?from=RG', '', 'width=450,height=500,scrollbars=YES');
		    }
		    function cambiaPwd() {
		        window.open('../CambiaPwd.aspx', '', 'width=410,height=300,scrollbars=NO');
		    }
		    function ClosePopUp() {
		        if (typeof (cambiapass) != 'undefined') {
		            if (!cambiapass.closed)
		                cambiapass.close();
		        }
		        if (typeof (hlp) != 'undefined') {
		            if (!hlp.closed)
		                hlp.close();
		        }
		    }
		
			
			
		</SCRIPT>
		
		<SCRIPT language="JavaScript">

		    function ShowValidationMessage(message, warning) {
		        if (warning) {
		            if (window.confirm(message + "\n\nContinuare?")) {
		                Form1.txtCommandPending.value = 'DELETE';
		                Form1.submit();
		            }
		            else {
		                Form1.txtCommandPending.value = '';
		            }
		        }
		        else {
		            alert(message);
		        }
		    }

		</SCRIPT>

	    <style type="text/css">
            .style1
            {
                font-weight: bold;
                font-size: 12px;
                color: #4b4b4b;
                font-family: Verdana;
                width: 136%;
            }
            .style2
            {
                font-weight: bold;
                font-size: 10px;
                color: #666666;
                font-family: Verdana;
                width: 20%;
            }
            .style3
            {
                width: 369px;
            }
        </style>

	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" onunload="ClosePopUp()"		>
		<form id="Form1" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Qualifiche" />
			<input id="txtCommandPending" type="hidden" runat="server">  
			<!-- Gestione del menu a tendina --><uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>

            <!-- INIZIO TABELLA FORMATTAZIONE PAGINA -->
			<table height="100%" cellSpacing="1" cellPadding="0" width="100%" border="0">
				<tr>
					<td>
						<!-- TESTATA CON MENU' --><uc1:testata id="Testata" runat="server"></uc1:testata>
                    </td>
				</tr>
				<tr>
					<!-- STRISCIA SOTTO IL MENU -->
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20">
                        <asp:label id="lbl_position" runat="server"></asp:label>
                    </td>
				</tr>
				<tr>
					<!-- TITOLO PAGINA -->
					<td class="titolo" align="center" width="100%" bgColor="#e0e0e0" style="height: 20px">Qualifiche
                    </td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<!-- INIZIO TABELLA CONTENUTI -->
                        <table cellSpacing="0" cellPadding="0" align="center" border="0">
								<tr>
									<td align="center" height="25">
                                    </td>
								</tr>
								<tr>
									<td class="pulsanti" align="center" width="700">
										<table width="100%">
											<tr>
												<td class="titolo" align="left"><asp:label id="lbl_tit" CssClass="titolo" Runat="server">
													Lista Qualifiche
												</asp:label></td>
												<td align="right"><asp:button id="btn_nuova" runat="server" CssClass="testo_btn" Text="Nuova Qualifica"></asp:button>&nbsp;</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td height="5"></td>
								</tr>
                                <tr>
									<td style="HEIGHT: 157px" align="center">
										<DIV id="DivDGList" style="OVERFLOW: auto; WIDTH: 720px; HEIGHT: 151px">
											<asp:datagrid id="dg_GQ" runat="server" Width="100%" BorderWidth="1px" CellPadding="1" BorderColor="Gray"
												AutoGenerateColumns="False">
												<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
												<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
												<ItemStyle CssClass="bg_grigioN"></ItemStyle>
												<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
												<Columns>
													<asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID">
														<HeaderStyle Width="0%"></HeaderStyle>
													</asp:BoundColumn>
													<asp:BoundColumn DataField="Codice" HeaderText="Codice">
														<HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
													</asp:BoundColumn>
													<asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione">
														<HeaderStyle HorizontalAlign="Center" Width="50px"></HeaderStyle>
													</asp:BoundColumn>
													<asp:ButtonColumn Text="&lt;img src=../Images/lentePreview.gif border=0 alt='Dettaglio'&gt;" CommandName="Select">
														<HeaderStyle Width="5%"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center"></ItemStyle>
													</asp:ButtonColumn>
													<asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 alt='Elimina'&gt;" CommandName="Delete">
														<HeaderStyle Width="5%"></HeaderStyle>
														<ItemStyle HorizontalAlign="Center"></ItemStyle>
													</asp:ButtonColumn>
													<asp:BoundColumn Visible="False" DataField="ID_AMM" HeaderText="ID_AMM">
														<HeaderStyle Width="0%"></HeaderStyle>
													</asp:BoundColumn>
												</Columns>
											</asp:datagrid></DIV>
									</td>
								</tr>
								<tr>
									<td height="10"></td>
								</tr>

                                <tr>
									<td vAlign="top">
										<asp:panel id="pnl_info" Runat="server" Visible="false">
											<TABLE class="contenitore" width="100%" >
												<TR>
													<TD colSpan="2">
														<TABLE cellSpacing="0" cellPadding="0" width="100%">
															<TR>
																<TD class="titolo_pnl" align="center">Dettagli Qualifica</TD>
															</TR>
														</TABLE>
													</TD>
												</TR>
												<TR>
													<TD colSpan="2" height=30px>
														<TABLE cellSpacing="0" cellPadding="0" width="100%">
															<TR>
																<TD class="testo_grigio_scuro" align="right" width="100">Codice *&nbsp;</TD>
																<TD>
																	<asp:TextBox id="txt_codice" Runat="server" CssClass="testo" Width="200px" MaxLength="16"></asp:TextBox>
																	<asp:Label id="lbl_cod" runat="server" CssClass="testo"></asp:Label></TD>
															</TR>
														</TABLE>
													</TD>
												</TR>
												<TR>
													<TD colSpan="2" style="height: 30px">
														<TABLE cellSpacing="0" cellPadding="0" width="100%">
															<TR>
																<TD class="testo_grigio_scuro" align="right" width="100">Descrizione *&nbsp;</TD>
																<TD>
																	<asp:TextBox id="txt_descrizione" Runat="server" CssClass="testo" Width="250px" MaxLength="128"></asp:TextBox></TD>
															</TR>
														</TABLE>
													</TD>
												</TR>
                                                
												
												<TR>
													<TD align="left" colSpan="3">
														<TABLE cellSpacing="0" cellPadding="0" width="100%">
															<TR>
																<TD align="left" class="style3">
																	<asp:Label id="lbl_msg" runat="server" CssClass="testo_rosso"></asp:Label></TD>
																<TD align="right">
																	<asp:Button id="btn_aggiungi" runat="server" CssClass="testo_btn" Text="Aggiungi"></asp:Button>&nbsp;
																</TD>
															</TR>
														</TABLE>
													</TD>
												</TR> 
                                        </TABLE>
									</asp:panel>
								</td></tr>
								<tr><td><cc2:messagebox id="msg_confirmModificaRF" runat="server"></cc2:messagebox></td></tr>




								
							</table>
                            <!-- FINE TABELLA CONTENUTI -->					
					</td>
				</tr>
				
			</table>
            <!-- FINE TABELLA FORMATTAZIONE PAGINA -->            
          
        
		</form>
	</body>
</HTML>
