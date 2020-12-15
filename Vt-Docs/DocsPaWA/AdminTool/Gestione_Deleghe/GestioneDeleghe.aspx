<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="GestioneDeleghe.aspx.cs"
    Inherits="DocsPAWA.AdminTool.Gestione_Deleghe.GestioneDeleghe" %>

<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="../../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">

    <script language="JavaScript" src="../CSS/caricamenujs.js"></script>

    <script language="javascript">
		function ApriRisultRic(idAmm) 
			{																								
				if(document.deleghe.txt_ricCod.value.length > 0 || document.deleghe.txt_ricDesc.value.length > 0)
				{
				    var myUrl = "../Gestione_Organigramma/RisultatoRicercaOrg.aspx?idAmm="+idAmm+"&tipo="+document.deleghe.ddl_ricTipo.value+"&cod="+document.deleghe.txt_ricCod.value+"&desc="+document.deleghe.txt_ricDesc.value;
					rtnValue = window.showModalDialog(myUrl,"","dialogWidth:750px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
					deleghe.hd_returnValueModal.value = rtnValue;
				}
			}
    </script>

</head>
<body bottommargin="0" leftmargin="0" topmargin="0">
    <form id="deleghe" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Gestione Sostituzioni" />
    <!-- Gestione del menu a tendina -->
    <uc2:MenuTendina ID="MenuTendina" runat="server"></uc2:MenuTendina>
    <input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
    <input id="hd_lastReturnValueModal" type="hidden" name="hd_lastReturnValueModal"
        runat="server">
    <table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
        <tr>
            <td>
                <!-- TESTATA CON MENU' -->
                <uc1:Testata ID="Testata" runat="server"></uc1:Testata>
            </td>
        </tr>
        <tr>
            <td class="testo_grigio_scuro" bgcolor="#c0c0c0" height="20">
                <asp:Label ID="lbl_position" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <!-- TITOLO PAGINA -->
            <td class="titolo" style="height: 20px" align="center" bgcolor="#e0e0e0" height="34">
                Gestione Sostituzioni
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
                <table cellspacing="0" cellpadding="0" align="center" border="0">
                    <tr>
                        <td align="center" height="25">
                            <asp:Label ID="lbl_avviso" runat="server" CssClass="testo_rosso"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td height="5px">
                        </td>
                    </tr>
                    <tr>
                        <td class="testo_grigio_scuro" align="left">
                            Stato sostituzioni:&nbsp;
                            <asp:DropDownList AutoPostBack="true" CssClass="testo_piccolo" ID="ddl_stato" runat="server">
                                <asp:ListItem Value="A" Text="Attive" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="I" Text="Impostate"></asp:ListItem>
                                <asp:ListItem Value="S" Text="Scadute"></asp:ListItem>
                                <asp:ListItem Value="T" Text="Tutte"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td height="5px">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" CssClass="testo_red" Text="" Visible="false"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td height="5px">
                        </td>
                    </tr>
                    <tr>
                        <td class="pulsanti" align="center">
                            <table width="900">
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lbl_titolo" runat="server" CssClass="titolo">Lista Sostituzioni</asp:Label>
                                    </td>
                                    <td align="right">
                                        &nbsp;
                                        <asp:Button ID="btn_nuova" CssClass="testo_btn_p" runat="server" Text="Nuova" ToolTip="Nuova delega" />&nbsp;
                                        <asp:Button ID="btn_revoca" CssClass="testo_btn_p" runat="server" Text="Revoca" ToolTip="Revoca delega" />&nbsp;
                                        <asp:Button ID="btn_modifica" CssClass="testo_btn_p" runat="server" Text="Modifica"
                                            ToolTip="Modifica delega" />&nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lbl_messaggio" runat="server" CssClass="testo_red" Text="" Visible="false"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" height="5">
                            <br>
                            <div id="DivDGList" style="overflow: auto; height: 160px">
                                <asp:DataGrid ID="dgDeleghe" runat="server" Visible="False" AutoGenerateColumns="False"
                                    CellPadding="1" Width="100%" BorderWidth="1px" BorderColor="Gray">
                                    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                    <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                    <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                    <Columns>
                                        <asp:TemplateColumn HeaderText="Sel" ItemStyle-Width="5%">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cbSel" runat="server" AutoPostBack="true" OnCheckedChanged="cbSel_CheckedChanged" />
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:BoundColumn DataField="dataDecorrenza" HeaderText="decorrenza" ItemStyle-Width="10%"
                                            ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundColumn DataField="dataScadenza" HeaderText="scadenza" ItemStyle-Width="10%"
                                            ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundColumn DataField="id_utente_delegato" HeaderText="Utente" Visible="false"
                                            ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:BoundColumn DataField="cod_utente_delegato" HeaderText="Sostituto" ItemStyle-Width="40%"
                                            ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:BoundColumn DataField="id_ruolo_delegato" HeaderText="Ruolo" Visible="false"
                                            ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:BoundColumn DataField="cod_ruolo_delegato" HeaderText="Ruolo sostituto" Visible="false"
                                            ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:BoundColumn DataField="id_utente_delegante" HeaderText="Utente" Visible="false"
                                            ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:BoundColumn DataField="cod_utente_delegante" HeaderText="Titolare" ItemStyle-Width="40%"
                                            ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:BoundColumn DataField="id_ruolo_delegante" HeaderText="Ruolo" Visible="false"
                                            ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:BoundColumn DataField="cod_ruolo_delegante" HeaderText="Ruolo titolare" Visible="false"
                                            ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:BoundColumn DataField="id_delega" Visible="false" ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:BoundColumn DataField="id_uo_delegato" Visible="false" ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:BoundColumn DataField="inEsercizio" Visible="false" ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:BoundColumn DataField="id_people_corr_globali" Visible="false" ReadOnly="true"
                                            HeaderStyle-HorizontalAlign="Left" />
                                        <asp:TemplateColumn HeaderText="Stato" ItemStyle-Width="5%">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:Label ID="stato" runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                    </Columns>
                                </asp:DataGrid>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td height="15px">
                        </td>
                    </tr>
                    <asp:Panel ID="pnl_nuovaDelega" runat="server" Visible="false">
                        <tr>
                            <td class="pulsanti">
                                <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbl_operazione" runat="server" CssClass="testo_grigio_scuro">NUOVA DELEGA</asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:ImageButton ID="btn_chiudiPnl" runat="server" ToolTip="Chiudi"
                                                ImageUrl="../Images/cancella.gif"></asp:ImageButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <table cellpadding="0" cellspacing="0" class="info" width="100%">
                                    <tr height="2px">
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <table cellpadding="2" cellspacing="0" border="0">
                                                <tr>
                                                    <td class="testo_piccoloB" width="134px">
                                                        Seleziona utente:
                                                    </td>
                                                    <td class="testo_piccoloB">
                                                        Ricerca tra:
                                                    </td>
                                                    <td class="testo_piccoloB">
                                                        Codice:
                                                    </td>
                                                    <td class="testo_piccoloB" id="td_descRicerca" runat="server">
                                                        Nome UO:
                                                    </td>
                                                    <td class="testo_piccoloB">
                                                    </td>
                                                </tr>
                                                <tr height="10px">
                                                    <td class="testo_piccoloB" width="134px">
                                                        <asp:DropDownList ID="chklst_utente" runat="server" Width="100" CssClass="testo_grigio_scuro"
                                                            AutoPostBack="True">
                                                            <asp:ListItem Text="titolare"></asp:ListItem>
                                                            <asp:ListItem Text="sostituto"></asp:ListItem>
                                                        </asp:DropDownList>
                                                        &nbsp;
                                                    </td>
                                                    <td class="testo_piccoloB">
                                                        <asp:DropDownList ID="ddl_ricTipo" runat="server" CssClass="testo_grigio_scuro" AutoPostBack="True">
                                                            <asp:ListItem Value="U" Selected="True">Unità Organizz.</asp:ListItem>
                                                            <asp:ListItem Value="R">Ruolo</asp:ListItem>
                                                            <asp:ListItem Value="PN">Nome</asp:ListItem>
                                                            <asp:ListItem Value="PC">Cognome</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td class="testo_piccoloB">
                                                        <asp:TextBox ID="txt_ricCod" TabIndex="1" runat="server" CssClass="testo_grigio_scuro" Width="80"></asp:TextBox>
                                                    </td>
                                                    <td class="testo_piccoloB">
                                                        <asp:TextBox ID="txt_ricDesc" TabIndex="2" runat="server" CssClass="testo_grigio_scuro" Width="210"></asp:TextBox>
                                                    </td>
                                                    <td class="testo_piccoloB">
                                                        <asp:Button ID="btn_find" TabIndex="3" runat="server" CssClass="testo_btn" Text="Cerca">
                                                        </asp:Button>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top" align="right">
                                        </td>
                                        <td>
                                            <iewc:TreeView ID="treeViewUO" runat="server" AutoPostBack="True" Height="200px"
                                                Width="600px" BackColor="antiquewhite" BorderWidth="1px" BorderStyle="solid"
                                                font="verdana" DefaultStyle="font-weight:normal;font-size:10px;color:black;text-indent:0px;font-family:Verdana;"></iewc:TreeView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td height="3px" colspan="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Label ID="lbl_delegante" runat="server" Width="120px" CssClass="testo_grigio_scuro">Titolare &nbsp;</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_delegante" Enabled="false" runat="server" Width="160px" CssClass="testo_piccolo"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:CheckBox ID="chkTutti" TextAlign="left" runat="server" Text="Tutti i ruoli"
                                                AutoPostBack="true" CssClass="testo_grigio_scuro" />&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Label ID="lbl_ruolo_delegante" runat="server" CssClass="testo_grigio_scuro">Ruolo titolare</asp:Label>&nbsp;
                                            <asp:DropDownList ID="chklst_ruoli" runat="server" Width="200px" CssClass="testo_piccolo"
                                                AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td height="3px" colspan="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Label ID="lbl_delegato" Width="120px" runat="server" CssClass="testo_grigio_scuro">Sostituto &nbsp;</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_delegato" Enabled="false" runat="server" Width="160px" CssClass="testo_piccolo"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td height="3px" colspan="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Label ID="lbl_dta_decorrenza" runat="server" Width="120px" CssClass="testo_grigio_scuro">Data decorrenza  &nbsp;</asp:Label>
                                        </td>
                                        <td>
                                            <uc3:Calendario ID="txt_dta_decorrenza" runat="server" Visible="true" CSS="testo_piccolo" />
                                            &nbsp;&nbsp;&nbsp;
                                            <asp:Label ID="lbl_ora_decorrenza" runat="server" Width="30px" CssClass="testo_grigio_scuro">Ora</asp:Label>
                                            <asp:DropDownList ID="ddl_ora_decorrenza" CssClass="testo_piccolo" runat="server"
                                                Visible="true">
                                                <asp:ListItem Value="00" Text="00" />
                                                <asp:ListItem Value="01" Text="01" />
                                                <asp:ListItem Value="02" Text="02" />
                                                <asp:ListItem Value="03" Text="03" />
                                                <asp:ListItem Value="04" Text="04" />
                                                <asp:ListItem Value="05" Text="05" />
                                                <asp:ListItem Value="06" Text="06" />
                                                <asp:ListItem Value="07" Text="07" />
                                                <asp:ListItem Value="08" Text="08" />
                                                <asp:ListItem Value="09" Text="09" />
                                                <asp:ListItem Value="10" Text="10" />
                                                <asp:ListItem Value="11" Text="11" />
                                                <asp:ListItem Value="12" Text="12" />
                                                <asp:ListItem Value="13" Text="13" />
                                                <asp:ListItem Value="14" Text="14" />
                                                <asp:ListItem Value="15" Text="15" />
                                                <asp:ListItem Value="16" Text="16" />
                                                <asp:ListItem Value="17" Text="17" />
                                                <asp:ListItem Value="18" Text="18" />
                                                <asp:ListItem Value="19" Text="19" />
                                                <asp:ListItem Value="20" Text="20" />
                                                <asp:ListItem Value="21" Text="21" />
                                                <asp:ListItem Value="22" Text="22" />
                                                <asp:ListItem Value="23" Text="23" />
                                            </asp:DropDownList>
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Label ID="lbl_dta_scadenza" runat="server" CssClass="testo_grigio_scuro">Data scadenza</asp:Label>
                                            &nbsp;
                                            <uc3:Calendario ID="txt_dta_scadenza" runat="server" Visible="true" CSS="testo_piccolo" />
                                            &nbsp;&nbsp;&nbsp;
                                            <asp:Label ID="lbl_ora_scadenza" runat="server" Width="30px" CssClass="testo_grigio_scuro">Ora</asp:Label>
                                            <asp:DropDownList ID="ddl_ora_scadenza" CssClass="testo_piccolo" runat="server" Visible="true">
                                                <asp:ListItem Value="" Text="" Selected="True" />
                                                <asp:ListItem Value="00" Text="00" />
                                                <asp:ListItem Value="01" Text="01" />
                                                <asp:ListItem Value="02" Text="02" />
                                                <asp:ListItem Value="03" Text="03" />
                                                <asp:ListItem Value="04" Text="04" />
                                                <asp:ListItem Value="05" Text="05" />
                                                <asp:ListItem Value="06" Text="06" />
                                                <asp:ListItem Value="07" Text="07" />
                                                <asp:ListItem Value="08" Text="08" />
                                                <asp:ListItem Value="09" Text="09" />
                                                <asp:ListItem Value="10" Text="10" />
                                                <asp:ListItem Value="11" Text="11" />
                                                <asp:ListItem Value="12" Text="12" />
                                                <asp:ListItem Value="13" Text="13" />
                                                <asp:ListItem Value="14" Text="14" />
                                                <asp:ListItem Value="15" Text="15" />
                                                <asp:ListItem Value="16" Text="16" />
                                                <asp:ListItem Value="17" Text="17" />
                                                <asp:ListItem Value="18" Text="18" />
                                                <asp:ListItem Value="19" Text="19" />
                                                <asp:ListItem Value="20" Text="20" />
                                                <asp:ListItem Value="21" Text="21" />
                                                <asp:ListItem Value="22" Text="22" />
                                                <asp:ListItem Value="23" Text="23" />
                                            </asp:DropDownList>
                                            &nbsp;
                                            <asp:Button ID="btn_conferma" CssClass="testo_btn_p" runat="server" Text="Conferma"
                                                ToolTip="Conferma nuova delega" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:Panel>
                    <tr>
                        <td height="5px">
                        </td>
                    </tr>
                    <tr>
                        <td width="1" height="1">
                            <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="msg_revoca" runat="server">
                            </cc2:MessageBox>
                            <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="msg_chiudi" runat="server">
                            </cc2:MessageBox>
                            <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="msg_DelegaPermanente"
                                runat="server"></cc2:MessageBox>
                            <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="msg_conferma" runat="server">
                            </cc2:MessageBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
