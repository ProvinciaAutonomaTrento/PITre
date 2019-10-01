<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Summeries.aspx.cs" Inherits="NttDataWA.Summaries.Summeries"
    MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Src="~/UserControls/SearchProjectsTabs.ascx" TagPrefix="uc2" TagName="SearchProjectsTabs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/CorrespondentCustom.ascx" TagPrefix="uc7" TagName="Correspondent" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/jquery.jstree.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        

        function closeAjaxModal(id, retval) { // chiude il popup modale [id] e imposta il valore di ritorno [retval] nel campo hidden
            var p = parent.fra_main;
            if (arguments.length > 2 && arguments[2] != null) {
                p = arguments[2];
            }
            else {
                try {
                    var e = p.$('iframe').get(0);

                    if (e.id != 'ifrm_' + id) {
                        p = e.contentWindow;
                        e = p.$('iframe').get(0);

                        if (e.id != 'ifrm_' + id) {
                            p = e.contentWindow;
                            e = p.$('iframe').get(0);
                        }
                    }
                }
                catch (err) {
                    try {
                        p = parent.fra_main;
                    }
                    catch (err2) {
                        p = parent;
                    }
                }
            }

            if (arguments.length > 1) {
                this.$('.retval' + id + ' input').get(0).value = retval;
            }
            this.$('#' + id + '_panel').dialog('close');
        }
    </script>
    <script type="text/javascript">

        function creatorePopulatedTransm(sender, e) {
            debugger;
            var behavior = $find('AutoCompleteRuoloTrasm');
            var target = behavior.get_completionList();
            if (behavior._currentPrefix != null) {
                var prefix = behavior._currentPrefix.toLowerCase();
                var i;
                for (i = 0; i < target.childNodes.length; i++) {
                    var sValue = target.childNodes[i].innerHTML.toLowerCase();
                    if (sValue.indexOf(prefix) != -1) {
                        var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                        var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                        var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                        target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                        try {
                            target.childNodes[i].attributes["_value"].value = fstr + pstr + estr;
                        }
                        catch (ex) {
                            target.childNodes[i].attributes["_value"] = fstr + pstr + estr;
                        }
                    }
                }
            }
        }



        function proprietarioPopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoProprietario');
            var target = behavior.get_completionList();
            if (behavior._currentPrefix != null) {
                var prefix = behavior._currentPrefix.toLowerCase();
                var i;
                for (i = 0; i < target.childNodes.length; i++) {
                    var sValue = target.childNodes[i].innerHTML.toLowerCase();
                    if (sValue.indexOf(prefix) != -1) {
                        var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                        var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                        var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                        target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                        try {
                            target.childNodes[i].attributes["_value"].value = fstr + pstr + estr;
                        }
                        catch (ex) {
                            target.childNodes[i].attributes["_value"] = fstr + pstr + estr;
                        }
                    }
                }
            }
        }

        function proprietarioSelected(sender, e) {
            var value = e.get_value();
            if (!value) {

                if (e._item.parentElement && e._item.parentElement.tagName == "LI") {

                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI") {
                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentNode && e._item.parentNode.tagName == "LI") {
                    value = e._item.parentNode._value;
                }
                else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI") {
                    value = e._item.parentNode.parentNode._value;
                }
                else value = "";

            }

            var searchText = $get('<%=txt_descrCorr.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txt_descrCorr.ClientID%>").focus();
            document.getElementById("<%=this.txt_descrCorr.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txt_codCorr.ClientID%>").value = codice;
            document.getElementById("<%=txt_descrCorr.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txt_codCorr.ClientID%>', '');
        }

        function CombineRowsHover() {
            $(".tbl_rounded tr.NormalRow td").hover(function () {
                var row = $(this).parent().parent().children().index($(this).parent());
                var index = row + 1;
                if (row % 2 == 0) index = row - 1;
                $($(".tbl_rounded tr").get(index)).addClass("NormalRowHover");
            }, function () {
                var row = $(this).parent().parent().children().index($(this).parent());
                var index = row + 1;
                if (row % 2 == 0) index = row - 1;
                $($(".tbl_rounded tr").get(index)).removeClass("NormalRowHover");
            });

            $(".tbl_rounded tr.AltRow td").hover(function () {
                var row = $(this).parent().parent().children().index($(this).parent());
                var index = row + 1;
                if (row % 2 == 0) index = row - 1;
                $($(".tbl_rounded tr").get(index)).addClass("AltRowHover");
            }, function () {
                var row = $(this).parent().parent().children().index($(this).parent());
                var index = row + 1;
                if (row % 2 == 0) index = row - 1;
                $($(".tbl_rounded tr").get(index)).removeClass("AltRowHover");
            });
        }
    </script>
    <script type="text/javascript">
        var fsoApp;

        function generateRendomExportFileName() {
            var text = "_";
            var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            for (var i = 0; i < 5; i++)
                text += possible.charAt(Math.floor(Math.random() * possible.length));

            return text;
        }

        function OpenFileApplet(typeFile) {
            var filePath;
            var exportUrl;
            var http;
            var applName;
            var fso;
            var folder;
            var path;

            if (fsoApp == undefined) {
                fsoApp = window.document.plugins[0];
            }
            if (fsoApp == undefined) {
                fsoApp = document.applets[0];
            }

            var i = 0;
            var found = false;
            while (!found && i <= document.applets.length) {
                try {
                    fsoApp.GetSpecialFolder();
                    found = true;
                }
                catch (ex2) {
                    fsoApp = document.applets[i];
                }
                i++;
            }

            try {
                // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0

                path = fsoApp.GetSpecialFolder();

                if (typeFile == "PDF") {
                    filePath = path + "\\export" + generateRendomExportFileName() + ".pdf";
                    applName = "Adobe Acrobat";
                }
                else if (typeFile == "XLS") {
                    filePath = path + "\\export" + generateRendomExportFileName() + ".xls";
                    applName = "Microsoft Excel";
                }
                else if (typeFile == "Model") {
                    filePath = path + "\\export" + generateRendomExportFileName() + ".xls";
                    applName = "Microsoft Excel";
                }
                else if (typeFile == "ODS") {
                    filePath = path + "\\export" + generateRendomExportFileName() + ".ods";
                    applName = "Open Office";
                }

                /*
                var status = 0;
                var content = '';
                $.ajax({
                    type: 'POST',
                    cache: false,
                    processData: false,
                    url: "../ExportDati/exportDatiPage.aspx?isapplet=true",
                    success: function (data, textStatus, jqXHR) {
                        status = jqXHR.status;
                        content = jqXHR.responseText;
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        status = textStatus;
                        content = null;
                    },
                    async: false
                });

                if (content != null && status == 200) {
                    fsoApp.saveFile(filePath, content);
                    fsoApp.openFile(filePath);

                    //self.close();
                }
                */
	            var encodedFilePath = EncodeHtml(filePath);
                var paramPost = "<<filePath:" + encodedFilePath + ">>";
                var urlPost = '<%=httpFullPath%>' + '/ExportDati/exportDatiPage.aspx';

                if (fsoApp.saveFileFromURL(filePath, urlPost, paramPost)) {
                    fsoApp.openFile(filePath);
                    retval = true;
                }
                else {
                    alert('Applet error to get file.');
                    retval = false;
                }
            }
            catch (ex) {
                alert(ex.message.toString());
            }
        }

        function OpenFileSocket(typeFile) {
            var filePath;
            var exportUrl;
            var http;
            var applName;
            var fso;
            var folder;
            var path;

		    try {
                getSpecialFolder(function (path, connection) {

                    if (typeFile == "PDF") {
                        filePath = path + "\export" + generateRendomExportFileName() + ".pdf";
                        applName = "Adobe Acrobat";
                    }
                    else if (typeFile == "XLS") {
                        filePath = path + "\export" + generateRendomExportFileName() + ".xls";
                        applName = "Microsoft Excel";
                    }
                    else if (typeFile == "Model") {
                        filePath = path + "\export" + generateRendomExportFileName() + ".xls";
                        applName = "Microsoft Excel";
                    }
                    else if (typeFile == "ODS") {
                        filePath = path + "\export" + generateRendomExportFileName() + ".ods";
                        applName = "Open Office";
                    }
                    var encodedFilePath = EncodeHtml(filePath);
                    var paramPost = "<<filePath:" + encodedFilePath + ">>";
                    var urlPost = '../ExportDati/exportDatiPage.aspx?isapplet=true';

                    $.ajax({
                        type: 'POST',
                        url: urlPost,
                        data: { "filePath": encodedFilePath },
                        success: function (content) {
                            connection.close();
                            saveFile(filePath, content, function (retVal, connection) {
                                if (retVal === "true") {
                                    //alert("SaveFileNoApplet 2 retVal" + retVal);
                                    openFile(filePath);
                                    retval = true;
                                    //alert("SaveFileSystem dentro");
                                    //ShowSuccess();
                                    reallowOp();
                                }
                                else {
                                    alert('Applet error to get file.');
                                    retval = false;
                                    reallowOp();
                                }
                                connection.close();
                            });
                        },
                        error: function () {
                            reallowOp();
                            connection.close();
                        },
                        async: true
                    });
                });
            }
            catch (ex) {
                alert(ex.message.toString());
            }
        }

        function EncodeHtml(value) {
            value = escape(value);
            value = value.replace(/\//g, "%2F");
            value = value.replace(/\?/g, "%3F");
            value = value.replace(/=/g, "%3D");
            value = value.replace(/&/g, "%26");
            value = value.replace(/@/g, "%40");
            return value;
        }

        function OpenFileActiveX(typeFile) {
            var filePath;
            var exportUrl;
            var http;
            var applName;
            var fso;
            var folder;
            var path;

            try {
                fso = FsoWrapper_CreateFsoObject();
                // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0

                path = fso.GetSpecialFolder(2).Path;

                if (typeFile == "PDF") {
                    filePath = path + "\\export" + generateRendomExportFileName() + ".pdf";
                    applName = "Adobe Acrobat";
                }
                else if (typeFile == "XLS") {
                    filePath = path + "\\export" + generateRendomExportFileName() + ".xls";
                    applName = "Microsoft Excel";
                }
                else if (typeFile == "Model") {
                    filePath = path + "\\export" + generateRendomExportFileName() + ".xls";
                    applName = "Microsoft Excel";
                }
                else if (typeFile == "ODS") {
                    filePath = path + "\\export" + generateRendomExportFileName() + ".ods";
                    applName = "Open Office";
                }

                exportUrl = "../ExportDati/exportDatiPage.aspx";
                http = CreateObject("MSXML2.XMLHTTP");
                http.Open("POST", exportUrl, false);
                http.send();

                var content = http.responseBody;

                if (content != null) {
                    AdoStreamWrapper_SaveBinaryData(filePath, content);

                    ShellWrappers_Execute(filePath);

                    self.close();
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
	</script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <!-- ADP inizio -->
    <uc:ajaxpopup Id="VisibilityHistory" runat="server" Url="../popup/VisibilityHistory.aspx?tipoObj=D"
        Width="800" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('containerStandard2', ''); }" />
    <uc:ajaxpopup Id="VisibilityRemove" runat="server" Url="../popup/VisibilityRemove.aspx"
        Width="600" Height="400" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('containerStandard2', ''); }" />
    <uc:ajaxpopup Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup Id="DocumentViewerSummeries" runat="server" Url="../popup/DocumentViewerSummeries.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {__doPostBack('containerStandard2','');}" />
    <uc:ajaxpopup Id="OpenObiettivo" runat="server" Url="../Popup/ObjectiveScheme.aspx"
        IsFullScreen="true" Width="600" Height="400" PermitClose="false" PermitScroll="false"
        CloseFunction="function (event, ui) {__doPostBack('UpPnlResponseProtocol', ''); }" />
    <uc:ajaxpopup Id="OpenCDC" runat="server" Url="../Popup/ricercaCDC.aspx" Width="600"
        Height="900" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('UpPnlResponseProtocol', ''); }" />
    <%-- <uc:ajaxpopup Id="PDFViewer" runat="server" Url="../summeries/PDFViewer.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {__doPostBack('containerStandard2','');}"/>--%>
    <div id="containerTop" style="background-color: Green">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop" style="background-color: Red">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p>
                                        <asp:Literal ID="LitSummeries" runat="server"></asp:Literal></p>
                                </div>
                                <div id="containerStandardTopDx">
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerStandardTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <div id="containerDocumentTabOrangeSx">
                                    <ul>
                                        <li class="searchIAmSearch" id="LiSearchProject" runat="server">
                                            <%--  <asp:HyperLink ID="LinkSearchVisibility" runat="server" NavigateUrl="~/Search/SearchVisibility.aspx"
                                                CssClass="clickable"></asp:HyperLink>--%></li>
                                    </ul>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                        </div>
                    </div>
                    <div id="containerStandardTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard2" runat="server" clientidmode="Static">
                <div id="content">
                    <!-- Parte sinistra-->
                    <div id="contentSx">
                        <div class="box_inside">
                            <div class="row">
                                <!-- filters -->
                                <fieldset class="basic">
                                    <!-- DDL seleziona Report <asp:RadioButtonList  -->
                                    <div class="row">
                                        <div class="col-full">
                                            <asp:DropDownList ID="ddl_prospettiRiepilogativi" runat="server" CssClass="chzn-select-deselect"
                                                AutoPostBack="true" OnSelectedIndexChanged="ddl_prospettiRiepilogativi_SelectedIndexChanged"
                                                RepeatLayout="UnorderedList" Width="97%">
                                                <%-- <asp:ListItem Value="reportAnnualeDoc" Selected="True">Report annuale sui documenti</asp:ListItem>
                                                        <asp:ListItem Value="reportDocClassificati">Report sui documenti classificati</asp:ListItem>
                                                        <asp:ListItem Value="reportAnnualeDocTrasmAmm">Report annuale sui documenti trasmessi ad altre amministrazioni</asp:ListItem>
                                                        <asp:ListItem Value="reportAnnualeDocTrasmAOO">Report annuale sui documenti trasmessi ad altre AOO</asp:ListItem>
                                                        <asp:ListItem Value="reportAnnualeFasc">Report annuale sui fascicoli su titolario attivo</asp:ListItem>
                                                        <asp:ListItem Value="reportAnnualeFascXTit">Report annuale sui fascicoli per voce di titolario</asp:ListItem>
                                                        <asp:ListItem Value="tempiMediLavFasc">Tempi medi di lavorazione fascicoli</asp:ListItem>
                                                        <asp:ListItem Value="ReportDocXSede">Report annuale sui documenti per sede</asp:ListItem>
                                                        <asp:ListItem Value="ReportDocXUo">Report annuale sui documenti protocollati per UO</asp:ListItem>
                                                        <asp:ListItem Value="reportNumFasc">Report conteggio fascicoli procedimentali</asp:ListItem> 
                                                        <asp:ListItem Value="reportNumDocInFasc">Report fascicoli procedimentali e documenti contenuti</asp:ListItem>    --%>
                                                <%--<asp:ListItem Value="stampaProtArma">Stampa protocollo Arma</asp:ListItem>
                                                        <asp:ListItem Value="stampaDettaglioPratica">Stampa Dettaglio Pratica</asp:ListItem>
                                                        <asp:ListItem Value="stampaGiornaleRiscontri">Stampa Giornale Riscontri</asp:ListItem>
                                                        <asp:ListItem Value="documentiSpediti">Documenti spediti</asp:ListItem>--%>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <!--  codice -->
                                </fieldset>
                            </div>
                        </div>
                        <div class="box_inside">
                            <div class="row">
                                <!-- filters -->
                                <fieldset class="basic">
                                    <!--reportAnnualeDoc-->
                                    <asp:PlaceHolder ID="pnl_amm" runat="server">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblAmm" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <asp:DropDownList ID="ddl_amm" TabIndex="1" runat="server" CssClass="chzn-select-deselect"
                                                    AutoPostBack="True" Width="97%">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--reportDocClassificati -->
                                    <asp:PlaceHolder ID="panel_reg" runat="server">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblReg" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <asp:DropDownList ID="ddl_registro" TabIndex="2" runat="server" CssClass="chzn-select-deselect"
                                                    AutoPostBack="True" OnSelectedIndexChanged="ddl_registro_SelectedIndexChanged"
                                                    Width="97%">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--reportAnnualeDocTrasmAmm -->
                                    <asp:PlaceHolder ID="panel_tit_ann_fasc" runat="server" Visible="true">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblTit" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <asp:DropDownList ID="ddl_titolari_report_annuale" TabIndex="2" runat="server" CssClass="chzn-select-deselect"
                                                    AutoPostBack="True" Width="97%">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--reportAnnualeDocTrasmAOO -->
                                    <asp:PlaceHolder ID="panel_sede" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblSede" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <asp:DropDownList ID="ddl_sede" TabIndex="3" runat="server" CssClass="chzn-select-deselect"
                                                    AutoPostBack="True" Width="97%">
                                                    <asp:ListItem Value="">Tutte le Sedi</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--reportAnnualeFas -->
                                    <asp:PlaceHolder ID="pnl_anno" runat="server" Visible="true">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblAnno" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <asp:DropDownList ID="ddl_anno" TabIndex="4" runat="server" CssClass="chzn-select-deselect"
                                                    AutoPostBack="True" Width="97%">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--reportAnnualeFascXTit -->
                                    <asp:PlaceHolder ID="pnl_mese" runat="server" Visible="true">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblMese" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <asp:DropDownList ID="ddl_Mese" runat="server" CssClass="chzn-select-deselect" Width="97%">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--tempiMediLavFasc -->
                                    <asp:PlaceHolder ID="pnl_modalita" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblModa" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <asp:RadioButtonList ID="rb_modalita" runat="server" CssClass="rblHorizontal" AutoPostBack="True"
                                                    CellPadding="0" CellSpacing="0" RepeatDirection="Horizontal">
                                                    <asp:ListItem Value="Compatta" Selected="True">Compatta</asp:ListItem>
                                                    <asp:ListItem Value="Estesa">Estesa</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--ReportDocXSede -->
                                    <asp:PlaceHolder ID="pnl_regCC" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblReg2" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <asp:DropDownList ID="ddl_regCC" TabIndex="2" runat="server" CssClass="chzn-select-deselect"
                                                    AutoPostBack="True" Width="97%">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--ReportDocXUo -->
                                    <asp:PlaceHolder ID="pnl_protArma" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblTit2" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <asp:DropDownList ID="ddl_et_titolario" TabIndex="4" runat="server" Width="97%" CssClass="chzn-select-deselect"
                                                    AutoPostBack="True">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--reportNumFasc -->
                                    <asp:PlaceHolder ID="pnl_DettPratica" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblPra" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <cc1:CustomTextArea ID="txt_NumPratica" runat="server" CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled" />
                                                <%--            <asp:TextBox ID="txt_NumPratica" runat="server" AutoPostBack="true" Width="40px"
                                    Height="18px" CssClass="testo_grigio"></asp:TextBox>--%>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--reportNumDocInFasc -->
                                    <asp:PlaceHolder ID="pnl_DocSpediti" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblSpe" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <%--  <div class="col">--%>
                                            <%-- <asp:label id="lbl_dataSpedDa" runat="server" cssclass="testo_grigio" width="8px">Da</asp:label>--%>
                                            <%-- <span class="weight"><asp:Literal ID="lbl_dataSpedDa" runat="server" /></span>--%>
                                            <%--<uc1:Calendario ID="txt_dataSpedDa" runat="server" Visible="true" PaginaChiamante="prospetti" />--%>&nbsp;&nbsp;&nbsp;
                                            <%--  <uc6:Calendar ID="txt_dataSpedDa" runat="server" Visible="true" PaginaChiamante="prospetti" /> --%>
                                            <%--<asp:label id="lbl_dataSpedA" runat="server" cssclass="testo_grigio" width="8px">a</asp:label>--%>
                                            <%--   <span class="weight"><asp:Literal ID="lbl_dataSpedA" runat="server" /></span>--%>
                                            <%--<uc1:Calendario ID="txt_dataSpedA" runat="server" Visible="true" PaginaChiamante="prospetti" />--%>
                                            <%--<uc6:Calendar ID="txt_dataSpedA" runat="server" Visible="true" PaginaChiamante="prospetti" />--%>
                                            <div class="col2">
                                                <asp:Literal runat="server" ID="LtlDaDataSegDiEmerg"></asp:Literal>
                                            </div>
                                            <div class="col4">
                                                <cc1:CustomTextArea ID="txt_dataSpedDa" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                    CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                            </div>
                                            <div class="col2">
                                                <asp:Literal runat="server" ID="LtlADataSegDiEmerg"></asp:Literal>
                                            </div>
                                            <div class="col4">
                                                <cc1:CustomTextArea ID="txt_dataSpedA" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                    CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                            </div>
                                            <%--</div>  --%>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <span class="weight">
                                                    <asp:Label ID="SummeriestLblProto" runat="server">
                                                    </asp:Label>
                                                </span>
                                            </div>
                                            <div class="col-full">
                                                <asp:RadioButtonList ID="rb_confermaSpedizione" runat="server" CssClass="rblHorizontal"
                                                    RepeatDirection="Horizontal">
                                                    <asp:ListItem Value="1">Sì&nbsp;&nbsp;</asp:ListItem>
                                                    <asp:ListItem Value="0">No&nbsp;&nbsp;</asp:ListItem>
                                                    <asp:ListItem Value="T" Selected="True">Tutti</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--stampaProtArma-->
                                    <asp:PlaceHolder ID="pnl_sottoposto" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblProprie" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                            <div class="col-right-no-margin">
                                                <cc1:CustomImageButton runat="server" ID="ImgProprietarioAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                    OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                    OnClick="SummeriesImgSenderAddressBook_Click" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <%--<INPUT id="hd_systemIdCorrSott" type="hidden" size="1" name="hd_systemIdSott" runat="server">--%>
                                                <asp:RadioButtonList ID="rb_scelta_sott" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="rb_scelta_sott_SelectedIndexChanged" AutoPostBack="true"
                                                    Style="height: 21px; float: left;">
                                                    <asp:ListItem Value="UO" Selected="True">UO</asp:ListItem>
                                                    <asp:ListItem Value="RF">RF</asp:ListItem>
                                                </asp:RadioButtonList>
                                                <%--<asp:Image id="btn_img_sott_rubr" runat="server" ToolTip="Seleziona una uo dalla rubrica"
														        ImageUrl="../../images/proto/rubrica.gif" AlternateText="Seleziona una uo dalla rubrica" style="float:right;margin-right:20px;"></asp:Image>--%>
                                            </div>
                                        </div>
                                        <asp:PlaceHolder ID="pnl_scelta_uo" runat="server" Visible="false">
                                            <%--     <div class="row">
                            <div class="col"> 
                                  <span class="weight">
                                         <asp:Label ID="SummeriestLblUO" runat="server" >
                                         </asp:Label>
                                  </span>                    
                                  </div>
                            <div class="col">
                              <cc1:CustomTextArea  id="txt1_corr_sott" runat="server" AutoPostBack="True" CssClass="txt_textdata"></cc1:CustomTextArea>
								<cc1:CustomTextArea id="txt2_corr_sott" runat="server" CssClass="txt_textdata" ></cc1:CustomTextArea>                            
                            </div>
                        </div>--%>
                                            <%-- <div class="row">
                            <asp:HiddenField ID="idProprietario" runat="server" />
                            <div class="col"> 
                                  <span class="weight">
                                         <asp:Label ID="SummeriestLblUO" runat="server" >
                                         </asp:Label>
                                  </span>           
                            <div class="colHalf">
                                <cc1:CustomTextArea ID="txt_codCorr" runat="server" CssClass="txt_addressBookLeft"
                                    AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                    AutoCompleteType="Disabled">
                                </cc1:CustomTextArea>
                            </div>
                                <div class="col">
                                    <div class="col3">
                                        <cc1:CustomTextArea ID="txt_descrCorr" runat="server" CssClass="txt_projectRight"
                                            CssClassReadOnly="txt_ProjectRight_disabled">
                                        </cc1:CustomTextArea>
                                    </div>
                                </div>
                                <uc1:AutoCompleteExtender runat="server" ID="RapidProprietario" TargetControlID="txt_descrCorr"
                                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                UseContextKey="true" OnClientItemSelected="proprietarioSelected" BehaviorID="AutoCompleteExIngressoProprietario"
                                OnClientPopulated="proprietarioPopulated ">
                            </uc1:AutoCompleteExtender>
                           </div>--%>
                                            <div class="row">
                                                <asp:UpdatePanel ID="UpPnlSummeriesUO" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <div class="row">
                                                            <div class="col">
                                                                <span class="weight">
                                                                    <asp:Label ID="SummeriestLblUO" runat="server"></asp:Label></span>
                                                            </div>
                                                        </div>
                                                        <%--<div class="col-right-no-margin">
                                                <cc1:CustomImageButton runat="server" ID="ImgProprietarioAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                            OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                            OnClick="SummeriesImgSenderAddressBook_Click" />
                                            </div>--%>
                                                        <div class="row">
                                                            <asp:HiddenField ID="idProprietario" runat="server" />
                                                            <div class="colHalf">
                                                                <cc1:CustomTextArea ID="txt_codCorr" runat="server" CssClass="txt_addressBookLeft"
                                                                    AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                                    AutoCompleteType="Disabled">
                                                                </cc1:CustomTextArea>
                                                            </div>
                                                            <div class="colHalf2">
                                                                <div class="colHalf3">
                                                                    <cc1:CustomTextArea ID="txt_descrCorr" runat="server" CssClass="txt_addressBookRight"
                                                                        CssClassReadOnly="txt_addressBookRight_disabled">
                                                                    </cc1:CustomTextArea>
                                                                </div>
                                                            </div>
                                                           <uc1:AutoCompleteExtender runat="server" ID="RapidProprietario" TargetControlID="txt_descrCorr"
                                                                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                                MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                UseContextKey="true" OnClientItemSelected="proprietarioSelected" BehaviorID="AutoCompleteExIngressoProprietario"
                                                                OnClientPopulated="proprietarioPopulated" Enabled="false">
                                                            </uc1:AutoCompleteExtender>
                                                        </div>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <asp:CheckBox ID="chk_sottoposti" runat="server" AutoPostBack="True" Text="visualizza sottoposti"
                                                        class="titolo_scheda" />
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="pnl_scelta_rf" runat="server" Visible="false">
                                            <div class="row">
                                                <div class="col">
                                                    RF*</div>
                                                <div class="col">
                                                    <asp:DropDownList ID="ddl_rf" TabIndex="4" runat="server" Width="150" CssClass="chzn-select-deselect"
                                                        AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Label ID="SummeriestLblDcrea" runat="server">
                                                        </asp:Label>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col2">
                                                <asp:DropDownList ID="ddl_tipo_data_creazione" runat="server" AutoPostBack="True"
                                                    CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_tipo_data_creazione_SelectedIndexChanged"
                                                    Width="150">
                                                    <asp:ListItem Text="Val. singolo" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Intervallo" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Oggi" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="Sett. Corr." Value="3"></asp:ListItem>
                                                    <asp:ListItem Text="Mese Corr." Value="4"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <%--  <div class="col">--%>
                                            <%-- <asp:label id="lbl_dal_apertura" runat="server" Visible="false">Dal</asp:label>--%>
                                            <%--<uc1:Calendario id="cld_creazione_dal" runat="server" PaginaChiamante="prospetti"/> --%>
                                            <%-- <uc6:Calendar id="cld_creazione_dal" runat="server" PaginaChiamante="prospetti"/> --%>
                                            <%--  <asp:label id="lbl_al_apertura" runat="server" Visible="false">al</asp:label> --%>
                                            <%--  <uc1:Calendario id="cld_creazione_al" runat="server" PaginaChiamante="prospetti" Visible="false"/>--%>
                                            <%--<uc6:Calendar id="cld_creazione_al" runat="server" PaginaChiamante="prospetti" Visible="false"/>--%>
                                            <div class="col2">
                                                <asp:Label ID="lbl_dal_apertura" runat="server" Visible="false">Dal</asp:Label>
                                            </div>
                                            <div class="col2">
                                                <cc1:CustomTextArea ID="cld_creazione_dal" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                    CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                            </div>
                                            <div class="col2">
                                                <asp:Label ID="lbl_al_apertura" runat="server" Visible="false">al</asp:Label>
                                            </div>
                                            <div class="col2">
                                                <cc1:CustomTextArea ID="cld_creazione_al" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                    CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                            </div>
                                        </div>
                                        <%-- </div>--%>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">Data chiusura</span></p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col2">
                                                <asp:DropDownList ID="ddl_tipo_data_chiusura" runat="server" AutoPostBack="True"
                                                    CssClass="chzn-select-deselect" OnSelectedIndexChanged="ddl_tipo_data_chiusura_SelectedIndexChanged"
                                                    Width="150">
                                                    <asp:ListItem Text="Val. singolo" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Intervallo" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Oggi" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="Sett. Corr." Value="3"></asp:ListItem>
                                                    <asp:ListItem Text="Mese Corr." Value="4"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col2">
                                                <asp:Label ID="lbl_dal_chiusura" runat="server" Visible="false">Dal</asp:Label>
                                            </div>
                                            <div class="col2">
                                                <cc1:CustomTextArea ID="cld_chiusura_dal" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                    CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                            </div>
                                            <div class="col2">
                                                <asp:Label ID="lbl_al_chiusura" runat="server" Visible="false">al</asp:Label>
                                            </div>
                                            <div class="col2">
                                                <cc1:CustomTextArea ID="cld_chiusura_al" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                    CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">Titolario*</span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <asp:DropDownList ID="ddl_titolari" TabIndex="4" runat="server" Width="232px" CssClass="chzn-select-deselect"
                                                    AutoPostBack="True">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--stampaDettaglioPratica-->
                                    <asp:PlaceHolder ID="pnl_FiltriCDC" runat="server" Visible="false">
                                        <div class="row">
                                            <asp:PlaceHolder ID="pnlContrData" runat="server">
                                                <div class="col">
                                                    <asp:Label ID="lbl_Data" runat="server" Text="Data *" /></div>
                                                <div class="col2">
                                                    <span class="weight">
                                                        <asp:Literal ID="lbl_DataDa" runat="server" /></span>
                                                </div>
                                                <div class="col2">
                                                    <cc1:CustomTextArea ID="dataDa" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                                <div class="col2">
                                                    <span class="weight">
                                                        <asp:Literal ID="lbl_DataA" runat="server" /></span>
                                                </div>
                                                <div class="col2">
                                                    <cc1:CustomTextArea ID="dataA" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                                </div>
                                            </asp:PlaceHolder>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <asp:Label ID="lbl_Uffici" runat="server" Text="Uffici" /></div>
                                            <div class="col">
                                                <asp:DropDownList ID="ddl_uffici" runat="server" CssClass="chzn-select-deselect"
                                                    Width="98%" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <asp:Label ID="lbl_Magistrato" runat="server" Text="Magistrato" /></div>
                                            <div class="col">
                                                <%-- <uc2:Corrispondente ID="corr_magistrato" runat="server" CSS_CODICE="testo_grigio" CSS_DESCRIZIONE="testo_grigio" Width="98%"/>--%>
                                                <uc7:Correspondent ID="corr_magistrato" runat="server" CSS_CODICE="testo_grigio"
                                                    CSS_DESCRIZIONE="testo_grigio" Width="98%" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <asp:Label ID="lbl_Revisore" runat="server" Text="Revisore" /></div>
                                            <div class="col">
                                                <uc7:Correspondent ID="corr_revisore" runat="server" CSS_CODICE="testo_grigio" CSS_DESCRIZIONE="testo_grigio"
                                                    Width="98%" />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!--stampaGiornaleRiscontri-->
                                    <asp:PlaceHolder ID="pnl_FiltriCDCElencoDecreti" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                <asp:Label ID="lbl_Elenco" runat="server" Text="Numero Elenco *" />
                                                <cc1:CustomTextArea ID="txt_Elenco" runat="server" CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled" />
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <!-- //DA COMMENTARE  relativo a misurazione obiettivi-->
                                    <asp:PlaceHolder ID="pnl_MisObiettivi" runat="server" Visible="false">
                                        <div class="row">
                                            <div class="col">
                                                Data Protocollo
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="black">
                                                        <asp:Label ID="lbl_dtaCollocationFrom" runat="server"></asp:Label>
                                                        <cc1:CustomTextArea ID="dtaCollocation_TxtFrom" runat="server" CssClass="txt_date datepicker"
                                                            CssClassReadOnly="txt_date_disabled" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </span>
                                                </p>
                                            </div>
                                            <div class="col">
                                                <p>
                                                    <span class="black">
                                                        <asp:Label ID="lbl_dtaCollocationTo" runat="server"></asp:Label>
                                                        <cc1:CustomTextArea ID="dtaCollocation_TxtTo" runat="server" CssClass="txt_date datepicker"
                                                            CssClassReadOnly="txt_date_disabled" ClientIDMode="Static"></cc1:CustomTextArea>
                                                    </span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                Obiettivo
                                            </div>
                                            <div class="col-right-no-margin">
                                                <cc1:CustomImageButton runat="server" ID="btn_obiettivo" ImageUrl="../Images/Icons/classification_scheme.png"
                                                    OnMouseOutImage="../Images/Icons/classification_scheme.png" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png"
                                                    ToolTip="Obiettivo" OnClientClick="return ajaxModalPopupOpenObiettivo();" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <input id="hd_idOb" type="hidden" name="hd_IdOb" runat="server">
                                            <div class="colHalf">
                                                <cc1:CustomTextArea ID="txtCodiceObiettivo" runat="server" CssClass="txt_addressBookLeft"
                                                    AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoCompleteType="Disabled"
                                                    OnTextChanged="txt_cod_obiettivo_TextChanged" Enabled="false">
                                                </cc1:CustomTextArea>
                                            </div>
                                            <div class="colHalf2">
                                                <div class="colHalf3">
                                                    <cc1:CustomTextArea ID="txtDescrizioneObiettivo" runat="server" CssClass="txt_addressBookRight"
                                                        CssClassReadOnly="txt_addressBookRight_disabled" Style="float: left;" Enabled="false">
                                                    </cc1:CustomTextArea>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                &nbsp;Centro di Costo
                                            </div>
                                            <div class="col-right-no-margin">
                                                <input id="hd_IdCorrCDC" type="hidden" name="hd_systemIdCorrCDC" runat="server">
                                                <cc1:CustomImageButton runat="server" ID="btn_CDC" ImageUrl="../../images/proto/rubrica_euro.gif"
                                                    OnMouseOutImage="../../images/proto/rubrica_euro.gif" OnMouseOverImage="../../images/proto/rubrica_euro.gif"
                                                    CssClass="clickable" ImageUrlDisabled="../../images/proto/rubrica_euro.gif" ToolTip="Ricerca Centro di Costo"
                                                    OnClientClick="return ajaxModalPopupOpenCDC();" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="colHalf">
                                                <cc1:CustomTextArea ID="txtCDC" runat="server" CssClass="txt_addressBookLeft" AutoPostBack="true"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" AutoCompleteType="Disabled" Enabled="false"
                                                    OnTextChanged="txt_cod_costo_TextChanged">
                                                </cc1:CustomTextArea>
                                            </div>
                                            <div class="colHalf2">
                                                <div class="colHalf3">
                                                    <cc1:CustomTextArea ID="txtdescrCDC" runat="server" CssClass="txt_addressBookRight"
                                                        CssClassReadOnly="txt_addressBookRight_disabled" Style="float: left;" Enabled="false">
                                                    </cc1:CustomTextArea>
                                                    <asp:TextBox ID="txt_id_corr" Visible="false" runat="server"></asp:TextBox><asp:TextBox
                                                        ID="txt_cdc_storico" Visible="false" runat="server" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                &nbsp;Tipo di &nbsp;Comunicazione
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-full">
                                                <p>
                                                    <div class="styled-select_full">
                                                        <asp:DropDownList ID="ddl_comunicazione" runat="server" AutoPostBack="True" CssClass="chzn-select-deselect"
                                                            Width="100%" Enabled="false">
                                                            <%-- onselectedindexchanged="ddlTipiCom_SelectedIndexChanged"--%>
                                                            <asp:ListItem Text=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                &nbsp;Mittente</div>
                                            <div class="col-right-no-margin">
                                                <cc1:CustomImageButton runat="server" ID="btn_Rubrica_M" ImageUrl="../Images/Icons/address_book.png"
                                                    OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                    CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png" />
                                                <%--OnClick="DocumentImgSenderAddressBook_Click"--%>
                                            </div>
                                            <div class="col">
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                </div>
                                                <div class="colHalf">
                                                    <cc1:CustomTextArea ID="txt_CodMit" runat="server" CssClass="txt_addressBookLeft"
                                                        AutoPostBack="true" OnTextChanged="txt_CodMit_TextChanged" CssClassReadOnly="txt_addressBookLeft_disabled"
                                                        onchange="disallowOp('Content2');">
                                                    </cc1:CustomTextArea>
                                                </div>
                                                <div class="colHalf2">
                                                    <div class="colHalf3">
                                                        <cc1:CustomTextArea ID="txt_DescMit" runat="server" CssClass="txt_projectRight" CssClassReadOnly="txt_ProjectRight_disabled">
                                                        </cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                <input id="hd_systemIdMit" type="hidden" size="1" name="hd_systemIdMit" runat="server">
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    &nbsp;Destinatario</div>
                                                <div class="col-right-no-margin">
                                                    <cc1:CustomImageButton runat="server" ID="btn_Rubrica_D" ImageUrl="../Images/Icons/address_book.png"
                                                        OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png" />
                                                    <%--OnClick="DocumentImgSenderAddressBook_Click"--%>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                </div>
                                                <div class="colHalf">
                                                    <cc1:CustomTextArea ID="txt_CodDest" runat="server" CssClass="txt_addressBookLeft"
                                                        AutoPostBack="true" OnTextChanged="txt_CodMit_TextChanged" CssClassReadOnly="txt_addressBookLeft_disabled"
                                                        onchange="disallowOp('Content2');">
                                                    </cc1:CustomTextArea>
                                                </div>
                                                <div class="colHalf2">
                                                    <div class="colHalf3">
                                                        <cc1:CustomTextArea ID="txt_DescDest" runat="server" CssClass="txt_projectRight"
                                                            CssClassReadOnly="txt_ProjectRight_disabled">
                                                        </cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                <input id="hd_systemIdDest" type="hidden" size="1" name="hd_systemIdDest" runat="server">
                                            </div>
                                    </asp:PlaceHolder>
                                </fieldset>
                            </div>
                        </div>
                        <!-- chiusura box_inside -->
                    </div>
                    <!-- chiusura contentSx -->
                    <!-- Parte destra che conterrà la visualizzazione del PDF-->
                    <div id="contentDx">
                        <asp:UpdatePanel runat="server" ID="UpPnlContentDxSx" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div id="contentDxSxProspetti" runat="server" clientidmode="Static">
                                    <%--       <asp:UpdatePanel runat="server" ID="UpPnlDocumentNotAcquired">
                                                    <ContentTemplate>
                                                    <fieldset id="docNotAcquisition" style="border: 0;">
                                                        <h4 class="vcenter"><asp:Label ID="ViewDocumentLblNoAcquired" runat="server"></asp:Label></h4>
                                                    </fieldset>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                        <asp:UpdatePanel runat="server" ID="UpPnlDocumentAcquired" Visible="false" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <fieldset id="docAcquisition" style="border: 0;">
                                                        <h4><asp:Label ID="ViewDocumentLblAcquired" runat="server"></asp:Label></h4>
                                                        <h5><img id="ViewDocumentImageDocumentAcquired" alt="" runat="server"/></h5>
                                                        <h6><asp:LinkButton ID="ViewDocumentLinkFile" runat="server" OnClick="LinkViewFileDocument" Font-Size="1.5em"></asp:LinkButton></h6>
                                                    </fieldset>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>--%>
                                    <asp:UpdatePanel ID="UpPnlDocumentData" runat="server" UpdateMode="Conditional" ClientIDMode="Static"
                                        Visible="false">
                                        <ContentTemplate>
                                            <%--<div id="divFrame" class="row" runat="server">--%>
                                            <fieldset>
                                                <%-- <div class="row">--%>
                                                <iframe width="100%" height="99%" frameborder="0" marginheight="0" marginwidth="0"
                                                    id="frame" runat="server" clientidmode="Static" style="z-index: 999999999;">
                                                </iframe>
                                                <%--</div>--%>
                                            </fieldset>
                                            <%-- </div>  --%>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- adp fine -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="SummeriesPrint" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SummeriesPrint_Click" />
            <cc1:CustomButton ID="SummeriesZoom" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="SummeriesZoom_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">        $(".chzn-select-deselect").chosen({
            allow_single_deselect: true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
          <asp:PlaceHolder ID="plcActiveX" runat="server" Visible="false"></asp:PlaceHolder>
    <asp:PlaceHolder ID="plcApplet" runat="server" Visible="false">
        <applet id='fsoApplet' 
            code = 'com.nttdata.fsoApplet.gui.FsoApplet' 
            codebase=  '<%=Page.ResolveClientUrl("~/Libraries/")%>'
            archive='FsoApplet.jar'
		    width = '10'   height = '9'>
            <param name="java_arguments" value="-Xms128m" />
            <param name="java_arguments" value="-Xmx512m" />
        </applet>
    </asp:PlaceHolder>
</asp:Content>
