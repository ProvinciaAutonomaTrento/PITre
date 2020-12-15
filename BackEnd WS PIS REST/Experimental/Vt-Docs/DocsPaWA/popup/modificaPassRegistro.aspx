<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="modificaPassRegistro.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.modificaPassRegistro" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat = "server">
		<title>Modifica Password</title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="modificaPassword" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Modifica Password Registro" />
			<TABLE id="tbl_modPassReg" class="info" align="center" border="0" width="420">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item"><asp:Label id="Label2" runat="server" CssClass="menu_grigio_popup">Modifica Password Registro</asp:Label></P>
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
						<asp:label id="lbOldPass" runat="server" CssClass="titolo_scheda" Width="132px">Vecchia Password:&nbsp;</asp:label>
                        <asp:textbox id="txtOldPass" runat="server" CssClass="testo_grigio" Width="250px" Height="22px" TextMode="Password" EnableViewState="true"></asp:textbox>
                        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidatorOldPass" runat="server" ErrorMessage="*" ControlToValidate="txtOldPass" ToolTip="Campo Obbligatorio"></asp:RequiredFieldValidator>--%>
					</TD>
				</TR>
				<TR>
					<TD>&nbsp;</TD>
				</TR>
				<TR>
					<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
						<asp:label id="lbNuovaPass" runat="server" CssClass="titolo_scheda" Width="132px">Nuova Password:&nbsp;</asp:label>
						<asp:textbox id="txtNuovaPass" runat="server" CssClass="testo_grigio" Width="250px" Height="22px" TextMode="Password" EnableViewState="true"></asp:textbox>
                        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidatorNewPass" runat="server" ErrorMessage="*" ControlToValidate="txtNuovaPass" ToolTip="Campo Obbligatorio"></asp:RequiredFieldValidator>--%>
                    </TD>
				</TR>
                
				<TR>
					<TD>&nbsp;</TD>
				</TR>
				<TR>
					<TD>
						<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
						<asp:label id="lbConfermaPass" runat="server" CssClass="titolo_scheda" Width="132px">Conferma Password:&nbsp;</asp:label>
						<asp:textbox id="txtConfermaPass" runat="server" CssClass="testo_grigio" Width="250px" Height="22px" TextMode="Password" EnableViewState="true"></asp:textbox>
					    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidatorConfPass" runat="server" ErrorMessage="*" ControlToValidate="txtConfermaPass" ToolTip="Campo Obbligatorio"></asp:RequiredFieldValidator>--%>
                    </TD>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<TD align="middle" height="30">
						<asp:Button id="btn_ok" runat="server" CssClass="PULSANTE" Text="OK" Width="40px"></asp:Button>&nbsp;
						<asp:Button id="btn_chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI" CausesValidation="false"></asp:Button>
					</TD>
				</TR>
				<TR>
					<TD align="middle" height="30">
						&nbsp;</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
