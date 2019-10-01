<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OpzioniScarto.aspx.cs"
    Inherits="DocsPAWA.Scarto.OpzioniScarto" %>

<%@ Register TagPrefix="mytree" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

    <link href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">

    <script language="JavaScript">
        var w = window.screen.width;
		var h = window.screen.height;
		var new_w = (w-100)/2;
		var new_h = (h-400)/2;
        function ApriSceltaTitolario(codClassifica)
		{   
	    	//window.open('../popup/sceltaTitolari.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=420,scrollbars=YES');
			window.showModalDialog('../popup/sceltaTitolari.aspx?CodClassifica='+codClassifica,'','dialogWidth:500px;dialogHeight:300px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);  
			opzioniScarto.submit();
		}	
		
		function SingleSelect(regex,current) 
        { 
            re = new RegExp(regex);
            for(i = 0; i < document.forms[0].elements.length;i++)
            {
        
                elm = document.forms[0].elements[i];

                if (elm.type == 'checkbox' && elm != current && re.test(elm.name))
                {
                    elm.checked = false; 
                } 
            } 
        }
    </script>

</head>
<body leftmargin="1" ms_positioning="GridLayout">
    <form id="opzioniScarto" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Gestione area di scarto" />
    <table id="tbl_contenitore" height="100%" cellspacing="0" cellpadding="0" width="408"
        align="center" border="0">
        <tr valign="top" height="20px">
            <td valign="top" class="item_editbox">
                <p class="boxform_item">
                    <asp:Label ID="lbl_titolo" CssClass="titolo_scheda" Text="Gestione area di scarto"
                        runat="server"></asp:Label>
                </p>
            </td>
        </tr>
        <tr>
            <td height="5" valign="top">
            </td>
        </tr>
        <tr valign="top">
            <td>
                <table width="96%" height="100%" border="0" cellpadding="0" cellspacing="0" align="center">
                    <tr valign="bottom" height="15">
                        <td style="height: 18px" height="18" valign="bottom">
                            <cc1:ImageButton ID="btn_filtroRicerca" Width="67px" BorderWidth="0px" Height="19px"
                                Visible="true" runat="server" Thema="ricerca_" SkinID="attivo"
                                AlternateText="Ricerca" ></cc1:ImageButton>
                            <cc1:ImageButton ID="btn_Istanza" Thema="lista_" Width="67px" BorderWidth="0px" Height="19px" Visible="true"
                                runat="server" AlternateText="Lista istanze di scarto" SkinID="scarto" ></cc1:ImageButton>
                        </td>
                    </tr>
                    <asp:Panel ID="pnl_filtroRicerca" runat="server" Visible="false">
                        <tr valign="top">
                            <td valign="top">
                                <table class="info" width="100%" border="0" align="center">
                                    <tr>
                                        <td height="25" class="titolo_rosso" align="center" valign="top" colspan="3">
                                            RICERCA FASCICOLI
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td colspan="3">
                                            <asp:RadioButtonList ID="rb_opzioni" runat="server" Width="300px" CssClass="testo_grigio"
                                                AutoPostBack="True">
                                                <asp:ListItem Value="tutti">Ricerca tutti i fascicoli in archivio di deposito</asp:ListItem>
                                                <asp:ListItem Value="codice">Ricerca fascicolo per codice</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    <asp:Panel ID="pnl_ricXCodice" runat="server">
                                    <tr>
                                        <td height="5px">
                                        </td>
                                    </tr>
                                    <tr height="25">
                                        <td class="testo_grigio_scuro">
                                            &nbsp; Registro
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddl_registri" runat="server" CssClass="testo_grigio" Width="120px"
                                                AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="center">
                                            <cc1:ImageButton ID="btn_titolario" ImageUrl="../images/proto/ico_titolario_noattivo.gif"
                                                Height="17px" runat="server" DisabledUrl="../images/proto/ico_titolario_noattivo.gif"
                                                AlternateText="Titolario"></cc1:ImageButton>
                                        </td>
                                    </tr>
                                    <tr height="25">
                                        <td class="testo_grigio_scuro">
                                            &nbsp; Titolario
                                        </td>
                                        <td colspan="2">
                                            <asp:DropDownList ID="ddl_titolari" runat="server" CssClass="testo_grigio" Width="260px"
                                                AutoPostBack="True" EnableViewState="True">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr height="25">
                                        <td class="testo_grigio_scuro">
                                            &nbsp; Codice
                                        </td>
                                        <td colspan="2">
                                            <asp:TextBox ID="txt_codice" runat="server" CssClass="testo_grigio" AutoPostBack="true"
                                                Width="50px"></asp:TextBox>&nbsp;&nbsp;
                                        </td>
                                    </tr>
                                    <tr height="25">
                                        <td class="testo_grigio_scuro">
                                            &nbsp;
                                            <asp:Label ID="lbl_mesi" Text="Mesi cons." runat="server"></asp:Label>
                                        </td>
                                        <td colspan="2">
                                            <asp:Label ID="mesi" CssClass="testo_grigio" runat="server" Width="40px"></asp:Label>&nbsp;&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <div style="width: 100%; height: 170;">
                                                <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                                    <tr>
                                                        <td class="testo_grigio_scuro">
                                                            <mytree:TreeView ID="Gerarchia" runat="server" CssClass="testo_grigio" Width="400px"
                                                                SystemImagesPath="../images/alberi/left/" name="Treeview1" BorderWidth="0px"
                                                                DefaultStyle="font-weight:normal;font-size:10px;color:#666666;text-indent:0px;font-family:Verdana;background-color: #d9d9d9;"
                                                                Height="170px" BorderStyle="Solid"></mytree:TreeView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                    </asp:Panel>
                                    <tr height="5px">
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnl_istanza" runat="server" Visible="false">
                        <tr valign="top">
                            <td valign="top">
                                <table class="info" width="100%" border="0" align="center">
                                    <tr>
                                        <td height="25" class="titolo_rosso" align="center" valign="top" colspan="3">
                                            LISTA ISTANZE DI SCARTO
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:DataGrid ID="dg_istanze" runat="server" SkinID="datagrid" AllowCustomPaging="True" BorderStyle="Inset"
                                                AutoGenerateColumns="False" Width="100%" CellPadding="1" BorderWidth="1px" BorderColor="Gray"
                                                HorizontalAlign="Center" AllowPaging="True">
                                                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                                <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                                <ItemStyle HorizontalAlign="Center" Height="20px" CssClass="bg_grigioN" VerticalAlign="Middle">
                                                </ItemStyle>
                                                <HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg"
                                                    BackColor="#810D06"></HeaderStyle>
                                                <Columns>
                                                    <asp:TemplateColumn HeaderText="Id. Istanza">
                                                        <HeaderStyle Width="5%"></HeaderStyle>
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.systemID") %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.systemID") %>'>
                                                            </asp:TextBox>
                                                        </EditItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Stato">
                                                        <HeaderStyle Width="5%"></HeaderStyle>
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
                                                            </asp:TextBox>
                                                        </EditItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn Visible="false">
                                                        <HeaderStyle Wrap="false" Width="5px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_Chiave" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                     <asp:TemplateColumn HeaderText="Descr.">
                                                        <HeaderStyle></HeaderStyle>
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label3" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="TextBox3" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>'>
                                                            </asp:TextBox>
                                                        </EditItemTemplate>
                                                    </asp:TemplateColumn>
                                                     <asp:TemplateColumn HeaderText="Data scarto">
                                                        <HeaderStyle></HeaderStyle>
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataScarto") %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="TextBox4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataScarto") %>'>
                                                            </asp:TextBox>
                                                        </EditItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Dett">
                                                        <HeaderStyle Width="5px"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="btn_dettaglio" runat="server" BorderWidth="1px" BorderColor="#404040"
                                                                ImageUrl="../images/proto/dettaglio.gif" CommandName="Select" AlternateText="Dettaglio">
                                                            </asp:ImageButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                </Columns>
                                                <PagerStyle HorizontalAlign="Center" Position="TopAndBottom" BackColor="#C2C2C2"
                                                    CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
                                            </asp:DataGrid>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:Panel>
                    <tr height="100%">
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr height="7%">
            <td>
                <!-- BOTTONIERA -->
                <table id="tbl_bottoniera" cellspacing="0" cellpadding="0" border="0" align="center">
                    <tr>
                        <td valign="top" height="5">
                            <img height="1" src="../images/proto/spaziatore.gif" width="1" border="0">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:ImageButton ID="btn_ricerca" Visible="true" runat="server" AlternateText="Ricerca" Thema="btn_" SkinID="ricerca_attivo">
                            </cc1:ImageButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
