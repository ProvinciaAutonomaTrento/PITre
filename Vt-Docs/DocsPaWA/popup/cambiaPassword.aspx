<%@ Page language="c#" Codebehind="cambiaPassword.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.cambiaPassword" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target=_self>
	</HEAD>
	<body bottomMargin="1" leftMargin="1" topMargin="1" rightMargin="1" MS_POSITIONING="GridLayout">
		<form id="cambiaPassword" method="post" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Cambia password utente" />
			<TABLE class="info" id="tbl_cambiaPWD" width="400" align="center" border="0">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item"><asp:label id="Label1" runat="server">Cambia password</asp:label></P>
					</td>
				</TR>
				<TR>
					<TD height="5"></TD>
				</TR>
				<asp:panel id="pnl_PWD" Runat="server" Visible="True">
					<TR height="22">
						<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
							<asp:Label id="Label2" runat="server" CssClass="titolo_scheda" Width="128px">Vecchia password</asp:Label>
							<asp:TextBox id="txt_vecchiaPWD" runat="server" CssClass="testo_grigio" Width="200px" TextMode="Password"
								MaxLength="30"></asp:TextBox></TD>
					</TR>
					<TR height="22">
						<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
							<asp:Label id="lbl_oggetto" runat="server" CssClass="titolo_scheda" Width="128px">Nuova password</asp:Label>
							<asp:TextBox id="txt_nuovaPWD" runat="server" CssClass="testo_grigio" Width="200px" TextMode="Password"
								MaxLength="30"></asp:TextBox></TD>
					</TR>
					<TR height="22">
						<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
							<asp:Label id="Label3" runat="server" CssClass="titolo_scheda" Width="128px">Conferma password</asp:Label>
							<asp:TextBox id="txt_confermaPWD" runat="server" CssClass="testo_grigio" Width="200px" TextMode="Password"
								MaxLength="30"></asp:TextBox></TD>
					</TR>
				</asp:panel>
				<TR style="height: 70px">
					<TD align="center">
					    <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
						<asp:label id="lbl_message" Runat="server" Visible="False" CssClass="testo_msg_grigio"></asp:label>
				    </TD>
				  </TR>
				<TR>
					<TD align="center" height="30"><asp:button id="btn_OK" runat="server" CssClass="pulsante" Text="Conferma"></asp:button>&nbsp;&nbsp;&nbsp;
						<INPUT class="pulsante" id="btn_chiudi" onclick="window.close();" type="button" value="CHIUDI"
							name="btn_chiudi"></TD>
				</TR>
			</TABLE>
					 <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="msg_modifica" 
                            runat="server" ongetmessageboxresponse="msg_modifica_GetMessageBoxResponse" />
		</form>
	</body>
</HTML>
