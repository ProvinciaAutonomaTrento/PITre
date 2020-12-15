<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true" CodeBehind="ImportPrevious.aspx.cs" Inherits="NttDataWA.ImportDati.ImportPrevious" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Src="~/ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="~/ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="uc2" %>
<%@ Register Src="~/ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }        
        
        fieldset {min-height: 70px;}
        
        #tabs {background: #fff; border: 0; width: 98%;}
        #tabs ul {background: #fff; border-top: 0; border-left: 0; border-right: 0;}
        #tabs div {background: #fff;}
        
        .tbl_center th, .tbl_center td {text-align: center;}
    </style>
    <script type="text/javascript">
        var fsoApp;

        function get_estensione(path) {
            var posizione_punto = path.lastIndexOf(".");
            var lunghezza_stringa = path.length;
            var estensione = path.substring(posizione_punto + 1, lunghezza_stringa);
            return estensione;
        }

        function controlla_estensione(el) {
            if (get_estensione(el.value) != "xls") {
                alert("Il file deve avere estensione .xls");
                sostituisciInputFile(el);
            }
        }

        function sostituisciInputFile(el) {
            return el.parentNode.replaceChild(el.cloneNode(true), el);
        }
        function EncodeHtml(value) {
            value = escape(value);
            value = value.replace(/\//g, '%2F');
            value = value.replace(/\?/g, '%3F');
            value = value.replace(/=/g, '%3D');
            value = value.replace(/&/g, '%26');
            value = value.replace(/@/g, '%40');
            return value;
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

                getSpecialFolder(function (path) {

                    if (path) {
                        filePath = path + '\export.xls';
                        applName = 'Microsoft Excel';
                        var encodedFilePath = EncodeHtml(filePath);
                        var urlPost = '../Popup/ImportPreviousReport_exportDatiPage.aspx?issocket=true';

                        $.ajax({
                            type: 'POST',
                            url: urlPost,
                            data: { "filePath": encodedFilePath },
                            success: function (content) {
                                //var content = response.responseText;
                                saveFile(filePath, content, function (retVal, connection) {
                                    connection.close();
                                    if (retVal == "true") {
                                        openFile(filePath);
                                        retval = true;
                                        ShowSuccess();
                                        reallowOp();
                                        if (callback)
                                            callback(retVal, connection);
                                    }
                                    else {
                                        alert('Applet error to get file.');
                                        retval = false;
                                        if (callback)
                                            callback(retVal, connection);
                                    }

                                });
                            },
                            error: function () {
                                reallowOp();
                                sendError();
                            },
                            async: true
                        });

                    } else {
                        alert('Applet error to get file.');
                    }

                });


            }
            catch (ex) {
                alert(ex.message.toString());
            }

            reallowOp();
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
                filePath = path + '\\export.xls';
                applName = 'Microsoft Excel';

                exportUrl = "<%NttDataWA.Utils.utils.getHttpFullPath();%>" + '/Popup/ImportPreviousReport_exportDatiPage.aspx';
                http = CreateObject('MSXML2.XMLHTTP');
                http.Open('POST', exportUrl, false);
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

            reallowOp();
        }

        // Creazione oggetto activex con gestione errore
        function CreateObject(objectType) {
            try { return new ActiveXObject(objectType); }
            catch (ex) { alert('Oggetto ' + objectType + ' non istanziato'); }
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
         
             try {
                // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0
                path = fsoApp.GetSpecialFolder();
         
                filePath = path + '\\export.xls';
                applName = 'Microsoft Excel';
         
                var encodedFilePath = EncodeHtml(filePath);
                var paramPost = '<<filePath:' + encodedFilePath + '>>';
                var urlPost = "<%NttDataWA.Utils.utils.getHttpFullPath();%>" + '/Popup/ImportPreviousReport_exportDatiPage.aspx';
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
         
            reallowOp();
         }
                                 

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup2 Id="ImportPreviousReport" runat="server" Url="../Popup/ImportPreviousReport.aspx" IsFullScreen="true"
        PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui)  {$('#UpPnlButtons').click();}" />

    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p><asp:Label ID="pageTitle" runat="server" /></p>
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
    </div>

    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard">
                <div id="content">
                    <div id="contentStandard1Column">
                        <div id="tabs">
                            <ul>
                                <li><a href="#tabs-1"><asp:Literal ID="lblTabNew" runat="server" /></a></li>
                                <li><a href="#tabs-2"><asp:Literal ID="lblTabStatus" runat="server" /></a></li>
                            </ul>
                            <div id="tabs-1">
                                <asp:UpdatePanel ID="box_upload" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <fieldset class="basic4">
                                            <div class="row">
                                                <div class="col-right"><img src="../Images/Icons/small_xls.png" alt="" /> <asp:HyperLink ID="lnkTemplate" runat="server" NavigateUrl="ImportPregressi.xls" Target="_blank" /></div>
                                                <div class="colHalf"><asp:Literal ID="lblFilename" runat="server" /></div>
                                                <div class="col"><input type="file" id="fileUpload" runat="server" onchange="controlla_estensione(this);"  /></div>
                                            </div>
                                            <br /><br />
                                            <div class="row">
                                                <div class="colHalf"><asp:Literal ID="lblDescription" runat="server" /></div>
                                                <div class="colHalf2"><cc1:CustomTextArea runat="server" ID="txtDescrizione" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" /></div>
                                            </div>
                                        </fieldset>
                                        <asp:Button ID="BtnUploadHidden" runat="server" CssClass="hidden" ClientIDMode="Static" OnClick="BtnUploadHidden_Click" OnClientClick="disallowOp('Content2');" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:PostBackTrigger ControlID="BtnUploadHidden" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel ID="upPnlReport" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlReport" runat="server" Visible="false">
                                            <fieldset class="basic4">
                                                <div class="topGrid">
                                                    <strong><asp:Label ID="lblAvviso" runat="server" CssClass="title"></asp:Label></strong>
                                                </div>
                                                <asp:Panel ID="pnlAvviso" runat="server" Visible="false">
                                                   <p><asp:Label ID="lbl_alert" runat="server"></asp:Label></p>
                                                </asp:Panel>
                                            </fieldset>
                                            <asp:GridView ID="grvItems" runat="server" AllowSorting="false" AutoGenerateColumns="false"
                                                AllowPaging="False" CssClass="tbl_rounded round_onlyextreme tbl_center"
                                                OnItemDataBound="ImageCreatedRender">
                                                <RowStyle CssClass="NormalRow" />
                                                <AlternatingRowStyle CssClass="AltRow" />
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-Width="5%">
                                                        <ItemTemplate>
                                                            <asp:Image ID="btn_periodo" runat="server" AlternateText='<%# this.GetTipoAvviso((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'
                                                                ImageUrl='<%# this.GetImageAvviso((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'
                                                                ToolTip='<%# this.GetTipoAvviso((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'>
                                                            </asp:Image>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="70%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="ERRORE" runat="server" Text='<%# this.GetErrore((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:Label ID="RIGAEXCEL" runat="server" Text='<%# this.GetLineaExcel((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div id="tabs-2">
                                <asp:UpdatePanel ID="upPanelReport" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="topGrid">
                                            <asp:Literal ID="titleReport" runat="server" />
                                        </div>
                                        <asp:GridView ID="gridReport" runat="server" AllowSorting="false" AutoGenerateColumns="false"
                                            AllowPaging="False" CssClass="tbl_rounded round_onlyextreme tbl_center" 
                                            OnRowDataBound="gridReport_RowDataBound">
                                                <RowStyle CssClass="NormalRow" />
                                                <AlternatingRowStyle CssClass="AltRow" />
                                            <Columns>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetReportID((NttDataWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="DESCR" runat="server" Text='<%# this.GetDescrizione((NttDataWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="DATAINIZIO" runat="server" Text='<%# this.GetDataInizio((NttDataWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="15%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="DATAFINE" runat="server" Text='<%# this.GetDataFine((NttDataWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="14%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="NDOC" runat="server" Text='<%# this.GetNumeroDocumenti((NttDataWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="14%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="PERC" runat="server" Text='<%# this.GetPercentuale((NttDataWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="20%">
                                                    <ItemTemplate>
                                                        <asp:Image runat="server" ID="ImgPerc" ImageUrl='<%# this.GetImmaginePercentuale((NttDataWA.DocsPaWR.ReportPregressi)Container.DataItem) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%">
                                                    <ItemTemplate>
                                                        <cc1:CustomImageButton ID="btn_dettagli" runat="server" CssClass="clickable" ImageUrl="../Images/Icons/ico_previous_details.png"
                                                            OnMouseOutImage="../Images/Icons/ico_previous_details.png" OnMouseOverImage="../Images/Icons/ico_previous_details_hover.png"
                                                            ImageUrlDisabled="../Images/Icons/ico_previous_details_disabled.png" OnClick="ViewDetails" Visible="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%">
                                                    <ItemTemplate>
                                                        <cc1:CustomImageButton ID="btn_Rimuovi" runat="server" CssClass="clickable" ImageUrl="../Images/Icons/delete.png"
                                                            OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                            ImageUrlDisabled="../Images/Icons/delete_disabled.png" OnClick="DeleteReport" Visible="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="8%">
                                                    <ItemTemplate>
                                                        <asp:Image ID="img_errore" runat="server"
                                                            AlternateText="Errore" ImageUrl='<%# this.GetImageErrore((NttDataWA.DocsPaWR.ReportPregressi)Container.DataItem) %>'
                                                            ToolTip='<%# this.GetNumeroDiErrori((NttDataWA.DocsPaWR.ReportPregressi)Container.DataItem) %>' Visible="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnImport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="$('#BtnUploadHidden').click();" />
            <cc1:CustomButton ID="BtnContinue" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnContinue_Click" OnClientClick="disallowOp('');" />
            <cc1:CustomButton ID="BtnNew" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnNew_Click" OnClientClick="disallowOp('');" />
            <cc1:CustomButton ID="BtnExport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnExport_Click" OnClientClick="disallowOp('');" />
            <cc1:CustomButton ID="BtnRefresh" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnRefresh_Click" OnClientClick="disallowOp('');" />
        </ContentTemplate>
    </asp:UpdatePanel>

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
