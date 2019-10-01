<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RicercaOggettoEsterno.aspx.cs" Inherits="DocsPAWA.popup.RicercaOggettoEsterno" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<HTML>
	<HEAD id="HEAD1" runat="server">
		<META HTTP-EQUIV="Pragma" CONTENT="no-cache">
        <META HTTP-EQUIV="Expires" CONTENT="-1">	
	    <base target="_self" />
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/ETcalendar.js"></script>
		<script language="javascript">
		    function ChiudiFinestra() {
		        window.close();

		    }

		    function SingleSelect(regex, current) {
		        re = new RegExp(regex);
		        var elems = document.getElementsByName(regex);
                var index=0;
		        for (i = 0; i < elems.length; i++) {
		            elm = elems[i];
		            if (elm != current) {
		                elm.checked = false;
		            }else{
                        index = i;
                    }
                }
                window.document.getElementById('<%=this.hf_checkedIndex.ClientID %>').value = index;
                window.document.getElementById('<%=this.btn_ok.ClientID %>').disabled = false;
		    }
		</script>
	</head>
	<body  bottommargin="3" leftmargin="3" topmargin="3" rightmargin="3">
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Ricerca oggetti esterni" />
			<table class="contenitore" height="100%" width="100%" align="center" border="0">
                <tr valign="middle">
                    <td class="menu_1_rosso" align="center">Ricerca Oggetti Esterni</asp:Label></td>
                </tr>
                <!--FORM DI RICERCA-->
                <tr valign="top">
                <td valign="top">
                <table class="info_grigio" cellspacing="3" cellpadding="0" width="95%" align="center" border="0">
                    <tr>
                        <td>
                            <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                <tr>
                                    <td>
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="lbl_codice" runat="server" CssClass="titolo_scheda"/>
                                    </td>
                                    <td>
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:TextBox ID="txt_codice" runat="server" CssClass="testo_grigio" BackColor="White" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="lbl_descr" runat="server" CssClass="titolo_scheda"/>
                                    </td>
                                    <td>
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:TextBox ID="txt_descr" runat="server" CssClass="testo_grigio" BackColor="White" />
                                    </td>
                                </tr>
                             </table>
                          </td>
                      </tr>
                      <tr>
                        <td align="center">
                            <asp:Button ID="btn_ric" runat="server" Width="55px" CssClass="pulsante" Height="19px"
                                Text="CERCA"></asp:Button>
                        </td>
                    </tr>
                 </table>
             </td>
             </tr>
             <!--FINE FORM DI RICERCA-->
             <!--RISULTATI-->
             <tr valign="top" align="center">
               <td class="countRecord" valign="middle" align="center" width="100%" height="20">
                <asp:Label ID="lbl_noResult" runat="server" CssClass="titolo_rosso" Visible="False">Nessun risultato</asp:Label>
               </td>
            </tr>
             <tr valign="top" align="center">
             <td valign="top">
                <asp:HiddenField ID="hf_checkedIndex" runat="server" />
                <div id="DivDataGrid" style="overflow: auto; width: 580px; height: 280px">
                    <asp:DataGrid ID="dg_OggEst" SkinID="datagrid" runat="server" Width="97%" AllowPaging="true"
                        AllowCustomPaging="true" HorizontalAlign="Center" BorderColor="Gray" BorderWidth="1px"
                        CellPadding="1" AutoGenerateColumns="False" BorderStyle="Inset" Visible="false">
                        <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                        <ItemStyle HorizontalAlign="Center" Height="20px" CssClass="bg_grigioN" VerticalAlign="Middle">
                        </ItemStyle>
                        <HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg">
                        </HeaderStyle>
                        <Columns>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="5%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <input type="radio" onclick="SingleSelect('rbSel',this)" name="rbSel" /><br />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="20%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>'
                                        ID="Label6">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="45%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left" Width="150px"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>'
                                        ID="Label7">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                        <PagerStyle HorizontalAlign="Center" Position="TopAndBottom" BackColor="#C2C2C2" CssClass="menu_pager_grigio" 
                            Mode="NumericPages"></PagerStyle>
                    </asp:DataGrid></div>
            </td>
        </tr>
             <!--FINE RISULTATI-->
             <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btn_ok" runat="server" CssClass="pulsante" Text="OK" Enabled="false"></asp:Button>
                        </td>
                        <td>
                            <asp:Button ID="btn_chiudi" runat="server" CssClass="pulsante" Text="CHIUDI" OnClientClick="ChiudiFinestra()"></asp:Button>
                        </td>
                    </tr>
                </table>
            </td>
            </tr>
			</table>
		</form>
	</body>
</html>
