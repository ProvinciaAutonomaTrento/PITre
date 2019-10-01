<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CalculateAtipicitaAfterInsertRole.aspx.cs"
    Inherits="SAAdminTool.AdminTool.Gestione_Organigramma.CalculateAtipicitaAfterInsertRole" %>

<%@ Register src="../../UserControls/AjaxMessageBox.ascx" tagname="AjaxMessageBox" tagprefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register src="../UserControl/CalculateAtipicitaOptions.ascx" tagname="CalculateAtipicitaOptions" tagprefix="uc2" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <script language="javascript" type="text/javascript">
        // Funzione utilizzata per mostrare la schermata
        // di waiting
        function showWait(sender, args) {
            // Viene visualizzato il popup di wait    
            this._popup = $find('mdlPopupWait');
            this._popup.show();

        }
    </script>
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="36000" />
    <uc1:AjaxMessageBox ID="AjaxMessageBox" runat="server" />
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Calcola atipicità inserimento ruolo" />
    <asp:UpdatePanel ID="upAtipicita" runat="server">
        <ContentTemplate>
            <div style="margin:5px;">
                <uc2:CalculateAtipicitaOptions ID="calculateAtipicitaOptions" runat="server" />
            </div>
            <div style="margin:5px; text-align:center;" >
                <asp:Button ID="btnOk" runat="server" ToolTip="Continua" Text="Continua" 
                    onclick="btnOk_Click" OnClientClick="showWait();" CssClass="testo_btn_org" />&nbsp;
                <asp:Button ID="btnClose" runat="server" Text="Annulla" CssClass="testo_btn_org" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <cc1:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
        BackgroundCssClass="modalBackground" />
    <div id="Wait" runat="server" style="display: none; font-weight: bold; font-size: xx-large;
        font-family: Arial; text-align: center;">
        <span style="font-family: Arial; font-size: small; text-align: center;">
            <asp:UpdatePanel ID="pnlUP" runat="server">
                <ContentTemplate>
                    <input id="hfTargetPerc" runat="server" type="hidden" value="0" />
                    <br />
                    <asp:Label ID="lblInfo" runat="server">Inserimento in corso...</asp:Label><br />
                </ContentTemplate>
            </asp:UpdatePanel>
            (L'operazione può richiedere diversi minuti.)</span><br />
        <br />
    </div>
    </form>
</body>
</html>
