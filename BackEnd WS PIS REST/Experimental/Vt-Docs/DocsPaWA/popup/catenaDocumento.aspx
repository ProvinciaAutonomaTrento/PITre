<%@ Page language="c#" Codebehind="catenaDocumento.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.catenaDocumento" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="mytree"
Namespace="Microsoft.Web.UI.WebControls" 
Assembly="Microsoft.Web.UI.WebControls" %>
<%@ import namespace="Microsoft.Web.UI.WebControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="frm_catenaDocumento" method="post" runat="server">
    		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Catena documento" />
			<table class="info" width="540" align="center" border="0">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item">
							<asp:Label id="Label1" runat="server">Catena documento</asp:Label></P>
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<td height="150">
						<mytree:treeview id="catenaDoc" runat="server" CssClass="testo_grigio" 
						HoverStyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana;background-color:#4b4b4b;"
						DefaultStyle="font-weight:normal;font-size:10px;color:#4b4b4b;text-indent:0px;font-family:Verdana;" 
						NAME="Treeview1" SystemImagesPath="../images/alberi/left/" width="100%" Height="100%" 
						SelectedStyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana;background-color:#810d06;">
					</mytree:treeview></td>
				</TR>
				<tr>
					<td height="100%"></td>
				</tr>
				<TR>
					<td align="middle" height="30">
						<asp:button id="btn_ok" runat="server" CssClass="PULSANTE" Height="19px" Width="55px" Text="OK"></asp:button>&nbsp;
						<asp:button id="btn_chiudi" runat="server" CssClass="PULSANTE" Height="19px" Width="55px" Text="CHIUDI"></asp:button></td>
				</TR>
			</table>
		</form>
		<script language="javascript">
<!--

//-->
		</script>
	</body>
</HTML>
