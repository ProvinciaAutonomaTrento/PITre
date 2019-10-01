<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true" CodeBehind="ImportDocuments.aspx.cs" Inherits="NttDataWA.ImportDati.ImportDocuments" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {
            width: 95%;
            margin: 0 auto;
        }

        fieldset {
            min-height: 70px;
        }

        #tabs {
            background: #fff;
            border: 0;
            width: 98%;
        }

            #tabs ul {
                background: #fff;
                border-top: 0;
                border-left: 0;
                border-right: 0;
            }

            #tabs div {
                background: #fff;
            }

        
    </style>
    <script type="text/javascript">
        function get_estensione(path) {
            var posizione_punto = path.lastIndexOf(".");
            var lunghezza_stringa = path.length;
            var estensione = path.substring(posizione_punto + 1, lunghezza_stringa);
            return estensione;
        }

        function getNomeFile(path) {
            var nomefile = "";
            try {
                var posizione_barra = path.lastIndexOf("\\");
                nomefile = path.substring(posizione_barra + 1, path.length);
            } catch (error) {
                nomefile = "";
            }
            return nomefile;
        }

        function controlla_estensione(el) {
            if (get_estensione(el.value) != "xls") {
                alert("Il file deve avere estensione .xls");
                sostituisciInputFile(el);
            } else {
                $("#lblImportExcel").text(getNomeFile(el.value));
            }
        }

        function sostituisciInputFile(el) {
            return el.parentNode.replaceChild(el.cloneNode(true), el);
        }

        $(document).ready(function () {
            
            $("#btnUploadDocuments").click(function () {
                $("#inputFileDocumentsToUpload").click();
            });

            $("#btnUploadAttachmentsDinamic").click(function () {
                $("#inputFileAttachmentsToUpload").click();
            });

        });

    </script>
    <script id="template-upload" type="text/x-jquery-tmpl">
        <tr class="template-upload{{if error}} ui-state-error{{/if}}">
            <td class="preview"></td>
            <td class="name">${name}</td>
            <td class="size">${sizef}</td>
            {{if error}}
            <td class="error" colspan="2">Error:
                {{if error === 'maxFileSize'}}File is too big
                {{else error === 'minFileSize'}}File is too small
                {{else error === 'acceptFileTypes'}}Filetype not allowed
                {{else error === 'maxNumberOfFiles'}}Max number of files exceeded
                {{else}}${error}
                {{/if}}
            </td>
            {{else}}
            <td class="progress">
                <div></div>
            </td>
            <td class="start">
                <button>Start</button></td>
            {{/if}}
        <td class="cancel">
            <button>Cancel</button></td>
        </tr>
    </script>
    <script id="template-download" type="text/x-jquery-tmpl">
    <tr class="template-download{{if error}} ui-state-error{{/if}}">
        {{if error}}
            <td></td>
            <td class="name">${name}</td>
            <td class="size">${sizef}</td>
            <td class="error" colspan="2">Error:
                {{if error === 1}}File exceeds upload_max_filesize (php.ini directive)
                {{else error === 2}}File exceeds MAX_FILE_SIZE (HTML form directive)
                {{else error === 3}}File was only partially uploaded
                {{else error === 4}}No File was uploaded
                {{else error === 5}}Missing a temporary folder
                {{else error === 6}}Failed to write file to disk
                {{else error === 7}}File upload stopped by extension
                {{else error === 'maxFileSize'}}File is too big
                {{else error === 'minFileSize'}}File is too small
                {{else error === 'acceptFileTypes'}}Filetype not allowed
                {{else error === 'maxNumberOfFiles'}}Max number of files exceeded
                {{else error === 'uploadedBytes'}}Uploaded bytes exceed file size
                {{else error === 'emptyResult'}}Empty file upload result
                {{else}}${error}
                {{/if}}
            </td>
        {{else}}
            <td class="preview">
                {{if thumbnail_url}}
                    <!--<a href=".${url}" target="_blank">-->
                        <img src=".${thumbnail_url}" style="height: 25px;width: 25px;">

                    <!--</a>-->
                {{/if}}
            </td>
            <td class="name">
                ${name}
                <!-- <a href=".${url}"{{if thumbnail_url}} target="_blank"{{/if}}>${name}</a> -->
            </td>
            <td class="size">${sizef}</td>
            <td colspan="2"></td>
        {{/if}}
        <%--<td class="delete">
            <button data-type="${delete_type}" data-url="${delete_url}">Delete</button>
        </td>--%>
    </tr>
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
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
                                    <p>
                                        <asp:Label ID="pageTitle" runat="server" />
                                    </p>
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
                        <asp:UpdatePanel ID="box_upload" runat="server">
                            <ContentTemplate>

                                <asp:Button ID="BtnUploadHidden" runat="server" CssClass="hidden" ClientIDMode="Static" OnClick="BtnUploadHidden_Click" OnClientClick="disallowOp('Content2');" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="BtnUploadHidden" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <!-- MODELLO EXCEL-->
                        <div style="width: 100%;">
                            <div class="ui-widget">
                                <div class="ui-widget-header">
                                    <label class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false">
                                        <div>
                                            <img src="../Images/Icons/small_xls.png" alt="" />
                                            <asp:HyperLink ID="lnkTemplate" runat="server" NavigateUrl="ImportDocumenti.xls" Target="_blank" />
                                        </div>
                                    </label>
                                    <label class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" aria-disabled="false">
                                        <span class="ui-button-icon-primary ui-icon ui-icon-folder-open"></span><span class="ui-button-text">
                                            <span>Carica modello</span>
                                            <input type="file" id="fileUpload" runat="server" style="display: none;" onchange="controlla_estensione(this);" />
                                        </span>
                                    </label>
                                    <label>
                                        <span id="lblImportExcel"></span>
                                        <asp:Literal ID="lblFilename" Visible="false" runat="server" />
                                    </label>

                                    <label>
                                        <a id="lnkDownload" runat="server" href="ZipDocumenti.aspx" visible="false">
                                            <img src="../Images/Icons/download_big.png" alt="" /></a>
                                    </label>
                                </div>
                            </div>
                        </div>

                        <!-- UPLOAD -->
                        <div id="boxUploadDocumenti" runat="server" clientidmode="Static" style="width: 100%;">
                            <!-- DOCUMENTI-->
                            <div id="boxDinamicUploadDocuments" style="width: 50%; float:left;">
                                <div id="fileupload_box" style="margin: 5px;">
                                    <div class="fileupload-buttonbar" style="background-position-y: top; background-size: auto; background-color: white !important;">
                                        <label id="btnUploadDocuments" class="fileinput-button">
                                            <span>Carica Documenti</span>
                                        </label>
                                        <div class="fileupload-progressbar" style="margin: 15px 10px 5px 10px"></div>
                                    </div>
                                    <div class="fileupload-content" style="margin: 0 7px 0 5px;">
                                        <table class="files"></table>
                                    </div>
                                </div>
                            </div>
                            <!-- FINE DOCUMENTI  -->

                            <!-- ALLEGATI  -->
                            <div id="boxDinamicUploadAttachments" style="width: 50%; float:left;;">
                                <div id="attachmentsUpload_box" style="margin: 5px;" >
                                    <div class="fileupload-buttonbar" style="background-position-y: top; background-size: auto; background-color: white !important;">
                                        <label id="btnUploadAttachmentsDinamic" class="fileinput-button">
                                            <span>Carica Allegati</span>
                                        </label>
                                        <div class="fileupload-progressbar" style="margin: 15px 10px 5px 10px"></div>
                                    </div>
                                    <div class="fileupload-content" style="margin: 0 7px 0 5px;">
                                        <table class="files"></table>
                                    </div>
                                </div>
                            </div>

                            <!-- FINE ALLEGATI -->
                            <div style="clear:left;"></div>
                        </div>
                        <!-- FINE UPLOAD -->


                        <asp:UpdatePanel ID="upReport" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:PlaceHolder ID="plcReport" runat="server" Visible="false">
                                    <div id="tabs">
                                        <ul>
                                            <li><a href="#tabs-1">
                                                <asp:Literal ID="lblTabGeneral" runat="server"></asp:Literal></a></li>
                                            <asp:PlaceHolder ID="plcLiArrive" runat="server">
                                                <li><a href="#tabs-2">
                                                    <asp:Literal ID="lblTabArrive" runat="server"></asp:Literal></a></li>
                                            </asp:PlaceHolder>
                                            <li><a href="#tabs-3">
                                                <asp:Literal ID="lblTabLeaving" runat="server"></asp:Literal></a></li>
                                            <li><a href="#tabs-4">
                                                <asp:Literal ID="lblTabInternal" runat="server"></asp:Literal></a></li>
                                            <li><a href="#tabs-5">
                                                <asp:Literal ID="lblTabGray" runat="server"></asp:Literal></a></li>
                                            <asp:PlaceHolder ID="plcLiAttachments" runat="server">
                                                <li><a href="#tabs-6">
                                                    <asp:Literal ID="lblTabAttachments" runat="server"></asp:Literal></a></li>
                                            </asp:PlaceHolder>
                                            <li><a href="#tabs-7">
                                                <asp:Literal ID="lblTabReportPdf" runat="server"></asp:Literal></a></li>
                                        </ul>
                                        <div id="tabs-1">
                                            <!-- generale -->
                                            <asp:GridView ID="grdGenerale" runat="server" ShowHeader="true" AutoGenerateColumns="False" CssClass="tbl_rounded round_onlyextreme">
                                                <RowStyle CssClass="NormalRow" />
                                                <AlternatingRowStyle CssClass="AltRow" />
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemStyle HorizontalAlign="Center" Width="3%" />
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlOrdinalNumber" runat="server" Text="<%#this.GetOrdinalNumber((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Message" ItemStyle-Width="45%" />
                                                    <asp:TemplateField>
                                                        <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="45%">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                        <asp:PlaceHolder ID="plcArrive" runat="server">
                                            <div id="tabs-2">
                                                <!-- arrivo -->
                                                <asp:GridView ID="grdArrivo" runat="server" ShowHeader="true" AutoGenerateColumns="False" CssClass="tbl_rounded round_onlyextreme">
                                                    <RowStyle CssClass="NormalRow" />
                                                    <AlternatingRowStyle CssClass="AltRow" />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-Width="3%">
                                                            <ItemStyle HorizontalAlign="Center" />
                                                            <ItemTemplate>
                                                                <asp:Literal ID="ltlOrdinalNumber" runat="server" Text="<%#this.GetOrdinalNumber((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Message" ItemStyle-Width="45%" />
                                                        <asp:TemplateField ItemStyle-Width="7%">
                                                            <ItemStyle HorizontalAlign="Center" />
                                                            <ItemTemplate>
                                                                <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField ItemStyle-Width="45%">
                                                            <ItemTemplate>
                                                                <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </asp:PlaceHolder>
                                        <div id="tabs-3">
                                            <!-- partenza -->
                                            <asp:GridView ID="grdPartenza" runat="server" ShowHeader="true" AutoGenerateColumns="False" CssClass="tbl_rounded round_onlyextreme">
                                                <RowStyle CssClass="NormalRow" />
                                                <AlternatingRowStyle CssClass="AltRow" />
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-Width="3%">
                                                        <ItemStyle HorizontalAlign="Center" />
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlOrdinalNumber" runat="server" Text="<%#this.GetOrdinalNumber((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Message" ItemStyle-Width="45%" />
                                                    <asp:TemplateField ItemStyle-Width="7%">
                                                        <ItemStyle HorizontalAlign="Center" />
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="45%">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                        <div id="tabs-4">
                                            <!-- interni -->
                                            <asp:GridView ID="grdInterni" runat="server" ShowHeader="true" AutoGenerateColumns="False" CssClass="tbl_rounded round_onlyextreme">
                                                <RowStyle CssClass="NormalRow" />
                                                <AlternatingRowStyle CssClass="AltRow" />
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-Width="3%">
                                                        <ItemStyle HorizontalAlign="Center" />
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlOrdinalNumber" runat="server" Text="<%#this.GetOrdinalNumber((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Message" ItemStyle-Width="45%" />
                                                    <asp:TemplateField ItemStyle-Width="7%">
                                                        <ItemStyle HorizontalAlign="Center" />
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="45%">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                        <div id="tabs-5">
                                            <!-- non protocollati -->
                                            <asp:GridView ID="grdGrigi" runat="server" ShowHeader="true" AutoGenerateColumns="False" CssClass="tbl_rounded round_onlyextreme">
                                                <RowStyle CssClass="NormalRow" />
                                                <AlternatingRowStyle CssClass="AltRow" />
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-Width="3%">
                                                        <ItemStyle HorizontalAlign="Center" />
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlOrdinalNumber" runat="server" Text="<%#this.GetOrdinalNumber((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Message" ItemStyle-Width="45%" />
                                                    <asp:TemplateField ItemStyle-Width="7%">
                                                        <ItemStyle HorizontalAlign="Center" />
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="45%">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                        <asp:PlaceHolder ID="plcAttachments" runat="server">
                                            <div id="tabs-6">
                                                <!-- allegati -->
                                                <asp:GridView ID="grdAllegati" runat="server" ShowHeader="true" AutoGenerateColumns="False" CssClass="tbl_rounded round_onlyextreme">
                                                    <RowStyle CssClass="NormalRow" />
                                                    <AlternatingRowStyle CssClass="AltRow" />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-Width="3%">
                                                            <ItemStyle HorizontalAlign="Center" />
                                                            <ItemTemplate>
                                                                <asp:Literal ID="ltlOrdinalNumber" runat="server" Text="<%#this.GetOrdinalNumber((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Message" ItemStyle-Width="45%" />
                                                        <asp:TemplateField>
                                                            <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                            <ItemTemplate>
                                                                <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField ItemStyle-Width="45%">
                                                            <ItemTemplate>
                                                                <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </asp:PlaceHolder>
                                        <div id="tabs-7">
                                            <!-- report pdf -->
                                            <iframe src="../Popup/MassiveReport.aspx" id="frame" width="100%" frameborder="0"></iframe>
                                            <script type="text/javascript">
                                                function resizePrintIframe() {
                                                    var height = document.documentElement.clientHeight;
                                                    height -= 90; /* whatever you set your body bottom margin/padding to be */
                                                    document.getElementById('frame').style.height = height + "px";
                                                };

                                                $(function () {
                                                    resizePrintIframe();
                                                });
                                            </script>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                            </ContentTemplate>
                        </asp:UpdatePanel>
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
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
