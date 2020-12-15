<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvioUORubricaComune.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_RubricaComune.InvioUORubricaComune" %>
<%@ Register tagprefix="uc1" src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider"  %>
<%@ Register TagPrefix="uc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>

<%@ Register src="../UserControl/RfInfo.ascx" tagname="RfInfo" tagprefix="uc3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title runat = "server"></title>
    <base target="_self" />
    <LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <script type = "text/javascript">
        function ShowMessage(message) { alert(message); }
        function CloseDialog() { window.returnValue = (document.getElementById('txtReturnValue').value == 'true'); window.close(); }

        function verificaCF() {
            if (document.getElementById('<%= RfInfo1.FindControl("txtCodiceFiscale").ClientID %>') != null) {
                var cf = document.getElementById('<%= RfInfo1.FindControl("txtCodiceFiscale").ClientID %>').value;
                if (cf.length == 16)
                    return confirm('Per un corrispondente di tipo UO stai inserendo/modificando il campo Codice fiscale con uno di tipo persona, sei sicuro di voler proseguire?');
            }
            return true;
        }
    </script>
</head>
<body>
    <form id="frmInvioRubricaComune" runat="server">
        <uc1:AppTitleProvider ID="titleProvider" runat="server" PageName="Invio a Rubrica Comune" />
        <input type = "hidden" id = "txtReturnValue" runat = "server" value = "false" />
        <table cellSpacing="2" cellPadding="0" width="100%" height="150px" border="0" class = "contenitore">
			<tr align="center" class="titolo">
				<td colspan = "2" class="titolo_pnl" vAlign="middle">
				    <asp:Label ID = "lblTitolo" runat = "server" Text = "Invio a Rubrica Comune" />
				</td>
			</tr>
			<tr class="testo_grigio_scuro" >
				<td colspan = "2">
				</td>
			</tr>
			<tr class="testo_grigio_scuro" >
				<td align = "left" width = "25%">
				    <asp:Label ID = "lblCodice" runat = "server" Text = "Codice: *"></asp:Label>
				</td>
				<td align = "left" width = "75%">
				    <asp:Label ID = "txtCodice" runat = "server" ReadOnly = "true" Width = "90%" CssClass = "testo" />
				</td>					
			</tr>
			<tr align="center" class="testo_grigio_scuro">
				<td align = "left" width = "25%">
				    <asp:Label ID = "lblDescrizione" runat = "server" Text = "Descrizione: *"></asp:Label>
				</td>
				<td align = "left" width = "75%" >
				    <asp:TextBox ID = "txtDescrizione" runat = "server" Width = "90%" CssClass ="testo"></asp:TextBox>
				</td>					
			</tr>		
            <tr align="left" valign="top" id="trRfInfo" runat="server">
                <td colspan="2">
                    <uc3:RfInfo ID="RfInfo1" runat="server" />
                </td>
            </tr>
            <tr align="center" class="testo_grigio_scuro">
                <td colspan="2">
                
                </td>
            </tr>					
            <tr align="center" class="testo_grigio_scuro">
				<td colspan = "2">
				</td>
			</tr>						
			<tr align="center" class="titolo">
				<td colspan = "2" class="titolo_pnl">
				    <asp:Button ID = "btnInvia"	 runat = "server" CssClass = "testo_btn_p" OnClientClick = "return verificaCF();"
                        Text = "Invia modifica" onclick="btnInvia_Click" />&nbsp;
				    <asp:Button ID = "btnChiudi" runat = "server" CssClass = "testo_btn_p" 
                        Text = "Chiudi" onclick="btnChiudi_Click" OnClientClick = "CloseDialog();" />
				</td>
			</tr>
        </table>
    </form>
</body>
</html>
