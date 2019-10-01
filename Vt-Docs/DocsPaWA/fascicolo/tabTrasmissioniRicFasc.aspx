<%@ Page language="c#" Codebehind="tabTrasmissioniRicFasc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.tabTrasmissioniRicFasc" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="cc1" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base id=tagBase runat=server target=_self></base>
		<% Response.Expires=-1; %>
        <script type="text/javascript" language="javascript">
            // Mostra clessidra di attesa
            function showWait(btnRef) {
                try {
                    if (btnRef != null && btnRef != undefined) {
                        btnRef.style.cursor = 'wait';
                        please_wait.style.display = '';
                        var height, width;
                        height = document.body.offsetHeight / 2 - 90 / 2;
                        width = document.body.offsetWidth / 2 - 350 / 2;
                        please_wait.style.top = height;
                        please_wait.style.left = width;
                    }
                } catch (e) {

                }
            }
        </script>
	</HEAD>
	<body rightmargin="0" topmargin="0" bottommargin="0" leftmargin="0">
    <form id="tabTrasmissioniRicFasc" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Dettaglio della trasmissione" />
    <table align="center" cellspacing="0" border="0" bordercolor="#ffffff"
        cellpadding="0" width="100%">
        <tr>
            <td height="3">
            </td>
        </tr>
        <tr>
            <td height="15" align="center" class="infoDT">
                <asp:Label ID="titolo" runat="server" CssClass="titolo_rosso">
					DETTAGLIO TRASMISSIONE SELEZIONATA</asp:Label>
            </td>
        </tr>
        <tr>
            <td style="height: 19px" width="100%">
                <asp:Table ID="tbl_trasmRic" runat="server" CellSpacing="1" HorizontalAlign="Center"
                    BorderWidth="1px" CellPadding="1" Width="100%">
                </asp:Table>
            </td>
        </tr>
        <tr valign="top">
            <td align="center">
                <asp:Table ID="Table1" runat="server" HorizontalAlign="Center" Width="100%" BorderWidth="1px"
                    BorderColor="LightGrey" BorderStyle="Solid" GridLines="Both">
                </asp:Table>
            </td>
        </tr>
        <tr id="tr_enableAccRif" runat="server" visible="false">
            <td class="testo_grigio">
                Note di Accettazione/Rifiuto&nbsp;
                <asp:TextBox ID="txt_noteAccRif" runat="server" CssClass="testo_grigio" Width="320px"
                    Height="20px" MaxLength="255" TabIndex="0"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td height="10">
                &nbsp;
            </td>
        </tr>
        <tr id="tr2_enableAccRif" runat="server" visible="false">
            <td class="testo_grigio" align="center">
                <asp:Button ID="btn_accetta" runat="server" CssClass="pulsante" Height="20px" Text="Accetta" OnClientClick="showWait(this);">
                </asp:Button>&nbsp;
                <asp:Button ID="btn_AdL" runat="server" CssClass="pulsante" Height="20px" Text="Accetta e AdL" OnClientClick="showWait(this);"
                    ToolTip="Accetta ed Inserisci il documento in Area di Lavoro"></asp:Button>&nbsp;
                <asp:Button ID="btn_rifiuta" runat="server" CssClass="pulsante" Height="20px" Text="Rifiuta" OnClientClick="showWait(this);">
                </asp:Button>&nbsp;
            </td>
        </tr>
        <tr id="tr2_vistoeADL" runat="server" visible="false">
            <td class="testo_grigio" align="center">
                <asp:Button ID="btn_visto" runat="server" CssClass="pulsante" Height="20px" Text="Visto" OnClientClick="showWait(this);" />&nbsp;
                <asp:Button ID="btn_vistoADL" runat="server" CssClass="pulsante" Height="20px" Text="Visto e AdL" OnClientClick="showWait(this);" />&nbsp;
            </td>
        </tr>
        <tr>
            <td height="80%" valign="top">
                &nbsp;
                <cc1:MessageBox ID="MessageBox1" runat="server" Width="90%"></cc1:MessageBox>
            </td>
        </tr>
        <tr>
            <td height="20%" align="center">
                <asp:Button ID="btn_chiudi" runat="server" CssClass="pulsante" Height="20px" Text="Chiudi"
                    Visible="false" OnClientClick="self.close(); return false;" />
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnl_destinatari" Visible="false" runat="server">
        <table align="center" cellspacing="0" border="0" cellpadding="0" width="100%"
            border="0">
            <tr>
                <td height="10px;">
                </td>
            </tr>
            <tr>
                <td height="15" align="center" class="infoDT">
                    <asp:Label ID="lbl_dett_destinatari" runat="server" CssClass="titolo_rosso">DETTAGLIO DESTINATARI</asp:Label>
                </td>
            </tr>
            <tr valign="top">
                <td align="center">
                    <asp:Table ID="tblTx" runat="server" HorizontalAlign="Center" Width="100%" BorderWidth="1"
                        BorderStyle="Solid" GridLines="Both" Height="100%">
                    </asp:Table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <div id="please_wait" style="display:none;z-index:1000; border-right: #000000 2px solid; border-top: #000000 2px solid;
        border-left: #000000 2px solid; border-bottom: #000000 2px solid; position: absolute;
        background-color: #d9d9d9">
        <table cellspacing="0" cellpadding="0" width="350px" border="0">
            <tr>
                <td valign="middle" style="font-weight: bold; font-size: 12pt; font-family: Verdana"
                    align="center" width="350px" height="90px">
                    Attendere, prego...
                </td>
            </tr>
        </table>
    </div>
    </form>
	</body>
</HTML>
