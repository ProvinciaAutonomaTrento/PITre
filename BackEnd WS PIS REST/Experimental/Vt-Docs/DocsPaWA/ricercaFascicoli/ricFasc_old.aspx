<%@ Page language="c#" Codebehind="ricFasc_old.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.fascicoli.browsingFasc_old"%>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary"%>
<%@ Register TagPrefix="mytree"
Namespace="Microsoft.Web.UI.WebControls" 
Assembly="Microsoft.Web.UI.WebControls" %>
<%@ import namespace="Microsoft.Web.UI.WebControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > fascicoli_sx</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/DocsPA.css" type="text/css" rel="stylesheet">
		<SCRIPT language="JavaScript">

ns=window.navigator.appName == "Netscape"
ie=window.navigator.appName == "Microsoft Internet Explorer"



function openDesc() {

try
{
			if(ns) {

			showbox= document.layers[1]
				showbox.visibility = "show";
				// showbox.top=63;
			       
				var items = 1     ;
				for (i=1; i<=items; i++) {
				elopen=document.layers[i]
					if (i != (1)) { 
					elopen.visibility = "hide" }
						}
			}    

			if(ie) {
			curEl = event.toElement
			// curEl.style.background = "#C08682"   

			showBox = document.all.descreg;
				showBox.style.visibility = "visible";

			}
   }
   catch(e)
   {return false;}
}

