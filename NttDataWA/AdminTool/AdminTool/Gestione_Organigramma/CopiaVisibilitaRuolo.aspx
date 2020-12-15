<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CopiaVisibilitaRuolo.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_Organigramma.CopiaVisibilitaRuolo" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> 

<html>
<head runat="server">
    <title></title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
     <base target=_self />
</head>
<body style="margin-top:5px; margin-left:5px; margin-bottom:5px; margin-right:5px">
    <form id="form1" runat="server">
    
    <script language="javascript" type="text/javascript">
        function showWait() {
            this._popup = $find('mdlPopupWait');
            this._popup.show();
        }

        function hideWait() {
            this._popup = $find('mdlPopupWait');
            this._popup.hide();
        }

        // Funzione per verificare se è possibile deflaggare una checkbox
        // utilizzata per specificare cosa copiare (non è possibile delselezionare 
        // tutti e tre i checkbox
        function chekIfCanDeflagChackbox(checkbox) {
            // Recupero di un riferimento ai tre checkbox per documenti protocollati, non protocollati e fascicoli procedimentali
            var docProtocollati = document.getElementsByName('cbx_docProtocollati');
            var docNonProtocollati = document.getElementsByName('cbx_docNonProtocollati');
            var fascProcedimentali = document.getElementsByName('cbx_fascicoliProcedimentali');

            // Se tutti e tre i checkbox sono deflaggati non si può deflaggare il checkbox passato per parametro
            if (docProtocollati[0].checked == '' && docNonProtocollati[0].checked == '' && fascProcedimentali[0].checked == '') { 
                alert('Almeno uno dei flag \'Documenti Protocollati\', \'Documenti non protocollati\' e \'Fascicoli procedimentali\' deve essere selezionato.');
                checkbox.checked = 'checked';
            }
        }  
    </script> 
    
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeOut="360000"></asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Copia Visibilità Ruolo" />

    <!--Testata-->
    <table width="100%" align="center">
		<tr>
			<td class="testo_grigio_scuro_grande" align="right" height="10">|&nbsp;&nbsp;<a onclick="javascript: self.close();" href="#">Chiudi</A>&nbsp;&nbsp;|</td>
		</tr>
		<tr>
			<td class="testo_grigio_scuro_grande" align="right" height="10">&nbsp;</td>
		</tr>
	</table>

    <!--Ruoli-->
    <table width="100%" align="center" class="contenitore">
        <tr>
            <td class="testo_grigio_scuro">Ruolo origine :</td>
            <td><asp:Label ID="lbl_ruoloOrigine" runat="server" class="titolo_piccolo" style="font-weight:bold"></asp:Label></td>
        </tr>
        <tr>
            <td class="testo_grigio_scuro">Ruolo destinazione :</td>
            <td>
                <asp:TextBox ID="txt_codRuoloDest" runat="server" CssClass="testo_grigio_scuro" Width="15%" OnTextChanged="txt_codRuoloDest_TextChanged" AutoPostBack="true"></asp:TextBox>&nbsp;
                <asp:TextBox ID="txt_descRuoloDest" runat="server" ReadOnly="true" CssClass="testo_grigio_scuro" Width="75%"></asp:TextBox>
            </td>
        </tr>
    </table>
    <!--Parametri di ricerca-->
    <table width="100%" align="center" class="contenitore" style="margin-top:20px">
        <tr>
            <td style="vertical-align:top">
                 <asp:Panel ID="pnl_doc" runat="server" GroupingText="Documenti" CssClass="testo_grigio_scuro">
                    <asp:CheckBox ID="cbx_docProtocollati" Text="Doc. Protocollati" runat="server" Checked="true" onclick="chekIfCanDeflagChackbox(this)" />
                    <br />   
                    <asp:CheckBox ID="cbx_docNonProtocollati" Text="Doc. Non Protocollati" runat="server" Checked="true" onclick="chekIfCanDeflagChackbox(this)"/>
                 </asp:Panel>
            </td>
            <td style="vertical-align:top">
                <asp:Panel ID="pnl_fasc" runat="server" GroupingText="Fascicoli" CssClass="testo_grigio_scuro">
                    <asp:CheckBox ID="cbx_fascicoliProcedimentali" Text="Fascicoli Procedimentali" runat="server" Checked="true" onclick="chekIfCanDeflagChackbox(this)"/>                    
                 </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="vertical-align:top">
                <asp:Panel ID="Panel1" runat="server" GroupingText="Documenti/Fascicoli" CssClass="testo_grigio_scuro">
                    <asp:CheckBox ID="cbx_visibilitaAttiva" Text="Solo visibilità attiva" ToolTip="Copia limitata a documenti/fascicoli di cui il ruolo &egrave; proprietario o su cui ha acquisito visibilit&agrave; per trasmissione diretta" runat="server" Checked="false"/>
                    <br />   
                    <asp:CheckBox ID="cbx_precCopiaVisibilita" Text="Precedente copia di visibilità" ToolTip="Copia estesa a documenti/fascicoli su cui il ruolo ha visibilit&agrave; per precedenti operazioni di copia visibilit&agrave;" runat="server" Checked="false"/>
                    <br /><br />
                    Estendi visibilità ai superiori del ruolo di destinazione :   
                    <asp:RadioButtonList ID="rbl_estendiVisibilita" runat="server" RepeatDirection="Horizontal" CssClass="testo_grigio_scuro">
                        <asp:ListItem Value="NO" Text="Non estendere" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="SI" Text="Estendi a tutti i documenti / fascicoli"></asp:ListItem>
                        <asp:ListItem Value="NO_ATIPICI" Text="Escludi i documenti / fascicoli atipici"></asp:ListItem>
                    </asp:RadioButtonList>                 
                 </asp:Panel>
            </td>
        </tr>
    </table>

    <!--Pulsanti-->
    <table width="100%" align="center" class="contenitore" style="margin-top:20px">
        <tr>
            <td align="right">
                <asp:Button ID="btn_conferma" runat="server" Text="Conferma" CssClass="testo_btn" OnClick="btn_conferma_Click" OnClientClick="showWait();" />
                <cc1:ConfirmButtonExtender ID="ConfirmButton" runat="server" ConfirmText="L'operazione potrebbe richiedere diversi minuti ..." TargetControlID="btn_conferma"></cc1:ConfirmButtonExtender>   
            </td>
        </tr>
    </table>

    <!-- PopUp Wait-->
    <cc1:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait" BackgroundCssClass="modalBackground" />
    <div id="Wait" runat="server" style="display:none; font-weight:bold; font-size:xx-large; font-family:Arial; text-align:center;">Attendere prego ...</div>

    </form>
</body>
</html>
