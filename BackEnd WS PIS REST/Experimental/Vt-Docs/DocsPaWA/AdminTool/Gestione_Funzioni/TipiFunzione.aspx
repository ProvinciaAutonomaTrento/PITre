<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Page language="c#" Codebehind="TipiFunzione.aspx.cs" AutoEventWireup="false" MaintainScrollPositionOnPostback="true" Inherits="Amministrazione.Gestione_Funzioni.TipiFunzione" %>
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
		<script language="JavaScript">
			
			var cambiapass;
			var hlp;
			var _url_export = '<%=UrlExport %>';

			function apriPopup()
             {
				hlp = window.open('../help.aspx?from=FU','','width=450,height=500,scrollbars=YES');
			}
			function DivHeight()
			{
				if (DivDGList.scrollHeight< 157) 
					DivDGList.style.height=DivDGList.scrollHeight;
				else
					DivDGList.style.height=157;
			}
			function cambiaPwd() 
            {
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

            function OpenExportMacro(cod, tipo) 
            {

                var newLeft = (screen.availWidth / 2 - 225);
                var newTop = (screen.availHeight - 704);
                //var retval = window.showModalDialog(_url_export + "?id=" + cod, 'OpenExportMacro', 'dialogWidth:700px;dialogHeight:350px;status:no;resizable:no;scroll:yes;center:no;help:no;', '');
                //var url = _url_export + "?id=" + cod + '&tipo=' + tipo;
                var retval = window.open(_url_export + "?id=" + cod + "&tipo=MACRO_FUNZ", 'OpenExportMacro', 'width=410,height=300,scrollbars=NO,top=' + newTop + ',left=' + newLeft);
            }

            function OpenExportMicro(cod, tipo) {

                var newLeft = (screen.availWidth / 2 - 225);
                var newTop = (screen.availHeight - 704);
                //var retval = window.showModalDialog(_url_export + "?id=" + cod, 'OpenExportMacro', 'dialogWidth:700px;dialogHeight:350px;status:no;resizable:no;scroll:yes;center:no;help:no;', '');
                //var url = _url_export + "?id=" + cod + '&tipo=' + tipo;
                var retval = window.open(_url_export + "?id=" + cod + "&tipo=MICRO_FUNZ", 'OpenExportMicro', 'width=410,height=300,scrollbars=NO,top=' + newTop + ',left=' + newLeft);
            }



		</script>
        <script type="text/javascript">

            function checkValTxtRicerca() {
                var validator = document.getElementById("cvRicerca");
                var txt = document.getElementById("txt_ricerca");
                var ddl = document.getElementById("ddl_ricTipo")
                
                switch (ddl.options[ddl.selectedIndex].value) {

                    case 'Codice':
                        {
                            if (txt.value == "")
                                alert('inserire un Codice o parte di esso da ricercare');
                            return;
                            break
                        }
                    case 'Descrizione':

                        {
                            if (txt.value == "")
                                alert('inserire una Descrizione o parte di essa da ricercare');
                            return;

                            break
                        }

                    default:
                        {
                            
                            break
                        }
                }
            }

function checkRicerca() {

    var txt = document.getElementById("txt_ricerca");
    var ddl = document.getElementById("ddl_ricTipo")
switch (ddl.options[ddl.selectedIndex].value) {

    case 'T':
        {
            txt.value = "";
            txt.disabled = true;
            txt.style.backgroundColor = '#DCDCDC';
            break
        }
    case 'SEL':

        {
            txt.value = "";
            txt.disabled = true;
            txt.style.backgroundColor  = '#DCDCDC';
            break
        }

    default:
        {
            txt.disabled = false;
            txt.style.backgroundColor = '#FFFFFF';
            break
        }
            }


   }
        </script>

		<script language="javascript">
		
			function ShowValidationMessage(message,warning)
			{
				if (message!=null && message!='')
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
			}
			
		</script>
        <script type="text/javascript">
            function setGridScroll() {
                var xPos, yPos;
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                prm.add_beginRequest(BeginRequestHandler);
                prm.add_endRequest(EndRequestHandler);
            }
            function BeginRequestHandler(sender, args) {
                xPos = $get('divGridFunzioni').scrollLeft;
                yPos = $get('divGridFunzioni').scrollTop;
            }
            function EndRequestHandler(sender, args) {
                $get('divGridFunzioni').scrollLeft = xPos;
                $get('divGridFunzioni').scrollTop = yPos;
            }

</script>
	    <style type="text/css">
            .style1
            {
                font-weight: bold;
                font-size: 10px;
                color: #666666;
                font-family: Verdana;
                width: 10%;
            }
            .style2
            {
                width: 10%;
            }
        </style>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" onload="javascript: DivHeight();"
		rightMargin="0" onunload="ClosePopUp()" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Funzioni" />
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
					<td class="titolo" align="center" bgColor="#e0e0e0" height="20">Tipi funzione</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<table cellSpacing="0" cellPadding="0" width="700" align="center" border="0">
							<tr>
								<td align="center" height="25"></td>
							</tr>
							<tr>
								<td class="pulsanti" align="center" width="700">
									<table width="100%">
										<tr>
											<td align="left"><asp:label id="lbl_tit" CssClass="titolo" Runat="server">Lista tipi funzione</asp:label></td>
											<td align="right"><asp:button id="btn_nuova" runat="server" CssClass="testo_btn" Text="Nuovo tipo funzione"></asp:button>&nbsp;</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td align="center" width="100%">
									<DIV id="DivDGList" style="OVERFLOW: auto" align="center">
										<asp:datagrid id="dg_macroFunzioni" runat="server" Width="100%" BorderWidth="1px" CellPadding="1"
											BorderColor="Gray" AutoGenerateColumns="False">
											<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle CssClass="bg_grigioN"></ItemStyle>
											<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
											<Columns>
												<asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID"></asp:BoundColumn>
												<asp:BoundColumn DataField="Codice" HeaderText="Codice">
													<HeaderStyle HorizontalAlign="Center" Width="30%"></HeaderStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione">
													<HeaderStyle HorizontalAlign="Center" Width="60%"></HeaderStyle>
												</asp:BoundColumn>
												<asp:ButtonColumn Text="&lt;img src=../Images/lentePreview.gif border=0 alt='Dettaglio'&gt;" CommandName="Select">
													<HeaderStyle Width="5%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
												</asp:ButtonColumn>
												<asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 alt='Elimina'&gt;" CommandName="Delete">
													<HeaderStyle Width="5%"></HeaderStyle>
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
								<td align="center"><asp:panel id="pnl_info" Runat="server" Visible="False">
										<TABLE class="contenitore" width="100%">
											<TR>
												<TD colSpan="4">
													<TABLE cellSpacing="0" cellPadding="0" width="100%">
														<TR>
															<TD class="titolo_pnl">Dettaglio tipo funzione</TD>
															<TD class="titolo_pnl" align="right">
																<asp:ImageButton id="btn_chiudiPnlInfo" runat="server" ImageUrl="../Images/cancella.gif" ToolTip="Chiudi"></asp:ImageButton></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD class="style1" align="right">Codice *&nbsp;</TD>
												<TD width="25%">
													<asp:TextBox id="txt_codice" Runat="server" CssClass="testo" Width="150px" MaxLength="16"></asp:TextBox>
													<asp:Label id="lbl_cod" runat="server" CssClass="testo"></asp:Label></TD>
												<TD class="testo_grigio_scuro" align="right" width="20%">Descrizione *&nbsp;</TD>
												<TD width="45%">
													<asp:TextBox id="txt_descrizione" Runat="server" CssClass="testo" Width="300px" MaxLength="64"></asp:TextBox></TD>
											</TR>
                                            <tr>
                                                <td class="style1" align="right" colspan="4">
                                                     <asp:Button ID="btn_report_macro_funz" CssClass="testo_btn_p_large" runat="server" CommandName="MACRO_FUNZ"
                                                    onCommand="btn_export" text="Esporta" ToolTip="Esporta dettaglio tipo funzione" />
                                                </td>
                                            </tr>
                                            <TR>
												<TD colSpan="4"></TD>
											</TR>
											
											<TR>
												<TD class="titolo_pnl" vAlign="middle" align="left" colSpan="4">Elenco funzioni 
													elementari</TD>
											</TR>
                                            <tr>
                                            <td class="style2" > 
                                                <asp:DropDownList ID="ddl_ricTipo" Runat="server" AutoPostBack="false" 
                                                    CssClass="testo_grigio_scuro" 
                                                    Width="130px">
                                                    <asp:ListItem Text="Seleziona ..." Value="SEL"></asp:ListItem>
                                                    <asp:ListItem Text="Tutti" Value="T"></asp:ListItem>
                                                    <asp:ListItem Text="Codice" Value="Codice"></asp:ListItem>
                                                    <asp:ListItem Text="Descrizione" Value="Descrizione"></asp:ListItem>
                                                   
                                                </asp:DropDownList>
                                            </td>
                                            <td class="style2"  >
                                               <asp:textbox id="txt_ricerca"  
                                                    Runat="server" Width="97%" BackColor="Gainsboro" Enabled="False" 
                                                    ValidationGroup="ricerca"></asp:textbox>
			                                </td>
			                                <td align="left" style="padding-right:13px;">
				                                <asp:button id="btn_find" CssClass="testo_btn_p_large" Runat="server" 
                                                    Text="Cerca" OnClick="btn_find_Click"></asp:button>	
                                                </td>
                                            <td align="right">
                                                <asp:Button ID="btn_report_micro_funz" CssClass="testo_btn_p_large" runat="server" CommandName="MICRO_FUNZ"
                                                    onCommand="btn_export" text="Esporta" />
                                            </td>
                                            </tr>
											<TR>
												<TD width="100%" colSpan="4">
													<TABLE width="100%" align="center">
														<TR>
															<TD>
																<DIV id="divGridFunzioni" style="OVERFLOW: auto; HEIGHT: 128px">
																	<asp:DataGrid id="dg_funzioni" runat="server" AutoGenerateColumns="False" BorderColor="Gray" CellPadding="1"
																		BorderWidth="1px" Width="100%" Visible="False"  >
																		<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
																		<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
																		<ItemStyle CssClass="bg_grigioN"></ItemStyle>
																		<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
																		<Columns>
																			<asp:TemplateColumn>
																				<ItemTemplate>
																					<asp:CheckBox id="cbSel" runat="server" Checked="False"></asp:CheckBox>
																				</ItemTemplate>
																			</asp:TemplateColumn>
																			<asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione"></asp:BoundColumn>
																			<asp:BoundColumn DataField="Codice" HeaderText="Codice"></asp:BoundColumn>
																			<asp:BoundColumn Visible="False" DataField="TipoFunzione" HeaderText="TipoFunzione"></asp:BoundColumn>
																			<asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID"></asp:BoundColumn>
																			<asp:BoundColumn Visible="False" DataField="Associato" HeaderText="Associato"></asp:BoundColumn>
                                                                            <asp:ButtonColumn Text="&lt;img src=../Images/lentePreview.gif border=0 alt='Dettaglio'&gt;" CommandName="SELECT" >
													                            <HeaderStyle Width="5%"></HeaderStyle>
													                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
												                            </asp:ButtonColumn>
																		</Columns>
																	</asp:DataGrid></DIV>
															</TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD align="right" colSpan="4">
													<asp:CustomValidator ID="cvRicerca" runat="server" 
                                                        ControlToValidate="txt_ricerca" Display="None" ValidationGroup="ricerca"></asp:CustomValidator>
													<asp:Button id="btn_aggiungiTutti" runat="server" CssClass="testo_btn" Text="Seleziona tutti"></asp:Button>&nbsp;&nbsp;&nbsp;
													<asp:Button id="btn_aggiungi" runat="server" CssClass="testo_btn" 
                                                        Text="Aggiungi"></asp:Button>&nbsp;
												</TD>
											</TR>
										</TABLE>
									</asp:panel></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		<asp:ValidationSummary ID="vSummary" runat="server" DisplayMode="List" 
            ShowMessageBox="True" ShowSummary="False" />
		</form>
	</body>
</HTML>
