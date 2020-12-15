<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="AnteprimaProfDinamica.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.AnteprimaProfDinamica" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat="server">
		<META HTTP-EQUIV="Pragma" CONTENT="no-cache">
        <META HTTP-EQUIV="Expires" CONTENT="-1">	
	    <base target="_self" />
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/ETcalendar.js"></script>
        <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript">
		    function chiudiFinestra() {
		        window.close();

		    }
		</script>
		<script type="text/javascript" language="javascript">
		    function clearSelezioneEsclusiva(id, numeroDiScelte) {
		        numeroDiScelte--;
		        while (numeroDiScelte >= 0) {
		            var elemento = id + "_" + numeroDiScelte;
		            document.getElementById(elemento).checked = false;
		            numeroDiScelte--;
		        }
		    }	    
    	</script>
        <script>
            var w = window.screen.width;
            var h = window.screen.height;
            var new_w = (w - 100) / 2;
            var new_h = (h - 400) / 2;

            function apriPopupAnnullamentoContatore(idOggetto, docNumber) {
                //window.open('AnteprimaProfDinamica.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=420,scrollbars=YES');
                window.showModalDialog('../popup/annullaContatore.aspx?idOggetto=' + idOggetto + '&docNumber=' + docNumber + '', '', 'dialogWidth:510px;dialogHeight:180px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
            }
        </script>
	</head>
	<body style="margin:2px;">
		<form id="Form1" method="post" runat="server">
		<asp:ScriptManager ID="ScriptManagerProfDinam" AsyncPostBackTimeout="360000" runat="server"></asp:ScriptManager>
        
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Anteprima" />
			<table width="100%" border="0">
				<tr>
					<td class="titolo" 
						align="center" bgColor="#e0e0e0" height="20"><asp:label id="lbl_NomeModello" runat="server" Font-Bold="True" Width="100%"></asp:label></td>
					<!--
					<td class="titolo" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid"
						align="center" bgcolor="#e0e0e0" height="20">
						<asp:Button id="btn_ConfermaProfilazione" runat="server" Text="Conferma" CssClass="pulsante" Width="80px"></asp:Button></td>
					<td class="titolo" style="BORDER-RIGHT: #810d06 1px solid; BORDER-TOP: #810d06 1px solid; BORDER-LEFT: #810d06 1px solid; BORDER-BOTTOM: #810d06 1px solid"
						align="center" width="25" bgcolor="#e0e0e0" height="20">
						<asp:Button id="btn_Chiudi" runat="server" Text="Chiudi" CssClass="pulsante" Width="80px"></asp:Button></td>
					-->
				</tr>
				<tr>
					<td align="center" colspan="3"><asp:label id="Label_Avviso" runat="server" Font-Bold="True" Font-Size="12px" ForeColor="Red"
							Visible="False"></asp:label></td>
				</tr>
                
                <tr>
					<td class="td_profDinamica" colspan="3">
                        <asp:panel id="panel_Contenuto" runat="server">
                            <div style="float:right;">
                                <cc1:imagebutton id="btn_HistoryField" Runat="server" Width="18" AlternateText="Storia modifica campi profilati" DisabledUrl="../images/proto/storia.gif"
									Tipologia="DO_PROFIL_HISTORY" Height="17" ImageUrl="../images/proto/storia.gif"></cc1:imagebutton>
                            </div>
                        </asp:panel>
                    </td>
                </tr>

				<tr>
				    <td class="td_profDinamica" colspan="3" align="center">
				    <asp:ImageButton ID="btn_salva" runat="server" SkinID="salva" Visible="false" />
				    <asp:ImageButton ID="btnChiudi" runat="server" SkinID="chiudi" Visible="false" />
				    </td>
				</tr>
			</table>
		</form>
	</body>
</html>
