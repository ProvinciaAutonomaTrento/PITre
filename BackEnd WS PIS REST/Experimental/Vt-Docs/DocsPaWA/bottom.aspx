<%@ Page language="c#" Codebehind="bottom.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.bottom" %>
<%@ Register src="UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="bottom" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" />
		    		<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<TD vAlign="middle" width="21%" bgColor="#e8e8e8">
					    <asp:label id="lbl_copyright" ForeColor="#959595" Height="10" CssClass="testo_grigio" Runat="server">&nbsp;</asp:label>
                    </TD>
					<TD vAlign="middle" width="20%" bgColor="#e8e8e8">
					    <asp:label id="lbl_ver" runat="server" ForeColor="#959595" Height="10px"  CssClass="testo_grigio"></asp:label>
                    </TD>
					<TD vAlign="middle" align="right" bgColor="#e8e8e8" colSpan="7">
					    <asp:label id="lbl_ip" runat="server" ForeColor="#959595" Height="10px"  CssClass="testo_grigio"></asp:label>
                   
					<IMG  id=bandiere height="17" src="images/proto/bandiere.gif"  width="48"  runat=server border="0"></TD>
				</tr>
			</table>
		</form>
	</body>
</HTML>
