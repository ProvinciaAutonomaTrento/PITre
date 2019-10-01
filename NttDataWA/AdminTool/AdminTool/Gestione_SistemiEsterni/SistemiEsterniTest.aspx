<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SistemiEsterniTest.aspx.cs"
    Inherits="SAAdminTool.AdminTool.Gestione_SistemiEsterni.SistemiEsterniTest" %>

<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <script language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <script language="javascript" src="../../LIBRERIE/rubrica.js"></script>
    <script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
    <script language="C#" runat="server">
			public bool getCheckBox(object abilita)
			{			
				string abil = abilita.ToString();
				if(abil == "true")
				{
					return true;
				}
				else
				{
					return false;
				}
			}                   
		</script>
    <script language="JavaScript">

        var cambiapass;
        var hlp;

        function apriPopup() {
            window.open('../help.aspx?from=RG', '', 'width=450,height=500,scrollbars=YES');
        }
        function cambiaPwd() {
            window.open('../CambiaPwd.aspx', '', 'width=410,height=300,scrollbars=NO');
        }
        function ClosePopUp() {
            if (typeof (cambiapass) != 'undefined') {
                if (!cambiapass.closed)
                    cambiapass.close();
            }
            if (typeof (hlp) != 'undefined') {
                if (!hlp.closed)
                    hlp.close();
            }
        }
		
			
			
    </script>
    <style type="text/css">
        .style1
        {
            font-weight: bold;
            font-size: 12px;
            color: #4b4b4b;
            font-family: Verdana;
            width: 136%;
        }
        .style2
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 20%;
        }
        .style3
        {
            width: 369px;
        }
    </style>
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" rightmargin="0" onunload="ClosePopUp()">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server">
    </asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Sistemi Esterni" />
    <!-- Gestione del menu a tendina -->
    <uc2:MenuTendina ID="MenuTendina" runat="server"></uc2:MenuTendina>
    <table height="800px" cellspacing="1" cellpadding="0" width="100%" border="0">
        <tr>
            <td>
                <!-- TESTATA CON MENU' -->
                <uc1:Testata ID="Testata" runat="server"></uc1:Testata>
            </td>
        </tr>
        <tr>
            <!-- STRISCIA SOTTO IL MENU -->
            <td class="testo_grigio_scuro" bgcolor="#c0c0c0" height="20">
                <asp:Label ID="lbl_position" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <!-- TITOLO PAGINA -->
            <td class="titolo" align="center" width="100%" bgcolor="#e0e0e0" style="height: 20px">
                Sistemi Esterni
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <asp:UpdatePanel ID="UpdatePanelSistemiEsterni" runat="server">
                    <ContentTemplate>
                    <asp:UpdateProgress  ID="updProgressCasella" runat="server" >
                            <ProgressTemplate >
                                <div style="width:80%;height:80%;color:black;position:absolute; 
                                    top:38%; left:10%;right:10%;bottom:0%; vertical-align:middle;padding-top:30%;"> 
                                    <asp:Label ID="lbl_wait" runat="server" Font-Bold="true" Font-Size="Medium">Attendere...</asp:Label>
                                    <asp:Image runat="server" ID="img_load_caselle" ImageUrl="~/images/loading.gif" ImageAlign="Middle" />
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <table cellspacing="0" cellpadding="0" align="center" border="0" width="820px">
                            <tr>
                                <td align="center" height="25">
                                </td>
                            </tr>
                            <tr>
                                <td class="pulsanti" align="center">
                                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td class="titolo" align="left">
                                                <asp:Label ID="lbl_tit" CssClass="titolo" runat="server">Sistemi Esterni</asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Button ID="btn_nuovo" runat="server" CssClass="testo_btn" Text="Nuovo Sistema Esterno">
                                                </asp:Button>&nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td height="5">
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <div id="DivDGList" style="overflow: auto; width: 100%; height: 133px">
                                        <asp:DataGrid ID="dg_SistemiEsterni" runat="server" Width="100%" BorderWidth="1px"
                                            CellPadding="1" BorderColor="Gray" AutoGenerateColumns="False">
                                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                            <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                            <Columns>
                                                <asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID">
                                                    <HeaderStyle Width="0%"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="Codice" HeaderText="Codice Applicazione">
                                                    <HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="UserID_UtSys" HeaderText="UserID">
                                                    <HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="Desc_Estesa" HeaderText="Descrizione">
                                                    <HeaderStyle HorizontalAlign="Center" Width="40%"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:BoundColumn Visible="False" DataField="TknTime" HeaderText="Periodo Token">
                                                    <HeaderStyle Width="0%"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:BoundColumn Visible="False" DataField="Diritti" HeaderText="Diritti">
                                                    <HeaderStyle Width="0%"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:BoundColumn Visible="False" DataField="IdRole" HeaderText="Id applicazione">
                                                    <HeaderStyle Width="0%"></HeaderStyle>
                                                </asp:BoundColumn>
                                                <asp:ButtonColumn Text="&lt;img src=../Images/lentePreview.gif border=0 alt='Dettaglio'&gt;"
                                                    CommandName="Select">
                                                    <HeaderStyle Width="5%"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:ButtonColumn>
                                                <asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 alt='Elimina'&gt;"
                                                    CommandName="Delete">
                                                    <HeaderStyle Width="5%"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                </asp:ButtonColumn>
                                            </Columns>
                                        </asp:DataGrid></div>
                                </td>
                            </tr>
                            <tr>
                                <td height="10">
                                </td>
                            </tr>
                            <asp:Panel ID="pnl_dett_sist_esterno" runat="server" Visible="true" Width="100%">
                                <tr>
                                    <td>
                                        <asp:Panel ID="pnl_info_sist_esterno" runat="server" Visible="true">
                                            <tr>
                                                <td>
                                                <asp:HiddenField ID="hd_idSysExt" runat="server" />
                                                <asp:HiddenField ID="hd_idSysRole" runat="server" />
                                                    <table class="contenitore" width="100%">
                                                        <tr>
                                                            <td colspan="2">
                                                                <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                                                    <tr>
                                                                        <td class="titolo_pnl" valign="middle" align="center" width="5%">
                                                                        </td>
                                                                        <td class="titolo_pnl" valign="middle" width="15%">
                                                                            Sistema Esterno:
                                                                        </td>
                                                                        <td class="titolo_pnl" valign="middle" align="right" width="80%">
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" width="20%">
                                                                Codice Applicazione* :
                                                            </td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txt_Sys_CodeApp" TabIndex="21" runat="server" CssClass="testo" Width="175px"
                                                                    MaxLength="30"></asp:TextBox>
                                                                    <asp:Label ID="lbl_err_sys_codeApp" runat="server" CssClass="testo_rosso" Text="Codice applicazione non valido" Visible="false" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" width="20%">
                                                                User ID* :
                                                            </td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txt_Sys_Ut_Sys" TabIndex="22" runat="server" CssClass="testo"
                                                                    Width="350px" MaxLength="128"></asp:TextBox>
                                                                    <asp:Label ID="lbl_err_sys_Ut_Sys" runat="server" CssClass="testo_rosso" Text="Nome Utente non valido" Visible="false" />
                                                            
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="testo_grigio_scuro" width="20%">
                                                                Descrizione:
                                                            </td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txt_Sys_extDesc" TabIndex="23" runat="server" CssClass="testo" Width="350px"
                                                                    TextMode="MultiLine" MaxLength="2000"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr id="tr_Sys_tknTime" runat="server">
                                                            <td class="testo_grigio_scuro" width="20%">
                                                                Periodo Token:
                                                            </td>
                                                            <td align="left" class="testo_grigio_scuro">
                                                                <asp:TextBox ID="txt_Sys_tknTime" TabIndex="24" runat="server" CssClass="testo" Width="175px"
                                                                    MaxLength="30"></asp:TextBox>
                                                                &nbsp;minuti
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="titolo_pnl" colspan="2">
                                                                <table cellspacing="0" cellpadding="0" align="right" border="0" width="100%">
                                                                    <tr>
                                                                        <td class="titolo_pnl"  align="right">
                                                                            &nbsp;
                                                                            <asp:Button ID="btn_mod_Sys" runat="server" CssClass="testo_btn_org" Text="Modifica"
                                                                                ToolTip="Modifica Sistema Esterno" OnClientClick="return ShowConfirmBeforeStartModMove(this);">
                                                                            </asp:Button>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </asp:Panel>
                            <tr>
                                <td>
                                    <!-- REGISTRI/RF -->
                                    <asp:Panel ID="pnl_reg_sist_esterno" runat="server" Visible="true" CssClass="contenitore">
                                        <asp:Panel ID="pnl_reg">
                                            <!-- intestazione Registro -->
                                            <table style="width: 99%; margin: 3px;" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td class="titolo_pnl" valign="middle" align="center" width="5%">
                                                        <img src="../Images/registri.gif" border="0">
                                                    </td>
                                                    <td class="titolo_pnl" valign="middle" width="30%">
                                                        Registri
                                                    </td>
                                                    <td class="titolo_pnl" valign="middle" align="right" width="65%">
                                                    </td>
                                                </tr>
                                            </table>
                                            <div style="overflow: auto; height: 89px; width: 99%; padding: 3px;">
                                                <asp:UpdatePanel ID="updPanelRegistri" runat="server" UpdateMode="Always">
                                                    <ContentTemplate>
                                                        <asp:DataGrid Width="100%" ID="dg_registri" TabIndex="28" runat="server" Height="59px"
                                                            AutoGenerateColumns="False" CellPadding="1" BorderWidth="1px" BorderColor="Gray">
                                                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                                            <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                                            <Columns>
                                                                <asp:BoundColumn Visible="False" DataField="IDRegistro" ReadOnly="True" HeaderText="ID">
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn DataField="Codice" ReadOnly="True" HeaderText="Codice">
                                                                    <ItemStyle Wrap="true"></ItemStyle>
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn DataField="Descrizione" ReadOnly="True" HeaderText="Descrizione">
                                                                    <ItemStyle Wrap="true"></ItemStyle>
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn DataField="EmailRegistro" ReadOnly="True" HeaderText="Email" Visible="false">
                                                                    <ItemStyle Wrap="true"></ItemStyle>
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn Visible="False" DataField="IDAmministrazione" ReadOnly="True" HeaderText="IDAmm">
                                                                </asp:BoundColumn>
                                                                <asp:TemplateColumn HeaderText="Sel">
                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                    <ItemStyle Wrap="true" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="cbx_Sel" AutoPostBack="true"
                                                                            Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.sel")) %>' runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn HeaderText="Cons"  Visible="false">
                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="cbx_Consulta" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.consulta")) %>'
                                                                            runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn HeaderText="Notifica"  Visible="false">
                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="cbx_Notifica" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.notifica")) %>'
                                                                            runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn HeaderText="Spedisci"  Visible="false">
                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="cbx_Spedisci" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.spedisci")) %>'
                                                                            runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:BoundColumn Visible="False" DataField="Sospeso" ReadOnly="True" HeaderText="Sospeso">
                                                                </asp:BoundColumn>
                                                            </Columns>
                                                        </asp:DataGrid>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </asp:Panel>
                                        <br />
                                        <asp:Panel runat="server" ID="pnlRF">
                                            <!-- intestazione rf -->
                                            <table style="width: 99%; margin: 3px;" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td class="titolo_pnl" valign="middle" align="center" width="5%">
                                                        <img src="../Images/registri.gif" border="0">
                                                    </td>
                                                    <td class="titolo_pnl" valign="middle" width="30%">
                                                        RF
                                                    </td>
                                                    <td class="titolo_pnl" valign="middle" align="right" width="65%">
                                                    </td>
                                                </tr>
                                            </table>
                                            <!-- content rf -->
                                            <div style="overflow: auto; height: 89px; width: 99%; padding: 3px;">
                                                <asp:UpdatePanel ID="updPanelRF" runat="server" UpdateMode="Always">
                                                    <ContentTemplate>
                                                        <asp:DataGrid Width="100%" ID="dg_RF" TabIndex="28" runat="server" Height="59px"
                                                            AutoGenerateColumns="False" CellPadding="1" BorderWidth="1px" BorderColor="Gray">
                                                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                                            <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                                            <Columns>
                                                                <asp:BoundColumn Visible="False" DataField="IDRegistro" ReadOnly="True" HeaderText="ID">
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn DataField="Codice" ReadOnly="True" HeaderText="Codice">
                                                                    <ItemStyle Wrap="true"></ItemStyle>
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn DataField="Descrizione" ReadOnly="True" HeaderText="Descrizione">
                                                                    <ItemStyle Wrap="true"></ItemStyle>
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn DataField="EmailRegistro" ReadOnly="True" HeaderText="Email" Visible="false">
                                                                    <ItemStyle Wrap="true" />
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn Visible="False" DataField="IDAmministrazione" ReadOnly="True" HeaderText="IDAmm">
                                                                </asp:BoundColumn>
                                                                <asp:TemplateColumn HeaderText="Sel">
                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="cbx_Sel" AutoPostBack="true" 
                                                                            Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.sel")) %>' runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn HeaderText="Cons" Visible="false">
                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="cbx_Consulta" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.consulta")) %>'
                                                                            runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn HeaderText="Notifica" Visible="false">
                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="cbx_Notifica" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.notifica")) %>'
                                                                            runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn HeaderText="Spedisci" Visible="false">
                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="cbx_Spedisci" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.spedisci")) %>'
                                                                            runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:BoundColumn DataField="Disabled" ReadOnly="True" HeaderText="" Visible="False">
                                                                    <ItemStyle></ItemStyle>
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn DataField="AooCollegata" ReadOnly="True" HeaderText="" Visible="False">
                                                                    <ItemStyle></ItemStyle>
                                                                </asp:BoundColumn>
                                                            </Columns>
                                                        </asp:DataGrid>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlTitolo" runat="server">
                                            <table style="width: 99%; margin: 3px;" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td class="titolo_pnl" valign="middle" align="right">
                                                        <asp:Button ID="btn_mod_registri" TabIndex="29" runat="server" CssClass="testo_btn_org"
                                                            Text="Modifica"></asp:Button>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Panel ID="pnl_metodi_pis" runat="server" Visible="true" CssClass="contenitore">
                                        <asp:Panel ID="pnl_pis">
                                            <table style="width: 99%; margin: 3px;" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td class="titolo_pnl" valign="middle" align="center" width="5%">
                                                        <img src="../Images/registri.gif" border="0">
                                                    </td>
                                                    <td class="titolo_pnl" valign="middle" width="30%">
                                                        Funzioni permesse
                                                    </td>
                                                    <td class="titolo_pnl" valign="middle" align="right" width="65%">
                                                    </td>
                                                </tr>
                                            </table>
                                            <div style="overflow: auto; height: 89px; width: 99%; padding: 3px;">
                                                <asp:UpdatePanel ID="uppnl_pis" runat="server" UpdateMode="Always">
                                                    <ContentTemplate>
                                                        <asp:DataGrid Width="100%" ID="dg_pis" TabIndex="28" runat="server" Height="59px"
                                                            AutoGenerateColumns="False" CellPadding="1" BorderWidth="1px" BorderColor="Gray">
                                                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                                            <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                                            <Columns>
                                                                <asp:BoundColumn Visible="False" DataField="IDPIS" ReadOnly="True" HeaderText="ID">
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn DataField="nomePIS" ReadOnly="True" HeaderText="Nome metodo">
                                                                    <ItemStyle Wrap="true"></ItemStyle>
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn DataField="Descrizione" ReadOnly="True" HeaderText="Descrizione">
                                                                    <ItemStyle Wrap="true"></ItemStyle>
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn DataField="FILE_SVC" ReadOnly="True" HeaderText="Servizio">
                                                                    <ItemStyle Wrap="true"></ItemStyle>
                                                                </asp:BoundColumn>
                                                                <asp:TemplateColumn HeaderText="Sel">
                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="true"></ItemStyle>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="cbx_Sel_pis" runat="server"  Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.sel")) %>'/>
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                            </Columns>
                                                        </asp:DataGrid>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pnl_btn_pis" runat="server">
                                            <table style="width: 99%; margin: 3px;" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td class="titolo_pnl" valign="middle" align="right">
                                                        <asp:Button ID="btn_mod_pis" TabIndex="29" runat="server" CssClass="testo_btn_org"
                                                            Text="Modifica"></asp:Button>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
                
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
