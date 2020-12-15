<%@ Page language="c#" Codebehind="rimuoviVersione.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.rimuoviVersione" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="uc1" TagName="CheckInOutController" Src="../CheckInOut/CheckInOutController.ascx" %>
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
		<base target="_self">
		<script language="javascript">
		
		function ClosePage(retValue)
		{
			window.returnValue=retValue;
			window.close();
		}
		
		function RemoveCheckOutVersion()
		{
			UndoCheckOutDocument(false,true);
		}
	
		</script>
	</HEAD>
	<body background="#d9d9d9" bottomMargin="1" leftMargin="1" topMargin="6" rightMargin="1"
		MS_POSITIONING="GridLayout">
		<form id="rimuoviVersione" method="post" runat="server">
	    	<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Rimuovi versione" />    
			<uc1:CheckInOutController id="checkInOutController" runat="server"></uc1:CheckInOutController>
			<TABLE id="Table1" class="info" width="330" align="center" border="0" style="WIDTH: 330px; HEIGHT: 104px">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item">
							Rimuovi versione</P>
					</td>
				</TR>
				<TR>
					<TD vAlign="middle" height="40" align="center">&nbsp;
						<asp:label id="lbl_messageCheckOut" Visible="False" runat="server" Width="317px" Height="14px"
							CssClass="titolo_scheda">
							Il documento risulta bloccato.<br>
							Impossibile rimuovere la versione selezionata.
						</asp:label>
						<asp:label id="lbl_messageOwnerCheckOut" Visible="False" runat="server" Width="317px" Height="14px"
							CssClass="titolo_scheda">
							Il documento risulta bloccato.<br>
							Tutte le eventuali modifiche effettuate verranno perse.<br>
							Sei sicuro di voler rimuovere la versione?
						</asp:label>
						<asp:label id="lbl_message" Visible="False" runat="server" Width="317px" Height="14px" CssClass="titolo_scheda">Sei sicuro di voler rimuovere la versione?</asp:label></TD>
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
