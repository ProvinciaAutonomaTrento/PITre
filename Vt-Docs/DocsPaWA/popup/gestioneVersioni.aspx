<%@ Page language="c#" Codebehind="gestioneVersioni.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.gestioneVersioni" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
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
		<%--<script language="javascript">
			function calcTesto(f,l) {
 
			  // riconosce il tipo di browser collegato
			  
			  if (document.all) {
			    var IE = true;
			  }
			  if (document.layers) {
			    var NS = true;
			  }
			  
			  // versione per NetScape
			
			  if (NS) {
			    StrLen = f.value.length
			    if (StrLen > l ) {
			      f.value = f.value.substring(0,l)
			      CharsLeft = 0
			      window.alert("Lunghezza NOTE eccessiva: " + StrLen + " caratteri, max " + l);
			    } else {
			      CharsLeft = l - StrLen
			    }
			    document.gestioneVersioni.clTesto.value = CharsLeft
			  }
			  
			  // versione per Internet Explorer
			  
			  if (IE) {
			    
			    var maxLength = l
			    var strLen = document.gestioneVersioni.TextNote.value.length
			    if (document.gestioneVersioni.TextNote.value.length > maxLength) {
			      document.gestioneVersioni.TextNote.value = document.gestioneVersioni.TextNote.value.substring(0,maxLength)
			      cleft = 0
			      window.alert("Lunghezza NOTE eccessiva: " + strLen + " caratteri, max " + maxLength);
			    } else {
			      cleft = maxLength - strLen
			    }
			    document.gestioneVersioni.clTesto.value = cleft
			  }
			}
		</script>--%>
	</HEAD>
	<body MS_POSITIONING="GridLayout" leftMargin="2" topMargin="2">
		<form id="gestioneVersioni" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione versioni" />
			<TABLE id="Table1" align="center" border="0" class="info">
				<TR>
					<td class="item_editbox" colspan="2">
						<P class="boxform_item"><asp:label id="Label" runat="server">Inserimento nuova versione</asp:label></P>
					</td>
				</TR>
				<TR>
					<TD height="2" colspan="2">
					</TD>
				</TR>
				<TR>
					<TD class="testo_grigio_light" valign=top>Oggetto:&nbsp;</TD>
					<TD><DIV style="OVERFLOW: auto; HEIGHT: 35px"><asp:Label  ID="descDoc" Runat="server" CssClass="testo_grigio_light" Width="340px"></asp:Label></div></TD>
				</TR>
				<TR height=20px>
					<TD align="right">
						<asp:label id="LabelCodice" runat="server" CssClass="testo_grigio_scuro">Codice:&nbsp;</asp:label>
					</TD>
					<TD>
						<asp:label id="TextCodice" runat="server" CssClass="testo_grigio_light" Width="360px"></asp:label>
					</TD>
				</TR>
				<TR>
					<TD align="right" valign="top">
						<asp:label id="LabelNote" runat="server" CssClass="testo_grigio_scuro">Note:&nbsp;</asp:label>
					</TD>
					<TD>
						<asp:textbox id="TextNote" runat="server" CssClass="testo_grigio" Width="360px" Height="45px"
							 TextMode="MultiLine"  ></asp:textbox>
					</TD>
				</TR>
				<tr>
					<td colspan="2" align="right" class="testo_grigio">
						caratteri disponibili:&nbsp;<input type="text" id="clTesto" runat="server" name="clTesto"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
				</tr>
				<TR>
					<TD height="2" colspan="2">
					</TD>
				</TR>
				<TR>
					<TD align="center" height="30" colspan="2">
						<asp:button id="btn_ok" runat="server" CssClass="PULSANTE" Text="Conferma" ></asp:button>&nbsp;
						<INPUT class="PULSANTE" id="btn_chiudi" onclick="javascript:window.close()" type="button"
							value="Chiudi" name="btn_chiudi">
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
