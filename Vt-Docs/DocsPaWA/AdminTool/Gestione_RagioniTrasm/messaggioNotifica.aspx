<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="messaggioNotifica.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_RagioniTrasm.messaggioNotifica" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
        <title></title>
    <LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <LINK href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script language=javascript>
    
    function calcTestoDoc(l/*,event*/) 
    {
      //  var keyCode = event.keyCode ? event.keyCode : event.which ? event.which : event.charCode;

	    var maxLength = l
	    var strLen = document.frmTestoMsgNotifica.txtMsgNotificaDoc.value.length;
	    if (document.frmTestoMsgNotifica.txtMsgNotificaDoc.value.length > maxLength) 
	    {
	      document.frmTestoMsgNotifica.txtMsgNotificaDoc.value = document.frmTestoMsgNotifica.txtMsgNotificaDoc.value.substring(0,maxLength);
	      cleft = 0;
	      window.alert("Lunghezza messaggio di notifica documenti eccessiva: " + strLen + " caratteri, max " + maxLength);
	    } 
	    else 
	    {
	     /* if (keyCode == 13) 
	      {
	        alert(document.frmTestoMsgNotifica.txtMsgNotificaDoc.value.length);
	        cleft = maxLength - strLen - 1;
	        cleft = (cleft+1);
	      }
	      else
	      {*/
                cleft = maxLength - strLen;
          /*  }*/
	    }
	    document.frmTestoMsgNotifica.clTesto.value = cleft;
		  
	}
    
     function calcTestoFasc(l) 
     {
 
	    var maxLength = l
	    var strLen = document.frmTestoMsgNotifica.txtMsgNotificaFasc.value.length;
	    if (document.frmTestoMsgNotifica.txtMsgNotificaFasc.value.length > maxLength) 
	    {
	      document.frmTestoMsgNotifica.txtMsgNotificaFasc.value = document.frmTestoMsgNotifica.txtMsgNotificaFasc.value.substring(0,maxLength);
	      cleft = 0;
	      window.alert("Lunghezza messaggio di notifica fascicoli eccessiva: " + strLen + " caratteri, max " + maxLength);
	    } 
	    else 
	    {
	      cleft = maxLength - strLen;
	    }
	    document.frmTestoMsgNotifica.clTestoFasc.value = cleft;
	}
	
	
	function ShowValidationMessage(message,warning)
			{
				if (message!=null && message!='')
				{
					if (warning)
					{
						if (window.confirm(message + "\n\nContinuare?"))
						{
							Form1.submit();
						}
					}
					else
					{
						alert(message);
					}
				}
			}
			
    </script>
    <base target=_self />
