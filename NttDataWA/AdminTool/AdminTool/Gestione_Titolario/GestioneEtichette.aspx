<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneEtichette.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_Titolario.GestioneEtichette" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html>
<head runat="server">
    <title></title>	
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" Content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
</head>
<body MS_POSITIONING="GridLayout">
    <form id="form1" runat="server">
        <table style="width:100%;" >
	        <tr>
	            <td class="titolo" style="background-color:#e0e0e0; height:20px; width:80%; text-align:center;">
	                <asp:Label id="lbl_titolo" runat="server">Etichette titolari</asp:Label>
	            </td>
	            <td style="background-color:#e0e0e0; height:20px; width:80%;">	               
	               <asp:Button id="bnt_conferma" runat="server" Text="Conferma" 
                        CssClass="testo_btn_p" onclick="bnt_conferma_Click"></asp:Button>&nbsp;
	               <asp:Button id="btn_chiudi" runat="server" Text="Chiudi" CssClass="testo_btn_p" onclick="btn_chiudi_Click"></asp:Button>
	            </td>
            </tr>	
        </table>
        <table style="margin-top:5px; border:1px #810d06 solid; background-color:#f6f4f4; height:90%; width:100%;">
            <tr>
                <td width="20%" class="testo_grigio_scuro">
                    <asp:Label ID="lbl_etTitolario" Text="Titolario" runat="server"></asp:Label>
                </td>
                <td width="80%">
                    <asp:TextBox ID="txt_etTitolario" runat="server" class="testo_grigio_scuro" Width="100%" MaxLength="12"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="testo_grigio_scuro">
                    <asp:Label ID="lbl_etLivello1" Text="Livello 1" runat="server"></asp:Label>
                </td>
                <td class="testo_grigio_scuro">
                    <asp:TextBox ID="txt_etLivello1" runat="server" class="testo_grigio_scuro" Width="100%" MaxLength="12"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="testo_grigio_scuro">
                    <asp:Label ID="lbl_etLivello2" Text="Livello 2" runat="server"></asp:Label>
                </td>
                <td class="testo_grigio_scuro">
                    <asp:TextBox ID="txt_etLivello2" runat="server" class="testo_grigio_scuro" Width="100%" MaxLength="12"></asp:TextBox>            
                </td>
            </tr>
            <tr>
                <td class="testo_grigio_scuro">
                    <asp:Label ID="lbl_etLivello3" Text="Livello 3" runat="server"></asp:Label>
                </td>
                <td class="testo_grigio_scuro">
                    <asp:TextBox ID="txt_etLivello3" runat="server" class="testo_grigio_scuro" Width="100%" MaxLength="12"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="testo_grigio_scuro">
                    <asp:Label ID="lbl_etLivello4" Text="Livello 4" runat="server"></asp:Label>
                </td>
                <td class="testo_grigio_scuro">
                    <asp:TextBox ID="txt_etLivello4" runat="server" class="testo_grigio_scuro" Width="100%" MaxLength="12"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="testo_grigio_scuro">
                    <asp:Label ID="lbl_etLivello5" Text="Livello 5" runat="server"></asp:Label>
                </td>
                <td class="testo_grigio_scuro">
                    <asp:TextBox ID="txt_etLivello5" runat="server" class="testo_grigio_scuro" Width="100%" MaxLength="12"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="testo_grigio_scuro">
                    <asp:Label ID="lbl_etLivello6" Text="Livello 6" runat="server"></asp:Label>
                </td>
                <td class="testo_grigio_scuro">
                    <asp:TextBox ID="txt_etLivello6" runat="server" class="testo_grigio_scuro" Width="100%" MaxLength="12"></asp:TextBox>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
