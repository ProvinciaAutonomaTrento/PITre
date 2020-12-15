<%@ Page language="c#" Codebehind="insParolaChiave.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.insParolaChiave" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="insParolaChiave" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Inserimento Parola Chiave" />
    		<TABLE id="tbl_insOggetto" class="info" width="400" align="center" border="0">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item">
							<asp:label id="Label1" runat="server" Width="177px">Inserimento Parola Chiave</asp:label>
						</P>
					</td>
				</TR>
				<TR>
					<TD height="5"></TD>
				</TR>
				<TR height="22">
					<TD>
						<IMG height="1" src="../images/proto/spaziatore.gif" width="6" border="0">
						<asp:label id="lbl_parolaC" runat="server" CssClass="titolo_scheda">Parola chiave&nbsp;</asp:label>
						<asp:textbox id="txt_parolaC" runat="server" CssClass="testo_grigio" Width="290px"></asp:textbox></TD>
				</TR>
				<TR>
					<TD height="10"></TD>
				</TR>
				<TR>
					<TD align="middle" height="30">
						<asp:button id="btn_Insert" runat="server" CssClass="pulsante" Text="INSERISCI"></asp:button>
						<asp:button id="Button1" runat="server" CssClass="pulsante" Text="CHIUDI"></asp:button></TD>
				</TR>
				<TR>
					<TD height="3"></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
