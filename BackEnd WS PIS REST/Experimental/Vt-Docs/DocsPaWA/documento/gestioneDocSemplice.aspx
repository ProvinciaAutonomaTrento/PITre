<%@ Page language="c#" Codebehind="gestioneDocSemplice.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.gestioneDocSemplice" %>
<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa.css" type="text/css" rel="stylesheet">
		<script language="javascript">
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

		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout" leftmargin="0" topmargin="0">
		<form id="gestioneDocSemplice" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione Documento Semplice" />
			<table cellpadding="0" cellspacing="0" width="100%" height="100%" border="0">
				<tr valign="top" height="100%">
					<td align="middle" width="100%" valign="top" height="100%">
						<table width="100%" height="100%">
							<tr height="5%">
								<td align="middle">
									<TABLE height="100%" cellSpacing="0" cellPadding="0" width="450" border="0">
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
												<TD class="testo_grigio" width="5" bgColor="#ffffff" height="30"><IMG height="2" src="../images/spaziatore.gif" width="5" border="0">
												</TD>
												<TD class="menu_1_grigio" vAlign="center" width="150" bgColor="#ffffff" height="20"><asp:label id="lbl_ruolo" runat="server" CssClass="menu_1_grigio"></asp:label></TD>
												<TD class="testo_grigio" vAlign="center" width="199" bgColor="#ffffff">&nbsp;
													<asp:dropdownlist id="ddl_registri" runat="server" CssClass="testo_grigio" AutoPostBack="True" Width="134px"></asp:dropdownlist><asp:label id="lbl_registri" runat="server" Width="138px"></asp:label>&nbsp;<asp:image id="icoReg" onmouseout="closeDesc()" onmouseover="openDesc()" runat="server" BorderColor="Gray" BorderStyle="Solid" BorderWidth="0px" ImageUrl="../images/proto/ico_registro.gif" ImageAlign="AbsMiddle"></asp:image></TD>
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
							<tr height="95%">
								<td>
									<cf1:IFrameWebControl id="iFrame_sx" runat="server" Marginwidth="0" Marginheight="0" iWidth="100%" iHeight="100%" Frameborder="0" Scrolling="no"></cf1:IFrameWebControl>
								</td>
							</tr>
						</table>
					</td>
					<!--
					<td width="1"><img border="0" src="../images/spaziatore.gif" width="1"></td>
					<TD vAlign="top" width="1" background="../images/tratteggio_bn_v.gif" bgColor="#9e9e9e" rowSpan="2" height="100%"><IMG height="6" src="../images/tratteggio_bn_v.gif" width="1" border="0"></TD>
					<td width="137" style="WIDTH: 137px"><img border="0" src="../images/spaziatore.gif" width="1"></td>
					<td width="100%">
						
						<cf1:IFrameWebControl id="iFrame_dx" name="iFrame_dx" runat="server" Marginwidth="0" Marginheight="1" iWidth="100%" iHeight="100%" Frameborder="0" Scrolling="no"></cf1:IFrameWebControl>
						
					</td>
					-->
				</tr>
			</table>
		</form>
	</body>
</HTML>
