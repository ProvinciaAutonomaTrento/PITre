<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MultiDestinatari.aspx.cs" Inherits="DocsPAWA.popup.MultiDestinatari" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
        <base target="_self" />
    <title>Scegli un corrispondente</title>
    <link type="text/css" href="../CSS/docspa_30.css" rel="Stylesheet" />
    <link href="../CSS/StyleSheet.css" type="text/css" rel="Stylesheet" />
    <style type="text/css">
        body
        {
            background-color: #eaeaea;
            font-family: Verdana;
        }
        .head_tab
        {
            height: 20px;
        }
        #cont_pref
        {
            float: left;
            width: 100%;
            background-color: #fafafa;
            height: 350px;
            overflow-y: scroll;
        }
        .tab_sx
        {
            text-align: left;
            padding-left: 5px;
            font-size: 10px;
            color:#333333;
        }
        .tab_dx
        {
            padding-left: 5px;
            font-size: 10px;
            color:#333333;
        }
        #button
        {
            text-align: center;
            margin-left: 15px;
            margin-right: 15px;
            padding-top: 15px;
            margin-bottom:10px;
        }
        #box_preferred_grids
        {
            margin: 5px;
            padding-left: 5px;
            padding-top: 5px;
            background-color:#ffffff;
            border:1px solid #cccccc;
            padding-right:5px;
        }
        #topGrid
        {
            text-align: center;
            width: 100%;
            float: left;
            border-bottom:5px solid #ffffff;
            padding-right:5px;
        }
        .title
        {
            margin: 0px;
            padding: 0px;
            font-size: 11px;
            font-weight: bold;
            text-align: center;
            width: 100%;
            float: left;
            padding-top: 4px;
            padding-bottom: 4px;
        }
    </style>
</head>
<body>
    <form id="chooseCorr" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
     <div id="cont_pref">
        <asp:UpdatePanel ID="box_preferred_grids" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="topGrid">
                    <asp:Label ID="titlePage" runat="server" Text="Stesso codice per più corrispondenti. Seleziona un corrispondente" CssClass="title"></asp:Label>
                </div>
                <asp:DataGrid ID="grvListaCorr" runat="server" AllowSorting="True" AutoGenerateColumns="false"
                    Width="96%" SkinID="datagrid" AllowPaging="False" OnPreRender="grvListaCorr_PreRender" 
                    AllowCustomPaging="false" PageSize="10" BorderStyle="Solid" BorderWidth="1" BorderColor="#cccccc">
                    <AlternatingItemStyle BackColor="White" />
                    <ItemStyle BackColor="#f0f0f0" ForeColor="#333333" Font-Size="Small" />
                    <Columns>
                        <asp:TemplateColumn ItemStyle-Width="5%" HeaderText="" ItemStyle-HorizontalAlign="center"
                            HeaderStyle-HorizontalAlign="center">
                            <ItemTemplate>
                                <input name="rbl_pref" type="radio" class="testo_grigio_scuro" value='<%# this.GetCorrID((DocsPAWA.DocsPaWR.ElementoRubrica)Container.DataItem) %>'>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn ItemStyle-Width="55%" HeaderText="Descrizione" ItemStyle-CssClass="tab_sx"
                            HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab">
                            <ItemTemplate>
                                <asp:Label ID="gridName" runat="server" Text='<%# this.GetCorrName((DocsPAWA.DocsPaWR.ElementoRubrica)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>

                        <asp:TemplateColumn ItemStyle-Width="16%" HeaderText="Codice" ItemStyle-HorizontalAlign="center"
                            HeaderStyle-HorizontalAlign="center"  ItemStyle-CssClass="tab_dx">
                            <ItemTemplate>
                            <asp:Label ID="gridCodice" runat="server" Text='<%# this.GetCorrCodice((DocsPAWA.DocsPaWR.ElementoRubrica)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn ItemStyle-Width="14%" HeaderText="Visibilità" ItemStyle-HorizontalAlign="center"
                            HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="tab_dx">
                            <ItemTemplate>
                            <asp:Label ID="gridTipo" runat="server" Text='<%# this.GetCorrTipo((DocsPAWA.DocsPaWR.ElementoRubrica)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>  
                        <asp:TemplateColumn Visible="false" ItemStyle-Width="10%" HeaderText="Storicizzato" ItemStyle-HorizontalAlign="center"
                            HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="tab_dx">
                            <ItemTemplate>
                            <asp:Label ID="gridSto" runat="server" Text='<%# this.GetStoricizzato((DocsPAWA.DocsPaWR.ElementoRubrica)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>

                    </Columns>
                </asp:DataGrid>
                <div id="button" style="margin-top:10px;width:96%;">
                    <asp:Button ID="btn_salva" runat="server" CssClass="pulsante_mini_3" Text="Scegli"
                        OnClick="BtnSaveCorr_Click"/>
                    &nbsp;
                    <asp:Button ID="btn_chiudi" runat="server" CssClass="pulsante_mini_3" Text="Annulla"
                        OnClientClick="window.close();" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
