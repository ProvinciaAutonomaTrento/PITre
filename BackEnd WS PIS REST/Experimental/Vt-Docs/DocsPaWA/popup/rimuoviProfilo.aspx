<%@ Page language="c#" Codebehind="rimuoviProfilo.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.rimuoviProfilo" EnableEventValidation="false"%>
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
		<base target="_self"/>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
		
		function ClosePage(retValue)
		{
			window.returnValue=retValue;
			window.close();
		}
	
		</script>
	</HEAD>
	<body background="#d9d9d9" MS_POSITIONING="GridLayout">
		<form id="rimuoviProfilo" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Rimuovi profilo" />
	        <uc1:CheckInOutController id="checkInOutController" runat="server"></uc1:CheckInOutController>
			<TABLE id="Table1" class="info" width="330" align="center" border="0" style="WIDTH: 330px; HEIGHT: 135px">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item">
							Rimuovi Documento</P>
					</td>
				</TR>
				<TR>
					<TD vAlign="middle"  align="center">&nbsp;
						<asp:label id="lbl_messageCheckOut" Visible="False" runat="server" CssClass="titolo_scheda"
							Height="14px" Width="317px">
							Il documento risulta bloccato, impossibile rimuoverlo.
						</asp:label>
						<asp:label id="lbl_messageOwnerCheckOut" Visible="False" runat="server" CssClass="titolo_scheda"
							Height="14px" Width="317px">
							Il documento risulta bloccato,<br>
							tutte le eventuali modifiche effettuate verranno perse.<br>
							Sei sicuro di voler rimuovere il Documento?
						</asp:label>
					</TD>
				</TR>
				<tr>
				<TD vAlign="middle"  align="center">&nbsp;
				<asp:label id="lbl_mess_conf_rimuovi" Visible="False" runat="server" CssClass="titolo_scheda"
							Height="20px" Width="317px">
							Per confermare l'operazione inserire una nota e premere Ok
						</asp:label>
				</td>
				</tr>
				<TR>
					<TD vAlign="middle" align="center">
						<asp:label id="lbl_result" runat="server" CssClass="testo_red"></asp:label></TD>
				</TR>
				<TR>
				<td>
				<table width=100% border=0 cellpadding=0 cellspacing=0>
				<tr valign="top">
				    <TD >
				        <asp:label id="lbl_note"  runat="server" CssClass="titolo_scheda">Note</asp:label>&nbsp;
				    </TD>
						<td align="left"> 
						 <asp:textbox  id="txt_note" runat="server" Width="280px" Height="50px" 
							CssClass="testo_grigio" TextMode="MultiLine"  MaxLength="256"></asp:textbox>			
				   
				    </td></tr></table>
				    </td>
				</TR>
				<TR height="40">
					<TD vAlign="middle" align="center" height="40">
						<asp:button id="btn_ok" runat="server" CssClass="PULSANTE" Text="Ok"></asp:button>&nbsp;
						<asp:button id="btn_annulla" runat="server" CssClass="PULSANTE" Text="Chiudi"></asp:button>
					</TD>
				</TR>
				
					
				
			</TABLE>
		</form>
	</body>
</HTML>
