<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="tabRisultatiRicArchivio.aspx.cs" Inherits="DocsPAWA.Archivio.tabRisultatiRicArchivio" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider" TagPrefix="uct" %>
<%@ Register src="../waiting/WaitingPanel.ascx" tagname="WaitingPanel" tagprefix="uc2" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/docspA_30.css" type="text/css" rel="stylesheet">
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <script type="text/javascript">
		function ShowWaitCursor()
		{
			window.document.body.style.cursor="wait";
		}
		
		// Script eseguito in fase di cambio pagina griglia
		function WaitGridPagingAction()
		{
			ShowWaitCursor();
		
			var ctl=document.getElementById("lbl_docProtocollati");
			if (ctl!=null)
				ctl.innerHTML="Ricerca in corso...";
		}
		
        function btn_archivia_clientClick()
        {
           ShowWaitPanel("<%=this.WaitingPanelTitle%>");
        }
        
    </script>
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" rightmargin="0" ms_positioning="GridLayout">
    <form id="tabDoc" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Lista Documenti" />
    <table height="100%" cellspacing="0" cellpadding="0" width="95%" align="right" border="0">
        <tr>
            <td height="2">
            </td>
        </tr>
        <tr id="trHeader" runat="server">
            <td class="pulsanti">
                <table width="100%">
                    <tr id="tr1" runat="server">
                        <td id="Td2" align="left" height="90%" runat="server" width="70%">
                            <asp:Label ID="titolo" CssClass="titolo" runat="server"></asp:Label>
                        </td>
                        <td class="testo_grigio_scuro" style="height: 19px" valign="middle" align="right">
                            <asp:ImageButton ID="btn_archivia" runat="server" ImageUrl="../images/proto/btn_archivia.gif"
                                AlternateText="Archivia il risultato della ricerca" OnClick = "btn_archivia_Click" OnClientClick="btn_archivia_clientClick()">
                            </asp:ImageButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td height="2"> <asp:Label class="testo_red" ID="lbl_messaggio" runat="server" Visible=false ></asp:Label>
            </td>
        </tr>
        <tr height="100%" runat="server">
            <td valign="top">
                <table height="100%" cellspacing="0" cellpadding="0" width="100%" align="right" border="0">
                    <tr>
                        <td class="titolo_grigio" align="center" style="width: 100%" runat="server" valign="top">
                                <asp:DataGrid ID="dgDoc" runat="server" SkinID="datagrid" Width="100%" PageSize="10"  
                                    AllowCustomPaging="True" AllowPaging="true"
                                    HorizontalAlign="Center" BorderColor="Gray" BorderWidth="1px" CellPadding="1"
                                    AutoGenerateColumns="False" BorderStyle="Inset" AllowSorting="True">
                                    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                    <ItemStyle HorizontalAlign="Center" Height="20px" CssClass="bg_grigioN" VerticalAlign="Middle"></ItemStyle>
                                    <HeaderStyle Height="20px" ForeColor="White" CssClass="bg_GrigioH"></HeaderStyle>
                                    <Columns>
                                        <asp:TemplateColumn>
                                            <HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="10%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:Label ID="LblSerie" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.numSerie") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TxtSerie" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.numSerie") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Doc.">
                                            <HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="10%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrDoc") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrDoc") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn Visible="False" HeaderText="segnatura">
                                            <HeaderStyle Font-Bold="True" HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>'
                                                    ID="Label2">
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>'
                                                    ID="Textbox2">
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="registro">
                                            <HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="15%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codRegistro") %>'
                                                    ID="Label4">
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codRegistro") %>'
                                                    ID="Textbox4">
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="tipo Prot">
                                            <HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="3%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipoProto") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="Textbox5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipoProto") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="oggetto">
                                            <HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="20%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:Label runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.oggetto")) %>'
                                                    ID="Label6">
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.oggetto") %>'
                                                    ID="Textbox6">
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="mitt/dest">
                                            <HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="30%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:Label runat="server" Text='<%# DocsPAWA.Utils.TruncateString_MittDest(DataBinder.Eval(Container, "DataItem.mittDest")) %>'
                                                    ID="Label8">
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.mittDest") %>'
                                                    ID="Textbox8">
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Dett.">
                                            <HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="5%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:ImageButton ID="Imagebutton2" runat="server" BorderColor="#404040" ImageUrl="../images/proto/dettaglio.gif"
                                                    CommandName="Select" ToolTip='<%# DataBinder.Eval(Container, "DataItem.descrDoc")  %>'>
                                                </asp:ImageButton>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:BoundColumn Visible="False" DataField="dataAnnullamento">
                                            <HeaderStyle Font-Bold="True"></HeaderStyle>
                                        </asp:BoundColumn>
                                        <asp:TemplateColumn Visible="False" HeaderText="Num Prot">
                                            <HeaderStyle Wrap="False"></HeaderStyle>
                                            <ItemTemplate>
                                                <asp:Label ID="Label77" runat="server" Text='<%# getSegnatura((string) DataBinder.Eval(Container, "DataItem.segnatura")) %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="Textbox77" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>'>
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="VIS">
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                            <HeaderTemplate>
                                                <asp:Label ID="lbl_Vis" runat="server" CssClass="menu_1_bianco_dg">Vis.</asp:Label>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:ImageButton ID="IMG_VIS" runat="server" ImageUrl="../images/proto/dett_lente_doc.gif" CommandName="VisDoc" visible=false></asp:ImageButton>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn Visible="False" HeaderText="idProfile">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_idprofile" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn Visible="False" HeaderText="docNumber">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_docnumber" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.docNumber") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn Visible="False" HeaderText="chaImg">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_chaImg" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.acquisitaImmagine") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn Visible="true" HeaderText="Trasf. deposito">
                                            <HeaderStyle Wrap="false" Font-Bold="True" HorizontalAlign="center" Width="5px" />
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <cc1:ImageButton ID="img_Archivio" runat="server" Width="18px" Height="18px" ImageUrl="../images/proto/btn_archiviaSC.gif"
                                                    CommandName="InArchivio" AlternateText="Inserisci questo fascicolo in 'Archivio'" />
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                    </Columns>
                                    <PagerStyle HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages" Position="TopAndBottom"></PagerStyle>
							 </asp:DataGrid>
                            
                                <uc2:WaitingPanel ID="WaitingPanel1" runat="server" />
                            
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <uc1:DataGridPagingWait ID="dgDoc_pagingWait" runat="server"></uc1:DataGridPagingWait>
    </form>
</body>
</html>
