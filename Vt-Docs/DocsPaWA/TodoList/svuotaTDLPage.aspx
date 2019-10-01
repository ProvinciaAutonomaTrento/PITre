<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="svuotaTDLPage.aspx.cs" Inherits="DocsPAWA.TodoList.svuotaTDLPage" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script language="JavaScript" src="../AdminTool/CSS/ETcalendar.js"></script>
    <base target=_self />
    <script language="javascript" id="btn_elimina_click" event="onclick()" for="btn_elimina">
		window.document.body.style.cursor='wait';
		var w_width = 500;
		var w_height = 280;							
		document.getElementById ("WAIT").style.top = 0;
		document.getElementById ("WAIT").style.left = 0;
		document.getElementById ("WAIT").style.width = w_width;
		document.getElementById ("WAIT").style.height = w_height;				
		document.getElementById ("WAIT").style.visibility = "visible";
	</script>		
	<script language="JavaScript">
	    function ClosePopUp()
		{					
			if(typeof(obj_calwindow) != 'undefined')
			{
				if(!obj_calwindow.closed)
					obj_calwindow.close();
			}
		}		
	</script>
</head>
<body onunload="ClosePopUp()">
    <form id="frmEliminaTDL" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Svuota TDL"/>
        <input type=hidden id=hd_tipoObjTrasm runat=server name=hd_tipoObjTrasm>
        <input type=hidden id=hd_noticeDays runat=server name=hd_noticeDays>
        <input type=hidden id=hd_dataSistema runat=server name=hd_dataSistema>
        <input type=hidden id=hd_functionEnabled runat=server name=hd_functionEnabled>     
        <table border=0 cellpadding=0 cellspacing=0 align=center width=100%>            
            <tr>
                <td height=10>&nbsp;</td>
            </tr>
            <asp:Panel ID=pnl_functionEnabled runat=server Visible=true>
                <tr>
                    <td class=testo_red align=center>Avviso</td>
                </tr>
                <tr>
                    <td height=10>&nbsp;</td>
                </tr>
                <tr>
                    <td class=testo10N align=center>
                        Ci sono <asp:Label ID=lbl_oldTxt runat=server></asp:Label> trasmissioni più vecchie di <asp:Label ID=lbl_noticeDays runat=server></asp:Label> giorni.<br>
                        Puoi scegliere di eliminarle dalle 'Cose da fare'<br>tramite il tasto 'Rimuovi'.</td>
                </tr>            
                <tr>
                    <td height=15>&nbsp;</td>
                </tr>
            </asp:Panel>
            <tr bgcolor=#F2F2F2>
                <td class=testo10N align=center><br>Rimuovi le trasmissioni antecedenti la seguente data</td>
            </tr>
            <!--<tr bgcolor=#F2F2F2>
                <td class=testo10N align=center valign=baseline>
                    (gg/mm/aaaa)&nbsp;<asp:TextBox ID="txt_gg" runat=server Width=68px MaxLength=10 CssClass=testo10N></asp:TextBox>&nbsp;&nbsp;
                    <a href="javascript:calendario.popup();"><img src="../images/cal.gif" width="16" height="16" border="0" alt="Inserisci la data"></a>
                    &nbsp;&nbsp;<a href="javascript:clearData();"><img src="../images/noData.gif" width="16" height="16" border="0" alt="Elimina la data"></a><br>&nbsp;</td>
            </tr>-->
            <tr bgcolor=#F2F2F2>
                <td class=testo10N align=center valign=baseline>(gg/mm/aaaa)<uc1:Calendario id="calendar" runat="server"/>&nbsp;&nbsp;
                <a href="javascript:clearData();"><img src="../images/noData.gif" width="16" height="16" border="0" alt="Elimina la data"></a><br>&nbsp;</td>
            </tr>

            <tr>
                <td height=5>&nbsp;</td>
            </tr> 
            <tr bgcolor=LemonChiffon>
                <td class=testo10N align=center valign=middle height=30>
                    <asp:CheckBox id="chk_noWF" Runat="server" Checked=true></asp:CheckBox>&nbsp;Non eliminare le trasmissioni che necessitano accettazione.
                </td>
            </tr> 
            <tr>
                <td height=10>&nbsp;</td>
            </tr>
            <tr>
                <td class=testo10N align=center><b>Nota</b>: le trasmissioni verranno rimosse solo da 'Cose da fare';<br>le stesse saranno comunque accessibili tramite le funzionalità di consultazione.</td>
            </tr>           
            <tr>
                <td height=20>&nbsp;</td>
            </tr>            
            <tr>
                <td align=center><asp:Button ID=btn_elimina runat=server Text=Rimuovi CssClass=pulsante OnClick="btn_elimina_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID=btn_annulla runat=server Text=Annulla CssClass=pulsante /></td>
            </tr>            
        </table>
        <DIV id="WAIT" style="BORDER-RIGHT: #f5f5f5 1px solid; BORDER-TOP: #f5f5f5 1px solid; VISIBILITY: hidden; BORDER-LEFT: #f5f5f5 1px solid; BORDER-BOTTOM: #f5f5f5 1px solid; POSITION: absolute; BACKGROUND-COLOR: #f5f5f5">
			<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td style="FONT-SIZE: 11pt; FONT-FAMILY: Verdana" vAlign="middle" align="center">Attendere 
						prego,<br><br>rimozione in corso...<br><br>
					</td>
				</tr>
			</table>
		</DIV>
    </form>
    <script language="JavaScript">
		/*var calendario = new calendar1(document.frmEliminaTDL.elements['txt_gg']);
		calendario.year_scroll = true;
		calendario.time_comp = false; */
				
	    function clearData()
	    {
	        document.frmEliminaTDL.calendar_txt_Data.value = "";
	    }	
	</script>
</body>
</html>
