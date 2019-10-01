<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneStato.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_DiagrammiStato.GestioneStato" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>DOCSPA - AMMINISTRAZIONE > Gestione stato</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" Content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
</head>
<body MS_POSITIONING="GridLayout">
    <form id="form1" runat="server">
        <table width="100%">
		    <tr>
			    <td class="titolo"  align="center" bgColor="#e0e0e0" height="20">
				    <asp:Label id="lbl_titolo" runat="server"></asp:Label>
				</td>
				<td align="center" width="50%" bgColor="#e0e0e0">
                    <asp:Button ID="btn_conferma" runat="server" CssClass="testo_btn_p" Text="Conferma" OnClick="btn_conferma_Click" />&nbsp;
                    <asp:Button id="btn_chiudi" runat="server" Text="Chiudi" CssClass="testo_btn_p" OnClick="btn_chiudi_Click"></asp:Button>
                </td>
			</tr>
        </table>
        
        <BR>
	    <table width="100%" bgColor="#f6f4f4" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid">
		    <tr>
		        <td class="testo_grigio_scuro" width="30%">Descrizione stato</td>
		        <td width="70%" align="right">
                    <asp:TextBox ID="txt_descrizioneStato" runat="server" CssClass="testo" Width="97%"></asp:TextBox>
                </td>
		    </tr>
		    <tr>
		        <td class="testo_grigio_scuro">Stato iniziale</td>
		        <td>
                    <asp:CheckBox ID="cb_statoIniziale" runat="server" AutoPostBack="True" 
                        oncheckedchanged="cb_statoIniziale_CheckedChanged" />
                </td>
		    </tr>
            <tr>
		        <td class="testo_grigio_scuro">Stato di Sistema</td>
		        <td>
                    <asp:CheckBox ID="cbxTransactionSystem" runat="server" Checked="false" CssClass="testo"/>
                </td>
		    </tr>
		    <asp:Panel ID="pnl_convertiInPdf" runat="server" Visible="false">
		    <tr>
		        <td class="testo_grigio_scuro">Converti in PDF</td>
		        <td>
		            <asp:CheckBox ID="cb_convertiInPdf" runat="server" />
		        </td>
		    </tr>
		    </asp:Panel>
            <asp:Panel ID="pnl_consolidamento" runat="server" Visible="false">
            <tr>
		        <td class="testo_grigio_scuro">Consolidamento</td>
		        <td>
		            <asp:DropDownList ID="cb_consolidamento" runat="server" CssClass="testo">
                        <asp:ListItem Selected="True" Value="0" Text=""></asp:ListItem>
                        <asp:ListItem Value="1" Text="Consolida documenti"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Consolida metadati documenti"></asp:ListItem>
                    </asp:DropDownList>
		        </td>
		    </tr>            
            </asp:Panel>
		    <tr>
		        <td class="testo_grigio_scuro">Stato non ricercabile</td>
		        <td>
		            <asp:CheckBox ID="cb_statoNonRicercabile" runat="server" />
		        </td>
		    </tr>   
		</table>
    </form>
</body>
</html>
