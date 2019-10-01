<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportCorrispondenti.aspx.cs" Inherits="DocsPAWA.popup.ImportCorrispondenti" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
     <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="../CSS/docspA_30.css" type="text/css" rel="stylesheet">
	<base target="_self">
	<SCRIPT language="javascript">
			function apriLog() 
			{			
			    var myUrl = "LogImportRubrica.aspx";				
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:700px;dialogHeight:500px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 									
			}
	</script>
	<%--<script language="javascript" id="btn_importa_click" event="onclick()" for="btn_importa">
			window.document.body.style.cursor='wait';
			var w_width = 490;
			var w_height = 180;							
			document.getElementById ("WAIT").style.top = 0;
			document.getElementById ("WAIT").style.left = 0;
			document.getElementById ("WAIT").style.width = w_width;
			document.getElementById ("WAIT").style.height = w_height;				
			document.getElementById ("WAIT").style.visibility = "visible";
		</script>--%>
</head>
<body MS_POSITIONING="GridLayout">
    <form id="frmRubrica" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Importa Rubrica da foglio Excel" />
    <table width="95%" align="center">
        <tr>
            <td align="center" class="titolo">&nbsp;<asp:Label ID="lbl_avviso" runat="server" ForeColor="#C00000" Visible="true"></asp:Label></td>
        </tr>
    </table>    
    <table width="95%" align="center">
        <tr>
			<td class="testo_grigio_scuro" style="padding-left:10px;">
			    Seleziona il file excel da importare:
		    </td>
		</tr>
        <tr>
            <td><INPUT type="file" runat="server" class="testo10N" id="uploadFile" size="62" name="uploadFile"></td>
        </tr>
        <tr>
            <td class="testo_grigio_scuro" style="padding:5px;" align="center">
                <br />
                <asp:Button id="btn_importa" runat="server" Text="Importa" CssClass="pulsante" Width="110px" OnClick="btn_importa_Click"></asp:Button>&nbsp;
                <asp:Button id="btn_annulla" runat="server" Text="Chiudi" CssClass="pulsante" Width="110px" OnClick="btn_annulla_Click"></asp:Button>&nbsp;
                <asp:Button ID="btn_log" runat="server" Text="Log..." CssClass="pulsante" Width="110px" OnClick="btn_log_Click" Visible="False"/>
            </td>
        </tr>
    </table>    	
    <DIV id="WAIT" style="BORDER-RIGHT: #f5f5f5 1px solid; BORDER-TOP: #f5f5f5 1px solid; VISIBILITY: hidden; BORDER-LEFT: #f5f5f5 1px solid; BORDER-BOTTOM: #f5f5f5 1px solid; POSITION:absolute; BACKGROUND-COLOR: #f5f5f5;">
	    <table height="420px" cellSpacing="0" cellPadding="0" width="100%" border="0">
		    <tr>
			    <td style="FONT-SIZE: 11pt; FONT-FAMILY: Verdana" vAlign="top" align="center">
				    <br><br><br><br>
					Attendere prego,<br><br>
					importazione dei dati in corso...<br><br>
				</td>
			</tr>
		</table>
	</DIV>		
    </form>
</body>
</html>
