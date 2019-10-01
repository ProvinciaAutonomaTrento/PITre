<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportPregressi.aspx.cs"
    Inherits="SAAdminTool.AdminTool.Gestione_Import.ReportPregressi" %>

<%@ Register TagPrefix="uc3" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuImportPregressi" Src="~/AdminTool/UserControl/MenuImportPregressi.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="HEAD1" runat="server">
    <title>Import Pregressi</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <script language="JavaScript" type="text/javascript">

        var cambiapass;
        var hlp;

        function apriPopup() {
            hlp = window.open('../help.aspx?from=CON', '', 'width=450,height=500,scrollbars=YES');
        }
        function cambiaPwd() {
            cambiapass = window.open('../CambiaPwd.aspx', '', 'width=410,height=300,scrollbars=NO');
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
        function Refresh() {
            window.document.getElementById('<%= Page.Form.ClientID %>').submit();
        }

        var _url_view_import = '<%=UrlViewImport %>';

        function OpenImpotDetails(id) {
            var newLeft = (screen.availWidth / 2 - 225);
            var newTop = (screen.availHeight - 704);
            var retval = window.showModalDialog(_url_view_import + "?id=" + id, 'OpenImpotDetails', 'dialogWidth:950px;dialogHeight:600px;status:no;resizable:no;scroll:yes;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
            if (retval != null) {
                window.document.getElementById('<%= Page.Form.ClientID %>').submit();
            }
        }
    </script>
    <script type="text/javascript" language="javascript">

        function beginRequest(sender, args) {
            $find("mdlWait").show();
        }

        function endRequest(sender, args) {
            $find("mdlWait").hide();
        }
    </script>
    <style type="text/css">
        .box_import
        {
            width: 820px;

            border: 1px solid #cccccc;
            background-color: #fdfdfd;
        }
        
        .box_import h2
        {
            margin: 0px;
            padding: 0px;
            background-color: #810D06;
            color: #ffffff;
            font-size: 12px;
            font-weight: normal;
            padding: 3px;
            text-align: left;
            margin-bottom: 5px;
        }
        
        .head_tab
        {
            background-color: #810D06;
            color: #ffffff;
            font-size: 12px;
            font-weight: normal;
            padding: 3px;
            text-align: center;
        }
        
        .tab_sx3
        {
            text-align: center;
            color: #333333;
            font-size: 11px;
            font-weight: normal;
        }
        
        .tab_format
        {
            border-collapse: collapse;
            border: 0px;
            padding: 0px;
            width: 100%;
        }
        
        .tab_format td
        {
            padding: 3px;
        }
        
        .modalPopup
        {
            position: absolute;
            top: 50%;
            left: 50%;
            width: XX;
            height: YY;
            margin-left: -XXX;
            margin-top: -YYY; /*meno la meta` di XX e YY */
        }
        
        .refresh
        {
            width: 820px;
            background-color: #ffffff;
            border-top: 1px solid #cccccc;
            border-left: 1px solid #cccccc;
            border-right: 1px solid #cccccc;
                        margin-top: 10px;
        }
    </style>
</head>
<body onunload="ClosePopUp()" style="margin: 0px; padding: 0px;">
    <form id="Form" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <script language="javascript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
    </script>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Import Pregressi > Report" />
    <!-- Gestione del menu a tendina -->
    <uc3:MenuTendina ID="MenuTendina" runat="server"></uc3:MenuTendina>
    <table height="100%" cellspacing="1" cellpadding="0" width="100%" border="0">
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
            <!-- STRISCIA DEL TITOLO DELLA PAGINA -->
            <td class="titolo" align="center" bgcolor="#e0e0e0" height="20">
                Import Pregressi -> Stato Import
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
                <table height="100" cellspacing="0" cellpadding="0" width="100%" border="0">
                    <tr>
                        <td width="120" height="100%">
                            <uc2:MenuImportPregressi ID="MenuImportPregressi" runat="server"></uc2:MenuImportPregressi>
                        </td>
                        <td width="100%" height="100%" valign="middle" align="center">
                            <table height="100" cellspacing="0" cellpadding="0" width="100%" border="0">
                                <tr>
                                    <td width="100%" height="100%" valign="top" align="center">
                                     <!--bottone Refresh-->
                                            <div class="refresh">
                                                <asp:UpdatePanel ID="refresh_pnl" runat="server">
                                                    <ContentTemplate>
                                                        <div style="float: left; width: 40px;">
                                                            <asp:ImageButton ID="btn_refresh" runat="server" CssClass="testo_grigio" CommandName="Aggiorna"
                                                                ImageUrl="../../images/ricerca/remove_search_filter.gif" ToolTip="Aggiorna Stato Import"
                                                                OnClick="Refresh" Visible="true" onmouseover="this.src='../../images/ricerca/remove_search_filter_up.gif'"
                                                                onmouseout="this.src='../../images/ricerca/remove_search_filter.gif'" BackColor="White">
                                                            </asp:ImageButton>
                                                        </div>
                                                        <div style="float: left;">
                                                            <asp:Label ID="Label1" runat="server" Font-Size="X-Small" Font-Names="Verdana" ForeColor="#333333">Ricarica lo Stato degli Import</asp:Label>
                                                        </div>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                            <!--End Refresh-->
                                        <div id="DivSel" style="height: 551px" class="testo_grigio_scuro">
                                           
                                            <div class="box_import">
                                                <asp:UpdatePanel ID="box_upload" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:DataGrid ID="grvItems" runat="server" AllowSorting="false" AutoGenerateColumns="false"
                                                            AllowPaging="False" CssClass="tab_format" OnItemCreated="DataGrid_ItemCreated"
                                                            OnItemDataBound="ImageCreatedRender">
                                                            <AlternatingItemStyle BackColor="#f2f2f2" />
                                                            <ItemStyle BackColor="#d9d9d9" ForeColor="#333333" />
                                                            <Columns>
                                                                <asp:TemplateColumn Visible="false">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetReportID((SAAdminTool.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn ItemStyle-Width="15%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                                                                    ItemStyle-CssClass="tab_sx3" HeaderText="DESCRIZIONE">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="DESCR" runat="server" Text='<%# this.GetDescrizione((SAAdminTool.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn ItemStyle-Width="15%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                                                                    ItemStyle-CssClass="tab_sx3" HeaderText="DATA INIZIO">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="DATAINIZIO" runat="server" Text='<%# this.GetDataInizio((SAAdminTool.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn ItemStyle-Width="15%" HeaderText="DATA FINE" ItemStyle-CssClass="tab_sx3"
                                                                    HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="DATAFINE" runat="server" Text='<%# this.GetDataFine((SAAdminTool.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn ItemStyle-Width="14%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                                                                    ItemStyle-CssClass="tab_sx3" HeaderText="N° DOCUMENTI">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="NDOC" runat="server" Text='<%# this.GetNumeroDocumenti((SAAdminTool.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn ItemStyle-Width="14%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                                                                    ItemStyle-CssClass="tab_sx3" HeaderText="PERCENTUALE">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="PERC" runat="server" Text='<%# this.GetPercentuale((SAAdminTool.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn ItemStyle-Width="20%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                                                                    ItemStyle-CssClass="tab_sx3" HeaderText="STATO">
                                                                    <ItemTemplate>
                                                                        <asp:Image runat="server" ID="ImgPerc" ImageUrl='<%# this.GetImmaginePercentuale((SAAdminTool.DocsPaWR.ReportPregressi)Container.DataItem) %>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn ItemStyle-Width="8%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                                                                    ItemStyle-CssClass="tab_sx3" HeaderText="REPORT">
                                                                    <ItemTemplate>
                                                                        <asp:ImageButton ID="btn_dettagli" runat="server" CssClass="testo_grigio" CommandName="Dettaglio"
                                                                            AlternateText="Vedi il dettaglio" ImageUrl="../../images/proto/dett_lente_doc.gif"
                                                                            ToolTip="Vedi il dettaglio" OnClick="ViewDetails" Visible="false"></asp:ImageButton>
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn ItemStyle-Width="8%" HeaderText="ELIMINA" ItemStyle-CssClass="tab_sx3"
                                                                    HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                                                    <ItemTemplate>
                                                                        <asp:ImageButton ID="btn_Rimuovi" runat="server" CssClass="testo_grigio" CommandName="Rimuovi"
                                                                            AlternateText="Rimuovi" ImageUrl="../../images/ricerca/cancella_griglia.gif"
                                                                            ToolTip="Rimuovi" OnClick="DeleteReport" Visible="false"></asp:ImageButton>
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                                <asp:TemplateColumn ItemStyle-Width="8%" ItemStyle-CssClass="tab_sx3"
                                                                    HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                                                    <ItemTemplate>
                                                                        <asp:Image ID="img_errore" runat="server" CssClass="testo_grigio" CommandName="Errore"
                                                                            AlternateText="Errore" ImageUrl='<%# this.GetImageErrore((SAAdminTool.DocsPaWR.ReportPregressi)Container.DataItem) %>'
                                                                            ToolTip='<%# this.GetNumeroDiErrori((SAAdminTool.DocsPaWR.ReportPregressi)Container.DataItem) %>' Visible="false"></asp:Image>
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>
                                                            </Columns>
                                                        </asp:DataGrid>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <!-- FINE CORPO CENTRALE -->
    </td> </tr> </table>
    <!-- PopUp Wait-->
    <cc2:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
        BackgroundCssClass="modalBackground" BehaviorID="mdlWait" />
    <div id="Wait" runat="server" style="display: none; font-weight: normal; font-size: 13px;
        font-family: Arial; text-align: center;">
        <asp:UpdatePanel ID="pnlUP" runat="server">
            <ContentTemplate>
                <div class="modalPopup">
                    <asp:Label ID="lblInfo" runat="server">Attendere prego...</asp:Label>
                    <br />
                    <img id="imgLoading" src="../images/loading.gif" style="border-width: 0px;" alt="Attendere prego" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
