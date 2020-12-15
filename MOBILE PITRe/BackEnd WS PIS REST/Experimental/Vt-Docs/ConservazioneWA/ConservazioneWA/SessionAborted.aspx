<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SessionAborted.aspx.cs" Inherits="ConservazioneWA.SessionAborted" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    
    <link href="CSS/docspa_30.css" type="text/css" rel="Stylesheet" />
    
    <script language="javascript">
			function body_onLoad()
			{
				window.resizeTo(screen.availWidth, screen.availHeight);	
				window.moveTo(0, 0);														
			}
		</script>
</head>
<body onload="body_onLoad();">
    <form id="form1" runat="server">
    <table cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
			<tr>
				<td align="left">
				<table cellspacing="0" cellpadding="0" border="0">
				<tr>
				<td><asp:image runat="server" ImageUrl="~/Img/Img_logo.gif" border="0" width="141" height="58" ID="img_logologin"></asp:image></td>
				<td><asp:image width="121" height="58" ImageUrl="Img/logoentelogin1.gif" id="img_logologinente1" runat="server" ImageAlign="AbsMiddle" Visible="true"></asp:image></td>
				<td style="width:100%"><asp:image Width="100%"  height="58px" ImageUrl="Img/logoentelogin2.gif" 
                        id="img_logologinente2" runat="server" ImageAlign="AbsMiddle" Visible="true" 
                        style="margin-right: 4px"></asp:image></td>
				</tr>
				</table>
				</td>				
			</tr>
			<tr>
				<td width="100%"  bgColor="#9e9e9e" colSpan="2" height="20"><img src="Img/spacer.gif" style="height:1; width:1; border:0" /></td>
			</tr>
			<tr height="40">
				<td colSpan="2"></td>
			</tr>
			<tr>
				<td align="center" colSpan="2"><asp:label id="errorLabel" Visible="false" BackColor="White" ForeColor="Maroon" Font-Name="verdana"
						Font-Size="10" Font-Bold="True" Width="100%" Runat="server"></asp:label></td>
			</tr>
			<tr>
				<td colSpan="2" height="70"></td>
			</tr>
			<tr>
				<td class="testo_grigio_scuro" align="center" colSpan="2">Seleziona 'Accedi' per 
					tornare alla pagina di login.
				in.
				</td>
			</tr>
			<tr>
				<td colSpan="2" height="10"></td>
			</tr>
			<tr>
				<td align="center" colSpan="2"><a class="testo_grigio_scuro" href="login.htm"><img src="Img/Butt_Accedi.jpg" style="border:0" /></a>
				</td>
			</tr>
		</table>
    </form>
</body>
</html>
