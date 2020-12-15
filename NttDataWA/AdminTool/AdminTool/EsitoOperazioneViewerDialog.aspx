<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EsitoOperazioneViewerDialog.aspx.cs" Inherits="SAAdminTool.AdminTool.EsitoOperazioneViewerDialog" %>
<%@ Register src="UserControl/EsitoOperazioneViewer.ascx" tagname="EsitoOperazioneViewer" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <base target = "_self" />    
    <LINK href="CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <script type ="text/javascript">
        function CloseDialog() { window.close();}
    </script> 
</head>
<body>
    <form id="frmEsitoOperazioneViewerDialog" runat="server" PageName = "AMMINISTRAZIONE > Esito operazione">
        <div>
            <table id="tblContainer" cellSpacing="1" cellPadding="0" width="95%" border="0" align="center">
            <tr>
                <td align="center" class="pulsanti">
				    <asp:Label id="lblTitle" CssClass="testo_grigio_scuro" runat="server"></asp:Label>
                </td>
            </tr>
			<tr style="height: 3px">
				<td></td>
			</tr>
            <tr>
                <td align="center">
                    <div style ="overflow:auto; height: 400px; width: 100%" id="divControlContainer" >
                        <uc1:EsitoOperazioneViewer ID="viewer" runat="server" />
                    </div>
                </td>
            </tr>    
            <tr>
                <td vAlign="middle" align="center">
                    <br />
				    <asp:button id="btnCancel" runat="server" CssClass="testo_btn" Text="Chiudi" OnClientClick="CloseDialog();"></asp:button>
                </td>
		    </tr>
          </table>
        </div>
    </form>
</body>
</html>
