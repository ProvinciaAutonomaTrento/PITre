<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="rimuoviVisibilita.aspx.cs" Inherits="DocsPAWA.popup.rimuoviVisibilita" EnableEventValidation="false"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > rimuoviVisibilita'</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
		
		function ClosePage(retValue)
		{
			window.returnValue=retValue;
			window.close();
		}
		</script>
</head>
<body background="#d9d9d9" MS_POSITIONING="GridLayout">
		<form id="rimuoviACL" method="post" runat="server">
	        <TABLE id="Table1" class="info" width="330" align="center" border="0" style="WIDTH: 330px; HEIGHT: 114px">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item">
							Rimuovi Visibilita'</P>
					</td>
				</TR>
				<TR>
					<TD vAlign="middle" align="center">
						<asp:label id="lbl_result" runat="server" CssClass="testo_red"></asp:label></TD>
				</TR>
				<TR>
				    <TD vAlign="middle">&nbsp;&nbsp;
				        <asp:label id="lbl_note" valign="top" runat="server" CssClass="titolo_scheda">Note</asp:label>&nbsp;
						<asp:textbox  id="txt_note" runat="server" Width="260px" Height="50px" 
							CssClass="testo_grigio" TextMode="MultiLine"></asp:textbox>			
				    </TD>
				</TR>
				<TR height="40">
					<TD vAlign="middle" align="center" height="40">
						<asp:button id="btn_ok" runat="server" CssClass="PULSANTE" Text="OK"></asp:button>&nbsp;
						<asp:button id="btn_annulla" runat="server" CssClass="PULSANTE" Text="ANNULLA"></asp:button>
					</TD>
				</TR>
				
			</TABLE>
		</form>
	</body>
</html>
