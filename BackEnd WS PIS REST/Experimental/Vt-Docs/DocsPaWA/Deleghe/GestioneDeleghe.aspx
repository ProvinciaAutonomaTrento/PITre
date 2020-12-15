<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneDeleghe.aspx.cs" Inherits="DocsPAWA.Deleghe.GestioneDeleghe" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary"%>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3"%>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider" TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head id="HEAD1" runat="server">
    <title>DOCSPA > Gestione Deleghe</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <meta http-equiv="Pragma" content="no-cache">
    <meta http-equiv="Expires" content="-1">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <base target="_self" />

    <script language="javascript" src="../LIBRERIE/rubrica.js"></script>

    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

    <script language="javascript">
		function VisualizzaAttendi()
			{				
				window.document.body.style.cursor='wait';
				
				var w_width = 200;
				var w_height = 50;	
				var t = (screen.availHeight - w_height) / 2;
				var l = (screen.availWidth - w_width) / 2;	
				if(document.getElementById ("WAIT"))
				{		
					document.getElementById ("WAIT").style.top = t;
					document.getElementById ("WAIT").style.left = l;
					document.getElementById ("WAIT").style.width = w_width;
					document.getElementById ("WAIT").style.height = w_height;				
					document.getElementById ("WAIT").style.visibility = "visible";
				}				
			}
			
			function ApriRisultRic(idAmm) {
			    var ddl_ricTipo = document.getElementById("ddl_ricTipo");
			    var txt_ricCod = document.getElementById("txt_ricCod");
			    var txt_ricDesc = document.getElementById("txt_ricDesc");
			    if (txt_ricCod.value.length > 0 || txt_ricDesc.value.length > 0)
				{
					var myUrl = "../organigramma/RisultatoRicercaOrg.aspx?idAmm="+idAmm+"&tipo="+ddl_ricTipo.value+"&cod="+txt_ricCod.value+"&desc="+txt_ricDesc.value;
					rtnValue = window.showModalDialog(myUrl,"","dialogWidth:750px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;");
					document.getElementById("hd_returnValueModal").value = rtnValue;
				}
            }

            function ApriRisultRicMD(idAmm) {
                var ddl_ricTipo_modelloDelega = document.getElementById("ddl_ricTipo_modelloDelega");
                var txt_ricCod_modelloDelega = document.getElementById("txt_ricCod_modelloDelega");
                var txt_ricDesc_modelloDelega = document.getElementById("txt_ricDesc_modelloDelega");
                if (txt_ricCod_modelloDelega.value.length > 0 || txt_ricDesc_modelloDelega.value.length > 0) {
                    var myUrl = "../organigramma/RisultatoRicercaOrg.aspx?idAmm=" + idAmm + "&tipo=" + ddl_ricTipo_modelloDelega.value + "&cod=" + txt_ricCod_modelloDelega.value + "&desc=" + txt_ricDesc_modelloDelega.value;
                    rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:750px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;");
                    document.getElementById("hd_returnValueModal_modelloDelega").value = rtnValue;
                }
            }
			
            function OpenHelp(from) {
                var pageHeight = 600;
                var pageWidth = 800;
                var posTop = (screen.availHeight - pageHeight) / 2;
                var posLeft = (screen.availWidth - pageWidth) / 2;

                var newwin = window.showModalDialog('../Help/Manuale.aspx?from=' + from,
							    '',
							    'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:yes;dialogLeft:' + posLeft + ';dialogTop:' + posTop + ';center:yes;help:no');
            }

		function SingleSelect(regex,current) 
        { 
            re = new RegExp(regex);
            for(i = 0; i < document.forms[0].elements.length;i++)
            {
                elm = document.forms[0].elements[i];
                if (elm.type == 'radio' && elm != current && re.test(elm.name))
                {
                    elm.checked = false; 
                } 
            } 
        }
        
    </script>

