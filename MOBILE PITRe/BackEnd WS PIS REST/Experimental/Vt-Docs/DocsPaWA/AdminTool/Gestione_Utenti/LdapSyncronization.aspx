<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LdapSyncronization.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Utenti.LdapSyncronization" validateRequest="false" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register assembly="DocsPaWebCtrlLibrary" namespace="DocsPaWebCtrlLibrary" tagprefix="uc4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
        <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
	<base target="_self"/>
	<script type="text/javascript">
	    function CloseDialog() { window.returnValue = document.getElementById('txtReturnValue').value; window.close(); }
	    
	    function btnImport_ClientClick() {
	        document.getElementById("<%=this.lblMessage.ClientID%>").innerHTML = "Sincronizzazione in corso, l'operazione potrebbe richiedere diversi secondi...";
	        document.getElementById("<%=this.btnSaveLdapConfigurations.ClientID%>").disabled = true;
	        document.getElementById("<%=this.btnCancel.ClientID%>").disabled = true;

	        var grid = document.getElementById("<%=this.grdEsitoSincronizzazione.ClientID%>");
	        if (grid != null) grid.style.visibility = "HIDDEN";
	    }

	    function ShowLdapSyncronizationHistory() {
	        var pageHeight = 430;
	        var pageWidth = 700;

	        var retValue = window.showModalDialog('LdapSyncronizationHistory.aspx', null, 'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no');

	        if (retValue != null && retValue != "") {
	            document.getElementById("txtIdHistory").value = retValue;
	            return true;
	        }
	        else
	            return false;
	    }    

	    function SetControlFocus(controlID) {
	        try {
	            var control = document.getElementById(controlID);

	            if (control != null) {
	                control.focus();
	            }
	        }
	        catch (e) {

	        }
	    }	    
	</script>
