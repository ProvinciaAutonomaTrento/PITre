<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cache.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_Cache.Cache" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title></title>
   <LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
		<SCRIPT language="JavaScript">			
			
			var cambiapass;
			var hlp;

			function Init() {
			
			}				
			function apriPopup() {
				hlp = window.open('../help.aspx?from=HP','','width=450,height=500,scrollbars=YES');
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
		<script language="javascript" id="btn_elimina_click" event="onclick()" for="btn_elimina">
			window.document.body.style.cursor='wait';
		</script>
		<script language="javascript" id="btn_salva_click" event="onclick()" for="btn_salva">
			window.document.body.style.cursor='wait';
		</script>
	<style type="text/css">
        .style1
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 204px;
        }
        .style2
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 287px;
        }
        </style>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" 
		rightMargin="0" onunload="Init();ClosePopUp()" text="Orar">
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > HomePage" />
			<input id="hd_idAmm" type="hidden" name="hd_idAmm" runat="server"> 
			<!-- Gestione del menu a tendina --><uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
			<table height="100%" cellSpacing="1" cellPadding="0" width="100%" border="0">
				<tr>
					<td>
						<!-- TESTATA CON MENU' --><uc1:testata id="Testata" runat="server"></uc1:testata></td>
				</tr>
				<tr>
					<!-- STRISCIA SOTTO IL MENU -->
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20"><asp:label id="lbl_position" runat="server"></asp:label></td>
				</tr>
				<tr>
					<!-- STRISCIA DEL TITOLO DELLA PAGINA -->
					<td class="titolo" align="center" bgColor="#e0e0e0" height="20">Gestione Cache</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" colSpan="2" height="25"><asp:label id="lbl_avviso" runat="server" CssClass="testo_rosso"></asp:label></td>
							</tr>
							<tr>
								<td vAlign="top" width="650">
									<table width="100%" border="0">
										<tr>
											<td align="center">
											<asp:panel id="pnl_info" Runat="server" Visible="True">
													<TABLE class="contenitore" cellSpacing="4" cellPadding="0" width="100%" 
                                                        ID="cb_cache"><TR>
															<TD colSpan="2">
																<TABLE cellSpacing="0" cellPadding="0" width="100%">
																	<TR>
																		<TD class="titolo_pnl" align="left">
																			<asp:Label id="lbl_titolo_pnl" Runat="server"></asp:Label></TD>
																	</TR>
																</TABLE>
															</TD>
														</TR>
                                                        <TR>
															<TD class="style2" align="left">
                                                                <asp:Label ID="Label2" runat="server" Text="Orario di sincronizzazione"></asp:Label>
                                                            </TD>
															<TD class="style1" align="left">
																inizio&#160;
                                                            
                                                                <asp:TextBox ID="txt_ora_start" runat="server" CssClass="testo" MaxLength="2" Width="22px" ></asp:TextBox>&#160;:&#160;
                                                                <asp:TextBox ID="txt_min_start" runat="server" CssClass="testo" MaxLength="2" Width="22px"></asp:TextBox>&#160; 
                                                              
                                                                   fine&#160;
															
                                                            	<asp:TextBox ID="txt_ora_fine" runat="server" CssClass="testo" MaxLength="2" Width="22px"></asp:TextBox>&#160;:&#160;
                                                                <asp:TextBox ID="txt_min_fine" runat="server" CssClass="testo" MaxLength="2" Width="22px"></asp:TextBox>
                                                              
                                                           </TD>

														</TR>
														<TR>
															<TD class="style2" align="left">
                                                                <asp:Label id="lbl_segnatura" CssClass="testo" Runat="server">Massima dimesione della cache (MByte) *</asp:Label></TD>
															<TD class="style1" align="left">
																<asp:textbox id="txt_dim_cache" runat="server" CssClass="testo" 
                                                            Width="130px" MaxLength="20"></asp:textbox>
                                                                &#160; 
                                                                <asp:CheckBox ID="ck_dim_cache_infinito" runat="server" AutoPostBack= "true"
                                                            onprerender="ck_dim_cache_infinito_PreRender" />
                                                                <asp:Label ID="Label4" runat="server" CssClass="testo" 
                                                            Text="Nessun Limite"></asp:Label>
                                                            </TD>
														</TR><TR>
															<TD class="style2" align="left">
                                                                <asp:Label ID="lbl_timbro_su_pdf" CssClass="testo" Runat="server">Massima dimensione del file da trasferire (KByte)*</asp:Label></TD>
															<TD class="style1" align="left" >
																<asp:textbox id="txt_dim_file" runat="server" CssClass="testo" 
                                                            Width="130px" MaxLength="20"></asp:textbox>&nbsp;&nbsp; 
                                                                <asp:CheckBox ID="ck_dim_file_infinito" runat="server" 
                                                            onprerender="ck_dim_file_infinito_PreRender" AutoPostBack="true" />
                                                                <asp:Label ID="Label5" runat="server" CssClass="testo" 
                                                            Text="Nessun Limite"></asp:Label>
                                                            </TD>
														</TR><TR>
															<TD class="style2" align="left">
                                                                <asp:Label ID="Label1" CssClass="testo" Runat="server">Doc 
                                                                root per il trasferimento dei file *</asp:Label></TD>
															<TD class="style1" align="left">
																<asp:textbox id="txt_doc_root" runat="server" CssClass="testo" 
                                                            Width="241px" MaxLength="255" ></asp:textbox>&#160;</TD>
														</TR>
														<TR>
															<TD class="style2" align="left">
                                                                <asp:Label ID="Label6" CssClass="testo" Runat="server">URL webservice del 
                                                                distretto generale*</asp:Label></TD>
															<TD class="style1" align="left">
																<asp:textbox id="txt_ws_generale" runat="server" CssClass="testo" 
                                                            Width="241px" MaxLength="255" ></asp:textbox>&#160;</TD>
														</TR>
														<TR>
															<TD class="style2" align="left">
                                                                <asp:Label ID="Label7" CssClass="testo" Runat="server">URL
                                                                webservice caching del server locale*</asp:Label></TD>
															<TD class="style1" align="left">
																<asp:textbox id="txt_wsLocale" runat="server" CssClass="testo" 
                                                            Width="241px" MaxLength="255" ></asp:textbox>&#160;</TD>
														</TR>
														<TR>
															<TD class="style2" align="left">
                                                                <asp:Label ID="Label8" CssClass="testo" Runat="server">Doc root del server locale*
                                                                </asp:Label></TD>
															<TD class="style1" align="left">
																<asp:textbox id="txt_docLocale" runat="server" CssClass="testo" 
                                                            Width="241px" MaxLength="255" ></asp:textbox>&#160;</TD>
														</TR>
														<TR>
															<TD class="style2" align="left">
                                                                &#160;Attiva
                                                                Caching</TD>
															<TD class="style1" align ="left" />
																<asp:CheckBox runat="server" ID="cb_cache" />
                                                                &#160; 
                                                            </TD>
														</TR><asp:Panel ID="pnl_protocolloTit" runat="server" Visible="false"><TR>
															<TD class="testo_grigio_scuro" align="right">
                                                                <asp:Label id="lbl_protocolloTit" CssClass="testo" Runat="server"></asp:Label></TD>
															<TD>
																<asp:textbox id="txt_protocolloTit" runat="server" CssClass="testo" 
                                                            Width="392px" MaxLength="255"></asp:textbox>&#160;
																<asp:ImageButton id="btn_protocolloTit" runat="server" 
                                                            ToolTip="Seleziona Opzioni" ImageUrl="../../images/proto/RispProtocollo/freccina_dx.gif"
																	ImageAlign="AbsBottom" ></asp:ImageButton></TD>
														</TR></asp:Panel><TR>
															<TD align="left" colSpan="2">
																<TABLE cellSpacing="0" cellPadding="0" width="100%">
																	<TR>
																		<TD align="left" width="55%">
																			<asp:Label id="lbl_msg" runat="server" CssClass="testo_rosso"></asp:Label></TD>
																		<TD align="right">
																			<asp:Button id="btn_annulla" runat="server" 
                                                            CssClass="testo_btn" Width="60"
																				Text="Annulla" onclick="btn_annulla_Click"></asp:Button>
																			<asp:Button id="btn_elimina" runat="server"  OnClientClick="return confirm('Sei sicuro di eliminare la configurazione');" 
                                                            CssClass="testo_btn" Visible="False" Width="60"
																				Text="Reset" onclick="btn_elimina_Click"></asp:Button>&#160;&#160;&#160;
																			<asp:button id="btn_salva" runat="server" 
                                                            CssClass="testo_btn" Width="60" Text="Salva" onclick="btn_salva_Click"></asp:button>&#160;
																		
																		
																		</TD>
																	</TR>
																</TABLE>
															</TD>
														</TR></TABLE>
											</asp:panel>
											</td>
										</tr>
									</table>
								</td>
								<td width="10">&nbsp;</td>
							</tr>
						</table>
					</td>
					<td>
					 <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="msg_elimina" 
                            runat="server" ongetmessageboxresponse="msg_elimina_GetMessageBoxResponse">
                 </cc2:MessageBox>
                 <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="modifica_effettuata" 
                            runat="server">
                 </cc2:MessageBox>
					</td>
				</tr>
			</table>
		</form>

</body>
</html>
