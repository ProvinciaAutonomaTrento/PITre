<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportaIndice.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Titolario.ImportaIndice" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">		
	<base target="_self"/>
	<script language="javascript">
	    function wait()
		{
		    document.getElementById("lbl_avviso").innerHTML = "Importazione in corso ...";
		}
		
		function apriLog() 
		{			
		    var myUrl = "LogImportIndiceSistematico.aspx";				
			rtnValue = window.showModalDialog(myUrl,"","dialogWidth:700px;dialogHeight:500px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 									
		}	
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Importa Titolario" />
    <table width="95%" align="center">
        <tr>
            <td class="titolo" align="center" bgColor="#e0e0e0" height="20"><asp:label id="lbl_import" runat="server">Importazione dell'indice sistematico da foglio Excel.</asp:label></td>
        </tr>
        <tr>
            <td align="center" class="titolo">&nbsp;<asp:Label ID="lbl_avviso" runat="server" ForeColor="#C00000" Visible="true"></asp:Label></td>
        </tr>
    </table>    
    <table width="95%" align="center" bgColor="#f6f4f4" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid">
        <tr>
            <td><INPUT type="file" runat="server" class="testo" id="uploadFile" size="62" name="uploadFile"></td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:Button id="btn_importa" runat="server" Text="Importa" CssClass="testo_btn" Width="110px" OnClick="btn_importa_Click" OnClientClick="wait();"></asp:Button>
                &nbsp;
                <asp:Button id="btn_annulla" runat="server" Text="Chiudi" CssClass="testo_btn" Width="110px" OnClick="btn_annulla_Click"></asp:Button>
                &nbsp;
                <asp:Button ID="btn_log" runat="server" Text="Log..." CssClass="testo_btn" Width="110px" OnClick="btn_log_Click" Visible="False"/>
            </td>
        </tr>
    </table>    	
    </form>
</body>
</html>
