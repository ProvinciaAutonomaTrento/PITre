<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="sceltaNuovoProprietario.aspx.cs" Inherits="DocsPAWA.popup.sceltaNuovoProprietario" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <title>DOCSPA > Scelta nuovo proprietario</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	<base target="_self" />
	<SCRIPT language="javascript">
	    function SingleSelect(regex,current) 
        { 
            re = new RegExp(regex);
            for(i = 0; i < document.forms[0].elements.length;i++)
            {
        
                elm = document.forms[0].elements[i];

                //if (elm.type == 'checkbox' && elm != current && re.test(elm.name))
                if (elm.type == 'radio' && elm != current && re.test(elm.name))
                {
                    elm.checked = false; 
                } 
            } 
         }
	</SCRIPT>
</head>
    <body>
		<form id="sceltaNuovoProprietario" method="post" runat="server">
		    <input type="hidden" name="hd_tipo" id="hd_tipo" runat=server />
		    <input type="hidden" name="hd_idPeopleNewPropr" id="hd_idPeopleNewPropr" runat=server />
		    <input type="hidden" name="hd_idRuoloNewPropr" id="hd_idRuoloNewPropr" runat=server />
		    <input type="hidden" name="hd_userIdNewPropr" id="hd_userIdNewPropr" runat=server />
			<table cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
			    <tr><td>&nbsp;</td></tr>
				<tr>
					<td class="infoDT" align="center" height="20">
					    <asp:label id="titolo" Runat="server" CssClass="titolo_rosso">Assegna un nuovo proprietario</asp:label></td>
				</tr>
				<tr>
				    <td align=center>
				        <asp:Label ID="lbl_avviso" runat="server" CssClass="testo_grigio_scuro"></asp:Label>
				    </td>
				</tr>
				<tr>
					<td align=center><asp:table id="tbl_Lista" runat="server" Width="95%" CellPadding="0" CellSpacing="1" TabIndex=0></asp:table></td>
				</tr>
				<tr><td>&nbsp;</td></tr>
				<asp:Panel ID="pnl_dett_modello" runat="server">
				<tr><td>
				    <TABLE width="95%" border=0 cellpadding=0 cellspacing=0 align=center>
				    <TR>
					    <TD class="testo_grigio" align="left" width="20%">&nbsp;Nome Modello&nbsp;*&nbsp;</TD>
					    <TD width="80%"><asp:textbox id="txt_nomeModello" runat="server" Width="95%" CssClass="testo_grigio" TabIndex=1></asp:textbox></TD>
				    </TR>
				    <TR><TD>&nbsp;</TD></TR>				    
				    <TR>
					    <TD class="testo_grigio" colspan="2">&nbsp;Rendi disponibile<br />&nbsp;
					        <asp:radiobuttonlist id="rbl_share" runat="server" Width="90%" CssClass="testo_grigio" TabIndex=2>
							    <asp:ListItem Value="usr" Selected="True">solo a me stesso (@usr@)</asp:ListItem>
							    <asp:ListItem Value="grp">a tutto il ruolo (@grp@)</asp:ListItem>
						    </asp:radiobuttonlist></TD>
				    </TR>			
				    </TABLE>
				</td></tr>
				</asp:Panel>
				<tr>
					<td vAlign="middle" align="center">
					    <br />
				        <asp:button id="btn_ok" runat="server" CssClass="pulsante" Text="    OK    " TabIndex=3 />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<asp:button id="btn_annulla" runat="server" CssClass="pulsante" Text=" CHIUDI " TabIndex=4 /><br /><br />
					</td>	
				</tr>
			</table>
		</form>
	</body>
</html>
