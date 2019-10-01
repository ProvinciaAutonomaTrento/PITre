<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home2.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Homepage.Home2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>DOCSPA - AMMINISTRAZIONE > Home</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
		<SCRIPT language="JavaScript">			
			
			var cambiapass;
			var hlp;

			function Init() {
			
			}				
			function apriPopup() {
				hlp = window.open('../help.aspx?from=HP','','width=450,height=500,scrollbars=YES');
			}				
			function cambiaPwd() {
				cambiapass = window.open('../CambiaPwd.aspx','','width=410,height=300,scrollbars=NO');
			}	
			function ClosePopUp()
			{	
				if(typeof(cambiapass) != 'undefined')
				{
					if(!cambiapass.closed)
						cambiapass.close();
				}
				if(typeof(hlp) != 'undefined')
				{
					if(!hlp.closed)
						hlp.close();
				}				
			}
		</SCRIPT>
</head>
	<body bottomMargin="0" leftMargin="0" topMargin="0" 
		rightMargin="0" onunload="Init();ClosePopUp()">
		<form id="Form1" method="post" runat="server">
			<input id="hd_idAmm" type="hidden" name="hd_idAmm" runat="server"> 
			<!-- Gestione del menu a tendina --><uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
			<table  cellSpacing="1" cellPadding="0" width="100%" border="0" height="100%">
				<tr>
					<td>
						<!-- TESTATA CON MENU' --><uc1:testata id="Testata" runat="server"></uc1:testata></td>
				</tr>
				<tr>
					<!-- STRISCIA SOTTO IL MENU -->
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20"><asp:label id="lbl_position" runat="server"></asp:label></td>
				</tr>
				<tr>
					<!-- STRISCIA DEL TITOLO DELLA PAGINA -->
					<td class="titolo" align="center" bgColor="#e0e0e0" height="20"></td>
				</tr>
				<tr>
					<td vAlign="top" align="center" height="100%">
						<!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
						<table cellSpacing="2" cellPadding="0" width="80%" border="0" align=center>
				            <tr>
				            <td class="testo_grigio_scuro" height="30"><br /><br /><br />Benvenuto&nbsp;<asp:Label ID="lb_utente" runat="server"></asp:Label></td>
				            </tr>
				            <tr>
				            <td class="testo_grigio_scuro" height="30">seleziona una voce di menù di tua competenza</td>
				            </tr>
				            
				        </table>
					</td>
				</tr>
			</table>
			</form>
</body>
</html>
