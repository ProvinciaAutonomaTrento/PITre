<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LdapSyncronizationHistory.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Utenti.LdapSyncronizationHistory" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register assembly="DocsPaWebCtrlLibrary" namespace="DocsPaWebCtrlLibrary" tagprefix="uc4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
	<base target="_self"/>
    <title runat = "server"></title>
    <script type="text/javascript">
        function CloseDialog() { window.returnValue = document.getElementById('txtReturnValue').value; window.close(); }
        function ConfirmDelete() { return confirm("Rimuovere l'elemento selezionato?"); }	    
    </script>
</head>
<body>
    <form id="frmLdapSyncronizationHistory" runat="server">
        <div id="divContainer" align = "center" runat = "server" style="overflow:auto; height:100%; width:100%" >
            <input id="txtReturnValue" type="hidden" runat="server" />
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Sincronizzazione utenti da LDAP" />    
            <table id="tblContainer" cellpadding="0" cellspacing="5" style ="width: 100%; height: 100%" align="center">
                <tr>
                    <td>
                        <table id="tableInnerContainer" cellpadding="0" cellspacing="5" class="contenitore" style ="width: 100%; height: 100%" align="center">
                            <tr>
                                <td colspan="2" style="height: 5px">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td align="left" class="titolo_pnl">
                                                <asp:Label ID="lblSincronizzazioniLdap" runat="server">Sicronizzazioni LDAP</asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>   
                            <tr>
                                <td align="center" class="testo_grigio_scuro" colspan = "2">
                                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                    <br />
                                    <div style="overflow: auto; height: 300px">
                                        <asp:datagrid id="grdHistory" runat="server" 
                                            AutoGenerateColumns="False" 
                                            BorderColor="Gray"
			                                CellPadding="1"
			                                BorderWidth="1px" 
                                            Width="95%" onitemcommand="grdHistory_ItemCommand">
			                                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
			                                <PagerStyle Mode="NumericPages" CssClass="bg_grigioN" />
			                                <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
			                                <ItemStyle CssClass="bg_grigioN"></ItemStyle>
			                                <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                            <Columns>
                                                <asp:BoundColumn DataField="Id" HeaderText="Id" Visible="False">
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="User" HeaderText="Utente">
                                                    <HeaderStyle Width="40%" />
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="Date" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" 
                                                    HeaderText="Data">
                                                    <HeaderStyle Width="20%" />
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="ItemsSyncronized" HeaderText="Utenti sincr.">
                                                    <HeaderStyle Width="15%" />
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="ErrorDetails" HeaderText="Errore">
                                                    <HeaderStyle Width="20%" />
                                                </asp:BoundColumn>
                                                <asp:TemplateColumn>
                                                    <ItemTemplate>
                                                        <uc4:ImageButton ID="btnEditItem" runat="server" CommandName="SELECT_ITEM" 
                                                            Height="20px" ImageUrl="../../images/proto/dett_lente.gif" ToolTip="Visualizza dettagli" 
                                                            Width="20px" />
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="5%" />
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn>
                                                    <ItemTemplate>
                                                        <uc4:ImageButton ID="btnDeleteItem" runat="server" CommandName="DELETE_ITEM" 
                                                            Height="20px" ImageUrl="../Images/cestino.gif" ToolTip="Cancella" 
                                                            Width="20px" OnClientClick = "return ConfirmDelete()" />
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="5%" />
                                                </asp:TemplateColumn>
                                            </Columns>
                                         </asp:datagrid>
                                     </div>
                                </td>
                            </tr>  
                            <tr>
                                <td colspan="2" style="height: 5px">
                                    <br />
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td align="center" class="titolo_pnl">
                                                &nbsp;<asp:Button id="btnCancel" runat="server" Text="Chiudi" CssClass="testo_btn" 
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
        </div>
    </form>
</body>
</html>
