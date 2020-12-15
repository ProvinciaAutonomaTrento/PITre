<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" AutoEventWireup="true" Codebehind="regRepertorioDettaglio.aspx.cs" Inherits="DocsPAWA.gestione.registro.regRepertorioDettaglio" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
    <HEAD id="HEAD1" runat="server"><meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
<meta content="C#" name="CODE_LANGUAGE"><meta content="JavaScript" name="vs_defaultClientScript">
<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"><script language="javascript" src="../../LIBRERIE/DocsPA_Func.js">
</script><LINK href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">
</HEAD>
    <body leftMargin="1" MS_POSITIONING="GridLayout">
        <form id="regDettagli" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Dettaglio registro selezionato" />
			<asp:panel id="panel_Det" runat="server">
				<TABLE class="info_grigio" id="tblTx" style="BORDER-RIGHT: 1px solid; BORDER-TOP: 1px solid; BORDER-LEFT: 1px solid; BORDER-BOTTOM: 1px solid" cellSpacing="1" cellPadding="1" width="77%" align="center" border="0">
					<TR>
						<TD class="infoDT" id="TD1" align="middle" colSpan="2" height="20" runat="server">
							<asp:Label id="titolo" Runat="server" CssClass="titolo_rosso" Font-Size="11px" 
                                ForeColor="#23415F">
								Dettaglio registro di repertorio</asp:Label>
                        </TD>
					</TR>
					<TR>
						<TD height="15"></TD>
					</TR>
					<TR>
						<TD class="titolo_Scheda" width="50%" align="left">
                            Responsabile del registro di repertorio</TD>
						<TD class="testo_grigio_scuro" vAlign="center" width="50%" align="left">
							<asp:Label id="lbl_Responsabile" runat="server" Font-Size="11px"></asp:Label>
                        </TD>
					</TR>
					<TR>
						<TD height="10"></TD>
					</TR>
					<TR>
						<TD class="titolo_Scheda" width="50%" align="left">Data ultima stampa</TD>
						<TD class="testo_grigio_scuro" width="50%" align="left">
							<asp:Label id="lbl_dtaLastPrint" runat="server" Font-Size="11px"></asp:Label>
                        </TD>
					</TR>
				</TABLE>
			</asp:panel>
            </form>
    </body>
</HTML>