</head>
<body bottommargin="1" leftmargin="1" topmargin="1" rightmargin="1" ms_positioning="GridLayout">
    <form id="deleghe" method="post" runat="server">
    <input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
    <input id="hd_lastReturnValueModal" type="hidden" name="hd_lastReturnValueModal"
        runat="server">
    <input id="hd_returnValueModal_modelloDelega" type="hidden" name="hd_returnValueModal_modelloDelega" runat="server"/>
    <uct:AppTitleProvider ID="appTitleProvider1" runat="server" PageName="Gestione deleghe" />
    <table class="info" width="95%" align="center" border="0">
        <tr>
            <td class="item_editbox">
                <table><tr><td>
                <p class="boxform_item">
                    <asp:Label ID="lbl_titolo" runat="server">Gestione deleghe</asp:Label>
                </p>
                </td>
                <td align="right"><asp:ImageButton id="help" runat="server" OnClientClick="OpenHelp('GestioneDeleghe')" AlternateText="Aiuto?" SkinID="btnHelp" /></td>
                </tr></table>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:RadioButtonList ID="rb_opzioni" runat="server" Width="608px" CssClass="testo_grigio"
                    AutoPostBack="True" RepeatDirection="Horizontal">
                    <asp:ListItem Value="ricevute">Deleghe ricevute</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <asp:MultiView ID="mv_deleghe" runat="server">
        <asp:View ID="deleghe_view" runat="server">
        <tr>
            <td height="5px">
            </td>
        </tr>
        <tr>
          <td>
           <!--FORM DI RICERCA DELLE DELEGHE-->
           <table cellpadding="5" cellspacing="0" border="0">
                <tr>
                    <td class="testo_grigio_scuro" align="left">
                        <asp:Label ID="lbl_stato" runat="server">Stato deleghe: &nbsp;</asp:Label>
                        <asp:DropDownList CssClass="testo_grigio" ID="ddl_stato" runat="server">
                            <asp:ListItem Value="A" Text="Attive" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="I" Text="Impostate"></asp:ListItem>
                            <asp:ListItem Value="S" Text="Scadute"></asp:ListItem>
                            <asp:ListItem Value="T" Text="Tutte"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td ID="td_nome_delegato" class="testo_grigio_scuro" align="left" runat="server" visible="false">
                        <asp:Label ID="lbl_nome_delegato" runat="server">Nome delegato: &nbsp;</asp:Label>
                        <asp:TextBox ID="txt_nome_delegato" runat="server" CssClass="testo_grigio" />
                    </td>
                    <td ID="td_nome_delegante" class="testo_grigio_scuro" align="left" runat="server" visible="false">
                        <asp:Label ID="lbl_nome_delegante" runat="server">Nome delegante: &nbsp;</asp:Label>
                        <asp:TextBox ID="txt_nome_delegante" runat="server" CssClass="testo_grigio" />
                    </td>
                    <td ID="td_ruolo_delegante" class="testo_grigio_scuro" align="left" runat="server" visible="false">
                        <asp:Label ID="lbl_ruolo_delegante" runat="server">Ruolo delegante: &nbsp;</asp:Label>
                        <asp:DropDownList ID="ddl_ruolo_delegante" runat="server" CssClass="testo_grigio"
                                    AutoPostBack="True" Width="300">
                        </asp:DropDownList>
                    </td>
                    <td align="center">
                        <asp:Button ID="btn_cerca_deleghe" runat="server" ToolTip="Cerca" Text="Cerca" CssClass="pulsante_hand"></asp:Button>
                    </td>
                </tr>
           </table>
           <!--FINE FORM DI RICERCA DELLE DELEGHE-->
          </td>
        </tr>
        <tr>
            <td height="5px">
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lbl_messaggio" runat="server" CssClass="testo_red" Text="" Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <div id="DivDGList" style="overflow: auto; height: 200px">
                    <asp:DataGrid ID="dgDeleghe" runat="server" SkinID="datagrid" BorderColor="Gray"
                        AutoGenerateColumns="false" PageSize="10" BorderWidth="1px" CellPadding="1" OnPageIndexChanged="dgDeleghe_PageIndexChanged" AllowCustomPaging="true"
                        AllowPaging="true" Width="100%">
                        <SelectedItemStyle CssClass="bg_grigioSP" />
                        <AlternatingItemStyle CssClass="bg_grigioAP" />
                        <ItemStyle CssClass="bg_grigioNP" />
                        <PagerStyle VerticalAlign="Middle" Position="TopAndBottom" HorizontalAlign="Center"
                            BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages" />
                        <HeaderStyle ForeColor="White" CssClass="menu_1_bianco_dg" />
                        <Columns>
                            <asp:TemplateColumn HeaderText="Sel" ItemStyle-Width="5%">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbSel" runat="server" AutoPostBack="true" OnCheckedChanged="cbSel_CheckedChanged" />
                                    <asp:RadioButton ID="rbSel" runat="server" AutoPostBack="true" Visible="false" onclick="SingleSelect('rbS',this);"
                                        OnCheckedChanged="rbSel_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="dataDecorrenza" HeaderText="decorrenza" ItemStyle-Width="5%"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundColumn DataField="dataScadenza" HeaderText="scadenza" ItemStyle-Width="5%"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundColumn DataField="id_utente_delegato" HeaderText="Utente" Visible="false"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundColumn DataField="cod_utente_delegato" HeaderText="Delegato" ItemStyle-Width="20%"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundColumn DataField="id_ruolo_delegato" HeaderText="Ruolo" Visible="false"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundColumn DataField="cod_ruolo_delegato" HeaderText="Ruolo" ItemStyle-Width="20%"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundColumn DataField="id_utente_delegante" HeaderText="Utente" Visible="false"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundColumn DataField="cod_utente_delegante" HeaderText="Delegante" ItemStyle-Width="20%"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundColumn DataField="id_ruolo_delegante" HeaderText="Ruolo" Visible="false"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundColumn DataField="cod_ruolo_delegante" HeaderText="Ruolo delegante" ItemStyle-Width="20%"
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
                                <asp:ImageButton ID="btn_chiudiPnl" TabIndex="7" runat="server" ToolTip="Chiudi"
                                    ImageUrl="../Images/cancella.gif"></asp:ImageButton>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <table border="0" cellpadding="0" cellspacing="0" class="info" width="100%">
                        <tr ID="tr_ddl_modelloDelega" runat="server" visible="false">
                           <td align="right">
                                <asp:Label ID="lbl_ddl_modelloDelega" runat="server" Width="160px" CssClass="testo_grigio_scuro">Seleziona modello delega &nbsp;</asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddl_modelloDelega" runat="server" Width="600px" CssClass="testo_grigio"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                        </tr> 
                        <tr>
                            <td align="right">
                                <asp:Label ID="lbl_ruolo" runat="server" Width="160px" CssClass="testo_grigio_scuro">Seleziona ruolo delegante &nbsp;</asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="chklst_ruoli" runat="server" Width="600px" CssClass="testo_grigio"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td height="10px" colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <table cellpadding="2" cellspacing="0" border="0">
                                    <tr>
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
                                    <tr>
                                        <td class="testo_piccoloB">
                                            <asp:DropDownList ID="ddl_ricTipo" runat="server" CssClass="testo_grigio_scuro" AutoPostBack="True">
                                                <asp:ListItem Value="U" Selected="True">Unità Organizz.</asp:ListItem>
                                                <asp:ListItem Value="R">Ruolo</asp:ListItem>
                                                <asp:ListItem Value="PN">Nome</asp:ListItem>
                                                <asp:ListItem Value="PC">Cognome</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="testo_piccoloB">
                                            <asp:TextBox ID="txt_ricCod" TabIndex="1" runat="server" CssClass="testo_grigio_scuro"
                                                Width="80"></asp:TextBox>
                                        </td>
                                        <td class="testo_piccoloB">
                                            <asp:TextBox ID="txt_ricDesc" TabIndex="2" runat="server" CssClass="testo_grigio_scuro"
                                                Width="210"></asp:TextBox>
                                        </td>
                                        <td class="testo_piccoloB">
                                            <asp:ImageButton ID="btn_find" runat="server" ToolTip="Cerca in organigramma" SkinID="btnFind">
                                            </asp:ImageButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lbl_avviso" CssClass="testo_rosso" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" align="right">
                                <asp:Label ID="lbl_delegato" runat="server" Width="160px" CssClass="testo_grigio_scuro">Seleziona delegato &nbsp;</asp:Label>
                            </td>
                            <td>
                                <iewc:TreeView ID="treeViewUO" runat="server" AutoPostBack="True" Height="240px"
                                    Width="600px" BackColor="antiquewhite" BorderWidth="1px" BorderStyle="solid"
                                    font="verdana" DefaultStyle="font-weight:normal;font-size:10px;color:black;text-indent:0px;font-family:Verdana;"></iewc:TreeView>
                            </td>
                        </tr>
                        <tr>
                            <td height="10px" colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="lbl_dta_decorrenza" runat="server" Width="160px" CssClass="testo_grigio_scuro">Data decorrenza  &nbsp;</asp:Label>
                            </td>
                            <td>
                                <uc3:Calendario ID="txt_dta_decorrenza" runat="server" Visible="true" />
                                &nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lbl_ora_decorrenza" runat="server" Width="30px" CssClass="testo_grigio_scuro">Ora</asp:Label>
                                <asp:DropDownList ID="ddl_ora_decorrenza" CssClass="testo_grigio" runat="server"
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
                                <uc3:Calendario ID="txt_dta_scadenza" runat="server" Visible="true" />
                                &nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lbl_ora_scadenza" runat="server" Width="30px" CssClass="testo_grigio_scuro">Ora</asp:Label>
                                <asp:DropDownList ID="ddl_ora_scadenza" CssClass="testo_grigio" runat="server" Visible="true">
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
                                <asp:Button ID="btn_conferma" CssClass="pulsante92" runat="server" Text="Conferma"
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
        <tr>
            <td class="titolo_scheda" colspan="2" align="center">
                <table id="tblButtonsContainer" cellspacing="0" cellpadding="2" width="100%" align="center"
                    border="0" runat="server">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btn_nuova" CssClass="pulsante69" runat="server" Text="Nuova" ToolTip="Nuova delega" />
                            <asp:Button ID="btn_revoca" CssClass="pulsante69" runat="server" Text="Revoca" ToolTip="Revoca delega" />
                            <asp:Button ID="btn_modifica" CssClass="pulsante69" runat="server" Text="Modifica" ToolTip="Modifica delega" />
                            <asp:Button ID="btn_esercita" CssClass="pulsante69" runat="server" Text="Esercita" ToolTip="Esercita delega" />
                            <asp:Button ID="btn_chiudi" CssClass="pulsante69" runat="server" Text="Chiudi" ToolTip="Chiudi" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        </asp:View>
        <!--MODELLI DI DELEGA-->
        <asp:View ID="modelli_deleghe_view" runat="server">
           <tr>
            <td height="5px">
            </td>
           </tr>
           <tr>
            <td>
              <!--FORM DI RICERCA MODELLI DI DELEGA-->
              <table cellpadding="5" cellspacing="0" border="0">
                <tr>
                  <td>
                     <asp:Label ID="Label1" runat="server" CssClass="testo_grigio_scuro">Stato modello: </asp:Label>
                     <asp:DropDownList ID="ric_statoMod_modelloDelega" runat="server" Width="200px" CssClass="testo_grigio"
                                    AutoPostBack="True">
                     </asp:DropDownList>
                  </td>
                  <td>
                     <asp:Label ID="lbl_ric_nomeMod_modelloDelega" runat="server" CssClass="testo_grigio_scuro">Nome modello: </asp:Label>
                     <asp:TextBox ID="ric_nomeMod_modelloDelega" CssClass="testo_grigio" runat="server"/>
                  </td>
                  <td>
                     <asp:Label ID="lbl_ric_nomeDel_modelloDelega" runat="server" CssClass="testo_grigio_scuro">Nome delegato: </asp:Label>
                     <asp:TextBox ID="ric_nomeDel_modelloRicerca" CssClass="testo_grigio" runat="server"/>
                  </td>
                </tr>
                <tr>
                  <td>
                     <asp:Label ID="lbl_ric_ruoloDel_modelloDelega" runat="server" CssClass="testo_grigio_scuro">Ruolo delegante: </asp:Label>
                     <asp:DropDownList ID="ric_ruoloDel_modelloDelega" runat="server" CssClass="testo_grigio"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                  </td>
                  <td>
                     <asp:Label ID="lbl_ric_dataInizio_modelloDelega" runat="server" CssClass="testo_grigio_scuro">Data inizio: </asp:Label>
                     <uc3:Calendario ID="ric_dataInizio_modelloDelega" runat="server" Visible="true" />
                  </td>
                  <td align="center">
                      <asp:Button ID="btn_ric_cerca_modelloDelega" runat="server" ToolTip="Cerca" Text="Cerca" CssClass="pulsante_hand"></asp:Button>
                  </td>
                </tr>
              </table>
              <!--FINE FORM DI RICERCA MODELLI DI DELEGA-->
            </td>
           </tr>
           <tr>
            <td height="5px">
            </td>
           </tr>
           <tr>
            <td>
                <asp:Label ID="lbl_noModelloDelega" runat="server" CssClass="testo_red" Text="" Visible="false">Nessun modello delega</asp:Label>
            </td>
        </tr>
           <tr>
            <td align="center">
                <div id="Div2" style="overflow: auto; height: 200px">
                    <asp:DataGrid ID="modelliDelegheGrid" runat="server" SkinID="datagrid" BorderColor="Gray"
                        AutoGenerateColumns="false" BorderWidth="1px" PageSize="10" OnPageIndexChanged = "mdGrid_PageIndexChanged" CellPadding="1" AllowCustomPaging="true"
                        AllowPaging="true" Width="100%">
                        <SelectedItemStyle CssClass="bg_grigioSP" />
                        <AlternatingItemStyle CssClass="bg_grigioAP" />
                        <ItemStyle CssClass="bg_grigioNP" />
                        <PagerStyle VerticalAlign="Middle" Position="TopAndBottom" HorizontalAlign="Center"
                            BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages" />
                        <HeaderStyle ForeColor="White" CssClass="menu_1_bianco_dg" />
                        <Columns>     
                            <asp:TemplateColumn HeaderText="Sel" ItemStyle-Width="5%">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbSelMD" runat="server" AutoPostBack="true" OnCheckedChanged="cbSelMD_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="nome" HeaderText="nome" ItemStyle-Width="10%"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundColumn DataField="stato" HeaderText="stato" ItemStyle-Width="10%"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundColumn DataField="descrUtenteDelegato" HeaderText="Utente" Visible="true" ItemStyle-Width="10%"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center"/>
                            <asp:BoundColumn DataField="descrRuoloDelegante" HeaderText="Ruolo delegante" Visible="true" ItemStyle-Width="20%"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center"/>
                            <asp:BoundColumn DataField="dataInizio" HeaderText="Data inizio" Visible="true"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center"/>
                            <asp:BoundColumn DataField="intervallo" HeaderText="Intervallo" Visible="true"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center"/>
                            <asp:BoundColumn DataField="dataFine" HeaderText="Data fine" Visible="true"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center"/>
                        </Columns>
                    </asp:DataGrid>
                </div>
            </td>
        </tr>
        <!--INSERIMENTO/MODIFICA MODELLO DI DELEGA-->
        <tr>
            <td height="15px">
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lbl_messaggio_modelloDelega" runat="server" CssClass="testo_red" Text="" Visible="false"></asp:Label>
            </td>
        </tr>
        <asp:Panel ID="pnl_nuovoModelloDelega" runat="server" Visible="false">
            <tr>
                <td class="pulsanti">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                        <tr>
                            <td>
                                <asp:Label ID="lbl_titoloPnl_ModelloDelega" runat="server" CssClass="testo_grigio_scuro"></asp:Label>
                            </td>
                            <td align="right">
                                <asp:ImageButton ID="btn_chiudiPnl_modelloDelega" TabIndex="7" runat="server" ToolTip="Chiudi"
                                    ImageUrl="../Images/cancella.gif"></asp:ImageButton>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <table border="0" cellpadding="0" cellspacing="0" class="info" width="100%">
                        <tr>
                            <td align="right">
                                <input type="hidden" ID="hd_id_modelloDelega" name="hd_id_modelloDelega" runat="server" />
                                <asp:Label ID="lbl_nome_modelloDelega" runat="server" Width="160px" CssClass="testo_grigio_scuro">Nome del modello &nbsp;</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txt_nome_modelloDelega" runat="server" CssClass="testo_grigio" Columns="40" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="Label2" runat="server" Width="160px" CssClass="testo_grigio_scuro">Seleziona ruolo delegante &nbsp;</asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="chklist_ruolo_modelloDelega" runat="server" Width="600px" CssClass="testo_grigio"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td height="10px" colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <table cellpadding="2" cellspacing="0" border="0">
                                    <tr>
                                        <td class="testo_piccoloB">
                                            Ricerca tra:
                                        </td>
                                        <td class="testo_piccoloB">
                                            Codice:
                                        </td>
                                        <td class="testo_piccoloB" id="td_descRicerca_modelloDelega" runat="server">
                                            Nome UO:
                                        </td>
                                        <td class="testo_piccoloB">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_piccoloB">
                                            <asp:DropDownList ID="ddl_ricTipo_modelloDelega" runat="server" CssClass="testo_grigio_scuro" AutoPostBack="True">
                                                <asp:ListItem Value="U" Selected="True">Unità Organizz.</asp:ListItem>
                                                <asp:ListItem Value="R">Ruolo</asp:ListItem>
                                                <asp:ListItem Value="PN">Nome</asp:ListItem>
                                                <asp:ListItem Value="PC">Cognome</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="testo_piccoloB">
                                            <asp:TextBox ID="txt_ricCod_modelloDelega" TabIndex="1" runat="server" CssClass="testo_grigio_scuro"
                                                Width="80"></asp:TextBox>
                                        </td>
                                        <td class="testo_piccoloB">
                                            <asp:TextBox ID="txt_ricDesc_modelloDelega" TabIndex="2" runat="server" CssClass="testo_grigio_scuro"
                                                Width="210"></asp:TextBox>
                                        </td>
                                        <td class="testo_piccoloB">
                                            <asp:ImageButton ID="btn_find_modelloDelega" runat="server" ToolTip="Cerca in organigramma" SkinID="btnFind">
                                            </asp:ImageButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lbl_avviso_modelloDelega" CssClass="testo_rosso" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" align="right">
                                <asp:Label ID="lbl_delegato_modelloDelega" runat="server" Width="160px" CssClass="testo_grigio_scuro">Seleziona delegato &nbsp;</asp:Label>
                            </td>
                            <td>
                                <iewc:TreeView ID="treeViewUO_modelloDelega" runat="server" AutoPostBack="True" Height="240px"
                                    Width="600px" BackColor="antiquewhite" BorderWidth="1px" BorderStyle="solid"
                                    font="verdana" DefaultStyle="font-weight:normal;font-size:10px;color:black;text-indent:0px;font-family:Verdana;"></iewc:TreeView>
                            </td>
                        </tr>
                        <tr>
                            <td height="10px" colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="lbl_data_inizio_modelloDelega" runat="server" Width="160px" CssClass="testo_grigio_scuro">Data decorrenza  &nbsp;</asp:Label>
                            </td>
                            <td>
                                <uc3:Calendario ID="txt_data_inizio_modelloDelega" runat="server" Visible="true" />
                                &nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lbl_ora_inizio_modelloDelega" runat="server" Width="30px" CssClass="testo_grigio_scuro">Ora</asp:Label>
                                <asp:DropDownList ID="ddl_ora_inizio_modelloDelega" CssClass="testo_grigio" runat="server"
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
                                <asp:Label ID="lbl_intervallo_modelloDelega" runat="server" CssClass="testo_grigio_scuro">Intervallo (gg)</asp:Label>
                                &nbsp;
                                <asp:TextBox ID="txt_intervallo_modelloDelega" runat="server" Visible="true" CssClass="testo_grigio"/>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lbl_data_fine_modelloDelega" runat="server" CssClass="testo_grigio_scuro">Data scadenza</asp:Label>
                                &nbsp;
                                <uc3:Calendario ID="txt_data_fine_modelloDelega" runat="server" Visible="true" />
                                &nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lbl_ora_fine_modelloDelega" runat="server" Width="30px" CssClass="testo_grigio_scuro">Ora</asp:Label>
                                <asp:DropDownList ID="ddl_ora_fine_modelloDelega" CssClass="testo_grigio" runat="server" Visible="true">
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
                                <asp:Button ID="btn_conferma_modelloDelega" CssClass="pulsante92" runat="server" Text="Conferma"
                                    ToolTip="Conferma nuovo modello delega" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </asp:Panel>
        <!--FINE INSERIMENTO/MODIFICA MODELLO DI DELEGA-->
        <tr>
            <td class="titolo_scheda" colspan="2" align="center">
                <table id="btn_modelloDelega" cellspacing="0" cellpadding="2" width="100%" align="center"
                    border="0" runat="server">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btn_nuovo_modelloDelega" CssClass="pulsante69" runat="server" Text="Nuovo" ToolTip="Nuovo modello delega"  Enabled="true"/>
                            <asp:Button ID="btn_modifica_modelloDelega" CssClass="pulsante69" runat="server" Text="Modifica" ToolTip="Modifica modello delega" Enabled="false"/>
                            <asp:Button ID="btn_cancella_modelloDelega" CssClass="pulsante69" runat="server" Text="Cancella" ToolTip="Cancella modello delega" Enabled="false"/>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        </asp:View>
        <!--FINE MODELLI DI DELEGA-->
        </asp:MultiView>
    </table>
    </form>
</body>
</html>
