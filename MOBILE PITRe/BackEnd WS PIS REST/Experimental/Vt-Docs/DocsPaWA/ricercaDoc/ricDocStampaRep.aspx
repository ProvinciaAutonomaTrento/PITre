<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ricDocStampaRep.aspx.cs" Inherits="DocsPAWA.ricercaDoc.ricDocStampaRep" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider" TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html>
<head runat="server">
    <title>DOCSPA > ricDocStampaRep</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		
        <script language="javascript" id="butt_ricerca_click" event="onclick()" for="butt_ricerca">
			window.document.body.style.cursor='wait';			
			WndWait();
		</script>
		
        <script language="javascript">
		    // Permette di inserire solamente caratteri numerici
		    function ValidateNumericKey() {
		        var inputKey = event.keyCode;
		        var returnCode = true;

		        if (inputKey > 47 && inputKey < 58) {
		            return;
		        }
		        else {
		            returnCode = false;
		            event.keyCode = 0;
		        }

		        event.returnValue = returnCode;
		    }
		</script>
</head>
<body leftMargin="0" MS_POSITIONING="GridLayout">
    <form id="ricDocStampaReg" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Ricerca registri di reprtorio" />
    <TABLE id="tbl_contenitore" height="100%" cellSpacing="0" cellPadding="0" width="413" align="center" border="0">
        <tr vAlign="top">
		    <td align="left">
			    <table class="contenitore" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
				    <tr>
					    <td height="15"></td>
				    </tr>
                    <tr vAlign="top">
					    <td>
                            <TABLE class="info_grigio" id="tbl_dataCreazione" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0">
							    <TR>
								    <TD class="titolo_scheda" vAlign="middle" height="19" colspan="2">
                                        <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Registri di repertorio
                                    </TD>                                    
							    </TR>
                                <tr>
                                    <td colspan="2">
                                        <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                        <asp:DropDownList ID="ddl_repertori" runat="server" class="testo_grigio" 
                                            Width="95%" onselectedindexchanged="ddl_repertori_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </td>
                                </tr>
                                <TR>
									<TD class="titolo_scheda" vAlign="middle" height="19" colspan="2">
                                        <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">RF / AOO
                                    </TD>									
								</TR>     
                                <tr>
                                    <td colspan="2">
                                        <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                        <asp:DropDownList ID="ddl_aoo_rf" runat="server" class="testo_grigio" Width="95%"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                    </td>
                                </tr>                           
							</TABLE>
							<br>
                            <table class="info_grigio" id="tblNumRepertorio" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0" runat="server">
							    <TR height="30">
                                    <TD class="titolo_scheda" width="15%" style="padding-left:5px;"><asp:label id="lblNumRepertorio" runat="server">Num. rep.</asp:label></TD>
								    <TD class="titolo_scheda" width="22%">
                                        <asp:dropdownlist id="cboFilterTypeNumRepertorio" runat="server" Width="100%" 
                                            CssClass="testo_grigio" AutoPostBack="True" 
                                            onselectedindexchanged="cboFilterTypeNumRepertorio_SelectedIndexChanged"></asp:dropdownlist></TD>
								    <TD class="testo_grigio" align="center" width="5%"><asp:label id="lblInitNumRepertorio" runat="server" CssClass="titolo_scheda">Da:</asp:label></TD>
								    <TD class="testo_grigio" align="left" width="15%"><asp:textbox id="txtInitNumRepertorio" runat="server" Width="98%" CssClass="testo_grigio"></asp:textbox></TD>
								    <TD class="testo_grigio" align="center" width="5%"><asp:label id="lblEndNumRepertorio" runat="server" CssClass="titolo_scheda">a:</asp:label></TD>
								    <TD class="testo_grigio" align="left" width="15%"><asp:textbox id="txtEndNumRepertorio" runat="server" Width="98%" CssClass="testo_grigio"></asp:textbox></TD>
								</TR>
                                <TR height="30">
									<TD class="titolo_scheda" width="15%" style="padding-left:5px;"><asp:label id="lblAnnoRepertorio" runat="server">Anno rep.</asp:label></TD>
									<TD class="titolo_scheda" colspan="5"><asp:textbox id="txtAnnoRepertorio" runat="server" Width="112px" CssClass="testo_grigio" MaxLength="4"></asp:textbox></TD>
								</TR>
							</table>
							<br>
                            <table class="info_grigio" id="tblStampa" height="30" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0" runat="server">
			                    <TR height="30">
                                <TD class="titolo_scheda" vAlign="middle" height="17" style="padding-left:5px;"><asp:label id="lblDataStampa" runat="server"> Data Stampa</asp:label></TD>
									<td class="titolo_scheda" vAlign="middle" height="17"><asp:label id="lblInitDataStampa" runat="server" CssClass="titolo_scheda">Da:</asp:label></td>
									<td class="titolo_scheda" vAlign="middle" height="17"><asp:label id="lblEndDataStampa" runat="server" CssClass="titolo_scheda">a:</asp:label></td>
								</TR>
								<TR height="30">
									<TD vAlign="middle" width="120" style="padding-left:5px;">
                                        <asp:dropdownlist id="cboFilterTypeDataStampa" runat="server" Width="100%" 
                                            CssClass="testo_grigio" AutoPostBack="True" 
                                            onselectedindexchanged="cboFilterTypeDataStampa_SelectedIndexChanged"></asp:dropdownlist></TD>
									<td vAlign="middle" width="100"><uc3:Calendario id="txtInitDataStampa" runat="server" Visible="true" Width="98%" CssClass="testo_grigio" /></td>
									<td valign="middle" width="100"><uc3:Calendario id="txtEndDataStampa" runat="server" Visible="false" Width="98%" CssClass="testo_grigio" /></td>
								</TR>
							</table>
                            <br>
                            <table class="info_grigio" id="Table1" height="30" cellSpacing="0" cellPadding="0" width="97%" align="center" border="0" runat="server">
                                <TR height="30">
                                    <TD class="titolo_scheda" vAlign="middle" height="17" style="padding-left:5px;">
                                        <asp:label id="lbl_tipiStampe" runat="server">Tipi stampe</asp:label>
                                    </TD>
								</TR>
                                <TR height="30">
                                    <TD vAlign="middle" width="100%" style="padding-left:5px;">
			                            <asp:RadioButtonList ID="rbl_TipiStampe" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal">
                                            <asp:ListItem Value="TUTTE" Text="Tutte" Selected="True"></asp:ListItem>                                    
                                            <asp:ListItem Value="NON_FIRMATE" Text="Non firmate"></asp:ListItem>                                    
                                            <asp:ListItem Value="FIRMATE" Text="Firmate"></asp:ListItem>                                    
                                        </asp:RadioButtonList>
                                    </TD>
                                </TR>
                            </table>
						</td>
					</tr>
				</table>				
            </td>
		</tr>
        <tr>
			<td height="10%">				
				<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" align="center" border="0">
					<TR>
						<TD vAlign="top" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
					</TR>
					<TR>
						<TD>
                            <asp:Button ID="butt_ricerca" runat="server" Text="Ricerca" 
                                CssClass="pulsante69" ToolTip="Ricerca documenti repertoriati" 
                                onclick="butt_ricerca_Click" />                            
                        </TD>
					</TR>
			    </TABLE>				
            </td>
		</tr>
    </TABLE>
    </form>
</body>
</html>
