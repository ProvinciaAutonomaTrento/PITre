<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="DocInCestino.aspx.cs"
    Inherits="DocsPAWA.popup.DocInCestino" EnableEventValidation="false" %>

<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">

    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

    <base target="_self" />
    <%Response.Expires = -1;%>

    <script language="javascript">	
	    function ShowDialogSearchDocuments() 
		{	
			var retValue=window.showModalDialog('../ricercaDoc/FiltriRicercaDocumenti.aspx?prov=Cestino',
										'',
												'dialogWidth:650px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
			
			DocCestino.txtFilterDocumentsRetValue.value=retValue;

			if (retValue)
				ShowWaitCursor()
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
		
		function StampaRisultatoRicerca()
		{				
		
			var args=new Object;
			args.window=window;
			window.showModalDialog("../exportDati/exportDatiSelection.aspx?export=docInCest",
										args,
										"dialogWidth:450px;dialogHeight:420px;status:no;resizable:no;scroll:no;center:yes;help:no;");						
		}
    </script>

</head>
<body bottommargin="2" leftmargin="2" topmargin="2" rightmargin="2" ms_positioning="GridLayout">
    <form id="DocCestino" runat="server" method="post">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Documenti rimossi" />
    <input id="txtFilterDocumentsRetValue" type="hidden" runat="server">
    <table cellspacing="0" cellpadding="0" width="100%" align="center">
        <tr>
            <td class="item_editbox">
                <p class="boxform_item">
                    <asp:Label ID="Label1" runat="server">Documenti in cestino</asp:Label></p>
            </td>
        </tr>
        <tr>
            <td align="right">
            </td>
        </tr>
        <tr>
            <td class="pulsanti">
                <table width="100%">
                    <tr>
                        <td align="left" height="75%">
                            <asp:Label ID="titolo" CssClass="titolo" runat="server"></asp:Label>
                        </td>
                        <td class="testo_grigio_scuro" style="height: 19px" valign="middle" align="right"
                            width="25%">
                            <asp:ImageButton ID="btn_svuota" runat="server" ImageUrl="../images/proto/rimuoviTDL.gif"
                                AlternateText="Rimuovi fisicamente tutti i documenti"></asp:ImageButton>&nbsp;&nbsp;
                            <asp:ImageButton ID="btn_stampa" runat="server" ImageUrl="../images/proto/export.gif"
                                AlternateText="Esporta il risultato della ricerca"></asp:ImageButton>
                            <asp:ImageButton id="help" runat="server" OnClientClick="OpenHelp('GestioneDocRimossi')" AlternateText="Aiuto?" SkinID="btnHelp" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <asp:DataGrid ID="DGDoc" SkinID="datagrid" runat="server" BorderWidth="1px" Width="100%"
                    AllowSorting="True" BorderStyle="Inset" AutoGenerateColumns="False" CellPadding="1"
                    BorderColor="Gray">
                    <EditItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                        Font-Underline="False" />
                    <SelectedItemStyle CssClass="bg_grigioS" Font-Bold="False" Font-Italic="False" Font-Overline="False"
                        Font-Strikeout="False" Font-Underline="False"></SelectedItemStyle>
                    <AlternatingItemStyle CssClass="bg_grigioA" Font-Bold="False" Font-Italic="False"
                        Font-Overline="False" Font-Strikeout="False" Font-Underline="False"></AlternatingItemStyle>
                    <ItemStyle CssClass="bg_grigioN" Font-Bold="False" Font-Italic="False" Font-Overline="False"
                        Font-Strikeout="False" Font-Underline="False"></ItemStyle>
                    <HeaderStyle Wrap="False" Height="20px" HorizontalAlign="Center" ForeColor="White"
                        CssClass="menu_1_bianco_dg"></HeaderStyle>
                    <Columns>
                        <asp:TemplateColumn HeaderText="Doc. Data" HeaderStyle-Width="10%" HeaderStyle-Wrap="true">
                            <HeaderStyle Wrap="True" Width="10%" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" Font-Bold="False" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.docData") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="Textbox2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.docData") %>'>
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Registro" HeaderStyle-Width="5%" HeaderStyle-Wrap="true">
                            <HeaderStyle Wrap="True" Width="5%" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" Font-Bold="False" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="Textbox4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>'>
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Tipo" HeaderStyle-Width="4%" HeaderStyle-Wrap="false">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" Font-Bold="False" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="Textbox9" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>'>
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Oggetto" HeaderStyle-Width="31%" HeaderStyle-Wrap="true">
                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" HorizontalAlign="Justify"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.oggetto")) %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="Textbox5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.oggetto") %>'>
                                </asp:TextBox>
                            </EditItemTemplate>
                            <HeaderStyle Wrap="True" Width="31%" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center"></HeaderStyle>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="mitt/dest" HeaderStyle-Width="20%" HeaderStyle-Wrap="true">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Justify"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# DocsPAWA.Utils.TruncateString_MittDest(DataBinder.Eval(Container, "DataItem.mittdest")) %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.mittdest") %>'>
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="motivo" HeaderStyle-Width="10%" HeaderStyle-Wrap="true">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Justify"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.motivo") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.motivo") %>'>
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn Visible="false">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Justify"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.autore") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.autore") %>'>
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Dett." HeaderStyle-Width="4%" HeaderStyle-Wrap="true">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Font-Bold="False" Font-Italic="False"
                                Font-Overline="False" Font-Strikeout="False" Font-Underline="False"></ItemStyle>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../images/proto/dett_lente.gif"
                                    CommandName="Select" AlternateText="Dettaglio"></asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="VIS" HeaderStyle-Width="4%">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"  Font-Bold="False" Font-Italic="False"
                                Font-Overline="False" Font-Strikeout="False" Font-Underline="False"></ItemStyle>
                            <HeaderTemplate>
                                <asp:Label ID="lbl_Vis" runat="server" CssClass="menu_1_bianco_dg">Vis.</asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="IMG_VIS" runat="server" ImageUrl="../images/proto/dett_lente_doc.gif"
                                    CommandName="VisDoc" ToolTip="Visualizza immagine documento"></asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Attiva" Visible="True" HeaderStyle-Width="4%" HeaderStyle-Wrap="true">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" Font-Bold="False" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></ItemStyle>
                            <ItemTemplate>
                                <asp:ImageButton ID="btn_Ripristina" runat="server" CssClass="testo_grigio" CommandName="Ripristina"
                                    ImageUrl="../images/proto/ico_risposta.gif" AlternateText="Ripristina"></asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn Visible="False" DataField="acquisitaImmagine">
                            <HeaderStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" HorizontalAlign="Center" />
                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" HorizontalAlign="Center" />
                        </asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Elimina" Visible="True" HeaderStyle-Width="2%" HeaderStyle-Wrap="true">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" Font-Bold="False" Font-Italic="False" Font-Overline="False"
                                Font-Strikeout="False" Font-Underline="False"></ItemStyle>
                            <ItemTemplate>
                                <asp:ImageButton ID="btn_Elimina" runat="server" CssClass="testo_grigio" CommandName="Elimina"
                                    ImageUrl="../images/proto/b_elimina.gif" AlternateText="Elimina">
                                </asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </td>
        </tr>
        <tr>
            <td height="20">
            </td>
        </tr>
        <tr height="100%">
            <td valign="bottom" align="center" class="testo_grigio">
                <cc1:ImageButton ID="btnFilterDocs" runat="server" Thema="btn_" SkinID="filtro_small"
                    autopostback="false" Tipologia="" DisabledUrl="~/App_Themes/ImgComuni/btn_FiltroDisabled.gif"
                    AlternateText="Filtra documenti"></cc1:ImageButton>
                <cc1:ImageButton ID="btnShowAllDocs" runat="server" Thema="btn_" SkinID="RimuoviFiltro"
                    autopostback="false" Tipologia="" DisabledUrl="~/App_Themes/ImgComuni/btn_RimuoviFiltroDisabled.gif"
                    AlternateText="Rimuovi filtro sui documenti"></cc1:ImageButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <cc1:ImageButton ID="btnChiudi" runat="server" Thema="btn_" SkinID="chiudi" autopostback="false"
                    Tipologia="" DisabledUrl="~/App_Themes/ImgComuni/btn_chiudi_little_nonAttivo.gif"
                    AlternateText="Esci" OnClick="btnChiudi_Click"></cc1:ImageButton>
            </td>
        </tr>
        <tr>
            <td>
                <cc2:MessageBox ID="Msg_Ripristina" runat="server"></cc2:MessageBox>
                <cc2:MessageBox ID="Msg_SvuotaCestino" runat="server"></cc2:MessageBox>
                <cc2:MessageBox ID="Msg_EliminaDoc" runat="server"></cc2:MessageBox>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
