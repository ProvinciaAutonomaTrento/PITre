<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="listaToDoList.aspx.cs" Inherits="DocsPAWA.popup.listaToDoList"%>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server" >
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/rubrica.js"></script>
		<LINK href="../LIBRERIE/DocsPA_Func.js" type="text/css" rel="stylesheet">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
</head>
<body>
    <form id="form1" runat="server" method="post">
        <uct:AppTitleProvider ID="appTitleProvider1" runat="server" PageName = "Liste todoList" />
        <div id="divLista" style="overflow:auto" runat="server">
        <table id="tbl_princ" cellSpacing="0" border="0" cellpadding="0" width="99%" height="100%">				
				<tr >
					<td height="25" align="center" class="infoDT">
						<asp:Label ID="titolo" Runat="server" CssClass="titolo_rosso">DETTAGLIO COSE DA FARE PER RUOLO</asp:Label>
				    </td>
				</tr>
				<TR valign="top">
					<td align="center">
						<asp:table id="tbl_todolist" runat="server" HorizontalAlign="Center" Width="100%" BorderWidth="1px"
							BorderColor="LightGrey" BorderStyle="Solid" >
					    </asp:table>
				    </td>
				</TR>
				<tr>
                    <td align="center" height="30">
                        <input class="PULSANTE" id="btn_chiudi" onclick="javascript:window.close()" type="button"
                            value="CHIUDI">
                    </td>
                </tr>
	    </table>
	    </div>
    </form>
</body>
</html>