<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewTabSearchResult.aspx.cs"
    Inherits="DocsPAWA.ricercaFascicoli.NewTabSearchResult" %>

    <%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="../UserControls/NewMassiveOperationButtons.ascx" TagName="MassiveOperationButtons"
    TagPrefix="uc1" %>
<%@ Register Src="../UserControls/GridPersonalizationButton.ascx" TagName="GridPersonalizationButton"
    TagPrefix="uc6" %>
<%@ Register Src="../UserControls/NewSearchIcons.ascx" TagName="SearchIcons" TagPrefix="uc2" %>
<%@ Register Src="~/UserControls/AjaxMessageBox.ascx" TagName="MessageBox" TagPrefix="uc3" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc5" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../CSS/docspA_30.css" rel="Stylesheet" />
    <link href="../CSS/StyleSheet.css" type="text/css" rel="Stylesheet" />
    <style type="text/css">
        gridPersonalization
        {
            float: right;
        }
        .rosso
        {
            color: #990000;
        }
        .modalBackground
        {
            background-color: #f0f0f0;
            filter: alpha(opacity=50);
            opacity: 0.5;
        }
        .modalPopup
        {
            background-color: #ffffdd;
            border-width: 3px;
            border-style: solid;
            border-color: Gray;
            padding: 3px;
            text-align: center;
        }
        .header_column a:link
        {
            color: #ffffff;
            text-decoration: underline;
            font-family: Verdana;
        }
        .header_column a:visited
        {
            color: #ffffff;
            text-decoration: underline;
            font-family: Verdana;
        }
        .header_column a:hover
        {
            color: #cccccc;
            text-decoration: underline;
            font-family: Verdana;
        }
        .header_column
        {
            color:#ffffff;
            font-weight:bold;
            text-transform:uppercase;
            font-size:10px;
            font-family: Verdana;
        }
        .text_search
        {
            font-family: Verdana;
            font-size:10px;
            color:#333333;
            font-weight:bold;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function beginRequest(sender, args) {
            $find("mdlWait").show();
        }

        function endRequest(sender, args) {
            $find("mdlWait").hide();
        }

    </script>
</head>
<body>
    <form id="frmNewTabSearchResult" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <script language="javascript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
    </script>
    <div id="divTitle" class="pulsanti" style="float: left; width: 100%;">
        <asp:UpdatePanel ID="upGridPersonalization" runat="server">
            <ContentTemplate>
                <div style="float: left; width: 100%;">
                    <div style="float: left;">
                        <asp:Label ID="lblArea" runat="server" Text="Risultati ricerca" CssClass="titolo_real"
                            Font-Bold="true" />
                        <asp:Label ID="lblTitle" runat="server" CssClass="titolo_real" />
                    </div>
                     <div style="float: right;">
                        <cc1:imagebutton ID="btnSalvaGrid" runat="server" Visible="true" AlternateText="Salva o modifica la griglia"
                            ToolTip="Salva o modifica la griglia" ImageUrl="~/images/ricerca/ico_salva_griglia.gif"
                            DisabledUrl="~/images/ricerca/ico_salva_griglia_disable.gif" />
                        <cc1:imagebutton ID="btnCustomizeGrid" runat="server" Visible="true" AlternateText="Personalizza griglia"
                            ToolTip="Personalizza la griglia di ricerca" ImageUrl="~/images/ricerca/ico_doc2.gif" />
                        <cc1:imagebutton ID="btnPreferredGrid" runat="server" Visible="true" AlternateText="Mie griglie preferite"
                            ToolTip="Mie griglie preferite" ImageUrl="~/images/ricerca/ico_griglie_preferite.gif" />
                     <!--   <cc1:imagebutton ID="btnRestorePreferredGrid" runat="server" Visible="true" AlternateText="Mie griglie preferite"
                            ToolTip="Ritorna alla griglia predefinita" ImageUrl="~/images/ricerca/ico_doc3.gif" DisabledUrl="~/images/ricerca/ico_doc3_disabled.gif"/>-->
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="divButtons" style="clear: both;">
        <asp:UpdatePanel ID="upButtons" runat="server">
            <ContentTemplate>
                <uc1:MassiveOperationButtons ID="mobButtons" runat="server" DataGridId="dgResult"
                    MessageBoxID="mbAlert" ObjType="P" GridType="Project" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="divResult">
        <asp:UpdatePanel ID="upResult" runat="server">
            <ContentTemplate>
                <asp:DataGrid ID="dgResult" runat="server" BorderWidth="1px" Width="100%" AllowSorting='<%# this.GetGridPersonalization() %>'
                    BorderStyle="Inset" AutoGenerateColumns="false" CellPadding="1" BorderColor="Gray"
                    HorizontalAlign="Center" AllowPaging="true" PageSize='<%# this.GetPageSize() %>' AllowCustomPaging="true"
                    ShowHeader="true" OnPageIndexChanged="dgResult_SelectedPageIndexChanged" OnItemCreated="dgResult_ItemCreated" OnSortCommand="Sort_Grid">
                    <AlternatingItemStyle BackColor="#fafafa" />
                    <ItemStyle BackColor="#eaeaea" CssClass="testo_grigio_scuro" ForeColor="#333333"
                        Height="20px" VerticalAlign="Middle" HorizontalAlign="Center" />
                    <HeaderStyle Wrap="False" Height="20px" CssClass="header_column"
                        HorizontalAlign="Center" VerticalAlign="Middle" />
                    <PagerStyle HorizontalAlign="Center" Position="TopAndBottom" BackColor="#C2C2C2"
                        CssClass="menu_pager_grigio" Mode="NumericPages" />
                    <SelectedItemStyle ForeColor="White" BackColor="Gray" />
                    <Columns>
                    </Columns>
                </asp:DataGrid>
            </ContentTemplate>
        </asp:UpdatePanel>
        <uc3:MessageBox ID="mbAlert" runat="server" />
    </div>
     <!-- PopUp Wait-->
    <uc5:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
        BackgroundCssClass="modalBackground" BehaviorID="mdlWait" />
        <div id="Wait" runat="server" style="display: none; font-weight: normal; font-size: 13px;
        font-family: Arial; text-align: center;">
        <asp:UpdatePanel ID="pnlUP" runat="server">
            <contenttemplate>
                <div class="modalPopup">
                    <asp:Label ID="lblInfo" runat="server">Attendere prego...</asp:Label>
                    <br />
                    <img id="imgLoading" src="../images/loading.gif" style="border-width: 0px;" alt="Attendere prego" />
                </div>
            </contenttemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
