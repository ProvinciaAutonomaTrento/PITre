<%@ Page language="c#" Codebehind="tabTrasmissioniEff.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.tabTrasmissioniEff" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
</head>
<body rightmargin="0" topmargin="0" bottommargin="0" leftmargin="0">
    <!--table cellSpacing="0" width="100%" border="0" cellpadding="0" align=center>
			<tr>
				<td-->
		<form id="Trasmissioni_cn" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Dettaglio della Trasmissione" />
			<table align="center" cellSpacing="0" border="0" height="100%" cellpadding="0" width="100%">
				<tr>
					<td height="3"></td>
				</tr>
				<tr>
					<td height="15" align="center" class="infoDT">
						<asp:Label ID="titolo" Runat="server" CssClass="titolo_rosso">DETTAGLIO 
						TRASMISSIONE SELEZIONATA</asp:Label></td>
				</tr>
				<TR valign="top">
					<td align="center">
						<asp:table id="tblTx" runat="server" HorizontalAlign="Center" Width="100%" Height="100%" BorderWidth="1px"
							BorderColor="LightGrey" BorderStyle="Solid" GridLines="Both"></asp:table></td>
				</TR>
				<tr>
					<td height="100%">&nbsp;</td>
				</tr>
			</table>
            <div id="please_wait" style="display:none;z-index:1000; border-right: #000000 2px solid; border-top: #000000 2px solid;
                border-left: #000000 2px solid; border-bottom: #000000 2px solid; position: absolute;
                background-color: #d9d9d9">
                <table cellspacing="0" cellpadding="0" width="350px" border="0">
                    <tr>
                        <td valign="middle" style="font-weight: bold; font-size: 12pt; font-family: Verdana"
                            align="center" width="350px" height="90px">
                            Attendere, prego...
                        </td>
                    </tr>
                </table>
            </div>
		</form>
		<!--/td>
			</tr>
		</table-->
</body>
</html>