function closeDesc() {


try{			
				var items = 1 
				for (i=0; i<items; i++) {
				if(ie){
					document.all.descreg.style.visibility = "hidden"
					
			         
				}
				if(ns){ document.layers[i].visibility = "hide"}          
			}
}
 
   catch(e)
   {return false;}
}


		</SCRIPT>
	</HEAD>
	<body leftMargin="1" topMargin="1" MS_POSITIONING="GridLayout">
		<form id="fascicoli_sx" method="post" runat="server">
			<table id="tbl_contenitore" height="100%" cellSpacing="0" cellPadding="0" width="410" align="center" border="0">
				<TBODY>
					<tr height="5%">
						<td>
							<TABLE height="100%" cellSpacing="0" cellPadding="0" width="410" border="0">
								<TBODY>
									<TR>
										<TD class="menu_2_bianco" bgColor="#4b4b4b" colSpan="2" height="11">&nbsp;Ruolo</TD>
										<TD vAlign="top" width="1" background="../images/tratteggio_bn_v.gif" bgColor="#9e9e9e" rowSpan="2"><IMG height="6" src="../images/tratteggio_bn_v.gif" width="1" border="0">
										</TD>
										<TD class="menu_2_bianco" bgColor="#4b4b4b">&nbsp; Registro</TD>
										<TD vAlign="top" width="1" background="../images/tratteggio_bn_v.gif" bgColor="#9e9e9e" rowSpan="2"><IMG height="6" src="../images/tratteggio_bn_v.gif" width="1" border="0"></TD>
										<TD class="menu_2_bianco" bgColor="#4b4b4b">&nbsp;Stato</TD>
										<TD vAlign="top" width="1" background="../images/tratteggio_bn_v.gif" bgColor="#9e9e9e" rowSpan="2"><IMG height="6" src="../images/tratteggio_bn_v.gif" width="1" border="0">
										</TD>
									</TR>
									<TR>
										<TD class="testo_grigio" width="1" bgColor="#ffffff" height="31"><IMG height="2" src="../images/spaziatore.gif" width="1" border="0">
										</TD>
										<TD class="menu_1_grigio" vAlign="center" width="135" bgColor="#ffffff" height="20"><asp:label id="lbl_ruolo" runat="server" CssClass="menu_1_grigio"></asp:label></TD>
										<TD class="testo_grigio" vAlign="center" width="199" bgColor="#ffffff">&nbsp;
											<asp:dropdownlist id="ddl_registri" runat="server" CssClass="testo_grigio" Width="134px" AutoPostBack="True"></asp:dropdownlist>&nbsp;
											<asp:image id="icoReg" onmouseover="openDesc()" onmouseout="closeDesc()" runat="server" ImageAlign="AbsMiddle" ImageUrl="../images/proto/ico_registro.gif" BorderWidth="0px" BorderStyle="Solid" BorderColor="Gray"></asp:image></TD>
										<TD vAlign="top" width="67" bgColor="#ffffff">
											<DIV align="right">
												<TABLE height="2" cellSpacing="1" cellPadding="0" width="67" border="0">
													<TBODY>
														<TR vAlign="top" height="2">
															<TD vAlign="top" align="right" height="1"><asp:image id="img_statoReg" runat="server" ImageUrl="../images/stato_giallo.gif"></asp:image></TD>
														</TR>
													</TBODY>
												</TABLE>
											</DIV>
										</TD>
									</TR>
									<TR>
										<TD bgColor="#ffffff" colSpan="7" height="3">
											<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
												<TBODY>
													<TR>
														<TD background="../images/tratteggio_gri_o.gif"><IMG height="1" src="../images/tratteggio_gri_o.gif" width="6" border="0"></TD>
													</TR>
												</TBODY>
											</TABLE>
										</TD>
									</TR>
								</TBODY>
							</TABLE>
						</td>
					</tr>
					<tr>
						<td class="testoBlue" borderColor="background" bgColor="buttonface" height="31%">
							<table cellSpacing="0" cellPadding="0" width="100%" border="0">
								<tr>
									<td noWrap align="left" colSpan="2" height="30" CssClass="testo_grigio">&nbsp;&nbsp;<asp:label id="Label1" CssClass="testo_grigio" Runat="server">Codice</asp:label>
										<asp:textbox id="txt_RicTit" CssClass="testo_grigio" Width="100px" Runat="server"></asp:textbox>&nbsp;&nbsp;
										<cc1:imagebutton id="btn_ricTit" ImageAlign="AbsMiddle" ImageUrl="../images/proto/zoom.gif" BorderWidth="1px" BorderStyle="Solid" BorderColor="Gray" Runat="server" AlternateText="Ricerca titolario" DisabledUrl="../images/proto/zoom.gif"></cc1:imagebutton></td>
									<td><asp:Label ID="lbl_result" Runat="server" Visible="False" CssClass="testo_grigio"></asp:Label></td>
								</tr>
								<tr>
									<td colSpan="3"><mytree:treeview id="Titolario" runat="server" CssClass="testo_grigio" ImageUrl="../images/alberi/titolario/mini_nodo.gif" SelectedStyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color:#c08682;" Height="150px" width="410px" SystemImagesPath="../images/alberi/left/" NAME="Treeview1" DefaultStyle="font-weight:normal;font-size:10px;color:#666666;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color:#d9d9d9;" HoverStyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana, Arial, Helvetica, sans-serif;background-color:#c08682;"></mytree:treeview></td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td width="410" bgColor="#f2f2f2" height="44%">
							<table cellSpacing="0" cellPadding="0" width="410" border="0">
								<TBODY>
									<TR>
										<TD class="testo_grigio"><asp:dropdownlist id="DDLFiltro1" runat="server" CssClass="testo_grigio" Width="205px" Height="100" Enabled="False"></asp:dropdownlist></TD>
										<TD style="HEIGHT: 21px"><asp:textbox id="TextFiltro1" runat="server" CssClass="testo_grigio" Width="190px" Enabled="False"></asp:textbox></TD>
									</TR>
									<TR>
										<TD class="testo_grigio"><asp:dropdownlist id="DDLFiltro2" runat="server" CssClass="testo_grigio" Width="205px" Height="100" Enabled="False"></asp:dropdownlist></TD>
										<TD style="HEIGHT: 27px"><asp:textbox id="TextFiltro2" runat="server" CssClass="testo_grigio" Width="190px" Enabled="False"></asp:textbox></TD>
									</TR>
									<TR>
										<TD class="testo_grigio" style="HEIGHT: 25px"><asp:dropdownlist id="DDLFiltro3" runat="server" CssClass="testo_grigio" Width="205px" Height="100" Enabled="False"></asp:dropdownlist></TD>
										<TD style="HEIGHT: 25px"><asp:textbox id="TextFiltro3" runat="server" CssClass="testo_grigio" Width="190px" Enabled="False"></asp:textbox></TD>
									</TR>
									<TR>
										<TD class="testo_grigio"><asp:dropdownlist id="DDLFiltro4" runat="server" CssClass="testo_grigio" Width="205px" Height="100" Enabled="False"></asp:dropdownlist></TD>
										<TD style="HEIGHT: 25px"><asp:textbox id="TextFiltro4" runat="server" CssClass="testo_grigio" Width="190px" Enabled="False"></asp:textbox></TD>
									</TR>
									<TR>
										<TD class="testo_grigio"><asp:dropdownlist id="DDLFiltro5" runat="server" CssClass="testo_grigio" Width="205px" Height="100" Enabled="False"></asp:dropdownlist></TD>
									</TR>
								</TBODY>
							</table>
						</td>
					</tr>
				</TBODY>
			</table>
		</form>
	</body>
</HTML>
