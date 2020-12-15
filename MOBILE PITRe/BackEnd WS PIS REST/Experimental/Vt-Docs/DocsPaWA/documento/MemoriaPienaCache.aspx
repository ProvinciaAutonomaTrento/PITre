<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemoriaPienaCache.aspx.cs" Inherits="DocsPAWA.MemoriaPienaCache" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<HEAD id="HEAD1" runat="server">
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	</HEAD>
<body MS_POSITIONING="GridLayout">
		<form id="CachePiena" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Cache Piena" />
			<table border="0" width="100%" height="100%">
				<tr height="10%">
					<td align="left" valign="top"><br>
						&nbsp;</td>
				</tr>
				<tr>
				<td valign="center" align="middle" class="testo_msg_grigio_grande">
					<asp:Label ID="L_msg" runat="server" Text="Memoria cache piena."></asp:Label>
     			</td>
				</tr>
                <tr>
                <td valign="center" align="middle" class="testo_msg_grigio_grande">
                   <asp:Label ID="Label1" runat="server" Text="Non è possibile aggiungere nessun file."></asp:Label>
                </td>
                </tr>
                <tr>
                <td valign="center" align="middle" class="testo_msg_grigio_grande">
                   <asp:Label ID="Label2" runat="server" Text="Riprovare più tardi"></asp:Label>
                </td>
                </tr>
				<tr height="10%">
					<td align="left" valign="top"><br>
						&nbsp;</td>
				</tr>
			</table>
		</form>
	</body>
</html>
