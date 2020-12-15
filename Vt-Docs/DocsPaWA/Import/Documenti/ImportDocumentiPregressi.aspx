<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportDocumentiPregressi.aspx.cs"
    Inherits="DocsPAWA.Import.Documenti.ImportDocumentiPregressi" %>
<%@ Register Src="../../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc6" %>
<%@ Register Src="../../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper"
    TagPrefix="uc5" %>
<%@ Register Src="../../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper"
    TagPrefix="uc4" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self">
    <title>Import Documenti Pregressi</title>
    <link type="text/css" href="../../CSS/docspa_30.css" rel="Stylesheet" />
    <link href="../../CSS/StyleSheet.css" type="text/css" rel="Stylesheet" />
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
        .cont_pref
        {
            float: left;
            width: 100%;
            background-color: #fafafa;
            height: 500px;
            overflow-y: scroll;
        }
        .tab_sx
        {
            text-align: left;
            padding-left: 5px;
            font-size: 11px;
            color: #333333;
        }
        #button
        {
            float: left;
            text-align: center;
            padding-top: 15px;
        }
        #box_upload
        {
            margin: 0px;
            margin-left: 5px;
            margin-right: 5px;
            margin-bottom: 5px;
            padding-left: 5px;
            padding-top: 5px;
            background-color: #ffffff;
            border: 1px solid #cccccc;
            padding-right: 5px;
            float: left;
            width: 97%;
        }
        .topGrid
        {
            text-align: center;
            width: 100%;
            float: left;
            border-bottom: 5px solid #ffffff;
            padding-right: 5px;
        }
        .title
        {
            margin: 0px;
            padding: 0px;
            font-size: 11px;
            font-weight: bold;
            text-align: left;
            width: 99%;
            float: left;
            padding-top: 4px;
            padding-bottom: 4px;
            padding-left: 4px;
            padding-right: 4px;
        }
        #box_upload p
        {
            float: left;
            width: 75%;
            font-size: 12px;
        }
        #box_upload h5
        {
            float: right;
            width: 25%;
            font-size: 11px;
        }
        .nero
        {
            color: #333333;
        }
        .cont_pref h3
        {
            margin: 0px;
            padding: 0px;
            text-align: center;
            clear: both;
            margin-bottom: 10px;
        }
        .pulsante_mini_4
        {
            cursor: pointer;
        }
        .pulsante_mini_3
        {
            cursor: pointer;
        }
        #pnlReport
        {
            margin: 5px;
            padding-left: 5px;
            padding-top: 5px;
            background-color: #ffffff;
            border: 1px solid #cccccc;
            padding-right: 5px;
            float: left;
            width: 97%;
        }
        #pnlAvviso p
        {
            margin: 0px;
            padding: 0px;
            font-size: 12px;
            color: #333333;
            margin-bottom: 5px;
            text-align: center;
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
            margin-left: 15px;
            text-align:left;
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
            font-size: 12px;
            font-weight: normal;
            padding: 3px;
            text-align: center;
        }
        
        .head_tab2
        {
            font-size: 12px;
            font-weight: normal;
            padding: 3px;
            text-align: center;
        }
        
        #menu
        {
            margin: 0px;
            padding: 0px;
            float: left;
        }
        
        #menu ul
        {
            margin: 0px;
            padding: 0px;
            margin-left: 5px;
            margin-top: 5px;
        }
        #menu li
        {
            margin: 0px;
            padding: 0px;
            list-style-type: none;
            display: inline;
            color: #333333;
            font-size: 13px;
        }
       /* 
        #menu a:link
        {
            color: #333333;
            text-decoration: none;
            float: left;
            width: 95px;
            height: 25px;
            line-height: 25px;
            padding-left: 5px;
            background-image: url('../../images/bg_imp_off.png');
        }
        
        #menu a:visited
        {
            color: #333333;
            text-decoration: none;
            float: left;
            width: 95px;
            height: 25px;
            line-height: 25px;
            padding-left: 5px;
            background-image: url('../../images/bg_imp_off.png');
        }
        
        #menu a:hover
        {
            color: #666666;
            text-decoration: none;
            float: left;
            width: 95px;
            height: 25px;
            line-height: 25px;
            padding-left: 5px;
            background-image: url('../../images/bg_imp_on.png');
        }
        */
        .noClick a:link
        {
            color: #333333;
            text-decoration: none;
            float: left;
            width: 95px;
            height: 25px;
            line-height: 25px;
            padding-left: 5px;
            background-image: url('../../images/bg_imp_off.png');
        }
        
        .noClick a:visited
        {
            color: #333333;
            text-decoration: none;
            float: left;
            width: 95px;
            height: 25px;
            line-height: 25px;
            padding-left: 5px;
            background-image: url('../../images/bg_imp_off.png');
        }
        
        .noClick a:hover
        {
            color: #666666;
            text-decoration: none;
            float: left;
            width: 95px;
            height: 25px;
            line-height: 25px;
            padding-left: 5px;
            background-image: url('../../images/bg_imp_on.png');
        }
        
        .Click a:link
        {
            color: #666666;
            text-decoration: none;
            float: left;
            width: 95px;
            height: 25px;
            line-height: 25px;
            padding-left: 5px;
            background-image: url('../../images/bg_imp_on.png');
        }
        
        .Click a:visited
        {
            color: #666666;
            text-decoration: none;
            float: left;
            width: 95px;
            height: 25px;
            line-height: 25px;
            padding-left: 5px;
            background-image: url('../../images/bg_imp_on.png');
        }
        
        .Click a:hover
        {
            color: #666666;
            text-decoration: none;
            float: left;
            width: 95px;
            height: 25px;
            line-height: 25px;
            padding-left: 5px;
            background-image: url('../../images/bg_imp_on.png');
        }
        
        #upPanelReport
        {
            margin: 0px;
            margin-left: 5px;
            margin-right: 5px;
            margin-bottom: 5px;
            padding-left: 5px;
            padding-top: 5px;
            background-color: #ffffff;
            border: 1px solid #cccccc;
            padding-right: 5px;
            float: left;
            width: 97%;
        }
        .testoLungo
        {
            width: 400px;
        }
        .list_item
        {
            float:left;
            width:80%;
        }
        .refresh
        {
            float:right;
            width:20%;
        }
    </style>
    <script type="text/javascript" language="javascript">


        function beginRequest(sender, args) {
            $find("mdlWait").show();
        }

        function endRequest(sender, args) {
            $find("mdlWait").hide();
        }

        function get_estensione(path) {

            posizione_punto = path.lastIndexOf(".");
            lunghezza_stringa = path.length;
            estensione = path.substring(posizione_punto + 1, lunghezza_stringa);
            return estensione;
        }

        function controlla_estensione(path) {
            if (get_estensione(path) != "xls")
            {
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

        function OpenImpotDetails(id) {
            var newLeft = (screen.availWidth / 2 - 225);
            var newTop = (screen.availHeight - 704);
            var retval = window.showModalDialog('ImportPregressiDettaglio.aspx?id=' + id, 'OpenImpotDetails', 'dialogWidth:950px;dialogHeight:600px;status:no;resizable:no;scroll:yes;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');

        }

        function beginRequest(sender, args) {
            $find("mdlWait").show();
        }

        function endRequest(sender, args) {
            $find("mdlWait").hide();
        }
    </script>
</head>
<body>
    <form id="Form" runat="server" method="post" enctype="multipart/form-data" defaultbutton="btn_chiudi">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000"
        EnablePartialRendering="true">
    </asp:ScriptManager>
    <script language="javascript" type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
    </script>
    <div id="menu" style="width:100%;">
        <div class="list_item">
            <ul>
            
                <li id="nuovo_imp" runat="server">
                    <asp:LinkButton ID="NIC" Text="Nuovo Import" OnClick="NI_Click" runat="server" ToolTip="Nuovo Import"></asp:LinkButton>
                </li>
                <li id="stato_imp" runat="server">
                    <asp:LinkButton ID="SIC" Text="Stato Import" OnClick="SI_Click" runat="server" ToolTip="Stato Import"></asp:LinkButton>
                </li>
            </ul>
         </div>
         <div class="refresh">
             <ul>
                <li>
                    <div style="float:left; width:80%;">
                        <asp:Label ID="label_refresh" runat="server" Font-Size="X-Small" Font-Names="Verdana" ForeColor="#333333" Visible="false">
                        Ricarica lo Stato Import
                        </asp:Label>
                    </div>
                    <div style="float:right; width:20%;">
                        <asp:UpdatePanel runat="server">
                            <ContentTemplate>
                                <asp:ImageButton ID="btn_refresh" runat="server" CssClass="testo_grigio" CommandName="Aggiorna"
                                    ImageUrl="../../images/ricerca/remove_search_filter.gif"
                                    ToolTip="Aggiorna Stato Import" OnClick="Refresh" Visible="false" 
                                    onmouseover="this.src='../../images/ricerca/remove_search_filter_up.gif'"
                                    onmouseout="this.src='../../images/ricerca/remove_search_filter.gif'"></asp:ImageButton>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </li>
             </ul>
         </div>
    </div>
    <asp:UpdatePanel ID="tipo_import" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="n_import" runat="server">
                <div class="cont_pref">
                    <asp:UpdatePanel ID="box_upload" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="topGrid">
                                <asp:Label ID="titlePage" runat="server" Text="IMPORT DOCUMENTI PREGRESSI" CssClass="title"></asp:Label>
                            </div>
                            <h5>
                                &raquo; <a href="../../AdminTool/Gestione_Import/import_pregressi.xls" title="Scarica il modulo excel"
                                    target="_blank"><span class="nero">Scarica il modulo excel</span></a> &laquo;</h5>
                            <p>
                                Seleziona un file Excel valido:
                                <input type="file" runat="server" class="testoLungo" id="uploadFile" name="uploadFile"
                                    onchange="controlla_estensione(document.getElementById('uploadFile').value);" />
                            </p>
                            <p>
                                Descrizione:
                                <asp:TextBox Width="85%" runat="server" ID="descrizione" ToolTip="Inserire una descrizione"></asp:TextBox>
                            </p>
                            <h3>
                                <asp:Button ID="btnImporta" runat="server" OnClick="btn_Importa_Click" Text="Inizia Import"
                                    CssClass="pulsante_mini_4" ToolTip="Importa i documenti" />
                                <asp:Button ID="btnNuovo" runat="server" OnClick="btn_Nuovo_Click" Text="Nuovo Import"
                                    CssClass="pulsante_mini_4" ToolTip="Nuovo import" Enabled="false" /></h3>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="upPnlReport" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="pnlReport" runat="server" Visible="false">
                                <div class="topGrid">
                                    <asp:Label ID="lblAvviso" runat="server" Text="ANALISI IMPORT" CssClass="title"></asp:Label>
                                </div>
                                <asp:Panel ID="pnlAvviso" runat="server" Visible="false">
                                    <p>
                                        <asp:Label ID="lbl_alert" runat="server">
                                        </asp:Label>
                                    </p>
                                </asp:Panel>
                                <div align="center">
                                    <asp:DataGrid ID="grvItems" runat="server" AllowSorting="false" AutoGenerateColumns="false"
                                        AllowPaging="False" CssClass="tab_format" OnItemCreated="DataGrid_ItemCreated"
                                        SkinID="datagrid" OnItemDataBound="ImageCreatedRender">
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
                                            <%--commentato per richiesta cliente--%>
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
                                            <%--Andrea De Marco - Modificato per richiesta cliente: Riga diventa Ordinale--%>
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
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </asp:Panel>
            <asp:Panel ID="stato_import" runat="server" Visible="false">
                <div class="cont_pref">
                    <asp:UpdatePanel ID="upPanelReport" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="topGrid">
                                <asp:Label ID="titleReport" runat="server" Text="STATO IMPORT EFFETTUATI" CssClass="title"></asp:Label>
                            </div>
                            <asp:DataGrid ID="gridReport" runat="server" AllowSorting="false" AutoGenerateColumns="false"
                                AllowPaging="False" CssClass="tab_format" OnItemCreated="DataGrid_ItemCreated"
                                OnItemDataBound="ImageCreatedRender" SkinID="datagrid">
                                <AlternatingItemStyle BackColor="#f2f2f2" />
                                <ItemStyle BackColor="#d9d9d9" ForeColor="#333333" />
                                <Columns>
                                    <asp:TemplateColumn Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetReportID((DocsPAWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn ItemStyle-Width="15%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                                        ItemStyle-CssClass="tab_sx3" HeaderText="DESCRIZIONE">
                                        <ItemTemplate>
                                            <asp:Label ID="DESCR" runat="server" Text='<%# this.GetDescrizione((DocsPAWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn ItemStyle-Width="15%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                                        ItemStyle-CssClass="tab_sx3" HeaderText="DATA INIZIO">
                                        <ItemTemplate>
                                            <asp:Label ID="DATAINIZIO" runat="server" Text='<%# this.GetDataInizio((DocsPAWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn ItemStyle-Width="15%" HeaderText="DATA FINE" ItemStyle-CssClass="tab_sx3"
                                        HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                        <ItemTemplate>
                                            <asp:Label ID="DATAFINE" runat="server" Text='<%# this.GetDataFine((DocsPAWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn ItemStyle-Width="14%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                                        ItemStyle-CssClass="tab_sx3" HeaderText="N° DOCUMENTI">
                                        <ItemTemplate>
                                            <asp:Label ID="NDOC" runat="server" Text='<%# this.GetNumeroDocumenti((DocsPAWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn ItemStyle-Width="14%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                                        ItemStyle-CssClass="tab_sx3" HeaderText="PERCENTUALE">
                                        <ItemTemplate>
                                            <asp:Label ID="PERC" runat="server" Text='<%# this.GetPercentuale((DocsPAWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn ItemStyle-Width="20%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                                        ItemStyle-CssClass="tab_sx3" HeaderText="STATO">
                                        <ItemTemplate>
                                            <asp:Image runat="server" ID="ImgPerc" ImageUrl='<%# this.GetImmaginePercentuale((DocsPAWA.DocsPaWR.ReportPregressi)Container.DataItem) %>' />
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
                                                AlternateText="Errore" ImageUrl='<%# this.GetImageErrore((DocsPAWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'
                                                ToolTip='<%# this.GetNumeroDiErrori((DocsPAWA.DocsPaWR.ReportPregressi)Container.DataItem) %>' Visible="false"></asp:Image>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                            </asp:DataGrid>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </asp:Panel>
            <div align="center">
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="btnContinua" runat="server" OnClick="btn_Continua_Click" Text="Continua"
                            CssClass="pulsante_mini_3" ToolTip="Continua con l'import" Enabled="false" />
                        <asp:Button ID="btnEsporta" runat="server" OnClick="btn_Esporta_Click" Text="Esporta"
                            CssClass="pulsante_mini_3" ToolTip="Esporta l'esito dei controlli sincroni" Enabled="false" />
                        <asp:Button ID="btn_chiudi" runat="server" CssClass="pulsante_mini_3" Text="Chiudi"
                            OnClick="btn_chiudi_Click" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
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
                    <img id="imgLoading" src="../../images/loading.gif" style="border-width: 0px;" alt="Attendere prego" />
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