</head>
<body>
    <div id="divContainer" align = "center" runat = "server" style="overflow:auto; height:100%; width:100%" >
        <form id="form1" runat="server">
            <input id="txtReturnValue" type="hidden" runat="server" />
            <input id="txtIdHistory" type="hidden" runat="server" />
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Sincronizzazione utenti da LDAP" />    
            <table id="tblContainer" cellpadding="0" cellspacing="5" style ="width: 100%; height: 100%" align="center">
                <tr>
                    <td>
                        <table id="tableInnerContainer" cellpadding="0" cellspacing="5" class="contenitore" style ="width: 100%; height: 100%" align="center">
                            <tr>
                                <td colspan="4" style="height: 5px">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td align="left" class="titolo_pnl">
                                                <asp:Label ID="lblConfigurazioni" runat="server">Configurazioni LDAP</asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="testo_grigio_scuro" style="width: 40%">
                                    <asp:Label ID="lblServer" runat="server">
                                        Server LDAP (controller di dominio): *
                                    </asp:Label>
                                </td>
                                <td align="left" style="width: 612px; color: #666666; width: 60%">
                                    <asp:TextBox ID="txtServerLdap" runat="server" CssClass="testo" Width="300px" 
                                        MaxLength = "255" ToolTip="es. &quot;LDAP://server:389/&quot;"></asp:TextBox>
                                    &nbsp;            
                                    </td>
                            </tr>
                            <tr>
                                <td align="right" class="testo_grigio_scuro" style="width: 40%">
                                    <asp:Label ID="lblGruppo" runat="server">
                                        Gruppo LDAP: *
                                    </asp:Label>
                                </td>
                                <td align="left" style="width: 612px; color: #666666; width: 60%">
                                    <asp:TextBox ID="txtRuoloLdap" runat="server" CssClass="testo" Width="300px" 
                                        MaxLength = "255" 
                                        
                                        
                                        ToolTip="Distinguished name del gruppo LDAP cui appartengono gli utenti da sincronizzare"></asp:TextBox>
                                </td>
                            </tr>     
                            <tr>
                                <td colspan="4" style="height: 5px">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td align="left" class="titolo_pnl">
                                                <asp:Label ID="Label4" runat="server">Credenziali utente di dominio</asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>            
                            <tr>
                                <td align="right" class="testo_grigio_scuro" style="width: 40%">
                                    <asp:Label ID="lblNomeUtente" runat="server">
                                        Nome utente:</asp:Label>
                                </td>
                                <td align="left" style="width: 612px; color: #666666; width: 60%" valign="middle">
                                    <asp:TextBox ID="txtNomeUtente" runat="server" CssClass="testo" Width="150px" 
                                        ontextchanged="txtNomeUtente_TextChanged" 
                                        ToolTip="Nome utente di dominio" ></asp:TextBox>
                                </td>
                            </tr>   
                            <tr>
                                <td align="right" class="testo_grigio_scuro" style="width: 40%">
                                    <asp:Label ID="lblPassword" runat="server">
                                        Password:</asp:Label>
                                </td>
                                <td align="left" style="width: 612px; color: #666666; width: 60%" valign="middle">
                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="testo" Width="150px" TextMode="Password" ToolTip="Password utente di dominio" ></asp:TextBox>
                                </td>
                            </tr>   
                            <tr>
                                <td align="right" class="testo_grigio_scuro" style="width: 40%">
                                    <asp:Label ID="lblConfermaPassword" runat="server">
                                        Conferma password:</asp:Label>
                                </td>
                                <td align="left" style="width: 612px; color: #666666; width: 60%" valign="middle">
                                    <asp:TextBox ID="txtConfermaPassword" runat="server" CssClass="testo" Width="150px" TextMode="Password" ToolTip="Password utente di dominio"></asp:TextBox>
                                </td>
                            </tr>                           
                            <tr>
                                <td colspan="4" style="height: 5px">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td align="left" class="titolo_pnl">
                                                <asp:Label ID="lblMapping" runat="server">Mapping campi utente LDAP</asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="testo_grigio_scuro" style="width: 40%">
                                    <asp:Label ID="lblUserIdAttributeName" runat="server">
                                        User Id:
                                    </asp:Label>
                                </td>
                                <td align="left" style="width: 612px; color: #666666; width: 60%">
                                    <asp:TextBox ID="txtUserIdAttributeName" runat="server" CssClass="testo" 
                                        Width="300px" MaxLength = "255" 
                                        ToolTip="Nome attributo in LDAP corrispondente alla UserId dell'utente"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="testo_grigio_scuro" style="width: 40%">
                                    <asp:Label ID="lblMatricolaAttributeName" runat="server">
                                        Matricola:
                                    </asp:Label>
                                </td>
                                <td align="left" style="width: 612px; color: #666666; width: 60%">
                                    <asp:TextBox ID="txtMatricolaAttributeName" runat="server" CssClass="testo" 
                                        Width="300px" MaxLength = "255" 
                                        ToolTip="Nome attributo in LDAP corrispondente alla Matricola dell'utente"></asp:TextBox>
                                </td>
                            </tr>        
                            <tr>
                                <td align="right" class="testo_grigio_scuro" style="width: 40%">
                                    <asp:Label ID="lblEmailAttributeName" runat="server">
                                        Email:
                                    </asp:Label>
                                </td>
                                <td align="left" style="width: 612px; color: #666666; width: 60%">
                                    <asp:TextBox ID="txtEmailAttributeName" runat="server" CssClass="testo" Width="300px" MaxLength = "255" ToolTip = "Nome attributo in LDAP corrispondente all'Email dell'utente"></asp:TextBox>
                                </td>
                            </tr>            
                            <tr>
                                <td align="right" class="testo_grigio_scuro" style="width: 40%">
                                    <asp:Label ID="lblNomeAttributeName" runat="server">
                                        Nome:
                                    </asp:Label>
                                </td>
                                <td align="left" style="width: 612px; color: #666666; width: 60%">
                                    <asp:TextBox ID="txtNomeAttributeName" runat="server" CssClass="testo" Width="300px" MaxLength = "255" ToolTip="Nome attributo in LDAP corrispondente al Nome dell'utente"></asp:TextBox>
                                </td>
                            </tr>       
                            <tr>
                                <td align="right" class="testo_grigio_scuro" style="width: 40%">
                                    <asp:Label ID="lblCognomeAttributeName" runat="server">
                                        Cognome:
                                    </asp:Label>
                                </td>
                                <td align="left" style="width: 612px; color: #666666; width: 60%">
                                    <asp:TextBox ID="txtCognomeAttributeName" runat="server" CssClass="testo" Width="300px" MaxLength = "255" ToolTip="Nome attributo in LDAP corrispondente al Cognome dell'utente"></asp:TextBox>
                                </td>
                            </tr>           
                            <tr>
                                <td align="right" class="testo_grigio_scuro" style="width: 40%">
                                    <asp:Label ID="lblSedeAttributeName" runat="server">
                                        Sede:
                                    </asp:Label>
                                </td>
                                <td align="left" style="width: 612px; color: #666666; width: 60%">
                                    <asp:TextBox ID="txtSedeAttributeName" runat="server" CssClass="testo" Width="300px" MaxLength = "255" ToolTip="Nome attributo in LDAP corrispondente alla Sede dell'utente"></asp:TextBox>
                                </td>
                            </tr>   
                            <tr>
                                <td colspan="4" style="height: 5px">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td align="left" class="titolo_pnl">
                                                <asp:Label ID="Label1" runat="server">Esito sincronizzazione</asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" class="testo_grigio_scuro" colspan = "2">
                                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                    <br />
                                    <div style="overflow: auto; height: 200px">
                                        <asp:datagrid id="grdEsitoSincronizzazione" runat="server" 
                                            AutoGenerateColumns="False" 
                                            BorderColor="Gray"
			                                CellPadding="1"
			                                BorderWidth="1px" 
                                            Width="95%">
			                                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
			                                <PagerStyle Mode="NumericPages" CssClass="bg_grigioN" />
			                                <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
			                                <ItemStyle CssClass="bg_grigioN"></ItemStyle>
			                                <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                            <Columns>
                                                <asp:TemplateColumn HeaderText="Utente">
                                                    <HeaderStyle Width="25%" />
	                                                <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                                        Font-Strikeout="False" Font-Underline="False"></ItemStyle>
	                                                <ItemTemplate>
	                                                    <asp:Label ID="lblCellUtente" runat="server" Text='<%# this.GetUtenteCellText((DocsPAWA.DocsPaWR.LdapSyncronizationResponseItem) Container.DataItem) %>' />
	                                                </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:BoundColumn DataField="Date" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" 
                                                    HeaderText="Data">
                                                    <HeaderStyle Width="25%" />
                                                </asp:BoundColumn>
                                                <asp:TemplateColumn HeaderText="Esito">
                                                    <HeaderStyle Width="50%"></HeaderStyle>
	                                                <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                                        Font-Strikeout="False" Font-Underline="False"></ItemStyle>
	                                                <ItemTemplate>
	                                                    <asp:Label ID="lblCellTipo" runat="server" Text='<%# this.GetEsitoSincronizzazioneCellText((DocsPAWA.DocsPaWR.LdapSyncronizationResponseItem) Container.DataItem) %>' />
	                                                </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                         </asp:datagrid>
                                     </div>
                                </td>
                            </tr>  
                            <tr>
                                <td colspan="4" style="height: 5px">
                                    <br />
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td align="center" class="titolo_pnl">
                                                <asp:Button id="btnSaveLdapConfigurations" runat="server" Text="Salva" 
                                                    CssClass="testo_btn" Width="110px" onclick="btnSaveLdapConfigurations_Click"></asp:Button>&nbsp;
                                                <asp:Button id="btnImport" runat="server" Text="Sincronizza" CssClass="testo_btn" 
                                                    Width="110px" OnClientClick="btnImport_ClientClick()" onclick="btnImport_Click" ></asp:Button>&nbsp;
                                                <asp:Button id="btnHistory" runat="server" Text="Log..." CssClass="testo_btn" 
                                                    Width="110px" onclick="btnHistory_Click" OnClientClick="return ShowLdapSyncronizationHistory()"></asp:Button>&nbsp;                                                    
                                                <asp:Button id="btnCancel" runat="server" Text="Chiudi" CssClass="testo_btn" 
                                                    Width="110px" OnClientClick="CloseDialog()"></asp:Button>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>                        
                        </table>
                    </td>
                </tr>
            </table>	
        </form>
    </div>
</body>
</html>
