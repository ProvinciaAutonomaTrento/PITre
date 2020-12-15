<%@ Page language="c#" Codebehind="rimuoviAllegato.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.rimuoviAllegato" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register src="../documento/AclDocumento.ascx" tagname="AclDocumento" tagprefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
	    <title></title>
		<base target = "_self" />
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script type="text/javascript">
		    function CloseMask(retValue)
		    {
		        window.returnValue = retValue;
		        window.close();
		    }
		</script> 		
	</HEAD>
	<body background="#d9d9d9" MS_POSITIONING="GridLayout" onblur="self.focus()">
		<form id="rimuoviAllegato" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Rimuovi allegato" />
			<uc1:AclDocumento ID="aclDocumento" runat="server" />
			<TABLE id="Table1" class="info" width="330" align="center" border="0" style="WIDTH: 330px; HEIGHT: 104px">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item">
							Rimuovi allegato</P>
					</td>
				</TR>
				<TR>
					<TD vAlign="middle" height="40" align="center">
						<asp:label id="lbl_message" runat="server" Width="317px" Height="14px" CssClass="titolo_scheda">
						</asp:label>
					</TD>
				</TR>
				<TR height="40">
					<TD vAlign="middle" align="center" height="40">
						<asp:button id="btn_ok" runat="server" CssClass="PULSANTE" Text="OK"></asp:button>&nbsp;
						<asp:button id="btn_annulla" runat="server" CssClass="PULSANTE" Text="ANNULLA"></asp:button>
					</TD>
				</TR>
				<TR>
					<TD vAlign="middle" align="center">
						<asp:label id="lbl_result" runat="server" CssClass="testo_red"></asp:label></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
