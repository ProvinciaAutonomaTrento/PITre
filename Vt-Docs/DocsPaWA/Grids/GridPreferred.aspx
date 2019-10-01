<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridPreferred.aspx.cs"
    Inherits="DocsPAWA.Grids.GridPreferred" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <title>Le mie grigle preferite</title>
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
            height: 450px;
            overflow-y: scroll;
        }
        .tab_sx
        {
            text-align: left;
            padding-left: 5px;
            font-size: 11px;
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
    <script type="text/javascript" language="javascript">
        function close_and_save() {
            var ret = document.getElementById("hid_tab_est").value;
            window.returnValue = ret;
            window.close();
        }
        function setFocus() {
            document.getElementById("btn_salva").focus();
        }
    </script>
</head>
<body style="background-color: #ffffff;">
    <form id="frmPreferredGrid" runat="server">
    <asp:HiddenField ID="hid_tab_est" runat="server" />
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <div id="cont_pref">
        <asp:UpdatePanel ID="box_preferred_grids" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="topGrid">
                    <asp:Label ID="titlePage" runat="server" Text="LE MIE GRIGLIE PREFERITE" CssClass="title"></asp:Label>
                </div>
                <asp:DataGrid ID="grvPreferred" runat="server" AllowSorting="True" AutoGenerateColumns="false"
                    Width="100%" SkinID="datagrid" AllowPaging="True" OnItemCreated="DataGrid_ItemCreated" 
                    AllowCustomPaging="false" PageSize="10" BorderStyle="Solid" BorderWidth="1" BorderColor="#cccccc"  OnPageIndexChanged="SelectedIndexChanged">
                    <AlternatingItemStyle BackColor="White" />
                    <ItemStyle BackColor="#f0f0f0" ForeColor="#333333" Font-Size="Small" />
                    <Columns>
                        <asp:TemplateColumn Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetGridID((DocsPAWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn ItemStyle-Width="71%" HeaderText="NOME GRIGLIA" ItemStyle-CssClass="tab_sx"
                            HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab">
                            <ItemTemplate>
                                <asp:Label ID="gridName" runat="server" Text='<%# this.GetGridName((DocsPAWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn ItemStyle-Width="8%" HeaderText="PRED." ItemStyle-HorizontalAlign="center"
                            HeaderStyle-HorizontalAlign="center">
                            <ItemTemplate>
                                <input name="rbl_pref" type="radio" class="testo_grigio_scuro" value='<%# this.GetGridID((DocsPAWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>'
                                    <%# this.GetGridPreferred((DocsPAWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn ItemStyle-Width="8%" HeaderText="Utente" ItemStyle-HorizontalAlign="center"
                            HeaderStyle-HorizontalAlign="center">
                            <ItemTemplate>
                                <image src='<%# this.GetImageUserGridID((DocsPAWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>'></image>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn ItemStyle-Width="8%" HeaderText="Ruolo" ItemStyle-HorizontalAlign="center"
                            HeaderStyle-HorizontalAlign="center">
                            <ItemTemplate>
                                <image src='<%# this.GetImageRoleGridID((DocsPAWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>'></image>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn ItemStyle-Width="5%" ItemStyle-HorizontalAlign="center">
                            <ItemTemplate>
                                <asp:ImageButton ID="btn_Rimuovi" runat="server" CssClass="testo_grigio" CommandName="Rimuovi"
                                    AlternateText="Rimuovi" ImageUrl="../images/ricerca/cancella_griglia.gif" Visible='<%# this.GetDeleteGridID((DocsPAWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>'
                                    OnClick="DeleteGrid" ToolTip="Rimuovi"></asp:ImageButton>
                                <cc3:ConfirmButtonExtender ID="bDeleteExtender" runat="server" TargetControlID="btn_Rimuovi"
                                    Enabled='<%# this.GetRoleGridID((DocsPAWA.DocsPaWR.GridBaseInfo)Container.DataItem) %>'
                                    ConfirmText="Attenzione! Questa griglia è visibile a tutto il ruolo. Sei sicuro di volerla cancellare?" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                    <PagerStyle HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
                        Mode="NumericPages" Position="TopAndBottom"></PagerStyle>
                </asp:DataGrid>
                <div id="button">
                    <asp:Button ID="btn_salva" runat="server" CssClass="pulsante_mini_3" Text="Salva"
                        OnClick="BtnSavePrefGrid_Click"/>
                    &nbsp;
                    <asp:Button ID="btn_chiudi" runat="server" CssClass="pulsante_mini_3" Text="Chiudi"
                        OnClientClick="window.close();" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
