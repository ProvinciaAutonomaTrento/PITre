<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IndiceSistematico.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_Titolario.IndiceSistematico" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html>
    <HEAD runat = "server">
        <title></title>	
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
        <form id="Form1" method="post" runat="server">
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Indice Sistematico" />
		    <table style="width:100%;" >
		        <tr>
		            <td class="titolo" style="background-color:#e0e0e0; height:20px; width:80%; text-align:center;">
		                <asp:Label id="lbl_titolo" runat="server">Indice Sistematico</asp:Label>
		            </td>
		            <td style="background-color:#e0e0e0; height:20px; width:80%;">
		               <asp:Button id="btn_chiudi" runat="server" Text="Chiudi" 
                            CssClass="testo_btn_p" onclick="btn_chiudi_Click"></asp:Button>
		            </td>
                </tr>	
            </table>
            <table style="margin-top:5px; border:1px #810d06 solid; background-color:#f6f4f4; height:96%; width:100%;">
			    <tr>
			        <td>
			            <table style="width:100%; height:100%">
	                        <tr>
	                            <td colspan="3" class="testo_grigio_scuro" style="height:10%;">
	                                <asp:Label ID="lbl_titolario" runat="server" CssClass="testo_grigio_scuro" Width="100%"></asp:Label>
	                                <asp:Label ID="lbl_nodoTitolario" runat="server" CssClass="testo_grigio_scuro" Width="100%"></asp:Label>
	                            </td>	                            
			                </tr>
			                <tr>
			                    <td colspan="3" class="testo_grigio_scuro" style="height:10%;">
			                        Nuova Voce <br /> 
			                        <asp:TextBox ID="txt_nuovaVoce" runat="server" Width="40%" CssClass="testo_grigio_scuro"></asp:TextBox>
			                        <asp:ImageButton ID="btn_aggiungiVoce" runat="server" 
                                        ImageUrl="../images/aggiungi.gif" ToolTip="Aggiungi voce in disponibili" 
                                        onclick="btn_aggiungiVoce_Click" />
			                        <asp:ImageButton ID="btn_rimuoviVoce" runat="server" 
                                        ImageUrl="../images/elimina.gif" ToolTip="Elimina voce da disponibili" 
                                        onclick="btn_rimuoviVoce_Click" style="width: 18px" />
			                    </td>
			                </tr>
			                <tr>
			                    <td class="testo_grigio_scuro" style="width:40%;">
			                        Voci Disponibili <br />
   			                        <asp:ListBox ID="lbx_vociDisponibili" runat="server" CssClass="testo_grigio_scuro" SelectionMode="Single" Width="100%" Height="98%"></asp:ListBox>
			                    </td>
			                    <td align="center" valign="middle" style="width:20%;">
			                        <asp:Button ID="btn_associaVoce" runat="server" CssClass="testo_btn_p" 
                                        Text=">>" onclick="btn_associaVoce_Click" />
			                        <br />
			                        <br />
			                        <asp:Button ID="btn_disassociaVoce" runat="server" CssClass="testo_btn_p" 
                                        Text="<<" onclick="btn_disassociaVoce_Click" />
			                    </td>
			                    <td class="testo_grigio_scuro" style="width:40%;">
			                        Voci Associate <br />
			                        <asp:ListBox ID="lbx_vociAssociate" runat="server" CssClass="testo_grigio_scuro" SelectionMode="Single" Width="100%" Height="98%"></asp:ListBox>
			                    </td>
			                </tr>
			            </table>				    
			        </td>
			    </tr>
		    </table>	
        </form>
    </body>
</html>