</head>
<body bottomMargin="5" leftMargin="5" topMargin="5">
    <form id="frmTestoMsgNotifica" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Testo del messaggio di notifica" />
    <table  border=0 width=100% height=100%><tr><td valign=top>
        <table cellSpacing="0" cellPadding="0" align="center" border="0" width=100%>	
				
				<tr>
					<td colspan=2 height=10px></td></tr>
				<tr>
					<td class="titolo" align="center" colspan=2>
					<table width=100% cellpadding=0 cellspacing=0 >
					<tr><td class=pulsanti height=25px> Gestione testo notifiche per la ragione&nbsp;<asp:Label ID=lblNomeRag runat=server></asp:Label></td></tr></table></td>
				</tr>
				<tr><td height=10px style="width: 765px"></td></tr>
				<tr><td   class=styleDotted style="width: 765px" >
				<table width=100% border=0>
				<tr>
					<td class="testo" height="10"  style="text-align:left">&nbsp;Testo per le notifiche dei documenti (*)</td></tr>
				<tr><td colspan=2></td></tr>
				<tr>
				<td align=center width=73%>
				<asp:textbox id="txtMsgNotificaDoc" onkeyUp="calcTestoDoc('1024')"  width="99%" runat="server" CssClass="testo_grigio" TextMode="MultiLine" Rows="8" Height="180px" MaxLength="1024"></asp:textbox>
				</td>
				<td  valign=middle style="width: 189px">
				<TABLE>
							<TR>
								<TD align="center">
									<asp:Label id="lblMsgNotificaDoc" runat="server" cssclass="titolo_piccolo">Opzioni per la notifica dei documenti</asp:Label></TD>
							</TR>
							<TR>
								<TD align="center" colSpan="2">
									<asp:DataGrid id="dgNotificaDoc" runat="server" Visible="True" Width="200px" AutoGenerateColumns="False" OnItemCommand="dgNotificaDoc_ItemCommand">
										<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
										<ItemStyle Font-Size="XX-Small" Font-Names="Verdana" ForeColor="#666666" BackColor="#F2F2F2"></ItemStyle>
										<HeaderStyle CssClass="titolo_piccolo" BackColor="#E0E0E0"></HeaderStyle>
										<Columns>
											<asp:ButtonColumn Text="&lt;img src=../Images/aggiungi.gif border=0 alt='Aggiungi'&gt;" CommandName="Select">
												<ItemStyle BackColor="#E0E0E0"></ItemStyle>
											</asp:ButtonColumn>
											<asp:BoundColumn DataField="codice" HeaderText="Codice"></asp:BoundColumn>
											<asp:BoundColumn DataField="descrizione" HeaderText="Descrizione"></asp:BoundColumn>
										</Columns>
									</asp:DataGrid>
							</td>
							</TR></TABLE>
				
				
				</td>
				</tr>
				<tr>
				<td>
				<table width=100%>
				<tr>
				<td align=left width=50% valign=top><asp:CheckBox CssClass=testo_grigio id=ckbDoc runat=server Text="Salva per tutte le ragioni"  /></td>
				<td class=testo_grigio align=right>caratteri disponibili:&nbsp;<input class="testo_grigio" readonly  type="text" size="4" value="1024" name="clTesto"></td>
				</tr>
				</table>
				</td>	
				</tr>
				</table>
				</td>
				</tr>
				
				
				<tr><td height=10px class=testo_grigio style="font-size:medium">
						        </td></tr>
				<tr><td   class=styleDotted style="width: 765px">
				<table width=100% border=0>
				<tr>
			<td class="testo" height="10"  style="text-align:left">&nbsp;Testo per le notifiche dei fascicoli (*)</td></tr>
				<tr><td colspan=2></td></tr>
							<tr>
				<td  class=testo_grigio align=center width=73%>
				<asp:textbox id="txtMsgNotificaFasc" onkeyup="calcTestoFasc('1024')" width=99% runat="server" CssClass="testo_grigio" TextMode="MultiLine" Rows="8" Height="180px" MaxLength="1024"></asp:textbox>
				
				</td>
				<td width=73% align=center valign=middle>
				
				<TABLE>
							<TR>
								<TD align="center">
									<asp:Label id="lblMsgNotificaFasc" runat="server" cssclass="titolo_piccolo">Opzioni per la notifica dei fascicoli</asp:Label></TD>
							</TR>
							<TR><TD  align="center" colSpan="2">
									<asp:DataGrid id="dgNotificaFasc" runat="server" Visible="True" Width="200px" AutoGenerateColumns="False" OnItemCommand="dgNotificaFasc_ItemCommand">
										<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
										<ItemStyle Font-Size="XX-Small" Font-Names="Verdana" ForeColor="#666666" BackColor="#F2F2F2"></ItemStyle>
										<HeaderStyle CssClass="titolo_piccolo" BackColor="#E0E0E0"></HeaderStyle>
										<Columns>
											<asp:ButtonColumn Text="&lt;img src=../Images/aggiungi.gif border=0 alt='Aggiungi'&gt;" CommandName="Select">
												<ItemStyle BackColor="#E0E0E0"></ItemStyle>
											</asp:ButtonColumn>
											<asp:BoundColumn DataField="codice" HeaderText="Codice">
												<ItemStyle HorizontalAlign="Center"></ItemStyle>
											</asp:BoundColumn>
											<asp:BoundColumn DataField="descrizione" HeaderText="Descrizione"></asp:BoundColumn>
										</Columns>
									</asp:DataGrid></TD>
							</TR>
						</TABLE>
				
				</td>
				</tr>
				<tr><td>
				
				<table width=100%>
				<tr>
				<td align=left width=50% valign=top><asp:CheckBox CssClass=testo_grigio id=ckbFasc runat=server Text="Salva per tutte le ragioni"  /></td>
				<td class=testo_grigio align=right>caratteri disponibili:&nbsp;<input class="testo_grigio" readOnly type="text" size="4" value="1024" name="clTestoFasc">
				</td>
				</tr>
				</table>
				</td>
				
				
				</tr>
				</table>
									
						</td></tr>
						
						<tr><td height=10px style="width: 765px"></td></tr>
						
						<tr><td align=center class=styleDotted height=40px style="width: 765px">
						<asp:Button ID="btnSalva" runat=server Text="Salva" CssClass="testo_btn" OnClick="btnSalva_Click"/>&nbsp;
						<asp:Button ID="btnChiudi" runat=server Text="Chiudi" CssClass="testo_btn" OnClick="btnChiudi_Click"/>&nbsp;
						
						</td></tr>
						<tr><td><cc2:messagebox id="msgBoxChiudiNotifica" runat="server" OnGetMessageBoxResponse="msgBoxChiudiNotifica_GetMessageBoxResponse"></cc2:messagebox>
							</td></tr>
						
						</table></td></tr>
						
						</table>				
				
				
	    </form>
</body>
</html>
