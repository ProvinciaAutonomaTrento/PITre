<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ElencoNote.aspx.cs" Inherits="DocsPAWA.Note.ElencoNote" %>

<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DOCSPA > Gestione Elenco Note</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <meta http-equiv="Pragma" content="no-cache">
    <meta http-equiv="Expires" content="-1">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <base target="_self" />

    <script language="javascript">
        function OpenHelp(from) {
            var pageHeight = 600;
            var pageWidth = 800;
            var posTop = (screen.availHeight - pageHeight) / 2;
            var posLeft = (screen.availWidth - pageWidth) / 2;

            var newwin = window.showModalDialog('../Help/Manuale.aspx?from=' + from,
							    '',
							    'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:yes;dialogLeft:' + posLeft + ';dialogTop:' + posTop + ';center:yes;help:no');
        }

    </script>
</head>
<body bottommargin="1" leftmargin="1" topmargin="1" rightmargin="1" ms_positioning="GridLayout">
    <form id="elencoNote" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider1" runat="server" PageName="Gestione elenco note" />
    <table class="info" width="95%" align="center" border="0">
        <tr>
            <td class="item_editbox">
                <p class="boxform_item">
                    <asp:Label ID="lbl_titolo" runat="server">Gestione elenco note</asp:Label>
                </p>
            </td>
        </tr>
        <tr>
            <td height="5px">
            </td>
        </tr>
        
        <tr>
            <td align="right"><asp:ImageButton id="help" runat="server" OnClientClick="OpenHelp('ElencoNote')" AlternateText="Aiuto?" SkinID="btnHelp" />
            </td>
        </tr>
        <tr>
            <td class="testo_grigio_scuro">Filtro per: &nbsp;&nbsp;&nbsp;RF&nbsp;
                <asp:DropDownList ID="ddlFiltroRf" runat="server" Visible="true" Width="400px"  CssClass="testo_grigio"></asp:DropDownList>                
                &nbsp;&nbsp;&nbsp;Descrizione&nbsp;
                <asp:TextBox ID="txt_desc" runat="server"  Visible="true"  Width="200px" CssClass="testo_grigio"></asp:TextBox>&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_filtro" runat="server" CssClass="pulsante69" Text="Ricerca" />
            </td>
        </tr>
        <tr style="height: 10px">
            <td align="right">
                <a href="Note.xls" target="_blank" style="color:Blue;" class="testo_grigio9" >Scarica modello</a>
            </td>
        </tr>
        <tr width=95%>
            <td class="pulsanti">
                <asp:Label ID="lbl_messaggio" runat="server" CssClass="testo_grigio_scuro" Text="" Visible="true"></asp:Label>
            </td>
        </tr>
        <tr width=95%>
            <td align="center" >
                <div id="DivDGElenco" style="overflow: auto; height: 200px">
                    <asp:DataGrid ID="dgNote" runat="server" SkinID="datagrid" BorderColor="Gray" AutoGenerateColumns="false"
                        BorderWidth="1px" CellPadding="1" AllowCustomPaging="false" AllowPaging="false" Width="99%">
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
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="idnota" Visible="false" ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundColumn DataField="codRegRf" HeaderText="Codice" ItemStyle-Width="45%" ReadOnly="true"
                                HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                            <asp:BoundColumn DataField="descNota" HeaderText="Descrizione" ItemStyle-Width="45%"
                                ReadOnly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                        </Columns>
                    </asp:DataGrid>
                </div>
            </td>
        </tr>
        <tr>
            <td height="15px">
            </td>
        </tr>
        <asp:Panel ID="pnl_nuovaNota" runat="server" Visible="false">
            <tr>
                <td class="pulsanti">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                        <tr>
                            <td>
                                <asp:Label ID="lbl_operazione" runat="server" CssClass="testo_grigio_scuro">Nuova nota</asp:Label>
                            </td>
                            <td align="right">
                                <asp:ImageButton ID="btn_chiudiPnl" TabIndex="7" runat="server" ToolTip="Chiudi"
                                    ImageUrl="../Images/cancella.gif"></asp:ImageButton>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr height="50px">
                <td align="center">
                    <table border="0" cellpadding="0" cellspacing="0" class="info" width="100%">
                        <asp:Panel ID="pnl_excel" runat="server">
                            <tr>
                                <td height="5px">
                                </td>
                            </tr>
                            <tr>
                                <td class="testo_grigio_scuro" align="left" valign="middle" width="150px">
                                    <asp:CheckBox CssClass="testo_grigio_scuro" ID="chk_Excel" runat="server" AutoPostBack="true"
                                        OnCheckedChanged="chk_Excel_Changed" />Inserimento da Excel
                                </td>
                                <td valign="middle" colspan="4" align="left">
                                    <asp:FileUpload class="testo_grigio" ID="uploadedFile"  CssClass="PULSANTE" size="55" name="uploadedFile"
                                        runat="server"></asp:FileUpload>&nbsp;
                                    <asp:Button ID="UploadBtn" Text="Carica" CssClass="PULSANTE"  runat="server"></asp:Button>&nbsp;
                                    <asp:Label ID="lbl_fileExcel" runat="server" CssClass="testo_grigio_scuro">&nbsp;Nessun file excel caricato.</asp:Label>
                                    <asp:ImageButton ID="btn_elimina_excel" runat="server" ImageUrl="../images/proto/b_elimina.gif"
                                        ToolTip="Elimina" Visible="false" CssClass />
                                </td>
                            </tr>
                            <tr height="10">
                                <td>
                                </td>
                            </tr>
                        </asp:Panel>
                        <tr>
                            <td align="left">
                                <asp:Label ID="lbl_codRegRf" runat="server" Width="150px" CssClass="testo_grigio_scuro">&nbsp;Seleziona registro/rf &nbsp;</asp:Label>
                            </td>
                            <td valign="middle" align="left">
                                <asp:DropDownList ID="ddl_regRF" runat="server" Visible="true" CssClass="testo_grigio"
                                    Width="320px" AutoPostBack="true">
                                </asp:DropDownList>
                            </td>
                            <td align="right">
                                <asp:Label ID="lbl_descNota" runat="server" Width="80px" CssClass="testo_grigio_scuro">Testo nota &nbsp;</asp:Label>
                            </td>
                            <td valign="middle">
                                <asp:TextBox ID="txt_descNota" runat="server" CssClass="testo_grigio" Width="280px"></asp:TextBox>
                            </td>
                            
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="pnl_risultatiExcel" runat="server" Height="220px" BorderStyle="Solid" BorderColor="#810d06"
                            BorderWidth="1px" ScrollBars="Auto" Visible="false">
                            <asp:Label ID="lbl_risultatiExcel" CssClass="testo_grigio_scuro">Risultati import note:</asp:Label> <br />
                            <asp:GridView ID="grdRisExcel" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                AutoGenerateColumns="false" Width ="98%">
                                <RowStyle BackColor="#FFFBD6" ForeColor="#333333" Font-Size="Small" />
                                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                                <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                                <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" Font-Size="Small" />
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    
                                    <asp:BoundField DataField="Message" HeaderText="Nota (Descrizione --- RF)"  ItemStyle-Width="39%" />
                                    <asp:TemplateField HeaderText="Risultato" ItemStyle-Width="10%">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dettagli" ItemStyle-Width="50%">
                                        <ItemTemplate>
                                            <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
            
                    </asp:Panel>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td height="15px">
            </td>
        </tr>
        <tr>
            <td class="titolo_scheda" colspan="2" align="center">
                <table id="tblButtonsContainer" cellspacing="0" cellpadding="2" width="100%" align="center"
                    border="0" runat="server">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btn_nuova" CssClass="pulsante79" runat="server" Text="Nuova" ToolTip="Nuova nota" />
                            <asp:Button ID="btn_modifica" CssClass="pulsante79" runat="server" Text="Modifica"
                                ToolTip="Modifica nota" />
                            <asp:Button ID="btn_rimuovi" CssClass="pulsante79" runat="server" Text="Rimuovi"
                                ToolTip="Rimuovi nota" />
                            <asp:Button ID="btn_chiudi" CssClass="pulsante69" runat="server" Text="Chiudi" ToolTip="Chiudi" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
