<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportaOggettario.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_Oggettario.ImportaOggettario" validateRequest="false"%>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
        <title></title>
    <script language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">		
	<base target="_self"/>
	<SCRIPT language="javascript">
			function apriLog() 
			{			
			    var myUrl = "LogImport.aspx";				
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:700px;dialogHeight:500px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 									
			}
			
			function wait()
			{
			    document.getElementById("lbl_avviso").innerHTML = "Importazione in corso ...";
			}			
	</script>
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" bgcolor="#f6f4f4" height="100%"  >
    <form id="form2" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Importa Oggettario" />
         <uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
        <table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
            <tr>
                <td>
                    <!-- TESTATA CON MENU' -->
                    <uc1:testata id="Testata" runat="server"></uc1:testata>
                </td>
            </tr>
            <tr>
                <td class="testo_grigio_scuro" bgcolor="#c0c0c0" height="20">
                    <asp:label id="lbl_position" runat="server"></asp:label>
                </td>
            </tr>
            <tr>
                <td valign="top" align="center" bgcolor="#f6f4f4" height="100%" width="100%">
                    <!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
    <table width="95%" align="center">
        <tr>
            <td class="titolo" align="center" bgColor="#e0e0e0" height="20"><asp:label id="lbl_import" runat="server">Importazione oggettario da foglio Excel.</asp:label></td>
        </tr>
        <tr>
            <td align="center" class="titolo">&nbsp;<asp:Label ID="lbl_avviso" runat="server" ForeColor="#C00000" Visible="true"></asp:Label></td>
        </tr>
    </table>    
    <table width="95%" align="center" bgColor="#f6f4f4" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid">
        <tr>
            <td><INPUT type="file" runat="server" class="testo" id="uploadFile" size="62" name="uploadFile"></td>
        </tr>
         <tr><td class="testo"><asp:CheckBox ID="ckb_update" runat="server" Checked="true" Text="Aggiorna la tabella se l'oggetto è già presente" /></td></tr>
       <tr>
            <td align="center">
            <br /><asp:Button id="btn_importa" runat="server" Text="Importa" CssClass="testo_btn" Width="110px" OnClick="btn_importa_Click" OnClientClick="wait();"></asp:Button>
            &nbsp;<asp:Button ID="btn_log" runat="server" Text="Log..." CssClass="testo_btn" Width="110px" OnClick="btn_log_Click" Visible="False"/>
            </td>
        </tr>
    </table>    			
                 </td>
            </tr>
        </table>
   </form>
</body>
</html>
