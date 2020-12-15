<%@ Page Language="c#" CodeBehind="sceltaTipoSpedizione.aspx.cs" AutoEventWireup="false"
    Inherits="DocsPAWA.popup.sceltaTipoSpedizione" %>

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
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <base target="_self" />
    <meta http-equiv="PRAGMA" content="NO-CACHE">
</head>
<body ms_positioning="GridLayout">
    <form id="sceltaTipoSpedizione" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Scelta Spedizione" />
    <div style="padding-left: 3px; padding-top: 2px;">
        <asp:Label ID="lblTutti" Text="Seleziona mezzo di spedizione per tutti" CssClass="testo_grigio_scuro"
            runat="server"></asp:Label>
        <asp:DropDownList ID="ddlTutti" CssClass="menu_pager_grigio" runat="server">
        </asp:DropDownList>
    </div>
    <br />
    <div align="center" style="position:absolute;top:30;bottom:40;left:0;width:100%;height:66%;" id="div1" runat="server">
        <div id="DivDataGrid" style="overflow: auto; padding-left: 3px;">
            <asp:DataGrid ID="dataGridMezzi" runat="server" SkinID="datagrid" AutoGenerateColumns="False"
                BorderWidth="1px" AllowPaging="False" CellPadding="1" BorderColor="Gray" OnPageIndexChanged="dataGridMezzi_SelectedPageIndexChanged"
                Width="95%" AllowCustomPaging="false">
                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                <HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
                <Columns>
                    <asp:TemplateColumn HeaderText="Destinatario" Visible="true">
                        <HeaderStyle></HeaderStyle>
                        <ItemStyle HorizontalAlign="Left"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label ID="lblDescrizione" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>'
                                CssClass="testo_grigio_scuro">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Id" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblId" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Id") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Mezzo di spedizione">
                        <HeaderStyle></HeaderStyle>
                        <ItemStyle HorizontalAlign="Left"></ItemStyle>
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlM" CssClass="menu_pager_grigio" runat="server">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
                <PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
                    Mode="NumericPages"></PagerStyle>
            </asp:DataGrid>
        </div>
    </div>
    <br />
    <div style="text-align: center; position:absolute;bottom:1%;left:0;width:100%;">
        <asp:Button ID="btn_ok" runat="server" CssClass="PULSANTE" Text="OK"></asp:Button>&nbsp;
        <asp:Button ID="btn_chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI"></asp:Button>
        <br />
        <br />
    </div>
    </form>
</body>
</html>
