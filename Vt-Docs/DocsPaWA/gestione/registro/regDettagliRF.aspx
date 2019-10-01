<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="regDettagliRF.aspx.cs" Inherits="DocsPAWA.gestione.registro.regDettagliRF" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
	<HEAD><title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body leftMargin="1" MS_POSITIONING="GridLayout">
		<form id="regDettagliRF" method="post" runat="server">
            <asp:ScriptManager ID="scrManager" runat="server"></asp:ScriptManager>
			<asp:UpdatePanel id="panel_Det" runat="server" Visible="False" UpdateMode="Always">
                <ContentTemplate>
				    <TABLE class="info_grigio" id="tblTx" style="BORDER-RIGHT: 1px solid; BORDER-TOP: 1px solid; BORDER-LEFT: 1px solid; BORDER-BOTTOM: 1px solid" cellSpacing="1" cellPadding="1" width="77%" align="center" border="0">
					    <TR>
						    <TD class="infoDT" id="TD1" align="middle" colSpan="2" height="20" runat="server">
							    <asp:Label id="titolo" Runat="server" CssClass="titolo_rosso">
								    Dettaglio raggruppamento selezionato</asp:Label></TD>
					    </TR>
					    <TR>
						    <TD height="15"></TD>
					    </TR>
					    <TR>
						    <TD class="titolo_scheda" width="40%"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">RF</TD>
						    <TD class="testo_grigio_scuro" vAlign="center">
							    <asp:Label id="lbl_registro" runat="server"></asp:Label></TD>
					    </TR>
					    <TR>
						    <TD height="10"></TD>
					    </TR>
					    <TR>
						    <TD class="titolo_scheda" width="40%"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Descrizione</TD>
						    <TD class="testo_grigio_scuro">
							    <asp:Label id="lbl_descrizione" runat="server"></asp:Label></TD>
					    </TR>
					    <TR>
						    <TD height="10"></TD>
					    </TR>
					    <TR>
						    <TD class="titolo_scheda" width="40%"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">E-mail</TD>
						    <TD class="testo_grigio_scuro" vAlign="center">
							    <asp:DropDownList ID="ddl_Caselle" runat="server" AutoPostBack="true" DataTextField="Caselle di posta" 
                                    onselectedindexchanged="ddl_Caselle_SelectedIndexChanged"></asp:DropDownList>
                            </TD>
					    </TR>
					    <TR>
						    <TD height="10"></TD>
					    </TR>
					    <TR>
						    <TD class="titolo_scheda" width="40%"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Codice aoo &nbsp;&nbsp;&nbsp;collegata</TD>
						    <TD class="testo_grigio_scuro">
							    <asp:Label id="lbl_AooColl" runat="server"></asp:Label></TD>
					    </TR>
					    <TR>
						    <TD height="10"></TD>
					    </TR>
					    <TR>
						    <TD class="titolo_scheda" width="40%"><IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0">Descrizione aoo &nbsp;&nbsp;&nbsp;collegata</TD>
						    <TD class="testo_grigio_scuro">
							    <asp:Label id="lbl_DescAooColl" runat="server"></asp:Label></TD>
					    </TR>
					    <TR>
						    <TD height="10"></TD>
					    </TR>
				    </TABLE>
                </ContentTemplate>
            </asp:UpdatePanel>
	    </form>
	</body>
</html>
