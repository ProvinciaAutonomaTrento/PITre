<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>

<%@ Page Language="c#" CodeBehind="GestImport.aspx.cs" AutoEventWireup="true" Inherits="DocsPAWA.AdminTool.Gestione_Import.GestImport" %>
<%@ Register Src="../../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc6" %>
<%@ Register Src="../../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper"
    TagPrefix="uc5" %>
<%@ Register Src="../../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper"
    TagPrefix="uc4" %>
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
            hlp = window.open('../help.aspx?from=IMP', '', 'width=450,height=500,scrollbars=YES');
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
        function get_estensione(path) {

            posizione_punto = path.lastIndexOf(".");
            lunghezza_stringa = path.length;
            estensione = path.substring(posizione_punto + 1, lunghezza_stringa);
            return estensione;
        }
        function controlla_estensione(path) {
            if (get_estensione(path) != "xls") {
                alert("Il file deve avere estensione .xls");
                sostituisciInputFile(document.getElementById("uploadFile"));
            }
        }
        function OpenFile(nomefile) {
            var filePath;
            var exportUrl;
            var http;
            var applName;
            var fso;
            var folder;
            var path;

            try {
                fso = FsoWrapper_CreateFsoObject();
                path = fso.GetSpecialFolder(2).Path;
                //Andrea De Marco - Nome del file da visualizzare
                //filePath = path + "\\exportDocspa.xls";
                filePath = path + "\\" + nomefile;
                applName = "Microsoft Excel";
                exportUrl = "ExportDatiPage.aspx";
                http = CreateObject("MSXML2.XMLHTTP");
                http.Open("POST", exportUrl, false);
                http.send();
                var content = http.responseBody;

                if (content != null) {
                    AdoStreamWrapper_SaveBinaryData(filePath, content);

                    ShellWrappers_Execute(filePath);

                    //self.close();
                }
            }
            catch (ex) {
                alert(ex.message.toString());
            }
        }
        // Creazione oggetto activex con gestione errore
        function CreateObject(objectType) {
            try { return new ActiveXObject(objectType); }
            catch (ex) { alert("Oggetto '" + objectType + "' non istanziato"); }
        }
        function sostituisciInputFile(inputFile) {
            return inputFile.parentNode.replaceChild(inputFile.cloneNode(true), inputFile);
        };

    </script>
    <style type="text/css">
        .cbtn
        {
            font-family: Verdana;
            font-size: 11px;
            margin: 0px;
            padding: 0px;
            padding: 2px;
            width: 120px;
            height: 25px;
            color: #ffffff;
            border: 1px solid #ffffff;
            background-image: url('../images/bg_button.gif');
        }
        
        #box_upload p
        {
            margin: 0px;
            padding: 0px;
            text-align: left;
            margin-bottom: 5px;
            margin-left: 20px;
            font-size: 12px;
            font-weight: normal;
            color: #333333;
            border-bottom: 1px dashed #cccccc;
            margin-right: 20px;
            padding-bottom: 5px;
        }
        
        #box_upload h5
        {
            margin: 0px;
            padding: 0px;
            font-size: 12px;
            font-weight: normal;
            color: #333333;
            text-align: right;
            margin-right: 10px;
        }
        
        #box_upload h3
        {
            margin: 0px;
            padding: 0px;
            text-align: center;
            margin-bottom: 10px;
            margin-left: 20px;
            font-size: 12px;
            font-weight: normal;
            color: #333333;
            margin-top: 10px;
        }
        
        .box_import
        {
            width: 820px;
            margin-top: 10px;
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
        
        .btnUpload
        {
            font-family: Verdana;
            font-size: 11px;
        }
        
        .tab_format
        {
            border-collapse: collapse;
            border: 0px;
            width: 100%;
        }
        
        .tab_format td
        {
            padding: 3px;
        }
        
        .head_tab
        {
            background-color: #810D06;
            color: #ffffff;
            font-size: 12px;
            font-weight: normal;
            padding: 3px;
            text-align: left;
        }
        
        .head_tab2
        {
            background-color: #810D06;
            color: #ffffff;
            font-size: 12px;
            font-weight: normal;
            padding: 3px;
            text-align: center;
        }
        
        #pnlAvviso p
        {
            margin: 0px;
            padding: 0px;
            font-size: 12px;
            color: #333333;
            margin-bottom: 5px;
        }
        
        .tab_sx2
        {
            color: #333333;
            font-size: 12px;
            text-align: left;
        }
        
        .tab_sx2 ul
        {
            margin: 0px;
            padding: 0px;
            margin-left: 20px;
        }
        
        .tab_sx2 li
        {
            margin: 0px;
            padding: 0px;
            list-style-type: square;
        }
        
        .tab_sx3
        {
            color: #333333;
            font-size: 12px;
            text-align: center;
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
        
        #box_upload a:link
        {
            color: #333333;
            text-decoration: none;
        }
        
        #box_upload a:visited
        {
            color: #333333;
        }
        
        #box_upload a:hover
        {
            color: #990000;
            text-decoration: underline;
        }
        .inpSfoglia
        {
            width: 370px;
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
<body onunload="ClosePopUp()" style="margin: 0px; padding: 0px;">
    <form id="Form" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000"
        EnablePageMethods="true">
    </asp:ScriptManager>
    <script language="javascript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
    </script>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Import Pregressi" />
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
                Import Pregressi -> Nuovo Import
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
                            <div id="DivSel" style="overflow: auto; height: 551px" class="testo_grigio_scuro">
                                <div class="box_import">
                                    <h2>
                                        Effettua un nuovo import pregressi</h2>
                                    <asp:UpdatePanel ID="box_upload" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <h5>
                                                &raquo; <a href="import_pregressi.xls" title="Scarica il modulo excel">Scarica il modulo
                                                    excel</a> &laquo;</h5>
                                            <p>
                                                Seleziona un file Excel valido:
                                                <input type="file" runat="server" class="inpSfoglia" id="uploadFile" name="uploadFile"
                                                    onchange="controlla_estensione(document.getElementById('uploadFile').value);" />
                                            </p>
                                            <p>
                                                Descrizione:
                                                <asp:TextBox Width="85%" runat="server" ID="descrizione" ToolTip="Inserire una descrizione"></asp:TextBox>
                                            </p>
                                            <h3>
                                                <asp:Button ID="btnImporta" runat="server" OnClick="btn_Importa_Click" Text="Inizia Import"
                                                    CssClass="cbtn" />
                                                <asp:Button ID="btnNuovo" runat="server" OnClick="btn_Nuovo_Click" Text="Nuovo Import"
                                                    CssClass="cbtn" /></h3>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                <asp:UpdatePanel ID="upPnlReport" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlReport" runat="server" Visible="false">
                                            <div class="box_import">
                                                <h2>
                                                    Analisi import</h2>
                                                <asp:Panel ID="pnlAvviso" runat="server" Visible="false">
                                                    <p>
                                                        <asp:Label ID="lbl_alert" runat="server">
                                                        </asp:Label>
                                                    </p>
                                                </asp:Panel>
                                                <div align="center">
                                                    <asp:DataGrid ID="grvItems" runat="server" AllowSorting="false" AutoGenerateColumns="false"
                                                        AllowPaging="False" CssClass="tab_format" OnItemCreated="DataGrid_ItemCreated"
                                                        OnItemDataBound="ImageCreatedRender">
                                                        <AlternatingItemStyle BackColor="#f2f2f2" />
                                                        <ItemStyle BackColor="#d9d9d9" ForeColor="#333333" />
                                                        <Columns>
                                                            <asp:TemplateColumn ItemStyle-Width="5%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab2"
                                                                ItemStyle-HorizontalAlign="center">
                                                                <ItemTemplate>
                                                                    <asp:Image ID="btn_periodo" runat="server" AlternateText='<%# this.GetTipoAvviso((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'
                                                                        ImageUrl='<%# this.GetImageAvviso((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'
                                                                        ToolTip='<%# this.GetTipoAvviso((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'>
                                                                    </asp:Image>
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
                                                            <%--commentato per Richiesta cliente--%>
                                                            <%--<asp:TemplateColumn ItemStyle-Width="15%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab2"
                                                                ItemStyle-CssClass="tab_sx3" HeaderText="TIPO">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="TIPOERRORE" runat="server" Text='<%# this.GetTipoAvviso((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>--%>
                                                            <asp:TemplateColumn ItemStyle-Width="70%" HeaderText="ERRORE" ItemStyle-CssClass="tab_sx2"
                                                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="ERRORE" runat="server" Text='<%# this.GetErrore((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
                                                            <%--Andrea De Marco - Modificato per Richiesta cliente: RIGA diventa ORDINALE--%>
                                                            <%--<asp:TemplateColumn ItemStyle-Width="10%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab2"
                                                                ItemStyle-CssClass="tab_sx3" HeaderText="RIGA">--%>
                                                                <asp:TemplateColumn ItemStyle-Width="10%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab2"
                                                                ItemStyle-CssClass="tab_sx3" HeaderText="ORDINALE">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="RIGAEXCEL" runat="server" Text='<%# this.GetLineaExcel((DocsPAWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
                                                        </Columns>
                                                    </asp:DataGrid>
                                                </div>
                                                <h3>
                                                    <asp:Button ID="btnContinua" runat="server" OnClick="btn_Continua_Click" Text="Continua"
                                                        CssClass="cbtn" />
                                                    <asp:Button ID="btnEsporta" runat="server" OnClick="btn_Esporta_Click" Text="Esporta" 
                                                        CssClass="cbtn"/>
                                                </h3>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </td>
                    </tr>
                </table>
                <!-- FINE CORPO CENTRALE -->
            </td>
        </tr>
    </table>
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
    <uc4:ShellWrapper ID="shellWrapper" runat="server" />
    <uc5:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
    <uc6:FsoWrapper ID="fsoWrapper" runat="server" />
    </form>
</body>
</html>
