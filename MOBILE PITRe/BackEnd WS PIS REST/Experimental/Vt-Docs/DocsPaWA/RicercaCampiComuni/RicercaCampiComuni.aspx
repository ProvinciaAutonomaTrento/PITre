<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RicercaCampiComuni.aspx.cs" Inherits="DocsPAWA.RicercaCampiComuni.RicercaCampiComuni" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
<head runat="server">
    <title></title>
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">   
    <LINK href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
	<script language="JavaScript" src="../CSS/ETcalendar.js"></script>	
    <script language="javascript" id="btn_ricerca_Click" event="onclick()" for="btn_ricerca">
			window.document.body.style.cursor='wait';
			WndWait();
    </script>
</head>
<body MS_POSITIONING="GridLayout">
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Ricerca Campi Comuni" />
        <TABLE id="tbl_contenitore" height="100%" cellSpacing="0" cellPadding="0" width="408" align="center" border="0">
            <tr valign="top" height="70%">
				<td valign=top>
					<TABLE width="100%" height=100% class="info" cellSpacing="1" cellPadding="1" align="center" border="0">
						<TR height="20px">
							<TD class="item_editbox"><P class="boxform_item">Ricerca Campi Comuni</P></TD>
						</TR>
						<TR>                            
                            <TD align="center" width="412" valign=Top>
                                <asp:Label ID="lbl_campiComuniDoc" runat="server" CssClass="testo_grigio_scuro_grande" Text="Campi Comuni Documenti" Width="100%" style="text-align:left; border-bottom:thin grey solid;"/>
								<DIV id="DivCampiComuniDoc" style="OVERFLOW: auto; WIDTH: 98%; HEIGHT: 50%;">
                                <asp:panel id="panel_ContenutoCampiDoc" runat="server" Height="100%"></asp:panel>
                                </DIV>
                            </TD>
                        </TR>
                        <TR>
                            <TD align="center" width="412" valign=Top>
                                <asp:Label ID="lbl_campiComuniFasc" runat="server" CssClass="testo_grigio_scuro_grande" Text="Campi Comuni Fascicoli" Width="100%" style="text-align:left; border-bottom:thin grey solid;"/>
                                <DIV id="DivCampiComuniFasc" style="OVERFLOW: auto; WIDTH: 98%; HEIGHT: 50%;">
                                <asp:panel id="panel_ContenutoCampiFasc" runat="server" Height="100%"></asp:panel>
                                </DIV>
                            </TD>
                        </TR>
                    </TABLE>
                </td>
            </tr>
            <tr height="7%">
				<td>
					<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" border="0" align="center">
						<TR>
							<TD vAlign="top" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
						</TR>
						<TR>
							<TD>
                                <asp:Button ID="btn_ricerca" CssClass="pulsante69" Text="Ricerca" 
                                    runat="server" ToolTip="Esegui Ricerca Campi Comuni" 
                                    onclick="btn_ricerca_Click" />
                            </TD>
                        </TR>
                    </TABLE>
                </td>
            </tr>
        </TABLE>
    </form>
</body>
</html>
