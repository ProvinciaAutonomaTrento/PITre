<%@ Page language="c#" Codebehind="salvaModTrasm.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.salvaModTrasm" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
	    <title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
    		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Salvataggio del Modello di Trasmissione" />
			<TABLE class="info" id="Table1" style="Z-INDEX: 101; LEFT: 40px; WIDTH: 408px; POSITION: absolute; TOP: 8px; HEIGHT: 208px"
				height="208" width="408" align="center" border="0">
				<TR>
					<TD class="item_editbox" colSpan="2" height="20">
						<P class="boxform_item">Salvataggio del Modello di Trasmissione</P>
					</TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" align="left" width="25%">&nbsp;Nome Modello&nbsp;*</TD>
					<TD width="75%"><asp:textbox id="txt_nomeModello" tabIndex="2" runat="server" Width="95%" CssClass="testo_grigio"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="titolo_scheda" colSpan="2">&nbsp;Rendi disponibile</TD>
				</TR>
				<TR>
					<TD colSpan="2" height="50"><asp:radiobuttonlist id="rbl_share" tabIndex="3" runat="server" Width="100%" CssClass="testo_grigio"
							Height="100%">
							<asp:ListItem Value="usr" Selected="True">solo a me stesso (@usr@)</asp:ListItem>
							<asp:ListItem Value="grp">a tutto il ruolo (@grp@)</asp:ListItem>
						</asp:radiobuttonlist></TD>
				</TR>
				<tr>
					<td align="center" colSpan="2"><asp:label id="lbl_messaggio" runat="server" Font-Size="10pt" Font-Bold="True" ForeColor="Red"
							Visible="False"></asp:label></td>
				</tr>
				<TR align="center">
					<TD colSpan="2"><asp:button id="btn_salva" tabIndex="1" runat="server" Width="80px" CssClass="pulsante_hand"
							Text="Conferma"></asp:button>&nbsp;
						<asp:button id="btn_annulla" runat="server" CssClass="pulsante_hand" Text="Chiudi" Width="80px"></asp:button></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
