<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="AttachmentsUpload.aspx.cs" Inherits="NttDataWA.Popup.AttachmentsUpload" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        div#boxUploadMassivoAttachments {
            width: 100%;
            height: 100%;
            margin: 0;
            padding: 0;
        }

        table#boxFilesDetail p {
            margin: 5px;
        }

        table#boxFilesDetail .name {
           font-weight: bold;
        }

        table#boxFilesDetail .error {
           color: red;
           font-size: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="boxUploadMassivoAttachments">
        <div id="boxLegenda" style="height: 40%; position: absolute; width: 80%; left: 10%; top: 40%; text-align: justify;">
            <h3><asp:Literal ID="labelLegendaAcquisizioneMassivaAllegati" runat="server"></asp:Literal></h3>
    </div>
        <div>
            
            <input id="inputFileMassiveAttachments" type="file" name="files[]" multiple="multiple" style="display: none;" />
        </div>



        <div id="uploadAttachments" style="margin-top:-10px;">
            <div class="fileupload-buttonbar">
                <div class="fileupload-progressbar"></div>
            </div>

            <%--<div id="fileupload-content" class="fileupload-content">
                <table id="tableFiles" class="files"></table>
            </div>--%>
        </div>

        <table id="boxFilesDetail"style="width: 100%;height: 300px; display: block; margin-top: 5px; overflow-y: auto;">

        </table>

        <p id="infoUpload" style='font-weight:bold; text-align: center; display: none;'>Attendere prego</p>
        <p id="errorEndUpload" style='font-weight:bold; color: red; text-align: center; display: none;'>Errore nel processo di acquisizione</p>
        <p id="infoEndUpload" style='font-weight:bold; text-align: center; display: none;'>Operazione conclusa</p>
        <div style="position: absolute; bottom: 0;">
            <fieldset id="boxConversionOptions" style="border: 0;">
                <asp:CheckBoxList ID="conversionOption" runat="server" RepeatDirection="Horizontal" CssClass="chackList">
                    <asp:ListItem id="optPDF"></asp:ListItem>
                </asp:CheckBoxList>
            </fieldset>
        </div>
    </div>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <div style="float: left;">
        <div id="btnSelectAttachments" class="btnEnable" style="float: left;" >Seleziona</div>
        <div id="btnStartUploadMassivoAttachments" style="float: left;" class="btnDisable" >Acquisisci</div>
        <div id="btnClosePopUp" style="float: left;" class="btnEnable"  >Chiudi</div>
<%--        <cc1:CustomButton ID="btnClose" Text="chiudi" runat="server" CssClass="btnEnable" ClientIDMode="Static" OnMouseOver="btnHover" OnClick="btnClosePopUp_Click"/>--%>
    </div>
    

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
                <td class="progress"><div></div></td>
                <td class="start"><button>Start</button></td>
            {{/if}}
            <td class="cancel"><button>Cancel</button></td>
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
                        <a href="${url}" target="_blank"><img src="${thumbnail_url}"></a>
                    {{/if}}
                </td>
                <td class="name">
                    <a href="${url}"{{if thumbnail_url}} target="_blank"{{/if}}>${name}</a>
                </td>
                <td class="size">${sizef}</td>
                <td colspan="2"></td>
            {{/if}}
            <td class="delete">
                <button data-type="${delete_type}" data-url="${delete_url}">Delete</button>
            </td>
        </tr>
    </script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.tmpl.js?v=3") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.tmplPlus.js") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.iframe-transport.js") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.fileupload.js") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/jquery.fileupload-ui.js") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/UploadFile/UploadMassiveAttachments.js?v=79") %>" type="text/javascript"></script>
</asp:Content>
